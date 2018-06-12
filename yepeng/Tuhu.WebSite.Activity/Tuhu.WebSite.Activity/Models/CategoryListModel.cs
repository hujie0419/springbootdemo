using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class CategoryListModel : BaseModel
    {
        /// <summary>类别ID/// </summary>
        public int id { get; set; }
        /// <summary>类别名称/// </summary>
        public string CategoryName { get; set; }
        /// <summary>类别图片/// </summary>
        public string CategoryImage { get; set; }
        /// <summary>排序（暂时没用）/// </summary>
        public int Sort { get; set; }
        public CategoryListModel() : base() { }
        public CategoryListModel(DataRow row) : base(row) { }
    }
}