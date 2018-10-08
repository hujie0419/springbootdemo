using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Nosql;
using Tuhu;
using Common.Logging;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.ZooKeeper;
using System.Threading;

namespace Tuhu.Service.Activity.Server.Manager
{
   public class FightGroupsPacketsManager
    {

        public static readonly ILog Logger = LogManager.GetLogger(typeof(FightGroupsPacketsManager));

        /// <summary>
        /// 分享红包缓存关键key
        /// </summary>
        private static readonly string FightGroupPacketsListRedisKey = "FightGroupsPacketsList";

        /// <summary>
        /// 获取分享红包组列表
        /// </summary>
        /// <param name="fightGroupsIdentity"></param>
        /// <returns></returns>
        public static async Task<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> GetFightGroupsPacketsList(Guid? fightGroupsIdentity,Guid userId)
        {
            using (var client = CacheHelper.CreateCacheClient(FightGroupPacketsListRedisKey))
            {
                if (fightGroupsIdentity == null)
                {
                    var resultIng = await client.GetOrSetAsync<List<FightGroupsPacketsLogModel>>(userId.ToString(), async () => await DalFightGroupsPacketsLog.SelectFightGroupsPacketByUser(userId), TimeSpan.FromDays(1));
                 
                    if (resultIng?.Value == null || resultIng?.Value?.Count <= 0)
                        return null;

                    Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse repModel = new FightGroupPacketListResponse()
                    {
                        EndDateTask = ConvertDateTimeLong(resultIng?.Value?.FirstOrDefault()?.CreateDateTime.AddDays(1)),
                        Items = resultIng?.Value,
                        ProvideCount = resultIng?.Value?.Where(_ => _.UserId != null)?.Count()
                    };
                    return repModel;
                }
                else
                {
                    var result = await client.GetOrSetAsync<List<FightGroupsPacketsLogModel>>(fightGroupsIdentity.Value.ToString(), async () => await DalFightGroupsPacketsLog.GetFightGroupsPacketsList(fightGroupsIdentity.Value), TimeSpan.FromDays(1));
                    if (result.Value == null || result?.Value?.Count<=0)
                    {
                        await client.RemoveAsync(fightGroupsIdentity.Value.ToString());
                        result = await client.GetOrSetAsync<List<FightGroupsPacketsLogModel>>(fightGroupsIdentity.Value.ToString(), async () => await DalFightGroupsPacketsLog.GetFightGroupsPacketsList(fightGroupsIdentity.Value), TimeSpan.FromDays(1));
                    }

                    if (result.Value == null || result?.Value?.Count <= 0)
                        return null;

                    Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse responseModel = new FightGroupPacketListResponse()
                    {
                        EndDateTask = ConvertDateTimeLong(result?.Value?.FirstOrDefault()?.CreateDateTime.AddDays(1)),
                        Items = result?.Value,
                        ProvideCount = result?.Value?.Where(_ => _.UserId != null)?.Count()
                    };
                    return responseModel;
                }
            }
        }

        /// <summary>
        /// 添加分享红包组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> InsertFightGroupsPacket(Guid userId)
        {
            try
            {
                Guid fightGroupsIdentity = Guid.NewGuid();
                List<FightGroupsPacketsLogModel> list = new List<FightGroupsPacketsLogModel>()
                {
                    new FightGroupsPacketsLogModel(){ CreateDateTime=DateTime.Now, FightGroupsIdentity= fightGroupsIdentity, IsLeader=true, LastUpdateDateTime=DateTime.Now, OrderBy=1, UserId =userId },
                    new FightGroupsPacketsLogModel(){ CreateDateTime=DateTime.Now, FightGroupsIdentity= fightGroupsIdentity, IsLeader=false, LastUpdateDateTime=DateTime.Now, OrderBy=2},
                    new FightGroupsPacketsLogModel(){ CreateDateTime=DateTime.Now, FightGroupsIdentity= fightGroupsIdentity, IsLeader=false, LastUpdateDateTime=DateTime.Now, OrderBy=3},
                    new FightGroupsPacketsLogModel(){ CreateDateTime=DateTime.Now, FightGroupsIdentity= fightGroupsIdentity, IsLeader=false, LastUpdateDateTime=DateTime.Now, OrderBy=4}
                };



                List<string> array = System.Configuration.ConfigurationManager.AppSettings["FightGroupsGuids"]?.ToString()?.Split(',')?.ToList();
                foreach (var item in list)
                {
                    Random random = new Random(Guid.NewGuid().GetHashCode());
                    int index = random.Next(0, array.Count);
                    item.GetRuleGuid = Guid.Parse(array[index]);
                    array.Remove(array[index]);
                }

                var result = await DalFightGroupsPacketsLog.InsertFightGroupsPacket(list);
                using (var client = CacheHelper.CreateCacheClient(FightGroupPacketsListRedisKey))
                {
                    await client.SetAsync<List<FightGroupsPacketsLogModel>>(fightGroupsIdentity.ToString(), list, TimeSpan.FromDays(1));
                    await client.SetAsync<List<FightGroupsPacketsLogModel>>(list?.Where(_ => _.IsLeader == true)?.FirstOrDefault()?.UserId?.ToString(), list,TimeSpan.FromDays(1));
                }

                Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse responseModel = new FightGroupPacketListResponse()
                {
                    EndDateTask = ConvertDateTimeLong(list?.FirstOrDefault()?.CreateDateTime.AddDays(1)),
                    Items =list,
                    ProvideCount = list?.Where(_ => _.UserId != null)?.Count()
                };
                return responseModel;
            }
            catch (Exception em)
            {
                Logger.ErrorException("创建分享红包组失败", em);
                return null;
            }
        }

