
/****** Object:  StoredProcedure [dbo].[USP_Split_RecLocationDet_UPDATE]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_RecLocationDet_UPDATE')
	DROP PROCEDURE USP_Split_RecLocationDet_UPDATE
CREATE PROCEDURE [dbo].[USP_Split_RecLocationDet_UPDATE]
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
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime,
	@Id int
)
AS
BEGIN
	IF @OrderType=1
	BEGIN
		UPDATE ORD_RecLocationDet_1 SET RecNo=@RecNo,RecDetId=@RecDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id
	END
	ELSE IF @OrderType=2
	BEGIN
		UPDATE ORD_RecLocationDet_2 SET RecNo=@RecNo,RecDetId=@RecDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id
	END
	ELSE IF @OrderType=3
	BEGIN
		UPDATE ORD_RecLocationDet_3 SET RecNo=@RecNo,RecDetId=@RecDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id
	END
	ELSE IF @OrderType=4
	BEGIN
		UPDATE ORD_RecLocationDet_4 SET RecNo=@RecNo,RecDetId=@RecDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id
	END
	ELSE IF @OrderType=5
	BEGIN
		UPDATE ORD_RecLocationDet_5 SET RecNo=@RecNo,RecDetId=@RecDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id
	END
	ELSE IF @OrderType=6
	BEGIN
		UPDATE ORD_RecLocationDet_6 SET RecNo=@RecNo,RecDetId=@RecDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id
	END
	ELSE IF @OrderType=7
	BEGIN
		UPDATE ORD_RecLocationDet_7 SET RecNo=@RecNo,RecDetId=@RecDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id
	END	
	ELSE IF @OrderType=8
	BEGIN
		UPDATE ORD_RecLocationDet_8 SET RecNo=@RecNo,RecDetId=@RecDetId,OrderType=@OrderType,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id
	END	
	ELSE
	BEGIN
		UPDATE ORD_RecLocationDet_0 SET RecNo=@RecNo,RecDetId=@RecDetId,OrderType=0,OrderDetId=@OrderDetId,Item=@Item,HuId=@HuId,LotNo=@LotNo,IsCreatePlanBill=@IsCreatePlanBill,IsCS=@IsCS,PlanBill=@PlanBill,ActBill=@ActBill,QualityType=@QualityType,IsFreeze=@IsFreeze,IsATP=@IsATP,OccupyType=@OccupyType,OccupyRefNo=@OccupyRefNo,Qty=@Qty,WMSSeq=@WMSSeq,LastModifyUser=@LastModifyUser,LastModifyUserNm=@LastModifyUserNm,LastModifyDate=@LastModifyDate
		WHERE Id=@Id
	END							
END
GO