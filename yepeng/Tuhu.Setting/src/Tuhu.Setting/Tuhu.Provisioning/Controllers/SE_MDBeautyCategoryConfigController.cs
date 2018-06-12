using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Controllers
{
    public class SE_MDBeautyCategoryConfigController : Controller
    {
        //
        // GET: /SE_MDBeautyCategoryConfig/

        public ActionResult Index()
        {
            ViewBag.ZTreeJson = SE_MDBeautyCategoryTreeJson();
            return View();
        }

        public ActionResult Edit(int id = 0, int parentId = 0)
        {
            SE_MDBeautyCategoryConfigModel model = new SE_MDBeautyCategoryConfigModel()
            {
                ParentId = parentId
            };

            if (id > 0)
            {
                model = SE_MDBeautyCategoryConfigBLL.Select(id) ?? new SE_MDBeautyCategoryConfigModel();
            }

            return View(model);
        }

        public ActionResult Save(SE_MDBeautyCategoryConfigModel model)
        {
            bool result = false;
            if (model != null)
            {
               

               // SE_MDBeautyCategoryConfigBLL.GetPidsFromMDBeautyCategoryProductConfigByCategoryIds(new);


                if (model.Id > 0)
                {
                    result = SE_MDBeautyCategoryConfigBLL.Update(model);
                }
                else
                {
                    result = SE_MDBeautyCategoryConfigBLL.Insert(model);
                }

                //刷新分类缓存
                try
                {
                    using (var client = new Tuhu.Service.Shop.CacheClient())
                    {
                        var clientResult = client.UpdateBeautyAvailableCategories();

                        if (clientResult.Result)
                            TempData["UpdateBeautyAvailableCategories"] = "刷新美容分类缓存成功！";
                        else
                            TempData["UpdateBeautyAvailableCategories"] = clientResult.ErrorMessage;
                    }
                }
                catch { }
            }
            return RedirectToAction("Index");
        }


        /// <summary>
        /// 获取门店美容类目树
        /// </summary>
        public static string SE_MDBeautyCategoryTreeJson(string opens = "", bool isDisable = false)
        {
            opens = opens ?? "";
            var opensArr = opens?.Split(',');
            var ZTreeModel = SE_MDBeautyCategoryConfigBLL.SelectList()?.Select(m => new
            {
                id = m.Id,
                pId = m.ParentId,
                name = m.CategoryName,
                open = opensArr.Contains(m.Id.ToString().Trim()),
                @checked = opensArr.Contains(m.Id.ToString().Trim()),
                chkDisabled = false,
                disabled = m.IsDisable,
                isParent = m.ParentId == 0,
                childs = m.Childs,
                parents = m.Parents
            });

            if (isDisable)
                ZTreeModel = ZTreeModel.Where(m => m.disabled == false);

            return JsonConvert.SerializeObject(ZTreeModel);
        }

        public static string SE_MDBeautyCategoryTreeJsonForPart(string opens = "", string excludePids = "", bool isDisable = false)
        {
            opens = opens ?? "";
            excludePids = excludePids ?? "";
            var opensArr = opens?.Split(',');
            var excludeArr = excludePids?.Split(',');
            var ZTreeModel = SE_MDBeautyCategoryConfigBLL.SelectListForPart()?.Select(m => new
            {
                id = m.Id,
                pId = m.ParentId,
                name = m.CategoryName,
                open = opensArr.Contains(m.Id.ToString().Trim()),
                @checked = m.Id.Contains('-') ? (excludeArr.Contains(m.Id.Trim()) ? false : true) : opensArr.Contains(m.Id.Trim()),
                chkDisabled = false,
                disabled = m.IsDisable,
                isParent = m.ParentId == 0,
                childs = m.Childs,
                parents = m.Parents
            });

            if (isDisable)
                ZTreeModel = ZTreeModel.Where(m => m.disabled == false);

            return JsonConvert.SerializeObject(ZTreeModel);
        }
    }
}