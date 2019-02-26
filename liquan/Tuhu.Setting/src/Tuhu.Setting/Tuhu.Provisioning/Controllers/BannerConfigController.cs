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
using Tuhu.Provisioning.Models;

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

            #region 下拉列表
            ViewBag.ChannelDLL = ChannelDropDownList();
            ViewBag.LocationDLL = LocationDropDownList();
            ViewBag.TargetSmallAppIdDLL = TargetSmallAppIdDropDownList();
            #endregion
            return View(bannerConfigs ?? new List<BannerConfig>());
        }

        #region 页面下拉列表数据
        /// <summary>
        /// 获取下拉列表数据--平台
        /// </summary>
        /// <returns></returns>
        public List<DropDownListViewModel> ChannelDropDownList()
        {
            var result = new List<DropDownListViewModel>();
            result.Add(new DropDownListViewModel() { Id = "0", Name = "全部", IsSelect=true});
            result.Add(new DropDownListViewModel() { Id = "1", Name = "Android" });
            result.Add(new DropDownListViewModel() { Id = "2", Name = "IOS" });
            result.Add(new DropDownListViewModel() { Id = "3", Name = "途虎查违章小程序" });
            return result;
        }
        /// <summary>
        ///获取下拉列表数据--位置
        /// </summary>
        /// <returns></returns>
        public List<DropDownListViewModel> LocationDropDownList()
        {
            var result = new List<DropDownListViewModel>();
            result.Add(new DropDownListViewModel() { Id = "0", Name = "全部", IsSelect = true });
            result.Add(new DropDownListViewModel() { Id = "1", Name = "签到弹窗" });
            result.Add(new DropDownListViewModel() { Id = "2", Name = "个人中心底部" });
            result.Add(new DropDownListViewModel() { Id = "3", Name = "会员特权中间" });
            result.Add(new DropDownListViewModel() { Id = "4", Name = "评价结果页顶部" });
            result.Add(new DropDownListViewModel() { Id = "5", Name = "违章查询无结果" });
            result.Add(new DropDownListViewModel() { Id = "8", Name = "会员商城弹窗" });
            result.Add(new DropDownListViewModel() { Id = "9", Name = "第三方商城顶部" });
            result.Add(new DropDownListViewModel() { Id = "10", Name = "个人中心中间" });
            result.Add(new DropDownListViewModel() { Id = "11", Name = "下单成功页" });
            result.Add(new DropDownListViewModel() { Id = "12", Name = "美容下单成功页弹窗" });
            result.Add(new DropDownListViewModel() { Id = "13", Name = "拼团首页顶部" });
            result.Add(new DropDownListViewModel() { Id = "14", Name = "违章查询页广告位" });
            result.Add(new DropDownListViewModel() { Id = "15", Name = "违章详情页有违章页面广告" });
            result.Add(new DropDownListViewModel() { Id = "20", Name = "个人中心弹窗" });
            result.Add(new DropDownListViewModel() { Id = "21", Name = "个人中心悬浮窗" });
            result.Add(new DropDownListViewModel() { Id = "30", Name = "砍价商城头部" });
            result.Add(new DropDownListViewModel() { Id = "31", Name = "砍价详情悬浮窗" });
            result.Add(new DropDownListViewModel() { Id = "32", Name = "砍价详情底部" });
            result.Add(new DropDownListViewModel() { Id = "40", Name = "会员中心Banner" });
            result.Add(new DropDownListViewModel() { Id = "41", Name = "个人中心生日特权弹窗" });
            result.Add(new DropDownListViewModel() { Id = "42", Name = "车友群banner图" });
            result.Add(new DropDownListViewModel() { Id = "43", Name = "会员福利中心" });
            return result;
        }

        /// <summary>
        ///获取下拉列表数据--目标小程序
        /// </summary>
        /// <returns></returns>
        public List<DropDownListViewModel> TargetSmallAppIdDropDownList()
        {
            var result = new List<DropDownListViewModel>();
            result.Add(new DropDownListViewModel() { Id = "", Name = "全部", IsSelect=true });
            result.Add(new DropDownListViewModel() { Id = "wx27d20205249c56a3", Name = "途虎养车Tuhu" });
            result.Add(new DropDownListViewModel() { Id = "wx25f9f129712845af", Name = "途虎拼团购" });
            result.Add(new DropDownListViewModel() { Id = "wx3429052123c19770", Name = "途虎查违章" });
            result.Add(new DropDownListViewModel() { Id = "wxfa83eefa43f2c0e9", Name = "途虎洗车" });
            result.Add(new DropDownListViewModel() { Id = "wx625c1a4227bab347", Name = "途虎众测" });
            result.Add(new DropDownListViewModel() { Id = "wx41fa1a41dc9816a2", Name = "途虎工场店" });
            result.Add(new DropDownListViewModel() { Id = "wxcb33a9e9af575642", Name = "途虎问答" });
            return result;
        }
        /// <summary>
        ///获取下拉列表数据--平台
        /// </summary>
        /// <returns></returns>
        public List<DropDownListViewModel> GetTargetDropDownListData()
        {
            var result = new List<DropDownListViewModel>();
            result.Add(new DropDownListViewModel() { Id = "", Name = "" });
            result.Add(new DropDownListViewModel() { Id = "", Name = "" });
            result.Add(new DropDownListViewModel() { Id = "", Name = "" });
            result.Add(new DropDownListViewModel() { Id = "", Name = "" });
            result.Add(new DropDownListViewModel() { Id = "", Name = "" });
            result.Add(new DropDownListViewModel() { Id = "", Name = "" });
            result.Add(new DropDownListViewModel() { Id = "", Name = "" });
            return result;
        }
        #endregion


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
            #region 下拉列表
            ViewBag.ChannelDLL = ChannelDropDownList();
            ViewBag.LocationDLL = LocationDropDownList();
            ViewBag.TargetSmallAppIdDLL = TargetSmallAppIdDropDownList();
            #endregion
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
                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
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
                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
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

            var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
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