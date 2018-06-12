using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.GaiZhuang;
using Tuhu.C.Job.GaizhuangRecommendCache.DAL;

namespace Tuhu.C.Job.GaizhuangRecommendCache.Job
{
    [DisallowConcurrentExecution]
    public class GaiZhuangProductUpdateToEsJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(GaiZhuangProductUpdateToEsJob));
        private static long mgIndex = 0;
        /// <summary>
        /// 刷新改装产品信息到ES
        /// </summary>
        /// <param name="context"></param>
        public virtual void Execute(IJobExecutionContext context)
        {
            mgIndex++;
            Logger.Info($"启动任务=>{mgIndex}");
            var dt = DateTime.Now;
            var isDeleteOld = true;
           

            var products = GaiZhuangDal.GetAllGaiZhuangProducts() ?? new List<string>();

            Logger?.Info($"RGZ{mgIndex}=更新改装产品总数为：" + products.Count);
            if (!products.Any())
            {
                return;
            }
            var batchIndex = 0;
            foreach (var list in products.Split(50))
            {
                batchIndex++;
                using (var gaizhuang = new GaiZhuangClient())
                {
                    var result = gaizhuang.RebuildGaiZhuangEsCache(list.ToList());
                    if (!result.Success)
                    {
                        using (var gaizhuang2 = new GaiZhuangClient())
                        {
                            Thread.Sleep(200);
                            result = gaizhuang2.RebuildGaiZhuangEsCache(list.ToList());
                            if (!result.Success)
                            {
                                Logger?.Warn($"RGZ{mgIndex}={batchIndex}=失败,不删除老数据：{result.ErrorCode};{result.ErrorMessage}");
                                isDeleteOld = false;
                                continue;
                            }
                        }
                    }
                }
                Logger?.Info($"RGZ{mgIndex}={batchIndex}=成功");
            }

            Thread.Sleep(TimeSpan.FromSeconds(20));
            if (!isDeleteOld)
            {
                Logger?.Info($"RGZ{mgIndex}=有更新错误的数据，不删除老数据");
                return;
            }
            using (var gaizhuang3 = new GaiZhuangClient())
            {
                var deleteResult = gaizhuang3.DeleteOldGaiZhuangEs(dt);
                if (!deleteResult.Success)
                    Logger?.Info($"RGZ{mgIndex}=删除老数据出错");
            }

            Logger?.Info($"RGZ{mgIndex}全部成功");
            return;
        }
    }
}
