using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.ConfigBase;
using Microsoft.Extensions.Logging;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{

    public interface IConfigBaseService
    {
        Task<OperationResult<Dictionary<string, string>>> GetBaseConfigListAsync(string type, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 服务代理 - 基础配置
    /// </summary>
    public class ConfigBaseService: IConfigBaseService
    {
        private readonly ILogger _logger;
        private IConfigBaseClient _Client;


        public ConfigBaseService(ILogger<ConfigBaseService> Logger, IConfigBaseClient IConfigBaseClient)
        {
            _logger = Logger;
            _Client = IConfigBaseClient;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="type"></param>
       /// <param name="cancellationToken"></param>
       /// <returns></returns>
        public async  Task<OperationResult<Dictionary<string, string>>> GetBaseConfigListAsync(string type, CancellationToken cancellationToken = default)
        {
            var result = await _Client.GetBaseConfigListAsync(type, cancellationToken).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"ConfigBaseService GetBaseConfigListAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }
    }
}
