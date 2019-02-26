using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 用户进入游戏接口请求参数
    /// </summary>
    public class UserParticipateGameRequest : GameObjectRequest
    {
        public Guid UserID { get; set; }

    }
}
