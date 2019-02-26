using System;
using System.Collections.Generic;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class TireActivityModel : BaseModel
    {
        /// <summary>活动ID</summary>
        [Column("ActivityID")]
        public Guid ActivityId { get; set; }
        /// <summary>车型ID</summary>
        [Column("VehicleID")]
        public string VehicleId { get; set; }
        /// <summary>轮胎规格</summary>
        public string TireSize { get; set; }
        /// <summary>活动名称</summary>
        public string ActivityName { get; set; }
        /// <summary>开始时间</summary>
        public DateTime StartTime { get; set; }
        /// <summary>结束时间</summary>
        public DateTime EndTime { get; set; }
        /// <summary>浮层图片</summary>
        public string Image { get; set; }
        /// <summary>新增浮层图片</summary>
        public string Image2 { get; set; }
        /// <summary>活动图标</summary>
        public string Icon { get; set; }
        /// <summary>按钮类型(0是立即参与活动1是立即领券)</summary>
        public int ButtonType { get; set; }
        /// <summary>领券ID</summary>
        [Column("GetRuleGUID")]
        public Guid? GetRuleId { get; set; }
        /// <summary>按钮文本</summary>
        public string ButtonText { get; set; }

        /// <summary>
        /// 1:高 2:中低 3:低
        /// </summary>
        [Column("Sort")]
        public int SortLevel { get; set; }

        public IEnumerable<string> Pids { get; set; }
    }
}
