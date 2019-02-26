using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class VPCouponPriceController : Controller
    {
        /// <summary>
        /// 获取所有指定产品类型的品牌
        /// </summary>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public async Task<JsonResult> GetAllBrands(string productType)
        {
            var manager = new VendorProductCommonManager();
            var managerResult = await manager.GetAllBrandsFromCache(productType);
            return Json(new { Status = managerResult.Item1 != null, Data = managerResult.Item1, Msg = managerResult.Item2 },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加或更新券后价展示配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpsertConfig(VendorProductCouponPriceConfigModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Pid))
            {
                return Json(new { Status = false, Msg = "请选择服务Pid" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VendorProductCouponPriceManager();
            var isExist = manager.IsExistVendorProductCouponPriceConfig(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复添加" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.UpSertVendorProductCouponPriceConfig(model, User.Identity.Name);
            return Json(new { Status = result, Msg = $"操作{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量添加或更新券后价展示配置
        /// </summary>
        /// <param name="models"></param>
        /// <param name="isShow"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MultUpsertConfig(List<VendorProductCouponPriceConfigModel> models, bool isShow)
        {
            if (models == null || !models.Any())
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            models.ForEach(s => s.IsShow = isShow);
            var manager = new VendorProductCouponPriceManager();
            var result = manager.MultUpSertVendorProductCouponPriceConfig(models, User.Identity.Name);
            return Json(new { Status = result, Msg = $"操作{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询券后价展示配置
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectConfig(string productType, string brand)
        {
            var manager = new VendorProductCouponPriceManager();
            var result = await manager.SelectVendorProductCouponPriceConfig(productType, brand, User.Identity.Name);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出券后价展示配置
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ExportExcel(string productType, string brand)
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
            sheet.SetColumnWidth(cellNum, 15 * 256);
            var manager = new VendorProductCouponPriceManager();
            var result = await manager.SelectVendorProductCouponPriceConfig(productType, brand,User.Identity.Name);
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
            var productTypeZhName= manager.GetZhNameByProductType(productType);
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls",
                $"{productTypeZhName}产品_{brand}品牌券后价查询 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RemoveCache(string productType, List<string> pids)
        {
            if (string.IsNullOrEmpty(productType) || pids == null || !pids.Any())
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VendorProductCouponPriceManager();
            var result = await manager.RemoveCache(productType, pids);
            return Json(new { Status = result, Msg = $" 清除缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
    }
}