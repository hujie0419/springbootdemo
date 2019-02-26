using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalBeautyCode
    {
        public static IEnumerable<BeautyServicePackageSimpleModel> SelectPackagesByPackageType(SqlConnection conn, string packageType)
        {
            var sql = @"SELECT  p.PKID ,
        p.PackageName
FROM    Tuhu_groupon..BeautyServicePackage AS p WITH ( NOLOCK )
WHERE   p.PackageType = @PackageType;";
            var parameters = new[]
            {
                new SqlParameter("@PackageType", packageType),
            };
            var dt = SqlHelper.ExecuteDataTable(conn, System.Data.CommandType.Text, sql, parameters);
            var result = dt.Rows.Cast<System.Data.DataRow>().Select(x => new BeautyServicePackageSimpleModel
            {
                PKID = (int)x["PKID"],
                PackageName = x["PackageName"].ToString(),
            });
            return result;
        }

        public static bool BatchAddBeautyCode(SqlConnection conn, DataTable table)
        {
            using (var sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = table.TableName;
                foreach (DataColumn cloumn in table.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(cloumn.ColumnName, cloumn.ColumnName);
                }
                sqlBulkCopy.WriteToServer(table);
                return true;
            }
        }

        public static IEnumerable<BeautyServicePackageDetailSimpleModel> SelectProductsByPackageId(SqlConnection conn, int packageId)
        {
            var sql = @"SELECT  d.PKID ,
        d.Name
FROM    Tuhu_groupon..BeautyServicePackageDetail AS d WITH ( NOLOCK )
WHERE   d.PackageId = @PackageId
        AND d.IsImportUser = 1;";
            var parameters = new[]
            {
                new SqlParameter("@PackageId", packageId),
            };
            var dt = SqlHelper.ExecuteDataTable(conn, System.Data.CommandType.Text, sql, parameters);
            var result = dt.Rows.Cast<System.Data.DataRow>().Select(x => new BeautyServicePackageDetailSimpleModel
            {
                PKID = (int)x["PKID"],
                Name = x["Name"].ToString(),
            });
            return result;
        }

        public static bool UpdateBeautyCodeStatus(SqlConnection conn, string batchCode, string status)
        {
            var sql = @"UPDATE  Tuhu_groupon..CreateBeautyCodeTask
SET     Status = @Status
WHERE   BatchCode = @BatchCode;";
            var parameters = new[]
            {
                new SqlParameter("@BatchCode", batchCode),
                new SqlParameter("@Status", status)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
        /// <summary>
        /// 根据服务码配置Ids分页查询上传用户批次号
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="mappingIds"></param>
        /// <returns></returns>
        public static Tuple<List<string>, int> SelectBatchCodesByMappingIds(SqlConnection conn, int pageIndex, int pageSize, IEnumerable<int> mappingIds)
        {
            var result = new List<string>();
            var totalCount = 0;
            var sql = @"SELECT DISTINCT
        A.BatchCode
FROM    Tuhu_groupon..CreateBeautyCodeTask AS A WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@MappingIds, ',', 1) AS B ON A.MappingId = B.Item
ORDER BY A.BatchCode DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;
SELECT  @TotalCount = COUNT(DISTINCT A.BatchCode)
FROM    Tuhu_groupon..CreateBeautyCodeTask AS A WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@MappingIds, ',', 1) AS B ON A.MappingId = B.Item;";
            var parameters = new[]
            {
                new SqlParameter("@MappingIds", string.Join(",",mappingIds)),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@TotalCount",SqlDbType.Int){ Direction=ParameterDirection.Output}
            };

            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var item = row.IsNull("BatchCode") ? null : row["BatchCode"].ToString();
                    if (!string.IsNullOrEmpty(item))
                        result.Add(item);
                }
                 totalCount = Convert.ToInt32(parameters.Last().Value);
            }

            return new Tuple<List<string>, int>(result, totalCount);
        }
        /// <summary>
        /// 分页查询上传用户批次号
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<string> SelectBatchCodes(SqlConnection conn, int pageIndex, int pageSize)
        {
            var result = new List<string>();
            var sql = @"SELECT  A.BatchCode
FROM    Tuhu_groupon..CreateBeautyCodeTask AS A WITH ( NOLOCK )
ORDER BY A.BatchCode
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            var parameters = new[]
            {
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize)
            };

            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var item = row.IsNull("BatchCode") ? null : row["BatchCode"].ToString();
                    if (!string.IsNullOrEmpty(item))
                        result.Add(item);
                }
            }

            return result;
        }

        public static List<BeautyCodeStatistics> SelectBeautyCodeStatisticsTotalCount(SqlConnection conn, IEnumerable<string> batchCodes)
        {
            var sql = @"SELECT  t.BatchCode ,
        t.MappingId ,
        t.CreateUser ,
        t.Type ,
        t.BuyoutOrderId ,
        COUNT(1) AS TotalCount
FROM    Tuhu_groupon..CreateBeautyCodeTask AS t WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@BatchCodes, ',', 1) AS c ON c.Item = t.BatchCode
GROUP BY t.BatchCode ,
        t.MappingId ,
        t.Type ,
        t.CreateUser,
		t.BuyoutOrderId;";
            var parameters = new[]
            {
                new SqlParameter("@BatchCodes", string.Join(",",batchCodes))
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            var result = dt.Rows.Cast<DataRow>().Select(row => new BeautyCodeStatistics
            {
                BatchCode = row["BatchCode"].ToString(),
                MappingId = (int)row["MappingId"],
                TotalCount = (int)row["TotalCount"],
                CreateUser = row["CreateUser"].ToString(),
                Type = row["Type"].ToString(),
                BuyoutOrderId = row.IsNull("BuyoutOrderId") ? 0 : Convert.ToInt32(row["BuyoutOrderId"])
            }).ToList();
            return result;
        }

        public static List<BeautyCodeStatistics> SelectBeautyCodeBatchTaskStatus(SqlConnection conn, IEnumerable<string> batchCodes)
        {
            var sql = @"SELECT DISTINCT
        R.BatchCode ,
        R.Status
FROM    Tuhu_groupon..CreateBeautyCodeTask AS R WITH ( NOLOCK )
        JOIN ( SELECT   A.BatchCode
               FROM     Tuhu_groupon..CreateBeautyCodeTask AS A WITH ( NOLOCK )
                        JOIN Tuhu_groupon..SplitString(@BatchCodes, ',', 1) AS B ON B.Item = A.BatchCode
               GROUP BY A.BatchCode
               HAVING   COUNT(DISTINCT A.Status) = 1
             ) AS T ON T.BatchCode = R.BatchCode;";
            var parameters = new[]
            {
                new SqlParameter("@BatchCodes", string.Join(",",batchCodes))
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            var result = dt.Rows.Cast<DataRow>().Select(row => new BeautyCodeStatistics
            {
                BatchCode = row["BatchCode"].ToString(),
                Status = row.IsNull("Status") ? null : row["Status"].ToString()
            }).ToList();
            return result;
        }

        public static List<BeautyCodeStatistics> SelectBeautyCodeStatisticsCount(SqlConnection conn, IEnumerable<string> batchCodes)
        {
            var sql = @"SELECT  t.BatchCode ,
        t.BuyoutOrderId ,
        t.MappingId ,
        t.CreateUser ,
        t.Type ,
        COUNT(1) AS Count
FROM    Tuhu_groupon..CreateBeautyCodeTask AS t WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@BatchCodes, ',', 1) AS c ON c.Item = t.BatchCode
WHERE   t.Status = N'Completed'
GROUP BY t.BatchCode ,
        t.BuyoutOrderId ,
        t.MappingId ,
        t.Type ,
        t.CreateUser;";
            var parameters = new[]
            {
                new SqlParameter("@BatchCodes", string.Join(",",batchCodes))
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            var result = dt.Rows.Cast<DataRow>().Select(row => new BeautyCodeStatistics
            {
                BatchCode = row["BatchCode"].ToString(),
                MappingId = (int)row["MappingId"],
                Count = (int)row["Count"],
                CreateUser = row["CreateUser"].ToString(),
                Type = row["Type"].ToString(),
                BuyoutOrderId = row.IsNull("BuyoutOrderId") ? 0 : Convert.ToInt32(row["BuyoutOrderId"])
            }).ToList();
            return result;
        }

        public static IEnumerable<CompanyUserSmsRecord> SelectCompanyUserSmsRecordByBatchCode(SqlConnection conn, string batchCode)
        {
            IEnumerable<CompanyUserSmsRecord> result = null;
            var sql = @"SELECT  A.PKID ,
        A.Type ,
        A.BatchCode ,
        A.SmsTemplateId ,
        A.SmsMsg ,
        A.SentTime ,
        A.Status ,
        A.CreatedUser ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_groupon..CompanyUserSmsRecord AS A WITH ( NOLOCK )
WHERE   A.BatchCode = @BatchCode
ORDER BY PKID DESC ";
            var parameters = new[]
            {
                new SqlParameter("@BatchCode", batchCode)
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            if (dt != null && dt.Rows != null)
            {
                result = dt.Rows.Cast<DataRow>().Select(row => new CompanyUserSmsRecord
                {
                    PKID = Convert.ToInt32(row["PKID"]),
                    Type = row["Type"].ToString(),
                    BatchCode = row["BatchCode"].ToString(),
                    SmsTemplateId = Convert.ToInt32(row["SmsTemplateId"]),
                    SmsMsg = row["SmsMsg"].ToString(),
                    SentTime = Convert.ToDateTime(row["SentTime"]),
                    Status = row["Status"].ToString(),
                    CreatedUser = row["CreatedUser"].ToString(),
                    CreatedDateTime = Convert.ToDateTime(row["CreatedDateTime"]),
                    UpdatedDateTime = Convert.ToDateTime(row["UpdatedDateTime"])
                }).ToList();
            }

            return result ?? new List<CompanyUserSmsRecord>();
        }

        public static bool InsertCompanyUserSmsRecord(SqlConnection conn, CompanyUserSmsRecord record)
        {
            var sql = @"INSERT  Tuhu_groupon..CompanyUserSmsRecord
        ( Type ,
          BatchCode ,
          SmsTemplateId ,
          SmsMsg ,
          SentTime ,
          Status ,
          CreatedUser 
        )
VALUES  ( @Type ,
          @BatchCode ,
          @SmsTemplateId ,
          @SmsMsg ,
          @SentTime ,
          @Status ,
          @CreatedUser 
        )";
            var parameters = new[]
            {
                new SqlParameter("@Type", record.Type),
                new SqlParameter("@BatchCode", record.BatchCode),
                new SqlParameter("@SmsTemplateId", record.SmsTemplateId),
                new SqlParameter("@SmsMsg", record.SmsMsg),
                new SqlParameter("@SentTime", record.SentTime),
                new SqlParameter("@Status", record.Status),
                new SqlParameter("@CreatedUser", record.CreatedUser)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
    }
}
