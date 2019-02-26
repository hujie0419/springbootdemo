using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.MoveCarQRCode;
using Tuhu.Provisioning.DataAccess.Entity.MoveCarQRCode;
using Tuhu.Service.ThirdParty;
using Tuhu.Service.ThirdParty.Models;

namespace Tuhu.Provisioning.Controllers
{
    /// <summary>
    /// 途虎挪车二维码配置后台
    /// </summary>
    public class MoveCarQRCodeController : Controller
    {
        private static readonly ILog logger = LoggerFactory.GetLogger("MoveCarQRCodeController");

        /// <summary>
        /// 新增生成
        /// </summary>
        /// <param name="generationNum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerationMoveCarQRCode(int generationNum)
        {

            var manager = new MoveCarQRCodeManager();

            //添加途虎挪车二维码生成记录
            var generationRecordModel = new MoveCarGenerationRecordsModel();
            generationRecordModel.GeneratedNum = generationNum;
            generationRecordModel.GeneratingStatus = 0; 
            generationRecordModel.CreateBy = User.Identity.Name;
            generationRecordModel.LastUpdateBy = User.Identity.Name;
            int batchID = manager.AddMoveCarGenerationRecords(generationRecordModel);//生成记录表的pkid
            bool isSuccess = batchID > 0;
            if (isSuccess)
            {
                return Json(new { status = isSuccess, msg = "操作成功！" });
            }
            else
            {
                return Json(new { status = isSuccess, msg = "操作失败！" });
            }
        }

        /// <summary>
        /// 新增下载
        /// </summary>
        /// <param name="downloadNum"></param>
        /// <returns></returns>
        public ActionResult DownloadMoveCarQRCode(int downloadNum)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var index = 0;
            var row = sheet.CreateRow(index++);

            var num = 0;
            row.CreateCell(num++).SetCellValue("二维码");
            row.CreateCell(num++).SetCellValue("使用状态");
            row.CreateCell(num++).SetCellValue("生成时间");

            num = 0;
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            var exm = new MoveCarQRCodeManager();

            //更新途虎挪车二维码表的下载flag为true 并获取更新flag的列表
            var list = exm.UpdateDownloadFlagAndSelectMoveCarQRCode(downloadNum, User.Identity.Name);


            //修改途虎挪车二维码总下载记录
            var existTotalModel = exm.GetMoveCarTotalRecord();
            var totalRecordModel = new MoveCarTotalRecordsModel();
            totalRecordModel.GeneratedNum = 0;
            totalRecordModel.DownloadedNum = downloadNum;
            exm.AddOrUpdateMoveCarTotalRecord(totalRecordModel, 1);

            string bingStatus = string.Empty;
            foreach (var item in list)
            {
                row = sheet.CreateRow(index++);
                num = 0;
                if (item.IsBinding == false) bingStatus = "未绑定";
                else bingStatus = "已绑定";
                row.CreateCell(num++).SetCellValue(item.QRCodeImageUrl);
                row.CreateCell(num++).SetCellValue(bingStatus);
                row.CreateCell(num++).SetCellValue(item.CreateDatetime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"下载途虎挪车二维码-{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}-{downloadNum}.xlsx");
        }

        /// <summary>
        /// 获取途虎挪车二维码生成记录列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public ActionResult GetMoveCarGenerationRecordsList(int pageSize = 20, int pageIndex = 1)
        {
            var manager = new MoveCarQRCodeManager();
            int recordCount = 0;
            var list = manager.GetMoveCarGenerationRecordsList(out recordCount, pageSize, pageIndex);
            return Json(new { data = list, count = recordCount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取途虎挪车二维码总生成下载记录
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMoveCarTotalRecordsModel()
        {
            var manager = new MoveCarQRCodeManager();
            var totalRecordsModel = manager.GetMoveCarTotalRecordsModel();
            return Json(new { data = totalRecordsModel }, JsonRequestBehavior.AllowGet);
        }
    }
}