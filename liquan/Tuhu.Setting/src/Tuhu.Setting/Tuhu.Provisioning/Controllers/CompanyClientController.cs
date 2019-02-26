using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.CompanyClient;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class CompanyClientController : Controller
    {
        public ActionResult CompanyClientConfig()
        {
            return View();
        }

        public JsonResult GetAllCompanyClientConfig(int pageIndex = 1, int pageSize = 15)
        {
            CompanyClientManager manager = new CompanyClientManager();
            var result = manager.GetAllCompanyClientConfig(pageIndex, pageSize);
            if (result != null && result.Any())
            {
                return Json(new { status = "success", data = result, count = result.FirstOrDefault().Total }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SelectCouponCodeByParentId(int parentId, int isBind)
        {
            CompanyClientManager manager = new CompanyClientManager();
            var result = manager.SelectCouponCodeByParentId(parentId, isBind);
            if (result != null && result.Any())
            {
                return Json(new { status = "success", data = result, count = result.FirstOrDefault().Total }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail", count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult InsertCompanyClientConfig(string channel, string url)
        {
            CompanyClientManager manager = new CompanyClientManager();

            var result = manager.InsertCompanyClientConfig(channel, url, HttpContext.User.Identity.Name);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateCompanyClientConfig(string channel, string url, int pkid)
        {
            CompanyClientManager manager = new CompanyClientManager();

            var result = manager.UpdateCompanyClientConfig(channel, url, pkid, HttpContext.User.Identity.Name);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeletedCompanyClientConfig(int pkid)
        {
            CompanyClientManager manager = new CompanyClientManager();

            var result = manager.DeletedCompanyClientConfig(pkid, HttpContext.User.Identity.Name);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeletedCouponCodeByPkid(int pkid)
        {
            CompanyClientManager manager = new CompanyClientManager();

            var result = manager.DeletedCouponCodeByPkid(pkid);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GenerateCouponCode(string channel, int count, int parentId)
        {
            CompanyClientManager manager = new CompanyClientManager();

            var result = manager.GenerateCouponCode(channel, count, parentId, HttpContext.User.Identity.Name);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ClientCouponCode(int parentId,string channel)
        {
            ViewBag.ParentId = parentId;
            ViewBag.Channel = channel;
            return View();
        }

        public ActionResult ExportClientCouponCode(int parentId, int isBind,string channel)
        {
            CompanyClientManager manager = new CompanyClientManager();
            var result = manager.SelectCouponCodeByParentId(parentId, isBind);
            
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            var fileName = channel + DateTime.Now.ToString("yyyy_MM_dd_HHmm") + ".xls";
            row1.CreateCell(0).SetCellValue("渠道");
            row1.CreateCell(1).SetCellValue("活动券码");
            row1.CreateCell(2).SetCellValue("手机号");
            row1.CreateCell(3).SetCellValue("创建时间");
            if (result != null && result.Any())
            {
                for (var i=0;i<result.Count;i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(channel);
                    rowtemp.CreateCell(1).SetCellValue(result[i].CouponCode);
                    rowtemp.CreateCell(2).SetCellValue(result[i].Telephone);
                    rowtemp.CreateCell(3).SetCellValue(result[i].CreatedTime.ToString());
                }
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        public JsonResult SelectOperationLog(string objectId)
        {
            CompanyClientManager manager = new CompanyClientManager();
            var result = manager.SelectOperationLog(objectId, string.Empty);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}