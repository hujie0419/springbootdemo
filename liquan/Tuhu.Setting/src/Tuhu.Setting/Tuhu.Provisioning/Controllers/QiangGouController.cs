using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.ProductQuery;
using CacheClient = Tuhu.Service.SearchProduct.CacheClient;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;

namespace Tuhu.Provisioning.Controllers
{
    public class QiangGouController : Controller
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("QiangGouController");

        public ActionResult Index(string type, QianggouSelectModel request)
        {
            var model = QiangGouManager.SelectAllQiangGou();
            model = model.Where(Q => Q.ActiveType == request.type
            && (request.aid == null || Q.ActivityID == request.aid.Value) && (string.IsNullOrWhiteSpace(request.aname) || Q.ActivityName.Contains(request.aname)) && (request.stime == null || request.stime.Value <= Q.StartDateTime) && (request.etime == null || request.etime.Value >= Q.EndDateTime));
            if (!string.IsNullOrWhiteSpace(request.pid))
            {
                var pidActivityList = QiangGouManager.SelectQiangGouProductModelsByPid(request.pid);
                model = from activityModel in model
                        join pidActivity in pidActivityList on activityModel.ActivityID equals pidActivity.ActivityID
                        select activityModel;
            }
            ViewBag.Select = request;
            return View(model);
        }

