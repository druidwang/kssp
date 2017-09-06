using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Service.Impl
{
    public class NativeSqlStatement
    {
        //        public static string SELECT_CREATED_SEQUENCE_ORDER_STATEMENT = @"select ord.OrderNo, ord.Flow, ord.OrderType, ord.QualityType, ord.StartTime, ord.WindowTime,
        //                                                                        ord.PartyFrom, ord.PartyFromNm, ord.PartyTo, ord.PartyToNm, ord.ShipFrom, ord.ShipFromAddr, ord.ShipFromTel,
        //                                                                        ord.ShipFromCell, ord.ShipFromFax, ord.ShipFromContact, ord.ShipTo, ord.ShipToAddr, ord.ShipToTel, ord.ShipToCell,
        //                                                                        ord.ShipToFax, ord.ShipToContact, ord.LocFrom, ord.LocFromNm, ord.LocTo, ord.LocToNm, ord.Dock, 
        //                                                                        ord.IsAutoReceive, ord.IsPrintAsn, ord.IsPrintRec, ord.IsCheckPartyFromAuth, ord.IsCheckPartyToAuth,
        //                                                                        ord.AsnTemplate, ord.RecTemplate, ord.Seq, ord.TraceCode, 
        //                                                                        ord.OrderDetId, ord.OrderDetSeq, ord.Item, ord.ItemDesc, ord.RefItemCode,
        //                                                                        ord.Uom, ord.UnitQty, ord.BaseUom, ord.UC, ord.ManufactureParty,
        //                                                                        ord.Container, ord.ContainerDesc, ord.OrderQty, ord.RecQty, isnull(Seq.Qty, 0) SeqQty
        //                                                                        from 
        //                                                                        ( 
        //                                                                        --采购排序单
        //                                                                        select mstr.OrderNo, mstr.Flow, mstr.[Type] as OrderType, mstr.QualityType, mstr.StartTime, mstr.WindowTime,
        //                                                                        mstr.PartyFrom, mstr.PartyFromNm, mstr.PartyTo, mstr.PartyToNm, mstr.ShipFrom, mstr.ShipFromAddr, mstr.ShipFromTel,
        //                                                                        mstr.ShipFromCell, mstr.ShipFromFax, mstr.ShipFromContact, mstr.ShipTo, mstr.ShipToAddr, mstr.ShipToTel, mstr.ShipToCell,
        //                                                                        mstr.ShipToFax, mstr.ShipToContact, mstr.LocFrom, mstr.LocFromNm, mstr.LocTo, mstr.LocToNm, mstr.Dock, 
        //                                                                        mstr.IsAutoReceive, mstr.IsPrintAsn, mstr.IsPrintRec, mstr.IsCheckPartyFromAuth, mstr.IsCheckPartyToAuth,
        //                                                                        mstr.AsnTemplate, mstr.RecTemplate, mstr.Seq, mstr.TraceCode, 
        //                                                                        det.Id as OrderDetId, det.Seq as OrderDetSeq, det.Item, det.ItemDesc, det.RefItemCode,
        //                                                                        det.Uom, det.UnitQty, det.BaseUom, det.UC, det.ManufactureParty,
        //                                                                        det.Container, det.ContainerDesc, det.OrderQty, det.RecQty
        //                                                                        from ord_orderdet_1 as det
        //                                                                        inner join ORD_OrderMstr_1 as mstr on det.OrderNo = mstr.OrderNo
        //                                                                        inner join 
        //                                                                        (select Flow, Min(Seq) as Seq from ORD_OrderMstr_1
        //                                                                            where OrderStrategy = 4 and Status in (1, 2) and IsPause = 0
        //                                                                            --可以获取所有已经关闭（已配送）的排序单
        //        	                                                                group by Flow) as minSeq on mstr.Flow = minSeq.Flow and mstr.Seq >= minSeq.Seq
        //                                                                        union all
        //                                                                        --移库排序单
        //                                                                        select mstr.OrderNo, mstr.Flow, mstr.[Type] as OrderType, mstr.QualityType, mstr.StartTime, mstr.WindowTime,
        //                                                                        mstr.PartyFrom, mstr.PartyFromNm, mstr.PartyTo, mstr.PartyToNm, mstr.ShipFrom, mstr.ShipFromAddr, mstr.ShipFromTel,
        //                                                                        mstr.ShipFromCell, mstr.ShipFromFax, mstr.ShipFromContact, mstr.ShipTo, mstr.ShipToAddr, mstr.ShipToTel, mstr.ShipToCell,
        //                                                                        mstr.ShipToFax, mstr.ShipToContact, mstr.LocFrom, mstr.LocFromNm, mstr.LocTo, mstr.LocToNm, mstr.Dock, 
        //                                                                        mstr.IsAutoReceive, mstr.IsPrintAsn, mstr.IsPrintRec, mstr.IsCheckPartyFromAuth, mstr.IsCheckPartyToAuth,
        //                                                                        mstr.AsnTemplate, mstr.RecTemplate, mstr.Seq, mstr.TraceCode, 
        //                                                                        det.Id as OrderDetId, det.Seq as OrderDetSeq, det.Item, det.ItemDesc, det.RefItemCode,
        //                                                                        det.Uom, det.UnitQty, det.BaseUom, det.UC, det.ManufactureParty,
        //                                                                        det.Container, det.ContainerDesc, det.OrderQty, det.RecQty 
        //                                                                        from ord_orderdet_2 as det
        //                                                                        inner join ORD_OrderMstr_2 as mstr on det.OrderNo = mstr.OrderNo
        //                                                                        inner join 
        //                                                                        (select Flow, Min(Seq) as Seq from ORD_OrderMstr_2
        //                                                                            where OrderStrategy = 4 and Status in (1, 2) and IsPause = 0
        //                                                                            --可以获取所有已经关闭（已配送）的排序单
        //        	                                                                group by Flow) as minSeq on mstr.Flow = minSeq.Flow and mstr.Seq >= minSeq.Seq
        //                                                                        union all
        //                                                                        --客供品排序单
        //                                                                        select mstr.OrderNo, mstr.Flow, mstr.[Type] as OrderType, mstr.QualityType, mstr.StartTime, mstr.WindowTime,
        //                                                                        mstr.PartyFrom, mstr.PartyFromNm, mstr.PartyTo, mstr.PartyToNm, mstr.ShipFrom, mstr.ShipFromAddr, mstr.ShipFromTel,
        //                                                                        mstr.ShipFromCell, mstr.ShipFromFax, mstr.ShipFromContact, mstr.ShipTo, mstr.ShipToAddr, mstr.ShipToTel, mstr.ShipToCell,
        //                                                                        mstr.ShipToFax, mstr.ShipToContact, mstr.LocFrom, mstr.LocFromNm, mstr.LocTo, mstr.LocToNm, mstr.Dock, 
        //                                                                        mstr.IsAutoReceive, mstr.IsPrintAsn, mstr.IsPrintRec, mstr.IsCheckPartyFromAuth, mstr.IsCheckPartyToAuth,
        //                                                                        mstr.AsnTemplate, mstr.RecTemplate, mstr.Seq, mstr.TraceCode, 
        //                                                                        det.Id as OrderDetId, det.Seq as OrderDetSeq, det.Item, det.ItemDesc, det.RefItemCode,
        //                                                                        det.Uom, det.UnitQty, det.BaseUom, det.UC, det.ManufactureParty,
        //                                                                        det.Container, det.ContainerDesc, det.OrderQty, det.RecQty 
        //                                                                        from ord_orderdet_6 as det
        //                                                                        inner join ORD_OrderMstr_6 as mstr on det.OrderNo = mstr.OrderNo
        //                                                                        inner join 
        //                                                                        (select Flow, Min(Seq) as Seq from ORD_OrderMstr_6
        //                                                                            where OrderStrategy = 4 and Status in (1, 2) and IsPause = 0
        //                                                                            --可以获取所有已经关闭（已配送）的排序单
        //        	                                                                group by Flow) as minSeq on mstr.Flow = minSeq.Flow and mstr.Seq >= minSeq.Seq
        //                                                                        ) as ord
        //                                                                        left join (
        //                                                                        --已经生成的排序装箱单
        //                                                                        select OrderDetId, count(1) as Qty 
        //                                                                        from ORD_SeqDet where IsClose = 0
        //                                                                        group by OrderDetId) as Seq on ord.OrderDetId = Seq.OrderDetId";
        //    @"select mstr.* from View_OrderMstr as mstr inner join (
        //	                                --获取排序路线上未配送排序件的最小序号，未创建排序装箱单的排序单状态为Submit或Release(如果排序件出现1件以上，并且部分创建了排序装箱单)
        //	                                select Flow, Min(Seq) as Seq from View_OrderMstr
        //                                    where OrderStrategy = ? and Status in (?, ?) and IsPause = ? and IsPLPause = ?
        //                                    --可以获取所有已经关闭（已配送）的排序单
        //	                                group by Flow) as minSeq on mstr.Flow = minSeq.Flow and mstr.Seq >= minSeq.Seq";

        //KIT都是移库类型的，可以锁定分表2
        public static string SELECT_KIT_RECEIPT_STATEMENT = @"select locDet.* from ORD_RecLocationDet_2 as locDet 
                                                                        inner join ORD_OrderDet_2 as ordDet on locDet.OrderDetId = ordDet.Id
                                                                        where ordDet.OrderNo in (?";


        public static string SELECT_NOT_BACKFLUSH_ORDER_BOM_DETAIL_STATEMENT = @"select bom.* from Ord_OrderOp as op
                                                                                inner join ORD_OrderBomDet as bom on op.OrderNo = bom.OrderNo and op.Op = bom.Op
                                                                                where op.OrderNo = ? and op.IsBackflush = ? and bom.FeedMethod = ? and bom.IsAutoFeed = ?";

        public static string SELECT_ORDER_BOM_DET_BY_ORDER_DET_ID = "select * from ORD_OrderBomDet_4 where OrderDetId = ?";

    }
}
