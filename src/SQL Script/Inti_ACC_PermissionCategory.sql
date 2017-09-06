USE [sconit5_test]
GO
Truncate table ACC_PermissionCategory
GO
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'Application', N'应用管理', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'Distribution', N'发货管理', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'Inventory', N'库存管理', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'MasterData', N'基础数据', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'MRP', N'计划管理', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'Procurement', N'供货管理', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'Production', N'生产管理', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'Quality', N'质量管理', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'Issue', N'异常管理', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'SupplierMenu', N'供应商管理', 1)
INSERT [dbo].[ACC_PermissionCategory] ([Code], [Desc1], [Type]) VALUES (N'Visualization', N'可视化', 1)
GO