        /// <summary>
        /// 判断是否为正常商品pid
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult JudgeProductPid(string pid)
        {
            if (!string.IsNullOrWhiteSpace(pid))
            {
                using (var client = new ProductInfoQueryClient())
                {
                    var pidList = new List<string>();
                    pidList.Add(pid);
                    var listResult = client.SelectProductBaseInfo(pidList);
                    if (listResult.Success && listResult.Result != null)
                    {
                        var result = listResult.Result;
                        if (result.Count == 0)
                        {
                            return Json(new { Status = -1, Message = "请输入正确的商品PID！" });
                        }
                        else
                        {
                            return Json(new { Status = 0, Message = "这是正确的商品PID！" });
                        }
                    }
                }
            }
            return Json(new { Status = 0, Message = "ok" });
        }
        /// <summary>
        /// 修改或创建活动
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult Detail(string aid)
        {
            QiangGouModel model = null;
            Guid activityId;
            if (Guid.TryParse(aid, out activityId))
                model = QiangGouManager.FetchQiangGouAndProducts(activityId);
            ViewBag.ActivityId = aid;
            return View(model);
        }
        /// <summary>
        /// 保存活动(修改或创建)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(QiangGouModel model, string Products, string QiangGouDiff)
        {
            try
            {
                if (model == null)
                    return Json(new { Status = 0, Message = "保存失败【活动内容为空】" });
                var cacheGetFlag = GetOperateFlagCache();
                if (cacheGetFlag)
                    return Json(new { Status = 0, Message = "现在有人正在操作请稍等下。。。。" });
                var cacheSetStartFlag = SetOperateFlagStartCache();
                if (!cacheSetStartFlag)
                    return Json(new { Status = 0, Message = "服务器异常，请再点击保存" });

                var isUpdate = model.ActivityID != null;

                var origin = new QiangGouModel(); //更新逻辑时赋值当前活动ID下信息
                if (isUpdate)
                    origin = QiangGouManager.FetchQiangGouAndProducts(model.ActivityID.Value);

                //转换产品数据对象
                model.Products = JsonConvert.DeserializeObject<List<QiangGouProductModel>>(Products);
                List<QiangGouDiffModel> qiangGouDiff = new List<QiangGouDiffModel>();
                if (QiangGouDiff != null)
                    qiangGouDiff = JsonConvert.DeserializeObject<List<QiangGouDiffModel>>(QiangGouDiff);

                if (!model.Products.Any())
                    return Json(new { Status = 0, Message = "保存失败【活动无产品】" });

                var result = QiangGouManager.Save(model, qiangGouDiff);
                if (result.Item1 == -1)
                    return Json(new { Status = 0, Message = "保存失败【闪购活动时间不允许重叠】" });
                if (result.Item1 == -3)
                    return Json(new { Status = 0, Message = "刷新缓存失败【请手动刷新】" });
                else if (result.Item1 > 0)
                {
                    QiangGouModel after;
                    if (model.ActivityID != null && !model.NeedExam)
                    {
                        after = QiangGouManager.FetchQiangGouAndProducts(model.ActivityID.Value);
                    }
                    else
                    {
                        after = QiangGouManager.FetchQiangGouAndProductsTemp(model.ActivityID.Value);
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
                    if (isUpdate)
                    {
                        var chandata = LogChangeDataManager.GetLogChangeData(origin, after);
                        var beforeValue = QiangGouManager.GenerateSimpleQiangGouModel(chandata.Item1);
                        var afterValue = QiangGouManager.GenerateSimpleQiangGouModel(chandata.Item2);
                        var oprLog = new FlashSaleProductOprLog
                        {
                            OperateUser = ThreadIdentity.Operator.Name,
                            CreateDateTime = DateTime.Now,
                            BeforeValue = JsonConvert.SerializeObject(beforeValue),
                            AfterValue = JsonConvert.SerializeObject(afterValue),
                            LogType = "FlashSaleLog",
                            LogId = result.Item2.ToString(),
                            Operation = model.NeedExam ? "修改活动到待审核" : "修改活动"
                        };
                        LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                        LoggerManager.InsertFlashSaleLog(chandata.Item1, beforeValue.HashKey);
                        LoggerManager.InsertFlashSaleLog(chandata.Item2, afterValue.HashKey);
                    }
                    else
                    {
                        var afterValue = QiangGouManager.GenerateSimpleQiangGouModel(after);
                        var oprLog = new FlashSaleProductOprLog
                        {
                            OperateUser = ThreadIdentity.Operator.Name,
                            CreateDateTime = DateTime.Now,
                            BeforeValue = JsonConvert.SerializeObject(Json(new { actvityid = result.Item2.ToString() })),
                            AfterValue = JsonConvert.SerializeObject(afterValue),
                            LogType = "FlashSaleLog",
                            LogId = result.Item2.ToString(),
                            Operation = model.NeedExam ? "新建活动到待审核" : "新建活动"
                        };
                        LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                        LoggerManager.InsertFlashSaleLog(after, afterValue.HashKey);
                    }
                    if (model.ActivityID != null && !model.NeedExam)
                    {
                        var cache = UpdateToCache(model.ActivityID.Value, false, model.ActiveType);
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
            finally
            {
                SetOperateFlagEndCache();
            }
        }

        /// <summary>
        /// 校验配置同一个商品,在一个活动类型中不允许重复配置
        /// </summary>
        /// <param name="ActivityID"></param>
        /// <param name="ActivityType"></param>
        /// <param name="Products"></param>
        /// <returns></returns>
        public ActionResult SelectDiffActivityProducts(Guid? ActivityID, int ActivityType, string Products, DateTime StartDateTime, DateTime EndDateTime)
        {
            var products = JsonConvert.DeserializeObject<List<QiangGouProductModel>>(Products);
            if (!products.Any())
                return Json(new { Status = 0, Message = "保存失败【活动无产品】" });
            var diff = QiangGouManager.SelectDiffActivityProducts(ActivityID, ActivityType, products, StartDateTime, EndDateTime);
            if (diff.Any())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"<div>");
                sb.AppendLine(@"<input type='button' onclick='QiangGou.SetAllChecked()' style='height: 37px; line - height: 25px; padding: 5px 10px; background - color: #b0232a;color:#fff;border:1px #000 solid;display:inline-block;text-decoration: none;font-size: 20px;font-weight:900;' value=全选 />");
                foreach (var item in diff)
                {
                    sb.Append(@"<div>");
                    if (ActivityType == 3)
                    {
                        sb.AppendLine(item.Key + " 已经被保存在同时段其他活动页秒杀场次中，请删除后再次保存");
                    }
                    else
                    {
                        sb.AppendLine(item.Key + " 在以下活动存在配置情况不一致的情况：");
                    }

                    sb.Append(@"<table id='difftable'> <tr>
                                        <td>勾选需要同步的数据</td>
                                        <td>活动ID</td>
                                        <td>活动名称</td>
                                        <td>产品PID</td>
                                        <td>产品名称</td>
                                        <td>促销价</td>
                                        <td>伪原价</td>
                                        <td>安装/付款方式</td>
                                        <td>优惠券</td>

                                    </tr>");
                    foreach (var p in item.Value)
                    {
                        sb.Append("<tr><td>");
                        sb.Append("<input type='checkbox' id='diffchecked'/>");
                        sb.Append("</td><td>");
                        sb.Append(p.ActivityID);
                        sb.Append("</td><td>");
                        sb.Append(p.ActivityName);
                        sb.Append("</td><td>");
                        sb.Append(p.PID);
                        sb.Append("</td><td>");
                        sb.Append(p.ProductName);
                        sb.Append("</td><td>");
                        sb.Append(p.Price.ToString("0.00"));
                        sb.Append("</td><td>");
                        sb.Append(p.FalseOriginalPrice == null ? "" : p.FalseOriginalPrice.Value.ToString("0.00"));
                        sb.Append("</td><td>");
                        sb.Append(string.IsNullOrWhiteSpace(p.InstallAndPay) ? "不限" : (p.InstallAndPay == "PayOnline" ? "在线支付" : (p.InstallAndPay == "InstallAtShop" ? "到店安装" : "在线支付且到店安装")));
                        sb.Append("</td><td>");
                        sb.Append(p.IsUsePCode ? "使用" : "不使用");
                        sb.Append("</td><tr>");
                    }
                    sb.Append("</table></div>");
                    sb.Append("</div>");
                }
                return Json(new { Status = 1, Html = sb.ToString() });
            }
            return Json(new { Status = 0 });
        }

        /// <summary>
        /// 校验配置的产品是否在其他活动中存在不同价格
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public ActionResult CheckPIDSamePrice(string p)
        {
            var dic = new Dictionary<string, string>();
            List<QiangGouProductModel> model = JsonConvert.DeserializeObject<List<QiangGouProductModel>>(p);
            if (model == null || model.Count() == 0)
            {
                dic.Add("", "无产品,无需验证");
                return Json(dic.Select(c => new { PID = c.Key, Content = c.Value }));
            }
            foreach (var item in model)
            {
                var M = QiangGouManager.CheckPIDSamePriceInOtherActivity(item);
                if (M != null && M.Count() > 0)
                {
                    var content = "";
                    foreach (var t in M)
                    {
                        content += "活动ID：" + t.ActivityID + "&nbsp;&nbsp;&nbsp;&nbsp;价格：" + t.Price.ToString("C") + "<br/>";
                    }
                    dic.Add(M.FirstOrDefault()?.PID, content);
                }
            }
            if (dic.Count() == 0)
                dic.Add("", "与其他活动相同产品价格不冲突");
            return Json(dic.Select(c => new { PID = c.Key, Content = c.Value }));
        }

        public ActionResult ExamIndex()
        {
            return View(QiangGouManager.SelectAllNeedExamQiangGou());
        }
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult ExamDetail(string aid)
        {
            QiangGouModel model = null;
            Guid activityID;
            if (Guid.TryParse(aid, out activityID))
                model = QiangGouManager.FetchNeedExamQiangGouAndProducts(activityID);
            return View(model);
        }
        [HttpPost]
        public ActionResult ExamUpdatePrice(QiangGouProductModel model)
        {
            var result = QiangGouManager.ExamUpdatePrice(model);
            if (result > 0)
            {
                var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog();
                oprLog.Author = ThreadIdentity.Operator.Name;
                oprLog.ChangeDatetime = DateTime.Now;
                oprLog.AfterValue = model.ActivityID.ToString();
                oprLog.ObjectType = "FlashSale";
                oprLog.Operation = "审核页面改" + model.PID + "价格：改为" + model.Price;
                new OprLogManager().AddOprLog(oprLog);
            }
            return Json(result);
        }

        public ActionResult ExamActivity(Guid aid)
        {
            try
            {
                var cacheGetFlag = GetOperateFlagCache();
                if (cacheGetFlag)
                    return Json(new { Status = 0, Message = "现在有人正在操作请稍等下。。。。" });
                var cacheSetStartFlag = SetOperateFlagStartCache();
                if (!cacheSetStartFlag)
                    return Json(new { Status = 0, Message = "服务器异常，请再点击审核" });
                var origin = QiangGouManager.FetchQiangGouAndProducts(aid) ?? new QiangGouModel();
                var origin_temp = QiangGouManager.FetchQiangGouAndProductsTemp(aid) ?? new QiangGouModel();   //需要审核的数据在_temp表

                //获取删除前的活动状态
                var statusModel = new ActivityApprovalStatus();
                try
                {
                    statusModel = SeckillManager.SelectActivityStatusModel(aid);
                }
                catch (Exception ex)
                {
                    Logger.Log(Level.Error, $"ExamActivity => SelectActivityStatusModel 异常,", ex);
                }

                var result = QiangGouManager.ExamActivity(aid);

                //老的项目后台中审核时需要删除待审核的数据，否则和新的天天秒杀后台数据冲突，导致新的天天秒杀后台显示出错
                #region 删除数据
                if (result > 0 && origin_temp.ActiveType == 1)
                {
                    var del = SeckillManager.DeleteStatusData(aid.ToString());
                    if (del > 0)
                    {
                        result = 1;
                    }
                }
                #endregion

                if (result > 0)
                {
                    var after = QiangGouManager.FetchQiangGouAndProducts(aid);
                    var chandata = LogChangeDataManager.GetLogChangeData(origin, after);
                    var beforeValue = QiangGouManager.GenerateSimpleQiangGouModel(chandata.Item1);
                    var afterValue = QiangGouManager.GenerateSimpleQiangGouModel(chandata.Item2);
                    var oprLog = new FlashSaleProductOprLog();
                    oprLog.OperateUser = ThreadIdentity.Operator.Name;
                    oprLog.CreateDateTime = DateTime.Now;
                    oprLog.BeforeValue = JsonConvert.SerializeObject(beforeValue);
                    oprLog.AfterValue = JsonConvert.SerializeObject(afterValue);
                    oprLog.LogType = "FlashSaleLog";
                    oprLog.LogId = after.ActivityID.ToString();
                    oprLog.Operation = "审核活动";
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    LoggerManager.InsertFlashSaleLog(chandata.Item1, beforeValue.HashKey);
                    LoggerManager.InsertFlashSaleLog(chandata.Item2, afterValue.HashKey);
                    UpdateToCache(aid, false, origin.ActiveType);

                    //审核后备份删除的数据
                    InsertDeleteInfosAfterExamActivity(aid, origin, origin_temp, statusModel);

                    if (origin.ActiveType == 4 || origin.ActiveType == 0)
                    {
                        //限时抢购刷新活动页Es商品数据
                        bool rebuildEsResult = RebuildFalseSaleProductEsAfterPostSave(after, origin, out string rebuildEsMsg);
                        if (!rebuildEsResult)
                        {
                            return Json(new { code = -4, msg = rebuildEsMsg });
                        }
                    }
                }
                return Json(new { code = result });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    code = -3,
                    msg = e.Message + e.InnerException + e.StackTrace
                });
            }
            finally
            {
                SetOperateFlagEndCache();
            }
        }

        /// <summary>
        /// 审核后备份删除的数据: 备份活动商品被删除的项、 审核临时表该活动的全部数据
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="origin_Temp"></param>
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
                }

