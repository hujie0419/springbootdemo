using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.Request;

namespace Tuhu.Service.Promotion.DataAccess.IRepository
{
    /// <summary>
    /// 区域 - 仓储
    /// </summary>
    public interface IRegionRepository
    {
        /// <summary>
        /// 获取区域信息
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="IsALL">是否全部</param>
        /// <returns></returns>
        ValueTask<List<RegionEntity>> GetRegionListAsync(CancellationToken cancellationToken, bool IsALL = false);
    }
}
