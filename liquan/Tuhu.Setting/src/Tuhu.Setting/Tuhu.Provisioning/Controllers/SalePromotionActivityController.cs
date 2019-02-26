using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.SalePromotionActivity;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity.SalePromotionActivity;
using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.Models.SalePromotionActivity;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class SalePromotionActivityController : Controller
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("SalePromotionActivityController");
        public static SalePromotionActivityManager Manager = new SalePromotionActivityManager();

        #region 活动列表和编辑页

        /// <summary>
        /// 新增活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult InsertOrUpdateActivity(SalePromotionActivityModel model, List<VueDiscountModel> VueDiscountContentList)
        {
            bool result = false;
            if (!ValidUserInputActivity(model, out string message))
            {
                return Json(new { Status = result, Msg = message });
            }
            //utc时间转化为本地时间
            DateTime.TryParse(model.StartTime, out DateTime startTime);
            model.StartTime = startTime.ToLocalTime().ToString();
            DateTime.TryParse(model.EndTime, out DateTime endTime);
            model.EndTime = endTime.ToLocalTime().ToString();
            if (model.Is_DefaultLabel == 1)
            {
                model.Label = "";
            }
            try
            {
                switch (model.PromotionType)
                {
                    //满折活动
                    case (int)SalePromotionActivityType.FullDiscount:
                        //验证活动内容
                        var checkResult = CheckFullDiscount(VueDiscountContentList, out message);
                        if (checkResult)
                        {
                            //添加活动的内容集合
                            model.DiscountContentList = new List<SalePromotionActivityDiscount>();
                            foreach (var item in VueDiscountContentList)
                            {
                                var contentModel = new SalePromotionActivityDiscount()
                                {
                                    ActivityId = model.ActivityId,
                                    DiscountMethod = model.DiscountMethod,
                                    Condition = item.Condition,
                                    DiscountRate = item.DiscountRate
                                };
                                model.DiscountContentList.Add(contentModel);
                            }
                            if (string.IsNullOrEmpty(model.ActivityId))
                            {
                                //新增活动
                                model.ActivityId = Guid.NewGuid().ToString();
                                model.CreateUserName = User?.Identity?.Name;
                                result = Manager.InsertActivity(model, out message);
                            }
                            else
                            {
                                var repeatList = Manager.GetActivityRepeatProductList(model.ActivityId, model.StartTime, model.EndTime);
                                if (repeatList?.Count > 0)
                                {
                                    return Json(new
                                    {
                                        Status = false,
                                        ActivityId = model.ActivityId,
                                        Msg = "当前活动中商品已存在其他活动中",
                                        FailList = repeatList
                                    });
                                }
                                else
                                {
                                    model.LastUpdateUserName = User?.Identity?.Name;
                                    result = Manager.UpdateActivity(model, out message);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                message = "保存失败,请刷新重试";
                Logger.Log(Level.Error,ex, "InsertOrUpdateActivity");
            }
            return Json(new { Status = result, ActivityId = model.ActivityId, Msg = message });
        }

        private bool ValidUserInputActivity(SalePromotionActivityModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                message = "请填写活动信息";
                return false;
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                message = "请填写活动名称";
                return false;
            }
            else
            {
                int len = 0;
                foreach (var item in model.Name)
                {
                    if (Regex.IsMatch(item.ToString(), @"[\u4E00-\u9FA5]+$"))
                        len = len + 2;
                    else
                        len++;
                }
                if (len > 20)
                {
                    message = "活动名称不超过10个汉字或20个字符";
                    return false;
                }
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                message = "请填写活动描述";
                return false;
            }
            else
            {
                int len = 0;
                foreach (var item in model.Description)
                {
                    if (Regex.IsMatch(item.ToString(), @"[\u4E00-\u9FA5]+$"))
                        len = len + 2;
                    else
                        len++;
                }
                if (len > 40)
                {
                    message = "活动描述不超过20个汉字或40个字符";
                    return false;
                }
            }
            if (string.IsNullOrEmpty(model.Banner))
            {
                message = "请填写促销描述";
                return false;
            }
            else
            {
                int len = 0;
                foreach (var item in model.Banner)
                {
                    if (Regex.IsMatch(item.ToString(), @"[\u4E00-\u9FA5]+$"))
                        len = len + 2;
                    else
                        len++;
                }
                if (len > 30)
                {
                    message = "促销描述不超过15个汉字或30个字符";
                    return false;
                }
            }
            if (!DateTime.TryParse(model.StartTime, out DateTime startTime) || !DateTime.TryParse(model.EndTime, out DateTime endTime))
            {
                message = "请选择正确的时间格式";
                return false;
            }
            if (startTime < DateTime.Now)
            {
                message = "活动开始时间应大于当前时间";
                return false;
            }
            if (endTime < DateTime.Now)
            {
                message = "活动终止时间应大于当前时间";
                return false;
            }
            if (endTime <= startTime)
            {
                message = "活动终止时间应大于活动开始时间";
                return false;
            }
            if (model.Is_DefaultLabel != 1)
            {
                if (string.IsNullOrEmpty(model.Label))
                {
                    message = "请填写自定义标签";
                    return false;
                }
                int len = 0;
                foreach (var item in model.Label)
                {
                    if (Regex.IsMatch(item.ToString(), @"[\u4E00-\u9FA5]+$"))
                        len = len + 2;
                    else
                        len++;
                }
                if (len > 10)
                {
                    message = "自定义标签不超过5个汉字或10个字符";
                    return false;
                }
            }
            if (model.Is_PurchaseLimit == 1)
            {
                if (model.LimitQuantity <= 0)
                {
                    message = "请输入限购数量";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 审核后修改活动信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateActivityAfterAudit(SalePromotionActivityModel model, int auditStatus)
        {
            bool result = false;
            string message = "操作失败,请刷新重试";
            if (model == null)
            {
                return Json(new { Status = false, Msg = message });
            }
            model.AuditStatus = auditStatus;
            try
            {
                if (!DateTime.TryParse(model.StartTime, out DateTime startTime) || !DateTime.TryParse(model.EndTime, out DateTime endTime))
                {
                    return Json(new { Status = false, Msg = "请选择正确的时间格式" });
                }
                if (endTime <= startTime)
                {
                    return Json(new { Status = false, Msg = "活动终止时间应大于活动开始时间" });
                }
                var repeatList = Manager.GetActivityRepeatProductList(model.ActivityId, model.StartTime, model.EndTime);
                if (repeatList?.Count > 0)
                {
                    return Json(new { Status = false, Msg = "当前活动中商品已存在其他活动中", FailList = repeatList });
                }
                else
                {
                    //utc时间转化为本地时间
                    model.StartTime = startTime.ToLocalTime().ToString();
                    model.EndTime = endTime.ToLocalTime().ToString();

                    model.LastUpdateUserName = User.Identity.Name;
                    result = Manager.UpdateActivityAfterAudit(model);
                }
            }
            catch (Exception ex)
            {
                message = "保存失败,请刷新重试";
                Logger.Log(Level.Error, ex, "UpdateActivityAfterAudit");
            }
            return Json(new { Status = result, Msg = message });
        }

        /// <summary>
        /// 检查打折内容
        /// </summary>
        /// <param name="discountList"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool CheckFullDiscount(List<VueDiscountModel> discountList, out string message)
        {
            bool result = true;
            message = string.Empty;
            if (discountList == null || discountList.Count == 0)
            {
                result = false;
                message = "请填写打折内容";
            }
            else
            {
                //检查是否按梯度增加力度
                if (discountList.Count >= 2)
                {
                    discountList = discountList.OrderBy(p => p.Condition).ToList();
                    for (int i = 0; i < discountList.Count - 1; i++)
                    {
                        if (!(discountList[i + 1].Condition > discountList[i].Condition && discountList[i + 1].DiscountRate < discountList[i].DiscountRate))
                        {
                            result = false;
                            message = "请按梯度依次增加优惠力度";
                        }
                        else
                        {
                            if (!(discountList[i].DiscountRate > 0 && discountList[i].DiscountRate < 100))
                            {
                                result = false;
                                message = "折扣力度请在0.00至 100.00之间";
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 查询活动列表
        /// </summary>
        /// <param name="search_data"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectActivityList(SalePromotionActivityModel search_data, int pageIndex = 1, int pageSize = 20)
        {
            if (search_data == null)
            {
                search_data = new SalePromotionActivityModel() { Status = 0 };
            }
            try
            {
                var selectModel = Manager.SelectActivityList(search_data, pageIndex, pageSize);
                if (selectModel?.ActivityList != null)
                {
                    foreach (var item in selectModel.ActivityList)
                    {
                        DateTime.TryParse(item.EndTime, out DateTime endTime);
                        DateTime.TryParse(item.StartTime, out DateTime startTime);
                        if (endTime < DateTime.Now)
                        {
                            item.Status = (int)SalePromotionActivityStatus.End;
                        }
                        else
                        {
                            if (item.Is_UnShelve == 1)
                            {
                                item.Status = (int)SalePromotionActivityStatus.End;
                            }
                            else
                            {
                                switch (item.AuditStatus)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        item.Status = (int)SalePromotionActivityStatus.WaitAudit;
                                        break;
                                    case 2:
                                        if (startTime < DateTime.Now)
                                        {
                                            item.Status = (int)SalePromotionActivityStatus.Online;
                                        }
                                        else
                                        {
                                            item.Status = (int)SalePromotionActivityStatus.StayOnline;
                                        }
                                        break;
                                    case 3:
                                        item.Status = (int)SalePromotionActivityStatus.Rejected;
                                        break;
                                }
                            }
                        }
                    }
                }
                return Json(new { Status = true, List = selectModel.ActivityList, Total = selectModel.CurrentStatusCount, CountModel = selectModel.Counts });
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SelectActivityList");
                return Json(new { Status = false, Msg = "获取活动列表失败" });
            }
        }

        /// <summary>
        /// 根据活动id获取活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult GetActivityModel(string activityId)
        {
            if (string.IsNullOrWhiteSpace(activityId))
            {
                return Json(new { Status = false, Msg = "活动信息加载失败,请刷新后重试" }, JsonRequestBehavior.AllowGet);
            }
            var model = new SalePromotionActivityModel();
            try
            {
                model = Manager.GetActivityInfo(activityId);
            }
            catch (Exception ex)
            {
                model = new SalePromotionActivityModel();
                Logger.Log(Level.Error, ex, "GetActivityModel");
            }
            return Json(new { Status = true, Data = model }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  

        /// <summary>
        /// 下载 导入PID的模板
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public FileResult ExportTemplate(string type)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(0);
            #region title
            if (string.Equals(type, "pidtemplate"))
            {
                var cellNum = 0;
                row.CreateCell(cellNum++).SetCellValue("商品PID");
                row.CreateCell(cellNum++).SetCellValue("限购库存");

                cellNum = 0;
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
            }
            #endregion

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"PID导入模板.xlsx");
        }

        /// <summary>
        /// 审核促销活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult SetActivityAuditStatus(string activityId, int auditStatus, string auditRemark)
        {
            bool result = false;
            string message = "操作失败,请刷新重试";
            if (auditStatus == 2 || auditStatus == 3)
            {
                var authModel = Manager.GetUserAuditAuth(1, User.Identity.Name);
                if (!(authModel?.PKID > 0))
                {
                    return Json(new { Status = false, Msg = "您没有审核权限" });
                }
            }
            if (auditStatus == 3)
            {
                if (string.IsNullOrEmpty(auditRemark))
                {
                    return Json(new { Status = result, Msg = "请填写拒绝原因" });
                }
                else
                {
                    int len = 0;
                    foreach (var item in auditRemark)
                    {
                        if (Regex.IsMatch(item.ToString(), @"[\u4E00-\u9FA5]+$"))
                            len = len + 2;
                        else
                            len++;
                    }
                    if (len > 200)
                    {
                        message = "拒绝原因不超过100个汉字或200个字符";
                        return Json(new { Status = false, Msg = message });
                    }
                }
            }
            try
            {
                if (auditStatus == 2)
                {
                    result = Manager.PassAuditActivity(activityId, User.Identity.Name, out message);
                }
                else
                {
                    result = Manager.SetActivityAuditStatus(activityId, auditStatus, auditRemark, User.Identity.Name);
                }
            }
            catch (Exception ex)
            {
                message = "操作失败，请刷新重试";
                Logger.Log(Level.Error, ex, "SetActivityAuditStatus");
            }
            return Json(new { Status = result, Msg = message });
        }

        /// <summary>
        /// 下架促销活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult UnShelveActivity(string activityId)
        {
            bool result = false; ;
            string message = "操作失败,请刷新重试";
            try
            {
                result = Manager.UnShelveActivity(activityId, User.Identity.Name);
            }
            catch (Exception ex)
            {
                message = "操作失败，请刷新重试";
                Logger.Log(Level.Error, ex, "UnShelveActivity");
            }
            return Json(new { Status = result, Msg = message });
        }

        /// <summary>
        /// 从活动中删除商品
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult RemoveProductFromActivity(string pid, string activityId)
        {
            bool result = false;
            string message = "操作失败,请刷新重试";
            try
            {
                result = Manager.DeleteProductFromActivity(pid, activityId, User.Identity.Name, out message);
            }
            catch (Exception ex)
            {
                message = "操作失败，请刷新重试";
                Logger.Log(Level.Error, ex, "RemoveProductFromActivity");
            }
            return Json(new { Status = result, Msg = message });
        }

        #endregion

        #region 查询

        /// <summary>
        /// 活动活动信息和商品列表
        /// </summary>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public JsonResult GetActivityAllInfo(string ActivityId, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(ActivityId))
            {
                return Json(new { Status = false, Msg = "活动信息加载失败,请刷新后重试" }, JsonRequestBehavior.AllowGet);
            }
            var model = new SalePromotionActivityModel();
            try
            {
                model = Manager.GetActivityInfo(ActivityId);
                if (model != null)
                {
                    var productList = Manager.GetActivityProductInfoList(new SelectActivityProduct() { ActivityId = ActivityId }, pageIndex, pageSize)?.Source.ToList();
                    model.Products = ComputeProductMargin(productList, model.DiscountContentList);
                }
            }
            catch (Exception ex)
            {
                model = new SalePromotionActivityModel();
                Logger.Log(Level.Error, ex, "GetActivityAllInfo");
            }
            return Json(new { Status = true, Data = model }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 计算商品毛利率
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="discountList"></param>
        /// <returns></returns>
        private List<SalePromotionActivityProduct> ComputeProductMargin(List<SalePromotionActivityProduct> productList, List<SalePromotionActivityDiscount> discountList)
        {
            if (discountList?.Count > 0 && productList?.Count > 0)
            {
                string discountLabel = "件";
                if (discountList[0].DiscountMethod == (int)SalePromotionActivityDiscountMethod.FullAmount)
                {
                    discountLabel = "元";
                }
                foreach (var product in productList)
                {
                    foreach (var discount in discountList)
                    {
                        var profitAfterDis = product.SalePrice * discount.DiscountRate / 100 - product.CostPrice;//折后毛利
                        if (profitAfterDis < 0)
                        {
                            product.IsMinusProfile = true;
                        }
                        product.DiscountMargin += $"满{discount.Condition}{discountLabel}折后毛利{profitAfterDis.ToString("0.00")}|";
                        product.DiscountMarginRate += $"满{discount.Condition}{discountLabel}折后毛利率{(profitAfterDis / ((profitAfterDis + product.CostPrice) == 0 ? 1 : (profitAfterDis + product.CostPrice))).ToString("0.00")}|";
                        product.Remark += $"满{discount.Condition}{discountLabel}折后价{(product.SalePrice * discount.DiscountRate / 100).ToString("0.00")}|";
                    }
                    product.DiscountMargin = product.DiscountMargin.Substring(0, product.DiscountMargin.Length - 1);
                    product.DiscountMarginRate = product.DiscountMarginRate.Substring(0, product.DiscountMarginRate.Length - 1);
                    product.Remark = product.Remark.Substring(0, product.Remark.Length - 1);
                }
            }
            return productList;
        }

        #endregion

        #region 活动商品操作

        /// <summary>
        /// 导入活动商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public JsonResult Upload(string activityId)
        { 
            var files = Request.Files;
            if (string.IsNullOrWhiteSpace(activityId))
            {
                activityId = Request["ActivityId"];
            }
            string msg = "操作失败请刷新重试";
            var resultStatus = true;
            var successList = new List<SalePromotionActivityProduct>();
            var failList = new List<SalePromotionActivityProduct>();
            var successCount = 0;
            try
            {
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
                        //验证是否有用户在操作
                        var operatingUser = GetAddProductOperateFlag(activityId);
                        if (!string.IsNullOrWhiteSpace(operatingUser))
                        {
                            return Json(new { Status = false, Msg = $"【{operatingUser}】正在编辑当前数据，请稍候再试", });
                        }
                        var setFlagResult = SetAddProductOperateFlag(activityId);
                        if (!setFlagResult)
                        {
                            //补偿设置一次
                            var setFlagResultRepeat = SetAddProductOperateFlag(activityId);
                            if (!setFlagResultRepeat)
                            {
                                Logger.Log(Level.Warning, $"Upload,设置新增商品操作标识缓存失败，activityId：{activityId}");
                            }
                        }

                        using (var stream = file.InputStream)
                        {
                            var buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                            var sheet = workBook.GetSheetAt(0);
                            //获取有效的excel数据列表
                            var convertResult = ConvertSheetToProduct(sheet, activityId, out msg);
                            if (!string.IsNullOrWhiteSpace(msg))
                            {
                                //文件不正确
                                return Json(new { Status = false, Msg = msg });
                            }
                            else
                            {
                                successList = convertResult.Item1;
                                failList = convertResult.Item2;
                                if (successList?.Count > 0)
                                {
                                    if (Manager.InsertActivityProductList(successList, activityId, User.Identity.Name))
                                    {
                                        successCount = successList.Count;
                                        resultStatus = true;
                                    }
                                    else
                                    {
                                        resultStatus = false;
                                        msg = "添加失败,请刷新后重试";
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    msg = "请先上传文件";
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "Upload");
            }
            finally
            {
                //操作结束
                var setEntFlag = SetAddProductOperateFlag(activityId, false);
                if (!setEntFlag)
                {
                    Logger.Log(Level.Warning,$"Upload设置新增商品结束标识缓存失败activityId：{activityId}");
                }
            }
            return Json(new { Status = resultStatus, Msg = msg, FailList = failList, InsertCount = successCount });
        }

        /// <summary>
        /// 添加活动商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult AddPid(string activityId, string pid)
        {
            string message = "添加失败";
            if (string.IsNullOrEmpty(pid))
            {
                return Json(new { Status = false, Msg = "请输入商品PID" });
            }
            var result = false;
            try
            {
                //验证是否有用户在操作
                var operatingUser = GetAddProductOperateFlag(activityId);
                if (!string.IsNullOrWhiteSpace(operatingUser))
                {
                    return Json(new { Status = false, Msg = $"【{operatingUser}】正在编辑当前数据，请稍候再试", });
                }
                var setFlagResult = SetAddProductOperateFlag(activityId);
                if (!setFlagResult)
                {
                    //补偿设置一次
                    var setFlagResultRepeat = SetAddProductOperateFlag(activityId);
                    if (!setFlagResultRepeat)
                    {
                        Logger.Log(Level.Warning, $"Upload,设置新增商品操作标识缓存失败，activityId：{activityId}");
                    }
                }
                 
                //获取活动下所有商品pid
                var activityProducts = Manager.GetProductInfoList(activityId);
                if (activityProducts.Select(p => p.Pid).Contains(pid))
                {
                    return Json(new { Status = result, Msg = "商品已经存在活动中" });
                }
                //验证是否有可以新增的商品
                var productList = new List<SalePromotionActivityProduct>() { new SalePromotionActivityProduct() { Pid = pid, LimitQuantity = 0 } };
                var checkdTuple = CheckInsertProduct(activityId, productList);
                var successList = checkdTuple.Item1;
                var failList = checkdTuple.Item2;
                if (successList?.Count > 0)
                {
                    //获取商品信息
                    var pidList = successList.Select(p => p.Pid).ToList();
                    var productInfoList = Manager.GetProductPriceAndName(pidList);
                    successList = (from p in successList
                                   join info in productInfoList on p.Pid equals info.Pid
                                   select new SalePromotionActivityProduct
                                   {
                                       ActivityId = activityId,
                                       Pid = p.Pid,
                                       ProductName = info.DisplayName,
                                       TotalStock = info.IsDaiFa ? -999 : p.TotalStock,//代发商品-999表示不限制库存
                                       CostPrice = p.CostPrice,
                                       SalePrice = info.Price,
                                       LimitQuantity = p.LimitQuantity,
                                       SoldQuantity = 0
                                   }).ToList();
                    if (Manager.InsertActivityProductList(successList, activityId, User.Identity.Name))
                    {
                        result = true;
                    }
                }
                else
                {
                    if (failList?.Count > 0)
                    {
                        message = failList[0].FailMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "AddPid");
            }
            finally
            {
                //操作结束
                var setEntFlag = SetAddProductOperateFlag(activityId, false);
                if (!setEntFlag)
                {
                    Logger.Log(Level.Warning, $"AddPid设置新增商品结束标识缓存失败activityId：{activityId}");
                }
            }
            return Json(new { Status = result, Msg = message });
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
        /// excel转化并获取商品信息
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="activityId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private Tuple<List<SalePromotionActivityProduct>, List<SalePromotionActivityProduct>> ConvertSheetToProduct(ISheet sheet, string activityId, out string message)
        {
            message = string.Empty;
            var successList = new List<SalePromotionActivityProduct>();
            var failList = new List<SalePromotionActivityProduct>();
            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;
            string cell1 = titleRow.GetCell(index++)?.StringCellValue;
            string cell2 = titleRow.GetCell(index++)?.StringCellValue;
            if (string.Compare(cell1, "商品PID", true) != 0 || cell2 != "限购库存")
            {
                message = "文件与模板不一致";
            }
            else
            {
                if (sheet.LastRowNum > 300)
                {
                    message = "一次最多可导入300条数据,请分批导入";
                }
                else
                {
                    bool rowCheck = true;
                    string rowFailMessage = string.Empty;
                    for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        rowCheck = true;
                        rowFailMessage = string.Empty;
                        var row = sheet.GetRow(rowIndex);
                        if (row == null || string.IsNullOrWhiteSpace(row.GetCell(0).StringCellValue)) continue;
                        var cellIndex = row.FirstCellNum;
                        int limitstock = -1;
                        try
                        {
                            limitstock = (int)row.GetCell(1).NumericCellValue;
                        }
                        catch (Exception)
                        {
                            limitstock = -1;
                        }
                        var item = new SalePromotionActivityProduct()
                        {
                            Pid = getStringValue(row.GetCell(cellIndex++)),
                            LimitQuantity = limitstock,
                        };
                        //1.验证导入数据格式
                        if (string.IsNullOrEmpty(item.Pid) || item.LimitQuantity <= 0)
                        {
                            rowCheck = false;
                            rowFailMessage = "导入数据不正确";
                        }
                        if (rowCheck)
                        {
                            successList.Add(item);
                        }
                        else
                        {
                            item.FailMessage = rowFailMessage;
                            failList.Add(item);
                        }
                    }
                    var checkTuple = CheckInsertProduct(activityId, successList);
                    failList.AddRange(checkTuple.Item2);
                    successList = checkTuple.Item1;
                }
            }
            return Tuple.Create(successList, failList);
        }

        /// <summary>
        /// 获取可以新增的商品信息和失败的原因
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="productList">商品信息</param>
        /// <returns></returns>
        private Tuple<List<SalePromotionActivityProduct>, List<SalePromotionActivityProduct>> CheckInsertProduct(string activityId, List<SalePromotionActivityProduct> productList)
        {
            var failList = new List<SalePromotionActivityProduct>();
            var tempSuccessList = new List<SalePromotionActivityProduct>();
            var resultSuccessList = new List<SalePromotionActivityProduct>();
            var isDaiFaPids = new List<string>(); //代发商品pids
            productList = productList.Distinct().ToList();

            var pidList = productList.Select(p => p.Pid).ToList();

            //获取商品所有基本信息
            var productAllInfos = Manager.GetProductAllInfo(pidList);

            //1.验证商品信息是否存在
            if (productList?.Count > 0)
            {
                if (productAllInfos?.Count > 0)
                {
                    foreach (var item in productList)
                    {
                        var infoModel = productAllInfos.FirstOrDefault(p => p.Pid == item.Pid);
                        if (infoModel != null && !string.IsNullOrWhiteSpace(infoModel.Pid))
                        {
                            var model = new SalePromotionActivityProduct()
                            {
                                ActivityId = activityId,
                                Pid = item.Pid,
                                ProductName = infoModel.ProductName,
                                SalePrice = infoModel.Price,
                                LimitQuantity = item.LimitQuantity,
                                SoldQuantity = 0
                            };
                            tempSuccessList.Add(model);
                        }
                        else
                        {
                            failList.Add(new SalePromotionActivityProduct()
                            {
                                Pid = item.Pid,
                                FailMessage = "PID不存在"
                            });
                        }
                    }
                }
                else//商品pid全部不对
                {
                    foreach (var item in productList)
                    {
                        item.FailMessage = "PID不存在";
                        failList.Add(item);
                    }
                    return Tuple.Create(resultSuccessList, failList);
                }
            }

            //2.验证商品是否已经存在当前活动中
            if (tempSuccessList?.Count > 0)
            {
                var activityProducts = Manager.GetProductInfoList(activityId);
                if (activityProducts?.Count > 0)
                {
                    var temp = new List<SalePromotionActivityProduct>();
                    foreach (var item in tempSuccessList)
                    {
                        if ((bool)activityProducts.Select(p => p.Pid)?.Contains(item.Pid))
                        {
                            item.FailMessage = $"商品已经存在当前活动中";
                            failList.Add(item);
                        }
                        else
                        {
                            temp.Add(item);
                        }
                    }
                    tempSuccessList = temp;
                }
            }

            //3.验证商品是否和其他活动商品在时间上有重叠
            if (tempSuccessList.Count > 0)
            {
                var repeatPidList = Manager.GetRepeatProductList(activityId, productList.Select(a => a.Pid).ToList());
                if (repeatPidList?.Count > 0)
                {
                    var temp = new List<SalePromotionActivityProduct>();
                    foreach (var item in tempSuccessList)
                    {
                        var repeatModel = repeatPidList.FirstOrDefault(p => p.Pid == item.Pid);
                        if (repeatModel != null)
                        {
                            item.FailMessage = $"商品已经存在活动【{repeatModel.ActivityName}】中";
                            failList.Add(item);
                        }
                        else
                        {
                            temp.Add(item);
                        }
                    }
                    tempSuccessList = temp;
                }
            }

            // 4.验证库存/代发商品不限制库存
            if (tempSuccessList.Count > 0)
            {
                foreach (var item in tempSuccessList)
                {
                    var stockModel = productAllInfos?.FirstOrDefault(p => p.Pid == item.Pid);
                    //代发商品，暂存-999代替
                    if ((bool)stockModel.IsDaiFa)
                    {
                        item.CostPrice = stockModel?.CostPrice ?? 0;
                        item.TotalStock = -999;
                        item.SoldQuantity = 0;
                        resultSuccessList.Add(item);
                    }
                    else
                    {
                        if (stockModel == null || string.IsNullOrWhiteSpace(stockModel.Pid))
                        {
                            item.FailMessage = "未查询到商品库存";
                            failList.Add(item);
                        }
                        else
                        {
                            if (item.LimitQuantity <= stockModel.TotalStock)
                            {
                                item.CostPrice = stockModel.CostPrice;
                                item.TotalStock = stockModel.TotalStock;
                                item.SoldQuantity = 0;
                                resultSuccessList.Add(item);
                            }
                            else
                            {
                                item.FailMessage = $"限购库存超出商品总库存,总库存:{stockModel.TotalStock.ToString()}";
                                failList.Add(item);
                            }
                        }
                    }
                }
            }
            return Tuple.Create(resultSuccessList, failList);
        }

        /// <summary>
        /// 分页查询活动的商品信息
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectProductList(SelectActivityProduct condition, int pageIndex, int pageSize)
        {
            int count = 0;
            bool status = false;
            var productList = new List<SalePromotionActivityProduct>();
            if (condition == null || string.IsNullOrWhiteSpace(condition.ActivityId))
            {
                return Json(new { Status = false, Msg = "获取活动Id失败，请刷新重试" });
            }
            try
            {
                //查询活动信息
                var activity = Manager.GetActivityInfo(condition.ActivityId);
                //查询活动商品信息
                var pageModel = Manager.GetActivityProductInfoList(condition, pageIndex, pageSize);
                productList = pageModel?.Source?.ToList();
                count = (int)pageModel?.Pager?.Total;
                if (productList?.Count > 0)
                {
                    //计算毛利率
                    productList = ComputeProductMargin(productList, activity?.DiscountContentList);
                }
                status = true;
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SelectProductList");
            }
            return Json(new { Status = status, List = productList, Count = count });
        }

        /// <summary>
        /// 设置活动商品限购库存
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="stock"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public ActionResult SetProductLimitStock(string activityId, int stock, List<string> pids)
        {
            bool result = true;
            if (stock <= 0)
            {
                return Json(new { Status = false, Msg = "请输入正确的库存数量" });
            }
            int setCount = 0;
            int failCount = 0;
            string msg = "操作失败,请刷新重试";
            var pidList = new List<string>();//可以修改库存的pid
            var failList = new List<SalePromotionActivityProduct>();//失败信息列表
            try
            {
                //获取打折商品库存信息
                var products = Manager.GetProductInfoList(activityId, pids)?.ToList();
                if (products?.Count > 0)
                {
                    var validList = GetValidStockList(products, stock);
                    pidList = validList.Item1;
                    failList = validList.Item2;
                    if (pidList?.Count > 0)//有可以修改的
                    {
                        setCount = Manager.SetProductLimitStock(activityId, pidList, stock, User.Identity.Name);
                        result = true;
                        failCount = products.Count - setCount;
                    }
                }
                else//未查询到商品
                {
                    result = false;
                    msg = "未查询到活动商品信息";
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SetProductLimitStock");
            }
            return Json(new { Status = result, FailList = failList, FailCount = failCount, Count = setCount, Msg = msg });
        }

        /// <summary>
        /// 设置商品列表牛皮癣
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="imgUrl"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public ActionResult SetProductImage(string activityId, string imgUrl, List<string> pids)
        {
            bool result = false;
            int setCount = 0;
            try
            {
                setCount = Manager.SetProductImage(activityId, pids, imgUrl, User.Identity.Name);
                if (setCount > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SetProductImage");
            }
            return Json(new { Status = result, Count = setCount, Msg = "设置失败" });
        }

        /// <summary>
        /// 设置商品详情页牛皮癣
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="imgUrl"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public ActionResult SetDiscountProductDetailImg(string activityId, string imgUrl, List<string> pids)
        {
            bool result = false; 
            int setCount = 0;
            try
            {
                setCount = Manager.SetDiscountProductDetailImg(activityId, pids, imgUrl, User.Identity.Name);
                if (setCount > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SetDiscountProductDetailImg");
            }
            return Json(new { Status = result, Count = setCount, Msg = "设置失败" });
        }

        /// <summary>
        /// 获取可以设置库存的pids 和不可以设置的列表信息
        /// </summary>
        /// <param name="products"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        private Tuple<List<string>, List<SalePromotionActivityProduct>> GetValidStockList(List<SalePromotionActivityProduct> products, int stock)
        {
            var failList = new List<SalePromotionActivityProduct>();
            var pidList = new List<string>();
            // 获取商品库存和成本价，验证库存
            // var stockList = Manager.GetProductStockAndCostList(products.Select(p => p.Pid).ToList());
            foreach (var item in products)
            {
                if (stock < item.LimitQuantity)
                {
                    item.FailMessage = $"新库存不能小于原限购库存，原库存：{item.LimitQuantity}";
                    item.LimitQuantity = stock;
                    failList.Add(item);
                }
                else if (item.SoldQuantity > stock)
                {
                    item.FailMessage = $"库存小于活动商品已售库存,已售库存{item.SoldQuantity}";
                    item.LimitQuantity = stock;
                    failList.Add(item);
                }
                //打折剩余库存不能超出商品总库存
                else if (item.TotalStock < stock - item.SoldQuantity)
                {
                    if (item.TotalStock == -999)///-999表示代发商品
                    {
                        pidList.Add(item.Pid);
                    }
                    else
                    {
                        item.FailMessage = $"限购库存减去打折已售库存({item.SoldQuantity})超出商品总库存,总库存{item.TotalStock}";
                        item.LimitQuantity = stock;
                        failList.Add(item);
                    }
                }
                else
                {
                    pidList.Add(item.Pid);
                }
            }
            return Tuple.Create(pidList, failList);
        }

        /// <summary>
        /// 同步商品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult RefreshProductInfo(string activityId)
        {
            string message = "操作失败";
            bool result = false;
            try
            {
                result = Manager.RefreshProductInfo(activityId, User.Identity.Name, out message);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "RefreshProductInfo");
            }
            return Json(new { Status = result, Msg = message });
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult UploadImage(string type)
        {
            var result = false;
            var msg = "操作失败,请刷新重试";
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

        /// <summary>
        /// 获取活动pids
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult GetActivityPids(string activityId)
        {
            var result = Manager.GetProductInfoList(activityId);
            return Json(result.Select(p => p.Pid));
        }

        /// <summary>
        /// 新增和删除活动商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="stock"></param>
        /// <param name="addPids"></param>
        /// <param name="delPids"></param>
        /// <returns></returns>
        public ActionResult AddAndDelActivityProduct(string activityId, int stock, List<string> addPids, List<string> delPids)
        {
            bool result = true;
            string message = "操作失败";
            if (addPids == null && delPids == null)
            {
                return Json(new { Status = false, Msg = "无改动项" });
            } 

            var successList = new List<SalePromotionActivityProduct>();
            var failList = new List<SalePromotionActivityProduct>();
            if (addPids?.Count > 0)
            {
                try
                {

                    //验证是否有用户在操作
                    var operatingUser = GetAddProductOperateFlag(activityId);
                    if (!string.IsNullOrWhiteSpace(operatingUser))
                    {
                        return Json(new { Status = false, Msg = $"【{operatingUser}】正在编辑当前数据，请稍候再试", });
                    }
                    var setFlagResult = SetAddProductOperateFlag(activityId);
                    if (!setFlagResult)
                    {
                        //补偿设置一次
                        var setFlagResultRepeat = SetAddProductOperateFlag(activityId);
                        if (!setFlagResultRepeat)
                        {
                            Logger.Log(Level.Warning, $"Upload,设置新增商品操作标识缓存失败，activityId：{activityId}");
                        }
                    }

                    var addList = (from p in addPids
                                   select new SalePromotionActivityProduct()
                                   {
                                       Pid = p,
                                       LimitQuantity = stock
                                   }).ToList();
                    //获取可以添加的商品和失败列表
                    var checkTuple = CheckInsertProduct(activityId, addList);
                    successList = checkTuple.Item1;
                    failList = checkTuple.Item2;
                    if (successList?.Count > 0 || delPids?.Count > 0)
                    {
                        result = Manager.AddAndDelActivityProduct(activityId, stock, successList, delPids, User.Identity.Name);
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    message = "操作失败,请刷新重试";
                    Logger.Log(Level.Error, ex, "AddAndDelActivityProduct");
                }
                finally
                {
                    //操作结束
                    var setEntFlag = SetAddProductOperateFlag(activityId, false);
                    if (!setEntFlag)
                    {
                        Logger.Log(Level.Warning, $"AddPid设置新增商品结束标识缓存失败activityId：{activityId}");
                    }
                }
            }
            return Json(new { Status = result, Msg = message, FailList = failList, InsertCount = successList?.Count, DeleteCount = delPids?.Count });
        }

        #endregion

        #region 活动商品查询

        /// <summary>
        /// 验证pid是否可以添加到活动中
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult ValidAddPid(string activityId, string pid)
        {
            //1.验证是否存在当前活动中
            var activityProducts = Manager.GetProductInfoList(activityId);
            if ((bool)activityProducts?.Select(p => p.Pid)?.Contains(pid))
            {
                return Json(new { Status = false, Msg = "已存在当前活动中" });
            }

            //2.验证是否存在其他活动中
            var repeatPidList = Manager.GetRepeatProductList(activityId, new List<string>() { pid });
            var repeatModel = repeatPidList?.FirstOrDefault(p => p.Pid == pid);
            if (repeatModel != null)
            {
                return Json(new { Status = false, Msg = $"已存在活动{repeatModel.ActivityName}中" });
            }

            //3.验证商品是否存在,验证库存
            var productInfo = Manager.GetProductAllInfo(new List<string>() { pid })?.FirstOrDefault();
            if (productInfo == null || string.IsNullOrWhiteSpace(productInfo.Pid))
            {
                return Json(new { Status = false, Msg = $"Pid不存在" });
            }

            //验证商品库存
            if ((bool)!productInfo.IsDaiFa && productInfo.TotalStock == 0)
            {
                return Json(new { Status = false, Msg = $"商品库存不足" });
            }

            return Json(new { Status = true });
        }

        #endregion

        #region 导出结果
        /// <summary>
        /// PID导入失败结果导出
        /// </summary>
        /// <param name="failList"></param>
        /// <returns></returns>
        public FileResult ExportImportFailResult(List<SalePromotionActivityProduct> failList)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(0);
            if (failList?.Count > 0)
            {
                var cellNum = 0;
                row.CreateCell(cellNum++).SetCellValue("商品PID");
                row.CreateCell(cellNum++).SetCellValue("商品名称");
                row.CreateCell(cellNum++).SetCellValue("导入限购库存");
                row.CreateCell(cellNum++).SetCellValue("失败原因");

                cellNum = 0;
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);

                for (var i = 0; i < failList.Count(); i++)
                {
                    cellNum = 0;
                    NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(i + 1);
                    rowtemp.CreateCell(cellNum++).SetCellValue(failList[i].Pid);
                    rowtemp.CreateCell(cellNum++).SetCellValue(failList[i].ProductName);
                    rowtemp.CreateCell(cellNum++).SetCellValue(failList[i].LimitQuantity);
                    rowtemp.CreateCell(cellNum++).SetCellValue(failList[i].FailMessage);
                }
                var ms = new MemoryStream();
                workbook.Write(ms);
                return File(ms.ToArray(), "application/x-xls", $"PID导入失败结果.xlsx");
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 审核权限

        /// <summary>
        /// 添加审核权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult InsertAuditAuth(SalePromotionAuditAuth model)
        {
            //验证
            if (model.PromotionType <= 0)
            {
                return Json(new { Status = false, Msg = "请选择促销类型" });
            }
            if (string.IsNullOrWhiteSpace(model.RoleType))
            {
                return Json(new { Status = false, Msg = "请选择角色" });
            }
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                return Json(new { Status = false, Msg = "请输入用户邮箱" });
            }
            if (!ValidOperateAuditAuth())
            {
                return Json(new { Status = false, Msg = "您没有权限操作促销活动审核权限" });
            }
            model.CreateUserName = User.Identity.Name;
            string message = "";
            var repeatValid = Manager.GetUserAuditAuth(model.PromotionType, model.UserName);
            if (repeatValid?.PKID > 0)
            {
                return Json(new { Status = false, Msg = "该用户已拥有该类型活动审核权限" });
            }
            model.AuthId = Guid.NewGuid().ToString();
            var resultCount = Manager.InsertAuditAuth(model, out message);
            if (string.IsNullOrWhiteSpace(message) && resultCount > 0)
            {
                return Json(new { Status = true });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "添加审核权限失败";
                }
                return Json(new { Status = false, Msg = message });
            }
        }

        /// <summary>
        /// 删除审核权限
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public ActionResult DeleteAuditAuth(int PKID)
        {
            if (!ValidOperateAuditAuth())
            {
                return Json(new { Status = false, Msg = "您没有权限操作促销活动审核权限" });
            }
            var resultCount = Manager.DeleteAuditAuth(PKID, User.Identity.Name);
            if (resultCount > 0)
            {
                return Json(new { Status = true });
            }
            else
            {
                return Json(new { Status = false, Msg = "删除失败,请刷新重试" });
            }
        }

        /// <summary>
        /// 查询审核权限列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectAuditAuthList(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize)
        {
            var result = Manager.SelectAuditAuthList(searchModel, pageIndex, pageSize);
            var source = new List<SalePromotionAuditAuth>();
            var pager = new PagerModel();
            if (result?.Pager != null)
            {
                pager = result.Pager;
            }
            if (result?.Source != null)
            {
                source = result.Source.ToList();
            }
            return Json(new { Status = true, List = source, Total = pager.Total });
        }

        /// <summary>
        /// 判断用户是否可以操作审核权限
        /// </summary>
        /// <returns></returns>
        private bool ValidOperateAuditAuth()
        {
            var authList = new List<string>()
            {
                "zhanglingjia@tuhu.cn",
                "dongjing1@tuhu.cn",
                "lixianpan@tuhu.cn",
                "devteam@tuhu.work"
            };
            string userName = this.User.Identity.Name;
            if (authList.Contains(userName))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 其他接口数据

        /// <summary>
        /// 根据业务业务线获取商品类目
        /// </summary>
        /// <param name="productline"></param>
        /// <returns></returns>
        public ActionResult GetCategoryByProductline(string productline)
        {
            bool outResult = false;
            var cateList = new List<RescueSelectModel>();
            try
            {
                cateList = Manager.GetCatrgoryList(productline, out outResult);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetCategoryByProductline");
                return Json(new { Status = false, Msg = "获取商品类目失败" });
            }
            if (outResult)
            {
                return Json(new { Status = true, CategotyList = cateList });
            }
            else
            {
                return Json(new { Status = false, Msg = "获取商品类目失败" });
            }
        }

        /// <summary>
        /// 根据类目获取品牌列表
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public ActionResult GetBrandsByCatgory(List<string> categoryList)
        {
            var category = string.Empty;
            bool outResult = false;
            var brandList = new List<string>();
            if (categoryList?.Count > 0)
            {
                category = categoryList.LastOrDefault();
            }
            else
            {
                return Json(new { Status = false, Msg = "请先选择商品类目" });
            }
            try
            {
                brandList = Manager.GetBrandsByCategory(category, out outResult);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetBrandsByCatgory");
                return Json(new { Status = false, Msg = "获取品牌列表失败" });
            }
            if (outResult)
            {
                return Json(new { Status = true, BrandList = brandList });
            }
            else
            {
                return Json(new { Status = false, Msg = "获取品牌列表失败" });
            }
        }

        /// <summary>
        /// 搜索商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="productline"></param>
        /// <param name="categoryList"></param>
        /// <param name="brandList"></param>
        /// <param name="sizeList"></param>
        /// <param name="patternList"></param>
        /// <returns></returns>
        public ActionResult SearchProduct(string activityId, int pageIndex, int pageSize, string category, List<string> brandList, List<string> sizeList, List<string> patternList)
        {
            try
            {
                var pageResult = Manager.SearchProduct(pageIndex, pageSize, category, brandList, sizeList, patternList);
                if (pageResult == null)
                {
                    return Json(new { Status = false, Msg = "未查询到商品" });
                }
                else
                {
                    //获取商品信息
                    var productList = GetProductInfoByPids(pageResult.Source.ToList());

                    //获取打折内容
                    var contentList = Manager.GetActivityContent(activityId);

                    //计算折扣率
                    productList = ComputeProductMargin(productList, contentList);
                    return Json(new
                    {
                        Status = true,
                        Total = pageResult?.Pager?.Total,
                        Current = pageResult?.Pager?.CurrentPage,
                        ProductList = productList
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SearchProduct");
                return Json(new { Status = false, Msg = "查询失败,请刷新重试" });
            }
        }

        /// <summary>
        /// 获取尺寸和花纹列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSizeAndPatternList(List<string> brandList, List<string> category)
        {
            if (brandList?.Count > 0 && category?.Count > 0)
            {
                string categotyStr = category.LastOrDefault();
                var sizeList = new List<string>();
                var result = Manager.GetSizeAndPatternList(brandList, categotyStr, out sizeList);
                return Json(Tuple.Create(sizeList, result));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据pids获取获取商品所有展示信息
        /// </summary>
        /// <param name="pids"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private List<SalePromotionActivityProduct> GetProductInfoByPids(List<string> pids)
        {
            var resultList = new List<SalePromotionActivityProduct>();

            //获取商品基本信息
            var productInfos = Manager.GetProductAllInfo(pids)?.Select(x => new SalePromotionActivityProduct()
            {
                Pid = x.Pid,
                ProductName = x.ProductName,
                SalePrice = x.Price,
                CostPrice = (decimal)x?.CostPrice,
                TotalStock = (int)x?.TotalStock,
            })?.ToList();

            return productInfos ?? new List<SalePromotionActivityProduct>();
        }

        #endregion

        #region 操作日志
        /// <summary>
        /// 获取活动的操作日志列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult GetOperationLogList(string activityId, int pageIndex = 1, int pageSize = 20)
        {
            var result = new PagedModel<SalePromotionActivityLogModel>();
            try
            {
                result = Manager.GetOperationLogList(activityId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetOperationLogList");
            }
            return Json(new { Status = true, List = result?.Source, Total = result?.Pager.Total });
        }

        public ActionResult GetOperationLogDetailList(string FPKID)
        {
            var result = new List<SalePromotionActivityLogDetail>();
            result = Manager.GetOperationLogDetailList(FPKID);
            return Json(new { Status = true, List = result });
        }

        #endregion

        #region 需求变动暂时用不上

        /// <summary>
        /// 获取所有渠道列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetChannelList()
        {
            var channelList = Manager.GetChannelList();
            return Json(channelList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChannelValueList(List<string> channelKeyList)
        {
            var channelList = Manager.GetChannelValueByKey(channelKeyList)?.Select(c => c.ChannelValue);
            return Json(channelList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取添加商品操作记录
        /// </summary>
        /// <returns></returns>
        private string GetAddProductOperateFlag(string activityId)
        {
            var result = string.Empty;
            try
            {
                using (var client = CacheHelper.CreateCacheClient("SalePromotionDiscountAddProduct"))
                {
                    var clientResult = client.Get($"OperateFlag/{activityId}");
                    if (clientResult.Success)
                    {
                        result = clientResult.Value; 
                    }
                    else
                    {
                        result = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error,$"GetAddProductOperateFlag,ex:{ex}"); 
            }
            return result;
        }

        /// <summary>
        /// 设置新增商品缓存标识
        /// </summary>
        /// <param name="isStart">true开始标识，false结束标识</param>
        /// <returns></returns>
        private bool SetAddProductOperateFlag(string activityId, bool isStart = true)
        {
            var result = false;
            var userName = this.User.Identity.Name;
            using (var client = CacheHelper.CreateCacheClient("SalePromotionDiscountAddProduct"))
            {
                if (isStart)
                {
                    result = client.Set($"OperateFlag/{activityId}", userName,TimeSpan.FromMinutes(5)).Success;
                }
                else
                {
                    var flag = GetAddProductOperateFlag(activityId);
                    if (!string.IsNullOrWhiteSpace(flag))
                    {
                        result = client.Remove($"OperateFlag/{activityId}").Success;
                    }
                    else
                    {
                        result = true;
                    }
                }
                if (!result)
                {
                    Logger.Log(Level.Warning, $"SetProductOperateFlag设置操作标识缓存失败isStart:{isStart}");
                }
            }
            return result;
        }

        #endregion

    }

}