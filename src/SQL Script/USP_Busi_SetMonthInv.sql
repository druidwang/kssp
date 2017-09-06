/****** Object:  StoredProcedure [dbo].[USP_Busi_SetMonthInv]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_SetMonthInv')
	DROP PROCEDURE USP_Busi_SetMonthInv
CREATE PROCEDURE [dbo].[USP_Busi_SetMonthInv]
(
	@FinanceYear int,
	@FinanceMonth int,
	@UserId int,
	@UserNm varchar(100)
)
as 
begin

	declare @StartDate datetime;
	declare @EndDate datetime;
	declare @CycleDate datetime;
	declare @DateTimeNow datetime = GetDate();

	--���ұ����½�Ļ���ڼ�
	select @StartDate = StartDate, @EndDate = EndDate from MD_FinanceCalendar where FinanceYear = @FinanceYear and FinanceMonth = @FinanceMonth
	
	--����������ĩ���
	set @CycleDate = dateadd(day, -1, @StartDate);
	select Item, Location, QualifyQty, InspectQty, RejectQty, TobeQualifyQty, TobeInspectQty, TobeRejectQty into #tempPreviousDayInv from INV_DailyInvBalance where InvDate = @cycleDate;
	
	begin tran
		--ѭ������ÿ����ĩ���Ϳ������
		while @CycleDate < @EndDate
		begin
			set @CycleDate = dateadd(day, 1, @CycleDate);
				
			----------------------------��ĩ���------------------------------------------
			select Item, 
			Location, 
			SUM(CASE WHEN QualityType = 0 then Qty else 0 end) as QualifyQty,
			SUM(CASE WHEN QualityType = 1 then Qty else 0 end) as InspectQty,
			SUM(CASE WHEN QualityType = 2 then Qty else 0 end) as RejectQty,
			SUM(CASE WHEN QualityType = 0 then TobeQty else 0 end) as TobeQualifyQty,
			SUM(CASE WHEN QualityType = 1 then TobeQty else 0 end) as TobeInspectQty,
			SUM(CASE WHEN QualityType = 2 then TobeQty else 0 end) as TobeRejectQty
			into #tempTodayTrans from 
			(
				--���������
				select Item, LocTo as Location, QualityType, 
				SUM(Qty) as Qty, 0 as TobeQty 
				from INV_LocTrans 
				where EffDate between Convert(varchar(8), @CycleDate, 112)  and Convert(varchar(8), dateadd(day,1,@CycleDate), 112)
				and IOType = 0 
				group by Item, LocTo, QualityType
				union all
				--���ܳ�����
				select Item, LocFrom as Location, QualityType, 
				SUM(Qty) as Qty, 0 as TobeQty 
				from INV_LocTrans 
				where EffDate between Convert(varchar(8), @CycleDate, 112)  and Convert(varchar(8), dateadd(day,1,@CycleDate), 112)
				and IOType = 1 
				group by Item, LocFrom, QualityType
				union all 
				--�ƿ����301/305������Ŀ�Ŀ�λ���տ��
				--�ƿ�������304/308������Ŀ�Ŀ�λ���տ��
				--�ƿ��˻�����311/315������Ŀ�Ŀ�λ���տ��
				--�ƿ��˻�������314/318������Ŀ�Ŀ�λ���տ��
				select Item, LocTo as Location, QualityType, 
				0 as Qty, SUM(Qty) as TobeQty 
				from INV_LocTrans 
				where EffDate between Convert(varchar(8), @CycleDate, 112)  and Convert(varchar(8), dateadd(day,1,@CycleDate), 112)
				and IOType = 1 and TransType in (301, 305, 304, 308, 311, 315, 314, 318) 
				group by Item, LocTo, QualityType
				union all
				--�ƿ�������302/306������Ŀ�Ŀ�λ���տ��
				--�ƿ����303/307������Ŀ�Ŀ�λ���տ��
				--�ƿ��˻��������312/316������Ŀ�Ŀ�λ���տ��
				--�ƿ��˻����313/317������Ŀ�Ŀ�λ���տ��
				select Item, LocTo as Location, QualityType, 
				0 as Qty, SUM(-Qty) as TobeQty 
				from INV_LocTrans 
				where EffDate between Convert(varchar(8), @CycleDate, 112)  and Convert(varchar(8), dateadd(day,1,@CycleDate), 112)
				and IOType = 1 and TransType in (302, 306, 303, 307, 312, 316, 313, 317) 
				group by Item, LocTo, QualityType
			) as A group by Item, Location, QualityType;
					
			--����DailyInvBalance������������ĩ���Ϊ��׼���ɵ�����ĩ���
			insert into INV_DailyInvBalance 
			(Item, Location, 
			QualifyQty, InspectQty, RejectQty,
			TobeQualifyQty, TobeInspectQty, TobeRejectQty,
			FinanceYear, FinanceMonth, 
			InvDate, CreateUser, CreateUserNm,
			CreateDate)
			select pre.Item, pre.Location, 
			pre.QualifyQty + isnull(td.QualifyQty, 0), 
			pre.InspectQty + isnull(td.InspectQty, 0), 
			pre.RejectQty + isnull(td.RejectQty, 0),
			pre.TobeQualifyQty + isnull(td.TobeQualifyQty, 0), 
			pre.TobeInspectQty + isnull(td.TobeInspectQty, 0), 
			pre.TobeRejectQty + isnull(td.TobeRejectQty, 0),
			@FinanceYear as FinanceYear, @FinanceMonth as FinanceMonth,
			@cycleDate as InvDate, @UserId as CreateUser, @UserNm  as CreateUserNm,
			@DateTimeNow as CreateDate
			from #tempPreviousDayInv as pre 
			left join #tempTodayTrans as td on pre.Item = td.Item and pre.Location = td.Location;
			
			--����DailyInvBalance������������ĩ��治���ڵļ�¼��ֱ�Ӽ�¼������ĩ���
			insert into INV_DailyInvBalance 
			(Item, Location, 
			QualifyQty, InspectQty, RejectQty,
			TobeQualifyQty, TobeInspectQty, TobeRejectQty,
			FinanceYear, FinanceMonth, 
			InvDate, CreateUser, CreateUserNm,
			CreateDate)
			select td.Item, td.Location, 
			td.QualifyQty, 
			td.InspectQty, 
			td.RejectQty,
			td.TobeQualifyQty, 
			td.TobeInspectQty, 
			td.TobeRejectQty,
			@FinanceYear as FinanceYear, @FinanceMonth as FinanceMonth,
			@cycleDate as InvDate, @UserId as CreateUser, @UserNm  as CreateUserNm,
			@DateTimeNow as CreateDate
			from #tempPreviousDayInv as pre 
			right join #tempTodayTrans as td on pre.Item = td.Item and pre.Location = td.Location
			where pre.Item is null;
			
			--�ѽ�����ĩ��渳ֵ��#tempPreviousDayInv��Ϊ�´�ѭ����������ĩ���
			truncate table #tempPreviousDayInv;
			
			insert into #tempPreviousDayInv
			select Item, Location, QualifyQty, InspectQty, RejectQty, TobeQualifyQty, TobeInspectQty, TobeRejectQty 
			from INV_DailyInvBalance where InvDate = @cycleDate;
			
			----------------------------�������------------------------------------------
			--��������
			insert into INV_DailyTransBalance
			select Item, TransType, 0 as IOType, PartyFrom as Region, LocFrom as Location,  
			sum(case when QualityType = 0 then Qty * UnitQty else 0 end) as QualifyQty,
			sum(case when QualityType = 1 then Qty * UnitQty else 0 end) as InspectQty,
			sum(case when QualityType = 2 then Qty * UnitQty else 0 end) as RejectQty,
			@FinanceYear as FinanceYear, @FinanceMonth as FinanceMonth,
			@cycleDate as InvDate, @UserId as CreateUser, @UserNm  as CreateUserNm,
			@DateTimeNow as CreateDate
			from INV_LocTrans 
			where EffDate between Convert(varchar(8), @CycleDate, 112)  and Convert(varchar(8), dateadd(day,1,@CycleDate), 112)
			and IOType = 0 
			and TransType not in (607, 608, 609, 610, 611, 612)     --���˿�������
			group by Item, TransType, PartyFrom, LocFrom, QualityType
			
			--�������
			insert into INV_DailyTransBalance
			select Item, TransType, 1 as IOType, PartyTo as Region, LocTo as Location,  
			sum(case when QualityType = 0 then Qty * UnitQty else 0 end) as QualifyQty,
			sum(case when QualityType = 1 then Qty * UnitQty else 0 end) as InspectQty,
			sum(case when QualityType = 2 then Qty * UnitQty else 0 end) as RejectQty,
			@FinanceYear as FinanceYear, @FinanceMonth as FinanceMonth,
			@cycleDate as InvDate, @UserId as CreateUser, @UserNm  as CreateUserNm,
			@DateTimeNow as CreateDate
			from INV_LocTrans 
			where EffDate between Convert(varchar(8), @CycleDate, 112)  and Convert(varchar(8), dateadd(day,1,@CycleDate), 112)
			and IOType = 1 
			and TransType not in (607, 608, 609, 610, 611, 612)     --���˿�������
			group by Item, TransType, PartyTo, LocTo, QualityType
		end
	commit tran
end
GO
