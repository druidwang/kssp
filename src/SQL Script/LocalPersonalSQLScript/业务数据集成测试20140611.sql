use Sconit_20140605
Truncate table SAP_Interface_PPMES0001 
Truncate table SAP_Interface_PPMES0002 
Truncate table SAP_Interface_PPMES0003 
Truncate table SAP_Interface_PPMES0004 
Truncate table SAP_Interface_PPMES0005 
Truncate table SAP_Interface_PPMES0006 
Truncate table SAP_Interface_MMMES0001
Truncate table SAP_Interface_MMMES0002
Truncate table SAP_Interface_SDMES0001
Truncate table SAP_Interface_SDMES0002
Truncate table SAP_Interface_STMES0001
Delete SAP_EX001 where RecTime >'2014-06-06'
Truncate table SAP_Interface_ExscraptDet 
Truncate table SAP_Interface_ExscraptMstr
Truncate table SAP_Interface_AdjustDet 
Truncate table SAP_Interface_AdjustMstr 

--exec [USP_SAP_PP_UpdateEXScraptInterim] 'jejhidswwsrgwqjdoqjdp','2014-06-02','2014-06-03'
--Insert into SAP_EX001
--Select * from sconit_20140607.dbo.SAP_EX001 

--Declare @BatchNo varchar(200)=replace(newID(),'-','')
--Declare @BatchNo1 varchar(200)=replace(newID(),'-','')
--Update SAP_Interface_MMMES0001
--Set BatchNo=@BatchNo,uniqueCOde=@BatchNo1,status=1 from SAP_Interface_MMMES0001 where ZMESPO like 'O5%' and status=0
--Update SAP_Interface_MMMES0001 Set status=1
--Set BatchNo=@BatchNo,uniqueCOde=@BatchNo1,status=1 from SAP_Interface_MMMES0002 where ZMESPO like 'O5%' and status=0
Select replace(newID(),'-','')
Select top 1000 * from SAP_Interface_MMMES0001 
Select top 1000 * from SAP_Interface_MMMES0002
Select top 1000 * from SAP_Interface_STMES0001 
Select top 10000 * from SAP_Interface_PPMES0001
Select top 1000 * from SAP_Interface_PPMES0002
Select top 1000 * from SAP_Interface_PPMES0003
Select top 1000 * from SAP_Interface_PPMES0004
Select top 1000 * from SAP_Interface_PPMES0005
Select top 1000 * from SAP_Interface_PPMES0006
Select top 1000 * from SAP_Interface_SDMES0001
Select top 1000 * from SAP_Interface_SDMES0002

--Update SAP_TransTimeCtrl Set LastTransDate ='2014-06-04 00:00:00.000',CurrTransDate ='2014-06-04 00:00:00.000' where SysCode ='BusinessData'
Select top 1000 * from SAP_TransTimeCtrl

Select top 1000 * from SAP_Interface_ExscraptDet
Select top 1000 * from SAP_Interface_ExscraptMstr
Select top 1000 * from SAP_Interface_AdjustDet
Select top 1000 * from SAP_Interface_AdjustMstr

Select top 1000 * from SAP_Interface_STMES0001 a  
where  Exists (Select matnr from SAP_Interface_MMMES0001 b 
where a.matnr1=b.matnr and(a.LGORT=b.LGORT or a.UMLGO=b.LGORT) ) 

Use Sconit_20140602
--排除移库和销售的库存
--排除移库部分
--Drop table #移库出
--Drop table #移库入
--冲销取相反数
--Locfrom ZMESGUID like '%1' 冲销 ZMESGUID like '%0' 正常
Select matnr1 As Item ,LGORT As SAPLocation,SUM(Cast(EPFMG As Float)*Case when ZMESGUID like '%1' then -1 else 1 End ) As Qty 
	into #移库出 from SAP_Interface_STMES0001 a  
	where  Exists (Select matnr from SAP_Interface_MMMES0001 b 
	where a.matnr1=b.matnr and a.LGORT=b.LGORT)Group by LGORT,matnr1

---LocTo
Select matnr1 As Item ,UMLGO As SAPLocation,SUM(Cast(EPFMG As Float)*Case when ZMESGUID like '%1' then -1 else 1 End ) As Qty
	into #移库入 from SAP_Interface_STMES0001 a  
	where  Exists (Select matnr from SAP_Interface_MMMES0001 b 
	where a.matnr1=b.matnr and a.UMLGO=b.LGORT) Group by UMLGO,matnr1
	
--MM冲销
---销售冲销 没有收集到数据先不写脚本
Select * from SAP_Interface_MMMES0002
----Sconit_20140602是六月二号零点备份的数据库,Inv_RecSIExecution是零点的库存记录
--RealTimeQTY（实时库存）	IpQty（在途库存）	NewTransQty（零点后的事务）	TotalQty（RealTimeQTY+IpQty-NewTransQty）

