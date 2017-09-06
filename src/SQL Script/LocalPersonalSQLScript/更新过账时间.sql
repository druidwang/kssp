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
Delete SAP_EX001 where RecTime >'2014-08-01'
Truncate table SAP_Interface_ExscraptDet 
Truncate table SAP_Interface_ExscraptMstr
Truncate table SAP_Interface_AdjustDet 
Truncate table SAP_Interface_AdjustMstr 
--Insert into SAP_EX001
--Select * from Sconit_20140609.dbo.SAP_EX001
Select top 1000 * from ORD_orderMstr_4
SP_help'SAP_Interface_PPMES0001'
--更新过账时间
Select REPLACE(REPLACE(replace(replace('2014-07-11 14:10:41.383','-',''),'.',''),':',''),' ','')

Declare @Date DateTime='2014-08-06 00:00:00.000'
Update SAP_Interface_MMMES0001 Set Status =0,BLDAT =@Date,BUDAT =@Date
Update SAP_Interface_STMES0001 Set Status =0,BLDAT =@Date,BUDAT =@Date
Update SAP_Interface_SDMES0001  Set Status =0,PRICEDATE =@Date,DOCDATE =@Date,WADATIST =@Date 

Update  SAP_Interface_PPMES0001 Set Status =0,BLDAT =@Date,BUDAT =@Date,GLTRP=@Date,GSTRP =@Date

Update  SAP_Interface_PPMES0003 Set Status =0,BLDAT =@Date,BUDAT =@Date
Update  SAP_Interface_PPMES0004 Set Status =0,BLDAT =@Date,BUDAT =@Date
Update  SAP_Interface_PPMES0005 Set Status =0,BLDAT =@Date,BUDAT =@Date,GLTRP=@Date,GSTRP =@Date

Update  SAP_Interface_PPMES0006 Set Status =0,BLDAT =@Date,BUDAT =@Date
		Update a
			Set a.SPART =b.Division from SAP_Interface_STMES0001 a,MD_Item b 
		where  a.MATNR1 =b.Code
--请在导入五号数据前,把所有的销售路线的分销组织/分销组织  都改为0101 /10 ; 
Update SAP_Interface_SDMES0001
Set SALESORG='0101',DISTRCHAN='10' from SAP_Interface_SDMES0001  

Select REPLACE(REPLACE(replace(replace('2014-07-11 14:10:40.410','-',''),'.',''),':',''),' ','')

--Update SAP_Interface_PPMES0001
--Set Status =0 where Id <8959

Select 'PP001' AS Typess,dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000') As GUIDS,COUNT(0) AS 明细数量 
from SAP_Interface_PPMES0001 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000') union all
Select'PP002', dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000'),COUNT(0) AS 明细数量 
from SAP_Interface_PPMES0002 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000')union all
Select'PP003', dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000'),COUNT(0) AS 明细数量 
from SAP_Interface_PPMES0003 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000')union all
Select 'PP004',dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000'),COUNT(0) AS 明细数量 
from SAP_Interface_PPMES0004 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000')union all
Select 'PP005',dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000'),COUNT(0) AS 明细数量 
from SAP_Interface_PPMES0005 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000')union all
Select 'PP006',dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000'),COUNT(0) AS 明细数量 
from SAP_Interface_PPMES0006 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS0000')union all

Select 'MM001',dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS'),COUNT(0) AS 明细数量 
from SAP_Interface_MMMES0001 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS')union all
Select 'MM002',dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS'),COUNT(0) AS 明细数量 
from SAP_Interface_MMMES0002 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS')union all
Select 'ST001',dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS'),COUNT(0) AS 明细数量 
from SAP_Interface_STMES0001 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS')union all
Select 'SD001',dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS'),COUNT(0) AS 明细数量 
from SAP_Interface_SDMES0001 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS')union all
Select 'SD002',dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS'),COUNT(0) AS 明细数量 
from SAP_Interface_SDMES0002 Group by  dbo.FormatDate(ZCSRQSJ,'YYYYMMMDDHHNNSS')
--Select * from SAP_Interface_SDMES0001
--Select Status,* from SAP_Interface_STMES0001
--Select Status,* from SAP_Interface_MMMES0001
--Select Status,* from SAP_Interface_MMMES0002
--Select Status,* from SAP_Interface_PPMES0001 where Status=0
--Select Status,* from SAP_Interface_PPMES0002
--Select Status,* from SAP_Interface_PPMES0003
--Select * from SAP_Interface_ExscraptDet
--Select * from SAP_Interface_ExscraptMstr
--Select Status,* from SAP_Interface_PPMES0004
--Select Status,* from SAP_Interface_PPMES0005
--Select Status,* from SAP_Interface_PPMES0006
--Select Status,* from SAP_Interface_SDMES0001  
--Select Status,* from SAP_Interface_SDMES0002  
 SP_Help'SAP_Interface_SDMES0001'
