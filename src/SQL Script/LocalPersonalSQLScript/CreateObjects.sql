
 
go
Update MD_Location
Set SAPLocation ='3020' from MD_Location where Code ='9307'
Update MD_Location
Set SAPLocation ='4001' from MD_Location where Code ='9201'
Update MD_Location
Set SAPLocation ='4010' from MD_Location where Code ='9202'
--销售组织 ： 0101
--分销渠道 ： 10
Select * from MD_SwitchTrading
Insert into MD_SwitchTrading  
Select '9101-9306','30607','500090',050,0101 ,10,'2603','超级用户',GETDATE(),'3','超级用户',GETDATE()
union
Select '9101-9306A','30607','500090',050,0101 ,10,'2603','超级用户',GETDATE(),'3','超级用户',GETDATE()
union
Select '9301-9306','30607','500090',050,0101 ,10,'2603','超级用户',GETDATE(),'3','超级用户',GETDATE()

Insert into SYS_EntityPreference
Select 90110,27,'30333-9306‖30520-9306‖30709-9306','接口:采购转手贸易','2603','超级用户',GETDATE(),'2603','超级用户',GETDATE()
Update Sconit_20140806.dbo.SYS_EntityPreference
Set Value=1 from Sconit_20140806.dbo.SYS_EntityPreference where Id =90102
Update Sconit_20140806.dbo.SYS_EntityPreference Set Value ='y123456' where Id =11002
Update Sconit_20140806.dbo.SYS_EntityPreference Set Value ='10.166.1.72' where Id =11004

CREATE TABLE [dbo].[ZTMP_INV](
	[Item] [varchar](50) NULL,
	[SAPLocation] [varchar](50) NULL,
	[Supplier] [varchar](50) NULL,
	[Uom] [nvarchar](255) NULL,
	[SAPQty] [float] NULL,
	[SAPinspectQty] [float] NULL
) ON [PRIMARY]
 Drop table [Inv_RecSIExecution] 
CREATE TABLE [dbo].[Inv_RecSIExecution](
	[Item] [varchar](50) NOT NULL,
	[Uom] [varchar](5) NOT NULL,
	[MesLocation] [varchar](50) NOT NULL,
	[SAPLocation] [varchar](50) NOT NULL,
	[RealTimeQTY] [decimal](38, 8) NULL,
	[IpQty] [decimal](38, 8) NULL,
	[NewTransQty] [decimal](38, 8) NULL,
	[TodayQty] [decimal](38, 8) NULL,
	[TotalQty] [decimal](38, 8) NULL,
	[CreateDate] [datetime] NULL
) ON [PRIMARY]
/* 为了防止任何可能出现的数据丢失问题，您应该先仔细检查此脚本，然后再在数据库设计器的上下文之外运行此脚本。*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.SAP_Interface_STMES0001 ADD
	SPART varchar(50) NULL
GO
ALTER TABLE dbo.SAP_Interface_STMES0001 SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
/* 为了防止任何可能出现的数据丢失问题，您应该先仔细检查此脚本，然后再在数据库设计器的上下文之外运行此脚本。*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Inv_RecSIExecution
	(
	Item varchar(50) NOT NULL,
	Uom varchar(5) NOT NULL,
	MesLocation varchar(50) NOT NULL,
	SAPLocation varchar(50) NOT NULL,
	RealTimeQTY decimal(38, 8) NULL,
	RealTimeQTYOriginal decimal(38, 8) NULL,
	IpQty decimal(38, 8) NULL,
	NewTransQty decimal(38, 8) NULL,
	TodayQty decimal(38, 8) NULL,
	TotalQty decimal(38, 8) NULL,
	CreateDate datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Inv_RecSIExecution SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.Inv_RecSIExecution)
	 EXEC('INSERT INTO dbo.Tmp_Inv_RecSIExecution (Item, Uom, MesLocation, SAPLocation, RealTimeQTY, IpQty, NewTransQty, TodayQty, TotalQty, CreateDate)
		SELECT Item, Uom, MesLocation, SAPLocation, RealTimeQTY, IpQty, NewTransQty, TodayQty, TotalQty, CreateDate FROM dbo.Inv_RecSIExecution WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.Inv_RecSIExecution
GO
EXECUTE sp_rename N'dbo.Tmp_Inv_RecSIExecution', N'Inv_RecSIExecution', 'OBJECT' 
GO
COMMIT
