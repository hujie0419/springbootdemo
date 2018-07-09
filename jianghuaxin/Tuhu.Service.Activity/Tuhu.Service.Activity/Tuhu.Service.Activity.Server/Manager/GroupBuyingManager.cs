using Common.Logging;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Model;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Request;
using Tuhu.Service.Product;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;
using Tuhu.Service.UserAccount;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class GroupBuyingManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GroupBuyingManager));
        private static readonly string IndexName = "groupbuying";
        //private static readonly string ProductIndexName = "groupbuyingproduct";

        private static readonly string PinTuanProductIndexName = "pintuanproduct";

        public static async Task<PagedModel<string>> GetGroupBuyingProductList(int pageIndex, int pageSize, bool flag, bool isOldUser,
            int groupType = -99, string channel = default(string))
        {
            var result = new PagedModel<string>();
            using (var cacheClient = CacheHelper.CreateCacheClient("GroupBuyingProductList"))
            {
                var cacheResult = await cacheClient.GetOrSetAsync($"index/{pageIndex}/{pageSize}/{flag}/{isOldUser}/{groupType}/{channel}",
                    () => GetGroupBuyingProductListFromDb(pageIndex, pageSize, flag,isOldUser, groupType, channel),
                    TimeSpan.FromMinutes(1));
                if (cacheResult.Success && cacheResult.Value?.Source != null)
                {
                    result = cacheResult.Value;
                }
                else
                {
                    if (!cacheResult.Success)
                    {
                        SetProcessLog();
                        Logger.Warn(
                            $"GetGroupBuyingProductList==>缓存获取失败==>{cacheResult?.Exception?.Message}/{cacheResult?.Exception}");
                    }
                    result = await GetGroupBuyingProductListFromDb(pageIndex, pageSize, flag, isOldUser, groupType, channel);
                }
            }

            return result;
        }

        private static async Task<PagedModel<string>> GetGroupBuyingProductListFromDb(int pageIndex, int pageSize,
            bool flag, bool isOldUser, int groupType = -99, string channel = default(string))
        {
            var result = new PagedModel<string>
            {
                Pager = new PagerModel
                {
                    PageSize = pageSize,
                    CurrentPage = pageIndex
                },
                Source = new List<string>()
            };
            result.Pager.Total = await DalGroupBuying.GetAvailableGroupBuyingCount(groupType, channel);
            if (result.Pager.Total > (pageIndex - 1) * pageSize && pageSize > 0)
                result.Source = await DalGroupBuying.GetAvailableGroupBuyingList(pageIndex, pageSize, flag, isOldUser, groupType, channel);
            return result;
        }

        public static async Task<List<string>> SelectGroupBuyingProductsById(string productGroupId)
            => await DalGroupBuying.SelectGroupBuyingProductsById(productGroupId);

        public static async Task<List<ProductGroupModel>> SelectProductGroupInfo(List<string> productGroupIds,
            bool isFilter = true)
        {
            var data = await GetOrSetGroupBuyingInfo(productGroupIds);

            var result = new List<ProductGroupModel>();
            foreach (var item in productGroupIds)
            {
                var value = data.FirstOrDefault(g => g.ProductGroupId == item);
                if (value != null)
                {
                    result.Add(value);
                }
            }

            return result;
        }

        public static async Task<List<ProductGroupModel>> SelectProductGroupDetail(string productGroupId)
        {
            var productGroupInfo = await SelectProductGroupInfo(new List<string> { productGroupId });
            var productInfo = new List<GroupBuyingProductModel>();
            var result = new List<ProductGroupModel>();
            if (productGroupInfo.Any())
            {
                var tmp = productGroupInfo.First();
                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GroupBuyingActivityName))
                {
                    var cache = await client.GetOrSetAsync($"GroupBuying/{productGroupId}",
                        () => DalGroupBuying.SelectProductInfoByPid(productGroupId), TimeSpan.FromMinutes(30));
                    if (cache.Success && cache.Value != null)
                    {
                        productInfo = cache.Value;
                    }
                    else
                    {
                        Logger.Warn($"查询拼团商品价格Redis缓存失败，GroupBuying/{productGroupId}");
                        productInfo = await DalGroupBuying.SelectProductInfoByPid(productGroupId);
                    }
                }

                foreach (var item in productInfo)
                {
                    var model = new ProductGroupModel
                    {
                        ProductGroupId = tmp.ProductGroupId,
                        Image = tmp.Image,
                        ShareId = tmp.ShareId,
                        GroupType = tmp.GroupType,
                        MemberCount = tmp.MemberCount,
                        Sequence = tmp.Sequence,
                        CurrentGroupCount = tmp.CurrentGroupCount,
                        BeginTime = tmp.BeginTime,
                        EndTime = tmp.EndTime,
                        LabelList = tmp.LabelList,
                        ActivityId = tmp.ActivityId,
                        ProductName = item.ProductName,
                        OriginalPrice = item.OriginalPrice,
                        FinalPrice = item.FinalPrice,
                        SpecialPrice = item.SpecialPrice,
                        PID = item.PID,
                        UseCoupon = item.UseCoupon,
                        UpperLimitPerOrder = item.UpperLimitPerOrder,
                        Label = tmp.Label,
                        ShareImage = tmp.ShareImage,
                        GroupDescription = tmp.GroupDescription,
                        GroupCategory = tmp.GroupCategory,
                        GroupOrderCount = tmp.GroupOrderCount
                    };
                    result.Add(model);
                }
            }

            return result;
        }


        public static async Task<GroupBuyingProductModel> SelectProductInfoByPid(string productGroupId, string pid)
        {
            var data = new List<GroupBuyingProductModel>();
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GroupBuyingActivityName))
            {
                var result = await client.GetOrSetAsync($"GroupBuying/{productGroupId}",
                    () => DalGroupBuying.SelectProductInfoByPid(productGroupId), TimeSpan.FromMinutes(30));
                if (result.Success && result.Value != null)
                {
                    data = result.Value;
                }
                else
                {
                    Logger.Warn($"查询拼团商品价格Redis缓存失败，GroupBuying/{productGroupId}");
                    data = await DalGroupBuying.SelectProductInfoByPid(productGroupId);
                }
            }

            return data.FirstOrDefault(g => g.PID == pid);
        }


        public static async Task<List<GroupBuyingCategoryModel>> GetGroupBuyingCategoryInfo()
        {
            var cacheKey = "GroupBuying/Category";
            var data = new List<GroupBuyingCategoryModel>();
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GroupBuyingCategoryKey))
            {
                var result = await client.GetOrSetAsync(cacheKey, DalGroupBuying.GetGroupBuyingCategoryInfo, TimeSpan.FromMinutes(10));
                if (result.Success && result.Value != null)
                {
                    data = result.Value;
                }
                else
                {
                    Logger.Warn($"查询拼团商品类目信息Redis缓存失败，{cacheKey}");
                    data = await DalGroupBuying.GetGroupBuyingCategoryInfo();
                }
            }
            return data;
        }


        public static async Task<PagedModel<SimplegroupBuyingModel>> GetGroupBuyingProductListNew(GroupBuyingQueryRequest request)
        {
            var cacheKey = $"GroupBuyingListNew/{request.PageIndex}/{request.PageSize}/{request.Channel}/{request.SortType}/{request.IsOldUser}/{request.NewCategoryCode}";
            var data = new PagedModel<SimplegroupBuyingModel>
            {
                Pager = new PagerModel { PageSize = request.PageSize, CurrentPage = request.PageIndex, Total = 0 }
            };
            using (var client= CacheHelper.CreateCacheClient(GlobalConstant.GroupBuyingListNewKey))
            {
                var result = await client.GetOrSetAsync(cacheKey, () => GetGroupBuyingListNew(request), TimeSpan.FromSeconds(10));
                if (result.Success && result.Value != null)
                {
                    data = result.Value;
                }
                else
                {
                    Logger.Warn($"查询拼团商品类目信息Redis缓存失败，{cacheKey}");
                    data = await GetGroupBuyingListNew(request);
                }
            }
            return data;
        }

        public static async Task<List<ProductGroupModel>> SelectProductListByPids(
            List<GroupBuyingProductRequest> request)
        {
            var result = new List<ProductGroupModel>();
            var data = await SelectProductGroupInfo(request.Select(g => g.ProductGroupId).ToList(), false);
            foreach (var item in request)
            {
                var tmp = data.FirstOrDefault(g => g.ProductGroupId == item.ProductGroupId);
                if (tmp == null) continue;
                var resultItem = CopyGroupInfo(tmp);

                var productInfo = await SelectProductInfoByPid(item.ProductGroupId, item.PId);
                if (productInfo == null) continue;
                resultItem.PID = productInfo.PID;
                resultItem.ProductName = productInfo.ProductName;
                resultItem.OriginalPrice = productInfo.OriginalPrice;
                resultItem.FinalPrice = productInfo.FinalPrice;
                resultItem.SpecialPrice = productInfo.SpecialPrice;
                resultItem.UseCoupon = productInfo.UseCoupon;
                resultItem.UpperLimitPerOrder = productInfo.UpperLimitPerOrder;
                result.Add(resultItem);
            }
            return result;
        }

        public static async Task<List<GroupInfoModel>> SelectGroupInfoByProductGroupId(string productGroupId,
            Guid userId, int count, bool flag = true)
        {
            var orderCount = 0;
            if (!flag && userId != Guid.Empty)
            {
                orderCount = (await CheckNewUser(userId)).Item2;
            }

            var data = (await DalGroupBuying.SelectGroupInfoByProductGroupId(productGroupId, userId,
                    flag || orderCount == 0, flag))
                .OrderByDescending(g => g.CurrentMemberCount)
                .ThenBy(g => g.StartTime).ToList();
            var values = new List<GroupInfoModel>();
            while (data.Any() && values.Count < count)
            {
                data = data.Except(values).ToList();
                values.AddRange(data.GroupBy(g => g.OwnerId).Select(g => g.First()));
            }

            return values.Take(count).ToList();

        }


        public static async Task<GroupInfoResponse> SelectGroupInfoWithTotalCount(string productGroupId,
            Guid userId, int count, bool flag = true)
        {
            using (var client = CacheHelper.CreateCacheClient("GroupBuyingList"))
            {
                var cacheResult = await client.GetOrSetAsync($"GroupBuyingListCache/{productGroupId}/{count}",
                    () => SelectGroupInfoWithTotalCountDeal(productGroupId, userId, count, flag),
                    TimeSpan.FromSeconds(30));
                GroupInfoResponse result;
                if (cacheResult.Success && cacheResult.Value != null)
                {
                    result = cacheResult.Value;
                }
                else
                {
                    result = await SelectGroupInfoWithTotalCountDeal(productGroupId, userId, count, flag);
                    Logger.Warn($"SelectGroupInfoWithTotalCount==>{productGroupId}/{userId}/{count}");
                }

                return result;
            }

        }

        private static async Task<GroupInfoResponse> SelectGroupInfoWithTotalCountDeal(string productGroupId,
            Guid userId, int count, bool flag = true)
        {
            var result = new GroupInfoResponse
            {
                TotalCount = await DalGroupBuying.GetGroupInfoCountByProductGroupId(productGroupId, userId, true, flag),
                Items = await SelectGroupInfoByProductGroupId(productGroupId, userId, count, flag)
            };
            return result;
        }

        public static async Task<GroupInfoModel> FetchGroupInfoByGroupId(Guid groupId)
        {
            var readOnly = !await GetGuidCache(groupId);
            return await DalGroupBuying.FetchGroupInfoByGroupId(groupId, readOnly);
        }

        public static async Task<GroupMemberModel> SelectGroupMemberByGroupId(Guid groupId)
        {
            var readOnly = !(await GetGuidCache(groupId));
            return await DalGroupBuying.SelectGroupMemberByGroupId(groupId, readOnly);
        }

        public static async Task<List<GroupMemberInfo>> GetGroupMemberInfoBy(Guid groupId)
        {
            var readOnly = !(await GetGuidCache(groupId));
            return await DalGroupBuying.GetGroupMemberInfoBy(groupId, readOnly);
        }

        public static async Task<VerificationResultModel> CreateGroupBuyingInfo(Guid userId, string productGroupId,
            string pid, int orderId, Guid groupId, bool flag = false)
        {
            var dat = await DalGroupBuying.CheckProductGroupInfo(productGroupId);

            if (groupId == Guid.Empty)
            {
                groupId = Guid.NewGuid();
            }

            if (dat.Code != 1 && dat.Code != 10)
            {
                if (dat.Code == 2)
                {
                    dat.Info = "商品已抢完";
                }
                else if (dat.Code == 3)
                {
                    dat.Info = "商品已下架";
                }

                return dat;
            }

            if (dat.Code == 10 && !await CheckSpecialUsers(productGroupId, userId))
            {
                return new VerificationResultModel
                {
                    Code = 10,
                    Info = "只有特定用户可见"
                };
            }

            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                var val = await DalGroupBuying.CreateGroupBuyingGroupInfo(userId, productGroupId, groupId, dbHelper);
                if (val > 0)
                {
                    if (!flag)
                        flag = await DalGroupBuying.CreateGroupBuyingUserInfo(userId, groupId, pid, orderId, dbHelper) >
                               0;

                    if (flag)
                    {
                        dbHelper.Commit();
                        await SetGuidCache(groupId);
                        await SetGuidCache(userId);
                        return new VerificationResultModel
                        {
                            Code = 1,
                            Info = "创建成功"
                        };
                    }
                }

                dbHelper.Rollback();
                return new VerificationResultModel
                {
                    Code = 0,
                    Info = "数据创建失败"
                };
            }
        }


        public static async Task<VerificationResultModel> TakePartInGroupBuying(Guid userId, Guid groupId, String pid,
            int orderId)
        {
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                var val = await DalGroupBuying.CreateGroupBuyingUserInfo(userId, groupId, pid, orderId, dbHelper);
                if (val > 0)
                {
                    await SetGuidCache(userId);
                    return new VerificationResultModel
                    {
                        Code = 1,
                        Info = "创建成功"
                    };
                }

                return new VerificationResultModel
                {
                    Code = 0,
                    Info = "数据创建失败"
                };
            }
        }

        public static async Task<PagedModel<UserGroupBuyingInfoModel>>
            GetUserGroupInfoByUserId(GroupInfoRequest request)
        {
            var result = await DalGroupBuying.GetUserGroupInfoByUserId(request, Logger);
            var data = result.Source?.ToList() ?? new List<UserGroupBuyingInfoModel>();
            //var productGroupInfo = await GetAllProductGroupCache();
            var productGroupInfo = await GetOrSetGroupBuyingInfo(data.Select(g => g.ProductGroupId).ToList());
            foreach (var item in data)
            {
                var productInfo = await SelectProductInfoByPid(item.ProductGroupId, item.PID);
                var dat = productGroupInfo.FirstOrDefault(g => g.ProductGroupId == item.ProductGroupId);
                if (dat != null && productInfo != null)
                {
                    item.Image = dat.Image;
                    item.ShareId = dat.ShareId;
                    item.GroupType = dat.GroupType;
                    item.MemberCount = item.CurrentMemberCount > dat.MemberCount
                        ? item.CurrentMemberCount
                        : dat.MemberCount;
                    item.Sequence = dat.Sequence;
                    item.CurrentGroupCount = dat.CurrentGroupCount;
                    item.Label = dat.Label;
                    item.ActivityId = dat.ActivityId;
                    item.IsShow = dat.IsShow;
                    //item.LabelList = item.Label.Split(';').Where(g => !string.IsNullOrWhiteSpace(g)).ToList();
                    item.GroupCategory = dat.GroupCategory;
                    item.GroupOrderCount = dat.GroupOrderCount;
                    item.GroupDescription = dat.GroupDescription;
                    // pid产品信息
                    item.PID = productInfo.PID;
                    item.ProductName = productInfo.ProductName;
                    item.OriginalPrice = productInfo.OriginalPrice;
                    item.FinalPrice = productInfo.FinalPrice;
                    item.SpecialPrice = productInfo.SpecialPrice;
                    item.UpperLimitPerOrder = productInfo.UpperLimitPerOrder;
                    item.UseCoupon = productInfo.UseCoupon;
                }
            }

            result.Source = data;
            return result;
        }


        public static async Task<GroupBuyingHistoryCount> GetUserGroupCountByUserId(Guid userId)
            => await DalGroupBuying.GetUserGroupCountByUserId(userId);

        public static async Task<UserOrderInfoModel> FetchUserOrderInfo(Guid groupId, Guid userId, bool readOnly = true)
            => await DalGroupBuying.FetchUserOrderInfo(groupId, userId, readOnly);

        public static async Task<GroupInfoModel> FetchGroupInfoByOrderId(int orderId)
            => await DalGroupBuying.FetchGroupInfoByOrderId(orderId);

        public static async Task<List<GroupFinalUserModel>> GetGroupFinalUserList(Guid groupId)
            => await DalGroupBuying.GetGroupFinalUserList(groupId);

        public static async Task<ProductGroupModel> FetchProductGroupInfoById(string productGroupId)
        {
            var data = await DalGroupBuying.FetchProductGroupInfoById(productGroupId);
            if (data != null)
            {
                data.LabelList = data.Label?.Split(';').Where(g => !string.IsNullOrWhiteSpace(g)).ToList();
                //data.ActivityId = GlobalConstant.GroupBuyingActivityId[data.GroupType];
            }

            return data;
        }

        public static async Task<VerificationResultModel> ExpireGroupBuying(Guid groupId)
        {
            var data = await DalGroupBuying.SelectGroupBuyingUserByGroupId(groupId);
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                var dat = await DalGroupBuying.SetGroupStatus(dbHelper, groupId);
                if (!dat)
                {
                    dbHelper.Rollback();
                    Logger.Warn($"将团号为{groupId:D}的团设置为过期失败");
                    return new VerificationResultModel
                    {
                        Code = 0,
                        Info = $"将团号为{groupId:D}的团设置为过期失败"
                    };
                }

                dat = await DalGroupBuying.SetProductGroupStatus(dbHelper, groupId);
                if (!dat)
                {
                    dbHelper.Rollback();
                    Logger.Warn($"拼团过期，设置产品拼团数量失败，{groupId:D}");
                    return new VerificationResultModel
                    {
                        Code = 0,
                        Info = $"拼团过期，设置产品拼团数量失败，{groupId:D}"
                    };
                }

                dat = await DalGroupBuying.SetUserStatus(dbHelper, groupId);
                if (!dat)
                {
                    dbHelper.Rollback();
                    Logger.Warn($"拼团过期，设置用户状态失败-->{groupId:D}");
                }
                dbHelper.Commit();

            }

            // 释放团长免单券
            var groupInfo = await DalGroupBuying.GetOwnerAndGPIdByGroupId(groupId);
            if (groupInfo != null && groupInfo.Item2 != Guid.Empty && groupInfo.Item1 == 3)
            {
                var orderId = data.FirstOrDefault(g => g.UserId == groupInfo.Item2)?.OrderId;
                if (orderId != null)
                {
                    var releaseResult = await DalGroupBuying.ReleaseFreeCoupon(groupInfo.Item1);
                    if (!releaseResult) Logger.Warn($"{groupId:D}拼团过期，团长免单券释放失败");
                    else
                    {
                        await SetFreeCouponCache(groupInfo.Item2);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(groupInfo?.Item3)) await RefreshCacheAsync(groupInfo.Item3);
            foreach(var item in data)
            {
                await SetGuidCache(item.UserId);
            }

            // 消息推送
            var pushResult = await PushPinTuanMessage(groupId, 1653);
            if (!pushResult)
            {
                Logger.Warn($"ExpireGroupBuying==>{groupId:D}==>拼团过期==>消息推送失败");
            }
            // 取消订单
            if (data.Any())
            {
                CancelUserOrder(data);
                data.ForEach(g =>
                {
                    TuhuNotification.SendNotification("notification.GroupByingToBigBrand", new
                    {
                        UserId = g.UserId.ToString("D"),
                        Type = "2GroupFaild" //1JoinGroupSuccess(参团成功),2GroupFaild(拼团失败),3OpenGroupSuccess(开团成功)
                    });
                });
            }
            await SetGuidCache(groupId);
            return new VerificationResultModel
            {
                Code = 1,
                Info = $"拼团过期,设置状态成功,{groupId:D}"
            };
        }

        private static void CancelUserOrder(List<UserOrderModel> dataList)
        {
            Logger.Info($"拼团失败，取消以下订单,{JsonConvert.SerializeObject(dataList)}");
            using (var client = new OrderOperationClient())
            {
                foreach (var item in dataList)
                {
                    var request = new CancelOrderRequest
                    {
                        OrderId = item.OrderId,
                        UserID = item.UserId,
                        Remark = "拼团失败",
                        FirstMenu = "拼团失败",
                        SecondMenu = "拼团超时"
                    };

                    var result = client.CancelOrderForApp(request);
                    if (!(result.Success && result.Result.IsSuccess))
                    {
                        if (result.Exception != null)
                        {
                            Logger.Warn($"订单取消失败，{item.UserId}/{item.OrderId}", result.Exception);
                        }
                        else if (result.Result != null && result.Result.RespCode != 1)
                        {
                            Logger.Warn($"订单取消失败,{result.Result.Message}");
                        }
                    }
                }
            }
        }


        public static async Task<VerificationResultModel> CancelGroupBuyingOrder(Guid groupId, int orderId,
            Guid userId)
        {
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                dbhelper.BeginTransaction();
                var dat1 = await DalGroupBuying.SetUserOrderCancel(orderId, dbhelper);
                if (dat1 == 1)
                {
                    var dat2 = await DalGroupBuying.SetGroupStatus(groupId, true, dbhelper);
                    if (dat2 < 1)
                    {
                        Logger.Warn($"设置团{groupId}的团成员数量失败");
                        dbhelper.Rollback();
                        return new VerificationResultModel
                        {
                            Code = 0,
                            Info = "设置失败"
                        };
                    }
                }
                dbhelper.Commit();

                if (dat1 < 2 && userId != Guid.Empty)
                {
                    CancelUserOrder(new List<UserOrderModel>
                        {
                            new UserOrderModel
                            {
                                OrderId = orderId,
                                UserId = userId
                            }
                        });
                }

                // 释放免单券
                var releaseResult = await DalGroupBuying.ReleaseFreeCoupon(orderId);
                if (releaseResult)
                {
                    await SetFreeCouponCache(userId);
                }
                await SetGuidCache(userId);
                await SetGuidCache(groupId);
                return new VerificationResultModel
                {
                    Code = 1,
                    Info = "设置成功"
                };
            }
        }

        public static async Task<bool> ChangeGroupBuyingStatus(Guid groupId, int orderId, bool tag = true)
        {
            var data = await FetchGroupInfoByGroupId(groupId);
            if (data == null)
            {
                return false;
            }

            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                var flag = await DalGroupBuying.SetProductGroupStatus2(groupId, dbHelper);
                if (flag)
                {
                    var dat = await DalGroupBuying.SetGroupStatus2(groupId, dbHelper);
                    if (dat)
                    {
                        dbHelper.Commit();
                        await SetGuidCache(groupId);

                        if (data.GroupType == 3 && tag)
                        {
                            var result = await DalGroupBuying.RelateFreeCouponAndOrderId(orderId, data.OwnerId);
                            if (!result)
                            {
                                Logger.Info($"FreeCoupons==>{orderId}/{groupId:D}==>未使用免单券");
                            }
                            else
                            {
                                await SetFreeCouponCache(data.OwnerId);
                            }

                        }

                        await RefreshCacheAsync(data.ProductGroupId);
                        return true;
                    }
                }

                dbHelper.Rollback();
                CancelUserOrder(new List<UserOrderModel>
                {
                    new UserOrderModel
                    {
                        OrderId = orderId,
                        UserId = data.OwnerId
                    }
                });

                return false;
            }
        }


        public static async Task<bool> ChangeUserStatus(Guid groupId, Guid userId, int orderId)
        {
            var data = await FetchGroupInfoByGroupId(groupId);
            if (data.GroupStatus > 1)
            {
                return await ChangeUserGroupId(orderId, groupId, userId, data.ProductGroupId, data.OwnerId);
            }

            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                var setUserStatus = await DalGroupBuying.SetUserStatus2(orderId, dbHelper);
                if (setUserStatus)
                {
                    var setMemberCount = await DalGroupBuying.SetGroupBuyingUserCount(groupId, dbHelper);
                    if (setMemberCount)
                    {
                        // 检查是否拼团成功
                        var setGroupStatus = DalGroupBuying.SetGroupBuyingFinish(groupId, dbHelper);

                        dbHelper.Commit();


                        await SetGuidCache(groupId);
                        if (setGroupStatus)
                        {
                            var memberList = await ChangeGroupOrderStatus(groupId, data.GroupCategory);

                            // 发放团长免单券
                            var freeCouponResult =
                                await DalGroupBuying.CreateFreeCoupons(groupId,
                                    GlobalConstant.GroupBuyingFreeCouponSpan);
                            if (!freeCouponResult)
                            {
                                Logger.Warn($"{groupId:D}拼团成功，发放免单券失败");
                            }

                            foreach (var item in memberList)
                            {
                                await SetFreeCouponCache(item.UserId);
                            }

                            // 拼团优惠券
                            if (data.GroupCategory == 2)
                            {
                                TuhuNotification.SendNotification("notification.GroupBuyingCreateCouponQueue", new Dictionary<string, object>
                                {
                                    ["GroupId"] = groupId,
                                    ["ProductGroupId"] = data.ProductGroupId
                                }, 10000);
                            }

                            // 拼团成功推送
                            var finish = await PushMessagDeal(groupId, userId, 1652);
                            if (!finish) Logger.Warn($"团{groupId:D}完成，推送消息出现异常");

                            // 大翻盘MQ消息
                            TuhuNotification.SendNotification("notification.GroupByingToBigBrand", new
                            {
                                UserId = userId.ToString("D"),
                                Type = "1JoinGroupSuccess" //1JoinGroupSuccess(参团成功),2GroupFaild(拼团失败),3OpenGroupSuccess(开团成功)
                            });
                        }
                        else if (userId == data.OwnerId && data.CurrentMemberCount < 2)
                        {
                            // 团长付款,该团可见-推送
                            var finish = await PushMessagDeal(groupId, data.OwnerId, 1649);
                            if (!finish) Logger.Warn($"团{groupId:D}团长付款，推送消息出现异常");

                            TuhuNotification.SendNotification("notification.GroupByingToBigBrand", new
                            {
                                UserId = data.OwnerId.ToString("D"),
                                Type = "3OpenGroupSuccess" //1JoinGroupSuccess(参团成功),2GroupFaild(拼团失败),3OpenGroupSuccess(开团成功)
                            });
                        }
                        else
                        {
                            // 用户参团成功-推送
                            var finish = await PushMessagDeal(groupId, userId, 1650);
                            if (!finish) Logger.Warn($"团{groupId:D}用户加入，推送消息出现异常");

                            // 大翻盘MQ消息
                            TuhuNotification.SendNotification("notification.GroupByingToBigBrand", new
                            {
                                UserId = userId.ToString("D"),
                                Type = "1JoinGroupSuccess" //1JoinGroupSuccess(参团成功),2GroupFaild(拼团失败),3OpenGroupSuccess(开团成功)
                            });
                        }

                        return true;
                    }
                }

                dbHelper.Rollback();
                CancelUserOrder(new List<UserOrderModel>
                {
                    new UserOrderModel
                    {
                        OrderId = orderId,
                        UserId = data.OwnerId
                    }
                });
                return false;
            }
        }


        private static async Task<bool> ChangeUserGroupId(int orderId, Guid groupId, Guid userId, string productGroupId,
            Guid ownerId)
        {
            var dat = await SelectGroupInfoByProductGroupId(productGroupId, userId, 10, false);
            if (dat.Any())
            {
                var item = dat.First().GroupId;
                var dat2 = await DalGroupBuying.ChangeUserGroupId(item, orderId);
                if (dat2)
                {
                    var dat3 = await ChangeUserStatus(item, userId, orderId);
                    if (!dat3)
                    {
                        Logger.Warn($"重新分配用户{userId}拼团失败{groupId}-{item}");
                        return false;
                    }
                }
            }
            else
            {
                // 开新团
                var newGroupId = Guid.NewGuid();
                var dat2 = await CreateGroupBuyingInfo(ownerId, productGroupId, "", orderId, newGroupId, true);
                if (dat2.Code == 1)
                {
                    var data = await DalGroupBuying.ChangeUserGroupId(newGroupId, orderId);
                    if (data)
                    {
                        var dat3 = await ChangeGroupBuyingStatus(newGroupId, orderId, false);
                        if (dat3)
                        {
                            var dat4 = await ChangeUserStatus(newGroupId, userId, orderId);
                            if (dat4)
                            {
                                Logger.Info($"订单{orderId}加入新团{newGroupId}成功");
                                return true;
                            }
                        }
                    }
                }

                Logger.Info($"订单{orderId}加入新团{newGroupId}失败");
                var datt = await CancelGroupBuyingOrder(newGroupId, orderId, userId);
                if (datt.Code != 1)
                {
                    Logger.Warn($"取消订单失败{orderId}");
                }

                return false;
            }

            return true;
        }


        private static async Task<List<UserOrderModel>> ChangeGroupOrderStatus(Guid groupId, int groupCategory)
        {
            var data = DalGroupBuying.ChangeGroupOrderStatus(groupId);
            if (data.Any())
            {
                Logger.Info($"{groupId}拼团完成，以下订单继续往下走{JsonConvert.SerializeObject(data)}");


                using (var client = new OrderOperationClient())
                {
                    foreach (var item in data)
                    {
                        var dat = await client.ExecuteOrderProcessAsync(new ExecuteOrderProcessRequest
                        {
                            OrderId = item.OrderId,
                            CreateBy = item.UserId.ToString("D"),
                            OrderProcessEnum = OrderProcessEnum.PinTuanSuccess
                        });
                        if (!dat.Success)
                        {
                            Logger.Warn("拼团完成，调用接口订单继续进行，出现异常", dat.Exception);
                        }
                        else if (string.IsNullOrWhiteSpace(dat.Result))
                        {
                            Logger.Warn($"拼团完成，调用接口订单继续进行，未成功。{dat.Result}");
                        }
                    }
                }

                // 拼团抽奖
                if (groupCategory == 1)
                {
                    var result = DalGroupBuying.SetGroupBuyingLottery(groupId);
                    Logger.Info($"GroupLottery==>{result}位用户拼团完成，进入抽奖列表");
                }
            }

            return data;
        }

        #region [拼团推送]

        public static async Task<bool> PushPinTuanMessage(Guid groupId, int batchId)
        {
            return await PushMessagDeal(groupId, Guid.Empty, batchId);
        }

        private static async Task<bool> PushMessagDeal(Guid groupId, Guid userId,
            int batchId)
        {
            var groupInfo = await FetchGroupInfoByGroupId(groupId);
            var memberInfo = await GetGroupMemberInfoBy(groupId);
            if (memberInfo == null || !memberInfo.Any()) return false;

            var nickNameList = await GetGroupBuyingUserName(memberInfo.Select(g => g.UserId).ToList()) ??
                               new List<Tuple<Guid, string>>();

            var memberNickNames = string.Join(",", nickNameList.Select(g => g.Item2));
            var newMemberNickName = nickNameList.FirstOrDefault(g => g.Item1 == userId)?.Item2 ?? "";
            var leftTime = groupInfo.StartTime + TimeSpan.FromHours(24) - DateTime.Now + TimeSpan.FromMinutes(30);
            var groupLeftTime = $"{leftTime.Days * 24 + leftTime.Hours}小时";
            var requireCount = groupInfo.RequiredMemberCount;
            var currentCount = groupInfo.CurrentMemberCount;
            //data
            var finalResult = true;
            foreach (var item in memberInfo)
            {
                //var dat = await FetchUserOrderInfo(groupId, item, false);
                var nickName = nickNameList.FirstOrDefault(g => g.Item1 == item.UserId)?.Item2 ?? "";
                var target = item.UserId.ToString("D");

                var templateLog = new PushTemplateLog
                {
                    Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        {"{{productname}}", item.ProductName},
                        {"{{groupmemberno}}", currentCount.ToString()},
                        {"{{groupmemberleftno}}", (requireCount - currentCount).ToString()},
                        {"{{newmembernickname}}", newMemberNickName},
                        {"{{grouplefttime}}", groupLeftTime},
                        {"{{membernicknames}}", memberNickNames},
                        {"{{groupid}}", groupId.ToString("D")},
                        {"{{orderid}}", $"{item.OrderId:D8}"},
                        {"{{nickname}}", nickName},
                        {"{{productgroupid}}", groupInfo.ProductGroupId},
                        {"{{pid}}", item.PID}
                    })
                };

                // 当新团员加入时，新成员推1769模板，其他成员推1650模板
                var realBatchId = batchId == 1650 && item.UserId == userId ? 1769 : batchId;
                var client = new TemplatePushClient();

                try
                {
                    var result =
                        await client.PushByUserIDAndBatchIDAsync(new List<string> { target }, realBatchId, templateLog);
                    result.ThrowIfException(true);
                    if (!(result.Success && result.Result))
                    {
                        Logger.Warn(
                            $"向用户{JsonConvert.SerializeObject(target)}推送信息{realBatchId}/{JsonConvert.SerializeObject(templateLog)},推送失败",
                            result.Exception);
                        finalResult = false;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn($"向用户{JsonConvert.SerializeObject(target)}推送信息{realBatchId}出现异常", ex);
                    finalResult = false;
                }
                finally
                {
                    client.Dispose();
                }
            }

            return finalResult;
        }

        #endregion

        #region [缓存]

        private static async Task SetGuidCache(Guid value)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GroupBuyingActivityName))
            {
                var result = await client.SetAsync($"PinTuan/{value}", true, TimeSpan.FromSeconds(10));
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"PinTuan/{value}是否变更缓存失败：{ex}");
                }
            }
        }

        private static async Task<bool> GetGuidCache(Guid value)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GroupBuyingActivityName))
            {
                var result = await client.GetAsync<bool>($"PinTuan/{value}");
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"获取PinTuan/{value}是否变更缓存失败：{ex}");
                }

                return result.Value;
            }
        }

        private static async Task SetFreeCouponCache(Guid userId)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GroupFreeCouponCacheName))
            {
                var result = await client.SetAsync($"FreeCoupon/{userId:D}", true, GlobalConstant.GroupFreeCouponSpan);
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"FreeCoupon/{userId:D}是否变更缓存失败：{ex}");
                }
            }
        }

        private static async Task<bool> GetFreeCouponCache(Guid userId)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GroupFreeCouponCacheName))
            {
                var result = await client.GetAsync<bool>($"FreeCoupon/{userId:D}");
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"获取FreeCoupon/{userId:D}是否变更缓存失败：{ex}");
                }

                return result.Value;
            }
        }

        #endregion

        #region [用户信息查询/校验]

        public static async Task<CheckResultModel> CheckGroupInfoByUserId(Guid groupId, Guid userId,
            string productGroupId, string pid)
        {
            var readOnly = !await GetGuidCache(groupId);
            var orderId = await DalGroupBuying.CheckUserGroupInfo(groupId, userId, Logger, readOnly);
            if (orderId > 1)
            {
                return new CheckResultModel
                {
                    Code = 5,
                    Info = "用户已参团",
                    OrderId = orderId
                };
            }

            #region 新人校验

            var orderCount = 0;
            var flag = false;
            if (userId != Guid.Empty)
            {
                var result = await CheckNewUser(userId);
                orderCount = result.Item2;
                flag = result.Item1 == userId;
            }

            var groupInfo = await FetchGroupInfoByGroupId(groupId);
            if (groupInfo.GroupType == 1 && orderCount != 0)
            {
                return new CheckResultModel
                {
                    Code = flag ? 6 : 9,
                    Info = "该团为新人团，用户没有资格加入"
                };
            }

            #endregion

            if (groupInfo.EndTime < DateTime.Now)
            {
                return new CheckResultModel
                {
                    Code = 7,
                    Info = "该团已过期"
                };
            }

            if (groupInfo.GroupStatus != 1)
            {
                return new CheckResultModel
                {
                    Code = 8,
                    Info = "该团已完成"
                };
            }

            // 为暂时兼容线上，当pid为空时不检测限购
            if (!string.IsNullOrWhiteSpace(pid))
            {
                var limitInfo = await GetUserBuyLimitInfo(pid, productGroupId, userId);
                if (limitInfo != null && limitInfo.BuyLimitCount != 0 && limitInfo.CurrentOrderCount >= limitInfo.BuyLimitCount)
                {
                    return new CheckResultModel
                    {
                        Code = 11,
                        Info = "已达到限购单数"
                    };
                }
            }

            return new CheckResultModel
            {
                Code = 1,
                Info = "该团可参加"
            };
        }

        public static async Task<CheckResultModel> CheckProductGroupInfoById(string productGroupId, Guid userId,
            string pid)
        {

            var limitInfo = await GetUserBuyLimitInfo(pid, productGroupId, userId);
            var result = new CheckResultModel
            {
                Code = 11,
                Info = "已达到限购单数"
            };
            if (string.IsNullOrWhiteSpace(pid) || (limitInfo != null &&
                 (limitInfo.BuyLimitCount == 0 || limitInfo.BuyLimitCount > limitInfo.CurrentOrderCount)))
            {
                var data = await DalGroupBuying.CheckProductGroupInfo(productGroupId);

                if (data.Code == 10 && await CheckSpecialUsers(productGroupId, userId)) // 特殊用户对象校验
                {
                    result.Code = 1;
                    result.Info = "校验通过";
                }
                else
                {
                    result.Code = data.Code;
                    result.Info = data.Info;
                }
            }

            return result;

        }

        public static async Task<Tuple<Guid, int>> CheckNewUser(Guid userId)
        {
            var userList = new List<Guid> { userId };
            var relativeList = await DalGroupBuying.GetRelaedUserId(userId);
            if (relativeList.Any())
            {
                userList.AddRange(relativeList.Where(g => g != userId)?.ToList());
            }

            using (var client = new UserProfileClient())
            {
                foreach (var item in userList)
                {
                    var result = await client.GetAsync(item, "CreatedOrderQTY");
                    if (result.Success)
                    {
                        var orderCount = Convert.ToInt32(result.Result);
                        if (orderCount > 0)
                        {
                            Logger.Info($"查询到{userId}的关联Userid-{item}订单数为{orderCount},不是新用户");
                            return new Tuple<Guid, int>(item, orderCount);
                        }
                    }
                    else
                    {
                        Logger.Warn($"查询用户订单数失败,UserId--{item}", result.Exception);
                    }
                }

                Logger.Info($"查询到{userId}及其关联UserId的订单数都为0,是新用户");
                return new Tuple<Guid, int>(Guid.Empty, 0);
            }
        }

        public static async Task<bool> CheckProductOnSale(string pid)
        {
            using (var client = new ProductClient())
            {
                var result = await client.SelectSkuProductListByPidsAsync(new List<string> { pid });
                if (result.Success && result.Result.Any())
                {
                    return result.Result.FirstOrDefault()?.Onsale ?? false;
                }
                Logger.Warn($"CheckProductOnSale-->{pid}上下架状态查询异常，{result.Exception?.InnerException}");
                return false;
            }
        }
        private static async Task<bool> CheckSpecialUsers(string productGroupId, Guid userId)
        {
            var phoneNumber = "";
            using (var client = new UserAccountClient())
            {
                var accountInfo = await client.GetUserByIdAsync(userId);
                if (accountInfo.Success && accountInfo.Result != null)
                {
                    phoneNumber = accountInfo.Result.MobileNumber;
                }
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var tagNo = await DalGroupBuying.FetchTagNoById(productGroupId);
                if (tagNo != 0)
                {
                    return await DalGroupBuying.CheckSpecialUserTag(tagNo, phoneNumber);
                }
            }

            return false;
        }

        private static async Task<List<Tuple<Guid, string>>> GetGroupBuyingUserName(List<Guid> userLists)
        {
            var result = new List<Tuple<Guid, string>>();
            using (var client = new UserAccountClient())
            {
                var searchResule = await client.GetUsersByIdsAsync(userLists);
                if (searchResule.Success && searchResule.Result.Any())
                {
                    foreach (var item in searchResule.Result)
                    {
                        result.Add(new Tuple<Guid, string>(item.UserId, item.Profile.NickName));
                    }
                }
                else
                {
                    Logger.Warn($"GetGroupBuyingUserName==>{string.Join(",", userLists)}");
                }

                return result;
            }
        }

        #endregion

        public static async Task<GroupBuyingProductInfo> GetProductGroupInfoByPId(string pId)
            => await DalGroupBuying.GetProductGroupInfoByPId(pId);

        public static async Task<bool> CheckProductGroupId(Guid activityId)
            => await DalGroupBuying.CheckProductGroupId(activityId);

        public static async Task<GroupLotteryRuleModel> GetLotteryRule(string productGroupId)
            => await DalGroupBuying.GetLotteryRule(productGroupId);

        public static async Task<PagedModel<GroupBuyingLotteryInfo>> GetWinnerList(string productGroupId, int level,
            int pageIndex, int pageSize)
        {
            var result = new PagedModel<GroupBuyingLotteryInfo>
            {
                Pager = new PagerModel
                {
                    PageSize = pageSize,
                    CurrentPage = pageIndex
                }
            };
            result.Pager.Total = await DalGroupBuying.GetWinnerCount(productGroupId, level);
            result.Source = await DalGroupBuying.GetWinnerList(productGroupId, level, pageIndex, pageSize);
            return result;
        }

        public static async Task<GroupBuyingLotteryInfo> CheckUserLotteryResult(Guid userId, string productGroupId,
            int orderId)
            => await DalGroupBuying.CheckUserLotteryResult(userId, productGroupId, orderId);

        public static async Task<List<GroupBuyingLotteryInfo>> GetUserLotteryHistory(Guid userId, List<int> orderIds)
        {
            var result = await DalGroupBuying.GetUserLotteryHistory(userId);
            if (result.Any() && orderIds != null && orderIds.Any())
            {
                result = result.Where(g => orderIds.Contains(g.OrderId)).ToList();
            }

            return result;
        }

        public static async Task<List<FreeCouponModel>> GetUserFreeCoupon(Guid userId)
        {
            var readOnly = !(await GetFreeCouponCache(userId));
            return await DalGroupBuying.GetUserFreeCoupon(userId, readOnly);
        }


        public static async Task<BuyLimitAndOrderLimitModel> GetBuyLimitInfo(Guid activityId, string pid, Guid userId)
        {
            var result = await DalGroupBuying.GetBuyLimitStaticInfo(activityId, pid);   
            if (result != null)
            {
                var readOnly = !await GetGuidCache(userId);
                var limitInfo = await DalGroupBuying.GetUserBuyLimitInfo(pid, result.ProductGroupId, userId, readOnly);
                result.UserId = userId;
                result.CurrentOrderCount = limitInfo.CurrentOrderCount;
            }
            return result;
        }

        public static async Task<GroupBuyingBuyLimitModel> GetUserBuyLimitInfo(string pid, string productGroupId, Guid userId)
        {
            var result = await DalGroupBuying.GetUserBuyInfo(pid, productGroupId);
            if (result != null)
            {
                var readOnly = !await GetGuidCache(userId);
                var limitInfo = await DalGroupBuying.GetUserBuyLimitInfo(pid, productGroupId, userId, readOnly);
                result.UserId = userId;
                result.CurrentOrderCount = limitInfo.CurrentOrderCount;
            }
            return result;
        }

        #region 缓存优化

        private static async Task<List<ProductGroupModel>> GetOrSetGroupBuyingInfo(List<string> productGroupIds)
        {
            using (var hashClient =
                CacheHelper.CreateHashClient(GlobalConstant.GroupBuyingHashCacheName, TimeSpan.FromDays(1)))
            {
                var cacheResult = await hashClient.GetAsync<ProductGroupModel>(productGroupIds);
                var result = new List<ProductGroupModel>();
                List<string> unfoundKey;
                if (cacheResult.Success && cacheResult.Value != null)
                {
                    unfoundKey = productGroupIds.Except(cacheResult.Value.Keys).ToList();
                    result = cacheResult.Value.Values.ToList();
                }
                else
                {
                    unfoundKey = productGroupIds;
                    Logger.Warn($"GroupBuyingProductGroupInfoCache==>fail==>{string.Join("/", unfoundKey)}");
                }

                if (unfoundKey.Any())
                {
                    var data = await GetProductGroupInfo(unfoundKey);
                    if (data.Any())
                    {
                        result.AddRange(data);
                        var dic = new Dictionary<string, object>();
                        data.ForEach(g => dic.Add(g.ProductGroupId, g));
                        var cacheData = new ReadOnlyDictionary<string, object>(dic);
                        await hashClient.SetAsync<ProductGroupModel>(cacheData);
                    }
                }

                return result;
            }
        }

        private static async Task<List<ProductGroupModel>> GetProductGroupInfo(List<string> productGroupIds)
        {
            var data = await DalGroupBuying.GetGroupBuyingProductList(productGroupIds);
            //var userList = await DalGroupBuying.GetGroupBuyingUserList(productGroupIds);
            var userCount = await DalGroupBuying.GetGroupBuyingUserCountList(productGroupIds);
            if (data.Any())
            {
                var dat = data.ToList();
                dat.ForEach(g =>
                {
                    //g.LabelList = g.Label.Split(';').Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
                    //g.GroupUserList = userList.Where(t => t.ProductGroupId == g.ProductGroupId).Select(t => t.UserId).Take(5).ToList();
                    g.GroupOrderCount = userCount.FirstOrDefault(t => g.ProductGroupId == t.ProductGroupId)?.Count ?? 0;
                });
                return dat;
            }

            return data;
        }

        public static async Task<VerificationResultModel> RefreshCacheAsync(string productGroupId = null)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GroupBuyingActivityName))
            {
                if (!string.IsNullOrWhiteSpace(productGroupId))
                {
                    // 移除产品缓存
                    var result2 = await client.RemoveAsync($"GroupBuying/{productGroupId}");
                    if (!result2.Success)
                    {
                        Logger.Warn($"清除缓存失败，{GlobalConstant.GroupBuyingActivityName}/GroupBuying/{productGroupId}");
                    }
                }
            }

            using (var hashClient =
                CacheHelper.CreateHashClient(GlobalConstant.GroupBuyingHashCacheName, TimeSpan.FromDays(1)))
            {
                List<string> parameterList;
                if (string.IsNullOrWhiteSpace(productGroupId))
                {
                    parameterList = (await hashClient.GetAllAsync())?.Value?.Keys.ToList() ?? new List<string>();
                }
                else
                {
                    parameterList = new List<string> { productGroupId };
                }

                if (parameterList.Any())
                {
                    var data = await GetProductGroupInfo(parameterList);
                    if (data.Any())
                    {
                        var dic = new Dictionary<string, object>();
                        data.ForEach(g => dic.Add(g.ProductGroupId, g));
                        var cacheData = new ReadOnlyDictionary<string, object>(dic);
                        await hashClient.SetAsync<ProductGroupModel>(cacheData);
                    }
                }
            }

            return new VerificationResultModel
            {
                Code = 1
            };

        }

        #endregion

        #region ES数据

        public static async Task<bool> UpdateGroupBuyingInfo(List<string> productGroupIds)
        {
            var client = ElasticsearchHelper.CreateClient();
            if(!await CheckGroupBuyingIndex(client))
            {
                Logger.Error($"CheckGroupBuyingIndex-->失败");
                return false;
            }
            if (!await CheckProductGroupBuyingIndex(client))
            {
                Logger.Error($"CheckProductGroupBuyingIndex-->失败");
                return false;
            }
            var result = await UpdateGroupBuyingGroupInfo(client,productGroupIds);
            if (result)
            {
                result = await UpdateGroupBuyingProductInfo(client, productGroupIds);
                if (!result)
                {
                    Logger.Warn($"更新拼团产品信息失败-->{string.Join("/", productGroupIds)}");
                }
            }
            else
            {
                Logger.Warn($"更新拼团团信息失败-->{string.Join("/", productGroupIds)}");
            }
            return result;
        }

        private static Task<bool> CheckGroupBuyingIndex(IElasticClient client)
            => client.CreateIndexIfNotExistsAsync(IndexName, c => c
                .Settings(cs => cs.NumberOfShards(5).NumberOfReplicas(1))
                .Mappings(cm => cm.MapDefault()
                .Map<ESGroupBuyingGroupModel>(mp => mp.AutoMap())));
        private static Task<bool> CheckProductGroupBuyingIndex(IElasticClient client)
            => client.CreateIndexIfNotExistsAsync(PinTuanProductIndexName, c => c
                .Settings(cs => cs.NumberOfShards(5).NumberOfReplicas(1))
                .Mappings(cm => cm.MapDefault()
                .Map<ESGroupBuyingProductModel>(mp => mp.AutoMap())));

        private static async Task<bool> UpdateGroupBuyingGroupInfo(IElasticClient client, List<string> productGroupIds)
        {
            var data = await DalGroupBuying.GetGroupBuyingGroupInfo(productGroupIds);

            foreach (var item in data)
            {
                switch (item.Label)
                {
                    case "低价团": item.NewUserSort = 3; item.OldUserSort = 2; break;
                    case "精品团": item.NewUserSort = 2; item.OldUserSort = 1; break;
                    case "优惠券团": item.NewUserSort = 1; item.OldUserSort = 3; break;
                    default:
                        item.NewUserSort = 0; item.OldUserSort = 0; break;
                }
            }

            var response = await client.BulkAsync(new BulkRequest(IndexName)
            {
                Operations = data.Select<ESGroupBuyingGroupModel, IBulkOperation>(item => new BulkIndexOperation<ESGroupBuyingGroupModel>(item)
                {
                    Routing = item.Label ?? "精品团"
                }).ToArray()
            });
            Logger.Info($"UpdateGroupBuyingGroupInfo-->更新{string.Join(",", productGroupIds)},成功{response.Items?.Count() ?? 0}，失败{response.ItemsWithErrors?.Count() ?? 0}");
            return response.IsValid;
        }

        private static async Task<bool> UpdateGroupBuyingProductInfo(IElasticClient client, List<string> productGroupIds)
        {
            var productInfo = await DalGroupBuying.GetGroupBuyingProductInfo(productGroupIds);
            var categoryInfo = await GetCategoryInfo(productInfo.Select(g => g.PID).ToList());
            var orderCountInfo = await DalGroupBuying.GetGroupBuyingUserCountList(productGroupIds);
            foreach (var item in productInfo)
            {
                var categoryItem = categoryInfo.FirstOrDefault(g => g.PID == item.PID);
                item.RootCategory = categoryItem?.RootCategory;
                item.ProductCategoryList = categoryItem?.CategoryList;
                item.NewCategoryId = categoryItem?.NewCategoryId;
                item.NewCategoryCode = categoryItem?.NewCategoryCode;
                item.NewCategoryName = categoryItem?.NewCategoryName;
                item.ProductIndex = $"{item.ProductGroupId}/{item.PID}";
                switch (item.Label)
                {
                    case "低价团": item.NewUserSort = 3; item.OldUserSort = 2; break;
                    case "精品团": item.NewUserSort = 2; item.OldUserSort = 1; break;
                    case "优惠券团": item.NewUserSort = 1; item.OldUserSort = 3; break;
                    default:
                        item.NewUserSort = 0; item.OldUserSort = 0; break;
                }
                item.GroupOrderCount = orderCountInfo.FirstOrDefault(g => g.ProductGroupId == item.ProductGroupId)?.Count ?? 0;
            }
            var response = await client.BulkAsync(new BulkRequest(PinTuanProductIndexName)
            {
                Operations = productInfo.Select<ESGroupBuyingProductModel, IBulkOperation>(item => new BulkIndexOperation<ESGroupBuyingProductModel>(item)
                {
                    Routing = item.ProductGroupId
                }).ToArray()
            });
            Logger.Info($"UpdateGroupBuyingProductInfo-->更新{string.Join(",", productGroupIds)},成功{response.Items?.Count() ?? 0}，失败{response.ItemsWithErrors?.Count() ?? 0}");
            return response.IsValid;
        }


        private static async Task<List<CategoryInfoModel>> GetCategoryInfo(List<string> pids)
        {
            var resultData = new List<CategoryInfoModel>();
            using (var client = new ProductClient())
            {
                var result = await client.GetCategoryInfoByPidsAsync(pids.Distinct());
                if (result.Success && result.Result != null)
                {
                    resultData = result.Result.Select(g => new CategoryInfoModel
                    {
                        PID = g.PID,
                        RootCategory = g.RootCategory,
                        CategoryList = g.NodeNo.Split('/').Select(t =>
                        {
                            int.TryParse(t?.ToString(), out var value);
                            return value;
                        }).ToList()
                    }).ToList();
                }
                else
                {
                    Logger.Warn($"查询商品类目失败,{result?.Exception?.InnerException}");
                }
            }
            foreach(var item in resultData)
            {
                var categoryInfo = await DalGroupBuying.GetReleaseCategory(item.CategoryList);
                item.NewCategoryCode = categoryInfo.Select(g => g.CategoryCode).ToList();
                item.NewCategoryId = categoryInfo.Select(g => g.OId).ToList();
                item.NewCategoryName = categoryInfo.Select(g => g.DisplayName).ToList();
            }
            return resultData;
        }



        public static async Task<PagedModel<SimplegroupBuyingModel>> GetGroupBuyingListNew(
            GroupBuyingQueryRequest request)
        {

            Func<QueryContainerDescriptor<ESGroupBuyingGroupModel>, QueryContainer> query = null;
            // 上下架时间，渠道，是否显示筛选
            query = q => q.TermRange(fd => fd.Field(c => c.BeginTime).LessThanOrEquals(DateTime.Now.ToString("s")))
                             && q.TermRange(fd => fd.Field(c => c.EndTime).GreaterThanOrEquals(DateTime.Now.ToString("s")))
                             && q.Terms(fd => fd.Field(c => c.GroupType).Terms(new List<int> { 0, 1 }))
                             && q.Terms(f => f.Field(fd => fd.Channel).Terms(new List<string> { request.Channel, "WXAPP;kH5", "kH5;WXAPP" }))
                             && q.Term(f => f.Field(fd => fd.IsShow).Value(true));

            var data = new List<SimplegroupBuyingModel>();
            // 类目筛选
            if (request.NewCategoryCode > 0)
            {
                var categoryList = (await DalGroupBuying.GetGroupBuyingChildCategory(request.NewCategoryCode)).Select(g => g.NewCategoryCode).ToList();         
                if (categoryList.Any())
                {
                    Func<QueryContainerDescriptor<ESGroupBuyingProductModel>, QueryContainer> query2 = null;
                    query2 = q => q.Terms(c => c.Field(f => f.NewCategoryId).Terms(categoryList))
                        && q.Term(f => f.Field(fd => fd.IsShow).Value(true));
                    data = await GetGroupBuyingPidInfo(query2);
                }
                if (data.Any())
                {
                    query = q => q.Terms(f => f.Field(fd => fd.ProductGroupId).Terms(data.Select(g => g.ProductGroupId)))
                       && q.TermRange(fd => fd.Field(c => c.BeginTime).LessThanOrEquals(DateTime.Now.ToString("s")))
                       && q.TermRange(fd => fd.Field(c => c.EndTime).GreaterThanOrEquals(DateTime.Now.ToString("s")))
                       && q.Terms(fd => fd.Field(c => c.GroupType).Terms(new List<int> { 0, 1 }))
                       && q.Terms(f => f.Field(fd => fd.Channel).Terms(new List<string> { request.Channel, "WXAPP;kH5", "kH5;WXAPP" }))
                       && q.Term(f => f.Field(fd => fd.IsShow).Value(true));
                }
                else
                {
                    return new PagedModel<SimplegroupBuyingModel>
                    {
                        Pager = new PagerModel { PageSize = request.PageSize, CurrentPage = request.PageIndex, Total = 0 }
                    };
                }
            }


            // 排序逻辑
            Func<SortDescriptor<ESGroupBuyingGroupModel>, IPromise<IList<ISort>>> sort = null;
            if (request.SortType == 0)
            {
                sort = fd => fd.Descending(f => f.Sequence).Descending(f => f.CurrentGroupCount);
            }
            else if (request.IsOldUser)
            {
                sort = fd => fd.Descending(f => f.OldUserSort).Descending(f => f.CurrentGroupCount);
            }
            else
            {
                sort = fd => fd.Descending(f => f.NewUserSort).Descending(f => f.CurrentGroupCount);
            }
            var result = await GetGroupBuyingGroupInfo(query, sort, request.PageIndex, request.PageSize);
            if (data.Any() && result.Source != null)
            {
                var dataList = result.Source.ToList();
                foreach(var item in dataList)
                {
                    var pidInfo = data.Where(g => g.ProductGroupId == item.ProductGroupId)?.OrderByDescending(g => g.IsDefault).FirstOrDefault();
                    if (pidInfo!=null)
                    {
                        item.PID = pidInfo.PID;
                        item.IsDefault = pidInfo.IsDefault;
                    }
                }
                result.Source = dataList;
            }
            return result;
        }


        public static async Task<List<SimplegroupBuyingModel>> GetGroupBuyingPidInfo(Func<QueryContainerDescriptor<ESGroupBuyingProductModel>, QueryContainer> query)
        {
            var client = ElasticsearchHelper.CreateClient();
            if (!await CheckProductGroupBuyingIndex(client)) {
                Logger.Warn("GetGroupBuyingPidInfo--> 建立Index失敗");
                return new List<SimplegroupBuyingModel>();
            }
            //ElasticsearchHelper.EnableDebug();
            var respone = await client.SearchAsync<ESGroupBuyingProductModel>(
                s => s.Index(PinTuanProductIndexName)
                        .Type("GroupBuyingProduct")
                        .Query(query).Size(1000)
            );
            if (respone.IsValid)
            {
                return respone.Documents.Select(g => new SimplegroupBuyingModel
                {
                    PID = g.PID,
                    IsDefault = g.IsDefault,
                    ProductGroupId = g.ProductGroupId,
                    ActivityId = g.ActivityId
                }).ToList();
            }
            return new List<SimplegroupBuyingModel>();
        }


        public static async Task<PagedModel<SimplegroupBuyingModel>> GetGroupBuyingGroupInfo(Func<QueryContainerDescriptor<ESGroupBuyingGroupModel>, QueryContainer> query, Func<SortDescriptor<ESGroupBuyingGroupModel>, IPromise<IList<ISort>>> sort, int pageIndex, int pageSize)
        {
            var result = new PagedModel<SimplegroupBuyingModel>
            {
                Pager = new PagerModel()
                {
                    CurrentPage = pageIndex,
                    PageSize = pageSize,
                    Total = 0
                }
            };
            var client = ElasticsearchHelper.CreateClient();
            if (!await CheckGroupBuyingIndex(client))
            {
                Logger.Warn("GetGroupBuyingGroupInfo--> 建立Index失敗");
                return result;
            }
            // ElasticsearchHelper.EnableDebug();
            var respone = await client.SearchAsync<ESGroupBuyingGroupModel>(
                s => s.Index(IndexName)
                        .Type("ProductGroup")
                        .Query(query)
                        .Sort(sort)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
            );
            if (respone.IsValid)
            {

                result.Pager.Total = Convert.ToInt32(respone.Total);
                result.Source = respone.Documents.Select(g => new SimplegroupBuyingModel
                {
                    ProductGroupId = g.ProductGroupId,
                    ActivityId = g.ActivityId,
                    PID = g.DefaultProduct,
                    IsDefault = true
                }).ToList();
            }
            return result;
        }

        public static async Task<PagedModel<SimplegroupBuyingModel>> SelectGroupBuyingListCache(GroupBuyingQueryRequest request)
        {
            var cacheKey = $"GroupBuyingList/{request.PageIndex}/{request.PageSize}/{request.Channel}/{request.SortType}/{request.IsOldUser}/{request.NewCategoryCode}/{request.KeyWord}";
            var data = new PagedModel<SimplegroupBuyingModel>
            {
                Pager = new PagerModel { PageSize = request.PageSize, CurrentPage = request.PageIndex, Total = 0 }
            };
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GroupBuyingListNewKey))
            {
                var result = await client.GetOrSetAsync(cacheKey, () => SelectGroupBuyingListNew(request), TimeSpan.FromSeconds(60));
                if (result.Success && result.Value != null)
                {
                    data = result.Value;
                }
                else
                {
                    Logger.Warn($"查询拼团商品类目信息Redis缓存失败，{cacheKey}");
                    data = await GetGroupBuyingListNew(request);
                }
            }
            return data;
        }
        public static async Task<PagedModel<SimplegroupBuyingModel>> SelectGroupBuyingListNew(GroupBuyingQueryRequest request)
        {
            var result = new PagedModel<SimplegroupBuyingModel>
            {
                Pager = new PagerModel()
                {
                    CurrentPage = request.PageIndex,
                    PageSize = request.PageSize,
                    Total = 0
                }
            };
            var data = new List<SimplegroupBuyingModel>();
            var array = new List<Func<QueryContainerDescriptor<ESGroupBuyingProductModel>, QueryContainer>>();
            // 上架时间
            array.Add(q => q.TermRange(fd => fd.Field(c => c.BeginTime).LessThanOrEquals(DateTime.Now.ToString("s"))));
            // 下架时间
            array.Add(q => q.TermRange(fd => fd.Field(c => c.EndTime).GreaterThanOrEquals(DateTime.Now.ToString("s"))));
            // 拼团类型
            array.Add(q => q.Terms(fd => fd.Field(c => c.GroupType).Terms(new List<int> { 0, 1 })));
            // 配置渠道
            array.Add(q => q.Terms(f => f.Field(fd => fd.ChannelList).Terms(new List<string> { request.Channel })));
            // 产品是否显示
            array.Add(q => q.Term(f => f.Field(fd => fd.IsShow).Value(true)));
            // 产品组是否显示
            array.Add(q => q.Term(f => f.Field(fd => fd.GroupIsShow).Value(true)));

            // 类目筛选
            if (request.NewCategoryCode > 0)
            {
                var categoryList = (await DalGroupBuying.GetGroupBuyingChildCategory(request.NewCategoryCode)).Select(g => g.NewCategoryCode).ToList();
                if (categoryList.Any())
                {
                    array.Add(q => q.Terms(c => c.Field(f => f.NewCategoryId).Terms(categoryList)));
                }
            }
            // 关键词搜索
            if (!string.IsNullOrWhiteSpace(request.KeyWord))
            {
                array.Add(q => q.Match(m => m.Field(f => f.SearchKeyForMax).Query(request.KeyWord).MinimumShouldMatch("100%")));
                array.Add(q => q.Term(f => f.Field(fd => fd.IsDefault).Value(true)));
            }
            Func<QueryContainerDescriptor<ESGroupBuyingProductModel>, QueryContainer> query = q => q.Bool(qb => qb.Must(array));
            Func<SortDescriptor<ESGroupBuyingProductModel>, IPromise<IList<ISort>>> sort = null;
            switch (request.SortType)
            {
                case 1 when request.IsOldUser:
                    sort = fd => fd.Descending(f => f.OldUserSort).Descending(f => f.GroupOrderCount);
                    break;
                case 1:
                    sort = fd => fd.Descending(f => f.NewUserSort).Descending(f => f.GroupOrderCount);
                    break;
                case 2:
                    sort = fd => fd.Descending(f => f.GroupOrderCount);
                    break;
                case 3:
                    sort = fd => fd.Ascending(f => f.ActivityPrice);
                    break;
                case 4:
                    sort = fd => fd.Descending(f => f.ActivityPrice);
                    break;
                default:
                    sort = fd => fd.Descending(f => f.Sequence).Descending(f => f.GroupOrderCount);
                    break;
            }
            var client = ElasticsearchHelper.CreateClient();
            if (!await CheckProductGroupBuyingIndex(client))
            {
                Logger.Warn("GetGroupBuyingPidInfo--> 建立Index失敗");
                return result;
            }
            //ElasticsearchHelper.EnableDebug();
            var respone = await client.SearchAsync<ESGroupBuyingProductModel>(
                s => s.Index(PinTuanProductIndexName)
                        .Type("GroupBuyingProduct")
                        .Query(query)
                        .Sort(sort)
                        .Take(10000)
                        .Source(sc => sc.Include(f => f.Fields(fd => fd.ActivityId, fd => fd.ProductGroupId, fd => fd.IsDefault, fd => fd.PID)))
            );
            if (respone.IsValid && respone.Documents.Any())
            {
                var resultData = respone.Documents.Select(g => new SimplegroupBuyingModel
                {
                    ProductGroupId = g.ProductGroupId,
                    PID = g.PID,
                    ActivityId = g.ActivityId,
                    IsDefault = g.IsDefault
                }).ToList();
                // 根据关键词搜索，对ProductGroupID不去重
                if (string.IsNullOrWhiteSpace(request.KeyWord))
                {
                    resultData = resultData.GroupBy(g => g.ProductGroupId, (k, v) => v.OrderByDescending(t => t.IsDefault).FirstOrDefault()).Where(g => g != null).ToList();
                }
                result.Pager.Total = resultData.Count;
                result.Source = resultData.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
            }
            return result;
        }
