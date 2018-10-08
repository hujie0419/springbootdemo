using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Model.GFCard
{
    public class GFBankCardRecord
    {
        public int PKID { get; set; }
        /// <summary>
        /// 途虎用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 脱敏用户姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 卡级别，例如金卡
        /// </summary>
        public string CardLevel { get; set; }
        /// <summary>
        /// 业务类型激活或注销
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 数据源文件名
        /// </summary>
        public string SourceFileName { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
