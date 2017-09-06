/****** Object:  StoredProcedure [dbo].[USP_Split_LocationLotDet_Delete]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_LocationLotDet_Delete')
	DROP PROCEDURE USP_Split_LocationLotDet_Delete
CREATE PROCEDURE [dbo].[USP_Split_LocationLotDet_Delete]
(
	@Id int,
	@Version int
)
AS 
BEGIN
	DECLARE @Location tinyint
	SELECT @Location=Location From VIEW_LocationLotDet WHERE Id=@Id AND [Version]=@Version
	
	IF @Location='RM'
	BEGIN
		DELETE FROM INV_LocationLotDet_1 WHERE Id=@Id AND [Version]=@Version
	END
	ELSE
	BEGIN
		DELETE FROM INV_LocationLotDet_0 WHERE Id=@Id AND [Version]=@Version
	END
    
    SELECT SCOPE_IDENTITY()
END
GO
