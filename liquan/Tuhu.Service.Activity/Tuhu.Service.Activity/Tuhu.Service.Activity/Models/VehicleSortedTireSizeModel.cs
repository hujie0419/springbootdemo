using System;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class VehicleSortedTireSizeModel : BaseModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 轮胎规格
        /// </summary>
        public string TireSize { get; set; }

        /// <summary>
        /// 车型ID
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 各轮胎规格历史销量
        /// </summary>
        public int HistorySaleOrder { get; set; }

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }
    }
}
