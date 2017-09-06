

/****** Object:  StoredProcedure [dbo].[USP_Busi_GetHuHistoryInv]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_GetHuHistoryInv')
	DROP PROCEDURE USP_Busi_GetHuHistoryInv
CREATE PROCEDURE [dbo].[USP_Busi_GetHuHistoryInv]
(
	@Location varchar(50),
	@Item varchar(4000),
	@HistoryDate datetime
)
AS
BEGIN
	
	select * from View_LocationLotDet
END
GO