/****** Object:  UserDefinedTableType [dbo].[PickResultTableType]    Script Date: 2015/12/24 10:27:47 ******/
DROP TYPE [dbo].[PickResultTableType]
GO

/****** Object:  UserDefinedTableType [dbo].[PickResultTableType]    Script Date: 2015/12/24 10:27:47 ******/
CREATE TYPE [dbo].[PickResultTableType] AS TABLE(
	[PickTaskID] [int] NOT NULL,
	[HuId] [varchar](50) NULL,
	[Qty] [decimal](18, 8) NULL
)
GO

alter table WMS_PickResult alter column CancelDate datetime null
go
alter table WMS_PickResult alter column CancelUser int null
go
alter table WMS_PickResult alter column CancelUserNm varchar(50) null
go