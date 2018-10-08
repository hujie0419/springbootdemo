using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShopCosmetologyServers
    {
        public int PKID { get; set; }

        public string ProductID { get; set; }

        public string ServersName { get; set; }

        public int CatogryID { get; set; }

    }

    public class ShopCosmetology
    {
        public int PKID { get; set; }

        public string ProductID { get; set; }

        public string ServersName { get; set; }

        public int CatogryID { get; set; }

        public object List { get; set; }
    }

    public class MeiRongAcitivityConfig
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? SignUpStartTime { get; set; }

        public DateTime? SignUpEndTime { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? ActivityStartTime { get; set; }
        public DateTime? ActivityEndTime { get; set; }

        public int CategoryId { get; set; }

        public string ActivityId { get; set; }

        public string Region { get; set; }

        public List<RegionRelation> RegionList { get; set; }

        public float MinPrice { get; set; }

        public float MaxPrice { get; set; }

        public int EverydayQuantity { get; set; }

        public int MinShopQuantity { get; set; }

        public int VehicleGrade { get; set; }

        public int ApplicationVehicle { get; set; }

        public int ShopType { get; set; }

        public int ShopGrade { get; set; }

        public int MeiRongAppraise { get; set; }

        public string ActivityRequire { get; set; }

        public string ActivityNotification { get; set; }

        public int Status { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string CreateName { get; set; }

        public string UpdateName { get; set; }

        public string StrProvince { get; set; }

        public string StrCity { get; set; }

        public string CategoryName { get; set; }

        public string ShowName { get; set; }

        public string ShopServices { get; set; }

    }

    public class RegionModel
    {
        public string ProvinceName { get; set; }

        public string CityName { get; set; }

        public int CityId { get; set; }

        public int ProvinceId { get; set; }
    }

    public class RegionRelation
    {
        public int Id { get; set; }

        public int ActivityId { get; set; }

        public string ProvinceName { get; set; }

        public string CityName { get; set; }

        public int CityId { get; set; }

        public int ProvinceId { get; set; }

        public short Type { get; set; }
    }

    public class ShopServiceRelation
    {
        public int Id { get; set; }

        public int ActivityId { get; set; }

        public int PKID { get; set; }

        public string ProductID { get; set; }

        public string ServersName { get; set; }

        public int CatogryID { get; set; }

        public short Type { get; set; }

    }

    public class BeautyHomePageConfig
    {
        public int Id { get; set; }

        public short Type { get; set; }

        public string StartVersion { get; set; }

        public string EndVersion { get; set; }

        public string Name { get; set; }

        public int Sort { get; set; }

        public string Title { get; set; }

        public string SmallTitle { get; set; }

        public string Channel { get; set; }

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string Link { get; set; }

        public string Icon { get; set; }

        public string Banner { get; set; }

        public bool Status { get; set; }
        public bool IsNotShow { get; set; }
        public bool IsRegion { get; set; }

        public short CarLevel { get; set; }

        public string ActivityId { get; set; }

        public string ActivityName { get; set; }

        public int ActivityPKID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string Region { get; set; }

        public List<RegionRelation> RegionList { get; set; }
        /// <summary>
        /// 美容项目列表Banner配置json
        /// </summary>
        public string BannerConfigs { get; set; }

        private List<BannerConfigsModel> _bannerConfigsList;
        /// <summary>
        /// 美容项目列表Banner配置
        /// </summary>
        public List<BannerConfigsModel> BannerConfigsList
        {
            get
            {
                if (!string.IsNullOrEmpty(BannerConfigs) && _bannerConfigsList == null)
                {
                    try
                    {
                        _bannerConfigsList = JsonConvert.DeserializeObject<List<BannerConfigsModel>>(BannerConfigs);
                    }
                    catch (Exception)
                    {
                        _bannerConfigsList = null;
                    }

                }
                return _bannerConfigsList;
            }
        }
    }
    public class BannerConfigsModel
    {
        /// <summary>
        /// 2=跳转地址；1=优惠券
        /// </summary>
        public int ConfigType { get; set; }
        /// <summary>
        /// 跳转地址
        /// </summary>
        public string JumpUrl { get; set; }
        /// <summary>
        /// 优惠券guid
        /// </summary>
        public Guid? PromotionGuid { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 展示顺序
        /// </summary>
        public int Sort { get; set; }

    }
    public class ShopNotificationRecord
    {
        public int Id { get; set; }

        public int ActivityId { get; set; }

        public int ShopId { get; set; }

        public string Notification { get; set; }

    }

    public class BeautyPopUpWindowsConfig
    {
        public int PKID { get; set; }
        public int PlaceType { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string StartVersion { get; set; }
        public string EndVersion { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string BackGroundImage { get; set; }
        public string BackGroundLink { get; set; }
        public string PromotionInfo { get; set; }
        /// <summary>
        /// 0=禁用，1=启用
        /// </summary>
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public bool IsRegion { get; set; }
        public string PopUpLogo { get; set; }

        public List<BeautyPopUpWindowsRegionModel> RegionList { get; set; }
    }
    public class PromotionInfoModel
    {
        public int Id { get; set; }
        public Guid PromotionGuid { get; set; }
        public string PromotionImage { get; set; }

    }
    public class BeautyCategorySimple
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class BeautyPopUpWindowsRegionModel
    {//PKID, , , , , CreateTime, UpdateTime
        public int BeautyPopUpId { get; set; }
        public int ProvinceId { get; set; }
        public string CityId { get; set; }
        public string[] CityIds
        {
            get
            {
                if (!string.IsNullOrEmpty(CityId))
                    return CityId.Split(',');
                return null;
            }
        }
        public bool IsAllCity { get; set; }
    }

}
