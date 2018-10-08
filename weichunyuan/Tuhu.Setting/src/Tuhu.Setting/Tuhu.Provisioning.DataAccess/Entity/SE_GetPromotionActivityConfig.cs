using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SE_GetPromotionActivityConfig
    {
        /// <summary>
        /// ID
        /// </summary>		
        public Guid? ID { get; set; }
        /// <summary>
        /// ActivityName
        /// </summary>		
        public string ActivityName { get; set; }
        /// <summary>
        /// StartDateTime
        /// </summary>		
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// EndDateTime
        /// </summary>		
        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// Channel
        /// </summary>		
        public string Channel { get; set; }
        /// <summary>
        /// Status
        /// </summary>		
        public bool Status { get; set; }

        /// <summary>
        /// true 新用户
        /// </summary>
        public bool IsNewUser { get; set; }

        /// <summary>
        /// true需要验证码
        /// </summary>
        public bool IsNeedCode { get; set; }

        /// <summary>
        /// TopBanner
        /// </summary>		
        public string TopBanner { get; set; }
        /// <summary>
        /// BottomBanner
        /// </summary>		
        public string BottomBanner { get; set; }
        /// <summary>
        /// NumberCodeTimes
        /// </summary>		
        public int NumberCodeTimes { get; set; }
        /// <summary>
        /// ChartCodeTimes
        /// </summary>		
        public int ChartCodeTimes { get; set; }
        /// <summary>
        /// AutoCompleteTimes
        /// </summary>		
        public int AutoCompleteTimes { get; set; }
        /// <summary>
        /// CodeErrorTimes
        /// </summary>		
        public int CodeErrorTimes { get; set; }
        /// <summary>
        /// LimitPhoneHourse
        /// </summary>		
        public int LimitPhoneHourse { get; set; }
        /// <summary>
        /// NewUserValidMode
        /// </summary>		
        public int NewUserValidMode { get; set; }
        /// <summary>
        /// SuccessfulTopBanner
        /// </summary>		
        public string SuccessfulTopBanner { get; set; }
        /// <summary>
        /// SuccessfulCenterBanner
        /// </summary>		
        public string SuccessfulCenterBanner { get; set; }
        /// <summary>
        /// SuccessfulBottomBanner
        /// </summary>		
        public string SuccessfulBottomBanner { get; set; }
        /// <summary>
        /// SuccessfulIOSUrl
        /// </summary>		
        public string SuccessfulIOSUrl { get; set; }
        /// <summary>
        /// SuccessfulAndroidUrl
        /// </summary>		
        public string SuccessfulAndroidUrl { get; set; }
        /// <summary>
        /// SuccessfulJumpMode
        /// </summary>		
        public int SuccessfulJumpMode { get; set; }
        /// <summary>
        /// SuccessfulWaitTime
        /// </summary>		
        public int SuccessfulWaitTime { get; set; }
        /// <summary>
        /// FailTopBanner
        /// </summary>		
        public string FailTopBanner { get; set; }
        /// <summary>
        /// FailCenterBanner
        /// </summary>		
        public string FailCenterBanner { get; set; }
        /// <summary>
        /// FailBottomBanner
        /// </summary>		
        public string FailBottomBanner { get; set; }
        /// <summary>
        /// FailIOSUrl
        /// </summary>		
        public string FailIOSUrl { get; set; }
        /// <summary>
        /// FailAndroidUrl
        /// </summary>		
        public string FailAndroidUrl { get; set; }
        /// <summary>
        /// FailJumpMode
        /// </summary>		
        public int FailJumpMode { get; set; }
        /// <summary>
        /// FailWaitTime
        /// </summary>		
        public int FailWaitTime { get; set; }
        /// <summary>
        /// CreateDateTime
        /// </summary>		
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// UpdateDateTime
        /// </summary>		
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        /// 是否获取位置
        /// </summary>
        public bool IsPostion { get; set; }

        public bool IsSendMsg { get; set; }

        public string SendMsg { get; set; }


        public IEnumerable<SE_GetPromotionActivityCouponInfoConfig> CouponItems { get; set; }


        public int GetCouponNumbers { get; set; }

        public int? GetCouponTotal { get; set; }

        /// <summary>
        /// 新用户领取底图 ,现在改为按钮的描述
        /// </summary>
        public string NewBottomBanner { get; set; }

        /// <summary>
        /// 新用户跳转Url -- IOS
        /// </summary>
        public string NewPageIOSUrl { get; set; }


        /// <summary>
        /// 新用户页面跳转Url Android
        /// </summary>
        public string NewPageAndroidUrl { get; set; }

        /// <summary>
        /// 弹出层连接
        /// </summary>
        public string NewIOSUrl { get; set; }

        /// <summary>
        /// 弹出层连接
        /// </summary>
        public string NewAndroidUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NewJumpMode { get; set; }


        public int? NewWaitTime { get; set; }

        /// <summary>
        /// 老用户页面成功连接 ios
        /// </summary>
        public string SuccessfulPageIOSUrl { get; set; }

        /// <summary>
        /// 老用户页面anroid
        /// </summary>
        public string SuccessfulPageAndroidUrl { get; set; }

        /// <summary>
        /// 失败页面跳转
        /// </summary>
        public string FailPageIOSUrl { get; set; }

        /// <summary>
        /// 失败页面跳转 Android
        /// </summary>
        public string FailPageAndroidUrl { get; set; }

        /// <summary>
        /// 提示方式 0浮层提示 1页面提示
        /// </summary>
        public int TipStyle { get; set; }

        /// <summary>
        /// 新用户领取成功
        /// </summary>

        public string NewUserTip { get; set; }


        /// <summary>
        /// 老用户领取成功
        /// </summary>

        public string OldUserTip { get; set; }

        /// <summary>
        /// 活动未开始
        /// </summary>

        public string ActivityNoStartTip { get; set; }

        /// <summary>
        /// 活动已结束
        /// </summary>
        public string ActivityOverTip { get; set; }

        /// <summary>
        /// 优惠券领完
        /// </summary>
        public string CouponTip { get; set; }

        /// <summary>
        /// 页面强制关闭
        /// </summary>
        public string PageTip { get; set; }


        /// <summary>
        /// 仅限新用户
        /// </summary>
        public string LimitUserTypeTip { get; set; }


        /// <summary>
        /// 重复领取
        /// </summary>

        public string AlreadyHadTip { get; set; }

        /// <summary>
        /// 服务器异常
        /// </summary>
        public string DefaultTip { get; set; }

        /// <summary>
        /// 小黑屋提示
        /// </summary>

        public string BlackTip { get; set; }


        /// <summary>
        /// 1:QQ 0:微信 -1:普通页面
        /// </summary>
        public int? CardChannel { get; set; }

        /// <summary>
        /// 卡券已消费时跳转的页面链接
        /// </summary>
        public string CardConsumedURL { get; set; }

        /// <summary>
        /// 卡券已过期时跳转的页面链接
        /// </summary>
        public string CardExpireURL { get; set; }

        /// <summary>
        /// 卡券转赠中时跳转的页面链接
        /// </summary>
        public string CardGiftingURL { get; set; }

        /// <summary>
        /// 卡券转赠超时时跳转的页面链接
        /// </summary>
        public string CardGiftTimeOutURL { get; set; }


        /// <summary>
        /// 卡券已删除时跳转的页面链接
        /// </summary>
        public string CardDeleteURL { get; set; }

        /// <summary>
        /// 卡券已失效时跳转的页面链接
        /// </summary>
        public string CardUnavailableURL { get; set; }

        /// <summary>
        /// 返回无效卡券时跳转的链接
        /// </summary>
        public string CardInvalidSerialCodeURL { get; set; }

        /// <summary>
        /// 卡券各类Token获取失败时返回的链接
        /// </summary>
        public string TokenAccessFailedURL { get; set; }


        public string CreatorUser { get; set; }

        public string UpdateUser { get; set; }

    }
}
