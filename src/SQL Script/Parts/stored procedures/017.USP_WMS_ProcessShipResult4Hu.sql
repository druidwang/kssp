SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_ProcessShipResult4Hu')
BEGIN
	DROP PROCEDURE USP_WMS_ProcessShipResult4Hu
END
GO

CREATE PROCEDURE dbo.USP_WMS_ProcessShipResult4Hu
	@ShipResultTable ShipResultTableType readonly,
	@CreateUserId int,
	@CreateUserNm varchar(100)
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)
	
	set @DateTimeNow = GetDate()

	create table #tempMsg_017 
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempBuffInv_017
	(
		UUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS,
		LotNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		Qty  decimal(18, 8),
		Location varchar(50) COLLATE  Chinese_PRC_CI_AS,
		IsLock bit,
		ShipPlanId int,
		[Version] int
	)

	create table #tempShipPlan_017
	(
		ShipPlanId int Primary Key,
		OrderNo varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderDetId int,
		OrderType tinyint,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UnitQty decimal(18, 8),
		UC decimal(18, 8),
		OrderQty decimal(18, 8),
		PackQty decimal(18, 8),
		ShipQty decimal(18, 8),
		ThisShipQty decimal(18, 8),
		IsActive bit,
		[Version] int,
	)

	begin try
		begin try
			if not exists(select top 1 1 from @ShipResultTable)
			begin
				insert into #tempMsg_017(Lvl, Msg) values(2, N'发运条码不能为空。')
			end

			if not exists(select top 1 1 from #tempMsg_017)
			begin
				insert into #tempBuffInv_017(UUID, HuId, LotNo, Item, Uom, UnitQty, UC, Qty, Location, IsLock, ShipPlanId, [Version])
				select bi.UUID, sr.HuId, hu.LotNo, hu.Item, hu.Uom, hu.UnitQty, hu.UC, hu.Qty, bi.Loc, bi.IsLock, bo.ShipPlanId, bi.[Version] 
				from @ShipResultTable as sr
				left join INV_Hu as hu on sr.HuId = hu.HuId
				left join WMS_BuffInv as bi on sr.HuId = bi.HuId and (bi.Qty > 0 and bi.IOType = 1) 
				left join WMS_BuffOccupy as bo on bi.UUID = bo.UUID

				insert into #tempMsg_017(Lvl, Msg)
				select 2, N'发运条码['+ HuId + N']大于1条。' 
				from @ShipResultTable group by HuId having COUNT(1) > 1

				insert into #tempMsg_017(Lvl, Msg)
				select 2, N'发运条码['+ HuId + N']不存在。' from #tempBuffInv_017 where Item is null
				
				insert into #tempMsg_017(Lvl, Msg)
				select distinct 2, N'条码['+ HuId + N']不在发货缓冲区。'
				from #tempBuffInv_017 where Item is not null and Location is null

				insert into #tempMsg_017(Lvl, Msg)
				select distinct 2, N'条码['+ HuId + N']没有和发运计划关联。'
				from #tempBuffInv_017 where ShipPlanId is null

				insert into #tempMsg_017(Lvl, Msg)
				select distinct 2, N'条码['+ HuId + N']和多个发运计划关联。'
				from #tempBuffInv_017 where ShipPlanId is not null
				group by HuId having count(1) > 1
					
				if not exists(select top 1 1 from #tempMsg_017)
				begin
					insert into #tempShipPlan_017(ShipPlanId, OrderNo, OrderDetId, OrderType, Item, Uom, UnitQty, UC, OrderQty, PackQty, ShipQty, ThisShipQty, IsActive, [Version])
					select distinct bi.ShipPlanId, sp.OrderNo, sp.OrderDetId, sp.OrderType, sp.Item, sp.Uom, sp.UnitQty, sp.UC, sp.OrderQty, sp.PackQty, sp.ShipQty, 0, sp.IsActive, sp.[Version]  
					from WMS_ShipPlan as sp 
					right join #tempBuffInv_017 as bi on sp.Id = bi.ShipPlanId
						
					update sp set ThisShipQty = bi.Qty
					from #tempShipPlan_017 as sp 
					inner join (select ShipPlanId, SUM(Qty) as Qty from #tempBuffInv_017 group by ShipPlanId) as bi on sp.ShipPlanId = bi.ShipPlanId
						
					insert into #tempMsg_017(Lvl, Msg)
					select 2, N'发运计划['+ convert(varchar, ShipPlanId) + N']不存在。'
					from #tempShipPlan_017 where OrderNo is null

					insert into #tempMsg_017(Lvl, Msg)
					select 2, N'发运计划['+ convert(varchar, ShipPlanId) + N']已经关闭。'
					from #tempShipPlan_017 where IsActive = 0

					insert into #tempMsg_017(Lvl, Msg)
					select 2, N'发运计划['+ convert(varchar, ShipPlanId) + N']的发货数超过了装箱的库存数。'
					from #tempShipPlan_017 where PackQty < ShipQty + ThisShipQty

					update #tempShipPlan_017 set IsActive = 0 where ShipQty + ThisShipQty >= OrderQty
				end
			end
		end try
		begin catch
			set @ErrorMsg = N'数据准备出现异常：' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		if not exists(select top 1 1 from #tempMsg_017)
		begin
			begin try
				declare @Trancount int
				set @Trancount = @@trancount

				if @Trancount = 0
				begin
					begin tran
				end

				declare @UpdateCount int

				select @UpdateCount = COUNT(1) from #tempBuffInv_017
				update bi set Qty  = 0, LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = bi.[Version] + 1
				from WMS_BuffInv as bi inner join #tempBuffInv_017 as tmp on bi.UUID = tmp.UUID and bi.[Version] = tmp.[Version]

				if (@@ROWCOUNT <> @UpdateCount)
				begin
					RAISERROR(N'数据已经被更新，请重试。', 16, 1)
				end
				
				select @UpdateCount = COUNT(1) from #tempShipPlan_017
				update sp set ShipQty  = sp.ShipQty + tmp.ThisShipQty, LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = sp.[Version] + 1,
				IsActive = tmp.IsActive, CloseUser = CASE WHEN tmp.IsActive = 0 THEN @CreateUserId ELSE NULL END, CloseUserNm = CASE WHEN tmp.IsActive = 0 THEN @CreateUserNm ELSE NULL END,
				CloseDate = CASE WHEN tmp.IsActive = 0 THEN @DateTimeNow ELSE NULL END
				from WMS_ShipPlan as sp inner join #tempShipPlan_017 as tmp on sp.Id = tmp.ShipPlanId and sp.[Version] = tmp.[Version]

				if (@@ROWCOUNT <> @UpdateCount)
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
		end
	end try
	begin catch
		set @ErrorMsg = N'处理发运结果发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	select Lvl, Msg from #tempMsg_017
	select bi.HuId, bi.LotNo, bi.Qty, sp.OrderDetId, mstr.Flow
	from #tempBuffInv_017 as bi 
	inner join #tempShipPlan_017 as sp on bi.ShipPlanId = sp.ShipPlanId
	inner join ORD_OrderDet_2 as det on sp.OrderDetId = det.Id
	inner join ORD_OrderMstr_2 as mstr on det.OrderNo = mstr.OrderNo
	where sp.OrderType = 2
	union all
	select bi.HuId, bi.LotNo, bi.Qty, sp.OrderDetId, mstr.Flow
	from #tempBuffInv_017 as bi
	inner join #tempShipPlan_017 as sp on bi.ShipPlanId = sp.ShipPlanId
	inner join ORD_OrderDet_3 as det on sp.OrderDetId = det.Id
	inner join ORD_OrderMstr_3 as mstr on det.OrderNo = mstr.OrderNo
	where sp.OrderType = 3
	
	drop table #tempMsg_017
	drop table #tempBuffInv_017
	drop table #tempShipPlan_017
END
GO