/****** Object:  StoredProcedure [dbo].[USP_GetDocNo_ORD]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_GetDocNo_ORD')
	DROP PROCEDURE USP_GetDocNo_ORD
CREATE PROCEDURE [dbo].[USP_GetDocNo_ORD]
(
	@Flow varchar(50),
	@OrderStrategy tinyint,
	@Type tinyint,
	@SubType tinyint,
	@QualityType tinyint,
	@Priority tinyint,
	@PartyFrom varchar(50),
	@PartyTo varchar(50),
	@LocTo  varchar(50),
	@LocFrom  varchar(50),
	@Dock  varchar(50),
	@IsQuick bit,
	@OrderNo varchar(1000) output
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @Code varchar(50)
	DECLARE @BlockSeq varchar(10)
	DECLARE @PreFixed char(2)
	DECLARE @YearCode varchar(6)
	DECLARE @MonthCode varchar(4)
	DECLARE @DayCode varchar(4)
	DECLARE @SeqLength tinyint
	DECLARE @SeqMin int
	DECLARE @NumCtrKey varchar(1000)
	
	SELECT @BlockSeq = SUBSTRING(BlockSeq,2,LEN(BlockSeq)-1),@Code = Code,
		 @PreFixed = PreFixed, @YearCode = YearCode,
		 @MonthCode = MonthCode,@DayCode = DayCode,
		 @SeqLength = SeqLength,@SeqMin = SeqMin
	FROM dbo.SYS_SNRule 
	WHERE Code = @Type+1000
	
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
	WHERE A.Code = @Type+1000 AND B.IsChoosed = 1
	
	SET @ORDERNO=@PreFixed	
	WHILE(@BlockSeq>0)
	BEGIN
		SET @CurrBlockSeq = SUBSTRING(@BlockSeq,1,1)
		PRINT @BlockSeq
		PRINT @CurrBlockSeq
		PRINT @OrderNo
		SET @BlockSeq = SUBSTRING(@BlockSeq,2,LEN(@BlockSeq)-1)
		
		IF @CurrBlockSeq = 2
		BEGIN
			SET @OrderNo = @OrderNo+dbo.FormatDate(@CurrentDate,@YearCode)
		END
		ELSE IF @CurrBlockSeq = 3
		BEGIN
			SET @OrderNo = @OrderNo+dbo.FormatDate(@CurrentDate,@MonthCode)
		END
		ELSE IF @CurrBlockSeq = 4
		BEGIN
			SET @OrderNo = @OrderNo+dbo.FormatDate(@CurrentDate,@DayCode)
		END
		ELSE IF @CurrBlockSeq = 5
		BEGIN
			--We will use '****' to replace the Seqence number for the number control key
			SET @OrderNo = @OrderNo+'****'
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
			--SET @OrderNo = @OrderNo+RIGHT('00000000000000000000'+CAST(@CurrSeq AS VARCHAR(20)),@SeqLength)
		END	
		ELSE IF @CurrBlockSeq = 6
		BEGIN
			SELECT @MaxNO = MAX(ROWNO) FROM #Temp
			WHILE(@Temp<=@MaxNO)
			BEGIN
				SELECT @CurrRefColumn = Field FROM #Temp WHERE ROWNO = @Temp
				IF(@CurrRefColumn = 'FLOW')
				BEGIN
					SET @OrderNo = @OrderNo + ISNULL(@Flow,'')
				END
				ELSE IF(@CurrRefColumn = 'OrderStrategy')
				BEGIN
					SET @OrderNo = @OrderNo + CAST(@OrderStrategy AS VARCHAR(10))
				END
				ELSE IF(@CurrRefColumn = 'Type')
				BEGIN
					SET @OrderNo = @OrderNo + CAST(@Type AS VARCHAR(10))
				END
				ELSE IF(@CurrRefColumn = 'SubType')
				BEGIN
					SET @OrderNo = @OrderNo + CAST(@SubType AS VARCHAR(10))
				END
				ELSE IF(@CurrRefColumn = 'QualityType')
				BEGIN
					SET @OrderNo = @OrderNo + CAST(@QualityType AS VARCHAR(10))
				END
				ELSE IF(@CurrRefColumn = 'Priority')
				BEGIN
					SET @OrderNo = @OrderNo + CAST(@Priority AS VARCHAR(10))
				END
				ELSE IF(@CurrRefColumn = 'PartyFrom')
				BEGIN
					SET @OrderNo = @OrderNo + ISNULL(@PartyFrom,'')
				END
				ELSE IF(@CurrRefColumn = 'PartyTo')
				BEGIN
					SET @OrderNo = @OrderNo + ISNULL(@PartyTo,'')
				END	
				ELSE IF(@CurrRefColumn = 'LocTo')
				BEGIN
					SET @OrderNo = @OrderNo + ISNULL(@LocTo,'')
				END	
				ELSE IF(@CurrRefColumn = 'LocFrom')
				BEGIN
					SET @OrderNo = @OrderNo + ISNULL(@LocFrom,'')
				END	
				ELSE IF(@CurrRefColumn = 'Dock')
				BEGIN
					SET @OrderNo = @OrderNo + ISNULL(@Dock,'')
				END	
				ELSE IF(@CurrRefColumn = 'IsQuick')
				BEGIN
					SET @OrderNo = @OrderNo + CAST(@IsQuick AS VARCHAR(10))
				END	
				SET @Temp = @Temp + 1														
			END
		END	
	END  
	
	PRINT @OrderNo
	
	IF CHARINDEX('****',@OrderNo)>0
	BEGIN
		PRINT @OrderNo
		SET @NumCtrKey = @OrderNo
		BEGIN TRAN
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
			UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+1 WHERE Code = @NumCtrKey
		COMMIT		
		SET @OrderNo = REPLACE(@OrderNo,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq AS VARCHAR(20)),@SeqLength))		
	END
	
END
GO