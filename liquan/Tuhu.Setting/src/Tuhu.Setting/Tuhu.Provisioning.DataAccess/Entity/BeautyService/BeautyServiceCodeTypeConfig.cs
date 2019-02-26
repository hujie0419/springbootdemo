using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BeautyServiceCodeTypeConfig
    {
        public int PKID { get; set; }

        public string PID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public int AdapterVehicle { get; set; }
        public string AdapterVehicleName
        {
            get
            {
                string result = string.Empty;
                switch (AdapterVehicle)
                {
                    case 1:
                        result = "五座轿车";
                        break;
                    case 2:
                        result = "SUV/MPV";
                        break;
                    case 3:
                        result = "SUV";
                        break;
                    case 4:
                        result = "MPV";
                        break;
                    case 5:
                        result = "七座轿车";
                        break;
                    case 6:
                        result = "SUV MPV 五座轿车 七座轿车";
                        break;
                    default:
                        break;
                }
                return result;
            }
        }
        public string LogoUrl { get; set; }

        public bool IsActive { get; set; }
        /// <summary>
        /// 服务产品类型（0正式；1测试）
        /// </summary>
        public int ServerType { get; set; }
        public string ServerTypeString
        {
            get
            {
                string result = null;
                switch (ServerType)
                {
                    case 0:
                        result = "正式";
                        break;
                    case 1:
                        result = "测试";
                        break;
                    default:
                        result = "数据异常";
                        break;
                }
                return result;
            }
        }
    }
}
