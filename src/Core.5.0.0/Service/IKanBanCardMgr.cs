using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.INV;

namespace com.Sconit.Service
{
    public interface IKanBanCardMgr
    {
        void CreateKanBanCard(KanBanCard kanBanCard, int? qty);

        void UpdateKanBanCard(KanBanCard kanBanCard, int? qty);

        void DeleteKanBanCard(KanBanCard kanBanCard, int? qty);

    }
}
