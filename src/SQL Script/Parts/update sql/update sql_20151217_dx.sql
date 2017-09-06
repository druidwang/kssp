alter table WMS_ShipPlan add LocTo varchar(50)
go
alter table WMS_ShipPlan add LocToNm varchar(100)
go
alter table WMS_ShipPlan add Station varchar(50)
go

/****** Object:  Table [dbo].[WMS_DeliveryBarCode]    Script Date: 2015/12/17 16:55:01 ******/
DROP TABLE [dbo].[WMS_DeliveryBarCode]
GO

/****** Object:  Table [dbo].[WMS_DeliveryBarCode]    Script Date: 2015/12/17 16:55:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_DeliveryBarCode](
	[BarCode] [varchar](50) NOT NULL,
	[OrderNo] [varchar](50) NOT NULL,
	[OrderSeq] [int] NOT NULL,
	[StartTime] [datetime] NULL,
	[WindowTime] [datetime] NULL,
	[Item] [varchar](50) NOT NULL,
	[ItemDesc] [varchar](100) NOT NULL,
	[RefItemCode] [varchar](50) NULL,
	[Uom] [varchar](5) NOT NULL,
	[UC] [decimal](18, 8) NOT NULL,
	[UCDesc] [varchar](50) NULL,
	[Qty] [decimal](18, 8) NOT NULL,
	[Priority] [tinyint] NOT NULL,
	[PartyFrom] [varchar](50) NOT NULL,
	[PartyFromNm] [varchar](100) NULL,
	[PartyTo] [varchar](50) NOT NULL,
	[PartyToNm] [varchar](100) NULL,
	[ShipFrom] [varchar](50) NULL,
	[ShipFromAddr] [varchar](256) NULL,
	[ShipFromTel] [varchar](50) NULL,
	[ShipFromCell] [varchar](50) NULL,
	[ShipFromFax] [varchar](50) NULL,
	[ShipFromContact] [varchar](50) NULL,
	[ShipTo] [varchar](50) NULL,
	[ShipToAddr] [varchar](256) NULL,
	[ShipToTel] [varchar](50) NULL,
	[ShipToCell] [varchar](50) NULL,
	[ShipToFax] [varchar](50) NULL,
	[ShipToContact] [varchar](50) NULL,
	[LocFrom] [varchar](50) NOT NULL,
	[LocFromNm] [varchar](100) NULL,
	[LocTo] [varchar](50) NULL,
	[LocToNm] [varchar](100) NULL,
	[Station] [varchar](50) NULL,
	[Dock] [varchar](50) NULL,
	[IsActive] [bit] NULL,
	[ShipPlanId] [int] NULL,
	[HuId] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[Flow] [varchar](50) NULL,
 CONSTRAINT [PK_WMS_DeliveryBarCode] PRIMARY KEY CLUSTERED 
(
	[BarCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


