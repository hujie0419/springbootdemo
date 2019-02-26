
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Data;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using CreateOrderRequest= Tuhu.Service.Order.Request.CreateOrderRequest;
using CreateOrderResult = Tuhu.Service.Activity.Models.CreateOrderResult;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Models.Activity;
using Tuhu.Service.Activity.Models.Requests.Activity;
using Tuhu.Service.Activity.Models.WashCarCoupon;
//不要在Activity.generated.cs文件里加任何代码，此文件内容为自动生成。需要加接口请在Activity.tt或Activity.cs中添加
namespace Tuhu.Service.Activity
{
	///<summary>大翻牌</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IBigBrandService
    {
    	///<summary>获取大翻牌数据信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetBigBrand", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetBigBrandResponse")]
        Task<OperationResult<BigBrandRewardListModel>> GetBigBrandAsync(string keyValue);
		///<summary>更新大翻牌数据信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrand", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandResponse")]
        Task<OperationResult<bool>> UpdateBigBrandAsync(string keyValue);
		///<summary>获取抽奖结果</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetPacketResponse")]
        Task<OperationResult<BigBrandResponse>> GetPacketAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string));
		///<summary>分享追加抽奖次数</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddOne", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddOneResponse")]
        Task<OperationResult<bool>> ShareAddOneAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string),int chanceType=2);
		///<summary>抽奖播报</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectCanPacker", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectCanPackerResponse")]
        Task<OperationResult<BigBrandCanResponse>> SelectCanPackerAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string));
		///<summary>获取大翻盘抽奖信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectPack", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectPackResponse")]
        Task<OperationResult<string>> SelectPackAsync(string hashKey);
		///<summary>更新实物奖励地址</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandRealLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandRealLogResponse")]
        Task<OperationResult<BigBrandRealResponse>> UpdateBigBrandRealLogAsync(string hashKey,Guid userId,string address, Guid tip,string phone,string deviceId,string Channal,string userName);
		///<summary>查询实物奖励的更新地址是否有未填写</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/IsNULLBigBrandRealByAddress", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/IsNULLBigBrandRealByAddressResponse")]
        Task<OperationResult<IEnumerable<BigBrandRealLogModel>>> IsNULLBigBrandRealByAddressAsync(string hashKey,Guid userId,string phone,string deviceId,string Channal);
		///<summary>订单状态增加抽奖次数</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddByOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddByOrderResponse")]
        Task<OperationResult<bool>> ShareAddByOrderAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times);
		///<summary>给大翻盘添加抽奖次数</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/AddBigBrandTimes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/AddBigBrandTimesResponse")]
        Task<OperationResult<bool>> AddBigBrandTimesAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times);
		///<summary>问答抽奖返回结果</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetAnswerPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetAnswerPacketResponse")]
        Task<OperationResult<BigBrandResponse>> GetAnswerPacketAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer);
		///<summary>问答设置问题答案</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SetAnswerRes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SetAnswerResResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Models.Response.QuestionAnsResponse>> SetAnswerResAsync(Tuhu.Service.Activity.Models.Requests.QuestionAnsRequestModel request);
		///<summary>获取问题列表</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetQuestionList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetQuestionListResponse")]
        Task<OperationResult<List<Tuhu.Service.Activity.Models.BigBrandQuesList>>> GetQuestionListAsync(Guid userId, string hashKey );
		///<summary>刷新问答题库</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateQuestionInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateQuestionInfoListResponse")]
        Task<OperationResult<bool>> UpdateQuestionInfoListAsync();
		///<summary>获取分享红包组列表</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetFightGroupsPacketsList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetFightGroupsPacketsListResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> GetFightGroupsPacketsListAsync(Guid? fightGroupsIdentity,Guid userId);
		///<summary>新增分享红包组</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/InsertFightGroupsPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/InsertFightGroupsPacketResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> InsertFightGroupsPacketAsync(Guid userId);
		///<summary>更新分享组红包中的用户</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateFightGroupsPacketByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateFightGroupsPacketByUserIdResponse")]
        Task<OperationResult<bool>> UpdateFightGroupsPacketByUserIdAsync(Tuhu.Service.Activity.Models.Requests.FightGroupsPacketsUpdateRequest request);
		///<summary>发放优惠券红包</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/CreateFightGroupsPacketByPromotion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/CreateFightGroupsPacketByPromotionResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupsPacketsProvideResponse>> CreateFightGroupsPacketByPromotionAsync(Guid fightGroupsIdentity);
		///<summary>获取错题分享的日志列表 chanceType:1.中奖纪录，2.分享纪录，3.答题答错分享纪录</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectShareList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectShareListResponse")]
        Task<OperationResult<List<BigBrandRewardLogModel>>> SelectShareListAsync(Guid userId,string hashKey,int chanceType);
	}

	///<summary>大翻牌</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IBigBrandClient : IBigBrandService, ITuhuServiceClient
    {
    	///<summary>获取大翻牌数据信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetBigBrand", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetBigBrandResponse")]
        OperationResult<BigBrandRewardListModel> GetBigBrand(string keyValue);
		///<summary>更新大翻牌数据信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrand", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandResponse")]
        OperationResult<bool> UpdateBigBrand(string keyValue);
		///<summary>获取抽奖结果</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetPacketResponse")]
        OperationResult<BigBrandResponse> GetPacket(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string));
		///<summary>分享追加抽奖次数</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddOne", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddOneResponse")]
        OperationResult<bool> ShareAddOne(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string),int chanceType=2);
		///<summary>抽奖播报</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectCanPacker", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectCanPackerResponse")]
        OperationResult<BigBrandCanResponse> SelectCanPacker(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string));
		///<summary>获取大翻盘抽奖信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectPack", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectPackResponse")]
        OperationResult<string> SelectPack(string hashKey);
		///<summary>更新实物奖励地址</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandRealLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandRealLogResponse")]
        OperationResult<BigBrandRealResponse> UpdateBigBrandRealLog(string hashKey,Guid userId,string address, Guid tip,string phone,string deviceId,string Channal,string userName);
		///<summary>查询实物奖励的更新地址是否有未填写</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/IsNULLBigBrandRealByAddress", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/IsNULLBigBrandRealByAddressResponse")]
        OperationResult<IEnumerable<BigBrandRealLogModel>> IsNULLBigBrandRealByAddress(string hashKey,Guid userId,string phone,string deviceId,string Channal);
		///<summary>订单状态增加抽奖次数</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddByOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddByOrderResponse")]
        OperationResult<bool> ShareAddByOrder(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times);
		///<summary>给大翻盘添加抽奖次数</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/AddBigBrandTimes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/AddBigBrandTimesResponse")]
        OperationResult<bool> AddBigBrandTimes(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times);
		///<summary>问答抽奖返回结果</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetAnswerPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetAnswerPacketResponse")]
        OperationResult<BigBrandResponse> GetAnswerPacket(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer);
		///<summary>问答设置问题答案</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SetAnswerRes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SetAnswerResResponse")]
        OperationResult<Tuhu.Service.Activity.Models.Response.QuestionAnsResponse> SetAnswerRes(Tuhu.Service.Activity.Models.Requests.QuestionAnsRequestModel request);
		///<summary>获取问题列表</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetQuestionList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetQuestionListResponse")]
        OperationResult<List<Tuhu.Service.Activity.Models.BigBrandQuesList>> GetQuestionList(Guid userId, string hashKey );
		///<summary>刷新问答题库</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateQuestionInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateQuestionInfoListResponse")]
        OperationResult<bool> UpdateQuestionInfoList();
		///<summary>获取分享红包组列表</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetFightGroupsPacketsList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetFightGroupsPacketsListResponse")]
        OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> GetFightGroupsPacketsList(Guid? fightGroupsIdentity,Guid userId);
		///<summary>新增分享红包组</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/InsertFightGroupsPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/InsertFightGroupsPacketResponse")]
        OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> InsertFightGroupsPacket(Guid userId);
		///<summary>更新分享组红包中的用户</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateFightGroupsPacketByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateFightGroupsPacketByUserIdResponse")]
        OperationResult<bool> UpdateFightGroupsPacketByUserId(Tuhu.Service.Activity.Models.Requests.FightGroupsPacketsUpdateRequest request);
		///<summary>发放优惠券红包</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/CreateFightGroupsPacketByPromotion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/CreateFightGroupsPacketByPromotionResponse")]
        OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupsPacketsProvideResponse> CreateFightGroupsPacketByPromotion(Guid fightGroupsIdentity);
		///<summary>获取错题分享的日志列表 chanceType:1.中奖纪录，2.分享纪录，3.答题答错分享纪录</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectShareList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectShareListResponse")]
        OperationResult<List<BigBrandRewardLogModel>> SelectShareList(Guid userId,string hashKey,int chanceType);
	}

	///<summary>大翻牌</summary>
	public partial class BigBrandClient : TuhuServiceClient<IBigBrandClient>, IBigBrandClient
    {
    	///<summary>获取大翻牌数据信息</summary>///<returns></returns>
        public OperationResult<BigBrandRewardListModel> GetBigBrand(string keyValue) => Invoke(_ => _.GetBigBrand(keyValue));

	///<summary>获取大翻牌数据信息</summary>///<returns></returns>
        public Task<OperationResult<BigBrandRewardListModel>> GetBigBrandAsync(string keyValue) => InvokeAsync(_ => _.GetBigBrandAsync(keyValue));
		///<summary>更新大翻牌数据信息</summary>///<returns></returns>
        public OperationResult<bool> UpdateBigBrand(string keyValue) => Invoke(_ => _.UpdateBigBrand(keyValue));

	///<summary>更新大翻牌数据信息</summary>///<returns></returns>
        public Task<OperationResult<bool>> UpdateBigBrandAsync(string keyValue) => InvokeAsync(_ => _.UpdateBigBrandAsync(keyValue));
		///<summary>获取抽奖结果</summary>///<returns></returns>
        public OperationResult<BigBrandResponse> GetPacket(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string)) => Invoke(_ => _.GetPacket(userId,deviceId,Channal,hashKey,phone,refer,openId));

	///<summary>获取抽奖结果</summary>///<returns></returns>
        public Task<OperationResult<BigBrandResponse>> GetPacketAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string)) => InvokeAsync(_ => _.GetPacketAsync(userId,deviceId,Channal,hashKey,phone,refer,openId));
		///<summary>分享追加抽奖次数</summary>///<returns></returns>
        public OperationResult<bool> ShareAddOne(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string),int chanceType=2) => Invoke(_ => _.ShareAddOne(userId,deviceId,Channal,hashKey,phone,refer,openId,chanceType));

	///<summary>分享追加抽奖次数</summary>///<returns></returns>
        public Task<OperationResult<bool>> ShareAddOneAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string),int chanceType=2) => InvokeAsync(_ => _.ShareAddOneAsync(userId,deviceId,Channal,hashKey,phone,refer,openId,chanceType));
		///<summary>抽奖播报</summary>///<returns></returns>
        public OperationResult<BigBrandCanResponse> SelectCanPacker(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string)) => Invoke(_ => _.SelectCanPacker(userId,deviceId,Channal,hashKey,phone,refer,openId));

	///<summary>抽奖播报</summary>///<returns></returns>
        public Task<OperationResult<BigBrandCanResponse>> SelectCanPackerAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string)) => InvokeAsync(_ => _.SelectCanPackerAsync(userId,deviceId,Channal,hashKey,phone,refer,openId));
		///<summary>获取大翻盘抽奖信息</summary>///<returns></returns>
        public OperationResult<string> SelectPack(string hashKey) => Invoke(_ => _.SelectPack(hashKey));

	///<summary>获取大翻盘抽奖信息</summary>///<returns></returns>
        public Task<OperationResult<string>> SelectPackAsync(string hashKey) => InvokeAsync(_ => _.SelectPackAsync(hashKey));
		///<summary>更新实物奖励地址</summary>///<returns></returns>
        public OperationResult<BigBrandRealResponse> UpdateBigBrandRealLog(string hashKey,Guid userId,string address, Guid tip,string phone,string deviceId,string Channal,string userName) => Invoke(_ => _.UpdateBigBrandRealLog(hashKey,userId,address,tip,phone,deviceId,Channal,userName));

	///<summary>更新实物奖励地址</summary>///<returns></returns>
        public Task<OperationResult<BigBrandRealResponse>> UpdateBigBrandRealLogAsync(string hashKey,Guid userId,string address, Guid tip,string phone,string deviceId,string Channal,string userName) => InvokeAsync(_ => _.UpdateBigBrandRealLogAsync(hashKey,userId,address,tip,phone,deviceId,Channal,userName));
		///<summary>查询实物奖励的更新地址是否有未填写</summary>///<returns></returns>
        public OperationResult<IEnumerable<BigBrandRealLogModel>> IsNULLBigBrandRealByAddress(string hashKey,Guid userId,string phone,string deviceId,string Channal) => Invoke(_ => _.IsNULLBigBrandRealByAddress(hashKey,userId,phone,deviceId,Channal));

	///<summary>查询实物奖励的更新地址是否有未填写</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<BigBrandRealLogModel>>> IsNULLBigBrandRealByAddressAsync(string hashKey,Guid userId,string phone,string deviceId,string Channal) => InvokeAsync(_ => _.IsNULLBigBrandRealByAddressAsync(hashKey,userId,phone,deviceId,Channal));
		///<summary>订单状态增加抽奖次数</summary>///<returns></returns>
        public OperationResult<bool> ShareAddByOrder(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times) => Invoke(_ => _.ShareAddByOrder(userId,deviceId,Channal,hashKey,phone,refer,times));

	///<summary>订单状态增加抽奖次数</summary>///<returns></returns>
        public Task<OperationResult<bool>> ShareAddByOrderAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times) => InvokeAsync(_ => _.ShareAddByOrderAsync(userId,deviceId,Channal,hashKey,phone,refer,times));
		///<summary>给大翻盘添加抽奖次数</summary>///<returns></returns>
        public OperationResult<bool> AddBigBrandTimes(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times) => Invoke(_ => _.AddBigBrandTimes(userId,deviceId,Channal,hashKey,phone,refer,times));

	///<summary>给大翻盘添加抽奖次数</summary>///<returns></returns>
        public Task<OperationResult<bool>> AddBigBrandTimesAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times) => InvokeAsync(_ => _.AddBigBrandTimesAsync(userId,deviceId,Channal,hashKey,phone,refer,times));
		///<summary>问答抽奖返回结果</summary>///<returns></returns>
        public OperationResult<BigBrandResponse> GetAnswerPacket(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer) => Invoke(_ => _.GetAnswerPacket(userId,deviceId,Channal,hashKey,phone,refer));

	///<summary>问答抽奖返回结果</summary>///<returns></returns>
        public Task<OperationResult<BigBrandResponse>> GetAnswerPacketAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer) => InvokeAsync(_ => _.GetAnswerPacketAsync(userId,deviceId,Channal,hashKey,phone,refer));
		///<summary>问答设置问题答案</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Models.Response.QuestionAnsResponse> SetAnswerRes(Tuhu.Service.Activity.Models.Requests.QuestionAnsRequestModel request) => Invoke(_ => _.SetAnswerRes(request));

	///<summary>问答设置问题答案</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Models.Response.QuestionAnsResponse>> SetAnswerResAsync(Tuhu.Service.Activity.Models.Requests.QuestionAnsRequestModel request) => InvokeAsync(_ => _.SetAnswerResAsync(request));
		///<summary>获取问题列表</summary>///<returns></returns>
        public OperationResult<List<Tuhu.Service.Activity.Models.BigBrandQuesList>> GetQuestionList(Guid userId, string hashKey ) => Invoke(_ => _.GetQuestionList(userId,hashKey));

	///<summary>获取问题列表</summary>///<returns></returns>
        public Task<OperationResult<List<Tuhu.Service.Activity.Models.BigBrandQuesList>>> GetQuestionListAsync(Guid userId, string hashKey ) => InvokeAsync(_ => _.GetQuestionListAsync(userId,hashKey));
		///<summary>刷新问答题库</summary>///<returns></returns>
        public OperationResult<bool> UpdateQuestionInfoList() => Invoke(_ => _.UpdateQuestionInfoList());

	///<summary>刷新问答题库</summary>///<returns></returns>
        public Task<OperationResult<bool>> UpdateQuestionInfoListAsync() => InvokeAsync(_ => _.UpdateQuestionInfoListAsync());
		///<summary>获取分享红包组列表</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> GetFightGroupsPacketsList(Guid? fightGroupsIdentity,Guid userId) => Invoke(_ => _.GetFightGroupsPacketsList(fightGroupsIdentity,userId));

	///<summary>获取分享红包组列表</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> GetFightGroupsPacketsListAsync(Guid? fightGroupsIdentity,Guid userId) => InvokeAsync(_ => _.GetFightGroupsPacketsListAsync(fightGroupsIdentity,userId));
		///<summary>新增分享红包组</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> InsertFightGroupsPacket(Guid userId) => Invoke(_ => _.InsertFightGroupsPacket(userId));

	///<summary>新增分享红包组</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> InsertFightGroupsPacketAsync(Guid userId) => InvokeAsync(_ => _.InsertFightGroupsPacketAsync(userId));
		///<summary>更新分享组红包中的用户</summary>///<returns></returns>
        public OperationResult<bool> UpdateFightGroupsPacketByUserId(Tuhu.Service.Activity.Models.Requests.FightGroupsPacketsUpdateRequest request) => Invoke(_ => _.UpdateFightGroupsPacketByUserId(request));

	///<summary>更新分享组红包中的用户</summary>///<returns></returns>
        public Task<OperationResult<bool>> UpdateFightGroupsPacketByUserIdAsync(Tuhu.Service.Activity.Models.Requests.FightGroupsPacketsUpdateRequest request) => InvokeAsync(_ => _.UpdateFightGroupsPacketByUserIdAsync(request));
		///<summary>发放优惠券红包</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupsPacketsProvideResponse> CreateFightGroupsPacketByPromotion(Guid fightGroupsIdentity) => Invoke(_ => _.CreateFightGroupsPacketByPromotion(fightGroupsIdentity));

	///<summary>发放优惠券红包</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupsPacketsProvideResponse>> CreateFightGroupsPacketByPromotionAsync(Guid fightGroupsIdentity) => InvokeAsync(_ => _.CreateFightGroupsPacketByPromotionAsync(fightGroupsIdentity));
		///<summary>获取错题分享的日志列表 chanceType:1.中奖纪录，2.分享纪录，3.答题答错分享纪录</summary>///<returns></returns>
        public OperationResult<List<BigBrandRewardLogModel>> SelectShareList(Guid userId,string hashKey,int chanceType) => Invoke(_ => _.SelectShareList(userId,hashKey,chanceType));

	///<summary>获取错题分享的日志列表 chanceType:1.中奖纪录，2.分享纪录，3.答题答错分享纪录</summary>///<returns></returns>
        public Task<OperationResult<List<BigBrandRewardLogModel>>> SelectShareListAsync(Guid userId,string hashKey,int chanceType) => InvokeAsync(_ => _.SelectShareListAsync(userId,hashKey,chanceType));
	}
	/// <summary>限时抢购</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleService
    {
    	/// <summary>更新限时抢购内容到缓存</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityIDResponse")]
        Task<OperationResult<bool>> UpdateFlashSaleDataToCouchBaseByActivityIDAsync(Guid activityID);
		/// <summary>天天秒杀数据列表缓存刷新</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SpikeListRefresh", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SpikeListRefreshResponse")]
        Task<OperationResult<bool>> SpikeListRefreshAsync(Guid activityId);
		/// <summary>根据活动ID查询活动详情</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIDResponse")]
        Task<OperationResult<FlashSaleModel>> SelectFlashSaleDataByActivityIDAsync(Guid activityID);
		/// <summary>根据活动ID,Pid查询活动详情与定义商品</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIdsResponse")]
        Task<OperationResult<List<FlashSaleModel>>> SelectFlashSaleDataByActivityIdsAsync(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests);
		/// <summary>新活动页查询活动信息接口</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleDataActivityPageByIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleDataActivityPageByIdsResponse")]
        Task<OperationResult<List<FlashSaleActivityPageModel>>> GetFlashSaleDataActivityPageByIdsAsync(List<FlashSaleActivityPageRequest> flashSaleActivityPageRequest);
		/// <summary>获取限时抢购缓存的内容</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleListResponse")]
        Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleListAsync(Guid[] activityIDs);
		/// <summary>查今天天天秒杀当天数据</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayDataResponse")]
        Task<OperationResult<IEnumerable<FlashSaleModel>>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate=null,bool needProducts=true,bool excludeProductTags = false);
		/// <summary>根据时间范围查询秒杀场次信息</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSecondsBoys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSecondsBoysResponse")]
        Task<OperationResult<IEnumerable<FlashSaleModel>>> GetSecondsBoysAsync(int activityType,DateTime? startDate=null,DateTime? endDate=null);
		/// <summary>活动页秒杀查询最新场次</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetActivePageSecondKill", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetActivePageSecondKillResponse")]
        Task<OperationResult<IEnumerable<FlashSaleModel>>> GetActivePageSecondKillAsync(int topNumber,bool isProducts=true);
		/// <summary>订单取消，减掉限时抢购的数量</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DeleteFlashSaleRecords", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DeleteFlashSaleRecordsResponse")]
        Task<OperationResult<int>> DeleteFlashSaleRecordsAsync(int orderId);
		/// <summary>获取天天秒杀用户提醒信息</summary>/// <returns>null</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserReminderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserReminderInfoResponse")]
        Task<OperationResult<UserReminderInfo>> GetUserReminderInfoAsync(EveryDaySeckillUserInfo model);
		/// <summary>插入天天秒杀用户信息</summary>/// <returns>null</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/InsertEveryDaySeckillUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/InsertEveryDaySeckillUserInfoResponse")]
        Task<OperationResult<InsertEveryDaySeckillUserInfoResponse>> InsertEveryDaySeckillUserInfoAsync(EveryDaySeckillUserInfo model);
		/// <summary>校验限时抢购产品是否被限购</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckFlashSaleProductBuyLimit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckFlashSaleProductBuyLimitResponse")]
        Task<OperationResult<IEnumerable<FlashSaleProductBuyLimitModel>>> CheckFlashSaleProductBuyLimitAsync(CheckFlashSaleProductRequest request);
		/// <summary>获取用户还可以购买此活动产品几件</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCanBuyFlashSaleItemCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCanBuyFlashSaleItemCountResponse")]
        Task<OperationResult<FlashSaleProductCanBuyCountModel>> GetUserCanBuyFlashSaleItemCountAsync(Guid userId,Guid activityId,string pid);
		/// <summary>校验是否可购买这个限时抢购订单</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckCanBuyFlashSaleOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckCanBuyFlashSaleOrderResponse")]
        Task<OperationResult<FlashSaleOrderResponse>> CheckCanBuyFlashSaleOrderAsync(FlashSaleOrderRequest request);
		/// <summary>/// 查询产品详情页限时抢购详情/// </summary>///  <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailForFlashSale", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailForFlashSaleResponse")]
        Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailForFlashSaleAsync(string productId, string variantId, string activityId,string channel, string userId,string productGroupId=null,int buyQty=1);
		///<summary>减少计数器</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DecrementCounter", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DecrementCounterResponse")]
        Task<OperationResult<bool>> DecrementCounterAsync(int orderId);
		///<summary>取消订单时维护数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCancerMaintenanceFlashSaleData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCancerMaintenanceFlashSaleDataResponse")]
        Task<OperationResult<bool>> OrderCancerMaintenanceFlashSaleDataAsync(int orderId);
		///<summary>刷新限购商品计数器缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshFlashSaleHashCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshFlashSaleHashCountResponse")]
        Task<OperationResult<bool>> RefreshFlashSaleHashCountAsync(List<string> activtyids ,bool isAllRefresh);
		///<summary>只查询活动配置信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleWithoutProductsList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleWithoutProductsListResponse")]
        Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleWithoutProductsListAsync(List<Guid> activtyids);
		///<summary>查询详情页打折信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailDisountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailDisountInfoResponse")]
        Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailDisountInfoAsync(DiscountActivtyProductRequest request);
		///<summary>查询用户下单是维护的限购计数器</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCreateFlashOrderCountCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCreateFlashOrderCountCacheResponse")]
        Task<OperationResult<OrderCountResponse>> GetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request);
		///<summary>设置用户下单是维护的限购计数器</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SetUserCreateFlashOrderCountCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SetUserCreateFlashOrderCountCacheResponse")]
        Task<OperationResult<OrderCountResponse>> SetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request);
		///<summary>获取缓存里数据不准确的数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleWrongCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleWrongCacheResponse")]
        Task<OperationResult<List<FlashSaleWrongCacheResponse>>> SelectFlashSaleWrongCacheAsync();
		///<summary>用户创建订单时候维护销售数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCreateMaintenanceFlashSaleDbData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCreateMaintenanceFlashSaleDbDataResponse")]
        Task<OperationResult<bool>> OrderCreateMaintenanceFlashSaleDbDataAsync(FlashSaleOrderRequest flashSale);
		///<summary>从日志表里更新销量到配置表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateConfigSaleoutQuantityFromLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateConfigSaleoutQuantityFromLogResponse")]
        Task<OperationResult<bool>> UpdateConfigSaleoutQuantityFromLogAsync(UpdateConfigSaleoutQuantityRequest request);
		///<summary>按照场次刷新秒杀默认数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshSeckillDefaultDataBySchedule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshSeckillDefaultDataByScheduleResponse")]
        Task<OperationResult<bool>> RefreshSeckillDefaultDataByScheduleAsync(string schedule);
		///<summary>获取传入时间段内的秒杀活动</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillScheduleInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillScheduleInfoResponse")]
        Task<OperationResult<Dictionary<string, List<SeckilScheduleInfoRespnose>>>> GetSeckillScheduleInfoAsync(List<string> pids, DateTime sSchedule, DateTime eSchedule);
		///<summary>根据活动产品获取该活动产品的库存信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillAvailableStockResponse", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillAvailableStockResponseResponse")]
        Task<OperationResult<List<SeckillAvailableStockInfoResponse>>> GetSeckillAvailableStockResponseAsync(List<SeckillAvailableStockInfoRequest> request);
		///<summary>查询首页天天秒杀数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectHomeSeckillData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectHomeSeckillDataResponse")]
        Task<OperationResult<List<FlashSaleModel>>> SelectHomeSeckillDataAsync(SelectHomeSecKillRequest request);
		///<summary>根据场次Id获取天天秒杀产品数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSeckillDataByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSeckillDataByActivityIdResponse")]
        Task<OperationResult<List<FlashSaleProductModel>>> SelectSeckillDataByActivityIdAsync(SelectSecKillByIdRequest request);
	}

	/// <summary>限时抢购</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleClient : IFlashSaleService, ITuhuServiceClient
    {
    	/// <summary>更新限时抢购内容到缓存</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityIDResponse")]
        OperationResult<bool> UpdateFlashSaleDataToCouchBaseByActivityID(Guid activityID);
		/// <summary>天天秒杀数据列表缓存刷新</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SpikeListRefresh", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SpikeListRefreshResponse")]
        OperationResult<bool> SpikeListRefresh(Guid activityId);
		/// <summary>根据活动ID查询活动详情</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIDResponse")]
        OperationResult<FlashSaleModel> SelectFlashSaleDataByActivityID(Guid activityID);
		/// <summary>根据活动ID,Pid查询活动详情与定义商品</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIdsResponse")]
        OperationResult<List<FlashSaleModel>> SelectFlashSaleDataByActivityIds(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests);
		/// <summary>新活动页查询活动信息接口</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleDataActivityPageByIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleDataActivityPageByIdsResponse")]
        OperationResult<List<FlashSaleActivityPageModel>> GetFlashSaleDataActivityPageByIds(List<FlashSaleActivityPageRequest> flashSaleActivityPageRequest);
		/// <summary>获取限时抢购缓存的内容</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleListResponse")]
        OperationResult<List<FlashSaleModel>> GetFlashSaleList(Guid[] activityIDs);
		/// <summary>查今天天天秒杀当天数据</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayDataResponse")]
        OperationResult<IEnumerable<FlashSaleModel>> SelectSecondKillTodayData(int activityType, DateTime? scheduleDate=null,bool needProducts=true,bool excludeProductTags = false);
		/// <summary>根据时间范围查询秒杀场次信息</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSecondsBoys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSecondsBoysResponse")]
        OperationResult<IEnumerable<FlashSaleModel>> GetSecondsBoys(int activityType,DateTime? startDate=null,DateTime? endDate=null);
		/// <summary>活动页秒杀查询最新场次</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetActivePageSecondKill", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetActivePageSecondKillResponse")]
        OperationResult<IEnumerable<FlashSaleModel>> GetActivePageSecondKill(int topNumber,bool isProducts=true);
		/// <summary>订单取消，减掉限时抢购的数量</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DeleteFlashSaleRecords", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DeleteFlashSaleRecordsResponse")]
        OperationResult<int> DeleteFlashSaleRecords(int orderId);
		/// <summary>获取天天秒杀用户提醒信息</summary>/// <returns>null</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserReminderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserReminderInfoResponse")]
        OperationResult<UserReminderInfo> GetUserReminderInfo(EveryDaySeckillUserInfo model);
		/// <summary>插入天天秒杀用户信息</summary>/// <returns>null</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/InsertEveryDaySeckillUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/InsertEveryDaySeckillUserInfoResponse")]
        OperationResult<InsertEveryDaySeckillUserInfoResponse> InsertEveryDaySeckillUserInfo(EveryDaySeckillUserInfo model);
		/// <summary>校验限时抢购产品是否被限购</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckFlashSaleProductBuyLimit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckFlashSaleProductBuyLimitResponse")]
        OperationResult<IEnumerable<FlashSaleProductBuyLimitModel>> CheckFlashSaleProductBuyLimit(CheckFlashSaleProductRequest request);
		/// <summary>获取用户还可以购买此活动产品几件</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCanBuyFlashSaleItemCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCanBuyFlashSaleItemCountResponse")]
        OperationResult<FlashSaleProductCanBuyCountModel> GetUserCanBuyFlashSaleItemCount(Guid userId,Guid activityId,string pid);
		/// <summary>校验是否可购买这个限时抢购订单</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckCanBuyFlashSaleOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckCanBuyFlashSaleOrderResponse")]
        OperationResult<FlashSaleOrderResponse> CheckCanBuyFlashSaleOrder(FlashSaleOrderRequest request);
		/// <summary>/// 查询产品详情页限时抢购详情/// </summary>///  <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailForFlashSale", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailForFlashSaleResponse")]
        OperationResult<FlashSaleProductDetailModel> FetchProductDetailForFlashSale(string productId, string variantId, string activityId,string channel, string userId,string productGroupId=null,int buyQty=1);
		///<summary>减少计数器</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DecrementCounter", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DecrementCounterResponse")]
        OperationResult<bool> DecrementCounter(int orderId);
		///<summary>取消订单时维护数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCancerMaintenanceFlashSaleData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCancerMaintenanceFlashSaleDataResponse")]
        OperationResult<bool> OrderCancerMaintenanceFlashSaleData(int orderId);
		///<summary>刷新限购商品计数器缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshFlashSaleHashCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshFlashSaleHashCountResponse")]
        OperationResult<bool> RefreshFlashSaleHashCount(List<string> activtyids ,bool isAllRefresh);
		///<summary>只查询活动配置信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleWithoutProductsList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleWithoutProductsListResponse")]
        OperationResult<List<FlashSaleModel>> GetFlashSaleWithoutProductsList(List<Guid> activtyids);
		///<summary>查询详情页打折信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailDisountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailDisountInfoResponse")]
        OperationResult<FlashSaleProductDetailModel> FetchProductDetailDisountInfo(DiscountActivtyProductRequest request);
		///<summary>查询用户下单是维护的限购计数器</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCreateFlashOrderCountCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCreateFlashOrderCountCacheResponse")]
        OperationResult<OrderCountResponse> GetUserCreateFlashOrderCountCache(OrderCountCacheRequest request);
		///<summary>设置用户下单是维护的限购计数器</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SetUserCreateFlashOrderCountCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SetUserCreateFlashOrderCountCacheResponse")]
        OperationResult<OrderCountResponse> SetUserCreateFlashOrderCountCache(OrderCountCacheRequest request);
		///<summary>获取缓存里数据不准确的数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleWrongCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleWrongCacheResponse")]
        OperationResult<List<FlashSaleWrongCacheResponse>> SelectFlashSaleWrongCache();
		///<summary>用户创建订单时候维护销售数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCreateMaintenanceFlashSaleDbData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCreateMaintenanceFlashSaleDbDataResponse")]
        OperationResult<bool> OrderCreateMaintenanceFlashSaleDbData(FlashSaleOrderRequest flashSale);
		///<summary>从日志表里更新销量到配置表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateConfigSaleoutQuantityFromLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateConfigSaleoutQuantityFromLogResponse")]
        OperationResult<bool> UpdateConfigSaleoutQuantityFromLog(UpdateConfigSaleoutQuantityRequest request);
		///<summary>按照场次刷新秒杀默认数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshSeckillDefaultDataBySchedule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshSeckillDefaultDataByScheduleResponse")]
        OperationResult<bool> RefreshSeckillDefaultDataBySchedule(string schedule);
		///<summary>获取传入时间段内的秒杀活动</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillScheduleInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillScheduleInfoResponse")]
        OperationResult<Dictionary<string, List<SeckilScheduleInfoRespnose>>> GetSeckillScheduleInfo(List<string> pids, DateTime sSchedule, DateTime eSchedule);
		///<summary>根据活动产品获取该活动产品的库存信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillAvailableStockResponse", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillAvailableStockResponseResponse")]
        OperationResult<List<SeckillAvailableStockInfoResponse>> GetSeckillAvailableStockResponse(List<SeckillAvailableStockInfoRequest> request);
		///<summary>查询首页天天秒杀数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectHomeSeckillData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectHomeSeckillDataResponse")]
        OperationResult<List<FlashSaleModel>> SelectHomeSeckillData(SelectHomeSecKillRequest request);
		///<summary>根据场次Id获取天天秒杀产品数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSeckillDataByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSeckillDataByActivityIdResponse")]
        OperationResult<List<FlashSaleProductModel>> SelectSeckillDataByActivityId(SelectSecKillByIdRequest request);
	}

	/// <summary>限时抢购</summary>
	public partial class FlashSaleClient : TuhuServiceClient<IFlashSaleClient>, IFlashSaleClient
    {
    	/// <summary>更新限时抢购内容到缓存</summary>
        /// <returns></returns>
        public OperationResult<bool> UpdateFlashSaleDataToCouchBaseByActivityID(Guid activityID) => Invoke(_ => _.UpdateFlashSaleDataToCouchBaseByActivityID(activityID));

	/// <summary>更新限时抢购内容到缓存</summary>
        /// <returns></returns>
        public Task<OperationResult<bool>> UpdateFlashSaleDataToCouchBaseByActivityIDAsync(Guid activityID) => InvokeAsync(_ => _.UpdateFlashSaleDataToCouchBaseByActivityIDAsync(activityID));
		/// <summary>天天秒杀数据列表缓存刷新</summary>
        /// <returns></returns>
        public OperationResult<bool> SpikeListRefresh(Guid activityId) => Invoke(_ => _.SpikeListRefresh(activityId));

	/// <summary>天天秒杀数据列表缓存刷新</summary>
        /// <returns></returns>
        public Task<OperationResult<bool>> SpikeListRefreshAsync(Guid activityId) => InvokeAsync(_ => _.SpikeListRefreshAsync(activityId));
		/// <summary>根据活动ID查询活动详情</summary>
        /// <returns></returns>
        public OperationResult<FlashSaleModel> SelectFlashSaleDataByActivityID(Guid activityID) => Invoke(_ => _.SelectFlashSaleDataByActivityID(activityID));

	/// <summary>根据活动ID查询活动详情</summary>
        /// <returns></returns>
        public Task<OperationResult<FlashSaleModel>> SelectFlashSaleDataByActivityIDAsync(Guid activityID) => InvokeAsync(_ => _.SelectFlashSaleDataByActivityIDAsync(activityID));
		/// <summary>根据活动ID,Pid查询活动详情与定义商品</summary>
        /// <returns></returns>
        public OperationResult<List<FlashSaleModel>> SelectFlashSaleDataByActivityIds(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests) => Invoke(_ => _.SelectFlashSaleDataByActivityIds(flashSaleDataByActivityRequests));

	/// <summary>根据活动ID,Pid查询活动详情与定义商品</summary>
        /// <returns></returns>
        public Task<OperationResult<List<FlashSaleModel>>> SelectFlashSaleDataByActivityIdsAsync(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests) => InvokeAsync(_ => _.SelectFlashSaleDataByActivityIdsAsync(flashSaleDataByActivityRequests));
		/// <summary>新活动页查询活动信息接口</summary>
        /// <returns></returns>
        public OperationResult<List<FlashSaleActivityPageModel>> GetFlashSaleDataActivityPageByIds(List<FlashSaleActivityPageRequest> flashSaleActivityPageRequest) => Invoke(_ => _.GetFlashSaleDataActivityPageByIds(flashSaleActivityPageRequest));

	/// <summary>新活动页查询活动信息接口</summary>
        /// <returns></returns>
        public Task<OperationResult<List<FlashSaleActivityPageModel>>> GetFlashSaleDataActivityPageByIdsAsync(List<FlashSaleActivityPageRequest> flashSaleActivityPageRequest) => InvokeAsync(_ => _.GetFlashSaleDataActivityPageByIdsAsync(flashSaleActivityPageRequest));
		/// <summary>获取限时抢购缓存的内容</summary>
        /// <returns></returns>
        public OperationResult<List<FlashSaleModel>> GetFlashSaleList(Guid[] activityIDs) => Invoke(_ => _.GetFlashSaleList(activityIDs));

	/// <summary>获取限时抢购缓存的内容</summary>
        /// <returns></returns>
        public Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleListAsync(Guid[] activityIDs) => InvokeAsync(_ => _.GetFlashSaleListAsync(activityIDs));
		/// <summary>查今天天天秒杀当天数据</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleModel>> SelectSecondKillTodayData(int activityType, DateTime? scheduleDate=null,bool needProducts=true,bool excludeProductTags = false) => Invoke(_ => _.SelectSecondKillTodayData(activityType,scheduleDate,needProducts,excludeProductTags));

	/// <summary>查今天天天秒杀当天数据</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleModel>>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate=null,bool needProducts=true,bool excludeProductTags = false) => InvokeAsync(_ => _.SelectSecondKillTodayDataAsync(activityType,scheduleDate,needProducts,excludeProductTags));
		/// <summary>根据时间范围查询秒杀场次信息</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleModel>> GetSecondsBoys(int activityType,DateTime? startDate=null,DateTime? endDate=null) => Invoke(_ => _.GetSecondsBoys(activityType,startDate,endDate));

	/// <summary>根据时间范围查询秒杀场次信息</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleModel>>> GetSecondsBoysAsync(int activityType,DateTime? startDate=null,DateTime? endDate=null) => InvokeAsync(_ => _.GetSecondsBoysAsync(activityType,startDate,endDate));
		/// <summary>活动页秒杀查询最新场次</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleModel>> GetActivePageSecondKill(int topNumber,bool isProducts=true) => Invoke(_ => _.GetActivePageSecondKill(topNumber,isProducts));

	/// <summary>活动页秒杀查询最新场次</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleModel>>> GetActivePageSecondKillAsync(int topNumber,bool isProducts=true) => InvokeAsync(_ => _.GetActivePageSecondKillAsync(topNumber,isProducts));
		/// <summary>订单取消，减掉限时抢购的数量</summary>
        /// <returns></returns>
        public OperationResult<int> DeleteFlashSaleRecords(int orderId) => Invoke(_ => _.DeleteFlashSaleRecords(orderId));

	/// <summary>订单取消，减掉限时抢购的数量</summary>
        /// <returns></returns>
        public Task<OperationResult<int>> DeleteFlashSaleRecordsAsync(int orderId) => InvokeAsync(_ => _.DeleteFlashSaleRecordsAsync(orderId));
		/// <summary>获取天天秒杀用户提醒信息</summary>/// <returns>null</returns>
        public OperationResult<UserReminderInfo> GetUserReminderInfo(EveryDaySeckillUserInfo model) => Invoke(_ => _.GetUserReminderInfo(model));

	/// <summary>获取天天秒杀用户提醒信息</summary>/// <returns>null</returns>
        public Task<OperationResult<UserReminderInfo>> GetUserReminderInfoAsync(EveryDaySeckillUserInfo model) => InvokeAsync(_ => _.GetUserReminderInfoAsync(model));
		/// <summary>插入天天秒杀用户信息</summary>/// <returns>null</returns>
        public OperationResult<InsertEveryDaySeckillUserInfoResponse> InsertEveryDaySeckillUserInfo(EveryDaySeckillUserInfo model) => Invoke(_ => _.InsertEveryDaySeckillUserInfo(model));

	/// <summary>插入天天秒杀用户信息</summary>/// <returns>null</returns>
        public Task<OperationResult<InsertEveryDaySeckillUserInfoResponse>> InsertEveryDaySeckillUserInfoAsync(EveryDaySeckillUserInfo model) => InvokeAsync(_ => _.InsertEveryDaySeckillUserInfoAsync(model));
		/// <summary>校验限时抢购产品是否被限购</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleProductBuyLimitModel>> CheckFlashSaleProductBuyLimit(CheckFlashSaleProductRequest request) => Invoke(_ => _.CheckFlashSaleProductBuyLimit(request));

	/// <summary>校验限时抢购产品是否被限购</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleProductBuyLimitModel>>> CheckFlashSaleProductBuyLimitAsync(CheckFlashSaleProductRequest request) => InvokeAsync(_ => _.CheckFlashSaleProductBuyLimitAsync(request));
		/// <summary>获取用户还可以购买此活动产品几件</summary>
        /// <returns></returns>
        public OperationResult<FlashSaleProductCanBuyCountModel> GetUserCanBuyFlashSaleItemCount(Guid userId,Guid activityId,string pid) => Invoke(_ => _.GetUserCanBuyFlashSaleItemCount(userId,activityId,pid));

	/// <summary>获取用户还可以购买此活动产品几件</summary>
        /// <returns></returns>
        public Task<OperationResult<FlashSaleProductCanBuyCountModel>> GetUserCanBuyFlashSaleItemCountAsync(Guid userId,Guid activityId,string pid) => InvokeAsync(_ => _.GetUserCanBuyFlashSaleItemCountAsync(userId,activityId,pid));
		/// <summary>校验是否可购买这个限时抢购订单</summary>
        /// <returns></returns>
        public OperationResult<FlashSaleOrderResponse> CheckCanBuyFlashSaleOrder(FlashSaleOrderRequest request) => Invoke(_ => _.CheckCanBuyFlashSaleOrder(request));

	/// <summary>校验是否可购买这个限时抢购订单</summary>
        /// <returns></returns>
        public Task<OperationResult<FlashSaleOrderResponse>> CheckCanBuyFlashSaleOrderAsync(FlashSaleOrderRequest request) => InvokeAsync(_ => _.CheckCanBuyFlashSaleOrderAsync(request));
		/// <summary>/// 查询产品详情页限时抢购详情/// </summary>///  <returns></returns>
        public OperationResult<FlashSaleProductDetailModel> FetchProductDetailForFlashSale(string productId, string variantId, string activityId,string channel, string userId,string productGroupId=null,int buyQty=1) => Invoke(_ => _.FetchProductDetailForFlashSale(productId,variantId,activityId,channel,userId,productGroupId,buyQty));

	/// <summary>/// 查询产品详情页限时抢购详情/// </summary>///  <returns></returns>
        public Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailForFlashSaleAsync(string productId, string variantId, string activityId,string channel, string userId,string productGroupId=null,int buyQty=1) => InvokeAsync(_ => _.FetchProductDetailForFlashSaleAsync(productId,variantId,activityId,channel,userId,productGroupId,buyQty));
		///<summary>减少计数器</summary>
        public OperationResult<bool> DecrementCounter(int orderId) => Invoke(_ => _.DecrementCounter(orderId));

	///<summary>减少计数器</summary>
        public Task<OperationResult<bool>> DecrementCounterAsync(int orderId) => InvokeAsync(_ => _.DecrementCounterAsync(orderId));
		///<summary>取消订单时维护数据</summary>
        public OperationResult<bool> OrderCancerMaintenanceFlashSaleData(int orderId) => Invoke(_ => _.OrderCancerMaintenanceFlashSaleData(orderId));

	///<summary>取消订单时维护数据</summary>
        public Task<OperationResult<bool>> OrderCancerMaintenanceFlashSaleDataAsync(int orderId) => InvokeAsync(_ => _.OrderCancerMaintenanceFlashSaleDataAsync(orderId));
		///<summary>刷新限购商品计数器缓存</summary>
        public OperationResult<bool> RefreshFlashSaleHashCount(List<string> activtyids ,bool isAllRefresh) => Invoke(_ => _.RefreshFlashSaleHashCount(activtyids,isAllRefresh));

	///<summary>刷新限购商品计数器缓存</summary>
        public Task<OperationResult<bool>> RefreshFlashSaleHashCountAsync(List<string> activtyids ,bool isAllRefresh) => InvokeAsync(_ => _.RefreshFlashSaleHashCountAsync(activtyids,isAllRefresh));
		///<summary>只查询活动配置信息</summary>
        public OperationResult<List<FlashSaleModel>> GetFlashSaleWithoutProductsList(List<Guid> activtyids) => Invoke(_ => _.GetFlashSaleWithoutProductsList(activtyids));

	///<summary>只查询活动配置信息</summary>
        public Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleWithoutProductsListAsync(List<Guid> activtyids) => InvokeAsync(_ => _.GetFlashSaleWithoutProductsListAsync(activtyids));
		///<summary>查询详情页打折信息</summary>
        public OperationResult<FlashSaleProductDetailModel> FetchProductDetailDisountInfo(DiscountActivtyProductRequest request) => Invoke(_ => _.FetchProductDetailDisountInfo(request));

	///<summary>查询详情页打折信息</summary>
        public Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailDisountInfoAsync(DiscountActivtyProductRequest request) => InvokeAsync(_ => _.FetchProductDetailDisountInfoAsync(request));
		///<summary>查询用户下单是维护的限购计数器</summary>
        public OperationResult<OrderCountResponse> GetUserCreateFlashOrderCountCache(OrderCountCacheRequest request) => Invoke(_ => _.GetUserCreateFlashOrderCountCache(request));

	///<summary>查询用户下单是维护的限购计数器</summary>
        public Task<OperationResult<OrderCountResponse>> GetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request) => InvokeAsync(_ => _.GetUserCreateFlashOrderCountCacheAsync(request));
		///<summary>设置用户下单是维护的限购计数器</summary>
        public OperationResult<OrderCountResponse> SetUserCreateFlashOrderCountCache(OrderCountCacheRequest request) => Invoke(_ => _.SetUserCreateFlashOrderCountCache(request));

	///<summary>设置用户下单是维护的限购计数器</summary>
        public Task<OperationResult<OrderCountResponse>> SetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request) => InvokeAsync(_ => _.SetUserCreateFlashOrderCountCacheAsync(request));
		///<summary>获取缓存里数据不准确的数据</summary>
        public OperationResult<List<FlashSaleWrongCacheResponse>> SelectFlashSaleWrongCache() => Invoke(_ => _.SelectFlashSaleWrongCache());

	///<summary>获取缓存里数据不准确的数据</summary>
        public Task<OperationResult<List<FlashSaleWrongCacheResponse>>> SelectFlashSaleWrongCacheAsync() => InvokeAsync(_ => _.SelectFlashSaleWrongCacheAsync());
		///<summary>用户创建订单时候维护销售数据</summary>
        public OperationResult<bool> OrderCreateMaintenanceFlashSaleDbData(FlashSaleOrderRequest flashSale) => Invoke(_ => _.OrderCreateMaintenanceFlashSaleDbData(flashSale));

	///<summary>用户创建订单时候维护销售数据</summary>
        public Task<OperationResult<bool>> OrderCreateMaintenanceFlashSaleDbDataAsync(FlashSaleOrderRequest flashSale) => InvokeAsync(_ => _.OrderCreateMaintenanceFlashSaleDbDataAsync(flashSale));
		///<summary>从日志表里更新销量到配置表</summary>
        public OperationResult<bool> UpdateConfigSaleoutQuantityFromLog(UpdateConfigSaleoutQuantityRequest request) => Invoke(_ => _.UpdateConfigSaleoutQuantityFromLog(request));

	///<summary>从日志表里更新销量到配置表</summary>
        public Task<OperationResult<bool>> UpdateConfigSaleoutQuantityFromLogAsync(UpdateConfigSaleoutQuantityRequest request) => InvokeAsync(_ => _.UpdateConfigSaleoutQuantityFromLogAsync(request));
		///<summary>按照场次刷新秒杀默认数据</summary>
        public OperationResult<bool> RefreshSeckillDefaultDataBySchedule(string schedule) => Invoke(_ => _.RefreshSeckillDefaultDataBySchedule(schedule));

	///<summary>按照场次刷新秒杀默认数据</summary>
        public Task<OperationResult<bool>> RefreshSeckillDefaultDataByScheduleAsync(string schedule) => InvokeAsync(_ => _.RefreshSeckillDefaultDataByScheduleAsync(schedule));
		///<summary>获取传入时间段内的秒杀活动</summary>
        public OperationResult<Dictionary<string, List<SeckilScheduleInfoRespnose>>> GetSeckillScheduleInfo(List<string> pids, DateTime sSchedule, DateTime eSchedule) => Invoke(_ => _.GetSeckillScheduleInfo(pids,sSchedule,eSchedule));

	///<summary>获取传入时间段内的秒杀活动</summary>
        public Task<OperationResult<Dictionary<string, List<SeckilScheduleInfoRespnose>>>> GetSeckillScheduleInfoAsync(List<string> pids, DateTime sSchedule, DateTime eSchedule) => InvokeAsync(_ => _.GetSeckillScheduleInfoAsync(pids,sSchedule,eSchedule));
		///<summary>根据活动产品获取该活动产品的库存信息</summary>
        public OperationResult<List<SeckillAvailableStockInfoResponse>> GetSeckillAvailableStockResponse(List<SeckillAvailableStockInfoRequest> request) => Invoke(_ => _.GetSeckillAvailableStockResponse(request));

	///<summary>根据活动产品获取该活动产品的库存信息</summary>
        public Task<OperationResult<List<SeckillAvailableStockInfoResponse>>> GetSeckillAvailableStockResponseAsync(List<SeckillAvailableStockInfoRequest> request) => InvokeAsync(_ => _.GetSeckillAvailableStockResponseAsync(request));
		///<summary>查询首页天天秒杀数据</summary>
        public OperationResult<List<FlashSaleModel>> SelectHomeSeckillData(SelectHomeSecKillRequest request) => Invoke(_ => _.SelectHomeSeckillData(request));

	///<summary>查询首页天天秒杀数据</summary>
        public Task<OperationResult<List<FlashSaleModel>>> SelectHomeSeckillDataAsync(SelectHomeSecKillRequest request) => InvokeAsync(_ => _.SelectHomeSeckillDataAsync(request));
		///<summary>根据场次Id获取天天秒杀产品数据</summary>
        public OperationResult<List<FlashSaleProductModel>> SelectSeckillDataByActivityId(SelectSecKillByIdRequest request) => Invoke(_ => _.SelectSeckillDataByActivityId(request));

	///<summary>根据场次Id获取天天秒杀产品数据</summary>
        public Task<OperationResult<List<FlashSaleProductModel>>> SelectSeckillDataByActivityIdAsync(SelectSecKillByIdRequest request) => InvokeAsync(_ => _.SelectSeckillDataByActivityIdAsync(request));
	}
	/// <summary>活动服务</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IActivityService
    {
    	/// <summary>查询轮胎活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityResponse")]
        Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize);
		/// <summary>查询轮胎活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityNewResponse")]
        Task<OperationResult<TireActivityModel>> SelectTireActivityNewAsync(TireActivityRequest request);
		/// <summary>查询轮胎跳转活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireChangedActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireChangedActivityResponse")]
        Task<OperationResult<string>> SelectTireChangedActivityAsync(TireActivityRequest request);
		/// <summary>查询轮胎活动的产品</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityPidsResponse")]
        Task<OperationResult<IEnumerable<string>>> SelectTireActivityPidsAsync(Guid activityId);
		/// <summary>更新轮胎活动缓存</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateTireActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateTireActivityCacheResponse")]
        Task<OperationResult<bool>> UpdateTireActivityCacheAsync(string vehicleId, string tireSize);
		/// <summary>更新轮胎活动的产品</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityPidsCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityPidsCacheResponse")]
        Task<OperationResult<bool>> UpdateActivityPidsCacheAsync(Guid activityId);
		/// <summary>车型适配轮胎</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptTires", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptTiresResponse")]
        Task<OperationResult<List<VehicleAdaptTireTireSizeDetailModel>>> SelectVehicleAaptTiresAsync(VehicleAdaptTireRequestModel request);
		///<summary>优惠券信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCarTagCouponConfigs", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCarTagCouponConfigsResponse")]
        Task<OperationResult<IEnumerable<CarTagCouponConfigModel>>> SelectCarTagCouponConfigsAsync();
		///<summary>车型适配保养</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptBaoyangs", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptBaoyangsResponse")]
        Task<OperationResult<IEnumerable<VehicleAdaptBaoyangModel>>> SelectVehicleAaptBaoyangsAsync(string vehicleId);
		///<summary>车型适配车品信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAdaptChepins", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAdaptChepinsResponse")]
        Task<OperationResult<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>>> SelectVehicleAdaptChepinsAsync(string vehicleId);
		///<summary>获取排序后的轮胎规格</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleSortedTireSizes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleSortedTireSizesResponse")]
        Task<OperationResult<IEnumerable<VehicleSortedTireSizeModel>>> SelectVehicleSortedTireSizesAsync(string vehicleId);
		///<summary> 插入用户分享信息并返回guid</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserShareInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserShareInfoResponse")]
        Task<OperationResult<Guid>> GetGuidAndInsertUserShareInfoAsync(string pid, Guid batchGuid, Guid userId);
		///<summary> 根据Guid取出写入表中的数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserShareInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserShareInfoResponse")]
        Task<OperationResult<ActivityUserShareInfoModel>> GetActivityUserShareInfoAsync(Guid shareId);
		///<summary> 根据活动ID查询用户领取次数</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPromotionPacketHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPromotionPacketHistoryResponse")]
        Task<OperationResult<IEnumerable<PromotionPacketHistoryModel>>> SelectPromotionPacketHistoryAsync(Guid userId, Guid luckyWheel);
		///<summary>根据配置表id跟用户id取出生成的新id，分享赚钱功能</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserForShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserForShareResponse")]
        Task<OperationResult<Guid>> GetGuidAndInsertUserForShareAsync(Guid configGuid, Guid userId);
		///<summary>获取配置表的一条数据，分享赚钱功能</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRecommendGetGiftConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRecommendGetGiftConfigResponse")]
        Task<OperationResult<RecommendGetGiftConfigModel>> FetchRecommendGetGiftConfigAsync(Guid? number=null,Guid? userId=null);
		///<summary>查询礼包领取</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPacketByUsers", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPacketByUsersResponse")]
        Task<OperationResult<DataTable>> SelectPacketByUsersAsync();
		///<summary>查询轮胎活动</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityByActivityIdResponse")]
        Task<OperationResult<TireActivityModel>> SelectTireActivityByActivityIdAsync(Guid activityId);
		/// <summary>获取地区活动页的url</summary>/// <returns>活动期间根据地区取得活动页的链接，否则返回是否是未开始或者过期</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionActivityPageUrl", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionActivityPageUrlResponse")]
        Task<OperationResult<RegionActivityPageModel>> GetRegionActivityPageUrlAsync(string city,string activityId);
		/// <summary>根据活动Id和地区Id或者车型Id获取目标活动地址</summary>
[Obsolete("请使用GetRegionVehicleIdActivityUrlNewAsync")]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrl", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlResponse")]
        Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlAsync(Guid activityId, int regionId, string vehicleId);
		/// <summary>根据活动Id,活动渠道和地区Id或者车型Id获取目标活动地址</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlNewResponse")]
        Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlNewAsync(Guid activityId, int regionId, string vehicleId, string activityChannel);
		/// <summary>清除缓存</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionVehicleIdActivityUrlCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionVehicleIdActivityUrlCacheResponse")]
        Task<OperationResult<bool>> RefreshRegionVehicleIdActivityUrlCacheAsync(Guid activityId);
		///<summary>落地页查询</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityConfigForDownloadApp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityConfigForDownloadAppResponse")]
        Task<OperationResult<DownloadApp>> GetActivityConfigForDownloadAppAsync(int id);
		///<summary>清除落地页数据缓存</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CleanActivityConfigForDownloadAppCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CleanActivityConfigForDownloadAppCacheResponse")]
        Task<OperationResult<bool>> CleanActivityConfigForDownloadAppCacheAsync(int id);
		///<summary>取消相同支付账户的订单 -1:失败 1:取消成功 2:无取消订单</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CancelActivityOrderOfSamePaymentAccount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CancelActivityOrderOfSamePaymentAccountResponse")]
        Task<OperationResult<int>> CancelActivityOrderOfSamePaymentAccountAsync(int orderId, string paymentAccount);
		///<summary>获取活动页数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelResponse")]
        Task<OperationResult<ActivePageListModel>> GetActivePageListModelAsync(ActivtyPageRequest request);
		///<summary>获取活动页数据-zip</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelZip", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelZipResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Zip.Models.ActivePageListModel>> GetActivePageListModelZipAsync(ActivtyPageRequest request);
		///<summary>获取大翻盘用户可翻盘数量</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckyWheelUserlotteryCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckyWheelUserlotteryCountResponse")]
        Task<OperationResult<int>> GetLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup,string hashkey=null);
		///<summary>更新大翻盘用户可翻盘数量</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateLuckyWheelUserlotteryCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateLuckyWheelUserlotteryCountResponse")]
        Task<OperationResult<int>> UpdateLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup,string hashkey=null);
		///<summary>刷新活动页数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivePageListModelCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivePageListModelCacheResponse")]
        Task<OperationResult<bool>> RefreshActivePageListModelCacheAsync(ActivtyPageRequest request);
		///<summary>刷新大翻盘数据数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshLuckWheelCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshLuckWheelCacheResponse")]
        Task<OperationResult<bool>> RefreshLuckWheelCacheAsync(string id);
		/// <summary>验证轮胎订单是否能购买</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationByTires", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationByTiresResponse")]
        Task<OperationResult<VerificationTiresResponseModel>> VerificationByTiresAsync(VerificationTiresRequestModel requestModel);
		/// <summary>增加轮胎下单记录</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertTiresOrderRecordResponse")]
        Task<OperationResult<bool>> InsertTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel);
		/// <summary>撤销轮胎下单记录</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RevokeTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RevokeTiresOrderRecordResponse")]
        Task<OperationResult<bool>> RevokeTiresOrderRecordAsync(int orderId);
		/// <summary>Redis的轮胎订单记录重建</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RedisTiresOrderRecordReconStruction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RedisTiresOrderRecordReconStructionResponse")]
        Task<OperationResult<bool>> RedisTiresOrderRecordReconStructionAsync(TiresOrderRecordRequestModel requestModel);
		/// <summary>RedisAndSql的轮胎订单记录查询</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTiresOrderRecordResponse")]
        Task<OperationResult<Dictionary<string, object>>> SelectTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel);
		/// <summary>验证轮胎优惠券是否能领取</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationTiresPromotionRule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationTiresPromotionRuleResponse")]
        Task<OperationResult<VerificationTiresResponseModel>> VerificationTiresPromotionRuleAsync(VerificationTiresRequestModel requestModel,int ruleId);
		///<summary>获取大翻盘数据数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckWheel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckWheelResponse")]
        Task<OperationResult<LuckyWheelModel>> GetLuckWheelAsync(string id);
		///<summary> 分享赚钱 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectShareActivityProductById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectShareActivityProductByIdResponse")]
        Task<OperationResult<ShareProductModel>> SelectShareActivityProductByIdAsync(string ProductId,string BatchGuid=null);
		///<summary>保养活动</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectBaoYangActivitySetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectBaoYangActivitySettingResponse")]
        Task<OperationResult<BaoYangActivitySetting>> SelectBaoYangActivitySettingAsync(string activityId);
		/// <summary></summary>///<returns></returns>
		[Obsolete("请使用SelectCouponActivityConfigNew")]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigResponse")]
        Task<OperationResult<CouponActivityConfigModel>> SelectCouponActivityConfigAsync(string activityNum, int type);
		///<summary>获取活动类型</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectActivityTypeByActivityIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectActivityTypeByActivityIdsResponse")]
        Task<OperationResult<IEnumerable<ActivityTypeModel>>> SelectActivityTypeByActivityIdsAsync(List<Guid> activityIds);
		///<summary>根据搜索词获取活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityBuildWithSelKey", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityBuildWithSelKeyResponse")]
        Task<OperationResult<ActivityBuild>> GetActivityBuildWithSelKeyAsync(string keyword);
		///<summary>记录活动类型</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityTypeLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityTypeLogResponse")]
        Task<OperationResult<bool>> RecordActivityTypeLogAsync(ActivityTypeRequest request);
		///<summary>更新保养活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangActivityConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangActivityConfigResponse")]
        Task<OperationResult<bool>> UpdateBaoYangActivityConfigAsync(Guid activityId);
		///<summary>获取保养活动状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityStatusResponse")]
        Task<OperationResult<FixedPriceActivityStatusResult>> GetFixedPriceActivityStatusAsync(Guid activityId, Guid userId, int regionId);
		///<summary>重置活动计数器</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangPurchaseCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangPurchaseCountResponse")]
        Task<OperationResult<bool>> UpdateBaoYangPurchaseCountAsync(Guid activityId);
		///<summary>根据activityId获取活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityRound", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityRoundResponse")]
        Task<OperationResult<FixedPriceActivityRoundResponse>> GetFixedPriceActivityRoundAsync(Guid activityId);
		///<summary>根据activityId和RegionId获取活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRegionTiresActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRegionTiresActivityResponse")]
        Task<OperationResult<TiresActivityResponse>> FetchRegionTiresActivityAsync(FlashSaleTiresActivityRequest request);
		///<summary>刷新轮胎活动缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionTiresActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionTiresActivityCacheResponse")]
        Task<OperationResult<bool>> RefreshRegionTiresActivityCacheAsync(Guid activityId, int regionId);
		///<summary>记录用户点击活动开始提醒</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityProductUserRemindLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityProductUserRemindLogResponse")]
        Task<OperationResult<bool>> RecordActivityProductUserRemindLogAsync(ActivityProductUserRemindRequest request);
		///<summary>添加返现申请记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordResponse")]
        Task<OperationResult<bool>> InsertRebateApplyRecordAsync(RebateApplyRequest request);
		///<summary>初始化白名单数据或者后面个别用户白名单状态调整使用</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertOrUpdateActivityPageWhiteListRecords", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertOrUpdateActivityPageWhiteListRecordsResponse")]
        Task<OperationResult<bool>> InsertOrUpdateActivityPageWhiteListRecordsAsync(List<ActivityPageWhiteListRequest> requests);
		///<summary>根据Userid判断是否是白名单用户</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageWhiteListByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageWhiteListByUserIdResponse")]
        Task<OperationResult<bool>> GetActivityPageWhiteListByUserIdAsync(Guid userId);
		///<summary>记录途虎轮胎节用户申请信息接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutUserRewardApplication", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutUserRewardApplicationResponse")]
        Task<OperationResult<UserRewardApplicationResponse>> PutUserRewardApplicationAsync(UserRewardApplicationRequest request);
		///<summary>添加返现申请记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordNewResponse")]
        Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordNewAsync(RebateApplyRequest request);
		///<summary>途虎贵就赔申请记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutApplyCompensateRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutApplyCompensateRecordResponse")]
        Task<OperationResult<bool>> PutApplyCompensateRecordAsync(ApplyCompensateRequest request);
		///<summary>批量活动有效性验证接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivtyValidityResponses", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivtyValidityResponsesResponse")]
        Task<OperationResult<List<ActivtyValidityResponse>>> GetActivtyValidityResponsesAsync(ActivtyValidityRequest request);
		///<summary>获取预付卡场次信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCardSaleConfigDetails", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCardSaleConfigDetailsResponse")]
        Task<OperationResult<List<VipCardSaleConfigDetailModel>>> GetVipCardSaleConfigDetailsAsync(string activityId);
		///<summary>check购买的批次是否还有剩余库存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VipCardCheckStock", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VipCardCheckStockResponse")]
        Task<OperationResult<Dictionary<string, bool>>> VipCardCheckStockAsync(List<string> batchIds);
		///<summary>创建订单时记录卡信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutVipCardRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutVipCardRecordResponse")]
        Task<OperationResult<bool>> PutVipCardRecordAsync(VipCardRecordRequest request);
		///<summary>支付成功时调用绑卡</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/BindVipCard", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/BindVipCardResponse")]
        Task<OperationResult<bool>> BindVipCardAsync(int orderId);
		///<summary>获取返现申请页面配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyPageConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyPageConfigResponse")]
        Task<OperationResult<RebateApplyPageConfig>> SelectRebateApplyPageConfigAsync();
		///<summary>申请返现</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordV2", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordV2Response")]
        Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordV2Async(RebateApplyRequest request);
		///<summary>获取用户所有返现申请信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyByOpenId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyByOpenIdResponse")]
        Task<OperationResult<List<RebateApplyResponse>>> SelectRebateApplyByOpenIdAsync(string openId);
		///<summary>通过OrderId获取最新的申请记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRebateApplyByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRebateApplyByOrderIdResponse")]
        Task<OperationResult<IEnumerable<RebateApplyResponse>>> GetRebateApplyByOrderIdAsync(int orderId);
		///<summary>取消订单的时候更新db跟缓存中的数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyVipCardRecordByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyVipCardRecordByOrderIdResponse")]
        Task<OperationResult<bool>> ModifyVipCardRecordByOrderIdAsync(int orderId);
		///<summary>获取2018世界杯的活动对象和积分规则信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetWorldCup2018Activity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetWorldCup2018ActivityResponse")]
        Task<OperationResult<ActivityResponse>> GetWorldCup2018ActivityAsync();
		///<summary>通过用户ID获取兑换券数量接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCouponCountByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCouponCountByUserIdResponse")]
        Task<OperationResult<int>> GetCouponCountByUserIdAsync(Guid userId, long activityId);
		///<summary>返回活动兑换券排行排名</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchCouponRank", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchCouponRankResponse")]
        Task<OperationResult<PagedModel<ActivityCouponRankResponse>>> SearchCouponRankAsync(long activityId, int pageIndex = 1, int pageSize = 20);
		///<summary>返回用户的兑换券排名情况</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponRank", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponRankResponse")]
        Task<OperationResult<ActivityCouponRankResponse>> GetUserCouponRankAsync(Guid userId, long activityId);
		///<summary>兑换物列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeListResponse")]
        Task<OperationResult<PagedModel<ActivityPrizeResponse>>> SearchPrizeListAsync(SearchPrizeListRequest searchPrizeListRequest);
		///<summary>用户兑换奖品 异常代码：-1 系统异常（请重试） , -2 兑换卡不足  -3 库存不足  -4 已经下架   -5 已经兑换   -6 兑换时间已经截止不能兑换</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UserRedeemPrizes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UserRedeemPrizesResponse")]
        Task<OperationResult<bool>> UserRedeemPrizesAsync(Guid userId, long prizeId,long activityId);
		///<summary>用户已兑换商品列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeOrderDetailListByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeOrderDetailListByUserIdResponse")]
        Task<OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>>> SearchPrizeOrderDetailListByUserIdAsync(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20);
		///<summary>今日竞猜题目</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionResponse")]
        Task<OperationResult<IEnumerable<Models.Response.Question>>> SearchQuestionAsync( Guid userId , long activityId);
		///<summary>提交用户竞猜</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionAnswerResponse")]
        Task<OperationResult<bool>> SubmitQuestionAnswerAsync(SubmitQuestionAnswerRequest submitQuestionAnswerRequest);
		///<summary>返回用户答题历史</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionAnswerHistoryByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionAnswerHistoryByUserIdResponse")]
        Task<OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>>> SearchQuestionAnswerHistoryByUserIdAsync(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest);
		///<summary>返回用户胜利次数和胜利称号</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVictoryInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVictoryInfoResponse")]
        Task<OperationResult<ActivityVictoryInfoResponse>> GetVictoryInfoAsync(Guid userId, long activityId);
		///<summary>活动分享赠送积分  异常：   -77 活动未开始  -2 今日已经分享   -1 系统异常</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityShareResponse")]
        Task<OperationResult<bool>> ActivityShareAsync(ActivityShareDetailRequest shareDetailRequest);
		///<summary>今日是否已经分享了 true = 今日已经分享</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTodayAlreadyShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTodayAlreadyShareResponse")]
        Task<OperationResult<bool>> ActivityTodayAlreadyShareAsync(Guid userId, long activityId);
		///<summary>用来获取或者刷新排序好的车型适配商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetOrSetActivityPageSortedPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetOrSetActivityPageSortedPidsResponse")]
        Task<OperationResult<List<string>>> GetOrSetActivityPageSortedPidsAsync(SortedPidsRequest request);
		///<summary>修改或者增加用户兑换券 并且增加日志  返回主键</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyActivityCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyActivityCouponResponse")]
        Task<OperationResult<long>> ModifyActivityCouponAsync(Guid userId, long activityId, int couponCount , string couponName,DateTime? modifyDateTime = null);
		///<summary>刷新活动题目  缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityQuestionCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityQuestionCacheResponse")]
        Task<OperationResult<bool>> RefreshActivityQuestionCacheAsync(long activityId);
		///<summary>刷新活动兑换物  缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityPrizeCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityPrizeCacheResponse")]
        Task<OperationResult<bool>> RefreshActivityPrizeCacheAsync(long activityId);
		///<summary>保存用户答题数据到数据库</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswerResponse")]
        Task<OperationResult<SubmitActivityQuestionUserAnswerResponse>> SubmitQuestionUserAnswerAsync(SubmitActivityQuestionUserAnswerRequest request);
		///<summary>更新用户答题结果状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResultResponse")]
        Task<OperationResult<ModifyQuestionUserAnswerResultResponse>> ModifyQuestionUserAnswerResultAsync(ModifyQuestionUserAnswerResultRequest request);
		///<summary>新活动页配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoConfigModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoConfigModelResponse")]
        Task<OperationResult<ActivityPageInfoModel>> GetActivityPageInfoConfigModelAsync(ActivityPageInfoRequest request);
		///<summary>活动页会场配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoHomeModels", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoHomeModelsResponse")]
        Task<OperationResult<List<ActivityPageInfoHomeModel>>> GetActivityPageInfoHomeModelsAsync(string hashKey);
		///<summary>活动页车品推荐</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoCpRecommends", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoCpRecommendsResponse")]
        Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoCpRecommendsAsync(ActivityPageInfoModuleRecommendRequest request);
		///<summary>活动页轮胎推荐</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoTireRecommends", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoTireRecommendsResponse")]
        Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoTireRecommendsAsync(ActivityPageInfoModuleRecommendRequest request);
		///<summary>活动页菜单配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowMenus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowMenusResponse")]
        Task<OperationResult<List<ActivityPageInfoRowMenuModel>>> GetActivityPageInfoRowMenusAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页老商品池配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPools", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPoolsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowProductPool>>> GetActivityPageInfoRowPoolsAsync(ActivityPageInfoModuleProductPoolRequest request);
		///<summary>活动页新商品池配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowNewPools", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowNewPoolsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowNewProductPool>>> GetActivityPageInfoRowNewPoolsAsync(ActivityPageInfoModuleNewProductPoolRequest request);
		///<summary>活动页倒计时配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCountDowns", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCountDownsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowCountDown>>> GetActivityPageInfoRowCountDownsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页文案配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowActivityTexts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowActivityTextsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowActivityText>>> GetActivityPageInfoRowActivityTextsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页文字链，滑动优惠券配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowJsons", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowJsonsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowJson>>> GetActivityPageInfoRowJsonsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页拼团配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPintuans", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPintuansResponse")]
        Task<OperationResult<List<ActivityPageInfoRowPintuan>>> GetActivityPageInfoRowPintuansAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页视频配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVideos", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVideosResponse")]
        Task<OperationResult<List<ActivityPageInfoRowVideo>>> GetActivityPageInfoRowVideosAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页其他活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOtherActivitys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOtherActivitysResponse")]
        Task<OperationResult<List<ActivityPageInfoRowOtherActivity>>> GetActivityPageInfoRowOtherActivitysAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页其他配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOthers", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOthersResponse")]
        Task<OperationResult<List<ActivityPageInfoRowOther>>> GetActivityPageInfoRowOthersAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页活动规则配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowRules", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowRulesResponse")]
        Task<OperationResult<List<ActivityPageInfoRowRule>>> GetActivityPageInfoRowRulesAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页保养配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowBys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowBysResponse")]
        Task<OperationResult<List<ActivityPageInfoRowBy>>> GetActivityPageInfoRowBysAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页优惠券配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCoupons", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCouponsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowCoupon>>> GetActivityPageInfoRowCouponsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页产品配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowProducts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowProductsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowProduct>>> GetActivityPageInfoRowProductsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页链接配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowLinks", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowLinksResponse")]
        Task<OperationResult<List<ActivityPageInfoRowLink>>> GetActivityPageInfoRowLinksAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页图片配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowImages", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowImagesResponse")]
        Task<OperationResult<List<ActivityPageInfoRowImage>>> GetActivityPageInfoRowImagesAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页秒杀配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowSeckills", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowSeckillsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowSeckill>>> GetActivityPageInfoRowSeckillsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页头图配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVehicleBanners", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVehicleBannersResponse")]
        Task<OperationResult<List<ActivityPageInfoRowVehicleBanner>>> GetActivityPageInfoRowVehicleBannersAsync(ActivityPageInfoModuleVehicleBannerRequest request);
		///<summary>通过活动类型获取活动 0 世界杯  1 拼团车型认证</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityInfoByType", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityInfoByTypeResponse")]
        Task<OperationResult<ActivityResponse>> GetActivityInfoByTypeAsync(int activityTypeId);
		///<summary>根据活动编号查询配置信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCustomerSettingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCustomerSettingInfoResponse")]
        Task<OperationResult<ActiveCustomerSettingResponse>> GetCustomerSettingInfoAsync(string activeNo);
		///<summary>查询用户绑定的券码</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponCode", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponCodeResponse")]
        Task<OperationResult<string>> GetUserCouponCodeAsync(string activityExclusiveId, string userid);
		///<summary>用户券码绑定</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CouponCodeBound", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CouponCodeBoundResponse")]
        Task<OperationResult<bool>> CouponCodeBoundAsync(ActivityCustomerCouponRequests activityCustomerCouponRequests);
		///<summary>客户专享活动下单验证</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActiveOrderVerify", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActiveOrderVerifyResponse")]
        Task<OperationResult<bool>> ActiveOrderVerifyAsync(ActivityOrderVerifyRequests activityOrderVerifyRequests);
		///<summary>H5 添加途虎星级门店认证信息服务</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddStarRatingStore", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddStarRatingStoreResponse")]
        Task<OperationResult<bool>> AddStarRatingStoreAsync(AddStarRatingStoreRequest request);
		///<summary>根据限时抢购ID查询大客户活动配置信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCustomerSettingInfoByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCustomerSettingInfoByActivityIdResponse")]
        Task<OperationResult<ActiveCustomerSettingResponse>> GetVipCustomerSettingInfoByActivityIdAsync(string activityId);
		///<summary>获取大客户订单中员工胎购买异常行为</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetExceptionalCustomerOrderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetExceptionalCustomerOrderInfoResponse")]
        Task<OperationResult<List<ActivityCustomerInvalidOrderResponse>>> GetExceptionalCustomerOrderInfoAsync();
		///<summary>获取蓄电池/加油卡活动配置</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigNewResponse")]
        Task<OperationResult<CouponActivityConfigNewModel>> SelectCouponActivityConfigNewAsync(CouponActivityConfigRequest request);
		///<summary>活动服务接口测试</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTestMethod", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTestMethodResponse")]
        Task<OperationResult<string>> ActivityTestMethodAsync(int testType);
		///<summary>添加活动报名页数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddRegistrationOfActivitiesData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddRegistrationOfActivitiesDataResponse")]
        Task<OperationResult<bool>> AddRegistrationOfActivitiesDataAsync(RegistrationOfActivitiesRequest request);
		///<summary>佣金商品列表查询接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductListResponse")]
        Task<OperationResult<List<CommissionProductModel>>> GetCommissionProductListAsync(GetCommissionProductListRequest request);
		///<summary>佣金商品详情查询接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductDetatils", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductDetatilsResponse")]
        Task<OperationResult<CommissionProductModel>> GetCommissionProductDetatilsAsync(GetCommissionProductDetatilsRequest request);
		///<summary>下单商品记录接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CreateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CreateOrderItemRecordResponse")]
        Task<OperationResult<CreateOrderItemRecordResponse>> CreateOrderItemRecordAsync(CreateOrderItemRecordRequest request);
		///<summary>佣金订单商品记录更新接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateOrderItemRecordResponse")]
        Task<OperationResult<UpdateOrderItemRecordResponse>> UpdateOrderItemRecordAsync(UpdateOrderItemRecordRequest request);
		///<summary>订单商品返佣接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityRebate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityRebateResponse")]
        Task<OperationResult<CommodityRebateResponse>> CommodityRebateAsync(CommodityRebateRequest request);
		///<summary>订单商品扣佣接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityDeduction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityDeductionResponse")]
        Task<OperationResult<CommodityDeductionResponse>> CommodityDeductionAsync(CommodityDeductionRequest request);
		///<summary>CPS修改佣金流水状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CpsUpdateRunning", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CpsUpdateRunningResponse")]
        Task<OperationResult<CpsUpdateRunningResponse>> CpsUpdateRunningAsync(CpsUpdateRunningRequest request);
	}

	/// <summary>活动服务</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IActivityClient : IActivityService, ITuhuServiceClient
    {
    	/// <summary>查询轮胎活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityResponse")]
        OperationResult<TireActivityModel> SelectTireActivity(string vehicleId, string tireSize);
		/// <summary>查询轮胎活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityNewResponse")]
        OperationResult<TireActivityModel> SelectTireActivityNew(TireActivityRequest request);
		/// <summary>查询轮胎跳转活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireChangedActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireChangedActivityResponse")]
        OperationResult<string> SelectTireChangedActivity(TireActivityRequest request);
		/// <summary>查询轮胎活动的产品</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityPidsResponse")]
        OperationResult<IEnumerable<string>> SelectTireActivityPids(Guid activityId);
		/// <summary>更新轮胎活动缓存</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateTireActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateTireActivityCacheResponse")]
        OperationResult<bool> UpdateTireActivityCache(string vehicleId, string tireSize);
		/// <summary>更新轮胎活动的产品</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityPidsCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityPidsCacheResponse")]
        OperationResult<bool> UpdateActivityPidsCache(Guid activityId);
		/// <summary>车型适配轮胎</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptTires", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptTiresResponse")]
        OperationResult<List<VehicleAdaptTireTireSizeDetailModel>> SelectVehicleAaptTires(VehicleAdaptTireRequestModel request);
		///<summary>优惠券信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCarTagCouponConfigs", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCarTagCouponConfigsResponse")]
        OperationResult<IEnumerable<CarTagCouponConfigModel>> SelectCarTagCouponConfigs();
		///<summary>车型适配保养</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptBaoyangs", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptBaoyangsResponse")]
        OperationResult<IEnumerable<VehicleAdaptBaoyangModel>> SelectVehicleAaptBaoyangs(string vehicleId);
		///<summary>车型适配车品信息</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAdaptChepins", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAdaptChepinsResponse")]
        OperationResult<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>> SelectVehicleAdaptChepins(string vehicleId);
		///<summary>获取排序后的轮胎规格</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleSortedTireSizes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleSortedTireSizesResponse")]
        OperationResult<IEnumerable<VehicleSortedTireSizeModel>> SelectVehicleSortedTireSizes(string vehicleId);
		///<summary> 插入用户分享信息并返回guid</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserShareInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserShareInfoResponse")]
        OperationResult<Guid> GetGuidAndInsertUserShareInfo(string pid, Guid batchGuid, Guid userId);
		///<summary> 根据Guid取出写入表中的数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserShareInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserShareInfoResponse")]
        OperationResult<ActivityUserShareInfoModel> GetActivityUserShareInfo(Guid shareId);
		///<summary> 根据活动ID查询用户领取次数</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPromotionPacketHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPromotionPacketHistoryResponse")]
        OperationResult<IEnumerable<PromotionPacketHistoryModel>> SelectPromotionPacketHistory(Guid userId, Guid luckyWheel);
		///<summary>根据配置表id跟用户id取出生成的新id，分享赚钱功能</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserForShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserForShareResponse")]
        OperationResult<Guid> GetGuidAndInsertUserForShare(Guid configGuid, Guid userId);
		///<summary>获取配置表的一条数据，分享赚钱功能</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRecommendGetGiftConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRecommendGetGiftConfigResponse")]
        OperationResult<RecommendGetGiftConfigModel> FetchRecommendGetGiftConfig(Guid? number=null,Guid? userId=null);
		///<summary>查询礼包领取</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPacketByUsers", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPacketByUsersResponse")]
        OperationResult<DataTable> SelectPacketByUsers();
		///<summary>查询轮胎活动</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityByActivityIdResponse")]
        OperationResult<TireActivityModel> SelectTireActivityByActivityId(Guid activityId);
		/// <summary>获取地区活动页的url</summary>/// <returns>活动期间根据地区取得活动页的链接，否则返回是否是未开始或者过期</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionActivityPageUrl", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionActivityPageUrlResponse")]
        OperationResult<RegionActivityPageModel> GetRegionActivityPageUrl(string city,string activityId);
		/// <summary>根据活动Id和地区Id或者车型Id获取目标活动地址</summary>
[Obsolete("请使用GetRegionVehicleIdActivityUrlNewAsync")]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrl", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlResponse")]
        OperationResult<ResultModel<string>> GetRegionVehicleIdActivityUrl(Guid activityId, int regionId, string vehicleId);
		/// <summary>根据活动Id,活动渠道和地区Id或者车型Id获取目标活动地址</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlNewResponse")]
        OperationResult<ResultModel<string>> GetRegionVehicleIdActivityUrlNew(Guid activityId, int regionId, string vehicleId, string activityChannel);
		/// <summary>清除缓存</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionVehicleIdActivityUrlCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionVehicleIdActivityUrlCacheResponse")]
        OperationResult<bool> RefreshRegionVehicleIdActivityUrlCache(Guid activityId);
		///<summary>落地页查询</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityConfigForDownloadApp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityConfigForDownloadAppResponse")]
        OperationResult<DownloadApp> GetActivityConfigForDownloadApp(int id);
		///<summary>清除落地页数据缓存</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CleanActivityConfigForDownloadAppCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CleanActivityConfigForDownloadAppCacheResponse")]
        OperationResult<bool> CleanActivityConfigForDownloadAppCache(int id);
		///<summary>取消相同支付账户的订单 -1:失败 1:取消成功 2:无取消订单</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CancelActivityOrderOfSamePaymentAccount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CancelActivityOrderOfSamePaymentAccountResponse")]
        OperationResult<int> CancelActivityOrderOfSamePaymentAccount(int orderId, string paymentAccount);
		///<summary>获取活动页数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelResponse")]
        OperationResult<ActivePageListModel> GetActivePageListModel(ActivtyPageRequest request);
		///<summary>获取活动页数据-zip</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelZip", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelZipResponse")]
        OperationResult<Tuhu.Service.Activity.Zip.Models.ActivePageListModel> GetActivePageListModelZip(ActivtyPageRequest request);
		///<summary>获取大翻盘用户可翻盘数量</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckyWheelUserlotteryCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckyWheelUserlotteryCountResponse")]
        OperationResult<int> GetLuckyWheelUserlotteryCount(Guid userid, Guid userGroup,string hashkey=null);
		///<summary>更新大翻盘用户可翻盘数量</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateLuckyWheelUserlotteryCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateLuckyWheelUserlotteryCountResponse")]
        OperationResult<int> UpdateLuckyWheelUserlotteryCount(Guid userid, Guid userGroup,string hashkey=null);
		///<summary>刷新活动页数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivePageListModelCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivePageListModelCacheResponse")]
        OperationResult<bool> RefreshActivePageListModelCache(ActivtyPageRequest request);
		///<summary>刷新大翻盘数据数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshLuckWheelCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshLuckWheelCacheResponse")]
        OperationResult<bool> RefreshLuckWheelCache(string id);
		/// <summary>验证轮胎订单是否能购买</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationByTires", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationByTiresResponse")]
        OperationResult<VerificationTiresResponseModel> VerificationByTires(VerificationTiresRequestModel requestModel);
		/// <summary>增加轮胎下单记录</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertTiresOrderRecordResponse")]
        OperationResult<bool> InsertTiresOrderRecord(TiresOrderRecordRequestModel requestModel);
		/// <summary>撤销轮胎下单记录</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RevokeTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RevokeTiresOrderRecordResponse")]
        OperationResult<bool> RevokeTiresOrderRecord(int orderId);
		/// <summary>Redis的轮胎订单记录重建</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RedisTiresOrderRecordReconStruction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RedisTiresOrderRecordReconStructionResponse")]
        OperationResult<bool> RedisTiresOrderRecordReconStruction(TiresOrderRecordRequestModel requestModel);
		/// <summary>RedisAndSql的轮胎订单记录查询</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTiresOrderRecordResponse")]
        OperationResult<Dictionary<string, object>> SelectTiresOrderRecord(TiresOrderRecordRequestModel requestModel);
		/// <summary>验证轮胎优惠券是否能领取</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationTiresPromotionRule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationTiresPromotionRuleResponse")]
        OperationResult<VerificationTiresResponseModel> VerificationTiresPromotionRule(VerificationTiresRequestModel requestModel,int ruleId);
		///<summary>获取大翻盘数据数据</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckWheel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckWheelResponse")]
        OperationResult<LuckyWheelModel> GetLuckWheel(string id);
		///<summary> 分享赚钱 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectShareActivityProductById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectShareActivityProductByIdResponse")]
        OperationResult<ShareProductModel> SelectShareActivityProductById(string ProductId,string BatchGuid=null);
		///<summary>保养活动</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectBaoYangActivitySetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectBaoYangActivitySettingResponse")]
        OperationResult<BaoYangActivitySetting> SelectBaoYangActivitySetting(string activityId);
		/// <summary></summary>///<returns></returns>
		[Obsolete("请使用SelectCouponActivityConfigNew")]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigResponse")]
        OperationResult<CouponActivityConfigModel> SelectCouponActivityConfig(string activityNum, int type);
		///<summary>获取活动类型</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectActivityTypeByActivityIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectActivityTypeByActivityIdsResponse")]
        OperationResult<IEnumerable<ActivityTypeModel>> SelectActivityTypeByActivityIds(List<Guid> activityIds);
		///<summary>根据搜索词获取活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityBuildWithSelKey", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityBuildWithSelKeyResponse")]
        OperationResult<ActivityBuild> GetActivityBuildWithSelKey(string keyword);
		///<summary>记录活动类型</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityTypeLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityTypeLogResponse")]
        OperationResult<bool> RecordActivityTypeLog(ActivityTypeRequest request);
		///<summary>更新保养活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangActivityConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangActivityConfigResponse")]
        OperationResult<bool> UpdateBaoYangActivityConfig(Guid activityId);
		///<summary>获取保养活动状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityStatusResponse")]
        OperationResult<FixedPriceActivityStatusResult> GetFixedPriceActivityStatus(Guid activityId, Guid userId, int regionId);
		///<summary>重置活动计数器</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangPurchaseCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangPurchaseCountResponse")]
        OperationResult<bool> UpdateBaoYangPurchaseCount(Guid activityId);
		///<summary>根据activityId获取活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityRound", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityRoundResponse")]
        OperationResult<FixedPriceActivityRoundResponse> GetFixedPriceActivityRound(Guid activityId);
		///<summary>根据activityId和RegionId获取活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRegionTiresActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRegionTiresActivityResponse")]
        OperationResult<TiresActivityResponse> FetchRegionTiresActivity(FlashSaleTiresActivityRequest request);
		///<summary>刷新轮胎活动缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionTiresActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionTiresActivityCacheResponse")]
        OperationResult<bool> RefreshRegionTiresActivityCache(Guid activityId, int regionId);
		///<summary>记录用户点击活动开始提醒</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityProductUserRemindLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityProductUserRemindLogResponse")]
        OperationResult<bool> RecordActivityProductUserRemindLog(ActivityProductUserRemindRequest request);
		///<summary>添加返现申请记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordResponse")]
        OperationResult<bool> InsertRebateApplyRecord(RebateApplyRequest request);
		///<summary>初始化白名单数据或者后面个别用户白名单状态调整使用</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertOrUpdateActivityPageWhiteListRecords", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertOrUpdateActivityPageWhiteListRecordsResponse")]
        OperationResult<bool> InsertOrUpdateActivityPageWhiteListRecords(List<ActivityPageWhiteListRequest> requests);
		///<summary>根据Userid判断是否是白名单用户</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageWhiteListByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageWhiteListByUserIdResponse")]
        OperationResult<bool> GetActivityPageWhiteListByUserId(Guid userId);
		///<summary>记录途虎轮胎节用户申请信息接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutUserRewardApplication", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutUserRewardApplicationResponse")]
        OperationResult<UserRewardApplicationResponse> PutUserRewardApplication(UserRewardApplicationRequest request);
		///<summary>添加返现申请记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordNewResponse")]
        OperationResult<ResultModel<bool>> InsertRebateApplyRecordNew(RebateApplyRequest request);
		///<summary>途虎贵就赔申请记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutApplyCompensateRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutApplyCompensateRecordResponse")]
        OperationResult<bool> PutApplyCompensateRecord(ApplyCompensateRequest request);
		///<summary>批量活动有效性验证接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivtyValidityResponses", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivtyValidityResponsesResponse")]
        OperationResult<List<ActivtyValidityResponse>> GetActivtyValidityResponses(ActivtyValidityRequest request);
		///<summary>获取预付卡场次信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCardSaleConfigDetails", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCardSaleConfigDetailsResponse")]
        OperationResult<List<VipCardSaleConfigDetailModel>> GetVipCardSaleConfigDetails(string activityId);
		///<summary>check购买的批次是否还有剩余库存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VipCardCheckStock", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VipCardCheckStockResponse")]
        OperationResult<Dictionary<string, bool>> VipCardCheckStock(List<string> batchIds);
		///<summary>创建订单时记录卡信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutVipCardRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutVipCardRecordResponse")]
        OperationResult<bool> PutVipCardRecord(VipCardRecordRequest request);
		///<summary>支付成功时调用绑卡</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/BindVipCard", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/BindVipCardResponse")]
        OperationResult<bool> BindVipCard(int orderId);
		///<summary>获取返现申请页面配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyPageConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyPageConfigResponse")]
        OperationResult<RebateApplyPageConfig> SelectRebateApplyPageConfig();
		///<summary>申请返现</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordV2", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordV2Response")]
        OperationResult<ResultModel<bool>> InsertRebateApplyRecordV2(RebateApplyRequest request);
		///<summary>获取用户所有返现申请信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyByOpenId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyByOpenIdResponse")]
        OperationResult<List<RebateApplyResponse>> SelectRebateApplyByOpenId(string openId);
		///<summary>通过OrderId获取最新的申请记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRebateApplyByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRebateApplyByOrderIdResponse")]
        OperationResult<IEnumerable<RebateApplyResponse>> GetRebateApplyByOrderId(int orderId);
		///<summary>取消订单的时候更新db跟缓存中的数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyVipCardRecordByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyVipCardRecordByOrderIdResponse")]
        OperationResult<bool> ModifyVipCardRecordByOrderId(int orderId);
		///<summary>获取2018世界杯的活动对象和积分规则信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetWorldCup2018Activity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetWorldCup2018ActivityResponse")]
        OperationResult<ActivityResponse> GetWorldCup2018Activity();
		///<summary>通过用户ID获取兑换券数量接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCouponCountByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCouponCountByUserIdResponse")]
        OperationResult<int> GetCouponCountByUserId(Guid userId, long activityId);
		///<summary>返回活动兑换券排行排名</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchCouponRank", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchCouponRankResponse")]
        OperationResult<PagedModel<ActivityCouponRankResponse>> SearchCouponRank(long activityId, int pageIndex = 1, int pageSize = 20);
		///<summary>返回用户的兑换券排名情况</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponRank", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponRankResponse")]
        OperationResult<ActivityCouponRankResponse> GetUserCouponRank(Guid userId, long activityId);
		///<summary>兑换物列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeListResponse")]
        OperationResult<PagedModel<ActivityPrizeResponse>> SearchPrizeList(SearchPrizeListRequest searchPrizeListRequest);
		///<summary>用户兑换奖品 异常代码：-1 系统异常（请重试） , -2 兑换卡不足  -3 库存不足  -4 已经下架   -5 已经兑换   -6 兑换时间已经截止不能兑换</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UserRedeemPrizes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UserRedeemPrizesResponse")]
        OperationResult<bool> UserRedeemPrizes(Guid userId, long prizeId,long activityId);
		///<summary>用户已兑换商品列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeOrderDetailListByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeOrderDetailListByUserIdResponse")]
        OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>> SearchPrizeOrderDetailListByUserId(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20);
		///<summary>今日竞猜题目</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionResponse")]
        OperationResult<IEnumerable<Models.Response.Question>> SearchQuestion( Guid userId , long activityId);
		///<summary>提交用户竞猜</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionAnswerResponse")]
        OperationResult<bool> SubmitQuestionAnswer(SubmitQuestionAnswerRequest submitQuestionAnswerRequest);
		///<summary>返回用户答题历史</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionAnswerHistoryByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionAnswerHistoryByUserIdResponse")]
        OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>> SearchQuestionAnswerHistoryByUserId(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest);
		///<summary>返回用户胜利次数和胜利称号</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVictoryInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVictoryInfoResponse")]
        OperationResult<ActivityVictoryInfoResponse> GetVictoryInfo(Guid userId, long activityId);
		///<summary>活动分享赠送积分  异常：   -77 活动未开始  -2 今日已经分享   -1 系统异常</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityShareResponse")]
        OperationResult<bool> ActivityShare(ActivityShareDetailRequest shareDetailRequest);
		///<summary>今日是否已经分享了 true = 今日已经分享</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTodayAlreadyShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTodayAlreadyShareResponse")]
        OperationResult<bool> ActivityTodayAlreadyShare(Guid userId, long activityId);
		///<summary>用来获取或者刷新排序好的车型适配商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetOrSetActivityPageSortedPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetOrSetActivityPageSortedPidsResponse")]
        OperationResult<List<string>> GetOrSetActivityPageSortedPids(SortedPidsRequest request);
		///<summary>修改或者增加用户兑换券 并且增加日志  返回主键</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyActivityCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyActivityCouponResponse")]
        OperationResult<long> ModifyActivityCoupon(Guid userId, long activityId, int couponCount , string couponName,DateTime? modifyDateTime = null);
		///<summary>刷新活动题目  缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityQuestionCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityQuestionCacheResponse")]
        OperationResult<bool> RefreshActivityQuestionCache(long activityId);
		///<summary>刷新活动兑换物  缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityPrizeCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityPrizeCacheResponse")]
        OperationResult<bool> RefreshActivityPrizeCache(long activityId);
		///<summary>保存用户答题数据到数据库</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswerResponse")]
        OperationResult<SubmitActivityQuestionUserAnswerResponse> SubmitQuestionUserAnswer(SubmitActivityQuestionUserAnswerRequest request);
		///<summary>更新用户答题结果状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResultResponse")]
        OperationResult<ModifyQuestionUserAnswerResultResponse> ModifyQuestionUserAnswerResult(ModifyQuestionUserAnswerResultRequest request);
		///<summary>新活动页配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoConfigModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoConfigModelResponse")]
        OperationResult<ActivityPageInfoModel> GetActivityPageInfoConfigModel(ActivityPageInfoRequest request);
		///<summary>活动页会场配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoHomeModels", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoHomeModelsResponse")]
        OperationResult<List<ActivityPageInfoHomeModel>> GetActivityPageInfoHomeModels(string hashKey);
		///<summary>活动页车品推荐</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoCpRecommends", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoCpRecommendsResponse")]
        OperationResult<List<ActivityPageInfoRecommend>> GetActivityPageInfoCpRecommends(ActivityPageInfoModuleRecommendRequest request);
		///<summary>活动页轮胎推荐</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoTireRecommends", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoTireRecommendsResponse")]
        OperationResult<List<ActivityPageInfoRecommend>> GetActivityPageInfoTireRecommends(ActivityPageInfoModuleRecommendRequest request);
		///<summary>活动页菜单配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowMenus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowMenusResponse")]
        OperationResult<List<ActivityPageInfoRowMenuModel>> GetActivityPageInfoRowMenus(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页老商品池配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPools", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPoolsResponse")]
        OperationResult<List<ActivityPageInfoRowProductPool>> GetActivityPageInfoRowPools(ActivityPageInfoModuleProductPoolRequest request);
		///<summary>活动页新商品池配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowNewPools", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowNewPoolsResponse")]
        OperationResult<List<ActivityPageInfoRowNewProductPool>> GetActivityPageInfoRowNewPools(ActivityPageInfoModuleNewProductPoolRequest request);
		///<summary>活动页倒计时配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCountDowns", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCountDownsResponse")]
        OperationResult<List<ActivityPageInfoRowCountDown>> GetActivityPageInfoRowCountDowns(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页文案配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowActivityTexts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowActivityTextsResponse")]
        OperationResult<List<ActivityPageInfoRowActivityText>> GetActivityPageInfoRowActivityTexts(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页文字链，滑动优惠券配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowJsons", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowJsonsResponse")]
        OperationResult<List<ActivityPageInfoRowJson>> GetActivityPageInfoRowJsons(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页拼团配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPintuans", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPintuansResponse")]
        OperationResult<List<ActivityPageInfoRowPintuan>> GetActivityPageInfoRowPintuans(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页视频配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVideos", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVideosResponse")]
        OperationResult<List<ActivityPageInfoRowVideo>> GetActivityPageInfoRowVideos(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页其他活动配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOtherActivitys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOtherActivitysResponse")]
        OperationResult<List<ActivityPageInfoRowOtherActivity>> GetActivityPageInfoRowOtherActivitys(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页其他配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOthers", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOthersResponse")]
        OperationResult<List<ActivityPageInfoRowOther>> GetActivityPageInfoRowOthers(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页活动规则配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowRules", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowRulesResponse")]
        OperationResult<List<ActivityPageInfoRowRule>> GetActivityPageInfoRowRules(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页保养配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowBys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowBysResponse")]
        OperationResult<List<ActivityPageInfoRowBy>> GetActivityPageInfoRowBys(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页优惠券配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCoupons", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCouponsResponse")]
        OperationResult<List<ActivityPageInfoRowCoupon>> GetActivityPageInfoRowCoupons(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页产品配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowProducts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowProductsResponse")]
        OperationResult<List<ActivityPageInfoRowProduct>> GetActivityPageInfoRowProducts(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页链接配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowLinks", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowLinksResponse")]
        OperationResult<List<ActivityPageInfoRowLink>> GetActivityPageInfoRowLinks(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页图片配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowImages", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowImagesResponse")]
        OperationResult<List<ActivityPageInfoRowImage>> GetActivityPageInfoRowImages(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页秒杀配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowSeckills", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowSeckillsResponse")]
        OperationResult<List<ActivityPageInfoRowSeckill>> GetActivityPageInfoRowSeckills(ActivityPageInfoModuleBaseRequest request);
		///<summary>活动页头图配置</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVehicleBanners", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVehicleBannersResponse")]
        OperationResult<List<ActivityPageInfoRowVehicleBanner>> GetActivityPageInfoRowVehicleBanners(ActivityPageInfoModuleVehicleBannerRequest request);
		///<summary>通过活动类型获取活动 0 世界杯  1 拼团车型认证</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityInfoByType", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityInfoByTypeResponse")]
        OperationResult<ActivityResponse> GetActivityInfoByType(int activityTypeId);
		///<summary>根据活动编号查询配置信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCustomerSettingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCustomerSettingInfoResponse")]
        OperationResult<ActiveCustomerSettingResponse> GetCustomerSettingInfo(string activeNo);
		///<summary>查询用户绑定的券码</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponCode", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponCodeResponse")]
        OperationResult<string> GetUserCouponCode(string activityExclusiveId, string userid);
		///<summary>用户券码绑定</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CouponCodeBound", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CouponCodeBoundResponse")]
        OperationResult<bool> CouponCodeBound(ActivityCustomerCouponRequests activityCustomerCouponRequests);
		///<summary>客户专享活动下单验证</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActiveOrderVerify", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActiveOrderVerifyResponse")]
        OperationResult<bool> ActiveOrderVerify(ActivityOrderVerifyRequests activityOrderVerifyRequests);
		///<summary>H5 添加途虎星级门店认证信息服务</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddStarRatingStore", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddStarRatingStoreResponse")]
        OperationResult<bool> AddStarRatingStore(AddStarRatingStoreRequest request);
		///<summary>根据限时抢购ID查询大客户活动配置信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCustomerSettingInfoByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCustomerSettingInfoByActivityIdResponse")]
        OperationResult<ActiveCustomerSettingResponse> GetVipCustomerSettingInfoByActivityId(string activityId);
		///<summary>获取大客户订单中员工胎购买异常行为</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetExceptionalCustomerOrderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetExceptionalCustomerOrderInfoResponse")]
        OperationResult<List<ActivityCustomerInvalidOrderResponse>> GetExceptionalCustomerOrderInfo();
		///<summary>获取蓄电池/加油卡活动配置</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigNewResponse")]
        OperationResult<CouponActivityConfigNewModel> SelectCouponActivityConfigNew(CouponActivityConfigRequest request);
		///<summary>活动服务接口测试</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTestMethod", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTestMethodResponse")]
        OperationResult<string> ActivityTestMethod(int testType);
		///<summary>添加活动报名页数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddRegistrationOfActivitiesData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddRegistrationOfActivitiesDataResponse")]
        OperationResult<bool> AddRegistrationOfActivitiesData(RegistrationOfActivitiesRequest request);
		///<summary>佣金商品列表查询接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductListResponse")]
        OperationResult<List<CommissionProductModel>> GetCommissionProductList(GetCommissionProductListRequest request);
		///<summary>佣金商品详情查询接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductDetatils", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductDetatilsResponse")]
        OperationResult<CommissionProductModel> GetCommissionProductDetatils(GetCommissionProductDetatilsRequest request);
		///<summary>下单商品记录接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CreateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CreateOrderItemRecordResponse")]
        OperationResult<CreateOrderItemRecordResponse> CreateOrderItemRecord(CreateOrderItemRecordRequest request);
		///<summary>佣金订单商品记录更新接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateOrderItemRecordResponse")]
        OperationResult<UpdateOrderItemRecordResponse> UpdateOrderItemRecord(UpdateOrderItemRecordRequest request);
		///<summary>订单商品返佣接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityRebate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityRebateResponse")]
        OperationResult<CommodityRebateResponse> CommodityRebate(CommodityRebateRequest request);
		///<summary>订单商品扣佣接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityDeduction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityDeductionResponse")]
        OperationResult<CommodityDeductionResponse> CommodityDeduction(CommodityDeductionRequest request);
		///<summary>CPS修改佣金流水状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CpsUpdateRunning", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CpsUpdateRunningResponse")]
        OperationResult<CpsUpdateRunningResponse> CpsUpdateRunning(CpsUpdateRunningRequest request);
	}

	/// <summary>活动服务</summary>
	public partial class ActivityClient : TuhuServiceClient<IActivityClient>, IActivityClient
    {
    	/// <summary>查询轮胎活动</summary>/// <returns></returns>
        public OperationResult<TireActivityModel> SelectTireActivity(string vehicleId, string tireSize) => Invoke(_ => _.SelectTireActivity(vehicleId,tireSize));

	/// <summary>查询轮胎活动</summary>/// <returns></returns>
        public Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize) => InvokeAsync(_ => _.SelectTireActivityAsync(vehicleId,tireSize));
		/// <summary>查询轮胎活动</summary>/// <returns></returns>
        public OperationResult<TireActivityModel> SelectTireActivityNew(TireActivityRequest request) => Invoke(_ => _.SelectTireActivityNew(request));

	/// <summary>查询轮胎活动</summary>/// <returns></returns>
        public Task<OperationResult<TireActivityModel>> SelectTireActivityNewAsync(TireActivityRequest request) => InvokeAsync(_ => _.SelectTireActivityNewAsync(request));
		/// <summary>查询轮胎跳转活动</summary>/// <returns></returns>
        public OperationResult<string> SelectTireChangedActivity(TireActivityRequest request) => Invoke(_ => _.SelectTireChangedActivity(request));

	/// <summary>查询轮胎跳转活动</summary>/// <returns></returns>
        public Task<OperationResult<string>> SelectTireChangedActivityAsync(TireActivityRequest request) => InvokeAsync(_ => _.SelectTireChangedActivityAsync(request));
		/// <summary>查询轮胎活动的产品</summary>/// <returns></returns>
        public OperationResult<IEnumerable<string>> SelectTireActivityPids(Guid activityId) => Invoke(_ => _.SelectTireActivityPids(activityId));

	/// <summary>查询轮胎活动的产品</summary>/// <returns></returns>
        public Task<OperationResult<IEnumerable<string>>> SelectTireActivityPidsAsync(Guid activityId) => InvokeAsync(_ => _.SelectTireActivityPidsAsync(activityId));
		/// <summary>更新轮胎活动缓存</summary>/// <returns></returns>
        public OperationResult<bool> UpdateTireActivityCache(string vehicleId, string tireSize) => Invoke(_ => _.UpdateTireActivityCache(vehicleId,tireSize));

	/// <summary>更新轮胎活动缓存</summary>/// <returns></returns>
        public Task<OperationResult<bool>> UpdateTireActivityCacheAsync(string vehicleId, string tireSize) => InvokeAsync(_ => _.UpdateTireActivityCacheAsync(vehicleId,tireSize));
		/// <summary>更新轮胎活动的产品</summary>/// <returns></returns>
        public OperationResult<bool> UpdateActivityPidsCache(Guid activityId) => Invoke(_ => _.UpdateActivityPidsCache(activityId));

	/// <summary>更新轮胎活动的产品</summary>/// <returns></returns>
        public Task<OperationResult<bool>> UpdateActivityPidsCacheAsync(Guid activityId) => InvokeAsync(_ => _.UpdateActivityPidsCacheAsync(activityId));
		/// <summary>车型适配轮胎</summary>/// <returns></returns>
        public OperationResult<List<VehicleAdaptTireTireSizeDetailModel>> SelectVehicleAaptTires(VehicleAdaptTireRequestModel request) => Invoke(_ => _.SelectVehicleAaptTires(request));

	/// <summary>车型适配轮胎</summary>/// <returns></returns>
        public Task<OperationResult<List<VehicleAdaptTireTireSizeDetailModel>>> SelectVehicleAaptTiresAsync(VehicleAdaptTireRequestModel request) => InvokeAsync(_ => _.SelectVehicleAaptTiresAsync(request));
		///<summary>优惠券信息</summary>///<returns></returns>
        public OperationResult<IEnumerable<CarTagCouponConfigModel>> SelectCarTagCouponConfigs() => Invoke(_ => _.SelectCarTagCouponConfigs());

	///<summary>优惠券信息</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<CarTagCouponConfigModel>>> SelectCarTagCouponConfigsAsync() => InvokeAsync(_ => _.SelectCarTagCouponConfigsAsync());
		///<summary>车型适配保养</summary>///<returns></returns>
        public OperationResult<IEnumerable<VehicleAdaptBaoyangModel>> SelectVehicleAaptBaoyangs(string vehicleId) => Invoke(_ => _.SelectVehicleAaptBaoyangs(vehicleId));

	///<summary>车型适配保养</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<VehicleAdaptBaoyangModel>>> SelectVehicleAaptBaoyangsAsync(string vehicleId) => InvokeAsync(_ => _.SelectVehicleAaptBaoyangsAsync(vehicleId));
		///<summary>车型适配车品信息</summary>///<returns></returns>
        public OperationResult<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>> SelectVehicleAdaptChepins(string vehicleId) => Invoke(_ => _.SelectVehicleAdaptChepins(vehicleId));

	///<summary>车型适配车品信息</summary>///<returns></returns>
        public Task<OperationResult<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>>> SelectVehicleAdaptChepinsAsync(string vehicleId) => InvokeAsync(_ => _.SelectVehicleAdaptChepinsAsync(vehicleId));
		///<summary>获取排序后的轮胎规格</summary>///<returns></returns>
        public OperationResult<IEnumerable<VehicleSortedTireSizeModel>> SelectVehicleSortedTireSizes(string vehicleId) => Invoke(_ => _.SelectVehicleSortedTireSizes(vehicleId));

	///<summary>获取排序后的轮胎规格</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<VehicleSortedTireSizeModel>>> SelectVehicleSortedTireSizesAsync(string vehicleId) => InvokeAsync(_ => _.SelectVehicleSortedTireSizesAsync(vehicleId));
		///<summary> 插入用户分享信息并返回guid</summary>///<returns></returns>
        public OperationResult<Guid> GetGuidAndInsertUserShareInfo(string pid, Guid batchGuid, Guid userId) => Invoke(_ => _.GetGuidAndInsertUserShareInfo(pid,batchGuid,userId));

	///<summary> 插入用户分享信息并返回guid</summary>///<returns></returns>
        public Task<OperationResult<Guid>> GetGuidAndInsertUserShareInfoAsync(string pid, Guid batchGuid, Guid userId) => InvokeAsync(_ => _.GetGuidAndInsertUserShareInfoAsync(pid,batchGuid,userId));
		///<summary> 根据Guid取出写入表中的数据</summary>///<returns></returns>
        public OperationResult<ActivityUserShareInfoModel> GetActivityUserShareInfo(Guid shareId) => Invoke(_ => _.GetActivityUserShareInfo(shareId));

	///<summary> 根据Guid取出写入表中的数据</summary>///<returns></returns>
        public Task<OperationResult<ActivityUserShareInfoModel>> GetActivityUserShareInfoAsync(Guid shareId) => InvokeAsync(_ => _.GetActivityUserShareInfoAsync(shareId));
		///<summary> 根据活动ID查询用户领取次数</summary>///<returns></returns>
        public OperationResult<IEnumerable<PromotionPacketHistoryModel>> SelectPromotionPacketHistory(Guid userId, Guid luckyWheel) => Invoke(_ => _.SelectPromotionPacketHistory(userId,luckyWheel));

	///<summary> 根据活动ID查询用户领取次数</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<PromotionPacketHistoryModel>>> SelectPromotionPacketHistoryAsync(Guid userId, Guid luckyWheel) => InvokeAsync(_ => _.SelectPromotionPacketHistoryAsync(userId,luckyWheel));
		///<summary>根据配置表id跟用户id取出生成的新id，分享赚钱功能</summary>///<returns></returns>
        public OperationResult<Guid> GetGuidAndInsertUserForShare(Guid configGuid, Guid userId) => Invoke(_ => _.GetGuidAndInsertUserForShare(configGuid,userId));

	///<summary>根据配置表id跟用户id取出生成的新id，分享赚钱功能</summary>///<returns></returns>
        public Task<OperationResult<Guid>> GetGuidAndInsertUserForShareAsync(Guid configGuid, Guid userId) => InvokeAsync(_ => _.GetGuidAndInsertUserForShareAsync(configGuid,userId));
		///<summary>获取配置表的一条数据，分享赚钱功能</summary>///<returns></returns>
        public OperationResult<RecommendGetGiftConfigModel> FetchRecommendGetGiftConfig(Guid? number=null,Guid? userId=null) => Invoke(_ => _.FetchRecommendGetGiftConfig(number,userId));

	///<summary>获取配置表的一条数据，分享赚钱功能</summary>///<returns></returns>
        public Task<OperationResult<RecommendGetGiftConfigModel>> FetchRecommendGetGiftConfigAsync(Guid? number=null,Guid? userId=null) => InvokeAsync(_ => _.FetchRecommendGetGiftConfigAsync(number,userId));
		///<summary>查询礼包领取</summary>///<returns></returns>
        public OperationResult<DataTable> SelectPacketByUsers() => Invoke(_ => _.SelectPacketByUsers());

	///<summary>查询礼包领取</summary>///<returns></returns>
        public Task<OperationResult<DataTable>> SelectPacketByUsersAsync() => InvokeAsync(_ => _.SelectPacketByUsersAsync());
		///<summary>查询轮胎活动</summary>///<returns></returns>
        public OperationResult<TireActivityModel> SelectTireActivityByActivityId(Guid activityId) => Invoke(_ => _.SelectTireActivityByActivityId(activityId));

	///<summary>查询轮胎活动</summary>///<returns></returns>
        public Task<OperationResult<TireActivityModel>> SelectTireActivityByActivityIdAsync(Guid activityId) => InvokeAsync(_ => _.SelectTireActivityByActivityIdAsync(activityId));
		/// <summary>获取地区活动页的url</summary>/// <returns>活动期间根据地区取得活动页的链接，否则返回是否是未开始或者过期</returns>
        public OperationResult<RegionActivityPageModel> GetRegionActivityPageUrl(string city,string activityId) => Invoke(_ => _.GetRegionActivityPageUrl(city,activityId));

	/// <summary>获取地区活动页的url</summary>/// <returns>活动期间根据地区取得活动页的链接，否则返回是否是未开始或者过期</returns>
        public Task<OperationResult<RegionActivityPageModel>> GetRegionActivityPageUrlAsync(string city,string activityId) => InvokeAsync(_ => _.GetRegionActivityPageUrlAsync(city,activityId));
		/// <summary>根据活动Id和地区Id或者车型Id获取目标活动地址</summary>
[Obsolete("请使用GetRegionVehicleIdActivityUrlNewAsync")]
        public OperationResult<ResultModel<string>> GetRegionVehicleIdActivityUrl(Guid activityId, int regionId, string vehicleId) => Invoke(_ => _.GetRegionVehicleIdActivityUrl(activityId, regionId, vehicleId));

	/// <summary>根据活动Id和地区Id或者车型Id获取目标活动地址</summary>
[Obsolete("请使用GetRegionVehicleIdActivityUrlNewAsync")]
        public Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlAsync(Guid activityId, int regionId, string vehicleId) => InvokeAsync(_ => _.GetRegionVehicleIdActivityUrlAsync(activityId, regionId, vehicleId));
		/// <summary>根据活动Id,活动渠道和地区Id或者车型Id获取目标活动地址</summary>/// <returns></returns>
        public OperationResult<ResultModel<string>> GetRegionVehicleIdActivityUrlNew(Guid activityId, int regionId, string vehicleId, string activityChannel) => Invoke(_ => _.GetRegionVehicleIdActivityUrlNew(activityId, regionId, vehicleId, activityChannel));

	/// <summary>根据活动Id,活动渠道和地区Id或者车型Id获取目标活动地址</summary>/// <returns></returns>
        public Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlNewAsync(Guid activityId, int regionId, string vehicleId, string activityChannel) => InvokeAsync(_ => _.GetRegionVehicleIdActivityUrlNewAsync(activityId, regionId, vehicleId, activityChannel));
		/// <summary>清除缓存</summary>/// <returns></returns>
        public OperationResult<bool> RefreshRegionVehicleIdActivityUrlCache(Guid activityId) => Invoke(_ => _.RefreshRegionVehicleIdActivityUrlCache(activityId));

	/// <summary>清除缓存</summary>/// <returns></returns>
        public Task<OperationResult<bool>> RefreshRegionVehicleIdActivityUrlCacheAsync(Guid activityId) => InvokeAsync(_ => _.RefreshRegionVehicleIdActivityUrlCacheAsync(activityId));
		///<summary>落地页查询</summary>///<returns></returns>
        public OperationResult<DownloadApp> GetActivityConfigForDownloadApp(int id) => Invoke(_ => _.GetActivityConfigForDownloadApp(id));

	///<summary>落地页查询</summary>///<returns></returns>
        public Task<OperationResult<DownloadApp>> GetActivityConfigForDownloadAppAsync(int id) => InvokeAsync(_ => _.GetActivityConfigForDownloadAppAsync(id));
		///<summary>清除落地页数据缓存</summary>///<returns></returns>
        public OperationResult<bool> CleanActivityConfigForDownloadAppCache(int id) => Invoke(_ => _.CleanActivityConfigForDownloadAppCache(id));

	///<summary>清除落地页数据缓存</summary>///<returns></returns>
        public Task<OperationResult<bool>> CleanActivityConfigForDownloadAppCacheAsync(int id) => InvokeAsync(_ => _.CleanActivityConfigForDownloadAppCacheAsync(id));
		///<summary>取消相同支付账户的订单 -1:失败 1:取消成功 2:无取消订单</summary>///<returns></returns>
        public OperationResult<int> CancelActivityOrderOfSamePaymentAccount(int orderId, string paymentAccount) => Invoke(_ => _.CancelActivityOrderOfSamePaymentAccount(orderId,paymentAccount));

	///<summary>取消相同支付账户的订单 -1:失败 1:取消成功 2:无取消订单</summary>///<returns></returns>
        public Task<OperationResult<int>> CancelActivityOrderOfSamePaymentAccountAsync(int orderId, string paymentAccount) => InvokeAsync(_ => _.CancelActivityOrderOfSamePaymentAccountAsync(orderId,paymentAccount));
		///<summary>获取活动页数据</summary>///<returns></returns>
        public OperationResult<ActivePageListModel> GetActivePageListModel(ActivtyPageRequest request) => Invoke(_ => _.GetActivePageListModel(request));

	///<summary>获取活动页数据</summary>///<returns></returns>
        public Task<OperationResult<ActivePageListModel>> GetActivePageListModelAsync(ActivtyPageRequest request) => InvokeAsync(_ => _.GetActivePageListModelAsync(request));
		///<summary>获取活动页数据-zip</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Zip.Models.ActivePageListModel> GetActivePageListModelZip(ActivtyPageRequest request) => Invoke(_ => _.GetActivePageListModelZip(request));

	///<summary>获取活动页数据-zip</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Zip.Models.ActivePageListModel>> GetActivePageListModelZipAsync(ActivtyPageRequest request) => InvokeAsync(_ => _.GetActivePageListModelZipAsync(request));
		///<summary>获取大翻盘用户可翻盘数量</summary>///<returns></returns>
        public OperationResult<int> GetLuckyWheelUserlotteryCount(Guid userid, Guid userGroup,string hashkey=null) => Invoke(_ => _.GetLuckyWheelUserlotteryCount(userid,userGroup,hashkey));

	///<summary>获取大翻盘用户可翻盘数量</summary>///<returns></returns>
        public Task<OperationResult<int>> GetLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup,string hashkey=null) => InvokeAsync(_ => _.GetLuckyWheelUserlotteryCountAsync(userid,userGroup,hashkey));
		///<summary>更新大翻盘用户可翻盘数量</summary>///<returns></returns>
        public OperationResult<int> UpdateLuckyWheelUserlotteryCount(Guid userid, Guid userGroup,string hashkey=null) => Invoke(_ => _.UpdateLuckyWheelUserlotteryCount(userid,userGroup,hashkey));

	///<summary>更新大翻盘用户可翻盘数量</summary>///<returns></returns>
        public Task<OperationResult<int>> UpdateLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup,string hashkey=null) => InvokeAsync(_ => _.UpdateLuckyWheelUserlotteryCountAsync(userid,userGroup,hashkey));
		///<summary>刷新活动页数据</summary>///<returns></returns>
        public OperationResult<bool> RefreshActivePageListModelCache(ActivtyPageRequest request) => Invoke(_ => _.RefreshActivePageListModelCache(request));

	///<summary>刷新活动页数据</summary>///<returns></returns>
        public Task<OperationResult<bool>> RefreshActivePageListModelCacheAsync(ActivtyPageRequest request) => InvokeAsync(_ => _.RefreshActivePageListModelCacheAsync(request));
		///<summary>刷新大翻盘数据数据</summary>///<returns></returns>
        public OperationResult<bool> RefreshLuckWheelCache(string id) => Invoke(_ => _.RefreshLuckWheelCache(id));

	///<summary>刷新大翻盘数据数据</summary>///<returns></returns>
        public Task<OperationResult<bool>> RefreshLuckWheelCacheAsync(string id) => InvokeAsync(_ => _.RefreshLuckWheelCacheAsync(id));
		/// <summary>验证轮胎订单是否能购买</summary>
			/// <returns></returns>
        public OperationResult<VerificationTiresResponseModel> VerificationByTires(VerificationTiresRequestModel requestModel) => Invoke(_ => _.VerificationByTires(requestModel));

	/// <summary>验证轮胎订单是否能购买</summary>
			/// <returns></returns>
        public Task<OperationResult<VerificationTiresResponseModel>> VerificationByTiresAsync(VerificationTiresRequestModel requestModel) => InvokeAsync(_ => _.VerificationByTiresAsync(requestModel));
		/// <summary>增加轮胎下单记录</summary>
			/// <returns></returns>
        public OperationResult<bool> InsertTiresOrderRecord(TiresOrderRecordRequestModel requestModel) => Invoke(_ => _.InsertTiresOrderRecord(requestModel));

	/// <summary>增加轮胎下单记录</summary>
			/// <returns></returns>
        public Task<OperationResult<bool>> InsertTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel) => InvokeAsync(_ => _.InsertTiresOrderRecordAsync(requestModel));
		/// <summary>撤销轮胎下单记录</summary>
			/// <returns></returns>
        public OperationResult<bool> RevokeTiresOrderRecord(int orderId) => Invoke(_ => _.RevokeTiresOrderRecord(orderId));

	/// <summary>撤销轮胎下单记录</summary>
			/// <returns></returns>
        public Task<OperationResult<bool>> RevokeTiresOrderRecordAsync(int orderId) => InvokeAsync(_ => _.RevokeTiresOrderRecordAsync(orderId));
		/// <summary>Redis的轮胎订单记录重建</summary>
			/// <returns></returns>
        public OperationResult<bool> RedisTiresOrderRecordReconStruction(TiresOrderRecordRequestModel requestModel) => Invoke(_ => _.RedisTiresOrderRecordReconStruction(requestModel));

	/// <summary>Redis的轮胎订单记录重建</summary>
			/// <returns></returns>
        public Task<OperationResult<bool>> RedisTiresOrderRecordReconStructionAsync(TiresOrderRecordRequestModel requestModel) => InvokeAsync(_ => _.RedisTiresOrderRecordReconStructionAsync(requestModel));
		/// <summary>RedisAndSql的轮胎订单记录查询</summary>
			/// <returns></returns>
        public OperationResult<Dictionary<string, object>> SelectTiresOrderRecord(TiresOrderRecordRequestModel requestModel) => Invoke(_ => _.SelectTiresOrderRecord(requestModel));

	/// <summary>RedisAndSql的轮胎订单记录查询</summary>
			/// <returns></returns>
        public Task<OperationResult<Dictionary<string, object>>> SelectTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel) => InvokeAsync(_ => _.SelectTiresOrderRecordAsync(requestModel));
		/// <summary>验证轮胎优惠券是否能领取</summary>
			/// <returns></returns>
        public OperationResult<VerificationTiresResponseModel> VerificationTiresPromotionRule(VerificationTiresRequestModel requestModel,int ruleId) => Invoke(_ => _.VerificationTiresPromotionRule(requestModel,ruleId));

	/// <summary>验证轮胎优惠券是否能领取</summary>
			/// <returns></returns>
        public Task<OperationResult<VerificationTiresResponseModel>> VerificationTiresPromotionRuleAsync(VerificationTiresRequestModel requestModel,int ruleId) => InvokeAsync(_ => _.VerificationTiresPromotionRuleAsync(requestModel,ruleId));
		///<summary>获取大翻盘数据数据</summary>///<returns></returns>
        public OperationResult<LuckyWheelModel> GetLuckWheel(string id) => Invoke(_ => _.GetLuckWheel(id));

	///<summary>获取大翻盘数据数据</summary>///<returns></returns>
        public Task<OperationResult<LuckyWheelModel>> GetLuckWheelAsync(string id) => InvokeAsync(_ => _.GetLuckWheelAsync(id));
		///<summary> 分享赚钱 </summary>
        public OperationResult<ShareProductModel> SelectShareActivityProductById(string ProductId,string BatchGuid=null) => Invoke(_ => _.SelectShareActivityProductById(ProductId,BatchGuid));

	///<summary> 分享赚钱 </summary>
        public Task<OperationResult<ShareProductModel>> SelectShareActivityProductByIdAsync(string ProductId,string BatchGuid=null) => InvokeAsync(_ => _.SelectShareActivityProductByIdAsync(ProductId,BatchGuid));
		///<summary>保养活动</summary>///<returns></returns>
        public OperationResult<BaoYangActivitySetting> SelectBaoYangActivitySetting(string activityId) => Invoke(_ => _.SelectBaoYangActivitySetting(activityId));

	///<summary>保养活动</summary>///<returns></returns>
        public Task<OperationResult<BaoYangActivitySetting>> SelectBaoYangActivitySettingAsync(string activityId) => InvokeAsync(_ => _.SelectBaoYangActivitySettingAsync(activityId));
		/// <summary></summary>///<returns></returns>
		[Obsolete("请使用SelectCouponActivityConfigNew")]
        public OperationResult<CouponActivityConfigModel> SelectCouponActivityConfig(string activityNum, int type) => Invoke(_ => _.SelectCouponActivityConfig(activityNum,type));

	/// <summary></summary>///<returns></returns>
		[Obsolete("请使用SelectCouponActivityConfigNew")]
        public Task<OperationResult<CouponActivityConfigModel>> SelectCouponActivityConfigAsync(string activityNum, int type) => InvokeAsync(_ => _.SelectCouponActivityConfigAsync(activityNum,type));
		///<summary>获取活动类型</summary>///<returns></returns>
        public OperationResult<IEnumerable<ActivityTypeModel>> SelectActivityTypeByActivityIds(List<Guid> activityIds) => Invoke(_ => _.SelectActivityTypeByActivityIds(activityIds));

	///<summary>获取活动类型</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<ActivityTypeModel>>> SelectActivityTypeByActivityIdsAsync(List<Guid> activityIds) => InvokeAsync(_ => _.SelectActivityTypeByActivityIdsAsync(activityIds));
		///<summary>根据搜索词获取活动配置</summary>
        public OperationResult<ActivityBuild> GetActivityBuildWithSelKey(string keyword) => Invoke(_ => _.GetActivityBuildWithSelKey(keyword));

	///<summary>根据搜索词获取活动配置</summary>
        public Task<OperationResult<ActivityBuild>> GetActivityBuildWithSelKeyAsync(string keyword) => InvokeAsync(_ => _.GetActivityBuildWithSelKeyAsync(keyword));
		///<summary>记录活动类型</summary>
        public OperationResult<bool> RecordActivityTypeLog(ActivityTypeRequest request) => Invoke(_ => _.RecordActivityTypeLog(request));

	///<summary>记录活动类型</summary>
        public Task<OperationResult<bool>> RecordActivityTypeLogAsync(ActivityTypeRequest request) => InvokeAsync(_ => _.RecordActivityTypeLogAsync(request));
		///<summary>更新保养活动配置</summary>
        public OperationResult<bool> UpdateBaoYangActivityConfig(Guid activityId) => Invoke(_ => _.UpdateBaoYangActivityConfig(activityId));

	///<summary>更新保养活动配置</summary>
        public Task<OperationResult<bool>> UpdateBaoYangActivityConfigAsync(Guid activityId) => InvokeAsync(_ => _.UpdateBaoYangActivityConfigAsync(activityId));
		///<summary>获取保养活动状态</summary>
        public OperationResult<FixedPriceActivityStatusResult> GetFixedPriceActivityStatus(Guid activityId, Guid userId, int regionId) => Invoke(_ => _.GetFixedPriceActivityStatus(activityId, userId, regionId));

	///<summary>获取保养活动状态</summary>
        public Task<OperationResult<FixedPriceActivityStatusResult>> GetFixedPriceActivityStatusAsync(Guid activityId, Guid userId, int regionId) => InvokeAsync(_ => _.GetFixedPriceActivityStatusAsync(activityId, userId, regionId));
		///<summary>重置活动计数器</summary>
        public OperationResult<bool> UpdateBaoYangPurchaseCount(Guid activityId) => Invoke(_ => _.UpdateBaoYangPurchaseCount(activityId));

	///<summary>重置活动计数器</summary>
        public Task<OperationResult<bool>> UpdateBaoYangPurchaseCountAsync(Guid activityId) => InvokeAsync(_ => _.UpdateBaoYangPurchaseCountAsync(activityId));
		///<summary>根据activityId获取活动配置</summary>
        public OperationResult<FixedPriceActivityRoundResponse> GetFixedPriceActivityRound(Guid activityId) => Invoke(_ => _.GetFixedPriceActivityRound(activityId));

	///<summary>根据activityId获取活动配置</summary>
        public Task<OperationResult<FixedPriceActivityRoundResponse>> GetFixedPriceActivityRoundAsync(Guid activityId) => InvokeAsync(_ => _.GetFixedPriceActivityRoundAsync(activityId));
		///<summary>根据activityId和RegionId获取活动配置</summary>
        public OperationResult<TiresActivityResponse> FetchRegionTiresActivity(FlashSaleTiresActivityRequest request) => Invoke(_ => _.FetchRegionTiresActivity(request));

	///<summary>根据activityId和RegionId获取活动配置</summary>
        public Task<OperationResult<TiresActivityResponse>> FetchRegionTiresActivityAsync(FlashSaleTiresActivityRequest request) => InvokeAsync(_ => _.FetchRegionTiresActivityAsync(request));
		///<summary>刷新轮胎活动缓存</summary>
        public OperationResult<bool> RefreshRegionTiresActivityCache(Guid activityId, int regionId) => Invoke(_ => _.RefreshRegionTiresActivityCache(activityId, regionId));

	///<summary>刷新轮胎活动缓存</summary>
        public Task<OperationResult<bool>> RefreshRegionTiresActivityCacheAsync(Guid activityId, int regionId) => InvokeAsync(_ => _.RefreshRegionTiresActivityCacheAsync(activityId, regionId));
		///<summary>记录用户点击活动开始提醒</summary>
        public OperationResult<bool> RecordActivityProductUserRemindLog(ActivityProductUserRemindRequest request) => Invoke(_ => _.RecordActivityProductUserRemindLog(request));

	///<summary>记录用户点击活动开始提醒</summary>
        public Task<OperationResult<bool>> RecordActivityProductUserRemindLogAsync(ActivityProductUserRemindRequest request) => InvokeAsync(_ => _.RecordActivityProductUserRemindLogAsync(request));
		///<summary>添加返现申请记录</summary>
        public OperationResult<bool> InsertRebateApplyRecord(RebateApplyRequest request) => Invoke(_ => _.InsertRebateApplyRecord(request));

	///<summary>添加返现申请记录</summary>
        public Task<OperationResult<bool>> InsertRebateApplyRecordAsync(RebateApplyRequest request) => InvokeAsync(_ => _.InsertRebateApplyRecordAsync(request));
		///<summary>初始化白名单数据或者后面个别用户白名单状态调整使用</summary>
        public OperationResult<bool> InsertOrUpdateActivityPageWhiteListRecords(List<ActivityPageWhiteListRequest> requests) => Invoke(_ => _.InsertOrUpdateActivityPageWhiteListRecords(requests));

	///<summary>初始化白名单数据或者后面个别用户白名单状态调整使用</summary>
        public Task<OperationResult<bool>> InsertOrUpdateActivityPageWhiteListRecordsAsync(List<ActivityPageWhiteListRequest> requests) => InvokeAsync(_ => _.InsertOrUpdateActivityPageWhiteListRecordsAsync(requests));
		///<summary>根据Userid判断是否是白名单用户</summary>
        public OperationResult<bool> GetActivityPageWhiteListByUserId(Guid userId) => Invoke(_ => _.GetActivityPageWhiteListByUserId(userId));

	///<summary>根据Userid判断是否是白名单用户</summary>
        public Task<OperationResult<bool>> GetActivityPageWhiteListByUserIdAsync(Guid userId) => InvokeAsync(_ => _.GetActivityPageWhiteListByUserIdAsync(userId));
		///<summary>记录途虎轮胎节用户申请信息接口</summary>
        public OperationResult<UserRewardApplicationResponse> PutUserRewardApplication(UserRewardApplicationRequest request) => Invoke(_ => _.PutUserRewardApplication(request));

	///<summary>记录途虎轮胎节用户申请信息接口</summary>
        public Task<OperationResult<UserRewardApplicationResponse>> PutUserRewardApplicationAsync(UserRewardApplicationRequest request) => InvokeAsync(_ => _.PutUserRewardApplicationAsync(request));
		///<summary>添加返现申请记录</summary>
        public OperationResult<ResultModel<bool>> InsertRebateApplyRecordNew(RebateApplyRequest request) => Invoke(_ => _.InsertRebateApplyRecordNew(request));

	///<summary>添加返现申请记录</summary>
        public Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordNewAsync(RebateApplyRequest request) => InvokeAsync(_ => _.InsertRebateApplyRecordNewAsync(request));
		///<summary>途虎贵就赔申请记录</summary>
        public OperationResult<bool> PutApplyCompensateRecord(ApplyCompensateRequest request) => Invoke(_ => _.PutApplyCompensateRecord(request));

	///<summary>途虎贵就赔申请记录</summary>
        public Task<OperationResult<bool>> PutApplyCompensateRecordAsync(ApplyCompensateRequest request) => InvokeAsync(_ => _.PutApplyCompensateRecordAsync(request));
		///<summary>批量活动有效性验证接口</summary>
        public OperationResult<List<ActivtyValidityResponse>> GetActivtyValidityResponses(ActivtyValidityRequest request) => Invoke(_ => _.GetActivtyValidityResponses(request));

	///<summary>批量活动有效性验证接口</summary>
        public Task<OperationResult<List<ActivtyValidityResponse>>> GetActivtyValidityResponsesAsync(ActivtyValidityRequest request) => InvokeAsync(_ => _.GetActivtyValidityResponsesAsync(request));
		///<summary>获取预付卡场次信息</summary>
        public OperationResult<List<VipCardSaleConfigDetailModel>> GetVipCardSaleConfigDetails(string activityId) => Invoke(_ => _.GetVipCardSaleConfigDetails(activityId));

	///<summary>获取预付卡场次信息</summary>
        public Task<OperationResult<List<VipCardSaleConfigDetailModel>>> GetVipCardSaleConfigDetailsAsync(string activityId) => InvokeAsync(_ => _.GetVipCardSaleConfigDetailsAsync(activityId));
		///<summary>check购买的批次是否还有剩余库存</summary>
        public OperationResult<Dictionary<string, bool>> VipCardCheckStock(List<string> batchIds) => Invoke(_ => _.VipCardCheckStock(batchIds));

	///<summary>check购买的批次是否还有剩余库存</summary>
        public Task<OperationResult<Dictionary<string, bool>>> VipCardCheckStockAsync(List<string> batchIds) => InvokeAsync(_ => _.VipCardCheckStockAsync(batchIds));
		///<summary>创建订单时记录卡信息</summary>
        public OperationResult<bool> PutVipCardRecord(VipCardRecordRequest request) => Invoke(_ => _.PutVipCardRecord(request));

	///<summary>创建订单时记录卡信息</summary>
        public Task<OperationResult<bool>> PutVipCardRecordAsync(VipCardRecordRequest request) => InvokeAsync(_ => _.PutVipCardRecordAsync(request));
		///<summary>支付成功时调用绑卡</summary>
        public OperationResult<bool> BindVipCard(int orderId) => Invoke(_ => _.BindVipCard(orderId));

	///<summary>支付成功时调用绑卡</summary>
        public Task<OperationResult<bool>> BindVipCardAsync(int orderId) => InvokeAsync(_ => _.BindVipCardAsync(orderId));
		///<summary>获取返现申请页面配置</summary>
        public OperationResult<RebateApplyPageConfig> SelectRebateApplyPageConfig() => Invoke(_ => _.SelectRebateApplyPageConfig());

	///<summary>获取返现申请页面配置</summary>
        public Task<OperationResult<RebateApplyPageConfig>> SelectRebateApplyPageConfigAsync() => InvokeAsync(_ => _.SelectRebateApplyPageConfigAsync());
		///<summary>申请返现</summary>
        public OperationResult<ResultModel<bool>> InsertRebateApplyRecordV2(RebateApplyRequest request) => Invoke(_ => _.InsertRebateApplyRecordV2(request));

	///<summary>申请返现</summary>
        public Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordV2Async(RebateApplyRequest request) => InvokeAsync(_ => _.InsertRebateApplyRecordV2Async(request));
		///<summary>获取用户所有返现申请信息</summary>
        public OperationResult<List<RebateApplyResponse>> SelectRebateApplyByOpenId(string openId) => Invoke(_ => _.SelectRebateApplyByOpenId(openId));

	///<summary>获取用户所有返现申请信息</summary>
        public Task<OperationResult<List<RebateApplyResponse>>> SelectRebateApplyByOpenIdAsync(string openId) => InvokeAsync(_ => _.SelectRebateApplyByOpenIdAsync(openId));
		///<summary>通过OrderId获取最新的申请记录</summary>
        public OperationResult<IEnumerable<RebateApplyResponse>> GetRebateApplyByOrderId(int orderId) => Invoke(_ => _.GetRebateApplyByOrderId(orderId));

	///<summary>通过OrderId获取最新的申请记录</summary>
        public Task<OperationResult<IEnumerable<RebateApplyResponse>>> GetRebateApplyByOrderIdAsync(int orderId) => InvokeAsync(_ => _.GetRebateApplyByOrderIdAsync(orderId));
		///<summary>取消订单的时候更新db跟缓存中的数据</summary>
        public OperationResult<bool> ModifyVipCardRecordByOrderId(int orderId) => Invoke(_ => _.ModifyVipCardRecordByOrderId(orderId));

	///<summary>取消订单的时候更新db跟缓存中的数据</summary>
        public Task<OperationResult<bool>> ModifyVipCardRecordByOrderIdAsync(int orderId) => InvokeAsync(_ => _.ModifyVipCardRecordByOrderIdAsync(orderId));
		///<summary>获取2018世界杯的活动对象和积分规则信息</summary>
        public OperationResult<ActivityResponse> GetWorldCup2018Activity() => Invoke(_ => _.GetWorldCup2018Activity( ));

	///<summary>获取2018世界杯的活动对象和积分规则信息</summary>
        public Task<OperationResult<ActivityResponse>> GetWorldCup2018ActivityAsync() => InvokeAsync(_ => _.GetWorldCup2018ActivityAsync( ));
		///<summary>通过用户ID获取兑换券数量接口</summary>
        public OperationResult<int> GetCouponCountByUserId(Guid userId, long activityId) => Invoke(_ => _.GetCouponCountByUserId(userId,activityId));

	///<summary>通过用户ID获取兑换券数量接口</summary>
        public Task<OperationResult<int>> GetCouponCountByUserIdAsync(Guid userId, long activityId) => InvokeAsync(_ => _.GetCouponCountByUserIdAsync(userId,activityId));
		///<summary>返回活动兑换券排行排名</summary>
        public OperationResult<PagedModel<ActivityCouponRankResponse>> SearchCouponRank(long activityId, int pageIndex = 1, int pageSize = 20) => Invoke(_ => _.SearchCouponRank( activityId,pageIndex,pageSize ));

	///<summary>返回活动兑换券排行排名</summary>
        public Task<OperationResult<PagedModel<ActivityCouponRankResponse>>> SearchCouponRankAsync(long activityId, int pageIndex = 1, int pageSize = 20) => InvokeAsync(_ => _.SearchCouponRankAsync( activityId,pageIndex,pageSize ));
		///<summary>返回用户的兑换券排名情况</summary>
        public OperationResult<ActivityCouponRankResponse> GetUserCouponRank(Guid userId, long activityId) => Invoke(_ => _.GetUserCouponRank( userId,activityId ));

	///<summary>返回用户的兑换券排名情况</summary>
        public Task<OperationResult<ActivityCouponRankResponse>> GetUserCouponRankAsync(Guid userId, long activityId) => InvokeAsync(_ => _.GetUserCouponRankAsync( userId,activityId ));
		///<summary>兑换物列表</summary>
        public OperationResult<PagedModel<ActivityPrizeResponse>> SearchPrizeList(SearchPrizeListRequest searchPrizeListRequest) => Invoke(_ => _.SearchPrizeList( searchPrizeListRequest ));

	///<summary>兑换物列表</summary>
        public Task<OperationResult<PagedModel<ActivityPrizeResponse>>> SearchPrizeListAsync(SearchPrizeListRequest searchPrizeListRequest) => InvokeAsync(_ => _.SearchPrizeListAsync( searchPrizeListRequest ));
		///<summary>用户兑换奖品 异常代码：-1 系统异常（请重试） , -2 兑换卡不足  -3 库存不足  -4 已经下架   -5 已经兑换   -6 兑换时间已经截止不能兑换</summary>
        public OperationResult<bool> UserRedeemPrizes(Guid userId, long prizeId,long activityId) => Invoke(_ => _.UserRedeemPrizes( userId,prizeId,activityId ));

	///<summary>用户兑换奖品 异常代码：-1 系统异常（请重试） , -2 兑换卡不足  -3 库存不足  -4 已经下架   -5 已经兑换   -6 兑换时间已经截止不能兑换</summary>
        public Task<OperationResult<bool>> UserRedeemPrizesAsync(Guid userId, long prizeId,long activityId) => InvokeAsync(_ => _.UserRedeemPrizesAsync( userId,prizeId,activityId ));
		///<summary>用户已兑换商品列表</summary>
        public OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>> SearchPrizeOrderDetailListByUserId(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20) => Invoke(_ => _.SearchPrizeOrderDetailListByUserId( userId,activityId,pageIndex,pageSize ));

	///<summary>用户已兑换商品列表</summary>
        public Task<OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>>> SearchPrizeOrderDetailListByUserIdAsync(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20) => InvokeAsync(_ => _.SearchPrizeOrderDetailListByUserIdAsync( userId,activityId,pageIndex,pageSize ));
		///<summary>今日竞猜题目</summary>
        public OperationResult<IEnumerable<Models.Response.Question>> SearchQuestion( Guid userId , long activityId) => Invoke(_ => _.SearchQuestion( userId,activityId ));

	///<summary>今日竞猜题目</summary>
        public Task<OperationResult<IEnumerable<Models.Response.Question>>> SearchQuestionAsync( Guid userId , long activityId) => InvokeAsync(_ => _.SearchQuestionAsync( userId,activityId ));
		///<summary>提交用户竞猜</summary>
        public OperationResult<bool> SubmitQuestionAnswer(SubmitQuestionAnswerRequest submitQuestionAnswerRequest) => Invoke(_ => _.SubmitQuestionAnswer( submitQuestionAnswerRequest ));

	///<summary>提交用户竞猜</summary>
        public Task<OperationResult<bool>> SubmitQuestionAnswerAsync(SubmitQuestionAnswerRequest submitQuestionAnswerRequest) => InvokeAsync(_ => _.SubmitQuestionAnswerAsync( submitQuestionAnswerRequest ));
		///<summary>返回用户答题历史</summary>
        public OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>> SearchQuestionAnswerHistoryByUserId(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest) => Invoke(_ => _.SearchQuestionAnswerHistoryByUserId( searchQuestionAnswerHistoryRequest ));

	///<summary>返回用户答题历史</summary>
        public Task<OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>>> SearchQuestionAnswerHistoryByUserIdAsync(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest) => InvokeAsync(_ => _.SearchQuestionAnswerHistoryByUserIdAsync( searchQuestionAnswerHistoryRequest ));
		///<summary>返回用户胜利次数和胜利称号</summary>
        public OperationResult<ActivityVictoryInfoResponse> GetVictoryInfo(Guid userId, long activityId) => Invoke(_ => _.GetVictoryInfo( userId,activityId ));

	///<summary>返回用户胜利次数和胜利称号</summary>
        public Task<OperationResult<ActivityVictoryInfoResponse>> GetVictoryInfoAsync(Guid userId, long activityId) => InvokeAsync(_ => _.GetVictoryInfoAsync( userId,activityId ));
		///<summary>活动分享赠送积分  异常：   -77 活动未开始  -2 今日已经分享   -1 系统异常</summary>
        public OperationResult<bool> ActivityShare(ActivityShareDetailRequest shareDetailRequest) => Invoke(_ => _.ActivityShare( shareDetailRequest ));

	///<summary>活动分享赠送积分  异常：   -77 活动未开始  -2 今日已经分享   -1 系统异常</summary>
        public Task<OperationResult<bool>> ActivityShareAsync(ActivityShareDetailRequest shareDetailRequest) => InvokeAsync(_ => _.ActivityShareAsync( shareDetailRequest ));
		///<summary>今日是否已经分享了 true = 今日已经分享</summary>
        public OperationResult<bool> ActivityTodayAlreadyShare(Guid userId, long activityId) => Invoke(_ => _.ActivityTodayAlreadyShare( userId, activityId));

	///<summary>今日是否已经分享了 true = 今日已经分享</summary>
        public Task<OperationResult<bool>> ActivityTodayAlreadyShareAsync(Guid userId, long activityId) => InvokeAsync(_ => _.ActivityTodayAlreadyShareAsync( userId, activityId));
		///<summary>用来获取或者刷新排序好的车型适配商品</summary>
        public OperationResult<List<string>> GetOrSetActivityPageSortedPids(SortedPidsRequest request) => Invoke(_ => _.GetOrSetActivityPageSortedPids(request));

	///<summary>用来获取或者刷新排序好的车型适配商品</summary>
        public Task<OperationResult<List<string>>> GetOrSetActivityPageSortedPidsAsync(SortedPidsRequest request) => InvokeAsync(_ => _.GetOrSetActivityPageSortedPidsAsync(request));
		///<summary>修改或者增加用户兑换券 并且增加日志  返回主键</summary>
        public OperationResult<long> ModifyActivityCoupon(Guid userId, long activityId, int couponCount , string couponName,DateTime? modifyDateTime = null) => Invoke(_ => _.ModifyActivityCoupon( userId, activityId,couponCount,couponName,modifyDateTime));

	///<summary>修改或者增加用户兑换券 并且增加日志  返回主键</summary>
        public Task<OperationResult<long>> ModifyActivityCouponAsync(Guid userId, long activityId, int couponCount , string couponName,DateTime? modifyDateTime = null) => InvokeAsync(_ => _.ModifyActivityCouponAsync( userId, activityId,couponCount,couponName,modifyDateTime));
		///<summary>刷新活动题目  缓存</summary>
        public OperationResult<bool> RefreshActivityQuestionCache(long activityId) => Invoke(_ => _.RefreshActivityQuestionCache( activityId));

	///<summary>刷新活动题目  缓存</summary>
        public Task<OperationResult<bool>> RefreshActivityQuestionCacheAsync(long activityId) => InvokeAsync(_ => _.RefreshActivityQuestionCacheAsync( activityId));
		///<summary>刷新活动兑换物  缓存</summary>
        public OperationResult<bool> RefreshActivityPrizeCache(long activityId) => Invoke(_ => _.RefreshActivityPrizeCache( activityId));

	///<summary>刷新活动兑换物  缓存</summary>
        public Task<OperationResult<bool>> RefreshActivityPrizeCacheAsync(long activityId) => InvokeAsync(_ => _.RefreshActivityPrizeCacheAsync( activityId));
		///<summary>保存用户答题数据到数据库</summary>
        public OperationResult<SubmitActivityQuestionUserAnswerResponse> SubmitQuestionUserAnswer(SubmitActivityQuestionUserAnswerRequest request) => Invoke(_ => _.SubmitQuestionUserAnswer( request));

	///<summary>保存用户答题数据到数据库</summary>
        public Task<OperationResult<SubmitActivityQuestionUserAnswerResponse>> SubmitQuestionUserAnswerAsync(SubmitActivityQuestionUserAnswerRequest request) => InvokeAsync(_ => _.SubmitQuestionUserAnswerAsync( request));
		///<summary>更新用户答题结果状态</summary>
        public OperationResult<ModifyQuestionUserAnswerResultResponse> ModifyQuestionUserAnswerResult(ModifyQuestionUserAnswerResultRequest request) => Invoke(_ => _.ModifyQuestionUserAnswerResult( request));

	///<summary>更新用户答题结果状态</summary>
        public Task<OperationResult<ModifyQuestionUserAnswerResultResponse>> ModifyQuestionUserAnswerResultAsync(ModifyQuestionUserAnswerResultRequest request) => InvokeAsync(_ => _.ModifyQuestionUserAnswerResultAsync( request));
		///<summary>新活动页配置</summary>
        public OperationResult<ActivityPageInfoModel> GetActivityPageInfoConfigModel(ActivityPageInfoRequest request) => Invoke(_ => _.GetActivityPageInfoConfigModel( request));

	///<summary>新活动页配置</summary>
        public Task<OperationResult<ActivityPageInfoModel>> GetActivityPageInfoConfigModelAsync(ActivityPageInfoRequest request) => InvokeAsync(_ => _.GetActivityPageInfoConfigModelAsync( request));
		///<summary>活动页会场配置</summary>
        public OperationResult<List<ActivityPageInfoHomeModel>> GetActivityPageInfoHomeModels(string hashKey) => Invoke(_ => _.GetActivityPageInfoHomeModels( hashKey));

	///<summary>活动页会场配置</summary>
        public Task<OperationResult<List<ActivityPageInfoHomeModel>>> GetActivityPageInfoHomeModelsAsync(string hashKey) => InvokeAsync(_ => _.GetActivityPageInfoHomeModelsAsync( hashKey));
		///<summary>活动页车品推荐</summary>
        public OperationResult<List<ActivityPageInfoRecommend>> GetActivityPageInfoCpRecommends(ActivityPageInfoModuleRecommendRequest request) => Invoke(_ => _.GetActivityPageInfoCpRecommends( request));

	///<summary>活动页车品推荐</summary>
        public Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoCpRecommendsAsync(ActivityPageInfoModuleRecommendRequest request) => InvokeAsync(_ => _.GetActivityPageInfoCpRecommendsAsync( request));
		///<summary>活动页轮胎推荐</summary>
        public OperationResult<List<ActivityPageInfoRecommend>> GetActivityPageInfoTireRecommends(ActivityPageInfoModuleRecommendRequest request) => Invoke(_ => _.GetActivityPageInfoTireRecommends( request));

	///<summary>活动页轮胎推荐</summary>
        public Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoTireRecommendsAsync(ActivityPageInfoModuleRecommendRequest request) => InvokeAsync(_ => _.GetActivityPageInfoTireRecommendsAsync( request));
		///<summary>活动页菜单配置</summary>
        public OperationResult<List<ActivityPageInfoRowMenuModel>> GetActivityPageInfoRowMenus(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowMenus( request));

	///<summary>活动页菜单配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowMenuModel>>> GetActivityPageInfoRowMenusAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowMenusAsync( request));
		///<summary>活动页老商品池配置</summary>
        public OperationResult<List<ActivityPageInfoRowProductPool>> GetActivityPageInfoRowPools(ActivityPageInfoModuleProductPoolRequest request) => Invoke(_ => _.GetActivityPageInfoRowPools( request));

	///<summary>活动页老商品池配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowProductPool>>> GetActivityPageInfoRowPoolsAsync(ActivityPageInfoModuleProductPoolRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowPoolsAsync( request));
		///<summary>活动页新商品池配置</summary>
        public OperationResult<List<ActivityPageInfoRowNewProductPool>> GetActivityPageInfoRowNewPools(ActivityPageInfoModuleNewProductPoolRequest request) => Invoke(_ => _.GetActivityPageInfoRowNewPools( request));

	///<summary>活动页新商品池配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowNewProductPool>>> GetActivityPageInfoRowNewPoolsAsync(ActivityPageInfoModuleNewProductPoolRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowNewPoolsAsync( request));
		///<summary>活动页倒计时配置</summary>
        public OperationResult<List<ActivityPageInfoRowCountDown>> GetActivityPageInfoRowCountDowns(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowCountDowns( request));

	///<summary>活动页倒计时配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowCountDown>>> GetActivityPageInfoRowCountDownsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowCountDownsAsync( request));
		///<summary>活动页文案配置</summary>
        public OperationResult<List<ActivityPageInfoRowActivityText>> GetActivityPageInfoRowActivityTexts(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowActivityTexts( request));

	///<summary>活动页文案配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowActivityText>>> GetActivityPageInfoRowActivityTextsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowActivityTextsAsync( request));
		///<summary>活动页文字链，滑动优惠券配置</summary>
        public OperationResult<List<ActivityPageInfoRowJson>> GetActivityPageInfoRowJsons(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowJsons( request));

	///<summary>活动页文字链，滑动优惠券配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowJson>>> GetActivityPageInfoRowJsonsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowJsonsAsync( request));
		///<summary>活动页拼团配置</summary>
        public OperationResult<List<ActivityPageInfoRowPintuan>> GetActivityPageInfoRowPintuans(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowPintuans( request));

	///<summary>活动页拼团配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowPintuan>>> GetActivityPageInfoRowPintuansAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowPintuansAsync( request));
		///<summary>活动页视频配置</summary>
        public OperationResult<List<ActivityPageInfoRowVideo>> GetActivityPageInfoRowVideos(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowVideos( request));

	///<summary>活动页视频配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowVideo>>> GetActivityPageInfoRowVideosAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowVideosAsync( request));
		///<summary>活动页其他活动配置</summary>
        public OperationResult<List<ActivityPageInfoRowOtherActivity>> GetActivityPageInfoRowOtherActivitys(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowOtherActivitys( request));

	///<summary>活动页其他活动配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowOtherActivity>>> GetActivityPageInfoRowOtherActivitysAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowOtherActivitysAsync( request));
		///<summary>活动页其他配置</summary>
        public OperationResult<List<ActivityPageInfoRowOther>> GetActivityPageInfoRowOthers(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowOthers( request));

	///<summary>活动页其他配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowOther>>> GetActivityPageInfoRowOthersAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowOthersAsync( request));
		///<summary>活动页活动规则配置</summary>
        public OperationResult<List<ActivityPageInfoRowRule>> GetActivityPageInfoRowRules(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowRules( request));

	///<summary>活动页活动规则配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowRule>>> GetActivityPageInfoRowRulesAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowRulesAsync( request));
		///<summary>活动页保养配置</summary>
        public OperationResult<List<ActivityPageInfoRowBy>> GetActivityPageInfoRowBys(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowBys( request));

	///<summary>活动页保养配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowBy>>> GetActivityPageInfoRowBysAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowBysAsync( request));
		///<summary>活动页优惠券配置</summary>
        public OperationResult<List<ActivityPageInfoRowCoupon>> GetActivityPageInfoRowCoupons(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowCoupons( request));

	///<summary>活动页优惠券配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowCoupon>>> GetActivityPageInfoRowCouponsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowCouponsAsync( request));
		///<summary>活动页产品配置</summary>
        public OperationResult<List<ActivityPageInfoRowProduct>> GetActivityPageInfoRowProducts(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowProducts( request));

	///<summary>活动页产品配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowProduct>>> GetActivityPageInfoRowProductsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowProductsAsync( request));
		///<summary>活动页链接配置</summary>
        public OperationResult<List<ActivityPageInfoRowLink>> GetActivityPageInfoRowLinks(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowLinks( request));

	///<summary>活动页链接配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowLink>>> GetActivityPageInfoRowLinksAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowLinksAsync( request));
		///<summary>活动页图片配置</summary>
        public OperationResult<List<ActivityPageInfoRowImage>> GetActivityPageInfoRowImages(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowImages( request));

	///<summary>活动页图片配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowImage>>> GetActivityPageInfoRowImagesAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowImagesAsync( request));
		///<summary>活动页秒杀配置</summary>
        public OperationResult<List<ActivityPageInfoRowSeckill>> GetActivityPageInfoRowSeckills(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowSeckills( request));

	///<summary>活动页秒杀配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowSeckill>>> GetActivityPageInfoRowSeckillsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowSeckillsAsync( request));
		///<summary>活动页头图配置</summary>
        public OperationResult<List<ActivityPageInfoRowVehicleBanner>> GetActivityPageInfoRowVehicleBanners(ActivityPageInfoModuleVehicleBannerRequest request) => Invoke(_ => _.GetActivityPageInfoRowVehicleBanners( request));

	///<summary>活动页头图配置</summary>
        public Task<OperationResult<List<ActivityPageInfoRowVehicleBanner>>> GetActivityPageInfoRowVehicleBannersAsync(ActivityPageInfoModuleVehicleBannerRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowVehicleBannersAsync( request));
		///<summary>通过活动类型获取活动 0 世界杯  1 拼团车型认证</summary>
        public OperationResult<ActivityResponse> GetActivityInfoByType(int activityTypeId) => Invoke(_ => _.GetActivityInfoByType(activityTypeId));

	///<summary>通过活动类型获取活动 0 世界杯  1 拼团车型认证</summary>
        public Task<OperationResult<ActivityResponse>> GetActivityInfoByTypeAsync(int activityTypeId) => InvokeAsync(_ => _.GetActivityInfoByTypeAsync(activityTypeId));
		///<summary>根据活动编号查询配置信息</summary>
        public OperationResult<ActiveCustomerSettingResponse> GetCustomerSettingInfo(string activeNo) => Invoke(_ => _.GetCustomerSettingInfo(activeNo));

	///<summary>根据活动编号查询配置信息</summary>
        public Task<OperationResult<ActiveCustomerSettingResponse>> GetCustomerSettingInfoAsync(string activeNo) => InvokeAsync(_ => _.GetCustomerSettingInfoAsync(activeNo));
		///<summary>查询用户绑定的券码</summary>
        public OperationResult<string> GetUserCouponCode(string activityExclusiveId, string userid) => Invoke(_ => _.GetUserCouponCode(activityExclusiveId,userid));

	///<summary>查询用户绑定的券码</summary>
        public Task<OperationResult<string>> GetUserCouponCodeAsync(string activityExclusiveId, string userid) => InvokeAsync(_ => _.GetUserCouponCodeAsync(activityExclusiveId,userid));
		///<summary>用户券码绑定</summary>
        public OperationResult<bool> CouponCodeBound(ActivityCustomerCouponRequests activityCustomerCouponRequests) => Invoke(_ => _.CouponCodeBound(activityCustomerCouponRequests));

	///<summary>用户券码绑定</summary>
        public Task<OperationResult<bool>> CouponCodeBoundAsync(ActivityCustomerCouponRequests activityCustomerCouponRequests) => InvokeAsync(_ => _.CouponCodeBoundAsync(activityCustomerCouponRequests));
		///<summary>客户专享活动下单验证</summary>
        public OperationResult<bool> ActiveOrderVerify(ActivityOrderVerifyRequests activityOrderVerifyRequests) => Invoke(_ => _.ActiveOrderVerify(activityOrderVerifyRequests));

	///<summary>客户专享活动下单验证</summary>
        public Task<OperationResult<bool>> ActiveOrderVerifyAsync(ActivityOrderVerifyRequests activityOrderVerifyRequests) => InvokeAsync(_ => _.ActiveOrderVerifyAsync(activityOrderVerifyRequests));
		///<summary>H5 添加途虎星级门店认证信息服务</summary>
        public OperationResult<bool> AddStarRatingStore(AddStarRatingStoreRequest request) => Invoke(_ => _.AddStarRatingStore( request));

	///<summary>H5 添加途虎星级门店认证信息服务</summary>
        public Task<OperationResult<bool>> AddStarRatingStoreAsync(AddStarRatingStoreRequest request) => InvokeAsync(_ => _.AddStarRatingStoreAsync( request));
		///<summary>根据限时抢购ID查询大客户活动配置信息</summary>
        public OperationResult<ActiveCustomerSettingResponse> GetVipCustomerSettingInfoByActivityId(string activityId) => Invoke(_ => _.GetVipCustomerSettingInfoByActivityId(activityId));

	///<summary>根据限时抢购ID查询大客户活动配置信息</summary>
        public Task<OperationResult<ActiveCustomerSettingResponse>> GetVipCustomerSettingInfoByActivityIdAsync(string activityId) => InvokeAsync(_ => _.GetVipCustomerSettingInfoByActivityIdAsync(activityId));
		///<summary>获取大客户订单中员工胎购买异常行为</summary>
        public OperationResult<List<ActivityCustomerInvalidOrderResponse>> GetExceptionalCustomerOrderInfo() => Invoke(_ => _.GetExceptionalCustomerOrderInfo());

	///<summary>获取大客户订单中员工胎购买异常行为</summary>
        public Task<OperationResult<List<ActivityCustomerInvalidOrderResponse>>> GetExceptionalCustomerOrderInfoAsync() => InvokeAsync(_ => _.GetExceptionalCustomerOrderInfoAsync());
		///<summary>获取蓄电池/加油卡活动配置</summary>///<returns></returns>
        public OperationResult<CouponActivityConfigNewModel> SelectCouponActivityConfigNew(CouponActivityConfigRequest request) => Invoke(_ => _.SelectCouponActivityConfigNew(request));

	///<summary>获取蓄电池/加油卡活动配置</summary>///<returns></returns>
        public Task<OperationResult<CouponActivityConfigNewModel>> SelectCouponActivityConfigNewAsync(CouponActivityConfigRequest request) => InvokeAsync(_ => _.SelectCouponActivityConfigNewAsync(request));
		///<summary>活动服务接口测试</summary>
        public OperationResult<string> ActivityTestMethod(int testType) => Invoke(_ => _.ActivityTestMethod(testType));

	///<summary>活动服务接口测试</summary>
        public Task<OperationResult<string>> ActivityTestMethodAsync(int testType) => InvokeAsync(_ => _.ActivityTestMethodAsync(testType));
		///<summary>添加活动报名页数据</summary>
        public OperationResult<bool> AddRegistrationOfActivitiesData(RegistrationOfActivitiesRequest request) => Invoke(_ => _.AddRegistrationOfActivitiesData(request));

	///<summary>添加活动报名页数据</summary>
        public Task<OperationResult<bool>> AddRegistrationOfActivitiesDataAsync(RegistrationOfActivitiesRequest request) => InvokeAsync(_ => _.AddRegistrationOfActivitiesDataAsync(request));
		///<summary>佣金商品列表查询接口</summary>
        public OperationResult<List<CommissionProductModel>> GetCommissionProductList(GetCommissionProductListRequest request) => Invoke(_ => _.GetCommissionProductList(request));

	///<summary>佣金商品列表查询接口</summary>
        public Task<OperationResult<List<CommissionProductModel>>> GetCommissionProductListAsync(GetCommissionProductListRequest request) => InvokeAsync(_ => _.GetCommissionProductListAsync(request));
		///<summary>佣金商品详情查询接口</summary>
        public OperationResult<CommissionProductModel> GetCommissionProductDetatils(GetCommissionProductDetatilsRequest request) => Invoke(_ => _.GetCommissionProductDetatils(request));

	///<summary>佣金商品详情查询接口</summary>
        public Task<OperationResult<CommissionProductModel>> GetCommissionProductDetatilsAsync(GetCommissionProductDetatilsRequest request) => InvokeAsync(_ => _.GetCommissionProductDetatilsAsync(request));
		///<summary>下单商品记录接口</summary>
        public OperationResult<CreateOrderItemRecordResponse> CreateOrderItemRecord(CreateOrderItemRecordRequest request) => Invoke(_ => _.CreateOrderItemRecord(request));

	///<summary>下单商品记录接口</summary>
        public Task<OperationResult<CreateOrderItemRecordResponse>> CreateOrderItemRecordAsync(CreateOrderItemRecordRequest request) => InvokeAsync(_ => _.CreateOrderItemRecordAsync(request));
		///<summary>佣金订单商品记录更新接口</summary>
        public OperationResult<UpdateOrderItemRecordResponse> UpdateOrderItemRecord(UpdateOrderItemRecordRequest request) => Invoke(_ => _.UpdateOrderItemRecord(request));

	///<summary>佣金订单商品记录更新接口</summary>
        public Task<OperationResult<UpdateOrderItemRecordResponse>> UpdateOrderItemRecordAsync(UpdateOrderItemRecordRequest request) => InvokeAsync(_ => _.UpdateOrderItemRecordAsync(request));
		///<summary>订单商品返佣接口</summary>
        public OperationResult<CommodityRebateResponse> CommodityRebate(CommodityRebateRequest request) => Invoke(_ => _.CommodityRebate(request));

	///<summary>订单商品返佣接口</summary>
        public Task<OperationResult<CommodityRebateResponse>> CommodityRebateAsync(CommodityRebateRequest request) => InvokeAsync(_ => _.CommodityRebateAsync(request));
		///<summary>订单商品扣佣接口</summary>
        public OperationResult<CommodityDeductionResponse> CommodityDeduction(CommodityDeductionRequest request) => Invoke(_ => _.CommodityDeduction(request));

	///<summary>订单商品扣佣接口</summary>
        public Task<OperationResult<CommodityDeductionResponse>> CommodityDeductionAsync(CommodityDeductionRequest request) => InvokeAsync(_ => _.CommodityDeductionAsync(request));
		///<summary>CPS修改佣金流水状态</summary>
        public OperationResult<CpsUpdateRunningResponse> CpsUpdateRunning(CpsUpdateRunningRequest request) => Invoke(_ => _.CpsUpdateRunning(request));

	///<summary>CPS修改佣金流水状态</summary>
        public Task<OperationResult<CpsUpdateRunningResponse>> CpsUpdateRunningAsync(CpsUpdateRunningRequest request) => InvokeAsync(_ => _.CpsUpdateRunningAsync(request));
	}
	/// <summary>缓存相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICacheService
    {
    	///<summary>清理活动站点的Redis缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RemoveRedisCacheKey", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RemoveRedisCacheKeyResponse")]
        Task<OperationResult<bool>> RemoveRedisCacheKeyAsync(string cacheName,string cacheKey,string prefixKey=null);
		///<summary>根据活动id重新setredis缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshVipCardCacheByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshVipCardCacheByActivityIdResponse")]
        Task<OperationResult<bool>> RefreshVipCardCacheByActivityIdAsync(string activityId);
		///<summary>刷新缓存前缀接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCachePrefixForCommon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCachePrefixForCommonResponse")]
        Task<OperationResult<bool>> RefreshRedisCachePrefixForCommonAsync(RefreshCachePrefixRequest request);
		///<summary>刷新客户专享配置缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCacheCustomerSetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCacheCustomerSettingResponse")]
        Task<OperationResult<bool>> RefreshRedisCacheCustomerSettingAsync(string activityExclusiveId);
	}

	/// <summary>缓存相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICacheClient : ICacheService, ITuhuServiceClient
    {
    	///<summary>清理活动站点的Redis缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RemoveRedisCacheKey", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RemoveRedisCacheKeyResponse")]
        OperationResult<bool> RemoveRedisCacheKey(string cacheName,string cacheKey,string prefixKey=null);
		///<summary>根据活动id重新setredis缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshVipCardCacheByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshVipCardCacheByActivityIdResponse")]
        OperationResult<bool> RefreshVipCardCacheByActivityId(string activityId);
		///<summary>刷新缓存前缀接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCachePrefixForCommon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCachePrefixForCommonResponse")]
        OperationResult<bool> RefreshRedisCachePrefixForCommon(RefreshCachePrefixRequest request);
		///<summary>刷新客户专享配置缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCacheCustomerSetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCacheCustomerSettingResponse")]
        OperationResult<bool> RefreshRedisCacheCustomerSetting(string activityExclusiveId);
	}

	/// <summary>缓存相关</summary>
	public partial class CacheClient : TuhuServiceClient<ICacheClient>, ICacheClient
    {
    	///<summary>清理活动站点的Redis缓存</summary>
        public OperationResult<bool> RemoveRedisCacheKey(string cacheName,string cacheKey,string prefixKey=null) => Invoke(_ => _.RemoveRedisCacheKey(cacheName,cacheKey,prefixKey));

	///<summary>清理活动站点的Redis缓存</summary>
        public Task<OperationResult<bool>> RemoveRedisCacheKeyAsync(string cacheName,string cacheKey,string prefixKey=null) => InvokeAsync(_ => _.RemoveRedisCacheKeyAsync(cacheName,cacheKey,prefixKey));
		///<summary>根据活动id重新setredis缓存</summary>
        public OperationResult<bool> RefreshVipCardCacheByActivityId(string activityId) => Invoke(_ => _.RefreshVipCardCacheByActivityId(activityId));

	///<summary>根据活动id重新setredis缓存</summary>
        public Task<OperationResult<bool>> RefreshVipCardCacheByActivityIdAsync(string activityId) => InvokeAsync(_ => _.RefreshVipCardCacheByActivityIdAsync(activityId));
		///<summary>刷新缓存前缀接口</summary>
        public OperationResult<bool> RefreshRedisCachePrefixForCommon(RefreshCachePrefixRequest request) => Invoke(_ => _.RefreshRedisCachePrefixForCommon(request));

	///<summary>刷新缓存前缀接口</summary>
        public Task<OperationResult<bool>> RefreshRedisCachePrefixForCommonAsync(RefreshCachePrefixRequest request) => InvokeAsync(_ => _.RefreshRedisCachePrefixForCommonAsync(request));
		///<summary>刷新客户专享配置缓存</summary>
        public OperationResult<bool> RefreshRedisCacheCustomerSetting(string activityExclusiveId) => Invoke(_ => _.RefreshRedisCacheCustomerSetting(activityExclusiveId));

	///<summary>刷新客户专享配置缓存</summary>
        public Task<OperationResult<bool>> RefreshRedisCacheCustomerSettingAsync(string activityExclusiveId) => InvokeAsync(_ => _.RefreshRedisCacheCustomerSettingAsync(activityExclusiveId));
	}
	/// <summary>文章相相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IArticleService
    {
    	///<summary>查询关注列表数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Article/SelectDiscoveryHome", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Article/SelectDiscoveryHomeResponse")]
        Task<OperationResult<PagedModel<HomePageTimeLineRequestModel>>> SelectDiscoveryHomeAsync(string userId,PagerModel page,int version);
	}

	/// <summary>文章相相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IArticleClient : IArticleService, ITuhuServiceClient
    {
    	///<summary>查询关注列表数据</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Article/SelectDiscoveryHome", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Article/SelectDiscoveryHomeResponse")]
        OperationResult<PagedModel<HomePageTimeLineRequestModel>> SelectDiscoveryHome(string userId,PagerModel page,int version);
	}

	/// <summary>文章相相关</summary>
	public partial class ArticleClient : TuhuServiceClient<IArticleClient>, IArticleClient
    {
    	///<summary>查询关注列表数据</summary>
        public OperationResult<PagedModel<HomePageTimeLineRequestModel>> SelectDiscoveryHome(string userId,PagerModel page,int version) => Invoke(_ => _.SelectDiscoveryHome(userId,page,version));

	///<summary>查询关注列表数据</summary>
        public Task<OperationResult<PagedModel<HomePageTimeLineRequestModel>>> SelectDiscoveryHomeAsync(string userId,PagerModel page,int version) => InvokeAsync(_ => _.SelectDiscoveryHomeAsync(userId,page,version));
	}
	/// <summary>途虎众测</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IZeroActivityService
    {
    	///<summary>获取未结束的首页众测活动列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectUnfinishedZeroActivitiesForHomepage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectUnfinishedZeroActivitiesForHomepageResponse")]
        Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectUnfinishedZeroActivitiesForHomepageAsync(bool resetCache=false);
		///<summary>获取已结束的首页众测活动列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectFinishedZeroActivitiesForHomepage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectFinishedZeroActivitiesForHomepageResponse")]
        Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectFinishedZeroActivitiesForHomepageAsync(int pageNumber);
		///<summary>获取众测活动详情</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchZeroActivityDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchZeroActivityDetailResponse")]
        Task<OperationResult<ZeroActivityDetailModel>> FetchZeroActivityDetailAsync(int period);
		///<summary>判断用户是否已提交众测申请</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityApplicationSubmitted", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityApplicationSubmittedResponse")]
        Task<OperationResult<bool>> HasZeroActivityApplicationSubmittedAsync(Guid userId, int period);
		///<summary>判断用户是否已触发开测提醒</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityReminderSubmitted", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityReminderSubmittedResponse")]
        Task<OperationResult<bool>> HasZeroActivityReminderSubmittedAsync(Guid userId, int period);
		///<summary>获取特定众测活动的入选用户与其报告概况</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectChosenUserReports", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectChosenUserReportsResponse")]
        Task<OperationResult<IEnumerable<SelectedTestReport>>> SelectChosenUserReportsAsync(int period);
		///<summary>获取众测报告详情</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchTestReportDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchTestReportDetailResponse")]
        Task<OperationResult<SelectedTestReportDetail>> FetchTestReportDetailAsync(int commentId);
		///<summary>获取用户众测活动申请,申请状态（0:申请中，1:申请成功，-1：申请失败）</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectMyApplications", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectMyApplicationsResponse")]
        Task<OperationResult<IEnumerable<MyZeroActivityApplications>>> SelectMyApplicationsAsync(Guid userId, int applicationStatus);
		///<summary>提交众测申请</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityApplication", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityApplicationResponse")]
        Task<OperationResult<bool>> SubmitZeroActivityApplicationAsync(ZeroActivityRequest requestModel);
		///<summary>触发开测提醒</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityReminder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityReminderResponse")]
        Task<OperationResult<bool>> SubmitZeroActivityReminderAsync(Guid userId, int period);
		///<summary>刷新众测活动配置缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/RefreshZeroActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/RefreshZeroActivityCacheResponse")]
        Task<OperationResult<bool>> RefreshZeroActivityCacheAsync();
		///<summary>首页众测模块用接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectHomePageModuleShowZeroActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectHomePageModuleShowZeroActivityResponse")]
        Task<OperationResult<List<ZeroActivitySimpleRespnseModel>>> SelectHomePageModuleShowZeroActivityAsync();
	}

	/// <summary>途虎众测</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IZeroActivityClient : IZeroActivityService, ITuhuServiceClient
    {
    	///<summary>获取未结束的首页众测活动列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectUnfinishedZeroActivitiesForHomepage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectUnfinishedZeroActivitiesForHomepageResponse")]
        OperationResult<IEnumerable<ZeroActivityModel>> SelectUnfinishedZeroActivitiesForHomepage(bool resetCache=false);
		///<summary>获取已结束的首页众测活动列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectFinishedZeroActivitiesForHomepage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectFinishedZeroActivitiesForHomepageResponse")]
        OperationResult<IEnumerable<ZeroActivityModel>> SelectFinishedZeroActivitiesForHomepage(int pageNumber);
		///<summary>获取众测活动详情</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchZeroActivityDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchZeroActivityDetailResponse")]
        OperationResult<ZeroActivityDetailModel> FetchZeroActivityDetail(int period);
		///<summary>判断用户是否已提交众测申请</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityApplicationSubmitted", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityApplicationSubmittedResponse")]
        OperationResult<bool> HasZeroActivityApplicationSubmitted(Guid userId, int period);
		///<summary>判断用户是否已触发开测提醒</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityReminderSubmitted", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityReminderSubmittedResponse")]
        OperationResult<bool> HasZeroActivityReminderSubmitted(Guid userId, int period);
		///<summary>获取特定众测活动的入选用户与其报告概况</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectChosenUserReports", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectChosenUserReportsResponse")]
        OperationResult<IEnumerable<SelectedTestReport>> SelectChosenUserReports(int period);
		///<summary>获取众测报告详情</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchTestReportDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchTestReportDetailResponse")]
        OperationResult<SelectedTestReportDetail> FetchTestReportDetail(int commentId);
		///<summary>获取用户众测活动申请,申请状态（0:申请中，1:申请成功，-1：申请失败）</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectMyApplications", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectMyApplicationsResponse")]
        OperationResult<IEnumerable<MyZeroActivityApplications>> SelectMyApplications(Guid userId, int applicationStatus);
		///<summary>提交众测申请</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityApplication", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityApplicationResponse")]
        OperationResult<bool> SubmitZeroActivityApplication(ZeroActivityRequest requestModel);
		///<summary>触发开测提醒</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityReminder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityReminderResponse")]
        OperationResult<bool> SubmitZeroActivityReminder(Guid userId, int period);
		///<summary>刷新众测活动配置缓存</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/RefreshZeroActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/RefreshZeroActivityCacheResponse")]
        OperationResult<bool> RefreshZeroActivityCache();
		///<summary>首页众测模块用接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectHomePageModuleShowZeroActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectHomePageModuleShowZeroActivityResponse")]
        OperationResult<List<ZeroActivitySimpleRespnseModel>> SelectHomePageModuleShowZeroActivity();
	}

	/// <summary>途虎众测</summary>
	public partial class ZeroActivityClient : TuhuServiceClient<IZeroActivityClient>, IZeroActivityClient
    {
    	///<summary>获取未结束的首页众测活动列表</summary>
        public OperationResult<IEnumerable<ZeroActivityModel>> SelectUnfinishedZeroActivitiesForHomepage(bool resetCache=false) => Invoke(_ => _.SelectUnfinishedZeroActivitiesForHomepage(resetCache));

	///<summary>获取未结束的首页众测活动列表</summary>
        public Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectUnfinishedZeroActivitiesForHomepageAsync(bool resetCache=false) => InvokeAsync(_ => _.SelectUnfinishedZeroActivitiesForHomepageAsync(resetCache));
		///<summary>获取已结束的首页众测活动列表</summary>
        public OperationResult<IEnumerable<ZeroActivityModel>> SelectFinishedZeroActivitiesForHomepage(int pageNumber) => Invoke(_ => _.SelectFinishedZeroActivitiesForHomepage(pageNumber));

	///<summary>获取已结束的首页众测活动列表</summary>
        public Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectFinishedZeroActivitiesForHomepageAsync(int pageNumber) => InvokeAsync(_ => _.SelectFinishedZeroActivitiesForHomepageAsync(pageNumber));
		///<summary>获取众测活动详情</summary>
        public OperationResult<ZeroActivityDetailModel> FetchZeroActivityDetail(int period) => Invoke(_ => _.FetchZeroActivityDetail(period));

	///<summary>获取众测活动详情</summary>
        public Task<OperationResult<ZeroActivityDetailModel>> FetchZeroActivityDetailAsync(int period) => InvokeAsync(_ => _.FetchZeroActivityDetailAsync(period));
		///<summary>判断用户是否已提交众测申请</summary>
        public OperationResult<bool> HasZeroActivityApplicationSubmitted(Guid userId, int period) => Invoke(_ => _.HasZeroActivityApplicationSubmitted(userId, period));

	///<summary>判断用户是否已提交众测申请</summary>
        public Task<OperationResult<bool>> HasZeroActivityApplicationSubmittedAsync(Guid userId, int period) => InvokeAsync(_ => _.HasZeroActivityApplicationSubmittedAsync(userId, period));
		///<summary>判断用户是否已触发开测提醒</summary>
        public OperationResult<bool> HasZeroActivityReminderSubmitted(Guid userId, int period) => Invoke(_ => _.HasZeroActivityReminderSubmitted(userId, period));

	///<summary>判断用户是否已触发开测提醒</summary>
        public Task<OperationResult<bool>> HasZeroActivityReminderSubmittedAsync(Guid userId, int period) => InvokeAsync(_ => _.HasZeroActivityReminderSubmittedAsync(userId, period));
		///<summary>获取特定众测活动的入选用户与其报告概况</summary>
        public OperationResult<IEnumerable<SelectedTestReport>> SelectChosenUserReports(int period) => Invoke(_ => _.SelectChosenUserReports(period));

	///<summary>获取特定众测活动的入选用户与其报告概况</summary>
        public Task<OperationResult<IEnumerable<SelectedTestReport>>> SelectChosenUserReportsAsync(int period) => InvokeAsync(_ => _.SelectChosenUserReportsAsync(period));
		///<summary>获取众测报告详情</summary>
        public OperationResult<SelectedTestReportDetail> FetchTestReportDetail(int commentId) => Invoke(_ => _.FetchTestReportDetail(commentId));

	///<summary>获取众测报告详情</summary>
        public Task<OperationResult<SelectedTestReportDetail>> FetchTestReportDetailAsync(int commentId) => InvokeAsync(_ => _.FetchTestReportDetailAsync(commentId));
		///<summary>获取用户众测活动申请,申请状态（0:申请中，1:申请成功，-1：申请失败）</summary>
        public OperationResult<IEnumerable<MyZeroActivityApplications>> SelectMyApplications(Guid userId, int applicationStatus) => Invoke(_ => _.SelectMyApplications(userId, applicationStatus));

	///<summary>获取用户众测活动申请,申请状态（0:申请中，1:申请成功，-1：申请失败）</summary>
        public Task<OperationResult<IEnumerable<MyZeroActivityApplications>>> SelectMyApplicationsAsync(Guid userId, int applicationStatus) => InvokeAsync(_ => _.SelectMyApplicationsAsync(userId, applicationStatus));
		///<summary>提交众测申请</summary>
        public OperationResult<bool> SubmitZeroActivityApplication(ZeroActivityRequest requestModel) => Invoke(_ => _.SubmitZeroActivityApplication(requestModel));

	///<summary>提交众测申请</summary>
        public Task<OperationResult<bool>> SubmitZeroActivityApplicationAsync(ZeroActivityRequest requestModel) => InvokeAsync(_ => _.SubmitZeroActivityApplicationAsync(requestModel));
		///<summary>触发开测提醒</summary>
        public OperationResult<bool> SubmitZeroActivityReminder(Guid userId, int period) => Invoke(_ => _.SubmitZeroActivityReminder(userId, period));

	///<summary>触发开测提醒</summary>
        public Task<OperationResult<bool>> SubmitZeroActivityReminderAsync(Guid userId, int period) => InvokeAsync(_ => _.SubmitZeroActivityReminderAsync(userId, period));
		///<summary>刷新众测活动配置缓存</summary>
        public OperationResult<bool> RefreshZeroActivityCache() => Invoke(_ => _.RefreshZeroActivityCache());

	///<summary>刷新众测活动配置缓存</summary>
        public Task<OperationResult<bool>> RefreshZeroActivityCacheAsync() => InvokeAsync(_ => _.RefreshZeroActivityCacheAsync());
		///<summary>首页众测模块用接口</summary>
        public OperationResult<List<ZeroActivitySimpleRespnseModel>> SelectHomePageModuleShowZeroActivity() => Invoke(_ => _.SelectHomePageModuleShowZeroActivity());

	///<summary>首页众测模块用接口</summary>
        public Task<OperationResult<List<ZeroActivitySimpleRespnseModel>>> SelectHomePageModuleShowZeroActivityAsync() => InvokeAsync(_ => _.SelectHomePageModuleShowZeroActivityAsync());
	}
	/// <summary>限购下单相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleCreateOrderService
    {
    	///<summary>限购下单</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FlashSaleCreateOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FlashSaleCreateOrderResponse")]
        Task<OperationResult<CreateOrderResult>> FlashSaleCreateOrderAsync(CreateOrderRequest request);
		/// <summary> 获得活动价 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FetchActivityProductPrice", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FetchActivityProductPriceResponse")]
        Task<OperationResult<IEnumerable<ActivityPriceModel>>> FetchActivityProductPriceAsync(ActivityPriceRequest request);
	}

	/// <summary>限购下单相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleCreateOrderClient : IFlashSaleCreateOrderService, ITuhuServiceClient
    {
    	///<summary>限购下单</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FlashSaleCreateOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FlashSaleCreateOrderResponse")]
        OperationResult<CreateOrderResult> FlashSaleCreateOrder(CreateOrderRequest request);
		/// <summary> 获得活动价 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FetchActivityProductPrice", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FetchActivityProductPriceResponse")]
        OperationResult<IEnumerable<ActivityPriceModel>> FetchActivityProductPrice(ActivityPriceRequest request);
	}

	/// <summary>限购下单相关</summary>
	public partial class FlashSaleCreateOrderClient : TuhuServiceClient<IFlashSaleCreateOrderClient>, IFlashSaleCreateOrderClient
    {
    	///<summary>限购下单</summary>
        public OperationResult<CreateOrderResult> FlashSaleCreateOrder(CreateOrderRequest request) => Invoke(_ => _.FlashSaleCreateOrder(request));

	///<summary>限购下单</summary>
        public Task<OperationResult<CreateOrderResult>> FlashSaleCreateOrderAsync(CreateOrderRequest request) => InvokeAsync(_ => _.FlashSaleCreateOrderAsync(request));
		/// <summary> 获得活动价 </summary>
        public OperationResult<IEnumerable<ActivityPriceModel>> FetchActivityProductPrice(ActivityPriceRequest request) => Invoke(_ => _.FetchActivityProductPrice(request));

	/// <summary> 获得活动价 </summary>
        public Task<OperationResult<IEnumerable<ActivityPriceModel>>> FetchActivityProductPriceAsync(ActivityPriceRequest request) => InvokeAsync(_ => _.FetchActivityProductPriceAsync(request));
	}
	/// <summary>分享砍价相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IShareBargainService
    {
    	/// <summary> 获取砍价商品列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainPaoductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainPaoductListResponse")]
        Task<OperationResult<PagedModel<BargainProductModel>>> GetBargainPaoductListAsync(GetBargainproductListRequest request);
		/// <summary>  获取用户该活动商品下的砍价记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductHistoryResponse")]
        Task<OperationResult<BargainProductHistory>> FetchBargainProductHistoryAsync(Guid userId,int activityProductId,string pid);
		/// <summary> 受邀人进行一次砍价 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddBargainAction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddBargainActionResponse")]
        Task<OperationResult<BargainResult>> AddBargainActionAsync(Guid idKey,Guid userId,int activityProductId);
		/// <summary> 检查此商品是否可购买 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductStatusResponse")]
        Task<OperationResult<ShareBargainBaseResult>> CheckBargainProductStatusAsync(Guid ownerId,int apId,string pid, string deviceId = default(string));
		/// <summary> 用户发起砍价分享活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddShareBargain", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddShareBargainResponse")]
        Task<OperationResult<BargainShareResult>> AddShareBargainAsync(Guid ownerId,int apId,string pid);
		/// <summary> 受邀人获取分享产品信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchShareBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchShareBargainInfoResponse")]
        Task<OperationResult<BargainShareProductModel>> FetchShareBargainInfoAsync(Guid idKey,Guid UserId);
		/// <summary> 刷新缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/RefreshShareBargainCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/RefreshShareBargainCacheResponse")]
        Task<OperationResult<bool>> RefreshShareBargainCacheAsync();
		/// <summary> 获得分享砍价全局配置 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainConfigResponse")]
        Task<OperationResult<BargainGlobalConfigModel>> GetShareBargainConfigAsync();
		/// <summary> 批量获取产品详情页 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductByIdResponse")]
        Task<OperationResult<IEnumerable<BargainProductModel>>> SelectBargainProductByIdAsync(Guid OwnerId, Guid UserId,List<BargainProductItem> ProductItems);
		/// <summary> 分页获取产品PID和apid </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductItems", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductItemsResponse")]
        Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductItemsAsync(Guid UserId,int PageIndex,int PageSize);
		/// <summary> 根据IdKey获取产品PID和apid </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductItemByShareId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductItemByShareIdResponse")]
        Task<OperationResult<BargainProductInfo>> FetchBargainProductItemByShareIdAsync(Guid IdKey);
		/// <summary> 设置分享idkey的状态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetShareBargainStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetShareBargainStatusResponse")]
        Task<OperationResult<bool>> SetShareBargainStatusAsync(Guid IdKey);
		/// <summary> 砍价流程完成后，用户领取优惠券 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/MarkUserReceiveCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/MarkUserReceiveCouponResponse")]
        Task<OperationResult<ShareBargainBaseResult>> MarkUserReceiveCouponAsync(Guid ownerId, int apId, string pid, string deviceId = default(string));
		/// <summary> 用户砍价次数，在某时间段内 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetUserBargainCountAtTimerange", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetUserBargainCountAtTimerangeResponse")]
        Task<OperationResult<int>> GetUserBargainCountAtTimerangeAsync(Guid ownerId, DateTime beginTime, DateTime endTime);
		/// <summary>首页获取默认的两个砍价商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductForIndex", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductForIndexResponse")]
        Task<OperationResult<List<SimpleBargainProduct>>> GetBargainProductForIndexAsync();
		/// <summary> 首页获取砍价产品列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductListResponse")]
        Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductListAsync(int pageIndex, int pageSize);
		/// <summary> 分页获取用户的砍价记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainHistoryResponse")]
        Task<OperationResult<PagedModel<BargainHistoryModel>>> SelectBargainHistoryAsync(int pageIndex, int pageSize, Guid userId);
		/// <summary> 首页获取砍价商品详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargsinProductDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargsinProductDetailResponse")]
        Task<OperationResult<List<BargainProductNewModel>>> GetBargsinProductDetailAsync(Guid userId, List<BargainProductItem> productItems);
		/// <summary> 获取轮播信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetSliceShowInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetSliceShowInfoResponse")]
        Task<OperationResult<List<SliceShowInfoModel>>> GetSliceShowInfoAsync(int count = 10);
		/// <summary> 用户创建并砍价 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargain", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainResponse")]
        Task<OperationResult<CreateBargainResult>> CreateserBargainAsync(Guid userId, int apId, string pid);
		/// <summary> 用户创建并砍价[是否发送推送] </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainNotPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainNotPushResponse")]
        Task<OperationResult<CreateBargainResult>> CreateserBargainNotPushAsync(Guid userId, int apId, string pid,bool isPush = false);
		/// <summary> 受邀人获取砍价结果 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfoResponse")]
        Task<OperationResult<InviteeBarginInfo>> GetInviteeBargainInfoAsync(Guid idKey,Guid userId);
		/// <summary> 获取未完成的 发起砍价记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetValidBargainOwnerActionsByApids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetValidBargainOwnerActionsByApidsResponse")]
        Task<OperationResult<List<CurrentBargainData>>> GetValidBargainOwnerActionsByApidsAsync(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true);
		/// <summary> 获取砍价的配置  【时间】 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductsByDate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductsByDateResponse")]
        Task<OperationResult<List<BargainProductNewModel>>> SelectBargainProductsByDateAsync(DateTime startDate, DateTime endDate);
		/// <summary> 砍价落地页推送 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/BargainPushMessage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/BargainPushMessageResponse")]
        Task<OperationResult<bool>> BargainPushMessageAsync(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey);
		/// <summary> 用户发起砍价并自砍 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateBargainAndCutSelf", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateBargainAndCutSelfResponse")]
        Task<OperationResult<CreateBargainResult>> CreateBargainAndCutSelfAsync(CreateBargainAndCutSelfRequest request);
		/// <summary> 检查用户是否可购买砍价商品 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductBuyStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductBuyStatusResponse")]
        Task<OperationResult<ShareBargainBaseResult>> CheckBargainProductBuyStatusAsync(CheckBargainProductBuyStatusRequest request);
		/// <summary> 用户领取砍价优惠券 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/ReceiveBargainCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/ReceiveBargainCouponResponse")]
        Task<OperationResult<ShareBargainBaseResult>> ReceiveBargainCouponAsync(ReceiveBargainCouponRequest request);
		/// <summary> 验证是否为砍价黑名单 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainBlackList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainBlackListResponse")]
        Task<OperationResult<ShareBargainBaseResult>> CheckBargainBlackListAsync(BargainBlackListRequest request);
		/// <summary>用户帮砍</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/HelpCut", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/HelpCutResponse")]
        Task<OperationResult<BargainResult>> HelpCutAsync(HelpCutRequest request);
		/// <summary>获取砍价活动商品信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainProductInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainProductInfoResponse")]
        Task<OperationResult<GetShareBargainProductInfoResponse>> GetShareBargainProductInfoAsync(GetShareBargainProductInfoRequest request);
		/// <summary>获取砍价活动商品配置基本信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainSettingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainSettingInfoResponse")]
        Task<OperationResult<GetShareBargainSettingInfoResponse>> GetShareBargainSettingInfoAsync(GetShareBargainSettingInfoRequest request);
		/// <summary>获取砍价分享的被帮砍记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutListResponse")]
        Task<OperationResult<List<GetShareBeHelpCutListResponse>>> GetShareBeHelpCutListAsync(GetShareBeHelpCutListRequest request);
		/// <summary>获取砍价配置当前参与用户信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainUserParticipantInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainUserParticipantInfoResponse")]
        Task<OperationResult<GetShareBargainUserParticipantInfoResponse>> GetShareBargainUserParticipantInfoAsync(GetShareBargainUserParticipantInfoRequest request);
		/// <summary>标识用户砍价失败已推送</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetBargainOwnerFailIsPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetBargainOwnerFailIsPushResponse")]
        Task<OperationResult<SetBargainOwnerFailIsPushResponse>> SetBargainOwnerFailIsPushAsync(SetBargainOwnerFailIsPushRequest request);
		/// <summary>获取 砍价失败的发起记录:砍价发起超过48小时|砍价成功24小时未购买</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainOwnerExpireList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainOwnerExpireListResponse")]
        Task<OperationResult<List<GetBargainOwnerExpireListResponse>>> GetBargainOwnerExpireListAsync(GetBargainOwnerExpireListRequest request);
		/// <summary>获取砍价分享的被帮砍记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutInfoListResponse")]
        Task<OperationResult<List<GetShareBeHelpCutInfoListResponse>>> GetShareBeHelpCutInfoListAsync(GetShareBeHelpCutInfoListRequest request);
		/// <summary>获取砍价商品信息和用户发起砍价信息(砍价详情页)</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductAndUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductAndUserInfoResponse")]
        Task<OperationResult<GetBargainProductAndUserInfoResponse>> GetBargainProductAndUserInfoAsync(GetBargainProductAndUserInfoRequest request);
	}

	/// <summary>分享砍价相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IShareBargainClient : IShareBargainService, ITuhuServiceClient
    {
    	/// <summary> 获取砍价商品列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainPaoductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainPaoductListResponse")]
        OperationResult<PagedModel<BargainProductModel>> GetBargainPaoductList(GetBargainproductListRequest request);
		/// <summary>  获取用户该活动商品下的砍价记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductHistoryResponse")]
        OperationResult<BargainProductHistory> FetchBargainProductHistory(Guid userId,int activityProductId,string pid);
		/// <summary> 受邀人进行一次砍价 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddBargainAction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddBargainActionResponse")]
        OperationResult<BargainResult> AddBargainAction(Guid idKey,Guid userId,int activityProductId);
		/// <summary> 检查此商品是否可购买 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductStatusResponse")]
        OperationResult<ShareBargainBaseResult> CheckBargainProductStatus(Guid ownerId,int apId,string pid, string deviceId = default(string));
		/// <summary> 用户发起砍价分享活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddShareBargain", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddShareBargainResponse")]
        OperationResult<BargainShareResult> AddShareBargain(Guid ownerId,int apId,string pid);
		/// <summary> 受邀人获取分享产品信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchShareBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchShareBargainInfoResponse")]
        OperationResult<BargainShareProductModel> FetchShareBargainInfo(Guid idKey,Guid UserId);
		/// <summary> 刷新缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/RefreshShareBargainCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/RefreshShareBargainCacheResponse")]
        OperationResult<bool> RefreshShareBargainCache();
		/// <summary> 获得分享砍价全局配置 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainConfigResponse")]
        OperationResult<BargainGlobalConfigModel> GetShareBargainConfig();
		/// <summary> 批量获取产品详情页 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductByIdResponse")]
        OperationResult<IEnumerable<BargainProductModel>> SelectBargainProductById(Guid OwnerId, Guid UserId,List<BargainProductItem> ProductItems);
		/// <summary> 分页获取产品PID和apid </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductItems", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductItemsResponse")]
        OperationResult<PagedModel<BargainProductItem>> SelectBargainProductItems(Guid UserId,int PageIndex,int PageSize);
		/// <summary> 根据IdKey获取产品PID和apid </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductItemByShareId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductItemByShareIdResponse")]
        OperationResult<BargainProductInfo> FetchBargainProductItemByShareId(Guid IdKey);
		/// <summary> 设置分享idkey的状态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetShareBargainStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetShareBargainStatusResponse")]
        OperationResult<bool> SetShareBargainStatus(Guid IdKey);
		/// <summary> 砍价流程完成后，用户领取优惠券 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/MarkUserReceiveCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/MarkUserReceiveCouponResponse")]
        OperationResult<ShareBargainBaseResult> MarkUserReceiveCoupon(Guid ownerId, int apId, string pid, string deviceId = default(string));
		/// <summary> 用户砍价次数，在某时间段内 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetUserBargainCountAtTimerange", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetUserBargainCountAtTimerangeResponse")]
        OperationResult<int> GetUserBargainCountAtTimerange(Guid ownerId, DateTime beginTime, DateTime endTime);
		/// <summary>首页获取默认的两个砍价商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductForIndex", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductForIndexResponse")]
        OperationResult<List<SimpleBargainProduct>> GetBargainProductForIndex();
		/// <summary> 首页获取砍价产品列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductListResponse")]
        OperationResult<PagedModel<BargainProductItem>> SelectBargainProductList(int pageIndex, int pageSize);
		/// <summary> 分页获取用户的砍价记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainHistoryResponse")]
        OperationResult<PagedModel<BargainHistoryModel>> SelectBargainHistory(int pageIndex, int pageSize, Guid userId);
		/// <summary> 首页获取砍价商品详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargsinProductDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargsinProductDetailResponse")]
        OperationResult<List<BargainProductNewModel>> GetBargsinProductDetail(Guid userId, List<BargainProductItem> productItems);
		/// <summary> 获取轮播信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetSliceShowInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetSliceShowInfoResponse")]
        OperationResult<List<SliceShowInfoModel>> GetSliceShowInfo(int count = 10);
		/// <summary> 用户创建并砍价 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargain", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainResponse")]
        OperationResult<CreateBargainResult> CreateserBargain(Guid userId, int apId, string pid);
		/// <summary> 用户创建并砍价[是否发送推送] </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainNotPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainNotPushResponse")]
        OperationResult<CreateBargainResult> CreateserBargainNotPush(Guid userId, int apId, string pid,bool isPush = false);
		/// <summary> 受邀人获取砍价结果 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfoResponse")]
        OperationResult<InviteeBarginInfo> GetInviteeBargainInfo(Guid idKey,Guid userId);
		/// <summary> 获取未完成的 发起砍价记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetValidBargainOwnerActionsByApids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetValidBargainOwnerActionsByApidsResponse")]
        OperationResult<List<CurrentBargainData>> GetValidBargainOwnerActionsByApids(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true);
		/// <summary> 获取砍价的配置  【时间】 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductsByDate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductsByDateResponse")]
        OperationResult<List<BargainProductNewModel>> SelectBargainProductsByDate(DateTime startDate, DateTime endDate);
		/// <summary> 砍价落地页推送 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/BargainPushMessage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/BargainPushMessageResponse")]
        OperationResult<bool> BargainPushMessage(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey);
		/// <summary> 用户发起砍价并自砍 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateBargainAndCutSelf", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateBargainAndCutSelfResponse")]
        OperationResult<CreateBargainResult> CreateBargainAndCutSelf(CreateBargainAndCutSelfRequest request);
		/// <summary> 检查用户是否可购买砍价商品 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductBuyStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductBuyStatusResponse")]
        OperationResult<ShareBargainBaseResult> CheckBargainProductBuyStatus(CheckBargainProductBuyStatusRequest request);
		/// <summary> 用户领取砍价优惠券 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/ReceiveBargainCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/ReceiveBargainCouponResponse")]
        OperationResult<ShareBargainBaseResult> ReceiveBargainCoupon(ReceiveBargainCouponRequest request);
		/// <summary> 验证是否为砍价黑名单 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainBlackList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainBlackListResponse")]
        OperationResult<ShareBargainBaseResult> CheckBargainBlackList(BargainBlackListRequest request);
		/// <summary>用户帮砍</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/HelpCut", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/HelpCutResponse")]
        OperationResult<BargainResult> HelpCut(HelpCutRequest request);
		/// <summary>获取砍价活动商品信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainProductInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainProductInfoResponse")]
        OperationResult<GetShareBargainProductInfoResponse> GetShareBargainProductInfo(GetShareBargainProductInfoRequest request);
		/// <summary>获取砍价活动商品配置基本信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainSettingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainSettingInfoResponse")]
        OperationResult<GetShareBargainSettingInfoResponse> GetShareBargainSettingInfo(GetShareBargainSettingInfoRequest request);
		/// <summary>获取砍价分享的被帮砍记录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutListResponse")]
        OperationResult<List<GetShareBeHelpCutListResponse>> GetShareBeHelpCutList(GetShareBeHelpCutListRequest request);
		/// <summary>获取砍价配置当前参与用户信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainUserParticipantInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainUserParticipantInfoResponse")]
        OperationResult<GetShareBargainUserParticipantInfoResponse> GetShareBargainUserParticipantInfo(GetShareBargainUserParticipantInfoRequest request);
		/// <summary>标识用户砍价失败已推送</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetBargainOwnerFailIsPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetBargainOwnerFailIsPushResponse")]
        OperationResult<SetBargainOwnerFailIsPushResponse> SetBargainOwnerFailIsPush(SetBargainOwnerFailIsPushRequest request);
		/// <summary>获取 砍价失败的发起记录:砍价发起超过48小时|砍价成功24小时未购买</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainOwnerExpireList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainOwnerExpireListResponse")]
        OperationResult<List<GetBargainOwnerExpireListResponse>> GetBargainOwnerExpireList(GetBargainOwnerExpireListRequest request);
		/// <summary>获取砍价分享的被帮砍记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutInfoListResponse")]
        OperationResult<List<GetShareBeHelpCutInfoListResponse>> GetShareBeHelpCutInfoList(GetShareBeHelpCutInfoListRequest request);
		/// <summary>获取砍价商品信息和用户发起砍价信息(砍价详情页)</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductAndUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductAndUserInfoResponse")]
        OperationResult<GetBargainProductAndUserInfoResponse> GetBargainProductAndUserInfo(GetBargainProductAndUserInfoRequest request);
	}

	/// <summary>分享砍价相关</summary>
	public partial class ShareBargainClient : TuhuServiceClient<IShareBargainClient>, IShareBargainClient
    {
    	/// <summary> 获取砍价商品列表 </summary>
        public OperationResult<PagedModel<BargainProductModel>> GetBargainPaoductList(GetBargainproductListRequest request) => Invoke(_ => _.GetBargainPaoductList(request));

	/// <summary> 获取砍价商品列表 </summary>
        public Task<OperationResult<PagedModel<BargainProductModel>>> GetBargainPaoductListAsync(GetBargainproductListRequest request) => InvokeAsync(_ => _.GetBargainPaoductListAsync(request));
		/// <summary>  获取用户该活动商品下的砍价记录 </summary>
        public OperationResult<BargainProductHistory> FetchBargainProductHistory(Guid userId,int activityProductId,string pid) => Invoke(_ => _.FetchBargainProductHistory(userId,activityProductId,pid));

	/// <summary>  获取用户该活动商品下的砍价记录 </summary>
        public Task<OperationResult<BargainProductHistory>> FetchBargainProductHistoryAsync(Guid userId,int activityProductId,string pid) => InvokeAsync(_ => _.FetchBargainProductHistoryAsync(userId,activityProductId,pid));
		/// <summary> 受邀人进行一次砍价 </summary>
        public OperationResult<BargainResult> AddBargainAction(Guid idKey,Guid userId,int activityProductId) => Invoke(_ => _.AddBargainAction(idKey,userId,activityProductId));

	/// <summary> 受邀人进行一次砍价 </summary>
        public Task<OperationResult<BargainResult>> AddBargainActionAsync(Guid idKey,Guid userId,int activityProductId) => InvokeAsync(_ => _.AddBargainActionAsync(idKey,userId,activityProductId));
		/// <summary> 检查此商品是否可购买 </summary>
        public OperationResult<ShareBargainBaseResult> CheckBargainProductStatus(Guid ownerId,int apId,string pid, string deviceId = default(string)) => Invoke(_ => _.CheckBargainProductStatus(ownerId,apId,pid,deviceId));

	/// <summary> 检查此商品是否可购买 </summary>
        public Task<OperationResult<ShareBargainBaseResult>> CheckBargainProductStatusAsync(Guid ownerId,int apId,string pid, string deviceId = default(string)) => InvokeAsync(_ => _.CheckBargainProductStatusAsync(ownerId,apId,pid,deviceId));
		/// <summary> 用户发起砍价分享活动 </summary>
        public OperationResult<BargainShareResult> AddShareBargain(Guid ownerId,int apId,string pid) => Invoke(_ => _.AddShareBargain(ownerId,apId,pid));

	/// <summary> 用户发起砍价分享活动 </summary>
        public Task<OperationResult<BargainShareResult>> AddShareBargainAsync(Guid ownerId,int apId,string pid) => InvokeAsync(_ => _.AddShareBargainAsync(ownerId,apId,pid));
		/// <summary> 受邀人获取分享产品信息 </summary>
        public OperationResult<BargainShareProductModel> FetchShareBargainInfo(Guid idKey,Guid UserId) => Invoke(_ => _.FetchShareBargainInfo(idKey,UserId));

	/// <summary> 受邀人获取分享产品信息 </summary>
        public Task<OperationResult<BargainShareProductModel>> FetchShareBargainInfoAsync(Guid idKey,Guid UserId) => InvokeAsync(_ => _.FetchShareBargainInfoAsync(idKey,UserId));
		/// <summary> 刷新缓存 </summary>
        public OperationResult<bool> RefreshShareBargainCache() => Invoke(_ => _.RefreshShareBargainCache());

	/// <summary> 刷新缓存 </summary>
        public Task<OperationResult<bool>> RefreshShareBargainCacheAsync() => InvokeAsync(_ => _.RefreshShareBargainCacheAsync());
		/// <summary> 获得分享砍价全局配置 </summary>
        public OperationResult<BargainGlobalConfigModel> GetShareBargainConfig() => Invoke(_ => _.GetShareBargainConfig());

	/// <summary> 获得分享砍价全局配置 </summary>
        public Task<OperationResult<BargainGlobalConfigModel>> GetShareBargainConfigAsync() => InvokeAsync(_ => _.GetShareBargainConfigAsync());
		/// <summary> 批量获取产品详情页 </summary>
        public OperationResult<IEnumerable<BargainProductModel>> SelectBargainProductById(Guid OwnerId, Guid UserId,List<BargainProductItem> ProductItems) => Invoke(_ => _.SelectBargainProductById(OwnerId,UserId,ProductItems));

	/// <summary> 批量获取产品详情页 </summary>
        public Task<OperationResult<IEnumerable<BargainProductModel>>> SelectBargainProductByIdAsync(Guid OwnerId, Guid UserId,List<BargainProductItem> ProductItems) => InvokeAsync(_ => _.SelectBargainProductByIdAsync(OwnerId,UserId,ProductItems));
		/// <summary> 分页获取产品PID和apid </summary>
        public OperationResult<PagedModel<BargainProductItem>> SelectBargainProductItems(Guid UserId,int PageIndex,int PageSize) => Invoke(_ => _.SelectBargainProductItems(UserId,PageIndex,PageSize));

	/// <summary> 分页获取产品PID和apid </summary>
        public Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductItemsAsync(Guid UserId,int PageIndex,int PageSize) => InvokeAsync(_ => _.SelectBargainProductItemsAsync(UserId,PageIndex,PageSize));
		/// <summary> 根据IdKey获取产品PID和apid </summary>
        public OperationResult<BargainProductInfo> FetchBargainProductItemByShareId(Guid IdKey) => Invoke(_ => _.FetchBargainProductItemByShareId(IdKey));

	/// <summary> 根据IdKey获取产品PID和apid </summary>
        public Task<OperationResult<BargainProductInfo>> FetchBargainProductItemByShareIdAsync(Guid IdKey) => InvokeAsync(_ => _.FetchBargainProductItemByShareIdAsync(IdKey));
		/// <summary> 设置分享idkey的状态 </summary>
        public OperationResult<bool> SetShareBargainStatus(Guid IdKey) => Invoke(_ => _.SetShareBargainStatus(IdKey));

	/// <summary> 设置分享idkey的状态 </summary>
        public Task<OperationResult<bool>> SetShareBargainStatusAsync(Guid IdKey) => InvokeAsync(_ => _.SetShareBargainStatusAsync(IdKey));
		/// <summary> 砍价流程完成后，用户领取优惠券 </summary>
        public OperationResult<ShareBargainBaseResult> MarkUserReceiveCoupon(Guid ownerId, int apId, string pid, string deviceId = default(string)) => Invoke(_ => _.MarkUserReceiveCoupon(ownerId,apId,pid,deviceId));

	/// <summary> 砍价流程完成后，用户领取优惠券 </summary>
        public Task<OperationResult<ShareBargainBaseResult>> MarkUserReceiveCouponAsync(Guid ownerId, int apId, string pid, string deviceId = default(string)) => InvokeAsync(_ => _.MarkUserReceiveCouponAsync(ownerId,apId,pid,deviceId));
		/// <summary> 用户砍价次数，在某时间段内 </summary>
        public OperationResult<int> GetUserBargainCountAtTimerange(Guid ownerId, DateTime beginTime, DateTime endTime) => Invoke(_ => _.GetUserBargainCountAtTimerange(ownerId,beginTime,endTime));

	/// <summary> 用户砍价次数，在某时间段内 </summary>
        public Task<OperationResult<int>> GetUserBargainCountAtTimerangeAsync(Guid ownerId, DateTime beginTime, DateTime endTime) => InvokeAsync(_ => _.GetUserBargainCountAtTimerangeAsync(ownerId,beginTime,endTime));
		/// <summary>首页获取默认的两个砍价商品</summary>
        public OperationResult<List<SimpleBargainProduct>> GetBargainProductForIndex() => Invoke(_ => _.GetBargainProductForIndex());

	/// <summary>首页获取默认的两个砍价商品</summary>
        public Task<OperationResult<List<SimpleBargainProduct>>> GetBargainProductForIndexAsync() => InvokeAsync(_ => _.GetBargainProductForIndexAsync());
		/// <summary> 首页获取砍价产品列表 </summary>
        public OperationResult<PagedModel<BargainProductItem>> SelectBargainProductList(int pageIndex, int pageSize) => Invoke(_ => _.SelectBargainProductList(pageIndex, pageSize));

	/// <summary> 首页获取砍价产品列表 </summary>
        public Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductListAsync(int pageIndex, int pageSize) => InvokeAsync(_ => _.SelectBargainProductListAsync(pageIndex, pageSize));
		/// <summary> 分页获取用户的砍价记录 </summary>
        public OperationResult<PagedModel<BargainHistoryModel>> SelectBargainHistory(int pageIndex, int pageSize, Guid userId) => Invoke(_ => _.SelectBargainHistory(pageIndex, pageSize, userId));

	/// <summary> 分页获取用户的砍价记录 </summary>
        public Task<OperationResult<PagedModel<BargainHistoryModel>>> SelectBargainHistoryAsync(int pageIndex, int pageSize, Guid userId) => InvokeAsync(_ => _.SelectBargainHistoryAsync(pageIndex, pageSize, userId));
		/// <summary> 首页获取砍价商品详情 </summary>
        public OperationResult<List<BargainProductNewModel>> GetBargsinProductDetail(Guid userId, List<BargainProductItem> productItems) => Invoke(_ => _.GetBargsinProductDetail(userId, productItems));

	/// <summary> 首页获取砍价商品详情 </summary>
        public Task<OperationResult<List<BargainProductNewModel>>> GetBargsinProductDetailAsync(Guid userId, List<BargainProductItem> productItems) => InvokeAsync(_ => _.GetBargsinProductDetailAsync(userId, productItems));
		/// <summary> 获取轮播信息 </summary>
        public OperationResult<List<SliceShowInfoModel>> GetSliceShowInfo(int count = 10) => Invoke(_ => _.GetSliceShowInfo(count));

	/// <summary> 获取轮播信息 </summary>
        public Task<OperationResult<List<SliceShowInfoModel>>> GetSliceShowInfoAsync(int count = 10) => InvokeAsync(_ => _.GetSliceShowInfoAsync(count));
		/// <summary> 用户创建并砍价 </summary>
        public OperationResult<CreateBargainResult> CreateserBargain(Guid userId, int apId, string pid) => Invoke(_ => _.CreateserBargain(userId, apId, pid));

	/// <summary> 用户创建并砍价 </summary>
        public Task<OperationResult<CreateBargainResult>> CreateserBargainAsync(Guid userId, int apId, string pid) => InvokeAsync(_ => _.CreateserBargainAsync(userId, apId, pid));
		/// <summary> 用户创建并砍价[是否发送推送] </summary>
        public OperationResult<CreateBargainResult> CreateserBargainNotPush(Guid userId, int apId, string pid,bool isPush = false) => Invoke(_ => _.CreateserBargainNotPush(userId, apId, pid, isPush));

	/// <summary> 用户创建并砍价[是否发送推送] </summary>
        public Task<OperationResult<CreateBargainResult>> CreateserBargainNotPushAsync(Guid userId, int apId, string pid,bool isPush = false) => InvokeAsync(_ => _.CreateserBargainNotPushAsync(userId, apId, pid, isPush));
		/// <summary> 受邀人获取砍价结果 </summary>
        public OperationResult<InviteeBarginInfo> GetInviteeBargainInfo(Guid idKey,Guid userId) => Invoke(_ => _.GetInviteeBargainInfo(idKey, userId));

	/// <summary> 受邀人获取砍价结果 </summary>
        public Task<OperationResult<InviteeBarginInfo>> GetInviteeBargainInfoAsync(Guid idKey,Guid userId) => InvokeAsync(_ => _.GetInviteeBargainInfoAsync(idKey, userId));
		/// <summary> 获取未完成的 发起砍价记录 </summary>
        public OperationResult<List<CurrentBargainData>> GetValidBargainOwnerActionsByApids(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true) => Invoke(_ => _.GetValidBargainOwnerActionsByApids(apId,startDate, endDate, status, IsOver,readOnly));

	/// <summary> 获取未完成的 发起砍价记录 </summary>
        public Task<OperationResult<List<CurrentBargainData>>> GetValidBargainOwnerActionsByApidsAsync(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true) => InvokeAsync(_ => _.GetValidBargainOwnerActionsByApidsAsync(apId,startDate, endDate, status, IsOver,readOnly));
		/// <summary> 获取砍价的配置  【时间】 </summary>
        public OperationResult<List<BargainProductNewModel>> SelectBargainProductsByDate(DateTime startDate, DateTime endDate) => Invoke(_ => _.SelectBargainProductsByDate(startDate, endDate));

	/// <summary> 获取砍价的配置  【时间】 </summary>
        public Task<OperationResult<List<BargainProductNewModel>>> SelectBargainProductsByDateAsync(DateTime startDate, DateTime endDate) => InvokeAsync(_ => _.SelectBargainProductsByDateAsync(startDate, endDate));
		/// <summary> 砍价落地页推送 </summary>
        public OperationResult<bool> BargainPushMessage(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey) => Invoke(_ => _.BargainPushMessage( data,  isOver,  apId,  userId,  idKey));

	/// <summary> 砍价落地页推送 </summary>
        public Task<OperationResult<bool>> BargainPushMessageAsync(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey) => InvokeAsync(_ => _.BargainPushMessageAsync( data,  isOver,  apId,  userId,  idKey));
		/// <summary> 用户发起砍价并自砍 </summary>
        public OperationResult<CreateBargainResult> CreateBargainAndCutSelf(CreateBargainAndCutSelfRequest request) => Invoke(_ => _.CreateBargainAndCutSelf(request));

	/// <summary> 用户发起砍价并自砍 </summary>
        public Task<OperationResult<CreateBargainResult>> CreateBargainAndCutSelfAsync(CreateBargainAndCutSelfRequest request) => InvokeAsync(_ => _.CreateBargainAndCutSelfAsync(request));
		/// <summary> 检查用户是否可购买砍价商品 </summary>
        public OperationResult<ShareBargainBaseResult> CheckBargainProductBuyStatus(CheckBargainProductBuyStatusRequest request) => Invoke(_ => _.CheckBargainProductBuyStatus(request));

	/// <summary> 检查用户是否可购买砍价商品 </summary>
        public Task<OperationResult<ShareBargainBaseResult>> CheckBargainProductBuyStatusAsync(CheckBargainProductBuyStatusRequest request) => InvokeAsync(_ => _.CheckBargainProductBuyStatusAsync(request));
		/// <summary> 用户领取砍价优惠券 </summary>
        public OperationResult<ShareBargainBaseResult> ReceiveBargainCoupon(ReceiveBargainCouponRequest request) => Invoke(_ => _.ReceiveBargainCoupon(request));

	/// <summary> 用户领取砍价优惠券 </summary>
        public Task<OperationResult<ShareBargainBaseResult>> ReceiveBargainCouponAsync(ReceiveBargainCouponRequest request) => InvokeAsync(_ => _.ReceiveBargainCouponAsync(request));
		/// <summary> 验证是否为砍价黑名单 </summary>
        public OperationResult<ShareBargainBaseResult> CheckBargainBlackList(BargainBlackListRequest request) => Invoke(_ => _.CheckBargainBlackList(request));

	/// <summary> 验证是否为砍价黑名单 </summary>
        public Task<OperationResult<ShareBargainBaseResult>> CheckBargainBlackListAsync(BargainBlackListRequest request) => InvokeAsync(_ => _.CheckBargainBlackListAsync(request));
		/// <summary>用户帮砍</summary>
        public OperationResult<BargainResult> HelpCut(HelpCutRequest request) => Invoke(_ => _.HelpCut(request));

	/// <summary>用户帮砍</summary>
        public Task<OperationResult<BargainResult>> HelpCutAsync(HelpCutRequest request) => InvokeAsync(_ => _.HelpCutAsync(request));
		/// <summary>获取砍价活动商品信息</summary>
        public OperationResult<GetShareBargainProductInfoResponse> GetShareBargainProductInfo(GetShareBargainProductInfoRequest request) => Invoke(_ => _.GetShareBargainProductInfo(request));

	/// <summary>获取砍价活动商品信息</summary>
        public Task<OperationResult<GetShareBargainProductInfoResponse>> GetShareBargainProductInfoAsync(GetShareBargainProductInfoRequest request) => InvokeAsync(_ => _.GetShareBargainProductInfoAsync(request));
		/// <summary>获取砍价活动商品配置基本信息</summary>
        public OperationResult<GetShareBargainSettingInfoResponse> GetShareBargainSettingInfo(GetShareBargainSettingInfoRequest request) => Invoke(_ => _.GetShareBargainSettingInfo(request));

	/// <summary>获取砍价活动商品配置基本信息</summary>
        public Task<OperationResult<GetShareBargainSettingInfoResponse>> GetShareBargainSettingInfoAsync(GetShareBargainSettingInfoRequest request) => InvokeAsync(_ => _.GetShareBargainSettingInfoAsync(request));
		/// <summary>获取砍价分享的被帮砍记录</summary>
        public OperationResult<List<GetShareBeHelpCutListResponse>> GetShareBeHelpCutList(GetShareBeHelpCutListRequest request) => Invoke(_ => _.GetShareBeHelpCutList(request));

	/// <summary>获取砍价分享的被帮砍记录</summary>
        public Task<OperationResult<List<GetShareBeHelpCutListResponse>>> GetShareBeHelpCutListAsync(GetShareBeHelpCutListRequest request) => InvokeAsync(_ => _.GetShareBeHelpCutListAsync(request));
		/// <summary>获取砍价配置当前参与用户信息</summary>
        public OperationResult<GetShareBargainUserParticipantInfoResponse> GetShareBargainUserParticipantInfo(GetShareBargainUserParticipantInfoRequest request) => Invoke(_ => _.GetShareBargainUserParticipantInfo(request));

	/// <summary>获取砍价配置当前参与用户信息</summary>
        public Task<OperationResult<GetShareBargainUserParticipantInfoResponse>> GetShareBargainUserParticipantInfoAsync(GetShareBargainUserParticipantInfoRequest request) => InvokeAsync(_ => _.GetShareBargainUserParticipantInfoAsync(request));
		/// <summary>标识用户砍价失败已推送</summary>
        public OperationResult<SetBargainOwnerFailIsPushResponse> SetBargainOwnerFailIsPush(SetBargainOwnerFailIsPushRequest request) => Invoke(_ => _.SetBargainOwnerFailIsPush(request));

	/// <summary>标识用户砍价失败已推送</summary>
        public Task<OperationResult<SetBargainOwnerFailIsPushResponse>> SetBargainOwnerFailIsPushAsync(SetBargainOwnerFailIsPushRequest request) => InvokeAsync(_ => _.SetBargainOwnerFailIsPushAsync(request));
		/// <summary>获取 砍价失败的发起记录:砍价发起超过48小时|砍价成功24小时未购买</summary>
        public OperationResult<List<GetBargainOwnerExpireListResponse>> GetBargainOwnerExpireList(GetBargainOwnerExpireListRequest request) => Invoke(_ => _.GetBargainOwnerExpireList(request));

	/// <summary>获取 砍价失败的发起记录:砍价发起超过48小时|砍价成功24小时未购买</summary>
        public Task<OperationResult<List<GetBargainOwnerExpireListResponse>>> GetBargainOwnerExpireListAsync(GetBargainOwnerExpireListRequest request) => InvokeAsync(_ => _.GetBargainOwnerExpireListAsync(request));
		/// <summary>获取砍价分享的被帮砍记录 </summary>
        public OperationResult<List<GetShareBeHelpCutInfoListResponse>> GetShareBeHelpCutInfoList(GetShareBeHelpCutInfoListRequest request) => Invoke(_ => _.GetShareBeHelpCutInfoList(request));

	/// <summary>获取砍价分享的被帮砍记录 </summary>
        public Task<OperationResult<List<GetShareBeHelpCutInfoListResponse>>> GetShareBeHelpCutInfoListAsync(GetShareBeHelpCutInfoListRequest request) => InvokeAsync(_ => _.GetShareBeHelpCutInfoListAsync(request));
		/// <summary>获取砍价商品信息和用户发起砍价信息(砍价详情页)</summary>
        public OperationResult<GetBargainProductAndUserInfoResponse> GetBargainProductAndUserInfo(GetBargainProductAndUserInfoRequest request) => Invoke(_ => _.GetBargainProductAndUserInfo(request));

	/// <summary>获取砍价商品信息和用户发起砍价信息(砍价详情页)</summary>
        public Task<OperationResult<GetBargainProductAndUserInfoResponse>> GetBargainProductAndUserInfoAsync(GetBargainProductAndUserInfoRequest request) => InvokeAsync(_ => _.GetBargainProductAndUserInfoAsync(request));
	}
	/// <summary>拼团活动相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IGroupBuyingService
    {
    	/// <summary> 分页获取首页ProductGroupId </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListResponse")]
        Task<OperationResult<PagedModel<string>>> GetGroupBuyingProductListAsync(int PageIndex = 1, int PageSize = 10, bool flag = false,string channel=default(string),bool isOldUser=false);
		/// <summary> 根据ProductGroupId获取对应产品的PID </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingProductsById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingProductsByIdResponse")]
        Task<OperationResult<List<string>>> SelectGroupBuyingProductsByIdAsync(string ProductGroupId);
		/// <summary> 根据ProductGroupId获取对应ProductGroup信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupInfoResponse")]
        Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupInfoAsync(List<string> ProductGroupIds);
		/// <summary> 根据PID获取对应产品的信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductInfoByPid", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductInfoByPidResponse")]
        Task<OperationResult<GroupBuyingProductModel>> SelectProductInfoByPidAsync(string ProductGroupId, string Pid);
		/// <summary> 根据PID获取该产品组下，该用户可以参加的若干个团 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoByProductGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoByProductGroupIdResponse")]
        Task<OperationResult<List<GroupInfoModel>>> SelectGroupInfoByProductGroupIdAsync(string ProductGroupId, Guid UserId, int Count = 100);
		/// <summary> 根据团号获取拼团信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByGroupIdResponse")]
        Task<OperationResult<GroupInfoModel>> FetchGroupInfoByGroupIdAsync(Guid GroupId);
		/// <summary> 根据团号获取当前团成员 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupMemberByGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupMemberByGroupIdResponse")]
        Task<OperationResult<GroupMemberModel>> SelectGroupMemberByGroupIdAsync(Guid GroupId);
		/// <summary> 校验用户的参团资格 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckGroupInfoByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckGroupInfoByUserIdResponse")]
        Task<OperationResult<CheckResultModel>> CheckGroupInfoByUserIdAsync(Guid GroupId, Guid UserId, string ProductGroupId,string pid=default(string));
		/// <summary> 用户创建新团 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CreateGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CreateGroupBuyingResponse")]
        Task<OperationResult<VerificationResultModel>> CreateGroupBuyingAsync(Guid UserId, string ProductGroupId, string Pid, int OrderId);
		/// <summary> 用户参与拼团 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/TakePartInGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/TakePartInGroupBuyingResponse")]
        Task<OperationResult<VerificationResultModel>> TakePartInGroupBuyingAsync(Guid UserId, Guid GroupId, string Pid, int OrderId);
		/// <summary> 分页获取该用户下团号 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupInfoByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupInfoByUserIdResponse")]
        Task<OperationResult<PagedModel<UserGroupBuyingInfoModel>>> GetUserGroupInfoByUserIdAsync(GroupInfoRequest request);
		/// <summary> 刷新拼团产品缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshCacheResponse")]
        Task<OperationResult<VerificationResultModel>> RefreshCacheAsync(string ProductGroupId = null);
		/// <summary> 刷新拼团缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshGroupCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshGroupCacheResponse")]
        Task<OperationResult<VerificationResultModel>> RefreshGroupCacheAsync(Guid GroupId);
		/// <summary> 根据团号，UserId获取用户订单信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchUserOrderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchUserOrderInfoResponse")]
        Task<OperationResult<UserOrderInfoModel>> FetchUserOrderInfoAsync(Guid GroupId, Guid UserId);
		/// <summary> 用户取消订单 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CancelGroupBuyingOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CancelGroupBuyingOrderResponse")]
        Task<OperationResult<VerificationResultModel>> CancelGroupBuyingOrderAsync(Guid GroupId, int OrderId);
		/// <summary> 团长付款，该团可见 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeGroupBuyingStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeGroupBuyingStatusResponse")]
        Task<OperationResult<bool>> ChangeGroupBuyingStatusAsync(Guid GroupId, int OrderId);
		/// <summary> 团员付款加入 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeUserStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeUserStatusResponse")]
        Task<OperationResult<bool>> ChangeUserStatusAsync(Guid GroupId, Guid UserId, int OrderId);
		/// <summary> 拼团过期 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ExpireGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ExpireGroupBuyingResponse")]
        Task<OperationResult<VerificationResultModel>> ExpireGroupBuyingAsync(Guid GroupId);
		/// <summary> 获取产品组中产品详细信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupDetailResponse")]
        Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupDetailAsync(string ProductGroupId);
		/// <summary> 根据OrderId查询团信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByOrderIdResponse")]
        Task<OperationResult<GroupInfoModel>> FetchGroupInfoByOrderIdAsync(int OrderId);
		/// <summary> 根据ProductGroupId查询产品组详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchProductGroupInfoById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchProductGroupInfoByIdResponse")]
        Task<OperationResult<ProductGroupModel>> FetchProductGroupInfoByIdAsync(string ProductGroupId);
		/// <summary> 根据UserId和OpenId校验新人 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckNewUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckNewUserResponse")]
        Task<OperationResult<NewUserCheckResultModel>> CheckNewUserAsync(Guid userId, string openId = default(string));
		/// <summary> 根据PID获取该产品组下，该用户可以参加的若干个团(带有TotalCount) </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoWithTotalCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoWithTotalCountResponse")]
        Task<OperationResult<GroupInfoResponse>> SelectGroupInfoWithTotalCountAsync(string ProductGroupId, Guid UserId, int Count = 100);
		/// <summary>拼团推送 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/PushPinTuanMessage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/PushPinTuanMessageResponse")]
        Task<OperationResult<bool>> PushPinTuanMessageAsync(Guid groupId,int batchId);
		/// <summary>根据PID获取所属ProductGroupId以及价格</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetProductGroupInfoByPId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetProductGroupInfoByPIdResponse")]
        Task<OperationResult<GroupBuyingProductInfo>> GetProductGroupInfoByPIdAsync(string pId);
		/// <summary>根据PID获取对应产品的信息(批量)</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductListByPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductListByPidsResponse")]
        Task<OperationResult<List<ProductGroupModel>>> SelectProductListByPidsAsync(List<GroupBuyingProductRequest> request);
		/// <summary>获取抽奖规则</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetLotteryRule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetLotteryRuleResponse")]
        Task<OperationResult<GroupLotteryRuleModel>> GetLotteryRuleAsync(string productGroupId);
		/// <summary>获取中奖名单</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetWinnerList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetWinnerListResponse")]
        Task<OperationResult<PagedModel<GroupBuyingLotteryInfo>>> GetWinnerListAsync(string productGroupId, int level = 0, int pageIndex = 1, int pageSize = 20);
		/// <summary> 查询用户中奖状态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckUserLotteryResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckUserLotteryResultResponse")]
        Task<OperationResult<GroupBuyingLotteryInfo>> CheckUserLotteryResultAsync(Guid userId,string productGroupId,int orderId);
		/// <summary>查询用户的中奖纪录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserLotteryHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserLotteryHistoryResponse")]
        Task<OperationResult<List<GroupBuyingLotteryInfo>>> GetUserLotteryHistoryAsync(Guid userId, List<int> orderIds);
		/// <summary>按照拼团类型获取产品列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetActivityProductGroup", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetActivityProductGroupResponse")]
        Task<OperationResult<PagedModel<string>>> GetActivityProductGroupAsync(ActivityGroupRequest request);
		/// <summary>查询用户免单券</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserFreeCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserFreeCouponResponse")]
        Task<OperationResult<List<FreeCouponModel>>> GetUserFreeCouponAsync(Guid userId);
		/// <summary> 获取用户拼团记录统计 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupCountByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupCountByUserIdResponse")]
        Task<OperationResult<GroupBuyingHistoryCount>> GetUserGroupCountByUserIdAsync(Guid userId);
		/// <summary> 获取最终成团的用户信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupFinalUserList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupFinalUserListResponse")]
        Task<OperationResult<List<GroupFinalUserModel>>> GetGroupFinalUserListAsync(Guid groupId);
		/// <summary> 查询用户限购信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserBuyLimitInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserBuyLimitInfoResponse")]
        Task<OperationResult<GroupBuyingBuyLimitModel>> GetUserBuyLimitInfoAsync(GroupBuyingBuyLimitRequest request);
		/// <summary> 查询拼团拼团类目信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingCategoryInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingCategoryInfoResponse")]
        Task<OperationResult<List<GroupBuyingCategoryModel>>> GetGroupBuyingCategoryInfoAsync();
		/// <summary> 查询拼团产品信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListNewResponse")]
        Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> GetGroupBuyingProductListNewAsync(GroupBuyingQueryRequest request);
		/// <summary> 刷新ES数据 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/UpdateGroupBuyingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/UpdateGroupBuyingInfoResponse")]
        Task<OperationResult<bool>> UpdateGroupBuyingInfoAsync(List<string> productGroupIds);
		/// <summary> 根据关键词搜索拼团产品信息 </summary>
		[Obsolete("已废弃，请使用SelectGroupBuyingListNewAsync",true)]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SearchGroupBuyingByKeyword", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SearchGroupBuyingByKeywordResponse")]
        Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SearchGroupBuyingByKeywordAsync(GroupBuyingQueryRequest request);
		/// <summary> 查询拼团产品列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingListNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingListNewResponse")]
        Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SelectGroupBuyingListNewAsync(GroupBuyingQueryRequest request);
	}

	/// <summary>拼团活动相关</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IGroupBuyingClient : IGroupBuyingService, ITuhuServiceClient
    {
    	/// <summary> 分页获取首页ProductGroupId </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListResponse")]
        OperationResult<PagedModel<string>> GetGroupBuyingProductList(int PageIndex = 1, int PageSize = 10, bool flag = false,string channel=default(string),bool isOldUser=false);
		/// <summary> 根据ProductGroupId获取对应产品的PID </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingProductsById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingProductsByIdResponse")]
        OperationResult<List<string>> SelectGroupBuyingProductsById(string ProductGroupId);
		/// <summary> 根据ProductGroupId获取对应ProductGroup信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupInfoResponse")]
        OperationResult<List<ProductGroupModel>> SelectProductGroupInfo(List<string> ProductGroupIds);
		/// <summary> 根据PID获取对应产品的信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductInfoByPid", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductInfoByPidResponse")]
        OperationResult<GroupBuyingProductModel> SelectProductInfoByPid(string ProductGroupId, string Pid);
		/// <summary> 根据PID获取该产品组下，该用户可以参加的若干个团 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoByProductGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoByProductGroupIdResponse")]
        OperationResult<List<GroupInfoModel>> SelectGroupInfoByProductGroupId(string ProductGroupId, Guid UserId, int Count = 100);
		/// <summary> 根据团号获取拼团信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByGroupIdResponse")]
        OperationResult<GroupInfoModel> FetchGroupInfoByGroupId(Guid GroupId);
		/// <summary> 根据团号获取当前团成员 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupMemberByGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupMemberByGroupIdResponse")]
        OperationResult<GroupMemberModel> SelectGroupMemberByGroupId(Guid GroupId);
		/// <summary> 校验用户的参团资格 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckGroupInfoByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckGroupInfoByUserIdResponse")]
        OperationResult<CheckResultModel> CheckGroupInfoByUserId(Guid GroupId, Guid UserId, string ProductGroupId,string pid=default(string));
		/// <summary> 用户创建新团 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CreateGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CreateGroupBuyingResponse")]
        OperationResult<VerificationResultModel> CreateGroupBuying(Guid UserId, string ProductGroupId, string Pid, int OrderId);
		/// <summary> 用户参与拼团 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/TakePartInGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/TakePartInGroupBuyingResponse")]
        OperationResult<VerificationResultModel> TakePartInGroupBuying(Guid UserId, Guid GroupId, string Pid, int OrderId);
		/// <summary> 分页获取该用户下团号 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupInfoByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupInfoByUserIdResponse")]
        OperationResult<PagedModel<UserGroupBuyingInfoModel>> GetUserGroupInfoByUserId(GroupInfoRequest request);
		/// <summary> 刷新拼团产品缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshCacheResponse")]
        OperationResult<VerificationResultModel> RefreshCache(string ProductGroupId = null);
		/// <summary> 刷新拼团缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshGroupCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshGroupCacheResponse")]
        OperationResult<VerificationResultModel> RefreshGroupCache(Guid GroupId);
		/// <summary> 根据团号，UserId获取用户订单信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchUserOrderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchUserOrderInfoResponse")]
        OperationResult<UserOrderInfoModel> FetchUserOrderInfo(Guid GroupId, Guid UserId);
		/// <summary> 用户取消订单 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CancelGroupBuyingOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CancelGroupBuyingOrderResponse")]
        OperationResult<VerificationResultModel> CancelGroupBuyingOrder(Guid GroupId, int OrderId);
		/// <summary> 团长付款，该团可见 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeGroupBuyingStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeGroupBuyingStatusResponse")]
        OperationResult<bool> ChangeGroupBuyingStatus(Guid GroupId, int OrderId);
		/// <summary> 团员付款加入 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeUserStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeUserStatusResponse")]
        OperationResult<bool> ChangeUserStatus(Guid GroupId, Guid UserId, int OrderId);
		/// <summary> 拼团过期 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ExpireGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ExpireGroupBuyingResponse")]
        OperationResult<VerificationResultModel> ExpireGroupBuying(Guid GroupId);
		/// <summary> 获取产品组中产品详细信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupDetailResponse")]
        OperationResult<List<ProductGroupModel>> SelectProductGroupDetail(string ProductGroupId);
		/// <summary> 根据OrderId查询团信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByOrderIdResponse")]
        OperationResult<GroupInfoModel> FetchGroupInfoByOrderId(int OrderId);
		/// <summary> 根据ProductGroupId查询产品组详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchProductGroupInfoById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchProductGroupInfoByIdResponse")]
        OperationResult<ProductGroupModel> FetchProductGroupInfoById(string ProductGroupId);
		/// <summary> 根据UserId和OpenId校验新人 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckNewUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckNewUserResponse")]
        OperationResult<NewUserCheckResultModel> CheckNewUser(Guid userId, string openId = default(string));
		/// <summary> 根据PID获取该产品组下，该用户可以参加的若干个团(带有TotalCount) </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoWithTotalCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoWithTotalCountResponse")]
        OperationResult<GroupInfoResponse> SelectGroupInfoWithTotalCount(string ProductGroupId, Guid UserId, int Count = 100);
		/// <summary>拼团推送 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/PushPinTuanMessage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/PushPinTuanMessageResponse")]
        OperationResult<bool> PushPinTuanMessage(Guid groupId,int batchId);
		/// <summary>根据PID获取所属ProductGroupId以及价格</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetProductGroupInfoByPId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetProductGroupInfoByPIdResponse")]
        OperationResult<GroupBuyingProductInfo> GetProductGroupInfoByPId(string pId);
		/// <summary>根据PID获取对应产品的信息(批量)</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductListByPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductListByPidsResponse")]
        OperationResult<List<ProductGroupModel>> SelectProductListByPids(List<GroupBuyingProductRequest> request);
		/// <summary>获取抽奖规则</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetLotteryRule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetLotteryRuleResponse")]
        OperationResult<GroupLotteryRuleModel> GetLotteryRule(string productGroupId);
		/// <summary>获取中奖名单</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetWinnerList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetWinnerListResponse")]
        OperationResult<PagedModel<GroupBuyingLotteryInfo>> GetWinnerList(string productGroupId, int level = 0, int pageIndex = 1, int pageSize = 20);
		/// <summary> 查询用户中奖状态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckUserLotteryResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckUserLotteryResultResponse")]
        OperationResult<GroupBuyingLotteryInfo> CheckUserLotteryResult(Guid userId,string productGroupId,int orderId);
		/// <summary>查询用户的中奖纪录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserLotteryHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserLotteryHistoryResponse")]
        OperationResult<List<GroupBuyingLotteryInfo>> GetUserLotteryHistory(Guid userId, List<int> orderIds);
		/// <summary>按照拼团类型获取产品列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetActivityProductGroup", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetActivityProductGroupResponse")]
        OperationResult<PagedModel<string>> GetActivityProductGroup(ActivityGroupRequest request);
		/// <summary>查询用户免单券</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserFreeCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserFreeCouponResponse")]
        OperationResult<List<FreeCouponModel>> GetUserFreeCoupon(Guid userId);
		/// <summary> 获取用户拼团记录统计 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupCountByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupCountByUserIdResponse")]
        OperationResult<GroupBuyingHistoryCount> GetUserGroupCountByUserId(Guid userId);
		/// <summary> 获取最终成团的用户信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupFinalUserList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupFinalUserListResponse")]
        OperationResult<List<GroupFinalUserModel>> GetGroupFinalUserList(Guid groupId);
		/// <summary> 查询用户限购信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserBuyLimitInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserBuyLimitInfoResponse")]
        OperationResult<GroupBuyingBuyLimitModel> GetUserBuyLimitInfo(GroupBuyingBuyLimitRequest request);
		/// <summary> 查询拼团拼团类目信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingCategoryInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingCategoryInfoResponse")]
        OperationResult<List<GroupBuyingCategoryModel>> GetGroupBuyingCategoryInfo();
		/// <summary> 查询拼团产品信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListNewResponse")]
        OperationResult<PagedModel<SimplegroupBuyingModel>> GetGroupBuyingProductListNew(GroupBuyingQueryRequest request);
		/// <summary> 刷新ES数据 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/UpdateGroupBuyingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/UpdateGroupBuyingInfoResponse")]
        OperationResult<bool> UpdateGroupBuyingInfo(List<string> productGroupIds);
		/// <summary> 根据关键词搜索拼团产品信息 </summary>
		[Obsolete("已废弃，请使用SelectGroupBuyingListNewAsync",true)]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SearchGroupBuyingByKeyword", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SearchGroupBuyingByKeywordResponse")]
        OperationResult<PagedModel<SimplegroupBuyingModel>> SearchGroupBuyingByKeyword(GroupBuyingQueryRequest request);
		/// <summary> 查询拼团产品列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingListNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingListNewResponse")]
        OperationResult<PagedModel<SimplegroupBuyingModel>> SelectGroupBuyingListNew(GroupBuyingQueryRequest request);
	}

	/// <summary>拼团活动相关</summary>
	public partial class GroupBuyingClient : TuhuServiceClient<IGroupBuyingClient>, IGroupBuyingClient
    {
    	/// <summary> 分页获取首页ProductGroupId </summary>
        public OperationResult<PagedModel<string>> GetGroupBuyingProductList(int PageIndex = 1, int PageSize = 10, bool flag = false,string channel=default(string),bool isOldUser=false) => Invoke(_ => _.GetGroupBuyingProductList(PageIndex,PageSize,flag,channel,isOldUser));

	/// <summary> 分页获取首页ProductGroupId </summary>
        public Task<OperationResult<PagedModel<string>>> GetGroupBuyingProductListAsync(int PageIndex = 1, int PageSize = 10, bool flag = false,string channel=default(string),bool isOldUser=false) => InvokeAsync(_ => _.GetGroupBuyingProductListAsync(PageIndex,PageSize,flag,channel,isOldUser));
		/// <summary> 根据ProductGroupId获取对应产品的PID </summary>
        public OperationResult<List<string>> SelectGroupBuyingProductsById(string ProductGroupId) => Invoke(_ => _.SelectGroupBuyingProductsById(ProductGroupId));

	/// <summary> 根据ProductGroupId获取对应产品的PID </summary>
        public Task<OperationResult<List<string>>> SelectGroupBuyingProductsByIdAsync(string ProductGroupId) => InvokeAsync(_ => _.SelectGroupBuyingProductsByIdAsync(ProductGroupId));
		/// <summary> 根据ProductGroupId获取对应ProductGroup信息 </summary>
        public OperationResult<List<ProductGroupModel>> SelectProductGroupInfo(List<string> ProductGroupIds) => Invoke(_ => _.SelectProductGroupInfo(ProductGroupIds));

	/// <summary> 根据ProductGroupId获取对应ProductGroup信息 </summary>
        public Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupInfoAsync(List<string> ProductGroupIds) => InvokeAsync(_ => _.SelectProductGroupInfoAsync(ProductGroupIds));
		/// <summary> 根据PID获取对应产品的信息 </summary>
        public OperationResult<GroupBuyingProductModel> SelectProductInfoByPid(string ProductGroupId, string Pid) => Invoke(_ => _.SelectProductInfoByPid(ProductGroupId, Pid));

	/// <summary> 根据PID获取对应产品的信息 </summary>
        public Task<OperationResult<GroupBuyingProductModel>> SelectProductInfoByPidAsync(string ProductGroupId, string Pid) => InvokeAsync(_ => _.SelectProductInfoByPidAsync(ProductGroupId, Pid));
		/// <summary> 根据PID获取该产品组下，该用户可以参加的若干个团 </summary>
        public OperationResult<List<GroupInfoModel>> SelectGroupInfoByProductGroupId(string ProductGroupId, Guid UserId, int Count = 100) => Invoke(_ => _.SelectGroupInfoByProductGroupId(ProductGroupId, UserId, Count));

	/// <summary> 根据PID获取该产品组下，该用户可以参加的若干个团 </summary>
        public Task<OperationResult<List<GroupInfoModel>>> SelectGroupInfoByProductGroupIdAsync(string ProductGroupId, Guid UserId, int Count = 100) => InvokeAsync(_ => _.SelectGroupInfoByProductGroupIdAsync(ProductGroupId, UserId, Count));
		/// <summary> 根据团号获取拼团信息 </summary>
        public OperationResult<GroupInfoModel> FetchGroupInfoByGroupId(Guid GroupId) => Invoke(_ => _.FetchGroupInfoByGroupId(GroupId));

	/// <summary> 根据团号获取拼团信息 </summary>
        public Task<OperationResult<GroupInfoModel>> FetchGroupInfoByGroupIdAsync(Guid GroupId) => InvokeAsync(_ => _.FetchGroupInfoByGroupIdAsync(GroupId));
		/// <summary> 根据团号获取当前团成员 </summary>
        public OperationResult<GroupMemberModel> SelectGroupMemberByGroupId(Guid GroupId) => Invoke(_ => _.SelectGroupMemberByGroupId(GroupId));

	/// <summary> 根据团号获取当前团成员 </summary>
        public Task<OperationResult<GroupMemberModel>> SelectGroupMemberByGroupIdAsync(Guid GroupId) => InvokeAsync(_ => _.SelectGroupMemberByGroupIdAsync(GroupId));
		/// <summary> 校验用户的参团资格 </summary>
        public OperationResult<CheckResultModel> CheckGroupInfoByUserId(Guid GroupId, Guid UserId, string ProductGroupId,string pid=default(string)) => Invoke(_ => _.CheckGroupInfoByUserId(GroupId, UserId, ProductGroupId, pid));

	/// <summary> 校验用户的参团资格 </summary>
        public Task<OperationResult<CheckResultModel>> CheckGroupInfoByUserIdAsync(Guid GroupId, Guid UserId, string ProductGroupId,string pid=default(string)) => InvokeAsync(_ => _.CheckGroupInfoByUserIdAsync(GroupId, UserId, ProductGroupId, pid));
		/// <summary> 用户创建新团 </summary>
        public OperationResult<VerificationResultModel> CreateGroupBuying(Guid UserId, string ProductGroupId, string Pid, int OrderId) => Invoke(_ => _.CreateGroupBuying(UserId, ProductGroupId, Pid, OrderId));

	/// <summary> 用户创建新团 </summary>
        public Task<OperationResult<VerificationResultModel>> CreateGroupBuyingAsync(Guid UserId, string ProductGroupId, string Pid, int OrderId) => InvokeAsync(_ => _.CreateGroupBuyingAsync(UserId, ProductGroupId, Pid, OrderId));
		/// <summary> 用户参与拼团 </summary>
        public OperationResult<VerificationResultModel> TakePartInGroupBuying(Guid UserId, Guid GroupId, string Pid, int OrderId) => Invoke(_ => _.TakePartInGroupBuying(UserId, GroupId, Pid, OrderId));

	/// <summary> 用户参与拼团 </summary>
        public Task<OperationResult<VerificationResultModel>> TakePartInGroupBuyingAsync(Guid UserId, Guid GroupId, string Pid, int OrderId) => InvokeAsync(_ => _.TakePartInGroupBuyingAsync(UserId, GroupId, Pid, OrderId));
		/// <summary> 分页获取该用户下团号 </summary>
        public OperationResult<PagedModel<UserGroupBuyingInfoModel>> GetUserGroupInfoByUserId(GroupInfoRequest request) => Invoke(_ => _.GetUserGroupInfoByUserId(request));

	/// <summary> 分页获取该用户下团号 </summary>
        public Task<OperationResult<PagedModel<UserGroupBuyingInfoModel>>> GetUserGroupInfoByUserIdAsync(GroupInfoRequest request) => InvokeAsync(_ => _.GetUserGroupInfoByUserIdAsync(request));
		/// <summary> 刷新拼团产品缓存 </summary>
        public OperationResult<VerificationResultModel> RefreshCache(string ProductGroupId = null) => Invoke(_ => _.RefreshCache(ProductGroupId));

	/// <summary> 刷新拼团产品缓存 </summary>
        public Task<OperationResult<VerificationResultModel>> RefreshCacheAsync(string ProductGroupId = null) => InvokeAsync(_ => _.RefreshCacheAsync(ProductGroupId));
		/// <summary> 刷新拼团缓存 </summary>
        public OperationResult<VerificationResultModel> RefreshGroupCache(Guid GroupId) => Invoke(_ => _.RefreshGroupCache(GroupId));

	/// <summary> 刷新拼团缓存 </summary>
        public Task<OperationResult<VerificationResultModel>> RefreshGroupCacheAsync(Guid GroupId) => InvokeAsync(_ => _.RefreshGroupCacheAsync(GroupId));
		/// <summary> 根据团号，UserId获取用户订单信息 </summary>
        public OperationResult<UserOrderInfoModel> FetchUserOrderInfo(Guid GroupId, Guid UserId) => Invoke(_ => _.FetchUserOrderInfo(GroupId, UserId));

	/// <summary> 根据团号，UserId获取用户订单信息 </summary>
        public Task<OperationResult<UserOrderInfoModel>> FetchUserOrderInfoAsync(Guid GroupId, Guid UserId) => InvokeAsync(_ => _.FetchUserOrderInfoAsync(GroupId, UserId));
		/// <summary> 用户取消订单 </summary>
        public OperationResult<VerificationResultModel> CancelGroupBuyingOrder(Guid GroupId, int OrderId) => Invoke(_ => _.CancelGroupBuyingOrder(GroupId, OrderId));

	/// <summary> 用户取消订单 </summary>
        public Task<OperationResult<VerificationResultModel>> CancelGroupBuyingOrderAsync(Guid GroupId, int OrderId) => InvokeAsync(_ => _.CancelGroupBuyingOrderAsync(GroupId, OrderId));
		/// <summary> 团长付款，该团可见 </summary>
        public OperationResult<bool> ChangeGroupBuyingStatus(Guid GroupId, int OrderId) => Invoke(_ => _.ChangeGroupBuyingStatus(GroupId, OrderId));

	/// <summary> 团长付款，该团可见 </summary>
        public Task<OperationResult<bool>> ChangeGroupBuyingStatusAsync(Guid GroupId, int OrderId) => InvokeAsync(_ => _.ChangeGroupBuyingStatusAsync(GroupId, OrderId));
		/// <summary> 团员付款加入 </summary>
        public OperationResult<bool> ChangeUserStatus(Guid GroupId, Guid UserId, int OrderId) => Invoke(_ => _.ChangeUserStatus(GroupId, UserId, OrderId));

	/// <summary> 团员付款加入 </summary>
        public Task<OperationResult<bool>> ChangeUserStatusAsync(Guid GroupId, Guid UserId, int OrderId) => InvokeAsync(_ => _.ChangeUserStatusAsync(GroupId, UserId, OrderId));
		/// <summary> 拼团过期 </summary>
        public OperationResult<VerificationResultModel> ExpireGroupBuying(Guid GroupId) => Invoke(_ => _.ExpireGroupBuying(GroupId));

	/// <summary> 拼团过期 </summary>
        public Task<OperationResult<VerificationResultModel>> ExpireGroupBuyingAsync(Guid GroupId) => InvokeAsync(_ => _.ExpireGroupBuyingAsync(GroupId));
		/// <summary> 获取产品组中产品详细信息 </summary>
        public OperationResult<List<ProductGroupModel>> SelectProductGroupDetail(string ProductGroupId) => Invoke(_ => _.SelectProductGroupDetail(ProductGroupId));

	/// <summary> 获取产品组中产品详细信息 </summary>
        public Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupDetailAsync(string ProductGroupId) => InvokeAsync(_ => _.SelectProductGroupDetailAsync(ProductGroupId));
		/// <summary> 根据OrderId查询团信息 </summary>
        public OperationResult<GroupInfoModel> FetchGroupInfoByOrderId(int OrderId) => Invoke(_ => _.FetchGroupInfoByOrderId(OrderId));

	/// <summary> 根据OrderId查询团信息 </summary>
        public Task<OperationResult<GroupInfoModel>> FetchGroupInfoByOrderIdAsync(int OrderId) => InvokeAsync(_ => _.FetchGroupInfoByOrderIdAsync(OrderId));
		/// <summary> 根据ProductGroupId查询产品组详情 </summary>
        public OperationResult<ProductGroupModel> FetchProductGroupInfoById(string ProductGroupId) => Invoke(_ => _.FetchProductGroupInfoById(ProductGroupId));

	/// <summary> 根据ProductGroupId查询产品组详情 </summary>
        public Task<OperationResult<ProductGroupModel>> FetchProductGroupInfoByIdAsync(string ProductGroupId) => InvokeAsync(_ => _.FetchProductGroupInfoByIdAsync(ProductGroupId));
		/// <summary> 根据UserId和OpenId校验新人 </summary>
        public OperationResult<NewUserCheckResultModel> CheckNewUser(Guid userId, string openId = default(string)) => Invoke(_ => _.CheckNewUser(userId,openId));

	/// <summary> 根据UserId和OpenId校验新人 </summary>
        public Task<OperationResult<NewUserCheckResultModel>> CheckNewUserAsync(Guid userId, string openId = default(string)) => InvokeAsync(_ => _.CheckNewUserAsync(userId,openId));
		/// <summary> 根据PID获取该产品组下，该用户可以参加的若干个团(带有TotalCount) </summary>
        public OperationResult<GroupInfoResponse> SelectGroupInfoWithTotalCount(string ProductGroupId, Guid UserId, int Count = 100) => Invoke(_ => _.SelectGroupInfoWithTotalCount(ProductGroupId, UserId, Count));

	/// <summary> 根据PID获取该产品组下，该用户可以参加的若干个团(带有TotalCount) </summary>
        public Task<OperationResult<GroupInfoResponse>> SelectGroupInfoWithTotalCountAsync(string ProductGroupId, Guid UserId, int Count = 100) => InvokeAsync(_ => _.SelectGroupInfoWithTotalCountAsync(ProductGroupId, UserId, Count));
		/// <summary>拼团推送 </summary>
        public OperationResult<bool> PushPinTuanMessage(Guid groupId,int batchId) => Invoke(_ => _.PushPinTuanMessage(groupId,batchId));

	/// <summary>拼团推送 </summary>
        public Task<OperationResult<bool>> PushPinTuanMessageAsync(Guid groupId,int batchId) => InvokeAsync(_ => _.PushPinTuanMessageAsync(groupId,batchId));
		/// <summary>根据PID获取所属ProductGroupId以及价格</summary>
        public OperationResult<GroupBuyingProductInfo> GetProductGroupInfoByPId(string pId) => Invoke(_ => _.GetProductGroupInfoByPId(pId));

	/// <summary>根据PID获取所属ProductGroupId以及价格</summary>
        public Task<OperationResult<GroupBuyingProductInfo>> GetProductGroupInfoByPIdAsync(string pId) => InvokeAsync(_ => _.GetProductGroupInfoByPIdAsync(pId));
		/// <summary>根据PID获取对应产品的信息(批量)</summary>
        public OperationResult<List<ProductGroupModel>> SelectProductListByPids(List<GroupBuyingProductRequest> request) => Invoke(_ => _.SelectProductListByPids(request));

	/// <summary>根据PID获取对应产品的信息(批量)</summary>
        public Task<OperationResult<List<ProductGroupModel>>> SelectProductListByPidsAsync(List<GroupBuyingProductRequest> request) => InvokeAsync(_ => _.SelectProductListByPidsAsync(request));
		/// <summary>获取抽奖规则</summary>
        public OperationResult<GroupLotteryRuleModel> GetLotteryRule(string productGroupId) => Invoke(_ => _.GetLotteryRule(productGroupId));

	/// <summary>获取抽奖规则</summary>
        public Task<OperationResult<GroupLotteryRuleModel>> GetLotteryRuleAsync(string productGroupId) => InvokeAsync(_ => _.GetLotteryRuleAsync(productGroupId));
		/// <summary>获取中奖名单</summary>
        public OperationResult<PagedModel<GroupBuyingLotteryInfo>> GetWinnerList(string productGroupId, int level = 0, int pageIndex = 1, int pageSize = 20) => Invoke(_ => _.GetWinnerList(productGroupId,level,pageIndex,pageSize));

	/// <summary>获取中奖名单</summary>
        public Task<OperationResult<PagedModel<GroupBuyingLotteryInfo>>> GetWinnerListAsync(string productGroupId, int level = 0, int pageIndex = 1, int pageSize = 20) => InvokeAsync(_ => _.GetWinnerListAsync(productGroupId,level,pageIndex,pageSize));
		/// <summary> 查询用户中奖状态 </summary>
        public OperationResult<GroupBuyingLotteryInfo> CheckUserLotteryResult(Guid userId,string productGroupId,int orderId) => Invoke(_ => _.CheckUserLotteryResult(userId,productGroupId,orderId));

	/// <summary> 查询用户中奖状态 </summary>
        public Task<OperationResult<GroupBuyingLotteryInfo>> CheckUserLotteryResultAsync(Guid userId,string productGroupId,int orderId) => InvokeAsync(_ => _.CheckUserLotteryResultAsync(userId,productGroupId,orderId));
		/// <summary>查询用户的中奖纪录</summary>
        public OperationResult<List<GroupBuyingLotteryInfo>> GetUserLotteryHistory(Guid userId, List<int> orderIds) => Invoke(_ => _.GetUserLotteryHistory(userId,orderIds));

	/// <summary>查询用户的中奖纪录</summary>
        public Task<OperationResult<List<GroupBuyingLotteryInfo>>> GetUserLotteryHistoryAsync(Guid userId, List<int> orderIds) => InvokeAsync(_ => _.GetUserLotteryHistoryAsync(userId,orderIds));
		/// <summary>按照拼团类型获取产品列表</summary>
        public OperationResult<PagedModel<string>> GetActivityProductGroup(ActivityGroupRequest request) => Invoke(_ => _.GetActivityProductGroup(request));

	/// <summary>按照拼团类型获取产品列表</summary>
        public Task<OperationResult<PagedModel<string>>> GetActivityProductGroupAsync(ActivityGroupRequest request) => InvokeAsync(_ => _.GetActivityProductGroupAsync(request));
		/// <summary>查询用户免单券</summary>
        public OperationResult<List<FreeCouponModel>> GetUserFreeCoupon(Guid userId) => Invoke(_ => _.GetUserFreeCoupon(userId));

	/// <summary>查询用户免单券</summary>
        public Task<OperationResult<List<FreeCouponModel>>> GetUserFreeCouponAsync(Guid userId) => InvokeAsync(_ => _.GetUserFreeCouponAsync(userId));
		/// <summary> 获取用户拼团记录统计 </summary>
        public OperationResult<GroupBuyingHistoryCount> GetUserGroupCountByUserId(Guid userId) => Invoke(_ => _.GetUserGroupCountByUserId(userId));

	/// <summary> 获取用户拼团记录统计 </summary>
        public Task<OperationResult<GroupBuyingHistoryCount>> GetUserGroupCountByUserIdAsync(Guid userId) => InvokeAsync(_ => _.GetUserGroupCountByUserIdAsync(userId));
		/// <summary> 获取最终成团的用户信息 </summary>
        public OperationResult<List<GroupFinalUserModel>> GetGroupFinalUserList(Guid groupId) => Invoke(_ => _.GetGroupFinalUserList(groupId));

	/// <summary> 获取最终成团的用户信息 </summary>
        public Task<OperationResult<List<GroupFinalUserModel>>> GetGroupFinalUserListAsync(Guid groupId) => InvokeAsync(_ => _.GetGroupFinalUserListAsync(groupId));
		/// <summary> 查询用户限购信息 </summary>
        public OperationResult<GroupBuyingBuyLimitModel> GetUserBuyLimitInfo(GroupBuyingBuyLimitRequest request) => Invoke(_ => _.GetUserBuyLimitInfo(request));

	/// <summary> 查询用户限购信息 </summary>
        public Task<OperationResult<GroupBuyingBuyLimitModel>> GetUserBuyLimitInfoAsync(GroupBuyingBuyLimitRequest request) => InvokeAsync(_ => _.GetUserBuyLimitInfoAsync(request));
		/// <summary> 查询拼团拼团类目信息 </summary>
        public OperationResult<List<GroupBuyingCategoryModel>> GetGroupBuyingCategoryInfo() => Invoke(_ => _.GetGroupBuyingCategoryInfo());

	/// <summary> 查询拼团拼团类目信息 </summary>
        public Task<OperationResult<List<GroupBuyingCategoryModel>>> GetGroupBuyingCategoryInfoAsync() => InvokeAsync(_ => _.GetGroupBuyingCategoryInfoAsync());
		/// <summary> 查询拼团产品信息 </summary>
        public OperationResult<PagedModel<SimplegroupBuyingModel>> GetGroupBuyingProductListNew(GroupBuyingQueryRequest request) => Invoke(_ => _.GetGroupBuyingProductListNew(request));

	/// <summary> 查询拼团产品信息 </summary>
        public Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> GetGroupBuyingProductListNewAsync(GroupBuyingQueryRequest request) => InvokeAsync(_ => _.GetGroupBuyingProductListNewAsync(request));
		/// <summary> 刷新ES数据 </summary>
        public OperationResult<bool> UpdateGroupBuyingInfo(List<string> productGroupIds) => Invoke(_ => _.UpdateGroupBuyingInfo(productGroupIds));

	/// <summary> 刷新ES数据 </summary>
        public Task<OperationResult<bool>> UpdateGroupBuyingInfoAsync(List<string> productGroupIds) => InvokeAsync(_ => _.UpdateGroupBuyingInfoAsync(productGroupIds));
		/// <summary> 根据关键词搜索拼团产品信息 </summary>
		[Obsolete("已废弃，请使用SelectGroupBuyingListNewAsync",true)]
        public OperationResult<PagedModel<SimplegroupBuyingModel>> SearchGroupBuyingByKeyword(GroupBuyingQueryRequest request) => Invoke(_ => _.SearchGroupBuyingByKeyword(request));

	/// <summary> 根据关键词搜索拼团产品信息 </summary>
		[Obsolete("已废弃，请使用SelectGroupBuyingListNewAsync",true)]
        public Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SearchGroupBuyingByKeywordAsync(GroupBuyingQueryRequest request) => InvokeAsync(_ => _.SearchGroupBuyingByKeywordAsync(request));
		/// <summary> 查询拼团产品列表 </summary>
        public OperationResult<PagedModel<SimplegroupBuyingModel>> SelectGroupBuyingListNew(GroupBuyingQueryRequest request) => Invoke(_ => _.SelectGroupBuyingListNew(request));

	/// <summary> 查询拼团产品列表 </summary>
        public Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SelectGroupBuyingListNewAsync(GroupBuyingQueryRequest request) => InvokeAsync(_ => _.SelectGroupBuyingListNewAsync(request));
	}
	///<summary>六周年投票</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISexAnnualVoteService
    {
    	/// <summary> 添加门店报名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopSignUpResponse")]
        Task<OperationResult<bool>> AddShopSignUpAsync(ShopVoteModel model);
		/// <summary> 添加技师报名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddEmployeeSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddEmployeeSignUpResponse")]
        Task<OperationResult<bool>> AddEmployeeSignUpAsync(ShopEmployeeVoteModel model);
		/// <summary> 验证门店是否已经报过名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckShopSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckShopSignUpResponse")]
        Task<OperationResult<bool>> CheckShopSignUpAsync(long shopId);
		/// <summary> 验证技师是否已经报过名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckEmployeeSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckEmployeeSignUpResponse")]
        Task<OperationResult<bool>> CheckEmployeeSignUpAsync(long shopId,long employeeId);
		/// <summary> 查询门店投票排名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopRanking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopRankingResponse")]
        Task<OperationResult<PagedModel<ShopVoteBaseModel>>> SelectShopRankingAsync(SexAnnualVoteQueryRequest query);
		/// <summary> 查询技师投票排名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeRanking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeRankingResponse")]
        Task<OperationResult<PagedModel<ShopEmployeeVoteBaseModel>>> SelectShopEmployeeRankingAsync(SexAnnualVoteQueryRequest query);
		/// <summary> 根据pkid查询门店基本详情（ShopId） </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopBaseInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopBaseInfoResponse")]
        Task<OperationResult<ShopVoteModel>> FetchShopBaseInfoAsync(long pkid);
		/// <summary> 查询门店详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopDetailResponse")]
        Task<OperationResult<ShopVoteModel>> FetchShopDetailAsync(long shopId);
		/// <summary> 根据pkid查询技师基本信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeBaseInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeBaseInfoResponse")]
        Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeBaseInfoAsync(long pkid);
		/// <summary> 查询技师详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeDetailResponse")]
        Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeDetailAsync(long shopId,long employeeId);
		/// <summary> 给门店投票 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopVoteResponse")]
        Task<OperationResult<bool>> AddShopVoteAsync(Guid userId,long shopId);
		/// <summary> 获取某个用户时间段内的门店投票记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopVoteRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopVoteRecordResponse")]
        Task<OperationResult<IEnumerable<ShopVoteRecordModel>>> SelectShopVoteRecordAsync(Guid userId,DateTime startDate,DateTime endDate);
		/// <summary> 分享门店投票信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopVoteResponse")]
        Task<OperationResult<bool>> AddShareShopVoteAsync(Guid userId,long shopId);
		/// <summary> 给技师投票 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopEmployeeVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopEmployeeVoteResponse")]
        Task<OperationResult<bool>> AddShopEmployeeVoteAsync(Guid userId,long shopId,long employeeId);
		/// <summary> 获取某个用户时间段内的技师投票记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeVoteRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeVoteRecordResponse")]
        Task<OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>>> SelectShopEmployeeVoteRecordAsync(Guid userId,DateTime startDate,DateTime endDate);
		/// <summary> 分享技师投票信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopEmployeeVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopEmployeeVoteResponse")]
        Task<OperationResult<bool>> AddShareShopEmployeeVoteAsync(Guid userId,long shopId,long employeeId);
		/// <summary> 获取门店报名城市二级联动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopRegion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopRegionResponse")]
        Task<OperationResult<IDictionary<int,Shop.Models.RegionModel>>> GetShopRegionAsync();
		/// <summary> 获取技师报名城市二级联动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopEmployeeRegion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopEmployeeRegionResponse")]
        Task<OperationResult<IDictionary<int,Shop.Models.RegionModel>>> GetShopEmployeeRegionAsync();
	}

	///<summary>六周年投票</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISexAnnualVoteClient : ISexAnnualVoteService, ITuhuServiceClient
    {
    	/// <summary> 添加门店报名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopSignUpResponse")]
        OperationResult<bool> AddShopSignUp(ShopVoteModel model);
		/// <summary> 添加技师报名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddEmployeeSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddEmployeeSignUpResponse")]
        OperationResult<bool> AddEmployeeSignUp(ShopEmployeeVoteModel model);
		/// <summary> 验证门店是否已经报过名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckShopSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckShopSignUpResponse")]
        OperationResult<bool> CheckShopSignUp(long shopId);
		/// <summary> 验证技师是否已经报过名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckEmployeeSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckEmployeeSignUpResponse")]
        OperationResult<bool> CheckEmployeeSignUp(long shopId,long employeeId);
		/// <summary> 查询门店投票排名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopRanking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopRankingResponse")]
        OperationResult<PagedModel<ShopVoteBaseModel>> SelectShopRanking(SexAnnualVoteQueryRequest query);
		/// <summary> 查询技师投票排名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeRanking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeRankingResponse")]
        OperationResult<PagedModel<ShopEmployeeVoteBaseModel>> SelectShopEmployeeRanking(SexAnnualVoteQueryRequest query);
		/// <summary> 根据pkid查询门店基本详情（ShopId） </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopBaseInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopBaseInfoResponse")]
        OperationResult<ShopVoteModel> FetchShopBaseInfo(long pkid);
		/// <summary> 查询门店详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopDetailResponse")]
        OperationResult<ShopVoteModel> FetchShopDetail(long shopId);
		/// <summary> 根据pkid查询技师基本信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeBaseInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeBaseInfoResponse")]
        OperationResult<ShopEmployeeVoteModel> FetchShopEmployeeBaseInfo(long pkid);
		/// <summary> 查询技师详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeDetailResponse")]
        OperationResult<ShopEmployeeVoteModel> FetchShopEmployeeDetail(long shopId,long employeeId);
		/// <summary> 给门店投票 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopVoteResponse")]
        OperationResult<bool> AddShopVote(Guid userId,long shopId);
		/// <summary> 获取某个用户时间段内的门店投票记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopVoteRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopVoteRecordResponse")]
        OperationResult<IEnumerable<ShopVoteRecordModel>> SelectShopVoteRecord(Guid userId,DateTime startDate,DateTime endDate);
		/// <summary> 分享门店投票信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopVoteResponse")]
        OperationResult<bool> AddShareShopVote(Guid userId,long shopId);
		/// <summary> 给技师投票 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopEmployeeVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopEmployeeVoteResponse")]
        OperationResult<bool> AddShopEmployeeVote(Guid userId,long shopId,long employeeId);
		/// <summary> 获取某个用户时间段内的技师投票记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeVoteRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeVoteRecordResponse")]
        OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>> SelectShopEmployeeVoteRecord(Guid userId,DateTime startDate,DateTime endDate);
		/// <summary> 分享技师投票信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopEmployeeVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopEmployeeVoteResponse")]
        OperationResult<bool> AddShareShopEmployeeVote(Guid userId,long shopId,long employeeId);
		/// <summary> 获取门店报名城市二级联动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopRegion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopRegionResponse")]
        OperationResult<IDictionary<int,Shop.Models.RegionModel>> GetShopRegion();
		/// <summary> 获取技师报名城市二级联动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopEmployeeRegion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopEmployeeRegionResponse")]
        OperationResult<IDictionary<int,Shop.Models.RegionModel>> GetShopEmployeeRegion();
	}

	///<summary>六周年投票</summary>///
	public partial class SexAnnualVoteClient : TuhuServiceClient<ISexAnnualVoteClient>, ISexAnnualVoteClient
    {
    	/// <summary> 添加门店报名 </summary>
        public OperationResult<bool> AddShopSignUp(ShopVoteModel model) => Invoke(_ => _.AddShopSignUp(model));

	/// <summary> 添加门店报名 </summary>
        public Task<OperationResult<bool>> AddShopSignUpAsync(ShopVoteModel model) => InvokeAsync(_ => _.AddShopSignUpAsync(model));
		/// <summary> 添加技师报名 </summary>
        public OperationResult<bool> AddEmployeeSignUp(ShopEmployeeVoteModel model) => Invoke(_ => _.AddEmployeeSignUp(model));

	/// <summary> 添加技师报名 </summary>
        public Task<OperationResult<bool>> AddEmployeeSignUpAsync(ShopEmployeeVoteModel model) => InvokeAsync(_ => _.AddEmployeeSignUpAsync(model));
		/// <summary> 验证门店是否已经报过名 </summary>
        public OperationResult<bool> CheckShopSignUp(long shopId) => Invoke(_ => _.CheckShopSignUp(shopId));

	/// <summary> 验证门店是否已经报过名 </summary>
        public Task<OperationResult<bool>> CheckShopSignUpAsync(long shopId) => InvokeAsync(_ => _.CheckShopSignUpAsync(shopId));
		/// <summary> 验证技师是否已经报过名 </summary>
        public OperationResult<bool> CheckEmployeeSignUp(long shopId,long employeeId) => Invoke(_ => _.CheckEmployeeSignUp(shopId,employeeId));

	/// <summary> 验证技师是否已经报过名 </summary>
        public Task<OperationResult<bool>> CheckEmployeeSignUpAsync(long shopId,long employeeId) => InvokeAsync(_ => _.CheckEmployeeSignUpAsync(shopId,employeeId));
		/// <summary> 查询门店投票排名 </summary>
        public OperationResult<PagedModel<ShopVoteBaseModel>> SelectShopRanking(SexAnnualVoteQueryRequest query) => Invoke(_ => _.SelectShopRanking(query));

	/// <summary> 查询门店投票排名 </summary>
        public Task<OperationResult<PagedModel<ShopVoteBaseModel>>> SelectShopRankingAsync(SexAnnualVoteQueryRequest query) => InvokeAsync(_ => _.SelectShopRankingAsync(query));
		/// <summary> 查询技师投票排名 </summary>
        public OperationResult<PagedModel<ShopEmployeeVoteBaseModel>> SelectShopEmployeeRanking(SexAnnualVoteQueryRequest query) => Invoke(_ => _.SelectShopEmployeeRanking(query));

	/// <summary> 查询技师投票排名 </summary>
        public Task<OperationResult<PagedModel<ShopEmployeeVoteBaseModel>>> SelectShopEmployeeRankingAsync(SexAnnualVoteQueryRequest query) => InvokeAsync(_ => _.SelectShopEmployeeRankingAsync(query));
		/// <summary> 根据pkid查询门店基本详情（ShopId） </summary>
        public OperationResult<ShopVoteModel> FetchShopBaseInfo(long pkid) => Invoke(_ => _.FetchShopBaseInfo(pkid));

	/// <summary> 根据pkid查询门店基本详情（ShopId） </summary>
        public Task<OperationResult<ShopVoteModel>> FetchShopBaseInfoAsync(long pkid) => InvokeAsync(_ => _.FetchShopBaseInfoAsync(pkid));
		/// <summary> 查询门店详情 </summary>
        public OperationResult<ShopVoteModel> FetchShopDetail(long shopId) => Invoke(_ => _.FetchShopDetail(shopId));

	/// <summary> 查询门店详情 </summary>
        public Task<OperationResult<ShopVoteModel>> FetchShopDetailAsync(long shopId) => InvokeAsync(_ => _.FetchShopDetailAsync(shopId));
		/// <summary> 根据pkid查询技师基本信息 </summary>
        public OperationResult<ShopEmployeeVoteModel> FetchShopEmployeeBaseInfo(long pkid) => Invoke(_ => _.FetchShopEmployeeBaseInfo(pkid));

	/// <summary> 根据pkid查询技师基本信息 </summary>
        public Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeBaseInfoAsync(long pkid) => InvokeAsync(_ => _.FetchShopEmployeeBaseInfoAsync(pkid));
		/// <summary> 查询技师详情 </summary>
        public OperationResult<ShopEmployeeVoteModel> FetchShopEmployeeDetail(long shopId,long employeeId) => Invoke(_ => _.FetchShopEmployeeDetail(shopId,employeeId));

	/// <summary> 查询技师详情 </summary>
        public Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeDetailAsync(long shopId,long employeeId) => InvokeAsync(_ => _.FetchShopEmployeeDetailAsync(shopId,employeeId));
		/// <summary> 给门店投票 </summary>
        public OperationResult<bool> AddShopVote(Guid userId,long shopId) => Invoke(_ => _.AddShopVote(userId,shopId));

	/// <summary> 给门店投票 </summary>
        public Task<OperationResult<bool>> AddShopVoteAsync(Guid userId,long shopId) => InvokeAsync(_ => _.AddShopVoteAsync(userId,shopId));
		/// <summary> 获取某个用户时间段内的门店投票记录 </summary>
        public OperationResult<IEnumerable<ShopVoteRecordModel>> SelectShopVoteRecord(Guid userId,DateTime startDate,DateTime endDate) => Invoke(_ => _.SelectShopVoteRecord(userId,startDate,endDate));

	/// <summary> 获取某个用户时间段内的门店投票记录 </summary>
        public Task<OperationResult<IEnumerable<ShopVoteRecordModel>>> SelectShopVoteRecordAsync(Guid userId,DateTime startDate,DateTime endDate) => InvokeAsync(_ => _.SelectShopVoteRecordAsync(userId,startDate,endDate));
		/// <summary> 分享门店投票信息 </summary>
        public OperationResult<bool> AddShareShopVote(Guid userId,long shopId) => Invoke(_ => _.AddShareShopVote(userId,shopId));

	/// <summary> 分享门店投票信息 </summary>
        public Task<OperationResult<bool>> AddShareShopVoteAsync(Guid userId,long shopId) => InvokeAsync(_ => _.AddShareShopVoteAsync(userId,shopId));
		/// <summary> 给技师投票 </summary>
        public OperationResult<bool> AddShopEmployeeVote(Guid userId,long shopId,long employeeId) => Invoke(_ => _.AddShopEmployeeVote(userId,shopId,employeeId));

	/// <summary> 给技师投票 </summary>
        public Task<OperationResult<bool>> AddShopEmployeeVoteAsync(Guid userId,long shopId,long employeeId) => InvokeAsync(_ => _.AddShopEmployeeVoteAsync(userId,shopId,employeeId));
		/// <summary> 获取某个用户时间段内的技师投票记录 </summary>
        public OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>> SelectShopEmployeeVoteRecord(Guid userId,DateTime startDate,DateTime endDate) => Invoke(_ => _.SelectShopEmployeeVoteRecord(userId,startDate,endDate));

	/// <summary> 获取某个用户时间段内的技师投票记录 </summary>
        public Task<OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>>> SelectShopEmployeeVoteRecordAsync(Guid userId,DateTime startDate,DateTime endDate) => InvokeAsync(_ => _.SelectShopEmployeeVoteRecordAsync(userId,startDate,endDate));
		/// <summary> 分享技师投票信息 </summary>
        public OperationResult<bool> AddShareShopEmployeeVote(Guid userId,long shopId,long employeeId) => Invoke(_ => _.AddShareShopEmployeeVote(userId,shopId,employeeId));

	/// <summary> 分享技师投票信息 </summary>
        public Task<OperationResult<bool>> AddShareShopEmployeeVoteAsync(Guid userId,long shopId,long employeeId) => InvokeAsync(_ => _.AddShareShopEmployeeVoteAsync(userId,shopId,employeeId));
		/// <summary> 获取门店报名城市二级联动 </summary>
        public OperationResult<IDictionary<int,Shop.Models.RegionModel>> GetShopRegion() => Invoke(_ => _.GetShopRegion());

	/// <summary> 获取门店报名城市二级联动 </summary>
        public Task<OperationResult<IDictionary<int,Shop.Models.RegionModel>>> GetShopRegionAsync() => InvokeAsync(_ => _.GetShopRegionAsync());
		/// <summary> 获取技师报名城市二级联动 </summary>
        public OperationResult<IDictionary<int,Shop.Models.RegionModel>> GetShopEmployeeRegion() => Invoke(_ => _.GetShopEmployeeRegion());

	/// <summary> 获取技师报名城市二级联动 </summary>
        public Task<OperationResult<IDictionary<int,Shop.Models.RegionModel>>> GetShopEmployeeRegionAsync() => InvokeAsync(_ => _.GetShopEmployeeRegionAsync());
	}
	///<summary>2017年双11分品类品牌销量排名 2017</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICategoryBrandRankService
    {
    	/// <summary> 获取某天的所有分类品牌销量排名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CategoryBrandRank/SelectAllCategoryBrandByDate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CategoryBrandRank/SelectAllCategoryBrandByDateResponse")]
        Task<OperationResult<IEnumerable<CategoryBrandRankModel>>> SelectAllCategoryBrandByDateAsync(DateTime date);
	}

	///<summary>2017年双11分品类品牌销量排名 2017</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICategoryBrandRankClient : ICategoryBrandRankService, ITuhuServiceClient
    {
    	/// <summary> 获取某天的所有分类品牌销量排名 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CategoryBrandRank/SelectAllCategoryBrandByDate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CategoryBrandRank/SelectAllCategoryBrandByDateResponse")]
        OperationResult<IEnumerable<CategoryBrandRankModel>> SelectAllCategoryBrandByDate(DateTime date);
	}

	///<summary>2017年双11分品类品牌销量排名 2017</summary>///
	public partial class CategoryBrandRankClient : TuhuServiceClient<ICategoryBrandRankClient>, ICategoryBrandRankClient
    {
    	/// <summary> 获取某天的所有分类品牌销量排名 </summary>
        public OperationResult<IEnumerable<CategoryBrandRankModel>> SelectAllCategoryBrandByDate(DateTime date) => Invoke(_ => _.SelectAllCategoryBrandByDate(date));

	/// <summary> 获取某天的所有分类品牌销量排名 </summary>
        public Task<OperationResult<IEnumerable<CategoryBrandRankModel>>> SelectAllCategoryBrandByDateAsync(DateTime date) => InvokeAsync(_ => _.SelectAllCategoryBrandByDateAsync(date));
	}
	/// <summary>问卷服务</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IQuestionnaireService
    {
    	///<summary>获取用户的问卷链接信息</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireURL", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireURLResponse")]
        Task<OperationResult<string>> GetQuestionnaireURLAsync(GetQuestionnaireURLRequest model);
		///<summary>获取问卷信息</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireInfoResponse")]
        Task<OperationResult<GetQuestionnaireInfoResponse>> GetQuestionnaireInfoAsync(Guid pageId);
		///<summary>提交问卷</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/SubmitQuestionnaire", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/SubmitQuestionnaireResponse")]
        Task<OperationResult<bool>> SubmitQuestionnaireAsync(SubmitQuestionnaireRequest model);
	}

	/// <summary>问卷服务</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IQuestionnaireClient : IQuestionnaireService, ITuhuServiceClient
    {
    	///<summary>获取用户的问卷链接信息</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireURL", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireURLResponse")]
        OperationResult<string> GetQuestionnaireURL(GetQuestionnaireURLRequest model);
		///<summary>获取问卷信息</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireInfoResponse")]
        OperationResult<GetQuestionnaireInfoResponse> GetQuestionnaireInfo(Guid pageId);
		///<summary>提交问卷</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/SubmitQuestionnaire", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/SubmitQuestionnaireResponse")]
        OperationResult<bool> SubmitQuestionnaire(SubmitQuestionnaireRequest model);
	}

	/// <summary>问卷服务</summary>
	public partial class QuestionnaireClient : TuhuServiceClient<IQuestionnaireClient>, IQuestionnaireClient
    {
    	///<summary>获取用户的问卷链接信息</summary>/// <returns> </returns>
        public OperationResult<string> GetQuestionnaireURL(GetQuestionnaireURLRequest model) => Invoke(_ => _.GetQuestionnaireURL(model));

	///<summary>获取用户的问卷链接信息</summary>/// <returns> </returns>
        public Task<OperationResult<string>> GetQuestionnaireURLAsync(GetQuestionnaireURLRequest model) => InvokeAsync(_ => _.GetQuestionnaireURLAsync(model));
		///<summary>获取问卷信息</summary>/// <returns> </returns>
        public OperationResult<GetQuestionnaireInfoResponse> GetQuestionnaireInfo(Guid pageId) => Invoke(_ => _.GetQuestionnaireInfo(pageId));

	///<summary>获取问卷信息</summary>/// <returns> </returns>
        public Task<OperationResult<GetQuestionnaireInfoResponse>> GetQuestionnaireInfoAsync(Guid pageId) => InvokeAsync(_ => _.GetQuestionnaireInfoAsync(pageId));
		///<summary>提交问卷</summary>/// <returns> </returns>
        public OperationResult<bool> SubmitQuestionnaire(SubmitQuestionnaireRequest model) => Invoke(_ => _.SubmitQuestionnaire(model));

	///<summary>提交问卷</summary>/// <returns> </returns>
        public Task<OperationResult<bool>> SubmitQuestionnaireAsync(SubmitQuestionnaireRequest model) => InvokeAsync(_ => _.SubmitQuestionnaireAsync(model));
	}
	///<summary>促销活动</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISalePromotionActivityService
    {
    	/// <summary> 新增促销活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityResponse")]
        Task<OperationResult<bool>> InsertActivityAsync(SalePromotionActivityModel model);
		/// <summary> 修改促销活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityResponse")]
        Task<OperationResult<bool>> UpdateActivityAsync(SalePromotionActivityModel model);
		/// <summary> 审核后修改促销活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityAfterAudit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityAfterAuditResponse")]
        Task<OperationResult<bool>> UpdateActivityAfterAuditAsync(SalePromotionActivityModel model);
		/// <summary> 下架促销活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UnShelveActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UnShelveActivityResponse")]
        Task<OperationResult<bool>> UnShelveActivityAsync(string activityId, string userName);
		/// <summary> 批量新增促销活动商品 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityProductListResponse")]
        Task<OperationResult<bool>> InsertActivityProductListAsync(List<SalePromotionActivityProduct> productList,string activityId,string userName);
		/// <summary> 按条件查询促销活动列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectActivityList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectActivityListResponse")]
        Task<OperationResult<SelectActivityListModel>> SelectActivityListAsync(SalePromotionActivityModel model, int pageIndex, int pageSize);
		/// <summary> 获取活动折扣内容列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityContent", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityContentResponse")]
        Task<OperationResult<List<SalePromotionActivityDiscount>>> GetActivityContentAsync(string activityId);
		/// <summary> 获取活动信息和折扣内容列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityInfoResponse")]
        Task<OperationResult<SalePromotionActivityModel>> GetActivityInfoAsync(string activityId);
		/// <summary> 根据活动id更新活动商品限购库存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductLimitStock", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductLimitStockResponse")]
        Task<OperationResult<int>> SetProductLimitStockAsync(string activityId, List<string> pidList, int stock, string userName);
		/// <summary>删除活动的商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteProductFromActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteProductFromActivityResponse")]
        Task<OperationResult<int>> DeleteProductFromActivityAsync(string pid, string activityId,string userName);
		/// <summary>获取活动的商品限购数</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetProductInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetProductInfoListResponse")]
        Task<OperationResult<IEnumerable<SalePromotionActivityProduct>>> GetProductInfoListAsync(string activityId, List<string> pidList);
		/// <summary>分页查询活动的商品列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectProductListResponse")]
        Task<OperationResult<PagedModel<SalePromotionActivityProduct>>> SelectProductListAsync(SelectActivityProduct condition, int pageIndex, int pageSize);
		/// <summary>获取已存在的商品列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetRepeatProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetRepeatProductListResponse")]
        Task<OperationResult<IList<SalePromotionActivityProduct>>> GetRepeatProductListAsync(string activityId,List<string> pidList);
		/// <summary>获取特定时间内当前活动和其他活动重复的商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityRepeatProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityRepeatProductListResponse")]
        Task<OperationResult<IList<SalePromotionActivityProduct>>> GetActivityRepeatProductListAsync(string activityId, string startTime, string endTime);
		/// <summary>添加和删除活动商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/AddAndDelActivityProduct", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/AddAndDelActivityProductResponse")]
        Task<OperationResult<bool>> AddAndDelActivityProductAsync(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName);
		/// <summary>同步商品信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/RefreshProduct", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/RefreshProductResponse")]
        Task<OperationResult<bool>> RefreshProductAsync(string activityId, List<SalePromotionActivityProduct> productList);
		/// <summary>设置活动审核状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetActivityAuditStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetActivityAuditStatusResponse")]
        Task<OperationResult<bool>> SetActivityAuditStatusAsync(string activityId, string auditUserName, int auditStatus, string remark);
		/// <summary>获取活动审核状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAuditStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAuditStatusResponse")]
        Task<OperationResult<int>> GetActivityAuditStatusAsync(string activityId);
		/// <summary>设置打折商品列表牛皮癣</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductImage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductImageResponse")]
        Task<OperationResult<int>> SetProductImageAsync(string activityId, List<string> pidList, string imgUrl, string userName);
		/// <summary>设置打折商品详情页牛皮癣</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetDiscountProductDetailImg", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetDiscountProductDetailImgResponse")]
        Task<OperationResult<SetDiscountProductDetailImgResponse>> SetDiscountProductDetailImgAsync(SetDiscountProductDetailImgRequest request);
		/// <summary>获取活动信息和商品信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAndProducts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAndProductsResponse")]
        Task<OperationResult<SalePromotionActivityModel>> GetActivityAndProductsAsync(string activityId, List<string> pidList);
		/// <summary>新增促销活动审核权限</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertAuditAuthResponse")]
        Task<OperationResult<int>> InsertAuditAuthAsync(SalePromotionAuditAuth model);
		/// <summary>删除促销活动审核权限</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteAuditAuthResponse")]
        Task<OperationResult<int>> DeleteAuditAuthAsync(int PKID);
		/// <summary>获取促销活动审核权限列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectAuditAuthList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectAuditAuthListResponse")]
        Task<OperationResult<PagedModel<SalePromotionAuditAuth>>> SelectAuditAuthListAsync(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize);
		/// <summary>验证用户是否有该促销类型的审核权限</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetUserAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetUserAuditAuthResponse")]
        Task<OperationResult<SalePromotionAuditAuth>> GetUserAuditAuthAsync(int promotionType,string userName);
	}

	///<summary>促销活动</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISalePromotionActivityClient : ISalePromotionActivityService, ITuhuServiceClient
    {
    	/// <summary> 新增促销活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityResponse")]
        OperationResult<bool> InsertActivity(SalePromotionActivityModel model);
		/// <summary> 修改促销活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityResponse")]
        OperationResult<bool> UpdateActivity(SalePromotionActivityModel model);
		/// <summary> 审核后修改促销活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityAfterAudit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityAfterAuditResponse")]
        OperationResult<bool> UpdateActivityAfterAudit(SalePromotionActivityModel model);
		/// <summary> 下架促销活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UnShelveActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UnShelveActivityResponse")]
        OperationResult<bool> UnShelveActivity(string activityId, string userName);
		/// <summary> 批量新增促销活动商品 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityProductListResponse")]
        OperationResult<bool> InsertActivityProductList(List<SalePromotionActivityProduct> productList,string activityId,string userName);
		/// <summary> 按条件查询促销活动列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectActivityList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectActivityListResponse")]
        OperationResult<SelectActivityListModel> SelectActivityList(SalePromotionActivityModel model, int pageIndex, int pageSize);
		/// <summary> 获取活动折扣内容列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityContent", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityContentResponse")]
        OperationResult<List<SalePromotionActivityDiscount>> GetActivityContent(string activityId);
		/// <summary> 获取活动信息和折扣内容列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityInfoResponse")]
        OperationResult<SalePromotionActivityModel> GetActivityInfo(string activityId);
		/// <summary> 根据活动id更新活动商品限购库存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductLimitStock", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductLimitStockResponse")]
        OperationResult<int> SetProductLimitStock(string activityId, List<string> pidList, int stock, string userName);
		/// <summary>删除活动的商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteProductFromActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteProductFromActivityResponse")]
        OperationResult<int> DeleteProductFromActivity(string pid, string activityId,string userName);
		/// <summary>获取活动的商品限购数</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetProductInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetProductInfoListResponse")]
        OperationResult<IEnumerable<SalePromotionActivityProduct>> GetProductInfoList(string activityId, List<string> pidList);
		/// <summary>分页查询活动的商品列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectProductListResponse")]
        OperationResult<PagedModel<SalePromotionActivityProduct>> SelectProductList(SelectActivityProduct condition, int pageIndex, int pageSize);
		/// <summary>获取已存在的商品列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetRepeatProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetRepeatProductListResponse")]
        OperationResult<IList<SalePromotionActivityProduct>> GetRepeatProductList(string activityId,List<string> pidList);
		/// <summary>获取特定时间内当前活动和其他活动重复的商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityRepeatProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityRepeatProductListResponse")]
        OperationResult<IList<SalePromotionActivityProduct>> GetActivityRepeatProductList(string activityId, string startTime, string endTime);
		/// <summary>添加和删除活动商品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/AddAndDelActivityProduct", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/AddAndDelActivityProductResponse")]
        OperationResult<bool> AddAndDelActivityProduct(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName);
		/// <summary>同步商品信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/RefreshProduct", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/RefreshProductResponse")]
        OperationResult<bool> RefreshProduct(string activityId, List<SalePromotionActivityProduct> productList);
		/// <summary>设置活动审核状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetActivityAuditStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetActivityAuditStatusResponse")]
        OperationResult<bool> SetActivityAuditStatus(string activityId, string auditUserName, int auditStatus, string remark);
		/// <summary>获取活动审核状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAuditStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAuditStatusResponse")]
        OperationResult<int> GetActivityAuditStatus(string activityId);
		/// <summary>设置打折商品列表牛皮癣</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductImage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductImageResponse")]
        OperationResult<int> SetProductImage(string activityId, List<string> pidList, string imgUrl, string userName);
		/// <summary>设置打折商品详情页牛皮癣</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetDiscountProductDetailImg", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetDiscountProductDetailImgResponse")]
        OperationResult<SetDiscountProductDetailImgResponse> SetDiscountProductDetailImg(SetDiscountProductDetailImgRequest request);
		/// <summary>获取活动信息和商品信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAndProducts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAndProductsResponse")]
        OperationResult<SalePromotionActivityModel> GetActivityAndProducts(string activityId, List<string> pidList);
		/// <summary>新增促销活动审核权限</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertAuditAuthResponse")]
        OperationResult<int> InsertAuditAuth(SalePromotionAuditAuth model);
		/// <summary>删除促销活动审核权限</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteAuditAuthResponse")]
        OperationResult<int> DeleteAuditAuth(int PKID);
		/// <summary>获取促销活动审核权限列表</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectAuditAuthList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectAuditAuthListResponse")]
        OperationResult<PagedModel<SalePromotionAuditAuth>> SelectAuditAuthList(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize);
		/// <summary>验证用户是否有该促销类型的审核权限</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetUserAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetUserAuditAuthResponse")]
        OperationResult<SalePromotionAuditAuth> GetUserAuditAuth(int promotionType,string userName);
	}

	///<summary>促销活动</summary>///
	public partial class SalePromotionActivityClient : TuhuServiceClient<ISalePromotionActivityClient>, ISalePromotionActivityClient
    {
    	/// <summary> 新增促销活动 </summary>
        public OperationResult<bool> InsertActivity(SalePromotionActivityModel model) => Invoke(_ => _.InsertActivity(model));

	/// <summary> 新增促销活动 </summary>
        public Task<OperationResult<bool>> InsertActivityAsync(SalePromotionActivityModel model) => InvokeAsync(_ => _.InsertActivityAsync(model));
		/// <summary> 修改促销活动 </summary>
        public OperationResult<bool> UpdateActivity(SalePromotionActivityModel model) => Invoke(_ => _.UpdateActivity(model));

	/// <summary> 修改促销活动 </summary>
        public Task<OperationResult<bool>> UpdateActivityAsync(SalePromotionActivityModel model) => InvokeAsync(_ => _.UpdateActivityAsync(model));
		/// <summary> 审核后修改促销活动 </summary>
        public OperationResult<bool> UpdateActivityAfterAudit(SalePromotionActivityModel model) => Invoke(_ => _.UpdateActivityAfterAudit(model));

	/// <summary> 审核后修改促销活动 </summary>
        public Task<OperationResult<bool>> UpdateActivityAfterAuditAsync(SalePromotionActivityModel model) => InvokeAsync(_ => _.UpdateActivityAfterAuditAsync(model));
		/// <summary> 下架促销活动 </summary>
        public OperationResult<bool> UnShelveActivity(string activityId, string userName) => Invoke(_ => _.UnShelveActivity(activityId,userName));

	/// <summary> 下架促销活动 </summary>
        public Task<OperationResult<bool>> UnShelveActivityAsync(string activityId, string userName) => InvokeAsync(_ => _.UnShelveActivityAsync(activityId,userName));
		/// <summary> 批量新增促销活动商品 </summary>
        public OperationResult<bool> InsertActivityProductList(List<SalePromotionActivityProduct> productList,string activityId,string userName) => Invoke(_ => _.InsertActivityProductList(productList,activityId,userName));

	/// <summary> 批量新增促销活动商品 </summary>
        public Task<OperationResult<bool>> InsertActivityProductListAsync(List<SalePromotionActivityProduct> productList,string activityId,string userName) => InvokeAsync(_ => _.InsertActivityProductListAsync(productList,activityId,userName));
		/// <summary> 按条件查询促销活动列表 </summary>
        public OperationResult<SelectActivityListModel> SelectActivityList(SalePromotionActivityModel model, int pageIndex, int pageSize) => Invoke(_ => _.SelectActivityList(model,pageIndex,pageSize));

	/// <summary> 按条件查询促销活动列表 </summary>
        public Task<OperationResult<SelectActivityListModel>> SelectActivityListAsync(SalePromotionActivityModel model, int pageIndex, int pageSize) => InvokeAsync(_ => _.SelectActivityListAsync(model,pageIndex,pageSize));
		/// <summary> 获取活动折扣内容列表 </summary>
        public OperationResult<List<SalePromotionActivityDiscount>> GetActivityContent(string activityId) => Invoke(_ => _.GetActivityContent(activityId));

	/// <summary> 获取活动折扣内容列表 </summary>
        public Task<OperationResult<List<SalePromotionActivityDiscount>>> GetActivityContentAsync(string activityId) => InvokeAsync(_ => _.GetActivityContentAsync(activityId));
		/// <summary> 获取活动信息和折扣内容列表 </summary>
        public OperationResult<SalePromotionActivityModel> GetActivityInfo(string activityId) => Invoke(_ => _.GetActivityInfo(activityId));

	/// <summary> 获取活动信息和折扣内容列表 </summary>
        public Task<OperationResult<SalePromotionActivityModel>> GetActivityInfoAsync(string activityId) => InvokeAsync(_ => _.GetActivityInfoAsync(activityId));
		/// <summary> 根据活动id更新活动商品限购库存 </summary>
        public OperationResult<int> SetProductLimitStock(string activityId, List<string> pidList, int stock, string userName) => Invoke(_ => _.SetProductLimitStock(activityId,pidList,stock,userName));

	/// <summary> 根据活动id更新活动商品限购库存 </summary>
        public Task<OperationResult<int>> SetProductLimitStockAsync(string activityId, List<string> pidList, int stock, string userName) => InvokeAsync(_ => _.SetProductLimitStockAsync(activityId,pidList,stock,userName));
		/// <summary>删除活动的商品</summary>
        public OperationResult<int> DeleteProductFromActivity(string pid, string activityId,string userName) => Invoke(_ => _.DeleteProductFromActivity(pid,activityId,userName));

	/// <summary>删除活动的商品</summary>
        public Task<OperationResult<int>> DeleteProductFromActivityAsync(string pid, string activityId,string userName) => InvokeAsync(_ => _.DeleteProductFromActivityAsync(pid,activityId,userName));
		/// <summary>获取活动的商品限购数</summary>
        public OperationResult<IEnumerable<SalePromotionActivityProduct>> GetProductInfoList(string activityId, List<string> pidList) => Invoke(_ => _.GetProductInfoList(activityId,pidList));

	/// <summary>获取活动的商品限购数</summary>
        public Task<OperationResult<IEnumerable<SalePromotionActivityProduct>>> GetProductInfoListAsync(string activityId, List<string> pidList) => InvokeAsync(_ => _.GetProductInfoListAsync(activityId,pidList));
		/// <summary>分页查询活动的商品列表</summary>
        public OperationResult<PagedModel<SalePromotionActivityProduct>> SelectProductList(SelectActivityProduct condition, int pageIndex, int pageSize) => Invoke(_ => _.SelectProductList(condition,pageIndex,pageSize));

	/// <summary>分页查询活动的商品列表</summary>
        public Task<OperationResult<PagedModel<SalePromotionActivityProduct>>> SelectProductListAsync(SelectActivityProduct condition, int pageIndex, int pageSize) => InvokeAsync(_ => _.SelectProductListAsync(condition,pageIndex,pageSize));
		/// <summary>获取已存在的商品列表</summary>
        public OperationResult<IList<SalePromotionActivityProduct>> GetRepeatProductList(string activityId,List<string> pidList) => Invoke(_ => _.GetRepeatProductList(activityId,pidList));

	/// <summary>获取已存在的商品列表</summary>
        public Task<OperationResult<IList<SalePromotionActivityProduct>>> GetRepeatProductListAsync(string activityId,List<string> pidList) => InvokeAsync(_ => _.GetRepeatProductListAsync(activityId,pidList));
		/// <summary>获取特定时间内当前活动和其他活动重复的商品</summary>
        public OperationResult<IList<SalePromotionActivityProduct>> GetActivityRepeatProductList(string activityId, string startTime, string endTime) => Invoke(_ => _.GetActivityRepeatProductList(activityId,startTime,endTime));

	/// <summary>获取特定时间内当前活动和其他活动重复的商品</summary>
        public Task<OperationResult<IList<SalePromotionActivityProduct>>> GetActivityRepeatProductListAsync(string activityId, string startTime, string endTime) => InvokeAsync(_ => _.GetActivityRepeatProductListAsync(activityId,startTime,endTime));
		/// <summary>添加和删除活动商品</summary>
        public OperationResult<bool> AddAndDelActivityProduct(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName) => Invoke(_ => _.AddAndDelActivityProduct(activityId,stock,addList,delList,userName));

	/// <summary>添加和删除活动商品</summary>
        public Task<OperationResult<bool>> AddAndDelActivityProductAsync(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName) => InvokeAsync(_ => _.AddAndDelActivityProductAsync(activityId,stock,addList,delList,userName));
		/// <summary>同步商品信息</summary>
        public OperationResult<bool> RefreshProduct(string activityId, List<SalePromotionActivityProduct> productList) => Invoke(_ => _.RefreshProduct(activityId,productList));

	/// <summary>同步商品信息</summary>
        public Task<OperationResult<bool>> RefreshProductAsync(string activityId, List<SalePromotionActivityProduct> productList) => InvokeAsync(_ => _.RefreshProductAsync(activityId,productList));
		/// <summary>设置活动审核状态</summary>
        public OperationResult<bool> SetActivityAuditStatus(string activityId, string auditUserName, int auditStatus, string remark) => Invoke(_ => _.SetActivityAuditStatus(activityId,auditUserName,auditStatus,remark));

	/// <summary>设置活动审核状态</summary>
        public Task<OperationResult<bool>> SetActivityAuditStatusAsync(string activityId, string auditUserName, int auditStatus, string remark) => InvokeAsync(_ => _.SetActivityAuditStatusAsync(activityId,auditUserName,auditStatus,remark));
		/// <summary>获取活动审核状态</summary>
        public OperationResult<int> GetActivityAuditStatus(string activityId) => Invoke(_ => _.GetActivityAuditStatus(activityId));

	/// <summary>获取活动审核状态</summary>
        public Task<OperationResult<int>> GetActivityAuditStatusAsync(string activityId) => InvokeAsync(_ => _.GetActivityAuditStatusAsync(activityId));
		/// <summary>设置打折商品列表牛皮癣</summary>
        public OperationResult<int> SetProductImage(string activityId, List<string> pidList, string imgUrl, string userName) => Invoke(_ => _.SetProductImage(activityId,pidList,imgUrl,userName));

	/// <summary>设置打折商品列表牛皮癣</summary>
        public Task<OperationResult<int>> SetProductImageAsync(string activityId, List<string> pidList, string imgUrl, string userName) => InvokeAsync(_ => _.SetProductImageAsync(activityId,pidList,imgUrl,userName));
		/// <summary>设置打折商品详情页牛皮癣</summary>
        public OperationResult<SetDiscountProductDetailImgResponse> SetDiscountProductDetailImg(SetDiscountProductDetailImgRequest request) => Invoke(_ => _.SetDiscountProductDetailImg(request));

	/// <summary>设置打折商品详情页牛皮癣</summary>
        public Task<OperationResult<SetDiscountProductDetailImgResponse>> SetDiscountProductDetailImgAsync(SetDiscountProductDetailImgRequest request) => InvokeAsync(_ => _.SetDiscountProductDetailImgAsync(request));
		/// <summary>获取活动信息和商品信息</summary>
        public OperationResult<SalePromotionActivityModel> GetActivityAndProducts(string activityId, List<string> pidList) => Invoke(_ => _.GetActivityAndProducts(activityId,pidList));

	/// <summary>获取活动信息和商品信息</summary>
        public Task<OperationResult<SalePromotionActivityModel>> GetActivityAndProductsAsync(string activityId, List<string> pidList) => InvokeAsync(_ => _.GetActivityAndProductsAsync(activityId,pidList));
		/// <summary>新增促销活动审核权限</summary>
        public OperationResult<int> InsertAuditAuth(SalePromotionAuditAuth model) => Invoke(_ => _.InsertAuditAuth(model));

	/// <summary>新增促销活动审核权限</summary>
        public Task<OperationResult<int>> InsertAuditAuthAsync(SalePromotionAuditAuth model) => InvokeAsync(_ => _.InsertAuditAuthAsync(model));
		/// <summary>删除促销活动审核权限</summary>
        public OperationResult<int> DeleteAuditAuth(int PKID) => Invoke(_ => _.DeleteAuditAuth(PKID));

	/// <summary>删除促销活动审核权限</summary>
        public Task<OperationResult<int>> DeleteAuditAuthAsync(int PKID) => InvokeAsync(_ => _.DeleteAuditAuthAsync(PKID));
		/// <summary>获取促销活动审核权限列表</summary>
        public OperationResult<PagedModel<SalePromotionAuditAuth>> SelectAuditAuthList(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize) => Invoke(_ => _.SelectAuditAuthList(searchModel,pageIndex,pageSize));

	/// <summary>获取促销活动审核权限列表</summary>
        public Task<OperationResult<PagedModel<SalePromotionAuditAuth>>> SelectAuditAuthListAsync(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize) => InvokeAsync(_ => _.SelectAuditAuthListAsync(searchModel,pageIndex,pageSize));
		/// <summary>验证用户是否有该促销类型的审核权限</summary>
        public OperationResult<SalePromotionAuditAuth> GetUserAuditAuth(int promotionType,string userName) => Invoke(_ => _.GetUserAuditAuth(promotionType,userName));

	/// <summary>验证用户是否有该促销类型的审核权限</summary>
        public Task<OperationResult<SalePromotionAuditAuth>> GetUserAuditAuthAsync(int promotionType,string userName) => InvokeAsync(_ => _.GetUserAuditAuthAsync(promotionType,userName));
	}
	///<summary>促销活动日志</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISalePromotionActivityLogService
    {
    	/// <summary> 获取日志列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogListResponse")]
        Task<OperationResult<PagedModel<SalePromotionActivityLogModel>>> GetOperationLogListAsync(string referId, int pageIndex, int pageSize);
		/// <summary> 获取日志详情列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogDetailList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogDetailListResponse")]
        Task<OperationResult<IEnumerable<SalePromotionActivityLogDetail>>> GetOperationLogDetailListAsync(string operationId);
		/// <summary> 新增日志描述 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertActivityLogDescription", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertActivityLogDescriptionResponse")]
        Task<OperationResult<bool>> InsertActivityLogDescriptionAsync(SalePromotionActivityLogDescription model);
		/// <summary> 新增日志和日志详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertAcitivityLogAndDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertAcitivityLogAndDetailResponse")]
        Task<OperationResult<bool>> InsertAcitivityLogAndDetailAsync(SalePromotionActivityLogModel model);
	}

	///<summary>促销活动日志</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISalePromotionActivityLogClient : ISalePromotionActivityLogService, ITuhuServiceClient
    {
    	/// <summary> 获取日志列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogListResponse")]
        OperationResult<PagedModel<SalePromotionActivityLogModel>> GetOperationLogList(string referId, int pageIndex, int pageSize);
		/// <summary> 获取日志详情列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogDetailList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogDetailListResponse")]
        OperationResult<IEnumerable<SalePromotionActivityLogDetail>> GetOperationLogDetailList(string operationId);
		/// <summary> 新增日志描述 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertActivityLogDescription", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertActivityLogDescriptionResponse")]
        OperationResult<bool> InsertActivityLogDescription(SalePromotionActivityLogDescription model);
		/// <summary> 新增日志和日志详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertAcitivityLogAndDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertAcitivityLogAndDetailResponse")]
        OperationResult<bool> InsertAcitivityLogAndDetail(SalePromotionActivityLogModel model);
	}

	///<summary>促销活动日志</summary>///
	public partial class SalePromotionActivityLogClient : TuhuServiceClient<ISalePromotionActivityLogClient>, ISalePromotionActivityLogClient
    {
    	/// <summary> 获取日志列表 </summary>
        public OperationResult<PagedModel<SalePromotionActivityLogModel>> GetOperationLogList(string referId, int pageIndex, int pageSize) => Invoke(_ => _.GetOperationLogList(referId,pageIndex,pageSize));

	/// <summary> 获取日志列表 </summary>
        public Task<OperationResult<PagedModel<SalePromotionActivityLogModel>>> GetOperationLogListAsync(string referId, int pageIndex, int pageSize) => InvokeAsync(_ => _.GetOperationLogListAsync(referId,pageIndex,pageSize));
		/// <summary> 获取日志详情列表 </summary>
        public OperationResult<IEnumerable<SalePromotionActivityLogDetail>> GetOperationLogDetailList(string operationId) => Invoke(_ => _.GetOperationLogDetailList(operationId));

	/// <summary> 获取日志详情列表 </summary>
        public Task<OperationResult<IEnumerable<SalePromotionActivityLogDetail>>> GetOperationLogDetailListAsync(string operationId) => InvokeAsync(_ => _.GetOperationLogDetailListAsync(operationId));
		/// <summary> 新增日志描述 </summary>
        public OperationResult<bool> InsertActivityLogDescription(SalePromotionActivityLogDescription model) => Invoke(_ => _.InsertActivityLogDescription(model));

	/// <summary> 新增日志描述 </summary>
        public Task<OperationResult<bool>> InsertActivityLogDescriptionAsync(SalePromotionActivityLogDescription model) => InvokeAsync(_ => _.InsertActivityLogDescriptionAsync(model));
		/// <summary> 新增日志和日志详情 </summary>
        public OperationResult<bool> InsertAcitivityLogAndDetail(SalePromotionActivityLogModel model) => Invoke(_ => _.InsertAcitivityLogAndDetail(model));

	/// <summary> 新增日志和日志详情 </summary>
        public Task<OperationResult<bool>> InsertAcitivityLogAndDetailAsync(SalePromotionActivityLogModel model) => InvokeAsync(_ => _.InsertAcitivityLogAndDetailAsync(model));
	}
	///<summary>打折活动信息</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IDiscountActivityInfoService
    {
    	/// <summary> 获取当前时间商品参与的打折活动信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductDiscountInfoForTag", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductDiscountInfoForTagResponse")]
        Task<OperationResult<IEnumerable<ProductActivityInfoForTag>>> GetProductDiscountInfoForTagAsync(List<string> pidList,DateTime startTime,DateTime endTime);
		/// <summary> 获取商品的打折活动信息和商品的用户限购数 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserDiscountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserDiscountInfoResponse")]
        Task<OperationResult<IEnumerable<ProductDiscountActivityInfo>>> GetProductAndUserDiscountInfoAsync(List<string> pidList, string userId);
		/// <summary> 根据pid和数量返回打折活动命中信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserHitDiscountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserHitDiscountInfoResponse")]
        Task<OperationResult<IEnumerable<ProductHitDiscountResponse>>> GetProductAndUserHitDiscountInfoAsync(List<DiscountActivityRequest> requestList, string userId);
		/// <summary> 创建订单时记录打折活动信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/SaveCreateOrderDiscountInfoAndSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/SaveCreateOrderDiscountInfoAndSetCacheResponse")]
        Task<OperationResult<bool>> SaveCreateOrderDiscountInfoAndSetCacheAsync(List<DiscountCreateOrderRequest> orderInfoList);
		/// <summary> 取消订单时修改记录的订单折扣信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/UpdateCancelOrderDiscountInfoAndSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/UpdateCancelOrderDiscountInfoAndSetCacheResponse")]
        Task<OperationResult<bool>> UpdateCancelOrderDiscountInfoAndSetCacheAsync(int orderId);
		/// <summary> 批量获取用户活动已购数量缓存数据 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetUserActivityBuyNumCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetUserActivityBuyNumCacheResponse")]
        Task<OperationResult<IEnumerable<UserActivityBuyNumModel>>> GetOrSetUserActivityBuyNumCacheAsync(string userId, List<string> activityIdList);
		/// <summary> 批量获取活动商品已售数量缓存数据 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetActivityProductSoldNumCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetActivityProductSoldNumCacheResponse")]
        Task<OperationResult<IEnumerable<ActivityPidSoldNumModel>>> GetOrSetActivityProductSoldNumCacheAsync(string activityId, List<string> pidList);
	}

	///<summary>打折活动信息</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IDiscountActivityInfoClient : IDiscountActivityInfoService, ITuhuServiceClient
    {
    	/// <summary> 获取当前时间商品参与的打折活动信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductDiscountInfoForTag", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductDiscountInfoForTagResponse")]
        OperationResult<IEnumerable<ProductActivityInfoForTag>> GetProductDiscountInfoForTag(List<string> pidList,DateTime startTime,DateTime endTime);
		/// <summary> 获取商品的打折活动信息和商品的用户限购数 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserDiscountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserDiscountInfoResponse")]
        OperationResult<IEnumerable<ProductDiscountActivityInfo>> GetProductAndUserDiscountInfo(List<string> pidList, string userId);
		/// <summary> 根据pid和数量返回打折活动命中信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserHitDiscountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserHitDiscountInfoResponse")]
        OperationResult<IEnumerable<ProductHitDiscountResponse>> GetProductAndUserHitDiscountInfo(List<DiscountActivityRequest> requestList, string userId);
		/// <summary> 创建订单时记录打折活动信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/SaveCreateOrderDiscountInfoAndSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/SaveCreateOrderDiscountInfoAndSetCacheResponse")]
        OperationResult<bool> SaveCreateOrderDiscountInfoAndSetCache(List<DiscountCreateOrderRequest> orderInfoList);
		/// <summary> 取消订单时修改记录的订单折扣信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/UpdateCancelOrderDiscountInfoAndSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/UpdateCancelOrderDiscountInfoAndSetCacheResponse")]
        OperationResult<bool> UpdateCancelOrderDiscountInfoAndSetCache(int orderId);
		/// <summary> 批量获取用户活动已购数量缓存数据 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetUserActivityBuyNumCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetUserActivityBuyNumCacheResponse")]
        OperationResult<IEnumerable<UserActivityBuyNumModel>> GetOrSetUserActivityBuyNumCache(string userId, List<string> activityIdList);
		/// <summary> 批量获取活动商品已售数量缓存数据 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetActivityProductSoldNumCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetActivityProductSoldNumCacheResponse")]
        OperationResult<IEnumerable<ActivityPidSoldNumModel>> GetOrSetActivityProductSoldNumCache(string activityId, List<string> pidList);
	}

	///<summary>打折活动信息</summary>///
	public partial class DiscountActivityInfoClient : TuhuServiceClient<IDiscountActivityInfoClient>, IDiscountActivityInfoClient
    {
    	/// <summary> 获取当前时间商品参与的打折活动信息 </summary>
        public OperationResult<IEnumerable<ProductActivityInfoForTag>> GetProductDiscountInfoForTag(List<string> pidList,DateTime startTime,DateTime endTime) => Invoke(_ => _.GetProductDiscountInfoForTag(pidList,startTime,endTime));

	/// <summary> 获取当前时间商品参与的打折活动信息 </summary>
        public Task<OperationResult<IEnumerable<ProductActivityInfoForTag>>> GetProductDiscountInfoForTagAsync(List<string> pidList,DateTime startTime,DateTime endTime) => InvokeAsync(_ => _.GetProductDiscountInfoForTagAsync(pidList,startTime,endTime));
		/// <summary> 获取商品的打折活动信息和商品的用户限购数 </summary>
        public OperationResult<IEnumerable<ProductDiscountActivityInfo>> GetProductAndUserDiscountInfo(List<string> pidList, string userId) => Invoke(_ => _.GetProductAndUserDiscountInfo(pidList,userId));

	/// <summary> 获取商品的打折活动信息和商品的用户限购数 </summary>
        public Task<OperationResult<IEnumerable<ProductDiscountActivityInfo>>> GetProductAndUserDiscountInfoAsync(List<string> pidList, string userId) => InvokeAsync(_ => _.GetProductAndUserDiscountInfoAsync(pidList,userId));
		/// <summary> 根据pid和数量返回打折活动命中信息 </summary>
        public OperationResult<IEnumerable<ProductHitDiscountResponse>> GetProductAndUserHitDiscountInfo(List<DiscountActivityRequest> requestList, string userId) => Invoke(_ => _.GetProductAndUserHitDiscountInfo(requestList,userId));

	/// <summary> 根据pid和数量返回打折活动命中信息 </summary>
        public Task<OperationResult<IEnumerable<ProductHitDiscountResponse>>> GetProductAndUserHitDiscountInfoAsync(List<DiscountActivityRequest> requestList, string userId) => InvokeAsync(_ => _.GetProductAndUserHitDiscountInfoAsync(requestList,userId));
		/// <summary> 创建订单时记录打折活动信息 </summary>
        public OperationResult<bool> SaveCreateOrderDiscountInfoAndSetCache(List<DiscountCreateOrderRequest> orderInfoList) => Invoke(_ => _.SaveCreateOrderDiscountInfoAndSetCache(orderInfoList));

	/// <summary> 创建订单时记录打折活动信息 </summary>
        public Task<OperationResult<bool>> SaveCreateOrderDiscountInfoAndSetCacheAsync(List<DiscountCreateOrderRequest> orderInfoList) => InvokeAsync(_ => _.SaveCreateOrderDiscountInfoAndSetCacheAsync(orderInfoList));
		/// <summary> 取消订单时修改记录的订单折扣信息 </summary>
        public OperationResult<bool> UpdateCancelOrderDiscountInfoAndSetCache(int orderId) => Invoke(_ => _.UpdateCancelOrderDiscountInfoAndSetCache(orderId));

	/// <summary> 取消订单时修改记录的订单折扣信息 </summary>
        public Task<OperationResult<bool>> UpdateCancelOrderDiscountInfoAndSetCacheAsync(int orderId) => InvokeAsync(_ => _.UpdateCancelOrderDiscountInfoAndSetCacheAsync(orderId));
		/// <summary> 批量获取用户活动已购数量缓存数据 </summary>
        public OperationResult<IEnumerable<UserActivityBuyNumModel>> GetOrSetUserActivityBuyNumCache(string userId, List<string> activityIdList) => Invoke(_ => _.GetOrSetUserActivityBuyNumCache(userId,activityIdList));

	/// <summary> 批量获取用户活动已购数量缓存数据 </summary>
        public Task<OperationResult<IEnumerable<UserActivityBuyNumModel>>> GetOrSetUserActivityBuyNumCacheAsync(string userId, List<string> activityIdList) => InvokeAsync(_ => _.GetOrSetUserActivityBuyNumCacheAsync(userId,activityIdList));
		/// <summary> 批量获取活动商品已售数量缓存数据 </summary>
        public OperationResult<IEnumerable<ActivityPidSoldNumModel>> GetOrSetActivityProductSoldNumCache(string activityId, List<string> pidList) => Invoke(_ => _.GetOrSetActivityProductSoldNumCache(activityId,pidList));

	/// <summary> 批量获取活动商品已售数量缓存数据 </summary>
        public Task<OperationResult<IEnumerable<ActivityPidSoldNumModel>>> GetOrSetActivityProductSoldNumCacheAsync(string activityId, List<string> pidList) => InvokeAsync(_ => _.GetOrSetActivityProductSoldNumCacheAsync(activityId,pidList));
	}
	///<summary>公众号红包服务</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IOARedEnvelopeService
    {
    	/// <summary> 公众号领红包活动详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoResponse")]
        Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoAsync(int officialAccountType=1 );
		/// <summary> 公众号领红包活动详情 - 无缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoNoCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoNoCacheResponse")]
        Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoNoCacheAsync(int officialAccountType=1 );
		/// <summary> 公众号领红包 - 用户是否可以领取 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserVerify", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserVerifyResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserVerifyAsync(OARedEnvelopeUserVerifyRequest request);
		/// <summary> 公众号领红包 - 用户领取 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceive", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserReceiveAsync(OARedEnvelopeUserReceiveRequest request);
		/// <summary> 公众号领红包 - 用户信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserInfoResponse")]
        Task<OperationResult<OARedEnvelopeUserInfoResponse>> OARedEnvelopeUserInfoAsync(OARedEnvelopeUserInfoRequest request);
		/// <summary> 公众号领红包 - 用户领取回调 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveCallback", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveCallbackResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserReceiveCallbackAsync(OARedEnvelopeUserReceiveCallbackRequest request);
		/// <summary> 公众号领红包 - 红包领取动态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeReceiveUpdatings", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeReceiveUpdatingsResponse")]
        Task<OperationResult<OARedEnvelopeReceiveUpdatingsResponse>> OARedEnvelopeReceiveUpdatingsAsync(OARedEnvelopeReceiveUpdatingsRequest request);
		/// <summary> 公众号领红包 - 刷新缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeRefreshCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeRefreshCacheResponse")]
        Task<OperationResult<bool>> OARedEnvelopeRefreshCacheAsync();
		/// <summary> 公众号领红包 - 红包设置更新 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeSettingUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeSettingUpdateResponse")]
        Task<OperationResult<bool>> OARedEnvelopeSettingUpdateAsync(OARedEnvelopeSettingUpdateRequest request);
		/// <summary> 公众号领红包 - 更新每日统计 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeStatisticsUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeStatisticsUpdateResponse")]
        Task<OperationResult<bool>> OARedEnvelopeStatisticsUpdateAsync(OARedEnvelopeStatisticsUpdateRequest request);
		/// <summary> 公众号领红包 - 删除用户数据 为了测试</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveDeleteResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserReceiveDeleteAsync(Guid userId,int officialAccountType=1);
		/// <summary> 公众号领红包 - 删除用户数据 为了测试</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitDeleteResponse")]
        Task<OperationResult<bool>> OARedEnvelopeDailyDataInitDeleteAsync(DateTime date,int officialAccountType=1);
		/// <summary> 公众号领红包 - 每日数据初始化</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitResponse")]
        Task<OperationResult<bool>> OARedEnvelopeDailyDataInitAsync(OARedEnvelopeDailyDataInitRequest request);
		/// <summary> 公众号领红包 - 分享</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserShareResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserShareAsync(OARedEnvelopeUserShareRequest request);
		/// <summary> 获取生成的全部红包对象 为了测试</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/GetAllOARedEnvelopeDailyData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/GetAllOARedEnvelopeDailyDataResponse")]
        Task<OperationResult<IDictionary<string, OARedEnvelopeObjectResponse>>> GetAllOARedEnvelopeDailyDataAsync(GetAllOARedEnvelopeDailyDataRequest request);
	}

	///<summary>公众号红包服务</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IOARedEnvelopeClient : IOARedEnvelopeService, ITuhuServiceClient
    {
    	/// <summary> 公众号领红包活动详情 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoResponse")]
        OperationResult<OARedEnvelopeActivityInfoResponse> OARedEnvelopeActivityInfo(int officialAccountType=1 );
		/// <summary> 公众号领红包活动详情 - 无缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoNoCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoNoCacheResponse")]
        OperationResult<OARedEnvelopeActivityInfoResponse> OARedEnvelopeActivityInfoNoCache(int officialAccountType=1 );
		/// <summary> 公众号领红包 - 用户是否可以领取 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserVerify", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserVerifyResponse")]
        OperationResult<bool> OARedEnvelopeUserVerify(OARedEnvelopeUserVerifyRequest request);
		/// <summary> 公众号领红包 - 用户领取 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceive", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveResponse")]
        OperationResult<bool> OARedEnvelopeUserReceive(OARedEnvelopeUserReceiveRequest request);
		/// <summary> 公众号领红包 - 用户信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserInfoResponse")]
        OperationResult<OARedEnvelopeUserInfoResponse> OARedEnvelopeUserInfo(OARedEnvelopeUserInfoRequest request);
		/// <summary> 公众号领红包 - 用户领取回调 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveCallback", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveCallbackResponse")]
        OperationResult<bool> OARedEnvelopeUserReceiveCallback(OARedEnvelopeUserReceiveCallbackRequest request);
		/// <summary> 公众号领红包 - 红包领取动态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeReceiveUpdatings", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeReceiveUpdatingsResponse")]
        OperationResult<OARedEnvelopeReceiveUpdatingsResponse> OARedEnvelopeReceiveUpdatings(OARedEnvelopeReceiveUpdatingsRequest request);
		/// <summary> 公众号领红包 - 刷新缓存 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeRefreshCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeRefreshCacheResponse")]
        OperationResult<bool> OARedEnvelopeRefreshCache();
		/// <summary> 公众号领红包 - 红包设置更新 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeSettingUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeSettingUpdateResponse")]
        OperationResult<bool> OARedEnvelopeSettingUpdate(OARedEnvelopeSettingUpdateRequest request);
		/// <summary> 公众号领红包 - 更新每日统计 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeStatisticsUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeStatisticsUpdateResponse")]
        OperationResult<bool> OARedEnvelopeStatisticsUpdate(OARedEnvelopeStatisticsUpdateRequest request);
		/// <summary> 公众号领红包 - 删除用户数据 为了测试</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveDeleteResponse")]
        OperationResult<bool> OARedEnvelopeUserReceiveDelete(Guid userId,int officialAccountType=1);
		/// <summary> 公众号领红包 - 删除用户数据 为了测试</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitDeleteResponse")]
        OperationResult<bool> OARedEnvelopeDailyDataInitDelete(DateTime date,int officialAccountType=1);
		/// <summary> 公众号领红包 - 每日数据初始化</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitResponse")]
        OperationResult<bool> OARedEnvelopeDailyDataInit(OARedEnvelopeDailyDataInitRequest request);
		/// <summary> 公众号领红包 - 分享</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserShareResponse")]
        OperationResult<bool> OARedEnvelopeUserShare(OARedEnvelopeUserShareRequest request);
		/// <summary> 获取生成的全部红包对象 为了测试</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/GetAllOARedEnvelopeDailyData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/GetAllOARedEnvelopeDailyDataResponse")]
        OperationResult<IDictionary<string, OARedEnvelopeObjectResponse>> GetAllOARedEnvelopeDailyData(GetAllOARedEnvelopeDailyDataRequest request);
	}

	///<summary>公众号红包服务</summary>///
	public partial class OARedEnvelopeClient : TuhuServiceClient<IOARedEnvelopeClient>, IOARedEnvelopeClient
    {
    	/// <summary> 公众号领红包活动详情 </summary>
        public OperationResult<OARedEnvelopeActivityInfoResponse> OARedEnvelopeActivityInfo(int officialAccountType=1 ) => Invoke(_ => _.OARedEnvelopeActivityInfo(officialAccountType));

	/// <summary> 公众号领红包活动详情 </summary>
        public Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoAsync(int officialAccountType=1 ) => InvokeAsync(_ => _.OARedEnvelopeActivityInfoAsync(officialAccountType));
		/// <summary> 公众号领红包活动详情 - 无缓存 </summary>
        public OperationResult<OARedEnvelopeActivityInfoResponse> OARedEnvelopeActivityInfoNoCache(int officialAccountType=1 ) => Invoke(_ => _.OARedEnvelopeActivityInfoNoCache(officialAccountType));

	/// <summary> 公众号领红包活动详情 - 无缓存 </summary>
        public Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoNoCacheAsync(int officialAccountType=1 ) => InvokeAsync(_ => _.OARedEnvelopeActivityInfoNoCacheAsync(officialAccountType));
		/// <summary> 公众号领红包 - 用户是否可以领取 </summary>
        public OperationResult<bool> OARedEnvelopeUserVerify(OARedEnvelopeUserVerifyRequest request) => Invoke(_ => _.OARedEnvelopeUserVerify(request));

	/// <summary> 公众号领红包 - 用户是否可以领取 </summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserVerifyAsync(OARedEnvelopeUserVerifyRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserVerifyAsync(request));
		/// <summary> 公众号领红包 - 用户领取 </summary>
        public OperationResult<bool> OARedEnvelopeUserReceive(OARedEnvelopeUserReceiveRequest request) => Invoke(_ => _.OARedEnvelopeUserReceive(request));

	/// <summary> 公众号领红包 - 用户领取 </summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserReceiveAsync(OARedEnvelopeUserReceiveRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserReceiveAsync(request));
		/// <summary> 公众号领红包 - 用户信息 </summary>
        public OperationResult<OARedEnvelopeUserInfoResponse> OARedEnvelopeUserInfo(OARedEnvelopeUserInfoRequest request) => Invoke(_ => _.OARedEnvelopeUserInfo(request));

	/// <summary> 公众号领红包 - 用户信息 </summary>
        public Task<OperationResult<OARedEnvelopeUserInfoResponse>> OARedEnvelopeUserInfoAsync(OARedEnvelopeUserInfoRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserInfoAsync(request));
		/// <summary> 公众号领红包 - 用户领取回调 </summary>
        public OperationResult<bool> OARedEnvelopeUserReceiveCallback(OARedEnvelopeUserReceiveCallbackRequest request) => Invoke(_ => _.OARedEnvelopeUserReceiveCallback(request));

	/// <summary> 公众号领红包 - 用户领取回调 </summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserReceiveCallbackAsync(OARedEnvelopeUserReceiveCallbackRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserReceiveCallbackAsync(request));
		/// <summary> 公众号领红包 - 红包领取动态 </summary>
        public OperationResult<OARedEnvelopeReceiveUpdatingsResponse> OARedEnvelopeReceiveUpdatings(OARedEnvelopeReceiveUpdatingsRequest request) => Invoke(_ => _.OARedEnvelopeReceiveUpdatings(request));

	/// <summary> 公众号领红包 - 红包领取动态 </summary>
        public Task<OperationResult<OARedEnvelopeReceiveUpdatingsResponse>> OARedEnvelopeReceiveUpdatingsAsync(OARedEnvelopeReceiveUpdatingsRequest request) => InvokeAsync(_ => _.OARedEnvelopeReceiveUpdatingsAsync(request));
		/// <summary> 公众号领红包 - 刷新缓存 </summary>
        public OperationResult<bool> OARedEnvelopeRefreshCache() => Invoke(_ => _.OARedEnvelopeRefreshCache());

	/// <summary> 公众号领红包 - 刷新缓存 </summary>
        public Task<OperationResult<bool>> OARedEnvelopeRefreshCacheAsync() => InvokeAsync(_ => _.OARedEnvelopeRefreshCacheAsync());
		/// <summary> 公众号领红包 - 红包设置更新 </summary>
        public OperationResult<bool> OARedEnvelopeSettingUpdate(OARedEnvelopeSettingUpdateRequest request) => Invoke(_ => _.OARedEnvelopeSettingUpdate(request));

	/// <summary> 公众号领红包 - 红包设置更新 </summary>
        public Task<OperationResult<bool>> OARedEnvelopeSettingUpdateAsync(OARedEnvelopeSettingUpdateRequest request) => InvokeAsync(_ => _.OARedEnvelopeSettingUpdateAsync(request));
		/// <summary> 公众号领红包 - 更新每日统计 </summary>
        public OperationResult<bool> OARedEnvelopeStatisticsUpdate(OARedEnvelopeStatisticsUpdateRequest request) => Invoke(_ => _.OARedEnvelopeStatisticsUpdate(request));

	/// <summary> 公众号领红包 - 更新每日统计 </summary>
        public Task<OperationResult<bool>> OARedEnvelopeStatisticsUpdateAsync(OARedEnvelopeStatisticsUpdateRequest request) => InvokeAsync(_ => _.OARedEnvelopeStatisticsUpdateAsync(request));
		/// <summary> 公众号领红包 - 删除用户数据 为了测试</summary>
        public OperationResult<bool> OARedEnvelopeUserReceiveDelete(Guid userId,int officialAccountType=1) => Invoke(_ => _.OARedEnvelopeUserReceiveDelete(userId,officialAccountType));

	/// <summary> 公众号领红包 - 删除用户数据 为了测试</summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserReceiveDeleteAsync(Guid userId,int officialAccountType=1) => InvokeAsync(_ => _.OARedEnvelopeUserReceiveDeleteAsync(userId,officialAccountType));
		/// <summary> 公众号领红包 - 删除用户数据 为了测试</summary>
        public OperationResult<bool> OARedEnvelopeDailyDataInitDelete(DateTime date,int officialAccountType=1) => Invoke(_ => _.OARedEnvelopeDailyDataInitDelete(date,officialAccountType));

	/// <summary> 公众号领红包 - 删除用户数据 为了测试</summary>
        public Task<OperationResult<bool>> OARedEnvelopeDailyDataInitDeleteAsync(DateTime date,int officialAccountType=1) => InvokeAsync(_ => _.OARedEnvelopeDailyDataInitDeleteAsync(date,officialAccountType));
		/// <summary> 公众号领红包 - 每日数据初始化</summary>
        public OperationResult<bool> OARedEnvelopeDailyDataInit(OARedEnvelopeDailyDataInitRequest request) => Invoke(_ => _.OARedEnvelopeDailyDataInit(request));

	/// <summary> 公众号领红包 - 每日数据初始化</summary>
        public Task<OperationResult<bool>> OARedEnvelopeDailyDataInitAsync(OARedEnvelopeDailyDataInitRequest request) => InvokeAsync(_ => _.OARedEnvelopeDailyDataInitAsync(request));
		/// <summary> 公众号领红包 - 分享</summary>
        public OperationResult<bool> OARedEnvelopeUserShare(OARedEnvelopeUserShareRequest request) => Invoke(_ => _.OARedEnvelopeUserShare(request));

	/// <summary> 公众号领红包 - 分享</summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserShareAsync(OARedEnvelopeUserShareRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserShareAsync(request));
		/// <summary> 获取生成的全部红包对象 为了测试</summary>
        public OperationResult<IDictionary<string, OARedEnvelopeObjectResponse>> GetAllOARedEnvelopeDailyData(GetAllOARedEnvelopeDailyDataRequest request) => Invoke(_ => _.GetAllOARedEnvelopeDailyData(request));

	/// <summary> 获取生成的全部红包对象 为了测试</summary>
        public Task<OperationResult<IDictionary<string, OARedEnvelopeObjectResponse>>> GetAllOARedEnvelopeDailyDataAsync(GetAllOARedEnvelopeDailyDataRequest request) => InvokeAsync(_ => _.GetAllOARedEnvelopeDailyDataAsync(request));
	}
	///<summary>七龙珠</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IDragonBallService
    {
    	/// <summary> 用户当前龙珠总数/兑换次数 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserInfoResponse")]
        Task<OperationResult<DragonBallUserInfoResponse>> DragonBallUserInfoAsync(DragonBallUserInfoRequest request);
		/// <summary> 获奖轮播 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallBroadcast", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallBroadcastResponse")]
        Task<OperationResult<DragonBallBroadcastResponse>> DragonBallBroadcastAsync(int count);
		/// <summary> 用户获取的奖励列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserLootList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserLootListResponse")]
        Task<OperationResult<DragonBallUserLootListResponse>> DragonBallUserLootListAsync(DragonBallUserLootListRequest request);
		/// <summary> 用户任务列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionListResponse")]
        Task<OperationResult<DragonBallUserMissionListResponse>> DragonBallUserMissionListAsync(DragonBallUserMissionListRequest request);
		/// <summary> 用户任务领取奖励 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionReward", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionRewardResponse")]
        Task<OperationResult<DragonBallUserMissionRewardResponse>> DragonBallUserMissionRewardAsync(DragonBallUserMissionRewardRequest request);
		/// <summary> 召唤神龙啦 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSummon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSummonResponse")]
        Task<OperationResult<DragonBallSummonResponse>> DragonBallSummonAsync(DragonBallSummonRequest request);
		/// <summary> 获取活动状态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityInfoResponse")]
        Task<OperationResult<ActivityResponse>> DragonBallActivityInfoAsync();
		/// <summary> 获取活动状态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingResponse")]
        Task<OperationResult<DragonBallSettingResponse>> DragonBallSettingAsync();
		/// <summary> 完成用户分享 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserShareResponse")]
        Task<OperationResult<bool>> DragonBallUserShareAsync(DragonBallUserShareRequest request);
		/// <summary> 创建一个用户任务 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallCreateUserMissionDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallCreateUserMissionDetailResponse")]
        Task<OperationResult<bool>> DragonBallCreateUserMissionDetailAsync(DragonBallCreateUserMissionDetailRequest request);
		/// <summary> 用户任务历史 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionHistoryList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionHistoryListResponse")]
        Task<OperationResult<DragonBallUserMissionHistoryListResponse>> DragonBallUserMissionHistoryListAsync(DragonBallUserMissionHistoryListRequest request);
		/// <summary> 用户任务历史初始化 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionInit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionInitResponse")]
        Task<OperationResult<bool>> DragonBallUserMissionInitAsync(Guid userId,bool isForce = false);
		/// <summary> 七龙珠 - 更新活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityUpdateResponse")]
        Task<OperationResult<bool>> DragonBallActivityUpdateAsync(DragonBallActivityUpdateRequest request);
		/// <summary> 七龙珠 - 更新设置 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingUpdateResponse")]
        Task<OperationResult<bool>> DragonBallSettingUpdateAsync(DragonBallSettingUpdateRequest request);
		/// <summary> 七龙珠 - 删除用户数据 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserDataDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserDataDeleteResponse")]
        Task<OperationResult<bool>> DragonBallUserDataDeleteAsync(Guid userId);
		/// <summary> 七龙珠 - 增加修改用户龙珠数量 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserUpdateResponse")]
        Task<OperationResult<bool>> DragonBallUserUpdateAsync(Guid userId,int dragonBallCount);
	}

	///<summary>七龙珠</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IDragonBallClient : IDragonBallService, ITuhuServiceClient
    {
    	/// <summary> 用户当前龙珠总数/兑换次数 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserInfoResponse")]
        OperationResult<DragonBallUserInfoResponse> DragonBallUserInfo(DragonBallUserInfoRequest request);
		/// <summary> 获奖轮播 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallBroadcast", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallBroadcastResponse")]
        OperationResult<DragonBallBroadcastResponse> DragonBallBroadcast(int count);
		/// <summary> 用户获取的奖励列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserLootList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserLootListResponse")]
        OperationResult<DragonBallUserLootListResponse> DragonBallUserLootList(DragonBallUserLootListRequest request);
		/// <summary> 用户任务列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionListResponse")]
        OperationResult<DragonBallUserMissionListResponse> DragonBallUserMissionList(DragonBallUserMissionListRequest request);
		/// <summary> 用户任务领取奖励 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionReward", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionRewardResponse")]
        OperationResult<DragonBallUserMissionRewardResponse> DragonBallUserMissionReward(DragonBallUserMissionRewardRequest request);
		/// <summary> 召唤神龙啦 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSummon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSummonResponse")]
        OperationResult<DragonBallSummonResponse> DragonBallSummon(DragonBallSummonRequest request);
		/// <summary> 获取活动状态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityInfoResponse")]
        OperationResult<ActivityResponse> DragonBallActivityInfo();
		/// <summary> 获取活动状态 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingResponse")]
        OperationResult<DragonBallSettingResponse> DragonBallSetting();
		/// <summary> 完成用户分享 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserShareResponse")]
        OperationResult<bool> DragonBallUserShare(DragonBallUserShareRequest request);
		/// <summary> 创建一个用户任务 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallCreateUserMissionDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallCreateUserMissionDetailResponse")]
        OperationResult<bool> DragonBallCreateUserMissionDetail(DragonBallCreateUserMissionDetailRequest request);
		/// <summary> 用户任务历史 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionHistoryList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionHistoryListResponse")]
        OperationResult<DragonBallUserMissionHistoryListResponse> DragonBallUserMissionHistoryList(DragonBallUserMissionHistoryListRequest request);
		/// <summary> 用户任务历史初始化 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionInit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionInitResponse")]
        OperationResult<bool> DragonBallUserMissionInit(Guid userId,bool isForce = false);
		/// <summary> 七龙珠 - 更新活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityUpdateResponse")]
        OperationResult<bool> DragonBallActivityUpdate(DragonBallActivityUpdateRequest request);
		/// <summary> 七龙珠 - 更新设置 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingUpdateResponse")]
        OperationResult<bool> DragonBallSettingUpdate(DragonBallSettingUpdateRequest request);
		/// <summary> 七龙珠 - 删除用户数据 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserDataDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserDataDeleteResponse")]
        OperationResult<bool> DragonBallUserDataDelete(Guid userId);
		/// <summary> 七龙珠 - 增加修改用户龙珠数量 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserUpdateResponse")]
        OperationResult<bool> DragonBallUserUpdate(Guid userId,int dragonBallCount);
	}

	///<summary>七龙珠</summary>///
	public partial class DragonBallClient : TuhuServiceClient<IDragonBallClient>, IDragonBallClient
    {
    	/// <summary> 用户当前龙珠总数/兑换次数 </summary>
        public OperationResult<DragonBallUserInfoResponse> DragonBallUserInfo(DragonBallUserInfoRequest request) => Invoke(_ => _.DragonBallUserInfo(request));

	/// <summary> 用户当前龙珠总数/兑换次数 </summary>
        public Task<OperationResult<DragonBallUserInfoResponse>> DragonBallUserInfoAsync(DragonBallUserInfoRequest request) => InvokeAsync(_ => _.DragonBallUserInfoAsync(request));
		/// <summary> 获奖轮播 </summary>
        public OperationResult<DragonBallBroadcastResponse> DragonBallBroadcast(int count) => Invoke(_ => _.DragonBallBroadcast(count));

	/// <summary> 获奖轮播 </summary>
        public Task<OperationResult<DragonBallBroadcastResponse>> DragonBallBroadcastAsync(int count) => InvokeAsync(_ => _.DragonBallBroadcastAsync(count));
		/// <summary> 用户获取的奖励列表 </summary>
        public OperationResult<DragonBallUserLootListResponse> DragonBallUserLootList(DragonBallUserLootListRequest request) => Invoke(_ => _.DragonBallUserLootList(request));

	/// <summary> 用户获取的奖励列表 </summary>
        public Task<OperationResult<DragonBallUserLootListResponse>> DragonBallUserLootListAsync(DragonBallUserLootListRequest request) => InvokeAsync(_ => _.DragonBallUserLootListAsync(request));
		/// <summary> 用户任务列表 </summary>
        public OperationResult<DragonBallUserMissionListResponse> DragonBallUserMissionList(DragonBallUserMissionListRequest request) => Invoke(_ => _.DragonBallUserMissionList(request));

	/// <summary> 用户任务列表 </summary>
        public Task<OperationResult<DragonBallUserMissionListResponse>> DragonBallUserMissionListAsync(DragonBallUserMissionListRequest request) => InvokeAsync(_ => _.DragonBallUserMissionListAsync(request));
		/// <summary> 用户任务领取奖励 </summary>
        public OperationResult<DragonBallUserMissionRewardResponse> DragonBallUserMissionReward(DragonBallUserMissionRewardRequest request) => Invoke(_ => _.DragonBallUserMissionReward(request));

	/// <summary> 用户任务领取奖励 </summary>
        public Task<OperationResult<DragonBallUserMissionRewardResponse>> DragonBallUserMissionRewardAsync(DragonBallUserMissionRewardRequest request) => InvokeAsync(_ => _.DragonBallUserMissionRewardAsync(request));
		/// <summary> 召唤神龙啦 </summary>
        public OperationResult<DragonBallSummonResponse> DragonBallSummon(DragonBallSummonRequest request) => Invoke(_ => _.DragonBallSummon(request));

	/// <summary> 召唤神龙啦 </summary>
        public Task<OperationResult<DragonBallSummonResponse>> DragonBallSummonAsync(DragonBallSummonRequest request) => InvokeAsync(_ => _.DragonBallSummonAsync(request));
		/// <summary> 获取活动状态 </summary>
        public OperationResult<ActivityResponse> DragonBallActivityInfo() => Invoke(_ => _.DragonBallActivityInfo());

	/// <summary> 获取活动状态 </summary>
        public Task<OperationResult<ActivityResponse>> DragonBallActivityInfoAsync() => InvokeAsync(_ => _.DragonBallActivityInfoAsync());
		/// <summary> 获取活动状态 </summary>
        public OperationResult<DragonBallSettingResponse> DragonBallSetting() => Invoke(_ => _.DragonBallSetting());

	/// <summary> 获取活动状态 </summary>
        public Task<OperationResult<DragonBallSettingResponse>> DragonBallSettingAsync() => InvokeAsync(_ => _.DragonBallSettingAsync());
		/// <summary> 完成用户分享 </summary>
        public OperationResult<bool> DragonBallUserShare(DragonBallUserShareRequest request) => Invoke(_ => _.DragonBallUserShare(request));

	/// <summary> 完成用户分享 </summary>
        public Task<OperationResult<bool>> DragonBallUserShareAsync(DragonBallUserShareRequest request) => InvokeAsync(_ => _.DragonBallUserShareAsync(request));
		/// <summary> 创建一个用户任务 </summary>
        public OperationResult<bool> DragonBallCreateUserMissionDetail(DragonBallCreateUserMissionDetailRequest request) => Invoke(_ => _.DragonBallCreateUserMissionDetail(request));

	/// <summary> 创建一个用户任务 </summary>
        public Task<OperationResult<bool>> DragonBallCreateUserMissionDetailAsync(DragonBallCreateUserMissionDetailRequest request) => InvokeAsync(_ => _.DragonBallCreateUserMissionDetailAsync(request));
		/// <summary> 用户任务历史 </summary>
        public OperationResult<DragonBallUserMissionHistoryListResponse> DragonBallUserMissionHistoryList(DragonBallUserMissionHistoryListRequest request) => Invoke(_ => _.DragonBallUserMissionHistoryList(request));

	/// <summary> 用户任务历史 </summary>
        public Task<OperationResult<DragonBallUserMissionHistoryListResponse>> DragonBallUserMissionHistoryListAsync(DragonBallUserMissionHistoryListRequest request) => InvokeAsync(_ => _.DragonBallUserMissionHistoryListAsync(request));
		/// <summary> 用户任务历史初始化 </summary>
        public OperationResult<bool> DragonBallUserMissionInit(Guid userId,bool isForce = false) => Invoke(_ => _.DragonBallUserMissionInit(userId,isForce));

	/// <summary> 用户任务历史初始化 </summary>
        public Task<OperationResult<bool>> DragonBallUserMissionInitAsync(Guid userId,bool isForce = false) => InvokeAsync(_ => _.DragonBallUserMissionInitAsync(userId,isForce));
		/// <summary> 七龙珠 - 更新活动 </summary>
        public OperationResult<bool> DragonBallActivityUpdate(DragonBallActivityUpdateRequest request) => Invoke(_ => _.DragonBallActivityUpdate(request));

	/// <summary> 七龙珠 - 更新活动 </summary>
        public Task<OperationResult<bool>> DragonBallActivityUpdateAsync(DragonBallActivityUpdateRequest request) => InvokeAsync(_ => _.DragonBallActivityUpdateAsync(request));
		/// <summary> 七龙珠 - 更新设置 </summary>
        public OperationResult<bool> DragonBallSettingUpdate(DragonBallSettingUpdateRequest request) => Invoke(_ => _.DragonBallSettingUpdate(request));

	/// <summary> 七龙珠 - 更新设置 </summary>
        public Task<OperationResult<bool>> DragonBallSettingUpdateAsync(DragonBallSettingUpdateRequest request) => InvokeAsync(_ => _.DragonBallSettingUpdateAsync(request));
		/// <summary> 七龙珠 - 删除用户数据 </summary>
        public OperationResult<bool> DragonBallUserDataDelete(Guid userId) => Invoke(_ => _.DragonBallUserDataDelete(userId));

	/// <summary> 七龙珠 - 删除用户数据 </summary>
        public Task<OperationResult<bool>> DragonBallUserDataDeleteAsync(Guid userId) => InvokeAsync(_ => _.DragonBallUserDataDeleteAsync(userId));
		/// <summary> 七龙珠 - 增加修改用户龙珠数量 </summary>
        public OperationResult<bool> DragonBallUserUpdate(Guid userId,int dragonBallCount) => Invoke(_ => _.DragonBallUserUpdate(userId,dragonBallCount));

	/// <summary> 七龙珠 - 增加修改用户龙珠数量 </summary>
        public Task<OperationResult<bool>> DragonBallUserUpdateAsync(Guid userId,int dragonBallCount) => InvokeAsync(_ => _.DragonBallUserUpdateAsync(userId,dragonBallCount));
	}
	///<summary>游戏</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IGameService
    {
    	/// <summary> 获取 游戏信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameInfoResponse")]
        Task<OperationResult<GetGameInfoResponse>> GetGameInfoAsync(GetGameInfoRequest request);
		/// <summary> 获取 里程碑信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameMilepostInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameMilepostInfoResponse")]
        Task<OperationResult<GetGameMilepostInfoResponse>> GetGameMilepostInfoAsync(GetGameMilepostInfoRequest request);
		/// <summary> 获取 当前用户信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserInfoResponse")]
        Task<OperationResult<GetGameUserInfoResponse>> GetGameUserInfoAsync(GetGameUserInfoRequest request);
		/// <summary> 用户分享 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserShareResponse")]
        Task<OperationResult<GameUserShareResponse>> GameUserShareAsync(GameUserShareRequest request);
		/// <summary> 用户兑换奖品 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserLoot", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserLootResponse")]
        Task<OperationResult<GameUserLootResponse>> GameUserLootAsync(GameUserLootRequest request);
		/// <summary> 获取 用户好友助力信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserFriendSupport", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserFriendSupportResponse")]
        Task<OperationResult<GetGameUserFriendSupportResponse>> GetGameUserFriendSupportAsync(GetGameUserFriendSupportRequest request);
		/// <summary> 帮助助力 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserFriendSupport", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserFriendSupportResponse")]
        Task<OperationResult<GameUserFriendSupportResponse>> GameUserFriendSupportAsync(GameUserFriendSupportRequest request);
		/// <summary> 获取 用户里程收支明细 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserDistanceInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserDistanceInfoResponse")]
        Task<OperationResult<GetGameUserDistanceInfoResponse>> GetGameUserDistanceInfoAsync(GetGameUserDistanceInfoRequest request);
		/// <summary> 获取 奖励滚动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserLootBroadcast", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserLootBroadcastResponse")]
        Task<OperationResult<GetGameUserLootBroadcastResponse>> GetGameUserLootBroadcastAsync(GetGameUserLootBroadcastRequest request);
		/// <summary> 获取 用户助力信息【剩余助力次数】 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserSupportInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserSupportInfoResponse")]
        Task<OperationResult<GetGameUserSupportInfoResponse>> GetGameUserSupportInfoAsync(GetGameUserSupportInfoRequest request);
		/// <summary> 小游戏 - 订单状态跟踪</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameOrderTracking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameOrderTrackingResponse")]
        Task<OperationResult<GameOrderTrackingResponse>> GameOrderTrackingAsync(GameOrderTackingRequest request);
		/// <summary> 小游戏 - 更新游戏信息 - 内部用</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UpdateGameInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UpdateGameInfoResponse")]
        Task<OperationResult<UpdateGameInfoResponse>> UpdateGameInfoAsync(UpdateGameInfoRequest request);
		/// <summary> 小游戏 - 删除游戏的人员数据 - 内部用</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/DeleteGameUserData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/DeleteGameUserDataResponse")]
        Task<OperationResult<bool>> DeleteGameUserDataAsync(DeleteGameUserDataRequest request);
		/// <summary> 获取游戏实时排行榜</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListResponse")]
        Task<OperationResult<GetRankListResponse>> GetRankListAsync(GetRankListAsyncRequest request);
		/// <summary> 用户进入游戏</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UserParticipateGame", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UserParticipateGameResponse")]
        Task<OperationResult<UserParticipateGameResponse>> UserParticipateGameAsync(UserParticipateGameRequest request);
		/// <summary> 获取用户最近获得的奖品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetUserLatestPrize", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetUserLatestPrizeResponse")]
        Task<OperationResult<GetUserLatestPrizeResponse>> GetUserLatestPrizeAsync(GetUserLatestPrizeRequest request);
		/// <summary> 获取某天之前的积分排行</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListBeforeDay", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListBeforeDayResponse")]
        Task<OperationResult<GetRankListBeforeDayResponse>> GetRankListBeforeDayAsync(GetRankListBeforeDayRequest request);
	}

	///<summary>游戏</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IGameClient : IGameService, ITuhuServiceClient
    {
    	/// <summary> 获取 游戏信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameInfoResponse")]
        OperationResult<GetGameInfoResponse> GetGameInfo(GetGameInfoRequest request);
		/// <summary> 获取 里程碑信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameMilepostInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameMilepostInfoResponse")]
        OperationResult<GetGameMilepostInfoResponse> GetGameMilepostInfo(GetGameMilepostInfoRequest request);
		/// <summary> 获取 当前用户信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserInfoResponse")]
        OperationResult<GetGameUserInfoResponse> GetGameUserInfo(GetGameUserInfoRequest request);
		/// <summary> 用户分享 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserShareResponse")]
        OperationResult<GameUserShareResponse> GameUserShare(GameUserShareRequest request);
		/// <summary> 用户兑换奖品 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserLoot", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserLootResponse")]
        OperationResult<GameUserLootResponse> GameUserLoot(GameUserLootRequest request);
		/// <summary> 获取 用户好友助力信息 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserFriendSupport", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserFriendSupportResponse")]
        OperationResult<GetGameUserFriendSupportResponse> GetGameUserFriendSupport(GetGameUserFriendSupportRequest request);
		/// <summary> 帮助助力 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserFriendSupport", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserFriendSupportResponse")]
        OperationResult<GameUserFriendSupportResponse> GameUserFriendSupport(GameUserFriendSupportRequest request);
		/// <summary> 获取 用户里程收支明细 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserDistanceInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserDistanceInfoResponse")]
        OperationResult<GetGameUserDistanceInfoResponse> GetGameUserDistanceInfo(GetGameUserDistanceInfoRequest request);
		/// <summary> 获取 奖励滚动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserLootBroadcast", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserLootBroadcastResponse")]
        OperationResult<GetGameUserLootBroadcastResponse> GetGameUserLootBroadcast(GetGameUserLootBroadcastRequest request);
		/// <summary> 获取 用户助力信息【剩余助力次数】 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserSupportInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserSupportInfoResponse")]
        OperationResult<GetGameUserSupportInfoResponse> GetGameUserSupportInfo(GetGameUserSupportInfoRequest request);
		/// <summary> 小游戏 - 订单状态跟踪</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameOrderTracking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameOrderTrackingResponse")]
        OperationResult<GameOrderTrackingResponse> GameOrderTracking(GameOrderTackingRequest request);
		/// <summary> 小游戏 - 更新游戏信息 - 内部用</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UpdateGameInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UpdateGameInfoResponse")]
        OperationResult<UpdateGameInfoResponse> UpdateGameInfo(UpdateGameInfoRequest request);
		/// <summary> 小游戏 - 删除游戏的人员数据 - 内部用</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/DeleteGameUserData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/DeleteGameUserDataResponse")]
        OperationResult<bool> DeleteGameUserData(DeleteGameUserDataRequest request);
		/// <summary> 获取游戏实时排行榜</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListResponse")]
        OperationResult<GetRankListResponse> GetRankList(GetRankListAsyncRequest request);
		/// <summary> 用户进入游戏</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UserParticipateGame", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UserParticipateGameResponse")]
        OperationResult<UserParticipateGameResponse> UserParticipateGame(UserParticipateGameRequest request);
		/// <summary> 获取用户最近获得的奖品</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetUserLatestPrize", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetUserLatestPrizeResponse")]
        OperationResult<GetUserLatestPrizeResponse> GetUserLatestPrize(GetUserLatestPrizeRequest request);
		/// <summary> 获取某天之前的积分排行</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListBeforeDay", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListBeforeDayResponse")]
        OperationResult<GetRankListBeforeDayResponse> GetRankListBeforeDay(GetRankListBeforeDayRequest request);
	}

	///<summary>游戏</summary>///
	public partial class GameClient : TuhuServiceClient<IGameClient>, IGameClient
    {
    	/// <summary> 获取 游戏信息 </summary>
        public OperationResult<GetGameInfoResponse> GetGameInfo(GetGameInfoRequest request) => Invoke(_ => _.GetGameInfo(request));

	/// <summary> 获取 游戏信息 </summary>
        public Task<OperationResult<GetGameInfoResponse>> GetGameInfoAsync(GetGameInfoRequest request) => InvokeAsync(_ => _.GetGameInfoAsync(request));
		/// <summary> 获取 里程碑信息 </summary>
        public OperationResult<GetGameMilepostInfoResponse> GetGameMilepostInfo(GetGameMilepostInfoRequest request) => Invoke(_ => _.GetGameMilepostInfo(request));

	/// <summary> 获取 里程碑信息 </summary>
        public Task<OperationResult<GetGameMilepostInfoResponse>> GetGameMilepostInfoAsync(GetGameMilepostInfoRequest request) => InvokeAsync(_ => _.GetGameMilepostInfoAsync(request));
		/// <summary> 获取 当前用户信息 </summary>
        public OperationResult<GetGameUserInfoResponse> GetGameUserInfo(GetGameUserInfoRequest request) => Invoke(_ => _.GetGameUserInfo(request));

	/// <summary> 获取 当前用户信息 </summary>
        public Task<OperationResult<GetGameUserInfoResponse>> GetGameUserInfoAsync(GetGameUserInfoRequest request) => InvokeAsync(_ => _.GetGameUserInfoAsync(request));
		/// <summary> 用户分享 </summary>
        public OperationResult<GameUserShareResponse> GameUserShare(GameUserShareRequest request) => Invoke(_ => _.GameUserShare(request));

	/// <summary> 用户分享 </summary>
        public Task<OperationResult<GameUserShareResponse>> GameUserShareAsync(GameUserShareRequest request) => InvokeAsync(_ => _.GameUserShareAsync(request));
		/// <summary> 用户兑换奖品 </summary>
        public OperationResult<GameUserLootResponse> GameUserLoot(GameUserLootRequest request) => Invoke(_ => _.GameUserLoot(request));

	/// <summary> 用户兑换奖品 </summary>
        public Task<OperationResult<GameUserLootResponse>> GameUserLootAsync(GameUserLootRequest request) => InvokeAsync(_ => _.GameUserLootAsync(request));
		/// <summary> 获取 用户好友助力信息 </summary>
        public OperationResult<GetGameUserFriendSupportResponse> GetGameUserFriendSupport(GetGameUserFriendSupportRequest request) => Invoke(_ => _.GetGameUserFriendSupport(request));

	/// <summary> 获取 用户好友助力信息 </summary>
        public Task<OperationResult<GetGameUserFriendSupportResponse>> GetGameUserFriendSupportAsync(GetGameUserFriendSupportRequest request) => InvokeAsync(_ => _.GetGameUserFriendSupportAsync(request));
		/// <summary> 帮助助力 </summary>
        public OperationResult<GameUserFriendSupportResponse> GameUserFriendSupport(GameUserFriendSupportRequest request) => Invoke(_ => _.GameUserFriendSupport(request));

	/// <summary> 帮助助力 </summary>
        public Task<OperationResult<GameUserFriendSupportResponse>> GameUserFriendSupportAsync(GameUserFriendSupportRequest request) => InvokeAsync(_ => _.GameUserFriendSupportAsync(request));
		/// <summary> 获取 用户里程收支明细 </summary>
        public OperationResult<GetGameUserDistanceInfoResponse> GetGameUserDistanceInfo(GetGameUserDistanceInfoRequest request) => Invoke(_ => _.GetGameUserDistanceInfo(request));

	/// <summary> 获取 用户里程收支明细 </summary>
        public Task<OperationResult<GetGameUserDistanceInfoResponse>> GetGameUserDistanceInfoAsync(GetGameUserDistanceInfoRequest request) => InvokeAsync(_ => _.GetGameUserDistanceInfoAsync(request));
		/// <summary> 获取 奖励滚动 </summary>
        public OperationResult<GetGameUserLootBroadcastResponse> GetGameUserLootBroadcast(GetGameUserLootBroadcastRequest request) => Invoke(_ => _.GetGameUserLootBroadcast(request));

	/// <summary> 获取 奖励滚动 </summary>
        public Task<OperationResult<GetGameUserLootBroadcastResponse>> GetGameUserLootBroadcastAsync(GetGameUserLootBroadcastRequest request) => InvokeAsync(_ => _.GetGameUserLootBroadcastAsync(request));
		/// <summary> 获取 用户助力信息【剩余助力次数】 </summary>
        public OperationResult<GetGameUserSupportInfoResponse> GetGameUserSupportInfo(GetGameUserSupportInfoRequest request) => Invoke(_ => _.GetGameUserSupportInfo(request));

	/// <summary> 获取 用户助力信息【剩余助力次数】 </summary>
        public Task<OperationResult<GetGameUserSupportInfoResponse>> GetGameUserSupportInfoAsync(GetGameUserSupportInfoRequest request) => InvokeAsync(_ => _.GetGameUserSupportInfoAsync(request));
		/// <summary> 小游戏 - 订单状态跟踪</summary>
        public OperationResult<GameOrderTrackingResponse> GameOrderTracking(GameOrderTackingRequest request) => Invoke(_ => _.GameOrderTracking(request));

	/// <summary> 小游戏 - 订单状态跟踪</summary>
        public Task<OperationResult<GameOrderTrackingResponse>> GameOrderTrackingAsync(GameOrderTackingRequest request) => InvokeAsync(_ => _.GameOrderTrackingAsync(request));
		/// <summary> 小游戏 - 更新游戏信息 - 内部用</summary>
        public OperationResult<UpdateGameInfoResponse> UpdateGameInfo(UpdateGameInfoRequest request) => Invoke(_ => _.UpdateGameInfo(request));

	/// <summary> 小游戏 - 更新游戏信息 - 内部用</summary>
        public Task<OperationResult<UpdateGameInfoResponse>> UpdateGameInfoAsync(UpdateGameInfoRequest request) => InvokeAsync(_ => _.UpdateGameInfoAsync(request));
		/// <summary> 小游戏 - 删除游戏的人员数据 - 内部用</summary>
        public OperationResult<bool> DeleteGameUserData(DeleteGameUserDataRequest request) => Invoke(_ => _.DeleteGameUserData(request));

	/// <summary> 小游戏 - 删除游戏的人员数据 - 内部用</summary>
        public Task<OperationResult<bool>> DeleteGameUserDataAsync(DeleteGameUserDataRequest request) => InvokeAsync(_ => _.DeleteGameUserDataAsync(request));
		/// <summary> 获取游戏实时排行榜</summary>
        public OperationResult<GetRankListResponse> GetRankList(GetRankListAsyncRequest request) => Invoke(_ => _.GetRankList(request));

	/// <summary> 获取游戏实时排行榜</summary>
        public Task<OperationResult<GetRankListResponse>> GetRankListAsync(GetRankListAsyncRequest request) => InvokeAsync(_ => _.GetRankListAsync(request));
		/// <summary> 用户进入游戏</summary>
        public OperationResult<UserParticipateGameResponse> UserParticipateGame(UserParticipateGameRequest request) => Invoke(_ => _.UserParticipateGame(request));

	/// <summary> 用户进入游戏</summary>
        public Task<OperationResult<UserParticipateGameResponse>> UserParticipateGameAsync(UserParticipateGameRequest request) => InvokeAsync(_ => _.UserParticipateGameAsync(request));
		/// <summary> 获取用户最近获得的奖品</summary>
        public OperationResult<GetUserLatestPrizeResponse> GetUserLatestPrize(GetUserLatestPrizeRequest request) => Invoke(_ => _.GetUserLatestPrize(request));

	/// <summary> 获取用户最近获得的奖品</summary>
        public Task<OperationResult<GetUserLatestPrizeResponse>> GetUserLatestPrizeAsync(GetUserLatestPrizeRequest request) => InvokeAsync(_ => _.GetUserLatestPrizeAsync(request));
		/// <summary> 获取某天之前的积分排行</summary>
        public OperationResult<GetRankListBeforeDayResponse> GetRankListBeforeDay(GetRankListBeforeDayRequest request) => Invoke(_ => _.GetRankListBeforeDay(request));

	/// <summary> 获取某天之前的积分排行</summary>
        public Task<OperationResult<GetRankListBeforeDayResponse>> GetRankListBeforeDayAsync(GetRankListBeforeDayRequest request) => InvokeAsync(_ => _.GetRankListBeforeDayAsync(request));
	}
	/// <summary>途虎联盟CPS返现</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ITuboAllianceService
    {
    	///<summary>佣金商品列表查询接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductListResponse")]
        Task<OperationResult<List<CommissionProductModel>>> GetCommissionProductListAsync(GetCommissionProductListRequest request);
		///<summary>佣金商品详情查询接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductDetatils", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductDetatilsResponse")]
        Task<OperationResult<CommissionProductModel>> GetCommissionProductDetatilsAsync(GetCommissionProductDetatilsRequest request);
		///<summary>下单商品记录接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CreateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CreateOrderItemRecordResponse")]
        Task<OperationResult<CreateOrderItemRecordResponse>> CreateOrderItemRecordAsync(CreateOrderItemRecordRequest request);
		///<summary>佣金订单商品记录更新接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/UpdateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/UpdateOrderItemRecordResponse")]
        Task<OperationResult<UpdateOrderItemRecordResponse>> UpdateOrderItemRecordAsync(UpdateOrderItemRecordRequest request);
		///<summary>订单商品返佣接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityRebate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityRebateResponse")]
        Task<OperationResult<CommodityRebateResponse>> CommodityRebateAsync(CommodityRebateRequest request);
		///<summary>订单商品扣佣接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityDeduction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityDeductionResponse")]
        Task<OperationResult<CommodityDeductionResponse>> CommodityDeductionAsync(CommodityDeductionRequest request);
		///<summary>CPS修改佣金流水状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CpsUpdateRunning", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CpsUpdateRunningResponse")]
        Task<OperationResult<CpsUpdateRunningResponse>> CpsUpdateRunningAsync(CpsUpdateRunningRequest request);
	}

	/// <summary>途虎联盟CPS返现</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ITuboAllianceClient : ITuboAllianceService, ITuhuServiceClient
    {
    	///<summary>佣金商品列表查询接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductListResponse")]
        OperationResult<List<CommissionProductModel>> GetCommissionProductList(GetCommissionProductListRequest request);
		///<summary>佣金商品详情查询接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductDetatils", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductDetatilsResponse")]
        OperationResult<CommissionProductModel> GetCommissionProductDetatils(GetCommissionProductDetatilsRequest request);
		///<summary>下单商品记录接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CreateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CreateOrderItemRecordResponse")]
        OperationResult<CreateOrderItemRecordResponse> CreateOrderItemRecord(CreateOrderItemRecordRequest request);
		///<summary>佣金订单商品记录更新接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/UpdateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/UpdateOrderItemRecordResponse")]
        OperationResult<UpdateOrderItemRecordResponse> UpdateOrderItemRecord(UpdateOrderItemRecordRequest request);
		///<summary>订单商品返佣接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityRebate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityRebateResponse")]
        OperationResult<CommodityRebateResponse> CommodityRebate(CommodityRebateRequest request);
		///<summary>订单商品扣佣接口</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityDeduction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityDeductionResponse")]
        OperationResult<CommodityDeductionResponse> CommodityDeduction(CommodityDeductionRequest request);
		///<summary>CPS修改佣金流水状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CpsUpdateRunning", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CpsUpdateRunningResponse")]
        OperationResult<CpsUpdateRunningResponse> CpsUpdateRunning(CpsUpdateRunningRequest request);
	}

	/// <summary>途虎联盟CPS返现</summary>
	public partial class TuboAllianceClient : TuhuServiceClient<ITuboAllianceClient>, ITuboAllianceClient
    {
    	///<summary>佣金商品列表查询接口</summary>
        public OperationResult<List<CommissionProductModel>> GetCommissionProductList(GetCommissionProductListRequest request) => Invoke(_ => _.GetCommissionProductList(request));

	///<summary>佣金商品列表查询接口</summary>
        public Task<OperationResult<List<CommissionProductModel>>> GetCommissionProductListAsync(GetCommissionProductListRequest request) => InvokeAsync(_ => _.GetCommissionProductListAsync(request));
		///<summary>佣金商品详情查询接口</summary>
        public OperationResult<CommissionProductModel> GetCommissionProductDetatils(GetCommissionProductDetatilsRequest request) => Invoke(_ => _.GetCommissionProductDetatils(request));

	///<summary>佣金商品详情查询接口</summary>
        public Task<OperationResult<CommissionProductModel>> GetCommissionProductDetatilsAsync(GetCommissionProductDetatilsRequest request) => InvokeAsync(_ => _.GetCommissionProductDetatilsAsync(request));
		///<summary>下单商品记录接口</summary>
        public OperationResult<CreateOrderItemRecordResponse> CreateOrderItemRecord(CreateOrderItemRecordRequest request) => Invoke(_ => _.CreateOrderItemRecord(request));

	///<summary>下单商品记录接口</summary>
        public Task<OperationResult<CreateOrderItemRecordResponse>> CreateOrderItemRecordAsync(CreateOrderItemRecordRequest request) => InvokeAsync(_ => _.CreateOrderItemRecordAsync(request));
		///<summary>佣金订单商品记录更新接口</summary>
        public OperationResult<UpdateOrderItemRecordResponse> UpdateOrderItemRecord(UpdateOrderItemRecordRequest request) => Invoke(_ => _.UpdateOrderItemRecord(request));

	///<summary>佣金订单商品记录更新接口</summary>
        public Task<OperationResult<UpdateOrderItemRecordResponse>> UpdateOrderItemRecordAsync(UpdateOrderItemRecordRequest request) => InvokeAsync(_ => _.UpdateOrderItemRecordAsync(request));
		///<summary>订单商品返佣接口</summary>
        public OperationResult<CommodityRebateResponse> CommodityRebate(CommodityRebateRequest request) => Invoke(_ => _.CommodityRebate(request));

	///<summary>订单商品返佣接口</summary>
        public Task<OperationResult<CommodityRebateResponse>> CommodityRebateAsync(CommodityRebateRequest request) => InvokeAsync(_ => _.CommodityRebateAsync(request));
		///<summary>订单商品扣佣接口</summary>
        public OperationResult<CommodityDeductionResponse> CommodityDeduction(CommodityDeductionRequest request) => Invoke(_ => _.CommodityDeduction(request));

	///<summary>订单商品扣佣接口</summary>
        public Task<OperationResult<CommodityDeductionResponse>> CommodityDeductionAsync(CommodityDeductionRequest request) => InvokeAsync(_ => _.CommodityDeductionAsync(request));
		///<summary>CPS修改佣金流水状态</summary>
        public OperationResult<CpsUpdateRunningResponse> CpsUpdateRunning(CpsUpdateRunningRequest request) => Invoke(_ => _.CpsUpdateRunning(request));

	///<summary>CPS修改佣金流水状态</summary>
        public Task<OperationResult<CpsUpdateRunningResponse>> CpsUpdateRunningAsync(CpsUpdateRunningRequest request) => InvokeAsync(_ => _.CpsUpdateRunningAsync(request));
	}
	///<summary>车友群服务</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICarFriendsGroupService
    {
    	/// <summary> 获取车友群列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupListResponse")]
        Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupListAsync(GetCarFriendsGroupListRequest request);
		/// <summary> 获取所有热门车型  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetRecommendVehicleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetRecommendVehicleListResponse")]
        Task<OperationResult<List<RecommendVehicleResponse>>> GetRecommendVehicleListAsync();
		/// <summary> 根据pkid获取车友群  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupModelResponse")]
        Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupModelAsync(int pkid);
		/// <summary> 根据pkid获取途虎管理员信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsAdministratorsModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsAdministratorsModelResponse")]
        Task<OperationResult<CarFriendsAdministratorsResponse>> GetCarFriendsAdministratorsModelAsync(int pkid);
		/// <summary> 车友群小程序推送车友群或群主信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupPushInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupPushInfoResponse")]
        Task<OperationResult<bool>> CarFriendsGroupPushInfoAsync(GetCarFriendsGroupPushInfoRequest request);
		/// <summary> 调用MQ延迟推送  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupMqDelayPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupMqDelayPushResponse")]
        Task<OperationResult<bool>> CarFriendsGroupMqDelayPushAsync(GetCarFriendsGroupPushInfoRequest request);
	}

	///<summary>车友群服务</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICarFriendsGroupClient : ICarFriendsGroupService, ITuhuServiceClient
    {
    	/// <summary> 获取车友群列表 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupListResponse")]
        OperationResult<CarFriendsGroupInfoResponse> GetCarFriendsGroupList(GetCarFriendsGroupListRequest request);
		/// <summary> 获取所有热门车型  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetRecommendVehicleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetRecommendVehicleListResponse")]
        OperationResult<List<RecommendVehicleResponse>> GetRecommendVehicleList();
		/// <summary> 根据pkid获取车友群  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupModelResponse")]
        OperationResult<CarFriendsGroupInfoResponse> GetCarFriendsGroupModel(int pkid);
		/// <summary> 根据pkid获取途虎管理员信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsAdministratorsModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsAdministratorsModelResponse")]
        OperationResult<CarFriendsAdministratorsResponse> GetCarFriendsAdministratorsModel(int pkid);
		/// <summary> 车友群小程序推送车友群或群主信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupPushInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupPushInfoResponse")]
        OperationResult<bool> CarFriendsGroupPushInfo(GetCarFriendsGroupPushInfoRequest request);
		/// <summary> 调用MQ延迟推送  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupMqDelayPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupMqDelayPushResponse")]
        OperationResult<bool> CarFriendsGroupMqDelayPush(GetCarFriendsGroupPushInfoRequest request);
	}

	///<summary>车友群服务</summary>///
	public partial class CarFriendsGroupClient : TuhuServiceClient<ICarFriendsGroupClient>, ICarFriendsGroupClient
    {
    	/// <summary> 获取车友群列表 </summary>
        public OperationResult<CarFriendsGroupInfoResponse> GetCarFriendsGroupList(GetCarFriendsGroupListRequest request) => Invoke(_ => _.GetCarFriendsGroupList(request));

	/// <summary> 获取车友群列表 </summary>
        public Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupListAsync(GetCarFriendsGroupListRequest request) => InvokeAsync(_ => _.GetCarFriendsGroupListAsync(request));
		/// <summary> 获取所有热门车型  </summary>
        public OperationResult<List<RecommendVehicleResponse>> GetRecommendVehicleList() => Invoke(_ => _.GetRecommendVehicleList());

	/// <summary> 获取所有热门车型  </summary>
        public Task<OperationResult<List<RecommendVehicleResponse>>> GetRecommendVehicleListAsync() => InvokeAsync(_ => _.GetRecommendVehicleListAsync());
		/// <summary> 根据pkid获取车友群  </summary>
        public OperationResult<CarFriendsGroupInfoResponse> GetCarFriendsGroupModel(int pkid) => Invoke(_ => _.GetCarFriendsGroupModel(pkid));

	/// <summary> 根据pkid获取车友群  </summary>
        public Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupModelAsync(int pkid) => InvokeAsync(_ => _.GetCarFriendsGroupModelAsync(pkid));
		/// <summary> 根据pkid获取途虎管理员信息  </summary>
        public OperationResult<CarFriendsAdministratorsResponse> GetCarFriendsAdministratorsModel(int pkid) => Invoke(_ => _.GetCarFriendsAdministratorsModel(pkid));

	/// <summary> 根据pkid获取途虎管理员信息  </summary>
        public Task<OperationResult<CarFriendsAdministratorsResponse>> GetCarFriendsAdministratorsModelAsync(int pkid) => InvokeAsync(_ => _.GetCarFriendsAdministratorsModelAsync(pkid));
		/// <summary> 车友群小程序推送车友群或群主信息  </summary>
        public OperationResult<bool> CarFriendsGroupPushInfo(GetCarFriendsGroupPushInfoRequest request) => Invoke(_ => _.CarFriendsGroupPushInfo(request));

	/// <summary> 车友群小程序推送车友群或群主信息  </summary>
        public Task<OperationResult<bool>> CarFriendsGroupPushInfoAsync(GetCarFriendsGroupPushInfoRequest request) => InvokeAsync(_ => _.CarFriendsGroupPushInfoAsync(request));
		/// <summary> 调用MQ延迟推送  </summary>
        public OperationResult<bool> CarFriendsGroupMqDelayPush(GetCarFriendsGroupPushInfoRequest request) => Invoke(_ => _.CarFriendsGroupMqDelayPush(request));

	/// <summary> 调用MQ延迟推送  </summary>
        public Task<OperationResult<bool>> CarFriendsGroupMqDelayPushAsync(GetCarFriendsGroupPushInfoRequest request) => InvokeAsync(_ => _.CarFriendsGroupMqDelayPushAsync(request));
	}
	///<summary>一分钱洗车服务</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IWashCarCouponService
    {
    	/// <summary> 新增 一分钱洗车优惠券领取记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/CreateWashCarCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/CreateWashCarCouponResponse")]
        Task<OperationResult<bool>> CreateWashCarCouponAsync(WashCarCouponRecordModel request);
		/// <summary> 根据userid获取  一分钱洗车优惠券领取记录  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponListByUserids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponListByUseridsResponse")]
        Task<OperationResult<List<WashCarCouponRecordModel>>> GetWashCarCouponListByUseridsAsync(GetWashCarCouponListByUseridsRequest request);
		/// <summary> 根据优惠券id获取  一分钱洗车优惠券领取记录  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponInfoByPromotionCodeID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponInfoByPromotionCodeIDResponse")]
        Task<OperationResult<WashCarCouponRecordModel>> GetWashCarCouponInfoByPromotionCodeIDAsync(GetWashCarCouponInfoByPromotionCodeIDRequest request);
	}

	///<summary>一分钱洗车服务</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IWashCarCouponClient : IWashCarCouponService, ITuhuServiceClient
    {
    	/// <summary> 新增 一分钱洗车优惠券领取记录 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/CreateWashCarCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/CreateWashCarCouponResponse")]
        OperationResult<bool> CreateWashCarCoupon(WashCarCouponRecordModel request);
		/// <summary> 根据userid获取  一分钱洗车优惠券领取记录  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponListByUserids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponListByUseridsResponse")]
        OperationResult<List<WashCarCouponRecordModel>> GetWashCarCouponListByUserids(GetWashCarCouponListByUseridsRequest request);
		/// <summary> 根据优惠券id获取  一分钱洗车优惠券领取记录  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponInfoByPromotionCodeID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponInfoByPromotionCodeIDResponse")]
        OperationResult<WashCarCouponRecordModel> GetWashCarCouponInfoByPromotionCodeID(GetWashCarCouponInfoByPromotionCodeIDRequest request);
	}

	///<summary>一分钱洗车服务</summary>///
	public partial class WashCarCouponClient : TuhuServiceClient<IWashCarCouponClient>, IWashCarCouponClient
    {
    	/// <summary> 新增 一分钱洗车优惠券领取记录 </summary>
        public OperationResult<bool> CreateWashCarCoupon(WashCarCouponRecordModel request) => Invoke(_ => _.CreateWashCarCoupon(request));

	/// <summary> 新增 一分钱洗车优惠券领取记录 </summary>
        public Task<OperationResult<bool>> CreateWashCarCouponAsync(WashCarCouponRecordModel request) => InvokeAsync(_ => _.CreateWashCarCouponAsync(request));
		/// <summary> 根据userid获取  一分钱洗车优惠券领取记录  </summary>
        public OperationResult<List<WashCarCouponRecordModel>> GetWashCarCouponListByUserids(GetWashCarCouponListByUseridsRequest request) => Invoke(_ => _.GetWashCarCouponListByUserids(request));

	/// <summary> 根据userid获取  一分钱洗车优惠券领取记录  </summary>
        public Task<OperationResult<List<WashCarCouponRecordModel>>> GetWashCarCouponListByUseridsAsync(GetWashCarCouponListByUseridsRequest request) => InvokeAsync(_ => _.GetWashCarCouponListByUseridsAsync(request));
		/// <summary> 根据优惠券id获取  一分钱洗车优惠券领取记录  </summary>
        public OperationResult<WashCarCouponRecordModel> GetWashCarCouponInfoByPromotionCodeID(GetWashCarCouponInfoByPromotionCodeIDRequest request) => Invoke(_ => _.GetWashCarCouponInfoByPromotionCodeID(request));

	/// <summary> 根据优惠券id获取  一分钱洗车优惠券领取记录  </summary>
        public Task<OperationResult<WashCarCouponRecordModel>> GetWashCarCouponInfoByPromotionCodeIDAsync(GetWashCarCouponInfoByPromotionCodeIDRequest request) => InvokeAsync(_ => _.GetWashCarCouponInfoByPromotionCodeIDAsync(request));
	}
	///<summary>活动集合服务</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ILuckyCharmService
    {
    	/// <summary> 新增 活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmActivityResponse")]
        Task<OperationResult<bool>> AddLuckyCharmActivityAsync(AddLuckyCharmActivityRequest request);
		/// <summary> 获取活动详情  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/GetLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/GetLuckyCharmActivityResponse")]
        Task<OperationResult<LuckyCharmActivityInfoResponse>> GetLuckyCharmActivityAsync(int pkid);
		/// <summary> 用户报名活动  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmUserResponse")]
        Task<OperationResult<bool>> AddLuckyCharmUserAsync(AddLuckyCharmUserRequest request);
		/// <summary> 修改用户报名信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/UpdateLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/UpdateLuckyCharmUserResponse")]
        Task<OperationResult<bool>> UpdateLuckyCharmUserAsync(UpdateLuckyCharmUserRequest request);
		/// <summary> 分页获取用户报名信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmUserResponse")]
        Task<OperationResult<PageLuckyCharmUserResponse>> PageLuckyCharmUserAsync(PageLuckyCharmUserRequest request);
		/// <summary> 审核报名用户  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AuditLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AuditLuckyCharmUserResponse")]
        Task<OperationResult<bool>> AuditLuckyCharmUserAsync(int pkid);
		/// <summary> 删除报名用户  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmUserResponse")]
        Task<OperationResult<bool>> DelLuckyCharmUserAsync(int pkid);
		/// <summary> 删除活动  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmActivityResponse")]
        Task<OperationResult<bool>> DelLuckyCharmActivityAsync(int pkid);
		/// <summary> 分页获取活动信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmActivityResponse")]
        Task<OperationResult<PageLuckyCharmActivityResponse>> PageLuckyCharmActivityAsync(PageLuckyCharmActivityRequest request);
	}

	///<summary>活动集合服务</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ILuckyCharmClient : ILuckyCharmService, ITuhuServiceClient
    {
    	/// <summary> 新增 活动 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmActivityResponse")]
        OperationResult<bool> AddLuckyCharmActivity(AddLuckyCharmActivityRequest request);
		/// <summary> 获取活动详情  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/GetLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/GetLuckyCharmActivityResponse")]
        OperationResult<LuckyCharmActivityInfoResponse> GetLuckyCharmActivity(int pkid);
		/// <summary> 用户报名活动  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmUserResponse")]
        OperationResult<bool> AddLuckyCharmUser(AddLuckyCharmUserRequest request);
		/// <summary> 修改用户报名信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/UpdateLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/UpdateLuckyCharmUserResponse")]
        OperationResult<bool> UpdateLuckyCharmUser(UpdateLuckyCharmUserRequest request);
		/// <summary> 分页获取用户报名信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmUserResponse")]
        OperationResult<PageLuckyCharmUserResponse> PageLuckyCharmUser(PageLuckyCharmUserRequest request);
		/// <summary> 审核报名用户  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AuditLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AuditLuckyCharmUserResponse")]
        OperationResult<bool> AuditLuckyCharmUser(int pkid);
		/// <summary> 删除报名用户  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmUserResponse")]
        OperationResult<bool> DelLuckyCharmUser(int pkid);
		/// <summary> 删除活动  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmActivityResponse")]
        OperationResult<bool> DelLuckyCharmActivity(int pkid);
		/// <summary> 分页获取活动信息  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmActivityResponse")]
        OperationResult<PageLuckyCharmActivityResponse> PageLuckyCharmActivity(PageLuckyCharmActivityRequest request);
	}

	///<summary>活动集合服务</summary>///
	public partial class LuckyCharmClient : TuhuServiceClient<ILuckyCharmClient>, ILuckyCharmClient
    {
    	/// <summary> 新增 活动 </summary>
        public OperationResult<bool> AddLuckyCharmActivity(AddLuckyCharmActivityRequest request) => Invoke(_ => _.AddLuckyCharmActivity(request));

	/// <summary> 新增 活动 </summary>
        public Task<OperationResult<bool>> AddLuckyCharmActivityAsync(AddLuckyCharmActivityRequest request) => InvokeAsync(_ => _.AddLuckyCharmActivityAsync(request));
		/// <summary> 获取活动详情  </summary>
        public OperationResult<LuckyCharmActivityInfoResponse> GetLuckyCharmActivity(int pkid) => Invoke(_ => _.GetLuckyCharmActivity(pkid));

	/// <summary> 获取活动详情  </summary>
        public Task<OperationResult<LuckyCharmActivityInfoResponse>> GetLuckyCharmActivityAsync(int pkid) => InvokeAsync(_ => _.GetLuckyCharmActivityAsync(pkid));
		/// <summary> 用户报名活动  </summary>
        public OperationResult<bool> AddLuckyCharmUser(AddLuckyCharmUserRequest request) => Invoke(_ => _.AddLuckyCharmUser(request));

	/// <summary> 用户报名活动  </summary>
        public Task<OperationResult<bool>> AddLuckyCharmUserAsync(AddLuckyCharmUserRequest request) => InvokeAsync(_ => _.AddLuckyCharmUserAsync(request));
		/// <summary> 修改用户报名信息  </summary>
        public OperationResult<bool> UpdateLuckyCharmUser(UpdateLuckyCharmUserRequest request) => Invoke(_ => _.UpdateLuckyCharmUser(request));

	/// <summary> 修改用户报名信息  </summary>
        public Task<OperationResult<bool>> UpdateLuckyCharmUserAsync(UpdateLuckyCharmUserRequest request) => InvokeAsync(_ => _.UpdateLuckyCharmUserAsync(request));
		/// <summary> 分页获取用户报名信息  </summary>
        public OperationResult<PageLuckyCharmUserResponse> PageLuckyCharmUser(PageLuckyCharmUserRequest request) => Invoke(_ => _.PageLuckyCharmUser(request));

	/// <summary> 分页获取用户报名信息  </summary>
        public Task<OperationResult<PageLuckyCharmUserResponse>> PageLuckyCharmUserAsync(PageLuckyCharmUserRequest request) => InvokeAsync(_ => _.PageLuckyCharmUserAsync(request));
		/// <summary> 审核报名用户  </summary>
        public OperationResult<bool> AuditLuckyCharmUser(int pkid) => Invoke(_ => _.AuditLuckyCharmUser(pkid));

	/// <summary> 审核报名用户  </summary>
        public Task<OperationResult<bool>> AuditLuckyCharmUserAsync(int pkid) => InvokeAsync(_ => _.AuditLuckyCharmUserAsync(pkid));
		/// <summary> 删除报名用户  </summary>
        public OperationResult<bool> DelLuckyCharmUser(int pkid) => Invoke(_ => _.DelLuckyCharmUser(pkid));

	/// <summary> 删除报名用户  </summary>
        public Task<OperationResult<bool>> DelLuckyCharmUserAsync(int pkid) => InvokeAsync(_ => _.DelLuckyCharmUserAsync(pkid));
		/// <summary> 删除活动  </summary>
        public OperationResult<bool> DelLuckyCharmActivity(int pkid) => Invoke(_ => _.DelLuckyCharmActivity(pkid));

	/// <summary> 删除活动  </summary>
        public Task<OperationResult<bool>> DelLuckyCharmActivityAsync(int pkid) => InvokeAsync(_ => _.DelLuckyCharmActivityAsync(pkid));
		/// <summary> 分页获取活动信息  </summary>
        public OperationResult<PageLuckyCharmActivityResponse> PageLuckyCharmActivity(PageLuckyCharmActivityRequest request) => Invoke(_ => _.PageLuckyCharmActivity(request));

	/// <summary> 分页获取活动信息  </summary>
        public Task<OperationResult<PageLuckyCharmActivityResponse>> PageLuckyCharmActivityAsync(PageLuckyCharmActivityRequest request) => InvokeAsync(_ => _.PageLuckyCharmActivityAsync(request));
	}
}
