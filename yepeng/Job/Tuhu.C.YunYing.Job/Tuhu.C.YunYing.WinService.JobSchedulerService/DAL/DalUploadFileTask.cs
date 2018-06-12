using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.DAL
{
   public class DalUploadFileTask
    {
        /// <summary>
        /// 文件处理失败状态更新
        /// </summary>
        /// <param name="item"></param>
        /// <param name="status"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool SetErrrorFileStatus(UploadFileTask item, FileStatus status, string errorMessage)
        {
            #region Sql
            var sql = @" UPDATE  Tuhu_log..UploadFileTaskLog
                            SET     Status = @Status ,
                                    Remarks = @Remarks ,
                                    LastUpdateTime = GETDATE()
                            WHERE   PKID = @PKID
                                    AND Type = @Type
                                    AND Status <> 'SUCCESS';";
            #endregion
            using (var dbHelper = DbHelper.CreateLogDbHelper(false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PKID", item.PKID);
                    cmd.Parameters.AddWithValue("@Type", item.Type.ToString());
                    cmd.Parameters.AddWithValue("@Status", status.ToString());
                    cmd.Parameters.AddWithValue("@Remarks", errorMessage);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
        }

        /// <summary>
        /// 文件处理成功状态更新
        /// </summary>
        /// <param name="item"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool UpdateFileTaskStatus(UploadFileTask item, FileStatus status)
        {
            #region Sql
            var sql = @"UPDATE  Tuhu_log..UploadFileTaskLog
                        SET     Status = @Status ,
                                LastUpdateTime = GETDATE()
                        WHERE   PKID = @PKID
                                AND Type = @Type
                                AND Status <> 'SUCCESS';";
            #endregion
            using (var dbHelper = DbHelper.CreateLogDbHelper(false))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", item.PKID);
                cmd.Parameters.AddWithValue("@Type", item.Type.ToString());
                cmd.Parameters.AddWithValue("@Status", status.ToString());
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
    }
}
