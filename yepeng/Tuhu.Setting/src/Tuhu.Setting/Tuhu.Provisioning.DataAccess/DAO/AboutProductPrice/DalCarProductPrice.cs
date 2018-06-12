using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.ProductPrice;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity.GroupBuyingV2;
using Tuhu.Component.Framework;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess.DAO.AboutProductPrice
{
    public static class DalCarProductPrice
    {  
        public static async Task<List<CarProductPriceModel>> GetProductBaseInfoByCatalog(SqlConnection conn,List<string> pids)
        {
            DynamicParameters paras = new DynamicParameters();
             
            string sql = $@"SELECT p.oid,pid PID,p.IsDaiFa,p.OnSale,p.StockOut,ISNULL(p.cy_list_price,0) OriginalPrice,p.Name ProductName 
 FROM Tuhu_productcatalog..[CarPAR_zh-CN] AS p with(NOLOCK) 
 INNER JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS h with(NOLOCK) ON p.oid = h.child_oid 
 WHERE  p.PID IN @pids "; 
            var list = (await conn.QueryAsync<CarProductPriceModel>(sql, new { pids})).ToList();
            return list;
        }
         
        /// <summary>
        /// 查询商品历史活动价格
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public static async Task<List<ActivityPriceModel>> GetActivityPricesByPids(SqlConnection conn, List<string> pids)
        {
            string sql = @" SELECT fs.StartDateTime BeginTime,fs.EndDateTime EndTime,fs.ActivityId,fsp.PID,fs.ActiveType,fsp.Price FROM Activity..tbl_FlashSaleProducts fsp WITH ( NOLOCK) 
  INNER JOIN Activity..tbl_FlashSale fs WITH(NOLOCK) ON fs.ActivityID=fsp.ActivityID 
   WHERE PID in @PIDs AND fs.ActiveType IN(1,4)";
            return (await conn.QueryAsync<ActivityPriceModel>(sql, new { PIDs = pids })).ToList();
        }

        /// <summary>
        /// 查询商品历史拼团价格
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public static async Task<List<ActivityPriceModel>> GetPintuanPricesByPids(SqlConnection conn, List<string> pids,DateTime EndTime)
        {
            try
            {
                string sql = @"SELECT PID,gc.BeginTime,gc.EndTime,gc.ActivityId,pc.FinalPrice Price FROM Configuration..GroupBuyingProductConfig pc WITH(NOLOCK) 
                             INNER JOIN Configuration..GroupBuyingProductGroupConfig gc WITH(NOLOCK) 
                             ON gc.ProductGroupId=pc.ProductGroupId 
                             WHERE pc.PID in @PIDs and gc.EndTime > @EndTime 
                             order  by  gc.EndTime desc";
                return (await conn.QueryAsync<ActivityPriceModel>(sql, new { PIDs = pids, EndTime = EndTime })).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

        /// <summary>
        /// 查询指定城市的库存信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pids">商品pid集合</param>
        /// <param name="wareHouserIds">需要查询的仓库id集合</param>
        /// <returns></returns>
        public static async Task<List<ProductStockInfo>> GetStockQuantityByPids(SqlConnection conn, List<string> pids, List<string> wareHouserIds)
        {
            if (wareHouserIds == null || wareHouserIds.Count < 1 || pids == null|| pids.Count < 1)
                return new List<ProductStockInfo>();

            string str = string.Join(" or ", wareHouserIds.Select(x => $"WAREHOUSEID={x}"));

            string sql = $@"select PID,
       WAREHOUSEID,
       TotalAvailableStockQuantity,
       StockCost,
       CaigouZaitu
from Tuhu_bi..dw_ProductAvaibleStockQuantity with (nolock)
where PID in @PIDs and ({str})";
            try
            {
                return (await conn.QueryAsync<ProductStockInfo>(sql, new { PIDs = pids })).ToList();

            }
            catch (Exception)
            {

                throw;
            }

            
        }

        /// <summary>
        /// 查询指定城市的库存信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pids">商品pid集合</param>
        /// <param name="wareHouserIds">需要查询的仓库id集合</param>
        /// <returns></returns>
        public static  List<ProductStockInfo> GetStockQuantityByPids_new (SqlConnection conn, List<string> pids, List<string> wareHouserIds)
        {
            List<ProductStockInfo> result = new List<ProductStockInfo>();
            if (wareHouserIds == null || wareHouserIds.Count < 1 || pids == null || pids.Count < 1)
                return result;

            string str = string.Join(" or ", wareHouserIds.Select(x => $"WAREHOUSEID={x}"));

            string pidStr ="'" +string.Join("','",pids)+"'";

            string sql = $@"select PID,
                           WAREHOUSEID,
                           TotalAvailableStockQuantity,
                           StockCost,
                           CaigouZaitu
                    from Tuhu_bi..dw_ProductAvaibleStockQuantity with (nolock)
                    where PID in ({pidStr}) and ({str})";
            try
            {
 
                //var parameters = new SqlParameter[] {
                //    new SqlParameter("@PIDs", pids),
                //};
                //var result =  SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<ProductStockInfo>().ToList();
                result =  SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<ProductStockInfo>().ToList();

                //result=conv
                //return (await conn.QueryAsync<ProductStockInfo>(sql, new { PIDs = pids })).ToList();
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public static async Task<List<CarProductPriceModel>> GetCaigouPriceByPids(SqlConnection conn,List<string> pids)
        {
            string sql = $@"SELECT  p.ProductCode PID ,
                            ISNULL(p.PurchasePrice, 0) PurchasePrice ,	 
                            ISNULL(p.ContractPrice, 0) ContractPrice , 
                            ISNULL(v.OfferPurchasePrice, 0) OfferPurchasePrice , 
                            ISNULL(v.OfferContractPrice, 0) OfferContractPrice  
                            FROM    Gungnir..PurchaseInfo AS p WITH ( NOLOCK )
                                    Left JOIN Gungnir..VendorPreferentialInfo AS v WITH ( NOLOCK ) ON v.VenderId = p.Id
                            WHERE   p.ProductCode IN @PIDs ";
            return (await conn.QueryAsync<CarProductPriceModel>(sql, new { PIDs = pids })).ToList();
        }

        /// <summary>
        /// 获取offset分页sql
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string GetPaginationSql(int page, int pageSize)
        {
            string pagerSql = $@" OFFSET {pageSize * (page - 1)} ROW FETCH NEXT {pageSize} rows ONLY ";
            return pagerSql;
        }

    }
}
