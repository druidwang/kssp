--SAP(非限制 + SAP在途数（From 库位）+ 冻结库存 + 质检库存)  = MES（待验数量+不合格数+合格数+在途数（From库位,移库+销售）+冻结数-寄售库存）；
Use Sconit_20140801
Create table #Inv_RecSIExecution_Tmp(Item varchar(20),Uom varchar(20),MesLocation varchar(20),SAPLocation varchar(20),
RealTimeQTY decimal(18,8),CSQty decimal(18,8),InspectQty decimal(18,8),QualifiedQty decimal(18,8),InQualifiedQty decimal(18,8),
IPQty decimal(18,8),SalesIPQty decimal(18,8),TransferIPQty decimal(18,8))
Insert into #Inv_RecSIExecution_Tmp(Item, Uom, MesLocation, SAPLocation, RealTimeQTY,CSQty, InspectQty, QualifiedQty, InQualifiedQty,
		 IPQty,SalesIpQty,TransferIPQty)
		select a.Item,c.Uom,b.Code As MesLocation,Case when b.Region like 'S%' then b.Code else b.SAPLocation End,
			SUM(Qty) as RealTimeQTY,sum(CsQty) As CSQty,SUM(a.InspectQty) As InspectQty,SUM(a.QualifyQty) As QualifiedQty,
			SUM(a.RejectQty) As InQualifiedQty,0 As IpQty,0 As SalesIpQty,0 As TransferIPQty
			from VIEW_LocationDet_IncludeZeroQty a
			inner join MD_Location b on a.Location=b.Code
			inner join MD_Item c on a.Item=c.Code
			group by a.Item,c.Uom,b.Code ,Case when b.Region like 'S%' then b.Code else b.SAPLocation End

--Select * from Inv_RecSIExecution a,#Inv_RecSIExecution_Tmp b 
--	where CreateDate ='2014-08-01 00:00:00.890' and a.Item =b.Item and a.MesLocation=b.MesLocation
--	and a.IpQty !=b.IpQty
		CREATE TABLE #TempTransQty
		(
			Item varchar(50), 
			Location varchar(50), 
			Qty decimal(18,8)
		)
		CREATE TABLE #TempSalesQty
		(
			Item varchar(50), 
			Location varchar(50), 
			Qty decimal(18,8)
		)
		--销售
			insert into #TempSalesQty
				select a.Item as Item,a.LocFrom as Location,
					sum((Qty - RecQty)*UnitQty) As Qty
					from ORD_IpDet_3 a  where a.IsClose =0 and ABS(Qty)>ABS(RecQty) 
					group by a.Item,a.LocFrom	
		--移库
			insert into #TempTransQty
				select a.Item as Item,a.LocFrom as Location,
					sum((Qty - RecQty)*UnitQty) As Qty
					from ORD_IpDet_2 a  where a.IsClose =0 and ABS(Qty)>ABS(RecQty) 
					group by a.Item,a.LocFrom

		--委外移库
			insert into #TempTransQty
				select a.Item as Item,a.LocFrom as Location,
					sum((Qty - RecQty)*UnitQty) As Qty
					from ORD_IpDet_7 a  where a.IsClose =0 and ABS(Qty)>ABS(RecQty) 
					group by a.Item,a.LocFrom

		Select Item ,Location ,SUM(QTy) As QTy into #TempTransQtySum from #TempTransQty Group by Item ,Location
		Select Item ,Location ,SUM(QTy) As QTy into #TempSalesQtySum from #TempSalesQty Group by Item ,Location
		--收货被冲销，发货没有被冲销???(收货单冲销，送货单关闭时不算在途的.)
		Select b.Item,b.LocFrom As Location,b.RecQty As Qty 
			into #NonCancelIpOrder 
			from VIEW_IpMstr a,VIEW_RecDet b,VIEW_RecMstr c 
			where b.IpNo=a.IpNo and b.RecNo =c.RecNo 
			and c.Status =1 and a.Status in(1/*,2*/)
			and a.OrderType in (2,3,7)
		
		Update a	
			Set a.TransferIPQty=b.Qty from #Inv_RecSIExecution_Tmp a ,#TempTransQtySum b 
				where a.MesLocation=b.Location and a.Item=b.Item 
		Update a	
			Set a.SalesIpQty=b.Qty from #Inv_RecSIExecution_Tmp a ,#TempSalesQtySum b 
				where a.MesLocation=b.Location and a.Item=b.Item 
		Update #Inv_RecSIExecution_Tmp Set IPQty=SalesIPQty+TransferIPQty
		----实时库存为零，在途库存不为零
		 --Select distinct b.* into #kkkkk from #Inv_RecSIExecution_Tmp a right join  #TempTransQtySum b 
			--on a.MesLocation=b.Location and a.Item=b.Item where a.Item is null 
		 --Insert into #Inv_RecSIExecution_Tmp
			--Select a.item ,c.Uom ,b.Code ,b.SAPLocation ,0,a.qty,a.qty,0,0,0,dateadd(minute,1,GETDATE())
			--from #kkkkk a,MD_Location b,MD_Item c where a.Location =b.Code and a.item=c.Code 	
		Update b
			Set b.RealTimeQTY=a.RealTimeQTYOriginal ,b.IPQty =a.IpQty  from Inv_RecSIExecution a,#Inv_RecSIExecution_Tmp b 
				where CreateDate ='2014-08-01 00:00:00.890' and a.Item =b.Item and a.MesLocation=b.MesLocation
				and a.IpQty !=b.IpQty
				
				
