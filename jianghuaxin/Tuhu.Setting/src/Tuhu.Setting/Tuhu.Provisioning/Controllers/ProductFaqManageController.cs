using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Product;
using Tuhu.Provisioning.Business.Logger;

namespace Tuhu.Provisioning.Controllers
{
    public class ProductFaqManageController : Controller
    {
        // GET: ProductFaqManage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult ListForm(Pagination pagination)
        {
            return View();
        }
        public ActionResult ConfigList()
        {
            return View();
        }
        /// <summary>
        /// 品牌checkbox列表
        /// </summary>
        public ActionResult BrandsCheckBoxListControl(string categorys = "1")
        {
            var cpBrandList =
                ProductLibraryConfigController.QueryProducts(new SeachProducts() { category = categorys })?.CP_BrandList ??
                new List<FilterConditionModel>();
            return View(cpBrandList);
        }
        public ActionResult BrandListSelect(string B_Categorys = "1")
        {
            IEnumerable<FilterConditionModel> cpBrandList =
       ProductLibraryConfigController.QueryProducts(new SeachProducts() { category = B_Categorys })?.CP_BrandList ??
       new List<FilterConditionModel>();

            //if (cpBrandList != null && cpBrandList.Any() && !string.IsNullOrWhiteSpace(B_Brands))
            //{
            //    cpBrandList.ForEach(a =>
            //    {
            //        var brands = B_Brands.Split(',');
            //        foreach (var item in brands)
            //        {
            //            if (a.Name.Contains(item))
            //                a.Name = a.Name + ":True";
            //        }
            //    });
            //}

            //var B_CategorysId = (B_Categorys ?? "").Contains(".")
            //    ? B_Categorys.Substring(B_Categorys.LastIndexOf(".") + 1)
            //    : B_Categorys;

            //ViewBag.BrandsListControlCategorys = InteriorCategorysTreeJson();
            ViewBag.CP_BrandList = cpBrandList;
            ViewBag.B_Categorys = InteriorCategorysTreeJson();
            return View();
        }

