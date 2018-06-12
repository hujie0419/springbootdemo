using BaoYangRefreshCacheService.BLL;
using BaoYangRefreshCacheService.Common;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Job
{
    /// <summary>
    /// 刷新全车件es数据
    /// </summary>
    public class VehiclePartEsJob : BaseJob
    {
        public VehiclePartEsJob()
        {
            logger = LogManager.GetLogger(typeof(VehiclePartEsJob));
        }

        protected override string JobName => typeof(VehiclePartEsJob).ToString();

        public override void Exec()
        {
            VehiclePartBll bll = new VehiclePartBll(logger);
            bll.RefreshEsData();
        }
    }
}
