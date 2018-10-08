using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Battery
{
    public class BatteryLevelUpEntity
    {
        [JsonIgnore]
        public int PKID { get; set; }
        /// <summary>
        /// 原始产品PID
        /// </summary>
        public string OriginalPID { get; set; }
        /// <summary>
        /// 升级后产品PID
        /// </summary>
        public string NewPID { get; set; }
        /// <summary>
        /// 提示语
        /// </summary>
        public string Copywriting { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        [JsonIgnore]
        public DateTime LsatUpdateDateTime { get; set; }

        [JsonIgnore]
        public DateTime CreateDateTime { get; set; }
    }
    public class BatteryLevelUpReslut : BatteryLevelUpEntity
    {
        /// <summary>
        /// 原始产品
        /// </summary>
        public string OriginalDisplayName { get; set; }
        /// <summary>
        /// 升级购产品
        /// </summary>
        public string NewDisplayName { get; set; }
    }
}
