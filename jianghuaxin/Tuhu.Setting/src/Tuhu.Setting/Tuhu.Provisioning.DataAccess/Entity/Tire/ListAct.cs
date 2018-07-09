using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    /// <summary>
    /// 轮胎活动筛选条件
    /// </summary>
    public class ListActCondition
    {
        /// <summary>
        /// 品牌系列 ;分割
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 价格区间 ;分割
        /// </summary>
        public string PriceRange { get; set; }

        /// <summary>
        /// 车型类型 ;分割
        /// </summary>
        public string VehicleBodyType { get; set; }

        /// <summary>
        /// 车型ID ;分割
        /// </summary>
        public string VehicleId { get; set; }
        /// <summary>
        /// 轮胎规格 ;分割
        /// </summary>
        public string TireSize { get; set; }

        /// <summary>
        /// 展示类型 0全部 1展示配置有活动的车型规格 -1展示没有配置活动的车型规格
        /// </summary>
        public int ShowType { get; set; }
        /// <summary>
        /// 进行状态  0全部 1进行中 -1未开始 -2已结束
        /// </summary>
        public int IngType { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动状态  0全部 1启用  -1禁用
        /// </summary>
        public int IsActive { get; set; }
        /// <summary>
        /// 0全部 1防爆  -1非防爆
        /// </summary>
        public int IsRof { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }


        /// <summary>
        /// 活动优先级
        /// </summary>
        public int Sort { get; set; }
    }

    public class ActivityItem
    {
        /// <summary>
        /// 德系 日系。。
        /// </summary>
        public string BrandCategory { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 车系 第二级
        /// </summary>
        public string Vehicle { get; set; }
        /// <summary>
        /// 车型ID
        /// </summary>
        public string VehicleId { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string TireSize { get; set; }
        /// <summary>
        /// 车价
        /// </summary>
        public decimal? MinPrice { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid? ActivityID { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 状态 -1删除 0禁用 1启用
        /// </summary>
        public int? Status { get; set; }

        public  int Sort { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 浮层图片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 新增浮层图片
        /// </summary>
        public string Image2 { get; set; }
        /// <summary>
        /// 活动角标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 领券ID
        /// </summary>
        public Guid? GetRuleGUID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 0参与 1领券
        /// </summary>
        public int? ButtonType { get; set; }
        /// <summary>
        /// 按钮文本
        /// </summary>
        public string ButtonText { get; set; }

        public IEnumerable<ActivityProducts> Products { get; set; }

        public IEnumerable<VehicleAndTireSize> VehicleIDTireSize { get; set; }

        public string PID { get; set; }
        public int Postion { get; set; }

        public string DisplayName { get; set; }
        public string StartTimeStr { get; set; }
        public string EndTimeStr { get; set; }

        public int OldStatus { get; set; }
        public string Manager { get; set; }
        public int? PKId { get; set; }
        public string HashKey { get; set; }
    }

    public class ActivityProducts
    {
        public Guid ActivityID { get; set; }
        public string PID { get; set; }
        public int Postion { get; set; }
        public string DisplayName { get; set; }
    }
    public class VehicleAndTireSize
    {
        public string VehicleID { get; set; }
        public string TireSize { get; set; }
    }

    public class ListActLog
    {
        public string VehicleId { get; set; }
        public string TireSize { get; set; }
       
        public Guid ActivityID { get; set; }
        public string ActivityName { get; set; }
        public int Status { get; set; }
       
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
     
        public string Image { get; set; }
        public string Image2 { get; set; }
        public string Icon { get; set; }
       
        public Guid? GetRuleGUID { get; set; }
        

       
        public int ButtonType { get; set; }
       
        public string ButtonText { get; set; }
        public Guid GUID { get; set; }
        public IEnumerable<ActivityProducts> Products { get; set; }
    }

    public class TireChangedActivityLog
    {
        public string VehicleId { get; set; }
        public string TireSize { get; set; }

        public string HashKey { get; set; }
        public int? State { get; set; }
        public string OldHashKey { get; set; }
        public int? OldState { get; set; }
        public string Author { get; set; }
        public string Message{ get; set; }
        public DateTime CreateDatetime { get; set; }
    }
}
