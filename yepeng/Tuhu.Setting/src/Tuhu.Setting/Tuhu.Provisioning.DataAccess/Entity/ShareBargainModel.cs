using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShareBargainBaseModel
    {
        public string PID { get; set; }
        public string ProductName { get; set; }
        public int CurrentStockCount { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 产品开始显示时间
        /// </summary>
        public DateTime? ShowBeginTime { get; set; }

        /// <summary>
        /// 产品类型 1.商品 2.优惠券 
        /// </summary>
        public int ProductType { get; set; }
    }
    public class ShareBargainItemModel : ShareBargainBaseModel
    {
        public int PKID { get; set; }
        public string Operator { get; set; }
    }
    public class ShareBargainProductModel : ShareBargainBaseModel
    {
        public int? PKID { get; set; }
        public decimal OriginalPrice { get; set; }
        public int Sequence { get; set; }
        public string Image1 { get; set; }
        public int TotalStockCount { get; set; }
        public int Times { get; set; }
        public string PageName { get; set; }
        /// <summary>
        /// 砍价成功提示
        /// </summary>
        public string SuccessfulHint { get; set; }
        /// <summary>
        /// 小程序分享标题
        /// </summary>
        public string WXShareTitle { get; set; }
        /// <summary>
        /// APP分享ID
        /// </summary>
        public string APPShareId { get; set; }
        /// <summary>
        /// 商品简称
        /// </summary>
        public string SimpleDisplayName { get; set; }
         
    }

    public class CheckPidResult
    {
        public int Code { get; set; }
        public string Info { get; set; }
    }

    public class BargainGlobalConfigModel
    {
        /// <summary>
        /// 背景主题编号
        /// </summary>
        public int BackgroundTheme { get; set; }
        /// <summary>
        /// 背景图片
        /// </summary>
        public string BackgroundImage { get; set; }
        /// <summary>
        /// 介绍页名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 配置规则数量
        /// </summary>
        public int RulesCount { get; set; }
        /// <summary>
        /// 规则数据
        /// </summary>
        public string QAData { get; set; }
        /// <summary>
        /// 规则数据（反序列化）
        /// </summary>
        public List<BargainRules> BargainRule { get; set; }
        /// <summary>
        /// 小程序列表分享文案
        /// </summary>
        public string WXAPPListShareText { get; set; }
        /// <summary>
        /// 小程序列表分享图片
        /// </summary>
        public string WXAPPListShareImg { get; set; }
        /// <summary>
        /// 小程序详情页分享文案
        /// </summary>
        public string WXAPPDetailShareText { get; set; }
        /// <summary>
        /// APP列表分享标签
        /// </summary>
        public string APPListShareTag { get; set; }
        /// <summary>
        /// APP详情分享标签
        /// </summary>
        public string AppDetailShareTag { get; set; }
        /// <summary>
        /// 轮播信息文案
        /// </summary>
        public string SliceShowText { get; set; }
    }
    public class BargainRules
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class BackgroundThemeModel
    {
        public int ThemeNum { get; set; }
        public string ThemeName { get; set; }
    }
    public class BargainProductRequest
    {
        /// <summary>
        /// 砍价商品上下架状态，1-上架，0-全部，2-下架，3-未上架
        /// </summary>
        public int OnSale { get; set; }
        public string ProductName { get; set; }
        public string PID { get; set; }
        public string Operator { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
