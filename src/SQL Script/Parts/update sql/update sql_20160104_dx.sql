alter table WMS_ShipPlan add OrderType tinyint
go
alter table TMS_OrderRoute drop column OrderRouteFrom
go
alter table TMS_OrderRoute drop column OrderRouteTo
go
alter table TMS_OrderDet add OrderRouteFrom int
go
alter table TMS_OrderDet add OrderRouteTo int
go