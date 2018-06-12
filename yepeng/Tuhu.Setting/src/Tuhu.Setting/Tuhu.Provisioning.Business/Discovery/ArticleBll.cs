using Common.Logging;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Framework.DbCore;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.Discovery;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.Business.Discovery
{
    public class ArticleBll : BaseBll
    {
        private static ArticleDal articleDal = new ArticleDal();
        private static CategoryDal CategoryDal = new CategoryDal();
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ArticleBll));


        #region Insert
        /// <summary>
        /// 新增文章
        /// </summary>
        /// <param name="article">文章对象</param>
        /// <returns></returns>
        public static async Task<Article> AddArticle(Article article)
        {
            try
            {
                //插入文章
                await articleDal.AddArticle(article);
                if (article.Type == 5)
                    await InsertOrUpdateES(article);
            }
            catch (Exception ex)
            {
                article = null;
                logger.Error(ex);
            }
            return article;
        }

        #endregion

        #region Update
        /// <summary>
        /// 更新文章状态
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static async Task<Article> UpdateArticleStatus(int articleId, ArticleStatus status)
        {
            Article article = null;

            try
            {
                var result = ArticleDal.UpdateArticleStatus(ProcessConnection.OpenMarketing, articleId, status, status == ArticleStatus.Published ? 1 : 0);
                article = await GetArticleDetailById(articleId);
                if (result && article.Type == 5)
                {
                    await InsertOrUpdateES(article);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return article;
        }

        /// <summary>
        /// 文章的评论数+1
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<Article> UpdateArticleCommentCount(int articleId)
        {
            return await articleDal.UpdateArticleCommentCount(articleId);
        }
        /// <summary>
        /// 修改Article
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public static async Task<Article> UpdateArticle(Article article)
        {
            Article result = new Article();
            try
            {
                result = await articleDal.UpdateArticle(article);
                if (article.Type == 5)
                    await InsertOrUpdateES(article);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 置顶或取消置顶文章
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static bool UpdateArticleTopMost(int articleId, string op)
        {
            var result = false;
            try
            {
                result = ArticleDal.UpdateArticleTopMost(ProcessConnection.OpenMarketing, articleId, op == "top" ? true : false);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        #endregion

        #region Delete
        public static bool DeleteArticleByPkid(int id)
        {
            return ArticleDal.DeleteArticleByPkid(ProcessConnection.OpenMarketing, id);
        }
        #endregion

        #region Get
        /// <summary>
        /// 搜索文章
        /// </summary>
        /// <param name="status">文章状态</param>
        /// <param name="keyword">文章标题</param>
        /// <param name="startCreateDateTime">创建开始时间</param>
        /// <param name="endCreateDateTime">创建结束时间</param>
        /// <param name="pager">分页模型</param>
        /// <returns>文章列表</returns>
        public static async Task<List<Article>> SearchArticle(ArticleStatus status, PagerModel pager, DateTime? startCreateDateTime, DateTime? endCreateDateTime, string pkid = null, string keyword = null, int type = 5)
        {
            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrEmpty(keyword))
                keyword = string.Empty;
            return await articleDal.SearchArticle(ProcessConnection.OpenMarketingReadOnly, status, pager, startCreateDateTime, endCreateDateTime, pkid, keyword, type);
        }


        /// <summary>
        /// 根据标签Id，分页查询文章
        /// </summary>
        /// <param name="CategoryId">标签Id</param>
        /// <param name="pager">分页模型</param>
        /// <returns></returns>
        public static async Task<List<Article>> GetArticlesByCategoryId(int CategoryId, PagerModel pager)
        {
            return await articleDal.SelectArticlesByCategoryId(CategoryId, pager);
        }

        /// <summary>
        /// 根据文章Id查询文章详情显示
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <param name="includeRelateArtilce">是否包含相关文章</param>
        /// <param name="userId">用户的Id</param>
        /// <returns></returns>
        public static async Task<Article> GetArticleDetailById(int articleId, string userId = null)
        {
            var article = await articleDal.SelectArticleDetailById(articleId);

            if (article == null)
                return null;

            Guid userIdGuid;
            //查询当前用户对于此文章的收藏状态
            if (string.IsNullOrEmpty(userId) == false && Guid.TryParse(userId, out userIdGuid))
                article.IsFavorite = await articleDal.IsFavoriteArticle(articleId, userIdGuid);
            return article;
        }



        /// <summary>
        /// 根据标签Id查询相关文章
        /// </summary>
        /// <param name="CategoryId">标签Id</param>
        /// <param name="pager">分页模型</param>
        /// <returns></returns>
        public static async Task<List<RelatedArticle>> GetRelateArticleByArticleId(int articleId, PagerModel pager)
        {
            var article = await GetArticleDetailById(articleId);
            if (article == null)
                return null;
            if (article.CategoryTags != null)
            {
                //查询相关文章

                return await articleDal.SelectRelatedAticleByCategoryId(article.PKID, article.ArticleCategories.FirstOrDefault().CategoryTagId, pager);
            }
            return new List<RelatedArticle>();
        }


        public static int SelectCommentCount(int articleId)
        {
            return articleDal.GetCommentCount(ProcessConnection.OpenMarketingReadOnly, articleId);
        }

        public static int SelectVoteByArticleId(int articleId)
        {
            var result= articleDal.SelectVoteByArticleId(ProcessConnection.OpenMarketingReadOnly, articleId);
            return result;
        }
        #endregion

        #region Record
        /// <summary>
        /// 新增用户操作记录
        /// </summary>
        /// <param name="userOperation"></param>
        /// <returns></returns>
        public static async Task InsertUserOperation(UserOperation userOperation)
        {
            if (userOperation.Operation == UserOperationEnum.Read.ToString())
            {
                await articleDal.InsertReadRecord(userOperation.ArticleId);
            }
            else if (userOperation.Operation == UserOperationEnum.Favorite.ToString())
            {
                ArticleFavoriteDal articleFavoriteDal = new ArticleFavoriteDal();
                var hasFavorite = await articleFavoriteDal.HasFavorite(userOperation);
                if (!hasFavorite)
                {
                    userOperation.Operation = UserOperationEnum.CancelFavorite.ToString();
                }
                await articleDal.InsertFavoriteRecord(userOperation.ArticleId, hasFavorite);
            }
            else if (userOperation.Operation == UserOperationEnum.Share.ToString())
            {
                await articleDal.InsertShareRecord(userOperation.ArticleId);
            }
            if (userOperation.Operation != UserOperationEnum.Read.ToString())
                await articleDal.InsertUserOperation(userOperation);
        }
        #endregion

        #region Utils
        public static async Task<Article> SelectArticleDetailByTitle(string title)
        {
            return await ArticleDal.SelectArticleDetailByTitle(title);
        }
        public static bool UpdateContentByPKID(int pkid, string content)
        {
            return ArticleDal.UpdateContentByPKID(ProcessConnection.OpenMarketing, pkid, content);
        }
        #endregion

        #region 作者管理


        public static bool AddAuthor(CoverAuthor model)
        {
            try
            {
                return ArticleDal.AddAuthor(ProcessConnection.OpenMarketing, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateAuthor(CoverAuthor model)
        {
            try
            {
                return ArticleDal.UpdateAuthor(ProcessConnection.OpenMarketing, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateFieldByPKID(string fieldName, string fieldValue, int PKID)
        {
            try
            {
                return ArticleDal.UpdateFieldByPKID(ProcessConnection.OpenMarketing, fieldName, fieldValue, PKID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeleteByPKID(int PKID)
        {
            try
            {
                return ArticleDal.DeleteByPKID(ProcessConnection.OpenMarketing, PKID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public static List<CoverAuthor> SelectAuthorList(PagerModel pager, string strWhere)
        {
            try
            {
                DataTable dt = ArticleDal.SelectAuthorList(ProcessConnection.OpenMarketing, pager, strWhere);
                return DTConvertCoverAuthor(dt.Rows.Cast<DataRow>().ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<CoverAuthor> SelectAll()
        {
            try
            {
                DataTable dt = ArticleDal.SelectAllAuthor(ProcessConnection.OpenMarketing);
                return DTConvertCoverAuthor(dt.Rows.Cast<DataRow>().ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static CoverAuthor SelectAuthorModelByPKID(int PKID)
        {
            try
            {
                DataRow dr = ArticleDal.SelectAuthorModelByPKID(ProcessConnection.OpenMarketing, PKID);
                return DTConvertCoverAuthor(new List<DataRow>() { dr }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool IsExistByName(string authorName)
        {
            try
            {
                return ArticleDal.IsExistByName(ProcessConnection.OpenMarketing, authorName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool IsExistByName2(string oldName, string newName)
        {
            try
            {
                return ArticleDal.IsExistByName2(ProcessConnection.OpenMarketing, oldName, newName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<CoverAuthor> DTConvertCoverAuthor(List<DataRow> rows)
        {
            List<CoverAuthor> modelList = new List<CoverAuthor>();
            if (rows.Count > 0)
            {
                foreach (DataRow row in rows)
                {
                    modelList.Add(new CoverAuthor()
                    {
                        PKID = row.Field<int>("PKID"),
                        AuthorName = row.Field<string>("AuthorName"),
                        AuthorPhone = row.Field<string>("AuthorPhone"),
                        AuthorHead = row.Field<string>("AuthorHead"),
                        Description = row.Field<string>("Description"),
                        CreateTime = row.Field<DateTime>("CreateTime"),
                        IsDelete = row.Field<bool>("IsDelete")
                    });
                }
            }
            return modelList;
        }

        #endregion

        public static List<ArticleTemp> SelectArticleShow(int top = 10)
        {
            try
            {
                DataTable dt = ArticleDal.SelectArticleShow(ProcessConnection.OpenMarketing, top);
                return DTConvertArticleData(dt.Rows.Cast<DataRow>().ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<ArticleTemp> SelectArticleByWords(string keyword = "")
        {
            try
            {
                DataTable dt = ArticleDal.SelectArticleByWords(ProcessConnection.OpenMarketing, keyword);
                return DTConvertArticleData(dt.Rows.Cast<DataRow>().ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ArticleTemp> SelectArticleByPKIDs(List<ColumnArticle> cas)
        {
            try
            {
                DataTable dt = ArticleDal.SelectArticleByPKIDs(ProcessConnection.OpenMarketing, cas);
                return DTConvertArticleData(dt.Rows.Cast<DataRow>().ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ArticleTemp> DTConvertArticleData(List<DataRow> rows)
        {
            List<ArticleTemp> modelList = new List<ArticleTemp>();
            if (rows.Count > 0)
            {
                foreach (DataRow row in rows)
                {
                    modelList.Add(new ArticleTemp()
                    {
                        PKID = row.Field<int>("PKID"),
                        SmallTitle = row.Field<string>("SmallTitle"),
                        BigTitle = row.Field<string>("BigTitle"),
                    });
                }
            }
            return modelList;
        }

        #region  Article ES
        const string articleIndexName = "hushuo_article";
        const string articleTypeName = "article";

        public static async Task<bool> SyncArticleToES(bool isNew)
        {
            List<Article> data = new List<Article>();
            if (isNew)
            {
                data = articleDal.SearchPublishArticle();
            }
            else
            {
                data = articleDal.SearchShowArticle();
            }

            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    await InsertOrUpdateES(item);
                }
            }
            return true;
        }

        public static List<ArticleES> GetESArticleBykeyWord(string keyWord)
        {
            List<ArticleES> result = new List<ArticleES>();
            try
            {
                var response = ElasticsearchHelper.CreateClient()
                    .Search<ArticleES>(s => s
                    .Index(articleIndexName)
                    .Type(articleTypeName)
                    .Query(q => q
                        .MultiMatch(t => t
                            .Fields(f => f
                                .Field(ae => ae.title))
                           .Query(keyWord).MinimumShouldMatch("100%"))));
                if (response != null && response.IsValid)
                {
                    result = response.Hits.Select(t => t.Source).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static List<ArticleES> GetESArticleById(int id)
        {
            List<ArticleES> result = new List<ArticleES>();
            try
            {
                var response = ElasticsearchHelper.CreateClient()
                    .Search<ArticleES>(s => s
                    .Index(articleIndexName)
                    .Type(articleTypeName)
                    .Query(m => m
                        .Term(mt => mt
                            .Field(ae => ae.id)
                            .Value(id))));
                if (response != null && response.IsValid)
                {
                    result = response.Hits.Select(t => t.Source).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static async Task<bool> InsertOrUpdateES(Article article)
        {
            var result = false;
            if (ConfigurationManager.AppSettings["IsOpen"] == "true")
            {
                try
                {
                    if (article.Type == 1 || article.Type == 0 ||
                        article.Status == ArticleStatus.Published.ToString()
                        || article.Status == ArticleStatus.Withdrew.ToString())
                    {
                        var createIndex = await ElasticsearchHelper.CreateClient()
                            .CreateIndexIfNotExistsAsync(articleIndexName, c => c
                                .Settings(cs => cs.NumberOfShards(2).NumberOfReplicas(1)) //设置副本和分片数
                                .Mappings(cm => cm.MapDefault()
                                    .Map<ArticleES>(m => m
                                        .AutoMap()))
                            );
                        if (createIndex)
                        {
                            if (article.IsShow == 1 || article.Status == ArticleStatus.Published.ToString())
                            {
                                var response = await ElasticsearchHelper.CreateClient()
                                    .IndexAsync(new ArticleES
                                    {
                                        id = article.PKID,
                                        title = article.SmallTitle,
                                        body = "",
                                        created_at = article.CreateDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                        updated_at = article.LastUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                        reply_count = "0",
                                        view_count = "0",
                                        vote_count = "0"
                                    }, i => i.Index(articleIndexName).Type(articleTypeName));
                                if (response.IsValid)
                                    result = true;
                            }
                            else
                            {
                                var response = await ElasticsearchHelper.CreateClient()
                                    .DeleteAsync<ArticleES>(article.PKID.ToString(), d => d.Index(articleIndexName).Type(articleTypeName));
                                if (response.IsValid)
                                    result = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            return result;
        }
        #endregion

        #region  优选文章

        public static ArticleCoverConfig SelectArticleCoverConfig(int ArticleId)
        {
            return ArticleDal.GetYxArticleCoverConfig(ProcessConnection.OpenMarketingReadOnly, ArticleId);
        }

        public static bool UpsertYxArticle(YouXuanArticle article,string user)
        {
            var result = false;
            var articleId = 0;
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Tuhu_Discovery_Db"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    dbhelper.BeginTransaction();
                    if (article.PKID > 0)
                    {
                        articleId = article.PKID;
                        ArticleDal.UpdateYxArticle(dbhelper, article);
                    }
                    else
                    {
                        articleId = ArticleDal.InsertYxArticle(dbhelper, article);
                    }
                    article.CoverConfig.ArticleId = articleId;
                    ArticleDal.DeleteYxArticleCoverConfig(dbhelper, articleId);
                    ArticleDal.DeleteArticleCategoryTag(dbhelper, articleId);
                    if (!string.IsNullOrEmpty(article.CategoryTags))
                    {
                        var categoryIds = JsonConvert.DeserializeObject<List<JObject>>(article.CategoryTags).Select(t => t.Value<int>("key"));
                        foreach (var id in categoryIds)
                        {
                            ArticleDal.InsertArticleCategoryTag(dbhelper, articleId, id);
                        }
                    }
                    ArticleDal.InsertYxArticleCoverConfig(dbhelper, article.CoverConfig);
                    dbhelper.Commit();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            if (result)
            {
                if (article.PKID > 0)
                {
                    LoggerManager.InsertOplog(user, "ArticleTblNew", articleId, "新增优选文章，文章状态：" + article.Status);
                }
                else
                {
                    LoggerManager.InsertOplog(user, "ArticleTblNew", articleId, "修改优选文章，文章状态：" + article.Status);
                }
            }
            return result;
        }

        #endregion

        #region 导购文章
        public static bool UpsertDaoGouArticle(YouXuanArticle article, string user)
        {
            var result = false;
            var articleId = 0;
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Tuhu_Discovery_Db"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    dbhelper.BeginTransaction();
                    if (article.PKID > 0)
                    {
                        articleId = article.PKID;
                        ArticleDal.UpdateDaoGouArticle(dbhelper, article);
                    }
                    else
                    {
                        articleId = ArticleDal.InsertDaoGouArticle(dbhelper, article);
                    }
                    dbhelper.Commit();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            if (result)
            {
                if (article.PKID > 0)
                {
                    LoggerManager.InsertOplog(user, "ArticleTblNew", articleId, "新增导购文章，文章状态：" + article.Status);
                }
                else
                {
                    LoggerManager.InsertOplog(user, "ArticleTblNew", articleId, "修改导购文章，文章状态：" + article.Status);
                }
            }
            return result;
        }
        #endregion
    }
}
