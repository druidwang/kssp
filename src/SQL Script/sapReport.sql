return
insert into ACC_PermissionCategory values('Menu_SI','SI',0,200)
go
insert into ACC_Permission values('Url_SI_SAPInterface','SI-�ֹ�����SAP�ӿ�','Menu_SI',100)
go
insert into SYS_Menu values('Url_SI_SAPInterface','�ֹ�����SAP�ӿ�','Url_SI_View',1010000100,'SI-�ֹ�����SAP�ӿ�','~/SAPInterface/Index','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPTransferLog_View','SI-SAP�ӿ���־����','Menu_SI',200)
go
insert into SYS_Menu values('Url_SAPTransferLog_View','SAP�ӿ���־����','Url_SI_View',1010000200,'SI-SAP�ӿ���־����','~/SAPInterface/SAPTransferLogIndex','~/Content/Images/Nav/Default.png',1)
go
--
insert into SYS_Menu values('Url_SAPReport','SAP�ӿڱ���','Url_SI_View',1010000300,'SI-SAP�ӿڱ���',null,'~/Content/Images/Nav/Default.png',1)
go
insert into SYS_Menu values('Url_SAPItem_View','����','Url_SAPReport',1010000400,'SI-SAP�ӿڱ���-����','~/SAPInterface/SAPItemIndex','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPItem_View','SAP�ӿڱ���-����','Menu_SI',400)
go
insert into SYS_Menu values('Url_SAPBom_View','Bom','Url_SAPReport',1010000500,'SI-SAP�ӿڱ���-Bom','~/SAPInterface/SAPBomIndex','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPBom_View','SAP�ӿڱ���-Bom','Menu_SI',500)
go
insert into SYS_Menu values('Url_SAPUomConvertion_View','��λת��','Url_SAPReport',1010000600,'SI-SAP�ӿڱ���-��λת��','~/SAPInterface/SAPUomConvertionIndex','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPUomConvertion_View','SAP�ӿڱ���-��λת��','Menu_SI',600)
go
insert into SYS_Menu values('Url_SAPPriceList_View','�ɹ��۸�','Url_SAPReport',1010000700,'SI-SAP�ӿڱ���-�ɹ��۸�','~/SAPInterface/SAPPriceListIndex','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPPriceList_View','SAP�ӿڱ���-�ɹ��۸�','Menu_SI',700)
go
insert into SYS_Menu values('Url_SAPSupplier_View','��Ӧ��','Url_SAPReport',1010000800,'SI-SAP�ӿڱ���-��Ӧ��','~/SAPInterface/SAPSupplierIndex','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPSupplier_View','SAP�ӿڱ���-��Ӧ��','Menu_SI',800)
go
insert into SYS_Menu values('Url_SAPCustomer_View','�ͻ�','Url_SAPReport',1010000900,'SI-SAP�ӿڱ���-�ͻ�','~/SAPInterface/SAPCustomerIndex','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPCustomer_View','SAP�ӿڱ���-�ͻ�','Menu_SI',900)
go
insert into SYS_Menu values('Url_SAPSDNormal_View','����','Url_SAPReport',1010001000,'SI-SAP�ӿڱ���-����','~/SAPInterface/SAPSDNormalIndex','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPSDNormal_View','SAP�ӿڱ���-����','Menu_SI',1000)
go
insert into SYS_Menu values('Url_SAPSDCancel_View','���۳���','Url_SAPReport',1010001100,'SI-SAP�ӿڱ���-���۳���','~/SAPInterface/SAPSDCancelIndex','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPSDCancel_View','SAP�ӿڱ���-���۳���','Menu_SI',1100)

--Delete SYS_Menu where code in ('Url_SAPSDNormal_View','Url_SAPSDCancel_View')
--Delete ACC_Permission where code in ('Url_SAPSDNormal_View','Url_SAPSDCancel_View')
----SAP-PPMES Menu

Select * from sys_menu where Desc1 like '%SAP�ӿڱ���%'
Order by Seq 
 
