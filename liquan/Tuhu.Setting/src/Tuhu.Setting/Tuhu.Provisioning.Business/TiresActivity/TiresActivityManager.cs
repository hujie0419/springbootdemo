using Common.Logging;
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
using Tuhu.Provisioning.DataAccess.DAO.TiresActivity;
using Tuhu.Provisioning.DataAccess.Entity.CommonServices;
using Tuhu.Provisioning.DataAccess.Entity.TiresActivity;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Config;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;

namespace Tuhu.Provisioning.Business.TiresActivity
{
    public class TiresActivityManager
    {
        private readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private readonly IConnectionManager ConnectionReadManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(TiresActivityManager));
        private static string LogType = "TiresActivity";

        public TiresActivityManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager); ;
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager); ;
        }

        #region 轮胎区域活动

        public List<TiresActivityModel> SelectTiresActivity(string activityName, DateTime? startTime, DateTime? endTime, Guid? activityId, int pageIndex, int pageSize)
        {
            var result = new List<TiresActivityModel>();

            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALTiresActivity.SelectTiresActivity(conn, activityName, startTime, endTime, activityId ?? Guid.Empty, pageIndex, pageSize);
                    if (result != null && result.Any())
                    {
                        result.ForEach(x => x.TiresFloorList = DALTiresActivity.SelectTiresFloorInfoByParentId(conn, x.ActivityId));
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public TiresFloorActivityConfig SelectTiresActivityByFloorId(Guid floorId)
        {
            var result = new TiresFloorActivityConfig();

            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALTiresActivity.SelectTiresFloorInfoByFloorId(conn, floorId);
                    if (result != null)
                    {
                        result.ImgList = DALTiresActivity.SelectActivityImgById(conn, result.FloorActivityId) ?? new List<DataAccess.Entity.TiresActivity.ActivityImageConfig>();
                        result.ProductList = DALTiresActivity.SelectRegionProductsByActivityId(conn, result.FloorActivityId) ?? new List<DataAccess.Entity.TiresActivity.TiresActivityProductConfig>();
                        var flashSaleInfo = SelectFlashSaleDataByActivityID(result.FlashSaleId);
                        if (flashSaleInfo != null && flashSaleInfo.Products != null && flashSaleInfo.Products.Any())
                        {
                            var products = flashSaleInfo.Products;
                            var pidStr = string.Join(",", products.Select(x => x.PID));
                            var tireInfo = DALTiresActivity.SelectProductInfoByPID(conn, pidStr);
                            result = ConvertDetails(result, flashSaleInfo, tireInfo);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public TiresFloorActivityConfig SelectTiresActivityByFlashId(Guid flashId, Guid parentId)
        {
            var result = new TiresFloorActivityConfig();

            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALTiresActivity.SelectTiresFloorInfoByFlashId(conn, flashId, parentId);
                    if (result != null)
                    {
                        var flashSaleInfo = SelectFlashSaleDataByActivityID(flashId);
                        if (flashSaleInfo != null && flashSaleInfo.Products != null && flashSaleInfo.Products.Any())
                        {
                            var products = flashSaleInfo.Products;
                            var pidStr = string.Join(",", products.Select(x => x.PID));
                            var tireInfo = DALTiresActivity.SelectProductInfoByPID(conn, pidStr);
                            result = ConvertDetails(result, flashSaleInfo, tireInfo);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool UpserTiresActivity(TiresActivityModel data, string user)
        {
            var result = false;
            var msg = string.Empty;
            var isAdd = false;
            try
            {
                dbScopeManager.Execute(conn =>
                {
                    if (data == null || data.ActivityId == Guid.Empty)
                    {
                        isAdd = true;
                        msg = "增加轮胎活动";
                        data.ActivityId = Guid.NewGuid();
                        result = DALTiresActivity.InsertTiresActivityConfig(conn, data, user) > 0;
                    }
                    else
                    {
                        msg = "修改轮胎活动";
                        result = DALTiresActivity.UpdateTiresActivityConfig(conn, data, user) > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            if (result)
            {
                InsertLog("UpserTiresActivity", data.ActivityId, JsonConvert.SerializeObject(data), msg, user, LogType);
                if (isAdd)
                    CallCRMService.NewAddActivity(data.ActivityName, data.StartTime.GetValueOrDefault(), data.EndTime.GetValueOrDefault(), "", data.ActivityRules, data.ActivityId.ToString(), CRMSourceType.TiresActivityTwo, user);
                else
                    CallCRMService.NewUpdateActivity(data.ActivityName, data.StartTime.GetValueOrDefault(), data.EndTime.GetValueOrDefault(), "", data.ActivityRules, data.ActivityId.ToString(), CRMSourceType.TiresActivityTwo, user);
            }

            return result;
        }

        public bool UpsertTiresFloorActivity(TiresFloorActivityConfig model, string user)
        {
            var result = false;
            string msg = string.Empty;

            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    if (model != null && model.TiresActivityId != Guid.Empty && model.FlashSaleId != Guid.Empty)
                    {
                        if (model.FloorActivityId == Guid.Empty)
                        {
                            msg = "添加楼层活动";
                            model.FloorActivityId = Guid.NewGuid();
                            DALTiresActivity.InsertTiresFloorActivity(conn, model);
                        }
                        else
                        {
                            msg = "修改楼层活动";
                            DALTiresActivity.UpdateTiresFloorActivity(conn, model);
                        }

                        foreach (var item in model.ImgList)
                        {
                            item.ActivityId = model.FloorActivityId;
                            var imgInfo = DALTiresActivity.SelectActivityImgByIdAndType(conn, item.ActivityId, item.Type);
                            if (imgInfo != null)
                            {
                                DALTiresActivity.UpdateActivityImgByIdAnType(conn, item.ActivityId, item.ImgUrl, item.Type, item.Position);
                            }
                            else
                            {
                                DALTiresActivity.InsertActivityImg(conn, item.ActivityId, item.ImgUrl, item.Type, item.Position);
                            }
                        }
                        foreach (var item in model.ProductList)
                        {
                            item.ActivityId = model.FloorActivityId;
                            var proInfo = DALTiresActivity.SelectRegionProductsByPID(conn, item.ActivityId, item.ProductId);
                            if (proInfo != null)
                            {
                                DALTiresActivity.UpdateTiresProductInfo(conn, item.ActivityId, item.ProductId, item.AdvertiseTitle, item.SpecialCondition, item.IsCancelProgressBar, item.Position);
                            }
                            else
                            {
                                DALTiresActivity.InsertTiresProductInfo(conn, item.ActivityId, item.ProductId, item.AdvertiseTitle, item.SpecialCondition, item.IsCancelProgressBar, item.Position);
                            }
                        }
                        result = true;
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error($"UpsertTiresFloorActivity Error:{ex.Message}; Stack:{ex.StackTrace}", ex);
            }

            if (result)
            {
                model.ProductList = new List<DataAccess.Entity.TiresActivity.TiresActivityProductConfig>();
                model.ImgList = new List<DataAccess.Entity.TiresActivity.ActivityImageConfig>();
                InsertLog("UpsertTiresFloorActivity", model.TiresActivityId, JsonConvert.SerializeObject(model), msg, user, LogType);
            }

            return result;
        }

        public bool DeleteTiresActivityConfig(Guid activityId, string user)
        {
            var result = false;

            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    DALTiresActivity.DeleteTiresActivityConfig(conn, activityId);
                    var floorActivityInfo = DALTiresActivity.SelectTiresFloorInfoByParentId(conn, activityId);
                    if (floorActivityInfo != null && floorActivityInfo.Any())
                    {
                        foreach (var item in floorActivityInfo)
                        {
                            DALTiresActivity.DeleteTiresFloorActivity(conn, item.FloorActivityId);
                        }
                    }
                    result = true;
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            if (result)
            {
                InsertLog("DeleteTiresActivityConfig", activityId, "删除轮胎活动", "删除轮胎活动", user, LogType);
                CallCRMService.NewDeleteActivityBySourceId(activityId.ToString(), CRMSourceType.TiresActivityTwo, user);
            }

            return result;
        }

        public bool DeleteTiresFloorActivity(Guid activityId, Guid floorId, string user)
        {
            var result = false;

            try
            {
                result = dbScopeManager.Execute(conn => DALTiresActivity.DeleteTiresFloorActivity(conn, floorId)) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            if (result)
            {
                InsertLog("DeleteTiresActivityConfig", activityId, "删除轮胎楼层活动", "删除轮胎楼层活动", user, LogType);
            }

            return result;
        }

        public IReadOnlyDictionary<string, string> GetAllImgMapping()
        {
            IReadOnlyDictionary<string, string> mapping = new Dictionary<string, string>
            {
                { "HeadImg","banner"},{"Img12","12寸"},{"Img13","13寸"},{"Img14","14寸"},{"Img15","15寸"},{"Img16","16寸"},{"Img17","17寸"},{ "Img18","18寸"},
                { "Img19","19寸"},{"Img20","20寸"},{"Img21","21寸"},{"Img22","22寸"},{"Img12C","12C"},{"Img13C","13C"},{"Img14C","14C"},{"Img15C","15C"},{"Img16C","16C"}
            };
            return mapping;
        }
        #endregion

        #region Convert
        private TiresFloorActivityConfig ConvertDetails(TiresFloorActivityConfig result, FlashSaleModel flashSaleInfo, List<DataAccess.Entity.TiresActivity.SimpleTireProductInfo> tireInfo)
        {

            try
            {
                result.ProductList = (from fs in flashSaleInfo.Products
                                      join pl in result.ProductList ?? new List<DataAccess.Entity.TiresActivity.TiresActivityProductConfig>()
                                      on fs.PID equals pl.ProductId into temp
                                      from t in temp.DefaultIfEmpty()
                                      join tp in tireInfo on fs.PID equals tp.PID into infoData
                                      from o in infoData.DefaultIfEmpty()
                                      select new DataAccess.Entity.TiresActivity.TiresActivityProductConfig()
                                      {
                                          ActivityId = fs.ActivityID,
                                          ProductId = fs.PID,
                                          ProductName = fs.ProductName,
                                          Position = fs.Position,
                                          Price = fs.Price,
                                          Size = o != null ? o.CP_Tire_Rim : string.Empty,
                                          Specification = o != null && !string.IsNullOrEmpty(o.CP_Tire_Width)
                                          ? (o.CP_Tire_Width + "/" + (!string.IsNullOrEmpty(o.CP_Tire_AspectRatio)
                                          ? o.CP_Tire_AspectRatio : string.Empty)) : string.Empty,
                                          MaxQuantity = fs.MaxQuantity,
                                          TotalQuantity = fs.TotalQuantity,
                                          AdvertiseTitle = t != null && !string.IsNullOrEmpty(t.AdvertiseTitle) ? t.AdvertiseTitle : fs.AdvertiseTitle,
                                          IsShow = t != null && !string.IsNullOrEmpty(t.AdvertiseTitle),
                                          SpecialCondition = t != null ? t.SpecialCondition : 0,
                                          IsCancelProgressBar = t != null ? t.IsCancelProgressBar : false
                                      }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        #endregion

        #region 日志
        public static void InsertLog(string method, Guid activityId, string remarks, string msg, string opera, string type)
        {
            try
            {
                var info = new
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
                logger.Error(ex);
            }
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
                logger.Error(ex);
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
                logger.Error(ex);
            }

            return result;
        }

        #endregion

        #region 刷新缓存

        public bool RefershAllRegionTiresActivityCache(Guid activityId)
        {
            var result = false;
            try
            {
                var miniRegion = GetAllMiniRegion();
                if (miniRegion != null && miniRegion.Any())
                {
                    foreach (var region in miniRegion)
                    {
                        foreach (var child in region.ChildRegions)
                        {
                            result = RefreshRegionTiresActivityCache(activityId, child.RegionId);
                        }
                        result = RefreshRegionTiresActivityCache(activityId, region.RegionId);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool RefreshRegionTiresActivityCache(Guid activityId, int regionId)
        {
            var result = false;

            try
            {
                using (var client = new ActivityClient())
                {
                    var getResult = client.RefreshRegionTiresActivityCache(activityId, regionId);
                    getResult.ThrowIfException(true);
                    if (!getResult.Result)
                        throw getResult.Exception;
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public List<MiniRegion> GetAllMiniRegion()
        {
            List<MiniRegion> result = new List<MiniRegion>();
            try
            {
                using (var client = new RegionClient())
                {
                    var data = client.GetAllMiniRegion();
                    data.ThrowIfException(true, "获取所有地区服务失败");
                    result = data.Result.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public List<ProductCityStock> GetProductRegionStock(int regionId, string pidStr)
        {
            List<ProductCityStock> result = new List<ProductCityStock>();
            try
            {
                using (var client = new ProductClient())
                {
                    var data = client.SelectProductsRegionStock(regionId, pidStr.Split(',').ToList());
                    data.ThrowIfException(true, "分地区库存接口异常");
                    result = data.Result.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
        #endregion
    }
}
