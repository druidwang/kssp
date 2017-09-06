---SAPMES ENDINV Of Each other
Select isnull(a.Item,b.item) As Item ,isnull(a.SAPLocation,b.SAPLocation) As SAPLocation ,
	isnull(a.SAPQty,0) As StartQTy,isnull(a.SAPinspectQty,0) As StartSAPinspectQty,isnull(b.SAPQty,0) As EndQty,
	isnull(b.SAPinspectQty,0) As EndSAPinspectQty into #SAP
  from Sconit_20140608.dbo.ZTMP_INV a full join Sconit_20140609.dbo.ZTMP_INV b
	on a.item=b.Item and a.saplocation=b.SAPLocation
Select * from Sconit_20140604.dbo.Inv_RecSIExecution where item ='200300'
Select * from Sconit_20140605.dbo.Inv_RecSIExecution where item ='200300'
---MES INV
--Drop table #MES
--Drop table #MES0601
--Drop table #MES0602
Select * from #MES0602 where item ='501040'
Select Item, Uom, SAPLocation,SUM(RealTimeQTY-NewTransQty) as QTY,SUM(IpQty)IpQty into #MES0601 from Sconit_20140608.dbo.Inv_RecSIExecution
group by Item, Uom, SAPLocation
Select Item, Uom, SAPLocation,SUM(RealTimeQTY-NewTransQty) as QTY,SUM(IpQty)IpQty,SUM(todayqty)todayqty into #MES0602 from Sconit_20140609.dbo.Inv_RecSIExecution
group by Item, Uom, SAPLocation	

--加上实时库存由非零变成零
Insert into #MES0602
Select Item, Uom, SAPLocation,0 as QTY,0 As IpQty,0 from #MES0601 a where not exists(select * from #MES0602 b where a.item=b.Item and a.saplocation=b.SAPLocation )
Select * from #MES0602 where Item ='501040'
Select * from Inv_RecSIExecution
--
--去掉过滤
Select b.ItemFrom,a.LGORT_H , sum(Cast(a.ERFMG_H as float) *case when BWART_H=101 then 1 else -1 end) AS Qty into #FIleter 
from SAP_Interface_PPMES0004 a,INV_ItemExchange b
where RIGHT(ZComnum,5)=b.Id Group by b.ItemFrom,a.LGORT_H 
Update a
	Set a.QTY =a.QTY -b.Qty  from  #MES0602 a,#FIleter b where a.item=b.ItemFrom and a.saplocation=b.LGORT_H 
Select * from #FIleter where ItemFrom ='270039'

--去掉废品报工
Drop table #ScrapQty
Select b.SAPLocation ,Item ,sum(Qty)Qty into #ScrapQty  from VIEW_LocTrans a,MD_Location b where OrderNo in(
 select OrderNo  from MRP_MrpExScrap where LastModifyDate>'2014-06-03 00:00:00' AND LastModifyDate<='2014-06-04 00:00:00'
 and OrderNo is not null and Id  !=16077
) and LocFrom =b.Code 
group by SAPLocation ,Item 
Update a
	Set a.QTY =a.QTY -b.Qty  from  #MES0602 a,#ScrapQty b where a.item=b.item and a.saplocation=b.saplocation 

-----
-----
--去掉6月1号之前的冲销
Select Item,'4002' LocFrom,SUM(BFQty)AS Qty into #hhkinki  from ORD_OrderBackflushDet 
where RecNo ='R400170570' and IsVoid =1 group by Item
Insert into #hhkinki
	Select '300848','4002',320
Update a
	Set a.QTY =a.QTY -b.Qty  from  #MES0602 a,#hhkinki b where a.item=b.item and a.saplocation=b.LocFrom 
	
	
	---销售正常
	Select MATERIAL  As Item ,LGORT As SAPLocation,SUM(Cast(TARGETQTY  As Float)) As Qty 
	into #Sale from SAP_Interface_SDMES0001 a  
	--where  Exists (Select matnr from SAP_Interface_MMMES0001 b 
	--where a.MATERIAL =b.matnr and(a.LGORT=b.LGORT) ) 
	Group by LGORT,MATERIAL 

	---销售冲销 没有收集到数据先不写脚本
	Select * from SAP_Interface_SDMES0002
	 
	Update a
	Set a.QTY=a.QTY+b.Qty from #MES0602 a ,#Sale b where a.Item=b.Item and a.SAPLocation=b.SAPLocation
	
-----
Select * from #ScrapQty
drop table #MES
Select isnull(a.Item,b.item) As Item ,isnull(a.SAPLocation,b.SAPLocation) As SAPLocation ,
	isnull(a.QTY,0) As StartMESQTy,isnull(a.IpQty,0) As StartIpQty,isnull(b.QTY,0)As EndMESQty,isnull(b.IpQty,0) As EndIpQty,isnull(b.todayqty,0) As todayqty  into #MES
	from #MES0601 a full join #MES0602 b
	on a.item=b.Item and a.saplocation=b.SAPLocation 
	

Select *   from #SAP a,#MES b where a.Item =b.Item and a.SAPLocation=b.SAPLocation 

and round(a.EndQty,3) !=round(b.EndMESQty,3)
--and a.Item in ('201272',
--'201269',
--'200003',
--'200321',
--'200003')	
--drop table #FIleter
Select * from sconit_20140604.dbo.Inv_RecSIExecution where Item ='200003' and SAPLocation ='1001'
Select * from sconit_20140605.dbo.Inv_RecSIExecution where Item ='200003' and SAPLocation ='1001'
Select * from Inv_RecSIExecution where createdate!='2014-06-16 19:24:20.280'
Select * from MD_Location  where SAPLocation  ='4005'



	
	
Select * from #temp a right join #FIleter b on a.item=b.ItemFrom and a.saplocation=b.LGORT_H 
 
 Select * from Sconit_20140604.dbo.ZTMP_INV a left join Sconit_20140603.dbo.ZTMP_INV b
	on a.item=b.Item and a.saplocation=b.SAPLocation where b.SAPLocation is null
  Select * from Sconit_20140604.dbo.ZTMP_INV where SAPinspectQty =300
--501040	3018
Select  * from SAP_Interface_PPMES0001 where MATNR_H ='501040'and LGORT_H ='3018'
Select * from VIEW_LocTrans where CreateDate 

Select * from MD_Location where SAPLocation  ='4002'

  --200423
  Select * from SAP_Interface_PPMES0001 where MATNR_I ='200423'
  
  Select * from VIEW_RecMstr  where LastModifyDate between '2014-06-01' and '2014-06-02' and Status =1
  Select * from VIEW_RecDet  where RecNo='R400172232'
 
 
 
 
  
  
