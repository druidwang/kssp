/****** Object:  StoredProcedure [dbo].[USP_GetDocNo_HUID]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_GetDocNo_HUID')
	DROP PROCEDURE USP_GetDocNo_HUID
CREATE PROCEDURE [dbo].[USP_GetDocNo_HUID]
(
	@LotNo varchar(50),
	@Item varchar(50),
	@Qty decimal(18,8),
	@UC decimal(18,8),
	@ManufactureParty varchar(50)
	--@HuId varchar(1000) output
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @HuId varchar(1000)
	DECLARE @Code varchar(50)
	DECLARE @BlockSeq varchar(10)
	DECLARE @PreFixed char(2)
	DECLARE @YearCode varchar(6)
	DECLARE @MonthCode varchar(4)
	DECLARE @DayCode varchar(4)
	DECLARE @SeqLength tinyint
	DECLARE @SeqMin int
	DECLARE @NumCtrKey varchar(1000)
	DECLARE @IsOdd bit
	DECLARE @Count int
	
	CREATE TABLE #HuIds(HuId varchar(1000),Cnt decimal(18,8))
	
	SELECT @BlockSeq = SUBSTRING(BlockSeq,2,LEN(BlockSeq)-1),@Code = Code,
		 @PreFixed = PreFixed, @YearCode = YearCode,
		 @MonthCode = MonthCode,@DayCode = DayCode,
		 @SeqLength = SeqLength,@SeqMin = SeqMin
	FROM dbo.SYS_SNRule 
	WHERE Code = 2001
	
	DECLARE @CurrSeq int
	DECLARE @CurrBlockSeq char(1)
	DECLARE @CurrRefColumn varchar(50)
	DECLARE @Temp int
	SET @Temp = 1
	DECLARE @MaxNO int
	DECLARE @CurrentDate datetime	
	SET @CurrentDate = GETDATE()
	
	SELECT ROWNO=ROW_NUMBER() OVER (ORDER BY B.FieldSeq),B.* INTO #TEMP FROM dbo.SYS_SNRule A
	LEFT JOIN SYS_SNRuleExt B
		ON A.CODE=B.CODE
	WHERE A.Code = 2001 AND B.IsChoosed = 1
	
	SET @HuId=@PreFixed	
	WHILE(@BlockSeq>0)
	BEGIN
		SET @CurrBlockSeq = SUBSTRING(@BlockSeq,1,1)
		--PRINT @BlockSeq
		--PRINT @CurrBlockSeq
		--PRINT @HuId
		SET @BlockSeq = SUBSTRING(@BlockSeq,2,LEN(@BlockSeq)-1)
		
		IF @CurrBlockSeq = 2
		BEGIN
			SET @HuId = @HuId+dbo.FormatDate(@CurrentDate,@YearCode)
		END
		ELSE IF @CurrBlockSeq = 3
		BEGIN
			SET @HuId = @HuId+dbo.FormatDate(@CurrentDate,@MonthCode)
		END
		ELSE IF @CurrBlockSeq = 4
		BEGIN
			SET @HuId = @HuId+dbo.FormatDate(@CurrentDate,@DayCode)
		END
		ELSE IF @CurrBlockSeq = 5
		BEGIN
			--We will use '****' to replace the Seqence number for the number control key
			SET @HuId = @HuId+'****'
			--BEGIN TRAN
			--	IF NOT EXISTS(SELECT * FROM dbo.SYS_NumCtrl WHERE Code = @Code)
			--	BEGIN
			--		INSERT INTO SYS_NumCtrl(Code,IntValue)
			--		VALUES(@Code,@SeqMin)
			--		SET @CurrSeq = @SeqMin
			--	END
			--	ELSE
			--	BEGIN
			--		SELECT @CurrSeq = IntValue FROM dbo.SYS_NumCtrl WHERE Code = @Code
			--	END
			--	UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+1 WHERE Code = @Code
			--COMMIT		
			--SET @HuId = @HuId+RIGHT('00000000000000000000'+CAST(@CurrSeq AS VARCHAR(20)),@SeqLength)
		END	
		ELSE IF @CurrBlockSeq = 6
		BEGIN
			SELECT @MaxNO = MAX(ROWNO) FROM #Temp
			WHILE(@Temp<=@MaxNO)
			BEGIN
			
				SELECT @CurrRefColumn = Field FROM #Temp WHERE ROWNO = @Temp
				IF(@CurrRefColumn = 'LotNo')
				BEGIN
					SET @HuId = @HuId + ISNULL(@LotNo,'')
				END
				ELSE IF(@CurrRefColumn = 'Item')
				BEGIN
					SET @HuId = @HuId + ISNULL(@Item,'')
				END
				ELSE IF(@CurrRefColumn = 'Qty')
				BEGIN
					SET @HuId = @HuId + CAST(FLOOR(@Qty) AS VARCHAR(50))
				END
				ELSE IF(@CurrRefColumn = 'UC')
				BEGIN
					SET @HuId = @HuId + CAST(FLOOR(@UC) AS VARCHAR(50)) 
				END
				ELSE IF(@CurrRefColumn = 'ManufactureParty')
				BEGIN
					SET @HuId = @HuId + ISNULL(@ManufactureParty,'')
				END
				ELSE IF(@CurrRefColumn = 'IsOdd')
				BEGIN
					SET @HuId = @HuId + '||||'
					SET @IsOdd = 1
				END				
				SET @Temp = @Temp + 1														
			END
		END	
	END  
	
	PRINT @HuId
	
	IF @IsOdd=1
	BEGIN
		BEGIN TRAN
			SET @NumCtrKey = @HuId
			IF NOT EXISTS(SELECT * FROM dbo.SYS_NumCtrl WHERE Code = @NumCtrKey)
			BEGIN
				INSERT INTO SYS_NumCtrl(Code,IntValue)
				VALUES(@NumCtrKey,ISNULL(@SeqMin,0))
				SET @CurrSeq = ISNULL(@SeqMin,0)
			END
			ELSE
			BEGIN
				SELECT @CurrSeq = IntValue FROM dbo.SYS_NumCtrl WHERE Code = @NumCtrKey
			END		
			IF @QTY%@UC=0
			BEGIN
				SET @Count=CAST(@QTY/@UC as int)
				WHILE(@Count>0)
				BEGIN
					--PRINT @HuId
					SET @Count=@Count-1
					INSERT INTO #HuIds(HuId,Cnt) values(REPLACE(REPLACE(@HuId,'||||','1'),'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+@Count AS VARCHAR(20)),@SeqLength)),@UC)	
					--SET @Count=@Count-1
				END
				UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+CAST(@QTY/@UC as int) WHERE Code = @NumCtrKey
			END
			ELSE
			BEGIN
				SET @Count=CAST(@QTY/@UC as int)
				WHILE(@Count>0)
				BEGIN
					--PRINT @HuId
					PRINT @Count
					SET @Count=@Count-1
					INSERT INTO #HuIds(HuId,Cnt) values(REPLACE(REPLACE(@HuId,'||||','1'),'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+@Count AS VARCHAR(20)),@SeqLength)),@UC)	
					--SET @Count=@Count-1
				END
				INSERT INTO #HuIds(HuId,Cnt) values(REPLACE(REPLACE(@HuId,'||||','0'),'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+CAST(@QTY/@UC as int) AS VARCHAR(20)),@SeqLength)),CAST(@QTY as int)%CAST(@UC as int))	
				UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+CEILING(@QTY/@UC) WHERE Code = @NumCtrKey
				
			END
		COMMIT	
	END
	ELSE
	BEGIN
		BEGIN TRAN
			SET @NumCtrKey = @HuId
			IF NOT EXISTS(SELECT * FROM dbo.SYS_NumCtrl WHERE Code = @NumCtrKey)
			BEGIN
				INSERT INTO SYS_NumCtrl(Code,IntValue)
				VALUES(@NumCtrKey,ISNULL(@SeqMin,0))
				SET @CurrSeq = ISNULL(@SeqMin,0)
			END
			ELSE
			BEGIN
				SELECT @CurrSeq = IntValue FROM dbo.SYS_NumCtrl WHERE Code = @NumCtrKey
			END		
			IF @QTY%@UC=0
			BEGIN
				
				SET @Count=CAST(@QTY/@UC as int)
				WHILE(@Count>0)
				BEGIN
					PRINT @HuId
					PRINT @Count
					SET @Count=@Count-1
					INSERT INTO #HuIds(HuId,Cnt) values(REPLACE(@HuId,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+@Count AS VARCHAR(20)),@SeqLength)),@UC)
					--SET @Count=@Count-1
				END
				UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+CAST(@QTY/@UC as int) WHERE Code = @NumCtrKey
			END
			ELSE
			BEGIN
				SET @Count=CAST(@QTY/@UC as int)
				WHILE(@Count>0)
				BEGIN
					PRINT @HuId
					PRINT @Count
					SET @Count=@Count-1
					INSERT INTO #HuIds(HuId,Cnt) values(REPLACE(@HuId,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+@Count AS VARCHAR(20)),@SeqLength)),@UC)
					--SET @Count=@Count-1
				END
				INSERT INTO #HuIds(HuId,Cnt) values(REPLACE(@HuId,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+CAST(@QTY/@UC as int) AS VARCHAR(20)),@SeqLength)),CAST(@QTY as int)%CAST(@UC as int))	
				UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+CEILING(@QTY/@UC) WHERE Code = @NumCtrKey
				
			END
		COMMIT	
			
		--BEGIN TRAN
		--	SET @Count=CEILING(@QTY/@UC)
		--	SET @NumCtrKey = @HuId
		--	IF NOT EXISTS(SELECT * FROM dbo.SYS_NumCtrl WHERE Code = @NumCtrKey)
		--	BEGIN
		--		INSERT INTO SYS_NumCtrl(Code,IntValue)
		--		VALUES(@NumCtrKey,ISNULL(@SeqMin,0))
		--		SET @CurrSeq = ISNULL(@SeqMin,0)
		--	END
		--	ELSE
		--	BEGIN
		--		SELECT @CurrSeq = IntValue FROM dbo.SYS_NumCtrl WHERE Code = @NumCtrKey
		--	END
		--	--PRINT @HuId
		--	WHILE(@Count>0)
		--	BEGIN
		--		SET @Count=@Count-1
		--		IF @Count>1
		--		BEGIN
		--			INSERT INTO #HuIds(HuId) values(REPLACE(@HuId,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq AS VARCHAR(20)),@SeqLength)))
		--		END
		--		ELSE
		--		BEGIN
		--			INSERT INTO #HuIds(HuId) values(REPLACE(@HuId,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq AS VARCHAR(20)),@SeqLength)))
		--		END
		--	END
		--	UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+CEILING(@QTY/@UC) WHERE Code = @NumCtrKey
		--COMMIT				
	END
	
	SELECT * FROM #HuIds ORDER BY HuId
END
GO