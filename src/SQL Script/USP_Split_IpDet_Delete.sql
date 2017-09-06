
/****** Object:  StoredProcedure [dbo].[USP_Split_IpDet_Delete]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_IpDet_Delete')
	DROP PROCEDURE USP_Split_IpDet_Delete
CREATE PROCEDURE [dbo].[USP_Split_IpDet_Delete]
(
	@Id int,
	@Version int
)
as 
begin
	DECLARE @OrderType tinyint
	SELECT @OrderType=OrderType From VIEW_IpDet WHERE Id=@Id AND Version=@Version
	
	IF @OrderType=1
	BEGIN
		DELETE FROM ORD_IpDet_1 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=2
	BEGIN
		DELETE FROM ORD_IpDet_2 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=3
	BEGIN
		DELETE FROM ORD_IpDet_3 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=4
	BEGIN
		DELETE FROM ORD_IpDet_4 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=5
	BEGIN
		DELETE FROM ORD_IpDet_5 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=6
	BEGIN
		DELETE FROM ORD_IpDet_6 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=7
	BEGIN
		DELETE FROM ORD_IpDet_7 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=8
	BEGIN
		DELETE FROM ORD_IpDet_8 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE IF @OrderType=0
	BEGIN
		DELETE FROM ORD_IpDet_0 WHERE Id=@Id AND [Version]=@Version
	END
end
GO
