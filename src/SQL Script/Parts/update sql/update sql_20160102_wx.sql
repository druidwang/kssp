update sys_menu set PageUrl='~/RepackTask/Assign' where code='Url_RepackTask_Assign'
go

insert into SYS_MENU values('Url_ShipPlan_Assign','分派','Url_ShipPlan','200','高级仓库-事务-发货任务-分派','~/ShipPlan/Assign','~/Content/Images/Nav/Default.png',1)
go

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_ShipPlan_Assign','发货任务分派','WMS',19000)
go