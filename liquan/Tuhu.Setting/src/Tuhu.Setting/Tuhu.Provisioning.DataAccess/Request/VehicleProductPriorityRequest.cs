using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority;

namespace Tuhu.Provisioning.DataAccess.Request
{
    public class VehicleOilProductPriorityRequest
    {

        public List<VehicleOilProductPriorityView> Views { get; set; }

        public List<ProductPriorityAreaOilDetailView> Details { get; set; }

        public int AreaId { get; set; }
    }

    public class VehicleProductPriorityRequst
    {

        public List<VehicleProductPriorityView> views { get; set; }

        public List<ProductPriorityAreaDetail> Details { get; set; }

        public int AreaId { get; set; }

        public string partName { get; set; }
    }

    public class VehicleProductPriorityRequest
    {
        public string PartName { get; set; }
        /// <summary>
        /// 地区配置ID
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 汽车品牌
        /// </summary>
        public string Brand { get; set; }

        public string VehicleId { get; set; }

        public decimal MaxPrice { get; set; }

        public decimal MinPrice { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Seq { get; set; }

        /// <summary>
        /// 商品品牌
        /// </summary>
        public string ProductBrand { get; set; }

        public string ProductSeries { get; set; }

        /// <summary>
        /// 车身类别
        /// </summary>
        public string VehicleBodyType { get; set; }
        /// <summary>
        /// 配置类别
        /// </summary>
        public VehicleProductPriorityRequestStatus Status { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
