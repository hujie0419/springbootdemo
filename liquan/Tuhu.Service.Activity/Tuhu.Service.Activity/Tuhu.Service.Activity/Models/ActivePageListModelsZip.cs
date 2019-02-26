using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;

namespace Tuhu.Service.Activity.Zip.Models
{
    public class ActivePageContentModel
    {
        public int Pkid { get; set; }

        public string Group { get; set; }
        /// <summary>
        /// -类型 0:轮胎 1：车品 2：链接 3：图片 4：优惠券 5：保养 6：限购轮胎 7：限购车品 8：活动规则 9：秒杀 10:轮毂 11：限购轮毂 12：其它 13：大翻盘 -2导航栏
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// --显示方式 -1：不选择 1：横向 0：竖向
        /// </summary>
        public int DisplayWay { get; set; }

        public string Pid { get; set; }

        public string ProductName { get; set; }

        //活动ID 对应老的 VID
        public Guid? ActivityId { get; set; }

        ///// <summary>
        ///// 产品原价
        ///// </summary>
        //[Column("ActivityPrice")]
        //public decimal OriginPrice { get; set; }

        public string TireSize { get; set; }

        /// <summary>
        /// --优惠券ID
        /// </summary>
        public Guid Cid { get; set; }

        /// <summary>
        /// --图片地址
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 行类型 对应老的 BigImg 0：一行一列 1：两列 2：三列 3：（网站）一行一列 4：四列 6：一行五列 7：一图三产品
        /// </summary>
        public int RowType { get; set; }

        /// <summary>
        /// --跳转链接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// --app跳转链接
        /// </summary>
        public string AppUrl { get; set; }

        /// <summary>
        /// --官网链接
        /// </summary>
        public string PcUrl { get; set; }

        public string Description { get; set; }

        public int OrderBy { get; set; }

        /// <summary> 已售出数量 </summary>
        /// <value> The sale out quantity. </value>
        public int SaleOutQuantity { get; set; }
        /// <summary> 库存数 </summary>
        /// <value> The total number of quantity. </value>
        /// 
        public int? TotalQuantity { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 适配过滤
        /// </summary>
        public int IsAdapter { get; set; }

        /// <summary>
        /// 产品价格(原价) 市场价格（伪原价）
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 活动价格 没有活动价格时显示的产品价格
        /// </summary>
        public decimal ActivityPrice { get; set; }

        /// <summary>
        /// 抢购模块显示标签  “即将开始”、“抢完了”、“马上抢”
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 购买的百分比
        /// </summary>
        public string Percent { get; set; }

        /// <summary>
        /// 显示标签样式
        /// </summary>
        public string LabelClass { get; set; }

        /// <summary>
        /// 行顺序
        /// </summary>
        public int Sequence { get; set; }

        public bool Onsale { get; set; }


        public bool IsShow { get; set; }

        /// <summary>
        /// 广告语
        /// </summary>
        public string AdvertiseTitle { get; set; }
        #region 6/19新增保养专用

        /// <summary>
        /// 保养服务
        /// </summary>
        public string ByService { get; set; }

        /// <summary>
        /// 保养活动id
        /// </summary>
        public string ByActivityId { get; set; }

        /// <summary>
        /// 车型要求
        /// </summary>
        public int? Vehicle { get; set; }
        public int? OthersType { get; set; }
        #endregion
        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WXAPPUrl { get; set; }


        public string Brand { get; set; }

        /// <summary>
        ///新大翻盘
        /// </summary>
        public string HashKey { get; set; }

        /// <summary>
        /// 拼团
        /// </summary>
        public string ProductGroupId { get; set; }



        #region AE动图
        public string FileUrl { get; set; }
        #endregion

        /// <summary>
        /// 文案
        /// </summary>
        public string ActiveText { get; set; }

        /// <summary>
        /// 文字链，优惠券等配置内容json格式存储
        /// </summary>
        public string JsonContent { get; set; }

        /// <summary>
        /// 倒计时样式：1,深色，2，浅色
        /// </summary>
        public int CountDownStyle { get; set; }

        /// <summary>
        /// 轮胎花纹
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// 英文花纹
        /// </summary>
        public string PatternEn { get; set; }

        /// <summary>
        /// 速度级别
        /// </summary>
        public string SpeedRating { get; set; }

        /// <summary>
        /// 文字链内容
        /// </summary>
        public List<Activity.Models.ScrollTextChainModel> ScrollTextChains { get; set; }

        /// <summary>
        /// 滑动优惠券内容
        /// </summary>
        public List<Activity.Models.SlipCouponModel> SlipCoupons { get; set; }

        #region 商品池类型新加
        /// <summary>
        /// 一行X列
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// 适配
        /// </summary>
        public string Tips { get; set; }

        /// <summary>
        /// 适配标签
        /// </summary>
        public int AdapterTag { get; set; }
        /// <summary>
        /// 商品池
        /// </summary>
        public List<ActivePageContentModel> Contents { get; set; }
        #region 新商品池

        /// <summary>
        /// 后台生成的活动id
        /// </summary>
        public string SystemActivityId { get; set; }
        #endregion

        #endregion
        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }

