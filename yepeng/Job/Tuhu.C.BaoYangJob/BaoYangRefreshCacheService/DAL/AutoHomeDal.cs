using BaoYangRefreshCacheService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu;

namespace BaoYangRefreshCacheService.DAL
{
    public class AutoHomeDal
    {
        private const string dbPrefix = "Tuhu_thirdparty.dbo.{0}";

        private static readonly string connStr = ConfigurationManager.ConnectionStrings["Tuhu_thirdparty"].ConnectionString;

        private static readonly string connStrRo = ConfigurationManager.ConnectionStrings["Tuhu_thirdparty_ro"].ConnectionString;

        private static async Task<bool> Insert(DataTable table)
        {
            using (var copy = new SqlBulkCopy(connStr, SqlBulkCopyOptions.UseInternalTransaction))
            {
                copy.DestinationTableName = table.TableName;
                foreach (DataColumn column in table.Columns)
                {
                    copy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                await copy.WriteToServerAsync(table);
                return true;
            }
        }

        #region Convert To DataTable

        private static DataTable CarModelGradesConvertToDataTable(List<AutoHomeCarModelGrade> gradeList, string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add(new DataColumn(nameof(AutoHomeCarModelGrade.PKID), typeof(long)) { AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 });
            dt.Columns.Add(nameof(AutoHomeCarModelGrade.GradeId), typeof(int));
            dt.Columns.Add(nameof(AutoHomeCarModelGrade.Brand), typeof(string));
            dt.Columns.Add(nameof(AutoHomeCarModelGrade.CarSeries), typeof(string));
            dt.Columns.Add(nameof(AutoHomeCarModelGrade.CarGrade), typeof(string));
            dt.Columns.Add(nameof(AutoHomeCarModelGrade.Url), typeof(string));
            dt.Columns.Add(nameof(AutoHomeCarModelGrade.BatchNo), typeof(long));
            gradeList.ForEach(x =>
            {
                var row = dt.NewRow();
                row[nameof(AutoHomeCarModelGrade.GradeId)] = x.GradeId;
                row[nameof(AutoHomeCarModelGrade.Brand)] = x.Brand;
                row[nameof(AutoHomeCarModelGrade.CarSeries)] = x.CarSeries;
                row[nameof(AutoHomeCarModelGrade.CarGrade)] = x.CarGrade;
                row[nameof(AutoHomeCarModelGrade.Url)] = x.Url;
                row[nameof(AutoHomeCarModelGrade.BatchNo)] = x.BatchNo;
                dt.Rows.Add(row);
            });
            return dt;
        }

        private static DataTable CarModelInfosConvertToDataTable(List<AutoHomeCarModelInfo> infoList, string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add(new DataColumn(nameof(AutoHomeCarModelInfo.PKID), typeof(long)) { AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 });
            dt.Columns.Add(nameof(AutoHomeCarModelInfo.GradeId), typeof(int));
            dt.Columns.Add(nameof(AutoHomeCarModelInfo.CarModelId), typeof(int));
            dt.Columns.Add(nameof(AutoHomeCarModelInfo.CarModelName), typeof(string));
            dt.Columns.Add(nameof(AutoHomeCarModelInfo.Url), typeof(string));
            dt.Columns.Add(nameof(AutoHomeCarModelInfo.BatchNo), typeof(long));
            infoList.ForEach(x =>
            {
                var row = dt.NewRow();
                row[nameof(AutoHomeCarModelInfo.GradeId)] = x.GradeId;
                row[nameof(AutoHomeCarModelInfo.CarModelId)] = x.CarModelId;
                row[nameof(AutoHomeCarModelInfo.CarModelName)] = x.CarModelName;
                row[nameof(AutoHomeCarModelInfo.Url)] = x.Url;
                row[nameof(AutoHomeCarModelInfo.BatchNo)] = x.BatchNo;
                dt.Rows.Add(row);
            });
            return dt;
        }

        private static DataTable CarModelParamsConvertToDataTable(List<AutoHomeCarModelParam> paramList, string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add(new DataColumn(nameof(AutoHomeCarModelParam.PKID), typeof(long)) { AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 });
            dt.Columns.Add(nameof(AutoHomeCarModelParam.CarModelId), typeof(int));
            dt.Columns.Add(nameof(AutoHomeCarModelParam.ParamName), typeof(string));
            dt.Columns.Add(nameof(AutoHomeCarModelParam.ParamValue), typeof(string));
            dt.Columns.Add(nameof(AutoHomeCarModelParam.BatchNo), typeof(long));
            paramList.ForEach(x =>
            {
                var row = dt.NewRow();
                row[nameof(AutoHomeCarModelParam.CarModelId)] = x.CarModelId;
                row[nameof(AutoHomeCarModelParam.ParamName)] = x.ParamName;
                row[nameof(AutoHomeCarModelParam.ParamValue)] = x.ParamValue;
                row[nameof(AutoHomeCarModelParam.BatchNo)] = x.BatchNo;
                dt.Rows.Add(row);
            });
            return dt;
        }

        #endregion

        public static async Task<bool> BatchInsertCarModelGrade(IEnumerable<AutoHomeCarModelGrade> grades)
        {
            var dt = CarModelGradesConvertToDataTable(grades.ToList(), string.Format(dbPrefix, nameof(AutoHomeCarModelGrade)));
            return await Insert(dt);
        }

        public static async Task<bool> BatchInsertCarModelInfo(IEnumerable<AutoHomeCarModelInfo> infos)
        {
            var dt = CarModelInfosConvertToDataTable(infos.ToList(), string.Format(dbPrefix, nameof(AutoHomeCarModelInfo)));
            return await Insert(dt);
        }

        public static async Task<bool> BatchInsertCarModelParam(IEnumerable<AutoHomeCarModelParam> paramters)
        {
            var dt = CarModelParamsConvertToDataTable(paramters.ToList(), string.Format(dbPrefix, nameof(AutoHomeCarModelParam)));
            return await Insert(dt);
        }

        public static async Task<List<AutoHomeCarModelGrade>> SelectCarModelGrades(int pageIndex, int pageSize)
        {
            using(var dbHelper = DbHelper.CreateDbHelper(connStrRo))
            using (var cmd = new SqlCommand(@"
SELECT  t.PKID ,
        t.GradeId ,
        t.Brand ,
        t.CarSeries ,
        t.CarGrade ,
        t.Url ,
        t.BatchNo
FROM    Tuhu_thirdparty..AutoHomeCarModelGrade AS t WITH ( NOLOCK )
WHERE   t.BatchNo = ( SELECT MAX(tInner.BatchNo)
                         FROM   Tuhu_thirdparty..AutoHomeCarModelGrade AS tInner WITH ( NOLOCK )
                       )
        AND t.EndTime IS NULL
ORDER BY t.GradeId
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;"))
            {
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                var result = await dbHelper.ExecuteSelectAsync<AutoHomeCarModelGrade>(cmd);
                return result.ToList();
            }
        }

        public static async Task<bool> UpdateCarModelGradeStartTime(int pkid)
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connStr))
            using (var cmd = new SqlCommand(@"
UPDATE  Tuhu_thirdparty..AutoHomeCarModelGrade
SET     StartTime = GETDATE()
WHERE   PKID = @PKID;"))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return await dbHelper.ExecuteNonQueryAsync(cmd) > 0;
            }
        }

        public static async Task<bool> UpdateCarModelGradeEndTime(int pkid)
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connStr))
            using (var cmd = new SqlCommand(@"
UPDATE  Tuhu_thirdparty..AutoHomeCarModelGrade
SET     EndTime = GETDATE()
WHERE   PKID = @PKID;"))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return await dbHelper.ExecuteNonQueryAsync(cmd) > 0;
            }
        }

    }
}
