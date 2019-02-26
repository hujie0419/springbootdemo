using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Provisioning.Areas.ActivityPage.Models
{
    public class LogResponse
    {
       public string  ResponseCode { get; set; }
       public string ResponseMessage { get; set; }
        public List<SalePromotionActivityLogModel> LogPageModel { get; set; }
        public int TotalCount { get; set; }

    }
}