using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 喷漆打折
    /// </summary>
    public class PaintDiscountConfigModel
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 服务Pid
        /// </summary>
        public string ServicePid { get; set; }
        [JsonIgnore]
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 面数
        /// </summary>
        public int SurfaceCount { get; set; }
        /// <summary>
        /// 活动价格
        /// </summary>
        public decimal ActivityPrice { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动说明
        /// </summary>
        public string ActivityExplain { get; set; }
        /// <summary>
        /// 活动图片
        /// </summary>
        public string ActivityImage { get; set; }
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
        /// <summary>
        /// 价格体系Id
        /// </summary>
        public int PackageId { get; set; }
    }

    /// <summary>
    /// 喷漆服务
    /// </summary>
    public class PaintDiscountServiceModel
    {
        /// <summary>
        /// 服务Pid
        /// </summary>
        public string ServicePid { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
    }

    /// <summary>
    /// 年检操作日志表
    /// </summary>
    public class PaintDiscountOprLogModel
    {
        /// <summary>
        /// 日志表PKID
        /// </summary>
        public string PKID { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public string LogType { get; set; }
        /// <summary>
        /// 唯一识别标识
        /// </summary>
        public string IdentityId { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType { get; set; }
        /// <summary>
        /// 操作前值
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// 操作后值
        /// </summary>
        public string NewValue { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        [JsonIgnore]
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
    }

    /// <summary>
    /// 喷漆打折价格体系配置
    /// </summary>
    public class PaintDiscountPackageModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 体系名称
        /// </summary>
        public string PackageName { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType { get; set; }
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
    /// 喷漆价格体系展示
    /// </summary>
    public class PaintDiscountPackageViewModel : PaintDiscountPackageModel
    {
        /// <summary>
        /// 城市门店配置
        /// </summary>
        public List<PaintDiscountPackageRegionViewModel> RegionShops { get; set; }
    }

    /// <summary>
    /// 喷漆打折城市配置Db
    /// </summary>
    public class PaintDiscountPackageRegionModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 价格体系Id
        /// </summary>
        public int PackageId { get; set; }
        /// <summary>
        /// 地区Id
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        public int? ShopId { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
        /// <summary>
        /// 最近更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }

    /// <summary>
    /// 喷漆打折分城市配置展示
    /// </summary>
    public class PaintDiscountPackageRegionViewModel
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
        /// 地区Id
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 门店
        /// </summary>
        public List<PaintDiscoutPackageShop> Shops { get; set; }
    }

    public class PaintDiscoutPackageShop
    {
        /// <summary>
        /// 门店Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 城市Id
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string ShopName { get; set; }
    }

    /// <summary>
    /// 用户类型
    /// </summary>
    public enum  UserType
    {
        /// <summary>
        /// 新用户
        /// </summary>
        NewUser=1,
        /// <summary>
        /// 老用户
        /// </summary>
        OldUser=2,
        /// <summary>
        /// 全部用户
        /// </summary>
        AllUser=3
    }

    /// <summary>
    /// 城市对应门店
    /// </summary>
    public class RegionShopPairModel
    {
        /// <summary>
        /// 地区Id
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        public List<int> ShopIds { get; set; }
    }
}
