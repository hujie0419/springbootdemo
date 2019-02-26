using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BankMRActivityDisplayRegionEntity
    {
        public int PKID { get; set; }
        /// <summary>
        /// 银行活动展示配置id
        /// </summary>
        public int DisplayConfigId { get; set; }
        /// <summary>
        /// 省id
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 是否所有城市
        /// </summary>
        public int IsAllCitys { get; set; }
        /// <summary>
        /// 二级城市id
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 三级区域id
        /// </summary>
        public int DistrictIds { get; set; }
        /// <summary>
        /// 是否当前城市下所有区域
        /// </summary>
        public int IsAllDistrict { get; set; }

    }
}
