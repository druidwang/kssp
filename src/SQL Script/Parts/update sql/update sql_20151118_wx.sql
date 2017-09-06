select * from SYS_Menu where Code like '%WMS%'

insert into SYS_MENU values('Menu.WMS','高级仓库',null,'3000','高级仓库',null,'~/Content/Images/Nav/Trans.png',1)
insert into SYS_MENU values('Url_WMS_Info','信息','Menu.WMS','200','信息',null,'~/Content/Images/Nav/Info.png',1)
insert into SYS_MENU values('Url_WMS_Setup','设置','Menu.WMS','300','信息',null,'~/Content/Images/Nav/Setup.png',1)
insert into SYS_MENU values('Url_WMS_Trans','事务','Menu.WMS','100','信息',null,'~/Content/Images/Nav/Trans.png',1)


insert into SYS_Menu values('Url_ShipPlan_View','发货任务','Url_WMS_Info',500,'高级仓库-事务-发货任务','~/ShipPlan/View','~/Content/Images/Nav/Default.png',1)


insert into ACC_PermissionCategory values('WMS','高级仓库',0,10)

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_ShipPlan_View','发货任务查看','WMS',10000)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_ShipPlan_Ship','发货任务发货','WMS',11000)