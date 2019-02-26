using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.MessageQueue;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.DataAccess.OARedEnvelope;
using Tuhu.Service.Activity.Server.Model;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Pay;
using Tuhu.Service.Pay.Models;

namespace Tuhu.Service.Activity.Server.Manager
{
    /// <summary>
    ///     公众号领红包 - 用户领取逻辑红包
    /// </summary>
    internal class OARedEnvelopeUserReceiveManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OARedEnvelopeUserReceiveManager));

        private static readonly RabbitMQProducer MqProducer =
            RabbitMQClient.DefaultClient.CreateProducer("direct.defaultExchage");

        private readonly DateTime _now;
        private readonly OARedEnvelopeBuilderModel _oaRedEnvelopeBuilderModel;
        private readonly OARedEnvelopeCacheManager _oaRedEnvelopeCacheManager;


        public OARedEnvelopeUserReceiveManager(OARedEnvelopeBuilderModel oARedEnvelopeBuilderModel
            , OARedEnvelopeCacheManager oaRedEnvelopeCacheManager
            , DateTime now)
        {
            _oaRedEnvelopeBuilderModel = oARedEnvelopeBuilderModel;
            _oaRedEnvelopeCacheManager = oaRedEnvelopeCacheManager;
            _now = now;
        }

        /// <summary>
        ///     再验证一下
        /// </summary>
        /// <returns></returns>
        internal async Task<Tuple<bool, string, string>> ValidateAsync()
        {
            //  获取用户是否在缓存里面
            var exists =
                await _oaRedEnvelopeCacheManager.GetUserOARedEnvelopeObjectExistsAsync(
                    _oaRedEnvelopeBuilderModel.UserId);
            if (exists)
            {
                return Tuple.Create(false, "-70", Resource.OARedEnvelope_UserAlreadRecive);
            }

            var incr = await _oaRedEnvelopeCacheManager.IncrementNumAsync(0);
            // 如果已经获得到了、那么验证当前红包是否存在、就不用+1了。
            if (!_oaRedEnvelopeBuilderModel.IsTicketGet)
            {
                incr = incr + 1;
            }

            var position = incr;

            var oaRedEnvelopeObjectModel = await _oaRedEnvelopeCacheManager.GetOARedEnvelopeObjectAsync(position);

            if (oaRedEnvelopeObjectModel == null)
            {
                //  没有红包了
                return Tuple.Create(false, "-60", "");
            }

            return Tuple.Create(true, string.Empty, string.Empty);
        }

        /// <summary>
        ///     读取用户数据 有缓存优先缓存
        /// </summary>
        /// <returns></returns>
        private static async Task<OperationResult<UserObjectModel>> FetchUserByUserIdByCacheAsync(Guid userId)
        {
            using (var userClient = new UserClient())
            {
                //获取用户数据
                var userInfo = await userClient.FetchUserByUserIdAsync(userId.ToString());
                return userInfo;
            }
        }

        /// <summary>
        ///     锁定一个红包
        /// </summary>
        public async Task<Tuple<bool, string, string>> LockRedEnvelopAsync()
        {
            try
            {
                // 红包数量 下标从1开始  原子
                var count = await _oaRedEnvelopeCacheManager.IncrementNumAsync(1);

                // 已经获取Ticket
                _oaRedEnvelopeBuilderModel.TicketGet();

                // 获取一个实际红包对象
                var redEnvelop = await _oaRedEnvelopeCacheManager.GetOARedEnvelopeObjectAsync(count);

                if (redEnvelop == null)
                {
                    return Tuple.Create(false, "-60", Resource.OARedEnvelope_RedEnvelopeNone);
                }

                //  已经获取到红包
                _oaRedEnvelopeBuilderModel.RedEnvelopeGet(redEnvelop.Money);

                return Tuple.Create(true, string.Empty, string.Empty);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeUserReceiveManager -> LockRedEnvelopAsync -> {JsonConvert.SerializeObject(_oaRedEnvelopeBuilderModel)}",
                    e.InnerException ?? e);
                throw;
            }
        }


        /// <summary>
        ///     发红包
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<bool, string, string>> SendRedEnvelopAsync()
        {
            try
            {
                // 发红包之前再验证下
                var validateResult = await ValidateAsync();
                if (!validateResult.Item1)
                {
                    return validateResult;
                }

                // 怼到redis里面
                var cacheResult =
                    await _oaRedEnvelopeCacheManager.SaveUserOARedEnvelopeObjectAsync(_oaRedEnvelopeBuilderModel);

                // 判断是否异步
                using (var configClient = new Service.Config.ConfigClient())
                {
                    // 判断开关是否打开
                    var asyncFlag = configClient.GetOrSetRuntimeSwitch("OAEnvelopeAsyscFlag")?.Result?.Value ?? false;
                    // 判断是走同步还是异步
                    if (asyncFlag)
                    {
                        // 发送MQ
                        if (cacheResult)
                        {
                            // 成功
                            // 发送到MQ中
                            MqProducer.Send("notification.OAEnvelopeReciveConsumer", _oaRedEnvelopeBuilderModel, 1000);

                            return await Task.FromResult(Tuple.Create(true, string.Empty, string.Empty));
                        }
                    }
                    else
                    {
                        return await CallbackRedEnvelopAsync();
                    }
                }
                // 失败
                return Tuple.Create(false, "-3", "");
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeUserReceiveManager -> SendRedEnvelopAsync -> {JsonConvert.SerializeObject(_oaRedEnvelopeBuilderModel)}",
                    e.InnerException ?? e);
                throw;
            }
        }


        /// <summary>
        ///     发红包回调
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<bool, string, string>> CallbackRedEnvelopAsync()
        {
            try
            {
                // 领取记录增加到数据库中。
                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    // 获取user
                    var user = await FetchUserByUserIdByCacheAsync(_oaRedEnvelopeBuilderModel.UserId);

                    // 添加到明细表
                    var result = await DalOARedEnvelopeDetail.InsertOARedEnvelopeDetailAsync(dbHelper,
                        new OARedEnvelopeDetailModel
                        {
                            GetMoney = _oaRedEnvelopeBuilderModel.Money,
                            OfficialAccountType = _oaRedEnvelopeBuilderModel.OfficialAccountType,
                            OpenId = _oaRedEnvelopeBuilderModel.OpenId,
                            UserId = _oaRedEnvelopeBuilderModel.UserId,
                            ReferrerUserId = _oaRedEnvelopeBuilderModel.ReferrerUserId,
                            NickName = _oaRedEnvelopeBuilderModel.WXNickName,
                            WXHeadImgUrl = _oaRedEnvelopeBuilderModel.WXHeadPicUrl,
                            GetDate = _oaRedEnvelopeBuilderModel.RequestTime.Date,
                        });
                    if (result > 0)
                    {
                        // 设置数据库插入成功
                        _oaRedEnvelopeBuilderModel.DbDetailGet(result);

                        using (var configClient = new Service.Config.ConfigClient())
                        {
                            // 判断开关是否打开
                            var payServiceFlag = configClient.GetOrSetRuntimeSwitch("OAEnvelopeUsePayService")?.Result?.Value ?? false;
                            if (payServiceFlag)
                            {
                                // 调用PAY 实际给用户发钱
                                using (var payClient = new PayClient())
                                {
                                    var payResult = await payClient.Wx_SendRedBagAsync(new WxSendRedBagRequest
                                    {
                                        ActName = "关注公众号领红包",
                                        Channel = "关注公众号领红包",
                                        // 这边 *100 是因为 支付系统是不支持小数的
                                        Money = (int)(_oaRedEnvelopeBuilderModel.Money * 100),
                                        OpenId = _oaRedEnvelopeBuilderModel.OpenId,
                                        Remark = "关注公众号领红包奖励"
                                    });

                                    payResult.ThrowIfException();
                                    if (!payResult.Success || !payResult.Result)
                                    {
                                        // 判断异常类型 --- 发送提醒

                                        Logger.Warn(
                                            $"OARedEnvelopeUserReceiveManager -> CallbackRedEnvelopAsync -> Wx_SendRedBagAsync -> false -> {JsonConvert.SerializeObject(_oaRedEnvelopeBuilderModel)} {payResult.ErrorMessage} {payResult.ErrorCode} ");
                                        return Tuple.Create(false, "-10", "");
                                    }
                                }
                            }
                        }
                    }

                    // 写日志
                    Logger.Info(
                        $"OARedEnvelopeUserReceiveManager -> CallbackRedEnvelopAsync -> finish -> {JsonConvert.SerializeObject(_oaRedEnvelopeBuilderModel)}");


                    // 实时统计
                    await _oaRedEnvelopeCacheManager.StatisticsDoingAsync(1, _oaRedEnvelopeBuilderModel.Money);
                }

                return await Task.FromResult(Tuple.Create(true, string.Empty, string.Empty));
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeUserReceiveManager -> CallbackRedEnvelopAsync -> {JsonConvert.SerializeObject(_oaRedEnvelopeBuilderModel)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     回滚
        /// </summary>
        public async Task RollBackAsync()
        {
            try
            {
                var taskList = new List<Task>();
                // 判断是否在缓存中领取过Ticket
                if (_oaRedEnvelopeBuilderModel.IsTicketGet)
                {
                    // 干掉
                    taskList.Add(_oaRedEnvelopeCacheManager.IncrementNumAsync(-1));
                }

                // 判断是否在缓存中领取过红包
                if (_oaRedEnvelopeBuilderModel.IsRedEnvelopeGet)
                {
                    // TODO
                }

                // 判断是否插入到了数据库中
                if (_oaRedEnvelopeBuilderModel.IsDbDetailGet)
                {
                    // 干掉
                    taskList.Add(
                        DalOARedEnvelopeDetail.DisableOARedEnvelopeDetailAsync(_oaRedEnvelopeBuilderModel.DetailId));
                }

                // 删除用户缓存记录
                taskList.Add(_oaRedEnvelopeCacheManager.RemoveUserOARedEnvelopeObjectAsync(_oaRedEnvelopeBuilderModel.UserId));

                await Task.WhenAll(taskList);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeUserReceiveManager -> RollBackAsync -> {JsonConvert.SerializeObject(_oaRedEnvelopeBuilderModel)}",
                    e.InnerException ?? e);
                throw;
            }
        }
    }
}
