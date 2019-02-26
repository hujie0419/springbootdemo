using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class ActivityTypeModel
    {

        public Guid ActivityId { get; set; }

        /// <summary>
        /// 1:限时抢购；5：保养定价,11：黎先攀的打折活动
        /// </summary>
        public int Type { get; set; }
    }
}
