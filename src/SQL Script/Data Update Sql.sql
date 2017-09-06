
update SCM_FlowMstr set HuTemplate ='BarCodeMI2D.xls' where HuTemplate='BarCodeMI.xls'
update SCM_FlowMstr set HuTemplate ='BarCodeEX2D.xls' where HuTemplate='BarCodeEX.xls'
update SCM_FlowMstr set HuTemplate ='BarCodeFI2D.xls' where HuTemplate='BarCodeFI.xls'
update SCM_FlowMstr set HuTemplate ='BarCodePurchase2D.xls' where HuTemplate='BarCodePurchase.xls'

update SYS_CodeDet set Value ='BarCodeMI2D.xls' where Value='BarCodeMI.xls'
update SYS_CodeDet set Value ='BarCodeEX2D.xls' where Value='BarCodeEX.xls'
update SYS_CodeDet set Value ='BarCodeFI2D.xls' where Value='BarCodeFI.xls'
update SYS_CodeDet set Value ='BarCodePurchase2D.xls' where Value='BarCodePurchase.xls'

delete from SYS_CodeDet where Id ='770'


update MRP_MrpExSectionPlan set ProductType ='A' where ProductType='10' or ProductType = '0' or ProductType is null
update MRP_MrpExSectionPlan set ProductType ='Z' where ProductType='260'
update MRP_MrpExSectionPlan set ProductType ='R' where ProductType='180'
update MRP_MrpExSectionPlan set ProductType ='J' where ProductType='100'
update MRP_MrpExSectionPlan set ProductType ='D' where ProductType='40'
update MRP_MrpExSectionPlan set ProductType ='B' where ProductType='20'
update MRP_MrpExSectionPlan set ProductType ='Q' where ProductType='170'
update MRP_MrpExSectionPlan set ProductType ='C' where ProductType='30'
update MRP_MrpExSectionPlan set ProductType ='P' where ProductType='160'


update MRP_ProdLineEx set ProductType ='A' where ProductType='10' or ProductType = '0' or ProductType is null
update MRP_SnapProdLineEx set ProductType ='A' where ProductType='10' or ProductType = '0' or ProductType is null
update MRP_ProdLineExInstance set ProductType ='A' where ProductType='10' or ProductType = '0' or ProductType is null



insert into acc_permission values('Url_Mrp_RccpPlanEx_AllotSection','Url_Mrp_RccpPlanEx_AllotSection','Menu_MRP',310202111)

insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_Mrp_RccpPlanEx_AllotSection','主辅线分配','Menu.MRP.Trans.Rccp.Ex',310202111,'主辅线分配','~/RccpPlanEx/AllotSection','~/Content/Images/Nav/Default.png',1 


insert into SYS_CodeDet values('HolidayType',10,'CodeDetail_HolidayType_Holiday',1,10)
insert into SYS_CodeDet values('HolidayType',20,'CodeDetail_HolidayType_Trial',1,20)
insert into SYS_CodeDet values('HolidayType',30,'CodeDetail_HolidayType_HaltTime',1,30)

insert SYS_CodeMstr values('HolidayType','休息类型',0)

update BAT_Job set Name='AutoCloseOrderJob',Desc1='AutoCloseOrderJob',ServiceType='AutoCloseOrderJob' where Id=6

insert into BAT_Job values('GetMasterDataJob','GetMasterDataJob','GetMasterDataJob')
insert into BAT_Job values('GetProcOrderJob','GetProcOrderJob','GetProcOrderJob')
insert into BAT_Job values('GetProductOrderJob','GetProductOrderJob','GetProductOrderJob')
insert into BAT_Job values('GetVanOrderJob','GetVanOrderJob','GetVanOrderJob')

insert into BAT_JobParam values(9,'UserCode','su')
insert into BAT_JobParam values(10,'UserCode','su')
insert into BAT_JobParam values(11,'UserCode','su')
insert into BAT_JobParam values(12,'UserCode','su')


