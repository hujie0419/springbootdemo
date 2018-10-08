using Tuhu.Component.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Component.Common;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using ProductModel = Tuhu.Provisioning.DataAccess.Entity.ProductModel;
using Tuhu.Service.Activity.Enum;

namespace Tuhu.Provisioning.Business
{
    public class QiangGouManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("QiangGouManager");
        /// <summary>
        /// 查询所有活动
        /// </summary>
        public static IEnumerable<QiangGouModel> SelectAllQiangGou()
        {
            return DALQiangGou.SelectAllQiangGou();
        }
        /// <summary>
        /// 查询所有活动
        /// </summary>
        public static IEnumerable<QiangGouProductModel> SelectQiangGouProductModels(Guid aid)
        {
            return DALQiangGou.SelectQiangGouProductModels(aid);
        }
        /// <summary>
        /// 根据活动查询活动明细
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public static QiangGouModel FetchQiangGouAndProducts(Guid aid)
        {
            var dt = DALQiangGou.FetchQiangGouAndProducts(aid);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            var model = dt.ConvertTo<QiangGouModel>()?.FirstOrDefault();
            model.Products = dt.ConvertTo<QiangGouProductModel>();
            return model;
        }

        /// <summary>
        /// 根据活动查询活动明细
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public static QiangGouModel FetchQiangGouAndProductsTemp(Guid aid)
        {
            var dt = DALQiangGou.FetchQiangGouAndProductsFromTemp(aid);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            var model = dt.ConvertTo<QiangGouModel>()?.FirstOrDefault();
            model.Products = dt.ConvertTo<QiangGouProductModel>();
            return model;
        }

