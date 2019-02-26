using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.KuaiXiu;
using Tuhu.Service.KuaiXiu.Models;

namespace Tuhu.Service.Activity.Server.ServiceProxy
{
    public class KuaiXiuServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(KuaiXiuServiceProxy));

        /// <summary>
        /// 根据TuhuOrderId获取服务码信息
        /// </summary>
        /// <param name="tuhuOrderId"></param>
        /// <returns></returns>
        public async static Task<ServiceCode> FetchServiceCodeByTuhuOrderIdAsync(int tuhuOrderId)
        {
            ServiceCode result = null;
            try
            {
                using (var client = new ServiceCodeClient())
                {
                    var getResult = await client.FetchServiceCodeByTuhuOrderIdAsync(tuhuOrderId);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }
    }
}
