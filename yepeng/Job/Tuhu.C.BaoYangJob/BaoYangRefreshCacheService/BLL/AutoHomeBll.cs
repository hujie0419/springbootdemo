using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using BaoYangRefreshCacheService.DAL;
using BaoYangRefreshCacheService.Model;
using BaoYangRefreshCacheService.Model.Config;
using BaoYangRefreshCacheService.Model.Option;
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.BLL
{
    public class AutoHomeBll
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AutoHomeBll));
        private static readonly Random Random = new Random();

        private static int WaitTime => Random.Next(1000, 3000);

        public static async Task GetCarModelGrade()
        {
            Logger.Info("获取车系数据开始");
            var instance = new AutoHomeBll();
            var BatchNo = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            var result = await instance.GetAllCarModelGrade();
            result.ForEach(x => x.BatchNo = BatchNo);
            await AutoHomeDal.BatchInsertCarModelGrade(result);
            Logger.Info("获取车系数据结束");
        }

        public static async Task GetCarModelData()
        {
            Logger.Info("获取车型信息数据开始");
            var instance = new AutoHomeBll();
            await instance.CompleteAllRequest();
            Logger.Info("获取车型信息数据结束");
        }

        private static async Task<Tuple<T, bool>> GetResultIfFailedRetryAsync<T>(Func<Task<T>> func, int retryCount = 3)
        {
            var success = false;
            var result = default(T);
            while (!success && retryCount > 0)
            {
                try
                {
                    result = await func();
                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.Info(ex.Message);
                    retryCount--;
                }
            }
            return Tuple.Create(result, success);
        }

        private static async Task<bool> GetResultIfFailedRetryAsync(Func<Task> action, int retryCount = 3)
        {
            var success = false;
            while (!success && retryCount > 0)
            {
                try
                {
                    await action();
                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.Info(ex.Message);
                    retryCount--;
                }
            }
            return success;
        }

        private Dictionary<string, string> GetStyleRules(string styleScript)
        {
            var regMatchVar = new Regex(@"var\s+?(?<name>\w+)\s*=\s*'(?<value>.)';");
            var regMatchFunc = new Regex(@"\w+\((?<value>.+?)\)");

            var regUrlEncode = new Regex(@"\w+\s*=\s*\w+\(\)\[.+?\]\((?<value>.+?)\);");
            var regInfo = new Regex(@"\w+\s*=\s*\w+\(\(([\w\s+'()]+?)\),(.+?)\);");

            var replaceDict = regMatchVar.Matches(styleScript).Cast<Match>().ToLookup(
                match => match.Groups["name"].Value,
                match => match.Groups["value"].Value).ToDictionary(x => x.Key, x => x.LastOrDefault());

            Func<string, string> convertFunc = input =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    return string.Empty;
                }
                input = input.Trim();
                var match = regMatchFunc.Match(input);
                if (match.Success) input = match.Groups["value"].Value.Trim();
                string result = null;
                if (input.Contains("\"") || input.Contains("'"))
                {
                    result = input.Replace("'", "").Replace("\"", "");
                }
                else
                {
                    replaceDict.TryGetValue(input, out result);
                }
                return result;
            };

            var urlEncodeList = regUrlEncode.Match(styleScript).Groups["value"].Value.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x)).Select(convertFunc).ToList();
            var zhs = WebUtility.UrlDecode(string.Join("", urlEncodeList));
            var indexMatch = regInfo.Match(styleScript);
            var indexList = indexMatch.Groups[1].Value.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(convertFunc);

            var indexText = string.Join("", indexList);
            var separator = convertFunc(indexMatch.Groups[2].Value);

            var seq = 0;
            var rules = indexText.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(new[] { ',' }).Select(indexTxt =>
                {
                    int index;
                    int.TryParse(indexTxt, out index);
                    return index;
                })).ToDictionary(x => $"hs_kw{seq++}_configpl", x => string.Join("", x.Select(index => zhs[index])));
            return rules;
        }



        /// <summary>
        /// 获取所有CarModelGrade
        /// </summary>
        /// <returns></returns>
        private async Task<List<AutoHomeCarModelGrade>> GetAllCarModelGrade()
        {
            var result = new List<AutoHomeCarModelGrade>();
            var chs = new List<char>();
            for (var ch = 'a'; ch <= 'z'; ch++)
            {
                var retruenResult = await GetResultIfFailedRetryAsync(() => GetCarModelGrade(ch));
                if (retruenResult.Item2)
                    result.AddRange(retruenResult.Item1);
                else
                    chs.Add(ch);
            }
            if (chs.Any())
            {
                Logger.Info($"【{string.Join(", ", chs)}】获取以上页面的车系数据失败");
            }
            return result;
        }

        /// <summary>
        /// 根据字母获取CarModelGrade
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private async Task<List<AutoHomeCarModelGrade>> GetCarModelGrade(char ch)
        {
            var regex = new Regex(@"^http://www.autohome.com.cn/(?<Id>\d+)/");
            var doc = await HttpManager.Create($"http://www.autohome.com.cn/grade/carhtml/{ch}.html").GetResult(o => o);
            var dlEnumerable = doc.QuerySelectorAll("dl").ToList().Select(dl => new
            {
                DT = dl.Children.FirstOrDefault(child => string.Equals(child.TagName, "dt", StringComparison.OrdinalIgnoreCase)),
                DD = dl.Children.FirstOrDefault(child => string.Equals(child.TagName, "dd", StringComparison.OrdinalIgnoreCase)),
            }).Where(x => x.DT != null && x.DD != null);

            var temp = dlEnumerable.Select(dl => new
            {
                Key = dl.DT.QuerySelector("div").QuerySelector("a").InnerHtml,
                Value = dl.DD.Children.Where(child => child.ClassName == "h3-tit").Select(element => new
                {
                    Key = element.InnerHtml,
                    Value = element.NextElementSibling?.QuerySelectorAll("li")
                            ?.Select(li => li.QuerySelector("h4")?.QuerySelector("a"))
                            ?.Where(a => a != null)
                            ?.Select(a => new { Href = a.GetAttribute("href"), Text = a.InnerHtml })
                            ?.Select(o => new { o.Href, o.Text, Id = regex.Match(o.Href).Groups["Id"].Value })
                })
            });

            var result = temp.SelectMany(
                x => x.Value.SelectMany(
                    y => y.Value.Select(
                        z => new AutoHomeCarModelGrade
                        {
                            Brand = x.Key,
                            GradeId = int.Parse(z.Id),
                            CarSeries = y.Key.Replace("-", ""),
                            CarGrade = z.Text,
                            Url = z.Href,
                        })
                    )
                ).ToList();

            return result;
        }

        private async Task CompleteAllRequest()
        {
            List<AutoHomeCarModelGrade> result = new List<AutoHomeCarModelGrade>();
            const int pageSize = 500;
            for (var pageIndex = 1; ; pageIndex++)
            {
                var items = await AutoHomeDal.SelectCarModelGrades(pageIndex, pageSize);
                if (items == null || !items.Any())
                {
                    break;
                }
                result.AddRange(items);
            }
            foreach (var item in result)
            {
                var success = await GetResultIfFailedRetryAsync(() => CompleteRequest(item));
                if (!success)
                {
                    Logger.Info($"【{item.Brand}-{item.CarSeries}-{item.CarGrade}】获取车型数据失败");
                }
            }
        }

        /// <summary>
        /// 根据CarModelGrade获取车型数据
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        private async Task CompleteRequest(AutoHomeCarModelGrade grade)
        {
            if (await AutoHomeDal.UpdateCarModelGradeStartTime(grade.PKID))
            {
                var carModelArgs = new List<AutoHomeCarModelParam>();
                carModelArgs.AddRange(await GetOnSaleCarModelArgs(grade.GradeId));
                carModelArgs.AddRange(await GetStopSaleCarModelArgs(grade.GradeId));
                carModelArgs.ForEach(x => x.BatchNo = grade.BatchNo);
                var infoArgs = carModelArgs.Where(x => x.ParamName == "车型名称");
                var tiresArgs = carModelArgs.Where(x => x.ParamName.Contains("轮胎"));
                var cmInfos = infoArgs.Select(x => new AutoHomeCarModelInfo
                {
                    CarModelId = x.CarModelId,
                    CarModelName = x.ParamValue,
                    GradeId = grade.GradeId,
                    Url = $"http://www.autohome.com.cn/spec/{x.CarModelId}",
                    BatchNo = x.BatchNo,
                }).ToList();
                await Task.WhenAll(AutoHomeDal.BatchInsertCarModelParam(tiresArgs), AutoHomeDal.BatchInsertCarModelInfo(cmInfos));
                await AutoHomeDal.UpdateCarModelGradeEndTime(grade.PKID);
            }
        }

        /// <summary>
        /// 获取在售的车型参数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<AutoHomeCarModelParam>> GetOnSaleCarModelArgs(int id)
            => await GetCarModelArgs($"http://car.autohome.com.cn/config/series/{id}.html");

        private async Task<List<AutoHomeCarModelParam>> GetCarModelArgs(string url)
        {
            var getArgsTask = HttpManager.Create(url).GetResult(doc => GetCarModelArguments(doc));

            await Task.WhenAll(Task.Delay(WaitTime), getArgsTask);

            return await getArgsTask;
        }

        /// <summary>
        /// 获取停售的车型参数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<AutoHomeCarModelParam>> GetStopSaleCarModelArgs(int id)
        {
            Func<IHtmlDocument, List<string>> convertFunc =
                doc => doc.QuerySelectorAll(".car_detail")
                .Select(
                    x => x.QuerySelector(".header")
                    .QuerySelector(".models_nav")
                    .Children
                    .FirstOrDefault(element => element.InnerHtml == "参数配置")
                    .GetAttribute("href")
                    )
                .Where(x => !string.IsNullOrEmpty(x)).ToList();

            var urlsTask = HttpManager.Create($"http://www.autohome.com.cn/{id}/sale.html").GetResult(convertFunc);
            await Task.WhenAll(Task.Delay(WaitTime), urlsTask);
            var urls = await urlsTask;

            var result = new List<AutoHomeCarModelParam>();
            foreach (var url in urls)
            {
                var vehicles = await GetCarModelArgs($"http://www.autohome.com.cn/{url}");
                result.AddRange(vehicles);
            };
            return result;
        }

        /// <summary>
        /// 获取车型参数
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public List<AutoHomeCarModelParam> GetCarModelArguments(IHtmlDocument doc)
        {
            var regex = new Regex(@"<span class='(.+?)'>(.*?)</span>");
            var regConfig = new Regex(@"var\s*config\s*?=\s*?({(?:(?<open>{)|(?<-open>})|[^{}])+(?(open)(?!))})", RegexOptions.Singleline);
            var regOption = new Regex(@"var\s*option\s*?=\s*?({(?:(?<open>{)|(?<-open>})|[^{}])+(?(open)(?!))})", RegexOptions.Singleline);
            var scripts = doc.Scripts.Select(s => s.InnerHtml).ToList();
            var jsonScript = scripts.FirstOrDefault(s => s.Contains("var config") && s.Contains("var option"));
            var styleScript = scripts.FirstOrDefault(s => s.Contains("::before"));
            var rules = styleScript == null ? new Dictionary<string, string>() : GetStyleRules(styleScript);
            List<AutoHomeCarModelParam> result = null;
            if (jsonScript != null)
            {
                var optionTxt = regOption.Match(jsonScript).Groups[1].Value;
                var configTxt = regConfig.Match(jsonScript).Groups[1].Value;

                var option = JsonConvert.DeserializeObject<Option>(optionTxt);
                var config = JsonConvert.DeserializeObject<Config>(configTxt);

                Func<string, string> convertFunc = input =>
                {
                    var returnResult = input;
                    var match = regex.Match(returnResult);
                    while (match.Success)
                    {
                        var sb = new StringBuilder();
                        var className = match.Groups[1].Value;
                        var innerText = match.Groups[2].Value;
                        string beforeValue = null;
                        rules.TryGetValue(className, out beforeValue);

                        var begin = returnResult.Substring(0, match.Index);
                        var end = returnResult.Substring(match.Index + match.Length);
                        returnResult = $"{begin}{(beforeValue ?? string.Empty)}{innerText}{end}";
                        match = regex.Match(returnResult);
                    }
                    return returnResult;
                };

                var configResult = config.Result.ParamTypeItems.SelectMany(
                    paramTypeItem => paramTypeItem.ParamItems.SelectMany(
                        paramItem =>
                        {
                            var name = convertFunc(paramItem.Name);
                            return paramItem.ValueItems.Select(valueItem => new AutoHomeCarModelParam
                            {
                                CarModelId = valueItem.SpecId,
                                ParamValue = convertFunc(valueItem.Value),
                                ParamName = name
                            });
                        })
                    );

                var optionResult = option.Result.ConfigTypeItems.SelectMany(
                    configTypeItem => configTypeItem.ConfigItems.SelectMany(
                        configItem =>
                        {
                            var name = convertFunc(configItem.Name);
                            return configItem.ValueItems.Select(valueItem => new AutoHomeCarModelParam
                            {
                                CarModelId = valueItem.SpecId,
                                ParamValue = convertFunc(valueItem.Value),
                                ParamName = name
                            });
                        })
                    );

                result = (configResult ?? new List<AutoHomeCarModelParam>()).Concat(optionResult ?? new List<AutoHomeCarModelParam>()).ToList();
            }
            return result ?? new List<AutoHomeCarModelParam>();
        }

    }
}
