using Common.Logging;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.CustomersActivity;
using Tuhu.Provisioning.Business.GroupBuyingV2;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.CustomersActivity;
using Tuhu.Provisioning.DataAccess.Entity.GroupBuyingV2;
using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.Models.GroupBuyingV2;
using Tuhu.Service.ConfigLog;
using Tuhu.Service.Member;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Request;
using Tuhu.Service.Product;
using Tuhu.Service.Purchase.Models;
using ProductModel = Tuhu.Provisioning.Models.GroupBuyingV2.ProductModel;


namespace Tuhu.Provisioning.Controllers
{
    public class CustomersActivityController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger<CustomersActivityController>();

        #region 大客户活动专享配置
        /// <summary>
        ///  查询大客户专享活动配置列表
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        public JsonResult GetCustomerExclusiveSettings(int pageIndex = 1, int pageSize = 20)
        {
            var listCustomerExculusive = CustomersActivityManager.SelectCustomerExclusives(pageIndex, pageSize);
            int totalCount = CustomersActivityManager.SelectCustomerExclusiveCount();

            return Json(new { data = listCustomerExculusive, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 调用服务端接口查询公司信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCompanyInfoDict()
        {
            var listCompanyInfo = CustomersActivityManager.GetCompanyInfoDict();

            return Json(new { data = listCompanyInfo }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 客户活动信息配置编辑
        /// </summary>
        /// <param name="customerExclusiveSettingModel">实体</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateCustomerExclusiveSetting(CustomerExclusiveSettingModel customerExclusiveSettingModel)
        {
            int result = CustomersActivityManager.UpdateCustomerExclusiveSetting(customerExclusiveSettingModel, User.Identity.Name);

            string message = "更新成功";
            if (result == -9)
            {
                message = "限时抢购活动ID已经存在,请从新输入!";
            }
            else if (result == 0)
            {
                message = "更新失败";
            }

            return Json(new { success = result == 1, msg = message });
        }

        /// <summary>
        /// 客户活动信息配置新增
        /// </summary>
        /// <param name="customerExclusiveSettingModel">实体</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InsertCustomerExclusiveSetting(CustomerExclusiveSettingModel customerExclusiveSettingModel)
        {
            int result = CustomersActivityManager.InsertCustomerExclusiveSetting(customerExclusiveSettingModel, User.Identity.Name);

            string message = "新增成功";
            if (result == -9)
            {
                message = "限时抢购活动ID已经存在,请从新输入!";
            }
            else if (result == 0)
            {
                message = "新增失败";
            }
            return Json(new { success = result == 1, msg = message });
        }

        #endregion


        #region 大客户活动专享券码


        /// <summary>
        ///  查询大客户专享活动券码列表
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="queryString">查询条件</param>
        /// <param name="customersSettingId">活动专享配置表PKID</param>
        /// <param name="activityExclusiveId">活动专享ID</param>
        /// <returns></returns>
        public JsonResult SelectCustomerCoupons(string queryString, string customersSettingId, string activityExclusiveId, int pageIndex = 1, int pageSize = 20)
        {
            var listCustomerCoupons = CustomersActivityManager.SelectCustomerCoupons(queryString, customersSettingId, activityExclusiveId, pageIndex, pageSize);
            int totalCount = CustomersActivityManager.SelectCustomerCouponCount(queryString, customersSettingId, activityExclusiveId);

            return Json(new { data = listCustomerCoupons, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 客户专享活动券码新增
        /// </summary>
        /// <param name="customerExclusiveCouponModel">实体</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InsertCustomerCoupon(CustomerExclusiveCouponModel customerExclusiveCouponModel)
        {
            customerExclusiveCouponModel.CouponCode = customerExclusiveCouponModel.CouponCode?.Trim();
            int result = CustomersActivityManager.InsertCustomerCoupon(customerExclusiveCouponModel, User.Identity.Name);

            string message = "新增成功";
            if (result == -9)
            {
                message = "券码已经存在,请重新输入!";
            }
            else if (result == 0)
            {
                message = "新增失败";
            }
            return Json(new { success = result == 1, msg = message });
        }


        /// <summary>
        /// 客户专享活动券码状态修改
        /// </summary>
        /// <param name="customerExclusiveCouponModel">实体</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateCustomerCouponStatus(CustomerExclusiveCouponModel customerExclusiveCouponModel)
        {
            int result = CustomersActivityManager.UpdateCustomerCouponStatus(customerExclusiveCouponModel, User.Identity.Name);

            string message = "更新成功";
            if (result == 0)
            {
                message = "更新失败";
            }

            return Json(new { success = result == 1, msg = message });
        }


        #endregion


        #region 活动专享券码导入导出功能

        /// <summary>
        /// 下载导入模板
        /// </summary>
        /// <returns></returns>
        public FileResult ExportSample()
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);

            #region title

            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("活动券码");

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            #endregion

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"活动专享券码模板.xlsx");
        }

        /// <summary>
        /// 券码批量导入
        /// </summary>
        /// <param name="customersSettingId">客户活动专享PKID</param>
        /// <param name="activityExclusiveId">客户活动专享ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Upload(string customersSettingId, string activityExclusiveId)
        {
            var files = Request.Files;

            string msg = "上传失败";
            bool bolRet = false;

            if (files.Count == 1)
            {
                var file = files[0];
                if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    msg = "请上传Excel文件";
                }
                else
                {
                    using (var stream = file.InputStream)
                    {
                        var buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                        var sheet = workBook.GetSheetAt(0);

                        var convertResult = ConvertCustomerExclusiveCoupon(sheet, customersSettingId, activityExclusiveId);
                        if (!string.IsNullOrEmpty(convertResult.Item1))
                        {
                            msg = convertResult.Item1;
                        }
                        else if (convertResult.Item2 == null || !convertResult.Item2.Any())
                        {
                            msg = "文件不能为空";
                        }
                        else if (convertResult.Item2.Distinct().Count() != convertResult.Item2.Count)
                        {
                            msg = "存在重复数据";
                        }
                        else
                        {
                            string resultCoupons = string.Empty;
                            var result = CustomersActivityManager.InsertCustomerCoupons(convertResult.Item2, User.Identity.Name, out resultCoupons);

                            if (result == convertResult.Item2.Count)
                                msg = "批量上传成功";
                            else
                                msg = "部分上传成功,共：" + convertResult.Item2.Count + "; 上传成功："
                                    + result + ";剩余" + resultCoupons + ";券码已重复";

                            bolRet = true;
                        }

                    }

                }
            }
            else
            {
                msg = "请先上传文件";
            }

            return Json(new { msg = msg, success = bolRet });
        }

        /// <summary>
        /// 转换成对应实体集合
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private Tuple<string, List<CustomerExclusiveCouponModel>> ConvertCustomerExclusiveCoupon(ISheet sheet, string customersSettingId, string activityExclusiveId)
        {
            var result = new List<CustomerExclusiveCouponModel>();
            var message = string.Empty;

            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;

            if (string.Equals(titleRow.GetCell(index++)?.StringCellValue, "活动券码"))
            {
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;


                    var customerExclusiveCouponModel = new CustomerExclusiveCouponModel();
                    customerExclusiveCouponModel.CouponCode = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    customerExclusiveCouponModel.CustomerExclusiveSettingPkId = customersSettingId;
                    customerExclusiveCouponModel.ActivityExclusiveId = activityExclusiveId;

                    if (!string.IsNullOrWhiteSpace(customerExclusiveCouponModel.CouponCode))
                    {
                        result.Add(customerExclusiveCouponModel);
                    }
                    else
                    {
                        message = $"第{rowIndex + 1}行数据错误";
                        break;
                    }
                }
            }
            else
            {
                message = "文件与模板不一致";
            }

            return Tuple.Create(message, result);
        }

        private Func<ICell, string> getStringValue = cell =>
        {
            if (cell != null)
            {
                if (cell.CellType == CellType.Numeric)
                {
                    return DateUtil.IsCellDateFormatted(cell) ?
                        cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                        cell.NumericCellValue.ToString();
                }
                return cell.StringCellValue;
            }
            return null;
        };

        /// <summary>
        /// 根据条件导出客户活动券码信息
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="customersSettingId"></param>
        /// <param name="activityExclusiveId"></param>
        /// <returns></returns>
        public FileResult ExportCustomerCoupon(string queryString, string customersSettingId, string activityExclusiveId)
        {
            #region Init

            var workBook = new XSSFWorkbook();
            var sheet = workBook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("活动专享ID");
            row.CreateCell(cellNum++).SetCellValue("活动券码");
            row.CreateCell(cellNum++).SetCellValue("姓名");
            row.CreateCell(cellNum++).SetCellValue("手机号");
            row.CreateCell(cellNum++).SetCellValue("UserId");
            row.CreateCell(cellNum++).SetCellValue("创建时间");
            row.CreateCell(cellNum++).SetCellValue("修改时间");
            row.CreateCell(cellNum++).SetCellValue("状态");

            cellNum = 0;

            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);


            #endregion Init

            #region 封装数据

            var result = CustomersActivityManager.SelectCustomerCoupons(queryString, customersSettingId, activityExclusiveId, 1, 99999999);

            if (result != null && result.Any())
            {
                for (var i = 0; i < result.Count(); i++)
                {
                    cellNum = 0;
                    NPOI.SS.UserModel.IRow rowTemp = sheet.CreateRow(i + 1);

                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].ActivityExclusiveId);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].CouponCode);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].UserName);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].Phone);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].UserId + "");
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].CreateTime.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].UpdateDatetime.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].Status == "0" ? "正常" : "删除");
                }
            }

            #endregion 封装数据

            var ms = new MemoryStream();
            workBook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        #endregion

        /// <summary>
        /// 查询客户专享活动日志信息
        /// </summary>
        /// <param name="objeId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCustomerExclusiveSettingLogs(string objeId, string source)
        {
            var result = new CustomersActivityManager().GetCustomerExclusiveSettingLog(objeId, source);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}