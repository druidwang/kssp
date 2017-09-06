--Select top 1000 * from SAP_EX001 a,MRP_ProdLineEx b where batchno ='333d37b549174da99c14e2ffdac9eafe'
 
--Select top 1000 * from MRP_ProdLineEx where Item ='290021'
 
--Select Qty*RateQty/Speed/60 from mrp_mrpexshiftplan 
----SP_help'mrp_mrpexshiftplan'
--select 1000.002%2
Use Sconit_20140509
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
Truncate table SAP_EX001
Truncate table SAP_Interface_ExscraptDet 
Truncate table SAP_Interface_ExscraptMstr
Truncate table SAP_Interface_AdjustDet 
Truncate table SAP_Interface_AdjustMstr 
Update Sconit_20140601.dbo.SYS_EntityPreference
Set Value ='y123456' from Sconit_20140601.dbo.SYS_EntityPreference where Id =11002
Update Sconit_20140601.dbo.SYS_EntityPreference
Set Value ='10.166.1.72' from Sconit_20140601.dbo.SYS_EntityPreference where Id =11004
Update Sconit_20140601.dbo.SYS_EntityPreference
Set Value ='1' from Sconit_20140601.dbo.SYS_EntityPreference where Id =90102
Select * from SAP_TransTimeCtrl where SysCode in ('BusinessData','','','','','')
Update SAP_TransTimeCtrl Set SysCode ='BusinessData',LastTransDate='2014-06-01 00:00:00.000',currtransdate='2014-06-01 00:00:00.000' where SysCode='BusinessData'





Select distinct ZMESSC,ZMESLN ,ZComnum,ZMESGUID from SAP_Interface_PPMES0001
update SAP_Interface_PPMES0001
Set  Status =0 from SAP_Interface_PPMES0001  
Select  ZMESGUID  ,COUNT(1)  from SAP_Interface_PPMES0001 group by ZMESGUID
Select  COUNT(0)  from SAP_Interface_PPMES0001
Select  COUNT(0)  from SAP_Interface_PPMES0004
Select  COUNT(0)  from SAP_Interface_PPMES0005
Select  COUNT(0)  from SAP_Interface_PPMES0006
Select top 10000 * from SAP_Interface_PPMES0001 where OrderType ='adjustorder'
Select top 10000 * from SAP_Interface_PPMES0001 where MATNR_I ='200043'

--Update SAP_Interface_PPMES0001
--Set ERFMG_I =ABS(ERFMG_I) from SAP_Interface_PPMES0001 
Select top 1000 * from SAP_Interface_PPMES0002
----Insert into SAP_Interface_PPMES0002
---- Select distinct top 3 ZMESSC,ZMESLN,'BUSCX01',ZComnum ,'201405091101470000',GETDATE(),
---- '9A0683C08ECA45A89D38AF16E6921D51','9A0683C08ECA45A89D38AF16E6921D51','0',OrderType  from SAP_Interface_PPMES0001
---- where OrderType ='EXOrder'
select cast(cast('1.2344 ' as float) as decimal(18,2))
update SAP_Interface_PPMES0005 Set TailQty =cast(cast(TailQty as float) as decimal(18,8))
Select top 1000 * from SAP_Interface_PPMES0001
Select top 1000 * from SAP_Interface_SDMES0001 
Select top 1000 * from SAP_Interface_PPMES0004
Select top 1000 * from SAP_Interface_PPMES0001 where TYPE is not null
Select top 1000 * from SAP_Interface_PPMES0006
Select top 1000 * from SAP_Interface_ExscraptDet
Select top 1000 * from SAP_Interface_ExscraptMstr
Select top 1000 * from SAP_Interface_AdjustDet
Select top 1000 * from SAP_Interface_AdjustMstr
Select REPLACE(newid(),'-','')--5CF70090D3B343F2A0A74D6A0FDA3267