insert into BAT_Trigger values(6,'AutoCloseOrder','订单自动关闭',GETDATE(),GETDATE(),0,1,2,0,0)
insert into BAT_Trigger values(9,'GetMasterData','导入SAP基础数据:物料,供应商,配额',GETDATE(),GETDATE(),0,1,2,0,0)
insert into BAT_Trigger values(10,'GetProcOrder','导入SAP采购单',GETDATE(),GETDATE(),0,1,2,0,0)
insert into BAT_Trigger values(11,'GetProductOrder','导入SAP生产单',GETDATE(),GETDATE(),0,1,2,0,0)
insert into BAT_Trigger values(12,'GetVanOrder','导入SAP整车生产单',GETDATE(),GETDATE(),0,1,2,0,0)

insert into BAT_TriggerParam values(14,'Plant','0084');
insert into BAT_TriggerParam values(15,'Plant','0084');
insert into BAT_TriggerParam values(16,'DayDiff','0');
insert into BAT_TriggerParam values(17,'DayDiff','1');


insert into BAT_Job values('WMSJob','LesCallWmsService','WMSJob')
insert into BAT_JobParam values(8,'UserCode','su')
insert into BAT_Trigger values(8,'WMSJob','LesCallWmsService',getdate(),GETDATE(),0,1,2,1,0)

insert SYS_EntityPreference values(10017,17,'LOC','WMSAnjiRegion',1,'su',GETDATE(),1,'su',GETDATE())

insert CUST_MiscOrderMoveType values(0,'Z01','Z02','LOC/BUF/线边 盘亏/工废/Z01',0,0,1,0,0,0,0,0,0,0)
insert CUST_MiscOrderMoveType values(1,'Z02','Z01','LOC/BUF/线边 盘赢/Z02',0,0,1,0,0,0,0,0,0,0)
insert CUST_MiscOrderMoveType values(0,'Z75','Z76','双桥集散库循环盘点盘亏/Z75',0,0,1,0,0,0,0,0,0,0)
insert CUST_MiscOrderMoveType values(1,'Z76','Z75','双桥集散库循环盘点盘盈/Z76',0,0,1,0,0,0,0,0,0,0)

update CUST_MiscOrderMoveType set CheckCostCenter =1 where MoveType ='201' or MoveType ='251'

update SCM_FlowMstr set  BillTerm = 1 where TYPE=1 and BillTerm =0

delete from SYS_Menu where Name ='雇员'
update SYS_Menu set PageUrl = null where PageUrl ='~/Content/Images/Nav/Default.png'

update dbo.CUST_MiscOrderMoveType set checkqualityType=2 where ID = 9

delete from CUST_MiscOrderMoveType where ID = 3
-----------20120507 liqiuyun ------
update CUST_MiscOrderMoveType set CheckDeliverRegion = 1 where Id in(5,6)
update CUST_MiscOrderMoveType set Desc1 ='从冻结库存报废/555' where Id = 9

update SYS_SNRule set YearCode= null,MonthCode = null,DayCode = null,BlockSeq ='15'

-----------------

-------------------start 李秋云 20120322 供应商门户菜单 权限
delete from sys_menu where code in ('Menu.SupplierMenu.CustInventory','Menu.SupplierMenu.CustInventoryIOB','Menu.SupplierMenu.Inventory','Menu.SupplierMenu.InventoryIOB')

