using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.DAL
{
    public interface IPromotionPushDalHelper
    {
        DataTable SelectIsPushList();

        IEnumerable<PromotionCodeModel> SelectPushPromotion(int minPkid, int pageSize, IEnumerable<int> ruleIds,
            IEnumerable<int> getRuleIds, int days);

        IEnumerable<PromotionCodeModel> SelectPushPromotionByUserId(IEnumerable<string> page,
            IEnumerable<int> ruleIds, IEnumerable<int> getRuleIds, int days);
    }

    public class PromotionPushDalHelper : IPromotionPushDalHelper
    {
        public DataTable SelectIsPushList()
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT PKID AS GetRuleId,RuleId,PushSetting FROM Activity..tbl_GetCouponRules AS GCR WITH(NOLOCK) WHERE GCR.IsPush=1 AND (DeadLineDate IS NULL OR DeadLineDate>=@Now)"
                ))
            {
                cmd.Parameters.AddWithValue("@Now",DateTime.Now.Date);
                return DbHelper.ExecuteQuery(true, cmd, dt => dt);
            }
        }

        public IEnumerable<PromotionCodeModel> SelectPushPromotion(int minPkid, int pageSize, IEnumerable<int> ruleIds,
            IEnumerable<int> getRuleIds, int days)
        {
            if (getRuleIds != null && getRuleIds.Any() && ruleIds != null && ruleIds.Any())
            {
                string startTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                string endTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd") + " 23:59:59";
                using (var cmd =
                    new SqlCommand(
                        $@"SELECT TOP {pageSize} P.PKID,P.UserId FROM  Gungnir..tbl_PromotionCode AS P WITH(NOLOCK)
                        WHERE P.PKID>@MinPkid"
                        //#if !DEBUG
                        + $@" AND P.EndTime BETWEEN @StartTime AND @EndTime
                             AND RuleId IN({string.Join(",", ruleIds)})
                             AND GetRuleId IN({string.Join(",", getRuleIds)})"
                        //#endif
                        + " ORDER BY P.PKID ASC;"
                    ))
                {
                    cmd.Parameters.AddWithValue("@MinPkid", minPkid);
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);
                    return DbHelper.ExecuteSelect<PromotionCodeModel>(true, cmd);
                }
            }
            return null;
        }

        public IEnumerable<PromotionCodeModel> SelectPushPromotionByUserId(IEnumerable<string> page,
            IEnumerable<int> ruleIds, IEnumerable<int> getRuleIds, int days)
        {
            if (getRuleIds != null && getRuleIds.Any() && page != null && page.Any() && ruleIds != null && ruleIds.Any())
            {
                string startTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                string endTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd") + " 23:59:59";
                using (var cmd =
                    new SqlCommand(
                        $@"SELECT P.UserId,Discount FROM  Gungnir..tbl_PromotionCode AS P WITH(NOLOCK)
                        WHERE UserId IN (SELECT Item FROM dbo.SplitString(@Page,',', 1)) AND Status=0 "
                        //#if !DEBUG
                        + $@" AND P.EndTime BETWEEN @StartTime AND @EndTime
                             AND RuleId IN({string.Join(",", ruleIds)})
                             AND GetRuleId IN({string.Join(",", getRuleIds)})"
                        //#endif
                        + " ORDER BY P.PKID ASC;"
                    ))
                {
                    cmd.Parameters.Add(new SqlParameter("@Page", string.Join(",", page)));
                    cmd.Parameters.Add(new SqlParameter("@StartTime", startTime));
                    cmd.Parameters.Add(new SqlParameter("@EndTime", endTime));
                    return DbHelper.ExecuteSelect<PromotionCodeModel>(true, cmd);
                }
            }
            return null;
        }
    }

    public class TaskPromotionPushDalHelper : IPromotionPushDalHelper
    {
        public DataTable SelectIsPushList()
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT TaskPromotionListId AS GetRuleId,CouponRulesId AS RuleId,PushSetting FROM Gungnir..tbl_PromotionTaskPromotionList AS GCR WITH(NOLOCK) WHERE GCR.IsPush=1 AND (EndTime IS NULL OR EndTime>=@Now)"
                ))
            {
                cmd.Parameters.AddWithValue("@Now", DateTime.Now.Date);
                return DbHelper.ExecuteQuery(true, cmd, dt => dt);
            }
        }

        public IEnumerable<PromotionCodeModel> SelectPushPromotion(int minPkid, int pageSize, IEnumerable<int> ruleIds, IEnumerable<int> getRuleIds, int days)
        {
            if (getRuleIds != null && getRuleIds.Any() && ruleIds != null && ruleIds.Any())
            {
                string startTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                string endTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd") + " 23:59:59";
                using (var cmd =
                    new SqlCommand(
                        $@"SELECT TOP {pageSize} P.PKID,P.UserId FROM  Gungnir..tbl_PromotionCode AS P WITH(NOLOCK)
                        WHERE P.PKID>@MinPkid"
                        //#if !DEBUG
                        + $@" AND P.EndTime BETWEEN @StartTime AND @EndTime
                             AND RuleId IN({string.Join(",", ruleIds)})
                             AND TaskPromotionListId IN({string.Join(",", getRuleIds)})"
                        //#endif
                        + " ORDER BY P.PKID ASC;"
                    ))
                {
                    cmd.Parameters.AddWithValue("@MinPkid", minPkid);
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);
                    return DbHelper.ExecuteSelect<PromotionCodeModel>(true, cmd);
                }
            }
            return null;
        }

        public IEnumerable<PromotionCodeModel> SelectPushPromotionByUserId(IEnumerable<string> page, IEnumerable<int> ruleIds, IEnumerable<int> getRuleIds, int days)
        {
            if (getRuleIds != null && getRuleIds.Any() && page != null && page.Any() && ruleIds != null && ruleIds.Any())
            {
                string startTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                string endTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd") + " 23:59:59";
                using (var cmd =
                    new SqlCommand(
                        $@"SELECT P.UserId,Discount FROM  Gungnir..tbl_PromotionCode AS P WITH(NOLOCK)
                        WHERE UserId IN (SELECT Item FROM dbo.SplitString(@Page,',', 1)) AND Status=0 "
                        //#if !DEBUG
                        + $@" AND P.EndTime BETWEEN @StartTime AND @EndTime
                             AND RuleId IN({string.Join(",", ruleIds)})
                             AND TaskPromotionListId IN({string.Join(",", getRuleIds)})"
                        //#endif
                        + " ORDER BY P.PKID ASC;"
                    ))
                {
                    cmd.Parameters.Add(new SqlParameter("@Page", string.Join(",", page)));
                    cmd.Parameters.Add(new SqlParameter("@StartTime", startTime));
                    cmd.Parameters.Add(new SqlParameter("@EndTime", endTime));
                    return DbHelper.ExecuteSelect<PromotionCodeModel>(true, cmd);
                }
            }
            return null;
        }
    }

}
