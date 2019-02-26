using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.MessageQueue;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class BigBrandService : IBigBrandService
    {
        public readonly RabbitMQProducer produceer;
        private static ILog Logger = LogManager.GetLogger(typeof(BigBrandService));
        public BigBrandService()
        {
            try
            {
                produceer = RabbitMQClient.DefaultClient.CreateProducer("topic.notificationExchage");
            }
            catch (Exception e)
            {

                produceer = null;
            }

        }

        /// <summary>
        /// 发放分享组优惠券
        /// </summary>
        /// <param name="fightGroupsIdentity"></param>
        /// <returns></returns>
        public async Task<OperationResult<FightGroupsPacketsProvideResponse>> CreateFightGroupsPacketByPromotionAsync(Guid fightGroupsIdentity) => OperationResult.FromResult(await FightGroupsPacketsManager.CreateFightGroupsPacketByPromotion(fightGroupsIdentity));


        public async Task<OperationResult<List<BigBrandRewardLogModel>>> SelectShareListAsync(Guid userId,
            string hashKey, int chanceType)
        {
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                return OperationResult.FromError<List<BigBrandRewardLogModel>>("-1", "抽奖活动不存在");
            }
            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
                return OperationResult.FromError<List<BigBrandRewardLogModel>>("-1", "抽奖活动hashkey不匹配");

            if (userId == Guid.Empty)
            {
                return OperationResult.FromError<List<BigBrandRewardLogModel>>("-1", "用户UserId不能都为空");
                ;
            }
            BigBrandResponse result = new BigBrandResponse();
            BigBrandLogic bigBrand = new BigBrandLogic(hashKey, userId, string.Empty, string.Empty, string.Empty);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return OperationResult.FromError<List<BigBrandRewardLogModel>>("-16", bigBrand.ErrorMessage);
            }

            return OperationResult.FromResult(await bigBrand.GetShareLogList(chanceType));
        }

        /// <summary>
        /// 问答抽奖
        /// </summary>
        /// <param name="userId">用户userId</param>
        /// <param name="deviceId">设备Id</param>
        /// <param name="Channal">渠道</param>
        /// <param name="hashKey">抽奖hashKey</param>
        /// <param name="phone">手机号</param>
        /// <param name="refer">url</param>
        /// <param name="grade">得分</param>
        /// <returns></returns>
        public async Task<OperationResult<BigBrandResponse>> GetAnswerPacketAsync(Guid userId, string deviceId, string Channal, string hashKey, string phone, string refer)
        {
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                return OperationResult.FromResult(new BigBrandResponse() { Code = -1, Msg = "抽奖活动不存在" });
            }

            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
                return OperationResult.FromResult(new BigBrandResponse() { Code = -1, Msg = "抽奖活动hashkey不匹配" });

            if (userId == null)
            {
                return OperationResult.FromResult(new BigBrandResponse() { Code = -1, Msg = "用户UserId不能为空" });
            }

            BigBrandResponse result = new BigBrandResponse();
            BigBrandLogic bigBrand = new BigBrandLogic(hashKey, userId, phone, deviceId, Channal);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return OperationResult.FromError<BigBrandResponse>("-16", bigBrand.ErrorMessage);
            }

            bigBrand.Refer = refer;

            var isResult = await bigBrand.IsSelectCanPackage();
            if (!isResult.Result)
            {
                result.Code = 0;
                if (bigBrand.entity.BigBrandType == 2)
                    result.Msg = "您的积分不足，无法抽奖哦";
                else if (bigBrand.entity.BigBrandType == 3)
                    result.Msg = "抱歉，只有指定用户才能参加此抽奖哦";
                else
                    result.Msg = "抽奖次数不足";

                return OperationResult.FromResult(result);
            }

            System.Diagnostics.Stopwatch watcher = new System.Diagnostics.Stopwatch();
            watcher.Restart();
            bigBrand._time = await QuesAnsManager.GetGrade(userId, hashKey);
            watcher.Stop();
            Logger.Info($"GetAnswerPacketAsync GetGrade times {watcher.ElapsedMilliseconds}");
            bigBrand._time += 1;
            watcher.Restart();
            var pageIndex = bigBrand.GetRandomPageckagePKID();
            watcher.Stop();
            Logger.Info($"GetAnswerPacketAsync GetRandomPageckagePKID times {watcher.ElapsedMilliseconds}");
            if (pageIndex <= 0)
            {
                result.Code = 0;
                result.Msg = "对不起，您来晚了";
                return OperationResult.FromResult(result);
            }
            watcher.Restart();
            var response = await bigBrand.CreatePageage(pageIndex);
            watcher.Stop();
            Logger.Info($"GetAnswerPacketAsync CreatePageage times {watcher.ElapsedMilliseconds}");
            if (!response.Success)
            {
                return OperationResult.FromError<BigBrandResponse>(response.ErrorCode, response.ErrorMessage);
            }
            var page = response.Result;
            watcher.Restart();
            await bigBrand.AddPackageLog(page.PKID, page.PromotionCodePKIDs);
            watcher.Stop();
            Logger.Info($"GetAnswerPacketAsync AddPackageLog times {watcher.ElapsedMilliseconds}");
            bigBrand._time -= 1;
            result.Code = 1;
            result.PromptImg = page.PromptImg;
            result.PromptMsg = page.PromptMsg.Replace("{{allquestion}}", bigBrand?.entity?.AnsQuesConfig?.TipCount.ToString()).Replace("{{rightanswer}}", bigBrand?._time.ToString());
            result.PromptType = page.PromptType;
            result.RedirectAPP = page.RedirectAPP;
            result.RedirectBtnText = page.RedirectBtnText;
            result.RedirectH5 = page.RedirectH5;
            result.RedirectWXAPP = page.RedirectWXAPP;
            result.RedirectHuaWei = page.RedirectHuaWei;
            result.WxAppId = page.WxAppId;
            result.RewardType = page.RewardType;
            result.IsShare = bigBrand.IsShare;
            result.TimeCount = bigBrand.TimeCount;
            result.ShareTimes = bigBrand.ShareTimes;
            result.Time = bigBrand._time;
            result.RealTip = bigBrand?.RealTip;
            if (GlobalConstant.HashKey_VehicleTypeCertificationRights.Contains(hashKey))
            {
                result.DefaultPool = bigBrand.DefaultPool;
                result.CouponRuleItems = await bigBrand.GetCouponRuleItems(page.PKID, DateTime.Now);
            }
            produceer?.Send("notification.TaskActionQueue", new
            {
                UserId =userId,
                ActionName = "2LuckyMoney",
                HashKey = hashKey
            });
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取 活动页配置 详情
        /// </summary>
        /// <param name="keyValue">活动的Hash值</param>
        /// <returns></returns>
        public async Task<OperationResult<BigBrandRewardListModel>> GetBigBrandAsync(string keyValue) => OperationResult.FromResult(await BigBrandManager.GetBigBrand(keyValue));

        public async Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> GetFightGroupsPacketsListAsync(Guid? fightGroupsIdentity,Guid userId) => OperationResult.FromResult(await FightGroupsPacketsManager.GetFightGroupsPacketsList(fightGroupsIdentity, userId));
       
        /// <summary>
        /// 获取抽奖结果 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <param name="Channal"></param>
        /// <param name="hashKey"></param>
        /// <param name="phone"></param>
        /// <param name="refer"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<OperationResult<BigBrandResponse>> GetPacketAsync(Guid userId, string deviceId, string Channal, string hashKey, string phone, string refer,string openId=default(string))
        {
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                return OperationResult.FromResult(new BigBrandResponse() { Code = -1, Msg = "抽奖活动不存在" });
            }
            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
                return OperationResult.FromResult(new BigBrandResponse() { Code = -1, Msg = "抽奖活动hashkey不匹配" });

            if (userId == Guid.Empty && string.IsNullOrEmpty(openId))
            {
                return OperationResult.FromResult(new BigBrandResponse() {Code = -1, Msg = "用户UserId和OpenId不能都为空"});
            }
            BigBrandResponse result = new BigBrandResponse();
            System.Diagnostics.Stopwatch watcher = new System.Diagnostics.Stopwatch();
            watcher.Start();
            BigBrandLogic bigBrand = new BigBrandLogic(hashKey, userId, phone, deviceId, Channal, openId);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return OperationResult.FromError<BigBrandResponse>("-16", bigBrand.ErrorMessage);
            }

            watcher.Stop();
            Logger.Info($"GetPacketAsync new BigBrandLogic times:{watcher.ElapsedMilliseconds}");
            bigBrand.Refer = refer;
            watcher.Restart();
            var isResult = await bigBrand.IsSelectCanPackage();
            watcher.Stop();
            Logger.Info($"GetPacketAsync IsSelectCanPackage times:{watcher.ElapsedMilliseconds}");
            if (!isResult.Result)
            {
                result.Code = 0;
                if (!string.IsNullOrEmpty(isResult.ErrorMessage))
                {
                    result.Code = int.Parse(isResult.ErrorCode);
                    result.Msg = isResult.ErrorMessage;
                }
                else if (bigBrand.entity.BigBrandType == 2)
                    result.Msg = "您的积分不足，无法抽奖哦";
                else if (bigBrand.entity.BigBrandType == 3)
                    result.Msg = "抱歉，只有指定用户才能参加此抽奖哦";
                else
                    result.Msg = "抽奖次数不足";

                return OperationResult.FromResult(result);
            }
            watcher.Restart();
            var pageIndex = bigBrand.GetRandomPageckagePKID();
            watcher.Stop();
            Logger.Info($"GetPacketAsync GetRandomPageckagePKID times:{watcher.ElapsedMilliseconds}");
            if (pageIndex <= 0)
            {
                result.Code = 0;
                result.Msg = "对不起，您来晚了";
                return OperationResult.FromResult(result);
            }
            watcher.Restart();
            var response = await bigBrand.CreatePageage(pageIndex);
            watcher.Stop();
            Logger.Info($"GetPacketAsync CreatePageage times:{watcher.ElapsedMilliseconds}");
            if (!response.Success)
            {
                return OperationResult.FromResult(new BigBrandResponse() { Code = -1, Msg = response.ErrorMessage });
            }
            var page = response.Result;
            watcher.Restart();
            await bigBrand.AddPackageLog(page.PKID, page.PromotionCodePKIDs);
            watcher.Stop();
            Logger.Info($"GetPacketAsync AddPackageLog times:{watcher.ElapsedMilliseconds}");
            result.Code = 1;
            result.PromptImg = page.PromptImg;
            result.PromptMsg = page.PromptMsg;
            result.PromptType = page.PromptType;
            result.RedirectAPP = page.RedirectAPP;
            result.RedirectBtnText = page.RedirectBtnText;
            result.RedirectH5 = page.RedirectH5;
            result.RedirectWXAPP = page.RedirectWXAPP;
            result.RedirectHuaWei = page.RedirectHuaWei;
            result.WxAppId = page.WxAppId;
            result.RewardType = page.RewardType;
            result.IsShare = bigBrand.IsShare;
            result.TimeCount = bigBrand.TimeCount;
            result.ShareTimes = bigBrand.ShareTimes;
            result.Time = bigBrand._time;
            result.RealTip = bigBrand?.RealTip;
            try
            {
                var couponRulesItems = await bigBrand.GetCouponRuleItems(page.PKID, DateTime.Now);
                result.CouponRules = couponRulesItems?.ToList() ?? new List<CouponRule>();
                if (GlobalConstant.HashKey_VehicleTypeCertificationRights.Contains(hashKey))
                {
                    result.DefaultPool = bigBrand.DefaultPool;
                    result.CouponRuleItems = couponRulesItems;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            produceer?.Send("notification.TaskActionQueue", new
            {
                UserId = userId,
                ActionName = "2LuckyMoney",
                HashKey = hashKey
            });
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 问题列表
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="hashKey">抽奖hashKey</param>
        /// <returns></returns>
        public async Task<OperationResult<List<BigBrandQuesList>>> GetQuestionListAsync(Guid userId, string hashKey)
        {
            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
                return null;

            var list = await QuesAnsManager.GetBigBrandQuesList(userId, hashKey);

            if (list == null || list?.Where(_ => _.IsFinish == false)?.Count() == 0)
            {
                var quesList = await QuesAnsManager.GetAnswerInfoList();
                if (quesList?.Count <= 0)
                {
                    return null;//问答题库没有信息
                }
                   
                //生成问答试卷
                Random random = new Random(Guid.NewGuid().GetHashCode());

                string curNo = DateTime.Now.ToString("yyyyMMddHHmmss") + (random.Next(0, quesList.Count)).ToString("0000");
                var bigBrand = await BigBrandManager.GetBigBrand(hashKey);
                if (bigBrand.AnsQuesConfig == null || bigBrand?.AnsQuesConfig?.TipCount == 0)
                {
                    return null;//没有配置问答抽奖的题目数量
                }

                List<BigBrandQuesList> quesModelList = new List<BigBrandQuesList>();
                #region 生成题库随机集合
                List<int> indexs = new List<int>();
                while (indexs?.Count < bigBrand.AnsQuesConfig?.TipCount)
                {
                    random = new Random(Guid.NewGuid().GetHashCode());
                    var number = random.Next(0, quesList.Count);
                    if (!indexs.Exists(_ => _ == number))
                        indexs.Add(number);
                    else
                    {
                        number++;
                        if (number >= quesList.Count || indexs.Exists(_ => _ == number))
                            continue;
                        else
                            indexs.Add(number);
                    }

                }
                #endregion

                int orderby = 1;

                #region 创建试卷
                foreach (var index in indexs)
                {
                    var item = quesList[index];
                    BigBrandQuesList model = new BigBrandQuesList()
                    {
                        Answer = item.Answer,
                        CreateDateTime = DateTime.Now,
                        CurAnsNo = curNo,
                        HashKey = hashKey,
                        IsFinish = false,
                        LastUpdateDateTime = DateTime.Now,
                        OptionsA = item.OptionsA,
                        OptionsB = item.OptionsB,
                        OptionsC = item.OptionsC,
                        OptionsD = item.OptionsD,
                        OptionsReal = item.OptionsReal,
                        OrderBy = orderby++,
                        Tip = item.Tip,
                        UserId = userId
                    };
                    quesModelList.Add(model);
                }
                #endregion
                var result = QuesAnsManager.InsertBigBrandQues(quesModelList);
                if (result)
                    list = await QuesAnsManager.GetBigBrandQuesList(userId, hashKey, false);

            }

            return OperationResult.FromResult(list);
        }

        /// <summary>
        /// 新增分享红包组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> InsertFightGroupsPacketAsync(Guid userId) => OperationResult.FromResult(await FightGroupsPacketsManager.InsertFightGroupsPacket(userId));
      

        public async Task<OperationResult<IEnumerable<BigBrandRealLogModel>>> IsNULLBigBrandRealByAddressAsync(string hashKey, Guid userId,string phone,string deviceId,string Channal)
        {
            if (string.IsNullOrWhiteSpace(hashKey))
                return null;
            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
                return null;

            BigBrandLogic bigBrand = new BigBrandLogic(hashKey, userId, phone, deviceId, Channal);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return null;
            }

            if (bigBrand == null)
                return null;

            return OperationResult.FromResult(await bigBrand.IsNULLBigBrandRealByAddress());
        }

        public async Task<OperationResult<BigBrandCanResponse>> SelectCanPackerAsync(Guid userId, string deviceId, string Channal, string hashKey, string phone, string refer,string openId=default(string))
        {
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                return OperationResult.FromResult(new BigBrandCanResponse() { Code = -1, Msg = "Hash不能为空" });
            }

            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
                return OperationResult.FromResult(new BigBrandCanResponse() { Code = -1, Msg = "Hash不匹配" });


            if (userId == Guid.Empty&&string.IsNullOrEmpty(openId))
            {
                return OperationResult.FromResult(new BigBrandCanResponse() { Code = -1, Msg = "用户UserId和OpenId不能都为空" });
            }

            BigBrandLogic bigBrand = new BigBrandLogic(hashKey, userId, phone, deviceId, Channal, openId);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return OperationResult.FromError<BigBrandCanResponse>("-16", bigBrand.ErrorMessage);
            }

            bigBrand.Refer = refer;
            var result = await bigBrand.IsSelectCanPackage();
            if (GlobalConstant.HashKey_VehicleTypeCertificationRights.Contains(hashKey))
                bigBrand.GetRandomPageckagePKID();

            if (result.Result)
            {
                return OperationResult.FromResult(new BigBrandCanResponse()
                {
                    Code = 1,
                    IsShare = bigBrand.IsShare,
                    Item = string.Empty,
                    Msg = "今天还有" + bigBrand.TimeCount + "次机会",
                    Times = bigBrand.TimeCount,
                    DefaultPool = bigBrand.DefaultPool, //只对 23338FF5 870F1F2E活动有效
                    ShareTimes=bigBrand.ShareTimes
                });
            }
            else
            {
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                   return OperationResult.FromError<BigBrandCanResponse>(result.ErrorCode, result.ErrorMessage);
                }
                else
                {
                    var model = new BigBrandCanResponse()
                    {
                        Code = 0,
                        IsShare = bigBrand.IsShare,
                        // Item = hashKey == "23338FF5" || hashKey == "870F1F2E" ? Newtonsoft.Json.JsonConvert.SerializeObject(bigBrand.GetRewardInfoLast(userId)) : "",
                        Msg = "今天还有" + bigBrand.TimeCount ?? 0 + "次机会",
                        Times = bigBrand.TimeCount,
                        DefaultPool = bigBrand.DefaultPool, //只对 23338FF5 870F1F2E活动有效
                        ShareTimes = bigBrand.ShareTimes
                    };
                    if (GlobalConstant.HashKey_VehicleTypeCertificationRights.Contains(hashKey))
                    {
                        //bigBrand.PromotionCodes
                        var package = bigBrand.GetRewardInfoLast(userId);
                        model.Item = Newtonsoft.Json.JsonConvert.SerializeObject(package);
                        if (package != null)
                            model.CouponRuleItems = await bigBrand.GetCouponRuleItems(package.PKID, package.DateTimeLog.Value);
                        if (package != null && bigBrand?.PromotionCodes?.Count() > 0)
                        {
                            using (var memberClient = new Tuhu.Service.Member.PromotionClient())
                            {
                                var memberResult = await memberClient.FetchPromotionCodeByIDAsync(new Member.Request.FetchPromotionCodeRequest()
                                {
                                    PKID = Convert.ToInt32(bigBrand.PromotionCodes[0])
                                });
                                if (memberResult.Success)
                                {
                                    var couponRuleModel = model.CouponRuleItems?.FirstOrDefault();
                                    couponRuleModel.CreateDateTime = Convert.ToDateTime(memberResult.Result.StartTime);
                                    couponRuleModel.ValiStartDate = Convert.ToDateTime(memberResult.Result.StartTime);
                                    couponRuleModel.ValiEndDate = Convert.ToDateTime(memberResult.Result.EndTime);
                                    couponRuleModel.Term = null;
                                    couponRuleModel.DateNumber = memberResult.Result.Status == 0 ? couponRuleModel.GetDateNumber() : -2;
                                }
                            }
                        }
                    }
                    return OperationResult.FromResult(model);
                }
                
            }
        }

        public async Task<OperationResult<string>> SelectPackAsync(string hashKey)
        {
            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
                return OperationResult.FromResult("参数不正确");

            BigBrandLogic bigBrand = new BigBrandLogic(hashKey);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return OperationResult.FromError<string>("-16", bigBrand.ErrorMessage);
            }

            bigBrand.Refer = "";
            return OperationResult.FromResult( Newtonsoft.Json.JsonConvert.SerializeObject( await bigBrand.GetSelectLoge()));
        }

        /// <summary>
        /// 设置问题结果
        /// </summary>
        /// <param name="userId">用户userId</param>
        /// <param name="pkid">问题pkid</param>
        /// <param name="resOptions">用户选项</param>
        /// <returns></returns>
        public async Task<OperationResult<QuestionAnsResponse>> SetAnswerResAsync(QuestionAnsRequestModel request)
        {
            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(request.hashKey))
                return OperationResult.FromResult(new QuestionAnsResponse() { IsReal = false });

            if (request.pkid <= 0)
                return OperationResult.FromResult(new QuestionAnsResponse() { IsReal=false });
            if (string.IsNullOrWhiteSpace(request.resOptions))
                return OperationResult.FromResult(new QuestionAnsResponse() { IsReal = false });
            var list = await QuesAnsManager.GetBigBrandQuesList(request.userId, request.hashKey, false);
            var realValue = list?.Where(_ => _.IsFinish == true)?.Where(_ => _.ResValue == _.OptionsReal)?.Count();
            var errorValue = list?.Where(_ => _.IsFinish == true)?.Where(_ => _.ResValue != _.OptionsReal)?.Count();
            await QuesAnsManager.UpdateBigBrandQues(request.pkid, request.resOptions);
            var entity = await QuesAnsManager.GetBigBrandQuesEntity(request.pkid);
            return OperationResult.FromResult(new QuestionAnsResponse() { IsReal = entity?.OptionsReal == request.resOptions, OptionsReal = entity?.OptionsReal, RealValue= entity?.OptionsReal == request.resOptions ? (realValue+1):realValue, Error= entity?.OptionsReal == request.resOptions ? (errorValue) : (errorValue+1) });
        }

        public async Task<OperationResult<bool>> ShareAddByOrderAsync(Guid userId, string deviceId, string Channal, string hashKey, string phone, string refer, int times)
        {
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                return OperationResult.FromResult(false);
            }

            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
            {
                return OperationResult.FromResult(false);
            }

            if (userId == null)
            {
                return OperationResult.FromResult(false);
            }

            if (times <= 0)
                return OperationResult.FromResult(false);

            BigBrandLogic bigBrand = new BigBrandLogic(hashKey, userId, phone, deviceId, Channal);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return OperationResult.FromError<bool>("-16", bigBrand.ErrorMessage);
            }

            bigBrand.Refer = refer;

            var result = await bigBrand.AddShareLog(times, 2);
            return OperationResult.FromResult(result);
        }
        public async Task<OperationResult<bool>> AddBigBrandTimesAsync(Guid userId, string deviceId, string Channal, string hashKey, string phone, string refer,
            int times)
        {
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                return OperationResult.FromResult(false);
            }

            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
            {
                return OperationResult.FromResult(false);
            }

            if (userId == null)
            {
                return OperationResult.FromResult(false);
            }

            if (times <= 0)
                return OperationResult.FromResult(false);

            BigBrandLogic bigBrand = new BigBrandLogic(hashKey, userId, phone, deviceId, Channal);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return OperationResult.FromError<bool>("-16", bigBrand.ErrorMessage);
            }

            bigBrand.Refer = refer;

            var result = await bigBrand.AddShareLog(times, 4);
            return OperationResult.FromResult(result);
        }
        public async Task<OperationResult<bool>> ShareAddOneAsync(Guid userId, string deviceId, string Channal, string hashKey, string phone, string refer,string openId=default(string), int chanceType = 2)
        {
            if (string.IsNullOrWhiteSpace(hashKey))
            {
                return OperationResult.FromResult(false);
            }

            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
            {
                return OperationResult.FromResult(false);
            }

            if (userId == null)
            {
                return OperationResult.FromResult(false);
            }

            BigBrandLogic bigBrand = new BigBrandLogic(hashKey, userId, phone, deviceId, Channal, openId);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return OperationResult.FromError<bool>("-16", bigBrand.ErrorMessage);
            }

            bigBrand.Refer = refer;
            chanceType = chanceType == 0 ? 2 : chanceType;
            var result = await bigBrand.AddShareLogWithChanceType(chanceType);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 更新活动 的缓存
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UpdateBigBrandAsync(string keyValue) => OperationResult.FromResult(await BigBrandManager.UpdateBigBrand(keyValue));

        public async Task<OperationResult<BigBrandRealResponse>> UpdateBigBrandRealLogAsync(string hashKey, Guid userId, string address, Guid tip, string phone, string deviceId, string Channal,string userName)
        {
            if (string.IsNullOrWhiteSpace(hashKey))
                return OperationResult.FromResult(new BigBrandRealResponse() {Code =-1,Msg="HashKey不能为空" });

            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(hashKey))
            {
                return OperationResult.FromResult(new BigBrandRealResponse() { Code = -1, Msg = "HashKey不匹配规则" });
            }

            BigBrandLogic bigBrand = new BigBrandLogic(hashKey, userId, phone, deviceId, Channal);
            if (!string.IsNullOrWhiteSpace(bigBrand?.ErrorMessage))
            {
                return OperationResult.FromError<BigBrandRealResponse>("-16", bigBrand.ErrorMessage);
            }

            var result = await bigBrand.UpdateBigBrandRealLog(address, tip,userName,phone);
            if (result)
                return OperationResult.FromResult(new BigBrandRealResponse() { Code=1, Msg="更新地址" });
            else
                return OperationResult.FromResult(new BigBrandRealResponse() { Code = -1, Msg = "更新地址" });
        }

        /// <summary>
        /// 更新分享组红包中的用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UpdateFightGroupsPacketByUserIdAsync(FightGroupsPacketsUpdateRequest request) => await FightGroupsPacketsManager.UpdateFightGroupsPacketByUserId(request);


        /// <summary>
        /// 更新问题库
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UpdateQuestionInfoListAsync()
        {
           return OperationResult.FromResult( await QuesAnsManager.UpdateAnswerInfoList());
        }
    }
}
