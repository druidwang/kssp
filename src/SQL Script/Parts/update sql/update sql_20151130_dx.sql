alter table WMS_ShipPlan add IsShipScanHu bit not null
go
alter table WMS_PickTask add IsPickHu bit not null
go
alter table MD_Location add IsSpread bit default(0) not null
go