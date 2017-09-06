/****** Object:  StoredProcedure [dbo].[USP_Busi_GetOrderPlan4LeanEngine]    Script Date: 07/05/2012 14:55:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_GetOrderPlan4LeanEngine')
	DROP PROCEDURE USP_Busi_GetOrderPlan4LeanEngine
CREATE PROCEDURE [dbo].[USP_Busi_GetOrderPlan4LeanEngine]
AS
BEGIN
	
	------------------------------计算生产Bom消耗----------------------------------
	------------------------------计算Bom总用量----------------------------------
	 select
	  LocFrom, 
	  Item, 
	  BaseUom, 
	  ManufactureParty, 
	  StartTime, 
	  OrderNo, 
	  Type, 
	  Flow, 
	  OrderQty
	  into #TempBomPlan from
	  (select
	  case when isnull(bomdet.Location, '') != '' then bomdet.Location else (case when isnull(det.LocFrom, '') != '' then det.LocFrom else mstr.LocFrom end) end as LocFrom, 
	  bomDet.Item,
	  bomDet.BaseUom,
	  bomDet.ManufactureParty,
	  mstr.StartTime, 
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  sum(det.OrderQty * bomDet.BomUnitQty * bomDet.UnitQty) as OrderQty  --转为Bom基本单位的用量
	  from ORD_OrderBomDet as bomDet inner join
	  ORD_OrderDet_4 as det on bomDet.OrderDetId = det.Id inner join
	  ORD_OrderMstr_4 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  group by 
	  bomdet.Location, 
	  det.LocFrom, 
	  mstr.LocFrom, 
	  bomDet.Item,
	  bomDet.BaseUom,
	  bomDet.ManufactureParty,
	  mstr.StartTime, 
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow
	  union all
	  select
	  case when isnull(bomdet.Location, '') != '' then bomdet.Location else (case when isnull(det.LocFrom, '') != '' then det.LocFrom else mstr.LocFrom end) end as LocFrom, 
	  bomDet.Item,
	  bomDet.BaseUom,
	  bomDet.ManufactureParty,
	  mstr.StartTime, 
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  sum(det.OrderQty * bomDet.BomUnitQty * bomDet.UnitQty) as OrderQty  --转为Bom基本单位的用量
	  from ORD_OrderBomDet as bomDet inner join
	  ORD_OrderDet_5 as det on bomDet.OrderDetId = det.Id inner join
	  ORD_OrderMstr_5 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  group by 
	  bomdet.Location, 
	  det.LocFrom, 
	  mstr.LocFrom, 
	  bomDet.Item,
	  bomDet.BaseUom,
	  bomDet.ManufactureParty,
	  mstr.StartTime, 
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow) as BomPlan;
	  
	------------------------------根据订单Id、零件号、制造商、来源库位汇总Bom----------------------------------
	  select OrderNo, Item, ManufactureParty, LocFrom into #TempGroupedBomPlan from #TempBomPlan 
	  group by OrderNo, Item, ManufactureParty, LocFrom;
	  
	------------------------------计算Bom实际消耗----------------------------------
	------------------------------Bom回冲量----------------------------------
	  select OrderNo, Item, ManufactureParty, LocFrom, SUM(ShipQty) as ShipQty into #TempBomConsume
	  from (
	  select BFDet.OrderNo, BFDet.Item, BFDet.ManufactureParty, BFDet.LocFrom, sum(-(BFDet.BFQty + BFDet.BFRejQty + BFDet.BFScrapQty) * BFDet.UnitQty) as ShipQty
	  from ORD_OrderBackflushDet as BFDet inner join #TempGroupedBomPlan as GBP 
	  on BFDet.OrderNo = GBP.OrderNo and BFDet.Item = GBP.Item and BFDet.LocFrom = GBP.LocFrom
	  and ISNULL(BFDet.ManufactureParty, '') = ISNULL(GBP.ManufactureParty, '')
	  group by BFDet.OrderNo, BFDet.Item, BFDet.ManufactureParty, BFDet.LocFrom
	  union all
	------------------------------生产投料量（在制品）----------------------------------
	  select GBP.OrderNo, GBP.Item, GBP.ManufactureParty, GBP.LocFrom, sum(locDet.Qty - locDet.BFQty - locDet.VoidQty) as ShipQty
	  from PRD_ProdLineLocationDet as locDet left join INV_Hu as hu
	  on locDet.HuId = hu.HuId inner join #TempGroupedBomPlan as GBP 
	  on locDet.OrderNo = GBP.OrderNo and locDet.Item = GBP.Item
	  and locDet.LocFrom = GBP.LocFrom and ISNULL(hu.ManufactureParty, '') = ISNULL(GBP.ManufactureParty, '')
	  where locDet.IsClose = 0   --未关闭的投料
	  and locDet.OrderNo is not null  --投料至工单
	  group by GBP.OrderNo, GBP.Item, GBP.ManufactureParty, GBP.LocFrom
	  ) as BFBomQty group by OrderNo, Item, ManufactureParty, LocFrom;






	------------------------------获取订单----------------------------------
    select * into #tempPlan from(
	  ---------------------------采购-------------------------------------
	  select 
	  null as LocFrom, 
	  case when isnull(det.LocTo, '') != '' then det.LocTo else mstr.LocTo end as LocTo, 
	  det.Item,
	  det.Uom,
	  det.BaseUom,
	  det.UnitQty,
	  det.ManufactureParty,
	  mstr.StartTime, 
	  mstr.WindowTime, 
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  det.OrderQty,
	  det.ShipQty,
	  det.RecQty
	  from ORD_OrderDet_1 as det inner join
	  ORD_OrderMstr_1 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  union all
	  ---------------------------移库-------------------------------------
	  select 
	  case when isnull(det.LocFrom, '') != '' then det.LocFrom else mstr.LocFrom end as LocFrom, 
	  case when isnull(det.LocTo, '') != '' then det.LocTo else mstr.LocTo end as LocTo, 
	  det.Item,
	  det.Uom,
	  det.BaseUom,
	  det.UnitQty,
	  det.ManufactureParty,
	  mstr.StartTime, 
	  mstr.WindowTime,
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  det.OrderQty,
	  det.ShipQty,
	  det.RecQty
	  from ORD_OrderDet_2 as det inner join
	  ORD_OrderMstr_2 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  union all
	  ---------------------------销售-------------------------------------
	  select 
	  case when isnull(det.LocFrom, '') != '' then det.LocFrom else mstr.LocFrom end as LocFrom, 
	  null as LocTo, 
	  det.Item,
	  det.Uom,
	  det.BaseUom,
	  det.UnitQty,
	  det.ManufactureParty,
	  mstr.StartTime, 
	  mstr.WindowTime,
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  det.OrderQty,
	  det.ShipQty,
	  det.RecQty
	  from ORD_OrderDet_3 as det inner join
	  ORD_OrderMstr_3 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  union all
	  ---------------------------生产收货-------------------------------------
	  --在OrderDetail上生产只考虑收货，不考虑发料。发料在OrderBomDetail中考虑。
	  select 
	  null as LocFrom, 
	  case when isnull(det.LocTo, '') != '' then det.LocTo else mstr.LocTo end as LocTo, 
	  det.Item,
	  det.Uom,
	  det.BaseUom,
	  det.UnitQty,
	  det.ManufactureParty,
	  null as StartTime, 
	  mstr.WindowTime,
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  det.OrderQty,
	  0 as ShipQty,
	  det.RecQty
	  from ORD_OrderDet_4 as det inner join
	  ORD_OrderMstr_4 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  and det.OrderQty > det.RecQty --只考虑未完成收货的
	  union all
	  ---------------------------生产原材料消耗-------------------------------------
	  --select 
	  --case when isnull(bomdet.Location, '') != '' then bomdet.Location else (case when isnull(det.LocFrom, '') != '' then det.LocFrom else mstr.LocFrom end) end as LocFrom, 
	  --null as LocTo, 
	  --bomDet.Item,
	  --bomDet.Uom,
	  --bomDet.BaseUom,
	  --bomDet.UnitQty,
	  --bomDet.ManufactureParty,
	  --mstr.StartTime, 
	  --null as WindowTime, 
	  --mstr.OrderNo,
	  --mstr.Type,
	  --mstr.Flow,
	  --det.OrderQty * bomDet.BomUnitQty as OrderQty,                            --转为Bom单位的用量
	  --det.RecQty * bomDet.BomUnitQty as ShipQty,  --转为Bom单位的用量，生产单Bom考虑待发，待发 = 订单数 - 已发数
	  --0 as RecQty   
	  --from ORD_OrderBomDet as bomDet inner join
	  --ORD_OrderDet_4 as det on bomDet.OrderDetId = det.Id inner join
	  --ORD_OrderMstr_4 as mstr on det.OrderNo = mstr.OrderNo
	  --where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  --and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  --and det.OrderQty > det.RecQty --只考虑未完成收货的
	  --union all
	  ---------------------------委外加工-------------------------------------
	  --在OrderDetail上委外加工只考虑收货，不考虑发料。发料在OrderBomDetail中考虑。
	  select 
	  null as LocFrom, 
	  case when isnull(det.LocTo, '') != '' then det.LocTo else mstr.LocTo end as LocTo, 
	  det.Item,
	  det.Uom,
	  det.BaseUom,
	  det.UnitQty,
	  det.ManufactureParty,
	  null as StartTime, 
	  mstr.WindowTime,
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  det.OrderQty,
	  0 as ShipQty,
	  det.RecQty
	  from ORD_OrderDet_5 as det inner join
	  ORD_OrderMstr_5 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  and det.OrderQty > det.RecQty --只考虑未完成收货的
	  union all
	  ---------------------------委外加工原材料消耗-------------------------------------
	  --select 
	  --case when isnull(bomdet.Location, '') != '' then bomdet.Location else (case when isnull(det.LocFrom, '') != '' then det.LocFrom else mstr.LocFrom end) end as LocFrom, 
	  --null as LocTo, 
	  --bomDet.Item,
	  --bomDet.Uom,
	  --bomDet.BaseUom,
	  --bomDet.UnitQty,
	  --bomDet.ManufactureParty,
	  --mstr.StartTime, 
	  --null as WindowTime, 
	  --mstr.OrderNo,
	  --mstr.Type,
	  --mstr.Flow,
	  --det.OrderQty * bomDet.BomUnitQty as OrderQty,                            --转为Bom单位的用量
	  --det.RecQty * bomDet.BomUnitQty as ShipQty,  --转为Bom单位的用量，生产单Bom考虑待发，待发 = 订单数 - 已发数
	  --0 as RecQty   
	  --from ORD_OrderBomDet as bomDet inner join
	  --ORD_OrderDet_5 as det on bomDet.OrderDetId = det.Id inner join
	  --ORD_OrderMstr_5 as mstr on det.OrderNo = mstr.OrderNo
	  --where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  --and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  --and det.OrderQty > det.RecQty --只考虑未完成收货的
	  --union all
	  ---------------------------客供品-------------------------------------
	  select 
	  null as LocFrom, 
	  case when isnull(det.LocTo, '') != '' then det.LocTo else mstr.LocTo end as LocTo, 
	  det.Item,
	  det.Uom,
	  det.BaseUom,
	  det.UnitQty,
	  det.ManufactureParty,
	  mstr.StartTime, 
	  mstr.WindowTime,
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  det.OrderQty,
	  det.ShipQty,
	  det.RecQty
	  from ORD_OrderDet_6 as det inner join
	  ORD_OrderMstr_6 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  union all
	  ---------------------------委外加工发料——发料至委外库-------------------------------------
	  select 
	  case when isnull(det.LocFrom, '') != '' then det.LocFrom else mstr.LocFrom end as LocFrom, 
	  case when isnull(det.LocTo, '') != '' then det.LocTo else mstr.LocTo end as LocTo, 
	  det.Item,
	  det.Uom,
	  det.BaseUom,
	  det.UnitQty,
	  det.ManufactureParty,
	  mstr.StartTime, 
	  mstr.WindowTime,
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  det.OrderQty,
	  det.ShipQty,
	  det.RecQty
	  from ORD_OrderDet_7 as det inner join
	  ORD_OrderMstr_7 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  union all
	  ---------------------------计划协议-------------------------------------
	  select 
	  case when isnull(det.LocFrom, '') != '' then det.LocFrom else mstr.LocFrom end as LocFrom, 
	  case when isnull(det.LocTo, '') != '' then det.LocTo else mstr.LocTo end as LocTo, 
	  det.Item,
	  det.Uom,
	  det.BaseUom,
	  det.UnitQty,
	  det.ManufactureParty,
	  case when det.StartDate is not null then det.StartDate else mstr.StartTime end as StartTime, 
	  case when det.EndDate is not null then det.EndDate else mstr.WindowTime end as WindowTime, 
	  mstr.OrderNo,
	  mstr.Type,
	  mstr.Flow,
	  det.OrderQty,
	  det.ShipQty,
	  det.RecQty
	  from ORD_OrderDet_8 as det inner join
	  ORD_OrderMstr_8 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --只考虑普通订单的需求，不考虑退货单的
	  and mstr.Status in (1, 2) --只考虑状态为释放和进行中的
	  union all
	  ------------------------------获取在途----------------------------------
	  ---------------------------采购在途-------------------------------------
	  select 
	  null as LocFrom, 
	  det.LocTo, 
	  locDet.Item, 
	  det.BaseUom as Uom, 
	  det.BaseUom, 
	  1 as UnitQty, 
	  hu.ManufactureParty,
	  mstr.DepartTime as StartTime, 
	  mstr.ArriveTime as WindowTime, 
	  mstr.IpNo as OrderNo, 
	  mstr.OrderType as Type, 
	  null as Flow,
	  locDet.Qty as OrderQty, 
	  locDet.Qty as ShipQty, 
	  locDet.RecQty
	  from ORD_IpLocationDet_1 as locDet 
	  inner join ORD_IpDet_1 as det on locDet.IpDetId = det.Id
	  inner join ORD_IpMstr_1 as mstr on det.IpNo = mstr.IpNo
	  left join INV_Hu as hu on locDet.HuId = hu.HuId
	  where locDet.IsClose = 0 --and det.Type = 0 在途差异作为有效库存
	  union all 
	  ---------------------------移库在途-------------------------------------
	  select 
	  null as LocFrom, 
	  det.LocTo, 
	  locDet.Item, 
	  det.BaseUom as Uom, 
	  det.BaseUom, 
	  1 as UnitQty, 
	  hu.ManufactureParty,
	  mstr.DepartTime as StartTime, 
	  mstr.ArriveTime as WindowTime, 
	  mstr.IpNo as OrderNo, 
	  mstr.OrderType as Type, 
	  null as Flow,
	  locDet.Qty as OrderQty, 
	  locDet.Qty as ShipQty, 
	  locDet.RecQty
	  from ORD_IpLocationDet_2 as locDet 
	  inner join ORD_IpDet_2 as det on locDet.IpDetId = det.Id
	  inner join ORD_IpMstr_2 as mstr on det.IpNo = mstr.IpNo
	  left join INV_Hu as hu on locDet.HuId = hu.HuId
	  where locDet.IsClose = 0 --and det.Type = 0 在途差异作为有效库存
	  union all 
	  ---------------------------销售在途-------------------------------------
	  select 
	  null as LocFrom, 
	  det.LocTo, 
	  locDet.Item, 
	  det.BaseUom as Uom, 
	  det.BaseUom, 
	  1 as UnitQty, 
	  hu.ManufactureParty,
	  mstr.DepartTime as StartTime, 
	  mstr.ArriveTime as WindowTime, 
	  mstr.IpNo as OrderNo, 
	  mstr.OrderType as Type, 
	  null as Flow,
	  locDet.Qty as OrderQty, 
	  locDet.Qty as ShipQty, 
	  locDet.RecQty 
	  from ORD_IpLocationDet_3 as locDet 
	  inner join ORD_IpDet_3 as det on locDet.IpDetId = det.Id
	  inner join ORD_IpMstr_3 as mstr on det.IpNo = mstr.IpNo
	  left join INV_Hu as hu on locDet.HuId = hu.HuId
	  where locDet.IsClose = 0 --and det.Type = 0 在途差异作为有效库存
	  union all 
	  ---------------------------客供品在途-------------------------------------
	  select 
	  null as LocFrom, 
	  det.LocTo, 
	  locDet.Item, 
	  det.BaseUom as Uom, 
	  det.BaseUom, 
	  1 as UnitQty, 
	  hu.ManufactureParty,
	  mstr.DepartTime as StartTime, 
	  mstr.ArriveTime as WindowTime, 
	  mstr.IpNo as OrderNo, 
	  mstr.OrderType as Type, 
	  null as Flow,
	  locDet.Qty as OrderQty, 
	  locDet.Qty as ShipQty, 
	  locDet.RecQty
	  from ORD_IpLocationDet_6 as locDet 
	  inner join ORD_IpDet_6 as det on locDet.IpDetId = det.Id
	  inner join ORD_IpMstr_6 as mstr on det.IpNo = mstr.IpNo
	  left join INV_Hu as hu on locDet.HuId = hu.HuId
	  where locDet.IsClose = 0 --and det.Type = 0 在途差异作为有效库存
	  union all 
	  ---------------------------委外领料在途-------------------------------------
	  select 
	  null as LocFrom, 
	  det.LocTo, 
	  locDet.Item, 
	  det.BaseUom as Uom, 
	  det.BaseUom, 
	  1 as UnitQty, 
	  hu.ManufactureParty,
	  mstr.DepartTime as StartTime, 
	  mstr.ArriveTime as WindowTime, 
	  mstr.IpNo as OrderNo, 
	  mstr.OrderType as Type, 
	  null as Flow,
	  locDet.Qty as OrderQty, 
	  locDet.Qty as ShipQty, 
	  locDet.RecQty
	  from ORD_IpLocationDet_7 as locDet 
	  inner join ORD_IpDet_7 as det on locDet.IpDetId = det.Id
	  inner join ORD_IpMstr_7 as mstr on det.IpNo = mstr.IpNo
	  left join INV_Hu as hu on locDet.HuId = hu.HuId
	  where locDet.IsClose = 0 --and det.Type = 0 在途差异作为有效库存
	  union all 
	  ---------------------------计划协议在途-------------------------------------
	  select 
	  null as LocFrom, 
	  det.LocTo, 
	  locDet.Item, 
	  det.BaseUom as Uom, 
	  det.BaseUom, 
	  1 as UnitQty, 
	  hu.ManufactureParty,
	  mstr.DepartTime as StartTime, 
	  mstr.ArriveTime as WindowTime, 
	  mstr.IpNo as OrderNo, 
	  mstr.OrderType as Type, 
	  null as Flow,
	  locDet.Qty as OrderQty, 
	  locDet.Qty as ShipQty, 
	  locDet.RecQty
	  from ORD_IpLocationDet_8 as locDet 
	  inner join ORD_IpDet_8 as det on locDet.IpDetId = det.Id
	  inner join ORD_IpMstr_8 as mstr on det.IpNo = mstr.IpNo
	  left join INV_Hu as hu on locDet.HuId = hu.HuId
	  where locDet.IsClose = 0 --and det.Type = 0 在途差异作为有效库存
	  ---------------------------生产消耗（含生产和委外加工）-------------------------------------
	  union all
	  select bp.LocFrom,
	  null as LocTo,
	  bp.Item,
	  bp.BaseUom as Uom,
	  bp.BaseUom,
	  1 as UnitQty,
	  bp.ManufactureParty,
	  bp.StartTime,
	  null as WindowTime,
	  bp.OrderNo,
	  bp.Type,
	  bp.Flow,
	  bp.OrderQty,
	  isnull(bc.ShipQty, 0) as ShipQty,
	  0 as RecQty
	  from #TempBomPlan as bp left join #TempBomConsume as bc
	  on bp.OrderNo = bc.OrderNo and bp.Item = bc.Item and bp.LocFrom = bc.LocFrom
	  and ISNULL(bp.ManufactureParty, '') = ISNULL(bc.ManufactureParty, '')
	 ) as A;
	
	
	
	
	 
	 -------------------汇总待发需求-----------------------
	 select 
	 LocFrom as Loc, 
	 Item as ItemCode, 
	 BaseUom as Uom, 
	 ManufactureParty,
	 StartTime as ReqTime, 
	 OrderNo, 
	 Flow as FlowCode, 
	 'ISS' as IRType,
	 'Orders' as PlanType,
	 Case 
	  when Type = 1 then 'Procurement'   --采购
	  when Type = 2 then 'Transfer'      --移库
	  when Type = 3 then 'Distribution'  --销售
	  when Type = 4 then 'Production'    --生产
	  when Type = 5 then 'Production'    --委外加工
	  when Type = 6 then 'Procurement'   --客供品
	  when Type = 7 then 'Transfer'      --委外领料
	  when Type = 8 then 'Procurement'   --计划协议
	 end as FlowType,
	 OrderQty * UnitQty as OrderedQty,    --转为库存单位
	 ShipQty * UnitQty as FinishedQty     --转为库存单位
	 from #tempPlan 
	 where LocFrom is not null and OrderQty > ShipQty
	 union
	 -------------------汇总待收需求-----------------------
	 select 
	 LocTo as Loc, 
	 Item as ItemCode, 
	 BaseUom as Uom, 
	 ManufactureParty,
	 WindowTime as ReqTime, 
	 OrderNo, 
	 Flow as FlowCode, 
	 'RCT' as IRType,
	 'Orders' as PlanType,
	 Case 
	  when Type = 1 then 'Procurement'   --采购
	  when Type = 2 then 'Transfer'      --移库
	  when Type = 3 then 'Distribution'  --销售
	  when Type = 4 then 'Production'    --生产
	  when Type = 5 then 'Production'    --委外加工
	  when Type = 6 then 'Procurement'   --客供品
	  when Type = 7 then 'Transfer'      --委外领料
	  when Type = 8 then 'Procurement'   --计划协议
	 end as FlowType,
	 OrderQty * UnitQty as OrderedQty,    --转为库存单位
	 RecQty * UnitQty as FinishedQty      --转为库存单位
	 from #tempPlan 
	 where LocTo is not null and OrderQty > RecQty;
	 
	 drop table #tempPlan;
	 drop table #TempGroupedBomPlan;
	 drop table #TempBomConsume
	 drop table #TempBomPlan
END
GO

