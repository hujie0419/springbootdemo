using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 银行活动白名单
    /// </summary>
    public class BankActivityWhiteUsers
    {
        public int PKID { get; set; }
        /// <summary>
        /// 组配置Id
        /// </summary>
        public int GroupConfigId { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNum { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
}
