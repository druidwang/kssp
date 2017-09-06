
/****** Object:  Table [dbo].[WMS_ShipPlan]    Script Date: 2015/11/16 16:50:43 ******/
DROP TABLE [dbo].[WMS_ShipPlan]
GO

/****** Object:  Table [dbo].[WMS_ShipPlan]    Script Date: 2015/11/16 16:50:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_ShipPlan](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Flow] [varchar](50) NULL,
	[OrderNo] [varchar](50) NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[WindowTime] [datetime] NULL,
	[Item] [varchar](50) NOT NULL,
	[ItemDesc] [varchar](100) NOT NULL,
	[RefItemCode] [varchar](50) NULL,
	[Uom] [varchar](5) NOT NULL,
	[BaseUom] [varchar](5) NOT NULL,
	[UC] [decimal](18, 8) NOT NULL,
	[UCDesc] [varchar](50) NULL,
	[OrderQty] [decimal](18, 8) NOT NULL,
	[ShipQty] [decimal](18, 8) NOT NULL,
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
	[Dock] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_WMS_ShipPlan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO