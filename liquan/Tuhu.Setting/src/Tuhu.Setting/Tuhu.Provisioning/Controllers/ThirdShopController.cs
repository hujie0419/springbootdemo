using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Tire;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Tuhu.Service.Product.Models;
using Tuhu.Service.ThirdParty;

namespace Tuhu.Provisioning.Controllers
{
    public class ThirdShopController : Controller
    {
        #region 第三方价格管理平台

        [PowerManage]
        public ActionResult Index()
        {
            var brands = RecommendManager.GetBrands();
            //var datas = ThirdShopManager.BuildProductMin_maoliValue();
            return View(brands);
        }

        public ActionResult MiaoSha()
        {
            var brands = RecommendManager.GetBrands();
            var tireSizes = RecommendManager.SelectALLTireSize();
            return View(Tuple.Create(brands, tireSizes));
        }
        public PartialViewResult ProductList(ProductListRequest request)
        {
            PagerModel pager = new PagerModel(request.PageIndex, request.PageSize);
            var result = ThirdShopManager.ForProductListAsync(request, pager, true);
            if (result.Any())
            {
                pager.TotalItem = result.Count;
                result = result.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
            }
            ViewBag.Request = request;
            return PartialView(new ListModel<ProductPriceModel>() { Pager = pager, Source = result });
        }
        public PartialViewResult MiaoShaProductList(PriceSelectModel selectModel, int PriceDifStatus, string shopCode, int pageIndex = 1, int pageSize = 10)
        {
            PagerModel pager = new PagerModel(pageIndex, pageSize);
            var model = ThirdShopManager.FetchThirdPartyMiaoShaItems(selectModel, pager, shopCode, PriceDifStatus);          
            ViewBag.PriceDifStatus = PriceDifStatus;
            ViewBag.SelectModel = selectModel;
            ViewBag.shopCode = shopCode;
            return PartialView(new ListModel<ThirdPartyMiaoShaModel>() { Pager = pager, Source = model });
        }

         
        #endregion
        #region 优惠券管理
        public ActionResult TireCouponManage()
        {
            var dat = ThirdShopManager.TireCouponManage();
            var ShopList = new List<string>
            {
                "自有平台",
                "途虎淘宝",
                "途虎淘宝2",
                "途虎天猫1",
                "途虎天猫2",
                "途虎天猫3",
                "途虎天猫4",
                "途虎京东2",
                "途虎京东旗舰"
            };
            var result = new List<TireCouponResultModel>();
            ShopList.ForEach(g =>
            {
                result.Add(dat.FirstOrDefault(t => t.ShopName == g) ?? new TireCouponResultModel
                {
                    ShopName = g
                });
            });
            return View(result);
        }



        public async System.Threading.Tasks.Task<JsonResult> EditCanUseCoupon(IEnumerable<string> pids, bool canUseCoupon)
        {
            JsonResult jr = new JsonResult();
            var checkresults = ThirdShopManager.SelectProductCouponPrice(pids);
            var exists = pids.Where(x => checkresults.ContainsKey(x));
            if (exists.Any())
            {
                jr.Data = new { code = 0, msg = $"已配置券后价.pid:{string.Join(" , ", exists)}" };
                return jr;
            }
            Dictionary<string, bool> beforvalues = new Dictionary<string, bool>();
            List<ProductExtraProperties> models = new List<ProductExtraProperties>();
            using (var client = new Tuhu.Service.Product.ProductConfigClient())
            {
                var getresult = client.SelectProductExtraPropertiesByPids(pids);
                getresult.ThrowIfException(true);
                foreach (var pid in pids)
                {
                    var item = getresult.Result.FirstOrDefault(x => x.PID == pid) ??
                               new ProductExtraProperties() { PID = pid, CanUseCoupon = true };
                    beforvalues[pid] = item.CanUseCoupon;
                    item.CanUseCoupon = canUseCoupon;
                    models.Add(item);
                }

                if (models.Any())
                {
                    var updateresult = client.BatchCreateOrUpdateProductExtraProperties(models);
                    updateresult.ThrowIfException(true);
                    jr.Data = new
                    {
                        code = updateresult.Result > 0 ? 1 : 0,
                        msg = updateresult.Result > 0 ? "保存成功" : "保存失败"
                    };
                    var logs = pids.Select(x => new TireModifyConfigLog()
                    {
                        Pid = x,
                        Type = "CanUseCoupon",
                        UserId = HttpContext.User.Identity.Name,
                        Before = (beforvalues.ContainsKey(x) ? beforvalues[x] : true).ToString(),
                        After = canUseCoupon.ToString()
                    });
                    await TireInsuranceYearsManager.WriteLogsAsync(logs);
                    return jr;
                }
                else
                {
                    jr.Data = new
                    {
                        code = 0,
                        msg = "数据错误"
                    };
                    return jr;
                }
            }
        }

