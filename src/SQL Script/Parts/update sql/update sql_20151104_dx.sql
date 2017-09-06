alter table TMS_Driver add LicenseNo varchar(50)
go
alter table TMS_OrderMstr drop column DrivingNo
go
alter table TMS_OrderMstr add LicenseNo varchar(50)
go
alter table TMS_Vehicle drop column Owner
go
alter table TMS_Vehicle add Carrier varchar(50)
go