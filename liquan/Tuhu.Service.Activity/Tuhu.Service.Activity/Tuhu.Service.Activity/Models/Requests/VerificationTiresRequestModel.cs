using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class VerificationTiresRequestModel
    {
        //string addressPhone,int number, string userIp, string userPhone, string deviceId
        public  string AddressPhone { set; get; }
        public  int Number { set; get; }
        public  string UserIp { set; get; }
        public  string UserPhone { set; get; }
        public  string DeviceId { set; get; }
    }

    public class TiresOrderRecordRequestModel : VerificationTiresRequestModel
    {
        public  int OrderId { set; get; }
    }

    public class TiresOrderRecordModel : TiresOrderRecordRequestModel
    {
        public int Pkid { set; get; }
        public bool IsRevoke { set; get; }
        public DateTime CreateTime { set; get; }
        public DateTime LastUpdateTime { set; get; }
    }
}
