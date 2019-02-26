using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Mapping;
using System.Linq.Expressions;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Areas.Activity.Controllers
{
    public class TrieSizeController : Controller
    {
        // GET: Activity/TrieSize
        public ActionResult Form()
        {
            return View();
        }

        public ActionResult SubmitForm(ActivePageTireSizeConfigEntity entity,int? IsTireSize=null)
        {
            if (entity == null || entity?.FKActiveID == 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "信息为空"
                }));

            RepositoryManager repository = new RepositoryManager();
            try
            {
                var activityEntity = repository.GetEntity<ActivePageListEntity>(entity.FKActiveID);
                if (activityEntity != null)
                {
                    activityEntity.IsTireSize = IsTireSize;
                    repository.Update<ActivePageListEntity>(activityEntity);
                }
                if (entity.PKID == 0)
                {
                    entity.CreateDateTime = DateTime.Now;
                    repository.Add<ActivePageTireSizeConfigEntity>(entity);
                    LoggerManager.InsertLog("ActivityPage", new FlashSaleProductOprLog()
                    {
                        OperateUser = User.Identity.Name,
                        CreateDateTime = DateTime.Now,
                        BeforeValue = JsonConvert.SerializeObject(""),
                        AfterValue = JsonConvert.SerializeObject(entity),
                        LogType = "ActivityPage",
                        LogId = entity.FKActiveID.ToString(),
                        Operation = "新增轮胎适配",
                    });
                   // LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增轮胎适配:"+entity.FKActiveID, ObjectType = "AUCAct", ObjectID = entity.PKID.ToString() });
                }
                else
                {
                    var before = repository.GetEntity<ActivePageTireSizeConfigEntity>(entity.PKID);
                    entity.UpdateDateTime = DateTime.Now;
                    repository.Update<ActivePageTireSizeConfigEntity>(entity);
                    LoggerManager.InsertLog("ActivityPage", new FlashSaleProductOprLog()
                    {
                        OperateUser = User.Identity.Name,
                        CreateDateTime = DateTime.Now,
                        BeforeValue = JsonConvert.SerializeObject(before),
                        AfterValue = JsonConvert.SerializeObject(entity),
                        LogType = "ActivityPage",
                        LogId = entity.FKActiveID.ToString(),
                        Operation = "修改轮胎适配",
                    });
                    //LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue=JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "修改轮胎适配:" + entity.FKActiveID, ObjectType = "AUCAct", ObjectID = entity.PKID.ToString() });

                }

                ActivityUnityController activity = new ActivityUnityController();
                activity.RefreshSource(entity.FKActiveID); //刷新活动页缓存

                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"
                }));
            }
            catch ( Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = JsonConvert.SerializeObject(em)
                }));
            }

        }

        public ActionResult GetEntityJson(int pkid)
        {
            if (pkid == 0)
                return Content("null");
            RepositoryManager repository = new RepositoryManager();
            Expression<Func<ActivePageTireSizeConfigEntity, bool>> expression = _ => _.FKActiveID == pkid;
            var entity = repository.GetEntity<ActivePageTireSizeConfigEntity>(expression);
            return Content(JsonConvert.SerializeObject(entity));
        }

    }
}