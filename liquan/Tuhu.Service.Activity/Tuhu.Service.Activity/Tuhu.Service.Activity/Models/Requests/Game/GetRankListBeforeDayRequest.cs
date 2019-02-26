using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// GetRankListBeforeDayAsync 请求参数
    /// </summary>
    public class GetRankListBeforeDayRequest : GameObjectRequest
    {
        public DateTime? DayTime { get; set; } 
    }
}
