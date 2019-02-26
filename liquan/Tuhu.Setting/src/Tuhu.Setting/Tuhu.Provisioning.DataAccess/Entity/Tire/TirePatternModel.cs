using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public class TirePatternConfig
    {
        public int PKID { get; set; }
        /// <summary>
        /// 轮胎花纹
        /// </summary>
        public string Tire_Pattern { get; set; }
        /// <summary>
        /// 花纹英文名
        /// </summary>
        public string Pattern_EN { get; set; }
        /// <summary>
        /// 花纹中文名
        /// </summary>
        public string Pattern_CN { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string CP_Brand { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }

    public class TirePatternChangeLog
    {
        /// <summary>
        /// 轮胎花纹
        /// </summary>
        public string Tire_Pattern { get; set; }

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

    public class TirePatternConfigQuery
    {
        public string PatternCriterion { get; set; }
        public string BrandCriterion { get; set; }
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

    public class ChangePatternAffectedProduct
    {
        public string PID { get; set; }
    }
}
