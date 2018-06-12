using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public class TireSpecParamsModel : TireSpecParamsConfig
    {
        public string ProductID { get; set; }
        public string DisplayName { get; set; }
        public string TireSize { get; set; }
    }

    public class TireSpecParamsConfig
    {
        public int PKID { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public string PId { get; set; }

        /// <summary>
        /// 质检名称
        /// </summary>
        public string QualityInspectionName { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string OriginPlace { get; set; }

        /// <summary>
        /// 轮辋保护
        /// </summary>
        public bool? RimProtection { get; set; }

        /// <summary>
        /// 载重
        /// </summary>
        public string TireLoad { get; set; }

        /// <summary>
        /// M+S指数
        /// </summary>
        public bool? MuddyAndSnow { get; set; }

        /// <summary>
        /// 3T耐磨指数
        /// </summary>
        public string ThreeT_Treadwear { get; set; }

        /// <summary>
        /// 3T抓地指数
        /// </summary>
        public string ThreeT_Traction { get; set; }

        /// <summary>
        /// 3T温度指数
        /// </summary>
        public string ThreeT_Temperature { get; set; }

        /// <summary>
        /// 胎冠聚酯层数
        /// </summary>
        public int? TireCrown_Polyester { get; set; }

        /// <summary>
        /// 胎冠钢丝层数
        /// </summary>
        public int? TireCrown_Steel { get; set; }

        /// <summary>
        /// 胎冠尼龙层数
        /// </summary>
        public int? TireCrown_Nylon { get; set; }

        /// <summary>
        /// 胎侧聚酯层数
        /// </summary>
        public int? TireSideWall_Polyester { get; set; }

        /// <summary>
        /// 标签滚动阻力
        /// </summary>
        public string TireLable_RollResistance { get; set; }

        /// <summary>
        /// 标签湿滑抓地性
        /// </summary>
        public string TireLable_WetGrip { get; set; }

        /// <summary>
        /// 标签噪音值
        /// </summary>
        public string TireLable_Noise { get; set; }

        /// <summary>
        /// 花纹对称
        /// </summary>
        public string PatternSymmetry { get; set; }

        /// <summary>
        /// 导向
        /// </summary>
        public string TireGuideRotation { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }

    public class TireSpecParamsEditLog
    {
        /// <summary>
        /// 产品id
        /// </summary>
        public string PId { get; set; }

        /// <summary>
        /// 修改前数据
        /// </summary>
        public string ChangeBefore { get; set; }

        /// <summary>
        /// 修改后数据
        /// </summary>
        public string ChangeAfter { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }

    public class TireSpecParamsConfigQuery
    {
        public string PIDCriterion { get; set; }
        public string DisplayNameCriterion { get; set; }
        /// <summary>
        /// 页面序号（1开始）
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页显示数据条数
        /// </summary>
        public int PageDataQuantity { get; set; }
        /// <summary>
        /// 查询出的总条数
        /// </summary>
        public int TotalCount { get; set; }
    }
}
