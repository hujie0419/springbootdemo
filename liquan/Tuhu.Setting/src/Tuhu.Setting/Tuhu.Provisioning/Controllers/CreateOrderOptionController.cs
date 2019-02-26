using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Config;
using Tuhu.Service.Config.Models.Requests;
using OprLog = Tuhu.Provisioning.DataAccess.Entity.OprLog;

namespace Tuhu.Provisioning.Controllers
{
    public class CreateOrderOptionController : Controller
    {
        // GET: CreateOrderOption
        public ActionResult Index()
        {
            var model = OrderOptionManager.SelectOrderOptions();
            return View(model);
        }
        public ActionResult SelectPids(int id)
        {
            var result = OrderOptionManager.SelectOrderOptionReferProducts(id);
            if (result.Any())
            {
               // var json = JsonConvert.SerializeObject(result);
                return Json(new
                {
                    status = 1,
                    data = result.ToList()
                });
            }
            else
            {
                return Json(new
                {
                    status = 0
                });
            }
        }
        [PowerManage]
        public ActionResult Edit(string value, int id)
        {
            var model = JsonConvert.DeserializeObject<List<OrderOptionReferProductModel>>(value);
            var result = OrderOptionManager.UpdateOrderOptionReferProducts(model, id, HttpContext.User.Identity.Name);

            return Json(new
            {
                state = result ? 1 : 0
            });
        }
        public ActionResult SelectById(int id)
        {
            if (id == 0)
            {
                return View("Edit", new TireCreateOrderOptionsConfigModel());
            }
            var models = OrderOptionManager.SelectOrderOptions();
            var model = models.FirstOrDefault(r => r.Id == id);
            return View("Edit", model);
        }
        [PowerManage]
        public JsonResult Save(TireCreateOrderOptionsConfigModel model)
        {
            if (model.Id > 0)
            {
                var origin = OrderOptionManager.SelectOrderOptionsById(model.Id);
                if (OrderOptionManager.UpdateOrderOption(model))
                {
                     LoggerManager.InsertLog("OrderOpertionOprLog",new FlashSaleProductOprLog()
                    {
                         OperateUser = HttpContext.User.Identity.Name,
                         BeforeValue = JsonConvert.SerializeObject(origin),
                         AfterValue = JsonConvert.SerializeObject(model),
                         CreateDateTime = DateTime.Now,
                         LogId = model.Id.ToString(),
                         LogType = "OOption",
                         Operation = "修改下单可选项配置",
                    });
                    return Json(new { Status = 1 });
                }
                return Json(new { Status = -1 });
            }
            else
            {
                if (OrderOptionManager.InsertOrderOption(model))
                {
                    LoggerManager.InsertLog("OrderOpertionOprLog", new FlashSaleProductOprLog()
                    {
                        OperateUser = HttpContext.User.Identity.Name,
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        CreateDateTime = DateTime.Now,
                        LogId = model.Id.ToString(),
                        LogType = "OOption",
                        Operation = "新增下单可选项配置",
                    });
                    return Json(new { Status = 1 });
                }
                return Json(new { Status = -1 });
            }
        }
        public ActionResult ListBoostrap(string objectType,int id)
        {
            //if (startDT == "" || endDT == "")
            //{
            //    startDT = DateTime.Now.ToString("yyyy-MM-dd");
            //    endDT = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            //}
            //ViewBag.StartDate = startDT;
            //ViewBag.EndDate = endDT;
            //LoggerManager manger = new LoggerManager();
            var list = LoggerManager.SelectFlashSaleHistoryByLogId(id.ToString(), objectType).ToList();
            return View(list);
        }

        public ActionResult ViewConfigLog(int id)
        {
            var result =  LoggerManager.SelectFlashSaleHistoryDetailByLogId(id)??new FlashSaleProductOprLog();
            return View(result);
        }

        public ActionResult RefreshCache()
        {
            var request = new RefreshCachePrefixRequest()
            {
                Prefix = "TireCreateOrderOptionsConfigPrefix/",
                ClientName = "Config1",
                Expiration = TimeSpan.FromHours(1)
            };
            try
            {
                using (var client = new CacheClient())
                {
                    var result = client.RefreshRedisCachePrefixForCommon(request);
                    if (result.Success && result.Result == true)
                    {
                        return Json(new
                        {
                            status = 1,
                            Msg = "成功"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            status = -1,
                            Msg = "失败"+ result.Exception
                        });
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = -1,
                    Msg = "失败" + e
                });
            }
        }
    }
}