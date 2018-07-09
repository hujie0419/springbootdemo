using Common.Logging;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
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
using Tuhu.Provisioning.Business.GroupBuyingV2;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity;
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
    public class GroupBuyingV2Controller : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger<GroupBuyingV2Controller>();

        protected int pageIndex
        {
            get
            {
                if (!int.TryParse(Request["pageIndex"], out var index))
                    index = 1;
                return index;
            }
        }

        protected int pageSize
        {
            get
            {
                if (!int.TryParse(Request["pageSize"], out var size))
                    size = 10;
                return size;
            }
        }

        /// <summary>
        /// 1=创建人，2=上下架时间，3=商品名称，4=商品PID
        /// </summary>
        protected int searchType
        {
            get
            {
                int searchType = 0;
                if (!int.TryParse(Request["searchType"], out searchType))
                    searchType = 0;
                return searchType;
            }
        }

        protected string keyWord
        {
            get { return Request["keyWord"]; }
        }

        //int groupCategory, int groupType, string groupLabel, int isFinishGroup
        protected int groupCategory
        {
            get
            {
                int groupCategory = 0;
                if (!int.TryParse(Request["groupCategory"], out groupCategory))
                    groupCategory = -1;
                return groupCategory;
            }
        }

        protected int groupType
        {
            get
            {
                int groupType = 0;
                if (!int.TryParse(Request["groupType"], out groupType))
                    groupType = -1;
                return groupType;
            }
        }

        protected int isFinishGroup
        {
            get
            {
                int isFinishGroup = 0;
                if (!int.TryParse(Request["isFinishGroup"], out isFinishGroup))
                    isFinishGroup = -1;
                return isFinishGroup;
            }
        }

        protected string groupLabel
        {
            get
            {
                return Request["groupLabel"]?.ToString();
            }
        }

        protected string productGroupId
        {
            get
            {
                var temp = $"PT{DateTime.Now.GetHashCode()}".Substring(0, 10);
                var result = GroupBuyingV2Manager.CheckIsExistProductGroupId(temp);
                if (result)
                    temp = $"PT{DateTime.Now.GetHashCode()}".Substring(0, 10);
                return temp;
            }
        }

        [PowerManage]
        // GET: GroupBuyingV2
        public async Task<ActionResult> Index()
        {
            var count = await GroupBuyingV2Manager.SelectGroupBuyingSettingCount(keyWord, searchType, groupCategory, groupType, groupLabel, isFinishGroup);
            string pages = GetPages(pageIndex, (int)Math.Ceiling(count * 1.0 / pageSize));
            if (count <= 0)
                return View(Tuple.Create(new List<GroupBuyingListModel>(), pages));
            var temp = await GroupBuyingV2Manager.SelectGroupBuyingSettingByPage(pageIndex, pageSize, keyWord,
                searchType, groupCategory, groupType, groupLabel, isFinishGroup);
            var result = temp.Rows.OfType<DataRow>().Select(s => new GroupBuyingListModel
            {
                BeginTime = s.GetValue<DateTime>("BeginTime"),
                EndTime = s.GetValue<DateTime>("EndTime"),
                Creator = s.GetValue<string>("Creator"),
                TotalGroupCount = s.GetValue<int>("TotalGroupCount"),
                CurrentGroupCount = s.GetValue<int>("CurrentGroupCount"),
                FinalPrice = s.GetValue<decimal>("FinalPrice"),
                GroupType = s.GetValue<int>("GroupType"),
                Image = s.GetValue<string>("Image"),
                Label = s.GetValue<string>("Label"),
                PID = s.GetValue<string>("PID"),
                ProductGroupId = s.GetValue<string>("ProductGroupId"),
                ProductName = s.GetValue<string>("ProductName"),
                Sequence = s.GetValue<int>("Sequence"),
                IsShow = s.GetValue<bool>("IsShow")
            }).ToList();

            //增加拼团产品库存信息
            var stockInfo = GroupBuyingV2Manager.GetGroupBuyingStockInfo(result.Select(g => g.PID).ToList());
            foreach (var item in result)
            {
                item.GroupStockInfo = stockInfo.FirstOrDefault(g => g.PID == item.PID);
            }

            return View(Tuple.Create(result, pages));
        }

        private async Task<List<GroupProductDetail>> GetGroupBuyingProductDetail(string groupId)
        {
            var result = new List<GroupProductDetail>();
            var temp = await GroupBuyingV2Manager.GetProductsByGroupBuyingId(groupId);
            if (temp.Rows.Count > 0)
                result = temp.Rows.OfType<DataRow>().Select(s => new GroupProductDetail
                {
                    DisPlay = s.GetValue<bool>("DisPlay"),
                    ProductName = s.GetValue<string>("ProductName"),
                    ProductGroupId = s.GetValue<string>("ProductGroupId"),
                    FinalPrice = s.GetValue<decimal>("FinalPrice"),
                    OriginalPrice = s.GetValue<decimal>("OriginalPrice"),
                    PID = s.GetValue<string>("PID"),
                    SpecialPrice = s.GetValue<decimal>("SpecialPrice"),
                    UseCoupon = s.GetValue<bool>("UseCoupon"),
                    UpperLimitPerOrder = s.GetValue<int>("UpperLimitPerOrder"),
                    IsShow = s.GetValue<bool>("IsShow"),
                    BuyLimitCount = s.GetValue<int>("BuyLimitCount")
                }).ToList();

            return (result);
        }

        public async Task<ActionResult> DeleteGroupBuyingSetting(string groupId)
        {
            var result = await GroupBuyingV2Manager.DeleteProductsByGroupBuyingId(groupId);
            if (result)
                await GroupBuyingV2Manager.InsertGroupBuyingModifyLog(groupId, ThreadIdentity.Operator.Name, "DELETE");
            ActivityService.RefrestPTCache(groupId);
            return Json(result);
        }

        [ValidateInput(false)]
        public async Task<ActionResult> SaveGroupBuyingSetting(string groupSettingJson, string productSettingJson)
        {
            var groupSetting = JsonConvert.DeserializeObject<GroupBuyingListModel>(groupSettingJson);
            var productSetting = JsonConvert.DeserializeObject<List<GroupProductDetail>>(productSettingJson);
            if (productSetting.Any(_ => string.IsNullOrWhiteSpace(_.PID)))
            {
                return Json(Tuple.Create(false, "商品PID不能为空"));
            }

            if (productSetting.Count(x => x.IsShow) <= 0)
            {
                return Json(Tuple.Create(false, "一个团至少显示一个商品"));
            }

            var groupSettingEntity = new GroupBuyingProductGroupConfigEntity
            {
                BeginTime = groupSetting.BeginTime,
                EndTime = groupSetting.EndTime,
                Creator = User.Identity.Name,
                CurrentGroupCount = groupSetting.CurrentGroupCount,
                GroupType = groupSetting.GroupType,
                Image = groupSetting.Image,
                Label = groupSetting.Label,
                MemberCount = groupSetting.MemberCount,
                Sequence = groupSetting.Sequence,
                TotalGroupCount = groupSetting.TotalGroupCount,
                ProductGroupId = string.IsNullOrWhiteSpace(groupSetting.ProductGroupId)
                    ? productGroupId
                    : groupSetting.ProductGroupId,
                ShareId = groupSetting.ShareId,
                SpecialUserTag = groupSetting.SpecialUserTag,
                IsShow = groupSetting.IsShow,
                GroupCategory = groupSetting.GroupCategory,
                GroupDescription = groupSetting.GroupDescription,
                ApplyCoupon = groupSetting.ApplyCoupon,
                ShareImage = groupSetting.ShareImage,
                Channel = groupSetting.Channel
            };
            var productSettingEntitys = productSetting.Select(s => new GroupBuyingProductConfigEntity
            {
                Creator = User.Identity.Name,
                DisPlay = s.DisPlay,
                FinalPrice = s.FinalPrice,
                OriginalPrice = s.OriginalPrice,
                PID = s.PID,
                ProductName = s.ProductName,
                SpecialPrice = s.SpecialPrice,
                ProductGroupId = groupSettingEntity.ProductGroupId,
                UseCoupon = s.UseCoupon,
                UpperLimitPerOrder = s.UpperLimitPerOrder,
                IsShow = s.IsShow,
                BuyLimitCount = s.BuyLimitCount
            }).ToList();
            if (groupSettingEntity.EndTime <= groupSettingEntity.BeginTime)
                return Json(Tuple.Create(false, "结束时间应大于开始时间！"));

            // 判断“XU-”商品是否配置虚拟商品
            if (productSettingEntitys.Any(_ => _.PID.StartsWith("XU-")) && groupSettingEntity.GroupCategory == 2)
            {
                var pids = productSettingEntitys.Where(_ => _.PID.StartsWith("XU-")).Select(_ => _.PID).ToList();

                var existsPids = GroupBuyingV2Manager.SelectProductCouponConfig(pids);
                if (existsPids.Count < pids.Count)
                {
                    //移除存在的pid
                    pids.RemoveAll(_ => existsPids.Contains(_));
                    return Json(Tuple.Create(false, $"虚拟商品未配置，请先配置虚拟商品，待配置虚拟商品【{string.Join(",", pids)}】"));
                }

                foreach (var pid in pids)
                {
                    var product = await GetProductInfo(pid);
                    if (product == null || !product.Onsale)
                    {
                        return Json(Tuple.Create(false, $"商品【{pid}】不存在或已下架"));
                    }
                }
            }

            var result = false;
            if (string.IsNullOrWhiteSpace(groupSetting.ProductGroupId))
            {
                result = await GroupBuyingV2Manager.AddGroupBuyingSetting(groupSettingEntity, productSettingEntitys);
                if (result)
                    await GroupBuyingV2Manager.InsertGroupBuyingModifyLog(groupSettingEntity.ProductGroupId,
                        ThreadIdentity.Operator.Name, "ADD");
            }
            else
            {
                result = await GroupBuyingV2Manager.UpdateGroupBuyingSetting(groupSettingEntity);
                if (result)
                {
                    await GroupBuyingV2Manager.InsertGroupBuyingModifyLog(groupSettingEntity.ProductGroupId,
                        ThreadIdentity.Operator.Name, "UPDATE");
                    //更新产品名称
                    await Task.WhenAll(
                        productSettingEntitys.Select(GroupBuyingV2Manager.UpdateGroupBuyingProductConfig));
                }
            }

            ActivityService.RefrestPTCache(groupSettingEntity.ProductGroupId);
            return Json(Tuple.Create(result, result ? string.Empty : "服务端异常！"));
        }

        public async Task<ActionResult> GetProductsByPID(string pid, bool isLottery = false)
        {
            //ProductModel
            var result = new ProductModel[] { };
            var temp = await GroupBuyingV2Manager.GetProductsByPID(pid, isLottery);
            if (temp.Rows.Count > 0)
                result = temp.Rows.OfType<DataRow>().Select(s => new ProductModel
                {
                    DisplayName = s.GetValue<string>("DisplayName"),
                    PID = s.GetValue<string>("PID"),
                    TuhuPrice = s.GetValue<decimal>("TuhuPrice")
                }).ToArray();
            return Json(result);
        }

        public async Task<ActionResult> EditGroupBuyingSetting(string groupId, int operatorType)
        {
            var GroupSetting = await GroupBuyingV2Manager.GetGroupBuyingSettingByid(groupId);
            if (GroupSetting == null || GroupSetting.Rows.Count <= 0)
                return View(Tuple.Create(new GroupBuyingListModel(), new List<GroupProductDetail>(), 0));
            var groupTemp = GroupSetting.Rows.OfType<DataRow>().Select(s => new GroupBuyingListModel
            {
                BeginTime = s.GetValue<DateTime>("BeginTime"),
                ShareId = s.GetValue<string>("ShareId"),
                MemberCount = s.GetValue<int>("MemberCount"),
                TotalGroupCount = s.GetValue<int>("TotalGroupCount"),
                EndTime = s.GetValue<DateTime>("EndTime"),
                Creator = s.GetValue<string>("Creator"),
                CurrentGroupCount = s.GetValue<int>("CurrentGroupCount"),
                FinalPrice = s.GetValue<decimal>("FinalPrice"),
                GroupType = s.GetValue<int>("GroupType"),
                Image = s.GetValue<string>("Image"),
                Label = s.GetValue<string>("Label"),
                PID = s.GetValue<string>("PID"),
                ProductGroupId = s.GetValue<string>("ProductGroupId"),
                ProductName = s.GetValue<string>("ProductName"),
                Sequence = s.GetValue<int>("Sequence"),
                SpecialUserTag = s.GetValue<int>("SpecialUser"),
                IsShow = s.GetValue<bool>("IsShow"),
                GroupCategory = s.GetValue<int>("GroupCategory"),
                GroupDescription = s.GetValue<string>("GroupDescription"),
                ApplyCoupon = s.GetValue<bool>("ApplyCoupon"),
                ShareImage = s.GetValue<string>("ShareImage"),
                Channel = s.GetValue<string>("Channel")
            }).FirstOrDefault();
            var productSetting = await GetGroupBuyingProductDetail(groupId);
            return View(Tuple.Create(groupTemp, productSetting, operatorType));
        }

        public async Task<ActionResult> GetGroupBuyingModifyLogByGroupId(string groupId)
        {
            //ProductModel
            var result = new GroupBuyingModifyLogModel[] { };
            var temp = await GroupBuyingV2Manager.GetGroupBuyingModifyLogByGroupId(groupId);
            if (temp.Rows.Count > 0)
                result = temp.Rows.OfType<DataRow>().Select(s => new GroupBuyingModifyLogModel
                {
                    Name = s.GetValue<string>("Name"),
                    CreateDateTime = s.GetValue<string>("CreateDateTime"),
                    Title = s.GetValue<string>("Title"),
                }).ToArray();
            return View(result);
        }

        public static string GetPages(int pageIndex, int pageCount)
        {
            var result = new StringBuilder();
            result.Append($"<a href='javascript:ToPage(1)'><首页></a>");
            for (int i = 1; i <= pageCount; i++)
            {
                if (pageIndex == i)
                    result.Append($"<a href='javascript:ToPage({i})' style='color: red'><{i}></a>");
                else
                {
                    result.Append($"<a href='javascript:ToPage({i})'><{i}></a>");
                }
            }

            pageCount = pageCount <= 0 ? 1 : pageCount;
            result.Append(
                $"<input style='width: 20px;height:10px;' id='ToPage' onkeydown='ToPage()' /><a href='javascript:ToPage({pageCount})'><尾页></a>");
            return result.ToString();
        }

        public JsonResult RefreshTaskCache()
        {
            var result = ActivityService.RefrestPTCache(string.Empty);

            return Json(result ? "刷新成功" : "刷新失败");
        }

        public FileResult ExportFile()
        {
            var pagesize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ExportSize"]);
            var labelList = new List<string> { "优惠券团", "抽奖团", "1分团", "低价团", "精品团" };
            var book = new HSSFWorkbook();
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            var fileName = $"拼团产品信息{ThreadIdentity.Operator.Name.Split('@')[0]}.xls";
            row.CreateCell(0).SetCellValue("ProductGroupId");
            row.CreateCell(1).SetCellValue("团类型");
            row.CreateCell(2).SetCellValue("运营标签");
            row.CreateCell(3).SetCellValue("上架时间");
            row.CreateCell(4).SetCellValue("下架时间");
            row.CreateCell(5).SetCellValue("首页是否展示");
            row.CreateCell(6).SetCellValue("拼团人数");
            row.CreateCell(7).SetCellValue("团库存上限");
            row.CreateCell(8).SetCellValue("已消耗图案库存");
            row.CreateCell(9).SetCellValue("拼团种类");
            row.CreateCell(10).SetCellValue("创建人");
            row.CreateCell(11).SetCellValue("商品PID");
            row.CreateCell(12).SetCellValue("商品名称");
            row.CreateCell(13).SetCellValue("商品售价");
            row.CreateCell(14).SetCellValue("商品活动价");
            row.CreateCell(15).SetCellValue("商品团长特价");
            row.CreateCell(16).SetCellValue("上海保养仓可用库存");
            row.CreateCell(17).SetCellValue("上海保养仓在途库存");
            row.CreateCell(18).SetCellValue("上海保养仓库存成本");
            row.CreateCell(19).SetCellValue("武汉保养仓可用库存");
            row.CreateCell(20).SetCellValue("武汉保养仓在途库存");
            row.CreateCell(21).SetCellValue("武汉保养仓库存成本");
            row.CreateCell(22).SetCellValue("可用库存总计");
            row.CreateCell(23).SetCellValue("在途库存总计");
            var data = GroupBuyingV2Manager.GetGroupBuyingExportInfo();
            var stock = GroupBuyingV2Manager.GetGroupBuyingStockInfo(data.Select(g => g.PID).ToList());
            var i = 1;
            foreach (var item in data)
            {
                var rowtemp = sheet1.CreateRow(i++);
                var groupType = item.GroupType == 0 ? "普通团" :
                    item.GroupType == 1 ? "新人团" : item.GroupType == 2 ? "团长特价" : "团长免单";
                var stockInfo = stock.FirstOrDefault(g => g.PID == item.PID);
                rowtemp.CreateCell(0).SetCellValue(item.ProductGroupId);
                rowtemp.CreateCell(1).SetCellValue(groupType);
                rowtemp.CreateCell(2).SetCellValue(labelList.Contains(item.Label) ? item.Label : "未选择");
                rowtemp.CreateCell(3).SetCellValue(item.BeginTime.ToString(CultureInfo.InvariantCulture));
                rowtemp.CreateCell(4).SetCellValue(item.EndTime.ToString(CultureInfo.InstalledUICulture));
                rowtemp.CreateCell(5).SetCellValue(item.IsShow);
                rowtemp.CreateCell(6).SetCellValue(item.MemberCount);
                rowtemp.CreateCell(7).SetCellValue(item.TotalGroupCount);
                rowtemp.CreateCell(8).SetCellValue(item.CurrentGroupCount);
                rowtemp.CreateCell(9).SetCellValue(item.GroupCategory == 0 ? "普通拼团" : item.GroupCategory == 1 ? "拼团抽奖" : "优惠券拼团");
                rowtemp.CreateCell(10).SetCellValue(item.Creator);
                rowtemp.CreateCell(11).SetCellValue(item.PID);
                rowtemp.CreateCell(12).SetCellValue(item.ProductName);
                rowtemp.CreateCell(13).SetCellValue(item.OriginalPrice.ToString("#.00"));
                rowtemp.CreateCell(14).SetCellValue(item.FinalPrice.ToString("#.00"));
                rowtemp.CreateCell(15).SetCellValue(item.SpecialPrice.ToString("#.00"));
                rowtemp.CreateCell(16).SetCellValue(stockInfo?.SHAvailableStockQuantity ?? 0);
                rowtemp.CreateCell(17).SetCellValue(stockInfo?.SHZaituStockQuantity ?? 0);
                rowtemp.CreateCell(18).SetCellValue((stockInfo?.SHStockCost ?? 0).ToString("#.00"));
                rowtemp.CreateCell(19).SetCellValue(stockInfo?.WHAvailableStockQuantity ?? 0);
                rowtemp.CreateCell(20).SetCellValue(stockInfo?.WHZaituStockQuantity ?? 0);
                rowtemp.CreateCell(21).SetCellValue((stockInfo?.WHStockCost ?? 0).ToString("#.00"));
                rowtemp.CreateCell(22).SetCellValue(stockInfo?.TotalAvailableStockQuantity ?? 0);
                rowtemp.CreateCell(23).SetCellValue(stockInfo?.TotalZaituStockQuantity ?? 0);
            }

            var ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        #region [拼团抽奖]

        [PowerManage]
        public ActionResult Lottery(string productGroupId)
        {
            var data = GroupBuyingV2Manager.GetLotteryGroup(productGroupId, true, 1, 10000);
            ViewBag.Filter = productGroupId;
            return View(data);
        }

        public ActionResult LotteryInfo(string productGroupId)
        {
            ViewBag.PeoductGroupId = string.IsNullOrWhiteSpace(productGroupId) ? "Tuhu" : productGroupId;
            return View();
        }

        public JsonResult GetLotteryCouponList(string productGroupId)
        {
            if (string.IsNullOrWhiteSpace(productGroupId))
            {
                return Json(new { Code = 0, Info = "参数错误" }, JsonRequestBehavior.AllowGet);
            }

            var data = GroupBuyingV2Manager.GetLotteryCoupon(productGroupId);
            return Json(new { Code = 1, Data = data }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult LotteryInfoList(string productGroupId, int orderId = 0, int lotteryResult = 0,
            int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            ViewBag.OrderId = orderId;
            ViewBag.LotteryResult = lotteryResult;
            ViewBag.ProductGroupId = productGroupId;
            var data = GroupBuyingV2Manager.LotteryInfoList(productGroupId, orderId, lotteryResult, pager);
            return PartialView(new ListModel<LotteryUserModel>() { Pager = pager, Source = data });
        }

        public JsonResult SetLotteryResult(string productGroupId, Guid userId, int orderId, int level)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || userId == Guid.Empty || orderId < 1 || level < 0)
            {
                return Json(new
                {
                    Code = 0,
                    Info = "参数无效"
                });
            }

            var result = GroupBuyingV2Manager.SetLotteryResult(productGroupId, level, userId, orderId,
                ThreadIdentity.Operator.Name);
            return Json(new
            {
                Code = result.Item1,
                Info = result.Item2
            });
        }

        public JsonResult GetCouponDetail(Guid couponId)
        {
            if (couponId == Guid.Empty)
            {
                return Json(new { Code = 0, Info = "参数错误" }, JsonRequestBehavior.AllowGet);
            }
            var result = GroupBuyingV2Manager.GetCouponDetail(couponId);
            if (result != null)
            {
                return Json(new
                {
                    Code = 1,
                    CouponDesc = result.CouponDesc,
                    CouponCondition = result.CouponCondition,
                    UsefulLife = result.UsefulLife
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Code = -1, Info = "未发现对应可用优惠券" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetOrderStatusForOne(string productGroupId, Guid userId, int orderId, int status)
        {
            if (userId == Guid.Empty || orderId < 1 || status != -1 && status != 1)
            {
                return Json(new { Code = 0, Info = "参数错误" });
            }

            var orderStatus = status == -1 ? "取消" : "完成";
            using (var client = new OrderOperationClient())
            {
                if (status == -1)
                {
                    var request = new CancelOrderRequest
                    {
                        OrderId = orderId,
                        UserID = userId,
                        Remark = "拼团抽奖",
                        FirstMenu = "拼团抽奖",
                        SecondMenu = "取消订单"
                    };
                    var result = client.CancelOrderForApp(request);
                    if (!(result.Success && result.Result.IsSuccess))
                    {
                        return Json(new { Code = 0, Info = "设置订单取消失败" });
                    }
                }
                else
                {
                    var result = client.ExecuteOrderProcess(new ExecuteOrderProcessRequest
                    {
                        OrderId = orderId,
                        CreateBy = userId.ToString("D"),
                        OrderProcessEnum = OrderProcessEnum.GeneralCompleteToHome
                    });
                    if (!result.Success)
                    {
                        return Json(new { Code = 0, Info = "设置订单完成失败" });
                    }
                }
            }

            GroupBuyingV2Manager.LotteryLog(productGroupId, "SetOrderStatus", ThreadIdentity.Operator.Name,
                $"{userId:D}/{orderId}订单==>{orderStatus}");
            return Json(new { Code = 1, Info = "操作成功" });
        }

        public JsonResult GetLotteryUserCount(string productGroupId, int tag)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || tag < -1 || tag > 2)
            {
                return Json(new { Code = 0, Info = "参数错误" });
            }

            var count = GroupBuyingV2Manager.GetLotteryUserCount(productGroupId, tag);
            return Json(new { Code = 1, Info = count });
        }

        public JsonResult AddLotteryCoupon(string productGroupId, Guid couponId)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || couponId == Guid.Empty)
            {
                return Json(new { Code = 0, Info = "参数错误" });
            }

            var result = GroupBuyingV2Manager.AddLotteryCoupon(productGroupId, couponId, ThreadIdentity.Operator.Name);
            if (result)
            {
                GroupBuyingV2Manager.LotteryLog(productGroupId, "AddCoupon", ThreadIdentity.Operator.Name,
                    $"新增优惠券{couponId:D}");
                return Json(new { Code = 1, Info = "添加成功" });
            }

            return Json(new { Code = -1, Info = "添加失败" });
        }

        public JsonResult DeleteLotteryCoupons(string productGroupId, List<Guid> couponIds)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || !couponIds.Any())
            {
                return Json(new { Code = 0, Info = "参数错误" });
            }

            var result =
                GroupBuyingV2Manager.DeleteLotteryCoupons(productGroupId, couponIds, ThreadIdentity.Operator.Name);
            if (result)
            {
                GroupBuyingV2Manager.LotteryLog(productGroupId, "DeleteCoupons", ThreadIdentity.Operator.Name,
                    $"{string.Join(";", couponIds)}");
                return Json(new { Code = 1, Info = "成功" });
            }

            return Json(new { Code = -1, Info = "删除失败" });
        }

        public ActionResult GetLotteryLog(string productGroupId, string type, int size = 100)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || string.IsNullOrWhiteSpace(type))
            {
                return Json(new { Code = 0, Info = "参数错误" }, JsonRequestBehavior.AllowGet);
            }

            var result = GroupBuyingV2Manager.GetLotteryLog(productGroupId, type, size);
            return Content(JsonConvert.SerializeObject(new { Code = 1, Data = result }), "application/json");
        }

        /// <summary>
        /// 批量取消订单
        /// </summary>
        /// <param name="productGroupId"></param>
        /// <param name="tag"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public JsonResult CancelLotteryOrder(string productGroupId, int tag, int orderId = 0)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || tag == -99 && orderId == 0)
                return Json(new { Code = 0, Info = "参数错误" });

            var tagStr = "个人";
            if (tag == -1) tagStr = "所有人";
            else if (tag == 1) tagStr = "一等奖用户";
            else if (tag == 2) tagStr = "二等奖用户";
            GroupBuyingV2Manager.LotteryLog(productGroupId, "CancelOrder", ThreadIdentity.Operator.Name,
                $"人群{tag}==>{tagStr}/订单号==>{orderId}");
            if (tag == -99)
            {
                var data = GroupBuyingV2Manager.GetUserInfo(productGroupId, orderId);
                if (data.Any())
                {
                    var mqData = GroupBuyingV2Manager.CancelConvertToMq(data);
                    TuhuNotification.SendNotification("notification.GroupBuyingLotteryCancelQueue", mqData);
                    GroupBuyingV2Manager.LotteryLog(productGroupId, "CancelMqItem", ThreadIdentity.Operator.Name,
                        $"{orderId}");
                }
            }
            else
            {
                var maxPkid = 0;
                var step = 100;
                var data = GroupBuyingV2Manager.GetUserInfoList(productGroupId, tag, maxPkid, step);
                while (data.Any())
                {
                    var mqData = GroupBuyingV2Manager.CancelConvertToMq(data);
                    TuhuNotification.SendNotification("notification.GroupBuyingLotteryCancelQueue", mqData);
                    GroupBuyingV2Manager.LotteryLog(productGroupId, "CancelMqItem", ThreadIdentity.Operator.Name,
                        $"{tag}/{maxPkid}/{step}");
                    maxPkid = data.Max(g => g.PKID);
                    data = GroupBuyingV2Manager.GetUserInfoList(productGroupId, tag, maxPkid, step);
                }
            }

            return Json(new { Code = 1, Info = "消息已触发" });
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="productGroupId"></param>
        /// <param name="tag"></param>
        /// <param name="batchId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public JsonResult PushLotteryMessage(string productGroupId, int tag, int batchId, int orderId = 0)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || batchId < 0 || tag == -99 && orderId == 0)
                return Json(new { Code = 0, Info = "参数错误" });
            var tagStr = "个人";
            if (tag == -1) tagStr = "所有人";
            else if (tag == 1) tagStr = "一等奖用户";
            else if (tag == 2) tagStr = "二等奖用户";
            GroupBuyingV2Manager.LotteryLog(productGroupId, "PushMessage", ThreadIdentity.Operator.Name,
                $"人群==>{tagStr}/模板ID==>{batchId}/订单号==>{orderId}");

            if (tag == -99)
            {
                var data = GroupBuyingV2Manager.GetUserInfo(productGroupId, orderId);
                if (data.Any())
                {
                    var mqData = GroupBuyingV2Manager.PushConvertToMq(data, batchId);
                    TuhuNotification.SendNotification("notification.GroupBuyingLotteryMessageQueue", mqData);
                    GroupBuyingV2Manager.LotteryLog(productGroupId, "PushMqItem", ThreadIdentity.Operator.Name,
                        $"{batchId}/{orderId}");
                }
            }
            else
            {
                var maxOrderId = 0;
                var step = 100;
                var data = GroupBuyingV2Manager.GetUserInfoListForPush(productGroupId, tag, maxOrderId, step);
                while (data.Any())
                {
                    var mqData = GroupBuyingV2Manager.PushConvertToMq(data, batchId);
                    TuhuNotification.SendNotification("notification.GroupBuyingLotteryMessageQueue", mqData);
                    GroupBuyingV2Manager.LotteryLog(productGroupId, "PushMqItem", ThreadIdentity.Operator.Name,
                        $"{tag}/{batchId}/{maxOrderId}/{step}");
                    maxOrderId = data.Max(g => g.OrderId);
                    data = GroupBuyingV2Manager.GetUserInfoListForPush(productGroupId, tag, maxOrderId, step);
                }
            }

            return Json(new { Code = 1, Info = "消息已触发" });
        }

        /// <summary>
        /// 发放优惠券
        /// </summary>
        /// <param name="productGroupId"></param>
        /// <param name="tag"></param>
        /// <param name="couponList"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public JsonResult SendLotteryCoupon(string productGroupId, int tag, List<Guid> couponList, int orderId = 0)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || !couponList.Any() || tag == -99 && orderId == 0)
                return Json(new { Code = 0, Info = "参数错误" });
            var tagStr = "个人";
            if (tag == -1) tagStr = "所有人";
            else if (tag == 1) tagStr = "一等奖用户";
            else if (tag == 2) tagStr = "二等奖用户";
            GroupBuyingV2Manager.LotteryLog(productGroupId, "SendCoupon", ThreadIdentity.Operator.Name,
                $"人群标签==>{tagStr}/优惠券列表==>{string.Join(";", couponList)}/用户订单号==>{orderId}");

            if (tag == -99)
            {
                var data = GroupBuyingV2Manager.GetUserInfo(productGroupId, orderId);
                if (data.Any())
                {
                    var mqData = GroupBuyingV2Manager.CouponConvertToMq(data, couponList);
                    TuhuNotification.SendNotification("notification.GroupBuyingLotteryCouponQueue", mqData);
                    GroupBuyingV2Manager.LotteryLog(productGroupId, "CouponMqItem", ThreadIdentity.Operator.Name,
                        $"{string.Join(";", couponList)}/{orderId}");
                }
            }
            else
            {
                var maxOrderId = 0;
                var step = 100;
                var data = GroupBuyingV2Manager.GetUserInfoListForPush(productGroupId, tag, maxOrderId, step, false);
                while (data.Any())
                {
                    var mqData = GroupBuyingV2Manager.CouponConvertToMq(data, couponList);
                    TuhuNotification.SendNotification("notification.GroupBuyingLotteryCouponQueue", mqData);
                    GroupBuyingV2Manager.LotteryLog(productGroupId, "CouponMqItem", ThreadIdentity.Operator.Name,
                        $"{tag}/{string.Join(";", couponList)}/{maxOrderId}/{step}");
                    maxOrderId = data.Max(g => g.OrderId);
                    data = GroupBuyingV2Manager.GetUserInfoListForPush(productGroupId, tag, maxOrderId, step, false);
                }
            }

            return Json(new { Code = 1, Info = "消息已触发" });
        }

        public FileResult DownloadGroupLotteryGuide()
        {
            var ms = new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Export/拼团抽奖活动配置手册.pdf")));
            var fileName = "拼团抽奖活动配置手册.pdf";
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf", fileName);
        }

        public ActionResult LotteryRecycleBin(string productGroupId, int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            ViewBag.Filter = productGroupId;
            pager.TotalItem = string.IsNullOrWhiteSpace(productGroupId) ? GroupBuyingV2Manager.GetLotteryGroupCount() : 1;
            var data = GroupBuyingV2Manager.GetLotteryGroup(productGroupId, false, pageIndex, pageSize);
            return View(new ListModel<LotteryGroupInfo>() { Pager = pager, Source = data });
        }

        [PowerManage]
        public ActionResult LotteryList(string groupId, int pageIndex = 1, int pageSize = 20) => Content(
            JsonConvert.SerializeObject(
                GroupBuyingV2Manager.GetLotteryGroup(groupId, true, pageIndex, pageSize),
                DefaultSerializeSetting), "application/json");

        [PowerManage]
        public ActionResult LotteryRecycleBinList(string groupId, int pageIndex = 1, int pageSize = 20)
        {
            var pagedData = new PagedDataModel<LotteryGroupInfo>
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            pagedData.TotalSize = GroupBuyingV2Manager.GetLotteryGroupCount();
            if (pagedData.TotalSize > 0)
            {
                pagedData.Data = GroupBuyingV2Manager.GetLotteryGroup(groupId, false, pageIndex, pageSize);
            }
            return Content(JsonConvert.SerializeObject(pagedData,
                DefaultSerializeSetting), "application/json");
        }

        [PowerManage]
        public ActionResult LotteryDetailList(string groupId, int orderId = 0, int lotteryResult = 0,
            int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            var pagedData = new PagedDataModel<LotteryUserModel>
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            pagedData.Data = GroupBuyingV2Manager.LotteryInfoList(groupId, orderId, lotteryResult, pager);
            pagedData.TotalSize = pager.TotalItem;

            return Content(JsonConvert.SerializeObject(pagedData, DefaultSerializeSetting), "application/json");
        }

        private static JsonSerializerSettings DefaultSerializeSetting => new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            NullValueHandling = NullValueHandling.Ignore
        };

        #endregion [拼团抽奖]

        #region 虚拟商品优惠券

        /// <summary>
        /// 虚拟商品优惠券配置页面
        /// </summary>
        /// <param name="pid">pid</param>
        /// <returns></returns>
        //[PowerManage]
        public ActionResult VirtualProductIndex()
        {
            return View();
        }

        public async Task<ActionResult> VirtualProductList(string pid = "")
        {
            try
            {
                var list = GroupBuyingV2Manager.SelectProductCouponConfig(pageIndex, pageSize, pid);
                var viewModel = await Task.WhenAll(list.Select(async _ =>
                {
                    var product = await GetProductInfo(_.PID);
                    return new VirtualProductViewModel
                    {
                        PID = _.PID,
                        CouponCount = _.CouponCount,
                        CreateTime = _.CreateDateTime,
                        UpdateTime = _.LastUpdateDateTime,
                        ProductName = product?.DisplayName,
                        ProductPrice = product?.Price ?? 0,
                        IsActive = product?.Onsale ?? false,
                    };
                }));
                return Content(JsonConvert.SerializeObject(viewModel, DefaultSerializeSetting), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(null, DefaultSerializeSetting), "application/json");
            }
        }

        public async Task<ActionResult> VirtualProductCouponConfigDetail(string pid)
        {
            var model = GroupBuyingV2Manager.FetchProductCouponConfig(pid);
            if (model == null || model.Count == 0)
            {
                return Json(new { status = 1, errMsg = "请求参数异常，不存在该PID的配置" }, JsonRequestBehavior.AllowGet);
            }
            var productInfo = await GetProductInfo(pid);
            if (null == productInfo)
            {
                return Json(new { status = 2, errMsg = "产品不存在" }, JsonRequestBehavior.AllowGet);
            }
            var viewModel = new VirtualProductViewModel
            {
                PID = pid,
                ProductName = productInfo.DisplayName,
                IsActive = productInfo.Onsale,
                ProductPrice = productInfo.Price,
                CouponCount = model.Count,
                Coupons = await GetCouponDetailAsync(model)
            };
            return Content(JsonConvert.SerializeObject(new { status = 0, data = viewModel }, DefaultSerializeSetting), "application/json");
        }

        public async Task<JsonResult> FetchProductInfo(string pid)
        {
            var result = await GetProductInfo(pid);

            return Json(result == null ? null : new { PID = pid, ProductName = result.DisplayName, ProductPrice = result.Price }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> FetchCouponInfo(string couponId)
        {
            Guid.TryParse(couponId, out var id);
            if (id == Guid.Empty)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var couponInfo = await GetCouponDetailAsync(new[] { id });
            return Json(couponInfo.Count > 0 ? couponInfo[0] : null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> SaveProductCouponConfig(string configJson, int operType)
        {
            VirtualProductCouponConfigModel[] model;
            try
            {
                model = JsonConvert.DeserializeObject<VirtualProductCouponConfigModel[]>(configJson);
            }
            catch (Exception)
            {
                return Json("请求参数异常，参数反序列化失败");
            }
            if (model == null || model.Length == 0)
            {
                return Json("请求参数异常");
            }
            var productExists = null != await GetProductInfo(model[0].PID);
            if (!productExists)
            {
                return Json("请求参数异常，商品不存在！");
            }
            var couponExists = model.Length == (await GetCouponDetailAsync(model.Select(_ => _.CouponId))).Count;
            if (!couponExists)
            {
                return Json("请求参数异常，优惠券信息有误！");
            }

            if (operType == 1)//新增时校验是否存在相同PID配置
            {
                var config = GroupBuyingV2Manager.FetchProductCouponConfig(model[0].PID);
                if ((config?.Count ?? 0) > 0)
                {
                    return Json("已存在相同 PID 的虚拟商品配置");
                }
            }
            var result = GroupBuyingV2Manager.AddOrUpdateProductCouponConfig(model);
            using (var client = new ConfigLogClient())
            {
                client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                {
                    ObjectId = model[0].PID,
                    ObjectType = "VirtualProductCouponConfig",
                    BeforeValue = "",
                    AfterValue = JsonConvert.SerializeObject(model),
                    Remark = $"{(operType == 1 ? "新增" : "更新")}虚拟商品优惠券关系配置--{model[0].PID}",
                    Creator = ThreadIdentity.Operator.Name
                }));
            }
            return Json(result > 0 ? "" : "操作失败");
        }

        [HttpPost]
        public JsonResult DeleteVirtualProductConfig(string pid)
        {
            if (string.IsNullOrWhiteSpace(pid))
            {
                return Json("请求参数异常，pid不能为空！");
            }

            var result = GroupBuyingV2Manager.DeleteVirtualProductConfig(pid);
            if (result > 0)
            {
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = pid,
                        ObjectType = "VirtualProductCouponConfig",
                        BeforeValue = pid,
                        AfterValue = "",
                        Remark = $"删除虚拟商品优惠券关系配置--{pid}",
                        Creator = ThreadIdentity.Operator.Name
                    }));
                }

                return Json("");
            }
            return Json("删除失败");
        }

        private static async Task<Service.Product.Models.ProductModel> GetProductInfo(string pid)
        {
            using (var client = new ProductClient())
            {
                var result = await client.FetchProductAsync(pid);
                result.ThrowIfException(true);
                return result.Result;
            }
        }

        private static async Task<IReadOnlyList<VirtualProductCouponViewModel>> GetCouponDetailAsync(
            ICollection<VirtualProductCouponConfigModel> models)
        {
            if (models == null || models.Count == 0)
            {
                return Enumerable.Empty<VirtualProductCouponViewModel>().ToArray();
            }

            var list = new List<VirtualProductCouponViewModel>();
            using (var client = new PromotionClient())
            {
                foreach (var configModel in models)
                {
                    var result = await client.GetCouponRuleAsync(configModel.CouponId);
                    if (result.Success && result.Result != null)
                    {
                        list.Add(new VirtualProductCouponViewModel
                        {
                            CouponId = result.Result.GetRuleGUID,
                            CouponName = result.Result.PromotionName,
                            CouponDescription = result.Result.Description,
                            CouponLeastPrice = result.Result.MinMoney,
                            CouponPrice = result.Result.Discount,
                            AvailablePeriod = result.Result.Term.HasValue ?
                                $"自领取之后{result.Result.Term}天内有效" :
                                $"从{result.Result.ValiStartDate}到{result.Result.ValiEndDate}",
                            CouponRate = configModel.CouponRate,
                            CouponValue = configModel.CouponValue,
                            PKID = configModel.PKID,
                            PID = configModel.PID
                        });
                    }
                }
            }
            return list;
        }

        private static async Task<IReadOnlyList<VirtualProductCouponViewModel>> GetCouponDetailAsync(IEnumerable<Guid> couponIds)
        {
            using (var client = new PromotionClient())
            {
                var results = await Task.WhenAll(couponIds.Select(_ =>
                    // ReSharper disable once AccessToDisposedClosure
                    client.GetCouponRuleAsync(_)));
                return results.Where(_ => _.Success && _.Result != null)
                    .Select(_ => new VirtualProductCouponViewModel
                    {
                        CouponId = _.Result.GetRuleGUID,
                        CouponName = _.Result.PromotionName,
                        CouponDescription = _.Result.Description,
                        CouponLeastPrice = _.Result.MinMoney,
                        CouponPrice = _.Result.Discount,
                        AvailablePeriod = _.Result.Term.HasValue ?
                            $"自领取之后{_.Result.Term}天内有效" :
                            $"从{_.Result.ValiStartDate}到{_.Result.ValiEndDate}"
                    }).ToArray();
            }
        }

        #endregion 虚拟商品优惠券

        #region 类目配置

        private static string ObjectType = "GBCategory";
        private static readonly GroupBuyCategoryConfigManager GetManager = new GroupBuyCategoryConfigManager();

        [PowerManage]
        public ActionResult CategoryIndex()
        {
            return View();
        }

        /// <summary>
        /// 添加&编辑类目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult AddOrEditCategory(int id = 0, int parentId = 0, int type = 0)
        {
            OperationCategoryModel ocModel = new OperationCategoryModel();
            if (id != 0)
            {
                ViewBag.Title = "修改";
                ocModel = GetManager.Select(id).FirstOrDefault();
                ocModel.Type = type;
                return View(ocModel);
            }
            else
            {
                ViewBag.Title = "新增";
                ocModel.Id = 0;
                ocModel.ParentId = parentId;
                ocModel.Type = type;
                return View(ocModel);
            }
        }

        /// <summary>
        /// 删除类目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteCategory(int id = 0)
        {
            if (id > 0)
            {
                return GetManager.Delete(id) ? 1 : 0;
            }
            return 0;
        }

        public ActionResult SaveCategory(OperationCategoryModel model, int id = 0)
        {
            if (!ModelState.IsValid)
            {
                return Content("请求参数异常");
            }
            var result = false;
            if (id == 0)
            {
                model.CreateTime = DateTime.Now;
                model.Id = id;
                result = GetManager.Insert(model);
                if (result) { LoggerManager.InsertOplog(User.Identity.Name, ObjectType, model.Id, "添加拼团类目" + model.DisplayName); }
            }
            else
            {
                model.CreateTime = DateTime.Now;
                model.Id = id;
                result = GetManager.Update(model);
                if (result) { LoggerManager.InsertOplog(User.Identity.Name, ObjectType, model.Id, "更新拼团类目" + model.DisplayName); }
            }

            return RedirectToAction("CategoryIndex");
        }

        public ActionResult TreeTableList(int type = 0)
        {
            ViewBag.TreeTableList = GetManager.Select(null, null, type);
            return View();
        }

        /// <summary>
        /// 后台类目产品列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult ProductTreeList(int id = 0, int type = 0)
        {
            ViewBag.id = id;
            ViewBag.type = type;

            var cateItems = GetManager.SelectProductCategories();
            var productList = GetManager.SelectOperationCategoryProducts(id);

            if (productList != null && productList.Count() > 0)
            {
                foreach (var pitem in productList)
                {
                    foreach (var citem in cateItems)
                    {
                        if (pitem.CorrelId.Equals(citem.id))
                        {
                            citem.open = true;
                            citem.isChecked = true;
                        }
                    }
                }
            }

            ViewBag.CategoryTagManager = JsonConvert.SerializeObject(cateItems).Replace("isChecked", "checked");
            return View();
        }

        public int SaveProductTreeList(int id, int type, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var result = false;
                var dicTreeItems = JsonConvert.DeserializeObject<List<ZTreeTagModel>>(data);
                if (dicTreeItems != null && dicTreeItems.Count > 0)
                {
                    var pmodel = new List<OperationCategoryProductsModel>();
                    for (var i = 0; i < dicTreeItems.Count; i++)
                    {
                        pmodel.Add(new OperationCategoryProductsModel
                        {
                            OId = id,
                            CorrelId = dicTreeItems[i].key,
                            CategoryCode = dicTreeItems[i].value,
                            DefinitionCode = dicTreeItems[i].userData,
                            Sort = i,
                            IsShow = 1,
                            Type = type,
                            CreateTime = DateTime.Now
                        });
                    }
                    result = GetManager.UpdateOperationCategoryProducts(id, type, pmodel);
                }
                else
                {
                    result = GetManager.UpdateOperationCategoryProducts(id, type, null);
                }
                return result ? 1 : 0;
            }
            return 0;
        }

        /// <summary>
        /// 搜索类目对应产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult SearchProductCategories(string data, int id = 0, int type = 0)
        {
            bool isSearch = false;                                                                                                      //判断是否搜索
            List<VW_ProductsModel> productCategoriesList = null;                                                                        //搜索匹配产品
            List<VW_ProductsModel> productCategoriesListForDefaultCheck = null;                                                         //默认选中保存产品
            IEnumerable<OperationCategoryProductsModel> productList = id > 0 ? GetManager.SelectOperationCategoryProducts(id) : null;   //类目关联外部产品列表
            if (!string.IsNullOrEmpty(data))
            {
                List<ZTreeTagModel> dicTreeItems = JsonConvert.DeserializeObject<List<ZTreeTagModel>>(data);
                if (dicTreeItems != null && dicTreeItems.Count > 0)
                {
                    isSearch = true;
                    productCategoriesList = GetManager.SelectProductCategories((from n in dicTreeItems select n.key.ToString()), false).ToList() ?? null;

                    if (productList != null && productList.Count() > 0)
                    {
                        productCategoriesListForDefaultCheck = GetManager.SelectProductCategories((from n in productList select n.CorrelId.ToString()), true).ToList() ?? null;
                        productCategoriesListForDefaultCheck.ForEach(x => x.isChecked = true); //默认为选中
                    }
                }
                else if (productList != null && productList.Count() > 0)
                {
                    isSearch = false;
                    productCategoriesList = GetManager.SelectProductCategories((from n in productList select n.CorrelId.ToString()), true).ToList() ?? null;
                    productCategoriesList.ForEach(x => x.isChecked = true); //默认为选中
                }

                //剔除搜索中存在的，以保存产品
                if (isSearch &&
                    productList != null && productList.Count() > 0 &&
                    productCategoriesList != null && productCategoriesList.Count() > 0)
                {
                    foreach (var plitem in productList)
                    {
                        productCategoriesList.Remove(productCategoriesList.FirstOrDefault(w => w.oid.Equals(plitem.CorrelId)));
                    }

                    if (productCategoriesListForDefaultCheck != null)
                    {
                        //将以保存产品，添加到搜索列表顶部
                        productCategoriesList.InsertRange(0, productCategoriesListForDefaultCheck);
                    }
                }
                ViewBag.JsonData = productCategoriesList;
            }
            return View();
        }

        #endregion 类目配置

        #region 新框架团购

        /// <summary>
        /// 获取团购商品信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="activityStatus"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult SelectGroupBuyingV2Config(string filterStr,
            string activityStatus, int pageIndex = 1, int pageSize = 20)
        {
            GroupBuyingV2Manager manager = new GroupBuyingV2Manager();
            var result = manager.SelectGroupBuyingV2Config(JsonConvert.DeserializeObject<GroupBuyingProductGroupConfigEntity>(filterStr),
                activityStatus, pageIndex, pageSize);
            return Json(new { data = result, totalCount = result?.FirstOrDefault()?.Total ?? 0 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过GroupId获取团购信息
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public JsonResult SelectGroupBuyingV2ConfigByGroupId(string groupId)
        {
            GroupBuyingV2Manager manager = new GroupBuyingV2Manager();
            var result = manager.SelectGroupBuyingV2ConfigByGroupId(groupId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 刷新产品
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="isLottery"></param>
        /// <returns></returns>
        public JsonResult RefreshProductConfigByGroupId(string groupId, bool isLottery)
        {
            GroupBuyingV2Manager manager = new GroupBuyingV2Manager();
            var result = manager.RefreshProductConfigByGroupId(groupId, isLottery);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加/编辑团购信息
        /// </summary>
        /// <param name="productConfig"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult UpsertGroupBuyingConfig(string productConfigJson, bool isJudge = true)
        {
            var result = false;
            var productConfig = JsonConvert.DeserializeObject<GroupBuyingProductGroupConfigEntity>(productConfigJson);
            GroupBuyingV2Manager manager = new GroupBuyingV2Manager();

            #region 条件判断

            if (productConfig == null || productConfig.GroupProductDetails == null || !productConfig.GroupProductDetails.Any()
                || productConfig.GroupProductDetails.Any(_ => string.IsNullOrWhiteSpace(_.PID)))
            {
                return Json(new { status = result, msg = "商品PID不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.BeginTime == null || productConfig.EndTime == null)
            {
                return Json(new { status = result, msg = "上架和下架时间不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.EndTime <= productConfig.BeginTime)
            {
                return Json(new { status = result, msg = "结束时间应大于开始时间" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.GroupType < 0)
            {
                return Json(new { status = result, msg = "请选择品团类型" }, JsonRequestBehavior.AllowGet);
            }
            //if (productConfig.TotalGroupCount == null || productConfig.TotalGroupCount < 0)
            //{
            //    return Json(new { status = result, msg = "请输入团库存上限" }, JsonRequestBehavior.AllowGet);
            //}
            //if (productConfig.TotalGroupCount - productConfig.CurrentGroupCount < 0)
            //{
            //    return Json(new { status = result, msg = "库存上限应大于等于已消耗库存" }, JsonRequestBehavior.AllowGet);
            //}
            if (productConfig.GroupCategory == 1 && string.IsNullOrEmpty(productConfig.GroupDescription))
            {
                return Json(new { status = result, msg = "请填写抽奖规则描述" }, JsonRequestBehavior.AllowGet);
            }
            //if (string.IsNullOrEmpty(productConfig.ShareId) || string.IsNullOrEmpty(productConfig.Image))
            //{
            //    return Json(new { status = result, msg = "分享图片和分享文案不能为空" }, JsonRequestBehavior.AllowGet);
            //}
            if (string.IsNullOrEmpty(productConfig.Channel))
            {
                return Json(new { status = result, msg = "请选择渠道" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.GroupProductDetails.Where(x => x.IsShow == true).Count() <= 0)
            {
                return Json(new { status = result, msg = "一个团至少显示一个商品" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.GroupProductDetails.Where(x => x.DisPlay == true).Count() <= 0
                || productConfig.GroupProductDetails.Where(x => x.DisPlay == true).Count() > 1)
            {
                return Json(new { status = result, msg = "一个团只能有一个默认展示商品" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.GroupProductDetails.Where(_ => _.DisPlay && !_.IsShow).Count() > 0)
            {
                return Json(new { status = result, msg = "默认显示商品不能配置为隐藏" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.GroupCategory != 0 && productConfig.GroupProductDetails.Where(_ => _.UpperLimitPerOrder > 1).Count() > 0)
            {
                return Json(new { status = result, msg = "优惠券拼团和拼团抽奖每单限购一件商品" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.GroupType == 0 &&
                productConfig.GroupProductDetails.Where(_ => _.FinalPrice != _.SpecialPrice).Count() > 0)
            {
                return Json(new { status = result, msg = "普通团的团长价应等于活动价" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.GroupProductDetails.Where(_ => _.TotalStockCount < _.CurrentSoldCount).Count() > 0)
            {
                return Json(new { status = result, msg = "配置总库存应大于等于产品已消耗库存" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.GroupProductDetails.Where(_ => _.FinalPrice == 0 || _.OriginalPrice == 0).Count() > 0)
            {
                return Json(new { status = result, msg = "商品售价及商品活动价不能为0" }, JsonRequestBehavior.AllowGet);
            }
            if (productConfig.GroupProductDetails.Any(_ => _.PID.StartsWith("XU-")))
            {
                if (productConfig.GroupCategory == 1 || productConfig.GroupCategory == 2)
                {
                    var pids = productConfig.GroupProductDetails.Where(_ => _.PID.StartsWith("XU-")).Select(_ => _.PID).ToList();

                    var existsPids = GroupBuyingV2Manager.SelectProductCouponConfig(pids);
                    if (productConfig.GroupCategory == 2 && existsPids.Count < pids.Count)
                    {
                        //移除存在的pid
                        pids.RemoveAll(_ => existsPids.Contains(_));
                        return Json(new { status = result, msg = $"虚拟商品未配置，请先配置虚拟商品，待配置虚拟商品【{string.Join(",", pids)}】" }, JsonRequestBehavior.AllowGet);
                    }

                    foreach (var pid in pids)
                    {
                        var product = ProductService.FetchProduct(pid);
                        if (product == null || !product.Onsale)
                        {
                            return Json(new { status = result, msg = $"商品【{pid}】不存在或已下架" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(new { status = result, msg = "虚拟商品只能配置抽奖团或者优惠券团" }, JsonRequestBehavior.AllowGet);
                }
            }
            //判断成本价格
            IEnumerable<CarPriceManagementResponse> carPrice = new List<CarPriceManagementResponse>();
            if (isJudge)
            {
                carPrice = PurchaseService.SelectPurchaseInfoByPID(productConfig.GroupProductDetails.Select(x => x.PID).ToList());
                carPrice = from c in carPrice
                           group c by c.PID into g
                           select g.OrderByDescending(x => x.CreatedDate)
                           .FirstOrDefault();
            }
            var priceData = (from c in carPrice
                             join p in productConfig.GroupProductDetails
                             on c.PID equals p.PID
                             select new
                             {
                                 PID = c.PID,
                                 FinalPrice = p.FinalPrice,
                                 PurchasePrice = c.PurchasePrice,
                                 ContractPrice = c.ContractPrice,
                                 OfferPurchasePrice = c.OfferPurchasePrice,
                                 OfferContractPrice = c.OfferContractPrice
                             }).ToList();
            if (priceData != null && priceData.Any())
            {
                priceData = priceData.Where(x
                    => ((x.OfferPurchasePrice > 0 && x.FinalPrice < (x.OfferPurchasePrice / 2))
                    || (x.OfferContractPrice > 0 && x.FinalPrice < (x.OfferContractPrice / 2)
                    || x.PurchasePrice > 0 && x.FinalPrice < (x.PurchasePrice / 2))
                    || (x.ContractPrice > 0 && x.FinalPrice < (x.ContractPrice / 2)))
                    ).ToList();
            }
            #endregion 条件判断
            if (isJudge && priceData != null && priceData.Any())
            {
                return Json(new { status = result, msg = "存在活动价低于成本价50%的商品", priceData = priceData }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                productConfig.Creator = User.Identity.Name;
                productConfig.TotalGroupCount = 999999999;
                var getResult = manager.UpsertGroupBuyingConfig(productConfig);
                return Json(new { status = getResult.Item1, msg = getResult.Item2 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取所有关联产品信息
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="isLottery"></param>
        /// <returns></returns>
        public ActionResult GetProductsByPIDAndIsLottery(string pid, bool isLottery = false)
        {
            var result = new GroupBuyingV2Manager().GetProductsByPIDAndIsLottery(pid.Trim(), isLottery);
            var data = new List<VW_ProductsModel>();
            if (result != null && result.Any())
            {
                var defaultProduct = result?.Where(x => String.Equals(x.PID, pid.Trim())).FirstOrDefault();
                if (defaultProduct != null)
                {
                    data.Add(defaultProduct);
                    if (result.Count > 1)
                    {
                        result.Remove(defaultProduct);
                        data.AddRange(result);
                    }
                }
                else
                {
                    data.AddRange(result);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult UploadImage(string type)
        {
            var result = false;
            var msg = string.Empty;
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

        public JsonResult GetStockInfoByPIDs(string groupId, string pid = "", string type = "GroupId", bool isLottery = false)
        {
            GroupBuyingV2Manager manager = new GroupBuyingV2Manager();
            List<GroupBuyingStockModel> result = new List<GroupBuyingStockModel>();
            List<string> pids = new List<string>();
            if (String.Equals(type, "GroupId") && !string.IsNullOrEmpty(groupId))
            {
                var data = manager.GetGroupBuyingV2ProductConfigByGroupId(groupId);
                pids = data?.Select(x => x.PID)?.ToList() ?? new List<string>();
            }
            else if (String.Equals(type, "PID") && !string.IsNullOrEmpty(pid))
            {
                var data = new GroupBuyingV2Manager().GetProductsByPIDAndIsLottery(pid.Trim(), isLottery);
                pids = data?.Select(x => x.PID)?.ToList() ?? new List<string>();
            }
            if (pids != null && pids.Any())
            {
                var stockInfo = manager.GetStockInfoByPIDs(pids);
                result = manager.ConvertStockModel(stockInfo);
            }
            return Json(result ?? new List<GroupBuyingStockModel>(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLogByGroupId(string groupId,string source= "GroupBuyingProductConfig")
        {
            var result = new GroupBuyingV2Manager().GetGroupBuyingLogByIdentityId(groupId, source);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateProductConfigIsShow(string groupId, bool isShow)
        {
            var result = false;
            GroupBuyingV2Manager manager = new GroupBuyingV2Manager();
            result = manager.UpdateProductConfigIsShow(groupId, isShow, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateProductSimpleData(string groupId, int sequence, DateTime beginTime, DateTime endTime, int totalGroupCount, int currentGroupCount)
        {
            var result = false;
            GroupBuyingV2Manager manager = new GroupBuyingV2Manager();
            if (endTime <= beginTime)
            {
                return Json(new { status = result, msg = "结束时间应大于开始时间" }, JsonRequestBehavior.AllowGet);
            }
            if (totalGroupCount < 0)
            {
                return Json(new { status = result, msg = "请输入团库存上限" }, JsonRequestBehavior.AllowGet);
            }
            if (totalGroupCount - currentGroupCount < 0)
            {
                return Json(new { status = result, msg = "库存上限应大于等于已消耗库存" }, JsonRequestBehavior.AllowGet);
            }
            result = manager.UpdateProductSimpleData(groupId, sequence, beginTime, endTime, totalGroupCount, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据条件导出
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="activityStatus"></param>
        /// <returns></returns>
        public FileResult ExportGroupBuyingCofig(string filterStr, string activityStatus)
        {
            #region Init

            var workBook = new XSSFWorkbook();
            var sheet = workBook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("GroupId");
            row.CreateCell(cellNum++).SetCellValue("团类型");
            row.CreateCell(cellNum++).SetCellValue("团种类");
            row.CreateCell(cellNum++).SetCellValue("运营标签");
            row.CreateCell(cellNum++).SetCellValue("显示渠道");
            row.CreateCell(cellNum++).SetCellValue("列表是否展示");
            row.CreateCell(cellNum++).SetCellValue("展示顺序");
            row.CreateCell(cellNum++).SetCellValue("上架时间");
            row.CreateCell(cellNum++).SetCellValue("下架时间");
            //row.CreateCell(cellNum++).SetCellValue("已消耗库存");
            //row.CreateCell(cellNum++).SetCellValue("库存上限");
            row.CreateCell(cellNum++).SetCellValue("拼团人数");
            row.CreateCell(cellNum++).SetCellValue("创建人");
            row.CreateCell(cellNum++).SetCellValue("主商品PID");
            row.CreateCell(cellNum++).SetCellValue("主商品名称");
            row.CreateCell(cellNum++).SetCellValue("主商品售价");
            row.CreateCell(cellNum++).SetCellValue("主商品活动价");
            row.CreateCell(cellNum++).SetCellValue("主商品团长价");
            row.CreateCell(cellNum++).SetCellValue("义乌可用库存");
            row.CreateCell(cellNum++).SetCellValue("义乌在途库存");
            row.CreateCell(cellNum++).SetCellValue("上海可用库存");
            row.CreateCell(cellNum++).SetCellValue("上海在途库存");
            row.CreateCell(cellNum++).SetCellValue("武汉可用库存");
            row.CreateCell(cellNum++).SetCellValue("武汉在途库存");
            row.CreateCell(cellNum++).SetCellValue("北京可用库存");
            row.CreateCell(cellNum++).SetCellValue("北京在途库存");
            row.CreateCell(cellNum++).SetCellValue("广州可用库存");
            row.CreateCell(cellNum++).SetCellValue("广州在途库存");
            row.CreateCell(cellNum++).SetCellValue("总可用库存");
            row.CreateCell(cellNum++).SetCellValue("总在途库存");

            cellNum = 0;

            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
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

            GroupBuyingV2Manager manager = new GroupBuyingV2Manager();
            var result = manager.SelectGroupBuyingV2Config(JsonConvert.DeserializeObject<GroupBuyingProductGroupConfigEntity>(filterStr), activityStatus, 1, 99999999);

            if (result != null && result.Any())
            {
                for (var i = 0; i < result.Count(); i++)
                {
                    cellNum = 0;
                    NPOI.SS.UserModel.IRow rowTemp = sheet.CreateRow(i + 1);
                    var defaultProduct = result[i].GroupProductDetails.Where(x => x.DisPlay == true).FirstOrDefault();
                    var stockInfo = manager.GetStockInfoByPIDs(new List<string> { defaultProduct.PID });
                    var stock = manager.ConvertStockModel(stockInfo);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].ProductGroupId);
                    rowTemp.CreateCell(cellNum++).SetCellValue(ConvertGroupType(result[i].GroupType));
                    rowTemp.CreateCell(cellNum++).SetCellValue(ConvertGroupCategory(result[i].GroupCategory));
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].Label);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].Channel);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].IsShow);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].Sequence ?? 0);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].BeginTime.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].EndTime.ToString());
                    //rowTemp.CreateCell(cellNum++).SetCellValue(result[i].CurrentGroupCount ?? 0);
                    //rowTemp.CreateCell(cellNum++).SetCellValue(result[i].TotalGroupCount ?? 0);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].MemberCount ?? 0);
                    rowTemp.CreateCell(cellNum++).SetCellValue(result[i].Creator );
                    rowTemp.CreateCell(cellNum++).SetCellValue(defaultProduct.PID);
                    rowTemp.CreateCell(cellNum++).SetCellValue(defaultProduct.ProductName);
                    rowTemp.CreateCell(cellNum++).SetCellValue(defaultProduct.OriginalPrice.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(defaultProduct.FinalPrice.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(defaultProduct.SpecialPrice.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.YiWuAvailableStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.YiWuZaituStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.SHAvailableStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.SHZaituStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.WHAvailableStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.WHZaituStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.BeiJingAvailableStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.BeiJingZaituStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.GuangZhouAvailableStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.GuangZhouZaituStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.TotalAvailableStockQuantity.ToString());
                    rowTemp.CreateCell(cellNum++).SetCellValue(stock?.FirstOrDefault()?.TotalZaituStockQuantity.ToString());
                }
            }

            #endregion 封装数据

            var ms = new MemoryStream();
            workBook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        #region private

        private string ConvertGroupType(int groupType)
        {
            var result = string.Empty;
            switch (groupType)
            {
                case 0:
                    result = "普通团";
                    break;

                case 1:
                    result = "新人团";
                    break;

                case 2:
                    result = "团长特价";
                    break;

                case 3:
                    result = "团长免单";
                    break;

                default:
                    break;
            };
            return result;
        }

        private string ConvertGroupCategory(int groupCategory)
        {
            var result = string.Empty;
            switch (groupCategory)
            {
                case 0:
                    result = "普通拼团";
                    break;

                case 1:
                    result = "抽奖拼团";
                    break;

                case 2:
                    result = "优惠券拼团";
                    break;

                default:
                    break;
            };
            return result;
        }

        #region 新拼团优惠券

        [PowerManage(IwSystem = "OperateSys")]
        public async Task<JsonResult> PT_GetCouponList(string pid, int pageIndex = 1, int pageSize = 20)
        {
            var list = GroupBuyingV2Manager.SelectProductCouponConfig(pageIndex, pageSize, pid);
            int totalCount = GroupBuyingV2Manager.SelectProductCouponConfigCount(pid);
            var viewModel = await Task.WhenAll(list.Select(async _ =>
            {
                var product = await GetProductInfo(_.PID);
                return new VirtualProductViewModel
                {
                    PID = _.PID,
                    CouponCount = _.CouponCount,
                    CreateTime = _.CreateDateTime,
                    UpdateTime = _.LastUpdateDateTime,
                    ProductName = product?.DisplayName,
                    ProductPrice = product?.Price ?? 0,
                    IsActive = product?.Onsale ?? false,
                };
            }));

            return Json(new { data = viewModel, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
            //return Json(new { data = viewModel, totalCount = viewModel?.FirstOrDefault()?.Total ?? 0 }, JsonRequestBehavior.AllowGet);
        }

        #endregion 新拼团优惠券

        #endregion private

        #endregion 新框架团购
    }
}