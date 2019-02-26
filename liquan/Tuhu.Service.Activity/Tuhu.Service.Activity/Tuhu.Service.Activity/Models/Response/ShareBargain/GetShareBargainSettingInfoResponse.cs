using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// GetShareBargainSettingInfoAsync 获取砍价活动商品配置基本信息 接口返回参数
    /// </summary>
    public class GetShareBargainSettingInfoResponse
    {

        public int PKID { get; set; }

        /// <summary>
        /// 砍价活动ID - 固定值
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 产品开始显示时间
        /// </summary>
        public DateTime? ShowBeginTime { get; set; }

        /// <summary>
        /// 砍价活动上架时间
        /// </summary>
        public DateTime BeginDateTime { get; set; }

        /// <summary>
        /// 砍价活动下架时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 产品类型 1.商品 2.优惠券 
        /// </summary>
        public int ProductType { get; set; }

        /// <summary>
        /// 商品pid/券id
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品简称
        /// </summary>
        public string SimpleDisplayName { get; set; }

        /// <summary>
        /// 商品售价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 商品活动价
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// 商品库存上限
        /// </summary>
        public int TotalStockCount { get; set; }

        /// <summary>
        /// 商品剩余库存
        /// </summary>
        public int CurrentStockCount { get; set; }

        /// <summary>
        /// 砍价人群限制:1-老带新商品 ,0-普通砍价商品
        /// </summary>
        public int CutPricePersonLimit { get; set; }

        /// <summary>
        /// 砍价所需次数
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// 帮砍次数
        /// </summary>
        public int HelpCutPriceTimes { get; set; }

        /// <summary>
        /// 前几个砍价力度大的人数 - 0表示未配置
        /// </summary>
        public int? BigCutBeforeCount { get; set; }

        /// <summary>
        /// 砍价力度大的区间占目标价百分比 - 0表示未配置
        /// </summary>
        public int? BigCutPriceRate { get; set; }

        /// <summary>
        /// 显示序号,从小到大
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 商品宣传图片
        /// </summary>
        public string Image1 { get; set; }

        /// <summary>
        /// 小程序分享标题
        /// </summary>
        public string WXShareTitle { get; set; }

        /// <summary>
        /// APP分享ID
        /// </summary>
        public string APPShareId { get; set; }

        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 砍价成功提示
        /// </summary>
        public string SuccessfulHint { get; set; }

        /// <summary>
        /// 砍价商品详情页图片展示
        /// </summary>
        public List<string> ProductImgList { get; set; } = new List<string>();

    }


}
