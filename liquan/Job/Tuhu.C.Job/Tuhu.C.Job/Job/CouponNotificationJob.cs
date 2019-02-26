using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;

namespace Tuhu.C.Job.Job
{

    [DisallowConcurrentExecution]
    public class CouponNotificationJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CouponNotificationJob));
        public static int ThreadCount = 3;
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("开始执行 CouponNotificationJob ");
                var dataMap = context.JobDetail.JobDataMap;
                bool isPush = dataMap.GetBoolean("IsPush");
                Logger.Info($"CouponNotificationJob IsPush = {isPush}");

                if (isPush)//获取当月已经得到的需要推送的数据进行推送
                {
                    ThreadCount = dataMap.GetInt("ThreadCount");
                    if (ThreadCount == 0) ThreadCount = 10;

                    Notification();
                    Logger.Info($"CouponNotificationJob Nnotification Success");
                }
                else//获取当月需要推送的数据存到数据库，等待Push
                {
                    CalculationNotificationData();
                    Logger.Info($"CouponNotificationJob CalculationNotificationData Success");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }

        }


        //一般是凌晨执行数据生成Job
        public void CalculationNotificationData()
        {
            DateTime now = DateTime.Now;
            string startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
            string endDate = $"{new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)):yyyy-MM-dd} 23:59:59";
            if (CheckIsExecuted(startDate, endDate)) return;
            int start = 1;
            int page = 100;//每次取100条
            int end = page;

            Guid? lastUserId = null; //用来记录每次最后一条和下一页的第一条的userid是否重复，因为一个人有多个车型，每页和每页之间衔接的地方直接分页，可能会重复

            //线判断这个人领取券的情况，过滤掉已经领取的人，然后插入数据库
            //这两个编号都是Setting配置的
#if DEBUG
            long promotionCode = 122321;//洗车券
            long carFue = 122322;//加油卡
#else
            long promotionCode = 122181;//洗车券
            long carFue = 122182;//加油卡
#endif


            //根据固定的券ID 获取 规则GUID ，发现可能有多个规则
            var ruleGuidDic = GetRuleGUID(new List<long> { promotionCode, carFue });
            var guidList = ruleGuidDic.Values.SelectMany(x => x).ToList();
            if (!guidList.Any())
            {
                return;
            }
            //根据获取到的规则ID，获取礼包ID，因为业务系统只保存了规则ID
            var fkPkidDic = GetParentPkid(guidList);

            //把对应关系转换为 key:券id,value :fkpkid 券id包含的礼包id集合
            var filterDic = ConvertToFkPkid(ruleGuidDic, fkPkidDic, promotionCode, carFue);
            //如果查出来的数据有误,则Job不允许执行
            if (!(filterDic.Count == 2 && filterDic.ContainsKey(promotionCode) && filterDic.ContainsKey(carFue)))
            {
                return;
            }
            int totalCount = GetVehicleListCount();
            while (start <= totalCount)
            {
                var dt = GetVehicleListDt(start, end);//获取到认证车型车主和车型

                if (dt != null && dt.Rows.Count > 0)
                {
                    var dic = dt.Rows.OfType<DataRow>().Where(_ => _.GetValue<Guid>("UserId") != lastUserId)
                        .Select(x => new { UserID = x.GetValue<Guid>("UserId"), CarId = x.GetValue<string>("CarId") })
                        .ToList();
                    if (dic.Count > 0)
                        lastUserId = dic.Last().UserID;
                    else
                        break;


                    //获取已经领取过券的人的信息
                    var filterDt = GetPromotionFilterDt(startDate, endDate, dic.Select(_ => _.UserID), filterDic.Values.SelectMany(_ => _).ToList());

                    //获取转换过的过滤数据
                    var filterData = GetFilterData(filterDt);


                    //插入到Tuhu_log.CouponNotificationUserLog
                    var data = new DataTable() {TableName = "Tuhu_log.dbo.CouponNotificationUserLog"};
                    data.Columns.AddRange(new []
                    {
                        new DataColumn("UserId", typeof(Guid)),
                        new DataColumn("Type", typeof(int)),
                        new DataColumn("CarId", typeof(Guid)),
                        new DataColumn("CreateTime", typeof(DateTime)) {DefaultValue = DateTime.Now},
                        new DataColumn("Status", typeof(int)) {DefaultValue = 0}
                    });
                    foreach (var d in dic)
                    {
                        bool isP = false; //是否领取过洗车券
                        bool isF = false; //是否领取过加油卡
                        if (filterData != null && filterData.ContainsKey(d.UserID))
                        {
                            var filterList = filterData[d.UserID];
                            if (filterList != null)
                            {

                                if (filterList.Intersect(filterDic[promotionCode]).Any())
                                {
                                    isP = true;
                                }

                                if (filterList.Intersect(filterDic[carFue]).Any())
                                {
                                    isF = true;
                                }
                            }
                        }
                        if (isP && isF) //如果两种券都领取过，那么就不再推送了
                        {
                            continue;
                        }
                        var type = 1; //默认两种都没有领取过
                        if (isP || isF) //如果领取过其中一种券，则根据领取的券，推送不同的内容
                        {
                            //没领取洗车券为2，没领取加油卡为3
                            type = isP ? 3 : 2;
                        }
                        var row = data.NewRow();
                        row.SetField("UserId", d.UserID);
                        row.SetField("Type", type);
                        row.SetField("CarId", d.CarId);
                        data.Rows.Add(row);
                    }

                    if (data.Rows.Count > 0)//把得到的结果批量插入到数据库
                    {
                        using (var helper = DbHelper.CreateLogDbHelper())
                        {
                            try
                            {
                                helper.BeginTransaction();
                                helper.BulkCopy(data);
                                helper.Commit();
                            }
                            catch
                            {
                                helper.Rollback();
                            }
                        }
                    }
                }
                else
                {
                    break;
                }

                start += page;
                end += page;
                System.Threading.Thread.Sleep(100);
            }

        }


        private bool CheckIsExecuted(string startDate, string endDate)
        {
            DataSet ds = new DataSet();
            using (var helper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = helper.CreateCommand("SELECT TOP 1 * FROM Tuhu_log.dbo.CouponNotificationUserLog WITH(NOLOCK) WHERE CreateTime BETWEEN @startDate AND @endDate"))
                {
                    cmd.Parameters.Add(new SqlParameter("@startDate", startDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", endDate));

                    return helper.ExecuteQuery(cmd, dt => dt.Rows.Count > 0);
                }
            }
        }

        private Dictionary<long, List<long>> ConvertToFkPkid(Dictionary<long, List<Guid>> ruleGuidDic, Dictionary<Guid, List<long>> fkPkidDic, long promotionCode, long carFue)
        {
            var filterDic = new Dictionary<long, List<long>>();

            if (ruleGuidDic.ContainsKey(promotionCode))
            {
                var pGuids = ruleGuidDic[promotionCode];
                filterDic.Add(promotionCode, new List<long>());
                foreach (var guid in pGuids)
                {
                    if (fkPkidDic.ContainsKey(guid))
                    {
                        var pfkPkid = fkPkidDic[guid];
                        filterDic[promotionCode].AddRange(pfkPkid);
                    }
                }

            }
            if (ruleGuidDic.ContainsKey(carFue))
            {
                var pGuids = ruleGuidDic[carFue];
                filterDic.Add(carFue, new List<long>());
                foreach (var guid in pGuids)
                {
                    if (fkPkidDic.ContainsKey(guid))
                    {
                        var pfkPkid = fkPkidDic[guid];
                        filterDic[carFue].AddRange(pfkPkid);
                    }
                }
            }
            return filterDic;
        }

        private Dictionary<Guid, List<long>> GetFilterData(DataTable filterDt)
        {
            Dictionary<Guid, List<long>> filterData = null;
            if (filterDt != null && filterDt.Rows.Count > 0)
            {
                var filterDic = filterDt.Rows.OfType<DataRow>()
                    .Select(_ => new
                    {
                        UserId = Guid.Parse(_["UserId"].ToString()),
                        IssueChannleId = long.Parse(_["FKPKID"].ToString())
                    }).ToList();
                filterData = filterDic.GroupBy(_ => _.UserId)
                    .ToDictionary(_ => _.Key, _ => _.Select(x => x.IssueChannleId).ToList());
            }
            return filterData;
        }

        /// <summary>
        /// 获取券对应的GUID，用来获取礼包ID
        /// </summary>
        /// <param name="ruleIds"></param>
        /// <returns></returns>
        private Dictionary<long, List<Guid>> GetRuleGUID(List<long> ruleIds)
        {
            DataTable dt;
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = helper.CreateCommand($@"SELECT RuleID,GetRuleGUID FROM Activity.dbo.tbl_GetCouponRules WITH (NOLOCK)
                                WHERE RuleID IN({string.Join(",", ruleIds)})"))
                {
                    dt = helper.ExecuteQuery(cmd, _ => _);
                }
            }

            var dic = dt.Rows.OfType<DataRow>().GroupBy(x => long.Parse(x["RuleID"].ToString()))
                .ToDictionary(k => k.Key, v =>
                {
                    return v
                        .Where(_ => !string.IsNullOrEmpty(_["GetRuleGUID"].ToString()))
                        .Select(_ => Guid.Parse(_["GetRuleGUID"].ToString())).Distinct().ToList();
                });
            return dic;
        }
        /// <summary>
        /// 获取券对应的礼包id
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private Dictionary<Guid, List<long>> GetParentPkid(List<Guid> guids)
        {
            DataTable dt;
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = helper.CreateCommand($@"SELECT ParentPKID,CouponGuid FROM Configuration..BigBrandRewardPool WITH (NOLOCK) 
                                    WHERE CouponGuid IN ('{string.Join("','", guids)}')"))
                {
                    dt = helper.ExecuteQuery(cmd, _ => _);
                }
            }

            var dic = dt.Rows.OfType<DataRow>().GroupBy(x => Guid.Parse(x["CouponGuid"].ToString()))
                .ToDictionary(k => k.Key, v =>
                {
                    return v
                    .Where(_ => !string.IsNullOrEmpty(_["ParentPKID"].ToString()))
                        .Select(_ => long.Parse(_["ParentPKID"].ToString())).Distinct().ToList();
                });
            return dic;
        }



        private DataTable GetPromotionFilterDt(string startDate, string endDate, IEnumerable<Guid> dic, List<long> fkPkid)
        {
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = helper.CreateCommand($@"
                    SELECT UserId,FKPKID 
                    FROM SystemLog.dbo.BigBrandRewardLog  
                    WITH (NOLOCK) WHERE Status=1
                    AND CreateDateTime BETWEEN @startDate AND  @endDate 
                    AND UserId IN ({string.Join(",", dic.Select(_ => $"'{_.ToString()}'"))})
                    AND FKPKID IN ({string.Join(",", fkPkid)})
                    ORDER BY UserId ASC, FKPKID ASC; ")) //获取已经领取过的人，要把这些人过滤掉
                {
                    cmd.Parameters.Add(new SqlParameter("@startDate", startDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", endDate));

                    return helper.ExecuteQuery(cmd, _ => _);
                }
            }
        }

        private DataTable GetVehicleListDt(int start, int end)
        {
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = helper.CreateCommand(@"
                SELECT UserId,CarId FROM 
                    (SELECT 
                    ROW_NUMBER() OVER(ORDER BY co.UserId) AS Num, 
                    ROW_NUMBER() OVER(PARTITION BY co.UserId ORDER BY co.UserId DESC, co.IsDefaultCar DESC, co.dt_date_last_changed DESC) AS rn,
                    co.CarId, co.UserId, co.IsDefaultCar
                    FROM Tuhu_profiles.dbo.CarObject  co WITH(NOLOCK)
                    Left join Tuhu_profiles..VehicleTypeCertificationInfo As VTC with(nolock) on VTC.CarId = co.CarID
                    WHERE  IsDeleted = 0  AND VTC.Status = 1) AS T
                WHERE T.rn = 1 AND T.Num BETWEEN @start AND @end"))
                {
                    cmd.Parameters.Add(new SqlParameter("@start", start));
                    cmd.Parameters.Add(new SqlParameter("@end", end));

                    return helper.ExecuteQuery(cmd, _ => _);

                }
            }
        }

        private int GetVehicleListCount()
        {
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = helper.CreateCommand(@"SELECT COUNT(1)
                    FROM Tuhu_profiles.dbo.CarObject  co WITH(NOLOCK)
                    Left join Tuhu_profiles..VehicleTypeCertificationInfo As VTC with(nolock) on VTC.CarId = co.CarID
                    WHERE  IsDeleted = 0  AND VTC.Status = 1"))
                {

                    return (int)helper.ExecuteScalar(cmd);

                }
            }
        }

        //获取需要推送的数据，进行推送
        public void Notification()
        {
            int minPkid = 0;
            int pageIndex = 1;
            int page = 1000;//每次取1000条

            //获取当月生成的需要推送的人的信息，发送Push
            DateTime now = DateTime.Now;
            string startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
            string endDate = $"{new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)):yyyy-MM-dd} 23:59:59";
            int totalCount = GetNotificationCount(startDate, endDate);
            int pageTotal = (totalCount - 1) / page + 1;
            while (pageIndex<=pageTotal)
            {
                pageIndex++;
                var dt = GetNotificationDt(startDate, endDate, page, minPkid);
                if (dt != null && dt.Rows.Count > 0)
                {
                    minPkid = dt.Rows.OfType<DataRow>().Last().GetValue<int>("PKID");
                    //得到数据之后，把数据归组，2，和3，可以批量发送，1，因为有参数，所以只能单个一个一个发送
                    DoPush(dt);
                }
                else
                {
                    break;
                }
                
                System.Threading.Thread.Sleep(100);
            }

        }

        private void DoPush(DataTable dt)
        {
            var dic = dt.Rows.OfType<DataRow>().GroupBy(x => int.Parse(x["Type"].ToString())).ToDictionary(k => k.Key,
                v => v.Select(_ => new { UserId = _["UserId"].ToString().ToUpper(), PKID = long.Parse(_["PKID"].ToString()), Status = int.Parse(_["Status"].ToString()), CarId = _["CarId"].ToString() }).ToList());

            if (dic.ContainsKey(1)) //单独发送
            {
                Parallel.ForEach(dic[1], new ParallelOptions() {MaxDegreeOfParallelism = ThreadCount}, temp =>
                {
                    using (var client = new Service.Push.TemplatePushClient())
                    {
                        client.PushByUserIDAndBatchID(new List<string> { temp.UserId }, 833,
                            new Service.Push.Models.Push.PushTemplateLog()
                            {
                                Replacement = Newtonsoft.Json.JsonConvert.SerializeObject(
                                    new Dictionary<string, string>
                                    {
                                        ["{{carID}}"] = temp.CarId
                                    })
                            });
                    }
                });

                if (dic[1].Any()) UpdateStatus(dic[1].Select(x=>x.PKID).ToList());
            }
            if (dic.ContainsKey(2)) //批量发送 没有领取洗车券的
            {
                if (dic[2].Count > 0)
                {
                    using (var client = new Service.Push.TemplatePushClient())
                    {
                        client.PushByUserIDAndBatchID(dic[2].Select(_ => _.UserId).ToList(), 836,
                            new Service.Push.Models.Push.PushTemplateLog());
                    }
                    UpdateStatus(dic[2].Select(_ => _.PKID).ToList());
                }

            }
            if (dic.ContainsKey(3)) //批量发送 没有领取加油卡的
            {
                if (dic[3].Count > 0)
                {
                    using (var client = new Service.Push.TemplatePushClient())
                    {
                        client.PushByUserIDAndBatchID(dic[3].Select(_ => _.UserId).ToList(), 839,
                            new Service.Push.Models.Push.PushTemplateLog());
                    }
                    UpdateStatus(dic[3].Select(_ => _.PKID).ToList());
                }

            }
        }

        private void UpdateStatus(List<long> pkids)
        {
            using (var helper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = helper.CreateCommand($@"UPDATE Tuhu_log.dbo.CouponNotificationUserLog WITH(ROWLOCK) SET [Status]=1 WHERE PKID IN ({string.Join(",", pkids)})"))
                {
                    helper.ExecuteNonQuery(cmd);
                }
            }
        }

        private DataTable GetNotificationDt(string startDate, string endDate, int page,int minPkid)
        {
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = helper.CreateCommand($@"SELECT TOP {page} [PKID],[UserId],[Type],[CarId],ISNULL([Status],0) AS Status 
                        FROM Tuhu_log.dbo.CouponNotificationUserLog WITH(NOLOCK)
                        WHERE CreateTime BETWEEN @startDate AND @endDate AND PKID>@MinPkid AND ISNULL([Status],0)=0 ORDER BY PKID ASC"))
                {
                    cmd.Parameters.Add(new SqlParameter("@startDate", startDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", endDate));
                    cmd.Parameters.Add(new SqlParameter("@MinPkid", minPkid));
                    return helper.ExecuteQuery(cmd, _ => _);
                }
            }
        }

        private int GetNotificationCount(string startDate, string endDate)
        {
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = helper.CreateCommand($@"SELECT COUNT(1)
                        FROM Tuhu_log.dbo.CouponNotificationUserLog WITH(NOLOCK)
                        WHERE CreateTime BETWEEN @startDate AND @endDate AND ISNULL([Status],0)=0"))
                {
                    cmd.Parameters.Add(new SqlParameter("@startDate", startDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", endDate));
                    return (int) helper.ExecuteScalar(cmd);
                }
            }
        }
    }
}
