

using System;

namespace Tuhu.Provisioning.DataAccess.Entity.DeviceConfig
{

    /// <summary>
    /// 机型号信息
    /// </summary>
    public class DeviceModelInfo
    {
        

        public int ModelID { get; set; }

        /// <summary>
        /// 机型号
        /// </summary>
        public string DeviceModel { get; set; }

        /// <summary>
        /// 机型
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// 机型信息
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 厂商编号
        /// </summary>
        public int BrandID { get; set; }

        /// <summary>
        /// 厂商信息
        /// </summary>
        public string DeviceBrand { get; set; }


        public DateTime CreateDateTime { get; set; }

        public string CreateUser { get; set; }


        public DateTime UpdateDateTime { get; set; }

        public string UpdateUser { get; set; }

    }
}
