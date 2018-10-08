using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ServiceAllController : Controller
    {
        //Type 4 服务大全
        // GET: ServiceAll
        public ActionResult Index()
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            return View(manager.GetFlowList("4")?.Where(_=>_.ParentPKID == null));
        }

        public ActionResult FlowEdit(int id, string type)
        {

            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (id == 0)
            {
                if (string.IsNullOrWhiteSpace(type))
                    type = "1";

                return View(new SE_HomePageFlowConfig() { Type = type.ToString() });
            }
            else
                return View(manager.GetFlowEntity(id));
        }

        public ActionResult Edit(int id, string type,int parentPKID)
        {
            ViewBag.ParentPKID = parentPKID;
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (id == 0)
            {
                if (string.IsNullOrWhiteSpace(type))
                    type = "1";

                return View(new SE_HomePageFlowConfig() { Type = type.ToString() });
            }
            else
                return View(manager.GetFlowEntity(id));
        }


        public ActionResult List(int parentId)
        {
            ViewBag.ParentPKID = parentId;
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var list = manager.GetFlowList("4");
            ViewBag.Title = list.Where(_ => _.ID == parentId)?.FirstOrDefault().Title;
            return View(list?.Where(_=>_.ParentPKID == parentId));
        }

        public async  Task<ActionResult> Reload()
        {
            using (var client = new Tuhu.Service.Config.HomePageClient())
            {
                var result = await client.RefreshServiceEncyclopediasAsync();
                return result.Success ? Content("1") : Content("0");
            }
        }
    }
}