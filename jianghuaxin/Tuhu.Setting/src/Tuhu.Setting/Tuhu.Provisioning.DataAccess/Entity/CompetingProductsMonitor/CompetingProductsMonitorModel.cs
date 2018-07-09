using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.CompetingProductsMonitor
{
    public class CompetingProductsMonitorModel:TireListModel
    {
        #region 竞品监控信息
        public long ItemID { get; set; }
        public long SkuID { get; set; }
        public string ItemCode { get; set; }
        public string ShopCode { get; set; }
        public string Title { get; set; }
        public string Properties { get; set; }
        public int MonitorCount { get; set; }
        public bool ThirdParty { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool Is_Deleted { get; set; }
        #endregion
    }
}
