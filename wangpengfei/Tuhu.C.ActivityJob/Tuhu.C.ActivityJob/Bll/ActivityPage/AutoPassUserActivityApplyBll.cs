using Common.Logging;
using System;
using Tuhu.C.ActivityJob.ServiceProxy;

namespace Tuhu.C.ActivityJob.Bll.ActivityPage
{
    public class AutoPassUserActivityApplyBll
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(AutoPassUserActivityApplyBll));

        /// <summary>
        /// 活动页关键字搜索数据刷新
        /// </summary>
        public static bool AutoPassUserActivityApplyJOB()
        {
            bool resultBool = false;

            try
            {
                resultBool = ActivcityServiceProxy.AutoPassUserActivityApply().Result;
            }
            catch (Exception ex)
            {
                Logger.Error($"AutoPassUserActivityApply根据区域自动通过活动申请失败刷新job异常：{ex.Message};堆栈信息：{ex.StackTrace}");
            }

            return resultBool;
        }
    }
}
