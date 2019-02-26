using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{

    /// <summary>
    /// 获取游戏实时排行榜 返回实体
    /// </summary>
    public class GetRankListResponse
    {

        /// <summary>
        /// 当前用户的排行信息
        /// </summary>
        public GameRankInfoModel LoginUserRank { get; set; }

        /// <summary>
        /// 实时排行榜信息
        /// </summary>
        public List<GameRankInfoModel> RankInfos { get; set; }

    }

    public class GameRankInfoModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 排名数
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 积分数
        /// </summary>
        public int Point { get; set; }

    }

}
