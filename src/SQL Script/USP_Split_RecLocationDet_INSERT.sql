/****** Object:  StoredProcedure [dbo].[USP_Split_RecLocationDet_INSERT]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_RecLocationDet_INSERT')
	DROP PROCEDURE USP_Split_RecLocationDet_INSERT
CREATE PROCEDURE [dbo].[USP_Split_RecLocationDet_INSERT]
(
	@RecNo varchar(50),
	@RecDetId int,
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
		IF EXISTS (SELECT * FROM SYS_TabIdSeq WITH (UPDLOCK,SERIALIZABLE) WHERE TabNm='ORD_RecLocationDet')
		BEGIN
			SELECT @Id=Id+1 FROM SYS_TabIdSeq WHERE TabNm='ORD_RecLocationDet'
			UPDATE SYS_TabIdSeq SET Id=Id+1 WHERE TabNm='ORD_RecLocationDet'
		END
		ELSE
		BEGIN
			INSERT INTO SYS_TabIdSeq(TabNm,Id)
			VALUES('ORD_RecLocationDet',1)
			SET @Id=1
		END
	COMMIT TRAN
	
	IF @OrderType=1
	BEGIN
		INSERT INTO ORD_RecLocationDet_1(Id,RecNo,RecDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@RecNo,@RecDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END	
	ELSE IF @OrderType=2
	BEGIN
		INSERT INTO ORD_RecLocationDet_2(Id,RecNo,RecDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@RecNo,@RecDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END		
	ELSE IF @OrderType=3
	BEGIN
		INSERT INTO ORD_RecLocationDet_3(Id,RecNo,RecDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@RecNo,@RecDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END	
	ELSE IF @OrderType=4
	BEGIN
		INSERT INTO ORD_RecLocationDet_4(Id,RecNo,RecDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@RecNo,@RecDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END	
	ELSE IF @OrderType=5
	BEGIN
		INSERT INTO ORD_RecLocationDet_5(Id,RecNo,RecDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@RecNo,@RecDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END	
	ELSE IF @OrderType=6
	BEGIN
		INSERT INTO ORD_RecLocationDet_6(Id,RecNo,RecDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@RecNo,@RecDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END	
	ELSE IF @OrderType=7
	BEGIN
		INSERT INTO ORD_RecLocationDet_7(Id,RecNo,RecDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@RecNo,@RecDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END	
	ELSE IF @OrderType=8
	BEGIN
		INSERT INTO ORD_RecLocationDet_8(Id,RecNo,RecDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@RecNo,@RecDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END	
	ELSE
	BEGIN
		INSERT INTO ORD_RecLocationDet_0(Id,RecNo,RecDetId,OrderType,OrderDetId,Item,HuId,LotNo,IsCreatePlanBill,IsCS,PlanBill,ActBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,Qty,WMSSeq,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
		VALUES(@Id,@RecNo,@RecDetId,@OrderType,@OrderDetId,@Item,@HuId,@LotNo,@IsCreatePlanBill,@IsCS,@PlanBill,@ActBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@Qty,@WMSSeq,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	END	
	SELECT @Id
END
GO

