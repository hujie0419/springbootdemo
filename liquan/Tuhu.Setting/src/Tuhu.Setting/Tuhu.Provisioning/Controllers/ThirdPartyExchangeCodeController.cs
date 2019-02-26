using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.Business.ThirdPartyExchangeCode;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ThirdPartyExchangeCodeController : Controller
    {

        // GET: ThirdPartyExchangeCode
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SelectBatches(int pageSize, int pageNumber, int sort, string branchId, string branchName,
            string user, string call)
        {
            SerchElement serch = new SerchElement()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                BatchName = branchName
            };
            if (!string.IsNullOrWhiteSpace(branchId))
            {
                serch.BatchGuid = new Guid(branchId);
            }
            serch.Creator = user;
            serch.Modifier = call;
            serch.Sort = sort;
            //serch.Status = select;
            var list = ThirdPartyExchangeCodeManage.SelectBatches(serch);
            return Json(list);
        }
        public JsonResult BranchOperate(string type, string branchId, string branchName, int limitQty, int batchQty, int stockQty, DateTime startDateTime, DateTime endDateTime, string instructions, int pkid)
        {
            int result = -1;

            if (ControllerContext.HttpContext.User == null)
            {
                return Json(new { result = "请重新登录！" });
            }
            ThirdPartyCodeBatch codeBranch = new ThirdPartyCodeBatch()
            {
                PKID = pkid,
                BatchName = branchName,
                BatchQty = batchQty,
                StockQty = stockQty,
                LimitQty = limitQty,
                Instructions = instructions,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                CreateDateTime = DateTime.Now,
                Creator = ControllerContext.HttpContext.User.Identity.Name,
                UpdateDateTime = DateTime.Now,
                Modifier = ControllerContext.HttpContext.User.Identity.Name,
            };
            switch (type)
            {
                case "insert":
                    codeBranch.BatchGuid = Guid.NewGuid();
                    Session["BatchId"] = codeBranch.BatchGuid;
                    result = ThirdPartyExchangeCodeManage.InserBatches(codeBranch);
                    if (result > 0)
                    {
                        new OprLogManager().AddOprLog(new OprLog()
                        {
                            Author = HttpContext.User.Identity.Name,
                            AfterValue = JsonConvert.SerializeObject(codeBranch),
                            ChangeDatetime = DateTime.Now,
                            ObjectID = result,
                            ObjectType = "ExchangeCodeBatch",
                            Operation = "新增兑换码批次记录",
                            HostName = Request.UserHostName
                        });
                    }
                    break;
                case "update":
                    codeBranch.BatchGuid = new Guid(branchId);
                    result = ThirdPartyExchangeCodeManage.UpdateBatches(codeBranch);
                    if (result > 0)
                    {
                        new OprLogManager().AddOprLog(new OprLog()
                        {
                            Author = HttpContext.User.Identity.Name,
                            AfterValue = JsonConvert.SerializeObject(codeBranch),
                            ChangeDatetime = DateTime.Now,
                            ObjectID = codeBranch.PKID,
                            ObjectType = "ExchangeCodeBatch",
                            Operation = "编辑兑换码批次记录",
                            HostName = Request.UserHostName
                        });
                    }
                    break;
            }
            return Json(new { msg = result });
        }

        //public static Guid BatchId = new Guid();
        public ActionResult Detail(string batchId)
        {
            Session["BatchId"] = batchId;
            HttpCookie batcook = new HttpCookie("batcook", batchId);
            batcook.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(batcook);
            SerchCodeElement detail = new SerchCodeElement()
            {
                BatchId = new Guid(batchId),
                PageSize = 50,
                PageNumber = 1
            };
            var result = ThirdPartyExchangeCodeManage.SelectExchangCode(detail);
            return View(result);
        }

        public JsonResult SelectExchangCode(int pageSize, int pageNumber, int sort, DateTime? gainDateTime, bool? isEnabled, bool? isGain, int? outTime, int? onTime)
        {
            string Batch = "";
            if (Session["BatchId"] == null)
            {
                Batch = Request.Cookies.Get("batcook")?.Value;
            }
            Batch = Session["BatchId"].ToString();
            SerchCodeElement detail = new SerchCodeElement();
            detail.BatchId = new Guid(Batch);
            detail.PageNumber = pageNumber;
            detail.PageSize = pageSize;
            detail.Sort = sort;
            detail.IsEnabled = isEnabled;
            detail.IsGain = isGain;
            if (outTime == 1)
            {
                detail.OutTime = DateTime.Now;
            }
            else
            {
                detail.OutTime = null;
            }
            if (outTime == 1)
            {
                detail.OnTime = DateTime.Now;
            }
            else
            {
                detail.OnTime = null;
            }

            var list = ThirdPartyExchangeCodeManage.SelectExchangCode(detail);

            return Json(list);
        }
        public JsonResult SelectBatch(string batchId)
        {
            var result = ThirdPartyExchangeCodeManage.SelectBatch(new Guid(batchId));
            return Json(result);
        }

        [HttpPost]
        public JsonResult Import(string batchId, DateTime endDateTime, DateTime startDateTime, int batchQty, int stockQty, int pkid)
        {
            if (Session["BatchId"] == null)
            {
                return Json(new { Status = -4, Result = "回话结束，请重新导入" }, "text/html");
            }
            if (batchId == "0")
            {
                batchId = Session["BatchId"].ToString();
            }
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (!file.FileName.Contains(".xlsx") && !file.FileName.Contains(".xls"))
                        return Json(new { Status = -1, Result = "请上传.xlsx文件或者.xls文件！" }, "text/html");

                    var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("Sheet1", true);

                    var lil = dt.ToList<string>();

                    List<string> errordt = new List<string>();
                    #region 批量将读取到的excel数据导入到数据库

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        ThirdPartyExchangeCode eCode = new ThirdPartyExchangeCode();
                        //创建兑换码明细的对照关系
                        string exchangeCode = dr["ExchangeCode"]?.ToString();
                        eCode.ExchangeCode = exchangeCode;
                        eCode.BatchGuid = new Guid(batchId);
                        eCode.EndDateTime = endDateTime;
                        eCode.GainDateTime = null;
                        eCode.ImportDateTime = DateTime.Now;
                        eCode.IsEnabled = true;
                        eCode.IsGain = false;
                        eCode.Operator = ControllerContext.HttpContext.User.Identity.Name;
                        eCode.StartDateTime = startDateTime;

                        var result = ThirdPartyExchangeCodeManage.InsertExchangeCode(eCode);

                        if (result < 1)
                        {
                            errordt.Add(exchangeCode);
                        }
                        #endregion

                    }
                    HashSet<string> li = new HashSet<string>(lil);
                    if (li.Count != lil.Count)
                    { return Json(new { Status = -3, Result = "兑换码不能重复，请检查兑换码！" }, "text/html"); }
                    //#endregion
                    var num = dt.Rows.Count - errordt.Count;
                    batchQty = batchQty + num;
                    stockQty = stockQty + num;
                    var udResult = ThirdPartyExchangeCodeManage.UpdateQty(pkid, batchQty, stockQty);
                    if (udResult > 0)
                    {
                        new OprLogManager().AddOprLog(new OprLog()
                        {
                            Author = HttpContext.User.Identity.Name,
                            AfterValue = $"{{ pkid:{pkid},batchId:{batchId},startDateTime:{startDateTime},endDateTime:{endDateTime},batchQty:{batchQty },stockQty:{stockQty}",
                            ChangeDatetime = DateTime.Now,
                            ObjectID = pkid,
                            ObjectType = "ExchangeCodeBatch",
                            Operation = "导入兑换码",
                            HostName = Request.UserHostName
                        });
                    }
                    #region 将问题输数据导出到excel

                    using (
                        MemoryStream ms =
                            new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Export/批次导入兑换码.xlsx")))
                        )
                    {
                        if (errordt.Any())
                        {
                            //创建工作簿对象
                            XSSFWorkbook book = new XSSFWorkbook(ms); //创建excel 2007工作簿对象，
                            //创建工作表
                            ISheet sheet1 = book.GetSheetAt(0);
                            //创建行row
                            IRow row1 = sheet1.CreateRow(0);

                            #region 工作簿的首行，头部标题

                            row1.CreateCell(0).SetCellValue("ExchangeCode");

                            #endregion

                            for (var i = 0; i < errordt.Count(); i++)
                            {
                                string item = errordt[i];

                                var row = sheet1.CreateRow(i + 1);

                                row.CreateCell(0).SetCellValue(item);


                            }
                            Response.ContentType = "application/vnd.ms-excel";
                            Response.Charset = "";
                            Response.AppendHeader("Content-Disposition", "attachment;fileName=批次导入兑换码问题兑换码" + ".xlsx");
                            book.Write(Response.OutputStream);
                            Response.End();
                        }
                    }

                    #endregion

                    return Json(new { Status = 0, Result = "写入完成" }, "text/html");
                }
                return Json(new { Status = -1, Result = "请选中文件" }, "text/html");
            }
            catch (Exception em)
            {
                return Json(new { Status = -2, Result = em.Message }, "text/html");
            }
        }


        public ActionResult SelectCout(int pageSize, int pageNumber, int sort, string branchId, string branchName,
            string user, string call)
        {
            SerchElement serch = new SerchElement()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                BatchName = branchName
            };
            if (!string.IsNullOrWhiteSpace(branchId))
            {
                serch.BatchGuid = new Guid(branchId);
            }
            serch.Creator = user;
            serch.Modifier = call;
            serch.Sort = sort;
            //serch.Status = select;
            var result = ThirdPartyExchangeCodeManage.SelectCount(serch);
            return Json(new { msg = result }); ;
        }

        public JsonResult SelectLog(string pkid)
        {
            var content = "";
            var result = LoggerManager.SelectOprLogByParams("ExchangeCodeBatch", pkid.ToString());
            var configHistories = result as ConfigHistory[] ?? result.ToArray();
            if (result != null && configHistories.Any())
            {
                content = configHistories.Aggregate(content, (current, h) => current + ("<tr><td>" + h.Author + "</td><td>" + h.Operation + "</td><td>" + h.ChangeDatetime + "</td></tr>"));
            }
            return Json(content);
        }
    }
}