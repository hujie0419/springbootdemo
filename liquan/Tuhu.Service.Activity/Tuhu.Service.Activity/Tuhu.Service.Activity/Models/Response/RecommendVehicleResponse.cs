using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// 热门推荐车型
    /// </summary>
    public class RecommendVehicleResponse
    {
        /// <summary>
        /// 绑定车型
        /// </summary>
        public string BindVehicleType { get; set; }

        /// <summary>
        /// 绑定车型ID
        /// </summary>
        public string BindVehicleTypeID { get; set; }
    }
}
