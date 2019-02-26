using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.MoveCarQRCode
{
    /// <summary>
    /// 扫码挪车二维码生成记录
    /// </summary>
    public class MoveCarGenerationRecordsModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 生成数量
        /// </summary>
        public int GeneratedNum { get; set; }

        /// <summary>
        /// 生成状态。0=待生成，1=生成中，2=已生成
        /// </summary>
        public int GeneratingStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string LastUpdateBy { get; set; }
    }
}
