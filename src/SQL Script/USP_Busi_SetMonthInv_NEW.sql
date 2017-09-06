/****** Object:  StoredProcedure [dbo].[USP_Busi_SetMonthInv_NEW]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_SetMonthInv_NEW')
	DROP PROCEDURE USP_Busi_SetMonthInv_NEW
CREATE PROCEDURE [dbo].[USP_Busi_SetMonthInv_NEW]
(
	@FinanceYear int,
	@FinanceMonth int,
	@UserId int,
	@UserNm varchar(100)
)
AS 
BEGIN
	SET NOCOUNT ON
----USP_Busi_SetMonthInv_NEW 2012,2,1,'su'
	BEGIN TRY
		DECLARE @StartDate datetime
		DECLARE @EndDate datetime
		DECLARE @CycleDate datetime
		DECLARE @CurrLoopDate datetime
		DECLARE @DateTimeNow datetime = GetDate();
		DECLARE @sql varchar(max)
		DECLARE @i int
		DECLARE @max int
		DECLARE @SumTabName varchar(50)
		DECLARE @Statement nvarchar(4000)
		DECLARE @Parameter nvarchar(4000)

		CREATE TABLE #tempTodayTrans(Item varchar(50),Location varchar(50),	ManufactureParty varchar(50),
			LotNo varchar(50) ,IsCs bit,QualifyQty decimal(18,8),InspectQty decimal(18,8),RejectQty decimal(18,8),
			TobeQualifyQty decimal(18,8), TobeInspectQty decimal(18,8), TobeRejectQty decimal(18,8))
		--���ұ����½�Ļ���ڼ�
		SELECT @StartDate = StartDate, @EndDate = EndDate FROM MD_FinanceCalendar 
			WHERE FinanceYear = @FinanceYear and FinanceMonth = @FinanceMonth
			
		set @CycleDate = dateadd(day, -1, @StartDate)
		----����������Ҫ�½��TransTable
		--SELECT id=ROW_NUMBER()OVER(ORDER BY name),name into #tempSumTabName FROM sys.objects WHERE type=N'U' AND name like N'INV_LocTrans_%'
		SELECT @sql='',@max=19,@i=0
		---����ǰһ��Ŀ���
		SELECT Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty, TobeQualifyQty, TobeInspectQty, TobeRejectQty 
			into #tempPreviousDayInv from INV_DailyInvBalance where 1<>1
		
		--ѭ������ÿ����ĩ���Ϳ������
		--SET @CycleDate='2012-01-08'
		--SET @EndDate='2012-01-10'
		WHILE @CycleDate < dateadd(day, -1, @EndDate)
		BEGIN	
			SET @i=0
			WHILE @i<@max
			BEGIN
				--PRINT 'CycleData='+Convert(varchar(10),@CycleDate,121)+'; @i='+Cast(@i as varchar(10))
				--PRINT @max
				--����ǰһ����,��һ��ѭ�����ҵ�Ϊ������ĩ���
				set @sql='SELECT Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty, TobeQualifyQty, TobeInspectQty, TobeRejectQty 
					FROM INV_DailyInvBalance_'+CAST(@i as varchar(10))+' where InvDate = '''+Convert(varchar(10), @CycleDate, 121)+''''
				
				--print @sql
				--exec(@sql)
				INSERT INTO #tempPreviousDayInv
				EXEC(@sql)
				
				--where EffDate between '''+Convert(varchar(10), @CycleDate, 121)+'''  and '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
				--SELECT @SumTabName=name FROM #tempSumTabName WHERE id=@i
				set @sql='select Item, Location, ManufactureParty, LotNo, IsCs, 
					SUM(CASE WHEN QualityType = 0 then Qty else 0 end) as QualifyQty,
					SUM(CASE WHEN QualityType = 1 then Qty else 0 end) as InspectQty,
					SUM(CASE WHEN QualityType = 2 then Qty else 0 end) as RejectQty,
					SUM(CASE WHEN QualityType = 0 then TobeQty else 0 end) as TobeQualifyQty,
					SUM(CASE WHEN QualityType = 1 then TobeQty else 0 end) as TobeInspectQty,
					SUM(CASE WHEN QualityType = 2 then TobeQty else 0 end) as TobeRejectQty
					from 
					(
					 ---��ͨ���ƿ�ɹ�����ļ���
						--���������
						SELECT lt.Item, lt.LocTo AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND lt.IOType=0 AND lt.IsCs=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocTo, hu.ManufactureParty , lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
						--���ܳ�����
						SELECT lt.Item, lt.LocFrom AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND lt.IOType=1 AND lt.IsCs=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocFrom, hu.ManufactureParty , lt.LotNo, lt.IsCs, lt.QualityType
					----�����ƿ���������	
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, pb.Party AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_PlanBill AS pb ON lt.PlanBill=pb.Id 
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND lt.IOType=0 AND lt.IsCs=1 AND lt.ActBill=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocTo, pb.Party, lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
						SELECT lt.Item, lt.LocFrom, pb.Party AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_PlanBill AS pb ON lt.PlanBill=pb.Id 
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND lt.IOType=1 AND lt.IsCs=1 AND lt.ActBill=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocFrom, pb.Party, lt.LotNo, lt.IsCs, lt.QualityType
					----���۽��������	
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, ab.Party AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.PlanBillQty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_ActBill AS ab ON lt.ActBill=ab.Id 
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND lt.IOType=0 AND lt.ActBill<>0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocTo, ab.Party, lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
						SELECT lt.Item, lt.LocFrom AS Location, ab.Party AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.PlanBillQty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_ActBill AS ab ON lt.ActBill=ab.Id 
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND lt.IOType=1 AND lt.ActBill<>0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocFrom, ab.Party, lt.LotNo, lt.IsCs, lt.QualityType
					----���۲ɹ�������
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, CASE WHEN lt.ActBill<>0 THEN ab.Party ELSE pb.Party END AS ManufactureParty, 
						lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_PlanBill AS pb ON lt.PlanBill=pb.Id
						LEFT JOIN BIL_ActBill AS ab ON lt.ActBill=ab.Id
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND TransType IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocTo,CASE WHEN lt.ActBill<>0 THEN ab.Party ELSE pb.Party END, lt.LotNo, lt.IsCs, lt.QualityType
					
					----�Ǽ�����;	
					----�ƿ����301/305������Ŀ�Ŀ�λ���տ��
					----�ƿ�������304/308������Ŀ�Ŀ�λ���տ��
					----�ƿ��˻�����311/315������Ŀ�Ŀ�λ���տ��
					----�ƿ��˻�������314/318������Ŀ�Ŀ�λ���տ��
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						0 AS QTY,SUM(-lt.Qty) AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND lt.IsCs=0 AND lt.ActBill=0 AND IOType = 1 and TransType in (301, 305, 304, 308, 311, 315, 314, 318) 
						GROUP BY lt.Item, lt.LocTo,hu.ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
					----�ƿ�������302/306������Ŀ�Ŀ�λ���տ��
					----�ƿ����303/307������Ŀ�Ŀ�λ���տ��
					----�ƿ��˻��������312/316������Ŀ�Ŀ�λ���տ��
					----�ƿ��˻����313/317������Ŀ�Ŀ�λ���տ��
						SELECT lt.Item, lt.LocTo AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						0 AS QTY,SUM(-lt.Qty) AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND lt.IsCs=0 AND lt.ActBill=0 AND IOType = 0 and TransType in (302, 306, 303, 307, 312, 316, 313, 317) 
						GROUP BY lt.Item, lt.LocTo ,hu.ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType	
						
					----������;
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, CASE WHEN lt.ActBill<>0 THEN ab.Party ELSE pb.Party END AS ManufactureParty, 
						lt.LotNo, 1 AS IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_PlanBill AS pb ON lt.PlanBill=pb.Id
						LEFT JOIN BIL_ActBill AS ab ON lt.ActBill=ab.Id
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND (lt.IsCs=1 OR (lt.IsCs=0 AND lt.ActBill<>0)) AND IOType = 1 AND lt.TransType IN (301, 305, 304, 308, 311, 315, 314, 318) 
						GROUP BY lt.Item, lt.LocTo,CASE WHEN lt.ActBill<>0 THEN ab.Party ELSE pb.Party END, lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						0 AS QTY,SUM(-lt.Qty) AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate > '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate <= '''+Convert(varchar(10), dateadd(day,2,@CycleDate), 121)+'''
						AND lt.IsCs=1 AND lt.ActBill=0 AND IOType = 0 and TransType in (302, 306, 303, 307, 312, 316, 313, 317) 
						GROUP BY lt.Item, lt.LocTo ,hu.ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType											
					) as A GROUP BY Item, Location,ManufactureParty, LotNo, IsCs, QualityType'
					--PRINT @sql	
					PRINT datalength(@sql)
					--exec(@sql)
					INSERT INTO	#tempTodayTrans
					exec(@sql)
					
					
					SET @CurrLoopDate=dateadd(day,1,@CycleDate)					
					
					--����DailyInvBalance������������ĩ���Ϊ��׼���ɵ�����ĩ���
					SET @Statement='insert into INV_DailyInvBalance_'+Cast(@i AS varchar(10))+'
					(Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty,
					TobeQualifyQty, TobeInspectQty, TobeRejectQty, FinanceYear, FinanceMonth, 
					InvDate, CreateUser, CreateUserNm,
					CreateDate)
					select pre.Item, pre.Location, pre.ManufactureParty, pre.LotNo, pre.IsCs,
					pre.QualifyQty + isnull(td.QualifyQty, 0), 
					pre.InspectQty + isnull(td.InspectQty, 0), 
					pre.RejectQty + isnull(td.RejectQty, 0),
					pre.TobeQualifyQty + isnull(td.TobeQualifyQty, 0), 
					pre.TobeInspectQty + isnull(td.TobeInspectQty, 0), 
					pre.TobeRejectQty + isnull(td.TobeRejectQty, 0),
					@FinanceYear_1 as FinanceYear, @FinanceMonth_1 as FinanceMonth,
					@cycleDate_1 as InvDate, @UserId_1 as CreateUser, @UserNm_1  as CreateUserNm,
					@DateTimeNow_1 as CreateDate
					from #tempPreviousDayInv as pre 
					left join #tempTodayTrans as td on pre.Item = td.Item and pre.Location = td.Location'
					SET @Parameter=N'@FinanceYear_1 int,@FinanceMonth_1 int,@cycleDate_1 datetime,@UserId_1 int,@UserNm_1 varchar(100),@DateTimeNow_1 datetime'
					--PRINT @Statement
					exec sp_executesql @Statement,@Parameter,
						@FinanceYear_1=@FinanceYear,@FinanceMonth_1=@FinanceMonth,@cycleDate_1=@CurrLoopDate,@UserId_1=@UserId,@UserNm_1=@UserNm,@DateTimeNow_1=@DateTimeNow						

					--PRINT '@i1='+Cast(@i as varchar(10))
					/*SET @Statement='select td.Item, td.Location, td.ManufactureParty, td.LotNo, td.IsCs,
					td.QualifyQty, 
					td.InspectQty, 
					td.RejectQty,
					td.TobeQualifyQty, 
					td.TobeInspectQty, 
					td.TobeRejectQty,					
					@FinanceYear_1 as FinanceYear, @FinanceMonth_1 as FinanceMonth,
					@cycleDate_1 as InvDate, @UserId_1 as CreateUser, @UserNm_1  as CreateUserNm,
					@DateTimeNow_1 as CreateDate
					from #tempPreviousDayInv as pre 
					right join #tempTodayTrans as td on pre.Item = td.Item and pre.Location = td.Location
					where pre.Item is null'
					SET @Parameter=N'@FinanceYear_1 int,@FinanceMonth_1 int,@cycleDate_1 datetime,@UserId_1 int,@UserNm_1 varchar(100),@DateTimeNow_1 datetime'
					--PRINT @Statement
					exec sp_executesql @Statement,@Parameter,
						@FinanceYear_1=@FinanceYear,@FinanceMonth_1=@FinanceMonth,@cycleDate_1=@CurrLoopDate,@UserId_1=@UserId,@UserNm_1=@UserNm,@DateTimeNow_1=@DateTimeNow						
					*/
					
					--����DailyInvBalance������������ĩ��治���ڵļ�¼��ֱ�Ӽ�¼������ĩ���
					SET @Statement='insert into INV_DailyInvBalance_'+Cast(@i AS varchar(10))+'
					(Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty,
					TobeQualifyQty, TobeInspectQty, TobeRejectQty, FinanceYear, FinanceMonth, 
					InvDate, CreateUser, CreateUserNm,
					CreateDate)
					select td.Item, td.Location, td.ManufactureParty, td.LotNo, td.IsCs,
					td.QualifyQty, 
					td.InspectQty, 
					td.RejectQty,
					td.TobeQualifyQty, 
					td.TobeInspectQty, 
					td.TobeRejectQty,
					@FinanceYear_1 as FinanceYear, @FinanceMonth_1 as FinanceMonth,
					@cycleDate_1 as InvDate, @UserId_1 as CreateUser, @UserNm_1  as CreateUserNm,
					@DateTimeNow_1 as CreateDate
					from #tempPreviousDayInv as pre 
					right join #tempTodayTrans as td on pre.Item = td.Item and pre.Location = td.Location
					where pre.Item is null'
					SET @Parameter=N'@FinanceYear_1 int,@FinanceMonth_1 int,@cycleDate_1 datetime,@UserId_1 int,@UserNm_1 varchar(100),@DateTimeNow_1 datetime'
					--PRINT @Statement
					exec sp_executesql @Statement,@Parameter,
						@FinanceYear_1=@FinanceYear,@FinanceMonth_1=@FinanceMonth,@cycleDate_1=@CurrLoopDate,@UserId_1=@UserId,@UserNm_1=@UserNm,@DateTimeNow_1=@DateTimeNow						
					
					--PRINT '@i2='+Cast(@i as varchar(10))
					/*
					SET @Statement='INSERT INTO INV_DailyTransBalance_'+Cast(@i AS varchar(10))+'
					(Item, TransType, IOType, Region, 
					Location, QualifyQty, InspectQty, 
					RejectQty, FinanceYear, FinanceMonth, 
					InvDate, CreateUser, CreateUserNm, CreateDate)
					select Item, TransType, 0 as IOType, PartyTo as Region, LocTo as Location,  
					sum(case when QualityType = 0 then Qty * UnitQty else 0 end) as QualifyQty,
					sum(case when QualityType = 1 then Qty * UnitQty else 0 end) as InspectQty,
					sum(case when QualityType = 2 then Qty * UnitQty else 0 end) as RejectQty,
					@FinanceYear_1 as FinanceYear, @FinanceMonth_1 as FinanceMonth,
					@cycleDate_1 as InvDate, @UserId_1 as CreateUser, @UserNm_1  as CreateUserNm,
					@DateTimeNow_1 as CreateDate
					from INV_LocTrans_'+Cast(@i AS varchar(10))+' 
					where EffDate between '''+Convert(varchar(10), @CycleDate, 121)+'''  and '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
					and IOType = 0 
					and TransType not in (607, 608, 609, 610, 611, 612)     --���˿�������
					group by Item, TransType, PartyTo, LocTo, QualityType'
					SET @Parameter=N'@FinanceYear_1 int,@FinanceMonth_1 int,@cycleDate_1 datetime,@UserId_1 int,@UserNm_1 varchar(100),@DateTimeNow_1 datetime'
					PRINT @Statement
					exec sp_executesql @Statement,@Parameter,
						@FinanceYear_1=@FinanceYear,@FinanceMonth_1=@FinanceMonth,@cycleDate_1=@cycleDate,@UserId_1=@UserId,@UserNm_1=@UserNm,@DateTimeNow_1=@DateTimeNow			
					PRINT '@i3='+Cast(@i as varchar(10))	
					
					SET @Statement='INSERT INTO INV_DailyTransBalance_'+Cast(@i AS varchar(10))+'
					(Item, TransType, IOType, Region, 
					Location, QualifyQty, InspectQty, 
					RejectQty, FinanceYear, FinanceMonth, 
					InvDate, CreateUser, CreateUserNm, CreateDate)
					select Item, TransType, 0 as IOType, PartyFrom as Region, LocFrom as Location,  
					sum(case when QualityType = 0 then Qty * UnitQty else 0 end) as QualifyQty,
					sum(case when QualityType = 1 then Qty * UnitQty else 0 end) as InspectQty,
					sum(case when QualityType = 2 then Qty * UnitQty else 0 end) as RejectQty,
					@FinanceYear_1 as FinanceYear, @FinanceMonth_1 as FinanceMonth,
					@cycleDate_1 as InvDate, @UserId_1 as CreateUser, @UserNm_1  as CreateUserNm,
					@DateTimeNow_1 as CreateDate
					from INV_LocTrans_'+Cast(@i AS varchar(10))+' 
					where EffDate between '''+Convert(varchar(10), @CycleDate, 121)+'''  and '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
					and IOType = 1 
					and TransType not in (607, 608, 609, 610, 611, 612)     --���˿�������
					group by Item, TransType, PartyFrom, LocFrom, QualityType'
					SET @Parameter=N'@FinanceYear_1 int,@FinanceMonth_1 int,@cycleDate_1 datetime,@UserId_1 int,@UserNm_1 varchar(100),@DateTimeNow_1 datetime'
					PRINT @Statement
					exec sp_executesql @Statement,@Parameter,
						@FinanceYear_1=@FinanceYear,@FinanceMonth_1=@FinanceMonth,@cycleDate_1=@cycleDate,@UserId_1=@UserId,@UserNm_1=@UserNm,@DateTimeNow_1=@DateTimeNow
					PRINT '@i4='+Cast(@i as varchar(10))
					*/
					
					--�����ʱ������
					
					TRUNCATE TABLE #tempPreviousDayInv
					TRUNCATE TABLE #tempTodayTrans
				SET @i=@i+1	
			END	
			----------------------------��ĩ���------------------------------------------
			set @CycleDate = dateadd(day, 1, @CycleDate);
		END
		
		DROP TABLE #tempPreviousDayInv 
		DROP TABLE #tempTodayTrans
	END TRY
	BEGIN CATCH
		INSERT INTO SYS_ErrorLog
		SELECT 'USP_SetMonthInv_All',Error_Message(),Error_Number(),Error_Severity(),
			Error_State(),Error_Procedure(),Error_Line();
			
		RAISERROR(N'�½��޷���ɣ����ݿ��������', 16, 1)
	END CATCH	
END

GO