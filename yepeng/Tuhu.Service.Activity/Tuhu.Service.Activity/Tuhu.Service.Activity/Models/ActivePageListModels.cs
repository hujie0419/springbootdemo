using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Tuhu.Service.Activity.Models
{
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
        public int FloatWindowJump{ get; set; }

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
        public List<ActivePageMenuModel> MenuList { get; set; }

        /// <summary>
        /// 左右滑动产品集合以及对应产品集合
        /// </summary>
        public List<SlideMenuList> SlideMenuList { get; set; }

        /// <summary>
        /// 全部菜单集合
        /// </summary>
        public List<ActivePageMenuModel> AllMenuList { get; set; }

        public IEnumerable<ActivePageHomeModel> ActivePageHomeModels { get; set; }


        /// <summary>
        /// 车型适配信息
        /// </summary>
        public ActivePageTireSizeConfigModel ActivePageTireSizeConfigModel { get; set; }

        /// <summary>
        /// 活动页内容分组
        /// </summary>
        public List<ActivePageGroupContentModel> ActivePageGroupContents { get; set; }

        /// <summary>
        /// 活动页未分组内容
        /// </summary>
        public List<ActivePageContentModel> ActivePageContents { get; set; }

        public LuckyWheelModel LuckyWheelModel { get; set; }

        public FixedPriceActivityConfig FixedPriceActivityConfig { get; set; }

        /// <summary>
        /// 秒杀产品
        /// </summary>
        public IOrderedEnumerable<FlashSaleActivityMenu> FlashSaleRows { get; set; }

        public List<BigBrandPageStyleModel> BigBrandPageStyleModels { get; set; }

        /// <summary>
        /// 问答抽奖模块页面样式
        /// </summary>
        public BigBrandAnsQuesModel QAlotteryPageStyleModel { get; set; }
    }
    /// <summary>
    /// 左右滑动菜单单独处理
    /// </summary>
    public class SlideMenuList
    {
        public string Group { get; set; }

        public List<ActivePageMenuModel> MenuList { get; set; }

        public List<ActivityMenuCell> SlideMenuContents { get; set; }
    }
    public class ActivityMenuCell
    {
        public string Group { get; set; }

        public List<ActivePageGroupContentModel> ActivePageGroupContentModels { get; set; }
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
    /// <summary>
    /// 活动页会场
    /// </summary>
    public class ActivePageHomeModel
    {
        public int Pkid { get; set; }

        public int FkActiveId { get; set; }

        /// <summary>
        /// --会场名称
        /// </summary>
        public string BigHomeName { get; set; }

        /// <summary>
        /// --图标
        /// </summary>
        public string HidBigHomePic { get; set; }

        /// <summary>
        /// --h5链接地址
        /// </summary>
        public string BigHomeUrl { get; set; }

        /// <summary>
        /// --网站图标
        /// </summary>
        public string HidBigHomePicWww { get; set; }

        /// <summary>
        /// --网站链接
        /// </summary>
        public string BigHomeUrlWww { get; set; }

        /// <summary>
        /// 小程序图标
        /// </summary>
        public string HidBigHomePicWxApp { get; set; }

        /// <summary>
        /// --小程序链接
        /// </summary>
        public string BigHomeUrlWxApp { get; set; }

        public int Sort { get; set; }

        /// <summary>
        /// 是否主会场
        /// </summary>
        public int IsHome { get; set; }

        /// <summary>
        /// 会场详情信息
        /// </summary>
        public IEnumerable<ActivePageHomeDeatilModel> ActivePageHomeDeatilModels { get; set; }
    }

    /// <summary>
    /// --会场详情信息
    /// </summary>
    public class ActivePageHomeDeatilModel
    {
        public int Pkid { get; set; }

        public int FkActiveHome { get; set; }

        public string HomeName { get; set; }

        public string HidBigFHomePic { get; set; }

        /// <summary>
        /// --h5链接
        /// </summary>
        public string BigFHomeMobileUrl { get; set; }

        /// <summary>
        /// --网站链接
        /// </summary>
        public string BigFHomeWwwUrl { get; set; }

        /// <summary>
        /// 小程序
        /// </summary>
        public string BigFHomeWxAppUrl { get; set; }

        public int BigFHomeOrder { get; set; }
    }
    /// <summary>
    /// --菜单内容
    /// </summary>
    public class ActivePageMenuModel
    {
        public int Pkid { get; set; }

        public int FkActiveContentId { get; set; }
        /// <summary>
        /// --菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// --菜单开始值
        /// </summary>
        public string MenuValue { get; set; }

        /// <summary>
        /// --菜单结束值
        /// </summary>
        public string MenuValueEnd { get; set; }

        public int Sort { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 导航菜单描述
        /// </summary>
        public string Description { get; set; }
    }

    public class ActivePageTireSizeConfigModel
    {
        public int Pkid { get; set; }

        public int FkActiveId { get; set; }
        /// <summary>
        /// -- 1:可换车 
        /// </summary>
        public int IsChangeTire { get; set; }

        /// <summary>
        /// --可换轮胎规格
        /// </summary>
        public int IsChangeTireSize { get; set; }

        /// <summary>
        /// --显示适配标签
        /// </summary>
        public int IsShowTag { get; set; }

        /// <summary>
        /// --有分割线
        /// </summary>
        public int IsMargin { get; set; }

        /// <summary>
        /// --分割线颜色
        /// </summary>
        public string MarginColor { get; set; }

        /// <summary>
        /// --填充颜色
        /// </summary>
        public string FillColor { get; set; }

        /// <summary>
        /// --提示语
        /// </summary>
        public string PromptColor { get; set; }

        /// <summary>
        /// --提示语字号
        /// </summary>
        public string PromptFontSize { get; set; }

        /// <summary>
        /// -车主信息颜色
        /// </summary>
        public string CarInfoColor { get; set; }

        /// <summary>
        /// --车主信息字号
        /// </summary>
        public string CarInfoFontSize { get; set; }

        /// <summary>
        /// --无车型提示语
        /// </summary>
        public string NoCarTypePrompt { get; set; }

        /// <summary>
        /// --无规格提示语
        /// </summary>
        public string NoFormatPrompt { get; set; }

        /// <summary>
        /// 是否显示车型条
        /// </summary>
        public int IsShowVehicleBar { get; set; }
    }

    public class ActivePageGroupContentModel
    {
        public string Order { get; set; }
        public int OrigionType { get; set; }
        public int RowType { get; set; }
        public int Type { get; set; }
        public IEnumerable<ActivePageContentModel> Contents { get; set; }
    }
    public enum AppRowType
    {
        /// <summary>
        /// 一行一列模板
        /// </summary>
        Template_1R1C = 1,
        /// <summary>
        /// 一行一列图片
        /// </summary>
        Image_1R1C = 2,
        /// <summary>
        /// 一行一列抢购模板
        /// </summary>
        FlashTemplate_1R1C = 3,
        /// <summary>
        /// 一行两列或者多列模板
        /// </summary>
        Template_1R2C_1R3C = 4,
        /// <summary>
        /// 一行两列或者三列抢购模板
        /// </summary>
        FlashTemplate_1R2C_1R3C = 6,
        /// <summary>
        /// 一行两列或者多列图片
        /// </summary>
        Image_1R2C_1R3C = 5,
        /// <summary>
        /// 滚动菜单
        /// </summary>
        Menu = 7,
        /// <summary>
        /// 左右滑动
        /// </summary>
        SlideMenu = 8,
        /// <summary>
        /// 秒杀
        /// </summary>
        SecondKill = 9,
        /// <summary>
        /// 大翻盘
        /// </summary>
        LuckyWheel = 10,

        /// <summary>
        /// 保养定价
        /// </summary>
        BaoyangPrice = 16,
        /// <summary>
        /// 新大翻盘
        /// </summary>
        NewLuckyWheel = 20,

        /// <summary>
        /// 礼包跟车型是一种
        /// </summary>
        Coupon = 18,

        /// <summary>
        ///视频
        /// </summary>
        Video = 17,

        /// <summary>
        /// 商品池
        /// </summary>
        ProductPool = 19,

        /// <summary>
        /// 问答抽奖
        /// </summary>
        QAlottery = 21,

        /// <summary>
        /// 红包抽奖
        /// </summary>
        Luckylottery = 22,
        /// <summary>
        /// 拼团
        /// </summary>
        ProductGroup = 23,

        /// <summary>
        /// 文案
        /// </summary>
        ActiveText = 24,

        /// <summary>
        /// 滚动文字链
        /// </summary>
        ScrollTextChain=25,

        /// <summary>
        /// 滑动优惠券
        /// </summary>
        SlipCoupon=26,
        /// <summary>
        /// 倒计时
        /// </summary>
        CountDown=27,

        /// <summary>
        /// 摇奖机
        /// </summary>
        LotteryMachine = 28,
        /// <summary>
        /// 车型头图
        /// </summary>
        VehicleBanner = 29,
        /// <summary>
        /// 新商品池
        /// </summary>
        NewProductPool = 30


    }

    public enum WebSiteRowType
    {
        /// <summary>
        /// 1一行一列
        /// </summary>
        OneRowOneColumn = 1,
        /// <summary>
        /// 2一行两列
        /// </summary>
        OneRowTwoColumn = 2,
        /// <summary>
        /// 3 一行三列
        /// </summary>
        OneRowThreeColumn = 3,
        /// <summary>
        /// 4一行四列
        /// </summary>
        OneRowFourColumn = 4,
        /// <summary>
        /// 5一行5列
        /// </summary>
        OneRowFiveColumn = 5,
        /// <summary>
        /// 7导航菜单滚动
        /// </summary>
        Menu = 7,
        /// <summary>
        /// 8左右切换菜单
        /// </summary>
        SildeMenu = 8,
        /// <summary>
        /// 9秒杀
        /// </summary>
        SecondKill = 9,
        /// <summary>
        ///13 一图三产品
        /// </summary>
        OneImageThreeProduct = 13,
        /// <summary>
        /// 14 banner
        /// </summary>
        Banner = 14,
    }
    public class ActivePageContentModel
    {
        public int Pkid { get; set; }

        public int FkActiveId { get; set; }

        public string Group { get; set; }
        /// <summary>
        /// -类型 0:轮胎 1：车品 2：链接 3：图片 4：优惠券 5：保养 6：限购轮胎 7：限购车品 8：活动规则 9：秒杀 10:轮毂 11：限购轮毂 12：其它 13：大翻盘 -2导航栏
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// --渠道 all wap www
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// --是否图片 1：图片 0：模板
        /// </summary>
        public int IsUploading { get; set; }

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
        /// 是否有赠品0：没有；1：有
        /// </summary>
        public int HasGift { get; set; }

        /// <summary>
        /// 产品安装服务0：无需安装；1：包安装；2：支持安装
        /// </summary>
        public int InstallService { get; set; }

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

        public bool IsNewUserFirstOrder { get; set; }

        /// <summary>
        /// 广告语
        /// </summary>
        public string AdvertiseTitle { get; set; }
        #region 6/19新增保养专用

        /// <summary>
        /// 途虎推荐
        /// </summary>
        public int? IsRecommended { get; set; }

        /// <summary>
        /// 强制登陆
        /// </summary>
        public int? IsLogin { get; set; }

        /// <summary>
        /// 轮胎规格
        /// </summary>
        public int? IsTireStandard { get; set; }

        /// <summary>
        /// 轮胎尺寸
        /// </summary>
        public int? IsTireSize { get; set; }

        /// <summary>
        /// 隐藏标题栏
        /// </summary>
        public int? IsHiddenTtile { get; set; }

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

        /// <summary>
        /// 是否替换商品
        /// </summary>
        public int IsReplace { get; set; }



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
        /// 是否区分车型，false不限，true分车型
        /// </summary>
        public bool IsVehicle { get; set; }

        /// <summary>
        /// 文字链内容
        /// </summary>
        public List<ScrollTextChainModel> ScrollTextChains { get; set; }

        /// <summary>
        /// 滑动优惠券内容
        /// </summary>
        public List<SlipCouponModel> SlipCoupons { get; set; }

        #region 商品池类型新加
        /// <summary>
        /// 一行X列
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public int ProductType { get; set; }

        /// <summary>
        /// 适配过滤
        /// </summary>
        public int IsAdapter { get; set; }

        /// <summary>
        /// 适配
        /// </summary>
        public string Tips { get; set; }

        /// <summary>
        /// 适配标签
        /// </summary>
        public int AdapterTag { get; set; }
        /// <summary>
        /// 分期标签
        /// </summary>
        public int InstalIlmentTag { get; set; }
        /// <summary>
        /// 商品池
        /// </summary>
        public List<ActivePageContentModel> Contents { get; set; }

        /// <summary>
        /// 活动类型0：不限，1：限时抢购，2：楼层活动
        /// </summary>
        public int ActivityType { get; set; }
        #region 新商品池
        /// <summary>
        /// 限制返回行数，0：不限
        /// </summary>
        public int RowLimit { get; set; }

        /// <summary>
        /// 车型级别，0不分车型，2：二级，5：5级
        /// </summary>
        public int VehicleLevel { get; set; }

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
        /// 是否需要登录配置
        /// </summary>
        public int IsNeedLogIn { get; set; }

        /// <summary>
        /// 是否使用优惠券
        /// </summary>
        public bool IsUsePcode { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public string ExtendedProperty { get; set; }
    }
    #region 大转盘活动模型
    public class LuckyWheelModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public int isNewUser { get; set; }

        public int isStatus { get; set; }

        public DateTime CreateDate { get; set; }


        public DateTime UpdateDate { get; set; }

        public string DataParames { get; set; }


        public List<LuckyWheelDeatil> Items { get; set; }

        public int IsAddOne { get; set; }

        /// <summary>
        /// 是否积分抽奖 1：是 0：否
        /// </summary>
        public int IsIntegral { get; set; }

        /// <summary>
        /// 单次积分数
        /// </summary>
        public int Integral { get; set; }

        public List<LuckyItem> LuckyItems { get; set; }
        public string CreatorUser { get; set; }

        public string UpdateUser { get; set; }

        /// <summary>
        /// 分享前的次数
        /// </summary>
        public int PreShareTimes { get; set; }

        /// <summary>
        /// 分享后增加的次数
        /// </summary>
        public int CompletedShareTimes { get; set; }
    }

    public class LuckyWheelDeatil
    {
        public int ID { get; set; }

        public string FKLuckyWheelID { get; set; }

        public int UserRank { get; set; }


        public int Type { get; set; }

        /// <summary>
        /// 优惠券 多个以，分隔
        /// </summary>
        public string CouponRuleID { get; set; }

        /// <summary>
        /// 优惠券限制多个以逗号分隔
        /// </summary>
        public string MaxCoupon { get; set; }

        public string BGImage { get; set; }

        public string ContentImage { get; set; }


        public string ChangeImage { get; set; }


        public string GetDescription { get; set; }


        public string GoDescription { get; set; }

        public string APPUrl { get; set; }

        public string WapUrl { get; set; }

        public string WwwUrl { get; set; }

        public string HandlerAndroid { get; set; }

        public string SOAPAndroid { get; set; }

        public string HandlerIOS { get; set; }

        public string SOAPIOS { get; set; }


        public int OrderBy { get; set; }


    }

    public class LuckyItem
    {
        public int UserRank { get; set; }


        public int Coupon { get; set; }

        public int? Count { get; set; }
    }
    #endregion

    /// <summary>
    /// 判断是否有赠品模型
    /// </summary>
    public class CheckGiftModel
    {
        public string Pid { get; set; }

        /// <summary>
        /// 0没有，1有
        /// </summary>
        public int HasGift { get; set; }

    }

    public class ScrollTextChainModel
    {
        /// <summary>
        /// 文字内容
        /// </summary>
        public string Test { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        public string H5Url { get; set; }

        public string WxUrl { get; set; }
    }

    public class SlipCouponModel
    {
        /// <summary>
        /// 优惠券Id
        /// </summary>
        public string CouponId { get; set; }
        /// <summary>
        /// 优惠券图片
        /// </summary>
        public string CouponImage { get; set; }
    }
    /// <summary>
    /// 分车型头图
    /// </summary>
    public class VehicleBanner
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 车型Id
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string ImageUrl { get; set; }

    }
    #region 保养定价
    public class FixedPriceActivityConfig
    {
        public int PKID { get; set; }

        public Guid ActivityId { get; set; }

        public string ActivityName { get; set; }

        public string PackageTypes { get; set; }

        public int MaxSaleQuantity { get; set; }

        public bool IsChargeInstallFee { get; set; }

        public bool IsUsePromotion { get; set; }

        public string InstallOrPayType { get; set; }

        public int ItemQuantityPerUser { get; set; }

        public decimal PromotionPrice { get; set; }

        /// <summary>
        /// 提示文字颜色
        /// </summary>
        public string TipTextColor { get; set; }

        /// <summary>
        /// 主按钮背景颜色
        /// </summary>
        public string ButtonBackgroundColor { get; set; }

        /// <summary>
        /// 主按钮字体颜色
        /// </summary>
        public string ButtonTextColor { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public string BackgroundImg { get; set; }


        public List<FixedPriceActivityProductConfig> ProductConfigs { get; set; }

        /// <summary>
        /// 场次信息
        /// </summary>
        public List<FixedPriceActivityRoundConfig> RoundConfigs { get; set; }

        public List<FixedPriceActivityShopConfig> ShopConfigs { get; set; }

        #region 扩展
        public string Label { get; set; }

        public string Type { get; set; }
        #endregion
    }

    public class FixedPriceActivityProductConfig
    {
        public string CategoryName { get; set; }

        public string PID { get; set; }

        public string Brand { get; set; }

        public bool IsIngore { get; set; }
    }

    public class FixedPriceActivityRoundConfig
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int LimitedQuantity { get; set; }
    }

    public class FixedPriceActivityShopConfig
    {
        public string ShopType { get; set; }

        public int ShopId { get; set; }
    }
    #endregion

    #region 批量验证活动有效性接口返回结果

    public class ActivtyValidityResponse
    {
        public string HashKey { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValidity { get; set; }

    }
    #endregion

}


