using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using com.Sconit.Entity;

namespace com.Sconit.Utility
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString Button(this HtmlHelper htmlHelper, string buttonText, string permissions, IDictionary<string, string> attributeDic)
        {
            var user = SecurityContextHolder.Get();
            if (user == null)
            {
                return MvcHtmlString.Empty;
            }
            if (attributeDic.ContainsKey("orderType"))
            {
                if (!user.OrderTypePermissions.Contains((CodeMaster.OrderType)(int.Parse(attributeDic["orderType"]))))
                {
                    return MvcHtmlString.Empty;
                }
            }

            if (!string.IsNullOrWhiteSpace(permissions))
            {

                string[] permissionArray = permissions.Split(',');
                var q = user.UrlPermissions.Where(p => permissionArray.Contains(p)).ToList();
                if (q == null || q.Count() == 0)
                {
                    return MvcHtmlString.Empty;
                }
            }

            var button = new TagBuilder("button");
            button.SetInnerText(buttonText);

            if (attributeDic.ContainsKey("needconfirm") && bool.Parse(attributeDic["needconfirm"]))
            {
                string confirmContent = string.Empty;
                if (attributeDic.ContainsKey("confirmContent"))
                {
                    confirmContent += attributeDic["confirmContent"] + ",";
                }
                confirmContent += string.Format(Resources.SYS.Global.Button_ConfirmOperation, buttonText);
                if (attributeDic.ContainsKey("onclick"))
                {
                    attributeDic["onclick"] = "if( confirm('" + string.Format(Resources.SYS.Global.Button_ConfirmOperation, buttonText) + "')){" + attributeDic["onclick"] + "}";
                }
                else
                {
                    attributeDic.Add("onclick", "return confirm('" + string.Format(Resources.SYS.Global.Button_ConfirmOperation, buttonText) + "');");
                }
            }
            button.MergeAttributes(attributeDic);

            return new HtmlString("&nbsp;" + button.ToString());
        }

        public static string SimpleLink(this HtmlHelper html, string url, string text)
        {
            return String.Format("<a href=\"{0}\">{1}</a>", url, text);
        }
    }
}
