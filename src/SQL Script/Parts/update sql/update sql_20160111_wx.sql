insert into SYS_MENU values('Url_PackingList','װ�䵥','Url_WMS_Trans','400','�߼��ֿ�-����-װ�䵥',null,'~/Content/Images/Nav/Default.png',1)
go

insert into SYS_MENU values('Url_PackingList_View','�鿴','Url_PackingList','100','�߼��ֿ�-����-װ�䵥-�鿴','~/PackingList/Index','~/Content/Images/Nav/Default.png',1)
go

insert into SYS_MENU values('Url_PackingList_New','װ��','Url_PackingList','200','�߼��ֿ�-����-װ�䵥-װ��','~/PackingList/New','~/Content/Images/Nav/Default.png',1)
go

insert into SYS_MENU values('Url_PackingList_Ship','����','Url_PackingList','300','�߼��ֿ�-����-װ�䵥-������','~/PackingList/Ship','~/Content/Images/Nav/Default.png',1)
go

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_PackingList_View','װ�䵥�鿴','WMS',21000)
go

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_PackingList_New','װ�䵥װ��','WMS',22000)
go

insert into ACC_Permission (Code,Desc1,category,Sequence) values('Url_PackingList_Ship','װ�䵥����','WMS',23000)
go