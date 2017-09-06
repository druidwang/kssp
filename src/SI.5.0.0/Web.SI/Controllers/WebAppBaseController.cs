

namespace com.Sconit.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using AutoMapper;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Util;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity;
    using Telerik.Web.Mvc.UI;
    using System.Data;

    public class WebAppBaseController : BaseController
    {
        public ISystemMgr systemMgr { get; set; }
        public IQueryMgr queryMgr { get; set; }

        public IQueryMgr siMgr { get { return GetService<IQueryMgr>("siMgr"); } }

        private IList<string> errorMessages = null;
        private IList<string> warningMessages = null;
        private IList<string> successMessages = null;
        private static Int32? _maxRowSize { get; set; }

        private static IDictionary<Type, IList<CodeDetailDescriptionPropertyMeta>> codeDetailDescriptionPropertyCache = new Dictionary<Type, IList<CodeDetailDescriptionPropertyMeta>>();
        private static string codeDetailDescriptionPropertyCacheLock = string.Empty;

        protected Int32 MaxRowSize
        {
            get
            {
                if (!_maxRowSize.HasValue)
                {
                    _maxRowSize = int.Parse(this.siMgr.FindById<com.Sconit.Entity.SI.EntityPreference>
                        (com.Sconit.Entity.SI.BusinessConstants.MAXROWSIZE).Value
                        );
                }
                return _maxRowSize.Value;
            }
        }

        #region override methods
        protected override void ExecuteCore()
        {
            User user = (User)Session[WebConstants.UserSessionKey];
            var lang = string.Empty;
            if (user != null)
            {
                // set the culture from the user (session)  
                lang = user.Language;
                // save the location into cookie   
                HttpCookie _cookie = new HttpCookie(WebConstants.CookieCurrentUICultureKey, Thread.CurrentThread.CurrentUICulture.Name);
                _cookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Response.SetCookie(_cookie);

            }
            else
            {
                // load the culture info from the cookie  
                var cookie = HttpContext.Request.Cookies[WebConstants.CookieCurrentUICultureKey];
                if (cookie != null)
                {
                    // set the culture by the cookie content  
                    lang = cookie.Value;
                }
                else
                {
                    // set the culture by the location if not speicified 
                    if (HttpContext.Request.UserLanguages != null)
                    {
                        lang = HttpContext.Request.UserLanguages[0];
                    }
                }
                // set the lang value into route data   
                //RouteData.Values["lang"] = langHeader;
            }

            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
            }
            catch (CultureNotFoundException)
            {
                //do nothing using default culture
            }
            try
            {
                base.ExecuteCore();
            }
            catch (HttpException)
            {
            }
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            #region 添加Message
            FlushMessage();
            #endregion
            base.OnResultExecuting(filterContext);
        }
        #endregion

        #region Message
        protected void SaveBusinessExceptionMessage(BusinessException ex)
        {
            foreach (Message message in ex.GetMessages())
            {
                if (message.MessageType == Sconit.CodeMaster.MessageType.Error)
                {
                    SaveErrorMessage(message.GetMessageString());
                }
                else if (message.MessageType == Sconit.CodeMaster.MessageType.Warning)
                {
                    SaveWarningMessage(message.GetMessageString());
                }
                else if (message.MessageType == Sconit.CodeMaster.MessageType.Info)
                {
                    SaveSuccessMessage(message.GetMessageString());
                }
            }
        }

        protected void SaveErrorMessage(string message)
        {
            if (errorMessages == null)
            {
                errorMessages = new List<string>();
            }
            errorMessages.Add(message);
        }

        protected virtual void SaveErrorMessage(string message, params string[] messageParams)
        {
            if (errorMessages == null)
            {
                errorMessages = new List<string>();
            }
            errorMessages.Add(string.Format(message, messageParams));
        }

        //protected void SaveWarningMessage(BusinessWarningException ex)
        //{
        //    if (ex.MessageParams != null)
        //    {
        //        SaveWarningMessage(ex.Message, ex.MessageParams);
        //    }
        //    else
        //    {
        //        SaveWarningMessage(ex.Message);
        //    }
        //}

        protected void SaveWarningMessage(string message)
        {
            if (warningMessages == null)
            {
                warningMessages = new List<string>();
            }
            warningMessages.Add(message);
        }

        protected void SaveWarningMessage(string message, params string[] messageParams)
        {
            if (warningMessages == null)
            {
                warningMessages = new List<string>();
            }
            warningMessages.Add(string.Format(message, messageParams));
        }

        protected void SaveSuccessMessage(string message)
        {
            if (successMessages == null)
            {
                successMessages = new List<string>();
            }
            successMessages.Add(message);
        }

        protected void SaveSuccessMessage(string message, params string[] messageParams)
        {
            if (successMessages == null)
            {
                successMessages = new List<string>();
            }
            successMessages.Add(string.Format(message, messageParams));
        }

        protected void FlushMessage()
        {
            #region 添加Message
            if (errorMessages != null)
            {
                TempData[WebConstants.ErrorMessages] = errorMessages;
            }
            if (warningMessages != null)
            {
                TempData[WebConstants.WarningMessages] = warningMessages;
            }
            if (successMessages != null)
            {
                TempData[WebConstants.SuccessMessages] = successMessages;
            }
            #endregion
        }

        //protected void SaveBusinessExceptionMessage(BusinessException ex)
        //{
        //    foreach(Message message in ex.messageList)
        //    {
        //        MessageHolder.AddMessage(message);
        //    }
        //}

        //protected void SaveErrorMessage(string message)
        //{
        //    MessageHolder.AddErrorMessage(message);
        //}

        //protected virtual void SaveErrorMessage(string message, params string[] messageParams)
        //{
        //    MessageHolder.AddErrorMessage(message, messageParams);
        //}

        //protected void SaveWarningMessage(string message)
        //{
        //    MessageHolder.AddWarningMessage(message);
        //}

        //protected void SaveWarningMessage(string message, params string[] messageParams)
        //{
        //    MessageHolder.AddWarningMessage(message, messageParams);
        //}

        //protected void SaveSuccessMessage(string message)
        //{
        //    MessageHolder.AddInfoMessage(message);
        //}

        //protected void SaveSuccessMessage(string message, params string[] messageParams)
        //{
        //    MessageHolder.AddInfoMessage(message, messageParams);
        //}
        #endregion

        #region Code Master
        protected SelectList GetCodeDetailDropDownList(com.Sconit.CodeMaster.CodeMaster code)
        {
            IList<CodeDetail> codeDetailList = systemMgr.GetCodeDetails(code);
            return Transfer2DropDownList(code, codeDetailList);
        }

        protected SelectList GetCodeDetailDropDownList(com.Sconit.CodeMaster.CodeMaster code, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue)
        {
            IList<CodeDetail> codeDetailList = systemMgr.GetCodeDetails(code, includeBlankOption, blankOptionDescription, blankOptionValue);
            return Transfer2DropDownList(code, codeDetailList);
        }

        protected SelectList Transfer2DropDownList(com.Sconit.CodeMaster.CodeMaster code, IList<CodeDetail> codeDetailList)
        {
            return Transfer2DropDownList(code, codeDetailList, null);
        }

        protected SelectList Transfer2DropDownList(com.Sconit.CodeMaster.CodeMaster code, IList<CodeDetail> codeDetailList, string selectedValue)
        {
            IList<SelectListItem> itemList = Mapper.Map<IList<CodeDetail>, IList<SelectListItem>>(codeDetailList);

            foreach (var item in itemList)
            {
                item.Text = systemMgr.TranslateCodeDetailDescription(item.Text);
            }

            return new SelectList(itemList, "Value", "Text", selectedValue);
        }

        protected void FillCodeDetailDescription<T>(T obj)
        {
            if (obj != null)
            {
                Type type = typeof(T);
                IList<CodeDetailDescriptionPropertyMeta> metaList = TryGetCachedCodeDetailDescriptionPropertyMetaList(type);

                foreach (CodeDetailDescriptionPropertyMeta meta in metaList)
                {
                    object refCodeValue = meta.ReferencePropertyInfo.GetValue(obj, null);
                    if (refCodeValue != null && refCodeValue.ToString().Trim() != string.Empty)
                    {
                        meta.TargetPropertyInfo.SetValue(obj, systemMgr.GetCodeDetailDescription(meta.CodeMaster, refCodeValue.ToString()), null);
                    }
                }
            }
        }

        protected void FillCodeDetailDescription<T>(IList<T> objList)
        {
            if (objList != null && objList.Count > 0)
            {
                Type type = typeof(T);
                IList<CodeDetailDescriptionPropertyMeta> metaList = TryGetCachedCodeDetailDescriptionPropertyMetaList(type);

                foreach (CodeDetailDescriptionPropertyMeta meta in metaList)
                {
                    //Parallel.ForEach(objList, (obj) =>
                    //{
                    //    object refCodeValue = meta.ReferencePropertyInfo.GetValue(obj, null);
                    //    if (refCodeValue != null && refCodeValue.ToString().Trim() != string.Empty)
                    //    {
                    //        meta.TargetPropertyInfo.SetValue(obj, systemMgr.GetCodeDetailDescription(meta.CodeMaster, refCodeValue.ToString()), null);
                    //    }
                    //});
                    foreach (T obj in objList)
                    {
                        object refCodeValue = meta.ReferencePropertyInfo.GetValue(obj, null);
                        if (refCodeValue != null && refCodeValue.ToString().Trim() != string.Empty)
                        {
                            if (refCodeValue.GetType() == typeof(short))
                            {
                                //meta.TargetPropertyInfo.SetValue(obj, systemMgr.GetCodeDetailDescription(meta.CodeMaster, ((int)refCodeValue).ToString()), null);
                                meta.TargetPropertyInfo.SetValue(obj, systemMgr.GetCodeDetailDescription(meta.CodeMaster, int.Parse(refCodeValue.ToString()).ToString()), null);

                            }
                            else if (refCodeValue.GetType() != typeof(string))
                            {
                                //如果refCodeValue不是String类型的，我们认为是CodeDetail定义的Enum值，并且都为int类型
                                //可能会有问题
                                meta.TargetPropertyInfo.SetValue(obj, systemMgr.GetCodeDetailDescription(meta.CodeMaster, ((int)refCodeValue).ToString()), null);

                            }
                            else
                            {
                                meta.TargetPropertyInfo.SetValue(obj, systemMgr.GetCodeDetailDescription(meta.CodeMaster, refCodeValue.ToString()), null);
                            }
                        }
                    }
                }
            }
        }

        private IList<CodeDetailDescriptionPropertyMeta> TryGetCachedCodeDetailDescriptionPropertyMetaList(Type type)
        {
            if (!codeDetailDescriptionPropertyCache.ContainsKey(type))
            {
                lock (codeDetailDescriptionPropertyCacheLock)
                {
                    //缓存反射的结果
                    codeDetailDescriptionPropertyCache.Add(type, new List<CodeDetailDescriptionPropertyMeta>());
                    PropertyInfo[] props = type.GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        object[] atrributes = prop.GetCustomAttributes(typeof(CodeDetailDescriptionAttribute), false);

                        if (atrributes.Length > 0)
                        {
                            CodeDetailDescriptionAttribute atrribute = (CodeDetailDescriptionAttribute)atrributes[0];

                            CodeDetailDescriptionPropertyMeta meta = new CodeDetailDescriptionPropertyMeta();
                            meta.TargetPropertyInfo = prop;
                            meta.ReferencePropertyInfo = type.GetProperty(atrribute.ValueField);
                            meta.CodeMaster = atrribute.CodeMaster;

                            codeDetailDescriptionPropertyCache[type].Add(meta);
                        }
                    }
                }
            }

            return codeDetailDescriptionPropertyCache[type];
        }
        #endregion

        #region MenuTree
        protected IList<MenuModel> GetAuthrizedMenuTree()
        {
            if (this.CurrentUser == null || this.CurrentUser.UrlPermissions == null)
            {
                return null;
            }

            IList<com.Sconit.Entity.SYS.Menu> allMenu = systemMgr.GetAllMenu();
            IList<MenuModel> allMenuModel = Mapper.Map<IList<com.Sconit.Entity.SYS.Menu>, IList<MenuModel>>(allMenu);

            IList<string> userUrlPermisson = this.CurrentUser.UrlPermissions;

            //平板的Menu
            List<MenuModel> flatMenuList = (from m in allMenuModel
                                            join u in userUrlPermisson on m.Code equals u
                                            select m).ToList();

            //因为不会对父菜单授权, 需要循环取得所有子菜单的父菜单
            NestGetParentMenu(flatMenuList, flatMenuList, allMenuModel);

            foreach (var menu in flatMenuList)
            {
                string name = Resources.Menu.ResourceManager.GetString(menu.Name);
                if (name != null)
                {
                    menu.Name = name;
                }
            }

            //////菜单多语言(the muilty thread will let the globalization uneffect)
            ////Parallel.ForEach(flatMenuList, (menu) =>
            ////{
            ////    string name = Resources.Menu.ResourceManager.GetString(menu.Name);
            ////    if (name != null)
            ////    {
            ////        menu.Name = name;
            ////    }
            ////});

            //根Menu
            IList<MenuModel> menuTree = (from m in flatMenuList
                                         where (m.ParentMenuCode == null || m.ParentMenuCode == string.Empty)
                                         && (m.Code.StartsWith("Url_SI"))
                                         orderby m.Sequence
                                         select m).ToList();

            //循环得到子Menu
            foreach (MenuModel menu in menuTree)
            {
                menu.ChildrenMenu = NestGetChildrenMenu(menu, flatMenuList);
            }

            return menuTree;
        }

        protected void NestGetParentMenu(IList<MenuModel> currentLevelMenuList, List<MenuModel> flatMenuList, IList<MenuModel> allMenuList)
        {
            IList<string> parentMenuCodeList = (from m in currentLevelMenuList
                                                select m.ParentMenuCode).Distinct().ToList();

            if (parentMenuCodeList.Count > 0)
            {
                List<MenuModel> parentMenuList = (from m in allMenuList
                                                  join p in parentMenuCodeList on m.Code equals p
                                                  where !flatMenuList.Contains(m)  //过滤掉已经选择的菜单
                                                  select m).ToList();

                flatMenuList.AddRange(parentMenuList);

                NestGetParentMenu(parentMenuList, flatMenuList, allMenuList);
            }
        }

        protected IList<MenuModel> NestGetChildrenMenu(MenuModel menu, IList<MenuModel> allMenuList)
        {
            IList<MenuModel> childrenMenu = (from m in allMenuList
                                             where m.ParentMenuCode == menu.Code
                                             orderby m.Sequence
                                             select m).ToList();

            if (childrenMenu != null && childrenMenu.Count() > 0)
            {
                foreach (MenuModel childMenu in childrenMenu)
                {
                    childMenu.ChildrenMenu = NestGetChildrenMenu(childMenu, allMenuList);
                }
            }

            return childrenMenu;
        }
        #endregion

        #region Grid

        #region Paging
        protected IList<T> GetPageData<T>(SearchStatementModel searchStatementModel)
        {
            return ExcutePageDataHql<T>(searchStatementModel, null);
        }

        protected IList<T> GetPageData<T>(SearchStatementModel searchStatementModel, GridCommand command)
        {
            ViewBag.Total = ExcutePageTotalHql(searchStatementModel);
            return ExcutePageDataHql<T>(searchStatementModel, command);
        }

        protected GridModel<T> GetAjaxPageData<T>(SearchStatementModel searchStatementModel, GridCommand command)
        {
            GridModel<T> GridModel = new GridModel<T>();
            GridModel.Total = ExcutePageTotalHql(searchStatementModel);
            GridModel.Data = ExcutePageDataHql<T>(searchStatementModel, command);
            ViewBag.Total = GridModel.Total;
            return GridModel;
        }

        private int ProcessPageSize(int pageSize)
        {
            return pageSize != 0 ? pageSize : int.Parse(systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultPageSize));
        }

        private int ExcutePageTotalHql(SearchStatementModel searchStatementModel)
        {
            return (int)siMgr.FindAll<long>(searchStatementModel.GetSearchCountStatement(), searchStatementModel.Parameters)[0];
        }

        private IList<T> ExcutePageDataHql<T>(SearchStatementModel searchStatementModel, GridCommand command)
        {
            IList<T> result;
            if (command != null)
            {
                command.PageSize = ProcessPageSize(command.PageSize);
                ViewBag.PageSize = command.PageSize;
                result = siMgr.FindAll<T>(searchStatementModel.GetSearchStatement(), searchStatementModel.Parameters, (command.Page - 1) * command.PageSize, command.PageSize);
            }
            else
            {
                result = siMgr.FindAll<T>(searchStatementModel.GetSearchStatement(), searchStatementModel.Parameters);
            }

            if (result == null)
            {
                //避免ajax请求返回null时报找不到View页面错误
                return new List<T>();
            }

            //填充CodeDetail的描述
            this.FillCodeDetailDescription<T>(result);

            return result;
        }

        #endregion

        #region Search Model Cache
        protected SearchCacheModel ProcessSearchModel(GridCommand command, SearchModelBase searchObj)
        {
            SearchCacheModel searchModel;
            string cacheKey = GetSearchCacheKey();

            if (!searchObj.isFromList.HasValue || !searchObj.isFromList.Value)
            {
                searchModel = TryGetCachedSearchModel(cacheKey, command, searchObj);
                //TempData[searchObj.GetType().Name] = searchModel.SearchObject;
            }
            else
            {
                searchModel = new SearchCacheModel
                {
                    SearchObject = searchObj,
                    Command = command,
                };
            }

            //无论是否使用查询缓存，都要进行缓存
            TempData[searchObj.GetType().Name] = searchModel.SearchObject;
            SetSearchModelCache(cacheKey, searchModel);

            return searchModel;
        }

        private string GetSearchCacheKey()
        {
            return this.ControllerContext.HttpContext.Request.Path.ToUpper();
        }

        private SearchCacheModel TryGetCachedSearchModel(string searchKey, GridCommand command, object obj)
        {
            Dictionary<string, SearchCacheModel> searchCache = GetSearchCacheCollection();

            if (searchCache.Keys.Contains(searchKey))
            {
                return ((Dictionary<string, SearchCacheModel>)HttpContext.Session[WebConstants.SearchCacheSessionKey])[searchKey];
            }
            else
            {
                return new SearchCacheModel
                {
                    SearchObject = obj,
                    Command = command,
                    //CachedTime = DateTime.Now
                };
            }
        }

        private void SetSearchModelCache(string searchKey, SearchCacheModel searchModel)
        {
            Dictionary<string, SearchCacheModel> searchCache = GetSearchCacheCollection();
            searchModel.CachedTime = DateTime.Now;

            if (searchCache.Keys.Contains(searchKey))
            {
                searchCache[searchKey] = searchModel;
            }
            else if (searchCache.Count < int.Parse(systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.SessionCachedSearchStatementCount)))
            {
                searchCache.Add(searchKey, searchModel);
            }
            else
            {
                searchCache.Remove(searchCache.OrderByDescending(t => t.Value.CachedTime).First().Key);
                searchCache.Add(searchKey, searchModel);
            }
        }

        private Dictionary<string, SearchCacheModel> GetSearchCacheCollection()
        {
            if (HttpContext.Session[WebConstants.SearchCacheSessionKey] != null)
            {
                return (Dictionary<string, SearchCacheModel>)HttpContext.Session[WebConstants.SearchCacheSessionKey];
            }
            else
            {
                Dictionary<string, SearchCacheModel> searchCacheCollection = new Dictionary<string, SearchCacheModel>();
                HttpContext.Session[WebConstants.SearchCacheSessionKey] = searchCacheCollection;

                return searchCacheCollection;
            }
        }
        #endregion

        #endregion
    }

    class CodeDetailDescriptionPropertyMeta
    {
        public PropertyInfo TargetPropertyInfo { get; set; }  //CodeDetail的描述Property
        public com.Sconit.CodeMaster.CodeMaster CodeMaster { get; set; }  //CodeMaster Code
        public PropertyInfo ReferencePropertyInfo { get; set; }  //CodeDetail的Value Property
    }


    public class ViewModel
    {
        public DataTable Data { get; set; }
        public IEnumerable<GridColumnSettings> Columns { get; set; }
    }


}
