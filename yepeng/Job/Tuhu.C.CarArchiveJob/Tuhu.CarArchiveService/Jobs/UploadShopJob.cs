using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.CarArchiveService.DataAccess;
using Tuhu.CarArchiveService.Utils;

namespace Tuhu.CarArchiveService.Jobs
{
    [DisallowConcurrentExecution]
    public class UploadShopJob : BaseJob
    {
        protected override string JobName => typeof(BaoYangRecordJob).ToString();

        protected override ILog logger { get; }

        public UploadShopJob()
        {
            this.logger = LogManager.GetLogger(JobName);
        }

        public override void Exec()
        {
            string vpn_name = ConfigurationManager.AppSettings["vpn_name"].ToString();
            string vpn_ip = ConfigurationManager.AppSettings["vpn_ip"].ToString();
            string vpn_uname = ConfigurationManager.AppSettings["vpn_uname"].ToString();
            string vpn_pwd = ConfigurationManager.AppSettings["vpn_pwd"].ToString();
            string vpn_presharedkey = ConfigurationManager.AppSettings["vpn_presharedkey"].ToString();
            VPNConnector connector = new VPNConnector(vpn_name, vpn_ip, vpn_uname, vpn_pwd, vpn_presharedkey);

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
                        var shopData = ServicesManager.FetchRoadOperationPermit(17910).FirstOrDefault(); //DalDefault.SelectShopLicenceAndCode(17910);
                        var shopCode = DalDefault.SelectShopCode(17910);

                        if (!string.IsNullOrEmpty(shopData?.RoadLicenceNum))
                        {
                            // 不存在，需要调用接口
                            shopCode = CarArchiveHelper.GetCompanyCode(shopData.RoadLicenceCompanyName, shopData.RoadLicenceNum, shopData.RoadLicenceRegionCode);
                            if (!string.IsNullOrEmpty(shopCode))
                            {
                                DalDefault.UpdateCarArchiveCode(17910, shopCode);
                            }
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
    }
}
