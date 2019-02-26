using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.SemCampaign;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;

namespace Tuhu.Provisioning.Controllers
{
    public class SemController : Controller
    {
        private readonly string imgUrl = "https://img1.tuhu.org";
        private static string strConn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(strConn.Decrypt());
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        
        public ActionResult Index(int pageNumber = 1)
        {
            var pager = new PagerModel(pageNumber);
            ListModel<AppDownloadCount> la = new ListModel<AppDownloadCount>() { Pager = pager, Source = new SemCampaignHandler(DbScopeManager).AppCampaignList() };
            return View(la);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AppDownloadListAsync(FormCollection fc)
        {
            HttpPostedFileBase file = Request.Files["appPath"];
            if (file != null)
            {
                AppDownloadCount adc = new AppDownloadCount()
                {
                    ArticleTitle = fc["apptitle"],
                    Channel = fc["appchannel"],
                    Creator = User.Identity.Name,
                    AppUrl = "",
                    OtherChannel = fc["otherSpread"]
                };

                int pkid = new SemCampaignHandler(DbScopeManager).CreateApp(adc);
                try
                {
                    var str = await SaveFileAsync(file);
                    if (str.Success)
                    {
                        new SemCampaignHandler(DbScopeManager).CreateOrDeleteApp(imgUrl + str.Result, pkid);
                    }
                    else
                    {
                        //没有更新成功，把之前新增的记录删除
                        var wew = new SemCampaignHandler(DbScopeManager).CreateOrDeleteApp("", pkid);
                    }
                  
                }
                catch (Exception ex)
                {
                    new SemCampaignHandler(DbScopeManager).CreateOrDeleteApp("", pkid);
                    return Json(ex.Message);
                }
                return RedirectToAction("Index");
            }
            else
                return Json("没有获取到上传文件，请返回重新提交！");
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public async Task<JsonResult> EditDownloadAppAsync(FormCollection fc)
        {
            HttpPostedFileBase file = Request.Files["appEditPath"];
            int pkid = fc["editPkid"] != null ? int.Parse(fc["editPkid"]) : 0;
            if (file != null && pkid > 0)
            {
                try
                {
                    var result = await SaveFileAsync(file);   //重新上传文件

                    if (!result.Success)
                        return Json(result.ErrorMessage);
                    else
                    {
                        //重新字段URL
                        new SemCampaignHandler(DbScopeManager).CreateOrDeleteApp(imgUrl + result.Result, pkid);
                        return Json("编辑上传成功");
                    }
                }
                catch(Exception e)
                {
                    return Json(e.Message);
                }
                

            }
            return Json("没有上传地址或没有获取到上传文件!");
        }
        /// <summary>
        /// 获取下载记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetHistory(int id)
        {
            var ss = new SemCampaignHandler(DbScopeManager).AppDownloadActionList(id);
            return Json(ss, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDelete(int id)
        {
            if (new SemCampaignHandler(DbScopeManager).DeleteApp(User.Identity.Name, id) > 0)
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            else
                return Json("删除失败", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<OperationResult<string>> SaveFileAsync(HttpPostedFileBase file)
        {
            var fileContent = default(byte[]);
            var ism = file.InputStream;
            using (var ms = new MemoryStream())
            {
                ism.CopyTo(ms);
                ms.Flush();
                fileContent = ms.ToArray();
            }
            using (FileUploadClient fileUploadClient = new FileUploadClient())
            {
                var fileUploadRequest = new FileUploadRequest
                {
                    Contents = fileContent,
                    Extension = ".apk"
                };
                var result = await fileUploadClient.UploadFileAsync(fileUploadRequest);
                return result;
            }
           
            
        }

    }
    //[DataContract]
    //public class UploadAuthorizationRequest: FileUploadRequest
    //{
    //    /// <summary>-1：私有上传；0：文件；1：图片；2：视频</summary>
    //    [DataMember]
    //    public int Type { get; set; }
    //    /// <summary>扩展名必须以.开始</summary>
    //    [DataMember]
    //    public string Extensions { get; set; }
    //}

}