using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.Business.ThirdParty;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class AlipayBaoyangManageController : Controller
    {
        // GET: AlipayBaoyangManage
        public ActionResult AlipayBaoyangItems()
        {
            AliPayBaoYangSetting setting = new AliPayBaoYangSetting();
            setting.Activity = AliPayBaoYangManage.GetAliPayBaoYangActivity();            
            if (setting.Activity == null)
            {
                setting.Activity = new AliPayBaoYangActivity();
                var datetime = DateTime.Now;
                setting.Activity.BeginTime =Convert.ToDateTime(datetime.ToString("yyyy-MM-dd HH:mm:ss"));
                setting.Activity.EndTime = Convert.ToDateTime(datetime.ToString("yyyy-MM-dd HH:mm:ss")); ;
            }

            setting.Activity.BeginTime= Convert.ToDateTime(setting.Activity.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"));
            setting.Activity.EndTime = Convert.ToDateTime(setting.Activity.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            setting.AliPayBaoYangItems=AliPayBaoYangManage.GetAliPayBaoYangItem(-1);
            return View(setting);
        }

        [HttpPost]
        public ActionResult UpdateAliPayBaoYangSetting(AliPayBaoYangActivity model)
        {
            bool flag = false;
            if (model != null)
            {
                AliPayBaoYangManage.UpdateAliPayBaoYangActivity(model);
                flag = true;              
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddAlipayBaoYangItem()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAlipayBaoYangItem(AliPayBaoYangItem item)
        {         
            bool flag = false;
            try
            {
                List<AliPayBaoYangItem> items=AliPayBaoYangManage.GetAliPayBaoYangItem(-1);
                if (items != null)
                {
                    if (!items.Where(q => q != null).Any(p => p.KeyWord.Equals(item.KeyWord, StringComparison.OrdinalIgnoreCase)))
                    {
                        AliPayBaoYangManage.SaveAliPayBaoYangItem(item);
                        flag = true;
                    }
                }               
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshCarBaoYangPackageCache()
        {         
            var httpClient = new HttpClient();
            var response =  httpClient.GetAsync("https://open.tuhu.cn/thirdparty/RemoveCarBaoYangPackageKey");
            Task<bool> returnValue = response.Result.Content.ReadAsAsync<bool>();                       
            return Json(returnValue?.Result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult UpdateAlipayBaoYangItem(int pkid)
        {
            var item = AliPayBaoYangManage.GetAliPayBaoYangItem(pkid);
            var first = item.FirstOrDefault() != null ? item.First() : new AliPayBaoYangItem();

            return View(first);

        }
        [HttpPost]
        public ActionResult UpdateAlipayBaoYangItem(AliPayBaoYangItem item)
        {
            int result = AliPayBaoYangManage.UpdateAliPayBaoYangItem(item);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAlipayBaoYangItem(int pkid)
        {
            bool flag = false;
            try
            {
                AliPayBaoYangManage.DeleteAliPayBaoYangItem(pkid);
                flag = true;
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

    }
}