
/****** Object:  StoredProcedure [dbo].[USP_Split_RecDet_INSERT]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_RecDet_INSERT')
	DROP PROCEDURE USP_Split_RecDet_INSERT
CREATE PROCEDURE [dbo].[USP_Split_RecDet_INSERT]
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
	@CreateUser int,
	@CreateUserNm varchar(100),
	@CreateDate datetime,
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime
)
AS
BEGIN
	DECLARE @Id bigint
	BEGIN TRAN
	IF EXISTS (SELECT * FROM SYS_TabIdSeq WITH (UPDLOCK,SERIALIZABLE) WHERE TabNm='ORD_OrderDet')
	BEGIN
		SELECT @Id=Id+1 FROM SYS_TabIdSeq WHERE TabNm='ORD_OrderDet'
		UPDATE SYS_TabIdSeq SET Id=Id+1 WHERE TabNm='ORD_OrderDet'
	END
	ELSE
	BEGIN
		INSERT INTO SYS_TabIdSeq(TabNm,Id)
		VALUES('ORD_OrderDet',1)
		SET @Id=1
	END
	COMMIT TRAN

	IF @OrderType=1
	BEGIN
		INSERT INTO ORD_RecDet_1(Id,Version,RecNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,IpNo,IpDetId,IpDetSeq,IpDetType,IpGapAdjOpt,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,QualityType,ManufactureParty,RecQty,ScrapQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@Version,@RecNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@IpNo,@IpDetId,@IpDetSeq,@IpDetType,@IpGapAdjOpt,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@QualityType,@ManufactureParty,@RecQty,@ScrapQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=2
	BEGIN
		INSERT INTO ORD_RecDet_2(Id,Version,RecNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,IpNo,IpDetId,IpDetSeq,IpDetType,IpGapAdjOpt,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,QualityType,ManufactureParty,RecQty,ScrapQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@Version,@RecNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@IpNo,@IpDetId,@IpDetSeq,@IpDetType,@IpGapAdjOpt,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@QualityType,@ManufactureParty,@RecQty,@ScrapQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=3
	BEGIN
		INSERT INTO ORD_RecDet_3(Id,Version,RecNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,IpNo,IpDetId,IpDetSeq,IpDetType,IpGapAdjOpt,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,QualityType,ManufactureParty,RecQty,ScrapQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@Version,@RecNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@IpNo,@IpDetId,@IpDetSeq,@IpDetType,@IpGapAdjOpt,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@QualityType,@ManufactureParty,@RecQty,@ScrapQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=4
	BEGIN
		INSERT INTO ORD_RecDet_4(Id,Version,RecNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,IpNo,IpDetId,IpDetSeq,IpDetType,IpGapAdjOpt,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,QualityType,ManufactureParty,RecQty,ScrapQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@Version,@RecNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@IpNo,@IpDetId,@IpDetSeq,@IpDetType,@IpGapAdjOpt,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@QualityType,@ManufactureParty,@RecQty,@ScrapQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=5
	BEGIN
		INSERT INTO ORD_RecDet_5(Id,Version,RecNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,IpNo,IpDetId,IpDetSeq,IpDetType,IpGapAdjOpt,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,QualityType,ManufactureParty,RecQty,ScrapQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@Version,@RecNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@IpNo,@IpDetId,@IpDetSeq,@IpDetType,@IpGapAdjOpt,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@QualityType,@ManufactureParty,@RecQty,@ScrapQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=6
	BEGIN
		INSERT INTO ORD_RecDet_6(Id,Version,RecNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,IpNo,IpDetId,IpDetSeq,IpDetType,IpGapAdjOpt,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,QualityType,ManufactureParty,RecQty,ScrapQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@Version,@RecNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@IpNo,@IpDetId,@IpDetSeq,@IpDetType,@IpGapAdjOpt,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@QualityType,@ManufactureParty,@RecQty,@ScrapQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=7
	BEGIN
		INSERT INTO ORD_RecDet_7(Id,Version,RecNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,IpNo,IpDetId,IpDetSeq,IpDetType,IpGapAdjOpt,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,QualityType,ManufactureParty,RecQty,ScrapQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@Version,@RecNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@IpNo,@IpDetId,@IpDetSeq,@IpDetType,@IpGapAdjOpt,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@QualityType,@ManufactureParty,@RecQty,@ScrapQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=8
	BEGIN
		INSERT INTO ORD_RecDet_8(Id,Version,RecNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,IpNo,IpDetId,IpDetSeq,IpDetType,IpGapAdjOpt,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,QualityType,ManufactureParty,RecQty,ScrapQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@Version,@RecNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@IpNo,@IpDetId,@IpDetSeq,@IpDetType,@IpGapAdjOpt,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@QualityType,@ManufactureParty,@RecQty,@ScrapQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE 
	BEGIN
		INSERT INTO ORD_RecDet_0(Id,Version,RecNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,IpNo,IpDetId,IpDetSeq,IpDetType,IpGapAdjOpt,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,QualityType,ManufactureParty,RecQty,ScrapQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@Version,@RecNo,@Seq,@OrderNo,0,@OrderSubType,@OrderDetSeq,@OrderDetId,@IpNo,@IpDetId,@IpDetSeq,@IpDetType,@IpGapAdjOpt,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@QualityType,@ManufactureParty,@RecQty,@ScrapQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END      
	SELECT @Id  
END
GO