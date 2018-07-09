using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;
using System.Web.Configuration;
using Tuhu.Component.Framework.Extension;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class BannerConfigController : Controller
    {
        private readonly Lazy<DownloadAppManager> lazy = new Lazy<DownloadAppManager>();

        private DownloadAppManager DownloadAppManager
        {
            get { return lazy.Value; }
        }

        private static readonly Lazy<VIPAuthorizationRuleConfigManager> lazyRule = new Lazy<VIPAuthorizationRuleConfigManager>();

        private VIPAuthorizationRuleConfigManager VIPAuthorizationRuleConfigManager
        {
            get
            {
                return lazyRule.Value;
            }
        }

        private Dictionary<string, string> GetTargetGroupDic()
        {
            var targetGroups = DownloadAppManager.GetTargetGroup();
            var result = new Dictionary<string, string>();
            if (targetGroups != null && targetGroups.Any())
            {
                Dictionary<string, string> targetGroupDic = new Dictionary<string, string>();
                foreach (var tg in targetGroups)
                {
                    if (!targetGroupDic.ContainsKey(tg.TargetKey))
                        targetGroupDic.Add(tg.TargetKey, tg.TargetGroups);
                }
                result = targetGroupDic;// JsonConvert.SerializeObject(targetGroupDic);
            }
            return result;
        }

        //[PowerManage]
        public ViewResult Index()
        {
            var bannerConfigs = DownloadAppManager.QueryBannerConfig(new BannerFilterQuery(), 1);
            if (bannerConfigs != null && bannerConfigs.Any())
            {
                foreach (var banner in bannerConfigs)
                    if (!string.IsNullOrWhiteSpace(banner.Image))
                    {
                        banner.Image = WebConfigurationManager.AppSettings["DoMain_image"] + banner.Image;
                    }
            }
            ViewBag.NoticeChannel = DownloadAppManager.QueryNoticeChannel() ?? new List<NoticeChannel>();
            ViewBag.AllTargetGroups = GetTargetGroupDic();
            ViewBag.RuleList = VIPAuthorizationRuleConfigManager.GetVIPAuthorizationRuleAndId();
            ViewBag.NumOfBanners = DownloadAppManager.CountBannerConfig(new BannerFilterQuery());
            ViewBag.Page = 1;
            return View(bannerConfigs ?? new List<BannerConfig>());
        }

        [HttpPost]
        public ViewResult Index(BannerFilterQuery bannerfilterQuery, int page = 1)
        {
            var bannerConfigs = DownloadAppManager.QueryBannerConfig(bannerfilterQuery, page);
            if (bannerConfigs != null && bannerConfigs.Any())
            {
                foreach (var banner in bannerConfigs)
                    if (!string.IsNullOrWhiteSpace(banner.Image))
                    {
                        banner.Image = WebConfigurationManager.AppSettings["DoMain_image"] + banner.Image;
                    }
            }
            ViewBag.NoticeChannel = DownloadAppManager.QueryNoticeChannel() ?? new List<NoticeChannel>();
            ViewBag.AllTargetGroups = GetTargetGroupDic();
            ViewBag.RuleList = VIPAuthorizationRuleConfigManager.GetVIPAuthorizationRuleAndId();
            ViewBag.NumOfBanners = DownloadAppManager.CountBannerConfig(bannerfilterQuery);
            ViewBag.Page = page;
            return View(bannerConfigs ?? new List<BannerConfig>());
        }

        [HttpPost]
        public JsonResult AddOrUpdate(BannerConfig model)
        {
            if (!string.IsNullOrWhiteSpace(model.Image))
            {
                model.Image = model.Image.Replace(WebConfigurationManager.AppSettings["DoMain_image"], "");
            }
            if (!string.IsNullOrWhiteSpace(model.StartVersion))
            {
                model.StartVersion = model.StartVersion.Trim();
            }
            if (!string.IsNullOrWhiteSpace(model.EndVersion))
            {
                model.EndVersion = model.EndVersion.Trim();
            }
            if (string.IsNullOrWhiteSpace(model.Creator))
            {
                model.Creator = string.Empty;
            }

            if (model.Id < 0)
            {
                var result = DownloadAppManager.InsertBannerConfig(model);
                if (result > 0)
                {
                    var oprLog = new OprLog
                    {
                        ObjectID = result,
                        ObjectType = "BannerConfig",
                        AfterValue =
                            "位置： " + model.Location + "， 平台：" + model.Channel + "， 状态：" + model.Status + "， 目标群体：" +
                            (string.IsNullOrWhiteSpace(model.TargetGroups) ? "" : model.TargetGroups),
                        Author = HttpContext.User.Identity.Name,
                        Operation = "新建Banner配置"
                    };
                    new OprLogManager().AddOprLog(oprLog);
                }
                return Json(result);
            }
            else
            {
                var result = DownloadAppManager.UpdateBannerConfig(model);
                if (result > 0)
                {
                    var oprLog = new OprLog
                    {
                        ObjectID = model.Id,
                        ObjectType = "BannerConfig",
                        AfterValue =
                            "位置： " + model.Location + "， 平台：" + model.Channel + "， 状态：" + model.Status + "， 目标群体：" +
                            (string.IsNullOrWhiteSpace(model.TargetGroups) ? "" : model.TargetGroups),
                        Author = HttpContext.User.Identity.Name,
                        Operation = "更新Banner配置"
                    };
                    new OprLogManager().AddOprLog(oprLog);
                }
                return Json(result);
            }
        }

        public ActionResult SelectOprLogByPKID(int PKID)
        {
            var result = LoggerManager.SelectOprLogByParams("BannerConfig", PKID.ToString());
            return result != null && result.Any()
                ? Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet)
                : Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBannerConfig(int Id)
        {
            var deleteResult = DownloadAppManager.DeleteBannerConfig(Id);
            if (!deleteResult)
                return Json(0, JsonRequestBehavior.AllowGet);

            var oprLog = new OprLog
            {
                ObjectID = Id,
                ObjectType = "BannerConfig",
                Author = HttpContext.User.Identity.Name,
                Operation = "删除Banner配置"
            };
            new OprLogManager().AddOprLog(oprLog);
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadImage()
        {
            var result = "";
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    using (var client = new FileUploadClient())
                    {
                        var res = client.UploadImage(new ImageUploadRequest()
                        {
                            Contents = buffer,
                            DirectoryName = "BannerConfigImg",
                            MaxHeight = 1920,
                            MaxWidth = 1920
                        });
                        if (res.Success && res.Result != null)
                        {
                            result = WebConfigurationManager.AppSettings["DoMain_image"] + res.Result;
                        }
                    }
                }
                catch (Exception exp)
                {
                    WebLog.LogException(exp);
                }
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult CacheUpdate()
        {
            try
            {
                using (var client = new PersonalCenterClient())
                {
                    var res = client.RefreshPersonalCenterBannerCache();
                    res.ThrowIfException(true);
                    if (res.Success && res.Result)
                    {
                        return Json(1);
                    }
                    else
                    {
                        return Json(-1);
                    }
                }
            }
            catch (Exception exp)
            {
                WebLog.LogException(exp);
                return Json(-1);
            }
        }
    }
}