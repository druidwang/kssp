
/****** Object:  StoredProcedure [dbo].[USP_Busi_GetHistoryInv]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_GetHistoryInv')
	DROP PROCEDURE USP_Busi_GetHistoryInv
CREATE PROCEDURE [dbo].[USP_Busi_GetHistoryInv]
(
	@Location varchar(50),
	@Item varchar(4000),
	@HistoryDate datetime
)
AS
BEGIN
	
	select * from View_LocationDet
END
GO