using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 活动申请
    /// </summary>
    public class GetUserActivityApplyResponse
    {
        /// <summary>
        /// ID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 人员ID
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityID { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }
        /// <summary>
        /// 通过时间
        /// </summary>
        public DateTime PassTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 逻辑删除
        /// </summary>
        public int IsDeleted { get; set; }
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
                if (this.Status == 1)
                {
                    return "未通过";
                }
                else if (this.Status == 2)
                {
                    return "已通过";
                }
                return "";
            }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public string ApplyTimeStr { get { return this.ApplyTime.ToString("yyyy-MM-dd HH:mm:ss"); } }
        /// <summary>
        /// 通过时间
        /// </summary>
        public string PassTimeStr { get { return this.PassTime.ToString("yyyy-MM-dd HH:mm:ss").Replace("0001-01-01 00:00:00", ""); } }
    }
}
