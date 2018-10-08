using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode
{
    public class ThirdPartyBeautyPackageConfigModel
    {
        public int PKID { get; set; }
        /// <summary>
        /// 服务包Id
        /// </summary>
        public Guid PackageId { get; set; }
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
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedDateTime { get; set; }
        /// <summary>
        /// 合作用户Id
        /// </summary>
        public int CooperateId { get; set; }
        /// <summary>
        /// 合作用户名
        /// </summary>
        public string CooperateName { get; set; }
        /// <summary>
        /// 包名称
        /// </summary>
        public string PackageName { get; set; }
        /// <summary>
        /// 包描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 结算方式：ByPeriod据实，PreSettled买断
        /// </summary>
        public string SettlementMethod { get; set; }
        public string SettlementMethodName
        {
            get
            {
                var result = "未知";
                if (SettlementMethod == "ByPeriod")
                    result = "据实";
                else if (SettlementMethod == "PreSettled")
                {
                    result = "买断";
                }
                return result;
            }
        }
        /// <summary>
        /// 第三方appId
        /// </summary>
        public string AppId { get; set; }
    }
}
