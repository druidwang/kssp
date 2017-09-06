Create Type CreatePickTaskTableType as table 
(
	Id int primary key,
	PickQty decimal(18, 8)
)
go

alter table WMS_ShipPlan add PickQty decimal(18, 8)
go

alter table WMS_ShipPlan add Priority bit
go

alter table WMS_PickTask add IsPickHu bit
go

alter table MD_Location add PickBy bit
go

alter table WMS_ShipPlan add PickedQty decimal(18, 8)
go