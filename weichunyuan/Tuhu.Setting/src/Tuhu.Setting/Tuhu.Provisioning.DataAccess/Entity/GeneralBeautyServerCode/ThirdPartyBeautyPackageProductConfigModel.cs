using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode
{
    public class ThirdPartyBeautyPackageProductConfigModel
    {
        public int PKID { get; set; }
        /// <summary>
        /// 合作用户ID
        /// </summary>
        public int CooperateId { get; set; }
        /// <summary>
        /// 大客户美容基础产品id
        /// </summary>
        public int CodeTypeConfigId { get; set; }
        /// <summary>
        /// 大客户美容基础产品pid
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementMethod { get; set; }
        /// <summary>
        /// 结算价
        /// </summary>
        public Decimal SettlementPrice { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUser { get; set; }
       /// <summary>
       /// 创建时间
       /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// 展示时间
        /// </summary>
        public string CreatedDateTimeString {
            get { return CreatedDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedDateTime { get; set; }
        /// <summary>
        /// 个数
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 服务包Id
        /// </summary>
        public Guid PackageId { get; set; }
        /// <summary>
        /// 服务描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 领取后有效天数
        /// </summary>
        public int ValidDate { get; set; }
    }
}
