using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Controllers
{
    public class RecommendGetGiftConfigController : Controller
    {
        private static readonly Lazy<RecommendGetGiftConfigManager> lazy = new Lazy<RecommendGetGiftConfigManager>();

        private static readonly Lazy<OprLogManager> lazyOprLog = new Lazy<OprLogManager>();

        private RecommendGetGiftConfigManager RecommendGetGiftConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        private OprLogManager OprLogManager
        {
            get
            {
                return lazyOprLog.Value;
            }
        }

        public ActionResult Index(RecommendGetGiftConfig model, int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
            string strSql = string.Empty;
            var lists = RecommendGetGiftConfigManager.GetRecommendGetGiftConfigList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<RecommendGetGiftConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<RecommendGetGiftConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new RecommendGetGiftConfig());
            }
            else
            {
                RecommendGetGiftConfig model = RecommendGetGiftConfigManager.GetRecommendGetGiftConfigById(id);            

                return View(model);
            }
        }

        public ActionResult Share(int id = 0)
        {
            if (id == 0)
            {
                return View(new RecommendGetGiftConfig());
            }
            else
            {
                RecommendGetGiftConfig model = RecommendGetGiftConfigManager.GetRecommendGetGiftConfigById(id);
                model.Channel = JsonConvert.DeserializeObject<List<ShareChannel>>(model.ShareChannel);               
                return View(model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string data)
        {
            RecommendGetGiftConfig model = JsonConvert.DeserializeObject<RecommendGetGiftConfig>(data);

            if (model.Id != 0)
            {             
                model.UpdateName = User.Identity.Name;
                if (RecommendGetGiftConfigManager.UpdateRecommendGetGiftConfig(model))
                {
                    AddOprLog(model, "修改");
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            else
            {              
                int newid = 0;
                model.CreateName = User.Identity.Name;
                if (RecommendGetGiftConfigManager.InsertRecommendGetGiftConfig(model, ref newid))
                {
                    model.Id = newid;
                    AddOprLog(model, "添加");
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool result = false;
            RecommendGetGiftConfig model = new RecommendGetGiftConfig();
            model.Id = id;

            if (RecommendGetGiftConfigManager.DeleteRecommendGetGiftConfig(id))
            {
                AddOprLog(model, "删除");
                result = true;
            }
            return Json(result);
        }


        public ActionResult LogList(int id)
        {
            var result = LoggerManager.SelectOprLogByParams("SERGG", id.ToString());
            return View(result);
        }


        public void AddOprLog(RecommendGetGiftConfig model, string opr)
        {
            OprLog oprModel = new OprLog();
            oprModel.Author = User.Identity.Name;
            oprModel.ChangeDatetime = DateTime.Now;
            oprModel.HostName = Request.UserHostName;
            oprModel.ObjectID = model.Id;
            oprModel.ObjectType = "SERGG";
            oprModel.Operation = opr;
            OprLogManager.AddOprLog(oprModel);
        }

        [HttpPost]
        public ActionResult GetCoupon(string guidOrId)
        {
            return JavaScript(RecommendGetGiftConfigManager.GetCoupon(guidOrId));
        }
    }
}