        public JsonResult TireCouponList(string ShopName)
        {
            var data = ThirdShopManager.TireCouponManage(ShopName).FirstOrDefault();
            var result = new List<TireCouponModel>();
            if (data != null)
            {
                result.AddRange(data.CouponA);
                result.AddRange(data.CouponB);
                result.AddRange(data.CouponC);
            }
            return Json(result);
        }

        public int AddTireCoupon(TireCouponModel request)
        {
            if (string.IsNullOrWhiteSpace(request.ShopName) || request.QualifiedPrice < request.Reduce || request.Reduce < 0 || request.EndTime < DateTime.Now || request.EndTime < request.StartTime || request.CouponType > 3 || request.CouponType < 0 || request.CouponUseRule < 0 || request.CouponUseRule > 1)
            {
                return 0;
            }
            return ThirdShopManager.AddTireCoupon(request, ThreadIdentity.Operator.Name);
        }
        public int DeleteTireCoupon(int PKID)
        {
            if (PKID < 1)
            {
                return 0;
            }
            return ThirdShopManager.DeleteTireCoupon(PKID, ThreadIdentity.Operator.Name);
        }

        public JsonResult FetchCouponLogByShopName(string ShopName)
        {
            if (string.IsNullOrWhiteSpace(ShopName))
            {
                return Json(new
                {
                    Code = 0
                });
            }
            return Json(new
            {
                Code = 1,
                Data = ThirdShopManager.FetchCouponLogByShopName(ShopName)
            });
        }
        #endregion
        #region 最低限价
        public JsonResult SetLowestLimitPrice(string PID, decimal? OldPrice, decimal LowestLimitPrice, string type)
        {
            if (string.IsNullOrWhiteSpace(PID))
            {
                return Json(new
                {
                    Code = 0,
                    Info = "参数错误"
                });
            }
            var result = ThirdShopManager.SetLowestLimitPrice(PID, OldPrice, LowestLimitPrice, ThreadIdentity.Operator.Name, type);
            return Json(new
            {
                Code = result,
                Info = result > 0 ? "修改成功" : "修改失败"
            });
        }
        public JsonResult GetLowestLimitLog(string PID, string type)
        {
            if (string.IsNullOrWhiteSpace(PID))
            {
                return Json(new { Code = 0, Info = "参数错误" });
            }
            var result = ThirdShopManager.GetLowestLimitLog(PID, type);
            return Json(new
            {
                Code = 1,
                Data = result
            });
        }


