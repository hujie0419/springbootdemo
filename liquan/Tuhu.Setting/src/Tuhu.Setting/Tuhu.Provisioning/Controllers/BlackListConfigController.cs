using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.BlackListConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class BlackListConfigController : Controller
    {
        private static readonly BlackListConfigManage _BlackListConfigManage = new BlackListConfigManage();
        public ActionResult Index()
        {
            return View(_BlackListConfigManage.GetList(null));
        }

        /// <summary>
        /// 解除黑名单
        /// </summary>
        public ActionResult DelBlackList(int id)
        {
            if (id > 0)
            {
                bool result = _BlackListConfigManage.Delete(id);
                return Content(result ? "1" : "0");
            }
            return Content("0");
        }

        /// <summary>
        /// 添加黑名单
        /// </summary>
        public ActionResult AddBlackList(string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                bool result = _BlackListConfigManage.AddOnlyUserID(userId);
                return Content(result ? "1" : "0");
            }
            return Content("0");
        }
    }
}