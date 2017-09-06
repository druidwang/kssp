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
		PickQty decimal(18, 8),		--�����ļ����
		PickedQty decimal(18, 8),	--�Ѽ����
		ThisPickQty decimal(18, 8),  --����Ҫ�����ļ����
		ThisPickedQty decimal(18, 8),  --����ռ�û����������������μ������
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
			set @ErrorMsg = 'û�ж��巵�����ݵĲ�����'
			RAISERROR(@ErrorMsg, 16, 1) 

			--���벻��ִ�е�����
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
			--��ȡ�����λ�����
			insert into #tempPickTarget_005(Loc, Item) 
			select distinct sp.LocFrom, sp.Item from @CreatePickTaskTable as t 
			inner join WMS_ShipPlan as sp on t.Id = sp.Id

			--��ȡ�����ƻ�
			insert into #tempShipPlan_005(ShipPlanId, [Priority], StartTime, LocFrom, Item, UOM, UC, UnitQty, 
			PickQty, PickedQty, ThisPickQty, ThisPickedQty, [Version])
			select sp.Id, sp.[Priority], sp.StartTime, sp.LocFrom, sp.Item, sp.Uom, sp.UC, sp.UnitQty, 
			sp.PickQty, sp.PickedQty, t.PickQty, 0, sp.[Version] 
			from @CreatePickTaskTable as t
			inner join WMS_ShipPlan as sp on t.Id = sp.Id
			order by sp.LocFrom, sp.Item, sp.[Priority], sp.StartTime, sp.Id

			--�ۼ����������
			--����������������ɺ�����shipPlan��PickedQty���Ѽ��������LockQty������������
			--PickedQty���Ѽ��������LockQty������������Ӧ����һ�µģ�����ֻ����PickedQty
			--��ȡ���õ�Buff����浥λ��
			--���ܹ����Ѿ��ƶ������ڵ�����
			insert into #tempBuffInv_005(Loc, Item, Qty)
			select bi.Loc, bi.Item, bi.Qty
			from #tempPickTarget_005 as pt
			inner join WMS_BuffInv as bi on pt.Item = bi.Item and pt.Loc = bi.Loc
			where bi.HuId is null and bi.Qty > 0 and bi.IOType = 1  --Ŀǰֻ���Ƿ������������������ջ���������Խ�������
			group by bi.Loc, bi.Item

			--�ۼ������˼ƻ�ռ�õ���������浥λ��
			--���õ����ۼ�ֱ���÷��˼ƻ�ռ�õ���(��WMS_PickOccupy)
			update bi set Qty = bi.Qty - sp.PickedQty
			from #tempBuffInv_005 as bi
			inner join (select sp.LocFrom, sp.Item, SUM((sp.PickedQty - sp.ShipQty) * sp.UnitQty) as PickedQty --ת��Ϊ��浥λ
						from WMS_ShipPlan as sp 
						inner join #tempPickTarget_005 as pt on sp.LocFrom = pt.Loc and sp.Item = pt.Item
						where sp.IsActive = 1 and sp.IsShipScanHu = 0 and sp.PickedQty > sp.ShipQty
						group by sp.LocFrom, sp.Item) as sp on bi.Loc = sp.LocFrom and bi.Item = sp.Item

			--���˼ƻ�ռ�û�������棬���շ������ȼ�������ʱ��˳��ռ��
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
			
			--��ȡ��Ҫ���µ�����
			declare @UpdateRowCount int
			select @UpdateRowCount = count(1) from #tempShipPlan_005 where ThisPickedQty > 0
			
			--���´�����������������Ѿ����������
			update sp set LockQty = sp.LockQty + tmp.ThisPickedQty, PickedQty = sp.PickedQty + tmp.ThisPickedQty, PickQty = sp.PickQty + tmp.ThisPickedQty, 
			LastModifyDate = @DateTimeNow, LastModifyUser = @CreateUserId, LastModifyUserNm = @CreateUserNm, [Version] = sp.[Version] + 1
			from  #tempShipPlan_005 as tmp
			inner join WMS_ShipPlan as sp on tmp.ShipPlanId = sp.Id and tmp.[Version] = sp.[Version]
			where tmp.ThisPickedQty > 0

			if (@@ROWCOUNT <> @UpdateRowCount)
			begin
				RAISERROR(N'�����Ѿ������£������ԡ�', 16, 1)
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

			set @ErrorMsg = N'���ݸ��³����쳣��' + Error_Message()
			RAISERROR(@ErrorMsg, 16, 1) 
		end catch
	end try
	begin catch
		set @ErrorMsg = N'ռ�û�������淢���쳣��' + Error_Message() 
		RAISERROR(@ErrorMsg, 16, 1) 
	end catch

	insert into #tempOccupyBuffInv_005(Id, PickedQty) select ShipPlanId, ThisPickedQty from #tempShipPlan_005 where ThisPickedQty > 0

	drop table #tempPickTarget_005
	drop table #tempShipPlan_005
	drop table #tempBuffInv_005
END
GO