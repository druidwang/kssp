Select  top 1000 * from  Inv_RecSIExecution where Item ='200885' and MesLocation ='9103'
Select  top 1000 * from Inv_RecSIExecution where Item ='200885' and MesLocation ='9103'
Select  top 1000 * from BAT_Job 
Select  top 1000 * from BAT_JobParam 
Select  top 1000 * from BAT_TriggerParam 
Select  top 1000 * from BAT_RunLog where JobCode ='CleanOrderJob' order by StartTime desc


Select  top 1000 CompleteDate ,CompleteUser ,* from ORD_OrderMstr_4 order by LastModifyDate desc
Update ORD_OrderMstr_4
Set Status =3 from ORD_OrderMstr_4 where OrderNo ='O4MI0100000828'  
Select  top 1000 Status ,* from ORD_OrderMstr_4 where OrderNo ='O4MI0100000828'  
Select  top 1000 * from BIL_ActBill where Item ='200885' order by CreateDate desc 
Select  top 1000 * from BIL_SettleBillTrans  where Item ='200885' order by CreateDate desc 
Select  top 1000 * from BIL_PlanBill where recno='R100001510'
Select  top 1000 * from VIEW_RecDet  where recno='R700001077'
Select  top 1000 * from VIEW_IpDet  where IpNo ='A100001529'
Select  top 1000 * from SCM_FlowMstr where Code ='9103-30275'  
Select  top 1000 * from MD_Location  where Code ='9103-30275'  
--

Select  top 1000 * from VIEW_RecDet  where recno='R100001510'
Select * from VIEW_LocTrans where CreateDate >'2014-06-05 23:55:07.000' and Item ='200885'
Select  top 1000 * from SAP_Interface_STMES0001  where MATNR1 ='200885'
Select  top 1000 * from SAP_Interface_MMMES0001  where MATNR  ='200885'
 
Select  top 1000 * from Sconit_20140605.dbo.VIEW_LocationDet where Item='200885' and Location ='9103'
Select  top 1000 * from VIEW_LocationDet where Location= '9206'

Select  top 1000 * from Sconit_20140605.dbo.VIEW_LocationDet where Item='200885' and Location ='30275'
Select  top 1000 * from VIEW_LocationDet where Item='200885' and Location ='30275'
Select  top 1000 * from VIEW_RecLocationDet where RecNo ='R100001510'
Select  top 1000 * from VIEW_RecLocationDet where RecNo ='R700001077'
Select  top 1000 * from VIEW_IpLocationDet where OrderDetId =944200

--Select 12159.14270000-11662.49016000

Select *from VIEW_LocTrans where HuId ='HU000607119'
 
 
Select top 1000 LastModifyDate sss ,CreateDate aa,BillTerm, * from BIL_PlanBill order by LastModifyDate desc
Select top 1000 CreateDate aa,BillTerm, * from BIL_ActBill order by CreateDate desc
Select top 1000 CreateDate aa,BillTerm, * from BIL_PlanBillTrans order by CreateDate desc
Select top 1000 CreateDate aa,BillTerm, * from BIL_SettleBillTrans order by CreateDate desc
Select top 1000 * from VIEW_IpLocationDet where IpNo ='A700001058'
Select top 1000 PlanBill ,* from VIEW_LocationLotDet with(nolock) where PlanBill =31319
Select top 1000  * from BIL_PlanBill where IpNo ='A100001308'
Select top 1000  * from ACC_User where Code like '%9308%'
Select top 1000  * from BIL_PlanBill where Id = 31319
 
 
 
 
 