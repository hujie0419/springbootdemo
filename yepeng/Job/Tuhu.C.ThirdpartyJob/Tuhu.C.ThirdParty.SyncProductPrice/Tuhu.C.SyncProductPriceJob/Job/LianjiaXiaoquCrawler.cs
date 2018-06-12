using AngleSharp.Parser.Html;
using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.DataAccess;
using Tuhu.C.SyncProductPriceJob.Helpers;
using Tuhu.C.SyncProductPriceJob.Models;
using Tuhu.Nosql;
using Tuhu.Service.Utility;

namespace Tuhu.C.SyncProductPriceJob.Job
{
    public class LianjiaXiaoquCrawler : BaseQuartzJob
    {
        private static readonly HtmlParser HtmlParser = new HtmlParser();

        public LianjiaXiaoquCrawler() : base(LogManager.GetLogger<LianjiaXiaoquCrawler>())
        {
        }

        protected override async Task ExecuteAsync(IJobExecutionContext context)
        {
            var proxyClient = new HttpProxyClient();
            try
            {
                using (var cacheClient = CacheHelper.CreateCacheClient())
                {
                    IReadOnlyCollection<LianjiaCityModel> cities;
                    var cacheResult = await cacheClient.GetAsync<LianjiaCityModel[]>("lianjia_city_info");
                    if (cacheResult.Success)
                    {
                        cities = cacheResult.Value;
                    }
                    else
                    {
                        Logger.Warn($"获取链家城市缓存失败，ErrorMsg:{cacheResult.Message}");
                        cities = await GetLianjiaCityListAsync(proxyClient);
                        var setResult = await cacheClient.SetAsync("lianjia_city_info", cities, TimeSpan.FromDays(7));
                        if (setResult.Success)
                        {
                            Logger.Info("lianjia_city_info 缓存设置成功");
                        }
                        else
                        {
                            Logger.Error($"lianjia_city_info 缓存设置失败，ErrorMsg:{setResult.Message}");
                        }
                    }
#if DEBUG
                    foreach (var city in cities)
                    {
                        await SyncDistrictsInfo(city, proxyClient);
                    }
#else
                    await Task.WhenAll(cities.Select(city => SyncDistrictsInfo(city, proxyClient)));
#endif
                }
            }
            catch (Exception ex)
            {
                Logger.Error("链家爬虫运行出错", ex);
            }
            finally
            {
                proxyClient.Dispose();
            }
        }

        private async Task<IReadOnlyList<LianjiaCityModel>> GetLianjiaCityListAsync(HttpProxyClient proxyClient)
        {
            var result = await RetryHelper.TryInvokeAsync(() => proxyClient.GetAsync("https://bj.lianjia.com/"), _ => _.Success);
            if (result.Success)
            {
                var doc = await HtmlParser.ParseAsync(Encoding.UTF8.GetString(result.Result));
                var cities = doc.QuerySelectorAll(".fc-main li a").Select(_ => new LianjiaCityModel
                {
                    Name = _.TextContent.Trim(),
                    LinkUrl = _.GetAttribute("href").EndsWith("/") ? _.GetAttribute("href") : $"{_.GetAttribute("href")}/"
                }).ToArray();
                foreach (var city in cities)
                {
                    var response = RetryHelper.TryInvoke(() => proxyClient.Get(city.LinkUrl), _ => _.Success);
                    if (response.Success)
                    {
                        if (Encoding.UTF8.GetString(result.Result).Contains("xiaoqu"))
                        {
                            city.HasXiaoqu = true;
                            //await SyncDistrictsInfo(city, proxyClient);
                        }
                        else
                        {
                            Logger.Info($"链家 {city.Name}不存在小区，url:{city.LinkUrl}");
                        }
                    }
                    else
                    {
                        Logger.Warn($"判断链家{city.Name}是否有小区出错，LinkUrl:{city.LinkUrl}");
                    }
                }
                return cities.Where(_ => _.HasXiaoqu).ToArray();
            }
            Logger.Error($"获取链家城市错误,ErrorCode:{result.ErrorCode},ErrorMsg:{result.ErrorMessage}", result.Exception);
            return Enumerable.Empty<LianjiaCityModel>().ToArray();
        }

