using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    public class ObsoleteCouponListReponse
    {
        /// <summary>
        /// 作废成功id
        /// </summary>
        public string SuccessedIDs { get; set; }
        /// <summary>
        /// 作废失败id
        /// </summary>
        public string FaidedIDs { get; set; }
    }
}
