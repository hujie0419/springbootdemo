using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Service.ConfigBase;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    /// <summary>
    /// 基础配置代理
    /// </summary>
    public class ConfigBaseServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigBaseServiceProxy));

        /// <summary>
        /// 获取基础配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetBaseConfigList(string type)
        {
            try
            {
                using (var configClient = new ConfigBaseClient())
                {
                    var configResult = await configClient.GetBaseConfigListAsync(type);
                    configResult.ThrowIfException(true);
                    return configResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"获取基础配置失败：{type}", ex);
                return null;
            }
        }
    }
}
