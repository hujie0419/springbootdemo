using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Promotion.DataAccess;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using Tuhu.Service.Promotion.DataAccess.Repository;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Model;

namespace Tuhu.Service.Promotion.Server.Manager
{
    public interface IRegionManager
    {
        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="IsALL">是否全部(默认否)</param>
        /// <returns></returns>
        ValueTask<List<GetRegionListResponse>> GetRegionListAsync(CancellationToken cancellationToken, bool IsALL = false);
    }

    public class RegionManager : IRegionManager
    {
        private readonly ILogger _logger;
        private readonly ICacheHelper _ICacheHelper;
        private readonly IRegionRepository _RegionRepository;

        /// <summary>
        /// 区域 逻辑层
        /// </summary>
        /// <param name="Logger"></param>
        /// <param name="ICacheHelper"></param>
        /// <param name="IRegionRepository"></param>
        public RegionManager(ILogger<CouponManager> Logger,ICacheHelper ICacheHelper,IRegionRepository IRegionRepository)
        {
            _logger = Logger;
            _ICacheHelper = ICacheHelper;
            _RegionRepository = IRegionRepository;
        }

        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="IsALL">是否全部(默认否)</param>
        /// <returns></returns>
        public async ValueTask<List<GetRegionListResponse>> GetRegionListAsync(CancellationToken cancellationToken, bool IsALL = false)
        {
            List<GetRegionListResponse> result = new List<GetRegionListResponse>();
            try
            {
                var entities = await _RegionRepository.GetRegionListAsync(cancellationToken, IsALL).ConfigureAwait(false);
                result = ObjectMapper.ConvertTo<RegionEntity, GetRegionListResponse>(entities).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error($"RegionManager GetRegionListAsync Exception", ex);
                throw;
            }
            return result;
        }
    }
}
