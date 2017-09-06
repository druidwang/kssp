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
		LockQty decimal(18, 8),		--�ɷ���������
		PickQty decimal(18, 8),		--�����ļ����
		PickedQty decimal(18, 8),	--�Ѽ����
		ThisLockQty decimal(18, 8),  --���ζ���Ŀɷ�������
		ThisPickQty decimal(18, 8),  --����Ҫ�����ļ����
		ThisPickFullQty decimal(18, 8),  --����Ҫ�����ļ����������װ��
		ThisPickOddQty decimal(18, 8),  --����Ҫ�����ļ��������ͷ����
		ThisPickedQty decimal(18, 8),  --����ռ�û����������������μ������
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
		IsOdd bit   --�Ƿ���ͷ��
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
			set @ErrorMsg = 'û�ж��巵�����ݵĲ�����'
			RAISERROR(@ErrorMsg, 16, 1) 

			--���벻��ִ�е�����
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
			--��ȡ�����λ�����
			insert into #tempPickTarget_006(Loc, Item) 
			select distinct sp.LocFrom, sp.Item from @CreatePickTaskTable as t 
			inner join WMS_ShipPlan as sp on t.Id = sp.Id

			--��ȡ�����ƻ�
			insert into #tempShipPlan_006(OrderNo, OrderSeq, ShipPlanId, TargetDock, [Priority], StartTime, LocFrom, Item, UOM, UC, UnitQty, 
			LockQty, PickQty, PickedQty, ThisLockQty, ThisPickQty, ThisPickFullQty, ThisPickOddQty, ThisPickedQty, [Version])
			select sp.OrderNo, sp.OrderSeq, sp.Id, sp.Dock, sp.[Priority], sp.StartTime, sp.LocFrom, sp.Item, sp.Uom, sp.UC, sp.UnitQty, 
			sp.LockQty, sp.PickQty, sp.PickedQty, 0, t.PickQty, ROUND(t.PickQty / sp.UC, 0, 1) * sp.UC, t.PickQty % sp.UC, 0, sp.[Version] 
			from @CreatePickTaskTable as t
			inner join WMS_ShipPlan as sp on t.Id = sp.Id
			order by sp.LocFrom, sp.Item, sp.[Priority], sp.StartTime, sp.Id

			--�ۼ����������
			--�����ɺ������PickedQty
			--Ҫ�Ȼ������Ŀ���ShipPlan��UOM��UC��ȫƥ�����ͨ�����������ֹ�����ShipPlan���Ż�����shipPlan��LockQty������������
			--��ȡ���õ�Buff����浥λ������װ��,ȥ��ֱ�ӱ����˼ƻ�ռ�õĿ��
			--���ܹ����Ѿ��ƶ������ڵ�����
			insert into #tempBuffInv_006(Loc, Item, Uom, UC, Qty, IsOdd)
			select bi.Loc, bi.Item, hu.Uom, hu.UC, SUM(bi.Qty), 0
			from #tempPickTarget_006 as pt
			inner join WMS_BuffInv as bi on pt.Item = bi.Item and pt.Loc = bi.Loc
			inner join INV_Hu as hu on bi.HuId = hu.HuId
			left join WMS_BuffOccupy as bo on bi.Id = bo.BuffInvId
			where bi.Qty > 0 and bi.IOType = 1  --Ŀǰֻ���Ƿ������������������ջ���������Խ�������
			and hu.UC = hu.Qty  --����װ
			and bo.BuffInvId is null
			group by bi.Loc, bi.Item, hu.Uom, hu.UC

			--��ȡ���õ�Buff����浥λ����ͷ��װ��,ȥ��ֱ�ӱ����˼ƻ�ռ�õĿ��
			--���ܹ����Ѿ��ƶ������ڵ�����
			insert into #tempBuffInv_006(Loc, Item, Uom, UC, Qty, IsOdd)
			select bi.Loc, bi.Item, hu.Uom, hu.UC, SUM(bi.Qty), 1
			from #tempPickTarget_006 as pt
			inner join WMS_BuffInv as bi on pt.Item = bi.Item and pt.Loc = bi.Loc
			inner join INV_Hu as hu on bi.HuId = hu.HuId
			left join WMS_BuffOccupy as bo on bi.Id = bo.BuffInvId
			where bi.Qty > 0 and bi.IOType = 1  --Ŀǰֻ���Ƿ������������������ջ���������Խ�������
			and hu.UC <> hu.Qty  --��ͷ��װ
			and bo.BuffInvId is null  --ȥ��ֱ�ӱ����˼ƻ�ռ�õĿ��
			group by bi.Loc, bi.Item, hu.Uom, hu.UC

			--��ȡ�����ܷ��˼ƻ��Ŀɷ������Ϳ��õ��Ѽ����������ʱȥ��ֱ�ӱ����˼ƻ�ռ�õĿ��
			--����Ϊ��浥λ
			insert into #tempCreatedShipPlan_006(Loc, Item, Uom, UC, RemainLockQty, RemainRepackQty)
			select sp.LocFrom, sp.Item, sp.Uom, sp.UC,
			SUM((sp.LockQty - sp.ShipQty) * sp.UnitQty - ISNULL(occ.Qty, 0)) as RemainLockQty,  --�ɷ�������ȥ����ֱ��ռ�õĿ��Ĳ��֣�
			SUM((sp.PickedQty - sp.LockQty) * sp.UnitQty) as RemainRepackQty  --��Ҫ����������
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

			--�ۼ��������Ŀ�棨����װ����װƥ�䣩
			update bi set Qty = bi.Qty - ISNULL(sp.RemainLockQty, 0)
			from #tempBuffInv_006 as bi 
			left join #tempCreatedShipPlan_006 as sp on bi.Loc = sp.Loc and bi.Item = sp.Item and bi.Uom = sp.Uom and bi.UC = sp.UC
			where bi.IsOdd = 0

			--�ۼ��������Ŀ�棨��Ҫ����ġ���װ��ƥ�����ͷ�䣩
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
			
				--������ͬ��װ����ͷ��
				update bi set Qty = Qty - @LastQty,
				@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= Qty THEN Qty ELSE @Qty END
				from #tempBuffInv_006 as bi
				where bi.Item = @Item and bi.Loc = @Loc  and bi.Uom = @Uom and bi.UC = @UC
				and bi.IsOdd = 1
				set @Qty = @Qty - @LastQty

				--��οۼ���װ����ͬ����ͷ��
				if (@Qty > 0)
				begin
					update bi set Qty = Qty - @LastQty,
					@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= Qty THEN Qty ELSE @Qty END
					from #tempBuffInv_006 as bi
					where bi.Item = @Item and bi.Loc = @Loc  and bi.Uom = @Uom and bi.UC <> @UC
					and bi.IsOdd = 1
					set @Qty = @Qty - @LastQty
				end

				--���ۼ���װ����ͬ������
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

			--����ͷ�ķ��˼ƻ��ڵ�����ţ�����Ҫ������ͷ��ռ�ÿ��
			update sp set OddRowId = ROW_NUMBER() over(order by sp.LocFrom, sp.Item, sp.[Priority], sp.StartTime, sp.Id)
			from #tempShipPlan_006 as sp inner join #tempBuffInv_006 as bi on sp.LocFrom = bi.Loc and sp.Item = bi.Item and sp.UOM = bi.UOM and sp.UC = bi.UC
			where (bi.Qty > = (sp.ThisPickOddQty * sp.UnitQty)) and bi.IsOdd = 1

			--�������ͷ������ռ�û�����
			declare @OrderNo varchar(50)
			declare @OrderSeq int
			declare @ShipPlanId int
			declare @TargetDock varchar(50)
			declare @OddQty decimal(18, 8)  --��浥λ
			select @RowId = MIN(OddRowId), @MaxRowId = MAX(OddRowId) from #tempShipPlan_006
			while (@RowId <= @MaxRowId)
			begin 
				select @Loc = LocFrom, @Item = Item, @Uom = Uom, @UC = UC, @OddQty = ThisPickOddQty * UnitQty,
				@OrderNo = OrderNo, @OrderSeq = OrderSeq, @ShipPlanId = ShipPlanId, @TargetDock = TargetDock
				from #tempShipPlan_006 as sp where OddRowId = @RowId

				--���˼ƻ�ֱ��ռ����ͷ
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
			
			--���˼ƻ�ռ�û�������棬���շ������ȼ�������ʱ��˳��ռ��
			declare @IsOdd bit
			select @RowId = MIN(RowId), @MaxRowId = MAX(RowId) from #tempBuffInv_006
			while (@RowId <= @MaxRowId)
			begin  --ѭ��ռ�û��������
				set @Qty = null
				set @LastQty = 0

				select @Loc = Loc, @Item = Item, @Uom = Uom, @UC = UC, @IsOdd = IsOdd, @Qty = Qty, @LastQty = 0 
				from #tempBuffInv_006 where RowId = @RowId

				if (@Qty > 0)
				begin
					--����Ѿ�������װ����ͷ������
					if @IsOdd = 0
					begin  --����ƥ�䣬ֱ�����������Ŀ������
						update sp set ThisLockQty = @LastQty,
						@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= (sp.ThisPickFullQty * sp.UnitQty) THEN sp.ThisPickFullQty * sp.UnitQty ELSE @Qty END
						from #tempShipPlan_006 as sp
						where sp.Item = @Item and sp.LocFrom = @Loc and sp.Uom = @Uom and sp.UC = @UC
					end
					else
					begin  --��ͷ��ƥ�䣬ֱ�������Ѽ���Ŀ������
						update sp set ThisPickedQty = @LastQty,
						@Qty = @Qty - @LastQty, @LastQty = CASE WHEN @Qty >= ((sp.ThisPickQty - sp.ThisLockQty) * sp.UnitQty) THEN (sp.ThisPickQty - sp.ThisLockQty) * sp.UnitQty ELSE @Qty END
						from #tempShipPlan_006 as sp
						where sp.Item = @Item and sp.LocFrom = @Loc and sp.Uom = @Uom
					end
				end
				
				set @RowId = @RowId + 1
			end

			--�Ѽ���� = �Ѽ���� + �����Ŀ������
			update #tempShipPlan_006 set ThisPickedQty = ThisPickedQty + ThisLockQty
		end try
		begin catch
			set @ErrorMsg = N'����׼�������쳣��' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch

		begin try
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
			from #tempBuffOccupy_006

			--��ȡ��Ҫ���µ�����
			declare @UpdateRowCount int
			select @UpdateRowCount = count(1) from #tempShipPlan_006 where ThisPickedQty > 0
			
			--���´�����������������Ѿ����������
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

			set @ErrorMsg = N'���ݸ��³����쳣��' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch
	end try
	begin catch
		set @ErrorMsg = N'ռ�û�������淢���쳣��' + Error_Message() 
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