Select Item,SAPlocation,SUM(totalqty) As Qty into #temp  from Inv_RecSIExecution
	Group by Item,SAPlocation
--移库出，库存加回去
Update a
Set a.QTY=a.QTY+b.Qty from #temp a ,#移库出 b where a.Item=b.Item and a.SAPLocation=b.SAPLocation
--移库入，库存减掉
Update a
Set a.QTY=a.QTY-b.Qty from #temp a ,#移库入 b where a.Item=b.Item and a.SAPLocation=b.SAPLocation

Select * from #移库出

--扣除销售的部分
---销售正常
Select MATERIAL  As Item ,LGORT As SAPLocation,SUM(Cast(TARGETQTY  As Float)) As Qty 
into #Sale from SAP_Interface_SDMES0001 a  
where  Exists (Select matnr from SAP_Interface_MMMES0001 b 
where a.MATERIAL =b.matnr and(a.LGORT=b.LGORT) ) Group by LGORT,MATERIAL 

---销售冲销 没有收集到数据先不写脚本
Select * from SAP_Interface_SDMES0002
 
Update a
Set a.QTY=a.QTY-b.Qty from #temp a ,#Sale b where a.Item=b.Item and a.SAPLocation=b.SAPLocation

---对比采购库存
Select * from ZTMP_INV a,#temp b, #temp061 d,#temp062 c where a.Item=b.Item and a.SAPLocation =b.SAPLocation 
and a.Item =c.item and a.SAPLocation =c.saplocation and a.Item =d.item and a.SAPLocation =d.saplocation
and exists(select * from SAP_Interface_MMMES0001 c where a.Item =c.MATNR and a.SAPLocation =
c.LGORT )

Select Item,SUM(BFQty*UnitQty )  from ORD_OrderBackflushDet where OrderNo in 
(select OrderNo  from MRP_MrpExScrap where 
CreateDate between '2014-06-02' and '2014-06-03' and ScrapType in (24,25)and IsVoid =0)
Group by Item order by Item
Select * from ORD_OrderBackflushDet
Select * from SAP_Interface_PPMES0003 where MATNR_I =''

Select MATNR_I,SUM(CAST (erfmg_I as float)) from SAP_Interface_PPMES0003 group by MATNR_I order by MATNR_I

Select top 1000 * from VIEW_LocTrans a,MD_Location b where Item ='200003' --and a.LocTo=b.Code and b.SAPLocation='1002'
and a.CreateDate >'2014-06-01 00:00:00'
Select top 1000 * from SAP_Interface_STMES0001 where LGORT ='1002' and MATNR1 ='201269'
Select top 1000 * from SAP_Interface_STMES0001 where UMLGO  ='1002' and MATNR1 ='201269'
Select top 1000 * from SAP_Interface_MMMES0001 where LGORT  ='1002' and MATNR  ='201269'
---对比委外库存
Select * from ZTMP_INV a,#temp b where a.Item=b.Item and a.SAPLocation =b.SAPLocation 
and exists(select * from SAP_Interface_MMMES0001 c where a.Item =c.MATNR_C and a.SAPLocation =
c.LIFNR)


Select * from ZTMP_INV a,#temp061 b,#temp062 c  where a.Item=b.Item and a.SAPLocation =b.SAPLocation
and a.Item =c.item and a.SAPLocation =c.SAPLocation 
and exists(select * from SAP_Interface_MMMES0001 c where a.Item =c.MATNR_C and a.SAPLocation =
c.LIFNR)

Select * from ZTMP_INV a,#temp061 b,#temp062 c  where a.Item=b.Item and a.SAPLocation =b.SAPLocation
and a.Item =c.item and a.SAPLocation =c.SAPLocation 
and exists(select * from SAP_Interface_MMMES0001 c where a.Item =c.MATNR  and a.SAPLocation =
c.LGORT)

----汇总PP业务移动
Select distinct ZMESSC ,MATNR_H,ZComnum ,LGORT_H ,ERFMG_H  from SAP_Interface_PPMES0001 

Select* from SAP_Interface_PPMES0001



Select* from SAP_Interface_STMES0001 a  
	where  Exists (Select matnr from SAP_Interface_MMMES0001 b 
	where a.matnr1=b.matnr and a.LGORT=b.LGORT)
Select top 1000 * from #temp0601 where SAPLocation ='30477' and Item ='200015'
Select top 1000 * from #temp0602 where SAPLocation ='30477' and Item ='200015'

