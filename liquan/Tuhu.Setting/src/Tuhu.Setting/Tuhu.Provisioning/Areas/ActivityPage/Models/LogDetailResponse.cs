using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Provisioning.Areas.ActivityPage.Models
{
    public class LogDetailResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public List<SalePromotionActivityLogDetail> LogPageModel { get; set; }
    }
}