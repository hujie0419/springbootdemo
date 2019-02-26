using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ThBiz.DataAccess.Entity;
using Tuhu.Component.Framework;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.Business.ProductLibraryConfing;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Request;
using OprLog = Tuhu.Provisioning.DataAccess.Entity.OprLog;

namespace Tuhu.Provisioning.Controllers
{
    public class ProductLibraryConfigController : Controller
    {
        private static readonly Lazy<OprLogManager> lazyOprLog = new Lazy<OprLogManager>();
        private static readonly ILog logger = LoggerFactory.GetLogger("ProductLibraryConfigController");
        private OprLogManager OprLogManager
        {
            get { return lazyOprLog.Value; }
        }
        private static readonly ProductLibraryConfigManage _ProductLibraryConfigManage = new ProductLibraryConfigManage();
        private static readonly Business.OperationCategory.OperationCategoryManager _OperationCategoryManager = new Business.OperationCategory.OperationCategoryManager();

        public ActionResult Index(FormCollection Form)
        {
            ViewBag.CategoryTagManager = ProductCategories();
            
            return View();
        }
         
        public ActionResult BatchAddCoupon(string oids = "", string coupons = "", int isBatch = 0, string opration = "")
        {

            if (!string.IsNullOrWhiteSpace(oids))
            {
                if (opration.Equals("Delete") && !string.IsNullOrWhiteSpace(coupons) && !string.IsNullOrWhiteSpace(oids))
                {
                    var model = _ProductLibraryConfigManage.BatchDeleteCoupon(oids, coupons);
                    _ProductLibraryConfigManage.RefreshCache(oids);
                    AddOprLog(oids, coupons, "删除");
                    return Content(JsonConvert.SerializeObject(model));
                }
                else
                {
                    var model = _ProductLibraryConfigManage.BatchAddCoupon(oids, coupons, isBatch);
                    AddOprLog(oids, coupons, "编辑");
                    _ProductLibraryConfigManage.RefreshCache(oids);
                    return Content(JsonConvert.SerializeObject(model));
                }
            }
            return View();
        }

        public JsonResult BatchAddCouponNew(string oids = "", string rulePKIDs = "", decimal grossProfit = 0)
        {
            MsgResultModel result = new MsgResultModel() { State = 1 };
            var oidList = GetIntList(oids);
            var rulePkIdList = GetIntList(rulePKIDs);
            var unavaiableProducts = new List<QueryProductsModel>();
            if (oidList.Any() && rulePkIdList.Any())
            {
                var productConponConfig = _ProductLibraryConfigManage.GetProductCouponConfigByOids(oidList);
                var products = _ProductLibraryConfigManage.QueryProducts(oidList);
                var unavaiableOids = new List<int>();
                var isCanInsert = false;
                var ruleList = new List<GetPCodeModel>();
                var hasExistsCouponOids = new List<int>();
                foreach (var rulePkid in rulePkIdList)
                {
                    var hasExistCurrentCouponOids = productConponConfig.Where(t => GetIntList(t.CouponIds).
                    Contains(rulePkid)).Select(t => t.Oid);
                    var currentoidList = oidList.Where(t => !hasExistCurrentCouponOids.Contains(t) 
                    && !hasExistsCouponOids.Contains(t)).ToList();
                    if (currentoidList.Any())
                    {
                        isCanInsert = true;
                        var currentprdocuts = products.Where(t => currentoidList.Contains(t.Oid));
                        var rule = _ProductLibraryConfigManage.GetGeCouponRulesByRulePkid(rulePkid);
                        if (rule != null)
                        {
                            ruleList.Add(rule);
                            var oidListFiltered = _ProductLibraryConfigManage.GetAvailableCouponProducts(currentprdocuts,
                                rule, grossProfit).Select(t => t.Oid);
                            if (oidListFiltered.Any())
                            {
                                var oidsFiltered = string.Join(",", oidListFiltered);
                                var addCouponResult = _ProductLibraryConfigManage.BatchAddCoupon(oidsFiltered, rulePkid.ToString(), 0);
                                if (addCouponResult.State == 0)
                                {
                                    result.Message = "插券失败";
                                    result.State = 0;
                                    break;
                                }
                                else
                                {
                                    hasExistsCouponOids.AddRange(oidListFiltered);
                                }
                            }
                        }                      
                    }
                }
                unavaiableProducts = _ProductLibraryConfigManage.GetUnavailableCouponProducts(products, ruleList, grossProfit).ToList();
                _ProductLibraryConfigManage.RefreshCache(oids);
                if (!isCanInsert)
                {
                    result.Message = "所有产品已经存在该优惠券";
                    result.State = 0;
                }
            }

            return Json(new { result = result, unavaiableProducts = unavaiableProducts },
                JsonRequestBehavior.AllowGet);
        }