update sys_menu set code ='Url_SupplierPrintHu_View',name ='SupplierPrintHu',seq='110',pageurl='~/SupplierPrintHu/Index',IsActive='1' where code ='Menu.SupplierMenu.SupplierHuPrint'
update sys_menu set code ='Url_SupplierSchedule_View',name ='SupplierSchedule',seq='120',pageurl='~/SupplierSchedule/Index',IsActive='1' where code ='Menu.SupplierMenu.SupplierSchedule'
update sys_menu set code ='Url_SupplierOrder_View',name ='SupplierOrder',seq='130',pageurl='~/SupplierOrder/Index',IsActive='1' where code ='Menu.SupplierMenu.ViewProcurementOrder'
update sys_menu set code ='Url_SupplierOrderIssue_View',name ='SupplierOrderIssue',seq='140',pageurl='~/SupplierOrderIssue/ShipIndex',IsActive='1' where code ='Menu.SupplierMenu.OrderIssue'
update sys_menu set code ='Url_SupplierIpMaster_View',name ='SupplierIpMaster',seq='150',pageurl='~/SupplierIpMaster/Index',IsActive='1' where code ='Menu.SupplierMenu.ASN'
update sys_menu set code ='Url_SupplierReceipt_View',name ='SupplierReceipt',seq='160',pageurl='~/SupplierReceipt/Index',IsActive='1' where code ='Menu.SupplierMenu.ReceiptNotes'
update sys_menu set code ='Url_SupplierPlanBill_View',name ='SupplierPlanBill',seq='170',pageurl='~/SupplierPlanBill/Index',IsActive='0' where code ='Menu.SupplierMenu.POPlanBill'
update sys_menu set code ='Url_SupplierActBill_View',name ='SupplierActBill',seq='180',pageurl='~/SupplierActBill/Index',IsActive='0' where code ='Menu.SupplierMenu.ActingBill'
update sys_menu set code ='Url_SupplierBill_View',name ='SupplierBill',seq='190',pageurl='~/SupplierBill/Index',IsActive='0' where code ='Menu.SupplierMenu.POBill'


delete from  acc_permission where code in  ('Menu.SupplierMenu.CustInventory','Menu.SupplierMenu.CustInventoryIOB','Menu.SupplierMenu.Inventory','Menu.SupplierMenu.InventoryIOB')

update acc_permission set code='Url_SupplierPrintHu_View' where code ='Menu.SupplierMenu.SupplierHuPrint'
update acc_permission set code='Url_SupplierSchedule_View' where code ='Menu.SupplierMenu.SupplierSchedule'
update acc_permission set code='Url_SupplierOrder_View' where code ='Menu.SupplierMenu.ViewProcurementOrder'
update acc_permission set code='Url_SupplierOrderIssue_View' where code ='Menu.SupplierMenu.OrderIssue'
update acc_permission set code='Url_SupplierIpMaster_View' where code ='Menu.SupplierMenu.ASN'
update acc_permission set code='Url_SupplierReceipt_View' where code ='Menu.SupplierMenu.ReceiptNotes'
update acc_permission set code='Url_SupplierPlanBill_View' where code ='Menu.SupplierMenu.POPlanBill'
update acc_permission set code='Url_SupplierActBill_View' where code ='Menu.SupplierMenu.ActingBill'
update acc_permission set code='Url_SupplierBill_View' where code ='Menu.SupplierMenu.POBill'

---------------end 李秋云 20120322


insert into acc_permission values('Url_OrderMstr_Production_New','Url_OrderMstr_Production_New','Production')
insert into acc_permission values('Url_OrderMstr_Production_View','Url_OrderMstr_Production_View','Production')
insert into acc_permission values('Url_OrderMstr_Production_Edit','Url_OrderMstr_Production_Edit','Production')
insert into acc_permission values('Url_OrderMstr_Production_Delete','Url_OrderMstr_Production_Delete','Production')
insert into acc_permission values('Url_OrderMstr_Production_Submit','Url_OrderMstr_Production_Submit','Production')
insert into acc_permission values('Url_OrderMstr_Production_Complete','Url_OrderMstr_Production_Complete','Production')
insert into acc_permission values('Url_OrderMstr_Production_Cancel','Url_OrderMstr_Production_Cancel','Production')

-- 不合格品相关  
insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_RejectOrder','Quality_RejectOrder','Menu.Quality',385,'不合格品处理单',NULL,'~/Content/Images/Nav/Default.png',1 union
select 'Url_RejectOrder_New','Quality_RejectOrder_New','Url_RejectOrder',1,'新建不合格品处理单','~/RejectOrder/New','~/Content/Images/Nav/Default.png',1 union
select 'Url_RejectOrder_View','Quality_RejectOrder_View','Url_RejectOrder',2,'查询不合格品处理单','~/RejectOrder/Index','~/Content/Images/Nav/Default.png',1

