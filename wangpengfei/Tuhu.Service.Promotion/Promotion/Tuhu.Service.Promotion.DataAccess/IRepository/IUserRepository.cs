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
    /// 人员 - 仓储
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 人员信息查询数量
        /// </summary>
        /// <param name="request">查询条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> GetUserListCountAsync(GetUserListRequest request, CancellationToken cancellationToken);
        /// <summary>
        /// 人员信息查询
        /// </summary>
        /// <param name="request">查询model</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<UserEntity>> GetUserListAsync(GetUserListRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 登录验证  Demo密码没做加密，线上可以机密或加密加盐
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="PassWord">密码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
         ValueTask<bool> Login(string UserName, string PassWord, CancellationToken cancellationToken);
    }
}