#endregion

        public static void SetProcessLog()
        {
            var log = JsonConvert.SerializeObject(Process.GetCurrentProcess().Threads
                .Cast<ProcessThread>()
                .GroupBy(t => t.ThreadState)
                .ToDictionary(t => t.Key, t => t.Count()));
            Logger.Info($"ProcessLog==>{log}");
        }

        public static ProductGroupModel CopyGroupInfo(ProductGroupModel resource)
        {
            return new ProductGroupModel
            {
                ProductGroupId = resource.ProductGroupId,
                Image = resource.Image,
                ShareId = resource.ShareId,
                GroupType = resource.GroupType,
                MemberCount = resource.MemberCount,
                Sequence = resource.Sequence,
                CurrentGroupCount = resource.CurrentGroupCount,
                TotalGroupCount = resource.TotalGroupCount,
                BeginTime = resource.BeginTime,
                EndTime = resource.EndTime,
                Label = resource.Label,
                LabelList = resource.LabelList,
                ActivityId = resource.ActivityId,
                GroupUserList = resource.GroupUserList,
                GroupOrderCount = resource.GroupOrderCount,
                IsShow = resource.IsShow,
                GroupCategory = resource.GroupCategory,
                GroupDescription = resource.GroupDescription,
                ShareImage = resource.ShareImage
            };
        }

    }
}
