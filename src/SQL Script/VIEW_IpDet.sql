/****** Object:  View [dbo].[VIEW_IpDet]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='V' AND name='VIEW_IpDet')
BEGIN
	DROP VIEW VIEW_IpDet
END
GO

CREATE VIEW [dbo].[VIEW_IpDet]
AS
SELECT Id, IpNo, OrderDetId, Item, ItemDesc, RefItemCode, Uom, UC, QualityType, Qty, 
    RecQty, UnitQty, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, PriceList, 
    UnitPrice, Currency, IsProvEst, Tax, IsIncludeTax, BillTerm, IsClose, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Seq, OrderNo, OrderDetSeq, OrderType, OrderSubType, ManufactureParty, 
    BaseUom, Type, GapRecNo, GapIpDetId, UCDesc, Container, ContainerDesc, ExtNo, 
    ExtSeq, StartTime, WindowTime, BinTo, IsScanHu, IsChangeUC, Flow, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_IpDet_1
UNION ALL
SELECT Id, IpNo, OrderDetId, Item, ItemDesc, RefItemCode, Uom, UC, QualityType, Qty, 
    RecQty, UnitQty, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, PriceList, 
    UnitPrice, Currency, IsProvEst, Tax, IsIncludeTax, BillTerm, IsClose, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Seq, OrderNo, OrderDetSeq, OrderType, OrderSubType, ManufactureParty, 
    BaseUom, Type, GapRecNo, GapIpDetId, UCDesc, Container, ContainerDesc, ExtNo, 
    ExtSeq, StartTime, WindowTime, BinTo, IsScanHu, IsChangeUC, Flow, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_IpDet_2
UNION ALL
SELECT Id, IpNo, OrderDetId, Item, ItemDesc, RefItemCode, Uom, UC, QualityType, Qty, 
    RecQty, UnitQty, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, PriceList, 
    UnitPrice, Currency, IsProvEst, Tax, IsIncludeTax, BillTerm, IsClose, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Seq, OrderNo, OrderDetSeq, OrderType, OrderSubType, ManufactureParty, 
    BaseUom, Type, GapRecNo, GapIpDetId, UCDesc, Container, ContainerDesc, ExtNo, 
    ExtSeq, StartTime, WindowTime, BinTo, IsScanHu, IsChangeUC, Flow, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_IpDet_3
UNION ALL
SELECT Id, IpNo, OrderDetId, Item, ItemDesc, RefItemCode, Uom, UC, QualityType, Qty, 
    RecQty, UnitQty, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, PriceList, 
    UnitPrice, Currency, IsProvEst, Tax, IsIncludeTax, BillTerm, IsClose, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Seq, OrderNo, OrderDetSeq, OrderType, OrderSubType, ManufactureParty, 
    BaseUom, Type, GapRecNo, GapIpDetId, UCDesc, Container, ContainerDesc, ExtNo, 
    ExtSeq, StartTime, WindowTime, BinTo, IsScanHu, IsChangeUC, Flow, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_IpDet_4
UNION ALL
SELECT Id, IpNo, OrderDetId, Item, ItemDesc, RefItemCode, Uom, UC, QualityType, Qty, 
    RecQty, UnitQty, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, PriceList, 
    UnitPrice, Currency, IsProvEst, Tax, IsIncludeTax, BillTerm, IsClose, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Seq, OrderNo, OrderDetSeq, OrderType, OrderSubType, ManufactureParty, 
    BaseUom, Type, GapRecNo, GapIpDetId, UCDesc, Container, ContainerDesc, ExtNo, 
    ExtSeq, StartTime, WindowTime, BinTo, IsScanHu, IsChangeUC, Flow, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_IpDet_5
UNION ALL
SELECT Id, IpNo, OrderDetId, Item, ItemDesc, RefItemCode, Uom, UC, QualityType, Qty, 
    RecQty, UnitQty, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, PriceList, 
    UnitPrice, Currency, IsProvEst, Tax, IsIncludeTax, BillTerm, IsClose, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Seq, OrderNo, OrderDetSeq, OrderType, OrderSubType, ManufactureParty, 
    BaseUom, Type, GapRecNo, GapIpDetId, UCDesc, Container, ContainerDesc, ExtNo, 
    ExtSeq, StartTime, WindowTime, BinTo, IsScanHu, IsChangeUC, Flow, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_IpDet_6
UNION ALL
SELECT Id, IpNo, OrderDetId, Item, ItemDesc, RefItemCode, Uom, UC, QualityType, Qty, 
    RecQty, UnitQty, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, PriceList, 
    UnitPrice, Currency, IsProvEst, Tax, IsIncludeTax, BillTerm, IsClose, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Seq, OrderNo, OrderDetSeq, OrderType, OrderSubType, ManufactureParty, 
    BaseUom, Type, GapRecNo, GapIpDetId, UCDesc, Container, ContainerDesc, ExtNo, 
    ExtSeq, StartTime, WindowTime, BinTo, IsScanHu, IsChangeUC, Flow, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_IpDet_7
UNION ALL
SELECT Id, IpNo, OrderDetId, Item, ItemDesc, RefItemCode, Uom, UC, QualityType, Qty, 
    RecQty, UnitQty, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, PriceList, 
    UnitPrice, Currency, IsProvEst, Tax, IsIncludeTax, BillTerm, IsClose, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Seq, OrderNo, OrderDetSeq, OrderType, OrderSubType, ManufactureParty, 
    BaseUom, Type, GapRecNo, GapIpDetId, UCDesc, Container, ContainerDesc, ExtNo, 
    ExtSeq, StartTime, WindowTime, BinTo, IsScanHu, IsChangeUC, Flow, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_IpDet_8
UNION ALL
SELECT Id, IpNo, OrderDetId, Item, ItemDesc, RefItemCode, Uom, UC, QualityType, Qty, 
    RecQty, UnitQty, LocFrom, LocFromNm, LocTo, LocToNm, IsInspect, BillAddr, PriceList, 
    UnitPrice, Currency, IsProvEst, Tax, IsIncludeTax, BillTerm, IsClose, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Seq, OrderNo, OrderDetSeq, OrderType, OrderSubType, ManufactureParty, 
    BaseUom, Type, GapRecNo, GapIpDetId, UCDesc, Container, ContainerDesc, ExtNo, 
    ExtSeq, StartTime, WindowTime, BinTo, IsScanHu, IsChangeUC, Flow, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_IpDet_0
GO