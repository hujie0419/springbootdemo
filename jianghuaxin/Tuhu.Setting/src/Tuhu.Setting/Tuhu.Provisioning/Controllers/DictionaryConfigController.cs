using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using Newtonsoft.Json.Linq;

namespace Tuhu.Provisioning.Controllers
{
    public class DictionaryConfigController : Controller
    {
        //
        // GET: /CommDictionary/

        public ActionResult List()
        {
            return View(SE_DictionaryConfigManager.GetList());
        }


        public ActionResult Edit(int? id = 0)
        {
            if (id.Value == 0)
            {
                return View(new SE_DictionaryConfigModel());
            }
            else
            {
                SE_DictionaryConfigModel model = SE_DictionaryConfigManager.GetEntity(id.Value.ToString());
                return View(model);
            }
          
        }
      

        public ActionResult Save(SE_DictionaryConfigModel entity)
        {
            JObject json = new JObject();
            try
            {
                int status = SE_DictionaryConfigManager.Save(entity);
                json.Add("Code",1);
                json.Add("Status",status);
            }
            catch
            {
                json.Add("Code", 0);
                json.Add("Status", 0);
            }
            return Json(json.ToString());
        }

        public ActionResult Delete(int? id)
        {
            JObject json = new JObject();
            if (id.HasValue)
            {
                if (SE_DictionaryConfigManager.Delete(id.Value.ToString()))
                {
                    json.Add("Code", 1);
                }
                else
                {
                    json.Add("Code",0);
                }
                return Json(json.ToString());
            }
            else
            {
                json.Add("Code",0);
                return Json(json.ToString());
            }
        }


    }
}
