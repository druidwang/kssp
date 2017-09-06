alter table TMS_OrderMstr add CurrArriveSiteId int
go
alter table TMS_OrderMstr add CurrArriveShipAddr varchar(50)
go
alter table TMS_OrderMstr add CurrArriveShipAddrDesc varchar(255)
go
/****** Object:  Table [dbo].[WMS_RepackTask]    Script Date: 2015/11/27 15:55:27 ******/
DROP TABLE [dbo].[WMS_RepackTask]
GO

/****** Object:  Table [dbo].[WMS_RepackTask]    Script Date: 2015/11/27 15:55:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_RepackTask](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[ItemDesc] [varchar](100) NOT NULL,
	[RefItemCode] [varchar](50) NULL,
	[Uom] [varchar](5) NOT NULL,
	[BaseUom] [varchar](5) NOT NULL,
	[UnitQty] [decimal](18, 8) NOT NULL,
	[UC] [decimal](18, 8) NOT NULL,
	[UCDesc] [varchar](50) NULL,
	[Qty] [decimal](18, 8) NOT NULL,
	[RepackQty] [decimal](18, 8) NOT NULL,
	[Loc] [varchar](50) NOT NULL,
	[Priority] [tinyint] NOT NULL,
	[RepackGroup] [varchar](50) NULL,
	[RepackUserId] [int] NULL,
	[RepackUserNm] [varchar](100) NULL,
	[StartTime] [datetime] NOT NULL,
	[WinTime] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[CloseUser] [int] NULL,
	[CloseUserNm] [varchar](100) NULL,
	[CloseDate] [datetime] NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_WMS_RepackTask] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[WMS_RepackIn]    Script Date: 2015/11/27 15:55:27 ******/
DROP TABLE [dbo].[WMS_RepackIn]
GO

/****** Object:  Table [dbo].[WMS_RepackOut]    Script Date: 2015/11/27 15:55:27 ******/
DROP TABLE [dbo].[WMS_RepackOut]
GO

