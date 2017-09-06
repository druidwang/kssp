/****** Object:  StoredProcedure [dbo].[USP_GetDocNo_ASN]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_GetDocNo_ASN')
	DROP PROCEDURE USP_GetDocNo_ASN
CREATE PROCEDURE [dbo].[USP_GetDocNo_ASN]
(
	@OrderType tinyint,
	@OrderSubType tinyint,
	@QualityType tinyint,
	@PartyFrom varchar(50),
	@PartyTo varchar(50),
	@Dock varchar(100),
	@ASNNo varchar(100) output
)
AS
BEGIN
	SET NOCOUNT ON;
	/*
	declare @ASNNo varchar(100)
	exec USP_GetAsnNo 6,2,'RM','WIP','ABC',@ASNNo output
	select @ASNNo
	*/
	DECLARE @Code varchar(50)
	DECLARE @BlockSeq varchar(10)
	DECLARE @PreFixed char(2)
	DECLARE @YearCode varchar(6)
	DECLARE @MonthCode varchar(4)
	DECLARE @DayCode varchar(4)
	DECLARE @SeqLength tinyint
	DECLARE @SeqMin int
	DECLARE @NumCtrKey varchar(100)
	
	SELECT @BlockSeq = SUBSTRING(BlockSeq,2,LEN(BlockSeq)-1),@Code = Code,
		 @PreFixed = PreFixed, @YearCode = YearCode,
		 @MonthCode = MonthCode,@DayCode = DayCode,
		 @SeqLength = SeqLength,@SeqMin = SeqMin
	FROM dbo.SYS_SNRule 
	WHERE Code = @OrderType+1100
	
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
	WHERE A.Code = @OrderType+1100 AND B.IsChoosed = 1
	
	SET @ASNNo=@PreFixed	
	WHILE(@BlockSeq>0)
	BEGIN
		SET @CurrBlockSeq = SUBSTRING(@BlockSeq,1,1)
		PRINT @BlockSeq
		PRINT @CurrBlockSeq
		PRINT @ASNNo
		SET @BlockSeq = SUBSTRING(@BlockSeq,2,LEN(@BlockSeq)-1)
		
		IF @CurrBlockSeq = 2
		BEGIN
			SET @ASNNo = @ASNNo+dbo.FormatDate(@CurrentDate,@YearCode)
		END
		ELSE IF @CurrBlockSeq = 3
		BEGIN
			SET @ASNNo = @ASNNo+dbo.FormatDate(@CurrentDate,@MonthCode)
		END
		ELSE IF @CurrBlockSeq = 4
		BEGIN
			SET @ASNNo = @ASNNo+dbo.FormatDate(@CurrentDate,@DayCode)
		END
		ELSE IF @CurrBlockSeq = 5
		BEGIN
			--We will use '****' to replace the Seqence number for the number control key
			SET @ASNNo = @ASNNo+'****'
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
			--SET @ASNNo = @ASNNo+RIGHT('00000000000000000000'+CAST(@CurrSeq AS VARCHAR(20)),@SeqLength)
		END	
		ELSE IF @CurrBlockSeq = 6
		BEGIN
			SELECT @MaxNO = MAX(ROWNO) FROM #Temp
			WHILE(@Temp<=@MaxNO)
			BEGIN
				SELECT @CurrRefColumn = Field FROM #Temp WHERE ROWNO = @Temp
				IF(@CurrRefColumn = 'OrderType')
				BEGIN
					SET @ASNNo = @ASNNo + CAST(@OrderType AS VARCHAR(10))
				END	
				ELSE IF(@CurrRefColumn = 'OrderSubType')
				BEGIN
					SET @ASNNo = @ASNNo + CAST(@OrderSubType AS VARCHAR(10))
				END			
				ELSE IF(@CurrRefColumn = 'QualityType')
				BEGIN
					SET @ASNNo = @ASNNo + CAST(@QualityType AS VARCHAR(10))
				END
				ELSE IF(@CurrRefColumn = 'PartyFrom')
				BEGIN
					SET @ASNNo = @ASNNo + ISNULL(@PartyFrom,'')
				END
				ELSE IF(@CurrRefColumn = 'PartyTo')
				BEGIN
					SET @ASNNo = @ASNNo + ISNULL(@PartyTo,'')
				END	
				ELSE IF(@CurrRefColumn = 'Dock')
				BEGIN
					SET @ASNNo = @ASNNo + ISNULL(@Dock,'')
				END					
				SET @Temp = @Temp + 1														
			END
			
		END	
	END  
	
	PRINT @ASNNo
	
	IF CHARINDEX('****',@ASNNo)>0
	BEGIN
		PRINT @ASNNo
		SET @NumCtrKey = @ASNNo
		BEGIN TRAN
			IF NOT EXISTS(SELECT * FROM dbo.SYS_NumCtrl WHERE Code = @NumCtrKey)
			BEGIN
				INSERT INTO SYS_NumCtrl(Code,IntValue)
				VALUES(@NumCtrKey,ISNULL(@SeqMin,1))
				SET @CurrSeq = ISNULL(@SeqMin,1)
			END
			ELSE
			BEGIN
				SELECT @CurrSeq = IntValue FROM dbo.SYS_NumCtrl WHERE Code = @NumCtrKey
			END
			UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+1 WHERE Code = @NumCtrKey
		COMMIT		
		SET @ASNNo = REPLACE(@ASNNo,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq AS VARCHAR(20)),@SeqLength))		
	END
	
END
GO