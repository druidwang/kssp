/****** Object:  StoredProcedure [dbo].[USP_Split_RecMstr_INSERT]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_RecMstr_INSERT')
	DROP PROCEDURE USP_Split_RecMstr_INSERT
CREATE PROCEDURE [dbo].[USP_Split_RecMstr_INSERT]
(
	@Version int,
	@ExtRecNo varchar(50),
	@IpNo varchar(50),
	@SeqNo varchar(50),
	@Flow varchar(50),
	@Status tinyint,
	@Type tinyint,
	@OrderType tinyint,
	@OrderSubType tinyint,
	@QualityType tinyint,
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
	@EffDate datetime,
	@IsPrintRec bit,
	@IsRecPrinted bit,
	@CreateHuOpt bit,
	@IsRecScanHu bit,
	@RecTemplate varchar(100),
	@WMSNo varchar(50),
	@CreateUser int,
	@CreateUserNm varchar(100),
	@CreateDate datetime,
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime,
	@IsCheckPartyFromAuth bit,
	@IsCheckPartyToAuth bit,
	@RecNo varchar(4000)
)
AS
BEGIN
	IF @OrderType=1
	BEGIN
		INSERT INTO ORD_RecMstr_1(Version,ExtRecNo,IpNo,SeqNo,Flow,Status,Type,OrderType,OrderSubType,QualityType,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,EffDate,IsPrintRec,IsRecPrinted,CreateHuOpt,IsRecScanHu,RecTemplate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,IsCheckPartyFromAuth,IsCheckPartyToAuth,RecNo)
		VALUES(@Version,@ExtRecNo,@IpNo,@SeqNo,@Flow,@Status,@Type,@OrderType,@OrderSubType,@QualityType,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@EffDate,@IsPrintRec,@IsRecPrinted,@CreateHuOpt,@IsRecScanHu,@RecTemplate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@RecNo)
	END	
	ELSE IF @OrderType=2
	BEGIN
		INSERT INTO ORD_RecMstr_2(Version,ExtRecNo,IpNo,SeqNo,Flow,Status,Type,OrderType,OrderSubType,QualityType,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,EffDate,IsPrintRec,IsRecPrinted,CreateHuOpt,IsRecScanHu,RecTemplate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,IsCheckPartyFromAuth,IsCheckPartyToAuth,RecNo)
		VALUES(@Version,@ExtRecNo,@IpNo,@SeqNo,@Flow,@Status,@Type,@OrderType,@OrderSubType,@QualityType,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@EffDate,@IsPrintRec,@IsRecPrinted,@CreateHuOpt,@IsRecScanHu,@RecTemplate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@RecNo)
	END	
	ELSE IF @OrderType=3
	BEGIN
		INSERT INTO ORD_RecMstr_3(Version,ExtRecNo,IpNo,SeqNo,Flow,Status,Type,OrderType,OrderSubType,QualityType,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,EffDate,IsPrintRec,IsRecPrinted,CreateHuOpt,IsRecScanHu,RecTemplate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,IsCheckPartyFromAuth,IsCheckPartyToAuth,RecNo)
		VALUES(@Version,@ExtRecNo,@IpNo,@SeqNo,@Flow,@Status,@Type,@OrderType,@OrderSubType,@QualityType,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@EffDate,@IsPrintRec,@IsRecPrinted,@CreateHuOpt,@IsRecScanHu,@RecTemplate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@RecNo)
	END	
	ELSE IF @OrderType=4
	BEGIN
		INSERT INTO ORD_RecMstr_4(Version,ExtRecNo,IpNo,SeqNo,Flow,Status,Type,OrderType,OrderSubType,QualityType,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,EffDate,IsPrintRec,IsRecPrinted,CreateHuOpt,IsRecScanHu,RecTemplate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,IsCheckPartyFromAuth,IsCheckPartyToAuth,RecNo)
		VALUES(@Version,@ExtRecNo,@IpNo,@SeqNo,@Flow,@Status,@Type,@OrderType,@OrderSubType,@QualityType,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@EffDate,@IsPrintRec,@IsRecPrinted,@CreateHuOpt,@IsRecScanHu,@RecTemplate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@RecNo)
	END	
	ELSE IF @OrderType=5
	BEGIN
		INSERT INTO ORD_RecMstr_5(Version,ExtRecNo,IpNo,SeqNo,Flow,Status,Type,OrderType,OrderSubType,QualityType,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,EffDate,IsPrintRec,IsRecPrinted,CreateHuOpt,IsRecScanHu,RecTemplate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,IsCheckPartyFromAuth,IsCheckPartyToAuth,RecNo)
		VALUES(@Version,@ExtRecNo,@IpNo,@SeqNo,@Flow,@Status,@Type,@OrderType,@OrderSubType,@QualityType,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@EffDate,@IsPrintRec,@IsRecPrinted,@CreateHuOpt,@IsRecScanHu,@RecTemplate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@RecNo)
	END	
	ELSE IF @OrderType=6
	BEGIN
		INSERT INTO ORD_RecMstr_6(Version,ExtRecNo,IpNo,SeqNo,Flow,Status,Type,OrderType,OrderSubType,QualityType,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,EffDate,IsPrintRec,IsRecPrinted,CreateHuOpt,IsRecScanHu,RecTemplate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,IsCheckPartyFromAuth,IsCheckPartyToAuth,RecNo)
		VALUES(@Version,@ExtRecNo,@IpNo,@SeqNo,@Flow,@Status,@Type,@OrderType,@OrderSubType,@QualityType,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@EffDate,@IsPrintRec,@IsRecPrinted,@CreateHuOpt,@IsRecScanHu,@RecTemplate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@RecNo)
	END	
	ELSE IF @OrderType=7
	BEGIN
		INSERT INTO ORD_RecMstr_7(Version,ExtRecNo,IpNo,SeqNo,Flow,Status,Type,OrderType,OrderSubType,QualityType,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,EffDate,IsPrintRec,IsRecPrinted,CreateHuOpt,IsRecScanHu,RecTemplate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,IsCheckPartyFromAuth,IsCheckPartyToAuth,RecNo)
		VALUES(@Version,@ExtRecNo,@IpNo,@SeqNo,@Flow,@Status,@Type,@OrderType,@OrderSubType,@QualityType,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@EffDate,@IsPrintRec,@IsRecPrinted,@CreateHuOpt,@IsRecScanHu,@RecTemplate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@RecNo)
	END	
	ELSE IF @OrderType=8
	BEGIN
		INSERT INTO ORD_RecMstr_8(Version,ExtRecNo,IpNo,SeqNo,Flow,Status,Type,OrderType,OrderSubType,QualityType,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,EffDate,IsPrintRec,IsRecPrinted,CreateHuOpt,IsRecScanHu,RecTemplate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,IsCheckPartyFromAuth,IsCheckPartyToAuth,RecNo)
		VALUES(@Version,@ExtRecNo,@IpNo,@SeqNo,@Flow,@Status,@Type,@OrderType,@OrderSubType,@QualityType,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@EffDate,@IsPrintRec,@IsRecPrinted,@CreateHuOpt,@IsRecScanHu,@RecTemplate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@RecNo)
	END	
	ELSE
	BEGIN
		INSERT INTO ORD_RecMstr_0(Version,ExtRecNo,IpNo,SeqNo,Flow,Status,Type,OrderType,OrderSubType,QualityType,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipTo,ShipToAddr,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Dock,EffDate,IsPrintRec,IsRecPrinted,CreateHuOpt,IsRecScanHu,RecTemplate,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,IsCheckPartyFromAuth,IsCheckPartyToAuth,RecNo)
		VALUES(@Version,@ExtRecNo,@IpNo,@SeqNo,@Flow,@Status,@Type,0,@OrderSubType,@QualityType,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipTo,@ShipToAddr,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Dock,@EffDate,@IsPrintRec,@IsRecPrinted,@CreateHuOpt,@IsRecScanHu,@RecTemplate,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@RecNo)
	END		
END
GO