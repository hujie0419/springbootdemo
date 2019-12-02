using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity;

namespace Tuhu.C.Job.Activity.Dal
{
    public class DalActivity
    {
        /// <summary>
        /// 备份已删除任务的记录
        /// </summary>
        /// <returns></returns>
        public static bool ReviewActivityTask()
        {
            //const string sqlStr = @"UPDATE [Activity].[dbo].[T_ActivityUserInfo_xhr]
            //               SET [PassStatus] = 1
            //                  ,[EditTime]=GETDATE()
            //                  ,[EditUser]='xhr'
            //             WHERE [AreaID] = 2";
            //using (var cmd = new SqlCommand(sqlStr))
            //{
            //    cmd.CommandType = CommandType.Text;
            //    return DbHelper.ExecuteNonQuery(cmd);
            //}
            using (var client = new ActivityClient())
            {
                var result = client.ReviewActivityTask();
                return result.Success;
            }

        }
    }
}
