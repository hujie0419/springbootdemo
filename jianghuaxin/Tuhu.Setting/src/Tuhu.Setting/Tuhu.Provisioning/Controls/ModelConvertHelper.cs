using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Service.Shop.Models;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controls
{
    public class ModelConvertHelper
    {
        /// <summary>
        /// shopSimDetailModel中的部分字段映射到ShopSortModel
        /// </summary>
        /// <param name="shopSimDetailModel"></param>
        /// <param name="serviceType"></param>
        /// <param>CommentTimes评价数量，不传serviceType时计算总数量</param>
        /// <returns></returns>
        public static ShopSortModel ConvertToShopSortModel(ShopSimpleDetailModel shopSimDetailModel, string serviceType)
        {
            if (shopSimDetailModel == null)
            {
                return null;
            }
            var shopClassification = (string.IsNullOrWhiteSpace(shopSimDetailModel.ShopClassification)
                ? string.Empty
                : shopSimDetailModel.ShopClassification.Substring(1, shopSimDetailModel.ShopClassification.Length - 1));
            return (new ShopSortModel
            {
                Brand = shopSimDetailModel.Brand,
                ShopId = shopSimDetailModel.ShopId,
                ServiceType = serviceType,
                Address = serviceType == "MR" ? shopSimDetailModel.AddressBrief : shopSimDetailModel.Address,
                CarparName = shopSimDetailModel.CarparName,
                ShopType = shopSimDetailModel.ShopType,
                Province = shopSimDetailModel.Province,
                City = shopSimDetailModel.City,
                District = shopSimDetailModel.District,
                IsTop = shopSimDetailModel.AdditionalInfo?.IsTop ?? false,
                ShopRang = shopSimDetailModel.AdditionalInfo?.ShopRang ?? 0,
                ShopRecommendRange = shopSimDetailModel.AdditionalInfo?.ShopRecommendRange ?? 0,
                ShopClassification = shopClassification == "修理厂" ? "维修厂" : shopClassification,
                Images =
                    shopSimDetailModel.Images?.Select(image => ImageHelper.GetShopImage(image, 350, 350)).ToArray() ??
                    new string[0],
                ShopLevel = string.Equals(serviceType, "TR")
                    ? shopSimDetailModel.ShopLevel?.TireLevel ?? 0
                    : (string.Equals(serviceType, "BY") ? shopSimDetailModel.ShopLevel?.BaoYangLevel ?? 0 : 0),
                ShopLat = Convert.ToDouble(shopSimDetailModel.Position[0]),
                ShopLng = Convert.ToDouble(shopSimDetailModel.Position[1]),
                InstallQuantity = string.IsNullOrWhiteSpace(serviceType)
                    ? (shopSimDetailModel.Statisticses?.Sum(s => s.InstallQuantity) ?? 0)
                    : (shopSimDetailModel.Statisticses?.FirstOrDefault(statistics =>
                               string.Equals(statistics.Type, serviceType.ToString()))
                           ?.InstallQuantity ?? 0),
                CommentRate = string.IsNullOrWhiteSpace(serviceType)
                    ? 0
                    : (shopSimDetailModel.Statisticses?.FirstOrDefault(statistics =>
                               string.Equals(statistics.Type, serviceType.ToString()))
                           ?.CommentRate ?? 0),
                CommentTimes = string.IsNullOrWhiteSpace(serviceType)
                    ? (shopSimDetailModel.Statisticses?.Sum(c => c.CommentTimes) ?? 0)
                    : (shopSimDetailModel.Statisticses?.FirstOrDefault(statistics =>
                               string.Equals(statistics.Type, serviceType.ToString()))
                           ?.CommentTimes ?? 0),
                Grade = string.IsNullOrWhiteSpace(serviceType)
                    ? 0
                    : (shopSimDetailModel.Grades?.FirstOrDefault(grade =>
                               string.Equals(grade.Type, serviceType.ToString()))
                           ?.Grade ?? 0),
                DistanceWeight = string.IsNullOrWhiteSpace(serviceType)
                    ? 0
                    : (shopSimDetailModel.Grades?.FirstOrDefault(grade =>
                               string.Equals(grade.Type, serviceType.ToString()))
                           ?.Weight ?? 0),
                CoverRegion = shopSimDetailModel.CoverRegion,
                IsSuspend = !(shopSimDetailModel.SuspendStartDateTime == null || shopSimDetailModel.SuspendEndDateTime == null || 
                (DateTime.Now < shopSimDetailModel.SuspendStartDateTime) || DateTime.Now > shopSimDetailModel.SuspendEndDateTime.Value.AddDays(1))
        });
        }
    }
}