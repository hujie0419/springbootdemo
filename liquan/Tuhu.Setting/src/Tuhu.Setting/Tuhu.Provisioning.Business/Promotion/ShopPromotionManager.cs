using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.DAO.Promotion;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Promotion
{
    public class ShopPromotionManager
    {
        public static ListModel<ShopCouponRulesModel> GetCouponList(string keywords, int? discount, string startDate,
            string endDate, int status, int pageIndex, int pageSize)
        {
            var result = DalShopPromotion.GetCouponList(keywords, discount, startDate, endDate, status, pageIndex,
                pageSize);
            var products = DalShopPromotion.GetCouponRuleProducts(result.Source.Select(x => x.RuleId).ToList());
            var productsDic = products.GroupBy(x => x.RuleId).ToDictionary(k => k.Key, v => v.ToList());

            foreach (var item in result.Source)
            {
                productsDic.TryGetValue(item.RuleId, out List<ShopCouponRuleProduct> p);
                item.Products = p;
            }
            return result;
        }
    }
}
