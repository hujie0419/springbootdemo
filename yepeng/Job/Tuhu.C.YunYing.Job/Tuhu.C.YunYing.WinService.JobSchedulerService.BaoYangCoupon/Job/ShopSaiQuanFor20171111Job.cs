using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;
using System.Threading.Tasks;
using Tuhu.Service.Utility;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    [DisallowConcurrentExecution]
    public class ShopSaiQuanFor20171111Job:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<ShopSaiQuanFor20171111Job>();
        private static readonly int PromotionTaskId = 9135;
        private static readonly IEnumerable<Rule> rule200 = new List<Rule>()
        {
            new Rule()
            {
                RuleId=123766,
                Name="全网轮胎券,部分产品不可用(深圳同富路店可用,限到店安装且在线支付）",
                Description="深圳同富路店专享，部分产品不可用（限到店安装且在线支付）",
                Money=499,
                Distinct=100,
                Fix="轮胎499减100"

            },
            new Rule()
            {
                RuleId=123767 ,
                Name="到店保养券(深圳同富路店可用,不适用安装费/年卡/套餐)",
                Description="深圳同富路店专享（不适用安装费/年卡/套餐）",
                Money=199,
                Distinct=100,
                Fix="保养199减100"
            }
        };


        private static readonly IEnumerable<string> testMobile = new List<string>()
        {
            "13166308063",
            "17621701430",
            "15385123650",
            "13818369094",
            "13817329026",
            "15021159091",
            "18516292400"
        };
        private static readonly bool IsTest = false;
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("工厂店塞券Job启动");
            var dataMap = context.JobDetail.JobDataMap;
            string ActionName = dataMap.GetString("ActionName");
            if (string.Equals(ActionName, "SendSms", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Info("工厂店塞券Job 发短信开始");
                SendSms();
                Logger.Info("工厂店塞券Job 发短信结束");
            }
            else
            {
                Logger.Info("工厂店塞券Job 塞券开始");
                AddPromotion();
                Logger.Info("工厂店塞券Job 塞券结束");
            }
            Logger.Info("工厂店塞券Job结束");
        }

        public void SendSms()
        {
            int pkid = 0;
            int page = 100;
            while (true)
            {
                IEnumerable<User> list = GetUsers(pkid, page);
                if (!list.Any()) break;
                Logger.Info($"工厂店塞券Job发送 短信 当前最小pkid{pkid} 最大pkid{list.Last().PKID}");
                Parallel.ForEach(list, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, x =>
                {
                    IEnumerable<Rule> rules = null;
                    //if (x.Type == "200元券")
                    //{
                        rules = rule200;
                    //}
                    //else if (x.Type == "300元券")
                    //{
                    //    rules = rule300;
                    //}
                    //验证这个人是否真的发了券
                    if (ValidatePromotion(x.UserId,rules.Select(_=>_.RuleId)) > 0)
                    {
                        //[TODO]发送短信
                        SmsClient client = null;
                        try
                        {
                            client = new SmsClient();

                            var response = client.SendSms(x.Mobile, 156, x.ShortUrl, "200",
                                string.Join(",", rules.Select(_ => _.Fix)));

                            response.ThrowIfException(true);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"ShopSaiQuanFor20171111Job:{x.Mobile}", ex);
                        }
                        finally
                        {
                            client?.Dispose();
                        }
                    }

                });
                Logger.Info($"工厂店塞券Job发送  短信 一批发送完最大pkid {list.Last().PKID}");
                pkid = list.Last().PKID;
            }
        }

        public void AddPromotion()
        {
            int pkid = 0;
            int page = 1000;
            while (true)
            {
                IEnumerable<User> list = GetUsers(pkid, page);
                if (!list.Any()) break;
                Logger.Info($"工厂店塞券Job发送 优惠券 当前最小pkid{pkid} 最大pkid{list.Last().PKID}");
                Parallel.ForEach(list, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, x =>
                {
                    int result = 0;
                    IEnumerable<Rule> rules = null;
                    //if (x.Type == "200元券")
                    //{
                        rules = rule200;
                    //}
                    //else if (x.Type == "300元券")
                    //{
                    //    rules = rule300;
                    //}
                    if (ValidatePromotion(x.UserId, rules.Select(_ => _.RuleId), false)==0)//没有塞过才给他塞券
                    {
                        CreateOrYzPromotion(x.UserId, rules);
                    }
                    
                });
                Logger.Info($"工厂店塞券Job发送  优惠券 一批发送完最大pkid {list.Last().PKID}");
                pkid = list.Last().PKID;
            }
        }

        public IEnumerable<User> GetUsers(int pkid, int page)
        {
            string conn = System.Configuration.ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            IEnumerable<User> list = new List<User>();

            using (var db = Tuhu.DbHelper.CreateDbHelper(conn))
            {
                string sql =
                    $@"SELECT TOP {
                            page
                        } U.PKID,U.UserTel AS Mobile FROM Tuhu_bi..tbl_PromotionTaskActivityUsers AS U with(nolock) JOIN Tuhu_bi..tbl_PromotionTaskActivity AS A with(nolock) ON A.PKID=U.PromotionTaskActivityId 
WHERE A.PromotionTaskId={PromotionTaskId} AND A.Status=0 AND U.PKID>@PKID ";
                if (IsTest)
                {
                    sql += $" AND U.UserTel IN({string.Join(",", testMobile.Select(m => $"'{m}'"))})";
                }
                sql += " ORDER BY U.PKID ASC;";
                using (var cmd = db.CreateCommand(sql))
                {
                    cmd.Parameters.Add(new SqlParameter("@PKID", pkid));
                    list= db.ExecuteSelect<User>(cmd);
                }
            }
            int PKID = 0;
            if (list.Any())
            {
                PKID = list.Last().PKID;
                using (var cmd =
                    new SqlCommand(
                        $@"SELECT {
                                PKID
                            }AS PKID,UserId,u_mobile_number AS Mobile,'l.tuhu.cn/6sOBl' AS ShortUrl FROM Tuhu_profiles..UserObject WITH(NOLOCK)
WHERE u_mobile_number IN({string.Join(",", list.Select(x => $"'{x.Mobile}'"))}) AND IsActive=1")
                )
                {
                    return DbHelper.ExecuteSelect<User>(true, cmd);
                }
            }
            else
            {
                return new List<User>();
            }

        }

        public int ValidatePromotion(string userId, IEnumerable<int> ruleIds, bool readOnly = true)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT COUNT(1) FROM Gungnir..tbl_PromotionCode WITH(NOLOCK) WHERE UserId=@UserId AND RuleID in({
                            string.Join(",", ruleIds)
                        }) AND CreateTime > '{DateTime.Now.Date:yyyy-MM-dd}' AND Status!=3")
            )
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                return (int) DbHelper.ExecuteScalar(readOnly, cmd);
            }
        }

        public class User
        {
            public int PKID { get; set; }
            public string UserId { get; set; }
            public string Type { get; set; }
            public string Mobile { get; set; }
            public string ShortUrl { get; set; }
        }

        public class Rule
        {
            public int RuleId { get; set; }
            public int Money { get; set; }
            public int Distinct { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
            public string Fix { get; set; }
        }

        public static int CreateOrYzPromotion(string userId, IEnumerable<Rule> rules)
        {
            int result = 0;
            BaseDbHelper db = null;
            try
            {
                db = DbHelper.CreateDbHelper();
                db.BeginTransaction();

                foreach (var rule in rules)
                {
                    var promotionCodeModel = new PromotionCodeModel();
                    promotionCodeModel.OrderId = 0;
                    promotionCodeModel.Status = 0;
                    promotionCodeModel.Description = rule.Description;
                    promotionCodeModel.PromtionName = rule.Name;
                    promotionCodeModel.Discount = rule.Distinct;
                    promotionCodeModel.MinMoney = rule.Money;
                    promotionCodeModel.EndTime = DateTime.Parse("2017-12-13 23:59:59");
                    promotionCodeModel.StartTime = DateTime.Parse("2017-12-06");
                    promotionCodeModel.UserId = new Guid(userId);
                    promotionCodeModel.CodeChannel = $"工厂店塞券{rule.RuleId}";
                    promotionCodeModel.RuleId = rule.RuleId;
                    promotionCodeModel.Number = 1;
                    result += DalCoupon.CreatePromotionCode(promotionCodeModel, db, "ShopSaiQuanFor20171111Job","门店", "工场店新店开业");
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
            }
            finally
            {
                db.Dispose();
            }
            return result;
        }
    }
}
