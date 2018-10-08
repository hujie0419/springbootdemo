using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Tuhu.C.Job.Initialization.Model;
using Common.Logging;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;

namespace Tuhu.C.Job.Initialization.Dal
{
    public static class DalTask
    {
        public static int GetFirstOrderUserCount()
        {
            const string sqlStr = @"SELECT  COUNT(UserId)
FROM    Gungnir..tbl_Order AS T WITH ( NOLOCK )
WHERE   ( T.InstallShopId IS NULL
          OR T.InstallShopId = 0
        )
        AND UserId IS NOT NULL
        AND T.DeliveryStatus = N'3.5Signed'
        OR T.InstallShopId > 0
        AND T.InstallStatus = N'2Installed';";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var result = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }
        public static List<OrderSimpleInfoModel> GetFirstOrderUserList(int start, int step)
        {
            const string sqlStr = @"SELECT  InstallShopId ,
        DeliveryStatus ,
        InstallStatus ,
        UserId
FROM    Gungnir..tbl_Order AS T WITH ( NOLOCK )
WHERE   PKID < @end
        AND PKID >= @start;";
            //List<Guid> Action(DataTable dt)
            //{
            //    var result = new List<Guid>();
            //    if (dt != null && dt.Rows.Count > 0)
            //    {
            //        for (var i = 0; i < dt.Rows.Count; i++)
            //        {
            //            var value = dt.Rows[i].GetValue<Guid>("UserId");
            //            if (value != Guid.Empty)
            //            {
            //                result.Add(value);
            //            }
            //        }
            //    }
            //    return result;
            //}
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", start + step);
                return DbHelper.ExecuteSelect<OrderSimpleInfoModel>(true, cmd)?.ToList() ?? new List<OrderSimpleInfoModel>();
            }
        }

        public static List<BindWxInfoModel> GetBindWxInfoList(int start, int step, bool flag = true)
        {

            if (flag)
            {
                const string sqlStr = @"SELECT  UserId ,
        AuthSource ,
        BindingStatus
FROM    tuhu_profiles..userauth WITH ( NOLOCK )
WHERE   PKID >= @start
        AND PKID < @end;";
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", start + step);
                    return DbHelper.ExecuteSelect<BindWxInfoModel>(cmd)?.ToList() ?? new List<BindWxInfoModel>();
                }
            }
            else
            {
                const string sqlStr = @"SELECT  UserId ,
        AuthSource ,
        BindingStatus ,
        UnionId
FROM    Tuhu_notification..WXUserAuth WITH ( NOLOCK )
WHERE   PKID >= @start
        AND PKID < @end;";
                using (var dbHelper = DbHelper.CreateLogDbHelper())
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", start + step);
                    return dbHelper.ExecuteSelect<BindWxInfoModel>(cmd)?.ToList() ?? new List<BindWxInfoModel>();
                }
            }

        }

        public static int GetCarUserCount()
        {
            const string sqlStr = @"SELECT COUNT(1) FROM Tuhu_profiles..CarObject WITH(NOLOCK) WHERE IsDeleted=0";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var result = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }
        public static List<UserCarInfoModel> GetCarUserList(Guid carId, int step)
        {
            const string sqlStr = @"SELECT TOP ( @step )
        u_user_id ,
        CarId ,
        u_cartype_pid_vid
FROM    Tuhu_profiles..CarObject WITH ( NOLOCK )
WHERE   CarId > @carid
        AND IsDeleted = 0
ORDER BY CarId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@carid", carId);
                cmd.Parameters.AddWithValue("@step", step);
                return DbHelper.ExecuteSelect<UserCarInfoModel>(true, cmd)?.ToList() ?? new List<UserCarInfoModel>();
            }
        }

        public static int GetAuthenticationUserCount()
        {
            const string sqlStr = @"SELECT  COUNT(1)
FROM    Tuhu_profiles..CarObject AS T WITH ( NOLOCK )
        INNER JOIN Tuhu_profiles..VehicleTypeCertificationInfo AS S WITH ( NOLOCK ) ON T.u_car_id = S.CarId
WHERE   S.Status = 1
        AND T.IsDeleted = 0
        AND T.u_cartype_pid_vid IS NOT NULL
        AND T.u_user_id IS NOT NULL;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var result = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }

        public static List<Guid> GetAuthenticationUserList(int start, int step)
        {
            const string sqlStr = @"SELECT  T.u_user_id
FROM    Tuhu_profiles..CarObject AS T WITH ( NOLOCK )
        INNER JOIN Tuhu_profiles..VehicleTypeCertificationInfo AS S WITH ( NOLOCK ) ON T.u_car_id = S.CarId
WHERE   S.Status = 1
        AND T.IsDeleted = 0
        AND T.u_cartype_pid_vid IS NOT NULL
        AND T.u_user_id IS NOT NULL
ORDER BY T.u_user_id
        OFFSET @start ROWS FETCH NEXT @step ROWS ONLY;";
            List<Guid> Action(DataTable dt)
            {
                var result = new List<Guid>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var value = dt.Rows[i].GetValue<Guid>("u_user_id");
                        if (value != Guid.Empty)
                        {
                            result.Add(value);
                        }
                    }
                }
                return result;
            }
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@step", step);
                return DbHelper.ExecuteQuery(true, cmd, Action);
            }
        }

        public static List<WXTaskInfoModel> GetWXTaskInfo(int start, int step)
        {
            const string sqlStr = @"SELECT UserId,BindType FROM [Tuhu_log].[dbo].[BindAuthSendIntergralRecord] WITH(NOLOCK)
WHERE PKID>=@start AND PKID<@end;";
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", start + step);
                return dbHelper.ExecuteSelect<WXTaskInfoModel>(cmd)?.ToList() ?? new List<WXTaskInfoModel>();
            }
        }

        //绑定微信和关注公众号Check数据
        public static List<Guid> CheckWxTaskUserId(List<Guid> userList, Guid taskId, ILog logger)
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new[] {
                new DataColumn("Id",typeof(Guid))
            });
            foreach (var item in userList)
            {
                var r = dt.NewRow();
                r[0] = item;
                dt.Rows.Add(r);
            }
            List<int> Action(DataTable datatable)
            {
                var result = new List<int>();
                if (datatable == null || datatable.Rows.Count <= 0) return result;
                for (var i = 0; i < datatable.Rows.Count; i++)
                {
                    var value = datatable.Rows[i].GetValue<int>("PKID");
                    if (value > 0)
                        result.Add(value);
                }
                return result;
            }

            List<Guid> Action2(DataTable datatable)
            {
                var result = new List<Guid>();
                if (datatable == null || datatable.Rows.Count <= 0) return result;
                for (var i = 0; i < datatable.Rows.Count; i++)
                {
                    var value = datatable.Rows[i].GetValue<Guid>("Id");
                    if (value != Guid.Empty)
                        result.Add(value);
                }
                return result;
            }

            const string sqlStr = @"DELETE S
