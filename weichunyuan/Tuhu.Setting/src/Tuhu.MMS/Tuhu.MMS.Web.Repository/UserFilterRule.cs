using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tuhu.MMS.Web.Domain.UserFilter;

namespace Tuhu.MMS.Web.Repository
{
    public class UserFilterRule
    {
        public static async Task<bool> SaveJobDescriptionAsync(int jobid, string description)
        {
            string sql = $"UPDATE Configuration..tbl_UserFilterRuleJob SET Description=N'{description}' WHERE pkid={jobid}";
            using (var helper = DbHelper.CreateDbHelper(false))
            {
                var result = await helper.ExecuteNonQueryAsync(sql);
                return result > 0;
            }
        }

        public static async Task<bool> SaveJobRunStatusAsync(int jobid, JobStatus status)
        {
            string sql = $"UPDATE Configuration..tbl_UserFilterRuleJob SET JobStatus=N'{status}' WHERE pkid={jobid}";
            using (var helper = DbHelper.CreateDbHelper(false))
            {
                var result = await helper.ExecuteNonQueryAsync(sql);
                return result > 0;
            }
        }

        public static async Task<bool> SaveJobRunSqlsAsync(int jobid, string querysql, string resultsql,string outputtables)
        {
            string sql = $"UPDATE Configuration..tbl_UserFilterRuleJob SET QuerySql=@QuerySql , ResultSql=@ResultSql,ResultTables=@ResultTables WHERE pkid={jobid}";
            using (var helper = DbHelper.CreateDbHelper(false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@QuerySql", querysql);
                    cmd.Parameters.AddWithValue("@ResultSql", resultsql);
                    cmd.Parameters.AddWithValue("@ResultTables", outputtables);
                    var result = await helper.ExecuteNonQueryAsync(cmd);
                    return result > 0;
                }

            }
        }

