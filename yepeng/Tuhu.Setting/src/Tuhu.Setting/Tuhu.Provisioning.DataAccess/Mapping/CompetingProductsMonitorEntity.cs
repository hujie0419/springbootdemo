using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class CompetingProductsMonitorEntity
    {
        public int PKID { get; set; }
        [Required]
        public string ShopCode { get; set; }
        [Required]
        public string Pid { get; set; }
        [Required]
        public long ItemID { get; set; }
        public long SkuID { get; set; }
        public string ItemCode { get; set; }
        public string Properties { get; set; }
        public decimal Price { get; set; }
        public bool Promotion { get; set; }
        public string Title { get; set; }
        public bool ThirdParty { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool Is_Deleted { get; set; }

        #region 补充字段
        /// <summary>
        /// 监控数量
        /// </summary>
        public int MonitorCount { get; set; }

        public int MinPrice { get; set; }

        #endregion 
    }
}
