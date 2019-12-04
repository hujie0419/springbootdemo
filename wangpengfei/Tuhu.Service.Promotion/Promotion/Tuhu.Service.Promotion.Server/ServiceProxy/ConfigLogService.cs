using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tuhu.Service.ConfigLog;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{

    public interface IConfigLogService
    {
        Task<OperationResult<bool>> InsertDefaultLogQueue(string type, string log);
    }
    /// <summary>
    /// 配置日志服务
    /// </summary>
    public class ConfigLogService: IConfigLogService
    {
        private readonly ILogger _logger;
        private IConfigLogClient _Client;

        public ConfigLogService(ILogger<ConfigBaseService> Logger, IConfigLogClient IConfigLogClient)
        {
            _logger = Logger;
            _Client = IConfigLogClient;
        }

        /// <summary>
        /// 将日志插入到队列
        /// </summary>
        /// <param name="type"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> InsertDefaultLogQueue(string type, string log)
        {
            var result = await _Client.InsertDefaultLogQueueAsync(type, log).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"ConfigLogService InsertDefaultLogQueue fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }
    }
}
