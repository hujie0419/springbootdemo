using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.WebSite.Component.SystemFramework;
using System.Collections.Generic;
using System.Configuration;
using K.DLL.Common;
using Newtonsoft.Json;
using Tuhu.Service;
using Tuhu.Service.Product;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.Service.ConfigLog;


namespace Tuhu.Yewu.WinService.JobSchedulerService.Job
{
    extern alias tuhu;

    public class BaoYangAutoAssociation : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangAutoAssociation));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");
            bool JobStatus = true;
            try
            {
                List<string> Tids = new List<string>();
                using (var cmd = new SqlCommand(@"DECLARE @StartDate DATETIME = NULL
                       SET @StartDate = (SELECT TOP 1 CreateTime FROM Tuhu_Log..JobRunHistory ORDER BY CreateTime DESC)
                       SELECT  TID
                       FROM  Gungnir..tbl_Vehicle_Type_Timing(NOLOCK)
                       WHERE  CreateTime >= CASE WHEN @StartDate IS NOT NULL
                              AND @StartDate != '' THEN @StartDate
                              ELSE GETDATE()
                       END
                       AND CreateTime <= GETDATE();"))
                {
                    var dt = DbHelper.ExecuteDataTable(true, cmd);
                    Tids.AddRange(from DataRow dr in dt.Rows select dr["TID"].ToString());
                }
                List<string> Pids = new List<string>();
                using (var cmd = new SqlCommand(@"SELECT ProductPID FROM Tuhu_productcatalog..tbl_ProductConfig (NOLOCK)
                       WHERE IsAutoAssociate = 1"))
                {
                    var dt = DbHelper.ExecuteDataTable(true, cmd);
                    Pids.AddRange(from DataRow dr in dt.Rows select dr["ProductPID"].ToString());
                }
                using (var client = new ProductClient())
                {
                    foreach (var pid in Pids)
                    {
                        foreach (var tid in Tids)
                        {
                            var result = client.SaveProductVehicleTypeConfigInfo(pid, tid);
                            if (!result.Success && result.Exception != null)
                            {
                                Logger.Error("产品适配车型失败,错误原因：" + result.Exception);
                            }
                            else
                            {
                                using (var configclient = new ConfigLogClient())
                                {
                                    var vehicleInfo = new
                                    {
                                        TID = tid,
                                        PID = pid
                                    };
                                    var status = configclient.InsertDefaultLogQueue("VehProShiPeiLog", JsonConvert.SerializeObject(vehicleInfo));
                                    if (!status.Success)
                                    {
                                        Logger.Error("记录产品适配车型日志失败，错误原因：" + status.Exception);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                JobStatus = false;
            }            
            Logger.Info("结束任务");
            using (var client = new ConfigLogClient())
            {
                var jobInfo = new
                {
                    JobName = "BaoYangAutoAssociation",
                    RunStatus = JobStatus ? "Success" : "Fail"
                };
                var result = client.InsertDefaultLogQueue("JobHistory", JsonConvert.SerializeObject(jobInfo));
                if (!result.Success)
                {
                    Logger.Error("记录job运行日志失败，错误原因：" + result.Exception);
                }
            }
        }
    }
}
