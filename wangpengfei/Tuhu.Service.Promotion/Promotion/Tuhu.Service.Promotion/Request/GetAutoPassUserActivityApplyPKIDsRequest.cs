using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
   public class GetAutoPassUserActivityApplyPKIDsRequest
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 页长
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 区域ID，逗号隔开
        /// </summary>
        public string AreaIDs { get; set; }
        /// <summary>
        /// 提高查询速度，传入上一次查询的最大PKID未本次最小PKID
        /// </summary>
        public int minPKID { get; set; }

    }
}
