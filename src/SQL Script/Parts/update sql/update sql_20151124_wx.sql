insert into SYS_Menu values('Url_PickTask_View','拣货任务','Url_WMS_Trans',100,'高级仓库-事务-拣货任务','~/PickTask/Index','~/Content/Images/Nav/Default.png',1)
insert into SYS_Menu values('Url_PickResult_View','拣货结果','Url_WMS_Info',100,'高级仓库-事务-拣货结果','~/PickResult/Index','~/Content/Images/Nav/Default.png',1)


insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_PickTask_View','拣货任务查看','WMS',16000)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_PickResult_View','拣货结果查看','WMS',17000)
