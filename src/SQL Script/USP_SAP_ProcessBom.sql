CREATE PROCEDURE [dbo].[USP_SAP_ProcessBom]
(
	@UserId  int,
	@BatchNo varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @UserName varchar(50) 
	DECLARE @CurrentDate datetime
	SELECT @UserName=FirstName+LastName FROM ACC_User WHERE Id=@UserId 
	SET @CurrentDate=GETDATE()
	DECLARE @ErrorMsg varchar(4000)=''
	
	BEGIN TRY 
		BEGIN TRAN ProcessBom
		SAVE TRAN ProcessBom_Point
		
		SELECT * FROM SAP_Bom
		IF EXISTS(SELECT 1 FROM SAP_Bom WHERE BatchNo=@BatchNo AND BMEIN NOT IN(SELECT COde FROM MD_Uom) OR MEINS NOT IN(SELECT COde FROM MD_Uom))
		BEGIN
			RAISERROR ('SAP数据中有单位不存在MES数据库中!' , 16, 1) WITH NOWAIT 
		END
		
		
		IF EXISTS(SELECT 1 FROM SAP_Bom WHERE BatchNo=@BatchNo AND MATNR NOT IN(SELECT Code FROM MD_Item) OR IDNRK NOT IN(SELECT Code FROM MD_Item))
		BEGIN
			RAISERROR ('SAP数据中有物料号不存在MES数据库中!' , 16, 1) WITH NOWAIT 
		END
		DELETE pb FROM PRD_BomDet pb INNER JOIN SAP_Bom sb ON pb.Bom=sb.MATNR
		
		INSERT INTO PRD_BomMstr(Code, Desc1, Uom, IsActive, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, Qty) 
		SELECT DISTINCT sb.MATNR,sb.MAKTX,sb.BMEIN,1,@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate,CAST(BMENG AS decimal(18,8)) 
			FROM SAP_Bom sb LEFT JOIN PRD_BomMstr pb ON pb.Code=sb.MATNR
		WHERE sb.BatchNo=@BatchNo AND pb.Code IS NULL
		
		INSERT INTO PRD_BomDet(Bom, Item, Op, OpRef, Uom, StruType, StartDate, EndDate, RateQty, ScrapPct, BFStrategy, BFMethod, FeedMethod, IsAutoFeed, Location, IsPrint, BomMrpOption, IsKeyComponent, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate)
		SELECT MATNR,IDNRK,0,'',MEINS,0,@CurrentDate,NULL,CAST(MENGE AS decimal(18,8)),CAST(AUSCH AS decimal(18,8))/100.0,NULL,0,0,0,NULL,0,0,NULL,@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate 
			FROM SAP_Bom WHERE BatchNo=@BatchNo 
		
		UPDATE SAP_Bom SET Status=1 WHERE BatchNo=@BatchNo 
		SELECT Status=1,BatchNo=@BatchNo,Message='SUCCESS UPDATE SAP BOM TO MES' 
		COMMIT TRAN ProcessBom
	END TRY 
	BEGIN CATCH 
		ROLLBACK TRAN GenJITOrder_Point
		COMMIT TRAN GenJITOrder
		
		--Log: 计算JIT订单发生错误
		SET @ErrorMsg = Error_Message()
		SELECT Status=2,BatchNo=@BatchNo,Message=@ErrorMsg 
	END CATCH
END