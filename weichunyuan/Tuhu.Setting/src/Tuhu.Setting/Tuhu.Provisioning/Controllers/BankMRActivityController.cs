using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using NPOI.SS.Util;
using Tuhu.Provisioning.Business.BeautyService;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Shop;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.Logger;

namespace Tuhu.Provisioning.Controllers
{
    public class BankMRActivityController : Controller
    {
        private BankMRManager manager = new BankMRManager();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 合作用户配置页面
        /// </summary>
        /// <returns></returns>
        [PowerManage]
        public ActionResult MrCooperateUser()
        {
            return View();
        }

        public ActionResult BankMRActivityLimitConfig(Guid activityId)
        {
            ViewBag.ActivityId = activityId;
            return View();
        }

        /// <summary>
        /// 分页获取银行美容活动配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetBankMRActivityConfigs(int pageIndex, int pageSize, int cooperateId = 0,
            string activityName = null, string serviceId = null, string settleMentMethod = null)
        {
            int count = 0;
            var configs = manager.GetBankMRActivityConfigs(pageIndex, pageSize, cooperateId, activityName, serviceId, settleMentMethod, out count);
            if (configs != null)
            {
                var cooperateIds = configs.Select(t => t.CooperateId).Distinct();
                var cooperateUsers = new List<MrCooperateUserConfig>();
                foreach (var id in cooperateIds)
                {
                    var config = manager.FetchMrCooperateUserConfigByPKID(id);
                    if (config != null)
                        cooperateUsers.Add(config);
                }
                configs.ForEach(s =>
                {
                    s.CooperateName = cooperateUsers.FirstOrDefault(t => t.PKID == s.CooperateId)?.CooperateName;
                    s.VerifyType = GetOrderByFirstLetter(s.VerifyType);
                });

            }
            return Json(new { result = configs, total = count }, JsonRequestBehavior.AllowGet);
        }
        private string GetOrderByFirstLetter(string str)
        {

            if (!string.IsNullOrEmpty(str) && str.Contains(","))
            {
                var temp = str.Split(',');
                var temp1 = temp.OrderByDescending(o => o.FirstOrDefault());
                str = string.Join(",", temp1);
            }
            return str;
        }

