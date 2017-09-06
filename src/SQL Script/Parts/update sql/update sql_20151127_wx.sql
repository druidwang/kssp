insert into SYS_Menu values('Url_RepackTask_View','翻箱任务','Url_WMS_Trans',100,'高级仓库-事务-翻箱任务','~/RepackTask/Index','~/Content/Images/Nav/Default.png',1)
insert into SYS_Menu values('Url_RepackResult_View','翻箱结果','Url_WMS_Info',200,'高级仓库-信息-翻箱结果','~/RepackResult/Index','~/Content/Images/Nav/Default.png',1)


insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_RepackTask_View','翻箱任务查看','WMS',18000)
insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_RepackResult_View','翻箱结果查看','WMS',19000)
