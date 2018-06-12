using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.WebSite.Web.Activity.Models;
using Tuhu.WebSite.Web.Activity.DataAccess;
using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Component.SystemFramework;
using System.Threading.Tasks;
using Tuhu.Nosql;

namespace Tuhu.WebSite.Web.Activity
{
    public static class ArticleSystem
    {
        /// <summary>
        /// 获取图文推送
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> SelectArticlesForApi(PagerModel pager, string Category)
        {
            var dt = Articles.SelectArticlesForApi(pager, Category);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ArticleModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleModel(x)).ToArray();
        }



        /// <summary>
        /// 图文推送=>查询所有的类别
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CategoryListModel> SelectAllCategory()
        {
            var dt = Articles.SelectAllCategory();
            if (dt == null || dt.Rows.Count == 0)
            {
                return new CategoryListModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new CategoryListModel(x)).ToArray();
        }

        /// <summary>
        /// 查询文章用户评论总数
        /// </summary>
        /// <param name="pkId"></param>
        /// <returns></returns>
        public static object SelectCommentCount(string pkId)
        {
            var CommentCount = Articles.SelectCommentCount(pkId);
            if (string.IsNullOrWhiteSpace(CommentCount))
            {
                CommentCount = "0";
            }
            return CommentCount;
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public static int AddComment(ArticleCommentModel ac)
        {
            return Articles.AddComment(ac);
        }

        /// <summary>
        /// 每次操作完修改热度
        /// </summary>
        /// <param name="pkId"></param>
        /// <param name="Num"></param>
        /// <returns></returns>
        public static int AddHeat(int pkId, int Num)
        {
            return Articles.AddHeat(pkId, Num);
        }

        /// <summary>
        /// 根据关键字
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> SelectArticlesByWord(string keyWord)
        {
            var dt = Articles.SelectArticlesByWord(keyWord);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ArticleModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleModel(x)).ToArray();
        }


        public static int UpdateArticleClick(int PKID)
        {
            return Articles.UpdateArticleClick(PKID);
        }

