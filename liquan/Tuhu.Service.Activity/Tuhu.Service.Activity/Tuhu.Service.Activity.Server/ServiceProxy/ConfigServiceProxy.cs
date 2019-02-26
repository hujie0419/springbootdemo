using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Config;
using Tuhu.Service.Config.Models.Response;

namespace Tuhu.Service.Activity.Server.ServiceProxy
{
    /// <summary>
    /// 配置服务代理
    /// </summary>
    public class ConfigServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigServiceProxy));

        /// <summary>
        /// 获取开关
        /// </summary>
        /// <param name="switchName"></param>
        /// <returns></returns>
        public async static Task<OperationResult<RuntimeSwitchResponse>> GetOrSetRuntimeSwitchAsync(string switchName)
        {
            using (var client = new ConfigClient())
            {
                var result = await client.GetOrSetRuntimeSwitchAsync(switchName);
                if (!result.Success)
                {
                    Logger.Error($" GetOrSetRuntimeSwitchAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage} ");
                }
                return result;
            }
        }
    }
}
