/****** Object:  StoredProcedure [dbo].[USP_Split_IpLocationDet_Delete]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_IpLocationDet_Delete')
	DROP PROCEDURE USP_Split_IpLocationDet_Delete
CREATE PROCEDURE [dbo].[USP_Split_IpLocationDet_Delete]
(
	@Id int,
	@Version int
)
AS
BEGIN
	DECLARE @OrderType tinyint
	SELECT @OrderType=OrderType From VIEW_IpLocationDet WHERE Id=@Id AND Version=@Version
	
	IF @OrderType=1
	BEGIN
		DELETE FROM ORD_IpLocationDet_1 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=2
	BEGIN
		DELETE FROM ORD_IpLocationDet_2 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=3
	BEGIN
		DELETE FROM ORD_IpLocationDet_3 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=4
	BEGIN
		DELETE FROM ORD_IpLocationDet_4 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=5
	BEGIN
		DELETE FROM ORD_IpLocationDet_5 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=6
	BEGIN
		DELETE FROM ORD_IpLocationDet_6 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=7
	BEGIN
		DELETE FROM ORD_IpLocationDet_7 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=8
	BEGIN
		DELETE FROM ORD_IpLocationDet_7 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=0
	BEGIN
		DELETE FROM ORD_IpLocationDet_0 WHERE Id=@Id AND [Version]=@Version
	END
END
GO


