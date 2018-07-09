using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.DataAccess.DAO.Promotion;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Request;

namespace Tuhu.Provisioning.Controllers
{
    public class ShopPromotionController : Controller
    {
        [PowerManage(IwSystem = "OperateSys")]
        [HttpGet]
        public JsonResult GetCouponList(string keywords, int? discount, string startDate, string endDate, int status,int pageIndex,int pageSize)
        {
            var result = ShopPromotionManager.GetCouponList(keywords, discount, startDate, endDate, status, pageIndex,
                pageSize);
            return Json(new
            {
                result.Source,
                result.Pager
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateCouponStatus(int ruleId, int status)
        {
            var result = DalShopPromotion.UpdateCouponRuleStatus(ruleId, status,User.Identity.Name);
            return Json(new
            {
                data = result
            });
        }
        [HttpPost]
        public JsonResult CopyCouponRule(int ruleId)
        {
            var result = DalShopPromotion.CopyCouponRule(ruleId, User.Identity.Name);
            return Json(new
            {
                data = result
            });
        }
        [HttpPost]
        public JsonResult SaveCouponRules(ShopCouponRuleRequest request) {
            using (var client = new ShopPromotionClient())
            {
                request.Operater = User.Identity.Name;
                if (request.Pids == null) request.Pids = new List<string>();
                if (request.ShopIds == null) request.ShopIds = new List<int>();
                var response = client.SaveCouponRules(request);
                return Json(new
                {
                    data = response.Success,
                    ruleId = response.Result
                });
            }
        }
        [HttpGet]
        public JsonResult GetShopServesAllCatogry()
        {
            using (var client = new Tuhu.Service.TuhuShop.ShopClient())
            {
                var response = client.GetShopServesAllCatogry();
                if (response.Success)
                {
                    return Json(new
                    {
                        data = response.Result
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        data = new List<Tuhu.Service.TuhuShop.Models.ShopServesAllCatogry>()
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpPost]
        public JsonResult GetServiceList(Service.TuhuShop.Models.ShopServesRequest request)
        {
            using (var client = new Tuhu.Service.TuhuShop.ShopClient())
            {
                var response = client.GetShopServesDetailList(request);
                if (response.Success)
                {
                    return Json(new
                    {
                        data = response.Result
                    });
                }
                else
                {
                    return Json(new
                    {
                        data = new List<Service.TuhuShop.Models.ShopServesDetailList>()
                    });
                }
            }
        }

        [HttpGet]
        public JsonResult GetCouponRuleDetail(int ruleId)
        {
            using (var client = new ShopPromotionClient())
            {
                var response = client.FetchCouponRulesByRuleId(ruleId);
                if (response.Success)
                {
                    return Json(new
                    {
                        data = response.Result
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        data = default(Service.Member.Models.ShopCouponRuleModel)
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpGet]
        public JsonResult GetNotUserdPromotionCount(int ruleId)
        {
            var result = DalShopPromotion.GetNotUserdPromotionCount(ruleId);
            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}