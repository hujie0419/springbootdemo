using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Server.Config
{
    /// <summary>
    /// 砍价相关的 全局变量
    /// </summary>
    public static class GlobalConstantForBargain
    {
        /// <summary>
        /// 砍价的推送模板
        /// </summary>
        public enum PushBatchid
        {
            发起砍价 = 5365,
            砍价进度到百分之50 = 5368,
            砍价进度到还剩5人 = 5371,
            砍价成功 = 5374,//只推送给发起人
            帮砍成功 = 3857,//推送给帮砍人
            库存不足等于3 = 5383,//只推送给发起人 且正在砍价【未完成】
            库存不足等于0 = 5377,//只推送给发起人 且正在砍价【未完成】
            砍价商品自动过期 = 5380,//job 轮询判断
        }

        /// <summary>
        /// 砍价缓存 ClientName
        /// </summary>
        public static string BargainCacheClient = "ShareBargainMessageName";


        //砍价推送缓存key  - 【只推送一次】
        public static string PushDistinctCacheKey = "ShareBargainKey/{idKey}/{batchId}";
        //砍价推送缓存 过期时间 
        public static int PushDistinctCacheTTL = 3 * 24 * 60 * 60;
    }
}