        private async Task SyncXiaoquInfo(LianjiaDistrictModel district, HttpProxyClient proxyClient)
        {
            foreach (var xiaoqu in district.XiaoquList)
            {
                try
                {
                    var response = await RetryHelper.TryInvokeAsync(() => proxyClient.GetAsync(xiaoqu.LinkUrl), _ => _.Success);
                    if (response.Success)
                    {
                        var doc = await HtmlParser.ParseAsync(Encoding.UTF8.GetString(response.Result));

                        if (string.IsNullOrEmpty(doc.QuerySelector(".detailTitle")?.TextContent.Trim()))
                        {
                            Logger.Warn($"爬取链家小区详细信息失败，XiaoquInfo:{JsonConvert.SerializeObject(xiaoqu)}");
                            continue;
                        }
                        xiaoqu.Address = doc.QuerySelector(".detailDesc").TextContent.Trim();

                        xiaoqu.Price = decimal.TryParse(doc
                            .QuerySelector(".xiaoquUnitPrice")?.TextContent.Trim(), out var price)
                            ? price
                            : 0;
                        xiaoqu.Age = doc.QuerySelector(".xiaoquInfoItem:nth-child(1) > .xiaoquInfoContent")?.TextContent.Trim();
                        xiaoqu.BuildingType = doc.QuerySelector(".xiaoquInfoItem:nth-child(2) > .xiaoquInfoContent")?.TextContent.Trim();
                        xiaoqu.WuyeFee = doc.QuerySelector(".xiaoquInfoItem:nth-child(3) > .xiaoquInfoContent")?.TextContent.Trim();
                        xiaoqu.WuyeCompany = doc.QuerySelector(".xiaoquInfoItem:nth-child(4) > .xiaoquInfoContent")?.TextContent.Trim();
                        xiaoqu.Developer = doc.QuerySelector(".xiaoquInfoItem:nth-child(5) > .xiaoquInfoContent")?.TextContent.Trim();
                        xiaoqu.BuildingNum = doc.QuerySelector(".xiaoquInfoItem:nth-child(6) > .xiaoquInfoContent")?.TextContent.Trim();
                        xiaoqu.HouseNum = doc.QuerySelector(".xiaoquInfoItem:nth-child(7) > .xiaoquInfoContent")?.TextContent.Trim();

                        var locationStr = doc.QuerySelector(".xiaoquInfoItem:nth-child(8) > .xiaoquInfoContent > span")
                            ?.GetAttribute("xiaoqu")?.Trim('[', ']', ' ');
                        if (string.IsNullOrWhiteSpace(locationStr))
                        {
                            var sourceText = doc.Source.Text;
                            try
                            {
                                locationStr = sourceText.Substring(sourceText.IndexOf("resblockPosition") + 18,
                                    sourceText.IndexOf(@"resblockName") - sourceText.IndexOf("resblockPosition") - 25);
                            }
                            catch
                            {
                                // ignore
                                locationStr = null;
                            }
                        }
                        var location = locationStr?.Split(',');
                        if (location != null && location.Length == 2)
                        {
                            xiaoqu.Longtitude = location[0];
                            xiaoqu.Latitude = location[1];
                        }

                        await LianjiaXiaoqu.SaveXiaoquInfoAsync(xiaoqu);
                    }
                    else
                    {
                        Logger.Error($"{xiaoqu.City}-{xiaoqu.District}-{xiaoqu.Name}--详情获取失败，url:{xiaoqu.LinkUrl}");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"爬取链家小区详细信息失败，XiaoquInfo:{JsonConvert.SerializeObject(xiaoqu)}", e);
                }
            }
        }

