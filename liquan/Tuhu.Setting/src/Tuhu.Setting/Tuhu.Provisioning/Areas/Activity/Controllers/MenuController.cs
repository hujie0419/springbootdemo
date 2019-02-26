using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Provisioning.Business;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Areas.Activity.Controllers
{
    public class MenuController : Controller
    {
        // GET: Activity/Menu
        public ActionResult MenuForm()
        {
            return View();
        }

        /// <summary>
        /// 返回菜单选择页面
        /// </summary>
        /// <returns></returns>

        public ActionResult SelectedActiveContentList()
        {
            return View();
        }

        public ActionResult Submit(string json, string group, int? fkActivityID, int pkid = 0, int? MenuType = 0)
        {
            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(group) || fkActivityID == null || fkActivityID == 0)
                return Content("null");

            try
            {
                RepositoryManager repository = new RepositoryManager();
                ActivePageContentEntity content = new ActivePageContentEntity();
                try
                {
                    using (var db = repository.BeginTrans())
                    {
                        var activityEntity = db.FindEntity<ActivePageListEntity>(fkActivityID);
                        activityEntity.MenuType = MenuType;
                        db.Update<ActivePageListEntity>(activityEntity);
                        IEnumerable<ActivePageMenuEntity> beforeList = null;
                        if (pkid != 0)
                        {
                            Expression<Func<ActivePageMenuEntity, bool>> expression = _ => _.FKActiveContentID == pkid;
                            beforeList = repository.GetEntityList<ActivePageMenuEntity>(expression);
                            db.Delete<ActivePageMenuEntity>(expression);
                        }
                        else
                        {
                            content.FKActiveID = fkActivityID;
                            content.GROUP = group;
                            content.CreateDateTime = DateTime.Now;
                            content.Type = -2;
                            content.RowType = 0;
                            content.OrderBy = 1;
                            content.IsUploading = false;
                            content.Channel = "wap";
                            repository.Add<ActivePageContentEntity>(content);
                        }
                        var list = JsonConvert.DeserializeObject<IEnumerable<ActivePageMenuEntity>>(json);
                        foreach (var item in list)
                        {
                            item.FKActiveContentID = content.PKID == 0 ? pkid : content.PKID;
                            item.CreateDateTime = DateTime.Now;
                            item.UpdateDateTime = DateTime.Now;
                            db.Insert<ActivePageMenuEntity>(item);
                        }
                        db.Commit();
                        LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(beforeList), AfterValue = JsonConvert.SerializeObject(list), Author = User.Identity.Name, Operation = "菜单配置", ObjectType = "AUCAct" });
                    }
                }
                catch (Exception em)
                {
                    Expression<Func<ActivePageContentEntity, bool>> expre = _ => _.PKID == content.PKID;
                    repository.Delete<ActivePageContentEntity>(expre);
                    throw new Exception("保存菜单明细错误");
                }

                ActivityUnityController activity = new ActivityUnityController();
                activity.RefreshSource(fkActivityID); //刷新活动页缓存

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

        public ActionResult Delete(string group, int pkid)
        {
            if (string.IsNullOrWhiteSpace(group) || pkid == 0)
                return Content("null");
            try
            {
                RepositoryManager repository = new RepositoryManager();
                using (var db = repository.BeginTrans())
                {
                    Expression<Func<ActivePageContentEntity, bool>> expression = _ => _.GROUP == group;
                    db.Delete<ActivePageContentEntity>(expression);

                    Expression<Func<ActivePageMenuEntity, bool>> expre = _ => _.FKActiveContentID == pkid;
                    db.Delete<ActivePageMenuEntity>(expre);
                    db.Commit();
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

        public ActionResult GetMenuList(int pkid)
        {
            if (pkid == 0)
                return Content("null");
            RepositoryManager manager = new RepositoryManager();
            Expression<Func<ActivePageMenuEntity, bool>> expression = _ => _.FKActiveContentID == pkid;
            var list = manager.GetEntityList<ActivePageMenuEntity>(expression);

            if (list == null || list?.Count() == 0)
                return Content("null");
            return Content(JsonConvert.SerializeObject(list?.OrderBy(_ => _.Sort)));
        }

    }
}