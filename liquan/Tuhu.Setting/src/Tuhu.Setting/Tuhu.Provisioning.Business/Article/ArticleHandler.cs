using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ArticleManagement
{
    public class ArticleHandler
    {
        private static readonly ILog logger = LoggerFactory.GetLogger("Article");

        #region Private Fields
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public ArticleHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion
        public List<Article> SelectBy(string KeyStr, int? PageSize, int? PageIndex, int? type)
        {
            Func<SqlConnection, List<Article>> action = (connection) => DalArticle.SelectBy(connection, KeyStr, PageSize, PageIndex, type);
            return dbManager.Execute(action);
        }

        public int YesterdaySumCount()
        {
            Func<SqlConnection, int> action = (connection) => DalArticle.YesterdaySumCount(connection);
            return dbManager.Execute(action);
        }
        public List<Article> SelectArticle(string KeyStr, string StartTime, string EndTime,string CategoryTag, int? PageSize, int? PageIndex, int? type)
        {
            Func<SqlConnection, List<Article>> action = (connection) => DalArticle.SelectArticle(connection, KeyStr, StartTime, EndTime, CategoryTag, PageSize, PageIndex, type);
            return dbManager.Execute(action);
        }

        public List<Article> SelectBy(int? PageSize, int? PageIndex)
        {
            Func<SqlConnection, List<Article>> action = (connection) => DalArticle.SelectBy(connection, PageSize, PageIndex);
            return dbManager.Execute(action);
        }
        public List<Article> SelectAll()
        {
            Func<SqlConnection, List<Article>> action = (connection) => DalArticle.SelectAll(connection);
            return dbManager.Execute(action);
        }
        public void Delete(int PKID)
        {
            Action<SqlConnection> action = (connection) => DalArticle.Delete(connection, PKID);
            dbManager.Execute(action);
        }
        public void Add(Article article)
        {
            try
            {
                Action<SqlConnection> action = (connection) => DalArticle.Add(connection, article);
                dbManager.Execute(action);
                logger.Log(Level.Info, "Add:" + JsonHelper.Serialize<Article>(article), null);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, "Add 错误:" + ex.ToString() + "参数:" + JsonHelper.Serialize<Article>(article), null);
            }
        }
        public void Add(Article article, out string contentUrl, out int id, string locationAddress)
        {
            string _contentUrl = "";
            int _id = 0;
            Action<SqlConnection> action = (connection) => DalArticle.Add(connection, article, out _contentUrl, out _id, locationAddress);
            dbManager.Execute(action);
            contentUrl = _contentUrl;
            id = _id;

        }
        public void Update(Article article)
        {
            Action<SqlConnection> action = (connection) => DalArticle.Update(connection, article);
            dbManager.Execute(action);
        }
        public Article GetByPKID(int PKID)
        {
            Func<SqlConnection, Article> action = (connection) => DalArticle.GetByPKID(connection, PKID);
            return dbManager.Execute(action);
        }

        public List<Article> GetErrorImgUrlData(int pageSize)
        {
            Func<SqlConnection, List<Article>> action = (connection) => DalArticle.GetErrorImgUrlData(connection, pageSize);
            return dbManager.Execute(action);
        }

        public List<SeekKeyWord> GetSeekKeyWord(int pageSize, int pageIndex, out int recordCount)
        {
            string strConn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            strConn = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;

            SqlConnection conn = new SqlConnection(strConn);
            return DalArticle.GetSeekKeyWord(conn, pageSize, pageIndex, out recordCount);
        }

        public bool DeleteSeekKeyWord(string keyWord)
        {
            return dbManager.Execute(conn => (DalArticle.DeleteSeekKeyWord(conn, keyWord)));

        }

        public List<Article> GetArticle(string KeyStr, int? PageSize, int? PageIndex)
        {
            Func<SqlConnection, List<Article>> action = (connection) => DalArticle.GetArticle(connection, KeyStr, PageSize, PageIndex);
            return dbManager.Execute(action);
        }
        public bool UpdateHotArticles(int pkid, bool hotArticles)
        {
            return dbManager.Execute(conn => DalArticle.UpdateHotArticles(conn, pkid, hotArticles));

        }
        public bool UpdateTheme(int pkid, Article modelValue)
        {
            return dbManager.Execute(conn => DalArticle.UpdateTheme(conn, pkid, modelValue));

        }
        public List<Article> GetSmallTilte()
        {
            return dbManager.Execute(conn => DalArticle.GetSmallTilte(conn));
        }
        public string GetContentUrlById(int id)
        {
            return dbManager.Execute(conn => DalArticle.GetContentUrlById(conn, id));
        }


        public List<SeekHotWord> GetSeekKeyWord(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string strConn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            strConn = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
            SqlConnection conn = new SqlConnection(strConn);
            return DalArticle.GetSeekKeyWord(conn, sqlStr, pageSize, pageIndex, out recordCount);

        }

        public bool DeleleSeekKeyWordById(int id)
        {
            return dbManager.Execute(conn => DalArticle.DeleleSeekKeyWordById(conn, id));

        }
        public bool InsertSeekKeyWord(SeekHotWord keyWord)
        {
            return dbManager.Execute(conn => DalArticle.InsertSeekKeyWord(conn, keyWord));
        }
        public bool IsRepeatSeekKeyWord(string keyword)
        {
            return dbManager.Execute(conn => DalArticle.IsRepeatSeekKeyWord(conn, keyword));
        }
        public bool IsRepeatSeekKeyWord(SeekHotWord keyword)
        {
            return dbManager.Execute(conn => DalArticle.IsRepeatSeekKeyWord(conn, keyword));
        }
        public bool UpdateSeekKeyWord(SeekHotWord keyword, int id)
        {
            return dbManager.Execute(conn => DalArticle.UpdateSeekKeyWord(conn, keyword, id));

        }
        public SeekHotWord GetSeekHotWordById(int id)
        {

            return dbManager.Execute(conn => DalArticle.GetSeekHotWordById(conn, id));
        }

        //文章类目表(tbl_NewCategoryList)相关操作（增，删，改，查）
        public List<ArticleCategory> SelectCategory()
        {
            return dbManager.Execute(conn => DalArticle.SelectCategory(conn));
        }
        public bool AddCategory(ArticleCategory categoryModel)
        {
            return dbManager.Execute(conn => DalArticle.AddCategory(conn, categoryModel));
        }
        public bool UpdateCategory(string categoryName, int id)
        {
            return dbManager.Execute(conn => DalArticle.UpdateCategory(conn, categoryName, id));
        }
        public bool DeleteCategory(int id)
        {
            return dbManager.Execute(conn => DalArticle.DeleteCategory(conn, id));
        }
        public bool EditCategory(List<ArticleCategory> categoryModelList)
        {
            StringBuilder strSql = new StringBuilder();
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            for (int i = 0; i < categoryModelList.Count; i++)
            {
                var data = categoryModelList[i];
                //判断ID是否为空（为空则Add 反之则Update）
                if (!string.IsNullOrEmpty(data.id.ToString()) && data.id > 0)
                {
                    string sqlSet = string.Format(" CategoryName = @CategoryName_{0},Sort = @Sort_{0},Color = @Color_{0} ", i);
                    string sqlWhere = string.Format(" id = @id_{0} ", i);
                    strSql.AppendFormat(" UPDATE [Marketing].[dbo].[tbl_NewCategoryList] WITH(rowlock) SET {0} WHERE {1} ", sqlSet, sqlWhere);

                    dicParams.Add("@CategoryName_" + i, data.CategoryName);
                    dicParams.Add("@Sort_" + i, data.Sort);
                    dicParams.Add("@id_" + i, data.id);
                    dicParams.Add("@Color_" + i, data.Color);
                }
                else
                {
                    string sqlSet = string.Format("CategoryName,Sort,Color");
                    string sqlWhere = string.Format("@CategoryName_{0},@Sort_{0},@Color_{0} ", i);
                    strSql.AppendFormat(" INSERT into [Marketing].[dbo].[tbl_NewCategoryList]({0}) VALUES({1}) ", sqlSet, sqlWhere);

                    dicParams.Add("@CategoryName_" + i, data.CategoryName);
                    dicParams.Add("@Sort_" + i, data.Sort);
                    dicParams.Add("@Color_" + i, data.Color);
                }
            }

            SqlParameter[] sqlParams = new SqlParameter[dicParams.Count];//参数值
            //修改
            for (int i = 0; i < dicParams.Count; i++)
            {
                var dicKey = dicParams.ElementAt(i).Key;
                var dicValue = dicParams.ElementAt(i).Value;
                sqlParams[i] = new SqlParameter(dicKey, dicValue);
            }
            Func<SqlConnection, bool> action = (connection) => DalArticle.EditCategory(connection, strSql.ToString(), sqlParams);
            return dbManager.Execute(action);
        }

        //表Marketing..tbl_ArticleNewList相关操作（增，删，改，查）
        public List<ArticleNewList> SelectArticleNewList(ArticleNewList modelValue)
        {
            string strSql = @"SELECT * FROM [Marketing].[dbo].[tbl_ArticleNewList] with (nolock) Where 1=1 {0}";

            StringBuilder strSqlWhere = new StringBuilder();
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            if (modelValue.Id > 0)
            {
                dicParams.Add("Id", modelValue.Id);
            }
            if (modelValue.ArticleId > 0)
            {
                dicParams.Add("ArticleId", modelValue.ArticleId);
            }
            if (!string.IsNullOrEmpty(modelValue.ArticleUrl))
            {
                dicParams.Add("ArticleUrl", modelValue.ArticleUrl);
            }
            if (!string.IsNullOrEmpty(modelValue.ProductId))
            {
                dicParams.Add("ProductId", modelValue.ProductId);
            }
            if (modelValue.Type > 0)
            {
                dicParams.Add("Type", modelValue.Type);
            }
            if (DateTime.Compare(modelValue.CreateTime, DateTime.MinValue) > 0)
            {
                dicParams.Add("CreateTime", modelValue.CreateTime);
            }
            if (!string.IsNullOrEmpty(modelValue.Field_1))
            {
                dicParams.Add("Field_1", modelValue.Field_1);
            }
            if (!string.IsNullOrEmpty(modelValue.Field_2))
            {
                dicParams.Add("Field_2", modelValue.Field_2);
            }
            if (!string.IsNullOrEmpty(modelValue.Field_3))
            {
                dicParams.Add("Field_3", modelValue.Field_3);
            }
            if (!string.IsNullOrEmpty(modelValue.Field_4))
            {
                dicParams.Add("Field_4", modelValue.Field_4);
            }
            SqlParameter[] sqlParams = new SqlParameter[dicParams.Count];
            for (int i = 0; i < dicParams.Count; i++)
            {
                var dicKey = dicParams.ElementAt(i).Key;
                var dicValue = dicParams.ElementAt(i).Value;
                sqlParams[i] = new SqlParameter("@" + dicKey, dicValue);
                strSqlWhere.AppendFormat(" AND {0} = @{0} ", dicKey);
            }

            strSql = string.Format(strSql, strSqlWhere.ToString());
            Func<SqlConnection, List<ArticleNewList>> action = (connection) => DalArticle.SelectArticleNewList(connection, strSql.ToString(), sqlParams);
            return dbManager.Execute(action);
        }

        public bool AddArticleNewList(ArticleNewList articleModel)
        {
            return dbManager.Execute(conn => DalArticle.AddArticleNewList(conn, articleModel));
        }

        /// <summary>
        /// 修改所有字段
        /// </summary>
        /// <param name="articleModel"></param>
        /// <returns></returns>
        public bool UpdateArticleNewList(ArticleNewList articleModel)
        {
            return dbManager.Execute(conn => DalArticle.UpdateArticleNewList(conn, articleModel));
        }

        /// <summary>
        /// 修改局部字段
        /// </summary>
        /// <param name="modelWhere">条件</param>
        /// <param name="modelValue">字段</param>
        /// <returns></returns>
        public bool UpdateArticleNewList(ArticleNewList modelWhere, ArticleNewList modelValue)
        {
            if (modelWhere != null && modelValue != null)
            {
                string strSql = @"update [Marketing].[dbo].[tbl_ArticleNewList]  WITH(rowlock) set {0} WHERE 1=1 {1}";

                StringBuilder strSqlWhere = new StringBuilder();
                Dictionary<string, object> dicParamsWhere = new Dictionary<string, object>();
                Dictionary<string, object> dicParamsValue = new Dictionary<string, object>();

                #region 更新字段
                if (modelValue.ArticleId > 0)
                {
                    dicParamsValue.Add("ArticleId", modelValue.ArticleId);
                }
                if (!string.IsNullOrEmpty(modelValue.ArticleUrl))
                {
                    dicParamsValue.Add("ArticleUrl", modelValue.ArticleUrl);
                }
                if (!string.IsNullOrEmpty(modelValue.ProductId))
                {
                    dicParamsValue.Add("ProductId", modelValue.ProductId);
                }
                if (modelValue.Type > 0)
                {
                    dicParamsValue.Add("Type", modelValue.Type);
                }
                if (DateTime.Compare(modelValue.CreateTime, DateTime.MinValue) > 0)
                {
                    dicParamsValue.Add("CreateTime", modelValue.CreateTime);
                }
                if (!string.IsNullOrEmpty(modelValue.Field_1))
                {
                    dicParamsValue.Add("Field_1", modelValue.Field_1);
                }
                if (!string.IsNullOrEmpty(modelValue.Field_2))
                {
                    dicParamsValue.Add("Field_2", modelValue.Field_2);
                }
                if (!string.IsNullOrEmpty(modelValue.Field_3))
                {
                    dicParamsValue.Add("Field_3", modelValue.Field_3);
                }
                if (!string.IsNullOrEmpty(modelValue.Field_4))
                {
                    dicParamsValue.Add("Field_4", modelValue.Field_4);
                }
                #endregion

                #region 更新条件
                if (modelWhere.Id > 0)
                {
                    dicParamsWhere.Add("Id", modelWhere.Id);
                }
                if (modelWhere.ArticleId > 0)
                {
                    dicParamsWhere.Add("ArticleId", modelWhere.ArticleId);
                }
                if (!string.IsNullOrEmpty(modelWhere.ArticleUrl))
                {
                    dicParamsWhere.Add("ArticleUrl", modelWhere.ArticleUrl);
                }
                if (!string.IsNullOrEmpty(modelValue.ProductId))
                {
                    dicParamsWhere.Add("ProductId", modelWhere.ProductId);
                }
                if (modelWhere.Type > 0)
                {
                    dicParamsWhere.Add("Type", modelWhere.Type);
                }
                if (DateTime.Compare(modelWhere.CreateTime, DateTime.MinValue) > 0)
                {
                    dicParamsWhere.Add("CreateTime", modelWhere.CreateTime);
                }
                if (!string.IsNullOrEmpty(modelWhere.Field_1))
                {
                    dicParamsWhere.Add("Field_1", modelWhere.Field_1);
                }
                if (!string.IsNullOrEmpty(modelWhere.Field_2))
                {
                    dicParamsWhere.Add("Field_2", modelWhere.Field_2);
                }
                if (!string.IsNullOrEmpty(modelWhere.Field_3))
                {
                    dicParamsWhere.Add("Field_3", modelWhere.Field_3);
                }
                if (!string.IsNullOrEmpty(modelWhere.Field_4))
                {
                    dicParamsWhere.Add("Field_4", modelWhere.Field_4);
                }
                #endregion

                StringBuilder strSqlField = new StringBuilder(); //字段
                SqlParameter[] sqlParams = new SqlParameter[dicParamsWhere.Count + dicParamsValue.Count];   //参数值

                for (int i = 0; i < dicParamsWhere.Count; i++)
                {
                    var dicKey = dicParamsWhere.ElementAt(i).Key;
                    var dicValue = dicParamsWhere.ElementAt(i).Value;
                    sqlParams[i] = new SqlParameter("@Where" + dicKey, dicValue);
                    strSqlWhere.AppendFormat(" AND {0} = @Where{0} ", dicKey);
                }

                for (int i = 0; i < dicParamsValue.Count; i++)
                {
                    var dicKey = dicParamsValue.ElementAt(i).Key;
                    var dicValue = dicParamsValue.ElementAt(i).Value;
                    sqlParams[dicParamsWhere.Count + i] = new SqlParameter("@Value" + dicKey, dicValue);
                    strSqlField.AppendFormat("{0}=@Value{0},", dicKey);
                }

                strSql = string.Format(strSql, strSqlField.ToString().TrimEnd(','), strSqlWhere.ToString());

                if (strSql.ToLower().IndexOf("and") > 0)
                {
                    Func<SqlConnection, bool> action = (connection) => DalArticle.UpdateArticleNewList(connection, strSql.ToString(), sqlParams);
                    return dbManager.Execute(action);
                }
            }
            return false;
        }

        /// <summary>
        /// 插入评论
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Insert(ArticleComment model)
        {
            return dbManager.Execute(conn => DalArticleComment.Insert(conn, model));
        }

        public bool DeleteArticleNewList(int Id)
        {
            return dbManager.Execute(conn => DalArticle.DeleteArticleNewList(conn, Id));
        }

        public int DeleteQuestion(int PKID)
        {
            return dbManager.Execute(conn => DalArticle.DeleteQuestion(conn, PKID));
        }

        public bool UpdateShImgCommentCount(int PKID, string op)
        {
            return dbManager.Execute(conn => DalArticleComment.UpdateShImgCommentCount(conn, PKID,op));
        }

        public Article GetByUrl(string url)
        {
            Func<SqlConnection, Article> action = (connection) => DalArticle.GetByUrl(connection, url);
            return dbManager.Execute(action);

        }
    }
}
