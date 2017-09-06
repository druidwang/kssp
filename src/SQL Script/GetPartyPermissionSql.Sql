USE [Sconit5_ShenYa]
GO

/****** Object:  UserDefinedFunction [dbo].[GetPartyPermissionSql]    Script Date: 11/01/2012 21:51:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		liqiuyun
-- Create date: 2012-11-01
-- Description:	获得用户订单/送货单/收货单权限Sql语句
-- =============================================
CREATE FUNCTION [dbo].[GetPartyPermissionSql]
(
	-- Add the parameters for the function here
	@Type int,
	@IsSupplier int,
	@IsReturn int
)
RETURNS varchar(4000)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @PermissionSql as varchar(4000)
	
	--Procurement = 1 
	--SubContract = 5,
	--CustomerGoods = 6,
	--ScheduleLine = 8
	IF(@Type=1 OR @Type=5 OR @Type=6 OR @Type=8)
	BEGIN
		IF(@IsReturn=0)
		BEGIN
			IF(@IsSupplier=0)
			BEGIN
			SET @PermissionSql=' AND ((o.IsCheckPartyToAuth=1 AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyFrom) AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyTo)) '
			SET @PermissionSql=@PermissionSql + ' OR (o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyFrom))) '
			END
			ELSE
			BEGIN
				SET @PermissionSql=' AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyFrom) '
			END
		END
		ELSE
		BEGIN
			IF(@IsSupplier=0)
			BEGIN
				SET @PermissionSql=' AND ((o.IsCheckPartyToAuth=1 AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyFrom) AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyTo)) '
				SET @PermissionSql=@PermissionSql + ' OR (o.PartyTo IN (SELECT PermissionCode FROM #TempPartyFrom))) '
			END
			ELSE
			BEGIN
				SET @PermissionSql=' AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyFrom) '
			END
		END			
	END
	--Transfer = 2,
	--Production = 4,
	--SubContractTransfer = 7,
	ELSE IF(@Type=2 OR @Type=4 OR @Type=7)
	BEGIN
		IF(@IsReturn=0)
		BEGIN
			SET @PermissionSql=' AND ((o.IsCheckPartyFromAuth=0 AND o.IsCheckPartyToAuth=0 AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyFrom) OR o.PartyTo IN (SELECT PermissionCode FROM #TempPartyTo)) '
			SET @PermissionSql=@PermissionSql + ' OR (o.IsCheckPartyFromAuth=0 AND o.IsCheckPartyToAuth=1 AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyTo)) '
			SET @PermissionSql=@PermissionSql + ' OR (o.IsCheckPartyFromAuth=1 AND o.IsCheckPartyToAuth=0 AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyFrom)) '
			SET @PermissionSql=@PermissionSql + ' OR (o.IsCheckPartyFromAuth=1 AND o.IsCheckPartyToAuth=1 AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyFrom) AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyTo))) '
		END
		ELSE 
		BEGIN
			
			SET @PermissionSql=' AND ((o.IsCheckPartyFromAuth=0 AND o.IsCheckPartyToAuth=0 AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyFrom) OR o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyTo)) '
			SET @PermissionSql=@PermissionSql + ' OR (o.IsCheckPartyFromAuth=0 AND o.IsCheckPartyToAuth=1 AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyTo)) '
			SET @PermissionSql=@PermissionSql + ' OR (o.IsCheckPartyFromAuth=1 AND o.IsCheckPartyToAuth=0 AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyFrom)) '
			SET @PermissionSql=@PermissionSql + ' OR (o.IsCheckPartyFromAuth=1 AND o.IsCheckPartyToAuth=1 AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyFrom) AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyTo))) '
		END	
	END
	--Distribution = 3,
	ELSE IF(@Type=3)
	BEGIN
		IF(@IsReturn=0)
		BEGIN
			SET @PermissionSql=' AND ((o.IsCheckPartyToAuth=1 AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyFrom) AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyTo)) '
			SET @PermissionSql=@PermissionSql+'OR (o.PartyTo IN (SELECT PermissionCode FROM #TempPartyTo))) '
		END
		ELSE 
		BEGIN
			SET @PermissionSql=' AND ((o.IsCheckPartyToAuth=1 AND o.PartyTo IN (SELECT PermissionCode FROM #TempPartyFrom) AND o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyTo)) '
			SET @PermissionSql=@PermissionSql+'OR (o.PartyFrom IN (SELECT PermissionCode FROM #TempPartyTo))) '
		END	
	END
	
	-- Return the result of the function
	RETURN @PermissionSql

END

GO

