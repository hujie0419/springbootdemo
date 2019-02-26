using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.Activity;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Member;

namespace Tuhu.Provisioning.Business.ProductLibraryConfing
{
    public class ProductLibraryConfigManage
    {
        #region Private Fields
        static string strConn = ProcessConnection.ProcessConnectionString("Gungnir", false); //ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("ProductLibraryConfig");
        private ProductLibraryConfigHandler handler = null;
        private static readonly IConnectionManager ConnectionManagerBI = new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_BI"].ConnectionString);
        private readonly IDBScopeManager dbScopeManagerBI = new DBScopeManager(ConnectionManagerBI);
        #endregion

        public ProductLibraryConfigManage()
        {
            handler = new ProductLibraryConfigHandler(DbScopeManager);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(ProductLibraryConfigModel model)
        {
            try
            {
                return handler.Add(model);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(ProductLibraryConfigModel model)
        {
            try
            {
                return handler.Update(model);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool BatchDelete(string Idlist)
        {
            try
            {
                return handler.BatchDelete(Idlist);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public IEnumerable<ProductLibraryConfigModel> GetList(string strWhere)
        {
            try
            {
                return handler.GetList(strWhere);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }

        public IEnumerable<ProductLibraryConfigModel> GetProductCouponConfigByOids(List<int> oids)
        {
            IEnumerable<ProductLibraryConfigModel> result = null;

            try
            {
                result = handler.GetProductCouponConfigByOids(oids);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetProductCouponConfigByOids异常");
            }

            return result;
        }

       
        /// <summary>
        /// 根据查询条件获取产品库优惠券数据列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<QueryProductsModel> QueryProductsForProductLibrary(SeachProducts model,out int totalCount)
        {
            totalCount = 0;
            try
            {
                List<QueryProductsModel> result = handler.QueryProductsForProductLibrary(model).ToList();
                totalCount = handler.QueryProductsForProductLibraryCount(model);
                if (!result.Any())
                {
                    return result ?? new List<QueryProductsModel>();
                }
                var pids = new List<string>(result.Count);
                var rulePkIds = new Dictionary<int, bool>(result.Count);
                result.ForEach(r =>
                {
                    pids.Add(r.PID);
                    r.CouponIds?.Split(',')?.ToList().ForEach(t =>
                    {
                        int.TryParse(t, out int id);
                        if (id > 0 && !rulePkIds.ContainsKey(id))
                            rulePkIds.Add(id, true);
                    });
                });
                var data = QueryProductSalesInfo(pids);
                var rulesDic = ActivityManager.GetPCodeModelByRulePKIDS(rulePkIds.Keys);
                if (data == null) data = new Dictionary<string, ProductSalesPredic>();
                foreach (var x in result)
                {
                    data.TryGetValue(x.PID, out ProductSalesPredic p);
                    x.cy_list_price = p?.OfficialWebsitePrice ?? 0;
                    x.cy_cost = p?.Cost ?? 0;
                    x.Maoli = x.cy_list_price - x.cy_cost;
                    var useCouponEffects = new List<UseCouponEffect>();
                    x.CouponIds?.Split(',')?.ToList().ForEach(t =>
                    {
                        int.TryParse(t, out int id);
                        if (id > 0 && rulesDic.TryGetValue(id, out GetPCodeModel rule))
                        {
                            if (rule != null)
                            {
                                var useCouponEffect = CalculateUseCouponEffect(x, rule);
                                if (useCouponEffect != null)
                                {
                                    useCouponEffects.Add(useCouponEffect);
                                }
                            }
                        }
                    });
                    if (useCouponEffects.Any())
                    {
                        x.UseCouponEffects = useCouponEffects;
                        x.PriceAfterCoupon = useCouponEffects.Min(t => t.PriceAfterCoupon.GetValueOrDefault());
                        x.GrossProfit = useCouponEffects.Min(t => t.GrossProfit.GetValueOrDefault());
                    }
                };
                return result;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return new List<QueryProductsModel>();
            }
        }

            /// <summary>
            /// 查询产品销售信息
            /// </summary>
            /// <param name="pids"></param>
            /// <returns></returns>
            public Dictionary<string, ProductSalesPredic> QueryProductSalesInfo(List<string> pids)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                long count = 0;
                using (var client = CacheHelper.CreateHashClient("ProductSalespredictBIData",TimeSpan.FromHours(4)))
                {
                    count = client.Count().Value;
                    if (count > 0)
                    {
                        var dic = client.Get(pids);
                        if (dic.Success)
                        {
                            return dic.Value.ToDictionary(k => k.Key, v => v.Value.To<ProductSalesPredic>());
                        }
                    }
                }
                if (count == 0)
                {
                    return RefreshProductSalespredictBIData();
                }
                return new Dictionary<string, ProductSalesPredic>();
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return new Dictionary<string, ProductSalesPredic>();
            }
        }

        public Dictionary<string, ProductSalesPredic> RefreshProductSalespredictBIData()
        {
           
            try
            {
                var list = dbScopeManagerBI.Execute(
                    conn => ProductLibraryConfigDAL.QueryProductSalesInfo(conn));
                var data = list.GroupBy(x => x.PID).ToDictionary(k => k.Key, v => v.FirstOrDefault());
                int pageSize = 1000;
                int pageTotal = (data.Count - 1) / pageSize + 1;
                //每次存进去1000条
                for (var pageIndex = 1; pageIndex <= pageTotal; pageIndex++)
                {
                    var result = data.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                        .ToDictionary(x => x.Key, x => (object)x.Value);
                    using (var client = CacheHelper.CreateHashClient("ProductSalespredictBIData",TimeSpan.FromDays(1)))
                    {
                        client.Set(result);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {

                logger.Log(Level.Error, ex.ToString());
            }
            return new Dictionary<string, ProductSalesPredic>();
        }


        /// <summary>
        /// 获取产品信息
        /// </summary>
        public QueryProductsModel QueryProduct(int oid)
        {
            QueryProductsModel result = null;
            try
            {
                result= handler.QueryProduct(oid);
                if (result != null)
                {
                    var data = dbScopeManagerBI.Execute(conn => ProductLibraryConfigDAL.QueryProductSalesInfoByPID(conn, result.PID));
                    if (data != null)
                    {
                        result.cy_list_price = data.OfficialWebsitePrice;
                        result.cy_cost = data.Cost;
                        result.Maoli = data.OfficialWebsitePrice - data.Cost;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
            return result;
        }
		
        public IEnumerable<QueryProductsModel> QueryProducts(List<int> oids)
        {
            IEnumerable<QueryProductsModel> result = new List<QueryProductsModel>();
            try
            {
                result= handler.QueryProducts(oids);
                if (result != null && result.Any())
                {
                    var data = QueryProductSalesInfo(result.Select(x => x.PID).ToList());
                    if (data != null)
                    {
                        result?.ToList().ForEach(x =>
                        {
                            data.TryGetValue(x.PID, out ProductSalesPredic p);
                            x.cy_list_price = p?.OfficialWebsitePrice ?? 0;
                            x.cy_cost = p?.Cost ?? 0;
                            x.Maoli = x.cy_list_price - x.cy_cost;
                            x.Maoli = x.cy_list_price - x.cy_cost;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
            return result;
        }
		
        public List<string> GetPattern()
        {
            try
            {
                return handler.GetPattern();
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 批量修改或添加产品
        /// </summary>
        public MsgResultModel BatchAddCoupon(string oids, string coupons, int isBatch)
        {
            MsgResultModel _MsgResultModel = new MsgResultModel();
            try
            {
                return handler.BatchAddCoupon(oids, coupons, isBatch);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());

                _MsgResultModel.State = 0;
                _MsgResultModel.Message = ex.ToString();
                return _MsgResultModel;
            }
        }

        public bool UpdateProductCouponConfig(int oid, List<int> couponPkids)
        {
            bool result = false;

            try
            {
                result = handler.UpdateProductCouponConfig(oid, couponPkids);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "更新单个产品优惠券信息失败");
            }

            return result;
        }

        public MsgResultModel BatchDeleteCoupon(string oids, string coupons)
        {
            MsgResultModel _MsgResultModel = new MsgResultModel();
            try
            {
                return handler.BatchDeleteCoupon(oids, coupons);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());

                _MsgResultModel.State = 0;
                _MsgResultModel.Message = ex.ToString();
                return _MsgResultModel;
            }
        }

        public bool RefreshCache(string oids)
        {
            using (var client = new Tuhu.Service.Member.CacheClient())
            {
                var response = client.RefreshOidsMatchPromotionCache(oids
                    .Split(new string[] {",", "，"}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)
                    .ToList());
                return response.Success;
            }
        }

        /// <summary>
        /// 获取 品牌，标签，尺寸 
        /// </summary>
        public System.Data.DataSet GetFilterCondition(string category)
        {
            try
            {
                return ProductLibraryConfigDAL.GetFilterCondition(category);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }

        public IEnumerable<QueryProductsModel> CalculateUseCouponEffects(IEnumerable<QueryProductsModel> products)
        {
            var ruleIdStrs = products.Select(t => t.CouponIds).Where(t => t != null);
            var rulePkIds = new List<int>();
            foreach (var str in ruleIdStrs)
            {
                var idList = str.Split(',').Where(t => !string.IsNullOrEmpty(t));
                rulePkIds.AddRange(idList.Select(t =>
                {
                    int id = -1;
                    Int32.TryParse(t, out id);
                    return id;
                }).Where(t => t > 0));
            }
            var rules = new List<GetPCodeModel>();
            var distinctPkids = rulePkIds.Distinct();
            foreach (var ruleId in distinctPkids)
            {
                var rule = GetGeCouponRulesByRulePkid(ruleId);
                if (rule != null)
                {
                    rules.Add(rule);
                }
            }
            foreach (var p in products)
            {
                var ruleList = p.CouponIds?.Split(',');
                if (ruleList != null)
                {
                    var useCouponEffects = new List<UseCouponEffect>();
                    foreach (var item in ruleList)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var rule = rules.FirstOrDefault(t => t.GETPKID == Convert.ToInt32(item));
                            if (rule != null)
                            {
                                var useCouponEffect = CalculateUseCouponEffect(p, rule);
                                if (useCouponEffect != null)
                                {
                                    useCouponEffects.Add(useCouponEffect);
                                }
                            }
                        }
                    }
                    if (useCouponEffects.Any())
                    {
                        p.UseCouponEffects = useCouponEffects;
                        p.PriceAfterCoupon = useCouponEffects.Min(t => t.PriceAfterCoupon.GetValueOrDefault());
                        p.GrossProfit = useCouponEffects.Min(t => t.GrossProfit.GetValueOrDefault());
                    }
                }
            }

            return products;
        }

        /// <summary>
        /// 返回哪些产品不能用相应的优惠券
        /// </summary>
        /// <param name="products"></param>
        /// <param name="couponRules"></param>
        /// <param name="grossProfit"></param>
        /// <returns></returns>
        public  Dictionary<string, IEnumerable<GetPCodeModel>> GetUnavailableCoupons(
            IEnumerable<QueryProductsModel> products, List<GetPCodeModel> couponRules, 
            decimal grossProfit)
        {
            var result = new Dictionary<string, IEnumerable<GetPCodeModel>>();
            foreach (var p in products)
            {
                var unavailableCoupons = new List<GetPCodeModel>();
                foreach (var rule in couponRules)
                {
                    var useCouponEffect = CalculateUseCouponEffect(p, rule);
                    if (useCouponEffect == null || useCouponEffect.GrossProfit < grossProfit)
                    {
                        unavailableCoupons.Add(rule);
                    }
                }
                if (unavailableCoupons.Any())
                    result[p.Oid.ToString()] = unavailableCoupons;
            }

            return result;
        }
        /// <summary>
        /// 返回可用当前券的产品
        /// </summary>
        /// <returns></returns>
        public IEnumerable<QueryProductsModel> GetAvailableCouponProducts(IEnumerable<QueryProductsModel> products, 
            GetPCodeModel rule, decimal grossProfit)
        {
            var result = new List<QueryProductsModel>();

            foreach (var p in products)
            {
                var useCouponEffect = CalculateUseCouponEffect(p, rule);
                if (useCouponEffect != null && useCouponEffect.GrossProfit >= grossProfit)
                {
                    result.Add(p);
                }
            }

            return result;
        }

        public IEnumerable<QueryProductsModel> GetUnavailableCouponProducts(IEnumerable<QueryProductsModel> products,
            List<GetPCodeModel> couponRules, decimal grossProfit)
        {
            var result = new List<QueryProductsModel>();

            foreach (var p in products)
            {
                var isAnyCanUse = false;
                foreach (var rule in couponRules)
                {
                    var useCouponEffect = CalculateUseCouponEffect(p, rule);
                    if (useCouponEffect != null && useCouponEffect.GrossProfit >= grossProfit)
                    {
                        isAnyCanUse = true;
                        break;
                    }
                }

                if (!isAnyCanUse)
                {
                    result.Add(p);
                }
            }

            return result;
        }


        public static UseCouponEffect CalculateUseCouponEffect(QueryProductsModel product, GetPCodeModel rule)
        {
            UseCouponEffect result = null;
            if (product != null && rule != null)
            {
                int length = 10;
                if (product.PID.StartsWith("FU-",StringComparison.OrdinalIgnoreCase)) length = 22; //如果是喷漆服务，则可以多买很多
                for (var i = 1; i <= length; i++)
                {
                    var price = product.cy_list_price * i;                 
                    if (price >= rule.Minmoney || price <= 0)
                    {
                        var status = string.Empty;
                        var now = DateTime.Now;
                        if (rule.ValiStartDate != null && rule.ValiEndDate != null)
                        {
                            if (rule.ValiStartDate > now)
                            {
                                status = "NotStart";
                            }
                            else if (now >= rule.ValiStartDate && now < rule.ValiEndDate)
                            {
                                status = "OnGoing";
                            }
                            else if (now >= rule.ValiEndDate)
                            {
                                status = "Overdue";
                            }
                        }
                        else
                        {
                            if (rule.Term > 0)
                            {
                                status = "ValidDays";
                            }
                            else
                            {
                                status = "Other";
                            }
                        }
                        var priceAfterCoupon = decimal.Round((price - rule.Discount.GetValueOrDefault()) / i, 2);
                        result = new UseCouponEffect()
                        {
                            ProductCount = i,
                            CouponPkId = rule.GETPKID,
                            CouponDescription = rule.Description,
                            Discount = rule.Discount,
                            Minmoney = rule.Minmoney,
                            PriceAfterCoupon = priceAfterCoupon,
                            GrossProfit = priceAfterCoupon - product.cy_cost,
                            StartTime = rule.ValiStartDate,
                            EndTime = rule.ValiEndDate,
                            CouponDuration = rule.Term,
                            Status = status
                        };
                        break;
                    }
                }
            }          

            return result;
        }

        public  GetPCodeModel GetGeCouponRulesByRulePkid(int rulePkId)
        {
            return ActivityManager.GetPCodeModelByRulePKID(rulePkId);
        }

        public string CouponVlidateForPKID(int rulePkId, int oid)
        {
            JObject json = new JObject();
            var rules = ActivityManager.GetPCodeModelByRulePKID(rulePkId);
            var products = QueryProduct(oid);
            var result = CalculateUseCouponEffect(products, rules);
            if (result!=null)
            {
                json.Add("RuleID", result.CouponPkId.ToString());
                json.Add("Name", "");
                json.Add("Description", result.CouponDescription.ToString());
                json.Add("Minmoney", result.Minmoney.ToString());
                json.Add("Discount", result.Discount.ToString());
                json.Add("CouponStartTime", result.StartTime.ToString());
                json.Add("CouponEndTime", result.EndTime.ToString());
                json.Add("CouponDuration", result.CouponDuration.ToString());
                json.Add("GrossProfit", result.GrossProfit.ToString());
                json.Add("PriceAfterCoupon", result.PriceAfterCoupon.ToString());
                json.Add("Quantity", "");
                json.Add("SupportUserRange", "");
                json.Add("Status", result.Status);
            }
            else
            {
                json.Add("RuleID", "");
                json.Add("Name", "");
                json.Add("Description", "");
                json.Add("Minmoney", "");
                json.Add("Discount", "");
                json.Add("CouponStartTime", "");
                json.Add("CouponEndTime", "");
                json.Add("CouponDuration", "");
                json.Add("Quantity", "");
                json.Add("GrossProfit", "");
                json.Add("PriceAfterCoupon", "");
                json.Add("SupportUserRange", "");
                json.Add("Status", "");
            }
            return json.ToString();
        }
    }
}