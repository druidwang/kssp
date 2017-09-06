/****** Object:  StoredProcedure [dbo].[USP_Split_IpLocationDet_INSERT]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_IpLocationDet_INSERT')
	DROP PROCEDURE USP_Split_IpLocationDet_INSERT
CREATE PROCEDURE [dbo].[USP_Split_IpLocationDet_INSERT]
(
	@Version int,
	@IpNo varchar(50),
	@IpDetId int,
	@OrderType tinyint,
	@OrderDetId int,
	@Item varchar(50),
	@HuId varchar(50),
	@LotNo varchar(50),
	@IsCreatePlanBill bit,
	@IsCS bit,
	@PlanBill int,
	@ActBill int,
	@QualityType tinyint,
	@IsFreeze bit,
	@IsATP bit,
	@OccupyType tinyint,
	@OccupyRefNo varchar(50),
	@Qty decimal(18,8),
	@RecQty decimal(18,8),
	@IsClose bit,
	@WMSSeq varchar(50),
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
		IF EXISTS (SELECT * FROM SYS_TabIdSeq WITH (UPDLOCK,SERIALIZABLE) WHERE TabNm='ORD_IpLocationDet')
		BEGIN
			SELECT @Id=Id+1 FROM SYS_TabIdSeq WHERE TabNm='ORD_IpLocationDet'
			UPDATE SYS_TabIdSeq SET Id=Id+1 WHERE TabNm='ORD_IpLocationDet'
		END
		ELSE
		BEGIN
			INSERT INTO SYS_TabIdSeq(TabNm,Id)
			VALUES('ORD_IpLocationDet',1)
			SET @Id=1
		END
	COMMIT TRAN
	
	IF @OrderType=1
	BEGIN
		INSERT INTO ORD_IpLocationDet_1(Id,Version,IpNo,IpDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,RecQty,IsClose,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@VERSION,@IpNo,@IpDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@RecQty,@IsClose,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=2
	BEGIN
		INSERT INTO ORD_IpLocationDet_2(Id,Version,IpNo,IpDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,RecQty,IsClose,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@VERSION,@IpNo,@IpDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@RecQty,@IsClose,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=3
	BEGIN
		INSERT INTO ORD_IpLocationDet_3(Id,Version,IpNo,IpDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,RecQty,IsClose,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@VERSION,@IpNo,@IpDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@RecQty,@IsClose,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=4
	BEGIN
		INSERT INTO ORD_IpLocationDet_4(Id,Version,IpNo,IpDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,RecQty,IsClose,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@VERSION,@IpNo,@IpDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@RecQty,@IsClose,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=5
	BEGIN
		INSERT INTO ORD_IpLocationDet_5(Id,Version,IpNo,IpDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,RecQty,IsClose,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@VERSION,@IpNo,@IpDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@RecQty,@IsClose,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=6
	BEGIN
		INSERT INTO ORD_IpLocationDet_6(Id,Version,IpNo,IpDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,RecQty,IsClose,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@VERSION,@IpNo,@IpDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@RecQty,@IsClose,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=7
	BEGIN
		INSERT INTO ORD_IpLocationDet_7(Id,Version,IpNo,IpDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,RecQty,IsClose,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@VERSION,@IpNo,@IpDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@RecQty,@IsClose,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE IF @OrderType=8
	BEGIN
		INSERT INTO ORD_IpLocationDet_8(Id,Version,IpNo,IpDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,RecQty,IsClose,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@VERSION,@IpNo,@IpDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@RecQty,@IsClose,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	ELSE 
	BEGIN
		INSERT INTO ORD_IpLocationDet_0(Id,Version,IpNo,IpDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,RecQty,IsClose,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@VERSION,@IpNo,@IpDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@RecQty,@IsClose,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END
	SELECT @Id
END
GO