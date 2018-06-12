using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public class TireInstallmentConfigModel
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brands { get; set; }
        /// <summary>
        /// 花纹
        /// </summary>
        public string Patterns { get; set; }
        public string TireSize { get; set; }
      
        public string Rims { get; set; }
        public string Rof { get; set; }
       
        public string PID { get; set; }
        public bool? IsConfig { get; set; }

        public int? OnSale { get; set; }

        public int? ThreeConfig{get; set;}
        public int? SixConfig { get; set; }
        public int? TweleveConfig { get; set; }
    }
    public class TireInstallmentLog
    {
        public string PID { get; set; }
        public string ProductName { get; set; }
        public string TireSize { get; set; }
        public bool IsInstallmentOpen { get; set; }
        /// <summary>
        /// 三期
        /// </summary>
        public InstallmentType? ThreePeriods { get; set; }
        /// <summary>
        /// 六期
        /// </summary>
        public InstallmentType? SixPeriods { get; set; }
        /// <summary>
        /// 十二期
        /// </summary>
        public InstallmentType? TwelvePeriods { get; set; }
    }
}
