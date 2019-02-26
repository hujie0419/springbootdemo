using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BeautyService;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.GeneralBeautyServerCode;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.UnivRedemptionCode;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode;
using Tuhu.Service.Product;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;
using Tuhu.Service.Utility;

namespace Tuhu.Provisioning.Controllers
{
    public class BeautyServicePackageController : Controller
    {
        private static bool IsOnline = ConfigurationManager.AppSettings["env"] == "pro";

        [PowerManage]
        public ActionResult PackageCode()
        {
            return View();
        }

        [PowerManage]
        public ActionResult ServiceCode()
        {
            return View();
        }

        [PowerManage]
        public ActionResult ServiceCodeForImport()
        {
            return View();
        }

        [PowerManage]
        public ActionResult PackageDetail(int packageId, string packageType, bool isPackageCodeGenerated)
        {
            ViewBag.PackageType = packageType;
            ViewBag.PackageId = packageId;
            ViewBag.IsPackageCodeGenerated = isPackageCodeGenerated;
            return View();
        }

        [PowerManage]
        public ActionResult LimitConfig(int packageDetailId)
        {
            ViewBag.PackageDetailId = packageDetailId;
            return View();
        }

        [PowerManage]
        public ActionResult ServiceCodeTypeConfig()
        {
            return View();
        }

