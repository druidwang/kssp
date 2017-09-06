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
		--查找本次月结的会计期间
		SELECT @StartDate = StartDate, @EndDate = EndDate FROM MD_FinanceCalendar 
			WHERE FinanceYear = @FinanceYear and FinanceMonth = @FinanceMonth
			
		set @CycleDate = @StartDate
		----查找所有需要月结得TransTable
		--SELECT id=ROW_NUMBER()OVER(ORDER BY name),name into #tempSumTabName FROM sys.objects WHERE type=N'U' AND name like N'INV_LocTrans_%'
		SELECT @sql='',@max=19,@i=0
		---创建前一天的库存表
		SELECT Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty, TobeQualifyQty, TobeInspectQty, TobeRejectQty 
			into #tempPreviousDayInv from INV_DailyInvBalance where 1<>1
		
		SET @sql='SELECT Item, Location, ManufactureParty, LotNo, IsCs, QualifyQty, InspectQty, RejectQty, TobeQualifyQty, TobeInspectQty, TobeRejectQty 
			 from INV_DailyInvBalance_'+@LastMonthSuffix+' where InvDate = '''+Convert(varchar(10), dateadd(day, -1, @StartDate), 121)+''''
		--PRINT @sql
		INSERT INTO #tempPreviousDayInv
		EXEC(@sql)	
		--SELECT * FROM #tempPreviousDayInv
		--RETURN
		--循环结算每天期末库存和库存事务
		--SET @CycleDate='2012-01-08'
		--SET @EndDate='2012-06-20'
		WHILE @CycleDate < @EndDate
		BEGIN			
			SET @i=0
			WHILE @i<@max
			BEGIN
				--PRINT @max
				--查找前一天库存,第一次循环查找得为上月期末库存
				set @sql='select Item, Location, ManufactureParty, LotNo, IsCs, 
					SUM(CASE WHEN QualityType = 0 then Qty else 0 end) as QualifyQty,
					SUM(CASE WHEN QualityType = 1 then Qty else 0 end) as InspectQty,
					SUM(CASE WHEN QualityType = 2 then Qty else 0 end) as RejectQty,
					SUM(CASE WHEN QualityType = 0 then TobeQty else 0 end) as TobeQualifyQty,
					SUM(CASE WHEN QualityType = 1 then TobeQty else 0 end) as TobeInspectQty,
					SUM(CASE WHEN QualityType = 2 then TobeQty else 0 end) as TobeRejectQty
					from 
					(
					 ---普通的移库采购事务的计算
						--汇总入库库存
						SELECT lt.Item, lt.LocTo AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IOType=0 AND lt.IsCs=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocTo, hu.ManufactureParty , lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
						--汇总出库库存
						SELECT lt.Item, lt.LocFrom AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						SUM(lt.Qty) AS Qty,0 AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IOType=1 AND lt.IsCs=0
						AND TransType NOT IN (201, 202, 203, 204, 205, 206)
						GROUP BY lt.Item, lt.LocFrom, hu.ManufactureParty , lt.LotNo, lt.IsCs, lt.QualityType
					----寄售移库的事务计算	
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
					----寄售结算的事务	
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
					----寄售采购的事务
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

					----非寄售在途	
					----移库出库301/305，增加目的库位待收库存
					----移库入库冲销304/308，增加目的库位待收库存
					----移库退货出库311/315，增加目的库位待收库存
					----移库退货入库冲销314/318，增加目的库位待收库存
						UNION ALL
						SELECT lt.Item, lt.LocTo AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						0 AS QTY,SUM(-lt.Qty) AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IsCs=0 AND lt.ActBill=0 AND IOType = 1 and TransType in (301, 305, 304, 308, 311, 315, 314, 318) 
						GROUP BY lt.Item, lt.LocTo,hu.ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType
						UNION ALL
					----移库出库冲销302/306，减少目的库位待收库存
					----移库入库303/307，减少目的库位待收库存
					----移库退货出库冲销312/316，减少目的库位待收库存
					----移库退货入库313/317，减少目的库位待收库存
						SELECT lt.Item, lt.LocTo AS Location, hu.ManufactureParty AS ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType, 
						0 AS QTY,SUM(-lt.Qty) AS TobeQty
						FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
						LEFT JOIN INV_Hu AS hu ON lt.Huid=hu.Huid 
						WHERE lt.EffDate >= '''+Convert(varchar(10), @CycleDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(10), dateadd(day,1,@CycleDate), 121)+'''
						AND lt.IsCs=0 AND lt.ActBill=0 AND IOType = 0 and TransType in (302, 306, 303, 307, 312, 316, 313, 317) 
						GROUP BY lt.Item, lt.LocTo ,hu.ManufactureParty, lt.LotNo, lt.IsCs, lt.QualityType	
						
					----寄售在途
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
			----------------------------期末库存------------------------------------------
			--插入DailyInvBalance，先以昨天期末库存为基准生成当天期末库存
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
					
			--插入DailyInvBalance，对于昨天期末库存不存在的记录，直接记录当天期末库存
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
			 
		RAISERROR(N'月结无法完成，数据库产生错误。', 16, 1)
	END CATCH	
END
