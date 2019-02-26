using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Common;
using System.Configuration;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
   public static class DALSE_BrandFlagship
    {
        public static DataTable SelectArticle(string id,SqlConnection conn)
        {
            string sql = "SELECT top 1  * FROM Marketing.dbo.tbl_Article WITH(NOLOCK) WHERE PKID LIKE '" + id+"%' ORDER BY PKID ASC ";
            using (var cmd = new SqlCommand(sql, conn))
            {
                return DbHelper.ExecuteDataTable(cmd);
            }
        }



        public static int Add(SE_BrandFlagship model)
        {
            int result = 0;
            var db = DbHelper.CreateDefaultDbHelper();
            db.BeginTransaction();
            try
            {
                string sql = @"INSERT INTO Configuration.dbo.SE_BrandFlagshipConfig
        ( Name ,
          ImageUrl ,
          ActivityHome ,
          Description ,
          ArticleID ,
          ArticleTitle ,
          DecorativePattern,
          LogoUrl,Brand,CreateDT,UpdateDT,ShareParameter
        )
VALUES  ( @Name , -- Name - nvarchar(200)
          @ImageUrl , -- ImageUrl - nvarchar(500)
          @ActivityHome , -- ActivityHome - nvarchar(200)
         @Description , -- Description - text
          @ArticleID , -- ArticleID - int
         @ArticleTitle , -- ArticleTitle - nvarchar(200)
         @DecorativePattern,  -- DecorativePattern - nvarchar(100)
        @LogoUrl,@Brand,GETDATE(),GETDATE(),@ShareParameter
        )     SELECT   @RowID=@@IDENTITY";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Name",model.Name);
                cmd.Parameters.AddWithValue("@ImageUrl",model.ImageUrl);
                cmd.Parameters.AddWithValue("@ActivityHome",model.ActivityHome);
                cmd.Parameters.AddWithValue("@Description",model.Description);
                cmd.Parameters.AddWithValue("@ArticleID",model.ArticleID);
                cmd.Parameters.AddWithValue("@ArticleTitle",model.ArticleTitle);
                cmd.Parameters.AddWithValue("@DecorativePattern",model.DecorativePattern);
                cmd.Parameters.AddWithValue("@LogoUrl",model.LogoUrl);
                cmd.Parameters.AddWithValue("@Brand",model.Brand);
                cmd.Parameters.AddWithValue("@ShareParameter",model.ShareParameter);
                cmd.Parameters.Add(new SqlParameter() {
                    ParameterName = "@RowID",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.Int
                });
                db.ExecuteNonQuery(cmd);
                model.ID = Convert.ToInt32(cmd.Parameters["@RowID"].Value);

                string sqlDeatil = @"INSERT INTO Configuration.dbo.SE_BrandFlagshipDetailConfig
        ( FK_BrandFlagship ,
          ArticleID ,
          ArticleTitle ,
          ArticleType,
            [Description],
            OrderBy
        )
VALUES  ( @FK_BrandFlagship , -- FK_BrandFlagship - int
          @ArticleID , -- ArticleID - int
          @ArticleTitle , -- ArticleTitle - nvarchar(200)
          @ArticleType,  -- ArticleType - int
            @Description,
          @OrderBy
        )";
                foreach (var item in model.Information)
                {
                   SqlCommand  cmdItem = new SqlCommand();
                    cmdItem.CommandText = sqlDeatil;
                    cmdItem.CommandType = CommandType.Text;
                    cmdItem.Parameters.AddWithValue("@FK_BrandFlagship", model.ID);
                    cmdItem.Parameters.AddWithValue("@ArticleID",item.ArticleID);
                    cmdItem.Parameters.AddWithValue("@ArticleTitle",item.ArticleTitle);
                    cmdItem.Parameters.AddWithValue("@ArticleType",item.ArticleType);
                    cmdItem.Parameters.AddWithValue("@Description",item.Description);
                    cmdItem.Parameters.AddWithValue("@OrderBy",item.OrderBy);
                    db.ExecuteNonQuery(cmdItem);
                }
                foreach (var item in model.Testing)
                {
                   SqlCommand cmdItem = new SqlCommand();
                    cmdItem.CommandText = sqlDeatil;
                    cmdItem.CommandType = CommandType.Text;
                    cmdItem.Parameters.AddWithValue("@FK_BrandFlagship", model.ID);
                    cmdItem.Parameters.AddWithValue("@ArticleID", item.ArticleID);
                    cmdItem.Parameters.AddWithValue("@ArticleTitle", item.ArticleTitle);
                    cmdItem.Parameters.AddWithValue("@ArticleType", item.ArticleType);
                    cmdItem.Parameters.AddWithValue("@Description", item.Description);
                    cmdItem.Parameters.AddWithValue("@OrderBy",item.OrderBy);
                    db.ExecuteNonQuery(cmdItem);
                }

                db.Commit();
                result = 1;
            }
            catch (Exception e)
            {
                db.Rollback();
            }
            finally
            {
                db.Dispose();
            }
            return result;

        }

        public static int Update(SE_BrandFlagship model)
        {
            int result = 0;
            var db = DbHelper.CreateDefaultDbHelper();
            try
            {
                db.BeginTransaction();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update Configuration.dbo.SE_BrandFlagshipConfig set ");
                strSql.Append("Name=@Name,");
                strSql.Append("ImageUrl=@ImageUrl,");
                strSql.Append("ActivityHome=@ActivityHome,");
                strSql.Append("[Description]=@Description,");
                strSql.Append("ArticleID=@ArticleID,");
                strSql.Append("ArticleTitle=@ArticleTitle,");
                strSql.Append("DecorativePattern=@DecorativePattern,");
                strSql.Append("LogoUrl=@LogoUrl,");
                strSql.Append("Brand=@Brand,UpdateDT=GETDATE(),ShareParameter=@ShareParameter");
                strSql.Append(" where ID=@ID");
               
                SqlCommand cmd = new SqlCommand(strSql.ToString());
                cmd.Parameters.AddWithValue("@Name",model.Name);
                cmd.Parameters.AddWithValue("@ImageUrl",model.ImageUrl);
                cmd.Parameters.AddWithValue("@ActivityHome", model.ActivityHome);
                cmd.Parameters.AddWithValue("@Description",model.Description);
                cmd.Parameters.AddWithValue("@ArticleID",model.ArticleID);
                cmd.Parameters.AddWithValue("@ArticleTitle",model.ArticleTitle);
                cmd.Parameters.AddWithValue("@DecorativePattern",model.DecorativePattern);
                cmd.Parameters.AddWithValue("@LogoUrl",model.LogoUrl);
                cmd.Parameters.AddWithValue("@Brand",model.Brand);
                cmd.Parameters.AddWithValue("@ShareParameter",model.ShareParameter);
                cmd.Parameters.AddWithValue("@ID",model.ID);
                db.ExecuteNonQuery(cmd);


                string delStr = @"DELETE FROM Configuration.dbo.SE_BrandFlagshipDetailConfig WHERE FK_BrandFlagship=@FK_BrandFlagship";
                cmd = new SqlCommand(delStr);
                cmd.Parameters.AddWithValue("@FK_BrandFlagship",model.ID);
                db.ExecuteNonQuery(cmd);

                string sqlDeatil = @"INSERT INTO Configuration.dbo.SE_BrandFlagshipDetailConfig
        ( FK_BrandFlagship ,
          ArticleID ,
          ArticleTitle ,
          ArticleType,
          [Description],
          OrderBy
        )
VALUES  ( @FK_BrandFlagship , -- FK_BrandFlagship - int
          @ArticleID , -- ArticleID - int
          @ArticleTitle , -- ArticleTitle - nvarchar(200)
          @ArticleType,  -- ArticleType - int
          @Description,
          @OrderBy
        )";
                foreach (var item in model.Information)
                {
                    SqlCommand cmdItem = new SqlCommand();
                    cmdItem.CommandText = sqlDeatil;
                    cmdItem.CommandType = CommandType.Text;
                    cmdItem.Parameters.AddWithValue("@FK_BrandFlagship", model.ID);
                    cmdItem.Parameters.AddWithValue("@ArticleID", item.ArticleID);
                    cmdItem.Parameters.AddWithValue("@ArticleTitle", item.ArticleTitle);
                    cmdItem.Parameters.AddWithValue("@ArticleType", item.ArticleType);
                    cmdItem.Parameters.AddWithValue("@Description",item.Description);
                    cmdItem.Parameters.AddWithValue("@OrderBy",item.OrderBy);
                    db.ExecuteNonQuery(cmdItem);
                }
                foreach (var item in model.Testing)
                {
                    SqlCommand cmdItem = new SqlCommand();
                    cmdItem.CommandText = sqlDeatil;
                    cmdItem.CommandType = CommandType.Text;
                    cmdItem.Parameters.AddWithValue("@FK_BrandFlagship", model.ID);
                    cmdItem.Parameters.AddWithValue("@ArticleID", item.ArticleID);
                    cmdItem.Parameters.AddWithValue("@ArticleTitle", item.ArticleTitle);
                    cmdItem.Parameters.AddWithValue("@ArticleType", item.ArticleType);
                    cmdItem.Parameters.AddWithValue("@Description",item.Description);
                    cmdItem.Parameters.AddWithValue("@OrderBy",item.OrderBy);
                    db.ExecuteNonQuery(cmdItem);
                }

                db.Commit();
                result = 1;
            }
            catch (Exception e)
            {
                db.Rollback();
            }
            finally
            {
                db.Dispose();
            }
            return result;
        }

        public static DataTable GetList()
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_BrandFlagshipConfig WITH(NOLOCK)";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                return db.ExecuteDataTable(cmd);
            }
        }


        public static DataTable GetBrandFlagship(string id)
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_BrandFlagshipConfig WITH(NOLOCK) where ID=@ID";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID",id);
                return db.ExecuteDataTable(cmd);
            }
        }


        public static DataTable GetBrandFlagshipDeatil(string id,int type)
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_BrandFlagshipDetailConfig WITH(NOLOCK) WHERE FK_BrandFlagship=@ID and ArticleType=@ArticleType";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID",id);
                cmd.Parameters.AddWithValue("@ArticleType",type);
                return db.ExecuteDataTable(cmd);
            }
        }

        public static bool ExistBrandFlagshipDeatil(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from SE_BrandFlagshipDetail");
            strSql.Append(" where ID=@ID");
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(strSql.ToString());
                cmd.Parameters.AddWithValue("@ID",id);
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }


        public static bool Delete(string id)
        {
            bool result = false;
            var db = DbHelper.CreateDefaultDbHelper();
            try
            {
                db.BeginTransaction();
                string sql = "DELETE FROM Configuration.dbo.SE_BrandFlagshipConfig WHERE ID=@ID";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID",id);
                db.ExecuteNonQuery(cmd);

                string delStr = @"DELETE FROM Configuration.dbo.SE_BrandFlagshipDetailConfig WHERE FK_BrandFlagship=@FK_BrandFlagship";
                cmd = new SqlCommand(delStr);
                cmd.Parameters.AddWithValue("@FK_BrandFlagship",id);
                db.ExecuteNonQuery(cmd);
                db.Commit();
                result = true;
            }
            catch (Exception e)
            {
                db.Rollback();
            }
            finally
            {
                db.Dispose();
            }
            return result;
        }

    }
}
