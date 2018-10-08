using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity.CommonServices;
using Tuhu.Provisioning.DataAccess.Entity.RegionMarketing;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Provisioning.Business.RegionMarketing
{
    public class RegionMarketingManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly ILog logger = LoggerFactory.GetLogger("RegionMarketingManager");
        private static string LogType = "RegionMarketing";

        public RegionMarketingManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }

        public List<RegionMarketingModel> GetRegionMarketingConfig(Guid? activityId, string activityName, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            List<RegionMarketingModel> result = new List<RegionMarketingModel>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALRegionMarketing.SelectRegionMarketingConfig(conn, activityId, activityName, startTime, endTime, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetRegionMarketingConfig");
            }

            return result;
        }

        public RegionMarketingModel GetRegionActivityConfigByActivityId(Guid activityId)
        {
            RegionMarketingModel result = null;
            FlashSaleModel flashSaleInfo = null;
            List<DataAccess.Entity.RegionMarketing.SimpleTireProductInfo> tireInfo = new List<DataAccess.Entity.RegionMarketing.SimpleTireProductInfo>();
            try
            {
                if (activityId != Guid.Empty)
                {
                    dbScopeReadManager.Execute(conn =>
                    {
                        result = DALRegionMarketing.SelectRegionConfigByActivityId(conn, activityId);
                        if (result != null)
                        {
                            result.ImgList = DALRegionMarketing.SelectActivityImgById(conn, result.ActivityId);
                            result.ProductList = DALRegionMarketing.SelectRegionProductsByActivityId(conn, result.ActivityId);
                            flashSaleInfo = SelectFlashSaleDataByActivityID(activityId);
                            if (flashSaleInfo != null && flashSaleInfo.Products != null && flashSaleInfo.Products.Any())
                            {
                                var products = flashSaleInfo.Products;
                                var pidStr = string.Join(",", products.Select(x => x.PID));
                                tireInfo = DALRegionMarketing.SelectProductInfoByPID(conn, pidStr);
                            }
                        }
                    });
                    if (result != null)
                    {
                        if (flashSaleInfo != null && flashSaleInfo.Products != null && flashSaleInfo.Products.Any())
                        {
                            result.ProductList = (from fs in flashSaleInfo.Products
                                                  join pl in result.ProductList
                                                  on fs.PID equals pl.ProductId into temp
                                                  from t in temp.DefaultIfEmpty()
                                                  join tp in tireInfo on fs.PID equals tp.PID into infoData
                                                  from o in infoData.DefaultIfEmpty()
                                                  select new RegionMarketingProductConfig()
                                                  {
                                                      ActivityId = fs.ActivityID,
                                                      ProductId = fs.PID,
                                                      ProductName = fs.ProductName,
                                                      Price = fs.Price,
                                                      Size = o != null ? o.CP_Tire_Rim : string.Empty,
                                                      Specification = o != null && !string.IsNullOrEmpty(o.CP_Tire_Width)
                                                      ? (o.CP_Tire_Width + "/" + (!string.IsNullOrEmpty(o.CP_Tire_AspectRatio)
                                                      ? o.CP_Tire_AspectRatio : string.Empty)) : string.Empty,
                                                      MaxQuantity = fs.MaxQuantity,
                                                      TotalQuantity = fs.TotalQuantity,
                                                      AdvertiseTitle = t != null && !string.IsNullOrEmpty(t.AdvertiseTitle) ? t.AdvertiseTitle : fs.AdvertiseTitle,
                                                      IsShow = t != null && !string.IsNullOrEmpty(t.AdvertiseTitle),
                                                      SpecialCondition = t != null ? t.SpecialCondition : 0
                                                  }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetRegionMarketingConfig");
            }

            return result;
        }

        public bool SaveRegionarketingConfig(RegionMarketingModel data, string user)
        {
            var result = false;
            var flag = false;
            string msg = string.Empty;
            var isAdd = false;
            var startTime = data.StartTime.GetValueOrDefault();
            var endTime = data.EndTime.GetValueOrDefault();
            try
            {
                if (data != null)
                {
                    data.StartTime = data.EndTime = null;
                    dbScopeManager.CreateTransaction(conn =>
                    {
                        var info = DALRegionMarketing.SelectRegionMarketingConfig(conn, data.ActivityId, string.Empty, null, null, 1, 10);
                        if (info == null || info.Count == 0)
                        {
                            isAdd = true;
                            msg = "添加活动配置";
                            flag = DALRegionMarketing.InsertRegionMarketingConfig(conn, data) > 0;

                        }
                        else
                        {
                            msg = "修改活动配置";
                            flag = DALRegionMarketing.UpdateRegionMarketingConfig(conn, data) > 0;
                        }
                        if (flag)
                        {
                            foreach (var item in data.ImgList)
                            {
                                var imgInfo = DALRegionMarketing.SelectActivityImgByIdAndType(conn, item.ActivityId, item.Type);
                                if (imgInfo != null)
                                {
                                    result = DALRegionMarketing.UpdateActivityImgByIdAnType(conn, item.ActivityId, item.ImgUrl, item.Type, item.Position) > 0;
                                }
                                else
                                {
                                    result = DALRegionMarketing.InsertActivityImg(conn, item.ActivityId, item.ImgUrl, item.Type, item.Position) > 0;
                                }
                            }
                            foreach (var item in data.ProductList)
                            {
                                var proInfo = DALRegionMarketing.SelectRegionProductsByPID(conn, item.ActivityId, item.ProductId);
                                if (proInfo != null)
                                {
                                    result = DALRegionMarketing.UpdateRegionProductInfo(conn, item.ActivityId, item.ProductId, item.AdvertiseTitle, item.SpecialCondition) > 0;
                                }
                                else
                                {
                                    result = DALRegionMarketing.InsertRegionProductInfo(conn, item.ActivityId, item.ProductId, item.AdvertiseTitle, item.SpecialCondition) > 0;
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Log(Level.Error, ex, "SaveRegionarketingConfig");
            }

            if (result)
            {
                data.ProductList = new List<RegionMarketingProductConfig>();
                data.ImgList = new List<DataAccess.Entity.RegionMarketing.ActivityImageConfig>();
                InsertLog("RegionMarketingLog", data.ActivityId, JsonConvert.SerializeObject(data), msg, user, LogType);
                if (isAdd)
                    CallCRMService.NewAddActivity(data.ActivityName, startTime, endTime, "", data.ActivityRules, data.ActivityId.ToString(), CRMSourceType.TiresActivityOne, user);
                else
                    CallCRMService.NewUpdateActivity(data.ActivityName, startTime, endTime, "", data.ActivityRules, data.ActivityId.ToString(), CRMSourceType.TiresActivityOne, user);
            }

            return result;
        }

        public bool DeleteRegionMarketingConfig(Guid activityId, string user)
        {
            var result = false;

            try
            {
                result = dbScopeManager.Execute(conn => DALRegionMarketing.DeleteRegionMarketingConfig(conn, activityId)) > 0;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "");
            }

            if (result)
            {
                InsertLog("DeleteRegionMarketingConfig", activityId, "", "删除活动", user, LogType);
                CallCRMService.NewDeleteActivityBySourceId(activityId.ToString(), CRMSourceType.TiresActivityOne, user);
            }

            return result;
        }

        #region 日志
        public static void InsertLog(string method, Guid activityId, string remarks, string msg, string opera, string type)
        {
            try
            {
                RegionMarketingLog info = new RegionMarketingLog
                {
                    ActivityId = activityId,
                    Method = method,
                    Message = msg,
                    Remarks = remarks,
                    Operator = opera,
                    Type = type.Trim(),
                    CreatedTime = DateTime.Now
                };
                LoggerManager.InsertLog("RegionMarketingLog", info);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "ConfigLog");
            }
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<RegionMarketingLog> SelectOperationLog(Guid objectId, string type)
        {
            List<RegionMarketingLog> result = new List<RegionMarketingLog>();
            try
            {
                result = DALRegionMarketing.SelectOperationLog(objectId, type);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "ConfigLog");
            }
            return result;
        }
        #endregion

        #region  限时抢购服务

        public FlashSaleModel SelectFlashSaleDataByActivityID(Guid activityId)
        {
            FlashSaleModel result = null;
            try
            {
                using (var client = new FlashSaleClient())
                {
                    var data = client.SelectFlashSaleDataByActivityID(activityId);
                    if (!data.Success && data.Exception != null)
                        throw data.Exception;
                    result = data.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectFlashSaleDataByActivityID");
            }

            return result;
        }

        public List<FlashSaleModel> GetFlashSaleList(Guid[] activityIds)
        {
            List<FlashSaleModel> result = null;
            try
            {
                using (var client = new FlashSaleClient())
                {
                    var data = client.GetFlashSaleList(activityIds);
                    if (!data.Success && data.Exception != null)
                        throw data.Exception;
                    result = data.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetFlashSaleList");
            }

            return result;
        }

        #endregion
    }
}
