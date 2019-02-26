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
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Provisioning.Business.CompetingProductsMonitorManager;

namespace Tuhu.Provisioning.Areas.CompetingProductsMonitor.Controllers
{
    public class MonitorController : Controller
    {
        /// <summary>
        /// 产品的竞品列表
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        public PartialViewResult PriceMonitorConfig(string pid, string productName, decimal? minPrice = 0)
        {
            ViewBag.Pid = pid;
            ViewBag.ProductName = productName;
            ViewBag.MinPrice = minPrice;
            var manager = new CompetingProductsMonitorManager();
            return PartialView(new ListModel<CompetingProductsMonitorEntity>(manager.GetAllProductsMonitorbyPid(pid)));
        }
        /// <summary>
        /// 添加产品的竞品信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PriceMonitorConfig(CompetingProductsMonitorEntity model)
        {
            if (model == null)
                return Json(-2);
            switch (model.ShopCode)
            {
                case "京东自营":
                    {
                        var price = GetItemPrice(model.ItemID.ToString(), client => client.GetJingdongPrice);
                        if (price == null)
                            return Json(-4);

                        model.Price = price.Price;
                        model.Title = price.Title;

                        model.SkuID = model.ItemID;
                        model.ItemID = 0;
                    }
                    break;
                case "麦轮胎官网":
                    {
                        var price = GetItemPrice(model.ItemID.ToString(), client => client.GetMailuntaiPrice);
                        if (price == null)
                            return Json(-4);

                        model.Price = price.Price;
                        model.Title = price.Title;
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
                    {
                        var qplProductInfo = QPLShopManage.GetQPLPrice(model.Pid);
                        model.Price = qplProductInfo.Item1;
                        model.Title = qplProductInfo.Item2;
                        model.ItemID = 0;
                        model.SkuID = 0;
                        model.ItemCode=string.Empty;
                    }
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
            var manager = new CompetingProductsMonitorManager();
            var result = manager.Insert(model);

            return result > 0 ? Json(model) : Json(result);
        }
        /// <summary>
        /// 获取各渠道的竞品信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static ItemPriceModel GetItemPrice(string id, Func<IHttpProxyClient, Func<string, OperationResult<ItemPriceModel>>> func)
        {
            using (var client = new HttpProxyClient())
            {
                return func(client)(id).Result;
            }
        }
        /// <summary>
        /// 根据pkid或pid、shopCode、ItemID删除竞品监控信息
        /// </summary>
        /// <param name="shopCode"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePriceMonitor(string pid, string shopCode, string itemId, long pkid = 0)
        {
            var manager = new CompetingProductsMonitorManager();
            return Json(manager.Delete(pid,shopCode,itemId,pkid));
        }
    }
}