alter table WMS_PickTask add NeedRepack bit
go

/****** Object:  UserDefinedTableType [dbo].[PickTargetTableType]    Script Date: 2015/12/15 14:45:13 ******/
DROP TYPE [dbo].[PickTargetTableType]
GO

/****** Object:  UserDefinedTableType [dbo].[PickTargetTableType]    Script Date: 2015/12/15 14:45:13 ******/
CREATE TYPE [dbo].[PickTargetTableType] AS TABLE(
	[Location] [varchar](50) NULL,
	[Item] [varchar](50) NULL,
	[Uom] [varchar](5) NULL,
	[UC] decimal(18, 8) NULL
)
GO