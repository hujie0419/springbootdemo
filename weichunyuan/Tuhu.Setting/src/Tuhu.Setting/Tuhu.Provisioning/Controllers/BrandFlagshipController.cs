using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.Activity;

namespace Tuhu.Provisioning.Controllers
{
    public class BrandFlagshipController : Controller
    {
        //
        // GET: /BrandFlagship/

        public ActionResult Index()
        {
            return View(SE_BrandFlagshipManager.GetList());
        }

        public ActionResult Edit(string id="")
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View(new SE_BrandFlagship());
            }
            else
            {
                SE_BrandFlagship model = SE_BrandFlagshipManager.GetEntity(id);
                return View(model);
            }
        }

        [ValidateInput(false)]
        public ActionResult ArticleEdit(int? type,string id="",string title="",string orderby="",string description="")
        {
            SE_BrandFlagshipDetail model = new SE_BrandFlagshipDetail();
            if (!string.IsNullOrWhiteSpace(id))
            {
                model.ArticleID = Convert.ToInt32(id);
                model.ArticleTitle = title;
                model.OrderBy = Convert.ToInt32(orderby);
                model.Description = description;
            }
            model.ArticleType = type.Value;
            return View(model);
        }


        public ActionResult SelectActricle(string id)
        {
            return Json(SE_BrandFlagshipManager.SelectArticle(id));
        }

        [ValidateInput(false)]
        public ActionResult Save(string content)
        {
            SE_BrandFlagship model = JsonConvert.DeserializeObject<SE_BrandFlagship>(content);
            return Json(SE_BrandFlagshipManager.Save(model));
        }

        public ActionResult Delete(string id)
        {
            if (SE_BrandFlagshipManager.Delete(id))
                return Json(1);
            else
                return Json(0);
        }

        public ActionResult FlowerValite(string id)
        {
            Guid result;
            if (!Guid.TryParse(id, out result))
            {
                return Json(0);
            }

            if (DecorativePatternManager.GetEntity(id) != null)
            {
                return Json(1);
            }
            else
                return Json(0);
        }

        public ActionResult SimpleActivity(string id)
        {

            if (SE_ActivityManager.GetEntity(id) != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }



    }


}
