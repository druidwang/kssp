
INSERT [dbo].[SYS_Menu] ([Code], [Name], [Parent], [Seq], [Desc1], [PageUrl], [ImageUrl], [IsActive]) VALUES (N'Url_Inventory_InMiscOrder_InitInventory', N'Inventory_InMiscOrder_InitInventory', N'Url_Inventory_InMiscOrder',163 , N'库存管理-事务-计划外入库-库存初始化', N'~/InMiscOrder/InitInventory', N'~/Content/Images/Nav/Default.png', 1);
insert into ACC_Permission(Code,Desc1,Category,Sequence) values ('Url_Inventory_InMiscOrder_InitInventory',N'库存管理-事务-计划外入库-库存初始化','Menu_Inventory',1)




/****** Object:  StoredProcedure [dbo].[USP_Report_GetInventoryMonitor]    Script Date: 2017/4/20 9:49:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:  <Author,,Name>
-- Create date: <Create Date,,>
-- Description: <Description,,>
--[USP_Report_GetInventoryMonitor] '30023','','',''
-- =============================================
Create PROCEDURE [dbo].[USP_Report_GetInventoryMonitor]
(
 @Location varchar(50),
 @Item varchar(50),
 @itemCategory varchar(50),
 @materialsGroup varchar(50),
 @InventoryType varchar(10)
)
AS
--declare 
-- @Location varchar(50),
-- @itemCategory varchar(50),
-- @materialsGroup varchar(50)
--select * from ORD_OrderDet_1
 
 
BEGIN
 SET NOCOUNT ON;
 SELECT @Location=LTRIM(RTRIM(@Location)),@itemCategory=LTRIM(RTRIM(@itemCategory)),@materialsGroup=LTRIM(RTRIM(@materialsGroup))
 ----Filter column "item" by @itemCategory&@materialsGroup to imrove performance.
 select code as item into #item  from md_item with(nolock) where itemCategory like 
	'%'+isnull(@ItemCategory,'')+'%' and MaterialsGroup like '%'+isnull(@MaterialsGroup,'')+'%'
	and Code like isnull(@Item,'')+'%'
 
----当传入的库位参数@Location为Null时，最后的返回记录集的表#WIP&#safestock#stock，会出现一个Item对应多个Loaction ,这样在最后会出现库存和差额将不正确
----这样Join的时候会出现数据混乱，所以在最开始产生记录集表的时候就舍掉栏位@Location
Declare @LocNull varchar(2)='N'
IF isnull(@Location,'')=''
	Begin
		Set @LocNull='Y'
	End
----在途
 select a.Item,case when @LocNull='Y' then '' else a.LocTo end as LocTo,SUM(IpQty) as recqty into #WIP from 
 (
  select a.Item,a.LocTo, sum((a.Qty - a.RecQty)*a.UnitQty) as IpQty  from
  ORD_IpDet_2 a with(nolock)  where  a.IsClose =0 and QualityType <>2 and ABS(Qty)>ABS(RecQty)
  Group by a.Item,a.LocTo
  union all
  select a.Item,a.LocTo,sum((a.Qty - a.RecQty)*a.UnitQty) as IpQty  from
  ORD_IpDet_7 a with(nolock)  where  a.IsClose =0 and QualityType <>2 and ABS(Qty)>ABS(RecQty)
  Group by a.Item ,a.LocTo
  union all
  select a.Item,a.LocTo,sum((a.Qty - a.RecQty)*a.UnitQty) as IpQty  from
  ORD_IpDet_1 a with(nolock)  where  a.IsClose =0 and QualityType <>2 and ABS(Qty)>ABS(RecQty)
  Group by a.Item ,a.LocTo
 )  a where a.item in (select item from #item)  
      and a.LocTo like '%'+isnull(@Location,'')+'%' 
      Group by a.Item,case when @LocNull='Y' then '' else a.LocTo end
----SafeStockQty one item matches only one flowdet data
 select a.item,case when @LocNull='Y' then '' else a.location end as location,a.safestock,a.maxstock into #safestock from
 (select a.item,isnull(a.locto, b.locto) as location,isnull(a.safestock,0) as safestock , 
 isnull(a.maxstock,0) as maxstock,row_number()over(partition by a.item order by a.maxstock desc) as Seq from
 scm_flowdet a with(nolock),scm_flowmstr b with(nolock) where a.flow = b.code and b.IsActive = 1 and isnull(a.locto, b.locto)
 like '%'+isnull(@Location,'')+'%')a where a.Seq=1 and a.item in (select item from #item) 
 
----TotalStockQty
--select top 100 * from VIEW_LocationDet
 select item,case when @LocNull='Y' then '' else location end as location,sum(qty) as qty,SUM(QualifyQty)QualifyQty ,
	sum(RejectQty)RejectQty ,SUM(FreezeQty)FreezeQty,SUM(qty)-SUM(ATPQty) as validQty,sum(CsQty)CsQty  into #stock from VIEW_LocationDet_IncludeZeroQty with(nolock) 
	where item in (select item from #item) 
	and location like '%'+isnull(@Location,'')+'%'group by item,case when @LocNull='Y' then '' else location end
----Recordset
 select a.location as 库位,a.item as 物料,d.desc1+case when Isnull(RefCode,'')='' then '' else '['+RefCode+']' end as 描述,d.uom as 单位,
	cast(a.qty as numeric(12,2))as 总库存,
	cast(a.validQty as numeric(12,2))as 无效库存,
	cast(a.QualifyQty as numeric(12,2))as 合格数,
	cast(a.RejectQty as numeric(12,2))as 不合格数,
	cast(a.FreezeQty as numeric(12,2))as 冻结数,
	cast(a.CsQty as numeric(12,2))as 寄售库存,
	cast(isnull(b.safestock,0) as numeric(12,2)) as 安全库存,
	cast(isnull(b.maxstock,0) as numeric(12,2)) as 最大库存,
	cast(isnull(recqty,0) as numeric(12,2)) as 在途,
	cast(a.qty-isnull(b.safestock,0) + isnull(recqty,0) as numeric(12,2))as 安全差额,
	cast(a.qty-isnull(b.maxstock,0) + isnull(recqty,0) as numeric(12,2))as 最大差额 into #record
	--cast(cast((a.qty-b.maxstock + isnull(recqty,0))/a.qty  as numeric(12,2))*100 as varchar)+'%' as 百分比
	from #stock a left join #safestock b on b.item=a.item left join #WIP c on c.item=a.item 
	left join MD_Item d on d.code=a.item
----@Location =null 时要去掉@Location的列并汇总各个库存的数量
----1.需要按照百分比,最大库存差额排序
----2.最大库存为0的不要显示百分比


If @LocNull='N'
	Begin
	--SP_help'MD_Item'
		select a.Code as 物料,b.Code 物料组,b.Desc1 物料组描述 into #ItemCategory from MD_Item a with(nolock) , MD_ItemCategory b with(nolock) where a.MaterialsGroup =b.Code 
				and b.SubCategory=5 and a.Code in (select 物料 from #record)
		if(@InventoryType ='0')
		begin	
			select 库位,a.物料,描述,ISNULL(b.物料组,'') 物料组,ISNULL(b.物料组描述,'')物料组描述,单位,总库存,无效库存,合格数,不合格数,冻结数,寄售库存,安全库存,最大库存,在途,case when isnull(安全差额,0)>0 then 0 else 安全差额 end as 安全差额,case when isnull(最大差额,0)<0 then 0 else 最大差额 end as 最大差额,
			case when 最大库存=0 then '' else cast(cast(isnull(最大差额,0)/最大库存  as numeric(12,2))*100 as varchar)+'%' end as 百分比,
			case when isnull(安全差额,0)>0 then 0 else 安全差额 end as seq1
			from #record a left join #ItemCategory b on a.物料=b.物料 
			order by seq1,最大差额
		end
		else if(@InventoryType ='1')
		begin
			select 库位,a.物料,描述,ISNULL(b.物料组,'') 物料组,ISNULL(b.物料组描述,'')物料组描述,单位,总库存,无效库存,合格数,不合格数,冻结数,寄售库存,安全库存,最大库存,在途,case when isnull(安全差额,0)>0 then 0 else 安全差额 end as 安全差额,case when isnull(最大差额,0)<0 then 0 else 最大差额 end as 最大差额,
			case when 最大库存=0 then '' else cast(cast(isnull(最大差额,0)/最大库存  as numeric(12,2))*100 as varchar)+'%' end as 百分比,
			case when isnull(安全差额,0)>0 then 0 else 安全差额 end as seq1
			from #record a left join #ItemCategory b on a.物料=b.物料 where 总库存=0 and 安全库存=0 and 最大库存=0 
			order by seq1,最大差额			
		end
		else
		begin
			select 库位,a.物料,描述,ISNULL(b.物料组,'') 物料组,ISNULL(b.物料组描述,'')物料组描述,单位,总库存,无效库存,合格数,不合格数,冻结数,寄售库存,安全库存,最大库存,在途,case when isnull(安全差额,0)>0 then 0 else 安全差额 end as 安全差额,case when isnull(最大差额,0)<0 then 0 else 最大差额 end as 最大差额,
			case when 最大库存=0 then '' else cast(cast(isnull(最大差额,0)/最大库存  as numeric(12,2))*100 as varchar)+'%' end as 百分比,
			case when isnull(安全差额,0)>0 then 0 else 安全差额 end as seq1
			from #record a left join #ItemCategory b on a.物料=b.物料 where 总库存!=0 or 安全库存!=0 or 最大库存!=0 
			order by seq1,最大差额	
		end
	End
Else
	Begin
		select 物料,描述,单位,sum(总库存)as 总库存,sum(无效库存)无效库存,sum(合格数)合格数,sum(不合格数)不合格数,sum(冻结数)冻结数,sum(寄售库存)寄售库存,sum(安全库存) as 安全库存,sum(最大库存) as 最大库存,
			sum(在途)as 在途,sum(isnull(安全差额,0))as 安全差额,sum(isnull(最大差额,0))as 最大差额 into #record1 from #record 
			group by 物料,描述,单位

		--select item ,count(location) as LocQty into #stock1 from #stock group by item
		select a.Code as 物料,b.Code 物料组,b.Desc1 物料组描述 into #ItemCategory1 from MD_Item a with(nolock) , MD_ItemCategory b with(nolock) where a.MaterialsGroup =b.Code 
			and b.SubCategory=5 and a.Code in (select 物料 from #record)
		--select '' as 库位,a.物料,描述,ISNULL(b.物料组,'') 物料组,ISNULL(b.物料组描述,'')物料组描述,单位,总库存 as 总库存,无效库存,合格数,不合格数,冻结数,寄售库存,安全库存 as 安全库存,最大库存 as 最大库存,
		--	在途 as 在途, isnull(安全差额,0) as 安全差额,isnull(最大差额,0)  as 最大差额,
		--	case when 最大库存=0 then '' else cast(cast(isnull(最大差额,0)/最大库存  as numeric(12,2))*100 as varchar)+'%' end as 百分比,
		--	case when isnull(安全差额,0)>0 then 0 else 安全差额 end as seq1
		--	from #record1 a left join #ItemCategory1 b on a.物料=b.物料  
		--	order by seq1,最大差额

		if(@InventoryType ='0')
		begin
			select '' as 库位,a.物料,描述,ISNULL(b.物料组,'') 物料组,ISNULL(b.物料组描述,'')物料组描述,单位,总库存 as 总库存,无效库存,合格数,不合格数,冻结数,寄售库存,安全库存 as 安全库存,最大库存 as 最大库存,
			在途 as 在途, isnull(安全差额,0) as 安全差额,isnull(最大差额,0)  as 最大差额,
			case when 最大库存=0 then '' else cast(cast(isnull(最大差额,0)/最大库存  as numeric(12,2))*100 as varchar)+'%' end as 百分比,
			case when isnull(安全差额,0)>0 then 0 else 安全差额 end as seq1
			from #record1 a left join #ItemCategory1 b on a.物料=b.物料  
			order by seq1,最大差额
		end
		else if(@InventoryType ='1')
		begin
			select '' as 库位,a.物料,描述,ISNULL(b.物料组,'') 物料组,ISNULL(b.物料组描述,'')物料组描述,单位,总库存 as 总库存,无效库存,合格数,不合格数,冻结数,寄售库存,安全库存 as 安全库存,最大库存 as 最大库存,
			在途 as 在途, isnull(安全差额,0) as 安全差额,isnull(最大差额,0)  as 最大差额,
			case when 最大库存=0 then '' else cast(cast(isnull(最大差额,0)/最大库存  as numeric(12,2))*100 as varchar)+'%' end as 百分比,
			case when isnull(安全差额,0)>0 then 0 else 安全差额 end as seq1
			from #record1 a left join #ItemCategory1 b on a.物料=b.物料 where 总库存=0 and 安全库存=0 and 最大库存=0 
			order by seq1,最大差额		
		end
		else
		begin
			select '' as 库位,a.物料,描述,ISNULL(b.物料组,'') 物料组,ISNULL(b.物料组描述,'')物料组描述,单位,总库存 as 总库存,无效库存,合格数,不合格数,冻结数,寄售库存,安全库存 as 安全库存,最大库存 as 最大库存,
			在途 as 在途, isnull(安全差额,0) as 安全差额,isnull(最大差额,0)  as 最大差额,
			case when 最大库存=0 then '' else cast(cast(isnull(最大差额,0)/最大库存  as numeric(12,2))*100 as varchar)+'%' end as 百分比,
			case when isnull(安全差额,0)>0 then 0 else 安全差额 end as seq1
			from #record1 a left join #ItemCategory1 b on a.物料=b.物料 where 总库存!=0 or 安全库存!=0 or 最大库存!=0 
			order by seq1,最大差额		
		end
	End

--select @Location ,'物料',' 描述',' 单位 ',100.0M,200.0M,300.0M,120.0M,-10.0M,30.0M,'10%'
 
END




/****** Object:  StoredProcedure [dbo].[USP_Report_RealTimeLocationDet]    Script Date: 2017/4/20 10:34:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_Report_RealTimeLocationDet]
(
	@Locations varchar(8000),
	@Items varchar(8000),
	@SortDesc varchar(100),
	@PageSize int,
	@Page int,
	@SummaryLevel int,
	@IsGroupByManufactureParty bit,
	@IsGroupByLotNo bit,
	@IsSummaryBySAPLoc bit,
	@IsOnlyShowQtyInv bit

)
AS
BEGIN
/*
exec USP_Report_RealTimeLocationDet '1002','','',100,1,1,0,1,1,1
500368184
--2014/08/01  在途分销售在途和移库在途，全部算到来源库位	--0001
--2014/09/24  去掉实时库存为0的		--0002
*/
    SELECT @Locations=LTRIM(RTRIM(@Locations)),@Items=LTRIM(RTRIM(@Items))
	SET NOCOUNT ON
	DECLARE @Sql varchar(max)
	DECLARE @Where  varchar(8000)
	DECLARE @PartSuffix varchar(5)
	DECLARE @PagePara varchar(8000)
	DECLARE @TmpForLoop int
	SELECT @Sql='',@TmpForLoop=0,@Where=''
	DECLARE @LocationIds table(Id int identity(1,1),PartSuffix varchar(5))
	CREATE TABLE #TempResult
	(
		rowid int,
		Location varchar(50), 
		Item varchar(50), 
		LotNo varchar(50),
		ManufactureParty varchar(50), 
		Qty decimal(18,8), 
		CsQty decimal(18,8), 
		QualifyQty decimal(18,8), 
		InspectQty decimal(18,8), 
		RejectQty decimal(18,8),  
		ATPQty decimal(18,8), 
		FreezeQty decimal(18,8)		
	)
	CREATE TABLE #TempInternal
	(
		Location varchar(50), 
		Item varchar(50), 
		LotNo varchar(50),
		ManufactureParty varchar(50), 
		Qty decimal(18,8), 
		CsQty decimal(18,8), 
		QualifyQty decimal(18,8), 
		InspectQty decimal(18,8), 
		RejectQty decimal(18,8),  
		ATPQty decimal(18,8), 
		FreezeQty decimal(18,8)
	)
	CREATE TABLE #TempTransQty
	(
		Item varchar(50), 
		Location varchar(50), 
		Qty decimal(18,8), 
		QualifyQty decimal(18,8), 
		RejectQty decimal(18,8)
	)
	CREATE TABLE #SalesTempTransQty
	(
		Item varchar(50), 
		Location varchar(50), 
		Qty decimal(18,8), 
		QualifyQty decimal(18,8), 
		RejectQty decimal(18,8)
	)	
	--如果有输入的库位则只查询输入库位的表，否则全部表拼接查询
	IF ISNULL(@Locations,'')='' 
	BEGIN
		INSERT INTO @LocationIds(PartSuffix)
		SELECT DISTINCT(PartSuffix) FROM MD_Location WHERE PartSuffix IS NOT NULL OR PartSuffix<>''
	END
	ELSE
	BEGIN
		--0002
		Select @Locations=@Locations+','+Code from MD_location where SAPLocation in(Select * from dbo.Func_SplitStr(@Locations,','))
		--0002
		SET @Where=' WHERE det.Location in('''+replace(@Locations,',',''',''')+''')'
	    SET @sql='SELECT DISTINCT PartSuffix FROM MD_Location WHERE Code in ('''+replace(@Locations,',',''',''')+''') or SAPLocation in ('''+replace(@Locations,',',''',''')+''')'
		 PRINT @Locations
		INSERT INTO @LocationIds(PartSuffix)
		EXEC(@sql)
	END
	
	---查询出数据时需要的条件
	-----物料
	IF ISNULL(@Items,'')<>'' 
	BEGIN
		IF ISNULL(@Where,'')=''
		BEGIN
			SET @Where=' WHERE det.Item IN ('''+replace(@Items,',',''',''')+''')'
		END
		ELSE
		BEGIN
			SET @Where=@Where+' AND det.Item IN ('''+replace(@Items,',',''',''')+''')'
		END
	END
	
	IF @IsOnlyShowQtyInv=1 AND @IsGroupByLotNo=0
	BEGIN
		IF ISNULL(@Where,'')=''
		BEGIN
			SET @Where=' WHERE det.LotNo is null '
		END
		ELSE
		BEGIN
			SET @Where=@Where+' AND det.LotNo is null '
		END
	END
	
	ELSE IF @IsGroupByLotNo=1 AND @IsOnlyShowQtyInv=0
	BEGIN
		IF ISNULL(@Where,'')=''
		BEGIN
			SET @Where=' WHERE det.LotNo is not null '
		END
		ELSE
		BEGIN
			SET @Where=@Where+' AND det.LotNo is not null '
		END
	END
	
	IF @IsGroupByLotNo=0 
	BEGIN
		IF @IsSummaryBySAPLoc=0
		BEGIN
		--移库
			insert into #TempTransQty
			select a.Item as Item,/*a.LocTo*/a.LocFrom as Location,
			sum((Qty - RecQty)*UnitQty) As Qty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=0 then 1 else 0 end) As QualifyQty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=2 then 1 else 0 end) As RejectQty
			from ORD_IpDet_2 a  where a.IsClose =0 and ABS(Qty)>ABS(RecQty) 
			group by a.Item,/*a.LocTo*/a.LocFrom
		--委外移库
			insert into #TempTransQty
			select a.Item as Item,/*a.LocTo*/a.LocFrom as Location,
			sum((Qty - RecQty)*UnitQty) As Qty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=0 then 1 else 0 end) As QualifyQty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=2 then 1 else 0 end) As RejectQty
			from ORD_IpDet_7 a  where a.IsClose =0 and ABS(Qty)>ABS(RecQty) 
			group by a.Item,/*a.LocTo*/a.LocFrom
		--销售
			insert into #SalesTempTransQty
			select a.Item as Item,/*a.LocTo*/a.LocFrom as Location,
			sum((Qty - RecQty)*UnitQty) As Qty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=0 then 1 else 0 end) As QualifyQty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=2 then 1 else 0 end) As RejectQty
			from ORD_IpDet_3 a  where a.IsClose =0 and ABS(Qty)>ABS(RecQty) 
			group by a.Item,/*a.LocTo*/a.LocFrom
		END
		ELSE
		BEGIN
		--移库
			insert into #TempTransQty
			select a.Item as Item,l.SAPLocation as Location,
			sum((Qty - RecQty)*UnitQty) As Qty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=0 then 1 else 0 end) As QualifyQty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=2 then 1 else 0 end) As RejectQty
			from ORD_IpDet_2 a 
			join MD_Location l on /*a.LocTo*/a.LocFrom = l.Code
			where a.IsClose =0 and ABS(Qty)>ABS(RecQty) 
			group by a.Item,l.SAPLocation
		--委外移库
			insert into #TempTransQty
			select a.Item as Item,l.SAPLocation as Location,
			sum((Qty - RecQty)*UnitQty) As Qty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=0 then 1 else 0 end) As QualifyQty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=2 then 1 else 0 end) As RejectQty
			from ORD_IpDet_7 a 
			join MD_Location l on /*a.LocTo*/a.LocFrom = l.Code
			where a.IsClose =0 and ABS(Qty)>ABS(RecQty) 
			group by a.Item,l.SAPLocation
		--销售
			insert into #SalesTempTransQty
			select a.Item as Item,l.SAPLocation as Location,
			sum((Qty - RecQty)*UnitQty) As Qty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=0 then 1 else 0 end) As QualifyQty,
			sum((Qty - RecQty)*UnitQty*Case when QualityType=2 then 1 else 0 end) As RejectQty
			from ORD_IpDet_3 a 
			join MD_Location l on /*a.LocTo*/a.LocFrom = l.Code
			where a.IsClose =0 and ABS(Qty)>ABS(RecQty) 
			group by a.Item,l.SAPLocation
		END
	END
	
	--PRINT @Where
	--select * from @LocationIds
	---排序条件
	IF ISNULL(@SortDesc,'')=''
	BEGIN
		SET @SortDesc=' ORDER BY Item ASC'
	END
		
	---查询出结果时需要的条件
	IF @Page>0
	BEGIN
		SET @PagePara='WHERE rowid BETWEEN '+cast(@PageSize*(@Page-1)+1 as varchar(50))+' AND '++cast(@PageSize*(@Page) as varchar(50))
	END

	DECLARE @MaxId int
	SELECT @MaxId = MAX(Id),@Sql='' FROM @LocationIds
	WHILE @TmpForLoop<@MaxId
	BEGIN
		SET @TmpForLoop=@TmpForLoop+1
		SELECT @PartSuffix=PartSuffix FROM @LocationIds WHERE Id=@TmpForLoop
		PRINT @TmpForLoop
		IF 	@IsGroupByManufactureParty=0 AND @IsGroupByLotNo=0
		BEGIN
			SET @Sql='SELECT det.Location, det.Item,'''' as LotNo,'''' as ManufactureParty,
					SUM(det.Qty) AS Qty, 
					SUM(CASE WHEN det.IsCS = 1 THEN det.Qty ELSE 0 END) AS CSQty, 					
                    SUM(CASE WHEN det.QualityType = 0 THEN det.Qty ELSE 0 END) AS QualifyQty, 
                    SUM(CASE WHEN det.QualityType = 1 THEN det.Qty ELSE 0 END) AS InspectQty, 
                    SUM(CASE WHEN det.QualityType = 2 THEN det.Qty ELSE 0 END) AS RejectQty, 
                    SUM(CASE WHEN det.IsATP = 1 THEN det.Qty ELSE 0 END) AS ATPQty, 
                    SUM(CASE WHEN det.IsFreeze = 1 THEN det.Qty ELSE 0 END) AS FreezeQty
					FROM  inv_locationlotdet_'+@PartSuffix+' AS det '+@Where+' 
					GROUP BY det.Location, det.Item'
		END	
		ELSE IF @IsGroupByManufactureParty=1 AND @IsGroupByLotNo=0
		BEGIN
			SET @Sql='SELECT det.Location, det.Item,'''' AS LotNo, 
					CASE WHEN bill.Party IS NOT NULL THEN bill.Party ELSE hu.ManufactureParty END AS ManufactureParty,
					SUM(det.Qty) AS Qty,
					SUM(CASE WHEN det.IsCS = 1 THEN det.Qty ELSE 0 END) AS CSQty,
                    SUM(CASE WHEN det.QualityType = 0 THEN det.Qty ELSE 0 END) AS QualifyQty, 
                    SUM(CASE WHEN det.QualityType = 1 THEN det.Qty ELSE 0 END) AS InspectQty, 
                    SUM(CASE WHEN det.QualityType = 2 THEN det.Qty ELSE 0 END) AS RejectQty, 
                    SUM(CASE WHEN det.IsATP = 1 THEN det.Qty ELSE 0 END) AS ATPQty, 
                    SUM(CASE WHEN det.IsFreeze = 1 THEN det.Qty ELSE 0 END) AS FreezeQty
					FROM  inv_locationlotdet_'+@PartSuffix+' AS det 
					LEFT JOIN dbo.INV_Hu AS hu ON det.HuId = hu.HuId  
					LEFT  JOIN BIL_PlanBill AS bill ON det.PlanBill=bill.id AND bill.Type=0 '+@Where+' 
					GROUP BY det.Location, det.Item,CASE WHEN bill.Party IS NOT NULL THEN bill.Party ELSE hu.ManufactureParty END '			
		END	
		ELSE IF @IsGroupByManufactureParty=0 AND @IsGroupByLotNo=1
		BEGIN
			SET @Sql='SELECT det.Location, det.Item, det.LotNo,'''' as ManufactureParty, 
					SUM(det.Qty) AS Qty, 
					SUM(CASE WHEN det.IsCS = 1 THEN det.Qty ELSE 0 END) AS CSQty,
                    SUM(CASE WHEN det.QualityType = 0 THEN det.Qty ELSE 0 END) AS QualifyQty, sum(CASE WHEN det.QualityType = 1 THEN det.Qty ELSE 0 END) AS InspectQty, 
                    SUM(CASE WHEN det.QualityType = 2 THEN det.Qty ELSE 0 END) AS RejectQty, sum(CASE WHEN det.IsATP = 1 THEN det.Qty ELSE 0 END) AS ATPQty, 
                    SUM(CASE WHEN det.IsFreeze = 1 THEN det.Qty ELSE 0 END) AS FreezeQty
					FROM  inv_locationlotdet_'+@PartSuffix+' AS det LEFT JOIN
					dbo.INV_Hu AS hu ON det.HuId = hu.HuId '+@Where+' and det.Qty<>0
					GROUP BY det.Location, det.Item, det.LotNo'		
		END			
		ELSE IF @IsGroupByManufactureParty=1 AND @IsGroupByLotNo=1	
		BEGIN
			SET @Sql='SELECT det.Location, det.Item,det.LotNo, 
					CASE WHEN bill.Party IS NOT NULL THEN bill.Party ELSE hu.ManufactureParty END AS ManufactureParty,
					SUM(det.Qty) AS Qty, 
					SUM(CASE WHEN det.IsCS = 1 THEN det.Qty ELSE 0 END) AS CSQty,
                    SUM(CASE WHEN det.QualityType = 0 THEN det.Qty ELSE 0 END) AS QualifyQty, sum(CASE WHEN det.QualityType = 1 THEN det.Qty ELSE 0 END) AS InspectQty, 
                    SUM(CASE WHEN det.QualityType = 2 THEN det.Qty ELSE 0 END) AS RejectQty, sum(CASE WHEN det.IsATP = 1 THEN det.Qty ELSE 0 END) AS ATPQty, 
                    SUM(CASE WHEN det.IsFreeze = 1 THEN det.Qty ELSE 0 END) AS FreezeQty
					FROM  inv_locationlotdet_'+@PartSuffix+' AS det 
					LEFT JOIN dbo.INV_Hu AS hu ON det.HuId = hu.HuId  
					LEFT  JOIN BIL_PlanBill AS bill ON det.PlanBill=bill.id AND bill.Type=0 '+@Where+' and det.Qty<>0
					GROUP BY det.Location, det.Item,det.LotNo,CASE WHEN bill.Party IS NOT NULL THEN bill.Party ELSE hu.ManufactureParty END '
		END	
		
		--exec(@Sql)	
		--print 'y'
		INSERT INTO #TempInternal
		EXEC(@Sql)		
	END
	--SELECT * FROM #TempInternal
	
	---最后的查询结果,包含2个数据集,一个是总数一个是分页数据
	IF @IsSummaryBySAPLoc=1
	BEGIN
		--汇总到SAP库位
		SET @sql = 'select row_number() over('+@SortDesc+') as rowid,* FROM (SELECT l.SAPLocation as Location, t.Item, t.LotNo, t.ManufactureParty,
			SUM(t.CSQty) AS CSQty,SUM(t.Qty) as QTY, SUM(t.QualifyQty) as QualifyQty, SUM(t.InspectQty) as InspectQty, 
			SUM(t.RejectQty) as RejectQty, SUM(t.ATPQty) as ATPQty, SUM(t.FreezeQty) as FreezeQty FROM #TempInternal as t
			INNER JOIN MD_Location l ON t.Location=l.Code
			left join #TempTransQty ts on ts.Item = t.Item and ts.Location = t.Location 
			left join #SalesTempTransQty s on s.Item = t.Item and s.Location = t.Location
			GROUP BY l.SAPLocation, t.Item, t.LotNo, t.ManufactureParty having SUM(t.Qty+isnull(ts.Qty,0)+isnull(s.Qty,0))<>0) as LocTranDet'
		--print 't'
		print @sql
		insert into #TempResult 
		exec(@sql)	
		--0002
		select count(1) from #TempResult d left join #TempTransQty t on t.Item = d.Item and t.Location = d.Location 
		left join #SalesTempTransQty s on s.Item = d.Item and s.Location = d.Location
		where d.Qty+isnull(t.Qty,0)+isnull(s.Qty,0)<>0
		--0002
		exec('select top('+@PageSize+') d.Location,l.Name as LocationName,d.Item,i.Desc1+case when Isnull(RefCode,'''')='''' then '''' else ''[''+RefCode+'']'' end as ItemDescription,
		i.MaterialsGroup as MaterialsGroup,c.Desc1 as MaterialsGroupDesc,i.Uom as Uom,d.LotNo,d.ManufactureParty,d.Qty,d.CSQTY, 
		d.QualifyQty,d.InspectQty,d.RejectQty,d.ATPQty,d.FreezeQty,isnull(t.Qty,0) as TransQty,
		isnull(t.QualifyQty,0) as TransQualifyQty,isnull(t.RejectQty,0) as TransRejectQty,
		isnull(s.Qty,0) as SalesTransQty,
		isnull(s.QualifyQty,0) as SalesTransQualifyQty,isnull(s.RejectQty,0) as SalesTransRejectQty
		from #TempResult as d
		left join MD_Item i on i.Code = d.Item
		left join MD_Location l on l.Code = d.Location
		left join MD_ItemCategory c on c.Code = i.MaterialsGroup and c.SubCategory=5
		left join #TempTransQty t on t.Item = d.Item and t.Location = d.Location 
		left join #SalesTempTransQty s on s.Item = d.Item and s.Location = d.Location '
		+@PagePara + ' and d.Qty+isnull(t.Qty,0)+isnull(s.Qty,0)<>0 ')		
	END
	ELSE
	BEGIN
		IF @SummaryLevel=0
		BEGIN
			--不汇总
			SET @sql = 'select row_number() over('+@SortDesc+') as rowid,* FROM (Select det.Location, det.Item, det.LotNo, det.ManufactureParty,
			det.Qty, det.CSQTY, det.QualifyQty, det.InspectQty, det.RejectQty, det.ATPQty, det.FreezeQty from #TempInternal as det 
			left join #TempTransQty ts on ts.Item = det.Item and ts.Location = det.Location  
			left join #SalesTempTransQty s on s.Item = det.Item and s.Location = det.Location 
			where det.Qty+isnull(ts.Qty,0)+isnull(s.Qty,0)<>0  ) As Dt'
			--print 'r'
			insert into #TempResult 
			exec(@sql)	
			
			PRINT @sql	
			--0002
			select count(1) from #TempResult d left join #TempTransQty t on t.Item = d.Item and t.Location = d.Location 
			left join #SalesTempTransQty s on s.Item = d.Item and s.Location = d.Location
			where d.Qty+isnull(t.Qty,0)+isnull(s.Qty,0)<>0
			--0002
			exec('select top('+@PageSize+') d.Location,l.Name as LocationName,d.Item,i.Desc1+case when Isnull(RefCode,'''')='''' then '''' else ''[''+RefCode+'']'' end as ItemDescription,
			i.MaterialsGroup as MaterialsGroup,c.Desc1 as MaterialsGroupDesc,i.Uom as Uom,d.LotNo,d.ManufactureParty,d.Qty,d.CSQTY, 
			d.QualifyQty,d.InspectQty,d.RejectQty,d.ATPQty,d.FreezeQty,isnull(t.Qty,0) as TransQty,
			isnull(t.QualifyQty,0) as TransQualifyQty,isnull(t.RejectQty,0) as TransRejectQty,
			isnull(s.Qty,0) as SalesTransQty,
			isnull(s.QualifyQty,0) as SalesTransQualifyQty,isnull(s.RejectQty,0) as SalesTransRejectQty
			from #TempResult as d
			left join MD_Item i on i.Code = d.Item
			left join MD_Location l on l.Code = d.Location
			left join MD_ItemCategory c on c.Code = i.MaterialsGroup and c.SubCategory=5
			left join #TempTransQty t on t.Item = d.Item and t.Location = d.Location 
			left join #SalesTempTransQty s on s.Item = d.Item and s.Location = d.Location  '
			+@PagePara + ' and d.Qty+isnull(t.Qty,0)+isnull(s.Qty,0)<>0 ') 
		END
		ELSE IF @SummaryLevel=1
		BEGIN
			--汇总到区域
			SET @sql = 'select row_number() over('+@SortDesc+') as rowid,* FROM (SELECT r.Code as Location, t.Item, t.LotNo, t.ManufactureParty,
				SUM(t.CSQty) AS CSQty,SUM(t.Qty) as QTY, SUM(t.QualifyQty) as QualifyQty, SUM(t.InspectQty) as InspectQty, 
				SUM(t.RejectQty) as RejectQty, SUM(t.ATPQty) as ATPQty, SUM(t.FreezeQty) as FreezeQty FROM #TempInternal as t
				INNER JOIN MD_Location l ON t.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region
				GROUP BY r.Code, t.Item, t.LotNo, t.ManufactureParty having SUM(t.Qty)<>0) as LocTranDet'
			--print 'e'
			insert into #TempResult 
			exec(@sql)	
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+')  Location, Item, LotNo, ManufactureParty, Qty, CSQTY, QualifyQty, InspectQty, RejectQty, ATPQty, FreezeQty from #TempResult '+@PagePara) 
		END
		ELSE IF @SummaryLevel=2
		BEGIN
			--汇总到车间
			SET @sql = 'select row_number() over('+@SortDesc+') as rowid,* FROM (SELECT r.Workshop as Location, t.Item, t.LotNo, t.ManufactureParty, 
				SUM(t.Qty) as QTY, SUM(t.CSQty) AS CSQty, SUM(t.QualifyQty) as QualifyQty, SUM(t.InspectQty) as InspectQty, 
				SUM(t.RejectQty) as RejectQty, SUM(t.ATPQty) as ATPQty, SUM(t.FreezeQty) as FreezeQty FROM #TempInternal as t
				INNER JOIN MD_Location l ON t.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region
				GROUP BY r.Workshop, t.Item, t.LotNo, t.ManufactureParty having SUM(t.Qty)<>0) as LocTranDet'
--print 'w'
			insert into #TempResult 
			exec(@sql)	
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+') Location, Item, LotNo, ManufactureParty, Qty, CSQTY, QualifyQty, InspectQty, RejectQty, ATPQty, FreezeQty from #TempResult '+@PagePara) 
		END
		ELSE IF @SummaryLevel=3
		BEGIN
			--汇总到工厂
			SET @sql = 'select row_number() over('+@SortDesc+') as rowid,* FROM (SELECT r.Plant as Location, t.Item, t.LotNo, t.ManufactureParty, 
				SUM(t.Qty) as QTY, SUM(t.CSQty) AS CSQty, SUM(t.QualifyQty) as QualifyQty, SUM(t.InspectQty) as InspectQty, 
				SUM(t.RejectQty) as RejectQty, SUM(t.ATPQty) as ATPQty, SUM(t.FreezeQty) as FreezeQty FROM #TempInternal as t
				INNER JOIN MD_Location l ON t.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region
				GROUP BY r.Plant, t.Item, t.LotNo, t.ManufactureParty having SUM(t.Qty)<>0) as LocTranDet'
			--print 'q'
			insert into #TempResult 
			exec(@sql)	
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+') Location, Item, LotNo, ManufactureParty, Qty, CSQTY, QualifyQty, InspectQty, RejectQty, ATPQty, FreezeQty from #TempResult '+@PagePara) 
		END
	END	
END
