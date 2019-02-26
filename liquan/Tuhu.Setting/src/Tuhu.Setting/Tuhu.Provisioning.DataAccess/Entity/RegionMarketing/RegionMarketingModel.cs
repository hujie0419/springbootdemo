using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.RegionMarketing
{
    public class RegionMarketingModel
    {
        public int PKID { get; set; }

        public Guid ActivityId { get; set; }

        public string ActivityName { get; set; }

        public string WXUrl { get; set; }

        public string AppUrl { get; set; }
        //分享图片
        public string ShareImg { get; set; }
        //分享标题
        public string ShareTitle { get; set; }
        //分享描述
        public string ShareDes { get; set; }
        //是否适配车型
        public bool IsAdaptationVehicle { get; set; }
        //活动规则
        public string ActivityRules { get; set; }
        //活动规则图片
        public string ActivityRulesImg { get; set; }

        public List<ActivityImageConfig> ImgList { get; set; }

        public List<RegionMarketingProductConfig> ProductList { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public int Total { get; set; }
    }

    public class ActivityImageConfig
    {
        public int PKID { get; set; }

        public Guid ActivityId { get; set; }

        public ActivityImageType Type { get; set; }

        public int Position { get; set; }

        public string ImgUrl { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }
    }

    public class RegionMarketingProductConfig
    {
        public int PKID { get; set; }

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
        //没人限购
        public int? MaxQuantity{get;set;}
        //总限购
        public int? TotalQuantity { get; set; }
        //广告语
        public string AdvertiseTitle { get; set; }
        //特殊条件   0:安全库存 1:小于等于1 2:小于等于4
        public int SpecialCondition { get; set; }

        public bool IsShow { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }
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
