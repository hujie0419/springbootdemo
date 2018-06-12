using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.DataAccess.DAO.Tire
{
    public sealed class DalStockoutStatus
    {
        public static IEnumerable<StockoutStatusModel> SelectList(StockoutStatusRequest request, PagerModel pager)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                var para = new SqlParameter[] {
                    new SqlParameter("@CityId",request.CityId),
                    new SqlParameter("@PID",string.IsNullOrWhiteSpace(request.PID)?null:request.PID),
                    new SqlParameter("@Status",request.Status),
                    new SqlParameter("@TireSize",request.TireSize),
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize),
                };
                var para_temp = new SqlParameter[] {
          new SqlParameter("@CityId",request.CityId),
                    new SqlParameter("@PID",string.IsNullOrWhiteSpace(request.PID)?null:request.PID),
                    new SqlParameter("@Status",request.Status),
                    new SqlParameter("@TireSize",request.TireSize),
                };
                pager.TotalItem = GetListCount(dbHelper, para_temp);
                return dbHelper.ExecuteDataTable(TireSql.StockoutWgite.select, CommandType.Text, para).ConvertTo<StockoutStatusModel>();
            }
        }

        public static int UpdateWhite(SqlDbHelper dbHelper, string pid)
        {
            return dbHelper.ExecuteNonQuery(TireSql.StockoutStatusWhite.update, CommandType.Text, new SqlParameter("@PID", pid));
        }

        public static int RemoveWhite(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
                return dbHelper.ExecuteNonQuery(TireSql.StockoutStatusWhite.delete, CommandType.Text, new SqlParameter("@PID", pid));
        }

        public static int InsertWhite(SqlDbHelper dbHelper, string pid)
        {
            return dbHelper.ExecuteNonQuery(TireSql.StockoutStatusWhite.insert, CommandType.Text, new SqlParameter("@PID", pid));
        }

        public static bool FetchPidStatus(SqlDbHelper dbHelper, string pid)
        {
            var obj = dbHelper.ExecuteScalar(TireSql.StockoutStatusWhite.fetch_pidStatus, CommandType.Text, new SqlParameter("@PID", pid));
            return obj == null || obj == DBNull.Value ? false : true;
        }

        public static IEnumerable<StockoutStatusWhiteModel> SelectWhiteList(WhiteRequest model, PagerModel pager)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                pager.TotalItem = GetListWhiteCount(dbHelper, model);
                return dbHelper.ExecuteDataTable(TireSql.StockoutWhite.select, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PID", model.PID),
                    new SqlParameter("@ProductName",model.DisplayName),
                    new SqlParameter("@OnSale",model.OnSale),
                    new SqlParameter("@TireSize",model.TireSize),
                    new SqlParameter("@stuckout",model.Stuckout),
                    new SqlParameter("@SystemStuckout",model.SystemStuckout),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@CityId",model.CityId ),
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize),
                    new SqlParameter("@RegionalStockout",model.RegionalStockout),
                    new SqlParameter("@isShow",model.IsShow)
                }).ConvertTo<StockoutStatusWhiteModel>();
            }
        }

        private static int GetListWhiteCount(SqlDbHelper dbHelper, WhiteRequest model)
        {
            return Convert.ToInt32(dbHelper.ExecuteScalar(TireSql.StockoutWhite.select_count, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PID", model.PID),
                    new SqlParameter("@ProductName",model.DisplayName),
                    new SqlParameter("@OnSale",model.OnSale),
                    new SqlParameter("@stuckout",model.Stuckout),
                    new SqlParameter("@TireSize",model.TireSize),
                    new SqlParameter("@SystemStuckout",model.SystemStuckout),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@CityId",model.CityId ),
                    new SqlParameter("@RegionalStockout",model.RegionalStockout),
                    new SqlParameter("@isShow",model.IsShow)
                }));
        }

        private static int GetListCount(SqlDbHelper dbHelper, SqlParameter[] para)
        {
            return Convert.ToInt32(dbHelper.ExecuteScalar(TireSql.StockoutWgite.select_count, CommandType.Text, para));
        }

        public static IEnumerable<StockoutStatusWhiteModel> GetStockoutStatusByPids(List<string> pids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return dbHelper.ExecuteDataTable(TireSql.StockoutWhite.select_showStatus, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@pids",string.Join(",",pids))
                }).ConvertTo<StockoutStatusWhiteModel>();
            }
        }

        public static List<RegionStockModel> SelectRegionStockList(RegionStockRequest model, PagerModel pager)
        {
            string sql = $@"WITH    c AS ( SELECT   *
               FROM     [Tuhu_bi].[dbo].[tbl_CarTireRecommendation] WITH ( NOLOCK )
                WHERE {string.Format(string.IsNullOrEmpty(model.VehicleId)||string.IsNullOrEmpty(model.TireSize)?"1=2": "  TireSize = @TireSize  and vehicleid = @vehicleid ")} 
             )
    SELECT  stock.pid Pid ,
            VMP.DisplayName ,
            stock.defaultwareHouse AS DefaultStockName ,
            stock.defaultavailnums StockNum ,
            stock.defaultsafenums SafeStockNum ,
            stock.nationalavailnums AllStockNum ,
            stock.nationalsafenums AllSafeStockNum ,
            stock.centralavailnums CentralStockNum ,
            stock.centralsafenums CentralSafeStockNum ,
            stock.regionavailnums RegionavailNums ,
            c.NoReduceStockGrade
    FROM    [Tuhu_bi].[dbo].[tbl_CarTireRecommendation_Region_StockList] AS stock
            WITH ( NOLOCK )
            LEFT JOIN Tuhu_productcatalog..vw_Products AS VMP WITH ( NOLOCK ) ON VMP.PID = stock.pid
            {string.Format(string.IsNullOrEmpty(model.VehicleId) ? " left join " : " join ")}  c ON c.ProductId = VMP.PID
    WHERE   stock.cityID = @CityId
            AND ( stock.pid LIKE 'TR-%'
                  AND ( stock.pid LIKE '%' + @PID + '%'
                        OR @PID IS NULL
                      )
                )
            AND ( @ProductName IS NULL
                  OR VMP.DisplayName LIKE '%' + @ProductName + '%'
                )
            {string.Format(string.IsNullOrEmpty(model.VehicleId) ? "  " : " and c.NoReduceStockGrade is not null ")}  
    ORDER BY stock.cityID ,
            stock.pid DESC
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;";

            pager.TotalItem = RegionStockListCount(model);
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PID",model.PID),
                    new SqlParameter("@CityId",model.CityId),
                    new SqlParameter("@ProductName",model.ProductName),
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize),
                    new SqlParameter("@vehicleid",model.VehicleId),
                    new SqlParameter("@TireSize",model.TireSize)
                }).ConvertTo<RegionStockModel>().ToList();
            }

        }
        private static int RegionStockListCount(RegionStockRequest model)
        {
            string sql = $@"WITH    c AS ( SELECT   *
  FROM     [Tuhu_bi].[dbo].[tbl_CarTireRecommendation] WITH ( NOLOCK )
              WHERE {string.Format(string.IsNullOrEmpty(model.VehicleId) || string.IsNullOrEmpty(model.TireSize) ? "1=2" : "  TireSize = @TireSize  and vehicleid = @vehicleid ")} 
             )
    SELECT  count(1)
    FROM    [Tuhu_bi].[dbo].[tbl_CarTireRecommendation_Region_StockList] AS stock
            WITH ( NOLOCK )
            LEFT JOIN Tuhu_productcatalog..vw_Products AS VMP WITH ( NOLOCK ) ON VMP.PID = stock.pid
            {string.Format(string.IsNullOrEmpty(model.VehicleId) ? " left join " : " join ")}  c ON c.ProductId = VMP.PID
    WHERE   stock.cityID = @CityId
            AND ( stock.pid LIKE 'TR-%'
                  AND ( stock.pid LIKE '%' + @PID + '%'
                        OR @PID IS NULL
                      )
                )
            AND ( @ProductName IS NULL
                  OR VMP.DisplayName LIKE '%' + @ProductName + '%'
                )
            {string.Format(string.IsNullOrEmpty(model.VehicleId) ? "  " : " and c.NoReduceStockGrade is not null ")}   ";


            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return Convert.ToInt32(dbHelper.ExecuteScalar(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PID",model.PID),
                    new SqlParameter("@CityId",model.CityId),
                    new SqlParameter("@ProductName",model.ProductName),
                    new SqlParameter("@vehicleid",model.VehicleId),
                    new SqlParameter("@TireSize",model.TireSize)
                }));
            }
        }
    }
}
