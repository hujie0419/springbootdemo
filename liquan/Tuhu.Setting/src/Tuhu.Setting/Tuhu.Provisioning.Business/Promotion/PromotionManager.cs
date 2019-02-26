using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.ConfigLog;

namespace Tuhu.Provisioning.Business.Promotion
{
    public class PromotionManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("PromotionManager");
        public static ListModel<PromotionModel> SelectAllPromotion(PromotionFilterConditionModel model) => DALPromotion.SelectAllPromotion(model);

        /// <summary>
        /// 获取某种类型的优惠券来的详细信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PromotionModel> SelectPromotionDetail(int id)
        {
            var dt = DALPromotion.SelectPromotionDetail(id);
            if (dt == null || dt.Rows.Count <= 0)
                return new PromotionModel[0];
            return dt.Rows.Cast<DataRow>().Select(row => new PromotionModel(row));
        }
        public static IEnumerable<GetPCodeModel> SelectGeCouponRulesByRuleID(int id)
        {
            var dt = DALPromotion.SelectGeCouponRulesByRuleID(id);
            if (dt == null || dt.Rows.Count <= 0)
                return new GetPCodeModel[0];
            return dt.Rows.Cast<DataRow>().Select(row => new GetPCodeModel(row));
        }
        /// <summary>
        /// 获取优惠券领取规则
        /// </summary>
        /// <param name="getRuleId"></param>
        /// <returns></returns>
        public static GetPCodeModel SelectGeCouponRulesByGetRuleID(int getRuleId)
        {
            var dt = DALPromotion.SelectGeCouponRulesByGetRuleID(getRuleId);
            if (dt == null || dt.Rows.Count <= 0)
                return null;
            return dt.Rows.Cast<DataRow>().Select(row => new GetPCodeModel(row)).FirstOrDefault();
        }
        public static ListModel<GetPCodeModel> SelectGeCouponRulesByCondition(int id, PromotionFilterConditionModel model) => DALPromotion.SelectGeCouponRulesByCondition(id, model);
        public static PromotionModel FetchPromotionByPKID(string id)
        {
            var dw = DALPromotion.FetchPromotionByPKID(id);
            return new PromotionModel(dw);
        }
        /// <summary>
        /// 获取产品的所有的分类
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Tuhu.Provisioning.DataAccess.Entity.Category> SelectProductCategory()
        {
            return Tuhu.Provisioning.DataAccess.Entity.Category.Parse(DALPromotion.SelectProductCategory());
        }

        public static IEnumerable<Tuhu.Provisioning.DataAccess.Entity.Category> SelectProductCategoryCategoryNameAndDisplayName()
        {
            var dt = DALPromotion.SelectProductCategoryCategoryNameAndDisplayName();
            if (dt == null || dt.Rows.Count <= 0)
                return new Tuhu.Provisioning.DataAccess.Entity.Category[0];

            var result = dt.Rows.Cast<DataRow>().Select(row => new Tuhu.Provisioning.DataAccess.Entity.Category
            {
                CategoryName = Convert.ToString(row["CategoryName"]),
                DisplayName = Convert.ToString(row["DisplayName"]),
                oid = Convert.ToInt32(row["oid"]),
                ParaentOid = Convert.ToInt32(row["ParaentOid"]),
                ChildrenCategory = new List<Tuhu.Provisioning.DataAccess.Entity.Category>()
            }).ToArray();
            foreach (var category in result)
            {
                category.ParentCategory = result.Where(c => category.ParaentOid == c.oid);
                category.ChildrenCategory = result.Where(c => c.ParaentOid == category.oid);
            }
            return result;
        }

        /// <summary>
        /// 获取产品品牌
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ProductBrand> SelectProductBrand(string type)
        {
            var dt = DALPromotion.SelectProductBrand(type);
            if (dt == null || dt.Rows.Count <= 0)
                return new List<ProductBrand>();
            return dt.Rows.Cast<DataRow>().Select(row => new ProductBrand(row));
        }

        public static int SavePromotionInfo(IEnumerable<PromotionModel> PromotionModel, string action, int ParentID, string shoptypes, string shopids)
        {
            return DALPromotion.SavePromotionInfoRule(PromotionModel, action, ParentID, shoptypes, shopids);
        }

        //public static int DeleteRecord(string type, int PKID)
        //{
        //	return DALPromotion.DeleteRecord(type,PKID);
        //}

        public static int SaveGetPCodeRule(GetPCodeModel model)
        {
            return DALPromotion.SaveGetPCodeRule(model);
        }
        public static int UpdateGetPCodeRule(GetPCodeModel model)
        {
            return DALPromotion.UpdateGetPCodeRule(model);
        }

        public static string FetchShopNameByID(int shopID) => DALPromotion.FetchShopNameByID(shopID);

