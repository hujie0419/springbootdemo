using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 优惠券领取规则 实体类
    /// </summary>
    public class CouponGetRuleEntity

    {

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
        /// 规则id
        /// </summary>		
        private int _ruleid;
        public int RuleID
        {
            get { return _ruleid; }
            set { _ruleid = value; }
        }
        /// <summary>
        /// 描述
        /// </summary>		
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        /// <summary>
        /// 优惠券名称
        /// </summary>		
        private string _promtionname;
        public string PromtionName
        {
            get { return _promtionname; }
            set { _promtionname = value; }
        }
        /// <summary>
        /// 面额
        /// </summary>		
        private decimal? _discount;
        public decimal? Discount
        {
            get { return _discount; }
            set { _discount = value; }
        }
        /// <summary>
        /// 使用条件，满多少才可以用
        /// </summary>		
        private decimal? _minmoney;
        public decimal? Minmoney
        {
            get { return _minmoney; }
            set { _minmoney = value; }
        }
        /// <summary>
        /// 是否允许电销发放
        /// </summary>		
        private int _allowchanel;
        public int AllowChanel
        {
            get { return _allowchanel; }
            set { _allowchanel = value; }
        }
        /// <summary>
        /// 自领取后多少天有效
        /// </summary>		
        private int? _term;
        public int? Term
        {
            get { return _term; }
            set { _term = value; }
        }
        /// <summary>
        /// 固定有效期 开始时间
        /// </summary>		
        private DateTime? _valistartdate;
        public DateTime? ValiStartDate
        {
            get { return _valistartdate; }
            set { _valistartdate = value; }
        }
        /// <summary>
        /// 固定有效期结束时间
        /// </summary>		
        private DateTime? _valienddate;
        public DateTime? ValiEndDate
        {
            get { return _valienddate; }
            set { _valienddate = value; }
        }
        /// <summary>
        /// 发行量
        /// </summary>		
        private int? _quantity;
        public int? Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        /// <summary>
        /// 已经发行多少
        /// </summary>		
        private int _getquantity;
        public int GetQuantity
        {
            get { return _getquantity; }
            set { _getquantity = value; }
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
        /// 最后修改时间
        /// </summary>		
        private DateTime _lastdatetime;
        public DateTime LastDateTime
        {
            get { return _lastdatetime; }
            set { _lastdatetime = value; }
        }
        /// <summary>
        /// 单个人限制领取数量
        /// </summary>		
        private int _singlequantity;
        public int SingleQuantity
        {
            get { return _singlequantity; }
            set { _singlequantity = value; }
        }
        /// <summary>
        /// 电销发放开始时间
        /// </summary>		
        private DateTime? _dxstartdate;
        public DateTime? DXStartDate
        {
            get { return _dxstartdate; }
            set { _dxstartdate = value; }
        }
        /// <summary>
        /// 电销发放结束时间
        /// </summary>		
        private DateTime? _dxenddate;
        public DateTime? DXEndDate
        {
            get { return _dxenddate; }
            set { _dxenddate = value; }
        }
        /// <summary>
        /// 获取规则GUID
        /// </summary>		
        private Guid _getruleguid;
        public Guid GetRuleGUID
        {
            get { return _getruleguid; }
            set { _getruleguid = value; }
        }
        /// <summary>
        /// 发放渠道
        /// </summary>		
        private string _channel;
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }
        /// <summary>
        /// 只是用户范围0全部，1新用户，2老用户
        /// </summary>		
        private int _supportuserrange;
        public int SupportUserRange
        {
            get { return _supportuserrange; }
            set { _supportuserrange = value; }
        }
        /// <summary>
        /// 详情页展示开始时间
        /// </summary>		
        private DateTime? _detailshowstartdate;
        public DateTime? DetailShowStartDate
        {
            get { return _detailshowstartdate; }
            set { _detailshowstartdate = value; }
        }
        /// <summary>
        /// 详情页展示结束时间
        /// </summary>		
        private DateTime? _detailshowenddate;
        public DateTime? DetailShowEndDate
        {
            get { return _detailshowenddate; }
            set { _detailshowenddate = value; }
        }
        /// <summary>
        /// 部门
        /// </summary>		
        private int _departmentid;
        public int DepartmentId
        {
            get { return _departmentid; }
            set { _departmentid = value; }
        }
        /// <summary>
        /// 用途
        /// </summary>		
        private int _intentionid;
        public int IntentionId
        {
            get { return _intentionid; }
            set { _intentionid = value; }
        }
        /// <summary>
        /// 创建者email
        /// </summary>		
        private string _creater;
        public string Creater
        {
            get { return _creater; }
            set { _creater = value; }
        }
        /// <summary>
        /// 部门
        /// </summary>		
        private string _departmentname;
        public string DepartmentName
        {
            get { return _departmentname; }
            set { _departmentname = value; }
        }
        /// <summary>
        /// 用途
        /// </summary>		
        private string _intentionname;
        public string IntentionName
        {
            get { return _intentionname; }
            set { _intentionname = value; }
        }
        /// <summary>
        /// 规则类型
        /// </summary>		
        private int _coupontype;
        public int CouponType
        {
            get { return _coupontype; }
            set { _coupontype = value; }
        }
        /// <summary>
        /// 最终有效期
        /// </summary>		
        private DateTime? _deadlinedate;
        public DateTime? DeadLineDate
        {
            get { return _deadlinedate; }
            set { _deadlinedate = value; }
        }
        /// <summary>
        /// 到期前是否提醒
        /// </summary>		
        private int _ispush;
        public int IsPush
        {
            get { return _ispush; }
            set { _ispush = value; }
        }
        /// <summary>
        /// 提前多少天提醒，多个用逗号隔开
        /// </summary>		
        private string _pushsetting;
        public string PushSetting
        {
            get { return _pushsetting; }
            set { _pushsetting = value; }
        }
        /// <summary>
        /// 业务线id
        /// </summary>		
        private int _businesslineid;
        public int BusinessLineId
        {
            get { return _businesslineid; }
            set { _businesslineid = value; }
        }
        /// <summary>
        /// 业务线名称
        /// </summary>		
        private string _businesslinename;
        public string BusinessLineName
        {
            get { return _businesslinename; }
            set { _businesslinename = value; }
        }
        /// <summary>
        /// 抢光警戒线
        /// </summary>		
        private int? _remindquantity;
        public int? RemindQuantity
        {
            get { return _remindquantity; }
            set { _remindquantity = value; }
        }
        /// <summary>
        /// 抢光警戒线提醒邮箱地址 
        /// </summary>		
        private string _remindemails;
        public string RemindEmails
        {
            get { return _remindemails; }
            set { _remindemails = value; }
        }

        /// <summary>
        /// 优惠券类型，0满减券，1后返券，2实付券，3抵扣券
        /// </summary>
        public int PromotionType { get; set; }

        /// <summary>
        /// 拼团是否可用
        /// </summary>
        public bool EnabledGroupBuy { get; set; }

        /// <summary>
        /// 使用规则说明
        /// </summary>
        public string RuleDescription { get; set; }
        


    }


}
