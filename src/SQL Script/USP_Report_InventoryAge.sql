IF EXISTS(SELECT * FROM SYS.OBJECTS WHERE TYPE='P' AND name='USP_Report_InventoryAge')
	DROP PROCEDURE USP_Report_InventoryAge	
GO
CREATE PROCEDURE [dbo].[USP_Report_InventoryAge]
(
	@Locations varchar(8000),
	@Items varchar(8000),
	@SortDesc varchar(100),
	@PageSize int,
	@Page int,
	@IsSummaryBySAPLoc bit,
	@SummaryLevel int,
	@Range1 int,
	@Range2 int,
	@Range3 int,
	@Range4 int,
	@Range5 int,
	@Range6 int,
	@Range7 int,
	@Range8 int,
	@Range9 int,
	@Range10 int,
	@Range11 int
)
AS
BEGIN
/*
exec USP_Report_InventoryAge @Locations='BJS001,LOC000,LOC100,LOC101,LOC600,LOC601',@Items='',@SortDesc='',@PageSize=20,@Page=1,@IsSummaryBySAPLoc=1,@SummaryLevel=2,@Range1=7,@Range2=14,@Range3=30,@Range4=60,@Range5=90,@Range6=180,@Range7=360,@Range8=720,@Range9=1080,@Range10=1440,@Range11=1800
---默认值 7,14,30,60,90,180,360,720,1080,1440,1800
*/
	DECLARE @CurrDate datetime,@Range1Date datetime,@Range2Date datetime,@Range3Date datetime,
	@Range4Date datetime,@Range5Date datetime,@Range6Date datetime,@Range7Date datetime,@Range8Date datetime,
	@Range9Date datetime,@Range10Date datetime,@Range11Date datetime
	
	SET @CurrDate=GETDATE()
	IF(ISNULL(@Range1,0)<>0)
	BEGIN
		SET @Range1Date=DATEADD(DAY,-@Range1,@CurrDate)
	END
	IF(ISNULL(@Range2,0)<>0)
	BEGIN
		SET @Range2Date=DATEADD(DAY,-@Range2,@CurrDate)
	END
	IF(ISNULL(@Range3,0)<>0)
	BEGIN
		SET @Range3Date=DATEADD(DAY,-@Range3,@CurrDate)
	END
	IF(ISNULL(@Range4,0)<>0)
	BEGIN
		SET @Range4Date=DATEADD(DAY,-@Range4,@CurrDate)
	END		
	IF(ISNULL(@Range5,0)<>0)
	BEGIN
		SET @Range5Date=DATEADD(DAY,-@Range5,@CurrDate)
	END	
	IF(ISNULL(@Range6,0)<>0)
	BEGIN
		SET @Range6Date=DATEADD(DAY,-@Range6,@CurrDate)
	END	
	IF(ISNULL(@Range7,0)<>0)
	BEGIN
		SET @Range7Date=DATEADD(DAY,-@Range7,@CurrDate)
	END	
	IF(ISNULL(@Range8,0)<>0)
	BEGIN
		SET @Range8Date=DATEADD(DAY,-@Range8,@CurrDate)
	END	
	IF(ISNULL(@Range9,0)<>0)
	BEGIN
		SET @Range9Date=DATEADD(DAY,-@Range9,@CurrDate)
	END	
	IF(ISNULL(@Range10,0)<>0)
	BEGIN
		SET @Range10Date=DATEADD(DAY,-@Range10,@CurrDate)
	END	
	IF(ISNULL(@Range11,0)<>0)
	BEGIN
		SET @Range11Date=DATEADD(DAY,-@Range11,@CurrDate)
	END		
	
	DECLARE @Sql varchar(max)
	DECLARE @Where  varchar(8000)
	DECLARE @PartSuffix varchar(5)
	DECLARE @PagePara varchar(8000)
	DECLARE @TmpForLoop int
	SELECT @Sql='',@TmpForLoop=0,@Where=''
	DECLARE @LocationIds table(Id int identity(1,1),PartSuffix varchar(5))
	
	---如果有输入的库位则只查询输入库位的表，否则全部表拼接查询
	IF ISNULL(@Locations,'')='' 
	BEGIN
		INSERT INTO @LocationIds(PartSuffix)
		SELECT DISTINCT(PartSuffix) FROM MD_Location WHERE PartSuffix IS NOT NULL OR PartSuffix<>''
	END
	ELSE
	BEGIN
		SET @Where=' AND det.Location in('''+replace(@Locations,',',''',''')+''')'
	    SET @sql='SELECT DISTINCT PartSuffix FROM MD_Location WHERE Code in ('''+replace(@Locations,',',''',''')+''')'
		INSERT INTO @LocationIds(PartSuffix)
		EXEC(@sql)
	END
	
	---查询出数据时需要的条件
	-----物料
	IF ISNULL(@Items,'')<>'' 
	BEGIN
		IF ISNULL(@Where,'')=''
		BEGIN
			SET @Where=' AND det.Item IN ('''+replace(@Items,',',''',''')+''')'
		END
		ELSE
		BEGIN
			SET @Where=@Where+' AND det.Item IN ('''+replace(@Items,',',''',''')+''')'
		END
	END
	--PRINT @Where
	--select * from @LocationIds
	---排序条件
	IF ISNULL(@SortDesc,'')=''
	BEGIN
		SET @SortDesc=' ORDER BY Location ASC'
	END
		
	----查询出结果时需要的条件
	IF @Page>0
	BEGIN
		SET @PagePara='WHERE rowid BETWEEN '+cast(@PageSize*(@Page-1) as varchar(50))+' AND '++cast(@PageSize*(@Page) as varchar(50))
	END
	
	CREATE TABLE #TempResult
	(
		rowid int,
		Location varchar(50), 
		Item varchar(50), 
		Range0 decimal(18,8), 
		Range1 decimal(18,8), 
		Range2 decimal(18,8), 
		Range3 decimal(18,8),  
		Range4 decimal(18,8), 
		Range5 decimal(18,8),
		Range6 decimal(18,8),
		Range7 decimal(18,8),
		Range8 decimal(18,8),
		Range9 decimal(18,8),
		Range10 decimal(18,8),
		Range11 decimal(18,8)
	)

	CREATE TABLE #TempInternal
	(
		Location varchar(50), 
		Item varchar(50), 
		Range0 decimal(18,8), 
		Range1 decimal(18,8), 
		Range2 decimal(18,8), 
		Range3 decimal(18,8),  
		Range4 decimal(18,8), 
		Range5 decimal(18,8),
		Range6 decimal(18,8),
		Range7 decimal(18,8),
		Range8 decimal(18,8),
		Range9 decimal(18,8),
		Range10 decimal(18,8),
		Range11 decimal(18,8)
	)
	DECLARE @MaxId int
	SELECT @MaxId = MAX(Id),@Sql='' FROM @LocationIds
	WHILE @TmpForLoop<@MaxId
	BEGIN
		SET @TmpForLoop=@TmpForLoop+1	
		SELECT @PartSuffix=PartSuffix FROM @LocationIds WHERE Id=@TmpForLoop
		PRINT @TmpForLoop	
		SET @Sql='SELECT det.Location, det.Item,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @CurrDate, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range1Date, 121)+''' THEN det.QTY END) AS Range0,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range1Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range2Date, 121)+''' THEN det.QTY END) AS Range1,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range2Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range3Date, 121)+''' THEN det.QTY END) AS Range2,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range3Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range4Date, 121)+''' THEN det.QTY END) AS Range3,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range4Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range5Date, 121)+''' THEN det.QTY END) AS Range4,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range5Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range6Date, 121)+''' THEN det.QTY END) AS Range5,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range6Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range7Date, 121)+''' THEN det.QTY END) AS Range6,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range7Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range8Date, 121)+''' THEN det.QTY END) AS Range7,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range8Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range9Date, 121)+''' THEN det.QTY END) AS Range8,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range9Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range10Date, 121)+''' THEN det.QTY END) AS Range9,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range10Date, 121)+''' AND CreateDate>'''+Convert(varchar(19), @Range11Date, 121)+''' THEN det.QTY END) AS Range10,
				SUM(CASE WHEN CreateDate<='''+Convert(varchar(19), @Range11Date, 121)+''' THEN det.QTY END) AS Range11
				FROM  INV_LocationLotDet_'+@PartSuffix+' AS det WHERE 1=1 '+@Where+' AND det.QTY<>0    
				GROUP BY det.Location, det.IsCS, det.Item'			
		--PRINT @Sql	
		INSERT INTO #TempInternal
		EXEC(@Sql)	
	END
	--PRINT @Sql
	
	IF @IsSummaryBySAPLoc=1
	BEGIN
		SET @sql = 'select row_number() over('+@SortDesc+') as rowid,* FROM (SELECT l.SAPLocation as Location, t.Item, SUM(t.Range0) AS Range0, SUM(t.Range1) AS Range1, SUM(t.Range2) AS Range2, 
		SUM(t.Range3) AS Range3, SUM(t.Range4) AS Range4, SUM(t.Range5) AS Range5, SUM(t.Range6) AS Range6,SUM(t.Range7) AS Range7, SUM(t.Range8) AS Range8, SUM(t.Range9) AS Range9, SUM(t.Range10) AS Range10, SUM(t.Range11) AS Range11 FROM #TempInternal as t
			INNER JOIN MD_Location l ON t.Location=l.Code
			WHERE Range0<>0 OR Range1<>0 OR Range2<>0 OR Range3<>0 OR Range4<>0 OR Range5<>0 OR Range6<>0 OR Range7<>0 OR Range8<>0
			OR Range9<>0 OR Range10<>0 OR Range11<>0
			GROUP BY l.SAPLocation, t.Item) as LocTranDet'
	
		insert into #TempResult 
		exec(@sql)	
			
		select count(1) from #TempResult
		exec('select top('+@PageSize+')  Location, Item, 
		Range0, Range1, Range2, Range3, Range4, Range5, Range6,Range7, Range8, Range9, Range10, Range11 from #TempResult '+@PagePara)
	END
	ELSE
	BEGIN
		IF @SummaryLevel=0
		BEGIN
			--不汇总
			SET @sql = 'select row_number() over('+@SortDesc+') as rowid, Location, Item, 
			Range0, Range1, Range2, Range3, Range4, Range5, Range6,Range7, Range8, Range9, Range10, Range11 from #TempInternal as det
			WHERE Range0<>0 OR Range1<>0 OR Range2<>0 OR Range3<>0 OR Range4<>0 OR Range5<>0 OR Range6<>0 OR Range7<>0 OR Range8<>0
			OR Range9<>0 OR Range10<>0 OR Range11<>0'
			
			insert into #TempResult 
			exec(@sql)	
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+') Location, Item, 
			Range0, Range1, Range2, Range3, Range4, Range5, Range6,Range7, Range8, Range9, Range10, Range11 from #TempResult '+@PagePara) 
		END
		ELSE IF @SummaryLevel=1
		BEGIN
			--汇总到区域
			SET @sql = 'select row_number() over('+@SortDesc+') as rowid,* FROM (SELECT r.Code as Location, t.Item, SUM(t.Range0) AS Range0, SUM(t.Range1) AS Range1, SUM(t.Range2) AS Range2, 
			SUM(t.Range3) AS Range3, SUM(t.Range4) AS Range4, SUM(t.Range5) AS Range5, SUM(t.Range6) AS Range6,SUM(t.Range7) AS Range7, SUM(t.Range8) AS Range8, SUM(t.Range9) AS Range9, SUM(t.Range10) AS Range10, SUM(t.Range11) AS Range11 FROM #TempInternal as t
				INNER JOIN MD_Location l ON t.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region
				WHERE Range0<>0 OR Range1<>0 OR Range2<>0 OR Range3<>0 OR Range4<>0 OR Range5<>0 OR Range6<>0 OR Range7<>0 OR Range8<>0
				OR Range9<>0 OR Range10<>0 OR Range11<>0
				GROUP BY r.Code, t.Item) as LocTranDet'
		
			insert into #TempResult 
			exec(@sql)	
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+')  Location, Item, 
			Range0, Range1, Range2, Range3, Range4, Range5, Range6,Range7, Range8, Range9, Range10, Range11 from #TempResult '+@PagePara) 
		END
		ELSE IF @SummaryLevel=2
		BEGIN
			--汇总到车间
			SET @sql = 'select row_number() over('+@SortDesc+') as rowid,* FROM (SELECT r.Workshop as Location, t.Item, SUM(t.Range0) AS Range0, SUM(t.Range1) AS Range1, SUM(t.Range2) AS Range2, 
			SUM(t.Range3) AS Range3, SUM(t.Range4) AS Range4, SUM(t.Range5) AS Range5, SUM(t.Range6) AS Range6,SUM(t.Range7) AS Range7, SUM(t.Range8) AS Range8, SUM(t.Range9) AS Range9, SUM(t.Range10) AS Range10, SUM(t.Range11) AS Range11 FROM #TempInternal as t
				INNER JOIN MD_Location l ON t.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region
				WHERE Range0<>0 OR Range1<>0 OR Range2<>0 OR Range3<>0 OR Range4<>0 OR Range5<>0 OR Range6<>0 OR Range7<>0 OR Range8<>0
				OR Range9<>0 OR Range10<>0 OR Range11<>0
				GROUP BY r.Workshop, t.Item) as LocTranDet'
			
			insert into #TempResult 
			exec(@sql)	
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+') Location, Item, 
			Range0, Range1, Range2, Range3, Range4, Range5, Range6,Range7, Range8, Range9, Range10, Range11 from #TempResult '+@PagePara) 
		END
		ELSE IF @SummaryLevel=3
		BEGIN
			--汇总到工厂
			SET @sql = 'select row_number() over('+@SortDesc+') as rowid,* FROM (SELECT r.Plant as Location, t.Item, SUM(t.Range0) AS Range0, SUM(t.Range1) AS Range1, SUM(t.Range2) AS Range2, 
			SUM(t.Range3) AS Range3, SUM(t.Range4) AS Range4, SUM(t.Range5) AS Range5, SUM(t.Range6) AS Range6,SUM(t.Range7) AS Range7, SUM(t.Range8) AS Range8, SUM(t.Range9) AS Range9, SUM(t.Range10) AS Range10, SUM(t.Range11) AS Range11 FROM #TempInternal as t
				INNER JOIN MD_Location l ON t.Location=l.Code
				INNER JOIN MD_Region r ON r.Code=l.Region
				WHERE Range0<>0 OR Range1<>0 OR Range2<>0 OR Range3<>0 OR Range4<>0 OR Range5<>0 OR Range6<>0 OR Range7<>0 OR Range8<>0
				OR Range9<>0 OR Range10<>0 OR Range11<>0
				GROUP BY r.Plant, t.Item) as LocTranDet'
			
			insert into #TempResult 
			exec(@sql)	
				
			select count(1) from #TempResult
			exec('select top('+@PageSize+') Location, Item, 
			Range0, Range1, Range2, Range3, Range4, Range5, Range6,Range7, Range8, Range9, Range10, Range11 from #TempResult '+@PagePara) 
		END
	END	
END
GO


