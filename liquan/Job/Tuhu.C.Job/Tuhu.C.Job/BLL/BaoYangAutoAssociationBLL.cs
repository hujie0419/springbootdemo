using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Service.ConfigLog;
using Tuhu.Service.Product;
using Tuhu.C.Job.Models;

namespace Tuhu.C.Job.BLL
{
    public class BaoYangAutoAssociationBLL
    {
        private ILog logger;

        public BaoYangAutoAssociationBLL(ILog logger)
        {
            this.logger = logger;
        }

        public string GetJobLastRunTime()
        {
            string startTime;
            using (var dbhelper = DbHelper.CreateDbHelper("Tuhu_log"))
            {
                var sql = @"SELECT TOP 1 CreateTime FROM Tuhu_Log..JobRunHistory (NOLOCK) WHERE RunStatus = 'Success' ORDER BY CreateTime DESC";
                var dr = dbhelper.ExecuteScalar(sql);
                startTime = dr?.ToString();
            }
            return startTime;
        }

        public List<string> SelectNewVehicleTids(string startTime)
        {
            List<string> tids = new List<string>();
            using (var cmd = new SqlCommand(@"
                       SELECT  TID
                       FROM  Gungnir..tbl_Vehicle_Type_Timing(NOLOCK)
                       WHERE  CreateTime >= CASE WHEN @StartDate IS NOT NULL
                              AND @StartDate != '' THEN @StartDate
                              ELSE GETDATE()
                       END
                       AND CreateTime <= GETDATE();"))
            {
                cmd.Parameters.AddWithValue("@StartDate", startTime);
                var dt = DbHelper.ExecuteQuery(true, cmd, _ => _);
                tids.AddRange(from DataRow dr in dt.Rows select dr["TID"].ToString());
            }
            return tids;
        }

        public List<string> SelectPids()
        {
            List<string> pids = new List<string>();
            using (var cmd = new SqlCommand(@"SELECT ProductPID FROM Tuhu_productcatalog..tbl_ProductConfig (NOLOCK)
                   WHERE IsAutoAssociate = 1"))
            {
                var dt = DbHelper.ExecuteQuery(true, cmd, _ => _);
                pids.AddRange(from DataRow dr in dt.Rows select dr["ProductPID"].ToString());
            }
            return pids;
        }

        public void AsociateVehicles(List<string> tids, List<string> pids )
        {
            using (var client = new ProductClient())
            {
                foreach (var pid in pids)
                {
                    var hasAddVehicle = false;
                    foreach (var tid in tids)
                    {
                        if (!CheckTidValidation(tid))
                        {
                            continue;
                        }
                        var result = client.SaveProductVehicleTypeConfigInfo(pid, tid);
                        if (!result.Success && result.Exception != null)
                        {
                            logger.Error("产品适配车型失败", result.Exception);
                        }
                        else if(result.Result)
                        {
                            hasAddVehicle = true;
                            InsertVehicleAssociateLog(tid, pid);
                        }
                    }
                    if (hasAddVehicle)
                        InsertPidAssociationLog(pid);
                }
            }
        }

        public void InsertPidAssociationLog(string pid)
        {
            using (var configclient = new ConfigLogClient())
            {
                var entity = new ProductVehicleTypeConfigOpLog()
                {
                    PID = pid,
                    Operator = "系统自动",
                    OperateContent = string.Format("修改配置：产品{0}已自动适配车型", pid),
                    OperateTime = DateTime.Now,
                    CreatedTime = DateTime.Now,
                };
                var status = configclient.InsertDefaultLogQueue("PrdVehicleOprLog", JsonConvert.SerializeObject(entity));
                if (!status.Success)
                {
                    logger.Error("记录产品适配车型日志失败", status.Exception);
                }
            }
        }

        public void InsertVehicleAssociateLog(string tid, string pid)
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
                    logger.Error("记录产品适配车型日志失败", status.Exception);
                }
            }
        }

        public bool CheckTidValidation(string tid)
        {
            bool result = false;
            using (var cmd = new SqlCommand(@"SELECT 1 FROM Gungnir.dbo.tbl_Vehicle_Type_Timing (NOLOCK) AS tm
                  JOIN Gungnir..tbl_Vehicle_Type (NOLOCK) AS vt
                  ON tm.VehicleID = vt.ProductID
                  WHERE tm.TID = @Tid 
                  AND vt.VehicleBodyType != N'皮卡'
                  AND vt.VehicleBodyType != N'轻型客车'"))
            {
                cmd.Parameters.AddWithValue("@Tid", tid);
                var dt = DbHelper.ExecuteQuery(true, cmd, _ => _);
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
