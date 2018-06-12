using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Service.BaoYang;
using Tuhu.C.Job.BaoYangSuggest.Model;

namespace Tuhu.C.Job.BaoYangSuggest
{
    [DisallowConcurrentExecution]
    public class BaoYangSuggestJob : IJob
    {
        private static readonly ILog CouponLog = LogManager.GetLogger(typeof(BaoYangSuggestJob));
        public void Execute(IJobExecutionContext context)
        {
            CouponLog.Info("保养项目推荐服务启动");

            try
            {
                var updateBaoYangRecordResult = SuggestBusiness.UpdateBaoYangRecord();
                if (updateBaoYangRecordResult)
                {
                    CouponLog.Info("保养档案更新成功");
                }
                else
                {
                    CouponLog.Info("保养档案更新失败");
                }                

                CouponLog.Info("保养推荐服务成功");

            }
            catch (Exception ex)
            {
                CouponLog.Error(ex.Message, ex);
            }

            CouponLog.Info("保养项目推荐服务结束");
        }
    }
}
