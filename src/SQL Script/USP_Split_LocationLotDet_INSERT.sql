/****** Object:  StoredProcedure [dbo].[USP_Split_LocationLotDet_INSERT]    Script Date: 07/05/2012 14:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Split_LocationLotDet_INSERT')
	DROP PROCEDURE USP_Split_LocationLotDet_INSERT
CREATE PROCEDURE [dbo].[USP_Split_LocationLotDet_INSERT]
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
	@CreateUser int,
	@CreateUserNm varchar(100),
	@CreateDate datetime,
	@LastModifyUser int,
	@LastModifyUserNm varchar(100),
	@LastModifyDate datetime
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Id bigint
	BEGIN TRAN
		IF EXISTS (SELECT * FROM SYS_TabIdSeq WITH (UPDLOCK,SERIALIZABLE) WHERE TabNm='INV_LocationLotDet')
		BEGIN
			SELECT @Id=Id+1 FROM SYS_TabIdSeq WHERE TabNm='INV_LocationLotDet'
			UPDATE SYS_TabIdSeq SET Id=Id+1 WHERE TabNm='INV_LocationLotDet'
		END
		ELSE
		BEGIN
			INSERT INTO SYS_TabIdSeq(TabNm,Id)
			VALUES('INV_LocationLotDet',1)
			SET @Id=1
		END
	COMMIT TRAN
	
	DECLARE @Statement nvarchar(4000)
	DECLARE @Parameter nvarchar(4000)
	DECLARE @PartSuffix varchar(50)
	SELECT @PartSuffix = PartSuffix FROM MD_Location WHERE Code = @Location
	
	IF ISNULL(@PartSuffix,'')=''
	BEGIN
		SET @PartSuffix='0'
		--RAISERROR ('Can''t get any data from table' , 16, 1) WITH NOWAIT
	END 
	
	SET @Statement=N'INSERT INTO INV_LocationLotDet_'+@PartSuffix+'(Id,Version,Location,Bin,Item,LotNo,HuId,Qty,IsCS,PlanBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate) VALUES (@Id_1,@Version_1,@Location_1,@Bin_1,@Item_1,@LotNo_1,@HuId_1,@Qty_1,@IsCS_1,@PlanBill_1,@QualityType_1,@IsFreeze_1,@IsATP_1,@OccupyType_1,@OccupyRefNo_1,@CreateUser_1,@CreateUserNm_1,@CreateDate_1,@LastModifyUser_1,@LastModifyUserNm_1,@LastModifyDate_1)'
	SET @Parameter=N'@Id_1 int,@Version_1 int,@Location_1 varchar(50),@Bin_1 varchar(50),@Item_1 varchar(50),@LotNo_1 varchar(50),@HuId_1 varchar(50),@Qty_1 decimal(18,8),@IsCS_1 bit,@PlanBill_1 int,@QualityType_1 tinyint,@IsFreeze_1 bit,@IsATP_1 bit,@OccupyType_1 tinyint,@OccupyRefNo_1 varchar(50),@CreateUser_1 int,@CreateUserNm_1 varchar(100),@CreateDate_1 datetime,@LastModifyUser_1 int,@LastModifyUserNm_1 varchar(100),@LastModifyDate_1 datetime'
	
	exec sp_executesql @Statement,@Parameter,
		@Id_1=@Id,@Version_1=@Version,@Location_1=@Location,@Bin_1=@Bin,@Item_1=@Item,@LotNo_1=@LotNo,@HuId_1=@HuId,
		@Qty_1=@Qty,@IsCS_1=@IsCS,@PlanBill_1=@PlanBill,@QualityType_1=@QualityType,@IsFreeze_1=@IsFreeze,@IsATP_1=@IsATP,
		@OccupyType_1=@OccupyType,@OccupyRefNo_1=@OccupyRefNo,@CreateUser_1=@CreateUser,@CreateUserNm_1=@CreateUserNm,
		@CreateDate_1=@CreateDate,@LastModifyUser_1=@LastModifyUser,@LastModifyUserNm_1=@LastModifyUserNm,@LastModifyDate_1=@LastModifyDate
		--N'@Id int,@Version int,@Location varchar(50),@Bin varchar(50),@Item varchar(50),@LotNo varchar(50),@HuId varchar(50),@Qty decimal(18,8),@IsCS bit,@PlanBill int,@QualityType tinyint,@IsFreeze bit,@IsATP bit,@OccupyType tinyint,@OccupyRefNo varchar(50),@CreateUser int,@CreateUserNm varchar(100),@CreateDate datetime,@LastModifyUser int,@LastModifyUserNm varchar(100),@LastModifyDate datetime'
		
	--IF @Location='RM'
	--BEGIN
	--	INSERT INTO INV_LocationLotDet_1(Id,Version,Location,Bin,Item,LotNo,HuId,Qty,IsCS,PlanBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
	--	VALUES(@Id,@Version,@Location,@Bin,@Item,@LotNo,@HuId,@Qty,@IsCS,@PlanBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	--END
	--ELSE
	--BEGIN
	--	INSERT INTO INV_LocationLotDet_0(Id,Version,Location,Bin,Item,LotNo,HuId,Qty,IsCS,PlanBill,QualityType,IsFreeze,IsATP,OccupyType,OccupyRefNo,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate)
	--	VALUES(@Id,@Version,@Location,@Bin,@Item,@LotNo,@HuId,@Qty,@IsCS,@PlanBill,@QualityType,@IsFreeze,@IsATP,@OccupyType,@OccupyRefNo,@CreateUser,@CreateUserNm,@CreateDate,@LastModifyUser,@LastModifyUserNm,@LastModifyDate)
	--END
	SELECT @Id
END
GO