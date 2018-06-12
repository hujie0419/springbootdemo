using MVCControlsToolkit.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common;
using Tuhu.Component.Framework.Extension;
using Tuhu.Models;
using Tuhu.Provisioning.Business.PromotionCodeManagerment;
using Tuhu.Provisioning.Business.ZeroActivity;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.Business.Comment;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Service.Comment;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;
using Tuhu.Provisioning.Business;
using Tuhu.Service.Comment.Models;
using Tuhu.Service.Comment.Request;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class CommentController : Controller
    {
        private readonly Lazy<LoggerManager> lazy = new Lazy<LoggerManager>();

        private LoggerManager LoggerManager
        {
            get { return lazy.Value; }

        }
        //
        // GET: /Comment/
#if DEBUG
        public const int PageDim = 20;
#else
        public const int PageDim = 20;
#endif
        [PowerManage]
        public ActionResult AdditionCommentList(int pageIndex = 1, string AdditionCommentStatusFilter = "", string SearchCriteriumFilter = "", int AutoApprove = 0, string COrAcId = "")
        {
            int commentStatus, idT;
            int? commentId = null, additionCommentId = null, idType = null, orderId = null;
            string idTextboxContent = null;
            if (!int.TryParse(AdditionCommentStatusFilter, out commentStatus))
            {
                commentStatus = 1;
            }
            //只有订单号或评论ID为空时,按照自动审核状态查询
            if (string.IsNullOrWhiteSpace(COrAcId))
            {
                return View(CommentReportManager.SelectAdditionProductCommentByOrderId(pageIndex, PageDim, commentStatus, orderId, AutoApprove));
            }

            if (int.TryParse(SearchCriteriumFilter, out idT))
            {
                if (idT != 0 && !string.IsNullOrWhiteSpace(COrAcId))
                {
                    idType = idT;
                    idTextboxContent = COrAcId.Trim();


                    #region 根据订单号进行查询

                    if (idT == 3)
                    {
                        int acid;
                        if (int.TryParse(idTextboxContent, out acid))
                        {
                            orderId = acid;
                        }
                        return View(
                            CommentReportManager.SelectAdditionProductCommentByOrderId(pageIndex, PageDim,
                                commentStatus, orderId));
                    }
                    #endregion                   
                    if (idT == 1)
                    {
                        int cid;
                        if (int.TryParse(idTextboxContent, out cid))
                        {
                            commentId = cid;
                        }
                    }
                    else
                    {
                        int acid;
                        if (int.TryParse(idTextboxContent, out acid))
                        {
                            additionCommentId = acid;
                        }
                    }
                }
            }
            PagedModel<AdditionProductComment> addtionalComments = new PagedModel<AdditionProductComment>();
            using (var client = new ProductCommentClient())
            {
                var result = client.SelectAdditionProductCommentByPage(new SelectAdditionProductCommentByPageRequest
                {
                    Status = commentStatus,
                    PageSize = PageDim,
                    PageIndex = pageIndex,
                    IdType = idType,
                    AdditionCommentId = additionCommentId,
                    CommentId = commentId
                });
                if (result != null && result.Success && result.Result != null)
                {
                    addtionalComments = result.Result;
                }
            }
            ViewBag.Status = commentStatus;
            ViewBag.SearchCriterium = idT == 0 ? 2 : idT;
            ViewBag.COrAcId = idTextboxContent;
            ViewBag.AutoApproveStatus = AutoApprove;
            return View(addtionalComments);
        }

        public async Task<JsonResult> AdditionCommentJudgement(int commentId, int additionCommentId, int commentStatus, int currentStatus)
        {
            bool res = false;
            using (var client = new ProductCommentClient())
            {
                var result = await client.AdditionProductCommentApprovedAsync(new AdditionProductCommentCheckRequest()
                {
                    AdditionCommentId = additionCommentId,
                    CommentId = commentId,
                    CheckStatus = commentStatus
                });
                if (result != null && result.Success && result.Result)
                {
                    res = true;
                    var userName = HttpContext.User.Identity.Name;
                    var log = new DataAccess.Entity.OprLog
                    {
                        ObjectID = additionCommentId,
                        ObjectType = "Comment",
                        BeforeValue = currentStatus == 0 ? "未审核" : (currentStatus == 1 ? "审核通过" : "审核不通过"),
                        AfterValue = commentStatus == 1 ? "审核通过" : "审核不通过",
                        Author = userName,
                        Operation = commentStatus == 1 ? "进行审核操作 + ES" : "进行审核操作 - ES"
                    };
                    new OprLogManager().AddOprLog(log);

                }
            }
            return Json(res);
        }

        //初始化
        [PowerManage]
        public ActionResult CommentList(int t = 1)
        {
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            var page = Request.QueryString["PageDim"];
            string word = Request.QueryString["keyWord"];
            int pageSize;
            string keyWord = string.Empty;
            var approveStatus = 0;
            if (string.IsNullOrEmpty(page))
            {
                pageSize = PageDim;
            }
            else
            {
                pageSize = Convert.ToInt32(page);
            }

            if (!string.IsNullOrEmpty(word))
            {
                keyWord = word;
            }
            int totalPages;
            List<KeyValuePair<LambdaExpression, OrderType>> order = new List<KeyValuePair<LambdaExpression, OrderType>>();
            int CommentStatus = 1;
            int Category = 0;
            int CommentChannel = 0;
            int wordCount = 9999;
            int commentCount = 0;
            DateTime? commentstartDate = DateTime.Now.AddDays(-1);
            DateTime? commentendDate = DateTime.Now;
            ViewBag.keyWord = keyWord;
            ViewBag.PageSize = pageSize;
            ViewBag.CommentStatus = CommentStatus;
            ViewBag.Category = Category;
            ViewBag.CommentChannel = CommentChannel;
            ViewBag.WordCount = wordCount;
            ViewBag.CommentstartDate = commentstartDate == null ? string.Empty : commentstartDate.Value.ToString("yyyy-MM-dd");
            ViewBag.CommentendDate = commentendDate == null ? string.Empty : commentendDate.Value.ToString("yyyy-MM-dd");
            var Filter = new List<int>();
            Filter.Add(CommentStatus);
            Filter.Add(Category);
            Filter.Add(CommentChannel);

            ViewBag.CommentfromPhone = dc.tbl_Comment.Where(c => c.CommentChannel == 1) == null
                ? 0 : dc.tbl_Comment.Where(c => c.CommentChannel == 1).Count();
            ViewBag.CommentfromPC = dc.tbl_Comment.Where(c => c.CommentChannel == 0) == null
                ? 0 : dc.tbl_Comment.Where(c => c.CommentChannel == 0).Count();

            Expression<Func<CommentItem, int>> defaultOrder = m => m.CommentId;
            CommentItemModel result = new CommentItemModel()
            {
                CommentItemList = CommentItemModel.GetCommentAllInfoBySearch(t, pageSize, keyWord, wordCount, approveStatus, "", "", "", commentstartDate, commentendDate, null, null, out totalPages, out commentCount, ref order, Filter, "", 1, null),
                TousuTypeItems = CommentItemModel.GetAllTousuTypeItems(),
                TotalPages = totalPages,
                CurrPage = 1,
                PrevPage = 1,
                OrderOrder = order
            };

            ViewBag.CommentCount = commentCount;

            for (var i = 0; i < result.CommentItemList.Count(); i++)
            {
                var complaint = result.CommentItemList[i].Value.Complaint;
                var tousuTypeItems = result.TousuTypeItems;
                if (!string.IsNullOrEmpty(complaint))
                {
                    result.CommentItemList[i].Value.ComplaintItem = JsonConvert.DeserializeObject<List<Complaint>>(complaint);
                    var complaintItems = result.CommentItemList[i].Value.ComplaintItem;
                    foreach (var item in complaintItems)
                    {
                        var typeText = string.Empty;
                        TousuTypeItem tousuTypeItem = null;
                        string key = item.key;
                        int level = 0;
                        bool flag = true;
                        while (flag)
                        {
                            if (level == 0)
                            {
                                tousuTypeItem = tousuTypeItems.Where(fd => fd.DicValue.Equals(key)).FirstOrDefault();
                            }
                            else
                            {
                                tousuTypeItem = tousuTypeItems.Where(fd => fd.DicValue.Equals(key) && fd.TypeLevel.Equals(level - 1)).FirstOrDefault();
                            }
                            if (tousuTypeItem != null)
                            {
                                typeText = tousuTypeItem.DicText + "-" + typeText;
                                key = tousuTypeItem.DicType;
                                level = tousuTypeItem.TypeLevel;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        if (!string.IsNullOrEmpty(typeText))
                        {
                            typeText = typeText.TrimEnd('-');
                            result.CommentItemList[i].Value.ComplaintItems += typeText + "|";
                        }
                    }
                }
            }

            return View(result);
        }

        [HttpPost, System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CommentList(CommentItemModel model, FormCollection collection, int t = 1)
        {
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            var page = Request.QueryString["PageDim"];
            int pageSize;
            if (string.IsNullOrEmpty(page))
            {
                pageSize = PageDim;
            }
            else
            {
                pageSize = Convert.ToInt32(page);
            }

            ViewBag.CommentfromPhone = dc.tbl_Comment.Where(c => c.CommentChannel == 1) == null ? 0 : dc.tbl_Comment.Where(c => c.CommentChannel == 1).Count();
            ViewBag.CommentfromPC = dc.tbl_Comment.Where(c => c.CommentChannel == 0) == null ? 0 : dc.tbl_Comment.Where(c => c.CommentChannel == 0).Count();

            int commentCount = 0;
            string installShop = string.Empty;
            string commentProductId = string.Empty;
            string keyWord = string.Empty;
            string orderid = string.Empty;
            string operationType = string.Empty;
            DateTime? commentstartDate = null;
            DateTime? commentendDate = null;
            DateTime? auditstartDate = null;
            DateTime? auditendDate = null;

            installShop = collection["InstallShop"];
            commentProductId = collection["CommentProductId"];
            keyWord = collection["keyWord"];
            orderid = collection["orderid"];


            if (!string.IsNullOrEmpty(collection["OperationType"]))
            {
                operationType = collection["OperationType"];
            }
            if (!string.IsNullOrEmpty(collection["CommentstartDate"]))
            {
                commentstartDate = Convert.ToDateTime(collection["CommentstartDate"]);
            }
            if (!string.IsNullOrEmpty(collection["CommentendDate"]))
            {
                commentendDate = Convert.ToDateTime(collection["CommentendDate"]);
            }
            if (!string.IsNullOrEmpty(collection["AuditstartDate"]))
            {
                auditstartDate = Convert.ToDateTime(collection["AuditstartDate"]);
            }
            if (!string.IsNullOrEmpty(collection["AuditendDate"]))
            {
                auditendDate = Convert.ToDateTime(collection["AuditendDate"]);
            }


            int.TryParse(collection["AutoApprove"], out var approveStatus);
            ViewBag.AutoApprove = approveStatus;
            //approveStatus = 1;


            string test = collection["CommentID"];
            string CommentStatus = collection["SoFilter.$.FilterDescription.CommentStatus"];
            string Category = collection["SoFilter.$.FilterDescription.Category"];
            string CommentChannel = collection["SoFilter.$.FilterDescription.CommentChannel"];
            string CommentTousuType = collection["SoFilter.$.FilterDescription.CommentTousuType"];
            var wordCount = Convert.ToInt32(collection["SoFilter.$.FilterDescription.WordCount"]);
            CommentTousuType = CommentTousuType ?? string.Empty;
            ViewBag.InstallShop = installShop ?? string.Empty;
            ViewBag.CommentProductId = commentProductId ?? string.Empty;
            ViewBag.CommentTousuType = CommentTousuType;
            ViewBag.CommentStatus = Convert.ToInt32(CommentStatus);
            ViewBag.Category = Convert.ToInt32(Category);
            ViewBag.CommentChannel = Convert.ToInt32(CommentChannel);
            ViewBag.PageSize = pageSize;
            ViewBag.WordCount = wordCount;
            ViewBag.keyWord = keyWord ?? string.Empty;
            ViewBag.orderid = orderid ?? string.Empty;
            ViewBag.CommentstartDate = commentstartDate == null ? string.Empty : commentstartDate.Value.ToString("yyyy-MM-dd");
            ViewBag.CommentendDate = commentendDate == null ? string.Empty : commentendDate.Value.ToString("yyyy-MM-dd");
            ViewBag.AuditstartDate = auditstartDate == null ? string.Empty : auditstartDate.Value.ToString("yyyy-MM-dd");
            ViewBag.AuditendDate = auditendDate == null ? string.Empty : auditendDate.Value.ToString("yyyy-MM-dd");

            if (collection["CommentID"] != null)
            {
                tbl_Comment model1 = dc.tbl_Comment.SingleOrDefault(m => m.CommentId.Equals(Convert.ToInt32(collection["CommentID"])));
                model1.CommentStatus = 2;
                dc.SubmitChanges();

            }
            ModelState.Clear();

            if (model.CurrPage < 1) model.CurrPage = 1;
            List<KeyValuePair<LambdaExpression, OrderType>> order = model.OrderOrder;
            int totalPages;
            var Filter = new List<int>();
            Filter.Add(Convert.ToInt32(CommentStatus));
            Filter.Add(Convert.ToInt32(Category));
            Filter.Add(Convert.ToInt32(CommentChannel));

            if (operationType == "导出")
            {
                var startDate = DateTime.Now;
                var exportInBatch = CommentItemModel.GetAllCommentListByExportExcel(t, pageSize, keyWord, wordCount,
                    installShop, commentProductId, CommentTousuType, commentstartDate, commentendDate, auditstartDate,
                    auditendDate, ref order, Filter, orderid);

                //创建Excel文件的对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
                //获取list数据
                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                var fileName = "产品评论审核" + DateTime.Now.ToString("yyyy_MM_dd_HHmm") + ".xls";
                row1.CreateCell(0).SetCellValue("评论来源");
                row1.CreateCell(1).SetCellValue("用户名称");
                row1.CreateCell(2).SetCellValue("评论内容");
                row1.CreateCell(3).SetCellValue("审核状态");
                row1.CreateCell(4).SetCellValue("评论的产品ID");
                row1.CreateCell(5).SetCellValue("评论的产品名称");
                row1.CreateCell(6).SetCellValue("订单ID");
                row1.CreateCell(7).SetCellValue("评论的门店");
                row1.CreateCell(8).SetCellValue("创建时间");
                row1.CreateCell(9).SetCellValue("R1");
                row1.CreateCell(10).SetCellValue("R2");
                row1.CreateCell(11).SetCellValue("R3");
                row1.CreateCell(12).SetCellValue("R4");
                row1.CreateCell(13).SetCellValue("R5");
                row1.CreateCell(14).SetCellValue("R6");
                row1.CreateCell(15).SetCellValue("R7");
                row1.CreateCell(16).SetCellValue("投诉类型");
                if (exportInBatch != null && exportInBatch.Any())
                {
                    var rowNum = 1;
                    foreach (var item in exportInBatch)
                    {
                        NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(rowNum++);
                        rowtemp.CreateCell(0).SetCellValue(item.CommentSource);
                        rowtemp.CreateCell(1).SetCellValue(item.CommentUserName);
                        rowtemp.CreateCell(2).SetCellValue(item.CommentContent);
                        rowtemp.CreateCell(3).SetCellValue(item.CommentStatusName);
                        rowtemp.CreateCell(4).SetCellValue(item.CommentProductId);
                        rowtemp.CreateCell(5).SetCellValue(item.CommentProductName);
                        rowtemp.CreateCell(6).SetCellValue(item.CommentOrderId.ToString());
                        rowtemp.CreateCell(7).SetCellValue(item.CommentExtAttr);
                        rowtemp.CreateCell(8).SetCellValue(item.CreateTimeDate.ToString());
                        rowtemp.CreateCell(9).SetCellValue(item.CommentR1.ToString());
                        rowtemp.CreateCell(10).SetCellValue(item.CommentR2.ToString());
                        rowtemp.CreateCell(11).SetCellValue(item.CommentR3.ToString());
                        rowtemp.CreateCell(12).SetCellValue(item.CommentR4.ToString());
                        rowtemp.CreateCell(13).SetCellValue(item.CommentR5.ToString());
                        rowtemp.CreateCell(14).SetCellValue(item.CommentR6.ToString());
                        rowtemp.CreateCell(15).SetCellValue(item.CommentR7.ToString());
                        rowtemp.CreateCell(16).SetCellValue(item.ComplaintItems);
                    }
                }
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel", fileName);
            }

            model = new CommentItemModel()
            {
                CommentItemList = CommentItemModel.GetCommentAllInfoBySearch(t, pageSize, keyWord, wordCount, approveStatus, installShop,
                    commentProductId, CommentTousuType, commentstartDate, commentendDate, auditstartDate, auditendDate,
                    out totalPages, out commentCount, ref order, Filter, orderid, model.CurrPage, model.SoFilter),
                TousuTypeItems = CommentItemModel.GetAllTousuTypeItems(),
                TotalPages = totalPages,
                CurrPage = Math.Min(model.CurrPage, totalPages),
                PrevPage = Math.Min(model.CurrPage, totalPages),
                SoFilter = model.SoFilter,
                OrderOrder = order,
            };

            ViewBag.CommentCount = commentCount;

            for (var i = 0; i < model.CommentItemList.Count(); i++)
            {
                var complaint = model.CommentItemList[i].Value.Complaint;
                var tousuTypeItems = model.TousuTypeItems;
                if (!string.IsNullOrEmpty(complaint))
                {
                    model.CommentItemList[i].Value.ComplaintItem = JsonConvert.DeserializeObject<List<Complaint>>(complaint);
                    var complaintItems = model.CommentItemList[i].Value.ComplaintItem;
                    foreach (var item in complaintItems)
                    {
                        var typeText = string.Empty;
                        TousuTypeItem tousuTypeItem = null;
                        string key = item.key;
                        int level = 0;
                        bool flag = true;
                        while (flag)
                        {
                            if (level == 0)
                            {
                                tousuTypeItem = tousuTypeItems.Where(fd => fd.DicValue.Equals(key)).FirstOrDefault();
                            }
                            else
                            {
                                tousuTypeItem = tousuTypeItems.Where(fd => fd.DicValue.Equals(key) && fd.TypeLevel.Equals(level - 1)).FirstOrDefault();
                            }
                            if (tousuTypeItem != null)
                            {
                                typeText = tousuTypeItem.DicText + "-" + typeText;
                                key = tousuTypeItem.DicType;
                                level = tousuTypeItem.TypeLevel;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        if (!string.IsNullOrEmpty(typeText))
                        {
                            typeText = typeText.TrimEnd('-');
                            model.CommentItemList[i].Value.ComplaintItems += typeText + "|";
                        }
                    }
                }
            }

            return View(model);
        }

        //更新评论状态
        public async Task<ActionResult> UpdateCommentStatus(int CommentId, int commentStatus)
        {
            GungnirDataContext dc = new GungnirDataContext();
            var userName = HttpContext.User.Identity.Name;
            var isSuccess = string.Empty;
            tbl_Comment model = dc.tbl_Comment.SingleOrDefault(m => m.CommentId.Equals(CommentId));
            model.UpdateTime = DateTime.Now;
            //model.UpdateTime = Convert.ToDateTime(DateTime.Now.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString());
            //查找该评论的产品ID是否存在CommentStatistics这个数据库里面
            if (model.CommentType == 1 && model.CommentStatus == 1 && model.ParentComment == null)
            {
                if (model.InstallShopID > 0)
                {
                    var installShopID = model.InstallShopID.ToString();
                    CommentStatistics CS2 = dc.CommentStatistics.FirstOrDefault(m => m.StatisticsType == 2 && m.ObjectID == installShopID);
                    if (CS2 == null)
                    {
                        CS2 = new CommentStatistics();

                        CS2.StatisticsType = 2;
                        CS2.CommentTimes = 1;
                        CS2.ObjectID = model.InstallShopID.ToString();

                        CS2.CommentR1 = model.CommentR7.GetValueOrDefault();
                        CS2.CommentRate = (decimal)1.0 * CS2.CommentR1 / CS2.CommentTimes;
                        dc.CommentStatistics.InsertOnSubmit(CS2);
                    }
                    else
                    {
                        CS2.CommentTimes += 1;
                        CS2.CommentR1 += model.CommentR7.GetValueOrDefault();
                        CS2.CommentRate = (decimal)1.0 * CS2.CommentR1 / CS2.CommentTimes;
                    }


                }
                else
                {
                    if (!string.IsNullOrEmpty(model.CommentProductFamilyId))
                    {
                        CommentStatistics CS = dc.CommentStatistics.FirstOrDefault(m => m.StatisticsType == 1 && m.ObjectID == model.CommentProductFamilyId);
                        if (CS == null)
                        {
                            CS = new CommentStatistics();

                            CS.StatisticsType = 1;
                            CS.CommentTimes = 1;
                            CS.CommentR1 = model.CommentR1 == null ? 0 : model.CommentR1.Value;
                            CS.CommentR2 = model.CommentR2;
                            CS.CommentR3 = model.CommentR3;
                            CS.CommentR4 = model.CommentR4;
                            CS.CommentR5 = model.CommentR5;
                            CS.CommentR6 = model.CommentR6;
                            CS.ObjectID = model.CommentProductFamilyId;
                            dc.CommentStatistics.InsertOnSubmit(CS);
                            dc.SubmitChanges();
                        }
                        else
                        {
                            CS.CommentTimes += 1;
                            //CS.ObjectID = model.CommentProductFamilyId;
                            CS.CommentR1 += model.CommentR1.GetValueOrDefault();
                            CS.CommentR2 = CS.CommentR2.HasValue || model.CommentR2.HasValue ? CS.CommentR2.GetValueOrDefault() + model.CommentR2.GetValueOrDefault() : (int?)null;
                            CS.CommentR3 = CS.CommentR3.HasValue || model.CommentR3.HasValue ? CS.CommentR3.GetValueOrDefault() + model.CommentR3.GetValueOrDefault() : (int?)null;
                            CS.CommentR4 = CS.CommentR4.HasValue || model.CommentR4.HasValue ? CS.CommentR4.GetValueOrDefault() + model.CommentR4.GetValueOrDefault() : (int?)null;
                            CS.CommentR5 = CS.CommentR5.HasValue || model.CommentR5.HasValue ? CS.CommentR5.GetValueOrDefault() + model.CommentR5.GetValueOrDefault() : (int?)null;
                            CS.CommentR6 = CS.CommentR6.HasValue || model.CommentR6.HasValue ? CS.CommentR6.GetValueOrDefault() + model.CommentR6.GetValueOrDefault() : (int?)null;
                        }

                        if (CS.ObjectID.StartsWith("TR-") || CS.ObjectID.StartsWith("LG-"))
                        {
                            CS.CommentRate = (decimal)0.2 * (CS.CommentR1 + CS.CommentR2.GetValueOrDefault() + CS.CommentR3.GetValueOrDefault() + CS.CommentR4.GetValueOrDefault() + CS.CommentR5.GetValueOrDefault()) / CS.CommentTimes;

                        }
                        else
                        {
                            CS.CommentRate = (decimal)1.0 * CS.CommentR1 / CS.CommentTimes;
                        }

                    }
                }
            }

            string originalValue;
            switch (model.CommentStatus)
            {
                case 1:
                    originalValue = "未审核";
                    break;
                case 2:
                    originalValue = "审核通过";
                    break;
                case 3:
                    originalValue = "仅打分";
                    break;
                default:
                    originalValue = "审核未通过";
                    break;
            }
            var addflag = model.CommentStatus == 1 ? true : false;//状态为1则表明为初始状态    否则是被审核之后的状态
            model.CommentStatus = commentStatus;
            if (commentStatus == 2 && model.CommentType == 3)
            {
                ZeroActivityManager.UpdateZAAStatus(model.CommentOrderId.GetValueOrDefault(), model.CommentUserId.GetValueOrDefault().ToString());
            }

            var value = string.Empty;
            if (commentStatus == 0)
            {
                value = "审核不通过";
            }
            else if (commentStatus == 2)
            {
                value = "审核通过";
            }
            else if (commentStatus == 3)
            {
                value = "仅打分";
            }

            dc.SubmitChanges();
            var od = dc.Orders.Where(o => o.PKID.Equals(model.CommentOrderId)).FirstOrDefault();

            if (commentStatus == 2)
            {
                //审核通过放入ES
                try
                {
                    using (var client = new Tuhu.Service.Comment.ProductCommentClient())
                    {
                        var result = client.ProductCommentApprovedAsync(CommentId).Result;
                        if (result.Success)
                        {
                            isSuccess = " + ES";
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }

                //审核通过推送消息
                if (model.CommentType == 1)
                {
                    ShareOrderConfigManager manager = new ShareOrderConfigManager();
                    var data = manager.GetOrderSharedPushMessageConfig();
                    try
                    {
                        using (var client = new PushClient())
                        {
                            if (data != null && od != null && !string.IsNullOrEmpty(od.UserTel))
                            {
                                var message = ConvertModel(data, od.UserTel);
                                var result = client.PushMessagesAsync(message).Result;
                                if (result.Success)
                                {
                                    isSuccess += " + Push";
                                }
                            }
                            else
                            {
                                isSuccess += " + Kong";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WebLog.LogException(ex);
                    }
                }
                //审核通过，调用“增加晒单”接口（四个条件：1、晒单 2、轮胎&保养订单 3、到点安装 4、评论审核通过）
                if (model.CommentType == 2)//1、晒单
                {
                    if (od != null)
                    {
                        //2、轮胎 & 保养订单 3、到点安装  通过下面这一句即可判断
                        if (od.InstallShopID > 0 && od.OrderType == "1普通")
                        {
                            using (var client = new UserIntegralClient())
                            {
                                var result = client.InsertOrderToShareAsync((Guid)od.UserID, od.PKID, true).Result;
                                if (result.Success)
                                {
                                    isSuccess = " + ShaiDan";
                                }
                            }
                        }
                    }
                }
            }
            #region add by 陈豪骅  如果用户选择审核不通过或者仅打分，那么删除ES原来添加的记录
            //如果用户选择审核不通过或者仅打分，那么删除ES原来添加的记录
            if (commentStatus == 0 || commentStatus == 3)
            {
                //判断上一个状态 //会有之前未删除缓存的数据，不用判断了
                //if (preCommentStatus == 2)
                //{
                try
                {
                    using (var client = new Tuhu.Service.Comment.ProductCommentClient())
                    {
                        var result = client.ProductCommentUnapprovedAsync(CommentId).Result;
                        if (result.Success)
                        {
                            isSuccess = " - ES";
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }
                //}
            }
            #endregion
            var log = new Tuhu.Provisioning.DataAccess.Entity.OprLog
            {
                ObjectID = CommentId,
                ObjectType = "Comment",
                BeforeValue = originalValue,
                AfterValue = value,
                Author = userName,
                Operation = "进行审核操作" + isSuccess
            };
            new Business.OprLogManagement.OprLogManager().AddOprLog(log);

            #region 返积分
            if (model.CommentOrderId != null && (model.CommentType == 2))
            {
                //var promotions = pcm.SelectPromotionByOrderId(int.Parse(model.CommentOrderId.ToString()));
                //var px = promotions == null || promotions.Count() == 0 ? new List<BizPromotionCode>() : promotions.Where(p => p.Description.Contains("返200"));
                //if (px.Count() > 0)
                //	exist = true;

                if (od != null && addflag && (model.CommentStatus == 0 || model.CommentStatus == 2 || model.CommentStatus == 3))
                {
                    var modelIntegral = new UserIntegralDetailModel
                    {
                        TransactionIntegral = model.CommentStatus == 2 ? 80 : 10,
                        TransactionChannel = model.CommentChannel.GetValueOrDefault() == 1 ? "APP" : "pc",
                        Versions = "1",
                        TransactionRemark = model.CommentProductId == null ? "门店评价" : "商品评价",
                        IntegralRuleID = new Guid("4a035753-3b80-4480-848c-81df35eebcd0")
                    };
                    var logId =
                        await
                            InsertUserIntegralLogAsync(HttpContext.Request.UserHostAddress,
                                od.UserID.GetValueOrDefault(), modelIntegral, null);
                    if (logId > 0)
                    {
                        var result =
                            await
                                UserIntegralChangeByUserIdAsync(od.UserID.GetValueOrDefault(), modelIntegral, null,
                                    logId);
                        if (result != null)
                        {
                            var logJifen = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                            {
                                ObjectID = CommentId,
                                ObjectType = "Comment",
                                Author = userName,
                                AfterValue = model.CommentStatus == 2 ? "返还80积分" : "返还10积分",
                                Operation = "返还积分"
                            };
                            new Business.OprLogManagement.OprLogManager().AddOprLog(logJifen);
                            if (model.CommentStatus == 2)
                            {
                                string smsBody =
                                    "尊敬的途虎会员，您的晒单已通过审核，80积分已经到账，赶紧去看看吧http://t.cn/RwZIin8";

                                // 模板：尊敬的途虎会员，您的晒单已通过审核，{0}积分已经到账，赶紧去看看吧 http://t.cn/RwZIin8
                                var sendSmsSucceeded = await Business.Sms.SmsManager.SendTemplateSmsMessageAsync(od.UserTel, 74, 80);

                                var logSms = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                                {
                                    ObjectID = CommentId,
                                    ObjectType = "Comment",
                                    Author = userName,
                                    AfterValue = smsBody,
                                    Operation = sendSmsSucceeded ? "发送确认短信成功" : "发送确认短信失败"
                                };
                                new Business.OprLogManagement.OprLogManager().AddOprLog(logSms);
                            }
                        }
                    }
                }
            }
            #endregion
            return Json(true);
        }

        private static async Task<int> InsertUserIntegralLogAsync(string host, Guid userId, Service.Member.Models.UserIntegralDetailModel model, Dictionary<string, string> param)
        {
            int logId = 0;
            try
            {
                using (var client = new UserIntegralClient())
                {
                    var result = await client.InsertUserIntegralLogAsync(host, userId, model, param);
                    result.ThrowIfException(true);
                    logId = result.Result;
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return logId;
        }

        private static async Task<Dictionary<string, string>> UserIntegralChangeByUserIdAsync(Guid userId, Service.Member.Models.UserIntegralDetailModel model, Dictionary<string, string> param, int logId)
        {
            Dictionary<string, string> dic = null;
            try
            {
                using (var client = new UserIntegralClient())
                {
                    var result = await client.UserIntegralChangeByUserIDAsync(userId, model, param, logId);
                    result.ThrowIfException(true);
                    dic = result.Result;
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return dic;
        }

        private int Insert_PromotionCode(Tuhu.Component.Common.SqlDbHelper dbhelper, int discount, int minMoney, Order od, int pctype, int num)
        {
            for (int i = 0; i < num; i++)
            {
                PromotionCode tPC = new PromotionCode();

                if (od != null)
                    od.PKID = 0;
                //content.tbl_PromotionCode.AddObject(new OrderController().GetTPCModel(tPC, od, ""));

                DateTime dt = DateTime.Now;
                if (pctype == 1)
                {
                    tPC.RuleID = 25;
                    tPC.OrderID = 0;
                    tPC.Status = 0;
                    tPC.Type = pctype;//轮胎
                    tPC.EndTime = dt.AddYears(1);
                    tPC.Description = "【晒单返券】满" + minMoney + "元可用，仅限轮胎使用";
                }
                else if (pctype == 2)
                {
                    tPC.RuleID = 23;
                    tPC.OrderID = 0;
                    tPC.Status = 0;
                    tPC.Type = pctype;//保养
                    tPC.EndTime = dt.AddMonths(6);
                    tPC.Description = "【晒单返券】满" + minMoney + "元可用，仅限保养使用";
                }
                tPC.UserID = od.UserID;
                tPC.StartTime = dt;
                tPC.Discount = discount;
                tPC.MinMoney = minMoney;
                int result = PromotionCodeManager.InsertPromotionCode(dbhelper, tPC);
                if (result <= 0)
                {
                    dbhelper.Rollback();
                    return -1;
                }
            }
            return 1;

        }

        public ActionResult UpdateOfficialReply(int CommentID, string OfficialReply)
        {
            GungnirDataContext dc = new GungnirDataContext();
            var isSuccess = string.Empty;
            var Comment = dc.tbl_Comment.Single(u => u.CommentId == CommentID);
            Comment.OfficialReply = OfficialReply;
            dc.SubmitChanges();
            try
            {
                using (var client = new Tuhu.Service.Comment.ProductCommentClient())
                {
                    var result = client.ProductCommentApprovedAsync(CommentID).Result;
                    if (result.Success)
                    {
                        isSuccess = " + ES";
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            var log = new Tuhu.Provisioning.DataAccess.Entity.OprLog
            {
                ObjectID = CommentID,
                ObjectType = "Comment",
                BeforeValue = "",
                AfterValue = OfficialReply,
                Author = HttpContext.User.Identity.Name,
                Operation = "进行回复操作" + isSuccess
            };
            new Business.OprLogManagement.OprLogManager().AddOprLog(log);
            return Json(true);
        }


        public ActionResult BatchOperationCommentList()
        {
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            var page = Request.QueryString["PageDim"];
            string word = Request.QueryString["keyWord"];
            int pageSize;
            string keyWord = "";
            if (page == null)
            {
                pageSize = PageDim;
            }
            else
            {
                pageSize = Convert.ToInt32(page);
            }

            if (word != null)
            {
                keyWord = word;
            }
            int totalPages;
            List<KeyValuePair<LambdaExpression, OrderType>> order = new List<KeyValuePair<LambdaExpression, OrderType>>();
            int CommentStatus = 1;
            int Category = 0;
            int CommentChannel = 0;
            int wordCount = 9999;
            ViewBag.keyWord = keyWord;
            ViewBag.PageSize = pageSize;
            ViewBag.CommentStatus = CommentStatus;
            ViewBag.Category = Category;
            ViewBag.CommentChannel = CommentChannel;
            ViewBag.WordCount = wordCount;
            var Filter = new List<int>();
            Filter.Add(CommentStatus);
            Filter.Add(Category);
            Filter.Add(CommentChannel);
            ViewBag.CommentfromPhone = dc.tbl_Comment.Where(c => c.CommentChannel == 1 && c.CommentStatus == 1) == null ? 0 : dc.tbl_Comment.Where(c => c.CommentChannel == 1 && c.CommentStatus == 1).Count();
            ViewBag.CommentfromPC = dc.tbl_Comment.Where(c => c.CommentChannel == 0 && c.CommentStatus == 1) == null ? 0 : dc.tbl_Comment.Where(c => c.CommentChannel == 0 && c.CommentStatus == 1).Count();
            Expression<Func<CommentItem, int>> defaultOrder = m => m.CommentId;
            CommentItemModel result = new CommentItemModel()
            {
                CommentItemList = CommentItemModel.GetAllComments(pageSize, keyWord, wordCount, out totalPages, ref order, Filter, 1, null),
                TotalPages = totalPages,
                CurrPage = 1,
                PrevPage = 1,
                OrderOrder = order
            };
            //result.CommentItemList.


            return View(result);
        }

        public ActionResult CommentDetails(int CommentId)
        {
            //获得该ID的model
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            tbl_Comment model = dc.tbl_Comment.SingleOrDefault(m => m.CommentId.Equals(CommentId));
            return View(model);
        }

        public async Task<ActionResult> BatchUpdateCommentStatus(string CommentIds, int commentStatus)
        {
            var commentIds = JsonConvert.DeserializeObject<List<int>>(CommentIds);
            bool result = false;
            if (commentIds.Count() > 0)
            {
                foreach (var commentId in commentIds)
                {
                    GungnirDataContext dc = new GungnirDataContext();
                    tbl_Comment model = dc.tbl_Comment.SingleOrDefault(m => m.CommentId.Equals(commentId));
                    model.UpdateTime = DateTime.Now;
                    //model.UpdateTime = Convert.ToDateTime(DateTime.Now.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString());
                    //查找该评论的产品ID是否存在CommentStatistics这个数据库里面
                    if (model.CommentType == 1 && model.CommentStatus == 1 && model.ParentComment == null)
                    {
                        if (model.InstallShopID > 0)
                        {
                            var installShopID = model.InstallShopID.ToString();
                            CommentStatistics CS2 = dc.CommentStatistics.FirstOrDefault(m => m.StatisticsType == 2 && m.ObjectID == installShopID);
                            if (CS2 == null)
                            {
                                CS2 = new CommentStatistics();

                                CS2.StatisticsType = 2;
                                CS2.CommentTimes = 1;
                                CS2.ObjectID = model.InstallShopID.ToString();

                                CS2.CommentR1 = model.CommentR7.GetValueOrDefault();
                                CS2.CommentRate = (decimal)1.0 * CS2.CommentR1 / CS2.CommentTimes;
                                dc.CommentStatistics.InsertOnSubmit(CS2);
                            }
                            else
                            {
                                CS2.CommentTimes += 1;
                                CS2.CommentR1 += model.CommentR7.GetValueOrDefault();
                                CS2.CommentRate = (decimal)1.0 * CS2.CommentR1 / CS2.CommentTimes;
                            }


                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(model.CommentProductFamilyId))
                            {
                                CommentStatistics CS = dc.CommentStatistics.FirstOrDefault(m => m.StatisticsType == 1 && m.ObjectID == model.CommentProductFamilyId);
                                if (CS == null)
                                {
                                    CS = new CommentStatistics();

                                    CS.StatisticsType = 1;
                                    CS.CommentTimes = 1;
                                    CS.CommentR1 = model.CommentR1 == null ? 0 : model.CommentR1.Value;
                                    CS.CommentR2 = model.CommentR2;
                                    CS.CommentR3 = model.CommentR3;
                                    CS.CommentR4 = model.CommentR4;
                                    CS.CommentR5 = model.CommentR5;
                                    CS.CommentR6 = model.CommentR6;
                                    CS.ObjectID = model.CommentProductFamilyId;
                                    dc.CommentStatistics.InsertOnSubmit(CS);
                                    dc.SubmitChanges();
                                }
                                else
                                {
                                    CS.CommentTimes += 1;
                                    //CS.ObjectID = model.CommentProductFamilyId;
                                    CS.CommentR1 += model.CommentR1.GetValueOrDefault();
                                    CS.CommentR2 = CS.CommentR2.HasValue || model.CommentR2.HasValue ? CS.CommentR2.GetValueOrDefault() + model.CommentR2.GetValueOrDefault() : (int?)null;
                                    CS.CommentR3 = CS.CommentR3.HasValue || model.CommentR3.HasValue ? CS.CommentR3.GetValueOrDefault() + model.CommentR3.GetValueOrDefault() : (int?)null;
                                    CS.CommentR4 = CS.CommentR4.HasValue || model.CommentR4.HasValue ? CS.CommentR4.GetValueOrDefault() + model.CommentR4.GetValueOrDefault() : (int?)null;
                                    CS.CommentR5 = CS.CommentR5.HasValue || model.CommentR5.HasValue ? CS.CommentR5.GetValueOrDefault() + model.CommentR5.GetValueOrDefault() : (int?)null;
                                    CS.CommentR6 = CS.CommentR6.HasValue || model.CommentR6.HasValue ? CS.CommentR6.GetValueOrDefault() + model.CommentR6.GetValueOrDefault() : (int?)null;
                                }

                                if (CS.ObjectID.StartsWith("TR-") || CS.ObjectID.StartsWith("LG-"))
                                {
                                    CS.CommentRate = (decimal)0.2 * (CS.CommentR1 + CS.CommentR2.GetValueOrDefault() + CS.CommentR3.GetValueOrDefault() + CS.CommentR4.GetValueOrDefault() + CS.CommentR5.GetValueOrDefault()) / CS.CommentTimes;

                                }
                                else
                                {
                                    CS.CommentRate = (decimal)1.0 * CS.CommentR1 / CS.CommentTimes;
                                }

                            }
                        }
                    }
                    var addflag = model.CommentStatus == 1 ? true : false;//状态为1则表明为初始状态    否则是被审核之后的状态
                    model.CommentStatus = commentStatus;
                    dc.SubmitChanges();
                    result = true;
                    if (commentStatus == 2)
                    {
                        //审核通过放入ES
                        try
                        {
                            using (var client = new Tuhu.Service.Comment.ProductCommentClient())
                            {
                                var data = await client.ProductCommentApprovedAsync(commentId);
                            }
                        }
                        catch (Exception ex)
                        {
                            WebLog.LogException(ex);
                        }

                        //产品评论审核通过推送消息
                        if (model.CommentType == 1)
                        {
                            var od = dc.Orders.Where(o => o.PKID.Equals(model.CommentOrderId)).FirstOrDefault();

                            ShareOrderConfigManager manager = new ShareOrderConfigManager();
                            var data = manager.GetOrderSharedPushMessageConfig();
                            try
                            {
                                using (var client = new PushClient())
                                {
                                    if (data != null && od != null && !string.IsNullOrEmpty(od.UserTel))
                                    {
                                        var message = ConvertModel(data, od.UserTel);
                                        var pushResult = await client.PushMessagesAsync(message);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                WebLog.LogException(ex);
                            }
                        }
                    }

                    string originalValue;
                    switch (commentStatus)
                    {
                        case 1:
                            originalValue = "未审核";
                            break;
                        case 2:
                            originalValue = "审核通过";
                            break;
                        case 3:
                            originalValue = "仅打分";
                            break;
                        default:
                            originalValue = "审核未通过";
                            break;
                    }

                    var log = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = commentId,
                        ObjectType = "Comment",
                        BeforeValue = string.Empty,
                        AfterValue = originalValue,
                        Author = HttpContext.User.Identity.Name,
                        Operation = "批量审核操作"
                    };
                    new Business.OprLogManagement.OprLogManager().AddOprLog(log);
                }
            }
            else
            {
                result = false;
            }

            return Json(result);
        }

        /// <summary>
        /// 推送消息Model转换
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userTel"></param>
        /// <returns></returns>
        private PushMessageModel ConvertModel(OrderSharedPushMessageConfig model, string userTel)
        {
            PushMessageModel message = new PushMessageModel();
            message.PhoneNumbers = new List<string> { userTel };
            message.Title = model.PushTitile;//采用推送标题
            message.Content = model.PushContent;//采用推送内容
            message.Type = MessageType.AppMessage;
            message.CenterMsgType = String.Equals(model.Type, "system", StringComparison.CurrentCultureIgnoreCase) ? "1普通" : "2广播";
            message.SourceName = "setting.tuhu.cn";
            if (model.AndriodModel != null)
            {
                message.AndriodModel = new Service.Push.Models.AndriodModel()
                {
                    AfterOpen = model.AndriodModel.AfterOpen.ToString() == DataAccess.Entity.AfterOpenEnum.GoApp.ToString()
                    ? Service.Push.Models.AfterOpenEnum.GoApp : Service.Push.Models.AfterOpenEnum.GoActivity,
                    AppActivity = model.AndriodModel.AppActivity,
                    ExKey1 = model.AndriodModel.ExKey1,
                    ExValue1 = model.AndriodModel.ExValue1,
                    ExKey2 = model.AndriodModel.ExKey2,
                    ExValue2 = model.AndriodModel.ExValue2,
                };
            }
            if (model.IOSModel != null)
            {
                message.IOSModel = new Service.Push.Models.IOSModel()
                {
                    ExKey1 = model.IOSModel.ExKey1,
                    ExValue1 = model.IOSModel.ExValue1,
                    ExKey2 = model.IOSModel.ExKey2,
                    ExValue2 = model.IOSModel.ExValue2,
                    ExKey3 = model.IOSModel.ExKey3,
                    ExValue3 = model.IOSModel.ExValue3,
                    ShowBadge = model.IOSModel.ShowBadge,
                };
            }
            message.OperUser = HttpContext.User.Identity.Name;

            return message;
        }


        [HttpPost, System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult BatchOperationCommentList(CommentItemModel model, FormCollection collection)
        {
            var page = Request.QueryString["PageDim"];
            int pageSize;
            if (page == null)
            {
                pageSize = PageDim;
            }
            else
            {
                pageSize = Convert.ToInt32(page);
            }
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            ViewBag.CommentfromPhone = dc.tbl_Comment.Where(c => c.CommentChannel == 1 && c.CommentStatus == 1) == null ? 0 : dc.tbl_Comment.Where(c => c.CommentChannel == 1 && c.CommentStatus == 1).Count();
            ViewBag.CommentfromPC = dc.tbl_Comment.Where(c => c.CommentChannel == 0 && c.CommentStatus == 1) == null ? 0 : dc.tbl_Comment.Where(c => c.CommentChannel == 0 && c.CommentStatus == 1).Count();
            var wordCount = Convert.ToInt32(collection["SoFilter.$.FilterDescription.WordCount"]);
            string keyWord = collection["keyWord"];
            string CommentStatus = "1";
            string Category = "0";
            string CommentChannel = "0";

            ModelState.Clear();

            if (model.CurrPage < 1) model.CurrPage = 1;
            List<KeyValuePair<LambdaExpression, OrderType>> order = model.OrderOrder;
            int totalPages;
            if (string.IsNullOrWhiteSpace(keyWord))
            {
                ViewBag.CommentStatus = Convert.ToInt32(CommentStatus);
                ViewBag.Category = Convert.ToInt32(Category);
                ViewBag.CommentChannel = Convert.ToInt32(CommentChannel);
                ViewBag.PageSize = pageSize;
                ViewBag.WordCount = wordCount;
                var Filter = new List<int>();
                Filter.Add(Convert.ToInt32(CommentStatus));
                Filter.Add(Convert.ToInt32(Category));
                Filter.Add(Convert.ToInt32(CommentChannel));
                model = new CommentItemModel()
                {
                    CommentItemList = CommentItemModel.GetAllComments(pageSize, "", wordCount, out totalPages, ref order, Filter, model.CurrPage, model.SoFilter),
                    TotalPages = totalPages,
                    CurrPage = Math.Min(model.CurrPage, totalPages),
                    PrevPage = Math.Min(model.CurrPage, totalPages),
                    SoFilter = model.SoFilter,
                    OrderOrder = order,

                };
            }
            else
            {
                ViewBag.WordCount = wordCount;
                ViewBag.CommentStatus = 1;
                ViewBag.Category = 0;
                ViewBag.CommentChannel = 0;
                ViewBag.PageSize = pageSize;
                ViewBag.keyWord = keyWord;
                var Filter = new List<int>();
                Filter.Add(1);
                Filter.Add(0);
                Filter.Add(0);
                model = new CommentItemModel()
                {
                    CommentItemList = CommentItemModel.GetAllComments(pageSize, keyWord, wordCount, out totalPages, ref order, Filter, model.CurrPage, model.SoFilter),
                    TotalPages = totalPages,
                    CurrPage = Math.Min(model.CurrPage, totalPages),
                    PrevPage = Math.Min(model.CurrPage, totalPages),
                    SoFilter = model.SoFilter,
                    OrderOrder = order,

                };
            }

            return View(model);
        }

        public ActionResult AddComplaintItem(int CommentID, string ComplaintItem, string FirstTousuType)
        {
            bool result = false;
            int flag = 0;
            var userName = HttpContext.User.Identity.Name;
            if (ComplaintItem != null)
            {
                GungnirDataContext dc = new GungnirDataContext();
                tbl_CommentTousu commentTousuItem = dc.tbl_CommentTousu.SingleOrDefault(m => m.CommentId.Equals(CommentID));
                var beforeValue = "";
                if (commentTousuItem == null)
                {
                    tbl_CommentTousu model = new tbl_CommentTousu();
                    model.CommentId = Convert.ToInt32(CommentID);
                    model.ComplaintContent = ComplaintItem;
                    model.FirstTousuType = FirstTousuType;
                    dc.tbl_CommentTousu.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    result = true;
                    flag = 1;
                }
                else
                {
                    beforeValue = commentTousuItem.ComplaintContent;
                    commentTousuItem.ComplaintContent = ComplaintItem;
                    commentTousuItem.FirstTousuType = FirstTousuType;
                    dc.SubmitChanges();
                    result = true;
                    flag = 2;
                }

                if (result == true && flag == 1)
                {
                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = CommentID,
                        ObjectType = "Comment",
                        BeforeValue = "New",
                        AfterValue = ComplaintItem,
                        Author = userName,
                        Operation = "创建投诉类型"
                    };
                    new OprLogManager().AddOprLog(oprLog);
                }
                else if (result == true && flag == 2)
                {
                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = CommentID,
                        ObjectType = "Comment",
                        BeforeValue = beforeValue,
                        AfterValue = ComplaintItem,
                        Author = userName,
                        Operation = "修改投诉类型"
                    };
                    new OprLogManager().AddOprLog(oprLog);
                }

            }

            return Json(result);
        }

        public ActionResult GetAllTousuTypeItems()
        {
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            List<TousuTypeItem> result = new List<TousuTypeItem>();

            result = dc.TousuType.Select(view => new TousuTypeItem()
            {
                DicType = view.DicType,
                DicText = view.DicText,
                DicValue = view.DicValue,
                TypeLevel = view.TypeLevel,
                IsDelete = view.IsDelete ?? false
            }).Where(x => x.IsDelete != true).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteComplaintItem(int CommentID)
        {
            GungnirDataContext dc = new GungnirDataContext();
            bool result = false;
            var userName = HttpContext.User.Identity.Name;
            var beforeValue = "";
            if (CommentID != 0)
            {
                tbl_CommentTousu model = dc.tbl_CommentTousu.SingleOrDefault(m => m.CommentId.Equals(CommentID));
                beforeValue = model.ComplaintContent;
                dc.tbl_CommentTousu.DeleteOnSubmit(model);
                dc.SubmitChanges();
                result = true;
            }
            if (result == true)
            {
                var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                {
                    ObjectID = CommentID,
                    ObjectType = "Comment",
                    BeforeValue = beforeValue,
                    AfterValue = "Delete",
                    Author = userName,
                    Operation = "删除投诉类型"
                };
                new OprLogManager().AddOprLog(oprLog);
            }

            return Json(result);
        }


        public ActionResult SelectOprLogByCommentID(int commentID)
        {
            var result = LoggerManager.SelectOprLogByParams("Comment", commentID.ToString());
            return result.Count() > 0 && result != null
                ? Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet)
                : Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CommentReport(FormCollection collection)
        {
            CommentReportModel model = new CommentReportModel();
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrEmpty(Request.QueryString["startDate"]))
            {
                startDate = Convert.ToDateTime(Request.QueryString["startDate"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["endDate"]))
            {
                endDate = Convert.ToDateTime(Request.QueryString["endDate"]);
            }

            ViewBag.StartDate = startDate == null ? string.Empty : startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate == null ? string.Empty : endDate.Value.ToString("yyyy-MM-dd");

            SortedDictionary<string, int> report = new SortedDictionary<string, int>();
            var tousuTypes = CommentReportManager.GetTousuType(1).ToList();
            List<CommentReportItem> reportItems = CommentReportManager.GetCommentCount(startDate, endDate).ToList();


            foreach (var tousuType in tousuTypes)
            {
                if (!report.ContainsKey(tousuType.DicText))
                {
                    report.Add(tousuType.DicText, 0);
                }
                foreach (var item in reportItems)
                {
                    if (!string.IsNullOrEmpty(item.DicText) && tousuType.DicText.Equals(item.DicText))
                    {
                        report[tousuType.DicText] += item.Count;
                    }
                }
            }
            model.TousuReportItems = report;
            model.NotPassCount = reportItems.Where(p => p.CommentStatus.Equals(0) || p.CommentStatus.Equals(5)).Sum(t => t.Count);
            model.NotVerifiedCount = reportItems.Where(p => p.CommentStatus.Equals(1) || p.CommentStatus.Equals(4)).Sum(t => t.Count);
            model.PassCount = reportItems.Where(p => p.CommentStatus.Equals(2)).Sum(t => t.Count);
            model.OnlyRankedCount = reportItems.Where(p => p.CommentStatus.Equals(3)).Sum(t => t.Count);
            model.WebCount = reportItems.Where(p => p.CommentChannel.Equals(0)).Sum(t => t.Count);
            model.PhoneCount = reportItems.Where(p => p.CommentChannel.Equals(1)).Sum(t => t.Count);

            return View(model);
        }
        /// <summary>
        /// 根据评论时间导出数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ExcelExport(DateTime startDate,DateTime endDate)
        {
            if(startDate==DateTime.MinValue)
                startDate = DateTime.Now.Date.AddDays(-1);
            if (endDate == DateTime.MinValue)
                endDate = DateTime.Now.Date;

            var dtComment = CommentReportManager.GetCommentByCreateTime(startDate, endDate);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            var fileName = "产品评论" + DateTime.Now.ToString("yyyy_MM_dd_HHmm") + ".xls";
            row1.CreateCell(0).SetCellValue("评论来源");
            row1.CreateCell(1).SetCellValue("用户名称");
            row1.CreateCell(2).SetCellValue("评论内容");
            row1.CreateCell(3).SetCellValue("审核状态");
            row1.CreateCell(4).SetCellValue("评论的产品ID");
            row1.CreateCell(5).SetCellValue("评论的产品名称");
            row1.CreateCell(6).SetCellValue("订单ID");
            row1.CreateCell(7).SetCellValue("评论的门店");
            row1.CreateCell(8).SetCellValue("创建时间");
            row1.CreateCell(9).SetCellValue("R1");
            row1.CreateCell(10).SetCellValue("R2");
            row1.CreateCell(11).SetCellValue("R3");
            row1.CreateCell(12).SetCellValue("R4");
            row1.CreateCell(13).SetCellValue("R5");
            row1.CreateCell(14).SetCellValue("R6");
            row1.CreateCell(15).SetCellValue("R7");
            row1.CreateCell(16).SetCellValue("投诉类型");

            if (dtComment != null && dtComment.Rows.Count>0)
            {
                var rowNum = 1;
                foreach (DataRow item in dtComment.Rows)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(rowNum++);
                    rowtemp.CreateCell(0).SetCellValue(item["评论来源"].ToString());
                    rowtemp.CreateCell(1).SetCellValue(item["用户名称"].ToString());
                    rowtemp.CreateCell(2).SetCellValue(item["评论内容"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(item["审核状态"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(item["评论的产品ID"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(item["评论的产品名称"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(item["订单ID"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(item["评论的门店"].ToString());
                    rowtemp.CreateCell(8).SetCellValue(item["创建时间"].ToString());
                    rowtemp.CreateCell(9).SetCellValue(item["R1"].ToString());
                    rowtemp.CreateCell(10).SetCellValue(item["R2"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(item["R3"].ToString());
                    rowtemp.CreateCell(12).SetCellValue(item["R4"].ToString());
                    rowtemp.CreateCell(13).SetCellValue(item["R5"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(item["R6"].ToString());
                    rowtemp.CreateCell(15).SetCellValue(item["R7"].ToString());
                    rowtemp.CreateCell(16).SetCellValue(item["投诉类型"].ToString());
                }
            }
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }
    }
}