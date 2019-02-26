using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.Server.Manager.SalePromotion
{
    public class ProductHitDiscountManager
    {
        private string _userId;
        private List<DiscountActivityRequest> _requestProducts;
        private IEnumerable<SalePromotionActivityProduct> _activityProducts;
        private IEnumerable<SalePromotionActivityModel> _activities;
        private IEnumerable<SalePromotionActivityDiscount> _activityDiscountRules;
        private IEnumerable<IGrouping<string, CompProduct>> _productGroups;
        private List<ProductHitDiscountResponse> _returnList = new List<ProductHitDiscountResponse>();
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ProductHitDiscountManager));
        private bool _verifyPayAndInstallMethod = true;
        private bool _isCreateOrder;
        public ProductHitDiscountManager(List<DiscountActivityRequest> requestProducts, string userId,
            bool verifyPayAndInstallMethod = true, bool isCreateOrder = false)
        {
            this._requestProducts = requestProducts;
            this._userId = userId;
            this._verifyPayAndInstallMethod = verifyPayAndInstallMethod;
            this._isCreateOrder = isCreateOrder;
        }
        /// <summary>
        /// 查询产品命中的活动和打折信息
        /// </summary>
        /// <param name="products"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProductHitDiscountResponse>> GetProductHitDiscountInfo()
        {
            try
            {
                await InitBaseData();
                await ForEachActivityGroup();
                FillNotActivityProducts();
            }
            catch (Exception ex)
            {
                _logger.Error($"产品命中打折规则查询异常", ex);
            }
          
            return _returnList;
        }
        /// <summary>
        /// 查询DB和缓存，初始化数据成员
        /// </summary>
        /// <returns></returns>
        private async Task InitBaseData()
        {
            this._activityProducts = await DalSalePromotion.SelectSalePromotionActivityProducts(_requestProducts.Select(p => p.Pid));
            _activityProducts = _activityProducts.OrderByDescending(s => s.CreateDateTime).GroupBy(s => s.Pid).Select(s => s.FirstOrDefault());//保证每个产品对应一个活动
            var activityIds = _activityProducts.Select(t => t.ActivityId).ToList();
            var activitiesTask = DalSalePromotion.SelectSalePromotionActivity(activityIds);
            var activityDiscountRulesTask = DalSalePromotion.SelectSalePromotionActivityDiscounts(activityIds);
            await Task.WhenAll(activitiesTask, activityDiscountRulesTask);
            this._activities = activitiesTask.Result;
            this._activityDiscountRules = activityDiscountRulesTask.Result;
            this._productGroups = _requestProducts.Join(_activityProducts, x => x.Pid, y => y.Pid, (x, y) =>
            {
                return new CompProduct
                {
                    Pid = x.Pid,
                    RequestPrice = x.Price,
                    RequestNum = x.Num,
                    ActivityId = y.ActivityId,
                    LimitQuantity = y.LimitQuantity,
                    RequestPaymentMethod = x.PaymentMethod,
                    RequestInstallMethod = x.InstallMethod
                };
            }).GroupBy(g => g.ActivityId);
        } 
        /// <summary>
        /// 非活动产品
        /// </summary>
        /// <returns></returns>
        private void FillNotActivityProducts()
        {
            var notActivityProducts = _requestProducts.Where(s => !_activityProducts.ToList().Exists(a => string.Equals(a.Pid, s.Pid)));
            _returnList.AddRange(notActivityProducts.Select(p =>
            {
                return new ProductHitDiscountResponse()
                {
                    DiscountPrice = p.Price,
                    HasDiscountActivity = false,
                    DiscountRule = null,
                    IsHit = false,
                    Pid = p.Pid,
                    FailCode = nameof(ProductHitFailCode.NoneActivityDiscount),
                    FailMessage = ProductHitFailCode.NoneActivityDiscount,
                };
            }));
        }
        /// <summary>
        /// 遍历每个活动组
        /// </summary>
        /// <returns></returns>
        private async Task ForEachActivityGroup()
        {
            foreach (var group in _productGroups)
            {
                var currentActivityId = group.Key;
                var currentActivity = _activities.FirstOrDefault(s => string.Equals(s.ActivityId, currentActivityId));
                var currentActivityPids = group.Select(g => g.Pid).ToList();
                var currentActivityRequestNum = group.Sum(s => s.RequestNum);
                var currentActivityRequestTotalPrice = group.Sum(s => s.RequestNum * s.RequestPrice);
                var userBuyNum = await DiscountActivityInfoManager.GetOrSetUserActivityBuyNumCache(_userId, new List<string>() { currentActivityId });
                if (_isCreateOrder)
                {
                    await DiscountActivityInfoManager.IncreaseUserActivityBuyNumCache(_userId, currentActivityId, currentActivityRequestNum, TimeSpan.FromMinutes(1));
                }
                var currentActivityUserAlreadyBuyNum = userBuyNum?.FirstOrDefault()?.BuyNum ?? 0;

                if (currentActivity != null)
                {
                    var currentActivityRules = _activityDiscountRules.Where(s => string.Equals(currentActivityId, s.ActivityId)).ToList();
                    if (!currentActivityRules.Any())
                    {
                        _returnList.AddRange(group.Select(g => new ProductHitDiscountResponse()
                        {
                            DiscountPrice = g.RequestPrice,
                            HasDiscountActivity = false,
                            DiscountRule = null,
                            IsHit = false,
                            Pid = g.Pid,
                            FailCode = nameof(ProductHitFailCode.NoneActivityDiscount),
                            FailMessage = ProductHitFailCode.NoneActivityDiscount,
                        }));
                        continue;
                    }
                    if (currentActivity.Is_PurchaseLimit == 1 && currentActivity.LimitQuantity < currentActivityUserAlreadyBuyNum + currentActivityRequestNum)//当前活动用户限购
                    {
                        _returnList.AddRange(group.Select(g => new ProductHitDiscountResponse()
                        {
                            DiscountPrice = g.RequestPrice,
                            HasDiscountActivity = true,
                            DiscountRule = null,
                            IsHit = false,
                            Pid = g.Pid,
                            FailCode = nameof(ProductHitFailCode.UserBeLimited),
                            FailMessage = ProductHitFailCode.UserBeLimited,
                        }));
                        continue;
                    }
                    var productSoldNums = (await DiscountActivityInfoManager.GetOrSetActivityProductSoldNumCache(currentActivityId, currentActivityPids));
                    await ForEachActivityGroupProducts(group, productSoldNums, currentActivityRules,
                        currentActivity, currentActivityRequestNum, currentActivityRequestTotalPrice);
                }
            }
        }
        /// <summary>
        /// 遍历活动组内的每个产品
        /// </summary>
        /// <param name="group"></param>
        /// <param name="productSoldNums"></param>
        /// <param name="currentActivityRules"></param>

        private async Task ForEachActivityGroupProducts(IGrouping<string, CompProduct> group, IEnumerable<ActivityPidSoldNumModel> productSoldNums,
            List<SalePromotionActivityDiscount> currentActivityRules, SalePromotionActivityModel currentActivity,
            int currentActivityRequestNum, decimal currentActivityRequestTotalPrice)
        {
            var effictiveCurrentActivityRequestNumAndPrice = GetEffictiveCurrentActivityRequestNumAndPrice(group, productSoldNums, currentActivityRules, currentActivity,
                currentActivityRequestNum, currentActivityRequestTotalPrice);
            currentActivityRequestNum = effictiveCurrentActivityRequestNumAndPrice.Item1;
            currentActivityRequestTotalPrice = effictiveCurrentActivityRequestNumAndPrice.Item2;
            foreach (var product in group)
            {
                if (_isCreateOrder)
                {
                    await DiscountActivityInfoManager.IncreaseActivityProductSoldNumCache(currentActivity.ActivityId,product.Pid, product.RequestNum, TimeSpan.FromMinutes(1));
                }
                if (_verifyPayAndInstallMethod)
                {
                    if (product.Pid.StartsWith("TR", StringComparison.CurrentCultureIgnoreCase) && currentActivity.InstallMethod > 0 && product.RequestInstallMethod != currentActivity.InstallMethod)
                    {
                        _returnList.Add(new ProductHitDiscountResponse()
                        {
                            DiscountPrice = product.RequestPrice,
                            HasDiscountActivity = true,
                            DiscountRule = null,
                            IsHit = false,
                            Pid = product.Pid,
                            FailCode = ProductHitFailCode.GetFailCodeAndDescriptionOfInstall(currentActivity.InstallMethod).Item1,
                            FailMessage = ProductHitFailCode.GetFailCodeAndDescriptionOfInstall(currentActivity.InstallMethod).Item2,
                        });
                        continue;
                    }
                    if (product.Pid.StartsWith("TR", StringComparison.CurrentCultureIgnoreCase) && currentActivity.PaymentMethod > 0 && product.RequestPaymentMethod != currentActivity.PaymentMethod)
                    {
                        _returnList.Add(new ProductHitDiscountResponse()
                        {
                            DiscountPrice = product.RequestPrice,
                            HasDiscountActivity = true,
                            DiscountRule = null,
                            IsHit = false,
                            Pid = product.Pid,
                            FailCode = ProductHitFailCode.GetFailCodeAndDescriptionOfPay(currentActivity.PaymentMethod).Item1,
                            FailMessage = ProductHitFailCode.GetFailCodeAndDescriptionOfPay(currentActivity.PaymentMethod).Item2,
                        });
                        continue;
                    }
                }               
                var currentProductSoldNum = productSoldNums.FirstOrDefault(s => string.Equals(s.Pid, product.Pid))?.SoldNum ?? 0;
                if (product.LimitQuantity < currentProductSoldNum + product.RequestNum)
                {
                    _returnList.Add(new ProductHitDiscountResponse()
                    {
                        DiscountPrice = product.RequestPrice,
                        HasDiscountActivity = true,
                        DiscountRule = null,
                        IsHit = false,
                        Pid = product.Pid,
                        FailCode = nameof(ProductHitFailCode.NotEnoughStock),
                        FailMessage = ProductHitFailCode.NotEnoughStock,
                    });
                    continue;
                }
                var judgeHitResult = JudgeHitDiscount(product.Pid, product.RequestPrice, product.RequestNum, currentActivityRules,
                    currentActivityRequestNum, currentActivityRequestTotalPrice);
                var isHit = judgeHitResult.Item1;
                _returnList.Add(new ProductHitDiscountResponse()
                {
                    DiscountPrice = judgeHitResult.Item2,
                    HasDiscountActivity = true,
                    DiscountRule = judgeHitResult.Item3,
                    IsHit = isHit,
                    Pid = product.Pid,
                    FailCode = !isHit ? judgeHitResult.Item3.DiscountMethod == 1 ? nameof(ProductHitFailCode.NotEnoughMoney) :
                    nameof(ProductHitFailCode.NotEnoughNum) : string.Empty,
                    FailMessage = !isHit ? judgeHitResult.Item3.DiscountMethod == 1 ? ProductHitFailCode.NotEnoughMoney :
                    ProductHitFailCode.NotEnoughNum : string.Empty,
                });
            }
        }
        /// <summary>
        /// 获取当前组有效的产品数量和价格
        /// </summary>
        /// <param name="group"></param>
        /// <param name="productSoldNums"></param>
        /// <param name="currentActivityRules"></param>
        /// <param name="currentActivity"></param>
        /// <param name="currentActivityRequestNum"></param>
        /// <param name="currentActivityRequestTotalPrice"></param>
        /// <returns></returns>
        private Tuple<int, decimal> GetEffictiveCurrentActivityRequestNumAndPrice(IGrouping<string, CompProduct> group, IEnumerable<ActivityPidSoldNumModel> productSoldNums,
            List<SalePromotionActivityDiscount> currentActivityRules, SalePromotionActivityModel currentActivity,
            int currentActivityRequestNum, decimal currentActivityRequestTotalPrice)
        {
            foreach (var product in group.AsEnumerable())
            {
                if (_verifyPayAndInstallMethod)
                {
                    if (product.Pid.StartsWith("TR", StringComparison.CurrentCultureIgnoreCase) && currentActivity.InstallMethod > 0 && product.RequestInstallMethod != currentActivity.InstallMethod)
                    {
                        currentActivityRequestNum = currentActivityRequestNum - product.RequestNum;
                        currentActivityRequestTotalPrice = currentActivityRequestTotalPrice - product.RequestPrice * product.RequestNum;
                        continue;
                    }
                    if (product.Pid.StartsWith("TR", StringComparison.CurrentCultureIgnoreCase) && currentActivity.PaymentMethod > 0 && product.RequestPaymentMethod != currentActivity.PaymentMethod)
                    {
                        currentActivityRequestNum = currentActivityRequestNum - product.RequestNum;
                        currentActivityRequestTotalPrice = currentActivityRequestTotalPrice - product.RequestPrice * product.RequestNum;
                        continue;
                    }
                }
                var currentProductSoldNum = productSoldNums.FirstOrDefault(s => string.Equals(s.Pid, product.Pid))?.SoldNum ?? 0;
                if (product.LimitQuantity < currentProductSoldNum + product.RequestNum)
                {
                    currentActivityRequestNum = currentActivityRequestNum - product.RequestNum;
                    currentActivityRequestTotalPrice = currentActivityRequestTotalPrice - product.RequestPrice * product.RequestNum;
                    continue;
                }
            }
            return new Tuple<int, decimal>(currentActivityRequestNum, currentActivityRequestTotalPrice);
        }
        /// <summary>
        /// 校验产品是否命中打折
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="price"></param>
        /// <param name="num"></param>
        /// <param name="discountRules"></param>
        /// <returns></returns>
        private Tuple<bool, decimal, SalePromotionActivityDiscount> JudgeHitDiscount(string pid, decimal price, int num,
            List<SalePromotionActivityDiscount> discountRules, int currentActivityRequestTotalNum,
            decimal currentActivityRequestTotalPrice)
        {
            var isHit = false;
            var salePrice = price;
            SalePromotionActivityDiscount hitRule = null;
            discountRules = discountRules.OrderByDescending(s => s.Condition).ToList();
            foreach (var discountRule in discountRules)
            {
                switch (discountRule.DiscountMethod)
                {
                    case 1://满额折
                        if (currentActivityRequestTotalPrice >= discountRule.Condition)
                        {
                            isHit = true;
                            salePrice = (price * (discountRule.DiscountRate)) / 100;
                        }
                        hitRule = discountRule;
                        break;
                    case 2://满件折
                        if (currentActivityRequestTotalNum >= discountRule.Condition)
                        {
                            isHit = true;
                            salePrice = (price * (discountRule.DiscountRate)) / 100;
                        }
                        hitRule = discountRule;
                        break;
                }
                if (isHit)
                    break;
            }

            return new Tuple<bool, decimal, SalePromotionActivityDiscount>(isHit, salePrice, hitRule);
        }
 
     

        private class CompProduct
        {
            public string Pid { get; set; }
            public decimal RequestPrice { get; set; }
            public int RequestNum { get; set; }
            public string ActivityId { get; set; }
            public int LimitQuantity { get; set; }
            public int RequestPaymentMethod { get; set; }
            public int RequestInstallMethod { get; set; }
        }    
    }
}