        #region 新版图文推送
        /// <summary>
        /// 查询所有文章
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="Category"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> SelectNewArticlesForApi(int pIndex, int pSize, string Category, string userId, string version, out int totalCount)
        {
            var dt = Articles.SelectNewArticlesForApi(pIndex, pSize, Category, userId, version, out totalCount);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ArticleModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleModel(x)).ToArray();
        }
        /// <summary>
        /// 查询文章列表并包含优选文章
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<List<ArticleListModel>> SelectArticleListAndYouXuanList(int pageIndex, int pageSize)
        {
            return (await Articles.SelectArticleListAndYouXuanList(pageIndex, pageSize))?.ToList();
        }

        public static IEnumerable<ArticleModel> SelectAllArticle()
        {
            var dt = Articles.SelectAllArticle();
            if (dt == null || dt.Rows.Count == 0)
                return new ArticleModel[0];
            return dt.Rows.Cast<DataRow>().Select(row => new ArticleModel(row));
        }
        /// <summary>
        /// 根据关键字查询图文推送文章
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> SelectNewArticlesByWord(PagerModel pager, string keyWord, string version)
        {
            var dt = Articles.SelectNewArticlesByWord(pager, keyWord, version);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ArticleModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleModel(x)).ToArray();
        }

        /// <summary>
        /// 将图文推送搜索关键词-->添加
        /// </summary>
        /// <param name="KeyWord"></param>
        /// <param name="Versions"></param>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public static int AddSeekKeyWord(string KeyWord, string Versions, string Channel)
        {
            return Articles.AddSeekKeyWord(KeyWord, Versions, Channel);
        }

        /// <summary>
        /// 评论或赞过-->后添加文章ID和对应UserID
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PKID"></param>
        /// <param name="Type">1、我喜欢的  2、我评论的</param>
        /// <returns></returns>
        public static int AddUserReviewOfArticles(string UserId, int PKID, int Type, string Vote, out bool Status)
        {
            return Articles.AddUserReviewOfArticles(UserId, PKID, Type, Vote, out Status);
        }

        /// <summary>
        /// 查询消息通知
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isIncludeAll">是否查询所有的回复(评论、回答、说说)</param>
        /// <returns></returns>
        public static IEnumerable<ArticleCommentModel> SelectCommentNotice(string userId, bool isIncludeAll = false, PagerModel page = null)
        {
            var dt = Articles.SelectCommentNotice(userId, isIncludeAll, page);
            if (dt.Rows.Count == 0 && dt == null)
            {
                return new ArticleCommentModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleCommentModel(x)).ToArray();
        }

        /// <summary>
        /// 发送已读消息通知
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public static async Task<int> ReadCommentNotic(int commentId)
        {
            return await Articles.ReadCommentNotic(commentId);
        }


        /// <summary>
        /// 发送已读点赞消息通知
        /// </summary>
        /// <param name="id"></param>
        /// <param name="praiseType"></param>
        /// <returns></returns>
        public static async Task<int> ReadPraiseNotice(int id, int praiseType)
        {
            return await Articles.ReadPraiseNotice(id, praiseType);
        }

        /// <summary> 
        /// 查询点赞消息通知
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleCommentModel> SelectVoteNotice(string userId)
        {
            List<ArticleCommentModel> LsitPraose = new List<ArticleCommentModel>();
            var dt = Articles.SelectVoteNotice(userId);
            if (dt.Rows.Count == 0 && dt == null)
            {
                return null;
            }
            LsitPraose = dt.Rows.Cast<DataRow>().GroupBy(row => row["CommentId"]).Select(order => new ArticleCommentModel(order.First())
            {
                Item = order.Select(row => new CommentPraise(row))
            }).ToList();
            //for (int i = 0; i < LsitPraose.Count; i++)
            //{
            return LsitPraose as IEnumerable<ArticleCommentModel>;
            //}
            //LsitPraose= new ArticleCommentModel(dt.Rows[0])
            //{
            //    Item = dt.Rows.Cast<DataRow>().ToList()
            //        .Select(row => new CommentPraise(row)).ToArray()
            //};

            //if (dt.Rows.Count == 0 && dt == null)
            //{
            //    return new CommentPraise[0];
            //}
            //return dt.Rows.Cast<DataRow>().Select(x => new CommentPraise(x)).ToArray();
        }


        /// <summary> 
        /// 查询点赞消息通知
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleCommentModel> SelectVoteNotices(string userId, PagerModel page)
        {
            List<ArticleCommentModel> LsitPraose = new List<ArticleCommentModel>();
            var dt = Articles.SelectVoteNotices(userId, page);
            if (dt.Rows.Count == 0 && dt == null)
            {
                return null;
            }
            LsitPraose = dt.Rows.Cast<DataRow>().GroupBy(row => row["CommentId"]).Select(order => new ArticleCommentModel(order.First())
            {
                Item = order.Select(row => new CommentPraise(row))
            }).ToList();
            return LsitPraose as IEnumerable<ArticleCommentModel>;
        }

        /// <summary>
        /// 查询特推主题
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> SelectTopics()
        {
            var dt = Articles.SelectTopics();
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ArticleModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleModel(x)).ToArray();
        }

        /// <summary>
        /// 查询相关文章，以及置顶评论
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="Category"></param>
        /// <param name="UserId"></param>
        /// <param name="IsLikeNew"></param>
        /// <returns></returns>
        public static NewsInfoModel SelectNewsInfo(int PKID, int Category, string UserId = "", bool IsLikeNew = false)
        {
            try
            {
                var ds = Articles.SelectNewsInfo(PKID, Category);
                if (ds != null && ds.Tables.Count == 2)
                {
                    NewsInfoModel model = new NewsInfoModel();

                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        model.CommentItems = (List<tbl_CommentModel>)Helper.ModelConvertHelper<tbl_CommentModel>.ConvertToModel(ds.Tables[0]);
                        if (model.CommentItems != null && model.CommentItems.Count > 0)
                        {
                            model.CommentItems.ForEach(w => w.UserName = CommentHelper.GetCommentUserName(w.UserName, 0));
                        }
                    }
                    else
                        model.CommentItems = null;

                    if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        model.ArticleItems = (List<tbl_ArticleModel>)Helper.ModelConvertHelper<tbl_ArticleModel>.ConvertToModel(ds.Tables[1]);
                    else
                        model.ArticleItems = null;

                    return model;
                }
                return null;
            }
            catch (Exception ex) { return null; }
        }

        /// <summary>
        /// 查询当前用户是否收藏过指定文章
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="Category"></param>
        /// <param name="UserId"></param>
        /// <param name="IsLikeNew"></param>
        /// <returns></returns>
        public static bool IsNewsForLikeNews(int PKID, string UserId, bool IsLikeNew = true)
        {
            try
            {
                return Articles.IsNewsForLikeNews(PKID, UserId);
            }
            catch { return false; }
        }

        /// <summary>
        /// 查询出搜索热门的前12个关键词
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<HotWordMode> SelectHotKeyWord()
        {
            var dt = Articles.SelectHotKeyWord();
            if (dt == null || dt.Rows.Count == 0)
            {
                return new HotWordMode[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new HotWordMode(x)).ToArray();
        }

        /// <summary>
        /// 查询文章URL链接地址
        /// </summary>
        /// <returns></returns>
        public static string SelectNewsUrl(int pkid)
        {
            return Articles.SelectNewsUrl(pkid);
        }

        /// <summary>
        /// 新版图文推送=>查询所有的类别
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CategoryListModel> SelectAllNewCategory()
        {
            var dt = Articles.SelectAllNewCategory();
            if (dt == null || dt.Rows.Count == 0)
            {
                return new CategoryListModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new CategoryListModel(x)).ToArray();
        }

        /// <summary>
        /// 查询点赞状态
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static bool SelectVoteState(string UserId, int PKID)
        {
            var Status = Articles.SelectVoteState(UserId, PKID);
            return Status;
        }

        /// <summary>
        /// 查询点击数
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static int SelectClickCount(int PKID)
        {
            return Articles.SelectClickCount(PKID);
        }

        /// <summary>
        /// 查点赞数
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static int SelectVoteCount(int PKID)
        {
            var VoteCount = Articles.SelectVoteCount(PKID);
            if (string.IsNullOrWhiteSpace(VoteCount))
            {
                VoteCount = "0";
            }
            return Convert.ToInt32(VoteCount);
        }

        /// <summary>
        /// 阅读文章后插入记录
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Type"></param>
        /// <param name="Vote"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static int AddReadingRecord(string UserId, int Type, string Vote, int PKID)
        {
            if (Type == 3)
                return Articles.AddReadingRecord2(UserId, PKID, Type);
            else
                return Articles.AddReadingRecord(UserId, Type, Vote, PKID);
        }

        /// <summary>
        /// 首页汽车头条
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ArticleModel>> SelectCarMadeHeadlines()
        {
            var dt = await Articles.SelectCarMadeHeadlines();
            if (dt == null || dt.Rows.Count == 0)
            {
                return new List<ArticleModel>();
            }
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleModel(x)).ToList();
        }


        /// <summary>
        /// 首页汽车头条
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> SelectCarMadeHeadlinesVersion1()
        {
            var dt = Articles.SelectCarMadeHeadlinesVersion1();
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ArticleModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleModel(x)).ToArray();
        }

        /// <summary>
        /// 发现文章小红点
        /// </summary>
        /// <returns></returns>
        public static string SelectArticleIsNew()
        {
            return Articles.SelectArticleIsNew();
        }

        /// <summary>
        /// 查询文章详情以及相关文章
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="UserId"></param>
        /// <param name="IsLikeNew"></param>
        /// <returns></returns>
        public static ArticleInfosModel SelectArticlesAndRelates(int PKID, string UserId = "", bool IsLikeNew = false)
        {
            try
            {
                var ds = Articles.SelectArticleAndRelated(PKID, UserId, IsLikeNew);
                if (ds != null && ds.Tables.Count == 2)
                {
                    ArticleInfosModel model = new ArticleInfosModel();

                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        model.RelatedArticles = (List<tbl_ArticleModelNew>)Helper.ModelConvertHelper<tbl_ArticleModelNew>.ConvertToModel(ds.Tables[0]);
                    }
                    else
                        model.RelatedArticles = null;

                    if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        model.ArticleDetail = Helper.ModelConvertHelper<tbl_ArticleModelNew>.ConvertToModel(ds.Tables[1]).FirstOrDefault();
                    else
                        model.ArticleDetail = null;

                    return model;
                }
                return null;
            }
            catch (Exception ex) { return null; }
        }
        #endregion
        /// <summary>
        /// 获取所有作者信息
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CoverAuthorModel>> SelectCoverAuthor()
        {
            return await Articles.SelectCoverAuthor();
        }
        /// <summary>
        /// 通过名称获取作者信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<CoverAuthorModel> SelectCoverAuthorByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new CoverAuthorModel(name);
            var list = await CacheManager.GetCoverAuthorAll();

            return list?.FirstOrDefault(x => name.Equals(x.AuthorName, StringComparison.CurrentCultureIgnoreCase)) ?? new CoverAuthorModel(name);
        }
        /// <summary>
        /// 获取文章前10条评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<Tuple<List<Component.Discovery.BusinessData.Comment>, int>> SelectCommentsTop10(int articleId)
        {
            return await CacheManager.SelectCommentsTop10(articleId);
        }
        public static async Task<List<RelatedArticleModel>> SelectRelateArticleByCategoryTagId(int categoryTagId)
        {
            return await Articles.SelectRelateArticleByCategoryTagId(categoryTagId);
        }
        /// <summary>
        /// 获取文章所有喜欢用户
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<List<string>> SelectArticleLikeUserListByArticleId(int articleId)
        {
            return await Articles.SelectArticleLikeUserListByArticleId(articleId);
        }
        /// <summary>
        /// 获取用户是否喜欢该文章
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<bool> isLikeByArticleIdAndUserId(int articleId, Guid userId)
        {
            if (userId == Guid.Empty || articleId <= 0)
                return false;

            var likeList = await CacheManager.SelectArticleLikeUserListByArticleId(articleId);
            return likeList != null && likeList.Any(x => Guid.TryParse(x, out Guid uid) && uid == userId);
        }
        public static async Task<YouXuanRelatedOperactionModel> SelectYouXuanRelatedOperaction(Guid userId, int articleId)
        {
            return await Articles.SelectYouXuanRelatedOperaction(userId, articleId);
        }
        /// <summary>
        /// 优选点赞或收藏
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static async Task<bool> YouXuanVoteOrFavorite(YouXuanRelatedOperactionModel info)
        {
            //查询是否存在如果存在返回当前信息
            var userRelated = await Articles.SelectYouXuanRelatedOperaction(info.UserId, info.ArticleId);
            if (userRelated?.PKID > 0)
            {
                info.PKID = userRelated.PKID;
                //修改状态
                var result = await Articles.UpdateYouXuanRelatedOperactionStatus(info);
                return result > 0;
            }
            else
            {
                //插入状态
                var result = await Articles.InserYouXuanRelatedOperaction(info);
                return result > 0;
            }
        }
        /// <summary>
        /// 查询我喜欢的优选列表id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<List<int>> SelectMyFavoriteYouXuanIdsByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                return null;

            var ids = await Articles.SelectMyFavoriteYouXuanIdsByUserId(userId);
            if (ids == null || !ids.Any())
                return null;
            return ids.Select(x => x.ArticleId).ToList();

        }
        /// <summary>
        /// 查询我喜欢的优选文章列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<List<YouXuantListModel>> SelectMyFavoriteYouXuanListByUserId(Guid userId, int pageIndex, int pageSize)
        {
            var ids = await CacheManager.SelectMyFavoriteYouXuanIdsByUserId(userId);

            if (ids == null || !ids.Any())
                return null;

            var articleIds = ids.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            if (articleIds == null || !articleIds.Any())
                return null;

            var articleList = await Articles.SelectMyFavoriteYouXuanListByIds(articleIds.ToList());

            if (articleList == null || !articleList.Any())
                return null;

            return articleList;
        }
        /// <summary>
        /// 查询优选文章喜欢数
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<int> SelectYouXuanFavoriteCountByArticleId(int articleId) => await Articles.SelectYouXuanFavoriteCountByArticleId(articleId);
        /// <summary>
        /// 根据优选标签查询文章列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="categoryTagId"></param>
        /// <returns></returns>
        public static async Task<List<YouXuantModel>> SelectYouXuanArticleByTagId(int pageIndex, int pageSize, string categoryTagId) => await Articles.SelectYouXuanArticleByTagId(pageIndex, pageSize, categoryTagId);

        public static async Task<YouXuantModel> FirstYouXuanDetailById(int id) => await Articles.FirstYouXuanDetailById(id);
    }
}
