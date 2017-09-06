SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_CreatePickTask4PickHu')
BEGIN
	DROP PROCEDURE USP_WMS_CreatePickTask4PickHu
END
GO

CREATE PROCEDURE dbo.USP_WMS_CreatePickTask4PickHu
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
	declare @IsPickHu bit

	set @DateTimeNow = GetDate()

	create table #tempShipPlan_004
	(
		RowId int identity(1, 1) primary key,
		ShipPlanId int,
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
		UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		TargetPickQty decimal(18, 8),
		TargetFullPickQty decimal(18, 8),
		TargetOddPickQty decimal(18, 8),
		FulfillFullPickQty decimal(18, 8),
		FulfillOddPickQty decimal(18, 8),
		TempPickQty decimal(18, 8),
		[Priority] tinyint,
		LocFrom varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Dock varchar(50) COLLATE  Chinese_PRC_CI_AS,
		[Version] int
	)

	CREATE TABLE #tempPickTask_004
	(
		RowId int identity(1, 1) primary key,
		UUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		[Priority] tinyint,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ItemDesc varchar(100) COLLATE  Chinese_PRC_CI_AS,
		RefItemCode varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		BaseUom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		ShipUC decimal(18, 8),
		OrderQty decimal(18, 8),
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Area varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Bin varchar(50) COLLATE  Chinese_PRC_CI_AS,
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS,
		LotNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		StartTime datetime,
		WinTime datetime,
		NeedRepack bit,
		IsOdd bit
	)

	CREATE TABLE #tempPickOccupy_004
	(
		UUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderSeq int,
		ShipPlanId int,
		TargetDock varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OccupyQty decimal(18, 8)
	)

	create table #tempAvailableInv_010
	(
		RowId int identity(1, 1),
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UC decimal(18, 8),
		UCDesc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Area varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Bin varchar(50) COLLATE  Chinese_PRC_CI_AS,
		LotNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Qty decimal(18, 8),
		OccupyQty decimal(18, 8),
		IsOdd bit
	)

	begin try
		if not exists(select top 1 1 FROM tempdb.sys.objects WHERE type = 'U' AND name like '#tempMsg_004%') 
		begin
			set @ErrorMsg = '没有定义返回数据的参数表。'
			RAISERROR(@ErrorMsg, 16, 1) 

			--代码不会执行到这里
			create table #tempMsg_004 
			(
				Id int identity(1, 1) primary key,
				Lvl tinyint,
				Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
			)
		end
		else
		begin
			truncate table #tempMsg_004
		end

		begin try
			--占用发货缓冲区的库存
			--exec USP_WMS_OcuppyBuffInv4PicHu @CreatePickTaskTable4PickHu, @CreateUserId, @CreateUserNm
			--update sp set ThisPickQty = ThisPickQty - bi.PickedQty from #tempShipPlan_004 as sp inner join #tempOccupyBuffInv_005 as bi on sp.Id = bi.Id

			--获取发运计划
			insert into #tempShipPlan_004(ShipPlanId, OrderNo, OrderSeq, StartTime, WindowTime, 
									Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc,
									TargetPickQty, TargetFullPickQty, TargetOddPickQty, FulfillFullPickQty, 
									FulfillOddPickQty, TempPickQty, [Priority], LocFrom, Dock, [Version])
			select sp.Id, sp.OrderNo, sp.OrderSeq, sp.StartTime, sp.WindowTime, 
			sp.Item, sp.ItemDesc, sp.RefItemCode, sp.Uom, sp.BaseUom, sp.UnitQty, sp.UC, sp.UCDesc,
			tmp.PickQty, 0, 0, 0,
			0, 0, sp.[Priority], sp.LocFrom, sp.Dock, sp.[Version]
			from @CreatePickTaskTable as tmp 
			inner join WMS_ShipPlan as sp on tmp.Id = sp.Id
			order by sp.StartTime, sp.Id

			--获取可用库存
			declare @PickTargetTable PickTargetTableType
			insert into @PickTargetTable(Location, Item, Uom) select distinct LocFrom, Item, Uom from #tempShipPlan_004
			exec USP_WMS_GetAvailableInv4PickHu @PickTargetTable

			-----------------------------↓包装相同的匹配-----------------------------	
			declare @SPRowId int
			declare @MaxSPRowId int
			declare @InvRowId int
			declare @MaxInvRowId int
			declare @UUID varchar(50)
			declare @Location varchar(50)
			declare @Item varchar(50)
			declare @HuId varchar(50)
			declare @Uom varchar(5)
			declare @UC decimal(18, 8)
			declare @UCDesc varchar(50)
			declare @Area varchar(50)
			declare @Bin varchar(50)
			declare @LotNo varchar(50)
			declare @OrgQty decimal(18, 8)
			declare @Qty decimal(18, 8)
			declare @LastQty decimal(18, 8)
			declare @TargetPickQty decimal(18, 8)
			declare @TargetFullPickQty decimal(18, 8)
			declare @TargetOddPickQty decimal(18, 8)
			declare @IsOdd bit
			select @SPRowId = MIN(RowId), @MaxSPRowId = MAX(RowId) from #tempShipPlan_004
			while @SPRowId <= @MaxSPRowId
			begin
				select @Location = LocFrom, @Item = Item, @Uom = Uom, @UC = UC, @UCDesc = UCDesc,
				@TargetFullPickQty = TargetFullPickQty, @TargetOddPickQty = TargetOddPickQty
				from #tempShipPlan_004 where RowId = @SPRowId

				while (@TargetFullPickQty > 0 and exists(select top 1 1 from #tempAvailableInv_010 
							where Location = @Location and Item = @Item and Uom = @Uom and UC = @UC and IsOdd = 0))
				begin  --满包装的匹配
					select top 1 @InvRowId = RowId, @Qty = Qty, @Area = Area, @Bin = Bin, @HuId = HuId, @LotNo = LotNo from #tempAvailableInv_010 
							where Location = @Location and Item = @Item and Uom = @Uom and UC = @UC and IsOdd = 0

					insert into #tempPickTask_004(UUID, [Priority], Item, ItemDesc, RefItemCode , Uom , BaseUom, UnitQty, UC, UCDesc, ShipUC, OrderQty, 
								Loc, Area, Bin, HuId, LotNo, StartTime, WinTime, NeedRepack, IsOdd)
					select NEWID(), [Priority], Item, ItemDesc, RefItemCode , Uom , BaseUom, UnitQty, @UC, @UCDesc, UC, @Qty, 
					@Location, @Area, @Bin, @HuId, @LotNo, @DateTimeNow, case when StartTime >= @DateTimeNow then StartTime else @DateTimeNow end, 1, @IsOdd
					from #tempShipPlan_004 where RowId = @SPRowId


					select @UUID = UUID from #tempPickTask_004 where RowId = @@IDENTITY
					insert into #tempPickOccupy_004(UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OccupyQty)
					select @UUID, OrderNo, OrderSeq, ShipPlanId, Dock, @Qty from #tempShipPlan_004 where RowId = @SPRowId

					update #tempAvailableInv_010 set OccupyQty = Qty where RowId = @InvRowId
					update #tempShipPlan_004 set FulfillFullPickQty = FulfillFullPickQty + @Qty where RowId = @SPRowId

					set @TargetFullPickQty = @TargetFullPickQty - @Qty
				end

				if (@TargetOddPickQty > 0)
				begin  --零头箱的匹配
					select top 1 @InvRowId = RowId, @Qty = Qty, @Area = Area, @Bin = Bin, @HuId = HuId, @LotNo = LotNo, @UCDesc = UCDesc from #tempAvailableInv_010
					where Location = @Location and Item = @Item and Uom = @Uom and UC = @UC and Qty = @TargetOddPickQty and IsOdd = 1

					if @InvRowId is not null
					begin  --库存中有和零头拣货数量相同的箱子
						insert into #tempPickTask_004(UUID, [Priority], Item, ItemDesc, RefItemCode , Uom , BaseUom, UnitQty, UC, UCDesc, ShipUC, OrderQty, 
										Loc, Area, Bin, HuId, LotNo, StartTime, WinTime, NeedRepack, IsOdd)
						select NEWID(), [Priority], Item, ItemDesc, RefItemCode , Uom , BaseUom, UnitQty, @UC, @UCDesc, UC, @TargetOddPickQty, 
						@Location, @Area, @Bin, @HuId, @LotNo, @DateTimeNow, case when StartTime >= @DateTimeNow then StartTime else @DateTimeNow end, 0, 1
						from #tempShipPlan_004 where RowId = @SPRowId

						--记录订单占用
						select @UUID = UUID from #tempPickTask_004 where RowId = @@IDENTITY
						insert into #tempPickOccupy_004(UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OccupyQty)
						select @UUID, OrderNo, OrderSeq, ShipPlanId, Dock, TargetOddPickQty from #tempShipPlan_004 where RowId = @SPRowId

						update #tempAvailableInv_010 set OccupyQty = Qty where RowId = @InvRowId
						update #tempShipPlan_004 set FulfillOddPickQty = @TargetOddPickQty where RowId = @SPRowId
						set @TargetOddPickQty = 0
					end
				end

				set @TargetPickQty = @TargetFullPickQty + @TargetOddPickQty
				if (@TargetPickQty > 0)
				begin  --剩余未匹配的进行匹配
					while @TargetPickQty > 0 and exists(select top 1 1 from #tempAvailableInv_010 
													where Location = @Location and Item = @Item and Uom = @Uom and UC = @UC and Qty > OccupyQty)
					begin
						select top 1 @InvRowId = RowId, @Qty = Qty - OccupyQty, @Area = Area, @Bin = Bin, @HuId = HuId, @LotNo = LotNo, @IsOdd = IsOdd, @UCDesc = UCDesc
						from #tempAvailableInv_010 
						where Location = @Location and Item = @Item and Uom = @Uom and UC = @UC and Qty > OccupyQty
						order by Qty asc

						if (@IsOdd = 0 and @Qty > @TargetPickQty)
						begin
							set @Qty = CEILING((@TargetPickQty) / @UC) * @UC
						end

						insert into #tempPickTask_004(UUID, [Priority], Item, ItemDesc, RefItemCode , Uom , BaseUom, UnitQty, UC, UCDesc, ShipUC, OrderQty, 
									Loc, Area, Bin, HuId, LotNo, StartTime, WinTime, NeedRepack, IsOdd)
						select NEWID(), [Priority], Item, ItemDesc, RefItemCode , Uom , BaseUom, UnitQty, @UC, @UCDesc, UC, @Qty, 
						@Location, @Area, @Bin, @HuId, @LotNo, @DateTimeNow, case when StartTime >= @DateTimeNow then StartTime else @DateTimeNow end, 1, @IsOdd
						from #tempShipPlan_004 where RowId = @SPRowId

						--记录订单占用
						select @UUID = UUID from #tempPickTask_004 where RowId = @@IDENTITY
						insert into #tempPickOccupy_004(UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OccupyQty)
						select @UUID, OrderNo, OrderSeq, ShipPlanId, Dock, @Qty from #tempShipPlan_004 where RowId = @SPRowId

						update #tempAvailableInv_010 set OccupyQty = Qty where RowId = @InvRowId
						update #tempShipPlan_004 set FulfillFullPickQty = FulfillFullPickQty + @Qty where RowId = @SPRowId

						set @TargetPickQty = @TargetPickQty - @Qty
					end
				end

				set @SPRowId = @SPRowId + 1
			end
			-----------------------------↑包装相同的匹配-----------------------------



			-----------------------------↓包装不相同的匹配-----------------------------	
			select @InvRowId = MIN(RowId), @MaxInvRowId = MAX(RowId) from #tempAvailableInv_010
			while @InvRowId <= @MaxInvRowId
			begin
				set @LastQty = 0
				select @Location = Location, @Item = Item, @Uom = Uom, @UC = UC, @UCDesc = UCDesc,
				@Area = Area, @Bin = Bin, @HuId = HuId, @LotNo = LotNo,
				@OrgQty = Qty - OccupyQty, @Qty = Qty - OccupyQty, @IsOdd = IsOdd 
				from #tempAvailableInv_010 where RowId = @MaxInvRowId
				
				if (@Qty > 0)
				begin
					update sp set TempPickQty = @LastQty, FulfillFullPickQty = sp.FulfillFullPickQty + @LastQty,
					@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= (sp.TargetPickQty - sp.FulfillFullPickQty - sp.FulfillOddPickQty) THEN (sp.TargetPickQty - sp.FulfillFullPickQty - sp.FulfillOddPickQty) ELSE @Qty END
					from #tempShipPlan_004 as sp
					where sp.Item = @Item and sp.LocFrom = @Location and sp.Uom = @Uom
					and sp.TargetFullPickQty > 0
					set @Qty = @Qty - @LastQty

					if (@OrgQty > @Qty)
					begin
						insert into #tempPickTask_004(UUID, [Priority], Item, ItemDesc, RefItemCode , Uom , BaseUom, UnitQty, UC, UCDesc, ShipUC, OrderQty, 
													Loc, Area, Bin, HuId, LotNo, StartTime, WinTime, NeedRepack, IsOdd)
						select top 1 NEWID(), [Priority], Item, ItemDesc, RefItemCode , Uom , BaseUom, UnitQty, @UC, @UCDesc, UC, @OrgQty, 
						@Location, @Area, @HuId, @Bin, @LotNo, @DateTimeNow, case when StartTime >= @DateTimeNow then StartTime else @DateTimeNow end, 1, @IsOdd
						from #tempShipPlan_004 where TempPickQty > 0
						order by StartTime asc

						--记录订单占用
						select @UUID = UUID from #tempPickTask_004 where RowId = @@IDENTITY
						insert into #tempPickOccupy_004(UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OccupyQty)
						select @UUID, OrderNo, OrderSeq, ShipPlanId, Dock, TempPickQty from #tempShipPlan_004 where TempPickQty > 0

						update #tempAvailableInv_010 set OccupyQty = OccupyQty + @OrgQty where RowId = @MaxInvRowId
						update #tempShipPlan_004 set TempPickQty = 0 where TempPickQty > 0
					end
				end

				set @MaxInvRowId = @MaxInvRowId - 1
			end			
			-----------------------------↑包装不相同的匹配-----------------------------

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

			if exists(select top 1 1 from #tempPickTask_004)
			begin
				declare @UpdateCount int
				select @UpdateCount = COUNT(1) from #tempShipPlan_004 where (FulfillFullPickQty + FulfillOddPickQty) > 0

				update sp set PickQty = sp.PickQty + tmp.FulfillFullPickQty + tmp.FulfillOddPickQty, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserId, LastModifyDate = @DateTimeNow, [Version] = sp.[Version] + 1
				from #tempShipPlan_004 as tmp inner join WMS_ShipPlan as sp on tmp.ShipPlanId = sp.Id and tmp.[Version] = sp.[Version]
				where (tmp.FulfillFullPickQty + tmp.FulfillOddPickQty) > 0

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				insert into WMS_PickTask(UUID, [Priority], Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, ShipUC, OrderQty, PickQty, 
				Loc, Area, Bin, LotNo, HuId, StartTime, WinTime, IsActive, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
				[Version], IsPickHu, PickBy, NeedRepack, IsOdd)
				select UUID, [Priority], Item, ItemDesc, RefItemCode, Uom, BaseUom, UnitQty, UC, UCDesc, ShipUC, OrderQty, 0, 
				Loc, Area, Bin, LotNo, HuId, StartTime, WinTime, 1, @CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 
				1, 1, 0, NeedRepack, IsOdd
				from #tempPickTask_004

				insert into WMS_PickOccupy(UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OccupyQty, ReleaseQty,
				CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])
				select UUID, OrderNo, OrderSeq, ShipPlanId, TargetDock, OccupyQty, 0,
				@CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 1
				from #tempPickOccupy_004
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

	insert into #tempMsg_004(Lvl, Msg)
	select 0, N'发货任务['+ convert(varchar, ShipPlanId) + N']库位[' + LocFrom + N']物料代码[' + Item + N']成功创建拣货单，数量为' + convert(varchar, convert(decimal, FulfillFullPickQty + FulfillOddPickQty)) + N'[' + Uom +  N']。'
	from #tempShipPlan_004 where (FulfillFullPickQty + FulfillOddPickQty) > 0

	insert into #tempMsg_004(Lvl, Msg)
	select 1, N'发货任务['+ convert(varchar, ShipPlanId) + N']库位[' + LocFrom + N']物料代码[' + Item + N']库存缺少' + convert(varchar, convert(decimal, TargetPickQty - (FulfillFullPickQty + FulfillOddPickQty))) + N'[' + Uom +  N']，不能创建拣货单。'
	from #tempShipPlan_004 where TargetPickQty > (FulfillFullPickQty + FulfillOddPickQty)

	drop table #tempShipPlan_004
	drop table #tempPickTask_004
	drop table #tempPickOccupy_004
	drop table #tempAvailableInv_010
END
GO