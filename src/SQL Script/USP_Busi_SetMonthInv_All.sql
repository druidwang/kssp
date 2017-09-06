/****** Object:  StoredProcedure [dbo].[USP_Busi_SetMonthInv_All]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_SetMonthInv_All')
	DROP PROCEDURE USP_Busi_SetMonthInv_All
GO
CREATE PROCEDURE [dbo].[USP_Busi_SetMonthInv_All]
(
	@FinanceYear int,
	@FinanceMonth int,
	@UserId int,
	@UserNm varchar(100)
)
AS 
BEGIN
	SET NOCOUNT ON
----USP_Busi_SetMonthInv_All 2012,2,1,'su'
	BEGIN TRY
		DECLARE @StartDate datetime
		DECLARE @EndDate datetime
		DECLARE @CycleDate datetime
		DECLARE @CurrLoopDate datetime
		DECLARE @DateTimeNow datetime = GetDate();
		DECLARE @sql varchar(max)
		DECLARE @i int
		DECLARE @max int
		DECLARE @TableSuffix varchar(50)
		DECLARE @LastMonthSuffix varchar(50)
		DECLARE @SumTabName varchar(50)
		DECLARE @Statement nvarchar(4000)
		DECLARE @Parameter nvarchar(4000)
		
		IF @FinanceMonth=1
		BEGIN
			SET @LastMonthSuffix=CAST(@FinanceYear-1 AS VARCHAR(5))+RIGHT('0'+CAST(12 AS VARCHAR(5)),2)
		END
		ELSE
		BEGIN
			SET @LastMonthSuffix=CAST(@FinanceYear AS VARCHAR(5))+RIGHT('0'+CAST(@FinanceMonth-1 AS VARCHAR(5)),2)
		END
		SET @TableSuffix=CAST(@FinanceYear AS VARCHAR(5))+RIGHT('0'+CAST(@FinanceMonth AS VARCHAR(5)),2)
		SET @sql='IF NOT EXISTS(SELECT * FROM SYS.OBJECTS WHERE Type=''U'' AND Name= ''INV_DailyInvBalance_'+@TableSuffix+''')'+CHAR(10)
		SET @sql=@sql+'BEGIN 
			CREATE TABLE INV_DailyInvBalance_'+@TableSuffix+'(Id bigint IDENTITY(1,1) NOT NULL PRIMARY KEY NONCLUSTERED,Item varchar(50) NOT NULL,Location varchar(50) NOT NULL,
			SAPLocation varchar(50) NOT NULL,ManufactureParty varchar(50) NULL,LotNo varchar(50) NULL,IsCs bit NOT NULL,
			QualifyQty decimal(18, 8) NOT NULL,InspectQty decimal(18, 8) NOT NULL,RejectQty decimal(18, 8) NOT NULL,
			TobeQualifyQty decimal(18, 8) NOT NULL,TobeInspectQty decimal(18, 8) NOT NULL,
			TobeRejectQty decimal(18, 8) NOT NULL,FinanceYear int NOT NULL,FinanceMonth int NOT NULL,
			InvDate datetime NOT NULL,CreateUser int NOT NULL,CreateUserNm varchar(100) NOT NULL,CreateDate datetime NOT NULL)
			CREATE CLUSTERED INDEX IX_INV_DailyInvBalance_'+@TableSuffix+'_1 ON INV_DailyInvBalance_'+@TableSuffix+'(Item,Location)
			CREATE INDEX IX_INV_DailyInvBalance_'+@TableSuffix+'_2 ON INV_DailyInvBalance_'+@TableSuffix+'(InvDate)'+CHAR(13)
		SET @sql=@sql+'END'
		--PRINT @sql
		--return 
		EXEC(@sql)
		CREATE TABLE #tempTodayTransNoGroup(Item varchar(50),Location varchar(50),	ManufactureParty varchar(50),
			LotNo varchar(50) ,IsCs bit,QualifyQty decimal(18,8),InspectQty decimal(18,8),RejectQty decimal(18,8),
			TobeQualifyQty decimal(18,8), TobeInspectQty decimal(18,8), TobeRejectQty decimal(18,8))
		CREATE TABLE #tempTodayTrans(Item varchar(50),Location varchar(50),	ManufactureParty varchar(50),
			LotNo varchar(50) ,IsCs bit,QualifyQty decimal(18,8),InspectQty decimal(18,8),RejectQty decimal(18,8),
			TobeQualifyQty decimal(18,8), TobeInspectQty decimal(18,8), TobeRejectQty decimal(18,8))			
		--���ұ����½�Ļ���ڼ�
		SELECT @StartDate = StartDate, @EndDate = EndDate FROM MD_FinanceCalendar 
			WHERE FinanceYear = @FinanceYear and FinanceMonth = @FinanceMonth
			
		set @CycleDate = @StartDate
		----����������Ҫ�½��TransTable
		--SELECT id=ROW_NUMBER()OVER(ORDER BY name),name into #tempSumTabName FROM sys.objects WHERE type=N'U' AND name like N'INV_LocTrans_%'
		SELECT @sql='',@max=19,@i=0
		---����ǰһ��Ŀ���
		SELECT Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty, TobeQualifyQty, TobeInspectQty, TobeRejectQty 
			into #tempPreviousDayInv from INV_DailyInvBalance where 1<>1
		
		SET @sql='SELECT Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty, TobeQualifyQty, TobeInspectQty, TobeRejectQty 
			 from INV_DailyInvBalance_'+@LastMonthSuffix+' where InvDate = '''+Convert(varchar(10), dateadd(day, -1, @StartDate), 121)+''''
		--PRINT @sql
		INSERT INTO #tempPreviousDayInv
		EXEC(@sql)	
		--SELECT * FROM #tempPreviousDayInv
		--RETURN
		--ѭ������ÿ����ĩ���Ϳ������
		--SET @CycleDate='2012-01-08'
		--SET @EndDate='2012-06-20'
		WHILE @CycleDate < @EndDate
		BEGIN			
			SET @i=0
			WHILE @i<@max
			BEGIN
				--PRINT @max
				--����ǰһ����,��һ��ѭ�����ҵ�Ϊ������ĩ���
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
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IOType=0 AND lt.IsCs=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocTo, hu.ManufactureParty , lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
						--���ܳ�����
						SELECT lt.Item, lt.LocFrom AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IOType=1 AND lt.IsCs=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocFrom, hu.ManufactureParty , lt.LotNo, lt.IsCs, lt.QualityType
					----�����ƿ���������	
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, pb.Party AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_PlanBill AS pb ON lt.PlanBill=pb.Id 
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IOType=0 AND lt.IsCs=1 AND lt.ActBill=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocTo, pb.Party, lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
						SELECT lt.Item, lt.LocFrom, pb.Party AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_PlanBill AS pb ON lt.PlanBill=pb.Id 
						WHERE lt.EffDate >= '''+Convert(varchar(10),@CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IOType=1 AND lt.IsCs=1 AND lt.ActBill=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocFrom, pb.Party, lt.LotNo, lt.IsCs, lt.QualityType
					----���۽��������	
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, ab.Party AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.PlanBillQty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_ActBill AS ab ON lt.ActBill=ab.Id 
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IOType=0 AND lt.ActBill<>0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocTo, ab.Party, lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
						SELECT lt.Item, lt.LocFrom AS Location, ab.Party AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.PlanBillQty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_ActBill AS ab ON lt.ActBill=ab.Id 
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IOType=1 AND lt.ActBill<>0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocFrom, ab.Party, lt.LotNo, lt.IsCs, lt.QualityType
					----���۲ɹ�������
						UNION ALL
						SELECT lt.Item, CASE WHEN TransType in (201,205,202,206) THEN lt.LocTo ELSE lt.LocFrom END AS Location, CASE WHEN lt.ActBill<>0 THEN ab.Party ELSE pb.Party END AS ManufactureParty, 
						lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_PlanBill AS pb ON lt.PlanBill=pb.Id
						LEFT JOIN BIL_ActBill AS ab ON lt.ActBill=ab.Id
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND TransType IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, CASE WHEN TransType in (201,205,202,206) THEN lt.LocTo ELSE lt.LocFrom END,
							CASE WHEN lt.ActBill<>0 THEN ab.Party ELSE pb.Party END, lt.LotNo, lt.IsCs, lt.QualityType

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
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
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
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IsCs=0 AND lt.ActBill=0 AND IOType = 0 and TransType in (302, 306, 303, 307, 312, 316, 313, 317) 
						GROUP BY lt.Item, lt.LocTo ,hu.ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType	
						
					----������;
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, CASE WHEN lt.ActBill<>0 THEN ab.Party ELSE pb.Party END AS ManufactureParty, 
						lt.LotNo, 1 AS IsCs, lt.QualityType, 
						SUM(-lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN BIL_PlanBill AS pb ON lt.PlanBill=pb.Id
						LEFT JOIN BIL_ActBill AS ab ON lt.ActBill=ab.Id
						WHERE lt.EffDate >= '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND (lt.IsCs=1 OR (lt.IsCs=0 AND lt.ActBill<>0)) AND IOType = 1 AND lt.TransType IN (301, 305, 304, 308, 311, 315, 314, 318) 
						GROUP BY lt.Item, lt.LocTo,CASE WHEN lt.ActBill<>0 THEN ab.Party ELSE pb.Party END, lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						0 AS QTY,SUM(-lt.Qty) AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IsCs=1 AND lt.ActBill=0 AND IOType = 0 and TransType in (302, 306, 303, 307, 312, 316, 313, 317) 
						GROUP BY lt.Item, lt.LocTo ,hu.ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType											
					) as A GROUP BY Item, Location,ManufactureParty, LotNo, IsCs, QualityType'
					--PRINT @sql	
					--PRINT datalength(@sql)
					--exec(@sql)
					INSERT INTO	#tempTodayTransNoGroup
					exec(@sql)
								
				
				SET @i=@i+1	
			END	
			INSERT INTO #tempTodayTrans(Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty,
			TobeQualifyQty, TobeInspectQty, TobeRejectQty)
			SELECT Item, Location, ManufactureParty, LotNo, IsCs,
				SUM(QualifyQty),SUM(InspectQty),SUM(RejectQty),SUM(TobeQualifyQty),SUM(TobeInspectQty),SUM(TobeRejectQty)
			FROM #tempTodayTransNoGroup GROUP BY Item, Location, ManufactureParty, LotNo, IsCs
			TRUNCATE TABLE #tempTodayTransNoGroup
			
			--PRINT 'CycleData='+Convert(varchar(10),@CycleDate,121)+';'
			--select * from #tempTodayTrans
			--select * from #tempPreviousDayInv
			--return
			--PRINT '1.lastday data'
			----------------------------��ĩ���------------------------------------------
			--����DailyInvBalance������������ĩ���Ϊ��׼���ɵ�����ĩ���
			SET @Statement='insert into INV_DailyInvBalance_'+@TableSuffix+'
			(Item, Location, SAPLocation, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty,
			TobeQualifyQty, TobeInspectQty, TobeRejectQty, FinanceYear, FinanceMonth, 
			InvDate, CreateUser, CreateUserNm,
			CreateDate)
			SELECT pre.Item, pre.Location, l.SAPLocation, pre.ManufactureParty, pre.LotNo, pre.IsCs,
			pre.QualifyQty + isnull(td.QualifyQty, 0), 
			pre.InspectQty + isnull(td.InspectQty, 0), 
			pre.RejectQty + isnull(td.RejectQty, 0),
			pre.TobeQualifyQty + isnull(td.TobeQualifyQty, 0), 
			pre.TobeInspectQty + isnull(td.TobeInspectQty, 0), 
			pre.TobeRejectQty + isnull(td.TobeRejectQty, 0),
			@FinanceYear_1 AS FinanceYear, @FinanceMonth_1 AS FinanceMonth,
			@cycleDate_1 as InvDate, @UserId_1 as CreateUser, @UserNm_1  AS CreateUserNm,
			@DateTimeNow_1 AS CreateDate
			FROM #tempPreviousDayInv AS pre 
			LEFT JOIN MD_Location AS l ON pre.Location=l.Code
			LEFT JOIN #tempTodayTrans AS td ON pre.Item = td.Item AND pre.Location = td.Location
				AND pre.ManufactureParty = td.ManufactureParty AND pre.LotNo = td.LotNo AND pre.IsCs = td.IsCs'
			SET @Parameter=N'@FinanceYear_1 int,@FinanceMonth_1 int,@cycleDate_1 datetime,@UserId_1 int,@UserNm_1 varchar(100),@DateTimeNow_1 datetime'
			--PRINT @Statement
			exec sp_executesql @Statement,@Parameter,
				@FinanceYear_1=@FinanceYear,@FinanceMonth_1=@FinanceMonth,@cycleDate_1=@CycleDate,@UserId_1=@UserId,@UserNm_1=@UserNm,@DateTimeNow_1=@DateTimeNow						
			--PRINT '2.today data'
					
			--����DailyInvBalance������������ĩ��治���ڵļ�¼��ֱ�Ӽ�¼������ĩ���
			SET @Statement='insert into INV_DailyInvBalance_'+@TableSuffix+'
			(Item, Location, SAPLocation, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty,
			TobeQualifyQty, TobeInspectQty, TobeRejectQty, FinanceYear, FinanceMonth, 
			InvDate, CreateUser, CreateUserNm,
			CreateDate)
			select td.Item, td.Location, l.SAPLocation, td.ManufactureParty, td.LotNo, td.IsCs,
			td.QualifyQty, 
			td.InspectQty, 
			td.RejectQty,
			td.TobeQualifyQty, 
			td.TobeInspectQty, 
			td.TobeRejectQty,
			@FinanceYear_1 AS FinanceYear, @FinanceMonth_1 AS FinanceMonth,
			@cycleDate_1 AS InvDate, @UserId_1 as CreateUser, @UserNm_1  AS CreateUserNm,
			@DateTimeNow_1 AS CreateDate
			FROM #tempPreviousDayInv as pre 
			RIGHT JOIN #tempTodayTrans AS td ON pre.Item = td.Item AND pre.Location = td.Location
			AND pre.ManufactureParty = td.ManufactureParty AND pre.LotNo = td.LotNo AND pre.IsCs = td.IsCs
			LEFT JOIN MD_Location AS l ON td.Location=l.Code
			WHERE pre.Item IS NULL'
			SET @Parameter=N'@FinanceYear_1 int,@FinanceMonth_1 int,@cycleDate_1 datetime,@UserId_1 int,@UserNm_1 varchar(100),@DateTimeNow_1 datetime'
			--PRINT @Statement
			exec sp_executesql @Statement,@Parameter,
				@FinanceYear_1=@FinanceYear,@FinanceMonth_1=@FinanceMonth,@cycleDate_1=@CycleDate,@UserId_1=@UserId,@UserNm_1=@UserNm,@DateTimeNow_1=@DateTimeNow						
			
			TRUNCATE TABLE #tempPreviousDayInv
			TRUNCATE TABLE #tempTodayTrans
			
			set @sql='SELECT Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty, TobeQualifyQty, TobeInspectQty, TobeRejectQty 
				FROM INV_DailyInvBalance_'+@TableSuffix+' where InvDate = '''+Convert(varchar(10), @CycleDate, 121)+''''
			INSERT INTO #tempPreviousDayInv
			EXEC(@sql)				
			
			set @CycleDate = dateadd(day, 1, @CycleDate)
		END
		 
		DROP TABLE #tempPreviousDayInv 
		DROP TABLE #tempTodayTransNoGroup
		DROP TABLE #tempTodayTrans
	END TRY
	BEGIN CATCH
		INSERT INTO SYS_ErrorLog
		SELECT 'USP_SetMonthInv_All',Error_Message(),Error_Number(),Error_Severity(),
			Error_State(),Error_Procedure(),Error_Line();
			 
		RAISERROR(N'�½��޷���ɣ����ݿ��������', 16, 1)
	END CATCH	
END
