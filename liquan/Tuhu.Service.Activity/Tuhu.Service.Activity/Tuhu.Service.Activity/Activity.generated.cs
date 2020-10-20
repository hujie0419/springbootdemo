
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
//��Ҫ��Activity.generated.cs�ļ�����κδ��룬���ļ�����Ϊ�Զ����ɡ���Ҫ�ӽӿ�����Activity.tt��Activity.cs�����
namespace Tuhu.Service.Activity
{
	///<summary>����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IBigBrandService
    {
    	///<summary>��ȡ����������Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetBigBrand", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetBigBrandResponse")]
        Task<OperationResult<BigBrandRewardListModel>> GetBigBrandAsync(string keyValue);
		///<summary>���´���������Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrand", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandResponse")]
        Task<OperationResult<bool>> UpdateBigBrandAsync(string keyValue);
		///<summary>��ȡ�齱���</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetPacketResponse")]
        Task<OperationResult<BigBrandResponse>> GetPacketAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string));
		///<summary>����׷�ӳ齱����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddOne", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddOneResponse")]
        Task<OperationResult<bool>> ShareAddOneAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string),int chanceType=2);
		///<summary>�齱����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectCanPacker", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectCanPackerResponse")]
        Task<OperationResult<BigBrandCanResponse>> SelectCanPackerAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string));
		///<summary>��ȡ���̳齱��Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectPack", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectPackResponse")]
        Task<OperationResult<string>> SelectPackAsync(string hashKey);
		///<summary>����ʵ�ｱ����ַ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandRealLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandRealLogResponse")]
        Task<OperationResult<BigBrandRealResponse>> UpdateBigBrandRealLogAsync(string hashKey,Guid userId,string address, Guid tip,string phone,string deviceId,string Channal,string userName);
		///<summary>��ѯʵ�ｱ���ĸ��µ�ַ�Ƿ���δ��д</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/IsNULLBigBrandRealByAddress", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/IsNULLBigBrandRealByAddressResponse")]
        Task<OperationResult<IEnumerable<BigBrandRealLogModel>>> IsNULLBigBrandRealByAddressAsync(string hashKey,Guid userId,string phone,string deviceId,string Channal);
		///<summary>����״̬���ӳ齱����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddByOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddByOrderResponse")]
        Task<OperationResult<bool>> ShareAddByOrderAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times);
		///<summary>��������ӳ齱����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/AddBigBrandTimes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/AddBigBrandTimesResponse")]
        Task<OperationResult<bool>> AddBigBrandTimesAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times);
		///<summary>�ʴ�齱���ؽ��</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetAnswerPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetAnswerPacketResponse")]
        Task<OperationResult<BigBrandResponse>> GetAnswerPacketAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer);
		///<summary>�ʴ����������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SetAnswerRes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SetAnswerResResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Models.Response.QuestionAnsResponse>> SetAnswerResAsync(Tuhu.Service.Activity.Models.Requests.QuestionAnsRequestModel request);
		///<summary>��ȡ�����б�</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetQuestionList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetQuestionListResponse")]
        Task<OperationResult<List<Tuhu.Service.Activity.Models.BigBrandQuesList>>> GetQuestionListAsync(Guid userId, string hashKey );
		///<summary>ˢ���ʴ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateQuestionInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateQuestionInfoListResponse")]
        Task<OperationResult<bool>> UpdateQuestionInfoListAsync();
		///<summary>��ȡ���������б�</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetFightGroupsPacketsList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetFightGroupsPacketsListResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> GetFightGroupsPacketsListAsync(Guid? fightGroupsIdentity,Guid userId);
		///<summary>������������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/InsertFightGroupsPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/InsertFightGroupsPacketResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> InsertFightGroupsPacketAsync(Guid userId);
		///<summary>���·��������е��û�</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateFightGroupsPacketByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateFightGroupsPacketByUserIdResponse")]
        Task<OperationResult<bool>> UpdateFightGroupsPacketByUserIdAsync(Tuhu.Service.Activity.Models.Requests.FightGroupsPacketsUpdateRequest request);
		///<summary>�����Ż�ȯ���</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/CreateFightGroupsPacketByPromotion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/CreateFightGroupsPacketByPromotionResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupsPacketsProvideResponse>> CreateFightGroupsPacketByPromotionAsync(Guid fightGroupsIdentity);
		///<summary>��ȡ����������־�б� chanceType:1.�н���¼��2.�����¼��3.����������¼</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectShareList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectShareListResponse")]
        Task<OperationResult<List<BigBrandRewardLogModel>>> SelectShareListAsync(Guid userId,string hashKey,int chanceType);
	}

	///<summary>����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IBigBrandClient : IBigBrandService, ITuhuServiceClient
    {
    	///<summary>��ȡ����������Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetBigBrand", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetBigBrandResponse")]
        OperationResult<BigBrandRewardListModel> GetBigBrand(string keyValue);
		///<summary>���´���������Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrand", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandResponse")]
        OperationResult<bool> UpdateBigBrand(string keyValue);
		///<summary>��ȡ�齱���</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetPacketResponse")]
        OperationResult<BigBrandResponse> GetPacket(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string));
		///<summary>����׷�ӳ齱����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddOne", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddOneResponse")]
        OperationResult<bool> ShareAddOne(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string),int chanceType=2);
		///<summary>�齱����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectCanPacker", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectCanPackerResponse")]
        OperationResult<BigBrandCanResponse> SelectCanPacker(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string));
		///<summary>��ȡ���̳齱��Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectPack", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectPackResponse")]
        OperationResult<string> SelectPack(string hashKey);
		///<summary>����ʵ�ｱ����ַ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandRealLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateBigBrandRealLogResponse")]
        OperationResult<BigBrandRealResponse> UpdateBigBrandRealLog(string hashKey,Guid userId,string address, Guid tip,string phone,string deviceId,string Channal,string userName);
		///<summary>��ѯʵ�ｱ���ĸ��µ�ַ�Ƿ���δ��д</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/IsNULLBigBrandRealByAddress", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/IsNULLBigBrandRealByAddressResponse")]
        OperationResult<IEnumerable<BigBrandRealLogModel>> IsNULLBigBrandRealByAddress(string hashKey,Guid userId,string phone,string deviceId,string Channal);
		///<summary>����״̬���ӳ齱����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddByOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/ShareAddByOrderResponse")]
        OperationResult<bool> ShareAddByOrder(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times);
		///<summary>��������ӳ齱����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/AddBigBrandTimes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/AddBigBrandTimesResponse")]
        OperationResult<bool> AddBigBrandTimes(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times);
		///<summary>�ʴ�齱���ؽ��</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetAnswerPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetAnswerPacketResponse")]
        OperationResult<BigBrandResponse> GetAnswerPacket(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer);
		///<summary>�ʴ����������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SetAnswerRes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SetAnswerResResponse")]
        OperationResult<Tuhu.Service.Activity.Models.Response.QuestionAnsResponse> SetAnswerRes(Tuhu.Service.Activity.Models.Requests.QuestionAnsRequestModel request);
		///<summary>��ȡ�����б�</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetQuestionList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetQuestionListResponse")]
        OperationResult<List<Tuhu.Service.Activity.Models.BigBrandQuesList>> GetQuestionList(Guid userId, string hashKey );
		///<summary>ˢ���ʴ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateQuestionInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateQuestionInfoListResponse")]
        OperationResult<bool> UpdateQuestionInfoList();
		///<summary>��ȡ���������б�</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetFightGroupsPacketsList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/GetFightGroupsPacketsListResponse")]
        OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> GetFightGroupsPacketsList(Guid? fightGroupsIdentity,Guid userId);
		///<summary>������������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/InsertFightGroupsPacket", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/InsertFightGroupsPacketResponse")]
        OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> InsertFightGroupsPacket(Guid userId);
		///<summary>���·��������е��û�</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateFightGroupsPacketByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/UpdateFightGroupsPacketByUserIdResponse")]
        OperationResult<bool> UpdateFightGroupsPacketByUserId(Tuhu.Service.Activity.Models.Requests.FightGroupsPacketsUpdateRequest request);
		///<summary>�����Ż�ȯ���</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/CreateFightGroupsPacketByPromotion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/CreateFightGroupsPacketByPromotionResponse")]
        OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupsPacketsProvideResponse> CreateFightGroupsPacketByPromotion(Guid fightGroupsIdentity);
		///<summary>��ȡ����������־�б� chanceType:1.�н���¼��2.�����¼��3.����������¼</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectShareList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/BigBrand/SelectShareListResponse")]
        OperationResult<List<BigBrandRewardLogModel>> SelectShareList(Guid userId,string hashKey,int chanceType);
	}

	///<summary>����</summary>
	public partial class BigBrandClient : TuhuServiceClient<IBigBrandClient>, IBigBrandClient
    {
    	///<summary>��ȡ����������Ϣ</summary>///<returns></returns>
        public OperationResult<BigBrandRewardListModel> GetBigBrand(string keyValue) => Invoke(_ => _.GetBigBrand(keyValue));

	///<summary>��ȡ����������Ϣ</summary>///<returns></returns>
        public Task<OperationResult<BigBrandRewardListModel>> GetBigBrandAsync(string keyValue) => InvokeAsync(_ => _.GetBigBrandAsync(keyValue));
		///<summary>���´���������Ϣ</summary>///<returns></returns>
        public OperationResult<bool> UpdateBigBrand(string keyValue) => Invoke(_ => _.UpdateBigBrand(keyValue));

	///<summary>���´���������Ϣ</summary>///<returns></returns>
        public Task<OperationResult<bool>> UpdateBigBrandAsync(string keyValue) => InvokeAsync(_ => _.UpdateBigBrandAsync(keyValue));
		///<summary>��ȡ�齱���</summary>///<returns></returns>
        public OperationResult<BigBrandResponse> GetPacket(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string)) => Invoke(_ => _.GetPacket(userId,deviceId,Channal,hashKey,phone,refer,openId));

	///<summary>��ȡ�齱���</summary>///<returns></returns>
        public Task<OperationResult<BigBrandResponse>> GetPacketAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string)) => InvokeAsync(_ => _.GetPacketAsync(userId,deviceId,Channal,hashKey,phone,refer,openId));
		///<summary>����׷�ӳ齱����</summary>///<returns></returns>
        public OperationResult<bool> ShareAddOne(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string),int chanceType=2) => Invoke(_ => _.ShareAddOne(userId,deviceId,Channal,hashKey,phone,refer,openId,chanceType));

	///<summary>����׷�ӳ齱����</summary>///<returns></returns>
        public Task<OperationResult<bool>> ShareAddOneAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string),int chanceType=2) => InvokeAsync(_ => _.ShareAddOneAsync(userId,deviceId,Channal,hashKey,phone,refer,openId,chanceType));
		///<summary>�齱����</summary>///<returns></returns>
        public OperationResult<BigBrandCanResponse> SelectCanPacker(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string)) => Invoke(_ => _.SelectCanPacker(userId,deviceId,Channal,hashKey,phone,refer,openId));

	///<summary>�齱����</summary>///<returns></returns>
        public Task<OperationResult<BigBrandCanResponse>> SelectCanPackerAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,string openId=default(string)) => InvokeAsync(_ => _.SelectCanPackerAsync(userId,deviceId,Channal,hashKey,phone,refer,openId));
		///<summary>��ȡ���̳齱��Ϣ</summary>///<returns></returns>
        public OperationResult<string> SelectPack(string hashKey) => Invoke(_ => _.SelectPack(hashKey));

	///<summary>��ȡ���̳齱��Ϣ</summary>///<returns></returns>
        public Task<OperationResult<string>> SelectPackAsync(string hashKey) => InvokeAsync(_ => _.SelectPackAsync(hashKey));
		///<summary>����ʵ�ｱ����ַ</summary>///<returns></returns>
        public OperationResult<BigBrandRealResponse> UpdateBigBrandRealLog(string hashKey,Guid userId,string address, Guid tip,string phone,string deviceId,string Channal,string userName) => Invoke(_ => _.UpdateBigBrandRealLog(hashKey,userId,address,tip,phone,deviceId,Channal,userName));

	///<summary>����ʵ�ｱ����ַ</summary>///<returns></returns>
        public Task<OperationResult<BigBrandRealResponse>> UpdateBigBrandRealLogAsync(string hashKey,Guid userId,string address, Guid tip,string phone,string deviceId,string Channal,string userName) => InvokeAsync(_ => _.UpdateBigBrandRealLogAsync(hashKey,userId,address,tip,phone,deviceId,Channal,userName));
		///<summary>��ѯʵ�ｱ���ĸ��µ�ַ�Ƿ���δ��д</summary>///<returns></returns>
        public OperationResult<IEnumerable<BigBrandRealLogModel>> IsNULLBigBrandRealByAddress(string hashKey,Guid userId,string phone,string deviceId,string Channal) => Invoke(_ => _.IsNULLBigBrandRealByAddress(hashKey,userId,phone,deviceId,Channal));

	///<summary>��ѯʵ�ｱ���ĸ��µ�ַ�Ƿ���δ��д</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<BigBrandRealLogModel>>> IsNULLBigBrandRealByAddressAsync(string hashKey,Guid userId,string phone,string deviceId,string Channal) => InvokeAsync(_ => _.IsNULLBigBrandRealByAddressAsync(hashKey,userId,phone,deviceId,Channal));
		///<summary>����״̬���ӳ齱����</summary>///<returns></returns>
        public OperationResult<bool> ShareAddByOrder(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times) => Invoke(_ => _.ShareAddByOrder(userId,deviceId,Channal,hashKey,phone,refer,times));

	///<summary>����״̬���ӳ齱����</summary>///<returns></returns>
        public Task<OperationResult<bool>> ShareAddByOrderAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times) => InvokeAsync(_ => _.ShareAddByOrderAsync(userId,deviceId,Channal,hashKey,phone,refer,times));
		///<summary>��������ӳ齱����</summary>///<returns></returns>
        public OperationResult<bool> AddBigBrandTimes(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times) => Invoke(_ => _.AddBigBrandTimes(userId,deviceId,Channal,hashKey,phone,refer,times));

	///<summary>��������ӳ齱����</summary>///<returns></returns>
        public Task<OperationResult<bool>> AddBigBrandTimesAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer,int times) => InvokeAsync(_ => _.AddBigBrandTimesAsync(userId,deviceId,Channal,hashKey,phone,refer,times));
		///<summary>�ʴ�齱���ؽ��</summary>///<returns></returns>
        public OperationResult<BigBrandResponse> GetAnswerPacket(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer) => Invoke(_ => _.GetAnswerPacket(userId,deviceId,Channal,hashKey,phone,refer));

	///<summary>�ʴ�齱���ؽ��</summary>///<returns></returns>
        public Task<OperationResult<BigBrandResponse>> GetAnswerPacketAsync(Guid userId, string deviceId, string Channal, string hashKey,string phone,string refer) => InvokeAsync(_ => _.GetAnswerPacketAsync(userId,deviceId,Channal,hashKey,phone,refer));
		///<summary>�ʴ����������</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Models.Response.QuestionAnsResponse> SetAnswerRes(Tuhu.Service.Activity.Models.Requests.QuestionAnsRequestModel request) => Invoke(_ => _.SetAnswerRes(request));

	///<summary>�ʴ����������</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Models.Response.QuestionAnsResponse>> SetAnswerResAsync(Tuhu.Service.Activity.Models.Requests.QuestionAnsRequestModel request) => InvokeAsync(_ => _.SetAnswerResAsync(request));
		///<summary>��ȡ�����б�</summary>///<returns></returns>
        public OperationResult<List<Tuhu.Service.Activity.Models.BigBrandQuesList>> GetQuestionList(Guid userId, string hashKey ) => Invoke(_ => _.GetQuestionList(userId,hashKey));

	///<summary>��ȡ�����б�</summary>///<returns></returns>
        public Task<OperationResult<List<Tuhu.Service.Activity.Models.BigBrandQuesList>>> GetQuestionListAsync(Guid userId, string hashKey ) => InvokeAsync(_ => _.GetQuestionListAsync(userId,hashKey));
		///<summary>ˢ���ʴ����</summary>///<returns></returns>
        public OperationResult<bool> UpdateQuestionInfoList() => Invoke(_ => _.UpdateQuestionInfoList());

	///<summary>ˢ���ʴ����</summary>///<returns></returns>
        public Task<OperationResult<bool>> UpdateQuestionInfoListAsync() => InvokeAsync(_ => _.UpdateQuestionInfoListAsync());
		///<summary>��ȡ���������б�</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> GetFightGroupsPacketsList(Guid? fightGroupsIdentity,Guid userId) => Invoke(_ => _.GetFightGroupsPacketsList(fightGroupsIdentity,userId));

	///<summary>��ȡ���������б�</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> GetFightGroupsPacketsListAsync(Guid? fightGroupsIdentity,Guid userId) => InvokeAsync(_ => _.GetFightGroupsPacketsListAsync(fightGroupsIdentity,userId));
		///<summary>������������</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse> InsertFightGroupsPacket(Guid userId) => Invoke(_ => _.InsertFightGroupsPacket(userId));

	///<summary>������������</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupPacketListResponse>> InsertFightGroupsPacketAsync(Guid userId) => InvokeAsync(_ => _.InsertFightGroupsPacketAsync(userId));
		///<summary>���·��������е��û�</summary>///<returns></returns>
        public OperationResult<bool> UpdateFightGroupsPacketByUserId(Tuhu.Service.Activity.Models.Requests.FightGroupsPacketsUpdateRequest request) => Invoke(_ => _.UpdateFightGroupsPacketByUserId(request));

	///<summary>���·��������е��û�</summary>///<returns></returns>
        public Task<OperationResult<bool>> UpdateFightGroupsPacketByUserIdAsync(Tuhu.Service.Activity.Models.Requests.FightGroupsPacketsUpdateRequest request) => InvokeAsync(_ => _.UpdateFightGroupsPacketByUserIdAsync(request));
		///<summary>�����Ż�ȯ���</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupsPacketsProvideResponse> CreateFightGroupsPacketByPromotion(Guid fightGroupsIdentity) => Invoke(_ => _.CreateFightGroupsPacketByPromotion(fightGroupsIdentity));

	///<summary>�����Ż�ȯ���</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Models.Response.FightGroupsPacketsProvideResponse>> CreateFightGroupsPacketByPromotionAsync(Guid fightGroupsIdentity) => InvokeAsync(_ => _.CreateFightGroupsPacketByPromotionAsync(fightGroupsIdentity));
		///<summary>��ȡ����������־�б� chanceType:1.�н���¼��2.�����¼��3.����������¼</summary>///<returns></returns>
        public OperationResult<List<BigBrandRewardLogModel>> SelectShareList(Guid userId,string hashKey,int chanceType) => Invoke(_ => _.SelectShareList(userId,hashKey,chanceType));

	///<summary>��ȡ����������־�б� chanceType:1.�н���¼��2.�����¼��3.����������¼</summary>///<returns></returns>
        public Task<OperationResult<List<BigBrandRewardLogModel>>> SelectShareListAsync(Guid userId,string hashKey,int chanceType) => InvokeAsync(_ => _.SelectShareListAsync(userId,hashKey,chanceType));
	}
	/// <summary>��ʱ����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleService
    {
    	/// <summary>������ʱ�������ݵ�����</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityIDResponse")]
        Task<OperationResult<bool>> UpdateFlashSaleDataToCouchBaseByActivityIDAsync(Guid activityID);
		/// <summary>������ɱ�����б���ˢ��</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SpikeListRefresh", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SpikeListRefreshResponse")]
        Task<OperationResult<bool>> SpikeListRefreshAsync(Guid activityId);
		/// <summary>���ݻID��ѯ�����</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIDResponse")]
        Task<OperationResult<FlashSaleModel>> SelectFlashSaleDataByActivityIDAsync(Guid activityID);
		/// <summary>���ݻID,Pid��ѯ������붨����Ʒ</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIdsResponse")]
        Task<OperationResult<List<FlashSaleModel>>> SelectFlashSaleDataByActivityIdsAsync(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests);
		/// <summary>�»ҳ��ѯ���Ϣ�ӿ�</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleDataActivityPageByIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleDataActivityPageByIdsResponse")]
        Task<OperationResult<List<FlashSaleActivityPageModel>>> GetFlashSaleDataActivityPageByIdsAsync(List<FlashSaleActivityPageRequest> flashSaleActivityPageRequest);
		/// <summary>��ȡ��ʱ�������������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleListResponse")]
        Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleListAsync(Guid[] activityIDs);
		/// <summary>�����������ɱ��������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayDataResponse")]
        Task<OperationResult<IEnumerable<FlashSaleModel>>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate=null,bool needProducts=true,bool excludeProductTags = false);
		/// <summary>����ʱ�䷶Χ��ѯ��ɱ������Ϣ</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSecondsBoys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSecondsBoysResponse")]
        Task<OperationResult<IEnumerable<FlashSaleModel>>> GetSecondsBoysAsync(int activityType,DateTime? startDate=null,DateTime? endDate=null);
		/// <summary>�ҳ��ɱ��ѯ���³���</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetActivePageSecondKill", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetActivePageSecondKillResponse")]
        Task<OperationResult<IEnumerable<FlashSaleModel>>> GetActivePageSecondKillAsync(int topNumber,bool isProducts=true);
		/// <summary>����ȡ����������ʱ����������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DeleteFlashSaleRecords", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DeleteFlashSaleRecordsResponse")]
        Task<OperationResult<int>> DeleteFlashSaleRecordsAsync(int orderId);
		/// <summary>��ȡ������ɱ�û�������Ϣ</summary>/// <returns>null</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserReminderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserReminderInfoResponse")]
        Task<OperationResult<UserReminderInfo>> GetUserReminderInfoAsync(EveryDaySeckillUserInfo model);
		/// <summary>����������ɱ�û���Ϣ</summary>/// <returns>null</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/InsertEveryDaySeckillUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/InsertEveryDaySeckillUserInfoResponse")]
        Task<OperationResult<InsertEveryDaySeckillUserInfoResponse>> InsertEveryDaySeckillUserInfoAsync(EveryDaySeckillUserInfo model);
		/// <summary>У����ʱ������Ʒ�Ƿ��޹�</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckFlashSaleProductBuyLimit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckFlashSaleProductBuyLimitResponse")]
        Task<OperationResult<IEnumerable<FlashSaleProductBuyLimitModel>>> CheckFlashSaleProductBuyLimitAsync(CheckFlashSaleProductRequest request);
		/// <summary>��ȡ�û������Թ���˻��Ʒ����</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCanBuyFlashSaleItemCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCanBuyFlashSaleItemCountResponse")]
        Task<OperationResult<FlashSaleProductCanBuyCountModel>> GetUserCanBuyFlashSaleItemCountAsync(Guid userId,Guid activityId,string pid);
		/// <summary>У���Ƿ�ɹ��������ʱ��������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckCanBuyFlashSaleOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckCanBuyFlashSaleOrderResponse")]
        Task<OperationResult<FlashSaleOrderResponse>> CheckCanBuyFlashSaleOrderAsync(FlashSaleOrderRequest request);
		/// <summary>/// ��ѯ��Ʒ����ҳ��ʱ��������/// </summary>///  <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailForFlashSale", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailForFlashSaleResponse")]
        Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailForFlashSaleAsync(string productId, string variantId, string activityId,string channel, string userId,string productGroupId=null,int buyQty=1);
		///<summary>���ټ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DecrementCounter", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DecrementCounterResponse")]
        Task<OperationResult<bool>> DecrementCounterAsync(int orderId);
		///<summary>ȡ������ʱά������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCancerMaintenanceFlashSaleData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCancerMaintenanceFlashSaleDataResponse")]
        Task<OperationResult<bool>> OrderCancerMaintenanceFlashSaleDataAsync(int orderId);
		///<summary>ˢ���޹���Ʒ����������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshFlashSaleHashCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshFlashSaleHashCountResponse")]
        Task<OperationResult<bool>> RefreshFlashSaleHashCountAsync(List<string> activtyids ,bool isAllRefresh);
		///<summary>ֻ��ѯ�������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleWithoutProductsList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleWithoutProductsListResponse")]
        Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleWithoutProductsListAsync(List<Guid> activtyids);
		///<summary>��ѯ����ҳ������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailDisountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailDisountInfoResponse")]
        Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailDisountInfoAsync(DiscountActivtyProductRequest request);
		///<summary>��ѯ�û��µ���ά�����޹�������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCreateFlashOrderCountCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCreateFlashOrderCountCacheResponse")]
        Task<OperationResult<OrderCountResponse>> GetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request);
		///<summary>�����û��µ���ά�����޹�������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SetUserCreateFlashOrderCountCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SetUserCreateFlashOrderCountCacheResponse")]
        Task<OperationResult<OrderCountResponse>> SetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request);
		///<summary>��ȡ���������ݲ�׼ȷ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleWrongCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleWrongCacheResponse")]
        Task<OperationResult<List<FlashSaleWrongCacheResponse>>> SelectFlashSaleWrongCacheAsync();
		///<summary>�û���������ʱ��ά����������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCreateMaintenanceFlashSaleDbData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCreateMaintenanceFlashSaleDbDataResponse")]
        Task<OperationResult<bool>> OrderCreateMaintenanceFlashSaleDbDataAsync(FlashSaleOrderRequest flashSale);
		///<summary>����־����������������ñ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateConfigSaleoutQuantityFromLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateConfigSaleoutQuantityFromLogResponse")]
        Task<OperationResult<bool>> UpdateConfigSaleoutQuantityFromLogAsync(UpdateConfigSaleoutQuantityRequest request);
		///<summary>���ճ���ˢ����ɱĬ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshSeckillDefaultDataBySchedule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshSeckillDefaultDataByScheduleResponse")]
        Task<OperationResult<bool>> RefreshSeckillDefaultDataByScheduleAsync(string schedule);
		///<summary>��ȡ����ʱ����ڵ���ɱ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillScheduleInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillScheduleInfoResponse")]
        Task<OperationResult<Dictionary<string, List<SeckilScheduleInfoRespnose>>>> GetSeckillScheduleInfoAsync(List<string> pids, DateTime sSchedule, DateTime eSchedule);
		///<summary>���ݻ��Ʒ��ȡ�û��Ʒ�Ŀ����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillAvailableStockResponse", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillAvailableStockResponseResponse")]
        Task<OperationResult<List<SeckillAvailableStockInfoResponse>>> GetSeckillAvailableStockResponseAsync(List<SeckillAvailableStockInfoRequest> request);
		///<summary>��ѯ��ҳ������ɱ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectHomeSeckillData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectHomeSeckillDataResponse")]
        Task<OperationResult<List<FlashSaleModel>>> SelectHomeSeckillDataAsync(SelectHomeSecKillRequest request);
		///<summary>���ݳ���Id��ȡ������ɱ��Ʒ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSeckillDataByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSeckillDataByActivityIdResponse")]
        Task<OperationResult<List<FlashSaleProductModel>>> SelectSeckillDataByActivityIdAsync(SelectSecKillByIdRequest request);
	}

	/// <summary>��ʱ����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleClient : IFlashSaleService, ITuhuServiceClient
    {
    	/// <summary>������ʱ�������ݵ�����</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityIDResponse")]
        OperationResult<bool> UpdateFlashSaleDataToCouchBaseByActivityID(Guid activityID);
		/// <summary>������ɱ�����б���ˢ��</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SpikeListRefresh", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SpikeListRefreshResponse")]
        OperationResult<bool> SpikeListRefresh(Guid activityId);
		/// <summary>���ݻID��ѯ�����</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIDResponse")]
        OperationResult<FlashSaleModel> SelectFlashSaleDataByActivityID(Guid activityID);
		/// <summary>���ݻID,Pid��ѯ������붨����Ʒ</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIdsResponse")]
        OperationResult<List<FlashSaleModel>> SelectFlashSaleDataByActivityIds(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests);
		/// <summary>�»ҳ��ѯ���Ϣ�ӿ�</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleDataActivityPageByIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleDataActivityPageByIdsResponse")]
        OperationResult<List<FlashSaleActivityPageModel>> GetFlashSaleDataActivityPageByIds(List<FlashSaleActivityPageRequest> flashSaleActivityPageRequest);
		/// <summary>��ȡ��ʱ�������������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleListResponse")]
        OperationResult<List<FlashSaleModel>> GetFlashSaleList(Guid[] activityIDs);
		/// <summary>�����������ɱ��������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayDataResponse")]
        OperationResult<IEnumerable<FlashSaleModel>> SelectSecondKillTodayData(int activityType, DateTime? scheduleDate=null,bool needProducts=true,bool excludeProductTags = false);
		/// <summary>����ʱ�䷶Χ��ѯ��ɱ������Ϣ</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSecondsBoys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSecondsBoysResponse")]
        OperationResult<IEnumerable<FlashSaleModel>> GetSecondsBoys(int activityType,DateTime? startDate=null,DateTime? endDate=null);
		/// <summary>�ҳ��ɱ��ѯ���³���</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetActivePageSecondKill", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetActivePageSecondKillResponse")]
        OperationResult<IEnumerable<FlashSaleModel>> GetActivePageSecondKill(int topNumber,bool isProducts=true);
		/// <summary>����ȡ����������ʱ����������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DeleteFlashSaleRecords", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DeleteFlashSaleRecordsResponse")]
        OperationResult<int> DeleteFlashSaleRecords(int orderId);
		/// <summary>��ȡ������ɱ�û�������Ϣ</summary>/// <returns>null</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserReminderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserReminderInfoResponse")]
        OperationResult<UserReminderInfo> GetUserReminderInfo(EveryDaySeckillUserInfo model);
		/// <summary>����������ɱ�û���Ϣ</summary>/// <returns>null</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/InsertEveryDaySeckillUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/InsertEveryDaySeckillUserInfoResponse")]
        OperationResult<InsertEveryDaySeckillUserInfoResponse> InsertEveryDaySeckillUserInfo(EveryDaySeckillUserInfo model);
		/// <summary>У����ʱ������Ʒ�Ƿ��޹�</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckFlashSaleProductBuyLimit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckFlashSaleProductBuyLimitResponse")]
        OperationResult<IEnumerable<FlashSaleProductBuyLimitModel>> CheckFlashSaleProductBuyLimit(CheckFlashSaleProductRequest request);
		/// <summary>��ȡ�û������Թ���˻��Ʒ����</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCanBuyFlashSaleItemCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCanBuyFlashSaleItemCountResponse")]
        OperationResult<FlashSaleProductCanBuyCountModel> GetUserCanBuyFlashSaleItemCount(Guid userId,Guid activityId,string pid);
		/// <summary>У���Ƿ�ɹ��������ʱ��������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckCanBuyFlashSaleOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/CheckCanBuyFlashSaleOrderResponse")]
        OperationResult<FlashSaleOrderResponse> CheckCanBuyFlashSaleOrder(FlashSaleOrderRequest request);
		/// <summary>/// ��ѯ��Ʒ����ҳ��ʱ��������/// </summary>///  <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailForFlashSale", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailForFlashSaleResponse")]
        OperationResult<FlashSaleProductDetailModel> FetchProductDetailForFlashSale(string productId, string variantId, string activityId,string channel, string userId,string productGroupId=null,int buyQty=1);
		///<summary>���ټ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DecrementCounter", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/DecrementCounterResponse")]
        OperationResult<bool> DecrementCounter(int orderId);
		///<summary>ȡ������ʱά������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCancerMaintenanceFlashSaleData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCancerMaintenanceFlashSaleDataResponse")]
        OperationResult<bool> OrderCancerMaintenanceFlashSaleData(int orderId);
		///<summary>ˢ���޹���Ʒ����������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshFlashSaleHashCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshFlashSaleHashCountResponse")]
        OperationResult<bool> RefreshFlashSaleHashCount(List<string> activtyids ,bool isAllRefresh);
		///<summary>ֻ��ѯ�������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleWithoutProductsList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleWithoutProductsListResponse")]
        OperationResult<List<FlashSaleModel>> GetFlashSaleWithoutProductsList(List<Guid> activtyids);
		///<summary>��ѯ����ҳ������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailDisountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/FetchProductDetailDisountInfoResponse")]
        OperationResult<FlashSaleProductDetailModel> FetchProductDetailDisountInfo(DiscountActivtyProductRequest request);
		///<summary>��ѯ�û��µ���ά�����޹�������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCreateFlashOrderCountCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetUserCreateFlashOrderCountCacheResponse")]
        OperationResult<OrderCountResponse> GetUserCreateFlashOrderCountCache(OrderCountCacheRequest request);
		///<summary>�����û��µ���ά�����޹�������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SetUserCreateFlashOrderCountCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SetUserCreateFlashOrderCountCacheResponse")]
        OperationResult<OrderCountResponse> SetUserCreateFlashOrderCountCache(OrderCountCacheRequest request);
		///<summary>��ȡ���������ݲ�׼ȷ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleWrongCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleWrongCacheResponse")]
        OperationResult<List<FlashSaleWrongCacheResponse>> SelectFlashSaleWrongCache();
		///<summary>�û���������ʱ��ά����������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCreateMaintenanceFlashSaleDbData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/OrderCreateMaintenanceFlashSaleDbDataResponse")]
        OperationResult<bool> OrderCreateMaintenanceFlashSaleDbData(FlashSaleOrderRequest flashSale);
		///<summary>����־����������������ñ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateConfigSaleoutQuantityFromLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateConfigSaleoutQuantityFromLogResponse")]
        OperationResult<bool> UpdateConfigSaleoutQuantityFromLog(UpdateConfigSaleoutQuantityRequest request);
		///<summary>���ճ���ˢ����ɱĬ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshSeckillDefaultDataBySchedule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/RefreshSeckillDefaultDataByScheduleResponse")]
        OperationResult<bool> RefreshSeckillDefaultDataBySchedule(string schedule);
		///<summary>��ȡ����ʱ����ڵ���ɱ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillScheduleInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillScheduleInfoResponse")]
        OperationResult<Dictionary<string, List<SeckilScheduleInfoRespnose>>> GetSeckillScheduleInfo(List<string> pids, DateTime sSchedule, DateTime eSchedule);
		///<summary>���ݻ��Ʒ��ȡ�û��Ʒ�Ŀ����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillAvailableStockResponse", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetSeckillAvailableStockResponseResponse")]
        OperationResult<List<SeckillAvailableStockInfoResponse>> GetSeckillAvailableStockResponse(List<SeckillAvailableStockInfoRequest> request);
		///<summary>��ѯ��ҳ������ɱ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectHomeSeckillData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectHomeSeckillDataResponse")]
        OperationResult<List<FlashSaleModel>> SelectHomeSeckillData(SelectHomeSecKillRequest request);
		///<summary>���ݳ���Id��ȡ������ɱ��Ʒ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSeckillDataByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSeckillDataByActivityIdResponse")]
        OperationResult<List<FlashSaleProductModel>> SelectSeckillDataByActivityId(SelectSecKillByIdRequest request);
	}

	/// <summary>��ʱ����</summary>
	public partial class FlashSaleClient : TuhuServiceClient<IFlashSaleClient>, IFlashSaleClient
    {
    	/// <summary>������ʱ�������ݵ�����</summary>
        /// <returns></returns>
        public OperationResult<bool> UpdateFlashSaleDataToCouchBaseByActivityID(Guid activityID) => Invoke(_ => _.UpdateFlashSaleDataToCouchBaseByActivityID(activityID));

	/// <summary>������ʱ�������ݵ�����</summary>
        /// <returns></returns>
        public Task<OperationResult<bool>> UpdateFlashSaleDataToCouchBaseByActivityIDAsync(Guid activityID) => InvokeAsync(_ => _.UpdateFlashSaleDataToCouchBaseByActivityIDAsync(activityID));
		/// <summary>������ɱ�����б���ˢ��</summary>
        /// <returns></returns>
        public OperationResult<bool> SpikeListRefresh(Guid activityId) => Invoke(_ => _.SpikeListRefresh(activityId));

	/// <summary>������ɱ�����б���ˢ��</summary>
        /// <returns></returns>
        public Task<OperationResult<bool>> SpikeListRefreshAsync(Guid activityId) => InvokeAsync(_ => _.SpikeListRefreshAsync(activityId));
		/// <summary>���ݻID��ѯ�����</summary>
        /// <returns></returns>
        public OperationResult<FlashSaleModel> SelectFlashSaleDataByActivityID(Guid activityID) => Invoke(_ => _.SelectFlashSaleDataByActivityID(activityID));

	/// <summary>���ݻID��ѯ�����</summary>
        /// <returns></returns>
        public Task<OperationResult<FlashSaleModel>> SelectFlashSaleDataByActivityIDAsync(Guid activityID) => InvokeAsync(_ => _.SelectFlashSaleDataByActivityIDAsync(activityID));
		/// <summary>���ݻID,Pid��ѯ������붨����Ʒ</summary>
        /// <returns></returns>
        public OperationResult<List<FlashSaleModel>> SelectFlashSaleDataByActivityIds(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests) => Invoke(_ => _.SelectFlashSaleDataByActivityIds(flashSaleDataByActivityRequests));

	/// <summary>���ݻID,Pid��ѯ������붨����Ʒ</summary>
        /// <returns></returns>
        public Task<OperationResult<List<FlashSaleModel>>> SelectFlashSaleDataByActivityIdsAsync(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests) => InvokeAsync(_ => _.SelectFlashSaleDataByActivityIdsAsync(flashSaleDataByActivityRequests));
		/// <summary>�»ҳ��ѯ���Ϣ�ӿ�</summary>
        /// <returns></returns>
        public OperationResult<List<FlashSaleActivityPageModel>> GetFlashSaleDataActivityPageByIds(List<FlashSaleActivityPageRequest> flashSaleActivityPageRequest) => Invoke(_ => _.GetFlashSaleDataActivityPageByIds(flashSaleActivityPageRequest));

	/// <summary>�»ҳ��ѯ���Ϣ�ӿ�</summary>
        /// <returns></returns>
        public Task<OperationResult<List<FlashSaleActivityPageModel>>> GetFlashSaleDataActivityPageByIdsAsync(List<FlashSaleActivityPageRequest> flashSaleActivityPageRequest) => InvokeAsync(_ => _.GetFlashSaleDataActivityPageByIdsAsync(flashSaleActivityPageRequest));
		/// <summary>��ȡ��ʱ�������������</summary>
        /// <returns></returns>
        public OperationResult<List<FlashSaleModel>> GetFlashSaleList(Guid[] activityIDs) => Invoke(_ => _.GetFlashSaleList(activityIDs));

	/// <summary>��ȡ��ʱ�������������</summary>
        /// <returns></returns>
        public Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleListAsync(Guid[] activityIDs) => InvokeAsync(_ => _.GetFlashSaleListAsync(activityIDs));
		/// <summary>�����������ɱ��������</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleModel>> SelectSecondKillTodayData(int activityType, DateTime? scheduleDate=null,bool needProducts=true,bool excludeProductTags = false) => Invoke(_ => _.SelectSecondKillTodayData(activityType,scheduleDate,needProducts,excludeProductTags));

	/// <summary>�����������ɱ��������</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleModel>>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate=null,bool needProducts=true,bool excludeProductTags = false) => InvokeAsync(_ => _.SelectSecondKillTodayDataAsync(activityType,scheduleDate,needProducts,excludeProductTags));
		/// <summary>����ʱ�䷶Χ��ѯ��ɱ������Ϣ</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleModel>> GetSecondsBoys(int activityType,DateTime? startDate=null,DateTime? endDate=null) => Invoke(_ => _.GetSecondsBoys(activityType,startDate,endDate));

	/// <summary>����ʱ�䷶Χ��ѯ��ɱ������Ϣ</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleModel>>> GetSecondsBoysAsync(int activityType,DateTime? startDate=null,DateTime? endDate=null) => InvokeAsync(_ => _.GetSecondsBoysAsync(activityType,startDate,endDate));
		/// <summary>�ҳ��ɱ��ѯ���³���</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleModel>> GetActivePageSecondKill(int topNumber,bool isProducts=true) => Invoke(_ => _.GetActivePageSecondKill(topNumber,isProducts));

	/// <summary>�ҳ��ɱ��ѯ���³���</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleModel>>> GetActivePageSecondKillAsync(int topNumber,bool isProducts=true) => InvokeAsync(_ => _.GetActivePageSecondKillAsync(topNumber,isProducts));
		/// <summary>����ȡ����������ʱ����������</summary>
        /// <returns></returns>
        public OperationResult<int> DeleteFlashSaleRecords(int orderId) => Invoke(_ => _.DeleteFlashSaleRecords(orderId));

	/// <summary>����ȡ����������ʱ����������</summary>
        /// <returns></returns>
        public Task<OperationResult<int>> DeleteFlashSaleRecordsAsync(int orderId) => InvokeAsync(_ => _.DeleteFlashSaleRecordsAsync(orderId));
		/// <summary>��ȡ������ɱ�û�������Ϣ</summary>/// <returns>null</returns>
        public OperationResult<UserReminderInfo> GetUserReminderInfo(EveryDaySeckillUserInfo model) => Invoke(_ => _.GetUserReminderInfo(model));

	/// <summary>��ȡ������ɱ�û�������Ϣ</summary>/// <returns>null</returns>
        public Task<OperationResult<UserReminderInfo>> GetUserReminderInfoAsync(EveryDaySeckillUserInfo model) => InvokeAsync(_ => _.GetUserReminderInfoAsync(model));
		/// <summary>����������ɱ�û���Ϣ</summary>/// <returns>null</returns>
        public OperationResult<InsertEveryDaySeckillUserInfoResponse> InsertEveryDaySeckillUserInfo(EveryDaySeckillUserInfo model) => Invoke(_ => _.InsertEveryDaySeckillUserInfo(model));

	/// <summary>����������ɱ�û���Ϣ</summary>/// <returns>null</returns>
        public Task<OperationResult<InsertEveryDaySeckillUserInfoResponse>> InsertEveryDaySeckillUserInfoAsync(EveryDaySeckillUserInfo model) => InvokeAsync(_ => _.InsertEveryDaySeckillUserInfoAsync(model));
		/// <summary>У����ʱ������Ʒ�Ƿ��޹�</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleProductBuyLimitModel>> CheckFlashSaleProductBuyLimit(CheckFlashSaleProductRequest request) => Invoke(_ => _.CheckFlashSaleProductBuyLimit(request));

	/// <summary>У����ʱ������Ʒ�Ƿ��޹�</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleProductBuyLimitModel>>> CheckFlashSaleProductBuyLimitAsync(CheckFlashSaleProductRequest request) => InvokeAsync(_ => _.CheckFlashSaleProductBuyLimitAsync(request));
		/// <summary>��ȡ�û������Թ���˻��Ʒ����</summary>
        /// <returns></returns>
        public OperationResult<FlashSaleProductCanBuyCountModel> GetUserCanBuyFlashSaleItemCount(Guid userId,Guid activityId,string pid) => Invoke(_ => _.GetUserCanBuyFlashSaleItemCount(userId,activityId,pid));

	/// <summary>��ȡ�û������Թ���˻��Ʒ����</summary>
        /// <returns></returns>
        public Task<OperationResult<FlashSaleProductCanBuyCountModel>> GetUserCanBuyFlashSaleItemCountAsync(Guid userId,Guid activityId,string pid) => InvokeAsync(_ => _.GetUserCanBuyFlashSaleItemCountAsync(userId,activityId,pid));
		/// <summary>У���Ƿ�ɹ��������ʱ��������</summary>
        /// <returns></returns>
        public OperationResult<FlashSaleOrderResponse> CheckCanBuyFlashSaleOrder(FlashSaleOrderRequest request) => Invoke(_ => _.CheckCanBuyFlashSaleOrder(request));

	/// <summary>У���Ƿ�ɹ��������ʱ��������</summary>
        /// <returns></returns>
        public Task<OperationResult<FlashSaleOrderResponse>> CheckCanBuyFlashSaleOrderAsync(FlashSaleOrderRequest request) => InvokeAsync(_ => _.CheckCanBuyFlashSaleOrderAsync(request));
		/// <summary>/// ��ѯ��Ʒ����ҳ��ʱ��������/// </summary>///  <returns></returns>
        public OperationResult<FlashSaleProductDetailModel> FetchProductDetailForFlashSale(string productId, string variantId, string activityId,string channel, string userId,string productGroupId=null,int buyQty=1) => Invoke(_ => _.FetchProductDetailForFlashSale(productId,variantId,activityId,channel,userId,productGroupId,buyQty));

	/// <summary>/// ��ѯ��Ʒ����ҳ��ʱ��������/// </summary>///  <returns></returns>
        public Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailForFlashSaleAsync(string productId, string variantId, string activityId,string channel, string userId,string productGroupId=null,int buyQty=1) => InvokeAsync(_ => _.FetchProductDetailForFlashSaleAsync(productId,variantId,activityId,channel,userId,productGroupId,buyQty));
		///<summary>���ټ�����</summary>
        public OperationResult<bool> DecrementCounter(int orderId) => Invoke(_ => _.DecrementCounter(orderId));

	///<summary>���ټ�����</summary>
        public Task<OperationResult<bool>> DecrementCounterAsync(int orderId) => InvokeAsync(_ => _.DecrementCounterAsync(orderId));
		///<summary>ȡ������ʱά������</summary>
        public OperationResult<bool> OrderCancerMaintenanceFlashSaleData(int orderId) => Invoke(_ => _.OrderCancerMaintenanceFlashSaleData(orderId));

	///<summary>ȡ������ʱά������</summary>
        public Task<OperationResult<bool>> OrderCancerMaintenanceFlashSaleDataAsync(int orderId) => InvokeAsync(_ => _.OrderCancerMaintenanceFlashSaleDataAsync(orderId));
		///<summary>ˢ���޹���Ʒ����������</summary>
        public OperationResult<bool> RefreshFlashSaleHashCount(List<string> activtyids ,bool isAllRefresh) => Invoke(_ => _.RefreshFlashSaleHashCount(activtyids,isAllRefresh));

	///<summary>ˢ���޹���Ʒ����������</summary>
        public Task<OperationResult<bool>> RefreshFlashSaleHashCountAsync(List<string> activtyids ,bool isAllRefresh) => InvokeAsync(_ => _.RefreshFlashSaleHashCountAsync(activtyids,isAllRefresh));
		///<summary>ֻ��ѯ�������Ϣ</summary>
        public OperationResult<List<FlashSaleModel>> GetFlashSaleWithoutProductsList(List<Guid> activtyids) => Invoke(_ => _.GetFlashSaleWithoutProductsList(activtyids));

	///<summary>ֻ��ѯ�������Ϣ</summary>
        public Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleWithoutProductsListAsync(List<Guid> activtyids) => InvokeAsync(_ => _.GetFlashSaleWithoutProductsListAsync(activtyids));
		///<summary>��ѯ����ҳ������Ϣ</summary>
        public OperationResult<FlashSaleProductDetailModel> FetchProductDetailDisountInfo(DiscountActivtyProductRequest request) => Invoke(_ => _.FetchProductDetailDisountInfo(request));

	///<summary>��ѯ����ҳ������Ϣ</summary>
        public Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailDisountInfoAsync(DiscountActivtyProductRequest request) => InvokeAsync(_ => _.FetchProductDetailDisountInfoAsync(request));
		///<summary>��ѯ�û��µ���ά�����޹�������</summary>
        public OperationResult<OrderCountResponse> GetUserCreateFlashOrderCountCache(OrderCountCacheRequest request) => Invoke(_ => _.GetUserCreateFlashOrderCountCache(request));

	///<summary>��ѯ�û��µ���ά�����޹�������</summary>
        public Task<OperationResult<OrderCountResponse>> GetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request) => InvokeAsync(_ => _.GetUserCreateFlashOrderCountCacheAsync(request));
		///<summary>�����û��µ���ά�����޹�������</summary>
        public OperationResult<OrderCountResponse> SetUserCreateFlashOrderCountCache(OrderCountCacheRequest request) => Invoke(_ => _.SetUserCreateFlashOrderCountCache(request));

	///<summary>�����û��µ���ά�����޹�������</summary>
        public Task<OperationResult<OrderCountResponse>> SetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request) => InvokeAsync(_ => _.SetUserCreateFlashOrderCountCacheAsync(request));
		///<summary>��ȡ���������ݲ�׼ȷ������</summary>
        public OperationResult<List<FlashSaleWrongCacheResponse>> SelectFlashSaleWrongCache() => Invoke(_ => _.SelectFlashSaleWrongCache());

	///<summary>��ȡ���������ݲ�׼ȷ������</summary>
        public Task<OperationResult<List<FlashSaleWrongCacheResponse>>> SelectFlashSaleWrongCacheAsync() => InvokeAsync(_ => _.SelectFlashSaleWrongCacheAsync());
		///<summary>�û���������ʱ��ά����������</summary>
        public OperationResult<bool> OrderCreateMaintenanceFlashSaleDbData(FlashSaleOrderRequest flashSale) => Invoke(_ => _.OrderCreateMaintenanceFlashSaleDbData(flashSale));

	///<summary>�û���������ʱ��ά����������</summary>
        public Task<OperationResult<bool>> OrderCreateMaintenanceFlashSaleDbDataAsync(FlashSaleOrderRequest flashSale) => InvokeAsync(_ => _.OrderCreateMaintenanceFlashSaleDbDataAsync(flashSale));
		///<summary>����־����������������ñ�</summary>
        public OperationResult<bool> UpdateConfigSaleoutQuantityFromLog(UpdateConfigSaleoutQuantityRequest request) => Invoke(_ => _.UpdateConfigSaleoutQuantityFromLog(request));

	///<summary>����־����������������ñ�</summary>
        public Task<OperationResult<bool>> UpdateConfigSaleoutQuantityFromLogAsync(UpdateConfigSaleoutQuantityRequest request) => InvokeAsync(_ => _.UpdateConfigSaleoutQuantityFromLogAsync(request));
		///<summary>���ճ���ˢ����ɱĬ������</summary>
        public OperationResult<bool> RefreshSeckillDefaultDataBySchedule(string schedule) => Invoke(_ => _.RefreshSeckillDefaultDataBySchedule(schedule));

	///<summary>���ճ���ˢ����ɱĬ������</summary>
        public Task<OperationResult<bool>> RefreshSeckillDefaultDataByScheduleAsync(string schedule) => InvokeAsync(_ => _.RefreshSeckillDefaultDataByScheduleAsync(schedule));
		///<summary>��ȡ����ʱ����ڵ���ɱ�</summary>
        public OperationResult<Dictionary<string, List<SeckilScheduleInfoRespnose>>> GetSeckillScheduleInfo(List<string> pids, DateTime sSchedule, DateTime eSchedule) => Invoke(_ => _.GetSeckillScheduleInfo(pids,sSchedule,eSchedule));

	///<summary>��ȡ����ʱ����ڵ���ɱ�</summary>
        public Task<OperationResult<Dictionary<string, List<SeckilScheduleInfoRespnose>>>> GetSeckillScheduleInfoAsync(List<string> pids, DateTime sSchedule, DateTime eSchedule) => InvokeAsync(_ => _.GetSeckillScheduleInfoAsync(pids,sSchedule,eSchedule));
		///<summary>���ݻ��Ʒ��ȡ�û��Ʒ�Ŀ����Ϣ</summary>
        public OperationResult<List<SeckillAvailableStockInfoResponse>> GetSeckillAvailableStockResponse(List<SeckillAvailableStockInfoRequest> request) => Invoke(_ => _.GetSeckillAvailableStockResponse(request));

	///<summary>���ݻ��Ʒ��ȡ�û��Ʒ�Ŀ����Ϣ</summary>
        public Task<OperationResult<List<SeckillAvailableStockInfoResponse>>> GetSeckillAvailableStockResponseAsync(List<SeckillAvailableStockInfoRequest> request) => InvokeAsync(_ => _.GetSeckillAvailableStockResponseAsync(request));
		///<summary>��ѯ��ҳ������ɱ����</summary>
        public OperationResult<List<FlashSaleModel>> SelectHomeSeckillData(SelectHomeSecKillRequest request) => Invoke(_ => _.SelectHomeSeckillData(request));

	///<summary>��ѯ��ҳ������ɱ����</summary>
        public Task<OperationResult<List<FlashSaleModel>>> SelectHomeSeckillDataAsync(SelectHomeSecKillRequest request) => InvokeAsync(_ => _.SelectHomeSeckillDataAsync(request));
		///<summary>���ݳ���Id��ȡ������ɱ��Ʒ����</summary>
        public OperationResult<List<FlashSaleProductModel>> SelectSeckillDataByActivityId(SelectSecKillByIdRequest request) => Invoke(_ => _.SelectSeckillDataByActivityId(request));

	///<summary>���ݳ���Id��ȡ������ɱ��Ʒ����</summary>
        public Task<OperationResult<List<FlashSaleProductModel>>> SelectSeckillDataByActivityIdAsync(SelectSecKillByIdRequest request) => InvokeAsync(_ => _.SelectSeckillDataByActivityIdAsync(request));
	}
	/// <summary>�����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IActivityService
    {
    	/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityResponse")]
        Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize);
		/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityNewResponse")]
        Task<OperationResult<TireActivityModel>> SelectTireActivityNewAsync(TireActivityRequest request);
		/// <summary>��ѯ��̥��ת�</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireChangedActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireChangedActivityResponse")]
        Task<OperationResult<string>> SelectTireChangedActivityAsync(TireActivityRequest request);
		/// <summary>��ѯ��̥��Ĳ�Ʒ</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityPidsResponse")]
        Task<OperationResult<IEnumerable<string>>> SelectTireActivityPidsAsync(Guid activityId);
		/// <summary>������̥�����</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateTireActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateTireActivityCacheResponse")]
        Task<OperationResult<bool>> UpdateTireActivityCacheAsync(string vehicleId, string tireSize);
		/// <summary>������̥��Ĳ�Ʒ</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityPidsCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityPidsCacheResponse")]
        Task<OperationResult<bool>> UpdateActivityPidsCacheAsync(Guid activityId);
		/// <summary>����������̥</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptTires", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptTiresResponse")]
        Task<OperationResult<List<VehicleAdaptTireTireSizeDetailModel>>> SelectVehicleAaptTiresAsync(VehicleAdaptTireRequestModel request);
		///<summary>�Ż�ȯ��Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCarTagCouponConfigs", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCarTagCouponConfigsResponse")]
        Task<OperationResult<IEnumerable<CarTagCouponConfigModel>>> SelectCarTagCouponConfigsAsync();
		///<summary>�������䱣��</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptBaoyangs", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptBaoyangsResponse")]
        Task<OperationResult<IEnumerable<VehicleAdaptBaoyangModel>>> SelectVehicleAaptBaoyangsAsync(string vehicleId);
		///<summary>�������䳵Ʒ��Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAdaptChepins", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAdaptChepinsResponse")]
        Task<OperationResult<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>>> SelectVehicleAdaptChepinsAsync(string vehicleId);
		///<summary>��ȡ��������̥���</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleSortedTireSizes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleSortedTireSizesResponse")]
        Task<OperationResult<IEnumerable<VehicleSortedTireSizeModel>>> SelectVehicleSortedTireSizesAsync(string vehicleId);
		///<summary> �����û�������Ϣ������guid</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserShareInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserShareInfoResponse")]
        Task<OperationResult<Guid>> GetGuidAndInsertUserShareInfoAsync(string pid, Guid batchGuid, Guid userId);
		///<summary> ����Guidȡ��д����е�����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserShareInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserShareInfoResponse")]
        Task<OperationResult<ActivityUserShareInfoModel>> GetActivityUserShareInfoAsync(Guid shareId);
		///<summary> ���ݻID��ѯ�û���ȡ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPromotionPacketHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPromotionPacketHistoryResponse")]
        Task<OperationResult<IEnumerable<PromotionPacketHistoryModel>>> SelectPromotionPacketHistoryAsync(Guid userId, Guid luckyWheel);
		///<summary>�������ñ�id���û�idȡ�����ɵ���id������׬Ǯ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserForShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserForShareResponse")]
        Task<OperationResult<Guid>> GetGuidAndInsertUserForShareAsync(Guid configGuid, Guid userId);
		///<summary>��ȡ���ñ��һ�����ݣ�����׬Ǯ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRecommendGetGiftConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRecommendGetGiftConfigResponse")]
        Task<OperationResult<RecommendGetGiftConfigModel>> FetchRecommendGetGiftConfigAsync(Guid? number=null,Guid? userId=null);
		///<summary>��ѯ�����ȡ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPacketByUsers", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPacketByUsersResponse")]
        Task<OperationResult<DataTable>> SelectPacketByUsersAsync();
		///<summary>��ѯ��̥�</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityByActivityIdResponse")]
        Task<OperationResult<TireActivityModel>> SelectTireActivityByActivityIdAsync(Guid activityId);
		/// <summary>��ȡ�����ҳ��url</summary>/// <returns>��ڼ���ݵ���ȡ�ûҳ�����ӣ����򷵻��Ƿ���δ��ʼ���߹���</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionActivityPageUrl", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionActivityPageUrlResponse")]
        Task<OperationResult<RegionActivityPageModel>> GetRegionActivityPageUrlAsync(string city,string activityId);
		/// <summary>���ݻId�͵���Id���߳���Id��ȡĿ����ַ</summary>
[Obsolete("��ʹ��GetRegionVehicleIdActivityUrlNewAsync")]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrl", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlResponse")]
        Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlAsync(Guid activityId, int regionId, string vehicleId);
		/// <summary>���ݻId,������͵���Id���߳���Id��ȡĿ����ַ</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlNewResponse")]
        Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlNewAsync(Guid activityId, int regionId, string vehicleId, string activityChannel);
		/// <summary>�������</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionVehicleIdActivityUrlCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionVehicleIdActivityUrlCacheResponse")]
        Task<OperationResult<bool>> RefreshRegionVehicleIdActivityUrlCacheAsync(Guid activityId);
		///<summary>���ҳ��ѯ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityConfigForDownloadApp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityConfigForDownloadAppResponse")]
        Task<OperationResult<DownloadApp>> GetActivityConfigForDownloadAppAsync(int id);
		///<summary>������ҳ���ݻ���</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CleanActivityConfigForDownloadAppCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CleanActivityConfigForDownloadAppCacheResponse")]
        Task<OperationResult<bool>> CleanActivityConfigForDownloadAppCacheAsync(int id);
		///<summary>ȡ����֧ͬ���˻��Ķ��� -1:ʧ�� 1:ȡ���ɹ� 2:��ȡ������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CancelActivityOrderOfSamePaymentAccount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CancelActivityOrderOfSamePaymentAccountResponse")]
        Task<OperationResult<int>> CancelActivityOrderOfSamePaymentAccountAsync(int orderId, string paymentAccount);
		///<summary>��ȡ�ҳ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelResponse")]
        Task<OperationResult<ActivePageListModel>> GetActivePageListModelAsync(ActivtyPageRequest request);
		///<summary>��ȡ�ҳ����-zip</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelZip", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelZipResponse")]
        Task<OperationResult<Tuhu.Service.Activity.Zip.Models.ActivePageListModel>> GetActivePageListModelZipAsync(ActivtyPageRequest request);
		///<summary>��ȡ�����û��ɷ�������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckyWheelUserlotteryCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckyWheelUserlotteryCountResponse")]
        Task<OperationResult<int>> GetLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup,string hashkey=null);
		///<summary>���´����û��ɷ�������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateLuckyWheelUserlotteryCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateLuckyWheelUserlotteryCountResponse")]
        Task<OperationResult<int>> UpdateLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup,string hashkey=null);
		///<summary>ˢ�»ҳ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivePageListModelCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivePageListModelCacheResponse")]
        Task<OperationResult<bool>> RefreshActivePageListModelCacheAsync(ActivtyPageRequest request);
		///<summary>ˢ�´�����������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshLuckWheelCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshLuckWheelCacheResponse")]
        Task<OperationResult<bool>> RefreshLuckWheelCacheAsync(string id);
		/// <summary>��֤��̥�����Ƿ��ܹ���</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationByTires", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationByTiresResponse")]
        Task<OperationResult<VerificationTiresResponseModel>> VerificationByTiresAsync(VerificationTiresRequestModel requestModel);
		/// <summary>������̥�µ���¼</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertTiresOrderRecordResponse")]
        Task<OperationResult<bool>> InsertTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel);
		/// <summary>������̥�µ���¼</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RevokeTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RevokeTiresOrderRecordResponse")]
        Task<OperationResult<bool>> RevokeTiresOrderRecordAsync(int orderId);
		/// <summary>Redis����̥������¼�ؽ�</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RedisTiresOrderRecordReconStruction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RedisTiresOrderRecordReconStructionResponse")]
        Task<OperationResult<bool>> RedisTiresOrderRecordReconStructionAsync(TiresOrderRecordRequestModel requestModel);
		/// <summary>RedisAndSql����̥������¼��ѯ</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTiresOrderRecordResponse")]
        Task<OperationResult<Dictionary<string, object>>> SelectTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel);
		/// <summary>��֤��̥�Ż�ȯ�Ƿ�����ȡ</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationTiresPromotionRule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationTiresPromotionRuleResponse")]
        Task<OperationResult<VerificationTiresResponseModel>> VerificationTiresPromotionRuleAsync(VerificationTiresRequestModel requestModel,int ruleId);
		///<summary>��ȡ������������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckWheel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckWheelResponse")]
        Task<OperationResult<LuckyWheelModel>> GetLuckWheelAsync(string id);
		///<summary> ����׬Ǯ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectShareActivityProductById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectShareActivityProductByIdResponse")]
        Task<OperationResult<ShareProductModel>> SelectShareActivityProductByIdAsync(string ProductId,string BatchGuid=null);
		///<summary>�����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectBaoYangActivitySetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectBaoYangActivitySettingResponse")]
        Task<OperationResult<BaoYangActivitySetting>> SelectBaoYangActivitySettingAsync(string activityId);
		/// <summary></summary>///<returns></returns>
		[Obsolete("��ʹ��SelectCouponActivityConfigNew")]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigResponse")]
        Task<OperationResult<CouponActivityConfigModel>> SelectCouponActivityConfigAsync(string activityNum, int type);
		///<summary>��ȡ�����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectActivityTypeByActivityIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectActivityTypeByActivityIdsResponse")]
        Task<OperationResult<IEnumerable<ActivityTypeModel>>> SelectActivityTypeByActivityIdsAsync(List<Guid> activityIds);
		///<summary>���������ʻ�ȡ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityBuildWithSelKey", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityBuildWithSelKeyResponse")]
        Task<OperationResult<ActivityBuild>> GetActivityBuildWithSelKeyAsync(string keyword);
		///<summary>��¼�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityTypeLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityTypeLogResponse")]
        Task<OperationResult<bool>> RecordActivityTypeLogAsync(ActivityTypeRequest request);
		///<summary>���±��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangActivityConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangActivityConfigResponse")]
        Task<OperationResult<bool>> UpdateBaoYangActivityConfigAsync(Guid activityId);
		///<summary>��ȡ�����״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityStatusResponse")]
        Task<OperationResult<FixedPriceActivityStatusResult>> GetFixedPriceActivityStatusAsync(Guid activityId, Guid userId, int regionId);
		///<summary>���û������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangPurchaseCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangPurchaseCountResponse")]
        Task<OperationResult<bool>> UpdateBaoYangPurchaseCountAsync(Guid activityId);
		///<summary>����activityId��ȡ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityRound", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityRoundResponse")]
        Task<OperationResult<FixedPriceActivityRoundResponse>> GetFixedPriceActivityRoundAsync(Guid activityId);
		///<summary>����activityId��RegionId��ȡ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRegionTiresActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRegionTiresActivityResponse")]
        Task<OperationResult<TiresActivityResponse>> FetchRegionTiresActivityAsync(FlashSaleTiresActivityRequest request);
		///<summary>ˢ����̥�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionTiresActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionTiresActivityCacheResponse")]
        Task<OperationResult<bool>> RefreshRegionTiresActivityCacheAsync(Guid activityId, int regionId);
		///<summary>��¼�û�������ʼ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityProductUserRemindLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityProductUserRemindLogResponse")]
        Task<OperationResult<bool>> RecordActivityProductUserRemindLogAsync(ActivityProductUserRemindRequest request);
		///<summary>��ӷ��������¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordResponse")]
        Task<OperationResult<bool>> InsertRebateApplyRecordAsync(RebateApplyRequest request);
		///<summary>��ʼ�����������ݻ��ߺ�������û�������״̬����ʹ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertOrUpdateActivityPageWhiteListRecords", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertOrUpdateActivityPageWhiteListRecordsResponse")]
        Task<OperationResult<bool>> InsertOrUpdateActivityPageWhiteListRecordsAsync(List<ActivityPageWhiteListRequest> requests);
		///<summary>����Userid�ж��Ƿ��ǰ������û�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageWhiteListByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageWhiteListByUserIdResponse")]
        Task<OperationResult<bool>> GetActivityPageWhiteListByUserIdAsync(Guid userId);
		///<summary>��¼;����̥���û�������Ϣ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutUserRewardApplication", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutUserRewardApplicationResponse")]
        Task<OperationResult<UserRewardApplicationResponse>> PutUserRewardApplicationAsync(UserRewardApplicationRequest request);
		///<summary>��ӷ��������¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordNewResponse")]
        Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordNewAsync(RebateApplyRequest request);
		///<summary>;������������¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutApplyCompensateRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutApplyCompensateRecordResponse")]
        Task<OperationResult<bool>> PutApplyCompensateRecordAsync(ApplyCompensateRequest request);
		///<summary>�������Ч����֤�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivtyValidityResponses", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivtyValidityResponsesResponse")]
        Task<OperationResult<List<ActivtyValidityResponse>>> GetActivtyValidityResponsesAsync(ActivtyValidityRequest request);
		///<summary>��ȡԤ����������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCardSaleConfigDetails", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCardSaleConfigDetailsResponse")]
        Task<OperationResult<List<VipCardSaleConfigDetailModel>>> GetVipCardSaleConfigDetailsAsync(string activityId);
		///<summary>check����������Ƿ���ʣ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VipCardCheckStock", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VipCardCheckStockResponse")]
        Task<OperationResult<Dictionary<string, bool>>> VipCardCheckStockAsync(List<string> batchIds);
		///<summary>��������ʱ��¼����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutVipCardRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutVipCardRecordResponse")]
        Task<OperationResult<bool>> PutVipCardRecordAsync(VipCardRecordRequest request);
		///<summary>֧���ɹ�ʱ���ð�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/BindVipCard", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/BindVipCardResponse")]
        Task<OperationResult<bool>> BindVipCardAsync(int orderId);
		///<summary>��ȡ��������ҳ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyPageConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyPageConfigResponse")]
        Task<OperationResult<RebateApplyPageConfig>> SelectRebateApplyPageConfigAsync();
		///<summary>���뷵��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordV2", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordV2Response")]
        Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordV2Async(RebateApplyRequest request);
		///<summary>��ȡ�û����з���������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyByOpenId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyByOpenIdResponse")]
        Task<OperationResult<List<RebateApplyResponse>>> SelectRebateApplyByOpenIdAsync(string openId);
		///<summary>ͨ��OrderId��ȡ���µ������¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRebateApplyByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRebateApplyByOrderIdResponse")]
        Task<OperationResult<IEnumerable<RebateApplyResponse>>> GetRebateApplyByOrderIdAsync(int orderId);
		///<summary>ȡ��������ʱ�����db�������е�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyVipCardRecordByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyVipCardRecordByOrderIdResponse")]
        Task<OperationResult<bool>> ModifyVipCardRecordByOrderIdAsync(int orderId);
		///<summary>��ȡ2018���籭�Ļ����ͻ��ֹ�����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetWorldCup2018Activity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetWorldCup2018ActivityResponse")]
        Task<OperationResult<ActivityResponse>> GetWorldCup2018ActivityAsync();
		///<summary>ͨ���û�ID��ȡ�һ�ȯ�����ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCouponCountByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCouponCountByUserIdResponse")]
        Task<OperationResult<int>> GetCouponCountByUserIdAsync(Guid userId, long activityId);
		///<summary>���ػ�һ�ȯ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchCouponRank", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchCouponRankResponse")]
        Task<OperationResult<PagedModel<ActivityCouponRankResponse>>> SearchCouponRankAsync(long activityId, int pageIndex = 1, int pageSize = 20);
		///<summary>�����û��Ķһ�ȯ�������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponRank", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponRankResponse")]
        Task<OperationResult<ActivityCouponRankResponse>> GetUserCouponRankAsync(Guid userId, long activityId);
		///<summary>�һ����б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeListResponse")]
        Task<OperationResult<PagedModel<ActivityPrizeResponse>>> SearchPrizeListAsync(SearchPrizeListRequest searchPrizeListRequest);
		///<summary>�û��һ���Ʒ �쳣���룺-1 ϵͳ�쳣�������ԣ� , -2 �һ�������  -3 ��治��  -4 �Ѿ��¼�   -5 �Ѿ��һ�   -6 �һ�ʱ���Ѿ���ֹ���ܶһ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UserRedeemPrizes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UserRedeemPrizesResponse")]
        Task<OperationResult<bool>> UserRedeemPrizesAsync(Guid userId, long prizeId,long activityId);
		///<summary>�û��Ѷһ���Ʒ�б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeOrderDetailListByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeOrderDetailListByUserIdResponse")]
        Task<OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>>> SearchPrizeOrderDetailListByUserIdAsync(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20);
		///<summary>���վ�����Ŀ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionResponse")]
        Task<OperationResult<IEnumerable<Models.Response.Question>>> SearchQuestionAsync( Guid userId , long activityId);
		///<summary>�ύ�û�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionAnswerResponse")]
        Task<OperationResult<bool>> SubmitQuestionAnswerAsync(SubmitQuestionAnswerRequest submitQuestionAnswerRequest);
		///<summary>�����û�������ʷ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionAnswerHistoryByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionAnswerHistoryByUserIdResponse")]
        Task<OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>>> SearchQuestionAnswerHistoryByUserIdAsync(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest);
		///<summary>�����û�ʤ��������ʤ���ƺ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVictoryInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVictoryInfoResponse")]
        Task<OperationResult<ActivityVictoryInfoResponse>> GetVictoryInfoAsync(Guid userId, long activityId);
		///<summary>��������ͻ���  �쳣��   -77 �δ��ʼ  -2 �����Ѿ�����   -1 ϵͳ�쳣</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityShareResponse")]
        Task<OperationResult<bool>> ActivityShareAsync(ActivityShareDetailRequest shareDetailRequest);
		///<summary>�����Ƿ��Ѿ������� true = �����Ѿ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTodayAlreadyShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTodayAlreadyShareResponse")]
        Task<OperationResult<bool>> ActivityTodayAlreadyShareAsync(Guid userId, long activityId);
		///<summary>������ȡ����ˢ������õĳ���������Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetOrSetActivityPageSortedPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetOrSetActivityPageSortedPidsResponse")]
        Task<OperationResult<List<string>>> GetOrSetActivityPageSortedPidsAsync(SortedPidsRequest request);
		///<summary>�޸Ļ��������û��һ�ȯ ����������־  ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyActivityCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyActivityCouponResponse")]
        Task<OperationResult<long>> ModifyActivityCouponAsync(Guid userId, long activityId, int couponCount , string couponName,DateTime? modifyDateTime = null);
		///<summary>ˢ�»��Ŀ  ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityQuestionCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityQuestionCacheResponse")]
        Task<OperationResult<bool>> RefreshActivityQuestionCacheAsync(long activityId);
		///<summary>ˢ�»�һ���  ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityPrizeCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityPrizeCacheResponse")]
        Task<OperationResult<bool>> RefreshActivityPrizeCacheAsync(long activityId);
		///<summary>�����û��������ݵ����ݿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswerResponse")]
        Task<OperationResult<SubmitActivityQuestionUserAnswerResponse>> SubmitQuestionUserAnswerAsync(SubmitActivityQuestionUserAnswerRequest request);
		///<summary>�����û�������״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResultResponse")]
        Task<OperationResult<ModifyQuestionUserAnswerResultResponse>> ModifyQuestionUserAnswerResultAsync(ModifyQuestionUserAnswerResultRequest request);
		///<summary>�»ҳ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoConfigModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoConfigModelResponse")]
        Task<OperationResult<ActivityPageInfoModel>> GetActivityPageInfoConfigModelAsync(ActivityPageInfoRequest request);
		///<summary>�ҳ�᳡����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoHomeModels", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoHomeModelsResponse")]
        Task<OperationResult<List<ActivityPageInfoHomeModel>>> GetActivityPageInfoHomeModelsAsync(string hashKey);
		///<summary>�ҳ��Ʒ�Ƽ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoCpRecommends", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoCpRecommendsResponse")]
        Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoCpRecommendsAsync(ActivityPageInfoModuleRecommendRequest request);
		///<summary>�ҳ��̥�Ƽ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoTireRecommends", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoTireRecommendsResponse")]
        Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoTireRecommendsAsync(ActivityPageInfoModuleRecommendRequest request);
		///<summary>�ҳ�˵�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowMenus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowMenusResponse")]
        Task<OperationResult<List<ActivityPageInfoRowMenuModel>>> GetActivityPageInfoRowMenusAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ����Ʒ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPools", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPoolsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowProductPool>>> GetActivityPageInfoRowPoolsAsync(ActivityPageInfoModuleProductPoolRequest request);
		///<summary>�ҳ����Ʒ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowNewPools", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowNewPoolsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowNewProductPool>>> GetActivityPageInfoRowNewPoolsAsync(ActivityPageInfoModuleNewProductPoolRequest request);
		///<summary>�ҳ����ʱ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCountDowns", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCountDownsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowCountDown>>> GetActivityPageInfoRowCountDownsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ�İ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowActivityTexts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowActivityTextsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowActivityText>>> GetActivityPageInfoRowActivityTextsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ�������������Ż�ȯ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowJsons", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowJsonsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowJson>>> GetActivityPageInfoRowJsonsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳƴ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPintuans", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPintuansResponse")]
        Task<OperationResult<List<ActivityPageInfoRowPintuan>>> GetActivityPageInfoRowPintuansAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��Ƶ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVideos", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVideosResponse")]
        Task<OperationResult<List<ActivityPageInfoRowVideo>>> GetActivityPageInfoRowVideosAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ���������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOtherActivitys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOtherActivitysResponse")]
        Task<OperationResult<List<ActivityPageInfoRowOtherActivity>>> GetActivityPageInfoRowOtherActivitysAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOthers", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOthersResponse")]
        Task<OperationResult<List<ActivityPageInfoRowOther>>> GetActivityPageInfoRowOthersAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ���������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowRules", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowRulesResponse")]
        Task<OperationResult<List<ActivityPageInfoRowRule>>> GetActivityPageInfoRowRulesAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowBys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowBysResponse")]
        Task<OperationResult<List<ActivityPageInfoRowBy>>> GetActivityPageInfoRowBysAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ�Ż�ȯ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCoupons", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCouponsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowCoupon>>> GetActivityPageInfoRowCouponsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��Ʒ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowProducts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowProductsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowProduct>>> GetActivityPageInfoRowProductsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowLinks", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowLinksResponse")]
        Task<OperationResult<List<ActivityPageInfoRowLink>>> GetActivityPageInfoRowLinksAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳͼƬ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowImages", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowImagesResponse")]
        Task<OperationResult<List<ActivityPageInfoRowImage>>> GetActivityPageInfoRowImagesAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��ɱ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowSeckills", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowSeckillsResponse")]
        Task<OperationResult<List<ActivityPageInfoRowSeckill>>> GetActivityPageInfoRowSeckillsAsync(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳͷͼ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVehicleBanners", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVehicleBannersResponse")]
        Task<OperationResult<List<ActivityPageInfoRowVehicleBanner>>> GetActivityPageInfoRowVehicleBannersAsync(ActivityPageInfoModuleVehicleBannerRequest request);
		///<summary>ͨ������ͻ�ȡ� 0 ���籭  1 ƴ�ų�����֤</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityInfoByType", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityInfoByTypeResponse")]
        Task<OperationResult<ActivityResponse>> GetActivityInfoByTypeAsync(int activityTypeId);
		///<summary>���ݻ��Ų�ѯ������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCustomerSettingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCustomerSettingInfoResponse")]
        Task<OperationResult<ActiveCustomerSettingResponse>> GetCustomerSettingInfoAsync(string activeNo);
		///<summary>��ѯ�û��󶨵�ȯ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponCode", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponCodeResponse")]
        Task<OperationResult<string>> GetUserCouponCodeAsync(string activityExclusiveId, string userid);
		///<summary>�û�ȯ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CouponCodeBound", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CouponCodeBoundResponse")]
        Task<OperationResult<bool>> CouponCodeBoundAsync(ActivityCustomerCouponRequests activityCustomerCouponRequests);
		///<summary>�ͻ�ר���µ���֤</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActiveOrderVerify", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActiveOrderVerifyResponse")]
        Task<OperationResult<bool>> ActiveOrderVerifyAsync(ActivityOrderVerifyRequests activityOrderVerifyRequests);
		///<summary>H5 ���;���Ǽ��ŵ���֤��Ϣ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddStarRatingStore", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddStarRatingStoreResponse")]
        Task<OperationResult<bool>> AddStarRatingStoreAsync(AddStarRatingStoreRequest request);
		///<summary>������ʱ����ID��ѯ��ͻ��������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCustomerSettingInfoByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCustomerSettingInfoByActivityIdResponse")]
        Task<OperationResult<ActiveCustomerSettingResponse>> GetVipCustomerSettingInfoByActivityIdAsync(string activityId);
		///<summary>��ȡ��ͻ�������Ա��̥�����쳣��Ϊ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetExceptionalCustomerOrderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetExceptionalCustomerOrderInfoResponse")]
        Task<OperationResult<List<ActivityCustomerInvalidOrderResponse>>> GetExceptionalCustomerOrderInfoAsync();
		///<summary>��ȡ����/���Ϳ������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigNewResponse")]
        Task<OperationResult<CouponActivityConfigNewModel>> SelectCouponActivityConfigNewAsync(CouponActivityConfigRequest request);
		///<summary>�����ӿڲ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTestMethod", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTestMethodResponse")]
        Task<OperationResult<string>> ActivityTestMethodAsync(int testType);
		///<summary>��ӻ����ҳ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddRegistrationOfActivitiesData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddRegistrationOfActivitiesDataResponse")]
        Task<OperationResult<bool>> AddRegistrationOfActivitiesDataAsync(RegistrationOfActivitiesRequest request);
		///<summary>Ӷ����Ʒ�б��ѯ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductListResponse")]
        Task<OperationResult<List<CommissionProductModel>>> GetCommissionProductListAsync(GetCommissionProductListRequest request);
		///<summary>Ӷ����Ʒ�����ѯ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductDetatils", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductDetatilsResponse")]
        Task<OperationResult<CommissionProductModel>> GetCommissionProductDetatilsAsync(GetCommissionProductDetatilsRequest request);
		///<summary>�µ���Ʒ��¼�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CreateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CreateOrderItemRecordResponse")]
        Task<OperationResult<CreateOrderItemRecordResponse>> CreateOrderItemRecordAsync(CreateOrderItemRecordRequest request);
		///<summary>Ӷ�𶩵���Ʒ��¼���½ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateOrderItemRecordResponse")]
        Task<OperationResult<UpdateOrderItemRecordResponse>> UpdateOrderItemRecordAsync(UpdateOrderItemRecordRequest request);
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityRebate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityRebateResponse")]
        Task<OperationResult<CommodityRebateResponse>> CommodityRebateAsync(CommodityRebateRequest request);
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityDeduction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityDeductionResponse")]
        Task<OperationResult<CommodityDeductionResponse>> CommodityDeductionAsync(CommodityDeductionRequest request);
		///<summary>CPS�޸�Ӷ����ˮ״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CpsUpdateRunning", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CpsUpdateRunningResponse")]
        Task<OperationResult<CpsUpdateRunningResponse>> CpsUpdateRunningAsync(CpsUpdateRunningRequest request);
	}

	/// <summary>�����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IActivityClient : IActivityService, ITuhuServiceClient
    {
    	/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityResponse")]
        OperationResult<TireActivityModel> SelectTireActivity(string vehicleId, string tireSize);
		/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityNewResponse")]
        OperationResult<TireActivityModel> SelectTireActivityNew(TireActivityRequest request);
		/// <summary>��ѯ��̥��ת�</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireChangedActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireChangedActivityResponse")]
        OperationResult<string> SelectTireChangedActivity(TireActivityRequest request);
		/// <summary>��ѯ��̥��Ĳ�Ʒ</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityPidsResponse")]
        OperationResult<IEnumerable<string>> SelectTireActivityPids(Guid activityId);
		/// <summary>������̥�����</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateTireActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateTireActivityCacheResponse")]
        OperationResult<bool> UpdateTireActivityCache(string vehicleId, string tireSize);
		/// <summary>������̥��Ĳ�Ʒ</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityPidsCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityPidsCacheResponse")]
        OperationResult<bool> UpdateActivityPidsCache(Guid activityId);
		/// <summary>����������̥</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptTires", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptTiresResponse")]
        OperationResult<List<VehicleAdaptTireTireSizeDetailModel>> SelectVehicleAaptTires(VehicleAdaptTireRequestModel request);
		///<summary>�Ż�ȯ��Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCarTagCouponConfigs", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCarTagCouponConfigsResponse")]
        OperationResult<IEnumerable<CarTagCouponConfigModel>> SelectCarTagCouponConfigs();
		///<summary>�������䱣��</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptBaoyangs", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAaptBaoyangsResponse")]
        OperationResult<IEnumerable<VehicleAdaptBaoyangModel>> SelectVehicleAaptBaoyangs(string vehicleId);
		///<summary>�������䳵Ʒ��Ϣ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAdaptChepins", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleAdaptChepinsResponse")]
        OperationResult<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>> SelectVehicleAdaptChepins(string vehicleId);
		///<summary>��ȡ��������̥���</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleSortedTireSizes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectVehicleSortedTireSizesResponse")]
        OperationResult<IEnumerable<VehicleSortedTireSizeModel>> SelectVehicleSortedTireSizes(string vehicleId);
		///<summary> �����û�������Ϣ������guid</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserShareInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserShareInfoResponse")]
        OperationResult<Guid> GetGuidAndInsertUserShareInfo(string pid, Guid batchGuid, Guid userId);
		///<summary> ����Guidȡ��д����е�����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserShareInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserShareInfoResponse")]
        OperationResult<ActivityUserShareInfoModel> GetActivityUserShareInfo(Guid shareId);
		///<summary> ���ݻID��ѯ�û���ȡ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPromotionPacketHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPromotionPacketHistoryResponse")]
        OperationResult<IEnumerable<PromotionPacketHistoryModel>> SelectPromotionPacketHistory(Guid userId, Guid luckyWheel);
		///<summary>�������ñ�id���û�idȡ�����ɵ���id������׬Ǯ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserForShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetGuidAndInsertUserForShareResponse")]
        OperationResult<Guid> GetGuidAndInsertUserForShare(Guid configGuid, Guid userId);
		///<summary>��ȡ���ñ��һ�����ݣ�����׬Ǯ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRecommendGetGiftConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRecommendGetGiftConfigResponse")]
        OperationResult<RecommendGetGiftConfigModel> FetchRecommendGetGiftConfig(Guid? number=null,Guid? userId=null);
		///<summary>��ѯ�����ȡ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPacketByUsers", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectPacketByUsersResponse")]
        OperationResult<DataTable> SelectPacketByUsers();
		///<summary>��ѯ��̥�</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityByActivityIdResponse")]
        OperationResult<TireActivityModel> SelectTireActivityByActivityId(Guid activityId);
		/// <summary>��ȡ�����ҳ��url</summary>/// <returns>��ڼ���ݵ���ȡ�ûҳ�����ӣ����򷵻��Ƿ���δ��ʼ���߹���</returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionActivityPageUrl", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionActivityPageUrlResponse")]
        OperationResult<RegionActivityPageModel> GetRegionActivityPageUrl(string city,string activityId);
		/// <summary>���ݻId�͵���Id���߳���Id��ȡĿ����ַ</summary>
[Obsolete("��ʹ��GetRegionVehicleIdActivityUrlNewAsync")]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrl", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlResponse")]
        OperationResult<ResultModel<string>> GetRegionVehicleIdActivityUrl(Guid activityId, int regionId, string vehicleId);
		/// <summary>���ݻId,������͵���Id���߳���Id��ȡĿ����ַ</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRegionVehicleIdActivityUrlNewResponse")]
        OperationResult<ResultModel<string>> GetRegionVehicleIdActivityUrlNew(Guid activityId, int regionId, string vehicleId, string activityChannel);
		/// <summary>�������</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionVehicleIdActivityUrlCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionVehicleIdActivityUrlCacheResponse")]
        OperationResult<bool> RefreshRegionVehicleIdActivityUrlCache(Guid activityId);
		///<summary>���ҳ��ѯ</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityConfigForDownloadApp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityConfigForDownloadAppResponse")]
        OperationResult<DownloadApp> GetActivityConfigForDownloadApp(int id);
		///<summary>������ҳ���ݻ���</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CleanActivityConfigForDownloadAppCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CleanActivityConfigForDownloadAppCacheResponse")]
        OperationResult<bool> CleanActivityConfigForDownloadAppCache(int id);
		///<summary>ȡ����֧ͬ���˻��Ķ��� -1:ʧ�� 1:ȡ���ɹ� 2:��ȡ������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CancelActivityOrderOfSamePaymentAccount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CancelActivityOrderOfSamePaymentAccountResponse")]
        OperationResult<int> CancelActivityOrderOfSamePaymentAccount(int orderId, string paymentAccount);
		///<summary>��ȡ�ҳ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelResponse")]
        OperationResult<ActivePageListModel> GetActivePageListModel(ActivtyPageRequest request);
		///<summary>��ȡ�ҳ����-zip</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelZip", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivePageListModelZipResponse")]
        OperationResult<Tuhu.Service.Activity.Zip.Models.ActivePageListModel> GetActivePageListModelZip(ActivtyPageRequest request);
		///<summary>��ȡ�����û��ɷ�������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckyWheelUserlotteryCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckyWheelUserlotteryCountResponse")]
        OperationResult<int> GetLuckyWheelUserlotteryCount(Guid userid, Guid userGroup,string hashkey=null);
		///<summary>���´����û��ɷ�������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateLuckyWheelUserlotteryCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateLuckyWheelUserlotteryCountResponse")]
        OperationResult<int> UpdateLuckyWheelUserlotteryCount(Guid userid, Guid userGroup,string hashkey=null);
		///<summary>ˢ�»ҳ����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivePageListModelCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivePageListModelCacheResponse")]
        OperationResult<bool> RefreshActivePageListModelCache(ActivtyPageRequest request);
		///<summary>ˢ�´�����������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshLuckWheelCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshLuckWheelCacheResponse")]
        OperationResult<bool> RefreshLuckWheelCache(string id);
		/// <summary>��֤��̥�����Ƿ��ܹ���</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationByTires", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationByTiresResponse")]
        OperationResult<VerificationTiresResponseModel> VerificationByTires(VerificationTiresRequestModel requestModel);
		/// <summary>������̥�µ���¼</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertTiresOrderRecordResponse")]
        OperationResult<bool> InsertTiresOrderRecord(TiresOrderRecordRequestModel requestModel);
		/// <summary>������̥�µ���¼</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RevokeTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RevokeTiresOrderRecordResponse")]
        OperationResult<bool> RevokeTiresOrderRecord(int orderId);
		/// <summary>Redis����̥������¼�ؽ�</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RedisTiresOrderRecordReconStruction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RedisTiresOrderRecordReconStructionResponse")]
        OperationResult<bool> RedisTiresOrderRecordReconStruction(TiresOrderRecordRequestModel requestModel);
		/// <summary>RedisAndSql����̥������¼��ѯ</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTiresOrderRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTiresOrderRecordResponse")]
        OperationResult<Dictionary<string, object>> SelectTiresOrderRecord(TiresOrderRecordRequestModel requestModel);
		/// <summary>��֤��̥�Ż�ȯ�Ƿ�����ȡ</summary>
			/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationTiresPromotionRule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VerificationTiresPromotionRuleResponse")]
        OperationResult<VerificationTiresResponseModel> VerificationTiresPromotionRule(VerificationTiresRequestModel requestModel,int ruleId);
		///<summary>��ȡ������������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckWheel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetLuckWheelResponse")]
        OperationResult<LuckyWheelModel> GetLuckWheel(string id);
		///<summary> ����׬Ǯ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectShareActivityProductById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectShareActivityProductByIdResponse")]
        OperationResult<ShareProductModel> SelectShareActivityProductById(string ProductId,string BatchGuid=null);
		///<summary>�����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectBaoYangActivitySetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectBaoYangActivitySettingResponse")]
        OperationResult<BaoYangActivitySetting> SelectBaoYangActivitySetting(string activityId);
		/// <summary></summary>///<returns></returns>
		[Obsolete("��ʹ��SelectCouponActivityConfigNew")]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigResponse")]
        OperationResult<CouponActivityConfigModel> SelectCouponActivityConfig(string activityNum, int type);
		///<summary>��ȡ�����</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectActivityTypeByActivityIds", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectActivityTypeByActivityIdsResponse")]
        OperationResult<IEnumerable<ActivityTypeModel>> SelectActivityTypeByActivityIds(List<Guid> activityIds);
		///<summary>���������ʻ�ȡ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityBuildWithSelKey", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityBuildWithSelKeyResponse")]
        OperationResult<ActivityBuild> GetActivityBuildWithSelKey(string keyword);
		///<summary>��¼�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityTypeLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityTypeLogResponse")]
        OperationResult<bool> RecordActivityTypeLog(ActivityTypeRequest request);
		///<summary>���±��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangActivityConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangActivityConfigResponse")]
        OperationResult<bool> UpdateBaoYangActivityConfig(Guid activityId);
		///<summary>��ȡ�����״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityStatusResponse")]
        OperationResult<FixedPriceActivityStatusResult> GetFixedPriceActivityStatus(Guid activityId, Guid userId, int regionId);
		///<summary>���û������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangPurchaseCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateBaoYangPurchaseCountResponse")]
        OperationResult<bool> UpdateBaoYangPurchaseCount(Guid activityId);
		///<summary>����activityId��ȡ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityRound", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetFixedPriceActivityRoundResponse")]
        OperationResult<FixedPriceActivityRoundResponse> GetFixedPriceActivityRound(Guid activityId);
		///<summary>����activityId��RegionId��ȡ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRegionTiresActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/FetchRegionTiresActivityResponse")]
        OperationResult<TiresActivityResponse> FetchRegionTiresActivity(FlashSaleTiresActivityRequest request);
		///<summary>ˢ����̥�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionTiresActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshRegionTiresActivityCacheResponse")]
        OperationResult<bool> RefreshRegionTiresActivityCache(Guid activityId, int regionId);
		///<summary>��¼�û�������ʼ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityProductUserRemindLog", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RecordActivityProductUserRemindLogResponse")]
        OperationResult<bool> RecordActivityProductUserRemindLog(ActivityProductUserRemindRequest request);
		///<summary>��ӷ��������¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordResponse")]
        OperationResult<bool> InsertRebateApplyRecord(RebateApplyRequest request);
		///<summary>��ʼ�����������ݻ��ߺ�������û�������״̬����ʹ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertOrUpdateActivityPageWhiteListRecords", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertOrUpdateActivityPageWhiteListRecordsResponse")]
        OperationResult<bool> InsertOrUpdateActivityPageWhiteListRecords(List<ActivityPageWhiteListRequest> requests);
		///<summary>����Userid�ж��Ƿ��ǰ������û�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageWhiteListByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageWhiteListByUserIdResponse")]
        OperationResult<bool> GetActivityPageWhiteListByUserId(Guid userId);
		///<summary>��¼;����̥���û�������Ϣ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutUserRewardApplication", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutUserRewardApplicationResponse")]
        OperationResult<UserRewardApplicationResponse> PutUserRewardApplication(UserRewardApplicationRequest request);
		///<summary>��ӷ��������¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordNewResponse")]
        OperationResult<ResultModel<bool>> InsertRebateApplyRecordNew(RebateApplyRequest request);
		///<summary>;������������¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutApplyCompensateRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutApplyCompensateRecordResponse")]
        OperationResult<bool> PutApplyCompensateRecord(ApplyCompensateRequest request);
		///<summary>�������Ч����֤�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivtyValidityResponses", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivtyValidityResponsesResponse")]
        OperationResult<List<ActivtyValidityResponse>> GetActivtyValidityResponses(ActivtyValidityRequest request);
		///<summary>��ȡԤ����������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCardSaleConfigDetails", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCardSaleConfigDetailsResponse")]
        OperationResult<List<VipCardSaleConfigDetailModel>> GetVipCardSaleConfigDetails(string activityId);
		///<summary>check����������Ƿ���ʣ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VipCardCheckStock", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/VipCardCheckStockResponse")]
        OperationResult<Dictionary<string, bool>> VipCardCheckStock(List<string> batchIds);
		///<summary>��������ʱ��¼����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutVipCardRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/PutVipCardRecordResponse")]
        OperationResult<bool> PutVipCardRecord(VipCardRecordRequest request);
		///<summary>֧���ɹ�ʱ���ð�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/BindVipCard", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/BindVipCardResponse")]
        OperationResult<bool> BindVipCard(int orderId);
		///<summary>��ȡ��������ҳ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyPageConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyPageConfigResponse")]
        OperationResult<RebateApplyPageConfig> SelectRebateApplyPageConfig();
		///<summary>���뷵��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordV2", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/InsertRebateApplyRecordV2Response")]
        OperationResult<ResultModel<bool>> InsertRebateApplyRecordV2(RebateApplyRequest request);
		///<summary>��ȡ�û����з���������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyByOpenId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectRebateApplyByOpenIdResponse")]
        OperationResult<List<RebateApplyResponse>> SelectRebateApplyByOpenId(string openId);
		///<summary>ͨ��OrderId��ȡ���µ������¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRebateApplyByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetRebateApplyByOrderIdResponse")]
        OperationResult<IEnumerable<RebateApplyResponse>> GetRebateApplyByOrderId(int orderId);
		///<summary>ȡ��������ʱ�����db�������е�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyVipCardRecordByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyVipCardRecordByOrderIdResponse")]
        OperationResult<bool> ModifyVipCardRecordByOrderId(int orderId);
		///<summary>��ȡ2018���籭�Ļ����ͻ��ֹ�����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetWorldCup2018Activity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetWorldCup2018ActivityResponse")]
        OperationResult<ActivityResponse> GetWorldCup2018Activity();
		///<summary>ͨ���û�ID��ȡ�һ�ȯ�����ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCouponCountByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCouponCountByUserIdResponse")]
        OperationResult<int> GetCouponCountByUserId(Guid userId, long activityId);
		///<summary>���ػ�һ�ȯ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchCouponRank", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchCouponRankResponse")]
        OperationResult<PagedModel<ActivityCouponRankResponse>> SearchCouponRank(long activityId, int pageIndex = 1, int pageSize = 20);
		///<summary>�����û��Ķһ�ȯ�������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponRank", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponRankResponse")]
        OperationResult<ActivityCouponRankResponse> GetUserCouponRank(Guid userId, long activityId);
		///<summary>�һ����б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeListResponse")]
        OperationResult<PagedModel<ActivityPrizeResponse>> SearchPrizeList(SearchPrizeListRequest searchPrizeListRequest);
		///<summary>�û��һ���Ʒ �쳣���룺-1 ϵͳ�쳣�������ԣ� , -2 �һ�������  -3 ��治��  -4 �Ѿ��¼�   -5 �Ѿ��һ�   -6 �һ�ʱ���Ѿ���ֹ���ܶһ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UserRedeemPrizes", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UserRedeemPrizesResponse")]
        OperationResult<bool> UserRedeemPrizes(Guid userId, long prizeId,long activityId);
		///<summary>�û��Ѷһ���Ʒ�б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeOrderDetailListByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchPrizeOrderDetailListByUserIdResponse")]
        OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>> SearchPrizeOrderDetailListByUserId(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20);
		///<summary>���վ�����Ŀ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionResponse")]
        OperationResult<IEnumerable<Models.Response.Question>> SearchQuestion( Guid userId , long activityId);
		///<summary>�ύ�û�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionAnswerResponse")]
        OperationResult<bool> SubmitQuestionAnswer(SubmitQuestionAnswerRequest submitQuestionAnswerRequest);
		///<summary>�����û�������ʷ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionAnswerHistoryByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SearchQuestionAnswerHistoryByUserIdResponse")]
        OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>> SearchQuestionAnswerHistoryByUserId(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest);
		///<summary>�����û�ʤ��������ʤ���ƺ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVictoryInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVictoryInfoResponse")]
        OperationResult<ActivityVictoryInfoResponse> GetVictoryInfo(Guid userId, long activityId);
		///<summary>��������ͻ���  �쳣��   -77 �δ��ʼ  -2 �����Ѿ�����   -1 ϵͳ�쳣</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityShareResponse")]
        OperationResult<bool> ActivityShare(ActivityShareDetailRequest shareDetailRequest);
		///<summary>�����Ƿ��Ѿ������� true = �����Ѿ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTodayAlreadyShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTodayAlreadyShareResponse")]
        OperationResult<bool> ActivityTodayAlreadyShare(Guid userId, long activityId);
		///<summary>������ȡ����ˢ������õĳ���������Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetOrSetActivityPageSortedPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetOrSetActivityPageSortedPidsResponse")]
        OperationResult<List<string>> GetOrSetActivityPageSortedPids(SortedPidsRequest request);
		///<summary>�޸Ļ��������û��һ�ȯ ����������־  ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyActivityCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyActivityCouponResponse")]
        OperationResult<long> ModifyActivityCoupon(Guid userId, long activityId, int couponCount , string couponName,DateTime? modifyDateTime = null);
		///<summary>ˢ�»��Ŀ  ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityQuestionCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityQuestionCacheResponse")]
        OperationResult<bool> RefreshActivityQuestionCache(long activityId);
		///<summary>ˢ�»�һ���  ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityPrizeCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/RefreshActivityPrizeCacheResponse")]
        OperationResult<bool> RefreshActivityPrizeCache(long activityId);
		///<summary>�����û��������ݵ����ݿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswer", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SubmitQuestionUserAnswerResponse")]
        OperationResult<SubmitActivityQuestionUserAnswerResponse> SubmitQuestionUserAnswer(SubmitActivityQuestionUserAnswerRequest request);
		///<summary>�����û�������״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ModifyQuestionUserAnswerResultResponse")]
        OperationResult<ModifyQuestionUserAnswerResultResponse> ModifyQuestionUserAnswerResult(ModifyQuestionUserAnswerResultRequest request);
		///<summary>�»ҳ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoConfigModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoConfigModelResponse")]
        OperationResult<ActivityPageInfoModel> GetActivityPageInfoConfigModel(ActivityPageInfoRequest request);
		///<summary>�ҳ�᳡����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoHomeModels", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoHomeModelsResponse")]
        OperationResult<List<ActivityPageInfoHomeModel>> GetActivityPageInfoHomeModels(string hashKey);
		///<summary>�ҳ��Ʒ�Ƽ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoCpRecommends", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoCpRecommendsResponse")]
        OperationResult<List<ActivityPageInfoRecommend>> GetActivityPageInfoCpRecommends(ActivityPageInfoModuleRecommendRequest request);
		///<summary>�ҳ��̥�Ƽ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoTireRecommends", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoTireRecommendsResponse")]
        OperationResult<List<ActivityPageInfoRecommend>> GetActivityPageInfoTireRecommends(ActivityPageInfoModuleRecommendRequest request);
		///<summary>�ҳ�˵�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowMenus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowMenusResponse")]
        OperationResult<List<ActivityPageInfoRowMenuModel>> GetActivityPageInfoRowMenus(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ����Ʒ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPools", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPoolsResponse")]
        OperationResult<List<ActivityPageInfoRowProductPool>> GetActivityPageInfoRowPools(ActivityPageInfoModuleProductPoolRequest request);
		///<summary>�ҳ����Ʒ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowNewPools", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowNewPoolsResponse")]
        OperationResult<List<ActivityPageInfoRowNewProductPool>> GetActivityPageInfoRowNewPools(ActivityPageInfoModuleNewProductPoolRequest request);
		///<summary>�ҳ����ʱ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCountDowns", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCountDownsResponse")]
        OperationResult<List<ActivityPageInfoRowCountDown>> GetActivityPageInfoRowCountDowns(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ�İ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowActivityTexts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowActivityTextsResponse")]
        OperationResult<List<ActivityPageInfoRowActivityText>> GetActivityPageInfoRowActivityTexts(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ�������������Ż�ȯ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowJsons", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowJsonsResponse")]
        OperationResult<List<ActivityPageInfoRowJson>> GetActivityPageInfoRowJsons(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳƴ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPintuans", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowPintuansResponse")]
        OperationResult<List<ActivityPageInfoRowPintuan>> GetActivityPageInfoRowPintuans(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��Ƶ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVideos", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVideosResponse")]
        OperationResult<List<ActivityPageInfoRowVideo>> GetActivityPageInfoRowVideos(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ���������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOtherActivitys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOtherActivitysResponse")]
        OperationResult<List<ActivityPageInfoRowOtherActivity>> GetActivityPageInfoRowOtherActivitys(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOthers", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowOthersResponse")]
        OperationResult<List<ActivityPageInfoRowOther>> GetActivityPageInfoRowOthers(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ���������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowRules", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowRulesResponse")]
        OperationResult<List<ActivityPageInfoRowRule>> GetActivityPageInfoRowRules(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowBys", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowBysResponse")]
        OperationResult<List<ActivityPageInfoRowBy>> GetActivityPageInfoRowBys(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ�Ż�ȯ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCoupons", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowCouponsResponse")]
        OperationResult<List<ActivityPageInfoRowCoupon>> GetActivityPageInfoRowCoupons(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��Ʒ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowProducts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowProductsResponse")]
        OperationResult<List<ActivityPageInfoRowProduct>> GetActivityPageInfoRowProducts(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowLinks", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowLinksResponse")]
        OperationResult<List<ActivityPageInfoRowLink>> GetActivityPageInfoRowLinks(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳͼƬ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowImages", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowImagesResponse")]
        OperationResult<List<ActivityPageInfoRowImage>> GetActivityPageInfoRowImages(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳ��ɱ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowSeckills", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowSeckillsResponse")]
        OperationResult<List<ActivityPageInfoRowSeckill>> GetActivityPageInfoRowSeckills(ActivityPageInfoModuleBaseRequest request);
		///<summary>�ҳͷͼ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVehicleBanners", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityPageInfoRowVehicleBannersResponse")]
        OperationResult<List<ActivityPageInfoRowVehicleBanner>> GetActivityPageInfoRowVehicleBanners(ActivityPageInfoModuleVehicleBannerRequest request);
		///<summary>ͨ������ͻ�ȡ� 0 ���籭  1 ƴ�ų�����֤</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityInfoByType", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityInfoByTypeResponse")]
        OperationResult<ActivityResponse> GetActivityInfoByType(int activityTypeId);
		///<summary>���ݻ��Ų�ѯ������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCustomerSettingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCustomerSettingInfoResponse")]
        OperationResult<ActiveCustomerSettingResponse> GetCustomerSettingInfo(string activeNo);
		///<summary>��ѯ�û��󶨵�ȯ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponCode", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetUserCouponCodeResponse")]
        OperationResult<string> GetUserCouponCode(string activityExclusiveId, string userid);
		///<summary>�û�ȯ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CouponCodeBound", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CouponCodeBoundResponse")]
        OperationResult<bool> CouponCodeBound(ActivityCustomerCouponRequests activityCustomerCouponRequests);
		///<summary>�ͻ�ר���µ���֤</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActiveOrderVerify", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActiveOrderVerifyResponse")]
        OperationResult<bool> ActiveOrderVerify(ActivityOrderVerifyRequests activityOrderVerifyRequests);
		///<summary>H5 ���;���Ǽ��ŵ���֤��Ϣ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddStarRatingStore", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddStarRatingStoreResponse")]
        OperationResult<bool> AddStarRatingStore(AddStarRatingStoreRequest request);
		///<summary>������ʱ����ID��ѯ��ͻ��������Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCustomerSettingInfoByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetVipCustomerSettingInfoByActivityIdResponse")]
        OperationResult<ActiveCustomerSettingResponse> GetVipCustomerSettingInfoByActivityId(string activityId);
		///<summary>��ȡ��ͻ�������Ա��̥�����쳣��Ϊ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetExceptionalCustomerOrderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetExceptionalCustomerOrderInfoResponse")]
        OperationResult<List<ActivityCustomerInvalidOrderResponse>> GetExceptionalCustomerOrderInfo();
		///<summary>��ȡ����/���Ϳ������</summary>///<returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectCouponActivityConfigNewResponse")]
        OperationResult<CouponActivityConfigNewModel> SelectCouponActivityConfigNew(CouponActivityConfigRequest request);
		///<summary>�����ӿڲ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTestMethod", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ActivityTestMethodResponse")]
        OperationResult<string> ActivityTestMethod(int testType);
		///<summary>��ӻ����ҳ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddRegistrationOfActivitiesData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddRegistrationOfActivitiesDataResponse")]
        OperationResult<bool> AddRegistrationOfActivitiesData(RegistrationOfActivitiesRequest request);
		///<summary>Ӷ����Ʒ�б��ѯ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductListResponse")]
        OperationResult<List<CommissionProductModel>> GetCommissionProductList(GetCommissionProductListRequest request);
		///<summary>Ӷ����Ʒ�����ѯ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductDetatils", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetCommissionProductDetatilsResponse")]
        OperationResult<CommissionProductModel> GetCommissionProductDetatils(GetCommissionProductDetatilsRequest request);
		///<summary>�µ���Ʒ��¼�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CreateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CreateOrderItemRecordResponse")]
        OperationResult<CreateOrderItemRecordResponse> CreateOrderItemRecord(CreateOrderItemRecordRequest request);
		///<summary>Ӷ�𶩵���Ʒ��¼���½ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateOrderItemRecordResponse")]
        OperationResult<UpdateOrderItemRecordResponse> UpdateOrderItemRecord(UpdateOrderItemRecordRequest request);
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityRebate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityRebateResponse")]
        OperationResult<CommodityRebateResponse> CommodityRebate(CommodityRebateRequest request);
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityDeduction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CommodityDeductionResponse")]
        OperationResult<CommodityDeductionResponse> CommodityDeduction(CommodityDeductionRequest request);
		///<summary>CPS�޸�Ӷ����ˮ״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CpsUpdateRunning", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CpsUpdateRunningResponse")]
        OperationResult<CpsUpdateRunningResponse> CpsUpdateRunning(CpsUpdateRunningRequest request);
	}

	/// <summary>�����</summary>
	public partial class ActivityClient : TuhuServiceClient<IActivityClient>, IActivityClient
    {
    	/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        public OperationResult<TireActivityModel> SelectTireActivity(string vehicleId, string tireSize) => Invoke(_ => _.SelectTireActivity(vehicleId,tireSize));

	/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        public Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize) => InvokeAsync(_ => _.SelectTireActivityAsync(vehicleId,tireSize));
		/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        public OperationResult<TireActivityModel> SelectTireActivityNew(TireActivityRequest request) => Invoke(_ => _.SelectTireActivityNew(request));

	/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        public Task<OperationResult<TireActivityModel>> SelectTireActivityNewAsync(TireActivityRequest request) => InvokeAsync(_ => _.SelectTireActivityNewAsync(request));
		/// <summary>��ѯ��̥��ת�</summary>/// <returns></returns>
        public OperationResult<string> SelectTireChangedActivity(TireActivityRequest request) => Invoke(_ => _.SelectTireChangedActivity(request));

	/// <summary>��ѯ��̥��ת�</summary>/// <returns></returns>
        public Task<OperationResult<string>> SelectTireChangedActivityAsync(TireActivityRequest request) => InvokeAsync(_ => _.SelectTireChangedActivityAsync(request));
		/// <summary>��ѯ��̥��Ĳ�Ʒ</summary>/// <returns></returns>
        public OperationResult<IEnumerable<string>> SelectTireActivityPids(Guid activityId) => Invoke(_ => _.SelectTireActivityPids(activityId));

	/// <summary>��ѯ��̥��Ĳ�Ʒ</summary>/// <returns></returns>
        public Task<OperationResult<IEnumerable<string>>> SelectTireActivityPidsAsync(Guid activityId) => InvokeAsync(_ => _.SelectTireActivityPidsAsync(activityId));
		/// <summary>������̥�����</summary>/// <returns></returns>
        public OperationResult<bool> UpdateTireActivityCache(string vehicleId, string tireSize) => Invoke(_ => _.UpdateTireActivityCache(vehicleId,tireSize));

	/// <summary>������̥�����</summary>/// <returns></returns>
        public Task<OperationResult<bool>> UpdateTireActivityCacheAsync(string vehicleId, string tireSize) => InvokeAsync(_ => _.UpdateTireActivityCacheAsync(vehicleId,tireSize));
		/// <summary>������̥��Ĳ�Ʒ</summary>/// <returns></returns>
        public OperationResult<bool> UpdateActivityPidsCache(Guid activityId) => Invoke(_ => _.UpdateActivityPidsCache(activityId));

	/// <summary>������̥��Ĳ�Ʒ</summary>/// <returns></returns>
        public Task<OperationResult<bool>> UpdateActivityPidsCacheAsync(Guid activityId) => InvokeAsync(_ => _.UpdateActivityPidsCacheAsync(activityId));
		/// <summary>����������̥</summary>/// <returns></returns>
        public OperationResult<List<VehicleAdaptTireTireSizeDetailModel>> SelectVehicleAaptTires(VehicleAdaptTireRequestModel request) => Invoke(_ => _.SelectVehicleAaptTires(request));

	/// <summary>����������̥</summary>/// <returns></returns>
        public Task<OperationResult<List<VehicleAdaptTireTireSizeDetailModel>>> SelectVehicleAaptTiresAsync(VehicleAdaptTireRequestModel request) => InvokeAsync(_ => _.SelectVehicleAaptTiresAsync(request));
		///<summary>�Ż�ȯ��Ϣ</summary>///<returns></returns>
        public OperationResult<IEnumerable<CarTagCouponConfigModel>> SelectCarTagCouponConfigs() => Invoke(_ => _.SelectCarTagCouponConfigs());

	///<summary>�Ż�ȯ��Ϣ</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<CarTagCouponConfigModel>>> SelectCarTagCouponConfigsAsync() => InvokeAsync(_ => _.SelectCarTagCouponConfigsAsync());
		///<summary>�������䱣��</summary>///<returns></returns>
        public OperationResult<IEnumerable<VehicleAdaptBaoyangModel>> SelectVehicleAaptBaoyangs(string vehicleId) => Invoke(_ => _.SelectVehicleAaptBaoyangs(vehicleId));

	///<summary>�������䱣��</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<VehicleAdaptBaoyangModel>>> SelectVehicleAaptBaoyangsAsync(string vehicleId) => InvokeAsync(_ => _.SelectVehicleAaptBaoyangsAsync(vehicleId));
		///<summary>�������䳵Ʒ��Ϣ</summary>///<returns></returns>
        public OperationResult<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>> SelectVehicleAdaptChepins(string vehicleId) => Invoke(_ => _.SelectVehicleAdaptChepins(vehicleId));

	///<summary>�������䳵Ʒ��Ϣ</summary>///<returns></returns>
        public Task<OperationResult<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>>> SelectVehicleAdaptChepinsAsync(string vehicleId) => InvokeAsync(_ => _.SelectVehicleAdaptChepinsAsync(vehicleId));
		///<summary>��ȡ��������̥���</summary>///<returns></returns>
        public OperationResult<IEnumerable<VehicleSortedTireSizeModel>> SelectVehicleSortedTireSizes(string vehicleId) => Invoke(_ => _.SelectVehicleSortedTireSizes(vehicleId));

	///<summary>��ȡ��������̥���</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<VehicleSortedTireSizeModel>>> SelectVehicleSortedTireSizesAsync(string vehicleId) => InvokeAsync(_ => _.SelectVehicleSortedTireSizesAsync(vehicleId));
		///<summary> �����û�������Ϣ������guid</summary>///<returns></returns>
        public OperationResult<Guid> GetGuidAndInsertUserShareInfo(string pid, Guid batchGuid, Guid userId) => Invoke(_ => _.GetGuidAndInsertUserShareInfo(pid,batchGuid,userId));

	///<summary> �����û�������Ϣ������guid</summary>///<returns></returns>
        public Task<OperationResult<Guid>> GetGuidAndInsertUserShareInfoAsync(string pid, Guid batchGuid, Guid userId) => InvokeAsync(_ => _.GetGuidAndInsertUserShareInfoAsync(pid,batchGuid,userId));
		///<summary> ����Guidȡ��д����е�����</summary>///<returns></returns>
        public OperationResult<ActivityUserShareInfoModel> GetActivityUserShareInfo(Guid shareId) => Invoke(_ => _.GetActivityUserShareInfo(shareId));

	///<summary> ����Guidȡ��д����е�����</summary>///<returns></returns>
        public Task<OperationResult<ActivityUserShareInfoModel>> GetActivityUserShareInfoAsync(Guid shareId) => InvokeAsync(_ => _.GetActivityUserShareInfoAsync(shareId));
		///<summary> ���ݻID��ѯ�û���ȡ����</summary>///<returns></returns>
        public OperationResult<IEnumerable<PromotionPacketHistoryModel>> SelectPromotionPacketHistory(Guid userId, Guid luckyWheel) => Invoke(_ => _.SelectPromotionPacketHistory(userId,luckyWheel));

	///<summary> ���ݻID��ѯ�û���ȡ����</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<PromotionPacketHistoryModel>>> SelectPromotionPacketHistoryAsync(Guid userId, Guid luckyWheel) => InvokeAsync(_ => _.SelectPromotionPacketHistoryAsync(userId,luckyWheel));
		///<summary>�������ñ�id���û�idȡ�����ɵ���id������׬Ǯ����</summary>///<returns></returns>
        public OperationResult<Guid> GetGuidAndInsertUserForShare(Guid configGuid, Guid userId) => Invoke(_ => _.GetGuidAndInsertUserForShare(configGuid,userId));

	///<summary>�������ñ�id���û�idȡ�����ɵ���id������׬Ǯ����</summary>///<returns></returns>
        public Task<OperationResult<Guid>> GetGuidAndInsertUserForShareAsync(Guid configGuid, Guid userId) => InvokeAsync(_ => _.GetGuidAndInsertUserForShareAsync(configGuid,userId));
		///<summary>��ȡ���ñ��һ�����ݣ�����׬Ǯ����</summary>///<returns></returns>
        public OperationResult<RecommendGetGiftConfigModel> FetchRecommendGetGiftConfig(Guid? number=null,Guid? userId=null) => Invoke(_ => _.FetchRecommendGetGiftConfig(number,userId));

	///<summary>��ȡ���ñ��һ�����ݣ�����׬Ǯ����</summary>///<returns></returns>
        public Task<OperationResult<RecommendGetGiftConfigModel>> FetchRecommendGetGiftConfigAsync(Guid? number=null,Guid? userId=null) => InvokeAsync(_ => _.FetchRecommendGetGiftConfigAsync(number,userId));
		///<summary>��ѯ�����ȡ</summary>///<returns></returns>
        public OperationResult<DataTable> SelectPacketByUsers() => Invoke(_ => _.SelectPacketByUsers());

	///<summary>��ѯ�����ȡ</summary>///<returns></returns>
        public Task<OperationResult<DataTable>> SelectPacketByUsersAsync() => InvokeAsync(_ => _.SelectPacketByUsersAsync());
		///<summary>��ѯ��̥�</summary>///<returns></returns>
        public OperationResult<TireActivityModel> SelectTireActivityByActivityId(Guid activityId) => Invoke(_ => _.SelectTireActivityByActivityId(activityId));

	///<summary>��ѯ��̥�</summary>///<returns></returns>
        public Task<OperationResult<TireActivityModel>> SelectTireActivityByActivityIdAsync(Guid activityId) => InvokeAsync(_ => _.SelectTireActivityByActivityIdAsync(activityId));
		/// <summary>��ȡ�����ҳ��url</summary>/// <returns>��ڼ���ݵ���ȡ�ûҳ�����ӣ����򷵻��Ƿ���δ��ʼ���߹���</returns>
        public OperationResult<RegionActivityPageModel> GetRegionActivityPageUrl(string city,string activityId) => Invoke(_ => _.GetRegionActivityPageUrl(city,activityId));

	/// <summary>��ȡ�����ҳ��url</summary>/// <returns>��ڼ���ݵ���ȡ�ûҳ�����ӣ����򷵻��Ƿ���δ��ʼ���߹���</returns>
        public Task<OperationResult<RegionActivityPageModel>> GetRegionActivityPageUrlAsync(string city,string activityId) => InvokeAsync(_ => _.GetRegionActivityPageUrlAsync(city,activityId));
		/// <summary>���ݻId�͵���Id���߳���Id��ȡĿ����ַ</summary>
[Obsolete("��ʹ��GetRegionVehicleIdActivityUrlNewAsync")]
        public OperationResult<ResultModel<string>> GetRegionVehicleIdActivityUrl(Guid activityId, int regionId, string vehicleId) => Invoke(_ => _.GetRegionVehicleIdActivityUrl(activityId, regionId, vehicleId));

	/// <summary>���ݻId�͵���Id���߳���Id��ȡĿ����ַ</summary>
[Obsolete("��ʹ��GetRegionVehicleIdActivityUrlNewAsync")]
        public Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlAsync(Guid activityId, int regionId, string vehicleId) => InvokeAsync(_ => _.GetRegionVehicleIdActivityUrlAsync(activityId, regionId, vehicleId));
		/// <summary>���ݻId,������͵���Id���߳���Id��ȡĿ����ַ</summary>/// <returns></returns>
        public OperationResult<ResultModel<string>> GetRegionVehicleIdActivityUrlNew(Guid activityId, int regionId, string vehicleId, string activityChannel) => Invoke(_ => _.GetRegionVehicleIdActivityUrlNew(activityId, regionId, vehicleId, activityChannel));

	/// <summary>���ݻId,������͵���Id���߳���Id��ȡĿ����ַ</summary>/// <returns></returns>
        public Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlNewAsync(Guid activityId, int regionId, string vehicleId, string activityChannel) => InvokeAsync(_ => _.GetRegionVehicleIdActivityUrlNewAsync(activityId, regionId, vehicleId, activityChannel));
		/// <summary>�������</summary>/// <returns></returns>
        public OperationResult<bool> RefreshRegionVehicleIdActivityUrlCache(Guid activityId) => Invoke(_ => _.RefreshRegionVehicleIdActivityUrlCache(activityId));

	/// <summary>�������</summary>/// <returns></returns>
        public Task<OperationResult<bool>> RefreshRegionVehicleIdActivityUrlCacheAsync(Guid activityId) => InvokeAsync(_ => _.RefreshRegionVehicleIdActivityUrlCacheAsync(activityId));
		///<summary>���ҳ��ѯ</summary>///<returns></returns>
        public OperationResult<DownloadApp> GetActivityConfigForDownloadApp(int id) => Invoke(_ => _.GetActivityConfigForDownloadApp(id));

	///<summary>���ҳ��ѯ</summary>///<returns></returns>
        public Task<OperationResult<DownloadApp>> GetActivityConfigForDownloadAppAsync(int id) => InvokeAsync(_ => _.GetActivityConfigForDownloadAppAsync(id));
		///<summary>������ҳ���ݻ���</summary>///<returns></returns>
        public OperationResult<bool> CleanActivityConfigForDownloadAppCache(int id) => Invoke(_ => _.CleanActivityConfigForDownloadAppCache(id));

	///<summary>������ҳ���ݻ���</summary>///<returns></returns>
        public Task<OperationResult<bool>> CleanActivityConfigForDownloadAppCacheAsync(int id) => InvokeAsync(_ => _.CleanActivityConfigForDownloadAppCacheAsync(id));
		///<summary>ȡ����֧ͬ���˻��Ķ��� -1:ʧ�� 1:ȡ���ɹ� 2:��ȡ������</summary>///<returns></returns>
        public OperationResult<int> CancelActivityOrderOfSamePaymentAccount(int orderId, string paymentAccount) => Invoke(_ => _.CancelActivityOrderOfSamePaymentAccount(orderId,paymentAccount));

	///<summary>ȡ����֧ͬ���˻��Ķ��� -1:ʧ�� 1:ȡ���ɹ� 2:��ȡ������</summary>///<returns></returns>
        public Task<OperationResult<int>> CancelActivityOrderOfSamePaymentAccountAsync(int orderId, string paymentAccount) => InvokeAsync(_ => _.CancelActivityOrderOfSamePaymentAccountAsync(orderId,paymentAccount));
		///<summary>��ȡ�ҳ����</summary>///<returns></returns>
        public OperationResult<ActivePageListModel> GetActivePageListModel(ActivtyPageRequest request) => Invoke(_ => _.GetActivePageListModel(request));

	///<summary>��ȡ�ҳ����</summary>///<returns></returns>
        public Task<OperationResult<ActivePageListModel>> GetActivePageListModelAsync(ActivtyPageRequest request) => InvokeAsync(_ => _.GetActivePageListModelAsync(request));
		///<summary>��ȡ�ҳ����-zip</summary>///<returns></returns>
        public OperationResult<Tuhu.Service.Activity.Zip.Models.ActivePageListModel> GetActivePageListModelZip(ActivtyPageRequest request) => Invoke(_ => _.GetActivePageListModelZip(request));

	///<summary>��ȡ�ҳ����-zip</summary>///<returns></returns>
        public Task<OperationResult<Tuhu.Service.Activity.Zip.Models.ActivePageListModel>> GetActivePageListModelZipAsync(ActivtyPageRequest request) => InvokeAsync(_ => _.GetActivePageListModelZipAsync(request));
		///<summary>��ȡ�����û��ɷ�������</summary>///<returns></returns>
        public OperationResult<int> GetLuckyWheelUserlotteryCount(Guid userid, Guid userGroup,string hashkey=null) => Invoke(_ => _.GetLuckyWheelUserlotteryCount(userid,userGroup,hashkey));

	///<summary>��ȡ�����û��ɷ�������</summary>///<returns></returns>
        public Task<OperationResult<int>> GetLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup,string hashkey=null) => InvokeAsync(_ => _.GetLuckyWheelUserlotteryCountAsync(userid,userGroup,hashkey));
		///<summary>���´����û��ɷ�������</summary>///<returns></returns>
        public OperationResult<int> UpdateLuckyWheelUserlotteryCount(Guid userid, Guid userGroup,string hashkey=null) => Invoke(_ => _.UpdateLuckyWheelUserlotteryCount(userid,userGroup,hashkey));

	///<summary>���´����û��ɷ�������</summary>///<returns></returns>
        public Task<OperationResult<int>> UpdateLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup,string hashkey=null) => InvokeAsync(_ => _.UpdateLuckyWheelUserlotteryCountAsync(userid,userGroup,hashkey));
		///<summary>ˢ�»ҳ����</summary>///<returns></returns>
        public OperationResult<bool> RefreshActivePageListModelCache(ActivtyPageRequest request) => Invoke(_ => _.RefreshActivePageListModelCache(request));

	///<summary>ˢ�»ҳ����</summary>///<returns></returns>
        public Task<OperationResult<bool>> RefreshActivePageListModelCacheAsync(ActivtyPageRequest request) => InvokeAsync(_ => _.RefreshActivePageListModelCacheAsync(request));
		///<summary>ˢ�´�����������</summary>///<returns></returns>
        public OperationResult<bool> RefreshLuckWheelCache(string id) => Invoke(_ => _.RefreshLuckWheelCache(id));

	///<summary>ˢ�´�����������</summary>///<returns></returns>
        public Task<OperationResult<bool>> RefreshLuckWheelCacheAsync(string id) => InvokeAsync(_ => _.RefreshLuckWheelCacheAsync(id));
		/// <summary>��֤��̥�����Ƿ��ܹ���</summary>
			/// <returns></returns>
        public OperationResult<VerificationTiresResponseModel> VerificationByTires(VerificationTiresRequestModel requestModel) => Invoke(_ => _.VerificationByTires(requestModel));

	/// <summary>��֤��̥�����Ƿ��ܹ���</summary>
			/// <returns></returns>
        public Task<OperationResult<VerificationTiresResponseModel>> VerificationByTiresAsync(VerificationTiresRequestModel requestModel) => InvokeAsync(_ => _.VerificationByTiresAsync(requestModel));
		/// <summary>������̥�µ���¼</summary>
			/// <returns></returns>
        public OperationResult<bool> InsertTiresOrderRecord(TiresOrderRecordRequestModel requestModel) => Invoke(_ => _.InsertTiresOrderRecord(requestModel));

	/// <summary>������̥�µ���¼</summary>
			/// <returns></returns>
        public Task<OperationResult<bool>> InsertTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel) => InvokeAsync(_ => _.InsertTiresOrderRecordAsync(requestModel));
		/// <summary>������̥�µ���¼</summary>
			/// <returns></returns>
        public OperationResult<bool> RevokeTiresOrderRecord(int orderId) => Invoke(_ => _.RevokeTiresOrderRecord(orderId));

	/// <summary>������̥�µ���¼</summary>
			/// <returns></returns>
        public Task<OperationResult<bool>> RevokeTiresOrderRecordAsync(int orderId) => InvokeAsync(_ => _.RevokeTiresOrderRecordAsync(orderId));
		/// <summary>Redis����̥������¼�ؽ�</summary>
			/// <returns></returns>
        public OperationResult<bool> RedisTiresOrderRecordReconStruction(TiresOrderRecordRequestModel requestModel) => Invoke(_ => _.RedisTiresOrderRecordReconStruction(requestModel));

	/// <summary>Redis����̥������¼�ؽ�</summary>
			/// <returns></returns>
        public Task<OperationResult<bool>> RedisTiresOrderRecordReconStructionAsync(TiresOrderRecordRequestModel requestModel) => InvokeAsync(_ => _.RedisTiresOrderRecordReconStructionAsync(requestModel));
		/// <summary>RedisAndSql����̥������¼��ѯ</summary>
			/// <returns></returns>
        public OperationResult<Dictionary<string, object>> SelectTiresOrderRecord(TiresOrderRecordRequestModel requestModel) => Invoke(_ => _.SelectTiresOrderRecord(requestModel));

	/// <summary>RedisAndSql����̥������¼��ѯ</summary>
			/// <returns></returns>
        public Task<OperationResult<Dictionary<string, object>>> SelectTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel) => InvokeAsync(_ => _.SelectTiresOrderRecordAsync(requestModel));
		/// <summary>��֤��̥�Ż�ȯ�Ƿ�����ȡ</summary>
			/// <returns></returns>
        public OperationResult<VerificationTiresResponseModel> VerificationTiresPromotionRule(VerificationTiresRequestModel requestModel,int ruleId) => Invoke(_ => _.VerificationTiresPromotionRule(requestModel,ruleId));

	/// <summary>��֤��̥�Ż�ȯ�Ƿ�����ȡ</summary>
			/// <returns></returns>
        public Task<OperationResult<VerificationTiresResponseModel>> VerificationTiresPromotionRuleAsync(VerificationTiresRequestModel requestModel,int ruleId) => InvokeAsync(_ => _.VerificationTiresPromotionRuleAsync(requestModel,ruleId));
		///<summary>��ȡ������������</summary>///<returns></returns>
        public OperationResult<LuckyWheelModel> GetLuckWheel(string id) => Invoke(_ => _.GetLuckWheel(id));

	///<summary>��ȡ������������</summary>///<returns></returns>
        public Task<OperationResult<LuckyWheelModel>> GetLuckWheelAsync(string id) => InvokeAsync(_ => _.GetLuckWheelAsync(id));
		///<summary> ����׬Ǯ </summary>
        public OperationResult<ShareProductModel> SelectShareActivityProductById(string ProductId,string BatchGuid=null) => Invoke(_ => _.SelectShareActivityProductById(ProductId,BatchGuid));

	///<summary> ����׬Ǯ </summary>
        public Task<OperationResult<ShareProductModel>> SelectShareActivityProductByIdAsync(string ProductId,string BatchGuid=null) => InvokeAsync(_ => _.SelectShareActivityProductByIdAsync(ProductId,BatchGuid));
		///<summary>�����</summary>///<returns></returns>
        public OperationResult<BaoYangActivitySetting> SelectBaoYangActivitySetting(string activityId) => Invoke(_ => _.SelectBaoYangActivitySetting(activityId));

	///<summary>�����</summary>///<returns></returns>
        public Task<OperationResult<BaoYangActivitySetting>> SelectBaoYangActivitySettingAsync(string activityId) => InvokeAsync(_ => _.SelectBaoYangActivitySettingAsync(activityId));
		/// <summary></summary>///<returns></returns>
		[Obsolete("��ʹ��SelectCouponActivityConfigNew")]
        public OperationResult<CouponActivityConfigModel> SelectCouponActivityConfig(string activityNum, int type) => Invoke(_ => _.SelectCouponActivityConfig(activityNum,type));

	/// <summary></summary>///<returns></returns>
		[Obsolete("��ʹ��SelectCouponActivityConfigNew")]
        public Task<OperationResult<CouponActivityConfigModel>> SelectCouponActivityConfigAsync(string activityNum, int type) => InvokeAsync(_ => _.SelectCouponActivityConfigAsync(activityNum,type));
		///<summary>��ȡ�����</summary>///<returns></returns>
        public OperationResult<IEnumerable<ActivityTypeModel>> SelectActivityTypeByActivityIds(List<Guid> activityIds) => Invoke(_ => _.SelectActivityTypeByActivityIds(activityIds));

	///<summary>��ȡ�����</summary>///<returns></returns>
        public Task<OperationResult<IEnumerable<ActivityTypeModel>>> SelectActivityTypeByActivityIdsAsync(List<Guid> activityIds) => InvokeAsync(_ => _.SelectActivityTypeByActivityIdsAsync(activityIds));
		///<summary>���������ʻ�ȡ�����</summary>
        public OperationResult<ActivityBuild> GetActivityBuildWithSelKey(string keyword) => Invoke(_ => _.GetActivityBuildWithSelKey(keyword));

	///<summary>���������ʻ�ȡ�����</summary>
        public Task<OperationResult<ActivityBuild>> GetActivityBuildWithSelKeyAsync(string keyword) => InvokeAsync(_ => _.GetActivityBuildWithSelKeyAsync(keyword));
		///<summary>��¼�����</summary>
        public OperationResult<bool> RecordActivityTypeLog(ActivityTypeRequest request) => Invoke(_ => _.RecordActivityTypeLog(request));

	///<summary>��¼�����</summary>
        public Task<OperationResult<bool>> RecordActivityTypeLogAsync(ActivityTypeRequest request) => InvokeAsync(_ => _.RecordActivityTypeLogAsync(request));
		///<summary>���±��������</summary>
        public OperationResult<bool> UpdateBaoYangActivityConfig(Guid activityId) => Invoke(_ => _.UpdateBaoYangActivityConfig(activityId));

	///<summary>���±��������</summary>
        public Task<OperationResult<bool>> UpdateBaoYangActivityConfigAsync(Guid activityId) => InvokeAsync(_ => _.UpdateBaoYangActivityConfigAsync(activityId));
		///<summary>��ȡ�����״̬</summary>
        public OperationResult<FixedPriceActivityStatusResult> GetFixedPriceActivityStatus(Guid activityId, Guid userId, int regionId) => Invoke(_ => _.GetFixedPriceActivityStatus(activityId, userId, regionId));

	///<summary>��ȡ�����״̬</summary>
        public Task<OperationResult<FixedPriceActivityStatusResult>> GetFixedPriceActivityStatusAsync(Guid activityId, Guid userId, int regionId) => InvokeAsync(_ => _.GetFixedPriceActivityStatusAsync(activityId, userId, regionId));
		///<summary>���û������</summary>
        public OperationResult<bool> UpdateBaoYangPurchaseCount(Guid activityId) => Invoke(_ => _.UpdateBaoYangPurchaseCount(activityId));

	///<summary>���û������</summary>
        public Task<OperationResult<bool>> UpdateBaoYangPurchaseCountAsync(Guid activityId) => InvokeAsync(_ => _.UpdateBaoYangPurchaseCountAsync(activityId));
		///<summary>����activityId��ȡ�����</summary>
        public OperationResult<FixedPriceActivityRoundResponse> GetFixedPriceActivityRound(Guid activityId) => Invoke(_ => _.GetFixedPriceActivityRound(activityId));

	///<summary>����activityId��ȡ�����</summary>
        public Task<OperationResult<FixedPriceActivityRoundResponse>> GetFixedPriceActivityRoundAsync(Guid activityId) => InvokeAsync(_ => _.GetFixedPriceActivityRoundAsync(activityId));
		///<summary>����activityId��RegionId��ȡ�����</summary>
        public OperationResult<TiresActivityResponse> FetchRegionTiresActivity(FlashSaleTiresActivityRequest request) => Invoke(_ => _.FetchRegionTiresActivity(request));

	///<summary>����activityId��RegionId��ȡ�����</summary>
        public Task<OperationResult<TiresActivityResponse>> FetchRegionTiresActivityAsync(FlashSaleTiresActivityRequest request) => InvokeAsync(_ => _.FetchRegionTiresActivityAsync(request));
		///<summary>ˢ����̥�����</summary>
        public OperationResult<bool> RefreshRegionTiresActivityCache(Guid activityId, int regionId) => Invoke(_ => _.RefreshRegionTiresActivityCache(activityId, regionId));

	///<summary>ˢ����̥�����</summary>
        public Task<OperationResult<bool>> RefreshRegionTiresActivityCacheAsync(Guid activityId, int regionId) => InvokeAsync(_ => _.RefreshRegionTiresActivityCacheAsync(activityId, regionId));
		///<summary>��¼�û�������ʼ����</summary>
        public OperationResult<bool> RecordActivityProductUserRemindLog(ActivityProductUserRemindRequest request) => Invoke(_ => _.RecordActivityProductUserRemindLog(request));

	///<summary>��¼�û�������ʼ����</summary>
        public Task<OperationResult<bool>> RecordActivityProductUserRemindLogAsync(ActivityProductUserRemindRequest request) => InvokeAsync(_ => _.RecordActivityProductUserRemindLogAsync(request));
		///<summary>��ӷ��������¼</summary>
        public OperationResult<bool> InsertRebateApplyRecord(RebateApplyRequest request) => Invoke(_ => _.InsertRebateApplyRecord(request));

	///<summary>��ӷ��������¼</summary>
        public Task<OperationResult<bool>> InsertRebateApplyRecordAsync(RebateApplyRequest request) => InvokeAsync(_ => _.InsertRebateApplyRecordAsync(request));
		///<summary>��ʼ�����������ݻ��ߺ�������û�������״̬����ʹ��</summary>
        public OperationResult<bool> InsertOrUpdateActivityPageWhiteListRecords(List<ActivityPageWhiteListRequest> requests) => Invoke(_ => _.InsertOrUpdateActivityPageWhiteListRecords(requests));

	///<summary>��ʼ�����������ݻ��ߺ�������û�������״̬����ʹ��</summary>
        public Task<OperationResult<bool>> InsertOrUpdateActivityPageWhiteListRecordsAsync(List<ActivityPageWhiteListRequest> requests) => InvokeAsync(_ => _.InsertOrUpdateActivityPageWhiteListRecordsAsync(requests));
		///<summary>����Userid�ж��Ƿ��ǰ������û�</summary>
        public OperationResult<bool> GetActivityPageWhiteListByUserId(Guid userId) => Invoke(_ => _.GetActivityPageWhiteListByUserId(userId));

	///<summary>����Userid�ж��Ƿ��ǰ������û�</summary>
        public Task<OperationResult<bool>> GetActivityPageWhiteListByUserIdAsync(Guid userId) => InvokeAsync(_ => _.GetActivityPageWhiteListByUserIdAsync(userId));
		///<summary>��¼;����̥���û�������Ϣ�ӿ�</summary>
        public OperationResult<UserRewardApplicationResponse> PutUserRewardApplication(UserRewardApplicationRequest request) => Invoke(_ => _.PutUserRewardApplication(request));

	///<summary>��¼;����̥���û�������Ϣ�ӿ�</summary>
        public Task<OperationResult<UserRewardApplicationResponse>> PutUserRewardApplicationAsync(UserRewardApplicationRequest request) => InvokeAsync(_ => _.PutUserRewardApplicationAsync(request));
		///<summary>��ӷ��������¼</summary>
        public OperationResult<ResultModel<bool>> InsertRebateApplyRecordNew(RebateApplyRequest request) => Invoke(_ => _.InsertRebateApplyRecordNew(request));

	///<summary>��ӷ��������¼</summary>
        public Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordNewAsync(RebateApplyRequest request) => InvokeAsync(_ => _.InsertRebateApplyRecordNewAsync(request));
		///<summary>;������������¼</summary>
        public OperationResult<bool> PutApplyCompensateRecord(ApplyCompensateRequest request) => Invoke(_ => _.PutApplyCompensateRecord(request));

	///<summary>;������������¼</summary>
        public Task<OperationResult<bool>> PutApplyCompensateRecordAsync(ApplyCompensateRequest request) => InvokeAsync(_ => _.PutApplyCompensateRecordAsync(request));
		///<summary>�������Ч����֤�ӿ�</summary>
        public OperationResult<List<ActivtyValidityResponse>> GetActivtyValidityResponses(ActivtyValidityRequest request) => Invoke(_ => _.GetActivtyValidityResponses(request));

	///<summary>�������Ч����֤�ӿ�</summary>
        public Task<OperationResult<List<ActivtyValidityResponse>>> GetActivtyValidityResponsesAsync(ActivtyValidityRequest request) => InvokeAsync(_ => _.GetActivtyValidityResponsesAsync(request));
		///<summary>��ȡԤ����������Ϣ</summary>
        public OperationResult<List<VipCardSaleConfigDetailModel>> GetVipCardSaleConfigDetails(string activityId) => Invoke(_ => _.GetVipCardSaleConfigDetails(activityId));

	///<summary>��ȡԤ����������Ϣ</summary>
        public Task<OperationResult<List<VipCardSaleConfigDetailModel>>> GetVipCardSaleConfigDetailsAsync(string activityId) => InvokeAsync(_ => _.GetVipCardSaleConfigDetailsAsync(activityId));
		///<summary>check����������Ƿ���ʣ����</summary>
        public OperationResult<Dictionary<string, bool>> VipCardCheckStock(List<string> batchIds) => Invoke(_ => _.VipCardCheckStock(batchIds));

	///<summary>check����������Ƿ���ʣ����</summary>
        public Task<OperationResult<Dictionary<string, bool>>> VipCardCheckStockAsync(List<string> batchIds) => InvokeAsync(_ => _.VipCardCheckStockAsync(batchIds));
		///<summary>��������ʱ��¼����Ϣ</summary>
        public OperationResult<bool> PutVipCardRecord(VipCardRecordRequest request) => Invoke(_ => _.PutVipCardRecord(request));

	///<summary>��������ʱ��¼����Ϣ</summary>
        public Task<OperationResult<bool>> PutVipCardRecordAsync(VipCardRecordRequest request) => InvokeAsync(_ => _.PutVipCardRecordAsync(request));
		///<summary>֧���ɹ�ʱ���ð�</summary>
        public OperationResult<bool> BindVipCard(int orderId) => Invoke(_ => _.BindVipCard(orderId));

	///<summary>֧���ɹ�ʱ���ð�</summary>
        public Task<OperationResult<bool>> BindVipCardAsync(int orderId) => InvokeAsync(_ => _.BindVipCardAsync(orderId));
		///<summary>��ȡ��������ҳ������</summary>
        public OperationResult<RebateApplyPageConfig> SelectRebateApplyPageConfig() => Invoke(_ => _.SelectRebateApplyPageConfig());

	///<summary>��ȡ��������ҳ������</summary>
        public Task<OperationResult<RebateApplyPageConfig>> SelectRebateApplyPageConfigAsync() => InvokeAsync(_ => _.SelectRebateApplyPageConfigAsync());
		///<summary>���뷵��</summary>
        public OperationResult<ResultModel<bool>> InsertRebateApplyRecordV2(RebateApplyRequest request) => Invoke(_ => _.InsertRebateApplyRecordV2(request));

	///<summary>���뷵��</summary>
        public Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordV2Async(RebateApplyRequest request) => InvokeAsync(_ => _.InsertRebateApplyRecordV2Async(request));
		///<summary>��ȡ�û����з���������Ϣ</summary>
        public OperationResult<List<RebateApplyResponse>> SelectRebateApplyByOpenId(string openId) => Invoke(_ => _.SelectRebateApplyByOpenId(openId));

	///<summary>��ȡ�û����з���������Ϣ</summary>
        public Task<OperationResult<List<RebateApplyResponse>>> SelectRebateApplyByOpenIdAsync(string openId) => InvokeAsync(_ => _.SelectRebateApplyByOpenIdAsync(openId));
		///<summary>ͨ��OrderId��ȡ���µ������¼</summary>
        public OperationResult<IEnumerable<RebateApplyResponse>> GetRebateApplyByOrderId(int orderId) => Invoke(_ => _.GetRebateApplyByOrderId(orderId));

	///<summary>ͨ��OrderId��ȡ���µ������¼</summary>
        public Task<OperationResult<IEnumerable<RebateApplyResponse>>> GetRebateApplyByOrderIdAsync(int orderId) => InvokeAsync(_ => _.GetRebateApplyByOrderIdAsync(orderId));
		///<summary>ȡ��������ʱ�����db�������е�����</summary>
        public OperationResult<bool> ModifyVipCardRecordByOrderId(int orderId) => Invoke(_ => _.ModifyVipCardRecordByOrderId(orderId));

	///<summary>ȡ��������ʱ�����db�������е�����</summary>
        public Task<OperationResult<bool>> ModifyVipCardRecordByOrderIdAsync(int orderId) => InvokeAsync(_ => _.ModifyVipCardRecordByOrderIdAsync(orderId));
		///<summary>��ȡ2018���籭�Ļ����ͻ��ֹ�����Ϣ</summary>
        public OperationResult<ActivityResponse> GetWorldCup2018Activity() => Invoke(_ => _.GetWorldCup2018Activity( ));

	///<summary>��ȡ2018���籭�Ļ����ͻ��ֹ�����Ϣ</summary>
        public Task<OperationResult<ActivityResponse>> GetWorldCup2018ActivityAsync() => InvokeAsync(_ => _.GetWorldCup2018ActivityAsync( ));
		///<summary>ͨ���û�ID��ȡ�һ�ȯ�����ӿ�</summary>
        public OperationResult<int> GetCouponCountByUserId(Guid userId, long activityId) => Invoke(_ => _.GetCouponCountByUserId(userId,activityId));

	///<summary>ͨ���û�ID��ȡ�һ�ȯ�����ӿ�</summary>
        public Task<OperationResult<int>> GetCouponCountByUserIdAsync(Guid userId, long activityId) => InvokeAsync(_ => _.GetCouponCountByUserIdAsync(userId,activityId));
		///<summary>���ػ�һ�ȯ��������</summary>
        public OperationResult<PagedModel<ActivityCouponRankResponse>> SearchCouponRank(long activityId, int pageIndex = 1, int pageSize = 20) => Invoke(_ => _.SearchCouponRank( activityId,pageIndex,pageSize ));

	///<summary>���ػ�һ�ȯ��������</summary>
        public Task<OperationResult<PagedModel<ActivityCouponRankResponse>>> SearchCouponRankAsync(long activityId, int pageIndex = 1, int pageSize = 20) => InvokeAsync(_ => _.SearchCouponRankAsync( activityId,pageIndex,pageSize ));
		///<summary>�����û��Ķһ�ȯ�������</summary>
        public OperationResult<ActivityCouponRankResponse> GetUserCouponRank(Guid userId, long activityId) => Invoke(_ => _.GetUserCouponRank( userId,activityId ));

	///<summary>�����û��Ķһ�ȯ�������</summary>
        public Task<OperationResult<ActivityCouponRankResponse>> GetUserCouponRankAsync(Guid userId, long activityId) => InvokeAsync(_ => _.GetUserCouponRankAsync( userId,activityId ));
		///<summary>�һ����б�</summary>
        public OperationResult<PagedModel<ActivityPrizeResponse>> SearchPrizeList(SearchPrizeListRequest searchPrizeListRequest) => Invoke(_ => _.SearchPrizeList( searchPrizeListRequest ));

	///<summary>�һ����б�</summary>
        public Task<OperationResult<PagedModel<ActivityPrizeResponse>>> SearchPrizeListAsync(SearchPrizeListRequest searchPrizeListRequest) => InvokeAsync(_ => _.SearchPrizeListAsync( searchPrizeListRequest ));
		///<summary>�û��һ���Ʒ �쳣���룺-1 ϵͳ�쳣�������ԣ� , -2 �һ�������  -3 ��治��  -4 �Ѿ��¼�   -5 �Ѿ��һ�   -6 �һ�ʱ���Ѿ���ֹ���ܶһ�</summary>
        public OperationResult<bool> UserRedeemPrizes(Guid userId, long prizeId,long activityId) => Invoke(_ => _.UserRedeemPrizes( userId,prizeId,activityId ));

	///<summary>�û��һ���Ʒ �쳣���룺-1 ϵͳ�쳣�������ԣ� , -2 �һ�������  -3 ��治��  -4 �Ѿ��¼�   -5 �Ѿ��һ�   -6 �һ�ʱ���Ѿ���ֹ���ܶһ�</summary>
        public Task<OperationResult<bool>> UserRedeemPrizesAsync(Guid userId, long prizeId,long activityId) => InvokeAsync(_ => _.UserRedeemPrizesAsync( userId,prizeId,activityId ));
		///<summary>�û��Ѷһ���Ʒ�б�</summary>
        public OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>> SearchPrizeOrderDetailListByUserId(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20) => Invoke(_ => _.SearchPrizeOrderDetailListByUserId( userId,activityId,pageIndex,pageSize ));

	///<summary>�û��Ѷһ���Ʒ�б�</summary>
        public Task<OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>>> SearchPrizeOrderDetailListByUserIdAsync(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20) => InvokeAsync(_ => _.SearchPrizeOrderDetailListByUserIdAsync( userId,activityId,pageIndex,pageSize ));
		///<summary>���վ�����Ŀ</summary>
        public OperationResult<IEnumerable<Models.Response.Question>> SearchQuestion( Guid userId , long activityId) => Invoke(_ => _.SearchQuestion( userId,activityId ));

	///<summary>���վ�����Ŀ</summary>
        public Task<OperationResult<IEnumerable<Models.Response.Question>>> SearchQuestionAsync( Guid userId , long activityId) => InvokeAsync(_ => _.SearchQuestionAsync( userId,activityId ));
		///<summary>�ύ�û�����</summary>
        public OperationResult<bool> SubmitQuestionAnswer(SubmitQuestionAnswerRequest submitQuestionAnswerRequest) => Invoke(_ => _.SubmitQuestionAnswer( submitQuestionAnswerRequest ));

	///<summary>�ύ�û�����</summary>
        public Task<OperationResult<bool>> SubmitQuestionAnswerAsync(SubmitQuestionAnswerRequest submitQuestionAnswerRequest) => InvokeAsync(_ => _.SubmitQuestionAnswerAsync( submitQuestionAnswerRequest ));
		///<summary>�����û�������ʷ</summary>
        public OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>> SearchQuestionAnswerHistoryByUserId(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest) => Invoke(_ => _.SearchQuestionAnswerHistoryByUserId( searchQuestionAnswerHistoryRequest ));

	///<summary>�����û�������ʷ</summary>
        public Task<OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>>> SearchQuestionAnswerHistoryByUserIdAsync(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest) => InvokeAsync(_ => _.SearchQuestionAnswerHistoryByUserIdAsync( searchQuestionAnswerHistoryRequest ));
		///<summary>�����û�ʤ��������ʤ���ƺ�</summary>
        public OperationResult<ActivityVictoryInfoResponse> GetVictoryInfo(Guid userId, long activityId) => Invoke(_ => _.GetVictoryInfo( userId,activityId ));

	///<summary>�����û�ʤ��������ʤ���ƺ�</summary>
        public Task<OperationResult<ActivityVictoryInfoResponse>> GetVictoryInfoAsync(Guid userId, long activityId) => InvokeAsync(_ => _.GetVictoryInfoAsync( userId,activityId ));
		///<summary>��������ͻ���  �쳣��   -77 �δ��ʼ  -2 �����Ѿ�����   -1 ϵͳ�쳣</summary>
        public OperationResult<bool> ActivityShare(ActivityShareDetailRequest shareDetailRequest) => Invoke(_ => _.ActivityShare( shareDetailRequest ));

	///<summary>��������ͻ���  �쳣��   -77 �δ��ʼ  -2 �����Ѿ�����   -1 ϵͳ�쳣</summary>
        public Task<OperationResult<bool>> ActivityShareAsync(ActivityShareDetailRequest shareDetailRequest) => InvokeAsync(_ => _.ActivityShareAsync( shareDetailRequest ));
		///<summary>�����Ƿ��Ѿ������� true = �����Ѿ�����</summary>
        public OperationResult<bool> ActivityTodayAlreadyShare(Guid userId, long activityId) => Invoke(_ => _.ActivityTodayAlreadyShare( userId, activityId));

	///<summary>�����Ƿ��Ѿ������� true = �����Ѿ�����</summary>
        public Task<OperationResult<bool>> ActivityTodayAlreadyShareAsync(Guid userId, long activityId) => InvokeAsync(_ => _.ActivityTodayAlreadyShareAsync( userId, activityId));
		///<summary>������ȡ����ˢ������õĳ���������Ʒ</summary>
        public OperationResult<List<string>> GetOrSetActivityPageSortedPids(SortedPidsRequest request) => Invoke(_ => _.GetOrSetActivityPageSortedPids(request));

	///<summary>������ȡ����ˢ������õĳ���������Ʒ</summary>
        public Task<OperationResult<List<string>>> GetOrSetActivityPageSortedPidsAsync(SortedPidsRequest request) => InvokeAsync(_ => _.GetOrSetActivityPageSortedPidsAsync(request));
		///<summary>�޸Ļ��������û��һ�ȯ ����������־  ��������</summary>
        public OperationResult<long> ModifyActivityCoupon(Guid userId, long activityId, int couponCount , string couponName,DateTime? modifyDateTime = null) => Invoke(_ => _.ModifyActivityCoupon( userId, activityId,couponCount,couponName,modifyDateTime));

	///<summary>�޸Ļ��������û��һ�ȯ ����������־  ��������</summary>
        public Task<OperationResult<long>> ModifyActivityCouponAsync(Guid userId, long activityId, int couponCount , string couponName,DateTime? modifyDateTime = null) => InvokeAsync(_ => _.ModifyActivityCouponAsync( userId, activityId,couponCount,couponName,modifyDateTime));
		///<summary>ˢ�»��Ŀ  ����</summary>
        public OperationResult<bool> RefreshActivityQuestionCache(long activityId) => Invoke(_ => _.RefreshActivityQuestionCache( activityId));

	///<summary>ˢ�»��Ŀ  ����</summary>
        public Task<OperationResult<bool>> RefreshActivityQuestionCacheAsync(long activityId) => InvokeAsync(_ => _.RefreshActivityQuestionCacheAsync( activityId));
		///<summary>ˢ�»�һ���  ����</summary>
        public OperationResult<bool> RefreshActivityPrizeCache(long activityId) => Invoke(_ => _.RefreshActivityPrizeCache( activityId));

	///<summary>ˢ�»�һ���  ����</summary>
        public Task<OperationResult<bool>> RefreshActivityPrizeCacheAsync(long activityId) => InvokeAsync(_ => _.RefreshActivityPrizeCacheAsync( activityId));
		///<summary>�����û��������ݵ����ݿ�</summary>
        public OperationResult<SubmitActivityQuestionUserAnswerResponse> SubmitQuestionUserAnswer(SubmitActivityQuestionUserAnswerRequest request) => Invoke(_ => _.SubmitQuestionUserAnswer( request));

	///<summary>�����û��������ݵ����ݿ�</summary>
        public Task<OperationResult<SubmitActivityQuestionUserAnswerResponse>> SubmitQuestionUserAnswerAsync(SubmitActivityQuestionUserAnswerRequest request) => InvokeAsync(_ => _.SubmitQuestionUserAnswerAsync( request));
		///<summary>�����û�������״̬</summary>
        public OperationResult<ModifyQuestionUserAnswerResultResponse> ModifyQuestionUserAnswerResult(ModifyQuestionUserAnswerResultRequest request) => Invoke(_ => _.ModifyQuestionUserAnswerResult( request));

	///<summary>�����û�������״̬</summary>
        public Task<OperationResult<ModifyQuestionUserAnswerResultResponse>> ModifyQuestionUserAnswerResultAsync(ModifyQuestionUserAnswerResultRequest request) => InvokeAsync(_ => _.ModifyQuestionUserAnswerResultAsync( request));
		///<summary>�»ҳ����</summary>
        public OperationResult<ActivityPageInfoModel> GetActivityPageInfoConfigModel(ActivityPageInfoRequest request) => Invoke(_ => _.GetActivityPageInfoConfigModel( request));

	///<summary>�»ҳ����</summary>
        public Task<OperationResult<ActivityPageInfoModel>> GetActivityPageInfoConfigModelAsync(ActivityPageInfoRequest request) => InvokeAsync(_ => _.GetActivityPageInfoConfigModelAsync( request));
		///<summary>�ҳ�᳡����</summary>
        public OperationResult<List<ActivityPageInfoHomeModel>> GetActivityPageInfoHomeModels(string hashKey) => Invoke(_ => _.GetActivityPageInfoHomeModels( hashKey));

	///<summary>�ҳ�᳡����</summary>
        public Task<OperationResult<List<ActivityPageInfoHomeModel>>> GetActivityPageInfoHomeModelsAsync(string hashKey) => InvokeAsync(_ => _.GetActivityPageInfoHomeModelsAsync( hashKey));
		///<summary>�ҳ��Ʒ�Ƽ�</summary>
        public OperationResult<List<ActivityPageInfoRecommend>> GetActivityPageInfoCpRecommends(ActivityPageInfoModuleRecommendRequest request) => Invoke(_ => _.GetActivityPageInfoCpRecommends( request));

	///<summary>�ҳ��Ʒ�Ƽ�</summary>
        public Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoCpRecommendsAsync(ActivityPageInfoModuleRecommendRequest request) => InvokeAsync(_ => _.GetActivityPageInfoCpRecommendsAsync( request));
		///<summary>�ҳ��̥�Ƽ�</summary>
        public OperationResult<List<ActivityPageInfoRecommend>> GetActivityPageInfoTireRecommends(ActivityPageInfoModuleRecommendRequest request) => Invoke(_ => _.GetActivityPageInfoTireRecommends( request));

	///<summary>�ҳ��̥�Ƽ�</summary>
        public Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoTireRecommendsAsync(ActivityPageInfoModuleRecommendRequest request) => InvokeAsync(_ => _.GetActivityPageInfoTireRecommendsAsync( request));
		///<summary>�ҳ�˵�����</summary>
        public OperationResult<List<ActivityPageInfoRowMenuModel>> GetActivityPageInfoRowMenus(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowMenus( request));

	///<summary>�ҳ�˵�����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowMenuModel>>> GetActivityPageInfoRowMenusAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowMenusAsync( request));
		///<summary>�ҳ����Ʒ������</summary>
        public OperationResult<List<ActivityPageInfoRowProductPool>> GetActivityPageInfoRowPools(ActivityPageInfoModuleProductPoolRequest request) => Invoke(_ => _.GetActivityPageInfoRowPools( request));

	///<summary>�ҳ����Ʒ������</summary>
        public Task<OperationResult<List<ActivityPageInfoRowProductPool>>> GetActivityPageInfoRowPoolsAsync(ActivityPageInfoModuleProductPoolRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowPoolsAsync( request));
		///<summary>�ҳ����Ʒ������</summary>
        public OperationResult<List<ActivityPageInfoRowNewProductPool>> GetActivityPageInfoRowNewPools(ActivityPageInfoModuleNewProductPoolRequest request) => Invoke(_ => _.GetActivityPageInfoRowNewPools( request));

	///<summary>�ҳ����Ʒ������</summary>
        public Task<OperationResult<List<ActivityPageInfoRowNewProductPool>>> GetActivityPageInfoRowNewPoolsAsync(ActivityPageInfoModuleNewProductPoolRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowNewPoolsAsync( request));
		///<summary>�ҳ����ʱ����</summary>
        public OperationResult<List<ActivityPageInfoRowCountDown>> GetActivityPageInfoRowCountDowns(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowCountDowns( request));

	///<summary>�ҳ����ʱ����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowCountDown>>> GetActivityPageInfoRowCountDownsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowCountDownsAsync( request));
		///<summary>�ҳ�İ�����</summary>
        public OperationResult<List<ActivityPageInfoRowActivityText>> GetActivityPageInfoRowActivityTexts(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowActivityTexts( request));

	///<summary>�ҳ�İ�����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowActivityText>>> GetActivityPageInfoRowActivityTextsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowActivityTextsAsync( request));
		///<summary>�ҳ�������������Ż�ȯ����</summary>
        public OperationResult<List<ActivityPageInfoRowJson>> GetActivityPageInfoRowJsons(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowJsons( request));

	///<summary>�ҳ�������������Ż�ȯ����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowJson>>> GetActivityPageInfoRowJsonsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowJsonsAsync( request));
		///<summary>�ҳƴ������</summary>
        public OperationResult<List<ActivityPageInfoRowPintuan>> GetActivityPageInfoRowPintuans(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowPintuans( request));

	///<summary>�ҳƴ������</summary>
        public Task<OperationResult<List<ActivityPageInfoRowPintuan>>> GetActivityPageInfoRowPintuansAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowPintuansAsync( request));
		///<summary>�ҳ��Ƶ����</summary>
        public OperationResult<List<ActivityPageInfoRowVideo>> GetActivityPageInfoRowVideos(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowVideos( request));

	///<summary>�ҳ��Ƶ����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowVideo>>> GetActivityPageInfoRowVideosAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowVideosAsync( request));
		///<summary>�ҳ���������</summary>
        public OperationResult<List<ActivityPageInfoRowOtherActivity>> GetActivityPageInfoRowOtherActivitys(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowOtherActivitys( request));

	///<summary>�ҳ���������</summary>
        public Task<OperationResult<List<ActivityPageInfoRowOtherActivity>>> GetActivityPageInfoRowOtherActivitysAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowOtherActivitysAsync( request));
		///<summary>�ҳ��������</summary>
        public OperationResult<List<ActivityPageInfoRowOther>> GetActivityPageInfoRowOthers(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowOthers( request));

	///<summary>�ҳ��������</summary>
        public Task<OperationResult<List<ActivityPageInfoRowOther>>> GetActivityPageInfoRowOthersAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowOthersAsync( request));
		///<summary>�ҳ���������</summary>
        public OperationResult<List<ActivityPageInfoRowRule>> GetActivityPageInfoRowRules(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowRules( request));

	///<summary>�ҳ���������</summary>
        public Task<OperationResult<List<ActivityPageInfoRowRule>>> GetActivityPageInfoRowRulesAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowRulesAsync( request));
		///<summary>�ҳ��������</summary>
        public OperationResult<List<ActivityPageInfoRowBy>> GetActivityPageInfoRowBys(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowBys( request));

	///<summary>�ҳ��������</summary>
        public Task<OperationResult<List<ActivityPageInfoRowBy>>> GetActivityPageInfoRowBysAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowBysAsync( request));
		///<summary>�ҳ�Ż�ȯ����</summary>
        public OperationResult<List<ActivityPageInfoRowCoupon>> GetActivityPageInfoRowCoupons(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowCoupons( request));

	///<summary>�ҳ�Ż�ȯ����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowCoupon>>> GetActivityPageInfoRowCouponsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowCouponsAsync( request));
		///<summary>�ҳ��Ʒ����</summary>
        public OperationResult<List<ActivityPageInfoRowProduct>> GetActivityPageInfoRowProducts(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowProducts( request));

	///<summary>�ҳ��Ʒ����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowProduct>>> GetActivityPageInfoRowProductsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowProductsAsync( request));
		///<summary>�ҳ��������</summary>
        public OperationResult<List<ActivityPageInfoRowLink>> GetActivityPageInfoRowLinks(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowLinks( request));

	///<summary>�ҳ��������</summary>
        public Task<OperationResult<List<ActivityPageInfoRowLink>>> GetActivityPageInfoRowLinksAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowLinksAsync( request));
		///<summary>�ҳͼƬ����</summary>
        public OperationResult<List<ActivityPageInfoRowImage>> GetActivityPageInfoRowImages(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowImages( request));

	///<summary>�ҳͼƬ����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowImage>>> GetActivityPageInfoRowImagesAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowImagesAsync( request));
		///<summary>�ҳ��ɱ����</summary>
        public OperationResult<List<ActivityPageInfoRowSeckill>> GetActivityPageInfoRowSeckills(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowSeckills( request));

	///<summary>�ҳ��ɱ����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowSeckill>>> GetActivityPageInfoRowSeckillsAsync(ActivityPageInfoModuleBaseRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowSeckillsAsync( request));
		///<summary>�ҳͷͼ����</summary>
        public OperationResult<List<ActivityPageInfoRowVehicleBanner>> GetActivityPageInfoRowVehicleBanners(ActivityPageInfoModuleVehicleBannerRequest request) => Invoke(_ => _.GetActivityPageInfoRowVehicleBanners( request));

	///<summary>�ҳͷͼ����</summary>
        public Task<OperationResult<List<ActivityPageInfoRowVehicleBanner>>> GetActivityPageInfoRowVehicleBannersAsync(ActivityPageInfoModuleVehicleBannerRequest request) => InvokeAsync(_ => _.GetActivityPageInfoRowVehicleBannersAsync( request));
		///<summary>ͨ������ͻ�ȡ� 0 ���籭  1 ƴ�ų�����֤</summary>
        public OperationResult<ActivityResponse> GetActivityInfoByType(int activityTypeId) => Invoke(_ => _.GetActivityInfoByType(activityTypeId));

	///<summary>ͨ������ͻ�ȡ� 0 ���籭  1 ƴ�ų�����֤</summary>
        public Task<OperationResult<ActivityResponse>> GetActivityInfoByTypeAsync(int activityTypeId) => InvokeAsync(_ => _.GetActivityInfoByTypeAsync(activityTypeId));
		///<summary>���ݻ��Ų�ѯ������Ϣ</summary>
        public OperationResult<ActiveCustomerSettingResponse> GetCustomerSettingInfo(string activeNo) => Invoke(_ => _.GetCustomerSettingInfo(activeNo));

	///<summary>���ݻ��Ų�ѯ������Ϣ</summary>
        public Task<OperationResult<ActiveCustomerSettingResponse>> GetCustomerSettingInfoAsync(string activeNo) => InvokeAsync(_ => _.GetCustomerSettingInfoAsync(activeNo));
		///<summary>��ѯ�û��󶨵�ȯ��</summary>
        public OperationResult<string> GetUserCouponCode(string activityExclusiveId, string userid) => Invoke(_ => _.GetUserCouponCode(activityExclusiveId,userid));

	///<summary>��ѯ�û��󶨵�ȯ��</summary>
        public Task<OperationResult<string>> GetUserCouponCodeAsync(string activityExclusiveId, string userid) => InvokeAsync(_ => _.GetUserCouponCodeAsync(activityExclusiveId,userid));
		///<summary>�û�ȯ���</summary>
        public OperationResult<bool> CouponCodeBound(ActivityCustomerCouponRequests activityCustomerCouponRequests) => Invoke(_ => _.CouponCodeBound(activityCustomerCouponRequests));

	///<summary>�û�ȯ���</summary>
        public Task<OperationResult<bool>> CouponCodeBoundAsync(ActivityCustomerCouponRequests activityCustomerCouponRequests) => InvokeAsync(_ => _.CouponCodeBoundAsync(activityCustomerCouponRequests));
		///<summary>�ͻ�ר���µ���֤</summary>
        public OperationResult<bool> ActiveOrderVerify(ActivityOrderVerifyRequests activityOrderVerifyRequests) => Invoke(_ => _.ActiveOrderVerify(activityOrderVerifyRequests));

	///<summary>�ͻ�ר���µ���֤</summary>
        public Task<OperationResult<bool>> ActiveOrderVerifyAsync(ActivityOrderVerifyRequests activityOrderVerifyRequests) => InvokeAsync(_ => _.ActiveOrderVerifyAsync(activityOrderVerifyRequests));
		///<summary>H5 ���;���Ǽ��ŵ���֤��Ϣ����</summary>
        public OperationResult<bool> AddStarRatingStore(AddStarRatingStoreRequest request) => Invoke(_ => _.AddStarRatingStore( request));

	///<summary>H5 ���;���Ǽ��ŵ���֤��Ϣ����</summary>
        public Task<OperationResult<bool>> AddStarRatingStoreAsync(AddStarRatingStoreRequest request) => InvokeAsync(_ => _.AddStarRatingStoreAsync( request));
		///<summary>������ʱ����ID��ѯ��ͻ��������Ϣ</summary>
        public OperationResult<ActiveCustomerSettingResponse> GetVipCustomerSettingInfoByActivityId(string activityId) => Invoke(_ => _.GetVipCustomerSettingInfoByActivityId(activityId));

	///<summary>������ʱ����ID��ѯ��ͻ��������Ϣ</summary>
        public Task<OperationResult<ActiveCustomerSettingResponse>> GetVipCustomerSettingInfoByActivityIdAsync(string activityId) => InvokeAsync(_ => _.GetVipCustomerSettingInfoByActivityIdAsync(activityId));
		///<summary>��ȡ��ͻ�������Ա��̥�����쳣��Ϊ</summary>
        public OperationResult<List<ActivityCustomerInvalidOrderResponse>> GetExceptionalCustomerOrderInfo() => Invoke(_ => _.GetExceptionalCustomerOrderInfo());

	///<summary>��ȡ��ͻ�������Ա��̥�����쳣��Ϊ</summary>
        public Task<OperationResult<List<ActivityCustomerInvalidOrderResponse>>> GetExceptionalCustomerOrderInfoAsync() => InvokeAsync(_ => _.GetExceptionalCustomerOrderInfoAsync());
		///<summary>��ȡ����/���Ϳ������</summary>///<returns></returns>
        public OperationResult<CouponActivityConfigNewModel> SelectCouponActivityConfigNew(CouponActivityConfigRequest request) => Invoke(_ => _.SelectCouponActivityConfigNew(request));

	///<summary>��ȡ����/���Ϳ������</summary>///<returns></returns>
        public Task<OperationResult<CouponActivityConfigNewModel>> SelectCouponActivityConfigNewAsync(CouponActivityConfigRequest request) => InvokeAsync(_ => _.SelectCouponActivityConfigNewAsync(request));
		///<summary>�����ӿڲ���</summary>
        public OperationResult<string> ActivityTestMethod(int testType) => Invoke(_ => _.ActivityTestMethod(testType));

	///<summary>�����ӿڲ���</summary>
        public Task<OperationResult<string>> ActivityTestMethodAsync(int testType) => InvokeAsync(_ => _.ActivityTestMethodAsync(testType));
		///<summary>��ӻ����ҳ����</summary>
        public OperationResult<bool> AddRegistrationOfActivitiesData(RegistrationOfActivitiesRequest request) => Invoke(_ => _.AddRegistrationOfActivitiesData(request));

	///<summary>��ӻ����ҳ����</summary>
        public Task<OperationResult<bool>> AddRegistrationOfActivitiesDataAsync(RegistrationOfActivitiesRequest request) => InvokeAsync(_ => _.AddRegistrationOfActivitiesDataAsync(request));
		///<summary>Ӷ����Ʒ�б��ѯ�ӿ�</summary>
        public OperationResult<List<CommissionProductModel>> GetCommissionProductList(GetCommissionProductListRequest request) => Invoke(_ => _.GetCommissionProductList(request));

	///<summary>Ӷ����Ʒ�б��ѯ�ӿ�</summary>
        public Task<OperationResult<List<CommissionProductModel>>> GetCommissionProductListAsync(GetCommissionProductListRequest request) => InvokeAsync(_ => _.GetCommissionProductListAsync(request));
		///<summary>Ӷ����Ʒ�����ѯ�ӿ�</summary>
        public OperationResult<CommissionProductModel> GetCommissionProductDetatils(GetCommissionProductDetatilsRequest request) => Invoke(_ => _.GetCommissionProductDetatils(request));

	///<summary>Ӷ����Ʒ�����ѯ�ӿ�</summary>
        public Task<OperationResult<CommissionProductModel>> GetCommissionProductDetatilsAsync(GetCommissionProductDetatilsRequest request) => InvokeAsync(_ => _.GetCommissionProductDetatilsAsync(request));
		///<summary>�µ���Ʒ��¼�ӿ�</summary>
        public OperationResult<CreateOrderItemRecordResponse> CreateOrderItemRecord(CreateOrderItemRecordRequest request) => Invoke(_ => _.CreateOrderItemRecord(request));

	///<summary>�µ���Ʒ��¼�ӿ�</summary>
        public Task<OperationResult<CreateOrderItemRecordResponse>> CreateOrderItemRecordAsync(CreateOrderItemRecordRequest request) => InvokeAsync(_ => _.CreateOrderItemRecordAsync(request));
		///<summary>Ӷ�𶩵���Ʒ��¼���½ӿ�</summary>
        public OperationResult<UpdateOrderItemRecordResponse> UpdateOrderItemRecord(UpdateOrderItemRecordRequest request) => Invoke(_ => _.UpdateOrderItemRecord(request));

	///<summary>Ӷ�𶩵���Ʒ��¼���½ӿ�</summary>
        public Task<OperationResult<UpdateOrderItemRecordResponse>> UpdateOrderItemRecordAsync(UpdateOrderItemRecordRequest request) => InvokeAsync(_ => _.UpdateOrderItemRecordAsync(request));
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        public OperationResult<CommodityRebateResponse> CommodityRebate(CommodityRebateRequest request) => Invoke(_ => _.CommodityRebate(request));

	///<summary>������Ʒ��Ӷ�ӿ�</summary>
        public Task<OperationResult<CommodityRebateResponse>> CommodityRebateAsync(CommodityRebateRequest request) => InvokeAsync(_ => _.CommodityRebateAsync(request));
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        public OperationResult<CommodityDeductionResponse> CommodityDeduction(CommodityDeductionRequest request) => Invoke(_ => _.CommodityDeduction(request));

	///<summary>������Ʒ��Ӷ�ӿ�</summary>
        public Task<OperationResult<CommodityDeductionResponse>> CommodityDeductionAsync(CommodityDeductionRequest request) => InvokeAsync(_ => _.CommodityDeductionAsync(request));
		///<summary>CPS�޸�Ӷ����ˮ״̬</summary>
        public OperationResult<CpsUpdateRunningResponse> CpsUpdateRunning(CpsUpdateRunningRequest request) => Invoke(_ => _.CpsUpdateRunning(request));

	///<summary>CPS�޸�Ӷ����ˮ״̬</summary>
        public Task<OperationResult<CpsUpdateRunningResponse>> CpsUpdateRunningAsync(CpsUpdateRunningRequest request) => InvokeAsync(_ => _.CpsUpdateRunningAsync(request));
	}
	/// <summary>�������</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICacheService
    {
    	///<summary>����վ���Redis����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RemoveRedisCacheKey", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RemoveRedisCacheKeyResponse")]
        Task<OperationResult<bool>> RemoveRedisCacheKeyAsync(string cacheName,string cacheKey,string prefixKey=null);
		///<summary>���ݻid����setredis����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshVipCardCacheByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshVipCardCacheByActivityIdResponse")]
        Task<OperationResult<bool>> RefreshVipCardCacheByActivityIdAsync(string activityId);
		///<summary>ˢ�»���ǰ׺�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCachePrefixForCommon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCachePrefixForCommonResponse")]
        Task<OperationResult<bool>> RefreshRedisCachePrefixForCommonAsync(RefreshCachePrefixRequest request);
		///<summary>ˢ�¿ͻ�ר�����û���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCacheCustomerSetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCacheCustomerSettingResponse")]
        Task<OperationResult<bool>> RefreshRedisCacheCustomerSettingAsync(string activityExclusiveId);
	}

	/// <summary>�������</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICacheClient : ICacheService, ITuhuServiceClient
    {
    	///<summary>����վ���Redis����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RemoveRedisCacheKey", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RemoveRedisCacheKeyResponse")]
        OperationResult<bool> RemoveRedisCacheKey(string cacheName,string cacheKey,string prefixKey=null);
		///<summary>���ݻid����setredis����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshVipCardCacheByActivityId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshVipCardCacheByActivityIdResponse")]
        OperationResult<bool> RefreshVipCardCacheByActivityId(string activityId);
		///<summary>ˢ�»���ǰ׺�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCachePrefixForCommon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCachePrefixForCommonResponse")]
        OperationResult<bool> RefreshRedisCachePrefixForCommon(RefreshCachePrefixRequest request);
		///<summary>ˢ�¿ͻ�ר�����û���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCacheCustomerSetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Cache/RefreshRedisCacheCustomerSettingResponse")]
        OperationResult<bool> RefreshRedisCacheCustomerSetting(string activityExclusiveId);
	}

	/// <summary>�������</summary>
	public partial class CacheClient : TuhuServiceClient<ICacheClient>, ICacheClient
    {
    	///<summary>����վ���Redis����</summary>
        public OperationResult<bool> RemoveRedisCacheKey(string cacheName,string cacheKey,string prefixKey=null) => Invoke(_ => _.RemoveRedisCacheKey(cacheName,cacheKey,prefixKey));

	///<summary>����վ���Redis����</summary>
        public Task<OperationResult<bool>> RemoveRedisCacheKeyAsync(string cacheName,string cacheKey,string prefixKey=null) => InvokeAsync(_ => _.RemoveRedisCacheKeyAsync(cacheName,cacheKey,prefixKey));
		///<summary>���ݻid����setredis����</summary>
        public OperationResult<bool> RefreshVipCardCacheByActivityId(string activityId) => Invoke(_ => _.RefreshVipCardCacheByActivityId(activityId));

	///<summary>���ݻid����setredis����</summary>
        public Task<OperationResult<bool>> RefreshVipCardCacheByActivityIdAsync(string activityId) => InvokeAsync(_ => _.RefreshVipCardCacheByActivityIdAsync(activityId));
		///<summary>ˢ�»���ǰ׺�ӿ�</summary>
        public OperationResult<bool> RefreshRedisCachePrefixForCommon(RefreshCachePrefixRequest request) => Invoke(_ => _.RefreshRedisCachePrefixForCommon(request));

	///<summary>ˢ�»���ǰ׺�ӿ�</summary>
        public Task<OperationResult<bool>> RefreshRedisCachePrefixForCommonAsync(RefreshCachePrefixRequest request) => InvokeAsync(_ => _.RefreshRedisCachePrefixForCommonAsync(request));
		///<summary>ˢ�¿ͻ�ר�����û���</summary>
        public OperationResult<bool> RefreshRedisCacheCustomerSetting(string activityExclusiveId) => Invoke(_ => _.RefreshRedisCacheCustomerSetting(activityExclusiveId));

	///<summary>ˢ�¿ͻ�ר�����û���</summary>
        public Task<OperationResult<bool>> RefreshRedisCacheCustomerSettingAsync(string activityExclusiveId) => InvokeAsync(_ => _.RefreshRedisCacheCustomerSettingAsync(activityExclusiveId));
	}
	/// <summary>���������</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IArticleService
    {
    	///<summary>��ѯ��ע�б�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Article/SelectDiscoveryHome", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Article/SelectDiscoveryHomeResponse")]
        Task<OperationResult<PagedModel<HomePageTimeLineRequestModel>>> SelectDiscoveryHomeAsync(string userId,PagerModel page,int version);
	}

	/// <summary>���������</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IArticleClient : IArticleService, ITuhuServiceClient
    {
    	///<summary>��ѯ��ע�б�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Article/SelectDiscoveryHome", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Article/SelectDiscoveryHomeResponse")]
        OperationResult<PagedModel<HomePageTimeLineRequestModel>> SelectDiscoveryHome(string userId,PagerModel page,int version);
	}

	/// <summary>���������</summary>
	public partial class ArticleClient : TuhuServiceClient<IArticleClient>, IArticleClient
    {
    	///<summary>��ѯ��ע�б�����</summary>
        public OperationResult<PagedModel<HomePageTimeLineRequestModel>> SelectDiscoveryHome(string userId,PagerModel page,int version) => Invoke(_ => _.SelectDiscoveryHome(userId,page,version));

	///<summary>��ѯ��ע�б�����</summary>
        public Task<OperationResult<PagedModel<HomePageTimeLineRequestModel>>> SelectDiscoveryHomeAsync(string userId,PagerModel page,int version) => InvokeAsync(_ => _.SelectDiscoveryHomeAsync(userId,page,version));
	}
	/// <summary>;���ڲ�</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IZeroActivityService
    {
    	///<summary>��ȡδ��������ҳ�ڲ��б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectUnfinishedZeroActivitiesForHomepage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectUnfinishedZeroActivitiesForHomepageResponse")]
        Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectUnfinishedZeroActivitiesForHomepageAsync(bool resetCache=false);
		///<summary>��ȡ�ѽ�������ҳ�ڲ��б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectFinishedZeroActivitiesForHomepage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectFinishedZeroActivitiesForHomepageResponse")]
        Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectFinishedZeroActivitiesForHomepageAsync(int pageNumber);
		///<summary>��ȡ�ڲ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchZeroActivityDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchZeroActivityDetailResponse")]
        Task<OperationResult<ZeroActivityDetailModel>> FetchZeroActivityDetailAsync(int period);
		///<summary>�ж��û��Ƿ����ύ�ڲ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityApplicationSubmitted", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityApplicationSubmittedResponse")]
        Task<OperationResult<bool>> HasZeroActivityApplicationSubmittedAsync(Guid userId, int period);
		///<summary>�ж��û��Ƿ��Ѵ�����������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityReminderSubmitted", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityReminderSubmittedResponse")]
        Task<OperationResult<bool>> HasZeroActivityReminderSubmittedAsync(Guid userId, int period);
		///<summary>��ȡ�ض��ڲ�����ѡ�û����䱨��ſ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectChosenUserReports", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectChosenUserReportsResponse")]
        Task<OperationResult<IEnumerable<SelectedTestReport>>> SelectChosenUserReportsAsync(int period);
		///<summary>��ȡ�ڲⱨ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchTestReportDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchTestReportDetailResponse")]
        Task<OperationResult<SelectedTestReportDetail>> FetchTestReportDetailAsync(int commentId);
		///<summary>��ȡ�û��ڲ�����,����״̬��0:�����У�1:����ɹ���-1������ʧ�ܣ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectMyApplications", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectMyApplicationsResponse")]
        Task<OperationResult<IEnumerable<MyZeroActivityApplications>>> SelectMyApplicationsAsync(Guid userId, int applicationStatus);
		///<summary>�ύ�ڲ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityApplication", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityApplicationResponse")]
        Task<OperationResult<bool>> SubmitZeroActivityApplicationAsync(ZeroActivityRequest requestModel);
		///<summary>������������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityReminder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityReminderResponse")]
        Task<OperationResult<bool>> SubmitZeroActivityReminderAsync(Guid userId, int period);
		///<summary>ˢ���ڲ����û���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/RefreshZeroActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/RefreshZeroActivityCacheResponse")]
        Task<OperationResult<bool>> RefreshZeroActivityCacheAsync();
		///<summary>��ҳ�ڲ�ģ���ýӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectHomePageModuleShowZeroActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectHomePageModuleShowZeroActivityResponse")]
        Task<OperationResult<List<ZeroActivitySimpleRespnseModel>>> SelectHomePageModuleShowZeroActivityAsync();
	}

	/// <summary>;���ڲ�</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IZeroActivityClient : IZeroActivityService, ITuhuServiceClient
    {
    	///<summary>��ȡδ��������ҳ�ڲ��б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectUnfinishedZeroActivitiesForHomepage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectUnfinishedZeroActivitiesForHomepageResponse")]
        OperationResult<IEnumerable<ZeroActivityModel>> SelectUnfinishedZeroActivitiesForHomepage(bool resetCache=false);
		///<summary>��ȡ�ѽ�������ҳ�ڲ��б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectFinishedZeroActivitiesForHomepage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectFinishedZeroActivitiesForHomepageResponse")]
        OperationResult<IEnumerable<ZeroActivityModel>> SelectFinishedZeroActivitiesForHomepage(int pageNumber);
		///<summary>��ȡ�ڲ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchZeroActivityDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchZeroActivityDetailResponse")]
        OperationResult<ZeroActivityDetailModel> FetchZeroActivityDetail(int period);
		///<summary>�ж��û��Ƿ����ύ�ڲ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityApplicationSubmitted", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityApplicationSubmittedResponse")]
        OperationResult<bool> HasZeroActivityApplicationSubmitted(Guid userId, int period);
		///<summary>�ж��û��Ƿ��Ѵ�����������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityReminderSubmitted", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/HasZeroActivityReminderSubmittedResponse")]
        OperationResult<bool> HasZeroActivityReminderSubmitted(Guid userId, int period);
		///<summary>��ȡ�ض��ڲ�����ѡ�û����䱨��ſ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectChosenUserReports", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectChosenUserReportsResponse")]
        OperationResult<IEnumerable<SelectedTestReport>> SelectChosenUserReports(int period);
		///<summary>��ȡ�ڲⱨ������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchTestReportDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/FetchTestReportDetailResponse")]
        OperationResult<SelectedTestReportDetail> FetchTestReportDetail(int commentId);
		///<summary>��ȡ�û��ڲ�����,����״̬��0:�����У�1:����ɹ���-1������ʧ�ܣ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectMyApplications", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectMyApplicationsResponse")]
        OperationResult<IEnumerable<MyZeroActivityApplications>> SelectMyApplications(Guid userId, int applicationStatus);
		///<summary>�ύ�ڲ�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityApplication", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityApplicationResponse")]
        OperationResult<bool> SubmitZeroActivityApplication(ZeroActivityRequest requestModel);
		///<summary>������������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityReminder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SubmitZeroActivityReminderResponse")]
        OperationResult<bool> SubmitZeroActivityReminder(Guid userId, int period);
		///<summary>ˢ���ڲ����û���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/RefreshZeroActivityCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/RefreshZeroActivityCacheResponse")]
        OperationResult<bool> RefreshZeroActivityCache();
		///<summary>��ҳ�ڲ�ģ���ýӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectHomePageModuleShowZeroActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ZeroActivity/SelectHomePageModuleShowZeroActivityResponse")]
        OperationResult<List<ZeroActivitySimpleRespnseModel>> SelectHomePageModuleShowZeroActivity();
	}

	/// <summary>;���ڲ�</summary>
	public partial class ZeroActivityClient : TuhuServiceClient<IZeroActivityClient>, IZeroActivityClient
    {
    	///<summary>��ȡδ��������ҳ�ڲ��б�</summary>
        public OperationResult<IEnumerable<ZeroActivityModel>> SelectUnfinishedZeroActivitiesForHomepage(bool resetCache=false) => Invoke(_ => _.SelectUnfinishedZeroActivitiesForHomepage(resetCache));

	///<summary>��ȡδ��������ҳ�ڲ��б�</summary>
        public Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectUnfinishedZeroActivitiesForHomepageAsync(bool resetCache=false) => InvokeAsync(_ => _.SelectUnfinishedZeroActivitiesForHomepageAsync(resetCache));
		///<summary>��ȡ�ѽ�������ҳ�ڲ��б�</summary>
        public OperationResult<IEnumerable<ZeroActivityModel>> SelectFinishedZeroActivitiesForHomepage(int pageNumber) => Invoke(_ => _.SelectFinishedZeroActivitiesForHomepage(pageNumber));

	///<summary>��ȡ�ѽ�������ҳ�ڲ��б�</summary>
        public Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectFinishedZeroActivitiesForHomepageAsync(int pageNumber) => InvokeAsync(_ => _.SelectFinishedZeroActivitiesForHomepageAsync(pageNumber));
		///<summary>��ȡ�ڲ�����</summary>
        public OperationResult<ZeroActivityDetailModel> FetchZeroActivityDetail(int period) => Invoke(_ => _.FetchZeroActivityDetail(period));

	///<summary>��ȡ�ڲ�����</summary>
        public Task<OperationResult<ZeroActivityDetailModel>> FetchZeroActivityDetailAsync(int period) => InvokeAsync(_ => _.FetchZeroActivityDetailAsync(period));
		///<summary>�ж��û��Ƿ����ύ�ڲ�����</summary>
        public OperationResult<bool> HasZeroActivityApplicationSubmitted(Guid userId, int period) => Invoke(_ => _.HasZeroActivityApplicationSubmitted(userId, period));

	///<summary>�ж��û��Ƿ����ύ�ڲ�����</summary>
        public Task<OperationResult<bool>> HasZeroActivityApplicationSubmittedAsync(Guid userId, int period) => InvokeAsync(_ => _.HasZeroActivityApplicationSubmittedAsync(userId, period));
		///<summary>�ж��û��Ƿ��Ѵ�����������</summary>
        public OperationResult<bool> HasZeroActivityReminderSubmitted(Guid userId, int period) => Invoke(_ => _.HasZeroActivityReminderSubmitted(userId, period));

	///<summary>�ж��û��Ƿ��Ѵ�����������</summary>
        public Task<OperationResult<bool>> HasZeroActivityReminderSubmittedAsync(Guid userId, int period) => InvokeAsync(_ => _.HasZeroActivityReminderSubmittedAsync(userId, period));
		///<summary>��ȡ�ض��ڲ�����ѡ�û����䱨��ſ�</summary>
        public OperationResult<IEnumerable<SelectedTestReport>> SelectChosenUserReports(int period) => Invoke(_ => _.SelectChosenUserReports(period));

	///<summary>��ȡ�ض��ڲ�����ѡ�û����䱨��ſ�</summary>
        public Task<OperationResult<IEnumerable<SelectedTestReport>>> SelectChosenUserReportsAsync(int period) => InvokeAsync(_ => _.SelectChosenUserReportsAsync(period));
		///<summary>��ȡ�ڲⱨ������</summary>
        public OperationResult<SelectedTestReportDetail> FetchTestReportDetail(int commentId) => Invoke(_ => _.FetchTestReportDetail(commentId));

	///<summary>��ȡ�ڲⱨ������</summary>
        public Task<OperationResult<SelectedTestReportDetail>> FetchTestReportDetailAsync(int commentId) => InvokeAsync(_ => _.FetchTestReportDetailAsync(commentId));
		///<summary>��ȡ�û��ڲ�����,����״̬��0:�����У�1:����ɹ���-1������ʧ�ܣ�</summary>
        public OperationResult<IEnumerable<MyZeroActivityApplications>> SelectMyApplications(Guid userId, int applicationStatus) => Invoke(_ => _.SelectMyApplications(userId, applicationStatus));

	///<summary>��ȡ�û��ڲ�����,����״̬��0:�����У�1:����ɹ���-1������ʧ�ܣ�</summary>
        public Task<OperationResult<IEnumerable<MyZeroActivityApplications>>> SelectMyApplicationsAsync(Guid userId, int applicationStatus) => InvokeAsync(_ => _.SelectMyApplicationsAsync(userId, applicationStatus));
		///<summary>�ύ�ڲ�����</summary>
        public OperationResult<bool> SubmitZeroActivityApplication(ZeroActivityRequest requestModel) => Invoke(_ => _.SubmitZeroActivityApplication(requestModel));

	///<summary>�ύ�ڲ�����</summary>
        public Task<OperationResult<bool>> SubmitZeroActivityApplicationAsync(ZeroActivityRequest requestModel) => InvokeAsync(_ => _.SubmitZeroActivityApplicationAsync(requestModel));
		///<summary>������������</summary>
        public OperationResult<bool> SubmitZeroActivityReminder(Guid userId, int period) => Invoke(_ => _.SubmitZeroActivityReminder(userId, period));

	///<summary>������������</summary>
        public Task<OperationResult<bool>> SubmitZeroActivityReminderAsync(Guid userId, int period) => InvokeAsync(_ => _.SubmitZeroActivityReminderAsync(userId, period));
		///<summary>ˢ���ڲ����û���</summary>
        public OperationResult<bool> RefreshZeroActivityCache() => Invoke(_ => _.RefreshZeroActivityCache());

	///<summary>ˢ���ڲ����û���</summary>
        public Task<OperationResult<bool>> RefreshZeroActivityCacheAsync() => InvokeAsync(_ => _.RefreshZeroActivityCacheAsync());
		///<summary>��ҳ�ڲ�ģ���ýӿ�</summary>
        public OperationResult<List<ZeroActivitySimpleRespnseModel>> SelectHomePageModuleShowZeroActivity() => Invoke(_ => _.SelectHomePageModuleShowZeroActivity());

	///<summary>��ҳ�ڲ�ģ���ýӿ�</summary>
        public Task<OperationResult<List<ZeroActivitySimpleRespnseModel>>> SelectHomePageModuleShowZeroActivityAsync() => InvokeAsync(_ => _.SelectHomePageModuleShowZeroActivityAsync());
	}
	/// <summary>�޹��µ����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleCreateOrderService
    {
    	///<summary>�޹��µ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FlashSaleCreateOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FlashSaleCreateOrderResponse")]
        Task<OperationResult<CreateOrderResult>> FlashSaleCreateOrderAsync(CreateOrderRequest request);
		/// <summary> ��û�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FetchActivityProductPrice", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FetchActivityProductPriceResponse")]
        Task<OperationResult<IEnumerable<ActivityPriceModel>>> FetchActivityProductPriceAsync(ActivityPriceRequest request);
	}

	/// <summary>�޹��µ����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleCreateOrderClient : IFlashSaleCreateOrderService, ITuhuServiceClient
    {
    	///<summary>�޹��µ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FlashSaleCreateOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FlashSaleCreateOrderResponse")]
        OperationResult<CreateOrderResult> FlashSaleCreateOrder(CreateOrderRequest request);
		/// <summary> ��û�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FetchActivityProductPrice", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSaleCreateOrder/FetchActivityProductPriceResponse")]
        OperationResult<IEnumerable<ActivityPriceModel>> FetchActivityProductPrice(ActivityPriceRequest request);
	}

	/// <summary>�޹��µ����</summary>
	public partial class FlashSaleCreateOrderClient : TuhuServiceClient<IFlashSaleCreateOrderClient>, IFlashSaleCreateOrderClient
    {
    	///<summary>�޹��µ�</summary>
        public OperationResult<CreateOrderResult> FlashSaleCreateOrder(CreateOrderRequest request) => Invoke(_ => _.FlashSaleCreateOrder(request));

	///<summary>�޹��µ�</summary>
        public Task<OperationResult<CreateOrderResult>> FlashSaleCreateOrderAsync(CreateOrderRequest request) => InvokeAsync(_ => _.FlashSaleCreateOrderAsync(request));
		/// <summary> ��û�� </summary>
        public OperationResult<IEnumerable<ActivityPriceModel>> FetchActivityProductPrice(ActivityPriceRequest request) => Invoke(_ => _.FetchActivityProductPrice(request));

	/// <summary> ��û�� </summary>
        public Task<OperationResult<IEnumerable<ActivityPriceModel>>> FetchActivityProductPriceAsync(ActivityPriceRequest request) => InvokeAsync(_ => _.FetchActivityProductPriceAsync(request));
	}
	/// <summary>���������</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IShareBargainService
    {
    	/// <summary> ��ȡ������Ʒ�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainPaoductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainPaoductListResponse")]
        Task<OperationResult<PagedModel<BargainProductModel>>> GetBargainPaoductListAsync(GetBargainproductListRequest request);
		/// <summary>  ��ȡ�û��û��Ʒ�µĿ��ۼ�¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductHistoryResponse")]
        Task<OperationResult<BargainProductHistory>> FetchBargainProductHistoryAsync(Guid userId,int activityProductId,string pid);
		/// <summary> �����˽���һ�ο��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddBargainAction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddBargainActionResponse")]
        Task<OperationResult<BargainResult>> AddBargainActionAsync(Guid idKey,Guid userId,int activityProductId);
		/// <summary> ������Ʒ�Ƿ�ɹ��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductStatusResponse")]
        Task<OperationResult<ShareBargainBaseResult>> CheckBargainProductStatusAsync(Guid ownerId,int apId,string pid, string deviceId = default(string));
		/// <summary> �û����𿳼۷��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddShareBargain", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddShareBargainResponse")]
        Task<OperationResult<BargainShareResult>> AddShareBargainAsync(Guid ownerId,int apId,string pid);
		/// <summary> �����˻�ȡ�����Ʒ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchShareBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchShareBargainInfoResponse")]
        Task<OperationResult<BargainShareProductModel>> FetchShareBargainInfoAsync(Guid idKey,Guid UserId);
		/// <summary> ˢ�»��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/RefreshShareBargainCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/RefreshShareBargainCacheResponse")]
        Task<OperationResult<bool>> RefreshShareBargainCacheAsync();
		/// <summary> ��÷�����ȫ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainConfigResponse")]
        Task<OperationResult<BargainGlobalConfigModel>> GetShareBargainConfigAsync();
		/// <summary> ������ȡ��Ʒ����ҳ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductByIdResponse")]
        Task<OperationResult<IEnumerable<BargainProductModel>>> SelectBargainProductByIdAsync(Guid OwnerId, Guid UserId,List<BargainProductItem> ProductItems);
		/// <summary> ��ҳ��ȡ��ƷPID��apid </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductItems", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductItemsResponse")]
        Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductItemsAsync(Guid UserId,int PageIndex,int PageSize);
		/// <summary> ����IdKey��ȡ��ƷPID��apid </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductItemByShareId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductItemByShareIdResponse")]
        Task<OperationResult<BargainProductInfo>> FetchBargainProductItemByShareIdAsync(Guid IdKey);
		/// <summary> ���÷���idkey��״̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetShareBargainStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetShareBargainStatusResponse")]
        Task<OperationResult<bool>> SetShareBargainStatusAsync(Guid IdKey);
		/// <summary> ����������ɺ��û���ȡ�Ż�ȯ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/MarkUserReceiveCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/MarkUserReceiveCouponResponse")]
        Task<OperationResult<ShareBargainBaseResult>> MarkUserReceiveCouponAsync(Guid ownerId, int apId, string pid, string deviceId = default(string));
		/// <summary> �û����۴�������ĳʱ����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetUserBargainCountAtTimerange", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetUserBargainCountAtTimerangeResponse")]
        Task<OperationResult<int>> GetUserBargainCountAtTimerangeAsync(Guid ownerId, DateTime beginTime, DateTime endTime);
		/// <summary>��ҳ��ȡĬ�ϵ�����������Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductForIndex", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductForIndexResponse")]
        Task<OperationResult<List<SimpleBargainProduct>>> GetBargainProductForIndexAsync();
		/// <summary> ��ҳ��ȡ���۲�Ʒ�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductListResponse")]
        Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductListAsync(int pageIndex, int pageSize);
		/// <summary> ��ҳ��ȡ�û��Ŀ��ۼ�¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainHistoryResponse")]
        Task<OperationResult<PagedModel<BargainHistoryModel>>> SelectBargainHistoryAsync(int pageIndex, int pageSize, Guid userId);
		/// <summary> ��ҳ��ȡ������Ʒ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargsinProductDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargsinProductDetailResponse")]
        Task<OperationResult<List<BargainProductNewModel>>> GetBargsinProductDetailAsync(Guid userId, List<BargainProductItem> productItems);
		/// <summary> ��ȡ�ֲ���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetSliceShowInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetSliceShowInfoResponse")]
        Task<OperationResult<List<SliceShowInfoModel>>> GetSliceShowInfoAsync(int count = 10);
		/// <summary> �û����������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargain", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainResponse")]
        Task<OperationResult<CreateBargainResult>> CreateserBargainAsync(Guid userId, int apId, string pid);
		/// <summary> �û�����������[�Ƿ�������] </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainNotPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainNotPushResponse")]
        Task<OperationResult<CreateBargainResult>> CreateserBargainNotPushAsync(Guid userId, int apId, string pid,bool isPush = false);
		/// <summary> �����˻�ȡ���۽�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfoResponse")]
        Task<OperationResult<InviteeBarginInfo>> GetInviteeBargainInfoAsync(Guid idKey,Guid userId);
		/// <summary> ��ȡδ��ɵ� ���𿳼ۼ�¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetValidBargainOwnerActionsByApids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetValidBargainOwnerActionsByApidsResponse")]
        Task<OperationResult<List<CurrentBargainData>>> GetValidBargainOwnerActionsByApidsAsync(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true);
		/// <summary> ��ȡ���۵�����  ��ʱ�䡿 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductsByDate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductsByDateResponse")]
        Task<OperationResult<List<BargainProductNewModel>>> SelectBargainProductsByDateAsync(DateTime startDate, DateTime endDate);
		/// <summary> �������ҳ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/BargainPushMessage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/BargainPushMessageResponse")]
        Task<OperationResult<bool>> BargainPushMessageAsync(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey);
		/// <summary> �û����𿳼۲��Կ� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateBargainAndCutSelf", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateBargainAndCutSelfResponse")]
        Task<OperationResult<CreateBargainResult>> CreateBargainAndCutSelfAsync(CreateBargainAndCutSelfRequest request);
		/// <summary> ����û��Ƿ�ɹ��򿳼���Ʒ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductBuyStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductBuyStatusResponse")]
        Task<OperationResult<ShareBargainBaseResult>> CheckBargainProductBuyStatusAsync(CheckBargainProductBuyStatusRequest request);
		/// <summary> �û���ȡ�����Ż�ȯ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/ReceiveBargainCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/ReceiveBargainCouponResponse")]
        Task<OperationResult<ShareBargainBaseResult>> ReceiveBargainCouponAsync(ReceiveBargainCouponRequest request);
		/// <summary> ��֤�Ƿ�Ϊ���ۺ����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainBlackList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainBlackListResponse")]
        Task<OperationResult<ShareBargainBaseResult>> CheckBargainBlackListAsync(BargainBlackListRequest request);
		/// <summary>�û��￳</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/HelpCut", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/HelpCutResponse")]
        Task<OperationResult<BargainResult>> HelpCutAsync(HelpCutRequest request);
		/// <summary>��ȡ���ۻ��Ʒ��Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainProductInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainProductInfoResponse")]
        Task<OperationResult<GetShareBargainProductInfoResponse>> GetShareBargainProductInfoAsync(GetShareBargainProductInfoRequest request);
		/// <summary>��ȡ���ۻ��Ʒ���û�����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainSettingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainSettingInfoResponse")]
        Task<OperationResult<GetShareBargainSettingInfoResponse>> GetShareBargainSettingInfoAsync(GetShareBargainSettingInfoRequest request);
		/// <summary>��ȡ���۷���ı��￳��¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutListResponse")]
        Task<OperationResult<List<GetShareBeHelpCutListResponse>>> GetShareBeHelpCutListAsync(GetShareBeHelpCutListRequest request);
		/// <summary>��ȡ�������õ�ǰ�����û���Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainUserParticipantInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainUserParticipantInfoResponse")]
        Task<OperationResult<GetShareBargainUserParticipantInfoResponse>> GetShareBargainUserParticipantInfoAsync(GetShareBargainUserParticipantInfoRequest request);
		/// <summary>��ʶ�û�����ʧ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetBargainOwnerFailIsPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetBargainOwnerFailIsPushResponse")]
        Task<OperationResult<SetBargainOwnerFailIsPushResponse>> SetBargainOwnerFailIsPushAsync(SetBargainOwnerFailIsPushRequest request);
		/// <summary>��ȡ ����ʧ�ܵķ����¼:���۷��𳬹�48Сʱ|���۳ɹ�24Сʱδ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainOwnerExpireList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainOwnerExpireListResponse")]
        Task<OperationResult<List<GetBargainOwnerExpireListResponse>>> GetBargainOwnerExpireListAsync(GetBargainOwnerExpireListRequest request);
		/// <summary>��ȡ���۷���ı��￳��¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutInfoListResponse")]
        Task<OperationResult<List<GetShareBeHelpCutInfoListResponse>>> GetShareBeHelpCutInfoListAsync(GetShareBeHelpCutInfoListRequest request);
		/// <summary>��ȡ������Ʒ��Ϣ���û����𿳼���Ϣ(��������ҳ)</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductAndUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductAndUserInfoResponse")]
        Task<OperationResult<GetBargainProductAndUserInfoResponse>> GetBargainProductAndUserInfoAsync(GetBargainProductAndUserInfoRequest request);
	}

	/// <summary>���������</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IShareBargainClient : IShareBargainService, ITuhuServiceClient
    {
    	/// <summary> ��ȡ������Ʒ�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainPaoductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainPaoductListResponse")]
        OperationResult<PagedModel<BargainProductModel>> GetBargainPaoductList(GetBargainproductListRequest request);
		/// <summary>  ��ȡ�û��û��Ʒ�µĿ��ۼ�¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductHistoryResponse")]
        OperationResult<BargainProductHistory> FetchBargainProductHistory(Guid userId,int activityProductId,string pid);
		/// <summary> �����˽���һ�ο��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddBargainAction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddBargainActionResponse")]
        OperationResult<BargainResult> AddBargainAction(Guid idKey,Guid userId,int activityProductId);
		/// <summary> ������Ʒ�Ƿ�ɹ��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductStatusResponse")]
        OperationResult<ShareBargainBaseResult> CheckBargainProductStatus(Guid ownerId,int apId,string pid, string deviceId = default(string));
		/// <summary> �û����𿳼۷��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddShareBargain", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/AddShareBargainResponse")]
        OperationResult<BargainShareResult> AddShareBargain(Guid ownerId,int apId,string pid);
		/// <summary> �����˻�ȡ�����Ʒ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchShareBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchShareBargainInfoResponse")]
        OperationResult<BargainShareProductModel> FetchShareBargainInfo(Guid idKey,Guid UserId);
		/// <summary> ˢ�»��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/RefreshShareBargainCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/RefreshShareBargainCacheResponse")]
        OperationResult<bool> RefreshShareBargainCache();
		/// <summary> ��÷�����ȫ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainConfig", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainConfigResponse")]
        OperationResult<BargainGlobalConfigModel> GetShareBargainConfig();
		/// <summary> ������ȡ��Ʒ����ҳ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductByIdResponse")]
        OperationResult<IEnumerable<BargainProductModel>> SelectBargainProductById(Guid OwnerId, Guid UserId,List<BargainProductItem> ProductItems);
		/// <summary> ��ҳ��ȡ��ƷPID��apid </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductItems", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductItemsResponse")]
        OperationResult<PagedModel<BargainProductItem>> SelectBargainProductItems(Guid UserId,int PageIndex,int PageSize);
		/// <summary> ����IdKey��ȡ��ƷPID��apid </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductItemByShareId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/FetchBargainProductItemByShareIdResponse")]
        OperationResult<BargainProductInfo> FetchBargainProductItemByShareId(Guid IdKey);
		/// <summary> ���÷���idkey��״̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetShareBargainStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetShareBargainStatusResponse")]
        OperationResult<bool> SetShareBargainStatus(Guid IdKey);
		/// <summary> ����������ɺ��û���ȡ�Ż�ȯ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/MarkUserReceiveCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/MarkUserReceiveCouponResponse")]
        OperationResult<ShareBargainBaseResult> MarkUserReceiveCoupon(Guid ownerId, int apId, string pid, string deviceId = default(string));
		/// <summary> �û����۴�������ĳʱ����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetUserBargainCountAtTimerange", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetUserBargainCountAtTimerangeResponse")]
        OperationResult<int> GetUserBargainCountAtTimerange(Guid ownerId, DateTime beginTime, DateTime endTime);
		/// <summary>��ҳ��ȡĬ�ϵ�����������Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductForIndex", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductForIndexResponse")]
        OperationResult<List<SimpleBargainProduct>> GetBargainProductForIndex();
		/// <summary> ��ҳ��ȡ���۲�Ʒ�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductListResponse")]
        OperationResult<PagedModel<BargainProductItem>> SelectBargainProductList(int pageIndex, int pageSize);
		/// <summary> ��ҳ��ȡ�û��Ŀ��ۼ�¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainHistoryResponse")]
        OperationResult<PagedModel<BargainHistoryModel>> SelectBargainHistory(int pageIndex, int pageSize, Guid userId);
		/// <summary> ��ҳ��ȡ������Ʒ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargsinProductDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargsinProductDetailResponse")]
        OperationResult<List<BargainProductNewModel>> GetBargsinProductDetail(Guid userId, List<BargainProductItem> productItems);
		/// <summary> ��ȡ�ֲ���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetSliceShowInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetSliceShowInfoResponse")]
        OperationResult<List<SliceShowInfoModel>> GetSliceShowInfo(int count = 10);
		/// <summary> �û����������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargain", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainResponse")]
        OperationResult<CreateBargainResult> CreateserBargain(Guid userId, int apId, string pid);
		/// <summary> �û�����������[�Ƿ�������] </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainNotPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateserBargainNotPushResponse")]
        OperationResult<CreateBargainResult> CreateserBargainNotPush(Guid userId, int apId, string pid,bool isPush = false);
		/// <summary> �����˻�ȡ���۽�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfoResponse")]
        OperationResult<InviteeBarginInfo> GetInviteeBargainInfo(Guid idKey,Guid userId);
		/// <summary> ��ȡδ��ɵ� ���𿳼ۼ�¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetValidBargainOwnerActionsByApids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetValidBargainOwnerActionsByApidsResponse")]
        OperationResult<List<CurrentBargainData>> GetValidBargainOwnerActionsByApids(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true);
		/// <summary> ��ȡ���۵�����  ��ʱ�䡿 </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductsByDate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SelectBargainProductsByDateResponse")]
        OperationResult<List<BargainProductNewModel>> SelectBargainProductsByDate(DateTime startDate, DateTime endDate);
		/// <summary> �������ҳ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/BargainPushMessage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/BargainPushMessageResponse")]
        OperationResult<bool> BargainPushMessage(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey);
		/// <summary> �û����𿳼۲��Կ� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateBargainAndCutSelf", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CreateBargainAndCutSelfResponse")]
        OperationResult<CreateBargainResult> CreateBargainAndCutSelf(CreateBargainAndCutSelfRequest request);
		/// <summary> ����û��Ƿ�ɹ��򿳼���Ʒ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductBuyStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainProductBuyStatusResponse")]
        OperationResult<ShareBargainBaseResult> CheckBargainProductBuyStatus(CheckBargainProductBuyStatusRequest request);
		/// <summary> �û���ȡ�����Ż�ȯ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/ReceiveBargainCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/ReceiveBargainCouponResponse")]
        OperationResult<ShareBargainBaseResult> ReceiveBargainCoupon(ReceiveBargainCouponRequest request);
		/// <summary> ��֤�Ƿ�Ϊ���ۺ����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainBlackList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/CheckBargainBlackListResponse")]
        OperationResult<ShareBargainBaseResult> CheckBargainBlackList(BargainBlackListRequest request);
		/// <summary>�û��￳</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/HelpCut", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/HelpCutResponse")]
        OperationResult<BargainResult> HelpCut(HelpCutRequest request);
		/// <summary>��ȡ���ۻ��Ʒ��Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainProductInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainProductInfoResponse")]
        OperationResult<GetShareBargainProductInfoResponse> GetShareBargainProductInfo(GetShareBargainProductInfoRequest request);
		/// <summary>��ȡ���ۻ��Ʒ���û�����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainSettingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainSettingInfoResponse")]
        OperationResult<GetShareBargainSettingInfoResponse> GetShareBargainSettingInfo(GetShareBargainSettingInfoRequest request);
		/// <summary>��ȡ���۷���ı��￳��¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutListResponse")]
        OperationResult<List<GetShareBeHelpCutListResponse>> GetShareBeHelpCutList(GetShareBeHelpCutListRequest request);
		/// <summary>��ȡ�������õ�ǰ�����û���Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainUserParticipantInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBargainUserParticipantInfoResponse")]
        OperationResult<GetShareBargainUserParticipantInfoResponse> GetShareBargainUserParticipantInfo(GetShareBargainUserParticipantInfoRequest request);
		/// <summary>��ʶ�û�����ʧ��������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetBargainOwnerFailIsPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/SetBargainOwnerFailIsPushResponse")]
        OperationResult<SetBargainOwnerFailIsPushResponse> SetBargainOwnerFailIsPush(SetBargainOwnerFailIsPushRequest request);
		/// <summary>��ȡ ����ʧ�ܵķ����¼:���۷��𳬹�48Сʱ|���۳ɹ�24Сʱδ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainOwnerExpireList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainOwnerExpireListResponse")]
        OperationResult<List<GetBargainOwnerExpireListResponse>> GetBargainOwnerExpireList(GetBargainOwnerExpireListRequest request);
		/// <summary>��ȡ���۷���ı��￳��¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetShareBeHelpCutInfoListResponse")]
        OperationResult<List<GetShareBeHelpCutInfoListResponse>> GetShareBeHelpCutInfoList(GetShareBeHelpCutInfoListRequest request);
		/// <summary>��ȡ������Ʒ��Ϣ���û����𿳼���Ϣ(��������ҳ)</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductAndUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetBargainProductAndUserInfoResponse")]
        OperationResult<GetBargainProductAndUserInfoResponse> GetBargainProductAndUserInfo(GetBargainProductAndUserInfoRequest request);
	}

	/// <summary>���������</summary>
	public partial class ShareBargainClient : TuhuServiceClient<IShareBargainClient>, IShareBargainClient
    {
    	/// <summary> ��ȡ������Ʒ�б� </summary>
        public OperationResult<PagedModel<BargainProductModel>> GetBargainPaoductList(GetBargainproductListRequest request) => Invoke(_ => _.GetBargainPaoductList(request));

	/// <summary> ��ȡ������Ʒ�б� </summary>
        public Task<OperationResult<PagedModel<BargainProductModel>>> GetBargainPaoductListAsync(GetBargainproductListRequest request) => InvokeAsync(_ => _.GetBargainPaoductListAsync(request));
		/// <summary>  ��ȡ�û��û��Ʒ�µĿ��ۼ�¼ </summary>
        public OperationResult<BargainProductHistory> FetchBargainProductHistory(Guid userId,int activityProductId,string pid) => Invoke(_ => _.FetchBargainProductHistory(userId,activityProductId,pid));

	/// <summary>  ��ȡ�û��û��Ʒ�µĿ��ۼ�¼ </summary>
        public Task<OperationResult<BargainProductHistory>> FetchBargainProductHistoryAsync(Guid userId,int activityProductId,string pid) => InvokeAsync(_ => _.FetchBargainProductHistoryAsync(userId,activityProductId,pid));
		/// <summary> �����˽���һ�ο��� </summary>
        public OperationResult<BargainResult> AddBargainAction(Guid idKey,Guid userId,int activityProductId) => Invoke(_ => _.AddBargainAction(idKey,userId,activityProductId));

	/// <summary> �����˽���һ�ο��� </summary>
        public Task<OperationResult<BargainResult>> AddBargainActionAsync(Guid idKey,Guid userId,int activityProductId) => InvokeAsync(_ => _.AddBargainActionAsync(idKey,userId,activityProductId));
		/// <summary> ������Ʒ�Ƿ�ɹ��� </summary>
        public OperationResult<ShareBargainBaseResult> CheckBargainProductStatus(Guid ownerId,int apId,string pid, string deviceId = default(string)) => Invoke(_ => _.CheckBargainProductStatus(ownerId,apId,pid,deviceId));

	/// <summary> ������Ʒ�Ƿ�ɹ��� </summary>
        public Task<OperationResult<ShareBargainBaseResult>> CheckBargainProductStatusAsync(Guid ownerId,int apId,string pid, string deviceId = default(string)) => InvokeAsync(_ => _.CheckBargainProductStatusAsync(ownerId,apId,pid,deviceId));
		/// <summary> �û����𿳼۷��� </summary>
        public OperationResult<BargainShareResult> AddShareBargain(Guid ownerId,int apId,string pid) => Invoke(_ => _.AddShareBargain(ownerId,apId,pid));

	/// <summary> �û����𿳼۷��� </summary>
        public Task<OperationResult<BargainShareResult>> AddShareBargainAsync(Guid ownerId,int apId,string pid) => InvokeAsync(_ => _.AddShareBargainAsync(ownerId,apId,pid));
		/// <summary> �����˻�ȡ�����Ʒ��Ϣ </summary>
        public OperationResult<BargainShareProductModel> FetchShareBargainInfo(Guid idKey,Guid UserId) => Invoke(_ => _.FetchShareBargainInfo(idKey,UserId));

	/// <summary> �����˻�ȡ�����Ʒ��Ϣ </summary>
        public Task<OperationResult<BargainShareProductModel>> FetchShareBargainInfoAsync(Guid idKey,Guid UserId) => InvokeAsync(_ => _.FetchShareBargainInfoAsync(idKey,UserId));
		/// <summary> ˢ�»��� </summary>
        public OperationResult<bool> RefreshShareBargainCache() => Invoke(_ => _.RefreshShareBargainCache());

	/// <summary> ˢ�»��� </summary>
        public Task<OperationResult<bool>> RefreshShareBargainCacheAsync() => InvokeAsync(_ => _.RefreshShareBargainCacheAsync());
		/// <summary> ��÷�����ȫ������ </summary>
        public OperationResult<BargainGlobalConfigModel> GetShareBargainConfig() => Invoke(_ => _.GetShareBargainConfig());

	/// <summary> ��÷�����ȫ������ </summary>
        public Task<OperationResult<BargainGlobalConfigModel>> GetShareBargainConfigAsync() => InvokeAsync(_ => _.GetShareBargainConfigAsync());
		/// <summary> ������ȡ��Ʒ����ҳ </summary>
        public OperationResult<IEnumerable<BargainProductModel>> SelectBargainProductById(Guid OwnerId, Guid UserId,List<BargainProductItem> ProductItems) => Invoke(_ => _.SelectBargainProductById(OwnerId,UserId,ProductItems));

	/// <summary> ������ȡ��Ʒ����ҳ </summary>
        public Task<OperationResult<IEnumerable<BargainProductModel>>> SelectBargainProductByIdAsync(Guid OwnerId, Guid UserId,List<BargainProductItem> ProductItems) => InvokeAsync(_ => _.SelectBargainProductByIdAsync(OwnerId,UserId,ProductItems));
		/// <summary> ��ҳ��ȡ��ƷPID��apid </summary>
        public OperationResult<PagedModel<BargainProductItem>> SelectBargainProductItems(Guid UserId,int PageIndex,int PageSize) => Invoke(_ => _.SelectBargainProductItems(UserId,PageIndex,PageSize));

	/// <summary> ��ҳ��ȡ��ƷPID��apid </summary>
        public Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductItemsAsync(Guid UserId,int PageIndex,int PageSize) => InvokeAsync(_ => _.SelectBargainProductItemsAsync(UserId,PageIndex,PageSize));
		/// <summary> ����IdKey��ȡ��ƷPID��apid </summary>
        public OperationResult<BargainProductInfo> FetchBargainProductItemByShareId(Guid IdKey) => Invoke(_ => _.FetchBargainProductItemByShareId(IdKey));

	/// <summary> ����IdKey��ȡ��ƷPID��apid </summary>
        public Task<OperationResult<BargainProductInfo>> FetchBargainProductItemByShareIdAsync(Guid IdKey) => InvokeAsync(_ => _.FetchBargainProductItemByShareIdAsync(IdKey));
		/// <summary> ���÷���idkey��״̬ </summary>
        public OperationResult<bool> SetShareBargainStatus(Guid IdKey) => Invoke(_ => _.SetShareBargainStatus(IdKey));

	/// <summary> ���÷���idkey��״̬ </summary>
        public Task<OperationResult<bool>> SetShareBargainStatusAsync(Guid IdKey) => InvokeAsync(_ => _.SetShareBargainStatusAsync(IdKey));
		/// <summary> ����������ɺ��û���ȡ�Ż�ȯ </summary>
        public OperationResult<ShareBargainBaseResult> MarkUserReceiveCoupon(Guid ownerId, int apId, string pid, string deviceId = default(string)) => Invoke(_ => _.MarkUserReceiveCoupon(ownerId,apId,pid,deviceId));

	/// <summary> ����������ɺ��û���ȡ�Ż�ȯ </summary>
        public Task<OperationResult<ShareBargainBaseResult>> MarkUserReceiveCouponAsync(Guid ownerId, int apId, string pid, string deviceId = default(string)) => InvokeAsync(_ => _.MarkUserReceiveCouponAsync(ownerId,apId,pid,deviceId));
		/// <summary> �û����۴�������ĳʱ����� </summary>
        public OperationResult<int> GetUserBargainCountAtTimerange(Guid ownerId, DateTime beginTime, DateTime endTime) => Invoke(_ => _.GetUserBargainCountAtTimerange(ownerId,beginTime,endTime));

	/// <summary> �û����۴�������ĳʱ����� </summary>
        public Task<OperationResult<int>> GetUserBargainCountAtTimerangeAsync(Guid ownerId, DateTime beginTime, DateTime endTime) => InvokeAsync(_ => _.GetUserBargainCountAtTimerangeAsync(ownerId,beginTime,endTime));
		/// <summary>��ҳ��ȡĬ�ϵ�����������Ʒ</summary>
        public OperationResult<List<SimpleBargainProduct>> GetBargainProductForIndex() => Invoke(_ => _.GetBargainProductForIndex());

	/// <summary>��ҳ��ȡĬ�ϵ�����������Ʒ</summary>
        public Task<OperationResult<List<SimpleBargainProduct>>> GetBargainProductForIndexAsync() => InvokeAsync(_ => _.GetBargainProductForIndexAsync());
		/// <summary> ��ҳ��ȡ���۲�Ʒ�б� </summary>
        public OperationResult<PagedModel<BargainProductItem>> SelectBargainProductList(int pageIndex, int pageSize) => Invoke(_ => _.SelectBargainProductList(pageIndex, pageSize));

	/// <summary> ��ҳ��ȡ���۲�Ʒ�б� </summary>
        public Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductListAsync(int pageIndex, int pageSize) => InvokeAsync(_ => _.SelectBargainProductListAsync(pageIndex, pageSize));
		/// <summary> ��ҳ��ȡ�û��Ŀ��ۼ�¼ </summary>
        public OperationResult<PagedModel<BargainHistoryModel>> SelectBargainHistory(int pageIndex, int pageSize, Guid userId) => Invoke(_ => _.SelectBargainHistory(pageIndex, pageSize, userId));

	/// <summary> ��ҳ��ȡ�û��Ŀ��ۼ�¼ </summary>
        public Task<OperationResult<PagedModel<BargainHistoryModel>>> SelectBargainHistoryAsync(int pageIndex, int pageSize, Guid userId) => InvokeAsync(_ => _.SelectBargainHistoryAsync(pageIndex, pageSize, userId));
		/// <summary> ��ҳ��ȡ������Ʒ���� </summary>
        public OperationResult<List<BargainProductNewModel>> GetBargsinProductDetail(Guid userId, List<BargainProductItem> productItems) => Invoke(_ => _.GetBargsinProductDetail(userId, productItems));

	/// <summary> ��ҳ��ȡ������Ʒ���� </summary>
        public Task<OperationResult<List<BargainProductNewModel>>> GetBargsinProductDetailAsync(Guid userId, List<BargainProductItem> productItems) => InvokeAsync(_ => _.GetBargsinProductDetailAsync(userId, productItems));
		/// <summary> ��ȡ�ֲ���Ϣ </summary>
        public OperationResult<List<SliceShowInfoModel>> GetSliceShowInfo(int count = 10) => Invoke(_ => _.GetSliceShowInfo(count));

	/// <summary> ��ȡ�ֲ���Ϣ </summary>
        public Task<OperationResult<List<SliceShowInfoModel>>> GetSliceShowInfoAsync(int count = 10) => InvokeAsync(_ => _.GetSliceShowInfoAsync(count));
		/// <summary> �û����������� </summary>
        public OperationResult<CreateBargainResult> CreateserBargain(Guid userId, int apId, string pid) => Invoke(_ => _.CreateserBargain(userId, apId, pid));

	/// <summary> �û����������� </summary>
        public Task<OperationResult<CreateBargainResult>> CreateserBargainAsync(Guid userId, int apId, string pid) => InvokeAsync(_ => _.CreateserBargainAsync(userId, apId, pid));
		/// <summary> �û�����������[�Ƿ�������] </summary>
        public OperationResult<CreateBargainResult> CreateserBargainNotPush(Guid userId, int apId, string pid,bool isPush = false) => Invoke(_ => _.CreateserBargainNotPush(userId, apId, pid, isPush));

	/// <summary> �û�����������[�Ƿ�������] </summary>
        public Task<OperationResult<CreateBargainResult>> CreateserBargainNotPushAsync(Guid userId, int apId, string pid,bool isPush = false) => InvokeAsync(_ => _.CreateserBargainNotPushAsync(userId, apId, pid, isPush));
		/// <summary> �����˻�ȡ���۽�� </summary>
        public OperationResult<InviteeBarginInfo> GetInviteeBargainInfo(Guid idKey,Guid userId) => Invoke(_ => _.GetInviteeBargainInfo(idKey, userId));

	/// <summary> �����˻�ȡ���۽�� </summary>
        public Task<OperationResult<InviteeBarginInfo>> GetInviteeBargainInfoAsync(Guid idKey,Guid userId) => InvokeAsync(_ => _.GetInviteeBargainInfoAsync(idKey, userId));
		/// <summary> ��ȡδ��ɵ� ���𿳼ۼ�¼ </summary>
        public OperationResult<List<CurrentBargainData>> GetValidBargainOwnerActionsByApids(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true) => Invoke(_ => _.GetValidBargainOwnerActionsByApids(apId,startDate, endDate, status, IsOver,readOnly));

	/// <summary> ��ȡδ��ɵ� ���𿳼ۼ�¼ </summary>
        public Task<OperationResult<List<CurrentBargainData>>> GetValidBargainOwnerActionsByApidsAsync(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true) => InvokeAsync(_ => _.GetValidBargainOwnerActionsByApidsAsync(apId,startDate, endDate, status, IsOver,readOnly));
		/// <summary> ��ȡ���۵�����  ��ʱ�䡿 </summary>
        public OperationResult<List<BargainProductNewModel>> SelectBargainProductsByDate(DateTime startDate, DateTime endDate) => Invoke(_ => _.SelectBargainProductsByDate(startDate, endDate));

	/// <summary> ��ȡ���۵�����  ��ʱ�䡿 </summary>
        public Task<OperationResult<List<BargainProductNewModel>>> SelectBargainProductsByDateAsync(DateTime startDate, DateTime endDate) => InvokeAsync(_ => _.SelectBargainProductsByDateAsync(startDate, endDate));
		/// <summary> �������ҳ���� </summary>
        public OperationResult<bool> BargainPushMessage(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey) => Invoke(_ => _.BargainPushMessage( data,  isOver,  apId,  userId,  idKey));

	/// <summary> �������ҳ���� </summary>
        public Task<OperationResult<bool>> BargainPushMessageAsync(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey) => InvokeAsync(_ => _.BargainPushMessageAsync( data,  isOver,  apId,  userId,  idKey));
		/// <summary> �û����𿳼۲��Կ� </summary>
        public OperationResult<CreateBargainResult> CreateBargainAndCutSelf(CreateBargainAndCutSelfRequest request) => Invoke(_ => _.CreateBargainAndCutSelf(request));

	/// <summary> �û����𿳼۲��Կ� </summary>
        public Task<OperationResult<CreateBargainResult>> CreateBargainAndCutSelfAsync(CreateBargainAndCutSelfRequest request) => InvokeAsync(_ => _.CreateBargainAndCutSelfAsync(request));
		/// <summary> ����û��Ƿ�ɹ��򿳼���Ʒ </summary>
        public OperationResult<ShareBargainBaseResult> CheckBargainProductBuyStatus(CheckBargainProductBuyStatusRequest request) => Invoke(_ => _.CheckBargainProductBuyStatus(request));

	/// <summary> ����û��Ƿ�ɹ��򿳼���Ʒ </summary>
        public Task<OperationResult<ShareBargainBaseResult>> CheckBargainProductBuyStatusAsync(CheckBargainProductBuyStatusRequest request) => InvokeAsync(_ => _.CheckBargainProductBuyStatusAsync(request));
		/// <summary> �û���ȡ�����Ż�ȯ </summary>
        public OperationResult<ShareBargainBaseResult> ReceiveBargainCoupon(ReceiveBargainCouponRequest request) => Invoke(_ => _.ReceiveBargainCoupon(request));

	/// <summary> �û���ȡ�����Ż�ȯ </summary>
        public Task<OperationResult<ShareBargainBaseResult>> ReceiveBargainCouponAsync(ReceiveBargainCouponRequest request) => InvokeAsync(_ => _.ReceiveBargainCouponAsync(request));
		/// <summary> ��֤�Ƿ�Ϊ���ۺ����� </summary>
        public OperationResult<ShareBargainBaseResult> CheckBargainBlackList(BargainBlackListRequest request) => Invoke(_ => _.CheckBargainBlackList(request));

	/// <summary> ��֤�Ƿ�Ϊ���ۺ����� </summary>
        public Task<OperationResult<ShareBargainBaseResult>> CheckBargainBlackListAsync(BargainBlackListRequest request) => InvokeAsync(_ => _.CheckBargainBlackListAsync(request));
		/// <summary>�û��￳</summary>
        public OperationResult<BargainResult> HelpCut(HelpCutRequest request) => Invoke(_ => _.HelpCut(request));

	/// <summary>�û��￳</summary>
        public Task<OperationResult<BargainResult>> HelpCutAsync(HelpCutRequest request) => InvokeAsync(_ => _.HelpCutAsync(request));
		/// <summary>��ȡ���ۻ��Ʒ��Ϣ</summary>
        public OperationResult<GetShareBargainProductInfoResponse> GetShareBargainProductInfo(GetShareBargainProductInfoRequest request) => Invoke(_ => _.GetShareBargainProductInfo(request));

	/// <summary>��ȡ���ۻ��Ʒ��Ϣ</summary>
        public Task<OperationResult<GetShareBargainProductInfoResponse>> GetShareBargainProductInfoAsync(GetShareBargainProductInfoRequest request) => InvokeAsync(_ => _.GetShareBargainProductInfoAsync(request));
		/// <summary>��ȡ���ۻ��Ʒ���û�����Ϣ</summary>
        public OperationResult<GetShareBargainSettingInfoResponse> GetShareBargainSettingInfo(GetShareBargainSettingInfoRequest request) => Invoke(_ => _.GetShareBargainSettingInfo(request));

	/// <summary>��ȡ���ۻ��Ʒ���û�����Ϣ</summary>
        public Task<OperationResult<GetShareBargainSettingInfoResponse>> GetShareBargainSettingInfoAsync(GetShareBargainSettingInfoRequest request) => InvokeAsync(_ => _.GetShareBargainSettingInfoAsync(request));
		/// <summary>��ȡ���۷���ı��￳��¼</summary>
        public OperationResult<List<GetShareBeHelpCutListResponse>> GetShareBeHelpCutList(GetShareBeHelpCutListRequest request) => Invoke(_ => _.GetShareBeHelpCutList(request));

	/// <summary>��ȡ���۷���ı��￳��¼</summary>
        public Task<OperationResult<List<GetShareBeHelpCutListResponse>>> GetShareBeHelpCutListAsync(GetShareBeHelpCutListRequest request) => InvokeAsync(_ => _.GetShareBeHelpCutListAsync(request));
		/// <summary>��ȡ�������õ�ǰ�����û���Ϣ</summary>
        public OperationResult<GetShareBargainUserParticipantInfoResponse> GetShareBargainUserParticipantInfo(GetShareBargainUserParticipantInfoRequest request) => Invoke(_ => _.GetShareBargainUserParticipantInfo(request));

	/// <summary>��ȡ�������õ�ǰ�����û���Ϣ</summary>
        public Task<OperationResult<GetShareBargainUserParticipantInfoResponse>> GetShareBargainUserParticipantInfoAsync(GetShareBargainUserParticipantInfoRequest request) => InvokeAsync(_ => _.GetShareBargainUserParticipantInfoAsync(request));
		/// <summary>��ʶ�û�����ʧ��������</summary>
        public OperationResult<SetBargainOwnerFailIsPushResponse> SetBargainOwnerFailIsPush(SetBargainOwnerFailIsPushRequest request) => Invoke(_ => _.SetBargainOwnerFailIsPush(request));

	/// <summary>��ʶ�û�����ʧ��������</summary>
        public Task<OperationResult<SetBargainOwnerFailIsPushResponse>> SetBargainOwnerFailIsPushAsync(SetBargainOwnerFailIsPushRequest request) => InvokeAsync(_ => _.SetBargainOwnerFailIsPushAsync(request));
		/// <summary>��ȡ ����ʧ�ܵķ����¼:���۷��𳬹�48Сʱ|���۳ɹ�24Сʱδ����</summary>
        public OperationResult<List<GetBargainOwnerExpireListResponse>> GetBargainOwnerExpireList(GetBargainOwnerExpireListRequest request) => Invoke(_ => _.GetBargainOwnerExpireList(request));

	/// <summary>��ȡ ����ʧ�ܵķ����¼:���۷��𳬹�48Сʱ|���۳ɹ�24Сʱδ����</summary>
        public Task<OperationResult<List<GetBargainOwnerExpireListResponse>>> GetBargainOwnerExpireListAsync(GetBargainOwnerExpireListRequest request) => InvokeAsync(_ => _.GetBargainOwnerExpireListAsync(request));
		/// <summary>��ȡ���۷���ı��￳��¼ </summary>
        public OperationResult<List<GetShareBeHelpCutInfoListResponse>> GetShareBeHelpCutInfoList(GetShareBeHelpCutInfoListRequest request) => Invoke(_ => _.GetShareBeHelpCutInfoList(request));

	/// <summary>��ȡ���۷���ı��￳��¼ </summary>
        public Task<OperationResult<List<GetShareBeHelpCutInfoListResponse>>> GetShareBeHelpCutInfoListAsync(GetShareBeHelpCutInfoListRequest request) => InvokeAsync(_ => _.GetShareBeHelpCutInfoListAsync(request));
		/// <summary>��ȡ������Ʒ��Ϣ���û����𿳼���Ϣ(��������ҳ)</summary>
        public OperationResult<GetBargainProductAndUserInfoResponse> GetBargainProductAndUserInfo(GetBargainProductAndUserInfoRequest request) => Invoke(_ => _.GetBargainProductAndUserInfo(request));

	/// <summary>��ȡ������Ʒ��Ϣ���û����𿳼���Ϣ(��������ҳ)</summary>
        public Task<OperationResult<GetBargainProductAndUserInfoResponse>> GetBargainProductAndUserInfoAsync(GetBargainProductAndUserInfoRequest request) => InvokeAsync(_ => _.GetBargainProductAndUserInfoAsync(request));
	}
	/// <summary>ƴ�Ż���</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IGroupBuyingService
    {
    	/// <summary> ��ҳ��ȡ��ҳProductGroupId </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListResponse")]
        Task<OperationResult<PagedModel<string>>> GetGroupBuyingProductListAsync(int PageIndex = 1, int PageSize = 10, bool flag = false,string channel=default(string),bool isOldUser=false);
		/// <summary> ����ProductGroupId��ȡ��Ӧ��Ʒ��PID </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingProductsById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingProductsByIdResponse")]
        Task<OperationResult<List<string>>> SelectGroupBuyingProductsByIdAsync(string ProductGroupId);
		/// <summary> ����ProductGroupId��ȡ��ӦProductGroup��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupInfoResponse")]
        Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupInfoAsync(List<string> ProductGroupIds);
		/// <summary> ����PID��ȡ��Ӧ��Ʒ����Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductInfoByPid", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductInfoByPidResponse")]
        Task<OperationResult<GroupBuyingProductModel>> SelectProductInfoByPidAsync(string ProductGroupId, string Pid);
		/// <summary> ����PID��ȡ�ò�Ʒ���£����û����Բμӵ����ɸ��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoByProductGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoByProductGroupIdResponse")]
        Task<OperationResult<List<GroupInfoModel>>> SelectGroupInfoByProductGroupIdAsync(string ProductGroupId, Guid UserId, int Count = 100);
		/// <summary> �����źŻ�ȡƴ����Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByGroupIdResponse")]
        Task<OperationResult<GroupInfoModel>> FetchGroupInfoByGroupIdAsync(Guid GroupId);
		/// <summary> �����źŻ�ȡ��ǰ�ų�Ա </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupMemberByGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupMemberByGroupIdResponse")]
        Task<OperationResult<GroupMemberModel>> SelectGroupMemberByGroupIdAsync(Guid GroupId);
		/// <summary> У���û��Ĳ����ʸ� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckGroupInfoByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckGroupInfoByUserIdResponse")]
        Task<OperationResult<CheckResultModel>> CheckGroupInfoByUserIdAsync(Guid GroupId, Guid UserId, string ProductGroupId,string pid=default(string));
		/// <summary> �û��������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CreateGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CreateGroupBuyingResponse")]
        Task<OperationResult<VerificationResultModel>> CreateGroupBuyingAsync(Guid UserId, string ProductGroupId, string Pid, int OrderId);
		/// <summary> �û�����ƴ�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/TakePartInGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/TakePartInGroupBuyingResponse")]
        Task<OperationResult<VerificationResultModel>> TakePartInGroupBuyingAsync(Guid UserId, Guid GroupId, string Pid, int OrderId);
		/// <summary> ��ҳ��ȡ���û����ź� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupInfoByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupInfoByUserIdResponse")]
        Task<OperationResult<PagedModel<UserGroupBuyingInfoModel>>> GetUserGroupInfoByUserIdAsync(GroupInfoRequest request);
		/// <summary> ˢ��ƴ�Ų�Ʒ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshCacheResponse")]
        Task<OperationResult<VerificationResultModel>> RefreshCacheAsync(string ProductGroupId = null);
		/// <summary> ˢ��ƴ�Ż��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshGroupCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshGroupCacheResponse")]
        Task<OperationResult<VerificationResultModel>> RefreshGroupCacheAsync(Guid GroupId);
		/// <summary> �����źţ�UserId��ȡ�û�������Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchUserOrderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchUserOrderInfoResponse")]
        Task<OperationResult<UserOrderInfoModel>> FetchUserOrderInfoAsync(Guid GroupId, Guid UserId);
		/// <summary> �û�ȡ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CancelGroupBuyingOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CancelGroupBuyingOrderResponse")]
        Task<OperationResult<VerificationResultModel>> CancelGroupBuyingOrderAsync(Guid GroupId, int OrderId);
		/// <summary> �ų�������ſɼ� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeGroupBuyingStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeGroupBuyingStatusResponse")]
        Task<OperationResult<bool>> ChangeGroupBuyingStatusAsync(Guid GroupId, int OrderId);
		/// <summary> ��Ա������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeUserStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeUserStatusResponse")]
        Task<OperationResult<bool>> ChangeUserStatusAsync(Guid GroupId, Guid UserId, int OrderId);
		/// <summary> ƴ�Ź��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ExpireGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ExpireGroupBuyingResponse")]
        Task<OperationResult<VerificationResultModel>> ExpireGroupBuyingAsync(Guid GroupId);
		/// <summary> ��ȡ��Ʒ���в�Ʒ��ϸ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupDetailResponse")]
        Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupDetailAsync(string ProductGroupId);
		/// <summary> ����OrderId��ѯ����Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByOrderIdResponse")]
        Task<OperationResult<GroupInfoModel>> FetchGroupInfoByOrderIdAsync(int OrderId);
		/// <summary> ����ProductGroupId��ѯ��Ʒ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchProductGroupInfoById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchProductGroupInfoByIdResponse")]
        Task<OperationResult<ProductGroupModel>> FetchProductGroupInfoByIdAsync(string ProductGroupId);
		/// <summary> ����UserId��OpenIdУ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckNewUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckNewUserResponse")]
        Task<OperationResult<NewUserCheckResultModel>> CheckNewUserAsync(Guid userId, string openId = default(string));
		/// <summary> ����PID��ȡ�ò�Ʒ���£����û����Բμӵ����ɸ���(����TotalCount) </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoWithTotalCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoWithTotalCountResponse")]
        Task<OperationResult<GroupInfoResponse>> SelectGroupInfoWithTotalCountAsync(string ProductGroupId, Guid UserId, int Count = 100);
		/// <summary>ƴ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/PushPinTuanMessage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/PushPinTuanMessageResponse")]
        Task<OperationResult<bool>> PushPinTuanMessageAsync(Guid groupId,int batchId);
		/// <summary>����PID��ȡ����ProductGroupId�Լ��۸�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetProductGroupInfoByPId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetProductGroupInfoByPIdResponse")]
        Task<OperationResult<GroupBuyingProductInfo>> GetProductGroupInfoByPIdAsync(string pId);
		/// <summary>����PID��ȡ��Ӧ��Ʒ����Ϣ(����)</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductListByPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductListByPidsResponse")]
        Task<OperationResult<List<ProductGroupModel>>> SelectProductListByPidsAsync(List<GroupBuyingProductRequest> request);
		/// <summary>��ȡ�齱����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetLotteryRule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetLotteryRuleResponse")]
        Task<OperationResult<GroupLotteryRuleModel>> GetLotteryRuleAsync(string productGroupId);
		/// <summary>��ȡ�н�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetWinnerList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetWinnerListResponse")]
        Task<OperationResult<PagedModel<GroupBuyingLotteryInfo>>> GetWinnerListAsync(string productGroupId, int level = 0, int pageIndex = 1, int pageSize = 20);
		/// <summary> ��ѯ�û��н�״̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckUserLotteryResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckUserLotteryResultResponse")]
        Task<OperationResult<GroupBuyingLotteryInfo>> CheckUserLotteryResultAsync(Guid userId,string productGroupId,int orderId);
		/// <summary>��ѯ�û����н���¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserLotteryHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserLotteryHistoryResponse")]
        Task<OperationResult<List<GroupBuyingLotteryInfo>>> GetUserLotteryHistoryAsync(Guid userId, List<int> orderIds);
		/// <summary>����ƴ�����ͻ�ȡ��Ʒ�б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetActivityProductGroup", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetActivityProductGroupResponse")]
        Task<OperationResult<PagedModel<string>>> GetActivityProductGroupAsync(ActivityGroupRequest request);
		/// <summary>��ѯ�û��ⵥȯ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserFreeCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserFreeCouponResponse")]
        Task<OperationResult<List<FreeCouponModel>>> GetUserFreeCouponAsync(Guid userId);
		/// <summary> ��ȡ�û�ƴ�ż�¼ͳ�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupCountByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupCountByUserIdResponse")]
        Task<OperationResult<GroupBuyingHistoryCount>> GetUserGroupCountByUserIdAsync(Guid userId);
		/// <summary> ��ȡ���ճ��ŵ��û���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupFinalUserList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupFinalUserListResponse")]
        Task<OperationResult<List<GroupFinalUserModel>>> GetGroupFinalUserListAsync(Guid groupId);
		/// <summary> ��ѯ�û��޹���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserBuyLimitInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserBuyLimitInfoResponse")]
        Task<OperationResult<GroupBuyingBuyLimitModel>> GetUserBuyLimitInfoAsync(GroupBuyingBuyLimitRequest request);
		/// <summary> ��ѯƴ��ƴ����Ŀ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingCategoryInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingCategoryInfoResponse")]
        Task<OperationResult<List<GroupBuyingCategoryModel>>> GetGroupBuyingCategoryInfoAsync();
		/// <summary> ��ѯƴ�Ų�Ʒ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListNewResponse")]
        Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> GetGroupBuyingProductListNewAsync(GroupBuyingQueryRequest request);
		/// <summary> ˢ��ES���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/UpdateGroupBuyingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/UpdateGroupBuyingInfoResponse")]
        Task<OperationResult<bool>> UpdateGroupBuyingInfoAsync(List<string> productGroupIds);
		/// <summary> ���ݹؼ�������ƴ�Ų�Ʒ��Ϣ </summary>
		[Obsolete("�ѷ�������ʹ��SelectGroupBuyingListNewAsync",true)]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SearchGroupBuyingByKeyword", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SearchGroupBuyingByKeywordResponse")]
        Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SearchGroupBuyingByKeywordAsync(GroupBuyingQueryRequest request);
		/// <summary> ��ѯƴ�Ų�Ʒ�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingListNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingListNewResponse")]
        Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SelectGroupBuyingListNewAsync(GroupBuyingQueryRequest request);
	}

	/// <summary>ƴ�Ż���</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IGroupBuyingClient : IGroupBuyingService, ITuhuServiceClient
    {
    	/// <summary> ��ҳ��ȡ��ҳProductGroupId </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListResponse")]
        OperationResult<PagedModel<string>> GetGroupBuyingProductList(int PageIndex = 1, int PageSize = 10, bool flag = false,string channel=default(string),bool isOldUser=false);
		/// <summary> ����ProductGroupId��ȡ��Ӧ��Ʒ��PID </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingProductsById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingProductsByIdResponse")]
        OperationResult<List<string>> SelectGroupBuyingProductsById(string ProductGroupId);
		/// <summary> ����ProductGroupId��ȡ��ӦProductGroup��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupInfoResponse")]
        OperationResult<List<ProductGroupModel>> SelectProductGroupInfo(List<string> ProductGroupIds);
		/// <summary> ����PID��ȡ��Ӧ��Ʒ����Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductInfoByPid", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductInfoByPidResponse")]
        OperationResult<GroupBuyingProductModel> SelectProductInfoByPid(string ProductGroupId, string Pid);
		/// <summary> ����PID��ȡ�ò�Ʒ���£����û����Բμӵ����ɸ��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoByProductGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoByProductGroupIdResponse")]
        OperationResult<List<GroupInfoModel>> SelectGroupInfoByProductGroupId(string ProductGroupId, Guid UserId, int Count = 100);
		/// <summary> �����źŻ�ȡƴ����Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByGroupIdResponse")]
        OperationResult<GroupInfoModel> FetchGroupInfoByGroupId(Guid GroupId);
		/// <summary> �����źŻ�ȡ��ǰ�ų�Ա </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupMemberByGroupId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupMemberByGroupIdResponse")]
        OperationResult<GroupMemberModel> SelectGroupMemberByGroupId(Guid GroupId);
		/// <summary> У���û��Ĳ����ʸ� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckGroupInfoByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckGroupInfoByUserIdResponse")]
        OperationResult<CheckResultModel> CheckGroupInfoByUserId(Guid GroupId, Guid UserId, string ProductGroupId,string pid=default(string));
		/// <summary> �û��������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CreateGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CreateGroupBuyingResponse")]
        OperationResult<VerificationResultModel> CreateGroupBuying(Guid UserId, string ProductGroupId, string Pid, int OrderId);
		/// <summary> �û�����ƴ�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/TakePartInGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/TakePartInGroupBuyingResponse")]
        OperationResult<VerificationResultModel> TakePartInGroupBuying(Guid UserId, Guid GroupId, string Pid, int OrderId);
		/// <summary> ��ҳ��ȡ���û����ź� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupInfoByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupInfoByUserIdResponse")]
        OperationResult<PagedModel<UserGroupBuyingInfoModel>> GetUserGroupInfoByUserId(GroupInfoRequest request);
		/// <summary> ˢ��ƴ�Ų�Ʒ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshCacheResponse")]
        OperationResult<VerificationResultModel> RefreshCache(string ProductGroupId = null);
		/// <summary> ˢ��ƴ�Ż��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshGroupCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/RefreshGroupCacheResponse")]
        OperationResult<VerificationResultModel> RefreshGroupCache(Guid GroupId);
		/// <summary> �����źţ�UserId��ȡ�û�������Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchUserOrderInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchUserOrderInfoResponse")]
        OperationResult<UserOrderInfoModel> FetchUserOrderInfo(Guid GroupId, Guid UserId);
		/// <summary> �û�ȡ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CancelGroupBuyingOrder", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CancelGroupBuyingOrderResponse")]
        OperationResult<VerificationResultModel> CancelGroupBuyingOrder(Guid GroupId, int OrderId);
		/// <summary> �ų�������ſɼ� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeGroupBuyingStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeGroupBuyingStatusResponse")]
        OperationResult<bool> ChangeGroupBuyingStatus(Guid GroupId, int OrderId);
		/// <summary> ��Ա������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeUserStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ChangeUserStatusResponse")]
        OperationResult<bool> ChangeUserStatus(Guid GroupId, Guid UserId, int OrderId);
		/// <summary> ƴ�Ź��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ExpireGroupBuying", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/ExpireGroupBuyingResponse")]
        OperationResult<VerificationResultModel> ExpireGroupBuying(Guid GroupId);
		/// <summary> ��ȡ��Ʒ���в�Ʒ��ϸ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductGroupDetailResponse")]
        OperationResult<List<ProductGroupModel>> SelectProductGroupDetail(string ProductGroupId);
		/// <summary> ����OrderId��ѯ����Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByOrderId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchGroupInfoByOrderIdResponse")]
        OperationResult<GroupInfoModel> FetchGroupInfoByOrderId(int OrderId);
		/// <summary> ����ProductGroupId��ѯ��Ʒ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchProductGroupInfoById", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/FetchProductGroupInfoByIdResponse")]
        OperationResult<ProductGroupModel> FetchProductGroupInfoById(string ProductGroupId);
		/// <summary> ����UserId��OpenIdУ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckNewUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckNewUserResponse")]
        OperationResult<NewUserCheckResultModel> CheckNewUser(Guid userId, string openId = default(string));
		/// <summary> ����PID��ȡ�ò�Ʒ���£����û����Բμӵ����ɸ���(����TotalCount) </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoWithTotalCount", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupInfoWithTotalCountResponse")]
        OperationResult<GroupInfoResponse> SelectGroupInfoWithTotalCount(string ProductGroupId, Guid UserId, int Count = 100);
		/// <summary>ƴ������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/PushPinTuanMessage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/PushPinTuanMessageResponse")]
        OperationResult<bool> PushPinTuanMessage(Guid groupId,int batchId);
		/// <summary>����PID��ȡ����ProductGroupId�Լ��۸�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetProductGroupInfoByPId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetProductGroupInfoByPIdResponse")]
        OperationResult<GroupBuyingProductInfo> GetProductGroupInfoByPId(string pId);
		/// <summary>����PID��ȡ��Ӧ��Ʒ����Ϣ(����)</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductListByPids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectProductListByPidsResponse")]
        OperationResult<List<ProductGroupModel>> SelectProductListByPids(List<GroupBuyingProductRequest> request);
		/// <summary>��ȡ�齱����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetLotteryRule", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetLotteryRuleResponse")]
        OperationResult<GroupLotteryRuleModel> GetLotteryRule(string productGroupId);
		/// <summary>��ȡ�н�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetWinnerList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetWinnerListResponse")]
        OperationResult<PagedModel<GroupBuyingLotteryInfo>> GetWinnerList(string productGroupId, int level = 0, int pageIndex = 1, int pageSize = 20);
		/// <summary> ��ѯ�û��н�״̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckUserLotteryResult", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/CheckUserLotteryResultResponse")]
        OperationResult<GroupBuyingLotteryInfo> CheckUserLotteryResult(Guid userId,string productGroupId,int orderId);
		/// <summary>��ѯ�û����н���¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserLotteryHistory", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserLotteryHistoryResponse")]
        OperationResult<List<GroupBuyingLotteryInfo>> GetUserLotteryHistory(Guid userId, List<int> orderIds);
		/// <summary>����ƴ�����ͻ�ȡ��Ʒ�б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetActivityProductGroup", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetActivityProductGroupResponse")]
        OperationResult<PagedModel<string>> GetActivityProductGroup(ActivityGroupRequest request);
		/// <summary>��ѯ�û��ⵥȯ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserFreeCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserFreeCouponResponse")]
        OperationResult<List<FreeCouponModel>> GetUserFreeCoupon(Guid userId);
		/// <summary> ��ȡ�û�ƴ�ż�¼ͳ�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupCountByUserId", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserGroupCountByUserIdResponse")]
        OperationResult<GroupBuyingHistoryCount> GetUserGroupCountByUserId(Guid userId);
		/// <summary> ��ȡ���ճ��ŵ��û���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupFinalUserList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupFinalUserListResponse")]
        OperationResult<List<GroupFinalUserModel>> GetGroupFinalUserList(Guid groupId);
		/// <summary> ��ѯ�û��޹���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserBuyLimitInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetUserBuyLimitInfoResponse")]
        OperationResult<GroupBuyingBuyLimitModel> GetUserBuyLimitInfo(GroupBuyingBuyLimitRequest request);
		/// <summary> ��ѯƴ��ƴ����Ŀ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingCategoryInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingCategoryInfoResponse")]
        OperationResult<List<GroupBuyingCategoryModel>> GetGroupBuyingCategoryInfo();
		/// <summary> ��ѯƴ�Ų�Ʒ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/GetGroupBuyingProductListNewResponse")]
        OperationResult<PagedModel<SimplegroupBuyingModel>> GetGroupBuyingProductListNew(GroupBuyingQueryRequest request);
		/// <summary> ˢ��ES���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/UpdateGroupBuyingInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/UpdateGroupBuyingInfoResponse")]
        OperationResult<bool> UpdateGroupBuyingInfo(List<string> productGroupIds);
		/// <summary> ���ݹؼ�������ƴ�Ų�Ʒ��Ϣ </summary>
		[Obsolete("�ѷ�������ʹ��SelectGroupBuyingListNewAsync",true)]
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SearchGroupBuyingByKeyword", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SearchGroupBuyingByKeywordResponse")]
        OperationResult<PagedModel<SimplegroupBuyingModel>> SearchGroupBuyingByKeyword(GroupBuyingQueryRequest request);
		/// <summary> ��ѯƴ�Ų�Ʒ�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingListNew", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/GroupBuying/SelectGroupBuyingListNewResponse")]
        OperationResult<PagedModel<SimplegroupBuyingModel>> SelectGroupBuyingListNew(GroupBuyingQueryRequest request);
	}

	/// <summary>ƴ�Ż���</summary>
	public partial class GroupBuyingClient : TuhuServiceClient<IGroupBuyingClient>, IGroupBuyingClient
    {
    	/// <summary> ��ҳ��ȡ��ҳProductGroupId </summary>
        public OperationResult<PagedModel<string>> GetGroupBuyingProductList(int PageIndex = 1, int PageSize = 10, bool flag = false,string channel=default(string),bool isOldUser=false) => Invoke(_ => _.GetGroupBuyingProductList(PageIndex,PageSize,flag,channel,isOldUser));

	/// <summary> ��ҳ��ȡ��ҳProductGroupId </summary>
        public Task<OperationResult<PagedModel<string>>> GetGroupBuyingProductListAsync(int PageIndex = 1, int PageSize = 10, bool flag = false,string channel=default(string),bool isOldUser=false) => InvokeAsync(_ => _.GetGroupBuyingProductListAsync(PageIndex,PageSize,flag,channel,isOldUser));
		/// <summary> ����ProductGroupId��ȡ��Ӧ��Ʒ��PID </summary>
        public OperationResult<List<string>> SelectGroupBuyingProductsById(string ProductGroupId) => Invoke(_ => _.SelectGroupBuyingProductsById(ProductGroupId));

	/// <summary> ����ProductGroupId��ȡ��Ӧ��Ʒ��PID </summary>
        public Task<OperationResult<List<string>>> SelectGroupBuyingProductsByIdAsync(string ProductGroupId) => InvokeAsync(_ => _.SelectGroupBuyingProductsByIdAsync(ProductGroupId));
		/// <summary> ����ProductGroupId��ȡ��ӦProductGroup��Ϣ </summary>
        public OperationResult<List<ProductGroupModel>> SelectProductGroupInfo(List<string> ProductGroupIds) => Invoke(_ => _.SelectProductGroupInfo(ProductGroupIds));

	/// <summary> ����ProductGroupId��ȡ��ӦProductGroup��Ϣ </summary>
        public Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupInfoAsync(List<string> ProductGroupIds) => InvokeAsync(_ => _.SelectProductGroupInfoAsync(ProductGroupIds));
		/// <summary> ����PID��ȡ��Ӧ��Ʒ����Ϣ </summary>
        public OperationResult<GroupBuyingProductModel> SelectProductInfoByPid(string ProductGroupId, string Pid) => Invoke(_ => _.SelectProductInfoByPid(ProductGroupId, Pid));

	/// <summary> ����PID��ȡ��Ӧ��Ʒ����Ϣ </summary>
        public Task<OperationResult<GroupBuyingProductModel>> SelectProductInfoByPidAsync(string ProductGroupId, string Pid) => InvokeAsync(_ => _.SelectProductInfoByPidAsync(ProductGroupId, Pid));
		/// <summary> ����PID��ȡ�ò�Ʒ���£����û����Բμӵ����ɸ��� </summary>
        public OperationResult<List<GroupInfoModel>> SelectGroupInfoByProductGroupId(string ProductGroupId, Guid UserId, int Count = 100) => Invoke(_ => _.SelectGroupInfoByProductGroupId(ProductGroupId, UserId, Count));

	/// <summary> ����PID��ȡ�ò�Ʒ���£����û����Բμӵ����ɸ��� </summary>
        public Task<OperationResult<List<GroupInfoModel>>> SelectGroupInfoByProductGroupIdAsync(string ProductGroupId, Guid UserId, int Count = 100) => InvokeAsync(_ => _.SelectGroupInfoByProductGroupIdAsync(ProductGroupId, UserId, Count));
		/// <summary> �����źŻ�ȡƴ����Ϣ </summary>
        public OperationResult<GroupInfoModel> FetchGroupInfoByGroupId(Guid GroupId) => Invoke(_ => _.FetchGroupInfoByGroupId(GroupId));

	/// <summary> �����źŻ�ȡƴ����Ϣ </summary>
        public Task<OperationResult<GroupInfoModel>> FetchGroupInfoByGroupIdAsync(Guid GroupId) => InvokeAsync(_ => _.FetchGroupInfoByGroupIdAsync(GroupId));
		/// <summary> �����źŻ�ȡ��ǰ�ų�Ա </summary>
        public OperationResult<GroupMemberModel> SelectGroupMemberByGroupId(Guid GroupId) => Invoke(_ => _.SelectGroupMemberByGroupId(GroupId));

	/// <summary> �����źŻ�ȡ��ǰ�ų�Ա </summary>
        public Task<OperationResult<GroupMemberModel>> SelectGroupMemberByGroupIdAsync(Guid GroupId) => InvokeAsync(_ => _.SelectGroupMemberByGroupIdAsync(GroupId));
		/// <summary> У���û��Ĳ����ʸ� </summary>
        public OperationResult<CheckResultModel> CheckGroupInfoByUserId(Guid GroupId, Guid UserId, string ProductGroupId,string pid=default(string)) => Invoke(_ => _.CheckGroupInfoByUserId(GroupId, UserId, ProductGroupId, pid));

	/// <summary> У���û��Ĳ����ʸ� </summary>
        public Task<OperationResult<CheckResultModel>> CheckGroupInfoByUserIdAsync(Guid GroupId, Guid UserId, string ProductGroupId,string pid=default(string)) => InvokeAsync(_ => _.CheckGroupInfoByUserIdAsync(GroupId, UserId, ProductGroupId, pid));
		/// <summary> �û��������� </summary>
        public OperationResult<VerificationResultModel> CreateGroupBuying(Guid UserId, string ProductGroupId, string Pid, int OrderId) => Invoke(_ => _.CreateGroupBuying(UserId, ProductGroupId, Pid, OrderId));

	/// <summary> �û��������� </summary>
        public Task<OperationResult<VerificationResultModel>> CreateGroupBuyingAsync(Guid UserId, string ProductGroupId, string Pid, int OrderId) => InvokeAsync(_ => _.CreateGroupBuyingAsync(UserId, ProductGroupId, Pid, OrderId));
		/// <summary> �û�����ƴ�� </summary>
        public OperationResult<VerificationResultModel> TakePartInGroupBuying(Guid UserId, Guid GroupId, string Pid, int OrderId) => Invoke(_ => _.TakePartInGroupBuying(UserId, GroupId, Pid, OrderId));

	/// <summary> �û�����ƴ�� </summary>
        public Task<OperationResult<VerificationResultModel>> TakePartInGroupBuyingAsync(Guid UserId, Guid GroupId, string Pid, int OrderId) => InvokeAsync(_ => _.TakePartInGroupBuyingAsync(UserId, GroupId, Pid, OrderId));
		/// <summary> ��ҳ��ȡ���û����ź� </summary>
        public OperationResult<PagedModel<UserGroupBuyingInfoModel>> GetUserGroupInfoByUserId(GroupInfoRequest request) => Invoke(_ => _.GetUserGroupInfoByUserId(request));

	/// <summary> ��ҳ��ȡ���û����ź� </summary>
        public Task<OperationResult<PagedModel<UserGroupBuyingInfoModel>>> GetUserGroupInfoByUserIdAsync(GroupInfoRequest request) => InvokeAsync(_ => _.GetUserGroupInfoByUserIdAsync(request));
		/// <summary> ˢ��ƴ�Ų�Ʒ���� </summary>
        public OperationResult<VerificationResultModel> RefreshCache(string ProductGroupId = null) => Invoke(_ => _.RefreshCache(ProductGroupId));

	/// <summary> ˢ��ƴ�Ų�Ʒ���� </summary>
        public Task<OperationResult<VerificationResultModel>> RefreshCacheAsync(string ProductGroupId = null) => InvokeAsync(_ => _.RefreshCacheAsync(ProductGroupId));
		/// <summary> ˢ��ƴ�Ż��� </summary>
        public OperationResult<VerificationResultModel> RefreshGroupCache(Guid GroupId) => Invoke(_ => _.RefreshGroupCache(GroupId));

	/// <summary> ˢ��ƴ�Ż��� </summary>
        public Task<OperationResult<VerificationResultModel>> RefreshGroupCacheAsync(Guid GroupId) => InvokeAsync(_ => _.RefreshGroupCacheAsync(GroupId));
		/// <summary> �����źţ�UserId��ȡ�û�������Ϣ </summary>
        public OperationResult<UserOrderInfoModel> FetchUserOrderInfo(Guid GroupId, Guid UserId) => Invoke(_ => _.FetchUserOrderInfo(GroupId, UserId));

	/// <summary> �����źţ�UserId��ȡ�û�������Ϣ </summary>
        public Task<OperationResult<UserOrderInfoModel>> FetchUserOrderInfoAsync(Guid GroupId, Guid UserId) => InvokeAsync(_ => _.FetchUserOrderInfoAsync(GroupId, UserId));
		/// <summary> �û�ȡ������ </summary>
        public OperationResult<VerificationResultModel> CancelGroupBuyingOrder(Guid GroupId, int OrderId) => Invoke(_ => _.CancelGroupBuyingOrder(GroupId, OrderId));

	/// <summary> �û�ȡ������ </summary>
        public Task<OperationResult<VerificationResultModel>> CancelGroupBuyingOrderAsync(Guid GroupId, int OrderId) => InvokeAsync(_ => _.CancelGroupBuyingOrderAsync(GroupId, OrderId));
		/// <summary> �ų�������ſɼ� </summary>
        public OperationResult<bool> ChangeGroupBuyingStatus(Guid GroupId, int OrderId) => Invoke(_ => _.ChangeGroupBuyingStatus(GroupId, OrderId));

	/// <summary> �ų�������ſɼ� </summary>
        public Task<OperationResult<bool>> ChangeGroupBuyingStatusAsync(Guid GroupId, int OrderId) => InvokeAsync(_ => _.ChangeGroupBuyingStatusAsync(GroupId, OrderId));
		/// <summary> ��Ա������� </summary>
        public OperationResult<bool> ChangeUserStatus(Guid GroupId, Guid UserId, int OrderId) => Invoke(_ => _.ChangeUserStatus(GroupId, UserId, OrderId));

	/// <summary> ��Ա������� </summary>
        public Task<OperationResult<bool>> ChangeUserStatusAsync(Guid GroupId, Guid UserId, int OrderId) => InvokeAsync(_ => _.ChangeUserStatusAsync(GroupId, UserId, OrderId));
		/// <summary> ƴ�Ź��� </summary>
        public OperationResult<VerificationResultModel> ExpireGroupBuying(Guid GroupId) => Invoke(_ => _.ExpireGroupBuying(GroupId));

	/// <summary> ƴ�Ź��� </summary>
        public Task<OperationResult<VerificationResultModel>> ExpireGroupBuyingAsync(Guid GroupId) => InvokeAsync(_ => _.ExpireGroupBuyingAsync(GroupId));
		/// <summary> ��ȡ��Ʒ���в�Ʒ��ϸ��Ϣ </summary>
        public OperationResult<List<ProductGroupModel>> SelectProductGroupDetail(string ProductGroupId) => Invoke(_ => _.SelectProductGroupDetail(ProductGroupId));

	/// <summary> ��ȡ��Ʒ���в�Ʒ��ϸ��Ϣ </summary>
        public Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupDetailAsync(string ProductGroupId) => InvokeAsync(_ => _.SelectProductGroupDetailAsync(ProductGroupId));
		/// <summary> ����OrderId��ѯ����Ϣ </summary>
        public OperationResult<GroupInfoModel> FetchGroupInfoByOrderId(int OrderId) => Invoke(_ => _.FetchGroupInfoByOrderId(OrderId));

	/// <summary> ����OrderId��ѯ����Ϣ </summary>
        public Task<OperationResult<GroupInfoModel>> FetchGroupInfoByOrderIdAsync(int OrderId) => InvokeAsync(_ => _.FetchGroupInfoByOrderIdAsync(OrderId));
		/// <summary> ����ProductGroupId��ѯ��Ʒ������ </summary>
        public OperationResult<ProductGroupModel> FetchProductGroupInfoById(string ProductGroupId) => Invoke(_ => _.FetchProductGroupInfoById(ProductGroupId));

	/// <summary> ����ProductGroupId��ѯ��Ʒ������ </summary>
        public Task<OperationResult<ProductGroupModel>> FetchProductGroupInfoByIdAsync(string ProductGroupId) => InvokeAsync(_ => _.FetchProductGroupInfoByIdAsync(ProductGroupId));
		/// <summary> ����UserId��OpenIdУ������ </summary>
        public OperationResult<NewUserCheckResultModel> CheckNewUser(Guid userId, string openId = default(string)) => Invoke(_ => _.CheckNewUser(userId,openId));

	/// <summary> ����UserId��OpenIdУ������ </summary>
        public Task<OperationResult<NewUserCheckResultModel>> CheckNewUserAsync(Guid userId, string openId = default(string)) => InvokeAsync(_ => _.CheckNewUserAsync(userId,openId));
		/// <summary> ����PID��ȡ�ò�Ʒ���£����û����Բμӵ����ɸ���(����TotalCount) </summary>
        public OperationResult<GroupInfoResponse> SelectGroupInfoWithTotalCount(string ProductGroupId, Guid UserId, int Count = 100) => Invoke(_ => _.SelectGroupInfoWithTotalCount(ProductGroupId, UserId, Count));

	/// <summary> ����PID��ȡ�ò�Ʒ���£����û����Բμӵ����ɸ���(����TotalCount) </summary>
        public Task<OperationResult<GroupInfoResponse>> SelectGroupInfoWithTotalCountAsync(string ProductGroupId, Guid UserId, int Count = 100) => InvokeAsync(_ => _.SelectGroupInfoWithTotalCountAsync(ProductGroupId, UserId, Count));
		/// <summary>ƴ������ </summary>
        public OperationResult<bool> PushPinTuanMessage(Guid groupId,int batchId) => Invoke(_ => _.PushPinTuanMessage(groupId,batchId));

	/// <summary>ƴ������ </summary>
        public Task<OperationResult<bool>> PushPinTuanMessageAsync(Guid groupId,int batchId) => InvokeAsync(_ => _.PushPinTuanMessageAsync(groupId,batchId));
		/// <summary>����PID��ȡ����ProductGroupId�Լ��۸�</summary>
        public OperationResult<GroupBuyingProductInfo> GetProductGroupInfoByPId(string pId) => Invoke(_ => _.GetProductGroupInfoByPId(pId));

	/// <summary>����PID��ȡ����ProductGroupId�Լ��۸�</summary>
        public Task<OperationResult<GroupBuyingProductInfo>> GetProductGroupInfoByPIdAsync(string pId) => InvokeAsync(_ => _.GetProductGroupInfoByPIdAsync(pId));
		/// <summary>����PID��ȡ��Ӧ��Ʒ����Ϣ(����)</summary>
        public OperationResult<List<ProductGroupModel>> SelectProductListByPids(List<GroupBuyingProductRequest> request) => Invoke(_ => _.SelectProductListByPids(request));

	/// <summary>����PID��ȡ��Ӧ��Ʒ����Ϣ(����)</summary>
        public Task<OperationResult<List<ProductGroupModel>>> SelectProductListByPidsAsync(List<GroupBuyingProductRequest> request) => InvokeAsync(_ => _.SelectProductListByPidsAsync(request));
		/// <summary>��ȡ�齱����</summary>
        public OperationResult<GroupLotteryRuleModel> GetLotteryRule(string productGroupId) => Invoke(_ => _.GetLotteryRule(productGroupId));

	/// <summary>��ȡ�齱����</summary>
        public Task<OperationResult<GroupLotteryRuleModel>> GetLotteryRuleAsync(string productGroupId) => InvokeAsync(_ => _.GetLotteryRuleAsync(productGroupId));
		/// <summary>��ȡ�н�����</summary>
        public OperationResult<PagedModel<GroupBuyingLotteryInfo>> GetWinnerList(string productGroupId, int level = 0, int pageIndex = 1, int pageSize = 20) => Invoke(_ => _.GetWinnerList(productGroupId,level,pageIndex,pageSize));

	/// <summary>��ȡ�н�����</summary>
        public Task<OperationResult<PagedModel<GroupBuyingLotteryInfo>>> GetWinnerListAsync(string productGroupId, int level = 0, int pageIndex = 1, int pageSize = 20) => InvokeAsync(_ => _.GetWinnerListAsync(productGroupId,level,pageIndex,pageSize));
		/// <summary> ��ѯ�û��н�״̬ </summary>
        public OperationResult<GroupBuyingLotteryInfo> CheckUserLotteryResult(Guid userId,string productGroupId,int orderId) => Invoke(_ => _.CheckUserLotteryResult(userId,productGroupId,orderId));

	/// <summary> ��ѯ�û��н�״̬ </summary>
        public Task<OperationResult<GroupBuyingLotteryInfo>> CheckUserLotteryResultAsync(Guid userId,string productGroupId,int orderId) => InvokeAsync(_ => _.CheckUserLotteryResultAsync(userId,productGroupId,orderId));
		/// <summary>��ѯ�û����н���¼</summary>
        public OperationResult<List<GroupBuyingLotteryInfo>> GetUserLotteryHistory(Guid userId, List<int> orderIds) => Invoke(_ => _.GetUserLotteryHistory(userId,orderIds));

	/// <summary>��ѯ�û����н���¼</summary>
        public Task<OperationResult<List<GroupBuyingLotteryInfo>>> GetUserLotteryHistoryAsync(Guid userId, List<int> orderIds) => InvokeAsync(_ => _.GetUserLotteryHistoryAsync(userId,orderIds));
		/// <summary>����ƴ�����ͻ�ȡ��Ʒ�б�</summary>
        public OperationResult<PagedModel<string>> GetActivityProductGroup(ActivityGroupRequest request) => Invoke(_ => _.GetActivityProductGroup(request));

	/// <summary>����ƴ�����ͻ�ȡ��Ʒ�б�</summary>
        public Task<OperationResult<PagedModel<string>>> GetActivityProductGroupAsync(ActivityGroupRequest request) => InvokeAsync(_ => _.GetActivityProductGroupAsync(request));
		/// <summary>��ѯ�û��ⵥȯ</summary>
        public OperationResult<List<FreeCouponModel>> GetUserFreeCoupon(Guid userId) => Invoke(_ => _.GetUserFreeCoupon(userId));

	/// <summary>��ѯ�û��ⵥȯ</summary>
        public Task<OperationResult<List<FreeCouponModel>>> GetUserFreeCouponAsync(Guid userId) => InvokeAsync(_ => _.GetUserFreeCouponAsync(userId));
		/// <summary> ��ȡ�û�ƴ�ż�¼ͳ�� </summary>
        public OperationResult<GroupBuyingHistoryCount> GetUserGroupCountByUserId(Guid userId) => Invoke(_ => _.GetUserGroupCountByUserId(userId));

	/// <summary> ��ȡ�û�ƴ�ż�¼ͳ�� </summary>
        public Task<OperationResult<GroupBuyingHistoryCount>> GetUserGroupCountByUserIdAsync(Guid userId) => InvokeAsync(_ => _.GetUserGroupCountByUserIdAsync(userId));
		/// <summary> ��ȡ���ճ��ŵ��û���Ϣ </summary>
        public OperationResult<List<GroupFinalUserModel>> GetGroupFinalUserList(Guid groupId) => Invoke(_ => _.GetGroupFinalUserList(groupId));

	/// <summary> ��ȡ���ճ��ŵ��û���Ϣ </summary>
        public Task<OperationResult<List<GroupFinalUserModel>>> GetGroupFinalUserListAsync(Guid groupId) => InvokeAsync(_ => _.GetGroupFinalUserListAsync(groupId));
		/// <summary> ��ѯ�û��޹���Ϣ </summary>
        public OperationResult<GroupBuyingBuyLimitModel> GetUserBuyLimitInfo(GroupBuyingBuyLimitRequest request) => Invoke(_ => _.GetUserBuyLimitInfo(request));

	/// <summary> ��ѯ�û��޹���Ϣ </summary>
        public Task<OperationResult<GroupBuyingBuyLimitModel>> GetUserBuyLimitInfoAsync(GroupBuyingBuyLimitRequest request) => InvokeAsync(_ => _.GetUserBuyLimitInfoAsync(request));
		/// <summary> ��ѯƴ��ƴ����Ŀ��Ϣ </summary>
        public OperationResult<List<GroupBuyingCategoryModel>> GetGroupBuyingCategoryInfo() => Invoke(_ => _.GetGroupBuyingCategoryInfo());

	/// <summary> ��ѯƴ��ƴ����Ŀ��Ϣ </summary>
        public Task<OperationResult<List<GroupBuyingCategoryModel>>> GetGroupBuyingCategoryInfoAsync() => InvokeAsync(_ => _.GetGroupBuyingCategoryInfoAsync());
		/// <summary> ��ѯƴ�Ų�Ʒ��Ϣ </summary>
        public OperationResult<PagedModel<SimplegroupBuyingModel>> GetGroupBuyingProductListNew(GroupBuyingQueryRequest request) => Invoke(_ => _.GetGroupBuyingProductListNew(request));

	/// <summary> ��ѯƴ�Ų�Ʒ��Ϣ </summary>
        public Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> GetGroupBuyingProductListNewAsync(GroupBuyingQueryRequest request) => InvokeAsync(_ => _.GetGroupBuyingProductListNewAsync(request));
		/// <summary> ˢ��ES���� </summary>
        public OperationResult<bool> UpdateGroupBuyingInfo(List<string> productGroupIds) => Invoke(_ => _.UpdateGroupBuyingInfo(productGroupIds));

	/// <summary> ˢ��ES���� </summary>
        public Task<OperationResult<bool>> UpdateGroupBuyingInfoAsync(List<string> productGroupIds) => InvokeAsync(_ => _.UpdateGroupBuyingInfoAsync(productGroupIds));
		/// <summary> ���ݹؼ�������ƴ�Ų�Ʒ��Ϣ </summary>
		[Obsolete("�ѷ�������ʹ��SelectGroupBuyingListNewAsync",true)]
        public OperationResult<PagedModel<SimplegroupBuyingModel>> SearchGroupBuyingByKeyword(GroupBuyingQueryRequest request) => Invoke(_ => _.SearchGroupBuyingByKeyword(request));

	/// <summary> ���ݹؼ�������ƴ�Ų�Ʒ��Ϣ </summary>
		[Obsolete("�ѷ�������ʹ��SelectGroupBuyingListNewAsync",true)]
        public Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SearchGroupBuyingByKeywordAsync(GroupBuyingQueryRequest request) => InvokeAsync(_ => _.SearchGroupBuyingByKeywordAsync(request));
		/// <summary> ��ѯƴ�Ų�Ʒ�б� </summary>
        public OperationResult<PagedModel<SimplegroupBuyingModel>> SelectGroupBuyingListNew(GroupBuyingQueryRequest request) => Invoke(_ => _.SelectGroupBuyingListNew(request));

	/// <summary> ��ѯƴ�Ų�Ʒ�б� </summary>
        public Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SelectGroupBuyingListNewAsync(GroupBuyingQueryRequest request) => InvokeAsync(_ => _.SelectGroupBuyingListNewAsync(request));
	}
	///<summary>������ͶƱ</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISexAnnualVoteService
    {
    	/// <summary> ����ŵ걨�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopSignUpResponse")]
        Task<OperationResult<bool>> AddShopSignUpAsync(ShopVoteModel model);
		/// <summary> ��Ӽ�ʦ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddEmployeeSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddEmployeeSignUpResponse")]
        Task<OperationResult<bool>> AddEmployeeSignUpAsync(ShopEmployeeVoteModel model);
		/// <summary> ��֤�ŵ��Ƿ��Ѿ������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckShopSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckShopSignUpResponse")]
        Task<OperationResult<bool>> CheckShopSignUpAsync(long shopId);
		/// <summary> ��֤��ʦ�Ƿ��Ѿ������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckEmployeeSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckEmployeeSignUpResponse")]
        Task<OperationResult<bool>> CheckEmployeeSignUpAsync(long shopId,long employeeId);
		/// <summary> ��ѯ�ŵ�ͶƱ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopRanking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopRankingResponse")]
        Task<OperationResult<PagedModel<ShopVoteBaseModel>>> SelectShopRankingAsync(SexAnnualVoteQueryRequest query);
		/// <summary> ��ѯ��ʦͶƱ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeRanking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeRankingResponse")]
        Task<OperationResult<PagedModel<ShopEmployeeVoteBaseModel>>> SelectShopEmployeeRankingAsync(SexAnnualVoteQueryRequest query);
		/// <summary> ����pkid��ѯ�ŵ�������飨ShopId�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopBaseInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopBaseInfoResponse")]
        Task<OperationResult<ShopVoteModel>> FetchShopBaseInfoAsync(long pkid);
		/// <summary> ��ѯ�ŵ����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopDetailResponse")]
        Task<OperationResult<ShopVoteModel>> FetchShopDetailAsync(long shopId);
		/// <summary> ����pkid��ѯ��ʦ������Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeBaseInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeBaseInfoResponse")]
        Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeBaseInfoAsync(long pkid);
		/// <summary> ��ѯ��ʦ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeDetailResponse")]
        Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeDetailAsync(long shopId,long employeeId);
		/// <summary> ���ŵ�ͶƱ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopVoteResponse")]
        Task<OperationResult<bool>> AddShopVoteAsync(Guid userId,long shopId);
		/// <summary> ��ȡĳ���û�ʱ����ڵ��ŵ�ͶƱ��¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopVoteRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopVoteRecordResponse")]
        Task<OperationResult<IEnumerable<ShopVoteRecordModel>>> SelectShopVoteRecordAsync(Guid userId,DateTime startDate,DateTime endDate);
		/// <summary> �����ŵ�ͶƱ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopVoteResponse")]
        Task<OperationResult<bool>> AddShareShopVoteAsync(Guid userId,long shopId);
		/// <summary> ����ʦͶƱ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopEmployeeVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopEmployeeVoteResponse")]
        Task<OperationResult<bool>> AddShopEmployeeVoteAsync(Guid userId,long shopId,long employeeId);
		/// <summary> ��ȡĳ���û�ʱ����ڵļ�ʦͶƱ��¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeVoteRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeVoteRecordResponse")]
        Task<OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>>> SelectShopEmployeeVoteRecordAsync(Guid userId,DateTime startDate,DateTime endDate);
		/// <summary> ����ʦͶƱ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopEmployeeVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopEmployeeVoteResponse")]
        Task<OperationResult<bool>> AddShareShopEmployeeVoteAsync(Guid userId,long shopId,long employeeId);
		/// <summary> ��ȡ�ŵ걨�����ж������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopRegion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopRegionResponse")]
        Task<OperationResult<IDictionary<int,Shop.Models.RegionModel>>> GetShopRegionAsync();
		/// <summary> ��ȡ��ʦ�������ж������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopEmployeeRegion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopEmployeeRegionResponse")]
        Task<OperationResult<IDictionary<int,Shop.Models.RegionModel>>> GetShopEmployeeRegionAsync();
	}

	///<summary>������ͶƱ</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISexAnnualVoteClient : ISexAnnualVoteService, ITuhuServiceClient
    {
    	/// <summary> ����ŵ걨�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopSignUpResponse")]
        OperationResult<bool> AddShopSignUp(ShopVoteModel model);
		/// <summary> ��Ӽ�ʦ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddEmployeeSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddEmployeeSignUpResponse")]
        OperationResult<bool> AddEmployeeSignUp(ShopEmployeeVoteModel model);
		/// <summary> ��֤�ŵ��Ƿ��Ѿ������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckShopSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckShopSignUpResponse")]
        OperationResult<bool> CheckShopSignUp(long shopId);
		/// <summary> ��֤��ʦ�Ƿ��Ѿ������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckEmployeeSignUp", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/CheckEmployeeSignUpResponse")]
        OperationResult<bool> CheckEmployeeSignUp(long shopId,long employeeId);
		/// <summary> ��ѯ�ŵ�ͶƱ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopRanking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopRankingResponse")]
        OperationResult<PagedModel<ShopVoteBaseModel>> SelectShopRanking(SexAnnualVoteQueryRequest query);
		/// <summary> ��ѯ��ʦͶƱ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeRanking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeRankingResponse")]
        OperationResult<PagedModel<ShopEmployeeVoteBaseModel>> SelectShopEmployeeRanking(SexAnnualVoteQueryRequest query);
		/// <summary> ����pkid��ѯ�ŵ�������飨ShopId�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopBaseInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopBaseInfoResponse")]
        OperationResult<ShopVoteModel> FetchShopBaseInfo(long pkid);
		/// <summary> ��ѯ�ŵ����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopDetailResponse")]
        OperationResult<ShopVoteModel> FetchShopDetail(long shopId);
		/// <summary> ����pkid��ѯ��ʦ������Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeBaseInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeBaseInfoResponse")]
        OperationResult<ShopEmployeeVoteModel> FetchShopEmployeeBaseInfo(long pkid);
		/// <summary> ��ѯ��ʦ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/FetchShopEmployeeDetailResponse")]
        OperationResult<ShopEmployeeVoteModel> FetchShopEmployeeDetail(long shopId,long employeeId);
		/// <summary> ���ŵ�ͶƱ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopVoteResponse")]
        OperationResult<bool> AddShopVote(Guid userId,long shopId);
		/// <summary> ��ȡĳ���û�ʱ����ڵ��ŵ�ͶƱ��¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopVoteRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopVoteRecordResponse")]
        OperationResult<IEnumerable<ShopVoteRecordModel>> SelectShopVoteRecord(Guid userId,DateTime startDate,DateTime endDate);
		/// <summary> �����ŵ�ͶƱ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopVoteResponse")]
        OperationResult<bool> AddShareShopVote(Guid userId,long shopId);
		/// <summary> ����ʦͶƱ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopEmployeeVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShopEmployeeVoteResponse")]
        OperationResult<bool> AddShopEmployeeVote(Guid userId,long shopId,long employeeId);
		/// <summary> ��ȡĳ���û�ʱ����ڵļ�ʦͶƱ��¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeVoteRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/SelectShopEmployeeVoteRecordResponse")]
        OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>> SelectShopEmployeeVoteRecord(Guid userId,DateTime startDate,DateTime endDate);
		/// <summary> ����ʦͶƱ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopEmployeeVote", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/AddShareShopEmployeeVoteResponse")]
        OperationResult<bool> AddShareShopEmployeeVote(Guid userId,long shopId,long employeeId);
		/// <summary> ��ȡ�ŵ걨�����ж������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopRegion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopRegionResponse")]
        OperationResult<IDictionary<int,Shop.Models.RegionModel>> GetShopRegion();
		/// <summary> ��ȡ��ʦ�������ж������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopEmployeeRegion", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SexAnnualVote/GetShopEmployeeRegionResponse")]
        OperationResult<IDictionary<int,Shop.Models.RegionModel>> GetShopEmployeeRegion();
	}

	///<summary>������ͶƱ</summary>///
	public partial class SexAnnualVoteClient : TuhuServiceClient<ISexAnnualVoteClient>, ISexAnnualVoteClient
    {
    	/// <summary> ����ŵ걨�� </summary>
        public OperationResult<bool> AddShopSignUp(ShopVoteModel model) => Invoke(_ => _.AddShopSignUp(model));

	/// <summary> ����ŵ걨�� </summary>
        public Task<OperationResult<bool>> AddShopSignUpAsync(ShopVoteModel model) => InvokeAsync(_ => _.AddShopSignUpAsync(model));
		/// <summary> ��Ӽ�ʦ���� </summary>
        public OperationResult<bool> AddEmployeeSignUp(ShopEmployeeVoteModel model) => Invoke(_ => _.AddEmployeeSignUp(model));

	/// <summary> ��Ӽ�ʦ���� </summary>
        public Task<OperationResult<bool>> AddEmployeeSignUpAsync(ShopEmployeeVoteModel model) => InvokeAsync(_ => _.AddEmployeeSignUpAsync(model));
		/// <summary> ��֤�ŵ��Ƿ��Ѿ������� </summary>
        public OperationResult<bool> CheckShopSignUp(long shopId) => Invoke(_ => _.CheckShopSignUp(shopId));

	/// <summary> ��֤�ŵ��Ƿ��Ѿ������� </summary>
        public Task<OperationResult<bool>> CheckShopSignUpAsync(long shopId) => InvokeAsync(_ => _.CheckShopSignUpAsync(shopId));
		/// <summary> ��֤��ʦ�Ƿ��Ѿ������� </summary>
        public OperationResult<bool> CheckEmployeeSignUp(long shopId,long employeeId) => Invoke(_ => _.CheckEmployeeSignUp(shopId,employeeId));

	/// <summary> ��֤��ʦ�Ƿ��Ѿ������� </summary>
        public Task<OperationResult<bool>> CheckEmployeeSignUpAsync(long shopId,long employeeId) => InvokeAsync(_ => _.CheckEmployeeSignUpAsync(shopId,employeeId));
		/// <summary> ��ѯ�ŵ�ͶƱ���� </summary>
        public OperationResult<PagedModel<ShopVoteBaseModel>> SelectShopRanking(SexAnnualVoteQueryRequest query) => Invoke(_ => _.SelectShopRanking(query));

	/// <summary> ��ѯ�ŵ�ͶƱ���� </summary>
        public Task<OperationResult<PagedModel<ShopVoteBaseModel>>> SelectShopRankingAsync(SexAnnualVoteQueryRequest query) => InvokeAsync(_ => _.SelectShopRankingAsync(query));
		/// <summary> ��ѯ��ʦͶƱ���� </summary>
        public OperationResult<PagedModel<ShopEmployeeVoteBaseModel>> SelectShopEmployeeRanking(SexAnnualVoteQueryRequest query) => Invoke(_ => _.SelectShopEmployeeRanking(query));

	/// <summary> ��ѯ��ʦͶƱ���� </summary>
        public Task<OperationResult<PagedModel<ShopEmployeeVoteBaseModel>>> SelectShopEmployeeRankingAsync(SexAnnualVoteQueryRequest query) => InvokeAsync(_ => _.SelectShopEmployeeRankingAsync(query));
		/// <summary> ����pkid��ѯ�ŵ�������飨ShopId�� </summary>
        public OperationResult<ShopVoteModel> FetchShopBaseInfo(long pkid) => Invoke(_ => _.FetchShopBaseInfo(pkid));

	/// <summary> ����pkid��ѯ�ŵ�������飨ShopId�� </summary>
        public Task<OperationResult<ShopVoteModel>> FetchShopBaseInfoAsync(long pkid) => InvokeAsync(_ => _.FetchShopBaseInfoAsync(pkid));
		/// <summary> ��ѯ�ŵ����� </summary>
        public OperationResult<ShopVoteModel> FetchShopDetail(long shopId) => Invoke(_ => _.FetchShopDetail(shopId));

	/// <summary> ��ѯ�ŵ����� </summary>
        public Task<OperationResult<ShopVoteModel>> FetchShopDetailAsync(long shopId) => InvokeAsync(_ => _.FetchShopDetailAsync(shopId));
		/// <summary> ����pkid��ѯ��ʦ������Ϣ </summary>
        public OperationResult<ShopEmployeeVoteModel> FetchShopEmployeeBaseInfo(long pkid) => Invoke(_ => _.FetchShopEmployeeBaseInfo(pkid));

	/// <summary> ����pkid��ѯ��ʦ������Ϣ </summary>
        public Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeBaseInfoAsync(long pkid) => InvokeAsync(_ => _.FetchShopEmployeeBaseInfoAsync(pkid));
		/// <summary> ��ѯ��ʦ���� </summary>
        public OperationResult<ShopEmployeeVoteModel> FetchShopEmployeeDetail(long shopId,long employeeId) => Invoke(_ => _.FetchShopEmployeeDetail(shopId,employeeId));

	/// <summary> ��ѯ��ʦ���� </summary>
        public Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeDetailAsync(long shopId,long employeeId) => InvokeAsync(_ => _.FetchShopEmployeeDetailAsync(shopId,employeeId));
		/// <summary> ���ŵ�ͶƱ </summary>
        public OperationResult<bool> AddShopVote(Guid userId,long shopId) => Invoke(_ => _.AddShopVote(userId,shopId));

	/// <summary> ���ŵ�ͶƱ </summary>
        public Task<OperationResult<bool>> AddShopVoteAsync(Guid userId,long shopId) => InvokeAsync(_ => _.AddShopVoteAsync(userId,shopId));
		/// <summary> ��ȡĳ���û�ʱ����ڵ��ŵ�ͶƱ��¼ </summary>
        public OperationResult<IEnumerable<ShopVoteRecordModel>> SelectShopVoteRecord(Guid userId,DateTime startDate,DateTime endDate) => Invoke(_ => _.SelectShopVoteRecord(userId,startDate,endDate));

	/// <summary> ��ȡĳ���û�ʱ����ڵ��ŵ�ͶƱ��¼ </summary>
        public Task<OperationResult<IEnumerable<ShopVoteRecordModel>>> SelectShopVoteRecordAsync(Guid userId,DateTime startDate,DateTime endDate) => InvokeAsync(_ => _.SelectShopVoteRecordAsync(userId,startDate,endDate));
		/// <summary> �����ŵ�ͶƱ��Ϣ </summary>
        public OperationResult<bool> AddShareShopVote(Guid userId,long shopId) => Invoke(_ => _.AddShareShopVote(userId,shopId));

	/// <summary> �����ŵ�ͶƱ��Ϣ </summary>
        public Task<OperationResult<bool>> AddShareShopVoteAsync(Guid userId,long shopId) => InvokeAsync(_ => _.AddShareShopVoteAsync(userId,shopId));
		/// <summary> ����ʦͶƱ </summary>
        public OperationResult<bool> AddShopEmployeeVote(Guid userId,long shopId,long employeeId) => Invoke(_ => _.AddShopEmployeeVote(userId,shopId,employeeId));

	/// <summary> ����ʦͶƱ </summary>
        public Task<OperationResult<bool>> AddShopEmployeeVoteAsync(Guid userId,long shopId,long employeeId) => InvokeAsync(_ => _.AddShopEmployeeVoteAsync(userId,shopId,employeeId));
		/// <summary> ��ȡĳ���û�ʱ����ڵļ�ʦͶƱ��¼ </summary>
        public OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>> SelectShopEmployeeVoteRecord(Guid userId,DateTime startDate,DateTime endDate) => Invoke(_ => _.SelectShopEmployeeVoteRecord(userId,startDate,endDate));

	/// <summary> ��ȡĳ���û�ʱ����ڵļ�ʦͶƱ��¼ </summary>
        public Task<OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>>> SelectShopEmployeeVoteRecordAsync(Guid userId,DateTime startDate,DateTime endDate) => InvokeAsync(_ => _.SelectShopEmployeeVoteRecordAsync(userId,startDate,endDate));
		/// <summary> ����ʦͶƱ��Ϣ </summary>
        public OperationResult<bool> AddShareShopEmployeeVote(Guid userId,long shopId,long employeeId) => Invoke(_ => _.AddShareShopEmployeeVote(userId,shopId,employeeId));

	/// <summary> ����ʦͶƱ��Ϣ </summary>
        public Task<OperationResult<bool>> AddShareShopEmployeeVoteAsync(Guid userId,long shopId,long employeeId) => InvokeAsync(_ => _.AddShareShopEmployeeVoteAsync(userId,shopId,employeeId));
		/// <summary> ��ȡ�ŵ걨�����ж������� </summary>
        public OperationResult<IDictionary<int,Shop.Models.RegionModel>> GetShopRegion() => Invoke(_ => _.GetShopRegion());

	/// <summary> ��ȡ�ŵ걨�����ж������� </summary>
        public Task<OperationResult<IDictionary<int,Shop.Models.RegionModel>>> GetShopRegionAsync() => InvokeAsync(_ => _.GetShopRegionAsync());
		/// <summary> ��ȡ��ʦ�������ж������� </summary>
        public OperationResult<IDictionary<int,Shop.Models.RegionModel>> GetShopEmployeeRegion() => Invoke(_ => _.GetShopEmployeeRegion());

	/// <summary> ��ȡ��ʦ�������ж������� </summary>
        public Task<OperationResult<IDictionary<int,Shop.Models.RegionModel>>> GetShopEmployeeRegionAsync() => InvokeAsync(_ => _.GetShopEmployeeRegionAsync());
	}
	///<summary>2017��˫11��Ʒ��Ʒ���������� 2017</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICategoryBrandRankService
    {
    	/// <summary> ��ȡĳ������з���Ʒ���������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CategoryBrandRank/SelectAllCategoryBrandByDate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CategoryBrandRank/SelectAllCategoryBrandByDateResponse")]
        Task<OperationResult<IEnumerable<CategoryBrandRankModel>>> SelectAllCategoryBrandByDateAsync(DateTime date);
	}

	///<summary>2017��˫11��Ʒ��Ʒ���������� 2017</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICategoryBrandRankClient : ICategoryBrandRankService, ITuhuServiceClient
    {
    	/// <summary> ��ȡĳ������з���Ʒ���������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CategoryBrandRank/SelectAllCategoryBrandByDate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CategoryBrandRank/SelectAllCategoryBrandByDateResponse")]
        OperationResult<IEnumerable<CategoryBrandRankModel>> SelectAllCategoryBrandByDate(DateTime date);
	}

	///<summary>2017��˫11��Ʒ��Ʒ���������� 2017</summary>///
	public partial class CategoryBrandRankClient : TuhuServiceClient<ICategoryBrandRankClient>, ICategoryBrandRankClient
    {
    	/// <summary> ��ȡĳ������з���Ʒ���������� </summary>
        public OperationResult<IEnumerable<CategoryBrandRankModel>> SelectAllCategoryBrandByDate(DateTime date) => Invoke(_ => _.SelectAllCategoryBrandByDate(date));

	/// <summary> ��ȡĳ������з���Ʒ���������� </summary>
        public Task<OperationResult<IEnumerable<CategoryBrandRankModel>>> SelectAllCategoryBrandByDateAsync(DateTime date) => InvokeAsync(_ => _.SelectAllCategoryBrandByDateAsync(date));
	}
	/// <summary>�ʾ����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IQuestionnaireService
    {
    	///<summary>��ȡ�û����ʾ�������Ϣ</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireURL", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireURLResponse")]
        Task<OperationResult<string>> GetQuestionnaireURLAsync(GetQuestionnaireURLRequest model);
		///<summary>��ȡ�ʾ���Ϣ</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireInfoResponse")]
        Task<OperationResult<GetQuestionnaireInfoResponse>> GetQuestionnaireInfoAsync(Guid pageId);
		///<summary>�ύ�ʾ�</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/SubmitQuestionnaire", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/SubmitQuestionnaireResponse")]
        Task<OperationResult<bool>> SubmitQuestionnaireAsync(SubmitQuestionnaireRequest model);
	}

	/// <summary>�ʾ����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IQuestionnaireClient : IQuestionnaireService, ITuhuServiceClient
    {
    	///<summary>��ȡ�û����ʾ�������Ϣ</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireURL", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireURLResponse")]
        OperationResult<string> GetQuestionnaireURL(GetQuestionnaireURLRequest model);
		///<summary>��ȡ�ʾ���Ϣ</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/GetQuestionnaireInfoResponse")]
        OperationResult<GetQuestionnaireInfoResponse> GetQuestionnaireInfo(Guid pageId);
		///<summary>�ύ�ʾ�</summary>/// <returns> </returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/SubmitQuestionnaire", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Questionnaire/SubmitQuestionnaireResponse")]
        OperationResult<bool> SubmitQuestionnaire(SubmitQuestionnaireRequest model);
	}

	/// <summary>�ʾ����</summary>
	public partial class QuestionnaireClient : TuhuServiceClient<IQuestionnaireClient>, IQuestionnaireClient
    {
    	///<summary>��ȡ�û����ʾ�������Ϣ</summary>/// <returns> </returns>
        public OperationResult<string> GetQuestionnaireURL(GetQuestionnaireURLRequest model) => Invoke(_ => _.GetQuestionnaireURL(model));

	///<summary>��ȡ�û����ʾ�������Ϣ</summary>/// <returns> </returns>
        public Task<OperationResult<string>> GetQuestionnaireURLAsync(GetQuestionnaireURLRequest model) => InvokeAsync(_ => _.GetQuestionnaireURLAsync(model));
		///<summary>��ȡ�ʾ���Ϣ</summary>/// <returns> </returns>
        public OperationResult<GetQuestionnaireInfoResponse> GetQuestionnaireInfo(Guid pageId) => Invoke(_ => _.GetQuestionnaireInfo(pageId));

	///<summary>��ȡ�ʾ���Ϣ</summary>/// <returns> </returns>
        public Task<OperationResult<GetQuestionnaireInfoResponse>> GetQuestionnaireInfoAsync(Guid pageId) => InvokeAsync(_ => _.GetQuestionnaireInfoAsync(pageId));
		///<summary>�ύ�ʾ�</summary>/// <returns> </returns>
        public OperationResult<bool> SubmitQuestionnaire(SubmitQuestionnaireRequest model) => Invoke(_ => _.SubmitQuestionnaire(model));

	///<summary>�ύ�ʾ�</summary>/// <returns> </returns>
        public Task<OperationResult<bool>> SubmitQuestionnaireAsync(SubmitQuestionnaireRequest model) => InvokeAsync(_ => _.SubmitQuestionnaireAsync(model));
	}
	///<summary>�����</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISalePromotionActivityService
    {
    	/// <summary> ��������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityResponse")]
        Task<OperationResult<bool>> InsertActivityAsync(SalePromotionActivityModel model);
		/// <summary> �޸Ĵ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityResponse")]
        Task<OperationResult<bool>> UpdateActivityAsync(SalePromotionActivityModel model);
		/// <summary> ��˺��޸Ĵ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityAfterAudit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityAfterAuditResponse")]
        Task<OperationResult<bool>> UpdateActivityAfterAuditAsync(SalePromotionActivityModel model);
		/// <summary> �¼ܴ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UnShelveActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UnShelveActivityResponse")]
        Task<OperationResult<bool>> UnShelveActivityAsync(string activityId, string userName);
		/// <summary> ���������������Ʒ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityProductListResponse")]
        Task<OperationResult<bool>> InsertActivityProductListAsync(List<SalePromotionActivityProduct> productList,string activityId,string userName);
		/// <summary> ��������ѯ������б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectActivityList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectActivityListResponse")]
        Task<OperationResult<SelectActivityListModel>> SelectActivityListAsync(SalePromotionActivityModel model, int pageIndex, int pageSize);
		/// <summary> ��ȡ��ۿ������б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityContent", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityContentResponse")]
        Task<OperationResult<List<SalePromotionActivityDiscount>>> GetActivityContentAsync(string activityId);
		/// <summary> ��ȡ���Ϣ���ۿ������б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityInfoResponse")]
        Task<OperationResult<SalePromotionActivityModel>> GetActivityInfoAsync(string activityId);
		/// <summary> ���ݻid���»��Ʒ�޹���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductLimitStock", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductLimitStockResponse")]
        Task<OperationResult<int>> SetProductLimitStockAsync(string activityId, List<string> pidList, int stock, string userName);
		/// <summary>ɾ�������Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteProductFromActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteProductFromActivityResponse")]
        Task<OperationResult<int>> DeleteProductFromActivityAsync(string pid, string activityId,string userName);
		/// <summary>��ȡ�����Ʒ�޹���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetProductInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetProductInfoListResponse")]
        Task<OperationResult<IEnumerable<SalePromotionActivityProduct>>> GetProductInfoListAsync(string activityId, List<string> pidList);
		/// <summary>��ҳ��ѯ�����Ʒ�б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectProductListResponse")]
        Task<OperationResult<PagedModel<SalePromotionActivityProduct>>> SelectProductListAsync(SelectActivityProduct condition, int pageIndex, int pageSize);
		/// <summary>��ȡ�Ѵ��ڵ���Ʒ�б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetRepeatProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetRepeatProductListResponse")]
        Task<OperationResult<IList<SalePromotionActivityProduct>>> GetRepeatProductListAsync(string activityId,List<string> pidList);
		/// <summary>��ȡ�ض�ʱ���ڵ�ǰ���������ظ�����Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityRepeatProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityRepeatProductListResponse")]
        Task<OperationResult<IList<SalePromotionActivityProduct>>> GetActivityRepeatProductListAsync(string activityId, string startTime, string endTime);
		/// <summary>��Ӻ�ɾ�����Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/AddAndDelActivityProduct", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/AddAndDelActivityProductResponse")]
        Task<OperationResult<bool>> AddAndDelActivityProductAsync(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName);
		/// <summary>ͬ����Ʒ��Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/RefreshProduct", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/RefreshProductResponse")]
        Task<OperationResult<bool>> RefreshProductAsync(string activityId, List<SalePromotionActivityProduct> productList);
		/// <summary>���û���״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetActivityAuditStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetActivityAuditStatusResponse")]
        Task<OperationResult<bool>> SetActivityAuditStatusAsync(string activityId, string auditUserName, int auditStatus, string remark);
		/// <summary>��ȡ����״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAuditStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAuditStatusResponse")]
        Task<OperationResult<int>> GetActivityAuditStatusAsync(string activityId);
		/// <summary>���ô�����Ʒ�б�ţƤѢ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductImage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductImageResponse")]
        Task<OperationResult<int>> SetProductImageAsync(string activityId, List<string> pidList, string imgUrl, string userName);
		/// <summary>���ô�����Ʒ����ҳţƤѢ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetDiscountProductDetailImg", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetDiscountProductDetailImgResponse")]
        Task<OperationResult<SetDiscountProductDetailImgResponse>> SetDiscountProductDetailImgAsync(SetDiscountProductDetailImgRequest request);
		/// <summary>��ȡ���Ϣ����Ʒ��Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAndProducts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAndProductsResponse")]
        Task<OperationResult<SalePromotionActivityModel>> GetActivityAndProductsAsync(string activityId, List<string> pidList);
		/// <summary>������������Ȩ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertAuditAuthResponse")]
        Task<OperationResult<int>> InsertAuditAuthAsync(SalePromotionAuditAuth model);
		/// <summary>ɾ����������Ȩ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteAuditAuthResponse")]
        Task<OperationResult<int>> DeleteAuditAuthAsync(int PKID);
		/// <summary>��ȡ��������Ȩ���б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectAuditAuthList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectAuditAuthListResponse")]
        Task<OperationResult<PagedModel<SalePromotionAuditAuth>>> SelectAuditAuthListAsync(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize);
		/// <summary>��֤�û��Ƿ��иô������͵����Ȩ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetUserAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetUserAuditAuthResponse")]
        Task<OperationResult<SalePromotionAuditAuth>> GetUserAuditAuthAsync(int promotionType,string userName);
	}

	///<summary>�����</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISalePromotionActivityClient : ISalePromotionActivityService, ITuhuServiceClient
    {
    	/// <summary> ��������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityResponse")]
        OperationResult<bool> InsertActivity(SalePromotionActivityModel model);
		/// <summary> �޸Ĵ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityResponse")]
        OperationResult<bool> UpdateActivity(SalePromotionActivityModel model);
		/// <summary> ��˺��޸Ĵ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityAfterAudit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UpdateActivityAfterAuditResponse")]
        OperationResult<bool> UpdateActivityAfterAudit(SalePromotionActivityModel model);
		/// <summary> �¼ܴ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UnShelveActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/UnShelveActivityResponse")]
        OperationResult<bool> UnShelveActivity(string activityId, string userName);
		/// <summary> ���������������Ʒ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertActivityProductListResponse")]
        OperationResult<bool> InsertActivityProductList(List<SalePromotionActivityProduct> productList,string activityId,string userName);
		/// <summary> ��������ѯ������б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectActivityList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectActivityListResponse")]
        OperationResult<SelectActivityListModel> SelectActivityList(SalePromotionActivityModel model, int pageIndex, int pageSize);
		/// <summary> ��ȡ��ۿ������б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityContent", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityContentResponse")]
        OperationResult<List<SalePromotionActivityDiscount>> GetActivityContent(string activityId);
		/// <summary> ��ȡ���Ϣ���ۿ������б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityInfoResponse")]
        OperationResult<SalePromotionActivityModel> GetActivityInfo(string activityId);
		/// <summary> ���ݻid���»��Ʒ�޹���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductLimitStock", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductLimitStockResponse")]
        OperationResult<int> SetProductLimitStock(string activityId, List<string> pidList, int stock, string userName);
		/// <summary>ɾ�������Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteProductFromActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteProductFromActivityResponse")]
        OperationResult<int> DeleteProductFromActivity(string pid, string activityId,string userName);
		/// <summary>��ȡ�����Ʒ�޹���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetProductInfoList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetProductInfoListResponse")]
        OperationResult<IEnumerable<SalePromotionActivityProduct>> GetProductInfoList(string activityId, List<string> pidList);
		/// <summary>��ҳ��ѯ�����Ʒ�б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectProductListResponse")]
        OperationResult<PagedModel<SalePromotionActivityProduct>> SelectProductList(SelectActivityProduct condition, int pageIndex, int pageSize);
		/// <summary>��ȡ�Ѵ��ڵ���Ʒ�б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetRepeatProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetRepeatProductListResponse")]
        OperationResult<IList<SalePromotionActivityProduct>> GetRepeatProductList(string activityId,List<string> pidList);
		/// <summary>��ȡ�ض�ʱ���ڵ�ǰ���������ظ�����Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityRepeatProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityRepeatProductListResponse")]
        OperationResult<IList<SalePromotionActivityProduct>> GetActivityRepeatProductList(string activityId, string startTime, string endTime);
		/// <summary>��Ӻ�ɾ�����Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/AddAndDelActivityProduct", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/AddAndDelActivityProductResponse")]
        OperationResult<bool> AddAndDelActivityProduct(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName);
		/// <summary>ͬ����Ʒ��Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/RefreshProduct", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/RefreshProductResponse")]
        OperationResult<bool> RefreshProduct(string activityId, List<SalePromotionActivityProduct> productList);
		/// <summary>���û���״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetActivityAuditStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetActivityAuditStatusResponse")]
        OperationResult<bool> SetActivityAuditStatus(string activityId, string auditUserName, int auditStatus, string remark);
		/// <summary>��ȡ����״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAuditStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAuditStatusResponse")]
        OperationResult<int> GetActivityAuditStatus(string activityId);
		/// <summary>���ô�����Ʒ�б�ţƤѢ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductImage", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetProductImageResponse")]
        OperationResult<int> SetProductImage(string activityId, List<string> pidList, string imgUrl, string userName);
		/// <summary>���ô�����Ʒ����ҳţƤѢ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetDiscountProductDetailImg", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SetDiscountProductDetailImgResponse")]
        OperationResult<SetDiscountProductDetailImgResponse> SetDiscountProductDetailImg(SetDiscountProductDetailImgRequest request);
		/// <summary>��ȡ���Ϣ����Ʒ��Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAndProducts", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetActivityAndProductsResponse")]
        OperationResult<SalePromotionActivityModel> GetActivityAndProducts(string activityId, List<string> pidList);
		/// <summary>������������Ȩ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/InsertAuditAuthResponse")]
        OperationResult<int> InsertAuditAuth(SalePromotionAuditAuth model);
		/// <summary>ɾ����������Ȩ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/DeleteAuditAuthResponse")]
        OperationResult<int> DeleteAuditAuth(int PKID);
		/// <summary>��ȡ��������Ȩ���б�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectAuditAuthList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/SelectAuditAuthListResponse")]
        OperationResult<PagedModel<SalePromotionAuditAuth>> SelectAuditAuthList(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize);
		/// <summary>��֤�û��Ƿ��иô������͵����Ȩ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetUserAuditAuth", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivity/GetUserAuditAuthResponse")]
        OperationResult<SalePromotionAuditAuth> GetUserAuditAuth(int promotionType,string userName);
	}

	///<summary>�����</summary>///
	public partial class SalePromotionActivityClient : TuhuServiceClient<ISalePromotionActivityClient>, ISalePromotionActivityClient
    {
    	/// <summary> ��������� </summary>
        public OperationResult<bool> InsertActivity(SalePromotionActivityModel model) => Invoke(_ => _.InsertActivity(model));

	/// <summary> ��������� </summary>
        public Task<OperationResult<bool>> InsertActivityAsync(SalePromotionActivityModel model) => InvokeAsync(_ => _.InsertActivityAsync(model));
		/// <summary> �޸Ĵ���� </summary>
        public OperationResult<bool> UpdateActivity(SalePromotionActivityModel model) => Invoke(_ => _.UpdateActivity(model));

	/// <summary> �޸Ĵ���� </summary>
        public Task<OperationResult<bool>> UpdateActivityAsync(SalePromotionActivityModel model) => InvokeAsync(_ => _.UpdateActivityAsync(model));
		/// <summary> ��˺��޸Ĵ���� </summary>
        public OperationResult<bool> UpdateActivityAfterAudit(SalePromotionActivityModel model) => Invoke(_ => _.UpdateActivityAfterAudit(model));

	/// <summary> ��˺��޸Ĵ���� </summary>
        public Task<OperationResult<bool>> UpdateActivityAfterAuditAsync(SalePromotionActivityModel model) => InvokeAsync(_ => _.UpdateActivityAfterAuditAsync(model));
		/// <summary> �¼ܴ���� </summary>
        public OperationResult<bool> UnShelveActivity(string activityId, string userName) => Invoke(_ => _.UnShelveActivity(activityId,userName));

	/// <summary> �¼ܴ���� </summary>
        public Task<OperationResult<bool>> UnShelveActivityAsync(string activityId, string userName) => InvokeAsync(_ => _.UnShelveActivityAsync(activityId,userName));
		/// <summary> ���������������Ʒ </summary>
        public OperationResult<bool> InsertActivityProductList(List<SalePromotionActivityProduct> productList,string activityId,string userName) => Invoke(_ => _.InsertActivityProductList(productList,activityId,userName));

	/// <summary> ���������������Ʒ </summary>
        public Task<OperationResult<bool>> InsertActivityProductListAsync(List<SalePromotionActivityProduct> productList,string activityId,string userName) => InvokeAsync(_ => _.InsertActivityProductListAsync(productList,activityId,userName));
		/// <summary> ��������ѯ������б� </summary>
        public OperationResult<SelectActivityListModel> SelectActivityList(SalePromotionActivityModel model, int pageIndex, int pageSize) => Invoke(_ => _.SelectActivityList(model,pageIndex,pageSize));

	/// <summary> ��������ѯ������б� </summary>
        public Task<OperationResult<SelectActivityListModel>> SelectActivityListAsync(SalePromotionActivityModel model, int pageIndex, int pageSize) => InvokeAsync(_ => _.SelectActivityListAsync(model,pageIndex,pageSize));
		/// <summary> ��ȡ��ۿ������б� </summary>
        public OperationResult<List<SalePromotionActivityDiscount>> GetActivityContent(string activityId) => Invoke(_ => _.GetActivityContent(activityId));

	/// <summary> ��ȡ��ۿ������б� </summary>
        public Task<OperationResult<List<SalePromotionActivityDiscount>>> GetActivityContentAsync(string activityId) => InvokeAsync(_ => _.GetActivityContentAsync(activityId));
		/// <summary> ��ȡ���Ϣ���ۿ������б� </summary>
        public OperationResult<SalePromotionActivityModel> GetActivityInfo(string activityId) => Invoke(_ => _.GetActivityInfo(activityId));

	/// <summary> ��ȡ���Ϣ���ۿ������б� </summary>
        public Task<OperationResult<SalePromotionActivityModel>> GetActivityInfoAsync(string activityId) => InvokeAsync(_ => _.GetActivityInfoAsync(activityId));
		/// <summary> ���ݻid���»��Ʒ�޹���� </summary>
        public OperationResult<int> SetProductLimitStock(string activityId, List<string> pidList, int stock, string userName) => Invoke(_ => _.SetProductLimitStock(activityId,pidList,stock,userName));

	/// <summary> ���ݻid���»��Ʒ�޹���� </summary>
        public Task<OperationResult<int>> SetProductLimitStockAsync(string activityId, List<string> pidList, int stock, string userName) => InvokeAsync(_ => _.SetProductLimitStockAsync(activityId,pidList,stock,userName));
		/// <summary>ɾ�������Ʒ</summary>
        public OperationResult<int> DeleteProductFromActivity(string pid, string activityId,string userName) => Invoke(_ => _.DeleteProductFromActivity(pid,activityId,userName));

	/// <summary>ɾ�������Ʒ</summary>
        public Task<OperationResult<int>> DeleteProductFromActivityAsync(string pid, string activityId,string userName) => InvokeAsync(_ => _.DeleteProductFromActivityAsync(pid,activityId,userName));
		/// <summary>��ȡ�����Ʒ�޹���</summary>
        public OperationResult<IEnumerable<SalePromotionActivityProduct>> GetProductInfoList(string activityId, List<string> pidList) => Invoke(_ => _.GetProductInfoList(activityId,pidList));

	/// <summary>��ȡ�����Ʒ�޹���</summary>
        public Task<OperationResult<IEnumerable<SalePromotionActivityProduct>>> GetProductInfoListAsync(string activityId, List<string> pidList) => InvokeAsync(_ => _.GetProductInfoListAsync(activityId,pidList));
		/// <summary>��ҳ��ѯ�����Ʒ�б�</summary>
        public OperationResult<PagedModel<SalePromotionActivityProduct>> SelectProductList(SelectActivityProduct condition, int pageIndex, int pageSize) => Invoke(_ => _.SelectProductList(condition,pageIndex,pageSize));

	/// <summary>��ҳ��ѯ�����Ʒ�б�</summary>
        public Task<OperationResult<PagedModel<SalePromotionActivityProduct>>> SelectProductListAsync(SelectActivityProduct condition, int pageIndex, int pageSize) => InvokeAsync(_ => _.SelectProductListAsync(condition,pageIndex,pageSize));
		/// <summary>��ȡ�Ѵ��ڵ���Ʒ�б�</summary>
        public OperationResult<IList<SalePromotionActivityProduct>> GetRepeatProductList(string activityId,List<string> pidList) => Invoke(_ => _.GetRepeatProductList(activityId,pidList));

	/// <summary>��ȡ�Ѵ��ڵ���Ʒ�б�</summary>
        public Task<OperationResult<IList<SalePromotionActivityProduct>>> GetRepeatProductListAsync(string activityId,List<string> pidList) => InvokeAsync(_ => _.GetRepeatProductListAsync(activityId,pidList));
		/// <summary>��ȡ�ض�ʱ���ڵ�ǰ���������ظ�����Ʒ</summary>
        public OperationResult<IList<SalePromotionActivityProduct>> GetActivityRepeatProductList(string activityId, string startTime, string endTime) => Invoke(_ => _.GetActivityRepeatProductList(activityId,startTime,endTime));

	/// <summary>��ȡ�ض�ʱ���ڵ�ǰ���������ظ�����Ʒ</summary>
        public Task<OperationResult<IList<SalePromotionActivityProduct>>> GetActivityRepeatProductListAsync(string activityId, string startTime, string endTime) => InvokeAsync(_ => _.GetActivityRepeatProductListAsync(activityId,startTime,endTime));
		/// <summary>��Ӻ�ɾ�����Ʒ</summary>
        public OperationResult<bool> AddAndDelActivityProduct(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName) => Invoke(_ => _.AddAndDelActivityProduct(activityId,stock,addList,delList,userName));

	/// <summary>��Ӻ�ɾ�����Ʒ</summary>
        public Task<OperationResult<bool>> AddAndDelActivityProductAsync(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName) => InvokeAsync(_ => _.AddAndDelActivityProductAsync(activityId,stock,addList,delList,userName));
		/// <summary>ͬ����Ʒ��Ϣ</summary>
        public OperationResult<bool> RefreshProduct(string activityId, List<SalePromotionActivityProduct> productList) => Invoke(_ => _.RefreshProduct(activityId,productList));

	/// <summary>ͬ����Ʒ��Ϣ</summary>
        public Task<OperationResult<bool>> RefreshProductAsync(string activityId, List<SalePromotionActivityProduct> productList) => InvokeAsync(_ => _.RefreshProductAsync(activityId,productList));
		/// <summary>���û���״̬</summary>
        public OperationResult<bool> SetActivityAuditStatus(string activityId, string auditUserName, int auditStatus, string remark) => Invoke(_ => _.SetActivityAuditStatus(activityId,auditUserName,auditStatus,remark));

	/// <summary>���û���״̬</summary>
        public Task<OperationResult<bool>> SetActivityAuditStatusAsync(string activityId, string auditUserName, int auditStatus, string remark) => InvokeAsync(_ => _.SetActivityAuditStatusAsync(activityId,auditUserName,auditStatus,remark));
		/// <summary>��ȡ����״̬</summary>
        public OperationResult<int> GetActivityAuditStatus(string activityId) => Invoke(_ => _.GetActivityAuditStatus(activityId));

	/// <summary>��ȡ����״̬</summary>
        public Task<OperationResult<int>> GetActivityAuditStatusAsync(string activityId) => InvokeAsync(_ => _.GetActivityAuditStatusAsync(activityId));
		/// <summary>���ô�����Ʒ�б�ţƤѢ</summary>
        public OperationResult<int> SetProductImage(string activityId, List<string> pidList, string imgUrl, string userName) => Invoke(_ => _.SetProductImage(activityId,pidList,imgUrl,userName));

	/// <summary>���ô�����Ʒ�б�ţƤѢ</summary>
        public Task<OperationResult<int>> SetProductImageAsync(string activityId, List<string> pidList, string imgUrl, string userName) => InvokeAsync(_ => _.SetProductImageAsync(activityId,pidList,imgUrl,userName));
		/// <summary>���ô�����Ʒ����ҳţƤѢ</summary>
        public OperationResult<SetDiscountProductDetailImgResponse> SetDiscountProductDetailImg(SetDiscountProductDetailImgRequest request) => Invoke(_ => _.SetDiscountProductDetailImg(request));

	/// <summary>���ô�����Ʒ����ҳţƤѢ</summary>
        public Task<OperationResult<SetDiscountProductDetailImgResponse>> SetDiscountProductDetailImgAsync(SetDiscountProductDetailImgRequest request) => InvokeAsync(_ => _.SetDiscountProductDetailImgAsync(request));
		/// <summary>��ȡ���Ϣ����Ʒ��Ϣ</summary>
        public OperationResult<SalePromotionActivityModel> GetActivityAndProducts(string activityId, List<string> pidList) => Invoke(_ => _.GetActivityAndProducts(activityId,pidList));

	/// <summary>��ȡ���Ϣ����Ʒ��Ϣ</summary>
        public Task<OperationResult<SalePromotionActivityModel>> GetActivityAndProductsAsync(string activityId, List<string> pidList) => InvokeAsync(_ => _.GetActivityAndProductsAsync(activityId,pidList));
		/// <summary>������������Ȩ��</summary>
        public OperationResult<int> InsertAuditAuth(SalePromotionAuditAuth model) => Invoke(_ => _.InsertAuditAuth(model));

	/// <summary>������������Ȩ��</summary>
        public Task<OperationResult<int>> InsertAuditAuthAsync(SalePromotionAuditAuth model) => InvokeAsync(_ => _.InsertAuditAuthAsync(model));
		/// <summary>ɾ����������Ȩ��</summary>
        public OperationResult<int> DeleteAuditAuth(int PKID) => Invoke(_ => _.DeleteAuditAuth(PKID));

	/// <summary>ɾ����������Ȩ��</summary>
        public Task<OperationResult<int>> DeleteAuditAuthAsync(int PKID) => InvokeAsync(_ => _.DeleteAuditAuthAsync(PKID));
		/// <summary>��ȡ��������Ȩ���б�</summary>
        public OperationResult<PagedModel<SalePromotionAuditAuth>> SelectAuditAuthList(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize) => Invoke(_ => _.SelectAuditAuthList(searchModel,pageIndex,pageSize));

	/// <summary>��ȡ��������Ȩ���б�</summary>
        public Task<OperationResult<PagedModel<SalePromotionAuditAuth>>> SelectAuditAuthListAsync(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize) => InvokeAsync(_ => _.SelectAuditAuthListAsync(searchModel,pageIndex,pageSize));
		/// <summary>��֤�û��Ƿ��иô������͵����Ȩ��</summary>
        public OperationResult<SalePromotionAuditAuth> GetUserAuditAuth(int promotionType,string userName) => Invoke(_ => _.GetUserAuditAuth(promotionType,userName));

	/// <summary>��֤�û��Ƿ��иô������͵����Ȩ��</summary>
        public Task<OperationResult<SalePromotionAuditAuth>> GetUserAuditAuthAsync(int promotionType,string userName) => InvokeAsync(_ => _.GetUserAuditAuthAsync(promotionType,userName));
	}
	///<summary>�������־</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISalePromotionActivityLogService
    {
    	/// <summary> ��ȡ��־�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogListResponse")]
        Task<OperationResult<PagedModel<SalePromotionActivityLogModel>>> GetOperationLogListAsync(string referId, int pageIndex, int pageSize);
		/// <summary> ��ȡ��־�����б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogDetailList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogDetailListResponse")]
        Task<OperationResult<IEnumerable<SalePromotionActivityLogDetail>>> GetOperationLogDetailListAsync(string operationId);
		/// <summary> ������־���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertActivityLogDescription", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertActivityLogDescriptionResponse")]
        Task<OperationResult<bool>> InsertActivityLogDescriptionAsync(SalePromotionActivityLogDescription model);
		/// <summary> ������־����־���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertAcitivityLogAndDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertAcitivityLogAndDetailResponse")]
        Task<OperationResult<bool>> InsertAcitivityLogAndDetailAsync(SalePromotionActivityLogModel model);
	}

	///<summary>�������־</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ISalePromotionActivityLogClient : ISalePromotionActivityLogService, ITuhuServiceClient
    {
    	/// <summary> ��ȡ��־�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogListResponse")]
        OperationResult<PagedModel<SalePromotionActivityLogModel>> GetOperationLogList(string referId, int pageIndex, int pageSize);
		/// <summary> ��ȡ��־�����б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogDetailList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/GetOperationLogDetailListResponse")]
        OperationResult<IEnumerable<SalePromotionActivityLogDetail>> GetOperationLogDetailList(string operationId);
		/// <summary> ������־���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertActivityLogDescription", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertActivityLogDescriptionResponse")]
        OperationResult<bool> InsertActivityLogDescription(SalePromotionActivityLogDescription model);
		/// <summary> ������־����־���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertAcitivityLogAndDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/SalePromotionActivityLog/InsertAcitivityLogAndDetailResponse")]
        OperationResult<bool> InsertAcitivityLogAndDetail(SalePromotionActivityLogModel model);
	}

	///<summary>�������־</summary>///
	public partial class SalePromotionActivityLogClient : TuhuServiceClient<ISalePromotionActivityLogClient>, ISalePromotionActivityLogClient
    {
    	/// <summary> ��ȡ��־�б� </summary>
        public OperationResult<PagedModel<SalePromotionActivityLogModel>> GetOperationLogList(string referId, int pageIndex, int pageSize) => Invoke(_ => _.GetOperationLogList(referId,pageIndex,pageSize));

	/// <summary> ��ȡ��־�б� </summary>
        public Task<OperationResult<PagedModel<SalePromotionActivityLogModel>>> GetOperationLogListAsync(string referId, int pageIndex, int pageSize) => InvokeAsync(_ => _.GetOperationLogListAsync(referId,pageIndex,pageSize));
		/// <summary> ��ȡ��־�����б� </summary>
        public OperationResult<IEnumerable<SalePromotionActivityLogDetail>> GetOperationLogDetailList(string operationId) => Invoke(_ => _.GetOperationLogDetailList(operationId));

	/// <summary> ��ȡ��־�����б� </summary>
        public Task<OperationResult<IEnumerable<SalePromotionActivityLogDetail>>> GetOperationLogDetailListAsync(string operationId) => InvokeAsync(_ => _.GetOperationLogDetailListAsync(operationId));
		/// <summary> ������־���� </summary>
        public OperationResult<bool> InsertActivityLogDescription(SalePromotionActivityLogDescription model) => Invoke(_ => _.InsertActivityLogDescription(model));

	/// <summary> ������־���� </summary>
        public Task<OperationResult<bool>> InsertActivityLogDescriptionAsync(SalePromotionActivityLogDescription model) => InvokeAsync(_ => _.InsertActivityLogDescriptionAsync(model));
		/// <summary> ������־����־���� </summary>
        public OperationResult<bool> InsertAcitivityLogAndDetail(SalePromotionActivityLogModel model) => Invoke(_ => _.InsertAcitivityLogAndDetail(model));

	/// <summary> ������־����־���� </summary>
        public Task<OperationResult<bool>> InsertAcitivityLogAndDetailAsync(SalePromotionActivityLogModel model) => InvokeAsync(_ => _.InsertAcitivityLogAndDetailAsync(model));
	}
	///<summary>���ۻ��Ϣ</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IDiscountActivityInfoService
    {
    	/// <summary> ��ȡ��ǰʱ����Ʒ����Ĵ��ۻ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductDiscountInfoForTag", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductDiscountInfoForTagResponse")]
        Task<OperationResult<IEnumerable<ProductActivityInfoForTag>>> GetProductDiscountInfoForTagAsync(List<string> pidList,DateTime startTime,DateTime endTime);
		/// <summary> ��ȡ��Ʒ�Ĵ��ۻ��Ϣ����Ʒ���û��޹��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserDiscountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserDiscountInfoResponse")]
        Task<OperationResult<IEnumerable<ProductDiscountActivityInfo>>> GetProductAndUserDiscountInfoAsync(List<string> pidList, string userId);
		/// <summary> ����pid���������ش��ۻ������Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserHitDiscountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserHitDiscountInfoResponse")]
        Task<OperationResult<IEnumerable<ProductHitDiscountResponse>>> GetProductAndUserHitDiscountInfoAsync(List<DiscountActivityRequest> requestList, string userId);
		/// <summary> ��������ʱ��¼���ۻ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/SaveCreateOrderDiscountInfoAndSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/SaveCreateOrderDiscountInfoAndSetCacheResponse")]
        Task<OperationResult<bool>> SaveCreateOrderDiscountInfoAndSetCacheAsync(List<DiscountCreateOrderRequest> orderInfoList);
		/// <summary> ȡ������ʱ�޸ļ�¼�Ķ����ۿ���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/UpdateCancelOrderDiscountInfoAndSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/UpdateCancelOrderDiscountInfoAndSetCacheResponse")]
        Task<OperationResult<bool>> UpdateCancelOrderDiscountInfoAndSetCacheAsync(int orderId);
		/// <summary> ������ȡ�û���ѹ������������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetUserActivityBuyNumCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetUserActivityBuyNumCacheResponse")]
        Task<OperationResult<IEnumerable<UserActivityBuyNumModel>>> GetOrSetUserActivityBuyNumCacheAsync(string userId, List<string> activityIdList);
		/// <summary> ������ȡ���Ʒ���������������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetActivityProductSoldNumCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetActivityProductSoldNumCacheResponse")]
        Task<OperationResult<IEnumerable<ActivityPidSoldNumModel>>> GetOrSetActivityProductSoldNumCacheAsync(string activityId, List<string> pidList);
	}

	///<summary>���ۻ��Ϣ</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IDiscountActivityInfoClient : IDiscountActivityInfoService, ITuhuServiceClient
    {
    	/// <summary> ��ȡ��ǰʱ����Ʒ����Ĵ��ۻ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductDiscountInfoForTag", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductDiscountInfoForTagResponse")]
        OperationResult<IEnumerable<ProductActivityInfoForTag>> GetProductDiscountInfoForTag(List<string> pidList,DateTime startTime,DateTime endTime);
		/// <summary> ��ȡ��Ʒ�Ĵ��ۻ��Ϣ����Ʒ���û��޹��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserDiscountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserDiscountInfoResponse")]
        OperationResult<IEnumerable<ProductDiscountActivityInfo>> GetProductAndUserDiscountInfo(List<string> pidList, string userId);
		/// <summary> ����pid���������ش��ۻ������Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserHitDiscountInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetProductAndUserHitDiscountInfoResponse")]
        OperationResult<IEnumerable<ProductHitDiscountResponse>> GetProductAndUserHitDiscountInfo(List<DiscountActivityRequest> requestList, string userId);
		/// <summary> ��������ʱ��¼���ۻ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/SaveCreateOrderDiscountInfoAndSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/SaveCreateOrderDiscountInfoAndSetCacheResponse")]
        OperationResult<bool> SaveCreateOrderDiscountInfoAndSetCache(List<DiscountCreateOrderRequest> orderInfoList);
		/// <summary> ȡ������ʱ�޸ļ�¼�Ķ����ۿ���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/UpdateCancelOrderDiscountInfoAndSetCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/UpdateCancelOrderDiscountInfoAndSetCacheResponse")]
        OperationResult<bool> UpdateCancelOrderDiscountInfoAndSetCache(int orderId);
		/// <summary> ������ȡ�û���ѹ������������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetUserActivityBuyNumCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetUserActivityBuyNumCacheResponse")]
        OperationResult<IEnumerable<UserActivityBuyNumModel>> GetOrSetUserActivityBuyNumCache(string userId, List<string> activityIdList);
		/// <summary> ������ȡ���Ʒ���������������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetActivityProductSoldNumCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DiscountActivityInfo/GetOrSetActivityProductSoldNumCacheResponse")]
        OperationResult<IEnumerable<ActivityPidSoldNumModel>> GetOrSetActivityProductSoldNumCache(string activityId, List<string> pidList);
	}

	///<summary>���ۻ��Ϣ</summary>///
	public partial class DiscountActivityInfoClient : TuhuServiceClient<IDiscountActivityInfoClient>, IDiscountActivityInfoClient
    {
    	/// <summary> ��ȡ��ǰʱ����Ʒ����Ĵ��ۻ��Ϣ </summary>
        public OperationResult<IEnumerable<ProductActivityInfoForTag>> GetProductDiscountInfoForTag(List<string> pidList,DateTime startTime,DateTime endTime) => Invoke(_ => _.GetProductDiscountInfoForTag(pidList,startTime,endTime));

	/// <summary> ��ȡ��ǰʱ����Ʒ����Ĵ��ۻ��Ϣ </summary>
        public Task<OperationResult<IEnumerable<ProductActivityInfoForTag>>> GetProductDiscountInfoForTagAsync(List<string> pidList,DateTime startTime,DateTime endTime) => InvokeAsync(_ => _.GetProductDiscountInfoForTagAsync(pidList,startTime,endTime));
		/// <summary> ��ȡ��Ʒ�Ĵ��ۻ��Ϣ����Ʒ���û��޹��� </summary>
        public OperationResult<IEnumerable<ProductDiscountActivityInfo>> GetProductAndUserDiscountInfo(List<string> pidList, string userId) => Invoke(_ => _.GetProductAndUserDiscountInfo(pidList,userId));

	/// <summary> ��ȡ��Ʒ�Ĵ��ۻ��Ϣ����Ʒ���û��޹��� </summary>
        public Task<OperationResult<IEnumerable<ProductDiscountActivityInfo>>> GetProductAndUserDiscountInfoAsync(List<string> pidList, string userId) => InvokeAsync(_ => _.GetProductAndUserDiscountInfoAsync(pidList,userId));
		/// <summary> ����pid���������ش��ۻ������Ϣ </summary>
        public OperationResult<IEnumerable<ProductHitDiscountResponse>> GetProductAndUserHitDiscountInfo(List<DiscountActivityRequest> requestList, string userId) => Invoke(_ => _.GetProductAndUserHitDiscountInfo(requestList,userId));

	/// <summary> ����pid���������ش��ۻ������Ϣ </summary>
        public Task<OperationResult<IEnumerable<ProductHitDiscountResponse>>> GetProductAndUserHitDiscountInfoAsync(List<DiscountActivityRequest> requestList, string userId) => InvokeAsync(_ => _.GetProductAndUserHitDiscountInfoAsync(requestList,userId));
		/// <summary> ��������ʱ��¼���ۻ��Ϣ </summary>
        public OperationResult<bool> SaveCreateOrderDiscountInfoAndSetCache(List<DiscountCreateOrderRequest> orderInfoList) => Invoke(_ => _.SaveCreateOrderDiscountInfoAndSetCache(orderInfoList));

	/// <summary> ��������ʱ��¼���ۻ��Ϣ </summary>
        public Task<OperationResult<bool>> SaveCreateOrderDiscountInfoAndSetCacheAsync(List<DiscountCreateOrderRequest> orderInfoList) => InvokeAsync(_ => _.SaveCreateOrderDiscountInfoAndSetCacheAsync(orderInfoList));
		/// <summary> ȡ������ʱ�޸ļ�¼�Ķ����ۿ���Ϣ </summary>
        public OperationResult<bool> UpdateCancelOrderDiscountInfoAndSetCache(int orderId) => Invoke(_ => _.UpdateCancelOrderDiscountInfoAndSetCache(orderId));

	/// <summary> ȡ������ʱ�޸ļ�¼�Ķ����ۿ���Ϣ </summary>
        public Task<OperationResult<bool>> UpdateCancelOrderDiscountInfoAndSetCacheAsync(int orderId) => InvokeAsync(_ => _.UpdateCancelOrderDiscountInfoAndSetCacheAsync(orderId));
		/// <summary> ������ȡ�û���ѹ������������� </summary>
        public OperationResult<IEnumerable<UserActivityBuyNumModel>> GetOrSetUserActivityBuyNumCache(string userId, List<string> activityIdList) => Invoke(_ => _.GetOrSetUserActivityBuyNumCache(userId,activityIdList));

	/// <summary> ������ȡ�û���ѹ������������� </summary>
        public Task<OperationResult<IEnumerable<UserActivityBuyNumModel>>> GetOrSetUserActivityBuyNumCacheAsync(string userId, List<string> activityIdList) => InvokeAsync(_ => _.GetOrSetUserActivityBuyNumCacheAsync(userId,activityIdList));
		/// <summary> ������ȡ���Ʒ���������������� </summary>
        public OperationResult<IEnumerable<ActivityPidSoldNumModel>> GetOrSetActivityProductSoldNumCache(string activityId, List<string> pidList) => Invoke(_ => _.GetOrSetActivityProductSoldNumCache(activityId,pidList));

	/// <summary> ������ȡ���Ʒ���������������� </summary>
        public Task<OperationResult<IEnumerable<ActivityPidSoldNumModel>>> GetOrSetActivityProductSoldNumCacheAsync(string activityId, List<string> pidList) => InvokeAsync(_ => _.GetOrSetActivityProductSoldNumCacheAsync(activityId,pidList));
	}
	///<summary>���ںź������</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IOARedEnvelopeService
    {
    	/// <summary> ���ں���������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoResponse")]
        Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoAsync(int officialAccountType=1 );
		/// <summary> ���ں���������� - �޻��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoNoCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoNoCacheResponse")]
        Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoNoCacheAsync(int officialAccountType=1 );
		/// <summary> ���ں����� - �û��Ƿ������ȡ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserVerify", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserVerifyResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserVerifyAsync(OARedEnvelopeUserVerifyRequest request);
		/// <summary> ���ں����� - �û���ȡ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceive", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserReceiveAsync(OARedEnvelopeUserReceiveRequest request);
		/// <summary> ���ں����� - �û���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserInfoResponse")]
        Task<OperationResult<OARedEnvelopeUserInfoResponse>> OARedEnvelopeUserInfoAsync(OARedEnvelopeUserInfoRequest request);
		/// <summary> ���ں����� - �û���ȡ�ص� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveCallback", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveCallbackResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserReceiveCallbackAsync(OARedEnvelopeUserReceiveCallbackRequest request);
		/// <summary> ���ں����� - �����ȡ��̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeReceiveUpdatings", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeReceiveUpdatingsResponse")]
        Task<OperationResult<OARedEnvelopeReceiveUpdatingsResponse>> OARedEnvelopeReceiveUpdatingsAsync(OARedEnvelopeReceiveUpdatingsRequest request);
		/// <summary> ���ں����� - ˢ�»��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeRefreshCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeRefreshCacheResponse")]
        Task<OperationResult<bool>> OARedEnvelopeRefreshCacheAsync();
		/// <summary> ���ں����� - ������ø��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeSettingUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeSettingUpdateResponse")]
        Task<OperationResult<bool>> OARedEnvelopeSettingUpdateAsync(OARedEnvelopeSettingUpdateRequest request);
		/// <summary> ���ں����� - ����ÿ��ͳ�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeStatisticsUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeStatisticsUpdateResponse")]
        Task<OperationResult<bool>> OARedEnvelopeStatisticsUpdateAsync(OARedEnvelopeStatisticsUpdateRequest request);
		/// <summary> ���ں����� - ɾ���û����� Ϊ�˲���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveDeleteResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserReceiveDeleteAsync(Guid userId,int officialAccountType=1);
		/// <summary> ���ں����� - ɾ���û����� Ϊ�˲���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitDeleteResponse")]
        Task<OperationResult<bool>> OARedEnvelopeDailyDataInitDeleteAsync(DateTime date,int officialAccountType=1);
		/// <summary> ���ں����� - ÿ�����ݳ�ʼ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitResponse")]
        Task<OperationResult<bool>> OARedEnvelopeDailyDataInitAsync(OARedEnvelopeDailyDataInitRequest request);
		/// <summary> ���ں����� - ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserShareResponse")]
        Task<OperationResult<bool>> OARedEnvelopeUserShareAsync(OARedEnvelopeUserShareRequest request);
		/// <summary> ��ȡ���ɵ�ȫ��������� Ϊ�˲���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/GetAllOARedEnvelopeDailyData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/GetAllOARedEnvelopeDailyDataResponse")]
        Task<OperationResult<IDictionary<string, OARedEnvelopeObjectResponse>>> GetAllOARedEnvelopeDailyDataAsync(GetAllOARedEnvelopeDailyDataRequest request);
	}

	///<summary>���ںź������</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IOARedEnvelopeClient : IOARedEnvelopeService, ITuhuServiceClient
    {
    	/// <summary> ���ں���������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoResponse")]
        OperationResult<OARedEnvelopeActivityInfoResponse> OARedEnvelopeActivityInfo(int officialAccountType=1 );
		/// <summary> ���ں���������� - �޻��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoNoCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeActivityInfoNoCacheResponse")]
        OperationResult<OARedEnvelopeActivityInfoResponse> OARedEnvelopeActivityInfoNoCache(int officialAccountType=1 );
		/// <summary> ���ں����� - �û��Ƿ������ȡ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserVerify", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserVerifyResponse")]
        OperationResult<bool> OARedEnvelopeUserVerify(OARedEnvelopeUserVerifyRequest request);
		/// <summary> ���ں����� - �û���ȡ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceive", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveResponse")]
        OperationResult<bool> OARedEnvelopeUserReceive(OARedEnvelopeUserReceiveRequest request);
		/// <summary> ���ں����� - �û���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserInfoResponse")]
        OperationResult<OARedEnvelopeUserInfoResponse> OARedEnvelopeUserInfo(OARedEnvelopeUserInfoRequest request);
		/// <summary> ���ں����� - �û���ȡ�ص� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveCallback", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveCallbackResponse")]
        OperationResult<bool> OARedEnvelopeUserReceiveCallback(OARedEnvelopeUserReceiveCallbackRequest request);
		/// <summary> ���ں����� - �����ȡ��̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeReceiveUpdatings", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeReceiveUpdatingsResponse")]
        OperationResult<OARedEnvelopeReceiveUpdatingsResponse> OARedEnvelopeReceiveUpdatings(OARedEnvelopeReceiveUpdatingsRequest request);
		/// <summary> ���ں����� - ˢ�»��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeRefreshCache", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeRefreshCacheResponse")]
        OperationResult<bool> OARedEnvelopeRefreshCache();
		/// <summary> ���ں����� - ������ø��� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeSettingUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeSettingUpdateResponse")]
        OperationResult<bool> OARedEnvelopeSettingUpdate(OARedEnvelopeSettingUpdateRequest request);
		/// <summary> ���ں����� - ����ÿ��ͳ�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeStatisticsUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeStatisticsUpdateResponse")]
        OperationResult<bool> OARedEnvelopeStatisticsUpdate(OARedEnvelopeStatisticsUpdateRequest request);
		/// <summary> ���ں����� - ɾ���û����� Ϊ�˲���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserReceiveDeleteResponse")]
        OperationResult<bool> OARedEnvelopeUserReceiveDelete(Guid userId,int officialAccountType=1);
		/// <summary> ���ں����� - ɾ���û����� Ϊ�˲���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitDeleteResponse")]
        OperationResult<bool> OARedEnvelopeDailyDataInitDelete(DateTime date,int officialAccountType=1);
		/// <summary> ���ں����� - ÿ�����ݳ�ʼ��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeDailyDataInitResponse")]
        OperationResult<bool> OARedEnvelopeDailyDataInit(OARedEnvelopeDailyDataInitRequest request);
		/// <summary> ���ں����� - ����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/OARedEnvelopeUserShareResponse")]
        OperationResult<bool> OARedEnvelopeUserShare(OARedEnvelopeUserShareRequest request);
		/// <summary> ��ȡ���ɵ�ȫ��������� Ϊ�˲���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/GetAllOARedEnvelopeDailyData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/OARedEnvelope/GetAllOARedEnvelopeDailyDataResponse")]
        OperationResult<IDictionary<string, OARedEnvelopeObjectResponse>> GetAllOARedEnvelopeDailyData(GetAllOARedEnvelopeDailyDataRequest request);
	}

	///<summary>���ںź������</summary>///
	public partial class OARedEnvelopeClient : TuhuServiceClient<IOARedEnvelopeClient>, IOARedEnvelopeClient
    {
    	/// <summary> ���ں���������� </summary>
        public OperationResult<OARedEnvelopeActivityInfoResponse> OARedEnvelopeActivityInfo(int officialAccountType=1 ) => Invoke(_ => _.OARedEnvelopeActivityInfo(officialAccountType));

	/// <summary> ���ں���������� </summary>
        public Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoAsync(int officialAccountType=1 ) => InvokeAsync(_ => _.OARedEnvelopeActivityInfoAsync(officialAccountType));
		/// <summary> ���ں���������� - �޻��� </summary>
        public OperationResult<OARedEnvelopeActivityInfoResponse> OARedEnvelopeActivityInfoNoCache(int officialAccountType=1 ) => Invoke(_ => _.OARedEnvelopeActivityInfoNoCache(officialAccountType));

	/// <summary> ���ں���������� - �޻��� </summary>
        public Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoNoCacheAsync(int officialAccountType=1 ) => InvokeAsync(_ => _.OARedEnvelopeActivityInfoNoCacheAsync(officialAccountType));
		/// <summary> ���ں����� - �û��Ƿ������ȡ </summary>
        public OperationResult<bool> OARedEnvelopeUserVerify(OARedEnvelopeUserVerifyRequest request) => Invoke(_ => _.OARedEnvelopeUserVerify(request));

	/// <summary> ���ں����� - �û��Ƿ������ȡ </summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserVerifyAsync(OARedEnvelopeUserVerifyRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserVerifyAsync(request));
		/// <summary> ���ں����� - �û���ȡ </summary>
        public OperationResult<bool> OARedEnvelopeUserReceive(OARedEnvelopeUserReceiveRequest request) => Invoke(_ => _.OARedEnvelopeUserReceive(request));

	/// <summary> ���ں����� - �û���ȡ </summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserReceiveAsync(OARedEnvelopeUserReceiveRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserReceiveAsync(request));
		/// <summary> ���ں����� - �û���Ϣ </summary>
        public OperationResult<OARedEnvelopeUserInfoResponse> OARedEnvelopeUserInfo(OARedEnvelopeUserInfoRequest request) => Invoke(_ => _.OARedEnvelopeUserInfo(request));

	/// <summary> ���ں����� - �û���Ϣ </summary>
        public Task<OperationResult<OARedEnvelopeUserInfoResponse>> OARedEnvelopeUserInfoAsync(OARedEnvelopeUserInfoRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserInfoAsync(request));
		/// <summary> ���ں����� - �û���ȡ�ص� </summary>
        public OperationResult<bool> OARedEnvelopeUserReceiveCallback(OARedEnvelopeUserReceiveCallbackRequest request) => Invoke(_ => _.OARedEnvelopeUserReceiveCallback(request));

	/// <summary> ���ں����� - �û���ȡ�ص� </summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserReceiveCallbackAsync(OARedEnvelopeUserReceiveCallbackRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserReceiveCallbackAsync(request));
		/// <summary> ���ں����� - �����ȡ��̬ </summary>
        public OperationResult<OARedEnvelopeReceiveUpdatingsResponse> OARedEnvelopeReceiveUpdatings(OARedEnvelopeReceiveUpdatingsRequest request) => Invoke(_ => _.OARedEnvelopeReceiveUpdatings(request));

	/// <summary> ���ں����� - �����ȡ��̬ </summary>
        public Task<OperationResult<OARedEnvelopeReceiveUpdatingsResponse>> OARedEnvelopeReceiveUpdatingsAsync(OARedEnvelopeReceiveUpdatingsRequest request) => InvokeAsync(_ => _.OARedEnvelopeReceiveUpdatingsAsync(request));
		/// <summary> ���ں����� - ˢ�»��� </summary>
        public OperationResult<bool> OARedEnvelopeRefreshCache() => Invoke(_ => _.OARedEnvelopeRefreshCache());

	/// <summary> ���ں����� - ˢ�»��� </summary>
        public Task<OperationResult<bool>> OARedEnvelopeRefreshCacheAsync() => InvokeAsync(_ => _.OARedEnvelopeRefreshCacheAsync());
		/// <summary> ���ں����� - ������ø��� </summary>
        public OperationResult<bool> OARedEnvelopeSettingUpdate(OARedEnvelopeSettingUpdateRequest request) => Invoke(_ => _.OARedEnvelopeSettingUpdate(request));

	/// <summary> ���ں����� - ������ø��� </summary>
        public Task<OperationResult<bool>> OARedEnvelopeSettingUpdateAsync(OARedEnvelopeSettingUpdateRequest request) => InvokeAsync(_ => _.OARedEnvelopeSettingUpdateAsync(request));
		/// <summary> ���ں����� - ����ÿ��ͳ�� </summary>
        public OperationResult<bool> OARedEnvelopeStatisticsUpdate(OARedEnvelopeStatisticsUpdateRequest request) => Invoke(_ => _.OARedEnvelopeStatisticsUpdate(request));

	/// <summary> ���ں����� - ����ÿ��ͳ�� </summary>
        public Task<OperationResult<bool>> OARedEnvelopeStatisticsUpdateAsync(OARedEnvelopeStatisticsUpdateRequest request) => InvokeAsync(_ => _.OARedEnvelopeStatisticsUpdateAsync(request));
		/// <summary> ���ں����� - ɾ���û����� Ϊ�˲���</summary>
        public OperationResult<bool> OARedEnvelopeUserReceiveDelete(Guid userId,int officialAccountType=1) => Invoke(_ => _.OARedEnvelopeUserReceiveDelete(userId,officialAccountType));

	/// <summary> ���ں����� - ɾ���û����� Ϊ�˲���</summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserReceiveDeleteAsync(Guid userId,int officialAccountType=1) => InvokeAsync(_ => _.OARedEnvelopeUserReceiveDeleteAsync(userId,officialAccountType));
		/// <summary> ���ں����� - ɾ���û����� Ϊ�˲���</summary>
        public OperationResult<bool> OARedEnvelopeDailyDataInitDelete(DateTime date,int officialAccountType=1) => Invoke(_ => _.OARedEnvelopeDailyDataInitDelete(date,officialAccountType));

	/// <summary> ���ں����� - ɾ���û����� Ϊ�˲���</summary>
        public Task<OperationResult<bool>> OARedEnvelopeDailyDataInitDeleteAsync(DateTime date,int officialAccountType=1) => InvokeAsync(_ => _.OARedEnvelopeDailyDataInitDeleteAsync(date,officialAccountType));
		/// <summary> ���ں����� - ÿ�����ݳ�ʼ��</summary>
        public OperationResult<bool> OARedEnvelopeDailyDataInit(OARedEnvelopeDailyDataInitRequest request) => Invoke(_ => _.OARedEnvelopeDailyDataInit(request));

	/// <summary> ���ں����� - ÿ�����ݳ�ʼ��</summary>
        public Task<OperationResult<bool>> OARedEnvelopeDailyDataInitAsync(OARedEnvelopeDailyDataInitRequest request) => InvokeAsync(_ => _.OARedEnvelopeDailyDataInitAsync(request));
		/// <summary> ���ں����� - ����</summary>
        public OperationResult<bool> OARedEnvelopeUserShare(OARedEnvelopeUserShareRequest request) => Invoke(_ => _.OARedEnvelopeUserShare(request));

	/// <summary> ���ں����� - ����</summary>
        public Task<OperationResult<bool>> OARedEnvelopeUserShareAsync(OARedEnvelopeUserShareRequest request) => InvokeAsync(_ => _.OARedEnvelopeUserShareAsync(request));
		/// <summary> ��ȡ���ɵ�ȫ��������� Ϊ�˲���</summary>
        public OperationResult<IDictionary<string, OARedEnvelopeObjectResponse>> GetAllOARedEnvelopeDailyData(GetAllOARedEnvelopeDailyDataRequest request) => Invoke(_ => _.GetAllOARedEnvelopeDailyData(request));

	/// <summary> ��ȡ���ɵ�ȫ��������� Ϊ�˲���</summary>
        public Task<OperationResult<IDictionary<string, OARedEnvelopeObjectResponse>>> GetAllOARedEnvelopeDailyDataAsync(GetAllOARedEnvelopeDailyDataRequest request) => InvokeAsync(_ => _.GetAllOARedEnvelopeDailyDataAsync(request));
	}
	///<summary>������</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IDragonBallService
    {
    	/// <summary> �û���ǰ��������/�һ����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserInfoResponse")]
        Task<OperationResult<DragonBallUserInfoResponse>> DragonBallUserInfoAsync(DragonBallUserInfoRequest request);
		/// <summary> ���ֲ� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallBroadcast", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallBroadcastResponse")]
        Task<OperationResult<DragonBallBroadcastResponse>> DragonBallBroadcastAsync(int count);
		/// <summary> �û���ȡ�Ľ����б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserLootList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserLootListResponse")]
        Task<OperationResult<DragonBallUserLootListResponse>> DragonBallUserLootListAsync(DragonBallUserLootListRequest request);
		/// <summary> �û������б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionListResponse")]
        Task<OperationResult<DragonBallUserMissionListResponse>> DragonBallUserMissionListAsync(DragonBallUserMissionListRequest request);
		/// <summary> �û�������ȡ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionReward", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionRewardResponse")]
        Task<OperationResult<DragonBallUserMissionRewardResponse>> DragonBallUserMissionRewardAsync(DragonBallUserMissionRewardRequest request);
		/// <summary> �ٻ������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSummon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSummonResponse")]
        Task<OperationResult<DragonBallSummonResponse>> DragonBallSummonAsync(DragonBallSummonRequest request);
		/// <summary> ��ȡ�״̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityInfoResponse")]
        Task<OperationResult<ActivityResponse>> DragonBallActivityInfoAsync();
		/// <summary> ��ȡ�״̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingResponse")]
        Task<OperationResult<DragonBallSettingResponse>> DragonBallSettingAsync();
		/// <summary> ����û����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserShareResponse")]
        Task<OperationResult<bool>> DragonBallUserShareAsync(DragonBallUserShareRequest request);
		/// <summary> ����һ���û����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallCreateUserMissionDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallCreateUserMissionDetailResponse")]
        Task<OperationResult<bool>> DragonBallCreateUserMissionDetailAsync(DragonBallCreateUserMissionDetailRequest request);
		/// <summary> �û�������ʷ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionHistoryList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionHistoryListResponse")]
        Task<OperationResult<DragonBallUserMissionHistoryListResponse>> DragonBallUserMissionHistoryListAsync(DragonBallUserMissionHistoryListRequest request);
		/// <summary> �û�������ʷ��ʼ�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionInit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionInitResponse")]
        Task<OperationResult<bool>> DragonBallUserMissionInitAsync(Guid userId,bool isForce = false);
		/// <summary> ������ - ���» </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityUpdateResponse")]
        Task<OperationResult<bool>> DragonBallActivityUpdateAsync(DragonBallActivityUpdateRequest request);
		/// <summary> ������ - �������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingUpdateResponse")]
        Task<OperationResult<bool>> DragonBallSettingUpdateAsync(DragonBallSettingUpdateRequest request);
		/// <summary> ������ - ɾ���û����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserDataDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserDataDeleteResponse")]
        Task<OperationResult<bool>> DragonBallUserDataDeleteAsync(Guid userId);
		/// <summary> ������ - �����޸��û��������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserUpdateResponse")]
        Task<OperationResult<bool>> DragonBallUserUpdateAsync(Guid userId,int dragonBallCount);
	}

	///<summary>������</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IDragonBallClient : IDragonBallService, ITuhuServiceClient
    {
    	/// <summary> �û���ǰ��������/�һ����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserInfoResponse")]
        OperationResult<DragonBallUserInfoResponse> DragonBallUserInfo(DragonBallUserInfoRequest request);
		/// <summary> ���ֲ� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallBroadcast", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallBroadcastResponse")]
        OperationResult<DragonBallBroadcastResponse> DragonBallBroadcast(int count);
		/// <summary> �û���ȡ�Ľ����б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserLootList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserLootListResponse")]
        OperationResult<DragonBallUserLootListResponse> DragonBallUserLootList(DragonBallUserLootListRequest request);
		/// <summary> �û������б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionListResponse")]
        OperationResult<DragonBallUserMissionListResponse> DragonBallUserMissionList(DragonBallUserMissionListRequest request);
		/// <summary> �û�������ȡ���� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionReward", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionRewardResponse")]
        OperationResult<DragonBallUserMissionRewardResponse> DragonBallUserMissionReward(DragonBallUserMissionRewardRequest request);
		/// <summary> �ٻ������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSummon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSummonResponse")]
        OperationResult<DragonBallSummonResponse> DragonBallSummon(DragonBallSummonRequest request);
		/// <summary> ��ȡ�״̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityInfoResponse")]
        OperationResult<ActivityResponse> DragonBallActivityInfo();
		/// <summary> ��ȡ�״̬ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSetting", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingResponse")]
        OperationResult<DragonBallSettingResponse> DragonBallSetting();
		/// <summary> ����û����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserShareResponse")]
        OperationResult<bool> DragonBallUserShare(DragonBallUserShareRequest request);
		/// <summary> ����һ���û����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallCreateUserMissionDetail", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallCreateUserMissionDetailResponse")]
        OperationResult<bool> DragonBallCreateUserMissionDetail(DragonBallCreateUserMissionDetailRequest request);
		/// <summary> �û�������ʷ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionHistoryList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionHistoryListResponse")]
        OperationResult<DragonBallUserMissionHistoryListResponse> DragonBallUserMissionHistoryList(DragonBallUserMissionHistoryListRequest request);
		/// <summary> �û�������ʷ��ʼ�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionInit", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserMissionInitResponse")]
        OperationResult<bool> DragonBallUserMissionInit(Guid userId,bool isForce = false);
		/// <summary> ������ - ���» </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallActivityUpdateResponse")]
        OperationResult<bool> DragonBallActivityUpdate(DragonBallActivityUpdateRequest request);
		/// <summary> ������ - �������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallSettingUpdateResponse")]
        OperationResult<bool> DragonBallSettingUpdate(DragonBallSettingUpdateRequest request);
		/// <summary> ������ - ɾ���û����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserDataDelete", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserDataDeleteResponse")]
        OperationResult<bool> DragonBallUserDataDelete(Guid userId);
		/// <summary> ������ - �����޸��û��������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserUpdate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/DragonBall/DragonBallUserUpdateResponse")]
        OperationResult<bool> DragonBallUserUpdate(Guid userId,int dragonBallCount);
	}

	///<summary>������</summary>///
	public partial class DragonBallClient : TuhuServiceClient<IDragonBallClient>, IDragonBallClient
    {
    	/// <summary> �û���ǰ��������/�һ����� </summary>
        public OperationResult<DragonBallUserInfoResponse> DragonBallUserInfo(DragonBallUserInfoRequest request) => Invoke(_ => _.DragonBallUserInfo(request));

	/// <summary> �û���ǰ��������/�һ����� </summary>
        public Task<OperationResult<DragonBallUserInfoResponse>> DragonBallUserInfoAsync(DragonBallUserInfoRequest request) => InvokeAsync(_ => _.DragonBallUserInfoAsync(request));
		/// <summary> ���ֲ� </summary>
        public OperationResult<DragonBallBroadcastResponse> DragonBallBroadcast(int count) => Invoke(_ => _.DragonBallBroadcast(count));

	/// <summary> ���ֲ� </summary>
        public Task<OperationResult<DragonBallBroadcastResponse>> DragonBallBroadcastAsync(int count) => InvokeAsync(_ => _.DragonBallBroadcastAsync(count));
		/// <summary> �û���ȡ�Ľ����б� </summary>
        public OperationResult<DragonBallUserLootListResponse> DragonBallUserLootList(DragonBallUserLootListRequest request) => Invoke(_ => _.DragonBallUserLootList(request));

	/// <summary> �û���ȡ�Ľ����б� </summary>
        public Task<OperationResult<DragonBallUserLootListResponse>> DragonBallUserLootListAsync(DragonBallUserLootListRequest request) => InvokeAsync(_ => _.DragonBallUserLootListAsync(request));
		/// <summary> �û������б� </summary>
        public OperationResult<DragonBallUserMissionListResponse> DragonBallUserMissionList(DragonBallUserMissionListRequest request) => Invoke(_ => _.DragonBallUserMissionList(request));

	/// <summary> �û������б� </summary>
        public Task<OperationResult<DragonBallUserMissionListResponse>> DragonBallUserMissionListAsync(DragonBallUserMissionListRequest request) => InvokeAsync(_ => _.DragonBallUserMissionListAsync(request));
		/// <summary> �û�������ȡ���� </summary>
        public OperationResult<DragonBallUserMissionRewardResponse> DragonBallUserMissionReward(DragonBallUserMissionRewardRequest request) => Invoke(_ => _.DragonBallUserMissionReward(request));

	/// <summary> �û�������ȡ���� </summary>
        public Task<OperationResult<DragonBallUserMissionRewardResponse>> DragonBallUserMissionRewardAsync(DragonBallUserMissionRewardRequest request) => InvokeAsync(_ => _.DragonBallUserMissionRewardAsync(request));
		/// <summary> �ٻ������� </summary>
        public OperationResult<DragonBallSummonResponse> DragonBallSummon(DragonBallSummonRequest request) => Invoke(_ => _.DragonBallSummon(request));

	/// <summary> �ٻ������� </summary>
        public Task<OperationResult<DragonBallSummonResponse>> DragonBallSummonAsync(DragonBallSummonRequest request) => InvokeAsync(_ => _.DragonBallSummonAsync(request));
		/// <summary> ��ȡ�״̬ </summary>
        public OperationResult<ActivityResponse> DragonBallActivityInfo() => Invoke(_ => _.DragonBallActivityInfo());

	/// <summary> ��ȡ�״̬ </summary>
        public Task<OperationResult<ActivityResponse>> DragonBallActivityInfoAsync() => InvokeAsync(_ => _.DragonBallActivityInfoAsync());
		/// <summary> ��ȡ�״̬ </summary>
        public OperationResult<DragonBallSettingResponse> DragonBallSetting() => Invoke(_ => _.DragonBallSetting());

	/// <summary> ��ȡ�״̬ </summary>
        public Task<OperationResult<DragonBallSettingResponse>> DragonBallSettingAsync() => InvokeAsync(_ => _.DragonBallSettingAsync());
		/// <summary> ����û����� </summary>
        public OperationResult<bool> DragonBallUserShare(DragonBallUserShareRequest request) => Invoke(_ => _.DragonBallUserShare(request));

	/// <summary> ����û����� </summary>
        public Task<OperationResult<bool>> DragonBallUserShareAsync(DragonBallUserShareRequest request) => InvokeAsync(_ => _.DragonBallUserShareAsync(request));
		/// <summary> ����һ���û����� </summary>
        public OperationResult<bool> DragonBallCreateUserMissionDetail(DragonBallCreateUserMissionDetailRequest request) => Invoke(_ => _.DragonBallCreateUserMissionDetail(request));

	/// <summary> ����һ���û����� </summary>
        public Task<OperationResult<bool>> DragonBallCreateUserMissionDetailAsync(DragonBallCreateUserMissionDetailRequest request) => InvokeAsync(_ => _.DragonBallCreateUserMissionDetailAsync(request));
		/// <summary> �û�������ʷ </summary>
        public OperationResult<DragonBallUserMissionHistoryListResponse> DragonBallUserMissionHistoryList(DragonBallUserMissionHistoryListRequest request) => Invoke(_ => _.DragonBallUserMissionHistoryList(request));

	/// <summary> �û�������ʷ </summary>
        public Task<OperationResult<DragonBallUserMissionHistoryListResponse>> DragonBallUserMissionHistoryListAsync(DragonBallUserMissionHistoryListRequest request) => InvokeAsync(_ => _.DragonBallUserMissionHistoryListAsync(request));
		/// <summary> �û�������ʷ��ʼ�� </summary>
        public OperationResult<bool> DragonBallUserMissionInit(Guid userId,bool isForce = false) => Invoke(_ => _.DragonBallUserMissionInit(userId,isForce));

	/// <summary> �û�������ʷ��ʼ�� </summary>
        public Task<OperationResult<bool>> DragonBallUserMissionInitAsync(Guid userId,bool isForce = false) => InvokeAsync(_ => _.DragonBallUserMissionInitAsync(userId,isForce));
		/// <summary> ������ - ���» </summary>
        public OperationResult<bool> DragonBallActivityUpdate(DragonBallActivityUpdateRequest request) => Invoke(_ => _.DragonBallActivityUpdate(request));

	/// <summary> ������ - ���» </summary>
        public Task<OperationResult<bool>> DragonBallActivityUpdateAsync(DragonBallActivityUpdateRequest request) => InvokeAsync(_ => _.DragonBallActivityUpdateAsync(request));
		/// <summary> ������ - �������� </summary>
        public OperationResult<bool> DragonBallSettingUpdate(DragonBallSettingUpdateRequest request) => Invoke(_ => _.DragonBallSettingUpdate(request));

	/// <summary> ������ - �������� </summary>
        public Task<OperationResult<bool>> DragonBallSettingUpdateAsync(DragonBallSettingUpdateRequest request) => InvokeAsync(_ => _.DragonBallSettingUpdateAsync(request));
		/// <summary> ������ - ɾ���û����� </summary>
        public OperationResult<bool> DragonBallUserDataDelete(Guid userId) => Invoke(_ => _.DragonBallUserDataDelete(userId));

	/// <summary> ������ - ɾ���û����� </summary>
        public Task<OperationResult<bool>> DragonBallUserDataDeleteAsync(Guid userId) => InvokeAsync(_ => _.DragonBallUserDataDeleteAsync(userId));
		/// <summary> ������ - �����޸��û��������� </summary>
        public OperationResult<bool> DragonBallUserUpdate(Guid userId,int dragonBallCount) => Invoke(_ => _.DragonBallUserUpdate(userId,dragonBallCount));

	/// <summary> ������ - �����޸��û��������� </summary>
        public Task<OperationResult<bool>> DragonBallUserUpdateAsync(Guid userId,int dragonBallCount) => InvokeAsync(_ => _.DragonBallUserUpdateAsync(userId,dragonBallCount));
	}
	///<summary>��Ϸ</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IGameService
    {
    	/// <summary> ��ȡ ��Ϸ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameInfoResponse")]
        Task<OperationResult<GetGameInfoResponse>> GetGameInfoAsync(GetGameInfoRequest request);
		/// <summary> ��ȡ ��̱���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameMilepostInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameMilepostInfoResponse")]
        Task<OperationResult<GetGameMilepostInfoResponse>> GetGameMilepostInfoAsync(GetGameMilepostInfoRequest request);
		/// <summary> ��ȡ ��ǰ�û���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserInfoResponse")]
        Task<OperationResult<GetGameUserInfoResponse>> GetGameUserInfoAsync(GetGameUserInfoRequest request);
		/// <summary> �û����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserShareResponse")]
        Task<OperationResult<GameUserShareResponse>> GameUserShareAsync(GameUserShareRequest request);
		/// <summary> �û��һ���Ʒ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserLoot", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserLootResponse")]
        Task<OperationResult<GameUserLootResponse>> GameUserLootAsync(GameUserLootRequest request);
		/// <summary> ��ȡ �û�����������Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserFriendSupport", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserFriendSupportResponse")]
        Task<OperationResult<GetGameUserFriendSupportResponse>> GetGameUserFriendSupportAsync(GetGameUserFriendSupportRequest request);
		/// <summary> �������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserFriendSupport", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserFriendSupportResponse")]
        Task<OperationResult<GameUserFriendSupportResponse>> GameUserFriendSupportAsync(GameUserFriendSupportRequest request);
		/// <summary> ��ȡ �û������֧��ϸ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserDistanceInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserDistanceInfoResponse")]
        Task<OperationResult<GetGameUserDistanceInfoResponse>> GetGameUserDistanceInfoAsync(GetGameUserDistanceInfoRequest request);
		/// <summary> ��ȡ �������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserLootBroadcast", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserLootBroadcastResponse")]
        Task<OperationResult<GetGameUserLootBroadcastResponse>> GetGameUserLootBroadcastAsync(GetGameUserLootBroadcastRequest request);
		/// <summary> ��ȡ �û�������Ϣ��ʣ������������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserSupportInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserSupportInfoResponse")]
        Task<OperationResult<GetGameUserSupportInfoResponse>> GetGameUserSupportInfoAsync(GetGameUserSupportInfoRequest request);
		/// <summary> С��Ϸ - ����״̬����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameOrderTracking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameOrderTrackingResponse")]
        Task<OperationResult<GameOrderTrackingResponse>> GameOrderTrackingAsync(GameOrderTackingRequest request);
		/// <summary> С��Ϸ - ������Ϸ��Ϣ - �ڲ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UpdateGameInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UpdateGameInfoResponse")]
        Task<OperationResult<UpdateGameInfoResponse>> UpdateGameInfoAsync(UpdateGameInfoRequest request);
		/// <summary> С��Ϸ - ɾ����Ϸ����Ա���� - �ڲ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/DeleteGameUserData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/DeleteGameUserDataResponse")]
        Task<OperationResult<bool>> DeleteGameUserDataAsync(DeleteGameUserDataRequest request);
		/// <summary> ��ȡ��Ϸʵʱ���а�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListResponse")]
        Task<OperationResult<GetRankListResponse>> GetRankListAsync(GetRankListAsyncRequest request);
		/// <summary> �û�������Ϸ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UserParticipateGame", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UserParticipateGameResponse")]
        Task<OperationResult<UserParticipateGameResponse>> UserParticipateGameAsync(UserParticipateGameRequest request);
		/// <summary> ��ȡ�û������õĽ�Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetUserLatestPrize", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetUserLatestPrizeResponse")]
        Task<OperationResult<GetUserLatestPrizeResponse>> GetUserLatestPrizeAsync(GetUserLatestPrizeRequest request);
		/// <summary> ��ȡĳ��֮ǰ�Ļ�������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListBeforeDay", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListBeforeDayResponse")]
        Task<OperationResult<GetRankListBeforeDayResponse>> GetRankListBeforeDayAsync(GetRankListBeforeDayRequest request);
	}

	///<summary>��Ϸ</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IGameClient : IGameService, ITuhuServiceClient
    {
    	/// <summary> ��ȡ ��Ϸ��Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameInfoResponse")]
        OperationResult<GetGameInfoResponse> GetGameInfo(GetGameInfoRequest request);
		/// <summary> ��ȡ ��̱���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameMilepostInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameMilepostInfoResponse")]
        OperationResult<GetGameMilepostInfoResponse> GetGameMilepostInfo(GetGameMilepostInfoRequest request);
		/// <summary> ��ȡ ��ǰ�û���Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserInfoResponse")]
        OperationResult<GetGameUserInfoResponse> GetGameUserInfo(GetGameUserInfoRequest request);
		/// <summary> �û����� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserShare", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserShareResponse")]
        OperationResult<GameUserShareResponse> GameUserShare(GameUserShareRequest request);
		/// <summary> �û��һ���Ʒ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserLoot", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserLootResponse")]
        OperationResult<GameUserLootResponse> GameUserLoot(GameUserLootRequest request);
		/// <summary> ��ȡ �û�����������Ϣ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserFriendSupport", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserFriendSupportResponse")]
        OperationResult<GetGameUserFriendSupportResponse> GetGameUserFriendSupport(GetGameUserFriendSupportRequest request);
		/// <summary> �������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserFriendSupport", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameUserFriendSupportResponse")]
        OperationResult<GameUserFriendSupportResponse> GameUserFriendSupport(GameUserFriendSupportRequest request);
		/// <summary> ��ȡ �û������֧��ϸ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserDistanceInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserDistanceInfoResponse")]
        OperationResult<GetGameUserDistanceInfoResponse> GetGameUserDistanceInfo(GetGameUserDistanceInfoRequest request);
		/// <summary> ��ȡ �������� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserLootBroadcast", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserLootBroadcastResponse")]
        OperationResult<GetGameUserLootBroadcastResponse> GetGameUserLootBroadcast(GetGameUserLootBroadcastRequest request);
		/// <summary> ��ȡ �û�������Ϣ��ʣ������������ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserSupportInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetGameUserSupportInfoResponse")]
        OperationResult<GetGameUserSupportInfoResponse> GetGameUserSupportInfo(GetGameUserSupportInfoRequest request);
		/// <summary> С��Ϸ - ����״̬����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameOrderTracking", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GameOrderTrackingResponse")]
        OperationResult<GameOrderTrackingResponse> GameOrderTracking(GameOrderTackingRequest request);
		/// <summary> С��Ϸ - ������Ϸ��Ϣ - �ڲ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UpdateGameInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UpdateGameInfoResponse")]
        OperationResult<UpdateGameInfoResponse> UpdateGameInfo(UpdateGameInfoRequest request);
		/// <summary> С��Ϸ - ɾ����Ϸ����Ա���� - �ڲ���</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/DeleteGameUserData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/DeleteGameUserDataResponse")]
        OperationResult<bool> DeleteGameUserData(DeleteGameUserDataRequest request);
		/// <summary> ��ȡ��Ϸʵʱ���а�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListResponse")]
        OperationResult<GetRankListResponse> GetRankList(GetRankListAsyncRequest request);
		/// <summary> �û�������Ϸ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UserParticipateGame", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/UserParticipateGameResponse")]
        OperationResult<UserParticipateGameResponse> UserParticipateGame(UserParticipateGameRequest request);
		/// <summary> ��ȡ�û������õĽ�Ʒ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetUserLatestPrize", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetUserLatestPrizeResponse")]
        OperationResult<GetUserLatestPrizeResponse> GetUserLatestPrize(GetUserLatestPrizeRequest request);
		/// <summary> ��ȡĳ��֮ǰ�Ļ�������</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListBeforeDay", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Game/GetRankListBeforeDayResponse")]
        OperationResult<GetRankListBeforeDayResponse> GetRankListBeforeDay(GetRankListBeforeDayRequest request);
	}

	///<summary>��Ϸ</summary>///
	public partial class GameClient : TuhuServiceClient<IGameClient>, IGameClient
    {
    	/// <summary> ��ȡ ��Ϸ��Ϣ </summary>
        public OperationResult<GetGameInfoResponse> GetGameInfo(GetGameInfoRequest request) => Invoke(_ => _.GetGameInfo(request));

	/// <summary> ��ȡ ��Ϸ��Ϣ </summary>
        public Task<OperationResult<GetGameInfoResponse>> GetGameInfoAsync(GetGameInfoRequest request) => InvokeAsync(_ => _.GetGameInfoAsync(request));
		/// <summary> ��ȡ ��̱���Ϣ </summary>
        public OperationResult<GetGameMilepostInfoResponse> GetGameMilepostInfo(GetGameMilepostInfoRequest request) => Invoke(_ => _.GetGameMilepostInfo(request));

	/// <summary> ��ȡ ��̱���Ϣ </summary>
        public Task<OperationResult<GetGameMilepostInfoResponse>> GetGameMilepostInfoAsync(GetGameMilepostInfoRequest request) => InvokeAsync(_ => _.GetGameMilepostInfoAsync(request));
		/// <summary> ��ȡ ��ǰ�û���Ϣ </summary>
        public OperationResult<GetGameUserInfoResponse> GetGameUserInfo(GetGameUserInfoRequest request) => Invoke(_ => _.GetGameUserInfo(request));

	/// <summary> ��ȡ ��ǰ�û���Ϣ </summary>
        public Task<OperationResult<GetGameUserInfoResponse>> GetGameUserInfoAsync(GetGameUserInfoRequest request) => InvokeAsync(_ => _.GetGameUserInfoAsync(request));
		/// <summary> �û����� </summary>
        public OperationResult<GameUserShareResponse> GameUserShare(GameUserShareRequest request) => Invoke(_ => _.GameUserShare(request));

	/// <summary> �û����� </summary>
        public Task<OperationResult<GameUserShareResponse>> GameUserShareAsync(GameUserShareRequest request) => InvokeAsync(_ => _.GameUserShareAsync(request));
		/// <summary> �û��һ���Ʒ </summary>
        public OperationResult<GameUserLootResponse> GameUserLoot(GameUserLootRequest request) => Invoke(_ => _.GameUserLoot(request));

	/// <summary> �û��һ���Ʒ </summary>
        public Task<OperationResult<GameUserLootResponse>> GameUserLootAsync(GameUserLootRequest request) => InvokeAsync(_ => _.GameUserLootAsync(request));
		/// <summary> ��ȡ �û�����������Ϣ </summary>
        public OperationResult<GetGameUserFriendSupportResponse> GetGameUserFriendSupport(GetGameUserFriendSupportRequest request) => Invoke(_ => _.GetGameUserFriendSupport(request));

	/// <summary> ��ȡ �û�����������Ϣ </summary>
        public Task<OperationResult<GetGameUserFriendSupportResponse>> GetGameUserFriendSupportAsync(GetGameUserFriendSupportRequest request) => InvokeAsync(_ => _.GetGameUserFriendSupportAsync(request));
		/// <summary> �������� </summary>
        public OperationResult<GameUserFriendSupportResponse> GameUserFriendSupport(GameUserFriendSupportRequest request) => Invoke(_ => _.GameUserFriendSupport(request));

	/// <summary> �������� </summary>
        public Task<OperationResult<GameUserFriendSupportResponse>> GameUserFriendSupportAsync(GameUserFriendSupportRequest request) => InvokeAsync(_ => _.GameUserFriendSupportAsync(request));
		/// <summary> ��ȡ �û������֧��ϸ </summary>
        public OperationResult<GetGameUserDistanceInfoResponse> GetGameUserDistanceInfo(GetGameUserDistanceInfoRequest request) => Invoke(_ => _.GetGameUserDistanceInfo(request));

	/// <summary> ��ȡ �û������֧��ϸ </summary>
        public Task<OperationResult<GetGameUserDistanceInfoResponse>> GetGameUserDistanceInfoAsync(GetGameUserDistanceInfoRequest request) => InvokeAsync(_ => _.GetGameUserDistanceInfoAsync(request));
		/// <summary> ��ȡ �������� </summary>
        public OperationResult<GetGameUserLootBroadcastResponse> GetGameUserLootBroadcast(GetGameUserLootBroadcastRequest request) => Invoke(_ => _.GetGameUserLootBroadcast(request));

	/// <summary> ��ȡ �������� </summary>
        public Task<OperationResult<GetGameUserLootBroadcastResponse>> GetGameUserLootBroadcastAsync(GetGameUserLootBroadcastRequest request) => InvokeAsync(_ => _.GetGameUserLootBroadcastAsync(request));
		/// <summary> ��ȡ �û�������Ϣ��ʣ������������ </summary>
        public OperationResult<GetGameUserSupportInfoResponse> GetGameUserSupportInfo(GetGameUserSupportInfoRequest request) => Invoke(_ => _.GetGameUserSupportInfo(request));

	/// <summary> ��ȡ �û�������Ϣ��ʣ������������ </summary>
        public Task<OperationResult<GetGameUserSupportInfoResponse>> GetGameUserSupportInfoAsync(GetGameUserSupportInfoRequest request) => InvokeAsync(_ => _.GetGameUserSupportInfoAsync(request));
		/// <summary> С��Ϸ - ����״̬����</summary>
        public OperationResult<GameOrderTrackingResponse> GameOrderTracking(GameOrderTackingRequest request) => Invoke(_ => _.GameOrderTracking(request));

	/// <summary> С��Ϸ - ����״̬����</summary>
        public Task<OperationResult<GameOrderTrackingResponse>> GameOrderTrackingAsync(GameOrderTackingRequest request) => InvokeAsync(_ => _.GameOrderTrackingAsync(request));
		/// <summary> С��Ϸ - ������Ϸ��Ϣ - �ڲ���</summary>
        public OperationResult<UpdateGameInfoResponse> UpdateGameInfo(UpdateGameInfoRequest request) => Invoke(_ => _.UpdateGameInfo(request));

	/// <summary> С��Ϸ - ������Ϸ��Ϣ - �ڲ���</summary>
        public Task<OperationResult<UpdateGameInfoResponse>> UpdateGameInfoAsync(UpdateGameInfoRequest request) => InvokeAsync(_ => _.UpdateGameInfoAsync(request));
		/// <summary> С��Ϸ - ɾ����Ϸ����Ա���� - �ڲ���</summary>
        public OperationResult<bool> DeleteGameUserData(DeleteGameUserDataRequest request) => Invoke(_ => _.DeleteGameUserData(request));

	/// <summary> С��Ϸ - ɾ����Ϸ����Ա���� - �ڲ���</summary>
        public Task<OperationResult<bool>> DeleteGameUserDataAsync(DeleteGameUserDataRequest request) => InvokeAsync(_ => _.DeleteGameUserDataAsync(request));
		/// <summary> ��ȡ��Ϸʵʱ���а�</summary>
        public OperationResult<GetRankListResponse> GetRankList(GetRankListAsyncRequest request) => Invoke(_ => _.GetRankList(request));

	/// <summary> ��ȡ��Ϸʵʱ���а�</summary>
        public Task<OperationResult<GetRankListResponse>> GetRankListAsync(GetRankListAsyncRequest request) => InvokeAsync(_ => _.GetRankListAsync(request));
		/// <summary> �û�������Ϸ</summary>
        public OperationResult<UserParticipateGameResponse> UserParticipateGame(UserParticipateGameRequest request) => Invoke(_ => _.UserParticipateGame(request));

	/// <summary> �û�������Ϸ</summary>
        public Task<OperationResult<UserParticipateGameResponse>> UserParticipateGameAsync(UserParticipateGameRequest request) => InvokeAsync(_ => _.UserParticipateGameAsync(request));
		/// <summary> ��ȡ�û������õĽ�Ʒ</summary>
        public OperationResult<GetUserLatestPrizeResponse> GetUserLatestPrize(GetUserLatestPrizeRequest request) => Invoke(_ => _.GetUserLatestPrize(request));

	/// <summary> ��ȡ�û������õĽ�Ʒ</summary>
        public Task<OperationResult<GetUserLatestPrizeResponse>> GetUserLatestPrizeAsync(GetUserLatestPrizeRequest request) => InvokeAsync(_ => _.GetUserLatestPrizeAsync(request));
		/// <summary> ��ȡĳ��֮ǰ�Ļ�������</summary>
        public OperationResult<GetRankListBeforeDayResponse> GetRankListBeforeDay(GetRankListBeforeDayRequest request) => Invoke(_ => _.GetRankListBeforeDay(request));

	/// <summary> ��ȡĳ��֮ǰ�Ļ�������</summary>
        public Task<OperationResult<GetRankListBeforeDayResponse>> GetRankListBeforeDayAsync(GetRankListBeforeDayRequest request) => InvokeAsync(_ => _.GetRankListBeforeDayAsync(request));
	}
	/// <summary>;������CPS����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ITuboAllianceService
    {
    	///<summary>Ӷ����Ʒ�б��ѯ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductListResponse")]
        Task<OperationResult<List<CommissionProductModel>>> GetCommissionProductListAsync(GetCommissionProductListRequest request);
		///<summary>Ӷ����Ʒ�����ѯ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductDetatils", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductDetatilsResponse")]
        Task<OperationResult<CommissionProductModel>> GetCommissionProductDetatilsAsync(GetCommissionProductDetatilsRequest request);
		///<summary>�µ���Ʒ��¼�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CreateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CreateOrderItemRecordResponse")]
        Task<OperationResult<CreateOrderItemRecordResponse>> CreateOrderItemRecordAsync(CreateOrderItemRecordRequest request);
		///<summary>Ӷ�𶩵���Ʒ��¼���½ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/UpdateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/UpdateOrderItemRecordResponse")]
        Task<OperationResult<UpdateOrderItemRecordResponse>> UpdateOrderItemRecordAsync(UpdateOrderItemRecordRequest request);
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityRebate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityRebateResponse")]
        Task<OperationResult<CommodityRebateResponse>> CommodityRebateAsync(CommodityRebateRequest request);
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityDeduction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityDeductionResponse")]
        Task<OperationResult<CommodityDeductionResponse>> CommodityDeductionAsync(CommodityDeductionRequest request);
		///<summary>CPS�޸�Ӷ����ˮ״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CpsUpdateRunning", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CpsUpdateRunningResponse")]
        Task<OperationResult<CpsUpdateRunningResponse>> CpsUpdateRunningAsync(CpsUpdateRunningRequest request);
	}

	/// <summary>;������CPS����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ITuboAllianceClient : ITuboAllianceService, ITuhuServiceClient
    {
    	///<summary>Ӷ����Ʒ�б��ѯ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductListResponse")]
        OperationResult<List<CommissionProductModel>> GetCommissionProductList(GetCommissionProductListRequest request);
		///<summary>Ӷ����Ʒ�����ѯ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductDetatils", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/GetCommissionProductDetatilsResponse")]
        OperationResult<CommissionProductModel> GetCommissionProductDetatils(GetCommissionProductDetatilsRequest request);
		///<summary>�µ���Ʒ��¼�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CreateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CreateOrderItemRecordResponse")]
        OperationResult<CreateOrderItemRecordResponse> CreateOrderItemRecord(CreateOrderItemRecordRequest request);
		///<summary>Ӷ�𶩵���Ʒ��¼���½ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/UpdateOrderItemRecord", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/UpdateOrderItemRecordResponse")]
        OperationResult<UpdateOrderItemRecordResponse> UpdateOrderItemRecord(UpdateOrderItemRecordRequest request);
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityRebate", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityRebateResponse")]
        OperationResult<CommodityRebateResponse> CommodityRebate(CommodityRebateRequest request);
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityDeduction", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CommodityDeductionResponse")]
        OperationResult<CommodityDeductionResponse> CommodityDeduction(CommodityDeductionRequest request);
		///<summary>CPS�޸�Ӷ����ˮ״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CpsUpdateRunning", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/TuboAlliance/CpsUpdateRunningResponse")]
        OperationResult<CpsUpdateRunningResponse> CpsUpdateRunning(CpsUpdateRunningRequest request);
	}

	/// <summary>;������CPS����</summary>
	public partial class TuboAllianceClient : TuhuServiceClient<ITuboAllianceClient>, ITuboAllianceClient
    {
    	///<summary>Ӷ����Ʒ�б��ѯ�ӿ�</summary>
        public OperationResult<List<CommissionProductModel>> GetCommissionProductList(GetCommissionProductListRequest request) => Invoke(_ => _.GetCommissionProductList(request));

	///<summary>Ӷ����Ʒ�б��ѯ�ӿ�</summary>
        public Task<OperationResult<List<CommissionProductModel>>> GetCommissionProductListAsync(GetCommissionProductListRequest request) => InvokeAsync(_ => _.GetCommissionProductListAsync(request));
		///<summary>Ӷ����Ʒ�����ѯ�ӿ�</summary>
        public OperationResult<CommissionProductModel> GetCommissionProductDetatils(GetCommissionProductDetatilsRequest request) => Invoke(_ => _.GetCommissionProductDetatils(request));

	///<summary>Ӷ����Ʒ�����ѯ�ӿ�</summary>
        public Task<OperationResult<CommissionProductModel>> GetCommissionProductDetatilsAsync(GetCommissionProductDetatilsRequest request) => InvokeAsync(_ => _.GetCommissionProductDetatilsAsync(request));
		///<summary>�µ���Ʒ��¼�ӿ�</summary>
        public OperationResult<CreateOrderItemRecordResponse> CreateOrderItemRecord(CreateOrderItemRecordRequest request) => Invoke(_ => _.CreateOrderItemRecord(request));

	///<summary>�µ���Ʒ��¼�ӿ�</summary>
        public Task<OperationResult<CreateOrderItemRecordResponse>> CreateOrderItemRecordAsync(CreateOrderItemRecordRequest request) => InvokeAsync(_ => _.CreateOrderItemRecordAsync(request));
		///<summary>Ӷ�𶩵���Ʒ��¼���½ӿ�</summary>
        public OperationResult<UpdateOrderItemRecordResponse> UpdateOrderItemRecord(UpdateOrderItemRecordRequest request) => Invoke(_ => _.UpdateOrderItemRecord(request));

	///<summary>Ӷ�𶩵���Ʒ��¼���½ӿ�</summary>
        public Task<OperationResult<UpdateOrderItemRecordResponse>> UpdateOrderItemRecordAsync(UpdateOrderItemRecordRequest request) => InvokeAsync(_ => _.UpdateOrderItemRecordAsync(request));
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        public OperationResult<CommodityRebateResponse> CommodityRebate(CommodityRebateRequest request) => Invoke(_ => _.CommodityRebate(request));

	///<summary>������Ʒ��Ӷ�ӿ�</summary>
        public Task<OperationResult<CommodityRebateResponse>> CommodityRebateAsync(CommodityRebateRequest request) => InvokeAsync(_ => _.CommodityRebateAsync(request));
		///<summary>������Ʒ��Ӷ�ӿ�</summary>
        public OperationResult<CommodityDeductionResponse> CommodityDeduction(CommodityDeductionRequest request) => Invoke(_ => _.CommodityDeduction(request));

	///<summary>������Ʒ��Ӷ�ӿ�</summary>
        public Task<OperationResult<CommodityDeductionResponse>> CommodityDeductionAsync(CommodityDeductionRequest request) => InvokeAsync(_ => _.CommodityDeductionAsync(request));
		///<summary>CPS�޸�Ӷ����ˮ״̬</summary>
        public OperationResult<CpsUpdateRunningResponse> CpsUpdateRunning(CpsUpdateRunningRequest request) => Invoke(_ => _.CpsUpdateRunning(request));

	///<summary>CPS�޸�Ӷ����ˮ״̬</summary>
        public Task<OperationResult<CpsUpdateRunningResponse>> CpsUpdateRunningAsync(CpsUpdateRunningRequest request) => InvokeAsync(_ => _.CpsUpdateRunningAsync(request));
	}
	///<summary>����Ⱥ����</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICarFriendsGroupService
    {
    	/// <summary> ��ȡ����Ⱥ�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupListResponse")]
        Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupListAsync(GetCarFriendsGroupListRequest request);
		/// <summary> ��ȡ�������ų���  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetRecommendVehicleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetRecommendVehicleListResponse")]
        Task<OperationResult<List<RecommendVehicleResponse>>> GetRecommendVehicleListAsync();
		/// <summary> ����pkid��ȡ����Ⱥ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupModelResponse")]
        Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupModelAsync(int pkid);
		/// <summary> ����pkid��ȡ;������Ա��Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsAdministratorsModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsAdministratorsModelResponse")]
        Task<OperationResult<CarFriendsAdministratorsResponse>> GetCarFriendsAdministratorsModelAsync(int pkid);
		/// <summary> ����ȺС�������ͳ���Ⱥ��Ⱥ����Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupPushInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupPushInfoResponse")]
        Task<OperationResult<bool>> CarFriendsGroupPushInfoAsync(GetCarFriendsGroupPushInfoRequest request);
		/// <summary> ����MQ�ӳ�����  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupMqDelayPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupMqDelayPushResponse")]
        Task<OperationResult<bool>> CarFriendsGroupMqDelayPushAsync(GetCarFriendsGroupPushInfoRequest request);
	}

	///<summary>����Ⱥ����</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ICarFriendsGroupClient : ICarFriendsGroupService, ITuhuServiceClient
    {
    	/// <summary> ��ȡ����Ⱥ�б� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupListResponse")]
        OperationResult<CarFriendsGroupInfoResponse> GetCarFriendsGroupList(GetCarFriendsGroupListRequest request);
		/// <summary> ��ȡ�������ų���  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetRecommendVehicleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetRecommendVehicleListResponse")]
        OperationResult<List<RecommendVehicleResponse>> GetRecommendVehicleList();
		/// <summary> ����pkid��ȡ����Ⱥ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsGroupModelResponse")]
        OperationResult<CarFriendsGroupInfoResponse> GetCarFriendsGroupModel(int pkid);
		/// <summary> ����pkid��ȡ;������Ա��Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsAdministratorsModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/GetCarFriendsAdministratorsModelResponse")]
        OperationResult<CarFriendsAdministratorsResponse> GetCarFriendsAdministratorsModel(int pkid);
		/// <summary> ����ȺС�������ͳ���Ⱥ��Ⱥ����Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupPushInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupPushInfoResponse")]
        OperationResult<bool> CarFriendsGroupPushInfo(GetCarFriendsGroupPushInfoRequest request);
		/// <summary> ����MQ�ӳ�����  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupMqDelayPush", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/CarFriendsGroup/CarFriendsGroupMqDelayPushResponse")]
        OperationResult<bool> CarFriendsGroupMqDelayPush(GetCarFriendsGroupPushInfoRequest request);
	}

	///<summary>����Ⱥ����</summary>///
	public partial class CarFriendsGroupClient : TuhuServiceClient<ICarFriendsGroupClient>, ICarFriendsGroupClient
    {
    	/// <summary> ��ȡ����Ⱥ�б� </summary>
        public OperationResult<CarFriendsGroupInfoResponse> GetCarFriendsGroupList(GetCarFriendsGroupListRequest request) => Invoke(_ => _.GetCarFriendsGroupList(request));

	/// <summary> ��ȡ����Ⱥ�б� </summary>
        public Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupListAsync(GetCarFriendsGroupListRequest request) => InvokeAsync(_ => _.GetCarFriendsGroupListAsync(request));
		/// <summary> ��ȡ�������ų���  </summary>
        public OperationResult<List<RecommendVehicleResponse>> GetRecommendVehicleList() => Invoke(_ => _.GetRecommendVehicleList());

	/// <summary> ��ȡ�������ų���  </summary>
        public Task<OperationResult<List<RecommendVehicleResponse>>> GetRecommendVehicleListAsync() => InvokeAsync(_ => _.GetRecommendVehicleListAsync());
		/// <summary> ����pkid��ȡ����Ⱥ  </summary>
        public OperationResult<CarFriendsGroupInfoResponse> GetCarFriendsGroupModel(int pkid) => Invoke(_ => _.GetCarFriendsGroupModel(pkid));

	/// <summary> ����pkid��ȡ����Ⱥ  </summary>
        public Task<OperationResult<CarFriendsGroupInfoResponse>> GetCarFriendsGroupModelAsync(int pkid) => InvokeAsync(_ => _.GetCarFriendsGroupModelAsync(pkid));
		/// <summary> ����pkid��ȡ;������Ա��Ϣ  </summary>
        public OperationResult<CarFriendsAdministratorsResponse> GetCarFriendsAdministratorsModel(int pkid) => Invoke(_ => _.GetCarFriendsAdministratorsModel(pkid));

	/// <summary> ����pkid��ȡ;������Ա��Ϣ  </summary>
        public Task<OperationResult<CarFriendsAdministratorsResponse>> GetCarFriendsAdministratorsModelAsync(int pkid) => InvokeAsync(_ => _.GetCarFriendsAdministratorsModelAsync(pkid));
		/// <summary> ����ȺС�������ͳ���Ⱥ��Ⱥ����Ϣ  </summary>
        public OperationResult<bool> CarFriendsGroupPushInfo(GetCarFriendsGroupPushInfoRequest request) => Invoke(_ => _.CarFriendsGroupPushInfo(request));

	/// <summary> ����ȺС�������ͳ���Ⱥ��Ⱥ����Ϣ  </summary>
        public Task<OperationResult<bool>> CarFriendsGroupPushInfoAsync(GetCarFriendsGroupPushInfoRequest request) => InvokeAsync(_ => _.CarFriendsGroupPushInfoAsync(request));
		/// <summary> ����MQ�ӳ�����  </summary>
        public OperationResult<bool> CarFriendsGroupMqDelayPush(GetCarFriendsGroupPushInfoRequest request) => Invoke(_ => _.CarFriendsGroupMqDelayPush(request));

	/// <summary> ����MQ�ӳ�����  </summary>
        public Task<OperationResult<bool>> CarFriendsGroupMqDelayPushAsync(GetCarFriendsGroupPushInfoRequest request) => InvokeAsync(_ => _.CarFriendsGroupMqDelayPushAsync(request));
	}
	///<summary>һ��Ǯϴ������</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IWashCarCouponService
    {
    	/// <summary> ���� һ��Ǯϴ���Ż�ȯ��ȡ��¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/CreateWashCarCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/CreateWashCarCouponResponse")]
        Task<OperationResult<bool>> CreateWashCarCouponAsync(WashCarCouponRecordModel request);
		/// <summary> ����userid��ȡ  һ��Ǯϴ���Ż�ȯ��ȡ��¼  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponListByUserids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponListByUseridsResponse")]
        Task<OperationResult<List<WashCarCouponRecordModel>>> GetWashCarCouponListByUseridsAsync(GetWashCarCouponListByUseridsRequest request);
		/// <summary> �����Ż�ȯid��ȡ  һ��Ǯϴ���Ż�ȯ��ȡ��¼  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponInfoByPromotionCodeID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponInfoByPromotionCodeIDResponse")]
        Task<OperationResult<WashCarCouponRecordModel>> GetWashCarCouponInfoByPromotionCodeIDAsync(GetWashCarCouponInfoByPromotionCodeIDRequest request);
	}

	///<summary>һ��Ǯϴ������</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IWashCarCouponClient : IWashCarCouponService, ITuhuServiceClient
    {
    	/// <summary> ���� һ��Ǯϴ���Ż�ȯ��ȡ��¼ </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/CreateWashCarCoupon", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/CreateWashCarCouponResponse")]
        OperationResult<bool> CreateWashCarCoupon(WashCarCouponRecordModel request);
		/// <summary> ����userid��ȡ  һ��Ǯϴ���Ż�ȯ��ȡ��¼  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponListByUserids", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponListByUseridsResponse")]
        OperationResult<List<WashCarCouponRecordModel>> GetWashCarCouponListByUserids(GetWashCarCouponListByUseridsRequest request);
		/// <summary> �����Ż�ȯid��ȡ  һ��Ǯϴ���Ż�ȯ��ȡ��¼  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponInfoByPromotionCodeID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/WashCarCoupon/GetWashCarCouponInfoByPromotionCodeIDResponse")]
        OperationResult<WashCarCouponRecordModel> GetWashCarCouponInfoByPromotionCodeID(GetWashCarCouponInfoByPromotionCodeIDRequest request);
	}

	///<summary>һ��Ǯϴ������</summary>///
	public partial class WashCarCouponClient : TuhuServiceClient<IWashCarCouponClient>, IWashCarCouponClient
    {
    	/// <summary> ���� һ��Ǯϴ���Ż�ȯ��ȡ��¼ </summary>
        public OperationResult<bool> CreateWashCarCoupon(WashCarCouponRecordModel request) => Invoke(_ => _.CreateWashCarCoupon(request));

	/// <summary> ���� һ��Ǯϴ���Ż�ȯ��ȡ��¼ </summary>
        public Task<OperationResult<bool>> CreateWashCarCouponAsync(WashCarCouponRecordModel request) => InvokeAsync(_ => _.CreateWashCarCouponAsync(request));
		/// <summary> ����userid��ȡ  һ��Ǯϴ���Ż�ȯ��ȡ��¼  </summary>
        public OperationResult<List<WashCarCouponRecordModel>> GetWashCarCouponListByUserids(GetWashCarCouponListByUseridsRequest request) => Invoke(_ => _.GetWashCarCouponListByUserids(request));

	/// <summary> ����userid��ȡ  һ��Ǯϴ���Ż�ȯ��ȡ��¼  </summary>
        public Task<OperationResult<List<WashCarCouponRecordModel>>> GetWashCarCouponListByUseridsAsync(GetWashCarCouponListByUseridsRequest request) => InvokeAsync(_ => _.GetWashCarCouponListByUseridsAsync(request));
		/// <summary> �����Ż�ȯid��ȡ  һ��Ǯϴ���Ż�ȯ��ȡ��¼  </summary>
        public OperationResult<WashCarCouponRecordModel> GetWashCarCouponInfoByPromotionCodeID(GetWashCarCouponInfoByPromotionCodeIDRequest request) => Invoke(_ => _.GetWashCarCouponInfoByPromotionCodeID(request));

	/// <summary> �����Ż�ȯid��ȡ  һ��Ǯϴ���Ż�ȯ��ȡ��¼  </summary>
        public Task<OperationResult<WashCarCouponRecordModel>> GetWashCarCouponInfoByPromotionCodeIDAsync(GetWashCarCouponInfoByPromotionCodeIDRequest request) => InvokeAsync(_ => _.GetWashCarCouponInfoByPromotionCodeIDAsync(request));
	}
	///<summary>����Ϸ���</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ILuckyCharmService
    {
    	/// <summary> ���� � </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmActivityResponse")]
        Task<OperationResult<bool>> AddLuckyCharmActivityAsync(AddLuckyCharmActivityRequest request);
		/// <summary> ��ȡ�����  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/GetLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/GetLuckyCharmActivityResponse")]
        Task<OperationResult<LuckyCharmActivityInfoResponse>> GetLuckyCharmActivityAsync(int pkid);
		/// <summary> �û������  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmUserResponse")]
        Task<OperationResult<bool>> AddLuckyCharmUserAsync(AddLuckyCharmUserRequest request);
		/// <summary> �޸��û�������Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/UpdateLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/UpdateLuckyCharmUserResponse")]
        Task<OperationResult<bool>> UpdateLuckyCharmUserAsync(UpdateLuckyCharmUserRequest request);
		/// <summary> ��ҳ��ȡ�û�������Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmUserResponse")]
        Task<OperationResult<PageLuckyCharmUserResponse>> PageLuckyCharmUserAsync(PageLuckyCharmUserRequest request);
		/// <summary> ��˱����û�  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AuditLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AuditLuckyCharmUserResponse")]
        Task<OperationResult<bool>> AuditLuckyCharmUserAsync(int pkid);
		/// <summary> ɾ�������û�  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmUserResponse")]
        Task<OperationResult<bool>> DelLuckyCharmUserAsync(int pkid);
		/// <summary> ɾ���  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmActivityResponse")]
        Task<OperationResult<bool>> DelLuckyCharmActivityAsync(int pkid);
		/// <summary> ��ҳ��ȡ���Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmActivityResponse")]
        Task<OperationResult<PageLuckyCharmActivityResponse>> PageLuckyCharmActivityAsync(PageLuckyCharmActivityRequest request);
	}

	///<summary>����Ϸ���</summary>///
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface ILuckyCharmClient : ILuckyCharmService, ITuhuServiceClient
    {
    	/// <summary> ���� � </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmActivityResponse")]
        OperationResult<bool> AddLuckyCharmActivity(AddLuckyCharmActivityRequest request);
		/// <summary> ��ȡ�����  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/GetLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/GetLuckyCharmActivityResponse")]
        OperationResult<LuckyCharmActivityInfoResponse> GetLuckyCharmActivity(int pkid);
		/// <summary> �û������  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AddLuckyCharmUserResponse")]
        OperationResult<bool> AddLuckyCharmUser(AddLuckyCharmUserRequest request);
		/// <summary> �޸��û�������Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/UpdateLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/UpdateLuckyCharmUserResponse")]
        OperationResult<bool> UpdateLuckyCharmUser(UpdateLuckyCharmUserRequest request);
		/// <summary> ��ҳ��ȡ�û�������Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmUserResponse")]
        OperationResult<PageLuckyCharmUserResponse> PageLuckyCharmUser(PageLuckyCharmUserRequest request);
		/// <summary> ��˱����û�  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AuditLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/AuditLuckyCharmUserResponse")]
        OperationResult<bool> AuditLuckyCharmUser(int pkid);
		/// <summary> ɾ�������û�  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmUserResponse")]
        OperationResult<bool> DelLuckyCharmUser(int pkid);
		/// <summary> ɾ���  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/DelLuckyCharmActivityResponse")]
        OperationResult<bool> DelLuckyCharmActivity(int pkid);
		/// <summary> ��ҳ��ȡ���Ϣ  </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/LuckyCharm/PageLuckyCharmActivityResponse")]
        OperationResult<PageLuckyCharmActivityResponse> PageLuckyCharmActivity(PageLuckyCharmActivityRequest request);
	}

	///<summary>����Ϸ���</summary>///
	public partial class LuckyCharmClient : TuhuServiceClient<ILuckyCharmClient>, ILuckyCharmClient
    {
    	/// <summary> ���� � </summary>
        public OperationResult<bool> AddLuckyCharmActivity(AddLuckyCharmActivityRequest request) => Invoke(_ => _.AddLuckyCharmActivity(request));

	/// <summary> ���� � </summary>
        public Task<OperationResult<bool>> AddLuckyCharmActivityAsync(AddLuckyCharmActivityRequest request) => InvokeAsync(_ => _.AddLuckyCharmActivityAsync(request));
		/// <summary> ��ȡ�����  </summary>
        public OperationResult<LuckyCharmActivityInfoResponse> GetLuckyCharmActivity(int pkid) => Invoke(_ => _.GetLuckyCharmActivity(pkid));

	/// <summary> ��ȡ�����  </summary>
        public Task<OperationResult<LuckyCharmActivityInfoResponse>> GetLuckyCharmActivityAsync(int pkid) => InvokeAsync(_ => _.GetLuckyCharmActivityAsync(pkid));
		/// <summary> �û������  </summary>
        public OperationResult<bool> AddLuckyCharmUser(AddLuckyCharmUserRequest request) => Invoke(_ => _.AddLuckyCharmUser(request));

	/// <summary> �û������  </summary>
        public Task<OperationResult<bool>> AddLuckyCharmUserAsync(AddLuckyCharmUserRequest request) => InvokeAsync(_ => _.AddLuckyCharmUserAsync(request));
		/// <summary> �޸��û�������Ϣ  </summary>
        public OperationResult<bool> UpdateLuckyCharmUser(UpdateLuckyCharmUserRequest request) => Invoke(_ => _.UpdateLuckyCharmUser(request));

	/// <summary> �޸��û�������Ϣ  </summary>
        public Task<OperationResult<bool>> UpdateLuckyCharmUserAsync(UpdateLuckyCharmUserRequest request) => InvokeAsync(_ => _.UpdateLuckyCharmUserAsync(request));
		/// <summary> ��ҳ��ȡ�û�������Ϣ  </summary>
        public OperationResult<PageLuckyCharmUserResponse> PageLuckyCharmUser(PageLuckyCharmUserRequest request) => Invoke(_ => _.PageLuckyCharmUser(request));

	/// <summary> ��ҳ��ȡ�û�������Ϣ  </summary>
        public Task<OperationResult<PageLuckyCharmUserResponse>> PageLuckyCharmUserAsync(PageLuckyCharmUserRequest request) => InvokeAsync(_ => _.PageLuckyCharmUserAsync(request));
		/// <summary> ��˱����û�  </summary>
        public OperationResult<bool> AuditLuckyCharmUser(int pkid) => Invoke(_ => _.AuditLuckyCharmUser(pkid));

	/// <summary> ��˱����û�  </summary>
        public Task<OperationResult<bool>> AuditLuckyCharmUserAsync(int pkid) => InvokeAsync(_ => _.AuditLuckyCharmUserAsync(pkid));
		/// <summary> ɾ�������û�  </summary>
        public OperationResult<bool> DelLuckyCharmUser(int pkid) => Invoke(_ => _.DelLuckyCharmUser(pkid));

	/// <summary> ɾ�������û�  </summary>
        public Task<OperationResult<bool>> DelLuckyCharmUserAsync(int pkid) => InvokeAsync(_ => _.DelLuckyCharmUserAsync(pkid));
		/// <summary> ɾ���  </summary>
        public OperationResult<bool> DelLuckyCharmActivity(int pkid) => Invoke(_ => _.DelLuckyCharmActivity(pkid));

	/// <summary> ɾ���  </summary>
        public Task<OperationResult<bool>> DelLuckyCharmActivityAsync(int pkid) => InvokeAsync(_ => _.DelLuckyCharmActivityAsync(pkid));
		/// <summary> ��ҳ��ȡ���Ϣ  </summary>
        public OperationResult<PageLuckyCharmActivityResponse> PageLuckyCharmActivity(PageLuckyCharmActivityRequest request) => Invoke(_ => _.PageLuckyCharmActivity(request));

	/// <summary> ��ҳ��ȡ���Ϣ  </summary>
        public Task<OperationResult<PageLuckyCharmActivityResponse>> PageLuckyCharmActivityAsync(PageLuckyCharmActivityRequest request) => InvokeAsync(_ => _.PageLuckyCharmActivityAsync(request));
	}
}
