using Common.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BLL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DLL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Job
{
    [DisallowConcurrentExecution]
    public class UploadFileTaskJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UploadFileTaskJob));

        private static readonly string FileDomain=ConfigurationManager.AppSettings["FileDoMain"];

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("UploadFileTaskJob Start");
            try
            {
                var tasks = GetNeedRunFileTasks();
                if (tasks != null && tasks.Any())
                    ExecuteUplodFileTasks(tasks);
                else
                    Logger.Info("UploadFileTaskJob Null");
            }
            catch (Exception ex)
            {
                Logger.Error("UploadFileTaskJobRun", ex);
            }
            Logger.Info("UploadFileTaskJob End");
        }

        private static bool ExecuteUplodFileTasks(List<UploadFileTask> tasks)
        {
            var result = false;

            try
            {
                if (tasks != null && tasks.Any())
                {
                    foreach (var task in tasks)
                    {
                        var fileUrl = FileDomain + task.FilePath;
                        WebClient webClient = new WebClient();
                        webClient.Credentials = CredentialCache.DefaultCredentials;
                        var buffer = webClient.DownloadData(fileUrl);
                        switch (task.Type)
                        {
                            case FileType.VipBaoYangPackage:
                                {
                                    var data = VipBaoYangPackageBusiness.LoadUploadBaoYangPromotionsFile(buffer);
                                    if (data != null && data.Any() && !string.IsNullOrEmpty(task.BatchCode))
                                    {
                                        VipBaoYangPackageBusiness.CreateVipBaoYngPackagePromotions(data, task);
                                    }
                                    break;
                                }
                            case FileType.VehicleLiYangId:
                                {
                                    var data = VehiclePartsLiYangBusiness.ConvertExcelToList(buffer, task);
                                    if (data != null && data.Rows.Count > 0)
                                        VehiclePartsLiYangBusiness.BatchAddVehiclePartsLiYang(data, task);
                                    break;
                                }
                            case FileType.LiYangId_LevelIdMap:
                                {
                                    var data = LiYangId_LevelIdMapBusiness.ConvertExcelToList(buffer, task);
                                    if (data != null && data.Rows.Count > 0)
                                        LiYangId_LevelIdMapBusiness.BatchAddLiYangId_LevelIdMap(data, task);
                                    break;
                                }
                            case FileType.VipPaintPackage:
                                {
                                    var data = VipPaintPackageBusiness.ConvertExcelToList(buffer, task);
                                    if (data != null && data.Any() && !string.IsNullOrEmpty(task.BatchCode))
                                    {
                                        VipPaintPackageBusiness.CreateVipPaintPackagePromotions(data, task);
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ExecuteUplodFileTasks", ex);
            }

            return result;
        }

        private static List<UploadFileTask> GetNeedRunFileTasks()
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(false))
            using (var cmd = new SqlCommand(@"SELECT  PKID ,
        BatchCode ,
        FilePath ,
        Type ,
        Status ,
        CreateUser ,
        CreateTime ,
        LastUpdateTime
FROM    Tuhu_log..UploadFileTaskLog WITH ( NOLOCK )
WHERE   Status IN ( N'Wait', N'Failed', N'Loaded', N'Repaired' )
ORDER BY PKID DESC;"))
            {
                var result = dbHelper.ExecuteSelect<UploadFileTask>(cmd);
                return result.ToList();
            }
        }
    }
}
