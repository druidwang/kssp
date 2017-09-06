USE [sconit5_test]
GO
Truncate table ACC_PermissionGroup
GO
SET IDENTITY_INSERT [dbo].[ACC_PermissionGroup] ON
INSERT [dbo].[ACC_PermissionGroup] ([Id], [Code], [Desc1], [CreateUser], [CreateUserNm], [CreateDate], [LastModifyUser], [LastModifyUserNm], [LastModifyDate]) VALUES (5, N'Op', N'操作员', 1, N'用户 超级', CAST(0x00009F8700C2AFD8 AS DateTime), 1, N'用户 超级', CAST(0x00009F8D00C4A8EC AS DateTime))
INSERT [dbo].[ACC_PermissionGroup] ([Id], [Code], [Desc1], [CreateUser], [CreateUserNm], [CreateDate], [LastModifyUser], [LastModifyUserNm], [LastModifyDate]) VALUES (8, N'Admin', N'ddd', 1, N'用户 超级', CAST(0x00009F8A00D0D9A0 AS DateTime), 1, N'用户 超级', CAST(0x00009F8D00CAA814 AS DateTime))
INSERT [dbo].[ACC_PermissionGroup] ([Id], [Code], [Desc1], [CreateUser], [CreateUserNm], [CreateDate], [LastModifyUser], [LastModifyUserNm], [LastModifyDate]) VALUES (10, N'yhthyth', N'thytht', 1, N'用户 超级', CAST(0x00009F8B01160700 AS DateTime), 1, N'用户 超级', CAST(0x00009F8B01160700 AS DateTime))
INSERT [dbo].[ACC_PermissionGroup] ([Id], [Code], [Desc1], [CreateUser], [CreateUserNm], [CreateDate], [LastModifyUser], [LastModifyUserNm], [LastModifyDate]) VALUES (11, N'ddd', N'adgfa', 1, N'用户 超级', CAST(0x00009F8C015CC8AC AS DateTime), 1, N'用户 超级', CAST(0x00009F8C015CC8AC AS DateTime))
SET IDENTITY_INSERT [dbo].[ACC_PermissionGroup] OFF
GO