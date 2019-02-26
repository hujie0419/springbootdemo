using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{

    /// <summary>
    /// 覆盖区域Model
    /// </summary>
    public class VendorProductCoverAreaModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        /// 产品类型 蓄电池Battery/玻璃Glass
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 覆盖类型  品牌/Pid
        /// </summary>
        public string CoverType { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Pid
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 覆盖区域
        /// </summary>
        public int CoverRegionId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }

    /// <summary>
    /// 覆盖区域展示Model
    /// </summary>
    public class VendorProductCoverAreaViewModel : VendorProductCoverAreaModel
    {
        /// <summary>
        /// 省份Id
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市Id
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 地区Id
        /// </summary>
        public int DistrictId { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        public string DistrictName { get; set; }
    }

}