        /// <summary>
        /// 更新分享红包组中的用户
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> UpdateFightGroupsPacketByUserId(FightGroupsPacketsUpdateRequest request)
        {
            //if (request.PKID <= 0)
            //    return OperationResult.FromError<bool>("0","参数错误");

            using (var zookeeperClient = new Tuhu.ZooKeeper.ZooKeeperLock(SecurityHelper.Hash(request.FightGroupsIdentity.ToString())))
            {

                try
                {
                    if (await zookeeperClient.WaitAsync(3000))
                    {
                        List<FightGroupsPacketsLogModel> result = new List<FightGroupsPacketsLogModel>();
                        using (var client = CacheHelper.CreateCacheClient(FightGroupPacketsListRedisKey))
                        {
                            var response = await client.GetOrSetAsync<List<FightGroupsPacketsLogModel>>(request.FightGroupsIdentity.ToString(), async () => await DalFightGroupsPacketsLog.GetFightGroupsPacketsList(request.FightGroupsIdentity), TimeSpan.FromDays(1));
                            if (response?.Value == null)
                            {
                                client.Remove(request.FightGroupsIdentity.ToString());
                                response = await client.GetOrSetAsync<List<FightGroupsPacketsLogModel>>(request.FightGroupsIdentity.ToString(), async () => await DalFightGroupsPacketsLog.GetFightGroupsPacketsList(request.FightGroupsIdentity), TimeSpan.FromDays(1));
                            }
                            result = response?.Value;
                            if (result?.Count(x => x.UserId == null || x.UserId.Value == Guid.Empty) == 0)
                                return OperationResult.FromError<bool>("-1", "该红包组已领取完毕");
                            if (result.Where(_ => _.UserId == request.UserId)?.Count() > 0)
                                return OperationResult.FromError<bool>("-2", "该用户已经领取");


                        }
                        var groupLeader = result?.Where(_ => _.IsLeader == true)?.FirstOrDefault()?.UserId.ToString();
                        using (var client =
                            CacheHelper.CreateCounterClient(
                                "UpdateFightGroupsPacketByUserId" + groupLeader + request.UserId, TimeSpan.FromDays(7)))
                        {
                            var response = await client.IncrementAsync("UpdateFightGroupsPacketByUserId", 1);
                            if (response.Success && response.Value > 1)
                            {
                                return OperationResult.FromError<bool>("-3", "7天内无法为同一好友拆红包哦");
                            }
                        }

                        using (var client = CacheHelper.CreateCacheClient(FightGroupPacketsListRedisKey))
                        {
                            var resPkid =
                                await DalFightGroupsPacketsLog.UpdateFightGroupsPacketByUserId(
                                    request.FightGroupsIdentity, request.UserId);
                            if (resPkid > 0)
                            {
                                var userItem = result?.Where(_ => _.PKID == resPkid)?.FirstOrDefault();
                                if (userItem != null)
                                    userItem.UserId = request.UserId;
                                await client.SetAsync<List<FightGroupsPacketsLogModel>>(
                                    request.FightGroupsIdentity.ToString(), result);
                                await client.SetAsync<List<FightGroupsPacketsLogModel>>(
                                    result?.Where(_ => _.IsLeader == true)?.FirstOrDefault()?.UserId.ToString(),
                                    result);
                                return OperationResult.FromResult(true);
                            }
                        }
                    }
                    return OperationResult.FromError<bool>("-3", "更新失败");
                }
                catch (Exception em)
                {
                    Logger.ErrorException("更新分享红包组中的用户失败", em);
                    return OperationResult.FromError<bool>("500", em.Message);
                }
                finally
                {
                    zookeeperClient.Release();
                }
            }
        }

