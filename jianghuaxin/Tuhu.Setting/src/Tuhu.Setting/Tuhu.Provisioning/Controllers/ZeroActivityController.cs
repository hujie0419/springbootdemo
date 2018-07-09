using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.ZeroActivity;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Activity;

namespace Tuhu.Provisioning.Controllers
{
    public class ZeroActivityController : Controller
    {
        //0元购成功获奖的处理
        public ActionResult SucceedAward(string model, int OrderId)
        {
            var Zaa = JsonConvert.DeserializeObject<ZeroActivityApply>(model);
            var UserID = Zaa.UserID.ToString();
            var UserMobileNumber = Zaa.UserMobileNumber;
            var Period = Zaa.Period;
            return Json(ZeroActivityManager.ZeroAward(Period, UserID, UserMobileNumber, OrderId));
        }

        //0元购   筛选功能
        public ActionResult ZeroActivity(ZeroActivityApply filtermodel, string type, int CurrentPage = 1)
        {
            var model = new List<ZeroActivityApply>();
            int TotalCount = 0;
            if (type == "导出")
            {
                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet();
                var row = sheet.CreateRow(0);
                var cell = null as ICell;
                var cellNum = 0;

                row.CreateCell(cellNum++).SetCellValue("申请渠道");
                row.CreateCell(cellNum++).SetCellValue("期数");
                row.CreateCell(cellNum++).SetCellValue("用户ID");
                row.CreateCell(cellNum++).SetCellValue("用户名");
                row.CreateCell(cellNum++).SetCellValue("用户电话号码");
                row.CreateCell(cellNum++).SetCellValue("用户订单数");
                row.CreateCell(cellNum++).SetCellValue("产品名称");
                row.CreateCell(cellNum++).SetCellValue("产品PID");
                row.CreateCell(cellNum++).SetCellValue("产品尺寸");
                row.CreateCell(cellNum++).SetCellValue("申请的产品数量");
                row.CreateCell(cellNum++).SetCellValue("所在省");
                row.CreateCell(cellNum++).SetCellValue("所在地区");
                row.CreateCell(cellNum++).SetCellValue("获得加油数");
                row.CreateCell(cellNum++).SetCellValue("申请理由");
                row.CreateCell(cellNum++).SetCellValue("车型");
                row.CreateCell(cellNum++).SetCellValue("是否成功获奖");
                row.CreateCell(cellNum++).SetCellValue("获得轮胎后的订单号");
                row.CreateCell(cellNum++).SetCellValue("试用报告的状态");
                row.CreateCell(cellNum++).SetCellValue("申请时间");
                row.CreateCell(cellNum++).SetCellValue("更新时间");

                cellNum = 0;
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                if (filtermodel.Period > 0)
                {
                    model = ZeroActivityManager.ZeroConditionFilter(filtermodel, 1, 9999, out TotalCount);
                    if (model != null && model.Any())
                    {
                        for (var i = 0; i < model.Count; i++)
                        {
                            cellNum = 0;
                            NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(i + 1);
                            rowtemp.CreateCell(cellNum++).SetCellValue(ConvertChannel(model[i].ApplyChannel));
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].Period.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].UserID?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].UserName?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].UserMobileNumber?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].UserOrderQuantity?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].ProductName?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].PID?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].ProductSize?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].Quantity.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].ProvinceName?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].CityName?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].SupportNumber.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].ApplyReason?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].CarName?.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].Succeed?.ToString() == "1" ? "是" : "否");
                            rowtemp.CreateCell(cellNum++).SetCellValue(ConvertStatus(model[i].Succeed, model[i].Status, model[i].OrderId));
                            rowtemp.CreateCell(cellNum++).SetCellValue(ConvertReportStatus(model[i].ReportStatus));
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].ApplyDateTime.ToString());
                            rowtemp.CreateCell(cellNum++).SetCellValue(model[i].LastUpdateDateTime.ToString());
                        }
                    }
                }

                var ms = new MemoryStream();
                workbook.Write(ms);
                return File(ms.ToArray(), "application/x-xls", $"第{filtermodel?.Period}期众测申请信息.xlsx");
            }
            else
            {
                int PageSize = 500;
                ViewBag.Period = filtermodel.Period;
                ViewBag.UserOrderQuantity = filtermodel.UserOrderQuantity;
                ViewBag.Succeed = filtermodel.Succeed;
                ViewBag.ReportStatus = filtermodel.ReportStatus;
                ViewBag.UserMobileNumber = filtermodel.UserMobileNumber;
                ViewBag.TotalCount = 0;
                model = ZeroActivityManager.ZeroConditionFilter(filtermodel, CurrentPage, PageSize, out TotalCount);
                var totalpage = TotalCount * 1.0 / PageSize;
                if (totalpage - (int)totalpage == 0)
                {
                    ViewBag.TotalCount = (int)totalpage;
                }
                else
                {
                    ViewBag.TotalCount = (int)totalpage + 1;
                }
                ViewBag.CurrentPage = CurrentPage;
                return View(model);
            }
        }

        private string ConvertChannel(int? applyChannel)
        {
            var result = string.Empty;
            switch (applyChannel)
            {
                case 0:
                    result = "网站";
                    break;
                case 1:
                    result = "手机端";
                    break;
                case 3:
                    result = "微信";
                    break;
            }
            return result;
        }

        private string ConvertStatus(int? success, int status,int orderId)
        {
            var result = string.Empty;
            if (success == 1)
            {
                result = orderId.ToString();
            }
            else
            {
                switch (status)
                {
                    case -1:
                        result = "申请失败";
                        break;
                    case 0:
                        result = "申请中";
                        break;
                    case 1:
                        result = "获奖";
                        break;
                }
            }
            return result;
        }

        private string ConvertReportStatus(int? reportStatus)
        {
            var result = "还未填写报告";
            switch (reportStatus)
            {
                case 1:
                    result = "已填报告但未填写个人信息";
                    break;
                case 2:
                    result = "报告填写完整还未审核请中";
                    break;
                case 3:
                    result = "报告审核通过可返押金";
                    break;
            }
            return result;
        }

        //0元购活动配置
        public ActionResult ZAConfigure(int pageIndex = 1, int pageSize = 10)
        {
            var result = ZeroActivityManager.SelectZeroActivityDetail();
            var totalpage = result.Count * 1.0 / pageSize;
            if (totalpage - (int)totalpage == 0)
            {
                ViewBag.TotalCount = (int)totalpage;
            }
            else
            {
                ViewBag.TotalCount = (int)totalpage + 1;
            }
            ViewBag.CurrentPage = pageIndex;
            result = result.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return View(result);
        }

        public JsonResult UpsertZeroActivity(string productId, string variantId, string des, DateTime start, DateTime end, string imgUrl, int period = 0, int successQuota = 0, int quantity = 0)
        {
            var msg = string.Empty;
            if (start != null && end != null && start > end)
            {
                return Json(new { status = "fail", msg = "日期选择错误" }, JsonRequestBehavior.AllowGet);
            }
            var details = ZeroActivityManager.SelectZeroActivityDetail();
            if (start > details.Where(o => !o.Period.Equals(period)).OrderByDescending(_ => _.EndDateTime).Select(x => x.EndDateTime).FirstOrDefault())
            {
                return Json(new { status = "fail", msg = "活动时间有间断" }, JsonRequestBehavior.AllowGet);
            }

            ZeroActivityDetail data = new ZeroActivityDetail()
            {
                Period = period,
                ProductID = productId,
                VariantID = variantId,
                Description = des,
                StartDateTime = start,
                EndDateTime = end,
                SucceedQuota = successQuota,
                Quantity = quantity,
                ImgUrl = imgUrl
            };
            var result = ZeroActivityManager.ZAConfigureAct(data) > 0;
            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail", msg = "操作失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ZAConfigureDelete(int period)
        {
            return Json(ZeroActivityManager.ZAConfigureDelete(period));
        }

        public ActionResult UpdateStatusByPeriod(int period)
        {
            return Json(ZeroActivityManager.UpdateStatusByPeriod(period));
        }

        public JsonResult UploadTrialReport(Guid userId, int orderId, int pkid, string title,
            string content, string imgStr)
        {
            var result = ZeroActivityManager.CreateProductComment(userId, orderId, pkid, title, content, imgStr);
            return Json(new { result = result.Item1, msg = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshCache()
        {
            try
            {
            using (var client = new ZeroActivityClient())
            {
                var result = client.SelectUnfinishedZeroActivitiesForHomepage(true);
                result.ThrowIfException(true);
                return Json(new
                {
                    msg = "缓存刷新成功"
                });
            }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    msg=e.InnerException
                });
            }
        }
    }
}
