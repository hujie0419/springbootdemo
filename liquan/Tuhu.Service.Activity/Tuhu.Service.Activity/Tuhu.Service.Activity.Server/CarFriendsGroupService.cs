using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Manager;
using static Tuhu.Service.Activity.Models.Response.CarFriendsGroupInfoResponse;

namespace Tuhu.Service.Activity.Server
{
    public class CarFriendsGroupService: ICarFriendsGroupService
    {
        /// <summary>
        /// 获取车友群列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupListAsync(GetCarFriendsGroupListRequest request)
        {
            return OperationResult.FromResult(await CarFriendsGroupManager.GetCarFriendsGroupListAsync(request));
        }

        /// <summary>
        /// 获取所有热门车型
        /// </summary>  
        /// <returns></returns>
        public async Task<OperationResult<List<RecommendVehicleResponse>>> GetRecommendVehicleListAsync()
        {
            return OperationResult.FromResult(await CarFriendsGroupManager.GetRecommendVehicleListAsync());
        }

        /// <summary>
        /// 根据pkid获取车友群
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupModelAsync(int pkid)
        {
            return await CarFriendsGroupManager.GetCarFriendsGroupModelAsync(pkid);
        }

        /// <summary>
        /// 获取途虎管理员信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async Task<OperationResult<CarFriendsAdministratorsResponse>> GetCarFriendsAdministratorsModelAsync(int pkid)
        {
            return await CarFriendsGroupManager.GetCarFriendsAdministratorsModelAsync(pkid);
        }

        /// <summary>
        /// 车友群小程序推送车友群或群主信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> CarFriendsGroupPushInfoAsync(GetCarFriendsGroupPushInfoRequest request)
        {
            return await CarFriendsGroupManager.CarFriendsGroupPushInfoAsync(request);
        }

        /// <summary>
        /// 调用MQ延迟推送
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> CarFriendsGroupMqDelayPushAsync(GetCarFriendsGroupPushInfoRequest request)
        {
            return  await CarFriendsGroupManager.CarFriendsGroupMqDelayPush(request);
        }
    }
}
