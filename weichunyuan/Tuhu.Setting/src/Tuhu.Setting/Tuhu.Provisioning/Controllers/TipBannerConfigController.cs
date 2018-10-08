using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.TipBannerConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class TipBannerConfigController : Controller
    {
        // GET: TipBannerConfig
        public ActionResult TipBannerConfig()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddTipBannerTypeConfig(TipBannerTypeConfigModel model = null)
        {
            if (string.IsNullOrWhiteSpace(model.TypeName))
            {
                return Json(new { Status = false, Msg = "新类型信息不能为空" }, JsonRequestBehavior.AllowGet);
            }
            TipBannerConfigManager manager = new TipBannerConfigManager();
            var isRepeat = manager.IsRepeatTipBannerTypeConfig(model.TypeName);
            if (isRepeat)
            {
                return Json(new { Status = false, Msg = "不能添加重复数据" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.AddTipBannerTypeConfig(model, User.Identity.Name);
            if (result)
            {
                return Json(new { Status = true, Msg = "添加新类型成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "添加新类型失败" });
            }
        }

        [HttpPost]
        public ActionResult AddTipBannerDetailConfig(TipBannerConfigDetailModel model)
        {
            var requiredPars = new List<string> { model.BackgroundColor, model.Content, model.ContentColor };
            if (requiredPars.Any(str => string.IsNullOrWhiteSpace(str)) || (model.TypeId < 1) || (model.BgTransparent < 0 || model.BgTransparent > 1))
            {
                return Json(new { Status = false, Msg = "请检查必填项" }, JsonRequestBehavior.AllowGet);
            }
            TipBannerConfigManager manager = new TipBannerConfigManager();
            var isRepeat = manager.IsRepeatTipBannerDetailConfig(model);
            if (isRepeat)
            {
                return Json(new { Status = false, Msg = "存在重复数据,同类型只允许开启一个配置" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.AddTipBannerDetailConfig(model, User.Identity.Name);
            if (result)
            {
                return Json(new { Status = true, Msg = "添加成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "添加失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteTipBannerDetailConfig(int pkid)
        {
            if (pkid < 1)
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            TipBannerConfigManager manager = new TipBannerConfigManager();
            var result = manager.DeleteTipBannerDetailConfigByPKID(pkid, User.Identity.Name);
            if (result)
            {
                return Json(new { Status = true, Msg = "删除成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateTipBannerDetailConfig(TipBannerConfigDetailModel model)
        {
            if (model.PKID < 1)
            {
                return Json(new { Status = false, Msg = "未知的更新对象" }, JsonRequestBehavior.AllowGet);
            }
            var requiredPars = new List<string> { model.BackgroundColor, model.Content, model.ContentColor };
            if (requiredPars.Any(str => string.IsNullOrWhiteSpace(str)) || (model.TypeId < 1) || (model.BgTransparent < 0 || model.BgTransparent > 1))
            {
                return Json(new { Status = false, Msg = "请检查必填项" }, JsonRequestBehavior.AllowGet);
            }
            TipBannerConfigManager manager = new TipBannerConfigManager();
            var isRepeat = manager.IsRepeatTipBannerDetailConfig(model);
            if (isRepeat)
            {
                return Json(new { Status = false, Msg = "存在重复数据,同类型只允许开启一个配置" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.UpdateTipBannerDetailConfig(model, User.Identity.Name);
            if (result)
            {
                return Json(new { Status = true, Msg = "更新成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "更新失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SelectTipBannerDetailConfig(int typeId = -1, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new TipBannerConfigManager();
            var result = manager.SelectTipBannerDetailConfig(typeId, pageIndex, pageSize);
            if (result.Item1 == null)
            {
                return Json(new { Status = false, Msg = "查询失败" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var totalPage = (result.Item2 % pageSize == 0) ? ((int)result.Item2 / pageSize) : ((int)result.Item2 / pageSize + 1);
                return Json(new { Status = true, Data = result.Item1, TotalCount = result.Item2, TotalPage = totalPage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllTipBannerTypeConfig()
        {
            TipBannerConfigManager manager = new TipBannerConfigManager();
            var result = manager.GetAllTipBannerTypeConfig();
            if (result.Any())
            {
                return Json(new { Status = true, Data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "无提示条类型信息" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RefreshTipBannerConfigCache()
        {
            TipBannerConfigManager manager = new TipBannerConfigManager();
            var result = manager.RefreshTipBannerConfigCache();
            return Json(new { Status = result, Msg = "刷新缓存" + (result ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }
    }
}