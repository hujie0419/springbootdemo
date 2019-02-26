using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.GeneralBeautyServerCode;
using Tuhu.Provisioning.Business.VipBaoYangPackage;
using Tuhu.Provisioning.DataAccess.DAO.RegionPackageMap;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Tuhu.Service.Shop.Models.Enum;

namespace Tuhu.Provisioning.Business.RegionPackageMap
{
    public class RegionPackageMapManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["ThirdParty"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["ThirdPartyReadOnly"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(RegionPackageMapManager));
        private static string LogType = "PingAnRegionPackageMap";

        public RegionPackageMapManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }
        /// <summary>
        /// 获取地区和包的对应关系
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="byPackagePID"></param>
        /// <param name="businessId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<PingAnRegionPackageMap> GetPingAnRegionPackageMapList(Guid? packageId, string byPackagePID, string businessId, int pageIndex, int pageSize)
        {
            List<PingAnRegionPackageMap> result = null;
            try
            {
                result = dbScopeReadManager.Execute(conn => DALRegionPackageMap.GetPingAnRegionPackageMapList(conn, packageId, byPackagePID, businessId, pageIndex, pageSize));
                if (result != null && result.Any())
                {
                    result.ForEach(x =>
                    {
                        var region = ShopService.GetRegionByRegionId(x.RegionId);
                        if (region != null && region.RegionId > 0)
                        {
                            if (region.IsBelongMunicipality)
                            {
                                if (region.RegionType == RegionType.District)
                                {
                                    x.ProvinceId = region.ProvinceId;
                                    x.ProvinceName = region.ProvinceName;
                                    x.CityId = region.RegionId;
                                    x.CityName = region.RegionName;
                                    x.RegionName = region.ProvinceName + "-" + region.RegionName;
                                }
                                else
                                {
                                    x.ProvinceId = region.RegionId;
                                    x.ProvinceName = region.RegionName;
                                    x.RegionName = region.RegionName;
                                }
                            }
                            else
                            {
                                switch (region.RegionType)
                                {
                                    case RegionType.Province:
                                        x.ProvinceId = region.RegionId;
                                        x.ProvinceName = region.RegionName;
                                        x.RegionName = region.RegionName;
                                        break;
                                    case RegionType.City:
                                        x.ProvinceId = region.ProvinceId;
                                        x.ProvinceName = region.ProvinceName;
                                        x.CityId = region.RegionId;
                                        x.CityName = region.RegionName;
                                        x.RegionName = region.ProvinceName + "-" + region.CityName;
                                        break;
                                    case RegionType.District:
                                        x.ProvinceId = region.ProvinceId;
                                        x.ProvinceName = region.ProvinceName;
                                        x.CityId = region.RegionId;
                                        x.CityName = region.RegionName;
                                        x.RegionName = region.ProvinceName + "-" + region.CityName + "-" + region.DistrictName;
                                        break;
                                }
                            }
                        }

                        //if (!string.IsNullOrEmpty(x.BYPackagePID))
                        //{
                        //    x.BYPakageName = new VipBaoYangPackageManager().SelectVipBaoYangPackageByPkid(Convert.ToInt32(x.BYPackagePID.Split('|')[1]))?.PackageName;
                        //}
                        //else if (x.PackageId != null && x.PackageId != Guid.Empty)
                        //{
                        //    x.PackageName = GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeTmpDetail(x.PackageId.Value)?.PackageName;
                        //}
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// 新增对应关系
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Tuple<bool, string> UpsertPingAnRegionPackageMap(PingAnRegionPackageMap data, string user)
        {
            var result = false;
            var msg = "";
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    var allConfig = DALRegionPackageMap.GetAllPingAnRegionPackageMapList(conn);
                    var isExist = false;
                    if (!string.IsNullOrEmpty(data.BYPackagePID))
                    {
                        isExist = allConfig?.Where(_ => String.Equals(_.BYPackagePID, data.BYPackagePID))?.Count() > 0;
                    }
                    else if (data.PackageId != null && data.PackageId != Guid.Empty)
                    {
                        isExist = allConfig?.Where(_ => String.Equals(_.PackageId, data.PackageId))?.Count() > 0;
                    }
                    data.RegionId = data.CityId > 0 ? data.CityId : (data.ProvinceId > 0 ? data.ProvinceId : 0);
                    if (!isExist)
                    {
                        result = DALRegionPackageMap.InsertPingAnRegionPackageMap(conn, data);
                    }
                    else
                    {
                        msg = "该美容包或保养套餐已存在关联关系";
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return Tuple.Create(result, msg);
        }
        /// <summary>
        /// 删除对应关系
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeletePingAnRegionPackageMap(int pkid, string user)
        {
            var result = false;
            try
            {
                result = dbScopeManager.Execute(conn => DALRegionPackageMap.DeletePingAnRegionPackageMap(conn, pkid));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }
    }
}