         public ActionResult VerfyProductCoupon(string oids, string rules, decimal grossProfit)
        {
            IEnumerable<QueryProductsModel> unavaiableProducts = null;
            var avaiableRuleProducts = new Dictionary<string, IEnumerable<QueryProductsModel>>();
            if (!string.IsNullOrEmpty(rules) && !string.IsNullOrEmpty(oids))
            {
                var couponRules = JsonConvert.DeserializeObject<List<GetPCodeModel>>(rules);
                var oidIntList = GetIntList(oids);
                var prodcuts = _ProductLibraryConfigManage.QueryProducts(oidIntList);
                if (prodcuts != null && prodcuts.Any() && oidIntList.Any())
                {
                    unavaiableProducts = _ProductLibraryConfigManage.GetUnavailableCouponProducts(prodcuts, couponRules, grossProfit);
                    var hasExistsCouponOids = new List<int>();
                    foreach (var rule in couponRules)
                    {
                        prodcuts = prodcuts.Where(p => !hasExistsCouponOids.Exists(t => t == p.Oid));
                        var vaiableProductsItem = _ProductLibraryConfigManage.GetAvailableCouponProducts(prodcuts, rule, grossProfit);
                        hasExistsCouponOids.AddRange(vaiableProductsItem.Select(s=>s.Oid));
                        if (vaiableProductsItem.Any())
                            avaiableRuleProducts[rule.Minmoney.ToString() + "/" + rule.Discount.ToString()] = vaiableProductsItem;
                    }
                }
            }

            return new ContentResult
            {
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new
                {
                    unavaiableProducts = unavaiableProducts,
                    avaiableProducts = avaiableRuleProducts
                }),
                ContentType = "application/json"
            };
        }


