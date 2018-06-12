using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Model
{
    public class OrderStatusModel : BaseModel
    {
        //PKID,OrderNo,Status,PurchaseStatus,DeliveryStatus,PayStatus,InstallStatus,InvoiceStatus 
        public int PKID { get; set; }
        public string OrderNo { get; set; }
        public string Status { get; set; }
        public string PurchaseStatus { get; set; }
        public string DeliveryStatus { get; set; }
        public string PayStatus { get; set; }
        public string InstallStatus { get; set; }
        public string InvoiceStatus { get; set; }
    }
}
