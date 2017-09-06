/****** Object:  StoredProcedure [dbo].[USP_Split_RecMstr_UPDATE]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_RecMstr_UPDATE')
	DROP PROCEDURE USP_Split_RecMstr_UPDATE
CREATE PROCEDURE [dbo].[USP_Split_RecMstr_UPDATE]
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
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime,
	@IsCheckPartyFromAuth bit,
	@IsCheckPartyToAuth bit,
	@RecNo varchar(4000),
	@VersionBerfore int
)
AS
BEGIN
	IF @OrderType=1
	BEGIN
		UPDATE ORD_RecMstr_1 SET Version=@Version,ExtRecNo=@ExtRecNo,IpNo=@IpNo,SeqNo=@SeqNo,Flow=@Flow,Status=@Status,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,EffDate=@EffDate,IsPrintRec=@IsPrintRec,IsRecPrinted=@IsRecPrinted,CreateHuOpt=@CreateHuOpt,IsRecScanHu=@IsRecScanHu,RecTemplate=@RecTemplate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,RecNo=@RecNo
		WHERE RecNo=@RecNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=2
	BEGIN
		UPDATE ORD_RecMstr_2 SET Version=@Version,ExtRecNo=@ExtRecNo,IpNo=@IpNo,SeqNo=@SeqNo,Flow=@Flow,Status=@Status,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,EffDate=@EffDate,IsPrintRec=@IsPrintRec,IsRecPrinted=@IsRecPrinted,CreateHuOpt=@CreateHuOpt,IsRecScanHu=@IsRecScanHu,RecTemplate=@RecTemplate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,RecNo=@RecNo
		WHERE RecNo=@RecNo AND Version=@VersionBerfore
	END	
	ELSE IF @OrderType=3
	BEGIN
		UPDATE ORD_RecMstr_3 SET Version=@Version,ExtRecNo=@ExtRecNo,IpNo=@IpNo,SeqNo=@SeqNo,Flow=@Flow,Status=@Status,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,EffDate=@EffDate,IsPrintRec=@IsPrintRec,IsRecPrinted=@IsRecPrinted,CreateHuOpt=@CreateHuOpt,IsRecScanHu=@IsRecScanHu,RecTemplate=@RecTemplate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,RecNo=@RecNo
		WHERE RecNo=@RecNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=4
	BEGIN
		UPDATE ORD_RecMstr_4 SET Version=@Version,ExtRecNo=@ExtRecNo,IpNo=@IpNo,SeqNo=@SeqNo,Flow=@Flow,Status=@Status,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,EffDate=@EffDate,IsPrintRec=@IsPrintRec,IsRecPrinted=@IsRecPrinted,CreateHuOpt=@CreateHuOpt,IsRecScanHu=@IsRecScanHu,RecTemplate=@RecTemplate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,RecNo=@RecNo
		WHERE RecNo=@RecNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=5
	BEGIN
		UPDATE ORD_RecMstr_5 SET Version=@Version,ExtRecNo=@ExtRecNo,IpNo=@IpNo,SeqNo=@SeqNo,Flow=@Flow,Status=@Status,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,EffDate=@EffDate,IsPrintRec=@IsPrintRec,IsRecPrinted=@IsRecPrinted,CreateHuOpt=@CreateHuOpt,IsRecScanHu=@IsRecScanHu,RecTemplate=@RecTemplate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,RecNo=@RecNo
		WHERE RecNo=@RecNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=6
	BEGIN
		UPDATE ORD_RecMstr_6 SET Version=@Version,ExtRecNo=@ExtRecNo,IpNo=@IpNo,SeqNo=@SeqNo,Flow=@Flow,Status=@Status,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,EffDate=@EffDate,IsPrintRec=@IsPrintRec,IsRecPrinted=@IsRecPrinted,CreateHuOpt=@CreateHuOpt,IsRecScanHu=@IsRecScanHu,RecTemplate=@RecTemplate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,RecNo=@RecNo
		WHERE RecNo=@RecNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=7
	BEGIN
		UPDATE ORD_RecMstr_7 SET Version=@Version,ExtRecNo=@ExtRecNo,IpNo=@IpNo,SeqNo=@SeqNo,Flow=@Flow,Status=@Status,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,EffDate=@EffDate,IsPrintRec=@IsPrintRec,IsRecPrinted=@IsRecPrinted,CreateHuOpt=@CreateHuOpt,IsRecScanHu=@IsRecScanHu,RecTemplate=@RecTemplate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,RecNo=@RecNo
		WHERE RecNo=@RecNo AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=8
	BEGIN
		UPDATE ORD_RecMstr_8 SET Version=@Version,ExtRecNo=@ExtRecNo,IpNo=@IpNo,SeqNo=@SeqNo,Flow=@Flow,Status=@Status,Type=@Type,OrderType=@OrderType,OrderSubType=@OrderSubType,QualityType=@QualityType,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,EffDate=@EffDate,IsPrintRec=@IsPrintRec,IsRecPrinted=@IsRecPrinted,CreateHuOpt=@CreateHuOpt,IsRecScanHu=@IsRecScanHu,RecTemplate=@RecTemplate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,RecNo=@RecNo
		WHERE RecNo=@RecNo AND Version=@VersionBerfore
	END
	ELSE
	BEGIN
		UPDATE ORD_RecMstr_0 SET Version=@Version,ExtRecNo=@ExtRecNo,IpNo=@IpNo,SeqNo=@SeqNo,Flow=@Flow,Status=@Status,Type=@Type,OrderType=0,OrderSubType=@OrderSubType,QualityType=@QualityType,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipTo=@ShipTo,ShipToAddr=@ShipToAddr,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Dock=@Dock,EffDate=@EffDate,IsPrintRec=@IsPrintRec,IsRecPrinted=@IsRecPrinted,CreateHuOpt=@CreateHuOpt,IsRecScanHu=@IsRecScanHu,RecTemplate=@RecTemplate,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,RecNo=@RecNo
		WHERE RecNo=@RecNo AND Version=@VersionBerfore
	END	
END
GO