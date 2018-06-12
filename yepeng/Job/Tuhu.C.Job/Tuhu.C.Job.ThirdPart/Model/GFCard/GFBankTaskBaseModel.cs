using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Model.GFCard
{
    public class GFBankTaskBaseModel
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
        /// 激活或注销或分享塞券
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 赛券状态，参照枚举GFTaskStatus
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 数据源文件名
        /// </summary>
        public string SourceFileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedDateTime { get; set; }
    }
}
