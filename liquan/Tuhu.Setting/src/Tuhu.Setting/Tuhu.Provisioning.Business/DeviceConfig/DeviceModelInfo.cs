

namespace Tuhu.Provisioning.Business.DeviceConfig
{

    /// <summary>
    /// 机型号信息
    /// </summary>
    public class DeviceModelInfo
    {
        /// <summary>
        /// 机型号主键
        /// </summary>
        public int PKID { get; set; }

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

    }
}
