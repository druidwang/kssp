/****** Object:  StoredProcedure [dbo].[USP_Busi_GetWaitSeqDet]    Script Date: 07/28/2012 15:18:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM SYS.objects WHERE type='P' AND name='USP_Busi_GetWaitSeqDet') DROP PROCEDURE USP_Busi_GetWaitSeqDet
go
	
Create PROCEDURE [dbo].[USP_Busi_GetWaitSeqDet]
AS
BEGIN
	--已经生成的排序装箱单
	select OrderDetId, count(1) as Qty into #tempSeqOrder
	from ORD_SeqDet where IsClose = 0
	group by OrderDetId

	select ord.OrderNo, ord.Flow, ord.OrderType, ord.QualityType, ord.StartTime, ord.WindowTime,
		ord.PartyFrom, ord.PartyFromNm, ord.PartyTo, ord.PartyToNm, ord.ShipFrom, ord.ShipFromAddr, ord.ShipFromTel,
		ord.ShipFromCell, ord.ShipFromFax, ord.ShipFromContact, ord.ShipTo, ord.ShipToAddr, ord.ShipToTel, ord.ShipToCell,
		ord.ShipToFax, ord.ShipToContact, ord.LocFrom, ord.LocFromNm, ord.LocTo, ord.LocToNm, ord.Dock, 
		ord.IsAutoReceive, ord.IsPrintAsn, ord.IsPrintRec, ord.IsCheckPartyFromAuth, ord.IsCheckPartyToAuth,
		ord.AsnTemplate, ord.RecTemplate, ord.Seq, ord.TraceCode, 
		ord.OrderDetId, ord.OrderDetSeq, ord.Item, ord.ItemDesc, ord.RefItemCode,
		ord.Uom, ord.UnitQty, ord.BaseUom, ord.UC, ord.ManufactureParty,
		ord.Container, ord.ContainerDesc, ord.OrderQty, ord.ShipQty, isnull(Seq.Qty, 0) SeqQty
	from 
	( 
		--采购排序单
		select mstr.OrderNo, mstr.Flow, mstr.[Type] as OrderType, mstr.QualityType, mstr.StartTime, mstr.WindowTime,
		mstr.PartyFrom, mstr.PartyFromNm, mstr.PartyTo, mstr.PartyToNm, mstr.ShipFrom, mstr.ShipFromAddr, mstr.ShipFromTel,
		mstr.ShipFromCell, mstr.ShipFromFax, mstr.ShipFromContact, mstr.ShipTo, mstr.ShipToAddr, mstr.ShipToTel, mstr.ShipToCell,
		mstr.ShipToFax, mstr.ShipToContact, mstr.LocFrom, mstr.LocFromNm, mstr.LocTo, mstr.LocToNm, mstr.Dock, 
		mstr.IsAutoReceive, mstr.IsPrintAsn, mstr.IsPrintRec, mstr.IsCheckPartyFromAuth, mstr.IsCheckPartyToAuth,
		mstr.AsnTemplate, mstr.RecTemplate, mstr.Seq, mstr.TraceCode, 
		det.Id as OrderDetId, det.Seq as OrderDetSeq, det.Item, det.ItemDesc, det.RefItemCode,
		det.Uom, det.UnitQty, det.BaseUom, det.UC, det.ManufactureParty,
		det.Container, det.ContainerDesc, det.OrderQty, det.ShipQty
		from ord_orderdet_1 as det
		inner join ORD_OrderMstr_1 as mstr on det.OrderNo = mstr.OrderNo
		inner join 
		(
			select Flow, Min(mstr.Seq) as Seq from ord_orderdet_1 as det
			inner join ORD_OrderMstr_1 as mstr on det.OrderNo = mstr.OrderNo
			left join #tempSeqOrder as Seq on det.Id = Seq.OrderDetId
			where OrderStrategy = 4 and Status in (1, 2) and IsPause = 0 and det.OrderQty > (det.ShipQty + ISNULL(Seq.Qty, 0))
			--可以获取所有已经关闭（已配送）的排序单
			group by Flow
		) as minSeq on mstr.Flow = minSeq.Flow and mstr.Seq >= minSeq.Seq
		union all
		--移库排序单
		select mstr.OrderNo, mstr.Flow, mstr.[Type] as OrderType, mstr.QualityType, mstr.StartTime, mstr.WindowTime,
		mstr.PartyFrom, mstr.PartyFromNm, mstr.PartyTo, mstr.PartyToNm, mstr.ShipFrom, mstr.ShipFromAddr, mstr.ShipFromTel,
		mstr.ShipFromCell, mstr.ShipFromFax, mstr.ShipFromContact, mstr.ShipTo, mstr.ShipToAddr, mstr.ShipToTel, mstr.ShipToCell,
		mstr.ShipToFax, mstr.ShipToContact, mstr.LocFrom, mstr.LocFromNm, mstr.LocTo, mstr.LocToNm, mstr.Dock, 
		mstr.IsAutoReceive, mstr.IsPrintAsn, mstr.IsPrintRec, mstr.IsCheckPartyFromAuth, mstr.IsCheckPartyToAuth,
		mstr.AsnTemplate, mstr.RecTemplate, mstr.Seq, mstr.TraceCode, 
		det.Id as OrderDetId, det.Seq as OrderDetSeq, det.Item, det.ItemDesc, det.RefItemCode,
		det.Uom, det.UnitQty, det.BaseUom, det.UC, det.ManufactureParty,
		det.Container, det.ContainerDesc, det.OrderQty, det.ShipQty 
		from ord_orderdet_2 as det
		inner join ORD_OrderMstr_2 as mstr on det.OrderNo = mstr.OrderNo
		inner join 
		(
			select Flow, Min(mstr.Seq) as Seq from ord_orderdet_2 as det
			inner join ORD_OrderMstr_2 as mstr on det.OrderNo = mstr.OrderNo
			left join #tempSeqOrder as Seq on det.Id = Seq.OrderDetId
			where OrderStrategy = 4 and Status in (1, 2) and IsPause = 0 and det.OrderQty > (det.ShipQty + ISNULL(Seq.Qty, 0))
			--可以获取所有已经关闭（已配送）的排序单
			group by Flow
		) as minSeq on mstr.Flow = minSeq.Flow and mstr.Seq >= minSeq.Seq
		union all
		--客供品排序单
		select mstr.OrderNo, mstr.Flow, mstr.[Type] as OrderType, mstr.QualityType, mstr.StartTime, mstr.WindowTime,
		mstr.PartyFrom, mstr.PartyFromNm, mstr.PartyTo, mstr.PartyToNm, mstr.ShipFrom, mstr.ShipFromAddr, mstr.ShipFromTel,
		mstr.ShipFromCell, mstr.ShipFromFax, mstr.ShipFromContact, mstr.ShipTo, mstr.ShipToAddr, mstr.ShipToTel, mstr.ShipToCell,
		mstr.ShipToFax, mstr.ShipToContact, mstr.LocFrom, mstr.LocFromNm, mstr.LocTo, mstr.LocToNm, mstr.Dock, 
		mstr.IsAutoReceive, mstr.IsPrintAsn, mstr.IsPrintRec, mstr.IsCheckPartyFromAuth, mstr.IsCheckPartyToAuth,
		mstr.AsnTemplate, mstr.RecTemplate, mstr.Seq, mstr.TraceCode, 
		det.Id as OrderDetId, det.Seq as OrderDetSeq, det.Item, det.ItemDesc, det.RefItemCode,
		det.Uom, det.UnitQty, det.BaseUom, det.UC, det.ManufactureParty,
		det.Container, det.ContainerDesc, det.OrderQty, det.ShipQty 
		from ord_orderdet_6 as det
		inner join ORD_OrderMstr_6 as mstr on det.OrderNo = mstr.OrderNo
		inner join 
		(
			select Flow, Min(mstr.Seq) as Seq from ord_orderdet_6 as det
			inner join ORD_OrderMstr_6 as mstr on det.OrderNo = mstr.OrderNo
			left join #tempSeqOrder as Seq on det.Id = Seq.OrderDetId
			where OrderStrategy = 4 and Status in (1, 2) and IsPause = 0 and det.OrderQty > (det.ShipQty + ISNULL(Seq.Qty, 0))
			--可以获取所有已经关闭（已配送）的排序单
			group by Flow
		) as minSeq on mstr.Flow = minSeq.Flow and mstr.Seq >= minSeq.Seq
	) as ord
	left join #tempSeqOrder as Seq on ord.OrderDetId = Seq.OrderDetId;
	
	drop table #tempSeqOrder;
END

GO


