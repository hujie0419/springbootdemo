using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangInstallFeeConfigModel
    {
        /// <summary>
        /// BaoYangInstallFeeConfig表PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 服务PID
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 二级车型均价价格区间--最低价
        /// </summary>
        public decimal CarMinPrice { get; set; }
        /// <summary>
        /// 二级车型均价价格区间--最高价
        /// </summary>
        public decimal CarMaxPrice { get; set; }
        /// <summary>
        /// 安装费加价
        /// </summary>
        public decimal AddInstallFee { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }

    public class BaoYangService
    {
        /// <summary>
        /// 保养项目Pid
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// 保养项目名称
        /// </summary>
        public string ServiceName { get; set; }
    }
}