insert into ACC_Permission(Code,Desc1,Category)
values('Url_RejectOrder_New','Url_RejectOrder_New','Quality')

insert into ACC_Permission(Code,Desc1,Category)
values('Url_RejectOrder_View','Url_RejectOrder_View','Quality')

insert into SYS_CodeMstr(Code,Desc1,Type)
select'RejectMasterStatus','不合格品处理单状态',0 union
select'HandleResult','处理结果',0

insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)
select 'RejectMasterStatus','0','CodeDetail_RejectMasterStatus_Create',1,1 union
select 'RejectMasterStatus','1','CodeDetail_RejectMasterStatus_InProcess',0,2 union
select 'RejectMasterStatus','2','CodeDetail_RejectMasterStatus_Close',0,3 union
select 'HandleResult','0','CodeDetail_HandleResult_Return',1,1 union
select 'HandleResult','1','CodeDetail_HandleResult_Concession',0,2 union
select 'HandleResult','2','CodeDetail_HandleResult_Rework',0,3 union
select 'HandleResult','3','CodeDetail_HandleResult_Scrap',0,4 union
select 'HandleResult','4','CodeDetail_HandleResult_Disassembly',0,5 

--容器相关
insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_Container_View','Application_Container','Menu.MasterData',38,'容器','~/Container/Index','~/Content/Images/Nav/Default.png',1 

insert into ACC_Permission(Code,Desc1,Category)
select 'Url_Container_Edit','Url_Container_Edit','Application' union
select 'Url_Container_View','Url_Container_View','Application' union
select 'Url_Container_Delete','Url_Container_Delete','Application'

insert into SYS_CodeMstr(Code,Desc1,Type)
select'InventoryType','容器类型',0 

insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)
select 'InventoryType','0','CodeDetail_InventoryType_Quality',1,1 union
select 'InventoryType','1','CodeDetail_InventoryType_Barcode',0,2


--工艺流程相关
insert into SYS_CodeMstr(Code,Desc1,Type)
select'RoutingTimeUnit','节拍单位',0 

insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)
select 'RoutingTimeUnit','1','CodeDetail_RoutingTimeUnit_Second',1,1 union
select 'RoutingTimeUnit','2','CodeDetail_RoutingTimeUnit_Minute',0,2 union
select 'RoutingTimeUnit','3','CodeDetail_RoutingTimeUnit_Hour',0,3 union
select 'RoutingTimeUnit','4','CodeDetail_RoutingTimeUnit_Day',0,4 


----BillTerm
delete from SYS_CodeDet where Code ='BillTerm'

insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)
select 'BillTerm','0','CodeDetail_BillTerm_NA',1,0 union
select 'BillTerm','1','CodeDetail_BillTerm_BAR',0,1 union
select 'BillTerm','2','CodeDetail_BillTerm_BAB',0,2 union
select 'BillTerm','3','CodeDetail_BillTerm_BAC',0,3 union
select 'BillTerm','4','CodeDetail_BillTerm_BAI',0,4 union
select 'BillTerm','5','CodeDetail_BillTerm_BBB',0,5 

----圆整系数
insert into SYS_CodeMstr(Code,Desc1,Type)
select'RoundUpOption','圆整系数',0 

insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)
select 'RoundUpOption','1','CodeDetail_RoundUpOption_ToUp',1,2 union
select 'RoundUpOption','0','CodeDetail_RoundUpOption_None',0,1 union
select 'RoundUpOption','2','CodeDetail_RoundUpOption_ToDown',0,3

----FlowStrategy
delete from SYS_CodeDet where Code ='FlowStrategy'

insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)
select 'FlowStrategy','0','CodeDetail_FlowStrategy_NA',1,0 union
select 'FlowStrategy','1','CodeDetail_FlowStrategy_Manual',0,1 union
select 'FlowStrategy','2','CodeDetail_FlowStrategy_KB',0,2 union
select 'FlowStrategy','3','CodeDetail_FlowStrategy_JIT',0,3 union
select 'FlowStrategy','4','CodeDetail_FlowStrategy_SEQ',0,4 union
select 'FlowStrategy','5','CodeDetail_FlowStrategy_KIT',0,5 union
select 'FlowStrategy','6','CodeDetail_FlowStrategy_MRP',0,6 union
select 'FlowStrategy','7','CodeDetail_FlowStrategy_ANDON',0,7 


------OrderSubType
insert into SYS_CodeMstr(Code,Desc1,Type)
select'OrderSubType','订单子类型',0 

insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)
select 'OrderSubType','0','CodeDetail_OrderSubType_Normal',1,1 union
select 'OrderSubType','1','CodeDetail_OrderSubType_Return',0,2 


-- 不合格品修改
delete from ACC_Permission where Code='Url_RejectManage_New'
or Code='Url_RejectManage_View'

delete from SYS_Menu where Code='Url_RejectManage' 
or Code='Url_RejectManage_New' 
or Code='Url_RejectManage_View' 

insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_RejectOrder','Quality_RejectOrder','Menu.Quality',385,'不合格品处理单',NULL,'~/Content/Images/Nav/Default.png',1 union
select 'Url_RejectOrder_New','Quality_RejectOrder_New','Url_RejectOrder',1,'新建不合格品处理单','~/RejectOrder/New','~/Content/Images/Nav/Default.png',1 union
select 'Url_RejectOrder_View','Quality_RejectOrder_View','Url_RejectOrder',2,'查询不合格品处理单','~/RejectOrder/Index','~/Content/Images/Nav/Default.png',1

insert into ACC_Permission(Code,Desc1,Category)
values('Url_RejectOrder_New','Url_RejectOrder_New','Quality')

insert into ACC_Permission(Code,Desc1,Category)
values('Url_RejectOrder_View','Url_RejectOrder_View','Quality')

--拣货单
insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_PickList_View','Distribution_PickList','Menu.Distribution.Trans',204,'拣货单','~/PickList/Index','~/Content/Images/Nav/Default.png',1 

insert into ACC_Permission(Code,Desc1,Category)
values('Url_PickList_View','Url_PickList_View','Distribution')

---送货单
insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_DistributionIpMaster','Distribution_IpMaster','Menu.Distribution.Trans',205,'送货单',NULL,'~/Content/Images/Nav/Default.png',1 union
select 'Url_DistributionIpMaster_View','Distribution_IpMaster_View','Url_DistributionIpMaster',1,'送货单查询','~/DistributionIpMaster/Index','~/Content/Images/Nav/Default.png',1

insert into ACC_Permission(Code,Desc1,Category)
values('Url_DistributionIpMaster','Url_DistributionIpMaster','Distribution')

insert into ACC_Permission(Code,Desc1,Category)
values('Url_DistributionIpMaster_View','Url_DistributionIpMaster_View','Distribution')


---收发差异
insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_ASNGap','Distribution_ASNGap','Menu.Distribution.Trans',206,'收发差异',NULL,'~/Content/Images/Nav/Default.png',1 union
select 'Url_ASNGap_View','Distribution_ASNGap_View','Url_ASNGap',1,'收发差异查询','~/ASNGap/Index','~/Content/Images/Nav/Default.png',1

insert into ACC_Permission(Code,Desc1,Category)
values('Url_ASNGap','Url_ASNGap','Distribution')

insert into ACC_Permission(Code,Desc1,Category)
values('Url_ASNGap_View','Url_ASNGap_View','Distribution')

--发货
--insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
--select 'Url_IpMaster_New','Distribution_IpMaster_New','Url_IpMaster',2,'发货','~/IpMaster/Index','~/Content/Images/Nav/Default.png',1

--insert into ACC_Permission(Code,Desc1,Category)
--values('Url_IpMaster_New','Url_IpMaster_New','Distribution')

--拣货状态
insert into SYS_CodeMstr(Code,Desc1,Type)
select'PickListStatus','拣货状态',0 

insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq)
select 'PickListStatus','0','CodeDetail_PickListStatus_Create',1,1 union
select 'PickListStatus','1','CodeDetail_PickListStatus_Submit',0,2 union
select 'PickListStatus','2','CodeDetail_PickListStatus_InProcess',0,3 union
select 'PickListStatus','3','CodeDetail_PickListStatus_Close',0,4 union
select 'PickListStatus','4','CodeDetail_PickListStatus_Cancel',0,5 


---菜单 权限 相关修改
---收货
update SYS_Menu set Code='Url_ProcurementReceipt_View',Name='Procurement_Receipt',Desc1='收货单',PageUrl='~/ProcurementReceipt/Index'
where Code='Url_ProcurementGoodsReceipt_View'

update ACC_Permission set Code='Url_ProcurementReceipt_View',Desc1='Url_ProcurementReceipt_View'
where Code='Url_ProcurementGoodsReceipt_View'

delete from ACC_Permission
where Code='Url_ProcurementGoodsReceipt_Edit'

delete from ACC_Permission 
where Code='Url_ProcurementGoodsReceipt_Delete'

---发货
update SYS_Menu set Code='Url_ProcurementIpMaster_View',Name='Procurement_IpMaster',Desc1='发货单',PageUrl='~/ProcurementIpMaster/Index'
where Code='Url_ProcurementOrderIssue_View'

update ACC_Permission set Code='Url_ProcurementIpMaster_View',Desc1='Url_ProcurementIpMaster_View'
where Code='Url_ProcurementOrderIssue_View'

insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive) values
('Url_OrderMstr_Distribution_Ship','Distribution_OrderMstr_Ship','Url_OrderMstr_Distribution',198,'发货','~/DistributionOrder/Ship','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(Code,Desc1,Category)
values('Url_OrderMstr_Distribution_Ship','Url_DistributionOrderMaster_Ship','Distribution')

--20120116
update sys_menu set pageurl='~/DistributionOrder/ShipIndex' where code='Url_OrderMstr_Distribution_Ship'



insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive) values
('Url_OrderMstr_Procurement_Receive','Procurement_OrderMstr_Receive','Url_OrderMstr_Procurement',198,'收货','~/ProcurementOrder/ReceiveIndex','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(Code,Desc1,Category)
values('Url_OrderMstr_Procurement_Receive','Url_ProcurementOrderMaster_Receive','Procurement')


insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_PickList','Distribution_PickList','Menu.Distribution.Trans',
205,'拣货单','','~/Content/Images/Nav/Default.png',1 

insert into ACC_Permission(Code,Desc1,Category)
values('Url_PickList','Url_PickList','Distribution')

update sys_menu set parent='Url_PickList',desc1='查询',name='Distribution_PickList_View' where code='Url_PickList_View'

insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_PickList_New','Distribution_PickList_New','Url_PickList',
207,'新增','~/PickList/NewIndex','~/Content/Images/Nav/Default.png',1 

insert into ACC_Permission(Code,Desc1,Category)
values('Url_PickList_New','Url_PickList_New','Distribution')

--20120203
insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_ProcurementIpMaster','Procurement_IpMaster','Menu.Procurement.Trans',202,'送货单',NULL,'~/Content/Images/Nav/Default.png',1

insert into ACC_Permission(Code,Desc1,Category)
values('Url_ProcurementIpMaster','Url_ProcurementIpMaster','Procurement')

update sys_menu set name='Procurement_IpMaster_View', desc1='送货单查询',parent='Url_ProcurementIpMaster' where code='Url_ProcurementIpMaster_View'

insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_ProcurementIpMaster_Receive','Procurement_IpMaster_Receive','Url_ProcurementIpMaster',222,'收货','~/ProcurementIpMaster/ReceiveIndex','~/Content/Images/Nav/Default.png',1

insert into ACC_Permission(Code,Desc1,Category)
values('Url_ProcurementIpMaster_Receive','Url_ProcurementIpMaster_Receive','Procurement')



---不合格品处理单的相关名称修改（2012-02-06）
delete from ACC_Permission where Code='Url_RejectManage_New'
or Code='Url_RejectManage_View'

delete from SYS_Menu where Code='Url_RejectManage' 
or Code='Url_RejectManage_New' 
or Code='Url_RejectManage_View' 

insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_RejectOrder','Quality_RejectOrder','Menu.Quality',385,'不合格品处理单',NULL,'~/Content/Images/Nav/Default.png',1 union
select 'Url_RejectOrder_New','Quality_RejectOrder_New','Url_RejectOrder',1,'新建不合格品处理单','~/RejectOrder/ChooseInspectResultInNew','~/Content/Images/Nav/Default.png',1 union
select 'Url_RejectOrder_View','Quality_RejectOrder_View','Url_RejectOrder',2,'查询不合格品处理单','~/RejectOrder/Index','~/Content/Images/Nav/Default.png',1

insert into ACC_Permission(Code,Desc1,Category)
values('Url_RejectOrder_New','Url_RejectOrder_New','Quality')

insert into ACC_Permission(Code,Desc1,Category)
values('Url_RejectOrder_View','Url_RejectOrder_View','Quality')

----2012.02.12
insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive)
select 'Url_ProcurementIpMaster_Cancel','Procurement_IpMaster_Cancel','Url_ProcurementIpMaster',223,'取消','~/ProcurementIpMaster/CancelIndex','~/Content/Images/Nav/Default.png',1

