/****** Object:  View [dbo].[VIEW_IpMstr]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='V' AND name='VIEW_IpMstr')
	DROP VIEW VIEW_IpMstr
CREATE VIEW [dbo].[VIEW_IpMstr]
AS
SELECT     IpNo, EffDate, IsShipScanHu, Version, CloseReason, CloseUserNm, CloseUser, CloseDate, LastModifyDate, LastModifyUserNm, CreateDate, LastModifyUser, 
                      CreateUserNm, CreateUser, HuTemplate, RecTemplate, RecGapTo, AsnTemplate, IsCheckPartyToAuth, IsCheckPartyFromAuth, IsRecScanHu, IsAsnUniqueRec, 
                      IsRecFifo, IsRecFulfillUC, IsRecExceed, IsPrintRec, IsAsnPrinted, IsPrintAsn, CreateHuOpt, IsAutoReceive, Dock, ShipToContact, ShipToFax, ShipToCell, ShipToTel, 
                      ShipToAddr, ShipTo, ShipFromContact, ShipFromFax, ShipFromCell, ShipFromTel, ShipFromAddr, ShipFrom, PartyToNm, PartyTo, PartyFromNm, PartyFrom, 
                      ArriveTime, DepartTime, Status, QualityType, OrderType, OrderSubType, Type, GapIpNo, ExtIpNo, SeqNo, WMSNo, Flow
FROM         dbo.ORD_IpMstr_1
UNION ALL
SELECT     IpNo, EffDate, IsShipScanHu, Version, CloseReason, CloseUserNm, CloseUser, CloseDate, LastModifyDate, LastModifyUserNm, CreateDate, LastModifyUser, 
                      CreateUserNm, CreateUser, HuTemplate, RecTemplate, RecGapTo, AsnTemplate, IsCheckPartyToAuth, IsCheckPartyFromAuth, IsRecScanHu, IsAsnUniqueRec, 
                      IsRecFifo, IsRecFulfillUC, IsRecExceed, IsPrintRec, IsAsnPrinted, IsPrintAsn, CreateHuOpt, IsAutoReceive, Dock, ShipToContact, ShipToFax, ShipToCell, ShipToTel, 
                      ShipToAddr, ShipTo, ShipFromContact, ShipFromFax, ShipFromCell, ShipFromTel, ShipFromAddr, ShipFrom, PartyToNm, PartyTo, PartyFromNm, PartyFrom, 
                      ArriveTime, DepartTime, Status, QualityType, OrderType, OrderSubType, Type, GapIpNo, ExtIpNo, SeqNo, WMSNo, Flow
FROM         dbo.ORD_IpMstr_2
UNION ALL
SELECT     IpNo, EffDate, IsShipScanHu, Version, CloseReason, CloseUserNm, CloseUser, CloseDate, LastModifyDate, LastModifyUserNm, CreateDate, LastModifyUser, 
                      CreateUserNm, CreateUser, HuTemplate, RecTemplate, RecGapTo, AsnTemplate, IsCheckPartyToAuth, IsCheckPartyFromAuth, IsRecScanHu, IsAsnUniqueRec, 
                      IsRecFifo, IsRecFulfillUC, IsRecExceed, IsPrintRec, IsAsnPrinted, IsPrintAsn, CreateHuOpt, IsAutoReceive, Dock, ShipToContact, ShipToFax, ShipToCell, ShipToTel, 
                      ShipToAddr, ShipTo, ShipFromContact, ShipFromFax, ShipFromCell, ShipFromTel, ShipFromAddr, ShipFrom, PartyToNm, PartyTo, PartyFromNm, PartyFrom, 
                      ArriveTime, DepartTime, Status, QualityType, OrderType, OrderSubType, Type, GapIpNo, ExtIpNo, SeqNo, WMSNo, Flow
FROM         dbo.ORD_IpMstr_3
UNION ALL
SELECT     IpNo, EffDate, IsShipScanHu, Version, CloseReason, CloseUserNm, CloseUser, CloseDate, LastModifyDate, LastModifyUserNm, CreateDate, LastModifyUser, 
                      CreateUserNm, CreateUser, HuTemplate, RecTemplate, RecGapTo, AsnTemplate, IsCheckPartyToAuth, IsCheckPartyFromAuth, IsRecScanHu, IsAsnUniqueRec, 
                      IsRecFifo, IsRecFulfillUC, IsRecExceed, IsPrintRec, IsAsnPrinted, IsPrintAsn, CreateHuOpt, IsAutoReceive, Dock, ShipToContact, ShipToFax, ShipToCell, ShipToTel, 
                      ShipToAddr, ShipTo, ShipFromContact, ShipFromFax, ShipFromCell, ShipFromTel, ShipFromAddr, ShipFrom, PartyToNm, PartyTo, PartyFromNm, PartyFrom, 
                      ArriveTime, DepartTime, Status, QualityType, OrderType, OrderSubType, Type, GapIpNo, ExtIpNo, SeqNo, WMSNo, Flow
FROM         dbo.ORD_IpMstr_4
UNION ALL
SELECT     IpNo, EffDate, IsShipScanHu, Version, CloseReason, CloseUserNm, CloseUser, CloseDate, LastModifyDate, LastModifyUserNm, CreateDate, LastModifyUser, 
                      CreateUserNm, CreateUser, HuTemplate, RecTemplate, RecGapTo, AsnTemplate, IsCheckPartyToAuth, IsCheckPartyFromAuth, IsRecScanHu, IsAsnUniqueRec, 
                      IsRecFifo, IsRecFulfillUC, IsRecExceed, IsPrintRec, IsAsnPrinted, IsPrintAsn, CreateHuOpt, IsAutoReceive, Dock, ShipToContact, ShipToFax, ShipToCell, ShipToTel, 
                      ShipToAddr, ShipTo, ShipFromContact, ShipFromFax, ShipFromCell, ShipFromTel, ShipFromAddr, ShipFrom, PartyToNm, PartyTo, PartyFromNm, PartyFrom, 
                      ArriveTime, DepartTime, Status, QualityType, OrderType, OrderSubType, Type, GapIpNo, ExtIpNo, SeqNo, WMSNo, Flow
FROM         dbo.ORD_IpMstr_5
UNION ALL
SELECT     IpNo, EffDate, IsShipScanHu, Version, CloseReason, CloseUserNm, CloseUser, CloseDate, LastModifyDate, LastModifyUserNm, CreateDate, LastModifyUser, 
                      CreateUserNm, CreateUser, HuTemplate, RecTemplate, RecGapTo, AsnTemplate, IsCheckPartyToAuth, IsCheckPartyFromAuth, IsRecScanHu, IsAsnUniqueRec, 
                      IsRecFifo, IsRecFulfillUC, IsRecExceed, IsPrintRec, IsAsnPrinted, IsPrintAsn, CreateHuOpt, IsAutoReceive, Dock, ShipToContact, ShipToFax, ShipToCell, ShipToTel, 
                      ShipToAddr, ShipTo, ShipFromContact, ShipFromFax, ShipFromCell, ShipFromTel, ShipFromAddr, ShipFrom, PartyToNm, PartyTo, PartyFromNm, PartyFrom, 
                      ArriveTime, DepartTime, Status, QualityType, OrderType, OrderSubType, Type, GapIpNo, ExtIpNo, SeqNo, WMSNo, Flow
FROM         dbo.ORD_IpMstr_6
UNION ALL
SELECT     IpNo, EffDate, IsShipScanHu, Version, CloseReason, CloseUserNm, CloseUser, CloseDate, LastModifyDate, LastModifyUserNm, CreateDate, LastModifyUser, 
                      CreateUserNm, CreateUser, HuTemplate, RecTemplate, RecGapTo, AsnTemplate, IsCheckPartyToAuth, IsCheckPartyFromAuth, IsRecScanHu, IsAsnUniqueRec, 
                      IsRecFifo, IsRecFulfillUC, IsRecExceed, IsPrintRec, IsAsnPrinted, IsPrintAsn, CreateHuOpt, IsAutoReceive, Dock, ShipToContact, ShipToFax, ShipToCell, ShipToTel, 
                      ShipToAddr, ShipTo, ShipFromContact, ShipFromFax, ShipFromCell, ShipFromTel, ShipFromAddr, ShipFrom, PartyToNm, PartyTo, PartyFromNm, PartyFrom, 
                      ArriveTime, DepartTime, Status, QualityType, OrderType, OrderSubType, Type, GapIpNo, ExtIpNo, SeqNo, WMSNo, Flow
FROM         dbo.ORD_IpMstr_7
UNION ALL
SELECT     IpNo, EffDate, IsShipScanHu, Version, CloseReason, CloseUserNm, CloseUser, CloseDate, LastModifyDate, LastModifyUserNm, CreateDate, LastModifyUser, 
                      CreateUserNm, CreateUser, HuTemplate, RecTemplate, RecGapTo, AsnTemplate, IsCheckPartyToAuth, IsCheckPartyFromAuth, IsRecScanHu, IsAsnUniqueRec, 
                      IsRecFifo, IsRecFulfillUC, IsRecExceed, IsPrintRec, IsAsnPrinted, IsPrintAsn, CreateHuOpt, IsAutoReceive, Dock, ShipToContact, ShipToFax, ShipToCell, ShipToTel, 
                      ShipToAddr, ShipTo, ShipFromContact, ShipFromFax, ShipFromCell, ShipFromTel, ShipFromAddr, ShipFrom, PartyToNm, PartyTo, PartyFromNm, PartyFrom, 
                      ArriveTime, DepartTime, Status, QualityType, OrderType, OrderSubType, Type, GapIpNo, ExtIpNo, SeqNo, WMSNo, Flow
FROM         dbo.ORD_IpMstr_8
UNION ALL
SELECT     IpNo, EffDate, IsShipScanHu, Version, CloseReason, CloseUserNm, CloseUser, CloseDate, LastModifyDate, LastModifyUserNm, CreateDate, LastModifyUser, 
                      CreateUserNm, CreateUser, HuTemplate, RecTemplate, RecGapTo, AsnTemplate, IsCheckPartyToAuth, IsCheckPartyFromAuth, IsRecScanHu, IsAsnUniqueRec, 
                      IsRecFifo, IsRecFulfillUC, IsRecExceed, IsPrintRec, IsAsnPrinted, IsPrintAsn, CreateHuOpt, IsAutoReceive, Dock, ShipToContact, ShipToFax, ShipToCell, ShipToTel, 
                      ShipToAddr, ShipTo, ShipFromContact, ShipFromFax, ShipFromCell, ShipFromTel, ShipFromAddr, ShipFrom, PartyToNm, PartyTo, PartyFromNm, PartyFrom, 
                      ArriveTime, DepartTime, Status, QualityType, OrderType, OrderSubType, Type, GapIpNo, ExtIpNo, SeqNo, WMSNo, Flow
FROM         dbo.ORD_IpMstr_0
GO
