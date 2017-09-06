namespace com.Sconit.Service.SI.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using com.Sconit.Entity.SYS;

    public class SD_SecurityMgrImpl : BaseMgr, com.Sconit.Service.SI.ISD_SecurityMgr
    {
        public Entity.SI.SD_ACC.User GetUser(string userCode, string hashedPassword, string ipAddress)
        {
            Entity.ACC.User user = securityMgr.GetUserWithPermissions(userCode);
            if (user == null)
            {
                throw new Entity.Exception.BusinessException("用户不存在。");
            }

            var sdUser = new Entity.SI.SD_ACC.User();
            //sdUser.AccountExpired = user.AccountExpired;
            //sdUser.AccountLocked = user.AccountLocked;
            sdUser.Code = user.Code;
            sdUser.Email = user.Email;
            sdUser.FirstName = user.FirstName;
            sdUser.Id = user.Id;
            sdUser.Language = user.Language;
            sdUser.LastName = user.LastName;
            sdUser.MobilePhone = user.MobilePhone;
            sdUser.Password = user.Password;
            //sdUser.PasswordExpired = user.PasswordExpired;
            sdUser.TelPhone = user.TelPhone;
            sdUser.IsActive = securityMgr.VerifyUserPassword(user, hashedPassword);
            if (sdUser.IsActive == false)
            {
                throw new Entity.Exception.BusinessException("登录失败。");
            }
            if (user.Permissions != null)
            {
                user.Permissions = user.Permissions.Where(p => p.PermissionCategoryType != Sconit.CodeMaster.PermissionCategoryType.Url).ToList();
                sdUser.Permissions = Mapper.Map<IList<Entity.VIEW.UserPermissionView>, List<Entity.SI.SD_ACC.Permission>>(user.Permissions);

            }
            sdUser.BarCodeTypes = GetBarCodeTypes();

            this.genericMgr.Update("update from User set LastAccessDate = ? ,IpAddress = ? where Code =?",
                new object[] { DateTime.Now, ipAddress, userCode });
            return sdUser;
        }

        public Entity.ACC.User GetBaseUser(string userCode, bool withPermissions = false)
        {
            if (!withPermissions)
            {
                return securityMgr.GetUser(userCode);
            }
            else
            {
                return securityMgr.GetUserWithPermissions(userCode);
            }
        }

        private List<Entity.SI.SD_ACC.BarCodeType> GetBarCodeTypes()
        {
            List<Entity.SI.SD_ACC.BarCodeType> barCodeTypes = new List<Entity.SI.SD_ACC.BarCodeType>();

            var snRules = genericMgr.FindAll<SNRule>().OrderByDescending(s => s.PreFixed.Length);
            foreach (var rule in snRules)
            {
                switch ((com.Sconit.CodeMaster.DocumentsType)rule.Code)
                {
                    case com.Sconit.CodeMaster.DocumentsType.ORD_Procurement:
                    case com.Sconit.CodeMaster.DocumentsType.ORD_Transfer:
                    case com.Sconit.CodeMaster.DocumentsType.ORD_Distribution:
                    case com.Sconit.CodeMaster.DocumentsType.ORD_Production:
                    case com.Sconit.CodeMaster.DocumentsType.ORD_SubContract:
                    case com.Sconit.CodeMaster.DocumentsType.ORD_CustomerGoods:
                    case com.Sconit.CodeMaster.DocumentsType.ORD_SubContractTransfer:
                    case com.Sconit.CodeMaster.DocumentsType.ORD_ScheduleLine:
                        barCodeTypes.Add(
                            new Entity.SI.SD_ACC.BarCodeType() { PreFixed = rule.PreFixed, Type = Entity.SI.SD_ACC.OrdType.ORD }
                        );
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.ASN_Procurement:
                    case com.Sconit.CodeMaster.DocumentsType.ASN_Transfer:
                    case com.Sconit.CodeMaster.DocumentsType.ASN_Distribution:
                    case com.Sconit.CodeMaster.DocumentsType.ASN_CustomerGoods:
                    case com.Sconit.CodeMaster.DocumentsType.ASN_ScheduleLine:
                    case com.Sconit.CodeMaster.DocumentsType.ASN_SubContract:
                    case com.Sconit.CodeMaster.DocumentsType.ASN_SubContractTransfer:
                        barCodeTypes.Add(
                         new Entity.SI.SD_ACC.BarCodeType() { PreFixed = rule.PreFixed, Type = Entity.SI.SD_ACC.OrdType.ASN }
                     );
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.PIK_Transfer:
                    case com.Sconit.CodeMaster.DocumentsType.PIK_Distribution:
                    case com.Sconit.CodeMaster.DocumentsType.PIK_SubContractTransfer:
                        barCodeTypes.Add(
                         new Entity.SI.SD_ACC.BarCodeType() { PreFixed = rule.PreFixed, Type = Entity.SI.SD_ACC.OrdType.PIK }
                     );
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.STT:
                        barCodeTypes.Add(
                         new Entity.SI.SD_ACC.BarCodeType() { PreFixed = rule.PreFixed, Type = Entity.SI.SD_ACC.OrdType.STT }
                     );
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.INS:
                        barCodeTypes.Add(
                         new Entity.SI.SD_ACC.BarCodeType() { PreFixed = rule.PreFixed, Type = Entity.SI.SD_ACC.OrdType.INS }
                     );
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.SEQ:
                        barCodeTypes.Add(
                         new Entity.SI.SD_ACC.BarCodeType() { PreFixed = rule.PreFixed, Type = Entity.SI.SD_ACC.OrdType.SEQ }
                     );
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.BIL_Procurement:
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.BIL_Distribution:
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.RED_Procurement:
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.RED_Distribution:
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.MIS_In:
                    case com.Sconit.CodeMaster.DocumentsType.MIS_Out:
                        barCodeTypes.Add(
                         new Entity.SI.SD_ACC.BarCodeType() { PreFixed = rule.PreFixed, Type = Entity.SI.SD_ACC.OrdType.MIS });
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.REJ:
                        break;
                    case com.Sconit.CodeMaster.DocumentsType.REC_Procurement:
                    case com.Sconit.CodeMaster.DocumentsType.REC_Transfer:
                    case com.Sconit.CodeMaster.DocumentsType.REC_Distribution:
                    case com.Sconit.CodeMaster.DocumentsType.REC_Production:
                    case com.Sconit.CodeMaster.DocumentsType.REC_SubContract:
                    case com.Sconit.CodeMaster.DocumentsType.REC_CustomerGoods:
                    case com.Sconit.CodeMaster.DocumentsType.CON:
                        break;
                    default:
                        break;
                }
            }
            return barCodeTypes;
        }

        public void CreateAccessLog(Entity.SYS.AccessLog accesslog)
        {
            this.genericMgr.Create(accesslog);

            string smartDeviceVersion = systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.SmartDeviceVersion, false);

            if (!string.IsNullOrWhiteSpace(smartDeviceVersion) && smartDeviceVersion.CompareTo(accesslog.UserAgent.Split(' ')[0]) > 0)
            {
                throw new Entity.Exception.BusinessException("手持终端版本过低,不能登录。");
            }
        }
    }
}
