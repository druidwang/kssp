
/****** Object:  View [dbo].[VIEW_RecLocationDet]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='V' AND name='VIEW_RecLocationDet')
	DROP VIEW VIEW_RecLocationDet
CREATE VIEW [dbo].[VIEW_RecLocationDet]
AS
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_0
UNION ALL
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_1
UNION ALL
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_2
UNION ALL
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_3
UNION ALL
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_4
UNION ALL
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_5
UNION ALL
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_6
UNION ALL
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_7
UNION ALL
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_8
UNION ALL
SELECT     Id, RecNo, RecDetId, OrderDetId, Item, HuId, LotNo, IsCreatePlanBill, IsCS, PlanBill, ActBill, QualityType, IsFreeze, IsATP, OccupyType, OccupyRefNo, Qty, 
                      CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, OrderType, WMSSeq
FROM         dbo.ORD_RecLocationDet_0
GO

