namespace com.Sconit.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using com.Sconit.Entity.Exception;
    using System.IO;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Entity;
    using com.Sconit.CodeMaster;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity.SCM;
    using com.Sconit.Entity.BIL;

    public static class SecurityHelper
    {
        public static void AddBillPermissionStatement(ref string whereStatement, string partyAlias, string partyName, BillType billType)
        {
            //su特殊处理，不用考虑权限
            User user = SecurityContextHolder.Get();
            if (whereStatement == string.Empty)
            {
                whereStatement += " where (";
            }
            else
            {
                whereStatement += " and (";
            }

            int permissionCategoryType = (int)PermissionCategoryType.Supplier;
            if (billType == BillType.Distribution)
            {
                permissionCategoryType = (int)PermissionCategoryType.Customer;
            }
            whereStatement += "  exists (select 1 from UserPermissionView as up where up.UserId ="
                 + user.Id + " and up.PermissionCategoryType = " + permissionCategoryType
                 + " and up.PermissionCode = " + partyAlias + "." + partyName + ")) ";
        }

        //参数OrderType指菜单类别：供货、发货、生产
        //参数orderType指订单类型：采购/生产/移库/销售
        public static void AddPartyFromAndPartyToPermissionStatement(
            ref string whereStatement, string orderTypeTableAlias, string orderTypeFieldName, string partyFromTableAlias,
            string partyFromFieldName, string partyToTableAlias, string partyToFieldName, OrderType orderType,
            bool isSupplier)
        {
            User user = SecurityContextHolder.Get();

            #region 移库 Hql
            string transferHql = " (" +
                        orderTypeTableAlias + "." + orderTypeFieldName + " in ("
                        + (int)OrderType.SubContractTransfer + ","
                        + (int)OrderType.Transfer + ") "

                        + "and ((" + partyToTableAlias + ".IsCheckPartyFromAuthority = 0 and " + partyToTableAlias + ".IsCheckPartyToAuthority = 0 "
                        + "and ( exists (select 1 from UserPermissionView as p where p.UserId ="
                        + user.Id + " and  p.PermissionCategoryType  = "
                        + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyFromTableAlias + "." + partyFromFieldName + ")"
                        + " or exists (select 1 from UserPermissionView as p where p.UserId ="
                        + user.Id + " and  p.PermissionCategoryType  = "
                        + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyToTableAlias + "." + partyToFieldName + ")))"

                        + " or (" + partyToTableAlias + ".IsCheckPartyFromAuthority = 0 and " + partyToTableAlias + ".IsCheckPartyToAuthority = 1 "
                            + " and (exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id
                        + " and  p.PermissionCategoryType =" + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyToTableAlias + "." + partyToFieldName + ")))"

                        + " or (" + partyToTableAlias + ".IsCheckPartyFromAuthority = 1 and " + partyToTableAlias + ".IsCheckPartyToAuthority = 0 "
                            + " and (exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id
                        + " and  p.PermissionCategoryType =" + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyFromTableAlias + "." + partyFromFieldName + ")))"

                        + " or (" + partyToTableAlias + ".IsCheckPartyFromAuthority = 1 and " + partyToTableAlias + ".IsCheckPartyToAuthority = 1 "
                        + "and ( exists (select 1 from UserPermissionView as p where p.UserId ="
                        + user.Id + "and  p.PermissionCategoryType  = "
                        + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyFromTableAlias + "." + partyFromFieldName + "))"
                        + " and exists (select 1 from UserPermissionView as p where p.UserId ="
                        + user.Id + " and  p.PermissionCategoryType  = "
                        + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyToTableAlias + "." + partyToFieldName + "))))";
            #endregion

            if (user.Code.Trim().ToLower() != "su")
            {
                if (whereStatement == string.Empty)
                {
                    whereStatement += " where (";
                }
                else
                {
                    whereStatement += " and (";
                }

                if (orderType == OrderType.Procurement)
                {
                    if (isSupplier)
                    {
                        whereStatement += orderTypeTableAlias + "." + orderTypeFieldName + " in ("
                            + (int)OrderType.CustomerGoods + ","
                            + (int)OrderType.Procurement + ","
                            + (int)OrderType.SubContract + ","
                            + (int)OrderType.ScheduleLine + ")"
                            + " and  exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id
                            + " and  p.PermissionCategoryType in (" + (int)PermissionCategoryType.Supplier
                            + "," + (int)PermissionCategoryType.Customer
                            + ") and p.PermissionCode = " + partyFromTableAlias + "." + partyFromFieldName + ")";
                    }
                    else
                    {
                        whereStatement += " (" + orderTypeTableAlias + "." + orderTypeFieldName + " in ("
                            + (int)OrderType.CustomerGoods + ","
                            + (int)OrderType.Procurement + ","
                            + (int)OrderType.SubContract + ","
                            + (int)OrderType.ScheduleLine
                            + ") and ( exists (select 1 from UserPermissionView as p where p.UserId ="
                            + user.Id + " and  p.PermissionCategoryType in ("
                            + (int)PermissionCategoryType.Supplier
                            + "," + (int)PermissionCategoryType.Region
                            + "," + (int)PermissionCategoryType.Customer
                            + ") and p.PermissionCode = " + partyFromTableAlias + "." + partyFromFieldName + "))"
                            + " and  ((" + partyToTableAlias + ".IsCheckPartyToAuthority = 0) or ( " + partyToTableAlias
                            + ".IsCheckPartyToAuthority = 1 and exists (select 1 from UserPermissionView as p where p.UserId ="
                            + user.Id + "and p.PermissionCategoryType =" + (int)PermissionCategoryType.Region
                            + " and p.PermissionCode = " + partyToTableAlias + "." + partyToFieldName + "))))"
                            + " or " + transferHql;
                    }
                }
                else if (orderType == OrderType.Distribution || orderType == OrderType.CustomerGoods)
                {
                    whereStatement += " ((" + orderTypeTableAlias + "." + orderTypeFieldName
                        + " = " + (int)OrderType.Distribution
                        + " and ( exists (select 1 from UserPermissionView as p where p.UserId ="
                        + user.Id + " and  p.PermissionCategoryType in ("
                        + (int)PermissionCategoryType.Region + ","
                        + (int)PermissionCategoryType.Customer + ") and p.PermissionCode =  "
                        + partyToTableAlias + "." + partyToFieldName

                        + "))) and  ((" + partyToTableAlias + ".IsCheckPartyFromAuthority = 0) or (" + partyToTableAlias
                        + ".IsCheckPartyFromAuthority = 1 and exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id
                        + " and  p.PermissionCategoryType =" + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyFromTableAlias + "." + partyFromFieldName + "))))"

                        + " or " + transferHql;
                }
                else if (orderType == OrderType.Transfer
                    || orderType == OrderType.SubContractTransfer)
                {
                    whereStatement += " and " + transferHql;
                }
                else if (orderType == OrderType.Production)
                {
                    whereStatement += " (" +
                        orderTypeTableAlias + "." + orderTypeFieldName + "  = "
                        + (int)OrderType.Production
                        + "and ((" + partyToTableAlias + ".IsCheckPartyFromAuthority = 0 and " + partyToTableAlias + ".IsCheckPartyToAuthority = 0 "
                        + "and ( exists (select 1 from UserPermissionView as p where p.UserId ="
                        + user.Id + " and  p.PermissionCategoryType  = "
                        + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyFromTableAlias + "." + partyFromFieldName + ")"
                        + " or exists (select 1 from UserPermissionView as p where p.UserId ="
                        + user.Id + " and  p.PermissionCategoryType  = "
                        + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyToTableAlias + "." + partyToFieldName + ")))"

                        + " or (" + partyToTableAlias + ".IsCheckPartyFromAuthority = 0 and " + partyToTableAlias + ".IsCheckPartyToAuthority = 1 "
                            + " and (exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id
                        + " and  p.PermissionCategoryType =" + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyToTableAlias + "." + partyToFieldName + ")))"

                        + " or (" + partyToTableAlias + ".IsCheckPartyFromAuthority = 1 and " + partyToTableAlias + ".IsCheckPartyToAuthority = 0 "
                            + " and (exists (select 1 from UserPermissionView as p where p.UserId =" + user.Id
                        + " and  p.PermissionCategoryType =" + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyFromTableAlias + "." + partyFromFieldName + ")))"

                        + " or (" + partyToTableAlias + ".IsCheckPartyFromAuthority = 1 and " + partyToTableAlias + ".IsCheckPartyToAuthority = 1 "
                        + "and ( exists (select 1 from UserPermissionView as p where p.UserId ="
                        + user.Id + "and  p.PermissionCategoryType  = "
                        + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyFromTableAlias + "." + partyFromFieldName + "))"
                        + " and exists (select 1 from UserPermissionView as p where p.UserId ="
                        + user.Id + " and  p.PermissionCategoryType  = "
                        + (int)PermissionCategoryType.Region
                        + " and p.PermissionCode = " + partyToTableAlias + "." + partyToFieldName + "))))";
                }
                else
                {
                    whereStatement += " 1=0 ";
                }
                whereStatement += " ) ";

                //var orderTypePermissions = user.OrderTypePermissions;
                //string orderTypes = " and (" + orderTypeTableAlias + "." + orderTypeFieldName + " in ( 10";
                //foreach (var orderTypePermission in orderTypePermissions)
                //{
                //    orderTypes += "," + ((int)orderTypePermission).ToString();
                //}
                //orderTypes += " ))";
                //whereStatement += orderTypes;
            }
        }

        public static void AddRegionPermissionStatement(ref string whereStatement, string regionTableAlias, string regionFieldName)
        {
            //su特殊处理，不用考虑权限
            User user = SecurityContextHolder.Get();
            if (user.Code.Trim().ToLower() != "su")
            {
                if (whereStatement == string.Empty)
                {
                    whereStatement = " where exists (select 1 from UserPermissionView as up where up.UserId =" + user.Id
                        + "and  up.PermissionCategoryType = " + (int)PermissionCategoryType.Region
                        + "  and up.PermissionCode = " + regionTableAlias + "." + regionFieldName + ")";
                }
                else
                {
                    whereStatement += " and exists (select 1 from UserPermissionView as up where up.UserId =" + user.Id
                        + "and  up.PermissionCategoryType = " + (int)PermissionCategoryType.Region
                        + "  and up.PermissionCode = " + regionTableAlias + "." + regionFieldName + ")";
                }
            }
        }

        public static void AddLocationPermissionStatement(ref string whereStatement, string locationTableAlias, string locationFieldName)
        {
            //su特殊处理，不用考虑权限
            User user = SecurityContextHolder.Get();
            if (user.Code.Trim().ToLower() != "su")
            {
                if (whereStatement == string.Empty)
                {
                    whereStatement = @" where exists (select 1 from UserPermissionView as up,Location as ln where up.UserId ="
                        + user.Id + "and  up.PermissionCategoryType = " + (int)PermissionCategoryType.Region
                        + "  and up.PermissionCode = ln.Region and ln.Code = " + locationTableAlias + "." + locationFieldName + ")";
                }
                else
                {
                    whereStatement += @" and exists (select 1 from UserPermissionView as up,Location as ln where up.UserId ="
                        + user.Id + "and  up.PermissionCategoryType = " + (int)PermissionCategoryType.Region
                        + "  and up.PermissionCode = ln.Region and ln.Code = " + locationTableAlias + "." + locationFieldName + ")";
                }
            }
        }

        /// <summary>
        /// 不完善
        /// </summary>
        public static void AddProductLinePermissionStatement(ref string whereStatement, string flowTableAlias, string flowFieldName)
        {
            //            //su特殊处理，不用考虑权限
            //            User user = SecurityContextHolder.Get();

            //            if (whereStatement == string.Empty)
            //            {
            //                whereStatement += " where ";
            //            }
            //            else
            //            {
            //                whereStatement += " and ";
            //            }

            //            whereStatement = @"  exists (select 1 from UserPermissionView as up1,FlowMaster as fm1 
            //                               where (fm1.IsCheckPartyFromAuthority = 0) or (up1.UserId =" + user.Id
            //                              + " and  up1.PermissionCategoryType in ("
            //                              + (int)PermissionCategoryType.Region
            //                              + ")  and up1.PermissionCode = fm1.PartyFrom and fm1.Code = "
            //                              + flowTableAlias + "." + flowFieldName + "))";

            //            whereStatement += @" and exists (select 1 from UserPermissionView as up2,FlowMaster as fm2 
            //                                where (fm2.IsCheckPartyToAuthority = 0) or (up2.UserId ="
            //                                + user.Id + " and  up2.PermissionCategoryType in ("
            //                                + (int)PermissionCategoryType.Region
            //                                + ")  and up2.PermissionCode = fm2.PartyTo and fm2.Code = "
            //                                + flowTableAlias + "." + flowFieldName + "))";
        }

        public static bool HasPermission(OrderMaster orderMaster, bool isSupplier = false)
        {
            return HasPermission(orderMaster.Type, orderMaster.IsCheckPartyFromAuthority, orderMaster.IsCheckPartyToAuthority, orderMaster.PartyFrom, orderMaster.PartyTo, isSupplier, true, orderMaster.SubType == OrderSubType.Return);
        }

        public static bool HasPermission(FlowMaster flowMaster, bool isSupplier = false, bool isCreateOrder = false)
        {
            return HasPermission(flowMaster.Type, flowMaster.IsCheckPartyFromAuthority, flowMaster.IsCheckPartyToAuthority, flowMaster.PartyFrom, flowMaster.PartyTo, isSupplier, isCreateOrder);
        }

        public static bool HasPermission(IpMaster ipMaster, bool isSupplier = false)
        {
            return HasPermission(ipMaster.OrderType, ipMaster.IsCheckPartyFromAuthority, ipMaster.IsCheckPartyToAuthority, ipMaster.PartyFrom, ipMaster.PartyTo, isSupplier, false);
        }

        public static bool HasPermission(PickListMaster pickListMaster, bool isSupplier = false)
        {
            return HasPermission(pickListMaster.OrderType, pickListMaster.IsCheckPartyFromAuthority, pickListMaster.IsCheckPartyToAuthority, pickListMaster.PartyFrom, pickListMaster.PartyTo, isSupplier, false);
        }

        public static bool HasPermission(ReceiptMaster receiptMaster, bool isSupplier = false)
        {
            return HasPermission(receiptMaster.OrderType, receiptMaster.IsCheckPartyFromAuthority, receiptMaster.IsCheckPartyToAuthority, receiptMaster.PartyFrom, receiptMaster.PartyTo, isSupplier, false);
        }

        public static bool HasPermission(OrderType orderType, bool isCheckPartyFromAuthority, bool isCheckPartyToAuthority, string partyFrom, string partyTo, bool isSupplier, bool isCreateOrder = true, bool isReturn = false)
        {
            try
            {
                var user = SecurityContextHolder.Get();
                var regionPermissions = user.RegionPermissions;
                var supplierPersmissions = user.SupplierPersmissions;
                var customerPermissions = user.CustomerPermissions;
                var orderTypePermissions = user.OrderTypePermissions;
                if (!orderTypePermissions.Contains(orderType) && isCreateOrder)
                {
                    return false;
                }

                bool hasPermission = true;
                if (orderType == OrderType.Transfer || orderType == OrderType.Production || orderType == OrderType.SubContractTransfer)
                {
                    if (partyFrom == null)
                    {
                        hasPermission = true;
                    }
                    else
                    {
                        //移库生产等只需要有一边的区域权限就可以创建释放订单
                        hasPermission = regionPermissions.Contains(partyFrom) || regionPermissions.Contains(partyTo);
                        //if (isCheckPartyFromAuthority && isCheckPartyToAuthority)
                        //{
                        //    hasPermission = regionPermissions.Contains(partyFrom) && regionPermissions.Contains(partyTo);
                        //}
                        //else if (isCheckPartyFromAuthority && !isCheckPartyToAuthority)
                        //{
                        //    hasPermission = regionPermissions.Contains(partyFrom);
                        //}
                        //else if (!isCheckPartyFromAuthority && isCheckPartyToAuthority)
                        //{
                        //    hasPermission = regionPermissions.Contains(partyTo);
                        //}
                        //else if (!isCheckPartyFromAuthority && !isCheckPartyToAuthority)
                        //{
                        //    hasPermission = regionPermissions.Contains(partyFrom) || regionPermissions.Contains(partyTo);
                        //}
                    }
                }
                else if (orderType == OrderType.Procurement || orderType == OrderType.CustomerGoods || orderType == OrderType.SubContract || orderType == OrderType.ScheduleLine)
                {
                    //采购的不看供应商权限，只要有目的区域权限就可以
                    if (isReturn)
                    {
                        if (isSupplier == true)
                        {
                            hasPermission = supplierPersmissions.Contains(partyTo);
                        }
                        else
                        {
                            hasPermission = regionPermissions.Contains(partyFrom);
                        }
                    }
                    else
                    {
                        if (isSupplier == true)
                        {
                            hasPermission = supplierPersmissions.Contains(partyFrom);
                        }
                        else
                        {
                            hasPermission = regionPermissions.Contains(partyTo);
                        }
                    }
                    //if (isCheckPartyToAuthority && !isSupplier)
                    //{
                    //    if (orderType == OrderType.CustomerGoods)
                    //    {
                    //        hasPermission = customerPermissions.Contains(partyFrom) && regionPermissions.Contains(partyTo);
                    //    }
                    //    else
                    //    {
                    //        hasPermission = supplierPersmissions.Contains(partyFrom) && regionPermissions.Contains(partyTo);
                    //    }
                    //}
                    //else
                    //{
                    //    if (orderType == OrderType.CustomerGoods)
                    //    {
                    //        hasPermission = customerPermissions.Contains(partyFrom);
                    //    }
                    //    else
                    //    {
                    //        hasPermission = supplierPersmissions.Contains(partyFrom);
                    //    }
                    //}
                }
                else if (orderType == OrderType.Distribution || orderType == OrderType.CustomerGoods)
                {
                    hasPermission = regionPermissions.Contains(partyFrom);
                    //if (isCheckPartyFromAuthority)
                    //{
                    //    hasPermission = regionPermissions.Contains(partyFrom) && customerPermissions.Contains(partyTo);
                    //}
                    //else
                    //{
                    //    hasPermission = customerPermissions.Contains(partyTo);
                    //}
                }
                return hasPermission;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}
