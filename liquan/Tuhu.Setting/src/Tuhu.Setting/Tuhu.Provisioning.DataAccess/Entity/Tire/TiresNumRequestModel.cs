using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public class TiresNumRequestModel
    {
        //string addressPhone,int number, string userIp, string userPhone, string deviceId
        public string AddressPhone { set; get; }
        public int Number { set; get; }
        public string UserIp { set; get; }
        public string UserPhone { set; get; }
        public string DeviceId { set; get; }
    }
}
