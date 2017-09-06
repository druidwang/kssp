/****** Object:  StoredProcedure [dbo].[USP_Split_OrderDet_Delete]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_OrderDet_Delete')
	DROP PROCEDURE USP_Split_OrderDet_Delete
CREATE PROCEDURE [dbo].[USP_Split_OrderDet_Delete]
(
	@Id int,
	@Version int
)
AS 
BEGIN
	DECLARE @OrderType tinyint
	SELECT @OrderType=OrderType From VIEW_OrderDet WHERE Id=@Id AND Version=@Version
	
	IF @OrderType=1
	BEGIN
		DELETE FROM ORD_OrderDet_1 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=2
	BEGIN
		DELETE FROM ORD_OrderDet_2 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=3
	BEGIN
		DELETE FROM ORD_OrderDet_3 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=4
	BEGIN
		DELETE FROM ORD_OrderDet_4 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=5
	BEGIN
		DELETE FROM ORD_OrderDet_5 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=6
	BEGIN
		DELETE FROM ORD_OrderDet_6 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=7
	BEGIN
		DELETE FROM ORD_OrderDet_7 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=8
	BEGIN
		DELETE FROM ORD_OrderDet_8 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=0
	BEGIN
		DELETE FROM ORD_OrderDet_0 WHERE Id=@Id AND [Version]=@Version
	END
END
GO