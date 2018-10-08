using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;

namespace Tuhu.Provisioning.DataAccess.DAO.ActivityBoard
{
    public static class DALThirdPartActivity
    {
        /// <summary>
        /// 获取第三方活动信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="activityType"></param>
        /// <returns></returns>
        public static List<ThirdPartActivity> SelectThirdPartActivity(SqlConnection conn, int pageIndex, int pageSize, DateTime? startTime, DateTime? endTime, int activityType,string channel)
        {
            var sql = @"SELECT  * ,
            COUNT(1) OVER ( ) AS Total
            FROM    Configuration..ThirdPartActivity WITH ( NOLOCK )
            WHERE   ( @ActivityType = 6
                      OR ActivityType = @ActivityType
                    )
                    AND ( @StartTime IS NULL
                          OR StartTime >= @StartTime
                        )
                    AND ( @EndTime IS NULL
                          OR EndTime <= @EndTime
                        )
                AND ( @Channel = ''
                      OR @Channel IS NULL
                      OR Channel = @Channel
                    )
            ORDER BY PKID DESC
                    OFFSET @PageSize * ( @PageIndex - 1 ) ROWS FETCH NEXT @PageSize ROWS
                    ONLY;";
            return conn.Query<ThirdPartActivity>(sql, new
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                StartTime = startTime,
                EndTime = endTime,
                ActivityType = activityType,
                Channel = channel
            }, commandType: CommandType.Text).ToList();
        }

        /// <summary>  
        /// 根据PKID获取活动信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static ThirdPartActivity SelectThirdPartActivityByPKID(SqlConnection conn, int pkid)
        {
            var sql = @"SELECT  *  FROM  Configuration..ThirdPartActivity WITH ( NOLOCK )  WHERE   PKID = @PKID ";
            return conn.Query<ThirdPartActivity>(sql, new { PKID = pkid }, commandType: CommandType.Text).SingleOrDefault();
        }
        
        /// <summary>
        /// 添加活动信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertThirdPartActivity(SqlConnection conn, ThirdPartActivity model)
        {
            var sql = @"
            INSERT  INTO Configuration..ThirdPartActivity
                    ( ActivityType ,
                      ActivityName ,
                      H5Url ,
                      WebUrl ,
                      ActivityRules ,
                      StartTime ,
                      EndTime ,
                      Owner ,
                      Channel ,
                      CreatedTime
                    )
            OUTPUT Inserted.PKID
            VALUES  ( @ActivityType ,
                      @ActivityName ,
                      @H5Url ,
                      @WebUrl ,
                      @ActivityRules ,
                      @StartTime ,
                      @EndTime ,
                      @Owner ,
                      @Channel ,
                      GETDATE()
                    );";

            return Convert.ToInt32(conn.ExecuteScalar(sql, new
            {
                ActivityType = model.ActivityType,
                ActivityName = model.ActivityName,
                H5Url = model.H5Url,
                WebUrl = model.WebUrl,
                ActivityRules = model.ActivityRules,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Owner = model.Owner,
                Channel = model.Channel
            }, commandType: CommandType.Text));
        }

        /// <summary>
        /// 更新活动信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int UpdateThirdPartActivity(SqlConnection conn, ThirdPartActivity model)
        {
            var sql = @"UPDATE  Configuration..ThirdPartActivity
                        SET     ActivityName = @ActivityName ,
                                ActivityType = @ActivityType ,
                                H5Url = @H5Url ,
                                WebUrl = @WebUrl ,
                                ActivityRules = @ActivityRules ,
                                StartTime = @StartTime ,
                                EndTime = @EndTime ,
                                Owner = @Owner ,
                                Channel = @Channel ,
                                UpdatedTime = GETDATE()
                        WHERE   PKID = @PKID";

            return conn.Execute(sql, new
            {
                PKID = model.PKID,
                ActivityType = (int)model.ActivityType,
                ActivityName = model.ActivityName,
                H5Url = model.H5Url,
                WebUrl = model.WebUrl,
                ActivityRules = model.ActivityRules,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Owner = model.Owner,
                Channel = model.Channel
            }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 删除活动信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static int DeleteActivityByPKID(SqlConnection conn, int pkid)
        {
            var sql = @"DELETE FROM  Configuration..ThirdPartActivity  WHERE   PKID = @PKID";

            return conn.Execute(sql, new { PKID = pkid }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 根据岂止时间获取活动信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static List<ThirdPartActivity> SelectActivityForActivityBoard(SqlConnection conn, DateTime startTime, DateTime endTime, string title, string owner, string channel)
        {
            var sql = @" SELECT  *
        FROM    Configuration..ThirdPartActivity WITH ( NOLOCK )
        WHERE   EndTime >= @StartTime
                AND StartTime <= @EndTime
                AND ( @ActivityName = ''
                      OR @ActivityName IS NULL
                      OR ActivityName LIKE '%' + @ActivityName + '%'
                    )
                AND ( @Owner = ''
                      OR @Owner IS NULL
                      OR Owner LIKE '%' + @Owner + '%'
                    )
                AND ( @Channel = ''
                      OR @Channel IS NULL
                      OR Channel = @Channel
                    );";
            return conn.Query<ThirdPartActivity>(sql, new
            {
                StartTime = startTime,
                EndTime = endTime,
                ActivityName = title,
                Owner = owner,
                Channel = channel
            }, commandType: CommandType.Text).ToList();
        }
    }
}
