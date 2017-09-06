ALTER TABLE INP_InspectResult ADD WMSResNo varchar(50);
ALTER TABLE INP_InspectResult ADD WMSResSeq varchar(50);

ALTER TABLE dbo.INV_ItemExchange ADD WMSNo varchar(50);
ALTER TABLE dbo.INV_ItemExchange ADD WMSSeq varchar(50);

--Wms_WmsCallLes_CancleExchangeMaterialInventory_Error

update SI_LogToUser set Descritpion ='WMS调用LES服务,LOC创建检验单失败' where Id ='11109'
update SI_LogToUser set Descritpion ='WMS调用LES服务,LOC质检判断失败' where Id ='11111'
update SI_LogToUser set Descritpion ='WMS调用LES服务,LOC移库失败' where Id ='11112'

update sys_menu set PageUrl ='~/SAPSupplier/Index' where Code ='Url_SI_SAP_Supplier_View'
insert into sys_menu values('Url_SI_SAP_Trans_View','移动类型','Url_SI_SAP','333','移动类型','~/SAPTrans/Index','~/Content/Images/Nav/Default.png','1')
insert into ACC_Permission values ('Url_SI_SAP_Trans_View','移动类型','SI')
insert into ACC_Permission values ('Url_SI_View','SI','SI')

insert into sys_menu values('Url_SI_View','SI',null,'1','SI',null,'~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS','WMS',null,'500','WMS',null,'~/Content/Images/Nav/Default.png','1')
go;
insert into sys_menu values('Url_SI_EntityPreference_View','企业选项','Url_SI','354','企业选项','~/SIEntityPreference/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_LogToUser_View','日志用户','Url_SI','355','日志用户','~/SILogToUser/Index','~/Content/Images/Nav/Default.png','1')

insert into sys_menu values('Url_SI_SAP_InvLoc_View','事务关系','Url_SI_SAP','356','事务关系','~/SAPInvLoc/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_MapMoveTypeTCode_View','Tcode','Url_SI_SAP','357','Tcode','~/SAPMapMoveTypeTCode/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProcOrder_View','采购单','Url_SI_SAP','358','采购单','~/SAPProcOrder/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProcOrderDetail_View','采购单明细','Url_SI_SAP','359','采购单明细','~/SAPProcOrderDetail/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProdOrder_View','生产单','Url_SI_SAP','360','生产单','~/SAPProdOrder/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProdOrderBomDet_View','生产单BOM','Url_SI_SAP','361','生产单BOM','~/SAPProdOrderBomDet/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProdSeqReport_View','生产报工','Url_SI_SAP','362','生产报工','~/SAPProdSeqReport/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_Quota_View','配额','Url_SI_SAP','363','配额','~/SAPQuota/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_SourceOrder_View','货源清单','Url_SI_SAP','364','货源清单','~/SAPSourceOrder/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_TableIndex_View','Table Index','Url_SI_SAP','365','Table Index','~/SAPTableIndex/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_TransCallBack_View','移动结果','Url_SI_SAP','366','移动结果','~/SAPTransCallBack/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_ConcessionMaterial_View','让步使用','Url_SI_WMS','367','让步使用','~/WMSConcessionMaterial/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_ExchangeMaterial_View','换料明细','Url_SI_WMS','368','换料明细','~/WMSExchangeMaterial/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_IpDetailInput_View','发货明细','Url_SI_WMS','369','发货明细','~/WMSIpDetailInput/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_MiscOrderDetail_View','计划外出入库','Url_SI_WMS','370','计划外出入库','~/WMSMiscOrderDetail/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_ReceiptDetailInput_View','收货明细','Url_SI_WMS','371','收货明细','~/WMSReceiptDetailInput/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_RejectMaterial_View','不合格明细','Url_SI_WMS','372','不合格明细','~/WMSRejectMaterial/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_ReturnOrderDetailInput_View','退货明细','Url_SI_WMS','373','退货明细','~/WMSReturnOrderDetailInput/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_MessageQueue_View','消息队列','Url_SI_WMS','374','消息队列','~/WMSMessageQueue/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_MessageQueueLog_View','消息队列日志','Url_SI_WMS','375','消息队列日志','~/WMSMessageQueueLog/Index','~/Content/Images/Nav/Default.png','1')

