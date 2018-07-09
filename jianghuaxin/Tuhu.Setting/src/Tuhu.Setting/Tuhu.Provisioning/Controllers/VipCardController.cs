using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Activity;

namespace Tuhu.Provisioning.Controllers
{
    public class VipCardController : Controller
    {
        [HttpGet]
        public JsonResult GetVipCardList(int clientId, int pageIndex, int pageSize)
        {
            var result = VipCardManager.GetVipCardSaleList(pageIndex, pageSize, clientId);
            return Json(new
            {
                result.Source,
                result.Pager
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllClients()
        {
            var result = VipCardManager.GetAllClients();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetBatchesByClientId(int clientId)
        {
            var result = VipCardManager.GetBatchesByClientId(clientId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save(string data, string activityName, int clientId)
        {
            var models = JsonConvert.DeserializeObject<List<VipCardDetailModel>>(data);
            var result = VipCardManager.InsertVipCardModelAndDetails(models, activityName, clientId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEditDatasByActivityId(string activityId, int clientId)
        {
            var result = VipCardManager.GetVipCardDetailForEdit(activityId, clientId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Edit(string data, string activityName, string activityId, int clientId)
        {
            var models = JsonConvert.DeserializeObject<List<VipCardDetailModel>>(data);
            var result = VipCardManager.UpdateVipCardModelAndDetails(models, activityName, activityId, clientId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLogList(string activityId)
        {
            if (string.IsNullOrEmpty(activityId))
                return Json(null);
            var result = LoggerManager.SelectFlashSaleHistoryByLogId(activityId, "VipCard");
            var log = result.Select(r =>
            {
                switch (r.Operation)
                {
                    case "Add":
                        r.Operation = "上架活动";
                        r.Remark = $"上架活动{activityId}";
                        return r;
                    case "EditEdit":
                        r.Operation = "修改活动名称";
                        r.Remark = $"活动名称{r.BeforeValue}修改为{r.AfterValue}";
                        return r;
                    case "EditDelete":
                        r.Operation = "删除批次";
                        r.Remark = $"删除的批次有{r.BeforeValue}";
                        return r;
                    case "EditAdd":
                        r.Operation = "增加批次";
                        r.Remark = $"增加的批次有{r.AfterValue}";
                        return r;
                    default:
                        return r;
                }

            });
            return Json(log.Select(r => new
            {
                Title = r.Operation,
                Name = r.OperateUser,
                CreateDateTime = r.CreateDateTime,
                Remark = r.Remark
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshCache(string activityId)
        {
            using (var client = new CacheClient())
            {
                var result = client.RefreshVipCardCacheByActivityId(activityId);
                return Json(result.Result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}