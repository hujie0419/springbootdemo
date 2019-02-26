using System;
using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class HomePagePopup
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 详细描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 图片链接
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 跳转链接
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 小程序页面跳转链接
        /// </summary>
        public string MGPageUrl { get; set; }
        /// <summary>
        /// 小程序Id
        /// </summary>
        public string MiniGramId { get; set; }
        /// <summary>
        /// 平台（0：全部 1：android 2：ios）
        /// </summary>
        public int Channel { get; set; }
        /// <summary>
        /// 位置 （1:首页）
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 目标群体 （0：全部 1：未下单2：未登录）
        /// </summary>
        public string TargetGroups { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 周期
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// 开始版本
        /// </summary>
        public string StartVersion { get; set; }
        /// <summary>
        /// 结束版本
        /// </summary>
        public string EndVersion { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime EndDateTime{ get; set; }
        /// <summary>
        /// 是否生效
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime{ get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 应用市场来源
        /// </summary>
        public string NoticeChannel  { get; set; }
        /// <summary>
        /// 应用市场渠道 
        /// </summary>
        public string NewNoticeChannel { get; set; }
    }

    public class HomePagePopupQuery
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string DescriptionCriterion { get; set; }
        /// <summary>
        /// 平台（0：全部 1：android 2：ios）
        /// </summary>
        public int ChannelCriterion { get; set; }
        /// <summary>
        /// 位置 （1:首页）
        /// </summary>
        public int PositionCriterion { get; set; }
        /// <summary>
        /// 目标群体 （0：全部 1：未下单2：未登录）
        /// </summary>
        public string TargetGroupsCriterion { get; set; }
        /// <summary>
        /// 开始版本
        /// </summary>
        public string StartVersionCriterion { get; set; }
        /// <summary>
        /// 结束版本
        /// </summary>
        public string EndVersionCriterion { get; set; }
        /// <summary>
        /// 显示状态：0(全部)， 1（显示），2（不显示）
        /// </summary>
        public int VisibleCriterion { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorCriterion { get; set; }
        /// <summary>
        /// 排序标准：0（ID降序）,1（ID升序）,2（优先级降序）,3（优先级升序）,4（周期降序）,5（周期升序）,6（创建时间降序）,7（创建时间升序）
        /// </summary>
        public int OrderCriterion { get; set; }
    }
    public class HomePagePopupAnimation
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKId { get; set; }
        /// <summary>
        /// 配置ID
        /// </summary>
        public int PopupConfigId { get; set; }
        /// <summary>
        /// 图片链接
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 动作类型
        /// </summary>
        public int MovementType { get; set; }
        /// <summary>
        /// 图片宽度
        /// </summary>
        public int? ImageWidth { get; set; }
        /// <summary>
        /// 图片宽度
        /// </summary>
        public int? ImageHeight{ get; set; }
        /// <summary>
        /// 左边距
        /// </summary>
        public int? LeftMargin { get; set; }
        /// <summary>
        ///上边距
        /// </summary>
        public int? TopMargin { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int? ZIndex { get; set; }
        /// <summary>
        /// 跳转链接
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 小程序页面跳转链接
        /// </summary>
        public string MGPageUrl { get; set; }
        /// <summary>
        /// 小程序Id
        /// </summary>
        public string MiniGramId { get; set; }

    }
    public class HomePagePopupTargetGroup
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKId { get; set; }
        /// <summary>
        /// 目标用户
        /// </summary>
        public string TargetGroups { get; set; }
        /// <summary>
        /// 目标值
        /// </summary>
        public string TargetKey { get; set; }
    }
    public class NoticeChannel
    {
        /// <summary>
        /// PKId
        /// </summary>
        public int PKId { get; set; }
        /// <summary>
        /// 应用市场渠道名
        /// </summary>
        public string  Name { get; set; }
        /// <summary>
        /// 应用市场渠道显示名
        /// </summary>
        public string DisplayName { get; set; }
    }

    public class CouponsInPopup
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// parentId
        /// </summary>
        public int PopupAnimationId { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// 发券人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 优惠券Id或GUID
        /// </summary>
        public string CouponId { get; set; }
    }
}