        /// <summary>
        /// 发送优惠券
        /// </summary>
        /// <param name="fightGroupsIdentity"></param>
        /// <returns></returns>
        public static async Task<FightGroupsPacketsProvideResponse> CreateFightGroupsPacketByPromotion(Guid fightGroupsIdentity)
        {
            using (var client = CacheHelper.CreateCacheClient(FightGroupPacketsListRedisKey))
            {
                List<FightGroupsPacketsLogModel> list = null;
                var result = await client.GetOrSetAsync<List<FightGroupsPacketsLogModel>>(fightGroupsIdentity.ToString(), async () => await DalFightGroupsPacketsLog.GetFightGroupsPacketsList(fightGroupsIdentity), TimeSpan.FromDays(1));
                list = result?.Value;
                if (list == null)
                {
                    list = await DalFightGroupsPacketsLog.GetFightGroupsPacketsList(fightGroupsIdentity);
                }
                if (list?.Where(_ => _.UserId == null)?.Count() <= 0 && list?.Where(_=>_.PromotionPKID != null)?.Count()<=0)
                {

                    using (var memberClient = new Tuhu.Service.Member.PromotionClient())
                    {
                        List<CreatePromotionModel> promotionModelList = new List<CreatePromotionModel>();
                        foreach (var item in list)
                        {
                            promotionModelList.Add(new CreatePromotionModel()
                            {
                                Author = item.UserId.ToString(),
                                GetRuleGUID = item.GetRuleGuid,
                                UserID = item.UserId,
                                Channel = "小程序分享红包",
                                DeviceID = "",
                                Operation = "小程序分享红包",
                                IssueChannle = "小程序分享红包",
                                IssueChannleId = fightGroupsIdentity.ToString(),
                                Issuer = "李维童"
                            });

                        }
                        var memberResult = await memberClient.CreatePromotionsNewAsync(promotionModelList);
                        if (memberResult.Result.IsSuccess)
                        {
                            for (int i = 0; i < list?.Count; i++)
                            {
                                if (memberResult?.Result?.promotionIds?.Count > i)
                                {
                                    list[i].PromotionPKID = memberResult?.Result?.promotionIds[i];
                                }
                            }
                            await DalFightGroupsPacketsLog.UpdateCreatePromotionPKID(list);
                            // 先注释掉，因为读写库原因，删掉之后，下次从数据库读取不到
                            //using (var reditClient = CacheHelper.CreateCacheClient(FightGroupPacketsListRedisKey))
                            //{
                            //    reditClient.Remove(list?.Where(_ => _.IsLeader == true)?.FirstOrDefault()?.UserId.ToString());
                            //    reditClient.Remove(fightGroupsIdentity.ToString());
                            //}


                            using (var pushClient = new Tuhu.Service.Push.TemplatePushClient())
                            {
                                foreach (var item in list)
                                {
                                    using (var userClient = new Tuhu.Service.UserAccount.UserAccountClient())
                                    {
                                        var userResult = await userClient.GetUserByIdAsync(item.UserId.Value);
                                        var pushResult = await pushClient.PushByUserIDAndBatchIDAsync(new List<string>() { item.UserId.Value.ToString() }, 1643, new Service.Push.Models.Push.PushTemplateLog()
                                        {
                                            Replacement = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>() {
                                             {"{{nickname}}", userResult.Result.Profile.NickName},
                                             { "{{packetgroupno}}", item.FightGroupsIdentity.ToString()}})
                                        });
                                        if (!pushResult.Success || !pushResult.Result)
                                            //Logger.ErrorException("拆红包推送失败", pushResult.Exception);
                                            Logger.Info($"拆红包推送失败Success={pushResult.Success};Result={pushResult.Result}");
                                    }
                                }
                            }

                            return new FightGroupsPacketsProvideResponse() { IsSuccess = memberResult.Result.IsSuccess, Msg = "优惠券领取成功" };
                        }
                        else
                        {
                            return new FightGroupsPacketsProvideResponse() { IsSuccess = memberResult.Result.IsSuccess, Msg = memberResult?.Result?.ErrorMessage };
                        }
                    }
                }
                else
                    return new FightGroupsPacketsProvideResponse() { IsSuccess = false, Msg = string.Format("分享红包组没有拼组完成还差{0}个或者已经发放过", list?.Where(_ => _.UserId == null)?.Count()) };
                
            }
        }


        private static long ConvertDateTimeLong(System.DateTime? time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime)?.TotalSeconds;//TotalSeconds
        }
    }
}
