/****** Object:  View [dbo].[VIEW_DailyInvBalance]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='V' AND name='VIEW_DailyInvBalance')
	DROP VIEW VIEW_DailyInvBalance
CREATE VIEW [dbo].[VIEW_DailyInvBalance]
AS
SELECT  *    
FROM         dbo.INV_DailyInvBalance_0
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_1
UNION ALL
SELECT *    
FROM         dbo.INV_DailyInvBalance_2
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_3
UNION ALL
SELECT  *    
FROM         dbo.INV_DailyInvBalance_4
UNION ALL
SELECT  *  
FROM         dbo.INV_DailyInvBalance_5
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_6
UNION ALL
SELECT  * 
FROM         dbo.INV_DailyInvBalance_7
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_8
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_9
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_10
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_11
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_12
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_13
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_14
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_15
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_16
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_17
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_18
UNION ALL
SELECT  *   
FROM         dbo.INV_DailyInvBalance_19
GO
