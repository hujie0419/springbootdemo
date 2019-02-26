using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class AnnouncementConfigController : Controller
    {
        private static readonly Lazy<AnnouncementConfigManager> lazy = new Lazy<AnnouncementConfigManager>();

        private AnnouncementConfigManager AnnouncementConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
            string strSql = string.Empty;
            AnnouncementConfig model = new AnnouncementConfig();

            List<AnnouncementConfig> lists = AnnouncementConfigManager.GetAnnouncementConfigList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<AnnouncementConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<AnnouncementConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                AnnouncementConfig model = new AnnouncementConfig();
                return View(model);
            }
            else
            {
                return View(AnnouncementConfigManager.GetAnnouncementConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(AnnouncementConfig model)
        {
            model.CreatedUser = HttpContext.User.Identity.Name;
            string js = "<script>alert(\"保存失败 \");location='/AnnouncementConfig/Index';</script>";
         
            if (model.PKID != 0)
            {
                if (AnnouncementConfigManager.UpdateAnnouncementConfig(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(js);
                }
            }
            else
            {
                if (model.NoticeType == 0)
                {
                    js = "<script>alert(\"请选择类型 \");location='/AnnouncementConfig/Index';</script>";
                    return Content(js);
                }
                if (AnnouncementConfigManager.InsertAnnouncementConfig(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(js);
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(AnnouncementConfigManager.DeleteAnnouncementConfig(id));
        }

    }
}
