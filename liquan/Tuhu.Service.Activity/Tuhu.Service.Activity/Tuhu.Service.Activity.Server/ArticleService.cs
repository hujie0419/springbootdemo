using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class ArticleService : IArticleService
    {
        /// <summary>
        /// 查询发现首页关注列表内容
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="page">分页模板</param>
        /// <returns></returns>
        public Task<OperationResult<PagedModel<HomePageTimeLineRequestModel>>> SelectDiscoveryHomeAsync(string userId, PagerModel page,int version) => OperationResult.FromResultAsync(ArticleManager.SelectDiscoveryHomeAsync(userId, page, version));
       
    }
}
