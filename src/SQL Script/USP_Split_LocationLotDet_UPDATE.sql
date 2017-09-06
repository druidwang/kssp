/****** Object:  StoredProcedure [dbo].[USP_Split_LocationLotDet_UPDATE]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_LocationLotDet_UPDATE')
	DROP PROCEDURE USP_Split_LocationLotDet_UPDATE
CREATE PROCEDURE [dbo].[USP_Split_LocationLotDet_UPDATE]
(
	@Version int,
	@Location varchar(50),
	@Bin varchar(50),
	@Item varchar(50),
	@LotNo varchar(50),
	@HuId varchar(50),
	@Qty decimal(18,8),
	@IsCS bit,
	@PlanBill int,
	@QualityType tinyint,
	@IsFreeze bit,
	@IsATP bit,
	@OccupyType tinyint,
	@OccupyRefNo varchar(50),
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime,
	@Id int,
	@VersionBerfore int
)
AS
BEGIN
	--SET NOCOUNT ON;
	DECLARE @Statement nvarchar(4000)
	DECLARE @Parameter nvarchar(4000)
	DECLARE @PartSuffix varchar(50)
	SELECT @PartSuffix = PartSuffix FROM MD_Location WHERE Code = @Location
	
	IF ISNULL(@PartSuffix,'')=''
	BEGIN
		SET @PartSuffix='0'
	END 
	
	SET @Statement='UPDATE INV_LocationLotDet_'+@PartSuffix+' SET Version=@Version_1,Location=@Location_1,Bin=@Bin_1,Item=@Item_1,LotNo=@LotNo_1,HuId=@HuId_1,Qty=@Qty_1,IsCS=@IsCS_1,PlanBill=@PlanBill_1,QualityType=@QualityType_1,IsFreeze=@IsFreeze_1,IsATP=@IsATP_1,OccupyType=@OccupyType_1,OccupyRefNo=@OccupyRefNo_1,LastModifyUser=@LastModifyUser_1,LastModifyUserNm=@LastModifyUserNm_1,LastModifyDate=@LastModifyDate_1 WHERE Id=@Id_1 AND Version=@VersionBerfore_1'
	SET @Parameter=N'@Version_1 int,@Location_1 varchar(50),@Bin_1 varchar(50),@Item_1 varchar(50),@LotNo_1 varchar(50),@HuId_1 varchar(50),@Qty_1 decimal(18,8),@IsCS_1 bit,@PlanBill_1 int,@QualityType_1 tinyint,@IsFreeze_1 bit,@IsATP_1 bit,@OccupyType_1 tinyint,@OccupyRefNo_1 varchar(50),@LastModifyUser_1 int,@LastModifyUserNm_1 varchar(100),@LastModifyDate_1 datetime,@Id_1 int,@VersionBerfore_1 int'
	PRINT @Statement

	exec sp_executesql @Statement,@Parameter,
		@Version_1=@Version,@Location_1=@Location,@Bin_1=@Bin,@Item_1=@Item,@LotNo_1=@LotNo,@HuId_1=@HuId,@Qty_1=@Qty,
		@IsCS_1=@IsCS,@PlanBill_1=@PlanBill,@QualityType_1=@QualityType,@IsFreeze_1=@IsFreeze,@IsATP_1=@IsATP,@OccupyType_1=@OccupyType,
		@OccupyRefNo_1=@OccupyRefNo,@LastModifyUser_1=@LastModifyUser,@LastModifyUserNm_1=@LastModifyUserNm,
		@LastModifyDate_1=@LastModifyDate,@Id_1=@Id,@VersionBerfore_1=@VersionBerfore
	

END
GO