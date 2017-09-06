
-------------------------1.MasterData-----------------------------


Create Table SAP_Item(
	Id		int identity(1,1) primary key,	
	MATNR	varchar(18),
	BISMT	varchar(18),
	MAKTX	varchar(40),
	MEINS	varchar(3),
	WERKS	varchar(4),
	MTART	varchar(4),
	MTBEZ	varchar(25),
	MATKL	varchar(9),
	WGBEZ	varchar(20),
	SPART	varchar(10),
	LVORM	varchar(1),
	SOBSL	varchar(2),
	BatchNo varchar(50),
	Status	int,
	CreateDate datetime
) ---entity SAPItem

Create Table SAP_Bom
(
	Id		int identity(1,1) primary key,
	MATNR	varchar(18),
	MAKTX	varchar(60),
	BMENG	varchar(13),
	BMEIN	varchar(3),
	IDNRK	varchar(18),
	MEINS	varchar(3),
	MENGE	varchar(13),
	AUSCH	varchar(5),
	BatchNo varchar(50),
	Status	int,
	CreateDate datetime
)---entity SAPBom

Create Table SAP_UomConv
(
	Id		int identity(1,1) primary key,
	MATNR	varchar(18),
	MEINS	varchar(3),
	MEINH	varchar(3),
	UMREZ	varchar(5),
	UMREN	varchar(5),
	BatchNo varchar(50),
	Status	int,
	CreateDate datetime
)---entity SAPUomConvertion

Create Table SAP_PriceList
(
	Id		int identity(1,1) primary key,
	LIFNR	varchar(10),
	WAERS	varchar(5),
	MATNR	varchar(18),
	BPRME	varchar(3),
	NETPR	varchar(11),
	MWSKZ	varchar(2),
	ERDAT	varchar(8),
	PRDAT	varchar(8),
	NORMB	varchar(5),
	BatchNo varchar(50),
	Status	int,
	CreateDate datetime
)---entity SAPPriceList

Create Table SAP_Supplier
(
	Id		int identity(1,1) primary key,
	LIFNR	varchar(10),
	NAME1	varchar(35),
	LAND1LANDX	varchar(18),
	REGIOBEZEI	varchar(23),
	REMARK	varchar(50),
	TELF2	varchar(16),
	TELFX	varchar(31),
	PARNR	varchar(10),
	PSTLZ	varchar(10),
	TELBX	varchar(15),
	TELF1	varchar(16),
	EKGRP	varchar(3),
	LOEVM	varchar(1),
	BatchNo varchar(50),
	Status	int,
	CreateDate datetime
)---entity SAPSupplier

Create Table SAP_Customer
(
	Id		int identity(1,1) primary key,
	KUNNR	varchar(10),
	NAME1GP	varchar(35),
	LAND1GPLANDX	varchar(18),
	REGIOBEZEI20	varchar(23),
	AD_REMARK1	varchar(50),
	TELF2	varchar(16),
	TELFX	varchar(31),
	PARNR	varchar(10),
	PSTLZ	varchar(10),
	TELBX	varchar(15),
	TELF1	varchar(16),
	LOEVM_B	varchar(1),
	BatchNo varchar(50),
	Status	int,
	CreateDate datetime
)---entity SAPCustomer
-------------------------1.MasterData-----------------------------



-------------------------2.MM-----------------------------
Create Table SAP_PurchaseOrder
(
	Id		int identity(1,1) primary key,
	ZMESPO	varchar(20),
	LIFNR	varchar(10),
	BSART	varchar(4),
	EKORG	varchar(4),
	EKGRP	varchar(3),
	BUKRS	varchar(4),
	BWART_H	varchar(3),
	LFSNR	varchar(16),	
	BUDAT	varchar(8),
	BLDAT	varchar(8),
	EBELP_I	varchar(5),
	EPSTP	varchar(1),
	MATNR	varchar(18),
	TARGET_QTY_I	varchar(13),
	NETPR	varchar(13),
	PEINH	varchar(5),
	WAERS	varchar(5),
	BPRME_I	varchar(3),
	EINDT	varchar(8),
	WERKS	varchar(4),
	LGORT	varchar(4),
	RETPO	varchar(1),
	BWART_C	varchar(3),
	MATNR_C	varchar(18),
	TARGET_QTY_C	varchar(13),
	BPRME_C	varchar(3),
	ZMESGUID	varchar(16),
	ZCSRQSJ	varchar(14),
	CreateDate datetime,
	Status int,
	BatchNo varchar(50)
)


-------------------------2.MM-----------------------------

CREATE TABLE SAP_TransTimeCtrl
(
	SysCode varchar(50) primary key,
	LastTransDate datetime,
	CurrTransDate datetime
)

CREATE TABLE SAP_TransLog
(
	Id int identity(1,1) primary key,
	BatchNo varchar(50),
	SysCode varchar(50),
	Interface varchar(50),
	Status int,
	ErrorMsg varchar(max),
	TransDate datetime
)



select om.RecNo,od.Seq,'ZKB','0101','10','10','',om.EffDate,om.EffDate,om.PartyTo,om.CreateDate,
om.ExtRecNo,od.Item,od.RecQty,od.Uom,od.LocFrom,NEWID(),GETDATE()
from ORD_RecMstr_3 om inner join ORD_RecDet_3 od on om.RecNo=od.RecNo 
where om.CreateDate between '2014-01-25' and '2014-02-26' and om.OrderSubType=0

select om.RecNo,od.Seq,'ZKA','0101','10','10','',om.EffDate,om.EffDate,om.PartyTo,om.CreateDate,
om.ExtRecNo,od.Item,od.RecQty,od.Uom,od.LocTo,NEWID(),GETDATE()
from ORD_RecMstr_3 om inner join ORD_RecDet_3 od on om.RecNo=od.RecNo 
where om.CreateDate between '2014-01-25' and '2014-02-26' and om.OrderSubType=1