--EXEC USP_SAP_PP_ExportMIOrder '5CF70090D3B343F2A0A74D6A0FDA3267','2014-05-04 00:00:00','2014-05-04 08:00:00'
--EXEC USP_SAP_PP_ExportEXOrder '8E0D310428D6479CB6BFE9B8FF9FD3C1','2014-05-04 00:00:00','2014-05-04 08:00:00'
--EXEC USP_SAP_PP_ExportFIOrder  '5CF70090D3B343F2A0A74D6A0FDA3267','2014-05-04 00:00:00','2014-05-04 08:00:00'
--EXEC USP_SAP_PP_ExportMIFilterOrder  'F5885A238FCB453B8B013ACB3240C9B2','2014-05-01 00:00:00','2014-05-09 08:00:00'
--EXEC USP_SAP_PP_ExportEXScraptOrder  'E75435B65C5A4E50B53E34B7E5088AE2','2014-05-13 09:00:00','2014-05-14 09:00:00'
--Select top 1000 * from ORD_MiscOrderMstr 
--EXEC USP_SAP_PP_ExportEXScraptOrder '631E26C5DD5C4DF6BA9F08404A94A14D','2014-03-21','2014-03-25'
Declare @Date Datetime='2014-03-01 00:00:00.000'
Declare @DateTo Datetime
Declare @batchno varchar(50)
while @Date<'2014-04-01 00:00:00.000'
	Begin
		Select @batchno=REPLACE(newid(),'-','')
		Select @DateTo=DATEADD(hour,+8,@Date)
		EXEC  dbo.USP_SAP_PP_GenEXSAPOrder @batchno, @Date,@DateTo
		Select @batchno,@Date,@DateTo
		
		Set @Date=DATEADD(hour,+8,@Date)
		waitfor delay '00:00:01:010'
	End
Select top 1000 * from SAP_EX001 

--Update SAP_TransTimeCtrl Set SysCode ='BusinessData',LastTransDate ='2014-05-01 00:00:00',CurrTransDate='2014-05-01 00:00:00' where SysCode ='ExportBusinessOrder'
Select top 1000 * from SAP_TransTimeCtrl 

--update SAP_TransTimeCtrl
--Set CurrTransDate ='2014-05-13 09:00:00' from SAP_TransTimeCtrl  
--where SysCode in ('PPMES0003')
select top 1000 * from SAP_Interface_PPMES0003 where ZComnum in ( select top 1000 * from SAP_Interface_PPMES0003 where ZPTYPE ='BUSCX02')
and Id not in (41,42,43,44)
select top 1000 * from SAP_EX001 where PlanNo ='OE141830085000'
select top 1000 * from CUST_MiscOrderMoveType
 --select sum( cast(ERFMG_I As decimal(18,8))) from SAP_Interface_PPMES0001 where ordertype='adjustorder'
--and MATNR_I='270393'
select top 1000 * from SAP_TransTimeCtrl
----SIOfBusinessOrderDataJob
select top 1000 * from MD_Location where Code ='9201'
select top 1000 * from MD_Item where Code ='270457'  
select top 1000 * from PRD_BomDet  where Bom  ='270457'
--201472/4006  Select 2220 -36680.736 /270457
select top 1000 * from VIEW_LocTrans where Item ='270457' and OrderNo like 'O4MI%' and CreateDate between '2014-04-01' and '2014-05-02'
select top 1000 * from VIEW_OrderDet where Item ='270457' and OrderNo like 'O4%' order by CreateDate desc

select top 1000 * from VIEW_RecDet  where Item ='270457' and OrderNo like 'O4MI%'  and LocTo ='9101'
select top 1000 * from SCM_FlowMstr where ResourceGroup in (10,20,30)
Truncate table MD_switchtrading
--Truncate table   SAP_Interface_SDMES0001  

--Delete from SAP_Interface_SDMES0001 where ZMESGUID like 'R2%'
--Update SAP_Interface_PPMES0004
--Set GRUND ='MES11' from SAP_Interface_PPMES0004 where BWART_H='102'
select top 1000 Flow ,* from ORD_RecMstr_2 where RecNo ='R200047101' 
select top 10000 * from SAP_Interface_PPMES0004 

