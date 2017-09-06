/****** Object:  StoredProcedure [dbo].[USP_Split_OrderMstr_Update]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_OrderMstr_Update')
	DROP PROCEDURE USP_Split_OrderMstr_Update
CREATE PROCEDURE [dbo].[USP_Split_OrderMstr_Update]
(
	@Version int,
	@Flow varchar(50),
	@FlowDesc varchar(100),
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
	@OrderNo varchar(4000),
	@VersionBerfore int
)
AS
BEGIN
	IF @Type=1
	BEGIN
		UPDATE ORD_OrderMstr_1 SET Version=@Version,Flow=@Flow,FlowDesc=@FlowDesc,TraceCode=@TraceCode,RefOrderNo=@RefOrderNo,ExtOrderNo=@ExtOrderNo,Type=@Type,SubType=@SubType,QualityType=@QualityType,StartTime=@StartTime,WindowTime=@WindowTime,IsPlanPause=@IsPlanPause,PauseSeq=@PauseSeq,IsPause=@IsPause,PauseTime=@PauseTime,IsPLPause=@IsPLPause,EffDate=@EffDate,Priority=@Priority,Status=@Status,Seq=@Seq,SapSeq=@SapSeq,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipToAddr=@ShipToAddr,ShipTo=@ShipTo,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Shift=@Shift,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,BillAddrDesc=@BillAddrDesc,PriceList=@PriceList,Currency=@Currency,Dock=@Dock,Routing=@Routing,CurtOp=@CurtOp,IsAutoRelease=@IsAutoRelease,IsAutoStart=@IsAutoStart,IsAutoShip=@IsAutoShip,IsAutoReceive=@IsAutoReceive,IsAutoBill=@IsAutoBill,IsManualCreateDet=@IsManualCreateDet,IsListPrice=@IsListPrice,IsPrintOrder=@IsPrintOrder,IsOrderPrinted=@IsOrderPrinted,IsPrintAsn=@IsPrintAsn,IsPrintRec=@IsPrintRec,IsShipExceed=@IsShipExceed,IsRecExceed=@IsRecExceed,IsOrderFulfillUC=@IsOrderFulfillUC,IsShipFulfillUC=@IsShipFulfillUC,IsRecFulfillUC=@IsRecFulfillUC,IsShipScanHu=@IsShipScanHu,IsRecScanHu=@IsRecScanHu,IsCreatePL=@IsCreatePL,IsPLCreate=@IsPLCreate,IsShipFifo=@IsShipFifo,IsRecFifo=@IsRecFifo,IsShipByOrder=@IsShipByOrder,IsOpenOrder=@IsOpenOrder,IsAsnUniqueRec=@IsAsnUniqueRec,RecGapTo=@RecGapTo,RecTemplate=@RecTemplate,OrderTemplate=@OrderTemplate,AsnTemplate=@AsnTemplate,HuTemplate=@HuTemplate,BillTerm=@BillTerm,CreateHuOpt=@CreateHuOpt,ReCalculatePriceOpt=@ReCalculatePriceOpt,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,ReleaseDate=@ReleaseDate,ReleaseUser=@ReleaseUser,ReleaseUserNm=@ReleaseUserNm,StartDate=@StartDate,StartUser=@StartUser,StartUserNm=@StartUserNm,CompleteDate=@CompleteDate,CompleteUser=@CompleteUser,CompleteUserNm=@CompleteUserNm,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CancelDate=@CancelDate,CancelUser=@CancelUser,CancelUserNm=@CancelUserNm,CancelReason=@CancelReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,OrderStrategy=@OrderStrategy,ProdLineFact=@ProdLineFact,IsQuick=@IsQuick,PickStrategy=@PickStrategy,ExtraDmdSource=@ExtraDmdSource,OrderNo=@OrderNo
		WHERE OrderNo= @OrderNo AND Version= @VersionBerfore
	END
	ELSE IF @Type=2
	BEGIN
		UPDATE ORD_OrderMstr_2 SET Version=@Version,Flow=@Flow,FlowDesc=@FlowDesc,TraceCode=@TraceCode,RefOrderNo=@RefOrderNo,ExtOrderNo=@ExtOrderNo,Type=@Type,SubType=@SubType,QualityType=@QualityType,StartTime=@StartTime,WindowTime=@WindowTime,IsPlanPause=@IsPlanPause,PauseSeq=@PauseSeq,IsPause=@IsPause,PauseTime=@PauseTime,IsPLPause=@IsPLPause,EffDate=@EffDate,Priority=@Priority,Status=@Status,Seq=@Seq,SapSeq=@SapSeq,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipToAddr=@ShipToAddr,ShipTo=@ShipTo,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Shift=@Shift,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,BillAddrDesc=@BillAddrDesc,PriceList=@PriceList,Currency=@Currency,Dock=@Dock,Routing=@Routing,CurtOp=@CurtOp,IsAutoRelease=@IsAutoRelease,IsAutoStart=@IsAutoStart,IsAutoShip=@IsAutoShip,IsAutoReceive=@IsAutoReceive,IsAutoBill=@IsAutoBill,IsManualCreateDet=@IsManualCreateDet,IsListPrice=@IsListPrice,IsPrintOrder=@IsPrintOrder,IsOrderPrinted=@IsOrderPrinted,IsPrintAsn=@IsPrintAsn,IsPrintRec=@IsPrintRec,IsShipExceed=@IsShipExceed,IsRecExceed=@IsRecExceed,IsOrderFulfillUC=@IsOrderFulfillUC,IsShipFulfillUC=@IsShipFulfillUC,IsRecFulfillUC=@IsRecFulfillUC,IsShipScanHu=@IsShipScanHu,IsRecScanHu=@IsRecScanHu,IsCreatePL=@IsCreatePL,IsPLCreate=@IsPLCreate,IsShipFifo=@IsShipFifo,IsRecFifo=@IsRecFifo,IsShipByOrder=@IsShipByOrder,IsOpenOrder=@IsOpenOrder,IsAsnUniqueRec=@IsAsnUniqueRec,RecGapTo=@RecGapTo,RecTemplate=@RecTemplate,OrderTemplate=@OrderTemplate,AsnTemplate=@AsnTemplate,HuTemplate=@HuTemplate,BillTerm=@BillTerm,CreateHuOpt=@CreateHuOpt,ReCalculatePriceOpt=@ReCalculatePriceOpt,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,ReleaseDate=@ReleaseDate,ReleaseUser=@ReleaseUser,ReleaseUserNm=@ReleaseUserNm,StartDate=@StartDate,StartUser=@StartUser,StartUserNm=@StartUserNm,CompleteDate=@CompleteDate,CompleteUser=@CompleteUser,CompleteUserNm=@CompleteUserNm,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CancelDate=@CancelDate,CancelUser=@CancelUser,CancelUserNm=@CancelUserNm,CancelReason=@CancelReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,OrderStrategy=@OrderStrategy,ProdLineFact=@ProdLineFact,IsQuick=@IsQuick,PickStrategy=@PickStrategy,ExtraDmdSource=@ExtraDmdSource,OrderNo=@OrderNo
		WHERE OrderNo= @OrderNo AND Version= @VersionBerfore
	END
	ELSE IF @Type=3
	BEGIN
		UPDATE ORD_OrderMstr_3 SET Version=@Version,Flow=@Flow,FlowDesc=@FlowDesc,TraceCode=@TraceCode,RefOrderNo=@RefOrderNo,ExtOrderNo=@ExtOrderNo,Type=@Type,SubType=@SubType,QualityType=@QualityType,StartTime=@StartTime,WindowTime=@WindowTime,IsPlanPause=@IsPlanPause,PauseSeq=@PauseSeq,IsPause=@IsPause,PauseTime=@PauseTime,IsPLPause=@IsPLPause,EffDate=@EffDate,Priority=@Priority,Status=@Status,Seq=@Seq,SapSeq=@SapSeq,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipToAddr=@ShipToAddr,ShipTo=@ShipTo,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Shift=@Shift,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,BillAddrDesc=@BillAddrDesc,PriceList=@PriceList,Currency=@Currency,Dock=@Dock,Routing=@Routing,CurtOp=@CurtOp,IsAutoRelease=@IsAutoRelease,IsAutoStart=@IsAutoStart,IsAutoShip=@IsAutoShip,IsAutoReceive=@IsAutoReceive,IsAutoBill=@IsAutoBill,IsManualCreateDet=@IsManualCreateDet,IsListPrice=@IsListPrice,IsPrintOrder=@IsPrintOrder,IsOrderPrinted=@IsOrderPrinted,IsPrintAsn=@IsPrintAsn,IsPrintRec=@IsPrintRec,IsShipExceed=@IsShipExceed,IsRecExceed=@IsRecExceed,IsOrderFulfillUC=@IsOrderFulfillUC,IsShipFulfillUC=@IsShipFulfillUC,IsRecFulfillUC=@IsRecFulfillUC,IsShipScanHu=@IsShipScanHu,IsRecScanHu=@IsRecScanHu,IsCreatePL=@IsCreatePL,IsPLCreate=@IsPLCreate,IsShipFifo=@IsShipFifo,IsRecFifo=@IsRecFifo,IsShipByOrder=@IsShipByOrder,IsOpenOrder=@IsOpenOrder,IsAsnUniqueRec=@IsAsnUniqueRec,RecGapTo=@RecGapTo,RecTemplate=@RecTemplate,OrderTemplate=@OrderTemplate,AsnTemplate=@AsnTemplate,HuTemplate=@HuTemplate,BillTerm=@BillTerm,CreateHuOpt=@CreateHuOpt,ReCalculatePriceOpt=@ReCalculatePriceOpt,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,ReleaseDate=@ReleaseDate,ReleaseUser=@ReleaseUser,ReleaseUserNm=@ReleaseUserNm,StartDate=@StartDate,StartUser=@StartUser,StartUserNm=@StartUserNm,CompleteDate=@CompleteDate,CompleteUser=@CompleteUser,CompleteUserNm=@CompleteUserNm,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CancelDate=@CancelDate,CancelUser=@CancelUser,CancelUserNm=@CancelUserNm,CancelReason=@CancelReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,OrderStrategy=@OrderStrategy,ProdLineFact=@ProdLineFact,IsQuick=@IsQuick,PickStrategy=@PickStrategy,ExtraDmdSource=@ExtraDmdSource,OrderNo=@OrderNo
		WHERE OrderNo= @OrderNo AND Version= @VersionBerfore
	END
	ELSE IF @Type=4
	BEGIN
		UPDATE ORD_OrderMstr_4 SET Version=@Version,Flow=@Flow,FlowDesc=@FlowDesc,TraceCode=@TraceCode,RefOrderNo=@RefOrderNo,ExtOrderNo=@ExtOrderNo,Type=@Type,SubType=@SubType,QualityType=@QualityType,StartTime=@StartTime,WindowTime=@WindowTime,IsPlanPause=@IsPlanPause,PauseSeq=@PauseSeq,IsPause=@IsPause,PauseTime=@PauseTime,IsPLPause=@IsPLPause,EffDate=@EffDate,Priority=@Priority,Status=@Status,Seq=@Seq,SapSeq=@SapSeq,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipToAddr=@ShipToAddr,ShipTo=@ShipTo,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Shift=@Shift,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,BillAddrDesc=@BillAddrDesc,PriceList=@PriceList,Currency=@Currency,Dock=@Dock,Routing=@Routing,CurtOp=@CurtOp,IsAutoRelease=@IsAutoRelease,IsAutoStart=@IsAutoStart,IsAutoShip=@IsAutoShip,IsAutoReceive=@IsAutoReceive,IsAutoBill=@IsAutoBill,IsManualCreateDet=@IsManualCreateDet,IsListPrice=@IsListPrice,IsPrintOrder=@IsPrintOrder,IsOrderPrinted=@IsOrderPrinted,IsPrintAsn=@IsPrintAsn,IsPrintRec=@IsPrintRec,IsShipExceed=@IsShipExceed,IsRecExceed=@IsRecExceed,IsOrderFulfillUC=@IsOrderFulfillUC,IsShipFulfillUC=@IsShipFulfillUC,IsRecFulfillUC=@IsRecFulfillUC,IsShipScanHu=@IsShipScanHu,IsRecScanHu=@IsRecScanHu,IsCreatePL=@IsCreatePL,IsPLCreate=@IsPLCreate,IsShipFifo=@IsShipFifo,IsRecFifo=@IsRecFifo,IsShipByOrder=@IsShipByOrder,IsOpenOrder=@IsOpenOrder,IsAsnUniqueRec=@IsAsnUniqueRec,RecGapTo=@RecGapTo,RecTemplate=@RecTemplate,OrderTemplate=@OrderTemplate,AsnTemplate=@AsnTemplate,HuTemplate=@HuTemplate,BillTerm=@BillTerm,CreateHuOpt=@CreateHuOpt,ReCalculatePriceOpt=@ReCalculatePriceOpt,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,ReleaseDate=@ReleaseDate,ReleaseUser=@ReleaseUser,ReleaseUserNm=@ReleaseUserNm,StartDate=@StartDate,StartUser=@StartUser,StartUserNm=@StartUserNm,CompleteDate=@CompleteDate,CompleteUser=@CompleteUser,CompleteUserNm=@CompleteUserNm,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CancelDate=@CancelDate,CancelUser=@CancelUser,CancelUserNm=@CancelUserNm,CancelReason=@CancelReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,OrderStrategy=@OrderStrategy,ProdLineFact=@ProdLineFact,IsQuick=@IsQuick,PickStrategy=@PickStrategy,ExtraDmdSource=@ExtraDmdSource,OrderNo=@OrderNo
		WHERE OrderNo= @OrderNo AND Version= @VersionBerfore
	END
	ELSE IF @Type=5
	BEGIN
		UPDATE ORD_OrderMstr_5 SET Version=@Version,Flow=@Flow,FlowDesc=@FlowDesc,TraceCode=@TraceCode,RefOrderNo=@RefOrderNo,ExtOrderNo=@ExtOrderNo,Type=@Type,SubType=@SubType,QualityType=@QualityType,StartTime=@StartTime,WindowTime=@WindowTime,IsPlanPause=@IsPlanPause,PauseSeq=@PauseSeq,IsPause=@IsPause,PauseTime=@PauseTime,IsPLPause=@IsPLPause,EffDate=@EffDate,Priority=@Priority,Status=@Status,Seq=@Seq,SapSeq=@SapSeq,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipToAddr=@ShipToAddr,ShipTo=@ShipTo,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Shift=@Shift,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,BillAddrDesc=@BillAddrDesc,PriceList=@PriceList,Currency=@Currency,Dock=@Dock,Routing=@Routing,CurtOp=@CurtOp,IsAutoRelease=@IsAutoRelease,IsAutoStart=@IsAutoStart,IsAutoShip=@IsAutoShip,IsAutoReceive=@IsAutoReceive,IsAutoBill=@IsAutoBill,IsManualCreateDet=@IsManualCreateDet,IsListPrice=@IsListPrice,IsPrintOrder=@IsPrintOrder,IsOrderPrinted=@IsOrderPrinted,IsPrintAsn=@IsPrintAsn,IsPrintRec=@IsPrintRec,IsShipExceed=@IsShipExceed,IsRecExceed=@IsRecExceed,IsOrderFulfillUC=@IsOrderFulfillUC,IsShipFulfillUC=@IsShipFulfillUC,IsRecFulfillUC=@IsRecFulfillUC,IsShipScanHu=@IsShipScanHu,IsRecScanHu=@IsRecScanHu,IsCreatePL=@IsCreatePL,IsPLCreate=@IsPLCreate,IsShipFifo=@IsShipFifo,IsRecFifo=@IsRecFifo,IsShipByOrder=@IsShipByOrder,IsOpenOrder=@IsOpenOrder,IsAsnUniqueRec=@IsAsnUniqueRec,RecGapTo=@RecGapTo,RecTemplate=@RecTemplate,OrderTemplate=@OrderTemplate,AsnTemplate=@AsnTemplate,HuTemplate=@HuTemplate,BillTerm=@BillTerm,CreateHuOpt=@CreateHuOpt,ReCalculatePriceOpt=@ReCalculatePriceOpt,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,ReleaseDate=@ReleaseDate,ReleaseUser=@ReleaseUser,ReleaseUserNm=@ReleaseUserNm,StartDate=@StartDate,StartUser=@StartUser,StartUserNm=@StartUserNm,CompleteDate=@CompleteDate,CompleteUser=@CompleteUser,CompleteUserNm=@CompleteUserNm,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CancelDate=@CancelDate,CancelUser=@CancelUser,CancelUserNm=@CancelUserNm,CancelReason=@CancelReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,OrderStrategy=@OrderStrategy,ProdLineFact=@ProdLineFact,IsQuick=@IsQuick,PickStrategy=@PickStrategy,ExtraDmdSource=@ExtraDmdSource,OrderNo=@OrderNo
		WHERE OrderNo= @OrderNo AND Version= @VersionBerfore
	END
	ELSE IF @Type=6
	BEGIN
		UPDATE ORD_OrderMstr_6 SET Version=@Version,Flow=@Flow,FlowDesc=@FlowDesc,TraceCode=@TraceCode,RefOrderNo=@RefOrderNo,ExtOrderNo=@ExtOrderNo,Type=@Type,SubType=@SubType,QualityType=@QualityType,StartTime=@StartTime,WindowTime=@WindowTime,IsPlanPause=@IsPlanPause,PauseSeq=@PauseSeq,IsPause=@IsPause,PauseTime=@PauseTime,IsPLPause=@IsPLPause,EffDate=@EffDate,Priority=@Priority,Status=@Status,Seq=@Seq,SapSeq=@SapSeq,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipToAddr=@ShipToAddr,ShipTo=@ShipTo,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Shift=@Shift,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,BillAddrDesc=@BillAddrDesc,PriceList=@PriceList,Currency=@Currency,Dock=@Dock,Routing=@Routing,CurtOp=@CurtOp,IsAutoRelease=@IsAutoRelease,IsAutoStart=@IsAutoStart,IsAutoShip=@IsAutoShip,IsAutoReceive=@IsAutoReceive,IsAutoBill=@IsAutoBill,IsManualCreateDet=@IsManualCreateDet,IsListPrice=@IsListPrice,IsPrintOrder=@IsPrintOrder,IsOrderPrinted=@IsOrderPrinted,IsPrintAsn=@IsPrintAsn,IsPrintRec=@IsPrintRec,IsShipExceed=@IsShipExceed,IsRecExceed=@IsRecExceed,IsOrderFulfillUC=@IsOrderFulfillUC,IsShipFulfillUC=@IsShipFulfillUC,IsRecFulfillUC=@IsRecFulfillUC,IsShipScanHu=@IsShipScanHu,IsRecScanHu=@IsRecScanHu,IsCreatePL=@IsCreatePL,IsPLCreate=@IsPLCreate,IsShipFifo=@IsShipFifo,IsRecFifo=@IsRecFifo,IsShipByOrder=@IsShipByOrder,IsOpenOrder=@IsOpenOrder,IsAsnUniqueRec=@IsAsnUniqueRec,RecGapTo=@RecGapTo,RecTemplate=@RecTemplate,OrderTemplate=@OrderTemplate,AsnTemplate=@AsnTemplate,HuTemplate=@HuTemplate,BillTerm=@BillTerm,CreateHuOpt=@CreateHuOpt,ReCalculatePriceOpt=@ReCalculatePriceOpt,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,ReleaseDate=@ReleaseDate,ReleaseUser=@ReleaseUser,ReleaseUserNm=@ReleaseUserNm,StartDate=@StartDate,StartUser=@StartUser,StartUserNm=@StartUserNm,CompleteDate=@CompleteDate,CompleteUser=@CompleteUser,CompleteUserNm=@CompleteUserNm,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CancelDate=@CancelDate,CancelUser=@CancelUser,CancelUserNm=@CancelUserNm,CancelReason=@CancelReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,OrderStrategy=@OrderStrategy,ProdLineFact=@ProdLineFact,IsQuick=@IsQuick,PickStrategy=@PickStrategy,ExtraDmdSource=@ExtraDmdSource,OrderNo=@OrderNo
		WHERE OrderNo= @OrderNo AND Version= @VersionBerfore
	END
	ELSE IF @Type=7
	BEGIN
		UPDATE ORD_OrderMstr_7 SET Version=@Version,Flow=@Flow,FlowDesc=@FlowDesc,TraceCode=@TraceCode,RefOrderNo=@RefOrderNo,ExtOrderNo=@ExtOrderNo,Type=@Type,SubType=@SubType,QualityType=@QualityType,StartTime=@StartTime,WindowTime=@WindowTime,IsPlanPause=@IsPlanPause,PauseSeq=@PauseSeq,IsPause=@IsPause,PauseTime=@PauseTime,IsPLPause=@IsPLPause,EffDate=@EffDate,Priority=@Priority,Status=@Status,Seq=@Seq,SapSeq=@SapSeq,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipToAddr=@ShipToAddr,ShipTo=@ShipTo,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Shift=@Shift,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,BillAddrDesc=@BillAddrDesc,PriceList=@PriceList,Currency=@Currency,Dock=@Dock,Routing=@Routing,CurtOp=@CurtOp,IsAutoRelease=@IsAutoRelease,IsAutoStart=@IsAutoStart,IsAutoShip=@IsAutoShip,IsAutoReceive=@IsAutoReceive,IsAutoBill=@IsAutoBill,IsManualCreateDet=@IsManualCreateDet,IsListPrice=@IsListPrice,IsPrintOrder=@IsPrintOrder,IsOrderPrinted=@IsOrderPrinted,IsPrintAsn=@IsPrintAsn,IsPrintRec=@IsPrintRec,IsShipExceed=@IsShipExceed,IsRecExceed=@IsRecExceed,IsOrderFulfillUC=@IsOrderFulfillUC,IsShipFulfillUC=@IsShipFulfillUC,IsRecFulfillUC=@IsRecFulfillUC,IsShipScanHu=@IsShipScanHu,IsRecScanHu=@IsRecScanHu,IsCreatePL=@IsCreatePL,IsPLCreate=@IsPLCreate,IsShipFifo=@IsShipFifo,IsRecFifo=@IsRecFifo,IsShipByOrder=@IsShipByOrder,IsOpenOrder=@IsOpenOrder,IsAsnUniqueRec=@IsAsnUniqueRec,RecGapTo=@RecGapTo,RecTemplate=@RecTemplate,OrderTemplate=@OrderTemplate,AsnTemplate=@AsnTemplate,HuTemplate=@HuTemplate,BillTerm=@BillTerm,CreateHuOpt=@CreateHuOpt,ReCalculatePriceOpt=@ReCalculatePriceOpt,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,ReleaseDate=@ReleaseDate,ReleaseUser=@ReleaseUser,ReleaseUserNm=@ReleaseUserNm,StartDate=@StartDate,StartUser=@StartUser,StartUserNm=@StartUserNm,CompleteDate=@CompleteDate,CompleteUser=@CompleteUser,CompleteUserNm=@CompleteUserNm,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CancelDate=@CancelDate,CancelUser=@CancelUser,CancelUserNm=@CancelUserNm,CancelReason=@CancelReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,OrderStrategy=@OrderStrategy,ProdLineFact=@ProdLineFact,IsQuick=@IsQuick,PickStrategy=@PickStrategy,ExtraDmdSource=@ExtraDmdSource,OrderNo=@OrderNo
		WHERE OrderNo= @OrderNo AND Version= @VersionBerfore
	END
	ELSE IF @Type=8
	BEGIN
		UPDATE ORD_OrderMstr_8 SET Version=@Version,Flow=@Flow,FlowDesc=@FlowDesc,TraceCode=@TraceCode,RefOrderNo=@RefOrderNo,ExtOrderNo=@ExtOrderNo,Type=@Type,SubType=@SubType,QualityType=@QualityType,StartTime=@StartTime,WindowTime=@WindowTime,IsPlanPause=@IsPlanPause,PauseSeq=@PauseSeq,IsPause=@IsPause,PauseTime=@PauseTime,IsPLPause=@IsPLPause,EffDate=@EffDate,Priority=@Priority,Status=@Status,Seq=@Seq,SapSeq=@SapSeq,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipToAddr=@ShipToAddr,ShipTo=@ShipTo,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Shift=@Shift,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,BillAddrDesc=@BillAddrDesc,PriceList=@PriceList,Currency=@Currency,Dock=@Dock,Routing=@Routing,CurtOp=@CurtOp,IsAutoRelease=@IsAutoRelease,IsAutoStart=@IsAutoStart,IsAutoShip=@IsAutoShip,IsAutoReceive=@IsAutoReceive,IsAutoBill=@IsAutoBill,IsManualCreateDet=@IsManualCreateDet,IsListPrice=@IsListPrice,IsPrintOrder=@IsPrintOrder,IsOrderPrinted=@IsOrderPrinted,IsPrintAsn=@IsPrintAsn,IsPrintRec=@IsPrintRec,IsShipExceed=@IsShipExceed,IsRecExceed=@IsRecExceed,IsOrderFulfillUC=@IsOrderFulfillUC,IsShipFulfillUC=@IsShipFulfillUC,IsRecFulfillUC=@IsRecFulfillUC,IsShipScanHu=@IsShipScanHu,IsRecScanHu=@IsRecScanHu,IsCreatePL=@IsCreatePL,IsPLCreate=@IsPLCreate,IsShipFifo=@IsShipFifo,IsRecFifo=@IsRecFifo,IsShipByOrder=@IsShipByOrder,IsOpenOrder=@IsOpenOrder,IsAsnUniqueRec=@IsAsnUniqueRec,RecGapTo=@RecGapTo,RecTemplate=@RecTemplate,OrderTemplate=@OrderTemplate,AsnTemplate=@AsnTemplate,HuTemplate=@HuTemplate,BillTerm=@BillTerm,CreateHuOpt=@CreateHuOpt,ReCalculatePriceOpt=@ReCalculatePriceOpt,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,ReleaseDate=@ReleaseDate,ReleaseUser=@ReleaseUser,ReleaseUserNm=@ReleaseUserNm,StartDate=@StartDate,StartUser=@StartUser,StartUserNm=@StartUserNm,CompleteDate=@CompleteDate,CompleteUser=@CompleteUser,CompleteUserNm=@CompleteUserNm,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CancelDate=@CancelDate,CancelUser=@CancelUser,CancelUserNm=@CancelUserNm,CancelReason=@CancelReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,OrderStrategy=@OrderStrategy,ProdLineFact=@ProdLineFact,IsQuick=@IsQuick,PickStrategy=@PickStrategy,ExtraDmdSource=@ExtraDmdSource,OrderNo=@OrderNo
		WHERE OrderNo= @OrderNo AND Version= @VersionBerfore
	END
	ELSE
	BEGIN
		UPDATE ORD_OrderMstr_0 SET Version=@Version,Flow=@Flow,FlowDesc=@FlowDesc,TraceCode=@TraceCode,RefOrderNo=@RefOrderNo,ExtOrderNo=@ExtOrderNo,Type=0,SubType=@SubType,QualityType=@QualityType,StartTime=@StartTime,WindowTime=@WindowTime,IsPlanPause=@IsPlanPause,PauseSeq=@PauseSeq,IsPause=@IsPause,PauseTime=@PauseTime,IsPLPause=@IsPLPause,EffDate=@EffDate,Priority=@Priority,Status=@Status,Seq=@Seq,SapSeq=@SapSeq,PartyFrom=@PartyFrom,PartyFromNm=@PartyFromNm,PartyTo=@PartyTo,PartyToNm=@PartyToNm,ShipFrom=@ShipFrom,ShipFromAddr=@ShipFromAddr,ShipFromTel=@ShipFromTel,ShipFromCell=@ShipFromCell,ShipFromFax=@ShipFromFax,ShipFromContact=@ShipFromContact,ShipToAddr=@ShipToAddr,ShipTo=@ShipTo,ShipToTel=@ShipToTel,ShipToCell=@ShipToCell,ShipToFax=@ShipToFax,ShipToContact=@ShipToContact,Shift=@Shift,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,BillAddrDesc=@BillAddrDesc,PriceList=@PriceList,Currency=@Currency,Dock=@Dock,Routing=@Routing,CurtOp=@CurtOp,IsAutoRelease=@IsAutoRelease,IsAutoStart=@IsAutoStart,IsAutoShip=@IsAutoShip,IsAutoReceive=@IsAutoReceive,IsAutoBill=@IsAutoBill,IsManualCreateDet=@IsManualCreateDet,IsListPrice=@IsListPrice,IsPrintOrder=@IsPrintOrder,IsOrderPrinted=@IsOrderPrinted,IsPrintAsn=@IsPrintAsn,IsPrintRec=@IsPrintRec,IsShipExceed=@IsShipExceed,IsRecExceed=@IsRecExceed,IsOrderFulfillUC=@IsOrderFulfillUC,IsShipFulfillUC=@IsShipFulfillUC,IsRecFulfillUC=@IsRecFulfillUC,IsShipScanHu=@IsShipScanHu,IsRecScanHu=@IsRecScanHu,IsCreatePL=@IsCreatePL,IsPLCreate=@IsPLCreate,IsShipFifo=@IsShipFifo,IsRecFifo=@IsRecFifo,IsShipByOrder=@IsShipByOrder,IsOpenOrder=@IsOpenOrder,IsAsnUniqueRec=@IsAsnUniqueRec,RecGapTo=@RecGapTo,RecTemplate=@RecTemplate,OrderTemplate=@OrderTemplate,AsnTemplate=@AsnTemplate,HuTemplate=@HuTemplate,BillTerm=@BillTerm,CreateHuOpt=@CreateHuOpt,ReCalculatePriceOpt=@ReCalculatePriceOpt,WMSNo=@WMSNo,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate,ReleaseDate=@ReleaseDate,ReleaseUser=@ReleaseUser,ReleaseUserNm=@ReleaseUserNm,StartDate=@StartDate,StartUser=@StartUser,StartUserNm=@StartUserNm,CompleteDate=@CompleteDate,CompleteUser=@CompleteUser,CompleteUserNm=@CompleteUserNm,CloseDate=@CloseDate,CloseUser=@CloseUser,CloseUserNm=@CloseUserNm,CancelDate=@CancelDate,CancelUser=@CancelUser,CancelUserNm=@CancelUserNm,CancelReason=@CancelReason,IsCheckPartyFromAuth=@IsCheckPartyFromAuth,IsCheckPartyToAuth=@IsCheckPartyToAuth,OrderStrategy=@OrderStrategy,ProdLineFact=@ProdLineFact,IsQuick=@IsQuick,PickStrategy=@PickStrategy,ExtraDmdSource=@ExtraDmdSource,OrderNo=@OrderNo
		WHERE OrderNo= @OrderNo AND Version= @VersionBerfore
	END	
END
GO
