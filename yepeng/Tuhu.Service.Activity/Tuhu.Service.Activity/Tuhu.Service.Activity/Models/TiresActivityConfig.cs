using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class TiresActivityConfig
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 微信Url
        /// </summary>
        public string WXUrl { get; set; }
        /// <summary>
        /// AppUrl
        /// </summary>
        public string AppUrl { get; set; }
        /// <summary>
        /// 分享图片
        /// </summary>
        public string ShareImg { get; set; }
        /// <summary>
        /// 分享标题
        /// </summary>
        public string ShareTitle { get; set; }
        /// <summary>
        /// 分享描述
        /// </summary>
        public string ShareDes { get; set; }
        /// <summary>
        /// 是否适配车型
        /// </summary>
        public bool IsAdaptationVehicle { get; set; }
        /// <summary>
        /// 活动规则
        /// </summary>
        public string ActivityRules { get; set; }
        /// <summary>
        /// 活动规则图片
        /// </summary>
        public string ActivityRulesImg { get; set; }
        /// <summary>
        /// 头图
        /// </summary>
        public string HeadImg { get; set; }
        /// <summary>
        /// 不适配图片
        /// </summary>
        public string NoAdaptationImg { get; set; }

        public string BackgroundColor { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 是否显示分期
        /// </summary>
        public bool IsShowInstallmentPrice { get; set; }
        /// <summary>
        /// 楼层活动配置
        /// </summary>
        public IEnumerable<TiresFloorActivityInfo> TiresFloorActivity { get; set; }
    }

    public class TiresFloorActivityInfo
    {
        /// <summary>
        /// 大活动父ID
        /// </summary>

        public Guid TiresActivityId { get; set; }
        /// <summary>
        /// 楼层活动ID
        /// </summary>
        public Guid FloorActivityId { get; set; }
        /// <summary>
        /// 小活动ID
        /// </summary>
        public Guid FlashSaleId { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 活动状态  NoStart(即将开始)  Overdue (已过期)  OnGoing(活动进行中)
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string ImgType { get; set; }
        /// <summary>
        /// 图片URL
        /// </summary>
        public string ImgUrl { get; set; }

        public IEnumerable<ActivityImageConfig> ActivityImageConfig { get; set; }

        public IEnumerable<TiresActivityProductConfig> TiresActivityProductConfig { get; set; }

        public IEnumerable<TireProductInfo> TireProductFiltrationInfo { get; set; }
    }
}
