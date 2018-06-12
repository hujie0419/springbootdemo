using Common.Logging;
using Quartz;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Tuhu.C.YunYing.WinService.JobSchedulerService.YLHCouponInit.DAL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.YLHCouponInit.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.YLHCouponInit.Job
{
    public class YLHCleanDiffCouponJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(YLHCouponInitJob));
        private static string logErrorStringIn = "YLHCleanDiffCoupon清理差异多发优惠券：ErrorMsg={0}";
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("YLHCleanDiffCoupon开始执行：" + DateTime.Now.ToString());
                var dal = new YLHCouponInitDal();
                List<string> ignoreServiceProjectList = new List<string>();
                ignoreServiceProjectList = dal.SplitYLHIgnoreServiceProjects();

                try {
                    var allCouponDiffList = dal.GetAllCouponDiff();
                    if(allCouponDiffList!=null&& allCouponDiffList.Any())
                    {
                        foreach(var couponDiff in allCouponDiffList)
                        {
                            if (ignoreServiceProjectList.Contains(couponDiff.ProjectName))
                                continue;
                            else {
                                if(string.IsNullOrEmpty(couponDiff.UserID) || couponDiff.RuleID == null)
                                {
                                    Logger.Info(string.Format(logErrorStringIn, 
                                        "PKID:" +couponDiff.PKID+ " in table Activity.dbo.YLHResidueServiceDiff has null value userid of ruleid"));
                                }// end of if(string.IsNullOrEmpty(couponDiff.UserID) || couponDiff.RuleID == null)
                                else{
                                    var promotionList = dal.GetPromotionPKIDList(couponDiff);
                                    if (promotionList.Count() < couponDiff.Diff)
                                    {
                                        #region 如果status=0可用的优惠券不够清理，先清理能清理的，剩余条数记log
                                        foreach (var promotion in promotionList)
                                        {
                                            var updateresult = dal.UpdatePromotionStatus(promotion);
                                            if (updateresult < 0)
                                            {
                                                Logger.Info(string.Format(logErrorStringIn,
                                                    "优惠券PKID:" + promotion.PKID + "更新状态失败"));
                                            }
                                            else
                                            {
                                                dal.InsertYewuLogForCleanYLHCoupon(promotion, couponDiff);
                                            }
                                        }
                                        Logger.Info(string.Format(logErrorStringIn,
                                        "PKID:" +
                                        couponDiff.PKID +
                                        " in table Activity.dbo.YLHResidueServiceDiff还有"
                                        + (couponDiff.Diff - promotionList.Count())
                                        + "条优惠券不够更新"));
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 如果status=0可用的优惠券够清理，清理掉Diff个数的优惠券
                                        for (int i=0; i<couponDiff.Diff;i++)
                                        {
                                            var updateresult = dal.UpdatePromotionStatus(promotionList[i]);
                                            if (updateresult < 0)
                                            {
                                                Logger.Info(string.Format(logErrorStringIn,
                                                    "优惠券PKID:" + promotionList[i].PKID + "更新状态失败"));
                                            }
                                            else
                                            {
                                                dal.InsertYewuLogForCleanYLHCoupon(promotionList[i], couponDiff);
                                            }
                                        }
                                        #endregion
                                    }
                                } //end of else
                            } // end of else
                        } //end of foreach(var couponDiff in allCouponDiffList)
                    } //end of if(allCouponDiffList!=null&& allCouponDiffList.Any())
                } //end of try
                catch(Exception ex)
                {
                    Logger.Info(string.Format(logErrorStringIn, ex.ToString()));
                }
                Logger.Info("YLHCleanDiffCoupon执行结束：" + DateTime.Now.ToString());
            }
            catch(Exception ex)
            {
                Logger.Info($"YLHCleanDiffCoupon：异常=》{ex}");
            }
        }
    }
}
