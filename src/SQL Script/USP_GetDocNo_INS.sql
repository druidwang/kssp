/****** Object:  StoredProcedure [dbo].[USP_GetDocNo_INS]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_GetDocNo_INS')
	DROP PROCEDURE USP_GetDocNo_INS
CREATE PROCEDURE [dbo].[USP_GetDocNo_INS]
(
	@Type tinyint,
	@Region varchar(50),
	@IsAtp bit,
	@InsNo varchar(100) output
)
AS
BEGIN
	SET NOCOUNT ON;
	/*
	declare @InsNo varchar(100)
	exec USP_GetInsNo 2,'RM',0,@InsNo output
	select @InsNo
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
	WHERE Code = 1601
	
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
	WHERE A.Code = 1601 AND B.IsChoosed = 1
	
	SET @InsNo=@PreFixed
	print @PreFixed
	WHILE(@BlockSeq>0)
	BEGIN
		SET @CurrBlockSeq = SUBSTRING(@BlockSeq,1,1)
		PRINT @BlockSeq
		PRINT @CurrBlockSeq
		PRINT @InsNo
		SET @BlockSeq = SUBSTRING(@BlockSeq,2,LEN(@BlockSeq)-1)
		
		IF @CurrBlockSeq = 2
		BEGIN
			SET @InsNo = @InsNo+dbo.FormatDate(@CurrentDate,@YearCode)
		END
		ELSE IF @CurrBlockSeq = 3
		BEGIN
			SET @InsNo = @InsNo+dbo.FormatDate(@CurrentDate,@MonthCode)
		END
		ELSE IF @CurrBlockSeq = 4
		BEGIN
			SET @InsNo = @InsNo+dbo.FormatDate(@CurrentDate,@DayCode)
		END
		ELSE IF @CurrBlockSeq = 5
		BEGIN
			--We will use '****' to replace the Seqence number for the number control key
			SET @InsNo = @InsNo+'****'
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
			--SET @InsNo = @InsNo+RIGHT('00000000000000000000'+CAST(@CurrSeq AS VARCHAR(20)),@SeqLength)
		END	
		ELSE IF @CurrBlockSeq = 6
		BEGIN
			SELECT @MaxNO = MAX(ROWNO) FROM #Temp
			WHILE(@Temp<=@MaxNO)
			BEGIN
				SELECT @CurrRefColumn = Field FROM #Temp WHERE ROWNO = @Temp
				IF(@CurrRefColumn = 'Type')
				BEGIN
					SET @InsNo = @InsNo + CAST(@Type AS VARCHAR(10))
				END
				ELSE IF(@CurrRefColumn = 'Region')
				BEGIN
					SET @InsNo = @InsNo + ISNULL(@Region,'')
				END
				ELSE IF(@CurrRefColumn = 'IsAtp')
				BEGIN
					SET @InsNo = @InsNo + CAST(@IsAtp AS VARCHAR(10))
				END
				SET @Temp = @Temp + 1														
			END
			
		END	
	END  
	
	PRINT @InsNo
	
	IF CHARINDEX('****',@InsNo)>0
	BEGIN
		PRINT @InsNo
		SET @NumCtrKey = @InsNo
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
		SET @InsNo = REPLACE(@InsNo,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq AS VARCHAR(20)),@SeqLength))		
	END
	
END
GO