Select COUNT(0) from VIEW_OrderDet where CreateDate between '2014-06-01' and '2014-06-02 00:00:00'
select distinct  b.name,a.text from syscomments a,sys.objects b where a.id=b.object_id --and b.name like 'usp%'
and type='P' and a.text like '%SAP_Interface_SDMES0001%'


--EXEC USP_SAP_SaveInvForSIExecution 'euhifsnfswnfisnwjnonj','2014-06-06','2014-06-07'

--Select dbo.FormatDate(GETDATE(),'YYYY-MM-DD 00:00:00')
 Declare @BatchNo varchar(200) =replace(Newid(),'-','')
 Declare @StartTime DateTime = '2014-08-06 09:00:00'
 Declare @EndTime DateTime = '2014-08-07'
Set @BatchNo =replace(Newid(),'-','')
Select @BatchNo,  @StartTime,  @EndTime
EXEC USP_SAP_SD_ExportSalesOrder @BatchNo,  @StartTime,  @EndTime
Set @BatchNo =replace(Newid(),'-','')
EXEC USP_SAP_MM_ExportPurOrder @BatchNo,  @StartTime,  @EndTime
Set @BatchNo =replace(Newid(),'-','')
EXEC USP_SAP_MM_ExportTransOrder @BatchNo,  @StartTime,  @EndTime
Set @BatchNo =replace(Newid(),'-','')
EXEC USP_SAP_PP_ExportMIOrder @BatchNo,  @StartTime,  @EndTime
Set @BatchNo =replace(Newid(),'-','')
EXEC USP_SAP_PP_ExportMIFilterOrder @BatchNo,  @StartTime,  @EndTime
Set @BatchNo =replace(Newid(),'-','')
EXEC USP_SAP_PP_ExportEXOrder @BatchNo,  @StartTime,  @EndTime
Set @BatchNo =replace(Newid(),'-','')
EXEC USP_SAP_PP_ExportEXScraptOrder @BatchNo,  @StartTime,  @EndTime
Set @BatchNo =replace(Newid(),'-','')
EXEC USP_SAP_PP_ExportFIOrder @BatchNo,  @StartTime,  @EndTime
Set @BatchNo =replace(Newid(),'-','')
EXEC USP_SAP_PP_ExportReworkOrder @BatchNo,  @StartTime,  @EndTime
Set @BatchNo =replace(Newid(),'-','')
EXEC USP_SAP_PP_ExportTrailMiscOrder @BatchNo,  @StartTime,  @EndTime

return
Select Flow,Status,* from ORD_MiscOrderMstr  where  SubType ='30'
Select  * from ORD_MiscOrderDet  where MiscOrderNo in(
Select MiscOrderNo from ORD_MiscOrderMstr  where  SubType ='30'
and Flow like 'FI%')
Update ORD_MiscOrderMstr 
  Set WBS ='5002263/0010' from ORD_MiscOrderMstr where MiscOrderNo ='MI00000416'
Select  * from SAP_Interface_PPMES0006 where MATNR1='270542'

Select  * from SAP_Interface_PPMES0001 where MATNR_I ='200423' and LGORT_I ='4002'
Select  * from MD_Item where Code ='200003'
Select  * from MD_ItemCategory 
	where Code ='200003'
Select  * from SAP_Interface_STMES0001 where MATNR1 ='200003'

Select OrderNo ,RecNo ,Bom ,Item ,BaseUom ,SUM(bfqty*unitqty) from ORD_OrderBackflushDet where OrderNo ='O4FI3100000888'
Group by OrderNo ,RecNo ,Bom ,Item,BaseUom 



 