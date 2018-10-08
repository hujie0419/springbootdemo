using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThBiz.Common;
using Tuhu.Service;
using Tuhu.Service.GaiZhuang;
using Tuhu.Service.GaiZhuang.Model;
using Tuhu.C.Job.GaizhuangRecommendCache.DAL;

namespace Tuhu.C.Job.GaizhuangRecommendCache.Job
{
    [DisallowConcurrentExecution]
    public class RefreshGaiZhuangProductsCacheJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(RefreshGaiZhuangProductsCacheJob));

        public void Execute(IJobExecutionContext context)
        {
            var client = new GaiZhuangClient();
            var product = client.RefreshGaiZhuangProductsCache("VE-VOLPOLON", "2016", "1.6L", "12177");
            try
            {
                Logger.Info("刷新改装商品缓存开始查询二级数据");
                //二级车型
                //查询数据库tbl_Vehicle_Type_Timing，读取所有的2级车型数据
                List<VehicleInfo> allList = new List<VehicleInfo>();
                IEnumerable<VehicleTypeTimingModel> list2 = new List<VehicleTypeTimingModel>();
                var vehicleTiming2 = GaiZhuangDal.GetAll2VehicleTypeTiming();
                list2 = vehicleTiming2.ToList();
                var distinctList2 = list2.Distinct();
                //循环结果，刷新缓存
                foreach (var item2 in distinctList2)
                {
                    VehicleInfo info = new VehicleInfo();
                    info.VehicleID = item2.VehicleID;
                    info.Nian = item2.Nian;
                    info.PaiLiang = item2.PaiLiang;
                    info.TID = item2.TID;
                    allList.Add(info);
                }
                Logger.Info("刷新改装商品缓存开始查询四级级数据");
                //四级车型
                var allVehicles4 = GaiZhuangDal.GetAll4VehiclesTiming();
                if (allVehicles4.Any())
                {
                    foreach (var item in allVehicles4)
                    {
                        if (item != null)
                        {
                            int startYear = CommonUtil.ConvertObjectToInt32(item.StartYear);
                            int endYear = CommonUtil.ConvertObjectToInt32(item.EndYear);
                            if (startYear > 0 && endYear > startYear)
                            {
                                for (int i = startYear; i <= endYear; i++)
                                {
                                    VehicleInfo info = new VehicleInfo();
                                    info.VehicleID = item.VehicleId;
                                    info.Nian = i.ToString();
                                    info.PaiLiang = item.PaiLiang;
                                    info.TID = null;
                                    allList.Add(info);
                                }
                            }
                        }
                    }
                }
                Logger.Info("刷新改装商品缓存开始查询五级数据");
                //五级车型
                //查询数据库tbl_Vehicle_Type_Timing，读取所有的5级车型数据
                var allVehicles5 = GaiZhuangDal.GetAll5VehicleTypeTiming();
               
                if (allVehicles5.Any())
                {
                    foreach (var item in allVehicles5)
                    {
                        if (item != null)
                        {
                            int startYear = CommonUtil.ConvertObjectToInt32(item.StartYear);
                            int endYear = CommonUtil.ConvertObjectToInt32(item.EndYear);
                            if (startYear > 0 && endYear > startYear)
                            {
                                for (int i = startYear; i <= endYear; i++)
                                {
                                    VehicleInfo info = new VehicleInfo();
                                    info.VehicleID = item.VehicleId;
                                    info.Nian = i.ToString();
                                    info.PaiLiang = item.PaiLiang;
                                    info.TID = item.Tid;
                                    allList.Add(info);
                                }
                            }
                        }
                    }
                }

                var allPids = allList.Distinct().ToList();
                Logger.Info("刷新改装商品缓存服务开始");
                Stopwatch watcher = new Stopwatch();
                watcher.Start();

                var bach = 4;
                var taskList = new List<Task<OperationResult<bool>>>(bach);

                var clientList = new List<GaiZhuangClient>(bach) {
                    new GaiZhuangClient(),
                    new GaiZhuangClient(),
                    new GaiZhuangClient(),
                    new GaiZhuangClient(),
                };
                for (var i = 0; i < allPids.Count; i = i + bach)
                {
                    taskList = new List<Task<OperationResult<bool>>>(bach);
                    for (var j = 0; j < bach && j + i < allPids.Count; j++)
                        taskList.Add(clientList[j].RefreshGaiZhuangProductsCacheAsync(
                            allPids[j + i].VehicleID, allPids[j + i].Nian, allPids[j + i].PaiLiang, allPids[j + i].TID));

                    if (taskList.Any())
                    {
                        Task.WhenAll(taskList);
                        var allError = taskList.FindAll(_ => _.Result?.Result == null || !_.Result.Success);
                        if (allError.Any())
                        {
                            allError.ForEach(_ =>
                            {
                                var index = taskList.IndexOf(_);
                                clientList[index] = new GaiZhuangClient();
                                Logger.Info("刷新改装商品失败，" + allPids[index + i].VehicleID + allPids[index + i].Nian + allPids[index + i].PaiLiang + allPids[index + i].TID);
                            });
                        }
                    }
                    if ((i + bach) % 1000 == 0)
                        Logger.Info("刷新改装商品 Count: " + (i + bach));
                }
                watcher.Stop();
                Logger.Info($"刷新改装商品缓存服务结束, 耗时：{watcher.ElapsedMilliseconds} ms");
                
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public class VehicleInfo
        {
            public string VehicleID { get; set; }
            public string Nian { get; set; }
            public string PaiLiang { get; set; }
            public string TID { get; set; }
        }

    }
}
