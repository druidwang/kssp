SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_GetAvailableInv4PickLotNo')
BEGIN
	DROP PROCEDURE USP_WMS_GetAvailableInv4PickLotNo
END
GO

CREATE PROCEDURE dbo.USP_WMS_GetAvailableInv4PickLotNo
	@PickTargetTable PickTargetTableType readonly
AS
BEGIN
	set nocount on
	declare @ErrorMsg nvarchar(MAX)

	create table #tempPickTarget_009
	(
		RowId int identity(1, 1),
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempLocation_009
	(
		RowId int identity(1, 1),
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Suffix varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempOddPickTask_009
	(
		RowId int identity(1, 1),
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UC decimal(18, 8),
		OrderQty decimal(18, 8),
		LotNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Bin varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	begin try
		if not exists(select top 1 1 FROM tempdb.sys.objects WHERE type = 'U' AND name like '#tempAvailableInv_009%') 
		begin
			set @ErrorMsg = '没有定义返回数据的参数表。'
			RAISERROR(@ErrorMsg, 16, 1) 

			--代码不会执行到这里
			create table #tempAvailableInv_009
			(
				RowId int identity(1, 1),
				Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
				Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
				Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
				UC decimal(18, 8),
				UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
				Area varchar(50) COLLATE  Chinese_PRC_CI_AS,
				Qty decimal(18, 8),
				OccupyQty decimal(18, 8),
				Bin varchar(50) COLLATE  Chinese_PRC_CI_AS,
				LotNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
				IsOdd bit
			)
		end
		else
		begin
			truncate table #tempAvailableInv_009
		end

		insert into #tempPickTarget_009(Location, Item) select distinct Location, Item from @PickTargetTable

		insert into #tempLocation_009(Location, Suffix) 
		select distinct l.Code, l.PartSuffix
		from #tempPickTarget_009 as tmp inner join MD_Location as l on tmp.Location = l.Code

		declare @LocationRowId int
		declare @MaxLocationRowId int
		declare @Location varchar(50)
		declare @LocSuffix varchar(50)
		declare @SelectInvStatement nvarchar(max)
		declare @Parameter nvarchar(max)

		select @LocationRowId = MIN(RowId), @MaxLocationRowId = MAX(RowId) from #tempLocation_009

		while (@LocationRowId <= @MaxLocationRowId)
		begin
			select @Location = Location, @LocSuffix = Suffix
			from #tempLocation_009 where RowId = @LocationRowId

			set @SelectInvStatement = 'insert into #tempAvailableInv_009(Location, Item, Uom, UC, UCDesc, LotNo, Area, Bin, Qty, OccupyQty, IsOdd) '
			set @SelectInvStatement = @SelectInvStatement + 'select Location, Item, Uom, UC, UCDesc, LotNo, Area, Bin, InvQty, OccupyQty, IsOdd from ('
			set @SelectInvStatement = @SelectInvStatement + 'select sp.Location, sp.Item, hu.Uom, hu.UC, MAX(hu.UCDesc) as UCDesc, hu.LotNo, bin.Area, llt.Bin, bin.Seq, SUM(hu.Qty) as InvQty, 0 as OccupyQty, 0 as IsOdd '
			set @SelectInvStatement = @SelectInvStatement + 'from #tempPickTarget_009 as sp '
			set @SelectInvStatement = @SelectInvStatement + 'inner join INV_LocationLotDet_' + @LocSuffix + ' as llt on sp.Location = llt.Location and sp.Item = llt.Item '
			set @SelectInvStatement = @SelectInvStatement + 'inner join INV_Hu as hu on llt.HuId = hu.HuId '
			set @SelectInvStatement = @SelectInvStatement + 'inner join MD_LocationBin as bin on llt.Bin = bin.Code '
			set @SelectInvStatement = @SelectInvStatement + 'where sp.Location = @Location_1 and llt.OccupyType = 0 and llt.Qty > 0 and llt.QualityType = 0 and hu.Qty = hu.UC '
			set @SelectInvStatement = @SelectInvStatement + 'group by sp.Location, sp.Item, hu.Uom, hu.UC, hu.LotNo, bin.Area, llt.Bin, bin.Seq '
			set @SelectInvStatement = @SelectInvStatement + 'union all '
			set @SelectInvStatement = @SelectInvStatement + 'select sp.Location, sp.Item, hu.Uom, hu.UC, hu.UCDesc, hu.LotNo, bin.Area, llt.Bin, bin.Seq, hu.Qty, 0 as OccupyQty, 1 as IsOdd '
			set @SelectInvStatement = @SelectInvStatement + 'from #tempPickTarget_009 as sp '
			set @SelectInvStatement = @SelectInvStatement + 'inner join INV_LocationLotDet_' + @LocSuffix + ' as llt on sp.Location = llt.Location and sp.Item = llt.Item ' 
			set @SelectInvStatement = @SelectInvStatement + 'inner join INV_Hu as hu on llt.HuId = hu.HuId '
			set @SelectInvStatement = @SelectInvStatement + 'inner join MD_LocationBin as bin on llt.Bin = bin.Code '
			set @SelectInvStatement = @SelectInvStatement + 'where sp.Location = @Location_1 and llt.OccupyType = 0 and llt.Qty > 0 and llt.QualityType = 0 and hu.Qty <> hu.UC '
			set @SelectInvStatement = @SelectInvStatement + ') as a order by Location, Item, Uom, UC, LotNo, Seq'
			set @Parameter = N'@Location_1 varchar(50)'

			exec sp_executesql @SelectInvStatement, @Parameter, @Location_1=@Location

			set @LocationRowId = @LocationRowId + 1
		end

		--添加收货缓冲的库存
		--insert into #tempAvailableInv_009(Location, Item, UC, LotNo, BinSeq, Qty)
		--select bi.Loc, bi.Item, hu.UC, hu.LotNo, -100, SUM(bi.Qty)
		--from #tempPickTarget_009 as sp
		--inner join WMS_BuffInv as bi on bi.Loc = sp.Location and bi.Item = sp.Item
		--inner join INV_Hu as hu on bi.HuId = hu.HuId
		--where bi.Qty > 0 and bi.IOType = 0 --在收货缓冲中
		--group by bi.Loc, bi.Item, hu.UC, hu.LotNo

		--扣减拣货单占用的库存（先扣减占用缓存的库存）          拣货都是拣的库格中的物料，暂不支持直接拣地板上的物料（因为地板上的物料应该都在缓冲区里面）
		--update inv set Qty = inv.Qty - oc.OccupyQty
		--from #tempAvailableInv_009 as inv inner join
		--(select pt.Loc, pt.Item, pt.UC, pt.LotNo, SUM((pt.OrderQty - pt.PickQty) * pt.UnitQty) as OccupyQty
		--from #tempPickTarget_009 as sp 
		--inner join WMS_PickTask as pt on sp.Location = pt.Loc and sp.Item = pt.Item
		--where pt.IsPickHu = 1 and pt.PickBy = 0 and pt.IsActive = 1 and pt.OrderQty > pt.PickQty and pt.Bin is null
		--group by pt.Loc, pt.Item, pt.UC, pt.LotNo) as oc on inv.Location = oc.Loc and inv.Item = oc.Item and inv.UC = oc.UC and inv.LotNo = oc.LotNo
		--where inv.Bin is null

		--扣减拣货单占用的库存（整箱）
		update inv set Qty = inv.Qty - oc.OccupyQty
		from #tempAvailableInv_009 as inv inner join
		(select pt.Loc, pt.Item, pt.Uom, pt.UC, pt.LotNo, pt.Bin, SUM(pt.OrderQty - pt.PickQty) as OccupyQty
		from @PickTargetTable as sp 
		inner join WMS_PickTask as pt on sp.Location = pt.Loc and sp.Item = pt.Item and sp.Uom = pt.Uom
		where pt.IsPickHu = 1 and pt.PickBy = 0 and pt.IsActive = 1 and pt.IsOdd = 0
		group by pt.Loc, pt.Item, pt.Uom, pt.UC, pt.LotNo, pt.Bin) as oc on inv.Location = oc.Loc and inv.Item = oc.Item and inv.Uom = oc.Uom and inv.UC = oc.UC and inv.LotNo = oc.LotNo and inv.Bin = oc.Bin
		where inv.IsOdd = 0

		-----------------------------↓扣减拣货单占用的库存（零头箱）-----------------------------	
		insert into #tempOddPickTask_009(Loc, Item, Uom, UC, OrderQty, LotNo, Bin)
		select pt.Loc, pt.Item, pt.Uom, pt.UC, (pt.OrderQty - pt.PickQty), pt.LotNo, pt.Bin
		from @PickTargetTable as sp 
		inner join WMS_PickTask as pt on sp.Location = pt.Loc and sp.Item = pt.Item and sp.Uom = pt.Uom
		where pt.IsPickHu = 1 and pt.PickBy = 0 and pt.IsActive = 1 and pt.IsOdd = 1

		declare @PickTaskRowId int
		declare @MaxPickTaskRowId int
		declare @Loc varchar(50) 
		declare @Item varchar(50) 
		declare @Uom varchar(5) 
		declare @UC decimal(18, 8)
		declare @OrderQty decimal(18, 8)
		declare @LotNo varchar(50) 
		declare @Bin varchar(50) 

		select @PickTaskRowId = MIN(RowId), @MaxPickTaskRowId = MAX(RowId) from #tempOddPickTask_009
		while @PickTaskRowId <= @MaxPickTaskRowId
		begin
			select @Loc = Loc, @Item = Item, @Uom = Uom, @UC = UC, @OrderQty = OrderQty, @LotNo = LotNo, @Bin = Bin
			from #tempOddPickTask_009 where RowId = @PickTaskRowId

			update ai set Qty = 0
			from #tempAvailableInv_009 as ai
			inner join (select top 1 RowId from #tempAvailableInv_009
						where Location = @Loc and Item = @Item and Uom = @Uom and UC = @UC
						and Qty = @OrderQty and LotNo = @LotNo and Bin = @Bin) as ri on ai.RowId = ri.RowId

			set @PickTaskRowId = @PickTaskRowId + 1
		end
		-----------------------------↑扣减拣货单占用的库存（零头箱）-----------------------------

		--删除数量小于等于0的库存
		delete from #tempAvailableInv_009 where Qty <= 0
	end try
	begin catch
		set @ErrorMsg = N'获取拣货可用库存发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	drop table #tempLocation_009
	drop table #tempPickTarget_009
	drop table #tempOddPickTask_009
END
GO