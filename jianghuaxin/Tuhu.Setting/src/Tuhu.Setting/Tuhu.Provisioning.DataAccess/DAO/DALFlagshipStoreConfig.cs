using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Common;



namespace Tuhu.Provisioning.DataAccess.DAO
{
   public class DALFlagshipStoreConfig
    {
       
        private static AsyncDbHelper db = null;
        public DALFlagshipStoreConfig()
        {
            db = DbHelper.CreateDefaultDbHelper();
        }


        public bool Add(SE_FlagshipStoreConfig model)
        {
            StringBuilder sql = new StringBuilder("INSERT INTO configuration.dbo.SE_FlagshipStoreConfig(Brand,Name,Describe,Uri,ImageUrl,Remark,CreateDT,UpdateDT)");
            sql.Append(" VALUES(@Brand,@Name,@Describe,@Uri,@ImageUrl,@Remark,GETDATE(),GETDATE()) ");
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql.ToString()))
                {
                    cmd.Parameters.AddWithValue("@Brand", model.Brand);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Describe", model.Describe);
                    cmd.Parameters.AddWithValue("@Uri", model.Uri);
                    cmd.Parameters.AddWithValue("@ImageUrl", model.ImageUrl.Trim());
                    cmd.Parameters.AddWithValue("@Remark", model.Remark);
                    return db.ExecuteNonQuery(cmd) > 0;
                }

            }
            catch (Exception e)
            {
                return false;
            }
           
        }


        public bool Update(SE_FlagshipStoreConfig model)
        {
            StringBuilder sql = new StringBuilder("UPDATE Configuration.DBO.SE_FlagshipStoreConfig SET Brand=@Brand,Name=@Name,Describe=@Describe,Uri=@Uri,ImageUrl=@ImageUrl,Remark=@Remark,UpdateDT=GETDATE() Where PKID=@PKID ");
            using (SqlCommand cmd = new SqlCommand(sql.ToString()))
            {
                cmd.Parameters.AddWithValue("@Brand", model.Brand);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Describe", model.Describe);
                cmd.Parameters.AddWithValue("@Uri", model.Uri);
                cmd.Parameters.AddWithValue("@ImageUrl", model.ImageUrl.Trim());
                cmd.Parameters.AddWithValue("@Remark", model.Remark);
                cmd.Parameters.AddWithValue("@PKID",model.PKID);
                int n = db.ExecuteNonQuery(cmd);
                return n > 0;
            }
        }


        public DataTable GetDataTable()
        {
            string sql = "SELECT * FROM configuration.dbo.SE_FlagshipStoreConfig (NOLOCK) ";
            return db.ExecuteDataTable(new SqlCommand(sql));
        }

        public DataTable GetDataRow(string PKID)
        {
            string sql = "SELECT * FROM configuration.dbo.SE_FlagshipStoreConfig (NOLOCK) Where PKID=@PKID ";
            return db.ExecuteDataTable(new SqlCommand(sql));
        }


        public DataTable GetBrand()
        {
            string sql = "SELECT DISTINCT CP_Brand FROM Tuhu_productcatalog.dbo.vw_Products (NOLOCK)  WHERE CP_Brand IS NOT NULL AND CP_Brand NOT IN ( SELECT Brand FROM configuration.dbo.SE_FlagshipStoreConfig (NOLOCK) )   ";
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                return db.ExecuteDataTable(cmd);
            }
        }

        public bool Delete(string PKID)
        {
            string sql = "DELETE FROM Configuration.dbo.SE_FlagshipStoreConfig WHERE PKID=@PKID ";
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID",PKID);
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }

         ~DALFlagshipStoreConfig()
        {
            db.Dispose();
        }


    }
}
