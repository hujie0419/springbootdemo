using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tuhu.WebSite.Web.Activity.Models;
using Tuhu.WebSite.Web.Activity.BusinessFacade;
using Tuhu.WebSite.Component.Discovery.BusinessData;
using Tuhu.WebSite.Component.Discovery.Business;

namespace Tuhu.WebSite.Web.Activity
{
    public static partial class CacheManager
    {
        /// <summary>
        /// 查询所有作者信息
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CoverAuthorModel>> GetCoverAuthorAll() => await GetFromCacheAsync("CoverAuthor", ArticleSystem.SelectCoverAuthor, CacheTimeEnum.Hour);
        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<Component.Discovery.BusinessData.Article> GetArticleDetailById(int articleId) => await GetFromCacheAsync($"DetailById/{articleId}", () => ArticleBll.GetArticleDetailById(articleId), CacheTimeEnum.Short);

        /// <summary>
        /// 文章详情前10条评价
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<Tuple<List<Comment>, int>> SelectCommentsTop10(int articleId) => await GetFromCacheAsync($"DetailCommentsTopNumById/{articleId}", () => CommentBll.SelectCommentsTopNum(articleId, 10), CacheTimeEnum.Short);
        /// <summary>
        /// 相关阅读
        /// </summary>
        /// <param name="categoryTagId"></param>
        /// <returns></returns>
        public static async Task<List<RelatedArticleModel>> SelectRelateArticleByCategoryTagId(int categoryTagId) => await GetFromCacheAsync($"RelatedArticlesById/{categoryTagId}", () => ArticleSystem.SelectRelateArticleByCategoryTagId(categoryTagId), CacheTimeEnum.Hour);
        /// <summary>
        /// 根据文章ID 查询点赞用户
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<List<string>> SelectArticleLikeUserListByArticleId(int articleId) => await GetFromCacheAsync($"VoteUserList/{articleId}", () => ArticleSystem.SelectArticleLikeUserListByArticleId(articleId), CacheTimeEnum.Short);
        /// <summary>
        /// 查询我喜欢的优选文章Ids
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<List<int>> SelectMyFavoriteYouXuanIdsByUserId(Guid userId) => await GetFromCacheAsync($"MyFavoriteYouXuanList/{userId.ToString()}", () => ArticleSystem.SelectMyFavoriteYouXuanIdsByUserId(userId), CacheTimeEnum.Hour);
        /// <summary>
        /// 查询优选文章喜欢数
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<int> SelectYouXuanFavoriteCountByArticleId(int articleId) => await GetFromCacheAsync($"YouXuanFavoriteCount/{articleId}", () => ArticleSystem.SelectYouXuanFavoriteCountByArticleId(articleId), CacheTimeEnum.Day);
        /// <summary>
        /// 查询文章列表并包含优选文章
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<List<ArticleListModel>> SelectArticleListAndYouXuanList(int pageIndex, int pageSize) => await GetFromCacheAsync($"NewArticleList{pageIndex}/{pageSize}", () => ArticleSystem.SelectArticleListAndYouXuanList(pageIndex,pageSize), CacheTimeEnum.Quarter);
    }
}
