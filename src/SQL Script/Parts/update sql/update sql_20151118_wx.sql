select * from SYS_Menu where Code like '%WMS%'

insert into SYS_MENU values('Menu.WMS','�߼��ֿ�',null,'3000','�߼��ֿ�',null,'~/Content/Images/Nav/Trans.png',1)
insert into SYS_MENU values('Url_WMS_Info','��Ϣ','Menu.WMS','200','��Ϣ',null,'~/Content/Images/Nav/Info.png',1)
insert into SYS_MENU values('Url_WMS_Setup','����','Menu.WMS','300','��Ϣ',null,'~/Content/Images/Nav/Setup.png',1)
insert into SYS_MENU values('Url_WMS_Trans','����','Menu.WMS','100','��Ϣ',null,'~/Content/Images/Nav/Trans.png',1)


insert into SYS_Menu values('Url_ShipPlan_View','��������','Url_WMS_Info',500,'�߼��ֿ�-����-��������','~/ShipPlan/View','~/Content/Images/Nav/Default.png',1)


insert into ACC_PermissionCategory values('WMS','�߼��ֿ�',0,10)

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_ShipPlan_View','��������鿴','WMS',10000)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_ShipPlan_Ship','�������񷢻�','WMS',11000)