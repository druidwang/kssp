/****** Object:  StoredProcedure [dbo].[USP_Split_IpMstr_Insert]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_IpMstr_Insert')
	DROP PROCEDURE USP_Split_IpMstr_Insert
CREATE PROCEDURE [dbo].[USP_Split_IpMstr_Insert]
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
	@CreateUser int,
	@CreateUserNm varchar(100),
	@CreateDate datetime,
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime,
	@CloseDate datetime,
	@CloseUser int,
	@CloseUserNm varchar(100),
	@CloseReason varchar(256),
	@IsCheckPartyFromAuth bit,
	@IsCheckPartyToAuth bit,
	@IpNo varchar(4000)
)
AS
BEGIN
	IF @OrderType=1
	BEGIN
		INSERT INTO ORD_IpMstr_1(Version,ExtIpNo,GapIpNo,SeqNo,Flow,Type,OrderType,OrderSubType,QualityType,Status,DepartTime,ArriveTime,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,IsAutoReceive,IsShipScanHu,CreateHuOpt,IsPrintAsn,IsAsnPrinted,IsPrintRec,IsRecExceed,IsRecFulfillUC,IsRecFifo,IsAsnUniqueRec,IsRecScanHu,RecGapTo,AsnTemplate,RecTemplate,HuTemplate,EffDate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,CloseDate,CloseUser,CloseUserNm,CloseReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,IpNo)
		VALUES(@Version,@ExtIpNo,@GapIpNo,@SeqNo,@Flow,@Type,@OrderType,@OrderSubType,@QualityType,@Status,@DepartTime,@ArriveTime,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@IsAutoReceive,@IsShipScanHu,@CreateHuOpt,@IsPrintAsn,@IsAsnPrinted,@IsPrintRec,@IsRecExceed,@IsRecFulfillUC,@IsRecFifo,@IsAsnUniqueRec,@IsRecScanHu,@RecGapTo,@AsnTemplate,@RecTemplate,@HuTemplate,@EffDate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@CloseDate,@CloseUser,@CloseUserNm,@CloseReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@IpNo)
		SELECT SCOPE_IDENTITY()
	END	
	ELSE IF @OrderType=2
	BEGIN
		INSERT INTO ORD_IpMstr_2(Version,ExtIpNo,GapIpNo,SeqNo,Flow,Type,OrderType,OrderSubType,QualityType,Status,DepartTime,ArriveTime,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,IsAutoReceive,IsShipScanHu,CreateHuOpt,IsPrintAsn,IsAsnPrinted,IsPrintRec,IsRecExceed,IsRecFulfillUC,IsRecFifo,IsAsnUniqueRec,IsRecScanHu,RecGapTo,AsnTemplate,RecTemplate,HuTemplate,EffDate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,CloseDate,CloseUser,CloseUserNm,CloseReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,IpNo)
		VALUES(@Version,@ExtIpNo,@GapIpNo,@SeqNo,@Flow,@Type,@OrderType,@OrderSubType,@QualityType,@Status,@DepartTime,@ArriveTime,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@IsAutoReceive,@IsShipScanHu,@CreateHuOpt,@IsPrintAsn,@IsAsnPrinted,@IsPrintRec,@IsRecExceed,@IsRecFulfillUC,@IsRecFifo,@IsAsnUniqueRec,@IsRecScanHu,@RecGapTo,@AsnTemplate,@RecTemplate,@HuTemplate,@EffDate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@CloseDate,@CloseUser,@CloseUserNm,@CloseReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@IpNo)
		SELECT SCOPE_IDENTITY()
	END	
	ELSE IF @OrderType=3
	BEGIN
		INSERT INTO ORD_IpMstr_3(Version,ExtIpNo,GapIpNo,SeqNo,Flow,Type,OrderType,OrderSubType,QualityType,Status,DepartTime,ArriveTime,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,IsAutoReceive,IsShipScanHu,CreateHuOpt,IsPrintAsn,IsAsnPrinted,IsPrintRec,IsRecExceed,IsRecFulfillUC,IsRecFifo,IsAsnUniqueRec,IsRecScanHu,RecGapTo,AsnTemplate,RecTemplate,HuTemplate,EffDate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,CloseDate,CloseUser,CloseUserNm,CloseReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,IpNo)
		VALUES(@Version,@ExtIpNo,@GapIpNo,@SeqNo,@Flow,@Type,@OrderType,@OrderSubType,@QualityType,@Status,@DepartTime,@ArriveTime,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@IsAutoReceive,@IsShipScanHu,@CreateHuOpt,@IsPrintAsn,@IsAsnPrinted,@IsPrintRec,@IsRecExceed,@IsRecFulfillUC,@IsRecFifo,@IsAsnUniqueRec,@IsRecScanHu,@RecGapTo,@AsnTemplate,@RecTemplate,@HuTemplate,@EffDate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@CloseDate,@CloseUser,@CloseUserNm,@CloseReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@IpNo)
		SELECT SCOPE_IDENTITY()
	END	
	ELSE IF @OrderType=4
	BEGIN
		INSERT INTO ORD_IpMstr_4(Version,ExtIpNo,GapIpNo,SeqNo,Flow,Type,OrderType,OrderSubType,QualityType,Status,DepartTime,ArriveTime,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,IsAutoReceive,IsShipScanHu,CreateHuOpt,IsPrintAsn,IsAsnPrinted,IsPrintRec,IsRecExceed,IsRecFulfillUC,IsRecFifo,IsAsnUniqueRec,IsRecScanHu,RecGapTo,AsnTemplate,RecTemplate,HuTemplate,EffDate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,CloseDate,CloseUser,CloseUserNm,CloseReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,IpNo)
		VALUES(@Version,@ExtIpNo,@GapIpNo,@SeqNo,@Flow,@Type,@OrderType,@OrderSubType,@QualityType,@Status,@DepartTime,@ArriveTime,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@IsAutoReceive,@IsShipScanHu,@CreateHuOpt,@IsPrintAsn,@IsAsnPrinted,@IsPrintRec,@IsRecExceed,@IsRecFulfillUC,@IsRecFifo,@IsAsnUniqueRec,@IsRecScanHu,@RecGapTo,@AsnTemplate,@RecTemplate,@HuTemplate,@EffDate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@CloseDate,@CloseUser,@CloseUserNm,@CloseReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@IpNo)
		SELECT SCOPE_IDENTITY()
	END	
	ELSE IF @OrderType=5
	BEGIN
		INSERT INTO ORD_IpMstr_5(Version,ExtIpNo,GapIpNo,SeqNo,Flow,Type,OrderType,OrderSubType,QualityType,Status,DepartTime,ArriveTime,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,IsAutoReceive,IsShipScanHu,CreateHuOpt,IsPrintAsn,IsAsnPrinted,IsPrintRec,IsRecExceed,IsRecFulfillUC,IsRecFifo,IsAsnUniqueRec,IsRecScanHu,RecGapTo,AsnTemplate,RecTemplate,HuTemplate,EffDate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,CloseDate,CloseUser,CloseUserNm,CloseReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,IpNo)
		VALUES(@Version,@ExtIpNo,@GapIpNo,@SeqNo,@Flow,@Type,@OrderType,@OrderSubType,@QualityType,@Status,@DepartTime,@ArriveTime,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@IsAutoReceive,@IsShipScanHu,@CreateHuOpt,@IsPrintAsn,@IsAsnPrinted,@IsPrintRec,@IsRecExceed,@IsRecFulfillUC,@IsRecFifo,@IsAsnUniqueRec,@IsRecScanHu,@RecGapTo,@AsnTemplate,@RecTemplate,@HuTemplate,@EffDate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@CloseDate,@CloseUser,@CloseUserNm,@CloseReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@IpNo)
		SELECT SCOPE_IDENTITY()
	END	
	ELSE IF @OrderType=6
	BEGIN
		INSERT INTO ORD_IpMstr_6(Version,ExtIpNo,GapIpNo,SeqNo,Flow,Type,OrderType,OrderSubType,QualityType,Status,DepartTime,ArriveTime,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,IsAutoReceive,IsShipScanHu,CreateHuOpt,IsPrintAsn,IsAsnPrinted,IsPrintRec,IsRecExceed,IsRecFulfillUC,IsRecFifo,IsAsnUniqueRec,IsRecScanHu,RecGapTo,AsnTemplate,RecTemplate,HuTemplate,EffDate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,CloseDate,CloseUser,CloseUserNm,CloseReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,IpNo)
		VALUES(@Version,@ExtIpNo,@GapIpNo,@SeqNo,@Flow,@Type,@OrderType,@OrderSubType,@QualityType,@Status,@DepartTime,@ArriveTime,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@IsAutoReceive,@IsShipScanHu,@CreateHuOpt,@IsPrintAsn,@IsAsnPrinted,@IsPrintRec,@IsRecExceed,@IsRecFulfillUC,@IsRecFifo,@IsAsnUniqueRec,@IsRecScanHu,@RecGapTo,@AsnTemplate,@RecTemplate,@HuTemplate,@EffDate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@CloseDate,@CloseUser,@CloseUserNm,@CloseReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@IpNo)
		SELECT SCOPE_IDENTITY()
	END	
	ELSE IF @OrderType=7
	BEGIN
		INSERT INTO ORD_IpMstr_7(Version,ExtIpNo,GapIpNo,SeqNo,Flow,Type,OrderType,OrderSubType,QualityType,Status,DepartTime,ArriveTime,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,IsAutoReceive,IsShipScanHu,CreateHuOpt,IsPrintAsn,IsAsnPrinted,IsPrintRec,IsRecExceed,IsRecFulfillUC,IsRecFifo,IsAsnUniqueRec,IsRecScanHu,RecGapTo,AsnTemplate,RecTemplate,HuTemplate,EffDate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,CloseDate,CloseUser,CloseUserNm,CloseReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,IpNo)
		VALUES(@Version,@ExtIpNo,@GapIpNo,@SeqNo,@Flow,@Type,@OrderType,@OrderSubType,@QualityType,@Status,@DepartTime,@ArriveTime,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@IsAutoReceive,@IsShipScanHu,@CreateHuOpt,@IsPrintAsn,@IsAsnPrinted,@IsPrintRec,@IsRecExceed,@IsRecFulfillUC,@IsRecFifo,@IsAsnUniqueRec,@IsRecScanHu,@RecGapTo,@AsnTemplate,@RecTemplate,@HuTemplate,@EffDate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@CloseDate,@CloseUser,@CloseUserNm,@CloseReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@IpNo)
		SELECT SCOPE_IDENTITY()
	END	
	ELSE IF @OrderType=8
	BEGIN
		INSERT INTO ORD_IpMstr_8(Version,ExtIpNo,GapIpNo,SeqNo,Flow,Type,OrderType,OrderSubType,QualityType,Status,DepartTime,ArriveTime,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,IsAutoReceive,IsShipScanHu,CreateHuOpt,IsPrintAsn,IsAsnPrinted,IsPrintRec,IsRecExceed,IsRecFulfillUC,IsRecFifo,IsAsnUniqueRec,IsRecScanHu,RecGapTo,AsnTemplate,RecTemplate,HuTemplate,EffDate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,CloseDate,CloseUser,CloseUserNm,CloseReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,IpNo)
		VALUES(@Version,@ExtIpNo,@GapIpNo,@SeqNo,@Flow,@Type,@OrderType,@OrderSubType,@QualityType,@Status,@DepartTime,@ArriveTime,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@IsAutoReceive,@IsShipScanHu,@CreateHuOpt,@IsPrintAsn,@IsAsnPrinted,@IsPrintRec,@IsRecExceed,@IsRecFulfillUC,@IsRecFifo,@IsAsnUniqueRec,@IsRecScanHu,@RecGapTo,@AsnTemplate,@RecTemplate,@HuTemplate,@EffDate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@CloseDate,@CloseUser,@CloseUserNm,@CloseReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@IpNo)
		SELECT SCOPE_IDENTITY()
	END			
	ELSE
	BEGIN
		INSERT INTO ORD_IpMstr_0(Version,ExtIpNo,GapIpNo,SeqNo,Flow,Type,OrderType,OrderSubType,QualityType,Status,DepartTime,ArriveTime,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,IsAutoReceive,IsShipScanHu,CreateHuOpt,IsPrintAsn,IsAsnPrinted,IsPrintRec,IsRecExceed,IsRecFulfillUC,IsRecFifo,IsAsnUniqueRec,IsRecScanHu,RecGapTo,AsnTemplate,RecTemplate,HuTemplate,EffDate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,CloseDate,CloseUser,CloseUserNm,CloseReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,IpNo)
		VALUES(@Version,@ExtIpNo,@GapIpNo,@SeqNo,@Flow,@Type,0,@OrderSubType,@QualityType,@Status,@DepartTime,@ArriveTime,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@IsAutoReceive,@IsShipScanHu,@CreateHuOpt,@IsPrintAsn,@IsAsnPrinted,@IsPrintRec,@IsRecExceed,@IsRecFulfillUC,@IsRecFifo,@IsAsnUniqueRec,@IsRecScanHu,@RecGapTo,@AsnTemplate,@RecTemplate,@HuTemplate,@EffDate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@CloseDate,@CloseUser,@CloseUserNm,@CloseReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@IpNo)
		SELECT SCOPE_IDENTITY()
	END							
END
GO