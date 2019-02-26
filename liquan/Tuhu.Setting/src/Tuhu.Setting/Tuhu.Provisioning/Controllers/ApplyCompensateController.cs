using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ApplyCompensateController : Controller
    {
        private static readonly Lazy<ApplyCompensateManager> lazy = new Lazy<ApplyCompensateManager>();

        private ApplyCompensateManager ApplyCompensateManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index(int pageIndex = 1)
        {
            return View();
        }

        public ActionResult List(ApplyCompensate model, int pageIndex = 1, int pageSize = 30)
        {
            int count = 0;

            var lists = ApplyCompensateManager.GetApplyCompensateList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<ApplyCompensate>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<ApplyCompensate>(list.ReturnValue, pager));
        }

        public JsonResult Export(ApplyCompensate model)
        {
            using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Export/申请赔付.xlsx"))))
            {
                var list = ApplyCompensateManager.GetApplyCompensate(model);
                var xssfWorkbook = new XSSFWorkbook(stream); //创建Workbook对象  2007+   
                                                             // var hssfWorkbook = new HSSFWorkbook(stream)://2003
                if (list.Count > 0)
                {
                    var i = 0;
                    var sheet = xssfWorkbook.GetSheetAt(0);
                    foreach (var item in list)
                    {
                        var status = "";
                        if (item.Status == 0)
                        {
                            status = "待审核";
                        }
                        else if (item.Status == 1)
                        {
                            status = "已驳回";
                        }
                        else
                        {
                            status = "已通过";
                        }
                        var row = sheet.CreateRow((i++) + 1);
                        row.CreateCell(0).SetCellValue(item.Id);
                        row.CreateCell(1).SetCellValue(item.UserName);
                        row.CreateCell(2).SetCellValue(item.PhoneNumber);
                        row.CreateCell(3).SetCellValue(item.OrderId);
                        row.CreateCell(4).SetCellValue(item.ProductName);
                        row.CreateCell(5).SetCellValue(item.DifferencePrice.ToString());
                        row.CreateCell(6).SetCellValue(item.ApplyTime?.ToString());
                        row.CreateCell(7).SetCellValue(item.AuditTime?.ToString());
                        row.CreateCell(8).SetCellValue(status);
                        row.CreateCell(9).SetCellValue(item.Link);
                        row.CreateCell(10).SetCellValue(item.OrderChannel);
                        if (!string.IsNullOrWhiteSpace(item.Images))
                        {
                            int imgId = 11;
                            foreach (var img in item.Images.Split(';'))
                            {
                                row.CreateCell(imgId).SetCellValue(img);
                                imgId++;
                            }
                        }

                    }
                }

                Response.AppendHeader("Content-Disposition", "attachment;fileName=申请赔付" + ".xlsx");
                xssfWorkbook.Write(Response.OutputStream);
                Response.End();
            }

            return Json(true);
        }

        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new ApplyCompensate());
            }
            else
            {
                return View(ApplyCompensateManager.GetApplyCompensate(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ApplyCompensate model)
        {
            string js = "<script>alert(\"保存失败 \");location='/ApplyCompensate/Index';</script>";

            if (ApplyCompensateManager.UpdateApplyCompensate(model))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Content(js);
            }
        }

    }
}
