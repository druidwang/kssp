Select * from SAP_Interface_PPMES0001 where Id >8958

Select  * from SAP_Interface_PPMES0004
Select SalesOrg,DistrChan ,* from SCM_FlowMstr where Type=3
Select top 1000 * from ORD_OrderBackflushDet
Select OrderNo,OrderDetId,Bom,Item,COUNT(0) from ORD_OrderBomDet 
Group by OrderNo,OrderDetId,Bom,Item having COUNT(0) >1

Select top 1000 * from ORD_OrderBomDet where OrderNo ='O4MI0100000231' and Bom ='270028' and Item ='200062'
Order by Bom 
Select 3255.00000000*0.00045000
Select top 1000 * from ORD_OrderDet_4 where ID=290692
--Select top 100000 * from PRD_BomDet where scrappct!=0
SP_help'PRD_BomDet'
Select top 1000 * from SAP_Bom  where cast(AUSCH as float) !=0 order by CreateDate desc


Select top 1000 * from ORD_OrderBackflushDet where OrderBomDetId =0 order by CreateDate desc

Select top 1000 * from ORD_OrderBomDet where OrderDetId  =937824

Select top 1000 * from ORD_RecDet_4 order by CreateDate desc

Select top 1000  * from ORD_OrderBackflushDet where IsVoid =1 order by CreateDate desc

	Select Item ,case when IOType=0 then Locto else Locfrom end As Location,SUM(Qty) As Qty 
	 
		from VIEW_LocTrans 
		where CreateDate > '2014-07-04 15:56:10.000'   
		Group by Item ,case when IOType=0 then Locto else Locfrom end
		
		Select Location ,Item ,SUM(Qty ) from VIEW_LocationDet where Item in ('200290','200823','270232','270393','300974')and Location ='9204'
	group by Location ,Item 

Select top 1000 CreateDate ss ,* from ORD_OrderBackflushDet order by CreateDate desc
Select Item ,SUM(BFQty) from ORD_OrderBackflushDet where RecNo ='R400171603'
group by Item
 order by CreateDate desc

Select top 1000 CreateDate ss ,* from ORD_OrderBomDet  where OrderNo ='O4EX0500002159'


