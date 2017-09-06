IF EXISTS(SELECT * FROM SYS.OBJECTS WHERE TYPE='P' AND name='USP_Report_RecSendDeposit')
	DROP PROCEDURE USP_Report_RecSendDeposit	
GO
CREATE PROCEDURE [dbo].[USP_Report_RecSendDeposit]
(
	@Locations varchar(8000),
	@Items varchar(8000),
	@BeginDate datetime,
	@EndDate datetime,
	@SortDesc varchar(100),
	@PageSize int,
	@Page int,
	@IsSummaryBySAPLoc bit,
	@SummaryLevel int
)
AS
BEGIN
	SET NOCOUNT ON
/*
	exec USP_Report_RecSendDeposit @Locations='',@Items='',@BeginDate='2012-06-01 10:23:51',@EndDate='2012-07-01 00:00:00',@SortDesc=NULL,@PageSize=20,@Page=1,@IsSummaryBySAPLoc=0,@SummaryLevel=2
*/
	IF EXISTS(SELECT 1 FROM MD_FinanceCalendar a WHERE EndDate>@BeginDate AND IsClose=1)
	BEGIN
		SELECT Id,ABS(DATEDIFF(SECOND,StartDate,@BeginDate)) AS StartGap,ABS(DATEDIFF(SECOND,EndDate,@BeginDate)) AS EndGap
		INTO #TMPGAP FROM MD_FinanceCalendar WHERE IsClose=1
		DECLARE @StartGap bigint,@EndGap bigint,@FinanceCalendarId int,@InvDate datetime,@StartDate datetime
		SELECT @StartGap=MIN(StartGap) FROM #TMPGAP
		SELECT TOP 1 @FinanceCalendarId=Id FROM #TMPGAP WHERE StartGap=@StartGap
		SELECT @StartDate=StartDate FROM dbo.MD_FinanceCalendar WHERE Id=@FinanceCalendarId
		SET @InvDate=DATEADD(DAY,DATEDIFF(SECOND,@StartDate,@BeginDate)/(60*60*24),@StartDate)
	END
	ELSE
	BEGIN
		SELECT TOP 1 @InvDate=DATEADD(DAY,-1,EndDate) FROM MD_FinanceCalendar WHERE IsClose=1 ORDER BY Id DESC
	END	
	--PRINT @InvDate
	--RETURN
	DECLARE @SqlHeader varchar(8000),@SqlTail varchar(8000)
	DECLARE @SqlDetail1 varchar(8000),@SqlDetail2 varchar(8000)

	DECLARE @i int
	DECLARE @max int
	DECLARE @Statement nvarchar(4000)
	DECLARE @Parameter nvarchar(4000)
	DECLARE @ItemWhere varchar(8000)=''
	DECLARE @LocationWhere varchar(8000)=''
	DECLARE @PagePara varchar(8000)=''
	DECLARE @TableSuffix varchar(50)
	SET @TableSuffix=CONVERT(varchar(6),@InvDate,112)
	
	IF(ISNULL(@Locations,'')<>'')
	BEGIN
		SET @LocationWhere=ISNULL(@LocationWhere,'')+' AND Location IN ('''+replace(@Locations,',',''',''')+''') '
	END
	
	IF(ISNULL(@Items,'')<>'')
	BEGIN
		SET @ItemWhere=ISNULL(@ItemWhere,'')+' AND lt.item in ('''+replace(@Items,',',''',''')+''') '
	END

	---排序条件
	IF ISNULL(@SortDesc,'')=''
	BEGIN
		SET @SortDesc=' ORDER BY Location ASC'
	END
	
	--PRINT @LocationWhere
	---查询出结果时需要的条件
	IF @Page>0
	BEGIN
		SET @PagePara='WHERE rowid BETWEEN '+cast(@PageSize*(@Page-1) as varchar(50))+' AND '++cast(@PageSize*(@Page) as varchar(50))
	END
	
	CREATE TABLE #tempStandarInv(Item varchar(50), Location varchar(50), BOPQty decimal(18,8))
	CREATE INDEX IX_tempStandarInv_1 ON #tempStandarInv(Location,Item)	
	
	CREATE TABLE #tempInvSummary(Item varchar(50), Location varchar(50), BOPQty decimal(18,8),
			InputQty decimal(18,8), OutputQty decimal(18,8))
	CREATE INDEX IX_tempInvSummary_1 ON #tempInvSummary(Location,Item)	
	
	CREATE TABLE #tempIOSummary(Item varchar(50), Location varchar(50), InputQty decimal(18,8), OutputQty decimal(18,8))
	CREATE INDEX IX_tempIOSummary_1 ON #tempIOSummary(Location,Item)
	
	CREATE TABLE #tempTransSummary(Item varchar(50),Location varchar(50), InputQty decimal(18,8), OutputQty decimal(18,8))
	CREATE INDEX IX_tempTransSummary_1 ON #tempTransSummary(Location,Item)		
	
	CREATE TABLE #tempResult(rowid int,Item varchar(50),Location varchar(50), BOPQty decimal(18,8),
			InputQty decimal(18,8), OutputQty decimal(18,8), EOPQty decimal(18,8))			
	
	SELECT @SqlHeader='',@SqlTail='',@max=19,@i=0, @SqlDetail1='',@SqlDetail2=''
		
	--查找基准库存
	set @SqlHeader='select Item, Location, SUM(QualifyQty+InspectQty+RejectQty) AS BOPQty
		from INV_DailyInvBalance_'+@TableSuffix+' AS lt where InvDate = '''+CONVERT(varchar(19),@InvDate,121)+''' '+@ItemWhere+' '+@LocationWhere+'
		GROUP BY Item, Location'

	--PRINT @SqlHeader
	INSERT INTO #tempStandarInv
	EXEC(@SqlHeader)
	
	--PRINT @InvDate
	--PRINT @BeginDate
	--PRINT @EndDate
	--循环结算每张表期初库存
	WHILE @i<@max
	BEGIN
		--PRINT @i
		SET @SqlHeader='SELECT * FROM (SELECT lt.Item, CASE WHEN IOType=0 THEN lt.LocTo ELSE lt.LocFrom END AS Location,
				SUM(CASE WHEN IOType = 0 then Qty else 0 end) as InputQty,
				SUM(CASE WHEN IOType = 1 then -Qty else 0 end) as OutputQty
				FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
				WHERE lt.EffDate >= '''+Convert(varchar(19), @InvDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(19), @BeginDate, 121)+''' '+@ItemWhere+'
				GROUP BY lt.Item, CASE WHEN IOType=0 THEN lt.LocTo ELSE lt.LocFrom END) AS T WHERE 1=1 '+@LocationWhere+''
		
		PRINT @SqlHeader	
		INSERT INTO	#tempTransSummary
		exec(@SqlHeader)			

		SET @SqlHeader='SELECT * FROM (SELECT lt.Item, CASE WHEN IOType=0 THEN lt.LocTo ELSE lt.LocFrom END AS Location,
				SUM(CASE WHEN IOType = 0 then Qty else 0 end) as InputQty,
				SUM(CASE WHEN IOType = 1 then -Qty else 0 end) as OutputQty
				FROM INV_LocTrans_'+Cast(@i AS varchar(10))+' AS lt
				WHERE lt.EffDate >= '''+Convert(varchar(19), @BeginDate, 121)+'''  and lt.EffDate < '''+Convert(varchar(19), @EndDate, 121)+''' '+@ItemWhere+'
				GROUP BY lt.Item, CASE WHEN IOType=0 THEN lt.LocTo ELSE lt.LocFrom END) AS T WHERE 1=1 '+@LocationWhere+''
		
		PRINT @SqlHeader		
		INSERT INTO	#tempIOSummary
		exec(@SqlHeader)					

		SET @i=@i+1
	END

	--select * from #tempStandarInv
	--select * from #tempTransSummary
	---得到期初库存			
	INSERT INTO #tempInvSummary 
	(Item, Location, BOPQty, InputQty, OutputQty)
	SELECT std.Item, std.Location, std.BOPQty, ISNULL(ts.InputQty,0), ISNULL(ts.OutputQty,0)
	FROM #tempStandarInv AS std 
	LEFT JOIN #tempTransSummary AS ts ON std.Item = ts.Item and std.Location = ts.Location
	
	INSERT INTO #tempInvSummary 
	(Item, Location, BOPQty, InputQty, OutputQty)
	SELECT ts.Item, ts.Location, 0 AS BOPQty, ts.InputQty, ts.OutputQty
	FROM #tempStandarInv AS std 
	RIGHT JOIN #tempTransSummary AS ts ON std.Item = ts.Item and std.Location = ts.Location
	WHERE std.Item IS NULL
	
	TRUNCATE TABLE #tempStandarInv
	TRUNCATE TABLE #tempTransSummary
	----利用期初库存加事务汇总算出期末库存	
	--INSERT INTO #tempResult(Item, Location, SAPLocation, BOPQty, InputQty, OutputQty, EOPQty)
	--SELECT ts.Item,ts.Location,l.SAPLocation,ts.BOPQty, 
	--ISNULL(ios.InputQty,0),ISNULL(ios.OutputQty,0),
	--ts.BOPQty+ISNULL(ios.InputQty,0)-ISNULL(ios.OutputQty,0)
	--FROM #tempInvSummary AS ts 
	--LEFT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
	--LEFT JOIN MD_Location l ON ts.Location=l.Code
	
	--INSERT INTO #tempResult(Item, Location, SAPLocation, BOPQty, InputQty, OutputQty, EOPQty)
	--SELECT ios.Item,ios.Location,l.SAPLocation,0 AS BOPQty, 
	--ios.InputQty,ios.OutputQty,ios.InputQty-ios.OutputQty AS EOPQty
	--FROM #tempInvSummary AS ts 
	--RIGHT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
	--LEFT JOIN MD_Location l ON ios.Location=l.Code
	--WHERE ts.Item IS NULL
	

	---最后的查询结果,包含2个数据集,一个是总数一个是分页数据
	IF @IsSummaryBySAPLoc=1
	BEGIN
		--汇总到SAP库位
		SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ts.Item,l.SAPLocation AS Location,
			SUM(ts.BOPQty) AS BOPQty, SUM(ISNULL(ios.InputQty,0)) AS InputQty,SUM(ISNULL(ios.OutputQty,0)) AS OutputQty,
			SUM(ts.BOPQty+ISNULL(ios.InputQty,0)-ISNULL(ios.OutputQty,0)) AS EOPQty
			FROM #tempInvSummary AS ts 
			LEFT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
			INNER JOIN MD_Location l ON ts.Location=l.Code GROUP BY ts.Item,l.SAPLocation) AS T
			WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
		insert into #TempResult 
		exec(@SqlHeader)
		
		SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ios.Item,l.SAPLocation AS Location,
			0 AS BOPQty, SUM(ios.InputQty) AS InputQty, SUM(ios.OutputQty) AS OutputQty, SUM(ios.InputQty-ios.OutputQty) AS EOPQty
			FROM #tempInvSummary AS ts 
			RIGHT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
			INNER JOIN MD_Location l ON ios.Location=l.Code
			WHERE ts.Item IS NULL GROUP BY ios.Item,l.SAPLocation) AS T
			WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
		insert into #TempResult 
		exec(@SqlHeader)			
			
		select count(1) from #TempResult
		exec('select top('+@PageSize+')  Location, Item, BOPQty, InputQty, OutputQty, EOPQty from #TempResult '+@PagePara) 		
	END
	ELSE
	BEGIN
		IF @SummaryLevel=0
		BEGIN
			--不汇总
			SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ts.Item,ts.Location,
				ts.BOPQty, ISNULL(ios.InputQty,0) AS InputQty,ISNULL(ios.OutputQty,0) AS OutputQty,
				ts.BOPQty+ISNULL(ios.InputQty,0)-ISNULL(ios.OutputQty,0) AS EOPQty
				FROM #tempInvSummary AS ts 
				LEFT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location) AS T
				WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
			insert into #TempResult 
			exec(@SqlHeader)
			
			SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ios.Item, ios.Location,
				0 AS BOPQty, ios.InputQty, ios.OutputQty, ios.InputQty-ios.OutputQty AS EOPQty
				FROM #tempInvSummary AS ts 
				RIGHT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
				WHERE ts.Item IS NULL) AS T WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
			INSERT INTO #TempResult 
			EXEC(@SqlHeader)			

			PRINT @SqlHeader	
			SELECT count(1) from #TempResult
			EXEC('select top('+@PageSize+')  Location, Item, BOPQty, InputQty, OutputQty, EOPQty from #TempResult '+@PagePara)
		END
		ELSE IF @SummaryLevel=1
		BEGIN
			--汇总到区域			
			SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ts.Item,l.Region AS Location,
				SUM(ts.BOPQty) AS BOPQty, SUM(ISNULL(ios.InputQty,0)) AS InputQty,SUM(ISNULL(ios.OutputQty,0)) AS OutputQty,
				SUM(ts.BOPQty+ISNULL(ios.InputQty,0)-ISNULL(ios.OutputQty,0)) AS EOPQty
				FROM #tempInvSummary AS ts 
				LEFT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
				INNER JOIN MD_Location l ON ts.Location=l.Code GROUP BY ts.Item,l.Region) AS T
				WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
			insert into #TempResult 
			exec(@SqlHeader)
			
			SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ios.Item,l.Region AS Location,
				0 AS BOPQty, SUM(ios.InputQty) AS InputQty, SUM(ios.OutputQty) AS OutputQty, SUM(ios.InputQty-ios.OutputQty) AS EOPQty
				FROM #tempInvSummary AS ts 
				RIGHT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
				INNER JOIN MD_Location l ON ios.Location=l.Code
				WHERE ts.Item IS NULL GROUP BY ios.Item,l.Region) AS T
				WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
			insert into #TempResult 
			exec(@SqlHeader)			
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+')  Location, Item, BOPQty, InputQty, OutputQty, EOPQty from #TempResult '+@PagePara) 		
		END
		ELSE IF @SummaryLevel=2
		BEGIN
			--汇总到车间
			SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ts.Item,r.Workshop AS Location,
				SUM(ts.BOPQty) AS BOPQty, SUM(ISNULL(ios.InputQty,0)) AS InputQty,SUM(ISNULL(ios.OutputQty,0)) AS OutputQty,
				SUM(ts.BOPQty+ISNULL(ios.InputQty,0)-ISNULL(ios.OutputQty,0)) AS EOPQty
				FROM #tempInvSummary AS ts 
				LEFT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
				INNER JOIN MD_Location l ON ts.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region GROUP BY ts.Item,r.Workshop) AS T
				WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
			insert into #TempResult 
			exec(@SqlHeader)
			
			SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ios.Item,r.Workshop AS Location,
				0 AS BOPQty, SUM(ios.InputQty) AS InputQty, SUM(ios.OutputQty) AS OutputQty, SUM(ios.InputQty-ios.OutputQty) AS EOPQty
				FROM #tempInvSummary AS ts 
				RIGHT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
				INNER JOIN MD_Location l ON ios.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region
				WHERE ts.Item IS NULL  GROUP BY ios.Item,r.Workshop) AS T
				WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
			insert into #TempResult 
			exec(@SqlHeader)			
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+')  Location, Item, BOPQty, InputQty, OutputQty, EOPQty from #TempResult '+@PagePara) 	
		END
		ELSE IF @SummaryLevel=3
		BEGIN
			--汇总到工厂

			--汇总到车间
			SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ts.Item,r.Plant AS Location,
				SUM(ts.BOPQty) AS BOPQty, SUM(ISNULL(ios.InputQty,0)) AS InputQty,SUM(ISNULL(ios.OutputQty,0)) AS OutputQty,
				SUM(ts.BOPQty+ISNULL(ios.InputQty,0)-ISNULL(ios.OutputQty,0)) AS EOPQty
				FROM #tempInvSummary AS ts 
				LEFT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
				INNER JOIN MD_Location l ON ts.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region GROUP BY ts.Item,r.Plant) AS T
				WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
			insert into #TempResult 
			exec(@SqlHeader)
			
			SET @SqlHeader = 'SELECT  row_number() over('+@SortDesc+'), * FROM( SELECT ios.Item,r.Plant AS Location,
				0 AS BOPQty, SUM(ios.InputQty) AS InputQty, SUM(ios.OutputQty) AS OutputQty, SUM(ios.InputQty-ios.OutputQty) AS EOPQty
				FROM #tempInvSummary AS ts 
				RIGHT JOIN #tempIOSummary AS ios ON ts.Item=ios.Item AND ts.Location=ios.Location
				INNER JOIN MD_Location l ON ios.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region
				WHERE ts.Item IS NULL  GROUP BY ios.Item,r.Plant) AS T
				WHERE BOPQty<>0 OR InputQty<>0 OR OutputQty<>0 OR EOPQty<>0'
			insert into #TempResult 
			exec(@SqlHeader)			
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+')  Location, Item, BOPQty, InputQty, OutputQty, EOPQty from #TempResult '+@PagePara) 	
		END
	END		
	DROP TABLE #tempIOSummary
	DROP TABLE #tempInvSummary
	DROP TABLE #tempStandarInv
	DROP TABLE #tempTransSummary	
END





GO


