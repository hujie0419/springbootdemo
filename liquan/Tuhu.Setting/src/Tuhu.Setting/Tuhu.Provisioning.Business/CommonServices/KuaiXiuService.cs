using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.KuaiXiu;
using Tuhu.Service.KuaiXiu.Enums;
using Tuhu.Service.KuaiXiu.Models;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class KuaiXiuService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(KuaiXiuService));

        public static IEnumerable<ServiceCode> GetServiceCodeDetailsByCodes(IEnumerable<string> serviceCodes)
        {
            IEnumerable<ServiceCode> result = null;
            try
            {
                using (var client = new ServiceCodeClient())
                {
                    var serviceResult = client.SelectServiceCodeDetailsByCodes(serviceCodes);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result ?? new List<ServiceCode>();
        }

        public static string GetServiceCodeStatusDescription(ServiceCode code)
        {
            string result = string.Empty;
            switch (code.Status)
            {
                case ServiceCodeStatusType.Cancelled:
                    result = "已取消";
                    break;
                case ServiceCodeStatusType.Created:
                    result = "已创建";
                    break;
                case ServiceCodeStatusType.Overdued:
                    result = "已过期";
                    break;
                case ServiceCodeStatusType.Reverted:
                    result = "已撤销";
                    break;
                case ServiceCodeStatusType.SmsSent:
                    result = "已发放";
                    break;
                case ServiceCodeStatusType.Verified:
                    result = "已核销";
                    break;
            }
            if (code.Source.Contains("Revert"))
                result = "已作废";
            return result;
        }
    }
}
