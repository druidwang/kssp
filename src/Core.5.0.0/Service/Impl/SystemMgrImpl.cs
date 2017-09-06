using System;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.SYS;
using NHibernate.Criterion;
using System.Threading.Tasks;
using EmitMapper;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity;
using com.Sconit.Entity.MSG;
using com.Sconit.Entity.Report;

namespace com.Sconit.Service.Impl
{
    public class SystemMgrImpl : BaseMgr, ISystemMgr
    {
        public IQueryMgr queryMgr { get; set; }
        private static IDictionary<EntityPreference.CodeEnum, string> entityPreferenceCache;
        private static IDictionary<com.Sconit.CodeMaster.CodeMaster, IList<CodeDetail>> codeMasterCache;
        private static IList<Menu> menuCache;
        private static User monitorUser;

        private static string entityPreferenceLock = string.Empty;
        private static string codeMasterLock = string.Empty;
        private static string menuCacheLock = string.Empty;

        public SystemMgrImpl()
        {
        }

        public void ResetCache()
        {
            entityPreferenceCache = null;
            codeMasterCache = null;
            menuCache = null;
        }

        public void LoadEntityPreferenceCache()
        {
            lock(menuCacheLock)
            {
                DetachedCriteria criteria = DetachedCriteria.For<EntityPreference>();
                IList<EntityPreference> epList = queryMgr.FindAll<EntityPreference>(criteria);

                entityPreferenceCache = new Dictionary<EntityPreference.CodeEnum, string>();

                foreach(EntityPreference ep in epList)
                {
                    entityPreferenceCache.Add(((EntityPreference.CodeEnum)ep.Id), ep.Value);
                }
            }
        }

        public string GetEntityPreferenceValue(EntityPreference.CodeEnum code, bool useCache = true)
        {
            if(useCache)
            {
                if(entityPreferenceCache == null)
                {
                    LoadEntityPreferenceCache();
                }
                return entityPreferenceCache[code];
            }
            else
            {
                IList<EntityPreference> epList = queryMgr.FindAll<EntityPreference>(
                    "from EntityPreference where Id = ? ", code, 0, 1);
                return epList.First().Value;
            }
        }

        public void LoadCodeDetailCache()
        {
            lock(codeMasterLock)
            {
                DetachedCriteria criteria = DetachedCriteria.For<CodeDetail>();
                IList<CodeDetail> codeDetailList = queryMgr.FindAll<CodeDetail>(criteria);


                var groupedCodeDetailList = from det in codeDetailList
                                            orderby det.Sequence
                                            group det by det.Code into result
                                            select new
                                            {
                                                Code = result.Key,
                                                List = result.ToList()
                                            };

                codeMasterCache = new Dictionary<com.Sconit.CodeMaster.CodeMaster, IList<CodeDetail>>();

                if(groupedCodeDetailList != null && groupedCodeDetailList.Count() > 0)
                {
                    foreach(var groupedCodeDetail in groupedCodeDetailList)
                    {
                        codeMasterCache.Add((com.Sconit.CodeMaster.CodeMaster)Enum.Parse(typeof(com.Sconit.CodeMaster.CodeMaster), groupedCodeDetail.Code), groupedCodeDetail.List);
                    }
                }
            }
        }

        public IDictionary<com.Sconit.CodeMaster.CodeMaster, IList<CodeDetail>> GetCodeDetailDictionary()
        {
            if(codeMasterCache == null)
            {
                LoadCodeDetailCache();
            }

            return codeMasterCache;
        }

