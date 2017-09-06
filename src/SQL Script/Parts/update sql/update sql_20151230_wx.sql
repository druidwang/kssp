insert into SYS_SNRule(code,Desc1,PreFixed,YearCode,MonthCode,DayCode,BlockSeq,SeqLength,SeqBaseType,SeqMin)
values('2016','Delivery_BarCode','DC',null,null,null,15,8,null,1)
go

insert into SYS_SNRuleExt(Code,Field,FieldSeq,IsChoosed) values ('2016','Item',1,1)
go

insert into SYS_SNRuleExt(Code,Field,FieldSeq,IsChoosed) values ('2016','Qty',1000,0)
go

insert into SYS_SNRuleExt(Code,Field,FieldSeq,IsChoosed) values ('2016','UC',1000,0)
go

insert into SYS_SNRuleExt(Code,Field,FieldSeq,IsChoosed) values ('2016','IsOdd',1000,0)
go


insert into SYS_EntityPreference(Id,Seq,Value,Desc1,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
values(90114,120,'DeliveryBarCode.xls','默认配送模板','2603','用户 超级',GETDATE(),'2603','用户 超级',GETDATE())
go

/****** Object:  StoredProcedure [dbo].[USP_GetDocNo_DBC]   Script Date: 2015/12/30 12:51:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[USP_GetDocNo_DBC]
(
	@Item varchar(50),
	@Qty decimal(18,8),
	@UC decimal(18,8)
)
AS
BEGIN
	SET NOCOUNT ON;
    SELECT @Item=LTRIM(RTRIM(@Item))
	DECLARE @BarCode varchar(1000)
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
	
	CREATE TABLE #BarCodes(BarCode varchar(1000),Cnt decimal(18,8))
	
	SELECT @BlockSeq = SUBSTRING(BlockSeq,2,LEN(BlockSeq)-1),@Code = Code,
		 @PreFixed = PreFixed, @YearCode = YearCode,
		 @MonthCode = MonthCode,@DayCode = DayCode,
		 @SeqLength = SeqLength,@SeqMin = SeqMin
	FROM dbo.SYS_SNRule 
	WHERE Code = 2016
	
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
	WHERE A.Code = 2016 AND B.IsChoosed = 1
	
	SET @BarCode=@PreFixed	
	WHILE(@BlockSeq>0)
	BEGIN
		SET @CurrBlockSeq = SUBSTRING(@BlockSeq,1,1)
		--PRINT @BlockSeq
		--PRINT @CurrBlockSeq
		--PRINT @BarCode
		SET @BlockSeq = SUBSTRING(@BlockSeq,2,LEN(@BlockSeq)-1)
		
		IF @CurrBlockSeq = 2
		BEGIN
			SET @BarCode = @BarCode+dbo.FormatDate(@CurrentDate,@YearCode)
		END
		ELSE IF @CurrBlockSeq = 3
		BEGIN
			SET @BarCode = @BarCode+dbo.FormatDate(@CurrentDate,@MonthCode)
		END
		ELSE IF @CurrBlockSeq = 4
		BEGIN
			SET @BarCode = @BarCode+dbo.FormatDate(@CurrentDate,@DayCode)
		END
		ELSE IF @CurrBlockSeq = 5
		BEGIN
			SET @BarCode = @BarCode+'****'
		END	
		ELSE IF @CurrBlockSeq = 6
		BEGIN
			SELECT @MaxNO = MAX(ROWNO) FROM #Temp
			WHILE(@Temp<=@MaxNO)
			BEGIN
			
				SELECT @CurrRefColumn = Field FROM #Temp WHERE ROWNO = @Temp
				IF(@CurrRefColumn = 'Item')
				BEGIN
					SET @BarCode = @BarCode + ISNULL(@Item,'')
				END
				ELSE IF(@CurrRefColumn = 'Qty')
				BEGIN
					SET @BarCode = @BarCode + CAST(FLOOR(@Qty) AS VARCHAR(50))
				END
				ELSE IF(@CurrRefColumn = 'UC')
				BEGIN
					SET @BarCode = @BarCode + CAST(FLOOR(@UC) AS VARCHAR(50)) 
				END
				ELSE IF(@CurrRefColumn = 'IsOdd')
				BEGIN
					SET @BarCode = @BarCode + '||||'
					SET @IsOdd = 1
				END				
				SET @Temp = @Temp + 1														
			END
		END	
	END  
	
	PRINT @BarCode
	
	IF @IsOdd=1
	BEGIN
		BEGIN TRAN
			SET @NumCtrKey = @BarCode
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
					--PRINT @BarCode
					SET @Count=@Count-1
					INSERT INTO #BarCodes(BarCode,Cnt) values(REPLACE(REPLACE(@BarCode,'||||','1'),'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+@Count AS VARCHAR(20)),@SeqLength)),@UC)	
					--SET @Count=@Count-1
				END
				UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+CAST(@QTY/@UC as int) WHERE Code = @NumCtrKey
			END
			ELSE
			BEGIN
				SET @Count=CAST(@QTY/@UC as int)
				WHILE(@Count>0)
				BEGIN
					--PRINT @BarCode
					PRINT @Count
					SET @Count=@Count-1
					INSERT INTO #BarCodes(BarCode,Cnt) values(REPLACE(REPLACE(@BarCode,'||||','1'),'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+@Count AS VARCHAR(20)),@SeqLength)),@UC)	
					--SET @Count=@Count-1
				END
				INSERT INTO #BarCodes(BarCode,Cnt) values(REPLACE(REPLACE(@BarCode,'||||','0'),'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+CAST(@QTY/@UC as int) AS VARCHAR(20)),@SeqLength)), @QTY % @UC )	
				UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+CEILING(@QTY/@UC) WHERE Code = @NumCtrKey
				
			END
		COMMIT	
	END
	ELSE
	BEGIN
		BEGIN TRAN
			SET @NumCtrKey = @BarCode
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
					PRINT @BarCode
					PRINT @Count
					SET @Count=@Count-1
					INSERT INTO #BarCodes(BarCode,Cnt) values(REPLACE(@BarCode,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+@Count AS VARCHAR(20)),@SeqLength)),@UC)
					--SET @Count=@Count-1
				END
				UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+CAST(@QTY/@UC as int) WHERE Code = @NumCtrKey
			END
			ELSE
			BEGIN
				SET @Count=CAST(@QTY/@UC as int)
				WHILE(@Count>0)
				BEGIN
					PRINT @BarCode
					PRINT @Count
					SET @Count=@Count-1
					INSERT INTO #BarCodes(BarCode,Cnt) values(REPLACE(@BarCode,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+@Count AS VARCHAR(20)),@SeqLength)),@UC)
					--SET @Count=@Count-1
				END
				INSERT INTO #BarCodes(BarCode,Cnt) values(REPLACE(@BarCode,'****',RIGHT('00000000000000000000'+CAST(@CurrSeq+CAST(@QTY/@UC as int) AS VARCHAR(20)),@SeqLength)), @QTY % @UC )	
				UPDATE dbo.SYS_NumCtrl SET IntValue = IntValue+CEILING(@QTY/@UC) WHERE Code = @NumCtrKey
				
			END
		COMMIT	
						
	END
	
	SELECT * FROM #BarCodes ORDER BY BarCode
END
