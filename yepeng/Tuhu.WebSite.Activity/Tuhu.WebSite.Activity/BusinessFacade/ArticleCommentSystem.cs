using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Web.Activity.DataAccess;
using Tuhu.WebSite.Web.Activity.Models;

namespace Tuhu.WebSite.Web.Activity.BusinessFacade
{
    public static class ArticleCommentSystem
    {
        public static IEnumerable<ArticleCommentModel> SelectArticleCommentByPKID(int PKID, string UserID, int PageIndex = 1, int PageSize = 10)
        {
            var dt = ArticleComment.SelectArticleCommentByPKID(PKID, UserID, PageIndex, PageSize);
            return dt.Rows.Cast<DataRow>().Select(row => new ArticleCommentModel(row)).Select(a =>
            {
                a.PhoneNum = CommentHelper.GetCommentUserName(a.PhoneNum, 0);
                a.UserName = CommentHelper.GetCommentUserName(a.UserName, 0);
                return a;
            });
        }

        /// <summary>
        /// 最新评论
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="UserID"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleCommentModel> GetArticleCommentByPKID(int PKID, string UserID,out int TotalCount, int PageIndex = 1, int PageSize = 10)
        {
            var dt = ArticleComment.GetArticleCommentByPKID(PKID, UserID,out TotalCount, PageIndex, PageSize);
            if(dt==null || dt.Rows.Count == 0)
            {
                return new ArticleCommentModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(row => new ArticleCommentModel(row)).Select(a =>
            {
                a.PhoneNum = CommentHelper.GetCommentUserName(a.PhoneNum, 0);
                //a.UserName = CommentHelper.GetCommentUserName(a.RealName, 0);
                return a;
            });
        }

        public static string GetUserNameByID(int id)
        {
            return ArticleComment.GetUserNameByID(id);

        }
        public static int GetIsPraise(int commentId, string userId)
        {
            return ArticleComment.GetIsPraise(commentId, userId);
        }


        /// <summary>
        /// 热门评论
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleCommentModel> GetArticleCommentTop3(int PKID, string UserID)
        {
            var dt = ArticleComment.GetArticleCommentTop3(PKID, UserID);
            if(dt==null || dt.Rows.Count == 0)
            {
                return new ArticleCommentModel[0];
            }
            return dt.Rows.Cast<DataRow>().Select(row => new ArticleCommentModel(row)).Select(a =>
            {
                a.PhoneNum = CommentHelper.GetCommentUserName(a.PhoneNum, 0);
                //a.UserName = CommentHelper.GetCommentUserName(a.RealName, 0);
                return a;
            });
        }
        public static int InsertCommentPraise(CommentPraise model)
        {
            return ArticleComment.InsertCommentPraise(model);
        }

        /// <summary>
        /// 评论点赞取消点赞
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Status">返回值 true 点赞  false 取消</param>
        /// <returns></returns>
        public static int InsertCommentPraiseNew(CommentPraise model, out int Status,out int articleId)
        {
            return ArticleComment.InsertCommentPraiseNew(model,out Status,out articleId);
        }

        /// <summary>
        /// 评论总数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int CountComment(int id)
        {
            return ArticleComment.CountComment(id);
        }
        /// <summary>
        /// 点赞总数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int CountPraise(int id)
        {
            return ArticleComment.CountPraise(id);
        }

        public static int GetCountCommentBYId(int PKID, string UserID)
        {

            return ArticleComment.GetCountCommentBYId(PKID, UserID);
        }
    }
}