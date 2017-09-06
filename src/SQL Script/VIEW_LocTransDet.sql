/****** Object:  View [dbo].[VIEW_LocTransDet]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.Objects WHERE TYPE='v' AND name='VIEW_LocTransDet')
	DROP PROCEDURE VIEW_LocTransDet
CREATE VIEW [dbo].[VIEW_LocTransDet]
AS
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_0
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_1
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_2
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_3
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_4

UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_5
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_6
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_7
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_8
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_9
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_10
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_11
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_12
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_13
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_14
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_15
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_16
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_17
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_18
UNION ALL
SELECT     Id, LocTransId, OrderNo, OrderType, OrderSubType, OrderDetSeq, OrderDetId, OrderBomDetId, IpNo, IpDetId, IpDetSeq, RecNo, RecDetId, RecDetSeq, SeqNo, BillTransId, 
                      LocLotDetId, Item, Qty, IsCS, PlanBill, PlanBillQty, ActBill, ActBillQty, QualityType, HuId, LotNo, TransType, IOType, PartyFrom, PartyTo, LocFrom, LocTo, Bin, LocIOReason, EffDate, CreateUser, CreateDate, 
                      TraceCode,OrderBomDetSeq, PlanBackflushId
FROM         dbo.INV_LocTransDet_19
GO
