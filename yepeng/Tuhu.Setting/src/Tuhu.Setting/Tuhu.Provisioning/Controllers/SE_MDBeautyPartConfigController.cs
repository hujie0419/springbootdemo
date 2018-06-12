using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.MDBeauty;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.OperationCategory;
using Tuhu.Service.GroupBuying;

namespace Tuhu.Provisioning.Controllers
{
    public class SE_MDBeautyPartConfigController : Controller
    {
        //
        // GET: /SE_MDBeautyPartConfig/

        public ActionResult Index(int pageIndex = 1, int pageSize = 10)
        {
            IEnumerable<SE_MDBeautyPartConfigModel> data = SE_MDBeautyPartConfigBLL.SelectPages(pageIndex, pageSize);

            ViewBag.totalRecords = (data != null && data.Any())
                ? data.FirstOrDefault().TotalCount
                : 0;

            ViewBag.totalPage = (data != null && data.Any())
                ? data.FirstOrDefault().TotalPage(pageSize)
                : 0;

            return View(data);
        }

        //
        // GET: /SE_MDBeautyPartConfig/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SE_MDBeautyPartConfigModel model = new SE_MDBeautyPartConfigModel();
            model.Soft = 1;

            if (id > 0)
            {
                model = SE_MDBeautyPartConfigBLL.Select(id);
            }

            ViewBag.InteriorCategorys = InteriorCategorysTreeJson(model.InteriorCategorys);
            ViewBag.ExternalCategorys = SE_MDBeautyCategoryConfigController.SE_MDBeautyCategoryTreeJsonForPart(model.ExternalCategorys, model.ExcludePids, true);

            return View(model);
        }

        //
        // GET: /SE_MDBeautyPartConfig/Delete/5

        public ActionResult Delete(int id)
        {
            if (id > 0)
            {
                SE_MDBeautyPartConfigBLL.Delete(id);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Save(SE_MDBeautyPartConfigModel model)
        {
            if (model != null)
            {
                var cates = model.ExternalCategorys?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var pids = cates.Where(w => w.Contains('-'));
                if (pids != null && pids.Any())
                {
                    var categoryIds = cates?.Where(w => !w.Contains('-'));
                    var cate_pids = Business.SE_MDBeautyCategoryConfigBLL.GetPidsFromMDBeautyCategoryProductConfigByCategoryIds(categoryIds);
                    if (cate_pids != null && cate_pids.Any())
                    {
                        var excludePids = cate_pids.Where(w => !pids.Contains(w));
                        if (excludePids != null && excludePids.Any())
                            model.ExcludePids = string.Join(",", excludePids);
                    }
                    model.ExternalCategorys = string.Join(",", categoryIds);
                }
                if (model.Id > 0)
                    SE_MDBeautyPartConfigBLL.Update(model);
                else
                    SE_MDBeautyPartConfigBLL.Insert(model);

                RefreshBeautyConfigCache("ios");
            }
            return RedirectToAction("Index");
        }

        private static readonly OperationCategoryManager _CategoryTagManager = new OperationCategoryManager();
        /// <summary>
        /// 获取门店美容类目树
        /// </summary>
        public static string InteriorCategorysTreeJson(string opens = "")
        {
            opens = opens ?? "";
            var opensArr = opens?.Split(',');
            var ZTreeModel = _CategoryTagManager.SelectProductCategories()?.Select(m => new
            {
                id = m.id,
                pId = m.pId,
                name = m.name,
                open = opensArr.Contains(m.id.ToString().Trim()),
                @checked = opensArr.Contains(m.id.ToString().Trim()),
                chkDisabled = m.chkDisabled,
                NodeNo = m.NodeNo
            });
            return JsonConvert.SerializeObject(ZTreeModel);
        }

        private bool RefreshBeautyConfigCache(string channle)
        {
            using (var client = new GroupBuyingClient())
            {
                var result = client.RefreshBeautyConfigCache(channle);
                return result.Success;
            }
        }
    }
}
