
/****** Object:  View [dbo].[VIEW_LocationLotDet]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='V' AND name='VIEW_LocationLotDet')
	DROP VIEW VIEW_LocationLotDet
CREATE VIEW [dbo].[VIEW_LocationLotDet]
  
AS
SELECT     dbo.INV_LocationLotDet_1.Id, dbo.INV_LocationLotDet_1.Location, dbo.INV_LocationLotDet_1.Bin, dbo.INV_LocationLotDet_1.Item, dbo.INV_LocationLotDet_1.HuId, 
                      dbo.INV_LocationLotDet_1.LotNo, dbo.INV_LocationLotDet_1.Qty, dbo.INV_LocationLotDet_1.IsCS, dbo.INV_LocationLotDet_1.PlanBill, dbo.INV_LocationLotDet_1.QualityType, 
                      dbo.INV_LocationLotDet_1.IsFreeze, dbo.INV_LocationLotDet_1.IsATP, dbo.INV_LocationLotDet_1.OccupyType, dbo.INV_LocationLotDet_1.OccupyRefNo, 
                      dbo.INV_LocationLotDet_1.CreateUser, dbo.INV_LocationLotDet_1.CreateUserNm, dbo.INV_LocationLotDet_1.CreateDate, dbo.INV_LocationLotDet_1.LastModifyUser, 
                      dbo.INV_LocationLotDet_1.LastModifyUserNm, dbo.INV_LocationLotDet_1.LastModifyDate, dbo.INV_LocationLotDet_1.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_1 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_1.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_1.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_1.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_1.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_1.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_0.Id, dbo.INV_LocationLotDet_0.Location, dbo.INV_LocationLotDet_0.Bin, dbo.INV_LocationLotDet_0.Item, dbo.INV_LocationLotDet_0.HuId, 
                      dbo.INV_LocationLotDet_0.LotNo, dbo.INV_LocationLotDet_0.Qty, dbo.INV_LocationLotDet_0.IsCS, dbo.INV_LocationLotDet_0.PlanBill, dbo.INV_LocationLotDet_0.QualityType, 
                      dbo.INV_LocationLotDet_0.IsFreeze, dbo.INV_LocationLotDet_0.IsATP, dbo.INV_LocationLotDet_0.OccupyType, dbo.INV_LocationLotDet_0.OccupyRefNo, 
                      dbo.INV_LocationLotDet_0.CreateUser, dbo.INV_LocationLotDet_0.CreateUserNm, dbo.INV_LocationLotDet_0.CreateDate, dbo.INV_LocationLotDet_0.LastModifyUser, 
                      dbo.INV_LocationLotDet_0.LastModifyUserNm, dbo.INV_LocationLotDet_0.LastModifyDate, dbo.INV_LocationLotDet_0.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_0 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_0.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_0.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_0.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_0.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_0.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_2.Id, dbo.INV_LocationLotDet_2.Location, dbo.INV_LocationLotDet_2.Bin, dbo.INV_LocationLotDet_2.Item, dbo.INV_LocationLotDet_2.HuId, 
                      dbo.INV_LocationLotDet_2.LotNo, dbo.INV_LocationLotDet_2.Qty, dbo.INV_LocationLotDet_2.IsCS, dbo.INV_LocationLotDet_2.PlanBill, dbo.INV_LocationLotDet_2.QualityType, 
                      dbo.INV_LocationLotDet_2.IsFreeze, dbo.INV_LocationLotDet_2.IsATP, dbo.INV_LocationLotDet_2.OccupyType, dbo.INV_LocationLotDet_2.OccupyRefNo, 
                      dbo.INV_LocationLotDet_2.CreateUser, dbo.INV_LocationLotDet_2.CreateUserNm, dbo.INV_LocationLotDet_2.CreateDate, dbo.INV_LocationLotDet_2.LastModifyUser, 
                      dbo.INV_LocationLotDet_2.LastModifyUserNm, dbo.INV_LocationLotDet_2.LastModifyDate, dbo.INV_LocationLotDet_2.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_2 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_2.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_2.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_2.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_2.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_2.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_3.Id, dbo.INV_LocationLotDet_3.Location, dbo.INV_LocationLotDet_3.Bin, dbo.INV_LocationLotDet_3.Item, dbo.INV_LocationLotDet_3.HuId, 
                      dbo.INV_LocationLotDet_3.LotNo, dbo.INV_LocationLotDet_3.Qty, dbo.INV_LocationLotDet_3.IsCS, dbo.INV_LocationLotDet_3.PlanBill, dbo.INV_LocationLotDet_3.QualityType, 
                      dbo.INV_LocationLotDet_3.IsFreeze, dbo.INV_LocationLotDet_3.IsATP, dbo.INV_LocationLotDet_3.OccupyType, dbo.INV_LocationLotDet_3.OccupyRefNo, 
                      dbo.INV_LocationLotDet_3.CreateUser, dbo.INV_LocationLotDet_3.CreateUserNm, dbo.INV_LocationLotDet_3.CreateDate, dbo.INV_LocationLotDet_3.LastModifyUser, 
                      dbo.INV_LocationLotDet_3.LastModifyUserNm, dbo.INV_LocationLotDet_3.LastModifyDate, dbo.INV_LocationLotDet_3.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_3 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_3.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_3.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_3.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_3.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_3.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_4.Id, dbo.INV_LocationLotDet_4.Location, dbo.INV_LocationLotDet_4.Bin, dbo.INV_LocationLotDet_4.Item, dbo.INV_LocationLotDet_4.HuId, 
                      dbo.INV_LocationLotDet_4.LotNo, dbo.INV_LocationLotDet_4.Qty, dbo.INV_LocationLotDet_4.IsCS, dbo.INV_LocationLotDet_4.PlanBill, dbo.INV_LocationLotDet_4.QualityType, 
                      dbo.INV_LocationLotDet_4.IsFreeze, dbo.INV_LocationLotDet_4.IsATP, dbo.INV_LocationLotDet_4.OccupyType, dbo.INV_LocationLotDet_4.OccupyRefNo, 
                      dbo.INV_LocationLotDet_4.CreateUser, dbo.INV_LocationLotDet_4.CreateUserNm, dbo.INV_LocationLotDet_4.CreateDate, dbo.INV_LocationLotDet_4.LastModifyUser, 
                      dbo.INV_LocationLotDet_4.LastModifyUserNm, dbo.INV_LocationLotDet_4.LastModifyDate, dbo.INV_LocationLotDet_4.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_4 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_4.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_4.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_4.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_4.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_4.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_5.Id, dbo.INV_LocationLotDet_5.Location, dbo.INV_LocationLotDet_5.Bin, dbo.INV_LocationLotDet_5.Item, dbo.INV_LocationLotDet_5.HuId, 
                      dbo.INV_LocationLotDet_5.LotNo, dbo.INV_LocationLotDet_5.Qty, dbo.INV_LocationLotDet_5.IsCS, dbo.INV_LocationLotDet_5.PlanBill, dbo.INV_LocationLotDet_5.QualityType, 
                      dbo.INV_LocationLotDet_5.IsFreeze, dbo.INV_LocationLotDet_5.IsATP, dbo.INV_LocationLotDet_5.OccupyType, dbo.INV_LocationLotDet_5.OccupyRefNo, 
                      dbo.INV_LocationLotDet_5.CreateUser, dbo.INV_LocationLotDet_5.CreateUserNm, dbo.INV_LocationLotDet_5.CreateDate, dbo.INV_LocationLotDet_5.LastModifyUser, 
                      dbo.INV_LocationLotDet_5.LastModifyUserNm, dbo.INV_LocationLotDet_5.LastModifyDate, dbo.INV_LocationLotDet_5.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_5 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_5.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_5.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_5.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_5.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_5.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_6.Id, dbo.INV_LocationLotDet_6.Location, dbo.INV_LocationLotDet_6.Bin, dbo.INV_LocationLotDet_6.Item, dbo.INV_LocationLotDet_6.HuId, 
                      dbo.INV_LocationLotDet_6.LotNo, dbo.INV_LocationLotDet_6.Qty, dbo.INV_LocationLotDet_6.IsCS, dbo.INV_LocationLotDet_6.PlanBill, dbo.INV_LocationLotDet_6.QualityType, 
                      dbo.INV_LocationLotDet_6.IsFreeze, dbo.INV_LocationLotDet_6.IsATP, dbo.INV_LocationLotDet_6.OccupyType, dbo.INV_LocationLotDet_6.OccupyRefNo, 
                      dbo.INV_LocationLotDet_6.CreateUser, dbo.INV_LocationLotDet_6.CreateUserNm, dbo.INV_LocationLotDet_6.CreateDate, dbo.INV_LocationLotDet_6.LastModifyUser, 
                      dbo.INV_LocationLotDet_6.LastModifyUserNm, dbo.INV_LocationLotDet_6.LastModifyDate, dbo.INV_LocationLotDet_6.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_6 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_6.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_6.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_6.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_6.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_6.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_7.Id, dbo.INV_LocationLotDet_7.Location, dbo.INV_LocationLotDet_7.Bin, dbo.INV_LocationLotDet_7.Item, dbo.INV_LocationLotDet_7.HuId, 
                      dbo.INV_LocationLotDet_7.LotNo, dbo.INV_LocationLotDet_7.Qty, dbo.INV_LocationLotDet_7.IsCS, dbo.INV_LocationLotDet_7.PlanBill, dbo.INV_LocationLotDet_7.QualityType, 
                      dbo.INV_LocationLotDet_7.IsFreeze, dbo.INV_LocationLotDet_7.IsATP, dbo.INV_LocationLotDet_7.OccupyType, dbo.INV_LocationLotDet_7.OccupyRefNo, 
                      dbo.INV_LocationLotDet_7.CreateUser, dbo.INV_LocationLotDet_7.CreateUserNm, dbo.INV_LocationLotDet_7.CreateDate, dbo.INV_LocationLotDet_7.LastModifyUser, 
                      dbo.INV_LocationLotDet_7.LastModifyUserNm, dbo.INV_LocationLotDet_7.LastModifyDate, dbo.INV_LocationLotDet_7.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_7 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_7.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_7.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_7.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_7.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_7.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_8.Id, dbo.INV_LocationLotDet_8.Location, dbo.INV_LocationLotDet_8.Bin, dbo.INV_LocationLotDet_8.Item, dbo.INV_LocationLotDet_8.HuId, 
                      dbo.INV_LocationLotDet_8.LotNo, dbo.INV_LocationLotDet_8.Qty, dbo.INV_LocationLotDet_8.IsCS, dbo.INV_LocationLotDet_8.PlanBill, dbo.INV_LocationLotDet_8.QualityType, 
                      dbo.INV_LocationLotDet_8.IsFreeze, dbo.INV_LocationLotDet_8.IsATP, dbo.INV_LocationLotDet_8.OccupyType, dbo.INV_LocationLotDet_8.OccupyRefNo, 
                      dbo.INV_LocationLotDet_8.CreateUser, dbo.INV_LocationLotDet_8.CreateUserNm, dbo.INV_LocationLotDet_8.CreateDate, dbo.INV_LocationLotDet_8.LastModifyUser, 
                      dbo.INV_LocationLotDet_8.LastModifyUserNm, dbo.INV_LocationLotDet_8.LastModifyDate, dbo.INV_LocationLotDet_8.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_8 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_8.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_8.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_8.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_8.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_8.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_9.Id, dbo.INV_LocationLotDet_9.Location, dbo.INV_LocationLotDet_9.Bin, dbo.INV_LocationLotDet_9.Item, dbo.INV_LocationLotDet_9.HuId, 
                      dbo.INV_LocationLotDet_9.LotNo, dbo.INV_LocationLotDet_9.Qty, dbo.INV_LocationLotDet_9.IsCS, dbo.INV_LocationLotDet_9.PlanBill, dbo.INV_LocationLotDet_9.QualityType, 
                      dbo.INV_LocationLotDet_9.IsFreeze, dbo.INV_LocationLotDet_9.IsATP, dbo.INV_LocationLotDet_9.OccupyType, dbo.INV_LocationLotDet_9.OccupyRefNo, 
                      dbo.INV_LocationLotDet_9.CreateUser, dbo.INV_LocationLotDet_9.CreateUserNm, dbo.INV_LocationLotDet_9.CreateDate, dbo.INV_LocationLotDet_9.LastModifyUser, 
                      dbo.INV_LocationLotDet_9.LastModifyUserNm, dbo.INV_LocationLotDet_9.LastModifyDate, dbo.INV_LocationLotDet_9.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_9 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_9.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_9.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_9.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_9.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_9.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_10.Id, dbo.INV_LocationLotDet_10.Location, dbo.INV_LocationLotDet_10.Bin, dbo.INV_LocationLotDet_10.Item, dbo.INV_LocationLotDet_10.HuId, 
                      dbo.INV_LocationLotDet_10.LotNo, dbo.INV_LocationLotDet_10.Qty, dbo.INV_LocationLotDet_10.IsCS, dbo.INV_LocationLotDet_10.PlanBill, dbo.INV_LocationLotDet_10.QualityType, 
                      dbo.INV_LocationLotDet_10.IsFreeze, dbo.INV_LocationLotDet_10.IsATP, dbo.INV_LocationLotDet_10.OccupyType, dbo.INV_LocationLotDet_10.OccupyRefNo, 
                      dbo.INV_LocationLotDet_10.CreateUser, dbo.INV_LocationLotDet_10.CreateUserNm, dbo.INV_LocationLotDet_10.CreateDate, dbo.INV_LocationLotDet_10.LastModifyUser, 
                      dbo.INV_LocationLotDet_10.LastModifyUserNm, dbo.INV_LocationLotDet_10.LastModifyDate, dbo.INV_LocationLotDet_10.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_10 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_10.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_10.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_10.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_10.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_10.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_11.Id, dbo.INV_LocationLotDet_11.Location, dbo.INV_LocationLotDet_11.Bin, dbo.INV_LocationLotDet_11.Item, dbo.INV_LocationLotDet_11.HuId, 
                      dbo.INV_LocationLotDet_11.LotNo, dbo.INV_LocationLotDet_11.Qty, dbo.INV_LocationLotDet_11.IsCS, dbo.INV_LocationLotDet_11.PlanBill, dbo.INV_LocationLotDet_11.QualityType, 
                      dbo.INV_LocationLotDet_11.IsFreeze, dbo.INV_LocationLotDet_11.IsATP, dbo.INV_LocationLotDet_11.OccupyType, dbo.INV_LocationLotDet_11.OccupyRefNo, 
                      dbo.INV_LocationLotDet_11.CreateUser, dbo.INV_LocationLotDet_11.CreateUserNm, dbo.INV_LocationLotDet_11.CreateDate, dbo.INV_LocationLotDet_11.LastModifyUser, 
                      dbo.INV_LocationLotDet_11.LastModifyUserNm, dbo.INV_LocationLotDet_11.LastModifyDate, dbo.INV_LocationLotDet_11.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_11 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_11.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_11.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_11.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_11.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_11.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_12.Id, dbo.INV_LocationLotDet_12.Location, dbo.INV_LocationLotDet_12.Bin, dbo.INV_LocationLotDet_12.Item, dbo.INV_LocationLotDet_12.HuId, 
                      dbo.INV_LocationLotDet_12.LotNo, dbo.INV_LocationLotDet_12.Qty, dbo.INV_LocationLotDet_12.IsCS, dbo.INV_LocationLotDet_12.PlanBill, dbo.INV_LocationLotDet_12.QualityType, 
                      dbo.INV_LocationLotDet_12.IsFreeze, dbo.INV_LocationLotDet_12.IsATP, dbo.INV_LocationLotDet_12.OccupyType, dbo.INV_LocationLotDet_12.OccupyRefNo, 
                      dbo.INV_LocationLotDet_12.CreateUser, dbo.INV_LocationLotDet_12.CreateUserNm, dbo.INV_LocationLotDet_12.CreateDate, dbo.INV_LocationLotDet_12.LastModifyUser, 
                      dbo.INV_LocationLotDet_12.LastModifyUserNm, dbo.INV_LocationLotDet_12.LastModifyDate, dbo.INV_LocationLotDet_12.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_12 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_12.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_12.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_12.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_12.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_12.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_13.Id, dbo.INV_LocationLotDet_13.Location, dbo.INV_LocationLotDet_13.Bin, dbo.INV_LocationLotDet_13.Item, dbo.INV_LocationLotDet_13.HuId, 
                      dbo.INV_LocationLotDet_13.LotNo, dbo.INV_LocationLotDet_13.Qty, dbo.INV_LocationLotDet_13.IsCS, dbo.INV_LocationLotDet_13.PlanBill, dbo.INV_LocationLotDet_13.QualityType, 
                      dbo.INV_LocationLotDet_13.IsFreeze, dbo.INV_LocationLotDet_13.IsATP, dbo.INV_LocationLotDet_13.OccupyType, dbo.INV_LocationLotDet_13.OccupyRefNo, 
                      dbo.INV_LocationLotDet_13.CreateUser, dbo.INV_LocationLotDet_13.CreateUserNm, dbo.INV_LocationLotDet_13.CreateDate, dbo.INV_LocationLotDet_13.LastModifyUser, 
                      dbo.INV_LocationLotDet_13.LastModifyUserNm, dbo.INV_LocationLotDet_13.LastModifyDate, dbo.INV_LocationLotDet_13.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_13 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_13.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_13.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_13.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_13.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_13.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_14.Id, dbo.INV_LocationLotDet_14.Location, dbo.INV_LocationLotDet_14.Bin, dbo.INV_LocationLotDet_14.Item, dbo.INV_LocationLotDet_14.HuId, 
                      dbo.INV_LocationLotDet_14.LotNo, dbo.INV_LocationLotDet_14.Qty, dbo.INV_LocationLotDet_14.IsCS, dbo.INV_LocationLotDet_14.PlanBill, dbo.INV_LocationLotDet_14.QualityType, 
                      dbo.INV_LocationLotDet_14.IsFreeze, dbo.INV_LocationLotDet_14.IsATP, dbo.INV_LocationLotDet_14.OccupyType, dbo.INV_LocationLotDet_14.OccupyRefNo, 
                      dbo.INV_LocationLotDet_14.CreateUser, dbo.INV_LocationLotDet_14.CreateUserNm, dbo.INV_LocationLotDet_14.CreateDate, dbo.INV_LocationLotDet_14.LastModifyUser, 
                      dbo.INV_LocationLotDet_14.LastModifyUserNm, dbo.INV_LocationLotDet_14.LastModifyDate, dbo.INV_LocationLotDet_14.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_14 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_14.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_14.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_14.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_14.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_14.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_15.Id, dbo.INV_LocationLotDet_15.Location, dbo.INV_LocationLotDet_15.Bin, dbo.INV_LocationLotDet_15.Item, dbo.INV_LocationLotDet_15.HuId, 
                      dbo.INV_LocationLotDet_15.LotNo, dbo.INV_LocationLotDet_15.Qty, dbo.INV_LocationLotDet_15.IsCS, dbo.INV_LocationLotDet_15.PlanBill, dbo.INV_LocationLotDet_15.QualityType, 
                      dbo.INV_LocationLotDet_15.IsFreeze, dbo.INV_LocationLotDet_15.IsATP, dbo.INV_LocationLotDet_15.OccupyType, dbo.INV_LocationLotDet_15.OccupyRefNo, 
                      dbo.INV_LocationLotDet_15.CreateUser, dbo.INV_LocationLotDet_15.CreateUserNm, dbo.INV_LocationLotDet_15.CreateDate, dbo.INV_LocationLotDet_15.LastModifyUser, 
                      dbo.INV_LocationLotDet_15.LastModifyUserNm, dbo.INV_LocationLotDet_15.LastModifyDate, dbo.INV_LocationLotDet_15.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_15 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_15.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_15.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_15.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_15.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_15.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_16.Id, dbo.INV_LocationLotDet_16.Location, dbo.INV_LocationLotDet_16.Bin, dbo.INV_LocationLotDet_16.Item, dbo.INV_LocationLotDet_16.HuId, 
                      dbo.INV_LocationLotDet_16.LotNo, dbo.INV_LocationLotDet_16.Qty, dbo.INV_LocationLotDet_16.IsCS, dbo.INV_LocationLotDet_16.PlanBill, dbo.INV_LocationLotDet_16.QualityType, 
                      dbo.INV_LocationLotDet_16.IsFreeze, dbo.INV_LocationLotDet_16.IsATP, dbo.INV_LocationLotDet_16.OccupyType, dbo.INV_LocationLotDet_16.OccupyRefNo, 
                      dbo.INV_LocationLotDet_16.CreateUser, dbo.INV_LocationLotDet_16.CreateUserNm, dbo.INV_LocationLotDet_16.CreateDate, dbo.INV_LocationLotDet_16.LastModifyUser, 
                      dbo.INV_LocationLotDet_16.LastModifyUserNm, dbo.INV_LocationLotDet_16.LastModifyDate, dbo.INV_LocationLotDet_16.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_16 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_16.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_16.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_16.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_16.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_16.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_17.Id, dbo.INV_LocationLotDet_17.Location, dbo.INV_LocationLotDet_17.Bin, dbo.INV_LocationLotDet_17.Item, dbo.INV_LocationLotDet_17.HuId, 
                      dbo.INV_LocationLotDet_17.LotNo, dbo.INV_LocationLotDet_17.Qty, dbo.INV_LocationLotDet_17.IsCS, dbo.INV_LocationLotDet_17.PlanBill, dbo.INV_LocationLotDet_17.QualityType, 
                      dbo.INV_LocationLotDet_17.IsFreeze, dbo.INV_LocationLotDet_17.IsATP, dbo.INV_LocationLotDet_17.OccupyType, dbo.INV_LocationLotDet_17.OccupyRefNo, 
                      dbo.INV_LocationLotDet_17.CreateUser, dbo.INV_LocationLotDet_17.CreateUserNm, dbo.INV_LocationLotDet_17.CreateDate, dbo.INV_LocationLotDet_17.LastModifyUser, 
                      dbo.INV_LocationLotDet_17.LastModifyUserNm, dbo.INV_LocationLotDet_17.LastModifyDate, dbo.INV_LocationLotDet_17.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_17 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_17.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_17.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_17.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_17.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_17.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_18.Id, dbo.INV_LocationLotDet_18.Location, dbo.INV_LocationLotDet_18.Bin, dbo.INV_LocationLotDet_18.Item, dbo.INV_LocationLotDet_18.HuId, 
                      dbo.INV_LocationLotDet_18.LotNo, dbo.INV_LocationLotDet_18.Qty, dbo.INV_LocationLotDet_18.IsCS, dbo.INV_LocationLotDet_18.PlanBill, dbo.INV_LocationLotDet_18.QualityType, 
                      dbo.INV_LocationLotDet_18.IsFreeze, dbo.INV_LocationLotDet_18.IsATP, dbo.INV_LocationLotDet_18.OccupyType, dbo.INV_LocationLotDet_18.OccupyRefNo, 
                      dbo.INV_LocationLotDet_18.CreateUser, dbo.INV_LocationLotDet_18.CreateUserNm, dbo.INV_LocationLotDet_18.CreateDate, dbo.INV_LocationLotDet_18.LastModifyUser, 
                      dbo.INV_LocationLotDet_18.LastModifyUserNm, dbo.INV_LocationLotDet_18.LastModifyDate, dbo.INV_LocationLotDet_18.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_18 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_18.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_18.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_18.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_18.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_18.Qty <> 0)
UNION ALL
SELECT     dbo.INV_LocationLotDet_19.Id, dbo.INV_LocationLotDet_19.Location, dbo.INV_LocationLotDet_19.Bin, dbo.INV_LocationLotDet_19.Item, dbo.INV_LocationLotDet_19.HuId, 
                      dbo.INV_LocationLotDet_19.LotNo, dbo.INV_LocationLotDet_19.Qty, dbo.INV_LocationLotDet_19.IsCS, dbo.INV_LocationLotDet_19.PlanBill, dbo.INV_LocationLotDet_19.QualityType, 
                      dbo.INV_LocationLotDet_19.IsFreeze, dbo.INV_LocationLotDet_19.IsATP, dbo.INV_LocationLotDet_19.OccupyType, dbo.INV_LocationLotDet_19.OccupyRefNo, 
                      dbo.INV_LocationLotDet_19.CreateUser, dbo.INV_LocationLotDet_19.CreateUserNm, dbo.INV_LocationLotDet_19.CreateDate, dbo.INV_LocationLotDet_19.LastModifyUser, 
                      dbo.INV_LocationLotDet_19.LastModifyUserNm, dbo.INV_LocationLotDet_19.LastModifyDate, dbo.INV_LocationLotDet_19.Version, dbo.MD_LocationBin.Area, 
                      dbo.MD_LocationBin.Seq AS BinSeq, dbo.INV_Hu.Qty AS HuQty, dbo.INV_Hu.UC, dbo.INV_Hu.Uom AS HuUom, dbo.INV_Hu.BaseUom, dbo.INV_Hu.UnitQty, 
                      dbo.INV_Hu.ManufactureParty, dbo.INV_Hu.ManufactureDate, dbo.INV_Hu.FirstInvDate, dbo.BIL_PlanBill.Party AS ConsigementParty, dbo.INV_Hu.IsOdd
FROM         dbo.INV_LocationLotDet_19 LEFT OUTER JOIN
                      dbo.INV_Hu ON dbo.INV_LocationLotDet_19.HuId = dbo.INV_Hu.HuId LEFT OUTER JOIN
                      dbo.BIL_PlanBill ON dbo.INV_LocationLotDet_19.PlanBill = dbo.BIL_PlanBill.Id AND dbo.INV_LocationLotDet_19.IsCS = 1 LEFT OUTER JOIN
                      dbo.MD_LocationBin ON dbo.INV_LocationLotDet_19.Bin = dbo.MD_LocationBin.Code
WHERE     (dbo.INV_LocationLotDet_19.Qty <> 0)
GO
