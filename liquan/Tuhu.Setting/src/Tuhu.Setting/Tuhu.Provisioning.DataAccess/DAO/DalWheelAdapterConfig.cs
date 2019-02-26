using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalWheelAdapterConfig
    {
        public static List<Str> SelectBrands(SqlConnection conn)
        {
            string sql = @"SELECT DISTINCT VT.Brand as str FROM Gungnir..tbl_Vehicle_Type AS VT with(nolock)";
            var sqlParam = new SqlParameter[] { };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<Str>().ToList();
        }
        public static List<Str> SelectVehiclesAndId(SqlConnection conn, string brand)
        {
            string sql = @"SELECT DISTINCT VT.Vehicle as str,VT.ProductID as str1 FROM Gungnir..tbl_Vehicle_Type AS VT with(nolock) WHERE VT.Brand=@brand";
            var sqlParam = new[]
                {
                    new SqlParameter("@brand",brand),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<Str>().ToList();
        }
        public static List<Str> SelectPaiLiang(SqlConnection conn, string vehicleid)
        {
            string sql = @"SELECT DISTINCT VTM.PaiLiang as str FROM Gungnir..tbl_Vehicle_Type_Timing AS VTM with(nolock) WHERE VTM.VehicleID=@vehicleid";
            var sqlParam = new[]
                {
                    new SqlParameter("@vehicleid",vehicleid),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<Str>().ToList();
        }
        public static List<Str> SelectYear(SqlConnection conn, string vehicleid, string pailiang)
        {
            string sql = @"SELECT VTM.ListedYear as str,VTM.StopProductionYear as str1 FROM Gungnir..tbl_Vehicle_Type_Timing AS VTM with(nolock) WHERE VTM.VehicleID=@vehicleid AND VTM.PaiLiang=@pailiang";
            var sqlParam = new[]
                {
                    new SqlParameter("@vehicleid",vehicleid),
                    new SqlParameter("@pailiang",pailiang),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<Str>().ToList();
        }
        public static List<Str> SelectNianAndSalesName(SqlConnection conn, string vehicleid, string pailiang, string year)
        {
            string sql = @"SELECT VTM.TID as str, VTM.Nian as str1,VTM.SalesName as str2 FROM Gungnir..tbl_Vehicle_Type_Timing AS VTM with(nolock)
                WHERE VTM.VehicleID=@vehicleid AND VTM.PaiLiang=@pailiang AND VTM.ListedYear<=@year AND (VTM.StopProductionYear>=@year OR VTM.StopProductionYear IS NULL)";
            var sqlParam = new[]
                {
                    new SqlParameter("@vehicleid",vehicleid),
                    new SqlParameter("@pailiang",pailiang),
                    new SqlParameter("@year",year),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<Str>().ToList();
        }
        public static List<VehicleTypeInfo> QueryVehicleTypeInfo(SqlConnection conn, WheelAdapterConfigQuery query)
        {
            string sqlcount = @"SELECT Count(1)
                FROM Gungnir..tbl_Vehicle_Type AS VT with(nolock) INNER JOIN Gungnir..tbl_Vehicle_Type_Timing AS VTM with(nolock)
                ON VT.ProductID=VTM.VehicleID
                AND(@brand IS NULL OR VT.Brand=@brand)
                AND(@vehicleid IS NULL OR VTM.VehicleID=@vehicleid)
                AND(@pailiang IS NULL OR VTM.PaiLiang=@pailiang)
                AND(@year IS NULL OR VTM.ListedYear<=@year)
                AND(@year IS NULL OR VTM.StopProductionYear>=@year OR VTM.StopProductionYear IS NULL)
                AND(@tid IS NULL OR VTM.TID=@tid)
                LEFT JOIN Configuration..WheelAdapterConfigWithTid AS WAC with(nolock) ON WAC.TID COLLATE Chinese_PRC_CI_AS = VTM.TID"
                + (query.IsInfoSpecified.GetValueOrDefault() == 2 ? @" WHERE WAC.TID IS NULL" : 
                (query.IsInfoSpecified.GetValueOrDefault() == 1 ? @" WHERE WAC.TID IS NOT NULL" : @""));
            string sql = @"SELECT VTM.PKID,VTM.TID,VT.Brand,VT.Vehicle,VTM.PaiLiang,VTM.Nian,VTM.SalesName,WAC.PCD,WAC.ET,
                WAC.MinET,WAC.MaxET,WAC.CB,WAC.BoltNutSpec,WAC.BoltNut,WAC.MinWheelSize,WAC.MaxWheelSize,WAC.MinWheelWidth,WAC.MaxWheelWidth
                FROM Gungnir..tbl_Vehicle_Type AS VT with(nolock) INNER JOIN Gungnir..tbl_Vehicle_Type_Timing AS VTM with(nolock)
                ON VT.ProductID=VTM.VehicleID
                AND(@brand IS NULL OR VT.Brand=@brand)
                AND(@vehicleid IS NULL OR VTM.VehicleID=@vehicleid)
                AND(@pailiang IS NULL OR VTM.PaiLiang=@pailiang)
                AND(@year IS NULL OR VTM.ListedYear<=@year)
                AND(@year IS NULL OR VTM.StopProductionYear>=@year OR VTM.StopProductionYear IS NULL)
                AND(@tid IS NULL OR VTM.TID=@tid)
                LEFT JOIN Configuration..WheelAdapterConfigWithTid AS WAC with(nolock) ON WAC.TID COLLATE Chinese_PRC_CI_AS = VTM.TID "
                + (query.IsInfoSpecified.GetValueOrDefault() == 2 ? @"WHERE WAC.TID IS NULL " : 
                (query.IsInfoSpecified.GetValueOrDefault() == 1 ? @"WHERE WAC.TID IS NOT NULL " : @"")) +
                @"ORDER BY VT.Brand ASC OFFSET @pagesdata ROWS FETCH NEXT @pagedataquantity ROWS ONLY";
            var sqlParam = new[]
                {
                    new SqlParameter("@brand",query.BrandCriterion),
                    new SqlParameter("@vehicleid",query.VehicleCriterion),
                    new SqlParameter("@pailiang",query.PaiLiangCriterion),
                    new SqlParameter("@year",query.YearCriterion),
                    new SqlParameter("@tid",query.SalesNameCriterion),
                    new SqlParameter("@pagesdata",(query.PageIndex-1)*query.PageDataQuantity),
                    new SqlParameter("@pagedataquantity",query.PageDataQuantity),
                };
            query.TotalCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlcount, sqlParam);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<VehicleTypeInfo>().ToList();
        }
        public static List<VehicleTypeInfo> QueryVehicleTypeInfoByTID(SqlConnection conn, WheelAdapterConfigQuery query)
        {
            string sqlcount = @"SELECT Count(1)
                FROM Gungnir..tbl_Vehicle_Type AS VT with(nolock) INNER JOIN Gungnir..tbl_Vehicle_Type_Timing AS VTM with(nolock)
                ON VT.ProductID=VTM.VehicleID
                AND VTM.TID=@tid
                LEFT JOIN Configuration..WheelAdapterConfigWithTid AS WAC with(nolock) ON WAC.TID COLLATE Chinese_PRC_CI_AS = VTM.TID"
                + (query.IsInfoSpecified.GetValueOrDefault() == 2 ? @" WHERE WAC.TID IS NULL" : 
                (query.IsInfoSpecified.GetValueOrDefault() == 1 ? @" WHERE WAC.TID IS NOT NULL" : @""));
            string sql = @"SELECT VTM.PKID,VTM.TID,VT.Brand,VT.Vehicle,VTM.PaiLiang,VTM.Nian,VTM.SalesName,WAC.PCD,WAC.ET,
                WAC.MinET,WAC.MaxET,WAC.CB,WAC.BoltNutSpec,WAC.BoltNut,WAC.MinWheelSize,WAC.MaxWheelSize,WAC.MinWheelWidth,WAC.MaxWheelWidth
                FROM Gungnir..tbl_Vehicle_Type AS VT with(nolock) INNER JOIN Gungnir..tbl_Vehicle_Type_Timing AS VTM with(nolock)
                ON VT.ProductID=VTM.VehicleID
                AND VTM.TID=@tid
                LEFT JOIN Configuration..WheelAdapterConfigWithTid AS WAC with(nolock) ON WAC.TID COLLATE Chinese_PRC_CI_AS = VTM.TID "
                + (query.IsInfoSpecified.GetValueOrDefault() == 2 ? @"WHERE WAC.TID IS NULL " :
                (query.IsInfoSpecified.GetValueOrDefault() == 1 ? @"WHERE WAC.TID IS NOT NULL " : @"")) +
                @"ORDER BY VT.Brand ASC OFFSET @pagesdata ROWS FETCH NEXT @pagedataquantity ROWS ONLY";
            var sqlParam = new[]
                {
                    new SqlParameter("@tid",query.TIDCriterion),
                    new SqlParameter("@pagesdata",(query.PageIndex-1)*query.PageDataQuantity),
                    new SqlParameter("@pagedataquantity",query.PageDataQuantity),
                };
            query.TotalCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlcount, sqlParam);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<VehicleTypeInfo>().ToList();
        }
        public static List<WheelAdapterConfigWithTid> SelectWheelAdapterConfigWithTid(SqlConnection conn,string tid)
        {
            string sql = @"SELECT * FROM Configuration..WheelAdapterConfigWithTid WHERE TID=@tid";
            var sqlParam = new[]
                {
                    new SqlParameter("@tid",tid),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<WheelAdapterConfigWithTid>().ToList();
        }
        public static List<WheelAdapterConfigWithTid> SelectWheelAdapterConfigWithTid(SqlConnection conn, IEnumerable<string> tids)
        {
            string sql = @"SELECT  *
FROM    Configuration..WheelAdapterConfigWithTid
WHERE   TID IN ( SELECT *
                 FROM   [Tuhu_productcatalog].dbo.SplitString(@tids, ',', 1) );";
            var sqlParam = new[]
            {
                new SqlParameter("@tids",string.Join(",",tids)),
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<WheelAdapterConfigWithTid>().ToList();
        }
        public static bool InsertWheelAdapterConfigWithTid(SqlConnection conn, WheelAdapterConfigWithTid wac, IEnumerable<string> tids)
        {
            bool result = true;
            var props = typeof(WheelAdapterConfigWithTid).GetProperties();
            var getresults = SelectWheelAdapterConfigWithTid(conn, tids);
            if (getresults != null && getresults.Any())
            {
                foreach (var item in getresults)
                {
                    foreach (var propertyInfo in props.Where(x => x.PropertyType != typeof(DateTime?) 
                                                                  && !string.Equals(x.Name, "pkid", StringComparison.OrdinalIgnoreCase)
                                                                  && !string.Equals(x.Name, "tid", StringComparison.OrdinalIgnoreCase)
                                                                  ))
                    {
                        var value = propertyInfo.GetValue(wac);
                        if (value != null)
                        {
                            propertyInfo.SetValue(item, value);
                        }
                    }
                    WheelAdapterConfigLog wacl = new WheelAdapterConfigLog()
                    {
                        TID = item.TID,
                        OperateType = 1,
                        CreateDateTime = wac.CreateDateTime,
                        LastUpdateDateTime = wac.LastUpdateDateTime,
                        Operator = wac.Creator,
                    };
                    InsertWheelAdapterConfigLog(conn,wacl);
                    item.LastUpdateDateTime = DateTime.Now;
                    item.Creator = wac.Creator;
                    result &= UpdateWheelAdapterConfigWithTid(conn, item);
                }
            }

            var notgetitems = tids.Except(getresults.Select(x => x.TID)).Select(x =>
            {
                var item = JsonConvert.DeserializeObject<WheelAdapterConfigWithTid>(JsonConvert.SerializeObject(wac));
                item.TID = x;
                return item;
            });
            foreach (var item in notgetitems)
            {
                WheelAdapterConfigLog wacl = new WheelAdapterConfigLog()
                {
                    TID = item.TID,
                    OperateType = 0,
                    CreateDateTime = wac.CreateDateTime,
                    LastUpdateDateTime = wac.LastUpdateDateTime,
                    Operator = wac.Creator,
                };
                InsertWheelAdapterConfigLog(conn, wacl);
                result &= InsertWheelAdapterConfigWithTid(conn, item);
            }

            return result;
            //string sql = "";
            //var sqlParam = new List<SqlParameter>();
            //var props = typeof(WheelAdapterConfigWithTid).GetProperties();
            //List<string> insertsqls = new List<string>();

            //foreach (var propertyInfo in props.Where(x => x.PropertyType != typeof(DateTime) && !string.Equals(x.Name, "pkid", StringComparison.OrdinalIgnoreCase)))
            //{

            //}
            //var records = new List<SqlDataRecord>(tids.Count());

            //foreach (var target in tids)
            //{
            //    var record = new SqlDataRecord(new SqlMetaData("Pid", SqlDbType.Char, 40));
            //    var chars = new SqlChars(target.ToString());
            //    record.SetSqlChars(0, chars);
            //    records.Add(record);
            //}
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.CommandType = CommandType.Text;
            //    SqlParameter p = new SqlParameter("@TVP", SqlDbType.Structured);
            //    p.TypeName = "dbo.Pids";
            //    p.Value = records;

            //    sqlParam.Add(p);

            //    //return await dbhelper.ExecuteNonQueryAsync(cmd);
            //    return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam.ToArray()) > 0;
            //}
        }

        public static bool InsertWheelAdapterConfigWithTid(SqlConnection conn, WheelAdapterConfigWithTid wac)
        {
            string sql = @"INSERT INTO Configuration..WheelAdapterConfigWithTid
                (TID,PCD,ET,MinET,MaxET,CB,BoltNutSpec,BoltNut,MinWheelSize,MaxWheelSize,
                MinWheelWidth,MaxWheelWidth,CreateDateTime,LastUpdateDateTime,Creator)
                VALUES(@tid,@pcd,@et,@minet,@maxet,@cb,@boltnutspec,@boltnut,@minwheelsize,@maxwheelsize,
                @minwheelwidth,@maxwheelwidth,@createdatetime,@lastupdatedatetime,@creator)";
            var sqlParam = new[]
                {
                    new SqlParameter("@tid",wac.TID),
                    new SqlParameter("@pcd",wac.PCD),
                    new SqlParameter("@et",wac.ET),
                    new SqlParameter("@minet",wac.MinET),
                    new SqlParameter("@maxet",wac.MaxET),
                    new SqlParameter("@cb",wac.CB),
                    new SqlParameter("@boltnutspec",wac.BoltNutSpec),
                    new SqlParameter("@boltnut",wac.BoltNut),
                    new SqlParameter("@minwheelsize",wac.MinWheelSize),
                    new SqlParameter("@maxwheelsize",wac.MaxWheelSize),
                    new SqlParameter("@minwheelwidth",wac.MinWheelWidth),
                    new SqlParameter("@maxwheelwidth",wac.MaxWheelWidth),
                    new SqlParameter("@createdatetime",wac.CreateDateTime),
                    new SqlParameter("@lastupdatedatetime",wac.LastUpdateDateTime),
                    new SqlParameter("@creator",wac.Creator),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }
        public static bool UpdateWheelAdapterConfigWithTid(SqlConnection conn, WheelAdapterConfigWithTid wac)
        {
            string sql = @"UPDATE Configuration..WheelAdapterConfigWithTid
                SET PCD=@pcd,
                ET=@et,
                MinET=@minet,
                MaxET=@maxet,
                CB=@cb,
                BoltNutSpec=@boltnutspec,
                BoltNut=@boltnut,
                MinWheelSize=@minwheelsize,
                MaxWheelSize=@maxwheelsize,
                MinWheelWidth=@minwheelwidth,
                MaxWheelWidth=@maxwheelwidth,
                LastUpdateDateTime=@lastupdatedatetime
                WHERE TID=@tid";
            var sqlParam = new[]
                {
                    new SqlParameter("@tid",wac.TID),
                    new SqlParameter("@pcd",wac.PCD),
                    new SqlParameter("@et",wac.ET),
                    new SqlParameter("@minet",wac.MinET),
                    new SqlParameter("@maxet",wac.MaxET),
                    new SqlParameter("@cb",wac.CB),
                    new SqlParameter("@boltnutspec",wac.BoltNutSpec),
                    new SqlParameter("@boltnut",wac.BoltNut),
                    new SqlParameter("@minwheelsize",wac.MinWheelSize),
                    new SqlParameter("@maxwheelsize",wac.MaxWheelSize),
                    new SqlParameter("@minwheelwidth",wac.MinWheelWidth),
                    new SqlParameter("@maxwheelwidth",wac.MaxWheelWidth),
                    new SqlParameter("@lastupdatedatetime",wac.LastUpdateDateTime),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }
        public static bool InsertWheelAdapterConfigLog(SqlConnection conn, WheelAdapterConfigLog wacl)
        {
            string sql = @"INSERT INTO Configuration..WheelAdapterConfigLog(TID,OperateType,CreateDateTime,LastUpdateDateTime,Operator)
                VALUES(@tid,@operatetype,@createdatetime,@lastupdatedatetime,@operator)";
            var sqlParam = new[]
                {
                    new SqlParameter("@tid",wacl.TID),
                    new SqlParameter("@operatetype",wacl.OperateType),
                    new SqlParameter("@createdatetime",wacl.CreateDateTime),
                    new SqlParameter("@lastupdatedatetime",wacl.LastUpdateDateTime),
                    new SqlParameter("@operator",wacl.Operator),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }
        public static List<WheelAdapterConfigLog> SelectWheelAdapterConfigLog(SqlConnection conn, string tid)
        {
            string sql = @"SELECT * FROM Configuration..WheelAdapterConfigLog WHERE TID=@tid";
            var sqlParam = new[]
                {
                    new SqlParameter("@tid",tid),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<WheelAdapterConfigLog>().ToList();
        }
    }
}
