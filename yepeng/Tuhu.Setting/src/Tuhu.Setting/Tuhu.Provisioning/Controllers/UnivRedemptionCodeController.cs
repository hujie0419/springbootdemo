using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BeautyService;
using Tuhu.Provisioning.Business.UnivRedemptionCode;
using Tuhu.Provisioning.Business.VipPaintPackage;
using Tuhu.Provisioning.DataAccess.Entity.UnivRedemptionCode;

namespace Tuhu.Provisioning.Controllers
{
    public class UnivRedemptionCodeController : Controller
    {
        private UnivRedemptionCodeManager manager = new UnivRedemptionCodeManager();

        const string BATCH_PRESETTLED = "BatchPreSettled";
        const string NO_PRESETTLED = "NoPreSettled";
        const string INTERFACE_TYPE = "Interface";
        const string BATCH_TYPE = "Batch";

        /// <summary>
        /// 通用兑换码模板列表
        /// </summary>
        /// <returns></returns>
        public ActionResult UnivCreateList()
        {
            return View();
        }

        /// <summary>
        /// 通用兑换码模板列表
        /// </summary>
        /// <returns></returns>
        public ActionResult UnivTmplList()
        {
            return View();
        }

        /// <summary>
        /// 兑换码模板详情
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public ActionResult TmplDetail(Guid id = default(Guid))
        {
            var config = id != Guid.Empty ? manager.GetRedemptionCodeConfig(id) : null;
            ViewData["Data"] = config ?? new RedemptionConfig();
            return View();
        }

        /// <summary>
        /// 券所属业务
        /// </summary>
        /// <returns></returns>
        public ActionResult CouponOfBusiness()
        {
            return View();
        }

        #region Config

