using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;

namespace Tuhu.Provisioning.Models
{
    public class CycleCountActivityViewModel
    {
        /// <summary>
        /// 即将开始
        /// </summary>
        public List<ActivityBoardViewModel> JustStarting { get; set; }
        /// <summary>
        /// 正在进行中
        /// </summary>
        public List<ActivityBoardViewModel> OnGoing { get; set; }
        /// <summary>
        /// 快要结束
        /// </summary>
        public List<ActivityBoardViewModel> TowardsTheEnd { get; set; }
    }
}