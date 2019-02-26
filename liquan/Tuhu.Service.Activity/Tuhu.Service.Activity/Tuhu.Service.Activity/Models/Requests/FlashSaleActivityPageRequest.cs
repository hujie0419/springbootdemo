using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class FlashSaleActivityPageRequest
    {
        public Guid activityID { get; set; }

        public List<string> pids { get; set; }
    }
}
