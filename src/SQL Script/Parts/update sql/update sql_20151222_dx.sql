DROP TABLE [dbo].[WMS_PickOccupy]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_PickOccupy](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UUID] [varchar](50) NOT NULL,
	[OrderNo] [varchar](50) NOT NULL,
	[OrderSeq] [int] NOT NULL,
	[ShipPlanId] [int] NOT NULL,
	[TargetDock] [varchar](50) NULL,
	[OccupyQty] [decimal](18, 8) NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[ReleaseQty] [decimal](18, 8) NULL,
 CONSTRAINT [PK_WMS_PickOccupy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

DROP TABLE [dbo].[WMS_RepackOccupy]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_RepackOccupy](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UUID] [varchar](50) NOT NULL,
	[OrderNo] [varchar](50) NOT NULL,
	[OrderSeq] [int] NOT NULL,
	[ShipPlanId] [int] NOT NULL,
	[TargetDock] [varchar](50) NULL,
	[OccupyQty] [decimal](18, 8) NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[ReleaseQty] [decimal](18, 8) NULL,
 CONSTRAINT [PK_WMS_RepackOccupy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

DROP TABLE [dbo].[WMS_BuffOccupy]
GO

/****** Object:  Table [dbo].[WMS_BuffOccupy]    Script Date: 2015/12/22 14:14:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_BuffOccupy](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UUID] [varchar](50) NOT NULL,
	[OrderNo] [varchar](50) NOT NULL,
	[OrderSeq] [int] NOT NULL,
	[ShipPlanId] [int] NOT NULL,
	[TargetDock] [varchar](50) NULL,
	[OccupyQty] [decimal](18, 8) NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_WMS_BuffOccupy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[WMS_RepackTask]    Script Date: 2015/12/22 14:19:03 ******/
DROP TABLE [dbo].[WMS_RepackTask]
GO

/****** Object:  Table [dbo].[WMS_RepackTask]    Script Date: 2015/12/22 14:19:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_RepackTask](
	[UUID] [varchar](50) NOT NULL,
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
	[UUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

DROP TABLE [dbo].[WMS_PickTask]
GO

/****** Object:  Table [dbo].[WMS_PickTask]    Script Date: 2015/12/22 14:19:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WMS_PickTask](
	[UUID] [varchar](50) NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Priority] [tinyint] NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[ItemDesc] [varchar](100) NOT NULL,
	[RefItemCode] [varchar](50) NULL,
	[Uom] [varchar](5) NOT NULL,
	[BaseUom] [varchar](5) NOT NULL,
	[UnitQty] [decimal](18, 8) NOT NULL,
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
	[IsPickHu] [bit] NULL,
	[NeedRepack] [bit] NULL,
	[IsOdd] [bit] NULL,
 CONSTRAINT [PK_WMS_PickTask] PRIMARY KEY CLUSTERED 
(
	[UUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  UserDefinedTableType [dbo].[PickResultTableType]    Script Date: 2015/12/22 15:06:07 ******/
DROP TYPE [dbo].[PickResultTableType]
GO

/****** Object:  UserDefinedTableType [dbo].[PickResultTableType]    Script Date: 2015/12/22 15:06:07 ******/
CREATE TYPE [dbo].[PickResultTableType] AS TABLE(
	[PickTaskUUID] [varchar](50) NOT NULL,
	[HuId] [int] NULL,
	[Qty] [decimal](18, 8) NULL
)
GO
