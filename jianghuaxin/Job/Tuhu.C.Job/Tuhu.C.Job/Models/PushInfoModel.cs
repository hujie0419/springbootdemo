
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Models
{
    public class PushInfoModel
    {
        public string OrderNo { set; get; }
        public string OrderID { set; get; }
        public string Device_Tokens { set; get; }
        public string Channel { set; get; }
        public string UserID { set; get; }
        public string Phone { set; get; }
        public string Products { set; get; }
        public string DeliveryCode { set; get; }
        public string Address { set; get; }
        public string Telephone { set; get; }
        public string CarparName { set; get; }
        public string InstallCode { set; get; }
        public DateTime OrderDatetime { get; set; }


    }
}
