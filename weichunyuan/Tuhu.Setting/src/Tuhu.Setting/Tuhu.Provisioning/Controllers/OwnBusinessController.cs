using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Mapping;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Tuhu.Provisioning.Business.Logger;

namespace Tuhu.Provisioning.Controllers
{
    public class OwnBusinessController : Controller
    {
        // GET: OwnBusiness
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridJson(Pagination pagination)
        {
            RepositoryManager manager = new RepositoryManager();
           var list =  manager.GetEntityList<BusniessListEntity>(pagination);
            return Content(JsonConvert.SerializeObject(new {
                rows = list,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }


        public ActionResult GetGridDeatilJson(int? keyValue)
        {
            if (keyValue == null)
                return Content("null");
            RepositoryManager manager = new RepositoryManager();
            Expression<Func<BusniessListDeatilEntity, bool>> expression = _ => _.FKID == keyValue;
            var list = manager.GetEntityList<BusniessListDeatilEntity>(expression)?.OrderBy(_=>_.OrderBy);
            return Content(JsonConvert.SerializeObject(list));
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult GetFormJson(int keyValue)
        {
            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<BusniessListEntity>(keyValue);
            return Content(JsonConvert.SerializeObject(entity));
        }


        public ActionResult SubmitForm(BusniessListEntity entity)
        {
            RepositoryManager manager = new RepositoryManager();
            entity.UpdateDateTime = DateTime.Now;
            entity.UpdateUserName = User.Identity.Name;
            var beforeEntity = manager.GetEntity<BusniessListEntity>(entity.PKID);
            manager.Update<BusniessListEntity>(entity);
            LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue=JsonConvert.SerializeObject(beforeEntity), AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "更新业务", ObjectType = "OWNBCON" });
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }

        public ActionResult DeleteForm(int? keyValue)
        {
            RepositoryManager manager = new RepositoryManager();
            using (var db = manager.BeginTrans())
            {
                var entity = manager.GetEntity<BusniessListEntity>(keyValue);
                Expression<Func<BusniessListEntity, bool>> exp = _ => _.PKID == keyValue;
                db.Delete<BusniessListEntity>(exp);
                Expression<Func<BusniessListDeatilEntity, bool>> expre = _ => _.FKID == keyValue;
                db.Delete<BusniessListDeatilEntity>(expre);
                db.Commit();
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "删除业务", ObjectType = "OWNBCON" });
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
        }

        public ActionResult SubmitFormCopy(int? keyValue)
        {
            RepositoryManager manager = new RepositoryManager();
           var entity =  manager.GetEntity<BusniessListEntity>(keyValue);
            if (entity != null)
            {
                Expression<Func<BusniessListDeatilEntity, bool>> exp = _ => _.FKID == entity.PKID;
                var list = manager.GetEntityList<BusniessListDeatilEntity>(exp);
                manager.Add<BusniessListEntity>(entity);
                if (list != null)
                {
                    using (var db = manager.BeginTrans())
                    {
                        foreach (var i in list)
                        {
                            i.FKID = entity.PKID;
                            db.Insert<BusniessListDeatilEntity>(i);
                        }
                        db.Commit();
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }

        public ActionResult FormDeatil()
        {
            return View();
        }

        public ActionResult GetFormDeatilJson(int keyValue)
        {
            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<BusniessListDeatilEntity>(keyValue);
            return Content(JsonConvert.SerializeObject(entity));
        }

        public ActionResult SubmitFormDeatil(BusniessListDeatilEntity entity,string keyValue,string Title="",int? OrderBy=null)
        {
            RepositoryManager manager = new RepositoryManager();
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                if (entity.FKID == null || entity.FKID ==0)
                {
                    BusniessListEntity busniess = new BusniessListEntity();
                    busniess.CreateDateTime = DateTime.Now;
                    busniess.CreateUserName = User.Identity.Name;
                    busniess.Title = Title;
                    busniess.OrderBy = OrderBy;
                    manager.Add<BusniessListEntity>(busniess);
                    entity.FKID = busniess.PKID;
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(busniess), Author = User.Identity.Name, Operation = "新增业务", ObjectType = "OWNBCON" });
                }
                entity.CreateDateTime = DateTime.Now;
                entity.CreateUserName = User.Identity.Name;
                manager.Add<BusniessListDeatilEntity>(entity);
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增业务内容", ObjectType = "OWNBCON" });
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = entity.FKID
                }));
            }
            else
            {
                entity.PKID = Convert.ToInt32(keyValue);
                entity.UpdateDateTime = DateTime.Now;
                entity.UpdateUserName = User.Identity.Name;
                var beforeEntity = manager.GetEntity<BusniessListDeatilEntity>(entity.PKID);
                manager.Update<BusniessListDeatilEntity>(entity);
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue=JsonConvert.SerializeObject(beforeEntity), AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "更新业务内容", ObjectType = "OWNBCON" });
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
        }

        public ActionResult DeleteFormDeatil(int? keyValue)
        {
            try
            {
                RepositoryManager manager = new RepositoryManager();
                var entity = manager.GetEntity<BusniessListDeatilEntity>(keyValue);
                Expression<Func<BusniessListDeatilEntity, bool>> exp = _ => _.PKID == keyValue;
                manager.Delete<BusniessListDeatilEntity>(exp);
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "删除业务内容", ObjectType = "OWNBCON" });
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败",
                    data = em.Message
                }));
            }
        }
    }
}