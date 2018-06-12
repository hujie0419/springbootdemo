using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.MessageQueue;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models.New;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class ZeroActivityManager
    {
        private static readonly string DefaultClientName = "ZeroActivity";

        private const string ZeroActivityCacheListKey = "ZeroActivityCacheList";
        private const string ZeroActivityListKey = "ZeroActivityList";
        private const string ZeroActivityProductListKey = "ZeroActivityProductList";
        private const string ZeroActivityPeriodKey = "ZeroActivityPeriod";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ZeroActivityManager));

        private static async Task<List<SkuProductDetailModel>> SelectProductInfoByPids(List<string> pids)
        {
            using (var pClient = new ProductClient())
            {
                var products = await pClient.SelectSkuProductListByPidsAsync(pids);

                if (products.Success && products.Result != null)
                {
                    return products.Result;
                }
                else
                {
                    if (!products.Success)
                        Logger.Warn($"获取相关产品信息失败。pids：{string.Join(",", pids)};Error:{products.ErrorMessage}", products.Exception);
                    return new List<SkuProductDetailModel>();
                }
            }
        }

        private static async Task<int> FetchNumOfAppliesOnPeriod(int period)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var numOfAppliesResult = await client.GetOrSetAsync(GlobalConstant.NumOfApplications + period + "/", () => DalZeroActivity.FetchNumOfApplicationsAsync(period), GlobalConstant.NumOfApplicationsExpiration);
                if (numOfAppliesResult.Success)
                    return numOfAppliesResult.Value;
                else
                {
                    Logger.Warn($"缓存redis失败NumOfApplications：{GlobalConstant.NumOfApplications + period + "/"};Error:{numOfAppliesResult.Message}", numOfAppliesResult.Exception);
                    return await DalZeroActivity.FetchNumOfApplicationsAsync(period);
                }
            }
        }

        private static async Task<SkuProductDetailModel> FetchProductInfoOnPid(int period, string pid)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var products = new List<SkuProductDetailModel>();
                var productInfoResult = await client.GetOrSetAsync(GlobalConstant.ProductInfoOfZeroActivityDetail + period + "/", () => SelectProductInfoByPids(new List<string> { pid }), GlobalConstant.productInfoExpiration);
                Logger.Info($"3redis{productInfoResult.RealKey};{productInfoResult.Value?.FirstOrDefault()?.ProductId};{productInfoResult.Value?.FirstOrDefault()?.DisplayName}");
                if (productInfoResult.Success && productInfoResult.Value != null)
                {
                    if (productInfoResult.Value.FirstOrDefault(_ => _.Pid == pid) == null)
                    {
                        var renewedProductInfoResult = await client.SetAsync(GlobalConstant.ProductInfoOfZeroActivityDetail + period + "/", await SelectProductInfoByPids(new List<string> { pid }), GlobalConstant.productInfoExpiration);

                        if (renewedProductInfoResult.Success && renewedProductInfoResult.Value != null)
                            products = renewedProductInfoResult.Value;
                        else
                        {
                            if (!renewedProductInfoResult.Success)
                                Logger.Warn($"3产品信息缓存redis设置失败ProductInfoOfZeroActivitydetail：{GlobalConstant.ProductInfoOfUnfinishedZeroActivities + period + "/"};Error:{renewedProductInfoResult.Message}", renewedProductInfoResult.Exception);
                            products = await SelectProductInfoByPids(new List<string> { pid });
                        }
                    }
                    else
                        products = productInfoResult.Value;
                }
                else
                {
                    if (!productInfoResult.Success)
                        Logger.Warn($"3缓存redis失败ProductInfoOfZeroActivityDetail：{GlobalConstant.ProductInfoOfZeroActivityDetail + period + "/"};Error:{productInfoResult.Message}", productInfoResult.Exception);
                    products = await SelectProductInfoByPids(new List<string> { pid });
                }
                return products.FirstOrDefault(_ => _.Pid == pid);
            }
        }


        private static async Task<IEnumerable<ZeroActivityModel>> GetAllZeroActivityList(bool resetCache = false)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                if (resetCache)
                {
                    var list =await DalZeroActivity.SelectUnfinishedZeroActivitiesForHomepageAsync();
                    var allZeroActivityList = list as ZeroActivityModel[] ?? list.ToArray();

                    var setResult = await client.SetAsync(ZeroActivityListKey, allZeroActivityList, TimeSpan.FromDays(1));
                    if (!setResult.Success)
                    {
                        Logger.Error($"设置redis缓存({ZeroActivityListKey})失败;Error:{setResult.Message}",setResult.Exception);
                    }
                    return setResult.Value;
                }
                else
                {
                    var getResult = await client.GetOrSetAsync(ZeroActivityListKey,
                                async () => await DalZeroActivity.SelectUnfinishedZeroActivitiesForHomepageAsync() ?? new List<ZeroActivityModel>(),
                                TimeSpan.FromDays(1));
                    if (!getResult.Success)
                    {
                        Logger.Error($"获取redis缓存({ZeroActivityListKey})失败;Error:{getResult.Message}", getResult.Exception);
                    }
                    return getResult.Value;
                }
            }
        }

        private static async Task<List<SkuProductDetailModel>> GetAllZeroActivityProductsList(IEnumerable<ZeroActivityModel> zeroActivities, bool resetCache = false)
        {
            var pids = zeroActivities.Select(_ => _.PID).ToList();

            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                if (resetCache)
                {
                    var productList = await SelectProductInfoByPids(pids);
                    var setResult = await client.SetAsync(ZeroActivityProductListKey,productList,TimeSpan.FromHours(1));
                    if (!setResult.Success)
                    {
                        Logger.Warn($"设置redis缓存({ZeroActivityListKey})失败;Error:{setResult.Message}", setResult.Exception);
                        throw setResult.Exception;
                    }
                    return setResult.Value;
                }
                else
                {
                    var getResult = await client.GetOrSetAsync(ZeroActivityProductListKey, async () => await SelectProductInfoByPids(pids),TimeSpan.FromHours(1));

                    if (!getResult.Success)
                    {
                        Logger.Warn($"获取redis缓存({ZeroActivityProductListKey})失败;Error:{getResult.Message}", getResult.Exception);
                    }
                    return getResult.Value;
                }
            }
        }


        private static async Task<int> FetchNumOfAppliesOnPeriod(int period  ,bool resetCache)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var key = ZeroActivityPeriodKey + "/" + period;

                if (resetCache)
                {
                    var val = await DalZeroActivity.FetchNumOfApplicationsAsync(period);

                    var setResult = await client.SetAsync(key, val, TimeSpan.FromMinutes(30));
                    if (!setResult.Success)
                    {
                        Logger.Error($"设置redis缓存({key})失败;Error:{setResult.Message}",setResult.Exception);
                    }
                    return setResult.Value;
                }
                else
                {
                    var getResult = await client.GetOrSetAsync(key,
                                            async () => await DalZeroActivity.FetchNumOfApplicationsAsync(period),
                                            TimeSpan.FromMinutes(30));
                    if (!getResult.Success)
                    {
                        Logger.Error($"获取redis缓存({key})失败;Error:{getResult.Message}", getResult.Exception);
                    }
                    return getResult.Value;
                }
            }
        }


        private static async Task<IEnumerable<ZeroActivityModel>> MapZeroActivities(
                                            IEnumerable<ZeroActivityModel> zeroActivities,
                                            List<SkuProductDetailModel> productList, bool resetCache)
        {

            var zeroActivityResult = new List<ZeroActivityModel>();

            foreach (var zeroActivity in zeroActivities.ToList())
            {
                var numOfApplies = await FetchNumOfAppliesOnPeriod(zeroActivity.Period, resetCache);

                Logger.Info($"第{zeroActivity.Period}期活动的申请次数是{numOfApplies}");

                var product = productList.FirstOrDefault(_ => _.Pid == zeroActivity.PID);
                if (product == null)
                    Logger.Warn($"1PID为{zeroActivity.PID}的产品详情失败");

                zeroActivityResult.Add(new ZeroActivityModel
                {
                    Period = zeroActivity.Period,
                    ProductName = product?.DisplayName,
                    NumOfApplications = numOfApplies,
                    NumOfWinners = zeroActivity.NumOfWinners,
                    SingleValue = zeroActivity.Quantity / zeroActivity.NumOfWinners * product?.Price ?? 0,
                    StartDateTime = zeroActivity.StartDateTime,
                    EndDateTime = zeroActivity.EndDateTime,
                    StatusOfActivity = (DateTime.Now < zeroActivity.StartDateTime) ? 0 : ((DateTime.Now < zeroActivity.EndDateTime) ? 1 : 3),
                    Description = zeroActivity.Description,
                    ImgUrl = zeroActivity.ImgUrl,
                    PID = zeroActivity.PID,
                    Quantity = zeroActivity.Quantity,

                    ProductImage = product?.ImageUrls?.FirstOrDefault(p => !string.IsNullOrEmpty(p)) //product?.ListImage?.Image
                });
            }

            Logger.Info($"1周期是{string.Join(";", zeroActivityResult.Select(p => p.Period).ToList())};获奖者人数是{string.Join(";", zeroActivityResult.Select(p => p.NumOfWinners).ToList())};申请次数是{string.Join(";", zeroActivityResult.Select(p => p.NumOfApplications).ToList())};产品PID是{string.Join(";", zeroActivityResult.Select(p => p.PID).ToList())};产品名称是{string.Join(";", zeroActivityResult.Select(p => p.ProductName).ToList())}");

            return zeroActivityResult;
        }

        /// <summary>
        /// 0元众测
        /// </summary>
        /// <param name="resetCache"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ZeroActivityModel>> SelectUnfinishedZeroActivitiesForHomepageAsync(bool resetCache = false)
        {
            IEnumerable<ZeroActivityModel> zeroActivityResult = new List<ZeroActivityModel>();

            var zeroActivities = await GetAllZeroActivityList(resetCache);
            var zeroActivityModels = zeroActivities as ZeroActivityModel[] ?? zeroActivities.ToArray();
            if (zeroActivityModels.Any())
            {
                var products = await GetAllZeroActivityProductsList(zeroActivityModels, resetCache);

                using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                {

                    if (resetCache)
                    {
                        var zeroActivityList = await MapZeroActivities(zeroActivityModels, products, true);
                        var setResult = await client.SetAsync(ZeroActivityCacheListKey, zeroActivityList,
                                                        TimeSpan.FromMinutes(30));
                        if (!setResult.Success)
                        {
                            Logger.Error($"设置redis缓存({ZeroActivityCacheListKey})失败;Error:{setResult.Message}",
                                setResult.Exception);
                        }
                        zeroActivityResult = setResult.Value;
                    }
                    else
                    {
                        var getResult = await client.GetOrSetAsync(ZeroActivityCacheListKey,
                            async  () => await MapZeroActivities(zeroActivityModels, products, false),
                            TimeSpan.FromMinutes(30));
                        if (!getResult.Success)
                        {
                            Logger.Error($"获取redis缓存({ZeroActivityCacheListKey})失败;Error:{getResult.Message}",getResult.Exception);
                        }
                        zeroActivityResult = getResult.Value;
                    }

                }
            }
            return zeroActivityResult.OrderByDescending(_ => _.StatusOfActivity).ThenByDescending(_ => _.Period).ToList();
        }

        public static async Task<IEnumerable<ZeroActivityModel>> SelectFinishedZeroActivitiesForHomepageAsync(int pageNumber)
        {
            IEnumerable<ZeroActivityModel> zeroActivities;
            IList<ZeroActivityModel> zeroActivityResult = new List<ZeroActivityModel>();
            var prefix = await RedisHelper.GetZeroActivityCacheKeyPrefix(GlobalConstant.FinishedZeroActivitiesForHomepage);

            Logger.Info($"2缓存key前缀为{prefix}");
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(
                    prefix + pageNumber.ToString() + "/",
                    () => DalZeroActivity.SelectFinishedZeroActivitiesForHomepageAsync(pageNumber),
                    GlobalConstant.ZeroActivitiesForHomepageExpiration);

                Logger.Info($"2第{pageNumber}页redis{result.RealKey};{string.Join(";", result.Value?.Select(p => p.Period).ToList())};{string.Join(";", result.Value?.Select(p => p.PID).ToList())}ProductName:{string.Join(";", result.Value?.Select(p => p.ProductName).ToList())}");

                if (result.Success && result.Value != null)
                    zeroActivities = result.Value;
                else
                {
                    if (!result.Success)
                        Logger.Warn($"获取已完成零元购活动列表缓存redis失败FinishedZeroActivitiesForHomepage：{GlobalConstant.FinishedZeroActivitiesForHomepage + pageNumber.ToString() + "/"};Error:{result.Message}", result.Exception);
                    zeroActivities = await DalZeroActivity.SelectFinishedZeroActivitiesForHomepageAsync(pageNumber);
                    Logger.Info($"获取到的已完成零元购活动列表数据库查询结果{string.Join(";", zeroActivities?.Select(p => p.Period).ToList())};{string.Join(";", zeroActivities?.Select(p => p.PID).ToList())}");
                }
                if (zeroActivities != null && zeroActivities.Any())
                {
                    foreach (var zeroActivity in zeroActivities.ToList())
                    {
                        Logger.Info($"2第{pageNumber}页第{zeroActivity.Period}期活动的关联商品的pid是{zeroActivity.PID}");
                    }
                    var products = new List<SkuProductDetailModel>();

                    var productInfoResult = await client.GetOrSetAsync(
                        GlobalConstant.ProductInfoOfFinishedZeroActivities + pageNumber + "/",
                        () => SelectProductInfoByPids(zeroActivities.Select(_ => _.PID).ToList()),
                        GlobalConstant.productInfoExpiration);

                    Logger.Info($"2第{pageNumber}页的产品信息redis{productInfoResult.RealKey};{string.Join(";", productInfoResult.Value?.Select(p => p.ProductId).ToList())};{string.Join(";", productInfoResult?.Value.Select(p => p.DisplayName).ToList())}");
                    if (productInfoResult.Success && productInfoResult.Value != null)
                    {
                        var needRenew = false;
                        foreach (var zeroActivity in zeroActivities.ToList())
                        {
                            if (!productInfoResult.Value.Any(_ => _.Pid == zeroActivity.PID))
                            {
                                Logger.Warn($"2PID为{zeroActivity.PID}的产品详情获取失败获取失败，可能是产品信息出现更变引起的");
                                needRenew = true;
                                break;
                            }
                        }
                        if (needRenew)
                        {
                            var renewedProductInfoResult = await client.SetAsync(GlobalConstant.ProductInfoOfFinishedZeroActivities,
                                await SelectProductInfoByPids(zeroActivities.Select(_ => _.PID).ToList()), GlobalConstant.productInfoExpiration);
                            if (renewedProductInfoResult.Success && renewedProductInfoResult.Value != null)
                                products = renewedProductInfoResult.Value;
                            else
                            {
                                if (!renewedProductInfoResult.Success)
                                    Logger.Warn($"2产品信息缓存redis设置失败ProductInfoOfFinishedZeroActivities：{GlobalConstant.ProductInfoOfFinishedZeroActivities};Error:{renewedProductInfoResult.Message}", renewedProductInfoResult.Exception);
                                products = await SelectProductInfoByPids(zeroActivities.Select(_ => _.PID).ToList());
                            }
                        }
                        else
                        {
                            products = productInfoResult.Value;
                        }
                    }
                    else
                    {
                        if (!productInfoResult.Success)
                            Logger.Warn($"2缓存redis失败ProductInfoOfUnfinishedZeroActivities：{GlobalConstant.ProductInfoOfUnfinishedZeroActivities + pageNumber + "/"};Error:{productInfoResult.Message}", productInfoResult.Exception);
                        products = await SelectProductInfoByPids(zeroActivities.Select(_ => _.PID).ToList());
                    }
                    Logger.Info($"2第{pageNumber}页的产品详情{products.Count()}条，应该为{zeroActivities.Count()}条,具体为{string.Join(";", products.Select(p => p.ProductId).ToList())};{string.Join(";", products.Select(p => p.DisplayName).ToList())}");

                    foreach (var zeroActivity in zeroActivities.ToList())
                    {
                        var numOfApplies = await FetchNumOfAppliesOnPeriod(zeroActivity.Period);
                        Logger.Info($"2第{pageNumber}页的第{zeroActivity.Period}期活动的申请次数是{numOfApplies}");
                        var product = products.FirstOrDefault(_ => _.Pid == zeroActivity.PID);
                        if (product == null)
                            Logger.Warn($"2PID为{zeroActivity.PID}的产品详情失败");
                        zeroActivityResult.Add(new ZeroActivityModel
                        {
                            Period = zeroActivity.Period,
                            ProductName = product?.DisplayName,
                            NumOfApplications = numOfApplies,
                            NumOfWinners = zeroActivity.NumOfWinners,
                            SingleValue = (product == null) ? 0 : (zeroActivity.Quantity / zeroActivity.NumOfWinners) * product.Price,
                            StartDateTime = zeroActivity.StartDateTime,
                            EndDateTime = zeroActivity.EndDateTime,
                            StatusOfActivity = (DateTime.Now < zeroActivity.EndDateTime.AddDays(30)) ? 3 : 4,
                            Description = zeroActivity.Description,
                            ImgUrl = zeroActivity.ImgUrl,
                            PID = zeroActivity.PID,
                            Quantity = zeroActivity.Quantity
                        });
                    }
                }
                Logger.Info($"2周期是{string.Join(";", zeroActivityResult.Select(p => p.Period).ToList())};获奖者人数是{string.Join(";", zeroActivityResult.Select(p => p.NumOfWinners).ToList())};申请次数是{string.Join(";", zeroActivityResult.Select(p => p.NumOfApplications).ToList())};产品PID是{string.Join(";", zeroActivityResult.Select(p => p.PID).ToList())};产品名称是{string.Join(";", zeroActivityResult.Select(p => p.ProductName).ToList())}");
                return zeroActivityResult;
            }
        }

        public static async Task<ZeroActivityDetailModel> FetchZeroActivityDetailAsync(int period)
        {
            var prefix = await RedisHelper.GetZeroActivityCacheKeyPrefix(GlobalConstant.ZeroActivityDetail);
            Logger.Info($"3缓存key前缀为{prefix}");
            ZeroActivityDetailModel zeroActivity;
            ZeroActivityDetailModel zeroActivityResult = new ZeroActivityDetailModel();
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(prefix + period.ToString() + "/", () => DalZeroActivity.FetchZeroActivityDetailAsync(period), GlobalConstant.ZeroActivityDetailExpiration);
                Logger.Info($"3redis{result.RealKey};{result.Value?.PID}ProductName:{result.Value?.ProductName}");
                if (result.Success)
                {
                    zeroActivity = result.Value;
                }
                else
                {
                    if (!result.Success)
                        Logger.Warn($"3缓存redis失败ZeroActivityDetail：{GlobalConstant.ZeroActivityDetail + period.ToString() + "/"};Error:{result.Message}", result.Exception);
                    zeroActivity = await DalZeroActivity.FetchZeroActivityDetailAsync(period);
                    Logger.Info($"3数据库查询结果{zeroActivity?.PID}");
                }
                if (zeroActivity != null)
                {
                    var product = await FetchProductInfoOnPid(period, zeroActivity.PID);
                    Logger.Info($"3产品详情具体为{product?.ProductId};{product?.DisplayName}");
                    if (product == null)
                        Logger.Warn($"PID为{zeroActivity.PID}的产品详情失败");
                    var numOfApplies = await FetchNumOfAppliesOnPeriod(zeroActivity.Period);
                    Logger.Info($"3第{zeroActivity.Period}期活动的申请次数是{numOfApplies}");
                    zeroActivityResult = new ZeroActivityDetailModel
                    {
                        Period = zeroActivity.Period,
                        ProductName = product?.DisplayName,
                        NumOfApplications = numOfApplies,
                        NumOfWinners = zeroActivity.NumOfWinners,
                        SingleValue = (product == null) ? 0 : (zeroActivity.Quantity / zeroActivity.NumOfWinners) * product.Price,
                        StartDateTime = zeroActivity.StartDateTime,
                        EndDateTime = zeroActivity.EndDateTime,
                        StatusOfActivity = (DateTime.Now < zeroActivity.StartDateTime) ? 0 : ((DateTime.Now < zeroActivity.EndDateTime) ? 1 : ((DateTime.Now < zeroActivity.EndDateTime.AddDays(30)) ? 3 : 4)),
                        Description = zeroActivity.Description,
                        ImgUrl = zeroActivity.ImgUrl,
                        PID = zeroActivity.PID,
                        Quantity = zeroActivity.Quantity,
                        ProductImages = product?.ImageUrls
                    };
                }
                Logger.Info($"3周期是{zeroActivityResult.Period.ToString()};获奖者人数是{zeroActivityResult.NumOfWinners.ToString()};申请次数是{zeroActivityResult.NumOfApplications.ToString()};产品PID是{zeroActivityResult.PID};产品名称是{zeroActivityResult.ProductName}");
                return zeroActivityResult;
            }
        }

        public static async Task<bool> HasZeroActivityApplicationSubmittedAsync(Guid userId, int period)
        {
            return await DalZeroActivity.HasZeroActivityApplicationSubmittedAsync(userId, period);
        }

        public static async Task<bool> HasZeroActivityReminderSubmittedAsync(Guid userId, int period)
        {
            return await DalZeroActivity.HasZeroActivityReminderSubmittedAsync(userId, period);
        }

        private static async Task<IEnumerable<SelectedTestReport>> HandleUserReports(int period)
        {
            var testReports = (await DalZeroActivity.SelectChosenUserReportsAsync(period)).ToList();
            if (testReports != null && testReports.Any())
            {
                IEnumerable<User> users = new List<User>();
                using (var uClient = new UserAccountClient())
                {
                    var result = await uClient.GetUsersByIdsAsync(testReports.Select(tr => tr.UserId).Distinct().ToList());
                    if (result.Success && result.Result != null && result.Result.Any())
                        users = result.Result;
                }
                if (users.Any())
                {
                    foreach (var tr in testReports)
                    {
                        var singleUser = users.FirstOrDefault(ur => Equals(ur.UserId, tr.UserId));
                        if (singleUser?.Profile != null)
                        {
                            if (!string.IsNullOrWhiteSpace(singleUser.Profile.HeadUrl))
                                tr.HeadImage = ImageHelper.GetImageUrl(singleUser.Profile.HeadUrl);
                            tr.UserName = string.IsNullOrWhiteSpace(singleUser.Profile.UserName) ? singleUser.Profile.NickName : singleUser.Profile.UserName;
                            tr.Gender = Convert.ToInt32(singleUser.Profile.Gender);
                        }
                    }
                }
                return testReports;
            }
            return new List<SelectedTestReport>();
        }

        public static async Task<IEnumerable<SelectedTestReport>> SelectChosenUserReportsAsync(int period)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var userReports = await client.GetOrSetAsync(GlobalConstant.TestReportsOfPeriod + period.ToString() + "/", () => HandleUserReports(period), GlobalConstant.TestReportsOfPeriodExpiration);
                if (userReports.Success && userReports.Value != null)
                    return userReports.Value;
                else
                {
                    if (!userReports.Success)
                        Logger.Warn($"获取入选用户及其报告列表缓存redis失败TestReportsOfPeriod：{GlobalConstant.TestReportsOfPeriod + period.ToString() + "/"};Error:{userReports.Message}", userReports.Exception);
                    return await HandleUserReports(period);
                }
            }
        }

        private static async Task<SelectedTestReportDetail> HandleTestReportDetail(int commentId)
        {
            var testReport = await DalZeroActivity.FetchTestReportDetailAsync(commentId);
            if (testReport != null)
            {
                using (var uClient = new UserAccountClient())
                {
                    var result = await uClient.GetUserByIdAsync(testReport.UserId);
                    if (result.Success && result.Result?.Profile != null)
                    {
                        if (!string.IsNullOrWhiteSpace(result.Result.Profile.HeadUrl))
                            testReport.HeadImage = ImageHelper.GetImageUrl(result.Result.Profile.HeadUrl);
                        testReport.UserName = string.IsNullOrWhiteSpace(result.Result.Profile.UserName) ? result.Result.Profile.NickName : result.Result.Profile.UserName;
                        testReport.Gender = Convert.ToInt32(result.Result.Profile.Gender);
                    }
                }
                var structuredCommentAttrbute = new CommentExtenstionAttribute
                {
                    CarTypeDes = string.Empty,
                    InstallShop = string.Empty,
                    TestEnvironment = new TestEnvironment
                    {
                        DriveSituation = string.Empty,
                        Humidity = string.Empty,
                        RoadSituation = string.Empty,
                        WeatherSituation = string.Empty,
                        Temperature = string.Empty
                    },
                    TestUserInfo = new TestUserInfo
                    {
                        Cellphone = string.Empty,
                        DriveDistance = string.Empty,
                        DriveStyle = string.Empty,
                        Name = string.Empty
                    }
                };
                if (testReport.TestReportExtenstionAttribute != null)
                {
                    structuredCommentAttrbute.CarTypeDes = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.CarTypeDes) ? testReport.TestReportExtenstionAttribute.CarTypeDes : string.Empty;
                    structuredCommentAttrbute.CarID =  testReport.TestReportExtenstionAttribute.CarID;
                    structuredCommentAttrbute.InstallShopID = testReport.TestReportExtenstionAttribute.InstallShopID;
                    structuredCommentAttrbute.OrderDatetime = testReport.TestReportExtenstionAttribute.OrderDatetime;
                    structuredCommentAttrbute.InstallShop = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.InstallShop) ? testReport.TestReportExtenstionAttribute.InstallShop : string.Empty;
                    if (testReport.TestReportExtenstionAttribute.TestEnvironment != null)
                    {
                        structuredCommentAttrbute.TestEnvironment.DriveSituation = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.TestEnvironment.DriveSituation) ? testReport.TestReportExtenstionAttribute.TestEnvironment.DriveSituation : string.Empty;
                        structuredCommentAttrbute.TestEnvironment.Humidity =  !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.TestEnvironment.Humidity) ? testReport.TestReportExtenstionAttribute.TestEnvironment.Humidity : string.Empty;
                        structuredCommentAttrbute.TestEnvironment.RoadSituation = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.TestEnvironment.RoadSituation) ? testReport.TestReportExtenstionAttribute.TestEnvironment.RoadSituation : string.Empty;
                        structuredCommentAttrbute.TestEnvironment.WeatherSituation = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.TestEnvironment.WeatherSituation) ? testReport.TestReportExtenstionAttribute.TestEnvironment.WeatherSituation : string.Empty;
                        structuredCommentAttrbute.TestEnvironment.Temperature = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.TestEnvironment.Temperature) ? testReport.TestReportExtenstionAttribute.TestEnvironment.Temperature : string.Empty;
                    }
                    if (testReport.TestReportExtenstionAttribute.TestUserInfo != null)
                    {
                        structuredCommentAttrbute.TestUserInfo.Age = testReport.TestReportExtenstionAttribute.TestUserInfo.Age;
                        structuredCommentAttrbute.TestUserInfo.Cellphone = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.TestUserInfo.Cellphone) ? testReport.TestReportExtenstionAttribute.TestUserInfo.Cellphone : string.Empty;
                        structuredCommentAttrbute.TestUserInfo.DriveAge = testReport.TestReportExtenstionAttribute.TestUserInfo.DriveAge;
                        structuredCommentAttrbute.TestUserInfo.DriveDistance = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.TestUserInfo.DriveDistance) ? testReport.TestReportExtenstionAttribute.TestUserInfo.DriveDistance : string.Empty;
                        structuredCommentAttrbute.TestUserInfo.DriveStyle = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.TestUserInfo.DriveStyle) ? testReport.TestReportExtenstionAttribute.TestUserInfo.DriveStyle : string.Empty;
                        structuredCommentAttrbute.TestUserInfo.Gender = testReport.TestReportExtenstionAttribute.TestUserInfo.Gender;
                        structuredCommentAttrbute.TestUserInfo.Name = !string.IsNullOrWhiteSpace(testReport.TestReportExtenstionAttribute.TestUserInfo.Name) ? testReport.TestReportExtenstionAttribute.TestUserInfo.Name : string.Empty;
                    }
                }
                testReport.TestReportExtenstionAttribute = structuredCommentAttrbute;
                return testReport;
            }
            return new SelectedTestReportDetail();
        }

        public static async Task<SelectedTestReportDetail> FetchTestReportDetailAsync(int commentId)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var reportDetail = await client.GetOrSetAsync(GlobalConstant.TestReportDetail + commentId.ToString() + "/", () => HandleTestReportDetail(commentId), GlobalConstant.TestReportDetailExpiration);
                if (reportDetail.Success && reportDetail.Value != null)
                    return reportDetail.Value;
                else
                {
                    if (!reportDetail.Success)
                        Logger.Warn($"获取入选报告详情缓存redis失败TestReportDetail：{GlobalConstant.TestReportDetail + commentId.ToString() + "/"};Error:{reportDetail.Message}", reportDetail.Exception);
                    return await HandleTestReportDetail(commentId);
                }
            }
        }

        private static async Task<IEnumerable<MyZeroActivityApplications>> HandleMyApplicationInfo(IEnumerable<MyZeroActivityApplications> userApplies)
        {
            var handleResult = new List<MyZeroActivityApplications>();
            var myApplies = userApplies.GroupBy(apply => apply.Period)
                                    .Select(group => group.FirstOrDefault()).ToList();
            var products = await SelectProductInfoByPids(userApplies.Select(_ => _.PID).ToList());
            foreach (var myApply in myApplies.Where(_ => _ != null).ToList())
            {
                var activityDetail = await FetchZeroActivityDetailAsync(myApply.Period);
                var product = products.FirstOrDefault(_ => _.Pid == myApply.PID);
                handleResult.Add(new MyZeroActivityApplications
                {
                    Period = myApply.Period,
                    ApplyDateTime = myApply.ApplyDateTime,
                    PID = myApply.PID,
                    OrderID = myApply.OrderID,
                    StartDateTime = activityDetail.StartDateTime,
                    EndDateTime = activityDetail.EndDateTime,
                    ProductName = product?.DisplayName,
                    ImgUrl = product?.Image,
                    StatusOfActivity = (DateTime.Now < activityDetail.StartDateTime) ? 0 : ((DateTime.Now < activityDetail.EndDateTime) ? 1 : ((DateTime.Now < activityDetail.EndDateTime.AddDays(30) ? 3 : 4)))
                });
            }
            return handleResult;
        }

        public static async Task<IEnumerable<MyZeroActivityApplications>> SelectMyApplicationsAsync(Guid userId, int applicationStatus)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var userApplies = await client.GetOrSetAsync(GlobalConstant.MyZeroApplyApplications + userId.ToString("B") + "/" + applicationStatus + "/", () => DalZeroActivity.SelectMyApplicationsAsync(userId, applicationStatus), GlobalConstant.MyZeroApplyApplicationsExpiration);
                if (userApplies.Success && userApplies.Value != null)
                {
                    return await HandleMyApplicationInfo(userApplies.Value);
                }
                else
                {
                    if (!userApplies.Success)
                        Logger.Warn($"获取用户众测申请列表缓存redis失败MyZeroApplyApplications：{GlobalConstant.MyZeroApplyApplications + userId.ToString("B") + "/" + applicationStatus + "/"};Error:{userApplies.Message}", userApplies.Exception);
                    return await HandleMyApplicationInfo(await DalZeroActivity.SelectMyApplicationsAsync(userId, applicationStatus));
                }
            }
        }

        public static async Task<int> SubmitZeroActivityApplicationAsync(ZeroActivityRequest requestModel)
        {
            try
            {
                var activityDetail = await FetchZeroActivityDetailAsync(requestModel.Period);
                if (activityDetail == null)
                    return -2;
                else if (activityDetail.StatusOfActivity != 1)
                    return -3;
                else if (activityDetail.PID == null || activityDetail.ProductName == null)
                    return -4;
                if (await DalZeroActivity.HasZeroActivityApplicationSubmittedAsync(requestModel.UserId, requestModel.Period))
                {
                    return -1;
                }
                string userMobile = "";

                using (var uClient = new UserAccountClient())
                {
                    var result = await uClient.GetUserByIdAsync(requestModel.UserId);
                    if (result.Success && !string.IsNullOrWhiteSpace(result.Result?.MobileNumber) && result.Result.MobileNumber.Length <= 30)
                        userMobile = result.Result.MobileNumber;
                }
                var count = await DalZeroActivity.SubmitZeroActivityApplicationAsync(requestModel, activityDetail, userMobile);
                if (count > 0)
                {
                    var actionQueueDic = new Dictionary<string,string>();
                    actionQueueDic["UserId"] = requestModel.UserId.ToString("B");
                    actionQueueDic["ActionName"] = "12PublicTest";
                    TuhuNotification.SendNotification("notification.TaskActionQueue", actionQueueDic);
                    using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                    {
                        var existenceResult = await client.ExistsAsync(GlobalConstant.MyZeroApplyApplications + requestModel.UserId.ToString("B") + "/0/");
                        if (existenceResult.Success)
                        {
                            var removeResult = await client.RemoveAsync(GlobalConstant.MyZeroApplyApplications + requestModel.UserId.ToString("B") + "/0/");
                            if (!removeResult.Success)
                                Logger.Warn($"删除用户众测申请列表(申请中)缓存redis失败MyZeroApplyApplications：{GlobalConstant.MyZeroApplyApplications + requestModel.UserId.ToString("B") + "/0/"};Error:{removeResult.Message}", removeResult.Exception);
                        }
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                Logger.ErrorException("SubmitZeroActivityApplication逻辑进行中出现异常", ex);
                return 0;
            }
        }

        public static async Task<int> SubmitZeroActivityReminderAsync(Guid userId, int period)
        {
            var startTime = await DalZeroActivity.FetchStartTimeOfZeroActivityAsync(period);
            DateTime startDateTime;
            if (!DateTime.TryParse(startTime, out startDateTime))
                return -2;
            else if (DateTime.Now >= startDateTime)
                return -3;
            if (await DalZeroActivity.HasZeroActivityReminderSubmittedAsync(userId, period))
            {
                return -1;
            }
            return await DalZeroActivity.SubmitZeroActivityReminderAsync(userId, period);
        }

        public static async Task<bool> RefreshZeroActivityCache()
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivityPrefix"))
            {
                var setUnfinishedZAKeyResult = await client.SetAsync(GlobalConstant.UnfinishedZeroActivitiesForHomepage, RedisHelper.GenerateZeroActivityCacheKeyPrefix(GlobalConstant.UnfinishedZeroActivitiesForHomepage), GlobalConstant.ZeroActivitiesForHomepageExpiration);
                if (!setUnfinishedZAKeyResult.Success)
                {
                    Logger.Warn("更新未完成零元购活动列表缓存Redis缓存失败", setUnfinishedZAKeyResult.Exception);
                }
                var setFinishedZAKeyResult = await client.SetAsync(GlobalConstant.FinishedZeroActivitiesForHomepage, RedisHelper.GenerateZeroActivityCacheKeyPrefix(GlobalConstant.FinishedZeroActivitiesForHomepage), GlobalConstant.ZeroActivitiesForHomepageExpiration);
                if (!setFinishedZAKeyResult.Success)
                {
                    Logger.Warn("更新已完成零元购活动列表缓存Redis缓存失败", setFinishedZAKeyResult.Exception);
                }
                var setZADetailKeyResult = await client.SetAsync(GlobalConstant.ZeroActivityDetail, RedisHelper.GenerateZeroActivityCacheKeyPrefix(GlobalConstant.ZeroActivityDetail), GlobalConstant.ZeroActivityDetailExpiration);
                if (!setZADetailKeyResult.Success)
                {
                    Logger.Warn("更新零元购活动详情缓存Redis缓存失败", setZADetailKeyResult.Exception);
                }
                return setUnfinishedZAKeyResult.Success && setFinishedZAKeyResult.Success && setZADetailKeyResult.Success;
            }
        }
    }
}
