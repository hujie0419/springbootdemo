using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALChangeCategory
    {
        /// <summary>
        /// 获取所有有效类目
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectChangeCategoryList()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT  [PKID]
                                      ,[CategoryName]
                                      ,[ParentCategoryID]
                                      ,[CategoryLevel]
                                      ,[CategoryType]
                                      ,[SubTitle]
                                      ,[Sort]
                                      ,[IsShow]
                                      ,[ShowNum]
                                      ,[IsActive]
                                      ,[CreatedTime]
                                      ,[UpdatedTime]
                                  FROM [Configuration].[dbo].[tbl_GaiZhuangCategory] WITH ( NOLOCK )
                                where IsActive = 1
                                order by case when ParentCategoryID is null then PKID
								else ParentCategoryID end,CategoryLevel, Sort ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }
        /// <summary>
        /// 根据PKID获取类目信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static DataRow GetChangeCategoryModelByPKID(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT [PKID]
                                      ,[CategoryName]
                                      ,[ParentCategoryID]
                                      ,[CategoryLevel]
                                      ,[CategoryType]
                                      ,[SubTitle]
                                      ,[Sort]
                                      ,[IsShow]
                                      ,[ShowNum]
                                      ,[IsActive]
                                      ,[CreatedTime]
                                      ,[UpdatedTime]
                                  FROM [Configuration].[dbo].[tbl_GaiZhuangCategory] where IsActive = 1
                            and PKID = @PKID;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteDataRow(cmd);
                }
            }
        }
        /// <summary>
        /// 修改类目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateChangeCategoryModel(GaiZhuangCategoryModel model)
        {
            bool result = false;
            if (model != null && model.PKID != 0)
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"UPDATE [Configuration].[dbo].[tbl_GaiZhuangCategory] SET CategoryName=@CategoryName,SubTitle=@SubTitle,Sort=@Sort,ShowNum=@ShowNum,IsShow=@IsShow where PKID=@PKID ";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@CategoryName", model.CategoryName);
                        cmd.Parameters.AddWithValue("@Sort", model.Sort);
                        cmd.Parameters.AddWithValue("@SubTitle", model.SubTitle);
                        cmd.Parameters.AddWithValue("@ShowNum", model.ShowNum);
                        cmd.Parameters.AddWithValue("@IsShow", model.IsShow);
                        cmd.Parameters.AddWithValue("@PKID", model.PKID);
                        if (dbhelper.ExecuteNonQuery(cmd) > 0)
                            result = true;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 添加类目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertChangeCategoryModel(GaiZhuangCategoryModel model)
        {
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"  insert into [Configuration].[dbo].[tbl_GaiZhuangCategory](CategoryName,ParentCategoryID,CategoryLevel,CategoryType,SubTitle,Sort,IsShow,ShowNum,IsActive,CreatedTime,UpdatedTime)
  values(@CategoryName,@ParentCategoryID,@CategoryLevel,@CategoryType,@SubTitle,@Sort,@IsShow,@ShowNum,1,getdate(),getdate())";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@CategoryName", model.CategoryName);
                        cmd.Parameters.AddWithValue("@ParentCategoryID", model.ParentCategoryID);
                        cmd.Parameters.AddWithValue("@CategoryLevel", model.CategoryLevel);
                        cmd.Parameters.AddWithValue("@CategoryType", model.CategoryType);
                        cmd.Parameters.AddWithValue("@SubTitle", model.SubTitle);
                        cmd.Parameters.AddWithValue("@Sort", model.Sort);
                        cmd.Parameters.AddWithValue("@IsShow", model.IsShow);
                        cmd.Parameters.AddWithValue("@ShowNum", model.ShowNum);
                        return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /// <summary>
        /// 删除类目信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteChangeCategoryModelByPkid(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"update [Configuration].[dbo].[tbl_GaiZhuangCategory] set IsActive = 0 where PKID = @PKID OR ParentCategoryID=@PKID ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                }
            }
        }

        /// <summary>
        /// 删除已关联商品
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteRelateProductByPkid(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"delete from  [Configuration].[dbo].[tbl_GaiZhuangRelateProduct]  where PKID = @PKID ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                }
            }
        }

        /// <summary>
        /// 批量删除已关联商品
        /// </summary>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public static bool DeleteRelateProductByPkids(string pkids)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"delete from  [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] WHERE PKID IN (select Item AS PKID from Gungnir..SplitString(@PKIDS, ',', 1))";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PKIDS", pkids);
                    return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                }
            }
        }

        /// <summary>
        /// 获取所有有效广告类目
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAdvertCategoryList(string categoryid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT [PKID]
                                      ,[CategoryId]
                                      ,[AdvertType]
                                      ,[AdvertTypeName]
                                      ,[AdvertName]
                                      ,[ImageURL]
                                      ,[Sort]
                                      ,[AppLink]
                                      ,[H5Link]
                                      ,[PromotionRuleID]
                                      ,[IsShow]
                                      ,[PromotionName]
                                      ,[PromotionDescription] 
                                      ,[IsActive]
                                      ,[CreatedTime]
                                      ,[UpdatedTime]
                                  FROM [Configuration].[dbo].[tbl_GaiZhuangRelateAdvert] WITH ( NOLOCK )
                                where IsActive = 1 and CategoryId=@CategoryId ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CategoryId", categoryid);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 修改广告类目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateAdvertCategoryModel(AdvertCategoryModel model)
        {
            bool result = false;
            if (model != null && model.PKID != 0)
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"  UPDATE [Configuration].[dbo].[tbl_GaiZhuangRelateAdvert]
                                      SET   CategoryId = @CategoryId,
		                                    AdvertType = @AdvertType,
		                                    AdvertTypeName = @AdvertTypeName,
		                                    AdvertName = @AdvertName,
		                                    ImageURL = @ImageURL,
		                                    Sort = @Sort,
		                                    AppLink = @AppLink,
		                                    H5Link = @H5Link,
		                                    PromotionRuleID = @PromotionRuleID,
		                                    IsShow = @IsShow,
		                                    PromotionName = @PromotionName,
		                                    PromotionDescription = @PromotionDescription,
		                                    IsActive = @IsActive,
		                                    UpdatedTime = getdate()
		                                    WHERE PKID = @PKID ";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@CategoryID", model.CategoryId);
                        cmd.Parameters.AddWithValue("@AdvertType", model.AdvertType);
                        cmd.Parameters.AddWithValue("@AdvertTypeName", model.AdvertTypeName);
                        cmd.Parameters.AddWithValue("@AdvertName", model.AdvertName);
                        cmd.Parameters.AddWithValue("@ImageURL", model.ImageURL);
                        cmd.Parameters.AddWithValue("@Sort", model.Sort);
                        cmd.Parameters.AddWithValue("@AppLink", model.AppLink);
                        cmd.Parameters.AddWithValue("@H5Link", model.H5Link);
                        cmd.Parameters.AddWithValue("@PromotionRuleID", model.PromotionRuleID);
                        cmd.Parameters.AddWithValue("@IsShow", model.IsShow);
                        cmd.Parameters.AddWithValue("@PromotionName", model.PromotionName);
                        cmd.Parameters.AddWithValue("@PromotionDescription", model.PromotionDescription);
                        cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                        cmd.Parameters.AddWithValue("@PKID", model.PKID);
                        if (dbhelper.ExecuteNonQuery(cmd) > 0)
                            result = true;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 添加广告类目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertAdvertCategoryModel(AdvertCategoryModel model)
        {
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"    INSERT INTO [Configuration].[dbo].[tbl_GaiZhuangRelateAdvert](CategoryId,AdvertType,AdvertTypeName,AdvertName,ImageURL,Sort,AppLink,H5Link,PromotionRuleID,IsShow,PromotionName,PromotionDescription,IsActive,CreatedTime,UpdatedTime)
                        VALUES(@CategoryId,@AdvertType,@AdvertTypeName,@AdvertName,@ImageURL,@Sort,@AppLink,@H5Link,@PromotionRuleID,@IsShow,@PromotionName,@PromotionDescription,@IsActive,getdate(),getdate()) ";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                        cmd.Parameters.AddWithValue("@AdvertType", model.AdvertType);
                        cmd.Parameters.AddWithValue("@AdvertTypeName", model.AdvertTypeName);
                        cmd.Parameters.AddWithValue("@AdvertName", model.AdvertName);
                        cmd.Parameters.AddWithValue("@ImageURL", model.ImageURL);
                        cmd.Parameters.AddWithValue("@Sort", model.Sort);
                        cmd.Parameters.AddWithValue("@AppLink", model.AppLink);
                        cmd.Parameters.AddWithValue("@H5Link", model.H5Link);
                        cmd.Parameters.AddWithValue("@PromotionRuleID", model.PromotionRuleID);
                        cmd.Parameters.AddWithValue("@IsShow", model.IsShow);
                        cmd.Parameters.AddWithValue("@PromotionName", model.PromotionName);
                        cmd.Parameters.AddWithValue("@PromotionDescription", model.PromotionDescription);
                        cmd.Parameters.AddWithValue("@IsActive", 1);
                        return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// 删除广告信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteAdvertCategoryModelByPkid(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"delete from  [Configuration].[dbo].[tbl_GaiZhuangRelateAdvert]  where PKID = @PKID ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                }
            }
        }

        /// <summary>
        /// 根据PKID获取广告信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static DataRow GetAdvertCategoryModelByPKID(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT [PKID]
                                      ,[CategoryId]
                                      ,[AdvertType]
                                      ,[AdvertTypeName]
                                      ,[AdvertName]
                                      ,[ImageURL]
                                      ,[Sort]
                                      ,[AppLink]
                                      ,[H5Link]
                                      ,[PromotionRuleID]
                                      ,[IsShow]
                                      ,[PromotionName]
                                      ,[PromotionDescription]
                                      ,[IsActive]
                                      ,[CreatedTime]
                                      ,[UpdatedTime]
                                  FROM [Configuration].[dbo].[tbl_GaiZhuangRelateAdvert] WITH ( NOLOCK )
                                where  PKID = @PKID;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteDataRow(cmd);
                }
            }
        }

        /// <summary>
        /// 根据PKID获取文章信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static DataRow GetRelateArticleByPKID(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT [PKID]
                                      ,[CategoryId]
                                      ,[ArticleName]
                                      ,[ArticleLink]
                                      ,[IsActive]
                                      ,[CreatedTime]
                                      ,[UpdatedTime]
                                  FROM [Configuration].[dbo].[tbl_GaiZhuangRelateArticle] WITH ( NOLOCK )
                                where  PKID = @PKID;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteDataRow(cmd);
                }
            }
        }


        /// <summary>
        /// 获取子类目文章具体信息列表
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectArticleCategoryList(string categoryid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"select a.PKID,a.ArticleName,a.ArticleLink,b.Brand as VehicleID
                                    from [Configuration].dbo.[tbl_GaiZhuangRelateArticle] a with(nolock)
                                    left join ( 
	                                select ArticleID, Brand=stuff((select ';'+t.Brand+v.Vehicle+t.PaiLiang+t.Nian  from 
	                                Configuration.dbo.tbl_ArticleAdaptVehicle t with(nolock)
	                                left join Gungnir.dbo.tbl_Vehicle_Type v on t.VehicleID=v.ProductID COLLATE Chinese_PRC_CI_AS 
	                                where t.ArticleID=Configuration.dbo.tbl_ArticleAdaptVehicle.ArticleId for xml path('')), 1, 1, '') 
                                from Configuration.dbo.tbl_ArticleAdaptVehicle with(nolock)
                                group by ArticleID) b on a.PKID=b.ArticleId

                                where a.CategoryId = @CategoryId and a.IsActive = 1 and a.ArticleName is not null and a.ArticleLink is not null
                                ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CategoryId", categoryid);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 根据文章ID获取子类目文章具体信息列表
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectArticleCategoryByArticleId(int articleId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"select a.PKID,a.ArticleName,a.ArticleLink,b.Brand as VehicleID
                                    from [Configuration].dbo.[tbl_GaiZhuangRelateArticle] a with(nolock)
                                    left join ( 
	                                select ArticleID, Brand=stuff((select ';'+t.Brand+v.Vehicle+t.PaiLiang+t.Nian  from 
	                                Configuration.dbo.tbl_ArticleAdaptVehicle t with(nolock)
	                                left join Gungnir.dbo.tbl_Vehicle_Type v on t.VehicleID=v.ProductID COLLATE Chinese_PRC_CI_AS 
	                                where t.ArticleID=Configuration.dbo.tbl_ArticleAdaptVehicle.ArticleId for xml path('')), 1, 1, '') 
                                from Configuration.dbo.tbl_ArticleAdaptVehicle with(nolock)
                                group by ArticleID) b on a.PKID=b.ArticleId

                                where a.PKID = @PKID and a.IsActive = 1 and a.ArticleName is not null and a.ArticleLink is not null
                                ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PKID", articleId);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 删除文章信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteArticleCategoryModelByPkid(int pkid)
        {
            //var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            //if (SecurityHelp.IsBase64Formatted(conn))
            //{
            //    conn = SecurityHelp.DecryptAES(conn);
            //}
            //using (var dbhelper = new SqlDbHelper(conn))
            //{
            //    string sql = @"delete from [Gungnir].[dbo].[tbl_GaiZhuangRelateArticle] where PKID = @PKID ";
            //    using (var cmd = new SqlCommand(sql))
            //    {
            //        cmd.CommandType = CommandType.Text;
            //        cmd.Parameters.AddWithValue("@PKID", pkid);
            //        return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
            //    }
            //}

            using (TransactionScope scope = new TransactionScope())
            {
                var sql = @"delete from [Configuration].[dbo].[tbl_ArticleAdaptVehicle] where ArticleId = @PKID ";
                var sqlDelete = @"delete from [Configuration].[dbo].[tbl_GaiZhuangRelateArticle] where PKID = @PKID";

                var sqlParamForDel = new SqlParameter[]
                {
                    new SqlParameter("@PKID",pkid),
                };
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParamForDel);
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlDelete, sqlParamForDel);


                scope.Complete();
            }
            return true;

        }

        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertArticleCategoryModel(GaiZhuangRelateArticleModel model)
        {
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"      INSERT INTO [Configuration].[dbo].[tbl_GaiZhuangRelateArticle](CategoryId,ArticleName,ArticleLink,IsActive,CreatedTime,UpdatedTime)
  VALUES(@CategoryId,@ArticleName,@ArticleLink,1,getdate(),getdate()) ";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                        cmd.Parameters.AddWithValue("@ArticleName", model.ArticleName);
                        cmd.Parameters.AddWithValue("@ArticleLink", model.ArticleLink);
                        return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// 添加文章,返回主键值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertArticleCategory(GaiZhuangRelateArticleModel model)
        {
            string sql = @" INSERT INTO [Configuration].[dbo].[tbl_GaiZhuangRelateArticle](CategoryId,ArticleName,ArticleLink,IsActive,CreatedTime,UpdatedTime)
  VALUES(@CategoryId,@ArticleName,@ArticleLink,1,getdate(),getdate()) 
                SELECT @@IDENTITY";
            SqlParameter[] collection = new SqlParameter[] {
          new SqlParameter("@CategoryId", model.CategoryId),
          new SqlParameter("@ArticleLink",model.ArticleLink),
          new SqlParameter("@ArticleName",model.ArticleName),
            };
            object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, collection);
            if (obj != null)
                return Convert.ToInt32(obj);
            else
                return 0;

        }
        //批量插入文章匹配车型
        public static bool BulkSaveAriticleVehicle(DataTable tb)
        {
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
                {

                    bulk.BatchSize = tb.Rows.Count;
                    bulk.DestinationTableName = "tbl_ArticleAdaptVehicle";
                    bulk.WriteToServer(tb);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改文章
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateArticleCategoryModel(GaiZhuangRelateArticleModel model)
        {
            bool result = false;
            if (model != null && model.PKID != 0)
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"  UPDATE [Configuration].[dbo].[tbl_GaiZhuangRelateArticle]
                                        SET CategoryId = @CategoryId,
	                                        ArticleName = @ArticleName,
	                                        ArticleLink = @ArticleLink,
	                                        UpdatedTime = getdate()
	                                        WHERE PKID = @PKID ";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                        cmd.Parameters.AddWithValue("@ArticleName", model.ArticleName); ;
                        cmd.Parameters.AddWithValue("@ArticleLink", model.ArticleLink);
                        cmd.Parameters.AddWithValue("@PKID", model.PKID);
                        if (dbhelper.ExecuteNonQuery(cmd) > 0)
                            result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 删除文章匹配信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteArticleVehicleCategoryModel(List<ArticleAdaptVehicleModel> list)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (var model in list)
                {
                    var sqlDelete = @"delete from [Configuration].[dbo].[tbl_ArticleAdaptVehicle] where PKID = @PKID";

                    var sqlParamForDel = new SqlParameter[]
                    {
                    new SqlParameter("@PKID",model.PKID),
                    };

                    SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlDelete, sqlParamForDel);
                }

                scope.Complete();
            }


            return true;
        }

        /// <summary>
        /// 根据文章ID获取文章匹配车型
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectArticleVehicleCategoryList(int articleId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT [PKID]
                                      ,[ArticleId]
                                      ,[Brand]
                                      ,[VehicleID]
                                      ,[Nian]
                                      ,[PaiLiang]
                                      ,[CreatedTime]
                                      ,[UpdatedTime]
                                  FROM [Configuration].[dbo].[tbl_ArticleAdaptVehicle] WITH ( NOLOCK )
                                where ArticleId=@ArticleId ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ArticleId", articleId);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 根据RuleID获取优惠券信息
        /// </summary>
        /// <param name="getGuid"></param>
        /// <returns></returns>
        public static DataRow GetPromotionModelByRuleID(string getGuid)
        {
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"SELECT GCR.GetRuleGUID, GCR.RuleID,GCR.SingleQuantity,GCR.PromtionName,
                                GCR.Description,GCR.Discount,GCR.Minmoney,GCR.Term, GCR.Channel,GCR.ValiStartDate 
                                FROM Activity..tbl_GetCouponRules AS GCR WITH(NOLOCK)  WHERE GCR.GetRuleGUID = @GetRuleGUID ";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddWithValue("@GetRuleGUID", getGuid);
                        return dbhelper.ExecuteDataRow(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取关联商品
        /// </summary>
        /// <returns></returns>
        public static DataTable GetRelateProductList()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT TOP 1000 CN.DisplayName as ProductName,CP.ProductID +'|'+CP.VariantID as PID,CN2.DisplayName as ProductType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN2 WITH ( NOLOCK ) ON CP.ParentOID = CN2.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)
                                 ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);
        /// <summary>
        /// 获取关联商品
        /// </summary>
        /// <returns></returns>
        /// <param name="sqlStr"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,
                                                CN.DisplayName as ProductName,CP.ProductID +'|'+CP.VariantID as PID,CN.DisplayName as ProductType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)   " + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)

                             ";
            string sqlCount = @"SELECT COUNT(1)  FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据商品PID获取关联商品
        /// </summary>
        /// <returns></returns>
        /// <param name="pid"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductPID(string pid, int pageSize, int pageIndex, out int recordCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,
                                                CN.DisplayName as ProductName,CP.ProductID +'|'+CP.VariantID as PID,CN.DisplayName as ProductType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)  AND (CP.ProductID +'|'+CP.VariantID)=@PID 
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)

                             ";
            string sqlCount = @"SELECT COUNT(1)  FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4) AND (CP.ProductID +'|'+CP.VariantID)=@PID  ";
            var sqlParameter = new SqlParameter[]
            {
                 new SqlParameter("@PID",pid)
            };
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount, sqlParameter);

            var sqlParameters = new SqlParameter[]
            {
                 new SqlParameter("@PID",pid),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据商品名称关键词获取关联商品
        /// </summary>
        /// <returns></returns>
        /// <param name="productNameKey"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductKeyName(string productNameKey, int pageSize, int pageIndex, out int recordCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,
                                                CN.DisplayName as ProductName,CP.ProductID +'|'+CP.VariantID as PID,CN.DisplayName as ProductType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)   AND CN.DisplayName like N'%" + productNameKey.Replace("'", "''") + "%' ) AS PG WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1) AND     STR(@PageIndex * @PageSize)";
            string sqlCount = @"SELECT COUNT(1)  FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)  AND CN.DisplayName like N'%" + productNameKey.Replace("'", "''") + "%' ";
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ProductName",productNameKey),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据选中的商品类目
        /// </summary>
        /// <returns></returns>
        /// <param name="productItems"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductItems(string productItems, int pageSize, int pageIndex, out int recordCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,
                                                CN.DisplayName as ProductName,CP.ProductID +'|'+CP.VariantID as PID,CN.DisplayName as ProductType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)  AND CP.ParentOID in (" + productItems + @" )
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)

                             ";
            string sqlCount = @"SELECT COUNT(1)  FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4) AND CP.ParentOID in (" + productItems + @" ) ";

            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据子类目ID获取关联商品
        /// </summary>
        /// <returns></returns>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static List<GaiZhuangRelateProductModel> GetSelectedRelateProductListByCategoryID(int categoryId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"  SELECT [PKID]
                                  ,[CategoryId]
                                  ,[PID]
                                  ,[IsActive]
                                  ,[CreatedTime]
                                  ,[UpdatedTime]
                              FROM [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] WITH ( NOLOCK )
                                where  CategoryId = @CategoryID and PID is not null ;         
                             ";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@CategoryID",categoryId)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<GaiZhuangRelateProductModel>().ToList();

        }

        //批量插入子类目关联商品
        public static bool BulkSaveRelateProduct(DataTable tb)
        {
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
                {

                    bulk.BatchSize = tb.Rows.Count;
                    bulk.DestinationTableName = "tbl_GaiZhuangRelateProduct";
                    bulk.WriteToServer(tb);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取产品类目
        /// </summary>
        /// <returns></returns>
        public static List<CategoryModel> GetProductCategoryList()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT	CH.child_oid AS oid,
		                                CH.oid AS ParentOid,
		                                CH.CategoryName,
		                                C.DisplayName,
		                                C.Description,
		                                 NodeNo
                                FROM	Tuhu_productcatalog.[dbo].[CarPAR_CatalogHierarchy] AS CH WITH ( NOLOCK )
                                JOIN	Tuhu_productcatalog.[dbo].[CarPAR_zh-CN_Catalog] AS C WITH ( NOLOCK )
		                                ON C.#Catalog_Lang_Oid = CH.child_oid
                                WHERE	CH.CategoryName IS NOT NULL;
                                 ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<CategoryModel>().ToList();
                }
            }
        }

        /// <summary>
        /// 获取已选择的关联商品
        /// </summary>
        /// <returns></returns>
        /// <param name="sqlStr"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,GZ.PKID,GZ.CategoryId,
                                                CN.DisplayName as ProductName,CP.ProductID +'|'+CP.VariantID as PID,CN.DisplayName as ProductType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.ProductID +'|'+CP.VariantID) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)   " + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)

                             ";
            string sqlCount = @"SELECT COUNT(1)  FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.ProductID +'|'+CP.VariantID) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<SelectedRelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据商品PID获取已选择的关联商品
        /// </summary>
        /// <returns></returns>
        /// <param name="pid"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductListByProductPID(string pid, int pageSize, int pageIndex, out int recordCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,GZ.PKID,GZ.CategoryId,
                                                CN.DisplayName as ProductName,CP.ProductID +'|'+CP.VariantID as PID,CN.DisplayName as ProductType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.ProductID +'|'+CP.VariantID) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)  AND (CP.ProductID +'|'+CP.VariantID)=@PID AND GZ.PID = @PID
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)

                             ";
            string sqlCount = @"SELECT COUNT(1)  FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.ProductID +'|'+CP.VariantID) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4) AND (CP.ProductID +'|'+CP.VariantID)=@PID AND GZ.PID = @PID ";
            var sqlParameter = new SqlParameter[]
            {
                 new SqlParameter("@PID",pid)
            };
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount, sqlParameter);

            var sqlParameters = new SqlParameter[]
            {
                 new SqlParameter("@PID",pid),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<SelectedRelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据商品名称关键词获取已选择的关联商品
        /// </summary>
        /// <returns></returns>
        /// <param name="productNameKey"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductListByProductKeyName(string productNameKey, int pageSize, int pageIndex, out int recordCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,GZ.PKID,GZ.CategoryId,
                                                CN.DisplayName as ProductName,CP.ProductID +'|'+CP.VariantID as PID,CN.DisplayName as ProductType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.ProductID +'|'+CP.VariantID) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)   AND CN.DisplayName like N'%" + productNameKey.Replace("'", "''") + "%' ) AS PG WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1) AND     STR(@PageIndex * @PageSize)";
            string sqlCount = @"SELECT COUNT(1)  FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.ProductID +'|'+CP.VariantID) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)  AND CN.DisplayName like N'%" + productNameKey.Replace("'", "''") + "%' ";
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ProductName",productNameKey),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<SelectedRelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据选中的商品类目获取已选择的商品信息
        /// </summary>
        /// <returns></returns>
        /// <param name="productItems"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductListByProductItems(string productItems, int pageSize, int pageIndex, out int recordCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,GZ.PKID,GZ.CategoryId,
                                                CN.DisplayName as ProductName,CP.ProductID +'|'+CP.VariantID as PID,CN.DisplayName as ProductType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.ProductID +'|'+CP.VariantID) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4)  AND CP.ParentOID in (" + productItems + @" )
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)

                             ";
            string sqlCount = @"SELECT COUNT(1)  FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.ProductID +'|'+CP.VariantID) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
			                    WHERE CP.i_ClassType in (2,4) AND CP.ParentOID in (" + productItems + @" ) ";

            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<SelectedRelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据选中的类目获取已选择的商品信息
        /// </summary>
        /// <returns></returns>
        /// <param name="categoryId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductListByCategoryId(int categoryId, string[] brands)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (brands == null || !brands.Any())
            {


                string sql =
                    @"  SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,GZ.PKID,GZ.CategoryId,
                                                GZ.Sort,
                                                CN.DisplayName as ProductName,CP.Pid2 as PID,CP.PrimaryParentCategory as ProductType
                                                ,CN.CP_Brand as Brand,CP.OnSale,CRD.RegionId,CRD.Type as CityType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.Pid2) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
                                LEFT JOIN Configuration.[dbo].[ConmmonRegionDetail] AS [CRD] ON GZ.PKID = CRD.ForeignKey and CRD.Location=1
			                    WHERE  GZ.CategoryId = @CategoryId  ORDER BY CN.CP_Brand,CN.DisplayName
                             ";
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@CategoryId", categoryId)
                };
                return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<SelectedRelateProductModel>().ToList();
            }
            else
            {
                string sql =
     @"  SELECT    ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,GZ.PKID,GZ.CategoryId, GZ.Sort,
                                                CN.DisplayName as ProductName,CP.Pid2 as PID,CP.PrimaryParentCategory as ProductType
                                                ,CN.CP_Brand as Brand,CP.OnSale,CRD.RegionId,CRD.Type as CityType
                                FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK ) 
                                JOIN [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] AS GZ WITH ( NOLOCK ) ON (CP.Pid2) = GZ.PID COLLATE Chinese_PRC_CI_AS  
                                JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
                                JOIN  (SELECT	Item COLLATE Chinese_PRC_CI_AS AS Brand
		     FROM	Tuhu_productcatalog..SplitString(@Brands, ',', 1)) as bs on bs.Brand=CN.cp_brand
                                LEFT JOIN Configuration.[dbo].[ConmmonRegionDetail] AS [CRD] ON GZ.PKID = CRD.ForeignKey and CRD.Location=1
			                    WHERE  GZ.CategoryId = @CategoryId  ORDER BY CN.CP_Brand,CN.DisplayName
                             ";
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@CategoryId", categoryId),
                    new SqlParameter("@Brands",string.Join(",",brands))
                };
                return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<SelectedRelateProductModel>().ToList();
            }

        }

        /// <summary>
        /// 根据商品PID获取关联商品不分页
        /// </summary>
        /// <returns></returns>
        /// <param name="pid"></param>CP.PrimaryParentCategory,C.CP_Brand,CP.OnSale
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductPIDNoPage(string pid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"
SELECT  ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,
        CN.DisplayName AS ProductName ,
        CP.Pid2 AS PID ,
        CP.PrimaryParentCategory AS ProductType ,
        CN.CP_Brand ,
        CP.OnSale
FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK )
        JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
