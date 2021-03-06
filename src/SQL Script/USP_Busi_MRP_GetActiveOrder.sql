USE [Sconit]
GO
/****** Object:  StoredProcedure [dbo].[USP_Busi_MRP_GetActiveOrder]    Script Date: 12/08/2014 15:10:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_Busi_MRP_GetActiveOrder]
(
	@SnapTime datetime
)
AS
BEGIN

select 
row_number() over(order by getdate()) Id,
det.OrderNo,
det.Id as OrderDetId,
mstr.Flow,
mstr.Type as OrderType,
mstr.LocFrom  as LocationFrom, 
mstr.PartyFrom as PartyFrom,
mstr.LocTo  as LocationTo, 
mstr.PartyTo as PartyTo,
det.Item,
mstr.StartTime, 
mstr.WindowTime,
det.OrderQty * det.UnitQty as OrderQty,
det.ShipQty * det.UnitQty as ShippedQty,
det.RecQty * det.UnitQty as ReceivedQty,
@SnapTime as SnapTime,
mstr.IsIndepentDemand as IsIndepentDemand,
mstr.ResourceGroup as ResourceGroup,
mstr.Shift as Shift,
det.Bom as Bom
from VIEW_OrderDet as det inner join
VIEW_OrderMstr as mstr on det.OrderNo = mstr.OrderNo
where --mstr.IsIndepentDemand = 1 and--只考虑普通订单的需求，不考虑退货单的
mstr.Status in (0, 1, 2)

END