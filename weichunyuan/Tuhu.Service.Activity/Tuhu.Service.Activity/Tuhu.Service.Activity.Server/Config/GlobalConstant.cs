using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Nosql;

namespace Tuhu.Service.Activity.Server.Config
{
    public class GlobalConstant
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(GlobalConstant));

        public const string ActivityDeafultKey = "ActivityDeafult";

        public static string VipCardBindUrl = ConfigurationManager.AppSettings["VipcardBindUrl"];
        public static string AllPlaceLimitId = ConfigurationManager.AppSettings["AllPlaceLimitId"];
        public static string AllPlaceLimitDate = ConfigurationManager.AppSettings["AllPlaceLimitDate"];
        public const string VipCardPrefix = "VipCardPrefix";
        public const string VipCardCounterPrefix = "VipCardCounterPrefix";


        /// <summary>
        /// 统一前缀缓存key的缓存 key
        /// key:ActivityDeafultCachPrefix/{cacheKey}
        /// </summary>
        public static readonly string ActivityDeafultCachPrefix = "ActivityDeafultCachPrefix/{cacheKey}";
        /// <summary>
        /// 统一前缀缓存key的缓存value
        /// value:{cacheKey}/{timespansecond}
        /// </summary>
        public static readonly string ActivityDeafultCachPrefixValue = "{cacheKey}/{timespansecond}";
        public static readonly TimeSpan ActivityDeafultCachPrefixSpan = TimeSpan.FromDays(30);

        public static TimeSpan VehicleAdapterCacheExpiration = TimeSpan.FromMinutes(30);
        public static string AbsolutePathPrefix = "http://image.tuhu.cn";
        public static string ActivityConfigForDownloadApp = "/GetDownloadAppById/";
        public static TimeSpan FlashSaleCacheExpiration = TimeSpan.FromDays(1);
        public static TimeSpan SeckillDefaultCacheExpiration = TimeSpan.FromDays(30);
        public static string FlashSaleCacheKeyPrefix = "FlashSale/";
        public static string FlashSaleDataCacheKeyPrefix = "FlashSaleNewData/";
        public static string FlashSaleProductInstallTagKeyPrefix = "FlashSaleProductInstallTag/";
        public static string FlashSaleSaleQuantityCacheKeyPrefix = "FlashSaleSaleQuantity/";
        public static TimeSpan FlashSaleSaleQuantityCacheExpiration = TimeSpan.FromSeconds(15);
        public static TimeSpan SecondKillTodayCacheExpiration = TimeSpan.FromDays(1);
        public static string HomepageModeuleShowZeroActivityKey = "hmszaprefix";
        public static string HomepageZeroActivityListKey = "HomepageZeroActivityListKey";
        public static string UnfinishedZeroActivitiesForHomepage = "UnfinishedZeroActivitiesForHomepageNew/";
        public static string ProductInfoOfUnfinishedZeroActivities = "ProductInfoOfUnfinishedZeroActivitiesNew/";
        public static string FinishedZeroActivitiesForHomepage = "FinishedZeroActivitiesForHomepageNew/";
        public static string ProductInfoOfFinishedZeroActivities = "ProductInfoOfFinishedZeroActivitiesNew/";
        public static TimeSpan ZeroActivitiesForHomepageExpiration = TimeSpan.FromMinutes(1);
        public static string ZeroActivityDetail = "ZeroActivityDetailNew/";
        public static string ProductInfoOfZeroActivityDetail = "ProductInfoOfZeroActivityDetailNew/";
        public static TimeSpan ZeroActivityDetailExpiration = TimeSpan.FromMinutes(1);
        public static string TestReportsOfPeriod = "TestReportsOfPeriod1/";
        public static TimeSpan TestReportsOfPeriodExpiration = TimeSpan.FromHours(2);
        public static string TestReportDetail = "TestReportDetail/";
        public static TimeSpan TestReportDetailExpiration = TimeSpan.FromHours(2);
        public static string MyZeroApplyApplications = "MyZeroApplyApplicationsNew/";
        public static TimeSpan MyZeroApplyApplicationsExpiration = TimeSpan.FromMinutes(1);
        public static TimeSpan productInfoExpiration = TimeSpan.FromMinutes(10);
        public static string NumOfApplications = "NumOfApplicationsNew/";
        public static TimeSpan NumOfApplicationsExpiration = TimeSpan.FromMinutes(1);

        public static readonly string ActivityPageClientName = "ActivityPageV2";
        public static readonly string ActivityPagePrefix = "ActivityPagePrefix2";
        public static readonly string LuckWheelPrefix = "LuckWheelPrefix";
        public static TimeSpan ActivityPageCache = TimeSpan.FromDays(30);

        public static readonly string ShareActivityClientName = "ShareActivity";
        public static readonly string ShareActivityKey = "ShareActivityCache";

        public static readonly string ActivityBuildKey = "ActivityBuild_1";
        public static TimeSpan ActivityBuildSpan = TimeSpan.FromHours(1);

        public static readonly string ActivityType = "ActivityType2";
        public static readonly int VerifyActivityNew = Convert.ToInt32(ConfigurationManager.AppSettings["VerifyActivityNew"]);

        public static readonly string BargainActivityName = "BargainActivity";
        public static string BargainActivityKey = "BargainActivityKey";
        public static TimeSpan BargainActivitySpan = TimeSpan.FromHours(1);


        public static readonly string ShareBargainName = "ShareBargainActivity";
        public static readonly string ShareBargainKey = "ShareBargainActivitykey";
        public static TimeSpan ShareBargainSpan = TimeSpan.FromHours(1);
        public static Guid BargainActivityId = new Guid("db5f63c4-1084-48ee-b1c7-459922403118");
        public static readonly string ShareBargainHashKey = "ShareBargainHashKey";


        public static readonly string ShareBargainSliceShowKey = "SliceShowCacheKey";

        public static readonly string BargainProductName = "BargainProductTable";
        public static readonly string BargainOwnerName = "BargainOwnerTable";

        public static readonly string GroupBuyingActivityName = "GroupBuyingActivity";
        public static readonly string GroupBuyingActivityKey = "GroupBuyingActivityKey2";
        public static readonly string GroupBuyingCategoryKey = "GroupBuyingCategoryKey";
        public static readonly string GroupBuyingListNewKey = "GroupBuyingListNewKey";
        public static TimeSpan GroupBuyingActivitySpan = TimeSpan.FromHours(12);

        // 拼团-团长免单券
        public static readonly string GroupFreeCouponCacheName = "GroupBuyingFreeCouponCache";
        public static TimeSpan GroupFreeCouponSpan = TimeSpan.FromSeconds(10);

        public static readonly string GroupBuyingHashCacheName = "GroupBuyingHashCacheName";

        public static TimeSpan GroupBuyingFreeCouponSpan = TimeSpan.FromHours(72);
        public static readonly int GroupBuyingProductCount = 5;

        public static string FlashSaleCacheKeyPrefix2 = "FlashSale2/";

        #region 蓄电池/加油卡 活动配置

        /// <summary>
        /// 活动配置缓存ClientName
        /// </summary>
        public static readonly string CouponActivityConfigName = "CouponActivityConfig";

        /// <summary>
        /// 活动配置缓存Key
        /// {ActivityNum}{Type}
        /// </summary>
        public static readonly string CouponActivityConfigKey = "CouponActivityConfig/{0}/{1}";

        /// <summary>
        /// 活动配置详情Key
        /// {ConfigId}
        /// </summary>
        public static readonly string CouponActivityChannelConfigKey = "CouponActivityChannelConfig/{0}";

        /// <summary>
        /// 活动配置缓存过期时间
        /// </summary>
        public static TimeSpan LazyExpireTime = TimeSpan.FromMinutes(20);

        #endregion


        public static readonly string TireChangedActivityKey = "TireChangedActivity/{VehicleId}/{TireSize}";
        public static TimeSpan TireChangedActivitySpan = TimeSpan.FromHours(1);

        //轮胎节活动时间
        public static readonly DateTime TyreFestivalActivityBeginTime = Convert.ToDateTime("2018-3-16 00:00:00");
        public static readonly DateTime TyreFestivalActivityEndTime = Convert.ToDateTime("2018-4-1 23:59:59");

        public static readonly Dictionary<int, Guid> GroupBuyingActivityId = new Dictionary<int, Guid>
        {
            [0] = new Guid("E579BA1C-65DE-477C-94E9-9578DF995675"),
            [1] = new Guid("4A5AE130-C802-4158-BB9A-76ABB3164FBE"),
            [2] = new Guid("A5AB52A5-5394-4598-AC34-9517ECE17C22")
        };

        public static string WhiteListKeyPrefix = "WhiteListKeyPrefix";
        public static string ActivityPageInfoPrefix = "activeinfo";
        /// <summary>
        /// 为cachekey追加时间戳,并缓存到redis
        /// key:ActivityDeafultCachPrefix/{cacheKey}
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns>value:{cacheKey}/{timespansecond}</returns>
        public static string GetCacheKeyPrefixWithCache(string cacheKey, string clientName = ActivityDeafultKey)
        {
            var cachePrefixKey = ActivityDeafultCachPrefix.Replace("{cacheKey}", cacheKey);
            var cachePrefixValue = ActivityDeafultCachPrefixValue
                    .Replace("{cacheKey}", cacheKey)
                    .Replace("{timespansecond}", DateTime.Now.ToFileTimeUtc().ToString());

            using (var client = CacheHelper.CreateCacheClient(clientName))
            {
                var result = client.GetOrSet(cachePrefixKey, () => cachePrefixValue, ActivityDeafultCachPrefixSpan);

                if (result.Success)
                    return result.Value;
                else
                {
                    Logger.Warn("redis查询失败 休眠100ms后重试一次");
                    Thread.Sleep(100);
                    using (var client2 = CacheHelper.CreateCacheClient(clientName))
                    {
                        result = client2.GetOrSet(cachePrefixKey, () => cachePrefixValue, ActivityDeafultCachPrefixSpan);
                        if (result.Success)
                            return result.Value;

                        Logger.Error("redis查询失败2");
                        return cacheKey;
                    }

                }
            }
        }
        public static string GenerateKeyPrefix(string prefix)
        {
            return DateTime.Now.Ticks + prefix;
        }

        public static async Task<string> GetKeyPrefix(string key, string clientName)
        {
            using (var client = CacheHelper.CreateCacheClient(clientName))
            {
                var result = await client.GetAsync<string>(key);
                return result.Success ? result.Value : key;
            }
        }

        /// <summary>
        /// 车型认证 用户权益 hashkey
        /// </summary>
        public static List<string> HashKey_VehicleTypeCertificationRights = new List<string>() { "23338FF5", "870F1F2E", "7C94860A" };

        /// <summary>
        /// 微信号以及对应的二维码
        /// </summary>
        public static readonly Dictionary<string, string> WechatQrcodeURL = new Dictionary<string, string>
        {
             //途虎养车
            { "WX_APP_OfficialAccount","https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQEH8jwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyRWxKUjE4WS05ZFAxMDAwME0wM1QAAgSldY9bAwQAAAAA"},
            //新汽车志公众号//subcribe
            { "WX_OA_NewCarMagazine","https://img2.tuhu.org/activity/image/57b0/ccf2/c379af4b839cb0db9b12bfd5_w258_h258.jpg"},
            //养车开发版
            { "WX_APP_YangCheKaiFaBan","https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQEE8TwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyNVZPLUlfUGtkWWgxMDAwME0wM0UAAgRQdo9bAwQAAAAA"},
            //途虎工单
            { "WX_OA_WorkOrder","https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQFB8TwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyNTdYLXRDZUVlSDMxMDAwME0wM0EAAgR0do9bAwQAAAAA"},
            //途虎一元团
            { "WX_OA_OneYuanBuy","https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQE48TwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyVDF2QlZLWHVmamwxMDAwMGcwM3EAAgSsSI9bAwQAAAAA"},
            //途虎供应商
            { "WX_OA_Supplier","https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQEx8TwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyTjZ1YUFHNW9jaW0xMDAwMHcwM2QAAgSqdo9bAwQAAAAA"},
            //泰哥会搞车//subcribe
            { "WX_OA_TigerGaoChe","https://img3.tuhu.org/activity/image/3ae9/2655/12c4b2373085d0b3922adce4_w1280_h1280.jpg"},
            //途虎快送
            { "WX_OA_TuhuExpress","https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQEB8TwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyNFlTTVZ4ZjFmaWoxMDAwMHcwM3AAAgQWH5JbAwQAAAAA"},
            //途虎二手车公众号
            { "WX_OA_SecondCar","https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQGg8DwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyTWtMVHdseUNjSmwxMDAwMGcwM3YAAgTodo9bAwQAAAAA"},
            //越野英雄谱公众号//subcribe
            { "WX_OA_YYHeroList","https://img3.tuhu.org/activity/image/b6ab/a722/78dabc9033743f71c8a85b09_w258_h258.jpg"},
            //途虎养车公众号
            { "WX_OA_CarMaintenance","https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQG18DwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyeHBFNDBrOFRkWWwxMDAwMHcwM20AAgQqd49bAwQAAAAA"},
            //途虎车主俱乐部公众号//subcribe
            { "WX_OA_CarOwnerClub","https://img2.tuhu.org/activity/image/8509/824d/cb459300a3d584893612e035_w1280_h1280.jpg"},
        };



        #region 公众号领红包

        /// <summary>
        ///     公众号领红包 - 缓存头
        /// </summary>
        public const string OARedEnvelopeCacheHeader = "OARedEnvelope";

        /// <summary>
        ///     公众号领红包 - 总数
        /// </summary>
        public const string OARedEnvelopeCountCache = "OARedEnvelopeCount";

        /// <summary>
        ///     公众号领红包 - 统计
        /// </summary>
        public const string OARedEnvelopeStatisticsCache = "OARedEnvelopeStatistics";


        /// <summary>
        ///     公众号领红包 -  用户领取
        /// </summary>
        public const string OARedEnvelopeUserGetCache = "OARedEnvelopeUserGet:{0}:{1}";
 


        #endregion

        #region 七龙珠

        /// <summary>
        ///      七龙珠缓存头
        /// </summary>
        public const string DragonBallCacheHeader = "DragonBallGame";

        /// <summary>
        ///     七龙珠用户锁Header
        /// </summary>
        public const string DragonBallUserLockHeader = "DragonBallUserLockHeader";

        /// <summary>
        ///     7颗龙珠 飞升啦
        /// </summary>
        public const int DragonBallCountDefine = 7;

        #endregion
    }
}
