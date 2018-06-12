using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using Common.Logging;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Component.SystemFramework.Log;
using System.Text;
using System.Threading.Tasks;
//using Tuhu.C.YunYing.WinService.JobSchedulerService.BLL;
//using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Job
{
    /// <summary>测试用Job huhengxing 2016-4-21 </summary>
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TestJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {                
                Logger.Info("启动任务Test");
                try
                {
                    using (var cmd = new SqlCommand(@" SELECT TOP 10	V.ProductID,
		V.Brand,
		V.Vehicle,
		V.Tires,
		V.Filter,
		V.FilterPack,
		V.Brake,
		V.BrakePack,
		V.TiresMatch,
		V.IsActive,
		V.LastUpdateTime,
		V.FilterMatch,
		V.IsDisplay,
		V.IsBaoyang,
		V.Priority1,
		V.Priority2,
		V.Priority3,
		V.VehicleSeries,
		V.OilLevel,
		V.MinPrince,
		V.AvgPrice,
		V.MaxPrice,
		V.MinPrice,
		V.VehicleLevel,
		V.ServicePriceLevel
FROM	Gungnir..tbl_Vehicle_Type AS V WITH ( NOLOCK )"))
                    {

                        var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(cmd);
                        if(dt.Rows.Count>0)
                        {
                            string item = dt.Rows[0]["Vehicle"].ToString();
                            Logger.Info(item);
                        }
                    }

                    //List<VehicleType> vehicleTypeList=
                    //Business.GetVehicleTypeForTest();
                    //if (vehicleTypeList == null || vehicleTypeList.Count < 1)
                    //{
                    //    Logger.Info("抓取测试数量为0");
                    //}
                    //else
                    //{
                    //    Logger.Info(vehicleTypeList[0].Vehicle);
                    //}
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }

                Logger.Info("结束任务Test");

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

    }
}
