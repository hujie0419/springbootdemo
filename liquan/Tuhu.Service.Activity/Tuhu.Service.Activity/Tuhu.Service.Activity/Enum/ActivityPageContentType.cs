
namespace Tuhu.Service.Activity.Enum
{
    /// <summary>
    /// -类型 0:轮胎 1：车品 2：链接 3：图片 4：优惠券 5：保养 6：限购轮胎 7：限购车品 8：活动规则 9：秒杀 10:轮毂 11：限购轮毂 12：其它 13：大翻盘 -2导航栏
    /// </summary>
    public enum ActivityPageContentType :int
    {
        Tire = 0,
        CarProduct = 1,
        Link = 2,
        Image = 3,
        Coupon = 4,
        Baoyang = 5,
        ATire = 6,
        ACarProduct = 7,
        Rule = 8,
        SecondKill = 9,
        LunGu = 10,
        ALunGu = 11,
        Other = 12,
        Luckwheel = 13,
        Navigation = -2,
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
        Packs = 18,
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
        /// 滚动文字链
        /// </summary>
        ScrollTextChain = 25,
        /// <summary>
        /// 滑动优惠券
        /// </summary>
        SlipCoupon = 26,
        /// <summary>
        /// 倒计时
        /// </summary>
        CountDown = 27,
        /// <summary>
        /// 车型头图
        /// </summary>
        VehicleBanner=29,
        /// <summary>
        /// 新商品池
        /// </summary>
        NewProductPool=30,

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


    }

    public enum ActivityIdType
    {
        /// <summary>
        /// 不限
        /// </summary>
        NolLimit=0,
        /// <summary>
        /// 限时抢购活动Id
        /// </summary>
        FlashSaleActivity=1,

        /// <summary>
        /// 楼层活动id
        /// </summary>
        FloorActivity=2,

        /// <summary>
        /// 自动生成的活动id
        /// </summary>
         AutoActivity=3   
    }
    //public enum AppRowType
    //{
    //    /// <summary>
    //    /// 一行一列模板
    //    /// </summary>
    //    Template_1R1C = 1,
    //    /// <summary>
    //    /// 一行一列图片
    //    /// </summary>
    //    Image_1R1C = 2,
    //    /// <summary>
    //    /// 一行一列抢购模板
    //    /// </summary>
    //    FlashTemplate_1R1C = 3,
    //    /// <summary>
    //    /// 一行两列或者三列模板
    //    /// </summary>
    //    Template_1R2C_1R3C = 4,
    //    /// <summary>
    //    /// 一行两列或者三列抢购模板
    //    /// </summary>
    //    FlashTemplate_1R2C_1R3C = 6,
    //    /// <summary>
    //    /// 一行两列或者三列图片
    //    /// </summary>
    //    Image_1R2C_1R3C = 5,
    //    /// <summary>
    //    /// 滚动菜单
    //    /// </summary>
    //    Menu = 7,
    //    /// <summary>
    //    /// 左右滑动
    //    /// </summary>
    //    SlideMenu = 8,
    //    /// <summary>
    //    /// 秒杀
    //    /// </summary>
    //    SecondKill = 9,
    //    /// <summary>
    //    /// 大翻盘
    //    /// </summary>
    //    LuckyWheel = 10,

    //    /// <summary>
    //    /// 保养定价
    //    /// </summary>
    //    BaoyangPrice = 16,
    //    /// <summary>
    //    /// 新大翻盘
    //    /// </summary>
    //    NewLuckyWheel = 20,

    //    /// <summary>
    //    /// 礼包跟车型是一种
    //    /// </summary>
    //    Coupon = 18,

    //    /// <summary>
    //    ///视频
    //    /// </summary>
    //    Video = 17,

    //    /// <summary>
    //    /// 商品池
    //    /// </summary>
    //    ProductPool = 19

    //}

    //public enum WebSiteRowType
    //{
    //    /// <summary>
    //    /// 1一行一列
    //    /// </summary>
    //    OneRowOneColumn = 1,
    //    /// <summary>
    //    /// 2一行两列
    //    /// </summary>
    //    OneRowTwoColumn = 2,
    //    /// <summary>
    //    /// 3 一行三列
    //    /// </summary>
    //    OneRowThreeColumn = 3,
    //    /// <summary>
    //    /// 4一行四列
    //    /// </summary>
    //    OneRowFourColumn = 4,
    //    /// <summary>
    //    /// 5一行5列
    //    /// </summary>
    //    OneRowFiveColumn = 5,
    //    /// <summary>
    //    /// 7导航菜单滚动
    //    /// </summary>
    //    Menu = 7,
    //    /// <summary>
    //    /// 8左右切换菜单
    //    /// </summary>
    //    SildeMenu = 8,
    //    /// <summary>
    //    /// 9秒杀
    //    /// </summary>
    //    SecondKill = 9,
    //    /// <summary>
    //    ///13 一图三产品
    //    /// </summary>
    //    OneImageThreeProduct = 13,
    //    /// <summary>
    //    /// 14 banner
    //    /// </summary>
    //    Banner = 14,
    //}
}