        private async Task SyncDistrictsInfo(LianjiaCityModel city, HttpProxyClient proxyClient)
        {
            var xiaoquLink = $"{city.LinkUrl}xiaoqu/";
            try
            {
                var response = await RetryHelper.TryInvokeAsync(() => proxyClient.GetAsync(xiaoquLink), _ => _.Success);
                if (response.Success)
                {
                    var doc = await HtmlParser.ParseAsync(Encoding.UTF8.GetString(response.Result));
                    city.Districts = doc.QuerySelectorAll(".position dl:nth-child(2) a").Select(_ => new LianjiaDistrictModel
                    {
                        City = city.Name,
                        LinkUrl = _.GetAttribute("href").StartsWith("http") ? _.GetAttribute("href") : $"{city.LinkUrl}{_.GetAttribute("href").Substring(1)}",
                        Name = _.TextContent.Trim()
                    }).ToArray();
                    //
                    foreach (var district in city.Districts)
                    {
                        var pageIndex = 1;
                        while (true)
                        {
                            var linkUrl = $"{district.LinkUrl}pg{pageIndex}/";
                            var response1 = RetryHelper.TryInvoke(() => proxyClient.Get(linkUrl), _ => _.Success);
                            if (response1.Success)
                            {
                                Logger.Info($"{district.City}-{district.Name}-获取pageIndex:{pageIndex} 小区成功，RequestUrl:{linkUrl}");

                                var doc1 = HtmlParser.Parse(Encoding.UTF8.GetString(response1.Result));

                                district.XiaoquList.AddRange(doc1.QuerySelectorAll(".listContent > .xiaoquListItem").Select(_ => new LianjiaXiaoquModel
                                {
                                    XiaoquId = long.TryParse(_.GetAttribute("data-id") ?? _.GetAttribute("data-housecode"), out var id) ? id : 0,
                                    LinkUrl = _.QuerySelector("a.img ").GetAttribute("href") ?? _.QuerySelector(".title>a").GetAttribute("href"),
                                    Name = _.QuerySelector(".info>.title>a").TextContent.Trim(),
                                    Price = decimal.TryParse(_.QuerySelector(".totalPrice>span")?.TextContent.Trim(), out var price) ? price : 0,
                                    Remark = _.QuerySelector(".district")?.TextContent.Trim(),
                                    Remark1 = _.QuerySelector(".bizcircle")?.TextContent.Trim(),
                                    City = city.Name,
                                    District = district.Name
                                }));
                                var totalCount = int.TryParse(doc1.QuerySelector(".total > span")?.TextContent.Trim(), out var tt) ? tt : 0;

                                var pageData = JsonConvert.DeserializeObject<JObject>(doc1.QuerySelector(".house-lst-page-box").GetAttribute("page-data"));

                                if (pageData["totalPage"].Value<int>() > pageIndex ||
                                    totalCount / 30 > pageIndex)
                                {
                                    pageIndex++;
                                }
                                else
                                {
                                    break;
                                }
#if DEBUG
                                break;
#endif
                            }
                            else
                            {
                                Logger.Error($"{district.City}-{district.Name}-获取pageIndex:{pageIndex}小区失败，RequestUrl:{linkUrl}，ErrorMsg:{response1.ErrorMessage}");
                                break;
                            }
                        }

                        // 移除已存在的小区
                        var existsXiaoquIds = LianjiaXiaoqu.SelectXiaoquId(district.City, district.Name);
                        district.XiaoquList.RemoveAll(_ => existsXiaoquIds.Any(x => x == _.XiaoquId));
                        // 同步小区数据
                        await SyncXiaoquInfo(district, proxyClient);
                    }
                }
                else
                {
                    Logger.Error($"获取{city.Name}-区域信息失败，RequestUrl:{xiaoquLink}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"获取城市【{city.Name}】区域信息以及小区列表失败,CityLink:{city.LinkUrl},RequestUrl:{xiaoquLink}", ex);
            }
        }
    }
}
