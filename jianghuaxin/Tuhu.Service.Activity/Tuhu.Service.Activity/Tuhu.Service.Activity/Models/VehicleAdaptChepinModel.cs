using System;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class VehicleAdaptChepinModel : BaseModel
    {
        ///<summary>主键</summary>
        public int PKID { get; set; }

        ///<summary> 车型Id </summary>
        public string VehicleId { get; set; }

        ///<summary> PID </summary>
        public string PID { get; set; }

        ///<summary>
        /// 七个自然日销量统计分数
        /// </summary>
        public int? SalesOrder { get; set; }


        /// <summary>品类</summary>
        public string CategoryName { get; set; }
    }

    public class VehicleAdaptChepinDetailModel : VehicleAdaptChepinModel
    {
        /// <summary>图片</summary>
        public string Image { get; set; }

        ///<summary>售价</summary>

        public decimal? SalePrice { get; set; }


        /// <summary>显示名称</summary>
        public string DisplayName { get; set; }
    }
}


