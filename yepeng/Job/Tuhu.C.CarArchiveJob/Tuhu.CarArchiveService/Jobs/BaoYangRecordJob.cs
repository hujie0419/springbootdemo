using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.CarArchiveService.Utils;
using Common.Logging;
using System.Data.SqlClient;
using System.Data;
using Tuhu.CarArchiveService.Models;
using Tuhu.CarArchiveService.DataAccess;
using Newtonsoft.Json;
using System.Threading;
using Tuhu.Service.TuhuShopWork.Models.ShopInfo;

namespace Tuhu.CarArchiveService.Jobs
{
    /// <summary>
    /// 汽车电子档案推送job
    /// </summary>
    [DisallowConcurrentExecution]
    public class BaoYangRecordJob : BaseJob
    {
        protected override string JobName => typeof(BaoYangRecordJob).ToString();

        protected override ILog logger { get; }

        /// <summary>
        /// licenceno, shopname, regioncode, cararchievecode
        /// </summary>
        private Dictionary<int, Tuple<string, string, string, string>> ShopCodes { get; set; }

        public BaoYangRecordJob()
        {
            this.logger = LogManager.GetLogger(JobName);
            this.ShopCodes = new Dictionary<int, Tuple<string, string, string, string>>();
        }

        /// <summary>
        /// 1. 获取一小时内的或者最新的已安装订单数据
        /// 2. 查询门店的道路经营许可证，调用接口获取门店唯一代码
        /// 3. 推送数据
        /// </summary>
        public override void Exec()
        {
            string vpn_name = ConfigurationManager.AppSettings["vpn_name"].ToString();
            string vpn_ip = ConfigurationManager.AppSettings["vpn_ip"].ToString();
            string vpn_uname = ConfigurationManager.AppSettings["vpn_uname"].ToString();
            string vpn_pwd = ConfigurationManager.AppSettings["vpn_pwd"].ToString();
            string vpn_presharedkey = ConfigurationManager.AppSettings["vpn_presharedkey"].ToString();
            VPNConnector connector = new VPNConnector(vpn_name, vpn_ip, vpn_uname, vpn_pwd, vpn_presharedkey);

            var currentTime = DateTime.Now;
            var time = DalDefault.GetMaxInstallTimeInHistory();
            var byServiceIds = DalDefault.SelectAllServiceIds();
            var orderIds = DalDefault.SelectOrderIds(time);

            try
            {
                //connector.CreateOrUpdateVPN();
                logger.Info("开始连接vpn！");
                var isConnected = connector.Connect();

                if (isConnected)
                {
                    logger.Info("VPN已连接!");
                    var token = CarArchiveHelper.GetAccessToken();

                    if (!string.IsNullOrEmpty(token))
                    {
                        var shopRoadOperations = ServicesManager.FetchRoadOperationPermit(0);
                        if (shopRoadOperations != null && shopRoadOperations.Any())
                        {
                            //注册门店数据
                            GetAllCompanyCode(shopRoadOperations, token);


                            var ShopIds = shopRoadOperations.Where(w => !w.RoadLicenceRegionCode?.StartsWith("310") ?? false)
                                                            .Where(w => !w.RoadLicenceRegionCode?.StartsWith("510") ?? false).Select(s => s.ShopID).Distinct().ToArray();
                            logger.Info($"符合要求的ShopId: {string.Join(",", ShopIds)}");
                            foreach (var orderId in orderIds)
                            {
                                var record = DalDefault.SelectOrderDetail(orderId.Item1, orderId.Item2);

                                if (record != null && ShopIds.Contains(record.InstallShopId) && !string.IsNullOrEmpty(record.VinCode) && record.PartList != null && record.PartList.Any() &&
                                    record.ProjectList != null && record.ProjectList.Any(o => byServiceIds.Contains(o.ServiceId)))
                                {
                                    FillRecord(record, token, shopRoadOperations.FirstOrDefault(f => f.ShopID == record.InstallShopId));
                                    logger.Info(JsonConvert.SerializeObject(record));

                                    if (!string.IsNullOrEmpty(record.ShopCode))
                                    {
                                        DalDefault.InsertRecord(record);
                                    }
                                    else
                                    {
                                        logger.Info($"数据不全, {record.OrderId}");
                                    }
                                }
                            }

                            while (true)
                            {
                                var unPushed = DalDefault.SelectTop500UnPushedRecords();

                                if (unPushed != null && unPushed.Any())
                                {
                                    logger.Info($"还剩{unPushed.Count()}条数据需要推送");
                                    foreach (var data in unPushed)
                                    {
                                        string pushResult = CarArchiveHelper.PushRecord(data);
                                        Thread.Sleep(20); // 避免调用过于频繁
                                        if (pushResult.Contains("新增成功"))
                                        {
                                            DalDefault.UpdatePushStatus(data.PKID, 1, pushResult);
                                        }
                                        else
                                        {
                                            DalDefault.UpdatePushStatus(data.PKID, -1, pushResult);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            logger.Info($"FetchRoadOperationPermit:未获取到门店数据");
                        }
                    }
                    else
                    {
                        logger.Error("无法获取到token");
                    }
                }
                else
                {
                    logger.Error("无法连上VPN!");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                //var isDisconnected = connector.Disconnect();
                //if (!isDisconnected)
                //{
                //    logger.Error("无法断开VPN!");
                //}
                //else
                //{
                //    logger.Info("VPN已断开!");
                //}
            }
        }

        /// <summary>
        /// 补充数据
        /// 1. 门店唯一编码
        /// 2. 订单保养数据
        /// </summary>
        /// <param name="record"></param>
        public void FillRecord(BaoYangRecordModel record, string accesstoken, RoadOperationPermit shopData)
        {
            // 填充门店唯一编码
            if (ShopCodes.ContainsKey(record.InstallShopId))
            {
                record.ShopCode = ShopCodes[record.InstallShopId].Item4;
                record.InstallShopName = ShopCodes[record.InstallShopId].Item2;
                record.ShopRegionCode = ShopCodes[record.InstallShopId].Item3;
            }
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
        /// <summary>
        /// 生成审核通过维修企业唯一标识
        /// </summary>
        public bool GetAllCompanyCode(RoadOperationPermit[] shops, string accesstoken)
        {
            if (shops != null && shops.Any())
            {
                var shopCompanyCodes = DalDefault.SelectShopCodeDic(shops.Select(s => s.ShopID));
                shops.ForEach(f =>
                {
                    if (!shopCompanyCodes.ContainsKey(f.ShopID))
                    {
                        if (!string.IsNullOrEmpty(f.RoadLicenceNum))
                        {
                            var shopCode = CarArchiveHelper.GetCompanyCode(f.RoadLicenceCompanyName, f.RoadLicenceNum, f.RoadLicenceRegionCode);
                            if (!string.IsNullOrEmpty(shopCode))
                            {
                                DalDefault.UpdateCarArchiveCode(f.ShopID, shopCode);
                                ShopCodes[f.ShopID] = Tuple.Create(f.RoadLicenceNum, f.RoadLicenceCompanyName, f.RoadLicenceRegionCode, shopCode);
                            }
                            Thread.Sleep(30); // 避免调用过于频繁
                        }
                    }
                    else if (!ShopCodes.ContainsKey(f.ShopID))
                    {
                        ShopCodes.Add(f.ShopID, Tuple.Create(f.RoadLicenceNum, f.RoadLicenceCompanyName, f.RoadLicenceRegionCode, shopCompanyCodes[f.ShopID]));
                    }
                });
            }
            return true;
        }
    }
}
