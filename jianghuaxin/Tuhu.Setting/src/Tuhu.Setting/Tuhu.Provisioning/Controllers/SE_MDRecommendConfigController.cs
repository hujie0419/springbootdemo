using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.MDBeauty;
using Newtonsoft.Json;
using Tuhu.Service.GroupBuying;

namespace Tuhu.Provisioning.Controllers
{
    public class SE_MDRecommendConfigController : Controller
    {
        //
        // GET: /SE_MDRecommendConfig/

        public ActionResult Index(int id = 0)
        {
            ViewBag.PartId = id;
            var source = SE_MDRecommendConfigBLL.SelectPages(1, 10, string.Format(" and PartId = {0} ", id)) ?? new List<SE_MDRecommendConfigModel>();
            return View(source);
        }


        public ActionResult GetProducts(int part,int type,int vtype)
        {
            return Json(SE_MDRecommendConfigBLL.GetProducts(part,type,vtype));
        }

        //
        // GET: /SE_MDRecommendConfig/Edit/5

        public JsonResult Edit(int partId = 0, string model = "")
        {
            try
            {
                List<SE_MDRecommendConfigModel> dataSource = JsonConvert.DeserializeObject<List<SE_MDRecommendConfigModel>>(model);
                if (partId > 0)
                {
                    foreach (var item in dataSource)
                    {
                        if (item.Id > 0)
                        {
                            SE_MDRecommendConfigBLL.Update(new SE_MDRecommendConfigModel()
                            {
                                Id = item.Id,
                                PartId = partId,
                                Type = item.Type,
                                VehicleType = item.VehicleType,
                                Products = item.Products,
                                IsDisable = item.IsDisable,
                                CreateTime = DateTime.Now
                            });
                        }
                        else
                        {
                            SE_MDRecommendConfigBLL.Insert(new SE_MDRecommendConfigModel()
                            {
                                PartId = partId,
                                Type = item.Type,
                                VehicleType = item.VehicleType,
                                Products = item.Products,
                                IsDisable = item.IsDisable,
                                CreateTime = DateTime.Now
                            });
                        }
                    }
                }
                return Json(new { code = 1, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 0, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult IsExistsPId(string pid)
        {
            if (string.IsNullOrWhiteSpace(pid))
                return Json(new { code = 0, msg = "PID为空,无法匹配!" }, JsonRequestBehavior.AllowGet);

            string result = SE_MDRecommendConfigBLL.IsExistsPId(pid);

            if (!string.IsNullOrWhiteSpace(result))
                return Json(new { code = 1, msg = $"匹配成功，【{result}】！" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { code = 0, msg = "匹配失败，PID不存在！" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RefreshBeautyCache()
        {
            var manager = new BeautyHomePageConfigManager();
            var channels = manager.GetBeautyChannel();
            if (channels.Any())
            {
                var results = channels.Select(x => RefreshBeautyConfigCache(x)).ToList();
                return Json(new { Status = results.All(x => x) });
            }
            return Json(new { Status = false });
        }

        private bool RefreshBeautyConfigCache(string channel)
        {
            using (var client = new GroupBuyingClient())
            {
                var serviceResult = client.RefreshBeautyConfigCache(channel);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }



    }
}
