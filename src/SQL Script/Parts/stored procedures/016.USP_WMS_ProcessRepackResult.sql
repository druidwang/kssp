SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_ProcessRepackResult')
BEGIN
	DROP PROCEDURE USP_WMS_ProcessRepackResult
END
GO

CREATE PROCEDURE dbo.USP_WMS_ProcessRepackResult
	@RepackTaskId int,
	@RepackResultIn RepackResultTableType readonly,
	@RepackResultOut RepackResultTableType readonly,
	@CreateUserId int,
	@CreateUserNm varchar(100),
	@EffDate datetime
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)
	
	set @DateTimeNow = GetDate()

	if @EffDate is null
	begin
		set @EffDate = @DateTimeNow
	end

	create table #tempMsg_016 
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempRepackInHu_016
	(
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS primary key,
		LotNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		Qty  decimal(18, 8),
	)

	create table #tempRepackOutHu_016
	(
		LocLotDetId int,
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS primary key,
		LotNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		Qty  decimal(18, 8),
	)

	create table #tempLocationLotDet_016
	(
		Id int primary key,
		Bin varchar(50) COLLATE  Chinese_PRC_CI_AS,
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS,
		IsCS bit,
		PlanBill int,
		QualityType tinyint,
		IsFreeze bit,
		OccupyType tinyint,
		[Version] int,
	)

	create table #tempBuffInv_016
	(
		UUID varchar(50) COLLATE  Chinese_PRC_CI_AS primary key,
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS,
		IsLock bit,
		[Version] int
	)

	create table #tempRepackOccupy_016
	(
		Id int Primary Key,
		ShipPlanId int,
		OccupyQty decimal(18, 8),
		ReleaseQty decimal(18, 8),
		ThisReleaseQty decimal(18, 8),
		[Version] int
	)

	create table #tempShipPlan_016
	(
		ShipPlanId int Primary Key,
		PickedQty decimal(18, 8),
		LockQty decimal(18, 8),
		ThisLockQty decimal(18, 8),
		[Version] int,
	)

	begin try
		begin try
			if @RepackTaskId is null
			begin
				insert into #tempMsg_016(Lvl, Msg) values(2, N'翻包任务不能为空。')
			end

			if not exists(select top 1 1 from @RepackResultIn)
			begin
				insert into #tempMsg_016(Lvl, Msg) values(2, N'翻包前的条码不能为空。')
			end

			if not exists(select top 1 1 from @RepackResultIn)
			begin
				insert into #tempMsg_016(Lvl, Msg) values(2, N'翻包后的条码不能为空。')
			end

			if not exists(select top 1 1 from #tempMsg_016)
			begin
				declare @UUID varchar(50)
				declare @Region varchar(50)
				declare @Location varchar(50)
				declare @LocSuffix varchar(50)
				declare @Item varchar(50)
				declare @Uom varchar(5)
				declare @BaseUom varchar(5)
				declare @UnitQty decimal(18, 8)
				declare @UC decimal(18, 8)
				declare @Qty decimal(18, 8)
				declare @RepackQty decimal(18, 8)
				declare @IsActive bit
				declare @Version int
				declare @IsCS bit
				declare @PlanBill int
				declare @InQty decimal(18, 8)
				declare @OutQty decimal(18, 8)
				declare @LastQty decimal(18, 8) = 0
				declare @selectStatement nvarchar(max)
				declare @Parameter nvarchar(max)
				declare @LocTransCount int
				declare @EndLocLotDetId int
				declare @StartLocLotDetId int
				declare @EndTransId int
				declare @StartInTransId int
				declare @StartOutTransId int

				select @UUID = UUID, @Location = Loc, @Item = Item, @Uom = Uom, @BaseUom = BaseUom, @UnitQty = UnitQty, @UC = UC, 
				@Qty = Qty, @RepackQty = RepackQty, @IsActive = IsActive, @Version = [Version]
				from WMS_RepackTask where Id = @RepackTaskId

				if @UUID is null
				begin
					insert into #tempMsg_016(Lvl, Msg) values(2, N'翻包任务[' + convert(varchar, @RepackTaskId) + N']不存在。')
				end
				else if @IsActive = 0
				begin
					insert into #tempMsg_016(Lvl, Msg) values(2, N'翻包任务[' + convert(varchar, @RepackTaskId) + N']已经关闭。')
				end

				insert into #tempRepackInHu_016(HuId, LotNo, Item, Uom, UnitQty, UC, Qty)
				select rri.HuId, hu.LotNo, hu.Item, hu.Uom, hu.UnitQty, hu.UC, hu.Qty
				from INV_Hu as hu right join @RepackResultIn as rri on hu.HuId = rri.HuId

				insert into #tempRepackOutHu_016(HuId, LotNo, Item, Uom, UnitQty, UC, Qty)
				select rro.HuId, hu.LotNo, hu.Item, hu.Uom, hu.UnitQty, hu.UC, hu.Qty
				from INV_Hu as hu right join @RepackResultOut as rro on hu.HuId = rro.HuId

				insert into #tempMsg_016(Lvl, Msg)
				select 2, N'翻包前的条码['+ HuId + N']不存在。' from #tempRepackInHu_016 where Item is null

				insert into #tempMsg_016(Lvl, Msg)
				select 2, N'翻包后的条码['+ HuId + N']不存在。' from #tempRepackOutHu_016 where Item is null

				if not exists(select top 1 1 from #tempMsg_016)
				begin
					insert into #tempMsg_016(Lvl, Msg)
					select 2, N'翻包前条码['+ HuId + N']大于1条。' 
					from @RepackResultIn group by HuId having COUNT(1) > 1

					insert into #tempMsg_016(Lvl, Msg)
					select 2, N'翻包后条码['+ HuId + N']大于1条。' 
					from @RepackResultOut group by HuId having COUNT(1) > 1

					insert into #tempMsg_016(Lvl, Msg)
					select 2, N'翻包前条码['+ HuId + N']的物料代码和翻包任务的物料代码不匹配。' 
					from #tempRepackInHu_016 where Item <> @Item

					insert into #tempMsg_016(Lvl, Msg)
					select 2, N'翻包后条码['+ HuId + N']的物料代码和翻包任务的物料代码不匹配。' 
					from #tempRepackOutHu_016 where Item <> @Item

					insert into #tempMsg_016(Lvl, Msg)
					select 2, N'翻包前条码['+ HuId + N']的单位和翻包任务的单位不匹配。' 
					from #tempRepackInHu_016 where Uom <> @Uom

					insert into #tempMsg_016(Lvl, Msg)
					select 2, N'翻包后条码['+ HuId + N']的单位和翻包任务的单位不匹配。' 
					from #tempRepackOutHu_016 where Uom <> @Uom

					--insert into #tempMsg_016(Lvl, Msg)
					--select 2, N'翻包前条码['+ HuId + N']的包装和翻包任务的包装不匹配。' 
					--from #tempRepackInHu_016 where UC <> @UC

					insert into #tempMsg_016(Lvl, Msg)
					select 2, N'翻包后条码['+ HuId + N']的包装和翻包任务的包装不匹配。' 
					from #tempRepackOutHu_016 where UC <> @UC

					select @Region = Region, @LocSuffix = PartSuffix from MD_Location where Code = @Location

					set @selectStatement = 'insert into #tempLocationLotDet_016(Id, Bin, HuId, IsCS, PlanBill, QualityType, IsFreeze, OccupyType, [Version]) '
					set @selectStatement = @selectStatement + 'select lld.Id, lld.Bin, rih.HuId, lld.IsCS, lld.PlanBill, lld.QualityType, lld.IsFreeze, lld.OccupyType, lld.[Version] '
					set @selectStatement = @selectStatement + 'from INV_LocationLotDet_' + @LocSuffix + ' as lld right join #tempRepackInHu_016 as rih on lld.HuId = rih.HuId and lld.Qty > 0 and lld.Location = @Location_1'
					set @Parameter = N'@Location_1 varchar(50)'

					exec sp_executesql @selectStatement, @Parameter, @Location_1=@Location

					insert into #tempBuffInv_016(UUID, HuId, IsLock, [Version]) 
					select bi.UUID, rih.HuId, bi.IsLock, bi.[Version] from WMS_BuffInv as bi 
					inner join #tempRepackInHu_016 as rih on bi.HuId = rih.HuId and bi.Loc = @Location and bi.IOType = 1

					insert into #tempMsg_016(Lvl, Msg)
					select 2, N'翻包前条码['+ HuId + N']在库存中存在多个。' 
					from #tempLocationLotDet_016 group by HuId having COUNT(1) > 1
					union
					select 2, N'翻包前条码['+ HuId + N']在库存中存在多个。' 
					from #tempBuffInv_016 group by HuId having COUNT(1) > 1

					insert into #tempMsg_016(Lvl, Msg) select 2, N'翻包前的条码['+ HuId + N']不在库位[' + @Location + ']中，不能进行翻包。' 
					from #tempLocationLotDet_016 where Id is null

					insert into #tempMsg_016(Lvl, Msg) select 2, N'翻包前的条码['+ HuId + N']在库格[' + Bin + ']中，不能进行翻包。' 
					from #tempLocationLotDet_016 where Bin is not null

					insert into #tempMsg_016(Lvl, Msg) select 2, N'翻包前的条码['+ HuId + N']不在库位[' + @Location + ']的发货缓冲区中，不能进行翻包。' 
					from #tempBuffInv_016 where UUID is null

					insert into #tempMsg_016(Lvl, Msg) 
					select 2, N'翻包前的条码['+ bi.HuId + N']已经被占用。' 
					from #tempBuffInv_016 as bi inner join WMS_BuffOccupy as bo on bi.UUID = bo.UUID
					group by bi.HuId
					union 
					select 2, N'翻包前的条码['+ HuId + N']已经被占用。' 
					from #tempBuffInv_016 where IsLock = 1
					--union
					--select 2, N'翻包前的条码['+ HuId + N']已经被占用。' 
					--from #tempLocationLotDet_016 where OccupyType <> 0

					insert into #tempMsg_016(Lvl, Msg) select 2, N'翻包前的条码['+ HuId + N']已经被冻结。' 
					from #tempLocationLotDet_016 where IsFreeze = 1

					insert into #tempMsg_016(Lvl, Msg) select 2, N'翻包前的条码['+ HuId + N']质量状态不是合格。' 
					from #tempLocationLotDet_016 where QualityType <> 0

					if exists(select top 1 1 from (select ROW_NUMBER() over (order by IsCS, PlanBill) as RowId 
									from #tempLocationLotDet_016 group by IsCS, PlanBill) as tmp where tmp.RowId > 1)
					begin
						insert into #tempMsg_016(Lvl, Msg) select 2, N'翻包前条码的寄售方式不一致或寄售供应商不一致，不能进行翻包。' 
					end
					else
					begin
						select top 1 @PlanBill = PlanBill from #tempLocationLotDet_016 where IsCS = 1
						set @IsCS = CASE WHEN @PlanBill is null THEN 0 ELSE 1 END
					end

					insert into #tempMsg_016(Lvl, Msg)
					select 2, N'翻包后的条码['+ roh.HuId + N']在已经库位[' + lld.Location + ']中存在。'
					from VIEW_LocationLotDet as lld 
					inner join #tempRepackOutHu_016 as roh on lld.HuId = roh.HuId where lld.Qty > 0

					select @InQty = SUM(Qty) from #tempRepackInHu_016
					select @OutQty = SUM(Qty) from #tempRepackOutHu_016

					if @InQty <> @OutQty
					begin
						insert into #tempMsg_016(Lvl, Msg) values(2, N'翻包任务[' + convert(varchar, @RepackTaskId) + N']翻包后的条码数量[' + convert(varchar, convert(decimal, @OutQty)) + N']和翻包后的条码数量[' + convert(varchar, convert(decimal, @InQty)) + N']不相等。')
					end
					else if @OutQty > @Qty - @RepackQty
					begin
						insert into #tempMsg_016(Lvl, Msg) values(2, N'翻包的条码数量[' + convert(varchar, convert(decimal, @OutQty)) + N']和超过了翻包任务[' + convert(varchar, @RepackTaskId) + N']的数量[' + convert(varchar, convert(decimal, @Qty - @RepackQty)) + N']。')
					end

					if not exists(select top 1 1 from #tempMsg_016)
					begin
						insert into #tempRepackOccupy_016(Id, ShipPlanId, OccupyQty, ReleaseQty, ThisReleaseQty, [Version])
						select Id, ShipPlanId, OccupyQty, ReleaseQty, 0, [Version] 
						from WMS_RepackOccupy where UUID = @UUID and OccupyQty > ReleaseQty

						update #tempRepackOccupy_016 set ThisReleaseQty = @LastQty,
						@OutQty = @OutQty - @LastQty, @LastQty = CASE WHEN @OutQty >= OccupyQty - ReleaseQty THEN OccupyQty - ReleaseQty ELSE @OutQty END
						set @OutQty = @OutQty - @LastQty

						if (@OutQty > 0)
						begin
							insert into #tempMsg_016(Lvl, Msg)values (2, N'翻包任务['+ convert(varchar, @RepackTaskId) + N']的已翻包数量已经超过了对应发运单的拣货数。')
						end
						
						insert into #tempShipPlan_016(ShipPlanId, PickedQty, LockQty, ThisLockQty, [Version])
						select sp.Id, sp.PickedQty, sp.LockQty, ro.ThisReleaseQty, sp.[Version] 
						from #tempRepackOccupy_016 as ro inner join WMS_ShipPlan as sp on ro.ShipPlanId = sp.Id

						insert into #tempMsg_016(Lvl, Msg)
						select 2, N'翻包任务['+ convert(varchar, @RepackTaskId) + N']的已翻包数量已经超过了对应发运单['+ convert(varchar, ShipPlanId) + N']的拣货数。' 
						from #tempShipPlan_016 where PickedQty < LockQty + ThisLockQty

						if not exists(select top 1 1 from #tempMsg_016)
						begin
							select @LocTransCount = COUNT(1) from #tempRepackOutHu_016

							exec USP_SYS_BatchGetNextId 'INV_LocationLotDet', @LocTransCount, @EndLocLotDetId output
							select @StartLocLotDetId = @EndLocLotDetId - @LocTransCount + 1
							update ri set LocLotDetId = id.LocLotDetId
							from #tempRepackOutHu_016 as ri 
							inner join (select HuId, ROW_NUMBER() over (order by HuId) + @StartLocLotDetId as LocLotDetId 
										from #tempRepackOutHu_016) as id on ri.HuId = id.HuId

							select @LocTransCount = @LocTransCount + COUNT(1) from #tempRepackOutHu_016
							exec USP_SYS_BatchGetNextId 'INV_LocTrans', @LocTransCount, @EndTransId output
							select @StartOutTransId = @EndTransId - COUNT(1) from #tempRepackOutHu_016
							select @StartInTransId = @StartOutTransId - COUNT(1) from #tempRepackInHu_016
						end
					end
				end
			end
		end try
		begin catch
			set @ErrorMsg = N'数据准备出现异常：' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		if not exists(select top 1 1 from #tempMsg_016)
		begin
			begin try
				declare @Trancount int
				set @Trancount = @@trancount

				if @Trancount = 0
				begin
					begin tran
				end

				declare @UpdateCount int

				update WMS_RepackTask set RepackQty = RepackQty + @InQty, IsActive = CASE WHEN RepackQty + @InQty >= Qty THEN 1 ELSE 0 END,
								LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = [Version] + 1
				where UUID = @UUID and [Version] = @Version

				if (@@ROWCOUNT <> 1)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				select @UpdateCount = COUNT(1) from  #tempRepackOccupy_016 where ThisReleaseQty > 0
				update ro set ReleaseQty = ro.ReleaseQty + tmp.ThisReleaseQty, [Version] = ro.[Version] + 1
				from WMS_RepackOccupy as ro inner join #tempRepackOccupy_016 as tmp on ro.Id = tmp.Id and ro.[Version] = tmp.[Version]
				where tmp.ThisReleaseQty > 0

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				select @UpdateCount = COUNT(1) from  #tempShipPlan_016 where ThisLockQty > 0
				update sp set LockQty = sp.LockQty + tmp.ThisLockQty, 
				LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = sp.[Version] + 1
				from WMS_ShipPlan as sp inner join #tempShipPlan_016 as tmp on sp.Id = tmp.ShipPlanId and sp.[Version] = tmp.[Version]
				where tmp.ThisLockQty > 0

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				select @UpdateCount = COUNT(1) from  #tempBuffInv_016
				update bi set Qty = 0, LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = bi.[Version] + 1
				from WMS_BuffInv as bi inner join #tempBuffInv_016 as tmp on bi.UUID = tmp.UUID and bi.[Version] = tmp.[Version]

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end

				insert into WMS_BuffInv(UUID, Loc, IOType, Item, Uom, UC, Qty, LotNo, HuId, IsLock, IsPack, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])
				select NEWID(), @Location, 1, Item, Uom, UC, Qty, LotNo, HuId, 1, 0, @CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 1 
				from #tempRepackOutHu_016

				declare @UpdateInvStatement nvarchar(max)
				declare @UpdateParameter nvarchar(max)

				set @UpdateInvStatement = 'declare @UpdateCount int '
				set @UpdateInvStatement = @UpdateInvStatement + 'select @UpdateCount = COUNT(1) from #tempLocationLotDet_016 '
				set @UpdateInvStatement = @UpdateInvStatement + 'update lld set Qty = 0, LastModifyUser = @CreateUserId_1, LastModifyUserNm = @CreateUserNm_1, LastModifyDate = @DateTimeNow_1, [Version] = lld.[Version] + 1 '
				set @UpdateInvStatement = @UpdateInvStatement + 'from INV_LocationLotDet_' + @LocSuffix + ' as lld '
				set @UpdateInvStatement = @UpdateInvStatement + 'inner join #tempLocationLotDet_016 as inv on lld.Id = inv.Id and lld.[Version] = inv.[Version] '
				set @UpdateInvStatement = @UpdateInvStatement + 'if (@@ROWCOUNT <> @UpdateCount) '
				set @UpdateInvStatement = @UpdateInvStatement + 'begin '
				set @UpdateInvStatement = @UpdateInvStatement + 'RAISERROR(N''数据已经被更新，请重试。'', 16, 1) '
				set @UpdateInvStatement = @UpdateInvStatement + 'end '
				set @UpdateInvStatement = @UpdateInvStatement + 'insert into INV_LocationLotDet_' + @LocSuffix + '(Id, Location, Item, HuId, LotNo, Qty, IsCS, PlanBill, QualityType, IsFreeze, IsATP, OccupyType, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version]) '
				set @UpdateInvStatement = @UpdateInvStatement + 'select LocLotDetId, @Location_1, Item, HuId, LotNo, Qty, @IsCS_1, @PlanBill_1, 0, 0, 1, 1, @CreateUserId_1, @CreateUserNm_1, @DateTimeNow_1, @CreateUserId_1, @CreateUserNm_1, @DateTimeNow_1, 1 from #tempRepackOutHu_016 '
				set @UpdateInvStatement = @UpdateInvStatement + 'insert into INV_LocTrans_' + @LocSuffix + '(Id, OrderNo, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, EffDate, CreateUser, CreateDate) '
				set @UpdateInvStatement = @UpdateInvStatement + 'select ROW_NUMBER() over(order by HuId) + @StartInTransId_1, CONVERT(varchar, @RepackTaskId_1), @Item_1, @Uom_1, @BaseUom_1, -Qty, @IsCS_1, @PlanBill_1, 0, 0, @UnitQty_1, 0, HuId, LotNo, 351, 1, @Region_1, @Region_1, @Location_1, @Location_1, @EffDate_1, @CreateUserId_1, @DateTimeNow_1 '
				set @UpdateInvStatement = @UpdateInvStatement + 'from #tempRepackInHu_016 '
				set @UpdateInvStatement = @UpdateInvStatement + 'union all '
				set @UpdateInvStatement = @UpdateInvStatement + 'select ROW_NUMBER() over(order by HuId) + @StartOutTransId_1, CONVERT(varchar, @RepackTaskId_1), @Item_1, @Uom_1, @BaseUom_1, Qty, @IsCS_1, @PlanBill_1, 0, 0, @UnitQty_1, 0, HuId, LotNo, 352, 0, @Region_1, @Region_1, @Location_1, @Location_1, @EffDate_1, @CreateUserId_1, @DateTimeNow_1 '
				set @UpdateInvStatement = @UpdateInvStatement + 'from #tempRepackOutHu_016'
				set @UpdateParameter = N'@Location_1 varchar(50), @IsCS_1 bit, @PlanBill_1 int, @CreateUserId_1 int, @CreateUserNm_1 varchar(100), @DateTimeNow_1 datetime, @StartOutTransId_1 int, @StartInTransId_1 int, @RepackTaskId_1 int, @Item_1 varchar(50), @Uom_1 varchar(5), @BaseUom_1 varchar(5), @UnitQty_1 decimal(18, 8), @Region_1 varchar(50), @EffDate_1 datetime'

				exec sp_executesql @UpdateInvStatement, @UpdateParameter, @Location_1=@Location, @IsCS_1=@IsCS, @PlanBill_1=@PlanBill, @CreateUserId_1=@CreateUserId, @CreateUserNm_1=@CreateUserNm, @DateTimeNow_1=@DateTimeNow, @StartOutTransId_1=@StartOutTransId, @StartInTransId_1=@StartInTransId, @RepackTaskId_1=@RepackTaskId, @Item_1=@Item, @Uom_1=@Uom, @BaseUom_1=@BaseUom, @UnitQty_1=@UnitQty, @Region_1=@Region, @EffDate_1=@EffDate

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
		set @ErrorMsg = N'处理翻包结果发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	select Lvl, Msg from #tempMsg_016

	drop table #tempMsg_016
	drop table #tempRepackInHu_016
	drop table #tempRepackOutHu_016
	drop table #tempLocationLotDet_016
	drop table #tempBuffInv_016
	drop table #tempRepackOccupy_016
	drop table #tempShipPlan_016
END
GO
