using System;
using System.Collections.Generic;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models.Activity
{
    public class ActivityBaseModel
    {
        public string HashKey { get; set; }
    }
    public class ActivityPageInfoModel
    {
        #region 活动页全局配置
        /// <summary>
        /// 主键
        /// </summary>
        public int Pkid { get; set; }

        /// <summary>
        /// 活动标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// --背景图片
        /// </summary>
        public string BgImageUrl { get; set; }

        /// <summary>
        /// --背景颜色
        /// </summary>
        public string BgColor { get; set; }

        /// <summary>
        /// --活动类型 
        /// </summary>
        public int ActivityType { get; set; }

        /// <summary>
        /// --分享参数
        /// </summary>
        public string DataParames { get; set; }
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
        /// 客服
        /// </summary>
        public int? CustomerService { get; set; }
        /// <summary>
        /// 是否需要登录配置
        /// </summary>
        public int IsNeedLogIn { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        /// <summary>
        /// 菜单类型0:上下菜单1:左右菜单
        /// </summary>
        public int? MenuType { get; set; }
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
        #endregion

        public ActivityPageInfoTireSizeConfigModel ActivityPageInfoTireSizeConfigModel { get; set; }
        public List<ActivityPageInfoRow> ActivityPageInfoRows { get; set; }
    }
    /// <summary>
    /// 车型适配
    /// </summary>
    public class ActivityPageInfoTireSizeConfigModel
    {
        /// <summary>
        /// 是否显示车型条
        /// </summary>
        public int IsShowVehicleBar { get; set; }
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
    }

    /// <summary>
    /// 行
    /// </summary>
    public class ActivityPageInfoRow
    {
        public string RowNum { get; set; }
        /// <summary>
        /// 行类型 0是一行一列以此类推，1:一行两列；2:一行三列，3:网站一行一列，4:一行四列，6:一行五列，7: 一图三产品,8:APP一图三产品
        /// </summary>
        public int RowType { get; set; }

        public List<ActivityPageInfoCell> ActivityPageInfoCells { get; set; }
    }

    /// <summary>
    /// 单元格
    /// </summary>
    public class ActivityPageInfoCell
    {
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 单元格号
        /// </summary>
        public int CellNum { get; set; }
    }
    #region BASE
    public class ActivityPageInfoRowBase
    {
        [Column("Group")]
        public string RowNum { get; set; }

    }

    public class ActivityPageInfoCellBase
    {
        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }

        /// <summary>
        /// --图片地址
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
    }
    #endregion

    #region Image
    public class ActivityPageInfoRowImage : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellImage> ActivityPageInfoCellImages { get; set; }
    }

    public class ActivityPageInfoCellImage : ActivityPageInfoCellBase
    {
        /// <summary>
        /// 动图链接
        /// </summary>
        public string FileUrl { get; set; }
    }
    #endregion

    #region 秒杀
    public class ActivityPageInfoRowSeckill : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellSeckill> ActivityPageInfoCellSeckills { get; set; }

        /// <summary>
        /// 行类型 0是一行一列以此类推
        /// </summary>
        public int RowType { get; set; }
    }

    public class ActivityPageInfoCellSeckill : ActivityPageInfoCellBase
    {
    }
    #endregion
    #region 链接
    public class ActivityPageInfoRowLink : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellLink> ActivityPageInfoCellLinks { get; set; }
    }
    public class ActivityPageInfoCellLink : ActivityPageInfoCellBase
    {
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

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WxAppUrl { get; set; }
    }
    #endregion

    #region 产品
    public class ActivityPageInfoRowProduct : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellProduct> ActivityPageInfoCellProducts { get; set; }
    }
    public class ActivityPageInfoCellProduct : ActivityPageInfoCellBase
    {
        public string Pid { get; set; }

        public Guid? ActivityId { get; set; }
        /// <summary>
        /// 是否显示进度条
        /// </summary>
        public int DisplayWay { get; set; }
        /// <summary>
        /// 是否替换商品
        /// </summary>
        public int IsReplace { get; set; }

    }
    #endregion

    #region 优惠券
    public class ActivityPageInfoRowCoupon : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellCoupon> ActivityPageInfoCellCoupons { get; set; }
    }
    public class ActivityPageInfoCellCoupon : ActivityPageInfoCellBase
    {
        public Guid Cid { get; set; }
    }
    #endregion

    #region 保养
    public class ActivityPageInfoRowBy : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellBy> ActivityPageInfoCellBys { get; set; }
    }
    public class ActivityPageInfoCellBy : ActivityPageInfoCellBase
    {
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
        ///  --是否开启轮胎适配 1:全部 2:轮胎 3:轮毂
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

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WxAppUrl { get; set; }
    }
    #endregion

    #region 活动规则
    public class ActivityPageInfoRowRule : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellRule> ActivityPageInfoCellRules { get; set; }
    }
    public class ActivityPageInfoCellRule : ActivityPageInfoCellBase
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
    #endregion

    #region 其他
    public class ActivityPageInfoRowOther : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellOther> ActivityPageInfoCellOthers { get; set; }
    }
    public class ActivityPageInfoCellOther : ActivityPageInfoCellBase
    {
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

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WxAppUrl { get; set; }

        /// <summary>
        /// 选项
        /// </summary>
        public int? OthersType { get; set; }
    }
    #endregion

    #region 其余渠道活动(Luckwheel,BaoyangPrice,NewLuckyWheel,Packs,QAlottery)
    public class ActivityPageInfoRowOtherActivity : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellOtherActivity> ActivityPageInfoCellOtherActivitys { get; set; }
    }
    public class ActivityPageInfoCellOtherActivity : ActivityPageInfoCellBase
    {
        /// <summary>
        /// 活动id
        /// </summary>
        public string ActivityId { get; set; }
    }

    #endregion

    #region 视频
    public class ActivityPageInfoRowVideo : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellVideo> ActivityPageInfoCellVideos { get; set; }
    }
    public class ActivityPageInfoCellVideo : ActivityPageInfoCellBase
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

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

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WxAppUrl { get; set; }
    }
    #endregion

    #region 拼团

    public class ActivityPageInfoRowPintuan : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellPintuan> ActivityPageInfoCellPintuans { get; set; }
    }
    public class ActivityPageInfoCellPintuan : ActivityPageInfoCellBase
    {
        public string ProductGroupId { get; set; }

        public string Pid { get; set; }
    }
    #endregion

    #region 文字链，滑动优惠券

    public class ActivityPageInfoRowJson : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellJson> ActivityPageInfoCellJsons { get; set; }
    }
    public class ActivityPageInfoCellJson : ActivityPageInfoCellBase
    {
        /// <summary>
        /// 文字链，优惠券等配置内容json格式存储
        /// </summary>
        public string JsonContent { get; set; }
    }
    #endregion

    #region 文案
    public class ActivityPageInfoRowActivityText : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellActivityText> ActivityPageInfoCellActivityTexts { get; set; }
    }
    public class ActivityPageInfoCellActivityText : ActivityPageInfoCellBase
    {
        /// <summary>
        /// 文案
        /// </summary>
        public string ActiveText { get; set; }
    }
    #endregion

    #region 倒计时
    public class ActivityPageInfoRowCountDown : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellCountDown> ActivityPageInfoCellCountDowns { get; set; }
    }
    public class ActivityPageInfoCellCountDown : ActivityPageInfoCellBase
    {
        /// <summary>
        /// 倒计时样式：1,深色，2，浅色
        /// </summary>
        public int CountDownStyle { get; set; }
    }
    #endregion

    #region 分车型头图
    public class ActivityPageInfoRowVehicleBanner : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellVehicleBanner> ActivityPageInfoCellVehicleBanners { get; set; }
    }
    public class ActivityPageInfoCellVehicleBanner : ActivityPageInfoCellBase
    {
        /// <summary>
        /// --跳转链接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WxAppUrl { get; set; }
    }
    #endregion

    #region 新商品池
    public class ActivityPageInfoRowNewProductPool : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellNewProductPool> ActivityPageInfoCellNewProductPools { get; set; }

        /// <summary>
        /// 一行X列
        /// </summary>
        public int ColumnNumber { get; set; }
    }
    public class ActivityPageInfoCellNewProductPool : ActivityPageInfoCellBase
    {
        /// <summary>
        /// 是否展示适配标签（适配过滤）
        /// </summary>
        public int IsAdapter { get; set; }


        public string Pid { get; set; }

        public string ProductName { get; set; }

        public Guid? ActivityId { get; set; }
        /// <summary>
        /// 产品价格(原价) 市场价格（伪原价）
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 活动价格 没有活动价格时显示的产品价格
        /// </summary>
        public decimal ActivityPrice { get; set; }

        public bool Onsale { get; set; }

        public string Brand { get; set; }

        public string Pattern { get; set; }

        public string SpeedRating { get; set; }

        public string Category { get; set; }

        public bool IsNewUserFirstOrder { get; set; }

        public string AdvertiseTitle { get; set; }

        public bool IsUsePcode { get; set; }

        public int? TotalQuantity { get; set; }

        public int SaleOutQuantity { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public bool IsShow { get; set; }

        public int DisplayWay { get; set; }

        public int ProductType { get; set; }

        public string TireSize { get; set; }
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
    }
    #endregion

    #region 商品池
    public class ActivityPageInfoRowProductPool : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoCellProductPool> ActivityPageInfoCellProductPools { get; set; }

        /// <summary>
        /// 一行X列
        /// </summary>
        public int ColumnNumber { get; set; }
    }
    public class ActivityPageInfoCellProductPool : ActivityPageInfoCellBase
    {

        /// <summary>
        /// 是否展示适配标签（适配过滤）
        /// </summary>
        public int IsAdapter { get; set; }


        public string Pid { get; set; }

        public string ProductName { get; set; }

        public Guid? ActivityId { get; set; }
        /// <summary>
        /// 产品价格(原价) 市场价格（伪原价）
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 活动价格 没有活动价格时显示的产品价格
        /// </summary>
        public decimal ActivityPrice { get; set; }

        public bool Onsale { get; set; }

        public string Brand { get; set; }

        public string Pattern { get; set; }

        public string SpeedRating { get; set; }

        public string Category { get; set; }

        public bool IsNewUserFirstOrder { get; set; }

        public string AdvertiseTitle { get; set; }

        public bool IsUsePcode { get; set; }

        public int? TotalQuantity { get; set; }

        public int SaleOutQuantity { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public bool IsShow { get; set; }

        public int DisplayWay { get; set; }

        public int ProductType { get; set; }

        public string TireSize { get; set; }

    }
    #endregion

    #region 推荐模块
    public class ActivityPageInfoRecommend
    {
        public string Pid { get; set; }

    }
    #endregion

    #region 菜单

    public class ActivityPageInfoRowMenuModel : ActivityPageInfoRowBase
    {
        public List<ActivityPageInfoMenu> ActivityPageInfoMenus { get; set; }
    }

    public class ActivityPageInfoMenu
    {
        /// <summary>
        /// --菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }

        public int Sort { get; set; }
        /// <summary>
        /// --菜单开始值
        /// </summary>
        public string MenuValue { get; set; }

        /// <summary>
        /// --菜单结束值
        /// </summary>
        public string MenuValueEnd { get; set; }

        /// <summary>
        /// 导航菜单描述
        /// </summary>
        public string Description { get; set; }
    }
    #endregion

    #region 会场信息
    public class ActivityPageInfoHomeModel
    {
        public int Pkid { get; set; }

        public int FkActiveId { get; set; }

        /// <summary>
        /// --图标
        /// </summary>
        public string HidBigHomePic { get; set; }

        /// <summary>
        /// --h5链接地址
        /// </summary>
        public string BigHomeUrl { get; set; }

        /// <summary>
        /// --网站链接
        /// </summary>
        public string BigHomeUrlWww { get; set; }

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
        /// 会场详情
        /// </summary>
        public List<ActivityPageInfoHomeDeatilModel> ActivityPageInfoHomeDeatilModels { get; set; }
    }
    /// <summary>
    /// --会场详情信息
    /// </summary>
    public class ActivityPageInfoHomeDeatilModel
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
    #endregion
}
