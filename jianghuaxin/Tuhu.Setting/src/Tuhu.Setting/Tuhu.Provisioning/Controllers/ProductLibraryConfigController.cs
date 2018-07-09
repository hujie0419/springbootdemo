using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ThBiz.DataAccess.Entity;
using Tuhu.Component.Common;
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

        public ActionResult Edit()
        {
            if (Session["ProductLibraryConfigPattern"] != null)
            {
                ViewBag.Pattern = Session["ProductLibraryConfigPattern"] as List<string>;
            }
            else
            {
                List<string> Pattern =  _ProductLibraryConfigManage.GetPattern();
                Session["ProductLibraryConfigPattern"] = Pattern;
                ViewBag.Pattern = Pattern;
            }
            
            
            int pageCount = 0;
            SeachProducts search = new SeachProducts()
            {
                category = Request.QueryString["Category"] ?? "Tires",
                brand = Request.QueryString["CP_Brand"] ?? "",
                tab = Request.QueryString["CP_Tab"] ?? "",
                rim = Request.QueryString["CP_Tire_Rim"] ?? "",
                couponIds = Request.QueryString["S_Coupon"] ?? "",
                price = Request.QueryString["S_Price"] ?? "",
                SalePriceAfter = Request.QueryString["SalePriceAfter"] ?? "",
                CostPrice = Request.QueryString["S_CostPrice"] ?? "",
                pid = Request.QueryString["S_PID"] ?? "",
                pattern = Request.QueryString["S_Figure"] ?? "",
                soft = Request.QueryString["S_PiceSoft"] ?? "",
                pageIndex = Request.QueryString["pageIndex"] != null ? int.Parse(Request.QueryString["pageIndex"]) : 1,
                pageSize = Request.QueryString["pageSize"] != null ? int.Parse(Request.QueryString["pageSize"]) : 100,
                onSale = Request.QueryString["onSale"] ?? "",
                maoli = Request.QueryString["maoli"] ?? "",
                MaoliAfter = Request.QueryString["maoliAfter"] ?? "",
                maoliSort = Request.QueryString["maoliSort"] ?? "",
                isShow = Request.QueryString["S_IsShow"] ?? "",
                FiltrateType = Request.QueryString["filtrateType"] ?? ""
            };

            var products = QueryProductsForProductLibrary(search);
            if (products != null && products.QueryProducts != null && products.QueryProducts.Any())
            {
                //products.QueryProducts = _ProductLibraryConfigManage.CalculateUseCouponEffects(products.QueryProducts).ToList(); //这一行的逻辑已经合并到query里去了，因为多个循环太慢了
                IEnumerable<QueryProductsModel> queryResult = products.QueryProducts;
                if (!string.IsNullOrEmpty(search.SalePriceAfter))//筛选券后最低价
                {
                    var salePriceAfter = search.SalePriceAfter.Split('|');
                    if (salePriceAfter.Length == 2)
                    {
                        queryResult = queryResult.Where(x => x.PriceAfterCoupon >= decimal.Parse(salePriceAfter[0].Trim()) && x.PriceAfterCoupon <= decimal.Parse(salePriceAfter[1].Trim()));
                    }
                }else if (!string.IsNullOrEmpty(search.MaoliAfter))//筛选券后毛利
                {
                    var maoliAfter = search.MaoliAfter.Split('|');
                    if (maoliAfter.Length == 2)
                    {
                        queryResult = queryResult.Where(x => x.GrossProfit >= decimal.Parse(maoliAfter[0].Trim()) && x.GrossProfit <= decimal.Parse(maoliAfter[1].Trim()));
                    }
                }else if (!string.IsNullOrEmpty(search.price))//筛选销售价
                {
                    var salePrice = search.price.Split('|');
                    if (salePrice.Length == 2)
                    {
                        queryResult = queryResult.Where(x => x.cy_list_price >= decimal.Parse(salePrice[0].Trim()) && x.cy_list_price <= decimal.Parse(salePrice[1].Trim()));
                    }
                }else if (!string.IsNullOrEmpty(search.CostPrice))//筛选成本价
                {
                    var costPrice = search.CostPrice.Split('|');
                    if (costPrice.Length == 2)
                    {
                        queryResult = queryResult.Where(x => x.cy_cost >= decimal.Parse(costPrice[0].Trim()) && x.cy_cost <= decimal.Parse(costPrice[1].Trim()));
                    }
                }
                if (!string.IsNullOrEmpty(search.maoliSort) && search.FiltrateType == "SalePriceAfter")
                {
                    if (search.maoliSort.Contains("asc"))
                    {
                        queryResult = queryResult.OrderBy(x => x.PriceAfterCoupon.GetValueOrDefault());
                    }
                    else
                    {
                        queryResult = queryResult.OrderByDescending(x => x.PriceAfterCoupon.GetValueOrDefault());
                    }
                }else if (!string.IsNullOrEmpty(search.maoliSort) && search.FiltrateType == "GrossMarginAfter")
                {
                    if (search.maoliSort.Contains("asc"))
                    {
                        queryResult = queryResult.OrderBy(x => x.GrossProfit.GetValueOrDefault());
                    }
                    else
                    {
                        queryResult = queryResult.OrderByDescending(x => x.GrossProfit.GetValueOrDefault());
                    }
                }else if (!string.IsNullOrEmpty(search.maoliSort) && search.FiltrateType == "SalePrice")
                {
                    if (search.maoliSort.Contains("asc"))
                    {
                        queryResult = queryResult.OrderBy(x => x.cy_list_price);
                    }
                    else
                    {
                        queryResult = queryResult.OrderByDescending(x => x.cy_list_price);
                    }
                }else if (!string.IsNullOrEmpty(search.maoliSort) && search.FiltrateType == "CostPrice")
                {
                    if (search.maoliSort.Contains("asc"))
                    {
                        queryResult = queryResult.OrderBy(x => x.cy_cost);
                    }
                    else
                    {
                        queryResult = queryResult.OrderByDescending(x => x.cy_cost);
                    }
                }
                
                products.QueryProducts = queryResult.ToList();
                if (products.QueryProducts.Count % search.pageSize == 0)
                    pageCount = products.QueryProducts.Count / search.pageSize;
                else
                    pageCount = (products.QueryProducts.Count / search.pageSize) + 1;
                products.QueryProducts = products.QueryProducts.Skip(search.pageSize * (search.pageIndex - 1))
                    .Take(search.pageSize).Select(
                        i =>
                        {
                            i.PageCount = pageCount;
                            return i;
                        }).ToList();
            }
            QueryProductsListModel model = products;
            ViewBag.pageIndex = Request.QueryString["pageIndex"] != null ? int.Parse(Request.QueryString["pageIndex"]) : 1;
            ViewBag.pageSize = Request.QueryString["pageSize"] != null ? int.Parse(Request.QueryString["pageSize"]) : 100;
            return View(model);
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
        /// 获取筛选产品
        /// </summary>
        public static QueryProductsListModel QueryProducts(SeachProducts model)
        {
            if (!string.IsNullOrWhiteSpace(model.category))
            {
                model.category = !model.category.EndsWith(".") ? model.category + "." : model.category;

                QueryProductsListModel _QueryProductsListModel = new QueryProductsListModel();
                var data = _ProductLibraryConfigManage.QueryProducts(model);
                if (data != null)
                {
                    _QueryProductsListModel.QueryProducts = data.OrderByDescending(x=>x.Maoli).ToList();
                    string sessionKey = $"ProductLibraryConfigBrandList{model.category}";
                    using (var client = CacheHelper.CreateCacheClient(nameof(ProductLibraryConfigController)))
                    {
                        var QueryProductsListModel = client.GetOrSet<QueryProductsListModel>(sessionKey, () =>
                        {
                            var queryProductsListModel = new QueryProductsListModel();
                            System.Data.DataSet _FilterConditionItems =
                                _ProductLibraryConfigManage.GetFilterCondition(model.category);
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

        public static QueryProductsListModel QueryProductsForProductLibrary(SeachProducts model)
        {
            if (!string.IsNullOrWhiteSpace(model.category))
            {
                model.category = !model.category.EndsWith(".") ? model.category + "." : model.category;
                QueryProductsListModel _QueryProductsListModel = new QueryProductsListModel();
                var data = _ProductLibraryConfigManage.QueryProductsForProductLibrary(model);
                
                if (data != null)
                {
                    _QueryProductsListModel.QueryProducts = data.OrderByDescending(x=>x.Maoli).ToList();//list as IEnumerable<QueryProductsModel>;
                    string sessionKey = $"ProductLibraryConfigBrandList{model.category}";
                    using (var client = CacheHelper.CreateCacheClient(nameof(ProductLibraryConfigController)))
                    {
                        var QueryProductsListModel = client.GetOrSet<QueryProductsListModel>(sessionKey, () =>
                        {
                            var queryProductsListModel = new QueryProductsListModel();
                            System.Data.DataSet _FilterConditionItems =
                                _ProductLibraryConfigManage.GetFilterCondition(model.category);
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