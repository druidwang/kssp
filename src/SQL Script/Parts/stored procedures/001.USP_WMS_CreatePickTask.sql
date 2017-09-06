SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_CreatePickTask')
BEGIN
	DROP PROCEDURE USP_WMS_CreatePickTask
END
GO

CREATE PROCEDURE dbo.USP_WMS_CreatePickTask
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
	
	create table #tempShipPlan_001
	(
		ShipPlanId int primary key,
		OrderQty decimal(18, 8),
		PickQty decimal(18, 8),
		ThisPickQty decimal(18, 8),
		IsShipScanHu bit,
		PickBy tinyint,
		IsActive bit,
	)

	create table #tempMsg_001 
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempMsg_002 
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempMsg_003 
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempMsg_004 
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	begin try
		if not exists(select top 1 1 from @CreatePickTaskTable)
		begin
			insert into #tempMsg_001(Lvl, Msg) values(2, N'发货任务为空，不能创建拣货单。')
		end
		else
		begin
			insert into #tempShipPlan_001(ShipPlanId, OrderQty, PickQty, ThisPickQty, IsShipScanHu, PickBy, IsActive)
			select sp.Id, sp.OrderQty, sp.PickQty, tmp.PickQty, sp.IsShipScanHu, l.PickBy, sp.IsActive
			from @CreatePickTaskTable as tmp 
			inner join WMS_ShipPlan as sp on tmp.Id = sp.Id
			inner join MD_Location as l on sp.LocFrom = l.Code

			--检查发运计划是否关闭
			insert into #tempMsg_001(Lvl, Msg)
			select 2, N'发货任务['+ convert(varchar, ShipPlanId) + N']已经关闭，不能创建拣货单。'
			from #tempShipPlan_001 where IsActive = 0

			--检查创建的拣货单数量是否超过了计划数
			insert into #tempMsg_001(Lvl, Msg)
			select 2, N'发货任务['+ convert(varchar, ShipPlanId) + N']的拣货数已经超过了订单数，不能创建拣货单。'
			from #tempShipPlan_001 where IsActive = 1
			and PickQty + ThisPickQty > OrderQty

			if not exists(select top 1 1 from #tempMsg_001)
			begin
				declare @CreatePickTask4PickQtyTable CreatePickTaskTableType
				declare @CreatePickTask4PickLotNoTable CreatePickTaskTableType
				declare @CreatePickTask4PickHuTable CreatePickTaskTableType

				insert into @CreatePickTask4PickQtyTable(Id, PickQty) select ShipPlanId, ThisPickQty from #tempShipPlan_001 where IsShipScanHu = 0
				insert into @CreatePickTask4PickLotNoTable(Id, PickQty) select ShipPlanId, ThisPickQty from #tempShipPlan_001 where IsShipScanHu = 1 and PickBy = 0
				insert into @CreatePickTask4PickHuTable(Id, PickQty) select ShipPlanId, ThisPickQty from #tempShipPlan_001 where IsShipScanHu = 1 and PickBy = 1

				if exists(select top 1 1 from @CreatePickTask4PickQtyTable)
				begin
					exec USP_WMS_CreatePickTask4PickQty @CreatePickTask4PickQtyTable, @CreateUserId,@CreateUserNm 
				end

				if exists(select top 1 1 from @CreatePickTask4PickLotNoTable)
				begin
					exec USP_WMS_CreatePickTask4PickLotNo @CreatePickTask4PickLotNoTable, @CreateUserId,@CreateUserNm 
				end

				if exists(select top 1 1 from @CreatePickTask4PickHuTable)
				begin
					exec USP_WMS_CreatePickTask4PickHu @CreatePickTask4PickHuTable, @CreateUserId,@CreateUserNm 
				end
			end
		end
	end try
	begin catch
		set @ErrorMsg = N'创建拣货任务发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	select Lvl, Msg from #tempMsg_001
	union all
	select Lvl, Msg from #tempMsg_002
	union all
	select Lvl, Msg from #tempMsg_003
	union all
	select Lvl, Msg from #tempMsg_004

	drop table #tempMsg_001
	drop table #tempMsg_002
	drop table #tempMsg_003
	drop table #tempMsg_004
END
GO