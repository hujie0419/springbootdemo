using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.ApplicationBlocks.Data;

using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO.SearchConfigDao
{
    public static class DalSearchWordConvert
    {
        public static List<SearchWordConvertMapDb> GetAllSearchWord(SqlConnection conn, SearchWordConfigType configType)
        {
            string sql = "";
            if (configType == SearchWordConfigType.Config)
            {
                sql = @"
                        SELECT ss.PKID,
                        ss.TargetWord,
                        ss.SourceWord,
                        ss.CreateTime,
                        ss.Tag,
                        ss.UpdateBy
                        FROM   Tuhu_productcatalog.dbo.tbl_SearchWordConvertMap AS ss WITH (NOLOCK)
                        ORDER BY UpdateTime DESC";
            }
            else if (configType == SearchWordConfigType.VehicleType)
            {
                sql = @"SELECT  ss.PKID ,
                        ss.TargetWord ,
                        ss.SourceWord ,
                        ss.CreateDateTime AS CreateTime ,
                        0 AS Tag ,
                        ss.VehicleID ,
                        ss.Sort ,
                        ss.TireSize ,
                        ss.SpecialTireSize,
                        ss.VehicleName
                FROM    Tuhu_productcatalog..SearchWordConvertMap_Vehicle AS ss WITH ( NOLOCK ); ";
            }
            else if (configType == SearchWordConfigType.NewWord)
            {
                sql = @"SELECT  PKID ,
                                KeyWord AS SourceWord ,
                                '' AS TargetWord ,
                                CreateDateTime AS CreateTime ,
                                0 AS Tag
                        FROM    Tuhu_productcatalog..SearchNewWordConvertMap WITH ( NOLOCK ); ";
            }
            if (!string.IsNullOrEmpty(sql))
            {
                var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<SearchWordConvertMapDb>().ToList();
                return resultList;
            }
            return new List<SearchWordConvertMapDb>();
        }

        public static bool DeleteSearchWord(SqlConnection conn, List<SearchWordConvertMapDb> delList, SearchWordConfigType configType)
        {
            string tablename = "";
            if (configType == SearchWordConfigType.Config)
            {
                tablename = "tbl_SearchWordConvertMap";
            }
            else if (configType == SearchWordConfigType.VehicleType)
            {
                tablename = "SearchWordConvertMap_Vehicle";
            }
            else if (configType == SearchWordConfigType.NewWord)
            {
                tablename = "SearchNewWordConvertMap";
            }
            if (!string.IsNullOrEmpty(tablename))
            {
                var sqlParams = new List<SqlParameter>();
                var strSql = new StringBuilder();
                for (var i = 0; i < delList.Count; i++)
                {
                    strSql.AppendFormat($" DELETE FROM {tablename}  Where PKID = {delList[i].PKID} ; ");
                }
                if (strSql.Length > 0)
                {
                    SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
                }
                return true;
            }
            return false;
        }

        public static bool BulkSaveSearchWordInfo(SqlConnection conn, DataTable tb, SearchWordConfigType configType)
        {
            using (var bulk = new SqlBulkCopy(conn))
            {

                bulk.BatchSize = tb.Rows.Count;
                if (configType == SearchWordConfigType.Config)
                {
                    bulk.DestinationTableName = "tbl_SearchWordConvertMap";

                    bulk.ColumnMappings.Add("TargetWord", "TargetWord");
                    bulk.ColumnMappings.Add("SourceWord", "SourceWord");
                    bulk.ColumnMappings.Add("CreateTime", "CreateTime");
                    bulk.ColumnMappings.Add("Tag", "Tag");
                    bulk.ColumnMappings.Add("UpdateBy", "UpdateBy");
                    bulk.ColumnMappings.Add("UpdateTime", "UpdateTime");
                    bulk.WriteToServer(tb);
                }
                else if (configType == SearchWordConfigType.VehicleType)
                {
                    bulk.DestinationTableName = "SearchWordConvertMap_Vehicle";
                    bulk.ColumnMappings.Add("TargetWord", "TargetWord");
                    bulk.ColumnMappings.Add("SourceWord", "SourceWord");
                    bulk.ColumnMappings.Add("CreateTime", "CreateDateTime");
                    bulk.ColumnMappings.Add("Type", "Type");
                    bulk.ColumnMappings.Add("VehicleID", "VehicleID");
                    bulk.ColumnMappings.Add("Sort", "Sort");
                    bulk.ColumnMappings.Add("TireSize", "TireSize");
                    bulk.ColumnMappings.Add("SpecialTireSize", "SpecialTireSize");
                    bulk.ColumnMappings.Add("VehicleName", "VehicleName");
                    bulk.WriteToServer(tb);
                }
                else if (configType == SearchWordConfigType.NewWord)
                {
                    bulk.DestinationTableName = "SearchNewWordConvertMap";
                    bulk.ColumnMappings.Add("TargetWord", "KeyWord");
                  
                    bulk.ColumnMappings.Add("CreateTime", "CreateDateTime");
                    bulk.WriteToServer(tb);
                }

            }
            return true;
        }

        public static bool UpdateSearchWord(SqlConnection conn, List<SearchWordConvertMapDb> list,
            SearchWordConfigType configType)
        {
            string tablename = "";
            if (configType == SearchWordConfigType.Config)
            {
                tablename = "Tuhu_productcatalog.dbo.tbl_SearchWordConvertMap";
            }
            else if (configType == SearchWordConfigType.VehicleType)
            {
                tablename = "Tuhu_productcatalog.dbo.SearchWordConvertMap_Vehicle";
            }
            else if (configType == SearchWordConfigType.NewWord)
            {
                tablename = "Tuhu_productcatalog.dbo.SearchNewWordConvertMap";
            }
            if (!string.IsNullOrEmpty(tablename))
            {
                var sqlParams = new List<SqlParameter>();
                var strSql = new StringBuilder();
                foreach (var item in list)
                {
                    strSql.AppendFormat($@"IF EXISTS ( SELECT  COUNT(0)
                                                    FROM    {tablename} WITH ( NOLOCK )
                                                    WHERE   PKID = {item.PKID}
                                                            OR (UpdateBy = '{item.UpdateBy}' OR ISNULL(UpdateBy, '') = '') )
                                                UPDATE  c
                                                SET     c.UpdateBy = '{item.UpdateBy}'
                                                FROM    {tablename} c
                                                WHERE   PKID = {item.PKID};");
                }
                if (strSql.Length > 0)
                {
                    SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql.ToString(), sqlParams.ToArray());
                }
                return true;
            }
            return false;
        }
    }
}
