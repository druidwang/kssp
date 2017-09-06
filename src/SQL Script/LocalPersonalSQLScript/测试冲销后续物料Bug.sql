

Select top 100 * from SCM_FlowMstr where Code ='FI51'
Select top 100 * from ORD_OrderBomDet where OrderNo ='O4MI0100000824' and Bom ='270028'
Select distinct BackFlushMethod,FeedMethod ,IsAutoFeed ,IsScanHu  from ORD_OrderBomDet 

Select Item,SUM(BFQty) from ORD_OrderBackflushDet where RecNo ='R400171598' Group by Item

Select top 100 * from ORD_OrderBackflushDet order by CreateDate desc
Select top 1000 Item ,case when IOType=0 then Locto else Locfrom end As Location,SUM(Qty)  from VIEW_LocTrans 
where RecNo ='R400171598' group by Item ,case when IOType=0 then Locto else Locfrom end 
 --order by CreateDate desc
--Select top 1000 * from ORD_OrderBackflushDet where OrderBomDetSeq  is null

Select top 1000 * from ORD_OrderBackflushDet where RecNo ='R400171595'
Select recno,COUNT(0) from ORD_RecDet_4
Group by RecNo having COUNT(0)>1

Select Machine ,* from SCM_FlowDet where Item ='500149'
Select Machine ,* from SCM_FlowDet where Item ='500151'
Select top 100 * from MRP_MrpFiShiftPlan  where PlanDate ='2014-07-07' 
and ProductLine ='FI31'
and Item in ('500149','500151')
Select top 100 * from PRD_BomDet where Item  ='270393'
Select top 100 * from MRP_MrpFiMachinePlan where PlanVersion ='2014-07-03 16:32:40.000'
and PlanVersion ='2014-07-03 16:32:40.000'	and Machine ='600127'


Select Location,Item, Sum(qty) As Qty into #kkk from VIEW_LocationDet 
Group by Location,Item

Select Location,Item, Sum(qty) As Qty into #jjj from VIEW_LocationDet 
Group by Location,Item

Select *,a.Qty - b.Qty from #kkk a ,#jjj b where a.Location =b.Location and a.Item =b.Item and a.Qty !=b.Qty

Drop table #kkk
Drop table #jjj


