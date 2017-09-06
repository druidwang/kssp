SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_ProcessPickResult4PickLotNoAndHu')
BEGIN
	DROP PROCEDURE USP_WMS_ProcessPickResult4PickLotNoAndHu
END
GO

CREATE PROCEDURE dbo.USP_WMS_ProcessPickResult4PickLotNoAndHu
	@PickResultTable PickResultTableType readonly,
	@PickBy tinyint,
	@CreateUserId int,
	@CreateUserNm varchar(100)
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)
	
	set @DateTimeNow = GetDate()

	create table #tempLocation_014
	(
		RowId int identity(1, 1),
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Suffix varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempPickResult_014
	(
		RowId int identity(1, 1),
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS primary key,
		LotNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		PickTaskId int,
		PickTaskUUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ItemDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		RefItemCode varchar(100) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		BaseUom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UC decimal(18, 8),
		UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Area varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Bin varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Qty decimal(18, 8)
	)

	create table #tempPickTask_014
	(
		PickTaskID int Primary Key,
		PickTaskUUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderQty decimal(18, 8),
		PickQty decimal(18, 8),
		ThisPickQty decimal(18, 8),
		UC decimal(18, 8),
		ShipUC decimal(18, 8), 
		[Version] int
	)

	create table #tempPickOccupy_014
	(
		Id int Primary Key,
		PickTaskUUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ShipPlanId int,
		OccupyQty decimal(18, 8),
		ReleaseQty decimal(18, 8),
		ThisReleaseQty decimal(18, 8),
		ThisLockQty decimal(18, 8),
		[Version] int
	)

	create table #tempShipPlan_014
	(
		ShipPlanId int Primary Key,
		PickQty decimal(18, 8),
		PickedQty decimal(18, 8),
		ThisPickedQty decimal(18, 8),
		ThisLockQty decimal(18, 8),
		[Version] int,
	)

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

	create table #tempRepackOccupy_014
	(
		RowId int Identity(1, 1) primary key,
		GroupId int,
		RepackUUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderSeq int,
		TargetDock varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ShipPlanId int,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ItemDesc varchar(100) COLLATE  Chinese_PRC_CI_AS,
		RefItemCode varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		BaseUom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		[Priority] tinyint,
		StartTime datetime,
		OccupyQty decimal(18, 8)
	)

	create table #tempRepackTask_014
	(
		RepackUUID varchar(50) COLLATE  Chinese_PRC_CI_AS primary key,
		GroupId int,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ItemDesc varchar(100) COLLATE  Chinese_PRC_CI_AS,
		RefItemCode varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		BaseUom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Qty decimal(18, 8),
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		[Priority] tinyint,
		StartTime datetime,
		WinTime datetime
	)

	create table #tempRepackBuff_014
	(
		ShipPlanId int,
		OrderNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderSeq int,
		TargetDock varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ItemDesc varchar(100) COLLATE  Chinese_PRC_CI_AS,
		RefItemCode varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		BaseUom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Qty decimal(18, 8),
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		[Priority] tinyint,
		StartTime datetime,
		WindowTime datetime,
		IsUpdate bit,
		[Version] int
	)

	begin try
		if not exists(select top 1 1 FROM tempdb.sys.objects WHERE type = 'U' AND name like '#tempMsg_014%') 
		begin
			set @ErrorMsg = '没有定义返回数据的参数表。'
			RAISERROR(@ErrorMsg, 16, 1) 

			--代码不会执行到这里
			create table #tempMsg_014 
			(
				Id int identity(1, 1) primary key,
				Lvl tinyint,
				Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
			)
		end
		else
		begin
			truncate table #tempMsg_014
		end

		begin try
			exec USP_WMS_GetHuInventory4Pick @PickResultTable
			
			insert into #tempLocation_014(Location, Suffix)
			select distinct l.Code, l.PartSuffix 
			from #tempHuInventory_015 as inv inner join MD_Location as l on inv.Location = l.Code

			insert into #tempMsg_014(Lvl, Msg)
			select 2, N'条码['+ pr.HuId + N']在库位['+ pt.Loc + N']中不存在。'
			from @PickResultTable as pr left join #tempHuInventory_015 as inv on pr.HuId = inv.HuId
			inner join WMS_PickTask as pt on pr.PickTaskId = pt.Id
			where inv.HuId is null

			insert into #tempMsg_014(Lvl, Msg)
			select 2, N'条码['+ HuId + N']已经被占用。'
			from #tempHuInventory_015
			where OccupyType <> 0

			insert into #tempMsg_014(Lvl, Msg)
			select 2, N'条码['+ HuId + N']的质量状态不是合格。'
			from #tempHuInventory_015
			where QualityType <> 0

			insert into #tempMsg_014(Lvl, Msg)
			select 2, N'条码['+ HuId + N']已经被冻结'
			from #tempHuInventory_015
			where IsFreeze = 1

			insert into #tempMsg_014(Lvl, Msg)
			select 2, N'条码['+ pr.HuId + N']不在库格上。'
			from @PickResultTable as pr left join #tempHuInventory_015 as inv on pr.HuId = inv.HuId
			inner join WMS_PickTask as pt on pr.PickTaskId = pt.Id
			where inv.Bin is null

			insert into #tempMsg_014(Lvl, Msg)
			select 2, N'拣货任务['+ convert(varchar, pt.Id) + N']的物料代码['+ pt.Item + N']和条码['+ inv.HuId + N']的物料代码['+ inv.Item + N']不匹配。'
			from @PickResultTable as pr inner join #tempHuInventory_015 as inv on pr.HuId = inv.HuId
			inner join WMS_PickTask as pt on pr.PickTaskId = pt.Id
			where inv.Item <> pt.Item

			insert into #tempMsg_014(Lvl, Msg)
			select 2, N'拣货任务['+ convert(varchar, pt.Id) + N']物料代码['+ pt.Item + N']的单位['+ pt.Uom + N']和条码['+ inv.HuId + N']物料代码['+ inv.Item + N']的单位['+ pt.Uom + N']不匹配。'
			from @PickResultTable as pr inner join #tempHuInventory_015 as inv on pr.HuId = inv.HuId
			inner join WMS_PickTask as pt on pr.PickTaskId = pt.Id
			where inv.Uom <> pt.Uom

			if not exists(select top 1 1 from #tempMsg_014)
			begin
				insert into #tempPickResult_014(PickTaskId, PickTaskUUID, HuId, LotNo, Item, ItemDesc, RefItemCode, 
												Uom, BaseUom, UC, UCDesc, UnitQty, Loc, Area, Bin, Qty)
				select pt.Id, pt.UUID, inv.HuId, inv.LotNo, inv.Item, inv.ItemDesc, inv.RefItemCode, 
				inv.Uom, inv.BaseUom, inv.UC, inv.UCDesc, inv.UnitQty, inv.Location, inv.Area, inv.Bin, inv.Qty
				from @PickResultTable as pr inner join #tempHuInventory_015 as inv on pr.HuId = inv.HuId
				inner join WMS_PickTask as pt on pr.PickTaskId = pt.Id

				insert into #tempPickTask_014(PickTaskID, PickTaskUUID, OrderQty, PickQty, ThisPickQty, UC, ShipUC, [Version])
				select distinct pt.Id, pt.UUID, pt.OrderQty, pt.PickQty, 0, pt.UC, pt.ShipUC, pt.[Version]
				from @PickResultTable as tmp 
				inner join WMS_PickTask as pt on tmp.PickTaskId = pt.Id

				update inv set IsLock = CASE WHEN inv.UC = pt.ShipUC THEN 1 ELSE 0 END
				from #tempHuInventory_015 as inv 
				inner join #tempPickResult_014 as pr on inv.HuId = pr.HuId
				inner join #tempPickTask_014 as pt on pr.PickTaskId = pt.PickTaskId

				update pt set ThisPickQty = tmp.Qty
				from #tempPickTask_014 as pt 
				inner join (select PickTaskId, SUM(Qty) as Qty 
							from @PickResultTable 
							group by PickTaskId) as tmp on pt.PickTaskId = tmp.PickTaskId

				insert into #tempPickOccupy_014(Id, PickTaskUUID, ShipPlanId, OccupyQty, ReleaseQty, ThisReleaseQty, ThisLockQty, [Version])
				select po.Id, po.UUID, po.ShipPlanId, po.OccupyQty, po.ReleaseQty, 0, 0, po.[Version]
				from #tempPickTask_014 as pt 
				inner join WMS_PickOccupy as po on pt.PickTaskUUID = po.UUID

				insert into #tempShipPlan_014(ShipPlanId, PickQty, PickedQty, ThisPickedQty, [Version])
				select distinct sp.Id, sp.PickQty, sp.PickedQty, 0, sp.[Version]
				from WMS_ShipPlan as sp 
				inner join #tempPickOccupy_014 as po on po.ShipPlanId = sp.Id

				declare @RowId int
				declare @MaxRowId int
				declare @PickTaskId int
				declare @PickTaskUUID varchar(50)
				declare @Qty decimal(18, 8)
				declare @IsRepack bit
				declare @LastQty decimal(18, 8)
				select @RowId = MIN(RowId), @MaxRowId = MAX(RowId) from #tempPickResult_014
				while @RowId <= @MaxRowId
				begin
					set @LastQty = 0
					select @PickTaskId = pr.PickTaskId, @PickTaskUUID = pr.PickTaskUUID, @Qty = pr.Qty, @IsRepack = CASE WHEN pt.ShipUC = pr.UC THEN 0 ELSE 1 END
					from #tempPickResult_014 as pr inner join #tempPickTask_014 as pt on pr.PickTaskUUID = pt.PickTaskUUID 
					where pr.RowId = @RowId

					update #tempPickOccupy_014 set ThisReleaseQty = ThisReleaseQty + @LastQty, ThisLockQty = ThisLockQty + CASE WHEN @IsRepack = 0 THEN @LastQty ELSE 0 END,
					@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= OccupyQty - ReleaseQty - ThisReleaseQty THEN OccupyQty - ReleaseQty - ThisReleaseQty ELSE @Qty END
					where PickTaskUUID = @PickTaskUUID
					set @Qty = @Qty - @LastQty

					--if (@Qty > 0)
					--begin
					--	insert into #tempMsg_014(Lvl, Msg)
					--	values (2, N'拣货任务['+ convert(varchar, @PickTaskId) + N']的已拣数已经超过了对应发运单的拣货数。')
					--end

					set @RowId = @RowId + 1
				end

				update sp set ThisPickedQty = po.ThisReleaseQty, ThisLockQty = po.ThisLockQty
				from #tempShipPlan_014 as sp 
				inner join (select ShipPlanId, SUM(ThisReleaseQty) as ThisReleaseQty, SUM(ThisLockQty) as ThisLockQty 
							from #tempPickOccupy_014 group by ShipPlanId) as po on po.ShipPlanId = sp.ShipPlanId

				insert into #tempMsg_014(Lvl, Msg)
				select 2, N'拣货任务关联的发运任务['+ convert(varchar, ShipPlanId) + N']的已拣数已经超过了待拣数。'
				from #tempShipPlan_014 where PickQty < PickedQty + ThisPickedQty
	
				insert into #tempRepackBuff_014(ShipPlanId, OrderNo, OrderSeq, TargetDock, Item, ItemDesc, RefItemCode, Uom, BaseUom,
												UnitQty, UC, UCDesc, Qty, Loc, [Priority], StartTime, WindowTime, [Version])
				select rb.ShipPlanId, rb.OrderNo, rb.OrderSeq, rb.TargetDock, rb.Item, rb.ItemDesc, rb.RefItemCode, rb.Uom, rb.BaseUom,
						rb.UnitQty, rb.UC, rb.UCDesc, rb.Qty, rb.Loc, rb.[Priority], rb.StartTime, rb.WindowTime, rb.[Version] 
				from WMS_RepackBuff as rb
				inner join (select distinct ShipPlanId from #tempPickOccupy_014) as po on rb.ShipPlanId = po.ShipPlanId

				update rb set Qty = rb.Qty + tmp.Qty, IsUpdate = 1
				from #tempRepackBuff_014 as rb
				inner join (select po.ShipPlanId, SUM(po.ThisReleaseQty - po.ThisLockQty) as Qty 
							from #tempPickOccupy_014 as po 
							inner join WMS_ShipPlan as sp on po.ShipPlanId = sp.Id
							where po.ThisReleaseQty > po.ThisLockQty group by po.ShipPlanId) as tmp on rb.ShipPlanId = tmp.ShipPlanId

				insert into #tempRepackBuff_014(ShipPlanId, OrderNo, OrderSeq, TargetDock, Item, ItemDesc, RefItemCode, Uom, BaseUom,
												UnitQty, UC, UCDesc, Qty, Loc, [Priority], StartTime, WindowTime, [Version])
				select sp.Id, sp.OrderNo, sp.OrderSeq, sp.Dock, sp.Item, sp.ItemDesc, sp.RefItemCode, sp.Uom, sp.BaseUom, 
				sp.UnitQty, sp.UC, sp.UCDesc, tmp.Qty, sp.LocFrom, sp.[Priority], sp.StartTime, sp.WindowTime, null  
				from (select sp.Id as ShipPlanId, SUM(po.ThisReleaseQty - po.ThisLockQty) as Qty 
						from #tempPickOccupy_014 as po 
						inner join WMS_ShipPlan as sp on po.ShipPlanId = sp.Id
						left join #tempRepackBuff_014 as rb on sp.Id = rb.ShipPlanId
						where rb.ShipPlanId is null and po.ThisReleaseQty > po.ThisLockQty
						group by sp.Id) as tmp
				inner join WMS_ShipPlan as sp on tmp.ShipPlanId = sp.Id

				insert into #tempRepackOccupy_014(GroupId, OrderNo, OrderSeq, TargetDock, ShipPlanId, Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, Loc, [Priority], StartTime, OccupyQty)
				select ROW_NUMBER() over (partition by Item, Uom, UC, Loc, [Priority] order by ShipPlanId), 
				OrderNo, OrderSeq, TargetDock, ShipPlanId, Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, Loc, [Priority], StartTime, ROUND(Qty / UC, 0, 1) * UC
				from #tempRepackBuff_014
				where Qty >= UC

				update #tempRepackBuff_014 set Qty = Qty % UC, IsUpdate = 1
				where Qty >= UC

				insert into #tempRepackTask_014(RepackUUID, GroupId, Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, Qty, Loc, [Priority], StartTime, WinTime)
				select NEWID(), GroupId, MIN(Item), MIN(ItemDesc), MIN(RefItemCode), MIN(Uom), MIN(BaseUom), MIN(UnitQty), MIN(UC), MIN(UCDesc), SUM(OccupyQty), MIN(Loc), MIN([Priority]), @DateTimeNow, MIN(StartTime)
				from #tempRepackOccupy_014
				group by GroupId

				update ro set RepackUUID = rt.RepackUUID
				from #tempRepackOccupy_014 as ro inner join #tempRepackTask_014 as rt on ro.GroupId = rt.GroupId
			end
		end try
		begin catch
			set @ErrorMsg = N'数据准备出现异常：' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		if not exists(select top 1 1 from #tempMsg_014)
		begin
			begin try
				declare @Trancount int
				set @Trancount = @@trancount

				if @Trancount = 0
				begin
					begin tran
				end
				
				declare @UpdateCount int

				select @UpdateCount = COUNT(1) from #tempPickTask_014
				update pt set PickQty = pt.PickQty + tmp.ThisPickQty, IsActive = CASE WHEN pt.PickQty + tmp.ThisPickQty >= pt.OrderQty THEN 1 ELSE 0 END,
								LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = pt.[Version] + 1
				from WMS_PickTask as pt inner join #tempPickTask_014 as tmp on pt.UUID = tmp.PickTaskUUID and pt.[Version] = tmp.[Version]

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				select @UpdateCount = COUNT(1) from  #tempPickOccupy_014 where ThisReleaseQty > 0
				update po set ReleaseQty = po.ReleaseQty + tmp.ThisReleaseQty, [Version] = po.[Version] + 1
				from WMS_PickOccupy as po inner join #tempPickOccupy_014 as tmp on po.Id = tmp.Id and po.[Version] = tmp.[Version]
				where tmp.ThisReleaseQty > 0

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				select @UpdateCount = COUNT(1) from  #tempShipPlan_014 where ThisPickedQty > 0
				update sp set LockQty = sp.LockQty + tmp.ThisLockQty, PickedQty = sp.PickedQty + tmp.ThisPickedQty, 
				LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = sp.[Version] + 1
				from WMS_ShipPlan as sp inner join #tempShipPlan_014 as tmp on sp.Id = tmp.ShipPlanId and sp.[Version] = tmp.[Version]
				where tmp.ThisPickedQty > 0

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				insert into WMS_PickResult(PickTaskId, PickTaskUUID, Item, ItemDesc, RefItemCode, Uom, BaseUom, UC, UCDesc, PickQty, Loc, 
					Area, Bin, LotNo, HuId, PickUserId, PickUserNm, CreateUser, CreateUserNm, CreateDate, IsCancel)
				select PickTaskId, PickTaskUUID, Item, ItemDesc, RefItemCode, Uom, BaseUom, UC, UCDesc, Qty, Loc,  
					Area, Bin, LotNo, HuId, @CreateUserId, @CreateUserNm, @CreateUserId, @CreateUserNm, @DateTimeNow, 0 
				from #tempPickResult_014

				select @UpdateCount = COUNT(1) from #tempRepackBuff_014 where IsUpdate = 1 and [Version] is not null
				update WMS_RepackBuff set Qty = 1
				from WMS_RepackBuff as rb inner join #tempRepackBuff_014 as tmp on rb.ShipPlanId = tmp.ShipPlanId and rb.[Version] = tmp.[Version]
				where tmp.IsUpdate = 1 and tmp.[Version] is not null

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				insert into WMS_RepackBuff(ShipPlanId, OrderNo, OrderSeq, TargetDock, Item, ItemDesc, RefItemCode, Uom, BaseUom,
												UnitQty, UC, UCDesc, Qty, Loc, [Priority], StartTime, WindowTime, [Version])
				select ShipPlanId, OrderNo, OrderSeq, TargetDock, Item, ItemDesc, RefItemCode, Uom, BaseUom,
												UnitQty, UC, UCDesc, Qty, Loc, [Priority], StartTime, WindowTime, ISNULL([Version], 1)
				from #tempRepackBuff_014 where [Version] is null

				if exists(select top 1 1 from #tempRepackTask_014)
				begin
					insert into WMS_RepackTask(UUID, Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, Qty, RepackQty, Loc, 
												[Priority], StartTime, WinTime, IsActive, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])
					select RepackUUID, Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, Qty, 0, Loc, 
					[Priority], StartTime, WinTime, 1, @CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 1 
					from #tempRepackTask_014

					insert into WMS_RepackOccupy(UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OccupyQty, ReleaseQty, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])
					select RepackUUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OccupyQty, 0, @CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 1 from #tempRepackOccupy_014
				end

				insert into WMS_BuffInv(UUID, Loc, IOType, Item, Uom, UC, Qty, LotNo, HuId, IsLock, IsPack, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])
				select NEWID(), Location, 1, Item, Uom, UC, Qty * UnitQty, LotNo, HuId, IsLock, 0, @CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 1 
				from #tempHuInventory_015

				declare @Location varchar(50)
				declare @Suffix varchar(50)
				declare @UpdateInvStatement nvarchar(max)
				declare @Parameter nvarchar(max)

				select @RowId = MIN(RowId), @MaxRowId = MAX(RowId) from #tempLocation_014
				while (@RowId <= @MaxRowId)
				begin
					select @Location = Location, @Suffix = Suffix from #tempLocation_014 where RowId = @RowId

					set @UpdateInvStatement = 'declare @UpdateCount int '
					set @UpdateInvStatement = @UpdateInvStatement + 'select @UpdateCount = COUNT(1) from #tempHuInventory_015 where Location = @Location_1 '
					set @UpdateInvStatement = @UpdateInvStatement + 'update lld set Bin = null, OccupyType = 1, LastModifyUser = @CreateUserId_1, LastModifyUserNm = @CreateUserNm_1, LastModifyDate = @DateTimeNow_1, [Version] = lld.[Version] + 1 '
					set @UpdateInvStatement = @UpdateInvStatement + 'from INV_LocationLotDet_' + @Suffix + ' as lld '
					set @UpdateInvStatement = @UpdateInvStatement + 'inner join #tempHuInventory_015 as inv on lld.Id = inv.LocationLotDetId and lld.[Version] = inv.[Version] where inv.Location = @Location_1 '
					set @UpdateInvStatement = @UpdateInvStatement + 'if (@@ROWCOUNT <> @UpdateCount) '
					set @UpdateInvStatement = @UpdateInvStatement + 'begin '
					set @UpdateInvStatement = @UpdateInvStatement + 'RAISERROR(N''数据已经被更新，请重试。'', 16, 1) '
					set @UpdateInvStatement = @UpdateInvStatement + 'end'
					set @Parameter = N'@Location_1 varchar(50), @CreateUserId_1 int, @CreateUserNm_1 varchar(100), @DateTimeNow_1 datetime'

					exec sp_executesql @UpdateInvStatement, @Parameter, @Location_1=@Location, @CreateUserId_1=@CreateUserId, @CreateUserNm_1=@CreateUserNm, @DateTimeNow_1=@DateTimeNow

					set @RowId = @RowId + 1
				end

				if @Trancount = 0 
				begin  
					commit
				end
			end try
			begin catch
				if @Trancount = 0
				begin
					rollback
				end 

				set @ErrorMsg = N'数据更新出现异常：' + Error_Message()
				RAISERROR(@ErrorMsg, 16, 1) 
			end catch
		end
	end try
	begin catch
		set @ErrorMsg = N'处理条码拣货结果发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	drop table #tempLocation_014
	drop table #tempPickResult_014
	drop table #tempPickTask_014
	drop table #tempPickOccupy_014
	drop table #tempShipPlan_014
	drop table #tempHuInventory_015
	drop table #tempRepackOccupy_014
	drop table #tempRepackTask_014
END
GO