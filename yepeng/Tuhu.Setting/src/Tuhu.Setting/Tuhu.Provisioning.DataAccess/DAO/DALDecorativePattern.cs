using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using System.Data.SqlClient;
using System.Data;


namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALDecorativePattern
    {

        public static bool Add(SE_DecorativePattern model)
        {
            bool result = false;
            string sql = @"INSERT INTO Configuration.dbo.SE_DecorativePatternConfig
        ( ID ,
          Name ,
          Brand ,
          Flower ,
          ImageUrl1 ,
          ImageUrl2 ,
          ImageUrl3,
        Description,
        ArticleID,
        ArticleTitle,
        CreateDT,
        UpdateDT,
        ShareParameter
        )
VALUES  ( @ID , -- ID - uniqueidentifier
          @Name , -- Name - nvarchar(100)
          @Brand , -- Brand - nvarchar(200)
          @Flower , -- Flower - nvarchar(200)
          @ImageUrl1 , -- ImageUrl1 - nvarchar(200)
          @ImageUrl2 , -- ImageUrl2 - nvarchar(200)
          @ImageUrl3 ,  -- ImageUrl3 - nvarchar(200)
         @Description,
            @ArticleID,
            @ArticleTitle,
          GETDATE(),
            GETDATE(),
          @ShareParameter
        )";
            var db = DbHelper.CreateDefaultDbHelper();
            try
            {
                db.BeginTransaction();
                model.ID = Guid.NewGuid();
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID", model.ID.ToString());
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Brand", model.Brand);
                cmd.Parameters.AddWithValue("@Flower", model.Flower);
                cmd.Parameters.AddWithValue("@ImageUrl1", model.ImageUrl1);
                cmd.Parameters.AddWithValue("@ImageUrl2", model.ImageUrl2);
                cmd.Parameters.AddWithValue("@ImageUrl3", model.ImageUrl3);
                cmd.Parameters.AddWithValue("@Description",model.Description);
                cmd.Parameters.AddWithValue("@ArticleID",model.ArticleID);
                cmd.Parameters.AddWithValue("@ArticleTitle",model.ArticleTitle);
                cmd.Parameters.AddWithValue("@ShareParameter",model.ShareParameter);
                db.ExecuteNonQuery(cmd);

                sql = @"INSERT INTO configuration.dbo.SE_DecorativePatternDetailConfig
        ( FK_DecorativePattern ,
          ArticleID ,
          ArticleTitle,
          [Image],
            [Description],
            OrderBy
        )
VALUES  ( @FK_DecorativePattern , -- FK_DecorativePattern - uniqueidentifier
          @ArticleID , -- ArticleID - int
          @ArticleTitle,  -- ArticleTitle - nvarchar(200)
           @Image,
            @Description,
          @OrderBy
        )";
                foreach (var item in model.Items)
                {
                    cmd = new SqlCommand(sql);
                    cmd.Parameters.AddWithValue("@FK_DecorativePattern", model.ID.ToString());
                    cmd.Parameters.AddWithValue("@ArticleID", item.ArticleID);
                    cmd.Parameters.AddWithValue("@ArticleTitle", item.ArticleTitle);
                    cmd.Parameters.AddWithValue("@Image",item.Image);
                    cmd.Parameters.AddWithValue("@Description",item.Description);
                    cmd.Parameters.AddWithValue("@OrderBy",item.OrderBy);
                    db.ExecuteNonQuery(cmd);
                }
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


        public static bool Update(SE_DecorativePattern model)
        {
            bool result = false;
            string sql = @"		UPDATE Configuration.dbo.SE_DecorativePatternConfig SET Name=@Name,Brand=@Brand, Flower=@Flower, ImageUrl1=@ImageUrl1, ImageUrl2=@ImageUrl2, ImageUrl3=@ImageUrl3,Description=@Description,ArticleID=@ArticleID,ArticleTitle=@ArticleTitle,UpdateDT=GETDATE(),ShareParameter=@ShareParameter  WHERE ID=@ID ";
            var db = DbHelper.CreateDefaultDbHelper();
            try
            {
                db.BeginTransaction();
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID", model.ID.ToString());
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Brand", model.Brand);
                cmd.Parameters.AddWithValue("@Flower", model.Flower);
                cmd.Parameters.AddWithValue("@ImageUrl1", model.ImageUrl1);
                cmd.Parameters.AddWithValue("@ImageUrl2", model.ImageUrl2);
                cmd.Parameters.AddWithValue("@ImageUrl3", model.ImageUrl3);
                cmd.Parameters.AddWithValue("@Description",model.Description);
                cmd.Parameters.AddWithValue("@ArticleID",model.ArticleID);
                cmd.Parameters.AddWithValue("@ArticleTitle",model.ArticleTitle);
                cmd.Parameters.AddWithValue("@ShareParameter",model.ShareParameter);
                db.ExecuteNonQuery(cmd);


                sql = @"DELETE FROM Configuration.dbo.SE_DecorativePatternDetailConfig WHERE FK_DecorativePattern=@FK_DecorativePattern ";
                cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FK_DecorativePattern", model.ID.ToString());
                db.ExecuteNonQuery(cmd);


                sql = @"INSERT INTO configuration.dbo.SE_DecorativePatternDetailConfig
        ( FK_DecorativePattern ,
          ArticleID ,
          ArticleTitle,
            Image,
            [Description],
            OrderBy
        )
VALUES  ( @FK_DecorativePattern , -- FK_DecorativePattern - uniqueidentifier
          @ArticleID , -- ArticleID - int
          @ArticleTitle , -- ArticleTitle - nvarchar(200)
         @Image,
            @Description,
            @OrderBy
        )";
                


                foreach (var item in model.Items)
                {
                    cmd = new SqlCommand(sql);
                    cmd.Parameters.AddWithValue("@FK_DecorativePattern", model.ID.ToString());
                    cmd.Parameters.AddWithValue("@ArticleID", item.ArticleID);
                    cmd.Parameters.AddWithValue("@ArticleTitle", item.ArticleTitle);
                    cmd.Parameters.AddWithValue("@Image",item.Image);
                    cmd.Parameters.AddWithValue("@Description",item.Description);
                    cmd.Parameters.AddWithValue("@OrderBy",item.OrderBy);
                    db.ExecuteNonQuery(cmd);
                }
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

        public static SE_DecorativePattern GetEntity(string id)
        {
            SE_DecorativePattern model = null;
            string sql = " SELECT * FROM Configuration.dbo.SE_DecorativePatternConfig WITH(NOLOCK) WHERE ID=@ID";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID", id);
                model = db.ExecuteDataTable(cmd).ConvertTo<SE_DecorativePattern>().FirstOrDefault();
                cmd = new SqlCommand("SELECT * FROM Configuration.dbo.SE_DecorativePatternDetailConfig WITH(NOLOCK) WHERE FK_DecorativePattern=@FK_DecorativePattern ");
                cmd.Parameters.AddWithValue("@FK_DecorativePattern", id);
                if (model != null)
                {
                    model.Items = db.ExecuteDataTable(cmd).ConvertTo<SE_DecorativePatternDetail>();
                }
                return model;
            }
        }


        public static IEnumerable<SE_DecorativePattern> GetList()
        {
            string sql = " SELECT * FROM Configuration.dbo.SE_DecorativePatternConfig WITH(NOLOCK) ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                return db.ExecuteDataTable(cmd).ConvertTo<SE_DecorativePattern>();

            }
        }

        public static IEnumerable<SE_DecorativePattern> GetList(string type, string name)
        {
            string sql = " SELECT * FROM Configuration.dbo.SE_DecorativePatternConfig WITH(NOLOCK) ";
          
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand();
                if (type == "0")
                {
                    sql += " where Name like  N'%"+name+"%'";
                  //  cmd.Parameters.AddWithValue("@Name", name);
                }
                else if (type == "1")
                {
                    sql += " where Brand like  N'%"+ name + "%' ";
                   // cmd.Parameters.AddWithValue("@Brand", name);
                }
                else if (type == "2")
                {
                    sql += " where Flower like  N'%"+name+"%' ";
                  //  cmd.Parameters.AddWithValue("@Flower", name);
                }
                else { }

                cmd.CommandText = sql;
               
                return db.ExecuteDataTable(cmd).ConvertTo<SE_DecorativePattern>();

            }
        }


        public static bool DeleteDeatil( string id)
        {
            string sql = @"DELETE FROM Configuration.dbo.SE_DecorativePatternDetailConfig WHERE FK_DecorativePattern=@FK_DecorativePattern ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FK_DecorativePattern", id);
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }


        public static bool Delete(string id)
        {
            bool result = false;
            string sql = @"DELETE FROM Configuration.dbo.SE_DecorativePatternConfig WHERE ID=@ID";
            var db = DbHelper.CreateDefaultDbHelper();
            try
            {
                db.BeginTransaction();
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID",id);
                db.ExecuteNonQuery(cmd);
                sql = @"DELETE FROM Configuration.dbo.SE_DecorativePatternDetailConfig WHERE FK_DecorativePattern=@FK_DecorativePattern ";
                cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FK_DecorativePattern", id);
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


        public static DataTable GetTirePatten(string tire)
        {
            string sql = "SELECT DISTINCT CP_Tire_Pattern  FROM Tuhu_productcatalog..vw_Products WITH(NOLOCK)  WHERE  CP_Tire_Pattern IS NOT NULL AND CP_Brand =@CP_Brand ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@CP_Brand",tire);
                return db.ExecuteDataTable(cmd);
            }
        }


    }
}
