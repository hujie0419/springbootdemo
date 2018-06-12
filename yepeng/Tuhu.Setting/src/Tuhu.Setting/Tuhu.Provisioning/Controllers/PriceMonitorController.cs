using System;
using System.Web.Mvc;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Service;
using Tuhu.Service.Utility;
using Tuhu.Provisioning.Models;
using Tuhu.Service;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;
using System.Collections.Generic;
using Tuhu.Provisioning.Controls;
namespace Tuhu.Provisioning.Controllers
{
    public class PriceMonitorController : Controller
    {
        //[PowerManage]
        public ActionResult PriceMonitorConfig(string shopCode, string q, int pn = 1)
        {
            if (string.IsNullOrWhiteSpace(shopCode))
            {
                shopCode = null;
            }
            ViewBag.q = q;
            ViewBag.shopCode = shopCode;
            var pager = new PagerModel(pn);
            //return View();
            return View(new ListModel<ProductMappingModel>(pager, ProductMappingManager.List(shopCode, q, pager)));
        }

        [HttpPost]
        public ActionResult PriceMonitorConfig(ProductMappingModel model)
        {
            if (model == null)
                return Json(-2);

            if (ModelState.IsValid)
            {
                switch (model.ShopCode)
                {
                    case "京东自营":
                        {
                            var price = GetItemPrice(model.ItemID.ToString(), client => client.GetJingdongPrice);
                            if (price == null)
                                return Json(-4);

                            model.Price = price.Price;
                            model.Title = price.Title;

                            model.SkuID = model.ItemID.GetValueOrDefault();
                            model.ItemID = 0;
                        }
                        break;                  
                    case "养车无忧":
                        {
                            var price = GetItemPrice(model.ItemCode, client => client.GetYangche51Price);
                            if (price == null)
                                return Json(-4);

                            model.Price = price.Price;
                            model.Title = price.Title;
                            //清除ItemID
                            model.ItemID = 0;
                            model.SkuID = 0;

                        }
                        break;
                    case "汽配龙":
                        model.Price = 0;
                        model.Title = "";
                        model.ItemID = 0;
                        model.SkuID = 0;
                        break;
                    case "汽车超人零售":
                        {
                            var price = GetItemPrice(model.ItemID.ToString(), client => client.GetQccrRetailPrice);
                            if (price == null)
                                return Json(-4);

                            model.Price = price.Price;
                            model.Title = price.Title;
                        }
                        break;
                    case "汽车超人批发":
                        {
                            var price = GetItemPrice(model.ItemID.ToString(), client => client.GetQccrTradePrice);
                            if (price == null)
                                return Json(-4);

                            model.Price = price.Price;
                            model.Title = price.Title;
                        }
                        break;
                    case "康众官网":
                        {
                            var price = GetItemPrice(model.ItemCode.ToString(), client => client.GetCarzonePriceFromApp);
                            if (price == null)
                                return Json(-4);

                            model.Price = price.Price;
                            model.Title = price.Title;
                        }
                        break;
                    default:
                        {
                            var price = GetItemPrice(model.ItemID.ToString(), client => client.GetTaobaoPrice);
                            if (price == null)
                                return Json(-4);

                            model.Price = price.Price;
                            model.Title = price.Title;
                        }
                        break;
                }

                var result = ProductMappingManager.Insert(model);

                return result > 0 ? Json(model) : Json(result);
            }
            return Json(-3);
        }
        public static ItemPriceModel GetItemPrice(string id, Func<IHttpProxyClient, Func<string, OperationResult<ItemPriceModel>>> func)
        {
            using(var client=new HttpProxyClient())
            {
                return func(client)(id).Result;
            }
        }
        [HttpPost]
        public ActionResult DeletePriceMonitor(string shopCode, string item)
        {
            return Json(ProductMappingManager.Delete(shopCode, item));
        }
    }
}