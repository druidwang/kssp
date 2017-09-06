USE [sconit5_test]
GO
Truncate table SYS_CodeDet
GO
SET IDENTITY_INSERT [dbo].[SYS_CodeDet] ON
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (1, N'UserType', N'0', N'CodeDetail_UserType_Normal', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (2, N'UserType', N'1', N'CodeDetail_UserType_System', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (3, N'Language', N'zh-CN', N'CodeDetail_Language_zh_CN', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (5, N'Language', N'en-US', N'CodeDetail_Language_en_US', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (11, N'AddressType', N'0', N'CodeDetail_Address_ShipAddress', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (12, N'AddressType', N'1', N'CodeDetail_Address_BillAddress', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (21, N'WorkingCalendarType', N'0', N'CodeDetail_WorkingCalendar_Work', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (22, N'WorkingCalendarType', N'1', N'CodeDetail_WorkingCalendar_Rest', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (23, N'DayOfWeek', N'0', N'CodeDetail_WorkingCalendar_Monday', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (25, N'DayOfWeek', N'1', N'CodeDetail_WorkingCalendar_Tuesday', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (26, N'DayOfWeek', N'2', N'CodeDetail_WorkingCalendar_Wednesday', 0, 3)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (27, N'DayOfWeek', N'3', N'CodeDetail_WorkingCalendar_Thursday', 0, 4)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (28, N'DayOfWeek', N'4', N'CodeDetail_WorkingCalendar_Friday', 0, 5)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (29, N'DayOfWeek', N'5', N'CodeDetail_WorkingCalendar_Saturday', 0, 6)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (30, N'DayOfWeek', N'6', N'CodeDetail_WorkingCalendar_Sunday', 0, 7)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (31, N'PermissionCategoryType', N'1', N'CodeDetail_PermissionCategoryType_Url', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (32, N'PermissionCategoryType', N'2', N'CodeDetail_PermissionCategoryType_Region', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (33, N'PermissionCategoryType', N'3', N'CodeDetail_PermissionCategoryType_Customer', 0, 3)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (34, N'PermissionCategoryType', N'4', N'CodeDetail_PermissionCategoryType_Supplier', 0, 4)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (35, N'LocationType', N'0', N'CodeDetail_LocationType_Normal', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (36, N'LocationType', N'1', N'CodeDetail_LocationType_Inspection', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Id], [Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (37, N'LocationType', N'2', N'CodeDetail_LocationType_Reject', 0, 3)
SET IDENTITY_INSERT [dbo].[SYS_CodeDet] OFF



INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderType', N'1', N'OrderType_Procurement', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderType', N'2', N'OrderType_Transfer', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderType', N'3', N'OrderType_Distribution', 0, 3)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderType', N'4', N'OrderType_Production', 0, 4)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderType', N'5', N'OrderType_Subconctracting', 0, 5)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderType', N'6', N'OrderType_CustomerGoods', 0, 6)           
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderPriority', N'0', N'OrderPriority_Normal', 0, 1)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderPriority', N'1', N'OrderPriority_Urgent', 0, 2)   
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderStatus', N'0', N'OrderStatus_Create', 1, 0)        
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderStatus', N'1', N'OrderStatus_Submit', 0, 1)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderStatus', N'2', N'OrderStatus_InProcess', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderStatus', N'3', N'OrderStatus_Pause', 0, 3)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderStatus', N'4', N'OrderStatus_Complete', 0, 4)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderStatus', N'5', N'OrderStatus_Close', 0, 5)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'OrderStatus', N'6', N'OrderStatus_Cancel', 0, 6)           


INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'LogLevel', N'DEBUG', N'DEBUG', 1, 1)  
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'LogLevel', N'INFO', N'INFO', 0, 2)  
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'LogLevel', N'WARN', N'WARN', 0, 3)  
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'LogLevel', N'ERROR', N'ERROR', 0, 4)  
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'LogLevel', N'FATAL', N'FATAL', 0, 5)  


INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssuePriority', N'0', N'CodeDetail_IssuePriority_Normal', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssuePriority', N'1', N'CodeDetail_IssuePriority_Urgent', 0, 2)   

INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssueType', N'0', N'CodeDetail_IssueType_Issue', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssueType', N'1', N'CodeDetail_IssueType_Improvement', 0, 2)  
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssueType', N'2', N'CodeDetail_IssueType_Changepoint', 0, 3)  

INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'SendStatus', N'0', N'CodeDetail_SendStatus_NotSend', 1, 1)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'SendStatus', N'1', N'CodeDetail_SendStatus_Success', 0, 2)  
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'SendStatus', N'2', N'CodeDetail_SendStatus_Fail', 0, 3)  


INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssueStatus', N'0', N'CodeDetail_IssueStatus_Create', 1, 0)        
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssueStatus', N'1', N'CodeDetail_IssueStatus_Submit', 0, 1)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssueStatus', N'2', N'CodeDetail_IssueStatus_InProcess', 0, 2)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssueStatus', N'3', N'CodeDetail_IssueStatus_Complete', 0, 3)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssueStatus', N'4', N'CodeDetail_IssueStatus_Close', 0, 4)
INSERT [dbo].[SYS_CodeDet] ([Code], [Value], [Desc1], [IsDefault], [Seq]) VALUES (N'IssueStatus', N'5', N'CodeDetail_IssueStatus_Cancel', 0, 5)   
