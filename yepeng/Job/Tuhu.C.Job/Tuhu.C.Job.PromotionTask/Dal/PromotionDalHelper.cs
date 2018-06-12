using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.C.Job.PromotionTask.Model;

namespace Tuhu.C.Job.PromotionTask.Dal
{
    public static class PromotionDalHelper
    {
        public static readonly string DBConntionStr= ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString; 
        /// <summary>
        /// 关闭超期的触发任务
        /// </summary>
        public  static int ClosePromotionTaskJob()
        {
            using (var conn = new SqlConnection(DBConntionStr))
            using (var cmd = new SqlCommand(@"
                   UPDATE dbo.tbl_promotiontask
					  SET TaskStatus = 2, 
						  Executetime = Getdate()  
					WHERE Taskendtime<=getdate() and TaskType=2 and TaskStatus = 1", conn) { CommandTimeout = 10 * 60 })
                return DbHelper.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 获取有效的塞券任务
        /// 已审核+单次任务+超过执行时间
        /// </summary>
        /// <returns></returns>
        public static List<PromotionTaskCls> GetValidPromotionTask()
        {
            using (var conn = new SqlConnection(DBConntionStr))
            using (var cmd = new SqlCommand(@"
                    SELECT PromotionTaskId,TaskName,TaskStartTime,CreateTime,UpdateTime,SelectUserType,Tasktype
                     FROM tbl_PromotionTask (NOLOCK)
                    WHERE taskStartTime<=getdate() and TaskStatus = 1 and TaskType=1 ", conn) { CommandTimeout = 5 * 60 })
                return DbHelper.ExecuteQuery<List<PromotionTaskCls>>(cmd, dt =>
                {
                    return dt.Rows.OfType<DataRow>().ConvertTo<PromotionTaskCls>().ToList();
                });
        }
        /// <summary>
        /// 执行塞券任务
        /// </summary>
        /// <param name="proTask">塞券任务</param>
        /// <returns></returns>
        public static void RunPromotionTask(PromotionTaskCls proTask)
        {
            using (var conn = new SqlConnection(DBConntionStr))
            using (var cmd = new SqlCommand(@"Promotion_SendPromotionToUserRepeatTaskJob", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120 * 60;
                cmd.Parameters.Add(
                    new SqlParameter("@PromotionTaskId", proTask == null ? null : proTask.PromotionTaskId)
                    );
                DbHelper.ExecuteNonQuery(cmd);
            }
        }
    }
}
