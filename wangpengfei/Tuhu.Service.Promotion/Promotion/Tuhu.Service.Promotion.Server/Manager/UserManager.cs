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
    public interface IUserManager
    {
        /// <summary>
        /// 人员信息查询
        /// </summary>
        /// <param name="UserModel">查询条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<PagedModel<GetUserListResponse>> GetUserListAsync(GetUserListRequest UserModel, CancellationToken cancellationToken);

        /// <summary>
        /// 登录验证  Demo密码没做加密，线上可以机密或加密加盐
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="PassWord">密码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> Login(string UserName, string PassWord, CancellationToken cancellationToken);
    }

    public class UserManager : IUserManager
    {
        private readonly ILogger _logger;
        private readonly ICacheHelper _ICacheHelper;
        private readonly IUserRepository _IUserRepository;

        /// <summary>
        /// 用户 逻辑层
        /// </summary>
        /// <param name="Logger"></param>
        /// <param name="ICacheHelper"></param>
        /// <param name="IUserRepository"></param>
        public UserManager(ILogger<CouponManager> Logger, ICacheHelper ICacheHelper, IUserRepository IUserRepository)
        {
            _logger = Logger;
            _ICacheHelper = ICacheHelper;
            _IUserRepository = IUserRepository;
        }

        /// <summary>
        /// 人员信息查询
        /// </summary>
        /// <param name="request">查询条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<PagedModel<GetUserListResponse>> GetUserListAsync(GetUserListRequest request, CancellationToken cancellationToken)
        {
            PagedModel<GetUserListResponse> result = new PagedModel<GetUserListResponse>()
            {
                Pager = new PagerModel()
                {
                    CurrentPage = request.CurrentPage,
                    PageSize = request.PageSize
                }
            };
            try
            {
                int count = await _IUserRepository.GetUserListCountAsync(request, cancellationToken).ConfigureAwait(false);
                result.Pager.Total = count;
                if (count > 0)
                {
                    var entities = await _IUserRepository.GetUserListAsync(request, cancellationToken).ConfigureAwait(false);
                    List<GetUserListResponse> source = ObjectMapper.ConvertTo<UserEntity, GetUserListResponse>(entities).ToList();
                    result.Source = source;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"UserManager GetUserListAsync Exception", ex);
                throw;
            }
            return result;
        }


        /// <summary>
        /// 登录验证  Demo密码没做加密，线上可以机密或加密加盐
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="PassWord">密码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> Login(string UserName, string PassWord, CancellationToken cancellationToken)
        {
            try
            {
                return (await _IUserRepository.Login(UserName, PassWord, cancellationToken).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                _logger.Error($"UserManager Login Exception", ex);
                throw;
            }
        }
    }
}
