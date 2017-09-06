SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_GetHuInventory4Pick')
BEGIN
	DROP PROCEDURE USP_WMS_GetHuInventory4Pick
END
GO

CREATE PROCEDURE dbo.USP_WMS_GetHuInventory4Pick
	@PickResultTable PickResultTableType readonly
AS
BEGIN
	set nocount on
	declare @ErrorMsg nvarchar(MAX)

	create table #tempPickResult_015
	(
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS primary key,
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempLocation_015
	(
		RowId int identity(1, 1),
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Suffix varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	begin try
		if not exists(select top 1 1 FROM tempdb.sys.objects WHERE type = 'U' AND name like '#tempHuInventory_015%') 
		begin
			set @ErrorMsg = '没有定义返回数据的参数表。'
			RAISERROR(@ErrorMsg, 16, 1) 

			--代码不会执行到这里
			create table #tempHuInventory_015
			(
				LocationLotDetId int primary key,
				HuId varchar(50) COLLATE  Chinese_PRC_CI_AS,
				LotNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
				Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
				ItemDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
				RefItemCode varchar(100) COLLATE  Chinese_PRC_CI_AS,
				Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
				BaseUom varchar(5) COLLATE  Chinese_PRC_CI_AS,
				UC decimal(18, 8),
				UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
				UnitQty decimal(18, 8),
				Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
				Area varchar(50) COLLATE  Chinese_PRC_CI_AS,
				Bin varchar(50) COLLATE  Chinese_PRC_CI_AS,
				Qty decimal(18, 8),
				QualityType tinyint,
				IsFreeze bit,
				OccupyType tinyint,
				IsLock bit,
				[Version] int
			)
		end
		else
		begin
			truncate table #tempHuInventory_015
		end

		insert into #tempPickResult_015(HuId, Location)
		select tmp.HuId, pt.Loc from @PickResultTable as tmp 
		inner join WMS_PickTask as pt on tmp.PickTaskId = pt.Id

		insert into #tempLocation_015(Location, Suffix) 
		select distinct pr.Location, l.PartSuffix
		from #tempPickResult_015 as pr inner join MD_Location as l on pr.Location = l.Code

		declare @RowId int
		declare @MaxRowId int
		declare @Location varchar(50)
		declare @Suffix varchar(50)
		declare @SelectInvStatement nvarchar(max)
		declare @Parameter nvarchar(max)

		select @RowId = MIN(RowId), @MaxRowId = MAX(RowId) from #tempLocation_015
		while (@RowId <= @MaxRowId)
		begin
			select @Location = Location, @Suffix = Suffix from #tempLocation_015 where RowId = @RowId

			set @SelectInvStatement = 'insert into #tempHuInventory_015(LocationLotDetId, HuId, LotNo, Item, ItemDesc, RefItemCode, Uom, BaseUom, UC, UCDesc, UnitQty, Location, Area, Bin, Qty, QualityType, IsFreeze, OccupyType, [Version]) '
			set @SelectInvStatement = @SelectInvStatement + 'select lld.Id, hu.HuId, hu.LotNo, hu.Item, hu.ItemDesc, hu.RefItemCode, hu.Uom, hu.BaseUom, hu.UC, hu.UCDesc, hu.UnitQty, lld.Location, bin.Area, lld.Bin, lld.Qty, lld.QualityType, lld.IsFreeze, lld.OccupyType, lld.[Version] '
			set @SelectInvStatement = @SelectInvStatement + 'from #tempPickResult_015 as pr '
			set @SelectInvStatement = @SelectInvStatement + 'inner join INV_Hu as hu on pr.HuId = hu.HuId '
			set @SelectInvStatement = @SelectInvStatement + 'inner join INV_LocationLotDet_' + @Suffix + ' as lld on lld.HuId = hu.HuId '
			set @SelectInvStatement = @SelectInvStatement + 'left join MD_LocationBin as bin on lld.Bin = bin.Code '
			set @SelectInvStatement = @SelectInvStatement + 'where pr.Location = @Location_1 and lld.Location = @Location_1 and lld.Qty > 0 '
			set @Parameter = N'@Location_1 varchar(50) '
	
			exec sp_executesql @SelectInvStatement, @Parameter, @Location_1=@Location

			set @RowId = @RowId + 1
		end
	end try
	begin catch
		set @ErrorMsg = N'获取条码库存发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	drop table #tempPickResult_015
	drop table #tempLocation_015
END
GO