using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.RebateConfig;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity.RebateConfig;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.Provisioning.Controllers
{
    public class RebateConfigController : Controller
    {
        private static readonly string[] UserPower = new string[]
        {
            "zhanglei1@tuhu.cn",
            "devteam@tuhu.work",
            "zhangchongqi@tuhu.cn",
            "guquan@tuhu.cn",
            "fuyao@tuhu.cn",
            "zhuyan@tuhu.cn",
            "zhoumingming@tuhu.cn"
        };

        private static readonly string[] WxPayRetryUserPower = new string[]
        {
            "zhanglei1@tuhu.cn",
            "guquan@tuhu.cn",
            "devteam@tuhu.work"
        };

        [PowerManage]
        public ActionResult RebateConfig()
        {
            ViewBag.IsEnableDelete = 0;
            ViewBag.IsWxPayRetry = 0;
            ViewBag.IsUploadFile = 0;
            if (UserPower.Contains(User.Identity.Name))
                ViewBag.IsEnableDelete = 1;
            if (WxPayRetryUserPower.Contains(User.Identity.Name))
            {
                ViewBag.IsWxPayRetry = 1;
                ViewBag.IsUploadFile = 1;
            }
            return View();
        }

        public JsonResult SelectRebateConfig(Status status, string orderId, string phone,
            string wxId, string remarks, string wxName, string timeType, DateTime? startTime,
            DateTime? endTime, string principalPerson, string rebateMoney,string source ,
            int pageIndex = 1, int pageSize = 10)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var result = manager.SelectRebateConfig(status, orderId, phone,
                wxId, remarks, timeType, startTime, endTime, wxName, principalPerson, rebateMoney,
                source, pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectRebateConfigByPKID(int pkid)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var result = manager.SelectRebateConfigByPKID(pkid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        public JsonResult UpsertRebateApplyConfig(RebateConfigModel data)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var result = manager.InsertRebateConfig(data, User.Identity.Name);
            return Json(new { status = result.Item1, msg = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRemarks(int pkid, string remarks)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var result = manager.UpdateRemarks(pkid, remarks, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadImage(string type = "")
        {
            var result = false;
            var msg = string.Empty;
            var imageUrl = string.Empty;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string fileExtension = System.IO.Path.GetExtension(Request.Files[0].FileName);
                string[] allowExtension = { ".jpg", ".gif", ".png", ".jpeg" };
                if (allowExtension.Contains(fileExtension.ToLower()))
                {
                    var buffers = new byte[file.ContentLength];
                    file.InputStream.Read(buffers, 0, file.ContentLength);
                    var upLoadResult = buffers.UpdateLoadImage();
                    if (upLoadResult.Item1)
                    {
                        result = true;
                        imageUrl = upLoadResult.Item2;
                    }
                }
                else
                {
                    msg = "图片格式不对";
                }
            }
            else
            {
                msg = "请选择文件";
            }

            return Json(new { Status = result, ImageUrl = imageUrl, Msg = msg, Type = type });
        }

        [PowerManage]
        public JsonResult UpdateStatusForComplete(int pkid)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var result = manager.UpdateStatusForComplete(pkid, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        public JsonResult UpdateStatus(int pkid, Status status, int refusalReasonId = 0)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var refusalReason = string.Empty;
            switch (refusalReasonId)
            {
                case 3384:
                    refusalReason = "您好，您的朋友圈截图未满20个赞 ，请重新提交信息！";
                    break;
                case 3385:
                    refusalReason = "您的订单还是已发货状态，请联系门店或客服更改订单状态为“已安装”，更改后请重新提交信息！";
                    break;
                case 3386:
                    refusalReason = "您好，您上传的截图不符，请您重新提交信息，朋友圈截图需满足文字40个字，6张图，20个赞！";
                    break;
                case 3450:
                    refusalReason = "您好，您的朋友圈文字不足40个字，请在评论里加上40字文字说明且包含有“途虎养车购买安装”等字样，添加完成后重新截图并提交信息哦！";
                    break;
                case 3333:
                    refusalReason = "";
                    break;
                default:
                    break;

            }
            var result = manager.UpdateStatus(pkid, status, refusalReason, refusalReasonId, User.Identity.Name);
            return Json(new { status = result.Item1, msg = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteRebateApplyConfig(string pkidStr, bool isDelete = true)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var user = User.Identity.Name;
            var result = false;
            if (UserPower.Contains(User.Identity.Name))
                result = manager.DeleteRebateApplyConfig(pkidStr.Split(',').ToList(), isDelete, user);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchRebateConfigLog(string pkid)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var result = manager.SearchRebateConfigLog(pkid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushWechatInfo(int pushBatchId, string openId)
        {
            var result = PushService.PushWechatInfoByBatchId(pushBatchId, new PushTemplateLog() { Target = openId });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult UpdateRebateTimeToComplete()
        //{
        //    RebateConfigManager manager = new RebateConfigManager();
        //    if (UserPower.Contains(User.Identity.Name))
        //    {
        //        var result = manager.UpdateRebateTimeToComplete();
        //        return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public JsonResult UpdateWxName(string pkidStr)
        //{
        //    var result = false;
        //    var msg = string.Empty;
        //    RebateConfigManager manager = new RebateConfigManager();
        //    if (UserPower.Contains(User.Identity.Name))
        //    {
        //        var data = manager.UpdateWxName(pkidStr.Split(',').ToList());
        //        result = data.Item1;
        //        msg = data.Item2;
        //    }
        //    return Json(new { result = result, msg = msg }, JsonRequestBehavior.AllowGet);
        //}

        public FileResult ExportRebateConfigInfo(Status status, string orderId, string phone,
            string wxId, string remarks, string timeType, DateTime? startTime, string wxName,
            DateTime? endTime, string principalPerson, string rebateMoney,string source,
            int pageIndex = 1, int pageSize = 10)
        {
            var workBook = new XSSFWorkbook();
            var sheet = workBook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("返现编号");
            row.CreateCell(cellNum++).SetCellValue("订单号");
            //row.CreateCell(cellNum++).SetCellValue("手机号");
            //row.CreateCell(cellNum++).SetCellValue("微信号");
            row.CreateCell(cellNum++).SetCellValue("微信昵称");
            row.CreateCell(cellNum++).SetCellValue("状态");
            row.CreateCell(cellNum++).SetCellValue("百度ID");
            row.CreateCell(cellNum++).SetCellValue("百度吧名");
            row.CreateCell(cellNum++).SetCellValue("红包专员");
            row.CreateCell(cellNum++).SetCellValue("用户名称");
            row.CreateCell(cellNum++).SetCellValue("车牌号");
            row.CreateCell(cellNum++).SetCellValue("来源");
            row.CreateCell(cellNum++).SetCellValue("内容链接");
            row.CreateCell(cellNum++).SetCellValue("返现金额");
            row.CreateCell(cellNum++).SetCellValue("返现时间");
            row.CreateCell(cellNum++).SetCellValue("备注");
            row.CreateCell(cellNum++).SetCellValue("申请时间");
            row.CreateCell(cellNum++).SetCellValue("审核时间");
            row.CreateCell(cellNum++).SetCellValue("拒绝原因");
            row.CreateCell(cellNum++).SetCellValue("新老数据");

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
            RebateConfigManager manager = new RebateConfigManager();
            List<RebateConfigModel> result = new List<RebateConfigModel>();

            result = manager.SelectRebateConfig(status, orderId, phone, wxId, remarks, timeType,
                 startTime, endTime, wxName, principalPerson, rebateMoney,source, pageIndex, 9999999);
            if (result != null && result.Any())
            {
                for (var i = 0; i < result.Count(); i++)
                {
                    cellNum = 0;
                    NPOI.SS.UserModel.IRow rowTemp = sheet.CreateRow(i + 1);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].PKID);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].OrderId);
                    //rowTemp.CreateCell(cellNum++).SetCellValue(result[i].UserPhone);
                    //rowTemp.CreateCell(cellNum++).SetCellValue(result[i].WXId);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].WXName);
                    rowTemp.CreateCell(cellNum++).SetCellValue(ConvertStatus(result[i].Status));
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].BaiDuId);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].BaiDuName);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].PrincipalPerson);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].UserName);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].CarNumber);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].Source);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].ContentUrl);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].RebateMoney.ToString("f2"));
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].RebateTime?.ToString() ?? string.Empty);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].Remarks);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].CreateTime.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].CheckTime.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].RefusalReason?.ToString() ?? string.Empty);
                    rowTemp.CreateCell(cellNum++).SetCellValue(!string.IsNullOrEmpty(result[i].OpenId) ? "新" : "老");
                }
            }
            var ms = new MemoryStream();
            workBook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        private string ConvertStatus(Status status)
        {
            var result = "未审核";
            switch (status)
            {
                case Status.Applying:
                    result = "待审核";
                    break;
                case Status.Approved:
                    result = "已通过但未支付";
                    break;
                case Status.Unapprove:
                    result = "审核拒绝";
                    break;
                case Status.Complete:
                    result = "已支付";
                    break;
            };
            return result;
        }

        public JsonResult UploadFile()
        {
            var files = Request.Files;
            RebateConfigManager manager = new RebateConfigManager();
            if (files.Count <= 0)
            {
                return Json(new { status = true, msg = "请上传文件" }, JsonRequestBehavior.AllowGet);
            }
            var result = ConvertExcelToList(files);
            if (result.Item1 != null && result.Item1.Any())
            {
                var data = manager.UpsertOldRebateApply(result.Item1, User.Identity.Name);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new List<RebateConfigModel>(), JsonRequestBehavior.AllowGet);
            }
        }

        private Tuple<List<RebateConfigModel>, string> ConvertExcelToList(HttpFileCollectionBase files)
        {
            var result = new List<RebateConfigModel>();
            var msg = string.Empty;
            var stream = files[0].InputStream;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var workBook = new XSSFWorkbook(new MemoryStream(buffer));
            var sheet = workBook.GetSheetAt(0);
            #region
            Func<ICell, string> getStringValue = cell =>
            {
                if (cell != null)
                {
                    if (cell.CellType == CellType.Numeric)
                    {
                        return DateUtil.IsCellDateFormatted(cell) ?
                            cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                            cell.NumericCellValue.ToString();
                    }
                    return cell?.StringCellValue ?? string.Empty;
                }
                return null;
            };
            #endregion
            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;
            if (titleRow.GetCell(index++)?.StringCellValue == "途虎订单号" &&
                titleRow.GetCell(index++)?.StringCellValue == "联系方式" &&
                titleRow.GetCell(index++)?.StringCellValue == "微信号" &&
                titleRow.GetCell(index++)?.StringCellValue == "微信昵称" &&
                titleRow.GetCell(index++)?.StringCellValue == "客户姓名" &&
                titleRow.GetCell(index++)?.StringCellValue == "车牌号" &&
                titleRow.GetCell(index++)?.StringCellValue == "来源" &&
                titleRow.GetCell(index++)?.StringCellValue == "发帖链接" &&
                titleRow.GetCell(index++)?.StringCellValue == "审核状态" &&
                titleRow.GetCell(index++)?.StringCellValue == "返现状态" &&
                titleRow.GetCell(index++)?.StringCellValue == "备注" &&
                titleRow.GetCell(index++)?.StringCellValue == "对应编号"&&
                titleRow.GetCell(index++)?.StringCellValue == "发帖时间")
            {
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var item = new RebateConfigModel { };
                    try
                    {
                        item.OrderId = int.Parse(getStringValue(row.GetCell(cellIndex++)));
                        item.UserPhone = getStringValue(row.GetCell(cellIndex++));
                        item.WXId = getStringValue(row.GetCell(cellIndex++));
                        item.WXName = getStringValue(row.GetCell(cellIndex++));
                        item.UserName = getStringValue(row.GetCell(cellIndex++));
                        item.CarNumber = getStringValue(row.GetCell(cellIndex++));
                        item.Source = getStringValue(row.GetCell(cellIndex++));
                        item.ContentUrl = getStringValue(row.GetCell(cellIndex++));
                        cellIndex++;
                        cellIndex++;
                        cellIndex++;
                        item.PrincipalPerson = getStringValue(row.GetCell(cellIndex++));
                        item.CreateTime = Convert.ToDateTime(getStringValue(row.GetCell(cellIndex++)));
                        result.Add(item);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else
            {
                msg = "导入模板有误";
            }
            return Tuple.Create(result, msg);
        }

        [ValidateInput(false)]
        public JsonResult InsertRebateApplyPageConfig(RebateApplyPageConfig data)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var result = manager.InsertRebateApplyPageConfig(data, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RebatePageConfig()
        {
            return View();
        }

        public JsonResult SelectRebatePageConfig(int pageIndex = 1, int pageSize = 10)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var result = manager.SelectRebateApplyPageConfig(pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WxPayManualRetry(int pkid)
        {
            RebateConfigManager manager = new RebateConfigManager();
            var user = User.Identity.Name;
            if (WxPayRetryUserPower.Contains(user))
            {
                var result = manager.WxPayManualRetry(pkid, user);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult InsertSearchLog(string pkid, string phone, string type)
        {
            RebateConfigManager.InsertLog(pkid, "SearchUserInfo", type == "phone" ? "查看手机号" : "查看微信号", phone, string.Empty, string.Empty, User.Identity.Name);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}