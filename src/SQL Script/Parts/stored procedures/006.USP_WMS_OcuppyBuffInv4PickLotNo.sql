SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_OcuppyBuffInv4PickLotNo')
BEGIN
	DROP PROCEDURE USP_WMS_OcuppyBuffInv4PickLotNo
END
GO

CREATE PROCEDURE dbo.USP_WMS_OcuppyBuffInv4PickLotNo
	@CreatePickTaskTable CreatePickTaskTableType readonly,
	@CreateUserId int,
	@CreateUserNm varchar(100)
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)
	declare @trancount int

	set @DateTimeNow = GetDate()

	create table #tempPickTarget_006
	(
		Id int identity(1, 1),
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempShipPlan_006
	(
		RowId int identity(1, 1) primary key,
		OrderNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderSeq int,
		ShipPlanId int,
		TargetDock varchar(50) COLLATE  Chinese_PRC_CI_AS,
		[Priority] tinyint,
		StartTime DateTime,
		LocFrom varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UC decimal(18, 8),
		UnitQty decimal(18, 8),
		LockQty decimal(18, 8),		--可发货的数量
		PickQty decimal(18, 8),		--创建的拣货数
		PickedQty decimal(18, 8),	--已拣货数
		ThisLockQty decimal(18, 8),  --本次冻结的可发货数量
		ThisPickQty decimal(18, 8),  --本次要创建的拣货数
		ThisPickFullQty decimal(18, 8),  --本次要创建的拣货数（整包装）
		ThisPickOddQty decimal(18, 8),  --本次要创建的拣货数（零头数）
		ThisPickedQty decimal(18, 8),  --本次占用缓冲区的数量（本次拣货数）
		[Version] int,
		OddRowId int
	)

	create table #tempBuffInv_006
	(
		RowId int identity(1, 1) primary key,
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UC decimal(18, 8),
		Qty decimal(18, 8),
		IsOdd bit   --是否零头箱
	)

	create table #tempCreatedShipPlan_006
	(
		RowId int identity(1, 1) primary key,
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UC decimal(18, 8),
		RemainLockQty decimal(18, 8),
		RemainRepackQty decimal(18, 8),
		RepackRowId int
	)

	create table #tempBuffOccupy_006
	(
		BuffInvId int, 
		OrderNo varchar(50) COLLATE  Chinese_PRC_CI_AS, 
		OrderSeq int, 
		ShipPlanId int, 
		TargetDock varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	begin try
		if not exists(select top 1 1 FROM tempdb.sys.objects WHERE type = 'U' AND name like '#tempOccupyBuffInv_006%') 
		begin
			set @ErrorMsg = '没有定义返回数据的参数表。'
			RAISERROR(@ErrorMsg, 16, 1) 

			--代码不会执行到这里
			create table #tempOccupyBuffInv_006
			(
				Id int primary key,
				PickedQty decimal(18, 8),
			)
		end
		else
		begin
			truncate table #tempOccupyBuffInv_006
		end

		begin try
			--获取拣货库位和零件
			insert into #tempPickTarget_006(Loc, Item) 
			select distinct sp.LocFrom, sp.Item from @CreatePickTaskTable as t 
			inner join WMS_ShipPlan as sp on t.Id = sp.Id

			--获取发货计划
			insert into #tempShipPlan_006(OrderNo, OrderSeq, ShipPlanId, TargetDock, [Priority], StartTime, LocFrom, Item, UOM, UC, UnitQty, 
			LockQty, PickQty, PickedQty, ThisLockQty, ThisPickQty, ThisPickFullQty, ThisPickOddQty, ThisPickedQty, [Version])
			select sp.OrderNo, sp.OrderSeq, sp.Id, sp.Dock, sp.[Priority], sp.StartTime, sp.LocFrom, sp.Item, sp.Uom, sp.UC, sp.UnitQty, 
			sp.LockQty, sp.PickQty, sp.PickedQty, 0, t.PickQty, ROUND(t.PickQty / sp.UC, 0, 1) * sp.UC, t.PickQty % sp.UC, 0, sp.[Version] 
			from @CreatePickTaskTable as t
			inner join WMS_ShipPlan as sp on t.Id = sp.Id
			order by sp.LocFrom, sp.Item, sp.[Priority], sp.StartTime, sp.Id

			--扣减缓冲区库存
			--拣货完成后就增加PickedQty
			--要等缓冲区的库存和ShipPlan的UOM和UC完全匹配或者通过配送条码手工关联ShipPlan，才会增加shipPlan的LockQty（已锁定数）
			--获取可用的Buff（库存单位、满包装）,去除直接被发运计划占用的库存
			--不能过滤已经移动至道口的物料
			insert into #tempBuffInv_006(Loc, Item, Uom, UC, Qty, IsOdd)
			select bi.Loc, bi.Item, hu.Uom, hu.UC, SUM(bi.Qty), 0
			from #tempPickTarget_006 as pt
			inner join WMS_BuffInv as bi on pt.Item = bi.Item and pt.Loc = bi.Loc
			inner join INV_Hu as hu on bi.HuId = hu.HuId
			left join WMS_BuffOccupy as bo on bi.Id = bo.BuffInvId
			where bi.Qty > 0 and bi.IOType = 1  --目前只考虑发货缓存区，不考虑收货缓冲区（越库操作）
			and hu.UC = hu.Qty  --满包装
			and bo.BuffInvId is null
			group by bi.Loc, bi.Item, hu.Uom, hu.UC

			--获取可用的Buff（库存单位、零头包装）,去除直接被发运计划占用的库存
			--不能过滤已经移动至道口的物料
			insert into #tempBuffInv_006(Loc, Item, Uom, UC, Qty, IsOdd)
			select bi.Loc, bi.Item, hu.Uom, hu.UC, SUM(bi.Qty), 1
			from #tempPickTarget_006 as pt
			inner join WMS_BuffInv as bi on pt.Item = bi.Item and pt.Loc = bi.Loc
			inner join INV_Hu as hu on bi.HuId = hu.HuId
			left join WMS_BuffOccupy as bo on bi.Id = bo.BuffInvId
			where bi.Qty > 0 and bi.IOType = 1  --目前只考虑发货缓存区，不考虑收货缓冲区（越库操作）
			and hu.UC <> hu.Qty  --零头包装
			and bo.BuffInvId is null  --去除直接被发运计划占用的库存
			group by bi.Loc, bi.Item, hu.Uom, hu.UC

			--获取并汇总发运计划的可发货数和可用的已拣货数，汇总时去除直接被发运计划占用的库存
			--数量为库存单位
			insert into #tempCreatedShipPlan_006(Loc, Item, Uom, UC, RemainLockQty, RemainRepackQty)
			select sp.LocFrom, sp.Item, sp.Uom, sp.UC,
			SUM((sp.LockQty - sp.ShipQty) * sp.UnitQty - ISNULL(occ.Qty, 0)) as RemainLockQty,  --可发货数（去除了直接占用的库存的部分）
			SUM((sp.PickedQty - sp.LockQty) * sp.UnitQty) as RemainRepackQty  --需要翻包的数量
			from WMS_ShipPlan as sp
			inner join #tempPickTarget_006 as pt on sp.LocFrom = pt.Loc and sp.Item = pt.Item
			left join (select bo.ShipPlanId, SUM(bi.Qty) as Qty 
						from #tempPickTarget_006 as pt
						inner join WMS_BuffInv as bi on pt.Item = bi.Item and pt.Loc = bi.Loc
						inner join WMS_BuffOccupy as bo on bi.Id = bo.BuffInvId
						where bi.Qty > 0 and bi.IOType = 1
						group by bo.ShipPlanId) as occ on sp.Id = occ.ShipPlanId
			where sp.IsActive = 1 and sp.IsShipScanHu = 1 and sp.PickedQty > sp.ShipQty
			group by sp.LocFrom, sp.Item, sp.Uom, sp.UC

			--扣减被锁定的库存（满包装、包装匹配）
			update bi set Qty = bi.Qty - ISNULL(sp.RemainLockQty, 0)
			from #tempBuffInv_006 as bi 
			left join #tempCreatedShipPlan_006 as sp on bi.Loc = sp.Loc and bi.Item = sp.Item and bi.Uom = sp.Uom and bi.UC = sp.UC
			where bi.IsOdd = 0

			--扣减被锁定的库存（需要翻箱的、包装不匹配和零头箱）
			update #tempCreatedShipPlan_006 set RepackRowId = ROW_NUMBER() over(order by LocFrom, Item, Uom, UC)
			where RemainRepackQty > 0

			declare @RowId int
			declare @MaxRowId int
			declare @Loc varchar(50)
			declare @Item varchar(50)
			declare @Uom varchar(5)
			declare @UC decimal(18, 8)
			declare @Qty decimal(18, 8)
			declare @LastQty decimal(18, 8)
			select @RowId = MIN(RepackRowId), @MaxRowId = MAX(RepackRowId) from #tempCreatedShipPlan_006
			while (@RowId <= @MaxRowId)
			begin
				set @Qty = null
				set @LastQty = 0

				select @Loc = Loc, @Item = Item, @Uom = Uom, @UC = UC, @Qty = RemainRepackQty
				from #tempCreatedShipPlan_006 as sp where RepackRowId = @RowId
			
				--优先相同包装的零头箱
				update bi set Qty = Qty - @LastQty,
				@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= Qty THEN Qty ELSE @Qty END
				from #tempBuffInv_006 as bi
				where bi.Item = @Item and bi.Loc = @Loc  and bi.Uom = @Uom and bi.UC = @UC
				and bi.IsOdd = 1
				set @Qty = @Qty - @LastQty

				--其次扣减包装不相同的零头箱
				if (@Qty > 0)
				begin
					update bi set Qty = Qty - @LastQty,
					@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= Qty THEN Qty ELSE @Qty END
					from #tempBuffInv_006 as bi
					where bi.Item = @Item and bi.Loc = @Loc  and bi.Uom = @Uom and bi.UC <> @UC
					and bi.IsOdd = 1
					set @Qty = @Qty - @LastQty
				end

				--最后扣减包装不相同的整箱
				if (@Qty > 0)
				begin
					update bi set Qty = Qty - @LastQty,
					@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= Qty THEN Qty ELSE @Qty END
					from #tempBuffInv_006 as bi
					where bi.Item = @Item and bi.Loc = @Loc  and bi.Uom = @Uom and bi.UC <> @UC
					and bi.IsOdd = 0
					set @Qty = @Qty - @LastQty
				end
			
				set @RowId = @RowId + 1
			end

			--有零头的发运计划在单独编号，后面要计算零头箱占用库存
			update sp set OddRowId = ROW_NUMBER() over(order by sp.LocFrom, sp.Item, sp.[Priority], sp.StartTime, sp.Id)
			from #tempShipPlan_006 as sp inner join #tempBuffInv_006 as bi on sp.LocFrom = bi.Loc and sp.Item = bi.Item and sp.UOM = bi.UOM and sp.UC = bi.UC
			where (bi.Qty > = (sp.ThisPickOddQty * sp.UnitQty)) and bi.IsOdd = 1

			--拣货的零头按条码占用缓冲库存
			declare @OrderNo varchar(50)
			declare @OrderSeq int
			declare @ShipPlanId int
			declare @TargetDock varchar(50)
			declare @OddQty decimal(18, 8)  --库存单位
			select @RowId = MIN(OddRowId), @MaxRowId = MAX(OddRowId) from #tempShipPlan_006
			while (@RowId <= @MaxRowId)
			begin 
				select @Loc = LocFrom, @Item = Item, @Uom = Uom, @UC = UC, @OddQty = ThisPickOddQty * UnitQty,
				@OrderNo = OrderNo, @OrderSeq = OrderSeq, @ShipPlanId = ShipPlanId, @TargetDock = TargetDock
				from #tempShipPlan_006 as sp where OddRowId = @RowId

				--发运计划直接占用零头
				insert into #tempBuffOccupy_006(BuffInvId, OrderNo, OrderSeq, ShipPlanId, TargetDock)
				select top 1 bi.Id, @OrderNo, @OrderSeq, @ShipPlanId, @TargetDock
				from WMS_BuffInv as bi
				inner join INV_Hu as hu on bi.HuId = hu.HuId
				left join WMS_BuffOccupy as bo on bi.Id = bo.BuffInvId
				where bi.Loc = @Loc and bi.Item = @Item and hu.Uom = @Uom and hu.UC = @UC and bi.Qty = @OddQty
				and bo.BuffInvId is null
				order by bi.Id
					
				if (@@ROWCOUNT = 1)
				begin
					update #tempShipPlan_006 set ThisLockQty = ThisLockQty + @OddQty, ThisPickedQty = ThisPickedQty + @OddQty
					update #tempBuffInv_006 set Qty = Qty - @OddQty where Loc = @Loc and Item = @Item and UOM = @Uom and UC = @UC and Qty = @OddQty
				end

				set @RowId = @RowId + 1
			end
			
			--发运计划占用缓冲区库存，按照发运优先级、发货时间顺序占用
			declare @IsOdd bit
			select @RowId = MIN(RowId), @MaxRowId = MAX(RowId) from #tempBuffInv_006
			while (@RowId <= @MaxRowId)
			begin  --循环占用缓冲区库存
				set @Qty = null
				set @LastQty = 0

				select @Loc = Loc, @Item = Item, @Uom = Uom, @UC = UC, @IsOdd = IsOdd, @Qty = Qty, @LastQty = 0 
				from #tempBuffInv_006 where RowId = @RowId

				if (@Qty > 0)
				begin
					--库存已经按满包装、零头箱排序
					if @IsOdd = 0
					begin  --满箱匹配，直接增加锁定的库存数量
						update sp set ThisLockQty = @LastQty,
						@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= (sp.ThisPickFullQty * sp.UnitQty) THEN sp.ThisPickFullQty * sp.UnitQty ELSE @Qty END
						from #tempShipPlan_006 as sp
						where sp.Item = @Item and sp.LocFrom = @Loc and sp.Uom = @Uom and sp.UC = @UC
					end
					else
					begin  --零头箱匹配，直接增加已拣货的库存数量
						update sp set ThisPickedQty = @LastQty,
						@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= ((sp.ThisPickQty - sp.ThisLockQty) * sp.UnitQty) THEN (sp.ThisPickQty - sp.ThisLockQty) * sp.UnitQty ELSE @Qty END
						from #tempShipPlan_006 as sp
						where sp.Item = @Item and sp.LocFrom = @Loc and sp.Uom = @Uom
					end
				end
				
				set @RowId = @RowId + 1
			end

			--已拣货数 = 已拣货数 + 锁定的库存数量
			update #tempShipPlan_006 set ThisPickedQty = ThisPickedQty + ThisLockQty
		end try
		begin catch
			set @ErrorMsg = N'数据准备出现异常：' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		begin try
			set @trancount = @@trancount
			if @Trancount = 0
			begin
				begin tran
			end

			--直接占用缓冲区的库存
			insert into WMS_BuffOccupy(BuffInvId, OrderNo, OrderSeq, ShipPlanId, TargetDock, 
			CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])
			select BuffInvId, OrderNo, OrderSeq, ShipPlanId, TargetDock, 
			@CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 1
			from #tempBuffOccupy_006

			--获取需要更新的行数
			declare @UpdateRowCount int
			select @UpdateRowCount = count(1) from #tempShipPlan_006 where ThisPickedQty > 0
			
			--更新创建拣货单的数量和已经拣货的数量
			update sp set LockQty = sp.LockQty + tmp.ThisLockQty, PickedQty = sp.PickedQty + tmp.ThisPickedQty, PickQty = sp.PickQty + tmp.ThisPickedQty, 
			LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = sp.[Version] + 1
			from  #tempShipPlan_006 as tmp
			inner join WMS_ShipPlan as sp on tmp.ShipPlanId = sp.Id and tmp.[Version] = sp.[Version]
			where tmp.ThisPickedQty > 0
			
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

	insert into #tempOccupyBuffInv_006(Id, PickedQty) select ShipPlanId, ThisPickedQty from #tempShipPlan_006 where ThisPickedQty > 0

	drop table #tempPickTarget_006
	drop table #tempShipPlan_006
	drop table #tempBuffInv_006
	drop table #tempCreatedShipPlan_006
	drop table #tempBuffOccupy_006
END
GO