        public static async Task<IEnumerable<UserFilterRuleJob>> SelectUserFilterRuleJobsAsync()
        {
            string sql = @"SELECT * FROM Configuration..tbl_UserFilterRuleJob WITH ( NOLOCK) ";
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                var result = await helper.ExecuteSelectAsync<UserFilterRuleJob>(sql);
                return result;
            }
        }
        public static async Task<UserFilterRuleJob> SelectUserFilterRuleJobsAsync(int jobid)
        {
            string sql = $"SELECT * FROM Configuration..tbl_UserFilterRuleJob WITH ( NOLOCK) where pkid={jobid}";
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                var result = await helper.ExecuteFetchAsync<UserFilterRuleJob>(sql);
                return result;
            }
        }

        public static async Task<int> InsertUserFilterRuleJobAsync(UserFilterRuleJob job)
        {
            string sql = @"INSERT INTO Configuration..tbl_UserFilterRuleJob
        ( JobName ,
          CreateUser ,
          ModifyUser ,
          IsPreview ,
          IsSubmit ,
          JobStatus ,
          PreviewStatus ,
          ResultCount ,
          CreateDateTime ,
          LastUpdateDateTime ,
          LastRunDateTime ,
          Description
        )
VALUES  ( @JobName , -- JobName - nvarchar(100)
          @CreateUser , -- CreateUser - nvarchar(100)
          @ModifyUser , -- ModifyUser - nvarchar(100)
          @IsPreview , -- IsPreview - bit
          @IsSubmit , -- IsSubmit - bit
          @JobStatus , -- JobStatus - nvarchar(100)
          @PreviewStatus , -- PreviewStatus - nvarchar(100)
          @ResultCount , -- ResultCount - int
          GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- LastUpdateDateTime - datetime
          GETDATE() , -- LastRunDateTime - datetime
          @Description  -- Description - nvarchar(500)
        );SELECT  @@IDENTITY; ";
            using (var dbhelper = DbHelper.CreateDbHelper(false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    var props = job.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        object value = prop.GetValue(job);

                        if (prop.Name.Equals("PKID", StringComparison.OrdinalIgnoreCase) ||
                            prop.PropertyType == typeof(DateTime))
                        {
                            continue;
                        }
                        if (prop.PropertyType.IsEnum)
                        {
                            cmd.Parameters.Add(new SqlParameter(string.Concat("@", prop.Name), value.ToString()));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter(string.Concat("@", prop.Name), value));
                        }

                    }
                    var result = await dbhelper.ExecuteScalarAsync(cmd);
                    return Convert.IsDBNull(result) ? 0 : Convert.ToInt32(result);
                }
            }
        }

        public static async Task<int> DeleteFilterRuleJobDetailAsync(string batchid)
        {
            string sql = $" UPDATE Configuration..tbl_UserFilterRuleJobDetail  WITH(ROWLOCK)  SET IsEffective=0 WHERE BatchID='{batchid}' ";
            using (var dbhelper = DbHelper.CreateDbHelper(false))
            {
                return await dbhelper.ExecuteNonQueryAsync(sql);
            }
        }
        public static async Task<int> InsertUserFilterRuleJobDetailAsync(UserFilterRuleJobDetail detail)
        {
            string sql = @"
INSERT Configuration..tbl_UserFilterRuleJobDetail
        ( JobId ,
          TableName ,
          SearchKey ,
          SearchValue ,
          JoinType ,
          BasicAttribute ,
          SecondAttribute ,
          CompareType ,
          CreateDateTime ,
          LastUpdateDateTime,
          BatchID
        )
VALUES  ( @JobId , -- JobId - int
          @TableName , -- TableName - nvarchar(100)
          @SearchKey , -- SearchKey - nvarchar(200)
          @SearchValue , -- SearchValue - nvarchar(500)
          @JoinType , -- JoinType - nvarchar(100)
          @BasicAttribute , -- BasicAttribute - nvarchar(100)
          @SecondAttribute , -- SecondAttribute - nvarchar(100)
          @CompareType , -- CompareType - nvarchar(100)
          GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- LastUpdateDateTime - datetime
          @BatchID
        );SELECT  @@IDENTITY; ";
            using (var dbhelper = DbHelper.CreateDbHelper(false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    var props = detail.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        object value = prop.GetValue(detail);

                        if (prop.Name.Equals("PKID", StringComparison.OrdinalIgnoreCase) ||
                            prop.PropertyType == typeof(DateTime))
                        {
                            continue;
                        }
                        if (prop.PropertyType.IsEnum)
                        {
                            cmd.Parameters.Add(new SqlParameter(string.Concat("@", prop.Name), value.ToString()));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter(string.Concat("@", prop.Name), value));
                        }

                    }
                    var result = await dbhelper.ExecuteScalarAsync(cmd);
                    return Convert.IsDBNull(result) ? 0 : Convert.ToInt32(result);
                }
            }
        }

        public static async Task<int> SelectUserFilterResultConfigCountAsync(int jobid)
        {
            string sql = $"SELECT COUNT(1) FROM Configuration..tbl_UserFilterResultConfig WHERE jobid={jobid} AND IsEffective=1 ";
            using (var dbhelper = DbHelper.CreateDbHelper(false))
            {
                var result = await dbhelper.ExecuteScalarAsync(sql);
                return Convert.IsDBNull(result) ? 0 : Convert.ToInt32(result);
            }
        }
        public static async Task<int> InsertOrUpdateUserFilterResultConfigAsync(UserFilterResultConfig config)
        {
            string sql = @"IF NOT EXISTS ( SELECT  *
                FROM    Configuration..tbl_UserFilterResultConfig
                WHERE   jobid = @JobId
                        AND TableName = @TableName
                        AND BasicAttribute = @BasicAttribute
                        AND ColName = @ColName )
    BEGIN
        INSERT  INTO Configuration..tbl_UserFilterResultConfig
                ( TableName ,
                  BasicAttribute ,
                  ColName ,
                  IsEffective ,
                  JobId ,
                  CreateDateTime ,
                  LastUpdateDateTime
                )
        VALUES  ( @TableName , -- TableName - nvarchar(100)
                  @BasicAttribute , -- BasicAttribute - nvarchar(100)
                  @ColName , -- ColName - nvarchar(100)
                  @IsEffective , -- IsEffective - bit
                  @JobId ,
                  GETDATE() , -- CreateDateTime - datetime
                  GETDATE()  -- LastUpdateDateTime - datetime
                );
            SELECT  @@IDENTITY; 
    END;
