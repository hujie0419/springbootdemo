using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.ThirdParty;
using Tuhu.Service.ThirdParty.Models.Reqests;
using static Tuhu.Service.Activity.Models.Response.CarFriendsGroupInfoResponse;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class CarFriendsGroupManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CarFriendsGroupManager));

        /// <summary>
        /// 获取车友群列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<CarFriendsGroupInfoResponse> GetCarFriendsGroupListAsync(GetCarFriendsGroupListRequest request)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.CarFriendsGroupName))
            {
                var result = new CarFriendsGroupInfoResponse();
                //筛选车型得到的车型群
                if (request.VehicleList != null && request.VehicleList.Count > 0)
                {
                    string key = string.Format(GlobalConstant.CarFriendsGroupKey, string.Join(":", request.VehicleList));
                    var clientResult = await client.GetOrSetAsync(SecurityHelper.Hash(key), async () => (await DalCarFriendsGroup.GetFilterCarFriendsGroupListAsync(request.VehicleList)), TimeSpan.FromMinutes(10));
                    if (clientResult.Success && clientResult.Value != null)
                    {
                        result = clientResult.Value;
                    }
                }
                else
                {
                    //热门车友群/热门推荐群/搜索车型得到的车型群/全部车友群
                    string key = string.Format(GlobalConstant.CarFriendsGroupKey, ":" + request.IsRecommend + ":" + request.SearchVehicleKey);
                    var clientResult = await client.GetOrSetAsync(SecurityHelper.Hash(key), async () => (await GetSearchCarFriendsGroupResponseAsync(request.IsRecommend, request.SearchVehicleKey)), TimeSpan.FromMinutes(10));
                    if (clientResult.Success && clientResult.Value != null && clientResult.Value.groupCount > 0)
                    {
                        result = clientResult.Value;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 获取热门车友群/热门推荐群/搜索车型得到的车型群/全部车友群
        /// </summary>
        /// <param name="isRecommend"></param>
        /// <param name="searchVehicleKey"></param>
        /// <returns></returns>
        private static async Task<CarFriendsGroupInfoResponse> GetSearchCarFriendsGroupResponseAsync(bool isRecommend, string searchVehicleKey)
        {
            var result = new CarFriendsGroupInfoResponse();
            var carFriendsGroupInfoResponse = await DalCarFriendsGroup.GetIsRecommendCarFriendsGroupListAsync(isRecommend);
            if (string.IsNullOrWhiteSpace(searchVehicleKey))
            {
                result = carFriendsGroupInfoResponse;
            }
            else
            {
                result.groupList = carFriendsGroupInfoResponse.groupList.Where(t => t.BindVehicleType.Contains(searchVehicleKey)).ToList();
                result.groupCount = result.groupList.Count;
            }
            return result;
        }

        /// <summary>
        ///获取所有热门车型 
        /// </summary>
        /// <returns></returns>
        public static async Task<List<RecommendVehicleResponse>> GetRecommendVehicleListAsync()
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.CarFriendsGroupName))
            {
                var result = new List<RecommendVehicleResponse>();
                string key = string.Format(GlobalConstant.CarFriendsGroupKey, "RecommendVehicle");
                var clientResult = await client.GetOrSetAsync(key, async () => (await DalCarFriendsGroup.GetRecommendVehicleListAsync()), TimeSpan.FromMinutes(10));
                if (clientResult.Success && clientResult.Value != null)
                {
                    result = clientResult.Value;
                }
                return result;
            }
        }

        /// <summary>
        /// 根据pkid获取车友群
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupModelAsync(int pkid)
        {
            if (pkid == 0)
                return OperationResult.FromError<CarFriendsGroupInfoResponse>("-31", string.Format(Resource.ParameterError_NotZero, "pkid"));
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.CarFriendsGroupName))
            {
                var result = new CarFriendsGroupInfoResponse();
                string key = string.Format(GlobalConstant.CarFriendsGroupKey, pkid);
                var clientResult = await client.GetOrSetAsync(key, async () => (await DalCarFriendsGroup.GetCarFriendsGroupModelAsync(pkid)), TimeSpan.FromMinutes(10));
                if (clientResult.Success && clientResult.Value != null && clientResult.Value.groupCount > 0)
                {
                    result = clientResult.Value;
                }
                return OperationResult.FromResult(result);
            }
        }

        /// <summary>
        /// 获取群主信息
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<CarFriendsAdministratorsResponse>> GetCarFriendsAdministratorsModelAsync(int pkid)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.CarFriendsGroupName))
            {
                var result = new CarFriendsAdministratorsResponse();
                var clientResult = await client.GetOrSetAsync(string.Format(GlobalConstant.CarFriendsAdministratorByPkidKey, pkid), async () => {
                    if (pkid == 0)
                    {
                        return await DalCarFriendsGroup.GetCarFriendsAdministratorsModelAsync();
                    }
                    else
                    {

                        return await DalCarFriendsGroup.GetCarFriendsAdministratorsModelByPkidAsync(pkid);
                 }

                }, TimeSpan.FromMinutes(10));
                if (clientResult.Success && clientResult.Value != null)
                {
                    result = clientResult.Value;
                }
                return OperationResult.FromResult(result);
            }
        }

        /// <summary>
        /// 车友群小程序推送车友群或群主信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> CarFriendsGroupPushInfoAsync(GetCarFriendsGroupPushInfoRequest request)
        {
            using(var client=new WeiXinServiceClient())
            {
                var consumerServiceRequest = new ConsumerServiceRequest();
                consumerServiceRequest.ToUserName = "gh_8bdcae9770d5";
                consumerServiceRequest.FromUserName = request.OpenId;
                consumerServiceRequest.MsgType = "link";
                consumerServiceRequest.Content = "";
                consumerServiceRequest.Event = "event";
               
                if (request.InfoType == 0)
                {
                    //车友群
                    var carFriendsGroupModel = new CarFriendsGroup();
                    var carFriendsResult = await GetCarFriendsGroupModelAsync(request.PKID);
                    if(carFriendsResult.Success&& carFriendsResult.Result != null&& carFriendsResult.Result.groupList !=null && carFriendsResult.Result.groupList.Count > 0)
                    {
                        carFriendsGroupModel = carFriendsResult.Result.groupList[0];
                        consumerServiceRequest.Description = "点击加入";
                        consumerServiceRequest.Title = carFriendsGroupModel.GroupName;
                        consumerServiceRequest.Thumb_url = carFriendsGroupModel.GroupHeadPortrait;
                        //线下测试
                        //consumerServiceRequest.Url = "https://wxdev.tuhu.work/vue/vueTest/pages/home/index?_project=NaCarFriendsGroup&pkid=" + request.PKID;
                        //ut测试
                        //consumerServiceRequest.Url = "https://wxut.tuhu.cn/vue/vueTest/pages/home/index?_project=NaCarFriendsGroup&pkid=" + request.PKID;
                        //线上地址
                        consumerServiceRequest.Url = "https://wx.tuhu.cn/vue/NaCarFriendsGroup/pages/home/index?pkid=" + request.PKID;
                    }
                    else
                    {
                        return OperationResult.FromResult(false);
                    }
                }
                else if (request.InfoType == 1)
                {
                    //群主
                    var administratorsModel = new CarFriendsAdministratorsResponse();
                    var administratorsResult=await GetCarFriendsAdministratorsModelAsync(request.PKID);
                    if(administratorsResult.Success&& administratorsResult.Result != null)
                    {
                        administratorsModel = administratorsResult.Result;
                    }
                    consumerServiceRequest.Description = "邀您入群";
                    consumerServiceRequest.Title = "欢迎加入途虎专属群";
                    consumerServiceRequest.Thumb_url = administratorsModel.WeChatHeadPortrait;
                    //线下测试
                    //consumerServiceRequest.Url = "https://wxdev.tuhu.work/vue/vueTest/pages/home/index?_project=NaCarFriendsGroup&infoType=1";
                    //ut测试
                    //consumerServiceRequest.Url = "https://wxut.tuhu.cn/vue/vueTest/pages/home/index?_project=NaCarFriendsGroup&infoType=1";
                    //线上地址
                    consumerServiceRequest.Url = "https://wx.tuhu.cn/vue/NaCarFriendsGroup/pages/home/index?infoType=1";
                }
                using (var redisClient = CacheHelper.CreateCounterClient(GlobalConstant.CarFriendsGroupName,TimeSpan.FromMinutes(1)))
                {
                    string key = string.Format(GlobalConstant.CarFriendsGroupConsumerServiceKey, ":" + request.OpenId + ":" + request.UserId + ":" + request.InfoType + ":" + request.PKID+ ":" + request.RandomNumberKey);
                    var cunsumerResult = await redisClient.IncrementAsync(key);
                    if (cunsumerResult.Success && cunsumerResult.Value == 1) {
                        //车友群小程序客服消息推送
                        var result = client.WechatAppCustomerServiceMessagePush(consumerServiceRequest);
                        result.ThrowIfException(true);
                        if (result.Success && result.Result)
                        {

                        }
                        else
                        {
                            await redisClient.DecrementAsync(key, 1);
                        }
                        Logger.Info(key);
                    }
                }
                return OperationResult.FromResult(true);
            }
        }

        /// <summary>
        /// 调用MQ延迟推送
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> CarFriendsGroupMqDelayPush(GetCarFriendsGroupPushInfoRequest request)
        {
            await Task.Yield();
            if (request.InfoType==0&&request.PKID == 0)
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_NotZero, "PKID"));
            if (string.IsNullOrWhiteSpace(request.OpenId))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_NotNull, "OpenId"));
            if ((request.InfoType != 0 && request.InfoType != 1) || request.UserId == Guid.Empty)
                return OperationResult.FromError<bool>("-31", Resource.ParameterError);
            try
            {
                var data = new
                {
                    PKID = request.PKID,
                    InfoType = request.InfoType,
                    UserId = request.UserId,
                    OpenId = request.OpenId,
                    RandomNumberKey = Guid.NewGuid()
                };
                    
                TuhuNotification.SendNotification("notification.CarFriendsGroupPushInfoQueue", data, 3000);
                TuhuNotification.SendNotification("notification.CarFriendsGroupPushInfoQueue", data, 10000);
                return OperationResult.FromResult(true);
            }
            catch (Exception e)
            {
                Logger.Error($"车友群推送调用MQ失败 -> {JsonConvert.SerializeObject(request)}", e);
                throw;
            }
        }
    }
}
