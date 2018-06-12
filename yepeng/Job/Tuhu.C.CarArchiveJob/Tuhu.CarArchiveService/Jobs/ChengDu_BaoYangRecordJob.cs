using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.CarArchiveService.DataAccess;
using Tuhu.CarArchiveService.Utils;
using System.Threading;
using Newtonsoft.Json;
using Tuhu.Service.TuhuShopWork.Models.ShopInfo;
using Tuhu.CarArchiveService.Models;

namespace Tuhu.CarArchiveService.Jobs
{
    [DisallowConcurrentExecution]
    public class ChengDu_BaoYangRecordJob : BaseJob
    {
        protected override ILog logger => LogManager.GetLogger<ChengDu_BaoYangRecordJob>();

        protected override string JobName => "ChengDu_BaoYangRecordJob";

        public override void Exec()
        {
            logger.Info("ChengDu_BaoYangRecordJob:Begin");
            var time = DalDefault.GetMaxInstallTimeInHistory_ChengDu();
            var byServiceIds = DalDefault.SelectAllServiceIds();
            var orderIds = DalDefault.SelectOrderIds(time);
            try
            {
                var shopRoadOperations = ServicesManager.FetchRoadOperationPermit(0)?.Where(w => w.RoadLicenceRegionCode?.StartsWith("510") ?? false).ToArray();
                if (shopRoadOperations != null && shopRoadOperations.Any())
                {
                    var shopIds = shopRoadOperations.Select(s => s.ShopID)?.ToArray();
                    RegisterCompany(shopRoadOperations);//注册
                    foreach (var orderId in orderIds)
                    {
                        var record = DalDefault.SelectOrderDetail(orderId.Item1, orderId.Item2);
                        if (record != null &&
                            shopIds.Contains(record.InstallShopId) &&
                            !string.IsNullOrEmpty(record.VinCode) &&
                            record.PartList != null && record.PartList.Any() &&
                            record.ProjectList != null &&
                            record.ProjectList.Any(o => byServiceIds.Contains(o.ServiceId)))
                        {
                            FillRecord(record);
                            logger.Info(JsonConvert.SerializeObject(record));
                            DalDefault.InsertRecord_ChengDu(record);
                        }
                    }
                }
                else
                {
                    logger.Info("ChengDu_BaoYangRecordJob:FetchRoadOperationPermit未获取到门店信息");
                }
                while (true)
                {
                    var unPushed = DalDefault.SelectTop500UnPushedRecordsChengDu();

                    if (unPushed != null && unPushed.Any())
                    {
                        logger.Info($"还剩{unPushed.Count()}条数据需要推送");
                        foreach (var data in unPushed)
                        {
                            var pushResult = new CarArchiveHelperForChengDu(data.InstallShopId).PushCarArchiveRecords(data);
                            Thread.Sleep(20); // 避免调用过于频繁
                            if (pushResult.Item1)
                            {
                                DalDefault.UpdatePushStatus_ChengDu(data.PKID, 1, pushResult.Item2);
                            }
                            else
                            {
                                DalDefault.UpdatePushStatus_ChengDu(data.PKID, -1, pushResult.Item2);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void RegisterCompany(RoadOperationPermit[] shopRoadOperations)
        {
            if (shopRoadOperations == null || !shopRoadOperations.Any())
                throw new Exception("shopRoadOperations不能为空");
            var ShopCodeDic_ChengDu = DalDefault.SelectShopCodeDic_All(shopRoadOperations.Select(s => s.ShopID), Models.PushToEnum.ChengDu);
            CarArchiveHelperForChengDu.TokenReset();
            shopRoadOperations.ForEach(f =>
            {
                if (!ShopCodeDic_ChengDu.ContainsKey(f.ShopID))
                {//未注册过
                    var shopdetail = ServicesManager.FetchShopDetail(f.ShopID);
                    var companyUnifiedSocialCreditidentifier = DalDefault.GetCreditidentifierByShopId(f.ShopID);
                    if (shopdetail == null||string.IsNullOrEmpty(companyUnifiedSocialCreditidentifier))
                        return;
                    var password = Guid.NewGuid().ToString();
                    var RoadTransportationLicenseStartdate = default(DateTime);
                    DateTime.TryParse(f.RoadLicenceValidStartDate, out RoadTransportationLicenseStartdate);
                    var RoadTransportationLicenseEnddate = default(DateTime);
                    DateTime.TryParse(f.RoadLicenceValidEndDate, out RoadTransportationLicenseEnddate);
                    var code = new CarArchiveHelperForChengDu(f.ShopID).CompanyRegister(new Models.ChengDuCarArchiveRegisterModel
                    {
                        CompanyAddress = shopdetail.AddressBrief,
                        CompanyBusinessscope = shopdetail.Brand,
                        CompanyCategory = shopdetail.ShopClassification,
                        CompanyEmail = "unavailable@tuhu.cn",//DalDefault.GetEmailAddressByShopId(f.ShopID),
                        CompanyLinkmanname = shopdetail.Contact,
                        CompanyLinkmantel = shopdetail.Mobile,
                        CompanyName = shopdetail.CompanyName,
                        CompanySuperintendentname = shopdetail.Contact,
                        CompanySuperintendenttel = shopdetail.Mobile,
                        CompanyOperationState = "营业",

                        CompanyAdministrativedivisioncode = f.RoadLicenceRegionCode,
                        CompanyPostalcode = "610000",
                        CompanyEconomicCategory = "个体自营",
                        CompanyRoadTransportationLicense = f.RoadLicenceNum,
                        CompanyUnifiedSocialCreditidentifier = companyUnifiedSocialCreditidentifier,
                        RoadTransportationLicenseStartdate = $"{RoadTransportationLicenseStartdate.Year.ToString("00")}{RoadTransportationLicenseStartdate.Month.ToString("00")}{RoadTransportationLicenseStartdate.Day.ToString("00")}",
                        RoadTransportationLicenseEnddate = $"{RoadTransportationLicenseEnddate.Year.ToString("00")}{RoadTransportationLicenseEnddate.Month.ToString("00")}{RoadTransportationLicenseEnddate.Day.ToString("00")}",

                        CompanyPassword = password
                    });
                    if (!code)
                        logger.Error($"InsertShopCodeDic_All:注册门店失败=》{f.ShopID},{code}");
                }
                else
                {
                    var shopconfig = ShopCodeDic_ChengDu[f.ShopID];
                    new CarArchiveHelperForChengDu(f.ShopID).Init(shopconfig.Item1, shopconfig.Item2);
                }
            });
        }


        /// <summary>
        /// 补充数据
        /// 1. 门店唯一编码
        /// 2. 订单保养数据
        /// </summary>
        /// <param name="record"></param>
        private void FillRecord(BaoYangRecordModel record)
        {
            // 填充零件号
            var pids = record.PartList.Select(o => o.ProductId).ToList();
            var partCodes = DalDefault.SelectPartCodes(pids);

            foreach (var part in record.PartList)
            {
                if (partCodes.ContainsKey(part.ProductId))
                {
                    part.PartCode = partCodes[part.ProductId];
                }
                else
                {
                    part.PartCode = part.ProductId;
                }
            }
        }
    }
}