        /// <summary>
        /// 分页获取
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetRedemptionCodeConfigs(SearchRedemptionConfigRequest request)
        {
            request = request ?? new SearchRedemptionConfigRequest();
            GenerateType type;
            if (!Enum.TryParse(request.GenerateType, out type) || type == GenerateType.None)
            {
                return Json(new
                {
                    status = false,
                    data = new List<RedemptionConfig>(),
                    total = 0,
                    msg = "查询参数有误"
                }, JsonRequestBehavior.AllowGet);
            }
            request.GenerateType = type.ToString();
            request.PageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            request.PageSize = request.PageSize < 1 ? 1 : request.PageSize;
            var result = manager.GetRedemptionCodeConfigs(request);
            return Json(new { status = true, data = result.Item2, total = result.Item1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRedemptionCodeConfigsByGenerateType(string generateType = BATCH_TYPE)
        {
            var result = manager.GetRedemptionCodeConfigsByGenerateType(generateType);
            return Json(new { status = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加或者更新
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult AddOrUpdateRedemptionCodeConfig(RedemptionConfig config)
        {
            #region validated
            if (config == null ||
                string.IsNullOrWhiteSpace(config.Name) ||
                config.CooperateId <= 0 ||
                string.IsNullOrWhiteSpace(config.CooperateName) ||
                config.EffectiveDay <= 0 ||
                config.AtLeastNum <= 0 || config.AtMostNum <= 0)
            {
                return Json(new { status = false, msg = "配置不能为空,参数错误" });
            }
            config.GenerateType = string.Equals(config.GenerateType, BATCH_TYPE, StringComparison.OrdinalIgnoreCase) ?
                BATCH_TYPE :
                (string.Equals(config.GenerateType, INTERFACE_TYPE, StringComparison.OrdinalIgnoreCase) ? INTERFACE_TYPE : string.Empty);
            if (string.IsNullOrEmpty(config.GenerateType))
            {
                return Json(new { status = false, msg = "生成类型有误，请重试" });
            }

            config.Name = config.Name.Trim();
            config.SettlementMethod = string.Equals(BATCH_PRESETTLED, config.SettlementMethod, StringComparison.OrdinalIgnoreCase) ?
                BATCH_PRESETTLED : string.Empty;

            var exists = manager.IsExistsRedemptionConfig(config);
            if (exists)
            {
                return Json(new { status = false, msg = "已存在相同名称的配置" });
            }
            if (config.GroupId != null)
            {
                var isUpdate = config.ConfigId != Guid.Empty;
                var groupSetting = manager.GetGroupSettingByGroupId(config.GroupId ?? Guid.Empty);
                if (groupSetting == null)
                    return Json(new { status = false, msg = $"所选群组不存在:{config.GroupId}" });


                var RedeemMrCodeConfigs = manager.GetRedeemMrCodeConfigs(config.ConfigId);
                if (groupSetting.SendCodeType == 1&&config.IsActive)//自动发码
                {
                    var groupConfigs = manager.GetRedemptionConfigsGroupId(config.GroupId ?? Guid.Empty);
                    if (!isUpdate&&groupConfigs != null && groupConfigs.Any())
                        return Json(new { status = false, msg = "所选群组为自动发码，已经存在一个套餐=》自动发码群组只能有一个套餐" });
                    if(isUpdate&& groupConfigs != null && groupConfigs.Count()>=1&&groupConfigs.Any(a=>a.ConfigId!=config.ConfigId))
                        return Json(new { status = false, msg = "所选群组为自动发码，已经存在一个套餐=》自动发码群组只能有一个套餐" });
                }
                if (RedeemMrCodeConfigs != null && RedeemMrCodeConfigs.Any(a => !a.IsRequired))
                    return Json(new { status = false, msg = "配置了群组的模板,模板详情内容都应该必选" });
            }
            #endregion

            config.CreateUser = User.Identity.Name;
            var result = config.ConfigId != Guid.Empty ?
                manager.UpdateRedemptionCodeConfig(config) :
                manager.AddRedemptionCodeConfig(config);
            return Json(new { status = result });
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="auditResult"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public ActionResult AuditRedemptionCodeConfig(bool auditResult, Guid configId = default(Guid))
        {
            if (configId == Guid.Empty)
            {
                return Json(new { status = false, msg = "配置选择不正确, 请重试！！" });
            }
            var config = manager.GetRedemptionCodeConfig(configId);
            if (config == null)
            {
                return Json(new { status = false, msg = "模板配置不存在" });
            }
            if (string.Equals(config.CreateUser, User.Identity.Name, StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { status = false, msg = "自己不能给自己审核，请找其他人审核" });
            }
            if (!string.Equals(config.AuditStatus, AudioStatus.Pending.ToString()))
            {
                return Json(new { status = false, msg = "该模板配置已经审核过了！请不要重复审核。" });
            }
            string auditStatus = auditResult ? AudioStatus.Accepted.ToString() : AudioStatus.Rejected.ToString();
            bool result = manager.AuditRedemptionCodeConfig(configId, auditStatus, User.Identity.Name);
            return Json(new { status = result });
        }

        public ActionResult GetRedemptionCodeConfig(Guid configId = default(Guid))
        {
            var config = configId == Guid.Empty ? null : manager.GetRedemptionCodeConfig(configId);
            return Json(new { status = config != null, data = config }, JsonRequestBehavior.AllowGet);
        }
        [PowerManage]
        public ActionResult DeleteRedemptionCodeConfig(Guid configId = default(Guid))
        {
            var config = manager.GetRedemptionCodeConfig(configId);
            if (config != null && config.SumQuantity > 0)
            {
                return Json(new { status = false, msg = "已生成兑换码不允许删除" });
            }
            var success = configId == Guid.Empty ? false : manager.DeleteRedemptionCodeConfig(configId, User.Identity.Name);
            return Json(new { status = success });
        }

        #endregion

        #region ServiceCodeConfig

        public ActionResult GetRedeemMrCodeConfigs(Guid configId = default(Guid))
        {
            if (configId == default(Guid))
            {
                return Json(new { status = false, msg = "配置Id有误" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.GetRedeemMrCodeConfigs(configId);
            var data = result?.Select(x => new
            {
                x.CodeTypeConfigId,

                x.Description,
                x.EffectiveDay,
                EndTime = x.EndTime?.ToString("yyyy-MM-dd"),
                StartTime = x.StartTime?.ToString("yyyy-MM-dd"),
                x.IsActive,
                x.IsRequired,
                x.Name,
                x.Num,
                x.PKID,
                x.RedemptionConfigId,
                x.SettlementMethod,
                x.SettlementPrice,
                x.ShopCommission,
                x.ServiceName,
            });
            return Json(new { status = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRedeemMrCodeConfig(int redeemMrCodeConfigId = 0)
        {
            var result = manager.GetRedeemMrCodeConfig(redeemMrCodeConfigId);
            var data = result == null ? null : new
            {
                result.CodeTypeConfigId,
                result.Description,
                result.EffectiveDay,
                EndTime = result.EndTime?.ToString("yyyy-MM-dd"),
                StartTime = result.StartTime?.ToString("yyyy-MM-dd"),
                result.IsActive,
                result.IsRequired,
                result.Name,
                result.Num,
                result.PKID,
                result.RedemptionConfigId,
                result.SettlementMethod,
                result.SettlementPrice,
                result.ShopCommission,
                result.ServiceName,
            };
            return Json(new { status = true, data }, JsonRequestBehavior.AllowGet);
        }
        [PowerManage]
        public ActionResult AddOrUpdateRedeemMrCodeConfig(RedeemMrCodeConfig config)
        {
            Func<string> validatedFun = () =>
            {
                if (config == null)
                {
                    return "参数不能为空";
                }

                if (string.IsNullOrEmpty(config.Name))
                {
                    return "名称不能为空";
                }
                SettlementMethod SettlementMethod;
                if (string.IsNullOrEmpty(config.SettlementMethod) ||
                    !Enum.TryParse(config.SettlementMethod, out SettlementMethod) ||
                    SettlementMethod == SettlementMethod.None)
                {
                    return "结算方式不是有效的值";
                }

                if (config.SettlementPrice <= 0.0M || config.ShopCommission < 0 || config.ShopCommission >= 1)
                {
                    return "结算价不能小于0并且佣金比必须再0~1之间";
                }

                if (config.CodeTypeConfigId <= 0 || config.Num <= 0)
                {
                    return "必须选择一个服务并且服务数量不能小于1";
                }
                var startTime = config.StartTime.GetValueOrDefault().Date;
                var endTime = config.EndTime.GetValueOrDefault().Date.AddDays(1).AddSeconds(-1);
                var effectiveDay = config.EffectiveDay.GetValueOrDefault();
                if (((startTime == DateTime.MinValue || endTime == DateTime.MinValue) && effectiveDay <= 0) ||
                    (startTime > DateTime.MinValue && endTime > startTime && effectiveDay > 0) ||
                    (startTime > DateTime.MinValue && endTime < startTime))
                {
                    return "时间范围和兑换后天数必须填一个,并且只能填一个;时间范围必须有效,天数必须大于0";
                }
                config.Name = config.Name.Trim();
                config.SettlementMethod = SettlementMethod.ToString();
                if (effectiveDay > 0)
                {
                    config.EffectiveDay = effectiveDay;
                    config.StartTime = null;
                    config.EndTime = null;
                }
                else
                {
                    config.EffectiveDay = null;
                    config.StartTime = startTime;
                    config.EndTime = endTime;
                }
                if (manager.IsExistsRedeemMrCodeConfig(config))
                {
                    return "该配置名称已经存在";
                }
                var temp_config = manager.GetRedemptionCodeConfig(config.RedemptionConfigId);
                if (temp_config != null && temp_config.GroupId != null && !config.IsRequired)
                    return "配置了群组的模板,模板详情内容都应该必选";
                var redemption = manager.GetRedemptionCodeConfig(config.RedemptionConfigId);
                if (redemption != null && redemption.SumQuantity > 0)
                {
                    if (config.PKID <= 0)
                    {
                        return "兑换码已生成，不允许新加项目";
                    }
                    var oldValue = manager.GetRedeemMrCodeConfig(config.PKID);
                    if (oldValue == null)
                    {
                        return "参数有误";
                    }
                    if (oldValue.CodeTypeConfigId != config.CodeTypeConfigId || !string.Equals(oldValue.SettlementMethod, config.SettlementMethod)
                    || oldValue.SettlementPrice != config.SettlementPrice || oldValue.ShopCommission != config.ShopCommission
                    || oldValue.Num != config.Num || oldValue.EffectiveDay != config.EffectiveDay || (oldValue.StartTime != null && config.StartTime != oldValue.StartTime)
                    || (oldValue.EndTime != null && config.EndTime != oldValue.EndTime)
                    || !string.Equals(oldValue.SettlementMethod, config.SettlementMethod) || oldValue.Num != config.Num
                    || oldValue.IsActive != config.IsActive)
                    {
                        return "兑换码已生成，关键信息不允许修改";
                    }
                }
                return string.Empty;
            };
            var validatedResult = validatedFun();
            if (!string.IsNullOrEmpty(validatedResult))
            {
                return Json(new { status = false, msg = validatedResult });
            }
            bool result = config.PKID <= 0 ? manager.AddRedeemMrCodeConfig(config, User.Identity.Name)
                : manager.UpdateRedeemMrCodeConfig(config, User.Identity.Name);
            return Json(new { status = result });
        }

        public ActionResult DeleteRedeemMrCodeConfig(int id = 0)
        {
            var success = false;
            if (id > 0)
            {
                var mrConfig = manager.GetRedeemMrCodeConfig(id);
                if (mrConfig != null)
                {
                    var config = manager.GetRedemptionCodeConfig(mrConfig.RedemptionConfigId);
                    if (config != null && config.SumQuantity > 0)
                    {
                        return Json(new { status = false, msg = "已生成兑换码的不允许删除" });
                    }
                }
                success = manager.DeleteRedeemMrCodeConfig(id, User.Identity.Name);
            }
            return Json(new { status = success });
        }

        #region RedeemMrCodeLimitConfig

        public ActionResult GetRedeemMrCodeLimitConfig(int mrCodeConfigId = 0)
        {
            var result = mrCodeConfigId <= 0 ? null : manager.GetRedeemMrCodeLimitConfig(mrCodeConfigId);
            return Json(new { status = result != null, data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRedeemMrCodeLimitConfig(RedeemMrCodeLimitConfig config)
        {
            Func<string> validatedFun = () =>
            {
                config.CreateUser = User.Identity.Name;
                return string.Empty;
            };
            var validatedResult = validatedFun();
            if (!string.IsNullOrEmpty(validatedResult))
            {
                return Json(new { status = false, msg = validatedResult });
            }
            var success = manager.UpdateRedeemMrCodeLimitConfig(config, User.Identity.Name);
            return Json(new { status = success });
        }

        #endregion

        #endregion

        #region GenerateCode
        [PowerManage]
        public ActionResult GenerateRedemptionCodeRecords(Guid configId, int num = 0)
        {
            if (num <= 0)
            {
                return Json(new { status = false, msg = "生成数量必须大于0" });
            }
            var config = configId == Guid.Empty ? null : manager.GetRedemptionCodeConfig(configId);
            if (config == null || !config.IsActive)
            {
                return Json(new { status = false, msg = "模板不存在或者无效" });
            }
            if (string.Equals(config.GenerateType, INTERFACE_TYPE, StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { status = false, msg = "模板运营配置类型" });
            }
            bool success = manager.GenerateRedemptionCodeRecords(config.ConfigId, config.EffectiveDay, num, User.Identity.Name);
            return Json(new { status = success });
        }

        public ActionResult GetRedemptionCodeRecords(SearchRedemptionConfigRequest request)
        {
            request = request ?? new SearchRedemptionConfigRequest();
            request.PageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
            request.PageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            switch (request.SettlementMethod?.ToLower())
            {
                case "all":
                    request.SettlementMethod = null;
                    break;
                case "none":
                    request.SettlementMethod = string.Empty;
                    break;
                case "batchpresettled":
                    request.SettlementMethod = BATCH_PRESETTLED;
                    break;
            }
            var result = manager.GetRedemptionCodeRecords(request);
            var total = result.Item1;
            var data = result.Item2?.Select(x => new
            {
                x.Name,
                x.Num,
                x.RedemptionConfigId,
                x.CreateUser,
                x.BatchCode,
                x.CooperateName,
                x.Status,
                CreateTime = x.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                StartTime = x.CreateTime.ToString("yyyy-MM-dd"),
                EndTime = x.CreateTime.AddDays(x.EffectiveDay).ToString("yyyy-MM-dd")
            });
            return Json(new { status = data != null, total, data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadRedemptionCodeRecords(string batchcode)
        {
            batchcode = batchcode?.Trim();
            var result = !string.IsNullOrEmpty(batchcode) ? manager.GetRedemptionCodeRecords(batchcode, User.Identity.Name) : new List<RedemptionCodeRecord>();
            var list = result.Select(x => new
            {
                x.RedemptionCode,
                StartTime = x.CreateTime.ToString("yyyy-MM-dd"),
                EndTime = x.CreateTime.AddDays(x.EffectiveDay).ToString("yyyy-MM-dd"),
            });

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("兑换码");
            row.CreateCell(cellNum++).SetCellValue("有效开始时间");
            row.CreateCell(cellNum++).SetCellValue("有效截至时间");

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 28 * 256);
            sheet.SetColumnWidth(cellNum++, 28 * 256);
            var rowNumber = 1;
            list.ForEach(x =>
            {
                cellNum = 0;
                row = sheet.CreateRow(rowNumber);

                row.CreateCell(cellNum++).SetCellValue(x.RedemptionCode);
                row.CreateCell(cellNum++).SetCellValue(x.StartTime);
                row.CreateCell(cellNum++).SetCellValue(x.EndTime);

                rowNumber++;
            });

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"批次{batchcode}兑换码{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 作废通用兑换码
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [PowerManage]
        public async Task<JsonResult> InvalidateRedemptionCode(string batchCode, string remark)
        {
            var redemptionManager = new RedemptionManager();
            var user = User.Identity.Name;
            var result = await redemptionManager.InvalidateRedemptionCode(batchCode, remark, user);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetData

        /// <summary>
        /// 获取保养套餐流程数据
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public ActionResult GetBaoYangPackages(Guid configId = default(Guid))
        {
            if (configId == default(Guid))
            {
                return Json(new { data = new List<Object>() }, JsonRequestBehavior.AllowGet);
            }
            var config = manager.GetRedemptionCodeConfig(configId);
            if (config == null)
            {
                return Json(new { data = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
            var packages = new Business.VipBaoYangPackage.VipBaoYangPackageManager().GetBaoYangPackageNameByVipUserId(config.VipUserId)
                ?.Where(s => string.Equals(s.SettlementMethod, "ByPeriod"));
            return Json(new
            {
                data = packages?.Select(x => new
                {
                    PackageId = x.PKID,
                    x.PackageName,
                    x.Price
                })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPaintPackages(Guid configId = default(Guid))
        {
            if (configId == default(Guid))
            {
                return Json(new { data = new List<Object>() }, JsonRequestBehavior.AllowGet);
            }
            var config = manager.GetRedemptionCodeConfig(configId);
            if (config == null)
            {
                return Json(new { data = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
            var paintManager = new VipPaintPackageManager();
            var packages = paintManager.GetVipPaintPackages(config.VipUserId)?.Where(s => string.Equals(s.SettlementMethod, "ByPeriod"));
            return Json(new
            {
                data = packages?.Select(x => new
                {
                    x.PackageId,
                    x.PackageName,
                    Price = x.PackagePrice
                })
            }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region 券所属业务

        public ActionResult AddCouponBusinessConfig(PromotionBusinessTypeConfig config)
        {
            Func<string> validatedFun = () =>
            {
                if (config == null ||
                    string.IsNullOrWhiteSpace(config.Name) ||
                    string.IsNullOrWhiteSpace(config.BusinessType) ||
                    config.GetRuleGuid == default(Guid))
                {
                    return "参数不能为空!";
                }
                BusinessType businessType;
                if (!Enum.TryParse(config.BusinessType, out businessType) || businessType == BusinessType.None)
                {
                    return "不是有效的业务类型!";
                }
                if (manager.IsExistsCouponBusinessConfig(config))
                {
                    return "已经存在相同名称或者相同规则的数据!";
                }
                if (string.Equals(config.BusinessType, nameof(BusinessType.BaoYangPackage)) && config.RuleId != 123128)
                {
                    return "保养套餐流程母券选择有误";
                }
                if (string.Equals(config.BusinessType, nameof(BusinessType.PaintPackage)) && config.RuleId != 126177)
                {
                    return "喷漆套餐流程母券选择有误";
                }
                config.CreateUser = User.Identity.Name;
                config.Name = config.Name.Trim();
                config.BusinessType = businessType.ToString();
                return string.Empty;
            };
            var validatedResult = validatedFun();
            if (!string.IsNullOrEmpty(validatedResult))
            {
                return Json(new { status = false, msg = validatedResult });
            }
            bool result = manager.AddCouponBusinessConfig(config);
            return Json(new { status = result });
        }

        public ActionResult RemoveCouponBusinessConfig(int id)
        {
            if (id <= 0)
            {
                return Json(new { status = false, msg = "指定项不存在!" });
            }
            var result = manager.RemoveCouponBusinessConfig(id, User.Identity.Name);
            return Json(new { status = result });
        }

        public ActionResult GetCouponBusinessConfigs(string businessType)
        {
            BusinessType businessTypeEnum;
            if (!string.IsNullOrEmpty(businessType) && (!Enum.TryParse(businessType, out businessTypeEnum) || businessTypeEnum == BusinessType.None))
            {
                return Json(new
                {
                    status = false,
                    data = new List<PromotionBusinessTypeConfig>(),
                    msg = "不是有效的业务类型!"
                }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.GetCouponBusinessConfigs(businessType);
            return Json(new { status = true, data = result, }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CouponConfig

        public ActionResult GetRedeemPromotionConfigs(Guid configId = default(Guid))
        {
            var result = configId == Guid.Empty ? new List<RedeemPromotionConfig>() : manager.GetRedeemPromotionConfigs(configId);
            return Json(new { status = true, data = result }, JsonRequestBehavior.AllowGet);
        }
        [PowerManage]
        public ActionResult AddOrUpdateRedeemPromotionConfig(RedeemPromotionConfig config)
        {
            Func<string> validatedFun = () =>
            {
                if (config == null)
                {
                    return "参数不能为空";
                }
                if (string.IsNullOrWhiteSpace(config.Name))
                {
                    return "名称不能为空";
                }
                BusinessType type;
                if (string.IsNullOrWhiteSpace(config.BusinessType) || !Enum.TryParse(config.BusinessType, out type) || type == BusinessType.None)
                {
                    return "业务类型不是有效的值";
                }
                if ((type == BusinessType.AnnualInspectionPackage ||
                    type == BusinessType.BaoYangPackage ||
                    type == BusinessType.PaintPackage) && config.PackageId <= 0)
                {
                    return "业务为套餐流程的时候必须选择一个套餐";
                }
                if (config.SettlementPrice <= 0)
                {
                    return "大客户结算价是必填的并且必须大于0";
                }
                if (config.Num <= 0)
                {
                    return "数量是必填的并且必须大于0";
                }
                if (manager.IsExistsRedeemPromotionConfig(config))
                {
                    return "该配置已经存在";
                }
                var redemption = manager.GetRedemptionCodeConfig(config.RedemptionConfigId);
                if (redemption != null && redemption.SumQuantity > 0)
                {
                    if (config.PKID <= 0)
                    {
                        return "兑换码已生成，不允许新加项目";
                    }
                    var oldValue = manager.GetRedeemPromotionConfig(config.PKID);
                    if (oldValue == null)
                    {
                        return "参数有误";
                    }
                    if (!string.Equals(oldValue.BusinessType, config.BusinessType) || !string.Equals(oldValue.GetRuleGuid, config.GetRuleGuid)
                    || oldValue.SettlementPrice != config.SettlementPrice || oldValue.PackageId != config.PackageId
                    || !string.Equals(oldValue.SettlementMethod, config.SettlementMethod) || oldValue.Num != config.Num
                    || oldValue.IsActive != config.IsActive)
                    {
                        return "兑换码已生成，关键信息不允许修改";
                    }
                }
                config.Name = config.Name.Trim();
                config.BusinessType = type.ToString();
                return string.Empty;
            };
            var validatedResult = validatedFun();
            if (!string.IsNullOrEmpty(validatedResult))
            {
                return Json(new { status = false, msg = validatedResult });
            }
            var result = config.PKID <= 0 ?
                manager.AddRedeemPromotionConfig(config, User.Identity.Name) :
                manager.UpdateRedeemPromotionConfig(config, User.Identity.Name);
            return Json(new { status = result });

        }

        public ActionResult GetRedeemPromotionConfig(int id = 0)
        {
            var config = id <= 0 ? null : manager.GetRedeemPromotionConfig(id);
            return Json(new { status = config != null, data = config }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteRedeemPromotionConfig(int id = 0)
        {
            var promotionConfig = manager.GetRedeemPromotionConfig(id);
            if (promotionConfig != null)
            {
                var config = manager.GetRedemptionCodeConfig(promotionConfig.RedemptionConfigId);
                if (config != null && config.SumQuantity > 0)
                {
                    return Json(new { status = false, msg = "已生成兑换码的不允许删除" });
                }
            }
            var result = id <= 0 ? false : manager.DeleteRedeemPromotionConfig(id, User.Identity.Name);
            return Json(new { status = result });
        }

        #endregion

        #region groupSetting配置
        /// <summary>
        /// groupSetting配置
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public ActionResult GroupSetting()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetGroupSettings(int PageSize, int PageIndex, string KeyWord)
        {
            int count = 0;
            var list = manager.SelectRedeemGroupSetting(out count, PageIndex, PageSize, KeyWord);
            return Json(new { result = list, total = count });
        }
        [HttpPost]
        public JsonResult SaveGroupSetting(RedeemGroupSetting model)
        {
            if (model == null)
                return Json(new { result = false, msg = "请求参数不能为空！" });
            if (string.IsNullOrWhiteSpace(model.GroupName))
                return Json(new { result = false, msg = "分组名称不能为空！" });

            var result = manager.SaveRedeemGroupSetting(model, User.Identity.Name);
            return Json(new { result = result.Item1, msg = result.Item2 });
        }
        [HttpPost]
        public JsonResult DeleteGroupSetting(RedeemGroupSetting model)
        {
            if (model == null)
                return Json(new { result = false, msg = "请求参数不能为空！" });
            if (model.GroupId == null || model.GroupId == Guid.Empty)
                return Json(new { result = false, msg = "分组GroupId不能为空！" });

            var result = manager.DeleteRedeemGroupSetting(model, User.Identity.Name);
            return Json(new { result = result.Item1, msg = result.Item2 });
        }

        public ActionResult RedeemTest()
        {
            return View();
        }
        #endregion

    }
}