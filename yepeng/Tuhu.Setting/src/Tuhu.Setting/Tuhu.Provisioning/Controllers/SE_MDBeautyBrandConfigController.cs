using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tuhu.Provisioning.Controllers
{
    public class SE_MDBeautyBrandConfigController : Controller
    {
        //
        // GET: /SE_MDBeautyBrandConfig/

        public ActionResult Index()
        {
            ViewBag.ZTreeJson = SE_MDBeautyBrandTreeJson();
            return View();
        }

        public ActionResult Edit(int id = 0, int parentId = 0)
        {
            SE_MDBeautyBrandConfigModel model = new SE_MDBeautyBrandConfigModel()
            {
                ParentId = parentId
            };

            if (id > 0)
            {
                model = SE_MDBeautyBrandConfigBLL.Select(id) ?? new SE_MDBeautyBrandConfigModel();
            }

            ViewBag.ZTreeJsonEdit = SE_MDBeautyCategoryConfigController.SE_MDBeautyCategoryTreeJson(model.CategoryIds, true);

            return View(model);
        }

        public ActionResult BatchEdit(int parentId = 0)
        {
            SE_MDBeautyBrandConfigModel model = new SE_MDBeautyBrandConfigModel()
            {
                Id = 0,
                ParentId = parentId,
                CreateTime = DateTime.Now,
                IsDisable = false
            };
            return View(model);
        }

        public ActionResult Save(SE_MDBeautyBrandConfigModel model)
        {
            bool result = false;
            if (model != null)
            {
                if (model.Id > 0)
                    result = SE_MDBeautyBrandConfigBLL.Update(model);
                else
                    result = SE_MDBeautyBrandConfigBLL.Insert(model);
            }
            return RedirectToAction("Index");
        }

        public ActionResult BatchSave(SE_MDBeautyBrandConfigModel model)
        {
            bool result = false;
            if (model != null)
            {
                if (model.ParentId > 0)
                    result = SE_MDBeautyBrandConfigBLL.BatchInsert(model);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 获取门店美容品牌树
        /// </summary>
        public static string SE_MDBeautyBrandTreeJson(string opens = "", bool isDisable = false)
        {
            opens = opens ?? "";
            var opensArr = opens.Split(',');
            var ZTreeModel = SE_MDBeautyBrandConfigBLL.SelectList()?.Select(m => new
            {
                id = m.Id,
                pId = m.ParentId,
                name = m.BrandName,
                open = opensArr.Contains(m.Id.ToString().Trim()),
                @checked = opensArr.Contains(m.Id.ToString().Trim()),
                chkDisabled = false,
                disabled = m.IsDisable,
                isParent = m.ParentId == 0
            });

            if (isDisable)
                ZTreeModel = ZTreeModel.Where(m => m.disabled == false);

            return JsonConvert.SerializeObject(ZTreeModel);
        }

        /// <summary>
        /// 检测是否有相同名称项
        /// </summary>
        public string SelectListByBrandName(string brandName = "",int id = 0)
        {
            JObject jj = new JObject();
            if (!string.IsNullOrWhiteSpace(brandName))
            {
                var data = SE_MDBeautyBrandConfigBLL.SelectListByBrandName(brandName.Split(','), id);
                if (data != null && data.Count() > 0)
                {
                    jj.Add("item", data.Count());
                    jj.Add("values", string.Join(",", data.Select(s => s.BrandName)));
                    return JsonConvert.SerializeObject(jj);
                }
            }
            jj.Add("item", 0);
            jj.Add("values", "");
            return JsonConvert.SerializeObject(jj);
        }
    }
}