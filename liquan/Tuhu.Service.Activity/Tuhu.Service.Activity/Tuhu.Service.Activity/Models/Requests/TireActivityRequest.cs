using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class TireActivityRequest
    {
        /// <summary>
        /// 车型ID
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 轮胎规格
        /// </summary>
        public string TireSize { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityID { get; set; }

    }
}
