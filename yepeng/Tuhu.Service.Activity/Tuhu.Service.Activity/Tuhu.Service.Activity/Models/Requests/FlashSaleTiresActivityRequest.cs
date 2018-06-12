using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class FlashSaleTiresActivityRequest
    {
        //ID
        public int RegionId { get; set; }
        //需要适配的轮胎尺寸
        public string TireSize { get; set; }
        //活动ID
        public Guid ActivityId { get; set; }
    }
}