        public static string InteriorCategorysTreeJson(string opens = "")
        {
            opens = opens ?? "";
            var opensArr = opens?.Split(',');
            var ZTreeModel = ProductFaqManage.SelectProductCategories()?.Select(m => new
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
        /// <summary>
        /// 筛选要配置的pid列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="searchString"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public ActionResult GetFaqPidList(Pagination pagination, string searchString, string pids)
        {
            var result = new List<ProductFaqConfigModel>();
            if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(pids))
            {
                var brandModel = new List<CBrandModel>();
                if (!string.IsNullOrEmpty(searchString))
                    brandModel = JsonConvert.DeserializeObject<List<CBrandModel>>(searchString);
                var dalPids = ProductFaqManage.SelectSimpleProductModels(brandModel, pids, pagination);
                var dalFaqs = ProductFaqManage.SelectProductFaqConfigModels();
                result = (from a in dalPids
                          join b in dalFaqs on a.Pid equals b.Pid into temp
                          from b in temp.DefaultIfEmpty()
                          select new ProductFaqConfigModel()
                          {
                              Pid = a.Pid,
                              ProductName = a.ProductName,
                              QuestionDetail = b?.QuestionDetail,
                              LastUpdateDateTime = b?.LastUpdateDateTime
                          }).ToList();
            }
            TempData["searchString"] = searchString;
            return Content(JsonConvert.SerializeObject(new
            {
                rows = result,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }
        /// <summary>
        /// 筛选已配置的商品列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetFaqProductList(Pagination pagination)
        {
            var result = ProductFaqManage.SelectProductFaqConfigModelsPagination(pagination);
            return Content(JsonConvert.SerializeObject(new
            {
                rows = result,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }

        /// <summary>
        /// 查询问题详情
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="searchString"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public ActionResult FaqConfig(string keyValue, string pids, string searchString)
        {
            var list = new List<ProductFaqConfigDetailModel>();
            var flag = true;
            if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(pids))
            {
                //var cbs = searchString.Replace("\'","\"");
                //var aa = cbs.Substring(1, cbs.Length - 1);
                searchString = TempData["searchString"].ToString();
                var brandModel = JsonConvert.DeserializeObject<List<CBrandModel>>(searchString);
                var dalPids = ProductFaqManage.SelectSimpleProductModels(brandModel, pids).ToList();
                var pidss = dalPids.Select(r => r.Pid.Trim()).ToList();
                list= CheckPidHasSingleFaq(pidss);
                //全选的情况 todo 传查询条件过来去表里重新查一遍全部的
                //if (pidss.Count > 5000)
                //    ViewBag.Pids = "Long";
                //else
                //ViewBag.Pids = pids;
                //ViewBag.searchString=
                TempData["Pids"] = pids;
                TempData["searchString"] = searchString;
            }
            else
            {
                var listpids = keyValue.Split(',').ToList();

                //多选情况
                //todo 多选还有一种情况是指勾选的产品配置的问题都一样的话要传过去
                if (listpids.Count() > 1)
                {
                    list = CheckPidHasSingleFaq(listpids);

                }

                else //单选情况
                {
                    list = ProductFaqManage.SelectProductFaqConfigDetailModels(keyValue).ToList();
                    if (!list.Any())
                    {
                        list.Add(new ProductFaqConfigDetailModel
                        {
                            Sort = 1
                        });
                    }
                }
            }
            ViewBag.DataCount = list.Count;
            return View(list);
        }
        /// <summary>
        /// 保存问题详情//这里有个坑，不编码穿不过来
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public ActionResult SaveDetails(string datas, string pids)
        {
            try
            {
                var models = JsonConvert.DeserializeObject<List<ProductFaqConfigDetailModel>>
                    (datas.Replace(@"\n","").Replace(@"\t","").Replace(@"\r","").Replace("&nbsp;",""));
                var productFaqConfigDetailModels = models.Select(r =>
                {
                    r.Answer = HttpUtility.HtmlDecode(r.Answer)
                    ?.Replace(@"&nbsp;", "").Replace(@"\u0008","")
                    .Replace(@"\u0009","").Replace(@"\u000A","")
                    .Replace(@"\u000B","").Replace(@"\u000C","")
                    .Replace(@"\u000D","").Replace(@"\u0022","")
                    .Replace(@"\u0027","").Replace(@"\u005C","")
                    .Replace(@"\u00A0","").Replace(@"\u2028","")
                    .Replace(@"\u2029","").Replace(@"\uFEFF","").
                     Replace("\u0008", "")
                    .Replace("\u0009", "").Replace("\u000A", "")
                    .Replace("\u000B", "").Replace("\u000C", "")
                    .Replace("\u000D", "").Replace("\u0022", "")
                    .Replace("\u0027", "").Replace("\u005C", "")
                    .Replace("\u00A0", "").Replace("\u2028", "")
                    .Replace("\u2029", "").Replace("\uFEFF", "");
                    return r;
                }).ToList();
                var listPids = new List<string>();
                if (string.IsNullOrEmpty(pids))
                {
                    var searchString = TempData["searchString"].ToString();
                    pids = TempData["Pids"]?.ToString();
                    var brandModel = JsonConvert.DeserializeObject<List<CBrandModel>>(searchString);
                    var dalPids = ProductFaqManage.SelectSimpleProductModels(brandModel, pids).ToList();
                    var pidss = dalPids.Select(r => r.Pid.Trim()).ToList();
                    listPids = pidss;
                }
                else
                {
                    listPids = pids.Split(',').Distinct().ToList();
                }                 
                var dalResult = ProductFaqManage.UpdateProductFaqConfigAndDetailDetailModels(productFaqConfigDetailModels, listPids);
                if (dalResult)
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(productFaqConfigDetailModels), Author = User.Identity.Name, Operation = "编辑常见问题", ObjectType = "Faq" });
                    return Json(new { status = 1, msg = "更新成功" });
                }
                else
                {
                    return Json(new { status = 0, msg = "更新失败" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, msg = "更新失败" + ex });
            }

        }

        public ActionResult RefreshCache()
        {
            bool result = true;
            try
            {
                using (var client = new ProductDetailClient())
                {
                    var res = client.RefreshProductFaqDetailCacheByPids(null);
                    result = res.Result;
                    if (result)
                        return Content(JsonConvert.SerializeObject(new
                        {
                            Code = 1,
                            Msg = "更新成功"
                        }));
                    else
                    {
                        return Content(JsonConvert.SerializeObject(new
                        {
                            Code = -1,
                            Msg = "更新失败"
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                return Content(JsonConvert.SerializeObject(new
                {
                    Code = -1,
                    Msg = "更新失败" + ex
                }));
            }
        }


        public ActionResult PatchDelFaqConfigPid(string keyValue, string pids, string searchString)
        {
            try
            {
                bool result = true;
                if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(pids))
                {
                    searchString = TempData["searchString"].ToString();
                    var brandModel = JsonConvert.DeserializeObject<List<CBrandModel>>(searchString);
                    var dalPids = ProductFaqManage.SelectSimpleProductModels(brandModel, pids).ToList();
                    var pidss = dalPids.Select(r => r.Pid).ToList();
                    result = ProductFaqManage.PatchDelFaqConfigPid(pidss.ToList());


                }
                else
                {
                    var listpids = JsonConvert.DeserializeObject<List<string>>(keyValue);

                    result = ProductFaqManage.PatchDelFaqConfigPid(listpids);
                }
                if (result)
                    return Content(JsonConvert.SerializeObject(new
                    {
                        state = "success",
                        message = "操作成功"
                    }));
                else
                {
                    return Content(JsonConvert.SerializeObject(new
                    {
                        state = "error",
                        message = "操作失败"
                    }));
                }
            }
            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "删除失败" + e
                }));
            }
        }

        private List<ProductFaqConfigDetailModel> CheckPidHasSingleFaq(List<string> listpids)
        {
            var list = new List<ProductFaqConfigDetailModel>();
            var flag = true;
            var firstPid = listpids.FirstOrDefault();
            var faqdetail = ProductFaqManage.SelectProductFaqConfigDetailModels(firstPid);
            if (faqdetail == null || !faqdetail.Any())
            {
                list.Add(new ProductFaqConfigDetailModel
                {
                    Sort = 1
                });
            }
            else
            {
                var fkfaqid = faqdetail.Select(r => r.FkFaqId).FirstOrDefault();
                if (listpids.Count() > 100)
                {
                    var count = ProductFaqManage.SelectNotSameFaqCount(listpids, fkfaqid);
                    if (count == 0)
                    {
                        list = faqdetail.ToList();

                    }
                    else
                    {
                        list.Add(new ProductFaqConfigDetailModel
                        {
                            Sort = 1
                        });
                    }
                }
                else
                {
                    foreach (var pid in listpids)
                    {
                        var innerList = ProductFaqManage.SelectProductFaqConfigDetailModels(pid);
                        if (innerList == null || !innerList.Any() ||
                            innerList.Select(r => r.FkFaqId).FirstOrDefault() != fkfaqid)
                        {
                            list = new List<ProductFaqConfigDetailModel>();
                            list.Add(new ProductFaqConfigDetailModel
                            {
                                Sort = 1
                            });
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        list = faqdetail.ToList();
                    }
                }
            }
            return list;

        }
    }
}