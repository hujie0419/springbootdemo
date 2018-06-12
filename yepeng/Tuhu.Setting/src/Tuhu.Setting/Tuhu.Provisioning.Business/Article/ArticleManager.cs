using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.ArticleManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class ArticleManager : IArticleManager
    {
        #region Private Fields

        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("Article");

        private ArticleHandler handler = null;

        #endregion

        public ArticleManager()
        {
            handler = new ArticleHandler(DbScopeManager);
        }

        public List<Article> SelectArticle(string KeyStr, string StartTime, string EndTime,string CategoryTag,int? PageSize, int? PageIndex, int? type)
        {
            try
            {
                return handler.SelectArticle(KeyStr, StartTime, EndTime, CategoryTag,PageSize, PageIndex, type);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectArticle");
                throw;
            }

        }
        public List<Article> SelectBy(string KeyStr, int? PageSize, int? PageIndex, int? type)
        {

            try
            {
                return handler.SelectBy(KeyStr, PageSize, PageIndex, type);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectBy");
                throw;
            }
        }

        public int YesterdaySumCount()
        {
            try
            {
                return handler.YesterdaySumCount();
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "YesterdaySumCount");
                throw;
            }

        }
        public List<Article> SelectBy(int? PageSize, int? PageIndex)
        {
            try
            {
                return handler.SelectBy(PageSize, PageIndex);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectBy");
                throw;
            }

        }

        public List<Article> SelectAll()
        {
            try
            {
                return handler.SelectAll();
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectAll");
                throw;
            }

        }
        public void Delete(int PKID)
        {
            try
            {
                handler.Delete(PKID);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Delete");
                throw;
            }

        }
        public void Add(Article article)
        {
            try
            {
                handler.Add(article);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Add");
                throw;
            }

        }
        public void Add(Article article, out string contentUrl, out int id, string locationAddress)
        {

            try
            {
                handler.Add(article, out contentUrl, out id, locationAddress);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "handler.Add(article, out contentUrl, out id, locationAddress)");
                throw;
            }
        }
        public void Update(Article article)
        {
            try
            {
                handler.Update(article);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Update");
                throw;
            }

        }
        public Article GetByPKID(int PKID)
        {
            try
            {
                return handler.GetByPKID(PKID);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetByPKID");
                throw;
            }
        }

        public List<Article> GetErrorImgUrlData(int pageSize)
        {
            try
            {
                return handler.GetErrorImgUrlData(pageSize);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetErrorImgUrlData");
                throw;
            }
        }

        public Article GetByUrl(String url)
        {
            try
            {
                return handler.GetByUrl(url);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetByUrl");
                throw;
            }
        }

        public Article GetArticleEntity(int id)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalArticle();
                return DalArticle.GetArticleEntity(connection, id);
            }
        }

        public List<SeekKeyWord> GetSeekKeyWord(int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return handler.GetSeekKeyWord(pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetSeekKeyWord");
                throw;
            }

        }

        public bool DeleteSeekKeyWord(string keyWord)
        {
            try
            {
                return handler.DeleteSeekKeyWord(keyWord);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleteSeekKeyWord");
                throw;
            }
        }

        public List<Article> GetArticle(string KeyStr, int? PageSize, int? PageIndex)
        {
            try
            {
                return handler.GetArticle(KeyStr, PageSize, PageIndex);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetArticle");
                throw;
            }

        }
        public bool UpdateHotArticles(int pkid, bool hotArticles)
        {
            try
            {
                return handler.UpdateHotArticles(pkid, hotArticles);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "UpdateHotArticles");
                throw;
            }


        }
        public bool UpdateTheme(int pkid, Article model)
        {
            try
            {
                return handler.UpdateTheme(pkid, model);

            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "UpdateTheme");
                throw;
            }

        }
        public List<Article> GetSmallTilte()
        {
            try
            {
                return handler.GetSmallTilte();
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetSmallTilte");
                throw;
            }

        }

        public string GetContentUrlById(int id)
        {
            try
            {
                return handler.GetContentUrlById(id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetContentUrlById");
                throw;
            }

        }
        public List<SeekHotWord> GetSeekKeyWord(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return handler.GetSeekKeyWord(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetSeekKeyWord");
                throw;
            }

        }

        public bool DeleleSeekKeyWordById(int id)
        {
            try
            {
                return handler.DeleleSeekKeyWordById(id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleleSeekKeyWordById");
                throw;
            }
        }
        public bool InsertSeekKeyWord(SeekHotWord keyWord)
        {
            try
            {
                return handler.InsertSeekKeyWord(keyWord);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "InsertSeekKeyWord");
                throw;
            }

        }
        public bool IsRepeatSeekKeyWord(string keyword)
        {
            try
            {
                return handler.IsRepeatSeekKeyWord(keyword);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "IsRepeatSeekKeyWord");
                throw;
            }
        }

        public bool IsRepeatSeekKeyWord(SeekHotWord keyword)
        {
            try
            {
                return handler.IsRepeatSeekKeyWord(keyword);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "IsRepeatSeekKeyWord");
                throw;
            }
        }
        public bool UpdateSeekKeyWord(SeekHotWord keyword, int id)
        {
            try
            {
                return handler.UpdateSeekKeyWord(keyword, id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "UpdateSeekKeyWord");
                throw;
            }

        }

        public SeekHotWord GetSeekHotWordById(int id)
        {
            try
            {
                return handler.GetSeekHotWordById(id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetSeekHotWordById");
                throw;
            }

        }

        //文章类目表(tbl_NewCategoryList)相关操作（增，删，改，查）
        public List<ArticleCategory> SelectCategory()
        {
            try
            {
                return handler.SelectCategory();
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectCategory");
                throw;
            }
        }
        public bool AddCategory(ArticleCategory categoryModel)
        {
            try
            {
                return handler.AddCategory(categoryModel);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "AddCategory");
                throw;
            }
        }
        public bool UpdateCategory(string categoryName, int id)
        {
            try
            {
                return handler.UpdateCategory(categoryName, id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "UpdateCategory");
                throw;
            }
        }
        public bool DeleteCategory(int id)
        {
            try
            {
                return handler.DeleteCategory(id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleteCategory");
                throw;
            }
        }
        /// <summary>
        /// 修改/添加
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public bool EditCategory(List<ArticleCategory> categoryModelList)
        {
            return handler.EditCategory(categoryModelList);
        }

        //表Marketing..tbl_ArticleNewList相关操作（增，删，改，查）
        public List<ArticleNewList> SelectArticleNewList(ArticleNewList modelValue)
        {
            try
            {
                return handler.SelectArticleNewList(modelValue);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectArticleNewList");
                throw;
            }
        }
        public bool AddArticleNewList(ArticleNewList articleModel)
        {
            try
            {
                return handler.AddArticleNewList(articleModel);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "AddArticleNewList");
                throw;
            }
        }
        public bool UpdateArticleNewList(ArticleNewList articleModel)
        {
            try
            {
                return handler.UpdateArticleNewList(articleModel);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "UpdateArticleNewList");
                throw;
            }
        }
        public bool UpdateArticleNewList(ArticleNewList modelWhere, ArticleNewList modelValue)
        {
            try
            {
                return handler.UpdateArticleNewList(modelWhere, modelValue);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "UpdateArticleNewList");
                throw;
            }
        }
        public bool DeleteArticleNewList(int Id)
        {
            try
            {
                return handler.DeleteArticleNewList(Id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleteArticleNewList");
                throw;
            }
        }

        /// <summary>
        /// 插入评论
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Insert(ArticleComment model)
        {
            try
            {
                return handler.Insert(model);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Insert(ArticleComment model)");
                return false;
            }
        }

        public int DeleteQuestion(int PKID)
        {
            try
            {
                return handler.DeleteQuestion(PKID);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleteQuestion");
                throw;
            }
        }
    }
}
