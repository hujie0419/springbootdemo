using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class VehicleAdaptTireRequestModel
    {
        /// <summary>
        /// 车型Id
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>当前页，默认1, 最大10000</summary>
		[Range(1, 10000)]
        public int CurrentPage { get; set; } = 1;

        /// <summary>当前页大小，最小1，最大10000</summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
        /// <summary>
        /// 轮胎规格
        /// </summary>
        public List<string> TireSizes { get; set; }
    }
}
