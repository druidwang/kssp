using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace com.Sconit.Utility
{
    public static class ImageHelper
    {
        public static IHtmlString Image(this HtmlHelper html, string url)
        {

            return html.Image(url, (IDictionary<string, object>)null);

        }


        public static IHtmlString Image(this HtmlHelper html, string url, object htmlAttributes)
        {

            return html.Image(url, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

        }


        public static IHtmlString Image(this HtmlHelper html, string url, IDictionary<string, object> htmlAttributes)
        {

            var img = new TagBuilder("img");

            img.Attributes.Add("src", url);

            img.MergeAttributes(htmlAttributes);

            return new HtmlString(img.ToString());

        }

    }
}
