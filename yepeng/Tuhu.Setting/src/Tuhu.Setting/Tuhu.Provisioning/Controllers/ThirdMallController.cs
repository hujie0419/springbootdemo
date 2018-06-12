using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.ThirdPartyMallConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.OprLogManagement;
using Newtonsoft.Json;
using System.Web.Configuration;
using Tuhu.Service.Utility.Request;
using Tuhu.Service.Utility;
using Tuhu.Component.Framework.Extension;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class ThirdMallController : Controller
    {
        // GET: ThirdMall
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Select(int pageSize, int pageNumber, bool? isEnabled, int? visible, string branchName,
            string batchGuid, int sort)
        {
            SerchThirdPartyMallModel serch = new SerchThirdPartyMallModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                BatchName = branchName,
                IsEnabled = isEnabled,
                Visible = visible,
                Sort = sort

            };
            if (!string.IsNullOrWhiteSpace(batchGuid))
            {
                serch.BatchGuid = new Guid(batchGuid);
            }
            else
            { serch.BatchGuid = null; }
            var list = ThirdPartyMallConfigManage.SelectThirdMall(serch);
            return Json(list);
        }
  
        public JsonResult Operate(string type, string branchId, string branchName,bool? isEnabled,int? sort, int? limitQty, int? batchQty, DateTime? startDateTime, DateTime? endDateTime, string description,string ImageUrl,int pkid)
        {
            int result = -1;

            if (ControllerContext.HttpContext.User == null)
            {
                return Json(new { result = "请重新登录！" });
            }
            ThirdPartyMallModel thirdMall = new ThirdPartyMallModel()
            {
                PKID=pkid,
                BatchName = branchName,
                BatchQty = batchQty,
                IsEnabled = isEnabled,
                Sort = sort,
                LimitQty = limitQty,
                Description = description,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                ImageUrl = ImageUrl,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now,
                operater = ControllerContext.HttpContext.User.Identity.Name,
                BatchGuid = new Guid(branchId)
        };
            switch (type)
            {
                case "insert":
                    result = ThirdPartyMallConfigManage.InserThirdMall(thirdMall);
                    if (result > 0)
                    {
                        new OprLogManager().AddOprLog(new OprLog()
                        {
                            Author = HttpContext.User.Identity.Name,
                            AfterValue = JsonConvert.SerializeObject(thirdMall),
                            ChangeDatetime = DateTime.Now,
                            ObjectID = result,
                            ObjectType = "ThirdMall",
                            Operation = "新增三方商城记录",
                            HostName = Request.UserHostName
                        });
                    }
                    break;
                case "update":    
                    result = ThirdPartyMallConfigManage.EditThirdMall(thirdMall);
                    new OprLogManager().AddOprLog(new OprLog()
                    {
                        Author = HttpContext.User.Identity.Name,
                        AfterValue = JsonConvert.SerializeObject(thirdMall),
                        ChangeDatetime = DateTime.Now,
                        ObjectID = thirdMall.PKID,
                        ObjectType = "ThirdMall",
                        Operation = "编辑三方商城记录",
                        HostName = Request.UserHostName
                    });
                    break;
            }
            return Json(new { msg = result });
        }


        public ActionResult Detail(int pkid)
        {
            var result = ThirdPartyMallConfigManage.SelectThirdMall(pkid);
            result.ImageUrl = result.ImageUrl;
            result.ImgUrl = WebConfigurationManager.AppSettings["DoMain_image"] + result.ImageUrl;
            var result1 = JsonConvert.DeserializeObject<List<DescriptionModal>>(result.Description);
            return Json(result);
        }

        public ActionResult SelectCout(int pageSize, int pageNumber, bool? isEnabled, int? visible, string branchName,
            string batchGuid, int sort)
        {
            SerchThirdPartyMallModel serch = new SerchThirdPartyMallModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                BatchName = branchName,
                IsEnabled = isEnabled,
                Visible = visible,
                Sort = sort

            };
            if (!string.IsNullOrWhiteSpace(batchGuid))
            {
                serch.BatchGuid = new Guid(batchGuid);
            }
            else
            { serch.BatchGuid = null; }
            var result = ThirdPartyMallConfigManage.SelectCout(serch);
            return Json(new { msg = result }); ;
        }

        public JsonResult SelectBatch(string batchId)
        {
            ThirdPartyCodeBatch result = new ThirdPartyCodeBatch();
            if(!string.IsNullOrWhiteSpace(batchId))
            {
                 result = ThirdPartyMallConfigManage.SelectBatch(new Guid(batchId));
            }        
            return Json(result);
        }
        public JsonResult SelectLog(string pkid)
        {
            var content = "";
            var result = LoggerManager.SelectOprLogByParams("ThirdMall", pkid.ToString());
            var configHistories = result as ConfigHistory[] ?? result.ToArray();
            if (result != null && configHistories.Any())
            {
                content = configHistories.Aggregate(content, (current, h) => current + ("<tr><td>" + h.Author + "</td><td>" + h.Operation + "</td><td>" + h.ChangeDatetime + "</td></tr>"));
            }
            return Json(content);
        }


        [HttpPost]
        public JsonResult UploadImage()
        {
            var result = "";
            var temp = "";
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    using (var client = new FileUploadClient())
                    {
                        var res = client.UploadImage(new ImageUploadRequest()
                        {
                            Contents = buffer,
                            DirectoryName = "HomePagePopUpImg",
                            MaxHeight = 1920,
                            MaxWidth = 1920
                        });
                        if (res.Success && res.Result != null)
                        {
                            result = WebConfigurationManager.AppSettings["DoMain_image"] + res.Result;
                            temp = res.Result;
                        }
                    }
                }
                catch (Exception exp)
                {
                    WebLog.LogException(exp);
                }
            }
            return Json(new { msg = result ,res= temp });
        }


        public JsonResult SortChange(int sort)
        {
          var  result = ThirdPartyMallConfigManage.SortChange(sort);

            return Json(new { msg = result });
        }

        public JsonResult RefreshCache()
        {
            bool result = false;
            using (var client=new ThirdPartyClient())
            {
                result = client.RefreshThirdPartyCache().Result;
            }
            return Json(new { msg = result });
        }
    }
}