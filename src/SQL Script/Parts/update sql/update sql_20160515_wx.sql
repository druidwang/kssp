insert into SYS_Menu values('Url_MesScanControlPoint_View','������Ϣ','Url_FMS_Info',300,'��ʩ����-��Ϣ-������Ϣ','~/MesScanControlPoint/Index','~/Content/Images/Nav/Default.png',1)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_MesScanControlPoint_View','������Ϣ�鿴','FMS',19000)


insert into SYS_Menu values('Url_FacilityParamater_View','�豸����','Url_FMS_Info',400,'��ʩ����-��Ϣ-�豸����','~/FacilityParamater/Index','~/Content/Images/Nav/Default.png',1)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_FacilityParamater_View','������Ϣ�鿴','FMS',20000)


insert into sys_codemstr values('FacilityParamaterType','�豸��������',0);
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values('FacilityParamaterType',0,'������Ϣ',1,1);
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)  values('FacilityParamaterType',1,'�豸����',0,2);


insert into sys_codemstr values('FacilityOrderType','�豸����������',0);
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values('FacilityOrderType',0,'����',1,1);
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)  values('FacilityOrderType',1,'ά��',0,2);
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)  values('FacilityOrderType',2,'���',0,3);


insert into SYS_Menu values('Url_FacilityOrder_View','�豸������','Url_FMS_Trans',300,'��ʩ����-����-�豸������','~/FacilityOrder/Index','~/Content/Images/Nav/Default.png',1)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_FacilityOrder_View','�������鿴','FMS',21000)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_FacilityOrder_Edit','�������༭','FMS',22000)
