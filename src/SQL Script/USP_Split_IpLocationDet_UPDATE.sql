/****** Object:  StoredProcedure [dbo].[USP_Split_IpLocationDet_UPDATE]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_IpLocationDet_UPDATE')
	DROP PROCEDURE USP_Split_IpLocationDet_UPDATE
CREATE PROCEDURE [dbo].[USP_Split_IpLocationDet_UPDATE]
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
		UPDATE ORD_IpLocationDet_1 SET Version=@Version,IpNo=@IpNo,IpDetId=@IpDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,RecQty=@RecQty,IsClose=@IsClose,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=2
	BEGIN
		UPDATE ORD_IpLocationDet_2 SET Version=@Version,IpNo=@IpNo,IpDetId=@IpDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,RecQty=@RecQty,IsClose=@IsClose,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=3
	BEGIN
		UPDATE ORD_IpLocationDet_3 SET Version=@Version,IpNo=@IpNo,IpDetId=@IpDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,RecQty=@RecQty,IsClose=@IsClose,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=4
	BEGIN
		UPDATE ORD_IpLocationDet_4 SET Version=@Version,IpNo=@IpNo,IpDetId=@IpDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,RecQty=@RecQty,IsClose=@IsClose,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=5
	BEGIN
		UPDATE ORD_IpLocationDet_5 SET Version=@Version,IpNo=@IpNo,IpDetId=@IpDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,RecQty=@RecQty,IsClose=@IsClose,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=6
	BEGIN
		UPDATE ORD_IpLocationDet_6 SET Version=@Version,IpNo=@IpNo,IpDetId=@IpDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,RecQty=@RecQty,IsClose=@IsClose,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=7
	BEGIN
		UPDATE ORD_IpLocationDet_7 SET Version=@Version,IpNo=@IpNo,IpDetId=@IpDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,RecQty=@RecQty,IsClose=@IsClose,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
	ELSE IF @OrderType=8
	BEGIN
		UPDATE ORD_IpLocationDet_8 SET Version=@Version,IpNo=@IpNo,IpDetId=@IpDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,RecQty=@RecQty,IsClose=@IsClose,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
	ELSE 
	BEGIN
		UPDATE ORD_IpLocationDet_0 SET Version=@Version,IpNo=@IpNo,IpDetId=@IpDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,RecQty=@RecQty,IsClose=@IsClose,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id AND Version=@VersionBerfore
	END
END
GO
