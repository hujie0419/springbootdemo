using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.CarsAffairs;

namespace Tuhu.Provisioning.DataAccess.DAO.CarsAffairs
{
    public static class DalCarsAffairs
    {
        /// <summary>
        /// 获取车务日志
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orderType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<CarsAffairsLog> GetCarsAffairs(SqlConnection conn, DateTime startTime,
            DateTime endTime, string orderType, int pageIndex, int pageSize)
        {
            const string sql = @"
             SELECT cf.PKID ,
                    cf.OrderNo ,
                    cf.PayStatus ,
                    cf.Remarks ,
                    cf.OrderType ,
                    cf.CreateTime ,
                    cf.UpdateTime ,
                    COUNT(1) OVER ( ) AS Total
             FROM   Tuhu_log..CarsAffairsLog AS cf WITH ( NOLOCK )
             WHERE  ( @StartTime = ''
                      OR @StartTime IS NULL
                      OR cf.CreateTime >= @StartTime
                    )
                    AND ( @EndTime = ''
                          OR @EndTie IS NULL
                          OR cf.CreateTime <= @EndTime
                        )
                    AND ( @OrderType = ''
                          OR @OrderType IS NULL
                          OR cf.OrderType = @OrderType
                        )
             ORDER BY cf.PKID DESC
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                    ONLY; ";
            return conn.Query<CarsAffairsLog>(sql, new
            {
                StartTime = startTime,
                EndTime = endTime,
                OrderType = orderType,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }
    }
}
