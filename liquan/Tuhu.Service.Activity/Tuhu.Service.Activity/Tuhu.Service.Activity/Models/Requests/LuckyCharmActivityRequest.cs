using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 添加锦鲤活动
    /// </summary>
    public class AddLuckyCharmActivityRequest
    {
        public ActivityTypeEnum ActivityType { get; set; }
        public string ActivityTitle { get; set; }
        public string ActivitySlug { get; set; }
        public string ActivityDes { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    /// <summary>
    /// 添加报名用户
    /// </summary>
    public class AddLuckyCharmUserRequest
    {
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
    }

    /// <summary>
    /// 修改报名用户信息
    /// </summary>
    public class UpdateLuckyCharmUserRequest
    {
        public int PKID { get; set; }
        public int ActivityId { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
    }

    /// <summary>
    /// 分页获取报名锦鲤活动用户
    /// </summary>
    public class PageLuckyCharmUserRequest : BaseQueryModel
    {
        public PageLuckyCharmUserRequest()
        {
            this.CheckState = -1;
        }
        public string AreaName { get; set; }
        public string Phone { get; set; }
        public int PKID { get; set; }

        public int CheckState { get; set; }
    }

    /// <summary>
    /// 分页获取锦鲤活动列表
    /// </summary>
    public class PageLuckyCharmActivityRequest : BaseQueryModel
    {
        public int PKID { get; set; }
    }



}
