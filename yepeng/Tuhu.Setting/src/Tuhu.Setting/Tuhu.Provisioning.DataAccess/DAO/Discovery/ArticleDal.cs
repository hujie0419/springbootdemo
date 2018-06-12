using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Framework.DbCore;
using System.Data.Entity;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using Dapper;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess.DAO.Discovery
{
    public class ArticleDal : BaseDal
    {
        private DiscoveryDbContext discoveryDbContext = (DiscoveryDbContext)new DBContextFactory().GetDbContext();
        #region Insert
        /// <summary>
        /// 新增文章
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public async Task<Article> AddArticle(Article article)
        {
            using (var addDbContext = new DiscoveryDbContext())
            {
                //插入文章并且插入文章和标签的关系
                if (string.IsNullOrEmpty(article.CategoryTags) == false)
                {
                    var Categorys = JsonConvert.DeserializeObject<List<JObject>>(article.CategoryTags).Select(t => t.Value<int>("key"));
                    var articleCategorys = await addDbContext.Category.Where(t => Categorys.Contains(t.Id)).ToListAsync();
                    //插入文章
                    addDbContext.Article.Add(article);
                    await addDbContext.SaveChangesAsync();

                    foreach (var Category in articleCategorys)
                    {
                        var articleCategory = new ArticleCategory { ArticleId = article.PKID, CategoryTagId = Category.Id };
                        addDbContext.ArticleCategory.Add(articleCategory);
                    }
                    await addDbContext.SaveChangesAsync();
                }
                else
                {
                    //插入文章
                    addDbContext.Article.Add(article);
                    await addDbContext.SaveChangesAsync();
                }

                return article;
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新文章状态
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <param name="status">状态值</param>
        /// <returns></returns>
        public async Task<Article> UpdateArticleStatus(int articleId, ArticleStatus status)
        {
            var updateArticle = await discoveryDbContext.Article.FirstOrDefaultAsync(article => article.PKID == articleId);
            if (updateArticle == null)
                throw new Exception("文章不存在");

            updateArticle.Status = status.ToString();
            updateArticle.LastUpdateDateTime = DateTime.Now;
            //如果将文章撤回，并且当前文章为置顶状态，则设置为非置顶
            if (status == ArticleStatus.Withdrew && updateArticle.IsTopMost.HasValue && updateArticle.IsTopMost.Value == true)
                updateArticle.IsTopMost = false;

            await discoveryDbContext.SaveChangesAsync();

            if (status == ArticleStatus.Withdrew)
                await InsertDataChangeRecord(updateArticle, DataOperationEnum.Withdrew);
            return updateArticle;
        }

        /// <summary>
        /// 文章的评论数+1
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public async Task<Article> UpdateArticleCommentCount(int articleId)
        {
            var updateArticle = await discoveryDbContext.Article.FirstOrDefaultAsync(article => article.PKID == articleId);
            if (updateArticle == null)
                throw new Exception("文章不存在");

            updateArticle.CommentCountNum = updateArticle.CommentCountNum + 1;
            await discoveryDbContext.SaveChangesAsync();
            return updateArticle;
        }

        /// <summary>
        /// 修改Article
        /// </summary>
        /// <param name="article">Article模型</param>
        /// <returns></returns>
        public async Task<Article> UpdateArticle(Article article)
        {
            using (var updateDbContext = new DiscoveryDbContext())
            {
                var updateArticle = await updateDbContext.Article.FirstOrDefaultAsync(a => a.PKID == article.PKID);
                //var updateArticle = updateDbContext.Article.Include("ArticleCategorys").SingleOrDefault(a => a.Id == article.Id);
                if (updateArticle == null)
                    throw new Exception("文章不存在");

                //清空旧的文章标签关系
                //updateArticle.ArticleCategorys.Clear();


                updateArticle.SmallTitle = article.SmallTitle;
                updateArticle.BigTitle = article.SmallTitle;
                updateArticle.Content = article.ContentHtml;
                updateArticle.ContentHtml = article.ContentHtml;
                updateArticle.CoverImage = article.CoverImage;
                updateArticle.CoverTag = article.CoverTag;
                updateArticle.CoverMode = article.CoverMode;
                updateArticle.CategoryTags = article.CategoryTags;
                updateArticle.Category = article.Category;
                updateArticle.LastUpdateDateTime = article.LastUpdateDateTime;
                updateArticle.Status = article.Status;
                updateArticle.IsShow = article.IsShow;
                if (article.PublishDateTime.HasValue)
                {
                    updateArticle.PublishDateTime = article.PublishDateTime;
                }
                updateArticle.Brief = article.Brief;
                updateArticle.Image = article.Image;
                updateArticle.SmallImage = article.SmallImage;
                updateArticle.ShowImages = article.ShowImages;
                updateArticle.ShowType = article.ShowType;
                updateArticle.IsShowTouTiao = article.IsShowTouTiao;
                updateArticle.QRCodeImg = article.QRCodeImg;
                updateArticle.Type = article.Type;
                //文章存在关联的标签
                if (string.IsNullOrEmpty(article.CategoryTags) == false)
                {
                    //文章关联的标签

                    var addArticleCategorys = JsonConvert.DeserializeObject<List<JObject>>(article.CategoryTags).Select(t => t.Value<int>("key"));
                    var articleCategorys = await updateDbContext.Category.Where(t => addArticleCategorys.Contains(t.Id)).ToListAsync();

                    var oldArtilceCategorys = updateDbContext.ArticleCategory.Where(at => at.ArticleId == updateArticle.PKID);
                    updateDbContext.ArticleCategory.RemoveRange(oldArtilceCategorys);
                    foreach (var Category in articleCategorys)
                    {
                        var articleCategory = new ArticleCategory { ArticleId = article.PKID, CategoryTagId = Category.Id };
                        updateDbContext.ArticleCategory.Add(articleCategory);
                    }
                }

                await updateDbContext.SaveChangesAsync();

                //await InsertDataChangeRecord(updateArticle, DataOperationEnum.Update);

                return article;
            }
        }


        /// <summary>
        /// 置顶或取消置顶文章
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <returns></returns>
        public async Task<int> UpdateArticleTopMost(int articleId, string op)
        {
            using (var updateDbContext = new DiscoveryDbContext())
            {
                var updateArticle = await discoveryDbContext.Article.FirstOrDefaultAsync(a => a.PKID == articleId);

                if (updateArticle == null)
                    throw new Exception("文章不存在");

                //当前为非置顶状态
                if (updateArticle.IsTopMost == null || updateArticle.IsTopMost == false)
                    updateArticle.LastUpdateDateTime = DateTime.Now;

                //修改当前文章为置顶(取消置顶)
                updateArticle.IsTopMost = op == "top" ? true : false;

                await discoveryDbContext.SaveChangesAsync();

                //if (updateArticle.IsTopMost.HasValue && updateArticle.IsTopMost.Value==true)
                //    await InsertDataChangeRecord(updateArticle, DataOperationEnum.TopBest);
                return Success;
            }
        }


        #endregion

        #region Delete
        public static bool DeleteArticleByPkid(SqlConnection conn, int id)
        {
            using (conn)
            {
                string updateSql = "UPDATE Marketing..tbl_Article SET IsDelete=1,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID";
                var sqlParamsInfo = new SqlParameter[]
                {
                        new SqlParameter("@PKID",id)
                };
                int res = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, updateSql, sqlParamsInfo);
                return res > 0;
            }
        }
        #endregion

        #region Select
        /// <summary>
        /// 搜索文章
        /// </summary>
        /// <param name="status">文章状态</param>
        /// <param name="articleTitle">文章标题</param>
        /// <param name="startDateTime">创建开始时间</param>
        /// <param name="endDateTime">创建结束时间</param>
        /// <param name="pager">分页模型</param>
        /// <returns>文章列表</returns>
        public Task<List<Article>> SearchArticle(SqlConnection conn, ArticleStatus status, PagerModel pager, DateTime? startCreateDateTime, DateTime? endCreateDateTime, string pkid, string keyword = "", int type = 5)
        {
            List<Article> result = new List<Article>();
            const string selectValues = @"PKID ,
                                        Catalog ,
                                        Image ,
                                        ShowImages ,
                                        ShowType ,
                                        IsDescribe ,
                                        SmallImage ,
                                        SmallTitle ,
                                        BigTitle ,
                                        TitleColor ,
                                        Brief ,
                                        Content ,
                                        ContentUrl ,
                                        Source ,
                                        PublishDateTime ,
                                        CreateDateTime ,
                                        LastUpdateDateTime ,
                                        ClickCount ,
                                        RedirectUrl ,
                                        Vote ,
                                        Category ,
                                        CategoryTags ,
                                        Heat ,
                                        CommentIsActive ,
                                        IsRead ,
                                        ArticleBanner ,
                                        SmallBanner ,
                                        Bestla ,
                                        HotArticles ,
                                        Type ,
                                        IsShow ,
                                        ShareWX ,
                                        SharePYQ ,
                                        CreatorID ,
                                        CreatorInfo ,
                                        VehicleImage ,
                                        LikeCountNum ,
                                        ReadCountNum ,
                                        IsDelete ,
                                        IsHasBestAnswer ,
                                        RelatedArticleId ,
                                        CoverMode ,
                                        CoverImage ,
                                        Status ,
                                        ShareCountNum ,
                                        CommentCountNum ,
                                        CoverTag ,
                                        IsTopMost ,
                                        Ranking ,
                                        ContentHTML ,
                                        IsShowTouTiao ,
                                        QRCodeImg";

            List<string> conditions = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(pkid))
            {
                int id = Convert.ToInt32(pkid);
                conditions.Add("PKID = @Id");
                parameters.Add("@Id", id);
            }
            else
            {
                string sqlStatus = string.Empty;
                if (status == ArticleStatus.Published)
                {
                    conditions.Add("Status = @Status");
                    conditions.Add("PublishDateTime < GETDATE()");
                    sqlStatus = ArticleStatus.Published.ToString();
                }
                else if (status == ArticleStatus.PrePublish)
                {
                    conditions.Add("Status = @Status");
                    conditions.Add("PublishDateTime > GETDATE()");
                    sqlStatus = ArticleStatus.Published.ToString();
                }
                else if (status != ArticleStatus.All)
                {
                    conditions.Add("Status = @Status");
                    sqlStatus = status.ToString();
                }
                parameters.Add("@Status", sqlStatus);

                if (!string.IsNullOrEmpty(keyword))
                {
                    conditions.Add("SmallTitle LIKE @Keyword");
                    parameters.Add("@Keyword", $"%{keyword}%");
                }

                if (startCreateDateTime.HasValue)
                {
                    conditions.Add("PublishDateTime > @StartCreateDateTime");
                    parameters.Add("@StartCreateDateTime", startCreateDateTime.Value);
                }

                if (endCreateDateTime.HasValue)
                {
                    conditions.Add("PublishDateTime < @EndCreateDateTime");
                    parameters.Add("@EndCreateDateTime", endCreateDateTime.Value);
                }
            }
            parameters.Add("@Type", type);

            conditions.Add("Type = @Type");
            conditions.Add("IsDelete = 0");
            string countSql = $"Select Count(1) From Marketing..tbl_Article WITH(NOLOCK) Where {string.Join(" AND ", conditions)}";
            string selectSql = $"Select {selectValues} From Marketing..tbl_Article WITH(NOLOCK) Where {string.Join(" AND ", conditions)} Order By IsTopMost desc, PublishDateTime desc OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY ";
            int skip = (pager.CurrentPage - 1) * pager.PageSize;
            int pageSize = pager.PageSize;
            parameters.Add("@Skip", skip);
            parameters.Add("@PageSize", pageSize);
            using (conn)
            {
                pager.TotalItem = conn.ExecuteScalar<int>(countSql, commandType: CommandType.Text, param: parameters);
                result = conn.Query<Article>(selectSql, commandType: CommandType.Text, param: parameters).ToList();
            }

            return Task.FromResult(result);
        }

        public List<Article> SearchPublishArticle()
        {
            using (var searchDbContext = new DiscoveryDbContext("name=Tuhu_Discovery_Db_ReadOnly"))
            {
                var articles = searchDbContext.Article.Where(article => (
                (
                    article.Status == ArticleStatus.Published.ToString()
                    || article.Status == ArticleStatus.PrePublish.ToString()
                )
                && article.Type == 5));

                return articles.ToList();
            }
        }

        public List<Article> SearchShowArticle()
        {
            using (var searchDbContext = new DiscoveryDbContext())
            {
                var articles = searchDbContext.Article.Where(article => (
                (
                    article.Type == 1
                    || article.Type == 0
                )
                && article.IsShow == 1));

                return articles.ToList();
            }
        }

        /// <summary>
        /// 根据文章Id查询文章详情
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public async Task<Article> SelectArticleDetailById(int articleId)
        {
            using (var detailDbcontext = new DiscoveryDbContext())
            {
                //查询当前文章详情
                var article = await detailDbcontext.Article.FirstOrDefaultAsync<Article>(a => a.PKID == articleId);
                if (article == null)
                    throw new Exception("文章不存在");
                return article;
            }
        }

        /// <summary>
        /// 用户是否关注了该文章
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> IsFavoriteArticle(int articleId, Guid userId)
        {
            return await DbManager.ReadOnly.AnyAsync<ArticleFavorite>(af => af.PKID == articleId &&
                                                                                                            af.UserId == userId.ToString("B"));
        }


        /// <summary>
        /// 根据标签Id查询相关文章
        /// </summary>
        /// <param name="CategoryId">标签Id</param>
        /// <param name="articleId">文章Id</param>
        /// <param name="pager">分页模型</param>
        /// <returns></returns>
        public async Task<List<RelatedArticle>> SelectRelatedAticleByCategoryId(int articleId, int CategoryId, PagerModel pager)
        {
            var relatedArticles =
                                 from ac in discoveryDbContext.ArticleCategory
                                 where ac.CategoryTagId == CategoryId
                                 join a in discoveryDbContext.Article on ac.ArticleId equals a.PKID
                                 where a.Status == ArticleStatus.Published.ToString() && a.PublishDateTime <= DateTime.Now
                                           && a.PKID != articleId
                                 orderby a.PublishDateTime descending
                                 select new RelatedArticle
                                 {
                                     PKID = a.PKID,
                                     Title = a.SmallTitle,
                                     CoverTag = a.CoverTag,
                                     CoverImage = a.CoverImage,
                                     PublishDateTime = a.PublishDateTime.Value
                                 };
            pager.TotalItem = await relatedArticles.CountAsync();
            return await relatedArticles.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToListAsync();
        }

        /// <summary>
        /// 根据标签Id，分页查询文章
        /// </summary>
        /// <param name="CategoryId">标签Id</param>
        ///<param name="pager">分页模型</param>
        /// <returns></returns>
        public async Task<List<Article>> SelectArticlesByCategoryId(int CategoryId, PagerModel pager)
        {
            var relatedArticles =
                                from ac in discoveryDbContext.ArticleCategory
                                where ac.CategoryTagId == CategoryId
                                join a in discoveryDbContext.Article on ac.ArticleId equals a.PKID
                                where a.Status == ArticleStatus.Published.ToString() && a.PublishDateTime <= DateTime.Now

                                orderby a.PublishDateTime descending
                                select a;
            pager.TotalItem = await relatedArticles.CountAsync();
            return await relatedArticles.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToListAsync();
        }

        public int GetCommentCount(SqlConnection conn, int articleId)
        {
            using (conn)
            {
                return conn.ExecuteScalar<int>(@"SELECT COUNT(1) FROM Marketing..tbl_Comment WITH(NOLOCK) WHERE PKID = @Id", new { Id = articleId }, commandType: CommandType.Text);
            }
        }

        public int SelectVoteByArticleId(SqlConnection conn, int articleId)
        {
            using (conn)
            {
                return conn.ExecuteScalar<int>(@"SELECT COUNT(1) FROM Marketing..YouXuanRelatedOperaction WITH (NOLOCK) WHERE ArticleId=@ArticleId AND Favorite=1", new { ArticleId = articleId }, commandType: CommandType.Text);
            }
        }

        #endregion

        #region User Operation Record

        /// <summary>
        /// 新增用户操作记录
        /// </summary>
        /// <param name="userOperation"></param>
        /// <returns></returns>
        public async Task InsertUserOperation(UserOperation userOperation)
        {
            await DbManager.InsertAsync<UserOperation>(userOperation);
        }

        /// <summary>
        /// 新增阅读记录
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <returns></returns>
        public async Task InsertReadRecord(int articleId)
        {
            var updateArticle = await discoveryDbContext.Article.FirstOrDefaultAsync<Article>(a => a.PKID == articleId);
            if (updateArticle != null)
            {
                updateArticle.ReadCountNum += 1;
                await discoveryDbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 新增收藏记录
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <returns></returns>
        public async Task InsertFavoriteRecord(int articleId, bool hasFavorite)
        {
            var updateArticle = await discoveryDbContext.Article.FirstOrDefaultAsync<Article>(a => a.PKID == articleId);
            if (updateArticle != null)
            {
                int StarCount = hasFavorite ? (updateArticle.LikeCountNum.Value + 1) : (updateArticle.LikeCountNum.Value - 1);
                updateArticle.LikeCountNum = StarCount < 0 ? 0 : StarCount;
                await discoveryDbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 新增分享记录
        /// </summary>
        /// <param name="articleId">文章ID</param>
        /// <returns></returns>
        public async Task InsertShareRecord(int articleId)
        {
            var updateArticle = await discoveryDbContext.Article.FirstOrDefaultAsync<Article>(a => a.PKID == articleId);
            if (updateArticle != null)
            {
                updateArticle.ShareCountNum += 1;
                await discoveryDbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Data Change Record

        /// <summary>
        /// 新增数据变更记录
        /// </summary>
        /// <param name="newArticle"></param>
        /// <returns></returns>
        public async Task InsertDataChangeRecord(Article newArticle, DataOperationEnum operation)
        {
            if (operation == DataOperationEnum.Update)
            {
                var newArticleData = JsonConvert.SerializeObject(newArticle, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var articleChangeLog = new ArticleChangeLog
                {
                    CreateDateTime = DateTime.Now,
                    Operation = operation.ToString(),
                    OperationValue = newArticleData,
                    OperatorId = new Guid(newArticle.CreatorId)
                };
                discoveryDbContext.ArticleChange_Log.Add(articleChangeLog);
                await discoveryDbContext.SaveChangesAsync();
            }
            else
            {
                var articleChangeLog = new ArticleChangeLog
                {
                    CreateDateTime = DateTime.Now,
                    Operation = operation.ToString(),
                    OperationValue = newArticle.PKID.ToString(),
                    OperatorId = new Guid(newArticle.CreatorId)
                };
                discoveryDbContext.ArticleChange_Log.Add(articleChangeLog);
            }

        }


        #endregion

        #region Utils
        public static async Task<Article> SelectArticleDetailByTitle(string title)
        {
            using (var detailDbcontext = new DiscoveryDbContext())
            {
                //查询当前文章详情
                var article = await detailDbcontext.Article.FirstOrDefaultAsync<Article>(a => a.SmallTitle == title);
                if (article == null)
                    throw new Exception("文章不存在");
                return article;
            }
        }
        public static bool UpdateContentByPKID(SqlConnection conn, int pkid, string content)
        {
            using (conn)
            {
                string sql = @"UPDATE Marketing..tbl_Article SET Content=@Content,ContentHTML=@ContentHTML WHERE PKID=@PKID";
                var sqlParamsInfo = new SqlParameter[]
                {
                    new SqlParameter("@Content",content),
                    new SqlParameter("@ContentHTML",content),
                    new SqlParameter("@PKID",pkid)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParamsInfo) > 0;
            }
        }
        #endregion

        #region 作者管理

        public static bool AddAuthor(SqlConnection conn, CoverAuthor model)
        {
            using (conn)
            {
                string insertSql = "INSERT INTO Marketing..tbl_CoverAuthor(AuthorName, AuthorPhone, AuthorHead, [Description], CreateTime, IsDelete) VALUES(@AuthorName, @AuthorPhone, @AuthorHead, @Description, GETDATE(), @IsDelete)";
                var sqlParamsInfo = new SqlParameter[]
                  {
                            new SqlParameter("@AuthorName",model.AuthorName),
                            new SqlParameter("@AuthorPhone",model.AuthorPhone),
                            new SqlParameter("@AuthorHead",model.AuthorHead),
                            new SqlParameter("@Description",model.Description),
                            new SqlParameter("@IsDelete",model.IsDelete)
                  };
                int res = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, insertSql, sqlParamsInfo);
                return res > 0;
            }
        }

        public static bool UpdateAuthor(SqlConnection conn, CoverAuthor model)
        {
            using (conn)
            {
                string updateSql = "UPDATE Marketing..tbl_CoverAuthor SET AuthorName=@AuthorName,AuthorPhone=@AuthorPhone,AuthorHead=@AuthorHead,[Description]=@Description WHERE PKID=@PKID";
                var sqlParamsInfo = new SqlParameter[]
                  {
                            new SqlParameter("@AuthorName",model.AuthorName),
                            new SqlParameter("@AuthorPhone",model.AuthorPhone),
                            new SqlParameter("@AuthorHead",model.AuthorHead),
                            new SqlParameter("@Description",model.Description),
                            new SqlParameter("@PKID",model.PKID)
                  };
                int res = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, updateSql, sqlParamsInfo);
                return res > 0;
            }
        }

        public static bool UpdateFieldByPKID(SqlConnection conn, string fieldName, string fieldValue, int PKID)
        {
            using (conn)
            {
                string updateSql = string.Format("UPDATE Marketing..tbl_CoverAuthor SET {0}='{1}' WHERE PKID='{2}' ", fieldName, fieldValue, PKID);
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, updateSql) > 0;
            }
        }

        public static bool DeleteByPKID(SqlConnection conn, int PKID)
        {
            using (conn)
            {
                string updateSql = string.Format("DELETE FROM Marketing..tbl_CoverAuthor WHERE PKID={0} ", PKID);
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, updateSql) > 0;
            }
        }

        public static DataTable SelectAuthorList(SqlConnection conn, PagerModel pager, string strWhere)
        {
            using (conn)
            {
                object totalCount = SqlHelper.ExecuteScalar(conn, CommandType.Text, "SELECT COUNT(1) FROM Marketing..tbl_CoverAuthor(NOLOCK) WHERE 1=1 " + strWhere);
                pager.TotalItem = Convert.ToInt32(totalCount);
                string querySql = @"
                            SELECT * FROM
                            (
                                SELECT *,ROW_NUMBER() OVER(ORDER BY PKID DESC) AS RowNum FROM Marketing..tbl_CoverAuthor(NOLOCK)
                                WHERE 1=1 {0}
                            ) T
                            WHERE T.RowNum BETWEEN (@PageIndex-1)*@PageSize+1 AND @PageIndex*@PageSize";
                var sqlParams = new[] {
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize)
                 };
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, string.Format(querySql, strWhere), sqlParams);
            }
        }

        public static DataTable SelectAllAuthor(SqlConnection conn)
        {
            using (conn)
            {
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, "SELECT * FROM [Marketing].[dbo].[tbl_CoverAuthor] WITH(NOLOCK) WHERE [IsDelete]=0");
            }
        }

        public static DataRow SelectAuthorModelByPKID(SqlConnection connection, int PKID)
        {
            using (connection)
            {
                return SqlHelper.ExecuteDataRow(connection, CommandType.Text, @"SELECT * FROM Marketing..tbl_CoverAuthor(NOLOCK) WHERE PKID=@PKID", new SqlParameter("@PKID", PKID));
            }
        }

        public static bool IsExistByName(SqlConnection conn, string authorName)
        {
            using (conn)
            {
                string sql = "SELECT COUNT(0) FROM [Marketing].[dbo].[tbl_CoverAuthor](NOLOCK) WHERE AuthorName='" + authorName + "'";
                object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
                return Convert.ToInt32(obj) > 0;
            }
        }
        public static bool IsExistByName2(SqlConnection conn, string oldName, string newName)
        {
            using (conn)
            {
                string sql = "SELECT COUNT(0) FROM [Marketing].[dbo].[tbl_CoverAuthor](NOLOCK) WHERE AuthorName!='" + oldName + "' AND AuthorName='" + newName + "'";
                object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
                return Convert.ToInt32(obj) > 0;
            }
        }


        #endregion


        #region 专题相关文章
        public static DataTable SelectArticleShow(SqlConnection conn, int top = 10)
        {
            using (conn)
            {
                string sql = @"SELECT TOP {0} PKID,SmallTitle,BigTitle FROM Marketing..tbl_Article(NOLOCK)
                                    WHERE IsShow = 1 AND(Type = 1 OR Type = 5) AND PublishDateTime< GETDATE()
                                    ORDER BY PublishDateTime DESC";
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, string.Format(sql, top.ToString()));
            }
        }
        public static DataTable SelectArticleByWords(SqlConnection conn, string keyword)
        {
            using (conn)
            {
                string sql = @"SELECT TOP 100 PKID,SmallTitle,BigTitle FROM Marketing..tbl_Article(NOLOCK)
                                    WHERE IsShow = 1 AND(Type = 1 OR Type = 5) AND PublishDateTime< GETDATE() AND SmallTitle LIKE '%{0}%'
                                    ORDER BY PublishDateTime DESC";
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, string.Format(sql, keyword));
            }
        }

        public static DataTable SelectArticleByPKIDs(SqlConnection conn, List<ColumnArticle> cas)
        {
            var pkids = cas.Select(s => s.PKID.ToString()).ToList();
            string str = string.Join(",", pkids);
            using (conn)
            {
                string sql = @"SELECT PKID,SmallTitle,BigTitle FROM Marketing..tbl_Article(NOLOCK) WHERE PKID IN ({0})";
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, string.Format(sql, str));
            }

        }
        #endregion

        #region  优选文章

        public static int InsertArticleCategoryTag(SqlDbHelper dbhelper, int articleId, int categoryTagId)
        {
            const string sql = @"
               INSERT  INTO Marketing..HD_ArticleCategoryTag
                        ( ArticleId ,
                          CategoryTagId ,
                          Source
	                    )
                VALUES  ( @ArticleId ,
                          @CategoryTagId ,
                          N'YouXuan'
                        );";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ArticleId", articleId);
                cmd.Parameters.AddWithValue("@CategoryTagId", categoryTagId);
                return Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
            };
        }

        public static int DeleteArticleCategoryTag(SqlDbHelper dbhelper, int articleId)
        {
            const string sql = @"DELETE FROM Marketing..HD_ArticleCategoryTag WHERE ArticleId=@ArticleId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ArticleId", articleId);
                return dbhelper.ExecuteNonQuery(cmd);
            };
        }

        public static int InsertYxArticle(SqlDbHelper dbhelper, YouXuanArticle article)
        {
            const string sql = @"
            INSERT  INTO Marketing..tbl_Article
                    ( Image ,
                      ShowImages ,
                      IsDescribe ,
                      SmallImage ,
                      SmallTitle ,
                      BigTitle ,
                      Brief ,
                      Content ,
                      ContentUrl ,
                      PublishDateTime ,
                      CreateDateTime ,
                      LastUpdateDateTime ,
                      CategoryTags ,
                      Type ,
                      IsShow ,
                      IsDelete ,
                      Status ,
                      CoverTag ,
                      IsTopMost ,
                      ContentHTML ,
                      IsShowTouTiao ,
                      QRCodeImg
                    )
            OUTPUT Inserted.PKID
            VALUES  ( '' ,
                      '' ,
                      1 ,
                      '' ,
                      @SmallTitle ,
                      @BigTitle ,
                      @Brief ,
                      @Content ,
                      '' ,
                      @PublishDateTime ,
                      GETDATE() ,
                      GETDATE() ,
                      @CategoryTags ,
                      9 ,
                      @IsShow ,
                      0 ,
                      @Status ,
                      @CoverTag ,
                      0 ,
                      @ContentHTML ,
                      0 ,
                      @QRCodeImg
                    );";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@SmallTitle", article.SmallTitle);
                cmd.Parameters.AddWithValue("@BigTitle", article.SmallTitle);
                cmd.Parameters.AddWithValue("@Brief", article.Brief);
                cmd.Parameters.AddWithValue("@Content", article.ContentHtml);
                cmd.Parameters.AddWithValue("@PublishDateTime", article.PublishDateTime);
                cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@CategoryTags", article.CategoryTags);
                cmd.Parameters.AddWithValue("@IsShow", article.IsShow);
                cmd.Parameters.AddWithValue("@Status", article.Status);
                cmd.Parameters.AddWithValue("@CoverTag", article.CoverTag);
                cmd.Parameters.AddWithValue("@ContentHTML", article.ContentHtml);
                cmd.Parameters.AddWithValue("@QRCodeImg", article.QRCodeImg);
                return Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
            };
        }

        public static int UpdateYxArticle(SqlDbHelper dbhelper, YouXuanArticle article)
        {
            const string sql = @"
            UPDATE  Marketing..tbl_Article
            SET     SmallTitle = @SmallTitle ,
                    BigTitle = @BigTitle ,
                    Brief = @Brief ,
                    Content = @Content ,
                    PublishDateTime = @PublishDateTime ,
                    CategoryTags = @CategoryTags ,
                    IsShow = @IsShow ,
                    Status = @Status ,
                    CoverTag = @CoverTag ,
                    ContentHTML = @ContentHTML ,
                    QRCodeImg = @QRCodeImg ,
                    LastUpdateDateTime = GETDATE()
            WHERE   PKID = @PKID;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@SmallTitle", article.SmallTitle);
                cmd.Parameters.AddWithValue("@BigTitle", article.SmallTitle);
                cmd.Parameters.AddWithValue("@Brief", article.Brief);
                cmd.Parameters.AddWithValue("@Content", article.ContentHtml);
                cmd.Parameters.AddWithValue("@PublishDateTime", article.PublishDateTime);
                cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@CategoryTags", article.CategoryTags);
                cmd.Parameters.AddWithValue("@IsShow", article.IsShow);
                cmd.Parameters.AddWithValue("@Status", article.Status);
                cmd.Parameters.AddWithValue("@CoverTag", article.CoverTag);
                cmd.Parameters.AddWithValue("@ContentHTML", article.ContentHtml);
                cmd.Parameters.AddWithValue("@QRCodeImg", article.QRCodeImg);
                cmd.Parameters.AddWithValue("@PKID", article.PKID);
                return dbhelper.ExecuteNonQuery(cmd);
            };
        }

        public static ArticleCoverConfig GetYxArticleCoverConfig(SqlConnection conn, int articleId)
        {
            const string sql = @"SELECT PKID,ArticleId,CoverType,CoverImg,CoverVideo,OtherImg,Source,CreateTime,UpdateTime FROM Marketing..ArticleCoverConfig WITH (NOLOCK) WHERE ArticleId=@ArticleId";
            return conn.Query<ArticleCoverConfig>(sql, new { ArticleId = articleId }, commandType: CommandType.Text).SingleOrDefault();
        }


        public static bool UpdateArticleStatus(SqlConnection conn, int articleId, ArticleStatus status, int isShow)
        {
            using (conn)
            {
                const string sql = @"UPDATE Marketing..tbl_Article SET Status=@Status,IsShow=@IsShow,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID";
                var parameter = new SqlParameter[]
                {
                        new SqlParameter("@PKID",articleId),
                        new SqlParameter("@Status",status.ToString()),
                        new SqlParameter("@IsShow",isShow)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
            }
        }

        public static bool UpdateArticleTopMost(SqlConnection conn, int articleId,bool isTopMost)
        {
            using (conn)
            {
                const string sql = @"UPDATE Marketing..tbl_Article SET IsTopMost=@IsTopMost,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID";
                var parameter = new SqlParameter[]
                {
                        new SqlParameter("@PKID",articleId),
                        new SqlParameter("@IsTopMost",isTopMost)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
            }
        }

        public static int InsertYxArticleCoverConfig(SqlDbHelper dbhelper, ArticleCoverConfig config)
        {
            const string sql = @"
            INSERT  INTO Marketing..ArticleCoverConfig
                    ( ArticleId ,
                      CoverType ,
                      CoverImg ,
                      CoverVideo ,
                      OtherImg ,
                      Source ,
                      CreateTime ,
                      UpdateTime
                    )
            VALUES  ( @ArticleId ,
                      @CoverType ,
                      @CoverImg ,
                      @CoverVideo ,
                      @OtherImg ,
                      @Source ,
                      GETDATE() , 
                      GETDATE()  
                    );";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ArticleId", config.ArticleId);
                cmd.Parameters.AddWithValue("@CoverType", config.CoverType);
                cmd.Parameters.AddWithValue("@CoverImg", config.CoverImg);
                cmd.Parameters.AddWithValue("@CoverVideo", config.CoverVideo);
                cmd.Parameters.AddWithValue("@OtherImg", config.OtherImg);
                cmd.Parameters.AddWithValue("@Source", config.Source);
                return dbhelper.ExecuteNonQuery(cmd);
            };
        }

        public static int DeleteYxArticleCoverConfig(SqlDbHelper dbhelper, int articleId)
        {
            const string sql = @"DELETE FROM Marketing..ArticleCoverConfig WHERE ArticleId=@ArticleId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ArticleId", articleId);
                return dbhelper.ExecuteNonQuery(cmd);
            };
        }
        #endregion

        #region
        public static int InsertDaoGouArticle(SqlDbHelper dbhelper, YouXuanArticle article)
        {
            const string sql = @"
            INSERT  INTO Marketing..tbl_Article
                    ( Image ,
                      ShowImages ,
                      IsDescribe ,
                      SmallImage ,
                      SmallTitle ,
                      BigTitle ,
                      Brief ,
                      Content ,
                      ContentUrl ,
                      PublishDateTime ,
                      CreateDateTime ,
                      LastUpdateDateTime ,
                      CoverMode ,
                      Type ,
                      IsShow ,
                      IsDelete ,
                      Status ,
                      IsTopMost ,
                      ContentHTML ,
                      IsShowTouTiao ,
                      CoverImage
                    )
            OUTPUT Inserted.PKID
            VALUES  ( '' ,
                      '' ,
                      1 ,
                      '' ,
                      @SmallTitle ,
                      @BigTitle ,
                      @Brief ,
                      @Content ,
                      '' ,
                      @PublishDateTime ,
                      GETDATE() ,
                      GETDATE() ,
                      'TopBigPicMode' ,
                      98 ,
                      @IsShow ,
                      0 ,
                      @Status ,
                      0 ,
                      @ContentHTML ,
                      0 ,
                      @CoverImage
                    );";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@SmallTitle", article.SmallTitle);
                cmd.Parameters.AddWithValue("@BigTitle", article.SmallTitle);
                cmd.Parameters.AddWithValue("@Brief", article.Brief);
                cmd.Parameters.AddWithValue("@Content", article.ContentHtml);
                cmd.Parameters.AddWithValue("@PublishDateTime", article.PublishDateTime);
                cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsShow", article.IsShow);
                cmd.Parameters.AddWithValue("@Status", article.Status);
                cmd.Parameters.AddWithValue("@ContentHTML", article.ContentHtml);
                cmd.Parameters.AddWithValue("@CoverImage", article.CoverImage);
                return Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
            };
        }

        public static int UpdateDaoGouArticle(SqlDbHelper dbhelper, YouXuanArticle article)
        {
            const string sql = @"
            UPDATE  Marketing..tbl_Article
            SET     SmallTitle = @SmallTitle ,
                    BigTitle = @BigTitle ,
                    Brief = @Brief ,
                    Content = @Content ,
                    PublishDateTime = @PublishDateTime ,
                    IsShow = @IsShow ,
                    Status = @Status ,
                    ContentHTML = @ContentHTML ,
                    CoverImage = @CoverImage ,
                    LastUpdateDateTime = GETDATE()
            WHERE   PKID = @PKID;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@SmallTitle", article.SmallTitle);
                cmd.Parameters.AddWithValue("@BigTitle", article.SmallTitle);
                cmd.Parameters.AddWithValue("@Brief", article.Brief);
                cmd.Parameters.AddWithValue("@Content", article.ContentHtml);
                cmd.Parameters.AddWithValue("@PublishDateTime", article.PublishDateTime);
                cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsShow", article.IsShow);
                cmd.Parameters.AddWithValue("@Status", article.Status);
                cmd.Parameters.AddWithValue("@ContentHTML", article.ContentHtml);
                cmd.Parameters.AddWithValue("@CoverImage", article.CoverImage);
                cmd.Parameters.AddWithValue("@PKID", article.PKID);
                return dbhelper.ExecuteNonQuery(cmd);
            };
        }
        #endregion
    }
}
