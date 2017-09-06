using System;
using System.Collections.Generic;

namespace com.Sconit.Web.Models
{
    /// <summary>
    ///MenuModel 的摘要说明
    /// </summary>
    public class MenuModel : BaseModel
    {
        public string Code { get; set; }
        public string ParentMenuCode { get; set; }
        public Int32 Sequence { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PageUrl { get; set; }
        public string ImageUrl { get; set; }
        public IList<MenuModel> ChildrenMenu { get; set; }

        public int Level { get; set; }
        public int NewSequence { get; set; }

        public MenuModel()
        {
        }
    }
}