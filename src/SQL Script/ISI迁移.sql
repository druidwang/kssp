----建表
CREATE TABLE [dbo].[ISI_TaskSubType](
	[Code] [varchar](50) NOT NULL,
	[Desc_] [varchar](50) NULL,
	[Parent] [varchar](50) NULL,
	[Type] [tinyint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsAutoAssign] [bit] NOT NULL,
	[Seq] [int] NULL,
	[AssignUser] [varchar](300) NULL,
	[StartUser] [varchar](300) NULL,
	[IsAssignUp] [bit] NOT NULL,
	[AssignUpTime] [decimal](18, 8) NULL,
	[AssignUpUser] [varchar](300) NULL,
	[IsStartUp] [bit] NOT NULL,
	[StartUpTime] [decimal](18, 8) NULL,
	[StartUpUser] [varchar](300) NULL,
	[IsCloseUp] [bit] NOT NULL,
	[CloseUpTime] [decimal](18, 8) NULL,
	[CloseUpUser] [varchar](300) NULL,
	[CreateUserNm] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](50) NULL,
	[LastModifyUser][int] NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[IsPublic] [bit] NOT NULL,
	[IsCompleteUp] [bit] NOT NULL,
	[CompleteUpTime] [decimal](18, 8) NULL,
	[IsStart] [bit] NOT NULL,
	[StartPercent] [decimal](18, 8) NULL,
	[ProjectType] [varchar](20) NULL,
	[IsQuote] [bit] NOT NULL,
	[IsInitiation] [bit] NOT NULL,
	[IsReport] [bit] NOT NULL,
	[ViewUser] [varchar](300) NULL,
	[IsOpen] [bit] NOT NULL,
	[OpenTime] [decimal](18, 8) NULL,
	[Org] [varchar](50)  NULL,
	[Version] [int] NOT NULL,
	[ECType] [varchar](20) NULL,
	[ECUser] [varchar](300) NULL,
	[IsEC] [bit] NOT NULL,
	[IsAutoStart] [bit] NOT NULL,
	[IsAutoComplete] [bit] NOT NULL,
	[IsAutoClose] [bit] NOT NULL,
	[IsAutoStatus] [bit] NOT NULL,
	[RegisterNo] [varchar](50) NULL,
	[ExtNo] [varchar](255) NULL,
	[Amount] [decimal](18, 9) NULL,
	[IsWF] [bit] NOT NULL,
 CONSTRAINT [PK_ISI_TASKSUBTYPE] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'3rd Party Consent', N'Third Party Consent', NULL, N'General', 1, 0, 130, N'|suyiru,qianpeijia|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A34C00B37F90 AS DateTime), N'su', CAST(0x0000A34C00B37F90 AS DateTime), N'su', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 1, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'Confirmatory dd', N'Confirmatory dd', NULL, N'General', 1, 0, 180, N'|huxiufeng|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A35600BAE7A8 AS DateTime), N'xuweili', CAST(0x0000A35600BAE7A8 AS DateTime), N'xuweili', 0, 0, CAST(0.00000000 AS Decimal(18, 8)), 0, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 0, N'', 0, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 1, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'Evaluation & audit', N'special audit and evaluation of both sides', NULL, N'General', 1, 0, 150, N'|xuweili|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A34F00F59CF4 AS DateTime), N'gewei', CAST(0x0000A34F00F5DE94 AS DateTime), N'gewei', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 2, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'Falcon-Application', N'Application Falcon Project', NULL, N'General', 1, 0, 2000, N'|tanweijuan,zoulei|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A363018162FC AS DateTime), N'wangweizhong', CAST(0x0000A3630182B800 AS DateTime), N'wangweizhong', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 2, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'Falcon-Infrasructure', N'Infrastructure Falcon Project', NULL, N'General', 1, 0, 2010, N'|xubo,lideqing|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A3630181C7C4 AS DateTime), N'wangweizhong', CAST(0x0000A36301827E94 AS DateTime), N'wangweizhong', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 2, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'Falcon-ItOperations', N'IT Operations Falcon Project', NULL, N'General', 1, 0, 2020, N'|xubo,lideqing|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A363018249D8 AS DateTime), N'wangweizhong', CAST(0x0000A363018249D8 AS DateTime), N'wangweizhong', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 1, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'Falcon-ItTSA', N'IT TSA Falcon Project', NULL, N'General', 1, 0, 2030, N'|tanweijuan,zoulei|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A363018316EC AS DateTime), N'wangweizhong', CAST(0x0000A363018316EC AS DateTime), N'wangweizhong', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 1, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'FTZco Capital Increase', N'FTZco Capital Increase', NULL, N'General', 1, 0, 140, N'|minxiongwei,yangcanhua|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A34C00B3DE7C AS DateTime), N'su', CAST(0x0000A34D00033DB0 AS DateTime), N'su', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 2, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'FTZco Establish', N'FTZco Establishment', NULL, N'General', 1, 0, 100, N'|minxiongwei|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A34B01222F80 AS DateTime), N'su', CAST(0x0000A34D00FF2148 AS DateTime), N'su', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 7, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'JCI Carve-out', N'JCI Carve-out Plan & Evalution', NULL, N'General', 1, 0, 110, N'|huxiufeng,zuojing|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A34B01231B0C AS DateTime), N'su', CAST(0x0000A34B012333A8 AS DateTime), N'su', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'本部', 2, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'JV DD', N'JV DD', NULL, N'General', 1, 0, 190, N'', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A357008EEFCC AS DateTime), N'xuweili', CAST(0x0000A357008EEFCC AS DateTime), N'xuweili', 0, 0, CAST(0.00000000 AS Decimal(18, 8)), 0, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 0, N'', 0, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 1, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'Sign contracts', N'legal contracts prepare and sign', NULL, N'General', 1, 0, 160, N'|yangcanhua|', N'|wuyanjie|', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A34F00F926D0 AS DateTime), N'gewei', CAST(0x0000A34F00F9360C AS DateTime), N'gewei', 0, 0, CAST(0.00000000 AS Decimal(18, 8)), 0, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 0, N'', 0, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 2, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'Tax reimbursement', N'JCI to reimburse tax payment to YF', NULL, N'General', 1, 0, 170, N'|zhuling|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A34F00F97428 AS DateTime), N'gewei', CAST(0x0000A34F00F978D8 AS DateTime), N'gewei', 0, 0, CAST(0.00000000 AS Decimal(18, 8)), 0, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 0, N'', 0, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 2, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'TTS Using', N'TTS Using Issue Posting', NULL, N'Issue', 1, 1, 999, N'|su|', N'|su,lideqing|', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A34B0130824C AS DateTime), N'gewei', CAST(0x0000A34D00FEC004 AS DateTime), N'su', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'|gewei|', 1, CAST(0.00000000 AS Decimal(18, 8)), N'延锋', 3, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)