        /// <summary>
        /// 根据活动ID查询广告位配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult GetBankMRActivityAdConfig(Guid activityId)
        {
            var adConfig = manager.GetBankMRActivityAdConfigByActivityId(activityId);
            return Json(adConfig, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 插入或更新银行美容活动配置
        /// </summary>
        /// <param name="config"></param>
        /// <param name="adConfig">广告位配置</param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult UpsertBankMRActivityConfig(BankMRActivityConfig config, IEnumerable<BankMRActivityAdConfig> adConfig)
        {
            var result = false;
            if (config != null)
            {
                var operateUser = HttpContext.User.Identity.Name;
                var cooperateUser = manager.FetchMrCooperateUserConfigByPKID(config.CooperateId);
                if (cooperateUser != null)
                {
                    var BeautyServiceCodeTypes = BeautyServicePackageManager.SelectAllBeautyServiceCodeTypeConfig();
                    config.BankId = cooperateUser.VipUserId;
                    config.CodeTypeConfigId = BeautyServiceCodeTypes?.FirstOrDefault(x => x.PID.Equals(config.ServiceId)).PKID ?? 0;
                    if (config.MillisecondsRoundStartTime > 0)
                    {
                        config.RoundStartTime = TimeSpan.FromMilliseconds(config.MillisecondsRoundStartTime);
                    }
                    result = manager.UpsertBankMRActivityConfig(config, adConfig, operateUser);
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据活动ID获取银行美容活动场次
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetBankMRActivityRoundConfigsByActivityId(Guid activityId)
        {
            var result = manager.GetBankMRActivityRoundConfigsByActivityId(activityId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsExistBankMRActivityUsersByActivityId(Guid activityId)
        {
            var result = false;
            var rounds = manager.GetBankMRActivityRoundConfigsByActivityId(activityId);

            if (rounds != null && rounds.Any())
            {
                result = manager.IsExistBankMRActivityUsersByRoundIds(rounds.Select(t => t.PKID));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 分页查询合作用户配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetMrCooperateUserConfigs(int pageIndex, int pageSize)
        {
            var result = manager.GetMrCooperateUserConfigs(pageIndex, pageSize);
            var config = result.Item1;
            if (config != null)
            {
                var userIds = config.Select(t => t.VipUserId).Distinct();
                var usersDetail = new List<SYS_CompanyUser>();
                using (var client = new UserAccountClient())
                {
                    foreach (var item in userIds)
                    {
                        var userResult = client.SelectCompanyUserInfo(item);
                        if (userResult.Success && userResult.Result != null)
                        {
                            usersDetail.Add(userResult.Result);
                        }
                    }
                }
                foreach (var item in config)
                {
                    var userDetail = usersDetail.FirstOrDefault(t => t.UserId == item.VipUserId);
                    if (userDetail != null)
                    {
                        item.VipUserName = userDetail.UserName;
                        item.VipCompanyName = userDetail.CompanyInfo?.Name;
                        item.VipCompanyId = userDetail.CompanyInfo?.Id ?? 0;
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有合作用户配置
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllMrCooperateUserConfigs()
        {
            var result = manager.GetAllMrCooperateUserConfigs();
            if (result != null)
            {
                var userIds = result.Select(t => t.VipUserId).Distinct();
                var usersDetail = new List<SYS_CompanyUser>();
                using (var client = new UserAccountClient())
                {
                    foreach (var item in userIds)
                    {
                        var userResult = client.SelectCompanyUserInfo(item);
                        if (userResult.Success && userResult.Result != null)
                        {
                            usersDetail.Add(userResult.Result);
                        }
                    }
                }
                foreach (var item in result)
                {
                    var userDetail = usersDetail.FirstOrDefault(t => t.UserId == item.VipUserId);
                    if (userDetail != null)
                    {
                        item.VipUserName = userDetail.UserName;
                        item.VipCompanyName = userDetail.CompanyInfo?.Name;
                        item.VipCompanyId = userDetail.CompanyInfo?.Id ?? 0;
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取导入用户的合作用户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCooperateUserConfigsForImportUser()
        {
            IEnumerable<MrCooperateUserConfig> result = null;
            var packageDetails = BeautyServicePackageManager.GetBeautyServicePackageDetails(1, 10000, true, string.Empty, 0, string.Empty);
            if (packageDetails != null)
            {
                var cooperateIds = packageDetails.Item1?.Select(s => s.CooperateId)?.Distinct() ?? new List<int>();
                var cooperateUserConfig = manager.GetAllMrCooperateUserConfigs();
                result = cooperateUserConfig?.Where(s => cooperateIds.Contains(s.PKID));
                if (result != null)
                {
                    var userIds = result.Select(t => t.VipUserId).Distinct();
                    var usersDetail = new List<SYS_CompanyUser>();
                    using (var client = new UserAccountClient())
                    {
                        foreach (var item in userIds)
                        {
                            var userResult = client.SelectCompanyUserInfo(item);
                            if (userResult.Success && userResult.Result != null)
                            {
                                usersDetail.Add(userResult.Result);
                            }
                        }
                    }
                    foreach (var item in result)
                    {
                        var userDetail = usersDetail.FirstOrDefault(t => t.UserId == item.VipUserId);
                        if (userDetail != null)
                        {
                            item.VipUserName = userDetail.UserName;
                            item.VipCompanyName = userDetail.CompanyInfo?.Name;
                            item.VipCompanyId = userDetail.CompanyInfo?.Id ?? 0;
                        }
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新合作用户配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [PowerManage]
        [HttpPost]
        public ActionResult UpsertMrCooperateUserConfig(MrCooperateUserConfig config)
        {
            var result = false;

            if (config != null)
            {
                if (config.PKID > 0)
                {
                    result = manager.UpdateMrCooperateUserConfig(config);
                }
                else
                {
                    config.CreateUser = HttpContext.User.Identity.Name;
                    result = manager.InsertMrCooperateUserConfig(config);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取限购配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FetchBankMRActivityLimitConfig(Guid activityId)
        {
            var result = manager.FetchBankMRActivityLimitConfig(activityId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 插入或更新限购配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpsertBankMRActivityLimitConfig(BankMRActivityLimitConfig config)
        {
            var result = false;

            if (config != null)
            {
                var operateUser = HttpContext.User.Identity.Name;
                if (config.BankLimitConfig != null && config.BankLimitConfig.Count > 0)
                {
                    config.ProvinceIds = String.Join(",", config.BankLimitConfig.Select(x => x.ProvinceId).ToList().Distinct());
                    config.CityIds = String.Join(",", config.BankLimitConfig.Where(_ => _.CityId > 0).Select(x => x.CityId).ToList().Distinct());
                    List<int> allDistrictIds = new List<int>();
                    config.BankLimitConfig.ForEach(x =>
                    {
                        if (x.DistrictIds != null && x.DistrictIds.Count() > 0)
                        {
                            allDistrictIds.AddRange(x.DistrictIds);
                        }
                    });
                    config.DistrictIds = String.Join(",", allDistrictIds);
                }
                else
                {
                    config.DistrictIds = null;
                }
                if (config.PKID > 0)
                {
                    result = manager.UpdateBankMRActivityLimitConfig(config, operateUser);
                }
                else
                {
                    result = manager.InsertBankMRActivityLimitConfig(config, operateUser);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownLoadBankMRActivityUsersByActivityId(Guid activityId)
        {
            var ms = new MemoryStream();
            if (activityId != null && activityId != Guid.Empty)
            {
                var roundIds = manager.GetBankMRActivityRoundConfigsByActivityId(activityId);

            }

            return File(ms.ToArray(), "application/x-xls", $"导入活动规则模板{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");

        }

        public FileResult DownLoadRulesTemplate(string verifyType, string otherVerifyName)
        {
            var ms = new MemoryStream();
            if (verifyType != null)
            {
                var verifyTypeArray = verifyType.Split(',');

                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet();

                var row = sheet.CreateRow(0);
                var cellNum = 0;
                var column = GetBankRuleExcelColumn(verifyType, otherVerifyName);
                foreach (var item in column)
                {
                    row.CreateCell(cellNum++).SetCellValue(item.Item1);
                }
                cellNum = 0;
                for (var i = 0; i < verifyTypeArray.Length + 2; i++)
                {
                    sheet.SetColumnWidth(cellNum++, 18 * 256);
                }
                workbook.Write(ms);
            }

            return File(ms.ToArray(), "application/x-xls", $"导入活动规则模板{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }
        public JsonResult UploadRulesFile(string verifyType,string otherVerifyName, Guid activityId, string importRoundType, bool pass = false)
        {
            var result = false;
            var msg = string.Empty;
            var files = Request.Files;
            var file = files[0];
            string fileExtension = System.IO.Path.GetExtension(Request.Files[0].FileName);

            if (files.Count <= 0 || (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                msg = "请上传Excel文件";
            }
            else
            {
                var stream = file.InputStream;
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                var sheet = workBook.GetSheetAt(0);
                var titleRow = sheet.GetRow(sheet.FirstRowNum);
                var userRules = new List<BankMRActivityUser>();
                try
                {
                    Func<ICell, string> getStringValue = cell =>
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
                    var checkMoreThan2Limit = !pass;
                    var column = GetBankRuleExcelColumn(verifyType, otherVerifyName);
                    for (var rowIndex = sheet.FirstRowNum; rowIndex <= sheet.LastRowNum; rowIndex++)//遍历每一行
                    {
                        BankMRActivityUser rule = new BankMRActivityUser() { BankCardNum = "" };
                        var row = sheet.GetRow(rowIndex);
                        if (row == null) continue;
                        var cellIndex = row.FirstCellNum;
                        for (var i = 0; i < column.Count; i++)//遍历每一列
                        {
                            var itemValue = getStringValue(row.GetCell(cellIndex++));
                            if (string.IsNullOrWhiteSpace(itemValue))
                                throw new Exception("必要列不能为空");
                            itemValue = itemValue.Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", "").Trim();
                            if (rowIndex == sheet.FirstRowNum)
                            {
                                if (!string.Equals(itemValue, column[i].Item1))
                                    throw new Exception("Excel头部列名不对");
                            }
                            else
                            {
                                switch (column[i].Item2)
                                {
                                    case "Moble1":
                                        long mobile = 0;
                                        if (long.TryParse(itemValue, out mobile))
                                        {
                                            rule.Mobile = mobile;
                                        }
                                        else
                                        {
                                            throw new Exception("手机号码格式有不正确的，请检查");
                                        }
                                        break;
                                    case "Card1":
                                        if (itemValue.Contains("*") && itemValue.Split('*')[0].Length > 0 && itemValue.Split('*')[1].Length > 0)
                                        {
                                            rule.BankCardNum = itemValue;
                                        }
                                        else
                                        {
                                            throw new Exception("银行卡不满足前N后M，中间用*号隔开");
                                        }
                                        break;
                                    case "Card2":
                                        if (Regex.IsMatch(itemValue, "^[0-9]+$"))
                                        {
                                            rule.BankCardNum = itemValue;
                                        }
                                        else
                                        {
                                            throw new Exception("银行卡号不正确");
                                        }
                                        break;
                                    case "Card3":
                                        if (Regex.IsMatch(itemValue, "^[0-9]+$"))
                                        {
                                            rule.BankCardNum = itemValue;
                                        }
                                        else
                                        {
                                            throw new Exception("银行卡号不正确");
                                        }
                                        break;
                                    case "Other1":
                                        rule.OtherNum = itemValue;
                                        break;
                                    case "Other":
                                        rule.OtherNum = itemValue;
                                        break;
                                    case "LimitCount":
                                        rule.LimitCount = Convert.ToInt32(itemValue);
                                        if (rule.LimitCount > 2 && checkMoreThan2Limit)
                                        {
                                            throw new Exception("注意:[场次限购/每日限购]次数存在超过2次的,确认上传?");
                                        }
                                        break;
                                    case "DayLimit":
                                        rule.DayLimit = Convert.ToInt32(itemValue);
                                        if (rule.DayLimit > 2 && checkMoreThan2Limit)
                                        {
                                            throw new Exception("注意:[场次限购/每日限购]次数存在超过2次的,确认上传?");
                                        }
                                        break;
                                    default:
                                        throw new Exception("未知列");
                                }
                            }
                        }
                        if (rowIndex != sheet.FirstRowNum)
                            userRules.Add(rule);
                    }
                    if (userRules.Any())
                    {
                        var note = GetRepeatDataRemark(userRules);

                        var rounds = manager.GetBankMRActivityRoundConfigsByActivityId(activityId);
                        rounds = rounds?.Where(s => s.IsActive);
                        var roundIds = new List<int>();
                        switch (importRoundType)
                        {
                            case "AllRound":
                                roundIds = rounds?.Select(t => t.PKID)?.ToList() ?? new List<int>();
                                break;
                            case "CurrentRound":
                                var currentDate = DateTime.Now.Date;
                                var avaiableRoundId = rounds.FirstOrDefault(s => s.StartTime <= currentDate && s.EndTime >= currentDate)?.PKID ?? 0;
                                if (avaiableRoundId > 0)
                                {
                                    roundIds.Add(avaiableRoundId);
                                }
                                else
                                {
                                    throw new Exception("当前没有有效场次");
                                }
                                break;
                            case "NextRound":
                                var nowDate = DateTime.Now.Date;
                                var notOverdueRound = rounds.Where(s => s.EndTime >= nowDate).OrderBy(s => s.StartTime);
                                if (notOverdueRound.Count() > 1)
                                {
                                    var nextId = notOverdueRound.Skip(1).Take(1).First().PKID;
                                    roundIds.Add(nextId);
                                }
                                else
                                {
                                    throw new Exception("没有下一个场次");
                                }
                                break;
                        }
                        if (roundIds.Any())
                        {
                            var operateUser = HttpContext.User.Identity.Name;
                            result = manager.BatchImportBankMRActivityUsers(userRules, roundIds, operateUser, importRoundType, note);
                        }
                    }
                    else
                    {
                        msg = "规则不能为空";
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }

            return Json(new { Status = result, Msg = msg });
        }

        private static string GetRepeatDataRemark(List<BankMRActivityUser> userRules)
        {
            List<int> indexs = new List<int>();
            for (int i = 1, count = userRules.Count(); i <= count; i++)
            {
                var item = userRules[i - 1];
                var index = i;
                var ex = userRules.FindAll(x => x.BankCardNum == item.BankCardNum &&
               x.BatchCode == item.BatchCode &&
               x.DayLimit == item.DayLimit &&
               x.LimitCount == item.LimitCount &&
               x.Mobile == item.Mobile &&
               x.OtherNum == item.OtherNum);
                if (ex.Count() > 1)
                    indexs.Add(++index);

            }
            if (indexs.Any())
                return $"第{string.Join("^", indexs)}行存在重复数据！";
            return string.Empty;
        }

        private List<Tuple<string, string>> GetBankRuleExcelColumn(string verifyType, string otherVerifyName)
        {
            var verifyTypeArray = verifyType.Split(',');
            var title = new List<Tuple<string, string>>();
            for (var i = 0; i < verifyTypeArray.Length; i++)
            {
                switch (verifyTypeArray[i])
                {
                    case "Moble1":
                        title.Add(new Tuple<string, string>("手机号", "Moble1"));
                        break;
                    case "Other1":
                    case "LicenseNo":
                    case "ETC":
                        title.Add(new Tuple<string, string>("其他号(如:ETC,车牌号)", "Other1"));
                        break;
                    case "Card1":
                        title.Add(new Tuple<string, string>("银行卡前N后M（中间*号隔开）", "Card1"));
                        break;
                    case "Card2":
                        title.Add(new Tuple<string, string>("卡BIN", "Card2"));
                        break;
                    case "Card3":
                        title.Add(new Tuple<string, string>("卡号", "Card3"));
                        break;
                    case "Other":
                        title.Add(new Tuple<string, string>(otherVerifyName, "Other"));
                        break;
                }
            }
            title.Add(new Tuple<string, string>("场次限购", "LimitCount"));
            title.Add(new Tuple<string, string>("每日限购", "DayLimit"));

            return title;
        }

        [HttpGet]
        public ActionResult GetAllVipService()
        {
            var allVipService = new Dictionary<string, string>();

            using (var client = new ShopClient())
            {
                var vipServiceIds = (client.GetBeautyProductIdsByCategoryId(67)).Result;
                if (vipServiceIds != null)
                {
                    foreach (var item in vipServiceIds)
                    {
                        var product = (client.GetBeautyProductDetailByPid(item)).Result;
                        allVipService[item] = product.ProductName;
                    }
                }
            }

            return Json(allVipService, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有的大客户母公司
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAllVipCompany()
        {
            using (var client = new UserAccountClient())
            {
                var serviceResult = client.SelectCompanyInfoById(-1);
                serviceResult.ThrowIfException(true);
                var allVipCompany = serviceResult.Result;
                return Json(allVipCompany, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 根据公司ID获取公司用户
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCompanyUsersByCompanyId(int companyId)
        {
            using (var client = new UserAccountClient())
            {
                var serviceResult = client.GetCompanyUsersByCompanyId(companyId);
                serviceResult.ThrowIfException(true);
                var companyUsers = serviceResult.Result;
                return Json(companyUsers, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetAllProvince()
        {
            using (var client = new RegionClient())
            {
                var allProvinceResult = client.GetAllProvince();
                allProvinceResult.ThrowIfException(true);
                return Json(allProvinceResult.Result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetRegionByRegionId(int regionId)
        {
            var result = manager.GetRegionByRegionId(regionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadActivityBannerImage()
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

            return Json(new { Status = result, ImageUrl = imageUrl, Msg = msg });
        }
        [HttpGet]
        public FileResult GetImportBankMRActivityUsers(string batchCode,string otherVerifyName)
        {
            var result = manager.GetImportBankMRActivityUsers(batchCode);
            var ms = new MemoryStream();
            #region excel
            var workbool = new XSSFWorkbook();//工作簿
            var sheet = workbool.CreateSheet(batchCode);//sheet

            var temp = result?.GroupBy(g => g.ActivityRoundId).ToDictionary(s => s.Key, v => v).FirstOrDefault();

            var bookName = temp.Value.Value.FirstOrDefault().BatchCode;

            var cloums = GetBankRuleExcelColumn(temp.Value.Value.FirstOrDefault().VerifyType, otherVerifyName);
            var row = sheet.CreateRow(0);
            int index = 0;
            foreach (var item in cloums)
            {
                switch (item.Item2)
                {
                    case "Moble1":
                        row.CreateCell(index, CellType.String).SetCellValue("手机号"); break;
                    case "Other1":
                    case "LicenseNo":
                    case "ETC":
                        row.CreateCell(index, CellType.String).SetCellValue("其他号(如:ETC,车牌号)"); break;
                    case "Card1":
                        row.CreateCell(index, CellType.String).SetCellValue("银行卡前N后M（中间*号隔开）"); break;
                    case "Card2":
                        row.CreateCell(index, CellType.String).SetCellValue("卡BIN"); break;
                    case "Card3":
                        row.CreateCell(index, CellType.String).SetCellValue("卡号"); break;
                    case "LimitCount":
                        row.CreateCell(index, CellType.String).SetCellValue("场次限购"); break;
                    case "DayLimit":
                        row.CreateCell(index, CellType.String).SetCellValue("每日限购"); break;
                    case "Other":
                        row.CreateCell(index, CellType.String).SetCellValue(otherVerifyName); break;
                    default:
                        break;
                }
                index++;
            }
            int index2 = 1;
            temp.Value.Value.ToArray().ForEach(f =>
            {
                var row2 = sheet.CreateRow(index2);
                for (int index3 = 0; index3 < row.Cells.Count(); index3++)
                {
                    switch (row.GetCell(index3).StringCellValue)
                    {
                        case "手机号":
                            row2.CreateCell(index3, CellType.String).SetCellValue(f.Mobile);
                            break;
                        case "其他号(如:ETC,车牌号)":
                            row2.CreateCell(index3, CellType.String).SetCellValue(f.OtherNum);
                            break;
                        case "银行卡前N后M（中间*号隔开）":
                        case "卡BIN":
                        case "卡号":
                            row2.CreateCell(index3, CellType.String).SetCellValue(f.BankCardNum);
                            break;
                        case "场次限购":
                            row2.CreateCell(index3, CellType.Numeric).SetCellValue(f.LimitCount ?? 0);
                            break;
                        case "每日限购":
                            row2.CreateCell(index3, CellType.Numeric).SetCellValue(f.DayLimit ?? 0);
                            break;
                        default:
                            row2.CreateCell(index3, CellType.String).SetCellValue(f.OtherNum);
                            break;
                    }
                }
                index2++;
            });
            workbool.Write(ms);
            #endregion
            return File(ms.ToArray(), "application/x-xls", $"{bookName}.xlsx");
        }
        [HttpPost]
        public JsonResult GetImportBankMRActivityUsersRecords(string activityId, int pageIndex, int pageSize)
        {
            var jsonresult = new List<ImportBankMrActivityUsersRecordModel>();
            if (string.IsNullOrWhiteSpace(activityId))
                return Json(new { Result = jsonresult, Msg = "activityId参数不存在" });
            int count = 0;
            var result = manager.GetImportBankMRActivityUsersRecords(new Guid(activityId), out count);
            result?.Where(w => !string.IsNullOrWhiteSpace(w.BatchCode)).GroupBy(g => g.BatchCode).ToDictionary(x => x.Key, v => v).ForEach(f =>
              {
                  var temp = new ImportBankMrActivityUsersRecordModel()
                  {
                      BatchCode = f.Key,
                      CreateTime = f.Value.FirstOrDefault().CreateTime,
                      OperateUser = f.Value.FirstOrDefault().OperateUser,
                      RoundTime = f.Value.OrderBy(o => o.StartTime).Select(s => new ImportRoundTime { StartTime = s.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), EndTime = s.EndTime.ToString("yyyy-MM-dd HH:mm:ss") }),
                      Note = f.Value.FirstOrDefault().Remarks?.Split(',').LastOrDefault()
                  };
                  jsonresult.Add(temp);
              });
            return Json(new { Result = jsonresult.Skip((pageIndex - 1) * pageSize).Take(pageSize), Total = count, Msg = "" });
        }
        [HttpPost]
        public JsonResult DeleteImportBankMRActivityUsers(string batchCode)
        {
            if (string.IsNullOrEmpty(batchCode))
                return Json(new { Result = false, Msg = "batchCode参数不存在" });
            var result = manager.DeleteImportBankMRActivityByBatchCode(batchCode, User.Identity.Name);
            return Json(new { Result = result.Item1, Msg = result.Item2 });
        }

        public JsonResult GetAllBeautyServiceCodeTypeConfig()
        {
            var result = new Dictionary<string, string>();
            var temp = BeautyServicePackageManager.SelectAllBeautyServiceCodeTypeConfig();
            temp.ForEach(f =>
            {
                if (!result.ContainsKey(f.PID))
                    result.Add(f.PID, f.Name);
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSignleBankMRActivityUser(string activityId, string searchVerifyType, string searchVerifyTypeName)
        {
            if (string.IsNullOrEmpty(activityId))
                return Json(new { IsSuccess = false, Msg = "activityId参数不存在" });
            if (string.IsNullOrEmpty(searchVerifyType))
                return Json(new { IsSuccess = false, Msg = "SearchVerifyType参数不存在" });
            if (string.IsNullOrEmpty(searchVerifyTypeName))
                return Json(new { IsSuccess = false, Msg = "SearchVerifyTypeName参数不存在" });
            string mobile = null; string card = null; string otherNo = null;
            switch (searchVerifyType)
            {
                case "Moble1": mobile = searchVerifyTypeName; break;
                case "Card1":
                case "Card2": card = searchVerifyTypeName; break;
                case "ETC":
                case "LicenseNo": otherNo = searchVerifyTypeName; break;
            }

            var result = manager.GetSignleBankMRActivityUser(new Guid(activityId), mobile, card, otherNo);
            return Json(new { IsSuccess = true, Result = result, Msg = "" });
        }

        [HttpGet]
        public ActionResult BankActivityGroupConfig()
        {
            return View();
        }

        /// <summary>
        /// 分页获取银行活动组配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetBankActivityGroupConfigs(Guid? groupId, int pageIndex = 1, int pageSize = 10)
        {
            Tuple<IEnumerable<BankActivityGroupConfig>, int> result = null;
            if (groupId == null || groupId == Guid.Empty)
            {
                result = manager.SelectBankActivityGroupConfigs(pageIndex, pageSize);
            }
            else
            {
                result = manager.SelectBankActivityGroupConfigByGroupId(groupId.Value, pageIndex, pageSize);
            }
            return Json(new { result = result.Item1.ToList(), total = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpsertBankActivityGroupConfig(Guid groupId, string groupName)
        {
            ViewBag.GroupId = groupId;
            ViewBag.GroupName = groupName;
            return View();
        }
        /// <summary>
        /// 检查银行活动组名是否已存在
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckDupBankActivityGroupName(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return Json(new { IsSuccess = false, Msg = "组名不能为空" });
            }
            var result = manager.SelectBankActivityGroupConfigsByGroupName(groupName);
            if (result.Any())
            {
                return Json(new { IsSuccess = false, Msg = "组名已存在" });
            }
            else
            {
                return Json(new { IsSuccess = true, Msg = "可用组名" });
            }
        }

        [HttpPost]
        public JsonResult UpsertBankActivityGroupName(Guid? groupId, string groupName)
        {
            if (groupId == null && !string.IsNullOrWhiteSpace(groupName))
            {
                var operateUser = HttpContext.User.Identity.Name;
                var config = new BankActivityGroupConfig() { GroupName = groupName, CreateUser = operateUser };
                var result = manager.InsertBankActivityGroupConfig(config, operateUser);
                return Json(new { IsSuccess = result, Msg = "插入成功" });
            }
            if (groupId == Guid.Empty || string.IsNullOrWhiteSpace(groupName))
            {
                return Json(new { IsSuccess = false, Msg = "参数不完整" });
            }
            else
            {
                var result =
                    manager.UpdateBankActivityGroupNameByGroupId(groupId.Value, groupName, HttpContext.User.Identity.Name);
                return Json(new { IsSuccess = result, Msg = "更新成功" });
            }
        }

        /// <summary>
        /// 插入或更银行活动组配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpsertBankActivityGroupConfig(BankActivityGroupConfig config)
        {
            var result = false;

            if (config != null)
            {
                var activityIds = manager.SelectBankActivityGroupConfigByGroupId(config.GroupId, 1, int.MaxValue).Item1.Select(i => i.ActivityId);
                if (!activityIds.Contains(config.ActivityId))
                {
                    var operateUser = HttpContext.User.Identity.Name;
                    if (config.PKID > 0)
                    {
                        result = manager.UpdateBankActivityGroupConfigByPKID(config, operateUser);
                        return Json(new { IsSuccess = true, Result = result, Msg = "更新成功" });
                    }
                    else
                    {
                        config.CreateUser = operateUser;
                        result = manager.InsertBankActivityGroupConfig(config, operateUser);
                        return Json(new { IsSuccess = true, Result = result, Msg = "成功" });
                    }
                }
                else
                {
                    return Json(new { IsSuccess = false, Result = result, Msg = "失败，该组下已存在该活动Id" });
                }
            }
            return Json(new { IsSuccess = false, Result = result, Msg = "失败，必要参数为空" });
        }

        [HttpPost]
        public ActionResult DeleteBankActivityGroupConfig(int pkid)
        {
            var result = false;
            if (pkid <= 0)
            {
                return Json(new { IsSuccess = false, Result = result, Msg = "pkid不能为空" });
            }
            result = manager.DeleteBankActivityGroupConfigByPKID(pkid, HttpContext.User.Identity.Name);
            return Json(new { IsSuccess = true, Result = result, Msg = "成功" });
        }
        /// <summary>
        /// 根据活动Id查询银行美容活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetBankMRActivityConfigByActivityId(string activityId)
        {
            if (!Guid.TryParse(activityId, out Guid g) || g == Guid.Empty)
            {
                return Json(new { IsSuccess = false, Msg = "activityId参数不存在" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.FetchBankMRActivityConfigByActivityId(g);
            return Json(new { IsSuccess = true, Result = result, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportWhiteUsers(string groupId, int groupConfigId)
        {
            bool result = false;
            if (string.IsNullOrWhiteSpace(groupId) || !Guid.TryParse(groupId, out Guid gid) || gid == Guid.Empty || groupConfigId <= 0)
            {
                return Json(new { IsSuccess = false, Result = result, Msg = "参数错误" });
            }
            var files = Request.Files;
            var file = files[0];
            if (files.Count <= 0 || file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { IsSuccess = false, Result = result, Msg = "请上传Excel文件" });
            }
            else
            {
                var stream = file.InputStream;
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                var sheet = workBook.GetSheetAt(0);
                var excelDataDic = new Dictionary<string, string>();//CardNum为Key，Mobile为Value
                var repeatCardNumList = new Dictionary<string, int>();
                var alreadyImportList = manager.SelectImportBankActivityWhiteUsersByGroupId(gid);
                var alreadyImportCardNumList = alreadyImportList.ToDictionary(i => i.CardNum, i => i.GroupConfigId);//.Select(i => i.CardNum).ToList();
                try
                {
                    var whiteUsers = new List<BankActivityWhiteUsers>();
                    for (var rowIndex = sheet.FirstRowNum; rowIndex <= sheet.LastRowNum; rowIndex++) //遍历每一行
                    {
                        var row = sheet.GetRow(rowIndex + 2);
                        if (row == null) continue;
                        var cardNum = row.Cells[0].ToString().Trim(); //银行卡
                        var mobile = row.Cells[1].ToString().Trim(); //手机号
                        if (string.IsNullOrWhiteSpace(cardNum) || string.IsNullOrWhiteSpace(mobile))
                        {
                            return Json(new { IsSuccess = false, Result = result, Msg = "必要列不能为空" });
                        }
                        if (!cardNum.Contains("*") || cardNum.Split('*')[0].Length <= 0 ||
                                                      cardNum.Split('*')[1].Length <= 0)
                        {
                            return Json(new { IsSuccess = false, Result = result, Msg = "银行卡格式不满足要求" });
                        }
                        if (mobile.Length != 11 || int.TryParse(mobile, out int m))
                        {
                            return Json(new { IsSuccess = false, Result = result, Msg = "手机号格式不正确" });
                        }
                        excelDataDic[cardNum] = mobile;
                    }
                    excelDataDic.ForEach(i =>
                    {
                        if (!alreadyImportCardNumList.ContainsKey(i.Key))
                        {
                            whiteUsers.Add(new BankActivityWhiteUsers
                            {
                                GroupConfigId = groupConfigId,
                                CardNum = i.Key,
                                Mobile = i.Value
                            });
                        }
                        else
                        {
                            repeatCardNumList[i.Key] = alreadyImportCardNumList[i.Key];
                        }
                    });
                    result = manager.BatchImportBankActivityWhiteUsers(whiteUsers, HttpContext.User.Identity.Name);
                    if (repeatCardNumList.Any())
                    {
                        var sb = new StringBuilder();
                        foreach (var cardNum in repeatCardNumList)
                        {
                            sb.AppendLine(
                                $"卡号：{cardNum.Key}，已绑定活动Id：{manager.SelectBankActivityGroupConfigByPKID(alreadyImportCardNumList[cardNum.Key]).ActivityId}");
                        }
                        return Json(new { IsSuccess = true, Result = result, Msg = $"导入成功 {whiteUsers.Count} 条,其中有 {repeatCardNumList.Count} 条数据卡号已在该组下绑定其他活动活动。\r\n {sb}" });
                    }
                    else
                    {
                        return Json(new { IsSuccess = true, Result = result, Msg = $"导入成功,{whiteUsers.Count} 条" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { IsSuccess = false, Result = result, Msg = ex.Message });
                }
            }
        }

        /// <summary>
        /// 下载导入银行活动组白名单模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public FileResult DownloadImportBankActivityGroupWhiteUsersTemplate()
        {
            var ms = new MemoryStream();
            var workbook = new XSSFWorkbook(); //工作簿
            var sheet = workbook.CreateSheet(); //sheet
            var row0 = sheet.CreateRow(0);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 22));
            row0.CreateCell(0, CellType.String).SetCellValue("注：卡号必须是前m后n中间用*隔开形式。如：6217*0017");
            var row1 = sheet.CreateRow(1);
            row1.CreateCell(0, CellType.String).SetCellValue("CardNum");
            row1.CreateCell(1, CellType.String).SetCellValue("Mobile");
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"银行活动白名单导入模板{DateTime.Now:yyyy-MM-dd}.xlsx");
        }

        [HttpGet]
        public FileResult DownloadImportBankActivityGroupWhiteUsers(int groupConfigId)
        {
            var result = manager.SelectImportBankActivityWhiteUsersByGroupConfigId(groupConfigId);
            var ms = new MemoryStream();
            var workbook = new XSSFWorkbook(); //工作簿
            var sheet = workbook.CreateSheet(); //sheet
            var row0 = sheet.CreateRow(0);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 22));
            row0.CreateCell(0, CellType.String).SetCellValue("注：卡号必须是前m后n中间用*隔开形式。如：6217*0017");
            int index = 1;
            var row1 = sheet.CreateRow(1);
            row1.CreateCell(index - 1, CellType.String).SetCellValue("CardNum");
            row1.CreateCell(index, CellType.String).SetCellValue("Mobile");
            foreach (var item in result)
            {
                var rowIndex = sheet.CreateRow(index + 1);
                rowIndex.CreateCell(0, CellType.String).SetCellValue(item.CardNum);
                rowIndex.CreateCell(1, CellType.String).SetCellValue(item.Mobile);
                index++;
            }
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"银行活动白名单{DateTime.Now:yyyy-MM-dd}.xlsx");
        }

        #region 银行活动列表
        public ActionResult BankMrActivityDisplayConfig()
        {
            return View();
        }
        /// <summary>
        /// 获取银行活动展示列表配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public JsonResult GetBankMrActivityDisplayConfigs(int pageIndex, int pageSize, int active)
        {
            if (pageIndex <= 0 || pageSize <= 0 || pageSize >= 50)
                return Json(new { Success = false, Msg = "请求参数不符合要求" });
            var temp = manager.SelectBankMrActivityDisplayConfigs(pageIndex, pageSize, active);
            return Json(new { Success = true, Total = temp.Item1, Result = temp.Item2, Msg = "" });
        }
        /// <summary>
        /// 银行活动展示列表配置详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public JsonResult GetBankMrActivityDisplayDetail(int pkid)
        {
            if (pkid <= 0)
                return Json(new { Success = true, Result = new BankMrActivityDisplayConfigEntity { } });
            var temp = manager.GetBankMrActivityDisplayConfigsDetail(pkid);
            return Json(new { Success = true, Result = temp, Msg = "" });
        }
        /// <summary>
        /// 保存银行活动展示列表配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveBankMrActivityDisplayConfig(BankMrActivityDisplayConfigEntity config)
        {
            if (config == null)
                return Json(new { Success = false, Msg = "请求参数不正确" });
            var result = false;
            var msg = string.Empty;
            try
            {
                if (config.PKID > 0)
                {
                    var oldModel = manager.GetBankMrActivityDisplayConfigsDetail(config.PKID);
                    result = manager.UpdateBankMrActivityDisplayConfig(config, config.Region);
                    var log = new BeautyOprLog
                    {
                        LogType = "SaveBankMrActivityDisplayConfig",
                        IdentityID = $"{config.PKID}",
                        OldValue = JsonConvert.SerializeObject(oldModel),
                        NewValue = JsonConvert.SerializeObject(config),
                        Remarks = $"更新大客户银行活动展示配置",
                        OperateUser = User.Identity.Name,
                    };
                    LoggerManager.InsertLog("BeautyOprLog", log);
                }
                else
                {
                    config.CreatedUser = User.Identity.Name;
                    result = manager.SaveBankMrActivityDisplayConfig(config, config.Region);
                    var log = new BeautyOprLog
                    {
                        LogType = "SaveBankMrActivityDisplayConfig",
                        IdentityID = $"{config.PKID}",
                        OldValue = "",
                        NewValue = JsonConvert.SerializeObject(config),
                        Remarks = $"新增大客户银行活动展示配置",
                        OperateUser = User.Identity.Name,
                    };
                    LoggerManager.InsertLog("BeautyOprLog", log);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new { Success = result, Msg = msg });
        }
        #endregion

        #region 银行活动使用记录

        public ActionResult BankMrUsageRecord()
        {
            return View();
        }
        /// <summary>
        /// 通过参数获取信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mobile"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult SelectBankMrActivityUsageRecode(string code, string mobile, int pageIndex = 1, int pageSize = 10)
        {
            var result = manager.SelectBankMrActivityUsageRecode(code, mobile, pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 通过PKID获取信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonResult SearchBankMrActivityInfoByType(int pkid, string type)
        {
            var result = manager.SelectBankMrAtivityCodeRecordByPKID(pkid, type, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}