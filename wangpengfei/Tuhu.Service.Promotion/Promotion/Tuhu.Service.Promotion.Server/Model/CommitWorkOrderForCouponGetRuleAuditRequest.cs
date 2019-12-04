using System;

namespace Tuhu.Service.Promotion.Server.Model
{
    /// <summary>
    /// 优惠券领取规则提交到工单 api 请求类
    /// </summary>
    public class CommitWorkOrderForCouponGetRuleAuditRequest
    {

        #region 领取规则信息
        /// <summary>
        ///领取规则guid
        /// </summary>
        public Guid GetRuleGUID { get; set; }

        /// <summary>
        /// 优惠券领取规则 名称
        /// </summary>
        public string PromtionName { get; set; }

        /// <summary>
        ///  优惠券领取规则描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 优惠券领取折扣
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 优惠券使用门槛价格
        /// </summary>
        public decimal Minmoney { get; set; }

        /// <summary>
        /// 折扣力度（面额/使用条件)
        /// </summary>
        private decimal _discountprecent { get; set; }
        public decimal DiscountPrecent
        {
            get
            {
                return Minmoney == 0 ? 1 : (Discount / Minmoney);
                //return Minmoney == 0 ? 1 :decimal.Round((Discount / Minmoney),4);
            }
            set { _discountprecent = value; }
        }

        /// <summary>
        /// 发行量
        /// 格式:千分位格式，1,231,231
        /// </summary> 
        public string Quantity { get; set; }

        /// <summary>
        ///单个人限制领取数量
        /// </summary>
        public int SingleQuantity { get; set; }

        /// <summary>
        /// 总让利额度（面值*发行量）
        /// 格式::千分位格式，1,231,231.00
        /// </summary> 
        public string SumDiscount { get; set; }

        /// <summary>
        /// 自领取后多少天有效
        /// </summary>
        public int Term { get; set; }
        /// <summary>
        /// 最终有效期
        /// </summary>
        private DateTime deadlinedate { get; set; }
        public DateTime DeadLineDate
        {
            get
            {
                return ValiEndDate > DateTime.Parse("2000-1-1") ? ValiEndDate : deadlinedate;
            }
            set { deadlinedate = value; }
        }


        /// <summary>
        /// 有效期
        /// </summary>
        private string expirationperiod { get; set; }
        public string ExpirationPeriod
        {
            get
            {
                return (Term == 0 ? ValiStartDate.ToString("yyyy/MM/dd") + "至" + ValiEndDate.ToString("yyyy/MM/dd") : "自领取后" + Term + "天");
            }
            set { expirationperiod = value; }
        }

        /// <summary>
        /// 固定有效期 开始时间
        /// </summary>
        public DateTime ValiStartDate { get; set; }


        /// <summary>
        /// 固定有效期 结束时间
        /// </summary>
        public DateTime ValiEndDate { get; set; }

        /// <summary>
        /// 是否允许电销发放
        /// </summary>

        public int AllowChanel { get; set; }
        /// <summary>
        /// 是否允许电销发放
        /// </summary>
        private string _allowchanelsstring;
        public string AllowChanelsString
        {
            get
            {
                return AllowChanel == 1 ? "是" : "否";
            }
            set { _allowchanelsstring = value; }
        }

        /// <summary>
        ///电销 固定有效期 开始时间
        /// </summary>
        public DateTime DXStartDate { get; set; }

        /// <summary>
        ///电销 固定有效期 结束时间
        /// </summary>
        public DateTime DXEndDate { get; set; }

        /// <summary>
        ///用户范围0全部，1新用户，2老用户
        /// </summary>
        public int SupportUserRange { get; set; }


        /// <summary>
        ///用户范围0全部，1新用户，2老用户
        /// </summary>
        private string _supportuserrangestring { get; set; }
        public string SupportUserRangeString
        {
            get
            {
                switch (SupportUserRange)
                {
                    case 0:
                        return "全部";
                    case 1:
                        return "新用户";
                    case 2:
                        return "老用户";
                    default:
                        return "错误类型";
                };
            }
            set { _supportuserrangestring = value; }
        }


        /// <summary>
        ///部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        ///用途名称
        /// </summary>
        public string IntentionName { get; set; }
        /// <summary>
        ///业务线id
        /// </summary>
        public int BusinessLineId { get; set; }

        /// <summary>
        ///业务线名称
        /// </summary>
        public string BusinessLineName { get; set; }

        #endregion

        #region 使用规则信息

        /// <summary>
        /// 优惠券类型，0满减券，1后返券，2实付券，3抵扣券
        /// </summary>
        public int PromotionType { get; set; }


        /// <summary>
        /// 优惠券类型，0满减券，1后返券，2实付券，3抵扣券
        /// </summary>

        private string _promotiontypestring;
        public string PromotionTypeString
        {
            get
            {
                switch (PromotionType)
                {
                    case 0:
                        return "满减券";
                    case 1:
                        return "后返券";
                    case 2:
                        return "实付券";
                    case 3:
                        return "抵扣券";
                    default:
                        return "错误类型";
                };
            }
            set { _promotiontypestring = value; }
        }


        /// <summary>
        /// 拼团是否可用
        /// </summary>
        public bool EnabledGroupBuy { get; set; }

        /// <summary>
        /// 拼团是否可用 - 字符串
        /// </summary>
        private string enabledgroupbuystring;
        public string EnabledGroupBuyString
        {
            get
            {
                return EnabledGroupBuy ?"是": "否";
            }
            set { enabledgroupbuystring = value; }
        }


        /// <summary>
        /// 使用规则说明
        /// </summary>
        public string RuleDescription { get; set; }

        /// <summary>
        /// 使用规则查看链接  [需要接入apollo配置 setting的 域名]
        /// </summary>
        public string CouponUseRuleDetailURL { get; set; }

        #endregion

        #region 工单信息
        /// <summary>
        /// 工单类型Id
        /// </summary>
        public int WorkOrderTypeId { get; set; }

        /// <summary>
        /// 工单创建人邮箱
        /// </summary>
        public string UserEmail { get; set; }


        /// <summary>
        /// 工单处理人邮箱
        /// </summary>
        public string TaskOwner { get; set; }

        #endregion
    }
}
