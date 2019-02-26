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
using Tuhu.Provisioning.Business.Cache;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.ServiceProxy;
using Tuhu.Provisioning.DataAccess.DAO.GroupBuyingV2Dao;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.GroupBuyingV2;
using Tuhu.Provisioning.DataAccess.Response;
using Tuhu.Service.Member;
using Tuhu.Service.Product.ModelDefinition.ProductModels;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Request;
using Common.Logging;
using Newtonsoft.Json;

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

        /// <summary>
        /// 将发送短信内容转化为Mq数据
        /// </summary>
        /// <param name="users">用户列表</param>
        /// <param name="templateId">短信模板Id</param>
        /// <param name="productName">商品名称</param>
        /// <returns></returns>
        public static async Task<List<Dictionary<string, object>>> SendSmsConvertToMqData(List<LotteryUserInfo> users, int templateId, string productName)
        {
            var result = new List<Dictionary<string, object>>();
            var userService = new UserService();
            foreach (var item in users)
            {
                var user = await userService.FetchUserByUserId(item.UserId);

                result.Add(new Dictionary<string, object>
                {
                    ["Cellphone"] = user.Cellphone,
                    ["TemplateId"] = templateId,
                    ["TemplateArguments"] = new[] { user.Nickname, productName }
                });
            }
            return result;
        }

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
        /// <param name="deletedConfigs">已删除配置</param>
        /// <returns></returns>
        public static int AddOrUpdateProductCouponConfig(IEnumerable<VirtualProductCouponConfigModel> configDetails,
            IEnumerable<VirtualProductCouponConfigModel> deletedConfigs)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return DalGroupBuyingProductGroupConfig.AddOrUpdateProductCouponConfig(conn, configDetails, deletedConfigs, ThreadIdentity.Operator.UserName);
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
                            ComputeGroupProductsCost(x.GroupProductDetails);
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
                TianJinAvailableStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 31860)?.TotalAvailableStockQuantity ?? 0,
                TianJinZaituStockQuantity = g.FirstOrDefault(t => t.WAREHOUSEID == 31860)?.CaigouZaitu ?? 0,
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
        public GroupBuyingProductGroupConfigEntity SelectGroupBuyingV2ConfigByGroupId(string productGroupId, bool isTireGroup = false)
        {
            GroupBuyingProductGroupConfigEntity result = null;
            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ConfigByGroupId(conn, productGroupId, isTireGroup);
                    if (!isTireGroup)
                    {
                        result.GroupProductDetails = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ProductConfigByGroupId(conn, new List<string> { productGroupId });
                        ComputeGroupProductsCost(result.GroupProductDetails);
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 计算拼团商品成本价
        /// </summary>
        /// <param name="products">商品列表</param>
        private void ComputeGroupProductsCost(List<GroupBuyingProductConfigEntity> products)
        {
            var purchaseInfo = PurchaseService.SelectPurchaseInfoByPID(products.Select(x => x.PID).ToList());
            if (purchaseInfo != null && purchaseInfo.Any())
            {
                purchaseInfo = from c in purchaseInfo
                               group c by c.PID into g
                               select g.OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                products.ForEach(x =>
                {
                    var pidItem = purchaseInfo.Where(y => string.Equals(x.PID, y.PID)).FirstOrDefault();
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
            return result.OrderByDescending(x => x.DisPlay).ToList();
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
        public Tuple<bool, string> UpsertGroupBuyingConfig(GroupBuyingProductGroupConfigEntity data,
            bool isTireGroup = false, string copyFrom = "")
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

                        // 先将轮胎拼团标记为IsDelete，在保存配置后取消（防止团内无商品）
                        if (isTireGroup) data.IsDelete = true;
                        DalGroupBuyingProductGroupConfig.InsertGroupBuyingGroupConfig(conn, data);

                        if (!isTireGroup)
                        {
                            foreach (var item in data.GroupProductDetails)
                            {
                                item.ProductGroupId = data.ProductGroupId;
                                item.Creator = data.Creator;
                                DalGroupBuyingProductGroupConfig.InsertGroupBuyingProductConfig(conn, item);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(copyFrom))
                            {
                                CopyGroupBuyingTireProducts(conn, data.ProductGroupId, copyFrom, data.Creator);
                            }
                        }
                    }
                    else
                    {
                        DalGroupBuyingProductGroupConfig.UpdateGroupBuyingGroupConfig(conn, data);
                        oldData = DalGroupBuyingProductGroupConfig.GetGroupBuyingV2ConfigByGroupId(conn, data.ProductGroupId);

                        if (!isTireGroup)
                        {
                            DalGroupBuyingProductGroupConfig.DeleteGroupBuyingProductConfig(conn, data.ProductGroupId);
                            foreach (var item in data.GroupProductDetails)
                            {
                                item.ProductGroupId = data.ProductGroupId;
                                item.Creator = data.Creator;
                                DalGroupBuyingProductGroupConfig.InsertGroupBuyingProductConfig(conn, item);
                            }

                            if (oldData != null)
                            {
                                oldData.GroupProductDetails = DalGroupBuyingProductGroupConfig
                                .GetGroupBuyingV2ProductConfigByGroupId(conn, new List<string> { data.ProductGroupId });
                            }
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
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    if (!isTireGroup)
                    {
                        foreach (var item in data.GroupProductDetails)
                            SendMQ2UpdateProductStatus(item.PID, data.ProductGroupId);
                    }

                    InsertLog(data.ProductGroupId, "GroupBuyingProductConfig", "UpsertGroupBuyingConfig", result ? "操作成功" : msg, JsonConvert.SerializeObject(oldData), JsonConvert.SerializeObject(data), data.Creator);
                    ActivityService.RefrestPTCache(data.ProductGroupId);
                });
            }

            msg = result == true ? data.ProductGroupId : msg;
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

        #region 轮胎拼团

        /// <summary>
        /// 获取轮胎规格和花纹
        /// </summary>
        /// <param name="bands">品牌</param>
        /// <returns></returns>
        public async Task<Dictionary<string, List<ProductFilterProperty>>>
            GetTireSizesWithPatterns(string[] bands)
        {
            var searchService = new ProductSearchService();
            var searchResult = await searchService.GetTiresByFilter(new SearchProductRequest()
            {
                Parameters = new Dictionary<string, IEnumerable<string>>
                {
                    ["Category"] = new[] { "Tires" },
                    ["CP_Brand"] = bands
                }
            }, new[] { "CP_Tire_Width", "CP_Tire_AspectRatio", "CP_Tire_Rim", "CP_Tire_Pattern" });

            return searchResult;
        }

        /// <summary>
        /// 查询轮胎商品
        /// </summary>
        /// <param name="pid">商品Id</param>
        /// <param name="brands">品牌</param>
        /// <param name="pattern">花纹</param>
        /// <param name="width">胎面宽</param>
        /// <param name="aspectRatio">扁平比</param>
        /// <param name="rim">直径</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Tuhu.Models.PagedModel<ProductBaseInfo>> SearchTireProducts(string pid, string[] brands,
            string pattern, string width, string aspectRatio, string rim, int pageIndex, int pageSize = 10)
        {
            var pagedProductInfo = new Tuhu.Models.PagedModel<ProductBaseInfo>();
            var pagedPids = new Tuhu.Models.PagedModel<string>();

            if (string.IsNullOrWhiteSpace(pid))
            {
                var searchService = new ProductSearchService();
                pagedPids = await searchService.SearchProduct(new SearchProductRequest()
                {
                    CurrentPage = pageIndex,
                    PageSize = pageSize,
                    Parameters = new Dictionary<string, IEnumerable<string>>
                    {
                        ["OnSale"] = new[] { "1" },
                        ["Category"] = new[] { "Tires" },
                        ["CP_Brand"] = brands,
                        ["CP_Tire_Pattern"] = new[] { pattern },
                        ["CP_Tire_Width"] = new[] { width },
                        ["CP_Tire_AspectRatio"] = new[] { aspectRatio },
                        ["CP_Tire_Rim"] = new[] { rim }
                    }
                });
            }
            else
            {
                pagedPids.Source = new[] { pid };
                pagedPids.Pager = new Tuhu.Models.PagerModel();
            }

            if (pagedPids == null) return pagedProductInfo;
            var productInfoQueryService = new ProductInfoQueryService();
            pagedProductInfo.Source = productInfoQueryService.GetProductsInfoByPids(pagedPids.Source.ToArray());
            pagedProductInfo.Pager = new Tuhu.Models.PagerModel()
            {
                Total = string.IsNullOrWhiteSpace(pid) ? pagedPids.Pager.Total : pagedProductInfo.Source.Count()
            };

            return pagedProductInfo;
        }

        /// <summary>
        /// 添加拼团轮胎商品
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="configs">配置信息</param>
        /// <param name="creator">创建人</param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> AddGroupBuyingTireProducts(string productGroupId, string[] pids, string creator)
        {
            var pinTuanService = new PinTuanService();
            var productCount = await pinTuanService.GetGroupBuyingProductCount(productGroupId);
            if (productCount + pids.Length > 100) return new Tuple<bool, string>(false, "商品超过最大限制100个");

            var productInfoQueryService = new ProductInfoQueryService();
            var productsInfo = productInfoQueryService.GetProductsInfoByPids(pids);
            var tireProductsInfo = productInfoQueryService.GetTireProductsInfoByPids(pids);

            var hasDefaultProduct = await pinTuanService.CheckGroupBuyingHasDefaultProduct(productGroupId);
            using (var dbHelper = Tuhu.Component.Common.DbHelper.CreateDefaultDbHelper())
            {
                try
                {
                    dbHelper.BeginTransaction();

                    foreach (var pid in pids)
                    {
                        // 检验当前商品是否已添加
                        var hasSpecifyProduct = await pinTuanService.CheckGroupBuyingHasSpecifyProduct(productGroupId, pid);
                        if (hasSpecifyProduct)
                        {
                            dbHelper.Rollback();
                            return new Tuple<bool, string>(false, $"已存在商品{pid}");
                        }

                        var productInfo = productsInfo.Where(p => p.Pid.Equals(pid)).FirstOrDefault();
                        var tireProductInfo = tireProductsInfo.Where(p => p.Pid.Equals(pid)).FirstOrDefault();
                        if (productInfo != null && tireProductInfo != null)
                        {
                            var productConfigId = DalGroupBuyingProductGroupConfig
                             .InsertGroupBuyingProductConfig(dbHelper,
                             new GroupBuyingProductConfigEntity
                             {
                                 ProductName = productInfo.DisplayName,
                                 PID = pid,
                                 ProductGroupId = productGroupId,
                                 OriginalPrice = productInfo.Price,
                                 FinalPrice = productInfo.Price,
                                 SpecialPrice = productInfo.Price,
                                 DisPlay = hasDefaultProduct ? false : pid.Equals(pids[0]),
                                 Creator = creator,
                                 UseCoupon = true,
                                 IsShow = true,
                                 UpperLimitPerOrder = 1,
                                 TotalStockCount = 10000
                             });

                            DalGroupBuyingProductGroupConfig.InsertGroupBuyingTireProductConfig
                                (dbHelper, new GroupBuyingTireProductConfigEntity
                                {
                                    ProductConfigID = productConfigId,
                                    TireBrand = productInfo.Brand,
                                    TirePattern = tireProductInfo.Pattern,
                                    TireWidth = productInfo.Size.Width,
                                    TireAspectRatio = productInfo.Size.AspectRatio,
                                    TireRim = productInfo.Size.Rim
                                });
                        }
                    }

                    dbHelper.Commit();
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    logger.Error(ex);
                    return new Tuple<bool, string>(false, "添加轮胎商品失败");
                }
            }

            InsertLog(productGroupId, "GroupBuyingProductConfig", "AddGroupBuyingTireProducts",
                "操作成功", string.Empty, string.Join(",", pids), creator);

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// 保存拼团轮胎商品
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="configs">配置信息</param>
        /// <param name="user">创建人</param>
        /// <returns></returns>
        public Tuple<bool, string> SaveGroupBuyingTireProducts(string productGroupId,
            List<GroupBuyingProductConfigEntity> configs, string user)
        {
            if (configs.Where(c => c.DisPlay).Count() != 1)
                return new Tuple<bool, string>(false, "有且只能有一个默认商品");

            var groupConfig = new GroupBuyingProductGroupConfigEntity();
            var products = new List<GroupBuyingProductConfigEntity>();
            dbScopeManager.Execute(conn =>
            {
                groupConfig = DalGroupBuyingProductGroupConfig
                .GetGroupBuyingV2ConfigByGroupId(conn, productGroupId, true);

                products = DalGroupBuyingProductGroupConfig.
                GetGroupBuyingV2ProductConfigByGroupId(conn, new List<string> { productGroupId });
            });

            var pids = configs.Select(p => p.PID).ToArray();
            var productInfoQueryService = new ProductInfoQueryService();
            var productsInfo = productInfoQueryService.GetProductsInfoByPids(pids);

            using (var dbHelper = Tuhu.Component.Common.DbHelper.CreateDefaultDbHelper())
            {
                try
                {
                    dbHelper.BeginTransaction();

                    // 更新轮胎商品配置
                    foreach (var config in configs)
                    {
                        if (config.FinalPrice == 0 || config.SpecialPrice == 0)
                        {
                            dbHelper.Rollback();
                            return new Tuple<bool, string>(false, "拼团价或团长价不能为0");
                        }

                        if (groupConfig.GroupType == 0 && (config.FinalPrice != config.SpecialPrice))
                        {
                            dbHelper.Rollback();
                            return new Tuple<bool, string>(false, "普通团的团长价应等于拼团价");
                        }

                        var productInfo = productsInfo.Where(p => p.Pid.Equals(config.PID)).FirstOrDefault();
                        if (productInfo?.Onsale != true)
                        {
                            dbHelper.Rollback();
                            return new Tuple<bool, string>(false, $"商品{config.PID}不存在或已下架");
                        }

                        DalGroupBuyingProductGroupConfig
                            .UpdateGroupBuyingTireProductConfig(dbHelper, config);
                    }

                    // 恢复轮胎团状态
                    DalGroupBuyingProductGroupConfig
                        .ChangeGroupBuyingTireGroupStatus(dbHelper, productGroupId);

                    dbHelper.Commit();
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    logger.Error(ex);
                    return new Tuple<bool, string>(false, "保存轮胎商品失败");
                }
            }

            // 记录日志以及刷新缓存
            ThreadPool.QueueUserWorkItem(_ =>
            {
                foreach (var config in configs)
                {
                    SendMQ2UpdateProductStatus(config.PID, productGroupId);

                    var oldConfig = products.Where(p => p.PID.Equals(config.PID)).FirstOrDefault();
                    InsertLog(productGroupId, "GroupBuyingProductConfig", "SaveGroupBuyingTireProducts",
                        "操作成功", JsonConvert.SerializeObject(oldConfig), JsonConvert.SerializeObject(config), user);
                }

                var pinTuanService = new PinTuanService();
                pinTuanService.RemoveGroupBuyingTireProductsCache(productGroupId);
                ActivityService.RefrestPTCache(productGroupId);
            });

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// 获取拼团轮胎商品
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="query">查询参数</param>
        /// <returns></returns>
        public List<GroupBuyingTireProductConfigResponse> GetGroupBuyingTireProducts
            (string productGroupId, string query)
        {
            var tireProducts = new List<GroupBuyingTireProductConfigResponse>();

            dbScopeManager.Execute(conn =>
            {
                tireProducts = DalGroupBuyingProductGroupConfig
                .GetGroupBuyingTireProductConfigs(conn, productGroupId, query);
            });

            var pids = tireProducts.Select(p => p.PID).ToArray();
            var productInfoQueryService = new ProductInfoQueryService();
            var productsInfo = productInfoQueryService.GetProductsInfoByPids(pids);

            tireProducts.ForEach(p =>
            {
                p.UsableStockCount = p.TotalStockCount - p.CurrentSoldCount;
                var productInfo = productsInfo.Where(x => x.Pid.Equals(p.PID)).FirstOrDefault();
                if (productInfo != null)
                {
                    p.OnSale = productInfo.Onsale;
                }
            });

            ComputeTireProductsCostPrice(tireProducts);
            return tireProducts;
        }

        /// <summary>
        /// 设置拼团默认商品
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="pid">产品Id</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public async Task<bool> SetGroupBuyingDefaultProduct(string productGroupId, string pid, string user)
        {
            var pinTuanService = new PinTuanService();
            var setResult = await pinTuanService.SetGroupBuyingDefaultProduct(productGroupId, pid);

            var pids = (await pinTuanService.GetGroupBuyingTireProductSetById(productGroupId)).Select(p => p.Key).ToArray();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                foreach (var id in pids)
                    SendMQ2UpdateProductStatus(id, productGroupId);

                InsertLog(productGroupId, "GroupBuyingProductConfig", "SetGroupBuyingDefaultProduct",
                    setResult ? "操作成功" : "操作失败", string.Empty, pid, user);
                ActivityService.RefrestPTCache(productGroupId);
            });

            return setResult;
        }

        /// <summary>
        /// 删除拼团轮胎商品
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="productConfigIds">产品配置Id</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> DeleteGroupBuyingTireProducts(string productGroupId,
            int[] productConfigIds, string user)
        {
            var errorProductConfigIdList = new List<int>();

            var pinTuanService = new PinTuanService();
            foreach (var productConfigId in productConfigIds)
            {
                var result = await pinTuanService
                    .DeleteGroupBuyingTireProductConfig(productConfigId);

                if (!result) errorProductConfigIdList.Add(productConfigId);
            }

            if (errorProductConfigIdList.Count == 0)
                return new Tuple<bool, string>(true, string.Empty);

            var pids = (await pinTuanService.GetGroupBuyingTireProductSetById(productGroupId)).Select(p => p.Key).ToArray();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                foreach (var pid in pids)
                    SendMQ2UpdateProductStatus(pid, productGroupId);

                InsertLog(productGroupId, "GroupBuyingProductConfig", "DeleteGroupBuyingTireProducts",
                    "操作成功", string.Empty, string.Join(",", productConfigIds), user);

                pinTuanService.RemoveGroupBuyingTireProductsCache(productGroupId);
                ActivityService.RefrestPTCache(productGroupId);
            });

            return new Tuple<bool, string>(false, string.Join("，", errorProductConfigIdList));
        }

        /// <summary>
        /// 批量更新拼团商品
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="columnName">列名</param>
        /// <param name="value">值</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public async Task<bool> BatchUpdateGroupBuyingProduct(string productGroupId,
            string columnName, object value, string user)
        {
            var pinTuanService = new PinTuanService();
            var updateResult = await pinTuanService
                .BatchUpdateGroupBuyingProductConfig(productGroupId, columnName, value);

            var pids = (await pinTuanService.GetGroupBuyingTireProductSetById(productGroupId)).Select(p => p.Key).ToArray();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                foreach (var pid in pids)
                    SendMQ2UpdateProductStatus(pid, productGroupId);

                InsertLog(productGroupId, "GroupBuyingProductConfig", "BatchUpdateGroupBuyingProduct",
                    updateResult ? "操作成功" : "操作失败", string.Empty, $"{columnName}-->{value}", user);
                ActivityService.RefrestPTCache(productGroupId);
            });

            return updateResult;
        }

        /// <summary>
        /// 复制拼团轮胎团信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="copyFrom">复制产品组Id</param>
        /// <param name="creator">创建人</param>
        /// <returns></returns>
        private void CopyGroupBuyingTireProducts(SqlConnection conn, string productGroupId,
            string copyFrom, string creator)
        {
            var products = DalGroupBuyingProductGroupConfig.
                GetGroupBuyingV2ProductConfigByGroupId(conn, new List<string> { copyFrom });
            var pids = products.Select(p => p.PID).ToArray();

            var productInfoQueryService = new ProductInfoQueryService();
            var productsInfo = productInfoQueryService.GetProductsInfoByPids(pids);
            var tireProductsInfo = productInfoQueryService.GetTireProductsInfoByPids(pids);

            foreach (var product in products)
            {
                var productInfo = productsInfo.Where(p => p.Pid.Equals(product.PID)).FirstOrDefault();
                var tireProductInfo = tireProductsInfo.Where(p => p.Pid.Equals(product.PID)).FirstOrDefault();
                if (productInfo != null && tireProductInfo != null)
                {
                    var productConfigId = DalGroupBuyingProductGroupConfig
                     .InsertGroupBuyingProductConfig(conn,
                     new GroupBuyingProductConfigEntity
                     {
                         ProductName = product.ProductName,
                         PID = product.PID,
                         ProductGroupId = productGroupId,
                         OriginalPrice = product.OriginalPrice,
                         FinalPrice = product.FinalPrice,
                         SpecialPrice = product.SpecialPrice,
                         DisPlay = product.DisPlay,
                         Creator = creator,
                         UseCoupon = product.UseCoupon,
                         IsShow = product.IsShow,
                         IsActive = product.IsActive,
                         UpperLimitPerOrder = product.UpperLimitPerOrder,
                         BuyLimitCount = product.BuyLimitCount,
                         TotalStockCount = product.TotalStockCount
                     });

                    DalGroupBuyingProductGroupConfig.InsertGroupBuyingTireProductConfig
                        (conn, new GroupBuyingTireProductConfigEntity
                        {
                            ProductConfigID = productConfigId,
                            TireBrand = productInfo.Brand,
                            TirePattern = tireProductInfo.Pattern,
                            TireWidth = productInfo.Size.Width,
                            TireAspectRatio = productInfo.Size.AspectRatio,
                            TireRim = productInfo.Size.Rim
                        });
                }
            }
        }

        /// <summary>
        /// 发送Mq通知以更新拼团产品状态
        /// </summary>
        /// <param name="pid">产品Id</param>
        /// <param name="productGroupId">产品组Id</param>
        private void SendMQ2UpdateProductStatus(string pid, string productGroupId)
        {
            TuhuNotification.SendNotification("#.PinTuanProductStatusSyncQueue.#",
                new Dictionary<string, object>
                {
                    ["PId"] = pid,
                    ["ProductGroupId"] = productGroupId
                }, 3000);
        }

        /// <summary>
        /// 计算轮胎商品成本价
        /// </summary>
        /// <param name="products">商品列表</param>
        private void ComputeTireProductsCostPrice(List<GroupBuyingTireProductConfigResponse> products)
        {
            var purchaseInfo = PurchaseService.SelectPurchaseInfoByPID(products.Select(x => x.PID).ToList());
            if (purchaseInfo != null && purchaseInfo.Any())
            {
                purchaseInfo = from c in purchaseInfo
                               group c by c.PID into g
                               select g.OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                products.ForEach(x =>
                {
                    var pidItem = purchaseInfo.Where(y => string.Equals(x.PID, y.PID)).FirstOrDefault();
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

        #endregion
    }
}
