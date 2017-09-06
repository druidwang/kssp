ALTER PROCEDURE UPS_SAP_ProcessPriceList
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
		BEGIN TRY 
			INSERT INTO BIL_PriceListMstr(Code, Type, Party, Currency, IsIncludeTax, Tax, IsActive, CreateUser, CreateUserNm, 
			CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, InterfacePriceType)
			SELECT DISTINCT LIFNR+WAERS,1,LIFNR,WAERS,0,NULL,1,@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate,
				CASE WHEN NORMB='L' THEN 1 ELSE 0 END---委外是L,其他是普通
			FROM Sap_PriceList sp LEFT JOIN BIL_PriceListMstr bp ON sp.LIFNR=bp.Party AND sp.WAERS=bp.Currency
			WHERE bp.Code IS NULL
			
			INSERT INTO BIL_PriceListDet(PriceList, Item, StartDate, Uom, EndDate, UnitPrice, IsProvEst, CreateUser, CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate)
			SELECT sp.LIFNR+sp.WAERS,sp.MATNR,CAST(sp.ERDAT AS datetime),sp.BPRME,CAST(sp.PRDAT AS datetime),sp.NETPR,0,
				@UserId,@UserName,@CurrentDate,@UserId,@UserName,@CurrentDate
			FROM Sap_PriceList sp INNER JOIN BIL_PriceListMstr bp ON sp.LIFNR=bp.Party AND sp.WAERS=bp.Currency
			LEFT JOIN BIL_PriceListDet pd ON pd.PriceList=bp.Code AND sp.MATNR=pd.Item AND sp.BPRME=pd.Uom AND CAST(sp.ERDAT AS datetime)=pd.StartDate
				AND CAST(sp.PRDAT AS datetime)=pd.EndDate
			WHERE pd.Id IS NULL	
				
			UPDATE pd SET UnitPrice=sp.NETPR FROM Sap_PriceList sp INNER JOIN BIL_PriceListMstr bp ON sp.LIFNR=bp.Party AND sp.WAERS=bp.Currency
			INNER JOIN BIL_PriceListDet pd ON pd.PriceList=bp.Code AND sp.MATNR=pd.Item AND sp.BPRME=pd.Uom AND CAST(sp.ERDAT AS datetime)=pd.StartDate
				AND CAST(sp.PRDAT AS datetime)=pd.EndDate
			
			UPDATE Sap_PriceList SET Status=1 WHERE BatchNo=@BatchNo  
			SELECT Status=1,BatchNo=@BatchNo,Message='SUCCESS UPDATE SAP PRICELIST TO MES' 
		END TRY 
		BEGIN CATCH 
			UPDATE Sap_Item SET Status=2 WHERE BatchNo=@BatchNo  
			SELECT Status=2,BatchNo=@BatchNo,Message=ERROR_MESSAGE() 
		END CATCH
	END 
END