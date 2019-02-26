using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{

    /// <summary>
    /// 获取用户最近次领奖信息
    /// </summary>
    public class GetUserLatestPrizeRequest: GameObjectRequest
    {

        public Guid UserId { get; set; }

    }
}
