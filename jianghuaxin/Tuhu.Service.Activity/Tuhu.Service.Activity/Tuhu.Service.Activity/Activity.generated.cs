
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
		/// <summary>���ݻID��ѯ�����</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIDResponse")]
        Task<OperationResult<FlashSaleModel>> SelectFlashSaleDataByActivityIDAsync(Guid activityID);
		/// <summary>��ȡ��ʱ�������������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleListResponse")]
        Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleListAsync(Guid[] activityIDs);
		/// <summary>�����������ɱ��������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayDataResponse")]
        Task<OperationResult<IEnumerable<FlashSaleModel>>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate=null,bool needProducts=true);
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
	}

	/// <summary>��ʱ����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IFlashSaleClient : IFlashSaleService, ITuhuServiceClient
    {
    	/// <summary>������ʱ�������ݵ�����</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/UpdateFlashSaleDataToCouchBaseByActivityIDResponse")]
        OperationResult<bool> UpdateFlashSaleDataToCouchBaseByActivityID(Guid activityID);
		/// <summary>���ݻID��ѯ�����</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityID", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectFlashSaleDataByActivityIDResponse")]
        OperationResult<FlashSaleModel> SelectFlashSaleDataByActivityID(Guid activityID);
		/// <summary>��ȡ��ʱ�������������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleList", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/GetFlashSaleListResponse")]
        OperationResult<List<FlashSaleModel>> GetFlashSaleList(Guid[] activityIDs);
		/// <summary>�����������ɱ��������</summary>
        /// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayData", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/FlashSale/SelectSecondKillTodayDataResponse")]
        OperationResult<IEnumerable<FlashSaleModel>> SelectSecondKillTodayData(int activityType, DateTime? scheduleDate=null,bool needProducts=true);
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
		/// <summary>���ݻID��ѯ�����</summary>
        /// <returns></returns>
        public OperationResult<FlashSaleModel> SelectFlashSaleDataByActivityID(Guid activityID) => Invoke(_ => _.SelectFlashSaleDataByActivityID(activityID));

	/// <summary>���ݻID��ѯ�����</summary>
        /// <returns></returns>
        public Task<OperationResult<FlashSaleModel>> SelectFlashSaleDataByActivityIDAsync(Guid activityID) => InvokeAsync(_ => _.SelectFlashSaleDataByActivityIDAsync(activityID));
		/// <summary>��ȡ��ʱ�������������</summary>
        /// <returns></returns>
        public OperationResult<List<FlashSaleModel>> GetFlashSaleList(Guid[] activityIDs) => Invoke(_ => _.GetFlashSaleList(activityIDs));

	/// <summary>��ȡ��ʱ�������������</summary>
        /// <returns></returns>
        public Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleListAsync(Guid[] activityIDs) => InvokeAsync(_ => _.GetFlashSaleListAsync(activityIDs));
		/// <summary>�����������ɱ��������</summary>
        /// <returns></returns>
        public OperationResult<IEnumerable<FlashSaleModel>> SelectSecondKillTodayData(int activityType, DateTime? scheduleDate=null,bool needProducts=true) => Invoke(_ => _.SelectSecondKillTodayData(activityType,scheduleDate,needProducts));

	/// <summary>�����������ɱ��������</summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<FlashSaleModel>>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate=null,bool needProducts=true) => InvokeAsync(_ => _.SelectSecondKillTodayDataAsync(activityType,scheduleDate,needProducts));
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
		///<summary></summary>///<returns></returns>
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
		///<summary>�ҳ����ʱ����</summary>
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
		/// <summary>/// ����������Ϣ/// </summary>/// <param name='infomodel'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddEnrollInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddEnrollInfoResponse")]
        Task<OperationResult<bool>> AddEnrollInfoAsync(TEnrollInfoModel infomodel);
		/// <summary>/// �޸ı�����Ϣ/// </summary>/// <param name='infomodel'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateEnrollInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateEnrollInfoResponse")]
        Task<OperationResult<bool>> UpdateEnrollInfoAsync(TEnrollInfoModel infomodel);
		///<summary>/// �Ϻ����������û��ı���״̬Ϊ��ͨ��/// </summary>/// <param name='area'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateUserEnrollStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateUserEnrollStatusResponse")]
        Task<OperationResult<bool>> UpdateUserEnrollStatusAsync(string area);
		/// <summary>/// ��������������ѯ�û���ҳ��Ϣ/// </summary>/// <param name='area'></param>/// <param name='pageIndex'></param>/// <param name='pageSize'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectEnrollInfoByArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectEnrollInfoByAreaResponse")]
        Task<OperationResult<PagedModel<TEnrollInfoModel>>> SelectEnrollInfoByAreaAsync(string area,int pageIndex, int pageSize);
		/// <summary>/// ����id��ȡmodel/// </summary>/// <param name='id'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetEnrollInfoModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetEnrollInfoModelResponse")]
        Task<OperationResult<TEnrollInfoModel>> GetEnrollInfoModelAsync(int id);
		/// <summary>/// �����ֻ��Ż�ȡmodel/// </summary>/// <param name='tel'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetEnrollInfoModelByTel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetEnrollInfoModelByTelResponse")]
        Task<OperationResult<TEnrollInfoModel>> GetEnrollInfoModelByTelAsync(string tel);
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
		///<summary></summary>///<returns></returns>
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
		///<summary>�ҳ����ʱ����</summary>
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
		/// <summary>/// ����������Ϣ/// </summary>/// <param name='infomodel'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddEnrollInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddEnrollInfoResponse")]
        OperationResult<bool> AddEnrollInfo(TEnrollInfoModel infomodel);
		/// <summary>/// �޸ı�����Ϣ/// </summary>/// <param name='infomodel'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateEnrollInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateEnrollInfoResponse")]
        OperationResult<bool> UpdateEnrollInfo(TEnrollInfoModel infomodel);
		///<summary>/// �Ϻ����������û��ı���״̬Ϊ��ͨ��/// </summary>/// <param name='area'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateUserEnrollStatus", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateUserEnrollStatusResponse")]
        OperationResult<bool> UpdateUserEnrollStatus(string area);
		/// <summary>/// ��������������ѯ�û���ҳ��Ϣ/// </summary>/// <param name='area'></param>/// <param name='pageIndex'></param>/// <param name='pageSize'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectEnrollInfoByArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectEnrollInfoByAreaResponse")]
        OperationResult<PagedModel<TEnrollInfoModel>> SelectEnrollInfoByArea(string area,int pageIndex, int pageSize);
		/// <summary>/// ����id��ȡmodel/// </summary>/// <param name='id'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetEnrollInfoModel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetEnrollInfoModelResponse")]
        OperationResult<TEnrollInfoModel> GetEnrollInfoModel(int id);
		/// <summary>/// �����ֻ��Ż�ȡmodel/// </summary>/// <param name='tel'></param>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetEnrollInfoModelByTel", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetEnrollInfoModelByTelResponse")]
        OperationResult<TEnrollInfoModel> GetEnrollInfoModelByTel(string tel);
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
		///<summary></summary>///<returns></returns>
        public OperationResult<CouponActivityConfigModel> SelectCouponActivityConfig(string activityNum, int type) => Invoke(_ => _.SelectCouponActivityConfig(activityNum,type));

	///<summary></summary>///<returns></returns>
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
		///<summary>�ҳ����ʱ����</summary>
        public OperationResult<List<ActivityPageInfoRowActivityText>> GetActivityPageInfoRowActivityTexts(ActivityPageInfoModuleBaseRequest request) => Invoke(_ => _.GetActivityPageInfoRowActivityTexts( request));

	///<summary>�ҳ����ʱ����</summary>
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
		/// <summary>/// ����������Ϣ/// </summary>/// <param name='infomodel'></param>/// <returns></returns>
        public OperationResult<bool> AddEnrollInfo(TEnrollInfoModel infomodel) => Invoke(_ => _.AddEnrollInfo(infomodel));

	/// <summary>/// ����������Ϣ/// </summary>/// <param name='infomodel'></param>/// <returns></returns>
        public Task<OperationResult<bool>> AddEnrollInfoAsync(TEnrollInfoModel infomodel) => InvokeAsync(_ => _.AddEnrollInfoAsync(infomodel));
		/// <summary>/// �޸ı�����Ϣ/// </summary>/// <param name='infomodel'></param>/// <returns></returns>
        public OperationResult<bool> UpdateEnrollInfo(TEnrollInfoModel infomodel) => Invoke(_ => _.UpdateEnrollInfo(infomodel));

	/// <summary>/// �޸ı�����Ϣ/// </summary>/// <param name='infomodel'></param>/// <returns></returns>
        public Task<OperationResult<bool>> UpdateEnrollInfoAsync(TEnrollInfoModel infomodel) => InvokeAsync(_ => _.UpdateEnrollInfoAsync(infomodel));
		///<summary>/// �Ϻ����������û��ı���״̬Ϊ��ͨ��/// </summary>/// <param name='area'></param>/// <returns></returns>
        public OperationResult<bool> UpdateUserEnrollStatus(string area) => Invoke(_ => _.UpdateUserEnrollStatus(area));

	///<summary>/// �Ϻ����������û��ı���״̬Ϊ��ͨ��/// </summary>/// <param name='area'></param>/// <returns></returns>
        public Task<OperationResult<bool>> UpdateUserEnrollStatusAsync(string area) => InvokeAsync(_ => _.UpdateUserEnrollStatusAsync(area));
		/// <summary>/// ��������������ѯ�û���ҳ��Ϣ/// </summary>/// <param name='area'></param>/// <param name='pageIndex'></param>/// <param name='pageSize'></param>/// <returns></returns>
        public OperationResult<PagedModel<TEnrollInfoModel>> SelectEnrollInfoByArea(string area,int pageIndex, int pageSize) => Invoke(_ => _.SelectEnrollInfoByArea(area,pageIndex,pageSize));

	/// <summary>/// ��������������ѯ�û���ҳ��Ϣ/// </summary>/// <param name='area'></param>/// <param name='pageIndex'></param>/// <param name='pageSize'></param>/// <returns></returns>
        public Task<OperationResult<PagedModel<TEnrollInfoModel>>> SelectEnrollInfoByAreaAsync(string area,int pageIndex, int pageSize) => InvokeAsync(_ => _.SelectEnrollInfoByAreaAsync(area,pageIndex,pageSize));
		/// <summary>/// ����id��ȡmodel/// </summary>/// <param name='id'></param>/// <returns></returns>
        public OperationResult<TEnrollInfoModel> GetEnrollInfoModel(int id) => Invoke(_ => _.GetEnrollInfoModel(id));

	/// <summary>/// ����id��ȡmodel/// </summary>/// <param name='id'></param>/// <returns></returns>
        public Task<OperationResult<TEnrollInfoModel>> GetEnrollInfoModelAsync(int id) => InvokeAsync(_ => _.GetEnrollInfoModelAsync(id));
		/// <summary>/// �����ֻ��Ż�ȡmodel/// </summary>/// <param name='tel'></param>/// <returns></returns>
        public OperationResult<TEnrollInfoModel> GetEnrollInfoModelByTel(string tel) => Invoke(_ => _.GetEnrollInfoModelByTel(tel));

	/// <summary>/// �����ֻ��Ż�ȡmodel/// </summary>/// <param name='tel'></param>/// <returns></returns>
        public Task<OperationResult<TEnrollInfoModel>> GetEnrollInfoModelByTelAsync(string tel) => InvokeAsync(_ => _.GetEnrollInfoModelByTelAsync(tel));
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
		/// <summary> �����˻�ȡ���۽�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfoResponse")]
        Task<OperationResult<InviteeBarginInfo>> GetInviteeBargainInfoAsync(Guid idKey,Guid userId);
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
		/// <summary> �����˻�ȡ���۽�� </summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfo", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/ShareBargain/GetInviteeBargainInfoResponse")]
        OperationResult<InviteeBarginInfo> GetInviteeBargainInfo(Guid idKey,Guid userId);
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
		/// <summary> �����˻�ȡ���۽�� </summary>
        public OperationResult<InviteeBarginInfo> GetInviteeBargainInfo(Guid idKey,Guid userId) => Invoke(_ => _.GetInviteeBargainInfo(idKey, userId));

	/// <summary> �����˻�ȡ���۽�� </summary>
        public Task<OperationResult<InviteeBarginInfo>> GetInviteeBargainInfoAsync(Guid idKey,Guid userId) => InvokeAsync(_ => _.GetInviteeBargainInfoAsync(idKey, userId));
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
}