        public static Tuple<int, Guid> Save(QiangGouModel model, List<QiangGouDiffModel> diffs)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                dbHelper.BeginTransaction();
                try
                {
                    var syncResult = SynchroDiffActivity(model, diffs, dbHelper);
                    if (syncResult < 0)
                        return new Tuple<int, Guid>(syncResult, model.ActivityID.Value);
                    var result = DALQiangGou.CreateOrUpdateQianggou(dbHelper, model);
                    dbHelper.Commit();
                    return result;
                }
                catch (Exception e)
                {
                    dbHelper.Rollback();
                    throw new Exception(e.Message); ;
                }
            }
            // return DALQiangGou.Save(model);
        }
        public static int SynchroDiffActivity(QiangGouModel qiang, List<QiangGouDiffModel> diffs, Tuhu.Component.Common.SqlDbHelper dbHelper)
        {
            var selectedDiffs = diffs.GroupBy(r => new { r.ActivityID, r.ActiveType }).Select(d => new QiangGouModel
            {
                ActivityID = d.Key.ActivityID,
                ActiveType = d.Key.ActiveType,
                Products = d.Select(product => new QiangGouProductModel
                {
                    PID = product.PID,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    FalseOriginalPrice = product.FalseOriginalPrice,
                    InstallAndPay = product.InstallAndPay,
                    IsUsePCode = product.IsUsePCode,
                })
            });
            List<QiangGouProductModel> list = new List<QiangGouProductModel>();
            //List<QiangGouProductModel> selectedlist = new List<QiangGouProductModel>();
            foreach (var a in selectedDiffs)
            {
                var selecteddiffs = DALQiangGou.SelectSelectedDiffActivityProducts(a.ActivityID, a.ActiveType, a.Products, dbHelper);
                list.AddRange(selecteddiffs);
            }
            var aids = list.Select(_ => _.ActivityID);
            foreach (var aid in aids)
            {
                var model = DALQiangGou.SelectQiangGouForSynchro(aid, dbHelper);
                var origin = DALQiangGou.SelectQiangGouForSynchro(aid, dbHelper);
                var needAsyncPids = list.Where(_ => _.ActivityID == aid).Select(_ => _.PID).ToList();
                var tempProducts = model.Products.Where(_ => !needAsyncPids.Contains(_.PID)).ToList();
                bool flag = true;
                foreach (var pid in needAsyncPids)
                {
                    var product = qiang.Products.FirstOrDefault(_ => _.PID == pid);
                    var y_product = model.Products.FirstOrDefault(_ => _.PID == pid);
                    y_product.ProductName = product.ProductName;
                    y_product.Price = product.Price;
                    y_product.FalseOriginalPrice = product.FalseOriginalPrice;
                    y_product.InstallAndPay = product.InstallAndPay;
                    y_product.IsUsePCode = product.IsUsePCode;

                    tempProducts.Add(y_product);
                    if (!string.IsNullOrWhiteSpace(qiang.NeedExamPids) && qiang.NeedExamPids.Split(';').Contains(pid) && flag)
                        flag = false;

                }

                model.NeedExam = !flag;
                model.Products = tempProducts;
                var result = DALQiangGou.CreateOrUpdateQianggou(dbHelper, model);
                if (result.Item1 < 0)
                {
                    dbHelper.Rollback();
                    return -38;//同步失败
                }
                else
                {
                    var chandata = LogChangeDataManager.GetLogChangeData(origin, model);
                    var beforeValue = QiangGouManager.GenerateSimpleQiangGouModel(chandata.Item1);
                    var afterValue = QiangGouManager.GenerateSimpleQiangGouModel(chandata.Item2);
                    var oprLog = new FlashSaleProductOprLog
                    {
                        OperateUser = ThreadIdentity.Operator.Name,
                        CreateDateTime = DateTime.Now,
                        BeforeValue = JsonConvert.SerializeObject(beforeValue),
                        AfterValue = JsonConvert.SerializeObject(afterValue),
                        LogType = "FlashSaleLog",
                        LogId = result.Item2.ToString(),
                        Operation = model.NeedExam ? "同步活动到待审核" : "同步活动"
                    };
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    LoggerManager.InsertFlashSaleLog(chandata.Item1, beforeValue.HashKey);
                    LoggerManager.InsertFlashSaleLog(chandata.Item2, afterValue.HashKey);
                }
            }
            foreach (var aid in aids)
            {
                var cache = ReflashQiangGouCache(aid, false, 0);
                if (cache == false)
                    return -3;
            }

            return 1;
        }
        public static Dictionary<string, IEnumerable<QiangGouProductModel>> SelectDiffActivityProducts(Guid? aid, int atype, IEnumerable<QiangGouProductModel> products, DateTime StartDateTime, DateTime EndDateTime)
        => DALQiangGou.SelectDiffActivityProducts(aid, atype, products, StartDateTime, EndDateTime);
        public static IEnumerable<QiangGouProductModel> CheckPIDSamePriceInOtherActivity(QiangGouProductModel product) => DALQiangGou.CheckPIDSamePriceInOtherActivity(product);


        public static IEnumerable<QiangGouModel> SelectAllNeedExamQiangGou()
        {
            return DALQiangGou.SelectAllNeedExamQiangGou();
        }

        public static List<ProductModel> SelectcostPriceSql(string pid)
        {
            return DALQiangGou.SelectProductCostPriceByPids(new List<string> { pid }).ToList();
        }
        public static QiangGouModel FetchNeedExamQiangGouAndProducts(Guid aid)
        {
            try
            {
                var dt = DALQiangGou.FetchNeedExamQiangGouAndProducts(aid);
                if (dt == null || dt.Rows.Count == 0)
                    return null;
                var model = dt.ConvertTo<QiangGouModel>()?.FirstOrDefault() ?? new QiangGouModel();
                //var  products = dt.ConvertTo<QiangGouProductModel>();
                var pids = dt.ConvertTo<QiangGouProductModel>().Select(r => r.PID).ToList();
                var costPriceSql = DALQiangGou.SelectProductCostPriceByPids(pids);
                var products = from a in dt.ConvertTo<QiangGouProductModel>()
                               join b in costPriceSql on a.PID equals b.PID into temp
                               from b in temp.DefaultIfEmpty()
                               select new QiangGouProductModel
                               {
                                   PKID = a.PKID,
                                   ActivityID = a.ActivityID,
                                   ActivityName = a.ActivityName,
                                   PID = a.PID,
                                   HashKey = a.HashKey,
                                   Price = a.Price,
                                   TotalQuantity = a.TotalQuantity,
                                   MaxQuantity = a.MaxQuantity,
                                   SaleOutQuantity = a.SaleOutQuantity,
                                   InstallAndPay = a.InstallAndPay,
                                   IsUsePCode = a.IsUsePCode,
                                   Channel = a.Channel,
                                   IsJoinPlace = a.IsJoinPlace,
                                   FalseOriginalPrice = a.FalseOriginalPrice,
                                   DisplayName = a.DisplayName,
                                   OriginalPrice = a.OriginalPrice,
                                   ProductName = a.ProductName,
                                   Label = a.Label,
                                   CostPrice = b?.CostPrice,
                                   Position = a.Position,
                                   IsShow = a.IsShow,
                                   InstallService = a.InstallService,
                                   DecreaseDegree = (Math.Round((a.OriginalPrice - a.Price) / a.OriginalPrice, 2) * 100) + "%"
                               };
                model.Products = products;
                return model;
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"审核查询数据失败==》{ex}-{ex.InnerException}-{ex.StackTrace}");
                return new QiangGouModel();
            }

        }

        public static int ExamUpdatePrice(QiangGouProductModel model) => DALQiangGou.ExamUpdatePrice(model);

        public static int ExamActivity(Guid ActitvityID) => DALQiangGou.ExamActivity(ActitvityID);

        public static QiangGouForCache SelectQiangGouToCache(Guid ActivityID)
        {
            var dt = DALQiangGou.SelectQiangGouToCache(ActivityID);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            var model = dt.ConvertTo<QiangGouForCache>()?.FirstOrDefault();
            model.Products = dt.ConvertTo<QiangGouProductForCache>();
            return model;
        }

        public static DataAccess.Entity.ProductModel FetchProductByPID(string pid)
        {
            var model = DALQiangGou.FetchProductByPID(pid);
            return AssemblyProductInstalService(model);

        }
        public static int? SelectFlashSaleSaleOutQuantity(string activityid, string pid)

           => DALQiangGou.SelectFlashSaleSaleOutQuantity(activityid, pid);


        public static bool RecordActivityType(ActivityTypeRequest request)
        {
            try
            {
                using (var client = new ActivityClient())
                {
                    var result = client.RecordActivityTypeLog(request);
                    result.ThrowIfException(true);
                    return result.Result;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static T AssemblyProductInstalService<T>(T model) where T : QiangGouProductModel, new()
        {
            if (model == null)
                return new T();
            var pid = model.PID;
            try
            {
                if (pid.StartsWith("TR-") || pid.StartsWith("LG-"))
                {
                    using (var client = new ProductClient())
                    {
                        var result = client.SelectTireAndHubInstallFeeByPids(
                            new List<ProductInstallFeeRequest> {
                    new ProductInstallFeeRequest {
                        PID =pid,
                        Quantity =1 }
                            });
                        result.ThrowIfException(true);
                        if (result.Success)
                        {
                            if (result.Result.ContainsKey(pid))
                                if (result.Result[pid].Select(r => r.Price).FirstOrDefault() > 0)
                                    model.InstallService = "支持安装";
                                else
                                    model.InstallService = "包安装";
                            else
                                model.InstallService = "无需安装";
                        }
                    }
                }
                else
                {
                    using (var client = new ProductClient())
                    {
                        var result = client.SelectProductInstallServices(new List<string> { pid });
                        result.ThrowIfException(true);
                        if (result.Success)
                        {
                            if (result.Result.Any())
                            {
                                if (result.Result.Select(r => r.ServicePrice).FirstOrDefault() > 0)
                                {
                                    model.InstallService = "支持安装";
                                }
                                else
                                    model.InstallService = "包安装";
                            }
                            else
                            {
                                model.InstallService = "无需安装";
                            }

                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"调用安装服务接口失败{ex}-{ex.InnerException}-{ex.StackTrace}");
                return model;
            }
        }

        public static IEnumerable<string> SelectPidsByParent(string productId)
       => DALQiangGou.SelectPidsByParent(productId);

        public static bool ReflashQiangGouCache(Guid aid, bool needLog, int activityType)
        {
            try
            {
                var result = true;
                if (activityType == 1 || activityType == 3)
                {
                    using (var client = new Service.Activity.CacheClient())
                    {
                        var cacheresult = client.RefreshRedisCachePrefixForCommon(new RefreshCachePrefixRequest()
                        {
                            Prefix = "SecondKillPrefix",
                            ClientName = "FlashSale",
                            Expiration = TimeSpan.FromDays(1)
                        });
                        cacheresult.ThrowIfException(true);
                        result = cacheresult.Result;
                    }
                }
                using (var client = new FlashSaleClient())
                {
                    var result1 = client.UpdateFlashSaleDataToCouchBaseByActivityID(aid);
                    //初始化缓存数据
                    var result2 = client.RefreshFlashSaleHashCount(new List<string> { aid.ToString() }, false);
                    //刷新活动页固化下来的数据
                    var activityExists = DALQiangGou.FetchActivityPageContentByActivityId(aid.ToString());

                    if (activityExists != null && activityExists.Any())
                    {
                        foreach (var activityExist in activityExists)
                        {
                            using (var actclient = new ActivityClient())
                            {
                                var activityId = actclient.GetOrSetActivityPageSortedPids(new SortedPidsRequest
                                {
                                    Brand = activityExist.Brand,
                                    ProductType = (ProductType)activityExist.ProductType,
                                    NeedUpdatePkid = activityExist.Pkid,
                                    DicActivityId = new KeyValuePair<string, ActivityIdType>(aid.ToString(), ActivityIdType.FlashSaleActivity)
                                });
                                var refresh = actclient.RefreshActivePageListModelCache(new ActivtyPageRequest
                                {
                                    Channel = "wap",
                                    HashKey = activityExist.HashKey,
                                });
                            }
                        }
                    }
                    if (needLog)
                    {
                        var oprLog = new FlashSaleProductOprLog();
                        oprLog.OperateUser = ThreadIdentity.Operator.Name;
                        oprLog.CreateDateTime = DateTime.Now;
                        oprLog.BeforeValue = JsonConvert.SerializeObject(new { actvityid = aid });
                        oprLog.AfterValue = JsonConvert.SerializeObject(new { actvityid = aid });
                        oprLog.LogType = "FlashSaleLog";
                        oprLog.LogId = aid.ToString();
                        oprLog.Operation = "刷新缓存";
                        LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    }
                    result1.ThrowIfException(true);
                    result2.ThrowIfException(true);

                    return result1.Result && result2.Result && result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Guid SelectLastActivityId()
        {
            return DALQiangGou.SelectLastActivityId();
        }

        public static QiangGouModel GenerateSimpleQiangGouModel(QiangGouModel model)
        {
            var hashkey = Guid.NewGuid().ToString();
            return new QiangGouModel
            {
                ActivityID = model.ActivityID,
                ActiveType = model.ActiveType,
                ActivityName = model.ActivityName,
                StartDateTime = model.StartDateTime,
                EndDateTime = model.EndDateTime,
                PlaceQuantity = model.PlaceQuantity,
                IsNewUserFirstOrder = model.IsNewUserFirstOrder,
                HashKey = hashkey
            };
        }

        /// <summary>
        /// 同步并保存
        /// </summary>
        /// <param name="model">产品信息</param>
        /// <param name="diffs">差别产品数据</param>
        /// <returns></returns>
        public static Tuple<int, Guid> SyncSave(QiangGouModel model, List<QiangGouDiffModel> diffs)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                dbHelper.BeginTransaction();
                try
                {
                    var syncResult = SyncDiffActivity(model, diffs, dbHelper);

                    if (syncResult < 0)
                    {
                        if (model.ActivityID != null)
                        {
                            return new Tuple<int, Guid>(syncResult, model.ActivityID.Value);
                        }
                    }

                    var result = DALQiangGou.CreateOrUpdateQianggou(dbHelper, model);

                    dbHelper.Commit();
                    return result;
                }
                catch (Exception e)
                {
                    dbHelper.Rollback();
                    throw new Exception(e.Message); ;
                }
            }
        }

        public static int SyncDiffActivity(QiangGouModel qiang, List<QiangGouDiffModel> diffs, SqlDbHelper dbHelper)
        {
            var diffList = new List<QiangGouProductModel>();
            if (diffs.Count > 0)
            {
                foreach (var item in diffs)
                {
                    diffList.Add(new QiangGouProductModel
                    {
                        PID = item.PID,
                        ActivityID = (Guid)item.ActivityID,
                        ProductName = item.ProductName,
                        Price = item.Price,
                        FalseOriginalPrice = item.FalseOriginalPrice,
                        InstallAndPay = item.InstallAndPay,
                        IsUsePCode = item.IsUsePCode
                    });
                }
            }

            var aids = diffList.Select(_ => _.ActivityID).Distinct().ToArray();

            foreach (var aid in aids)
            {
                var model = DALQiangGou.SelectQiangGouForSynchro(aid, dbHelper);
                var origin = model;
                var needAsyncPids = diffList.Where(_ => _.ActivityID == aid).Select(_ => _.PID).ToList();
                var tempProducts = model.Products.Where(_ => !needAsyncPids.Contains(_.PID)).ToList();
                bool flag = true;
                foreach (var pid in needAsyncPids)
                {
                    var product = qiang.Products.FirstOrDefault(_ => _.PID == pid);

                    var y_product = model.Products.FirstOrDefault(_ => _.PID == pid);
                    y_product.ProductName = product.ProductName;
                    y_product.Price = product.Price;
                    y_product.FalseOriginalPrice = product.FalseOriginalPrice;
                    y_product.InstallAndPay = product.InstallAndPay;
                    y_product.IsUsePCode = product.IsUsePCode;

                    tempProducts.Add(y_product);
                    if (!string.IsNullOrWhiteSpace(qiang.NeedExamPids) && qiang.NeedExamPids.Split(';').Contains(pid) && flag)
                        flag = false;
                }

                model.NeedExam = !flag;
                model.Products = tempProducts;

                var result = DALQiangGou.CreateOrUpdateQiangGou(dbHelper, model);

                if (result.Item1 < 0)
                {
                    dbHelper.Rollback();
                    return -38;//同步失败
                }

                var chandata = LogChangeDataManager.GetLogChangeData(origin, model);
                var beforeValue = GenerateSimpleQiangGouModel(chandata.Item1);
                var afterValue = GenerateSimpleQiangGouModel(chandata.Item2);
                var oprLog = new FlashSaleProductOprLog
                {
                    OperateUser = ThreadIdentity.Operator.Name,
                    CreateDateTime = DateTime.Now,
                    BeforeValue = JsonConvert.SerializeObject(beforeValue),
                    AfterValue = JsonConvert.SerializeObject(afterValue),
                    LogType = "FlashSaleLog",
                    LogId = result.Item2.ToString(),
                    Operation = model.NeedExam ? "同步活动到待审核" : "同步活动"
                };
                LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                LoggerManager.InsertFlashSaleLog(chandata.Item1, beforeValue.HashKey);
                LoggerManager.InsertFlashSaleLog(chandata.Item2, afterValue.HashKey);
            }

            foreach (var aid in aids)
            {
                var cache = ReflashActivityCache(aid, false, 0);
                if (cache == false)
                    return -3;
            }

            return 1;
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="needLog"></param>
        /// <param name="activityType"></param>
        /// <returns></returns>
        public static bool ReflashActivityCache(Guid aid, bool needLog, int activityType)
        {
            try
            {
                var result = true;
                if (activityType == 1 || activityType == 3)
                {
                    using (var client = new Service.Activity.CacheClient())
                    {
                        var cacheresult = client.RefreshRedisCachePrefixForCommon(new RefreshCachePrefixRequest()
                        {
                            Prefix = "SecondKillPrefix",
                            ClientName = "FlashSale",
                            Expiration = TimeSpan.FromDays(1)
                        });
                        cacheresult.ThrowIfException(true);
                        result = cacheresult.Result;
                    }
                }
                using (var client = new FlashSaleClient())
                {
                    var result1 = client.UpdateFlashSaleDataToCouchBaseByActivityID(aid);
                    //初始化缓存数据
                    var result2 = client.RefreshFlashSaleHashCount(new List<string> { aid.ToString() }, false);
                    //刷新活动页固化下来的数据
                    var activityExists = DALQiangGou.FetchActivityPageContentByActivityId(aid.ToString());

                    if (activityExists != null && activityExists.Any())
                    {
                        foreach (var activityExist in activityExists)
                        {
                            using (var actclient = new ActivityClient())
                            {
                                var activityId = actclient.GetOrSetActivityPageSortedPids(new SortedPidsRequest
                                {
                                    Brand = activityExist.Brand,
                                    ProductType = (ProductType)activityExist.ProductType,
                                    NeedUpdatePkid = activityExist.Pkid,
                                    DicActivityId = new KeyValuePair<string, ActivityIdType>(aid.ToString(), ActivityIdType.FlashSaleActivity)
                                });
                                var refresh = actclient.RefreshActivePageListModelCache(new ActivtyPageRequest
                                {
                                    Channel = "wap",
                                    HashKey = activityExist.HashKey,
                                });
                            }
                        }
                    }
                    if (needLog)
                    {
                        var oprLog = new FlashSaleProductOprLog();
                        oprLog.OperateUser = ThreadIdentity.Operator.Name;
                        oprLog.CreateDateTime = DateTime.Now;
                        oprLog.BeforeValue = JsonConvert.SerializeObject(new { actvityid = aid });
                        oprLog.AfterValue = JsonConvert.SerializeObject(new { actvityid = aid });
                        oprLog.LogType = "FlashSaleLog";
                        oprLog.LogId = aid.ToString();
                        oprLog.Operation = "刷新缓存";
                        LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    }
                    result1.ThrowIfException(true);
                    result2.ThrowIfException(true);

                    return result1.Result && result2.Result && result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
