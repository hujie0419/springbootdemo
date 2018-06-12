using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class TiresActivityProductConfig
    {
        public Guid ActivityId { get; set; }

        public string ProductId { get; set; }
        //产品名称
        public string ProductName { get; set; }
        //尺寸
        public string Size { get; set; }
        //规格
        public string Specification { get; set; }
        //促销价
        public decimal Price { get; set; }
        //每人限购
        public int? MaxQuantity { get; set; }
        //总限购
        public int? TotalQuantity { get; set; }
        //广告语
        public string AdvertiseTitle { get; set; }
        //特殊条件   0:安全库存 1:小于等于1 2:小于等于4
        public int SpecialCondition { get; set; }
        //是否取消进度条
        public bool IsCancelProgressBar { get; set; }
    }

    public class SimpleTireProductInfo
    {
        public string PID { get; set; }
        //尺寸
        public string CP_Tire_Rim { get; set; }
        //规格
        public string CP_Tire_Width { get; set; }
        //规格
        public string CP_Tire_AspectRatio { get; set; }
    }
}
