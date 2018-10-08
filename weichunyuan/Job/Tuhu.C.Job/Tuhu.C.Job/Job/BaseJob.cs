using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Tuhu.C.Job.Job
{
    public abstract class BaseJob : IJob
    {
        protected ILog logger;

        protected abstract string JobName { get; }

        public void Execute(IJobExecutionContext context)
        {
            if (context == null)
            {
                logger.Info(this + " manully begin");
            }
            else
            {
                logger.Info(this + " begin");
            }
            var jobId = InsertJobRunStatus();
            try
            {
                Exec();
                UpdateJobRunStatus(jobId, "Success");
            }
            catch (Exception ex)
            {
                UpdateJobRunStatus(jobId, "Fail");
                logger.Error(ex.Message, ex);
            }
            logger.Info(this + " end");
        }

        public int InsertJobRunStatus()
        {
            var jobId = 0;
            try
            {
                using (var dbhelper = DbHelper.CreateDbHelper("Tuhu_log"))
                {
                    var sql = @"INSERT INTO Tuhu_Log..JobRunHistory
                           ( ServiceName, JobName, CreateTime, RunStatus )
                           VALUES  ( @ServiceName, 
                                     @JobName, 
                                     GETDATE(), 
                                     @RunStatus);
                           SELECT SCOPE_IDENTITY()";
                    DbParameter[] parameters =
                    {
                        new SqlParameter("@ServiceName", "YewuJob"),
                        new SqlParameter("@JobName", this.JobName),
                        new SqlParameter("@RunStatus", "Running")
                    };
                    var data = dbhelper.ExecuteScalar(sql, CommandType.Text, parameters);
                    if (data != null)
                    {
                        jobId = Convert.ToInt32(data);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return jobId;
        }

        public void UpdateJobRunStatus(int jobId, string status)
        {
            try
            {
                using (var dbhelper = DbHelper.CreateDbHelper("Tuhu_log"))
                {
                    var sql = @"UPDATE Tuhu_Log..JobRunHistory
                       SET RunStatus = @RunStatus,
                       UpdateTime = GETDATE()
                       WHERE PKID = @PKID";
                    DbParameter[] parameters =
                    {
                        new SqlParameter("@RunStatus", status),
                        new SqlParameter("@PKID", jobId),
                    };

                    dbhelper.ExecuteNonQuery(sql, CommandType.Text, parameters);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        public abstract void Exec();
    }
}
