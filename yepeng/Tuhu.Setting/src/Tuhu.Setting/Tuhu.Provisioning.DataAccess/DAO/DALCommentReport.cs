using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALCommentReport
    {

        public static DataTable GetCommentCount(DateTime? startDate, DateTime? endDate)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("Gungnir.[dbo].[Comment_GetCommentCount]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        public static DataTable GetTousuType(int level)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("Gungnir.[dbo].[Tousu_GetTousuType]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Level", level);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        public static DataTable SelectAdditionProductCommentByOrderId(int PageIndex,
            int PageSize, int Status, int? OrderId, int AutoApproveStatus = 0)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@Status", Status),
                    new SqlParameter("@orderid", OrderId),
                    new SqlParameter("@begin", (PageIndex-1)*PageSize),
                    new SqlParameter("@step",PageSize),
                    new SqlParameter("@approve",AutoApproveStatus)
                };
                return dbHelper.ExecuteDataTable(@"SELECT  TAC.AdditionCommentId ,
        TAC.CommentId ,
        TAC.AdditionCommentContent ,
        TAC.AdditionCommentImages ,
        TAC.AdditionCommentStatus ,
        TAC.CreateTime ,
        TAC.UpdateTime ,
        TAC.AutoApproveStatus
FROM    Gungnir..tbl_AdditionComment AS TAC WITH ( NOLOCK )
        LEFT JOIN Gungnir..tbl_Comment AS TCT WITH ( NOLOCK ) ON TAC.CommentId = TCT.CommentId
WHERE   IsDel = 0
        AND ( @approve = 0
              AND TAC.AutoApproveStatus != -1
              OR @approve = 3
              AND TAC.AutoApproveStatus = 0
              OR TAC.AutoApproveStatus = @approve
            )
        AND ( @Status = 0
              OR @Status = TAC.AdditionCommentStatus
            )
        AND ( @orderid IS NULL
              OR TCT.CommentOrderId = @orderid
            )
ORDER BY TAC.CreateTime DESC
        OFFSET @begin ROWS FETCH NEXT @step ROWS ONLY;", CommandType.Text,
                    sqlPrams);
            }
        }

        public static int SelectAdditionProductCommentCountByOrderId(int Status, int? OrderId, int AutoApproveStatus = 0)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@Status", Status),
                    new SqlParameter("@orderid", OrderId),
                    new SqlParameter("@approve",AutoApproveStatus)
                };
                return Convert.ToInt32(dbHelper.ExecuteScalar(@"SELECT  COUNT(1)
FROM    Gungnir..tbl_AdditionComment AS TAC WITH ( NOLOCK )
        LEFT JOIN Gungnir..tbl_Comment AS TCT WITH ( NOLOCK ) ON TAC.CommentId = TCT.CommentId
WHERE   IsDel = 0
        AND ( @approve = 0
              AND TAC.AutoApproveStatus != -1
              OR @approve = 3
              AND TAC.AutoApproveStatus = 0
              OR TAC.AutoApproveStatus = @approve
            )
        AND ( @Status = 0
              OR @Status = TAC.AdditionCommentStatus
            )
        AND ( @orderid IS NULL
              OR TCT.CommentOrderId = @orderid
            );", CommandType.Text,
                    sqlPrams));
            }
        }
        /// <summary>
        /// 根据创建时间查询评论信息
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static DataTable GetCommentByCreateTime(DateTime startDate, DateTime endDate)
        {
            #region SQL
            string sql = @"SELECT  TOP 100000
                                    CASE WHEN CommentChannel IS NULL
                                          OR CommentChannel = 0 THEN N'PC端'
                                     ELSE N'手机端'
                                    END AS N'评论来源' ,
                                    CommentUserName AS N'用户名称' ,
                                    CommentContent AS N'评论内容' ,
                                    CASE c.CommentStatus
                                      WHEN 0 THEN N'审核不通过'
                                      WHEN 1 THEN N'未审核'
                                      WHEN 2 THEN N'审核通过'
                                      WHEN 3 THEN N'仅打分'
                                    END AS N'审核状态' ,
                                    CommentProductId AS N'评论的产品ID' ,
                                    ISNULL(vwp.DisplayName, '') AS N'评论的产品名称' ,
                                    CommentOrderId AS N'订单ID' ,
                                    ISNULL(Q.CarparName, '') AS N'评论的门店' ,
                                    c.CreateTime AS N'创建时间' ,
                                    ISNULL(c.CommentR1, '') AS R1 ,
                                    ISNULL(c.CommentR2, '') AS R2 ,
                                    ISNULL(c.CommentR3, '') AS R3 ,
                                    ISNULL(c.CommentR4, '') AS R4 ,
                                    ISNULL(c.CommentR5, '') AS R5 ,
                                    ISNULL(c.CommentR6, '') AS R6 ,
                                    ISNULL(c.CommentR7, '') AS R7 ,
                                    ISNULL(s.FirstTousuType, '') AS N'投诉类型'
                            FROM    Gungnir..tbl_Comment AS c WITH ( NOLOCK )
                                    LEFT JOIN Gungnir..tbl_CommentTousu AS s WITH ( NOLOCK ) ON s.CommentId = c.CommentId
                                    LEFT JOIN Gungnir..vw_tbl_Order AS P WITH ( NOLOCK ) ON c.CommentOrderId = P.PKID
                                    LEFT JOIN Gungnir..Shops AS Q WITH ( NOLOCK ) ON P.InstallShopID = Q.PKID
                                    LEFT JOIN Tuhu_productcatalog..vw_Products AS vwp WITH ( NOLOCK ) ON c.CommentProductId = vwp.PID COLLATE Chinese_PRC_CI_AS
                            WHERE   CommentType = 1
                                    AND CreateTime >= @CreateDate
                                    AND CreateTime <= @EndDate;";
            #endregion

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@CreateDate",startDate),
                    new SqlParameter("@EndDate", endDate)
                };
                return dbHelper.ExecuteDataTable(sql, CommandType.Text,sqlPrams);
            }
        }
    }
}
