using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using System.Linq;
using Castle.Services.Transaction;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class PartyMgrImpl : BaseMgr, IPartyMgr
    {

        public IGenericMgr genericMgr { get; set; }

        #region public methods

        public IList<Party> GetOrderFromParty(com.Sconit.CodeMaster.OrderType type)
        {
            return GetOrderParty(type, "From");
        }

        public IList<Party> GetOrderToParty(com.Sconit.CodeMaster.OrderType type)
        {
            return GetOrderParty(type, "To");
        }

        [Transaction(TransactionMode.Requires)]
        public void Create(Party party)
        {
            genericMgr.Create(party);

            #region 加权限
            Permission permission = new Permission();
            permission.Code = party.Code;
            permission.Description = party.CodeDescription;
            permission.PermissionCategory = party.GetType().Name;
            genericMgr.Create(permission);
            #endregion

            #region 加用户权限
            UserPermission up = new UserPermission();
            up.Permission = permission;
            up.User = SecurityContextHolder.Get();
            genericMgr.Create(up);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void AddPartyAddress(PartyAddress partyAddress)
        {
            if (partyAddress.IsPrimary)
            {
                this.genericMgr.Update("update from PartyAddress set IsPrimary = ? where Party = ?", new object[] { false, partyAddress.Party });
                this.genericMgr.FlushSession();
            }

            genericMgr.Create(partyAddress);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdatePartyAddress(PartyAddress partyAddress)
        {
            if (partyAddress.IsPrimary)
            {
                this.genericMgr.Update("update from PartyAddress set IsPrimary = ? where Party = ?", new object[] { false, partyAddress.Party });
                this.genericMgr.FlushSession();
            }

            genericMgr.Update(partyAddress);
        }
        #endregion

        #region private methods
        private IList<Party> GetOrderParty(com.Sconit.CodeMaster.OrderType type, string fromTo)
        {
            IList<Customer> customerList = genericMgr.FindAll<Customer>();
            IList<Supplier> supplierList = genericMgr.FindAll<Supplier>();
            IList<Region> regionList = genericMgr.FindAll<Region>();
            IList<Party> partyList = new List<Party>();

            if (fromTo == "From")
            {
                if (type == com.Sconit.CodeMaster.OrderType.Procurement)
                {
                    partyList.Union<Party>(supplierList);
                    partyList.Union<Party>(customerList);
                    partyList.Union<Party>(regionList);
                }
                else if (type == com.Sconit.CodeMaster.OrderType.ScheduleLine)
                {
                    partyList.Union<Party>(supplierList);
                    partyList.Union<Party>(regionList);
                }
                else if (type == com.Sconit.CodeMaster.OrderType.Distribution)
                {
                    partyList.Union<Party>(regionList);
                }
                else if (type == com.Sconit.CodeMaster.OrderType.Production)
                {
                    partyList.Union<Party>(regionList);
                }
            }
            else if (fromTo == "To")
            {
                if (type == com.Sconit.CodeMaster.OrderType.Procurement)
                {
                    partyList.Union<Party>(regionList);
                }
                else if (type == com.Sconit.CodeMaster.OrderType.ScheduleLine)
                {
                    partyList.Union<Party>(regionList);
                }
                else if (type == com.Sconit.CodeMaster.OrderType.Distribution)
                {
                    partyList.Union<Party>(customerList);
                }
                else if (type == com.Sconit.CodeMaster.OrderType.Production)
                {
                    partyList.Union<Party>(regionList);
                }
            }
            return partyList;
        }

        #endregion
    }
}