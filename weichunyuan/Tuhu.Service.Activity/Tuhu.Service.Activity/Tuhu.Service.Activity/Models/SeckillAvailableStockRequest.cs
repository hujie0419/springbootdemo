using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
  public class SeckillAvailableStockInfoRequest
    {
        /// <summary>
        /// 产品id
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public Guid ActivityId { get; set; }
    }
}
