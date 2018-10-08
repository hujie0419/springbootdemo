using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using System.Data;
using System.Data.SqlClient;

namespace Tuhu.Provisioning.DataAccess.DAO
{
   public static class DALSE_DictionaryConfig
    {
        public static bool Add(SE_DictionaryConfigModel entity)
        {
            StringBuilder sql = new StringBuilder(@"INSERT INTO Configuration.dbo.SE_DictionaryConfig
	        ( ParentId ,
	          [Key] ,
	          Value ,
	          Describe ,
	          Sort ,
	          [State] ,
	          Url ,
	          Images ,
	          CreateTime ,
	          UpdateTime ,
	          Extend1 ,
	          Extend2 ,
	          Extend3 ,
	          Extend4 ,
	          Extend5
	        )");
            sql.Append("VALUES(@ParentId,@Key,@Value,@Describe,@Sort,@State,@Url,@Images,GETDATE(),GETDATE(),@Extend1,@Extend2,@Extend3,@Extend4,@Extend5)");

            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql.ToString());
                cmd.Parameters.AddWithValue("@ParentId", entity.ParentId);
                cmd.Parameters.AddWithValue("@Key", entity.Key);
                cmd.Parameters.AddWithValue("@Value", entity.Value);
                cmd.Parameters.AddWithValue("@Describe", entity.Describe);
                cmd.Parameters.AddWithValue("@Sort", entity.Sort);
                cmd.Parameters.AddWithValue("@State", entity.State);
                cmd.Parameters.AddWithValue("@Url",entity.Url);
                cmd.Parameters.AddWithValue("@Images",entity.Images);
                cmd.Parameters.AddWithValue("@Extend1",entity.Extend1);
                cmd.Parameters.AddWithValue("@Extend2", entity.Extend2);
                cmd.Parameters.AddWithValue("@Extend3", entity.Extend3);
                cmd.Parameters.AddWithValue("@Extend4", entity.Extend4);
                cmd.Parameters.AddWithValue("@Extend5", entity.Extend5);
                return  db.ExecuteNonQuery(cmd) > 0;
            }
           

        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Exist(SE_DictionaryConfigModel entity)
        {
            string sql = "   SELECT COUNT(1) FROM Configuration.dbo.SE_DictionaryConfig WHERE [Key]=@Key  ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Key", entity.Key);
                int n = (int)db.ExecuteScalar(cmd);
                return  n> 0;
            }
        }


        public static bool Update(SE_DictionaryConfigModel entity)
        {
            string sql = @"          UPDATE Configuration.dbo.SE_DictionaryConfig  SET ParentId=@ParentId ,
	          [Key]=@Key ,
	          Value=@Value ,
	          Describe=@Describe ,
	          Sort=@Sort ,
	          [State]=@State ,
	          Url=@Url ,
	          Images=@Images ,
	          UpdateTime=GETDATE() ,
	          Extend1=@Extend1 ,
	          Extend2 =@Extend2,
	          Extend3=@Extend3 ,
	          Extend4=@Extend4,
	          Extend5=@Extend5
          WHERE Id=@Id ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql.ToString());
                cmd.Parameters.AddWithValue("@ParentId", entity.ParentId);
                cmd.Parameters.AddWithValue("@Key", entity.Key);
                cmd.Parameters.AddWithValue("@Value", entity.Value);
                cmd.Parameters.AddWithValue("@Describe", entity.Describe);
                cmd.Parameters.AddWithValue("@Sort", entity.Sort);
                cmd.Parameters.AddWithValue("@State", entity.State);
                cmd.Parameters.AddWithValue("@Url", entity.Url);
                cmd.Parameters.AddWithValue("@Images", entity.Images);
                cmd.Parameters.AddWithValue("@Extend1", entity.Extend1);
                cmd.Parameters.AddWithValue("@Extend2", entity.Extend2);
                cmd.Parameters.AddWithValue("@Extend3", entity.Extend3);
                cmd.Parameters.AddWithValue("@Extend4", entity.Extend4);
                cmd.Parameters.AddWithValue("@Extend5", entity.Extend5);
                cmd.Parameters.AddWithValue("@Id", entity.Id);
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static DataTable GetTable()
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_DictionaryConfig WITH(NOLOCK) ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                return db.ExecuteDataTable(cmd);
            }
        }

        public static DataTable GetEntity(string id)
        {
            string sql = "   SELECT * FROM Configuration.dbo.SE_DictionaryConfig WHERE Id=@Id  ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Id", id);
               return db.ExecuteDataTable(cmd);
               
            }
        }


        public static bool Delete(string id)
        {
            string sql = " DELETE FROM Configuration.dbo.SE_DictionaryConfig WHERE Id=@Id  ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Id", id);
                int n = (int)db.ExecuteNonQuery(cmd);
                return n > 0;
            }
        }

    }
}
