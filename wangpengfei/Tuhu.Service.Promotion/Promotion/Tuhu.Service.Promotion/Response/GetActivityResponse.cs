using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 活动
    /// </summary>
    public class GetActivityResponse
    {
        /// <summary>
        /// 主键自增列
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string StatusName {
            get
            {
                //最好是改成枚举
                if (this.Status == 0)
                {
                    return "初始";
                }
                else if (this.Status == 1)
                {
                    return "已开始";
                }
                else if (this.Status == 2)
                {
                    return "已结束";
                }
                return "";
            }
        }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public string StartTimeStr { get { return this.StartTime.ToString("yyyy-MM-dd"); } }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public string EndTimeStr { get { return this.EndTime.ToString("yyyy-MM-dd").Replace("0001-01-01", ""); } }
    }
}
