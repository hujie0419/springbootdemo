using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangOprLog
    {
        [JsonIgnore]
        public long PKID { get; set; }
        public string LogType { get; set; }
        public string IdentityID { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string OperateUser { get; set; }
        public string Remarks { get; set; }
        [JsonIgnore]
        public DateTime CreateTime { get; set; }

        public BaoYangOprLog()
        {
            OldValue = "";
            NewValue = "";
            OperateUser = "";
            Remarks = "";
            IdentityID = "";
        }
    }
}
