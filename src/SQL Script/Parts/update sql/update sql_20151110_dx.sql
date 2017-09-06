alter table TMS_OrderMstr drop column IsAutoSubmit
go
alter table TMS_OrderMstr add IsAutoRelease bit
go
alter table SCM_FlowDet add PalletLotSize decimal(18,8)
go
alter table SCM_FlowDet add PackageVolumn decimal(18,8)
go
alter table SCM_FlowDet add PackageWeight decimal(18,8)
go
alter table MD_Item add PalletLotSize decimal(18,8)
go
alter table MD_Item add PackageVolumn decimal(18,8)
go
alter table MD_Item add PackageWeight decimal(18,8)
go
alter table ORD_OrderDet add PalletLotSize decimal(18,8)
alter table ORD_OrderDet_0 add PalletLotSize decimal(18,8)
alter table ORD_OrderDet_1 add PalletLotSize decimal(18,8)
alter table ORD_OrderDet_2 add PalletLotSize decimal(18,8)
alter table ORD_OrderDet_3 add PalletLotSize decimal(18,8)
alter table ORD_OrderDet_4 add PalletLotSize decimal(18,8)
alter table ORD_OrderDet_5 add PalletLotSize decimal(18,8)
alter table ORD_OrderDet_6 add PalletLotSize decimal(18,8)
alter table ORD_OrderDet_7 add PalletLotSize decimal(18,8)
alter table ORD_OrderDet_8 add PalletLotSize decimal(18,8)
go
alter table ORD_OrderDet add PackageVolumn decimal(18,8)
alter table ORD_OrderDet_0 add PackageVolumn decimal(18,8)
alter table ORD_OrderDet_1 add PackageVolumn decimal(18,8)
alter table ORD_OrderDet_2 add PackageVolumn decimal(18,8)
alter table ORD_OrderDet_3 add PackageVolumn decimal(18,8)
alter table ORD_OrderDet_4 add PackageVolumn decimal(18,8)
alter table ORD_OrderDet_5 add PackageVolumn decimal(18,8)
alter table ORD_OrderDet_6 add PackageVolumn decimal(18,8)
alter table ORD_OrderDet_7 add PackageVolumn decimal(18,8)
alter table ORD_OrderDet_8 add PackageVolumn decimal(18,8)
go
alter table ORD_OrderDet add PackageWeight decimal(18,8)
alter table ORD_OrderDet_0 add PackageWeight decimal(18,8)
alter table ORD_OrderDet_1 add PackageWeight decimal(18,8)
alter table ORD_OrderDet_2 add PackageWeight decimal(18,8)
alter table ORD_OrderDet_3 add PackageWeight decimal(18,8)
alter table ORD_OrderDet_4 add PackageWeight decimal(18,8)
alter table ORD_OrderDet_5 add PackageWeight decimal(18,8)
alter table ORD_OrderDet_6 add PackageWeight decimal(18,8)
alter table ORD_OrderDet_7 add PackageWeight decimal(18,8)
alter table ORD_OrderDet_8 add PackageWeight decimal(18,8)
go

alter table ORD_IpDet add PalletLotSize decimal(18,8)
alter table ORD_IpDet_0 add PalletLotSize decimal(18,8)
alter table ORD_IpDet_1 add PalletLotSize decimal(18,8)
alter table ORD_IpDet_2 add PalletLotSize decimal(18,8)
alter table ORD_IpDet_3 add PalletLotSize decimal(18,8)
alter table ORD_IpDet_4 add PalletLotSize decimal(18,8)
alter table ORD_IpDet_5 add PalletLotSize decimal(18,8)
alter table ORD_IpDet_6 add PalletLotSize decimal(18,8)
alter table ORD_IpDet_7 add PalletLotSize decimal(18,8)
alter table ORD_IpDet_8 add PalletLotSize decimal(18,8)
go
alter table ORD_IpDet add PackageVolumn decimal(18,8)
alter table ORD_IpDet_0 add PackageVolumn decimal(18,8)
alter table ORD_IpDet_1 add PackageVolumn decimal(18,8)
alter table ORD_IpDet_2 add PackageVolumn decimal(18,8)
alter table ORD_IpDet_3 add PackageVolumn decimal(18,8)
alter table ORD_IpDet_4 add PackageVolumn decimal(18,8)
alter table ORD_IpDet_5 add PackageVolumn decimal(18,8)
alter table ORD_IpDet_6 add PackageVolumn decimal(18,8)
alter table ORD_IpDet_7 add PackageVolumn decimal(18,8)
alter table ORD_IpDet_8 add PackageVolumn decimal(18,8)
go
alter table ORD_IpDet add PackageWeight decimal(18,8)
alter table ORD_IpDet_0 add PackageWeight decimal(18,8)
alter table ORD_IpDet_1 add PackageWeight decimal(18,8)
alter table ORD_IpDet_2 add PackageWeight decimal(18,8)
alter table ORD_IpDet_3 add PackageWeight decimal(18,8)
alter table ORD_IpDet_4 add PackageWeight decimal(18,8)
alter table ORD_IpDet_5 add PackageWeight decimal(18,8)
alter table ORD_IpDet_6 add PackageWeight decimal(18,8)
alter table ORD_IpDet_7 add PackageWeight decimal(18,8)
alter table ORD_IpDet_8 add PackageWeight decimal(18,8)
go
