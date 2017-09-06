using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.Exception;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class KanBanCardMgrImpl : BaseMgr, IKanBanCardMgr
    {
        public INumberControlMgr numberControlMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public void CreateKanBanCard(KanBanCard kanBanCard, int? qty)
        {
            IList<KanBanCard> kanBanCardList = genericMgr.FindAll<KanBanCard>("from KanBanCard as k where k.LocationTo = ?", kanBanCard.LocationTo);

            int kbCount = kanBanCardList == null || kanBanCardList.Count == 0 ? 1 : kanBanCardList.Count + 1;

            kanBanCard.Code = "KB" + kanBanCard.LocationTo + kbCount.ToString().PadLeft(3, '0');

            //kanBanCard.Code = numberControlMgr.GetKanBanCardNo();
            int count = qty == null || qty == 0 ? 1 : qty.Value;
            kanBanCard.Qty = count;
            genericMgr.Create(kanBanCard);

            for (int i = 0; i < count; i++)
            {
                KanBanCardInfo kanBanCardInfo = new KanBanCardInfo();
                kanBanCardInfo.KBICode = kanBanCard.Code;
                kanBanCardInfo.CardNo = kanBanCard.Code + i.ToString().PadLeft(4, '0');
                kanBanCardInfo.Sequence = i + 1;
                genericMgr.Create(kanBanCardInfo);
            }

        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateKanBanCard(KanBanCard kanBanCard, int? qty)
        {
           

            IList<KanBanCardInfo> kanBanCardInfoList = genericMgr.FindAll<KanBanCardInfo>("from KanBanCardInfo as k where k.KBICode = ?", kanBanCard.Code);
            int count = kanBanCardInfoList.Count();
            count = qty == null || qty == 0 ? kanBanCardInfoList.Count() + 1 : kanBanCardInfoList.Count() + qty.Value;
            for (int i = kanBanCardInfoList.Count(); i < count; i++)
            {
                KanBanCardInfo kanBanCardInfo = new KanBanCardInfo();
                kanBanCardInfo.KBICode = kanBanCard.Code;
                kanBanCardInfo.CardNo = kanBanCard.Code + i.ToString().PadLeft(4, '0');
                kanBanCardInfo.Sequence = i + 1;
                genericMgr.Create(kanBanCardInfo);
            }
            kanBanCard.Qty = count;
            genericMgr.Update(kanBanCard);
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteKanBanCard(KanBanCard kanBanCard, int? qty)
        {

            IList<KanBanCardInfo> kanBanCardInfoList = genericMgr.FindAll<KanBanCardInfo>("from KanBanCardInfo as k where k.KBICode = ?", kanBanCard.Code);
            int count = qty == null || qty == 0 ? 1 : qty.Value;
            if (count > kanBanCardInfoList.Count())
            {
                throw new BusinessException("现有看板卡数{0}少于要删除数量{1}", kanBanCardInfoList.Count().ToString(), count.ToString());
            }
            var q = kanBanCardInfoList.OrderByDescending(k => k.Sequence).Take(count);
            genericMgr.Delete<KanBanCardInfo>(q.ToList());
            kanBanCard.Qty = kanBanCardInfoList.Count() - count;
            if (kanBanCard.Qty > 0)
            {
                genericMgr.Update(kanBanCard);
            }
            else
            {
                genericMgr.Delete(kanBanCardInfoList);
            }
        }

    }
}
