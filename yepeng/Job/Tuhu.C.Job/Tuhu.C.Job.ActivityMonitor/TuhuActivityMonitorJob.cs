using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Tuhu.Service.Config;
using Tuhu.C.Job.ActivityMonitor.BLL;
using Tuhu.C.Job.ActivityMonitor.DAL;
using Tuhu.C.Job.ActivityMonitor.Model;

namespace Tuhu.C.Job.ActivityMonitor
{
    public class TuhuActivityMonitorJob : IJob
    {

        public static readonly ILog Logger = LogManager.GetLogger(typeof(TuhuActivityMonitorJob));
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("TuhuActivityMonitor");
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                bool result = ActivityMonitor();
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                if (result)
                    Logger.Info($"ActivityMonitor执行成功");
                else
                {
                    Logger.Info($"ActivityMonitor执行失败");
                }
                Logger.Info(string.Format("耗时：{0}s", ts.TotalSeconds));
            }
            catch (Exception ex)
            {
                Logger.Error($"ActivityMonitor异常：{ex.ToString()}");
            }
        }

        public List<OverdueActivityConfig> GetActivityConfigList(string pageId, string type = "H5")
        {
            List<OverdueActivityConfig> activityConfigList = new List<OverdueActivityConfig>();
            DateTime nowday = DateTime.Now.Date;
            string searchkey = "activity/index?id=";
            string regexstr = @"(.*)activity/index\?id=(\w*)";
            if (type.Equals("lightApp", StringComparison.OrdinalIgnoreCase))
            {
                searchkey = "luck/luck2?id=";
                regexstr = @"(.*)luck/luck2\?id=(\w*)";
            }
            using (var homepageclient = new HomePageClient())
            {
                var response = homepageclient.GetLightAppHomePageModuleInfoByModuleKey(pageId);
                if (response != null && response.Result != null)
                {
                    //   var homemodels = response.Result.HomePageModels.Where(p => p.Contents.Where(q => q.Uri.IndexOf("activity/index?id=") >= 0).Count()>0).ToList();
                    foreach (var homeModel in response.Result.HomePageModels.Where(p => p != null))
                    {
                        var homepageName = homeModel.Title;
                        var typeName = homeModel.TypeName;
                        foreach (var content in homeModel.Contents.Where(p => p != null))
                        {
                            if (content.Uri?.IndexOf(searchkey) >= 0)
                            {
                                Regex idreg = new Regex(regexstr);
                                Match idMatch = idreg.Match(content.Uri);
                                if (idMatch.Success)
                                {
                                    var hashkey = idMatch.Result("$2");
                                    List<ActivePage> promotionItemList = ModuleContentConfigDAL.GetActivePageByHashKey(hashkey);
                                    if (promotionItemList != null && promotionItemList.FirstOrDefault() != null)
                                    {
                                        var promotionItem = promotionItemList.First();
                                        var endDate = promotionItem.EndDate.Date;
                                        TimeSpan sp = endDate - nowday;
                                        int status = 0;
                                        if (sp.Days <= 4 && sp.Days > 0)
                                        {
                                            status = 1;
                                        }
                                        else if (sp.Days <= 0)
                                        {
                                            status = -1;
                                        }
                                        if (status != 0)
                                        {
                                            var overdueActivityConfigItem = new OverdueActivityConfig()
                                            {
                                                ModuleName = homepageName,
                                                //  HomePageName = promotionModuleItem.HomePageName,
                                                HelperModuleName = content.Title,
                                                Url = "https://wx.tuhu.cn/staticpage/promotion/activity/?id=" + hashkey,
                                                Title = promotionItem.Title,
                                                EndDate = promotionItem.EndDate,
                                                Status = status,
                                                Type = typeName

                                            };

                                            activityConfigList.Add(overdueActivityConfigItem);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return activityConfigList;
        }


        public string MonitorH5Activity()
        {
            string htmlStr = string.Empty;
            List<OverdueActivityConfig> activityConfigs = GetActivityConfigList("78788223");
            if (activityConfigs.Any())
            {
                htmlStr = GenerateH5andLightAppHtmlString(activityConfigs);
                //  MailService.SendMail("轻应用首页模块活动页到期提醒", ConfigurationManager.AppSettings["ActivityMonitorJob:To"], htmlStr);
            }
            else
            {
                Logger.Info("无过期的轻应用首页活动,故无需邮件发送");
            }
            return htmlStr;
        }

        public string MonitorLightAppActivity()
        {
            string htmlStr = string.Empty;
            List<OverdueActivityConfig> activityConfigs = GetActivityConfigList("62510283", "lightApp");
            if (activityConfigs.Any())
            {
                htmlStr = GenerateH5andLightAppHtmlString(activityConfigs);
                // MailService.SendMail("小程序首页模块活动页到期提醒", ConfigurationManager.AppSettings["ActivityMonitorJob:To"], htmlStr);
            }
            else
            {
                Logger.Info("无过期的小程序首页活动,故无需邮件发送");
            }
            return htmlStr;
        }

        public string MonitorSpecificDaysExpireActivites(int days)
        {
            string htmlStr = string.Empty;
            List<ActivePage> activePages = ModuleContentConfigDAL.GetSpecificDaysExpireNewActivities(days);
            List<OverdueActivityConfig> OverdueNewActiveConfigList = activePages.Where(q => q != null).Select(p => new OverdueActivityConfig()
            {
                Url = "https://wx.tuhu.cn/staticpage/promotion/activity/?id=" + p.HashKey,
                EndDate = p.EndDate,
                Title = p.Title,
                Type = "新活动",
                ExpireDays = (p.EndDate.Date - DateTime.Now.Date).Days
            }).ToList();
            List<ActivityBuild> activitybuilds = ModuleContentConfigDAL.GetSpecificDaysExpireOldActivities(days);
            List<OverdueActivityConfig> OverdueOldActiveConfigList = activitybuilds.Where(q => q != null).Select(p => new OverdueActivityConfig()
            {
                Url = "https://wx.tuhu.cn/staticpage/activity/list.html?id=" + p.id,
                EndDate = p.EndDate,
                Title = p.Title,
                Type = "老活动",
                ExpireDays = (p.EndDate.Date - DateTime.Now.Date).Days
            }).ToList();

            List<OverdueActivityConfig> overdueActivities = new List<OverdueActivityConfig>();
            overdueActivities.AddRange(OverdueNewActiveConfigList.OrderBy(p => p.ExpireDays).ToList());
            overdueActivities.AddRange(OverdueOldActiveConfigList.OrderBy(p => p.ExpireDays).ToList());
            if (overdueActivities.Any())
            {
                htmlStr = GenerateHtmlTableString(overdueActivities);
                //MailService.SendMail("活动到期提醒", ConfigurationManager.AppSettings["ActivityMonitorJob:To"], htmlstr);
            }
            else
            {
                Logger.Info($"{days}天内无过期的活动,故无需邮件发送");
            }
            return htmlStr;

        }

        public string MonitorAppActivity()
        {
            List<OverdueActivityConfig> OverdueActiveConfigList = new List<OverdueActivityConfig>();
            IEnumerable<ModuleContentConfig> activityitemlist = ModuleContentConfigDAL.GetActivityModuleContentConfigList();

            IEnumerable<ModuleContentConfig> promotionitemlist = ModuleContentConfigDAL.GetPromotionModuleContentConfigList();

            IEnumerable<FlowConfig> flowactivityList = ModuleContentConfigDAL.GetFlowConfigActivityList();
            IEnumerable<FlowConfig> flowpromotionList = ModuleContentConfigDAL.GetFlowConfigPromotionList();

            IEnumerable<FlowConfig> flowconfiglist = ModuleContentConfigDAL.GetFlowConfigList();
            DateTime nowday = DateTime.Now.Date;
            List<ActivityBuild> overdueActivityBuildList = new List<ActivityBuild>();
            List<ActivePage> overdueActivePageList = new List<ActivePage>();

            List<FlowConfig> overdueFlowConfigList = new List<FlowConfig>();
            #region 坑位中老活动过期判断
            if (activityitemlist != null && activityitemlist.Any())
            {
                foreach (var activityModuleitem in activityitemlist.Where(p => !string.IsNullOrWhiteSpace(p.LinkUrl) && p.LinkUrl.Contains(@"/staticpage/activity/")).ToList())
                {
                    Regex reg = new Regex(@"^\/webView\?url=(.*)$");
                    Match activityMatch = reg.Match(activityModuleitem.LinkUrl);
                    if (activityMatch.Success)
                    {
                        var encodeactivityUrl = activityMatch.Result("$1");
                        var decodeactivityUrl = HttpUtility.UrlDecode(encodeactivityUrl);
                        Regex idreg = new Regex(@"(.*)\?id=(\d+)(.*)$");
                        Match idMatch = idreg.Match(decodeactivityUrl);
                        if (idMatch.Success)
                        {
                            var idstring = idMatch.Result("$2");
                            List<ActivityBuild> activityBuildItemList = ModuleContentConfigDAL.GetActivityBuildById(Convert.ToInt32(idstring));
                            if (activityBuildItemList != null && activityBuildItemList.FirstOrDefault() != null)
                            {

                                var activityItem = activityBuildItemList.First();
                                var endDate = activityItem.EndDate.Date;
                                TimeSpan sp = endDate - nowday;

                                if (sp.Days <= 4 && sp.Days > 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = activityModuleitem.ModuleName,
                                        HomePageName = activityModuleitem.HomePageName,
                                        HelperModuleName = activityModuleitem.HelperModuleName,
                                        Url = decodeactivityUrl,
                                        Title = activityItem.Title,
                                        EndDate = activityItem.EndDate,
                                        Type = "活动",
                                        Status = 1 //yellow color overdue at once
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);

                                }
                                else if (sp.Days == 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = activityModuleitem.ModuleName,
                                        HomePageName = activityModuleitem.HomePageName,
                                        HelperModuleName = activityModuleitem.HelperModuleName,
                                        Url = decodeactivityUrl,
                                        Title = activityItem.Title,
                                        EndDate = activityItem.EndDate,
                                        Type = "活动",
                                        Status = 0 //orange  today overdue
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                }
                                else if (sp.Days < 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = activityModuleitem.ModuleName,
                                        HomePageName = activityModuleitem.HomePageName,
                                        HelperModuleName = activityModuleitem.HelperModuleName,
                                        Url = decodeactivityUrl,
                                        Title = activityItem.Title,
                                        EndDate = activityItem.EndDate,
                                        Type = "活动",
                                        Status = -1 //red   already overdue
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                }
                            }

                        }
                    }
                }


                foreach (var activityModuleitem in activityitemlist.Where(p => !string.IsNullOrWhiteSpace(p.MoreUri) && p.MoreUri.Contains(@"/staticpage/activity/")).ToList())
                {
                    Regex reg = new Regex(@"^\/webView\?url=(.*)$");
                    Match activityMatch = reg.Match(activityModuleitem.MoreUri);
                    if (activityMatch.Success)
                    {
                        var encodeactivityUrl = activityMatch.Result("$1");
                        var decodeactivityUrl = HttpUtility.UrlDecode(encodeactivityUrl);
                        Regex idreg = new Regex(@"(.*)\?id=(\d+)(.*)$");
                        Match idMatch = idreg.Match(decodeactivityUrl);
                        if (idMatch.Success)
                        {
                            var idstring = idMatch.Result("$2");
                            List<ActivityBuild> activityBuildItemList = ModuleContentConfigDAL.GetActivityBuildById(Convert.ToInt32(idstring));
                            if (activityBuildItemList != null && activityBuildItemList.FirstOrDefault() != null)
                            {

                                var activityItem = activityBuildItemList.First();
                                var endDate = activityItem.EndDate.Date;
                                TimeSpan sp = endDate - nowday;

                                if (sp.Days <= 4 && sp.Days > 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = activityModuleitem.ModuleName,
                                        HomePageName = activityModuleitem.HomePageName,
                                        HelperModuleName = activityModuleitem.HelperModuleName,
                                        ID = activityItem.id,
                                        Url = decodeactivityUrl,
                                        Title = activityItem.Title,
                                        EndDate = activityItem.EndDate,
                                        Type = "活动",
                                        Status = 1 //yellow color overdue at once
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);

                                }
                                else if (sp.Days == 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = activityModuleitem.ModuleName,
                                        HomePageName = activityModuleitem.HomePageName,
                                        HelperModuleName = activityModuleitem.HelperModuleName,
                                        Url = decodeactivityUrl,
                                        Title = activityItem.Title,
                                        EndDate = activityItem.EndDate,
                                        Type = "活动",
                                        Status = 0 //orange  today overdue
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                }
                                else if (sp.Days < 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = activityModuleitem.ModuleName,
                                        HomePageName = activityModuleitem.HomePageName,
                                        HelperModuleName = activityModuleitem.HelperModuleName,
                                        Url = decodeactivityUrl,
                                        Title = activityItem.Title,
                                        EndDate = activityItem.EndDate,
                                        Type = "活动",
                                        Status = -1 //red   already overdue
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                }
                            }

                        }
                    }
                }
            }
            #endregion

            #region 瀑布流中老活动过期判断
            if (flowactivityList != null && flowactivityList.Any())
            {
                foreach (var flowactivity in flowactivityList.Where(p => !string.IsNullOrWhiteSpace(p.LinkUrl) && p.LinkUrl.Contains(@"/staticpage/activity/")).ToList())
                {
                    //瀑布流没有过期才做后续处理
                    if (flowactivity.EndDateTime.Date >= nowday)
                    {
                        Regex reg = new Regex(@"^\/webView\?url=(.*)$");
                        Match activityMatch = reg.Match(flowactivity.LinkUrl);
                        if (activityMatch.Success)
                        {
                            var encodeactivityUrl = activityMatch.Result("$1");
                            var decodeactivityUrl = HttpUtility.UrlDecode(encodeactivityUrl);
                            Regex idreg = new Regex(@"(.*)\?id=(\d+)(.*)$");
                            Match idMatch = idreg.Match(decodeactivityUrl);
                            if (idMatch.Success)
                            {
                                var idstring = idMatch.Result("$2");
                                List<ActivityBuild> activityBuildItemList = ModuleContentConfigDAL.GetActivityBuildById(Convert.ToInt32(idstring));
                                if (activityBuildItemList != null && activityBuildItemList.FirstOrDefault() != null)
                                {

                                    var activityItem = activityBuildItemList.First();
                                    var endDate = activityItem.EndDate.Date;
                                    TimeSpan sp = endDate - nowday;

                                    if (sp.Days <= 4 && sp.Days > 0)
                                    {
                                        var overdueActivityConfigItem = new OverdueActivityConfig()
                                        {
                                            ModuleName = flowactivity.Title,
                                            Url = decodeactivityUrl,
                                            Title = activityItem.Title,
                                            EndDate = activityItem.EndDate,
                                            Type = "瀑布流",
                                            Status = 1 //yellow color overdue at once
                                        };
                                        OverdueActiveConfigList.Add(overdueActivityConfigItem);

                                    }
                                    else if (sp.Days == 0)
                                    {
                                        var overdueActivityConfigItem = new OverdueActivityConfig()
                                        {
                                            ModuleName = flowactivity.Title,
                                            Url = decodeactivityUrl,
                                            Title = activityItem.Title,
                                            EndDate = activityItem.EndDate,
                                            Type = "瀑布流",
                                            Status = 0 //orange  today overdue
                                        };
                                        OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                    }
                                    else if (sp.Days < 0)
                                    {
                                        var overdueActivityConfigItem = new OverdueActivityConfig()
                                        {
                                            ModuleName = flowactivity.Title,
                                            Url = decodeactivityUrl,
                                            Title = activityItem.Title,
                                            EndDate = activityItem.EndDate,
                                            Type = "瀑布流",
                                            Status = -1 //red   already overdue
                                        };
                                        OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                    }
                                }

                            }
                        }
                    }
                }
            }
            #endregion


            #region 坑位中新活动过期判断
            if (promotionitemlist != null && promotionitemlist.Any())
            {
                // var promotionlinkUrlList = promotionitemlist.Select(p => p.LinkUrl).Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                foreach (var promotionModuleItem in promotionitemlist.Where(p => !string.IsNullOrWhiteSpace(p.LinkUrl) && p.LinkUrl.Contains(@"/staticpage/promotion/activity/")).ToList())
                {
                    Regex reg = new Regex(@"^\/webView\?url=(.*)$");
                    Match promotionMatch = reg.Match(promotionModuleItem.LinkUrl);
                    if (promotionMatch.Success)
                    {
                        var encodepromotionUrl = promotionMatch.Result("$1");
                        var decodepromotionUrl = HttpUtility.UrlDecode(encodepromotionUrl);
                        Regex idreg = new Regex(@"(.*)\?id=(.*)&tuhu$");
                        Match idMatch = idreg.Match(decodepromotionUrl);
                        if (idMatch.Success)
                        {
                            var hashkey = idMatch.Result("$2");
                            List<ActivePage> promotionItemList = ModuleContentConfigDAL.GetActivePageByHashKey(hashkey);
                            if (promotionItemList != null && promotionItemList.FirstOrDefault() != null)
                            {

                                var promotionItem = promotionItemList.First();
                                var endDate = promotionItem.EndDate.Date;
                                TimeSpan sp = endDate - nowday;
                                if (sp.Days <= 4 && sp.Days > 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = promotionModuleItem.ModuleName,
                                        HomePageName = promotionModuleItem.HomePageName,
                                        HelperModuleName = promotionModuleItem.HelperModuleName,
                                        Url = decodepromotionUrl,
                                        Title = promotionItem.Title,
                                        EndDate = promotionItem.EndDate,
                                        Status = 1,
                                        Type = "活动"

                                    };

                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                }
                                else if (sp.Days == 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = promotionModuleItem.ModuleName,
                                        HomePageName = promotionModuleItem.HomePageName,
                                        HelperModuleName = promotionModuleItem.HelperModuleName,
                                        Url = decodepromotionUrl,
                                        Title = promotionItem.Title,
                                        EndDate = promotionItem.EndDate,
                                        Status = 0,
                                        Type = "活动"
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                }
                                else if (sp.Days < 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = promotionModuleItem.ModuleName,
                                        HomePageName = promotionModuleItem.HomePageName,
                                        HelperModuleName = promotionModuleItem.HelperModuleName,
                                        Url = decodepromotionUrl,
                                        Title = promotionItem.Title,
                                        EndDate = promotionItem.EndDate,
                                        Status = -1,
                                        Type = "活动"
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);

                                }
                            }
                        }
                    }
                }


                // var promotionMoreUriList = promotionitemlist.Select(p => p.MoreUri).Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                foreach (var promotionMoreUriItem in promotionitemlist.Where(p => !string.IsNullOrWhiteSpace(p.MoreUri) && p.MoreUri.Contains(@"/staticpage/promotion/activity/")).ToList())
                {
                    Regex reg = new Regex(@"^\/webView\?url=(.*)$");
                    Match promotionMatch = reg.Match(promotionMoreUriItem.MoreUri);
                    if (promotionMatch.Success)
                    {
                        var encodepromotionUrl = promotionMatch.Result("$1");
                        var decodepromotionUrl = HttpUtility.UrlDecode(encodepromotionUrl);
                        Regex idreg = new Regex(@"(.*)\?id=(.*)&tuhu$");
                        Match idMatch = idreg.Match(decodepromotionUrl);
                        if (idMatch.Success)
                        {
                            var hashkey = idMatch.Result("$2");
                            List<ActivePage> promotionItemList = ModuleContentConfigDAL.GetActivePageByHashKey(hashkey);
                            if (promotionItemList != null && promotionItemList.FirstOrDefault() != null)
                            {

                                var promotionItem = promotionItemList.First();
                                var endDate = promotionItem.EndDate.Date;
                                TimeSpan sp = endDate - nowday;
                                if (sp.Days <= 4 && sp.Days > 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = promotionMoreUriItem.ModuleName,
                                        HomePageName = promotionMoreUriItem.HomePageName,
                                        HelperModuleName = promotionMoreUriItem.HelperModuleName,
                                        Url = decodepromotionUrl,
                                        Title = promotionItem.Title,
                                        EndDate = promotionItem.EndDate,
                                        Status = 1,
                                        Type = "活动"

                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);

                                }
                                else if (sp.Days == 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = promotionMoreUriItem.ModuleName,
                                        HomePageName = promotionMoreUriItem.HomePageName,
                                        HelperModuleName = promotionMoreUriItem.HelperModuleName,
                                        Url = decodepromotionUrl,
                                        Title = promotionItem.Title,
                                        EndDate = promotionItem.EndDate,
                                        Status = 0,
                                        Type = "活动"
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                }
                                else if (sp.Days < 0)
                                {
                                    var overdueActivityConfigItem = new OverdueActivityConfig()
                                    {
                                        ModuleName = promotionMoreUriItem.ModuleName,
                                        HomePageName = promotionMoreUriItem.HomePageName,
                                        HelperModuleName = promotionMoreUriItem.HelperModuleName,
                                        Url = decodepromotionUrl,
                                        Title = promotionItem.Title,
                                        EndDate = promotionItem.EndDate,
                                        Status = -1,
                                        Type = "活动"
                                    };
                                    OverdueActiveConfigList.Add(overdueActivityConfigItem);

                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region 瀑布流中新活动过期判断
            if (flowpromotionList != null && flowpromotionList.Any())
            {
                foreach (var flowpromotionItem in flowpromotionList.Where(p => !string.IsNullOrWhiteSpace(p.LinkUrl) && p.LinkUrl.Contains(@"/staticpage/promotion/activity/")).ToList())
                {
                    if (flowpromotionItem.EndDateTime.Date >= nowday)
                    {
                        Regex reg = new Regex(@"^\/webView\?url=(.*)$");
                        Match promotionMatch = reg.Match(flowpromotionItem.LinkUrl);
                        if (promotionMatch.Success)
                        {
                            var encodepromotionUrl = promotionMatch.Result("$1");
                            var decodepromotionUrl = HttpUtility.UrlDecode(encodepromotionUrl);
                            Regex idreg = new Regex(@"(.*)\?id=(.*)&tuhu$");
                            Match idMatch = idreg.Match(decodepromotionUrl);
                            if (idMatch.Success)
                            {
                                var hashkey = idMatch.Result("$2");
                                List<ActivePage> promotionItemList = ModuleContentConfigDAL.GetActivePageByHashKey(hashkey);
                                if (promotionItemList != null && promotionItemList.FirstOrDefault() != null)
                                {

                                    var promotionItem = promotionItemList.First();
                                    var endDate = promotionItem.EndDate.Date;
                                    TimeSpan sp = endDate - nowday;
                                    if (sp.Days <= 4 && sp.Days > 0)
                                    {
                                        var overdueActivityConfigItem = new OverdueActivityConfig()
                                        {
                                            ModuleName = flowpromotionItem.Title,
                                            Url = decodepromotionUrl,
                                            Title = promotionItem.Title,
                                            EndDate = promotionItem.EndDate,
                                            Status = 1,
                                            Type = "瀑布流"

                                        };
                                        OverdueActiveConfigList.Add(overdueActivityConfigItem);

                                    }
                                    else if (sp.Days == 0)
                                    {
                                        var overdueActivityConfigItem = new OverdueActivityConfig()
                                        {
                                            ModuleName = flowpromotionItem.Title,
                                            Url = decodepromotionUrl,
                                            Title = promotionItem.Title,
                                            EndDate = promotionItem.EndDate,
                                            Status = 0,
                                            Type = "瀑布流"
                                        };
                                        OverdueActiveConfigList.Add(overdueActivityConfigItem);
                                    }
                                    else if (sp.Days < 0)
                                    {
                                        var overdueActivityConfigItem = new OverdueActivityConfig()
                                        {
                                            ModuleName = flowpromotionItem.Title,
                                            Url = decodepromotionUrl,
                                            Title = promotionItem.Title,
                                            EndDate = promotionItem.EndDate,
                                            Status = -1,
                                            Type = "瀑布流"
                                        };
                                        OverdueActiveConfigList.Add(overdueActivityConfigItem);

                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            //通过ModuleName以及URL做判重
            OverdueActiveConfigList = OverdueActiveConfigList.Distinct(new OverdueAcitivityConfigComparer()).ToList();


            string htmlString = "";
            if (OverdueActiveConfigList.Any())
            {
                Logger.Info($"有{OverdueActiveConfigList.Count()}个App首页模块活动页到期提醒需处理");
                htmlString = GenerateHtmlString(OverdueActiveConfigList);
              //  MailService.SendMail("App首页模块活动页到期提醒", ConfigurationManager.AppSettings["ActivityMonitorJob:To"], htmlString);

            }
            else
            {
                Logger.Info("无过期的APP首页活动,故无需邮件发送");
            }
            return htmlString;
        }

        public bool ActivityMonitor()
        {
            bool flag = true;
            try
            {

             
                var date = DateTime.Now.AddDays(4);

                Task<string> apptask = new Task<string>(() => MonitorAppActivity());
                apptask.Start();

                Task<string> H5task = new Task<string>(() => MonitorH5Activity());
                //H5task.
                H5task.Start();

                var LightApptask = new Task<string>(() => MonitorLightAppActivity());

                LightApptask.Start();

                var allactivitiestask = new Task<string>(() => MonitorSpecificDaysExpireActivites(4));
                allactivitiestask.Start();
                Task.WaitAll(new Task[] { apptask,H5task, LightApptask, allactivitiestask });
                var appresult = apptask.Result;
                var h5result = H5task.Result;
                var lightAppresult = LightApptask.Result;
                var allactivitiesresult = allactivitiestask.Result;
                if (!string.IsNullOrWhiteSpace(appresult) || !string.IsNullOrWhiteSpace(h5result) || !string.IsNullOrWhiteSpace(lightAppresult) || !string.IsNullOrWhiteSpace(allactivitiesresult))
                {
                    StringBuilder message = new StringBuilder("Hi all,");
                    message.AppendLine("<br/>");
                    message.AppendLine("<br/>");
                    message.AppendLine("<div><pre>以下是已过期及待过期的活动列表，请运营同学确认，及时更新配置活动(背景色黄色为待过期,红色为已过期)</pre></div>");
                    //   message.AppendLine("");

                    if (!string.IsNullOrWhiteSpace(appresult))
                    {
                        message.AppendLine("<br/>");
                        message.AppendLine("<br/>");

                        message.AppendLine("<div align=center><b>App首页活动过期列表:</b></div>");
                        message.AppendLine(appresult);
                    }
                    if (!string.IsNullOrWhiteSpace(h5result))
                    {
                        message.AppendLine("<br/>");
                        message.AppendLine("<br/>");

                        message.AppendLine("<div align=center><b>华为轻应用首页活动过期列表:</b></div>");
                        message.AppendLine(h5result);
                    }
                    if (!string.IsNullOrWhiteSpace(lightAppresult))
                    {
                        message.AppendLine("<br/>");
                        message.AppendLine("<br/>");
                        message.AppendLine("<div align=center><b>小程序首页活动过期列表:</b></div>");
                        message.AppendLine(lightAppresult);
                    }
                    if (!string.IsNullOrWhiteSpace(allactivitiesresult))
                    {
                        message.AppendLine("<br/>");
                        message.AppendLine("<br/>");
                        message.AppendLine("<div align=center><b>4天内活动过期列表:</b></div>");
                        message.AppendLine(allactivitiesresult);
                    }
                    MailService.SendMail("【到期提醒】过期活动配置", ConfigurationManager.AppSettings["ActivityMonitorJob:To"], message.ToString());
                }
                else
                {
                    Logger.Info($"4天内无过期的活动,故无需邮件发送");
                }
             
            }

            catch (Exception ex)
            {
                Logger.Error($"ActivityMonitor Exception：{ex.ToString()}");
                flag = false;
            }
            return flag;
        }

        public string GenerateHtmlTableString(List<OverdueActivityConfig> OverdueActiveConfigList)
        {
            StringBuilder sb = new StringBuilder();
            sb = sb.Append("<table border=1" + " align=center" + " width =1000 " + "><tr bgcolor='#4da6ff'><td align=center><b>名称</b></td> <td align=center> <b> 类型</b> </td> <td align=center> <b> 地址</b> </td><td align=center> <b> 结束时间</b> </td> <td align=center> <b> 剩余过期时间</b> </td></tr>");
            foreach (var item in OverdueActiveConfigList)
            {
                string color = "yellow";
                int days = (item.EndDate.Date - DateTime.Now.Date).Days;
                sb.AppendFormat("<tr style=\"background-color:" + color + "\">");

                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.Title ?? "");
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.Type);
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.Url);
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.EndDate.ToString());
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.ExpireDays.ToString());
                sb.AppendFormat("</td>");
                sb.AppendFormat("</tr>\n");
            }
            sb.AppendFormat("</table>");

            return sb.ToString();
        }

        public string GenerateHtmlString(List<OverdueActivityConfig> OverdueActiveConfigList)
        {
            StringBuilder sb = new StringBuilder();
            sb = sb.Append(@"<table border=1" + " align=center" + " width =1000 " + "><tr bgcolor='#4da6ff'><td align=center><b>名称</b></td> <td align=center> <b> 类型</b> </td> <td align=center> <b> 地址</b> </td><td align=center> <b> 所在首页名称</b> </td> <td align=center> <b> 所在模块名称</b> </td><td align=center> <b> 所在帮助模块名称</b> </td><td align=center> <b> 结束时间</b> </td> <td align=center> <b> 剩余过期时间</b> </td></tr>");
            foreach (var item in OverdueActiveConfigList.OrderBy(p => p.EndDate))
            {
                string color = item.Status > 0 ? "yellow" : "orangered";
                int days = (item.EndDate.Date - DateTime.Now.Date).Days;
                sb.AppendFormat("<tr style=\"background-color:" + color + "\">\n");

                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.Title);
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.Type);
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.Url ?? "");
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.HomePageName ?? "");
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.ModuleName ?? "");
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.HelperModuleName ?? "");
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.EndDate.ToString("yyyy-MM-dd"));
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(days.ToString());
                sb.AppendFormat("</td>");
                sb.AppendFormat("</tr>\n");
            }
            sb.AppendFormat("</table>");

            return sb.ToString();
        }


        public string GenerateH5andLightAppHtmlString(List<OverdueActivityConfig> OverdueActiveConfigList)
        {
            StringBuilder sb = new StringBuilder();
            sb = sb.Append(@"<table border=1" + " align=center" + " width =1000 " + "><tr bgcolor='#4da6ff'><td align=center><b>名称</b></td> <td align=center> <b> 类型</b> </td> <td align=center> <b> 地址</b> </td><td align=center> <b> 所在模块名称</b> </td><td align=center> <b> 所在帮助模块名称</b> </td><td align=center> <b> 结束时间</b> </td> <td align=center> <b> 剩余过期时间</b> </td></tr>");
            foreach (var item in OverdueActiveConfigList.OrderBy(p => p.EndDate))
            {
                string color = item.Status > 0 ? "yellow" : "orangered";
                int days = (item.EndDate.Date - DateTime.Now.Date).Days;
                sb.AppendFormat("<tr style=\"background-color:" + color + "\">\n");

                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.Title);
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.Type);
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.Url ?? "");
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.ModuleName ?? "");
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.HelperModuleName ?? "");
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(item.EndDate.ToString("yyyy-MM-dd"));
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td style=\"text-align:center\">");
                sb.AppendFormat(days.ToString());
                sb.AppendFormat("</td>");
                sb.AppendFormat("</tr>\n");
            }
            sb.AppendFormat("</table>");

            return sb.ToString();
        }

    }
}
