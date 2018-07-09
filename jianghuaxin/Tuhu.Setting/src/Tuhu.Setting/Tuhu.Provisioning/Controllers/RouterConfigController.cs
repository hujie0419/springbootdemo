using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.RouterConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Router;

namespace Tuhu.Provisioning.Controllers
{
    public class RouterConfigController : Controller
    {
        // GET: RouterConfig
        //配置工具的配置功能

        /// <summary>
        /// 查询所有主链接及参数
        /// </summary>
        public ActionResult Index(int linkKind)
        {
            RouterConfigManager manager = new RouterConfigManager();
            if(linkKind==0)
            return View(manager.GetList());
            if (linkKind == 1)
                return View(manager.GetListForApp());
            if (linkKind == 2)
                return View(manager.GetListForWeixin());
            return null;
        }

        public ActionResult EditMainLink(int id)
        {
            if (id == 0)
                return View(new RouterMainLink());
            else
            {
                RouterConfigManager manager = new RouterConfigManager();
                return View(manager.GetMainLink(id));
            }

        }

        public ActionResult EditParameter(int id)
        {
            if (id == 0)
                return View(new RouterParameter());
            else
            {
                
                   RouterConfigManager manager = new RouterConfigManager();
                return View(manager.GetParameter(id));
            }

        }

        public ActionResult EditLink(int id,int mId)
        {
            if (id == 0)
            {
                RouterConfigManager manager = new RouterConfigManager();
                return View(new RouterLink() {MainLink = manager.GetMainLink(mId),RouterParameterList=manager.GetParameterList(mId) });
            }
            else
            {
                RouterConfigManager manager = new RouterConfigManager();
                return View(manager.GetLink(id));
            }

        }

        /// <summary>
        /// 删除选中的主链接
        /// </summary>
        public ActionResult DeleteMainLink(int id)
        {
            RouterConfigManager manager = new RouterConfigManager();
            var before = manager.GetMainLink(id);
            if (manager.DeleteMainLink(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = id.ToString(), ObjectType = "MainLink", Operation = "删除主链接配置-" + before.Discription+"-"+before.Content
 });
                return Json(1);
            }
            
                return Json(0);
        }

        /// <summary>
        /// 删除选中的参数
        /// </summary>
        public ActionResult DeleteParameter(int id)
        {
            RouterConfigManager manager = new RouterConfigManager();
            var before = manager.GetParameter(id);
            if (manager.DeleteParameter(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory()
                {
                    AfterValue = "",
                    Author = User.Identity.Name,
                    BeforeValue = JsonConvert.SerializeObject(before),
                    ChangeDatetime = DateTime.Now,
                    ObjectID = id.ToString(),
                    ObjectType = "Parameter",
                    Operation = "删除参数配置-" + before.Discription + "-" + before.Content
                });
                return Json(1);
            }

            return Json(0);
        }

        /// <summary>
        /// 删除选中的链接关系
        /// </summary>
        public ActionResult DeleteLink(int id)
        {
            RouterConfigManager manager = new RouterConfigManager();
            var beforeLink = manager.GetLink(id);
            if (manager.DeleteLink(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory()
                {
                    AfterValue = "",
                    Author = User.Identity.Name,
                    BeforeValue = JsonConvert.SerializeObject(beforeLink),
                    ChangeDatetime = DateTime.Now,
                    ObjectID = id.ToString(),
                    ObjectType = "Link",
                    Operation = "删除链接关联配置-" + beforeLink.MainLink.Discription + "-" + beforeLink.Parameter.Discription
                });
                return Json(1);
            }

            return Json(0);
        }

        /// <summary>
        /// 保存主链接的新增或修改操作
        /// </summary>
        public ActionResult MainLinkSave(RouterMainLink model)
        {
            RouterConfigManager manger = new RouterConfigManager();
            if (model.PKID == 0)
            {

                if (manger.AddMainLink(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "MainLink", Operation = "新增主链接配置" + model.Content });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                var before = manger.GetMainLink(model.PKID);
                if (manger.UpdateMainLink(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "MainLink", Operation = "编辑主链接配置" + model.Content });
                    return Json(1);
                }
                else
                    return Json(0);
            }
        }

        /// <summary>
        /// 保存参数的新增或修改操作
        /// </summary>
        public ActionResult ParameterSave(RouterParameter model)
        {
            RouterConfigManager manger = new RouterConfigManager();
            if (model.PKID == 0)
            {

                if (manger.AddParameter(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "Parameter", Operation = "新增参数配置" + model.Discription });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                var before = manger.GetParameter(model.PKID);
                if (manger.UpdateParameter(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "Parameter", Operation = "编辑参数配置" + model.Discription });
                    return Json(1);
                }
                else
                    return Json(0);
            }
        }

        /// <summary>
        /// 保存链接关系的新增或修改操作
        /// </summary>
        public ActionResult LinkSave(RouterLink model)
        {
            RouterConfigManager manger = new RouterConfigManager();
            if (model.PKID == 0)
            {

                if (manger.AddLink(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "Link", Operation = "新增链接参数配置,id:" + model.PKID });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                var before = manger.GetLink(model.PKID);
                if (manger.UpdateLink(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "Link", Operation = "编辑链接参数配置,id:" + model.PKID });
                    return Json(1);
                }
                else
                    return Json(0);
            }
        }

        /// <summary>
        /// 查看日志
        /// </summary>
        public ActionResult Logger(string startDateTime = "", string endDateTime = "", string type = "")
        {
            if (string.IsNullOrWhiteSpace(type))
                return View();
            if (string.IsNullOrWhiteSpace(startDateTime))
            {
                startDateTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                endDateTime = DateTime.Now.ToString("yyyy-MM-dd");
            }
            LoggerManager manager = new LoggerManager();
            return View(manager.GetList(type, startDateTime, endDateTime));
        }

        /// <summary>
        /// 查看该主链接关联的参数
        /// </summary>
        public ActionResult List(int id)
        {
            RouterConfigManager manager = new RouterConfigManager();
            if (id != 0)
                return View(manager.GetLinkList(id));
            return null;
        }



    }
}