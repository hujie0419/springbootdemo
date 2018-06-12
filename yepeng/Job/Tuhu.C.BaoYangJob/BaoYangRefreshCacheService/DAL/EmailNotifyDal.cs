using BaoYangRefreshCacheService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.DAL
{
    public static class EmailNotifyDal
    {
        private static readonly string connString = ConfigurationManager.ConnectionStrings["Tuhu_Log_ReadOnly"].ConnectionString;
        public static List<PartsNotifyReportConfig> SelectPartsNotifyReportConfig()
        {
            var sql = @"SELECT  PKID ,
        ReportType ,
        Frequency ,
        ReportName ,
        ReportUrl ,
        AuthSecret ,
        NotifyUsers ,
        CreateTime ,
        UpdateTime
FROM    BaoYang..PartsNotifyReportConfig WITH ( NOLOCK )";
            var result = Tuhu.DbHelper.ExecuteSelect<PartsNotifyReportConfig>(true, sql, CommandType.Text).ToList();
            return result;
        }

        public static DateTime? GetRencentlyExecuteTime(string reportType)
        {
            var para = new SqlParameter("@ReportType", reportType);
            var sql = @"SELECT TOP 1
        ReportTime
FROM    PartsNotifyReportHistory
WHERE   ReportType = @ReportType
ORDER BY CreateTime DESC;";

            using (var dbHelper = Tuhu.DbHelper.CreateDbHelper(connString))
            {
                var result = (DateTime?)dbHelper.ExecuteScalar(sql, CommandType.Text, para);
                return result;
            }
        }
    }
}
