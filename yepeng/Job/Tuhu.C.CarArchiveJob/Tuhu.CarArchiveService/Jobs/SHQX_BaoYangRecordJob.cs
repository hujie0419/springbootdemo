using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.CarArchiveService.DataAccess;
using Tuhu.CarArchiveService.Models;
using Newtonsoft.Json;
using Tuhu.CarArchiveService.Utils;
using System.Threading;

namespace Tuhu.CarArchiveService.Jobs
{
    [DisallowConcurrentExecution]
    class SHQX_BaoYangRecordJob : BaseJob
    {
        protected override ILog logger => LogManager.GetLogger<SHQX_BaoYangRecordJob>();

        protected override string JobName => "SHQX_BaoYangRecordJob";

        private Dictionary<int, Tuple<string, string>> shopAccount_shqx;

        private Dictionary<int, string> shop_tokens = new Dictionary<int, string>();

        private static bool upload =false;
        public override void Exec()
        {
            logger.Info("SHQX_BaoYangRecordJob:Begin");
            var time = DalDefault.GetMaxInstallTimeInHistory_SHQX();
            var byServiceIds = DalDefault.SelectAllServiceIds();
            var orderIds = DalDefault.SelectOrderIds(time);
            try
            {
                var shopRoadOperations = ServicesManager.FetchRoadOperationPermit(0);
                if (shopRoadOperations != null && shopRoadOperations.Any())
                {
                    UpLoadExcelData(shopRoadOperations);
                    var shopIds = shopRoadOperations.Select(s => s.ShopID)?.ToArray();

                    GetAllShopAccount(shopIds);
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

                            if (shop_tokens.ContainsKey(record.InstallShopId))
                            {
                                DalDefault.InsertRecord_SHQX(record);
                            }
                            else
                            {
                                logger.Info($"订单{record.OrderId}对应门店token未获取到");
                            }
                        }
                    }
                }
                else
                {
                    logger.Info("SHQX_BaoYangRecordJob:FetchRoadOperationPermit未获取到门店信息");
                }
                while (true)
                {
                    var unPushed = DalDefault.SelectTop500UnPushedRecordsSHQX();

                    if (unPushed != null && unPushed.Any())
                    {
                        logger.Info($"还剩{unPushed.Count()}条数据需要推送");
                        foreach (var data in unPushed)
                        {
                            var pushResult = CarArchiveHelperForSHQX.PushCarArchiveRecords(data, shop_tokens[data.InstallShopId]);
                            Thread.Sleep(20); // 避免调用过于频繁
                            if (pushResult.Item1)
                            {
                                DalDefault.UpdatePushStatus_SHQX(data.PKID, 1, pushResult.Item2);
                            }
                            else
                            {
                                DalDefault.UpdatePushStatus_SHQX(data.PKID, -1, pushResult.Item2);
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
                logger.Info($"SHQX_BaoYangRecordJob:{ex.Message}", ex);
            }

        }

        private void UpLoadExcelData(Service.TuhuShopWork.Models.ShopInfo.RoadOperationPermit[] shops)
        {
            if (upload)
                return;
            var data = new ExcelHelper(AppDomain.CurrentDomain.BaseDirectory + @"Src\途虎门店对接密码表v1.0.xlsx").GetExcelData().ToArray();
            var shopdatas = from s in shops
                            join a in data
                            on s.RoadLicenceNum equals a.ShopAccount
                            select new
                            {
                                shopId = s.ShopID,
                                shopAccount = a.ShopAccount,
                                shopPassWord = a.PassWord
                            };
            shopdatas.ForEach(f =>
            {
                if (!DalDefault.UpdateShopConfigs(f.shopId, f.shopAccount, CarArchiveHelperForSHQX.GetEncryptPassWord(f.shopPassWord)))
                {
                    logger.Error($"UpdateShopConfigs:ShopId={f.shopId},ShopAccount={f.shopAccount},ShopPassWord={f.shopPassWord}");
                }
            });
            upload = true;
        }

        private void GetAllShopAccount(int[] shopIds)
        {
            shop_tokens = new Dictionary<int, string>();
            if (shopIds != null && shopIds.Any())
            {
                shopAccount_shqx = DalDefault.SelectShopCodeDic_SHQX(shopIds);
            }
            if (shopAccount_shqx != null && shopAccount_shqx.Any())
            {
                shopAccount_shqx.ForEach(f =>
                {
                    if (!shop_tokens.ContainsKey(f.Key))
                    {
                        var token = CarArchiveHelperForSHQX.GetAccessToken(f.Value.Item1, f.Value.Item2);
                        if (!string.IsNullOrEmpty(token))
                            shop_tokens.Add(f.Key, token);
                    }
                });

            }
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
