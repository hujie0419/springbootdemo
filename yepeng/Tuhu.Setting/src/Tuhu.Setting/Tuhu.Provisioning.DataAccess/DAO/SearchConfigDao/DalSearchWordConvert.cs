using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO.SearchConfigDao
{
    public static class DalSearchWordConvert
    {
        public static List<SearchWordConvertMapDb> GetAllSearchWord(SqlConnection conn)
        {
            const string sql = @"
                        SELECT ss.PKID,
                        ss.TargetWord,
                        ss.SourceWord,
                        ss.CreateTime
                        FROM   Tuhu_productcatalog.dbo.tbl_SearchWordConvertMap AS ss WITH (NOLOCK)
                    ";

            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<SearchWordConvertMapDb>().ToList();
            return resultList;
        }

        public static bool DeleteSearchWord(SqlConnection conn, List<SearchWordConvertMapDb> delList)
        {
            var sqlParams = new List<SqlParameter>();
            var strSql = new StringBuilder();
            for (var i = 0; i < delList.Count; i++)
            {
                sqlParams.Add(new SqlParameter($"@PKID{i}", delList[i].PKID));
                strSql.AppendFormat(" DELETE FROM tbl_SearchWordConvertMap  Where PKID = @PKID{0} ; ", i);
            }
            if (strSql.Length > 0)
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
            }
            return true;
        }

        public static bool BulkSaveSearchWordInfo(SqlConnection conn, DataTable tb)
        {
            using (var bulk = new SqlBulkCopy(conn))
            {

                bulk.BatchSize = tb.Rows.Count;
                bulk.DestinationTableName = "tbl_SearchWordConvertMap";

                bulk.ColumnMappings.Add("TargetWord", "TargetWord");
                bulk.ColumnMappings.Add("SourceWord", "SourceWord");
                bulk.ColumnMappings.Add("CreateTime", "CreateTime");

                bulk.WriteToServer(tb);

            }
            return true;
        }

    }
}
