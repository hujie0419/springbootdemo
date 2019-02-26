using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.DeviceConfig
{

    /// <summary>
    /// 厂商信息
    /// </summary>
    public class DeviceBrandEntity
    {

        public int PKID { get; set; }


        public string DeviceBrand { get; set; }



        public DateTime CreateDateTime { get; set; }



        public DateTime UpdateDateTime { get; set; }



        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }

    }
}
