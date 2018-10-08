using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BatteryCouponPriceDisplay;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class BatteryCouponPriceDisplayController : Controller
    {
        /// <summary>
        /// 添加或更新蓄电池券后价展示配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpsertConfig(BatteryCouponPriceDisplayModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Pid))
            {
                return Json(new { Status = false, Msg = "请选择服务Pid" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BatteryCouponPriceDisplayManager(User.Identity.Name);
            var isExist = manager.IsExistBatteryCouponPriceDisplay(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复添加" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.UpSertBatteryCouponPriceDisplay(model);
            return Json(new { Status = result, Msg = $"操作{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量添加或更新蓄电池券后价展示配置
        /// </summary>
        /// <param name="models"></param>
        /// <param name="isShow"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MultUpsertConfig(List<BatteryCouponPriceDisplayModel> models, bool isShow)
        {
            if (models == null || !models.Any())
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            models.ForEach(s => s.IsShow = isShow);
            var manager = new BatteryCouponPriceDisplayManager(User.Identity.Name);
            var result = manager.MultUpSertBatteryCouponPriceDisplay(models);
            return Json(new { Status = result, Msg = $"操作{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询蓄电池券后价展示配置
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectConfig(string brand)
        {
            var manager = new BatteryCouponPriceDisplayManager(User.Identity.Name);
            var result = await manager.SelectBatteryCouponPriceDisplay(brand);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出蓄电池券后价展示配置
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ExportExcel(string brand)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(0);
            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("商品PID");
            row.CreateCell(cellNum++).SetCellValue("商品名称");
            row.CreateCell(cellNum++).SetCellValue("券后价");
            row.CreateCell(cellNum++).SetCellValue("可用券");
            row.CreateCell(cellNum).SetCellValue("是否展示券后价");
            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 28 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum, 5 * 256);
            var manager = new BatteryCouponPriceDisplayManager(User.Identity.Name);
            var result = await manager.SelectBatteryCouponPriceDisplay(brand);
            if (result != null && result.Any())
            {
                int modelRowCount = 1;
                foreach (var model in result)
                {
                    int modelCol = 0;
                    var modelRow = sheet.CreateRow(modelRowCount);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.Pid);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.DisplayName);
                    modelRow.CreateCell(modelCol++).SetCellValue((double)model.Price);
                    modelRow.CreateCell(modelCol++).SetCellValue(string.Join(",", model.Coupons));
                    modelRow.CreateCell(modelCol).SetCellValue(model.IsShow ? "是" : "否");
                    modelRowCount++;
                }
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls",
                $"{brand}蓄电池券后价查询 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RemoveCache(List<string> pids)
        {
            if (pids == null || !pids.Any())
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BatteryCouponPriceDisplayManager(User.Identity.Name);
            var result = await manager.RemoveCache(pids);
            return Json(new { Status = result, Msg = $" 清除缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
    }
}