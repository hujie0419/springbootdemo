using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.DAL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Job
{
    public class Common
    {
        private static ILog Logger;

        private Common()
        {
        }

        public Common(ILog logger)
        {
            Logger = logger;
        }

        public void Executed(PromotionTaskCls oneTask)
        {
            if (oneTask.SelectUserType == 3) //如果job是从ib库里获取的多数据，则需要同步里面的执行状态
            {
                PromotionDalHelper.UpdatePromotionTaskActivityStatus(oneTask.PromotionTaskActivityId,
                    oneTask.PromotionTaskId, 1);
            }

            SendNotification(oneTask);
        }


        void SendNotification(PromotionTaskCls oneTask)
        {
            //验证job是否已经执行成功
            var taskPromotionList = PromotionDalHelper.GetTaskPromotionList(oneTask.PromotionTaskId);

            var promotionListInfos = taskPromotionList as PromotionListInfo[] ?? taskPromotionList.ToArray();
            if (oneTask.SmsId > 0 || promotionListInfos.Any(x => x.IsRemind > 0))
            {
                var task = PromotionDalHelper.GetPromotionTask(oneTask.PromotionTaskId);
                if (task?.ExecuteTime == null) return;

                var pageSize = 1000; //每次发一千条
                int minPkid = 0;
                Logger.InfoFormat(@"执行塞券任务 Notification =》id:{0} name={1} 开始发 Notification", oneTask.PromotionTaskId, oneTask.TaskName);
                List<string> smsParam = new List<string>();
                if (!string.IsNullOrEmpty(oneTask.SmsParam))
                {
                    smsParam = JsonConvert.DeserializeObject<List<string>>(oneTask.SmsParam);
                }
                while (true)
                {
                    var users = PromotionDalHelper.GetPromotionTaskHistoryUsers(minPkid, pageSize, oneTask.PromotionTaskId);
                    Logger.InfoFormat(@"执行塞券任务 Notification =》id:{0} name={1} 获取到{2}个待发用户 minPkid={3}", oneTask.PromotionTaskId, oneTask.TaskName, users.Count(), minPkid);
                    if (!users.Any())
                    {

                        break;
                    }
                    else
                    {
                        minPkid = users.Last().PromotionSingleTaskUsersHistoryId;
                    }

                    var userObjects = PromotionDalHelper.GetPromotionTaskUserObjects(users.Select(x => x.UserCellPhone));

                    var mobiles = new List<string>();
                    var userIds = new List<string>();
                    foreach (var r in userObjects)
                    {
                        mobiles.Add(r.Mobile);
                        userIds.Add(r.UserId.ToString());
                    }
                    if (oneTask.SmsId > 0)
                    {
                        var request = new Service.Utility.Request.SendBatchCellphoneSmsRequest()
                        {
                            Cellphones = mobiles,
                            TemplateId = oneTask.SmsId,
                            TemplateArguments = smsParam.ToArray()
                        };
                        Logger.InfoFormat(@"执行塞券任务 发短信 =》id:{0} name={1} 准备发送 minPkid={2}", oneTask.PromotionTaskId, oneTask.TaskName, minPkid);
                        using (var client = new Tuhu.Service.Utility.SmsClient())
                        {
                            var response = client.SendBatchSms(request);
                            if (response.Success && response.Result <= 0)
                            {
                                Logger.InfoFormat(@"执行塞券任务 发短信 =》id:{0} name={1} 发送失败 minPkid={2}", oneTask.PromotionTaskId,
                                    oneTask.TaskName, minPkid);
                            }
                            else if (!response.Success)
                            {
                                Logger.Warn("执行塞券任务 发短信 发送出错 ", response.Exception);
                            }
                        }
                        Logger.InfoFormat(@"执行塞券任务 发短信 =》id:{0} name={1} 发送结束 minPkid={2}", oneTask.PromotionTaskId, oneTask.TaskName, minPkid);
                    }

                    foreach (var p in promotionListInfos)
                    {
                        if (p.IsRemind > 0)
                        {
                            Logger.InfoFormat(@"执行塞券任务 发Push =》id:{0} name={1} minPkid={2} 优惠券 {3}",
                                oneTask.PromotionTaskId, oneTask.TaskName, minPkid, p.TaskPromotionListId);
                            using (var client = new Tuhu.Service.Push.TemplatePushClient())
                            {
                                client.PushByUserIDAndBatchID(userIds, 1509,
                                    new Service.Push.Models.Push.PushTemplateLog()
                                    {
                                        Replacement = Newtonsoft.Json.JsonConvert.SerializeObject(
                                            new Dictionary<string, string>
                                            {
                                                ["{{currenttime}}"] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm"),
                                                ["{{couponamount}}"] = p.DiscountMoney.ToString(CultureInfo.InvariantCulture),
                                                ["{{couponname}}"] = p.PromotionDescription
                                            })
                                    });
                            }
                            Logger.InfoFormat(@"执行塞券任务 发Push =》id:{0} name={1} minPkid={2} 发送结束 优惠券 {3}",
                                oneTask.PromotionTaskId, oneTask.TaskName, minPkid, p.TaskPromotionListId);
                        }
                    }

                }
                Logger.InfoFormat(@"执行塞券任务 发短信 =》id:{0} name={1} 所有短信发送完毕", oneTask.PromotionTaskId, oneTask.TaskName);
            }
        }

        public void ExecutBefore(PromotionTaskCls oneTask)
        {
            if (oneTask.Tasktype == 1 && oneTask.SelectUserType == 3 && oneTask.PromotionTaskActivityId > 0 &&
                oneTask.TaskStartTime < DateTime.Now)
            {
                SyncBiActivityData(oneTask);
            }else if (oneTask.Tasktype == 1 && oneTask.SelectUserType == 2 && oneTask.TaskStartTime < DateTime.Now)
            {
                SyncOrderData(oneTask);
            }
        }


        void SyncBiActivityData(PromotionTaskCls oneTask)
        {
            //判断是否已经同步过，同步过就不再执行
            Logger.InfoFormat(@"执行塞券任务 导BI数据 任务符合导入条件 =》id:{0}", oneTask.PromotionTaskId);
            var waitUsers = PromotionDalHelper.GetPromotionTaskUsers(0, 1, oneTask.PromotionTaskId);
            var historyUsers = PromotionDalHelper.GetPromotionTaskHistoryUsers(0, 1, oneTask.PromotionTaskId);
            if (waitUsers.Any() || historyUsers.Any())
            {
                Logger.InfoFormat(@"执行塞券任务 导BI数据 已经导入过数据了 =》id:{0}", oneTask.PromotionTaskId);
                return;
            }

            //把BI表里的数据同步到待发送表里去
            long minPkid = 0;
            int pageSize = 1000;
            var totalCount =
                PromotionDalHelper.SelectPromotionTaskActivityUsersCount(minPkid, oneTask.PromotionTaskId,
                    oneTask.PromotionTaskActivityId);
            int pageIndex = 1;
            int pageTotal = (totalCount - 1) / pageSize + 1;

            while (pageIndex <= pageTotal)
            {
                pageIndex++;
                try
                {
                    var users = PromotionDalHelper.SelectPromotionTaskActivityUsers(minPkid, pageSize,
                        oneTask.PromotionTaskId,
                        oneTask.PromotionTaskActivityId);
                        var promotionTaskActivityUserses = users as PromotionTaskActivityUsers[] ?? users.ToArray();
                        if (!promotionTaskActivityUserses.Any())
                    {
                        Logger.InfoFormat(@"执行塞券任务 导BI数据 没有数据了 =》id:{0} taskActivityId={1}", oneTask.PromotionTaskId,
                            oneTask.PromotionTaskActivityId);
                        break;
                    }
                    Logger.InfoFormat(@"执行塞券任务 导BI数据 =》id:{0} minPkid={1}", oneTask.PromotionTaskId, minPkid);
                        minPkid = promotionTaskActivityUserses.Last().PKID;
                    Logger.InfoFormat(@"执行塞券任务 导BI数据 =》id:{0} maxPkid={1} 导入开始", oneTask.PromotionTaskId, minPkid);
                        PromotionDalHelper.MovePromotionTaskActivityUsers(promotionTaskActivityUserses, oneTask.PromotionTaskId);
                    Logger.InfoFormat(@"执行塞券任务 导BI数据 =》id:{0} maxPkid={1} 导入完成", oneTask.PromotionTaskId, minPkid);
                }
                catch (Exception e)
                {
                    Logger.InfoFormat(@"执行塞券任务 导BI数据 =》id:{0} maxPkid={1} 导入失败", oneTask.PromotionTaskId, minPkid);
                }
            }
        }

        void SyncOrderData(PromotionTaskCls oneTask)
        {
            //判断是否已经同步过，同步过就不再执行
            Logger.InfoFormat(@"执行塞券任务 过滤Order 任务符合导入条件 =》id:{0}", oneTask.PromotionTaskId);
            var waitUsers = PromotionDalHelper.GetPromotionTaskUsers(0, 1, oneTask.PromotionTaskId);
            var historyUsers = PromotionDalHelper.GetPromotionTaskHistoryUsers(0, 1, oneTask.PromotionTaskId);
            if (waitUsers.Any() || historyUsers.Any())
            {
                Logger.InfoFormat(@"执行塞券任务 过滤Order 已经导入过数据了 =》id:{0}", oneTask.PromotionTaskId);
                return;
            }
            PromotionDalHelper.MoveFilterOrderData(oneTask.PromotionTaskId);
            Logger.InfoFormat(@"执行塞券任务 过滤Order 数据导入完毕 =》id:{0}", oneTask.PromotionTaskId);
        }

        public void RunPromotionTask(PromotionTaskCls oneTask)
        {
            //分页获取待塞券的人

            int minPkid = 0;
            int pageSize = 1000;

            var promotionList = PromotionDalHelper.GetTaskPromotionList(oneTask.PromotionTaskId);
            var promotionListInfos = promotionList as PromotionListInfo[] ?? promotionList.ToArray();
            Logger.Info($"执行塞券任务 id:{oneTask.PromotionTaskId} 获取需要塞的优惠券列表 获取到{promotionListInfos.Count()}个优惠券");
            if (!promotionListInfos.Any()) return; //无券可发
            var couponRules = PromotionDalHelper.GetCouponRules(promotionListInfos.Select(x => x.CouponRulesId));
            foreach (var p in promotionListInfos)
            {
                var c = couponRules.FirstOrDefault(x => x.PKID == p.CouponRulesId);
                if (c != null)
                {
                    p.PromotionName = c.Name;
                    p.Type = c.Type;
                }

            }



            while (true)
            {
                var users = PromotionDalHelper.GetPromotionTaskUsers(minPkid, pageSize, oneTask.PromotionTaskId);
                var promotionTaskUsers = users as PromotionTaskUser[] ?? users.ToArray();
                if (!promotionTaskUsers.Any()) break;
                Logger.Info($"执行塞券任务 id:{oneTask.PromotionTaskId}  minPkid:{minPkid} 待发送用户列表 获取到{promotionTaskUsers.Count()}个用户");
                minPkid = promotionTaskUsers.Last().PromotionSingleTaskUsersId;
                Logger.Info($"执行塞券任务 id:{oneTask.PromotionTaskId}  maxPkid:{minPkid}");

                var mobiles = promotionTaskUsers.AsParallel().Select(x => x.UserCellPhone);
                if (oneTask.IsLimitOnce == 1) //去除重复 的手机号
                {
                    mobiles = mobiles.Distinct();
                }
                //获取user对应的userid
                var userObjects =
                    PromotionDalHelper.GetPromotionTaskUserObjects(mobiles);

                //获取要塞的优惠券列表
                var list = userObjects.AsParallel().SelectMany((user) =>
                {
                    var tempList = new List<CreatePromotionModel>();
                    foreach (var promotion in promotionListInfos)
                    {
                        for (int i = 0; i < promotion.Number; i++)
                        {
                            tempList.Add(new CreatePromotionModel()
                            {
                                Author = promotion.Creater,
                                MinMoney = promotion.MinMoney,
                                Discount = promotion.DiscountMoney,
                                DepartmentName = promotion.DepartmentName,
                                IntentionName = promotion.IntentionName,
                                BusinessLineName=promotion.BusinessLineName,
                                Description = promotion.PromotionDescription,
                                UserID = user.UserId,
                                Channel = "Job塞券",
                                StartTime = promotion.StartTime,
                                EndTime = promotion.EndTime,
                                RuleId = promotion.CouponRulesId,
                                Creater = promotion.Creater,
                                IssueChannleId = oneTask.PromotionTaskId.ToString(),
                                IssueChannle = "手动塞券",
                                Issuer = promotion.Issuer,
                                Type = promotion.Type,
                                TaskPromotionListId=promotion.TaskPromotionListId
                            });
                        }
                    }
                    return tempList;
                });

                Logger.Info($"执行塞券任务 id:{oneTask.PromotionTaskId}  maxPkid:{minPkid} 需要发送 {list.Count()} 张券");

                //分批发送 分成 20批发送
                list.GroupBy(x => x.GetHashCode() % 20).ForEach(listItem =>
                {
                    using (var client = new PromotionClient())
                    {
                        var response = client.CreatePromotionsForYeWu(listItem);
                        if (!response.Success)
                        {
                            Logger.Info(
                                $"执行塞券任务 id:{oneTask.PromotionTaskId}  maxPkid:{minPkid} 发送失败  {response.ErrorMessage}",
                                response.Exception);
                        }
                    }
                });
                Logger.Info(
                    $"执行塞券任务 id:{oneTask.PromotionTaskId}  maxPkid:{minPkid} 发送成功");

                //把塞过券的数据移动到历史表里
                PromotionDalHelper.MovePromotionTaskSingleUsers(promotionTaskUsers);

                Logger.Info(
                    $"执行塞券任务 id:{oneTask.PromotionTaskId}  把这批次用户移动到 历史表成功");

                PromotionDalHelper.RemovePromotionTaskSingleUsers(promotionTaskUsers.AsParallel()
                    .Select(x => x.PromotionSingleTaskUsersId));

                if (oneTask.IsLimitOnce==1) //删除重复的手机号
                {
                    PromotionDalHelper.RemovePromotionTaskSingleUsersByMobiles(mobiles,oneTask.PromotionTaskId);
                }

                Logger.Info(
                    $"执行塞券任务 id:{oneTask.PromotionTaskId}  把这批次用户从待发送列表里删除 成功");

            }


            PromotionDalHelper.ClosePromotionTaskJob(oneTask.PromotionTaskId);
            Logger.Info(
                $"执行塞券任务 id:{oneTask.PromotionTaskId} 塞券完毕 关闭任务");
        }


    }
}
