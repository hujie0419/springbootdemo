using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 天天秒杀接口请求参数
    /// </summary>
    public class SelectSecKillBaseRequest
    {
        /// <summary>
        /// 是否排除查询产品标签数据,默认不排除
        /// </summary>
        public bool excludeProductTags { get; set; } = false;

    }

    /// <summary>
    /// 获取首页秒杀数据
    /// </summary>
    public class SelectHomeSecKillRequest: SelectSecKillBaseRequest
    {
        /// <summary>
        /// 取前 num 个产品
        /// </summary>
        public int topNum { get; set; }

        /// <summary>
        /// 活动场次日期
        /// </summary>
        public DateTime scheduleDate { get; set; }

    }

    public class SelectSecKillByIdRequest : SelectSecKillBaseRequest
    {
        /// <summary>
        /// 活动场次Id
        /// </summary>
        public Guid activityId { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int pageSize { get; set; }
    }
}
