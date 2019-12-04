using Common.Logging;
using System;
using Tuhu.C.ActivityJob.ServiceProxy;

namespace Tuhu.C.ActivityJob.Bll.ActivityPage
{
    public class ActiviyPageBll
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ActiviyPageBll));

        /// <summary>
        /// 活动页关键字搜索数据刷新
        /// </summary>
        public static bool ActivityKeywordBll()
        {
            bool resultBool = false;

            try
            {
                resultBool = ActivcityPageServiceProxy.RefreshActivityKeyword();
            }
            catch (Exception ex)
            {
                Logger.Error($"JobActivityKeywordBll活动页关键字搜索数据刷新job异常：{ex.Message};堆栈信息：{ex.StackTrace}");
            }

            return resultBool;
        }
    }
}