        public IEnumerable<ExchangeCodeDetail> SelectGiftBag(int PageNumber, int PageSize, out int TotalCount)
        {
            DataTable dt = DALPromotion.SelectGiftBag(PageNumber, PageSize, out TotalCount);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
        }
        public IEnumerable<ExchangeCodeDetail> UpdateOEM(int pkid)
        {
            DataTable dt = DALPromotion.UpdateOEM(pkid);
            if (dt == null || dt.Rows.Count == 0)
                return new List<ExchangeCodeDetail>();
            return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
        }
        public IEnumerable<ExchangeCodeDetail> SelectPromotionDetails(int pkid)
        {
            DataTable dt = DALPromotion.SelectPromotionDetails(pkid);
            if (dt == null || dt.Rows.Count == 0)
                return new List<ExchangeCodeDetail>();
            return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
        }
        public DataTable SelectGiftByDonwLoad(int pkid)
        {
            DataTable dt = DALPromotion.SelectGiftByDonwLoad(pkid);
            return dt;
        }
        public string GenerateCoupon(int Number, int DetailsID)
        {
            return DALPromotion.GenerateCoupon(Number, DetailsID);

        }
        public object SelectCodeChannelByAddGift(int id)
        {
            return DALPromotion.SelectCodeChannelByAddGift(id);
        }
        public IEnumerable<ExchangeCodeDetail> SelectPromotionDetailsByEdit(int id)
        {
            DataTable dt = DALPromotion.SelectPromotionDetailsByEdit(id);
            if (dt == null || dt.Rows.Count == 0)
                return new List<ExchangeCodeDetail>();
            return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
        }
        /// <summary>
        /// 查询优惠券下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable SelectDropDownList()
        {
            return DALPromotion.SelectDropDownList();
        }
        public int AddOEM(ExchangeCodeDetail ecd)
        {
            return DALPromotion.AddOEM(ecd);
        }
        public int DoUpdateOEM(ExchangeCodeDetail ecd)
        {
            return DALPromotion.DoUpdateOEM(ecd);
        }
        public int SelectCountByDownLoad(int pkid)
        {
            return DALPromotion.SelectDownloadByPKID(pkid);
        }
        public int SelectPromoCodeCount(int pkid)
        {
            return DALPromotion.SelectPromoCodeCount(pkid);
        }
        public int DeleteGift(int pkid)
        {
            return DALPromotion.DeleteGift(pkid);
        }
        public DataTable CreateExcel(int pkid)
        {
            return DALPromotion.CreateExcel(pkid);
        }
        public int CreeatePromotion(ExchangeCodeDetail ecd)
        {
            return DALPromotion.CreeatePromotion(ecd);
        }
        public int UpdatePromotionDetailsByOK(ExchangeCodeDetail ecd)
        {
            return DALPromotion.UpdatePromotionDetailsByOK(ecd);
        }
        public int DeletePromoCode(int pkid)
        {
            return DALPromotion.DeletePromoCode(pkid);
        }
        public static DataTable GetDepartmentUseSetting()
        {
            return DALPromotion.GetDepartmentUseSetting();
        }
        public static DataTable GetDepartmentUseSettingNameBySettingId(int[] ids)
        {
            return DALPromotion.GetDepartmentUseSettingNameBySettingId(ids.ToList());
        }
        public static DataTable GetDepartmentUseSettingByParentId(int parentId)
        {
            return DALPromotion.GetDepartmentUseSettingByParentId(parentId);
        }
        public static DataTable GetDepartmentUseSettingNameBySettingId(int id)
        {
            return DALPromotion.GetDepartmentUseSettingNameBySettingId(id);
        }
        /// <summary>
        /// 获取部门和用途信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="settingId"></param>
        /// <returns></returns>
        public static DepartmentAndUseModel GetCouponDepartmentUseSettingBySettingId(int settingId)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return DALPromotion.GetCouponDepartmentUseSettingBySettingId(conn, settingId);
            }
        }
        /// <summary>
        /// 删除部门或用途
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool DeleteDepartmentUseSettingNameBySettingId(int id, string userName)
        {
            var CouponDepartUseInfo = GetCouponDepartmentUseSettingBySettingId(id);
            var result = DALPromotion.DeleteDepartmentUseSettingNameBySettingId(id);
            #region 新增日志
            if (result)
            {
                using (var log = new ConfigLogClient())
                {
                    log.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = id,
                        ObjectType = "CouponDepartmentAndUse",
                        BeforeValue = JsonConvert.SerializeObject(CouponDepartUseInfo),
                        AfterValue = "",
                        Operate = "删除",
                        Author = userName
                    }));
                }
            }
            #endregion
            return result;
        }
        /// <summary>
        /// 新增部门或用途
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool InsertDepartmentUseSettingName(DepartmentAndUseModel model, string userName)
        {
            var result = DALPromotion.InsertDepartmentUseSetting(model);
            #region 新增日志
            if (result)
            {
                using (var log = new ConfigLogClient())
                {
                    log.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.SettingId,
                        ObjectType = "CouponDepartmentAndUse",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Operate = "新增",
                        Author = userName
                    }));
                }
            }
            #endregion
            return result;
        }
        /// <summary>
        /// 修改部门或用途
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool UpdateDepartmentUseSetting(DepartmentAndUseModel model, string userName)
        {
            var CouponDepartUseInfo = GetCouponDepartmentUseSettingBySettingId(model.SettingId);
            var result = DALPromotion.UpdateDepartmentUseSetting(model);
            #region 新增日志
            if (result)
            {
                using (var log = new ConfigLogClient())
                {
                    log.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.SettingId,
                        ObjectType = "CouponDepartmentAndUse",
                        BeforeValue = JsonConvert.SerializeObject(CouponDepartUseInfo),
                        AfterValue = JsonConvert.SerializeObject(model),
                        Operate = "编辑",
                        Author = userName
                    }));
                }
            }
            #endregion
            return result;
        }
        public static int SaveCouponRuleInfo(PromotionModel PromotionModel, string action, string[] shoptypes, string[] shopids, string[] categorys, string[] brands, string[] pids)
        {
            PromotionModel.ConfigType = ((categorys ?? new string[] { }).Any() ? 1 : 0) +
                                        ((brands ?? new string[] { }).Any() ? 2 : 0) +
                                        ((pids ?? new string[] { }).Any() ? 4 : 0) +
                                        ((shoptypes ?? new string[] { }).Any() ? 8 : 0) +
                                        ((shopids ?? new string[] { }).Any() ? 16 : 0);
            switch (action)
            {
                case "add":
                    return DALPromotion.SaveCouponRuleInfo(PromotionModel, shoptypes, shopids, categorys, brands, pids);
                case "update":
                    #region 链接信息处理
                    //APP
                    if (PromotionModel.CustomSkipPage == "{{TempProductListRouteLink}}")
                        PromotionModel.CustomSkipPage = "/searchResult?ruleid=" + PromotionModel.PKID + "&s=";
                    else if (PromotionModel.CustomSkipPage == "{{GroupBuyingProductLink}}")
                        PromotionModel.CustomSkipPage = "/webView?url=https%3A%2F%2Fwx.tuhu.cn%2Fvue%2FGroupBuy%2Fpages%2Fsearch%2Fsearch%3FruleId%3D" + PromotionModel.PKID;
                    //小程序
                    if (PromotionModel.WxSkipPage == "{{TempProductListRouteLink}}")       //商品列表
                        PromotionModel.WxSkipPage = "/pages/search/search?flag=coupon&ruleId=" + PromotionModel.PKID;
                    else if (PromotionModel.WxSkipPage == "{{GroupBuyingProductLink}}")    //拼团商品列表
                        PromotionModel.WxSkipPage = "/pages/search/search?appid=wx25f9f129712845af&ruleId=" + PromotionModel.PKID;
                    //H5
                    if (PromotionModel.H5SkipPage == "{{TempProductListRouteLink}}")
                        PromotionModel.H5SkipPage = "http://wx.tuhu.cn/ChePin/CpList?ruleId=" + PromotionModel.PKID;
                    else if (PromotionModel.H5SkipPage == "{{GroupBuyingProductLink}}")
                        PromotionModel.H5SkipPage = "https://wx.tuhu.cn/vue/GroupBuy/pages/search/search?ruleId=" + PromotionModel.PKID;
                    #endregion
                    return DALPromotion.UpdateCouponRuleInfo(PromotionModel, shoptypes, shopids, categorys, brands, pids);
                default:
                    throw new Exception($"action={action}没有对应的处理方法");
            }
        }
        public static PromotionModel GetPromotionDetail(int id)
        {
            var dt = DALPromotion.GetPromotionDetail(id);
            if (dt == null || dt.Rows.Count <= 0)
                return new PromotionModel();
            var result = dt.Rows.Cast<DataRow>().Select(row => new PromotionModel(row)).FirstOrDefault();

            var proConfig = DALPromotion.GetCouponProductConfig(result.PKID);
            var shopConfig = DALPromotion.GetCouponShopConfig(result.PKID);
            result.ShopConfig = shopConfig.Rows.Cast<DataRow>().Select(row => new CouponRulesConfigShop(row)).ToList();
            result.ProductsConfig = proConfig.Rows.Cast<DataRow>().Select(row => new CouponRulesConfigProduct(row)).ToList();
            var pids = result.ProductsConfig.Where(x => x.Type == 4).Select(x => x.ConfigValue).ToList();
            if (pids.Any())
            {
                IAutoSuppliesManager manager = new AutoSuppliesManager();
                var names = manager.GetProductNamesByPids(pids);
                foreach (var p in result.ProductsConfig)
                {
                    if (p.Type == 4 && names.ContainsKey(p.ConfigValue))
                    {
                        p.ProductName = names[p.ConfigValue];
                    }
                }
            }

            return result;


        }
        public static IEnumerable<Tuple<string, string, bool>> GetProductsByPIDs(string[] pids)
        {
            var result = new List<Tuple<string, string, bool>>();
            var productTable = DALPromotion.SelectProductNamesByPIDs(pids);
            if (productTable != null)
                foreach (DataRow item in productTable.Rows)
                {
                    result.Add(Tuple.Create(item.GetValue<string>("PID"), item.GetValue<string>("DisplayName"), item.GetValue<bool>("Exist")));
                }
            pids?.ToList().ForEach(f =>
            {
                var product = result.Where(w => w.Item1 == f).FirstOrDefault();
                if (product == null)
                {
                    result.Add(Tuple.Create(f, "", false));
                }
            });
            return result;
        }
        public static IEnumerable<Tuple<int, string, bool>> GetShopsByShopIds(int[] shopIds)
        {
            var result = new List<Tuple<int, string, bool>>();
            var shopTable = DALPromotion.SelectShopNamesByIDs(shopIds);
            if (shopTable != null)
                foreach (DataRow item in shopTable.Rows)
                {
                    result.Add(Tuple.Create(item.GetValue<int>("PKID"), item.GetValue<string>("CarparName"), item.GetValue<bool>("Exist")));
                }
            shopIds?.ToList().ForEach(f =>
            {
                var shops = result.Where(w => w.Item1 == f).FirstOrDefault();
                if (shops == null)
                {
                    result.Add(Tuple.Create(f, "", false));
                }
            });
            return result;
        }
        public static IEnumerable<PromotionModel> SelectPromotionDetailNew(int id)
        {
            return DALPromotion.SelectPromotionDetailNew(id);
        }
        public static DataTable SelectExchangeCodeDetailByPage(int PageNumber, int PageSize, out int TotalCount)
        {
            return DALPromotion.SelectExchangeCodeDetailByPage(PageNumber, PageSize, out TotalCount);
        }

        public static int UpdatePromotionTaskStatus(int id, PromotionConsts.PromotionTaskStatusEnum taskStatus,
            string operateBy)
        {
            int result = DalPromotionJob.UpdatePromotionTaskStatus(id, taskStatus, operateBy);
            if (result > 0)
            {
                var promotionTask = DalPromotionJob.GetPromotionTaskById(id);
                if (promotionTask == null) return 0;
                if (promotionTask.IsImmediately == 1) //需要立即执行
                {
                    if (taskStatus == PromotionConsts.PromotionTaskStatusEnum.Executed)
                    {
                        TuhuNotification.SendNotification("ExecutePromotionTask.setting", new
                        {
                            PromotionTaskId = id
                        }, 10000);
                    }
                }
            }
            return result;
        }
        public static void ExecutBefore(SearchPromotionByCondition oneTask)
        {
            if (oneTask.TaskType == 1 && oneTask.SelectUserType == 3 && oneTask.PromotionTaskActivityId > 0)
            {
                SyncBiActivityData(oneTask);
            }
            else if (oneTask.TaskType == 1 && oneTask.SelectUserType == 2)
            {
                SyncOrderData(oneTask);
            }
        }


        static void SyncBiActivityData(SearchPromotionByCondition oneTask)
        {
            //判断是否已经同步过，同步过就不再执行
            var waitUsers = DalPromotionJob.ExistsPromotionTaskUsers(oneTask.PromotionTaskId);
            var historyUsers = DalPromotionJob.ExistsPromotionTaskHistoryUsers(oneTask.PromotionTaskId);
            if (waitUsers || historyUsers)
            {
                return;
            }

            //把BI表里的数据同步到待发送表里去
            DalPromotionJob.MovePromotionTaskActivityUsers(oneTask.PromotionTaskActivityId, oneTask.PromotionTaskId);
        }

        static void SyncOrderData(SearchPromotionByCondition oneTask)
        {
            //判断是否已经同步过，同步过就不再执行
            var waitUsers = DalPromotionJob.ExistsPromotionTaskUsers(oneTask.PromotionTaskId);
            var historyUsers = DalPromotionJob.ExistsPromotionTaskHistoryUsers(oneTask.PromotionTaskId);
            if (waitUsers || historyUsers)
            {
                return;
            }
            DalPromotionJob.MoveFilterOrderData(oneTask.PromotionTaskId);
        }

        public static DataTable GetPromotionLog(string objectId, string objectType)
        {
            return DALPromotion.GetPromotionLog(objectId, objectType);
        }
    }
}
