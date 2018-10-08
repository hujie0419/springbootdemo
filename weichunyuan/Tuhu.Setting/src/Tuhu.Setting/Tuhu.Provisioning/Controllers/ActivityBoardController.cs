using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.Activity;
using Tuhu.Provisioning.Business.ActivityBoard;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class ActivityBoardController : Controller
    {
        ActivityBoardPowerManager powerManager = new ActivityBoardPowerManager();

        #region 活动看板
        public ActionResult Index(ActivityBoardModuleType activityBoardType = ActivityBoardModuleType.TuhuActivity, string title = "", string createdUser = "", string owner = "",string channel="", string typeStr = "ZongHe,Tires,BaoYang,ChePin,GaiZhuang,Outside")
        {
            ViewBag.ModuleType = activityBoardType.ToString();
            ViewBag.Name = title;
            ViewBag.CreatedUser = createdUser;
            ViewBag.Channel = channel;
            ViewBag.Owner = owner;
            ViewBag.Tires = true;
            ViewBag.BaoYang = true;
            ViewBag.ChePin = true;
            ViewBag.GaiZhuang = true;
            ViewBag.Outside = true;
            ViewBag.ZongHe = true;
            if (!typeStr.Contains(ActivityType.Tires.ToString()))
            {
                ViewBag.Tires = false;
            }
            if (!typeStr.Contains(ActivityType.BaoYang.ToString()))
            {
                ViewBag.BaoYang = false;
            }
            if (!typeStr.Contains(ActivityType.ChePin.ToString()))
            {
                ViewBag.ChePin = false;
            }
            if (!typeStr.Contains(ActivityType.GaiZhuang.ToString()))
            {
                ViewBag.GaiZhuang = false;
            }
            if (!typeStr.Contains(ActivityType.Outside.ToString()))
            {
                ViewBag.Outside = false;
            }
            if (!typeStr.Contains(ActivityType.ZongHe.ToString()))
            {
                ViewBag.ZongHe = false;
            }
            ViewBag.IsViewEffect = powerManager.VerifyPermissions(HttpContext.User.Identity.Name, activityBoardType, OperationType.Effect);
            return View();
        }

        /// <summary>
        /// 获取活动看板信息
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public JsonResult GetActivityList(DateTime start, DateTime end, ActivityBoardModuleType type, string title = "", string createdUser = "", string owner = "",string typeStr="",string channel="")
        {
            var result = new List<ActivityBoardViewModel>();
            ThirdPartActivityManager manager = new ThirdPartActivityManager();
            ActivityBoardPowerManager powerManager = new ActivityBoardPowerManager();
            ActivityManager activityManager = new ActivityManager();
            var powerConfig = powerManager.GetPowerConfigByUserEmail(HttpContext.User.Identity.Name, type);
            if (powerConfig != null && powerConfig.ViewActivity)
            {
                if (powerConfig.VisibleActivityDays != 0)
                {
                    if (powerConfig.VisibleActivityDays != -1)
                    {
                        var visibleDays = (powerConfig.VisibleActivityDays % 2) == 0 ? powerConfig.VisibleActivityDays + 1 : powerConfig.VisibleActivityDays;
                        var diffDays = Math.Ceiling((visibleDays + 1) * 1.00 / 2);
                        start = Convert.ToDateTime(DateTime.Now.AddDays(-(powerConfig.VisibleActivityDays-diffDays)).ToShortDateString());
                        end = Convert.ToDateTime(DateTime.Now.AddDays(+diffDays).ToShortDateString());
                    }
                    switch (type)
                    {
                        case ActivityBoardModuleType.TuhuActivity:
                            var allActivityOne = activityManager.GetActivityDetailsForActivityBoard(start, end, title, createdUser, owner);
                            result = ConvertViewModelByType(allActivityOne, start, end, ActivityBoardModuleType.TuhuActivity, typeStr);
                            break;
                        //case ActivityBoardModuleType.OutsideActivity:
                        //    var allActivityTwo = activityManager.GetActivityDetailsForActivityBoard(start, end);
                        //    result = ConvertViewModelByType(allActivityTwo, start, end, ActivityBoardModuleType.OutsideActivity);
                        //    break;
                        case ActivityBoardModuleType.ThirdPartyActivity:
                            var thirdPartyActivity = manager.GetActivityForActivityBoard(start, end, title, owner, channel);
                            result = ConvertViewModelForThirdParty(thirdPartyActivity, start, end, typeStr);
                            break;
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CycleCountActivity(DateTime? start,string title = "", string createdUser = "", string owner = "", string typeStr = "ZongHe,Tires,BaoYang,ChePin,GaiZhuang,Outside")
        {
            ViewBag.NowTime = start == null ? DateTime.Now.ToShortDateString() : start.Value.ToShortDateString();
            ViewBag.Name = title;
            ViewBag.CreatedUser = createdUser;
            ViewBag.Owner = owner;
            ViewBag.Tires = true;
            ViewBag.BaoYang = true;
            ViewBag.ChePin = true;
            ViewBag.GaiZhuang = true;
            ViewBag.Outside = true;
            ViewBag.ZongHe = true;
            if (!typeStr.Contains(ActivityType.Tires.ToString()))
            {
                ViewBag.Tires = false;
            }
            if (!typeStr.Contains(ActivityType.BaoYang.ToString()))
            {
                ViewBag.BaoYang = false;
            }
            if (!typeStr.Contains(ActivityType.ChePin.ToString()))
            {
                ViewBag.ChePin = false;
            }
            if (!typeStr.Contains(ActivityType.GaiZhuang.ToString()))
            {
                ViewBag.GaiZhuang = false;
            }
            if (!typeStr.Contains(ActivityType.Outside.ToString()))
            {
                ViewBag.Outside = false;
            }
            if (!typeStr.Contains(ActivityType.ZongHe.ToString()))
            {
                ViewBag.ZongHe = false;
            }
            ViewBag.IsViewEffect = powerManager.VerifyPermissions(HttpContext.User.Identity.Name, ActivityBoardModuleType.TuhuActivity, OperationType.Effect);
            return View();
        }
        /// <summary>
        /// 活动周期盘点
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public JsonResult GetCycleCountActivity(DateTime start, string title = "", string createdUser = "", string owner = "", string typeStr = "")
        {
            var end = start.AddDays(+8);
            CycleCountActivityViewModel data = new CycleCountActivityViewModel();
            ActivityManager manager = new ActivityManager();
            ActivityBoardPowerManager powerManager = new ActivityBoardPowerManager();

            var powerConfig = powerManager.GetPowerConfigByUserEmail(HttpContext.User.Identity.Name, ActivityBoardModuleType.TuhuActivity);
            if (powerConfig != null && powerConfig.ViewActivity)
            {
                if (powerConfig.VisibleActivityDays != 0)
                {
                    if (powerConfig.VisibleActivityDays != -1)
                    {
                        start = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        end = Convert.ToDateTime(DateTime.Now.AddDays(+(powerConfig.VisibleActivityDays)).ToShortDateString());
                    }
                    var result = manager.GetActivityDetailsForActivityBoard(start, end, title, createdUser, owner);
                    data = ConvertViewModelFroCycleCountActivity(result, start, end, typeStr);
                }
            }

            return Json(new { ToStart = data.JustStarting, Going = data.OnGoing, ToEnd = data.TowardsTheEnd }, JsonRequestBehavior.AllowGet);
        }

        #region 私有方法
        /// <summary>
        /// 转换第三方活动Model
        /// </summary>
        /// <param name="thirdPartyActivity"></param>
        /// <param name="tuhuActivity"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private List<ActivityBoardViewModel> ConvertViewModelForThirdParty(List<ThirdPartActivity> thirdPartyActivity, DateTime start, DateTime end, string typeStr = "")
        {
            List<ActivityBoardViewModel> result = new List<ActivityBoardViewModel>();

            if (thirdPartyActivity != null && thirdPartyActivity.Any())
            {
                result = thirdPartyActivity.Select(x => new ActivityBoardViewModel
                {
                    ActivityId = x.PKID,
                    Title = x.ActivityName,
                    ActivityBoardType = ActivityBoardModuleType.ThirdPartyActivity.ToString(),
                    ActivityType = x.ActivityType.ToString(),
                    StartTime = x.StartTime < start ? start : x.StartTime,
                    EndTime = x.EndTime > end ? end : x.EndTime
                }).Where(x => typeStr.Contains(x.ActivityType)).ToList();
            }

            return result;
        }


        private CycleCountActivityViewModel ConvertViewModelFroCycleCountActivity(List<ActivityBuild> tuhuActivity, DateTime start, DateTime end,string activityType="")
        {
            CycleCountActivityViewModel result = new CycleCountActivityViewModel();
            //筛选即将开始的活动
            result.JustStarting = tuhuActivity
                .Where(_ => _.StartDT.Value >= start && _.StartDT.Value < end)
                .Select(x =>
                {
                    ActivityBoardViewModel item = new ActivityBoardViewModel();
                    item.ActivityId = x.id;
                    item.Title = x.Title;
                    item.ActivityBoardType = "";
                    item.ActivityType = ((ActivityType)x.ActivityConfigType).ToString();
                    item.StartTime = x.StartDT.Value < start ? start : Convert.ToDateTime(x.StartDT.Value.ToShortDateString());
                    item.EndTime = x.EndDate.Value > end ? end : Convert.ToDateTime((x.EndDate.Value.ToShortDateString()));
                    item.DiffDays = x.EndDate.Value > end ? (item.EndTime - item.StartTime).Days : (item.EndTime - item.StartTime).Days + 1;
                    item.IsNew = x.IsNew;
                    item.HashKey = x.HashKey;
                    return item;
                }).Where(x => activityType.Contains(x.ActivityType)).ToList();
            //筛选正在进行的活动
            result.OnGoing = tuhuActivity
                .Where(_ => _.StartDT.Value <= start && _.EndDate.Value >= end)
                .Select(x =>
                {
                    ActivityBoardViewModel item = new ActivityBoardViewModel();
                    item.ActivityId = x.id;
                    item.Title = x.Title;
                    item.ActivityBoardType = "";
                    item.ActivityType = ((ActivityType)x.ActivityConfigType).ToString();
                    item.StartTime = x.StartDT.Value < start ? start : Convert.ToDateTime(x.StartDT.Value.ToShortDateString());
                    item.EndTime = x.EndDate.Value > end ? end : Convert.ToDateTime((x.EndDate.Value.ToShortDateString()));
                    item.DiffDays = x.EndDate.Value > end ? (item.EndTime - item.StartTime).Days : (item.EndTime - item.StartTime).Days + 1;
                    item.IsNew = x.IsNew;
                    item.HashKey = x.HashKey;
                    return item;
                }).Where(x => activityType.Contains(x.ActivityType)).ToList();
            //筛选即将结束的活动
            result.TowardsTheEnd = tuhuActivity
                .Where(_ => _.EndDate.Value >= start && _.EndDate.Value <= end)
                .Select(x =>
                {
                    ActivityBoardViewModel item = new ActivityBoardViewModel();
                    item.ActivityId = x.id;
                    item.Title = x.Title;
                    item.ActivityBoardType = "";
                    item.ActivityType = ((ActivityType)x.ActivityConfigType).ToString();
                    item.StartTime = x.StartDT.Value < start ? start : Convert.ToDateTime(x.StartDT.Value.ToShortDateString());
                    item.EndTime = x.EndDate.Value > end ? end : Convert.ToDateTime((x.EndDate.Value.ToShortDateString()));
                    item.DiffDays = x.EndDate.Value > end ? (item.EndTime - item.StartTime).Days : (item.EndTime - item.StartTime).Days + 1;
                    item.IsNew = x.IsNew;
                    item.HashKey = x.HashKey;
                    return item;
                }).Where(x => activityType.Contains(x.ActivityType)).ToList();

            return result;
        }

        /// <summary>
        /// 转换Model
        /// </summary>
        /// <param name="tuhuActivity"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<ActivityBoardViewModel> ConvertViewModelByType(List<ActivityBuild> tuhuActivity, DateTime start, DateTime end, ActivityBoardModuleType type, string typeStr = "")
        {
            List<ActivityBoardViewModel> result = new List<ActivityBoardViewModel>();

            if (tuhuActivity != null && tuhuActivity.Any())
            {
                //if (type == ActivityBoardModuleType.TuhuActivity)
                //{
                //    tuhuActivity = tuhuActivity.Where(_ => _.ActivityConfigType != 5).ToList();
                //}
                //else if (type == ActivityBoardModuleType.OutsideActivity)
                //{
                //    tuhuActivity = tuhuActivity.Where(_ => _.ActivityConfigType == 5).ToList();
                //}
                result = tuhuActivity.Select(x => new ActivityBoardViewModel
                {
                    ActivityId = x.id,
                    Title = x.Title,
                    ActivityBoardType = type.ToString(),
                    ActivityType = ((ActivityType)x.ActivityConfigType).ToString(),
                    StartTime = x.StartDT.Value < start ? start : x.StartDT.Value,
                    EndTime = x.EndDate.Value > end ? end : x.EndDate.Value,
                    IsNew = x.IsNew,
                    HashKey = x.HashKey
                }).Where(x => typeStr.Contains(x.ActivityType)).ToList();
            }

            return result;
        }
        #endregion
        #endregion

        #region 活动效果
        /// <summary>
        /// 活动效果
        /// </summary>
        /// <returns></returns>
        public ActionResult ActivityEffect(string activityId, DateTime? start, DateTime? end, ActivityBoardModuleType moduleType, string typeStr = "Tires,BaoYang,ChePin,GaiZhuang")
        {
            var user = HttpContext.User.Identity.Name;
            if (user == "zhanglei1@tuhu.cn")
            {
                ActivityBoardManager manager = new ActivityBoardManager();
                ViewBag.Tires = true;
                ViewBag.BaoYang = true;
                ViewBag.ChePin = true;
                ViewBag.GaiZhuang = true;
                ViewBag.ActivityId = activityId;
                if (!typeStr.Contains(ActivityType.Tires.ToString()))
                {
                    ViewBag.Tires = false;
                }
                if (!typeStr.Contains(ActivityType.BaoYang.ToString()))
                {
                    ViewBag.BaoYang = false;
                }
                if (!typeStr.Contains(ActivityType.ChePin.ToString()))
                {
                    ViewBag.ChePin = false;
                }
                if (!typeStr.Contains(ActivityType.GaiZhuang.ToString()))
                {
                    ViewBag.GaiZhuang = false;
                }
                ActivityBoardEffectViewModel result = new ActivityBoardEffectViewModel();
                if (start == null || end == null)
                {
                    start = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToShortDateString());
                    end = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                }
                ViewBag.StartTime = start;
                ViewBag.EndTime = end;
                ViewBag.ModuleType = moduleType;
                if (powerManager.VerifyPermissions(user, moduleType, OperationType.Effect))
                {
                    var data = manager.GetActivityForBI(start.Value, end.Value, activityId);
                    result = ConvertBIModel(data, typeStr);
                }

                return View(result);
            }
            else
            {
                return HttpNotFound();
            }
        }

        /// <summary>
        /// 活动效果数据导出
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="moduleType"></param>
        /// <param name="typeStr"></param>
        /// <returns></returns>
        public ActionResult ExportActivityEffect(string activityId, DateTime? start, DateTime? end, ActivityBoardModuleType moduleType, string typeStr = "Tires,BaoYang,ChePin,GaiZhuang")
        {
            ActivityBoardManager manager = new ActivityBoardManager();
            ActivityBoardEffectViewModel result = new ActivityBoardEffectViewModel();
            List<BIActivityPageModel> data = new List<BIActivityPageModel>();
            if (powerManager.VerifyPermissions(HttpContext.User.Identity.Name, moduleType, OperationType.Effect))
            {
                var info = manager.GetActivityForBI(start.Value, end.Value, activityId);
                result = ConvertBIModel(info, typeStr);
                if (result.IOSPlatforms != null && result.IOSPlatforms.Any())
                {
                    data.AddRange(result.IOSPlatforms);
                }
                if (result.AndroidPlatforms != null && result.AndroidPlatforms.Any())
                {
                    data.AddRange(result.AndroidPlatforms);
                }
                if (result.WebPlatforms != null && result.WebPlatforms.Any())
                {
                    data.AddRange(result.WebPlatforms);
                }
                if (result.WeiXinPlatforms != null && result.WeiXinPlatforms.Any())
                {
                    data.AddRange(result.WeiXinPlatforms);
                }
            }
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            var fileName = "活动效果" + DateTime.Now.ToString("yyyy_MM_dd_HHmm") + ".xls";
            row1.CreateCell(0).SetCellValue("时间");
            row1.CreateCell(1).SetCellValue("活动ID");
            row1.CreateCell(2).SetCellValue("渠道");
            row1.CreateCell(3).SetCellValue("PV");
            row1.CreateCell(4).SetCellValue("UV");
            row1.CreateCell(5).SetCellValue("点击UV");
            row1.CreateCell(6).SetCellValue("下单UV");
            row1.CreateCell(7).SetCellValue("点击转换率");
            row1.CreateCell(8).SetCellValue("下单转换率");
            row1.CreateCell(9).SetCellValue("注册用户数");
            row1.CreateCell(10).SetCellValue("领券人数");
            row1.CreateCell(11).SetCellValue("订单总数");
            row1.CreateCell(12).SetCellValue("销售额");
            row1.CreateCell(13).SetCellValue("券后销售额");
            row1.CreateCell(14).SetCellValue("毛利率");
            row1.CreateCell(15).SetCellValue("券后毛利率");

            for (var i = 0; i < data.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(data[i].ClickTime_Varchar);
                rowtemp.CreateCell(1).SetCellValue(data[i].ID);
                rowtemp.CreateCell(2).SetCellValue(data[i].APP_ID);
                rowtemp.CreateCell(3).SetCellValue(data[i].PV);
                rowtemp.CreateCell(4).SetCellValue(data[i].UV);
                rowtemp.CreateCell(5).SetCellValue(data[i].ClickUV);
                rowtemp.CreateCell(6).SetCellValue(data[i].OrederUV);
                rowtemp.CreateCell(7).SetCellValue((data[i].UV != 0 ? Convert.ToDecimal(data[i].ClickUV * 1.00 / data[i].UV * 100) : 0).ToString("f2") + "%");
                rowtemp.CreateCell(8).SetCellValue((data[i].ClickUV != 0 ? Convert.ToDecimal(data[i].OrederUV * 1.00 / data[i].ClickUV * 100) : 0).ToString("f2") + "%");
                rowtemp.CreateCell(9).SetCellValue(data[i].RegistrationNum);
                rowtemp.CreateCell(10).SetCellValue(data[i].PromotionUV);
                rowtemp.CreateCell(11).SetCellValue(data[i].Ordernum);
                rowtemp.CreateCell(12).SetCellValue((data[i].SalesAmount1_Tire + data[i].SalesAmount1_Maintenance + data[i].SalesPercent1_CarProduct + data[i].SalesPercent1_Hubbeauty).ToString());
                rowtemp.CreateCell(13).SetCellValue((data[i].SalesAmount2_Tire + data[i].SalesAmount2_Maintenance + data[i].SalesPercent2_CarProduct + data[i].SalesPercent2_Hubbeauty).ToString());
                rowtemp.CreateCell(14).SetCellValue((((data[i].SalesAmount1_Tire != 0 ? (data[i].SalesAmount1_Tire - data[i].Cost_Tire) / data[i].SalesAmount1_Tire : 0) + (data[i].SalesAmount1_Maintenance != 0 ? (data[i].SalesAmount1_Maintenance - data[i].Cost_Maintenance) / data[i].SalesAmount1_Maintenance : 0) + (data[i].SalesAmount1_CarProduct != 0 ? (data[i].SalesAmount1_CarProduct - data[i].Cost_CarProduct) / data[i].SalesAmount1_CarProduct : 0) + (data[i].SalesAmount1_Hubbeauty != 0 ? (data[i].SalesAmount1_Hubbeauty - data[i].Cost_Hubbeauty) / data[i].SalesAmount1_Hubbeauty : 0)) * 100).ToString("f2") + "%");
                rowtemp.CreateCell(15).SetCellValue((((data[i].SalesAmount2_Tire != 0 ? (data[i].SalesAmount2_Tire - data[i].Cost_Tire) / data[i].SalesAmount2_Tire : 0) + (data[i].SalesAmount2_Maintenance != 0 ? (data[i].SalesAmount2_Maintenance - data[i].Cost_Maintenance) / data[i].SalesAmount2_Maintenance : 0) + (data[i].SalesAmount2_CarProduct != 0 ? (data[i].SalesAmount2_CarProduct - data[i].Cost_CarProduct) / data[i].SalesAmount2_CarProduct : 0) + (data[i].SalesAmount2_Hubbeauty != 0 ? (data[i].SalesAmount2_Hubbeauty - data[i].Cost_Hubbeauty) / data[i].SalesAmount2_Hubbeauty : 0)) * 100).ToString("f2") + "%");
            }

            //// 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        #region 私有方法
        private ActivityBoardEffectViewModel ConvertBIModel(List<BIActivityPageModel> data, string activityType)
        {
            ActivityBoardEffectViewModel result = new ActivityBoardEffectViewModel();
            if (data != null && data.Any())
            {
                if (activityType.Contains(ActivityType.BaoYang.ToString()) && !activityType.Contains(ActivityType.Tires.ToString())
                    && !activityType.Contains(ActivityType.ChePin.ToString()) && !activityType.Contains(ActivityType.GaiZhuang.ToString()))
                {
                    data.ForEach(x => x.Ordernum = 0);
                    data.ForEach(x => x.OrederUV = 0);
                    data.ForEach(x => x.OrderPercent = 0);
                }
                if (!activityType.Contains(ActivityType.Tires.ToString()))
                {
                    data.ForEach(x => x.Ordernum_Tire = 0);
                    data.ForEach(x => x.SalesAmount1_Tire = 0);
                    data.ForEach(x => x.SalesAmount2_Tire = 0);
                    data.ForEach(x => x.SalesPercent1_Tire = 0);
                    data.ForEach(x => x.SalesPercent2_Tire = 0);
                }
                if (!activityType.Contains(ActivityType.BaoYang.ToString()))
                {
                    data.ForEach(x => x.OrderNum_BY = 0);
                    data.ForEach(x => x.OrderUV_BY = 0);
                    data.ForEach(x => x.OrderPercent_BY = 0);
                    data.ForEach(x => x.SalesAmount1_BY = 0);
                    data.ForEach(x => x.SalesAmount2_BY = 0);
                    data.ForEach(x => x.SalesPercent1_BY = 0);
                    data.ForEach(x => x.SalesPercent2_BY = 0);
                }
                if (!activityType.Contains(ActivityType.ChePin.ToString()))
                {
                    data.ForEach(x => x.Ordernum_CarProduct = 0);
                    data.ForEach(x => x.SalesAmount1_CarProduct = 0);
                    data.ForEach(x => x.SalesAmount2_CarProduct = 0);
                    data.ForEach(x => x.SalesPercent1_CarProduct = 0);
                    data.ForEach(x => x.SalesPercent2_CarProduct = 0);
                }
                if (!activityType.Contains(ActivityType.GaiZhuang.ToString()))
                {
                    data.ForEach(x => x.Ordernum_Hubbeauty = 0);
                    data.ForEach(x => x.SalesAmount1_Hubbeauty = 0);
                    data.ForEach(x => x.SalesAmount2_Hubbeauty = 0);
                    data.ForEach(x => x.SalesPercent1_Hubbeauty = 0);
                    data.ForEach(x => x.SalesPercent2_Hubbeauty = 0);
                }
                result.IOSPlatforms = data.Where(x => String.Equals(x.APP_ID, "h5_ios_app", StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(_ => _.ClickTime_Varchar).ToList();
                result.AndroidPlatforms = data.Where(x => String.Equals(x.APP_ID, "h5_android_app", StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(_ => _.ClickTime_Varchar).ToList();
                result.WebPlatforms = data.Where(x => String.Equals(x.APP_ID, "tuhu_web", StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(_ => _.ClickTime_Varchar).ToList();
                result.WeiXinPlatforms = data.Where(x => String.Equals(x.APP_ID, "weixin", StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(_ => _.ClickTime_Varchar).ToList();

                result.AllPlatforms = new BIActivityPageModel()
                {
                    PV = data.Sum(x => x.PV),
                    UV = data.Sum(x => x.UV),
                    ClickUV = data.Sum(x => x.ClickUV),
                    RegistrationNum = data.Sum(x => x.RegistrationNum),
                    PromotionUV = data.Sum(x => x.PromotionUV),
                    OrederUV = data.Sum(x => x.OrederUV + x.OrderUV_BY),
                    Ordernum = data.Sum(x => x.Ordernum + x.OrderNum_BY),
                    Ordernum_Tire = data.Sum(x => x.Ordernum_Tire),
                    Ordernum_Maintenance = data.Sum(x => x.Ordernum_Maintenance),
                    Ordernum_CarProduct = data.Sum(x => x.Ordernum_CarProduct),
                    Ordernum_Hubbeauty = data.Sum(x => x.Ordernum_Hubbeauty),
                    Ordernum_Others = data.Sum(x => x.Ordernum_Others),
                    SalesAmount1_Tire = data.Sum(x => x.SalesAmount1_Tire),
                    SalesAmount1_BY = data.Sum(x => x.SalesAmount1_BY),
                    SalesAmount1_CarProduct = data.Sum(x => x.SalesAmount1_CarProduct),
                    SalesAmount1_Hubbeauty = data.Sum(x => x.SalesAmount1_Hubbeauty),
                    SalesAmount1_Others = data.Sum(x => x.SalesAmount1_Others),
                    SalesAmount2_Tire = data.Sum(x => x.SalesAmount2_Tire),
                    SalesAmount2_BY = data.Sum(x => x.SalesAmount2_BY),
                    SalesAmount2_CarProduct = data.Sum(x => x.SalesAmount2_CarProduct),
                    SalesAmount2_Hubbeauty = data.Sum(x => x.SalesAmount2_Hubbeauty),
                    SalesAmount2_Others = data.Sum(x => x.SalesAmount2_Others),
                    ClickPercent = data.Sum(x => x.UV) != 0 ? Convert.ToDecimal(data.Sum(x => x.ClickUV) * 1.00 / data.Sum(x => x.UV)) : 0,
                    OrderPercent = data.Sum(x => x.ClickUV) != 0 ? Convert.ToDecimal(data.Sum(x => x.OrederUV + x.OrderUV_BY) * 1.00 / data.Sum(x => x.ClickUV)) : 0,
                    Cost_Tire = data.Sum(x => x.Cost_Tire),
                    Cost_BY = data.Sum(x => x.Cost_BY),
                    Cost_CarProduct = data.Sum(x => x.Cost_CarProduct),
                    Cost_Hubbeauty = data.Sum(x => x.Cost_Hubbeauty)
                };
            }

            return result;
        }
        #endregion
        #endregion

        #region 途虎活动与外部活动
        public JsonResult GetTuhuOrOutSideActivity(int pkid, bool isNew = false)
        {
            ActivityBuild result = null;
            var des = new List<string>();
            if (!isNew)
            {
                ActivityManager manager = new ActivityManager();
                result = manager.GetActivityBuildById(pkid);
                if (result != null && result.Content.Contains("total"))
                {
                    var json = JObject.Parse(result.Content);
                    var data = JsonConvert.DeserializeObject<List<ActivityBuildDetail>>(json["rows"].ToString());
                    if (data != null && data.Any())
                    {
                        var getResult = data.Where(x => x.Type == 8).ToList();
                        foreach (var item in getResult)
                        {
                            des.Add(item.Description.Replace("<li>", "").Replace("</li>", ""));
                        }
                    }
                }
            }
            else
            {
                ActivityBoardManager manaer = new ActivityBoardManager();
                result = manaer.GetActivityForPage(pkid);
                if (result != null && !string.IsNullOrEmpty(result.Content))
                {
                    des.Add(result.Content);
                }
            }

            if (result != null)
            {
                return Json(new { status = true, data = result, des = des }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 第三方活动
        /// <summary>
        /// 第三方活动
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="ActivityType"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult ThirdPartActivity(DateTime? StartTime, DateTime? EndTime, ActivityType ActivityType = ActivityType.None, int pageIndex = 1,string channel="")
        {
            ThirdPartActivityManager manager = new ThirdPartActivityManager();
            List<ThirdPartActivity> result = new List<ThirdPartActivity>();
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex,
                PageSize = 20
            };
            if (powerManager.VerifyPermissions(HttpContext.User.Identity.Name, ActivityBoardModuleType.ThirdPartyActivity, OperationType.View))
            {
                result = manager.GetThirdPartActivity(pageIndex, pager.PageSize, StartTime, EndTime, (int)ActivityType, channel);
                if (result != null && result.Any()) { pager.TotalItem = result.FirstOrDefault().Total; }
            }
            ViewBag.CurrentPage = pager.CurrentPage;
            ViewBag.StartTime = StartTime;
            ViewBag.EndTime = EndTime;
            ViewBag.ActivityType = (int)ActivityType;
            ViewBag.ModuleTyp = ActivityBoardModuleType.ThirdPartyActivity.ToString();
            ViewBag.TotalPage = pager.TotalPage;
            ViewBag.Channel = channel;
            ViewBag.IsViewEffect = powerManager.VerifyPermissions(HttpContext.User.Identity.Name, ActivityBoardModuleType.ThirdPartyActivity, OperationType.Effect);

            return View(result);
        }

        /// <summary>
        /// 根据PKID获取第三方活动
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public JsonResult GetThirdPartActivityByPKID(int pkid)
        {
            ThirdPartActivityManager manager = new ThirdPartActivityManager();
            ThirdPartActivity result = null;
            string msg = "";

            if (powerManager.VerifyPermissions(HttpContext.User.Identity.Name, ActivityBoardModuleType.ThirdPartyActivity, OperationType.View))
            {
                result = manager.GetThirdPartActivityByPKID(pkid);
            }
            else
            {
                msg = "权限不足，请联系相关人员配置相应权限";
            }

            if (result != null)
            {
                return Json(new { status = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, msg = msg }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 添加第三方活动
        /// </summary>
        /// <param name="thirdPartActivity"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public JsonResult InsertThirdPartActivity(string thirdPartActivity, ActivityBoardModuleType moduleType = ActivityBoardModuleType.ThirdPartyActivity)
        {
            ThirdPartActivityManager manager = new ThirdPartActivityManager();
            var result = false;
            var msg = "参数为空";
            var data = JsonConvert.DeserializeObject<ThirdPartActivity>(thirdPartActivity);
            var user = HttpContext.User.Identity.Name;

            if (powerManager.VerifyPermissions(user, moduleType, OperationType.Insert))
            {
                if (data != null)
                {
                    data.ActivityRules = Server.UrlDecode(data.ActivityRules);
                    if (data.StartTime != null && data.EndTime != null && data.StartTime < data.EndTime)
                    {
                        result = manager.InsertThirdPartActivity(data, user);
                    }
                    else
                    {
                        msg = "活动时间不合法";
                    }
                }
            }
            else
            {
                msg = "权限不足，请联系相关人员配置相应权限";
            }

            return Json(new { status = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑第三方活动
        /// </summary>
        /// <param name="thirdPartActivity"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public JsonResult UpdateThirdPartActivity(string thirdPartActivity, ActivityBoardModuleType moduleType = ActivityBoardModuleType.ThirdPartyActivity)
        {
            ThirdPartActivityManager manager = new ThirdPartActivityManager();
            var result = false;
            var msg = "参数为空";
            var data = JsonConvert.DeserializeObject<ThirdPartActivity>(thirdPartActivity);
            var user = HttpContext.User.Identity.Name;

            if (powerManager.VerifyPermissions(user, moduleType, OperationType.Update))
            {
                if (data != null)
                {
                    data.ActivityRules = Server.UrlDecode(data.ActivityRules);
                    if (data.StartTime != null && data.EndTime != null && data.StartTime < data.EndTime)
                    {
                        result = manager.UpdateThirdPartActivity(data, user);
                    }
                    else
                    {
                        msg = "活动时间不合法";
                    }
                }
            }
            else
            {
                msg = "权限不足，请联系相关人员配置相应权限";
            }

            return Json(new { status = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除第三方活动
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public JsonResult DeleteActivityByPKID(int pkid, ActivityBoardModuleType moduleType = ActivityBoardModuleType.ThirdPartyActivity)
        {
            ThirdPartActivityManager manager = new ThirdPartActivityManager();
            var result = false;
            string msg = string.Empty;
            if (powerManager.VerifyPermissions(HttpContext.User.Identity.Name, moduleType, OperationType.Delete))
            {
                result = manager.DeleteActivityByPKID(pkid, HttpContext.User.Identity.Name);
            }
            else
            {
                msg = "权限不足，请联系相关人员配置相应权限";
            }

            return Json(new { status = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 权限配置
        /// <summary>
        /// 活动看板权限
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        //[PowerManage]
        public ActionResult PowerConfig(string userEmail = "", int pageIndex = 1)
        {
            ActivityBoardPowerManager manager = new ActivityBoardPowerManager();
            List<AcvitityBoardPowerViewModel> result = new List<AcvitityBoardPowerViewModel>();
            ViewBag.UserEmail = userEmail.Trim();
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex,
                PageSize = 20
            };

            if (ConfigurationManager.AppSettings["ActivityBoardPowerConfig"].Contains(HttpContext.User.Identity.Name))
            {
                var data = manager.GetActivityBoardPowerConfig(userEmail, pageIndex, pager.PageSize, pager);

                if (data != null && data.Any())
                {
                    result = ConvertViewModel(data);
                }
            }

            ViewBag.TotalPage = pager.TotalPage;
            ViewBag.CurrentPage = pager.CurrentPage;

            return View(result);
        }

        /// <summary>
        /// 根据用户账号和模块名称获取权限
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonResult GetPowerConfigByUserEmail(string userEmail, ActivityBoardModuleType type)
        {
            ActivityBoardPowerManager manager = new ActivityBoardPowerManager();
            ActivityBoardPermissionConfig result = null;

            result = manager.GetPowerConfigByUserEmail(userEmail.Trim(), type) ?? new ActivityBoardPermissionConfig { UserEmail = userEmail, ModuleType = type };

            if (result != null)
            {
                return Json(new { status = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 添加模块权限
        /// </summary>
        /// <param name="powerConfig"></param>
        /// <returns></returns>
        //[PowerManage]
        public JsonResult InsertPowerConfig(string powerConfig)
        {
            ActivityBoardPowerManager manager = new ActivityBoardPowerManager();
            var result = false;
            string msg = "权限不足，请联系相关人员配置相应权限";
            var user = HttpContext.User.Identity.Name;

            if (ConfigurationManager.AppSettings["ActivityBoardPowerConfig"].Contains(HttpContext.User.Identity.Name))
            {
                if (!string.IsNullOrEmpty(powerConfig))
                {
                    var data = JsonConvert.DeserializeObject<ActivityBoardPermissionConfig>(powerConfig);
                    var getResult = manager.InsertActivityBoardPowerConfig(data);
                    result = getResult.Item1;
                    msg = getResult.Item2;
                }
            }

            return Json(new { status = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑模块权限
        /// </summary>
        /// <param name="powerConfig"></param>
        /// <returns></returns>
        //[PowerManage]
        public JsonResult UpdatePowerConfig(string powerConfig)
        {
            ActivityBoardPowerManager manager = new ActivityBoardPowerManager();
            var result = false;
            string msg = "权限不足，请联系相关人员配置相应权限";

            if (ConfigurationManager.AppSettings["ActivityBoardPowerConfig"].Contains(HttpContext.User.Identity.Name))
            {
                if (!string.IsNullOrEmpty(powerConfig))
                {
                    var data = JsonConvert.DeserializeObject<ActivityBoardPermissionConfig>(powerConfig);
                    var getResult = manager.UpdateActivityBoardPowerConfig(data);
                    result = getResult.Item1;
                    msg = getResult.Item2;
                }
            }

            return Json(new { status = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除模块权限
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        //[PowerManage]
        public JsonResult DeletePowerConfig(string userEmail)
        {
            ActivityBoardPowerManager manager = new ActivityBoardPowerManager();
            var result = false;

            if (ConfigurationManager.AppSettings["ActivityBoardPowerConfig"].Contains(HttpContext.User.Identity.Name))
            {
                result = manager.DeleteActivityBoardPowerConfig(userEmail.Trim());
            }

            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>       
        public JsonResult UploadFile()
        {
            var success = new List<ActivityBoardPermissionConfig>();
            var error = new List<ActivityBoardPermissionConfig>();
            var msg = string.Empty;
            ActivityBoardPowerManager manager = new ActivityBoardPowerManager();
            var data = new List<ActivityBoardPermissionConfig>();

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                DataTable dataTable = null;
                string fileExtension = Path.GetExtension(file.FileName); // 文件扩展名
                bool isxlsx = fileExtension.Equals(".xlsx");
                bool isxls = fileExtension.Equals(".xls");
                if (file != null && (isxls || isxlsx))
                {
                    using (Stream writer = file.InputStream)
                    {
                        if (isxlsx)
                        {
                            dataTable = RenderDataTableForXLSX(writer, 0);
                        }
                        if (isxls)
                        {
                            dataTable = RenderDataTableForXLS(writer, 0);
                        }
                    }
                    data = ConvertList(dataTable);
                    var verifyData = manager.VerifyData(data);
                    success = verifyData.Item1;
                    error = verifyData.Item2;
                }
                else
                {
                    msg = "文件为空或是文件上传格式不正确";
                }
            }

            return Json(new { SuccessData = success, ErrorData = error, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量操作数据
        /// </summary>
        /// <param name="powerConfigList"></param>
        /// <returns></returns>
        //[PowerManage]
        public JsonResult BatchOperationData(string powerConfigList)
        {
            ActivityBoardPowerManager manager = new ActivityBoardPowerManager();
            var result = false;
            string msg = "权限不足，请联系相关人员配置相应权限";
            var data = JsonConvert.DeserializeObject<List<ActivityBoardPermissionConfig>>(powerConfigList);

            if (ConfigurationManager.AppSettings["ActivityBoardPowerConfig"].Contains(HttpContext.User.Identity.Name))
            {
                var getResult = manager.BatchOperationData(data);
                result = getResult.Item1;
                msg = getResult.Item2;
            }

            return Json(new { status = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下载权限模板
        /// </summary>
        /// <returns></returns>
        public ActionResult DownLoadFile()
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("用户名");
            row1.CreateCell(1).SetCellValue("模块名称");
            row1.CreateCell(2).SetCellValue("是否可见");
            row1.CreateCell(3).SetCellValue("可见天数");
            row1.CreateCell(4).SetCellValue("是否允许添加");
            row1.CreateCell(5).SetCellValue("是否允许删除");
            row1.CreateCell(6).SetCellValue("是够允许修改");
            row1.CreateCell(7).SetCellValue("是否允许查看");
            row1.CreateCell(8).SetCellValue("活动效果是否可见");
            var fileName = "活动看板权限导入模板.xls";
            NPOI.SS.UserModel.IRow row2 = sheet1.CreateRow(1);
            row2.CreateCell(0).SetCellValue("tuhu@tuhu.cn(示例,上传时删掉这一行)");
            row2.CreateCell(1).SetCellValue("第三方活动/途虎活动(可选，选择一个模块，配置多个模块写多行)");
            row2.CreateCell(2).SetCellValue("是/否");
            row2.CreateCell(3).SetCellValue("-1 代表全部 15 代表15天 1 代表1天 0 代表0天");
            row2.CreateCell(4).SetCellValue("是/否");
            row2.CreateCell(5).SetCellValue("是/否");
            row2.CreateCell(6).SetCellValue("是/否");
            row2.CreateCell(7).SetCellValue("是/否");
            row2.CreateCell(8).SetCellValue("是/否");
            // 写入到客户端 
            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonResult SelectOperationLog(string objectId, ActivityBoardModuleType type)
        {
            ActivityBoardManager manager = new ActivityBoardManager();
            var result = manager.SelectOperationLog(objectId, type);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region 私有方法
        /// <summary>
        /// 转换上传的文件信息
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private List<ActivityBoardPermissionConfig> ConvertList(DataTable dataTable)
        {
            var result = new List<ActivityBoardPermissionConfig>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new ActivityBoardPermissionConfig()
                    {
                        UserEmail = row["用户名"].ToString().Trim(),
                        ModuleType = ConvertModuleName(row["模块名称"].ToString().Trim()),
                        IsVisible = string.Equals(row["是否可见"].ToString().Trim(), "是",
                        StringComparison.CurrentCultureIgnoreCase) ? true : false,
                        VisibleActivityDays = Convert.ToInt32(row["可见天数"].ToString().Trim() ?? "0"),
                        InsertActivity = string.Equals(row["是否允许添加"].ToString().Trim(), "是",
                        StringComparison.CurrentCultureIgnoreCase) ? true : false,
                        DeleteActivity = string.Equals(row["是否允许删除"].ToString().Trim(), "是",
                        StringComparison.CurrentCultureIgnoreCase) ? true : false,
                        UpdateActivity = string.Equals(row["是够允许修改"].ToString().Trim(), "是",
                        StringComparison.CurrentCultureIgnoreCase) ? true : false,
                        ViewActivity = string.Equals(row["是否允许查看"].ToString().Trim(), "是",
                        StringComparison.CurrentCultureIgnoreCase) ? true : false,
                        ActivityEffect = string.Equals(row["活动效果是否可见"].ToString().Trim(), "是",
                        StringComparison.CurrentCultureIgnoreCase) ? true : false,
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// 转换Model
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<AcvitityBoardPowerViewModel> ConvertViewModel(List<ActivityBoardPermissionConfig> data)
        {
            List<AcvitityBoardPowerViewModel> result = new List<AcvitityBoardPowerViewModel>();

            if (data != null && data.Any())
            {
                result = data.GroupBy(x => x.UserEmail).Select(g => new AcvitityBoardPowerViewModel
                {
                    UserEmail = g.Key,
                    ModuleConfig = new ModuleConfig
                    {
                        TuhuActivityPower = g.Where(x => x.ModuleType.Equals(ActivityBoardModuleType.TuhuActivity)).FirstOrDefault() ?? new ActivityBoardPermissionConfig(),
                        ThirdPartyActivityPower = g.Where(x => x.ModuleType.Equals(ActivityBoardModuleType.ThirdPartyActivity)).FirstOrDefault() ?? new ActivityBoardPermissionConfig(),
                        OutSideActivityPower = g.Where(x => x.ModuleType.Equals(ActivityBoardModuleType.OutsideActivity)).FirstOrDefault() ?? new ActivityBoardPermissionConfig(),
                    },
                }).ToList();
            }

            return result;
        }

        /// <summary>
        /// 模块名称对应枚举
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        private ActivityBoardModuleType ConvertModuleName(string moduleName)
        {
            ActivityBoardModuleType type = ActivityBoardModuleType.TuhuActivity;
            switch (moduleName)
            {
                case "途虎活动":
                    type = ActivityBoardModuleType.TuhuActivity;
                    break;
                case "第三方活动":
                    type = ActivityBoardModuleType.ThirdPartyActivity;
                    break;
                    //case "外部投放活动":
                    //    type = ActivityBoardModuleType.OutsideActivity;
                    //break;
            }
            return type;
        }

        private DataTable RenderDataTableForXLSX(Stream excelFileStream, int headerRowIndex)
        {
            DataTable table = new DataTable();

            XSSFWorkbook workbook = new XSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(0);
            var headerRow = sheet.GetRow(headerRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue.Trim(), typeof(string));
                table.Columns.Add(column);
            }

            int rowCount = sheet.LastRowNum;
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null && row.FirstCellNum >= 0)
                {
                    DataRow dataRow = table.NewRow();
                    bool hasValue = false;

                    for (int j = row.FirstCellNum; j < row.Cells.Count; j++)
                    {
                        if (row.GetCell(j) != null && !string.IsNullOrEmpty(row.GetCell(j).ToString()))
                        {
                            hasValue = true;
                            dataRow[j] = row.GetCell(j).ToString().Trim();
                        }
                    }
                    if (hasValue)
                    {
                        table.Rows.Add(dataRow);
                    }
                }
            }

            workbook = null;
            sheet = null;

            return table;
        }

        private DataTable RenderDataTableForXLS(Stream excelFileStream, int headerRowIndex)
        {
            DataTable table = new DataTable();

            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(0);
            var headerRow = sheet.GetRow(headerRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue.Trim(), typeof(string));
                table.Columns.Add(column);
            }

            int rowCount = sheet.LastRowNum;
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null && row.FirstCellNum >= 0)
                {
                    DataRow dataRow = table.NewRow();
                    bool hasValue = false;
                    for (int j = row.FirstCellNum; j < row.Cells.Count; j++)
                    {
                        if (row.GetCell(j) != null && !string.IsNullOrEmpty(row.GetCell(j).ToString()))
                        {
                            hasValue = true;
                            dataRow[j] = row.GetCell(j).ToString().Trim();
                        }
                    }
                    if (hasValue)
                    {
                        table.Rows.Add(dataRow);
                    }
                }
            }

            workbook = null;
            sheet = null;

            return table;
        }
        #endregion

        #endregion
    }
}