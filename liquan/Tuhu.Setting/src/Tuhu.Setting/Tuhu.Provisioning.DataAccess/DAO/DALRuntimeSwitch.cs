using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALRuntimeSwitch
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);
        public static List<RuntimeSwitch> GetRuntimeSwitch()
        {
            string sql = @" SELECT PKID,SwitchName,Value,CreatedTime,UpdatedTime,Description FROM [Gungnir].[dbo].[RuntimeSwitch] WITH (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<RuntimeSwitch>().ToList();

        }
        public static RuntimeSwitch GetRuntimeSwitch(int id)
        {
            const string sql = @"SELECT TOP 1 PKID,SwitchName,Value,CreatedTime,UpdatedTime,Description FROM [Gungnir].[dbo].[RuntimeSwitch] WITH (NOLOCK) WHERE PKID=@Id";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<RuntimeSwitch>().ToList().FirstOrDefault();
        }
        public static bool UpdateRuntimeSwitch(RuntimeSwitch model)
        {
            const string sql = @"UPDATE [Gungnir].[dbo].[RuntimeSwitch] SET Value=@Value,CreatedTime=GETDATE(),Description=@desc WHERE PKID=@Id";
            //const string sql = @"UPDATE [Gungnir].[dbo].[RuntimeSwitch] SET Value=@Value,UpdatedTime=GETDATE() WHERE PKID=@Id";
            var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@Value",model.Value),
                    new SqlParameter("@Id",model.PKID),
                    new SqlParameter("@desc",model.Description)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0;
        }
        public static bool DeleteRuntimeSwitch(int id)
        {
            const string sql = @"DELETE  FROM [Gungnir].[dbo].[RuntimeSwitch] WHERE PKID = @id";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@id", id)) > 0;
        }
        public static int InsertRuntimeSwitch(RuntimeSwitch model)
        {
            const string sql1 = @"SELECT  COUNT(0) FROM [Gungnir].[dbo].[RuntimeSwitch] WITH ( NOLOCK ) WHERE [SwitchName]=@SwitchName ";

            var sqlParam1 = new SqlParameter[]
           {
               new SqlParameter("@Switchname",model.SwitchName)
           };

            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql1, sqlParam1);
            if (count > 0)
            {
                return -1;
            }

            const string sql = @"INSERT INTO Gungnir..RuntimeSwitch
                                        (SwitchName, Value, Description, CreatedTime, UpdatedTime)
                                VALUES  (
                                         @SwitchName,
                                         @Value,
                                         @Description,
                                         GETDATE(),
                                         GETDATE()
                                         )";

            var sqlParam = new SqlParameter[]
             {
                    new SqlParameter("@SwitchName",model.SwitchName),
                    new SqlParameter("@Value",model.Value),
                    new SqlParameter("@Description",model.Description)
             };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam);
        }
    }
}
