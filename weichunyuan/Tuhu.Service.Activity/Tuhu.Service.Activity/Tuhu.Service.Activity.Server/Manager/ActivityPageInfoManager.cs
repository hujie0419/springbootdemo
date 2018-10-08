using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.DataAccess.Models.Activity;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models.Activity;
using Tuhu.Service.Activity.Models.Requests.Activity;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models.New;
using Tuhu.Service.Product.Models.ProductConfig;
using Tuhu.Service.Product.Request;
using FlashSaleProductModel = Tuhu.Service.Activity.Models.FlashSaleProductModel;

namespace Tuhu.Service.Activity.Server.Manager
{
    public abstract class ActivityPageInfoBaseManager<TIn, TOut>
        where TIn : ActivityPageInfoModuleBaseRequest
        where TOut : ActivityPageInfoRowBase
    {
        protected abstract ILog Logger { get; }
        protected abstract string Key { get; set; }
        protected virtual TimeSpan ExpireTime => TimeSpan.FromDays(1);
        protected abstract string HashKey { get; set; }
        protected abstract string Channel { get; set; }
        protected abstract List<int> Types { get; set; }
        protected static string ClientName => GlobalConstant.ActivityPageClientName;
        protected static string Prefix => GlobalConstant.ActivityPageInfoPrefix;
        protected static Task<string> GetKeyPrefix() => GlobalConstant.GetKeyPrefix(Prefix, ClientName);

        #region 【缓存hashkey跟主键对应关系】

        private async Task<int> GetActivityIdByHashKey(string hashKey)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var keyprefix = await GetKeyPrefix();
                var result = await client.GetOrSetAsync($"activityId{keyprefix}{hashKey}",
                    () => DalActivity.FetchActivePageListModelIdasync(hashKey), ExpireTime);
                if (result.Success)
                {
                    return result.Value;
                }
                else
                {
                    Logger.Error($"从redis缓存中根据活动hashkey{hashKey}获取活动pkid失败");
                    return 0;
                }
            }
        }

        #endregion

        #region 【缓存行跟配置对应关系】

        protected async Task<List<T>> GetActivityRowConfig<T>(List<string> rowNums)
            where T : ActivityPageInfoRowBase, new()
        {
            var keyprefix = await GetKeyPrefix();
            var result = new List<T>();
            using (var hashclient = CacheHelper.CreateHashClient(keyprefix + Key, ExpireTime))
            {
                IResult<IDictionary<string, List<T>>> cacheresult = null;
                if (rowNums != null && rowNums.Any())
                {
                    cacheresult = await hashclient.GetAsync<List<T>>(rowNums);
                }
                else
                {
                    cacheresult = await hashclient.GetAllAsync<List<T>>();
                }
                if (!cacheresult.Success)
                {
                    Logger.Error($"GetActivityRowConfig,CreateHashClientError活动为{HashKey}");
                }
                else
                {
                    if (cacheresult.Value != null && cacheresult.Value.Any())
                    {
                        result = cacheresult.Value.Values.SelectMany(r => r).ToList();
                    }
                    else
                    {
                        var fkId = await GetActivityIdByHashKey(HashKey);
                        var dbs = await DalActivityPage.SelectActivePageInfoCells<T>(fkId, Types, Channel);
                        var dic = dbs.Select(r => r.RowNum)
                            .Distinct()
                            .ToDictionary<string, string, object>(item => item,
                                item => dbs.Where(r => r.RowNum == item).ToList());
                        IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                        var iResult = await hashclient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                        if (!iResult.Success)
                        {
                            Logger.Error($"set hash结构redis缓存失败，活动是{HashKey}");
                        }
                        if (rowNums != null && rowNums.Any())
                        {
                            result = dbs.Where(r => rowNums.Contains(r.RowNum)).ToList();
                        }
                        else
                        {
                            result = dbs.ToList();
                        }
                    }
                }
                return result;
            }
        }

        #endregion

        #region 【类型转换】

        protected int ConvertType(int productType, string categoryName = null)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                switch (productType)
                {
                    case 0:
                        return (int)ActivityPageContentType.ATire;
                    case 1:
                        return (int)ActivityPageContentType.ACarProduct;
                    case 2:
                        return (int)ActivityPageContentType.ALunGu;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (categoryName.ToLower())
                {
                    case "tires":
                        return (int)ActivityPageContentType.ATire;
                    case "autoproduct":
                        return (int)ActivityPageContentType.ACarProduct;
                    case "hub":
                        return (int)ActivityPageContentType.ALunGu;
                    default:
                        return 0;
                }
            }
        }

        #endregion

        public abstract Task<List<TOut>> GetActivityPageInfoRowByTypes(TIn request);

    }

    #region 车型头图

    public class ActivityPageInfoVehicleBannerManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleVehicleBannerRequest, ActivityPageInfoRowVehicleBanner>
    {
        #region 【车型头图信息】

        private async Task<List<VehicleBanner>> GetVechicleBannerAsync(string hashKey, string group, int col)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var keyprefix = await GetKeyPrefix();
                var result = await client.GetOrSetAsync($"{keyprefix}{hashKey}/{group}/{col}",
                    () => DalActivity.SelectVehicleBannerAsync(hashKey, group, col), ExpireTime);
                if (result.Success)
                {
                    return result.Value;
                }
                else
                {
                    Logger.Error($"redis缓存失败GetVechicleBannerAsync");
                    return null;
                }
            }
        }

        #endregion

        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoVehicleBannerManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowVehicleBanner>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleVehicleBannerRequest request)
        {
            this.Key = $"vbanner{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.VehicleBanner
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoVehicleBanner>(request.RowNums);
            foreach (var item in config)
            {
                if (item.IsVehicle)
                {
                    var bannerlist = await GetVechicleBannerAsync(request.HashKey, item.RowNum, 1);
                    var banner =
                        bannerlist.FirstOrDefault(r => r.VehicleId.ToLower().Equals(request.VehicleId.ToLower()));
                    if (banner != null)
                        item.Image = banner.ImageUrl;
                }
            }
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowVehicleBanner()
            {
                RowNum = r.Key,
                ActivityPageInfoCellVehicleBanners = r.Select(p => new ActivityPageInfoCellVehicleBanner()
                {
                    LinkUrl = p.LinkUrl,
                    WxAppUrl = p.WxAppUrl,
                    Image = p.Image
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 图片

    public class ActivityPageInfoImageManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowImage>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoImageManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowImage>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"image{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Image
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoImage>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowImage()
            {
                RowNum = r.Key,
                ActivityPageInfoCellImages = r.Select(p => new ActivityPageInfoCellImage()
                {
                    Image = p.Image,
                    FileUrl = p.FileUrl,
                    IsUploading = p.IsUploading
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 图片

    public class ActivityPageInfoSeckillManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowSeckill>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoImageManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowSeckill>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"seckill{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.SecondKill
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoSeckill>(request.RowNums);
            return config.GroupBy(r => new
            {
                r.RowNum,
                r.RowType
            }).Select(r => new ActivityPageInfoRowSeckill()
            {
                RowNum = r.Key.RowNum,
                RowType = r.Key.RowType,
                ActivityPageInfoCellSeckills = r.Select(p => new ActivityPageInfoCellSeckill()
                {
                    Image = p.Image,
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 链接

    public class ActivityPageInfoLinkManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowLink>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoLinkManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowLink>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"link{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Link
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoLink>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowLink()
            {
                RowNum = r.Key,
                ActivityPageInfoCellLinks = r.Select(p => new ActivityPageInfoCellLink()
                {
                    LinkUrl = p.LinkUrl,
                    AppUrl = p.AppUrl,
                    PcUrl = p.PcUrl,
                    WxAppId = p.WxAppId,
                    WxAppUrl = p.WxAppUrl,
                    IsUploading = p.IsUploading,
                    Image = p.Image
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 产品

    public class ActivityPageInfoProductManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowProduct>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoProductManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowProduct>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"product{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Tire,
                (int) ActivityPageContentType.ATire,
                (int) ActivityPageContentType.LunGu,
                (int) ActivityPageContentType.ALunGu,
                (int) ActivityPageContentType.ACarProduct,
                (int) ActivityPageContentType.CarProduct,
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoProduct>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowProduct()
            {
                RowNum = r.Key,
                ActivityPageInfoCellProducts = r.Select(p => new ActivityPageInfoCellProduct()
                {
                    Pid = p.Pid,
                    ActivityId = p.ActivityId,
                    IsUploading = p.IsUploading,
                    DisplayWay = p.DisplayWay,
                    IsReplace = p.IsReplace,
                    Type = p.Type
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 优惠券

    public class ActivityPageInfoCouponManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowCoupon>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoCouponManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowCoupon>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"Coupon{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Coupon
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoCoupon>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowCoupon()
            {
                RowNum = r.Key,
                ActivityPageInfoCellCoupons = r.Select(p => new ActivityPageInfoCellCoupon()
                {
                    Cid = p.Cid,
                    IsUploading = p.IsUploading,
                    Image = p.Image
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 保养

    public class ActivityPageInfoByManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowBy>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoByManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowBy>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"By{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Baoyang
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoBy>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowBy()
            {
                RowNum = r.Key,
                ActivityPageInfoCellBys = r.Select(p => new ActivityPageInfoCellBy()
                {
                    IsRecommended = p.IsRecommended,
                    IsLogin = p.IsLogin,
                    IsTireStandard = p.IsTireStandard,
                    IsTireSize = p.IsTireSize,
                    IsHiddenTtile = p.IsHiddenTtile,
                    ByService = p.ByService,
                    ByActivityId = p.ByActivityId,
                    Vehicle = p.Vehicle,
                    IsUploading = p.IsUploading,
                    LinkUrl = p.LinkUrl,
                    AppUrl = p.AppUrl,
                    PcUrl = p.PcUrl,
                    WxAppId = p.WxAppId,
                    WxAppUrl = p.WxAppUrl,
                    Image = p.Image
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 活动规则

    public class ActivityPageInfoRuleManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowRule>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoRuleManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowRule>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"Rule{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Rule
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoRule>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowRule()
            {
                RowNum = r.Key,
                ActivityPageInfoCellRules = r.Select(p => new ActivityPageInfoCellRule()
                {
                    Image = p.Image,
                    Description = p.Description,
                    IsUploading = p.IsUploading
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 其他

    public class ActivityPageInfoOtherManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowOther>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoOtherManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowOther>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"Other{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Other
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoOther>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowOther()
            {
                RowNum = r.Key,
                ActivityPageInfoCellOthers = r.Select(p => new ActivityPageInfoCellOther()
                {
                    IsUploading = p.IsUploading,
                    LinkUrl = p.LinkUrl,
                    AppUrl = p.AppUrl,
                    PcUrl = p.PcUrl,
                    WxAppId = p.WxAppId,
                    WxAppUrl = p.WxAppUrl,
                    Image = p.Image,
                    OthersType = p.OthersType
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 其他活动

    public class ActivityPageInfoOtherActivityManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowOtherActivity>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoOtherActivityManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowOtherActivity>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"OtherActivity{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Luckwheel,
                (int) ActivityPageContentType.BaoyangPrice,
                (int) ActivityPageContentType.NewLuckyWheel,
                (int) ActivityPageContentType.Packs,
                (int) ActivityPageContentType.QAlottery,
                (int) ActivityPageContentType.Luckylottery
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoOtherActivity>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowOtherActivity()
            {
                RowNum = r.Key,
                ActivityPageInfoCellOtherActivitys = r.Select(p => new ActivityPageInfoCellOtherActivity()
                {
                    IsUploading = p.IsUploading,
                    ActivityId = p.ActivityId ?? p.HashKey,
                    Image = p.Image,
                    Type = p.Type
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 视频

    public class ActivityPageInfoVideoManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowVideo>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoVideoManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowVideo>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"Video{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Video
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoVideo>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowVideo()
            {
                RowNum = r.Key,
                ActivityPageInfoCellVideos = r.Select(p => new ActivityPageInfoCellVideo()
                {
                    IsUploading = p.IsUploading,
                    LinkUrl = p.LinkUrl,
                    AppUrl = p.AppUrl,
                    PcUrl = p.PcUrl,
                    WxAppId = p.WxAppId,
                    WxAppUrl = p.WxAppUrl,
                    Image = p.Image,
                    Description = p.Description
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 拼团

    public class ActivityPageInfoPintuanManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowPintuan>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoVideoManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowPintuan>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"Pintuan{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.ProductGroup
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoPintuan>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowPintuan()
            {
                RowNum = r.Key,
                ActivityPageInfoCellPintuans = r.Select(p => new ActivityPageInfoCellPintuan()
                {
                    IsUploading = p.IsUploading,
                    ProductGroupId = p.ProductGroupId,
                    Image = p.Image,
                    Pid = p.Pid
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 【文字链，滑动优惠券】

    public class ActivityPageInfoJsonManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowJson>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoJsonManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowJson>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"Json{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.ScrollTextChain,
                (int) ActivityPageContentType.SlipCoupon,
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoJson>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowJson()
            {
                RowNum = r.Key,
                ActivityPageInfoCellJsons = r.Select(p => new ActivityPageInfoCellJson()
                {
                    IsUploading = p.IsUploading,
                    JsonContent = p.JsonContent,
                    Type = p.Type
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 【文案】

    public class ActivityPageInfoActivityTextManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowActivityText>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoActivityTextManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowActivityText>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"ActivityText{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.ActiveText,
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoActivityText>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowActivityText()
            {
                RowNum = r.Key,
                ActivityPageInfoCellActivityTexts = r.Select(p => new ActivityPageInfoCellActivityText()
                {
                    IsUploading = p.IsUploading,
                    ActiveText = p.ActiveText,
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 【倒计时】

    public class ActivityPageInfoCountDownManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowCountDown>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoCountDownManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        public override async Task<List<ActivityPageInfoRowCountDown>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"CountDown{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.CountDown,
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoCountDown>(request.RowNums);
            return config.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowCountDown()
            {
                RowNum = r.Key,
                ActivityPageInfoCellCountDowns = r.Select(p => new ActivityPageInfoCellCountDown()
                {
                    IsUploading = p.IsUploading,
                    CountDownStyle = p.CountDownStyle
                }).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 【新商品池】

    public class ActivityPageInfoNewProductPoolManager :
        ActivityPageInfoBaseManager<ActivityPageInfoModuleNewProductPoolRequest, ActivityPageInfoRowNewProductPool>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoNewProductPoolManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }


        #region 私有方法
        private async Task<List<ActivePageContentModel>> SelectNewProductPoolProductsAsCache(
ActivityPageInfoModuleNewProductPoolRequest request, DalActivityPageInfoNewProductPool item)
        {
            var sortedContents = new List<ActivePageContentModel>();
            var categorys = item.ProductType == (int)ProductType.Default
                ? new[] { nameof(ProductType.Tires), nameof(ProductType.hub), nameof(ProductType.AutoProduct) }
                : item.ProductType == (int)ProductType.Tires
                    ? new[] { nameof(ProductType.Tires) }
                    : item.ProductType == (int)ProductType.hub
                        ? new[] { nameof(ProductType.hub) }
                        : new[] { nameof(ProductType.AutoProduct) };
            var searchRequest = new SearchProductRequest()
            {
                VehicleId = item.VehicleLevel == 2 ? request.VehicleId : null,
                Tid = item.VehicleLevel == 5 ? request.Tid : null,
                CurrentPage = 1,
                OrderType = 1,
                PageSize = 1000,
                JustAdpter = true,
                Parameters = new Dictionary<string, IEnumerable<string>>()
                {
                    ["CP_Brand"] = new[] { item.Brand },
                    ["CP_Tire_Rim"] = new[] { request.Rim },
                    ["CP_Tire_Width"] = new[] { request.Width },
                    ["CP_Tire_AspectRatio"] = new[] { request.AspectRatio },
                    ["OnSale"] = new[] { "1" },
                    ["stockout"] = new[] { "0" },
                    ["Category"] = categorys,
                    ["StockOutExceptCategory"] = new[] { "Tires" }
                }
            };
            var adaptAllPidList = await SearchProductByPidsAsync(searchRequest);
            var sortedPidsRequest = new SortedPidsRequest
            {
                DicActivityId = new KeyValuePair<string, ActivityIdType>(item.SystemActivityId,
                    ActivityIdType.AutoActivity),
                AdaptPids = adaptAllPidList,
                ProductType = ProductType.Default
            };
            var sortedList = new List<string>();
            if (item.VehicleLevel != 0)
            {
                try
                {
                    sortedList =
                        (await ActivityManager.GetOrSetActivityPageSortedPidsAsync(sortedPidsRequest)).Item2;
                }
                catch (Exception e)
                {
                    Logger.Error("调用排序好的活动页适配产品失败", e.InnerException);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(item.SystemActivityId))
                    sortedList =
                        (await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(new Guid(item.SystemActivityId)))
                        .Products?.Select(p => p.PID)?.ToList();
            }
            if (sortedList == null || !sortedList.Any())
            {
                Logger.Info(
                    $"新商品池获取排序好的商品列表为空活动{request.HashKey};组号{item.RowNum}sortedList:{JsonConvert.SerializeObject(sortedList)}sortedPidsRequest:{JsonConvert.SerializeObject(sortedPidsRequest)}");
            }
            else
            {
                var needReturnPids = item.RowLimit == 0
                    ? sortedList
                    : sortedList.Take(item.RowLimit * item.ColumnNumber).ToList();
                var products = (await SelectSkuProductDetailModels(needReturnPids)).ToList();
                var flashSale = new FlashSaleModel()
                {
                    Products = new List<FlashSaleProductModel>()
                };
                if (item.ActivityType == (int)ActivityIdType.FlashSaleActivity)
                {
                    flashSale =
                        await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(item.ActivityId.Value);
                }
                var query = (from product in products
                             join actvity in flashSale.Products on product.Pid equals actvity.PID into temp
                             from actvity in temp.DefaultIfEmpty()
                             select new
                             {
                                 product,
                                 actvity
                             });
                var innerContents = new List<ActivePageContentModel>();
                foreach (var item1 in query)
                {
                    innerContents.Add(new ActivePageContentModel
                    {
                        DisplayWay = item.DisplayWay,
                        ActivityId = item1.actvity?.ActivityID,
                        Pid = item1.product.Pid,
                        ProductName = item1.actvity == null ? item1.product.DisplayName : item1.actvity.ProductName,
                        Type = ConvertType(item.ProductType, item1.product.RootCategoryName),
                        ProductType = item.ProductType,
                        IsAdapter = item.IsAdapter,
                        Image = item1.product.Image.GetImageUrl(),
                        Price = item1.actvity?.FalseOriginalPrice ?? item1.product.Price,
                        ActivityPrice = item1.actvity?.Price ?? item1.product.Price,
                        AdvertiseTitle = item1.product.ShuXing5,
                        TotalQuantity = item1.actvity?.TotalQuantity,
                        SaleOutQuantity = item1.actvity?.SaleOutQuantity ?? 0,
                        StartDateTime = flashSale?.StartDateTime ?? DateTime.MinValue,
                        EndDateTime = flashSale?.EndDateTime ?? DateTime.MinValue,
                        TireSize = item1.product.Size.ToString(),
                        Onsale = item1.product.Onsale,
                        Pattern = item1.product.Pattern,
                        SpeedRating = item1.product.SpeedRating,
                        IsUsePcode = item1.actvity?.IsUsePCode ?? false,
                        Brand = item1.product.Brand,
                        Category = item1.product.RootCategoryName,
                        Group = item.RowNum,
                        ColumnNumber = item.ColumnNumber,
                        TemplateType = item.TemplateType,
                        IsProgressBar = item.IsProgressBar,
                        IsBrandName = item.IsBrandName
                    });
                }

                sortedContents =
                    needReturnPids.Select(sortedPid => innerContents.FirstOrDefault(r => r.Pid == sortedPid))
                        .ToList();
            }

            return sortedContents;
        }


        private async Task<List<string>> SearchProductByPidsAsync(SearchProductRequest request)
        {
            var result = new List<string>();
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                using (var client = new ProductSearchClient())
                {
                    var pageModelResult = await client.SearchProductWithPidsAsync(request);
                    var data = pageModelResult.Result.Source.ToList();
                    result.AddRange(data);
                    if (pageModelResult.Result.Pager != null && pageModelResult.Result.Pager.TotalPage > 1)
                    {
                        for (int i = 2; i <= pageModelResult.Result.Pager.TotalPage; i++)
                        {
                            request.CurrentPage += 1;
                            var splitresult = await client.SearchProductWithPidsAsync(request);
                            if (splitresult.Result.Source != null && splitresult.Result.Source.Any())
                            {
                                result.AddRange(splitresult.Result.Source.ToList());
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                }

                sw.Stop();
                if (sw.ElapsedMilliseconds > 30)
                {
                    Logger.Info($"调用接口SearchProductByPidsAsync耗时{sw.ElapsedMilliseconds}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"调用接口SearchProductByPidsAsync报错", ex);
            }
            return result;
        }

        private async Task<IEnumerable<SkuProductDetailModel>> SelectSkuProductDetailModels(List<string> pids)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                using (var client = new ProductClient())
                {
                    var skuList = await client.SelectSkuProductListByPidsAsync(pids);
                    skuList.ThrowIfException();
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > 50)
                        Logger.Info($"活动页调用产品接口数据耗时==>{sw.ElapsedMilliseconds},接口耗时=》{skuList.ElapsedMilliseconds}");
                    return skuList.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"活动页调用产品接口失败Exception{ex.Message}{ex.InnerException}");
                return new List<SkuProductDetailModel>();
            }


        }

        private ActivityPageInfoCellNewProductPool ModelConvert(ActivePageContentModel p)
        {
            return new ActivityPageInfoCellNewProductPool()
            {
                IsAdapter = p.IsAdapter,
                Pid = p.Pid,
                ProductName = p.ProductName,
                ActivityId = p.ActivityId,
                Price = p.Price,
                ActivityPrice = p.ActivityPrice,
                Onsale = p.Onsale,
                Brand = p.Brand,
                Pattern = p.Pattern,
                SpeedRating = p.SpeedRating,
                Category = p.Category,
                IsNewUserFirstOrder = p.IsNewUserFirstOrder,
                AdvertiseTitle = p.AdvertiseTitle,
                IsUsePcode = p.IsUsePcode,
                TotalQuantity = p.TotalQuantity,
                SaleOutQuantity = p.SaleOutQuantity,
                StartDateTime = p.StartDateTime,
                EndDateTime = p.EndDateTime,
                IsShow = p.IsShow,
                DisplayWay = p.DisplayWay,
                Type = p.Type,
                ProductType = p.ProductType,
                TireSize = p.TireSize,
                Image = p.Image,
                IsProgressBar = p.IsProgressBar,
                TemplateType = p.TemplateType,
                IsBrandName = p.IsBrandName
            };
        }
        #endregion

        public override async Task<List<ActivityPageInfoRowNewProductPool>> GetActivityPageInfoRowByTypes(
            ActivityPageInfoModuleNewProductPoolRequest request)
        {
            this.Key = $"NewProductPool{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.NewProductPool,
            };
            var config = await GetActivityRowConfig<DalActivityPageInfoNewProductPool>(request.RowNums);
            var sortedContents = new List<ActivePageContentModel>();
            foreach (var item in config)
            {
                using (var client = CacheHelper.CreateCacheClient("Activity"))
                {
                    var key =
                        $"newpool{request.VehicleId}{request.Tid}{request.AspectRatio}{request.Rim}{request.Width}{request.HashKey}{item.RowNum}";
                    var cache = await client.GetOrSetAsync(key, () => SelectNewProductPoolProductsAsCache(request, item),
                        TimeSpan.FromSeconds(30));
                    if (cache.Success)
                    {
                        sortedContents.AddRange(cache.Value);
                    }
                    else
                    {
                        sortedContents.AddRange(await SelectNewProductPoolProductsAsCache(request, item));
                    }

                }
            }
            return sortedContents.GroupBy(r => new
            {
                r.Group,
                r.ColumnNumber
            }).Select(r => new ActivityPageInfoRowNewProductPool()
            {
                RowNum = r.Key.Group,
                ColumnNumber = r.Key.ColumnNumber,
                ActivityPageInfoCellNewProductPools = r.Select(ModelConvert).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 【老商品池】

    public class ActivityPageInfoProductPoolManager : ActivityPageInfoBaseManager<ActivityPageInfoModuleProductPoolRequest, ActivityPageInfoRowProductPool>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoProductPoolManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }

        #region 私有方法

        private async Task<FlashSaleModel> GetFlashSaleModelByFloor(FlashSaleModel productPool, string rim,
            Guid activityId, int cityId)
        {
            try
            {
                var swf = new Stopwatch();
                swf.Start();
                var result = await ActivityManager.FetchRegionTiresActivity(new FlashSaleTiresActivityRequest()
                {
                    ActivityId = activityId,
                    RegionId = cityId,
                    TireSize = rim
                });
                swf.Stop();
                Logger.Info($"调用FetchRegionTiresActivity接口总耗时=》{swf.ElapsedMilliseconds}");
                productPool.StartDateTime = result.StartTime;
                productPool.EndDateTime = result.EndTime;
                productPool.Products = new List<FlashSaleProductModel>();
                var listproducts = new List<FlashSaleProductModel>();
                foreach (var floorActivity in result.TiresFloorActivity)
                {
                    var single = floorActivity.OtherImgAndProductMap
                                     .FirstOrDefault(r => r.IsAdaptaionTireSize) ??
                                 new Models.ImgAndProductMap();
                    var listP = single.ProductDetails?.NoSloganProductInfo.Union(
                                    single.ProductDetails?.SloganProductInfo) ?? new List<Models.TireProductInfo>();
                    listproducts.AddRange(listP.Select(item => new FlashSaleProductModel
                    {
                        ActivityID = floorActivity.TiresActivityId,
                        PID = item.PID,
                        ProductName = item.ProductName,
                        ProductImg = item.ProductImg.GetImageUrl(),
                        Price = item.Price,
                        AdvertiseTitle = item.AdvertiseTitle,
                        TotalQuantity = item.TotalQuantity,
                        SaleOutQuantity = item.SaleOutQuantity,
                    }));
                    productPool.Products = listproducts;
                }
                return productPool;
            }
            catch (Exception e)
            {
                Logger.Error($"调用FetchRegionTiresActivity接口报错", e.InnerException);
                return productPool;
            }
        }

        private ActivityPageInfoCellProductPool ModelConvert(ActivePageContentModel p)
        {
            return new ActivityPageInfoCellProductPool()
            {
                IsAdapter = p.IsAdapter,
                Pid = p.Pid,
                ProductName = p.ProductName,
                ActivityId = p.ActivityId,
                Price = p.Price,
                ActivityPrice = p.ActivityPrice,
                Onsale = p.Onsale,
                Brand = p.Brand,
                Pattern = p.Pattern,
                SpeedRating = p.SpeedRating,
                Category = p.Category,
                IsNewUserFirstOrder = p.IsNewUserFirstOrder,
                AdvertiseTitle = p.AdvertiseTitle,
                IsUsePcode = p.IsUsePcode,
                TotalQuantity = p.TotalQuantity,
                SaleOutQuantity = p.SaleOutQuantity,
                StartDateTime = p.StartDateTime,
                EndDateTime = p.EndDateTime,
                IsShow = p.IsShow,
                DisplayWay = p.DisplayWay,
                Type = p.Type,
                ProductType = p.ProductType,
                TireSize = p.TireSize,
                Image = p.Image
            };
        }

        private async Task<IEnumerable<SkuProductDetailModel>> SelectSkuProductDetailModels(List<string> pids)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                using (var client = new ProductClient())
                {
                    var skuList = await client.SelectSkuProductListByPidsAsync(pids);
                    skuList.ThrowIfException();
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > 50)
                        Logger.Info($"活动页调用产品接口数据耗时==>{sw.ElapsedMilliseconds},接口耗时=》{skuList.ElapsedMilliseconds}");
                    return skuList.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"活动页调用产品接口失败Exception{ex.Message}{ex.InnerException}");
                return new List<SkuProductDetailModel>();
            }


        }

        private static void SetTag(ActivePageContentModel detail, string value,
            Dictionary<string, IEnumerable<string>> tags)
        {
            //var type = detail.GetType();
            //var propertyInfo = type.GetProperty(value);
            //if(propertyInfo!=null)
            //propertyInfo.SetValue(detail, 1, null); 
            if (tags.ContainsKey(value) && tags[value] != null)
            {
                if (tags[value].Any(t => t == detail.Pid))
                {
                    switch (value)
                    {
                        case "GiftProduct":
                            detail.HasGift = 1;
                            break;
                        case "Adapter":
                            detail.AdapterTag = 1;
                            break;
                        case "InstallNoSupport":
                            detail.InstallService = 0;
                            break;
                        case "InstallSupport":
                            detail.InstallService = 2;
                            break;
                        case "InstallFree":
                            detail.InstallService = 1;
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        private async Task<List<ActivePageContentModel>> SelectProductPoolProductsAsCache(
            ActivityPageInfoModuleProductPoolRequest request, DalActivityPageInfoProductPool first)
        {
            var listContents = new List<ActivePageContentModel>();
            var activityId = first?.ActivityId;
            var activityIdType = first?.ActivityType;
            var productPool = new FlashSaleModel();
            if (activityId != null)
            {
                switch (activityIdType)
                {
                    case (int)ActivityIdType.FlashSaleActivity:
                        productPool =
                            await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(activityId.Value);
                        break;
                    case (int)ActivityIdType.FloorActivity when (!string.IsNullOrEmpty(request.CityId)):
                        productPool = await GetFlashSaleModelByFloor(productPool, request.Rim,
                            activityId.Value,
                            Convert.ToInt32(request.CityId));
                        break;
                }
                if (productPool.Products != null)
                {
                    var tips = first.Tips?.Split(',');

                    var tags = new Dictionary<string, IEnumerable<string>>();
                    var installment = new List<TireInstallmentModel>();
                    var pidList = productPool.Products.Select(r => r.PID).ToList();
                    var productDetails = (await SelectSkuProductDetailModels(pidList)).ToList();
                    if (tips != null)
                    {
                        try
                        {
                            var sw1 = new Stopwatch();
                            sw1.Start();
                            using (var client = new ProductClient())
                            {

                                var searchTags = new List<ProductTagTypeIn>();

                                //if (tips.Contains("1"))
                                //    searchTags.Add(ProductTagTypeIn.Adapter);
                                if (tips.Contains("3"))
                                    searchTags.Add(ProductTagTypeIn.GiftProduct);
                                if (tips.Contains("4"))
                                {
                                    searchTags.Add(ProductTagTypeIn.InstallNoSupport);
                                    searchTags.Add(ProductTagTypeIn.InstallSupport);
                                    searchTags.Add(ProductTagTypeIn.InstallFree);
                                }

                                var productTags = await client.SelectProductsTagAsync(new SeachTagRequest()
                                {
                                    PidList = pidList,
                                    SeachTags = searchTags,
                                    VehicleId = request.VehicleId,
                                    Tid = request.Tid != null ? Convert.ToInt32(request.Tid) : 0
                                });
                                productTags.ThrowIfException(true);
                                tags = productTags.Result;
                                sw1.Stop();
                                if (sw1.ElapsedMilliseconds > 50)
                                    Logger.Info(
                                        $"调用标签接口总耗时=》{sw1.ElapsedMilliseconds},接口耗时=》{productTags.ElapsedMilliseconds}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("调用标签接口报错，错误信息：", ex);

                        }

                        if (tips.Contains("2"))
                        {
                            try
                            {
                                using (var client2 = new ProductConfigClient())
                                {
                                    var installments = client2.SelectTireInstallmentByPIDs(pidList);
                                    installments.ThrowIfException();
                                    installment = installments.Result.ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("调用分期购接口报错，错误信息：", ex);

                            }
                        }
                    }
                    foreach (var item in productPool.Products.Where(r => r.IsShow == true))
                    {
                        var product = productDetails.FirstOrDefault(r => r.Pid == item.PID);
                        if (product != null)
                        {
                            var content = new ActivePageContentModel
                            {
                                DisplayWay = first.DisplayWay,
                                ActivityId = item.ActivityID,
                                Pid = item.PID,
                                ProductName = item.ProductName,
                                Type = ConvertType(first.ProductType),
                                ProductType = first.ProductType,
                                IsAdapter = first.IsAdapter,
                                Image = item.ProductImg.GetImageUrl(),
                                ActivityPrice = item.Price,
                                Price = item.FalseOriginalPrice,
                                AdvertiseTitle = item.AdvertiseTitle,
                                TotalQuantity = item.TotalQuantity,
                                SaleOutQuantity = item.SaleOutQuantity,
                                StartDateTime = productPool.StartDateTime,
                                EndDateTime = productPool.EndDateTime,
                                TireSize = product.Size.ToString(),
                                Onsale = product.Onsale,
                                Pattern = product.Pattern,
                                SpeedRating = product.SpeedRating,
                                IsUsePcode = item?.IsUsePCode ?? false,
                                Brand = product.Brand,
                                Category = product.RootCategoryName,
                                Group = first.RowNum,
                                ColumnNumber = first.ColumnNumber
                            };

                            #region 活动商品

                            //  content.ConcatUrl(first.ProductType);

                            #endregion

                            #region 标签

                            SetTag(content, "GiftProduct", tags);
                            SetTag(content, "Adapter", tags);
                            SetTag(content, "InstallNoSupport", tags);
                            SetTag(content, "InstallSupport", tags);
                            SetTag(content, "InstallFree", tags);

                            #endregion

                            #region 分期

                            if (installment != null && installment.Any(r => r.PID == content.Pid))
                            {
                                content.InstalIlmentTag = 1;
                            }

                            #endregion

                            listContents.Add(content);
                        }
                    }

                }
            }
            return listContents;
        }

        #endregion

        public override async Task<List<ActivityPageInfoRowProductPool>> GetActivityPageInfoRowByTypes(
                ActivityPageInfoModuleProductPoolRequest request)
        {
            this.Key = $"ProductPool{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
                {
                    (int) ActivityPageContentType.ProductPool,
                };
            var config = await GetActivityRowConfig<DalActivityPageInfoProductPool>(request.RowNums);
            var contents = new List<ActivePageContentModel>();
            foreach (var item in config)
            {
                using (var client = CacheHelper.CreateCacheClient("Activity"))
                {
                    var key =
                        $"pool{request.HashKey}{item.RowNum}";
                    var cache = await client.GetOrSetAsync(key,
                        () => SelectProductPoolProductsAsCache(request, item),
                        TimeSpan.FromSeconds(30));
                    if (cache.Success)
                    {
                        contents.AddRange(cache.Value);
                    }
                    else
                    {
                        contents.AddRange(await SelectProductPoolProductsAsCache(request, item));
                    }

                }
            }
            return contents.GroupBy(r => new
            {
                r.Group,
                r.ColumnNumber
            }).Select(r => new ActivityPageInfoRowProductPool()
            {
                RowNum = r.Key.Group,
                ColumnNumber = r.Key.ColumnNumber,
                ActivityPageInfoCellProductPools = r.Select(ModelConvert).ToList()
            }).ToList();
        }
    }

    #endregion

    #region 【菜单】
    public class ActivityPageInfoMenuManager :
    ActivityPageInfoBaseManager<ActivityPageInfoModuleBaseRequest, ActivityPageInfoRowMenuModel>
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(ActivityPageInfoMenuManager));
        protected override string Key { get; set; }
        protected override string HashKey { get; set; }
        protected override string Channel { get; set; }
        protected override List<int> Types { get; set; }
        public async Task<List<DalActivityPageMenuModel>> GetActivityPageMenuModels(List<DalActivityPageInfoMenu> request)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var prefix = await GetKeyPrefix();
                var pkids = request.Select(r => r.Pkid);
                var result = await client.GetOrSetAsync(prefix + "menu" + string.Join("/", pkids), () => DalActivityPage.SelectActivePageMenus(pkids?.ToList()), ExpireTime);
                if (result.Success)
                {
                    return (from a in result.Value
                            join b in request on a.FKActiveContentID equals b.Pkid
                            select new DalActivityPageMenuModel()
                            {
                                MenuName = a.MenuName,
                                MenuValue = a.MenuValue,
                                MenuValueEnd = a.MenuValueEnd,
                                Color = a.Color,
                                Description = a.Description,
                                Sort = a.Sort,
                                RowNum = b.RowNum
                            })?.ToList();

                }
                else
                {
                    Logger.Error($"活动页{HashKey}从redis获取菜单信息失败");
                    return null;
                }
            }
        }
        public override async Task<List<ActivityPageInfoRowMenuModel>> GetActivityPageInfoRowByTypes(
                    ActivityPageInfoModuleBaseRequest request)
        {
            this.Key = $"menu{request.HashKey}{request.Channel}";
            this.HashKey = request.HashKey;
            this.Channel = request.Channel;
            this.Types = new List<int>
            {
                (int) ActivityPageContentType.Navigation,
            };
            var config1 = await GetActivityRowConfig<DalActivityPageInfoMenu>(request.RowNums);
            var config = await GetActivityPageMenuModels(config1);
            return config?.GroupBy(r => r.RowNum).Select(r => new ActivityPageInfoRowMenuModel()
            {
                RowNum = r.Key,
                ActivityPageInfoMenus = r.Select(p => new ActivityPageInfoMenu()
                {
                    MenuName = p.MenuName,
                    Color = p.Color,
                    Sort = p.Sort,
                    MenuValue = p.MenuValue,
                    MenuValueEnd = p.MenuValueEnd,
                    Description = p.Description
                }).ToList()
            }).ToList();
        }
    }
    #endregion

    public class ActivityPageInfoManager
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(ActivityManager));
        protected static string ClientName => GlobalConstant.ActivityPageClientName;
        protected static string Prefix => GlobalConstant.ActivityPageInfoPrefix;
        protected static Task<string> GetKeyPrefix() => GlobalConstant.GetKeyPrefix(Prefix, ClientName);
        #region 私有方法
        private static async Task<bool> CheckActivityTimeAsync(ActivityPageInfoModel activePageList, ActivityPageInfoRequest request)
        {
            var flag = true;
            if (request.UserId != Guid.Empty)
            {
                var isExist = await ActivityManager.GetActivityPageWhiteListByUserIdAsync(request.UserId);
                if (isExist)
                {
                    if (activePageList.EndDate < DateTime.Now)
                        flag = false;
                }
                else
                {
                    if (activePageList.StartDate > DateTime.Now || activePageList.EndDate < DateTime.Now)
                        flag = false;
                }
            }
            else
            {
                if (activePageList.StartDate > DateTime.Now || activePageList.EndDate < DateTime.Now)
                    flag = false;
            }
            return flag;
        }
        private ActivityPageInfoTireSizeConfigModel TireSizeConfigTrim(ActivityPageInfoTireSizeConfigModel trimtireSizeConfig)
        {
            string RemoveSpace(string configProp) => configProp?.Trim().Replace("&nbsp;", "");
            trimtireSizeConfig.CarInfoColor = RemoveSpace(trimtireSizeConfig.CarInfoColor);
            trimtireSizeConfig.PromptFontSize = RemoveSpace(trimtireSizeConfig.PromptFontSize);
            trimtireSizeConfig.MarginColor = RemoveSpace(trimtireSizeConfig.MarginColor);
            trimtireSizeConfig.FillColor = RemoveSpace(trimtireSizeConfig.FillColor);
            trimtireSizeConfig.PromptColor = RemoveSpace(trimtireSizeConfig.PromptColor);
            trimtireSizeConfig.CarInfoFontSize = RemoveSpace(trimtireSizeConfig.CarInfoFontSize);
            trimtireSizeConfig.NoCarTypePrompt = RemoveSpace(trimtireSizeConfig.NoCarTypePrompt);
            trimtireSizeConfig.NoFormatPrompt = RemoveSpace(trimtireSizeConfig.NoFormatPrompt);
            trimtireSizeConfig.CarInfoColor = RemoveSpace(trimtireSizeConfig.CarInfoColor);
            return trimtireSizeConfig;
        }
        #region 【缓存hashkey跟主键对应关系】

        private async Task<int> GetActivityIdByHashKeyAsync(string hashKey)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var keyprefix = await CacheManager.CommonGetKeyPrefixAsync(ClientName, Prefix);
                var result = await client.GetOrSetAsync($"activityId{keyprefix}{hashKey}",
                    () => DalActivity.FetchActivePageListModelIdasync(hashKey), TimeSpan.FromDays(1));
                return result.Success ? result.Value : 0;
            }
        }

        #endregion
        private async Task<ActivityPageInfoModel> GetActivityPageConfigCache(ActivityPageInfoRequest request)
        {
            var sw = new Stopwatch();
            sw.Start();
            var activityId = await GetActivityIdByHashKeyAsync(request.HashKey);
            var tireSizeConfig = DalActivityPage.FetchActivityPageTireSizeConfigModelAsync(activityId);
            var activityPage = DalActivityPage.FetchActivityPageModelAsync(activityId);
            var activityPageContents = DalActivityPage.SelectActivityPageContentsAsync(activityId, request.Channel);
            await Task.WhenAll(tireSizeConfig, activityPage, activityPageContents);
            var result = activityPage.Result ?? new ActivityPageInfoModel();
            var pageContents = activityPageContents.Result;
            var configtypes = pageContents.Select(r => r.Type);
            var types = new List<int>();
            if (configtypes.Any(r => r == (int)ActivityPageContentType.NewProductPool))
            {
                types.Add((int)ActivityPageConfigType.NewProductPool);
            }
            if (configtypes.Any(r => r == (int)ActivityPageContentType.ProductPool))
            {
                types.Add((int)ActivityPageConfigType.NewProductPool);
            }
            if (configtypes.Any(r => new List<int>
                {
                    (int)ActivityPageContentType.Tire,
                    (int)ActivityPageContentType.ATire,
                    (int)ActivityPageContentType.LunGu,
                    (int)ActivityPageContentType.ALunGu,
                    (int)ActivityPageContentType.CarProduct,
                    (int)ActivityPageContentType.ACarProduct
                }.Contains(r)))
            {
                types.Add((int)ActivityPageConfigType.Normal);
            }
            await SetActivityPageInfoListTypeAsync(request.HashKey, types);
            result.ActivityPageInfoRows = pageContents.GroupBy(r => new
            {
                r.Group,
                r.RowType
            }).Select(row => new ActivityPageInfoRow()
            {
                RowNum = row.Key.Group,
                RowType = row.Key.RowType,
                ActivityPageInfoCells = row.Select(cell => new ActivityPageInfoCell()
                {
                    CellNum = cell.OrderBy,
                    Type = cell.Type
                }).OrderBy(o => o.CellNum).ToList()
            }).OrderBy(o => o.RowNum).ToList();
            result.ActivityPageInfoTireSizeConfigModel = TireSizeConfigTrim(tireSizeConfig.Result ??
                                                     new ActivityPageInfoTireSizeConfigModel());
            sw.Stop();
            if (sw.ElapsedMilliseconds > 50)
                Logger.Info($"活动页获取配置数据耗时==>{sw.ElapsedMilliseconds}");
            return result;
        }

        private static async Task<List<string>> SearchProductByPidsAsync(SearchProductRequest request)
        {
            var result = new List<string>();
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                using (var client = new ProductSearchClient())
                {
                    var pageModelResult = await client.SearchProductWithPidsAsync(request);
                    var data = pageModelResult.Result.Source.ToList();
                    result.AddRange(data);
                    if (pageModelResult.Result.Pager != null && pageModelResult.Result.Pager.TotalPage > 1)
                    {
                        for (int i = 2; i <= pageModelResult.Result.Pager.TotalPage; i++)
                        {
                            request.CurrentPage += 1;
                            var splitresult = await client.SearchProductWithPidsAsync(request);
                            if (splitresult.Result.Source != null && splitresult.Result.Source.Any())
                            {
                                result.AddRange(splitresult.Result.Source.ToList());
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                }

                sw.Stop();
                if (sw.ElapsedMilliseconds > 30)
                {
                    Logger.Info($"调用接口SearchProductByPidsAsync耗时{sw.ElapsedMilliseconds}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"调用接口SearchProductByPidsAsync报错", ex);
            }
            return result;
        }

        private static async Task<List<string>> GetNoRepeatRecommendPidsAsync(List<string> pids, int currentPage, int pageSize, Guid userId)
        {
            var listPids = new List<string>();
            try
            {
                using (var client = new ProductSearchClient())
                {
                    var esResult = await client.GetRecommendEsCacheAsync(new RecommendEsSearchRequest()
                    {
                        UserId = userId,
                        PageSize = pageSize,
                        CurrentPage = currentPage

                    });
                    var esPids = esResult.Result.Source?.ToList() ?? new List<string>();
                    listPids = esPids.Except(pids).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error("调用获取bi推荐产品接口失败GetRecommendEsCacheAsync", e);
            }
            return listPids;
        }
        #region 【设置活动页类型】
        private async Task<bool> SetActivityPageInfoListTypeAsync(string hashKey, List<int> types)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var keyprefix = await GetKeyPrefix();
                var result = await client.SetAsync($"actTypes{keyprefix}{hashKey}", types, TimeSpan.FromDays(1));
                return result.Success;
            }
        }
        #endregion
        #region 【获取活动页类型】
        private async Task<List<int>> GetActivityPageInfoListTypeAsync(string hashKey)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var keyprefix = await GetKeyPrefix();
                var result = await client.GetAsync<List<int>>($"actTypes{keyprefix}{hashKey}");
                if (result.Success)
                {
                    return result.Value;
                }
                else
                {
                    Logger.Warn($"从redis缓存中读取{hashKey}下类型出错");
                    return new List<int>();
                }
            }
        }
        #endregion
        #endregion

        #region 【推荐】

        #region 车品推荐
        public async Task<List<ActivityPageInfoRecommend>> GetActivityPageInfoCpRecommendsAsync(
                ActivityPageInfoModuleRecommendRequest request)
        {
            var configTypes = await GetActivityPageInfoListTypeAsync(request.HashKey);
            var allConfigPids = new List<string>();
            foreach (var type in configTypes)
            {
                switch (type)
                {
                    case (int)ActivityPageConfigType.Normal:
                        var product =
                        await new ActivityPageInfoProductManager().GetActivityPageInfoRowByTypes(request);
                        allConfigPids.AddRange(product.SelectMany(r => r.ActivityPageInfoCellProducts.Select(p => p.Pid)));
                        break;
                    case (int)ActivityPageConfigType.NewProductPool:
                        var newpool =
                            await new ActivityPageInfoNewProductPoolManager().GetActivityPageInfoRowByTypes(request);
                        allConfigPids.AddRange(newpool.SelectMany(r => r.ActivityPageInfoCellNewProductPools.Select(p => p.Pid)));
                        break;
                    case (int)ActivityPageConfigType.ProductPool:
                        var pool =
                            await new ActivityPageInfoProductPoolManager().GetActivityPageInfoRowByTypes(request);
                        allConfigPids.AddRange(pool.SelectMany(r => r.ActivityPageInfoCellProductPools.Select(p => p.Pid)));
                        break;
                    default:
                        break;
                }
            }
            var listNoRepeatePids = new List<string>();
            var counter = 0;
            while (listNoRepeatePids.Count < request.RecommendCount && counter <= 10)
            {
                counter++;
                var noRepeatePids = await GetNoRepeatRecommendPidsAsync(allConfigPids, counter, 50, request.UserId);
                listNoRepeatePids.AddRange(noRepeatePids);
            }
            var returnPids = listNoRepeatePids.Take(request.RecommendCount).ToList();
            return returnPids.Select(pid => new ActivityPageInfoRecommend()
            {
                Pid = pid,
            }).ToList();
        }
        #endregion

        #region 轮胎推荐
        internal async Task<List<ActivityPageInfoRecommend>> GetActivityPageInfoTireRecommendsAsync(
              ActivityPageInfoModuleRecommendRequest request)
        {
            int? cityid = null;
            if (string.IsNullOrEmpty(request.CityId))
            {
                cityid = null;
            }
            else
            {
                if (int.TryParse(request.CityId, out int cid))
                {
                    cityid = cid;
                }
                else
                {
                    Logger.Error($"活动页传参城市id不合法{request.CityId}");
                }
            }
            var count = request.RecommendCount;
            var referRequest = new SearchProductRequest()
            {
                CityId = cityid,
                VehicleId = request.VehicleId,
                CurrentPage = 1,
                OrderType = 0,
                PageSize = count,
                Parameters = new Dictionary<string, IEnumerable<string>>()
                {
                    ["CP_Tire_Width"] = new string[] { request.Width },
                    ["CP_Tire_AspectRatio"] = new string[] { request.AspectRatio },
                    ["CP_Tire_Rim"] = new string[] { request.Rim },
                    ["OnSale"] = new string[] { "1" },
                    ["stockout"] = new string[] { "0" },
                    ["Category"] = new string[] { "Tires" },
                    ["StockOutExceptCategory"] = new string[] { "Tires" }
                }
            };
            var result = await SearchProductByPidsAsync(referRequest);
            var returnPids = result.Take(request.RecommendCount).ToList();
            return returnPids.Select(pid => new ActivityPageInfoRecommend()
            {
                Pid = pid,
            }).ToList();
        }
        #endregion

        #endregion

        #region 【会场】
        public async Task<List<ActivityPageInfoHomeModel>> GetActivityPageInfoHomeModelsAsync(string hashKey)
        {
            var activityId = await GetActivityIdByHashKeyAsync(hashKey);
            List<ActivePageHomeWithDetailModel> homewithDetails;
            var keyprefix = await GetKeyPrefix();
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var result = await client.GetOrSetAsync("activityPageHome" + keyprefix + hashKey, () => DalActivity.SelectActivePageHomeWithDetailModels(activityId), TimeSpan.FromDays(1));
                if (result.Success)
                {
                    homewithDetails = result.Value.ToList();
                }
                else
                {
                    homewithDetails = (await DalActivity.SelectActivePageHomeWithDetailModels(activityId)).ToList();
                }
            }
            return homewithDetails.GroupBy(
    r =>
         new
         {
             r.FkActiveId,
             r.BigHomeName,
             r.HidBigHomePic,
             r.BigHomeUrl,
             r.HidBigHomePicWww,
             r.BigHomeUrlWww,
             r.HidBigHomePicWxApp,
             r.BigHomeUrlWxApp,
             r.IsHome,
             r.Sort
         }).
                OrderBy(g => g.Key.Sort).
                Select(a => new ActivityPageInfoHomeModel
                {
                    FkActiveId = a.Key.FkActiveId,
                    HidBigHomePic = a.Key.HidBigHomePic,
                    BigHomeUrl = a.Key.BigHomeUrl,
                    BigHomeUrlWww = a.Key.BigHomeUrlWww,
                    BigHomeUrlWxApp = a.Key.BigHomeUrlWxApp,
                    IsHome = a.Key.IsHome,
                    ActivityPageInfoHomeDeatilModels = a.All(r => r.DetailPkid == 0) ? null : a.Select(detail => new ActivityPageInfoHomeDeatilModel
                    {
                        HomeName = detail.HomeName,
                        HidBigFHomePic = detail.HidBigFHomePic,
                        BigFHomeMobileUrl = detail.BigFHomeMobileUrl,
                        BigFHomeWwwUrl = detail.BigFHomeWwwUrl,
                        BigFHomeOrder = detail.BigFHomeOrder,
                        BigFHomeWxAppUrl = detail.BigFHomeWxAppUrl,
                        Pkid = detail.DetailPkid
                    })?.ToList()
                }).ToList();
        }
        #endregion

        #region 【活动配置】
        public async Task<ActivityPageInfoModel> GetActivityPageInfoConfigModelAsync(ActivityPageInfoRequest request)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var keyprefix = await GetKeyPrefix();
                ActivityPageInfoModel result = null;
                var cacehResult = await client.GetOrSetAsync(keyprefix + GlobalConstant.ActivityPagePrefix + request.HashKey + request.Channel
                    , () => GetActivityPageConfigCache(request), TimeSpan.FromDays(1));
                if (cacehResult.Success)
                {
                    if (await CheckActivityTimeAsync(cacehResult.Value, request))
                    {
                        result = cacehResult.Value;
                    }
                }
                else
                {
                    Logger.Error($"活动页获取redis缓存失败，活动id{request.HashKey}");
                }
                return result;
            }
        }
        #endregion
    }
}



