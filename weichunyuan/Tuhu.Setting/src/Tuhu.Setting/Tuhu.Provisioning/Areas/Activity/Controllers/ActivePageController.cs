using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Mapping;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Areas.Activity.Controllers
{
    public class ActivePageController : Controller
    {
        // GET: Activity/ActivePage
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetGridJson(int keyValue)
        {
            Expression<Func<ActivePageHomeEntity, bool>> expression = _ => _.FKActiveID == keyValue;
            RepositoryManager repository = new RepositoryManager();
            var list = repository.GetEntityList<ActivePageHomeEntity>(expression);
            return Content(JsonConvert.SerializeObject(list?.OrderBy(_ => _.Sort)));
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult SubmitForm(ActivePageHomeEntity entity)
        {
            if (entity == null)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "信息为空"
                }));
            }

            try
            {
                RepositoryManager repository = new RepositoryManager();

                if (entity.PKID == 0)
                {
                    Expression<Func<ActivePageHomeEntity, bool>> checkexpression = o => o.FKActiveID == entity.FKActiveID;
                    var result = repository.GetEntityList(checkexpression)?.ToList();
                    if (result.Count >= 6)
                    {
                        throw new Exception("会场数量最多可配置6个");
                    }

                    entity.CreateDateTime = DateTime.Now;
                    repository.Add<ActivePageHomeEntity>(entity);
                    LoggerManager.InsertLog("ActivityPage", new FlashSaleProductOprLog()
                    {
                        OperateUser = User.Identity.Name,
                        CreateDateTime = DateTime.Now,
                        BeforeValue = JsonConvert.SerializeObject(""),
                        AfterValue = JsonConvert.SerializeObject(entity),
                        LogType = "ActivityPage",
                        LogId = entity.FKActiveID.ToString(),
                        Operation = "新增会场",
                    });
                   // LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增会场:" + entity.BigHomeName, ObjectType = "AUActivity", ObjectID = entity.PKID.ToString() });
                }
                else
                {
                    var before = repository.GetEntity<ActivePageHomeEntity>(entity.PKID);
                    entity.UpdateDateTime = DateTime.Now;
                    repository.Update<ActivePageHomeEntity>(entity);
                    LoggerManager.InsertLog("ActivityPage", new FlashSaleProductOprLog()
                    {
                        OperateUser = User.Identity.Name,
                        CreateDateTime = DateTime.Now,
                        BeforeValue = JsonConvert.SerializeObject(before),
                        AfterValue = JsonConvert.SerializeObject(entity),
                        LogType = "ActivityPage",
                        LogId = entity.FKActiveID.ToString(),
                        Operation = "修改会场",
                    });
                   // LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "修改会场:" + entity.BigHomeName, ObjectType = "AUActivity", ObjectID = entity.PKID.ToString() });
                }
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = em.Message
                }));
            }
        }

        public ActionResult Delete(int keyValue)
        {
            RepositoryManager repository = new RepositoryManager();
            Expression<Func<ActivePageHomeEntity, bool>> expresion = _ => _.PKID == keyValue;
            try
            {
                repository.Delete<ActivePageHomeEntity>(expresion);
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = em.Message
                }));
            }
        }

        public ActionResult GetFormJson(int keyValue)
        {
            if (keyValue == 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "信息为空"
                }));
            RepositoryManager repository = new RepositoryManager();
            var entity = repository.GetEntity<ActivePageHomeEntity>(keyValue);
            return Content(JsonConvert.SerializeObject(entity));
        }

        public ActionResult ActivePageDeatil()
        {
            return View();
        }

        public ActionResult GetActivePageDeatilJson(int fkActivePage)
        {
            if (fkActivePage == 0)
                return Content("null");

            Expression<Func<ActivePageHomeDeatilEntity, bool>> expression = _ => _.FKActiveHome == fkActivePage;
            RepositoryManager repository = new RepositoryManager();

            var list = repository.GetEntityList<ActivePageHomeDeatilEntity>(expression);
            return Content(JsonConvert.SerializeObject(list?.OrderBy(_ => _.BigFHomeOrder)));
        }

        public ActionResult SubmitHomeDeatil(string json, int fkActivePage)
        {
            if (string.IsNullOrWhiteSpace(json))
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "信息为空"
                }));

            var list = JsonConvert.DeserializeObject<IEnumerable<ActivePageHomeDeatilEntity>>(json.Replace("\"&nbsp;\"", "null"));
            if (list == null || list?.Count() == 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "信息为空"
                }));
            try
            {
                RepositoryManager repository = new RepositoryManager();
                IEnumerable<ActivePageHomeDeatilEntity> beforeList = null;
                using (var db = repository.BeginTrans())
                {
                    Expression<Func<ActivePageHomeDeatilEntity, bool>> expression = _ => _.FKActiveHome == fkActivePage;
                    beforeList = repository.GetEntityList<ActivePageHomeDeatilEntity>(expression);
                    db.Delete<ActivePageHomeDeatilEntity>(expression);
                    foreach (var item in list)
                    {
                        item.CreateDateTime = DateTime.Now;
                        item.FKActiveHome = fkActivePage;
                        db.Insert<ActivePageHomeDeatilEntity>(item);
                    }
                    db.Commit();
                    LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(beforeList), AfterValue = JsonConvert.SerializeObject(list), Author = User.Identity.Name, Operation = "会场明细", ObjectType = "AUCAct" });
                }
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = em.Message
                }));
            }

        }

        public ActionResult GetBackgroundColor(int keyValue)
        {
            Expression<Func<ActivePageHomeEntity, bool>> expression = _ => _.FKActiveID == keyValue;
            RepositoryManager repository = new RepositoryManager();
            var list = repository.GetEntityList<ActivePageHomeEntity>(expression);
            return Content(list?.OrderBy(_ => _.Sort)?.FirstOrDefault(x => !string.IsNullOrEmpty(x?.BackgroundColor))?.BackgroundColor);
        }
        public ActionResult SetBackgroundColor(int keyValue, string color)
        {
            try
            {
                Expression<Func<ActivePageHomeEntity, bool>> expression = _ => _.FKActiveID == keyValue;
                RepositoryManager repository = new RepositoryManager();
                var list = repository.GetEntityList<ActivePageHomeEntity>(expression);
                if (list != null && list.Any())
                {
                    using (var db = repository.BeginTrans())
                    {
                        foreach (var item in list)
                        {
                            item.BackgroundColor = color;
                            db.Update(item);
                        }
                        db.Commit();
                    }
                }
                return Content("1");
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message);
            }
        }

    }
}