Select top 1000 * from SAP_Interface_MMMES0001 where LGORT ='1002' and MATNR_C  ='200003'
Select top 1000 * from SAP_Interface_MMMES0002
Select top 1000 * from SAP_Interface_SDMES0001
Select top 1000 * from SAP_Interface_SDMES0002
Select top 1000 * from SAP_Interface_STMES0001


Select Item,SAPlocation,SUM(totalqty) As Qty into #temp0601 from sconit_20140601.dbo.Inv_RecSIExecution
	Group by Item,SAPlocation
Select Item,SAPlocation,SUM(totalqty) As Qty into #temp0602 from Inv_RecSIExecution
	Group by Item,SAPlocation
--Select	250200.00000000+
--689.087+
--1139.294
Select top 1000 * from VIEW_LocationDet where CsQty !=0 and Item ='201354'
--Select SUM(Qty) from VIEW_LocationLotDet where IsCS =0 and Item ='201354'
--R100001870




Select * from VIEW_LocationLotDet where HuId ='HU000587943'
Select top 1000 * from VIEW_LocTrans order by CreateDate desc



Select top 1000 * from VIEW_LocationDet where Item in ('200290','200823','270020','270232')
Select top 1000 * from PRD_BomDet  where Bom  like '290088%'


-------

 
Update SAP_Interface_PPMES0001 Set Status =1 where   Status =0
Update SAP_Interface_PPMES0001 Set Status =0,ZMESGUID =dbo.FormatDate(GETDATE(),'YYYYMMDDHHNNSS0000') where ZMESSC like 'O4FI%'
Select top 10000 * from SAP_Interface_PPMES0001 where Status =0
--Select 1427+3113+2348
--Select top 1000 * from SAP_Interface_PPMES0001  
Select top 1000 * from SAP_Interface_MMMES0001
Select top 1000 * from SAP_Interface_MMMES0002
Select top 1000 * from SAP_Interface_STMES0001 where MATNR1 ='200815'

Select top 10000 * from SAP_Interface_PPMES0001 where zmessc='O4MI0100000831'
Select top 1000 * from SAP_Interface_PPMES0002
Select top 1000 * from SAP_Interface_PPMES0003 where grund in ('MES21','MES22','MES23') or MTSNR='OF00016077'
--Update SAP_Interface_PPMES0003
--Set Status =0,XMNGA=CEILING(XMNGA) from SAP_Interface_PPMES0003 where grund in ('MES21','MES22','MES23') or MTSNR='OF00016077'
--Update SAP_Interface_PPMES0004
--Set LFSNR =ZComnum  from SAP_Interface_PPMES0004
--Update SAP_Interface_PPMES0004
--Set Status =1 from SAP_Interface_PPMES0004 where ZMESSC not in (Select ZMESSC from SAP_Interface_PPMES0001)
Select top 1000 * from SAP_Interface_PPMES0004 where zmessc='O4MI0100000831'
Select top 1000 * from SAP_Interface_PPMES0005 
Select top 1000 * from SAP_Interface_PPMES0006 
--Update SAP_Interface_PPMES0006
--Set Status =1 from SAP_Interface_PPMES0006 where ZMESSC ='MI00000260'
Select top 1000 * from SAP_Interface_SDMES0001
Select top 1000 * from SAP_Interface_SDMES0002
Select top 1000 * from SAP_EX001 where PlanNo ='OE142330111500'
--truncate table SAP_Interface_ExscraptDet
--truncate table SAP_Interface_ExscraptMstr
--truncate table SAP_Interface_PPMES0003
Select top 1000 * from SAP_Interface_ExscraptDet where MiscOrder ='OF00016055'
Select top 1000 * from SAP_Interface_ExscraptMstr where MiscOrder ='OF00016055'
Select top 1000 * from SAP_Interface_AdjustDet 
Select top 1000 * from SAP_Interface_AdjustMstr 
Select REPLACE(NEWID(),'-','')
--EXEC dbo.USP_SAP_PP_ExportEXScraptOrder '456335BF8EBD4EC4BE2AB43F00800551','2014-06-02','2014-06-03'

select distinct ZMESGUID ,ZMESSC , ZComnum  from SAP_Interface_PPMES0001 
select distinct zmespo, ZMESGUID   from SAP_Interface_MMMES0001 
select distinct MATNR   from SAP_Interface_MMMES0001 
select * from SAP_Interface_MMMES0001 


select distinct ZMESGUID ,ZMESSC,ZMESLN,ZComnum from SAP_Interface_PPMES0001 where ZMESSC like 'O4FI%'

select distinct ZMESGUID from SAP_Interface_PPMES0001 
select * from SAP_Interface_PPMES0002 where MATNR_H  ='501952'


select * from SAP_Interface_PPMES0001 where cast(EPFMG as float) =144

select * from SAP_Interface_PPMES0006



