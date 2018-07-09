using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalCategoryBrandRank
    {
        public static IEnumerable<CategoryBrandRankModel> SelectCategoryBrand(long pkid, DateTime? date,BaseDbHelper db)
        {
            string sql =
                @"SELECT * FROM Activity..tbl_CategoryBrandRank WITH(NOLOCK) WHERE ParentPkid=@PKID";
            if (date != null)
            {
                sql += " AND Date=@Date";
            }

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID",pkid);
                if (date !=null)
                {
                    cmd.Parameters.AddWithValue("@Date", date.Value.ToString("yyyy-MM-dd"));
                }
                return db.ExecuteDataTable(cmd).ConvertTo<CategoryBrandRankModel>();

            }
        }

        public static CategoryBrandRankModel FetchCategoryBrand(long pkid,BaseDbHelper db)
        {
            string sql =
                @"SELECT * FROM Activity..tbl_CategoryBrandRank WITH(NOLOCK) WHERE PKID=@PKID";
            

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return db.ExecuteDataTable(cmd).ConvertTo<CategoryBrandRankModel>().FirstOrDefault();

            }
        }


        public static int UpdateCategoryBrand(CategoryBrandRankModel model,BaseDbHelper db)
        {
            string sql =
                @"UPDATE Activity..[tbl_CategoryBrandRank] WITH(ROWLOCK)
                    SET 
                         [ParentPkid] = @ParentPkid
                        ,[Name] = ISNULL(@Name,[Name])
                        ,[NameIndex] = @NameIndex
                        ,[PageTitle] = ISNULL(@PageTitle,PageTitle)
                        ,[PageShareTitle] = ISNULL(@PageShareTitle,PageShareTitle)
                        ,[PageShareDescription] = ISNULL(@PageShareDescription,PageShareDescription)
                        ,[PageShareContent] = ISNULL(@PageShareContent,PageShareContent)
                    WHERE PKID=@PKID";
            
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ParentPkid", model.ParentPkid);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@NameIndex",model.NameIndex);
                cmd.Parameters.AddWithValue("@PageTitle", model.PageTitle);
                cmd.Parameters.AddWithValue("@PageShareTitle", model.PageShareTitle);
                cmd.Parameters.AddWithValue("@PageShareDescription",model.PageShareDescription);
                cmd.Parameters.AddWithValue("@PageShareContent",model.PageShareContent);
                cmd.Parameters.AddWithValue("@PKID", model.PKID);
                return db.ExecuteNonQuery(cmd);

            }
        }

        public static int DeleteCategoryBrand(long id, BaseDbHelper db)
        {
            string sql =
                @"DELETE FROM Activity..[tbl_CategoryBrandRank] WHERE PKID=@PKID";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", id);
                return db.ExecuteNonQuery(cmd);

            }
        }

        public static long InsertCategoryBrand(CategoryBrandRankModel model, BaseDbHelper db)
        {
            string sql =
                @"INSERT INTO [Activity].[dbo].[tbl_CategoryBrandRank]
                   ([ParentPkid]
                   ,[Date]
                   ,[Name]
                   ,[NameIndex]
                   ,[PageTitle]
                   ,[PageShareTitle]
                   ,[PageShareDescription]
                   ,[PageShareContent]
                   ,[CreateTime])
             VALUES
                   (@ParentPkid
                   ,@Date
                   ,@Name
                   ,@NameIndex
                   ,@PageTitle
                   ,@PageShareTitle
                   ,@PageShareDescription
                   ,@PageShareContent
                   ,GETDATE());SELECT @@IDENTITY";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ParentPkid", model.ParentPkid);
                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@NameIndex", model.NameIndex);
                cmd.Parameters.AddWithValue("@PageTitle", model.PageTitle);
                cmd.Parameters.AddWithValue("@PageShareTitle", model.PageShareTitle);
                cmd.Parameters.AddWithValue("@PageShareDescription", model.PageShareDescription);
                cmd.Parameters.AddWithValue("@PageShareContent", model.PageShareContent);
                return long.Parse(db.ExecuteScalar(cmd).ToString());

            }
        }
    }
}