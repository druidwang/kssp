ALTER TABLE INP_InspectResult ADD WMSResNo varchar(50);
ALTER TABLE INP_InspectResult ADD WMSResSeq varchar(50);

ALTER TABLE dbo.INV_ItemExchange ADD WMSNo varchar(50);
ALTER TABLE dbo.INV_ItemExchange ADD WMSSeq varchar(50);

--Wms_WmsCallLes_CancleExchangeMaterialInventory_Error

update SI_LogToUser set Descritpion ='WMS����LES����,LOC�������鵥ʧ��' where Id ='11109'
update SI_LogToUser set Descritpion ='WMS����LES����,LOC�ʼ��ж�ʧ��' where Id ='11111'
update SI_LogToUser set Descritpion ='WMS����LES����,LOC�ƿ�ʧ��' where Id ='11112'

update sys_menu set PageUrl ='~/SAPSupplier/Index' where Code ='Url_SI_SAP_Supplier_View'
insert into sys_menu values('Url_SI_SAP_Trans_View','�ƶ�����','Url_SI_SAP','333','�ƶ�����','~/SAPTrans/Index','~/Content/Images/Nav/Default.png','1')
insert into ACC_Permission values ('Url_SI_SAP_Trans_View','�ƶ�����','SI')
insert into ACC_Permission values ('Url_SI_View','SI','SI')

insert into sys_menu values('Url_SI_View','SI',null,'1','SI',null,'~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS','WMS',null,'500','WMS',null,'~/Content/Images/Nav/Default.png','1')
go;
insert into sys_menu values('Url_SI_EntityPreference_View','��ҵѡ��','Url_SI','354','��ҵѡ��','~/SIEntityPreference/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_LogToUser_View','��־�û�','Url_SI','355','��־�û�','~/SILogToUser/Index','~/Content/Images/Nav/Default.png','1')

insert into sys_menu values('Url_SI_SAP_InvLoc_View','�����ϵ','Url_SI_SAP','356','�����ϵ','~/SAPInvLoc/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_MapMoveTypeTCode_View','Tcode','Url_SI_SAP','357','Tcode','~/SAPMapMoveTypeTCode/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProcOrder_View','�ɹ���','Url_SI_SAP','358','�ɹ���','~/SAPProcOrder/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProcOrderDetail_View','�ɹ�����ϸ','Url_SI_SAP','359','�ɹ�����ϸ','~/SAPProcOrderDetail/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProdOrder_View','������','Url_SI_SAP','360','������','~/SAPProdOrder/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProdOrderBomDet_View','������BOM','Url_SI_SAP','361','������BOM','~/SAPProdOrderBomDet/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_ProdSeqReport_View','��������','Url_SI_SAP','362','��������','~/SAPProdSeqReport/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_Quota_View','���','Url_SI_SAP','363','���','~/SAPQuota/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_SourceOrder_View','��Դ�嵥','Url_SI_SAP','364','��Դ�嵥','~/SAPSourceOrder/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_TableIndex_View','Table Index','Url_SI_SAP','365','Table Index','~/SAPTableIndex/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_SAP_TransCallBack_View','�ƶ����','Url_SI_SAP','366','�ƶ����','~/SAPTransCallBack/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_ConcessionMaterial_View','�ò�ʹ��','Url_SI_WMS','367','�ò�ʹ��','~/WMSConcessionMaterial/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_ExchangeMaterial_View','������ϸ','Url_SI_WMS','368','������ϸ','~/WMSExchangeMaterial/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_IpDetailInput_View','������ϸ','Url_SI_WMS','369','������ϸ','~/WMSIpDetailInput/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_MiscOrderDetail_View','�ƻ�������','Url_SI_WMS','370','�ƻ�������','~/WMSMiscOrderDetail/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_ReceiptDetailInput_View','�ջ���ϸ','Url_SI_WMS','371','�ջ���ϸ','~/WMSReceiptDetailInput/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_RejectMaterial_View','���ϸ���ϸ','Url_SI_WMS','372','���ϸ���ϸ','~/WMSRejectMaterial/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_ReturnOrderDetailInput_View','�˻���ϸ','Url_SI_WMS','373','�˻���ϸ','~/WMSReturnOrderDetailInput/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_MessageQueue_View','��Ϣ����','Url_SI_WMS','374','��Ϣ����','~/WMSMessageQueue/Index','~/Content/Images/Nav/Default.png','1')
insert into sys_menu values('Url_SI_WMS_MessageQueueLog_View','��Ϣ������־','Url_SI_WMS','375','��Ϣ������־','~/WMSMessageQueueLog/Index','~/Content/Images/Nav/Default.png','1')