        public JsonResult VerifyPrice(string oids,decimal price)
        {
            var result = false;
            if (!string.IsNullOrEmpty(oids))
            {
                var oidIntList = GetIntList(oids);
                var prodcuts = _ProductLibraryConfigManage.QueryProducts(oidIntList);
                if (prodcuts != null && prodcuts.Any())
                {
                    var productPrice = prodcuts.Where(_ => _.cy_list_price > 0);
                    if (productPrice.Any())
                    {
                        var minPrice = productPrice.Min(x => x.cy_list_price);
                        result = (minPrice * 10) >= price;
                    }
                    else
                    {
                        result = true;
                    }                  
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建母券和子券
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CreateCoupon(Dictionary<string, List<string>> products,
            InsertOrUpdateCouponRulePara parentCoupon, InsertOrUpdateGetCouponRulePara childCoupon, 
            List<InsertOrUpdateGetCouponRulePara> rules)
        {
            var result = new Dictionary<string, int>();
            if (rules != null && rules.Any())
            {
                using (var memClient = new PromotionClient())
                {
                    var mark = 1;
                    foreach (var rule in rules)
                    {
                        var key = rule.Minmoney + "/" + rule.Discount;
                        if (products.ContainsKey(key) && products[key].Any())
                        {
                            parentCoupon.ProductID = products[key];
                            if (rules.Count > 1)
                                parentCoupon.Name += "-" + mark;
                            var createParentResult = await memClient.InsertOrUpdateCouponRulesAsync(parentCoupon);
                            var ruleId = createParentResult.Success ? createParentResult.Result : -1;
                            if (ruleId > 0)
                            {
                                childCoupon.RuleID = ruleId;
                                childCoupon.Minmoney = rule.Minmoney;
                                childCoupon.Discount = rule.Discount;
                                childCoupon.PromtionName = rule.PromtionName;
                                childCoupon.Description = rule.Description;
                                var createChildResult = await memClient.InsertOrUpdateGetCouponRuleAsync(childCoupon);
                                var getRulePkid = createChildResult.Success ? createChildResult.Result.Item1 : -1;
                                if (getRulePkid > 0)
                                {
                                    result[rule.Minmoney + "/" + rule.Discount] = getRulePkid;
                                }
                            }
                            mark++;
                        }                      
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentUseSetting()
        {
            var result = new List<DepartmentAndUse>();

            var setting = PromotionManager.GetDepartmentUseSetting();
            if (setting != null && setting.Rows.Count > 0)
            {
                result = setting.Rows.OfType<System.Data.DataRow>().Select(s =>
                new DepartmentAndUse
                {
                    ParentSettingId = s["ParentSettingId"].ToString(),
                    SettingId = s["SettingId"].ToString(),
                    DisplayName = s["DisplayName"].ToString(),
                    Type = s["Type"].ToString()
                }).ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCouponRule()
        {
            return View();
        }

        public ActionResult BatchDeleteCoupon()
        {
            return View();
        }

        public ActionResult GetProduct(int oid)
        {
            var data = _ProductLibraryConfigManage.QueryProduct(oid) ?? null;
            return Content(JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 产品优惠券配置数据列表
        /// </summary>
        /// <param name="search">查询对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProductCouponList(SeachProducts search)
        {
            search = search ?? new SeachProducts();
            if (search.PageIndex <= 0)
            {
                search.PageIndex = 1;
            }
            if (search.PageSize <= 0)
            {
                search.PageSize = 100;
            }
            ViewBag.pageIndex = search.PageIndex;
            if (string.IsNullOrWhiteSpace(search.Category))
            {
                search.Category = "1";
            }
            ViewBag.ErrorPId = "";
            search.PidList = new List<string>();
            if (!string.IsNullOrWhiteSpace(search.PId))
            {
                var pids = search.PId.Split(',');
                foreach (var pid in pids)
                {
                    if (string.IsNullOrWhiteSpace(pid))
                    {
                        continue;
                    }
                    search.PidList.Add(pid.Trim());
                }
            }
            var products = QueryProductsForProductLibrary(search);

            //判断导入PID是否有错误
            if (search.PidList != null && search.PidList.Count > 1 && products != null && products.QueryProducts != null)
            {
                var getPIds = products.QueryProducts.Select(t => t.PID);
                if (getPIds != null && getPIds.Any())
                {
                    var strbErrorPId = new StringBuilder();
                    foreach (var searchPId in search.PidList)
                    {
                        if (!getPIds.Contains(searchPId.Trim()))
                        {
                            strbErrorPId.AppendFormat("{0},", searchPId);
                        }
                    }
                    if (strbErrorPId.Length > 0)
                    {
                        strbErrorPId = strbErrorPId.Remove(strbErrorPId.Length - 1, 1);
                    }
                    ViewBag.ErrorPId = strbErrorPId.ToString();
                }
            }

            if (products != null && products.QueryProducts != null && products.QueryProducts.Any())
            {
                IEnumerable<QueryProductsModel> queryResult = products.QueryProducts;
                if (!string.IsNullOrEmpty(search.SalePriceAfter))//筛选券后最低价
                {
                    var salePriceAfter = search.SalePriceAfter.Split('|');
                    if (salePriceAfter.Length == 2)
                    {
                        queryResult = queryResult.Where(x => x.PriceAfterCoupon >= decimal.Parse(salePriceAfter[0].Trim()) && x.PriceAfterCoupon <= decimal.Parse(salePriceAfter[1].Trim()));
                    }
                }
                else if (!string.IsNullOrEmpty(search.MaoliAfter))//筛选券后毛利
                {
                    var maoliAfter = search.MaoliAfter.Split('|');
                    if (maoliAfter.Length == 2)
                    {
                        queryResult = queryResult.Where(x => x.GrossProfit >= decimal.Parse(maoliAfter[0].Trim()) && x.GrossProfit <= decimal.Parse(maoliAfter[1].Trim()));
                    }
                }
                else if (!string.IsNullOrEmpty(search.Price))//筛选销售价
                {
                    var salePrice = search.Price.Split('|');
                    if (salePrice.Length == 2)
                    {
                        queryResult = queryResult.Where(x => x.cy_list_price >= decimal.Parse(salePrice[0].Trim()) && x.cy_list_price <= decimal.Parse(salePrice[1].Trim()));
                    }
                }
                else if (!string.IsNullOrEmpty(search.CostPrice))//筛选成本价
                {
                    var costPrice = search.CostPrice.Split('|');
                    if (costPrice.Length == 2)
                    {
                        queryResult = queryResult.Where(x => x.cy_cost >= decimal.Parse(costPrice[0].Trim()) && x.cy_cost <= decimal.Parse(costPrice[1].Trim()));
                    }
                }
                if (!string.IsNullOrEmpty(search.MaoliSort) && search.FiltrateType == "SalePriceAfter")
                {
                    if (search.MaoliSort.Contains("asc"))
                    {
                        queryResult = queryResult.OrderBy(x => x.PriceAfterCoupon.GetValueOrDefault());
                    }
                    else
                    {
                        queryResult = queryResult.OrderByDescending(x => x.PriceAfterCoupon.GetValueOrDefault());
                    }
                }
                else if (!string.IsNullOrEmpty(search.MaoliSort) && search.FiltrateType == "GrossMarginAfter")
                {
                    if (search.MaoliSort.Contains("asc"))
                    {
                        queryResult = queryResult.OrderBy(x => x.GrossProfit.GetValueOrDefault());
                    }
                    else
                    {
                        queryResult = queryResult.OrderByDescending(x => x.GrossProfit.GetValueOrDefault());
                    }
                }
                else if (!string.IsNullOrEmpty(search.MaoliSort) && search.FiltrateType == "SalePrice")
                {
                    if (search.MaoliSort.Contains("asc"))
                    {
                        queryResult = queryResult.OrderBy(x => x.cy_list_price);
                    }
                    else
                    {
                        queryResult = queryResult.OrderByDescending(x => x.cy_list_price);
                    }
                }
                else if (!string.IsNullOrEmpty(search.MaoliSort) && search.FiltrateType == "CostPrice")
                {
                    if (search.MaoliSort.Contains("asc"))
                    {
                        queryResult = queryResult.OrderBy(x => x.cy_cost);
                    }
                    else
                    {
                        queryResult = queryResult.OrderByDescending(x => x.cy_cost);
                    }
                }

                products.QueryProducts = queryResult.ToList();
            }

            ViewBag.SearchModel = search;
            if (products.ProductsTotalCount % search.PageSize == 0)
                ViewBag.TotalCount = products.ProductsTotalCount / search.PageSize;
            else
                ViewBag.TotalCount = products.ProductsTotalCount / search.PageSize + 1;

            ViewBag.ProductList = products.QueryProducts;
            ViewBag.pageSize = search.PageSize;
            return View();
        }

        #region 上传及获取上传文件内容
        /// <summary>
        /// 上传PID导入列表
        /// </summary>
        /// <param name="file">文件名</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InputProductIds()
        {
            try
            {
                var files = Request.Files;
                if (files == null || files.Count <= 0)
                {
                    return Json(new { result = false, Name = "", FileName = "", NewFileName = "", data = 0 });
                }
                var file = files[0];
                var fileName = file.FileName;
                var pointPosit = fileName.LastIndexOf('.');
                var name = fileName.Substring(0, pointPosit);
                var expandName = fileName.Substring(pointPosit, fileName.Length - pointPosit);
                var newFileName = "ProductPIdImput_" + DateTime.Now.ToString("yyyyMMddHHssmm") + expandName;
                var bb = file.ContentType;
                var folderPath = Server.MapPath("~/UploadFile/ProductLibraryConfig/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                file.SaveAs(folderPath + newFileName);
                var pidsList = GetImputPIdList(newFileName);
                var strPids = "";
                if (pidsList != null && pidsList.Count > 0)
                {
                    strPids = string.Join(",", pidsList);
                }
                if (pidsList.Count > 500)
                {
                    return Json(new { result = false, errorMessage = "单次导入不能超过500条", FileName = fileName, NewFileName = newFileName, Pids = strPids, Count = pidsList.Count });
                }
                return Json(new { result = true, errorMessage="", FileName = fileName, NewFileName = newFileName, Pids = strPids });
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "上传PID导入列表异常");
                return Json(new { result = false, errorMessage = "上传失败，请检查excel格式和状态！", Name = "", FileName = "", NewFileName = "", data = 0, Pids = "" });
            }
        }

      
        /// <summary>
        /// 获取上传的PID集合
        /// </summary>
        /// <param name="uploadFileName"></param>
        /// <returns></returns>
        public List<string> GetImputPIdList(string uploadFileName)
        {
            var pidList = new List<string>();

            var file = Server.MapPath("~/UploadFile/ProductLibraryConfig/") + uploadFileName;
            FileStream fileStrema = new FileStream(file, FileMode.Open);
            DataTable inputDt = null ;
            using (Common.ProvisioningExcelHelper excel = new Common.ProvisioningExcelHelper())
            {
                inputDt = excel.ExcelToDataTable(fileStrema, uploadFileName);
                //清楚空行
                inputDt = excel.ClearEmputyRow(inputDt);
                System.IO.File.Delete(file);
            }
            if (inputDt == null)
            {
                return pidList;
            }
            foreach (DataRow row in inputDt.Rows)
            {
                var values = row["PID"] != null ? row["PID"].ToString() : "";
                if (string.IsNullOrWhiteSpace(values.Trim())) continue;
                pidList.Add(values);
            }
            return pidList.Distinct().ToList();
        }
        #endregion


        /// <summary>
        /// 类别查询条件
        /// </summary>
        /// <returns></returns>
        public ActionResult CatetoryCondition(string category)
        {
            if (Session["ProductLibraryConfigPattern"] != null)
            {
                ViewBag.Pattern = Session["ProductLibraryConfigPattern"] as List<string>;
            }
            else
            {
                List<string> Pattern = _ProductLibraryConfigManage.GetPattern();
                Session["ProductLibraryConfigPattern"] = Pattern;
                ViewBag.Pattern = Pattern;
            }

            category = string.IsNullOrWhiteSpace(category) ? "1" : category;
            category = !category.EndsWith(".") ? category + "." : category;

            List<FilterConditionModel> brandList = null;
            HashSet<string> cpTabList = null;
            List<FilterConditionModel> tireRimList = null;
            try
            {
                System.Data.DataSet _FilterConditionItems = _ProductLibraryConfigManage.GetFilterCondition(category);
                if (_FilterConditionItems == null || _FilterConditionItems.Tables == null || _FilterConditionItems.Tables.Count <= 0)
                {
                    ViewBag.CP_BrandList = new List<FilterConditionModel>();
                    ViewBag.CP_tabList = new HashSet<string>();
                    ViewBag.CP_Tire_RimList = new List<FilterConditionModel>();
                    return View();
                }
                if (_FilterConditionItems.Tables[0] != null && _FilterConditionItems.Tables[0].Rows.Count > 0)
                {
                    brandList = _FilterConditionItems.Tables[0].ConvertTo<FilterConditionModel>().ToList() ?? new List<FilterConditionModel>();
                }
                if (_FilterConditionItems.Tables[2] != null && _FilterConditionItems.Tables[2].Rows.Count > 0)
                {
                    tireRimList = _FilterConditionItems.Tables[2].ConvertTo<FilterConditionModel>().ToList() ?? new List<FilterConditionModel>();
                }
                if (_FilterConditionItems.Tables[1] != null && _FilterConditionItems.Tables[1].Rows.Count > 0)
                {
                    IEnumerable<FilterConditionModel> CP_tabList = _FilterConditionItems.Tables[1].ConvertTo<FilterConditionModel>() ?? new List<FilterConditionModel>();

                    List<string> tabList = new List<string>();
                    foreach (var item in CP_tabList)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Name))
                        {
                            string[] tabArr = item.Name.Split(';');
                            foreach (var itemarr in tabArr)
                            {
                                tabList.Add(itemarr);
                            }
                        }
                    }
                    cpTabList = new HashSet<string>(tabList);
                }
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "类别查询条件异常");
            }
            ViewBag.CP_BrandList = brandList ?? new List<FilterConditionModel>();
            ViewBag.CP_tabList = cpTabList ?? new HashSet<string>();
            ViewBag.CP_Tire_RimList = tireRimList ?? new List<FilterConditionModel>();
            return View();
        }

        /// <summary>
        /// 获取筛选产品
        /// </summary>
        public static QueryProductsListModel QueryProducts(SeachProducts model)
        {
            if (!string.IsNullOrWhiteSpace(model.Category))
            {
                model.Category = !model.Category.EndsWith(".") ? model.Category + "." : model.Category;

                QueryProductsListModel _QueryProductsListModel = new QueryProductsListModel();

                var data = _ProductLibraryConfigManage.QueryProductsForProductLibrary(model, out int totalCount);
                
                if (data != null)
                {
                    var pageCount = 0;
                    if (totalCount % model.PageSize == 0)
                        pageCount = totalCount / model.PageSize;
                    else
                        pageCount = (totalCount / model.PageSize) + 1;
                    data.ToList().ForEach(w =>
                    {
                        w.PageCount = pageCount;
                    });

                    _QueryProductsListModel.QueryProducts = data.OrderByDescending(x=>x.Maoli).ToList();
                    string sessionKey = $"ProductLibraryConfigBrandList{model.Category}";
                    using (var client = CacheHelper.CreateCacheClient(nameof(ProductLibraryConfigController)))
                    {
                        var QueryProductsListModel = client.GetOrSet<QueryProductsListModel>(sessionKey, () =>
                        {
                            var queryProductsListModel = new QueryProductsListModel();
                            System.Data.DataSet _FilterConditionItems =
                                _ProductLibraryConfigManage.GetFilterCondition(model.Category);
                            if (_FilterConditionItems != null && _FilterConditionItems.Tables.Count <= 3)
                            {
                                if (_FilterConditionItems.Tables[0] != null && _FilterConditionItems.Tables[0].Rows.Count > 0)
                                {
                                    queryProductsListModel.CP_BrandList = _FilterConditionItems.Tables[0].ConvertTo<FilterConditionModel>().ToList() ?? new List<FilterConditionModel>();
                                }

                                if (_FilterConditionItems.Tables[2] != null && _FilterConditionItems.Tables[2].Rows.Count > 0)
                                {
                                    queryProductsListModel.CP_Tire_RimList = _FilterConditionItems.Tables[2].ConvertTo<FilterConditionModel>().ToList() ?? new List<FilterConditionModel>();
                                }

                                if (_FilterConditionItems.Tables[1] != null && _FilterConditionItems.Tables[1].Rows.Count > 0)
                                {
                                    IEnumerable<FilterConditionModel> CP_tabList = _FilterConditionItems.Tables[1].ConvertTo<FilterConditionModel>() ?? new List<FilterConditionModel>();

                                    List<string> tabList = new List<string>();
                                    foreach (var item in CP_tabList)
                                    {
                                        if (!string.IsNullOrWhiteSpace(item.Name))
                                        {
                                            string[] tabArr = item.Name.Split(';');
                                            foreach (var itemarr in tabArr)
                                            {
                                                tabList.Add(itemarr);
                                            }
                                        }
                                    }
                                    queryProductsListModel.CP_tabList = new HashSet<string>(tabList);
                                }
                            }
                            return queryProductsListModel;
                        }, TimeSpan.FromHours(1)).Value;
                        QueryProductsListModel.QueryProducts = _QueryProductsListModel.QueryProducts;
                        return QueryProductsListModel;
                    }
                }
                return _QueryProductsListModel;
            }
            return null;
        }

        /// <summary>
        /// 查询产品库优惠券配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static QueryProductsListModel QueryProductsForProductLibrary(SeachProducts model)
        {
            if (string.IsNullOrWhiteSpace(model.Category))
            {
                return null;
            }
            model.Category = !model.Category.EndsWith(".") ? model.Category + "." : model.Category;
            var queryProductModel = new QueryProductsListModel();
            var data = _ProductLibraryConfigManage.QueryProductsForProductLibrary(model, out int queryProductTotalCount);
            if (data == null)
            {
                return queryProductModel;
            }
            queryProductModel.ProductsTotalCount = queryProductTotalCount;
            queryProductModel.QueryProducts = data.OrderByDescending(x => x.Maoli).ToList();
            return queryProductModel;
        }

        /// <summary>
        /// 获取商品分类
        /// </summary>
        private string ProductCategories()
        {
            List<ZTreeModel> productCategories = null;
            if (Session["_productCategories"] == null)
            {
                productCategories = _OperationCategoryManager.SelectProductCategories().ToList() ?? null;
                if (productCategories != null)
                    Session["_productCategories"] = productCategories;
            }
            else
                productCategories = (List<ZTreeModel>)Session["_productCategories"] ?? null;

            return JsonConvert.SerializeObject(productCategories).Replace("isChecked", "checked");
        }

        public static List<int> GetIntList(string str)
        {
            var result = new List<int>();

            if (!string.IsNullOrEmpty(str))
            {
                var strList = str.Split(',');
                foreach (var item in strList)
                {
                    int intItem = -1;
                    if (Int32.TryParse(item, out intItem))
                    {
                        if (intItem > 0)
                        {
                            result.Add(intItem);
                        }
                    }
                }
            }

            return result;
        }

        private class ComparerForBrand : IEqualityComparer<QueryProductsModel>
        {
            public bool Equals(QueryProductsModel p1, QueryProductsModel p2)
            {
                if (p1 == null)
                    return p2 == null;
                return p1.CP_Brand == p2.CP_Brand;
            }

            public int GetHashCode(QueryProductsModel p)
            {
                return 0;
            }
        }

        private class ComparerForRim : IEqualityComparer<QueryProductsModel>
        {
            public bool Equals(QueryProductsModel p1, QueryProductsModel p2)
            {
                if (p1 == null)
                    return p2 == null;
                return p1.CP_Tire_Rim == p2.CP_Tire_Rim;
            }

            public int GetHashCode(QueryProductsModel p)
            {
                return 0;
            }
        }

        private class ComparerForTab : IEqualityComparer<QueryProductsModel>
        {
            public bool Equals(QueryProductsModel p1, QueryProductsModel p2)
            {
                if (p1 == null)
                    return p2 == null;
                return p1.CP_Tab == p2.CP_Tab;
            }

            public int GetHashCode(QueryProductsModel p)
            {
                return 0;
            }
        }

        public ActionResult Loglist(string obj)
        {
            return View(LoggerManager.SelectOprLogByObjectType(obj));
        }

        public void AddOprLog(string before, string after, string opr)
        {
            OprLog oprModel = new OprLog();
            oprModel.BeforeValue = before;
            oprModel.AfterValue = after;
            oprModel.Author = User.Identity.Name;
            oprModel.ChangeDatetime = DateTime.Now;
            oprModel.HostName = Request.UserHostName;
            oprModel.ObjectType = "PLC";
            oprModel.Operation = opr;
            OprLogManager.AddOprLog(oprModel);
        }

        public ActionResult EditCoupon()
        {
            return View();
        }

        public ActionResult CouponVlidateForPKID(int couponRulePKID,int oid)
        {
            return JavaScript(_ProductLibraryConfigManage.CouponVlidateForPKID(couponRulePKID,oid));
        }

        public ActionResult CouponValidateForPKIDs(List<int> pkids, int oid)
        {
            var result = new List<string>();
            foreach (var item in pkids)
            {
              result.Add( _ProductLibraryConfigManage.CouponVlidateForPKID(item, oid));
            }
            return JavaScript(JsonConvert.SerializeObject(result));
        }

        public JsonResult RefreshProductSalespredictBIData()
        {
            return Json(_ProductLibraryConfigManage.RefreshProductSalespredictBIData().Count > 0,
                JsonRequestBehavior.AllowGet);
        }

    }
}