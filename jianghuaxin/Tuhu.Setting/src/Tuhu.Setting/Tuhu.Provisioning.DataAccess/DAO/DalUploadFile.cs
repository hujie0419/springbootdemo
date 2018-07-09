using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalUploadFile
    {
        /// <summary>
        /// 获取上传文件状态
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchcode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFileTaskStatus(SqlConnection conn, string batchcode, FileType type)
        {
            var sql = @"SELECT  t.Status
                        FROM    Tuhu_log..UploadFileTaskLog AS t WITH ( NOLOCK )
                        WHERE   t.BatchCode = @BatchCode
                                AND t.Type = @Type;";
            var returnvalue = conn.ExecuteScalar(sql, new
            {
                BatchCode = batchcode,
                Type = type.ToString(),
            }, commandType: CommandType.Text);
            return Convert.ToString(returnvalue);
        }

        /// <summary>
        /// 更新文件上传状态
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchCode"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="originStatus"></param>
        /// <returns></returns>
        public static int UpdateFileStatus(SqlConnection conn, string batchCode, FileType type,
            FileStatus status, FileStatus originStatus)
        {
            const string sql = @"UPDATE  Tuhu_log..UploadFileTaskLog
                                SET     Status = @Status ,
                                        LastUpdateTime = GETDATE()
                                WHERE   Type = @Type
                                        AND BatchCode = @BatchCode
                                        AND Status = @OriginStatus;";
            return Convert.ToInt32(conn.Execute(sql, new
            {
                BatchCode = batchCode,
                Type = type.ToString(),
                Status = status.ToString(),
                OriginStatus = originStatus.ToString()
            }, commandType: CommandType.Text));
        }
    }
}
