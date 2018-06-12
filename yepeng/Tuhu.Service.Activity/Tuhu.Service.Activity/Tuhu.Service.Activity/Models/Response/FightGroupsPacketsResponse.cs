using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// 获取红包组列表信息
    /// </summary>
   public class FightGroupsPacketsProvideResponse
    {
        /// <summary>
        /// 是否创建成功
        /// </summary>
        public bool IsSuccess { get; set; }

        public string Msg { get; set; }

    }

    public class FightGroupPacketListResponse
    {
        /// <summary>
        /// 分享组列表
        /// </summary>
        public List<FightGroupsPacketsLogModel> Items { get; set; }

        /// <summary>
        /// 已经领取数
        /// </summary>
        public int? ProvideCount { get; set; }

        /// <summary>
        /// 结束日期时间戳
        /// </summary>
        public long EndDateTask { get; set; }


    }

}
