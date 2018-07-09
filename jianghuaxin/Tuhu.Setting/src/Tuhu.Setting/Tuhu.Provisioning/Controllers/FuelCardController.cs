using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.SimpleConfig;
using Tuhu.Provisioning.Business.Vender;
//using Tuhu.Provisioning.Business.SimpleConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Vender;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models.New;

namespace Tuhu.Provisioning.Controllers
{
    public class FuelCardController : Controller
    {
        /// <summary>
        /// 加油卡配置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            ViewBag.VenderData = new List<VenderModel>()
            {
                new VenderModel(){ VenderPKID=6054, VenderID="GYS222",VenderShortName="苏州新科兰德",VenderName="苏州新科兰德科技有限公司",},
                new VenderModel(){ VenderPKID=6426,VenderID="GYS14:46:26",VenderShortName="山东鼎信",VenderName="山东鼎信网络科技有限公司"}
            };
            return View();
        }

        /// <summary>
        /// 查询加油卡配置
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        public JsonResult GetValue(string cardType)
        {
            SimpleConfigManager manager = new SimpleConfigManager();
            var config = manager.SelectFuelCardConfig();
            CardType result = null;
            if (config.CardTypes != null && config.CardTypes.Any())
            {
                result = config.CardTypes.FirstOrDefault(p => p.TypeName.Equals(cardType));
            }
            return Json(new {Status = result!=null, Data = result},
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新加油卡配置
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateFuelCardConfig(CardType cardType)
        {
            SimpleConfigManager manager = new SimpleConfigManager();
            var config = manager.SelectFuelCardConfig();
            if (string.IsNullOrEmpty(cardType.TypeName))
            {
                return Json(new {Status = false, Msg = "加油卡名为空" });
            }
            if (cardType.FuelCards != null)
            {
                foreach (var item in cardType.FuelCards)
                {
                    item.Pid = item.Pid.Trim();
                }
                if (cardType.FuelCards.Select(p => p.Pid).Distinct().Count() != cardType.FuelCards.Count)
                {
                    return Json(new { Status = false, Msg = "加油卡Pid存在重复值" });
                }
            }
            if (config.CardTypes != null && config.CardTypes.Any())
            {
                if (config.CardTypes.Any(t => t.TypeName.Equals(cardType.TypeName)))
                {
                    foreach (var item in config.CardTypes.Where(t => t.TypeName.Equals(cardType.TypeName)))
                    {
                        item.FuelCards = cardType.FuelCards;
                        item.AnnounceMent = cardType.AnnounceMent;
                    }
                }
                else
                {
                    config.CardTypes.Add(cardType);
                }
            }
            else
            {
                config.CardTypes = new List<CardType> {cardType};
            }
            var result = manager.UpdateConfig("FuelCardConfig",config);
            return Json(new {Status = result});
        }

        /// <summary>
        /// 通过pid查询产品信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public JsonResult GetProductPriceByPid(string pid)
        {
            using ( var client = new ProductClient())
            {
                var product = client.SelectSkuProductListByPids(new List<string> {pid});
                if (product.Success && product.Result != null)
                {
                    var fuelCard = product.Result.FirstOrDefault();
                    if (fuelCard != null)
                    {
                        if (!fuelCard.Category.Equals("JYK"))
                        {
                            return Json(new { Status = -1, Msg = "当前产品不是加油卡" }, JsonRequestBehavior.AllowGet);
                        }
                        if (!fuelCard.Onsale)
                        {
                            return Json(new { Status = -1, Msg = "当前产品已下架" }, JsonRequestBehavior.AllowGet);
                        }
                        if (fuelCard.Stockout)
                        {
                            return Json(new { Status = -1, Msg = "当前产品无货" }, JsonRequestBehavior.AllowGet);
                        }
                        if (fuelCard.MarketingPrice != null && fuelCard.MarketingPrice > 0 && fuelCard.Price >= 0)
                        {
                            return Json(new { Status = 1, ListPrice = fuelCard.Price, MarketPrice = fuelCard.MarketingPrice }, JsonRequestBehavior.AllowGet);
                        }

                    }                   
                }
            }
            return Json(new { Status = 0 },JsonRequestBehavior.AllowGet);
        }
    }
}