alter table WMS_ShipPlan add [Priority] [tinyint] NOT NULL
go

insert into sys_menu values('Url_PickTask_New','�����������','Url_PickTask',200,'�߼��ֿ�-����-�������-����','~/PickTask/New','~/Content/Images/Nav/Default.png',1);
insert into sys_menu values('Url_PickTask_Assign','����','Url_PickTask',300,'�߼��ֿ�-����-�������-����','~/PickTask/Assign','~/Content/Images/Nav/Default.png',1);
insert into sys_menu values('Url_RepackTask_Assign','����','Url_RepackTask',200,'�߼��ֿ�-����-��������-����','~/RepackTask/Assign','~/Content/Images/Nav/Default.png',1);
insert into sys_menu values('Url_ShipPlan_Ship','����','Url_ShipPlan',300,'�߼��ֿ�-����-��������-����','~/ShipPlan/Ship','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(code,desc1,category,Sequence) values('Url_PickTask_New','�����������','WMS',12000)
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_PickTask_Assign','���ɼ������','WMS',13000)
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_RepackTask_Assign','���ɷ�������','WMS',14000)