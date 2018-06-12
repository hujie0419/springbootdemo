using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.CarInsurance
{
    public class CarInsuranceManage
    {
        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(CarInsuranceManage));

        private static IConnectionManager connectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static IDBScopeManager dbScopeManager = new DBScopeManager(connectionManager);
        private static IConnectionManager readConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private static IDBScopeManager readDbScopeManager = new DBScopeManager(readConnectionManager);

        #region private
        private static CarInsuranceRegion GetRegionInfo(int regionId)
        {
            using (var client = new Service.Shop.RegionClient())
            {
                var result = client.GetRegionByRegionId(regionId);
                if (!result.Success || result.Result == null)
                {
                    result.ThrowIfException(true);
                }
                var region = new CarInsuranceRegion();
                region.ProvinceId = result.Result.ProvinceId;
                region.ProvinceName = result.Result.ProvinceName;
                region.CityId = result.Result.CityId == 0 ? region.ProvinceId : result.Result.CityId;
                region.CityName = result.Result.CityName == null ? result.Result.ProvinceName : result.Result.CityName;
                return region;
            }
        }

        #endregion

        #region banner

        public static bool UpdateBannerIndex(string bannerIds)
        {
            var index = 0;
            bannerIds = bannerIds.Remove(bannerIds.Count() - 1, 1);
            string[] biArray = bannerIds.Split(',');
            foreach (var bi in biArray)
            {
                if (!dbScopeManager.Execute(conn => DalCarInsuranceConfig.UpdateBannerIndex(conn, int.Parse(bi), ++index)))
                    return false;
            }
            return true;
        }

        public static bool UpdateBanner(int bannerId, string name, string img, string linkUrl, string displayPage)
        {
            var result = dbScopeManager.Execute(conn => DalCarInsuranceConfig.UpdateBanner(conn, bannerId, name, img, linkUrl, displayPage));
            return result;
        }

        public static bool UpdateBannerIsEnable(int bannerId, int isEnable)
        {
            var result = dbScopeManager.Execute(conn => DalCarInsuranceConfig.UpdateBannerIsEnable(conn, bannerId, isEnable));
            return result;
        }

        public static bool CreateBanner(string name, string img, string linkUrl, string displayPage, int isEnable)
        {
            var maxIndex = dbScopeManager.Execute(conn => DalCarInsuranceConfig.GetMaxBannerIndex(conn));
            var result = dbScopeManager.Execute(conn => DalCarInsuranceConfig.CreateBanner(conn, name, img, linkUrl, maxIndex + 1, displayPage, isEnable));
            return result;
        }

        public static bool DeleteBanner(int bannerId)
        {
            var result = dbScopeManager.Execute(conn => DalCarInsuranceConfig.DeleteBanner(conn, bannerId));
            return result;
        }

        #endregion

        #region insurancePartner

        public static bool UpdateInsuranceIndex(List<int> insuranceIds)
        {
            bool success = false;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    var index = 0;
                    foreach (var insuranceId in insuranceIds)
                    {
                        DalCarInsuranceConfig.UpdateInsuranceIndex(conn, insuranceId, ++index);
                    }
                    success = true;
                });
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateInsuranceIndex", ex);
            }
            return success;
        }

        public static bool UpdateInsurance(int insuracePartnerId, string name, string img, string linkUrl, 
            string insuranceId, string markText, string title, string subTitle, string tagText, 
            string tagColor, int displayIndex, int isEnable, string regionCode, string providerCode)
        {
            CarInsurancePartner insurance = new CarInsurancePartner();
            insurance.PKID = insuracePartnerId;
            insurance.Name = name;
            insurance.Img = img;
            insurance.LinkUrl = linkUrl;
            insurance.InsuranceId = insuranceId;
            insurance.Remarks = markText;
            insurance.Title = title;
            insurance.SubTitle = subTitle;
            insurance.TagText = tagText;
            insurance.TagColor = tagColor;
            insurance.DisplayIndex = displayIndex;
            insurance.IsEnable = isEnable;
            insurance.RegionCode = regionCode;
            insurance.ProviderCode = providerCode;
            var result = dbScopeManager.Execute(conn => DalCarInsuranceConfig.UpdateInsurance(conn, insurance));
            return result;
        }

        public static bool UpdateInsuranceRegions(int insurancePartnerId, string regionIds)
        {
            dbScopeManager.Execute(conn => DalCarInsuranceConfig.DeleteRegionByInsurancePartnerId(conn, insurancePartnerId));

            string[] regionIdList = regionIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var regionId in regionIdList)
            {
                var region = GetRegionInfo(int.Parse(regionId));
                region.InsurancePartnerId = insurancePartnerId;

                dbScopeManager.Execute(conn => DalCarInsuranceConfig.CreateRegion(conn, region));
            }
            return true;
        }
            

        public static bool UpdateInsuranceIsEnable(int insurancePartnerId, int isEnable)
        {
            var result = dbScopeManager.Execute(conn => DalCarInsuranceConfig.UpdateInsuranceIsEnable(conn, insurancePartnerId, isEnable));
            return result;
        }

        public static bool CreateInsurance(string name, string img, string linkUrl, string insuranceId, 
            string markText, string title, string subTitle, string tagText, string tagColor, 
            int displayIndex, int isEnable, string regions, string regionCode, string providerCode)
        {
            if (displayIndex == -1)
                displayIndex = readDbScopeManager.Execute(conn => DalCarInsuranceConfig.GetMaxInsuranceIndex(conn)) + 1;
            CarInsurancePartner insurance = new CarInsurancePartner();
            insurance.Name = name;
            insurance.Img = img;
            insurance.LinkUrl = linkUrl;
            insurance.InsuranceId = insuranceId;
            insurance.Remarks = markText;
            insurance.Title = title;
            insurance.SubTitle = subTitle;
            insurance.TagText = tagText;
            insurance.TagColor = tagColor;
            insurance.IsEnable = isEnable;
            insurance.DisplayIndex = displayIndex - 1;
            insurance.RegionCode = regionCode;
            insurance.ProviderCode = providerCode;
            var pkid = dbScopeManager.Execute(conn => DalCarInsuranceConfig.CreateInsurance(conn, insurance));
            if (pkid == -1)
                return false;
            if (regions == "")
                return true;
            var result = UpdateInsuranceRegions(pkid, regions);
            return result;
        }

        public static bool DeleteInsurance(int insurancePartnerId)
        {
            var success = false;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    DalCarInsuranceConfig.DeleteInsurance(conn, insurancePartnerId);
                    DalCarInsuranceConfig.DeleteRegionByInsurancePartnerId(conn, insurancePartnerId);
                });
                success = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return success;
        }

        #endregion

        #region select
        public static List<CarInsuranceBanner> SelectBanners()
        {
            var result = readDbScopeManager.Execute(conn => DalCarInsuranceConfig.SelectBanners(conn));
            return result;
        }

        public static List<CarInsurancePartner> SelectInsurance()
        {
            var result = readDbScopeManager.Execute(conn => DalCarInsuranceConfig.SelectInsurance(conn));
            return result;
        }

        public static CarInsuranceBanner SelectBannerById(int bannerId)
        {
            var result = readDbScopeManager.Execute(conn => DalCarInsuranceConfig.SelectBannerById(conn, bannerId));
            return result;
        }

        public static CarInsurancePartner SelectInsurancePartnerById(int insurancePartnerId)
        {
            var result = readDbScopeManager.Execute(conn => DalCarInsuranceConfig.SelectInsuranceById(conn, insurancePartnerId));
            result.Regions = GetInsurancePartnerRegionIds(insurancePartnerId);
            return result;
        }

        public static List<CarInsuranceRegion> GetRegionByInsurancePartner(int insurancePartnerId)
        {
            var result = readDbScopeManager.Execute(conn => DalCarInsuranceConfig.GetRegionByInsurancePartnerId(conn, insurancePartnerId));
            return result;
        }

        public static string GetInsurancePartnerRegionIds(int insurancePartnerId)
        {
            var insurancePartnerIds = "";
            var regionList = readDbScopeManager.Execute(conn => DalCarInsuranceConfig.GetRegionByInsurancePartnerId(conn, insurancePartnerId));
            foreach (var region in regionList)
            {
               
                    insurancePartnerIds += region.ProvinceId + "-" + region.CityId + ",";
            }
            return insurancePartnerIds;
        }
        #endregion

        #region
        public static bool UpdateFAQ(string FAQ)
        {
            var result = dbScopeManager.Execute(conn => DalCarInsuranceConfig.UpdateFAQ(conn, FAQ));
            if(!result)
            {
                result = dbScopeManager.Execute(conn => DalCarInsuranceConfig.CreateFAQ(conn, FAQ));
            }
            return result;

        }

        public static string SelectFAQ()
        {
            var result = readDbScopeManager.Execute(conn => DalCarInsuranceConfig.SelectFAQ(conn));
            return result;
        }
        #endregion

        public static List<int> GetRegionIds()
        {
            var result = null as List<int>;
            try
            {
                result = readDbScopeManager.Execute(conn => DalCarInsuranceConfig.SelectRegionIds(conn));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result ?? new List<int>();
        }

        public static bool UpdateBannerCache()
        {
            using (var client = new Tuhu.Service.Config.CacheClient())
            {
                var serviceResult = client.UpdateCarInsuranceBannerCache();
                return serviceResult.Result;
            }
        }

        public static bool UpdateFooterCache()
        {
            using (var client = new Service.Config.CacheClient())
            {
                var serviceResult = client.UpdateCarInsuranceFooterCache();

                return serviceResult.Result;
            }
        }

        public static bool UpdatePartnerCache(IEnumerable<int> regionIds)
        {
            if (regionIds == null || !regionIds.Any())
            {
                return true;
            }
            regionIds = regionIds.Distinct();
            var success = true;
            using (var client = new Service.Config.CacheClient())
            {
                foreach (var regionId in regionIds)
                {
                    var serviceResult = client.UpdateCarInsurancePartnerCache(regionId);
                    success = serviceResult.Result && success;
                }
            }
            return success;
        }
    }
}
