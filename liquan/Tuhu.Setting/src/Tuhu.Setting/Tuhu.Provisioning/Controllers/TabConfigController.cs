using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.TagConfig;
using Tuhu.Provisioning.DataAccess.Entity.TagConfig;

namespace Tuhu.Provisioning.Controllers
{
    public class TabConfigController : Controller
    {
        private readonly Lazy<TagConfigManager> tagConfig = new Lazy<TagConfigManager>();

        private TagConfigManager TagConfigManager
        {
            get { return this.tagConfig.Value; }
        }

        public ActionResult ArticleTabConfig()
        {
            return View();
        }
        /// <summary>
        /// 获取标签配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult SelectArticleTabConfig(int pageIndex = 1, int pageSize = 10)
        {
            var result = TagConfigManager.SelectArticleTabConfig(pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加标签配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public JsonResult InsertArticleTabConfig(ArticleTabConfig config)
        {
            var result = TagConfigManager.InsertArticleTabConfig(config, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public JsonResult RefreshArticleTabConfigCache()
        {
            var result = TagConfigManager.RefreshTArticleTabConfigCache();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}