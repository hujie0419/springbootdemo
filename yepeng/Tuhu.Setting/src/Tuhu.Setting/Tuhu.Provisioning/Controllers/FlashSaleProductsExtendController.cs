using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class FlashSaleProductsExtendController : Controller
    {
        private static readonly Lazy<FlashSaleProductsExtendManager> lazy = new Lazy<FlashSaleProductsExtendManager>();

        private readonly string ObjectType = "FPIUC";
        private FlashSaleProductsExtendManager FlashSaleProductsExtendManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public ActionResult List(string PID, string ActivityID, string order, int pageNumber = 1, int pageSize = 100)
        {
            int count = 0;
            string strSql = string.Empty;
            if (!string.IsNullOrWhiteSpace(PID))
            {
                strSql += "  AND A.PID='" + PID.TrimStart().TrimEnd() + "'";
            }
            if (!string.IsNullOrWhiteSpace(ActivityID))
            {
                strSql += "  AND A.ActivityID='" + ActivityID.TrimStart().TrimEnd() + "'";
            }
            //string orderBy = "((Price-C.cost)/Price)*100 DESC";

            if (!string.IsNullOrWhiteSpace(order))
            {
                if (order == "MonthSalesdesc")
                {
                    orderBy = " C.salenum DESC";

                }
                else if (order == "MonthSalesasc")
                {
                    orderBy = " C.salenum ASC";
                }
                else if (order == "GrossProfitMargindesc")
                {
                    orderBy = @"  ISNULL( CONVERT(NUMERIC(8, 2), ROUND(( ( Price - C.cost )
                                        / ( CASE WHEN Price = 0 THEN 1
                                                ELSE Price
                                            END ) ) * 100, 2)),100) DESC";
                }
                else if (order == "GrossProfitMarginasc")
                {
                    orderBy = @" ISNULL( CONVERT(NUMERIC(8, 2), ROUND(( ( Price - C.cost )
                                        / ( CASE WHEN Price = 0 THEN 1
                                                ELSE Price
                                            END ) ) * 100, 2)),100) ASC";
                }

            }

            var lists = FlashSaleProductsExtendManager.GetFlashSaleProductsExtendList(strSql, pageSize, pageNumber, orderBy, out count);

            var list = new OutData<List<FlashSaleProductsExtend>, int>(lists, count);
            var pager = new PagerModel(pageNumber, pageSize)
            {
                TotalItem = count
            };
            return View("List", new ListModel<FlashSaleProductsExtend>(list.ReturnValue, pager));
        }

        private static string orderBy = @" ISNULL( CONVERT(NUMERIC(8, 2), ROUND(( ( Price - C.cost )
                                        / (CASE WHEN Price = 0 THEN 1
                                                ELSE Price
                                            END ) ) * 100, 2)),100) DESC";

        [HttpPost]
        public JsonResult Edit(FlashSaleProductsExtend model)
        {
            try
            {
                FlashSaleProductsExtendManager.UpdateFlashSaleProductsIsUsePCode(model);
                LoggerManager.InsertOplog(User.Identity.Name, ObjectType, model.PKID, "修改PKID=" + model.PKID + ",限时抢购产品的优惠券的使用状态IsUsePCode：" + model.IsUsePCode);

                return Json(true);
            }
            catch
            {
                return Json(false);
            }

        }
    }
}
