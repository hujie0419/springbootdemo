using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
   public  class DalLimitAreaSale
    {
        public static IEnumerable<LimitAreaSaleModel> GetProductByPid(string keyWord)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @" SELECT  P.PID ,
                        P.DisplayName AS ProductName,
                        LP.IsLimit
                        FROM    Tuhu_productcatalog..vw_Products AS P WITH ( NOLOCK ) 
                        left join Configuration..LimitAreaSaleProductConfig AS LP WITH ( NOLOCK ) on P.Pid=LP.Pid
                        where p.Pid=@Pid";
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[]
                    {
                        new SqlParameter("@Pid", keyWord),
                    }).ConvertTo<LimitAreaSaleModel>();

            }
        }

        public static int? GetLimitAreaSaleProductConfigLimit(string keyWord)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @" select LP.IsLimit
                        FROM  Configuration..LimitAreaSaleProductConfig AS LP WITH ( NOLOCK )
                        where Pid=@Pid";
                return dbHelper.ExecuteScalar(sql, CommandType.Text, new SqlParameter[]
                    {
                        new SqlParameter("@Pid", keyWord),
                    }) as int?;

            }
        }

        public static IEnumerable<LimitAreaSaleModel> GetLimitAreaSalePidList(int pageSize,int pageNum,string keyWord,int lsLimit)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @"
               SELECT   P.PID ,
                        P.DisplayName AS ProductName ,
                        LP.IsLimit ,
                        COUNT(1) OVER ( ) AS TotalCount
               FROM     Configuration..LimitAreaSaleProductConfig AS LP WITH ( NOLOCK )
                        JOIN Tuhu_productcatalog..vw_Products AS P WITH ( NOLOCK ) ON LP.Pid = P.PID
               WHERE    IsLimit = @IsLimit
               ORDER BY Pid
                        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize
                        ROWS ONLY";
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[]
                    {
                        //new SqlParameter("@Pid", keyWord),
                        new SqlParameter("@IsLimit", lsLimit),
                        new SqlParameter("@PageSize", pageSize),
                        new SqlParameter("@PageIndex", pageNum)
                    }).ConvertTo<LimitAreaSaleModel>();

            }
        }
        public static int SaveLimitAreaSalePid(string pid,int islimit)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sql = @"
                        MERGE INTO  Configuration..LimitAreaSaleProductConfig AS T
						USING (select @Pid as pid,@islimit as isLimit)AS S
						ON t.Pid=s.Pid
						when matched
						THEN UPDATE   SET IsLimit=@islimit
						when NOT MATCHED THEN 
						INSERT (pid,IsLimit)VALUES(@Pid,@islimit);
                        SELECT Pkid FROM Configuration..LimitAreaSaleProductConfig WHERE Pid=@Pid";
                return Convert.ToInt32(dbHelper.ExecuteScalar(sql, CommandType.Text, new SqlParameter[]
                    {
                        new SqlParameter("@Pid", pid),
                        new SqlParameter("@islimit",islimit),
                    }));

            }
        }
        public static int SaveLimitAreaSaleCity(int productConfigId, int cityId, string cityName, int isAllowSale, int? warehouseId, string warehouseName, int? supplierId, string supplierName)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sql = @"
                        MERGE INTO  Configuration..LimitAreaSaleCityConfig AS T
						USING (select @CityId as CityId,@ProductConfigId as ProductConfigId)AS S
						ON t.CityId=s.CityId and t.ProductConfigId=s.ProductConfigId
						when matched
						THEN UPDATE   SET IsAllowSale=@IsAllowSale,WarehouseId=@WarehouseId,WarehouseName=@WarehouseName,SupplierId=@SupplierId,SupplierName=@SupplierName
						when NOT MATCHED THEN 
						INSERT (ProductConfigId,CityId,CityName,IsAllowSale,WarehouseId,WarehouseName,SupplierId,SupplierName)
                        VALUES(@ProductConfigId,@CityId,@CityName,@IsAllowSale,@WarehouseId,@WarehouseName,@SupplierId,@SupplierName);
                        SELECT Pkid FROM Configuration..LimitAreaSaleCityConfig WHERE ProductConfigId=@ProductConfigId and CityId=@CityId";
                return Convert.ToInt32(dbHelper.ExecuteScalar(sql, CommandType.Text, new SqlParameter[]
                    {
                        new SqlParameter("@ProductConfigId", productConfigId),
                        new SqlParameter("@CityId",cityId),
                        new SqlParameter("@CityName", cityName),
                        new SqlParameter("@IsAllowSale", isAllowSale),
                        new SqlParameter("@WarehouseId",warehouseId),
                        new SqlParameter("@WarehouseName", warehouseName),
                        new SqlParameter("@SupplierId",supplierId),
                        new SqlParameter("@SupplierName", supplierName)
                    }));

            }
        }
        public static SimpleLimitAreaSaleCityModel GetLimitAreaSaleCityConfigLimit(int fk,int cityId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @" select *
                        FROM  Configuration..LimitAreaSaleCityConfig AS LP WITH ( NOLOCK )
                        where CityId=@cityId and ProductConfigId=@ProductConfigId";
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[]
                    {
                        new SqlParameter("@ProductConfigId", fk),
                         new SqlParameter("@cityId", cityId),
                    }).ConvertTo<SimpleLimitAreaSaleCityModel>().FirstOrDefault();

            }
        }
        public static DataTable SelectRegions()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT R.PKID, R.RegionName, R.ParentID, R.IsInstall FROM Gungnir..tbl_Region AS R WHERE PKID NOT IN (32, 33, 34) AND R.IsActive = 1");
            }
        }

        /// <summary>
        /// 这个用来判断这个产品开启了限制地区销售
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int SelectLimitAreaSaleByPid(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = $@" SELECT  PKid from  Configuration..LimitAreaSaleProductConfig AS LP WITH ( NOLOCK )
                        where Pid='{pid}' and IsLimit=1";
                return Convert.ToInt32(dbHelper.ExecuteScalar(sql));
            }
        }
        /// <summary>
        /// 这个是用来获取之前的配置
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<LimitAreaSaleCityModel> SelectLimitAreaSaleCityInfoByPid(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = $@" SELECT  LC.* from  Configuration..LimitAreaSaleCityConfig AS LC WITH ( NOLOCK )
                            join Configuration..LimitAreaSaleProductConfig AS LP WITH ( NOLOCK ) on LC.ProductConfigId=LP.PKID
                        where LP.Pid='{pid}'";
                return dbHelper.ExecuteDataTable(sql).ConvertTo<LimitAreaSaleCityModel>().ToList();
            }
        }

        public static List<LimitAreaSaleCityModel> SelectLimitAreaSaleCityInfo()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = $@" SELECT  LC.* from  Configuration..LimitAreaSaleCityConfig AS LC WITH ( NOLOCK )";
                return dbHelper.ExecuteDataTable(sql).ConvertTo<LimitAreaSaleCityModel>().ToList();
            }
        }
    }
}
