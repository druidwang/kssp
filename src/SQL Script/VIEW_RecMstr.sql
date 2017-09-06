/****** Object:  View [dbo].[VIEW_RecMstr]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='V' AND name='VIEW_RecMstr')
	DROP VIEW VIEW_RecMstr
CREATE VIEW [dbo].[VIEW_RecMstr]
AS
SELECT     RecNo, ExtRecNo, IpNo, SeqNo, Status, Type, OrderType, OrderSubType, QualityType, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, 
                      ShipFromFax, ShipFromContact, ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Dock, EffDate, IsPrintRec, IsRecPrinted, 
                      IsCheckPartyFromAuth, IsCheckPartyToAuth, RecTemplate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, 
                      IsRecScanHu, CreateHuOpt, WMSNo, Flow
FROM         dbo.ORD_RecMstr_1
UNION ALL
SELECT     RecNo, ExtRecNo, IpNo, SeqNo, Status, Type, OrderType, OrderSubType, QualityType, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, 
                      ShipFromFax, ShipFromContact, ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Dock, EffDate, IsPrintRec, IsRecPrinted, 
                      IsCheckPartyFromAuth, IsCheckPartyToAuth, RecTemplate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, 
                      IsRecScanHu, CreateHuOpt, WMSNo, Flow
FROM         dbo.ORD_RecMstr_2
UNION ALL
SELECT     RecNo, ExtRecNo, IpNo, SeqNo, Status, Type, OrderType, OrderSubType, QualityType, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, 
                      ShipFromFax, ShipFromContact, ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Dock, EffDate, IsPrintRec, IsRecPrinted, 
                      IsCheckPartyFromAuth, IsCheckPartyToAuth, RecTemplate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, 
                      IsRecScanHu, CreateHuOpt, WMSNo, Flow
FROM         dbo.ORD_RecMstr_3
UNION ALL
SELECT     RecNo, ExtRecNo, IpNo, SeqNo, Status, Type, OrderType, OrderSubType, QualityType, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, 
                      ShipFromFax, ShipFromContact, ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Dock, EffDate, IsPrintRec, IsRecPrinted, 
                      IsCheckPartyFromAuth, IsCheckPartyToAuth, RecTemplate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, 
                      IsRecScanHu, CreateHuOpt, WMSNo, Flow
FROM         dbo.ORD_RecMstr_4
UNION ALL
SELECT     RecNo, ExtRecNo, IpNo, SeqNo, Status, Type, OrderType, OrderSubType, QualityType, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, 
                      ShipFromFax, ShipFromContact, ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Dock, EffDate, IsPrintRec, IsRecPrinted, 
                      IsCheckPartyFromAuth, IsCheckPartyToAuth, RecTemplate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, 
                      IsRecScanHu, CreateHuOpt, WMSNo, Flow
FROM         dbo.ORD_RecMstr_5
UNION ALL
SELECT     RecNo, ExtRecNo, IpNo, SeqNo, Status, Type, OrderType, OrderSubType, QualityType, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, 
                      ShipFromFax, ShipFromContact, ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Dock, EffDate, IsPrintRec, IsRecPrinted, 
                      IsCheckPartyFromAuth, IsCheckPartyToAuth, RecTemplate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, 
                      IsRecScanHu, CreateHuOpt, WMSNo, Flow
FROM         dbo.ORD_RecMstr_6
UNION ALL
SELECT     RecNo, ExtRecNo, IpNo, SeqNo, Status, Type, OrderType, OrderSubType, QualityType, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, 
                      ShipFromFax, ShipFromContact, ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Dock, EffDate, IsPrintRec, IsRecPrinted, 
                      IsCheckPartyFromAuth, IsCheckPartyToAuth, RecTemplate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, 
                      IsRecScanHu, CreateHuOpt, WMSNo, Flow
FROM         dbo.ORD_RecMstr_7
UNION ALL
SELECT     RecNo, ExtRecNo, IpNo, SeqNo, Status, Type, OrderType, OrderSubType, QualityType, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, 
                      ShipFromFax, ShipFromContact, ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Dock, EffDate, IsPrintRec, IsRecPrinted, 
                      IsCheckPartyFromAuth, IsCheckPartyToAuth, RecTemplate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, 
                      IsRecScanHu, CreateHuOpt, WMSNo, Flow
FROM         dbo.ORD_RecMstr_8
UNION ALL
SELECT     RecNo, ExtRecNo, IpNo, SeqNo, Status, Type, OrderType, OrderSubType, QualityType, PartyFrom, PartyFromNm, PartyTo, PartyToNm, ShipFrom, ShipFromAddr, ShipFromTel, ShipFromCell, 
                      ShipFromFax, ShipFromContact, ShipTo, ShipToAddr, ShipToTel, ShipToCell, ShipToFax, ShipToContact, Dock, EffDate, IsPrintRec, IsRecPrinted, 
                      IsCheckPartyFromAuth, IsCheckPartyToAuth, RecTemplate, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Version, 
                      IsRecScanHu, CreateHuOpt, WMSNo, Flow
FROM         dbo.ORD_RecMstr_0
GO
