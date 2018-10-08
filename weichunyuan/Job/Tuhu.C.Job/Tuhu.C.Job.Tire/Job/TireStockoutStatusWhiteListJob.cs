using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.C.Job.Tire.BLL;

namespace Tuhu.C.Job.Tire.Job
{
    public class TireStockoutStatusWhiteListJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(TireStockoutStatusWhiteListJob));
        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("启动任务");
            try
            {
                // 清空表~
                var truncateResult = TireStockoutStatusWhileManager.TruncateTableInfo();
                _logger.Info($"TireStockoutStatusWhiteListJob==>删除{truncateResult}条数据");

                var pids = TireStockoutStatusWhileManager.SelectTiresMatchAndSaleQuantityMoreThanEight();
                if (pids.Any())
                {
                    var b = TireStockoutStatusWhileManager.JoinWhiteList(pids, _logger);
                    if (b)
                    {
                        using (var client = new CacheClient())
                        {
                            var result = client.RefreshTireStockoutStatusWhiteListCache();
                            result.ThrowIfException(true);
                            if (!result.Result)
                                _logger.Error("更新成功，刷新缓存失败");
                        }
                    }
                    else
                        _logger.Error("更新失败");
                }
                else
                {
                    _logger.Info("未获取到月销量大于等于8条或者原配的轮胎");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            _logger.Info("任务完成");
        }
    }
}
