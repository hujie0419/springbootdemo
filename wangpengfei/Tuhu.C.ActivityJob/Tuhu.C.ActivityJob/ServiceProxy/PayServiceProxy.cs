using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Pay;
using Tuhu.Service.Pay.Models;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    public class PayServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PayServiceProxy));

        /// <summary>
        /// 微信付款（红包、企业付款）支付状态查询
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="payWay"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
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
                    if (!getResult.Success)
                    {
                        Logger.Warn($"QueryWxPayStatus,微信付款（红包、企业付款）支付状态查询接口失败，message:{getResult.ErrorMessage}");
                    }
                    else
                    {
                        result = getResult.Result ?? new List<WxPayStatusItem>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"QueryWxPayStatus接口异常，异常信息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
            return result;
        }
    }
}
