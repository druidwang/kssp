DROP TABLE [dbo].[TMS_Mileage]
GO

/****** Object:  Table [dbo].[TMS_Mileage]    Script Date: 2015/11/3 16:21:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TMS_Mileage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Desc] [varchar](255) NULL,
	[TransMode] [tinyint] NULL,
	[ShipFrom] [varchar](50) NULL,
	[ShipFromDesc] [varchar](255) NULL,
	[ShipTo] [varchar](50) NULL,
	[ShipToDesc] [varchar](255) NULL,
	[IsActive] [bit] NULL,
	[Distance] [decimal](18, 8) NULL,
	[CreateUserNm] [varchar](255) NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](255) NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TMS_Mileage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



/****** Object:  Table [dbo].[TMS_Vehicle]    Script Date: 2015/11/3 16:46:59 ******/
DROP TABLE [dbo].[TMS_Vechile]
GO

/****** Object:  Table [dbo].[TMS_Vehicle]    Script Date: 2015/11/3 16:46:59 ******/
DROP TABLE [dbo].[TMS_Vehicle]
GO

/****** Object:  Table [dbo].[TMS_Vehicle]    Script Date: 2015/11/3 16:46:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TMS_Vehicle](
	[Code] [varchar](50) NOT NULL,
	[Desc] [varchar](255) NOT NULL,
	[DrivingNo] [varchar](50) NULL,
	[Owner] [varchar](50) NOT NULL,
	[Phone] [varchar](50) NULL,
	[MobilePhone] [varchar](50) NULL,
	[VIN] [varchar](50) NULL,
	[EngineNo] [varchar](50) NULL,
	[Address] [varchar](255) NULL,
	[Fax] [varchar](50) NULL,
	[Driver] [varchar](50) NULL,
	[Tonnage] [varchar](50) NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TMS_Vehicle] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[TMS_OrderMstr]    Script Date: 2015/11/4 14:37:58 ******/
DROP TABLE [dbo].[TMS_OrderMstr]
GO

/****** Object:  Table [dbo].[TMS_OrderMstr]    Script Date: 2015/11/4 14:37:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TMS_OrderMstr](
	[OrderNo] [varchar](50) NOT NULL,
	[ExtOrderNo] [varchar](50) NULL,
	[RefOrderNo] [varchar](50) NULL,
	[Flow] [varchar](50) NULL,
	[FlowDesc] [varchar](255) NULL,
	[Status] [tinyint] NULL,
	[Carrier] [varchar](50) NULL,
	[CarrierNm] [varchar](255) NULL,
	[Vehicle] [varchar](50) NULL,
	[Tonnage] [varchar](50) NULL,
	[DrivingNo] [varchar](50) NULL,
	[Driver] [varchar](50) NULL,
	[DriverMobilePhone] [nchar](10) NULL,
	[LoadVolume] [decimal](18, 8) NULL,
	[LoadWeight] [decimal](18, 8) NULL,
	[MinLoadRate] [decimal](18, 8) NULL,
	[IsAutoSubmit] [bit] NULL,
	[IsAutoStart] [bit] NULL,
	[ShipFrom] [varchar](50) NULL,
	[ShipFromAddr] [varchar](255) NULL,
	[ShipTo] [varchar](50) NULL,
	[ShipToAddr] [varchar](255) NULL,
	[TransMode] [tinyint] NOT NULL,
	[PriceList] [varchar](50) NULL,
	[BillAddr] [varchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[SubmitDate] [datetime] NULL,
	[SubmitUser] [varchar](50) NULL,
	[SubmitUserNm] [varchar](100) NULL,
	[StartDate] [datetime] NULL,
	[StartUser] [varchar](50) NULL,
	[StartUserNm] [varchar](100) NULL,
	[CloseDate] [datetime] NULL,
	[CloseUserNm] [varchar](100) NULL,
	[CloseUser] [varchar](50) NULL,
	[CancelDate] [datetime] NULL,
	[CancelUser] [varchar](50) NULL,
	[CancelUserNm] [varchar](100) NULL,
	[Version] [int] NULL,
 CONSTRAINT [PK_TMS_OrderMstr] PRIMARY KEY CLUSTERED 
(
	[OrderNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[TMS_OrderDet]    Script Date: 2015/11/4 14:38:38 ******/
DROP TABLE [dbo].[TMS_OrderDet]
GO

/****** Object:  Table [dbo].[TMS_OrderDet]    Script Date: 2015/11/4 14:38:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TMS_OrderDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNo] [varchar](50) NOT NULL,
	[Seq] [int] NOT NULL,
	[IpNo] [varchar](50) NULL,
	[OrderRouteId] [int] NULL,
	[EstPalletQty] [int] NULL,
	[PalletQty] [int] NULL,
	[EstVolume] [decimal](18, 8) NULL,
	[Volume] [decimal](18, 8) NULL,
	[EstWeight] [decimal](18, 8) NULL,
	[Weight] [decimal](18, 8) NULL,
	[EstBoxCount] [int] NULL,
	[BoxCount] [int] NULL,
	[LoadTime] [datetime] NULL,
	[UnloadTime] [datetime] NULL,
	[PartyFrom] [varchar](50) NOT NULL,
	[PartyFromNm] [varchar](100) NULL,
	[PartyTo] [varchar](50) NOT NULL,
	[PartyToNm] [varchar](100) NULL,
	[ShipFrom] [varchar](50) NULL,
	[ShipFromAddr] [varchar](255) NULL,
	[ShipTo] [varchar](50) NULL,
	[ShipToAddr] [varchar](255) NULL,
	[Dock] [varchar](50) NULL,
	[Distance] [decimal](18, 8) NULL,
	[IsRec] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[CreateUserNm] [varchar](255) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
	[LastModifyUserNm] [varchar](255) NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TMS_OrderDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[TMS_OrderRoute]    Script Date: 2015/11/4 14:38:53 ******/
DROP TABLE [dbo].[TMS_OrderRoute]
GO

/****** Object:  Table [dbo].[TMS_OrderRoute]    Script Date: 2015/11/4 14:38:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TMS_OrderRoute](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNo] [varchar](50) NOT NULL,
	[Seq] [int] NOT NULL,
	[ShipAddr] [varchar](50) NOT NULL,
	[ShipAddrDesc] [varchar](255) NULL,
	[Distance] [decimal](18, 8) NULL,
	[EstDepartTime] [datetime] NULL,
	[EstArriveTime] [datetime] NULL,
	[DepartTime] [datetime] NULL,
	[DepartInputUser] [varchar](50) NULL,
	[DepartInputUserNm] [varchar](255) NULL,
	[ArriveTime] [datetime] NULL,
	[ArriveInputUser] [varchar](50) NULL,
	[ArriveInputUserNm] [varchar](255) NULL,
	[LoadRate] [decimal](18, 8) NULL,
	[WeightRate] [decimal](18, 8) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[CreateUserNm] [varchar](255) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
	[LastModifyUserNm] [varchar](255) NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TMS_OrderRoute] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO





