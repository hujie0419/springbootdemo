using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.DeviceConfig
{
    /// <summary>
    /// 机型信息 
    /// </summary>
    public class DeviceTypeEntity
    {
        public int PKID { get; set; }


        public int BrandId { get; set; } 



        public string DeviceType { get; set; }



        public DateTime CreateDateTime { get; set; }



        public DateTime UpdateDateTime { get; set; }


        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }

    }
}
