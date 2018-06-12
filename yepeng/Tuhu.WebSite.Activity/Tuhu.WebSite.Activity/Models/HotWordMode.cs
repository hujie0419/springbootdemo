using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class HotWordMode : BaseModel
    {
        /// <summary>ID</summary>
        public int id { get; set; }
        /// <summary>热词</summary>
        public string HotWord { get; set; }
        /// <summary>创建时间</summary>
        public string CreateTime { get; set; }
        /// <summary>开关</summary>
        public bool OnOff { get; set; }
        public HotWordMode() : base() { }
        public HotWordMode(DataRow row) : base(row) { }
    }
}