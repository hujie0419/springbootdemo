using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.ActivityJob.Dal.SalePromotion;
using Tuhu.C.ActivityJob.ServiceProxy;
using Tuhu.Service.EmailProcess.Model;

namespace Tuhu.C.ActivityJob.Job.SalePromotion
{
    /// <summary>
    /// 次日上线促销活动邮件提醒Job
    /// </summary>
    public class NextDayEmailReminderJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(NextDayEmailReminderJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info($"次日上线促销活动邮件提醒开始.");

                var staffDict = GetPromotionOperationalStaff();

                if (staffDict != null && staffDict.ContainsKey("StaffEmailGroup"))
                {
                    var subject = $"{DateTime.Now.AddDays(1).ToString("yy年MM月dd日")}上线的促销活动";

                    var sbBody = new StringBuilder();
                    AppendSeckillEmailBody(sbBody, staffDict);
                    AppendPromotionEmailBody(sbBody, staffDict);
                    AppendGroupBuyingEmailBody(sbBody, staffDict);

                    var emails = staffDict["StaffEmailGroup"].Split(',');
                    foreach (var item in emails)
                    {
                        EmailProcessServiceProxy.InsertEmail(new InsertSMSModel
                        {
                            url = string.Empty,
                            Body = sbBody.ToString(),
                            ToMail = item,
                            CC = string.Empty,
                            Status = "New",
                            Subject = subject,
                            Type = "Email",
                            OrderNo = string.Empty
                        });
                    }
                }
                else
                {
                    Logger.Error($"获取促销活动邮件配置失败.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"次日上线促销活动邮件提醒异常.", ex);
            }
        }

        /// <summary>
        /// 增加天天秒杀邮件正文
        /// </summary>
        /// <param name="sbBody"></param>
        /// <param name="staffDict"></param>
        private void AppendSeckillEmailBody(StringBuilder sbBody, Dictionary<string, string> staffDict)
        {
            if (staffDict.ContainsKey("SeckillStaff"))
            {
                sbBody.AppendLine($"1. 天天秒杀 —— {staffDict["SeckillStaff"]}<br>");

                var seckills = SalePromotionDal.GetNextDayFlashSales(1);
                var seckillTemps = SalePromotionDal.GetNextDayFlashSaleTemps(1);

                // 排除审核状态为驳回的活动
                var tempActivityIds = seckillTemps.Select(x => x.ActivityID).ToList();
                var tempDisapproves = SalePromotionDal.GetActivityApprovalStatus(tempActivityIds).Where(x => x.Status == 2);
                seckillTemps = seckillTemps.Where(x => !tempDisapproves.Any(t => t.ActivityId == x.ActivityID)).ToList();

                var url = "https://setting.tuhu.cn/pages/activity/#/seckill/index";
                for (int i = 0; i < seckills.Count; i++)
                {
                    // 避免重复发送待审核活动
                    if (!seckillTemps.Any(x => x.ActivityID.Equals(seckills[i].ActivityID)))
                    {
                        sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1}. {seckills[i].ActivityName} - <a href='{url}'>{url}</a><br>");
                    }
                }

                for (int i = 0; i < seckillTemps.Count; i++)
                {
                    sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1 + seckills.Count}. {seckillTemps[i].ActivityName}（待审核） - <a href='{url}'>{url}</a><br>");
                }
            }
        }

        /// <summary>
        /// 增加全网/限时抢购/打折/赠品/活动页秒杀邮件正文
        /// </summary>
        /// <param name="sbBody"></param>
        /// <param name="staffDict"></param>
        private void AppendPromotionEmailBody(StringBuilder sbBody, Dictionary<string, string> staffDict)
        {
            if (staffDict.ContainsKey("PromotionStaff"))
            {
                sbBody.AppendLine($"<br>2. 全网 / 限时抢购 / 打折 / 赠品 / 活动页秒杀 —— {staffDict["PromotionStaff"]}<br>");

                #region 全网活动
                var wholes = SalePromotionDal.GetNextDayFlashSales(0);
                var wholeTemps = SalePromotionDal.GetNextDayFlashSaleTemps(0);
                if (wholes.Count > 0 || wholeTemps.Count > 0)
                {
                    sbBody.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;全网活动：<br>");
                }

                for (int i = 0; i < wholes.Count; i++)
                {
                    if (!wholeTemps.Any(x => x.ActivityID.Equals(wholes[i].ActivityID)))
                    {
                        var url = $"https://setting.tuhu.cn/QiangGou/Detail?aid={wholes[i].ActivityID}";
                        sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1}. {wholes[i].ActivityName} - {wholes[i].ActivityID} - <a href='{url}'>{url}</a><br>");
                    }
                }

                for (int i = 0; i < wholeTemps.Count; i++)
                {
                    var url = $"https://setting.tuhu.cn/QiangGou/Detail?aid={wholeTemps[i].ActivityID}";
                    sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1 + wholes.Count}. {wholeTemps[i].ActivityName}（待审核） - {wholeTemps[i].ActivityID} - <a href='{url}'>{url}</a><br>");
                }
                #endregion

                #region 限时抢购
                var limits = SalePromotionDal.GetNextDayFlashSales(4);
                var limitTemps = SalePromotionDal.GetNextDayFlashSaleTemps(4);
                if (limits.Count > 0 || limitTemps.Count > 0)
                {
                    sbBody.AppendLine("<br>&nbsp;&nbsp;&nbsp;&nbsp;限时抢购：<br>");
                }

                for (int i = 0; i < limits.Count; i++)
                {
                    if (!limitTemps.Any(x => x.ActivityID.Equals(limits[i].ActivityID)))
                    {
                        var url = $"https://setting.tuhu.cn/QiangGou/Detail?aid={limits[i].ActivityID}";
                        sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1}. {limits[i].ActivityName} - {limits[i].ActivityID} - <a href='{url}'>{url}</a><br>");
                    }
                }

                for (int i = 0; i < limitTemps.Count; i++)
                {
                    var url = $"https://setting.tuhu.cn/QiangGou/Detail?aid={limitTemps[i].ActivityID}";
                    sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1 + limits.Count}. {limitTemps[i].ActivityName}（待审核） - {limitTemps[i].ActivityID} - <a href='{url}'>{url}</a><br>");
                }
                #endregion

                #region 打折活动
                var discounts = SalePromotionDal.GetNextDaySalePromotions();
                if (discounts.Count > 0)
                {
                    sbBody.AppendLine("<br>&nbsp;&nbsp;&nbsp;&nbsp;打折活动：<br>");
                }

                for (int i = 0; i < discounts.Count; i++)
                {
                    var status = discounts[i].AuditStatus == 1 ? "（待审核）" : "";
                    var url = $"https://setting.tuhu.cn/pages/salepromotion/#/discount/activityinfo?activityId={discounts[i].ActivityId}";
                    sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1}. {discounts[i].Name}{status} - {discounts[i].ActivityId} - <a href='{url}'>{url}</a><br>");
                }
                #endregion

                #region 赠品活动
                var gifts = SalePromotionDal.GetNextDayGiftActivities();
                if (gifts.Count > 0)
                {
                    sbBody.AppendLine("<br>&nbsp;&nbsp;&nbsp;&nbsp;赠品活动：<br>");
                }

                for (int i = 0; i < gifts.Count; i++)
                {
                    var url = $"https://setting.tuhu.cn/SE_GiftManageConfig/Edit/{gifts[i].Id}";
                    sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1}. {gifts[i].Name} - {gifts[i].Id} - <a href='{url}'>{url}</a><br>");
                }
                #endregion

                #region 活动页秒杀
                var seckills = SalePromotionDal.GetNextDayFlashSales(3);
                var seckillTemps = SalePromotionDal.GetNextDayFlashSaleTemps(3);
                if (seckills.Count > 0 || seckillTemps.Count > 0)
                {
                    sbBody.AppendLine("<br>&nbsp;&nbsp;&nbsp;&nbsp;活动页秒杀：<br>");
                }

                for (int i = 0; i < seckills.Count; i++)
                {
                    if (!seckillTemps.Any(x => x.ActivityID.Equals(seckills[i].ActivityID)))
                    {
                        var url = $"https://setting.tuhu.cn/QiangGou/Detail?aid={seckills[i].ActivityID}";
                        sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1}. {seckills[i].ActivityName} - {seckills[i].ActivityID} - <a href='{url}'>{url}</a><br>");
                    }
                }

                for (int i = 0; i < seckillTemps.Count; i++)
                {
                    var url = $"https://setting.tuhu.cn/QiangGou/Detail?aid={seckillTemps[i].ActivityID}";
                    sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1 + seckills.Count}. {seckillTemps[i].ActivityName}（待审核） - {seckillTemps[i].ActivityID} - <a href='{url}'>{url}</a><br>");
                }
                #endregion
            }
        }

        /// <summary>
        /// 增加拼团邮件正文
        /// </summary>
        /// <param name="sbBody"></param>
        /// <param name="staffDict"></param>
        private void AppendGroupBuyingEmailBody(StringBuilder sbBody, Dictionary<string, string> staffDict)
        {
            if (staffDict.ContainsKey("GroupBuyingStaff"))
            {
                sbBody.AppendLine($"<br>3. 拼团 —— {staffDict["GroupBuyingStaff"]}<br>");

                var groupBuyings = SalePromotionDal.GetNextDayGroupBuyings();

                var url = "https://setting.tuhu.cn/pages/pintuan/#/ProductConfig";
                for (int i = 0; i < groupBuyings.Count; i++)
                {
                    sbBody.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;{i + 1}. {groupBuyings[i].ProductName} - {groupBuyings[i].ProductGroupId} - <a href='{url}'>{url}</a><br>");
                }
            }
        }

        /// <summary>
        /// 获取促销活动邮件配置
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetPromotionOperationalStaff()
        {
            var staffDict = new Dictionary<string, string>();

            AsyncHelper.RunSync(async () =>
                staffDict = await ConfigBaseServiceProxy.GetBaseConfigList("PromotionOperationalStaff"));

            return staffDict;
        }
    }
}
