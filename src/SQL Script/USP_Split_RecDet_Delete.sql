
/****** Object:  StoredProcedure [dbo].[USP_Split_RecDet_Delete]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_RecDet_Delete')
	DROP PROCEDURE USP_Split_RecDet_Delete
CREATE PROCEDURE [dbo].[USP_Split_RecDet_Delete]
(
	@Id int,
	@Version int
)
AS 
BEGIN
	DECLARE @OrderType tinyint
	SELECT @OrderType=OrderType From VIEW_RecDet WHERE Id=@Id AND Version=@Version
	
	IF @OrderType=1
	BEGIN
		DELETE FROM ORD_RecDet_1 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=2
	BEGIN
		DELETE FROM ORD_RecDet_2 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=3
	BEGIN
		DELETE FROM ORD_RecDet_3 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=4
	BEGIN
		DELETE FROM ORD_RecDet_4 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=5
	BEGIN
		DELETE FROM ORD_RecDet_5 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=6
	BEGIN
		DELETE FROM ORD_RecDet_6 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=7
	BEGIN
		DELETE FROM ORD_RecDet_7 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=8
	BEGIN
		DELETE FROM ORD_RecDet_8 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=0
	BEGIN
		DELETE FROM ORD_RecDet_0 WHERE Id=@Id AND [Version]=@Version
	END
    DELETE FROM ORD_RecDet WHERE Id=@Id AND [Version]=@Version
    SELECT SCOPE_IDENTITY()
END
GO