using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.AllSort;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.AllSort;

namespace Tuhu.Provisioning.Controllers
{
    public class AllSortController : Controller
    {
        /// <summary>
        /// 查询所有全品类分类栏目
        /// </summary>
        [PowerManage]
        public ActionResult Index()
        {
            AllSortManager manager = new AllSortManager();
            return View(manager.GetList());
        }

        
        /// <summary>
        /// 进入新增或修改全品类分类栏目页面
        /// </summary>
        public ActionResult EditConfig(int id)
        {

            if(id==0)
                return View(new AllSortConfig());
            else
            {
                AllSortManager manager = new AllSortManager();
                return View(manager.GetEntity(id));
            }
        }
        /// <summary>
        /// 逻辑删除选中的全品类分类
        /// </summary>
        public ActionResult DeleteConfig(int id)
        {
            AllSortManager manager = new AllSortManager();
            var before = manager.GetEntity(id);
            if (manager.DeleteEntity(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = id.ToString(), ObjectType = "AllSort", Operation = "逻辑删除全品类分类配置" + before.Maintitle});
                return Json(1);
            }
            else
                return Json(0);
        }
        /// <summary>
        /// 保存全品类分类栏目的新增或修改操作
        /// </summary>
        public ActionResult ConfigSave(AllSortConfig model)
        {
            AllSortManager manger = new AllSortManager();
            if (model.PKID== 0)
            {
                
                if (manger.AddConfig(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "AllSort", Operation = "新增全品类分类配置" + model.Maintitle });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                var before = manger.GetEntity(model.PKID);
                if (manger.UpdateConfig(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "AllSort", Operation = "编辑全品类分类配置" + model.Maintitle });
                    return Json(1);
                }
                else
                    return Json(0);
            }
        }

        
        /// <summary>
        /// 查看全品类分类详情
        /// </summary>
        public ActionResult List(int id)
        {
            AllSortManager manager = new AllSortManager();
            if (id != 0)
                return View(manager.GetEntity(id));
             return null;
        }


        
        /// <summary>
        /// 进入新增或修改标签栏的页面
        /// </summary>
        public ActionResult EditTagConfig(int id,int parentId,String parentTitle)
        {

            if (id == 0)
                return View(new AllSortTagConfig() { ParentId = parentId,ParentMaintitle= parentTitle });
            else
            {
                AllSortManager manager = new AllSortManager();
                return View(manager.GetTagEntity(id, parentTitle));
            }
        }
        /// <summary>
        /// 逻辑删除选中的标签栏
        /// </summary>
        public ActionResult DeleteTagConfig(int id)
        {
            AllSortManager manager = new AllSortManager();
            var title = "";
            var before = manager.GetTagEntity(id,title);
            if (manager.DeleteTagEntity(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = id.ToString(), ObjectType = "SortTag", Operation = "逻辑删除标签栏配置" + before.Maintitle });
                return Json(1);
            }
            else
                return Json(0);
        }
        /// <summary>
        /// 保存标签栏的新增或修改操作
        /// </summary>
        public ActionResult ConfigTagSave(AllSortTagConfig model)
        {
            AllSortManager manger = new AllSortManager();
            if (model.PKID == 0)
            {

                if (manger.AddTagConfig(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "SortTag", Operation = "新增标签栏配置" + model.Maintitle });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                var title = "";
                var before = manger.GetTagEntity(model.PKID,title);
                if (manger.UpdateTagConfig(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "SortTag", Operation = "编辑标签栏配置" + model.Maintitle });
                    return Json(1);
                }
                else
                    return Json(0);
            }
        }


       
        /// <summary>
        /// 进入新增或修改的内容栏页面
        /// </summary>
        public ActionResult EditListConfig(int id, int parentId,String parentTitle)
        {

            if (id == 0)
                return View(new AllSortListConfig() {
                    ParentId = parentId,
                    ParentMaintitle = parentTitle,
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    PKID = 0,
                    TitleColor = "#ffffff",
                    TitleBgColor = "#000000"
                });
            else
            {
                AllSortManager manager = new AllSortManager();
                return View(manager.GetListEntity(id, parentTitle));
            }
        }
        /// <summary>
        /// 逻辑删除选中的内容栏
        /// </summary>
        public ActionResult DeleteListConfig(int id)
        {
            AllSortManager manager = new AllSortManager();
            var title = "";
            var before = manager.GetListEntity(id,title);
            if (manager.DeleteListEntity(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = id.ToString(), ObjectType = "SortList", Operation = "逻辑删除内容栏配置" + before.Title });
                return Json(1);
            }
            else
                return Json(0);
        }
        /// <summary>
        /// 保存内容栏的新增或修改操作
        /// </summary>
        public ActionResult ConfigListSave(AllSortListConfig model)
        {
            AllSortManager manger = new AllSortManager();
            if (model.PKID == 0)
            {

                if (manger.AddListConfig(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "SortList", Operation = "新增内容栏配置" + model.Title });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                var title = "";
                var before = manger.GetListEntity(model.PKID,title);
                if (manger.UpdateListConfig(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.PKID.ToString(), ObjectType = "SortList", Operation = "编辑内容栏配置" + model.Title });
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
        /// 刷新缓存
        /// </summary>
        public async Task<ActionResult> Reload()
        {
            using (var client = new Tuhu.Service.Config.HomePageClient())
            {
                var result = await client.RefreshAllCarProductListInfoAsync();
                return result.Success ? Content("1") : Content("0");
            }
        }



    }

}