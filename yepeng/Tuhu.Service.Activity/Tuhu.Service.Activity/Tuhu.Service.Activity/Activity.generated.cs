
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Data;
using Tuhu.Models;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using CreateOrderRequest= Tuhu.Service.Order.Request.CreateOrderRequest;
using CreateOrderResult = Tuhu.Service.Activity.Models.CreateOrderResult;
using Tuhu.Service.Activity.Models.Response;
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
		/// <summary>根据活动ID查询活动详情</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIDResponse")]
        Task<OperationResult<FlashSaleModel>> SelectFlashSaleDataByActivityIDAsync(Guid activityID);
		/// <summary>获取限时抢购缓存的内容</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleListResponse")]
        Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleListAsync(Guid[] activityIDs);
		/// <summary>查今天天天秒杀当天数据</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayDataResponse")]
        Task<OperationResult<IEnumerable<FlashSaleModel>>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate=null,bool needProducts=true);
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
	}

	/// <summary>限时抢购</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleClient : IFlashSaleService, ITuhuServiceClient
    {
    	/// <summary>更新限时抢购内容到缓存</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityIDResponse")]
        OperationResult<bool> UpdateFlashSaleDataToCouchBaseByActivityID(Guid activityID);
		/// <summary>根据活动ID查询活动详情</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIDResponse")]
        OperationResult<FlashSaleModel> SelectFlashSaleDataByActivityID(Guid activityID);
		/// <summary>获取限时抢购缓存的内容</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleListResponse")]
        OperationResult<List<FlashSaleModel>> GetFlashSaleList(Guid[] activityIDs);
		/// <summary>查今天天天秒杀当天数据</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayDataResponse")]
        OperationResult<IEnumerable<FlashSaleModel>> SelectSecondKillTodayData(int activityType, DateTime? scheduleDate=null,bool needProducts=true);
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
		/// <summary>根据活动ID查询活动详情</summary>
        /// <returns></returns>
        public OperationResult<FlashSaleModel> SelectFlashSaleDataByActivityID(Guid activityID) => Invoke(_ => _.SelectFlashSaleDataByActivityID(activityID));

	/// <summary>根据活动ID查询活动详情</summary>
        /// <returns></returns>
        public Task<OperationResult<FlashSaleModel>> SelectFlashSaleDataByActivityIDAsync(Guid activityID) => InvokeAsync(_ => _.SelectFlashSaleDataByActivityIDAsync(activityID));
		/// <summary>获取限时抢购缓存的内容</summary>
        /// <returns></returns>
        public OperationResult<List<FlashSaleModel>> GetFlashSaleList(Guid[] activityIDs) => Invoke(_ => _.GetFlashSaleList(activityIDs));

	/// <summary>获取限时抢购缓存的内容</summary>
        /// <returns></returns>
        public Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleListAsync(Guid[] activityIDs) => InvokeAsync(_ => _.GetFlashSaleListAsync(activityIDs));
		/// <summary>查今天天天秒杀当天数据</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleModel>> SelectSecondKillTodayData(int activityType, DateTime? scheduleDate=null,bool needProducts=true) => Invoke(_ => _.SelectSecondKillTodayData(activityType,scheduleDate,needProducts));

	/// <summary>查今天天天秒杀当天数据</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleModel>>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate=null,bool needProducts=true) => InvokeAsync(_ => _.SelectSecondKillTodayDataAsync(activityType,scheduleDate,needProducts));
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
		///<summary></summary>///<returns></returns>
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
<<<<<<< HEAD
		///<summary>保存用户答题数据到数据库</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswerResponse")]
        Task<OperationResult<SubmitActivityQuestionUserAnswerResponse>> SubmitQuestionUserAnswerAsync(SubmitActivityQuestionUserAnswerRequest request);
		///<summary>更新用户答题结果状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResultResponse")]
        Task<OperationResult<ModifyQuestionUserAnswerResultResponse>> ModifyQuestionUserAnswerResultAsync(ModifyQuestionUserAnswerResultRequest request);
=======
		///<summary>��ȡ��б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityModelsPaged", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityModelsPagedResponse")]
        Task<OperationResult<Tuple<IEnumerable<ActivityNewModel>, int>>> GetActivityModelsPagedAsync(int pageIndex,int pageSize);
		///<summary>���ݻId��ȡ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityModelByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityModelByActivityIdResponse")]
        Task<OperationResult<ActivityNewModel>> GetActivityModelByActivityIdAsync(Guid activityId);
		///<summary>�������������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertActivityModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertActivityModelResponse")]
        Task<OperationResult<int>> InsertActivityModelAsync(ActivityNewModel activityModel);
		///<summary>���»</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityModelResponse")]
        Task<OperationResult<int>> UpdateActivityModelAsync(ActivityNewModel activityModel);
		///<summary>ɾ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/DeleteActivityModelByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/DeleteActivityModelByActivityIdResponse")]
        Task<OperationResult<bool>> DeleteActivityModelByActivityIdAsync(Guid activityId);
		///<summary>�û�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertUserActivityModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertUserActivityModelResponse")]
        Task<OperationResult<bool>> InsertUserActivityModelAsync(UserApplyActivityModel userActivityModel);
		///<summary>����û������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateUserActivityStatusByPKID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateUserActivityStatusByPKIDResponse")]
        Task<OperationResult<bool>> UpdateUserActivityStatusByPKIDAsync(UserApplyActivityModel userActivityModel);
		///<summary>��ҳ��ȡ�û������б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityModelsPaged", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityModelsPagedResponse")]
        Task<OperationResult<Tuple<IEnumerable<UserApplyActivityModel>, int>>> GetUserApplyActivityModelsPagedAsync(Guid activityId, AuditStatus auditStatus, int pageIndex, int pageSize);
		///<summary>���ݻid��ȡ������Ա����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityApplyUserCountByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityApplyUserCountByActivityIdResponse")]
        Task<OperationResult<int>> GetActivityApplyUserCountByActivityIdAsync(Guid activityId);
		///<summary>���ݻid��ȡ������Ա���ͨ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityApplyUserPassCountByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityApplyUserPassCountByActivityIdResponse")]
        Task<OperationResult<int>> GetActivityApplyUserPassCountByActivityIdAsync(Guid activityId);
		///<summary>����pkid��ȡ������Ա</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityByPKID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityByPKIDResponse")]
        Task<OperationResult<UserApplyActivityModel>> GetUserApplyActivityByPKIDAsync(int pkid);
		///<summary>ɾ���û�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/DeleteUserApplyActivityModelByPKID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/DeleteUserApplyActivityModelByPKIDResponse")]
        Task<OperationResult<bool>> DeleteUserApplyActivityModelByPKIDAsync(int pkid);
		///<summary>����û�������ֻ��š����ƺš���ʻ֤���Ƿ��ظ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckUserApplyActivityInfoIsExist", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckUserApplyActivityInfoIsExistResponse")]
        Task<OperationResult<bool>> CheckUserApplyActivityInfoIsExistAsync(Guid activityId,string mobile, string carNum, string driverNum);
		///<summary>���û����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityModelByActivityIdCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityModelByActivityIdCacheResponse")]
        Task<OperationResult<bool>> RefreshActivityModelByActivityIdCacheAsync(Guid activityId);
		///<summary>�Ƴ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RemoveActivityModelByActivityIdCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RemoveActivityModelByActivityIdCacheResponse")]
        Task<OperationResult<bool>> RemoveActivityModelByActivityIdCacheAsync(Guid activityId);
		///<summary>�����û������SortedSetCache</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddUserApplyActivitySortedSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddUserApplyActivitySortedSetCacheResponse")]
        Task<OperationResult<bool>> AddUserApplyActivitySortedSetCacheAsync(UserApplyActivityModel userApplyActivityModel);
		///<summary>ɾ��һ���û������SortedSetCache</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RemoveOneUserApplyActivitySortedSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RemoveOneUserApplyActivitySortedSetCacheResponse")]
        Task<OperationResult<bool>> RemoveOneUserApplyActivitySortedSetCacheAsync(UserApplyActivityModel userApplyActivityModel);
		///<summary>��ȡ�û����������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityRangeByScore", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityRangeByScoreResponse")]
        Task<OperationResult<IEnumerable<UserApplyActivityModel>>> GetUserApplyActivityRangeByScoreAsync();
		///<summary>��ȡ�û�����������û������SortedSetLength</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivitySortedSetLength", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivitySortedSetLengthResponse")]
        Task<OperationResult<long>> GetUserApplyActivitySortedSetLengthAsync();
>>>>>>> cef10ecfe95a62dbe27086896d29d0abab5da3d4
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
		///<summary></summary>///<returns></returns>
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
<<<<<<< HEAD
		///<summary>保存用户答题数据到数据库</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswerResponse")]
        OperationResult<SubmitActivityQuestionUserAnswerResponse> SubmitQuestionUserAnswer(SubmitActivityQuestionUserAnswerRequest request);
		///<summary>更新用户答题结果状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResultResponse")]
        OperationResult<ModifyQuestionUserAnswerResultResponse> ModifyQuestionUserAnswerResult(ModifyQuestionUserAnswerResultRequest request);
=======
		///<summary>��ȡ��б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityModelsPaged", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityModelsPagedResponse")]
        OperationResult<Tuple<IEnumerable<ActivityNewModel>, int>> GetActivityModelsPaged(int pageIndex,int pageSize);
		///<summary>���ݻId��ȡ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityModelByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityModelByActivityIdResponse")]
        OperationResult<ActivityNewModel> GetActivityModelByActivityId(Guid activityId);
		///<summary>�������������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertActivityModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertActivityModelResponse")]
        OperationResult<int> InsertActivityModel(ActivityNewModel activityModel);
		///<summary>���»</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityModelResponse")]
        OperationResult<int> UpdateActivityModel(ActivityNewModel activityModel);
		///<summary>ɾ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/DeleteActivityModelByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/DeleteActivityModelByActivityIdResponse")]
        OperationResult<bool> DeleteActivityModelByActivityId(Guid activityId);
		///<summary>�û�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertUserActivityModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertUserActivityModelResponse")]
        OperationResult<bool> InsertUserActivityModel(UserApplyActivityModel userActivityModel);
		///<summary>����û������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateUserActivityStatusByPKID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateUserActivityStatusByPKIDResponse")]
        OperationResult<bool> UpdateUserActivityStatusByPKID(UserApplyActivityModel userActivityModel);
		///<summary>��ҳ��ȡ�û������б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityModelsPaged", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityModelsPagedResponse")]
        OperationResult<Tuple<IEnumerable<UserApplyActivityModel>, int>> GetUserApplyActivityModelsPaged(Guid activityId, AuditStatus auditStatus, int pageIndex, int pageSize);
		///<summary>���ݻid��ȡ������Ա����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityApplyUserCountByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityApplyUserCountByActivityIdResponse")]
        OperationResult<int> GetActivityApplyUserCountByActivityId(Guid activityId);
		///<summary>���ݻid��ȡ������Ա���ͨ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityApplyUserPassCountByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityApplyUserPassCountByActivityIdResponse")]
        OperationResult<int> GetActivityApplyUserPassCountByActivityId(Guid activityId);
		///<summary>����pkid��ȡ������Ա</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityByPKID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityByPKIDResponse")]
        OperationResult<UserApplyActivityModel> GetUserApplyActivityByPKID(int pkid);
		///<summary>ɾ���û�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/DeleteUserApplyActivityModelByPKID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/DeleteUserApplyActivityModelByPKIDResponse")]
        OperationResult<bool> DeleteUserApplyActivityModelByPKID(int pkid);
		///<summary>����û�������ֻ��š����ƺš���ʻ֤���Ƿ��ظ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckUserApplyActivityInfoIsExist", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckUserApplyActivityInfoIsExistResponse")]
        OperationResult<bool> CheckUserApplyActivityInfoIsExist(Guid activityId,string mobile, string carNum, string driverNum);
		///<summary>���û����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityModelByActivityIdCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityModelByActivityIdCacheResponse")]
        OperationResult<bool> RefreshActivityModelByActivityIdCache(Guid activityId);
		///<summary>�Ƴ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RemoveActivityModelByActivityIdCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RemoveActivityModelByActivityIdCacheResponse")]
        OperationResult<bool> RemoveActivityModelByActivityIdCache(Guid activityId);
		///<summary>�����û������SortedSetCache</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddUserApplyActivitySortedSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddUserApplyActivitySortedSetCacheResponse")]
        OperationResult<bool> AddUserApplyActivitySortedSetCache(UserApplyActivityModel userApplyActivityModel);
		///<summary>ɾ��һ���û������SortedSetCache</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RemoveOneUserApplyActivitySortedSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RemoveOneUserApplyActivitySortedSetCacheResponse")]
        OperationResult<bool> RemoveOneUserApplyActivitySortedSetCache(UserApplyActivityModel userApplyActivityModel);
		///<summary>��ȡ�û����������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityRangeByScore", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivityRangeByScoreResponse")]
        OperationResult<IEnumerable<UserApplyActivityModel>> GetUserApplyActivityRangeByScore();
		///<summary>��ȡ�û�����������û������SortedSetLength</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivitySortedSetLength", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserApplyActivitySortedSetLengthResponse")]
        OperationResult<long> GetUserApplyActivitySortedSetLength();
>>>>>>> cef10ecfe95a62dbe27086896d29d0abab5da3d4
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
		///<summary></summary>///<returns></returns>
        public OperationResult<CouponActivityConfigModel> SelectCouponActivityConfig(string activityNum, int type) => Invoke(_ => _.SelectCouponActivityConfig(activityNum,type));

	///<summary></summary>///<returns></returns>
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
<<<<<<< HEAD
		///<summary>保存用户答题数据到数据库</summary>
        public OperationResult<SubmitActivityQuestionUserAnswerResponse> SubmitQuestionUserAnswer(SubmitActivityQuestionUserAnswerRequest request) => Invoke(_ => _.SubmitQuestionUserAnswer( request));

	///<summary>保存用户答题数据到数据库</summary>
        public Task<OperationResult<SubmitActivityQuestionUserAnswerResponse>> SubmitQuestionUserAnswerAsync(SubmitActivityQuestionUserAnswerRequest request) => InvokeAsync(_ => _.SubmitQuestionUserAnswerAsync( request));
		///<summary>更新用户答题结果状态</summary>
        public OperationResult<ModifyQuestionUserAnswerResultResponse> ModifyQuestionUserAnswerResult(ModifyQuestionUserAnswerResultRequest request) => Invoke(_ => _.ModifyQuestionUserAnswerResult( request));

	///<summary>更新用户答题结果状态</summary>
        public Task<OperationResult<ModifyQuestionUserAnswerResultResponse>> ModifyQuestionUserAnswerResultAsync(ModifyQuestionUserAnswerResultRequest request) => InvokeAsync(_ => _.ModifyQuestionUserAnswerResultAsync( request));
=======
		///<summary>��ȡ��б�</summary>
        public OperationResult<Tuple<IEnumerable<ActivityNewModel>, int>> GetActivityModelsPaged(int pageIndex,int pageSize) => Invoke(_ => _.GetActivityModelsPaged( pageIndex,pageSize));

	///<summary>��ȡ��б�</summary>
        public Task<OperationResult<Tuple<IEnumerable<ActivityNewModel>, int>>> GetActivityModelsPagedAsync(int pageIndex,int pageSize) => InvokeAsync(_ => _.GetActivityModelsPagedAsync( pageIndex,pageSize));
		///<summary>���ݻId��ȡ�</summary>
        public OperationResult<ActivityNewModel> GetActivityModelByActivityId(Guid activityId) => Invoke(_ => _.GetActivityModelByActivityId(activityId));

	///<summary>���ݻId��ȡ�</summary>
        public Task<OperationResult<ActivityNewModel>> GetActivityModelByActivityIdAsync(Guid activityId) => InvokeAsync(_ => _.GetActivityModelByActivityIdAsync(activityId));
		///<summary>�������������</summary>
        public OperationResult<int> InsertActivityModel(ActivityNewModel activityModel) => Invoke(_ => _.InsertActivityModel(activityModel));

	///<summary>�������������</summary>
        public Task<OperationResult<int>> InsertActivityModelAsync(ActivityNewModel activityModel) => InvokeAsync(_ => _.InsertActivityModelAsync(activityModel));
		///<summary>���»</summary>
        public OperationResult<int> UpdateActivityModel(ActivityNewModel activityModel) => Invoke(_ => _.UpdateActivityModel(activityModel));

	///<summary>���»</summary>
        public Task<OperationResult<int>> UpdateActivityModelAsync(ActivityNewModel activityModel) => InvokeAsync(_ => _.UpdateActivityModelAsync(activityModel));
		///<summary>ɾ���</summary>
        public OperationResult<bool> DeleteActivityModelByActivityId(Guid activityId) => Invoke(_ => _.DeleteActivityModelByActivityId(activityId));

	///<summary>ɾ���</summary>
        public Task<OperationResult<bool>> DeleteActivityModelByActivityIdAsync(Guid activityId) => InvokeAsync(_ => _.DeleteActivityModelByActivityIdAsync(activityId));
		///<summary>�û�����</summary>
        public OperationResult<bool> InsertUserActivityModel(UserApplyActivityModel userActivityModel) => Invoke(_ => _.InsertUserActivityModel(userActivityModel));

	///<summary>�û�����</summary>
        public Task<OperationResult<bool>> InsertUserActivityModelAsync(UserApplyActivityModel userActivityModel) => InvokeAsync(_ => _.InsertUserActivityModelAsync(userActivityModel));
		///<summary>����û������</summary>
        public OperationResult<bool> UpdateUserActivityStatusByPKID(UserApplyActivityModel userActivityModel) => Invoke(_ => _.UpdateUserActivityStatusByPKID(userActivityModel));

	///<summary>����û������</summary>
        public Task<OperationResult<bool>> UpdateUserActivityStatusByPKIDAsync(UserApplyActivityModel userActivityModel) => InvokeAsync(_ => _.UpdateUserActivityStatusByPKIDAsync(userActivityModel));
		///<summary>��ҳ��ȡ�û������б�</summary>
        public OperationResult<Tuple<IEnumerable<UserApplyActivityModel>, int>> GetUserApplyActivityModelsPaged(Guid activityId, AuditStatus auditStatus, int pageIndex, int pageSize) => Invoke(_ => _.GetUserApplyActivityModelsPaged(activityId,auditStatus,pageIndex,pageSize));

	///<summary>��ҳ��ȡ�û������б�</summary>
        public Task<OperationResult<Tuple<IEnumerable<UserApplyActivityModel>, int>>> GetUserApplyActivityModelsPagedAsync(Guid activityId, AuditStatus auditStatus, int pageIndex, int pageSize) => InvokeAsync(_ => _.GetUserApplyActivityModelsPagedAsync(activityId,auditStatus,pageIndex,pageSize));
		///<summary>���ݻid��ȡ������Ա����</summary>
        public OperationResult<int> GetActivityApplyUserCountByActivityId(Guid activityId) => Invoke(_ => _.GetActivityApplyUserCountByActivityId(activityId));

	///<summary>���ݻid��ȡ������Ա����</summary>
        public Task<OperationResult<int>> GetActivityApplyUserCountByActivityIdAsync(Guid activityId) => InvokeAsync(_ => _.GetActivityApplyUserCountByActivityIdAsync(activityId));
		///<summary>���ݻid��ȡ������Ա���ͨ������</summary>
        public OperationResult<int> GetActivityApplyUserPassCountByActivityId(Guid activityId) => Invoke(_ => _.GetActivityApplyUserPassCountByActivityId(activityId));

	///<summary>���ݻid��ȡ������Ա���ͨ������</summary>
        public Task<OperationResult<int>> GetActivityApplyUserPassCountByActivityIdAsync(Guid activityId) => InvokeAsync(_ => _.GetActivityApplyUserPassCountByActivityIdAsync(activityId));
		///<summary>����pkid��ȡ������Ա</summary>
        public OperationResult<UserApplyActivityModel> GetUserApplyActivityByPKID(int pkid) => Invoke(_ => _.GetUserApplyActivityByPKID(pkid));

	///<summary>����pkid��ȡ������Ա</summary>
        public Task<OperationResult<UserApplyActivityModel>> GetUserApplyActivityByPKIDAsync(int pkid) => InvokeAsync(_ => _.GetUserApplyActivityByPKIDAsync(pkid));
		///<summary>ɾ���û�����</summary>
        public OperationResult<bool> DeleteUserApplyActivityModelByPKID(int pkid) => Invoke(_ => _.DeleteUserApplyActivityModelByPKID(pkid));

	///<summary>ɾ���û�����</summary>
        public Task<OperationResult<bool>> DeleteUserApplyActivityModelByPKIDAsync(int pkid) => InvokeAsync(_ => _.DeleteUserApplyActivityModelByPKIDAsync(pkid));
		///<summary>����û�������ֻ��š����ƺš���ʻ֤���Ƿ��ظ�</summary>
        public OperationResult<bool> CheckUserApplyActivityInfoIsExist(Guid activityId,string mobile, string carNum, string driverNum) => Invoke(_ => _.CheckUserApplyActivityInfoIsExist(activityId,mobile,carNum,driverNum));

	///<summary>����û�������ֻ��š����ƺš���ʻ֤���Ƿ��ظ�</summary>
        public Task<OperationResult<bool>> CheckUserApplyActivityInfoIsExistAsync(Guid activityId,string mobile, string carNum, string driverNum) => InvokeAsync(_ => _.CheckUserApplyActivityInfoIsExistAsync(activityId,mobile,carNum,driverNum));
		///<summary>���û����</summary>
        public OperationResult<bool> RefreshActivityModelByActivityIdCache(Guid activityId) => Invoke(_ => _.RefreshActivityModelByActivityIdCache(activityId));

	///<summary>���û����</summary>
        public Task<OperationResult<bool>> RefreshActivityModelByActivityIdCacheAsync(Guid activityId) => InvokeAsync(_ => _.RefreshActivityModelByActivityIdCacheAsync(activityId));
		///<summary>�Ƴ������</summary>
        public OperationResult<bool> RemoveActivityModelByActivityIdCache(Guid activityId) => Invoke(_ => _.RemoveActivityModelByActivityIdCache(activityId));

	///<summary>�Ƴ������</summary>
        public Task<OperationResult<bool>> RemoveActivityModelByActivityIdCacheAsync(Guid activityId) => InvokeAsync(_ => _.RemoveActivityModelByActivityIdCacheAsync(activityId));
		///<summary>�����û������SortedSetCache</summary>
        public OperationResult<bool> AddUserApplyActivitySortedSetCache(UserApplyActivityModel userApplyActivityModel) => Invoke(_ => _.AddUserApplyActivitySortedSetCache(userApplyActivityModel));

	///<summary>�����û������SortedSetCache</summary>
        public Task<OperationResult<bool>> AddUserApplyActivitySortedSetCacheAsync(UserApplyActivityModel userApplyActivityModel) => InvokeAsync(_ => _.AddUserApplyActivitySortedSetCacheAsync(userApplyActivityModel));
		///<summary>ɾ��һ���û������SortedSetCache</summary>
        public OperationResult<bool> RemoveOneUserApplyActivitySortedSetCache(UserApplyActivityModel userApplyActivityModel) => Invoke(_ => _.RemoveOneUserApplyActivitySortedSetCache(userApplyActivityModel));

	///<summary>ɾ��һ���û������SortedSetCache</summary>
        public Task<OperationResult<bool>> RemoveOneUserApplyActivitySortedSetCacheAsync(UserApplyActivityModel userApplyActivityModel) => InvokeAsync(_ => _.RemoveOneUserApplyActivitySortedSetCacheAsync(userApplyActivityModel));
		///<summary>��ȡ�û����������</summary>
        public OperationResult<IEnumerable<UserApplyActivityModel>> GetUserApplyActivityRangeByScore() => Invoke(_ => _.GetUserApplyActivityRangeByScore());

	///<summary>��ȡ�û����������</summary>
        public Task<OperationResult<IEnumerable<UserApplyActivityModel>>> GetUserApplyActivityRangeByScoreAsync() => InvokeAsync(_ => _.GetUserApplyActivityRangeByScoreAsync());
		///<summary>��ȡ�û�����������û������SortedSetLength</summary>
        public OperationResult<long> GetUserApplyActivitySortedSetLength() => Invoke(_ => _.GetUserApplyActivitySortedSetLength());

	///<summary>��ȡ�û�����������û������SortedSetLength</summary>
        public Task<OperationResult<long>> GetUserApplyActivitySortedSetLengthAsync() => InvokeAsync(_ => _.GetUserApplyActivitySortedSetLengthAsync());
>>>>>>> cef10ecfe95a62dbe27086896d29d0abab5da3d4
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
		/// <summary> 受邀人获取砍价结果 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfoResponse")]
        Task<OperationResult<InviteeBarginInfo>> GetInviteeBargainInfoAsync(Guid idKey,Guid userId);
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
		/// <summary> 受邀人获取砍价结果 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfoResponse")]
        OperationResult<InviteeBarginInfo> GetInviteeBargainInfo(Guid idKey,Guid userId);
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
		/// <summary> 受邀人获取砍价结果 </summary>
        public OperationResult<InviteeBarginInfo> GetInviteeBargainInfo(Guid idKey,Guid userId) => Invoke(_ => _.GetInviteeBargainInfo(idKey, userId));

	/// <summary> 受邀人获取砍价结果 </summary>
        public Task<OperationResult<InviteeBarginInfo>> GetInviteeBargainInfoAsync(Guid idKey,Guid userId) => InvokeAsync(_ => _.GetInviteeBargainInfoAsync(idKey, userId));
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
}
