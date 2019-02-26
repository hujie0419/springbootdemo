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
            string sql = @"select top 100000
       c.CommentId,
       case
           when CommentChannel is null
                or CommentChannel = 0 then
               N'PC端'
           else
               N'手机端'
       end as N'评论来源',
       CommentUserName as N'用户名称',
       CommentContent as N'评论内容',
       case c.CommentStatus
           when 0 then
               N'审核不通过'
           when 1 then
               N'未审核'
           when 2 then
               N'审核通过'
           when 3 then
               N'仅打分'
       end as N'审核状态',
       CommentProductId as N'评论的产品ID',
       ISNULL(vwp.DisplayName, '') as N'评论的产品名称',
       CommentOrderId as N'订单ID',
       ISNULL(Q.CarparName, '') as N'评论的门店',
       c.CreateTime as N'创建时间',
       ISNULL(c.CommentR1, '') as R1,
       ISNULL(c.CommentR2, '') as R2,
       ISNULL(c.CommentR3, '') as R3,
       ISNULL(c.CommentR4, '') as R4,
       ISNULL(c.CommentR5, '') as R5,
       ISNULL(c.CommentR6, '') as R6,
       ISNULL(c.CommentR7, '') as R7,
       ISNULL(s.FirstTousuType, '') as N'投诉类型'
from Gungnir..tbl_Comment as c with (nolock)
    left join Gungnir..tbl_CommentTousu as s with (nolock)
        on s.CommentId = c.CommentId
    left join Gungnir..vw_tbl_Order as P with (nolock)
        on c.CommentOrderId = P.PKID
    left join Gungnir..Shops as Q with (nolock)
        on P.InstallShopID = Q.PKID
    left join Tuhu_productcatalog..vw_Products as vwp with (nolock)
        on c.CommentProductId = vwp.PID collate Chinese_PRC_CI_AS
where CommentType = 1
      and CreateTime >= @CreateDate
      and CreateTime <= @EndDate;";
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

        public static List<Tuple<int,string>> GetTousuInfo(List<int> ids)
        {
            var sqlStr = $@"
select CommentId,
       ComplaintContent
from Gungnir..tbl_CommentTousu with (nolock)
where CommentId in ( {string.Join(",", ids)} )";
            using(var cmd=new SqlCommand(sqlStr))
            {
                var dt= DbHelper.ExecuteDataTable(cmd);
                var result = new List<Tuple<int, string>>();
                if (dt == null || dt.Rows.Count < 1) return result;
                foreach(DataRow item in dt.Rows)
                {
                    var content = item["ComplaintContent"].ToString();
                    int.TryParse(item["CommentId"]?.ToString(), out var id);
                    result.Add(Tuple.Create(id, content));
                }
                return result;
            }
        }

        public static List<Tuple<int,string,string,string>> GetTousuType()
        {
            const string sqlStr = @"
select DicText,
       TypeLevel,
       DicValue,
       DicType
from Gungnir..TousuType with (nolock)
where IsDelete = 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var dt = DbHelper.ExecuteDataTable(cmd);
                var result = new List<Tuple<int, string, string, string>>();
                if (dt == null || dt.Rows.Count < 1) return result;
                foreach (DataRow item in dt.Rows)
                {
                    int.TryParse(item["TypeLevel"]?.ToString(), out var level);
                    var value = item["DicValue"].ToString();
                    var text = item["DicText"].ToString();
                    var type = item["DicType"].ToString();
                    result.Add(Tuple.Create(level, value, text, type));
                }
                return result;
            }
        }
    }
}