insert into ACC_Permission values ('Url_SI_EntityPreference_View','企业选项','SI')
insert into ACC_Permission values ('Url_SI_LogToUser_View','日志用户','SI')
insert into ACC_Permission values ('Url_SI_SAP_InvLoc_View','事务关系','SI')
insert into ACC_Permission values ('Url_SI_SAP_MapMoveTypeTCode_View','Tcode','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProcOrder_View','采购单','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProcOrderDetail_View','采购单明细','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProdOrder_View','生产单','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProdOrderBomDet_View','生产单BOM','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProdSeqReport_View','生产报工','SI')
insert into ACC_Permission values ('Url_SI_SAP_Quota_View','配额','SI')
insert into ACC_Permission values ('Url_SI_SAP_SourceOrder_View','货源清单','SI')
insert into ACC_Permission values ('Url_SI_SAP_TableIndex_View','Table Index','SI')
insert into ACC_Permission values ('Url_SI_SAP_TransCallBack_View','移动结果','SI')
insert into ACC_Permission values ('Url_SI_WMS_ConcessionMaterial_View','让步使用','SI')
insert into ACC_Permission values ('Url_SI_WMS_ExchangeMaterial_View','换料明细','SI')
insert into ACC_Permission values ('Url_SI_WMS_IpDetailInput_View','发货明细','SI')
insert into ACC_Permission values ('Url_SI_WMS_MiscOrderDetail_View','计划外出入库','SI')
insert into ACC_Permission values ('Url_SI_WMS_ReceiptDetailInput_View','收货明细','SI')
insert into ACC_Permission values ('Url_SI_WMS_RejectMaterial_View','不合格明细','SI')
insert into ACC_Permission values ('Url_SI_WMS_ReturnOrderDetailInput_View','退货明细','SI')
insert into ACC_Permission values ('Url_SI_WMS_MessageQueue_View','消息队列','SI')
insert into ACC_Permission values ('Url_SI_WMS_MessageQueueLog_View','消息队列日志','SI')


----------===数据初始化
insert SI_EntityPreference values('MaxRowSize','5000','显示条目数')
insert SI_EntityPreference values('Les2WmsBatch','10','LES->WMS批量')

Truncate table SI_EntityPreference
insert SI_EntityPreference values('NumberControlLength','10','WMS号长度')
insert SI_EntityPreference values('SAPServicePassword','12345678','SAP Service Password')
insert SI_EntityPreference values('SAPServiceTimeOut','600000','SAP Service Time Out(毫秒)')
insert SI_EntityPreference values('SAPServiceUserName','dms_wangjun','SAP Service User Name')
insert SI_EntityPreference values('SAPItemCategory','FG','导入的默认物料组')
insert SI_EntityPreference values('SAPTransLes2SiBatch','10','LES->SI批量')
insert SI_EntityPreference values('SAPTranSi2SapBatch','10','SI->SAP批量')
insert SI_EntityPreference values('WMSAnjiLoc','1000','默认安吉库位')
insert SI_EntityPreference values('WMSPlant','0084','默认工厂')
insert SI_EntityPreference values('WMSMiscSQLoc','0085','默认双桥工厂')
insert SI_EntityPreference values('WMSAnJiRegion','1000','默认安吉区域')
insert SI_EntityPreference values('WMSServicePassword','password','WMS Web服务密码')
insert SI_EntityPreference values('WMSServiceTimeOut','100000','WMS Web服务超时时间')
insert SI_EntityPreference values('WMSServiceUserName','User','WMS Web服务用户名')
insert SI_EntityPreference values('WMSServiceReCallTimes','3','WMS Web服务失败后重调次数')
insert SI_EntityPreference values('SAPServiceReCallTimes','3','SAP Web服务失败后重调次数')
insert SI_EntityPreference values('MaxRowSize','5000','显示条目数')