        /// <summary>
        /// 是否使用优惠券
        /// </summary>
        public bool IsUsePcode { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public string ExtendedProperty { get; set; }

        /// <summary>
        /// 类目
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 模板样式,1:极简版; 2:无按钮版;3:完整版 
        /// </summary>
        public int TemplateType { get; set; }

        /// <summary>
        /// 是否选择进度条
        /// </summary>
        public bool? IsProgressBar { get; set; }

        /// <summary>
        /// 是否选择品牌
        /// </summary>
        public bool? IsBrandName { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal? MarketingPrice { get; set; }

    }
    /// <summary>
    /// --活动页列表
    /// </summary>
    public class ActivePageListModel
    {
        #region 全局配置数据

        public int Pkid { get; set; }
        public Guid PkGuid { get; set; }

        public string Title { get; set; }

        public string H5Uri { get; set; }

        public string WwwUri { get; set; }
        /// <summary>
        /// --是否启用
        /// </summary>
        public int IsEnabled { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        /// <summary>
        /// --背景图片
        /// </summary>
        public string BgImageUrl { get; set; }

        /// <summary>
        /// --背景颜色
        /// </summary>
        public string BgColor { get; set; }
        /// <summary>
        /// --轮胎品牌 多个以逗号分隔
        /// </summary>
        public string TireBrand { get; set; }

        /// <summary>
        /// --活动类型 0:无 1:轮毂列表 2:轮胎列表
        /// </summary>
        public int ActivityType { get; set; }

        /// <summary>
        /// --分享参数
        /// </summary>
        public string DataParames { get; set; }

        /// <summary>
        /// --菜单类型 0:上下菜单 1：左右菜单
        /// </summary>
        public int MenuType { get; set; }

        /// <summary>
        /// --是否显示倒计时 1:显示 0:不显示
        /// </summary>
        public int IsShowDate { get; set; }
        /// <summary>
        /// --关键词图片
        /// </summary>
        public string SelKeyImage { get; set; }
        /// <summary>
        /// --关键词
        /// </summary>
        public string SelKeyName { get; set; }

        /// <summary>
        /// --是否开启轮胎适配 1:全部 2:轮胎 3:轮毂
        /// </summary>
        public int IsTireSize { get; set; }

        /// <summary>
        /// 客服
        /// </summary>
        public int? CustomerService { get; set; }
        #region 悬浮窗
        /// <summary>
        /// 悬浮窗 0无悬浮窗 1静图悬浮窗 2动图悬浮窗
        /// </summary>
        public int FloatWindow { get; set; }
        /// <summary>
        /// 悬浮窗图片
        /// </summary>
        public string FloatWindowImageUrl { get; set; }

        /// <summary>
        /// 悬浮窗跳转 0弹窗 1链接
        /// </summary>
        public int FloatWindowJump { get; set; }

        /// <summary>
        /// 弹窗图片
        /// </summary>
        public string AlertTabImageUrl { get; set; }

        /// <summary>
        /// app跳转
        /// </summary>
        public string AlertJumpApp { get; set; }

        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string AlertJumpWxApp { get; set; }
        #endregion
        /// <summary>
        /// 是否需要登录配置
        /// </summary>
        public int IsNeedLogIn { get; set; }
        #endregion
        /// <summary>
        /// 会场信息（主会场，分会场）
        /// </summary>

        /// <summary>
        /// 上下滚动滑动
        /// </summary>
        public List<Activity.Models.ActivePageMenuModel> MenuList { get; set; }

        /// <summary>
        /// 左右滑动产品集合以及对应产品集合
        /// </summary>
        public List<Activity.Models.SlideMenuList> SlideMenuList { get; set; }

        /// <summary>
        /// 全部菜单集合
        /// </summary>
        public List<Activity.Models.ActivePageMenuModel> AllMenuList { get; set; }

        public IEnumerable<Activity.Models.ActivePageHomeModel> ActivePageHomeModels { get; set; }

        /// <summary>
        /// 车型适配信息
        /// </summary>
        public Activity.Models.ActivePageTireSizeConfigModel ActivePageTireSizeConfigModel { get; set; }

        /// <summary>
        /// 活动页内容分组
        /// </summary>
        public List<ActivePageGroupContentModel> ActivePageGroupContents { get; set; }

        public Activity.Models.LuckyWheelModel LuckyWheelModel { get; set; }

        /// <summary>
        /// 秒杀产品
        /// </summary>
        public List<FlashSaleActivityMenu> FlashSaleRows { get; set; }

        public List<Activity.Models.BigBrandPageStyleModel> BigBrandPageStyleModels { get; set; }

        /// <summary>
        /// 问答抽奖模块页面样式
       /// </summary>
        public Activity.Models.BigBrandAnsQuesModel QAlotteryPageStyleModel { get; set; }

        public Activity.Models.FixedPriceActivityConfig FixedPriceActivityConfig { get; set; }
    }

    public class ActivePageGroupContentModel
    {
        public string Order { get; set; }
        public int OrigionType { get; set; }
        public int RowType { get; set; }
        public int Type { get; set; }
        public List<ActivePageContentModel> Contents { get; set; }
    }
    public class FlashSaleActivityMenu
    {
        public string Group { get; set; }

        /// <summary>
        /// 小时
        /// </summary>
        public string HourTime { get; set; }

        /// <summary>
        /// 状态 0正在抢购 -1 已结束 1即将开始
        /// </summary>
        public int Status { get; set; }


        public List<ActivePageGroupContentModel> Rows { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

    }
}


