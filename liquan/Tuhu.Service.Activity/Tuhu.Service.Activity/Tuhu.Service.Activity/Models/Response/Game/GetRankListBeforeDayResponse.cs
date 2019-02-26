using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{

    /// <summary>
    /// GetRankListBeforeDayAsync 返回参数
    /// </summary>
    public class GetRankListBeforeDayResponse
    {
        /// <summary>
        /// 积分排名
        /// </summary>
        public List<GameRankInfoModel> RankList { get; set; }

    }
}
