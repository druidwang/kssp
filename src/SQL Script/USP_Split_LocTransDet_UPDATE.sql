/****** Object:  StoredProcedure [dbo].[USP_Split_LocTransDet_UPDATE]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.Objects WHERE TYPE='p' AND name='USP_Split_LocTransDet_UPDATE')
	DROP PROCEDURE USP_Split_LocTransDet_UPDATE
CREATE PROCEDURE [dbo].[USP_Split_LocTransDet_UPDATE]
(
	@LocTransId bigint,
	@OrderNo varchar(50),
	@OrderType tinyint,
	@OrderSubType tinyint,
	@OrderDetSeq int,
	@OrderDetId int,
	@OrderBomDetId int,
	@OrderBomDetSeq int,
	@IpNo varchar(50),
	@IpDetId int,
	@IpDetSeq int,
	@RecNo varchar(50),
	@RecDetId int,
	@RecDetSeq int,
	@SeqNo varchar(50),
	@BillTransId int,
	@LocLotDetId int,
	@TraceCode varchar(50),
	@Item varchar(50),
	@Qty decimal(18,8),
	@IsCS bit,
	@PlanBill int,
	@PlanBillQty decimal(18,8),
	@ActBill int,
	@ActBillQty decimal(18,8),
	@QualityType tinyint,
	@HuId varchar(50),
	@LotNo varchar(50),
	@TransType int,
	@IOType tinyint,
	@PartyFrom varchar(50),
	@PartyTo varchar(50),
	@LocFrom varchar(50),
	@LocTo varchar(50),
	@Bin varchar(50),
	@PlanBackflushId int,
	@LocIOReason varchar(50),
	@EffDate datetime,
	@Id int
)
AS
BEGIN
	IF @TransType=301
	BEGIN
		UPDATE INV_LocTransDet_1 SET LocTransId=@LocTransId,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,OrderBomDetId=@OrderBomDetId,OrderBomDetSeq=@OrderBomDetSeq,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,RecNo=@RecNo,RecDetId=@RecDetId,RecDetSeq=@RecDetSeq,SeqNo=@SeqNo,BillTransId=@BillTransId,LocLotDetId=@LocLotDetId,TraceCode=@TraceCode,Item=@Item,Qty=@Qty,IsCS=@IsCS,PlanBill=@PlanBill,PlanBillQty=@PlanBillQty,ActBill=@ActBill,ActBillQty=@ActBillQty,QualityType=@QualityType,HuId=@HuId,LotNo=@LotNo,TransType=@TransType,IOType=@IOType,PartyFrom=@PartyFrom,PartyTo=@PartyTo,LocFrom=@LocFrom,LocTo=@LocTo,Bin=@Bin,PlanBackflushId=@PlanBackflushId,LocIOReason=@LocIOReason,EffDate=@EffDate
		WHERE Id=@Id
	END
	ELSE
	BEGIN
		UPDATE INV_LocTransDet_0 SET LocTransId=@LocTransId,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,OrderBomDetId=@OrderBomDetId,OrderBomDetSeq=@OrderBomDetSeq,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,RecNo=@RecNo,RecDetId=@RecDetId,RecDetSeq=@RecDetSeq,SeqNo=@SeqNo,BillTransId=@BillTransId,LocLotDetId=@LocLotDetId,TraceCode=@TraceCode,Item=@Item,Qty=@Qty,IsCS=@IsCS,PlanBill=@PlanBill,PlanBillQty=@PlanBillQty,ActBill=@ActBill,ActBillQty=@ActBillQty,QualityType=@QualityType,HuId=@HuId,LotNo=@LotNo,TransType=@TransType,IOType=@IOType,PartyFrom=@PartyFrom,PartyTo=@PartyTo,LocFrom=@LocFrom,LocTo=@LocTo,Bin=@Bin,PlanBackflushId=@PlanBackflushId,LocIOReason=@LocIOReason,EffDate=@EffDate
		WHERE Id=@Id
	END
END
GO