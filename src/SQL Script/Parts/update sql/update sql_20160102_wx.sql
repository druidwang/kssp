update sys_menu set PageUrl='~/RepackTask/Assign' where code='Url_RepackTask_Assign'
go

insert into SYS_MENU values('Url_ShipPlan_Assign','����','Url_ShipPlan','200','�߼��ֿ�-����-��������-����','~/ShipPlan/Assign','~/Content/Images/Nav/Default.png',1)
go

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_ShipPlan_Assign','�����������','WMS',19000)
go