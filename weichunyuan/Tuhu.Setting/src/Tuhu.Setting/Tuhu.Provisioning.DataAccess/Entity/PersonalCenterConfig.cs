using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PersonalCenterConfig
    {

        public int Id { get; set; }
        /// <summary>
        /// 位置 （1:签到2:个人中心3:会员中心4:订单成功页5:违章查询无结果页6:会员商城7:个人中心浮层8:会员商城浮层9:第三方参数10:个人中心中间Banner11：下单成功页分享）
        /// </summary>
        public int Location { get; set; }

        public int? Sort { get; set; }

        public int? Grade { get; set; }

        public int UrlType { get; set; }

        public bool Status { get; set; }

        public string Image { get; set; }

        public string Link { get; set; }

        public string IOSProcessValue { get; set; }

        public string AndroidProcessValue { get; set; }

        public string IOSCommunicationValue { get; set; }

        public string AndroidCommunicationValue { get; set; }

        public DateTime? CreateTime { get; set; }


        public int? VIPAuthorizationRuleId { get; set; }

        public string RuleName { get; set; }

        public string Route { get; set; }


        /// <summary>
        ///1. 途虎养车Tuhu
        ///2. 途虎拼团购
        ///3. 途虎查违章
        ///4. 途虎洗车
        ///5. 途虎众测
        ///6. 途虎工场店
        ///7. 途虎问答 
        /// </summary>
        public string TargetSmallAppId { get; set; }

        public string TargetSmallAppUrl { get; set; }
    }

    public class BannerConfig : PersonalCenterConfig
    {
        public string Description { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string StartVersion { get; set; }
        public string EndVersion { get; set; }
        public string Creator { get; set; }
        public int Channel { get; set; }
        public string NoticeChannels { get; set; }
        public string TargetGroups { get; set; }
        public int ShareType { get; set; }
    }

    public class BannerFilterQuery
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? IDCriterion { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string DescriptionCriterion { get; set; }
        /// <summary>
        /// 平台（0：全部 1：android 2：ios）
        /// </summary>
        public int ChannelCriterion { get; set; }
        /// <summary>
        /// 位置 （0：全部 1:签到 2:个人中心 3:会员中心 4:订单成功页 5:违章查询无结果页 6:会员商城 7:个人中心浮层 8:会员商城浮层 9:第三方参数 10:个人中心中间Banner 11：下单成功页分享）
        /// </summary>
        public int LocationCriterion { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool? StatusCriterion { get; set; }
        /// <summary>
        /// 开始版本
        /// </summary>
        public string StartVersionCriterion { get; set; }
        /// <summary>
        /// 结束版本
        /// </summary>
        public string EndVersionCriterion { get; set; }
        /// <summary>
        /// 目标群体 
        /// </summary>
        public string TargetGroupsCriterion { get; set; }
        /// <summary>
        /// 轮播顺序
        /// </summary>
        public int? SortCriterion { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDateCriterion { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDateCriterion { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorCriterion { get; set; }


        /// <summary>
        ///1. 途虎养车Tuhu
        ///2. 途虎拼团购
        ///3. 途虎查违章
        ///4. 途虎洗车
        ///5. 途虎众测
        ///6. 途虎工场店
        ///7. 途虎问答 
        /// </summary>
        public string TargetSmallAppIdCriterion { get; set; }
    }
}
