using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity.DeviceConfig;

namespace Tuhu.Provisioning.DataAccess.DAO.DeviceConfig
{
    public class DalDeviceBrand
    {

        private static readonly string ConnStr;

        static DalDeviceBrand()
        {
            string gungnirConnStr = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(gungnirConnStr))
            {
                gungnirConnStr = SecurityHelp.DecryptAES(gungnirConnStr);
            }
            ConnStr = gungnirConnStr;
        }



        public static IEnumerable<DeviceBrandEntity> GetAllBrandList()
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                var sql = "SELECT * FROM Configuration.dbo.tbl_DeviceBrand AS tdb";

                return conn.Query<DeviceBrandEntity>(sql);
            }
        }


        public static IEnumerable<DeviceModelInfo> GetAllInfo()
        {

            const string sql = @"
SELECT 
tdb.PKID AS  BrandID,tdb.DeviceBrand,
tdt.PKID AS TypeID,tdt.DeviceType, 
tdm.PKID AS ModelID,tdm.DeviceModel,
COALESCE(tdm.CreateDateTime,tdt.CreateDateTime,tdb.CreateDateTime) AS CreateDateTime,
COALESCE(tdm.UpdateDateTime,tdt.UpdateDateTime,tdb.UpdateDateTime) AS UpdateDateTime,
COALESCE(tdm.CreateUser,tdt.CreateUser,tdt.CreateUser) AS CreateUser,
COALESCE(tdm.UpdateUser,tdm.UpdateUser,tdb.UpdateUser) AS UpdateUser
FROM Configuration.dbo.tbl_DeviceBrand AS tdb
LEFT JOIN Configuration.dbo.tbl_DeviceType AS tdt ON tdb.PKID = tdt.BrandID
LEFT JOIN Configuration.dbo.tbl_DeviceModel AS tdm ON tdt.PKID = tdm.TypeID
";
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                return conn.Query<DeviceModelInfo>(sql);
            } 
        }


        public static int InsertBrand(string deviceBrand,string userName)
        {
            using (var conn = new SqlConnection(deviceBrand))
            {
                conn.Open();

                var sql = @"
INSERT INTO  Configuration.dbo.tbl_DeviceBrand
         ( DeviceBrand ,
           CreateDateTime ,
           UpdateDateTime ,
           CreateUser ,
           UpdateUser 
         )
 VALUES  ( @DeviceBrand , -- DeviceBrand - nvarchar(30)
           GETDATE() , -- CreateDateTime - datetime
           GETDATE() , -- UpdateDateTime - datetime
           @CreateUser,
           @UpdateUser
         );
Select SCOPE_IDENTITY();
";

                var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@DeviceBrand", deviceBrand),
                    new SqlParameter("@CreateUser",userName), 
                    new SqlParameter("@UpdateUser",userName)
                };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParam));
            }
        }


        public static bool UpdateBrand(DeviceBrandEntity entity)
        {

            using (var conn = new SqlConnection(ConnStr))
            {
                var sql = @"
 UPDATE Configuration.dbo.tbl_DeviceBrand
 SET DeviceBrand =@DeviceBrand,UpdateDateTime = GETDATE(),UpdateUser =@UpdateUser
 WHERE PKID = @Pkid
"; 

                var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@Pkid",entity.PKID),
                    new SqlParameter("@DeviceBrand", entity.DeviceBrand),
                    new SqlParameter("@UpdateUser", entity.UpdateUser),
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0;
            }
        }

        public static bool DeleteBrand(int brandId)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                var sql = @"DELETE FROM Configuration.dbo.tbl_DeviceBrand WHERE PKID = @id;";
                var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@id",brandId)
                };

                return SqlHelper.ExecuteNonQuery(conn,CommandType.Text,sql , sqlParam)>0;
            } 
        }
         
        public static int InsertType(DeviceTypeEntity entity)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                var sql = @"
 INSERT INTO Configuration.dbo.tbl_DeviceType
         ( BrandID ,
           DeviceType ,
           CreateDateTime ,
           UpdateDateTime ,
           CreateUser,
           UpdateUser 
         )
 VALUES  ( @BrandID , -- BrandID - int
           @DeviceType , -- DeviceType - nvarchar(30)
           GETDATE() , -- CreateDateTime - datetime
           GETDATE() , -- UpdateDateTime - datetime
           @CreateUser,
           @UpdateUser
         );
Select SCOPE_IDENTITY();
";

                var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@BrandID", entity.BrandId),
                    new SqlParameter("@DeviceType", entity.DeviceType),
                    new SqlParameter("@CreateUser", entity.CreateUser),
                    new SqlParameter("@UpdateUser", entity.UpdateUser),
                };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParam));
            }
        }

        public static bool DeleteType(int typeId)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                var sql = @"DELETE FROM Configuration.dbo.tbl_DeviceType WHERE PKID = @id;";
                var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@id",typeId)
                };

                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0;
            }
        }

        public static bool UpdateType(DeviceTypeEntity entity)
        {
            using (var con = new SqlConnection(ConnStr))
            {
                var sql = @"
 UPDATE Configuration.dbo.tbl_DeviceType 
 SET BrandID =@brandId, DeviceType =@deviceType,UpdateDateTime =GETDATE(),UpdateUser=@UpdateUser
 WHERE PKID = @Pkid
"; 

                var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@brandId", entity.BrandId),
                    new SqlParameter("@deviceType", entity.DeviceType),
                    new SqlParameter("@UpdateUser", entity.UpdateUser),
                    new SqlParameter("@Pkid", entity.PKID),
                };

                return SqlHelper.ExecuteNonQuery(con, CommandType.Text, sql, sqlParam) > 0;
            }
        }

        public static int InsertModel(DeviceModelEntity entity)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                var sql = @"
 INSERT INTO Configuration.dbo.tbl_DeviceModel
         ( TypeID ,
           DeviceModel ,
           CreateDateTime ,
           UpdateDateTime ,
           CreateUser,
           UpdateUser
         )
 VALUES  ( @TypeID , -- TypeID - int
           @DeviceModel , -- DeviceModel - nvarchar(30)
           GETDATE() , -- CreateDateTime - datetime
           GETDATE() , 
           @CreateUser,
           @UpdateUser
         );
Select SCOPE_IDENTITY();
";

                var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@TypeID", entity.TypeId),
                    new SqlParameter("@DeviceModel", entity.DeviceModel),
                    new SqlParameter("@CreateUser", entity.CreateUser),
                    new SqlParameter("@UpdateUser", entity.UpdateUser),
                };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParam));
            }
        }



        public static bool UpdateModel(DeviceModelEntity entity)
        {
            using (var con = new SqlConnection(ConnStr))
            {
                var sql = @"
 UPDATE Configuration.dbo.tbl_DeviceModel
 SET TypeID =@typeId ,DeviceModel = @deviceModel ,UpdateDateTime = GETDATE(),  UpdateUser = @UpdateUser
 WHERE PKID = @Pkid
";

                var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@typeId", entity.TypeId),
                    new SqlParameter("@deviceModel", entity.DeviceModel),
                    new SqlParameter("@UpdateUser", entity.UpdateUser),
                    new SqlParameter("@Pkid", entity.PKID),
                };

                return SqlHelper.ExecuteNonQuery(con, CommandType.Text, sql, sqlParam) > 0;
            }
        }



        public static bool DeleteModel(int modelId)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                var sql = @"DELETE FROM Configuration.dbo.tbl_DeviceModel WHERE PKID = @id;";
                var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@id",modelId)
                };

                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0;
            }
        }





    }
}
