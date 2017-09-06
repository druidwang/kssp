/****** Object:  StoredProcedure [dbo].[USP_Split_LocTrans_INSERT]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_LocTrans_INSERT')
	DROP PROCEDURE USP_Split_LocTrans_INSERT
CREATE PROCEDURE [dbo].[USP_Split_LocTrans_INSERT]
(
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
	@TraceCode varchar(50),
	@Item varchar(50),
	@Uom varchar(5),
	@BaseUom varchar(5),
	@Qty decimal(18,8),
	@IsCS bit,
	@PlanBill int,
	@PlanBillQty decimal(18,8),
	@ActBill int,
	@ActBillQty decimal(18,8),
	@UnitQty decimal(18,8),
	@QualityType tinyint,
	@HuId varchar(50),
	@LotNo varchar(50),
	@TransType int,
	@IOType tinyint,
	@PartyFrom varchar(50),
	@PartyTo varchar(50),
	@LocFrom varchar(50),
	@LocTo varchar(50),
	@LocIOReason varchar(50),
	@EffDate datetime,
	@CreateUser int,
	@CreateDate datetime
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Id bigint
	BEGIN TRAN
		IF EXISTS (SELECT * FROM SYS_TabIdSeq WITH (UPDLOCK,SERIALIZABLE) WHERE TabNm='INV_LocTrans')
		BEGIN
			SELECT @Id=Id+1 FROM SYS_TabIdSeq WHERE TabNm='INV_LocTrans'
			UPDATE SYS_TabIdSeq SET Id=Id+1 WHERE TabNm='INV_LocTrans'
		END
		ELSE
		BEGIN
			INSERT INTO SYS_TabIdSeq(TabNm,Id)
			VALUES('INV_LocTrans',1)
			SET @Id=1
		END
	COMMIT TRAN

	DECLARE @Statement nvarchar(4000)
	DECLARE @Parameter nvarchar(4000)
	DECLARE @PartSuffix varchar(50)
	IF @IOType=0 OR (@TransType in (301, 305, 304, 308, 311, 315, 314, 318))
	BEGIN
		SELECT @PartSuffix = PartSuffix FROM MD_Location WHERE Code = @LocTo
	END
	ELSE
	BEGIN
		SELECT @PartSuffix = PartSuffix FROM MD_Location WHERE Code = @LocFrom
	END
	
	IF ISNULL(@PartSuffix,'')=''
	BEGIN
		SET @PartSuffix='0'
	END 	
	
	SET @Statement=N'INSERT INTO INV_LocTrans_'+@PartSuffix+'(Id,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,OrderBomDetId,OrderBomDetSeq,IpNo,IpDetId,IpDetSeq,RecNo,RecDetId,RecDetSeq,SeqNo,TraceCode,Item,Uom,BaseUom,Qty,IsCS,PlanBill,PlanBillQty,ActBill,ActBillQty,UnitQty,QualityType,HuId,LotNo,TransType,IOType,PartyFrom,PartyTo,LocFrom,LocTo,LocIOReason,EffDate,CreateUser,CreateDate) 
	VALUES (@Id_1,@OrderNo_1,@OrderType_1,@OrderSubType_1,@OrderDetSeq_1,@OrderDetId_1,@OrderBomDetId_1,@OrderBomDetSeq_1,@IpNo_1,@IpDetId_1,@IpDetSeq_1,@RecNo_1,@RecDetId_1,@RecDetSeq_1,@SeqNo_1,@TraceCode_1,@Item_1,@Uom_1,@BaseUom_1,@Qty_1,@IsCS_1,@PlanBill_1,@PlanBillQty_1,@ActBill_1,@ActBillQty_1,@UnitQty_1,@QualityType_1,@HuId_1,@LotNo_1,@TransType_1,@IOType_1,@PartyFrom_1,@PartyTo_1,@LocFrom_1,@LocTo_1,@LocIOReason_1,@EffDate_1,@CreateUser_1,@CreateDate_1)'
	SET @Parameter=N'@Id_1 int,@OrderNo_1 varchar(50),@OrderType_1 tinyint,@OrderSubType_1 tinyint,@OrderDetSeq_1 int,@OrderDetId_1 int,@OrderBomDetId_1 int,@OrderBomDetSeq_1 int,@IpNo_1 varchar(50),@IpDetId_1 int,@IpDetSeq_1 int,@RecNo_1 varchar(50),@RecDetId_1 int,@RecDetSeq_1 int,@SeqNo_1 varchar(50),@TraceCode_1 varchar(50),@Item_1 varchar(50),@Uom_1 varchar(5),@BaseUom_1 varchar(5),@Qty_1 decimal(18,8),@IsCS_1 bit,@PlanBill_1 int,@PlanBillQty_1 decimal(18,8),@ActBill_1 int,@ActBillQty_1 decimal(18,8),@UnitQty_1 decimal(18,8),@QualityType_1 tinyint,@HuId_1 varchar(50),@LotNo_1 varchar(50),@TransType_1 int,@IOType_1 tinyint,@PartyFrom_1 varchar(50),@PartyTo_1 varchar(50),@LocFrom_1 varchar(50),@LocTo_1 varchar(50),@LocIOReason_1 varchar(50),@EffDate_1 datetime,@CreateUser_1 int,@CreateDate_1 datetime'
	
	exec sp_executesql @Statement,@Parameter,
		@Id_1=@Id,@OrderNo_1=@OrderNo,@OrderType_1=@OrderType,@OrderSubType_1=@OrderSubType,@OrderDetSeq_1=@OrderDetSeq,
		@OrderDetId_1=@OrderDetId,@OrderBomDetId_1=@OrderBomDetId,@OrderBomDetSeq_1=@OrderBomDetSeq,@IpNo_1=@IpNo,
		@IpDetId_1=@IpDetId,@IpDetSeq_1=@IpDetSeq,@RecNo_1=@RecNo,@RecDetId_1=@RecDetId,@RecDetSeq_1=@RecDetSeq,@SeqNo_1=@SeqNo,
		@TraceCode_1=@TraceCode,@Item_1=@Item,@Uom_1=@Uom,@BaseUom_1=@BaseUom,@Qty_1=@Qty,@IsCS_1=@IsCS,@PlanBill_1=@PlanBill,
		@PlanBillQty_1=@PlanBillQty,@ActBill_1=@ActBill,@ActBillQty_1=@ActBillQty,@UnitQty_1=@UnitQty,@QualityType_1=@QualityType,
		@HuId_1=@HuId,@LotNo_1=@LotNo,@TransType_1=@TransType,@IOType_1=@IOType,@PartyFrom_1=@PartyFrom,@PartyTo_1=@PartyTo,
		@LocFrom_1=@LocFrom,@LocTo_1=@LocTo,@LocIOReason_1=@LocIOReason,@EffDate_1=@EffDate,@CreateUser_1=@CreateUser,@CreateDate_1=@CreateDate
	--IF @TransType=301
	--BEGIN
	--	INSERT INTO INV_LocTrans_1(Id,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,OrderBomDetId,OrderBomDetSeq,IpNo,IpDetId,IpDetSeq,RecNo,RecDetId,RecDetSeq,SeqNo,TraceCode,Item,Uom,BaseUom,Qty,IsCS,PlanBill,PlanBillQty,ActBill,ActBillQty,UnitQty,QualityType,HuId,LotNo,TransType,IOType,PartyFrom,PartyTo,LocFrom,LocTo,LocIOReason,EffDate,CreateUser,CreateDate)
	--	VALUES(@Id,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@OrderBomDetId,@OrderBomDetSeq,@IpNo,@IpDetId,@IpDetSeq,@RecNo,@RecDetId,@RecDetSeq,@SeqNo,@TraceCode,@Item,@Uom,@BaseUom,@Qty,@IsCS,@PlanBill,@PlanBillQty,@ActBill,@ActBillQty,@UnitQty,@QualityType,@HuId,@LotNo,@TransType,@IOType,@PartyFrom,@PartyTo,@LocFrom,@LocTo,@LocIOReason,@EffDate,@CreateUser,@CreateDate)
	--END
	--ELSE
	--BEGIN
	--	INSERT INTO INV_LocTrans_0(Id,OrderNo,OrderType,OrderSubType,OrderDetSeq,OrderDetId,OrderBomDetId,OrderBomDetSeq,IpNo,IpDetId,IpDetSeq,RecNo,RecDetId,RecDetSeq,SeqNo,TraceCode,Item,Uom,BaseUom,Qty,IsCS,PlanBill,PlanBillQty,ActBill,ActBillQty,UnitQty,QualityType,HuId,LotNo,TransType,IOType,PartyFrom,PartyTo,LocFrom,LocTo,LocIOReason,EffDate,CreateUser,CreateDate)
	--	VALUES(@Id,@OrderNo,@OrderType,@OrderSubType,@OrderDetSeq,@OrderDetId,@OrderBomDetId,@OrderBomDetSeq,@IpNo,@IpDetId,@IpDetSeq,@RecNo,@RecDetId,@RecDetSeq,@SeqNo,@TraceCode,@Item,@Uom,@BaseUom,@Qty,@IsCS,@PlanBill,@PlanBillQty,@ActBill,@ActBillQty,@UnitQty,@QualityType,@HuId,@LotNo,@TransType,@IOType,@PartyFrom,@PartyTo,@LocFrom,@LocTo,@LocIOReason,@EffDate,@CreateUser,@CreateDate)
	--END
	SELECT @Id
END
GO