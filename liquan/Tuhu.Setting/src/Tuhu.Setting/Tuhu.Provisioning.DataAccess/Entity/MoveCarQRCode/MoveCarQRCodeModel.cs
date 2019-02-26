using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.MoveCarQRCode
{
    /// <summary>
    /// 扫码挪车二维码
    /// </summary>
    public class MoveCarQRCodeModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 途虎用户pkid
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 二维码url
        /// </summary>
        public string QRCodeUrl { get; set; }

        /// <summary>
        /// 二维码ID
        /// </summary>
        public long QRCodeID { get; set; }

        /// <summary>
        /// 二维码imageUrl
        /// </summary>
        public string QRCodeImageUrl { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicensePlateNumber { get; set; }

        /// <summary>
        /// 微信openid
        /// </summary>
        public string OpenID { get; set; }

        /// <summary>
        /// 是否下载
        /// </summary>
        public bool IsDownload { get; set; }

        /// <summary>
        /// 是否绑定。指二维码是否与手机号绑定
        /// </summary>
        public bool IsBinding { get; set; }

        /// <summary>
        /// 批次ID
        /// </summary>
        public int BatchID { get; set; }

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