        /// <summary>
        /// 服务码查询页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchCode()
        {
            return View();
        }
        /// <summary>
        /// 根据服务码或手机号查询服务码详情
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public async Task<ActionResult> SearchServiceCode(string serviceCode, string mobile, int pageIndex = 1, int pageSize = 1000, string tabValue = "BeautyCode")
        {
            var log = new DataAccess.Entity.BeautyOprLog
            {
                LogType = "SearchServiceCode",
                IdentityID = $"{serviceCode}|{mobile}",
                OldValue = null,
                NewValue = null,
                Remarks = $"根据服务码[{serviceCode}]或手机号[{mobile}]查询服务码详情[{tabValue}]",
                OperateUser = User.Identity.Name,
            };
            LoggerManager.InsertLog("BeautyOprLog", log);
            if (String.Equals(tabValue, "BeautyCode"))
            {
                Tuple<List<ServiceCodeDetail>, int> result = null;
                result = await SearchCodeManager.GetBeautyServicePackageDetailCodes(mobile, serviceCode, pageIndex, pageSize);
                var shopDic = await SearchCodeManager.GetShopInfoByShopIds(result.Item1.Where(w => !string.IsNullOrEmpty(w.VerifyShop)).Select(s => int.Parse(s.VerifyShop)));
                if (shopDic.Any())
                    result.Item1.ForEach(f =>
                    {
                        if (!string.IsNullOrEmpty(f.VerifyShop) && shopDic.ContainsKey(f.VerifyShop))
                            f.VerifyShop = shopDic[f.VerifyShop];
                        if (!string.IsNullOrEmpty(f.OrderNo))
                        {
                            f.OrderNoLink = IsOnline ?
                                            "https://oms.tuhu.cn/Order/Details/" + f.OrderNo.Remove(0, 2) :
                                            "https://oms.tuhu.work/Order/Details/" + f.OrderNo.Remove(0, 2);
                        }

                    });
                return Json(new { Status = true, Data = result.Item1, TotalCount = result.Item2, TotalPage = 0 }, JsonRequestBehavior.AllowGet);
            }
            else if (String.Equals(tabValue, "GeneralServiceCode"))
            {
                var result = await GeneralBeautyServerCodeManager.GetGeneralBeautyServerCodes(mobile, serviceCode, pageIndex, pageSize);
                return Json(new { Status = true, Data = result, TotalCount = 0, TotalPage = 0 }, JsonRequestBehavior.AllowGet);
            }
            else if (String.Equals(tabValue, "GeneralRedemtionCode"))
            {
                var result = await UnivRedemptionCodeManager.GetRedeemMrServerCodes(mobile, serviceCode, pageIndex, pageSize);
                return Json(new { Status = true, Data = result, TotalCount = 0, TotalPage = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = true, Data = new List<ServiceCodeDetail>(), TotalCount = 0, TotalPage = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBeautyServicePackage(int pageIndex, int pageSize, string packageType = "",
            string packageName = "", string vipCompanyName = "", string settleMentMethod = "", int cooperateId = 0)
        {
            var result = BeautyServicePackageManager.GetBeautyServicePackage(pageIndex, pageSize, packageType,
                packageName, vipCompanyName, settleMentMethod, cooperateId);
            var status = result != null && result.Item1 != null && result.Item1.Any() ? "Success" : "fail";
            if (string.Equals(status, "Success"))
            {
                var manager = new BankMRManager();
                var allCooperateUsers = manager.GetAllMrCooperateUserConfigs();
                if (allCooperateUsers != null && allCooperateUsers.Any())
                    result.Item1?.ToList().ForEach(u =>
                    {
                        u.CooperateName = allCooperateUsers.FirstOrDefault(t => t.PKID == u.CooperateId)?.CooperateName;
                    });
            }

            return Json(new { Status = status, Data = result.Item1, TotalCount = result.Item2 },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBeautyServicePackageDetails(int pageIndex, int pageSize,
            bool isImportUser, string settlementMethod, int cooperateId, string serviceId)
        {
            var result = BeautyServicePackageManager.GetBeautyServicePackageDetails(pageIndex, pageSize, isImportUser, settlementMethod,
                    cooperateId, serviceId);
            if (result != null && result.Item1.Any())
            {
                var allCooperateUsers = new BankMRManager().GetAllMrCooperateUserConfigs();
                if (allCooperateUsers != null && allCooperateUsers.Any())
                    result.Item1?.ToList().ForEach(u =>
                    {
                        u.CooperateName = allCooperateUsers.FirstOrDefault(t => t.PKID == u.CooperateId)?.CooperateName;
                    });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllVipUserName()
        {
            var result = BeautyServicePackageManager.GetAllVipUserName();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBeautyServicePackageDetail(int packageId)
        {
            var result = BeautyServicePackageManager.GetBeautyServicePackageDetails(packageId);
            var status = result != null && result.Any() ? "Success" : "fail";
            return Json(new { Status = status, Data = result }, JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        [HttpPost]
        public ActionResult UpsertBeautyServicePackage(BeautyServicePackage package)
        {
            bool isSuccess = false;
            var msg = string.Empty;
            var user = HttpContext.User.Identity.Name;
            if (package != null && !string.IsNullOrEmpty(package.PackageName) && package.CooperateId > 0)
            {
                var manager = new BankMRManager();
                var cooperateUser = manager.FetchMrCooperateUserConfigByPKID(package.CooperateId);
                if (cooperateUser != null)
                {
                    package.VipUserId = cooperateUser.VipUserId;
                    using (var client = new UserAccountClient())
                    {
                        var userServiceResult = client.SelectCompanyUserInfo(cooperateUser.VipUserId);
                        if (userServiceResult.Success && userServiceResult.Result != null)
                        {
                            package.VipUserName = userServiceResult.Result.UserName;
                            if (userServiceResult.Result.CompanyInfo != null)
                            {
                                package.VipCompanyId = userServiceResult.Result.CompanyInfo.Id;
                                package.VipCompanyName = userServiceResult.Result.CompanyInfo.Name;
                            }
                        }
                    }
                }

                if (package.PKID > 0)
                {
                    package.UpdateUser = user;
                    isSuccess = BeautyServicePackageManager.UpdateBeautyServicePackage(package);
                }
                else
                {
                    package.CreateUser = user;
                    isSuccess = BeautyServicePackageManager.InsertBeautyServicePackage(package);
                }
                if (!isSuccess)
                {
                    msg = "更新失败";
                }
                else
                {
                    msg = "成功";
                }
            }
            else
            {
                msg = "信息不完善";
            }

            return Json(new { IsSuccess = isSuccess, Msg = msg }, JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        [HttpPost]
        public ActionResult UpsertBeautyServicePackageDetail(BeautyServicePackageDetail packageDetail)
        {
            var isSuccess = false;
            var msg = string.Empty;
            var rawPackage = BeautyServicePackageManager.GetBeautyServicePackage(packageDetail.PackageId);
            if (rawPackage != null && rawPackage.IsPackageCodeGenerated)
            {
                if (packageDetail.PKID <= 0)
                {
                    msg = "已生成兑换码不允许添加产品！";
                    return Json(new { IsSuccess = false, Msg = msg }, JsonRequestBehavior.AllowGet);
                }
                var rawPackageDetail = BeautyServicePackageManager.GetBeautyServicePackageDetails(packageDetail.PackageId).FirstOrDefault();
                if (rawPackageDetail != null &&
                    (rawPackageDetail.ShopCommission != packageDetail.ShopCommission ||
                     rawPackageDetail.VipSettlementPrice != packageDetail.VipSettlementPrice ||
                     rawPackageDetail.ServiceCodeNum != packageDetail.ServiceCodeNum ||
                     rawPackageDetail.ServiceCodeStartTime != packageDetail.ServiceCodeStartTime ||
                     rawPackageDetail.ServiceCodeEndTime != packageDetail.ServiceCodeEndTime ||
                     rawPackageDetail.EffectiveDayAfterExchange != packageDetail.EffectiveDayAfterExchange ||
                     rawPackageDetail.Num != packageDetail.Num))
                {
                    msg = "已生成兑换码只允许修改名称！";
                    return Json(new { IsSuccess = false, Msg = msg }, JsonRequestBehavior.AllowGet);
                }
            }

            var user = HttpContext.User.Identity.Name;
            if (packageDetail != null && !string.IsNullOrEmpty(packageDetail.Name) && !string.IsNullOrEmpty(packageDetail.PID))
            {
                if (packageDetail.Num > 20 || packageDetail.Num < 0)
                {
                    msg = "兑换码包含的服务数量介于0~20之间";
                    return Json(new { IsSuccess = false, Msg = msg }, JsonRequestBehavior.AllowGet);
                }
                if (packageDetail.CooperateId <= 0)//兑换码的合作用户配置在外层
                {
                    var package = BeautyServicePackageManager.GetBeautyServicePackage(packageDetail.PackageId);
                    packageDetail.CooperateId = package?.CooperateId ?? 0;
                }
                if (packageDetail.PKID > 0)
                {
                    packageDetail.UpdateUser = user;
                    var updateResult = BeautyServicePackageManager.UpdateBeautyServicePackageDetail(packageDetail);
                    isSuccess = updateResult.Item1;
                    msg = updateResult.Item2;
                }
                else
                {
                    packageDetail.CreateUser = user;
                    isSuccess = BeautyServicePackageManager.InsertBeautyServicePackageDetail(packageDetail);
                    if (!isSuccess)
                    {
                        msg = "更新失败";
                    }
                }
            }
            else
            {
                msg = "信息不完善";
            }

            return Json(new { IsSuccess = isSuccess, Msg = msg }, JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        [HttpPost]
        public ActionResult DeleteBeautyServicePackage(int pkid)
        {
            var isSuccess = false;
            var msg = string.Empty;
            var package = BeautyServicePackageManager.GetBeautyServicePackage(pkid);
            var packageDetails = BeautyServicePackageManager.GetBeautyServicePackageDetails(pkid);
            if (package != null && packageDetails != null && !package.IsPackageCodeGenerated && !packageDetails.Any(t => t.IsServiceCodeGenerated))
            {
                var user = HttpContext.User.Identity.Name;
                isSuccess = BeautyServicePackageManager.DeleteBeautyServicePackage(pkid, package, user);
            }
            else if (package == null)
            {
                msg = "不存在";
            }
            else
            {
                msg = "已经生成兑换码或服务码的不允许删除";
            }

            return Json(new { IsSuccess = isSuccess, Msg = msg }, JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        [HttpPost]
        public ActionResult DeleteBeautyServicePackageDetail(int pkid,int packageId)
        {
            var isSuccess = false;
            var msg = string.Empty;
            var package = BeautyServicePackageManager.GetBeautyServicePackage(packageId);
            if (package != null && package.IsPackageCodeGenerated)
            {
                msg = "已经生成兑换码的不允许删除";
            }
            else if (package == null)
            {
                msg = "兑换码配置不存在";
            }
            else
            {
                var packageDetail = BeautyServicePackageManager.GetBeautyServicePackageDetail(pkid);
                if (packageDetail != null && !packageDetail.IsServiceCodeGenerated)
                {
                    var user = HttpContext.User.Identity.Name;
                    isSuccess = BeautyServicePackageManager.DeleteBeautyServicePackageDetail(pkid, packageDetail, user);
                }
                else if (packageDetail == null)
                {
                    msg = "不存在";
                }
                else
                {
                    msg = "已经生成服务码的不允许删除";
                }
            }

            return Json(new { IsSuccess = isSuccess, Msg = msg }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAllBeautyServiceCodeTypeConfig(int ServerType = 0)
        {
            var result = BeautyServicePackageManager.SelectAllBeautyServiceCodeTypeConfig(ServerType);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 更新或新增服务码类型配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult UpsertBeautyServiceCodeTypeConfig(BeautyServiceCodeTypeConfig config)
        {
            var isSuccess = false;
            var msg = "";
            if (config != null && !string.IsNullOrEmpty(config.PID) && !string.IsNullOrEmpty(config.Name))
            {
                config.PID = config.PID.Trim();
                var product = BeautyProductManager.GetBeautyProductByPid(config.PID);
                var vipCategoryIds = BeautyProductManager.GetBeautyChildAndSelfCategoryIdsByCategoryId(67);
                if (product != null && vipCategoryIds != null && vipCategoryIds.Contains(product.CategoryId))
                {
                    isSuccess = config.PKID > 0 ? BeautyServicePackageManager.UpdateBeautyServiceCodeTypeConfig(config)
                                        : BeautyServicePackageManager.InsertBeautyServiceCodeTypeConfig(config);
                    msg = isSuccess ? "插入成功" : "更新失败";
                }
                else
                {
                    msg = "产品PID不正确";
                }
            }
            else
            {
                msg = "数据不完善";
            }

            return Json(new { IsSuccess = isSuccess, Msg = msg }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取限购配置
        /// </summary>
        /// <param name="packageDetailId"></param>
        /// <returns></returns>
        public ActionResult GetBeautyServicePackageLimitConfig(int packageDetailId)
        {
            BeautyServicePackageLimitConfig config = null;
            var msg = string.Empty;
            if (packageDetailId > 0)
            {
                var limitConfigManager = new LimitConfigManager(new BeautyServicePackageLimitConfig() { PackageDetailId = packageDetailId });
                config = limitConfigManager.GetBeautyServicePackageLimitConfig();
            }
            else
            {
                msg = "参数错误";
            }

            return Json(new { Data = config, Msg = msg }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 更新限购配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult UpsertBeautyServicePackageLimitConfig(BeautyServicePackageLimitConfig config)
        {
            var result = false;
            var msg = string.Empty;
            //if (config != null && config.PackageDetailId > 0 && ((!string.IsNullOrEmpty(config.CycleType) && config.CycleLimit > 0)
            //    || !string.IsNullOrEmpty(config.ProvinceIds)))
            if (config != null && config.PackageDetailId > 0)
            {
                config.CycleLimit = config.CycleLimit;
                var limitConfigManager = new LimitConfigManager(config);
                var oldValue = limitConfigManager.GetBeautyServicePackageLimitConfig();
                if (config.PKID > 0)
                {
                    result = limitConfigManager.UpdateBeautyServicePackageLimitConfig();
                }
                else
                {
                    result = limitConfigManager.InsertBeautyServicePackageLimitConfig();
                }
                var log = new BeautyOprLog
                {
                    LogType = config.PKID > 0 ? "UpsertBeautyServicePackageLimitConfig" : "InsertBeautyServicePackageLimitConfig",
                    IdentityID = $"{config?.PKID}",
                    OldValue = JsonConvert.SerializeObject(oldValue),
                    NewValue = JsonConvert.SerializeObject(config),
                    Remarks = $"修改限购次数,ID为:{config?.PKID}",
                    OperateUser = HttpContext.User.Identity.Name
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            else
            {
                msg = "配置信息不全";
            }
            return Json(new { Status = result, Msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllVipCompany()
        {
            using (var client = new UserAccountClient())
            {
                var serviceReult = await client.SelectCompanyInfoByIdAsync(-1);
                serviceReult.ThrowIfException(true);
                return Json(serviceReult.Result, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetUsersByCompanyId(int companyId)
        {
            IEnumerable<SYS_CompanyUser> result = null;

            using (var client = new UserAccountClient())
            {
                var serviceResult = await client.GetCompanyUsersByCompanyIdAsync(companyId);
                serviceResult.ThrowIfException(true);
                result = serviceResult.Result;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetProductByPid(string pid)
        {
            Service.Product.Models.ProductModel product = null;

            using (var productClient = new ProductClient())
            {
                var productResult = await productClient.FetchProductAsync(pid);
                productResult.ThrowIfException(true);
                product = productResult.Result;
            }

            return Json(product, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> GeneratePackageCode(int packageId)
        {
            var isSuccess = false;
            var msg = string.Empty;
            var packageDetail = BeautyServicePackageManager.GetBeautyServicePackageDetails(packageId);
            if (packageDetail != null && packageDetail.Any())
            {
                var packageCodes = BeautyServicePackageManager.GetBeautyServicePackageCodesByPackageId(packageId);
                if (packageCodes != null && packageCodes.Any())
                {
                    msg = "之前已经生成兑换码了";
                }
                else
                {
                    isSuccess = await BeautyServicePackageManager.GeneratePackageCodes(packageId);
                    if (!isSuccess)
                    {
                        msg = "生成兑换码失败";
                    }
                    else
                    {
                        isSuccess = true;
                        msg = "兑换码生成成功";
                    }
                }
            }
            else
            {
                msg = "礼包下没有产品";
            }

            return Json(new { IsSuccess = isSuccess, Msg = msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> GenerateServiceCode(int packageDetailId)
        {
            var isSuccess = false;
            var msg = string.Empty;
            var serviceCodes = BeautyServicePackageManager.GetBeautyServicePackageDetailCodesByPackageDetailId(packageDetailId);
            if (serviceCodes != null && serviceCodes.Any())
            {
                msg = "之前已经生成服务码了";

            }
            else
            {
                isSuccess = await BeautyServicePackageManager.GenerateServiceCodes(packageDetailId);
                if (!isSuccess)
                {
                    msg = "生成服务码失败";
                }
                else
                {
                    isSuccess = true;
                    msg = "服务码生成成功";
                }
            }

            return Json(new { IsSuccess = isSuccess, Msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetServiceCodeGenerateingRate(int packageDetailId)
        {
            var result = await BeautyServicePackageManager.GetServiceCodeGenerateingRate(packageDetailId);

            return Json(Math.Round(result, 4).ToString("#0.00"), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetPackageCodeGenerateingRate(int packageId)
        {
            var result = await BeautyServicePackageManager.GetPacakgeCodeGenerateingRate(packageId);

            return Json(Math.Round(result, 4).ToString("#0.00"), JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        public FileResult ExportServiceCodes(int packageDetailId, string name)
        {
            var serviceCodes = BeautyServicePackageManager.GetBeautyServicePackageDetailCodesByPackageDetailId(packageDetailId);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;

            #region title

            row.CreateCell(cellNum++).SetCellValue("服务码");
            row.CreateCell(cellNum++).SetCellValue("开始时间");
            row.CreateCell(cellNum++).SetCellValue("结束时间");

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);

            #endregion
            #region content
            if (serviceCodes != null && serviceCodes.Any())
            {
                var i = 0;
                foreach (var item in serviceCodes)
                {
                    cellNum = 0;
                    row = sheet.CreateRow(i + 1);
                    row.CreateCell(cellNum++).SetCellValue(item.ServiceCode);
                    row.CreateCell(cellNum++).SetCellValue(item.StartTime.ToString("yyyyMMdd"));
                    row.CreateCell(cellNum++).SetCellValue(item.EndTime.ToString("yyyyMMdd"));
                    i++;
                }
            }
            #endregion

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"{name}_服务码_{DateTime.Now.ToString("yyyyMMdd HHmmss")}.xlsx");
        }

        [PowerManage]
        public FileResult ExportPackageCodes(int packageId, string name)
        {
            var packageCodes = BeautyServicePackageManager.GetBeautyServicePackageCodesByPackageId(packageId);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;

            #region title

            row.CreateCell(cellNum++).SetCellValue("兑换码");
            row.CreateCell(cellNum++).SetCellValue("开始时间");
            row.CreateCell(cellNum++).SetCellValue("结束时间");

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);

            #endregion
            #region content
            if (packageCodes != null && packageCodes.Any())
            {
                var i = 0;
                foreach (var item in packageCodes)
                {
                    cellNum = 0;
                    row = sheet.CreateRow(i + 1);
                    row.CreateCell(cellNum++).SetCellValue(item.PackageCode);
                    row.CreateCell(cellNum++).SetCellValue(item.StartTime.ToString("yyyyMMdd"));
                    row.CreateCell(cellNum++).SetCellValue(item.EndTime.ToString("yyyyMMdd"));
                    i++;
                }
            }
            #endregion

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"{name}_兑换码_{DateTime.Now.ToString("yyyyMMdd HHmmss")}.xlsx");
        }

        /// <summary>
        /// 更新美容大客户服务码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public async Task<ActionResult> UpdateBeautyServicePackageDetailCodes(ServiceCodeDetail model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ServiceCode))
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            if (model.StartTime == null || model.EndTime == null)
            {
                return Json(new { Status = false, Msg = "请选择有效期" }, JsonRequestBehavior.AllowGet);
            }

            ServiceCodeDetail codeDetail = null;
            if (model.PackageDetailCodeId > 0)
            {
                codeDetail = (await SearchCodeManager.GetServiceCodeDetail(new List<string>() { model.ServiceCode }))?.FirstOrDefault();
            }
            else
            {
                var bankRecords = BankMRManager.GetBankMRActivityCodeRecordByServiceCode(new List<string>() { model.ServiceCode });
                codeDetail = (await BankMRManager.SearchBankMRActivityCodeDetailByRecords(bankRecords))?.FirstOrDefault();
                if (codeDetail.StartTime.Date != model.StartTime.Date)
                {
                    return Json(new { Status = false, Msg = $"银行服务码开始时间不能修改" }, JsonRequestBehavior.AllowGet);
                }
            }

            if (codeDetail == null || string.IsNullOrWhiteSpace(codeDetail.Status))
            {
                return Json(new { Status = false, Msg = "无法获取服务码详情" }, JsonRequestBehavior.AllowGet);
            }
            if (!(codeDetail.Status == "Created" || codeDetail.Status == "SmsSent"))
            {
                return Json(new { Status = false, Msg = $"服务码:{model.ServiceCode}信息无法更改,请确认服务码状态" }, JsonRequestBehavior.AllowGet);
            }
            var userName = User.Identity.Name;
            var result = SearchCodeManager.UpdateBeautyServicePackageDetailCodes(model, userName, codeDetail);
            return Json(new { Status = result, Msg = "修改" + (result ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }

        #region  发码权限配置
        [PowerManage]
        public ActionResult UserBeautyConfig()
        {
            return View();
        }

        public JsonResult SelectCooperateUserByCompanyId(int companyId)
        {
            var cooperateUser = BeautyServicePackageManager.SelectCooperateUserByCompanyId(companyId);
            var account = UserAccountService.GetCompanyUsersByCompanyId(companyId);
            return Json(new { cooperateUser = cooperateUser, account = account }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectConfigByUserId(Guid userId, int companyId)
        {
            var total = 0;
            var result = BeautyServicePackageManager.GetEnterpriseUserConfig(1, 10, userId, out total).FirstOrDefault();
            var cooperateUser = BeautyServicePackageManager.SelectCooperateUserByCompanyId(companyId);
            result.PackageDetailIds = result?.CooperateUserServiceDeails?.Select(x => x.PackageDetailId).ToList();
            result.SearchServiceCode = result?.UserModuleDetails?.Where(x => String.Equals(x.ModuleType, "SearchServiceCode")).Count() > 0;
            result.SearchVerifyServiceCode = result?.UserModuleDetails?.Where(x => String.Equals(x.ModuleType, "SearchVerifyServiceCode")).Count() > 0;
            result.GenerateCode = result?.UserModuleDetails?.Where(x => String.Equals(x.ModuleType, "GenerateCode")).Count() > 0;
            result.ServiceCodeStatement= result?.UserModuleDetails?.Where(x => String.Equals(x.ModuleType, "ServiceCodeStatement")).Count() > 0;
            return Json(new { data = result, cooperateUser = cooperateUser }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEnterpriseUserConfig(Guid userId, int pageIndex = 1, int pageSize = 10)
        {
            var total = 0;
            var result = BeautyServicePackageManager.GetEnterpriseUserConfig(pageIndex, pageSize, userId, out total);
            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    using (var client = new UserAccountClient())
                    {
                        var userServiceResult = client.SelectCompanyUserInfo(item.UserId);
                        if (userServiceResult.Success && userServiceResult.Result != null)
                        {
                            item.UserMobile = userServiceResult.Result.UserMobile;
                            item.CompanyId = userServiceResult?.Result?.CompanyInfo?.Id ?? 0;
                            item.CompanyName = userServiceResult?.Result?.CompanyInfo?.Name ?? "";
                        }
                    }
                }
            }
            return Json(new { data = result, total = total }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UsertEnterpriseUserConfig(EnterpriseUserViewModel data)
        {
            var result = false;
            var msg = string.Empty;
            if (data.UserId != Guid.Empty)
            {
                result = BeautyServicePackageManager.UsertEnterpriseUserConfig(data, User.Identity.Name);
            }
            else
            {
                msg = "参数有误";
            }
            return Json(new { status = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 上传服务码服务类型配置图片
        private HttpPostedFileBase _file;
        public HttpPostedFileBase SingleFile
        {
            get
            {
                if (_file == null && Request.Files.Count > 0)
                    _file = Request.Files[0];
                return _file;
            }
        }
        public async Task<JsonResult> UploadServiceCodeTypeConfigLogo()
        {
            if (SingleFile == null)
                return Json(Tuple.Create(false, "请选择上传的图片！"));
            try
            {
                var bytes = new byte[SingleFile.InputStream.Length];
                SingleFile.InputStream.Read(bytes, 0, bytes.Length);
                using (var client = new FileUploadClient())
                {
                    var result = await client.UploadImageAsync(new Service.Utility.Request.ImageUploadRequest
                    {
                        Contents = bytes,
                        DirectoryName = "ServiceCodeTypeConfig"
                    });
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        return Json(Tuple.Create(true, result.Result.GetImageUrl()));
                    }
                    return Json(Tuple.Create(false, $"上传失败:{result.ErrorMessage}"));
                }

            }
            catch (Exception ex)
            {
                return Json(Tuple.Create(false, $"服务端异常：{ex.Message}"));
            }

        }
        #endregion
    }
}