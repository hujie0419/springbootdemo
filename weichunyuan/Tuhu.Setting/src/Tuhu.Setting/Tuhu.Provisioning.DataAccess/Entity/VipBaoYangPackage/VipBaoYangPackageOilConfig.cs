using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.VipBaoYangPackage
{
    public class VipBaoYangPackageOilConfig
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        /// 套餐ID
        /// </summary>
        public int PackageId { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// 系列
        /// </summary>
        public string Series { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 更改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
