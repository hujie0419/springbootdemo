using Dapper;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO.ThirdReplaceOrder
{
    public static class DALThirdReplaceOrder
    {
        public static List<OrderLists> SelectNeedSendOrder(SqlConnection conn, DateTime startTime, DateTime endTime, string orderType)
        {
            const string sqlOne = @"
            SELECT DISTINCT
                    O.PKID ,
                    O.OrderType ,
                    O.OrderNo ,
                    O.OrderDatetime ,
                    O.Status ,
                    O.PurchaseStatus ,
                    O.PayStatus
            FROM    Gungnir..tbl_Order AS O ( NOLOCK )
                    LEFT JOIN Gungnir..tbl_OrderList AS ol ( NOLOCK ) ON O.PKID = ol.OrderID
            WHERE   O.OrderType = N'12加油卡'
                    AND O.Status <> '7Canceled'
                    AND O.PayStatus = '2paid'
                    AND ( O.PurchaseStatus = 1
                          OR O.PurchaseStatus = 0
                        )
                    AND O.OrderDatetime >= @StartTime
                    AND O.OrderDatetime <= @EndTime
                    AND NOT EXISTS ( SELECT 1
                                     FROM   Gungnir_Finance..tbl_DebitReturnInfo AS dri ( NOLOCK )
                                     WHERE  dri.VenderID IN ( 6054, 6426 )
                                            AND dri.OrderListID = ol.PKID )
            ORDER BY O.OrderDatetime DESC";

            const string sqlTwo = @"
            SELECT DISTINCT
                    O.PKID ,
                    O.OrderType ,
                    O.OrderNo ,
                    O.OrderDatetime ,
                    O.Status ,
                    O.PurchaseStatus ,
                    O.PayStatus
            FROM    Gungnir..tbl_Order AS O ( NOLOCK )
                    LEFT JOIN Gungnir..tbl_OrderList AS ol ( NOLOCK ) ON O.PKID = ol.OrderID
            WHERE   O.OrderType = N'11违章代缴'
                    AND O.Status <> '7Canceled'
                    AND O.OrderDatetime >= @StartTime
                    AND O.OrderDatetime <= @EndTime
                    AND NOT EXISTS ( SELECT 1
                                     FROM   Gungnir_Finance..tbl_DebitReturnInfo AS dri ( NOLOCK )
                                     WHERE  dri.VenderID = 6242
                                            AND dri.OrderListID = ol.PKID )
            ORDER BY O.OrderDatetime DESC";

            var sql = sqlOne;
            if (orderType == "11违章代缴")
                sql = sqlTwo;

            SqlParameter[] parameter =
            {
                new SqlParameter("@StartTime", startTime),
                new SqlParameter("@EndTime", endTime),
                new SqlParameter("@OrderType", orderType)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter).ConvertTo<OrderLists>().ToList();
        }

        public static int IsSendOrderPayNotice(SqlConnection conn, long tuhuOrderId)
        {
            const string sql = @"SELECT COUNT(1) FROM Tuhu_log..ThirdPartyOrderSubmitLog (NOLOCK) WHERE Result=1 AND OrderType IN (N'12加油卡',N'11违章代缴') AND TuhuOrderId=@TuhuOrderId";
            return Convert.ToInt32(conn.ExecuteScalar(sql, new { TuhuOrderId = tuhuOrderId }, commandType: CommandType.Text));
        }

        public static List<long> SelectSendOrderPayNoticeOrderIds(SqlConnection conn)
        {
            const string sql = @"SELECT TuhuOrderId FROM Tuhu_log..ThirdPartyOrderSubmitLog (NOLOCK) WHERE Result=1 AND OrderType IN (N'12加油卡',N'11违章代缴')";
            return conn.Query<long>(sql, commandType: CommandType.Text).ToList();
        }

        public static string SelectSerialNumByTuhuOrderId(SqlConnection conn, long tuhuOrderId)
        {
            const string sql = @"SELECT  TOP 1 SerialNumbers FROM    Gungnir..tbl_OrderSerialNumbers (NOLOCK) WHERE OrderID=@TuhuOrderId AND Status='Success' ORDER BY LastUpdateTime DESC";
            return Convert.ToString(conn.ExecuteScalar(sql, new { TuhuOrderId = tuhuOrderId }, commandType: CommandType.Text));
        }

        public static string SelectCheXingYiIdByTuhuOrderId(SqlConnection conn, long tuhuOrderId)
        {
            const string sql = @"SELECT TOP 1  JuheOrderNum FROM Tuhu_order..tbl_JuhePeccancyOrder (NOLOCK) WHERE OrderId=@TuhuOrderId ORDER BY CreateTime DESC";
            return Convert.ToString(conn.ExecuteScalar(sql, new { TuhuOrderId = tuhuOrderId }, commandType: CommandType.Text));
        }
    }
}
