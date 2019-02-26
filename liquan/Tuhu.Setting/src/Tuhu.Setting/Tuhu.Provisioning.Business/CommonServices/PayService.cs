using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Pay;
using Tuhu.Service.Pay.Models;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public static class PayService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(PayService));

        public static Tuple<bool, string> SendRedBag(string openId, decimal money, string channel,
            string wishing, string actName, string remark, string productCategory, string paymentType,
            string payWay, string requestPlatformCode, string terminalType)
        {
            var result = false;
            var errorMsg = string.Empty;
            try
            {
                using (var client = new PayClient())
                {
                    var getResult = client.Wx_SendRedBag(new WxSendRedBagRequest()
                    {
                        OpenId = openId,
                        ProductCategory = productCategory,
                        PaymentType = paymentType,
                        PayWay = payWay,
                        RequestPlatformCode = requestPlatformCode,
                        TerminalType = terminalType,
                        Money = money,
                        Channel = channel,
                        Wishing = wishing,
                        ActName = actName,
                        Remark = remark
                    });
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                    errorMsg = getResult.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return Tuple.Create(result, errorMsg);
        }

        public static IEnumerable<WxPayStatusItem> QueryWxPayStatus(string channel, string payWay, List<string> orders)
        {
            IEnumerable<WxPayStatusItem> result = new List<WxPayStatusItem>();
            try
            {
                using (var client = new PayClient())
                {
                    var getResult = client.QueryWxPayStatus(new QueryWxPayStatusRequest()
                    {
                        Channel = channel,
                        PayWay = payWay,
                        Orders = orders
                    });
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
    }
}
