insert into SYS_Menu values('Url_FacilityMaster_View','̨��','Url_FMS_Trans',100,'��ʩ����-����-̨��','~/FacilityMaster/Index','~/Content/Images/Nav/Default.png',1)


insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_FacilityMaster_View','̨�˲鿴','FMS',12000)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_FacilityMaster_Edit','̨�˱༭','FMS',13000)

insert into SYS_CodeMstr values('FacilityStatus','��ʩ״̬',0)
insert into SYS_CodeDet values('FacilityStatus',0,'CodeDetail_FacilityStatus_Create',1,1)
insert into SYS_CodeDet values('FacilityStatus',1,'CodeDetail_FacilityStatus_Idle',0,2)
insert into SYS_CodeDet values('FacilityStatus',2,'CodeDetail_FacilityStatus_InUse',0,3)
insert into SYS_CodeDet values('FacilityStatus',3,'CodeDetail_FacilityStatus_Maintaining',0,4)
insert into SYS_CodeDet values('FacilityStatus',4,'CodeDetail_FacilityStatus_Inspecting',0,5)
insert into SYS_CodeDet values('FacilityStatus',5,'CodeDetail_FacilityStatus_Failure',0,6)
insert into SYS_CodeDet values('FacilityStatus',6,'CodeDetail_FacilityStatus_Repairing',0,7)


insert into SYS_Menu values('Url_FacilityTrans_View','��ʩ��־','Url_FMS_Info',100,'��ʩ����-��Ϣ-��ʩ��־','~/FacilityTrans/Index','~/Content/Images/Nav/Default.png',1)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_FacilityTrans_View','��ʩ��־�鿴','FMS',14000)


insert into SYS_Menu values('Url_MaintainPlan_View','Ԥ������','Url_FMS_Setup',200,'��ʩ����-����-Ԥ������','~/MaintainPlan/Index','~/Content/Images/Nav/Default.png',1)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_MaintainPlan_View','Ԥ�����Բ鿴','FMS',15000)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_MaintainPlan_Edit','Ԥ�����Ա༭','FMS',16000)


insert into SYS_CodeMstr values('MaintainPlanType','Ԥ����������',0)
insert into SYS_CodeDet values('MaintainPlanType',0,'CodeDetail_MaintainPlanType_Once',1,1)
insert into SYS_CodeDet values('MaintainPlanType',1,'CodeDetail_MaintainPlanType_Year',0,2)
insert into SYS_CodeDet values('MaintainPlanType',2,'CodeDetail_MaintainPlanType_Month',0,3)
insert into SYS_CodeDet values('MaintainPlanType',3,'CodeDetail_MaintainPlanType_Week',0,4)
insert into SYS_CodeDet values('MaintainPlanType',4,'CodeDetail_MaintainPlanType_Day',0,5)
insert into SYS_CodeDet values('MaintainPlanType',5,'CodeDetail_MaintainPlanType_Hour',0,6)
insert into SYS_CodeDet values('MaintainPlanType',6,'CodeDetail_MaintainPlanType_Minute',0,7)
insert into SYS_CodeDet values('MaintainPlanType',7,'CodeDetail_MaintainPlanType_Second',0,8)