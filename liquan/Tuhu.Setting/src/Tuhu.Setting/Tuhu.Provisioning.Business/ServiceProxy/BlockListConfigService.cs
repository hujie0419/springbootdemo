using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Config;
using Tuhu.Service.Config.Models;
using Common.Logging;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    /// <summary>
    /// 黑名单配置服务
    /// </summary>
    public class BlockListConfigService
    {
        private readonly ILog _logger;

        public BlockListConfigService()
        {
            _logger = LogManager.GetLogger<BlockListConfigService>();
        }

        /// <summary>
        /// 根据系统和黑名单值查询黑名单列表
        /// </summary>
        /// <param name="blockSystem">系统名称</param>
        /// <param name="blockValue">黑名单值</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PagedModel<BlockListItem>> SearchPagedBlockList
            (string blockSystem, string blockValue, int pageIndex, int pageSize)
        {
            try
            {
                using (var configClient = new BlockListConfigClient())
                {
                    var configResult = await configClient
                        .SearchPagedBlockListAsync(blockSystem, blockValue, pageIndex, pageSize);

                    configResult.ThrowIfException(true);
                    return configResult?.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"查询黑名单列表失败 {blockValue}", ex);
                return null;
            }
        }

        /// <summary>
        /// 新增黑名单
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> AddBlockListItem(BlockListItem item)
        {
            try
            {
                using (var configClient = new BlockListConfigClient())
                {
                    var configResult = await configClient.AddBlockListItemAsync(item);
                    configResult.ThrowIfException(true);
                    return configResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"新增黑名单失败 {item.BlockValue}", ex);
                return false;
            }
        }
    }
}