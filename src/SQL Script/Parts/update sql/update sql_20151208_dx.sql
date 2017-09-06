IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRD_WorkingCalendar]') AND type in (N'U'))
DROP TABLE [dbo].[PRD_WorkingCalendar]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRD_WorkingCalendar](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Region] [varchar](50) NULL,
	[Shift] [varchar](50) NOT NULL,
	[WorkingDate] [date] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[DayOfWeek] [tinyint] NOT NULL,
	[RegionName] [varchar](50) NULL,
	[Category] [tinyint] NOT NULL,
	[ProdLine] [varchar](50) NULL,
 CONSTRAINT [PK_PRD_WORKINGCALENDAR] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRD_StandardWorkingCalendar]') AND type in (N'U'))
DROP TABLE [dbo].[PRD_StandardWorkingCalendar]
GO

/****** Object:  Table [dbo].[PRD_StandardWorkingCalendar]    Script Date: 12/08/2015 14:09:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRD_StandardWorkingCalendar](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Region] [varchar](50) NULL,
	[Shift] [varchar](50) NOT NULL,
	[DayOfWeek] [tinyint] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[RegionName] [varchar](50) NULL,
	[Category] [tinyint] NOT NULL,
	[ProdLine] [varchar](50) NULL,
 CONSTRAINT [PK_PRD_STANDARDWORKINGCALENDAR] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRD_WorkingShiftMstr]') AND type in (N'U'))
DROP TABLE [dbo].[PRD_WorkingShiftMstr]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRD_WorkingShiftMstr](
	[Code] [varchar](50) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[ShiftCount] [int] NOT NULL,
 CONSTRAINT [PK_PRD_WORKINGSHIFTMSTR] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRD_WorkingShiftDet]') AND type in (N'U'))
DROP TABLE [dbo].[PRD_WorkingShiftDet]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRD_WorkingShiftDet](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Shift] [varchar](50) NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateUserNm] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [int] NOT NULL,
	[LastModifyUserNm] [varchar](100) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[StartTime] [varchar](5) NULL,
	[EndTime] [varchar](5) NULL,
	[IsOvernight] [int] NOT NULL,
	[Seq] [int] NOT NULL,
 CONSTRAINT [PK_PRD_WORKINGSHIFTDET] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO