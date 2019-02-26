using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 获取车友群列表的请求对象
    /// </summary>
    public class GetCarFriendsGroupListRequest
    {
        /// <summary>
        /// 车型集合
        /// </summary>
        public List<string> VehicleList { get; set; }

        /// <summary>
        /// 是否热门推荐
        /// </summary>
        public bool IsRecommend { get; set; }

        /// <summary>
        /// 搜索车型
        /// </summary>
        public string SearchVehicleKey { get; set; }
    }
}