ELSE
    BEGIN
        SELECT  @IsEffective=IsEffective 
        FROM    Configuration..tbl_UserFilterResultConfig
        WHERE   jobid = @JobId
                AND TableName = @TableName
                AND BasicAttribute = @BasicAttribute
                AND ColName = @ColName;  
	
        IF ( @IsEffective = 1 )
            BEGIN
                SET @IsEffective = 0;
            END;
        
        ELSE
            BEGIN
                SET @IsEffective = 1;
            END;
        UPDATE  Configuration..tbl_UserFilterResultConfig
        SET     IsEffective = @IsEffective
        WHERE   jobid = @JobId
                AND TableName = @TableName
                AND BasicAttribute = @BasicAttribute
	            AND ColName = @ColName;
    END;";
            using (var dbhelper = DbHelper.CreateDbHelper(false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    var props = config.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        object value = prop.GetValue(config);

                        if (prop.Name.Equals("PKID", StringComparison.OrdinalIgnoreCase) ||
                            prop.PropertyType == typeof(DateTime))
                        {
                            continue;
                        }
                        if (prop.PropertyType.IsEnum)
                        {
                            cmd.Parameters.Add(new SqlParameter(string.Concat("@", prop.Name), value.ToString()));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter(string.Concat("@", prop.Name), value));
                        }

                    }
                    var result = await dbhelper.ExecuteNonQueryAsync(cmd);
                    return result;
                }
            }
        }
        public static async Task<int> InsertUserFilterResultConfigAsync(UserFilterResultConfig config)
        {
            string sql = @" 
INSERT INTO Configuration..tbl_UserFilterResultConfig
        ( TableName ,
          BasicAttribute ,
          ColName ,
          IsEffective ,
          JobId ,
          CreateDateTime ,
          LastUpdateDateTime
        )
VALUES  ( @TableName , -- TableName - nvarchar(100)
          @BasicAttribute , -- BasicAttribute - nvarchar(100)
          @ColName , -- ColName - nvarchar(100)
          @IsEffective , -- IsEffective - bit
          @JobId , 
          GETDATE() , -- CreateDateTime - datetime
          GETDATE()  -- LastUpdateDateTime - datetime
        );SELECT  @@IDENTITY; ";
            using (var dbhelper = DbHelper.CreateDbHelper(false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    var props = config.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        object value = prop.GetValue(config);

                        if (prop.Name.Equals("PKID", StringComparison.OrdinalIgnoreCase) ||
                            prop.PropertyType == typeof(DateTime))
                        {
                            continue;
                        }
                        if (prop.PropertyType.IsEnum)
                        {
                            cmd.Parameters.Add(new SqlParameter(string.Concat("@", prop.Name), value.ToString()));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter(string.Concat("@", prop.Name), value));
                        }

                    }
                    var result = await dbhelper.ExecuteScalarAsync(cmd);
                    return Convert.IsDBNull(result) ? 0 : Convert.ToInt32(result);
                }
            }
        }

        public static async Task<IEnumerable<UserFilterRuleJobDetail>> SelectUserFilterRuleJobDetailsAsync(int jobid)
        {
            string sql =
                $"SELECT * FROM Configuration..tbl_UserFilterRuleJobDetail WITH ( NOLOCK) WHERE JobId={jobid} and IsEffective=1 order by pkid desc ";
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                var result = await helper.ExecuteSelectAsync<UserFilterRuleJobDetail>(sql);
                return result;
            }
        }

        public static async Task<IEnumerable<UserFilterResultConfig>> SelectUserFilterResultConfigAsync(int jobid)
        {
            string sql =
                $"SELECT * FROM Configuration..tbl_UserFilterResultConfig WITH ( NOLOCK) WHERE JobId={jobid} and IsEffective=1 order by pkid desc ";
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                var result = await helper.ExecuteSelectAsync<UserFilterResultConfig>(sql);
                return result;
            }
        }

        public static async Task<IEnumerable<UserFilterValueConfig>> SelectAllUserFilterValueConfigAsync()
        {
            string sql = $"SELECT   * FROM tuhu_bi..tbl_UserFilterValueConfig WITH ( NOLOCK) WHERE  value is not null and value <> '' ";
            using (var helper = DbHelper.CreateDbHelper("Tuhu_BI"))
            {
                var result = await helper.ExecuteSelectAsync<UserFilterValueConfig>(sql);
                return result;
            }
        }
        public static async Task<IEnumerable<UserFilterValueConfig>> SelectUserFilterValueConfigByColNameAsync(string colname)
        {
            string sql = $"SELECT   * FROM tuhu_bi..tbl_UserFilterValueConfig WITH ( NOLOCK) WHERE colname='{colname}'  and value is not null and value <> '' ";
            using (var helper = DbHelper.CreateDbHelper("Tuhu_BI"))
            {
                var result = await helper.ExecuteSelectAsync<UserFilterValueConfig>(sql);
                return result;
            }
        }
        public static async Task<IEnumerable<UserFilterValueConfig>> SelectUserFilterValueConfigByParentValueAsync(string parentvalue)
        {
            string sql = $"SELECT   * FROM tuhu_bi..tbl_UserFilterValueConfig WITH ( NOLOCK) WHERE parentvalue='{parentvalue}' and value is not null and value <> '' ";
            using (var helper = DbHelper.CreateDbHelper("Tuhu_BI"))
            {
                var result = await helper.ExecuteSelectAsync<UserFilterValueConfig>(sql);
                return result;
            }
        }
    }
}
