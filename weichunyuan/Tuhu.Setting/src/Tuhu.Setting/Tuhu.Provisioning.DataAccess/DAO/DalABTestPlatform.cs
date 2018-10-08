using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.ABTestPlatform;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalABTestPlatform
    {
        private static AsyncDbHelper db = null;

        public DalABTestPlatform()
        {
            db = DbHelper.CreateDefaultDbHelper();
        }

        public DataTable GetABTestDetail()
        {
            string sql = @"SELECT *
                            FROM   [Configuration].[dbo].[ABTestConfig] WITH ( NOLOCK )
                            WHERE IsDelete =0";
            return db.ExecuteDataTable(new SqlCommand(sql));
        }

        public DataTable GetAllTestName()
        {
            string sql = @"SELECT TestName
                            FROM   [Configuration].[dbo].[ABTestConfig] WITH ( NOLOCK )";
            return db.ExecuteDataTable(new SqlCommand(sql));
        }

        public DataTable GetABTestDetailByGuid(Guid testGuid)
        {
            string sql = @"SELECT *
                            FROM   [Configuration].[dbo].[ABTestConfig] WITH ( NOLOCK )
                            WHERE IsDelete =0
                                  AND TestGuid = @TestGuid";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@TestGuid", testGuid.ToString("D"));

            return db.ExecuteDataTable(cmd);
        }

        public DataTable GetABTestGroupDetailByGuid(Guid testGuid)
        {
            string sql = @"SELECT *
                            FROM   [Configuration].dbo.ABTestGroupDetail WITH ( NOLOCK )
                            WHERE  TestGuid = @TestGuid";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@TestGuid", testGuid.ToString("D"));

            return db.ExecuteDataTable(cmd);
        }

        public bool DeleteABTestByGuid(Guid testGuid)
        {
            string sql = @"UPDATE [Configuration].[dbo].[ABTestConfig]
                            SET    IsDelete = 1
                            WHERE  TestGuid = @TestGuid";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@TestGuid", testGuid);

            return db.ExecuteNonQuery(cmd) > 0;
        }

        public bool SelectABTestResult(string testGuid, string testGroupId)
        {
            string sql = @"UPDATE [Configuration].[dbo].[ABTestConfig]
                            SET    [Status] = 'Done' ,
                                   TestScale = 1 ,
                                   LastUpdateDataTime = GETDATE()
                            WHERE  TestGuid = @TestGuid";
            string sqlGroup = @"UPDATE [Configuration].dbo.ABTestGroupDetail
                            SET    Selected = 1 ,
                                   LastUpdateDataTime = GETDATE()
                            WHERE  TestGuid = @TestGuid
                                   AND GroupId = @GroupId";
            using (var cmd = new SqlCommand(sql))
            using (var cmdGroup = new SqlCommand(sqlGroup))

            using (var dbHelper = DbHelper.CreateDefaultDbHelper())
            {
                dbHelper.BeginTransaction();

                try
                {
                    cmd.Parameters.AddWithValue("@TestGuid", testGuid);

                    var result = dbHelper.ExecuteNonQuery(cmd);
                    if (result <= 0)
                    {
                        dbHelper.Rollback();
                        return false;
                    }

                    cmdGroup.Parameters.AddWithValue("@TestGuid", testGuid);
                    cmdGroup.Parameters.AddWithValue("@GroupId", testGroupId);

                    var resultGroup = dbHelper.ExecuteNonQuery(cmdGroup);
                    if (resultGroup <= 0)
                    {
                        dbHelper.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    return false;
                }
                dbHelper.Commit();
                return true;
            }
        }

        public bool InsertLog(ABTestEditLog log)
        {
            string sql = @"INSERT [Configuration].dbo.ABTestEditLog (   TestGuid ,
                                             TestName ,
                                             Change ,
                                             Operator ,
                                             CreateTime ,
                                             LastUpdateDataTime
                                         )
                            VALUES ( @TestGuid ,
                                     @TestName ,
                                     @Change ,
                                     @Operator ,
                                     GETDATE(),
                                     GETDATE()
                                     );";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@TestGuid", log.TestGuid);
            cmd.Parameters.AddWithValue("@TestName", log.TestName);
            cmd.Parameters.AddWithValue("@Change", log.Change);
            cmd.Parameters.AddWithValue("@Operator", log.Operator);

            return db.ExecuteNonQuery(cmd) > 0;
        }

        public DataTable GetLog(Guid testGuid)
        {
            string sql = @"SELECT *
                            FROM   [Configuration].dbo.ABTestEditLog WITH ( NOLOCK )
                            WHERE TestGuid = @TestGuid";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@TestGuid", testGuid.ToString("D"));

            return db.ExecuteDataTable(cmd);
        }
    }
}
