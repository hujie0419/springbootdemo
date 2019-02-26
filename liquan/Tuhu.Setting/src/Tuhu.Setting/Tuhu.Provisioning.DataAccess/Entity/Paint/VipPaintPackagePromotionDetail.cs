using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Paint
{
    public class VipPaintPackagePromotionDetail
    {
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchCode { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public string PromotionId { get; set; }
    }
}
