using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.DataAccess;
using Common.Logging;
using Tuhu.Nosql;
using Tuhu.Service.Member.Models;
using System.Text.RegularExpressions;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Config;
using Tuhu.Service.Activity.Server.ServiceProxy;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class BigBrandManager
    {
        public static readonly string DefaultClientName = "BigBrandRewardService";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BigBrandManager));

        public static async Task<BigBrandRewardListModel> GetBigBrand(string keyValue)
        {
            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(keyValue))
                return null;

            if (string.IsNullOrWhiteSpace(keyValue))
                return null;

            using (var client = Tuhu.Nosql.CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync<BigBrandRewardListModel>(keyValue, async () => await GetEntity(keyValue), TimeSpan.FromHours(6));
                if (result.Success)
                    return result.Value;
                else
                    return await GetEntity(keyValue);
            }
        }

        public static async Task<bool> UpdateBigBrand(string keyValue)
        {
            if (string.IsNullOrWhiteSpace(keyValue))
                return false;

            using (var client = Tuhu.Nosql.CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.SetAsync<BigBrandRewardListModel>(keyValue, await GetEntity(keyValue));
                return result.Success;
            }
        }

        private static async Task<BigBrandRewardListModel> GetEntity(string keyValue)
        {
            if (string.IsNullOrWhiteSpace(keyValue))
                return null;

            Regex regex = new Regex("^([0-9]|[a-z]|[A-Z]){7}([0-9]|[a-z]|[A-Z])$");
            if (!regex.IsMatch(keyValue))
            {
                return null;
            }

            var listEntity = await DalBigBrand.GetBigBrandRewardListModel(keyValue);
            var styleEntity = DalBigBrand.GetBigBrandPageStyleList(listEntity.PKID);
            var timeEntity = DalBigBrand.GetBigBrandWheelList(listEntity.PKID);
            var poolEntity = DalBigBrand.GetBigBrandRewardPoolList(listEntity.PKID);
            var ansquesEntity = DalBigBrand.GetAnsQuesEntity(listEntity.PKID);
            var pageConfigEntity = DalBigBrand.GetBigBrandPageConfigModel(listEntity.PKID,0);
            var bigBrandPageConfigEntity = DalBigBrand.GetBigBrandPageConfigBrand(listEntity.PKID);
            await Task.WhenAll(styleEntity, timeEntity, poolEntity, ansquesEntity, pageConfigEntity, bigBrandPageConfigEntity);
            listEntity.ItemStyles = styleEntity.Result;
            listEntity.ItemTimes = timeEntity.Result;
            listEntity.AnsQuesConfig = ansquesEntity.Result;
            listEntity.PageConfig = pageConfigEntity.Result;
            listEntity.BigBrandPageConfig = bigBrandPageConfigEntity.Result;
            var poolList = new List<BigBrandRewardPoolModel>();
            var items = poolEntity?.Result;
            if (items != null)
                foreach (var parent in items?.Where(_ => _.ParentPKID == null && _.Status == true))
                {
                    var entity = parent;
                    var part = items.Where(_ => _.ParentPKID == entity.PKID && _.Status == true)?.ToList();
                    part.ForEach(_ =>
                    {
                        _.PartItem = items?.Where(o => o.ParentPKID == _.PKID && o.Status == true)?.ToList();
                    });
                    entity.PartItem = part;
                    poolList.Add(entity);
                }
            listEntity.ItemPools = poolList;
            return listEntity;

        }


    }

    public class BigBrandLogic
    {
        /// <summary>
        /// 统计抽奖领取各个礼包的总数字典
        /// </summary>
        private static readonly string BigBrandCountLogKey = "_BigBrandCountLogKey";

        /// <summary>
        /// 用户领取记录缓存
        /// </summary>
        private static readonly string BigBrandLogByUser = "_BigBrandLogByUser";

        public BigBrandRewardListModel entity { get; set; }

        public Guid UserId { get; set; }

        public string DeviceId { get; set; }

        public string Channel { get; set; }

        public string Phone { get; set; }
        public string UnionId { get; set; }
        public string OpenId { get; set; }
        /// <summary>
        /// 是否分享过
        /// </summary>
        public bool IsShare { get; set; }

        /// <summary>
        /// 分享后可以增加的次数
        /// </summary>
        public int? ShareTimes { get; set; }

        /// <summary>
        /// 分享总数
        /// </summary>
        public int? ShareCount { get; set; }


        /// <summary>
        /// 具有的抽奖次数
        /// </summary>
        public int? TimeCount { get; set; }

        public string Refer { get; set; }

        /// <summary>
        /// 是否开启了兜底奖池
        /// </summary>
        public bool DefaultPool { get; set; }

        /// <summary>
        /// 抽奖轮次
        /// </summary>
        public int _time { get; set; }

        /// <summary>
        /// 实物奖励标记
        /// </summary>
        public Guid? RealTip { get; set; }

        /// <summary>
        /// 本次抽奖记录的开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 本次抽奖结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 创建对象时错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        public IEnumerable<Push.Models.WeiXinPush.WXUserAuthModel> WxUsers { get; set; }
        public static ILog Logger = LogManager.GetLogger(typeof(BigBrandLogic));
        private System.Diagnostics.Stopwatch watcher = new System.Diagnostics.Stopwatch();


        public BigBrandLogic(string keyValue, Guid userId, string phone, string deviceId, string channel, string openId = default(string))
        {
            watcher.Restart();
            entity = GetEntity(keyValue);
            watcher.Stop();
            Logger.Info($"BigBrandLogic GetEntity times:{watcher.ElapsedMilliseconds}");
            //string uid = userId.ToString();

            //if (uid.IndexOf("{") != 0)
            //{
            //    uid = "{" + uid + "}";
            //}
            if (!string.IsNullOrEmpty(openId)) //如果openid 不为空,根据openid查找userid
            {
                watcher.Restart();
                using (var client = new Push.WeiXinPushClient())
                {
                    var wxAuthResponse = client.SelectAllWxAuthInfos(openId);
                    if (wxAuthResponse.Success && wxAuthResponse.Result.Any())
                    {
                        this.WxUsers = wxAuthResponse.Result;
                        var first = wxAuthResponse.Result.First();
                        if (userId == Guid.Empty)
                        {
                            this.UserId = first.UserId ?? userId;
                        }
                        this.OpenId = openId;
                        this.UnionId = first.UnionId;
                    }
                }
                watcher.Stop();
                Logger.Info($"BigBrandLogic SelectAllWxAuthInfos times:{watcher.ElapsedMilliseconds}");
            }
            else
            {
                this.UserId = userId;
            }
            this.DeviceId = deviceId;
            this.Channel = channel;
            this.Phone = phone;
            this.ShareTimes = entity.CompletedTimes;

            //计算当前大翻盘的周期时间 并存入缓存
            var resultMsg = SetPeriodDateTime(keyValue);
            this.ErrorMessage = resultMsg;
        }

        /// <summary>
        /// 计算当前大翻盘的周期时间 并存入缓存
        /// </summary>
        public string SetPeriodDateTime(string keyValue)
        {
            var errorResult = "";
            if (entity.PeriodType != 1 && entity.PeriodType != 2 && entity.PeriodType != 3)
            {
                return "获取领取记录的周期类型不符合配置";
            }

            if (this.entity.StartDateTime == null)
                this.entity.StartDateTime = this.entity.CreateDateTime;
            if (this.entity.Period == null || this.entity.Period <= 0)
                this.entity.Period = 1;
            if (DateTime.Now < this.entity.StartDateTime)
            {
                return "不在抽奖时间范围内，不能抽奖";
            }

            //从缓存中获取，第一次获取时 为最小值
            if (DateTime.Now >= this.entity.PeriodStartDateTime && DateTime.Now < this.entity.PeriodEndDateTime)
            {
                this.StartDateTime = this.entity.PeriodStartDateTime;
                this.EndDateTime = this.entity.PeriodEndDateTime;
                return "";
            }
            else
            {
                if (entity.PeriodType == 1)//小时
                {
                    double seconds = (DateTime.Now - this.entity.StartDateTime.Value).TotalSeconds;
                    int section = (int)(seconds / (60 * 60 * this.entity.Period));
                    this.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + this.entity.StartDateTime.Value.AddHours(this.entity.Period.Value * section).ToString("HH:mm:ss"));
                    this.EndDateTime = Convert.ToDateTime(this.StartDateTime.AddHours(this.entity.Period.Value).ToString("yyyy-MM-dd HH:mm:ss"));
                }

                if (entity.PeriodType == 2)//天
                {
                    double seconds = (DateTime.Now - this.entity.StartDateTime.Value).TotalSeconds;
                    int section = (int)(seconds / (60 * 60 * 24 * this.entity.Period));
                    this.StartDateTime = Convert.ToDateTime(this.entity.StartDateTime.Value.AddDays(this.entity.Period.Value * section).ToString("yyyy-MM-dd HH:mm:ss"));
                    this.EndDateTime = Convert.ToDateTime(this.StartDateTime.AddDays(this.entity.Period.Value).ToString("yyyy-MM-dd HH:mm:ss"));
                }

                if (entity.PeriodType == 3)//月
                {
                    this.StartDateTime = this.entity.StartDateTime.Value;
                    while (true)
                    {
                        int i = 0;
                        this.EndDateTime = this.StartDateTime.AddMonths(this.entity.Period.Value);
                        if (DateTime.Now >= this.StartDateTime && DateTime.Now < this.EndDateTime)
                        {
                            break;
                        }
                        if (i >= 100)//避免死循环
                        {
                            return $"按月计算抽奖周期 异常 PKID={this.entity.PKID}";
                        }

                        this.StartDateTime = this.StartDateTime.AddMonths(this.entity.Period.Value);
                        i++;
                    }
                }

                //重新计算周期后 重复 覆盖缓存
                this.entity.PeriodStartDateTime = this.StartDateTime;
                this.entity.PeriodEndDateTime = this.EndDateTime;

                using (var client = CacheHelper.CreateCacheClient(BigBrandManager.DefaultClientName))
                {
                    var result = client.Set<BigBrandRewardListModel>(keyValue, this.entity, TimeSpan.FromHours(6));
                    if (!result.Success)
                        Logger.Info($"BigBrandLogic 更新大盘发 缓存 异常 pkid = {this.entity.PKID}");
                }
            }

            return errorResult;
        }


        public BigBrandLogic(string keyValue)
        {
            entity = GetEntity(keyValue);
            // 这个计数查出来 暂时没有用到 ，而且是慢查询，暂时注释掉 俊桥说是一个功能做了一半结果没有做了,之后可以用计数器来实现
            //using (var client = Tuhu.Nosql.CacheHelper.CreateCacheClient("BigBrandLogic"))
            //{
            //    var result = client.GetOrSet("BigBrandShareCount", () => { return DalBigBrand.GetBigBrandShareCount(entity.PKID); });
            //    if (result.Success)
            //        this.ShareCount = result.Value;
            //}
        }

        /// <summary>
        /// 获取抽奖池内容
        /// </summary>
        /// <returns></returns>
        private List<BigBrandPoolModel> CreatePool()
        {


            List<BigBrandPoolModel> list = new List<BigBrandPoolModel>();
            //var dicResult = DalBigBrand.GetBigBrandLogList(entity.PKID, entity.Period.Value, entity.PeriodType);
            watcher.Restart();
            var poolPKIDs = entity.ItemTimes?.Where(_ => _.TimeNumber == _time);
            if (poolPKIDs != null && poolPKIDs.Count() > 0)
                entity.ItemPools = entity.ItemPools?.Where(_ => poolPKIDs.FirstOrDefault(o => o.FKPoolPKID == _.PKID) != null || _.IsDefault == true)?.ToList();
            watcher.Stop();
            Logger.Info($"BigBrandLogic CreatePool StartFilter times:{watcher.ElapsedMilliseconds}");
            string key = BigBrandCountLogKey + this.entity.PKID + this.StartDateTime.ToString("yyyyMMddHHmmss") + this.EndDateTime.ToString("yyyyMMddHHmmss");
            using (var counter = CacheHelper.CreateCounterClient(key, TimeSpan.FromDays(30)))
            {
                Dictionary<int, int> noCacheDic = new Dictionary<int, int>(entity?.ItemPools?.Count() ?? 0);
                watcher.Restart();
                foreach (var item in entity?.ItemPools?.Where(_ => _.IsDefault == false))
                {
                    foreach (var part in item?.PartItem)
                    {
                        int counterNumber = 0;
                        var counterResult =
                            counter.Count(part.PKID
                                .ToString()); //RedisHelper.GetCounter(BigBrandCountLogKey +, part.PKID.ToString(), TimeSpan.FromDays(30), async () => await DalBigBrand.GetBigBrandLogList(entity.PKID, part.PKID, startDT, endDT));
                        if (counterResult.Value == 0 || !counterResult.Success)
                        {
                            noCacheDic[part.PKID] = part.Number ?? 0;
                            //把数据库获取的改为批量获取，挪到循环外面去
                            //counterNumber = DalBigBrand.GetBigBrandLogList(entity.PKID, part.PKID, this.StartDateTime, this.EndDateTime);
                            //counter.Increment(part.PKID.ToString(), counterNumber);
                        }
                        else
                        {
                            counterNumber = (int)counterResult.Value;

                            var number = part.Number - counterNumber;
                            if (number > 0)
                                list.Add(new BigBrandPoolModel() { Count = number.Value, PKID = part.PKID });
                        }

                    }
                }
                watcher.Stop();
                Logger.Info($"BigBrandLogic CreatePool GetCountFromRedis times:{watcher.ElapsedMilliseconds}");

                if (noCacheDic.Count > 0)
                {
                    watcher.Restart();
                    var noCacheResult = DalBigBrand.GetBatchBigBrandLogList(entity.PKID, noCacheDic.Keys,
                        this.StartDateTime, this.EndDateTime);
                    foreach (var keyItem in noCacheDic.Keys)
                    {
                        var counterNumber = 0;
                        noCacheResult.TryGetValue(keyItem, out counterNumber);
                        if (counterNumber > 0)
                        {
                            counter.Increment(keyItem.ToString(), counterNumber);
                        }
                        var number = noCacheDic[keyItem] - counterNumber;
                        if (number > 0)
                        {
                            list.Add(new BigBrandPoolModel() { Count = number, PKID = keyItem });
                        }
                    }
                    watcher.Stop();
                    Logger.Info($"BigBrandLogic CreatePool GetCountFromDatabase times:{watcher.ElapsedMilliseconds}");
                }
            }
            return list;
        }

        /// <summary>
        /// 获取抽中的礼包PKID
        /// </summary>
        /// <returns></returns>
        public int GetRandomPageckagePKID()
        {
            watcher.Restart();
            var poolList = CreatePool();
            watcher.Stop();
            Logger.Info($"BigBrandLogic CreatePool times:{watcher.ElapsedMilliseconds}");
            int pkid = 0;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            if (poolList?.Count > 0)
            {
                var index = random.Next(0, poolList.Sum(_ => _.Count));
                int count = 0;
                poolList.ForEach(_ =>
                {
                    count += _.Count;
                    if (count >= index)
                    {
                        pkid = _.PKID;
                        index = 999999999;
                    }
                });
            }
            else
            {
                this.DefaultPool = true;
                var pools = entity.ItemPools.Where(_ => _.IsDefault == true);
                List<int> pkids = new List<int>();
                if (pools != null)
                    foreach (var item in pools)
                    {
                        foreach (var part in item.PartItem)
                        {
                            pkids.Add(part.PKID);
                        }
                    }
                int index = random.Next(0, pkids.Count);
                if (pkids.Count > index)
                    pkid = pkids[index];
            }
            return pkid;
        }

        private BigBrandRewardListModel GetEntity(string keyValue)
        {
            var result = BigBrandManager.GetBigBrand(keyValue);
            return result.Result;
        }

        /// <summary>
        /// 获取抽中优惠券的code
        /// </summary>
        public string[] PromotionCodes { get; set; }
        /// <summary>
        /// 判断是否可以抽奖
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<bool>> IsSelectCanPackage()
        {
            var checkBigRed = IsCanPackage();
            if (!checkBigRed.Success) return checkBigRed;
            //不同抽奖
            watcher.Restart();
            var listLoger = await GetBigBrandLogListByUserIdCache(this.entity.PKID, this.entity.PeriodType, this.UserId, this.UnionId);
            watcher.Stop();
            Logger.Info($"BigBrandLogic GetBigBrandLogListByUserIdCache times:{watcher.ElapsedMilliseconds}");
            #region 设置抽中的PromotionCodes
            if (listLoger != null && listLoger?.Where(_ => _.ChanceType == 1)?.Count() > 0)
            {
                var modelLoger = listLoger?.Where(_ => _.ChanceType == 1 && !string.IsNullOrWhiteSpace(_.PromotionCodePKIDs));
                this.PromotionCodes = modelLoger?.Select(_ => _.PromotionCodePKIDs.TrimEnd(','))?.FirstOrDefault()?.Split(',');
            }
            #endregion
            if (listLoger?.Where(_ => _.ChanceType == 1) != null)
                this._time = listLoger.Where(_ => _.ChanceType == 1).Count() + 1;
            else
                this._time = 1;

            if (listLoger?.Where(_ => _.ChanceType == 2)?.Count() > 0)
                this.IsShare = true;
            else
                this.IsShare = false;

            if (this.entity.HashKeyValue.ToUpper() == "9A42C4C8")
            {

                var shopRecordResult = await SexAnnualVoteManager.SelectShopVoteRecordAsync(this.UserId, DateTime.Now.Date, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59")));
                var employeeRecordResult = await SexAnnualVoteManager.SelectShopEmployeeVoteRecordAsync(this.UserId, DateTime.Now.Date, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59")));
                if (shopRecordResult.Success || employeeRecordResult.Success)
                {
                    var allowTimeCount = shopRecordResult?.Result?.Count() + employeeRecordResult?.Result?.Count();
                    if (allowTimeCount >= listLoger?.Where(_ => _.ChanceType == 1)?.Count())
                    {
                        this.TimeCount = allowTimeCount - listLoger?.Where(_ => _.ChanceType == 1)?.Count();
                        return OperationResult.FromResult(true);
                    }
                    else
                        return OperationResult.FromResult(false);
                }
                else
                    return OperationResult.FromResult(false);
            }

            if (this.entity.BigBrandType == 1)
            {
                this.TimeCount = this.entity.PreTimes +
                                 (listLoger?.Where(_ => _.ChanceType == 2)?.Count() > 0
                                     ? this.entity.CompletedTimes
                                     : 0) + listLoger?.Where(_ => _.ChanceType == 4).Count() - //增加从其他途径过来增加的抽奖次数
                                 listLoger?.Where(_ => _.ChanceType == 1)?.Count();
            }
            else if (this.entity.BigBrandType == 2)
            {
                //积分抽奖
                using (var client = new Tuhu.Service.Member.UserIntegralClient())
                {
                    var result = await client.SelectUserIntegralByUserIDAsync(this.UserId);
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        if (result?.Result?.Integral >= this.entity.NeedIntegral)
                        {
                            this.TimeCount = 1;
                        }
                        else
                            this.TimeCount = 0;
                    }
                    else
                        this.TimeCount = 0;

                }
            }
            else
            {
                //人群抽奖
                var activityResult = await ActivityManager.GetLuckyWheelUserlotteryCountAsync(this.UserId, Guid.Empty, entity.HashKeyValue);
                if (activityResult > 0)
                    this.TimeCount = activityResult;
                else
                    this.TimeCount = 0;
            }
            return OperationResult.FromResult(this.TimeCount > 0);
        }

        #region 大翻盘抽奖验证  判断是否是红包抽奖 如果是红包抽奖，必须关注途虎微信公众号  如果设置了必须关注某个公众号做判断



        /// <summary>
        /// 判断是否是红包抽奖
        /// 如果是红包抽奖，必须关注途虎微信公众号
        /// 如果设置了必须关注某个公众号做判断
        /// </summary>
        /// <returns></returns>
        private OperationResult<bool> IsCanPackage()
        {
            if (this.entity.AfterLoginType == 0 || this.entity.AfterLoginType == null || string.IsNullOrWhiteSpace(this.entity.AfterLoginValue))//没有设置登录后置
                return OperationResult.FromResult(true);

            var afterLoginValueArr = this.entity.AfterLoginValue.Split(',');
            List<KeyValuePair<string, string>> wechatQRCodeList = GlobalConstant.WechatQrcodeURL.Where(p => afterLoginValueArr.Contains(p.Key)).ToList();

            if (this.UserId == Guid.Empty && this.WxUsers == null)
            {
                return OperationResult.FromError<bool>("-100", GlobalConstant.WechatQrcodeURL.Values.FirstOrDefault());
            }
            if (this.entity.AfterLoginType == 1 && !string.IsNullOrEmpty(this.entity.AfterLoginValue)) //OPENID验证
            {
                if (string.IsNullOrEmpty(this.OpenId))
                {
                    return OperationResult.FromError<bool>("0", "没有通过微信授权");
                }
                else if (this.WxUsers == null)
                {
                    return OperationResult.FromError<bool>("-100", GlobalConstant.WechatQrcodeURL.Values.FirstOrDefault());//配置的公众号一个也没关注
                }
                foreach (var wechatQRCode in wechatQRCodeList)
                {
                    if (!this.WxUsers.Any(x => x.Channel == wechatQRCode.Key && x.BindingStatus == "Bound"))
                    {
                        return OperationResult.FromError<bool>("-100", wechatQRCode.Value);//没有配置要求的公众号
                    }
                }
            }

            if (this.entity.ItemPools.Any(x => x.RewardType == 5)) //是微信红包抽奖
            {
                if (this.WxUsers == null)
                {
                    return OperationResult.FromError<bool>("-100", GlobalConstant.WechatQrcodeURL.Values.FirstOrDefault());
                }
                else
                {
                    if (this.WxUsers.Any(x => x.Channel == GlobalConstant.WechatQrcodeURL.Keys.FirstOrDefault() && x.BindingStatus == "Bound"))
                    {
                        return OperationResult.FromResult(true);
                    }
                    else
                    {
                        return OperationResult.FromError<bool>("-100", GlobalConstant.WechatQrcodeURL.Values.FirstOrDefault());
                    }
                }
            }
            else
            {
                return OperationResult.FromResult(true);
            }
        }


        //private OperationResult<bool> IsCanPackage()
        //{
        //    if (this.entity.AfterLoginType == 0) return OperationResult.FromResult(true);
        //    if (this.UserId == Guid.Empty && this.WxUsers == null)
        //    {
        //        return OperationResult.FromError<bool>("-100", "WX_APP_OfficialAccount");
        //    }
        //    if (this.entity.AfterLoginType == 1 && !string.IsNullOrEmpty(this.entity.AfterLoginValue)) //OPENID验证
        //    {
        //        if (string.IsNullOrEmpty(this.OpenId))
        //        {
        //            return OperationResult.FromError<bool>("0", "没有通过微信授权");
        //        }
        //        else if (this.WxUsers == null)
        //        {
        //            return OperationResult.FromError<bool>("-100", this.entity.AfterLoginValue);//配置的公众号一个也没关注
        //        }
        //        var afterLoginValueArr = this.entity.AfterLoginValue.Split(',');
        //        foreach (var item in afterLoginValueArr)
        //        {
        //            if (!this.WxUsers.Any(x => x.Channel == item && x.BindingStatus == "Bound"))
        //            {
        //                return OperationResult.FromError<bool>("-100", item);//没有配置要求的公众号
        //            }
        //        }
        //    }

        //    if (this.entity.ItemPools.Any(x => x.RewardType == 5)) //是微信红包抽奖
        //    {
        //        if (this.WxUsers == null)
        //        {
        //            return OperationResult.FromError<bool>("-100", "WX_APP_OfficialAccount");
        //        }
        //        else
        //        {
        //            if (this.WxUsers.Any(x => x.Channel == "WX_APP_OfficialAccount" && x.BindingStatus == "Bound"))
        //            {
        //                return OperationResult.FromResult(true);
        //            }
        //            else
        //            {
        //                return OperationResult.FromError<bool>("-100", "WX_APP_OfficialAccount");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return OperationResult.FromResult(true);
        //    }
        //}

        #endregion



        /// <summary>
        /// 获取本抽奖中用户的领取以及分享记录
        /// </summary>
        /// <param name="fkBigBrandPkid"></param>
        /// <param name="period"></param>
        /// <param name="periodType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<List<BigBrandRewardLogModel>> GetBigBrandLogListByUserIdCache(int fkBigBrandPkid,
            int periodType, Guid userId, string unionId)
        {
            List<BigBrandRewardLogModel> response = new List<BigBrandRewardLogModel>();
            using (var client =
                CacheHelper.CreateCacheClient(BigBrandLogByUser + fkBigBrandPkid.ToString() + periodType +
                                              this.StartDateTime.ToString("yyyyMMddHHmmss") +
                                              this.EndDateTime.ToString("yyyyMMddHHmmss")))
            {
                if (userId != Guid.Empty)
                {
                    watcher.Restart();
                    var result = await client.GetOrSetAsync<List<BigBrandRewardLogModel>>(userId.ToString(),
                        async () => await DalBigBrand.GetBigBrandLogListByUserId(fkBigBrandPkid, this.StartDateTime,
                            this.EndDateTime, userId, unionId), TimeSpan.FromDays(30));
                    if (result.Success)
                    {
                        response = result?.Value;
                        watcher.Stop();
                        Logger.Info($"BigBrandLogic GetBigBrandLogListByUserIdCache UserId Redis {(watcher.ElapsedMilliseconds > 1000 ? "1" : "0")} times:{watcher.ElapsedMilliseconds}");
                    }
                    else
                    {
                        watcher.Restart();
                        response = await DalBigBrand.GetBigBrandLogListByUserId(fkBigBrandPkid, this.StartDateTime, this.EndDateTime, userId, unionId);
                        watcher.Stop();
                        Logger.Info($"BigBrandLogic GetBigBrandLogListByUserIdCache Database {(watcher.ElapsedMilliseconds > 1000 ? "1" : "0")} times:{watcher.ElapsedMilliseconds}");
                    }

                }
                else if (!string.IsNullOrEmpty(unionId))
                {
                    watcher.Restart();
                    var result = await client.GetOrSetAsync<List<BigBrandRewardLogModel>>(unionId,
                        async () => await DalBigBrand.GetBigBrandLogListByUserId(fkBigBrandPkid, this.StartDateTime,
                            this.EndDateTime, userId, unionId), TimeSpan.FromDays(30));
                    if (result.Success)
                    {
                        response = result?.Value;
                        watcher.Stop();
                        Logger.Info($"BigBrandLogic GetBigBrandLogListByUserIdCache UnionId Redis {(watcher.ElapsedMilliseconds > 1000 ? "1" : "0")} times:{watcher.ElapsedMilliseconds}");
                    }
                    else
                    {
                        watcher.Restart();
                        response = await DalBigBrand.GetBigBrandLogListByUserId(fkBigBrandPkid, this.StartDateTime, this.EndDateTime, userId, unionId);
                        watcher.Stop();
                        Logger.Info($"BigBrandLogic GetBigBrandLogListByUserIdCache Database {(watcher.ElapsedMilliseconds > 1000 ? "1" : "0")} times:{watcher.ElapsedMilliseconds}");
                    }

                }
                //if (response == null || response?.Count <= 0)
                //{
                //    watcher.Restart();
                //    response =  await DalBigBrand.GetBigBrandLogListByUserId(fkBigBrandPkid, this.StartDateTime, this.EndDateTime, userId, unionId);
                //    watcher.Stop();
                //    Logger.Info($"BigBrandLogic GetBigBrandLogListByUserIdCache GetCountFromDatabase times:{watcher.ElapsedMilliseconds}");
                //}
            }
            return response;
        }

        /// <summary>
        /// 添加分享日志
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddShareLogWithChanceType(int chanceType = 2)
        {
            var listLoger =
                await GetBigBrandLogListByUserIdCache(this.entity.PKID, this.entity.PeriodType, this.UserId,
                    this.UnionId);
            if (listLoger?.Where(_ => _.ChanceType == chanceType)?.Count() == 0)
            {
                for (int i = 0; i < this.entity.CompletedTimes; i++)
                {
                    var model = new BigBrandRewardLogModel()
                    {
                        ChanceType = chanceType,
                        Channel = this.Channel,
                        CreateDateTime = DateTime.Now,
                        DeviceSerialNumber = this.DeviceId,
                        FKPKID = 0,
                        Phone = this.Phone,
                        Refer = this.Refer,
                        Status = true,
                        UserId = this.UserId,
                        UnionId = this.UnionId,
                        FKBigBrandPkid = this.entity.PKID
                    };
                    await DalBigBrand.AddBigBrandLog(model);
                    string key = BigBrandLogByUser + this.entity.PKID.ToString() + this.entity.PeriodType.ToString() + this.StartDateTime.ToString("yyyyMMddHHmmss") + this.EndDateTime.ToString("yyyyMMddHHmmss");
                    using (var client = CacheHelper.CreateCacheClient(key))
                    {
                        listLoger.Add(model);
                        if (this.UserId != Guid.Empty)
                            await client.SetAsync(this.UserId.ToString(), listLoger, TimeSpan.FromDays(30));
                        else if (!string.IsNullOrEmpty(this.UnionId))
                            await client.SetAsync(this.UnionId, listLoger, TimeSpan.FromDays(30));

                    }

                }

                // DistributedCache.Upsert("BigBrandShareCount", this.ShareCount + 1);
                // 这个计数查出来 暂时没有用到 ，而且是慢查询，暂时注释掉 俊桥说是一个功能做了一半结果没有做了,之后可以用计数器来实现
                //using (var redisClient = Tuhu.Nosql.CacheHelper.CreateCacheClient("BigBrandLogic"))
                //{
                //    var result = await redisClient.SetAsync("BigBrandShareCount",DalBigBrand.GetBigBrandShareCount(entity.PKID));//增加分享次数
                //}
                return true;
            }
            else
                return false;
        }

        public async Task<List<BigBrandRewardLogModel>> GetShareLogList(int chanceType)
        {
            var listLoger =
                await GetBigBrandLogListByUserIdCache(this.entity.PKID, this.entity.PeriodType, this.UserId,
                    this.UnionId);
            return listLoger.Where(x => x.ChanceType == chanceType).ToList();
        }

        public async Task<bool> AddShareLog(int times, int chanceType = 2)
        {
            var isExist = DalBigBrand.IsExistShareLog(this.UserId, this.entity.HashKeyValue, this.Channel);
            if (isExist)
                return false;

            for (int i = 0; i < times; i++)
            {
                var model = new BigBrandRewardLogModel()
                {
                    ChanceType = chanceType,
                    Channel = this.Channel,
                    CreateDateTime = DateTime.Now,
                    DeviceSerialNumber = this.DeviceId,
                    FKPKID = 0,
                    Phone = this.Phone,
                    Refer = this.Refer,
                    Status = true,
                    UserId = this.UserId,
                    UnionId = this.UnionId,
                    FKBigBrandPkid = this.entity.PKID
                };
                await DalBigBrand.AddBigBrandLog(model);
                string key = BigBrandLogByUser + this.entity.PKID.ToString() + this.entity.PeriodType.ToString() + this.StartDateTime.ToString("yyyyMMddHHmmss") + this.EndDateTime.ToString("yyyyMMddHHmmss");

                using (var client = CacheHelper.CreateCacheClient(key))
                {
                    if (this.UserId != Guid.Empty)
                    {
                        await client.RemoveAsync(this.UserId.ToString());
                    }
                    else if (!string.IsNullOrEmpty(this.UnionId))
                    {
                        await client.RemoveAsync(this.UnionId);
                    }

                }
            }
            return true;
        }

        /// <summary>
        /// 发放优惠积分，
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async Task<OperationResult<BigBrandRewardPoolModel>> CreatePageage(int pkid)
        {
            var page = new BigBrandRewardPoolModel();

            foreach (var item in this.entity.ItemPools)
            {
                if (item.PartItem?.Where(_ => _.PKID == pkid)?.Count() > 0)
                    page = item.PartItem?.Where(_ => _.PKID == pkid)?.FirstOrDefault();
            }
            if (this.entity.BigBrandType == 2)
            {
                //积分抽奖
                using (var client = new Tuhu.Service.Member.UserIntegralClient())
                {
                    ///规则guid
                    var integralRuleId = new Guid("9A40D2E0-3085-40F1-9CCF-A5AC2A0ABAD2");
                    Service.Member.Models.UserIntegralDetailModel integralDetailModel =
                        new Service.Member.Models.UserIntegralDetailModel();
                    integralDetailModel.TransactionIntegral = this.entity.NeedIntegral.Value;
                    integralDetailModel.TransactionChannel = "H5";
                    integralDetailModel.Versions = "1.0.0";
                    integralDetailModel.TransactionRemark = "大翻盘积分抽奖";
                    integralDetailModel.IntegralRuleID = integralRuleId;

                    var invokeResult =
                        await client.UserIntegralChangeByUserIDAsync(this.UserId, integralDetailModel, null, 0); //先扣积分
                    invokeResult.ThrowIfException(true);
                    if (invokeResult.Success)
                    {
                        var response = (await CreatePackageInfo(page));
                        return response.Success
                            ? OperationResult.FromResult(page)
                            : OperationResult.FromError<BigBrandRewardPoolModel>(response.ErrorCode,
                                response.ErrorMessage);
                    }

                    else
                        return null;
                }
            }
            else
            {
                var response = (await CreatePackageInfo(page));
                return response.Success
                    ? OperationResult.FromResult(page)
                    : OperationResult.FromError<BigBrandRewardPoolModel>(response.ErrorCode,
                        response.ErrorMessage);
            }


        }

        public async Task<bool> AddPackageLog(int fkpkid, string promotionCodePKIDs)
        {
            var model = new BigBrandRewardLogModel()
            {
                ChanceType = 1,
                Channel = this.Channel,
                CreateDateTime = DateTime.Now,
                DeviceSerialNumber = this.DeviceId,
                FKPKID = fkpkid,
                Phone = this.Phone,
                Refer = this.Refer,
                Status = true,
                UserId = this.UserId,
                UnionId = this.UnionId,
                FKBigBrandPkid = this.entity.PKID,
                PromotionCodePKIDs = promotionCodePKIDs
            };
            var result = await DalBigBrand.AddBigBrandLog(model);

            if (result)
            {
                string key = BigBrandCountLogKey + this.entity.PKID + this.StartDateTime.ToString("yyyyMMddHHmmss") + this.EndDateTime.ToString("yyyyMMddHHmmss");
                using (var counter = CacheHelper.CreateCounterClient(key, TimeSpan.FromDays(30)))
                {
                    counter.Increment(fkpkid.ToString());
                }

                string keyCache = BigBrandLogByUser + this.entity.PKID.ToString() + this.entity.PeriodType.ToString() + this.StartDateTime.ToString("yyyyMMddHHmmss") + this.EndDateTime.ToString("yyyyMMddHHmmss");
                using (var client = CacheHelper.CreateCacheClient(keyCache))
                {
                    if (this.UserId != Guid.Empty)
                    {
                        await client.RemoveAsync(this.UserId.ToString());
                    }
                    else if (!string.IsNullOrEmpty(this.UnionId))
                    {
                        await client.RemoveAsync(this.UnionId);
                    }

                }

            }
            return result;
        }


        public async Task<List<BigBrandPackerModel>> GetSelectLoge()
        {
            using (var client = CacheHelper.CreateCacheClient("BigBrandLogic"))
            {
                var result = await client.GetOrSetAsync($"SelectPackage{this.entity.HashKeyValue}",
                    () => DalBigBrand.GetSelectPackageDataTable(this.entity.PKID), TimeSpan.FromMinutes(5));
                return result.Value;
            }
        }

        /// <summary>
        /// 获取最近领取礼包的信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bigBrandPKID"></param>
        /// <returns></returns>
        public BigBrandRewardPoolModel GetRewardInfoLast(Guid userId)
        {
            return DalBigBrand.GetRewardInfoLast(userId, this.entity.PKID);
        }

        private async Task<OperationResult<bool>> CreatePackageInfo(BigBrandRewardPoolModel page)
        {
            List<BigBrandRealLogModel> realList = new List<BigBrandRealLogModel>();
            Guid tip = Guid.NewGuid();

            if (page.PartItem == null)
            {
                return OperationResult.FromError<bool>("-1", "奖励礼包为空");
            }

            foreach (var info in page.PartItem)
            {
                if (page.RewardType == 1)
                {
                    //发放优惠券
                    if (this.UserId != Guid.Empty)
                    {
                        #region 接新优惠券服务
                        var flag = false;
                        var cacheFlag = await ConfigServiceProxy.GetOrSetRuntimeSwitchAsync("CreatePromotionNewForBigbrand");
                        if (cacheFlag != null && cacheFlag.Result != null)
                        {
                            if (cacheFlag.Result.Value)
                                flag = true;
                        }
                        if (flag)
                        {

                            var result = await CreatePromotionServiceProxy.CreatePromotionNewAsync(new CreatePromotion.Models.CreatePromotionModel()
                            {
                                Author = this.UserId.ToString(),
                                GetRuleGUID = Guid.Parse(info.CouponGuid),
                                UserID = this.UserId,
                                Channel = this.Channel + "大翻盘",
                                DeviceID = this.DeviceId,
                                Operation = "大翻盘抽奖",
                                Referer = this.Refer,
                                IssueChannle = "幸运大翻牌",
                                IssueChannleId = this.entity.HashKeyValue,
                                Issuer = string.IsNullOrWhiteSpace(this.entity.UpdateUserName) == true ? this.entity.CreateUserName : this.entity.UpdateUserName
                            });
                            result.ThrowIfException(true);
                            if (result.Result.IsSuccess)
                                page.PromotionCodePKIDs += result.Result?.PromotionId.ToString() + ",";
                            else
                            {
                                return OperationResult.FromError<bool>(result.Result.ErrorCode.ToString(), result.Result.ErrorMessage);
                            }

                        }
                        else
                        {
                            var result = await MemberServiceProxy.CreatePromotionNewAsync(new CreatePromotionModel()
                            {
                                Author = this.UserId.ToString(),
                                GetRuleGUID = Guid.Parse(info.CouponGuid),
                                UserID = this.UserId,
                                Channel = this.Channel + "大翻盘",
                                DeviceID = this.DeviceId,
                                Operation = "大翻盘抽奖",
                                Referer = this.Refer,
                                IssueChannle = "幸运大翻牌",
                                IssueChannleId = this.entity.HashKeyValue,
                                Issuer = string.IsNullOrWhiteSpace(this.entity.UpdateUserName) == true ? this.entity.CreateUserName : this.entity.UpdateUserName
                            });
                            result.ThrowIfException(true);
                            if (result.Result.IsSuccess)
                                page.PromotionCodePKIDs += result.Result?.PromotionId.ToString() + ",";
                            else
                            {
                                return OperationResult.FromError<bool>(result.Result.ErrorCode.ToString(), result.Result.ErrorMessage);
                            }

                        }
                        #endregion
                    }
                    else if (!string.IsNullOrEmpty(this.UnionId)) //如果用户id为空,根据unionid发券
                    {
                        using (var client = new Tuhu.Service.Member.ThirdPartyPromotionClient())
                        {
                            var result = await client.InsertUserCardInfoAsync(new UserCardInfo()
                            {
                                Channel = this.Channel + "大翻盘",
                                CreatedTime = DateTime.Now,
                                DeviceID = this.DeviceId,
                                EventTime = DateTime.Now,
                                GetRuleId = Guid.Parse(info.CouponGuid),
                                IssueChannle = "幸运大翻盘",
                                UnionId = this.UnionId,
                                OpenId = this.OpenId,
                                IssueChannleId = this.entity.HashKeyValue,
                                Issuer = string.IsNullOrWhiteSpace(this.entity.UpdateUserName) == true
                                    ? this.entity.CreateUserName
                                    : this.entity.UpdateUserName,
                                UpdatedTime = DateTime.Now
                            });
                            if (!result.Success)
                            {
                                return OperationResult.FromError<bool>(result.ErrorCode.ToString(), result.ErrorMessage);
                            }
                        }
                    }

                }
                else if (page.RewardType == 2)
                {
                    //积分抽奖
                    using (var client = new Tuhu.Service.Member.UserIntegralClient())
                    {
                        ///规则guid
                        var integralRuleId = new Guid("04E70162-9588-4329-BCED-E149E22D7DCE");
                        Service.Member.Models.UserIntegralDetailModel integralDetailModel = new Service.Member.Models.UserIntegralDetailModel();
                        integralDetailModel.TransactionIntegral = info.Integral.Value;
                        integralDetailModel.TransactionChannel = "H5";
                        integralDetailModel.Versions = "1.0.0";
                        integralDetailModel.TransactionRemark = "大翻盘积分抽奖";
                        integralDetailModel.IntegralRuleID = integralRuleId;
                        var result = await client.UserIntegralChangeByUserIDAsync(this.UserId, integralDetailModel, null, 0);
                        result.ThrowIfException(true);
                    }
                }
                else if (page.RewardType == 4)
                {
                    this.RealTip = tip;
                    BigBrandRealLogModel real = new BigBrandRealLogModel()
                    {
                        CreateDateTime = DateTime.Now,
                        FKBigBrandID = this.entity.PKID,
                        FKBigBrandPoolID = page.PKID,
                        Prize = info.RealProductName,
                        Tip = tip,
                        UserId = this.UserId,
                        LastUpdateDateTime = DateTime.Now
                    };
                    realList.Add(real);
                }
                else if (page.RewardType == 5)//抽中的是微信红包，给用户发微信红包
                {
                    //using (var client = new Pay.PayClient())
                    //{
                    //    var sendResponse = await client.Wx_SendRedBagAsync(new Pay.Models.WxSendRedBagRequest()
                    //    {
                    //        OpenId = this.OpenId,
                    //        Channel = this.Channel + "大翻盘",
                    //        ActName = "大翻盘抽奖",
                    //        Remark = this.entity.HashKeyValue,
                    //        Wishing = "新年大吉",
                    //        Money = (info.WxRedBagAmount ?? 0) * 100
                    //    });
                    //    sendResponse.ThrowIfException(true);
                    //}
                    //发红包改为发消息队列，从消息队列里慢慢发
                    TuhuNotification.SendNotification("notification.WxSendRedBag", new Pay.Models.WxSendRedBagRequest()
                    {
                        OpenId = this.OpenId,
                        Channel = this.Channel + "大翻盘",
                        ActName = "大翻盘抽奖",
                        Remark = this.entity.HashKeyValue,
                        Wishing = "新年大吉",
                        Money = (info.WxRedBagAmount ?? 0) * 100
                    });
                }
                else
                {
                    //空奖
                }
            }

            if (page.RewardType == 4)
            {
                //实物奖励
                DalBigBrand.AddBigBrandRealLog(realList);
            }
            if (this.entity.BigBrandType == 1)
                this.TimeCount -= 1;
            if (this.entity.BigBrandType == 3)
            {
                this.TimeCount -= 1;
                await ActivityManager.UpdateLuckyWheelUserlotteryCountAsync(this.UserId, Guid.Empty,
                    this.entity.HashKeyValue);
            }
            return OperationResult.FromResult(true);
        }

        /// <summary>
        /// 根据礼包
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CouponRule>> GetCouponRuleItems(int pkid, DateTime logDateTime)
        {
            List<Guid> guids = new List<Guid>();
            foreach (var item in this.entity.ItemPools)
            {
                if (item?.PartItem?.Where(_ => _.PKID == pkid)?.Count() > 0)
                {
                    foreach (var coupon in item?.PartItem?.Where(_ => _.PKID == pkid)?.FirstOrDefault()?.PartItem)
                        if (!string.IsNullOrWhiteSpace(coupon.CouponGuid))
                            guids.Add(Guid.Parse(coupon.CouponGuid));

                    break;
                }
            }
            if (guids.Count > 0)
            {
                using (var client = new Tuhu.Service.Member.PromotionClient())
                {
                    var result = await client.GetCouponRulesAsync(guids);
                    result.ThrowIfException(true);
                    List<CouponRule> list = new List<CouponRule>();
                    foreach (var _ in result?.Result)
                    {
                        var model = new CouponRule()
                        {
                            DeadLineDate = _.DeadLineDate,
                            Description = _.Description,
                            Discount = _.Discount,
                            MinMoney = _.MinMoney,
                            PromotionName = _.PromotionName,
                            Term = _.Term,
                            ValiEndDate = _.ValiEndDate,
                            ValiStartDate = _.ValiStartDate,
                            CreateDateTime = logDateTime,
                        };
                        model.DateNumber = model.GetDateNumber();
                        #region 设置优惠券使用状态
                        if (this.PromotionCodes != null && this.PromotionCodes?.Length > 0)
                        {
                            using (var promotionClient = new Tuhu.Service.Member.PromotionClient())
                            {
                                var resultPromotion = await promotionClient.GetPromotionCodeForUserCenterAsync(new GetPromotionCodeRequest()
                                {
                                    UserID = this.UserId,
                                    Type = PromotionStatus.AlreadyUsed
                                }, new Tuhu.Models.PagerModel() { PageSize = 50 });
                                if (resultPromotion.Success && resultPromotion?.Result?.Where(p => p.Pkid == Convert.ToInt64(this.PromotionCodes[0].Split(',')[0]))?.Count() > 0)
                                    model.DateNumber = -2;

                            }
                        }
                        #endregion
                        list.Add(model);
                    }

                    return list;
                }
            }
            else
                return null;

        }

        /// <summary>
        /// 更新实物奖励地址
        /// </summary>
        /// <param name="address"></param>
        /// <param name="tip"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBigBrandRealLog(string address, Guid tip, string userName, string phone)
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;
            return await DalBigBrand.UpdateBigBrandRealLog(address, this.UserId, tip, userName, phone);
        }

        /// <summary>
        /// 查询没有登记地址的实物奖励
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BigBrandRealLogModel>> IsNULLBigBrandRealByAddress()
        {
            return await DalBigBrand.IsNULLBigBrandRealByAddress(this.UserId, this.entity.PKID);
        }
    }
}
