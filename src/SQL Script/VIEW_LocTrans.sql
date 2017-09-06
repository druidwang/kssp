/****** Object:  View [dbo].[VIEW_LocTrans]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.Objects WHERE TYPE='v' AND name='VIEW_LocTrans')
	DROP VIEW VIEW_LocTrans
CREATE VIEW [dbo].[VIEW_LocTrans]
AS
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode, OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_1
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_0
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_2
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_3
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_4
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_5
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_6
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_7
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_8
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_9
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_10
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_11
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_12
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_13
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_14
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_15
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_16
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_17
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_18
UNION ALL
SELECT     Id, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, Item, Uom, BaseUom, Qty, IsCS, PlanBill, PlanBillQty,
                      ActBill, ActBillQty, UnitQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, LocIOReason, EffDate, CreateUser, CreateDate, TraceCode,OrderBomDetSeq, SeqNo
FROM         dbo.INV_LocTrans_19
GO
