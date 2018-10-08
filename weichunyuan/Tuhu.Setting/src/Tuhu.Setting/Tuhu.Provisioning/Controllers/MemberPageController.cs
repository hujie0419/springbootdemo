using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.MemberPage;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity.MemberPage;
using Tuhu.Service.ConfigLog;

namespace Tuhu.Provisioning.Controllers
{
    public class MemberPageController : Controller
    {
        // GET: MemberPage
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取模块和模块内容列表
        /// </summary>
        /// <param name="pageCode">页面编码，member=个人中心，more=更多</param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        [HttpGet]
        public JsonResult GetMemberPageList(string pageCode="member")
        {
            var manager = new MemberPageModuleManager();
            var list = manager.GetMemberPageModuleList(pageCode);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取模块或模块内容详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMemberPageModuleInfo(long contentId, long moduleId, int moduleType)
        {
            var contentManager = new MemberPageModuleContentManager();
            var moduleManager = new MemberPageModuleManager();
            if (moduleType == 0)
            {
                //模块内容信息
                var contentInfo = contentManager.GetMemberPageModuleContentInfo(contentId);
                return Json(contentInfo, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //模块信息
                var moduleInfo = moduleManager.GetMemberPageModuleInfo(moduleId);
                return Json(moduleInfo, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 新增/编辑模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddMemberPageModule(MemberPageModuleModel model)
        {
            bool flag = false;
            var memberPageManager = new MemberPageManager();
            var moduleManager = new MemberPageModuleManager();
            if (model.PKID == 0)//新建
            {
                var memberPageInfo = memberPageManager.GetMemberPageByPageCode(model.PageCode);
                if (memberPageInfo != null)
                    model.MemberPageID = memberPageInfo.PKID;
                model.Creator = User.Identity.Name;
                flag = moduleManager.AddMemberPageModule(model);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "MemberPageModule",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = "创建",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            else//编辑
            {
                var moduleInfo = moduleManager.GetMemberPageModuleInfo(model.PKID);
                flag = moduleManager.UpdateMemberPageModule(model);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "MemberPageModule",
                        BeforeValue = JsonConvert.SerializeObject(moduleInfo),
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = "编辑",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            moduleManager.RefreshMemberPageCache();
            return Json(flag);
        }
        /// <summary>
        /// 新增/编辑模块内容信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddMemberPageModuleContent(MemberPageModuleContentModel model)
        {
            #region 数据验证
            if (!string.IsNullOrEmpty(model.StartVersion))
            {
                Version startVersion;
                if (!Version.TryParse(model.StartVersion, out startVersion))
                    return Json(new { status = false, msg = "APP开始版本的类型有误" }, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrEmpty(model.EndVersion))
            {
                Version endVersion;
                if (!Version.TryParse(model.EndVersion, out endVersion))
                    return Json(new { status = false, msg = "APP结束版本的类型有误" }, JsonRequestBehavior.AllowGet);
            }
            if (model.ChannelList != null)
            {
                foreach (var item in model.ChannelList)
                {
                    if (string.IsNullOrWhiteSpace(item.Link))
                        return Json(new { status = false, msg = $"请输入{item.Channel}渠道的跳转链接" }, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion
            bool flag = false;
            var memberPageManager = new MemberPageManager();
            var moduleManager = new MemberPageModuleManager();
            var contentManager = new MemberPageModuleContentManager();
            var channelManager = new MemberPageChannelManager();

            if (model.ImageUrl == "static/default.jpg")
                model.ImageUrl = null;

            if (model.PKID == 0)//新建
            {
                var memberPageInfo = memberPageManager.GetMemberPageByPageCode(model.PageCode);
                if (memberPageInfo != null)
                    model.MemberPageID = memberPageInfo.PKID;
                model.Creator = User.Identity.Name;
                flag = contentManager.AddMemberPageModuleContent(model);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "MemberPageModuleContent",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = "创建",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            else//编辑
            {
                var contentInfo = contentManager.GetMemberPageModuleContentInfo(model.PKID);
                flag = contentManager.UpdateMemberPageModuleContent(model);
                channelManager.DeleteMemberPageChannel(model.PKID);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "MemberPageModuleContent",
                        BeforeValue = JsonConvert.SerializeObject(contentInfo),
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = "编辑",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            //重新保存渠道信息
            if (model.ChannelList != null)
            {
                foreach (var item in model.ChannelList)
                {
                    if (item.MemberPageModuleContentID == 0)
                        item.MemberPageModuleContentID = model.PKID;
                    channelManager.AddMemberPageChannel(item);
                }
            }

            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            moduleManager.RefreshMemberPageCache();

            return Json(new { status = flag, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除模块或模块内容
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="moduleId"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public JsonResult DeleteMemberPageModule(long contentId, long moduleId, int moduleType)
        {
            bool flag = false;
            var channelManager = new MemberPageChannelManager();
            var contentManager = new MemberPageModuleContentManager();
            var moduleManager = new MemberPageModuleManager();
            if (moduleType == 0)//模块内容
            {
                channelManager.DeleteMemberPageChannel(contentId);
                flag = contentManager.DeleteMemberPageModuleContent(contentId);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = contentId,
                        ObjectType = "MemberPageModuleContent",
                        BeforeValue = "",
                        AfterValue = "",
                        Remark = "删除",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            else//模块
            {
                channelManager.DeleteMemberPageChannelByModuleID(moduleId);
                contentManager.DeleteMemberPageModuleContentByModuleID(moduleId);
                flag = moduleManager.DeleteMemberPageModule(moduleId);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = moduleId,
                        ObjectType = "MemberPageModule",
                        BeforeValue = "",
                        AfterValue = "",
                        Remark = "删除",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            moduleManager.RefreshMemberPageCache();
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult UploadImage(string type)
        {
            var result = false;
            var msg = string.Empty;
            var imageUrl = string.Empty;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string fileExtension = System.IO.Path.GetExtension(Request.Files[0].FileName);
                string[] allowExtension = { ".jpg", ".gif", ".png", ".jpeg" };
                if (allowExtension.Contains(fileExtension.ToLower()))
                {
                    var buffers = new byte[file.ContentLength];
                    file.InputStream.Read(buffers, 0, file.ContentLength);
                    var upLoadResult = buffers.UpdateLoadImage();
                    if (upLoadResult.Item1)
                    {
                        result = true;
                        imageUrl = upLoadResult.Item2;
                    }
                }
                else
                {
                    msg = "图片格式不对";
                }
            }
            else
            {
                msg = "请选择文件";
            }

            return Json(new { Status = result, ImageUrl = imageUrl, Msg = msg, Type = type });
        }
    }
}