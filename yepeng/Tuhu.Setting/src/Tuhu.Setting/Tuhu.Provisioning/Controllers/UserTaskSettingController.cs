using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.UserTaskSetting;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Member;

namespace Tuhu.Provisioning.Controllers
{
    public class UserTaskSettingController : Controller
    {
        // GET: UserTaskSetting
        public ActionResult Index()
        {
            List<UserTaskSettingModel> result = new List<UserTaskSettingModel>();
            var temp = UserTaskSettingManager.GetUserTaskSettingList(1, 30);
            return View(temp);
        }

        public ActionResult AddOrEditUserTaskSetting(int PKID, string Type)
        {
            var model = new UserTaskSettingModel();
            if (Type == "Update")
            {
                model = UserTaskSettingManager.GetUserTaskSetting(PKID);
            }
            var ConditionList = UserTaskSettingManager.GetTaskCompleteCondition();
            return View(Tuple.Create(model, ConditionList));
        }
        public ActionResult SaveUserTaskSetting(UserTaskSettingModel model)
        {
            var result = UserTaskSettingManager.SaveTaskSetting(model);
            if (result.Item1)
            {
                CleanTaskCache();
            }
            return Json(result);
        }
        public ActionResult UploadTaskImage()
        {
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength < 200 * 1024)
            {
                var file = Request.Files[0];
                string fileExtension = System.IO.Path.GetExtension(Request.Files[0].FileName);
                string[] allowExtension = { ".jpg", ".gif", ".png", ".jpeg" };
                var flag = false;
                //对上传的文件的类型进行一个个匹对
                for (int i = 0; i < allowExtension.Length; i++)
                {
                    if (fileExtension.ToLower() == allowExtension[i])
                    {
                        flag = true;
                    }
                }
                if (!flag)
                    return Content("-1");//仅支持"jpg", "gif", "png", "jpeg"
                try
                {
                    var buffers = new byte[file.ContentLength];
                    file.InputStream.Read(buffers, 0, file.ContentLength);
                    var temp = buffers.UpdateLoadImage();
                    if (temp.Item1)
                    {
                        return Content(temp.Item2);
                    }
                    return Content("-98");//调用上传服务异常
                }
                catch (Exception ex)
                {
                    return Content("-99");//服务器异常
                }
            }
            return Content("-3");//图片太大


        }
        private bool CleanTaskCache()
        {
            using (var client = new UserShareClient())
            {
                var result = client.CleanTaskCache(new Service.Member.Request.CleanTaskRequest { });
                return result.Result;
            }
        }
    }
}