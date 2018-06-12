using Dapper;
using Microsoft.ApplicationBlocks.Data;
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
    public static class DALActivityBoardPower
    {
        /// <summary>
        /// 、查询用户权限信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userEmailStr"></param>
        /// <returns></returns>
        public static List<ActivityBoardPermissionConfig> SelectActivityBoardPowerConfig(SqlConnection conn, string userEmailStr)
        {
            const string sql = @" WITH   UserEmailList
          AS ( SELECT   *
               FROM     Configuration..SplitString(@UserEmailStr, ',', 1)
             )
            SELECT  *
            FROM    Configuration..ActivityBoardPermissionConfig AS abp WITH ( NOLOCK )
            JOIN UserEmailList AS ue ON abp.UserEmail = ue.Item;";
            return conn.Query<ActivityBoardPermissionConfig>(sql, new { UserEmailStr = userEmailStr }, commandType: CommandType.Text).ToList();
        }

        /// <summary>
        /// 查询用户权限信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public static List<ActivityBoardPermissionConfig> SelectPowerConfigByUserEmail(SqlConnection conn, string userEmail)
        {
            const string sql = @"
            SELECT  *
            FROM    Configuration..ActivityBoardPermissionConfig AS abp WITH ( NOLOCK ) WHERE UserEmail = @UserEmail";
            return conn.Query<ActivityBoardPermissionConfig>(sql, new { UserEmail = userEmail }, commandType: CommandType.Text).ToList();
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int SelectPowerConfigUserCount(SqlConnection conn)
        {
            const string sql = @"SELECT  COUNT(DISTINCT UserEmail) FROM    Configuration..ActivityBoardPermissionConfig WITH ( NOLOCK )";
            return (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
        }

        /// <summary>
        /// 查询用户权限
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userEmail"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<string> SelectActivityBoardPowerConfigUser(SqlConnection conn, string userEmail, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT  DISTINCT UserEmail 
        FROM    Configuration..ActivityBoardPermissionConfig WITH (NOLOCK)
        WHERE   ( @UserEmail = '' OR @UserEmail IS NULL
                  OR UserEmail = @UserEmail
                )
        ORDER BY UserEmail DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";
            return conn.Query<string>(sql, new { UserEmail = userEmail.Trim(), PageIndex = pageIndex, PageSize = pageSize }, commandType: CommandType.Text).ToList();
        }

        /// <summary>
        /// 根据用户账号和模块名称获取权限
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userEmail"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ActivityBoardPermissionConfig SelectPowerConfigByUserEmail(SqlConnection conn, string userEmail, ActivityBoardModuleType type)
        {
            const string sql = @"SELECT  * ,COUNT(1) OVER ( ) AS Total
        FROM    Configuration..ActivityBoardPermissionConfig WITH (NOLOCK)
        WHERE    UserEmail = @UserEmail AND ModuleType = @Type";
            return conn.Query<ActivityBoardPermissionConfig>(sql, new { UserEmail = userEmail.Trim(), Type = type.ToString() }, commandType: CommandType.Text).SingleOrDefault();
        }

        /// <summary>
        /// 添加权限信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int InsertActivityBoardPowerConfig(SqlConnection conn, ActivityBoardPermissionConfig data)
        {
            const string sql = @"INSERT  INTO Configuration..ActivityBoardPermissionConfig
                    ( UserEmail ,
                      IsVisible ,
                      VisibleActivityDays ,
                      InsertActivity ,
                      UpdateActivity ,
                      DeleteActivity ,
                      ViewActivity ,
                      ActivityEffect ,
                      ModuleType ,
                      CreatedTime
                    )           
            VALUES  ( @UserEmail ,
                      @IsVisible ,
                      @VisibleActivityDays ,
                      @InsertActivity ,
                      @UpdateActivity ,
                      @DeleteActivity ,
                      @ViewActivity ,
                      @ActivityEffect ,
                      @ModuleType ,
                      GETDATE()
                    )";
            return conn.Execute(sql, new
            {
                UserEmail = data.UserEmail.Trim(),
                IsVisible = data.IsVisible,
                VisibleActivityDays = data.VisibleActivityDays,
                InsertActivity = data.InsertActivity,
                UpdateActivity = data.UpdateActivity,
                DeleteActivity = data.DeleteActivity,
                ViewActivity = data.ViewActivity,
                ActivityEffect = data.ActivityEffect,
                ModuleType = data.ModuleType.ToString()
            }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 更新权限信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int UpdateActivityBoardPowerConfig(SqlConnection conn, ActivityBoardPermissionConfig data)
        {
            const string sql = @"UPDATE  Configuration..ActivityBoardPermissionConfig
            SET     IsVisible = @IsVisible ,
                    VisibleActivityDays = @VisibleActivityDays ,
                    InsertActivity = @InsertActivity ,
                    UpdateActivity = @UpdateActivity ,
                    DeleteActivity = @DeleteActivity ,
                    ViewActivity = @ViewActivity ,
                    ActivityEffect = @ActivityEffect ,
                    UpdatedTime = GETDATE() 
            WHERE UserEmail = @UserEmail AND ModuleType = @ModuleType";
            return conn.Execute(sql, new
            {
                UserEmail = data.UserEmail.Trim(),
                IsVisible = data.IsVisible,
                VisibleActivityDays = data.VisibleActivityDays,
                InsertActivity = data.InsertActivity,
                UpdateActivity = data.UpdateActivity,
                DeleteActivity = data.DeleteActivity,
                ViewActivity = data.ViewActivity,
                ActivityEffect = data.ActivityEffect,
                ModuleType = data.ModuleType.ToString()
            }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 删除权限信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public static int DeleteActivityBoardPowerConfig(SqlConnection conn, string userEmail)
        {
            var sql = @"DELETE FROM  Configuration..ActivityBoardPermissionConfig  WHERE   UserEmail = @UserEmail";

            return conn.Execute(sql, new { UserEmail = userEmail }, commandType: CommandType.Text);
        }
    }
}
