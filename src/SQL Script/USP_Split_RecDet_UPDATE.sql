/****** Object:  StoredProcedure [dbo].[USP_Split_RecDet_UPDATE]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_RecDet_UPDATE')
	DROP PROCEDURE USP_Split_RecDet_UPDATE
CREATE PROCEDURE [dbo].[USP_Split_RecDet_UPDATE]
(
	@Version int,
	@RecNo varchar(50),
	@Seq int,
	@OrderNo varchar(50),
	@OrderType tinyint,
	@OrderSubType tinyint,
	@OrderDetSeq int,
	@OrderDetId int,
	@IpNo varchar(50),
	@IpDetId int,
	@IpDetSeq int,
	@IpDetType tinyint,
	@IpGapAdjOpt tinyint,
	@ExtNo varchar(50),
	@ExtSeq varchar(50),
	@Flow varchar(50),
	@Item varchar(50),
	@ItemDesc varchar(100),
	@RefItemCode varchar(50),
	@Uom varchar(5),
	@BaseUom varchar(5),
	@UC decimal(18,8),
	@QualityType tinyint,
	@ManufactureParty varchar(50),
	@RecQty decimal(18,8),
	@ScrapQty decimal(18,8),
	@UnitQty decimal(18,8),
	@LocFrom varchar(50),
	@LocFromNm varchar(100),
	@LocTo varchar(50),
	@LocToNm varchar(100),
	@IsInspect bit,
	@BillAddr varchar(50),
	@PriceList varchar(50),
	@UnitPrice decimal(18,8),
	@Currency varchar(50),
	@IsProvEst bit,
	@Tax varchar(50),
	@IsIncludeTax bit,
	@BillTerm tinyint,
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime,
	@Id int,
	@VersionBerfore int
)
AS
BEGIN
	IF @OrderType=1
	BEGIN
		UPDATE ORD_RecDet_1 SET Version=@Version,RecNo=@RecNo,Seq=@Seq,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,IpDetType=@IpDetType,IpGapAdjOpt=@IpGapAdjOpt,ExtNo=@ExtNo,ExtSeq=@ExtSeq,Flow=@Flow,Item=@Item,ItemDesc=@ItemDesc,RefItemCode=@RefItemCode,Uom=@Uom,BaseUom=@BaseUom,UC=@UC,QualityType=@QualityType,ManufactureParty=@ManufactureParty,RecQty=@RecQty,ScrapQty=@ScrapQty,UnitQty=@UnitQty,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,PriceList=@PriceList,UnitPrice=@UnitPrice,Currency=@Currency,IsProvEst=@IsProvEst,Tax=@Tax,IsIncludeTax=@IsIncludeTax,BillTerm=@BillTerm,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=2
	BEGIN
		UPDATE ORD_RecDet_2 SET Version=@Version,RecNo=@RecNo,Seq=@Seq,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,IpDetType=@IpDetType,IpGapAdjOpt=@IpGapAdjOpt,ExtNo=@ExtNo,ExtSeq=@ExtSeq,Flow=@Flow,Item=@Item,ItemDesc=@ItemDesc,RefItemCode=@RefItemCode,Uom=@Uom,BaseUom=@BaseUom,UC=@UC,QualityType=@QualityType,ManufactureParty=@ManufactureParty,RecQty=@RecQty,ScrapQty=@ScrapQty,UnitQty=@UnitQty,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,PriceList=@PriceList,UnitPrice=@UnitPrice,Currency=@Currency,IsProvEst=@IsProvEst,Tax=@Tax,IsIncludeTax=@IsIncludeTax,BillTerm=@BillTerm,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END 
	ELSE IF @OrderType=3
	BEGIN
		UPDATE ORD_RecDet_3 SET Version=@Version,RecNo=@RecNo,Seq=@Seq,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,IpDetType=@IpDetType,IpGapAdjOpt=@IpGapAdjOpt,ExtNo=@ExtNo,ExtSeq=@ExtSeq,Flow=@Flow,Item=@Item,ItemDesc=@ItemDesc,RefItemCode=@RefItemCode,Uom=@Uom,BaseUom=@BaseUom,UC=@UC,QualityType=@QualityType,ManufactureParty=@ManufactureParty,RecQty=@RecQty,ScrapQty=@ScrapQty,UnitQty=@UnitQty,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,PriceList=@PriceList,UnitPrice=@UnitPrice,Currency=@Currency,IsProvEst=@IsProvEst,Tax=@Tax,IsIncludeTax=@IsIncludeTax,BillTerm=@BillTerm,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END 
	ELSE IF @OrderType=4
	BEGIN
		UPDATE ORD_RecDet_4 SET Version=@Version,RecNo=@RecNo,Seq=@Seq,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,IpDetType=@IpDetType,IpGapAdjOpt=@IpGapAdjOpt,ExtNo=@ExtNo,ExtSeq=@ExtSeq,Flow=@Flow,Item=@Item,ItemDesc=@ItemDesc,RefItemCode=@RefItemCode,Uom=@Uom,BaseUom=@BaseUom,UC=@UC,QualityType=@QualityType,ManufactureParty=@ManufactureParty,RecQty=@RecQty,ScrapQty=@ScrapQty,UnitQty=@UnitQty,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,PriceList=@PriceList,UnitPrice=@UnitPrice,Currency=@Currency,IsProvEst=@IsProvEst,Tax=@Tax,IsIncludeTax=@IsIncludeTax,BillTerm=@BillTerm,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END 
	ELSE IF @OrderType=5
	BEGIN
		UPDATE ORD_RecDet_5 SET Version=@Version,RecNo=@RecNo,Seq=@Seq,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,IpDetType=@IpDetType,IpGapAdjOpt=@IpGapAdjOpt,ExtNo=@ExtNo,ExtSeq=@ExtSeq,Flow=@Flow,Item=@Item,ItemDesc=@ItemDesc,RefItemCode=@RefItemCode,Uom=@Uom,BaseUom=@BaseUom,UC=@UC,QualityType=@QualityType,ManufactureParty=@ManufactureParty,RecQty=@RecQty,ScrapQty=@ScrapQty,UnitQty=@UnitQty,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,PriceList=@PriceList,UnitPrice=@UnitPrice,Currency=@Currency,IsProvEst=@IsProvEst,Tax=@Tax,IsIncludeTax=@IsIncludeTax,BillTerm=@BillTerm,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END 
	ELSE IF @OrderType=6
	BEGIN
		UPDATE ORD_RecDet_6 SET Version=@Version,RecNo=@RecNo,Seq=@Seq,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,IpDetType=@IpDetType,IpGapAdjOpt=@IpGapAdjOpt,ExtNo=@ExtNo,ExtSeq=@ExtSeq,Flow=@Flow,Item=@Item,ItemDesc=@ItemDesc,RefItemCode=@RefItemCode,Uom=@Uom,BaseUom=@BaseUom,UC=@UC,QualityType=@QualityType,ManufactureParty=@ManufactureParty,RecQty=@RecQty,ScrapQty=@ScrapQty,UnitQty=@UnitQty,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,PriceList=@PriceList,UnitPrice=@UnitPrice,Currency=@Currency,IsProvEst=@IsProvEst,Tax=@Tax,IsIncludeTax=@IsIncludeTax,BillTerm=@BillTerm,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END 
	ELSE IF @OrderType=7
	BEGIN
		UPDATE ORD_RecDet_7 SET Version=@Version,RecNo=@RecNo,Seq=@Seq,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,IpDetType=@IpDetType,IpGapAdjOpt=@IpGapAdjOpt,ExtNo=@ExtNo,ExtSeq=@ExtSeq,Flow=@Flow,Item=@Item,ItemDesc=@ItemDesc,RefItemCode=@RefItemCode,Uom=@Uom,BaseUom=@BaseUom,UC=@UC,QualityType=@QualityType,ManufactureParty=@ManufactureParty,RecQty=@RecQty,ScrapQty=@ScrapQty,UnitQty=@UnitQty,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,PriceList=@PriceList,UnitPrice=@UnitPrice,Currency=@Currency,IsProvEst=@IsProvEst,Tax=@Tax,IsIncludeTax=@IsIncludeTax,BillTerm=@BillTerm,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END 
	ELSE IF @OrderType=8
	BEGIN
		UPDATE ORD_RecDet_8 SET Version=@Version,RecNo=@RecNo,Seq=@Seq,OrderNo=@OrderNo,OrderType=@OrderType,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,IpDetType=@IpDetType,IpGapAdjOpt=@IpGapAdjOpt,ExtNo=@ExtNo,ExtSeq=@ExtSeq,Flow=@Flow,Item=@Item,ItemDesc=@ItemDesc,RefItemCode=@RefItemCode,Uom=@Uom,BaseUom=@BaseUom,UC=@UC,QualityType=@QualityType,ManufactureParty=@ManufactureParty,RecQty=@RecQty,ScrapQty=@ScrapQty,UnitQty=@UnitQty,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,PriceList=@PriceList,UnitPrice=@UnitPrice,Currency=@Currency,IsProvEst=@IsProvEst,Tax=@Tax,IsIncludeTax=@IsIncludeTax,BillTerm=@BillTerm,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END 
	ELSE
	BEGIN
		UPDATE ORD_RecDet_0 SET Version=@Version,RecNo=@RecNo,Seq=@Seq,OrderNo=@OrderNo,OrderType=0,OrderSubType=@OrderSubType,OrderDetSeq=@OrderDetSeq,OrderDetId=@OrderDetId,IpNo=@IpNo,IpDetId=@IpDetId,IpDetSeq=@IpDetSeq,IpDetType=@IpDetType,IpGapAdjOpt=@IpGapAdjOpt,ExtNo=@ExtNo,ExtSeq=@ExtSeq,Flow=@Flow,Item=@Item,ItemDesc=@ItemDesc,RefItemCode=@RefItemCode,Uom=@Uom,BaseUom=@BaseUom,UC=@UC,QualityType=@QualityType,ManufactureParty=@ManufactureParty,RecQty=@RecQty,ScrapQty=@ScrapQty,UnitQty=@UnitQty,LocFrom=@LocFrom,LocFromNm=@LocFromNm,LocTo=@LocTo,LocToNm=@LocToNm,IsInspect=@IsInspect,BillAddr=@BillAddr,PriceList=@PriceList,UnitPrice=@UnitPrice,Currency=@Currency,IsProvEst=@IsProvEst,Tax=@Tax,IsIncludeTax=@IsIncludeTax,BillTerm=@BillTerm,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END 
END
GO