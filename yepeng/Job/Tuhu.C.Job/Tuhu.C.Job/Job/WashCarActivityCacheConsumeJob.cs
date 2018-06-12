using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using Tuhu.Nosql;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class WashCarActivityCacheConsumeJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WashCarActivityCacheConsumeJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");
            AsyncHelper.RunSync(TaskMethod);
            Logger.Info("结束任务");
        }

        public async Task TaskMethod()
        {
            try
            {
                var list = new List<UserApplyActivityModel>();
                var noRepeatList = new List<string>();
                using (var client = new ActivityClient())
                {
                    var result = await client.GetUserApplyActivityRangeByScoreAsync();
                    if (result.Success)
                    {
                        using (var activityClient = new ActivityClient())
                        {
                            foreach (var item in result.Result)
                            {
                                if (await CheckApplyUserCountAsync(item.ActivityId))
                                {
                                    //检查用户手机号、车牌号、驾驶证号是否已经使用
                                    var isExist =
                                        await activityClient.CheckUserApplyActivityInfoIsExistAsync(item.ActivityId,
                                            item.Mobile, item.CarNum, item.DriverNum);
                                    if (isExist.Success && !isExist.Result)
                                    {
                                        User user = null;
                                        using (var userClient = new UserAccountClient())
                                        {
                                            try
                                            {
                                                var userResult = userClient.GetUserByMobile(item.Mobile);
                                                userResult.ThrowIfException(true);
                                                user = userResult.Result;
                                                if (user == null)
                                                {
                                                    var createResult = userClient.CreateUserRequest(
                                                        new CreateUserRequest
                                                        {
                                                            MobileNumber = item.Mobile,
                                                            Profile = new UserProfile
                                                            {
                                                                UserName = item.UserName,
                                                                NickName = null,
                                                                HeadUrl = null
                                                            },
                                                            ChannelIn = nameof(ChannelIn.None),
                                                            UserCategoryIn = nameof(UserCategory.Tuhu),
                                                        });
                                                    createResult.ThrowIfException(true);
                                                    user = createResult.Result;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Logger.Error("用户创建失败。" + ex.Message, ex);
                                            }
                                        }
                                        //添加到待审核集合中
                                        if (!noRepeatList.Contains(item.CarNum) &&
                                            !noRepeatList.Contains(item.Mobile) &&
                                            !noRepeatList.Contains(item.DriverNum))
                                        {
                                            list.Add(item);
                                            noRepeatList.AddRange(new[] { item.Mobile, item.CarNum, item.DriverNum });
                                        }
                                        //从SortedSet中移除
                                        var removeResult = await client
                                            .RemoveOneUserApplyActivitySortedSetCacheAsync(item);
                                        if (!removeResult.Success)
                                        {
                                            Logger.Error(removeResult.ErrorMessage, removeResult.Exception);
                                        }
                                        item.UserId = user.UserId;
                                    }
                                    else
                                    {
                                        Logger.Error(isExist.ErrorMessage, isExist.Exception);
                                    }
                                }
                            }
                        }

                    }
                }
                if (list.Any())
                {
                    await WashCarActivityMinHangAutoAuditAsync(list);
                }
                list.Clear();
                noRepeatList.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        private async Task WashCarActivityMinHangAutoAuditAsync(List<UserApplyActivityModel> userApplyActivityModels,
            int batchInsertCount = 2)
        {
            try
            {
                var list = new List<UserApplyActivityModel>();
                var serviceCodeDic = new Dictionary<string, Guid>();
                using (var ht = new HttpClient())
                {
                    ht.DefaultRequestHeaders.Add("Authorization", "APPCODE " + "4aca04217e154a2487d5f595e749a28f");
                    foreach (var model in userApplyActivityModels)
                    {
                        if (model.DriverNum.StartsWith("310112") && model.CarNum.StartsWith("沪") &&
                            !model.CarNum.ToLower().Contains("沪c")) //闵行区自动审核通过
                        {
                            if (await CheckApplyUserCountAsync(model.ActivityId))
                            {
                                var serviceCode = Guid.NewGuid();
                                model.Region = "上海市,闵行区";
                                model.Status = AuditStatus.Passed;
                                model.ServiceCode = serviceCode;
                                list.Add(model);
                                serviceCodeDic[model.Mobile] = serviceCode;//
                            }
                        }
                        else
                        {
                            var url = $"http://idquery.market.alicloudapi.com/idcard/query?number={model.DriverNum}";
                            var jsonStr = await ht.GetStringAsync(url);
                            dynamic jObject = JObject.Parse(jsonStr);
                            model.Status = AuditStatus.Pending;
                            if (jObject.ret == "200")
                            {
                                model.Region = $"{jObject.data.prov},{jObject.data.city},{jObject.data.region}";
                                list.Add(model);
                            }
                            else
                            {
                                model.Region = "未知";
                                list.Add(model);
                            }
                        }
                    }
                }
                var sb = new StringBuilder();
                int singleInsertCount = batchInsertCount;
                var count = Math.Ceiling(list.Count * 1.0f / singleInsertCount);
                for (int i = 0; i < count; i++)
                {
                    var currentList = list.Skip(i * singleInsertCount).Take(singleInsertCount);
                    foreach (var item in currentList)
                    {
                        sb.Append($@"INSERT INTO Activity..T_UserApplyActivity
                                    (
                                        UserId,
                                        ActivityId,
                                        Mobile,
                                        CarNum,
                                        DriverNum,
                                        Ip,
                                        Region,
                                        Status,
                                        Remark,
                                        ServiceCode,
                                        ExpirationTime,
                                        CreatedDateTime,
                                        UpdatedDateTime
                                    )
                                    VALUES
                                    (   '{item.UserId}',      
                                        '{item.ActivityId}',      
                                        N'{item.Mobile}',       
                                        N'{item.CarNum}',       
                                        N'{item.DriverNum}',       
                                        N'{item.Ip}',       
                                        N'{item.Region}',       
                                        {(int)item.Status},         
                                        N'{item.Remark}',       
                                        '{item.ServiceCode}',      
                                        NULL, 
                                        GETDATE(), 
                                        GETDATE()  
                                        );");
                    }
                    //批量插入数据
                    await DbHelper.ExecuteNonQueryAsync(sb.ToString(), CommandType.Text);
                    sb.Clear();
                }

                foreach (var dic in serviceCodeDic)
                {
                    //发送通知短信
                    using (var client = new Service.Utility.SmsClient())
                    {
                        client.SendSms(dic.Key, 138, dic.Value.ToString());
                    }
                }
                list.Clear();
                serviceCodeDic.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        private async Task<bool> CheckApplyUserCountAsync(Guid activityId)
        {
            //判断当前活动审核通过人数是否已达上限
            using (var activityClient = new ActivityClient())
            {
                //检查活动审核通过人数是否已满
                var applyUserPassCountResult =
                    await activityClient.GetActivityApplyUserPassCountByActivityIdAsync(activityId);
                var activityModelResult =
                    await activityClient.GetActivityModelByActivityIdAsync(activityId);
                if (applyUserPassCountResult.Success && activityModelResult.Success)
                {
                    if (applyUserPassCountResult.Result >= activityModelResult.Result.Quota)
                    {
                        return false;
                    }
                }
                else
                {
                    Logger.Error($"审核通过人数查询失败。Message:{(applyUserPassCountResult.Success ? applyUserPassCountResult.Exception : activityModelResult.Exception)}");
                    return false;
                }

            }
            return true;
        }
    }
}
