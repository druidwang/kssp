/****** Object:  StoredProcedure [dbo].[USP_Split_IpMstr_UPDATE]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_IpMstr_UPDATE')
	DROP PROCEDURE USP_Split_IpMstr_UPDATE
CREATE PROCEDURE [dbo].[USP_Split_IpMstr_UPDATE]
(
	@Version int,
	@ExtIpNo varchar(50),
	@GapIpNo varchar(50),
	@SeqNo varchar(50),
	@Flow varchar(50),
	@Type tinyint,
	@OrderType tinyint,
	@OrderSubType tinyint,
	@QualityType tinyint,
	@Status tinyint,
	@DepartTime datetime,
	@ArriveTime datetime,
	@PartyFrom varchar(50),
	@PartyFromNm varchar(100),
	@PartyTo varchar(50),
	@PartyToNm varchar(100),
	@ShipFrom varchar(50),
	@ShipFromAddr varchar(256),
	@ShipFromTel varchar(50),
	@ShipFromCell varchar(50),
	@ShipFromFax varchar(50),
	@ShipFromContact varchar(50),
	@ShipTo varchar(50),
	@ShipToAddr varchar(256),
	@ShipToTel varchar(50),
	@ShipToCell varchar(50),
	@ShipToFax varchar(50),
	@ShipToContact varchar(50),
	@Dock varchar(100),
	@IsAutoReceive bit,
	@IsShipScanHu bit,
	@CreateHuOpt tinyint,
	@IsPrintAsn bit,
	@IsAsnPrinted bit,
	@IsPrintRec bit,
	@IsRecExceed bit,
	@IsRecFulfillUC bit,
	@IsRecFifo bit,
	@IsAsnUniqueRec bit,
	@IsRecScanHu bit,
	@RecGapTo tinyint,
	@AsnTemplate varchar(100),
	@RecTemplate varchar(100),
	@HuTemplate varchar(100),
	@EffDate datetime,
	@WMSNo varchar(50),
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime,
	@CloseDate datetime,
	@CloseUser int,
	@CloseUserNm varchar(100),
	@CloseReason varchar(256),
	@IsCheckPartyFromAuth bit,
	@IsCheckPartyToAuth bit,
	@IpNo varchar(4000),
	@VersionBerfore int
)
AS
BEGIN
	IF @OrderType=1
	BEGIN
		UPDATE ORD_IpMstr_1 SET Version=@Version,ExtIpNo=@ExtIpNo,GapIpNo=@GapIpNo,SeqNo=@SeqNo,Flow=@Flow,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,Status=@Status,DepartTime=@DepartTime,ArriveTime=@ArriveTime,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,IsAutoReceive=@IsAutoReceive,IsShipScanHu=@IsShipScanHu,CreateHuOpt=@CreateHuOpt,IsPrintAsn=@IsPrintAsn,IsAsnPrinted=@IsAsnPrinted,IsPrintRec=@IsPrintRec,IsRecExceed=@IsRecExceed,IsRecFulfillUC=@IsRecFulfillUC,IsRecFifo=@IsRecFifo,IsAsnUniqueRec=@IsAsnUniqueRec,IsRecScanHu=@IsRecScanHu,RecGapTo=@RecGapTo,AsnTemplate=@AsnTemplate,RecTemplate=@RecTemplate,HuTemplate=@HuTemplate,EffDate=@EffDate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CloseReason=@CloseReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,IpNo=@IpNo
		WHERE IpNo=@IpNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=2
	BEGIN
		UPDATE ORD_IpMstr_2 SET Version=@Version,ExtIpNo=@ExtIpNo,GapIpNo=@GapIpNo,SeqNo=@SeqNo,Flow=@Flow,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,Status=@Status,DepartTime=@DepartTime,ArriveTime=@ArriveTime,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,IsAutoReceive=@IsAutoReceive,IsShipScanHu=@IsShipScanHu,CreateHuOpt=@CreateHuOpt,IsPrintAsn=@IsPrintAsn,IsAsnPrinted=@IsAsnPrinted,IsPrintRec=@IsPrintRec,IsRecExceed=@IsRecExceed,IsRecFulfillUC=@IsRecFulfillUC,IsRecFifo=@IsRecFifo,IsAsnUniqueRec=@IsAsnUniqueRec,IsRecScanHu=@IsRecScanHu,RecGapTo=@RecGapTo,AsnTemplate=@AsnTemplate,RecTemplate=@RecTemplate,HuTemplate=@HuTemplate,EffDate=@EffDate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CloseReason=@CloseReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,IpNo=@IpNo
		WHERE IpNo=@IpNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=3
	BEGIN
		UPDATE ORD_IpMstr_3 SET Version=@Version,ExtIpNo=@ExtIpNo,GapIpNo=@GapIpNo,SeqNo=@SeqNo,Flow=@Flow,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,Status=@Status,DepartTime=@DepartTime,ArriveTime=@ArriveTime,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,IsAutoReceive=@IsAutoReceive,IsShipScanHu=@IsShipScanHu,CreateHuOpt=@CreateHuOpt,IsPrintAsn=@IsPrintAsn,IsAsnPrinted=@IsAsnPrinted,IsPrintRec=@IsPrintRec,IsRecExceed=@IsRecExceed,IsRecFulfillUC=@IsRecFulfillUC,IsRecFifo=@IsRecFifo,IsAsnUniqueRec=@IsAsnUniqueRec,IsRecScanHu=@IsRecScanHu,RecGapTo=@RecGapTo,AsnTemplate=@AsnTemplate,RecTemplate=@RecTemplate,HuTemplate=@HuTemplate,EffDate=@EffDate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CloseReason=@CloseReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,IpNo=@IpNo
		WHERE IpNo=@IpNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=4
	BEGIN
		UPDATE ORD_IpMstr_4 SET Version=@Version,ExtIpNo=@ExtIpNo,GapIpNo=@GapIpNo,SeqNo=@SeqNo,Flow=@Flow,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,Status=@Status,DepartTime=@DepartTime,ArriveTime=@ArriveTime,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,IsAutoReceive=@IsAutoReceive,IsShipScanHu=@IsShipScanHu,CreateHuOpt=@CreateHuOpt,IsPrintAsn=@IsPrintAsn,IsAsnPrinted=@IsAsnPrinted,IsPrintRec=@IsPrintRec,IsRecExceed=@IsRecExceed,IsRecFulfillUC=@IsRecFulfillUC,IsRecFifo=@IsRecFifo,IsAsnUniqueRec=@IsAsnUniqueRec,IsRecScanHu=@IsRecScanHu,RecGapTo=@RecGapTo,AsnTemplate=@AsnTemplate,RecTemplate=@RecTemplate,HuTemplate=@HuTemplate,EffDate=@EffDate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CloseReason=@CloseReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,IpNo=@IpNo
		WHERE IpNo=@IpNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=5
	BEGIN
		UPDATE ORD_IpMstr_5 SET Version=@Version,ExtIpNo=@ExtIpNo,GapIpNo=@GapIpNo,SeqNo=@SeqNo,Flow=@Flow,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,Status=@Status,DepartTime=@DepartTime,ArriveTime=@ArriveTime,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,IsAutoReceive=@IsAutoReceive,IsShipScanHu=@IsShipScanHu,CreateHuOpt=@CreateHuOpt,IsPrintAsn=@IsPrintAsn,IsAsnPrinted=@IsAsnPrinted,IsPrintRec=@IsPrintRec,IsRecExceed=@IsRecExceed,IsRecFulfillUC=@IsRecFulfillUC,IsRecFifo=@IsRecFifo,IsAsnUniqueRec=@IsAsnUniqueRec,IsRecScanHu=@IsRecScanHu,RecGapTo=@RecGapTo,AsnTemplate=@AsnTemplate,RecTemplate=@RecTemplate,HuTemplate=@HuTemplate,EffDate=@EffDate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CloseReason=@CloseReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,IpNo=@IpNo
		WHERE IpNo=@IpNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=6
	BEGIN
		UPDATE ORD_IpMstr_6 SET Version=@Version,ExtIpNo=@ExtIpNo,GapIpNo=@GapIpNo,SeqNo=@SeqNo,Flow=@Flow,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,Status=@Status,DepartTime=@DepartTime,ArriveTime=@ArriveTime,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,IsAutoReceive=@IsAutoReceive,IsShipScanHu=@IsShipScanHu,CreateHuOpt=@CreateHuOpt,IsPrintAsn=@IsPrintAsn,IsAsnPrinted=@IsAsnPrinted,IsPrintRec=@IsPrintRec,IsRecExceed=@IsRecExceed,IsRecFulfillUC=@IsRecFulfillUC,IsRecFifo=@IsRecFifo,IsAsnUniqueRec=@IsAsnUniqueRec,IsRecScanHu=@IsRecScanHu,RecGapTo=@RecGapTo,AsnTemplate=@AsnTemplate,RecTemplate=@RecTemplate,HuTemplate=@HuTemplate,EffDate=@EffDate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CloseReason=@CloseReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,IpNo=@IpNo
		WHERE IpNo=@IpNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=7
	BEGIN
		UPDATE ORD_IpMstr_7 SET Version=@Version,ExtIpNo=@ExtIpNo,GapIpNo=@GapIpNo,SeqNo=@SeqNo,Flow=@Flow,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,Status=@Status,DepartTime=@DepartTime,ArriveTime=@ArriveTime,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,IsAutoReceive=@IsAutoReceive,IsShipScanHu=@IsShipScanHu,CreateHuOpt=@CreateHuOpt,IsPrintAsn=@IsPrintAsn,IsAsnPrinted=@IsAsnPrinted,IsPrintRec=@IsPrintRec,IsRecExceed=@IsRecExceed,IsRecFulfillUC=@IsRecFulfillUC,IsRecFifo=@IsRecFifo,IsAsnUniqueRec=@IsAsnUniqueRec,IsRecScanHu=@IsRecScanHu,RecGapTo=@RecGapTo,AsnTemplate=@AsnTemplate,RecTemplate=@RecTemplate,HuTemplate=@HuTemplate,EffDate=@EffDate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CloseReason=@CloseReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,IpNo=@IpNo
		WHERE IpNo=@IpNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=8
	BEGIN
		UPDATE ORD_IpMstr_8 SET Version=@Version,ExtIpNo=@ExtIpNo,GapIpNo=@GapIpNo,SeqNo=@SeqNo,Flow=@Flow,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,Status=@Status,DepartTime=@DepartTime,ArriveTime=@ArriveTime,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,IsAutoReceive=@IsAutoReceive,IsShipScanHu=@IsShipScanHu,CreateHuOpt=@CreateHuOpt,IsPrintAsn=@IsPrintAsn,IsAsnPrinted=@IsAsnPrinted,IsPrintRec=@IsPrintRec,IsRecExceed=@IsRecExceed,IsRecFulfillUC=@IsRecFulfillUC,IsRecFifo=@IsRecFifo,IsAsnUniqueRec=@IsAsnUniqueRec,IsRecScanHu=@IsRecScanHu,RecGapTo=@RecGapTo,AsnTemplate=@AsnTemplate,RecTemplate=@RecTemplate,HuTemplate=@HuTemplate,EffDate=@EffDate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CloseReason=@CloseReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,IpNo=@IpNo
		WHERE IpNo=@IpNo AND Version=@VersionBerfore
	END
	ELSE 
	BEGIN
		UPDATE ORD_IpMstr_0 SET Version=@Version,ExtIpNo=@ExtIpNo,GapIpNo=@GapIpNo,SeqNo=@SeqNo,Flow=@Flow,Type=@Type,OrderType=0,OrderSubType=@OrderSubType,QualityType=@QualityType,Status=@Status,DepartTime=@DepartTime,ArriveTime=@ArriveTime,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,IsAutoReceive=@IsAutoReceive,IsShipScanHu=@IsShipScanHu,CreateHuOpt=@CreateHuOpt,IsPrintAsn=@IsPrintAsn,IsAsnPrinted=@IsAsnPrinted,IsPrintRec=@IsPrintRec,IsRecExceed=@IsRecExceed,IsRecFulfillUC=@IsRecFulfillUC,IsRecFifo=@IsRecFifo,IsAsnUniqueRec=@IsAsnUniqueRec,IsRecScanHu=@IsRecScanHu,RecGapTo=@RecGapTo,AsnTemplate=@AsnTemplate,RecTemplate=@RecTemplate,HuTemplate=@HuTemplate,EffDate=@EffDate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CloseReason=@CloseReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,IpNo=@IpNo
		WHERE IpNo=@IpNo AND Version=@VersionBerfore
	END    
END
GO
