using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.ArticleManagement;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class ArticleCommentManager : IArticleCommentManager
    {
        #region Private Fields

        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("ArticleComment");

        private ArticleCommentHandler handler = null;

        #endregion

        public ArticleCommentManager()
        {
            handler = new ArticleCommentHandler(DbScopeManager);
        }
        public List<ArticleComment> SelectBy(int? PageSize, int? PageIndex, DateTime CommentTime, string Category, string Title, string CommentContent, string PhoneNum, int? AuditStatus, int Index)
        {
            return handler.SelectBy(PageSize, PageIndex, CommentTime, Category, Title, CommentContent, PhoneNum, AuditStatus, Index);
        }

        /// <summary>
        /// 评论置顶
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool UpdateSort(int ID)
        {
            return handler.UpdateSort(ID) > 0;
        }

        /// <summary>
        /// 评论批量置顶
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool UpdateSortBatch(IEnumerable<int> IDs, int sort)
        {
            return handler.UpdateSortBatch(IDs, sort) > 0;
        }

        public bool Delete(int PKID)
        {
            return handler.Delete(PKID) > 0;
        }
        public bool DeleteBatch(IEnumerable<int> IDs)
        {
            return handler.DeleteBatch(IDs) > 0;
        }
        public bool Pass(int ID)
        {
            return handler.Pass(ID) > 0;
        }
        public bool PassBatch(IEnumerable<int> IDs)
        {
            return handler.PassBatch(IDs) > 0;
        }
        public bool UnPassBatch(IEnumerable<int> IDs)
        {
            return handler.UnPassBatch(IDs) > 0;
        }
        public List<ArticleComment> SelectAll()
        {
            return handler.SelectAll();
        }

        //public void Add(ArticleComment article)
        //{
        //	handler.Add(article);
        //}
        //public void Update(ArticleComment article)
        //{
        //	handler.Update(article);
        //}
        public IEnumerable<ArticleComment> GetByPKID(int PKID, string Type)
        {
            return handler.GetByPKID(PKID, Type);
        }

        public ArticleComment GetByID(int ID)
        {
            return handler.GetByID(ID);
        }

        /// <summary>
        /// 修改文章表修改问题评论
        /// </summary>
        public bool UpdateArticleToComment(int pkid, int type, int isShow)
        {
            try
            {
                return handler.UpdateArticleToComment(pkid, type, isShow);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        public IEnumerable<ArticleComment> GetByIDs(IEnumerable<int> IDs)
        {
            return handler.GetByIDs(IDs);
        }

        public bool UpdateShImgCommentCount(int PKID, string op)
        {
            return handler.UpdateShImgCommentCount(PKID,op);
        }

    }
}
