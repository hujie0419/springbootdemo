using BaoYangRefreshCacheService.Common;
using BaoYangRefreshCacheService.DAL;
using BaoYangRefreshCacheService.Model;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Job
{
    [DisallowConcurrentExecution]
    public class VehiclePartsMergeJob : BaseJob
    {

        public VehiclePartsMergeJob()
        {
            logger = LogManager.GetLogger<VehiclePartsMergeJob>();
        }
        private int pageSize = 50000;

        protected override string JobName => typeof(VehiclePartsMergeJob).ToString();



        public override void Exec()
        {
            DoJob();
        }
        private void DoJob()
        {
            #region TidLiYangLevelIdMapping
            logger.Info($"TidLiYangLevelIdMapping:数据处理");
            var TidLiYangIdMaps = VehiclePartDal.GetVehicleTypeIdMap();
            var LiYangIdLiYangLevelIdMaps = VehiclePartDal.GetLiYangVehicleModelS();
            Task.WaitAll(TidLiYangIdMaps, LiYangIdLiYangLevelIdMaps);

            var TidLiYangIdMapsArray = TidLiYangIdMaps.Result?.ToArray();
            var LiYangIdLiYangLevelIdMapsArray = LiYangIdLiYangLevelIdMaps.Result?.ToArray();

            var TidLiYangLevelIdMapping = (from a in TidLiYangIdMapsArray
                                           join b in LiYangIdLiYangLevelIdMapsArray
                                           on a.ExternalId?.Trim() equals b.LiYangId?.Trim()
                                           select new LiYangLevelIdTidMap
                                           {
                                               LiYangLevelId = b.LiYangLevelId,
                                               Tid = a.Tid
                                           })?.Distinct().ToArray();



            if (!TidLiYangLevelIdMapping.Any())
                logger.Error($"TidLiYangLevelIdMapping:异常：TidLiYangIdMaps={TidLiYangIdMapsArray.Count()},LiYangIdLiYangLevelIdMaps={LiYangIdLiYangLevelIdMapsArray.Count()},TidLiYangLevelIdMapping={TidLiYangLevelIdMapping.Count()}");
            else
            {
                var existIdMaps = VehiclePartDal.GetVehicleTypeLevelIdMap();
                Task.WaitAll(existIdMaps);
                var existLevelIdMaps = existIdMaps.Result?.ToList() ?? new List<VehicleTypeIdMap>();
                TidLiYangLevelIdMapping.ForEach(e =>
                {
                    try
                    {
                        if (!existLevelIdMaps.Any() || !existLevelIdMaps.Any(s => string.Equals(s.Tid, e.Tid) && string.Equals(s.ExternalId, e.LiYangLevelId)))
                        {
                            if (!VehiclePartDal.InsertTidLiYangLevelIdMaps(e.LiYangLevelId, e.Tid))
                                logger.Warn($"InsertTidLiYangLevelIdMaps:异常：LiYangLevelId={e.LiYangLevelId},Tid={e.Tid}");
                            logger.Info($"InsertTidLiYangLevelIdMaps：LiYangLevelId={e.LiYangLevelId},Tid={e.Tid}");
                        }
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(1000);
                    }

                });
            }
            logger.Info($"TidLiYangLevelIdMapping:数据处理结束");
            #endregion
        }
    }
}
