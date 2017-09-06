SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_CreatePickTask4PickQty')
BEGIN
	DROP PROCEDURE USP_WMS_CreatePickTask4PickQty
END
GO

CREATE PROCEDURE dbo.USP_WMS_CreatePickTask4PickQty
(
	@CreatePickTaskTable CreatePickTaskTableType readonly,
	@CreateUserId int,
	@CreateUserNm varchar(100)
)
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)

	set @DateTimeNow = GetDate()

	create table #tempShipPlan_002
	(
		ShipPlanId int primary key,
		OrderNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderSeq int,
		StartTime datetime,
		WindowTime datetime,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ItemDesc varchar(100) COLLATE  Chinese_PRC_CI_AS,
		RefItemCode varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		BaseUom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		UCDesc varchar(50),
		TargetPickQty decimal(18, 8),
		FulfillPickQty decimal(18, 8),
		[Priority] tinyint,
		LocFrom varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Dock varchar(50) COLLATE  Chinese_PRC_CI_AS,
		[Version] int
	)

	CREATE TABLE #tempPickTask_002
	(
		UUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		[Priority] tinyint,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ItemDesc varchar(100),
		RefItemCode varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		BaseUom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderQty decimal(18, 8),
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		StartTime datetime,
		WinTime datetime,
		OrderNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderSeq int,
		ShipPlanId int,
		TargetDock varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempAvailableInv_008
	(
		RowId int identity(1, 1),
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Qty decimal(18, 8)
	)

	begin try
		if not exists(select top 1 1 FROM tempdb.sys.objects WHERE type = 'U' AND name like '#tempMsg_002%') 
		begin
			set @ErrorMsg = '没有定义返回数据的参数表。'
			RAISERROR(@ErrorMsg, 16, 1) 

			--代码不会执行到这里
			create table #tempMsg_002 
			(
				Id int identity(1, 1) primary key,
				Lvl tinyint,
				Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
			)
		end
		else
		begin
			truncate table #tempMsg_002
		end

		begin try
			--占用发货缓冲区的库存
			--exec USP_WMS_OcuppyBuffInv4PickQty @CreatePickTaskTable, @CreateUserId, @CreateUserNm
			--update sp set ThisPickQty = ThisPickQty - bi.PickedQty from #tempShipPlan_002 as sp inner join #tempOccupyBuffInv_003 as bi on sp.Id = bi.Id

			--获取发运计划
			insert into #tempShipPlan_002(ShipPlanId, OrderNo, OrderSeq, StartTime, WindowTime, 
									Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc,
									TargetPickQty, FulfillPickQty, [Priority], LocFrom, Dock, [Version])
			select sp.Id, sp.OrderNo, sp.OrderSeq, sp.StartTime, sp.WindowTime, 
			sp.Item, sp.ItemDesc, sp.RefItemCode, sp.Uom, sp.BaseUom, sp.UnitQty, sp.UC, sp.UCDesc,
			tmp.PickQty, 0, sp.[Priority], sp.LocFrom, sp.Dock, sp.[Version]
			from @CreatePickTaskTable as tmp 
			inner join WMS_ShipPlan as sp on tmp.Id = sp.Id
			order by sp.StartTime, sp.Id

			--获取可用库存
			declare @PickTargetTable PickTargetTableType
			insert into @PickTargetTable(Location, Item) select distinct LocFrom, Item from #tempShipPlan_002
			exec USP_WMS_GetAvailableInv4PickQty @PickTargetTable

			declare @RowId int
			declare @MaxRowId int
			declare @Location varchar(50)
			declare @Item varchar(50)
			declare @Qty decimal(18, 8)
			declare @LastQty decimal(18, 8)
			select @RowId = MIN(RowId), @MaxRowId = MAX(RowId) from #tempAvailableInv_008
			while @RowId <= @MaxRowId
			begin
				set @LastQty = 0
				select @Location = Location, @Item = Item, @Qty = Qty 
				from #tempAvailableInv_008 where RowId = @RowId

				update sp set FulfillPickQty = @LastQty / sp.UnitQty,
				@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= sp.TargetPickQty * sp.UnitQty THEN sp.TargetPickQty * sp.UnitQty ELSE @Qty END
				from #tempShipPlan_002 as sp
				where sp.Item = @Item and sp.LocFrom = @Location
				set @Qty = @Qty - @LastQty

				insert into #tempPickTask_002(UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock,
											[Priority], Item, ItemDesc, RefItemCode , Uom , BaseUom, UnitQty, UC, UCDesc, OrderQty, 
											Loc, StartTime, WinTime)
				select NEWID(), OrderNo, OrderSeq, ShipPlanId, Dock,
				[Priority], Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, FulfillPickQty, 
				LocFrom, @DateTimeNow, case when StartTime >= @DateTimeNow then StartTime else @DateTimeNow end
				from #tempShipPlan_002 where FulfillPickQty > 0

				set @RowId = @RowId + 1 
			end
		end try
		begin catch
			set @ErrorMsg = N'数据准备出现异常：' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		begin try
			declare @Trancount int
			set @Trancount = @@trancount

			if @Trancount = 0
			begin
				begin tran
			end

			if exists(select top 1 1 from #tempPickTask_002)
			begin
				declare @UpdateCount int
				select @UpdateCount = COUNT(1) from #tempShipPlan_002 where FulfillPickQty > 0

				update sp set PickQty = sp.PickQty + tmp.FulfillPickQty, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserId, LastModifyDate = @DateTimeNow, [Version] = sp.[Version] + 1
				from #tempShipPlan_002 as tmp inner join WMS_ShipPlan as sp on tmp.ShipPlanId = sp.Id and tmp.[Version] = sp.[Version]
				where tmp.FulfillPickQty > 0

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				insert into WMS_PickTask(UUID, [Priority], Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, ShipUC, OrderQty, PickQty, 
				Loc, StartTime, WinTime, IsActive, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
				[Version], IsPickHu, PickBy, NeedRepack, IsOdd)
				select UUID, [Priority], Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, UC, OrderQty, 0, 
				Loc, StartTime, WinTime, 1, @CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 
				1, 0, 0, 0, 0
				from #tempPickTask_002

				insert into WMS_PickOccupy(UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OccupyQty, ReleaseQty,
				CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])
				select UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OrderQty, 0,
				@CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 1
				from #tempPickTask_002
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
	end try
	begin catch
		set @ErrorMsg = N'创建拣货任务发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	insert into #tempMsg_002(Lvl, Msg)
	select 0, N'发货任务['+ convert(varchar, ShipPlanId) + N']库位[' + LocFrom + N']物料代码[' + Item + N']成功创建拣货单，数量为' + convert(varchar, convert(decimal, FulfillPickQty)) + N'[' + Uom +  N']。'
	from #tempShipPlan_002 where FulfillPickQty > 0

	insert into #tempMsg_002(Lvl, Msg)
	select 1, N'发货任务['+ convert(varchar, ShipPlanId) + N']库位[' + LocFrom + N']物料代码[' + Item + N']库存缺少' + convert(varchar, convert(decimal, TargetPickQty - FulfillPickQty)) + N'[' + Uom +  N']，不能创建拣货单。'
	from #tempShipPlan_002 where TargetPickQty > FulfillPickQty

	drop table #tempShipPlan_002
	drop table #tempPickTask_002
	drop table #tempAvailableInv_008
END
GO