using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// 锦鲤活动
    /// </summary>
    public class LuckyCharmActivityModel
    {
        public int PKID { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }
        public string ActivityTitle { get; set; }
        public string ActivitySlug { get; set; }
        public string ActivityDes { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int IsDelete { get; set; }
    }


    /// <summary>
    /// 锦鲤用户
    /// </summary>
    public class LuckyCharmUserModel
    {
        public int PKID { get; set; }
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public ActivityStatusEnum status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int IsDelete { get; set; }

    }

    public enum ActivityStatusEnum
    {
        None = 0,
        Pass = 1,
    }

    public enum ActivityTypeEnum
    {
        None = 0,
        A = 1,
        B = 2,
    }

}
