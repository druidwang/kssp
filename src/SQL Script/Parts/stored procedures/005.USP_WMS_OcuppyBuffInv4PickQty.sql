SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_OcuppyBuffInv4PickQty')
BEGIN
	DROP PROCEDURE USP_WMS_OcuppyBuffInv4PickQty
END
GO

CREATE PROCEDURE dbo.USP_WMS_OcuppyBuffInv4PickQty
	@CreatePickTaskTable CreatePickTaskTableType readonly,
	@CreateUserId int,
	@CreateUserNm varchar(100)
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)

	set @DateTimeNow = GetDate()

	create table #tempPickTarget_005
	(
		Id int identity(1, 1),
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempShipPlan_005
	(
		RowId int identity(1, 1) primary key,
		ShipPlanId int,
		[Priority] tinyint,
		StartTime DateTime,
		LocFrom varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UC decimal(18, 8),
		UnitQty decimal(18, 8),
		PickQty decimal(18, 8),		--创建的拣货数
		PickedQty decimal(18, 8),	--已拣货数
		ThisPickQty decimal(18, 8),  --本次要创建的拣货数
		ThisPickedQty decimal(18, 8),  --本次占用缓冲区的数量（本次拣货数）
		[Version] int
	)

	create table #tempBuffInv_005
	(
		RowId int identity(1, 1) primary key,
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Qty decimal(18, 8)
	)

	begin try
		if not exists(select top 1 1 FROM tempdb.sys.objects WHERE type = 'U' AND name like '#tempOccupyBuffInv_005%') 
		begin
			set @ErrorMsg = '没有定义返回数据的参数表。'
			RAISERROR(@ErrorMsg, 16, 1) 

			--代码不会执行到这里
			create table #tempOccupyBuffInv_005
			(
				Id int primary key,
				PickedQty decimal(18, 8),
			)
		end
		else
		begin
			truncate table #tempOccupyBuffInv_005
		end

		begin try
			--获取拣货库位和零件
			insert into #tempPickTarget_005(Loc, Item) 
			select distinct sp.LocFrom, sp.Item from @CreatePickTaskTable as t 
			inner join WMS_ShipPlan as sp on t.Id = sp.Id

			--获取发货计划
			insert into #tempShipPlan_005(ShipPlanId, [Priority], StartTime, LocFrom, Item, UOM, UC, UnitQty, 
			PickQty, PickedQty, ThisPickQty, ThisPickedQty, [Version])
			select sp.Id, sp.[Priority], sp.StartTime, sp.LocFrom, sp.Item, sp.Uom, sp.UC, sp.UnitQty, 
			sp.PickQty, sp.PickedQty, t.PickQty, 0, sp.[Version] 
			from @CreatePickTaskTable as t
			inner join WMS_ShipPlan as sp on t.Id = sp.Id
			order by sp.LocFrom, sp.Item, sp.[Priority], sp.StartTime, sp.Id

			--扣减缓冲区库存
			--按数量拣货：拣货完成后即增加shipPlan的PickedQty（已拣货数）和LockQty（已锁定数）
			--PickedQty（已拣货数）和LockQty（已锁定数）应该是一致的，这里只计算PickedQty
			--获取可用的Buff（库存单位）
			--不能过滤已经移动至道口的物料
			insert into #tempBuffInv_005(Loc, Item, Qty)
			select bi.Loc, bi.Item, bi.Qty
			from #tempPickTarget_005 as pt
			inner join WMS_BuffInv as bi on pt.Item = bi.Item and pt.Loc = bi.Loc
			where bi.HuId is null and bi.Qty > 0 and bi.IOType = 1  --目前只考虑发货缓存区，不考虑收货缓冲区（越库操作）
			group by bi.Loc, bi.Item

			--扣减掉发运计划占用的数量（库存单位）
			--不用单独扣减直接用发运计划占用的量(表WMS_PickOccupy)
			update bi set Qty = bi.Qty - sp.PickedQty
			from #tempBuffInv_005 as bi
			inner join (select sp.LocFrom, sp.Item, SUM((sp.PickedQty - sp.ShipQty) * sp.UnitQty) as PickedQty --转换为库存单位
						from WMS_ShipPlan as sp 
						inner join #tempPickTarget_005 as pt on sp.LocFrom = pt.Loc and sp.Item = pt.Item
						where sp.IsActive = 1 and sp.IsShipScanHu = 0 and sp.PickedQty > sp.ShipQty
						group by sp.LocFrom, sp.Item) as sp on bi.Loc = sp.LocFrom and bi.Item = sp.Item

			--发运计划占用缓冲区库存，按照发运优先级、发货时间顺序占用
			declare @RowId int
			declare @MaxRowId int
			declare @Loc varchar(50)
			declare @Item varchar(50)
			declare @Qty decimal(18, 8)
			declare @LastQty decimal(18, 8)
			select @RowId = MIN(RowId), @MaxRowId = MAX(RowId) from #tempBuffInv_005
			while (@RowId <= @MaxRowId)
			begin
				set @Qty = null
				set @LastQty = 0

				select @Loc = Loc, @Item = Item, @Qty = Qty from #tempBuffInv_005 where RowId = @RowId

				if (@Qty > 0)
				begin
					update sp set ThisPickedQty = @LastQty,
					@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= sp.ThisPickQty * sp.UnitQty THEN sp.ThisPickQty * sp.UnitQty ELSE @Qty END
					from #tempShipPlan_005 as sp
					where sp.Item = @Item and sp.LocFrom = @Loc
				end
				
				set @RowId = @RowId + 1
			end
		end try
		begin catch
			set @ErrorMsg = N'数据准备出现异常：' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		begin try
			declare @trancount int
			set @trancount = @@trancount

			if @Trancount = 0
			begin
				begin tran
			end
			
			--获取需要更新的行数
			declare @UpdateRowCount int
			select @UpdateRowCount = count(1) from #tempShipPlan_005 where ThisPickedQty > 0
			
			--更新创建拣货单的数量和已经拣货的数量
			update sp set LockQty = sp.LockQty + tmp.ThisPickedQty, PickedQty = sp.PickedQty + tmp.ThisPickedQty, PickQty = sp.PickQty + tmp.ThisPickedQty, 
			LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = sp.[Version] + 1
			from  #tempShipPlan_005 as tmp
			inner join WMS_ShipPlan as sp on tmp.ShipPlanId = sp.Id and tmp.[Version] = sp.[Version]
			where tmp.ThisPickedQty > 0

			if (@@ROWCOUNT <> @UpdateRowCount)
			begin
				RAISERROR(N'数据已经被更新，请重试。', 16, 1)
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
		set @ErrorMsg = N'占用缓冲区库存发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	insert into #tempOccupyBuffInv_005(Id, PickedQty) select ShipPlanId, ThisPickedQty from #tempShipPlan_005 where ThisPickedQty > 0

	drop table #tempPickTarget_005
	drop table #tempShipPlan_005
	drop table #tempBuffInv_005
END
GO