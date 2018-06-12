using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Activity;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.Logger;
using Newtonsoft.Json.Linq;

namespace Tuhu.Provisioning.Controllers
{
    public class SimpleActivityController : Controller
    {
        //
        // GET: /SimpleActivity/

        public ActionResult ActivityList(int pageIndex = 1, int pageSize = 50, string ActivityName = "", int? SelActivityID = null)
        {
            int count = 0;
            string strSql = string.Empty;

            if (ActivityName.Length > 0)
            {
                strSql = " and  M.Title LIKE N'%" + ActivityName + "%'";
            }

            if (SelActivityID.HasValue)
            {
                strSql += " and M.ID like '%" + SelActivityID.Value + "%'";
            }

            var model = SE_ActivityManager.GetList(strSql, pageSize, pageIndex, out count);

            ViewBag.Total = count;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageIndex;
            return this.View(model);
        }

        public ActionResult ActivityEdit(string id = "")
        {
            SE_Activity model = new SE_Activity();
            if (!string.IsNullOrWhiteSpace(id))
            {
                model = SE_ActivityManager.GetEntity(id);

            }
            else
            {
                //  model.Content = "{total:0,rows:[]}";
            }
            return View(model);
        }


        public ActionResult Save(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                SE_Activity model = JsonConvert.DeserializeObject<SE_Activity>(data);
                SE_Activity preModel = null;
                if (model.ID.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    preModel = SE_ActivityManager.GetEntity(model.ID.ToString());
                }
                if (SE_ActivityManager.Save(model) == 1)
                {
                    JObject json = JObject.Parse(data);
                    JToken token = null;
                    if (!json.TryGetValue("ID",out token))
                    {
                        LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, Operation = "新增活动:" + model.Title, ObjectType = "SPACF" });
                    }
                    else
                    {
                        LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(preModel), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, Operation = "修改活动:" + model.Title, ObjectType = "SPACF" });
                    }
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
                return Json(0);

        }

        [ValidateInput(false)]
        public ActionResult Edit(string rows = "")
        {
            SE_ActivityDeatil model = JsonConvert.DeserializeObject<SE_ActivityDeatil>(rows);
            return View(model);
        }


        public ActionResult Delete(string id)
        {
            return Json(SE_ActivityManager.Delete(id));
        }

        public ActionResult GetLogger(string id)
        {
            ConfigHistory model = LoggerManager.GetConfigHistory(id);
            if (model?.ObjectType.Trim().ToLower() == "SPACF".ToLower())
            {
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
            // return Json(model,JsonRequestBehavior.AllowGet);

        }

    }
}
