using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public interface IArticleManager
    {
        List<Article> SelectArticle(string KeyStr, string StartTime, string EndTime,string CategoryTag, int? PageSize, int? PageIndex, int? type = -1);
        List<Article> SelectBy(string KeyStr, int? PageSize, int? PageIndex, int? type = -1);
        int YesterdaySumCount();
        List<Article> SelectBy(int? PageSize, int? PageIndex);
        List<Article> SelectAll();
        void Delete(int PKID);
        int DeleteQuestion(int PKID);
        void Add(Article article, out string contentUrl, out int id, string locationAddress);
        void Update(Article article);
        Article GetByPKID(int PKID);
    }
}
