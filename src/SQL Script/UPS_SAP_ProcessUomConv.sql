ALTER PROCEDURE UPS_SAP_ProcessUomConv
(
	@UserId int,
	@BatchNo varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON 
	DECLARE @UserName varchar(50) 
	DECLARE @CurrentDate datetime
	SELECT @UserName=FirstName+LastName FROM ACC_User WHERE Id=@UserId 
	SET @CurrentDate=GETDATE()
	IF(ISNULL(@BatchNo,'')<>'') 
	BEGIN 
		-----≤Â»Îµ•Œª 
		BEGIN TRY 
			INSERT INTO MD_Uom(Code,Desc1,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate) 
			SELECT DISTINCT MEINS,MEINS,@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate 
				FROM Sap_UomConv i WHERE BatchNo=@BatchNo AND i.MEINS IS NOT NULL 
				AND NOT EXISTS(SELECT 1 FROM MD_Uom u WHERE i.MEINS=u.Code) 
			UNION
			SELECT DISTINCT MEINH,MEINH,@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate 
				FROM Sap_UomConv i WHERE BatchNo=@BatchNo AND i.MEINH IS NOT NULL 
				AND NOT EXISTS(SELECT 1 FROM MD_Uom u WHERE i.MEINH=u.Code) 	
			
			INSERT INTO MD_UomConv(Item, BaseUom, AltUom, BaseQty, AltQty, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate)
			SELECT MATNR,MEINS,MEINH,UMREZ,UMREN,@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate 
				FROM Sap_UomConv su LEFT JOIN MD_UomConv mu ON su.MATNR=mu.Item AND su.MEINS=mu.BaseUom AND su.MEINH=mu.AltUom
			WHERE mu.Id IS NULL AND su.Status=0
			
			UPDATE mu 
				SET mu.BaseQty=su.UMREZ,mu.AltQty=su.UMREN,
				mu.LastModifyUser=@UserId,
				mu.LastModifyUserNm=@UserName,
				mu.LastModifyDate=@CurrentDate
				FROM Sap_UomConv su INNER JOIN MD_UomConv mu ON su.MATNR=mu.Item AND su.MEINS=mu.BaseUom AND su.MEINH=mu.AltUom
				WHERE su.Status=0
			UPDATE Sap_UomConv SET Status=1 WHERE Status=0
			SELECT Status=1,BatchNo=@BatchNo,Message='SUCCESS UPDATE SAP UomConv TO MES' 
		END TRY 
		BEGIN CATCH 
			UPDATE Sap_Item SET Status=2 WHERE BatchNo=@BatchNo  
			SELECT Status=2,BatchNo=@BatchNo,Message=ERROR_MESSAGE() 
		END CATCH
	END 
END