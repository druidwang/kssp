USE [sconit5_test]
GO
Truncate table SYS_CodeMstr
GO
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'AddressType', N'地址', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'DayOfWeek', N'星期', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'Language', N'语言', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'LocationType', N'库区', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'PermissionCategoryType', N'权限分类', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'UserType', N'用户', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'WorkingCalendarType', N'日历', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'OrderType', N'订单类型', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'OrderPrioity', N'订单优先级', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'OrderStatus', N'订单状态', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'LogLevel', N'日志等级', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'IssueType', N'异常类型', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'IssuePriority', N'异常优先级', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'SMSEventHeadler', N'短信状态', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'SendStatus', N'发送状态', 0)
INSERT [dbo].[SYS_CodeMstr] ([Code], [Desc1], [Type]) VALUES (N'IssueStatus', N'异常状态', 0)