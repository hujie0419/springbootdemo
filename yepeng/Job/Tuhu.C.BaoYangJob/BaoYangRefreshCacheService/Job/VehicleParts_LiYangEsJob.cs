using BaoYangRefreshCacheService.BLL;
using BaoYangRefreshCacheService.Common;
using BaoYangRefreshCacheService.DAL;
using BaoYangRefreshCacheService.Model;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BaoYangRefreshCacheService.Job
{
    [DisallowConcurrentExecution]
    public class VehicleParts_LiYangEsJob : BaseJob
    {
        public VehicleParts_LiYangEsJob()
        {
            logger = LogManager.GetLogger<VehicleParts_LiYangEsJob>();
        }
        protected override string JobName => typeof(VehiclePartsMergeJob).ToString();
        private int pageSize = 3000;
        public override void Exec()
        {
            logger.Info("VehicleParts_LiYangEsJob:Start");
            DoJob();
            logger.Info("VehicleParts_LiYangEsJob:End");
        }

        private void DoJob()
        {
            var VehicleLiYangList = VehiclePartDal.GetOeCodeLiYangMap()?.ToArray();

            if (VehicleLiYangList == null || !VehicleLiYangList.Any())
            {
                logger.Error($"GetOeCodeLiYangMap:未获取到数据");
                return;
            }
            if (!VehiclePartDal.DeleteVehicleParts_LiYangCategory())
            {
                logger.Error($"DeleteVehicleParts_LiYangCategory:清理旧数据失败");
            }
            logger.Info($"VehicleParts_LiYangEsJob:开始处理{VehicleLiYangList.Length}条类目数据");
            int i = 1;
            VehicleLiYangList.ForEach(f =>
            {
                logger.Info($"VehicleParts_LiYangEsJob:开始处理第{i}条类目数据");
                var pkids = VehiclePartDal.GetPkIdsByCategory(f.Category, f.MainCategory, f.SubGroup)?.ToArray();
                if (pkids == null || !pkids.Any())
                    return;
                if (VehiclePartDal.InsertVehicleParts_LiYangCategory(f.Category, f.MainCategory, f.SubGroup, pkids))
                {
                    logger.Info($"第{i}条{f.Category}=>{f.MainCategory}=>{f.SubGroup}类目下pkids处理成功");
                }
                else
                {
                    logger.Info($"第{i}条{f.Category}=>{f.MainCategory}=>{f.SubGroup}类目下pkids处理失败");
                    System.Threading.Thread.Sleep(1000);
                }
                i++;
            });
        }
    }
}
