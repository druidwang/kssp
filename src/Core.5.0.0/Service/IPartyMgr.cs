using System.Collections.Generic;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.MD;

//TODO: Add other using statements here.

namespace com.Sconit.Service
{
    public interface IPartyMgr 
    {
        #region Customized Methods

        IList<Party> GetOrderFromParty(com.Sconit.CodeMaster.OrderType type);

        IList<Party> GetOrderToParty(com.Sconit.CodeMaster.OrderType type);

        void Create(Party party);

        void AddPartyAddress(PartyAddress partyAddress);

        void UpdatePartyAddress(PartyAddress partyAddress);

        #endregion Customized Methods
    }
}
