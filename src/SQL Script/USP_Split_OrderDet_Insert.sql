SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_OrderDet_Insert')
BEGIN
	DROP PROCEDURE USP_Split_OrderDet_Insert
END
GO

CREATE PROCEDURE [dbo].[USP_Split_OrderDet_Insert]
(
 @Version int,
 @OrderNo varchar(50),
 @OrderType tinyint,
 @OrderSubType tinyint,
 @Seq int,
 @ExtNo varchar(50),
 @ExtSeq varchar(50),
 @StartDate datetime,
 @EndDate datetime,
 @ScheduleType tinyint,
 @Item varchar(50),
 @ItemDesc varchar(100),
 @RefItemCode varchar(50),
 @Uom varchar(5),
 @BaseUom varchar(5),
 @UC decimal(18,8),
 @UCDesc varchar(50),
 @MinUC decimal(18,8),
 @QualityType tinyint,
 @ManufactureParty varchar(50),
 @ReqQty decimal(18,8),
 @OrderQty decimal(18,8),
 @ShipQty decimal(18,8),
 @RecQty decimal(18,8),
 @RejQty decimal(18,8),
 @ScrapQty decimal(18,8),
 @PickQty decimal(18,8),
 @UnitQty decimal(18,8),
 @RecLotSize decimal(18,8),
 @LocFrom varchar(50),
 @LocFromNm varchar(100),
 @LocTo varchar(50),
 @LocToNm varchar(100),
 @IsInspect bit,
 @BillAddr varchar(50),
 @BillAddrDesc varchar(256),
 @PriceList varchar(50),
 @UnitPrice decimal(18,8),
 @IsProvEst bit,
 @Tax varchar(50),
 @IsIncludeTax bit,
 @Currency varchar(50),
 @Bom varchar(50),
 @Routing varchar(50),
 @BillTerm tinyint,
 @IsScanHu bit,
 @ReserveNo varchar(50),
 @ReserveLine varchar(50),
 @ZOPWZ varchar(50),
 @ZOPID varchar(50),
 @ZOPDS varchar(50),
 @BinTo varchar(50),
 @WMSSeq varchar(50), 
 @CreateUser int,
 @CreateUserNm varchar(100),
 @CreateDate datetime,
 @LastModifyUser int,
 @LastModifyUserNm varchar(100),
 @LastModifyDate datetime,
 @Container varchar(4000),
 @ContainerDesc varchar(50),
 @PickStrategy varchar(50),
 @ExtraDmdSource varchar(256),
 @IsChangeUC bit,
 @AUFNR varchar(50),
 @ICHARG varchar(50),
 @BWART varchar(50),
 @Direction varchar(50), 
 @Remark varchar(255),
 @PalletLotSize decimal(18,8),
 @PackageVolumn decimal(18,8),
 @PackageWeight decimal(18,8)
)
AS
BEGIN
 SET NOCOUNT ON
 SELECT @OrderNo=LTRIM(RTRIM(@OrderNo)),@ExtNo=LTRIM(RTRIM(@ExtNo)),@ExtSeq=LTRIM(RTRIM(@ExtSeq)),@Item=LTRIM(RTRIM(@Item)),
	 @ItemDesc=LTRIM(RTRIM(@ItemDesc)),@RefItemCode=LTRIM(RTRIM(@RefItemCode)),@Uom=LTRIM(RTRIM(@Uom)),@BaseUom=LTRIM(RTRIM(@BaseUom)),
	 @UCDesc=LTRIM(RTRIM(@UCDesc)),@ManufactureParty=LTRIM(RTRIM(@ManufactureParty)),@LocFrom=LTRIM(RTRIM(@LocFrom)),@LocFromNm=LTRIM(RTRIM(@LocFromNm)),
	 @LocTo=LTRIM(RTRIM(@LocTo)),@LocToNm=LTRIM(RTRIM(@LocToNm)),@BillAddr=LTRIM(RTRIM(@BillAddr)),@BillAddrDesc=LTRIM(RTRIM(@BillAddrDesc)),
	 @PriceList=LTRIM(RTRIM(@PriceList)),@Tax=LTRIM(RTRIM(@Tax)),@Currency=LTRIM(RTRIM(@Currency)),@Bom=LTRIM(RTRIM(@Bom)),@Routing=LTRIM(RTRIM(@Routing)),
	 @ReserveNo=LTRIM(RTRIM(@ReserveNo)),@ReserveLine=LTRIM(RTRIM(@ReserveLine)),@ZOPWZ=LTRIM(RTRIM(@ZOPWZ)),@ZOPID=LTRIM(RTRIM(@ZOPID)),
	 @ZOPDS=LTRIM(RTRIM(@ZOPDS)),@BinTo=LTRIM(RTRIM(@BinTo)),@WMSSeq=LTRIM(RTRIM(@WMSSeq)),@CreateUserNm=LTRIM(RTRIM(@CreateUserNm)),
	 @LastModifyUserNm=LTRIM(RTRIM(@LastModifyUserNm)),@Container=LTRIM(RTRIM(@Container)),@ContainerDesc=LTRIM(RTRIM(@ContainerDesc)),
	 @PickStrategy=LTRIM(RTRIM(@PickStrategy)),@ExtraDmdSource=LTRIM(RTRIM(@ExtraDmdSource)),@AUFNR=LTRIM(RTRIM(@AUFNR)),@ICHARG=LTRIM(RTRIM(@ICHARG)),
	 @BWART=LTRIM(RTRIM(@BWART)),@Direction=LTRIM(RTRIM(@Direction)),@Remark=LTRIM(RTRIM(@Remark))
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
 
 ---select @Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource
 
 IF @OrderType=1
 BEGIN
  INSERT INTO ORD_OrderDet_1(Id,Version,OrderNo,OrderType,OrderSubType,Seq,ExtNo,ExtSeq,StartDate,EndDate,ScheduleType,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,MinUC,QualityType,ManufactureParty,ReqQty,OrderQty,ShipQty,RecQty,RejQty,ScrapQty,PickQty,UnitQty,RecLotSize,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,UnitPrice,IsProvEst,Tax,IsIncludeTax,Currency,Bom,Routing,BillTerm,IsScanHu,ReserveNo,ReserveLine,ZOPWZ,ZOPID,ZOPDS,BinTo,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,PickStrategy,ExtraDmdSource,IsChangeUC,AUFNR,ICHARG,BWART,Direction,Remark,PalletLotSize,PackageVolumn,PackageWeight)
  VALUES(@Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource,@IsChangeUC,@AUFNR,@ICHARG,@BWART,@Direction,@Remark,@PalletLotSize,@PackageVolumn,@PackageWeight)
 END
 ELSE IF @OrderType=2
 BEGIN
  INSERT INTO ORD_OrderDet_2(Id,Version,OrderNo,OrderType,OrderSubType,Seq,ExtNo,ExtSeq,StartDate,EndDate,ScheduleType,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,MinUC,QualityType,ManufactureParty,ReqQty,OrderQty,ShipQty,RecQty,RejQty,ScrapQty,PickQty,UnitQty,RecLotSize,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,UnitPrice,IsProvEst,Tax,IsIncludeTax,Currency,Bom,Routing,BillTerm,IsScanHu,ReserveNo,ReserveLine,ZOPWZ,ZOPID,ZOPDS,BinTo,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,PickStrategy,ExtraDmdSource,IsChangeUC,AUFNR,ICHARG,BWART,Direction,Remark,PalletLotSize,PackageVolumn,PackageWeight)
  VALUES(@Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource,@IsChangeUC,@AUFNR,@ICHARG,@BWART,@Direction,@Remark,@PalletLotSize,@PackageVolumn,@PackageWeight)
 END
 ELSE IF @OrderType=3
 BEGIN
  INSERT INTO ORD_OrderDet_3(Id,Version,OrderNo,OrderType,OrderSubType,Seq,ExtNo,ExtSeq,StartDate,EndDate,ScheduleType,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,MinUC,QualityType,ManufactureParty,ReqQty,OrderQty,ShipQty,RecQty,RejQty,ScrapQty,PickQty,UnitQty,RecLotSize,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,UnitPrice,IsProvEst,Tax,IsIncludeTax,Currency,Bom,Routing,BillTerm,IsScanHu,ReserveNo,ReserveLine,ZOPWZ,ZOPID,ZOPDS,BinTo,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,PickStrategy,ExtraDmdSource,IsChangeUC,AUFNR,ICHARG,BWART,Direction,Remark,PalletLotSize,PackageVolumn,PackageWeight)
  VALUES(@Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource,@IsChangeUC,@AUFNR,@ICHARG,@BWART,@Direction,@Remark,@PalletLotSize,@PackageVolumn,@PackageWeight)
 END
 ELSE IF @OrderType=4
 BEGIN
  INSERT INTO ORD_OrderDet_4(Id,Version,OrderNo,OrderType,OrderSubType,Seq,ExtNo,ExtSeq,StartDate,EndDate,ScheduleType,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,MinUC,QualityType,ManufactureParty,ReqQty,OrderQty,ShipQty,RecQty,RejQty,ScrapQty,PickQty,UnitQty,RecLotSize,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,UnitPrice,IsProvEst,Tax,IsIncludeTax,Currency,Bom,Routing,BillTerm,IsScanHu,ReserveNo,ReserveLine,ZOPWZ,ZOPID,ZOPDS,BinTo,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,PickStrategy,ExtraDmdSource,IsChangeUC,AUFNR,ICHARG,BWART,Direction,Remark,PalletLotSize,PackageVolumn,PackageWeight)
  VALUES(@Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource,@IsChangeUC,@AUFNR,@ICHARG,@BWART,@Direction,@Remark,@PalletLotSize,@PackageVolumn,@PackageWeight)
 END
 ELSE IF @OrderType=5
 BEGIN
  INSERT INTO ORD_OrderDet_5(Id,Version,OrderNo,OrderType,OrderSubType,Seq,ExtNo,ExtSeq,StartDate,EndDate,ScheduleType,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,MinUC,QualityType,ManufactureParty,ReqQty,OrderQty,ShipQty,RecQty,RejQty,ScrapQty,PickQty,UnitQty,RecLotSize,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,UnitPrice,IsProvEst,Tax,IsIncludeTax,Currency,Bom,Routing,BillTerm,IsScanHu,ReserveNo,ReserveLine,ZOPWZ,ZOPID,ZOPDS,BinTo,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,PickStrategy,ExtraDmdSource,IsChangeUC,AUFNR,ICHARG,BWART,Direction,Remark,PalletLotSize,PackageVolumn,PackageWeight)
  VALUES(@Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource,@IsChangeUC,@AUFNR,@ICHARG,@BWART,@Direction,@Remark,@PalletLotSize,@PackageVolumn,@PackageWeight)
 END
 ELSE IF @OrderType=6
 BEGIN
  INSERT INTO ORD_OrderDet_6(Id,Version,OrderNo,OrderType,OrderSubType,Seq,ExtNo,ExtSeq,StartDate,EndDate,ScheduleType,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,MinUC,QualityType,ManufactureParty,ReqQty,OrderQty,ShipQty,RecQty,RejQty,ScrapQty,PickQty,UnitQty,RecLotSize,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,UnitPrice,IsProvEst,Tax,IsIncludeTax,Currency,Bom,Routing,BillTerm,IsScanHu,ReserveNo,ReserveLine,ZOPWZ,ZOPID,ZOPDS,BinTo,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,PickStrategy,ExtraDmdSource,IsChangeUC,AUFNR,ICHARG,BWART,Direction,Remark,PalletLotSize,PackageVolumn,PackageWeight)
  VALUES(@Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource,@IsChangeUC,@AUFNR,@ICHARG,@BWART,@Direction,@Remark,@PalletLotSize,@PackageVolumn,@PackageWeight)
 END
 ELSE IF @OrderType=7
 BEGIN
  INSERT INTO ORD_OrderDet_7(Id,Version,OrderNo,OrderType,OrderSubType,Seq,ExtNo,ExtSeq,StartDate,EndDate,ScheduleType,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,MinUC,QualityType,ManufactureParty,ReqQty,OrderQty,ShipQty,RecQty,RejQty,ScrapQty,PickQty,UnitQty,RecLotSize,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,UnitPrice,IsProvEst,Tax,IsIncludeTax,Currency,Bom,Routing,BillTerm,IsScanHu,ReserveNo,ReserveLine,ZOPWZ,ZOPID,ZOPDS,BinTo,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,PickStrategy,ExtraDmdSource,IsChangeUC,AUFNR,ICHARG,BWART,Direction,Remark,PalletLotSize,PackageVolumn,PackageWeight)
  VALUES(@Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource,@IsChangeUC,@AUFNR,@ICHARG,@BWART,@Direction,@Remark,@PalletLotSize,@PackageVolumn,@PackageWeight)
 END
 ELSE IF @OrderType=8
 BEGIN
  INSERT INTO ORD_OrderDet_8(Id,Version,OrderNo,OrderType,OrderSubType,Seq,ExtNo,ExtSeq,StartDate,EndDate,ScheduleType,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,MinUC,QualityType,ManufactureParty,ReqQty,OrderQty,ShipQty,RecQty,RejQty,ScrapQty,PickQty,UnitQty,RecLotSize,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,UnitPrice,IsProvEst,Tax,IsIncludeTax,Currency,Bom,Routing,BillTerm,IsScanHu,ReserveNo,ReserveLine,ZOPWZ,ZOPID,ZOPDS,BinTo,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,PickStrategy,ExtraDmdSource,IsChangeUC,AUFNR,ICHARG,BWART,Direction,Remark,PalletLotSize,PackageVolumn,PackageWeight)
  VALUES(@Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource,@IsChangeUC,@AUFNR,@ICHARG,@BWART,@Direction,@Remark,@PalletLotSize,@PackageVolumn,@PackageWeight)
 END
 ELSE
 BEGIN
  INSERT INTO ORD_OrderDet_0(Id,Version,OrderNo,OrderType,OrderSubType,Seq,ExtNo,ExtSeq,StartDate,EndDate,ScheduleType,Item,ItemDesc,RefItemCode,Uom,BaseUom,UC,UCDesc,MinUC,QualityType,ManufactureParty,ReqQty,OrderQty,ShipQty,RecQty,RejQty,ScrapQty,PickQty,UnitQty,RecLotSize,LocFrom,LocFromNm,LocTo,LocToNm,IsInspect,BillAddr,BillAddrDesc,PriceList,UnitPrice,IsProvEst,Tax,IsIncludeTax,Currency,Bom,Routing,BillTerm,IsScanHu,ReserveNo,ReserveLine,ZOPWZ,ZOPID,ZOPDS,BinTo,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate,Container,ContainerDesc,PickStrategy,ExtraDmdSource,IsChangeUC,AUFNR,ICHARG,BWART,Direction,Remark,PalletLotSize,PackageVolumn,PackageWeight)
  VALUES(@Id,@Version,@OrderNo,@OrderType,@OrderSubType,@Seq,@ExtNo,@ExtSeq,@StartDate,@EndDate,@ScheduleType,@Item,@ItemDesc,@RefItemCode,@Uom,@BaseUom,@UC,@UCDesc,@MinUC,@QualityType,@ManufactureParty,@ReqQty,@OrderQty,@ShipQty,@RecQty,@RejQty,@ScrapQty,@PickQty,@UnitQty,@RecLotSize,@LocFrom,@LocFromNm,@LocTo,@LocToNm,@IsInspect,@BillAddr,@BillAddrDesc,@PriceList,@UnitPrice,@IsProvEst,@Tax,@IsIncludeTax,@Currency,@Bom,@Routing,@BillTerm,@IsScanHu,@ReserveNo,@ReserveLine,@ZOPWZ,@ZOPID,@ZOPDS,@BinTo,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate,@Container,@ContainerDesc,@PickStrategy,@ExtraDmdSource,@IsChangeUC,@AUFNR,@ICHARG,@BWART,@Direction,@Remark,@PalletLotSize,@PackageVolumn,@PackageWeight)
 END     
 SELECT @Id 
END
GO