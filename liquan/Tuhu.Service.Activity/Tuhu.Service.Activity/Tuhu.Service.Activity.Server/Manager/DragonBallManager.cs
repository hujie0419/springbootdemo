using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.DataAccess.DragonBall;
using Tuhu.Service.Activity.DataAccess.Models.Activity;
using Tuhu.Service.Activity.DataAccess.Models.DragonBall;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Manager.DragonBallInternal;
using Tuhu.Service.Activity.Server.Model.DragonBall;
using Tuhu.Service.Member;
using Tuhu.Service.UserAccount;
using Tuhu.ZooKeeper;

namespace Tuhu.Service.Activity.Server.Manager
{
    /// <summary>
    ///     七龙珠 业务类
    /// </summary>
    public class DragonBallManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DragonBallManager));

        #region 七龙珠 - 更新活动 / 设置

        /// <summary>
        ///     七龙珠 - 更新活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> DragonBallActivityUpdateAsync(DragonBallActivityUpdateRequest request)
        {
            try
            {
                var dbResult = await DalActivity.GetActivityByTypeId(3);

                using (var dbHelper = DbHelper.CreateDbHelper())
                {

                    dbResult.StartTime = request.StartTime;
                    dbResult.EndTime = request.EndTime;

                    await DalActivity.UpdateActivtyAsync(dbHelper, dbResult);


                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"DragonBallManager -> DragonBallActivityUpdateAsync -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     七龙珠 - 更新设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> DragonBallSettingUpdateAsync(DragonBallSettingUpdateRequest request)
        {
            try
            {
                var dbResult = await DragonBallSettingModelAsync();

                dbResult.Pids = request.Pids;
                dbResult.MissionBigBrand = request.MissionBigBrand;
                dbResult.SummonBigBrand = request.SummonBigBrand;

                await DalDragonBallSetting.UpdateDragonBallSettingAsync(dbResult);

                return true;

            }
            catch (Exception e)
            {
                Logger.Error(
                    $"DragonBallManager -> DragonBallSettingUpdateAsync -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 七龙珠 - 更新和删除用户数据

        /// <summary>
        ///     七龙珠 - 删除用户数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<bool> DragonBallUserDataDeleteAsync(Guid userId)
        {
            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    await DalDragonBallUserInfo.DeleteDragonBallUserInfoModelAsync(dbHelper, userId);
                    await DalDragonBallUserLoot.DeleteDragonBallUserLootAsync(dbHelper, userId);
                    await DalDragonBallUserMission.DeleteDragonBallUserMissionAsync(dbHelper, userId);
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"DragonBallManager -> DragonBallUserDataDeleteAsync -> {userId}",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     七龙珠 - 增加修改用户龙珠数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dragonBallCount"></param>
        /// <returns></returns>
        public static async Task<bool> DragonBallUserUpdateAsync(Guid userId,int dragonBallCount)
        {
            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    var dbResult = await DalDragonBallUserInfo.SearchDragonBallUserInfoModelAsync(false, userId);

                    dbResult.DragonBallCount = dragonBallCount;

                    await DalDragonBallUserInfo.UpdateDragonBallUserInfoModelUserMissionAsync(dbHelper,
                        dbResult);

                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"DragonBallManager -> DragonBallUserUpdateAsync -> {userId}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion


        #region 七龙珠设置表 - 大翻盘相关

        /// <summary>
        ///     获取七龙珠设置表
        /// </summary>
        /// <returns></returns>
        private static async Task<DragonBallSettingModel> DragonBallSettingModelAsync()
        {
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.DragonBallCacheHeader))
            {
                var result = await cacheClient.GetOrSetAsync($"{nameof(DragonBallSettingModelAsync)}"
                    , async () => await DalDragonBallSetting.GetDragonBallSettingAsync(), TimeSpan.FromMinutes(5));

                // 返回数据
                return result?.Value;
            }
        }

        #endregion

        #region 用户任务列表

        /// <summary>
        ///     七龙珠 - 用户任务列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<DragonBallUserMissionListResponse>> DragonBallUserMissionListAsync(
            DragonBallUserMissionListRequest request)
        {
            try
            {
                await Task.Yield();

                var userId = request.UserId;


                var flagResult = await DragonBallUserMissionInitAsync(userId);

                // 获取用户全部的任务明细
                var userMissionList =
                    await DalDragonBallUserMission.SearchDragonBallUserMissionByUserIdAsync(!flagResult.Result, userId);

                // 获取任务列表
                var missionList = DragonBallMissionFactory.GetAllMissions();

                // 转换成任务对象
                var dragonBallUserMissionBussinessModels = missionList.Select(mission =>
                {
                    var missionId = (int)mission.MissionEnumModel;
                    var userMissionIdList = userMissionList.Where(p => p.MissionId == missionId).ToList();
                    return DragonBallUserMissionBussinessFactory.Create(mission, userMissionIdList);
                }).ToList();



                // 返回对象
                var returnValue = new DragonBallUserMissionListResponse
                {
                    Items = dragonBallUserMissionBussinessModels.Select(p => new DragonBallUserMissionListResponseItem
                    {
                        DragonBallCount = p.DragonBallCount,
                        MissionId = p.MissionId,
                        MissionUserStatus = p.MissionUserStatus
                    }).ToList()
                };

                return OperationResult.FromResult(returnValue);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"DragonBallManager -> DragonBallUserMissionListAsync -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 用户领取任务奖励

        /// <summary>
        ///     七龙珠 - 用户获取任务奖励
        ///     返回值
        ///             -10  参数错误
        ///             -20  时间不对
        ///             -30  请重试
        ///             -40  系统异常
        ///             -50  不能领取
        ///             -60  系统异常
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<DragonBallUserMissionRewardResponse>> DragonBallUserMissionRewardAsync(
            DragonBallUserMissionRewardRequest request)
        {
            await Task.Yield();

            var now = DateTime.Now;
            // 校验参数
            if (request == null || request.UserId == Guid.Empty || request.MissionId == 0)
            {
                return OperationResult.FromError<DragonBallUserMissionRewardResponse>("-10", "");
            }

            try
            {
                // 获取必要的配置类
                var activityTask = DragonBallActivityInfoAsync();
                var dragonBallSettingModelTask = DragonBallSettingModelAsync();

                await Task.WhenAll(activityTask, dragonBallSettingModelTask);

                // 活动信息
                var activityInfo = activityTask.Result?.Result;
                // 七龙珠设置
                var dragonBallSettingModel = dragonBallSettingModelTask.Result;

                // 判断参数
                if (activityInfo == null || dragonBallSettingModel == null ||
                    string.IsNullOrWhiteSpace(dragonBallSettingModel.MissionBigBrand) ||
                    string.IsNullOrWhiteSpace(dragonBallSettingModel.SummonBigBrand))
                {
                    return OperationResult.FromError<DragonBallUserMissionRewardResponse>("-10", "");
                }

                if (activityInfo.StartTime > now || activityInfo.EndTime < now)
                {
                    return OperationResult.FromError<DragonBallUserMissionRewardResponse>("-20", "");
                }

                var missionId = request.MissionId;
                var userId = request.UserId;
                // 开启用户锁防止并发
                using (var zkLock = new ZooKeeperLock($"/{GlobalConstant.DragonBallUserLockHeader}/{userId:N}"))
                {
                    if (!await zkLock.WaitAsync(20000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                    {
                        return OperationResult.FromError<DragonBallUserMissionRewardResponse>("-30", "");
                    }

                    // 获取对应的任务
                    var mission = DragonBallMissionFactory.GetByMissionId(missionId);

                    if (mission == null)
                    {
                        // 任务ID 异常
                        return OperationResult.FromError<DragonBallUserMissionRewardResponse>("-40", "");
                    }

                    // 获取用户任务详情
                    var userMissionList =
                        await DalDragonBallUserMission.SearchDragonBallUserMissionByUserIdAsync(false, userId);


                    // 获取用户信息
                    var dragonBallUserInfo =
                        await DalDragonBallUserInfo.SearchDragonBallUserInfoModelAsync(false, userId);

                    // 用户可领取奖励的任务
                    var userTargetMissionList =
                        userMissionList.Where(p => p.MissionId == missionId)
                            .ToList();

                    if (!DragonBallUserMissionBussinessFactory.Create(mission, userTargetMissionList).CanRecive())
                    {
                        return OperationResult.FromError<DragonBallUserMissionRewardResponse>("-50", "");
                    }

                    var userMission = userTargetMissionList.FirstOrDefault(p => p.MissionStatus == 1);

                    if (userMission == null)
                    {
                        return OperationResult.FromError<DragonBallUserMissionRewardResponse>("-50", "");
                    }

                    // 设置领取调用大翻盘
                    using (var bigBrandClient = new BigBrandClient())
                    // 数据库
                    using (var sqlClient = DbHelper.CreateDbHelper())
                    {
                        try
                        {
                            sqlClient.BeginTransaction();
                            var flagUserInfo = false;
                            var flagUserMission = false;
                            var flagBigBrand = false;
                            var flagLoot = false;

                            // 用户获取礼物model
                            var dragonBallUserLootModel = new DragonBallUserLootModel();

                            #region 更新用户信息

                            if (dragonBallUserInfo == null)
                            {
                                dragonBallUserInfo = new DragonBallUserInfoModel()
                                {
                                    DragonBallCount = mission.MissionDragonBall,
                                    FinishMissionCount = 1,
                                    UserId = userId
                                };


                                // 新增 用户信息
                                flagUserInfo = await DalDragonBallUserInfo.InsertDragonBallUserInfoModelAsync(sqlClient,
                                                   dragonBallUserInfo) > 0;
                            }
                            else
                            {
                                dragonBallUserInfo.FinishMissionCount++;
                                dragonBallUserInfo.DragonBallCount =
                                    dragonBallUserInfo.DragonBallCount + mission.MissionDragonBall;
                                // 修改 用户信息
                                flagUserInfo =
                                    await DalDragonBallUserInfo.UpdateDragonBallUserInfoModelUserMissionAsync(sqlClient,
                                        dragonBallUserInfo);
                            }

                            #endregion

                            #region 更新用户任务

                            userMission.MissionStatus = 2;
                            userMission.DragonBallCount = mission.MissionDragonBall;

                            flagUserMission =
                                await DalDragonBallUserMission.UpdateDragonBallUserMissionAsync(sqlClient, userMission);

                            #endregion

                            if (!flagUserMission || !flagUserInfo)
                            {
                                sqlClient.Rollback();
                                return OperationResult.FromError<DragonBallUserMissionRewardResponse>("-60", "");
                            }

                            #region 大翻盘

                            var bigBrandResult = await bigBrandClient.GetPacketAsync(userId, "", "dragonball",
                                dragonBallSettingModel.MissionBigBrand,
                                "",
                                $"{nameof(DragonBallManager)}/{nameof(DragonBallUserMissionRewardAsync)}/{missionId}",
                                "");

                            bigBrandResult.ThrowIfException();

                            if (bigBrandResult.Success && bigBrandResult?.Result?.Code == 1 &&
                                bigBrandResult.Result != null)
                            {
                                var bigBrandFirstLoot = bigBrandResult.Result.CouponRules?.FirstOrDefault();
                                if (bigBrandFirstLoot != null)
                                {
                                    // 大翻盘成功
                                    flagBigBrand = true;
                                    dragonBallUserLootModel.UserId = userId;
                                    // 优惠券名称
                                    dragonBallUserLootModel.LootName = bigBrandFirstLoot.PromotionName;

                                    if (bigBrandFirstLoot.ValiStartDate == null)
                                    {
                                        dragonBallUserLootModel.LootStartTime = DateTime.Today;
                                        var end = DateTime.Today.AddDays(bigBrandFirstLoot.Term ?? 0);
                                        if (end < bigBrandFirstLoot.ValiEndDate)
                                        {
                                            end = bigBrandFirstLoot.ValiEndDate.Value;
                                        }

                                        dragonBallUserLootModel.LootEndTime = end;
                                    }
                                    else
                                    {
                                        dragonBallUserLootModel.LootStartTime = bigBrandFirstLoot.ValiStartDate;
                                        dragonBallUserLootModel.LootEndTime = bigBrandFirstLoot.ValiEndDate;
                                    }

                                    dragonBallUserLootModel.LootPicUrl = bigBrandResult.Result.PromptImg;
                                    dragonBallUserLootModel.LootType = 1;
                                    dragonBallUserLootModel.LootDesc = bigBrandFirstLoot.Description;

                                    dragonBallUserLootModel.LootTitile = bigBrandResult.Result.PromptMsg;
                                    dragonBallUserLootModel.LootMemo = "";
                                }
                            }
                            else
                            {
                                // 大翻盘失败
                                Logger.Warn(
                                    $"DragonBallManager -> DragonBallUserMissionRewardAsync -> bigBrandClient -> GetPacketAsync -> {JsonConvert.SerializeObject(request)} {bigBrandResult?.Result?.Code} {bigBrandResult?.ErrorCode} {bigBrandResult?.ErrorMessage} {bigBrandResult?.Result?.Msg}");
                                flagBigBrand = false;
                            }

                            #endregion

                            #region 用户战利品

                            if (flagBigBrand)
                            {
                                flagLoot = await DalDragonBallUserLoot.InsertDragonBallUserLootAsync(sqlClient,
                                               dragonBallUserLootModel) > 0;
                            }

                            #endregion

                            if (!flagUserInfo || !flagUserMission || !flagBigBrand || !flagLoot)
                            {
                                Logger.Warn(
                                    $"DragonBallManager -> DragonBallUserMissionRewardAsync -> {flagUserInfo} {flagUserMission} {flagBigBrand} {flagLoot}  -> {JsonConvert.SerializeObject(request)}");
                                sqlClient.Rollback();
                                return OperationResult.FromError<DragonBallUserMissionRewardResponse>("-60", "");
                            }

                            sqlClient.Commit();

                            await DragonBallUserInfoCacheSetAsync(userId, new DragonBallUserInfoResponse()
                            {
                                DragonBallCount = dragonBallUserInfo.DragonBallCount,
                                DragonBallSummonCount = dragonBallUserInfo.DragonBallSummonCount
                            });

                            return OperationResult.FromResult(new DragonBallUserMissionRewardResponse
                            {
                                DragonBallCount = mission.MissionDragonBall,
                                LootDesc = dragonBallUserLootModel.LootDesc,
                                LootEndTime = dragonBallUserLootModel.LootEndTime,
                                LootName = dragonBallUserLootModel.LootName,
                                LootPicUrl = dragonBallUserLootModel.LootPicUrl,
                                LootStartTime = dragonBallUserLootModel.LootStartTime,
                                LootTitile = dragonBallUserLootModel.LootTitile,
                                LootMemo = dragonBallUserLootModel.LootMemo
                            });
                        }
                        catch (Exception e)
                        {
                            sqlClient?.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"DragonBallManager -> DragonBallUserMissionRewardAsync -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 召唤神龙

        /// <summary>
        ///     召唤神龙
        ///     返回值
        ///             -10  参数错误
        ///             -20  请重试
        ///             -25  配置异常
        ///             -30  操作异常，当前龙珠数不足
        ///             -40  系统异常
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<DragonBallSummonResponse>> DragonBallSummonAsync(
            DragonBallSummonRequest request)
        {
            try
            {
                await Task.Yield();

                // 校验参数
                if (request == null || request.UserId == Guid.Empty)
                {
                    return OperationResult.FromError<DragonBallSummonResponse>("-10", "");
                }

                var userId = request.UserId;

                // 开启用户锁防止并发
                using (var zkLock = new ZooKeeperLock($"/{GlobalConstant.DragonBallUserLockHeader}/{userId:N}"))
                {
                    if (!await zkLock.WaitAsync(20000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                    {
                        return OperationResult.FromError<DragonBallSummonResponse>("-20", "");
                    }

                    // 配置
                    var dragonBallSettingModel = await DragonBallSettingModelAsync();

                    if (dragonBallSettingModel == null)
                    {
                        return OperationResult.FromError<DragonBallSummonResponse>("-25", "");
                    }

                    // 获取用户信息
                    var dragonBallUserInfo =
                        await DalDragonBallUserInfo.SearchDragonBallUserInfoModelAsync(false, userId);

                    // 如果当前用户的数据小于7颗龙珠，那么：
                    if (dragonBallUserInfo == null ||
                        dragonBallUserInfo?.DragonBallCount < GlobalConstant.DragonBallCountDefine)
                    {
                        return OperationResult.FromError<DragonBallSummonResponse>("-30", "");
                    }

                    // 设置领取调用大翻盘
                    using (var bigBrandClient = new BigBrandClient())
                    // 数据库
                    using (var sqlClient = DbHelper.CreateDbHelper())
                    {
                        try
                        {
                            var flagBigBrand = false;
                            var flagLoot = true;
                            // 用户获取礼物model
                            var dragonBallUserLootModel = new DragonBallUserLootModel();
                            // 事务
                            sqlClient.BeginTransaction();

                            dragonBallUserInfo.DragonBallCount =
                                dragonBallUserInfo.DragonBallCount - GlobalConstant.DragonBallCountDefine;
                            dragonBallUserInfo.DragonBallSummonCount++;

                            // 更新用户数据
                            var updateUserInfoResult =
                                await DalDragonBallUserInfo.UpdateDragonBallUserInfoModelAsync(sqlClient,
                                    dragonBallUserInfo);

                            if (!updateUserInfoResult)
                            {
                                sqlClient.Rollback();
                                return OperationResult.FromError<DragonBallSummonResponse>("-40", "");
                            }

                            // 大翻盘
                            var bigBrandResult = await bigBrandClient.GetPacketAsync(userId, "", "dragonball",
                                dragonBallSettingModel.SummonBigBrand,
                                "",
                                $"{nameof(DragonBallManager)}/{nameof(DragonBallSummonAsync)}", "");

                            bigBrandResult.ThrowIfException();

                            if (bigBrandResult.Success && bigBrandResult?.Result?.Code == 1 &&
                                bigBrandResult.Result != null)
                            {
                                var bigBrandFirstLoot = bigBrandResult.Result.CouponRules?.FirstOrDefault();
                                if (bigBrandFirstLoot != null)
                                {
                                    flagBigBrand = true;
                                    // 大翻盘成功
                                    dragonBallUserLootModel.UserId = userId;
                                    // 优惠券名称
                                    dragonBallUserLootModel.LootName = bigBrandFirstLoot.PromotionName;

                                    if (bigBrandFirstLoot.ValiStartDate == null)
                                    {
                                        dragonBallUserLootModel.LootStartTime = DateTime.Today;
                                        var end = DateTime.Today.AddDays(bigBrandFirstLoot.Term ?? 0);
                                        if (end < bigBrandFirstLoot.ValiEndDate)
                                        {
                                            end = bigBrandFirstLoot.ValiEndDate.Value;
                                        }

                                        dragonBallUserLootModel.LootEndTime = end;
                                    }
                                    else
                                    {
                                        dragonBallUserLootModel.LootStartTime = bigBrandFirstLoot.ValiStartDate;
                                        dragonBallUserLootModel.LootEndTime = bigBrandFirstLoot.ValiEndDate;
                                    }


                                    dragonBallUserLootModel.LootPicUrl = bigBrandResult.Result.PromptImg;
                                    dragonBallUserLootModel.LootType = 2;
                                    dragonBallUserLootModel.LootDesc = bigBrandFirstLoot.Description;

                                    dragonBallUserLootModel.LootTitile = bigBrandResult.Result.PromptMsg;
                                    dragonBallUserLootModel.LootMemo = "";
                                }
                            }
                            else
                            {
                                // 大翻盘失败
                                Logger.Warn(
                                    $"DragonBallManager -> DragonBallSummonAsync -> bigBrandClient -> GetPacketAsync -> {JsonConvert.SerializeObject(request)}");
                            }

                            if (flagBigBrand)
                            {
                                flagLoot = await DalDragonBallUserLoot.InsertDragonBallUserLootAsync(sqlClient,
                                               dragonBallUserLootModel) > 0;
                            }

                            if (!flagBigBrand || !flagLoot)
                            {
                                sqlClient.Rollback();
                                return OperationResult.FromError<DragonBallSummonResponse>("-40", "");
                            }

                            sqlClient.Commit();
                            await DragonBallUserInfoCacheSetAsync(userId, new DragonBallUserInfoResponse()
                            {
                                DragonBallCount = dragonBallUserInfo.DragonBallCount,
                                DragonBallSummonCount = dragonBallUserInfo.DragonBallSummonCount
                            });
                            return OperationResult.FromResult(new DragonBallSummonResponse
                            {
                                LootDesc = dragonBallUserLootModel.LootDesc,
                                LootEndTime = dragonBallUserLootModel.LootEndTime,
                                LootName = dragonBallUserLootModel.LootName,
                                LootPicUrl = dragonBallUserLootModel.LootPicUrl,
                                LootStartTime = dragonBallUserLootModel.LootStartTime,
                                LootTitile = dragonBallUserLootModel.LootTitile
                            });
                        }
                        catch (Exception e)
                        {
                            sqlClient?.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallSummonAsync -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 用户当前龙珠总数/兑换次数

        /// <summary>
        ///     缓存用户信息
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> DragonBallUserInfoCacheSetAsync(Guid userId, DragonBallUserInfoResponse data)
        {

            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.DragonBallCacheHeader))
            {
                var iResult = await cacheClient.SetAsync($"{nameof(DragonBallUserInfoCacheSetAsync)}:{userId:N}", data, TimeSpan.FromSeconds(5));
                return iResult.Success;
            }
        }

        /// <summary>
        ///     缓存用户信息
        /// </summary>
        /// <returns></returns>
        private static async Task<DragonBallUserInfoResponse> DragonBallUserInfoCacheGetAsync(Guid userId)
        {

            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.DragonBallCacheHeader))
            {
                var iResult = await cacheClient.GetAsync<DragonBallUserInfoResponse>($"{nameof(DragonBallUserInfoCacheSetAsync)}:{userId:N}");
                return iResult.Value;
            }
        }

        /// <summary>
        ///     用户当前龙珠总数/兑换次数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<DragonBallUserInfoResponse>> DragonBallUserInfoAsync(
            DragonBallUserInfoRequest request)
        {
            try
            {
                var userId = request.UserId;

                var cacheResult = await DragonBallUserInfoCacheGetAsync(userId);

                if (cacheResult != null)
                {
                    return OperationResult.FromResult(cacheResult);
                }

                // 获取用户信息
                var dragonBallUserInfo =
                    await DalDragonBallUserInfo.SearchDragonBallUserInfoModelAsync(true, userId);

                var result = new DragonBallUserInfoResponse
                {
                    DragonBallCount = dragonBallUserInfo?.DragonBallCount ?? 0,
                    DragonBallSummonCount = dragonBallUserInfo?.DragonBallSummonCount ?? 0
                };
                return OperationResult.FromResult(result);
            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallUserInfoAsync -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 获取七龙珠活动数据

        /// <summary>
        ///     七龙珠 - 活动信息
        /// </summary>
        /// <returns></returns>
        private static async Task<ActivityResponse> ActivityInfoAsync()
        {
            var dbResult = await DalActivity.GetActivityByTypeId(3);

            return ObjectMapper.ConvertTo<ActivityModel, ActivityResponse>(dbResult);
        }

        /// <summary>
        ///     七龙珠 - 活动信息
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<ActivityResponse>> DragonBallActivityInfoAsync()
        {
            try
            {
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.DragonBallCacheHeader))
                {
                    var result = await cacheClient.GetOrSetAsync($"{nameof(DragonBallActivityInfoAsync)}"
                        , async () => OperationResult.FromResult(await ActivityInfoAsync())
                        , TimeSpan.FromMinutes(5));

                    // 返回数据
                    return result?.Value;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallActivityInfoAsync", e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     七龙珠 - 活动信息 无缓存
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<ActivityResponse>> DragonBallActivityInfoNoCacheAsync()
        {
            try
            {
                return OperationResult.FromResult(await ActivityInfoAsync());
            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallActivityInfoNoCacheAsync", e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 七龙珠 - 获取设置

        /// <summary>
        ///     七龙珠 - 获取设置
        /// </summary>
        /// <returns></returns>
        private static async Task<DragonBallSettingResponse> GetSettingAsync()
        {
            // 配置
            var dragonBallSettingModel = await DragonBallSettingModelAsync();

            return new DragonBallSettingResponse()
            {
                Pids = dragonBallSettingModel.Pids?.Split(',').ToList()
            };
        }

        /// <summary>
        ///     七龙珠 - 获取设置
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<DragonBallSettingResponse>> DragonBallSettingAsync()
        {
            try
            {
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.DragonBallCacheHeader))
                {
                    var result = await cacheClient.GetOrSetAsync($"{nameof(DragonBallSettingAsync)}"
                        , async () => OperationResult.FromResult(await GetSettingAsync())
                        , TimeSpan.FromMinutes(5));

                    // 返回数据
                    return result?.Value;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallSettingAsync", e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 七龙珠 - 获奖轮播

        /// <summary>
        ///     数据 脱敏
        /// </summary>
        private static string NickNameDataMasking(string nickName)
        {
            nickName = nickName?.Trim() ?? "";
            //加密 如果11位 那么隐藏中间 四位
            if (nickName.Length == 11)
            {
                nickName = nickName.Substring(0, 3) + "****" + nickName.Substring(7, 4);
            }

            //写死 临时需求 不想在排行榜显示这些词汇
            var keyNames = new[] { "京东", "淘宝", "测试", "途虎" };
            keyNames.ForEach(name => { nickName = nickName.Replace(name, "*"); });
            return nickName;
        }

        /// <summary>
        ///     七龙珠 - 获奖轮播
        /// </summary>
        /// <param name="count"></param>
        /// <param name="lootType"></param>
        /// <returns></returns>
        public static async Task<OperationResult<DragonBallBroadcastResponse>> DragonBallBroadcastAsync(int count,
            int lootType = 2)
        {
            await Task.Yield();
            if (count > 100)
            {
                return OperationResult.FromError<DragonBallBroadcastResponse>("-2", "");
            }

            try
            {
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.DragonBallCacheHeader))
                {
                    var cacheResult = await cacheClient.GetOrSetAsync($":{nameof(DragonBallBroadcastAsync)}:{count}", async () =>
                    {
                        // 获取数据
                        var dragonBallUserLootModels = await DalDragonBallUserLoot.SearchDragonBallUserLootAsync(count, lootType);
                        using (var userClient = new UserClient())
                        {
                            var dragonBallBroadcastResponseItems = dragonBallUserLootModels.Select(p =>
                            {
                                var userInfo = userClient.FetchUserByUserId(p.UserId.ToString());
                                return new DragonBallBroadcastResponseItem
                                {
                                    LootName = p.LootName,
                                    NickName = NickNameDataMasking(userInfo?.Result?.Nickname)
                                };
                            }).ToList();

                            var returnValue = new DragonBallBroadcastResponse
                            {
                                Items = dragonBallBroadcastResponseItems
                            };

                            return OperationResult.FromResult(returnValue);
                        }
                    }, TimeSpan.FromMinutes(5));

                    return cacheResult?.Value;
                }

            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallBroadcastAsync", e.InnerException ?? e);
                throw;
            }

        }


        #endregion

        #region 七龙珠 - 用户获得战利品列表

        /// <summary>
        ///     七龙珠 - 用户获得战利品列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<DragonBallUserLootListResponse>> DragonBallUserLootListAsync(DragonBallUserLootListRequest request)
        {
            try
            {
                // 获取用户领取的列表
                var lootList = (await DalDragonBallUserLoot.SearchDragonBallUserLootByUserIdAsync(true, request.UserId))
                    .OrderByDescending(p => p.PKID)
                    .ToList();

                var returnValue = new DragonBallUserLootListResponse()
                {
                    CouponItems = lootList.Where(p => p.LootType == 1).Select(p => new DragonBallUserLootListResponseItem()
                    {
                        LootDesc = p.LootDesc,
                        LootEndTime = p.LootEndTime,
                        LootName = p.LootName,
                        LootPicUrl = p.LootPicUrl,
                        LootStartTime = p.LootStartTime
                    }).ToList(),
                    FreeItems = lootList.Where(p => p.LootType == 2).Select(p => new DragonBallUserLootListResponseItem()
                    {
                        LootDesc = p.LootDesc,
                        LootEndTime = p.LootEndTime,
                        LootName = p.LootName,
                        LootPicUrl = p.LootPicUrl,
                        LootStartTime = p.LootStartTime
                    }).ToList()

                };
                return OperationResult.FromResult(returnValue);
            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallUserLootListAsync", e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 七龙珠 - 用户分享

        /// <summary>
        ///     七龙珠 - 用户分享
        ///     返回值
        ///             -5  活动尚未开始
        ///             -10 请重试
        ///             -20 今天已经分享
        ///             -25 不能再分享了
        ///             -30 系统异常
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> DragonBallUserShareAsync(DragonBallUserShareRequest request)
        {
            try
            {
                await Task.Yield();

                // 校验参数
                if (request == null || request.UserId == Guid.Empty)
                {
                    return OperationResult.FromError<bool>("-2", "");
                }

                var userId = request.UserId;

                // 任务信息
                var missionInfo = DragonBallMissionFactory.GetByMissionId((int)DragonBallMissionEnumModel.DailyShare);

                // 获取此活动
                var activityInfoResult = await DragonBallActivityInfoAsync();

                var now = DateTime.Now;
                if (activityInfoResult.Result?.StartTime > now || activityInfoResult.Result?.EndTime < now)
                {
                    return OperationResult.FromError<bool>("-5", "");
                }

                // 开启用户锁防止并发
                using (var zkLock = new ZooKeeperLock($"/{GlobalConstant.DragonBallUserLockHeader}/{userId:N}"))
                {
                    if (!await zkLock.WaitAsync(5000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                    {
                        return OperationResult.FromError<bool>("-10", "");
                    }

                    // 用户任务记录
                    var dragonBallUserMissionModels = await DalDragonBallUserMission.SearchDragonBallUserMissionByUserIdAsync(false, userId);

                    // 获取用户今天分享任务记录
                    if (dragonBallUserMissionModels.Any(p =>
                        p.MissionId == (int)DragonBallMissionEnumModel.DailyShare && p.CreateDatetime.Date == DateTime.Today))
                    {
                        return OperationResult.FromError<bool>("-20", "");
                    }

                    // 判断是否已经到了分享上限
                    if (dragonBallUserMissionModels.Count(p =>
                            p.MissionId == (int)DragonBallMissionEnumModel.DailyShare) >= missionInfo.IsCanFinishCount)
                    {
                        return OperationResult.FromError<bool>("-25", "");
                    }


                    using (var sqlClient = DbHelper.CreateDbHelper())
                    {
                        //开启事务
                        sqlClient.BeginTransaction();

                        var insertUserMissionResult = await DalDragonBallUserMission.InsertDragonBallUserMissionAsync(sqlClient,
                            new DragonBallUserMissionModel()
                            {
                                DragonBallCount = missionInfo.MissionDragonBall,
                                MissionId = (int)DragonBallMissionEnumModel.DailyShare,
                                MissionStatus = 1,
                                UserId = userId
                            });

                        if (insertUserMissionResult > 0)
                        {
                            sqlClient.Commit();
                            return OperationResult.FromResult(true);

                        }
                        else
                        {
                            sqlClient.Rollback();
                            return OperationResult.FromError<bool>("-30", "");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallUserShareAsync", e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 七龙珠 - 给用户创建一个任务

        /// <summary>
        ///     七龙珠 - 给用户创建一个任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> DragonBallCreateUserMissionDetailAsync(DragonBallCreateUserMissionDetailRequest request)
        {
            try
            {
                Logger.Info($"DragonBallManager -> DragonBallCreateUserMissionDetailAsync -> {JsonConvert.SerializeObject(request)} -> start ");

                await Task.Yield();
                var missionId = request.MissionId;
                var userId = request.UserId;

                if (!request.ForceActivityTime)
                {
                    // 获取活动
                    var activityInfo = await DragonBallActivityInfoAsync();

                    var now = DateTime.Now;

                    if (activityInfo.Result == null || activityInfo.Result.StartTime > now || activityInfo.Result.EndTime < now)
                    {
                        return OperationResult.FromError<bool>("-5", "");
                    }
                }


                // 开启用户锁防止并发
                using (var zkLock = new ZooKeeperLock($"/{nameof(DragonBallCreateUserMissionDetailAsync)}/{userId:N}"))
                {

                    if (!await zkLock.WaitAsync(5000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                    {
                        return OperationResult.FromError<bool>("-10", "");
                    }

                    // 任务信息
                    var missionInfo = DragonBallMissionFactory.GetByMissionId(missionId);

                    var dragonBallUserMissionModels = await DalDragonBallUserMission.SearchDragonBallUserMissionByUserIdAsync(false, userId);

                    if (missionInfo == null)
                    {
                        return OperationResult.FromError<bool>("-10", "");
                    }

                    if (dragonBallUserMissionModels.Count(p => p.MissionId == missionId) >=
                        missionInfo?.IsCanFinishCount)
                    {
                        return OperationResult.FromError<bool>("-20", "");
                    }

                    using (var sqlClient = DbHelper.CreateDbHelper())
                    {
                        //开启事务
                        sqlClient.BeginTransaction();

                        var insertUserMissionResult = await DalDragonBallUserMission.InsertDragonBallUserMissionAsync(sqlClient,
                            new DragonBallUserMissionModel()
                            {
                                DragonBallCount = missionInfo.MissionDragonBall,
                                MissionId = missionId,
                                MissionStatus = 1,
                                UserId = userId
                            });

                        if (insertUserMissionResult > 0)
                        {
                            sqlClient.Commit();
                            return OperationResult.FromResult(true);

                        }
                        else
                        {
                            sqlClient.Rollback();
                            return OperationResult.FromError<bool>("-30", "");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallCreateUserMissionDetailAsync", e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 七龙珠 - 用户任务历史

        /// <summary>
        ///     七龙珠 - 用户任务历史
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<DragonBallUserMissionHistoryListResponse>> DragonBallUserMissionHistoryListAsync(DragonBallUserMissionHistoryListRequest request)
        {
            try
            {
                var userId = request.UserId;

                var dragonBallUserMissionModels = await DalDragonBallUserMission.SearchDragonBallUserMissionByUserIdAsync(true, userId);

                var result = new DragonBallUserMissionHistoryListResponse();

                dragonBallUserMissionModels = dragonBallUserMissionModels.Where(p => p.MissionStatus == 2).ToList();

                result.Items = dragonBallUserMissionModels
                    .OrderByDescending(p => p.LastUpdateDateTime)
                    .Select(p => new DragonBallUserMissionHistoryListResponseItem()
                    {
                        Date = p.LastUpdateDateTime,
                        DragonBallCount = p.DragonBallCount,
                        MissionId = p.MissionId
                    })
                    .ToList();

                return OperationResult.FromResult(result);

            }
            catch (Exception e)
            {
                Logger.Error($"DragonBallManager -> DragonBallUserMissionHistoryListAsync", e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 七龙珠 - 初始化用户数据

        /// <summary>
        ///     七龙珠 - 初始化用户数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isForce"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> DragonBallUserMissionInitAsync(Guid userId, bool isForce = false)
        {
            await Task.Yield();
            var flag = false;
            using (var configClient = new Service.Config.ConfigClient())
            {
                flag = configClient.GetOrSetRuntimeSwitch("DragonBallUserMissionInitType")?.Result?.Value ?? false;
            }

            Logger.Info($"DragonBallManager -> DragonBallUserMissionInitAsync -> {userId} -> start ");
            // 关注公众号 ， 个人微信号 ， 车型认证 
            // 开启用户锁防止并发
            using (var cacheClient = CacheHelper.CreateCounterClient(GlobalConstant.DragonBallCacheHeader, TimeSpan.FromMinutes(3)))
            {
                if (flag)
                {
                    if (cacheClient.Increment($"DragonBallUserMissionInitTypeNew_1/{userId:N}").Value == 1 || isForce)
                    {
                        // 从会员任务中判断
                        using (var taskClient = new TaskClient())
                        {
                            var taskResult = await taskClient.GetUserTaskListAsync(userId);
                            taskResult?
                                .Result?
                                .Where(p => new[] { 1, 2 }.Contains(p.TaskStatus) &&
                                            new[] { "4BindWX", "5Follow", "7Authentication" }.Contains(p.TaskTag))
                                .Select(p =>
                                {
                                    if (p.TaskTag == "4BindWX")
                                    {
                                        return (int)DragonBallMissionEnumModel.BindPersonalWechat;
                                    }

                                    if (p.TaskTag == "5Follow")
                                    {
                                        return (int)DragonBallMissionEnumModel.FollowOfficalAccount;
                                    }
                                    else
                                    {
                                        return (int)DragonBallMissionEnumModel.FinishCarCertification;
                                    }
                                })
                                .Distinct()
                                .ToList()
                                .ForEach(p =>
                                {
                                    var t = DragonBallCreateUserMissionDetailAsync(
                                        new DragonBallCreateUserMissionDetailRequest()
                                        {
                                            MissionId = p,
                                            UserId = userId,
                                            ForceActivityTime = true
                                        });
                                    t.Wait();
                                });
                            return OperationResult.FromResult(true);
                        }
                    }
                }
                else
                {
                    // 从实际数据判断

                    // 绑定微信
                    var task1 = Task.Run(async () =>
                    {
                        using (var userAccountClient = new UserAccountClient())
                        {
                            // 获取用户数据
                            var userInfo = await userAccountClient.GetUserByIdAsync(userId);

                            return userInfo.Result?.UserAuths.ContainsKey("WeiXinAppOpen");
                        }
                    });

                    // 关注公众号
                    //var task2 = Task.Run(async () =>
                    //{
                    //    using (var userAuthAccountClient = new UserAuthAccountClient())
                    //    {

                    //        // 获取用户数据
                    //        //var userInfo = await Push.pu

                    //        return userInfo.Result?.UserAuths.ContainsKey("WeiXinAppOpen");
                    //    }
                    //});

                    // 车型认证

                }


            }
            return OperationResult.FromResult(false);
        }

        #endregion
    }
}
