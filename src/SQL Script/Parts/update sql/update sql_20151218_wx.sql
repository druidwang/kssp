alter table WMS_DeliveryBarCode add IsPickHu bit;
go

insert into sys_menu values('Url_DeliveryBarCode','���ͱ�ǩ','Url_WMS_Trans',400,'�߼��ֿ�-����-���ͱ�ǩ',null,'~/Content/Images/Nav/Default.png',1)
insert into sys_menu values('Url_DeliveryBarCode_View','��ѯ','Url_DeliveryBarCode',100,'�߼��ֿ�-����-���ͱ�ǩ-��ѯ','~/DeliveryBarCode/View','~/Content/Images/Nav/Default.png',1);
insert into sys_menu values('Url_DeliveryBarCode_New','��ӡ','Url_DeliveryBarCode',200,'�߼��ֿ�-����-���ͱ�ǩ-��ӡ','~/DeliveryBarCode/New','~/Content/Images/Nav/Default.png',1);
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_DeliveryBarCode_View','���ͱ�ǩ�鿴','WMS',15000);
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_DeliveryBarCode_New','���ͱ�ǩ��ӡ','WMS',16000);
go