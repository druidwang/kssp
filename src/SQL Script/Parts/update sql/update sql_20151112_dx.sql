alter table TMS_FlowMstr add MultiSitePick bit
go
alter table TMS_OrderMstr add MultiSitePick bit
go
alter table TMS_Driver alter column CreateUser int not null
go
alter table TMS_Driver alter column LastModifyUser int not null
go
alter table TMS_FlowCarrier alter column CreateUser int not null
go
alter table TMS_FlowCarrier alter column LastModifyUser int not null
go
alter table TMS_FlowMstr alter column CreateUser int not null
go
alter table TMS_FlowMstr alter column LastModifyUser int not null
go
alter table TMS_FlowRoute alter column CreateUser int not null
go
alter table TMS_FlowRoute alter column LastModifyUser int not null
go
alter table TMS_Mileage alter column CreateUser int not null
go
alter table TMS_Mileage alter column LastModifyUser int not null
go
alter table TMS_OrderDet alter column CreateUser int not null
go
alter table TMS_OrderDet alter column LastModifyUser int not null
go
alter table TMS_OrderMstr alter column CreateUser int not null
go
alter table TMS_OrderMstr alter column LastModifyUser int not null
go
alter table TMS_OrderMstr alter column SubmitUser int not null
go
alter table TMS_OrderMstr alter column StartUser int not null
go
alter table TMS_OrderMstr alter column CloseUser int not null
go
alter table TMS_OrderMstr alter column CancelUser int not null
go
alter table TMS_OrderRoute alter column CreateUser int not null
go
alter table TMS_OrderRoute alter column LastModifyUser int not null
go
alter table TMS_PriceList alter column CreateUser int not null
go
alter table TMS_PriceList alter column LastModifyUser int not null
go
alter table TMS_PriceListDet alter column CreateUser int not null
go
alter table TMS_PriceListDet alter column LastModifyUser int not null
go
alter table TMS_Tonnage alter column CreateUser int not null
go
alter table TMS_Tonnage alter column LastModifyUser int not null
go
alter table TMS_Vehicle alter column CreateUser int not null
go
alter table TMS_Vehicle alter column LastModifyUser int not null
go