using System.Collections.Generic;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ServiceTagModel : BaseModel
    {
        public ServiceTagModel() { }
        public ServiceTagModel(DataRow row) : base(row) { }
        public int PKID { get; set; }
        public string Category { get; set; }
        public string PID { get; set; }
        public string ServiceTag { get; set; }
        public int Type { get; set; }
        public string DisplayName { get; set; }
        public List<string> DisplayNames { get; set; }
        public List<string> ServiceIDs { get; set; }
        public string StrServiceIDs { get; set; }
        public string StrDisplayNames { get; set; }
        public string ServiceDescribe { get; set; } //tag服务描述
        public string ServiceDescribeTID { get; set; }//tagid 服务描述

        public string ProductTag { get; set; }

        public string ProductDescribe { get; set; }

    }
}
