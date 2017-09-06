ALTER PROCEDURE UPS_SAP_ProcessItem
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
		-----插入单位 
		BEGIN TRY 
			INSERT INTO MD_Uom(Code,Desc1,CreateUser,CreateUserNm,CreateDate,LastModifyUser,LastModifyUserNm,LastModifyDate) 
			SELECT DISTINCT MEINS,'',@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate 
				FROM Sap_Item i WHERE BatchNo=@BatchNo AND i.MEINS IS NOT NULL 
				AND NOT EXISTS(SELECT 1 FROM MD_Uom u WHERE i.MEINS=u.Code) 

			-----插入物料类型 b
			INSERT INTO MD_ItemCategory(Code, SubCategory, Desc1, IsActive, ParentCategory, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate) 
			SELECT DISTINCT MTART,0,MTBEZ,1,null,@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate 
				FROM Sap_Item i WHERE BatchNo=@BatchNo AND MTART IS NOT NULL 
				AND NOT EXISTS(SELECT 1 FROM MD_ItemCategory ic WHERE i.MTART=ic.Code) 
			-----插入物料组别
			INSERT INTO MD_ItemCategory(Code, SubCategory, Desc1, IsActive, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate) 
			SELECT DISTINCT MATKL,5,WGBEZ,1,@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate 
				FROM Sap_Item i WHERE BatchNo=@BatchNo AND MATKL IS NOT NULL 
				AND NOT EXISTS(SELECT 1 FROM MD_ItemCategory ic WHERE i.MATKL=ic.Code) 			
			-----更新物料主数据 
			UPDATE mi SET mi.Desc1=REPLACE(si.MAKTX,',',' '),
				mi.IsActive=CASE WHEN si.LVORM='X' THEN 0 ELSE 1 END,   
				mi.ItemCategory=si.MTART, 
				mi.RefCode=si.MAKTX,
				mi.MaterialsGroup=si.MATKL,
				mi.Division=si.SPART,
				mi.IsVirtual=CASE WHEN si.SOBSL='X' THEN 0 ELSE 1 END,
				mi.LastModifyUser=@UserId,
				mi.LastModifyUserNm=@UserName,
				mi.LastModifyDate=@CurrentDate
			FROM MD_Item mi,Sap_Item si 
			WHERE mi.Code=si.MATNR AND si.BatchNo=@BatchNo 
			-----插入物料主数据  
			INSERT INTO MD_Item(Code, 
				RefCode, 
				Uom, 
				Desc1, 
				UC, 
				ItemCategory, 
				MaterialsGroup,
				IsActive,  
				IsVirtual, 
				IsKit,
				IsInvFreeze,
				Warranty, 
				WarnLeadTime, 
				ItemPriority, 
				CreateUser, 
				CreateUserNm, 
				CreateDate, 
				LastModifyUser, 
				LastModifyUserNm, 
				LastModifyDate,
				Division) 
			SELECT DISTINCT si.MATNR,
				si.MAKTX,
				si.MEINS,
				REPLACE(si.MAKTX,',',' '),
				1,
				si.MTART,
				si.MATKL,
				CASE WHEN si.LVORM='X' THEN 0 ELSE 1 END,
				CASE WHEN si.SOBSL='X' THEN 0 ELSE 1 END,
				0,
				0,
				1,
				1,
				0,
				@UserId,
				@UserName,
				@CurrentDate,
				@UserId,
				@UserName,
				@CurrentDate,
				si.SPART
			FROM Sap_Item si 
			WHERE NOT EXISTS(SELECT 1 FROM MD_Item mi WHERE mi.Code=si.MATNR)  AND si.BatchNo=@BatchNo
						
			UPDATE Sap_Item SET Status=1 WHERE BatchNo=@BatchNo  
			SELECT Status=1,BatchNo=@BatchNo,Message='SUCCESS UPDATE SAP ITEM TO MES' 
		END TRY 
		BEGIN CATCH 
			UPDATE Sap_Item SET Status=2 WHERE BatchNo=@BatchNo  
			SELECT Status=2,BatchNo=@BatchNo,Message=ERROR_MESSAGE() 
		END CATCH
	END 
END