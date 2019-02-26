using System;
using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public interface IArticleCommentManager
    {
        List<ArticleComment> SelectBy(int? PageSize, int? PageIndex, DateTime CommentTime, string Category, string Title, string CommentContent, string PhoneNum, int? AuditStatus, int Index);
        List<ArticleComment> SelectAll();
        bool Delete(int ID);
        bool DeleteBatch(IEnumerable<int> IDs);
        bool Pass(int ID);
        bool PassBatch(IEnumerable<int> IDs);
        bool UnPassBatch(IEnumerable<int> IDs);
        //void Add(ArticleComment article);
        //void Update(ArticleComment article);
        IEnumerable<ArticleComment> GetByPKID(int PKID, string Type);
        bool UpdateSortBatch(IEnumerable<int> IDs, int sort);
        ArticleComment GetByID(int ID);
        bool UpdateArticleToComment(int pkid, int type, int isShow);
        IEnumerable<ArticleComment> GetByIDs(IEnumerable<int> IDs);
        bool UpdateShImgCommentCount(int PKID, string op);

    }
}