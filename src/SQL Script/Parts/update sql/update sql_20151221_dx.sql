alter table WMS_PickOccupy alter column TargetDock varchar(50) null
go
alter table WMS_BuffOccupy alter column TargetDock varchar(50) null
go
alter table WMS_RepackOccupy alter column TargetDock varchar(50) null
go

CREATE TYPE PickResultTableType AS TABLE 
(
	PickTaskId varchar(50) not null,
	HuId int null,
	Qty decimal(18,8)
)
GO

alter table WMS_PickResult alter column PickTaskId varchar(50) not null
go
alter table WMS_PickResult drop column PickBy
go
alter table WMS_PickOccupy add ReleaseQty decimal(18, 8)
go
alter table WMS_RepackOccupy add ReleaseQty decimal(18, 8)
go