Select * from Inv_RecSIExecution a,#Inv_RecSIExecution_Tmp b 
				where CreateDate ='2014-08-01 00:00:00.890' and a.Item =b.Item and a.MesLocation=b.MesLocation
				and a.RealTimeQTYOriginal !=b.RealTimeQTY
		drop table 	#TempTransQtySum
		drop table 	#TempTransQty
		drop table 	#TempSalesQty
		drop table 	#TempSalesQtySum
		drop table 	#NonCancelIpOrder		
			Drop table #Inv_RecSIExecution_Tmp

---SAP 数据导入
 --="Insert into ZTMP_INV Select '" &A2&"','"&C2&"','"&D2&"','"&F2&"','"&G2&"','"&J2&"','"&P2&"','"&L2&"'"
Create table ZTMP_INV(Item varchar(20),	SAPLocation varchar(20),Supplier varchar(20), Uom varchar(20),
SAPQty varchar(20),SAPIpQty varchar(20),SAPFreezeQty varchar(20),SAPinspectQty varchar(20))
	Select * from ZTMP_INV			
drop table #temp
Select top 100 * from #temp
Select Code ,Name,Case when region like 'S%' then Code else SAPLocation End SAPLocation  into #jjj from MD_Location 


Select SAPLocation as SAPLocation,
	stuff((SELECT '/' + Code from #jjj as t where t.SAPLocation = p.SAPLocation FOR xml path('')), 1, 1, '') as Location,
	stuff((SELECT '/' + Name from #jjj as t where t.SAPLocation = p.SAPLocation FOR xml path('')), 1, 1, '') as LocationDesc
	into #hhhhhh 
	from #jjj p 
	group by SAPLocation
	--Select * from #hhhhhh where SAPLocation ='3020'
	
--select distinct item ,SAPLocation,supplier  from ZTMP_INV drop table #temp
--Drop table #temp
Select Item, Uom, SAPLocation,SUM(realtimeqty) as QTY,SUM(csqty)csqty,SUM(TransferIpqty ) As TransferIpqty,
 SUM(SalesIpqty ) As SalesIpqty into #temp from #Inv_RecSIExecution_Tmp
group by Item, Uom, SAPLocation
Select * from ZTMP_INV a left join #temp b 
on a.Item=b.item And a.SAPLocation =b.SAPLocation
left join MD_Item c on a.Item =c.Code 
left join  #hhhhhh d on a.SAPLocation = d.SAPLocation 
 --where b.item is null
---寄售部分对比
truncate table _CS
Create table _CS(Item varchar(20),Uom varchar(20),SAPLocation varchar(20),
CSQty decimal(18,8))
Select Item, Uom, SAPLocation,SUM(csqty)csqty  into #CS from #Inv_RecSIExecution_Tmp
group by Item, Uom, SAPLocation
Select a.*,b.csqty As MESCSQty,c.Desc1 ,d.* from _CS a left join #CS b 
on a.Item=b.item And a.SAPLocation =b.SAPLocation
left join MD_Item c on a.Item =c.Code 
left join  #hhhhhh d on a.SAPLocation = d.SAPLocation 
 