insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values('HuTemplate','BarCodeFG.xls','BarCodeFinishGoods',0,2)

update SYS_CodeDet set Seq=1 where Value='BarCodePurchase.xls'


alter table md_pallet alter column Desc1 varchar(100)