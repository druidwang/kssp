/****** Object:  View [dbo].[VIEW_OrderDet]    Script Date: 07/05/2012 14:55:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='V' AND name='VIEW_OrderDet')
BEGIN
	DROP VIEW VIEW_OrderDet
END
GO

CREATE VIEW [dbo].[VIEW_OrderDet]
AS
SELECT Id, OrderNo, Seq, ExtNo, ExtSeq, StartDate, EndDate, ScheduleType, Item, ItemDesc, 
    RefItemCode, Uom, UC, Container, QualityType, ReqQty, OrderQty, ShipQty, RecQty, 
    RejQty, ScrapQty, PickQty, UnitQty, RecLotSize, LocFrom, LocFromNm, LocTo, LocToNm, 
    IsInspect, BillAddr, BillAddrDesc, PriceList, UnitPrice, IsProvEst, Tax, IsIncludeTax, Bom, 
    Routing, BillTerm, ReserveNo, ReserveLine, ZOPWZ, ZOPID, ZOPDS, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Currency, OrderType, ManufactureParty, OrderSubType, BaseUom, 
    PickStrategy, ExtraDmdSource, UCDesc, MinUC, ContainerDesc, IsScanHu, BinTo, 
    WMSSeq, IsChangeUC, AUFNR, ICHARG, BWART, Direction, Remark, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_OrderDet_1
UNION ALL
SELECT Id, OrderNo, Seq, ExtNo, ExtSeq, StartDate, EndDate, ScheduleType, Item, ItemDesc, 
    RefItemCode, Uom, UC, Container, QualityType, ReqQty, OrderQty, ShipQty, RecQty, 
    RejQty, ScrapQty, PickQty, UnitQty, RecLotSize, LocFrom, LocFromNm, LocTo, LocToNm, 
    IsInspect, BillAddr, BillAddrDesc, PriceList, UnitPrice, IsProvEst, Tax, IsIncludeTax, Bom, 
    Routing, BillTerm, ReserveNo, ReserveLine, ZOPWZ, ZOPID, ZOPDS, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Currency, OrderType, ManufactureParty, OrderSubType, BaseUom, 
    PickStrategy, ExtraDmdSource, UCDesc, MinUC, ContainerDesc, IsScanHu, BinTo, 
    WMSSeq, IsChangeUC, AUFNR, ICHARG, BWART, Direction, Remark, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_OrderDet_2
UNION ALL
SELECT Id, OrderNo, Seq, ExtNo, ExtSeq, StartDate, EndDate, ScheduleType, Item, ItemDesc, 
    RefItemCode, Uom, UC, Container, QualityType, ReqQty, OrderQty, ShipQty, RecQty, 
    RejQty, ScrapQty, PickQty, UnitQty, RecLotSize, LocFrom, LocFromNm, LocTo, LocToNm, 
    IsInspect, BillAddr, BillAddrDesc, PriceList, UnitPrice, IsProvEst, Tax, IsIncludeTax, Bom, 
    Routing, BillTerm, ReserveNo, ReserveLine, ZOPWZ, ZOPID, ZOPDS, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Currency, OrderType, ManufactureParty, OrderSubType, BaseUom, 
    PickStrategy, ExtraDmdSource, UCDesc, MinUC, ContainerDesc, IsScanHu, BinTo, 
    WMSSeq, IsChangeUC, AUFNR, ICHARG, BWART, Direction, Remark, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_OrderDet_3
UNION ALL
SELECT Id, OrderNo, Seq, ExtNo, ExtSeq, StartDate, EndDate, ScheduleType, Item, ItemDesc, 
    RefItemCode, Uom, UC, Container, QualityType, ReqQty, OrderQty, ShipQty, RecQty, 
    RejQty, ScrapQty, PickQty, UnitQty, RecLotSize, LocFrom, LocFromNm, LocTo, LocToNm, 
    IsInspect, BillAddr, BillAddrDesc, PriceList, UnitPrice, IsProvEst, Tax, IsIncludeTax, Bom, 
    Routing, BillTerm, ReserveNo, ReserveLine, ZOPWZ, ZOPID, ZOPDS, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Currency, OrderType, ManufactureParty, OrderSubType, BaseUom, 
    PickStrategy, ExtraDmdSource, UCDesc, MinUC, ContainerDesc, IsScanHu, BinTo, 
    WMSSeq, IsChangeUC, AUFNR, ICHARG, BWART, Direction, Remark, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_OrderDet_4
UNION ALL
SELECT Id, OrderNo, Seq, ExtNo, ExtSeq, StartDate, EndDate, ScheduleType, Item, ItemDesc, 
    RefItemCode, Uom, UC, Container, QualityType, ReqQty, OrderQty, ShipQty, RecQty, 
    RejQty, ScrapQty, PickQty, UnitQty, RecLotSize, LocFrom, LocFromNm, LocTo, LocToNm, 
    IsInspect, BillAddr, BillAddrDesc, PriceList, UnitPrice, IsProvEst, Tax, IsIncludeTax, Bom, 
    Routing, BillTerm, ReserveNo, ReserveLine, ZOPWZ, ZOPID, ZOPDS, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Currency, OrderType, ManufactureParty, OrderSubType, BaseUom, 
    PickStrategy, ExtraDmdSource, UCDesc, MinUC, ContainerDesc, IsScanHu, BinTo, 
    WMSSeq, IsChangeUC, AUFNR, ICHARG, BWART, Direction, Remark, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_OrderDet_5
UNION ALL
SELECT Id, OrderNo, Seq, ExtNo, ExtSeq, StartDate, EndDate, ScheduleType, Item, ItemDesc, 
    RefItemCode, Uom, UC, Container, QualityType, ReqQty, OrderQty, ShipQty, RecQty, 
    RejQty, ScrapQty, PickQty, UnitQty, RecLotSize, LocFrom, LocFromNm, LocTo, LocToNm, 
    IsInspect, BillAddr, BillAddrDesc, PriceList, UnitPrice, IsProvEst, Tax, IsIncludeTax, Bom, 
    Routing, BillTerm, ReserveNo, ReserveLine, ZOPWZ, ZOPID, ZOPDS, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Currency, OrderType, ManufactureParty, OrderSubType, BaseUom, 
    PickStrategy, ExtraDmdSource, UCDesc, MinUC, ContainerDesc, IsScanHu, BinTo, 
    WMSSeq, IsChangeUC, AUFNR, ICHARG, BWART, Direction, Remark, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_OrderDet_6
UNION ALL
SELECT Id, OrderNo, Seq, ExtNo, ExtSeq, StartDate, EndDate, ScheduleType, Item, ItemDesc, 
    RefItemCode, Uom, UC, Container, QualityType, ReqQty, OrderQty, ShipQty, RecQty, 
    RejQty, ScrapQty, PickQty, UnitQty, RecLotSize, LocFrom, LocFromNm, LocTo, LocToNm, 
    IsInspect, BillAddr, BillAddrDesc, PriceList, UnitPrice, IsProvEst, Tax, IsIncludeTax, Bom, 
    Routing, BillTerm, ReserveNo, ReserveLine, ZOPWZ, ZOPID, ZOPDS, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Currency, OrderType, ManufactureParty, OrderSubType, BaseUom, 
    PickStrategy, ExtraDmdSource, UCDesc, MinUC, ContainerDesc, IsScanHu, BinTo, 
    WMSSeq, IsChangeUC, AUFNR, ICHARG, BWART, Direction, Remark, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_OrderDet_7
UNION ALL
SELECT Id, OrderNo, Seq, ExtNo, ExtSeq, StartDate, EndDate, ScheduleType, Item, ItemDesc, 
    RefItemCode, Uom, UC, Container, QualityType, ReqQty, OrderQty, ShipQty, RecQty, 
    RejQty, ScrapQty, PickQty, UnitQty, RecLotSize, LocFrom, LocFromNm, LocTo, LocToNm, 
    IsInspect, BillAddr, BillAddrDesc, PriceList, UnitPrice, IsProvEst, Tax, IsIncludeTax, Bom, 
    Routing, BillTerm, ReserveNo, ReserveLine, ZOPWZ, ZOPID, ZOPDS, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Currency, OrderType, ManufactureParty, OrderSubType, BaseUom, 
    PickStrategy, ExtraDmdSource, UCDesc, MinUC, ContainerDesc, IsScanHu, BinTo, 
    WMSSeq, IsChangeUC, AUFNR, ICHARG, BWART, Direction, Remark, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_OrderDet_8
UNION ALL
SELECT Id, OrderNo, Seq, ExtNo, ExtSeq, StartDate, EndDate, ScheduleType, Item, ItemDesc, 
    RefItemCode, Uom, UC, Container, QualityType, ReqQty, OrderQty, ShipQty, RecQty, 
    RejQty, ScrapQty, PickQty, UnitQty, RecLotSize, LocFrom, LocFromNm, LocTo, LocToNm, 
    IsInspect, BillAddr, BillAddrDesc, PriceList, UnitPrice, IsProvEst, Tax, IsIncludeTax, Bom, 
    Routing, BillTerm, ReserveNo, ReserveLine, ZOPWZ, ZOPID, ZOPDS, CreateUser, 
    CreateUserNm, CreateDate, LastModifyUser, LastModifyUserNm, LastModifyDate, 
    Version, Currency, OrderType, ManufactureParty, OrderSubType, BaseUom, 
    PickStrategy, ExtraDmdSource, UCDesc, MinUC, ContainerDesc, IsScanHu, BinTo, 
    WMSSeq, IsChangeUC, AUFNR, ICHARG, BWART, Direction, Remark, PalletLotSize, PackageVolumn, PackageWeight
FROM dbo.ORD_OrderDet_0
GO
