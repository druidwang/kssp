
Use Sconit_201405010Olock 
drop table #temp
select a.Item,c.Uom,b.SAPLocation,SUM(qty) as QTY into #temp from VIEW_LocationLotDet a
inner join MD_Location b on a.Location=b.Code
inner join MD_Item c on a.Item=c.Code
--where b.Region not like 'S%'
group by a.Item,b.SAPLocation,c.Uom

Select top 100 * from #temp
Select top 100 * from VIEW_LocationLotDet
Select top 100 * from MD_Location where Code in (Select Code from MD_Supplier ) 
Select top 100 * from MD_Location where Region like 'S%'
Select top 100 * from sys.objects where name like '%supplier%' 
Select top 100 * from sys.columns  where name like '%supplier%' 

select Item ,SUM(Qty )Qty into #kkkk from VIEW_LocationDet group by Item
select Item ,SUM(Qty )Qty into #ffff from VIEW_LocationLotDet group by Item
 --Select a.Qty,b.Qty ,* from #kkkk a left join #ffff b on a.item=b.Item and a.Qty !=b.Qty  
 --Select * from #kkkk a inner join #ffff b on a.item=b.Item and a.Qty =b.Qty  
Select * from #temp
Select * from Inv_RecSIExecution
Drop table #temp
Select Code ,Name,Case when region like 'S%' then Code else SAPLocation End SAPLocation  into #jjj from MD_Location 

--去掉过滤
Select b.ItemFrom,a.LGORT_H , sum(Cast(a.ERFMG_H as float) *case when BWART_H=101 then 1 else -1 end) AS Qty into #FIleter 
from SAP_Interface_PPMES0004 a,INV_ItemExchange b 
where RIGHT(ZComnum,5)=b.Id and ZMESSC!='O4MI0100000831' Group by b.ItemFrom,a.LGORT_H 

--去掉废品报工
Drop table #ScrapQty
Select b.SAPLocation ,Item ,sum(Qty)Qty into #ScrapQty  from VIEW_LocTrans a,MD_Location b where OrderNo in(
 select OrderNo  from MRP_MrpExScrap where LastModifyDate>'2014-06-02 00:00:00' AND LastModifyDate<='2014-06-03 00:00:00'
 and OrderNo is not null and Id  !=16077
) and LocFrom =b.Code 
group by SAPLocation ,Item 

-----
 

