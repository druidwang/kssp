while exists(select MAX(ID) from ACC_Permission group by Code, Category having count(1) > 1)
begin
	delete from ACC_PermissionGroupPermission where PermissionId in
	(
		select MAX(ID) from ACC_Permission group by Code, Category having count(1) > 1
	)

	delete from ACC_UserPermission where PermissionId in
	(
		select MAX(ID) from ACC_Permission group by Code, Category having count(1) > 1
	)

	delete from ACC_RolePermission where PermissionId in
	(
		select MAX(ID) from ACC_Permission group by Code, Category having count(1) > 1
	)

	delete from ACC_Permission where Id in 
	(
		select MAX(ID) from ACC_Permission group by Code, Category having count(1) > 1
	)
end
go

DROP INDEX [IX_ACC_Permission_Code_Category] ON [dbo].[ACC_Permission]
GO

/****** Object:  Index [IX_ACC_Permission_Code_Category]    Script Date: 2015/12/18 15:12:17 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ACC_Permission_Code_Category] ON [dbo].[ACC_Permission]
(
	[Code] ASC,
	[Category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO