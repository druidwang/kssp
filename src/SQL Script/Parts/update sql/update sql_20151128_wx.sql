insert into SYS_Menu values('Url_PickTask','�������','Url_WMS_Trans',100,'�߼��ֿ�-����-�������',null,'~/Content/Images/Nav/Default.png',1)
insert into SYS_Menu values('Url_RepackTask','��������','Url_WMS_Trans',200,'�߼��ֿ�-����-��������',null,'~/Content/Images/Nav/Default.png',1)
insert into SYS_Menu values('Url_ShipPlan','��������','Url_WMS_Trans',300,'�߼��ֿ�-����-��������',null,'~/Content/Images/Nav/Default.png',1)


update SYS_Menu set Name='��ѯ' ,seq=100,parent='Url_RepackTask',desc1='�߼��ֿ�-����-��������-��ѯ' where code='Url_RepackTask_View'
update SYS_Menu set Name='��ѯ' ,seq=100,parent='Url_PickTask',desc1='�߼��ֿ�-����-�������-��ѯ' where code='Url_PickTask_View'
update SYS_Menu set name='��ѯ' ,seq=100,parent='Url_ShipPlan',desc1='�߼��ֿ�-����-��������-��ѯ'  where code='Url_ShipPlan_View'

update SYS_Menu set name='�������',desc1='�߼��ֿ�-����-�������' where code='Url_RepackResult_View'
update SYS_Menu set name='��������',desc1='�߼��ֿ�-����-��������' where code='Url_RepackTask'
