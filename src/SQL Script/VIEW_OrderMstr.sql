
/****** Object:  View [dbo].[VIEW_OrderMstr]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='V' AND name='VIEW_OrderMstr')
	DROP VIEW VIEW_OrderMstr
CREATE VIEW [dbo].[VIEW_OrderMstr]
AS
SELECT     OrderNo, Flow, FlowDesc, ProdLineFact, TraceCode, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPlanPause, PauseSeq, IsPause, PauseTime, IsPLPause, 
                      EffDate, Priority, Status, Seq, SapSeq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
                      ShipToAddr, ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, 
                      Currency, Dock, Routing, CurtOp, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, 
                      IsPrintRec, IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPLCreate, IsShipFifo, IsRecFifo, 
                      IsShipByOrder, IsOpenOrder, IsAsnUniqueRec, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, HuTemplate, 
                      BillTerm, CreateHuOpt, ReCalculatePriceOpt, PickStrategy, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CompleteDate, CompleteUser, CompleteUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version, IsQuick, ExtraDmdSource, WMSNo
FROM         dbo.ORD_OrderMstr_1
UNION ALL
SELECT     OrderNo, Flow, FlowDesc, ProdLineFact, TraceCode, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPlanPause, PauseSeq, IsPause, PauseTime, IsPLPause, 
                      EffDate, Priority, Status, Seq, SapSeq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
                      ShipToAddr, ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, 
                      Currency, Dock, Routing, CurtOp, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, 
                      IsPrintRec, IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPLCreate, IsShipFifo, IsRecFifo, 
                      IsShipByOrder, IsOpenOrder, IsAsnUniqueRec, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, HuTemplate, 
                      BillTerm, CreateHuOpt, ReCalculatePriceOpt, PickStrategy, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CompleteDate, CompleteUser, CompleteUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version, IsQuick, ExtraDmdSource, WMSNo
FROM         dbo.ORD_OrderMstr_2
UNION ALL
SELECT     OrderNo, Flow, FlowDesc, ProdLineFact, TraceCode, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPlanPause, PauseSeq, IsPause, PauseTime, IsPLPause, 
                      EffDate, Priority, Status, Seq, SapSeq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
                      ShipToAddr, ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, 
                      Currency, Dock, Routing, CurtOp, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, 
                      IsPrintRec, IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPLCreate, IsShipFifo, IsRecFifo, 
                      IsShipByOrder, IsOpenOrder, IsAsnUniqueRec, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, HuTemplate, 
                      BillTerm, CreateHuOpt, ReCalculatePriceOpt, PickStrategy, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CompleteDate, CompleteUser, CompleteUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version, IsQuick, ExtraDmdSource, WMSNo
FROM         dbo.ORD_OrderMstr_3
UNION ALL
SELECT     OrderNo, Flow, FlowDesc, ProdLineFact, TraceCode, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPlanPause, PauseSeq, IsPause, PauseTime, IsPLPause, 
                      EffDate, Priority, Status, Seq, SapSeq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
                      ShipToAddr, ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, 
                      Currency, Dock, Routing, CurtOp, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, 
                      IsPrintRec, IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPLCreate, IsShipFifo, IsRecFifo, 
                      IsShipByOrder, IsOpenOrder, IsAsnUniqueRec, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, HuTemplate, 
                      BillTerm, CreateHuOpt, ReCalculatePriceOpt, PickStrategy, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CompleteDate, CompleteUser, CompleteUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version, IsQuick, ExtraDmdSource, WMSNo
FROM         dbo.ORD_OrderMstr_4
UNION ALL
SELECT     OrderNo, Flow, FlowDesc, ProdLineFact, TraceCode, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPlanPause, PauseSeq, IsPause, PauseTime, IsPLPause, 
                      EffDate, Priority, Status, Seq, SapSeq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
                      ShipToAddr, ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, 
                      Currency, Dock, Routing, CurtOp, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, 
                      IsPrintRec, IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPLCreate, IsShipFifo, IsRecFifo, 
                      IsShipByOrder, IsOpenOrder, IsAsnUniqueRec, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, HuTemplate, 
                      BillTerm, CreateHuOpt, ReCalculatePriceOpt, PickStrategy, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CompleteDate, CompleteUser, CompleteUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version, IsQuick, ExtraDmdSource, WMSNo
FROM         dbo.ORD_OrderMstr_5
UNION ALL
SELECT     OrderNo, Flow, FlowDesc, ProdLineFact, TraceCode, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPlanPause, PauseSeq, IsPause, PauseTime, IsPLPause, 
                      EffDate, Priority, Status, Seq, SapSeq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
                      ShipToAddr, ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, 
                      Currency, Dock, Routing, CurtOp, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, 
                      IsPrintRec, IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPLCreate, IsShipFifo, IsRecFifo, 
                      IsShipByOrder, IsOpenOrder, IsAsnUniqueRec, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, HuTemplate, 
                      BillTerm, CreateHuOpt, ReCalculatePriceOpt, PickStrategy, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CompleteDate, CompleteUser, CompleteUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version, IsQuick, ExtraDmdSource, WMSNo
FROM         dbo.ORD_OrderMstr_6
UNION ALL
SELECT     OrderNo, Flow, FlowDesc, ProdLineFact, TraceCode, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPlanPause, PauseSeq, IsPause, PauseTime, IsPLPause, 
                      EffDate, Priority, Status, Seq, SapSeq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
                      ShipToAddr, ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, 
                      Currency, Dock, Routing, CurtOp, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, 
                      IsPrintRec, IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPLCreate, IsShipFifo, IsRecFifo, 
                      IsShipByOrder, IsOpenOrder, IsAsnUniqueRec, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, HuTemplate, 
                      BillTerm, CreateHuOpt, ReCalculatePriceOpt, PickStrategy, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CompleteDate, CompleteUser, CompleteUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version, IsQuick, ExtraDmdSource, WMSNo
FROM         dbo.ORD_OrderMstr_7
UNION ALL
SELECT     OrderNo, Flow, FlowDesc, ProdLineFact, TraceCode, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPlanPause, PauseSeq, IsPause, PauseTime, IsPLPause, 
                      EffDate, Priority, Status, Seq, SapSeq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
                      ShipToAddr, ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, 
                      Currency, Dock, Routing, CurtOp, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, 
                      IsPrintRec, IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPLCreate, IsShipFifo, IsRecFifo, 
                      IsShipByOrder, IsOpenOrder, IsAsnUniqueRec, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, HuTemplate, 
                      BillTerm, CreateHuOpt, ReCalculatePriceOpt, PickStrategy, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CompleteDate, CompleteUser, CompleteUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version, IsQuick, ExtraDmdSource, WMSNo
FROM         dbo.ORD_OrderMstr_8
UNION ALL
SELECT     OrderNo, Flow, FlowDesc, ProdLineFact, TraceCode, OrderStrategy, RefOrderNo, ExtOrderNo, Type, SubType, QualityType, StartTime, WindowTime, IsPlanPause, PauseSeq, IsPause, PauseTime, IsPLPause, 
                      EffDate, Priority, Status, Seq, SapSeq, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, ShipFromFax, ShipFromContact, 
                      ShipToAddr, ShipTo, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Shift, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, BillAddrDesc, PriceList, 
                      Currency, Dock, Routing, CurtOp, IsAutoRelease, IsAutoStart, IsAutoShip, IsAutoReceive, IsAutoBill, IsManualCreateDet, IsListPrice, IsPrintOrder, IsOrderPrinted, IsPrintAsn, 
                      IsPrintRec, IsShipExceed, IsRecExceed, IsOrderFulfillUC, IsShipFulfillUC, IsRecFulfillUC, IsShipScanHu, IsRecScanHu, IsCreatePL, IsPLCreate, IsShipFifo, IsRecFifo, 
                      IsShipByOrder, IsOpenOrder, IsAsnUniqueRec, IsCheckPartyFromAuth, IsCheckPartyToAuth, RecGapTo, RecTemplate, OrderTemplate, AsnTemplate, HuTemplate, 
                      BillTerm, CreateHuOpt, ReCalculatePriceOpt, PickStrategy, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
                      ReleaseDate, ReleaseUser, ReleaseUserNm, StartDate, StartUser, StartUserNm, CompleteDate, CompleteUser, CompleteUserNm, CloseDate, CloseUser, CloseUserNm, CancelDate, CancelUser, CancelUserNm, 
                      CancelReason, Version, IsQuick, ExtraDmdSource, WMSNo
FROM         dbo.ORD_OrderMstr_0
GO
