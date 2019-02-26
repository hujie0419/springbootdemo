using System;
using System.Collections.Generic;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class VehicleAdaptTireModel : BaseModel
    {
        ///<summary>主键</summary>
        public int PKID { get; set; }

        ///<sumamry> 轮胎规格 </sumamry>
        public string TireSize { get; set; }

        ///<summary> 车型Id </summary>
        public string VehicleId { get; set; }

        ///<summary> PID </summary>
        public string PID { get; set; }

        ///<summary>
        /// 七个自然日销量统计分数
        /// </summary>

        public int? SalesOrder { get; set; }

    }

    public class VehicleAdaptTireDetailModel : VehicleAdaptTireModel
    {
        /// <summary>图片</summary>
        public string Image { get; set; }

        ///<summary>售价</summary>

        public decimal? SalePrice { get; set; }


        /// <summary>显示名称</summary>
        public string DisplayName { get; set; }

    }

    public class VehicleAdaptTireTireSizeDetailModel
    {
        public string TireSize;
        public IEnumerable<VehicleAdaptTireDetailModel> Products;
    }
}