        public IList<CodeDetail> GetCodeDetails(com.Sconit.CodeMaster.CodeMaster code)
        {
            if(codeMasterCache == null)
            {
                LoadCodeDetailCache();
            }

            IList<CodeDetail> returnList = new List<CodeDetail>();
            foreach(CodeDetail codeDetail in codeMasterCache[code])
            {
                returnList.Add(ObjectMapperManager.DefaultInstance.GetMapper<CodeDetail, CodeDetail>().Map(codeDetail));
            }

            return returnList;
        }
        public IList<CodeDetail> GetMultiCodeDetails(IList<string> Multicode)
        {
            if(codeMasterCache == null)
            {
                LoadCodeDetailCache();
            }
            com.Sconit.CodeMaster.CodeMaster code;
            IList<CodeDetail> returnList = new List<CodeDetail>();
            int i = Multicode.Count();
            for(int j = 0; j < i; j++)
            {
                code = (com.Sconit.CodeMaster.CodeMaster)Enum.Parse(typeof(com.Sconit.CodeMaster.CodeMaster), Multicode[j]);
                foreach(CodeDetail codeDetail in codeMasterCache[code])
                {
                    returnList.Add(ObjectMapperManager.DefaultInstance.GetMapper<CodeDetail, CodeDetail>().Map(codeDetail));
                }
            }
            return returnList;
        }
        public IList<CodeDetail> GetMultiCodeDetails(IList<string> Multicode, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        {
            IList<CodeDetail> codeDetailList = this.GetMultiCodeDetails(Multicode);

            if(includeBlankOption.HasValue && includeBlankOption.Value)
            {
                CodeDetail blankCodeDetail = new CodeDetail();
                blankCodeDetail.Description = blankOptionDescription != null ? blankOptionDescription : string.Empty;
                blankCodeDetail.Value = blankOptionValue != null ? blankOptionValue : string.Empty;
                codeDetailList.Insert(0, blankCodeDetail);
            }

            return codeDetailList;
        }
        public CodeDetail GetDefaultCodeDetail(com.Sconit.CodeMaster.CodeMaster code)
        {
            if(codeMasterCache == null)
            {
                LoadCodeDetailCache();
            }

            return (from cd in codeMasterCache[code]
                    where cd.IsDefault == true
                    select cd).SingleOrDefault();
        }

        public IList<CodeDetail> GetCodeDetails(com.Sconit.CodeMaster.CodeMaster code, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        {
            IList<CodeDetail> codeDetailList = this.GetCodeDetails(code);

            if(includeBlankOption.HasValue && includeBlankOption.Value)
            {
                CodeDetail blankCodeDetail = new CodeDetail();
                blankCodeDetail.Description = blankOptionDescription != null ? blankOptionDescription : string.Empty;
                blankCodeDetail.Value = blankOptionValue != null ? blankOptionValue : string.Empty;
                codeDetailList.Insert(0, blankCodeDetail);
            }

            return codeDetailList;
        }

        public string GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster code, string value)
        {
            if(codeMasterCache == null)
            {
                LoadCodeDetailCache();
            }

            if(codeMasterCache.ContainsKey(code))
            {
                CodeDetail codeDetail = codeMasterCache[code].Where(det => det.Value == value).SingleOrDefault();
                if(codeDetail != null)
                {
                    string desc = Resources.SYS.CodeDetail.ResourceManager.GetString(codeDetail.Description);
                    if(desc != null)
                    {
                        return desc;
                    }
                    else
                    {
                        return codeDetail.Description;
                        //throw new TechnicalException("Description define not correct of codeMaster [" + code + "] and value [" + value + "].");
                    }
                }
                else
                {
                    //throw new TechnicalException("CodeMaster [" + code + "] does not contain value [" + value + "].");
                    return Resources.SYS.CodeDetail.Errors_CodeDetail_ValueNotFound;
                }
            }
            else
            {
                throw new TechnicalException("CodeMaster [" + code + "] does not exist.");
            }
        }

        public string GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster code, int value)
        {
            return GetCodeDetailDescription(code, value.ToString());
        }

        public IList<CodeDetail> GetCodeDetailDescription(IList<CodeDetail> codeDetailList)
        {
            foreach(var codeDetail in codeDetailList)
            {
                codeDetail.Description = this.TranslateCodeDetailDescription(codeDetail.Description);
            }
            return codeDetailList;
        }

        public string TranslateCodeDetailDescription(string description)
        {
            if(string.IsNullOrWhiteSpace(description))
            {
                return string.Empty;
            }
            string desc = Resources.SYS.CodeDetail.ResourceManager.GetString(description);
            if(desc != null)
            {
                return desc;
            }
            else
            {
                return description;
            }
        }
        public string TranslaterccpExClassifyDescription(string description)
        {
            if(string.IsNullOrWhiteSpace(description))
            {
                return string.Empty;
            }
            string desc = Resources.MRP.FlowClassify.ResourceManager.GetString(description);
            if(desc != null)
            {
                return desc;
            }
            else
            {
                return description;
            }
        }
        public string TranslateEntityPreferenceDescription(string description)
        {
            string desc = Resources.SYS.EntityPreference.ResourceManager.GetString(description);
            if(desc != null)
            {
                return desc;
            }
            else
            {
                return description;
            }
        }

        public void LoadMenuCache()
        {
            lock(menuCacheLock)
            {
                DetachedCriteria criteria = DetachedCriteria.For<Menu>();
                criteria.Add(Expression.Eq("IsActive", true));
                menuCache = queryMgr.FindAll<Menu>(criteria);

                var custReportMasterList = queryMgr.FindAll<CustReportMaster>("from CustReportMaster where IsActive =?", true);

                foreach(var custReportMaster in custReportMasterList)
                {
                    var menu = new Menu();
                    menu.Code = custReportMaster.Code;
                    menu.Description = string.Format("报表管理-信息-{0}", custReportMaster.Name);
                    menu.ImageUrl = "~/Content/Images/Nav/Default.png";
                    menu.IsActive = true;
                    menu.Name = custReportMaster.Name;
                    menu.PageUrl = string.Format("~/CustReport/ListIndex?Code={0}", custReportMaster.Code);
                    menu.ParentMenuCode = "Menu.CustReport.Info";
                    menu.Sequence = custReportMaster.Seq;
                    menuCache.Add(menu);
                }
            }
        }

        public IList<Menu> GetAllMenu()
        {
            if(menuCache == null)
            {
                LoadMenuCache();
            }

            return menuCache;
        }

        public User GetMonitorUser()
        {
            if(monitorUser == null)
            {
                monitorUser = queryMgr.FindById<User>(BusinessConstants.SYSTEM_USER_MONITOR);
            }
            return monitorUser;
        }


        protected void CreateMessageQueue(string methodName, string paramValue)
        {
            MessageQueue messageQueue = new MessageQueue();
            messageQueue.MethodName = methodName;
            messageQueue.ParamValue = paramValue;
            messageQueue.Status = CodeMaster.MQStatusEnum.Pending;
            messageQueue.LastModifyDate = DateTime.Now;
            messageQueue.CreateTime = DateTime.Now;

        }
    }
}
