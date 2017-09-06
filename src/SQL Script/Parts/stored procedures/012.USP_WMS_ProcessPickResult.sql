SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_ProcessPickResult')
BEGIN
	DROP PROCEDURE USP_WMS_ProcessPickResult
END
GO

CREATE PROCEDURE dbo.USP_WMS_ProcessPickResult
	@PickResultTable PickResultTableType readonly,
	@CreateUserId int,
	@CreateUserNm varchar(100)
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)
	
	set @DateTimeNow = GetDate()

	create table #tempPickTask_012
	(
		PickTaskId int Primary Key,
		PickTaskUUID varchar(50) COLLATE  Chinese_PRC_CI_AS,
		OrderQty decimal(18, 8),
		PickQty decimal(18, 8),
		ThisPickQty decimal(18, 8),
		IsPickHu tinyint,
		PickBy tinyint,
		IsActive bit
	)

	create table #tempMsg_012 
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempMsg_013 
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempMsg_014
	(
		Id int identity(1, 1) primary key,
		Lvl tinyint,
		Msg varchar(2000) COLLATE  Chinese_PRC_CI_AS
	)

	begin try
		if not exists(select top 1 1 from @PickResultTable)
		begin
			insert into #tempMsg_012(Lvl, Msg) values(2, N'拣货结果不能为空。')
		end
		else
		begin
			insert into #tempPickTask_012(PickTaskId, PickTaskUUID, OrderQty, PickQty, ThisPickQty, IsPickHu, PickBy, IsActive)
			select distinct sp.Id, sp.UUID, sp.OrderQty, sp.PickQty, 0, sp.IsPickHu, sp.PickBy, sp.IsActive
			from @PickResultTable as tmp 
			inner join WMS_PickTask as sp on tmp.PickTaskId = sp.Id

			update pt set ThisPickQty = tmp.PickQty
			from #tempPickTask_012 as pt
			inner join (select PickTaskId, SUM(Qty) as PickQty 
						from @PickResultTable where HuId is null 
						group by PickTaskId) as tmp on pt.PickTaskId = tmp.PickTaskId

			update pt set ThisPickQty = tmp.PickQty
			from #tempPickTask_012 as pt
			inner join (select PickTaskId, SUM(hu.Qty) as PickQty 
						from @PickResultTable as pr inner join INV_Hu as hu on pr.HuId = hu.HuId 
						group by PickTaskId) as tmp on pt.PickTaskId = tmp.PickTaskId

			insert into #tempMsg_012(Lvl, Msg)
			select 2, N'拣货任务['+ convert(varchar, PickTaskId) + N']已经关闭。'
			from #tempPickTask_012 where IsActive = 0

			insert into #tempMsg_012(Lvl, Msg)
			select 2, N'拣货任务['+ convert(varchar, PickTaskId) + N']的已拣数已经超过了待拣数。'
			from #tempPickTask_012 where OrderQty < PickQty + ThisPickQty and IsActive = 1

			if not exists(select top 1 1 from #tempMsg_012)
			begin
				declare @PickResult4PickQtyTable PickResultTableType
				declare @PickResult4PickLotNoTable PickResultTableType
				declare @PickResult4PickHuTable PickResultTableType

				insert into @PickResult4PickQtyTable(PickTaskId, Qty) 
				select PickTaskId, ThisPickQty from #tempPickTask_012 where IsPickHu = 0
				
				insert into @PickResult4PickLotNoTable(PickTaskId, HuId, Qty) 
				select tmp.PickTaskId, hu.HuId, hu.Qty
				from #tempPickTask_012 as pt 
				inner join @PickResultTable as tmp on pt.PickTaskId = tmp.PickTaskId 
				inner join INV_Hu as hu on tmp.HuId = hu.HuId 
				where IsPickHu = 1 and PickBy = 0
				
				insert into @PickResult4PickHuTable(PickTaskId, HuId, Qty)
				select tmp.PickTaskId, hu.HuId, hu.Qty 
				from #tempPickTask_012 as pt 
				inner join @PickResultTable as tmp on pt.PickTaskId = tmp.PickTaskId 
				inner join INV_Hu as hu on tmp.HuId = hu.HuId 
				where IsPickHu = 1 and PickBy = 1

				if exists(select top 1 1 from @PickResult4PickQtyTable)
				begin
					exec USP_WMS_ProcessPickResult4PickQty @PickResult4PickQtyTable, @CreateUserId, @CreateUserNm 
				end

				if exists(select top 1 1 from @PickResult4PickLotNoTable)
				begin
					exec USP_WMS_ProcessPickResult4PickLotNoAndHu @PickResult4PickLotNoTable, 0, @CreateUserId,@CreateUserNm 
				end

				if exists(select top 1 1 from @PickResult4PickHuTable)
				begin
					exec USP_WMS_ProcessPickResult4PickLotNoAndHu @PickResult4PickHuTable, 1, @CreateUserId,@CreateUserNm 
				end
			end
		end
	end try
	begin catch
		set @ErrorMsg = N'处理拣货结果发生异常：' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	select Lvl, Msg from #tempMsg_012
	union all
	select Lvl, Msg from #tempMsg_013
	union all
	select Lvl, Msg from #tempMsg_014

	drop table #tempMsg_012
	drop table #tempMsg_013
	drop table #tempMsg_014
END
GO