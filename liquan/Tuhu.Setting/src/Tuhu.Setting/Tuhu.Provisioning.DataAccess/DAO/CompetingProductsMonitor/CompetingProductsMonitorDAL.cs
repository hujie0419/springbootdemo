using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Mapping;

namespace Tuhu.Provisioning.DataAccess.DAO.CompetingProductsMonitor
{
    public class CompetingProductsMonitorDAL
    {
        /// <summary>
        /// 根据Pids查询竞品中最低价商品
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public IEnumerable<CompetingProductsMonitorEntity> GetProductsMonitorbyPids(SqlConnection conn, IEnumerable<string> pids)
        {

            StringBuilder sbSql = new StringBuilder();
            var parameters = new SqlParameter[1];
            sbSql.Append(@" WITH MinPriceMonitor 
                                    AS ( SELECT MIN(Price) AS minPrice ,
                                                Pid, 
                                                COUNT(*) AS MonitorCount
                                        FROM Tuhu_productcatalog..CompetingProductsMonitor WITH (NOLOCK) WHERE Is_Deleted=0 AND ");
            if (pids.Count() == 1)
            {
                sbSql.Append(" Pid=@Pid ");
                parameters[0] = new SqlParameter("@Pid", pids.First());
            }
            else
            {
                sbSql.Append(" Pid IN (SELECT * FROM [Tuhu_productcatalog].dbo.SplitString(@Pids,',', 1))");
                parameters[0] = new SqlParameter("@Pids", string.Join(",", pids));
            }
            sbSql.Append(" GROUP BY Pid )");
            sbSql.Append(@" SELECT  [PKID] ,
                                    [ShopCode] ,
                                    mpp.[Pid] ,
                                    [ItemID] ,
                                    [SkuID] ,
                                    [ItemCode] ,
                                    [Properties] ,
                                    [Price] as MinPrice ,
                                    mpp.[MonitorCount] ,
                                    [Promotion] ,
                                    [Title] ,
                                    [ThirdParty] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [Is_Deleted]
                            FROM    MinPriceMonitor mpp WITH (NOLOCK)
                                    LEFT JOIN Tuhu_productcatalog..CompetingProductsMonitor cpm WITH (NOLOCK) ON  cpm.Pid=mpp.Pid 
                                                                                                    AND cpm.Price=mpp.minPrice
                            WHERE   cpm.Is_Deleted = 0; ");
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sbSql.ToString(), parameters).ConvertTo<CompetingProductsMonitorEntity>().ToList(); ;
        }

        /// <summary>
        /// 根据pid获取所有竞品信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public IEnumerable<CompetingProductsMonitorEntity> GetAllProductsMonitorbyPid(SqlConnection conn, string pid)
        {

            string sql = @"SELECT [PKID]
                                  ,[ShopCode]
                                  ,[Pid]
                                  ,[ItemID]
                                  ,[SkuID]
                                  ,[ItemCode]
                                  ,[Properties]
                                  ,[Price]
                                  ,[Promotion]
                                  ,[Title]
                                  ,[ThirdParty]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[Is_Deleted]
                              FROM Tuhu_productcatalog..[CompetingProductsMonitor] WITH (NOLOCK)
                              WHERE Pid=@Pid 
                                    AND Is_Deleted = 0
                              ORDER BY Price ASC;";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter ("@Pid",pid)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter).ConvertTo<CompetingProductsMonitorEntity>().ToList(); ;
        }

        /// <summary>
        /// 新增产品监控信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(CompetingProductsMonitorEntity model)
        {
            if (model == null)
                return -2;

            #region 判断渠道的产品监控是否存在
            using (var cmd = new SqlCommand())
            {
                switch (model.ShopCode)
                {
                    case "京东自营":
                        cmd.CommandText = "SELECT	1 FROM	Tuhu_productcatalog..CompetingProductsMonitor AS PPM WHERE	PPM.ShopCode = @ShopCode AND PPM.SkuID = @SkuID AND Is_Deleted=0";
                        cmd.Parameters.AddWithValue("@SkuID", model.SkuID);
                        break;
                    case "养车无忧":
                    case "康众官网":
                        cmd.CommandText = "SELECT	1 FROM	Tuhu_productcatalog..CompetingProductsMonitor AS PPM WHERE	PPM.ShopCode = @ShopCode AND PPM.ItemCode = @ItemCode AND Is_Deleted=0";
                        cmd.Parameters.AddWithValue("@ItemCode", model.ItemCode);
                        break;
                    case "汽配龙":
                        cmd.CommandText = "SELECT	1 FROM	Tuhu_productcatalog..CompetingProductsMonitor AS PPM WHERE	ShopCode = @ShopCode AND PPM.Pid = @Pid AND Is_Deleted=0";
                        cmd.Parameters.AddWithValue("@Pid", model.Pid);
                        break;
                    default:
                        cmd.CommandText = "SELECT	1 FROM	Tuhu_productcatalog..CompetingProductsMonitor AS PPM WHERE	PPM.ShopCode = @ShopCode AND PPM.ItemID = @ItemID AND Is_Deleted=0";
                        cmd.Parameters.AddWithValue("@ItemID", model.ItemID);
                        break;
                }
                cmd.Parameters.AddWithValue("@ShopCode", model.ShopCode);
                try
                {
                    if (Tuhu.Component.Common.DbHelper.ExecuteScalar(cmd) != null)
                        return -1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            #endregion

            #region 判断PID是否存在
            var displayName = DbHelper.ExecuteScalar(@"SELECT	C.DisplayName
                                                        FROM	Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH(NOLOCK)
                                                        JOIN	Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH(NOLOCK)
            		                                            ON C.#Catalog_Lang_Oid = CP.oid
                                                        WHERE	CP.Pid2=@Pid
            		                                            AND CP.i_ClassType IN (2, 4)"
                                    , CommandType.Text,
                                     new SqlParameter("Pid", model.Pid));

            if (displayName == null)
                return -5;
            #endregion

            #region 执行插入产品监控表
            using (var cmd = new SqlCommand(@"
                    INSERT INTO [Tuhu_productcatalog]..[CompetingProductsMonitor]
                           ([ShopCode]
                           ,[Pid]
                           ,[ItemID]
                           ,[SkuID]
                           ,[ItemCode]
                           ,[Price]
                           ,[Promotion]
                           ,[Title]
                           ,[ThirdParty])
      
                     VALUES
                           (@ShopCode
                           ,@Pid
                           ,@ItemID
                           ,@SkuID
                           ,@ItemCode
                           ,@Price
                           ,0
                           ,@Title
                           ,1)"))
            {
                cmd.Parameters.AddWithValue("@ShopCode", model.ShopCode);
                cmd.Parameters.AddWithValue("@Pid", model.Pid.Trim());
                if (model.ShopCode == "养车无忧" || model.ShopCode == "康众官网")
                {
                    cmd.Parameters.AddWithValue("@ItemID", 0);
                    cmd.Parameters.AddWithValue("@SkuID", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ItemID", model.ItemID);
                    cmd.Parameters.AddWithValue("@SkuID", model.SkuID);
                }
                cmd.Parameters.AddWithValue("@Price", model.Price);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                if (model.ShopCode == "养车无忧" || model.ShopCode == "康众官网")
                    cmd.Parameters.AddWithValue("@ItemCode", model.ItemCode);
                //else if (model.ShopCode == "汽配龙")
                //    cmd.Parameters.AddWithValue("@ItemCode", model.Pid);
                else
                    cmd.Parameters.AddWithValue("@ItemCode", "");

                return DbHelper.ExecuteNonQuery(cmd);
            }
            #endregion

        }

        /// <summary>
        /// 根据pkid删除竞品监控信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public int Delete(long pkid)
        {
            using (var cmd = new SqlCommand(@"
                                               UPDATE  [Tuhu_productcatalog]..[CompetingProductsMonitor]
                                                SET     LastUpdateDateTime = GETDATE() ,
                                                        [Is_Deleted] = 1
                                                WHERE   PKID = @PKID;"))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return DbHelper.ExecuteNonQuery(cmd);

            }
        }
        /// <summary>
        /// 根据pid、shopCode、itemId删除竞品监控信息
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="shopCode"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int Delete(string pid, string shopCode, string itemId)
        {
            using (var cmd = new SqlCommand("UPDATE	Tuhu_productcatalog..CompetingProductsMonitor SET Is_Deleted=1 WHERE	ThirdParty = 1 AND Pid=@Pid AND ShopCode = @ShopCode AND "))
            {
                cmd.Parameters.AddWithValue("@Pid", pid);
                cmd.Parameters.AddWithValue("@ShopCode", shopCode);
                long itemCode;
                if (long.TryParse(itemId, out itemCode))
                {
                    cmd.CommandText += "(ItemID > 0 AND ItemID = @ItemID OR SkuID > 0 AND SkuID = @ItemID)";
                    cmd.Parameters.AddWithValue("@ItemID", itemCode);
                }
                else
                {
                    cmd.CommandText += "ItemCode = @itemCode";
                    cmd.Parameters.AddWithValue("@itemCode", itemId);
                }

                return Tuhu.Component.Common.DbHelper.ExecuteNonQuery(cmd);
            }
        }
    }
}
