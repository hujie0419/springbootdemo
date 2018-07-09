using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.C.Job.GaizhuangRecommendCache.Model
{
    /// <summary>
    /// 车型实体基类
    /// 
    /// </summary>
    public class BaseVehicleModel : BaseModel
    {
        /// <summary>
        /// 力洋编号
        /// 
        /// </summary>
        public string LiYangId { get; set; }

        /// <summary>
        /// 嘉之道编号
        /// 
        /// </summary>
        public string Tid { get; set; }

        /// <summary>
        /// 车型编号
        /// 
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 品牌名称
        /// 
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 车型名称
        /// 
        /// </summary>
        public string Vehicle { get; set; }

        /// <summary>
        /// 排量
        /// 
        /// </summary>
        public string PaiLiang { get; set; }

        /// <summary>
        /// 生产年份
        /// 
        /// </summary>
        public string Nian { get; set; }

        /// <summary>
        /// 生产年份子款型名称（例如：“2010款 2.0T 双离合 Quattro 四驱 越野版”）
        /// 
        /// </summary>
        public string SalesName { get; set; }

        /// <summary>
        /// 燃料类型
        /// 
        /// </summary>
        public string FuelType { get; set; }

        /// <summary>
        /// 生产起始年份
        /// 
        /// </summary>
        public string StartYear { get; set; }

        /// <summary>
        /// 生产结束年份
        /// 
        /// </summary>
        public string EndYear { get; set; }

        /// <summary>
        /// 平均价格
        /// 
        /// </summary>
        public float AvgPrice { get; set; }
    }
}
