/****** Object:  StoredProcedure [dbo].[USP_Split_RecMstr_Delete]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_RecMstr_Delete')
	DROP PROCEDURE USP_Split_RecMstr_Delete
CREATE PROCEDURE [dbo].[USP_Split_RecMstr_Delete]
(
	@RecNo varchar(4000),
	@Version int

)
AS
BEGIN
	DECLARE @OrderType tinyint
	SELECT @OrderType=OrderType From VIEW_RecMstr WHERE RecNo=@RecNo AND Version=@Version
	
	IF @OrderType=1
	BEGIN
		DELETE FROM ORD_RecMstr_1 WHERE RecNo=@RecNo AND [Version]=@Version
	END
	ELSE IF @OrderType=2
	BEGIN
		DELETE FROM ORD_RecMstr_2 WHERE RecNo=@RecNo AND [Version]=@Version
	END
	ELSE IF @OrderType=3
	BEGIN
		DELETE FROM ORD_RecMstr_3 WHERE RecNo=@RecNo AND [Version]=@Version
	END
	ELSE IF @OrderType=4
	BEGIN
		DELETE FROM ORD_RecMstr_4 WHERE RecNo=@RecNo AND [Version]=@Version
	END
	ELSE IF @OrderType=5
	BEGIN
		DELETE FROM ORD_RecMstr_5 WHERE RecNo=@RecNo AND [Version]=@Version
	END
	ELSE IF @OrderType=6
	BEGIN
		DELETE FROM ORD_RecMstr_6 WHERE RecNo=@RecNo AND [Version]=@Version
	END
	ELSE IF @OrderType=7
	BEGIN
		DELETE FROM ORD_RecMstr_7 WHERE RecNo=@RecNo AND [Version]=@Version
	END
	ELSE IF @OrderType=8
	BEGIN
		DELETE FROM ORD_RecMstr_8 WHERE RecNo=@RecNo AND [Version]=@Version
	END
	ELSE IF @OrderType=0
	BEGIN
		DELETE FROM ORD_RecMstr_0 WHERE RecNo=@RecNo AND [Version]=@Version
	END
	
	SELECT SCOPE_IDENTITY()
END
GO