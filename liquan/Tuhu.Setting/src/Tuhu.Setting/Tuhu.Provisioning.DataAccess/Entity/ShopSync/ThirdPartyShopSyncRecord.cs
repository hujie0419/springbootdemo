using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ShopSync
{
    public class ThirdPartyShopSyncRecord
    {
        public int PKID { get; set; }
        /// <summary>
        /// 第三方平台名
        /// </summary>
        public string ThirdPartyName { get; set; }
        /// <summary>
        /// 第三方门店ID
        /// </summary>
        public string ThirdPartyShopId { get; set; }
        /// <summary>
        /// 途虎门店ID
        /// </summary>
        public int TuhuShopId { get; set; }
        /// <summary>
        /// 门店全称
        /// </summary>
        public string ShopFullName { get; set; }
        /// <summary>
        /// 门店简称
        /// </summary>
        public string ShopSimpleName { get; set; }
        /// <summary>
        /// 门店营业时间
        /// </summary>
        public string WorkTime { get; set; }
        /// <summary>
        /// 门店地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 门店状态 门店本身的是否开业(open、close)
        /// </summary>
        public string ShopStatus { get; set; }
        /// <summary>
        /// 服务的上下架(OnShelf OffShelf)
        /// </summary>
        public string ServiceStatus { get; set; }
        /// <summary>
        /// 同步状态 success(同步成功) fail（同步失败）
        /// </summary>
        public string SyncStatus { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
