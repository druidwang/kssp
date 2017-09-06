
Select 'select * from ',*from sys.objects where name like '%BIL%' and type ='U'
Select distinct a.name from sys.objects a ,sys.all_columns b where a.object_id =b.object_id and b.name like '%BillTerm%'
Select distinct a.name from sys.objects a ,sys.all_columns b where a.object_id =b.object_id and b.name like '%IsCS%'
Select COUNT(*) from BIL_ActBill
Select COUNT(*) from BIL_PlanBill
Select COUNT(*) from BIL_PlanBillTrans
Select COUNT(*) from BIL_SettleBillTrans
Select top 1000 CreateDate aa,BillTerm, * from BIL_PlanBill order by CreateDate desc
Select top 1000 CreateDate aa,BillTerm, * from BIL_ActBill order by CreateDate desc
Select top 1000 CreateDate aa,BillTerm, * from BIL_PlanBillTrans order by CreateDate desc
Select top 1000 CreateDate aa,BillTerm, * from BIL_SettleBillTrans order by CreateDate desc

select top 1000 CreateDate aa,* from 	BIL_PlanBillTrans order by CreateDate desc
--select top 1000 CreateDate aa,* from 	BIL_PriceListDet order by CreateDate desc
--select top 1000 CreateDate aa,* from 	BIL_PriceListMstr order by CreateDate desc
--select top 1000 CreateDate aa,* from 	BIL_BillDet order by CreateDate desc
--select top 1000 CreateDate aa,* from 	BIL_BillTrans order by CreateDate desc
select top 1000 CreateDate aa,* from 	BIL_ActBill order by CreateDate desc
--select top 1000 CreateDate aa,* from 	BIL_BillMstr order by CreateDate desc
 
select top 1000 CreateDate aa,* from 	BIL_SettleBillTrans order by CreateDate desc
--select top 1000 CreateDate aa,* from 	BIL_PriceListDet_Interim order by CreateDate desc
select top 1000 CreateDate aa,* from 	BIL_PlanBill order by CreateDate desc
Select top 1000 * from VIEW_RecLocationDet where HuId ='HU000220746' Order by CreateDate DESC
Select top 1000 * from VIEW_LocationLotDet  where HuId ='HU000220746' Order by CreateDate DESC

Select top 1000 * from VIEW_LocationLotDet where IsCS =1 Order by CreateDate Desc
Update INV_LocationLotDet_2
Set IsCS =0 from INV_LocationLotDet_2 where IsCS =1 and HuId ='HU000403320'---- Order by Location ,LotNo 
--9103	NULL	200003	HU000403319
Select top 1000 * from SCM_FlowDet  where Item ='201598' Order by CreateDate Desc
Select top 1000 CreateDate dddd,* from BIL_PlanBill order by CreateDate desc
Select top 1000 CreateDate dddd,* from BIL_ActBill  order by CreateDate desc
Select top 1000 * from ORD_MiscOrderMstr where MiscOrderNo ='MI00000001'
Select top 1000 * from #kkkkk 

Select top 1000 * from VIEW_LocationLotDet where Location ='9101' and Item ='201598'
Select top 1000 * from VIEW_LocationLotDet where IsCS =1


-----目前看起来,直接更新INV_LocationLotDet_2的ISCS标记就可以了
--测试发现一个问题，
--@1：数量寄售库存的上线系统没有生成开票的信息
--@2：是否需要更新VIEW_RecLocationDet这张表的信息
