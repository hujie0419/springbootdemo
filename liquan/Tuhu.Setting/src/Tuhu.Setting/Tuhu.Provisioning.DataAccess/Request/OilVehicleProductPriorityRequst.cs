using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Request
{
    public class OilVehicleProductPriorityRequst : VehicleProductPriorityRequest
    {
        /// <summary>
        /// 原厂等级 
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        ///产品排序等级
        /// </summary>
        public string ProductPriorityGrade { get; set; }
        /// <summary>
        /// 新粘度
        /// </summary>
        public string NewViscosity { get; set; }
        /// <summary>
        /// 原厂粘度
        /// </summary>
        public string Viscosity { get; set; }

        public bool IsNewViscosity { get; set; }
    }
}
