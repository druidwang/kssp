insert into SYS_Menu values('Url_FacilityCategory_View','设施类别','Url_FMS_Setup',100,'设施管理-设置-设施类别','~/FacilityCategory/Index','~/Content/Images/Nav/Default.png',1)


insert into ACC_PermissionCategory values('FMS','设施管理',0,130)

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_FacilityCategory_View','设施类别查看','FMS',10000)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_FacilityCategory_Edit','设施类别编辑','FMS',11000)