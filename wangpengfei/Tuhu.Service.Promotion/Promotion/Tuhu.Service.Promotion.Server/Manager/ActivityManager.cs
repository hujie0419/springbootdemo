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
using Tuhu.Models;

namespace Tuhu.Service.Promotion.Server.Manager
{
    public interface IActivityManager
    {
        /// <summary>
        /// 获取所有活动列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<PagedModel<GetActivityResponse>> GetActivityListAsync(GetActivityListRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 获取所有活动列表
        /// </summary>
        /// <param name="ActivityID">活动ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<GetActivityResponse> GetActivityInfoAsync(int ActivityID, CancellationToken cancellationToken);
    }

    public class ActivityManager : IActivityManager
    {
        private readonly ILogger _logger;
        private readonly ICacheHelper _ICacheHelper;
        private readonly IActivityRepository _IActivityRepository;

        /// <summary>
        /// 活动 逻辑层
        /// </summary>
        /// <param name="Logger"></param>
        /// <param name="ICacheHelper"></param>
        /// <param name="IActivityRepository"></param>
        public ActivityManager(ILogger<CouponManager> Logger,ICacheHelper ICacheHelper,
            IActivityRepository IActivityRepository)
        {
            _logger = Logger;
            _ICacheHelper = ICacheHelper;
            _IActivityRepository = IActivityRepository;
        }

        /// <summary>
        /// 获取所有活动列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<PagedModel<GetActivityResponse>> GetActivityListAsync(GetActivityListRequest request,  CancellationToken cancellationToken)
        {
            PagedModel<GetActivityResponse> result = new PagedModel<GetActivityResponse>()
            {
                Pager = new PagerModel()
                {
                    CurrentPage = request.CurrentPage,
                    PageSize = request.PageSize
                }
            };
            try
            {
                int count = await _IActivityRepository.GetActivityListCountAsync(request, cancellationToken).ConfigureAwait(false);
                result.Pager.Total = count;
                if (count > 0)
                {
                    var entities = await _IActivityRepository.GetActivityListAsync(request, cancellationToken).ConfigureAwait(false);
                    List<GetActivityResponse> source = ObjectMapper.ConvertTo<ActivityEntity, GetActivityResponse>(entities).ToList();
                    result.Source = source;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"ActivityManager GetAllActivityListAsync Exception", ex);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="ActivityID">活动ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<GetActivityResponse> GetActivityInfoAsync(int ActivityID, CancellationToken cancellationToken)
        {
            GetActivityResponse result = new GetActivityResponse();
            try
            {
                var entities = await _IActivityRepository.GetActivityInfoAsync(ActivityID, cancellationToken).ConfigureAwait(false);
                result = ObjectMapper.ConvertTo<ActivityEntity, GetActivityResponse>(entities);
            }
            catch (Exception ex)
            {
                _logger.Error($"ActivityManager GetActivityAsync Exception", ex);
                throw;
            }
            return result;
        }
    }
}