------2014-05-22先采购 在库存移动 在生产 在销售按顺序传数据
 Select REPLACE(newid(),'-','')
 Select GETDATE()
EXEC dbo.USP_SAP_MM_ExportPurOrder '65548D5C529E4B51829906E271C22A08','2014-05-03 00:00:00.000','2014-05-04 00:00:00.000'
EXEC dbo.USP_SAP_MM_ExportTransOrder 'D55FA34491CF4D5FBAE4AF5499AD8BE2','2014-05-01 00:00:45.000','2014-05-02 09:00:27.000'


EXEC USP_SAP_PP_ExportMIOrder '83CEB2F8FDAD484C89D31A1A5ADD7EE4','2014-05-01 00:00:45.000','2014-05-02 09:00:27.000'
EXEC USP_SAP_PP_ExportEXOrder 'BDFB204843B14A178D168F340C3EBCAD','2014-05-01 00:00:45.000','2014-05-02 09:00:27.000'
EXEC USP_SAP_PP_ExportFIOrder  '1FEE3BA86AB4490AB984CA662FC9BB56','2014-05-01 00:00:45.000','2014-05-02 09:00:27.000'
EXEC USP_SAP_PP_ExportMIFilterOrder  '6F239621332949E9945DFC694450CD1C','2014-05-01 00:00:45.000','2014-05-02 09:00:27.000'
EXEC USP_SAP_PP_ExportEXScraptOrder  '7A757FFC756C4A47B65FD2EE8D43F69F','2014-05-01 00:00:45.000','2014-05-02 09:00:27.000'
EXEC USP_SAP_PP_ExportReworkOrder  'B35066DD37894CE394E184C2931989B8','2014-05-01 00:00:45.000','2014-05-02 09:00:27.000'



EXEC USP_SAP_ExportSalesOrder  'B3E7F61F39584A7FB8145CA58EB0E273','2014-05-01 00:00:45.000','2014-05-02 09:00:27.000'



Select top 1000 * from VIEW_LocTrans where RecNo ='R200053893'
select * from VIEW_RecMstr where RecNo in (
Select LFSNR from SAP_Interface_PPMES0001)



Select top 1000 * from SAP_Interface_STMES0001 a  
where  Exists (Select matnr from SAP_Interface_MMMES0001 b 
where a.matnr1=b.matnr and(a.LGORT=b.LGORT or a.UMLGO=b.LGORT) ) 

---移库来源库位
--Drop table #移库出
--Drop table #移库入
Select matnr1 As Item ,LGORT As SAPLocation,SUM(Cast(EPFMG As Float)) As Qty into #移库出 from SAP_Interface_STMES0001 a  
where  Exists (Select matnr from SAP_Interface_MMMES0001 b 
where a.matnr1=b.matnr and(a.LGORT=b.LGORT) ) Group by LGORT,matnr1

---移库目的库位
Select matnr1 As Item ,UMLGO As SAPLocation,SUM(Cast(EPFMG As Float)) As Qty into #移库入 from SAP_Interface_STMES0001 a  
where  Exists (Select matnr from SAP_Interface_MMMES0001 b 
where a.matnr1=b.matnr and(a.UMLGO=b.LGORT) ) Group by UMLGO,matnr1


Select Item,  SAPLocation,SUM(totalqty) as QTY into #temp from Inv_RecSIExecution
group by Item,  SAPLocation
Update a
Set a.QTY=a.QTY -b.Qty  from #temp a ,#移库出 b where a.Item=b.Item and a.SAPLocation=b.SAPLocation
Update a
Set a.QTY=a.QTY +b.Qty  from #temp a ,#移库入 b where a.Item=b.Item and a.SAPLocation=b.SAPLocation


Select Item,  SAPLocation,SUM(QTY) from #temp where Item in (Select matnr from SAP_Interface_MMMES0001) Group by Item,  SAPLocation

Select distinct matnr from SAP_Interface_MMMES0001
Select * from SAP_Interface_SDMES0001