insert into ACC_Permission(Code,Desc1,Category)
values('Url_ProcurementIpMaster_Cancel','Url_ProcurementIpMaster_Cancel','Procurement')


insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive) values
('Url_OrderMstr_Production_Receive','Production_OrderMstr_Receive','Url_OrderMstr_Production',198,'收货','~/ProductionOrder/ReceiveIndex','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(Code,Desc1,Category)
values('Url_OrderMstr_Production_Receive','Url_OrderMstr_Production_Receive','Distribution')

insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive) values
('Url_OrderMstr_Production_MaterialIn','Production_OrderMstr_MaterialIn','Url_OrderMstr_Production',200,'工单投料','~/ProductionOrder/MaterialInIndex','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(Code,Desc1,Category)
values('Url_OrderMstr_Production_MaterialIn','Url_OrderMstr_Production_MaterialIn','Production')


insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive) values
('Url_ProductionLine_MaterialIn','Production_ProductionLine_MaterialIn','Menu.Production.Trans',30,'生产线投料','~/ProductionLine/MaterialInIndex','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(Code,Desc1,Category)
values('Url_ProductionLine_MaterialIn','Url_ProductionLine_MaterialIn','Production')


insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive) values
('Url_Inventory_Hu','Inventory_Hu','Menu.Inventory.Trans',10,'条码',null,'~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(Code,Desc1,Category)
values('Url_Inventory_Hu','Url_Inventory_Hu','Inventory')

insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive) values
('Url_Inventory_Hu_View','Inventory_Hu_View','Url_Inventory_Hu',10,'查询','~/Hu/Index','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(Code,Desc1,Category)
values('Url_Inventory_Hu_View','Url_Inventory_Hu_View','Inventory')

insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive) values
('Url_Inventory_Hu_New','Inventory_Hu_New','Url_Inventory_Hu',20,'新建','~/Hu/New','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(Code,Desc1,Category)
values('Url_Inventory_Hu_New','Url_Inventory_Hu_New','Inventory')


insert into SYS_Menu(Code,Name,Parent,Seq,Desc1,PageUrl,ImageUrl,IsActive) values
('Url_OrderMstr_Production_ForceMaterialIn','Production_OrderMstr_ForceMaterialIn','Url_OrderMstr_Production',202,'工单强制投料','~/ProductionOrder/ForceMaterialInIndex','~/Content/Images/Nav/Default.png',1)

insert into ACC_Permission(Code,Desc1,Category)
values('Url_OrderMstr_Production_ForceMaterialIn','Url_OrderMstr_Production_ForceMaterialIn','Production')













