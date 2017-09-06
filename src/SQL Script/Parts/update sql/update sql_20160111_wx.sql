insert into SYS_MENU values('Url_PackingList','装箱单','Url_WMS_Trans','400','高级仓库-事务-装箱单',null,'~/Content/Images/Nav/Default.png',1)
go

insert into SYS_MENU values('Url_PackingList_View','查看','Url_PackingList','100','高级仓库-事务-装箱单-查看','~/PackingList/Index','~/Content/Images/Nav/Default.png',1)
go

insert into SYS_MENU values('Url_PackingList_New','装箱','Url_PackingList','200','高级仓库-事务-装箱单-装箱','~/PackingList/New','~/Content/Images/Nav/Default.png',1)
go

insert into SYS_MENU values('Url_PackingList_Ship','发货','Url_PackingList','300','高级仓库-事务-装箱单-发货箱','~/PackingList/Ship','~/Content/Images/Nav/Default.png',1)
go

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_PackingList_View','装箱单查看','WMS',21000)
go

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_PackingList_New','装箱单装箱','WMS',22000)
go

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_PackingList_Ship','装箱单发货','WMS',23000)
go