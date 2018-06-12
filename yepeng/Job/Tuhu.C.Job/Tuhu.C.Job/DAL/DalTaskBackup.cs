using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.DAL
{
    public class DalTaskBackup
    {
        /// <summary>
        /// 分批获取userId列表
        /// </summary>
        /// <param name="step"></param>
        /// <param name="maxGuid"></param>
        /// <returns></returns>
        public static List<Guid> GetUserList(int step,Guid maxGuid)
        {
            const string sqlStr = @"
select top (@step)
       UserID
from Tuhu_profiles..UserObject with (nolock)
where IsActive = 1
      and UserID > @max
order by UserID asc;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@step", step);
                cmd.Parameters.AddWithValue("@max", maxGuid);
                return DbHelper.ExecuteQuery(true, cmd, dt => dt.ToList<Guid>())?.ToList() ?? new List<Guid>();
            }
        }
        /// <summary>
        /// 备份已删除任务的记录
        /// </summary>
        /// <returns></returns>
        public static int BackupDeletedTask()
        {
            const string sqlStr = @"
insert into Tuhu_profiles..UserTaskInfo_temp
(
    pkid,
    userId,
    TaskId,
    TriggerTime,
    TaskType,
    EndTime,
    ActionCount,
    TaskStatus,
    CreateDateTime,
    LastUpdateDateTime,
    TaskStep
)
select T.PKID,
       T.UserId,
       T.TaskId,
       T.TriggerTime,
       T.TaskType,
       T.EndTime,
       T.ActionCount,
       T.TaskStatus,
       T.CreateDateTime,
       T.LastUpdateDateTime,
       99
from Tuhu_profiles..tbl_UserTaskInfo as T with (nolock)
    inner join Configuration..TaskConfigInfo as S with (nolock)
        on T.TaskId = S.TaskId
where S.IsDelete = 1";
            using(var cmd=new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 删除Tuhu_profiles..tbl_UserTaskInfo表数据
        /// </summary>
        /// <returns></returns>
        public static int DeleteData()
        {
            const string sqlStr = @"delete T
from Tuhu_profiles..tbl_UserTaskInfo as T with (rowlock)
    inner join Tuhu_profiles..UserTaskInfo_temp as S with (nolock)
        on T.PKID = S.PKID";
            using(var cmd=new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }


        public static int DeleteDetailDate()
        {
            const string sqlStr = @"delete T
from Tuhu_profiles..tbl_UserTaskDetailInfo as T with (rowlock)
where exists (   select 1
                 from Tuhu_profiles..UserTaskInfo_temp S with (nolock)
                 where T.UserTaskId = S.PKID);";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 通过制定userId列表，分批删除Tuhu_profiles..tbl_UserTaskInfo表数据
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
        public static int DeleteData(List<Guid> userList)
        {
            var dt = ToDateTable(userList);
            const string sqlStr = @"
delete P
from @ids as T
    inner join Tuhu_profiles..UserTaskInfo_temp as S with (nolock)
        on T.Id = S.UserId
    inner join Tuhu_profiles..tbl_UserTaskInfo as P with (rowlock)
        on P.PKID = S.PKID";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var dtPara = cmd.Parameters.AddWithValue("@ids", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 按用户分批备份Tuhu_profiles..tbl_UserTaskInfo表数据
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
        public static int BackupUserTaskInfo(List<Guid> userList)
        {
            var dt = ToDateTable(userList);
            const string sqlStr = @"insert into Tuhu_profiles..UserTaskInfo_temp
(
    PKID,
    UserId,
    TaskId,
    TriggerTime,
    TaskType,
    EndTime,
    ActionCount,
    TaskStatus,
    CreateDateTime,
    LastUpdateDateTime,
    TaskStep
)
select T.PKID,
       T.UserId,
       T.TaskId,
       T.TriggerTime,
       T.TaskType,
       T.EndTime,
       T.ActionCount,
       T.TaskStatus,
       T.CreateDateTime,
       T.LastUpdateDateTime,
       99
from @ids as P
    inner join Tuhu_profiles..tbl_UserTaskInfo as T with (nolock)
        on P.Id = T.UserId
    inner join Configuration..TaskConfigInfo as S with (nolock)
        on T.TaskId = S.TaskId
where S.Duration < 0
      and T.TaskStatus = 0";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var dtPara = cmd.Parameters.AddWithValue("@ids", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 分批备份Tuhu_profiles..tbl_UserTaskDetailInfo数据
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int BackupDetailInfo(int min, int max)
        {
            const string sqlStr = @"
insert into Tuhu_profiles..UserTaskDetailInfo_temp
(
    PKID,
    UserTaskId,
    ActionName,
    CurrentCount,
    ExpectCount,
    CreateDateTime,
    LastUpdateDateTime,
    SpecialPara
)
select T.PKID,
       T.UserTaskId,
       T.ActionName,
       T.CurrentCount,
       T.ExpectCount,
       T.CreateDateTime,
       T.LastUpdateDateTime,
       T.SpecialPara
from Tuhu_profiles..tbl_UserTaskDetailInfo as T with (rowlock)
    left join Tuhu_profiles..tbl_UserTaskInfo as S with (nolock)
        on T.UserTaskId = S.PKID
where S.PKID is null
      and T.PKID >= @min
      and T.PKID < @max;";
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@min", min);
                cmd.Parameters.AddWithValue("@max", max);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 分批删除Tuhu_profiles..tbl_UserTaskDetailInfo数据
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int DeleteDetailInfo(int min,int max)
        {
            const string sqlStr = @"
delete T
from Tuhu_profiles..tbl_UserTaskDetailInfo as T with (rowlock)
    left join Tuhu_profiles..tbl_UserTaskInfo as S with (nolock)
        on T.UserTaskId = S.PKID
where S.PKID is null
      and T.PKID >= @min
      and T.PKID < @max;";
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@min", min);
                cmd.Parameters.AddWithValue("@max", max);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }


        public static int GetMaxPkid()
        {
            const string sqlStr = @"select MAX(PKID)
from Tuhu_profiles..tbl_UserTaskDetailInfo with (nolock);";
            using(var cmd=new SqlCommand(sqlStr))
            {
                var value = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(value?.ToString(), out var outValue);
                return outValue;
            }
        }
        private static DataTable ToDateTable(List<Guid> userList)
        {
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
            return dt;
        }
    }
}
