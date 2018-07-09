using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.DeviceConfig
{

    /// <summary>
    /// 机型号信息
    /// </summary>
    public class DeviceModelEntity
    {

        public int PKID { get; set; }


        public int TypeId { get; set; }



        public string DeviceModel { get; set; }



        public DateTime CreateDateTime { get; set; }



        public DateTime UpdateDateTime { get; set; }


        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }

    }
}
