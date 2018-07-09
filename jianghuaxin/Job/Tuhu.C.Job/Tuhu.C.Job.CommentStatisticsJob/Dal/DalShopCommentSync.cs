using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.C.Job.CommentStatisticsJob.Model;

namespace Tuhu.C.Job.CommentStatisticsJob.Dal
{
    public static class DalShopCommentSync
    {
        public static int GetShopCommentCount(string startTime, string endTime)
        {
            const string sqlStr = @"
SELECT  COUNT(1)
FROM    Tuhu_comment.dbo.tbl_ShopComment WITH ( NOLOCK )
WHERE   UpdateTime > @startTime
        AND UpdateTime < @endTime
        AND AuditPerson IS NOT NULL
        AND CommentStatus = 2
        AND CommentType < 5";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);
                var result = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }
        
        public static List<int> GetShopCommentId(string startTime, string endTime, int start, int step)
        {
            const string sqlStr = @"
SELECT  DISTINCT
        CommentId
FROM    Tuhu_comment.dbo.tbl_ShopComment WITH ( NOLOCK )
WHERE   UpdateTime > @startTime
        AND UpdateTime < @endTime
        AND AuditPerson IS NOT NULL
        AND CommentStatus = 2
        AND CommentType < 5
ORDER BY CommentId
        OFFSET @start ROWS FETCH NEXT @step ROWS ONLY";

            List<int> GetCommentIdAction(DataTable dt)
            {
                var result = new List<int>();
                if (dt == null || dt.Rows.Count < 1) return result;
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var value = dt.Rows[i].GetValue<int>("CommentId");
                    if (value > 0) result.Add(value);
                }
                return result;
            }

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@step", step);
                return DbHelper.ExecuteQuery(cmd, GetCommentIdAction);
            }
        }
        public static IEnumerable<ShopCommentDataModel> GetShopCommentOrder(int pageIndex, int pageSize, int shopId) {
            using (var cmd = new SqlCommand($@"SELECT InstallShopID AS ShopId,CommentOrderId AS OrderId FROM Tuhu_comment..tbl_ShopComment WITH(NOLOCK)
                                            WHERE InstallShopID={shopId} AND CommentStatus IN (2,3)
                                            ORDER BY OrderId ASC
                                            OFFSET {(pageIndex-1)*pageSize} ROW
                                            FETCH NEXT {pageSize} ROWS ONLY")) {
                return DbHelper.ExecuteSelect<ShopCommentDataModel>(true, cmd);
            }
        }

        /// <summary>
        /// 分页 查询 门店的 评论
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IEnumerable<ShopCommentDataModel> GetShopCommentOrder(int pageIndex, int pageSize)
        {
            using (var cmd = new SqlCommand($@"SELECT CommentId,InstallShopID AS ShopId,CommentOrderId AS OrderId FROM Tuhu_comment..tbl_ShopComment WITH(NOLOCK)
                                            ORDER BY CommentId ASC
                                            OFFSET {(pageIndex - 1) * pageSize} ROW
                                            FETCH NEXT {pageSize} ROWS ONLY"))
            {
                return DbHelper.ExecuteSelect<ShopCommentDataModel>(true, cmd);
            }
        }

        public static IEnumerable<ShopProductOrderModel> GetShopOrderProduct(int orderId) {
            using (var cmd = new SqlCommand($@"SELECT PID,OrderID FROM Gungnir..tbl_OrderList AS L WITH(NOLOCK)
		                                            WHERE L.OrderID=@OrderId"))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                return DbHelper.ExecuteSelect<ShopProductOrderModel>(true, cmd);
            }
        }
        public static IEnumerable<ShopProductOrderModel> GetShopProductOrder(int shopId,string pid) {
            using (var cmd = new SqlCommand($@"
                                            SELECT CommentId,PID,OrderID,O.InstallShopID AS ShopId 
                                            FROM Gungnir..tbl_OrderList AS OL WITH(NOLOCK) 
                                            JOIN Tuhu_comment..tbl_ShopComment AS O ON OL.OrderID=O.CommentOrderId
                                            WHERE O.InstallShopID=@ShopId AND PID=@Pid AND o.CommentStatus IN(2,3)"))
            {
                cmd.Parameters.AddWithValue("@ShopId", shopId);
                cmd.Parameters.AddWithValue("@Pid",pid);
                return DbHelper.ExecuteSelect<ShopProductOrderModel>(true, cmd);
            }
        }

        public static int GetShopCommentOrderCount(int shopId) {
            using (var cmd = new SqlCommand($@"SELECT COUNT(1) FROM Tuhu_comment..tbl_ShopComment WITH(NOLOCK)
                                            WHERE InstallShopID=@ShopId AND CommentStatus  IN (2,3)"))
            {
                cmd.Parameters.AddWithValue("@ShopId", shopId);
                return (int)DbHelper.ExecuteScalar(true, cmd);
            }
        }

        public static IEnumerable<ShopCommentStatisticsModel> GetShopCommentStatistics(int shopId,string pid, IEnumerable<int> commentIds) {
            using (var cmd = new SqlCommand($@"
                                            SELECT  CAST(AVG(CAST(S.CommentR1 AS DECIMAL(4,2))) AS DECIMAL(4,2)) AS CommentAvgScore,
                                            COUNT(1) AS CommentCount,@Pid AS Pid,s.InstallShopID AS ShopId 
                                            FROM Tuhu_comment..tbl_ShopComment AS S WITH(NOLOCK) 
                                            WHERE S.InstallShopID=@ShopId  AND S.CommentStatus IN (2,3)
                                            AND S.CommentId IN(SELECT Item FROM Tuhu_comment.dbo.SplitString(@CommentIds,',',1))
                                            GROUP BY s.InstallShopID"))
            {
                cmd.Parameters.AddWithValue("@Pid",pid);
                cmd.Parameters.AddWithValue("@ShopId",shopId);
                cmd.Parameters.AddWithValue("@CommentIds", string.Join(",", commentIds));
                return DbHelper.ExecuteSelect<ShopCommentStatisticsModel>(true, cmd);
            }
        }

        public static int SyncShopCommentStatistics(int shopId, ShopCommentStatisticsModel model)
        {
            using (var cmd = new SqlCommand($@"MERGE INTO Tuhu_comment..ShopCommentStatistics AS T
USING (SELECT @Pid AS Pid,@ShopId AS ShopId) AS S
ON S.Pid=T.Pid AND S.ShopId=T.ShopId
WHEN MATCHED
THEN UPDATE SET T.CommentAvgScore=@CommentAvgScore,T.CommentCount=@CommentCount,T.LastUpdateTime=GETDATE()
WHEN NOT MATCHED BY TARGET
THEN INSERT (Pid,ShopId,CommentAvgScore,CommentCount,CreateTime,LastUpdateTime) VALUES(S.Pid,S.ShopId,@CommentAvgScore,@CommentCount,GETDATE(),GETDATE())
;"))
            {
                cmd.Parameters.AddWithValue("@Pid", model.Pid);
                cmd.Parameters.AddWithValue("@ShopId", shopId);
                cmd.Parameters.AddWithValue("@CommentAvgScore", model.CommentAvgScore);
                cmd.Parameters.AddWithValue("@CommentCount", model.CommentCount);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static IEnumerable<TechCommentStatisticsModel> GetTechCommentStatistics(int techId)
        {
            using (var cmd = new SqlCommand($@"SELECT AVG(S.Score) AS CommentAvgScore,COUNT(1) AS CommentCount,C.InstallShopID AS ShopId,S.TechnicianId FROM 
                                                Tuhu_comment..TechnicianComment AS S WITH(NOLOCK)
												JOIN Tuhu_comment..tbl_ShopComment AS C WITH(NOLOCK)
												ON S.OrderId=C.CommentOrderId
                                                JOIN (SELECT MAX(PKID) AS PKID 
                                                FROM Tuhu_comment..TechnicianComment WHERE TechnicianId=@TechnicianId GROUP BY OrderId) AS R
                                                ON S.PKID=R.PKID
												WHERE S.TechnicianId=@TechnicianId AND c.CommentStatus IN (2,3)
                                                GROUP BY C.InstallShopID,S.TechnicianId"))
            {
                cmd.Parameters.AddWithValue("@TechnicianId", techId);
                return DbHelper.ExecuteSelect<TechCommentStatisticsModel>(true, cmd);
            }
        }

        public static int SyncTechCommentStatistics(int shopId,int technicianId, TechCommentStatisticsModel model)
        {
            using (var cmd = new SqlCommand($@"MERGE INTO Tuhu_comment..TechnicianStatistics AS T
USING (SELECT @TechnicianId AS TechnicianId,@ShopId AS ShopId) AS S
ON S.TechnicianId=T.TechnicianId AND S.ShopId=T.ShopId
WHEN MATCHED
THEN UPDATE SET T.CommentAvgScore=@CommentAvgScore,T.CommentCount=@CommentCount,T.LastUpdateTime=GETDATE()
WHEN NOT MATCHED BY TARGET
THEN INSERT (TechnicianId,ShopId,CommentAvgScore,CommentCount,CreateTime,LastUpdateTime) VALUES(S.TechnicianId,S.ShopId,@CommentAvgScore,@CommentCount,GETDATE(),GETDATE())
;"))
            {
                cmd.Parameters.AddWithValue("@TechnicianId", technicianId);
                cmd.Parameters.AddWithValue("@ShopId", shopId);
                cmd.Parameters.AddWithValue("@CommentAvgScore", model.CommentAvgScore);
                cmd.Parameters.AddWithValue("@CommentCount", model.CommentCount);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }



        #region 门店产品评分统计

        public static List<string> GetProductInfo()
        {
            const string sqlStr = @"
select ProdcutId
from [Tuhu_groupon].[dbo].[SE_MDBeautyCategoryProductConfig] with (nolock)
where IsDisable = 0;";
            using (var dbHelper = DbHelper.CreateDbHelper("Tuhu_Groupon_ReadOnly"))
            using (var cmd = new SqlCommand(sqlStr))
            {
                return dbHelper.ExecuteQuery(cmd, dt => dt.ToList<string>())?.ToList() ?? new List<string>();
            }
        }

        public static List<ProductScoreModel> GetProductScore(string pid)
        {
            const string sqlStr = @"
select T.InstallShopID as ShopId,
       COUNT(*) as CommentCount,
       AVG(T.CommentR1) as AvgScore,
       @pid as Pid
from Tuhu_comment..tbl_ShopComment as T with (nolock)
    inner join Gungnir..vw_tbl_OrderList as S with (nolock)
        on T.CommentOrderId = S.OrderID
           and S.PID = @pid
where T.CommentStatus in ( 2, 3 )
      and T.CommentType = 1
      and T.ParentComment = 0
      and T.InstallShopID > 0
      and T.CommentR1 is not null
group by T.InstallShopID;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                return DbHelper.ExecuteSelect<ProductScoreModel>(true, cmd)?.ToList() ?? new List<ProductScoreModel>();
            }
        }

        public static bool CreatOrUpdateRecord(ProductScoreModel info)
        {
            const string sqlStr = @"
update Tuhu_comment..ProductScoreStatistics with (rowlock)
set CommentCount = @count,
    CommentAvgScore = @avgScore,
    LastUpdateTime = GETDATE()
where ShopId = @shopId
      and Pid = @pid;";
            const string sqlStr2 = @"
insert Tuhu_comment..ProductScoreStatistics
(
    ShopId,
    Pid,
    CommentCount,
    CommentAvgScore
)
values
(@shopId, @pid, @count, @avgScore);";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@count", info.CommentCount);
                cmd.Parameters.AddWithValue("@avgScore", info.AvgScore);
                cmd.Parameters.AddWithValue("@shopId", info.ShopId);
                cmd.Parameters.AddWithValue("@pid", info.Pid);
                var result = DbHelper.ExecuteNonQuery(cmd);
                if (result < 1)
                {
                    cmd.CommandText = sqlStr2;
                    return DbHelper.ExecuteNonQuery(cmd) > 0;
                }

                return false;
            }
        }
        #endregion

        #region 评论 分类

        /// <summary>
        /// 根据时间查询 所有的门店的 评论 的数目
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetShopCommentCountByTime(string startTime, string endTime,int CommentType=1)
        {
            const string sqlStr = @"
                                    SELECT  COUNT(1)
                                    FROM    Tuhu_comment.dbo.tbl_ShopComment WITH ( NOLOCK )
                                    WHERE   CreateTime > @startTime
                                            AND CommentType = @CommentType
                                            AND CreateTime < @endTime";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);
                cmd.Parameters.AddWithValue("@CommentType", CommentType);
                var result = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }

        /// <summary>
        /// 分页 和 时间 查询 门店的 评论
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static IEnumerable<ShopCommentDataModel> GetShopCommentsPage(int pageIndex, int pageSize, string startTime, string endTime, int CommentType = 1)
        {
            using (var cmd = new SqlCommand($@"SELECT CommentId,InstallShopID AS ShopId,CommentOrderId AS OrderId,CommentStatus as [Status],CreateTime as CreateTime
                                                FROM Tuhu_comment.dbo.tbl_ShopComment WITH(nolock)
                                                where  CreateTime > @startTime
                                                AND CreateTime < @endTime
                                                AND CommentType = @CommentType
                                                ORDER BY CommentId ASC
                                                OFFSET {(pageIndex - 1) * pageSize} ROW
                                                FETCH NEXT {pageSize} ROWS ONLY"))
            {
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);
                cmd.Parameters.AddWithValue("@CommentType", CommentType);
                return DbHelper.ExecuteSelect<ShopCommentDataModel>(true, cmd);
            }
        }

        /// <summary>
        /// 更新门店评论的 类型
        /// </summary>
        /// <param name="CommentId">评论id</param>
        /// <param name="ShopType">门店 的 分类</param>
        /// <returns></returns>
        public static bool UpdateShopCommentShopType(int CommentId, string ShopType)
        {
            const string sqlStr = @"
                                    update Tuhu_comment.dbo.tbl_ShopComment 
                                    set ShopTypeBU = @ShopType ,UpdateTime=GETDATE()
                                    where CommentId = @CommentId;
                                ";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@CommentId", CommentId);
                cmd.Parameters.AddWithValue("@ShopType", ShopType);
                var result = DbHelper.ExecuteNonQuery(cmd);
                return result > 0;
            }
        }

        #endregion

    }
}
