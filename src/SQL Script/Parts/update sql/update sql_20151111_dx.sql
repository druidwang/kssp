alter table TMS_OrderRoute add IsArrive bit
go
alter table TMS_OrderRoute drop column OrderRouteId
go
alter table TMS_OrderRoute add OrderRouteFrom int
go
alter table TMS_OrderRoute add OrderRouteTo int
go