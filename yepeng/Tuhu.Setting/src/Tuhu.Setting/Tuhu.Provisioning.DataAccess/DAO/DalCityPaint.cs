using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalCityPaint
    {
        /// <summary>
        /// 获取油漆产品全国价
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectCountryPaintList()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT PKID,PID,DisplayName,Price,IsCountry,CreatedTime,UpdatedTime FROM Configuration.dbo.tbl_PaintInfo WITH ( NOLOCK )
                                WHERE  IsCountry = 1 ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }
        /// <summary>
        /// 根据PKID获取油漆产品全国价
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static DataRow GetCountryPaintByPKID(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT PKID,PID,DisplayName,Price,IsCountry,CreatedTime,UpdatedTime FROM Configuration.dbo.tbl_PaintInfo WITH ( NOLOCK )  WHERE  PKID = @PKID AND IsCountry = 1 ;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteDataRow(cmd);
                }
            }
        }

        /// <summary>
        /// 根据PID获取油漆产品全国价
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static DataRow GetCountryPaintByPID(string pid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT PKID,PID,DisplayName,Price,IsCountry,CreatedTime,UpdatedTime FROM Configuration.dbo.tbl_PaintInfo WITH ( NOLOCK )  WHERE  PID = @PID AND IsCountry = 1 ;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PID", pid);
                    return dbhelper.ExecuteDataRow(cmd);
                }
            }
        }
        /// <summary>
        /// 修改油漆产品全国价
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateCountryPaint(PaintInfoModel model)
        {
            bool result = false;
            if (model != null && model.PKID != 0)
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"UPDATE Configuration.dbo.tbl_PaintInfo SET PID=@PID,DisplayName=@DisplayName,Price=@Price,UpdatedTime=getdate() where PKID=@PKID ";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@PID", model.PID);
                        cmd.Parameters.AddWithValue("@DisplayName", model.DisplayName);
                        cmd.Parameters.AddWithValue("@Price", model.Price);
                        cmd.Parameters.AddWithValue("@PKID", model.PKID);
                        if (dbhelper.ExecuteNonQuery(cmd) > 0)
                            result = true;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 添加油漆产品全国价
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertCountryPaint(PaintInfoModel model)
        {
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"  insert into Configuration.dbo.tbl_PaintInfo(PID,DisplayName,Price,IsCountry,CreatedTime,UpdatedTime)
  values(@PID,@DisplayName,@Price,@IsCountry,getdate(),getdate())";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@PID", model.PID);
                        cmd.Parameters.AddWithValue("@DisplayName", model.DisplayName);
                        cmd.Parameters.AddWithValue("@Price", model.Price);
                        cmd.Parameters.AddWithValue("@IsCountry", model.IsCountry);
                        return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /// <summary>
        /// 删除油漆产品全国价
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteCountryPaintByPkid(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"delete from  Configuration.dbo.tbl_PaintInfo where PKID = @PKID  ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                }
            }
        }

        /// <summary>
        /// 查询已有的油漆产品
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectPaintList()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT PID,OrigProductID,CategoryName,cy_list_price,ProductID,VariantID,CatalogName,PrimaryParentCategory,oid,DefinitionName,OnSale,Brand,DisplayName,Description,Name,CP_Brand
    FROM    [Tuhu_productcatalog]..[CarPAR_zh-CN] WITH ( NOLOCK )
    WHERE   ( PrimaryParentCategory = N'喷漆服务' )
            AND cy_list_price > 0
            AND OnSale = 1
            AND stockout = 0  ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 获取油漆产品特殊价
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectCityPaintList()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT TP.*,CT.CityName FROM Configuration.dbo.tbl_PaintInfo AS TP WITH ( NOLOCK )
                                LEFT JOIN (
                                SELECT PaintId,(SELECT CityName+' ' FROM Configuration.dbo.tbl_CityPaint WHERE PaintId=A.PaintId  FOR XML PATH('')) AS CityName
                                FROM Configuration.dbo.tbl_CityPaint A
                                GROUP BY PaintId
                                ) CT on TP.PKID=CT.PaintId
                                WHERE  TP.IsCountry = 0 ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }
        /// <summary>
        /// 根据PKID获取油漆产品特殊价
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static DataRow GetCityPaintByPKID(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT TP.*,CT.CityName FROM Configuration.dbo.tbl_PaintInfo AS TP WITH ( NOLOCK )
                                LEFT JOIN (
                                SELECT PaintId,(SELECT CityName+' ' FROM Configuration.dbo.tbl_CityPaint WHERE PaintId=A.PaintId  FOR XML PATH('')) AS CityName
                                FROM Configuration.dbo.tbl_CityPaint A
                                GROUP BY PaintId
                                ) CT on TP.PKID=CT.PaintId
                                WHERE  TP.IsCountry = 0 AND  TP.PKID = @PKID ;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteDataRow(cmd);
                }
            }
        }

        /// <summary>
        /// 根据PID和城市ID获取油漆产品特殊价
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public static DataTable GetCityPaintByPIDAndCityId(string pid, int cityId, int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT TP.* FROM Configuration.dbo.tbl_PaintInfo AS TP WITH ( NOLOCK )
                                LEFT JOIN Configuration.dbo.tbl_CityPaint AS CP WITH ( NOLOCK ) ON TP.PKID = CP.PaintId
                                WHERE TP.PID= @PID AND CP.CityId =@CityId AND TP.PKID !=@PKID ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PID", pid);
                    cmd.Parameters.AddWithValue("@CityId", cityId);
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);
        /// <summary>
        /// 删除油漆特殊价信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteCityPaintByPkid(int pkid)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var sql = @"delete from [Configuration].[dbo].[tbl_CityPaint] where PaintId = @PKID ";
                var sqlDelete = @"delete from  Configuration.dbo.tbl_PaintInfo where PKID = @PKID";

                var sqlParamForDel = new SqlParameter[]
                {
                    new SqlParameter("@PKID",pkid),
                };
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParamForDel);
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlDelete, sqlParamForDel);


                scope.Complete();
            }
            return true;

        }

        /// <summary>
        /// 添加油漆特殊价,返回主键值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertCityPaint(PaintInfoModel model)
        {
            try
            {
                string sql = @" insert into Configuration.dbo.tbl_PaintInfo(PID,DisplayName,Price,IsCountry,CreatedTime,UpdatedTime)
  values(@PID,@DisplayName,@Price,@IsCountry,getdate(),getdate())
                SELECT @@IDENTITY";
                SqlParameter[] collection = new SqlParameter[]
                {
                    new SqlParameter("@PID", model.PID),
                    new SqlParameter("@DisplayName", model.DisplayName),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@IsCountry", model.IsCountry)
                };
                object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, collection);
                if (obj != null)
                    return Convert.ToInt32(obj);
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        //批量插入油漆城市
        public static bool BulkSaveCityPaint(DataTable tb)
        {
            try
            {
                var conn = ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
                {

                    bulk.BatchSize = tb.Rows.Count;
                    bulk.DestinationTableName = "tbl_CityPaint";
                    bulk.WriteToServer(tb);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改油漆特殊价
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateCityPaint(PaintInfoModel model)
        {
            bool result = false;
            if (model != null && model.PKID != 0)
            {
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    string sql = @"  UPDATE Configuration.dbo.tbl_PaintInfo SET PID=@PID,DisplayName=@DisplayName,Price=@Price,UpdatedTime=getdate() where PKID=@PKID ";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@PID", model.PID);
                        cmd.Parameters.AddWithValue("@DisplayName", model.DisplayName);
                        cmd.Parameters.AddWithValue("@Price", model.Price);
                        cmd.Parameters.AddWithValue("@PKID", model.PKID);
                        if (dbhelper.ExecuteNonQuery(cmd) > 0)
                            result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 删除油漆匹配城市信息
        /// </summary>
        /// <returns></returns>
        public static bool DeleteCity(List<CityPaintModel> list)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (var model in list)
                {
                    var sqlDelete = @"delete from [Configuration].[dbo].[tbl_CityPaint] where PKID = @PKID";

                    var sqlParamForDel = new SqlParameter[]
                    {
                    new SqlParameter("@PKID",model.PKID),
                    };

                    SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlDelete, sqlParamForDel);
                }

                scope.Complete();
            }


            return true;
        }

        /// <summary>
        /// 根据城市IDs获取城市信息
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectCityRegionList(string pkids)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT	ISNULL(P.PKID, C.PKID) AS ProvinceID,
		                                ISNULL(P.RegionName, C.RegionName) AS Province,
		                                ISNULL(P.IsInstall, C.IsInstall) AS ProvinceIsInstall,
		                                C.PKID AS CityID,
		                                C.RegionName AS City,
		                                C.IsInstall AS CityIsInstall
                                FROM	[Gungnir]..tbl_Region AS C WITH(NOLOCK)
                                LEFT JOIN [Gungnir]..tbl_Region AS P WITH(NOLOCK)
		                                ON C.ProvinceID = P.PKID
                                WHERE C.PKID IN (select Item AS PKID from Gungnir..SplitString(@PKIDS, ',', 1)) ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PKIDS", pkids);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 根据油漆ID获取油漆已配置城市
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectPaintCityList(int paintId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT PKID,PaintId,CityId,ProvinceId,CityName,CreatedTime,UpdatedTime FROM Configuration.dbo.tbl_CityPaint WITH ( NOLOCK )
                                WHERE  PaintId = @PaintId ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PaintId", paintId);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 根据油漆PID获取油漆已配置城市
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectPaintCityListByPid(string pid, int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT CP.* FROM Configuration.dbo.tbl_PaintInfo AS TP WITH ( NOLOCK )
                               JOIN Configuration.dbo.tbl_CityPaint AS CP WITH ( NOLOCK ) ON TP.PKID = CP.PaintId
                            WHERE TP.PID = @PID AND TP.PKID != @PKID";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PID", pid);
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }

        /// <summary>
        /// 获取城市信息
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllCityRegionList()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @"SELECT	ISNULL(P.PKID, C.PKID) AS ProvinceID,
		                                ISNULL(P.RegionName, C.RegionName) AS Province,
		                                C.PKID AS CityID,
		                                C.RegionName AS City,
										substring(C.PinYin,1,1) AS Letter
                                FROM	[Gungnir]..tbl_Region AS C WITH(NOLOCK)
                                LEFT JOIN [Gungnir]..tbl_Region AS P WITH(NOLOCK)
		                                ON C.ProvinceID = P.PKID
WHERE C.PinYin is not null and ( C.ProvinceID <>0 or C.ParentID <> 0 or C.CityID <> 0 ) order by substring(C.PinYin,1,1),C.RegionName ";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    return dbhelper.ExecuteDataTable(cmd);
                }
            }
        }
    }
}
