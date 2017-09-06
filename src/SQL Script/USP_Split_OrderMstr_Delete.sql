/****** Object:  StoredProcedure [dbo].[USP_Split_OrderMstr_Delete]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_OrderMstr_Delete')
	DROP PROCEDURE USP_Split_OrderMstr_Delete
CREATE procedure [dbo].[USP_Split_OrderMstr_Delete]
(
	@OrderNo varchar(4000),
	@Version int

)
AS
BEGIN
	DECLARE @OrderType tinyint
	SELECT @OrderType=[Type] From VIEW_OrderMstr WHERE OrderNo = @OrderNo AND [Version] = @Version
	
	IF @OrderType=1
	BEGIN
		DELETE FROM ORD_OrderMstr_1 WHERE OrderNo = @OrderNo AND [Version] = @Version
	END
	ELSE IF @OrderType=2
	BEGIN
		DELETE FROM ORD_OrderMstr_2 WHERE OrderNo = @OrderNo AND [Version] = @Version
	END
	ELSE IF @OrderType=3
	BEGIN
		DELETE FROM ORD_OrderMstr_3 WHERE OrderNo = @OrderNo AND [Version] = @Version
	END
	ELSE IF @OrderType=4
	BEGIN
		DELETE FROM ORD_OrderMstr_4 WHERE OrderNo = @OrderNo AND [Version] = @Version
	END
	ELSE IF @OrderType=5
	BEGIN
		DELETE FROM ORD_OrderMstr_5 WHERE OrderNo = @OrderNo AND [Version] = @Version
	END
	ELSE IF @OrderType=6
	BEGIN
		DELETE FROM ORD_OrderMstr_6 WHERE OrderNo = @OrderNo AND [Version] = @Version
	END
	ELSE IF @OrderType=7
	BEGIN
		DELETE FROM ORD_OrderMstr_7 WHERE OrderNo = @OrderNo AND [Version] = @Version
	END
	ELSE IF @OrderType=8
	BEGIN
		DELETE FROM ORD_OrderMstr_8 WHERE OrderNo = @OrderNo AND [Version] = @Version
	END
	ELSE IF @OrderType=0
	BEGIN
		DELETE FROM ORD_OrderMstr_0 WHERE OrderNo = @OrderNo AND [Version] = @Version
	END
	
	SELECT SCOPE_IDENTITY()
END
GO