        public JsonResult SyncMiaoShaItems(string shopCode)
        {

            var flag = true;
            using (var client = new ThirdPartyClient())
            {
                var response = client.SyncMiaoShaItems(shopCode);
                if (response != null)
                {
                    flag = response.Result;
                }
            }
                return Json(new
                {
                    result = flag
                });
        }
        #endregion
        public JsonResult FetchLowestPrice(string ShopName, decimal Price, int MaxTireCount)
        {
            var result = ThirdShopManager.FetchLowestPrice(ShopName, Price, MaxTireCount);
            if (result == null)
            {
                return Json(new
                {
                    Code = 0,
                    Info = "无可用优惠券"
                });
            }
            return Json(new
            {
                Code = 1,
                Count = result.Item3,
                FinalPrice = result.Item1,
                CouponDetail = string.Join(",", result.Item2.OrderBy(g => g.CouponType).Select(g => $"{(char)('A' + g.CouponType - 1)}类优惠券-{g.Description}"))
            }, JsonRequestBehavior.AllowGet);
        }
        public FileResult ExportExcel(ProductListRequest request)
        {
            PagerModel pager = new PagerModel(request.PageIndex, request.PageSize);
            var result = ThirdShopManager.ForProductListAsync(request, pager, true);
            var pagesize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ExportSize"]);
            var book = new HSSFWorkbook();
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            var fileName = $"第三方平台{ThreadIdentity.Operator.Name.Split('@')[0]}.xls";
            row.CreateCell(0).SetCellValue("品牌");
            row.CreateCell(1).SetCellValue("PID");
            row.CreateCell(2).SetCellValue("产品名称");
            row.CreateCell(3).SetCellValue("进货价");
            row.CreateCell(4).SetCellValue("最小毛利");
            row.CreateCell(5).SetCellValue("可用券");
            row.CreateCell(6).SetCellValue("官网价格");
            row.CreateCell(7).SetCellValue("自有平台最低价");
            row.CreateCell(8).SetCellValue("途虎淘宝");
            row.CreateCell(9).SetCellValue("途虎淘宝最低价");
            row.CreateCell(10).SetCellValue("途虎淘宝2");
            row.CreateCell(11).SetCellValue("途虎淘宝2最低价");
            row.CreateCell(12).SetCellValue("途虎天猫1");
            row.CreateCell(13).SetCellValue("途虎天猫1最低价");
            row.CreateCell(14).SetCellValue("途虎天猫2");
            row.CreateCell(15).SetCellValue("途虎天猫2最低价");
            row.CreateCell(16).SetCellValue("途虎天猫3");
            row.CreateCell(17).SetCellValue("途虎天猫3最低价");
            row.CreateCell(18).SetCellValue("途虎天猫4");
            row.CreateCell(19).SetCellValue("途虎天猫4最低价");
            row.CreateCell(20).SetCellValue("途虎京东2");
            row.CreateCell(21).SetCellValue("途虎京东2最低价");
            row.CreateCell(22).SetCellValue("途虎京东旗舰");
            row.CreateCell(23).SetCellValue("途虎京东旗舰最低价");
            row.CreateCell(24).SetCellValue("最低面价(平时)");
            row.CreateCell(25).SetCellValue("最低面价(大促)");
            row.CreateCell(26).SetCellValue("最低券后价(平时)");
            row.CreateCell(27).SetCellValue("最低券后价(大促)");
            var i = 0;
            foreach (var item in result)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(++i);
                rowtemp.CreateCell(0).SetCellValue(item.Brand);
                rowtemp.CreateCell(1).SetCellValue(item.PID);
                rowtemp.CreateCell(2).SetCellValue(item.ProductName);
                rowtemp.CreateCell(3).SetCellValue(item.Cost == null ? "" : item.Cost.Value.ToString("0.00"));
                rowtemp.CreateCell(4).SetCellValue(item.Min_maoliValue == null ? "" : item.Min_maoliValue.Value.ToString("0.00"));
                rowtemp.CreateCell(5).SetCellValue(item.CanUseCoupon.HasValue ? (item.CanUseCoupon.Value ? "是" : "否") : "是");
                rowtemp.CreateCell(6).SetCellValue(item.Price == null ? "" : item.Price.Value.ToString("0.00"));
                rowtemp.CreateCell(7).SetCellValue(item.LowestPrice == null ? "" : item.LowestPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(8).SetCellValue(item.TBPrice == null ? "" : item.TBPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(9).SetCellValue(item.TBLowestPrice == null ? "" : item.TBLowestPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(10).SetCellValue(item.TB2Price == null ? "" : item.TB2Price.Value.ToString("0.00"));
                rowtemp.CreateCell(11).SetCellValue(item.TB2LowestPrice == null ? "" : item.TB2LowestPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(12).SetCellValue(item.TM1Price == null ? "" : item.TM1Price.Value.ToString("0.00"));
                rowtemp.CreateCell(13).SetCellValue(item.TM1LowestPrice == null ? "" : item.TM1LowestPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(14).SetCellValue(item.TM2Price == null ? "" : item.TM2Price.Value.ToString("0.00"));
                rowtemp.CreateCell(15).SetCellValue(item.TM2LowestPrice == null ? "" : item.TM2LowestPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(16).SetCellValue(item.TM3Price == null ? "" : item.TM3Price.Value.ToString("0.00"));
                rowtemp.CreateCell(17).SetCellValue(item.TM3LowestPrice == null ? "" : item.TM3LowestPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(18).SetCellValue(item.TM4Price == null ? "" : item.TM4Price.Value.ToString("0.00"));
                rowtemp.CreateCell(19).SetCellValue(item.TM4LowestPrice == null ? "" : item.TM4LowestPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(20).SetCellValue(item.JDPrice == null ? "" : item.JDPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(21).SetCellValue(item.JDLowestPrice == null ? "" : item.JDLowestPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(22).SetCellValue(item.JDFlagShipPrice == null ? "" : item.JDFlagShipPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(23).SetCellValue(item.JDFlagShipLowestPrice == null ? "" : item.JDFlagShipLowestPrice.Value.ToString("0.00"));
                rowtemp.CreateCell(24).SetCellValue(item.LowestPrice_Normal.HasValue ? item.LowestPrice_Normal.Value.ToString("0.00") : "");
                rowtemp.CreateCell(25).SetCellValue(item.LowestPrice_Promotion.HasValue ? item.LowestPrice_Promotion.Value.ToString("0.00") : "");
                rowtemp.CreateCell(26).SetCellValue(item.CouponPrice_Normal.HasValue ? item.CouponPrice_Normal.Value.ToString("0.00") : "");
                rowtemp.CreateCell(27).SetCellValue(item.CouponPrice_Promotion.HasValue ? item.CouponPrice_Promotion.Value.ToString("0.00") : "");
            }
            var ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        public JsonResult FetchPurchaseRestriction()
        {
            var Value = ThirdShopManager.FetchPurchaseRestriction();
            if (Value > 0)
            {
                return Json(new
                {
                    Code = 1,
                    Data = Value
                });
            }
            return Json(new
            {
                Code = 0,
                Info = "无法读取到PurchaseRestriction的值"
            });
        }
    }
}