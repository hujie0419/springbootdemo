using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{

    /// <summary>
    /// 获取游戏实时排行榜 请求参数
    /// </summary>
    public class GetRankListAsyncRequest: GameObjectRequest
    {

        /// <summary>
        /// 有传用户id且用户有碎片时，返回结果包含用户的排名 
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 分页页数 默认1
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 分页页码 默认10
        /// </summary>
        public int PageSize { get; set; } = 10;

    }
}
