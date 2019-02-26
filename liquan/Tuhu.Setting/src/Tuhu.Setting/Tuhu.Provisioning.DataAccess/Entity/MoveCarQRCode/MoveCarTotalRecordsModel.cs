using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.MoveCarQRCode
{
    /// <summary>
    /// 总生成下载记录
    /// </summary>
    public class MoveCarTotalRecordsModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 已生成数量
        /// </summary>
        public long GeneratedNum { get; set; }

        /// <summary>
        /// 已下载数量
        /// </summary>
        public long DownloadedNum { get; set; }

        /// <summary>
        /// 可下载数量
        /// </summary>
        public long DownloadableNum { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

    }
}
