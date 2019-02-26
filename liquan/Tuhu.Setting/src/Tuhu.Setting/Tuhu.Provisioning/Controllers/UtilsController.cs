using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using swc = System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.Framework.FileUpload;

namespace Tuhu.Provisioning.Controllers
{
    public class UtilsController : Controller
    {
        [HttpPost]
        public JsonResult UploadImage(string path, string name, string key)
        {
            int result = 0;
            string imageUrl = string.Empty;
            Exception ex = null;
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var exttension = Path.GetExtension(Imgfile.FileName);
                    name = (name ?? Guid.NewGuid().ToString()) + exttension;
                    imageUrl = path + name;
                    var client = new WcfClinet<IFileUpload>();

                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);

                    result = client.InvokeWcfClinet(w => w.UploadImage(imageUrl, buffer, 0, 0));
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            return Json(new
            {
                Url = imageUrl,
                Name = name,
                Msg = result > 0 ? "上传成功" : ex?.Message,
                Key = key
            });
        }
    }
}