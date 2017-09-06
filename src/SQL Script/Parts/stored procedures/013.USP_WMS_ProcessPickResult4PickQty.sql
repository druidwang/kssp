SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_ProcessPickResult4PickQty')
BEGIN
	DROP PROCEDURE USP_WMS_ProcessPickResult4PickQty
END
GO

CREATE PROCEDURE dbo.USP_WMS_ProcessPickResult4PickQty
	@PickResultTable PickResultTableType readonly,
	@CreateUserId int,
	@CreateUserNm varchar(100)
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)
	
	set @DateTimeNow = GetDate()

	create table #tempPickTask_013
	(
		RowId int identity(1, 1),
		PickTaskID int Primary Key,
		PickTaskUUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ItemDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		RefItemCode varchar(100) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		BaseUom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UC decimal(18, 8),
		UCDesc varchar(50),
		UnitQty decimal(18, 8),
		OrderQty decimal(18, 8),
		PickQty decimal(18, 8),
		ThisPickQty decimal(18, 8),
		[Version] int
	)

	create table #tempPickOccupy_013
	(
		Id int Primary Key,
		PickTaskUUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ShipPlanId int,
		OccupyQty decimal(18, 8),
		ReleaseQty decimal(18, 8),
		ThisReleaseQty decimal(18, 8),
		[Version] int
	)

	create table #tempShipPlan_013
	(
		ShipPlanId int Primary Key,
		PickQty decimal(18, 8),
		PickedQty decimal(18, 8),
		ThisPickedQty decimal(18, 8),
		[Version] int,
	)

	create table #tempLocation_013
	(
		RowId int identity(1, 1),
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Suffix varchar(50) COLLATE  Chinese_PRC_CI_AS,
		AllowNegaInv bit
	)

	create table #tempUpdatedLocLotDet_013
	(
		LocationLotDetId int primary key,
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Qty decimal(18, 8),
		UpdateQty decimal(18, 8),
		PlanBill int,
		OccupyType tinyint,
		[Version] int,
		Seq int
	)

	create table #tempInsertedLocLotDet_013
	(
		RowId int primary key,
		LocationLotDetId int,
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Qty decimal(18, 8),
		PlanBill int
	)

	begin try
		if not exists(select top 1 1 FROM tempdb.sys.objects WHERE type = 'U' AND name like '#tempMsg_013%') 
		begin
			set @ErrorMsg = '没有定义返回数据的参数表。'
			RAISERROR(@ErrorMsg, 16, 1) 

			--代码不会执行到这里
			create table #tempMsg_013 
			(
				Id int identity(1, 1) primary key,
				Lvl tinyint,
				Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
			)
		end
		else
		begin
			truncate table #tempMsg_013
		end

		begin try
			insert into #tempPickTask_013(PickTaskId, PickTaskUUID, Loc, Item, ItemDesc, RefItemCode, Uom, BaseUom, UC, UCDesc, UnitQty, OrderQty, PickQty, ThisPickQty, [Version])
			select distinct tp.Id, tp.UUID, tp.Loc, tp.Item, tp.ItemDesc, tp.RefItemCode, tp.Uom, tp.BaseUom, tp.UC, tp.UCDesc, tp.UnitQty, tp.OrderQty, tp.PickQty, tmp.Qty, tp.[Version]
			from @PickResultTable as tmp 
			inner join WMS_PickTask as tp on tmp.PickTaskId = tp.Id

			insert into #tempPickOccupy_013(Id, PickTaskUUID, ShipPlanId, OccupyQty, ReleaseQty, ThisReleaseQty, [Version])
			select po.Id, po.UUID, po.ShipPlanId, po.OccupyQty, po.ReleaseQty, 0, po.[Version]
			from #tempPickTask_013 as pt 
			inner join WMS_PickOccupy as po on pt.PickTaskUUID = po.UUID

			insert into #tempShipPlan_013(ShipPlanId, PickQty, PickedQty, ThisPickedQty, [Version])
			select distinct sp.Id, sp.PickQty, sp.PickedQty, 0, sp.[Version]
			from WMS_ShipPlan as sp 
			inner join #tempPickOccupy_013 as po on po.ShipPlanId = sp.Id

			insert into #tempLocation_013(Location, Suffix, AllowNegaInv) 
			select distinct l.Code, l.PartSuffix, l.AllowNegaInv
			from #tempPickTask_013 as pt inner join MD_Location as l on pt.Loc = l.Code

			declare @LocationRowId int
			declare @MaxLocationRowId int
			declare @Location varchar(50)
			declare @LocSuffix varchar(50)
			declare @SelectInvStatement nvarchar(max)
			declare @Parameter nvarchar(max)

			select @LocationRowId = MIN(RowId), @MaxLocationRowId = MAX(RowId) from #tempLocation_013

			while (@LocationRowId <= @MaxLocationRowId)
			begin
				select @Location = Location, @LocSuffix = Suffix
				from #tempLocation_013 where RowId = @LocationRowId
				set @SelectInvStatement = ''

				set @SelectInvStatement = 'insert into #tempUpdatedLocLotDet_013(LocationLotDetId, Location, Item, Qty, UpdateQty, PlanBill, OccupyType, [Version]) '
				set @SelectInvStatement = @SelectInvStatement + 'select llt.Id, llt.Location, llt.Item, llt.Qty, 0, CASE WHEN llt.IsCS = 1 THEN llt.PlanBill ELSE NULL END, llt.OccupyType, llt.[Version] '
				set @SelectInvStatement = @SelectInvStatement + 'from INV_LocationLotDet_' + @LocSuffix + ' as llt inner join #tempPickTask_013 as pt on lld.Location = pt.Loc and lld.Item = pt.Item '
				set @SelectInvStatement = @SelectInvStatement + 'where lld.Location = @Location_1 and pt.Location = @Location_1 and llt.OccupyType in (0, 1) and llt.Qty > 0 and llt.HuId is null and llt.QualityType = 0'
				set @Parameter = N'@Location_1 varchar(50)'

				exec sp_executesql @SelectInvStatement, @Parameter, @Location_1=@Location

				set @LocationRowId = @LocationRowId + 1
			end
			
			declare @RowId int
			declare @MaxRowId int
			declare @PickTaskId int
			declare @PickTaskUUID varchar(50)
			declare @Item varchar(50)
			declare @AllowNegaInv bit
			declare @Qty decimal(18, 8)
			declare @BaseQty decimal(18, 8)
			declare @LastQty decimal(18, 8)
			declare @LastBaseQty decimal(18, 8)
			select @RowId = MIN(RowId), @MaxRowId = MAX(RowId) from #tempPickTask_013
			while @RowId <= @MaxRowId
			begin
				set @LastQty = 0
				set @LastBaseQty = 0
				select @PickTaskId = pt.PickTaskId, @PickTaskUUID = pt.PickTaskUUID, @Qty = pt.ThisPickQty, @BaseQty = pt.ThisPickQty * pt.UnitQty,
				@Location = pt.Loc, @Item = pt.Item, @AllowNegaInv = l.AllowNegaInv
				from #tempPickTask_013 as pt inner join #tempLocation_013 as l on pt.Loc = l.Location 
				where pt.RowId = @RowId

				update #tempPickOccupy_013 set ThisReleaseQty = @LastQty,
				@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= OccupyQty - ReleaseQty THEN OccupyQty - ReleaseQty ELSE @Qty END
				where PickTaskUUID = @PickTaskUUID
				set @Qty = @Qty - @LastQty

				if (@Qty > 0)
				begin
					insert into #tempMsg_013(Lvl, Msg)
					values (2, N'拣货任务['+ convert(varchar, @PickTaskId) + N']的已拣数已经超过了对应发运单的拣货数。')
				end

				update #tempUpdatedLocLotDet_013 set UpdateQty = UpdateQty + @LastBaseQty,
				@BaseQty = @BaseQty - @LastBaseQty, @LastBaseQty = CASE WHEN @BaseQty >= (Qty - UpdateQty) THEN (Qty - UpdateQty) ELSE @BaseQty END
				where Location = @Location and Item = @Item and OccupyType = 0
				set @BaseQty = @BaseQty - @LastBaseQty

				if (@BaseQty > 0 and @AllowNegaInv = 0)
				begin
					insert into #tempMsg_013(Lvl, Msg)
					values (2, N'物料代码['+ @Item + N']在库位['+ @Location + N']中的库存不足。')
				end

				set @RowId = @RowId + 1
			end

			update sp set ThisPickedQty = po.ThisReleaseQty
			from #tempShipPlan_013 as sp 
			inner join (select ShipPlanId, SUM(ThisReleaseQty) as ThisReleaseQty 
						from #tempPickOccupy_013 group by ShipPlanId) as po on po.ShipPlanId = sp.ShipPlanId
		
			insert into #tempMsg_013(Lvl, Msg)
			select 2, N'和拣货任务关联的发运任务['+ convert(varchar, ShipPlanId) + N']的已拣数已经超过了待拣数。'
			from #tempShipPlan_013 where PickQty < PickedQty + ThisPickedQty


			update #tempUpdatedLocLotDet_013 set Seq = ROW_NUMBER() over (partition by Location, Item, PlanBill order by LocationLotDetId)
			where OccupyType = 1

			update lld1 set UpdateQty = lld1.Qty + lld2.Qty
			from #tempUpdatedLocLotDet_013 as lld1
			inner join (select Location, Item, PlanBill, SUM(UpdateQty) as Qty
						from #tempUpdatedLocLotDet_013 where OccupyType = 0 and UpdateQty > 0
						group by Location, Item, PlanBill) as lld2 on lld1.Location = lld2.Location and lld1.Item = lld2.Item and lld1.PlanBill = lld2.PlanBill
			where lld1.OccupyType = 1 and lld1.Seq = 1

			insert into #tempInsertedLocLotDet_013(Location, Item, Qty, PlanBill)
			select lld2.Location, lld2.Item, lld2.PlanBill, lld2.Qty
			from #tempUpdatedLocLotDet_013 as lld1
			right join (select Location, Item, PlanBill, SUM(UpdateQty) as Qty
						from #tempUpdatedLocLotDet_013 where OccupyType = 0 and UpdateQty > 0
						group by Location, Item, PlanBill) as lld2 on lld1.Location = lld2.Location and lld1.Item = lld2.Item and lld1.PlanBill = lld2.PlanBill and lld1.OccupyType = 1 and lld1.Seq = 1
			where lld1.LocationLotDetId is null

			if exists(select top 1 1 from #tempInsertedLocLotDet_013)
			begin
				declare @LocLotDetCount int 
				declare @StartLocLotDetId int 
				declare @EndLocLotDetId int 

				select @LocLotDetCount = COUNT(1) from #tempInsertedLocLotDet_013
				exec USP_SYS_BatchGetNextId 'INV_LocationLotDet', @LocLotDetCount, @EndLocLotDetId output
				select @StartLocLotDetId = @EndLocLotDetId - @LocLotDetCount + 1
				update #tempInsertedLocLotDet_013 set LocationLotDetId = ROW_NUMBER() over (order by RowId) + @StartLocLotDetId
			end
		end try
		begin catch
			set @ErrorMsg = N'数据准备出现异常：' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		if not exists(select top 1 1 from #tempMsg_013)
		begin
			begin try
				declare @Trancount int
				set @Trancount = @@trancount

				if @Trancount = 0
				begin
					begin tran
				end
				
				declare @UpdateCount int

				select @UpdateCount = COUNT(1) from #tempPickTask_013
				update pt set PickQty = pt.PickQty + tmp.ThisPickQty, IsActive = CASE WHEN pt.PickQty + tmp.ThisPickQty >= pt.OrderQty THEN 1 ELSE 0 END,
								LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = pt.[Version] + 1
				from WMS_PickTask as pt inner join #tempPickTask_013 as tmp on pt.UUID = tmp.PickTaskUUID and pt.[Version] = tmp.[Version]

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				select @UpdateCount = COUNT(1) from  #tempPickOccupy_013 where ThisReleaseQty > 0
				update po set ReleaseQty = po.ReleaseQty + tmp.ThisReleaseQty, [Version] = po.[Version] + 1
				from WMS_PickOccupy as po inner join #tempPickOccupy_013 as tmp on po.Id = tmp.Id and po.[Version] = tmp.[Version]
				where tmp.ThisReleaseQty > 0

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				select @UpdateCount = COUNT(1) from  #tempShipPlan_013 where ThisPickedQty > 0
				update sp set LockQty = sp.LockQty + tmp.ThisPickedQty, PickedQty = sp.PickedQty + tmp.ThisPickedQty, 
				LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = sp.[Version] + 1
				from WMS_ShipPlan as sp inner join #tempShipPlan_013 as tmp on sp.Id = tmp.ShipPlanId and sp.[Version] = tmp.[Version]
				where tmp.ThisPickedQty > 0

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				insert into WMS_PickResult(PickTaskId, PickTaskUUID, Item, ItemDesc, RefItemCode, Uom, BaseUom, UC, UCDesc, PickQty, Loc, 
				PickUserId, PickUserNm, CreateUser, CreateUserNm, CreateDate, IsCancel)
				select PickTaskId, PickTaskUUID, Item, ItemDesc, RefItemCode, Uom, BaseUom, UC, UCDesc, PickQty, Loc,  
				@CreateUserId, @CreateUserNm, @CreateUserId, @CreateUserNm, @DateTimeNow, 0 
				from #tempPickTask_013

				insert into WMS_BuffInv(UUID, Loc, IOType, Item, Uom, UC, Qty, IsLock, IsPack, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])
				select NEWID(), Loc, 1, Item, Uom, UC, ThisPickQty * UnitQty, 1, 0, @CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 1 
				from #tempPickTask_013

				declare @UpdateInvStatement nvarchar(max)
				declare @UpdateParameter nvarchar(max)
				select @LocationRowId = MIN(RowId), @MaxLocationRowId = MAX(RowId) from #tempLocation_013
				while (@LocationRowId <= @MaxLocationRowId)
				begin
					select @Location = Location, @LocSuffix = Suffix
					from #tempLocation_013 where RowId = @LocationRowId
					set @UpdateInvStatement = ''

					set @UpdateInvStatement = 'insert into INV_LocationLotDet_' + @LocSuffix + '(Id, Location, Item, Qty, IsCS, PlanBill, QualityType, IsFreeze, IsATP, OccupyType, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])'
					set @UpdateInvStatement = @UpdateInvStatement + 'select LocationLotDetId, Location, Item, Qty, CASE WHEN PlanBill is null THEN 0 ELSE 1 END, PlanBill, 0, 0, 1, 1, @CreateUserId_1, @CreateUserNm_1, @DateTimeNow_1, @CreateUserId_1, @CreateUserNm_1, @DateTimeNow_1, 1 from #tempInsertedLocLotDet_013 '
					set @UpdateInvStatement = @UpdateInvStatement + 'declare @UpdateCount int '
					set @UpdateInvStatement = @UpdateInvStatement + 'select @UpdateCount = COUNT(1) from #tempUpdatedLocLotDet_013 where Location = @Location_1 and UpdateQty > 0 '
					set @UpdateInvStatement = @UpdateInvStatement + 'update lld set Qty = CASE WHEN inv.OccupyType = 0 THEN inv.Qty - inv.UpdateQty ELSE inv.UpdateQty END, LastModifyUser = @CreateUserId_1, LastModifyUserNm = @CreateUserNm_1, LastModifyDate = @DateTimeNow_1, [Version] = lld.[Version] + 1 '
					set @UpdateInvStatement = @UpdateInvStatement + 'from INV_LocationLotDet_' + @LocSuffix + ' as lld '
					set @UpdateInvStatement = @UpdateInvStatement + 'inner join #tempUpdatedLocLotDet_013 as inv on lld.Id = inv.LocationLotDetId and lld.[Version] = inv.[Version] where inv.Location = @Location_1 and inv.UpdateQty > 0 '
					set @UpdateInvStatement = @UpdateInvStatement + 'if (@@ROWCOUNT <> @UpdateCount) '
					set @UpdateInvStatement = @UpdateInvStatement + 'begin '
					set @UpdateInvStatement = @UpdateInvStatement + 'RAISERROR(N''数据已经被更新，请重试。'', 16, 1) '
					set @UpdateInvStatement = @UpdateInvStatement + 'end'
					set @Parameter = N'@Location_1 varchar(50), @CreateUserId_1 int, @CreateUserNm_1 varchar(100), @DateTimeNow_1 datetime'

					exec sp_executesql @UpdateInvStatement, @UpdateParameter, @Location_1=@Location

					set @LocationRowId = @LocationRowId + 1
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
		set @ErrorMsg = N'处理数量拣货结果发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	drop table #tempPickTask_013
	drop table #tempPickOccupy_013
	drop table #tempShipPlan_013
	drop table #tempLocation_013
	drop table #tempUpdatedLocLotDet_013
	drop table #tempInsertedLocLotDet_013
END
GO