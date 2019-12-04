using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;

namespace Tuhu.Service.Promotion.DataAccess.IRepository
{
    /// <summary>
    /// 渠道 仓储
    /// </summary>
    public interface IChannelDictionariesRepository
    {
        /// <summary>
        /// 获取所有渠道
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<ChannelDictionariesEntity>> GetAllAsync(CancellationToken cancellationToken);
    }
}
