/****** Object:  StoredProcedure [dbo].[USP_Split_IpDet_INSERT]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_IpDet_INSERT')
BEGIN
	DROP PROCEDURE USP_Split_IpDet_INSERT
END
GO

CREATE PROCEDURE [dbo].[USP_Split_IpDet_INSERT]
(
	@Version int,
	@Type tinyint,
	@IpNo varchar(50),
	@Seq int,
	@OrderNo varchar(50),
	@OrderType tinyint,
	@OrderSubType tinyint,
	@OrderDetSeq int,
	@OrderDetId int,
	@ExtNo varchar(50),
	@ExtSeq varchar(50),
	@Flow varchar(50),
	@Item varchar(50),
	@ItemDesc varchar(100),
	@RefItemCode varchar(50),
	@Uom varchar(5),
	@BaseUom varchar(5),
	@UC decimal(18,8),
	@UCDesc varchar(50),
	@StartTime datetime,
	@WindowTime datetime,
	@QualityType tinyint,
	@ManufactureParty varchar(50),
	@Qty decimal(18,8),
	@RecQty decimal(18,8),
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
	@IsClose bit,
	@GapRecNo varchar(50),
	@GapIpDetId int,
	@BinTo varchar(50),
	@IsScanHu bit,
	@CreateUser int,
	@CreateUserNm varchar(100),
	@CreateDate datetime,
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime,
	@Container varchar(50),
	@ContainerDesc varchar(50),
	@IsChangeUC bit,
	@PalletLotSize decimal(18,8),
	@PackageVolumn decimal(18,8),
	@PackageWeight decimal(18,8)
)
AS
BEGIN
    SELECT @IpNo=LTRIM(RTRIM(@IpNo)),@OrderNo=LTRIM(RTRIM(@OrderNo)),@ExtNo=LTRIM(RTRIM(@ExtNo)),@ExtSeq=LTRIM(RTRIM(@ExtSeq)),@Flow=LTRIM(RTRIM(@Flow)),
		@Item=LTRIM(RTRIM(@Item)),@ItemDesc=LTRIM(RTRIM(@ItemDesc)),@RefItemCode=LTRIM(RTRIM(@RefItemCode)),@Uom=LTRIM(RTRIM(@Uom)),
		@BaseUom=LTRIM(RTRIM(@BaseUom)),@UCDesc=LTRIM(RTRIM(@UCDesc)),@ManufactureParty=LTRIM(RTRIM(@ManufactureParty)),@LocFrom=LTRIM(RTRIM(@LocFrom)),
		@LocFromNm=LTRIM(RTRIM(@LocFromNm)),@LocTo=LTRIM(RTRIM(@LocTo)),@LocToNm=LTRIM(RTRIM(@LocToNm)),@BillAddr=LTRIM(RTRIM(@BillAddr)),
		@PriceList=LTRIM(RTRIM(@PriceList)),@Currency=LTRIM(RTRIM(@Currency)),@Tax=LTRIM(RTRIM(@Tax)),@GapRecNo=LTRIM(RTRIM(@GapRecNo)),
		@BinTo=LTRIM(RTRIM(@BinTo)),@CreateUserNm=LTRIM(RTRIM(@CreateUserNm)),@LastModifyUserNm=LTRIM(RTRIM(@LastModifyUserNm)),
		@Container=LTRIM(RTRIM(@Container)),@ContainerDesc=LTRIM(RTRIM(@ContainerDesc))
	DECLARE @Id bigint
	BEGIN TRAN
		IF EXISTS (SELECT * FROM SYS_TabIdSeq WITH (UPDLOCK,SERIALIZABLE) WHERE TabNm='ORD_IpDet')
		BEGIN
			SELECT @Id=Id+1 FROM SYS_TabIdSeq WHERE TabNm='ORD_IpDet'
			UPDATE SYS_TabIdSeq SET Id=Id+1 WHERE TabNm='ORD_IpDet'
		END
		ELSE
		BEGIN
			INSERT INTO SYS_TabIdSeq(TabNm,Id)
			VALUES('ORD_IpDet',1)
			SET @Id=1
		END
	COMMIT TRAN

	IF @OrderType=1
	BEGIN
		INSERT INTO ORD_IpDet_1(Id,Version,Type,IpNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,StartTime,WindowTime,QualityType,ManufactureParty,Qty,RecQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,IsClose,GapRecNo,GapIpDetId,BinTo,IsScanHu,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,IsChangeUC,PalletLotSize,PackageVolumn,PackageWeight)
		VALUES(@Id,@Version,@Type,@IpNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@StartTime,@WindowTime,@QualityType,@ManufactureParty,@Qty,@RecQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@IsClose,@GapRecNo,@GapIpDetId,@BinTo,@IsScanHu,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@IsChangeUC,@PalletLotSize,@PackageVolumn,@PackageWeight)
	END	
	ELSE IF @OrderType=2
	BEGIN
		INSERT INTO ORD_IpDet_2(Id,Version,Type,IpNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,StartTime,WindowTime,QualityType,ManufactureParty,Qty,RecQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,IsClose,GapRecNo,GapIpDetId,BinTo,IsScanHu,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,IsChangeUC,PalletLotSize,PackageVolumn,PackageWeight)
		VALUES(@Id,@Version,@Type,@IpNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@StartTime,@WindowTime,@QualityType,@ManufactureParty,@Qty,@RecQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@IsClose,@GapRecNo,@GapIpDetId,@BinTo,@IsScanHu,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@IsChangeUC,@PalletLotSize,@PackageVolumn,@PackageWeight)
	END	
	ELSE IF @OrderType=3
	BEGIN
		INSERT INTO ORD_IpDet_3(Id,Version,Type,IpNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,StartTime,WindowTime,QualityType,ManufactureParty,Qty,RecQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,IsClose,GapRecNo,GapIpDetId,BinTo,IsScanHu,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,IsChangeUC,PalletLotSize,PackageVolumn,PackageWeight)
		VALUES(@Id,@Version,@Type,@IpNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@StartTime,@WindowTime,@QualityType,@ManufactureParty,@Qty,@RecQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@IsClose,@GapRecNo,@GapIpDetId,@BinTo,@IsScanHu,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@IsChangeUC,@PalletLotSize,@PackageVolumn,@PackageWeight)
	END	
	ELSE IF @OrderType=4
	BEGIN
		INSERT INTO ORD_IpDet_4(Id,Version,Type,IpNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,StartTime,WindowTime,QualityType,ManufactureParty,Qty,RecQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,IsClose,GapRecNo,GapIpDetId,BinTo,IsScanHu,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,IsChangeUC,PalletLotSize,PackageVolumn,PackageWeight)
		VALUES(@Id,@Version,@Type,@IpNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@StartTime,@WindowTime,@QualityType,@ManufactureParty,@Qty,@RecQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@IsClose,@GapRecNo,@GapIpDetId,@BinTo,@IsScanHu,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@IsChangeUC,@PalletLotSize,@PackageVolumn,@PackageWeight)
	END	
	ELSE IF @OrderType=5
	BEGIN
		INSERT INTO ORD_IpDet_5(Id,Version,Type,IpNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,StartTime,WindowTime,QualityType,ManufactureParty,Qty,RecQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,IsClose,GapRecNo,GapIpDetId,BinTo,IsScanHu,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,IsChangeUC,PalletLotSize,PackageVolumn,PackageWeight)
		VALUES(@Id,@Version,@Type,@IpNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@StartTime,@WindowTime,@QualityType,@ManufactureParty,@Qty,@RecQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@IsClose,@GapRecNo,@GapIpDetId,@BinTo,@IsScanHu,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@IsChangeUC,@PalletLotSize,@PackageVolumn,@PackageWeight)
	END	
	ELSE IF @OrderType=6
	BEGIN
		INSERT INTO ORD_IpDet_6(Id,Version,Type,IpNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,StartTime,WindowTime,QualityType,ManufactureParty,Qty,RecQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,IsClose,GapRecNo,GapIpDetId,BinTo,IsScanHu,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,IsChangeUC,PalletLotSize,PackageVolumn,PackageWeight)
		VALUES(@Id,@Version,@Type,@IpNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@StartTime,@WindowTime,@QualityType,@ManufactureParty,@Qty,@RecQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@IsClose,@GapRecNo,@GapIpDetId,@BinTo,@IsScanHu,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@IsChangeUC,@PalletLotSize,@PackageVolumn,@PackageWeight)
	END	
	ELSE IF @OrderType=7
	BEGIN
		INSERT INTO ORD_IpDet_7(Id,Version,Type,IpNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,StartTime,WindowTime,QualityType,ManufactureParty,Qty,RecQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,IsClose,GapRecNo,GapIpDetId,BinTo,IsScanHu,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,IsChangeUC,PalletLotSize,PackageVolumn,PackageWeight)
		VALUES(@Id,@Version,@Type,@IpNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@StartTime,@WindowTime,@QualityType,@ManufactureParty,@Qty,@RecQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@IsClose,@GapRecNo,@GapIpDetId,@BinTo,@IsScanHu,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@IsChangeUC,@PalletLotSize,@PackageVolumn,@PackageWeight)
	END
	ELSE IF @OrderType=8
	BEGIN
		INSERT INTO ORD_IpDet_8(Id,Version,Type,IpNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,StartTime,WindowTime,QualityType,ManufactureParty,Qty,RecQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,IsClose,GapRecNo,GapIpDetId,BinTo,IsScanHu,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,IsChangeUC,PalletLotSize,PackageVolumn,PackageWeight)
		VALUES(@Id,@Version,@Type,@IpNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@StartTime,@WindowTime,@QualityType,@ManufactureParty,@Qty,@RecQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@IsClose,@GapRecNo,@GapIpDetId,@BinTo,@IsScanHu,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@IsChangeUC,@PalletLotSize,@PackageVolumn,@PackageWeight)
	END				
	ELSE
	BEGIN
		INSERT INTO ORD_IpDet_0(Id,Version,Type,IpNo,Seq,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,ExtNo,ExtSeq,Flow,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,StartTime,WindowTime,QualityType,ManufactureParty,Qty,RecQty,UnitQty,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,PriceList,UnitPrice,Currency,IsProvEst,Tax,IsIncludeTax,BillTerm,IsClose,GapRecNo,GapIpDetId,BinTo,IsScanHu,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,IsChangeUC,PalletLotSize,PackageVolumn,PackageWeight)
		VALUES(@Id,@Version,@Type,@IpNo,@Seq,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@ExtNo,@ExtSeq,@Flow,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@StartTime,@WindowTime,@QualityType,@ManufactureParty,@Qty,@RecQty,@UnitQty,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@PriceList,@UnitPrice,@Currency,@IsProvEst,@Tax,@IsIncludeTax,@BillTerm,@IsClose,@GapRecNo,@GapIpDetId,@BinTo,@IsScanHu,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@IsChangeUC,@PalletLotSize,@PackageVolumn,@PackageWeight)
	END		
	
	SELECT @Id							
END
GO