Truncate table SI_LogToUser
insert dbo.SI_LogToUser values('10101','连接SAPWebService中Item信息失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10102','物料信息从SAP导入到SI失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10103','LES新增了新的物料','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10104','物料信息从SI导出到LES失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10201','连接SAPWebService中Supplier信息失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10202','供应商信息从SAP导入到SI失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10203','LES新增了新的供应商','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10204','供应商信息从SI导出到LES失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10301','导入生产单web服务不能连接','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10302','导入生产单数据转换失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10303','导入生产单插入中间表失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10304','创建生产单','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10401','导入生产单BOMweb服务不能连接','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10402','导入生产单BOM数据转换失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10403','导入生产单BOM插入中间表失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10404','查找整车生产单失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10405','生成整车生产单失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10501','导入采购单web服务不能连接','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10502','导入采购单数据转换失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10503','导入采购单插入中间表失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10504','创建采购单','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10510','创建计划协议web服务不能连接','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10601','导入配额web服务不能连接','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10602','配额插入中间表失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10603','配额导出到LES失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10604','新的配额','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10605','配额变更','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10606','禁用路线失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10701','导入移动类型连接Web服务失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10702','移动类型导入到中间表失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10703','导入移动类型读取LES数据失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10704','更新移动类型中间表失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10705','导入移动类型记录表之间关系失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10706','导入移动类型记录SAP返回结果失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10707','导入盘点移动类型中失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10801','报工web服务不能连接','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10802','报工失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10803','报工插入中间表失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11101','WMS调用LES服务,LOC送货单进行收货失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11102','WMS调用LES服务,LOC冲销对供应商送货单的收货记录失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11103','WMS调用LES服务,LOC向Buff备件公司发货失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11104','WMS调用LES服务,LOC向Buff备件公司发货冲销失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11105','WMS调用LES服务,LOC退货失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11106','WMS调用LES服务,LOC退货冲销失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11107','WMS调用LES服务,LOC计划外出入库失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11108','WMS调用LES服务,LOC计划外出入库冲销','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11109','WMS调用LES服务,LOC进行盘点调整','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11110','WMS调用LES服务,LOC换料失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11111','WMS调用LES服务,LOC质检不合格失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11112','WMS调用LES服务,LOC让步使用失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11113','WMS调用LES服务,LOC创建条码失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11201','LES调用WMS服务,供应商双桥Buff向安吉送货创建送货单失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11202','LES调用WMS服务,供应商双桥Buff向安吉送货创建送货单冲销失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11203','LES调用WMS服务,Buff向LOC拉动物料向备件公司配送物料创建要货单失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11204','LES调用WMS服务,Buff向LOC拉动物料向备件公司配送物料创建要货单失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11205','LES调用WMS服务,Buff向LOC拉动物料创建排序单失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11206','LES调用WMS服务,Buff向LOC拉动物料创建排序单冲销失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11207','LES调用WMS服务,Buff接收LOC的送货单,创建收货单失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11208','LES调用WMS服务,Buff接收LOC的送货单,创建收货单冲销失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11209','LES调用WMS服务,LES中创建的条码导入WMS失败','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('20101','精益引擎创建订单失败','liqiuyun@sconit.com',null);
---===========================



select e.BatchNo, d.FRBNR,d.SGTXT,d.MSTXT,d.CreateDate as SapCreateDate,
a.MiscOrderNo,a.Item,a.Uom,a.HuId,a.LotNo,a.IsCS,a.CSSupplier,a.PlanBill,a.ActBill,a.QualityType, 
a.Qty,b.Type,b.Status,b.MoveType,b.CancelMoveType,b.CostCenter,b.Asn,b.RefNo,
b.Region,b.Location,b.DeliverRegion as ReceiveRegion,b.RecLoc,b.WBS,e.*
from Sconit5_Test.dbo.ORD_MiscOrderLocationDet a
left join Sconit5_Test.dbo.ORD_MiscOrderMstr b on a.MiscOrderNo = b.MiscOrderNo
left join Sconit5_SI_Test.dbo.SI_SAP_InvLoc c on c.SourceId = a.Id
left join Sconit5_SI_Test.dbo.SI_SAP_InvTrans e on (e.FRBNR = c.FRBNR and e.SGTXT = c.SGTXT)
left join Sconit5_SI_Test.dbo.SI_SAP_TransCallBack d on (d.FRBNR = c.FRBNR and d.SGTXT = c.SGTXT)
where d.Id>0 and c.SourceType = 1 order by d.Id desc


select e.BatchNo, d.*,
a.OrderNo,a.Item,a.Uom,a.IsCS,a.PlanBill,a.ActBill,a.QualityType, 
a.Qty,b.Type,b.Status,a.TransType,b.ExtOrderNo,
a.PartyFrom,a.LocFrom,a.PartyTo,b.LocTo,e.*
from Sconit5_Test.dbo.VIEW_LocTrans a
left join Sconit5_Test.dbo.ORD_OrderMstr b on a.OrderNo = b.OrderNo
left join Sconit5_SI_Test.dbo.SI_SAP_InvLoc c on c.SourceId = a.Id
left join Sconit5_SI_Test.dbo.SI_SAP_InvTrans e on (e.FRBNR = c.FRBNR and e.SGTXT = c.SGTXT)
left join Sconit5_SI_Test.dbo.SI_SAP_TransCallBack d on (d.FRBNR = c.FRBNR and d.SGTXT = c.SGTXT)
where d.Id>0 and c.SourceType = 0 order by d.Id desc 


select c.*,a.*,b.* from Sconit5_SI_Test.dbo.SI_SAP_InvLoc c
left join Sconit5_SI_Test.dbo.SI_SAP_InvTrans a on (a.FRBNR = c.FRBNR  and a.SGTXT = c.SGTXT)
left join Sconit5_SI_Test.dbo.SI_SAP_TransCallBack b on (b.FRBNR = c.FRBNR  and b.SGTXT = c.SGTXT)
where c.SourceType = 0
order by c.FRBNR

EXEC   sp_rename   'SI_SAP_ProdOrderBomDet.[WORKCENTER]','WORK_CENTER','COLUMN' 

Insert into SI_EntityPreference values('SAPItemCategory', 'FG', '零件默认分类');

Insert into SI_EntityPreference values('NumberControlLength', '10', 'WMS号长度');

-------20120419
ALTER TABLE SI_SAP_InvTrans DROP COLUMN IsOutbound;
ALTER TABLE SI_SAP_InvTrans DROP COLUMN OutBoundDate;
ALTER TABLE SI_SAP_InvTrans ADD LastModifyDate datetime;
ALTER TABLE SI_SAP_InvTrans ADD [Status] tinyint;
ALTER TABLE SI_SAP_InvTrans ADD ErrorCount int;
GO
update SI_SAP_InvTrans set LastModifyDate = GETDATE();
update SI_SAP_InvTrans set [Status] = 0;
update SI_SAP_InvTrans set ErrorCount = 0;
GO
ALTER TABLE SI_SAP_InvTrans ALTER column LastModifyDate datetime not null;
ALTER TABLE SI_SAP_InvTrans ALTER column [Status] tinyint not null;
ALTER TABLE SI_SAP_InvTrans ALTER column ErrorCount int not null;


insert into ACC_PermissionCategory values('SI','SI',1)
insert into ACC_Permission values ('Url_SI_SAP_Item_View','SAP物料','SI')
insert into sys_menu values('Url_SI_SAP','SAP',null,'30','SAP',null,'~/Content/Images/Nav/Quality.png','1')
insert into sys_menu values('Url_SI_SAP_Item_View','SAP物料','Url_SI_SAP','331','SAP物料','~/SAPItem/Index','~/Content/Images/Nav/Default.png','1')




/****** Object:  Table [dbo].[SYS_NumCtrl]    Script Date: 04/12/2012 13:28:55 ******/				
SET ANSI_NULLS ON				
GO				
				
SET QUOTED_IDENTIFIER ON				
GO				
				
SET ANSI_PADDING ON				
GO				
				
CREATE TABLE [dbo].[SYS_NumCtrl](				
	[Code] [varchar](50) NOT NULL,			
	[IntValue] [int] NULL,			
	[StrValue] [varchar](50) NULL,			
 CONSTRAINT [PK_SYS_NumCtrl] PRIMARY KEY CLUSTERED 				
(				
	[Code] ASC			
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]				
) ON [PRIMARY]				
				
GO				
				
SET ANSI_PADDING OFF				
GO			
		


/****** Object:  StoredProcedure [dbo].[USP_GetNextSequence]    Script Date: 04/12/2012 13:30:21 ******/				
SET ANSI_NULLS ON				
GO				
				
SET QUOTED_IDENTIFIER ON				
GO				
				
CREATE PROCEDURE [dbo].[USP_GetNextSequence]				
	@CodePrefix varchar(50),			
	@NextSequence int OUTPUT			
AS				
Begin Tran				
	Declare @invValue int;			
	select  @invValue = IntValue FROM SYS_NumCtrl WITH (UPDLOCK, ROWLOCK) where Code = @CodePrefix;			
	if @invValue is null			
	begin			
		if @NextSequence is not null		
		begin 		
			insert into SYS_NumCtrl(Code, IntValue) values (@CodePrefix, @NextSequence + 1);	
		end		
		else		
		begin		
			set @NextSequence = 1;	
			insert into SYS_NumCtrl(Code, IntValue) values (@CodePrefix, 2);	
		end		
	end 			
	else			
	begin			
		if @NextSequence is not null		
		begin 		
			if @invValue <= @NextSequence	
			begin	
				update SYS_NumCtrl set IntValue = @NextSequence + 1 where Code = @CodePrefix;
			end	
		end		
		else		
		begin		
			set @NextSequence = @invValue;	
			update SYS_NumCtrl set IntValue = @NextSequence + 1 where Code = @CodePrefix;	
		end		
	end			
Commit tran				
GO				

