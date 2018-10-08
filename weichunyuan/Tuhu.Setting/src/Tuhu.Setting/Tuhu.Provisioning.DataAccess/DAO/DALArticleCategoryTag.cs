using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;
using Tuhu.Component.Common;
using Microsoft.ApplicationBlocks.Data;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALArticleCategoryTag
    {
        public static bool InsertBatch(SqlConnection sqlconnection, List<ArticleCategoryTagModel> models)
        {
            StringBuilder sbSql = new StringBuilder();
            List<SqlParameter> listParas = new List<SqlParameter>();
            string templateSql = "INSERT INTO [Marketing].[dbo].[HD_ArticleCategoryTag]([ArticleId],[CategoryTagId]) VALUES({0},{1})";

            for (int i = 0; i < models.Count(); i++)
            {
                var item = models[i];
                sbSql.AppendFormat(templateSql, item.ArticleId, item.CategoryTagId);
                listParas.Add(new SqlParameter("@ArticleId" + i, item.ArticleId));
                listParas.Add(new SqlParameter("@CategoryTagId" + i, item.CategoryTagId));
            }

            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sbSql.ToString(), listParas.ToArray()) > 0 ? true : false;
        }

        public static bool DeleteBatch(SqlConnection sqlconnection, int articleId)
        {
            string strSql = "DELETE [Marketing].[dbo].[HD_ArticleCategoryTag] WHERE ArticleId = @ArticleId";
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, strSql, new SqlParameter("@ArticleId", articleId)) > 0 ? true : false;
        }

        /// <summary>
        /// 文章标签关系表，先批量删除，然后批量插入
        /// </summary>
        public static bool UpdateBatch(SqlConnection sqlconnection, int articleId, List<ArticleCategoryTagModel> models)
        {
            string delSql = " DELETE [Marketing].[dbo].[HD_ArticleCategoryTag] WHERE ArticleId = @ArticleId_del ";
            string insertSql = " INSERT INTO [Marketing].[dbo].[HD_ArticleCategoryTag]([ArticleId],[CategoryTagId]) VALUES({0},{1}) ";

            StringBuilder sbSql = new StringBuilder();
            List<SqlParameter> listParas = new List<SqlParameter>();

            sbSql.AppendLine(delSql);
            listParas.Add(new SqlParameter("@ArticleId_del", articleId));

            for (int i = 0; i < models.Count(); i++)
            {
                var item = models[i];
                sbSql.AppendLine(string.Format(insertSql, "@ArticleId" + i, "@CategoryTagId" + i));
                listParas.Add(new SqlParameter("@ArticleId" + i, item.ArticleId));
                listParas.Add(new SqlParameter("@CategoryTagId" + i, item.CategoryTagId));
            }

            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sbSql.ToString(), listParas.ToArray()) > 0 ? true : false;
        }
    }
}
