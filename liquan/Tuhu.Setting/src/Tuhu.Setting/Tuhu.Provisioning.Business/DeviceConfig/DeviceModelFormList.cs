using System.Collections.Generic;

namespace Tuhu.Provisioning.Business.DeviceConfig
{ 
    /// <summary>
    /// 
    /// </summary>
    public class DeviceModelFormList
    { 
        ///// <summary>
        ///// 机型号主键
        ///// </summary>
        //public int PKID { get; set; }

         
        /// <summary>
        /// 机型号
        /// </summary>
        public string DeviceModel { get; set; }

        
        public List<ModelItem> Models { get; set; }


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

    public class ModelItem
    {
       public  int ModelId { get; set; }

       public string ModelName { get; set; }
    }
}