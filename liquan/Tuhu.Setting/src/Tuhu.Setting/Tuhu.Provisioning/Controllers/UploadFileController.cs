using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Configuration;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class UploadFileController : Controller
    {
        // GET: UploadFile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadFileByZip()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var extension = file.FileName.Substring(file.FileName.LastIndexOf('.'), 4);
                if (extension.ToLower() != ".zip")
                {
                    dic.Add("Code",0);
                    dic.Add("Msg","请上传.zip格式的文件");
                    return Content(JsonConvert.SerializeObject(dic));
                }

                var buffer = new byte[file.ContentLength];
                file.InputStream.Read(buffer, 0, buffer.Length);
                using (var client = new Tuhu.Service.Utility.FileUploadClient())
                {
                  var result =  client.UploadFile(new Service.Utility.Request.FileUploadRequest()
                    {
                         Contents=buffer,
                         DirectoryName=ConfigurationManager.AppSettings["UploadDoMain_file"].ToString(),
                         Extension=".zip"
                    });
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        dic.Add("Code",1);
                        dic.Add("Zip", ConfigurationManager.AppSettings["DoMain_image"].ToString() + result.Result);
                        dic.Add("Md5", UtilityHelper.GetMD5HashFromFile(buffer));
                        buffer = null;
                        return Content(JsonConvert.SerializeObject(dic));
                       
                    }
                    else
                    {
                        dic.Add("Code",0);
                        dic.Add("Exception",result.Exception);
                        dic.Add("Msg", "上传失败");
                        return Content(JsonConvert.SerializeObject(dic));
                    }
                }

            }
            else
            {
                dic.Add("Code",0);
                dic.Add("Msg","请选择要上传的zip文件");
            }
            return Content("");
        }


        [PowerManage]
        public ActionResult UploadFile()
        {
            return View();
        }

       
        public ActionResult UploadFileByApi()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var extension = file.FileName.Substring(file.FileName.LastIndexOf('.'), 4);
                var buffer = new byte[file.ContentLength];
                file.InputStream.Read(buffer, 0, buffer.Length);
                using (var client = new Tuhu.Service.Utility.FileUploadClient())
                {
                    var result = client.UploadFile(new Service.Utility.Request.FileUploadRequest()
                    {
                        Contents = buffer,
                        DirectoryName = ConfigurationManager.AppSettings["UploadDoMain_pdf"].ToString(),
                        Extension = extension.ToLower()
                    });
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = result.Result, ChangeDatetime = DateTime.Now, ObjectType = "FileUp", Operation = "上传文件记录", Author=User.Identity.Name });
                        dic.Add("Code", 1);
                        dic.Add("File", ConfigurationManager.AppSettings["DoMain_image"].ToString() + result.Result);
                        dic.Add("Md5", UtilityHelper.GetMD5HashFromFile(buffer));
                        buffer = null;
                        return Content(JsonConvert.SerializeObject(dic));

                    }
                    else
                    {
                        dic.Add("Code", 0);
                        dic.Add("Exception", result.Exception);
                        dic.Add("Msg", "上传失败");
                        return Content(JsonConvert.SerializeObject(dic));
                    }
                }

            }
            else
            {
                dic.Add("Code", 0);
                dic.Add("Msg", "请选择要上传的文件");
            }
            return Content("");
        }

        public ActionResult UploadFileByParam(string directoryName)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var extension = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                var buffer = new byte[file.ContentLength];
                file.InputStream.Read(buffer, 0, buffer.Length);
                using (var client = new Tuhu.Service.Utility.FileUploadClient())
                {
                    var result = client.UploadFile(new Service.Utility.Request.FileUploadRequest()
                    {
                        Contents = buffer,
                        DirectoryName = directoryName,
                        Extension = extension.ToLower()
                    });
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = result.Result, ChangeDatetime = DateTime.Now, ObjectType = "FileUp", Operation = "上传文件记录", Author = User.Identity.Name });
                        dic.Add("Code", 1);
                        dic.Add("File", ConfigurationManager.AppSettings["DoMain_image"].ToString() + result.Result);
                        dic.Add("Md5", UtilityHelper.GetMD5HashFromFile(buffer));
                        buffer = null;
                        return Content(JsonConvert.SerializeObject(dic));

                    }
                    else
                    {
                        dic.Add("Code", 0);
                        dic.Add("Exception", result.Exception);
                        dic.Add("Msg", "上传失败");
                        return Content(JsonConvert.SerializeObject(dic));
                    }
                }

            }
            else
            {
                dic.Add("Code", 0);
                dic.Add("Msg", "请选择要上传的文件");
            }
            return Content("");
        }


    }
}