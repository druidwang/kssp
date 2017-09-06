SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_WMS_OcuppyBuffInv4PickHu')
BEGIN
	DROP PROCEDURE USP_WMS_OcuppyBuffInv4PickHu
END
GO

CREATE PROCEDURE dbo.USP_WMS_OcuppyBuffInv4PickHu
	@CreatePickTaskTable CreatePickTaskTableType readonly,
	@CreateUserId int,
	@CreateUserNm varchar(100)
AS
BEGIN
	set nocount on
	declare @DateTimeNow datetime
	declare @ErrorMsg nvarchar(MAX)

	set @DateTimeNow = GetDate()

	create table #tempPickTarget_007
	(
		Id int identity(1, 1),
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	create table #tempShipPlan_007
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
		PickQty decimal(18, 8),		--�����ļ����
		PickedQty decimal(18, 8),	--�Ѽ����
		ThisPickQty decimal(18, 8),  --����Ҫ�����ļ����
		ThisPickFullQty decimal(18, 8),  --����Ҫ�����ļ����������װ��
		ThisPickOddQty decimal(18, 8),  --����Ҫ�����ļ��������ͷ����
		ThisPickedQty decimal(18, 8),  --����ռ�û����������������μ������
		[Version] int
	)

	create table #tempBuffInv_007
	(
		RowId int identity(1, 1) primary key,
		Id int,
		Loc varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Item varchar(50) COLLATE  Chinese_PRC_CI_AS,
		HuId varchar(50) COLLATE  Chinese_PRC_CI_AS,
		Uom varchar(5) COLLATE  Chinese_PRC_CI_AS,
		UC decimal(18, 8),
		Qty decimal(18, 8),  --���뵥λ
		UnitQty decimal(18, 8)
	)

	create table #tempBuffOccupy_007
	(
		BuffInvId int, 
		OrderNo varchar(50) COLLATE  Chinese_PRC_CI_AS, 
		OrderSeq int, 
		ShipPlanId int, 
		TargetDock varchar(50) COLLATE  Chinese_PRC_CI_AS
	)

	begin try
		if not exists(select top 1 1 FROM tempdb.sys.objects WHERE type = 'U' AND name like '#tempOccupyBuffInv_007%') 
		begin
			set @ErrorMsg = 'û�ж��巵�����ݵĲ�����'
			RAISERROR(@ErrorMsg, 16, 1) 

			--���벻��ִ�е�����
			create table #tempOccupyBuffInv_007
			(
				Id int primary key,
				PickedQty decimal(18, 8),
			)
		end
		else
		begin
			truncate table #tempOccupyBuffInv_007
		end

		begin try
			--��ȡ�����λ�����
			insert into #tempPickTarget_007(Loc, Item) 
			select distinct sp.LocFrom, sp.Item from @CreatePickTaskTable as t 
			inner join WMS_ShipPlan as sp on t.Id = sp.Id

			--��ȡ�����ƻ�
			insert into #tempShipPlan_007(OrderNo, OrderSeq, ShipPlanId, TargetDock, [Priority], StartTime, LocFrom, Item, UOM, UC, UnitQty, 
			PickQty, PickedQty, ThisPickQty, ThisPickFullQty, ThisPickOddQty, ThisPickedQty, [Version])
			select sp.OrderNo, sp.OrderSeq, sp.Id, sp.Dock, sp.[Priority], sp.StartTime, sp.LocFrom, sp.Item, sp.Uom, sp.UC, sp.UnitQty, 
			sp.PickQty, sp.PickedQty, t.PickQty, ROUND(t.PickQty / sp.UC, 0, 1) * sp.UC, t.PickQty % sp.UC, 0, sp.[Version] 
			from @CreatePickTaskTable as t
			inner join WMS_ShipPlan as sp on t.Id = sp.Id
			order by sp.LocFrom, sp.Item, sp.[Priority], sp.StartTime, sp.Id

			--��ȡ������������
			insert into #tempBuffInv_007(Id, Loc, Item, HuId, Uom, UC, Qty, UnitQty)
			select bi.Id, bi.Loc, bi.Item, hu.HuId, hu.Uom, hu.UC, hu.Qty, hu.UnitQty
			from #tempPickTarget_007 as pt
			inner join WMS_BuffInv as bi on pt.Item = bi.Item and pt.Loc = bi.Loc
			inner join INV_Hu as hu on bi.HuId = hu.HuId
			left join WMS_BuffOccupy as bo on bi.Id = bo.BuffInvId
			where bi.Qty > 0 and bi.IOType = 1  --Ŀǰֻ���Ƿ������������������ջ���������Խ�������
			and bo.BuffInvId is null
			
			--���˼ƻ�ռ�û�������棬���շ������ȼ�������ʱ��˳��ռ��
			declare @RowId int
			declare @MaxRowId int
			declare @Loc varchar(50)
			declare @Item varchar(50)
			declare @Uom varchar(5)
			declare @UC decimal(18, 8)
			declare @BuffInvId int
			declare @Qty decimal(18, 8)
			declare @UnitQty decimal(18, 8)
			declare @ShipPlanRowId int
			select @RowId = MIN(RowId), @MaxRowId = MAX(RowId) from #tempBuffInv_007
			while (@RowId <= @MaxRowId)
			begin  --ѭ��ռ�û��������
				select @BuffInvId = Id, @Loc = Loc, @Item = Item, @Uom = Uom, @UC = UC, @Qty = Qty, @UnitQty = UnitQty
				from #tempBuffInv_007 where RowId = @RowId
				set @ShipPlanRowId = null

				if (@UC = @Qty)
				begin --����ƥ��
					insert into #tempBuffOccupy_007(BuffInvId, OrderNo, OrderSeq, ShipPlanId, TargetDock)
					select top 1 @BuffInvId, sp.OrderNo, sp.OrderSeq, sp.ShipPlanId, sp.TargetDock
					from #tempShipPlan_007 as sp 
					where sp.Item = @Item and sp.LocFrom = @Loc and Uom = @Uom and UC = @UC 
					and sp.ThisPickQty >= sp.ThisPickedQty + @Qty
					order by sp.RowId
				end
				else --��ͷ��ƥ��
				begin
					insert into #tempBuffOccupy_007(BuffInvId, OrderNo, OrderSeq, ShipPlanId, TargetDock)
					select top 1 @BuffInvId, sp.OrderNo, sp.OrderSeq, sp.ShipPlanId, sp.TargetDock
					from #tempShipPlan_007 as sp 
					where sp.Item = @Item and sp.LocFrom = @Loc and Uom = @Uom and UC = @UC 
					and sp.ThisPickOddQty = @Qty
					order by sp.RowId
				end

				set @ShipPlanRowId = @@Identity

				if (@@Identity is not null)
				begin  --����
					update #tempShipPlan_007 set ThisPickedQty = ThisPickedQty + @Qty where ShipPlanId = @ShipPlanRowId
					update #tempBuffInv_007 set Qty = Qty - @Qty where Id = @BuffInvId

					if (@UC <> @Qty)
					begin
						update #tempShipPlan_007 set ThisPickOddQty = 0
					end
				end
			
				set @RowId = @RowId + 1
			end

		end try
		begin catch
			set @ErrorMsg = N'����׼�������쳣��' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		begin try
			declare @trancount int
			set @trancount = @@trancount

			if @Trancount = 0
			begin
				begin tran
			end

			--ֱ��ռ�û������Ŀ��
			insert into WMS_BuffOccupy(BuffInvId, OrderNo, OrderSeq, ShipPlanId, TargetDock, 
			CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, [Version])
			select BuffInvId, OrderNo, OrderSeq, ShipPlanId, TargetDock, 
			@CreateUserId, @CreateUserNm, @DateTimeNow, @CreateUserId, @CreateUserNm, @DateTimeNow, 1
			from #tempBuffOccupy_007

			--��ȡ��Ҫ���µ�����
			declare @UpdateRowCount int
			select @UpdateRowCount = count(1) from #tempShipPlan_007 where ThisPickedQty > 0
			
			--���´�����������������Ѿ����������
			update sp set LockQty = sp.LockQty + tmp.ThisPickedQty, PickedQty = sp.PickedQty + tmp.ThisPickedQty, PickQty = sp.PickQty + tmp.ThisPickedQty, 
			LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = sp.[Version] + 1
			from  #tempShipPlan_007 as tmp
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

			set @ErrorMsg = N'���ݸ��³����쳣��' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch
	end try
	begin catch
		set @ErrorMsg = N'ռ�û�������淢���쳣��' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	insert into #tempOccupyBuffInv_007(Id, PickedQty) select ShipPlanId, ThisPickedQty from #tempShipPlan_007 where ThisPickedQty > 0

	drop table #tempPickTarget_007
	drop table #tempShipPlan_007
	drop table #tempBuffInv_007
	drop table #tempBuffOccupy_007
END
GO