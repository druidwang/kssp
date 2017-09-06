/****** Object:  StoredProcedure [dbo].[USP_Busi_GetActiveFlowDet]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_GetActiveFlowDet')
	DROP PROCEDURE USP_Busi_GetActiveFlowDet
CREATE PROCEDURE [dbo].[USP_Busi_GetActiveFlowDet]
(
	@EffectiveDate datetime
)
AS
BEGIN
	SELECT     det.Id, mstr.Code AS Flow, mstr.PartyFrom, mstr.PartyTo, str.Strategy, str.LeadTime, str.EmLeadTime, str.TimeUnit, str.WeekInterval, str.WinTime1, str.WinTime2, 
                      str.WinTime3, str.WinTime4, str.WinTime5, str.WinTime6, str.WinTime7, str.NextOrderTime, str.NextWinTime, mstr.Type, det.Item, det.Uom, det.BaseUom, det.UC, 
                      CASE WHEN isnull(det.LocFrom, '') = '' THEN mstr.LocFrom ELSE det.LocFrom END AS LocFrom, CASE WHEN isnull(det.LocTo, '') 
                      = '' THEN mstr.LocTo ELSE det.LocTo END AS LocTo, det.SafeStock, det.MaxStock, det.MinLotSize, det.OrderLotSize, det.RoundUpOpt, det.MrpWeight, det.MrpTotal, 
                      det.MrpTotalAdj, det.ExtraDmdSource, mstr.ExtraDmdSource
FROM         dbo.SCM_FlowDet AS det INNER JOIN
                      dbo.SCM_FlowMstr AS mstr ON det.Flow = mstr.Code INNER JOIN
                      dbo.SCM_FlowStrategy AS str ON mstr.Code = str.Flow
                      WHERE (det.StartDate is null or det.StartDate <= @EffectiveDate) and (det.EndDate is null or det.EndDate > @EffectiveDate)
END
GO
