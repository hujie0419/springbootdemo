using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 批量查询活动信息请求实体
    /// </summary>
    public class FlashSaleDataByActivityRequest
    {
        public Guid activityID { get; set; }

        public List<string> pids { get; set; }
    }
}