--insert into SYS_Menu values('Url_SAPPPMES0001_View','�����ջ�','Url_SAPReport',1010001200,'SI-SAP�ӿڱ���-�����ջ�','~/SAPInterface/SAPPPMES0001Index','~/Content/Images/Nav/Default.png',1)
--go
--insert into ACC_Permission values('Url_SAPPPMES0001_View','SAP�ӿڱ���-�����ջ�','Menu_SI',1010001200)

-- insert into SYS_Menu values('Url_SAPPPMES0002_View','�����ջ�����','Url_SAPReport',1010001300,'SI-SAP�ӿڱ���-�����ջ�����','~/SAPInterface/SAPPPMES0002Index','~/Content/Images/Nav/Default.png',1)
--go
--insert into ACC_Permission values('Url_SAPPPMES0002_View','SAP�ӿڱ���-�����ջ�����','Menu_SI',1010001300)

--insert into SYS_Menu values('Url_SAPPPMES0003_View','������Ʒ����','Url_SAPReport',1010001400,'SI-SAP�ӿڱ���-������Ʒ����','~/SAPInterface/SAPPPMES0003Index','~/Content/Images/Nav/Default.png',1)
--go
--insert into ACC_Permission values('Url_SAPPPMES0003_View','SAP�ӿڱ���-������Ʒ����','Menu_SI',1010001400)


--insert into SYS_Menu values('Url_SAPPPMES0004_View','��������','Url_SAPReport',1010001500,'SI-SAP�ӿڱ���-��������','~/SAPInterface/SAPPPMES0004Index','~/Content/Images/Nav/Default.png',1)
--go
--insert into ACC_Permission values('Url_SAPPPMES0004_View','SAP�ӿڱ���-��������','Menu_SI',1010001500)

--insert into SYS_Menu values('Url_SAPPPMES0005_View','��������','Url_SAPReport',1010001600,'SI-SAP�ӿڱ���-��������','~/SAPInterface/SAPPPMES0005Index','~/Content/Images/Nav/Default.png',1)
--go
--insert into ACC_Permission values('Url_SAPPPMES0005_View','SAP�ӿڱ���-��������','Menu_SI',1010001600)

--insert into SYS_Menu values('Url_SAPPPMES0006_View','��������','Url_SAPReport',1010001700,'SI-SAP�ӿڱ���-��������','~/SAPInterface/SAPPPMES0006Index','~/Content/Images/Nav/Default.png',1)
--go
--insert into ACC_Permission values('Url_SAPPPMES0006_View','SAP�ӿڱ���-��������','Menu_SI',1010001700)

 
 insert into SYS_Menu values('Url_SAPMMMES0001_View','MM�ɹ�ҵ��','Url_SAPReport',1010001800,'SI-SAP�ӿڱ���-MM�ɹ�ҵ��','~/SAPInterface/SAPMMMES0001Index','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPMMMES0001_View','SAP�ӿڱ���-MM�ɹ�ҵ��','Menu_SI',1010001800)
  insert into SYS_Menu values('Url_SAPMMMES0002_View','MM�ɹ�ҵ�����','Url_SAPReport',1010001900,'SI-SAP�ӿڱ���-MM�ɹ�ҵ�����','~/SAPInterface/SAPMMMES0002Index','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPMMMES0002_View','SAP�ӿڱ���-MM�ɹ�ҵ�����','Menu_SI',1010001900)

   insert into SYS_Menu values('Url_SAPSTMES0001_View','MM����ƶ�','Url_SAPReport',1010002000,'SI-SAP�ӿڱ���-MM����ƶ�','~/SAPInterface/SAPSTMES0001Index','~/Content/Images/Nav/Default.png',1)
go
insert into ACC_Permission values('Url_SAPSTMES0001_View','SAP�ӿڱ���-MM����ƶ�','Menu_SI',1010002000)



 
--insert into ACC_Permission values('Url_SI_SAPBuInterface','SI-SAPҵ��ӿڵ���(������)','Menu_SI',150)
--go
--insert into SYS_Menu values('Url_SI_SAPFunInterface','SAPҵ��ӿڵ���(������)','Url_SI_View',1010000150,'SI-SAPҵ��ӿڵ���(������)','~/SAPInterface/Index1','~/Content/Images/Nav/Default.png',1)
--go
 
 