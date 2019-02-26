using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode
{
    public class ThirdPartyBeautyPackageRecordDetailModel
    {
        public int PKID { get; set; }
        /// <summary>
        /// 包Id
        /// </summary>
        public Guid PackageId { get; set; }
        /// <summary>
        /// 服务码
        /// </summary>
        public string ServiceCode { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedDateTime { get; set; }
        /// <summary>
        /// 包产品配置id
        /// </summary>
        public int PackageProductId { get; set; }
        /// <summary>
        /// 发放包记录id
        /// </summary>
        public int PackageRecordId { get; set; }
    }
}
