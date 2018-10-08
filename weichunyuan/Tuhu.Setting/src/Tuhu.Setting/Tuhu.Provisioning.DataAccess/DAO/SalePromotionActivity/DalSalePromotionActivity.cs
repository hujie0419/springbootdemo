using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.SalePromotionActivity;

namespace Tuhu.Provisioning.DataAccess.DAO.SalePromotionActivity
{
    public class DalSalePromotionActivity 
    {
        private static readonly string ConnStr;

        /// <summary>
        /// 获取渠道列表
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static IEnumerable<ChannelVModel> GetChannelList(SqlConnection conn)
        {
            string sql = @" SELECT  [ChannelType]
                          ,[ChannelKey]
                          ,[ChannelValue]
                      FROM [Gungnir].[dbo].[tbl_ChannelDictionaries]  WITH (NOLOCK)
                    where ChannelType in (N'自有渠道','第三方平台',N'H5合作渠道') ";
            return conn.Query<ChannelVModel>(sql);
        }

        /// <summary>
        /// 批量获取商品的库存和成本价
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static IEnumerable<ProductCostAndStock> GetProductStockAndCostList(List<string> pidList)
        {
            var list = new List<ProductCostAndStock>();
            if(pidList==null|| pidList.Count == 0)
            {
                return list;
            }
            string sql = @" SELECT Pid, isnull(totalstock,0) as TotalStock,cost as CostPrice
                            FROM Tuhu_bi.dbo.dm_Product_SalespredictData WITH (NOLOCK)
                            JOIN Tuhu_bi..SplitString(@pids, ',', 1) AS B ON pid = B.Item;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                var result = dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] { new SqlParameter("@pids", string.Join(",", pidList)) });
                return result.ConvertTo<ProductCostAndStock>();
            }
        }

        /// <summary>
        /// 根据key获取渠道名称列表
        /// </summary>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public static IEnumerable<ChannelVModel> GetChannelValueByKey(List<string> keyList) 
        {
            string sql = @" SELECT c.[ChannelValue]
                      FROM [Gungnir].[dbo].[tbl_ChannelDictionaries] c WITH (NOLOCK) 
                          JOIN Tuhu_bi..SplitString(@keyList, ',', 1) AS B 
                            ON c.ChannelKey = B.Item collate Chinese_PRC_CI_AI_WS";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                var result = dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] { new SqlParameter("@keyList", string.Join(",", keyList)) });
                return result.ConvertTo<ChannelVModel>();
            }
            }

    }
}
