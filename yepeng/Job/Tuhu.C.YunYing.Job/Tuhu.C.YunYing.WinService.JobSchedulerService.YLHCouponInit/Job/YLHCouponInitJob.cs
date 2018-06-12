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
    [DisallowConcurrentExecution]
    public class YLHCouponInitJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(YLHCouponInitJob));
        private static string logErrorStringIn = "YLHCouponInit初始塞券：ErrorMsg={0}";

        public void Execute(IJobExecutionContext context)
        {
            try {
                Logger.Info("YLHCouponInitJob开始执行：" + DateTime.Now.ToString());
                var dal = new YLHCouponInitDal();
                List<string> ignoreServiceProjectList = new List<string>();
                ignoreServiceProjectList = dal.SplitYLHIgnoreServiceProjects();

                try {
                    var allYLHResidueProjectList = dal.GetAllYLHResidueProject();
                    if (allYLHResidueProjectList != null && allYLHResidueProjectList.Any())
                    {
                        foreach(var ylhResidueProject in allYLHResidueProjectList)
                        {
                            if (ignoreServiceProjectList.Contains(ylhResidueProject.ProjectName))
                                continue;
                            else
                            {
                                UserObjectModel temp = new UserObjectModel();
                                temp = dal.GetUserIDByDisplayCardNBR(ylhResidueProject.DisplayCardNBR);

                                if (temp == null) {
                                    Logger.Info(string.Format(logErrorStringIn, "用户ID:"+ ylhResidueProject.DisplayCardNBR+"对应用户不存在"));
                                    continue;
                                }
                                else {
                                    CouponRuleModel coupon = new CouponRuleModel();
                                    coupon = dal.SelectCouponRule(ylhResidueProject.ProjectName,ylhResidueProject.DefaultPrice);

                                    if (coupon == null)
                                    {
                                        Logger.Info(string.Format(logErrorStringIn,
                                            "用户ID:" + ylhResidueProject.DisplayCardNBR +
                                            " 服务项目:" + ylhResidueProject.ProjectName + "不存在这个优惠券"));
                                        continue;
                                    }
                                    else {
                                        //set EndTime
                                        DateTime endDate = new DateTime();
                                        if (DateTime.TryParseExact(ylhResidueProject.EffectiveDate.ToString(), "yyyyMMdd",
                                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                    out endDate)) { }
                                        else { endDate = DateTime.Now.AddMonths(1); }

                                        //set Discount and MinMoney
                                        double discount = 0;
                                        double minmoney = 0;
                                        if("实付券" == coupon.PromotionName)
                                        {
                                            discount = 0;
                                            minmoney = 0;
                                        }
                                        else
                                        {
                                            discount = ylhResidueProject.DefaultPrice;
                                            minmoney = ylhResidueProject.DefaultPrice;
                                        }

                                        PromotionCodeModel model = new PromotionCodeModel
                                        {
                                            StartTime = DateTime.Now,
                                            EndTime = endDate,
                                            CodeChannel = "YLH",
                                            UserId = (Guid)temp.UserId,
                                            Discount = Convert.ToInt32(discount),
                                            MinMoney = Convert.ToDecimal(minmoney),
                                            BatchId = 9999,
                                            Number = ylhResidueProject.RemainCount,
                                            Description = coupon.Description,
                                            RuleId = coupon.RuleID
                                        };

                                        dal.CreatePromotion(model);
                                    }
                                }                     
                            }
                        }
                    }
                }
                catch(Exception ex) {
                    Logger.Info(string.Format(logErrorStringIn, ex.ToString()));
                }

                Logger.Info("YLHCouponInitJob执行结束：" + DateTime.Now.ToString());
            }
            catch(Exception ex)
            {
                Logger.Info($"YLHCouponInitJob：异常=》{ex}");
            }
        }
    }
}
