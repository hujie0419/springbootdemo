using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.PromotionTask.Dal;
using Tuhu.C.Job.PromotionTask.Model;

namespace Tuhu.C.Job.PromotionTask.Job
{
    [DisallowConcurrentExecution]
    class CouponRulesDataMoveJob : IJob
    {
        private static ILog CouponRulesDataMoveJobLogger = LogManager.GetLogger<CouponRulesDataMoveJob>();
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                CouponRulesDataMoveJobLogger.Info("CouponRulesDataMoveJob：job开始");
                DoJob();
                CouponRulesDataMoveJobLogger.Info("CouponRulesDataMoveJob：job结束");
            }
            catch (Exception ex)
            {
                CouponRulesDataMoveJobLogger.Error(ex);
            }


        }

        private void DoJob()
        {
            if (!DalCouponRules.CleanCouponRulesConfig())
            {
                CouponRulesDataMoveJobLogger.Warn("配置表数据清除失败");
            }
            var CouponRuleChild = DalCouponRules.GetCouponRulesChild();
            if (CouponRuleChild == null || !CouponRuleChild.Any())
            {
                CouponRulesDataMoveJobLogger.Info("CouponRuleChild没有数据，job结束");
            }
            Dictionary<int, Tuple<int, bool>> configTypeDic = new Dictionary<int, Tuple<int, bool>>();
            CouponRuleChild.OrderBy(o => o.ParentId).GroupBy(g => g.ParentId).ForEach(f =>
              {
                  var brands = f.Where(w => !string.IsNullOrWhiteSpace(w.Brand)).Select(s => new { ParentId = s.ParentId, Brand = s.Brand, Category = s.Category }).Distinct().Select(s => new CouponRulesModel { Brand = s.Brand, ParentId = s.ParentId, Category = s.Category });
                  var categorys = f.Where(w => !string.IsNullOrWhiteSpace(w.Category) && string.IsNullOrWhiteSpace(w.Brand)).Select(s => new { ParentId = s.ParentId, Category = s.Category }).Distinct().Select(s => new CouponRulesModel { ParentId = s.ParentId, Category = s.Category });
                  var productIds = f.Where(w => !string.IsNullOrWhiteSpace(w.ProductId)).Select(s => new { ParentId = s.ParentId, ProductId = s.ProductId }).Distinct().Select(s => new CouponRulesModel { ParentId = s.ParentId, ProductId = s.ProductId });
                  var shopIds = f.Where(w => w.ShopId > 0).Select(s => new { ParentId = s.ParentId, ShopId = s.ShopId }).Distinct().Select(s => new CouponRulesModel { ParentId = s.ParentId, ShopId = s.ShopId });
                  var shopTypes = f.Where(w => w.ShopType > 0).Select(s => new { ParentId = s.ParentId, ShopType = s.ShopType }).Distinct().Select(s => new CouponRulesModel { ParentId = s.ParentId, ShopType = s.ShopType });

                  var configType = (categorys.Any() ? 1 : 0) + (brands.Any() ? 2 : 0) + (productIds.Any() ? 4 : 0) + (shopTypes.Any() ? 8 : 0) + (shopIds.Any() ? 16 : 0);
                  bool pidType = f.FirstOrDefault(x=>!string.IsNullOrWhiteSpace(x.ProductId))?.PIDType ?? false;
                  if (!configTypeDic.ContainsKey(f.Key))
                      configTypeDic.Add(f.Key, Tuple.Create(configType, pidType));

                  brands?.ForEach(e =>
                  {
                      CreateBrandConfig(e);
                  });
                  categorys?.ForEach(e =>
                  {
                      CreateCategoryConfig(e);
                  });
                  productIds?.ForEach(e =>
                  {
                      CreatePidConfig(e);
                  });
                  shopIds?.ForEach(e =>
                  {
                      CreateShopIdConfig(e);
                  });
                  shopTypes?.ForEach(e =>
                  {
                      CreateShopTypeConfig(e);
                  });
                  if (!DalCouponRules.UpdateCouponRuleConfigType(f.Key, configType,pidType))
                      CouponRulesDataMoveJobLogger.Warn($"UpdateCouponRuleConfigType：更新领取规则主数据ConfigType时失败");
              });
            var result = CouponRulesDataHelper.Instanse.ExcuteDatas();
            if (result < 2)
            {
                CouponRulesDataMoveJobLogger.Warn($"DoJob：result={result}");
            }
        }

        private void CreateShopTypeConfig(CouponRulesModel e)
        {
            CouponRulesDataHelper.Instanse.AddData(new CouponRulesConfig
            {
                RuleID = e.ParentId,
                Type = 8,
                ConfigValue = e.ShopType?.ToString()
            }, 1);
        }

        private void CreateShopIdConfig(CouponRulesModel e)
        {
            CouponRulesDataHelper.Instanse.AddData(new CouponRulesConfig
            {
                RuleID = e.ParentId,
                Type = 16,
                ConfigValue = e.ShopId?.ToString()
            }, 1);
        }

        private void CreateCategoryConfig(CouponRulesModel e)
        {
            CouponRulesDataHelper.Instanse.AddData(new CouponRulesConfig
            {
                RuleID = e.ParentId,
                Type = 1,
                ConfigValue = e.Category
            }, 0);
        }
        private void CreateBrandConfig(CouponRulesModel e)
        {
            CouponRulesDataHelper.Instanse.AddData(new CouponRulesConfig
            {
                RuleID = e.ParentId,
                Type = 2,
                ConfigValue = $"{e.Category}|{e.Brand}"
            }, 0);
        }
        private void CreatePidConfig(CouponRulesModel e)
        {
            CouponRulesDataHelper.Instanse.AddData(new CouponRulesConfig
            {
                RuleID = e.ParentId,
                Type = 4,
                ConfigValue = e.ProductId
            }, 0);
        }
    }
}
