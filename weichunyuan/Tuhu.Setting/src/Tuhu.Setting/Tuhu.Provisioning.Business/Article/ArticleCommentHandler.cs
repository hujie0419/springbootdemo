using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ArticleManagement
{
    public class ArticleCommentHandler
    {
        #region Private Fields
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public ArticleCommentHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion

        public List<ArticleComment> SelectBy(int? PageSize, int? PageIndex, DateTime CommentTime, string Category, string Title, string CommentContent, string PhoneNum, int? AuditStatus, int Index)
        {
            Func<SqlConnection, List<ArticleComment>> action = (connection) => DalArticleComment.SelectBy(connection, PageSize, PageIndex, CommentTime, Category, Title, CommentContent, PhoneNum, AuditStatus, Index);
            return dbManager.Execute(action);
        }
        public List<ArticleComment> SelectAll()
        {
            Func<SqlConnection, List<ArticleComment>> action = (connection) => DalArticleComment.SelectAll(connection);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 评论置顶
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int UpdateSort(int ID)
        {
            Func<SqlConnection, int> action = (connection) => DalArticleComment.UpdateSort(connection, ID);
            return dbManager.Execute<int>(action);
        }

        /// <summary>
        /// 评论批量置顶
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int UpdateSortBatch(IEnumerable<int> IDs, int sort)
        {
            Func<SqlConnection, int> action = (connection) => DalArticleComment.UpdateSortBatch(connection, IDs, sort);
            return dbManager.Execute<int>(action);
        }

        public int Delete(int ID)
        {
            Func<SqlConnection, int> action = (connection) => DalArticleComment.Delete(connection, ID);
            return dbManager.Execute<int>(action);
        }
        public int DeleteBatch(IEnumerable<int> IDs)
        {
            Func<SqlConnection, int> action = (connection) => DalArticleComment.DeleteBatch(connection, IDs);
            return dbManager.Execute<int>(action);
        }
        public int Pass(int ID)
        {
            Func<SqlConnection, int> action = (connection) => DalArticleComment.Pass(connection, ID);
            return dbManager.Execute<int>(action);
        }
        public int PassBatch(IEnumerable<int> IDs)
        {
            Func<SqlConnection, int> action = (connection) => DalArticleComment.PassBatch(connection, IDs);
            return dbManager.Execute<int>(action);
        }
        public int UnPassBatch(IEnumerable<int> IDs)
        {
            Func<SqlConnection, int> action = (connection) => DalArticleComment.UnPassBatch(connection, IDs);
            return dbManager.Execute<int>(action);
        }
        //public void Add(ArticleComment article)
        //{
        //	Action<SqlConnection> action = (connection) => DalArticleComment.Add(connection, article);
        //	dbManager.Execute(action);
        //}
        //public void Update(ArticleComment article)
        //{
        //	Action<SqlConnection> action = (connection) => DalArticle.Update(connection, article);
        //	dbManager.Execute(action);
        //}
        public IEnumerable<ArticleComment> GetByPKID(int PKID, string Type)
        {
            Func<SqlConnection, IEnumerable<ArticleComment>> action = (connection) => DalArticleComment.GetByPKID(connection, PKID, Type);
            return dbManager.Execute(action);
        }
        public ArticleComment GetByID(int ID)
        {
            Func<SqlConnection, ArticleComment> action = (connection) => DalArticleComment.GetByID(connection, ID);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 修改文章表修改问题评论
        /// </summary>
        public bool UpdateArticleToComment(int pkid, int type, int isShow)
        {
            Func<SqlConnection,bool> action = (connection) => DalArticleComment.UpdateArticleToComment(connection, pkid, type, isShow);
            return dbManager.Execute(action);
        }

        public IEnumerable<ArticleComment> GetByIDs(IEnumerable<int> IDs)
        {
            Func<SqlConnection, IEnumerable<ArticleComment>> action = (connection) => DalArticleComment.GetByIDs(connection, IDs);
            return dbManager.Execute(action);
        }

        public bool UpdateShImgCommentCount(int PKID, string op)
        {
            return dbManager.Execute(conn => DalArticleComment.UpdateShImgCommentCount(conn, PKID, op));
        }
    }
}