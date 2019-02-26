using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CityAgingModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int PKid { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 是否展示时效性
        /// </summary>
        public int IsShow { get; set; }
        /// <summary>
        /// 通告标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 通告栏
        /// </summary>
        public string Content { get; set; }
       
      
        /// <summary>
        /// 
        /// </summary>
        public string CreateUser { get; set; }
       
      
        /// <summary>
        /// 
        /// </summary>
        public string UpdateUser { get; set; }

    }


    public class CityAreaAgingModel
    {
        public int PKid { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityId { get; set; }

        public int ProvinceId { get; set; }

        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 是否展示时效性
        /// </summary>
        public int IsShow { get; set; }

       

        /// <summary>
        /// 通告标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 通告栏
        /// </summary>
        public string Content { get; set; }

       

    }


   


}
