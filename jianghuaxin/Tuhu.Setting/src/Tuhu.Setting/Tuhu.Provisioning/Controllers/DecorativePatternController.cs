using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Controllers
{
    public class DecorativePatternController : Controller
    {
        //
        // GET: /DecorativePattern/

        public ActionResult Index(string type="",string selName="")
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                IEnumerable<SE_DecorativePattern> list = DecorativePatternManager.GetList();
                return View(list);
            }
            else
            {
                IEnumerable<SE_DecorativePattern> list = DecorativePatternManager.GetList(type,selName);
                return View(list);
            }
              

        }


        [ValidateInput(false)]
        public ActionResult ArticleEdit(string id = "", string title = "",string image="",string orderby="",string description="")
        {
            SE_DecorativePatternDetail model = new SE_DecorativePatternDetail();
            if (!string.IsNullOrWhiteSpace(id))
            {
                model.ArticleID = Convert.ToInt32(id);
                model.ArticleTitle = title;
                model.Image = image;
                model.OrderBy = Convert.ToInt32(orderby);
                model.Description = description;
            }
           
            return View(model);
        }


        public ActionResult Edit(string id="")
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View(new SE_DecorativePattern());
            }
            else
            {
                return View(DecorativePatternManager.GetEntity(id));
            }
        }

        [ValidateInput(false)]
        public ActionResult Save(string content = "")
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                SE_DecorativePattern model = JsonConvert.DeserializeObject<SE_DecorativePattern>(content);
                if (model != null)
                {
                    if (DecorativePatternManager.Save(model))
                        return Json(1);
                    else
                        return Json(0);
                }
                else
                    return Json(0);
            }
            else
                return Json(0);
           
        }


        public ActionResult GetTriePatten(string tire)
        {
            return Json(JsonConvert.SerializeObject(DecorativePatternManager.GetTriePatten(tire)));
        }

        public ActionResult Delete(string id)
        {
            return Json(DecorativePatternManager.Delete(id)==true?1:0);
        }

    }
}
