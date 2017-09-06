USE [sconit5_test]
GO
Truncate table ACC_User
GO
SET IDENTITY_INSERT [dbo].[ACC_User] ON
INSERT [dbo].[ACC_User] ([Id], [Code], [Password], [FirstName], [LastName], [Type], [Email], [TelPhone], [MobilePhone], [Language], [IsActive], [AccountExpired], [AccountLocked], [PasswordExpired], [CreateUser], [CreateUserNm], [CreateDate], [LastModifyUser], [LastModifyUserNm], [LastModifyDate], [Version]) VALUES (1, N'su', N'E10ADC3949BA59ABBE56E057F20F883E', N'超级', N'用户', 0, N'su@sconit.com', NULL, NULL, N'zh-CN', 1, 0, 0, 0, 1, N'超级用户', CAST(0x00009E5E00000000 AS DateTime), 1, N'用户 超级', CAST(0x00009F8D0125B380 AS DateTime), 4)
INSERT [dbo].[ACC_User] ([Id], [Code], [Password], [FirstName], [LastName], [Type], [Email], [TelPhone], [MobilePhone], [Language], [IsActive], [AccountExpired], [AccountLocked], [PasswordExpired], [CreateUser], [CreateUserNm], [CreateDate], [LastModifyUser], [LastModifyUserNm], [LastModifyDate], [Version]) VALUES (2, N'monitor', N'FE01CE2A7FBAC8FAFAED7C982A04E229', N'后台', N'服务', 0, N'monitor@sconit.com', NULL, NULL, N'zh-CN', 1, 0, 0, 0, 1, N'超级用户', CAST(0x00009E5E00000000 AS DateTime), 1, N'用户 超级', CAST(0x00009F8601742A9C AS DateTime), 4)
SET IDENTITY_INSERT [dbo].[ACC_User] OFF
GO