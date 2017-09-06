alter table WMS_ShipPlan add OrderSeq int
go
alter table WMS_ShipPlan add OrderDetId int
go
alter table WMS_ShipPlan add IsOccupyInv bit
go
alter table WMS_ShipPlan add CloseUser int
go
alter table WMS_ShipPlan add CloseUserNm varchar(100)
go
alter table WMS_ShipPlan add CloseDate datetime
go
alter table MD_Location add PickScheduleNo varchar(50)
go


/****** Object:  Table [dbo].[WMS_PickSchedule]    Script Date: 2015/11/19 13:49:24 ******/
DROP TABLE [dbo].[WMS_PickSchedule]
GO

/****** Object:  Table [dbo].[WMS_PickSchedule]    Script Date: 2015/11/19 13:49:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_PickSchedule](
	[PickScheduleNo] [varchar](50) NOT NULL,
	[PickLeadTime] [decimal](18, 8) NOT NULL,
	[RepackLeadTime] [decimal](18, 8) NOT NULL,
	[SpreadLeadTime] [decimal](18, 8) NOT NULL,
	[EmPickLeadTime] [decimal](18, 8) NOT NULL,
	[EmRepackLeadTime] [decimal](18, 8) NOT NULL,
	[EmSpreadLeadTime] [decimal](18, 8) NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_WMS_PickSchedule] PRIMARY KEY CLUSTERED 
(
	[PickScheduleNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



/****** Object:  Table [dbo].[WMS_PickWinTime]    Script Date: 2015/11/19 13:26:54 ******/
DROP TABLE [dbo].[WMS_PickWinTime]
GO

/****** Object:  Table [dbo].[WMS_PickWinTime]    Script Date: 2015/11/19 13:26:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_PickWinTime](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PickScheduleNo] [varchar](50) NOT NULL,
	[ShiftCode] [varchar](50) NOT NULL,
	[WinTime] [varchar](255) NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_WMS_PickWinTime] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


alter table WMS_ShipPlan add [Priority] tinyint not null
go

/****** Object:  Table [dbo].[WMS_PickTask]    Script Date: 2015/11/19 14:15:11 ******/
DROP TABLE [dbo].[WMS_PickTask]
GO

/****** Object:  Table [dbo].[WMS_PickTask]    Script Date: 2015/11/19 14:15:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_PickTask](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNo] [varchar](50) NULL,
	[OrderSeq] [int] NULL,
	[ShipPlanId] [int] NULL,
	[Priority] [tinyint] NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[ItemDesc] [varchar](100) NOT NULL,
	[RefItemCode] [varchar](50) NULL,
	[Uom] [varchar](5) NOT NULL,
	[BaseUom] [varchar](5) NOT NULL,
	[UC] [decimal](18, 8) NOT NULL,
	[UCDesc] [varchar](50) NULL,
	[OrderQty] [decimal](18, 8) NOT NULL,
	[PickQty] [decimal](18, 8) NOT NULL,
	[Loc] [varchar](50) NOT NULL,
	[Area] [varchar](50) NULL,
	[Bin] [varchar](50) NULL,
	[LotNo] [varchar](50) NULL,
	[HuId] [varchar](50) NULL,
	[PickBy] [tinyint] NOT NULL,
	[PickGroup] [varchar](50) NULL,
	[PickUserId] [int] NULL,
	[PickUserNm] [varchar](100) NULL,
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
 CONSTRAINT [PK_WMS_PickTask] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[WMS_PickResult]    Script Date: 2015/11/19 14:28:07 ******/
DROP TABLE [dbo].[WMS_PickResult]
GO

/****** Object:  Table [dbo].[WMS_PickResult]    Script Date: 2015/11/19 14:28:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_PickResult](
	[Id] [int] NOT NULL,
	[PickTaskId] [int] NOT NULL,
	[OrderNo] [varchar](50) NULL,
	[OrderSeq] [int] NULL,
	[ShipPlanId] [int] NULL,
	[Item] [varchar](50) NOT NULL,
	[ItemDesc] [varchar](100) NOT NULL,
	[RefItemCode] [varchar](50) NULL,
	[Uom] [varchar](5) NOT NULL,
	[BaseUom] [varchar](5) NOT NULL,
	[UC] [decimal](18, 8) NOT NULL,
	[UCDesc] [varchar](50) NULL,
	[PickQty] [decimal](18, 8) NOT NULL,
	[Loc] [varchar](50) NOT NULL,
	[Area] [varchar](50) NULL,
	[Bin] [varchar](50) NULL,
	[LotNo] [varchar](50) NULL,
	[HuId] [varchar](50) NULL,
	[PickBy] [tinyint] NOT NULL,
	[PickUserId] [int] NOT NULL,
	[PickUserNm] [varchar](100) NOT NULL,
	[PickDate] [datetime] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[IsCancel] [bit] NOT NULL,
	[CancelUser] [int] NOT NULL,
	[CancelUserNm] [varchar](100) NOT NULL,
	[CancelDate] [datetime] NOT NULL,
 CONSTRAINT [PK_WMS_PickResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