insert into ACC_Permission values ('Url_SI_EntityPreference_View','��ҵѡ��','SI')
insert into ACC_Permission values ('Url_SI_LogToUser_View','��־�û�','SI')
insert into ACC_Permission values ('Url_SI_SAP_InvLoc_View','�����ϵ','SI')
insert into ACC_Permission values ('Url_SI_SAP_MapMoveTypeTCode_View','Tcode','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProcOrder_View','�ɹ���','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProcOrderDetail_View','�ɹ�����ϸ','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProdOrder_View','������','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProdOrderBomDet_View','������BOM','SI')
insert into ACC_Permission values ('Url_SI_SAP_ProdSeqReport_View','��������','SI')
insert into ACC_Permission values ('Url_SI_SAP_Quota_View','���','SI')
insert into ACC_Permission values ('Url_SI_SAP_SourceOrder_View','��Դ�嵥','SI')
insert into ACC_Permission values ('Url_SI_SAP_TableIndex_View','Table Index','SI')
insert into ACC_Permission values ('Url_SI_SAP_TransCallBack_View','�ƶ����','SI')
insert into ACC_Permission values ('Url_SI_WMS_ConcessionMaterial_View','�ò�ʹ��','SI')
insert into ACC_Permission values ('Url_SI_WMS_ExchangeMaterial_View','������ϸ','SI')
insert into ACC_Permission values ('Url_SI_WMS_IpDetailInput_View','������ϸ','SI')
insert into ACC_Permission values ('Url_SI_WMS_MiscOrderDetail_View','�ƻ�������','SI')
insert into ACC_Permission values ('Url_SI_WMS_ReceiptDetailInput_View','�ջ���ϸ','SI')
insert into ACC_Permission values ('Url_SI_WMS_RejectMaterial_View','���ϸ���ϸ','SI')
insert into ACC_Permission values ('Url_SI_WMS_ReturnOrderDetailInput_View','�˻���ϸ','SI')
insert into ACC_Permission values ('Url_SI_WMS_MessageQueue_View','��Ϣ����','SI')
insert into ACC_Permission values ('Url_SI_WMS_MessageQueueLog_View','��Ϣ������־','SI')


----------===���ݳ�ʼ��
insert SI_EntityPreference values('MaxRowSize','5000','��ʾ��Ŀ��')
insert SI_EntityPreference values('Les2WmsBatch','10','LES->WMS����')

Truncate table SI_EntityPreference
insert SI_EntityPreference values('NumberControlLength','10','WMS�ų���')
insert SI_EntityPreference values('SAPServicePassword','12345678','SAP Service Password')
insert SI_EntityPreference values('SAPServiceTimeOut','600000','SAP Service Time Out(����)')
insert SI_EntityPreference values('SAPServiceUserName','dms_wangjun','SAP Service User Name')
insert SI_EntityPreference values('SAPItemCategory','FG','�����Ĭ��������')
insert SI_EntityPreference values('SAPTransLes2SiBatch','10','LES->SI����')
insert SI_EntityPreference values('SAPTranSi2SapBatch','10','SI->SAP����')
insert SI_EntityPreference values('WMSAnjiLoc','1000','Ĭ�ϰ�����λ')
insert SI_EntityPreference values('WMSPlant','0084','Ĭ�Ϲ���')
insert SI_EntityPreference values('WMSMiscSQLoc','0085','Ĭ��˫�Ź���')
insert SI_EntityPreference values('WMSAnJiRegion','1000','Ĭ�ϰ�������')
insert SI_EntityPreference values('WMSServicePassword','password','WMS Web��������')
insert SI_EntityPreference values('WMSServiceTimeOut','100000','WMS Web����ʱʱ��')
insert SI_EntityPreference values('WMSServiceUserName','User','WMS Web�����û���')
insert SI_EntityPreference values('WMSServiceReCallTimes','3','WMS Web����ʧ�ܺ��ص�����')
insert SI_EntityPreference values('SAPServiceReCallTimes','3','SAP Web����ʧ�ܺ��ص�����')
insert SI_EntityPreference values('MaxRowSize','5000','��ʾ��Ŀ��')

