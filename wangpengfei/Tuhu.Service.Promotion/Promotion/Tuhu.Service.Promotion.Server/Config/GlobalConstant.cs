using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Server.Config
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class GlobalConstant
    {
        //优惠券  客户端名称
        public static readonly string RedisClient = "Coupon";


        /// <summary>
        /// 优惠券领取规则 缓存key 
        /// </summary>
        public static readonly string RedisKeyForGetRule = "CouponRules:{0}";

        /// <summary>
        /// 优惠券领取规则 缓存 时间
        /// </summary>
        public static readonly double RedisTTLForGetRule = 60 * 60;//s

        /// <summary>
        /// 优惠券领取规则一级审核人- 成本承担方关系 配置 key
        /// </summary>
        public static readonly string CouponGetRuleAudit2ndDepartmentRelation = "CouponGetRuleAudit_2nd_DepartmentRelation";



        /// <summary>
        /// 日志类型 和之前保持一致
        /// </summary>
        public static readonly string LogTypeCouponGetRuleAudit = "PromotionConfigLog";


        /// <summary>
        /// 日志子类型 和之前保持一致
        /// </summary>
        public static readonly string LogObjectTypeCouponGetRule = "SaveGetRule";

        /// <summary>
        /// 日志子类型 
        /// </summary>
        public static readonly string LogObjectTypeCouponGetRuleAudit = "CouponGetRuleAudit";

        /// <summary>
        /// 优惠券防并发前缀
        /// </summary>
        public static readonly string ConcurrencyPrefixForCouponTask = "CouponTask";

        /// <summary>
        /// 订单渠道
        /// </summary>
        public static readonly string RedisKeyForOrderChannel = "AllOrderChannel";
        /// <summary>
        /// 订单渠道 - 过期时间
        /// </summary>
        public static readonly double RedisTTLForOrderChannel = 4 * 60 * 60;//s


        /// <summary>
        /// 优惠券任务对应的类目
        /// </summary>
        public static readonly string RedisKeyForPromotionTaskCategory = "PromotionTaskCategory";
        /// <summary>
        /// 优惠券任务对应的类目 - 过期时间
        /// </summary>
        public static readonly double RedisTTLForPromotionTaskCategory = 1 * 60 * 60;//s


        /// <summary>
        /// 优惠券任务列表
        /// </summary>
        public static readonly string RedisKeyForPromotionTaskList = "PromotionTaskList";
        /// <summary>
        /// 优惠券任务列表 - 过期时间
        /// </summary>
        public static readonly double RedisTTLForPromotionTaskList = 3 * 60;//s
    }
}