INSERT [dbo].[ISI_TaskSubType] ([Code], [Desc_], [Parent], [Type], [IsActive], [IsAutoAssign], [Seq], [AssignUser], [StartUser], [IsAssignUp], [AssignUpTime], [AssignUpUser], [IsStartUp], [StartUpTime], [StartUpUser], [IsCloseUp], [CloseUpTime], [CloseUpUser], [CreateDate], [CreateUser], [LastModifyDate], [LastModifyUser], [IsPublic], [IsCompleteUp], [CompleteUpTime], [IsStart], [StartPercent], [ProjectType], [IsQuote], [IsInitiation], [IsReport], [ViewUser], [IsOpen], [OpenTime], [Org], [Version], [ECType], [ECUser], [IsEC], [IsAutoStart], [IsAutoComplete], [IsAutoClose], [IsAutoStatus], [RegisterNo], [ExtNo], [Amount], [IsWF]) VALUES (N'YF Carve-out', N'YF Carve-out Plan & Evalution', NULL, N'General', 1, 0, 120, N'|yuanxinhua|', N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', 0, CAST(4320.00000000 AS Decimal(18, 8)), N'', CAST(0x0000A34B012460D4 AS DateTime), N'su', CAST(0x0000A34B012460D4 AS DateTime), N'su', 0, 1, CAST(0.00000000 AS Decimal(18, 8)), 1, CAST(0.70000000 AS Decimal(18, 8)), N'', 1, 1, 1, N'', 1, CAST(0.00000000 AS Decimal(18, 8)), N'本部', 1, N'', N'', 1, 0, 0, 0, 0, NULL, NULL, NULL, 0)




/****** Object:  Table [dbo].[ISI_TaskMstr]    Script Date: 01/14/2015 14:09:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ISI_TaskMstr](
	[Code] [varchar](50) NOT NULL,
	[Address] [varchar](100) NULL,
	[Type] [tinyint] NOT NULL,
	[Subject] [varchar](100) NULL,
	[Desc1] [varchar](4000) NULL,
	[Desc2] [varchar](1000) NULL,
	[Status] [tinyint] NOT NULL,
	[Priority] [varchar](50) NOT NULL,
	[StartDate] [datetime] NULL,
	[TaskSubType] [varchar](50) NOT NULL,
	[FailureMode] [varchar](50) NULL,
	[TraceCode] [varchar](50) NULL,
	[Flag] [varchar](10) NULL,
	[Color] [varchar](10) NULL,
	[PlanStartDate] [datetime] NULL,
	[PlanCompleteDate] [datetime] NULL,
	[Wiki] [varchar](1000) NULL,
	[ExpectedResults] [varchar](1000) NULL,
	[UserName] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[MobilePhone] [varchar](50) NULL,
	[Scheduling] [int] NULL,
	[SchedulingStartUser] [varchar](300) NULL,
	[SchedulingShift] [varchar](50) NULL,
	[SchedulingShiftTime] [varchar](255) NULL,
	[AssignStartUser] [varchar](500) NULL,
	[CreateUserNm] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[SubmitUserNm] [varchar](50) NULL,
	[SubmitUser] [int] NULL,
	[SubmitDate] [datetime] NULL,
	[CancelUserNm] [varchar](50) NULL,
	[CancelUser] [int] NULL,
	[CancelDate] [datetime] NULL,
	[AssignUserNm] [varchar](50) NULL,
	[AssignUser] [int] NULL,
	[AssignDate] [datetime] NULL,
	[StartUserNm] [varchar](50) NULL,
	[StartUser] [int] NULL,
	[CompleteUserNm] [varchar](50) NULL,
	[CompleteUser] [int] NULL,
	[CompleteDate] [datetime] NULL,
	[CloseUserNm] [varchar](50) NULL,
	[CloseUser] [int] NULL,
	[CloseDate] [datetime] NULL,
	[ReassignDate] [datetime] NULL,
	[ReassignUserNm] [varchar](50) NULL,
	[ReassignUser] [int] NULL,
	[LastModifyUserNm] [varchar](50) NULL,
	[LastModifyUser][int] NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[Seq] [varchar](10) NULL,
	[Phase] [varchar](10) NULL,
	[ProjectTaskId] [int] NOT NULL,
	[Version] [int] NOT NULL,
	[AssignStartUserNm] [varchar](500) NULL,
	[OpenUser] [int] NULL,
	[OpenUserNm] [varchar](50) NULL,
	[OpenDate] [datetime] NULL,
	[PatrolTime] [varchar](50) NULL,
	[LastSendEmailTime] [datetime] NULL,
	[RejectUser] [int] NULL,
	[RejectUserNm] [varchar](50) NULL,
	[RejectDate] [datetime] NULL,
	[FocusUser] [varchar](500) NULL,
	[PlanAmount] [decimal](18, 9) NULL,
	[Amount] [decimal](18, 9) NULL,
	[IsAutoStart] [bit] NOT NULL,
	[IsAutoComplete] [bit] NOT NULL,
	[IsAutoClose] [bit] NOT NULL,
	[IsAutoAssign] [bit] NOT NULL,
	[IsAutoStatus] [bit] NOT NULL,
	[AssignUpUser] [varchar](300) NULL,
 CONSTRAINT [PK_ISI_TASKMSTR] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[ISI_TaskAddr]    Script Date: 01/14/2015 14:09:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ISI_TaskAddr](
	[Code] [varchar](50) NOT NULL,
	[Desc_] [varchar](100) NOT NULL,
	[Parent] [varchar](50) NULL,
	[Seq] [int] NOT NULL,
	[CreateUserNm] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](50) NULL,
	[LastModifyUser][int] NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ISI_TASKADDR] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[ISI_Filter]    Script Date: 01/14/2015 14:09:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ISI_Filter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskType] [varchar](50) NULL,
	[TaskSubType] [varchar](50) NULL,
	[TaskCode] [varchar](50) NULL,
	[UserCode] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[CreateUserNm] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](50) NULL,
	[LastModifyUser][int] NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ISI_FILTER] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[ISI_TaskDet]    Script Date: 01/14/2015 14:09:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ISI_TaskDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskCode] [varchar](50) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[BackYards] [varchar](50) NULL,
	[Level_] [varchar](50) NULL,
	[Priority] [tinyint] NOT NULL,
	[IsSMS] [bit] NOT NULL,
	[IsEmail] [bit] NOT NULL,
	[Receiver] [varchar](50) NOT NULL,
	[Email] [varchar](50) NULL,
	[EmailCount] [int] NOT NULL,
	[EmailStatus] [varchar](10) NOT NULL,
	[MobilePhone] [varchar](50) NULL,
	[SMSStatus] [varchar](10) NOT NULL,
	[SMSCount] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUserNm] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](50) NULL,
	[LastModifyUser][int] NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[TaskSubType] [varchar](50) NOT NULL,
	[Subject] [varchar](100) NULL,
	[Desc1] [varchar](4000) NULL,
	[Desc2] [varchar](1000) NULL,
	[FailureMode] [varchar](50) NULL,
	[Color] [varchar](50) NULL,
	[Flag] [varchar](10) NULL,
	[PlanStartDate] [datetime] NULL,
	[PlanCompleteDate] [datetime] NULL,
	[ExpectedResults] [varchar](1000) NULL,
	[UserName] [varchar](50) NULL,
	[UserEmail] [varchar](50) NULL,
	[UserMobilePhone] [varchar](50) NULL,
 CONSTRAINT [PK_ISI_TASKDET] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


----进展
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ISI_TaskStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskCode] [varchar](50) NOT NULL,
	[Desc_] [varchar](1000) NOT NULL,
	[Flag] [varchar](10) NULL,
	[Color] [varchar](10) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CreateUserNm] [varchar](50) NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUserNm] [varchar](50) NULL,
	[LastModifyUser][int] NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ISI_TASKSTATUS] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO


----菜单
insert into ACC_PermissionCategory values('ISI','任务管理',1)
insert into ACC_Permission (Code,Desc1,Category) values('Url_TaskSubType_Edit','任务分类编辑','ISI')
insert into ACC_Permission (Code,Desc1,Category) values('Url_TaskSubType_View','任务分类查看','ISI')
insert into ACC_Permission (Code,Desc1,Category) values('Url_TaskMaster_View','我的任务','ISI')
insert into ACC_Permission (Code,Desc1,Category) values('Url_TaskAddress_View','地点','ISI')


insert into sys_menu values('Menu.ISI','任务管理',null,1000,'任务管理',null,'~/Content/Images/Nav/Default.png',1)
insert into sys_menu values('Menu.ISI.Trans','事务','Menu.ISI',100,'事务',null,'~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Menu.ISI.Info','信息','Menu.ISI',200,'信息',null,'~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Menu.ISI.Setup','设置','Menu.ISI',300,'设置',null,'~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Url_TaskSubType_View','任务分类','Menu.ISI.Setup',100,'任务分类','~/TaskSubType/Index','~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Url_TaskMaster_View','我的任务','Menu.ISI.Trans',100,'我的任务','~/TaskMaster/Index','~/Content/Images/Nav/Trans.png',1)
insert into sys_menu values('Url_TaskAddress_View','地点','Menu.ISI.Setup',200,'地点','~/TaskAddress/Index','~/Content/Images/Nav/Trans.png',1)


insert into SYS_CodeMstr values('TaskPriority','优先级',0)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq) values('TaskPriority',0,'CodeDetail_TaskPriority_Normal',1,1)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq) values('TaskPriority',1,'CodeDetail_TaskPriority_Urgent',0,2)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq) values('TaskPriority',2,'CodeDetail_TaskPriority_High',0,3)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq) values('TaskPriority',3,'CodeDetail_TaskPriority_Major',0,4)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq) values('TaskPriority',4,'CodeDetail_TaskPriority_Low',0,5)


insert into SYS_CodeMstr values('TaskType', '任务类型', 0)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',0,'CodeDetail_TaskType_Audit', 1,1)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',1,'CodeDetail_TaskType_Change', 0,2)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',2,'CodeDetail_TaskType_Ecn',  0, 3)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',3,'CodeDetail_TaskType_General',0,4)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',4,'CodeDetail_TaskType_Improve',  0,5)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',5,'CodeDetail_TaskType_Issue', 0, 6)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',6,'CodeDetail_TaskType_Plan',  0, 7)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',7,'CodeDetail_TaskType_Privacy', 0,8)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',8,'CodeDetail_TaskType_PrjectIssue',  0, 9)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',9,'CodeDetail_TaskType_Project',  0, 10)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskType',10,'CodeDetail_TaskType_Response',  0, 11)


insert into SYS_CodeMstr values('TaskStatus', '任务状态', 0)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskStatus',0,'CodeDetail_TaskStatus_Create', 1,1)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskStatus',1,'CodeDetail_TaskStatus_Submit', 0,2)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskStatus',2,'CodeDetail_TaskStatus_Cancel',  0, 3)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskStatus',3,'CodeDetail_TaskStatus_Assign',0,4)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskStatus',4,'CodeDetail_TaskStatus_InProcess', 0,5)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskStatus',5,'CodeDetail_TaskStatus_Complete', 0, 6)
insert into SYS_CodeDet (code,value,Desc1,IsDefault,seq)  VALUES ('TaskStatus',6,'CodeDetail_TaskStatus_Close',  0, 7)

update SYS_EntityPreference set Value='smtp.qiye.163.com' where id=10008
update SYS_EntityPreference set Value='sconit@sconit.com' where id=10007
update SYS_EntityPreference set Value='sconit' where id=10009


insert into SYS_EntityPreference values(10073,'100','延科','公司名称',2603,'用户 超级',getdate(),2603,'用户 超级',GETDATE())
insert into SYS_EntityPreference values(10074,'100','www.sconit.com','公司网址',2603,'用户 超级',getdate(),2603,'用户 超级',GETDATE())

/****** Object:  StoredProcedure [dbo].[USP_Search_TaskEmail]    Script Date: 2015/5/22 15:38:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_Search_TaskEmail]
(
	-- Add the parameters for the stored procedure here
	@TaskType varchar(50),
	@TaskSubType varchar(50),
	@TaskCode varchar(50),
	@UserIds varchar(1000),
	@CurrentUser varchar(50)
)
AS
BEGIN
DECLARE @SqlHeader varchar(8000);

set @SqlHeader='';

	set @SqlHeader = @SqlHeader+' select max(u.Code) Code,u.Email Email from ACC_User u ';
	
	set @SqlHeader = @SqlHeader+' where u.IsActive =1 and u.Id in ('+@UserIds +') ';
	set @SqlHeader = @SqlHeader+' and u.Email not in ( ';
	set @SqlHeader = @SqlHeader+' select u1.Email from ISI_Filter f join ACC_User u1 on (f.usercode=u1.Code or f.email=u1.Email) ';
	set @SqlHeader = @SqlHeader+'  and (f.TaskType is null or  f.TaskType ='''+@TaskType+''') ';
	set @SqlHeader = @SqlHeader+'  and (f.TaskSubType is null or  f.TaskSubType ='''+@TaskSubType+''') ';
	set @SqlHeader = @SqlHeader+'  and (f.TaskCode is null or  f.TaskCode ='''+@TaskCode+''') )' ;	
	set @SqlHeader = @SqlHeader+' and u.Email not in ( ';
	set @SqlHeader = @SqlHeader+' select u2.Email from ACC_User u2  where u2.Id = '''+@CurrentUser+'''';
	set @SqlHeader = @SqlHeader+' )  group by  u.Email ';
	--print @SqlHeader
	exec(@SqlHeader)
	--exec USP_Search_TaskEmail 'Plan','ZRFZ', 'PLN000006322','chengyunpeng'',''monitor','chengyunpeng'
	
End



insert into SYS_CodeMstr(Code,Desc1,Type) values ('TaskFlag','任务标记',0)
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TaskFlag',1,'CodeDetail_TaskFlag_DI2',0,2);
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TaskFlag',2,'CodeDetail_TaskFlag_DI3',0,3);
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TaskFlag',3,'CodeDetail_TaskFlag_DI4',0,4);

insert into SYS_CodeMstr(Code,Desc1,Type) values ('TaskColor','任务颜色',0)
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TaskColor',0,'CodeDetail_TaskColor_Green',1,1);
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TaskColor',1,'CodeDetail_TaskColor_Yellow',0,2);
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TaskColor',2,'CodeDetail_TaskColor_Red',0,3);