using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Component.Identity;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.GroupBuyingV2Dao;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.GroupBuyingV2;
using Tuhu.Service.Member;

namespace Tuhu.Provisioning.Business.GroupBuyingV2
{
    public class GroupBuyingV2Manager
    {
        #region Connection
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(GroupBuyingV2Manager));

        public GroupBuyingV2Manager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }
        #endregion

        public static async Task<DataTable> SelectGroupBuyingSettingByPage(int pageIndex, int pageSize, string keyWord, int searchType, int groupCategory, int groupType, string groupLabel, int isFinishGroup)
        {
            return await DalGroupBuyingProductGroupConfig.SearchGroupBuyingSettingByPage(pageIndex, pageSize, keyWord, searchType, groupCategory, groupType, groupLabel, isFinishGroup);
        }
        public static async Task<int> SelectGroupBuyingSettingCount(string keyWord, int searchType, int groupCategory, int groupType, string groupLabel, int isFinishGroup)
        {
            return await DalGroupBuyingProductGroupConfig.SearchGroupBuyingSettingCount(keyWord, searchType, groupCategory, groupType, groupLabel, isFinishGroup);
        }
        public static async Task<DataTable> GetProductsByGroupBuyingId(string groupBuyingId)
        {
            return await DalGroupBuyingProductGroupConfig.GetProductsByGroupBuyingId(groupBuyingId);
        }
        public static async Task<bool> DeleteProductsByGroupBuyingId(string groupBuyingId)
        {
            return await DalGroupBuyingProductGroupConfig.DeleteProductsByGroupBuyingId(groupBuyingId);
        }
        public static async Task<bool> AddGroupBuyingSetting(GroupBuyingProductGroupConfigEntity GroupConfig, List<GroupBuyingProductConfigEntity> ProductConfig)
        {
            return await DalGroupBuyingProductGroupConfig.AddGroupBuyingSetting(GroupConfig, ProductConfig);
        }
        public static async Task<bool> UpdateGroupBuyingSetting(GroupBuyingProductGroupConfigEntity GroupConfig)
        {
            return await DalGroupBuyingProductGroupConfig.UpdateGroupBuyingSetting(GroupConfig);
        }

        public static Task<bool> UpdateGroupBuyingProductConfig(
            GroupBuyingProductConfigEntity productConfigEntity)
            => DalGroupBuyingProductGroupConfig.UpdateGroupBuyingProductConfig(productConfigEntity);

        public static async Task<DataTable> GetProductsByPID(string pid, bool isLottery)
        {
            return await DalGroupBuyingProductGroupConfig.GetProductsByPID(pid, isLottery);
        }
        //
        public static async Task<DataTable> GetGroupBuyingSettingByid(string ProductGroupId)
        {
            return await DalGroupBuyingProductGroupConfig.GetGroupBuyingSettingByid(ProductGroupId);
        }

        public static bool CheckIsExistProductGroupId(string productGroupId)
        {
            return DalGroupBuyingProductGroupConfig.CheckIsExistProductGroupId(productGroupId);
        }
        public static async Task<DataTable> GetGroupBuyingModifyLogByGroupId(string productGroupId)
        {
            return await DalGroupBuyingProductGroupConfig.GetGroupBuyingModifyLogByGroupId(productGroupId);
        }
        public static async Task<bool> InsertGroupBuyingModifyLog(string productGroupId, string name, string title)
        {
            return await DalGroupBuyingProductGroupConfig.InsertGroupBuyingModifyLog(productGroupId, name, title);
        }
        //
        public static bool CheckPIdWithGroupType(List<string> pids, int groupType)
        {
            return DalGroupBuyingProductGroupConfig.CheckPIdWithGroupType(pids, groupType);
        }

        public static List<string> GetAllProductGroupId()
            => DalGroupBuyingProductGroupConfig.GetAllProductGroupId();

        public static List<LotteryGroupInfo> GetLotteryGroup(string productGroupId, bool isActivity, int pageIndex, int pageSize)
            => DalGroupBuyingProductGroupConfig.GetLotteryGroup(productGroupId, isActivity, pageIndex, pageSize);
        public static int GetLotteryGroupCount()
            => DalGroupBuyingProductGroupConfig.GetLotteryGroupCount();
        public static Tuple<int, string> SetLotteryResult(string productGroupId, int level, Guid userId, int orderId, string _operator)
        {
            var result = -1;
            var info = "设置失败";
            var setResult = DalGroupBuyingProductGroupConfig.SetUserLotteryResult(productGroupId, level, orderId, userId);
            if (setResult > 0) { result = 1; info = $"{productGroupId}产品组下，用户{orderId}设置{level}等奖成功；"; }
            if (setResult > 0 && level == 1)
            {
                setResult = DalGroupBuyingProductGroupConfig.SetOtherLotteryResult(productGroupId, 2);
                info = $"{productGroupId}产品组下，用户{orderId}设置{level}等奖成功；" + (setResult == 0 ? "" : $"其他未开奖订单设置奖励{setResult}个成功");
                if (setResult < 1) result = 2;
            }

            LotteryLog(productGroupId, $"SetLevel->{level}", _operator, info);
            return Tuple.Create(result, info);
        }

        public static List<LotteryCouponModel> GetLotteryCoupon(string productGroupId)
        {
            var data = DalGroupBuyingProductGroupConfig.GetLotteryCouponList(productGroupId);
            var result = new List<LotteryCouponModel>();
            if (data.Any())
            {
                foreach (var item in data)
                {
                    var resultItem = GetCouponDetail(item.Item1, productGroupId);
                    if (resultItem != null)
                    {
                        resultItem.Creator = item.Item2;
                        result.Add(resultItem);
                    }
                }
            }
            return result;
        }

        public static List<LotteryUserModel> LotteryInfoList(string productGroupId, int orderId, int lotteryResult, PagerModel pager)
        {
            var totalCount = DalGroupBuyingProductGroupConfig.LotteryInfoCount(productGroupId, orderId, lotteryResult);
            pager.TotalItem = totalCount;
            if (totalCount > 0)
            {
                return DalGroupBuyingProductGroupConfig.LotteryInfoList(productGroupId, orderId, lotteryResult, pager.CurrentPage, pager.PageSize);
            }
            return new List<LotteryUserModel>();
        }
        public static bool LotteryLog(string productGroupId, string operateType, string _operator, string remark)
            => DalGroupBuyingProductGroupConfig.LotteryLog(productGroupId, operateType, _operator, remark);
        public static int GetLotteryUserCount(string productGroupId, int tag)
            => DalGroupBuyingProductGroupConfig.GetLotteryUserCount(productGroupId, tag);
        public static bool AddLotteryCoupon(string productGroupId, Guid couponId, string creator)
            => DalGroupBuyingProductGroupConfig.AddLotteryCoupon(productGroupId, couponId, creator);
        public static LotteryCouponModel GetCouponDetail(Guid couponId, string productGroupId = "")
        {
            using (var client = new PromotionClient())
            {
                var result = client.GetCouponRule(couponId);
                if (result.Success && result.Result != null)
                {
                    return new LotteryCouponModel
                    {
                        ProductGroupId = productGroupId,
                        CouponId = couponId,
                        CouponDesc = result.Result.Description,
                        CouponCondition = $"满{result.Result.MinMoney}减{result.Result.Discount}",
                        UsefulLife = result.Result.Term == null ? $"{result.Result.ValiStartDate}到{result.Result.ValiEndDate}" : $"自领取之后{result.Result.Term}天有效"
                    };
                }
            }
            return null;
        }
        public static bool DeleteLotteryCoupons(string productGroupId, List<Guid> couponIds, string _operator)
        {
            var result = false;
            foreach (var item in couponIds)
            {
                var resultItem = DalGroupBuyingProductGroupConfig.DeleteLotteryCoupon(productGroupId, item);
                var resultStr = resultItem ? "成功" : "失败";
                LotteryLog(productGroupId, "DeleteCouponOne", _operator, $"{item:D}优惠券删除{resultStr}");
                result |= resultItem;
            }
            return result;
        }

        public static List<LotteryLogInfo> GetLotteryLog(string productGroupId, string type, int size)
            => DalGroupBuyingProductGroupConfig.GetLotteryLog(productGroupId, type, size);

        public static List<LotteryUserInfo> GetUserInfoList(string productGroupId, int tag, int maxPkid, int step,
            bool? awardResult = null)
            => DalGroupBuyingProductGroupConfig.GetUserInfoList(productGroupId, tag, maxPkid, step, awardResult);

        public static List<LotteryUserInfo> GetUserInfoListForPush(string productGroupId, int tag, int maxOrderId, int step, bool? awardResult = null)
            => DalGroupBuyingProductGroupConfig.GetUserInfoListForPush(productGroupId, tag, maxOrderId, step, awardResult);

        public static List<LotteryUserInfo> GetUserInfo(string productGroupId, int orderId)
            => DalGroupBuyingProductGroupConfig.GetUserInfo(orderId, productGroupId);

        public static List<Dictionary<string, object>> PushConvertToMq(List<LotteryUserInfo> data, int batchId)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (var item in data)
            {
                var resultItem = new Dictionary<string, object>
                {
                    ["BatchId"] = batchId,
                    ["OrderId"] = item.OrderId,
                    ["UserId"] = item.UserId,
                    ["ProductGroupId"] = item.ProductGroupId
                };
                result.Add(resultItem);
            }
            return result;
        }

        public static List<Dictionary<string, object>> CancelConvertToMq(List<LotteryUserInfo> data)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (var item in data)
            {
                var resultItem = new Dictionary<string, object>
                {
                    ["OrderId"] = item.OrderId,
                    ["UserId"] = item.UserId,
                    ["ProductGroupId"] = item.ProductGroupId
                };
                result.Add(resultItem);
            }
            return result;
        }
        public static List<Dictionary<string, object>> CouponConvertToMq(List<LotteryUserInfo> data, List<Guid> couponList)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (var item in data)
            {
                var resultItem = new Dictionary<string, object>
                {
                    ["OrderId"] = item.OrderId,
                    ["UserId"] = item.UserId,
                    ["ProductGroupId"] = item.ProductGroupId,
                    ["CouponList"] = couponList
                };
                result.Add(resultItem);
            }
            return result;
        }

        public static List<GroupBuyingExportInfo> GetGroupBuyingExportInfo()
            => DalGroupBuyingProductGroupConfig.GetGroupBuyingExportInfo();

        public static List<GroupBuyingStockModel> GetGroupBuyingStockInfo(List<string> pids)
        {
            var data = DalGroupBuyingProductGroupConfig.GetGroupBuyingStockInfo(pids);
            var result = data.GroupBy(g => g.PID).Select(g => new GroupBuyingStockModel
            {
                PID = g.Key,
                SHAvailableStockQuantity =
                    g.FirstOrDefault(t => t.WAREHOUSEID == 8598)?.TotalAvailableStockQuantity ?? 0,
                SHStockCost = g.FirstOrDefault(t => t.WAREHOUSEID == 8598)?.StockCost ?? 0,
                SHZaituStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 8598)?.CaigouZaitu ?? 0,
                WHAvailableStockQuantity =
                    g.FirstOrDefault(t => t.WAREHOUSEID == 7295)?.TotalAvailableStockQuantity ?? 0,
                WHStockCost = g.FirstOrDefault(t => t.WAREHOUSEID == 7295)?.StockCost ?? 0,
                WHZaituStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 7295)?.CaigouZaitu ?? 0,
                TotalAvailableStockQuantity = g.Sum(t => t.TotalAvailableStockQuantity),
                TotalZaituStockQuantity = g.Sum(t => t.CaigouZaitu)
            }).ToList();
            return result;
        }

        #region 虚拟商品优惠券

        /// <summary>
        /// 查询虚拟商品列表
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="pid">商品pid</param>
        /// <returns></returns>
        public static List<VirtualProductCouponConfigModel> SelectProductCouponConfig(int pageIndex, int pageSize, string pid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return DalGroupBuyingProductGroupConfig.
                    SelectProductCouponConfig(conn, pageIndex, pageSize, pid).ToList();
            }
        }

        public static int SelectProductCouponConfigCount(string pid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return DalGroupBuyingProductGroupConfig.
                    SelectProductCouponConfigCount(conn, pid);
            }
        }

        /// <summary>
        /// 根据PID查询单个优惠券详情
        /// </summary>
        /// <param name="pid">pid</param>
        /// <returns></returns>
        public static List<VirtualProductCouponConfigModel> FetchProductCouponConfig(string pid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return DalGroupBuyingProductGroupConfig.FetchProductCouponConfig(conn, pid).ToList();
            }
        }

        public static List<string> SelectProductCouponConfig(IEnumerable<string> pids)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return DalGroupBuyingProductGroupConfig.SelectProductCouponConfig(conn, pids).ToList();
            }
        }

        public static int DeleteVirtualProductConfig(string pid)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return DalGroupBuyingProductGroupConfig.DeleteVirtualProductConfig(conn, pid, ThreadIdentity.Operator.UserName);
            }
        }

        /// <summary>
        /// 添加或更新虚拟商品配置
        /// </summary>
        /// <param name="configDetails">配置详情</param>
        /// <param name="updateBy">更新人</param>
        /// <returns></returns>
        public static int AddOrUpdateProductCouponConfig(IReadOnlyList<VirtualProductCouponConfigModel> configDetails)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return DalGroupBuyingProductGroupConfig.AddOrUpdateProductCouponConfig(conn, configDetails, ThreadIdentity.Operator.UserName);
            }
        }
        #endregion

        #region 新框架团购
        /// <summary>
        /// 获取团购商品信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="activityStatus"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<GroupBuyingProductGroupConfigEntity> SelectGroupBuyingV2Config(GroupBuyingProductGroupConfigEntity filter, string activityStatus, int pageIndex, int pageSize)
        {
            List<GroupBuyingProductGroupConfigEntity> result = new List<GroupBuyingProductGroupConfigEntity>();
            try
            {
                List<GroupBuyingStockModel> stockInfo = new List<GroupBuyingStockModel>();
                dbScopeReadManager.Execute(conn =>
                {
                    if (!string.IsNullOrEmpty(filter.PID) || !string.IsNullOrEmpty(filter.ProductName))
                    {
                        var filterData = DalGroupBuyingProductGroupConfig.GetGroupBuyingProductConfig(conn,
                            filter.PID, filter.ProductName);
                        if (filterData != null && filterData.Any())
                        {
                            result = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2Config(conn, filter,
                               activityStatus, filterData.Select(x => x.ProductGroupId).ToList(), pageIndex, pageSize);
                        }
                    }
                    else
                    {
                        result = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2Config(conn, filter,
                            activityStatus, new List<string> { filter.ProductGroupId }, pageIndex, pageSize);
                    }
                    if (result != null && result.Any())
                    {
                        result.ForEach(x =>
                        {
                            x.GroupProductDetails = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ProductConfigByGroupId(conn, new List<string> { x.ProductGroupId });
                        });
                    }
                });


            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public List<GroupBuyingStockModel> ConvertStockModel(List<ProductStockInfo> data)
        {
            return data?.GroupBy(g => g.PID)?.Select(g => new GroupBuyingStockModel
            {
                PID = g.Key,
                SHAvailableStockQuantity =
                   g.FirstOrDefault(t => t.WAREHOUSEID == 8598)?.TotalAvailableStockQuantity ?? 0,
                SHStockCost = g.FirstOrDefault(t => t.WAREHOUSEID == 8598)?.StockCost ?? 0,
                SHZaituStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 8598)?.CaigouZaitu ?? 0,
                WHAvailableStockQuantity =
                   g.FirstOrDefault(t => t.WAREHOUSEID == 7295)?.TotalAvailableStockQuantity ?? 0,
                WHStockCost = g.FirstOrDefault(t => t.WAREHOUSEID == 7295)?.StockCost ?? 0,
                WHZaituStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 7295)?.CaigouZaitu ?? 0,
                BeiJingAvailableStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 8634)?.TotalAvailableStockQuantity ?? 0,
                BeiJingZaituStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 8634)?.CaigouZaitu ?? 0,
                GuangZhouAvailableStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 11410)?.TotalAvailableStockQuantity ?? 0,
                GuangZhouZaituStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 11410)?.CaigouZaitu ?? 0,
                YiWuAvailableStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 28790)?.TotalAvailableStockQuantity ?? 0,
                YiWuZaituStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 28790)?.CaigouZaitu ?? 0,
                TotalAvailableStockQuantity = g.Sum(t => t.TotalAvailableStockQuantity),
                TotalZaituStockQuantity = g.Sum(t => t.CaigouZaitu)
            })?.ToList();
        }

        /// <summary>
        /// 通过GroupId获取团购信息
        /// </summary>
        /// <param name="ProductGroupId"></param>
        /// <returns></returns>
        public GroupBuyingProductGroupConfigEntity SelectGroupBuyingV2ConfigByGroupId(string productGroupId)
        {
            GroupBuyingProductGroupConfigEntity result = null;
            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ConfigByGroupId(conn, productGroupId);
                    result.GroupProductDetails = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ProductConfigByGroupId(conn, new List<string> { productGroupId });
                    var purchaseInfo = PurchaseService.SelectPurchaseInfoByPID(result.GroupProductDetails.Select(x => x.PID).ToList());
                    if (purchaseInfo != null && purchaseInfo.Any())
                    {
                        purchaseInfo = from c in purchaseInfo
                                       group c by c.PID into g
                                       select g.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        result.GroupProductDetails.ForEach(x =>
                        {
                            var pidItem = purchaseInfo.Where(y => String.Equals(x.PID, y.PID)).FirstOrDefault();
                            if (pidItem != null)
                            {
                                if (pidItem.OfferContractPrice > 0 || pidItem.OfferPurchasePrice > 0)
                                {
                                    x.CostPrice = pidItem.OfferPurchasePrice >= pidItem.OfferContractPrice ? pidItem.OfferContractPrice : pidItem.OfferPurchasePrice;
                                }
                                else if (pidItem.PurchasePrice > 0 && pidItem.ContractPrice > 0)
                                {
                                    x.CostPrice = pidItem.PurchasePrice >= pidItem.ContractPrice ? pidItem.ContractPrice : pidItem.PurchasePrice;
                                }
                                else if (pidItem.PurchasePrice > 0 || pidItem.ContractPrice > 0)
                                {
                                    x.CostPrice = pidItem.PurchasePrice > 0 ? pidItem.PurchasePrice : pidItem.ContractPrice;
                                }
                            }
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public List<GroupBuyingProductConfigEntity> GetGroupBuyingV2ProductConfigByGroupId(string productGroupId)
        {
            List<GroupBuyingProductConfigEntity> result = new List<GroupBuyingProductConfigEntity>();
            try
            {
                result = dbScopeReadManager.Execute(conn => DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ProductConfigByGroupId(conn, new List<string> { productGroupId }));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }


        public List<GroupBuyingProductConfigEntity> RefreshProductConfigByGroupId(string productGroupId, bool isLottery)
        {
            List<GroupBuyingProductConfigEntity> result = new List<GroupBuyingProductConfigEntity>();
            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    var data = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ProductConfigByGroupId(conn, new List<string> { productGroupId });
                    var products = DalGroupBuyingProductGroupConfig.GetProductsByPIDAndIsLottery(conn, data.Where(x => x.DisPlay == true).FirstOrDefault()?.PID, isLottery);
                    if (products != null && products.Any())
                    {
                        var purchaseInfo = PurchaseService.SelectPurchaseInfoByPID(products.Select(x => x.PID).ToList());
                        purchaseInfo = from c in purchaseInfo
                                       group c by c.PID into g
                                       select g.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        products.ForEach(x =>
                        {
                            GroupBuyingProductConfigEntity item = new GroupBuyingProductConfigEntity();
                            var existItem = data.Where(_ => String.Equals(_.PID, x.PID)).FirstOrDefault();
                            var pidItem = purchaseInfo?.Where(y => String.Equals(x.PID, y.PID))?.FirstOrDefault();
                            if (pidItem != null)
                            {
                                if (pidItem?.OfferContractPrice > 0 || pidItem?.OfferPurchasePrice > 0)
                                {
                                    item.CostPrice = pidItem.OfferPurchasePrice >= pidItem.OfferContractPrice ? pidItem.OfferContractPrice : pidItem.OfferPurchasePrice;
                                }
                                else if (pidItem.PurchasePrice > 0 && pidItem.ContractPrice > 0)
                                {
                                    x.CostPrice = pidItem.PurchasePrice >= pidItem.ContractPrice ? pidItem.ContractPrice : pidItem.PurchasePrice;
                                }
                                else if (pidItem.PurchasePrice > 0 || pidItem.ContractPrice > 0)
                                {
                                    x.CostPrice = pidItem.PurchasePrice > 0 ? pidItem.PurchasePrice : pidItem.ContractPrice;
                                }
                            }
                            item.PID = x.PID;
                            item.ProductName = x.DisplayName;
                            item.UpperLimitPerOrder = existItem?.UpperLimitPerOrder ?? 1;
                            item.BuyLimitCount = existItem?.BuyLimitCount ?? 0;
                            item.DisPlay = existItem?.DisPlay ?? false;
                            item.UseCoupon = existItem?.UseCoupon ?? true;
                            item.IsShow = existItem?.IsShow ?? true;
                            item.OriginalPrice = existItem?.OriginalPrice ?? decimal.Parse(x.CY_List_Price.ToString());
                            item.FinalPrice = existItem?.FinalPrice ?? decimal.Parse(x.CY_List_Price.ToString());
                            item.SpecialPrice = existItem?.SpecialPrice ?? decimal.Parse(x.CY_List_Price.ToString());
                            item.TotalStockCount = existItem?.TotalStockCount ?? 1000000;
                            item.CurrentSoldCount = existItem?.CurrentSoldCount ?? 0;
                            item.IsAutoStock = existItem?.IsAutoStock ?? false;
                            item.IsShowApp = existItem?.IsShowApp ?? false;
                            result.Add(item);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result.OrderByDescending(x=>x.DisPlay).ToList();
        }

        public List<VW_ProductsModel> GetProductsByPIDAndIsLottery(string pid, bool isLottery)
        {
            List<VW_ProductsModel> result = new List<VW_ProductsModel>();
            try
            {
                result = dbScopeReadManager.Execute(conn => DalGroupBuyingProductGroupConfig.GetProductsByPIDAndIsLottery(conn, pid, isLottery));
                var purchaseInfo = PurchaseService.SelectPurchaseInfoByPID(result.Select(x => x.PID).ToList());
                if (purchaseInfo != null && purchaseInfo.Any())
                {
                    purchaseInfo = from c in purchaseInfo
                                   group c by c.PID into g
                                   select g.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    result.ForEach(x =>
                    {
                        var pidItem = purchaseInfo.Where(y => String.Equals(x.PID, y.PID)).FirstOrDefault();
                        if (pidItem != null)
                        {
                            if (pidItem.OfferContractPrice > 0 || pidItem.OfferPurchasePrice > 0)
                            {
                                x.CostPrice = pidItem.OfferPurchasePrice >= pidItem.OfferContractPrice ? pidItem.OfferContractPrice : pidItem.OfferPurchasePrice;
                            }
                            else if (pidItem.PurchasePrice > 0 && pidItem.ContractPrice > 0)
                            {
                                x.CostPrice = pidItem.PurchasePrice >= pidItem.ContractPrice ? pidItem.ContractPrice : pidItem.PurchasePrice;
                            }
                            else if (pidItem.PurchasePrice > 0 || pidItem.ContractPrice > 0)
                            {
                                x.CostPrice = pidItem.PurchasePrice > 0 ? pidItem.PurchasePrice : pidItem.ContractPrice;
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public DataTable GetLogByGroupId(string groupId)
        {
            return DalGroupBuyingProductGroupConfig.GetLogByGroupId(groupId);
        }

        public List<ProductStockInfo> GetStockInfoByPIDs(List<string> pids)
        {
            List<ProductStockInfo> result = new List<ProductStockInfo>();
            try
            {
                result = DalGroupBuyingProductGroupConfig.GetStockInfoByPIDs(pids);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// 编辑团购信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Tuple<bool, string> UpsertGroupBuyingConfig(GroupBuyingProductGroupConfigEntity data)
        {
            var result = false;
            var msg = "操作失败";
            GroupBuyingProductGroupConfigEntity oldData = null;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    if (string.IsNullOrWhiteSpace(data.ProductGroupId))
                    {
                        data.ProductGroupId = $"PT{DateTime.Now.GetHashCode()}".Substring(0, 10);
                        if (DalGroupBuyingProductGroupConfig.IsExistProductGroupId(conn, data.ProductGroupId) > 0)
                            data.ProductGroupId = $"PT{DateTime.Now.GetHashCode()}".Substring(0, 10);
                        DalGroupBuyingProductGroupConfig.InsertGroupBuyingGroupConfig(conn, data);
                        foreach (var item in data.GroupProductDetails)
                        {
                            item.ProductGroupId = data.ProductGroupId;
                            item.Creator = data.Creator;
                            DalGroupBuyingProductGroupConfig.InsertGroupBuyingProductConfig(conn, item);
                        }
                    }
                    else
                    {
                        oldData = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ConfigByGroupId(conn, data.ProductGroupId );
                        if (oldData != null)
                            oldData.GroupProductDetails = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ProductConfigByGroupId(conn, new List<string> { data.ProductGroupId });

                        DalGroupBuyingProductGroupConfig.UpdateGroupBuyingGroupConfig(conn, data);
                        DalGroupBuyingProductGroupConfig.DeleteGroupBuyingProductConfig(conn, data.ProductGroupId);
                        foreach (var item in data.GroupProductDetails)
                        {
                            item.ProductGroupId = data.ProductGroupId;
                            item.Creator = data.Creator;
                            DalGroupBuyingProductGroupConfig.InsertGroupBuyingProductConfig(conn, item);
                        }
                    }
                    result = true;
                });
            }
            catch (Exception ex)
            {
                msg = "系统异常";
                logger.Error(ex);
            }
            if (result)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    foreach (var item in data.GroupProductDetails)
                    {
                        TuhuNotification.SendNotification("#.PinTuanProductStatusSyncQueue.#",
                            new Dictionary<string, object>
                            {
                                ["PId"] = item.PID,
                                ["ProductGroupId"] = data.ProductGroupId
                            }, 3000);
                    }
                    InsertLog(data.ProductGroupId, "GroupBuyingProductConfig", "UpsertGroupBuyingConfig", result ? "操作成功" : msg, JsonConvert.SerializeObject(oldData), JsonConvert.SerializeObject(data), data.Creator);
                    ActivityService.RefrestPTCache(data.ProductGroupId);
                });
            }
            return Tuple.Create(result, msg);
        }

        public bool UpdateProductConfigIsShow(string groupId, bool isShow, string user)
        {
            var result = false;
            try
            {
                result = dbScopeManager.Execute(conn => DalGroupBuyingProductGroupConfig.UpdateProductConfigIsShow(conn, groupId, isShow)) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            InsertLog(groupId, "GroupBuyingProductConfig", "UpdateProductConfigIsShow", result ? "操作成功" : "操作失败", string.Empty, isShow.ToString(), user);
            return result;
        }

        public bool UpdateProductSimpleData(string groupId, int sequence, DateTime beginTime, DateTime endTime, int totalGroupCount, string user)
        {
            var result = false;
            try
            {
                dbScopeManager.Execute(conn =>
                {
                    result = DalGroupBuyingProductGroupConfig.UpdateProductSimpleData(conn, groupId, sequence, beginTime, endTime, totalGroupCount) > 0;
                    if (result)
                        DalGroupBuyingProductGroupConfig.InsertGroupBuyingLog(conn, groupId, user, "UpdateSimpleData");
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public List<GroupBuyingConfigLog> GetGroupBuyingLogByIdentityId(string identityId, string source)
        {
            return DalGroupBuyingProductGroupConfig.GetGroupBuyingLogByIdentityId(identityId, source);
        }

        public static void InsertLog(string identityId, string source, string methodType,
            string msg, string beforeValue, string afterValue, string user)
        {
            try
            {
                var data = new
                {
                    IdentityID = identityId,
                    Source = source,
                    MethodType = methodType,
                    Msg = msg,
                    BeforeValue = beforeValue,
                    AfterValue = afterValue,
                    OperateUser = user
                };
                LoggerManager.InsertLog("GroupBuyingConfigLog", data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        #endregion
    }
}
