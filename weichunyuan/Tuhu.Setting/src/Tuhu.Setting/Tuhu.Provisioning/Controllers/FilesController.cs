using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.FileUpload;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Utility.Request;

namespace Tuhu.Provisioning.Controllers
{
    public class FilesController : Controller
    {
        // GET: Files
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadImg()
        {
            var result = new BaoYangResultEntity<Dictionary<string, string>>() { Status = false };
            if (Request.Files.Count <= 0)
            {
                result.Msg = "未找到上传文件";
                return Json(result);
            }
            var imgDic = new Dictionary<string, string>();
            var imgType = new[] { "JPG", "PNG", "GIF", "BMP" };
            foreach (var key in Request.Files.AllKeys)
            {
                Stream stream;
                HttpPostedFileBase file = Request.Files[key];
                if (file == null || file.InputStream.Length <= 0)
                {
                    result.Msg = $"文件没有包含任何数据{file.FileName}";
                    continue;
                }
                var imgName = file.FileName.Split('.');
                if (imgName.Length < 2 || !imgType.Contains(imgName.Last().ToUpper()))
                {
                    result.Msg = $"图片验证失败{file.FileName}";
                    continue;
                }
                byte[] fileByte = new byte[file.InputStream.Length];
                stream = file.InputStream;
                stream.Read(fileByte, 0, fileByte.Length);
                var manager = new FileUploadManager();
                var serviceResult = manager.UploadImg(fileByte);
                if (!imgDic.ContainsKey(file.FileName))
                {
                    imgDic.Add(file.FileName, $"https://img1.tuhu.org{serviceResult}");
                }
            }
            result.Status = true;
            result.Data = imgDic;
            return Json(result);
        }
    }
}