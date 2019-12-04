using System;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 优惠券使用规则 实体类
    /// </summary>
    public class CouponUseRuleEntity
    {

        /// <summary>
        /// 优惠券规则名称
        /// </summary>		
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 【不用】产品分类
        /// </summary>		
        private string _category;
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
        /// <summary>
        /// 【不用】产品id
        /// </summary>		
        private string _productid;
        public string ProductID
        {
            get { return _productid; }
            set { _productid = value; }
        }
        /// <summary>
        /// 【不用】品牌
        /// </summary>		
        private string _brand;
        public string Brand
        {
            get { return _brand; }
            set { _brand = value; }
        }
        /// <summary>
        /// 【没用】之前主券子券都在以前，现在拆出来了，有GetRuleid
        /// </summary>		
        private long _parentid;
        public long ParentID
        {
            get { return _parentid; }
            set { _parentid = value; }
        }
        /// <summary>
        /// 【没用】不知道干啥的
        /// </summary>		
        private bool _isactive;
        public bool IsActive
        {
            get { return _isactive; }
            set { _isactive = value; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>		
        private DateTime _createdatetime;
        public DateTime CreateDateTime
        {
            get { return _createdatetime; }
            set { _createdatetime = value; }
        }
        /// <summary>
        /// 最后更改时间
        /// </summary>		
        private DateTime _lastdatetime;
        public DateTime LastDateTime
        {
            get { return _lastdatetime; }
            set { _lastdatetime = value; }
        }
        /// <summary>
        /// 子超说很老的优惠券用来特殊判断的，最好不要动
        /// </summary>		
        private int _type;
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// 0所有订单，1仅到家订单，2仅到店订单
        /// </summary>		
        private int _installtype;
        public int InstallType
        {
            get { return _installtype; }
            set { _installtype = value; }
        }
        /// <summary>
        /// 商品范围按PID的时候 0增加PID，1排除PID
        /// </summary>		
        private bool _pidtype;
        public bool PIDType
        {
            get { return _pidtype; }
            set { _pidtype = value; }
        }
        /// <summary>
        /// IOS跳转地址
        /// </summary>		
        private string _ioskey;
        public string IOSKey
        {
            get { return _ioskey; }
            set { _ioskey = value; }
        }
        /// <summary>
        /// IOS跳转地址
        /// </summary>		
        private string _iosvalue;
        public string IOSValue
        {
            get { return _iosvalue; }
            set { _iosvalue = value; }
        }
        /// <summary>
        /// 安卓跳转地址
        /// </summary>		
        private string _androidkey;
        public string androidKey
        {
            get { return _androidkey; }
            set { _androidkey = value; }
        }
        /// <summary>
        /// 安卓跳转地址
        /// </summary>		
        private string _androidvalue;
        public string androidValue
        {
            get { return _androidvalue; }
            set { _androidvalue = value; }
        }
        /// <summary>
        /// TempCate
        /// </summary>		
        //private string _tempcate;
        //public string TempCate
        //{
        //    get { return _tempcate; }
        //    set { _tempcate = value; }
        //}
        /// <summary>
        /// 优惠券跳转地址（老版本）0.优惠券说明，1.轮胎列表,2.保养列表，3.车品商城，4.商品列表，5.洗车，6.打蜡，7.内饰清洗
        /// </summary>		
        private int _hreftype;
        public int HrefType
        {
            get { return _hreftype; }
            set { _hreftype = value; }
        }
        /// <summary>
        /// 【没用】
        /// </summary>		
        //private int _tbl_couponrules;
        //public int tbl_CouponRules
        //{
        //    get { return _tbl_couponrules; }
        //    set { _tbl_couponrules = value; }
        //}
        /// <summary>
        /// 优惠券类型，0满减券，1后返券，2实付券，3抵扣券
        /// </summary>		
        private int _promotiontype;
        public int PromotionType
        {
            get { return _promotiontype; }
            set { _promotiontype = value; }
        }
        /// <summary>
        /// 【不用】门店id
        /// </summary>		
        private int _shopid;
        public int ShopID
        {
            get { return _shopid; }
            set { _shopid = value; }
        }
        /// <summary>
        /// 【不用】门店类型
        /// </summary>		
        private int _shoptype;
        public int ShopType
        {
            get { return _shoptype; }
            set { _shoptype = value; }
        }
        /// <summary>
        /// 主键自增列
        /// </summary>		
        private int _pkid;
        public int PKID
        {
            get { return _pkid; }
            set { _pkid = value; }
        }
        /// <summary>
        /// APP跳转页
        /// </summary>		
        private string _customskippage;
        public string CustomSkipPage
        {
            get { return _customskippage; }
            set { _customskippage = value; }
        }
        /// <summary>
        /// 规则类型 null,0: 普通券， 1:门店券；
        /// </summary>		
        private int _coupontype;
        public int CouponType
        {
            get { return _coupontype; }
            set { _coupontype = value; }
        }
        /// <summary>
        /// 微信小程序跳转页
        /// </summary>		
        private string _wxskippage;
        public string WxSkipPage
        {
            get { return _wxskippage; }
            set { _wxskippage = value; }
        }
        /// <summary>
        /// 商品范围，0已选中 1未选中
        /// </summary>		
        private int _configtype;
        public int ConfigType
        {
            get { return _configtype; }
            set { _configtype = value; }
        }
        /// <summary>
        /// H5跳转页
        /// </summary>		
        private string _h5skippage;
        public string H5SkipPage
        {
            get { return _h5skippage; }
            set { _h5skippage = value; }
        }
        /// <summary>
        /// 支付方式，0不限，1在线支付，2到店支付
        /// </summary>		
        private int _orderpaymethod;
        public int OrderPayMethod
        {
            get { return _orderpaymethod; }
            set { _orderpaymethod = value; }
        }
        /// <summary>
        /// EnabledGroupBuy
        /// </summary>		
        private bool _enabledgroupbuy;
        public bool EnabledGroupBuy
        {
            get { return _enabledgroupbuy; }
            set { _enabledgroupbuy = value; }
        }
        /// <summary>
        /// 使用规则说明
        /// </summary>		
        private string _ruledescription;
        public string RuleDescription
        {
            get { return _ruledescription; }
            set { _ruledescription = value; }
        }

    }
}
