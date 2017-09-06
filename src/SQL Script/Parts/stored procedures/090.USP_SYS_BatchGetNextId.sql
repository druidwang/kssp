SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_SYS_BatchGetNextId')
BEGIN
	DROP PROCEDURE USP_SYS_BatchGetNextId
END
GO

CREATE PROCEDURE dbo.USP_SYS_BatchGetNextId
	@TablePrefix varchar(50), 
	@BatchSize int, 
	@NextId Bigint OUTPUT 
AS 
BEGIN
	BEGIN TRAN 
		IF EXISTS (SELECT * FROM SYS_TabIdSeq WITH (UPDLOCK,SERIALIZABLE) WHERE TabNm=@TablePrefix) 
		BEGIN 
			SELECT @NextId=Id+@BatchSize FROM SYS_TabIdSeq WHERE TabNm=@TablePrefix 
			UPDATE SYS_TabIdSeq SET Id=Id+@BatchSize WHERE TabNm=@TablePrefix 
		END 
		ELSE 
		BEGIN 
			INSERT INTO SYS_TabIdSeq(TabNm,Id) 
			VALUES(@TablePrefix,@BatchSize) 
			SET @NextId=@BatchSize 
		END 
	COMMIT TRAN
END
GO