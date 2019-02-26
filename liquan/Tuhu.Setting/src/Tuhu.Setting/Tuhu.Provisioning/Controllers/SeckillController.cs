using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using CacheClient = Tuhu.Service.Activity.CacheClient;
using Tuhu.Component.Log;

namespace Tuhu.Provisioning.Controllers
{
    public class SeckillController : Controller
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("SeckillController");

        [PowerManage(IwSystem = "OperateSys")]
        [HttpGet]
        public JsonResult GetSeckillList(string dt)
        {
            try
            {

                var datedt = string.IsNullOrEmpty(dt) ? DateTime.Now : Convert.ToDateTime(dt.Replace("GMT+0800 (中国标准时间)", ""));
                var week = datedt.DayOfWeek.ToString();
                var footDay = "";
                switch (week)
                {
                    case "Monday":
                        footDay = $"{datedt.ToShortDateString()}-{datedt.AddDays(6).ToShortDateString()}";
                        break;
                    case "Tuesday":
                        footDay = $"{datedt.AddDays(-1).ToShortDateString()}-{datedt.AddDays(5).ToShortDateString()}";
                        break;
                    case "Wednesday":
                        footDay = $"{datedt.AddDays(-2).ToShortDateString()}-{datedt.AddDays(4).ToShortDateString()}";
                        break;
                    case "Thursday":
                        footDay = $"{datedt.AddDays(-3).ToShortDateString()}-{datedt.AddDays(3).ToShortDateString()}";
                        break;
                    case "Friday":
                        footDay = $"{datedt.AddDays(-4).ToShortDateString()}-{datedt.AddDays(2).ToShortDateString()}";
                        break;
                    case "Saturday":
                        footDay = $"{datedt.AddDays(-5).ToShortDateString()}-{datedt.AddDays(1).ToShortDateString()}";
                        break;
                    case "Sunday":
                        footDay = $"{datedt.AddDays(-6).ToShortDateString()}-{datedt.ToShortDateString()}";
                        break;
                    default:
                        break;
                }
                var result = SeckillManager.SelectSeckillList(datedt);
                return Json(new
                {
                    result,
                    FooterDay = footDay
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public JsonResult GetEditDatasByActivityId(string activityId, bool isDefault, string strStatus)
        {
            QiangGouModel model = new QiangGouModel();
            if (strStatus == "待审核" || strStatus == "已驳回")
            {
                Guid activityID;
                if (Guid.TryParse(activityId, out activityID))
                    model = QiangGouManager.FetchNeedExamQiangGouAndProducts(activityID);
            }
            else
            {
                model = SeckillManager.FetchNeedExamQiangGouAndProducts(new Guid(activityId));
            }

            return Json(
                model?.Products ?? new List<QiangGouProductModel>()
            , JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ExamDetail(string aid)
        {
            QiangGouModel model = null;
            Guid activityID;
            if (Guid.TryParse(aid, out activityID))
                model = QiangGouManager.FetchNeedExamQiangGouAndProducts(activityID);
            return View(model);
        }

        /// <summary>
        /// 获取商品信息，验证pid是否有效
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult FetchProductWithCostPriceByPid(string pid, string activityId)
        {
            var product = CheckPid(pid, activityId);
            return Json(product, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取商品信息，验证pid是否有效
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private ProductModel CheckPid(string pid, string activityId)
        {
            var product = new ProductModel();
            using (var client = new ProductClient())
            {
                var sku = client.FetchProduct(pid);
                sku.ThrowIfException(true);
                if (sku.Success && sku.Result != null)
                {
                    product.PID = sku.Result.Pid;
                    product.OnSale = sku.Result.Onsale;
                    product.Stockout = sku.Result.Stockout;
                    product.DisplayName = sku.Result.DisplayName;
                    product.Image = sku.Result.Image;
                    product.cy_list_price = sku.Result.Price;
                }
            };

            if (!string.IsNullOrEmpty(product.PID) && product.PID != pid)
            {
                product.CaseSensitive = true;
            }
            if (!string.IsNullOrEmpty(activityId))
            {
                var saleoutqty = QiangGouManager.SelectFlashSaleSaleOutQuantity(activityId, pid);
                product.TotalQuantity = saleoutqty;
                product.SaleOutQuantity = saleoutqty ?? 0;
            }
            var costPriceModel = QiangGouManager.SelectcostPriceSql(pid).FirstOrDefault();
            if (costPriceModel != null)
                product.CostPrice = costPriceModel.CostPrice;
            product = QiangGouManager.AssemblyProductInstalService(product);

            return product;
        }

        [HttpPost]
        public ActionResult Save(string strmodel, string products, string originProducts)
        {
            try
            {
                if (strmodel == null)
                    return Json(new { Status = 0, Message = "保存失败【活动内容为空】" });
                var model1 = JsonConvert.DeserializeObject<QiangGouModel>(strmodel);
                if (model1.StrStatus == "已驳回")
                {
                    if (SeckillManager.SelectQiangGouIsExist(model1.ActivityID.ToString()) == 0)
                    {
                        var delResult = SeckillManager.DeleteFirstCreateActivityApproveBack(model1.ActivityID.ToString());
                        if (delResult == -1)
                        {
                            return Json(new { Status = 0, Message = "保存失败【请重试】" });
                        }
                        model1.ActivityID = null;
                    }
                }
                var model = new QiangGouModel();

                if (model1.IsDefault)
                {
                    switch (model1.ActivityName)
                    {
                        case "0点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  00:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  10:00:00"));
                            break;
                        case "10点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  10:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  13:00:00"));
                            break;
                        case "13点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  13:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  16:00:00"));
                            break;
                        case "16点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  16:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  20:00:00"));
                            break;
                        case "20点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  20:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  23:59:59"));
                            break;
                    }
                    model.IsDefault = model1.IsDefault;
                    model.ActivityID = model1.ActivityID;
                    model.ActiveType = 1;
                    model.ActivityName = $"默认配置:{model1.ActivityName}";
                    model.NeedExam = model1.NeedExam;
                }
                else
                {
                    switch (model1.ActivityName)
                    {
                        case "0点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  00:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  10:00:00"));
                            break;
                        case "10点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  10:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  13:00:00"));
                            break;
                        case "13点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  13:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  16:00:00"));
                            break;
                        case "16点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  16:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  20:00:00"));
                            break;
                        case "20点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  20:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  23:59:59"));
                            break;
                    }
                    model.IsDefault = model1.IsDefault;
                    model.ActivityID = model1.ActivityID;
                    model.ActiveType = 1;
                    model.ActivityName = $"{model1.ShortDate}:{model1.ActivityName}";
                    model.NeedExam = model1.NeedExam;
                }
                var isUpdate = model.ActivityID != null;
                var originModels = new List<QiangGouProductModel>();
                if (originProducts != "[]" && !string.IsNullOrWhiteSpace(originProducts))
                {
                    originModels = JsonConvert.DeserializeObject<List<QiangGouProductModel>>(originProducts);
                }
                model.Products = JsonConvert.DeserializeObject<List<QiangGouProductModel>>(products);
                List<QiangGouDiffModel> qiangGouDiff = new List<QiangGouDiffModel>();
                if (!model.Products.Any())
                    return Json(new { Status = 0, Message = "保存失败【活动无产品】" });

                var result = QiangGouManager.Save(model, qiangGouDiff);
                if (result.Item1 == -1)
                    return Json(new { Status = 0, Message = "保存失败【闪购活动时间不允许重叠】" });
                if (result.Item1 == -3)
                    return Json(new { Status = 0, Message = "刷新缓存失败【请手动刷新】" });
                else if (result.Item1 > 0)
                {
                    if (model.NeedExam)
                    {
                        var approveResult = SeckillManager.UpdateSeckillToToApprove(result.Item2.ToString());
                        {
                            if (approveResult <= 0)
                            {
                                return Json(new { Status = 0, Message = "保存失败重试" });
                            }
                        }
                    }

                    if (!isUpdate)
                    {
                        SeckillManager.OpertionLogs(SeckillManager.OpertionType.Add, model.Products.Count().ToString(),
                            "", result.Item2.ToString());
                    }
                    else
                    {
                        var logdata = CompareModifyData(originModels, model.Products.ToList());
                        foreach (var log in logdata)
                        {
                            if (log.Key == SeckillManager.OpertionType.EditEdit)
                            {
                                var count = log.Value.Length;
                                //先拆成两段来存储，如果后面出现还存不下的想办法多条存储
                                SeckillManager.OpertionLogs(log.Key, log.Value.Substring(0, count / 2),
                                    log.Value.Substring(count / 2), result.Item2.ToString());
                            }
                            else
                            {
                                SeckillManager.OpertionLogs(log.Key, log.Value, "", result.Item2.ToString());
                            }

                        }
                    }
                    var request = new ActivityTypeRequest()
                    {
                        ActivityId = model.ActivityID.Value,
                        StartDateTime = model.StartDateTime,
                        EndDateTime = model.EndDateTime,
                        Status = 1,
                        Type = 1
                    };
                    var activityTypeResult = QiangGouManager.RecordActivityType(request);
                    if (!activityTypeResult)
                    {
                        return Json(new { Status = 0, Message = "保存失败重试" });
                    }
                    if (model.ActivityID != null && !model.NeedExam)
                    {
                        var cache = QiangGouManager.ReflashQiangGouCache(model.ActivityID.Value, false, model.ActiveType);
                        if (cache == false)
                            return Json(new { Status = 0, Message = "刷新缓存失败【请手动刷新】" });
                    }
                    return Json(new { Status = 1, Message = "保存成功" });
                }

                return Json(new { Status = 0, Message = "保存失败【未知错误】" });
            }
            catch (Exception e)
            {
                return Json(new { Status = 0, Message = e.Message + e.InnerException + e.StackTrace });
            }
        }

        #region 导入 
        public ActionResult ExportProducts()
        {

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.FileName.Contains(".xlsx") || file.FileName.Contains(".xls"))
                {
                    var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("Sheet1", true);
                    IEnumerable<QiangGouSeckillImportModel> data = GetDataFromExcel(dt);
                    if (data.Any())
                        return CheckExcelData(data);
                    else
                        return Content(JsonConvert.SerializeObject(new { code = -1 }));
                }
            }
            return Content(JsonConvert.SerializeObject(new { code = 0 }));
        }

        private ActionResult CheckExcelData(IEnumerable<QiangGouSeckillImportModel> data)
        {
            int excelRow = 1;
            List<QiangGouProductModel> products = new List<QiangGouProductModel>();
            List<string> errorMessage = new List<string>();
            Dictionary<string, string> installPay = new Dictionary<string, string>() {
                { "不限",""}, { "在线支付" ,"PayOnline"}, { "到店安装" ,"InstallAtShop"}, { "在线支付且到店安装","PayOnlineAndInstallAtShop" }
        };
            Dictionary<string, bool> isUseCode = new Dictionary<string, bool>()
            {
                {"使用",true }, {"不使用",false }
            };

            Dictionary<string, bool> isyes = new Dictionary<string, bool>() { { "是", true }, { "否", false } };
            Dictionary<string, string> channels = new Dictionary<string, string> { { "全部", "all" }, { "仅网站", "pc" }, { "仅APP", "app" } };
            List<string> tags = new List<string>() { "无", "热卖", "秒杀", "限量" };
            foreach (var model in data)
            {
                excelRow++;
                StringBuilder sb = new StringBuilder();
                #region PID
                if (string.IsNullOrWhiteSpace(model.PID))
                    sb.Append("PID为空_");
                #endregion

                #region 排序
                int postion = 0;
                if (!string.IsNullOrWhiteSpace(model.排序) && !int.TryParse(model.排序, out postion))
                    sb.Append("排序不合法_");
                #endregion

                #region 促销价
                decimal activityPrice = 0;
                if (!string.IsNullOrWhiteSpace(model.秒杀价) && !decimal.TryParse(model.秒杀价, out activityPrice))
                    sb.Append("促销价不合法_");
                #endregion

                #region 伪原价
                decimal falsePrice = 0;
                if (!string.IsNullOrWhiteSpace(model.伪原价) && !decimal.TryParse(model.伪原价, out falsePrice))
                    sb.Append("伪原价不合法_");
                #endregion

                #region 每人限购
                int max = 0;
                if (!string.IsNullOrWhiteSpace(model.每人限购) && !int.TryParse(model.每人限购, out max))
                    sb.Append("每人限购不合法_");
                #endregion

                #region 总限购
                int total = 0;
                if (!string.IsNullOrWhiteSpace(model.秒杀库存) && !int.TryParse(model.秒杀库存, out total))
                    sb.Append("总限购不合法_");

                #endregion



                #region 优惠券
                if (string.IsNullOrWhiteSpace(model.优惠券))
                    model.优惠券 = "不使用";
                if (!isUseCode.ContainsKey(model.优惠券))
                    sb.Append("优惠券不合法_");
                #endregion


                #region 显示
                if (string.IsNullOrWhiteSpace(model.显示))
                    model.显示 = "是";
                if (!isyes.ContainsKey(model.显示))
                    sb.Append("显示不合法_");
                #endregion
                var error = sb.ToString();
                if (!string.IsNullOrWhiteSpace(error))
                    errorMessage.Add("第" + excelRow + "行：" + error);
                else
                {
                    var productModel = new QiangGouProductModel()
                    {
                        PID = model.PID,
                        Position = postion,
                        ProductName = model.秒杀标题,
                        Price = activityPrice,
                        FalseOriginalPrice = falsePrice,
                        MaxQuantity = max,
                        TotalQuantity = total,
                        IsUsePCode = isUseCode[model.优惠券],
                        Channel = "all",
                        IsShow = isyes[model.显示]
                    };
                    //var assembalModel = QiangGouManager.AssemblyProductInstalService(productModel);
                    products.Add(productModel);
                }
            }
            return Content(JsonConvert.SerializeObject(new { data = products, error = errorMessage, code = 1 }));
        }

        private IEnumerable<QiangGouSeckillImportModel> GetDataFromExcel(DataTable dt)
        {
            var data = dt.ConvertTo<QiangGouSeckillImportModel>();
            data = data.Where(_ => !(string.IsNullOrWhiteSpace(_.PID)
              && string.IsNullOrWhiteSpace(_.优惠券)
              && string.IsNullOrWhiteSpace(_.伪原价)
              && string.IsNullOrWhiteSpace(_.秒杀价)
              && string.IsNullOrWhiteSpace(_.秒杀标题)
              && string.IsNullOrWhiteSpace(_.秒杀库存)
              && string.IsNullOrWhiteSpace(_.排序)
              && string.IsNullOrWhiteSpace(_.显示)
              && string.IsNullOrWhiteSpace(_.每人限购))
             );
            if (!data.Any())
                return new QiangGouSeckillImportModel[0];
            data = data.Select(_ =>
            {
                return new QiangGouSeckillImportModel()
                {
                    PID = getEmptyOrTrim(_.PID),
                    优惠券 = getEmptyOrTrim(_.优惠券),
                    伪原价 = getEmptyOrTrim(_.伪原价),
                    秒杀价 = getEmptyOrTrim(_.秒杀价),
                    秒杀标题 = getEmptyOrTrim(_.秒杀标题),
                    秒杀库存 = getEmptyOrTrim(_.秒杀库存),
                    排序 = getEmptyOrTrim(_.排序),
                    显示 = getEmptyOrTrim(_.显示),
                    每人限购 = getEmptyOrTrim(_.每人限购)
                };
            });
            return data;
        }

        private string getEmptyOrTrim(string str)
               => str == null ? string.Empty : str.Trim();
        #endregion
        public ActionResult ExamActivity(Guid aid)
        {
            try
            {
                var result = QiangGouManager.ExamActivity(aid);
                if (result > 0)
                {
                    var del = SeckillManager.DeleteStatusData(aid.ToString());
                    if (del > 0)
                    {
                        result = 1;
                    }
                }
                if (result > 0)
                {
                    var cache = RefershSekillCache(aid);
                    if (!cache)
                        return Json(new
                        {
                            Status = 0,
                            Message = "刷新缓存失败【请手动刷新】"
                        });
                    SeckillManager.OpertionLogs(SeckillManager.OpertionType.ApprovePass, "", "", aid.ToString());
                }
                return Json(new
                {
                    code = result
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    code = -3,
                    msg = e.Message + e.InnerException + e.StackTrace
                });
            }
        }
        public ActionResult ApproveBack(Guid aid)
        {
            try
            {
                var result = SeckillManager.ApproveBack(aid.ToString());
                if (result > 0)
                {
                    SeckillManager.OpertionLogs(SeckillManager.OpertionType.ApproveBack, "", "", aid.ToString());
                }
                return Json(new
                {
                    code = result
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    code = -3,
                    msg = e.Message + e.InnerException + e.StackTrace
                });
            }
        }

        private Dictionary<SeckillManager.OpertionType, string> CompareModifyData(List<QiangGouProductModel> model1,
            List<QiangGouProductModel> model2)
        {
            var dic = new Dictionary<SeckillManager.OpertionType, string>();
            var beforePids = model1.Select(r => r.PID).ToList();
            var afterPids = model2.Select(r => r.PID).ToList();
            var deldata = beforePids.Except(afterPids);
            var adddata = afterPids.Except(beforePids);
            if (adddata.Any())
            {
                dic.Add(SeckillManager.OpertionType.EditAdd, string.Join(",", adddata));
            }
            if (deldata.Any())
            {
                dic.Add(SeckillManager.OpertionType.EditDelete, string.Join(",", deldata));
            }
            var sumReamrk = "";
            foreach (var item in model1)
            {
                var afterItem = model2.FirstOrDefault(r => r.PID == item.PID);
                if (afterItem != null)
                {
                    var remark = $"将{item.PID}的";
                    if (item.Position != afterItem.Position)
                    {
                        remark += $"排序从{item.Position}改为{afterItem.Position}";
                    }
                    if (item.ProductName != afterItem.ProductName)
                    {
                        remark += $"秒杀标题从{item.ProductName}改为{afterItem.ProductName}";
                    }
                    if (item.Price != afterItem.Price)
                    {
                        remark += $"秒杀价从{item.Price}改为{afterItem.Price}";
                    }
                    if (item.FalseOriginalPrice != afterItem.FalseOriginalPrice)
                    {
                        remark += $"伪原价从{item.FalseOriginalPrice}改为{afterItem.FalseOriginalPrice}";
                    }
                    if (item.MaxQuantity != afterItem.MaxQuantity)
                    {
                        remark += $"每人限购从{item.MaxQuantity}改为{afterItem.MaxQuantity}";
                    }
                    if (item.TotalQuantity != afterItem.TotalQuantity)
                    {
                        remark += $"秒杀库存从{item.TotalQuantity}改为{afterItem.TotalQuantity}";
                    }
                    if (item.IsUsePCode != afterItem.IsUsePCode)
                    {
                        remark += $"优惠券从{item.IsUsePCode}改为{afterItem.IsUsePCode}";
                    }
                    if (item.IsShow != afterItem.IsShow)
                    {
                        remark += $"是否显示从{item.IsShow}改为{afterItem.IsShow}";
                    }
                    if (remark != $"将{item.PID}的")
                    {
                        sumReamrk += remark + "<br/>";
                    }
                }
            }
            dic.Add(SeckillManager.OpertionType.EditEdit, sumReamrk);
            return dic;
        }


        public JsonResult GetLogList(string activityId)
        {
            if (string.IsNullOrEmpty(activityId))
                return Json(null);
            var result = LoggerManager.SelectFlashSaleHistoryByLogId(activityId, "Seckill");
            var log = result.Select(r =>
            {
                switch (r.Operation)
                {
                    case "Add":
                        r.Operation = "新建活动";
                        r.Remark = $"新建{r.AfterValue}";
                        return r;
                    case "EditEdit":
                        r.Operation = "修改";
                        r.Remark = $"{r.BeforeValue}{r.AfterValue}";
                        return r;
                    case "EditDelete":
                        r.Operation = "删除";
                        r.Remark = $"删除的pid有{r.BeforeValue}";
                        return r;
                    case "EditAdd":
                        r.Operation = "添加";
                        r.Remark = $"添加的pid有{r.BeforeValue}";
                        return r;
                    case "ApprovePass":
                        r.Operation = "审核通过";
                        r.Remark = $"";
                        return r;
                    case "ApproveBack":
                        r.Operation = "审核驳回";
                        r.Remark = $"";
                        return r;
                    default:
                        return r;
                }

            });
            return Json(log.Select(r => new
            {
                Title = r.Operation,
                Name = r.OperateUser,
                CreateDateTime = r.CreateDateTime,
                Remark = r.Remark
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///天天秒杀活动页刷新缓存按钮调用（2018-10-15修改天天秒杀刷新按钮启用,使用SpikeRefreshCache方法）
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public JsonResult RefreshCache(string activityId)
        {
            try
            {
                using (var client = new FlashSaleClient())
                {
                    var result = client.UpdateFlashSaleDataToCouchBaseByActivityID(new Guid(activityId));
                    result.ThrowIfException(true);
                    if (result.Success && result.Result)
                    {
                        return Json(new
                        {

                            code = "1",
                            msg = "成功"

                        });

                    }
                    else
                    {
                        return Json(new
                        {

                            code = "0",
                            msg = "失败"

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {

                    code = "0",
                    msg = ex.Message

                });
            }
        }


        #region 新天天秒杀独立调用方法
        /// <summary>
        /// 天天秒杀刷新缓存按钮事件
        /// （此方法目前独立调用，与其他业务并无耦合。如需修改请增加注释）
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public JsonResult SpikeRefreshCache(string activityId)
        {
            try
            {
                using (var client = new FlashSaleClient())
                {
                    var result = client.SpikeListRefresh(new Guid(activityId));
                    result.ThrowIfException(true);
                    if (result.Success && result.Result)
                    {
                        return Json(new
                        {

                            code = "1",
                            msg = "成功"

                        });

                    }
                    else
                    {
                        return Json(new
                        {

                            code = "0",
                            msg = "失败"

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {

                    code = "0",
                    msg = ex.Message

                });
            }
        }

        /// <summary>
        /// 天天秒杀保存按钮事件
        /// （此方法目前独立调用，与其他业务并无耦合。如需修改请增加注释）
        /// </summary>
        /// <param name="strmodel">活动基本信息实体</param>
        /// <param name="products">活动关联的产品实体</param>
        /// <param name="originProducts"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SpikeSave(string strmodel, string products, string originProducts)
        {
            try
            {
                if (strmodel == null)
                    return Json(new { Status = 0, Message = "保存失败【活动内容为空】" });
                var model1 = JsonConvert.DeserializeObject<QiangGouModel>(strmodel);
                var model = new QiangGouModel();
                model.Products = JsonConvert.DeserializeObject<List<QiangGouProductModel>>(products);

                if ((bool)model.Products?.Any())
                {
                    foreach (var item in model.Products)
                    {
                        var checkModel = CheckPid(item.PID, null);
                        if (checkModel == null || string.IsNullOrWhiteSpace(checkModel.PID)
                            || checkModel.CaseSensitive || !checkModel.OnSale || checkModel.Stockout)
                        {
                            //Status = -1 表示pid验证未通过 前端重新验证
                            return Json(new { Status = 0, Message = $"PID：{checkModel.PID};校验未通过" });
                        }

                        item.PID = checkModel.PID;
                    }
                }

                //已驳回的活动删除审核表活动信息,与产品信息
                if (model1.StrStatus == "已驳回")
                {
                    if (SeckillManager.SelectQiangGouIsExist(model1.ActivityID.ToString()) == 0)
                    {
                        var delResult = SeckillManager.DeleteFirstCreateActivityApproveBack(model1.ActivityID.ToString());
                        if (delResult == -1)
                        {
                            return Json(new { Status = 0, Message = "保存失败【请重试】" });
                        }
                        model1.ActivityID = null;
                    }
                }

                if (model1.IsDefault)
                {
                    switch (model1.ActivityName)
                    {
                        case "0点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  00:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  10:00:00"));
                            break;
                        case "10点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  10:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  13:00:00"));
                            break;
                        case "13点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  13:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  16:00:00"));
                            break;
                        case "16点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  16:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  20:00:00"));
                            break;
                        case "20点场":
                            model.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("2018-04-26  20:00:00"));
                            model.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("2019-04-26  23:59:59"));
                            break;
                    }
                    model.IsDefault = model1.IsDefault;
                    model.ActivityID = model1.ActivityID;
                    model.ActiveType = 1;
                    model.ActivityName = $"默认配置:{model1.ActivityName}";
                    model.NeedExam = model1.NeedExam;
                }
                else
                {
                    switch (model1.ActivityName)
                    {
                        case "0点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  00:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  10:00:00"));
                            break;
                        case "10点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  10:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  13:00:00"));
                            break;
                        case "13点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  13:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  16:00:00"));
                            break;
                        case "16点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  16:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  20:00:00"));
                            break;
                        case "20点场":
                            model.StartDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  20:00:00"));
                            model.EndDateTime = Convert.ToDateTime(Convert.ToDateTime(model1.ShortDate).ToString("yyyy-MM-dd  23:59:59"));
                            break;
                    }
                    model.IsDefault = model1.IsDefault;
                    model.ActivityID = model1.ActivityID;
                    model.ActiveType = 1;
                    model.ActivityName = $"{model1.ShortDate}:{model1.ActivityName}";
                    model.NeedExam = model1.NeedExam;
                }
                var isUpdate = model.ActivityID != null;
                var originModels = new List<QiangGouProductModel>();
                if (originProducts != "[]" && !string.IsNullOrWhiteSpace(originProducts))
                {
                    originModels = JsonConvert.DeserializeObject<List<QiangGouProductModel>>(originProducts);
                }

                //查询修改前 审核临时表的活动、活动商品数据
                var origin = new QiangGouModel();
                var origin_Temp = new QiangGouModel();

                if (isUpdate)
                {
                    try
                    {
                        origin = QiangGouManager.FetchQiangGouAndProducts(model.ActivityID.Value);
                        origin_Temp = QiangGouManager.FetchQiangGouAndProductsTemp(model.ActivityID.Value);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, $"SpikeSave=>FetchQiangGouAndProducts_Temp异常", ex);
                    }
                }

                List<QiangGouDiffModel> qiangGouDiff = new List<QiangGouDiffModel>();
                if (!model.Products.Any())
                    return Json(new { Status = 0, Message = "保存失败【活动无产品】" });

                var result = QiangGouManager.SpikeSave(model, qiangGouDiff);
                if (result.Item1 == -1)
                    return Json(new { Status = 0, Message = "保存失败【闪购活动时间不允许重叠】" });
                if (result.Item1 == -3)
                    return Json(new { Status = 0, Message = "刷新缓存失败【请手动刷新】" });
                else if (result.Item1 > 0)
                {
                    if (model.NeedExam)
                    {
                        var approveResult = SeckillManager.UpdateSeckillToToApprove(result.Item2.ToString());
                        {
                            if (approveResult <= 0)
                            {
                                return Json(new { Status = 0, Message = "保存失败重试" });
                            }
                        }
                    }

                    if (!isUpdate)
                    {
                        SeckillManager.OpertionLogs(SeckillManager.OpertionType.Add, model.Products.Count().ToString(),
                            "", result.Item2.ToString());
                    }
                    else
                    {
                        var logdata = CompareModifyData(originModels, model.Products.ToList());
                        foreach (var log in logdata)
                        {
                            if (log.Key == SeckillManager.OpertionType.EditEdit)
                            {
                                var count = log.Value.Length;
                                //先拆成两段来存储，如果后面出现还存不下的想办法多条存储
                                SeckillManager.OpertionLogs(log.Key, log.Value.Substring(0, count / 2),
                                    log.Value.Substring(count / 2), result.Item2.ToString());
                            }
                            else
                            {
                                SeckillManager.OpertionLogs(log.Key, log.Value, "", result.Item2.ToString());
                            }
                        }

                        //备份被删除的数据
                        InsertDeletedAfterSave(model,origin,origin_Temp);
                    }
                    var request = new ActivityTypeRequest()
                    {
                        ActivityId = model.ActivityID.Value,
                        StartDateTime = model.StartDateTime,
                        EndDateTime = model.EndDateTime,
                        Status = 1,
                        Type = 1
                    };
                    var activityTypeResult = QiangGouManager.RecordActivityType(request);
                    if (!activityTypeResult)
                    {
                        return Json(new { Status = 0, Message = "保存失败重试" });
                    }
                    if (model.ActivityID != null && !model.NeedExam)
                    {
                        var cache = QiangGouManager.SpikeReflashCache(model.ActivityID.Value, false);
                        if (cache == false)
                            return Json(new { Status = 0, Message = "刷新缓存失败【请手动刷新】" });
                    }
                    return Json(new { Status = 1, Message = "保存成功" });
                }

                return Json(new { Status = 0, Message = "保存失败【未知错误】" });
            }
            catch (Exception e)
            {
                return Json(new { Status = 0, Message = e.Message + e.InnerException + e.StackTrace });
            }
        }

        /// <summary>
        /// 保存后 备份被删除的数据
        /// </summary>
        /// <param name="model">保存后的活动数据</param>
        /// <param name="origin">保存前活动数据</param>
        /// <param name="origin_Temp">保存前审核临时表的活动数据</param>
        private void InsertDeletedAfterSave(QiangGouModel model, QiangGouModel origin, 
            QiangGouModel origin_Temp)
        {
            //备份 审核的Temp表数据
            try
            {
                if (model.NeedExam)
                {
                    if (origin_Temp != null && origin_Temp.ActivityID != Guid.Empty)
                    {
                        var insertDeleteActivity = QiangGouManager.InsertDeletedActivityInfo_Temp(origin_Temp);
                        var insertDeleteProducuts = QiangGouManager.InsertDeletedActivityProducts_Temp(origin_Temp?.Products?.ToList());
                    }
                }
                else
                {
                    //备份修改活动删掉的商品信息
                    var modelPids = model.Products?.Select(x => x.PID);
                    var deleteProducts = origin.Products.Where(x => (bool)!modelPids?.Contains(x.PID))?.ToList();
                    if (deleteProducts?.Count() > 0)
                    {
                        var insertDeleteProducuts = QiangGouManager.InsertDeletedActivityProducts(deleteProducts);
                        if (insertDeleteProducuts <= 0)
                        {
                            Logger.Log(Level.Warning, $"InsertDeletedAfterSave=>InsertDeletedActivityProducts失败，" +
                                $"{JsonConvert.SerializeObject(deleteProducts)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"InsertDeletedAfterSave=>记录活动审核临时数据失败,{JsonConvert.SerializeObject(origin_Temp)}", ex);
            }
        }

        /// <summary>
        ///天天秒杀审核按钮事件
        /// （此方法目前独立调用，与其他业务并无耦合。如需修改请增加注释）
        /// </summary>
        /// <param name="aid">活动ID</param>
        /// <returns></returns>
        public ActionResult SpikeExamActivity(Guid aid)
        {
            try
            {
                //查询修改前 活动、活动商品数据和审核临时表的数据
                var origin = new QiangGouModel();
                var origin_Temp = new QiangGouModel();
                var statusModel = new ActivityApprovalStatus();
                try
                {
                    origin = QiangGouManager.FetchQiangGouAndProducts(aid);
                    origin_Temp = QiangGouManager.FetchQiangGouAndProductsTemp(aid);
                    statusModel = SeckillManager.SelectActivityStatusModel(aid);
                }
                catch (Exception ex)
                {
                    Logger.Log(Level.Error, $"SpikeExamActivity=>FetchQiangGouAndProducts_Temp异常", ex);
                }

                //调用存过同步审核表数据
                var result = QiangGouManager.ExamActivity(aid);
                
                if (result > 0)
                {
                    //删除审核状态
                    var del = SeckillManager.DeleteStatusData(aid.ToString());
                    if (del > 0)
                    {
                        result = 1;
                    }
                }
                if (result > 0)
                {
                    var cache = SpikeRefershCache(aid);

                    //备份删除数据
                    InsertDeleteInfosAfterExamActivity(aid, origin, origin_Temp, statusModel);

                    if (!cache)
                        return Json(new
                        {
                            Status = 0,
                            Message = "刷新缓存失败【请手动刷新】"
                        });
                    SeckillManager.OpertionLogs(SeckillManager.OpertionType.ApprovePass, "", "", aid.ToString());
                }
                return Json(new
                {
                    code = result
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    code = -3,
                    msg = e.Message + e.InnerException + e.StackTrace
                });
            }
        }

        /// <summary>
        /// 审核后备份删除的数据: 备份活动商品被删除的项、 审核临时表该活动的全部数据
        /// </summary>
        /// <param name="origin">审核前活动数据</param>
        /// <param name="origin_Temp">审核前 审核临时表的活动数据</param>
        private void InsertDeleteInfosAfterExamActivity(Guid activityId, QiangGouModel origin,
            QiangGouModel origin_Temp, ActivityApprovalStatus statusModel)
        {
            try
            {
                if (origin != null && origin.ActivityID != Guid.Empty)
                {
                    //审核后活动中被删除的商品进行备份
                    var modelPids = origin_Temp.Products?.Select(x => x.PID);
                    var deleteProducts = origin.Products.Where(x => (bool)!modelPids?.Contains(x.PID))?.ToList();
                    if (deleteProducts?.Count() > 0)
                    {
                        var insertDeleteProducuts = QiangGouManager.InsertDeletedActivityProducts(deleteProducts);
                        if (insertDeleteProducuts <= 0)
                        {
                            Logger.Log(Level.Warning, $"InsertDeleteInfosAfterExamActivity=>InsertDeletedActivityProducts失败，" +
                                $"{JsonConvert.SerializeObject(deleteProducts)}");
                        }
                    }
                }

                if (origin_Temp != null && origin.ActivityID != Guid.Empty)
                {
                    //记录到活动审核备份表
                    var insertDeleteActivity_Temp = QiangGouManager.InsertDeletedActivityInfo_Temp(origin_Temp);
                    if (insertDeleteActivity_Temp <= 0)
                    {
                        Logger.Log(Level.Warning, $"InsertDeleteInfosAfterExamActivity,备份审核活动数据失败，" +
                            $"{JsonConvert.SerializeObject(origin_Temp)}");
                    }

                    if (origin_Temp?.Products?.Count() > 0)
                    {
                        //记录到活动商品审核备份表
                        var insertDeleteActivityProduct_Temp = QiangGouManager.InsertDeletedActivityProducts_Temp(origin_Temp?.Products?.ToList());
                        if (insertDeleteActivityProduct_Temp <= 0)
                        {
                            Logger.Log(Level.Warning, $"InsertDeleteInfosAfterExamActivity,备份审核活动商品数据失败，" +
                                $"{JsonConvert.SerializeObject(origin_Temp)}");
                        }
                    }

                    //记录到活动状态备份表
                    if (statusModel != null)
                    {
                        var insertStatusModel = SeckillManager.InsertActivityApprovalStatus(statusModel);
                        if (insertStatusModel <= 0)
                        {
                            Logger.Log(Level.Warning, $"InsertActivityApprovalStatus,备份活动商品数据失败，" +
                                $"{JsonConvert.SerializeObject(insertStatusModel)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"InsertDeleteInfosAfterExamActivity 备份数据异常," +
                    $"origin:{JsonConvert.SerializeObject(origin)},origin_temp:{JsonConvert.SerializeObject(origin_Temp)}", ex);
            }
        }

        /// <summary>
        /// 天天秒杀审核按钮私有方法
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        private bool SpikeRefershCache(Guid aid)
        {
            //刷新缓存
            var cache = QiangGouManager.SpikeReflashCache(aid, false);
            if (cache == false)
                return false;

            //发送通知
            var pids = SeckillManager.SelectActivityProductPids(aid);
            Thread.Sleep(50);
            TuhuNotification.SendNotification("notification.productModify.ProductCommonTag", new
            {
                type = "RebuildCache",
                pids,
                tag = ProductCommonTag.Seckilling

            });
            return true;
        }

        #endregion

        private bool RefershSekillCache(Guid aid)
        {
            using (var client = new CacheClient())
            {
                var cacheresult = client.RefreshRedisCachePrefixForCommon(new RefreshCachePrefixRequest()
                {
                    Prefix = "SecondKillPrefix",
                    ClientName = "FlashSale",
                    Expiration = TimeSpan.FromDays(1)
                });
                cacheresult.ThrowIfException(true);
            }
            var cache = QiangGouManager.ReflashQiangGouCache(aid, false, 1);
            if (cache == false)
                return false;
            var pids = SeckillManager.SelectActivityProductPids(aid);
            Thread.Sleep(50);
            TuhuNotification.SendNotification("notification.productModify.ProductCommonTag", new
            {
                type = "RebuildCache",
                pids,
                tag = ProductCommonTag.Seckilling

            });
            return true;
        }

    }
}