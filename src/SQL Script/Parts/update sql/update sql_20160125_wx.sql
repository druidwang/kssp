alter table TMS_OrderMstr add IsValuated bit default 0
go
alter table TMS_OrderMstr add Expense varchar(50)
go

alter table TMS_PriceList add Currency varchar(50)
go

alter table TMS_Ordermstr add PricingMethod varchar(50)
go

alter table tms_pricelistdet add MinVolume decimal(18,8)
go

alter table tms_pricelistdet add MinWeight decimal(18,8)
go

alter table tms_pricelist add Tax varchar(50)
go

alter table tms_pricelist add IsIncludeTax bit
go

alter table tms_actbill add PriceListDetail int
go