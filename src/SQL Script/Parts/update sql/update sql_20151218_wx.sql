alter table WMS_DeliveryBarCode add IsPickHu bit;
go

insert into sys_menu values('Url_DeliveryBarCode','配送标签','Url_WMS_Trans',400,'高级仓库-事务-配送标签',null,'~/Content/Images/Nav/Default.png',1)
insert into sys_menu values('Url_DeliveryBarCode_View','查询','Url_DeliveryBarCode',100,'高级仓库-事务-配送标签-查询','~/DeliveryBarCode/View','~/Content/Images/Nav/Default.png',1);
insert into sys_menu values('Url_DeliveryBarCode_New','打印','Url_DeliveryBarCode',200,'高级仓库-事务-配送标签-打印','~/DeliveryBarCode/New','~/Content/Images/Nav/Default.png',1);
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_DeliveryBarCode_View','配送标签查看','WMS',15000);
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_DeliveryBarCode_New','配送标签打印','WMS',16000);
go