                //记录到活动状态备份表
                if (statusModel != null)
                {
                    var insertStatusModel = SeckillManager.InsertActivityApprovalStatus(statusModel);
                    if (insertStatusModel <= 0)
                    {
                        Logger.Log(Level.Warning, $"InsertDeleteInfosAfterExamActivity => " +
                            $"InsertActivityApprovalStatus,备份活动商品数据失败，" +
                            $"{JsonConvert.SerializeObject(insertStatusModel)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"InsertDeleteInfosAfterExamActivity 备份数据异常,origin:{JsonConvert.SerializeObject(origin)},origin_temp:{JsonConvert.SerializeObject(origin_Temp)}", ex);
            }
        }

        public bool UpdateToCache(Guid aid, bool needLog = true, int activityType = 0) => QiangGouManager.ReflashQiangGouCache(aid, needLog, activityType);

        public ActionResult SelectQGCache(Guid aid, bool needLog)
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.GetFlashSaleList(new Guid[] { aid });
                result.ThrowIfException(true);
                if (needLog)
                {
                    var oprLog = new FlashSaleProductOprLog();
                    oprLog.OperateUser = ThreadIdentity.Operator.Name;
                    oprLog.CreateDateTime = DateTime.Now;
                    oprLog.BeforeValue = JsonConvert.SerializeObject(Json(new { actvityid = aid }));
                    oprLog.AfterValue = JsonConvert.SerializeObject(Json(new { actvityid = aid }));
                    oprLog.LogType = "FlashSaleLog";
                    oprLog.LogId = aid.ToString();
                    oprLog.Operation = "刷新缓存";
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                }
                return Json(result.Result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FetchProductByPID(string pid, string activityId)
        {
            var product = QiangGouManager.FetchProductByPID(pid);
            if (product != null)
            {
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
            }

            return Json(product);
        }
        public ActionResult SelectPidsByParent(string productId)
        {
            var pids = QiangGouManager.SelectPidsByParent(productId);
            return Json(pids);
        }

        /// <summary>
        /// 批量导入活动商品数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportProducts(string activityId)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.FileName.Contains(".xlsx") || file.FileName.Contains(".xls"))
                {
                    //  var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = ExcelToDataTable("Sheet1", true, file.InputStream, file.FileName, out string msg);
                    IEnumerable<QiangGouImportModel> data = GetDataFromExcel(dt);

                    if (data.Any())
                        return CheckExcelData(data, activityId);
                    else
                        return Content(JsonConvert.SerializeObject(new { code = -1, msg = msg }));
                }
            }
            return Content(JsonConvert.SerializeObject(new { code = 0 }));
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        private DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn, Stream fs, string fileName, out string msg)
        {
            msg = string.Empty;
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            int excelRowNum = 1;
            try
            {
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    Func<ICell, string> GetFormULAValue = cell =>
                    {
                        object returnValue = new object();
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue;
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = cell.NumericCellValue.ToString();
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                        return returnValue.ToString();
                    };

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    bool isAllEmpty;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        excelRowNum++;
                        IRow row = sheet.GetRow(i);
                        if (row == null || string.IsNullOrWhiteSpace(row.GetCell(0)?.StringCellValue))
                            continue;
                        DataRow dataRow = data.NewRow();
                        isAllEmpty = true;
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            ICell cell = row.GetCell(j);
                            if (cell != null && cell.CellType != CellType.Blank) //同理，没有数据的单元格都默认是null
                            {
                                object value = new object();
                                switch (cell.CellType)
                                {
                                    case CellType.Numeric:
                                        // Date comes here
                                        if (HSSFDateUtil.IsCellDateFormatted(cell))
                                        {
                                            value = cell.DateCellValue;
                                        }
                                        else
                                        {
                                            // Numeric type
                                            value = cell.NumericCellValue;
                                        }
                                        break;
                                    case CellType.Boolean:
                                        // Boolean type
                                        value = cell.BooleanCellValue;
                                        break;
                                    case CellType.Formula:
                                        value = GetFormULAValue(cell);
                                        break;
                                    //value = cell.CellFormula;
                                    //break;

                                    default:
                                        // String type
                                        value = cell.StringCellValue;
                                        break;
                                }
                                if (!string.IsNullOrEmpty(value?.ToString()))
                                    isAllEmpty = false;

                                dataRow[j] = value.ToString();
                            }
                        }
                        if (!isAllEmpty)
                            data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                msg = $"第{excelRowNum}行数据异常,{ex.Message}";
                return null;
            }
        }

        private ActionResult CheckExcelData(IEnumerable<QiangGouImportModel> data, string aid)
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
                if (!string.IsNullOrWhiteSpace(model.促销价) && !decimal.TryParse(model.促销价, out activityPrice))
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
                if (!string.IsNullOrWhiteSpace(model.总限购) && !int.TryParse(model.总限购, out total))
                    sb.Append("总限购不合法_");
                //int total = 0;
                //var saleoutQty = 0;
                //if (!string.IsNullOrEmpty(model.PID))
                // saleoutQty = QiangGouManager.SelectFlashSaleSaleOutQuantity(aid, model.PID)??0;
                //if (!string.IsNullOrWhiteSpace(model.总限购) && !int.TryParse(model.总限购, out total))
                //    sb.Append("总限购不合法_");
                //else if(!string.IsNullOrWhiteSpace(model.总限购)&&saleoutQty > Convert.ToInt32(model.总限购))
                //{
                //    sb.Append("总限购数量小于已经售出数量请调整_");
                //}
                #endregion

                #region 安装和支付方式
                if (string.IsNullOrWhiteSpace(model.安装和支付方式))
                    model.安装和支付方式 = "不限";
                if (!installPay.ContainsKey(model.安装和支付方式))
                    sb.Append("安装和支付方式不合法_");
                #endregion

                #region 优惠券
                if (string.IsNullOrWhiteSpace(model.优惠券))
                    model.优惠券 = "使用";
                if (!isUseCode.ContainsKey(model.优惠券))
                    sb.Append("优惠券不合法_");
                #endregion

                #region 渠道
                if (string.IsNullOrWhiteSpace(model.渠道))
                    model.渠道 = "全部";
                if (!channels.ContainsKey(model.渠道))
                    sb.Append("渠道不合法_");
                #endregion

                #region 是否加入会长限购
                if (string.IsNullOrWhiteSpace(model.是否加入会场限购))
                    model.是否加入会场限购 = "否";
                if (!isyes.ContainsKey(model.是否加入会场限购))
                    sb.Append("是否加入会场限购不合法_");
                #endregion

                #region 标签
                if (string.IsNullOrWhiteSpace(model.标签))
                    model.标签 = "无";
                if (!tags.Contains(model.标签))
                    sb.Append("标签不合法_");
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
                        ProductName = model.名称,
                        Price = activityPrice,
                        FalseOriginalPrice = falsePrice,
                        MaxQuantity = max,
                        TotalQuantity = total,
                        InstallAndPay = installPay[model.安装和支付方式],
                        IsUsePCode = isUseCode[model.优惠券],
                        Channel = channels[model.渠道],
                        IsJoinPlace = isyes[model.是否加入会场限购],
                        Label = model.标签,
                        IsShow = isyes[model.显示]
                    };
                    var assembalModel = QiangGouManager.AssemblyProductInstalService(productModel);
                    products.Add(assembalModel);
                }
            }
            return Content(JsonConvert.SerializeObject(new { data = products, error = errorMessage, code = 1 }));
        }

        private IEnumerable<QiangGouImportModel> GetDataFromExcel(DataTable dt)
        {
            var data = dt.ConvertTo<QiangGouImportModel>();
            data = data.Where(_ => !(string.IsNullOrWhiteSpace(_.PID)
              && string.IsNullOrWhiteSpace(_.优惠券)
              && string.IsNullOrWhiteSpace(_.伪原价)
              && string.IsNullOrWhiteSpace(_.促销价)
              && string.IsNullOrWhiteSpace(_.名称)
              && string.IsNullOrWhiteSpace(_.安装和支付方式)
              && string.IsNullOrWhiteSpace(_.总限购)
              && string.IsNullOrWhiteSpace(_.排序)
              && string.IsNullOrWhiteSpace(_.是否加入会场限购)
              && string.IsNullOrWhiteSpace(_.显示)
              && string.IsNullOrWhiteSpace(_.标签)
              && string.IsNullOrWhiteSpace(_.每人限购)
              && string.IsNullOrWhiteSpace(_.渠道))
             );
            if (!data.Any())
                return new QiangGouImportModel[0];
            data = data.Select(_ =>
            {
                return new QiangGouImportModel()
                {
                    PID = getEmptyOrTrim(_.PID),
                    优惠券 = getEmptyOrTrim(_.优惠券),
                    伪原价 = getEmptyOrTrim(_.伪原价),
                    促销价 = getEmptyOrTrim(_.促销价),
                    名称 = getEmptyOrTrim(_.名称),
                    安装和支付方式 = getEmptyOrTrim(_.安装和支付方式),
                    总限购 = getEmptyOrTrim(_.总限购),
                    排序 = getEmptyOrTrim(_.排序),
                    是否加入会场限购 = getEmptyOrTrim(_.是否加入会场限购),
                    显示 = getEmptyOrTrim(_.显示),
                    标签 = getEmptyOrTrim(_.标签),
                    每人限购 = getEmptyOrTrim(_.每人限购),
                    渠道 = getEmptyOrTrim(_.渠道)
                };
            });
            return data;
        }

        private string getEmptyOrTrim(string str)
               => str == null ? string.Empty : str.Trim();

        public ActionResult RefreshCache(Guid aid)
        {
            var cache = UpdateToCache(aid);
            if (cache == false)
                return Json(new { Status = 0, Message = "刷新缓存失败【请重试】" });
            else
            {
                return Json(new { Status = 1, Message = "刷新缓存成功" });
            }
        }

        public ActionResult FlashSaleProductDetail(string activityId)
        {
            var aid = Guid.Empty;
            if (Guid.TryParse(activityId, out aid))
            {
                var model = QiangGouManager.FetchQiangGouAndProducts(aid);
                return View(model);
            }
            else
            {
                return View(new QiangGouModel());
            }
        }

        /// <summary>
        /// 根据活动id删除活动
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public ActionResult DelActivity(Guid aid)
        {
            if (aid != Guid.Empty)
            {
                var origin = new QiangGouModel();
                var origin_Temp = new QiangGouModel();
                try
                {
                    origin = QiangGouManager.FetchQiangGouAndProducts(aid);
                    origin_Temp = QiangGouManager.FetchQiangGouAndProductsTemp(aid);
                }
                catch (Exception ex)
                {
                    Logger.Log(Level.Error, $"DelActivity,ex:{ex}");
                }

                var result = DALQiangGou.DelFlashSale(aid);
                if (result > 0)
                {
                    if (origin?.ActiveType == 4)
                    {
                        //同步限时抢购商品信息到活动页(es)
                        RebuildActivityProductEs(origin?.Products?.Select(p => p.PID)?.ToList());
                    }

                    //刷新服务缓存，记录日志
                    UpdateToCache(aid, true, (int)origin?.ActiveType);

                    var oprLog = new FlashSaleProductOprLog();
                    oprLog.OperateUser = ThreadIdentity.Operator.Name;
                    oprLog.BeforeValue = aid.ToString();
                    oprLog.CreateDateTime = DateTime.Now;
                    oprLog.AfterValue = aid.ToString();
                    oprLog.LogType = "FlashSaleLog";
                    oprLog.LogId = aid.ToString();
                    oprLog.Operation = "删除活动";
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);

                    //备份删除的活动数据 - 伪逻辑删除
                    InsertDeletedAfterDeleteActivity(origin, origin_Temp);

                    return Json(new { Status = 1 });
                }
            }
            return Json(new { Status = 0 });
        }

        /// <summary>
        /// 删除后备份删除的数据 
        /// </summary>
        /// <param name="origin">活动数据</param>
        /// <param name="origin_Temp">审核临时表的</param>
        private void InsertDeletedAfterDeleteActivity(QiangGouModel origin, QiangGouModel origin_Temp)
        {
            try
            {
                var insertDeleteActivity = QiangGouManager.InsertDeletedActivityInfo(origin);
                var insertDeleteProducts = QiangGouManager.InsertDeletedActivityProducts(origin?.Products?.ToList());

                if (insertDeleteActivity > 0)
                {
                    Logger.Log(Level.Warning, $"InsertDeletedAfterDeleteActivity,备份被删除的数据失败," +
                        $"{JsonConvert.SerializeObject(origin)}");
                }

                if (origin_Temp != null && origin.ActivityID != Guid.Empty)
                {
                    //记录到活动审核备份表
                    var insertDeleteActivity_Temp = QiangGouManager.InsertDeletedActivityInfo_Temp(origin_Temp);
                    if (insertDeleteActivity_Temp <= 0)
                    {
                        Logger.Log(Level.Warning, $"InsertDeletedAfterDeleteActivity,备份审核活动数据失败，" +
                            $"{JsonConvert.SerializeObject(origin_Temp)}");
                    }

                    if (origin_Temp?.Products?.Count() > 0)
                    {
                        //记录到活动商品审核备份表
                        var insertDeleteActivityProduct_Temp = QiangGouManager.InsertDeletedActivityProducts_Temp(origin_Temp?.Products?.ToList());
                        if (insertDeleteActivityProduct_Temp <= 0)
                        {
                            Logger.Log(Level.Warning, $"InsertDeletedAfterDeleteActivity,备份审核活动商品数据失败，" +
                                $"{JsonConvert.SerializeObject(origin_Temp)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"InsertDeletedAfterDeleteActivity,备份被删除的数据异常", ex);
            }
        }


        public ActionResult ListBoostrap(string logId)
        {
            var list = LoggerManager.SelectFlashSaleHistoryByLogId(logId).ToList();
            return View(list);
        }

        public ActionResult ViewConfigLog(int id)
        {
            var result = LoggerManager.GetFlashSaleHistoryByPkid(id) ?? new FlashSaleProductOprLog();
            var beforevalue = JsonConvert.DeserializeObject<QiangGouModel>(result.BeforeValue);
            var aftervalue = JsonConvert.DeserializeObject<QiangGouModel>(result.AfterValue);
            beforevalue.Products = LoggerManager.SelectFlashSaleProductsLog(beforevalue.HashKey);
            aftervalue.Products = LoggerManager.SelectFlashSaleProductsLog(aftervalue.HashKey);
            result.BeforeValue = JsonConvert.SerializeObject(beforevalue);
            result.AfterValue = JsonConvert.SerializeObject(aftervalue);
            return View(result);
        }

        private bool SetOperateFlagStartCache()
        {
            using (var client = CacheHelper.CreateCacheClient("Qinanggou"))
            {
                var result = client.Set("OperateFlag", true, TimeSpan.FromMinutes(1));
                return result.Success;
            }
        }
        private bool SetOperateFlagEndCache()
        {
            using (var client = CacheHelper.CreateCacheClient("Qinanggou"))
            {
                var result = client.Set("OperateFlag", false);
                return result.Success;
            }
        }
        private bool GetOperateFlagCache()
        {
            using (var client = CacheHelper.CreateCacheClient("Qinanggou"))
            {
                var result = client.Get<bool>("OperateFlag");
                return result.Success && result.Value;
            }
        }

        /// <summary>
        /// 保存活动(修改或创建)
        /// </summary>
        /// <param name="model">活动数据</param>
        /// <param name="products">产品数据</param>
        /// <param name="qiangGouDiff">同步数据</param>
        /// <returns></returns>
        public ActionResult PostSave(QiangGouModel model, string products, string qiangGouDiff)
        {
            try
            {
                if (model == null)
                    return Json(new { Status = 0, Message = "保存失败【活动内容为空】" });

                var cacheGetFlag = GetOperateFlagCache();
                if (cacheGetFlag)
                    return Json(new { Status = 0, Message = "现在有人正在操作请稍等下。。。。" });

                var cacheSetStartFlag = SetOperateFlagStartCache();
                if (!cacheSetStartFlag)
                    return Json(new { Status = 0, Message = "服务器异常，请再点击保存" });

                // 是否更新标识
                var isUpdate = model.ActivityID != null;

                var origin = new QiangGouModel();
                var origin_Temp = new QiangGouModel();

                // 更新 查询活动详细数据
                if (isUpdate)
                {
                    origin = QiangGouManager.FetchQiangGouAndProducts(model.ActivityID.Value);
                    try
                    {
                        origin_Temp = QiangGouManager.FetchQiangGouAndProductsTemp(model.ActivityID.Value);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, $"PostSave=>FetchQiangGouAndProducts_Temp异常", ex);
                    }
                }

                // 序列化产品数据
                model.Products = JsonConvert.DeserializeObject<List<QiangGouProductModel>>(products);

                // 产品不存在 退出
                if (!model.Products.Any())
                    return Json(new { Status = 0, Message = "保存失败【活动无产品】" });

                // 存在差异产品 并序列化成list
                var diffQiangGouList = new List<QiangGouDiffModel>();
                if (qiangGouDiff != null)
                    diffQiangGouList = JsonConvert.DeserializeObject<List<QiangGouDiffModel>>(qiangGouDiff);

                var result = QiangGouManager.SyncSave(model, diffQiangGouList);

                switch (result.Item1)
                {
                    case -1:
                        return Json(new { Status = 0, Message = "保存失败【闪购活动时间不允许重叠】" });
                    case -3:
                        return Json(new { Status = 0, Message = "刷新缓存失败【请手动刷新】" });
                    default:
                        if (result.Item1 > 0)
                        {
                            QiangGouModel after;
                            if (model.ActivityID != null && !model.NeedExam)
                            {
                                after = QiangGouManager.FetchQiangGouAndProducts(model.ActivityID.Value);
                            }
                            else
                            {
                                after = QiangGouManager.FetchQiangGouAndProductsTemp(model.ActivityID.Value);
                            }
                            var request = new ActivityTypeRequest
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
                            if (isUpdate)
                            {
                                var chandata = LogChangeDataManager.GetLogChangeData(origin, after);
                                var beforeValue = QiangGouManager.GenerateSimpleQiangGouModel(chandata.Item1);
                                var afterValue = QiangGouManager.GenerateSimpleQiangGouModel(chandata.Item2);
                                var oprLog = new FlashSaleProductOprLog
                                {
                                    OperateUser = ThreadIdentity.Operator.Name,
                                    CreateDateTime = DateTime.Now,
                                    BeforeValue = JsonConvert.SerializeObject(beforeValue),
                                    AfterValue = JsonConvert.SerializeObject(afterValue),
                                    LogType = "FlashSaleLog",
                                    LogId = result.Item2.ToString(),
                                    Operation = model.NeedExam ? "修改活动到待审核" : "修改活动"
                                };
                                LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                                LoggerManager.InsertFlashSaleLog(chandata.Item1, beforeValue.HashKey);
                                LoggerManager.InsertFlashSaleLog(chandata.Item2, afterValue.HashKey);

                                //备份 审核的Temp表数据
                                InsertDeletedAfterPostSave(origin_Temp, origin, model);
                            }
                            else
                            {
                                var afterValue = QiangGouManager.GenerateSimpleQiangGouModel(after);
                                var oprLog = new FlashSaleProductOprLog
                                {
                                    OperateUser = ThreadIdentity.Operator.Name,
                                    CreateDateTime = DateTime.Now,
                                    BeforeValue = JsonConvert.SerializeObject(Json(new { actvityid = result.Item2.ToString() })),
                                    AfterValue = JsonConvert.SerializeObject(afterValue),
                                    LogType = "FlashSaleLog",
                                    LogId = result.Item2.ToString(),
                                    Operation = model.NeedExam ? "新建活动到待审核" : "新建活动"
                                };
                                LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                                LoggerManager.InsertFlashSaleLog(after, afterValue.HashKey);
                            }
                            if (model.ActivityID != null && !model.NeedExam)
                            {
                                var cache = UpdateToCache(model.ActivityID.Value, false, model.ActiveType);
                                if (cache == false)
                                    return Json(new { Status = 0, Message = "刷新缓存失败【请手动刷新】" });
                            }

                            //限时抢购保存后 刷新商品信息到活动页
                            if ((model.ActiveType == 4 || model.ActiveType == 0) && !model.NeedExam)
                            {
                                var rebuildEsResult = RebuildFalseSaleProductEsAfterPostSave(model, origin, out string rebuildEsMsg);
                                if (!rebuildEsResult)
                                {
                                    return Json(new { Status = 1, Message = rebuildEsMsg });
                                }
                            }

                            return Json(new { Status = 1, Message = "保存成功" });
                        }
                        break;
                }

                return Json(new { Status = 0, Message = "保存失败【未知错误】" });
            }
            catch (Exception e)
            {
                return Json(new { Status = 0, Message = e.Message + e.InnerException + e.StackTrace });
            }
            finally
            {
                SetOperateFlagEndCache();
            }
        }

        /// <summary>
        /// 保存后 记录删除的数据
        /// </summary>
        /// <param name="origin_Temp"></param>
        /// <param name="origin"></param>
        /// <param name="model"></param>
        private void InsertDeletedAfterPostSave(QiangGouModel origin_Temp, QiangGouModel origin,QiangGouModel model)
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
                            Logger.Log(Level.Warning, $"InsertDeletedAfterPostSave=>InsertDeletedActivityProducts失败{JsonConvert.SerializeObject(deleteProducts)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"InsertDeletedAfterPostSave异常{JsonConvert.SerializeObject(origin_Temp)}", ex);
            }
        }

        /// <summary>
        /// 限时抢购保存后刷新活动页ES
        /// </summary>
        /// <param name="newModel">保存后限时抢购活动信息</param>
        /// <param name="oldModel">保存前限时抢购活动信息</param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool RebuildFalseSaleProductEsAfterPostSave(QiangGouModel newModel, QiangGouModel oldModel, out string message)
        {
            bool result = true;
            message = string.Empty;
            try
            {
                if (oldModel?.StartDateTime != newModel.StartDateTime || oldModel?.EndDateTime != newModel?.EndDateTime)
                {
                    //时间有变动就刷新修改前后所有pid,没有超过500个限制
                    var pids = new List<string>();
                    if (newModel.Products?.Count() > 0)
                    {
                        pids.AddRange(newModel.Products?.Select(a => a.PID));
                    }
                    if (oldModel.Products?.Count() > 0)
                    {
                        pids.AddRange(oldModel.Products?.Select(a => a.PID));
                    }
                    pids = pids.Distinct().ToList();

                    bool rebuildEsResult = RebuildActivityProductEs(pids);
                    if (!rebuildEsResult)//刷新失败
                    {
                        Logger.Log(Level.Warning, $"PostSave,[{newModel.ActivityID}],数据保存成功,但商品信息同步到活动页失败");
                        message = "数据保存成功,但商品信息同步到活动页失败";
                        result = false;
                    }
                }
                else
                {
                    //时间没有变动就刷新变动(新增、删除)的pid
                    var changePids = GetFlashSaleSaveChangePids(oldModel?.Products?.Select(a => a.PID)?.ToList(), newModel?.Products?.Select(a => a.PID)?.ToList());

                    //限时抢购刷新活动产品搜索信息
                    if (changePids?.Count > 500)//变动商品超过500个
                    {
                        Logger.Log(Level.Info, $"PostSave,[{newModel.ActivityID}],数据保存成功,但变动pid超过500个，未同步到活动页");
                        message = "数据保存成功,但变动商品超过500个,商品信息同步到活动页失败";
                        result = false;
                    }
                    else if (changePids?.Count > 0)//变动pid不超过500个
                    {
                        bool rebuildEsResult = RebuildActivityProductEs(changePids);
                        if (!rebuildEsResult)
                        {
                            Logger.Log(Level.Warning, $"PostSave,数据保存成功,但商品信息同步到活动页失败,ActivityID:[{newModel.ActivityID}],pids:{string.Join(",", changePids)}");
                            message = "数据保存成功,但商品信息同步到活动页失败 ";
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Log(Level.Error, $"RebuildFalseSaleProductEsAfterPostSave,限时抢购保存刷新活动产品搜索信息异常,ActivityID:[{newModel.ActivityID}]，ex:{ex}");
                message = "数据保存成功,但商品信息同步到活动页失败";
            }
            return result;
        }

        /// <summary>
        /// 刷新活动产品搜索信息
        /// </summary>
        /// <param name="oldList"></param>
        /// <param name="newList"></param>
        private List<string> GetFlashSaleSaveChangePids(List<string> oldList, List<string> newList)
        {
            var changeList = new List<string>();
            var delList = new List<string>();
            var addList = new List<string>();

            try
            {
                if (newList?.Count > 0 && oldList?.Count > 0)//修改前后都有pid
                {
                    addList = newList.Except(oldList)?.ToList();
                    delList = oldList.Except(newList)?.ToList();
                }
                else if (newList?.Count > 0 && !(oldList?.Count > 0))//修改前没有pid
                {
                    addList = newList;
                }
                else if (!(newList?.Count > 0) && oldList?.Count > 0)//修改后没有pid
                {
                    delList = oldList;
                }

                if (delList?.Count() > 0)
                {
                    changeList.AddRange(delList);
                }

                if (addList?.Count() > 0)
                {
                    changeList.AddRange(addList);
                }
                changeList = changeList.Distinct()?.ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"GetFlashSaleSaveChangePids异常，ex:{ex}");
            }
            return changeList;
        }

        /// <summary>
        /// 刷新活动产品搜索信息
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        private bool RebuildActivityProductEs(List<string> pids)
        {
            bool result = false;
            if (pids.Count > 0)
            {
                using (var client = new CacheClient())
                {
                    var clientResult = client.RebuildActivityProductEs(pids);
                    if (clientResult.Success)
                    {
                        result = true;
                    }
                    else
                    {
                        Logger.Log(Level.Warning, $"RebuildActivityProductEs刷新活动产品搜索信息失败,Pid:{string.Join(",", pids)}，ErrorMessage:{clientResult.ErrorMessage}");
                    }
                }
            }
            return result;
        }

    }
}

