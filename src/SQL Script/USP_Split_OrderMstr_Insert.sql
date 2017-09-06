/****** Object:  StoredProcedure [dbo].[USP_Split_OrderMstr_Insert]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_OrderMstr_Insert')
	DROP PROCEDURE USP_Split_OrderMstr_Insert
CREATE PROCEDURE [dbo].[USP_Split_OrderMstr_Insert]
(
	@Version int,
	@Flow varchar(50),
	@FlowDesc  varchar(100),
	@TraceCode varchar(50),
	@RefOrderNo varchar(50),
	@ExtOrderNo varchar(50),
	@Type tinyint,
	@SubType tinyint,
	@QualityType tinyint,
	@StartTime datetime,
	@WindowTime datetime,
	@IsPlanPause bit,
	@PauseSeq int,
	@IsPause bit,
	@PauseTime datetime,
	@IsPLPause bit,
	@EffDate datetime,
	@Priority tinyint,
	@Status tinyint,
	@Seq bigint,
	@SapSeq bigint,
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
	@ShipToAddr varchar(256),
	@ShipTo varchar(50),
	@ShipToTel varchar(50),
	@ShipToCell varchar(50),
	@ShipToFax varchar(50),
	@ShipToContact varchar(50),
	@Shift varchar(50),
	@LocFrom varchar(50),
	@LocFromNm varchar(100),
	@LocTo varchar(50),
	@LocToNm varchar(100),
	@IsInspect bit,
	@BillAddr varchar(50),
	@BillAddrDesc varchar(256),
	@PriceList varchar(50),
	@Currency varchar(50),
	@Dock varchar(100),
	@Routing varchar(50),
	@CurtOp int,
	@IsAutoRelease bit,
	@IsAutoStart bit,
	@IsAutoShip bit,
	@IsAutoReceive bit,
	@IsAutoBill bit,
	@IsManualCreateDet bit,
	@IsListPrice bit,
	@IsPrintOrder bit,
	@IsOrderPrinted bit,
	@IsPrintAsn bit,
	@IsPrintRec bit,
	@IsShipExceed bit,
	@IsRecExceed bit,
	@IsOrderFulfillUC bit,
	@IsShipFulfillUC bit,
	@IsRecFulfillUC bit,
	@IsShipScanHu bit,
	@IsRecScanHu bit,
	@IsCreatePL bit,
	@IsPLCreate bit,
	@IsShipFifo bit,
	@IsRecFifo bit,
	@IsShipByOrder bit,
	@IsOpenOrder bit,
	@IsAsnUniqueRec bit,
	@RecGapTo tinyint,
	@RecTemplate varchar(100),
	@OrderTemplate varchar(100),
	@AsnTemplate varchar(100),
	@HuTemplate varchar(100),
	@BillTerm tinyint,
	@CreateHuOpt tinyint,
	@ReCalculatePriceOpt tinyint,
	@WMSNo varchar(50),
	@CreateUser int,
	@CreateUserNm varchar(100),
	@CreateDate datetime,
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime,
	@ReleaseDate datetime,
	@ReleaseUser int,
	@ReleaseUserNm varchar(100),
	@StartDate datetime,
	@StartUser int,
	@StartUserNm varchar(100),
	@CompleteDate datetime,
	@CompleteUser int,
	@CompleteUserNm varchar(100),
	@CloseDate datetime,
	@CloseUser int,
	@CloseUserNm varchar(100),
	@CancelDate datetime,
	@CancelUser int,
	@CancelUserNm varchar(100),
	@CancelReason varchar(256),
	@IsCheckPartyFromAuth bit,
	@IsCheckPartyToAuth bit,
	@OrderStrategy tinyint,
	@ProdLineFact varchar(50),
	@IsQuick bit,
	@PickStrategy varchar(50),
	@ExtraDmdSource varchar(256),
	@OrderNo varchar(4000)
)
AS
BEGIN
	
	IF @Type=1
	BEGIN
		INSERT INTO ORD_OrderMstr_1(Version,Flow,FlowDesc,TraceCode,RefOrderNo,ExtOrderNo,Type,SubType,QualityType,StartTime,WindowTime,IsPlanPause,PauseSeq,IsPause,PauseTime,IsPLPause,EffDate,Priority,Status,Seq,SapSeq,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipToAddr,ShipTo,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Shift,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,Currency,Dock,Routing,CurtOp,IsAutoRelease,IsAutoStart,IsAutoShip,IsAutoReceive,IsAutoBill,IsManualCreateDet,IsListPrice,IsPrintOrder,IsOrderPrinted,IsPrintAsn,IsPrintRec,IsShipExceed,IsRecExceed,IsOrderFulfillUC,IsShipFulfillUC,IsRecFulfillUC,IsShipScanHu,IsRecScanHu,IsCreatePL,IsPLCreate,IsShipFifo,IsRecFifo,IsShipByOrder,IsOpenOrder,IsAsnUniqueRec,RecGapTo,RecTemplate,OrderTemplate,AsnTemplate,HuTemplate,BillTerm,CreateHuOpt,ReCalculatePriceOpt,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,ReleaseDate,ReleaseUser,ReleaseUserNm,StartDate,StartUser,StartUserNm,CompleteDate,CompleteUser,CompleteUserNm,CloseDate,CloseUser,CloseUserNm,CancelDate,CancelUser,CancelUserNm,CancelReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,OrderStrategy,ProdLineFact,IsQuick,PickStrategy,ExtraDmdSource,OrderNo) 
		VALUES(@Version,@Flow,@FlowDesc,@TraceCode,@RefOrderNo,@ExtOrderNo,@Type,@SubType,@QualityType,@StartTime,@WindowTime,@IsPlanPause,@PauseSeq,@IsPause,@PauseTime,@IsPLPause,@EffDate,@Priority,@Status,@Seq,@SapSeq,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipToAddr,@ShipTo,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Shift,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@Currency,@Dock,@Routing,@CurtOp,@IsAutoRelease,@IsAutoStart,@IsAutoShip,@IsAutoReceive,@IsAutoBill,@IsManualCreateDet,@IsListPrice,@IsPrintOrder,@IsOrderPrinted,@IsPrintAsn,@IsPrintRec,@IsShipExceed,@IsRecExceed,@IsOrderFulfillUC,@IsShipFulfillUC,@IsRecFulfillUC,@IsShipScanHu,@IsRecScanHu,@IsCreatePL,@IsPLCreate,@IsShipFifo,@IsRecFifo,@IsShipByOrder,@IsOpenOrder,@IsAsnUniqueRec,@RecGapTo,@RecTemplate,@OrderTemplate,@AsnTemplate,@HuTemplate,@BillTerm,@CreateHuOpt,@ReCalculatePriceOpt,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@ReleaseDate,@ReleaseUser,@ReleaseUserNm,@StartDate,@StartUser,@StartUserNm,@CompleteDate,@CompleteUser,@CompleteUserNm,@CloseDate,@CloseUser,@CloseUserNm,@CancelDate,@CancelUser,@CancelUserNm,@CancelReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@OrderStrategy,@ProdLineFact,@IsQuick,@PickStrategy,@ExtraDmdSource,@OrderNo)
	END	
	ELSE IF @Type=2
	BEGIN
		INSERT INTO ORD_OrderMstr_2(Version,Flow,FlowDesc,TraceCode,RefOrderNo,ExtOrderNo,Type,SubType,QualityType,StartTime,WindowTime,IsPlanPause,PauseSeq,IsPause,PauseTime,IsPLPause,EffDate,Priority,Status,Seq,SapSeq,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipToAddr,ShipTo,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Shift,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,Currency,Dock,Routing,CurtOp,IsAutoRelease,IsAutoStart,IsAutoShip,IsAutoReceive,IsAutoBill,IsManualCreateDet,IsListPrice,IsPrintOrder,IsOrderPrinted,IsPrintAsn,IsPrintRec,IsShipExceed,IsRecExceed,IsOrderFulfillUC,IsShipFulfillUC,IsRecFulfillUC,IsShipScanHu,IsRecScanHu,IsCreatePL,IsPLCreate,IsShipFifo,IsRecFifo,IsShipByOrder,IsOpenOrder,IsAsnUniqueRec,RecGapTo,RecTemplate,OrderTemplate,AsnTemplate,HuTemplate,BillTerm,CreateHuOpt,ReCalculatePriceOpt,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,ReleaseDate,ReleaseUser,ReleaseUserNm,StartDate,StartUser,StartUserNm,CompleteDate,CompleteUser,CompleteUserNm,CloseDate,CloseUser,CloseUserNm,CancelDate,CancelUser,CancelUserNm,CancelReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,OrderStrategy,ProdLineFact,IsQuick,PickStrategy,ExtraDmdSource,OrderNo) 
		VALUES(@Version,@Flow,@FlowDesc,@TraceCode,@RefOrderNo,@ExtOrderNo,@Type,@SubType,@QualityType,@StartTime,@WindowTime,@IsPlanPause,@PauseSeq,@IsPause,@PauseTime,@IsPLPause,@EffDate,@Priority,@Status,@Seq,@SapSeq,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipToAddr,@ShipTo,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Shift,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@Currency,@Dock,@Routing,@CurtOp,@IsAutoRelease,@IsAutoStart,@IsAutoShip,@IsAutoReceive,@IsAutoBill,@IsManualCreateDet,@IsListPrice,@IsPrintOrder,@IsOrderPrinted,@IsPrintAsn,@IsPrintRec,@IsShipExceed,@IsRecExceed,@IsOrderFulfillUC,@IsShipFulfillUC,@IsRecFulfillUC,@IsShipScanHu,@IsRecScanHu,@IsCreatePL,@IsPLCreate,@IsShipFifo,@IsRecFifo,@IsShipByOrder,@IsOpenOrder,@IsAsnUniqueRec,@RecGapTo,@RecTemplate,@OrderTemplate,@AsnTemplate,@HuTemplate,@BillTerm,@CreateHuOpt,@ReCalculatePriceOpt,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@ReleaseDate,@ReleaseUser,@ReleaseUserNm,@StartDate,@StartUser,@StartUserNm,@CompleteDate,@CompleteUser,@CompleteUserNm,@CloseDate,@CloseUser,@CloseUserNm,@CancelDate,@CancelUser,@CancelUserNm,@CancelReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@OrderStrategy,@ProdLineFact,@IsQuick,@PickStrategy,@ExtraDmdSource,@OrderNo)
	END	
	ELSE IF @Type=3
	BEGIN
		INSERT INTO ORD_OrderMstr_3(Version,Flow,FlowDesc,TraceCode,RefOrderNo,ExtOrderNo,Type,SubType,QualityType,StartTime,WindowTime,IsPlanPause,PauseSeq,IsPause,PauseTime,IsPLPause,EffDate,Priority,Status,Seq,SapSeq,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipToAddr,ShipTo,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Shift,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,Currency,Dock,Routing,CurtOp,IsAutoRelease,IsAutoStart,IsAutoShip,IsAutoReceive,IsAutoBill,IsManualCreateDet,IsListPrice,IsPrintOrder,IsOrderPrinted,IsPrintAsn,IsPrintRec,IsShipExceed,IsRecExceed,IsOrderFulfillUC,IsShipFulfillUC,IsRecFulfillUC,IsShipScanHu,IsRecScanHu,IsCreatePL,IsPLCreate,IsShipFifo,IsRecFifo,IsShipByOrder,IsOpenOrder,IsAsnUniqueRec,RecGapTo,RecTemplate,OrderTemplate,AsnTemplate,HuTemplate,BillTerm,CreateHuOpt,ReCalculatePriceOpt,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,ReleaseDate,ReleaseUser,ReleaseUserNm,StartDate,StartUser,StartUserNm,CompleteDate,CompleteUser,CompleteUserNm,CloseDate,CloseUser,CloseUserNm,CancelDate,CancelUser,CancelUserNm,CancelReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,OrderStrategy,ProdLineFact,IsQuick,PickStrategy,ExtraDmdSource,OrderNo) 
		VALUES(@Version,@Flow,@FlowDesc,@TraceCode,@RefOrderNo,@ExtOrderNo,@Type,@SubType,@QualityType,@StartTime,@WindowTime,@IsPlanPause,@PauseSeq,@IsPause,@PauseTime,@IsPLPause,@EffDate,@Priority,@Status,@Seq,@SapSeq,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipToAddr,@ShipTo,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Shift,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@Currency,@Dock,@Routing,@CurtOp,@IsAutoRelease,@IsAutoStart,@IsAutoShip,@IsAutoReceive,@IsAutoBill,@IsManualCreateDet,@IsListPrice,@IsPrintOrder,@IsOrderPrinted,@IsPrintAsn,@IsPrintRec,@IsShipExceed,@IsRecExceed,@IsOrderFulfillUC,@IsShipFulfillUC,@IsRecFulfillUC,@IsShipScanHu,@IsRecScanHu,@IsCreatePL,@IsPLCreate,@IsShipFifo,@IsRecFifo,@IsShipByOrder,@IsOpenOrder,@IsAsnUniqueRec,@RecGapTo,@RecTemplate,@OrderTemplate,@AsnTemplate,@HuTemplate,@BillTerm,@CreateHuOpt,@ReCalculatePriceOpt,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@ReleaseDate,@ReleaseUser,@ReleaseUserNm,@StartDate,@StartUser,@StartUserNm,@CompleteDate,@CompleteUser,@CompleteUserNm,@CloseDate,@CloseUser,@CloseUserNm,@CancelDate,@CancelUser,@CancelUserNm,@CancelReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@OrderStrategy,@ProdLineFact,@IsQuick,@PickStrategy,@ExtraDmdSource,@OrderNo)
	END	
	ELSE IF @Type=4
	BEGIN
		INSERT INTO ORD_OrderMstr_4(Version,Flow,FlowDesc,TraceCode,RefOrderNo,ExtOrderNo,Type,SubType,QualityType,StartTime,WindowTime,IsPlanPause,PauseSeq,IsPause,PauseTime,IsPLPause,EffDate,Priority,Status,Seq,SapSeq,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipToAddr,ShipTo,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Shift,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,Currency,Dock,Routing,CurtOp,IsAutoRelease,IsAutoStart,IsAutoShip,IsAutoReceive,IsAutoBill,IsManualCreateDet,IsListPrice,IsPrintOrder,IsOrderPrinted,IsPrintAsn,IsPrintRec,IsShipExceed,IsRecExceed,IsOrderFulfillUC,IsShipFulfillUC,IsRecFulfillUC,IsShipScanHu,IsRecScanHu,IsCreatePL,IsPLCreate,IsShipFifo,IsRecFifo,IsShipByOrder,IsOpenOrder,IsAsnUniqueRec,RecGapTo,RecTemplate,OrderTemplate,AsnTemplate,HuTemplate,BillTerm,CreateHuOpt,ReCalculatePriceOpt,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,ReleaseDate,ReleaseUser,ReleaseUserNm,StartDate,StartUser,StartUserNm,CompleteDate,CompleteUser,CompleteUserNm,CloseDate,CloseUser,CloseUserNm,CancelDate,CancelUser,CancelUserNm,CancelReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,OrderStrategy,ProdLineFact,IsQuick,PickStrategy,ExtraDmdSource,OrderNo) 
		VALUES(@Version,@Flow,@FlowDesc,@TraceCode,@RefOrderNo,@ExtOrderNo,@Type,@SubType,@QualityType,@StartTime,@WindowTime,@IsPlanPause,@PauseSeq,@IsPause,@PauseTime,@IsPLPause,@EffDate,@Priority,@Status,@Seq,@SapSeq,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipToAddr,@ShipTo,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Shift,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@Currency,@Dock,@Routing,@CurtOp,@IsAutoRelease,@IsAutoStart,@IsAutoShip,@IsAutoReceive,@IsAutoBill,@IsManualCreateDet,@IsListPrice,@IsPrintOrder,@IsOrderPrinted,@IsPrintAsn,@IsPrintRec,@IsShipExceed,@IsRecExceed,@IsOrderFulfillUC,@IsShipFulfillUC,@IsRecFulfillUC,@IsShipScanHu,@IsRecScanHu,@IsCreatePL,@IsPLCreate,@IsShipFifo,@IsRecFifo,@IsShipByOrder,@IsOpenOrder,@IsAsnUniqueRec,@RecGapTo,@RecTemplate,@OrderTemplate,@AsnTemplate,@HuTemplate,@BillTerm,@CreateHuOpt,@ReCalculatePriceOpt,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@ReleaseDate,@ReleaseUser,@ReleaseUserNm,@StartDate,@StartUser,@StartUserNm,@CompleteDate,@CompleteUser,@CompleteUserNm,@CloseDate,@CloseUser,@CloseUserNm,@CancelDate,@CancelUser,@CancelUserNm,@CancelReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@OrderStrategy,@ProdLineFact,@IsQuick,@PickStrategy,@ExtraDmdSource,@OrderNo)
	END	
	ELSE IF @Type=5
	BEGIN
		INSERT INTO ORD_OrderMstr_5(Version,Flow,FlowDesc,TraceCode,RefOrderNo,ExtOrderNo,Type,SubType,QualityType,StartTime,WindowTime,IsPlanPause,PauseSeq,IsPause,PauseTime,IsPLPause,EffDate,Priority,Status,Seq,SapSeq,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipToAddr,ShipTo,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Shift,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,Currency,Dock,Routing,CurtOp,IsAutoRelease,IsAutoStart,IsAutoShip,IsAutoReceive,IsAutoBill,IsManualCreateDet,IsListPrice,IsPrintOrder,IsOrderPrinted,IsPrintAsn,IsPrintRec,IsShipExceed,IsRecExceed,IsOrderFulfillUC,IsShipFulfillUC,IsRecFulfillUC,IsShipScanHu,IsRecScanHu,IsCreatePL,IsPLCreate,IsShipFifo,IsRecFifo,IsShipByOrder,IsOpenOrder,IsAsnUniqueRec,RecGapTo,RecTemplate,OrderTemplate,AsnTemplate,HuTemplate,BillTerm,CreateHuOpt,ReCalculatePriceOpt,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,ReleaseDate,ReleaseUser,ReleaseUserNm,StartDate,StartUser,StartUserNm,CompleteDate,CompleteUser,CompleteUserNm,CloseDate,CloseUser,CloseUserNm,CancelDate,CancelUser,CancelUserNm,CancelReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,OrderStrategy,ProdLineFact,IsQuick,PickStrategy,ExtraDmdSource,OrderNo) 
		VALUES(@Version,@Flow,@FlowDesc,@TraceCode,@RefOrderNo,@ExtOrderNo,@Type,@SubType,@QualityType,@StartTime,@WindowTime,@IsPlanPause,@PauseSeq,@IsPause,@PauseTime,@IsPLPause,@EffDate,@Priority,@Status,@Seq,@SapSeq,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipToAddr,@ShipTo,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Shift,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@Currency,@Dock,@Routing,@CurtOp,@IsAutoRelease,@IsAutoStart,@IsAutoShip,@IsAutoReceive,@IsAutoBill,@IsManualCreateDet,@IsListPrice,@IsPrintOrder,@IsOrderPrinted,@IsPrintAsn,@IsPrintRec,@IsShipExceed,@IsRecExceed,@IsOrderFulfillUC,@IsShipFulfillUC,@IsRecFulfillUC,@IsShipScanHu,@IsRecScanHu,@IsCreatePL,@IsPLCreate,@IsShipFifo,@IsRecFifo,@IsShipByOrder,@IsOpenOrder,@IsAsnUniqueRec,@RecGapTo,@RecTemplate,@OrderTemplate,@AsnTemplate,@HuTemplate,@BillTerm,@CreateHuOpt,@ReCalculatePriceOpt,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@ReleaseDate,@ReleaseUser,@ReleaseUserNm,@StartDate,@StartUser,@StartUserNm,@CompleteDate,@CompleteUser,@CompleteUserNm,@CloseDate,@CloseUser,@CloseUserNm,@CancelDate,@CancelUser,@CancelUserNm,@CancelReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@OrderStrategy,@ProdLineFact,@IsQuick,@PickStrategy,@ExtraDmdSource,@OrderNo)
	END	
	ELSE IF @Type=6
	BEGIN
		INSERT INTO ORD_OrderMstr_6(Version,Flow,FlowDesc,TraceCode,RefOrderNo,ExtOrderNo,Type,SubType,QualityType,StartTime,WindowTime,IsPlanPause,PauseSeq,IsPause,PauseTime,IsPLPause,EffDate,Priority,Status,Seq,SapSeq,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipToAddr,ShipTo,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Shift,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,Currency,Dock,Routing,CurtOp,IsAutoRelease,IsAutoStart,IsAutoShip,IsAutoReceive,IsAutoBill,IsManualCreateDet,IsListPrice,IsPrintOrder,IsOrderPrinted,IsPrintAsn,IsPrintRec,IsShipExceed,IsRecExceed,IsOrderFulfillUC,IsShipFulfillUC,IsRecFulfillUC,IsShipScanHu,IsRecScanHu,IsCreatePL,IsPLCreate,IsShipFifo,IsRecFifo,IsShipByOrder,IsOpenOrder,IsAsnUniqueRec,RecGapTo,RecTemplate,OrderTemplate,AsnTemplate,HuTemplate,BillTerm,CreateHuOpt,ReCalculatePriceOpt,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,ReleaseDate,ReleaseUser,ReleaseUserNm,StartDate,StartUser,StartUserNm,CompleteDate,CompleteUser,CompleteUserNm,CloseDate,CloseUser,CloseUserNm,CancelDate,CancelUser,CancelUserNm,CancelReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,OrderStrategy,ProdLineFact,IsQuick,PickStrategy,ExtraDmdSource,OrderNo) 
		VALUES(@Version,@Flow,@FlowDesc,@TraceCode,@RefOrderNo,@ExtOrderNo,@Type,@SubType,@QualityType,@StartTime,@WindowTime,@IsPlanPause,@PauseSeq,@IsPause,@PauseTime,@IsPLPause,@EffDate,@Priority,@Status,@Seq,@SapSeq,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipToAddr,@ShipTo,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Shift,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@Currency,@Dock,@Routing,@CurtOp,@IsAutoRelease,@IsAutoStart,@IsAutoShip,@IsAutoReceive,@IsAutoBill,@IsManualCreateDet,@IsListPrice,@IsPrintOrder,@IsOrderPrinted,@IsPrintAsn,@IsPrintRec,@IsShipExceed,@IsRecExceed,@IsOrderFulfillUC,@IsShipFulfillUC,@IsRecFulfillUC,@IsShipScanHu,@IsRecScanHu,@IsCreatePL,@IsPLCreate,@IsShipFifo,@IsRecFifo,@IsShipByOrder,@IsOpenOrder,@IsAsnUniqueRec,@RecGapTo,@RecTemplate,@OrderTemplate,@AsnTemplate,@HuTemplate,@BillTerm,@CreateHuOpt,@ReCalculatePriceOpt,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@ReleaseDate,@ReleaseUser,@ReleaseUserNm,@StartDate,@StartUser,@StartUserNm,@CompleteDate,@CompleteUser,@CompleteUserNm,@CloseDate,@CloseUser,@CloseUserNm,@CancelDate,@CancelUser,@CancelUserNm,@CancelReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@OrderStrategy,@ProdLineFact,@IsQuick,@PickStrategy,@ExtraDmdSource,@OrderNo)
	END	
	ELSE IF @Type=7
	BEGIN
		INSERT INTO ORD_OrderMstr_7(Version,Flow,FlowDesc,TraceCode,RefOrderNo,ExtOrderNo,Type,SubType,QualityType,StartTime,WindowTime,IsPlanPause,PauseSeq,IsPause,PauseTime,IsPLPause,EffDate,Priority,Status,Seq,SapSeq,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipToAddr,ShipTo,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Shift,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,Currency,Dock,Routing,CurtOp,IsAutoRelease,IsAutoStart,IsAutoShip,IsAutoReceive,IsAutoBill,IsManualCreateDet,IsListPrice,IsPrintOrder,IsOrderPrinted,IsPrintAsn,IsPrintRec,IsShipExceed,IsRecExceed,IsOrderFulfillUC,IsShipFulfillUC,IsRecFulfillUC,IsShipScanHu,IsRecScanHu,IsCreatePL,IsPLCreate,IsShipFifo,IsRecFifo,IsShipByOrder,IsOpenOrder,IsAsnUniqueRec,RecGapTo,RecTemplate,OrderTemplate,AsnTemplate,HuTemplate,BillTerm,CreateHuOpt,ReCalculatePriceOpt,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,ReleaseDate,ReleaseUser,ReleaseUserNm,StartDate,StartUser,StartUserNm,CompleteDate,CompleteUser,CompleteUserNm,CloseDate,CloseUser,CloseUserNm,CancelDate,CancelUser,CancelUserNm,CancelReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,OrderStrategy,ProdLineFact,IsQuick,PickStrategy,ExtraDmdSource,OrderNo) 
		VALUES(@Version,@Flow,@FlowDesc,@TraceCode,@RefOrderNo,@ExtOrderNo,@Type,@SubType,@QualityType,@StartTime,@WindowTime,@IsPlanPause,@PauseSeq,@IsPause,@PauseTime,@IsPLPause,@EffDate,@Priority,@Status,@Seq,@SapSeq,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipToAddr,@ShipTo,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Shift,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@Currency,@Dock,@Routing,@CurtOp,@IsAutoRelease,@IsAutoStart,@IsAutoShip,@IsAutoReceive,@IsAutoBill,@IsManualCreateDet,@IsListPrice,@IsPrintOrder,@IsOrderPrinted,@IsPrintAsn,@IsPrintRec,@IsShipExceed,@IsRecExceed,@IsOrderFulfillUC,@IsShipFulfillUC,@IsRecFulfillUC,@IsShipScanHu,@IsRecScanHu,@IsCreatePL,@IsPLCreate,@IsShipFifo,@IsRecFifo,@IsShipByOrder,@IsOpenOrder,@IsAsnUniqueRec,@RecGapTo,@RecTemplate,@OrderTemplate,@AsnTemplate,@HuTemplate,@BillTerm,@CreateHuOpt,@ReCalculatePriceOpt,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@ReleaseDate,@ReleaseUser,@ReleaseUserNm,@StartDate,@StartUser,@StartUserNm,@CompleteDate,@CompleteUser,@CompleteUserNm,@CloseDate,@CloseUser,@CloseUserNm,@CancelDate,@CancelUser,@CancelUserNm,@CancelReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@OrderStrategy,@ProdLineFact,@IsQuick,@PickStrategy,@ExtraDmdSource,@OrderNo)
	END
	ELSE IF @Type=8
	BEGIN
		INSERT INTO ORD_OrderMstr_8(Version,Flow,FlowDesc,TraceCode,RefOrderNo,ExtOrderNo,Type,SubType,QualityType,StartTime,WindowTime,IsPlanPause,PauseSeq,IsPause,PauseTime,IsPLPause,EffDate,Priority,Status,Seq,SapSeq,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipToAddr,ShipTo,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Shift,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,Currency,Dock,Routing,CurtOp,IsAutoRelease,IsAutoStart,IsAutoShip,IsAutoReceive,IsAutoBill,IsManualCreateDet,IsListPrice,IsPrintOrder,IsOrderPrinted,IsPrintAsn,IsPrintRec,IsShipExceed,IsRecExceed,IsOrderFulfillUC,IsShipFulfillUC,IsRecFulfillUC,IsShipScanHu,IsRecScanHu,IsCreatePL,IsPLCreate,IsShipFifo,IsRecFifo,IsShipByOrder,IsOpenOrder,IsAsnUniqueRec,RecGapTo,RecTemplate,OrderTemplate,AsnTemplate,HuTemplate,BillTerm,CreateHuOpt,ReCalculatePriceOpt,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,ReleaseDate,ReleaseUser,ReleaseUserNm,StartDate,StartUser,StartUserNm,CompleteDate,CompleteUser,CompleteUserNm,CloseDate,CloseUser,CloseUserNm,CancelDate,CancelUser,CancelUserNm,CancelReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,OrderStrategy,ProdLineFact,IsQuick,PickStrategy,ExtraDmdSource,OrderNo) 
		VALUES(@Version,@Flow,@FlowDesc,@TraceCode,@RefOrderNo,@ExtOrderNo,@Type,@SubType,@QualityType,@StartTime,@WindowTime,@IsPlanPause,@PauseSeq,@IsPause,@PauseTime,@IsPLPause,@EffDate,@Priority,@Status,@Seq,@SapSeq,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipToAddr,@ShipTo,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Shift,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@Currency,@Dock,@Routing,@CurtOp,@IsAutoRelease,@IsAutoStart,@IsAutoShip,@IsAutoReceive,@IsAutoBill,@IsManualCreateDet,@IsListPrice,@IsPrintOrder,@IsOrderPrinted,@IsPrintAsn,@IsPrintRec,@IsShipExceed,@IsRecExceed,@IsOrderFulfillUC,@IsShipFulfillUC,@IsRecFulfillUC,@IsShipScanHu,@IsRecScanHu,@IsCreatePL,@IsPLCreate,@IsShipFifo,@IsRecFifo,@IsShipByOrder,@IsOpenOrder,@IsAsnUniqueRec,@RecGapTo,@RecTemplate,@OrderTemplate,@AsnTemplate,@HuTemplate,@BillTerm,@CreateHuOpt,@ReCalculatePriceOpt,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@ReleaseDate,@ReleaseUser,@ReleaseUserNm,@StartDate,@StartUser,@StartUserNm,@CompleteDate,@CompleteUser,@CompleteUserNm,@CloseDate,@CloseUser,@CloseUserNm,@CancelDate,@CancelUser,@CancelUserNm,@CancelReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@OrderStrategy,@ProdLineFact,@IsQuick,@PickStrategy,@ExtraDmdSource,@OrderNo)
	END
	ELSE
	BEGIN
		INSERT INTO ORD_OrderMstr_0(Version,Flow,FlowDesc,TraceCode,RefOrderNo,ExtOrderNo,Type,SubType,QualityType,StartTime,WindowTime,IsPlanPause,PauseSeq,IsPause,PauseTime,IsPLPause,EffDate,Priority,Status,Seq,SapSeq,PartyFrom,PartyFromNm,PartyTo,PartyToNm,ShipFrom,ShipFromAddr,ShipFromTel,ShipFromCell,ShipFromFax,ShipFromContact,ShipToAddr,ShipTo,ShipToTel,ShipToCell,ShipToFax,ShipToContact,Shift,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,Currency,Dock,Routing,CurtOp,IsAutoRelease,IsAutoStart,IsAutoShip,IsAutoReceive,IsAutoBill,IsManualCreateDet,IsListPrice,IsPrintOrder,IsOrderPrinted,IsPrintAsn,IsPrintRec,IsShipExceed,IsRecExceed,IsOrderFulfillUC,IsShipFulfillUC,IsRecFulfillUC,IsShipScanHu,IsRecScanHu,IsCreatePL,IsPLCreate,IsShipFifo,IsRecFifo,IsShipByOrder,IsOpenOrder,IsAsnUniqueRec,RecGapTo,RecTemplate,OrderTemplate,AsnTemplate,HuTemplate,BillTerm,CreateHuOpt,ReCalculatePriceOpt,WMSNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,ReleaseDate,ReleaseUser,ReleaseUserNm,StartDate,StartUser,StartUserNm,CompleteDate,CompleteUser,CompleteUserNm,CloseDate,CloseUser,CloseUserNm,CancelDate,CancelUser,CancelUserNm,CancelReason,IsCheckPartyFromAuth,IsCheckPartyToAuth,OrderStrategy,ProdLineFact,IsQuick,PickStrategy,ExtraDmdSource,OrderNo) 
		VALUES(@Version,@Flow,@FlowDesc,@TraceCode,@RefOrderNo,@ExtOrderNo,0,@SubType,@QualityType,@StartTime,@WindowTime,@IsPlanPause,@PauseSeq,@IsPause,@PauseTime,@IsPLPause,@EffDate,@Priority,@Status,@Seq,@SapSeq,@PartyFrom,@PartyFromNm,@PartyTo,@PartyToNm,@ShipFrom,@ShipFromAddr,@ShipFromTel,@ShipFromCell,@ShipFromFax,@ShipFromContact,@ShipToAddr,@ShipTo,@ShipToTel,@ShipToCell,@ShipToFax,@ShipToContact,@Shift,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@Currency,@Dock,@Routing,@CurtOp,@IsAutoRelease,@IsAutoStart,@IsAutoShip,@IsAutoReceive,@IsAutoBill,@IsManualCreateDet,@IsListPrice,@IsPrintOrder,@IsOrderPrinted,@IsPrintAsn,@IsPrintRec,@IsShipExceed,@IsRecExceed,@IsOrderFulfillUC,@IsShipFulfillUC,@IsRecFulfillUC,@IsShipScanHu,@IsRecScanHu,@IsCreatePL,@IsPLCreate,@IsShipFifo,@IsRecFifo,@IsShipByOrder,@IsOpenOrder,@IsAsnUniqueRec,@RecGapTo,@RecTemplate,@OrderTemplate,@AsnTemplate,@HuTemplate,@BillTerm,@CreateHuOpt,@ReCalculatePriceOpt,@WMSNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@ReleaseDate,@ReleaseUser,@ReleaseUserNm,@StartDate,@StartUser,@StartUserNm,@CompleteDate,@CompleteUser,@CompleteUserNm,@CloseDate,@CloseUser,@CloseUserNm,@CancelDate,@CancelUser,@CancelUserNm,@CancelReason,@IsCheckPartyFromAuth,@IsCheckPartyToAuth,@OrderStrategy,@ProdLineFact,@IsQuick,@PickStrategy,@ExtraDmdSource,@OrderNo)
	END									
END
GO
