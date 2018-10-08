using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class ActivityProductInfo
    {
        public Guid ActivityId { get; set; }
        //活动名称
        public string ActivityName { get; set; }
        //微信Url
        public string WXUrl { get; set; }
        //AppUrl
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
        //开始时间
        public DateTime? StartTime { get; set; }
        //结束时间
        public DateTime? EndTime { get; set; }
        //头图
        public ImgAndProductMap HeadImg { get; set; }
        //无商品图片
        public ImgAndProductMap NoProductImg { get; set; }
        //其他尺寸商品
        public List<ImgAndProductMap> OtherImgAndProductMap { get; set; }
    }

    public class ImgAndProductMap
    {
        /// <summary>
        /// 尺寸
        /// </summary>
        public string ImgType { get; set; }
        /// <summary>
        /// 图片URL
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 位置编号
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 是否适配轮胎尺寸
        /// </summary>
        public bool IsAdaptaionTireSize { get; set; }
        /// <summary>
        /// 产品信息
        /// </summary>
        public ProductDetails ProductDetails { get; set; }
    }

    public class ProductDetails
    {
        /// <summary>
        /// 有广告语商品
        /// </summary>
        public List<TireProductInfo> SloganProductInfo { get; set; }
        /// <summary>
        /// 无广告语商品
        /// </summary>
        public List<TireProductInfo> NoSloganProductInfo { get; set; }
    }

    public class TireProductInfo
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string TireSize { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 促销价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 是否分期
        /// </summary>
        public bool IsInstallment { get; set; }
        /// <summary>
        /// 分期价格
        /// </summary>
        public string InstallmentPrice { get; set; }
        /// <summary>
        /// 每人限购
        /// </summary>
        public int? MaxQuantity { get; set; }
        /// <summary>
        /// 产品图片
        /// </summary>
        public string ProductImg { get; set; }
        /// <summary>
        /// 总限购
        /// </summary>
        public int? TotalQuantity { get; set; }
        /// <summary>
        /// 售出
        /// </summary>
        public int SaleOutQuantity { get; set; }
        /// <summary>
        /// 广告语
        /// </summary>
        public string AdvertiseTitle { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string CP_Brand { get; set; }
        /// <summary>
        /// 是否取消进度条
        /// </summary>
        public bool IsCancelProgressBar { get; set; }
    }
}
