using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.NewCoupon
{
    public class NewCouponActivity
    {
        public int PKID { get; set; }
        //是否启用
        public bool IsEnabled { get; set; }
        //活动ID
        public Guid ActivityId { get; set; }
        //活动名称
        public string ActivityName { get; set; }
        //渠道
        public string Channel { get; set; }
        //开始时间
        public DateTime? StartTime { get; set; }
        //结束时间
        public DateTime? EndTime { get; set; }
        //领取之日起的天数
        public int ValidDays { get; set; }
        //每人每次领取数量
        public int QuantityPerUser { get; set; }
        //每人每天参与次数
        public int DailyQuantityPerUser { get; set; }
        //页面参与人数
        public int PageQuantity { get; set; }
        //是否是新用户
        public bool IsNewUser { get; set; }
        //头图
        public string HeadImgUrl { get; set; }
        //底图
        public string BottomImgUrl { get; set; }
        //活动规则
        public string Description { get; set; }
        //验证码次数
        public int VerifyCodeNum { get; set; }
        //图片验证次数
        public int VerifyImgNum { get; set; }
        //自动结束次数
        public int AutoEndNum { get; set; }
        //验证码错误次数
        public int VerifyCodeErrorNum { get; set; }
        //手机锁定时间  小时
        public int MobileLockTime { get; set; }
        //成功页面类型  默认页面/自定义页面
        public bool IsDefaultPage { get; set; }
        //好友运气
        public bool IsShowLuckFriends { get; set; }
        //成功头图
        public string SuccessHeadImgUrl { get; set; }
        //成功底图
        public string SuccessBottomImgUrl { get; set; }
        //默认成功按钮文字
        public string ButtonText { get; set; }
        //默认成功按钮跳转链接（小程序）
        public string ButtonUrl { get; set; }
        //默认成功按钮跳转链接（H5）
        public string ButtonUrlH5 { get; set; }
        //默认失败按钮文字
        public string DefaultFailText { get; set; }
        //默认失败按钮跳转链接
        public string DefaultFailUrl { get; set; }
        //自定义 成功按钮跳转链接
        public string SuccessButtonUrl { get; set; }
        //自定义 失败按钮跳转链接
        public string FailButtonUrl { get; set; }
        //领取成功消息
         
        public string SuccessMsg { get; set; }
        //活动未开始消息
        public string NoStartMsg { get; set; }
        //活动过期消息
        public string OverdueMsg { get; set; }
        //优惠券领完消息
        public string NoCouponMsg { get; set; }
        //页面关闭消息
        public string PageClosedMsg { get; set; }
        //重复领取消息
        public string DuplicateAttemptMsg { get; set; }
        //服务器异常消息
        public string ServiceExceptionsMsg { get; set; }
        //小黑屋消息
        public string BlackHouseMsg { get; set; }
        //成功页面推荐活动信息
        public List<RecommendActivityConfig> RecommendActivityForSuccess { get; set; }
        //初始化页面推荐活动信息
        public List<RecommendActivityConfig> RecommendActivityForInit { get; set; }

        public List<RecommendActivityConfig> AllRecommendActivity { get; set; }

        public List<CouponRulesConfig> CouponRulesConfig { get; set; }

        public DateTime CreateDateTime { get; set; }

        public string CreateUser { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

        public string LastUpdateUser { get; set; }

        public int Total { get; set; }
        ///页面用途  1  外部投放  0 订单分享
        public int PageType { get; set; }
        ///领券是否随机
        public bool IsRandom { get; set; }
        //负责人
        public string Owner { get; set; }

        public string RandomGroupId { get; set; }
        //分享ID
        public int AppShareId { get; set; }

        /// <summary>
        /// 随机优惠券的大礼包
        /// </summary>
        public string RandomBigGroupId { get; set; }

        /// <summary>
        /// 大奖随机起始位置
        /// </summary>
        public int RandomMinPos { get; set; }


        /// <summary>
        /// 大奖随机结束位置
        /// </summary>
        public int RandomMaxPos { get; set; }


        /// <summary>
        /// 次数用光
        /// </summary>
        public string FinishCount { get; set; }


        /// <summary>
        /// 活动标签
        /// </summary>
        public string ActivityDesc { get; set; }

    }

    public class CouponRulesConfig
    {
        public int PKID { get; set; }

        public Guid ActivityId { get; set; }

        public Guid RulesGUID { get; set; }
        //有效天数
        public int? ValidDays { get; set; }
        //用户类型
        public int UserType { get; set; }
        /// 开始时间
        public DateTime? ValidStartDateTime { get; set; }
        /// 结束时间 
        public DateTime? ValidEndDateTime { get; set; }
        /// 说明
        public string Description { get; set; }
        /// 使用条件
        public decimal MinMoney { get; set; }
        /// 限领数量
        public int SingleQuantity { get; set; }
        /// 总数量
        public int? Quantity { get; set; }

        public DateTime CreateDateTime { get; set; }
    }

    public class RecommendActivityConfig
    {
        public int PKID { get; set; }
        //活动ID
        public Guid ActivityId { get; set; }
        //活动Type
        public RecommendActivityType ActivityType { get; set; }
        //图片链接
        public string ImgUrl { get; set; }
        //活动链接
        public string ActivityUrl { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }
    }

    public enum RecommendActivityType
    {
        InitActivity,//初始化页面推荐活动
        SuccessActivity//成功页面推荐活动
    }
}
