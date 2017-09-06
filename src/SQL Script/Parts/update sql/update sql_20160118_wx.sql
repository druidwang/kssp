insert into sys_menu values('Url_TransportBill','运输账单','Menu.TMS.Trans',200,'运输管理-事务-运单',null,'~/Content/Images/Nav/Default.png',1)
insert into sys_menu values('Url_TransportBill_View','查询','Url_TransportBill',100,'运输管理-事务-运输账单-查询','~/TransportBill/View','~/Content/Images/Nav/Default.png',1);
insert into sys_menu values('Url_TransportBill_New','新增','Url_TransportBill',200,'运输管理-事务-运输账单-新增','~/TransportBill/New','~/Content/Images/Nav/Default.png',1);
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_TransportBill_View','查看运输账单','WMS',22000);
insert into ACC_Permission(code,desc1,category,Sequence) values('Url_TransportBill_New','新增运输账单','WMS',23000);


