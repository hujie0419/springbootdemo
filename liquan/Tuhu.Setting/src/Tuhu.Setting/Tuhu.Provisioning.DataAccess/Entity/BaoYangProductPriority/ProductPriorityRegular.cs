using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority
{
    public class ProductPriorityRegular
    {
        public int PKID { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 系列
        /// </summary>
        public string Series { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 部件名称
        /// </summary>
        public string PartName { get; set; }
    }
}
