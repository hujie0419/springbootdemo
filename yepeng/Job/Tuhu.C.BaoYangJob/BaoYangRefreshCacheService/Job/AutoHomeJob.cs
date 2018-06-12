using BaoYangRefreshCacheService.BLL;
using BaoYangRefreshCacheService.Common;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Job
{
    /// <summary>
    /// 获取汽车之家车系数据
    /// </summary>
    [DisallowConcurrentExecution]
    class AutoHomeGetCarGradeJob : BaseJob
    {

        public AutoHomeGetCarGradeJob()
        {
            logger = LogManager.GetLogger(typeof(AutoHomeGetCarGradeJob));
        }

        protected override string JobName => typeof(AutoHomeGetCarGradeJob).ToString();

        public override void Exec()
        {
            AutoHomeBll.GetCarModelGrade().Wait();
        }
    }

    /// <summary>
    /// 根据汽车之家车系获取车型数据
    /// </summary>
    [DisallowConcurrentExecution]
    class AutoHomeGetCarDataJob : BaseJob
    {

        public AutoHomeGetCarDataJob()
        {
            logger = LogManager.GetLogger(typeof(AutoHomeGetCarDataJob));
        }

        protected override string JobName => typeof(AutoHomeGetCarDataJob).ToString();

        public override void Exec()
        {
            AutoHomeBll.GetCarModelData().Wait();
        }
    }
}
