using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Promotion.DataAccess;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.QueryModel;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using Tuhu.Service.Promotion.DataAccess.Repository;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Model;
using Tuhu.Models;

namespace Tuhu.Service.Promotion.Server.Manager
{
    public interface IUserActivityApplyManager
    {
        /// <summary>
        /// 活动申请信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<PagedModel<GetUserActivityApplyResponse>> GetUserActivityApplyListAsync(
            GetUserActivityApplyListRequest request,
            CancellationToken cancellationToken);


        /// <summary>
        /// 新增活动申请
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<OperationResult<bool>> CreateUserActivityApplyAsync(
            CreateUserActivityApplyRequest request,
            CancellationToken cancellationToken);

        /// <summary>
        /// 批量通过活动申请
        /// </summary>
        /// <param name="PKIDs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> BatchPassUserActivityApplyByPKIDsAsync(List<int> PKIDs, CancellationToken cancellationToken);

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> DeleteUserActivityApplyByPKIDAsync(int PKID,
            CancellationToken cancellationToken);

        /// <summary>
        /// 获取可自动审核的活动申请数据
        /// </summary>
        /// <param name="AreaIDs"></param>
        /// <param name="minPKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<int>> GetAutoPassUserActivityApplyPKIDsAsync(GetAutoPassUserActivityApplyPKIDsRequest request, CancellationToken cancellationToken);
    }

    public class UserActivityApplyManager : IUserActivityApplyManager
    {
        private readonly ILogger _logger;
        private readonly ICacheHelper _ICacheHelper;
        private readonly IUserActivityApplyRepository _IUserActivityApplyRepository;

        /// <summary>
        /// 活动申请 逻辑层
        /// </summary>
        /// <param name="Logger"></param>
        /// <param name="ICacheHelper"></param>
        /// <param name="IUserActivityApplyRepository"></param>
        public UserActivityApplyManager(ILogger<CouponManager> Logger, ICacheHelper ICacheHelper,
            IUserActivityApplyRepository IUserActivityApplyRepository)
        {
            _logger = Logger;
            _ICacheHelper = ICacheHelper;
            _IUserActivityApplyRepository = IUserActivityApplyRepository;
        }

        /// <summary>
        /// 活动申请信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<PagedModel<GetUserActivityApplyResponse>> GetUserActivityApplyListAsync(
            GetUserActivityApplyListRequest request,
            CancellationToken cancellationToken)
        {
            PagedModel<GetUserActivityApplyResponse> result = new PagedModel<GetUserActivityApplyResponse>()
            {
                Pager = new PagerModel()
                {
                    CurrentPage = request.CurrentPage,
                    PageSize = request.PageSize
                }
            };
            try
            {
                var queryModel = ObjectMapper.ConvertTo<GetUserActivityApplyListRequest, GetUserActivityApplyListQueryModel>(request);
                int count = await _IUserActivityApplyRepository.GetUserActivityApplyListCountAsync(queryModel, cancellationToken).ConfigureAwait(false);
                result.Pager.Total = count;
                if (count > 0)
                {
                    var entities = await _IUserActivityApplyRepository.GetUserActivityApplyListAsync(queryModel, cancellationToken).ConfigureAwait(false);
                    List<GetUserActivityApplyResponse> source = ObjectMapper.ConvertTo<UserActivityApplyEntity, GetUserActivityApplyResponse>(entities).ToList();
                    result.Source = source;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"UserActivityApplyManager GetUserActivityApplyListAsync Exception", ex);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取可自动审核的活动申请数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<int>> GetAutoPassUserActivityApplyPKIDsAsync(GetAutoPassUserActivityApplyPKIDsRequest request, CancellationToken cancellationToken)
        {
            List<int> updatePKIDs = await _IUserActivityApplyRepository.
                   GetAutoPassUserActivityApplyPKIDsAsync(request, cancellationToken).ConfigureAwait(false);
            return updatePKIDs;
        }


        /// <summary>
        /// 批量通过活动申请
        /// </summary>
        /// <param name="PKIDs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> BatchPassUserActivityApplyByPKIDsAsync(List<int> PKIDs, CancellationToken cancellationToken)
        {
            try
            {
                return await _IUserActivityApplyRepository.
                    BatchPassUserActivityApplyByPKIDsAsync(PKIDs, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error($"UserActivityApplyManager AutoPassUserActivityApplyAsync Exception", ex);
                throw;
            }
        }

        /// <summary>
        /// 新增活动申请
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<OperationResult<bool>> CreateUserActivityApplyAsync(
            CreateUserActivityApplyRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var existQueryModel = new GetUserActivityApplyListQueryModel()
                {
                    ActivityID = request.ActivityId,
                    CurrentPage = 1,
                    PageSize = 1,
                    UserId = request.UserId

                };

                var existApply = await _IUserActivityApplyRepository.GetUserActivityApplyListCountAsync(existQueryModel,
                   cancellationToken).ConfigureAwait(false);
                if (existApply > 0)
                {
                    return OperationResult.FromError<bool>("-1", "已存在");
                }
                else
                {
                    var result = await _IUserActivityApplyRepository.CreateUserActivityApplyAsync(request,
                        cancellationToken).ConfigureAwait(false);
                    return OperationResult.FromResult<bool>(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"UserActivityApplyManager CreateUserActivityApplyAsync Exception", ex);
                throw;
            }
        }
        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> DeleteUserActivityApplyByPKIDAsync(int PKID,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _IUserActivityApplyRepository.
                    DeleteUserActivityApplyByPKIDAsync(PKID, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error($"UserActivityApplyManager AutoPassUserActivityApplyAsync Exception", ex);
                throw;
            }
        }
    }
}
