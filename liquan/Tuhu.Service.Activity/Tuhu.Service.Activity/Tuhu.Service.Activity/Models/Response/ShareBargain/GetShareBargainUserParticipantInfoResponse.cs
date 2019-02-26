using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// GetShareBargainUserParticipantInfoAsync 获取砍价配置当前参与用户信息 接口返回实体
    /// </summary>
    public class GetShareBargainUserParticipantInfoResponse
    {
        /// <summary>
        /// 当前用户48小时倒计时还没结束的用户数量
        /// </summary>
        public int BargainUserCount { get; set; }

        /// <summary>
        /// 用户倒计时结束最晚时间
        /// </summary>
        public DateTime? LastUserEndDateTime { get; set; }

    }
}
