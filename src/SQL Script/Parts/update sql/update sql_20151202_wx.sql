alter table WMS_ShipPlan add [Priority] [tinyint] NOT NULL
go

insert into sys_menu values('Url_PickTask_New','创建拣货任务','Url_PickTask',200,'高级仓库-事务-拣货任务-创建','~/PickTask/New','~/Content/Images/Nav/Default.png',1);
insert into sys_menu values('Url_PickTask_Assign','分派','Url_PickTask',300,'高级仓库-事务-拣货任务-分派','~/PickTask/Assign','~/Content/Images/Nav/Default.png',1);
insert into sys_menu values('Url_RepackTask_Assign','分派','Url_RepackTask',200,'高级仓库-事务-翻包任务-分派','~/RepackTask/Assign','~/Content/Images/Nav/Default.png',1);
insert into sys_menu values('Url_ShipPlan_Ship','发货','Url_ShipPlan',300,'高级仓库-事务-发货任务-发货','~/ShipPlan/Ship','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(code,desc1,category,Sequence) values('Url_PickTask_New','创建拣货任务','WMS',12000)
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_PickTask_Assign','分派拣货任务','WMS',13000)
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_RepackTask_Assign','分派翻包任务','WMS',14000)