Select SAPLocation as SAPLocation,
	stuff((SELECT '/' + Code from #jjj as t where t.SAPLocation = p.SAPLocation FOR xml path('')), 1, 1, '') as Location,
	stuff((SELECT '/' + Name from #jjj as t where t.SAPLocation = p.SAPLocation FOR xml path('')), 1, 1, '') as LocationDesc
	into #hhhhhh 
	from #jjj p 
	group by SAPLocation
	--Select * from #hhhhhh where SAPLocation ='3020'
	
--select distinct item ,SAPLocation,supplier  from ZTMP_INV drop table #temp
--Drop table #temp
Select Item, Uom, SAPLocation,SUM(totalqty) as QTY,SUM(Ipqty ) As Ipqty into #temp from Inv_RecSIExecution
group by Item, Uom, SAPLocation

Select distinct Item ,case when Region like 's%' then Code else SAPLocation End SapLocation into #JISHOU from VIEW_LocationDet  a,MD_Location b  where  CsQty  !=0 and a.Location =b.Code 
 Select top 1000 * from Inv_RecSIExecution
Select top 100 * from  MD_Location where IsCS =1
--去掉过滤
Update a
	Set a.QTY =a.QTY -b.Qty  from #temp a,#FIleter b where a.item=b.ItemFrom and a.saplocation=b.LGORT_H 
Select * from #temp a right join #FIleter b on a.item=b.ItemFrom and a.saplocation=b.LGORT_H 
--去掉废品报工
Update a
	Set a.QTY =a.QTY -b.Qty  from  #temp a,#ScrapQty b where a.item=b.item and a.saplocation=b.saplocation 
Select * from #ScrapQty

Select item ,saplocation,SUM( realtimeqtyoriginal-realtimeqty) As CSQty into #jjjjj from Inv_RecSIExecution  Group by item,saplocation

select  a.Item ,c.Desc1,a.SAPLocation  ,a.SAPQty+SAPInspectqty AS SAPQty,b.QTY As MESQty,
a.sapinspectqty,Case when a.SAPQty+SAPInspectqty-round(b.QTY,3)!=0 then 'Y' else 'N' ENd,a.Uom ,d.Location,d.LocationDesc,e.CSQty  from ZTMP_INV a 
inner join #temp b on a.Item=b.item and a.SAPLocation=b.SAPLocation 
inner join MD_Item c on a.Item =c.Code 
left join  #hhhhhh d on a.SAPLocation = d.SAPLocation 
left join #jjjjj e on a.Item=e.item and a.SAPLocation=e.SAPLocation
where a.SAPLocation is not null
 
 
 Select * from #FIleter
select  a.Item ,c.Desc1,a.SAPLocation  ,a.SAPQty+SAPInspectqty AS SAPQty,b.QTY As MESQty,Case when 
a.SAPQty+SAPInspectqty-b.QTY!=0 then 'Y' else 'N' ENd,a.Uom ,d.Location,d.LocationDesc  from ZTMP_INV a 
left join #temp b on a.Item=b.item and a.SAPLocation=b.SAPLocation 
Left join MD_Item c on a.Item =c.Code 
left join  #hhhhhh d on a.SAPLocation = d.SAPLocation 
where b.SAPLocation is   null

 Select * from #temp where QTY =0
 Select * from MD_Location  where SAPLocation ='1002'

Select  b.Item ,c.Desc1,b.SAPLocation  ,a.SAPQty+SAPInspectqty AS SAPQty,b.QTY As MESQty,Case when a.SAPQty+SAPInspectqty-b.QTY!=0 then 'Y' else 'N' ENd,c.Uom ,d.Location,d.LocationDesc  from ZTMP_INV a 
right join #temp b on a.Item=b.item and a.SAPLocation=b.SAPLocation 
inner join MD_Item c on b.Item =c.Code 
left join  #hhhhhh d on d.SAPLocation = b.SAPLocation 
where a.SAPLocation is  null
 
select  a.*,b.Desc1,d.* from #temp a left join MD_Item b on a.item=b.Code left join #hhhhhh d on d.SAPLocation = a.SAPLocation 
select top 1000 * from MD_Location 
select top 1000 * from VIEW_LocationLotDet where HuId is null

select * from ZTMP_INV a right join #temp b on a.Item=b.item and a.SAPLocation=b.SAPLocation 
where a.Item is null
select top 1000 * from ZTMP_INV 

Select * from #temp  
--Select SUM(qty) from VIEW_LocationDet where Item ='501848'
--Select SUM(qty) from VIEW_LocationLotDet where Item ='501848'

Select * from PRD_BomMstr where Code ='270084'

Select * from SAP_Bom  where MATNR ='270084'
--Update sconit_20140601.dbo.SYS_EntityPreference
--Set Value ='y123456' from sconit_20140601.dbo.SYS_EntityPreference  where Id =11002
--Update sconit_20140601.dbo.SYS_EntityPreference
--Set Value ='10.166.1.72' from sconit_20140601.dbo.SYS_EntityPreference  where Id =11004
--100007
Select top 1000 * from Inv_RecSIExecution where saplocation='N/A'

--Select top 1000 * from MD_Location where Code ='500026'
--Select top 1000 * from MD_Location where Code ='9306'

------
		select LEFT(code,5)Location into #WL from SCM_FlowMstr where Type =5
		
		
		Select * into #KKKKKK from VIEW_LocTrans where case when IOType=0 then Locto else Locfrom end in (select * from #WL)
		
		Select case when IOType=0 then '入' else '出' End 动作,Qty As 数量,OrderNo,IpNo ,RecNo ,RecDetId ,Item 
		,case when IOType=0 then Locto else Locfrom end As Location into #hhhhhhs
		 from #KKKKKK
		 
		 
		 Select Location,Item ,sum(Case when Orderno Like 'O%' then 1 else 0 end*数量) As 差异收货,
		 sum(Case when Orderno Like 'O%' then 0 else 1 end*数量) As 计划外数量,
		 sum(数量) As 当前数量 into #jjjuuu
		   from #hhhhhhs Group by Location,Item
		 Select * from #jjjuuu where Item in (select Item from SCM_FlowDet where Flow in(select Code from SCM_FlowMstr where TYPE=5))
		 Select * from #hhhhhhs where Item in (select Item from SCM_FlowDet where Flow in(select Code from SCM_FlowMstr where TYPE=5))
		 
		 Select * from SCM_FlowDet where Item ='400005'
		 
		 
			Select  a.IpNo ,b.* from VIEW_RecMstr a ,VIEW_recDet b where a.Type =1   and a.RecNo =b.RecNo 
and a.Flow in(select Code from SCM_FlowMstr where TYPE=5) 
-------

	 Select * from SAP_Interface_PPMES0001


Select RecNo,Item,SUM(BFQty),MIN(CreateDate) from ORD_OrderBackflushDet where 
  RecNo in (Select RecNo from ORD_OrderBackflushDet where IsVoid =1 and CreateDate >'2014-06-01')
Group by RecNo,Item having SUM(BFQty)>0
---去掉假的在途
Drop table #FakeIQP
Select LocFrom,Item ,SUM(Qty) As Qty into #FakeIQP from VIEW_LocTrans 
where CreateDate >'2014-06-06' and IpNo like 'A%' group by LocFrom,Item
 

Select * from VIEW_LocTrans 
where CreateDate >'2014-06-06' and IpNo like 'A%' group by LocFrom
order by CreateDate desc
Update a
Set  a.TotalQty =a.TotalQty -ABS(b.Qty) from Sconit_20140606.dbo.Inv_RecSIExecution a,#FakeIQP b where a.Item =b.Item and a.MesLocation  =b.LocFrom 
Select * from Sconit_20140607.dbo.Inv_RecSIExecution where Item ='300541' and SapLocation ='3001' 

