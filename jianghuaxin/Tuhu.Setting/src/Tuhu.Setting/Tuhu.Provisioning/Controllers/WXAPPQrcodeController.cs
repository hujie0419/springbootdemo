using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Nosql;
using Tuhu.Provisioning.Common;
using System.Threading.Tasks;
using Tuhu;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.WXApp;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;

namespace Tuhu.Provisioning.Controllers
{
    public class WXAPPQrcodeController : Controller
    {
        // GET: WXAPPQrcode
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetQrcode()
        {
            WXAPPManager manager = new WXAPPManager();
            return Content(manager.GetAccess_token());
        }

        public ActionResult RemoveQrcode(string version)
        {
            if (version == "1.1.1")
            {
                WXAPPManager manager = new WXAPPManager();
                manager.Remove_token();
                return Content("更新token");
            }
            else
                return Content("未更新");
        }


        public ActionResult SubmitQcode(string data)
        {
            try
            {
                WXAPPManager manager = new WXAPPManager();
                if (string.IsNullOrWhiteSpace(data))
                    return Content("参数不能为空");
                var entity = JsonConvert.DeserializeObject<QCodeEntity>(data);
                entity.path = entity.path.Trim();
                using (var client = Tuhu.Nosql.CacheHelper.CreateCacheClient("WXAPPQcode"))
                {
                    var resultClient = client.Set<QCodeEntity>("qcode",  entity, TimeSpan.FromMinutes(2));
                    if(resultClient.Success)
                        return Content(JsonConvert.SerializeObject(new
                        {
                            Code = 1,
                            Msg = "成功"
                        }));
                    else
                        return Content(JsonConvert.SerializeObject(new
                        {
                            Code = -1,
                            Msg = "缓存设置失败"
                        }));
                }
               // return File(manager.GetQcode(entity, User.Identity.Name), "image/jpeg", Guid.NewGuid().ToString() + entity.path + ".jpg");
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new {
                    Code=-1,
                    Msg =em.Message
                }));
            }

        }


        public ActionResult GetQcode()
        {
            try
            {
                WXAPPManager manager = new WXAPPManager();
                using (var client = Tuhu.Nosql.CacheHelper.CreateCacheClient("WXAPPQcode"))
                {
                    var resultClient = client.Get<QCodeEntity>("qcode");
                    if (resultClient.Success)
                    {
                        var bytes = manager.GetQcode(resultClient.Value, User.Identity.Name);
                        client.Remove("qcode");
                        return File(bytes, "image/jpeg", Guid.NewGuid().ToString() + resultClient.Value.path + ".jpg");
                    }
                    else
                        return Content(JsonConvert.SerializeObject(new
                        {
                            Code = -1,
                            Msg = "缓存设置失败"
                        }));
                }
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(em));
            }
        }

        public ActionResult GetGridJson(Pagination pagination)
        {
            RepositoryManager manager = new RepositoryManager();
            var list = manager.GetEntityList<Tuhu.Provisioning.DataAccess.Mapping.QcodeRecordEntity>(pagination);
            return Content(JsonConvert.SerializeObject(new
            {
                rows = list,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }
            
           
        

    }
}