WHERE   CP.i_ClassType IN ( 2, 4 )
        AND CP.Pid2 = @PID
ORDER BY CN.CP_Brand ,
        CN.DisplayName;";

            var sqlParameters = new SqlParameter[]
            {
                 new SqlParameter("@PID",pid)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据商品名称关键词获取关联商品
        /// </summary>
        /// <returns></returns>
        /// <param name="productNameKey"></param>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductKeyNameNoPage(string productNameKey)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"
SELECT  ROW_NUMBER() OVER ( ORDER BY CN.#Catalog_Lang_Oid ) AS ROWNUMBER ,
        CN.DisplayName AS ProductName ,
        CP.Pid2 AS PID ,
        CP.PrimaryParentCategory AS ProductType ,
        CN.CP_Brand ,
        CP.OnSale
FROM    [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH ( NOLOCK )
        JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS CN WITH ( NOLOCK ) ON CP.oid = CN.#Catalog_Lang_Oid
WHERE   CP.i_ClassType IN ( 2, 4 )
        AND CN.DisplayName like N'%" + productNameKey.Replace("'", "''") + "%'  ORDER BY CN.CP_Brand,CN.DisplayName ";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ProductName",productNameKey)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RelateProductModel>().ToList();

        }

        /// <summary>
        /// 根据选中的商品类目
        /// </summary>
        /// <returns></returns>
        /// <param name="productItems"></param>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductItemsNoPage(string productItems)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"      SELECT  
            T.ProductID+'|'+T.VariantID as PID ,
           T.DisplayName AS ProductName,
		   T.PrimaryParentCategory AS ProductType,
		   T.CP_Brand AS Brand,
		   T.OnSale
    FROM    ( SELECT    CP.ProductID ,
						CP.VariantID,
                        C.DisplayName ,CP.ParentOID,CP.PrimaryParentCategory,C.CP_Brand,CP.OnSale
              FROM      [Tuhu_productcatalog].[dbo].CarPAR_CatalogProducts AS CP WITH ( NOLOCK )
                        JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS C WITH ( NOLOCK ) ON C.#Catalog_Lang_Oid = CP.oid
              WHERE     CP.i_ClassType IN (2, 4)
                        AND CP.oid  IN (
                        SELECT  CH1.child_oid
                        FROM    [Tuhu_productcatalog].[dbo].CarPAR_CatalogHierarchy AS CH1 WITH ( NOLOCK )
                                JOIN [Tuhu_productcatalog].[dbo].CarPAR_CatalogHierarchy AS CH2 WITH ( NOLOCK ) ON CH1.NodeNo  LIKE CH2.NodeNo
                                                              + '.%'
								JOIN		Gungnir..SplitString(@CategoryNames, ',', 1) AS SS
				                                                                ON CH2.CategoryName = SS.Item  COLLATE Chinese_PRC_CI_AS 
						 )
            ) AS T
			JOIN [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS ZH ON zh.#Catalog_Lang_Oid=t.ParentOID
			ORDER BY T.CP_Brand,T.DisplayName ";
            var sqlParameters = new SqlParameter[]
           {
                 new SqlParameter("@CategoryNames",productItems)
           };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RelateProductModel>().ToList();

        }

        public static int DelRegionid(SqlDbHelper dbhelper, string foreignKey)
        {
            using (var cmd = new SqlCommand(@"DELETE FROM  Configuration.[dbo].[ConmmonRegionDetail]  WHERE  ForeignKey IN (select Item AS PKID from Gungnir..SplitString(@ForeignKey, ',', 1))"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ForeignKey", foreignKey);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int InsertCityId(SqlDbHelper dbhelper, string foreignKey, int regionId, int type)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO  Configuration.[dbo].[ConmmonRegionDetail] ( [ForeignKey] ,[RegionId] ,[Type],[Location]) VALUES  (@ForeignKey,@RegionId,@Type,1)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ForeignKey", foreignKey);
                cmd.Parameters.AddWithValue("@RegionId", regionId);
                cmd.Parameters.AddWithValue("@Type", type);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }


        /// <summary>
        /// 根据PKID关联商品信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static DataRow EditRelateProduct(int pkid)
        {
            DataRow dataRow = null;

            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"SELECT [PKID]
                              ,[CategoryId]
                              ,[PID]
                              ,[Sort]
                          FROM [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] WITH (NOLOCK)
                         WHERE IsActive = 1 and PKID = @PKID";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddWithValue("@PKID", pkid);
                        dataRow = dbhelper.ExecuteDataRow(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }

            return dataRow;
        }


        /// <summary>
        /// 编辑关联商品排序字段
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateRelateProduct(int pkid, string sort)
        {
            bool result = false;
            if (pkid != 0)
            {
                try
                {
                    var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                    if (SecurityHelp.IsBase64Formatted(conn))
                    {
                        conn = SecurityHelp.DecryptAES(conn);
                    }
                    using (var dbhelper = new SqlDbHelper(conn))
                    {
                        string sql = @"UPDATE [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] WITH(ROWLOCK)
                                   SET [Sort] = @Sort
                                 WHERE  PKID = @PKID";
                        using (var cmd = new SqlCommand(sql))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Sort", sort);
                            cmd.Parameters.AddWithValue("@PKID", pkid);
                            if (dbhelper.ExecuteNonQuery(cmd) > 0)
                                result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }
            }

            return result;
        }

    }
}