OUTPUT Deleted.PKID
FROM @userList AS T LEFT JOIN Tuhu_profiles..tbl_UserTaskInfo AS S WITH(NOLOCK) ON T.Id=S.UserId AND S.TaskId=@taskId
WHERE S.TaskStatus<>2;";
//            const string sqlStr2 = @"DELETE  T
//FROM    @usertaskidList AS S
//        LEFT JOIN Tuhu_profiles..tbl_UserTaskDetailInfo AS T WITH ( NOLOCK ) ON S.IntId = T.UserTaskId;";
            const string sqlStr3 = @"SELECT T.Id
FROM @userList AS T LEFT JOIN Tuhu_profiles..tbl_UserTaskInfo AS S WITH(NOLOCK) ON T.Id=S.UserId AND S.TaskId=@taskId
WHERE S.PKID IS NULL;";
            List<int> dat;
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                var dtPara = cmd.Parameters.AddWithValue("@userList", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                dat = DbHelper.ExecuteQuery(cmd, Action);
                if (dat.Count > 0) logger.Warn($"删除脏数据{dat}条");
            }
            //if (dat.Count > 0)
            //{
            //    var dt2 = new DataTable();
            //    dt2.Columns.AddRange(new[] {
            //        new DataColumn("IntId",typeof(int))
            //    });
            //    foreach (var item in dat)
            //    {
            //        var r = dt2.NewRow();
            //        r[0] = item;
            //        dt2.Rows.Add(r);
            //    }
            //    using (var cmd = new SqlCommand(sqlStr2))
            //    {
            //        var dtPara = cmd.Parameters.AddWithValue("@usertaskidList", dt2);
            //        dtPara.SqlDbType = SqlDbType.Structured;
            //        dtPara.TypeName = "[IntIdsType] ";
            //        DbHelper.ExecuteNonQuery(cmd);
            //    }
            //}
            using (var cmd = new SqlCommand(sqlStr3))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                var dtPara = cmd.Parameters.AddWithValue("@userList", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                return DbHelper.ExecuteQuery(cmd, Action2);
            }
        }

        // 校验已存在的UserId
        public static List<Guid> CheckTaskUserId(List<Guid> userList, Guid taskId,ILog logger)
        {
            const string sqlStr2 = @"DELETE S
FROM @userList AS T LEFT JOIN Tuhu_profiles..tbl_UserTaskInfo AS S WITH(NOLOCK) ON T.Id=S.UserId AND S.TaskId=@taskId
WHERE S.TaskStatus=0";
            const string sqlStr = @"SELECT T.Id
FROM @userList AS T LEFT JOIN Tuhu_profiles..tbl_UserTaskInfo AS S WITH(NOLOCK) ON T.Id=S.UserId AND S.TaskId=@taskId
WHERE S.PKID IS NULL;";
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                    new DataColumn("Id",typeof(Guid))
                });
            foreach (var item in userList)
            {
                DataRow r = dt.NewRow();
                r[0] = item;
                dt.Rows.Add(r);
            }

            List<Guid> Action(DataTable datatable)
            {
                var result = new List<Guid>();
                if (datatable != null && datatable.Rows.Count > 0)
                {
                    for (var i = 0; i < datatable.Rows.Count; i++)
                    {
                        var value = datatable.Rows[i].GetValue<Guid>("Id");
                        if (value != Guid.Empty)
                            result.Add(value);
                    }
                }
                return result;
            }

            using (var cmd = new SqlCommand(sqlStr2))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                var dtPara = cmd.Parameters.AddWithValue("@userList", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                var dat=DbHelper.ExecuteNonQuery(cmd);
                if (dat > 0) logger.Warn($"删除脏数据{dat}条");
            }
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                var dtPara = cmd.Parameters.AddWithValue("@userList", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                return DbHelper.ExecuteQuery(cmd, Action);
            }
        }

        // 插入数据
        public static Tuple<int,int> InitUserTaskInfo(List<Guid> userList, Guid taskId, string actionName, ILog logger,int status=1)
        {

            int count1 = 0, count2 = 0;
            #region 插入用户任务信息表
            const string sqlStr = @"INSERT  INTO Tuhu_profiles..tbl_UserTaskInfo
        ( UserId ,
          TaskId ,
          TriggerTime ,
          TaskType ,
          EndTime ,
          ActionCount ,
          TaskStatus
        )
OUTPUT  Inserted.PKID
        SELECT  T.Id ,
                @taskId ,
                GETDATE() ,
                1 ,
                DATEADD(YEAR, 100, GETDATE()) ,
                1 ,
                @status
        FROM    @UserIds AS T;";
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                    new DataColumn("Id",typeof(Guid))
                });
            foreach (var item in userList)
            {
                DataRow r = dt.NewRow();
                r[0] = item;
                dt.Rows.Add(r);
            }

            List<int> Action(DataTable datatable)
            {
                var result = new List<int>();
                count1 = datatable.Rows.Count;
                if (datatable.Rows.Count > 0)
                {
                    for (var i = 0; i < datatable.Rows.Count; i++)
                    {
                        var value = datatable.Rows[i].GetValue<int>("PKID");
                        if (value > 0)
                            result.Add(value);
                    }
                }
                return result;
            }
            var userTaskId = new List<int>();
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                cmd.Parameters.AddWithValue("@status", status);
                var dtPara = cmd.Parameters.AddWithValue("@UserIds", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                userTaskId = DbHelper.ExecuteQuery(cmd, Action);
            }
            #endregion
            const string sqlStr2 = @"INSERT INTO Tuhu_profiles..tbl_UserTaskDetailInfo(UserTaskId,ActionName,CurrentCount,ExpectCount)
