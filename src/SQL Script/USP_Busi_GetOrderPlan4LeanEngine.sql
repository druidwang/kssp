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
	
	------------------------------��������Bom����----------------------------------
	------------------------------����Bom������----------------------------------
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
	  sum(det.OrderQty * bomDet.BomUnitQty * bomDet.UnitQty) as OrderQty  --תΪBom������λ������
	  from ORD_OrderBomDet as bomDet inner join
	  ORD_OrderDet_4 as det on bomDet.OrderDetId = det.Id inner join
	  ORD_OrderMstr_4 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
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
	  sum(det.OrderQty * bomDet.BomUnitQty * bomDet.UnitQty) as OrderQty  --תΪBom������λ������
	  from ORD_OrderBomDet as bomDet inner join
	  ORD_OrderDet_5 as det on bomDet.OrderDetId = det.Id inner join
	  ORD_OrderMstr_5 as mstr on det.OrderNo = mstr.OrderNo
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
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
	  
	------------------------------���ݶ���Id������š������̡���Դ��λ����Bom----------------------------------
	  select OrderNo, Item, ManufactureParty, LocFrom into #TempGroupedBomPlan from #TempBomPlan 
	  group by OrderNo, Item, ManufactureParty, LocFrom;
	  
	------------------------------����Bomʵ������----------------------------------
	------------------------------Bom�س���----------------------------------
	  select OrderNo, Item, ManufactureParty, LocFrom, SUM(ShipQty) as ShipQty into #TempBomConsume
	  from (
	  select BFDet.OrderNo, BFDet.Item, BFDet.ManufactureParty, BFDet.LocFrom, sum(-(BFDet.BFQty + BFDet.BFRejQty + BFDet.BFScrapQty) * BFDet.UnitQty) as ShipQty
	  from ORD_OrderBackflushDet as BFDet inner join #TempGroupedBomPlan as GBP 
	  on BFDet.OrderNo = GBP.OrderNo and BFDet.Item = GBP.Item and BFDet.LocFrom = GBP.LocFrom
	  and ISNULL(BFDet.ManufactureParty, '') = ISNULL(GBP.ManufactureParty, '')
	  group by BFDet.OrderNo, BFDet.Item, BFDet.ManufactureParty, BFDet.LocFrom
	  union all
	------------------------------����Ͷ����������Ʒ��----------------------------------
	  select GBP.OrderNo, GBP.Item, GBP.ManufactureParty, GBP.LocFrom, sum(locDet.Qty - locDet.BFQty - locDet.VoidQty) as ShipQty
	  from PRD_ProdLineLocationDet as locDet left join INV_Hu as hu
	  on locDet.HuId = hu.HuId inner join #TempGroupedBomPlan as GBP 
	  on locDet.OrderNo = GBP.OrderNo and locDet.Item = GBP.Item
	  and locDet.LocFrom = GBP.LocFrom and ISNULL(hu.ManufactureParty, '') = ISNULL(GBP.ManufactureParty, '')
	  where locDet.IsClose = 0   --δ�رյ�Ͷ��
	  and locDet.OrderNo is not null  --Ͷ��������
	  group by GBP.OrderNo, GBP.Item, GBP.ManufactureParty, GBP.LocFrom
	  ) as BFBomQty group by OrderNo, Item, ManufactureParty, LocFrom;






	------------------------------��ȡ����----------------------------------
    select * into #tempPlan from(
	  ---------------------------�ɹ�-------------------------------------
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
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  union all
	  ---------------------------�ƿ�-------------------------------------
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
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  union all
	  ---------------------------����-------------------------------------
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
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  union all
	  ---------------------------�����ջ�-------------------------------------
	  --��OrderDetail������ֻ�����ջ��������Ƿ��ϡ�������OrderBomDetail�п��ǡ�
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
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  and det.OrderQty > det.RecQty --ֻ����δ����ջ���
	  union all
	  ---------------------------����ԭ��������-------------------------------------
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
	  --det.OrderQty * bomDet.BomUnitQty as OrderQty,                            --תΪBom��λ������
	  --det.RecQty * bomDet.BomUnitQty as ShipQty,  --תΪBom��λ��������������Bom���Ǵ��������� = ������ - �ѷ���
	  --0 as RecQty   
	  --from ORD_OrderBomDet as bomDet inner join
	  --ORD_OrderDet_4 as det on bomDet.OrderDetId = det.Id inner join
	  --ORD_OrderMstr_4 as mstr on det.OrderNo = mstr.OrderNo
	  --where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  --and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  --and det.OrderQty > det.RecQty --ֻ����δ����ջ���
	  --union all
	  ---------------------------ί��ӹ�-------------------------------------
	  --��OrderDetail��ί��ӹ�ֻ�����ջ��������Ƿ��ϡ�������OrderBomDetail�п��ǡ�
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
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  and det.OrderQty > det.RecQty --ֻ����δ����ջ���
	  union all
	  ---------------------------ί��ӹ�ԭ��������-------------------------------------
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
	  --det.OrderQty * bomDet.BomUnitQty as OrderQty,                            --תΪBom��λ������
	  --det.RecQty * bomDet.BomUnitQty as ShipQty,  --תΪBom��λ��������������Bom���Ǵ��������� = ������ - �ѷ���
	  --0 as RecQty   
	  --from ORD_OrderBomDet as bomDet inner join
	  --ORD_OrderDet_5 as det on bomDet.OrderDetId = det.Id inner join
	  --ORD_OrderMstr_5 as mstr on det.OrderNo = mstr.OrderNo
	  --where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  --and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  --and det.OrderQty > det.RecQty --ֻ����δ����ջ���
	  --union all
	  ---------------------------�͹�Ʒ-------------------------------------
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
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  union all
	  ---------------------------ί��ӹ����ϡ���������ί���-------------------------------------
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
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  union all
	  ---------------------------�ƻ�Э��-------------------------------------
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
	  where mstr.SubType = 0  --ֻ������ͨ���������󣬲������˻�����
	  and mstr.Status in (1, 2) --ֻ����״̬Ϊ�ͷźͽ����е�
	  union all
	  ------------------------------��ȡ��;----------------------------------
	  ---------------------------�ɹ���;-------------------------------------
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
	  where locDet.IsClose = 0 --and det.Type = 0 ��;������Ϊ��Ч���
	  union all 
	  ---------------------------�ƿ���;-------------------------------------
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
	  where locDet.IsClose = 0 --and det.Type = 0 ��;������Ϊ��Ч���
	  union all 
	  ---------------------------������;-------------------------------------
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
	  where locDet.IsClose = 0 --and det.Type = 0 ��;������Ϊ��Ч���
	  union all 
	  ---------------------------�͹�Ʒ��;-------------------------------------
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
	  where locDet.IsClose = 0 --and det.Type = 0 ��;������Ϊ��Ч���
	  union all 
	  ---------------------------ί��������;-------------------------------------
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
	  where locDet.IsClose = 0 --and det.Type = 0 ��;������Ϊ��Ч���
	  union all 
	  ---------------------------�ƻ�Э����;-------------------------------------
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
	  where locDet.IsClose = 0 --and det.Type = 0 ��;������Ϊ��Ч���
	  ---------------------------�������ģ���������ί��ӹ���-------------------------------------
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
	
	
	
	
	 
	 -------------------���ܴ�������-----------------------
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
	  when Type = 1 then 'Procurement'   --�ɹ�
	  when Type = 2 then 'Transfer'      --�ƿ�
	  when Type = 3 then 'Distribution'  --����
	  when Type = 4 then 'Production'    --����
	  when Type = 5 then 'Production'    --ί��ӹ�
	  when Type = 6 then 'Procurement'   --�͹�Ʒ
	  when Type = 7 then 'Transfer'      --ί������
	  when Type = 8 then 'Procurement'   --�ƻ�Э��
	 end as FlowType,
	 OrderQty * UnitQty as OrderedQty,    --תΪ��浥λ
	 ShipQty * UnitQty as FinishedQty     --תΪ��浥λ
	 from #tempPlan 
	 where LocFrom is not null and OrderQty > ShipQty
	 union
	 -------------------���ܴ�������-----------------------
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
	  when Type = 1 then 'Procurement'   --�ɹ�
	  when Type = 2 then 'Transfer'      --�ƿ�
	  when Type = 3 then 'Distribution'  --����
	  when Type = 4 then 'Production'    --����
	  when Type = 5 then 'Production'    --ί��ӹ�
	  when Type = 6 then 'Procurement'   --�͹�Ʒ
	  when Type = 7 then 'Transfer'      --ί������
	  when Type = 8 then 'Procurement'   --�ƻ�Э��
	 end as FlowType,
	 OrderQty * UnitQty as OrderedQty,    --תΪ��浥λ
	 RecQty * UnitQty as FinishedQty      --תΪ��浥λ
	 from #tempPlan 
	 where LocTo is not null and OrderQty > RecQty;
	 
	 drop table #tempPlan;
	 drop table #TempGroupedBomPlan;
	 drop table #TempBomConsume
	 drop table #TempBomPlan
END
GO

