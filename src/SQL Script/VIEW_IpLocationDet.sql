/****** Object:  View [dbo].[VIEW_IpLocationDet]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='V' AND name='VIEW_IpLocationDet')
	DROP VIEW VIEW_IpLocationDet
CREATE VIEW [dbo].[VIEW_IpLocationDet]
AS
SELECT     Id, IpNo, IpDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, RecQty, 
                      IsClose, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, OrderType, WMSSeq
FROM         dbo.ORD_IpLocationDet_1
UNION ALL
SELECT     Id, IpNo, IpDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, RecQty, 
                      IsClose, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, OrderType, WMSSeq
FROM         dbo.ORD_IpLocationDet_2
UNION ALL
SELECT     Id, IpNo, IpDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, RecQty, 
                      IsClose, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, OrderType, WMSSeq
FROM         dbo.ORD_IpLocationDet_3
UNION ALL
SELECT     Id, IpNo, IpDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, RecQty, 
                      IsClose, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, OrderType, WMSSeq
FROM         dbo.ORD_IpLocationDet_4
UNION ALL
SELECT     Id, IpNo, IpDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, RecQty, 
                      IsClose, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, OrderType, WMSSeq
FROM         dbo.ORD_IpLocationDet_5
UNION ALL
SELECT     Id, IpNo, IpDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, RecQty, 
                      IsClose, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, OrderType, WMSSeq
FROM         dbo.ORD_IpLocationDet_6
UNION ALL
SELECT     Id, IpNo, IpDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, RecQty, 
                      IsClose, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, OrderType, WMSSeq
FROM         dbo.ORD_IpLocationDet_7
UNION ALL
SELECT     Id, IpNo, IpDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, RecQty, 
                      IsClose, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, OrderType, WMSSeq
FROM         dbo.ORD_IpLocationDet_8
UNION ALL
SELECT     Id, IpNo, IpDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, RecQty, 
                      IsClose, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, OrderType, WMSSeq
FROM         dbo.ORD_IpLocationDet_0
GO