SELECT T.IntId,@actionName,1,1
FROM @usertaskIds AS T;";
            DataTable dt2 = new DataTable();
            dt2.Columns.AddRange(new DataColumn[] {
                    new DataColumn("IntId",typeof(Int32))
                });
            foreach (var item in userTaskId)
            {
                DataRow r = dt2.NewRow();
                r[0] = item;
                dt2.Rows.Add(r);
            }
            using (var cmd = new SqlCommand(sqlStr2))
            {
                cmd.Parameters.AddWithValue("@actionName", actionName);
                var dtPara = cmd.Parameters.AddWithValue("@usertaskIds", dt2);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[IntIdsType] ";
                count2 = DbHelper.ExecuteNonQuery(cmd);
            }
            return new Tuple<int, int>(count1, count2);
        }


        public static int GetWXBindUserCount()
        {
            const string sqlStr = @"
with tmp
as (select distinct
           UA.UserId
    from Tuhu_profiles..UserAuth as UA with (nolock)
        left join Tuhu_profiles..tbl_UserTaskInfo as S with (nolock)
            on UA.UserId = S.UserId
               and S.TaskId = '6cf07554-9b2e-4e18-9c73-5e15c16a801c'
    where UA.AuthSource = 'Weixin'
          and UA.BindingStatus = 'Bound'
          and S.PKID is null)
select COUNT(*)
from tmp;";
            using(var cmd=new SqlCommand(sqlStr))
            {
                var value = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(value?.ToString(), out var result);
                return result;
            }
        }

        public static List<Guid> GetWXBindUserList(int start,int step)
        {
            const string sqlStr = @"
with tmp
as (select distinct
           UA.UserId
    from Tuhu_profiles..UserAuth as UA with (nolock)
        left join Tuhu_profiles..tbl_UserTaskInfo as S with (nolock)
            on UA.UserId = S.UserId
               and S.TaskId = '6cf07554-9b2e-4e18-9c73-5e15c16a801c'
    where UA.AuthSource = 'Weixin'
          and UA.BindingStatus = 'Bound'
          and S.PKID is null)
select tmp.UserId
from tmp
order by tmp.UserId offset @start rows fetch next @step rows only;";
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@step", step);
                return DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var result = new List<Guid>();
                    if (dt == null || dt.Rows.Count < 1) return result;
                    for(var i = 0; i < dt.Rows.Count; i++)
                    {
                        var value = dt.Rows[i].GetValue<Guid>("UserId");
                        if (value != Guid.Empty) result.Add(value);
                    }
                    return result;
                });
            }
        }

    }
}
