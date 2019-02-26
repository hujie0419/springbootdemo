using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// 获取用户最近获得的奖品 接口请求实体
    /// </summary>
    public class GetUserLatestPrizeResponse : GetGameUserInfoResponsePrizeItems
    {

        /// <summary>
        /// 用户领奖记录id
        /// </summary>
        public int PKID { get; set; }

    }
}
