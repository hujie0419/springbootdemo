using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.BaoYangPriceGuide;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.Business.Tire;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangPriceGuideController : Controller
    {
        [PowerManage]
        public ActionResult BaoYangPriceGuide()
        {
            ViewBag.Type = "BaoYang";
            return View();
        }

        [PowerManage]
        public ActionResult BaoYangPriceReport()
        {
            ViewBag.Type = "BaoYang";
            return View();
        }

        [PowerManage]
        public ActionResult BaoYangPriceCostReport()
        {
            ViewBag.Type = "BaoYang";
            return View();
        }

        [PowerManage]
        public ActionResult CarPriceGuide()
        {
            ViewBag.Type = "Car";
            return View();
        }

        [PowerManage]
        public ActionResult CarPriceReport()
        {
            ViewBag.Type = "Car";
            return View();
        }

        [PowerManage]
        public ActionResult CarPriceCostReport()
        {
            ViewBag.Type = "Car";
            return View();
        }

        public PartialViewResult Search()
        {
            return PartialView();
        }

        public JsonResult GetCategoryBrand(string firstType, string secondType, string thirdType)
        {
            var manager = new BaoYangPriceGuideManager();
            var result = manager.SelectBaoYangBrands(firstType, secondType, thirdType);
            return Json(new {data = result}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStock(string pid, string type = "")
        {
            if (string.IsNullOrWhiteSpace(pid))
            {
                return Json(new { Status = false, Msg = "参数验证失败" });
            }

            BaoYangPriceGuideManager manager = new BaoYangPriceGuideManager();
            var result = manager.SelectStock(new[] { pid });
            if (string.Equals(type, "QPLStock", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { Status = result.Item1, data = result.Item2?.Where(x => string.Equals(x.WarehouseType, "DuLiCang", StringComparison.OrdinalIgnoreCase))?.ToList() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = result.Item1, data = result.Item2?.Where(x => !string.Equals(x.WarehouseType, "DuLiCang", StringComparison.OrdinalIgnoreCase)) }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult PriceProductList(BaoYangPriceSelectModel param, string pageType, int pageIndex, int pageSize)
        {
            PagerModel pager = new PagerModel(pageIndex, pageSize);
            var manager = new BaoYangPriceGuideManager();
            var result = new List<BaoYangPriceGuideList>();
            var data = manager.SelectBaoYangPriceGuide(param, pager);
            if (data.Item1)
            {
                result = data.Item2;
            }
            ViewBag.SelectModel = param;
            ViewBag.PageType = pageType;
            return PartialView(new ListModel<BaoYangPriceGuideList>() { Pager = pager, Source = result });
        }

        public ActionResult ZXTPrice(string pid)
        {
            var result = PriceManager.PriceChangeLog(pid).Where(c => c.ChangeDateTime > DateTime.Now.AddYears(-1)).OrderBy(c => c.ChangeDateTime).ToList();
            if (result.Any())
            {
                ViewBag.xArr = string.Join(",", result.Select(_ => _.ChangeDateTime.ToShortDateString()));
                ViewBag.yArr = string.Join(",", result.Select(_ => _.NewPrice.ToString()));
            }
            ViewBag.PID = pid;
            return View(result.OrderByDescending(_ => _.ChangeDateTime).ToList());
        }

        public ActionResult ZXTCost(string pid)
        {
            var result2 = new List<ZXTCost>();
            var result = PriceManager.GetZXTPurchaseByPID(pid).OrderBy(c => c.CreatedDatetime).ToList();
            if (result.Any())
            {
                var dic = result.GroupBy(_ => _.CreatedDatetime.ToShortDateString()).ToDictionary(_ => _.Key, _ => _.ToList());
                foreach (var item in dic)
                {
                    if (item.Value.Count == 1)
                        result2.AddRange(item.Value);
                    else
                    {
                        var tempModel = new ZXTCost()
                        {
                            CreatedDatetime = item.Value.FirstOrDefault().CreatedDatetime,
                            CostPrice = item.Value.Average(_ => _.CostPrice),
                            PID = item.Value.FirstOrDefault().PID,
                            Num = item.Value.Sum(_ => _.Num),
                            WareHouse = "多仓库"
                        };
                        result2.Add(tempModel);
                    }
                }
                if (result2.Any())
                {
                    ViewBag.xArr = string.Join(",", result2.Select(_ => $"({_.WareHouse + _.Num}件)   " + _.CreatedDatetime.ToShortDateString()));
                    ViewBag.yArr = string.Join(",", result2.Select(_ => _.CostPrice.ToString("0.00")));
                }
            }
            ViewBag.PID = pid;
            return View(result.OrderByDescending(_ => _.CreatedDatetime).ToList());
        }

        [PowerManage]
        public ActionResult PriceGuidePara(string type = "")
        {
            var manager = new BaoYangPriceGuideManager();
            ViewBag.Type = type;
            return View(new BaoYangGuideViewModel()
            {
                Warn = type == "warn" ? manager.SelectWarningLine() : null
            });
        }

        [HttpPost]
        public JsonResult SaveGuidePara(BaoYangPriceWeight model, string prevValue)
        {
            var manager = new BaoYangPriceGuideManager();
            var result = manager.SavePriceWeight(model);
            if (result > 0 && result != 99)
            {
                string operation;
                if (model.WeightType.Equals("Brand"))
                {
                    operation = model.WeightType + "|" + model.CategoryName + "|" +
                                HttpUtility.UrlDecode(model.WeightName);
                }
                else
                {
                    operation = model.WeightType + "|" + HttpUtility.UrlDecode(model.WeightName);
                }
                AddOprLog(prevValue, model.WeightValue.GetValueOrDefault(0).ToString(), "ByWeight", operation);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveWarningLine(BaoYangWarningLine model)
        {
            if (model.PKID > 0 && model.UpperLimit > 0 && model.LowerLimit > 0)
            {
                var manager = new BaoYangPriceGuideManager();
                var result = manager.UpdateWarningLine(model);
                if (result > 0)
                    AddOprLog(model.LowerLimit + "|" + model.UpperLimit, model.PKID.ToString(), "ByWarn", "修改");
                return Json(result);
            }
            else
                return Json(-1);
        }

        public static void AddOprLog(string beforeValue, string afterValue, string objectType, string operation)
        {
            var oprLog = new
            {
                Author = ThreadIdentity.Operator.Name,
                ChangeDatetime = DateTime.Now,
                BeforeValue = beforeValue,
                AfterValue = afterValue,
                ObjectType = objectType,
                Operation = operation,
                ObjectID = "0",
                IPAddress = ThreadIdentity.Operator.IPAddress,
                HostName = "MessageConsumerHostService"
            };
            LoggerManager.InsertLog("BaoYangPriceGuideLog", oprLog);
        }

        public void AddWarnOprLog(string ValueJson, string ObjectType, string Operation, string afterValue = null)
        {
            var oprLog = new OprLog
            {
                Author = ThreadIdentity.Operator.Name,
                ChangeDatetime = DateTime.Now,
                BeforeValue = ValueJson,
                AfterValue = afterValue,
                ObjectType = ObjectType,
                Operation = Operation
            };
            new OprLogManager().AddOprLog(oprLog);
        }

        public ActionResult GuideParaLog(string key)
        {
            ViewBag.Type = key.Split('|')[0];
            ViewBag.SecondType = string.Empty;
            ViewBag.ThirdType = string.Empty;
            var category = key.Split('|')[1];
            var logs = BaoYangPriceGuideManager.SelectWeightOprLog("ByWeight", key);
            using (var client = new ProductClient())
            {
                var product = client.SelectProductAllCategory();
                if (product.Success && product.Result != null)
                {
                    var currentCategory = product.Result.FirstOrDefault(p => p?.CategoryName == category);
                    ViewBag.ThirdType = currentCategory?.DisplayName;
                    var oid = Convert.ToInt32(currentCategory?.NodeNo.Split('.')[1] ?? "0");
                    var secondType = BaoYangPriceGuideManager.SelectProductCategoryByOid(oid);
                    if (secondType != null)
                    {
                        ViewBag.SecondType = secondType.Item2;
                    }
                }
            }
            return View(logs);
        }
        public ActionResult GuideWarnLog(int pkid, decimal min, decimal max)
        {
            ViewBag.guidePrice = string.Concat(min.ToString("0.00"), "-", max.ToString("0.00"));
            var logs = BaoYangPriceGuideManager.SelectWarnOprLog("ByWarn", pkid.ToString());
            return View(logs);
        }

        /// <summary>
        /// 修改汽配龙价格
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="price"></param>
        /// <param name="originalPrice"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdateQPLPrice(string pid, decimal price, decimal originalPrice, string changeReason = "")
        {
            if (string.IsNullOrWhiteSpace(pid) || price <= 0)
            {
                return Json(new { Status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangPriceGuideManager();
            var dic = new Dictionary<string, decimal>();
            dic.Add(pid, price);
            var msg = await manager.UpdateQPLProductPriceByPidAsync(dic, User.Identity.Name);
            if (string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
            }
            PriceManager.InsrtCouponPriceHistory(new CouponPriceHistory()
            { PID = pid, OldPrice = originalPrice, NewPrice = price, ChangeUser = User.Identity.Name, ChangeDateTime = DateTime.Now, ChangeReason = string.IsNullOrWhiteSpace(changeReason) ? "通过GLXT修改" : changeReason });
            return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        public async Task<ActionResult> UpdateListPrice(string pid, decimal originPrice, decimal price)
        {
            var manager = new BaoYangPriceGuideManager();
            var warnLine =
                manager.SelectWarningLine().FirstOrDefault(p => p.MinGuidePrice <= originPrice && p.MaxGuidePrice >= originPrice);
            if (warnLine == null)
            {
                return Json(-5);
            }
            if ((originPrice + warnLine.UpperLimit) < price ||  (originPrice - warnLine.LowerLimit) > price)
            {
                return Json(-4);
            }
            else
            {
                var result = await UpdatePriceAsync(pid, price, "通过BYGLXT修改");

                return Json(result);
            }
        }

        private async Task<int> UpdatePriceAsync(string pid, decimal price, string changeReason)
        {
            using (var client = new ProductClient())
            {
                var result = await client.UpdateProductPriceAsync(new UpdateProductModel()
                {
                    Pid = pid,
                    Price = price,
                    ChangeReason = changeReason,
                    ChangeUser = ThreadIdentity.Operator.Name
                });
                result.ThrowIfException(true);
                return result.Result;
            }
        }
        public ActionResult ApplyUpdatePrice(PriceUpdateAuditModel model)
        {
            if (model == null)
                return Json(-99);
            model.ApplyPerson = ThreadIdentity.Operator.Name;
            var result = BaoYangPriceGuideManager.ApplyUpdatePrice(model);
            if (!Request.Url.Host.Contains(".tuhu.cn"))
                TuhuMessage.SendEmail("［待审批］保养品价格修改申请", "xujian@tuhu.cn,yuanfang@tuhu.cn", $"{ThreadIdentity.Operator.Name}于{DateTime.Now}申请将{model.PID}保养品价格修改为{model.ApplyPrice}元，超过系统预警线，请您审批！<br/><a href='http://setting.tuhu.cn/BaoYang/AuditPrice' target='_blank'>点此审核</a>");
            else
                TuhuMessage.SendEmail("［待审批］保养品价格修改申请", "xujian@tuhu.cn,yuanfang@tuhu.cn", $"{ThreadIdentity.Operator.Name}于{DateTime.Now}申请将{model.PID}保养品价格修改为{model.ApplyPrice}元，超过系统预警线，请您审批！<br/><a href='http://setting.tuhu.cn/BaoYang/AuditPrice' target='_blank'>点此审核</a>");
            return Json(result);

        }

        public ActionResult IsLowerThanActivityPrice(string pid, decimal price)
        {
            var data = PriceManager.GetFlashSalePriceByPID(pid);
            if (data == null || !data.Any())
                return Json(1);
            else
                return Json(data.Any(_ => _.Price >= price) ? 0 : 1);
        }

        [PowerManage]
        public ActionResult AuditPrice(string type = "Tuhu")
        {
            ViewBag.Type = type;
            var list = BaoYangPriceGuideManager.SelectNeedAuditBaoYang();
            if (string.Equals("Tuhu", type, StringComparison.OrdinalIgnoreCase))
            {
                list = list?.Where(x => string.IsNullOrWhiteSpace(x.Type) || string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                list = list?.Where(x => string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase));
            }
            return View(list);
        }

        public async Task<ActionResult> GotoExam(int pkid, string pid, decimal price, int type, decimal? cost, decimal? PurchasePrice, int? totalstock, int? num_week, int? num_month, decimal? guidePrice, decimal nowPrice, string maoliLv, string chaochu, decimal? jdself, decimal? maolie)
        {
            var resultExam = BaoYangPriceGuideManager.GotoAudit(type > 0, ThreadIdentity.Operator.Name, pid, cost, PurchasePrice, totalstock, num_week, num_month, guidePrice, nowPrice, maoliLv, chaochu, jdself, maolie);
            var model = BaoYangPriceGuideManager.FetchPriceAudit(pkid);
            if (type <= 0)
            {

                TuhuMessage.SendEmail("[被驳回]保养品价格修改申请", model.ApplyPerson, $"您于{DateTime.Now}申请将{model.PID}保养品价格修改为{model.ApplyPrice}元，已被{model.AuditPerson}驳回。");
                return Json(resultExam);
            }
            int result = -99;
            if (resultExam > 0 && type > 0)
            {
                if (!string.Equals(model.Type, "QPLPrice", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = await UpdatePriceAsync(pid, price, $"通过保养价格指导系统审核通过后修改({model.ApplyReason})");
                }
                else
                {
                    var manager = new BaoYangPriceGuideManager();
                    var dic = new Dictionary<string, decimal>();
                    dic.Add(pid, price);
                    var qplprice = manager.GetQPLPriceBypid(pid);
                    var msg = await manager.UpdateQPLProductPriceByPidAsync(dic, User.Identity.Name);
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        PriceManager.InsrtCouponPriceHistory(new CouponPriceHistory()
                        { PID = pid, OldPrice = qplprice, NewPrice = price, ChangeUser = User.Identity.Name, ChangeDateTime = DateTime.Now, ChangeReason = $"通过GLXT审核通过后修改(申请原因：{ model.ApplyReason})" });
                        result = 1;
                    }
                    else
                    {
                        result = -4;
                    }
                }
            }
            return Json(result);
        }

        public ActionResult AuditLog(string PID, int PageIndex = 1, int PageSize = 50, string type = "Tuhu")
        {
            if (string.IsNullOrWhiteSpace(PID))
                PID = null;
            PagerModel pager = new PagerModel(PageIndex, PageSize);
            var list = BaoYangPriceGuideManager.SelectAuditLogByPID(PID, pager, type);
            ViewBag.PID = PID;
            ViewBag.Type = type;
            return View(new ListModel<PriceUpdateAuditModel>() { Pager = pager, Source = list });
        }

        [PowerManage]
        public ActionResult ExportExcel(BaoYangPriceSelectModel selectModel, string pageType)
        {
            PagerModel pager = new PagerModel(1, 100000);
            var manager = new BaoYangPriceGuideManager();
            var list = manager.SelectBaoYangPriceGuide(selectModel, pager);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            var fileName = $"保养价格指导{ThreadIdentity.Operator.Name.Split('@')[0]}.xls";
            var rowNum = 0;
            row.CreateCell(rowNum).SetCellValue("品牌");
            row.CreateCell(rowNum++).SetCellValue("PID");
            row.CreateCell(rowNum++).SetCellValue("二级类目");
            row.CreateCell(rowNum++).SetCellValue("三级类目");
            row.CreateCell(rowNum++).SetCellValue("产品名称");
            row.CreateCell(rowNum++).SetCellValue("总库存");
            row.CreateCell(rowNum++).SetCellValue("仓库库存");
            row.CreateCell(rowNum++).SetCellValue("工场店库存");
            row.CreateCell(rowNum++).SetCellValue("近7天销量");
            row.CreateCell(rowNum++).SetCellValue("近30天销量");
            row.CreateCell(rowNum++).SetCellValue("周转天数");
            if (!pageType.Equals("see"))
            {
                row.CreateCell(rowNum++).SetCellValue("进货价");
                row.CreateCell(rowNum++).SetCellValue("最近一次采购价");
            }
            if (string.IsNullOrWhiteSpace(pageType))
            {
                row.CreateCell(rowNum++).SetCellValue("理论指导价");
                row.CreateCell(rowNum++).SetCellValue("实际指导价");
            }
            row.CreateCell(rowNum++).SetCellValue("官网价格");
            if (string.IsNullOrWhiteSpace(pageType))
            {
                row.CreateCell(rowNum++).SetCellValue("活动价");
            }
            if (!pageType.Equals("see"))
            {
                row.CreateCell(rowNum++).SetCellValue("途虎毛利率");
                row.CreateCell(rowNum++).SetCellValue("途虎毛利额");
            }
            row.CreateCell(rowNum++).SetCellValue("汽配龙");
            if (!pageType.Equals("see"))
            {
                row.CreateCell(rowNum++).SetCellValue("汽配龙毛利率");
                row.CreateCell(rowNum++).SetCellValue("汽配龙毛利额");
                row.CreateCell(rowNum++).SetCellValue("工场店毛利率");
                row.CreateCell(rowNum++).SetCellValue("工场店毛利额");
            }
            row.CreateCell(rowNum++).SetCellValue("京东自营");
            row.CreateCell(rowNum++).SetCellValue("特维轮天猫");
            row.CreateCell(rowNum++).SetCellValue("养车无忧官网");
            row.CreateCell(rowNum++).SetCellValue("汽车超人零售");
            row.CreateCell(rowNum++).SetCellValue("康众官网");
            row.CreateCell(rowNum++).SetCellValue("汽车超人批发");
            row.CreateCell(rowNum++).SetCellValue("途虎淘宝");
            row.CreateCell(rowNum++).SetCellValue("途虎淘宝2");
            row.CreateCell(rowNum++).SetCellValue("途虎天猫1");
            row.CreateCell(rowNum++).SetCellValue("途虎天猫2");
            row.CreateCell(rowNum++).SetCellValue("途虎天猫3");
            row.CreateCell(rowNum++).SetCellValue("途虎天猫4");
            row.CreateCell(rowNum++).SetCellValue("途虎京东");
            row.CreateCell(rowNum).SetCellValue("途虎京东旗舰");

            if (list.Item1 && list.Item2.Any())
            {
                var i = 0;

                foreach (var item in list.Item2)
                {
                    var rowtempNum = 0;
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(++i);
                    rowtemp.CreateCell(rowtempNum).SetCellValue(item.Brand);
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.PID);
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.SecondType);
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.ThirdType);
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.ProductName);
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.totalstock == null ? "" : item.totalstock.Value.ToString());
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.totalstock == null ? "" : (item.totalstock.Value - item.ShopStock).ToString());
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.ShopStock);
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.num_week == null ? "" : item.num_week.Value.ToString());
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.num_month == null ? "" : item.num_month.Value.ToString());
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.num_month == null || item.totalstock == null || item.num_month <= 0 || item.totalstock <= 0 
                    ? "" : 
                    Math.Ceiling((decimal) item.totalstock / item.num_month.Value * 30).ToString(CultureInfo.InvariantCulture));
                    if (!pageType.Equals("see"))
                    {
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.cost == null ? "" : item.cost.Value.ToString("0.00"));
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.PurchasePrice == null ? "" : item.PurchasePrice.Value.ToString("0.00"));
                    }
                    if (string.IsNullOrWhiteSpace(pageType))
                    {
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.TheoryGuidePrice == null
                                ? ""
                                : item.TheoryGuidePrice.Value.ToString("0.00"));
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.ActualGuidePrice == null
                                ? ""
                                : item.ActualGuidePrice.Value.ToString("0.00"));
                    }
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.Price.ToString("0.00"));
                    if (string.IsNullOrWhiteSpace(pageType))
                    {
                        rowtemp.CreateCell(rowtempNum++).SetCellValue(item.FlashSalePrice != null ? item.FlashSalePrice.Value.ToString("0.00") : "-");
                    }
                    if (!pageType.Equals("see"))
                    {
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.cost.GetValueOrDefault(0) > 0 && item.Price > 0
                                ? ((item.Price - item.cost.Value)/item.Price).ToString("0.00%")
                                : "");
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.cost.GetValueOrDefault(0) > 0 && item.Price > 0
                                ? (item.Price - item.cost.Value).ToString("0.00")
                                : "");
                    }
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.QPLPrice == null ? "" : item.QPLPrice.Value.ToString("0.00"));
                    if (!pageType.Equals("see"))
                    {
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.QPLPrice > 0 && item.cost > 0 ? (((decimal)item.QPLPrice - item.cost.Value) / (decimal)item.QPLPrice).ToString("0.00%") : "");
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.QPLPrice > 0 && item.cost > 0 ? ((decimal)item.QPLPrice - item.cost.Value).ToString("0.00") : "");
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.QPLPrice > 0 && item.Price > 0 ? ((item.Price - (decimal)item.QPLPrice) / item.Price).ToString("0.00%") : "");
                        rowtemp.CreateCell(rowtempNum++)
                            .SetCellValue(item.QPLPrice > 0 ? (item.Price - (decimal)item.QPLPrice).ToString("0.00") : "");
                    }
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.JDSelfPrice == null ? "" : item.JDSelfPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.TWLTMPrice == null ? "" : item.TWLTMPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.QccrlPrice == null ? "" : item.QccrlPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.YcwyPrice == null ? "" : item.YcwyPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.KzPrice == null ? "" : item.KzPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.QccrpPrice == null ? "" : item.QccrpPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.TBPrice == null ? "" : item.TBPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.TB2Price == null ? "" : item.TB2Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.TM1Price == null ? "" : item.TM1Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.TM2Price == null ? "" : item.TM2Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.TM3Price == null ? "" : item.TM3Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.TM4Price == null ? "" : item.TM4Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum++).SetCellValue(item.JDPrice == null ? "" : item.JDPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(rowtempNum).SetCellValue(item.JDFlagShipPrice == null ? "" : item.JDFlagShipPrice.Value.ToString("0.00"));
                }

            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        public JsonResult GetBaoYangShopStockByPid(string pid)
        {
            var result = BaoYangPriceGuideManager.SelectBaoYangShopStocks(pid);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PriceChangeLog(string pid)
        {
            return View(PriceManager.PriceChangeLog(pid));
        }

        public JsonResult GetProductCategoryByParentOid(int oid)
        {
            var result = BaoYangPriceGuideManager.SelectProductCategoryByParentOid(oid);
            return Json(new {data = result}, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult BaoYangPriceWeight(string firstType, string secondType, string thirdType)
        {
            BaoYangPriceGuideManager manager = new BaoYangPriceGuideManager();
            var model = manager.SelectGuideParaByType(firstType, secondType, thirdType);
            return PartialView(model);
        }

        public ActionResult GetFlashSalePriceByPid(string pid)
        {
            return Json(PriceManager.GetFlashSalePriceByPID(pid), JsonRequestBehavior.AllowGet);
        }
    }
}