Truncate table SI_LogToUser
insert dbo.SI_LogToUser values('10101','����SAPWebService��Item��Ϣʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10102','������Ϣ��SAP���뵽SIʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10103','LES�������µ�����','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10104','������Ϣ��SI������LESʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10201','����SAPWebService��Supplier��Ϣʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10202','��Ӧ����Ϣ��SAP���뵽SIʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10203','LES�������µĹ�Ӧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10204','��Ӧ����Ϣ��SI������LESʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10301','����������web����������','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10302','��������������ת��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10303','���������������м��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10304','����������','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10401','����������BOMweb����������','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10402','����������BOM����ת��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10403','����������BOM�����м��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10404','��������������ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10405','��������������ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10501','����ɹ���web����������','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10502','����ɹ�������ת��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10503','����ɹ��������м��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10504','�����ɹ���','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10510','�����ƻ�Э��web����������','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10601','�������web����������','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10602','�������м��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10603','������LESʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10604','�µ����','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10605','�����','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10606','����·��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10701','�����ƶ���������Web����ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10702','�ƶ����͵��뵽�м��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10703','�����ƶ����Ͷ�ȡLES����ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10704','�����ƶ������м��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10705','�����ƶ����ͼ�¼��֮���ϵʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10706','�����ƶ����ͼ�¼SAP���ؽ��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10707','�����̵��ƶ�������ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10801','����web����������','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10802','����ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('10803','���������м��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11101','WMS����LES����,LOC�ͻ��������ջ�ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11102','WMS����LES����,LOC�����Թ�Ӧ���ͻ������ջ���¼ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11103','WMS����LES����,LOC��Buff������˾����ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11104','WMS����LES����,LOC��Buff������˾��������ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11105','WMS����LES����,LOC�˻�ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11106','WMS����LES����,LOC�˻�����ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11107','WMS����LES����,LOC�ƻ�������ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11108','WMS����LES����,LOC�ƻ����������','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11109','WMS����LES����,LOC�����̵����','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11110','WMS����LES����,LOC����ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11111','WMS����LES����,LOC�ʼ첻�ϸ�ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11112','WMS����LES����,LOC�ò�ʹ��ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11113','WMS����LES����,LOC��������ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11201','LES����WMS����,��Ӧ��˫��Buff�򰲼��ͻ������ͻ���ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11202','LES����WMS����,��Ӧ��˫��Buff�򰲼��ͻ������ͻ�������ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11203','LES����WMS����,Buff��LOC���������򱸼���˾�������ϴ���Ҫ����ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11204','LES����WMS����,Buff��LOC���������򱸼���˾�������ϴ���Ҫ����ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11205','LES����WMS����,Buff��LOC�������ϴ�������ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11206','LES����WMS����,Buff��LOC�������ϴ������򵥳���ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11207','LES����WMS����,Buff����LOC���ͻ���,�����ջ���ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11208','LES����WMS����,Buff����LOC���ͻ���,�����ջ�������ʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('11209','LES����WMS����,LES�д��������뵼��WMSʧ��','liqiuyun@sconit.com',null);
insert dbo.SI_LogToUser values('20101','�������洴������ʧ��','liqiuyun@sconit.com',null);
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

Insert into SI_EntityPreference values('SAPItemCategory', 'FG', '���Ĭ�Ϸ���');

Insert into SI_EntityPreference values('NumberControlLength', '10', 'WMS�ų���');

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
insert into ACC_Permission values ('Url_SI_SAP_Item_View','SAP����','SI')
insert into sys_menu values('Url_SI_SAP','SAP',null,'30','SAP',null,'~/Content/Images/Nav/Quality.png','1')
insert into sys_menu values('Url_SI_SAP_Item_View','SAP����','Url_SI_SAP','331','SAP����','~/SAPItem/Index','~/Content/Images/Nav/Default.png','1')




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

