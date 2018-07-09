using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity.ShopDistribution;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO.ShopDistribution
{
    public class ShopDistributionDAL
    {
        /// <summary>
        /// 获取商品的列表数据[车品类目下的商品]
        /// </summary>
        /// <param name="keyword">商品PID/商品标题</param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<SDProductModel> GetProductList(SqlConnection conn, string keyword, Pagination pagination)
        {
            string sql = @" SELECT  p.PID ,
                                    p.DisplayName ,
                                    p.Onsale
                            FROM    Tuhu_productcatalog..vw_Products AS p WITH ( NOLOCK )
                                    JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS ch WITH ( NOLOCK ) ON p.oid = ch.child_oid
                                    LEFT JOIN Configuration..ShopDistribution sd WITH ( NOLOCK ) ON sd.FKPID=p.pid  --过滤已添加项
                            WHERE   ch.NodeNo LIKE '28349.%'
                                    AND sd.PKID IS NULL
                                    AND ( p.PID = @Keyword OR p.DisplayName LIKE @Keyword2)
                            ORDER BY p.PID
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS  ONLY ";
            string sqlCount = @"SELECT  count(0)
                                FROM    Tuhu_productcatalog..vw_Products AS p WITH ( NOLOCK )
                                        JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS ch WITH ( NOLOCK ) ON p.oid = ch.child_oid
                                        LEFT JOIN Configuration..ShopDistribution sd WITH ( NOLOCK ) ON sd.FKPID=p.pid  --过滤已添加项
                                WHERE   ch.NodeNo LIKE '28349.%'
                                        AND sd.PKID IS NULL
                                        AND ( p.PID = @Keyword OR p.DisplayName LIKE @Keyword2)";

            var sqlParameters = new SqlParameter[]
                   {
                    new SqlParameter("@PageSize",pagination.rows),
                    new SqlParameter("@PageIndex",pagination.page),
                    new SqlParameter("@Keyword",keyword),
                    new SqlParameter("@Keyword2","%"+keyword+"%"),
                   };
            pagination.records = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount, sqlParameters);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<SDProductModel>().ToList();
        }

    }
}
