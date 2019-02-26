using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Activity;
using System.Data;
using NPOI.XSSF.UserModel;
using Tuhu.Component.ExportImport;
using System.IO;
using Tuhu.Provisioning.Common;

namespace Tuhu.Provisioning.Controllers
{
    public class StarRatingStoreController : Controller
    {
        private readonly Lazy<StarRatingStoreManage> lazyStarRatingStoreManager = new Lazy<StarRatingStoreManage>();

        private StarRatingStoreManage StarRatingStoreManager
        {
            get { return this.lazyStarRatingStoreManager.Value; }
        }


        /// <summary>
        /// 工厂店投放-获取某个时间段的工厂店列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public ActionResult GetStarRatingStoreList(string startTime="", string endTime="", int pageSize = 10, int pageIndex = 1)
        {
            if(!string.IsNullOrWhiteSpace(startTime) && !string.IsNullOrWhiteSpace(endTime))
            {
                if(Convert.ToDateTime(endTime)< Convert.ToDateTime(startTime))
                {
                    var temp = startTime;
                    startTime = endTime;
                    endTime = temp;
                }
            }
            //return Json(new { pageSize = pageSize, pageIndex = pageIndex, startTime = startTime, endTime = endTime }, JsonRequestBehavior.AllowGet);
            int recordCount = 0;
            var list= StarRatingStoreManager.GetStarRatingStoreList(out recordCount, startTime, endTime, pageSize, pageIndex);
            return Json(new { data = list, count = recordCount, pageSize= pageSize, pageIndex= pageIndex, startTime= startTime, endTime= endTime },JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 工厂店投放-查看详情
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public ActionResult GetStarRatingStoreModel(int PKID)
        {
            var model = StarRatingStoreManager.GetStarRatingStoreModel(PKID);
            return Json(new { data = model},JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 工厂店投放-下载所选时间段表单信息
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public ActionResult ExportExcel(string startTime, string endTime)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var index = 0;
            var row = sheet.CreateRow(index++);

            var num = 0;
            row.CreateCell(num++).SetCellValue("姓名");
            row.CreateCell(num++).SetCellValue("手机号码");
            row.CreateCell(num++).SetCellValue("门店名称");
            row.CreateCell(num++).SetCellValue("职务");
            row.CreateCell(num++).SetCellValue("省份");
            row.CreateCell(num++).SetCellValue("城市");
            row.CreateCell(num++).SetCellValue("区/县");
            row.CreateCell(num++).SetCellValue("门店详细地址");
            row.CreateCell(num++).SetCellValue("门店地址");
            row.CreateCell(num++).SetCellValue("门店面积");
            row.CreateCell(num++).SetCellValue("门面数量");
            row.CreateCell(num++).SetCellValue("工位数");
            row.CreateCell(num++).SetCellValue("维修资质");
            row.CreateCell(num++).SetCellValue("现有门头");
            row.CreateCell(num++).SetCellValue("门头备注");
            row.CreateCell(num++).SetCellValue("门店位置");
            row.CreateCell(num++).SetCellValue("是否同意按照途虎认证店自行制作门头");
            row.CreateCell(num++).SetCellValue("创建时间");

            num = 0;
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            var exm = new StarRatingStoreManage();
            var list = exm.GetStarList(startTime, endTime);
            foreach (var item in list)
            {
                row = sheet.CreateRow(index++);
                num = 0;
                string StoreNumString = item.StoreNum == 1 ? "一个" : item.StoreNum == 2 ? "二个" : item.StoreNum == 3 ? "三个" : "四个以上";
                string IsAgreeString = item.IsAgree == true ? "是" : "否";
                row.CreateCell(num++).SetCellValue(item.UserName);
                row.CreateCell(num++).SetCellValue(item.Phone);
                row.CreateCell(num++).SetCellValue(item.StoreName);
                row.CreateCell(num++).SetCellValue(item.Duty);
                row.CreateCell(num++).SetCellValue(item.ProvinceName);
                row.CreateCell(num++).SetCellValue(item.CityName);
                row.CreateCell(num++).SetCellValue(item.DistrictName);
                row.CreateCell(num++).SetCellValue(item.StoreAddress);
                row.CreateCell(num++).SetCellValue(item.Area);
                row.CreateCell(num++).SetCellValue((double)item.StoreArea);
                row.CreateCell(num++).SetCellValue(StoreNumString);
                row.CreateCell(num++).SetCellValue(item.WorkPositionNum);
                row.CreateCell(num++).SetCellValue(item.MaintainQualification);
                row.CreateCell(num++).SetCellValue(item.Storefront);
                row.CreateCell(num++).SetCellValue(item.StorefrontDesc);
                row.CreateCell(num++).SetCellValue(item.StoreLocation);
                row.CreateCell(num++).SetCellValue(IsAgreeString); 
                row.CreateCell(num++).SetCellValue(item.CreateDateTime.ToString()); 
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"星级门店认证表单信息-{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }
    }
}