
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Data;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
//��Ҫ��Activity.generated.cs�ļ�����κδ��룬���ļ�����Ϊ�Զ����ɡ���Ҫ�ӽӿ�����Activity.tt��Activity.cs�����
namespace Tuhu.Service.Activity
{
	/// <summary>�����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IActivityService
    {
    	/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityResponse")]
        Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize);
		/// <summary>��ѯ���п��û</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityResponse")]
        Task<OperationResult<List<T_Activity_xhrModel>>> GetAllActivityAsync(int pageIndex, int pageSize);
		///<summary>�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivityResponse")]
        Task<OperationResult<bool>> AddActivityAsync(T_Activity_xhrModel request);
		///<summary>�޸Ļ��Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityResponse")]
        Task<OperationResult<bool>> UpdateActivityAsync(T_Activity_xhrModel request);
		///<summary>�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivitiesUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivitiesUserResponse")]
        Task<OperationResult<bool>> AddActivitiesUserAsync(ActivityUserInfo_xhrRequest request);
		///<summary>�޸ı�����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivitiesUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivitiesUserResponse")]
        Task<OperationResult<bool>> UpdateActivitiesUserAsync(ActivityUserInfo_xhrRequest request);
		/// <summary>��ѯ���е���</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllAreaResponse")]
        Task<OperationResult<IEnumerable<T_ArearModel>>> GetAllAreaAsync();
		/// <summary>���ݵ�����ѯ�û���Ϣ</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserInfoByArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserInfoByAreaResponse")]
        Task<OperationResult<IEnumerable<T_ActivityUserInfo_xhrModels>>> GetActivityUserInfoByAreaAsync(int areaId,int pageIndex, int pageSize);
		/// <summary>�Զ����</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ReviewActivityTask", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ReviewActivityTaskResponse")]
        Task<OperationResult<bool>> ReviewActivityTaskAsync();
		///<summary>����Ա��¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerLogin", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerLoginResponse")]
        Task<OperationResult<string>> ManagerLoginAsync(T_ActivityManagerUserInfo_xhrModel request);
		///<summary>����Աע��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerRegister", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerRegisterResponse")]
        Task<OperationResult<bool>> ManagerRegisterAsync(T_ActivityManagerUserInfo_xhrModel request);
		///<summary>��֤��¼״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckLogin", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckLoginResponse")]
        Task<OperationResult<bool>> CheckLoginAsync(int managerId);
		/// <summary>��ѯ���л</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityManager", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityManagerResponse")]
        Task<OperationResult<IEnumerable<T_Activity_xhrModel>>> GetAllActivityManagerAsync(int pageIndex, int pageSize);
	}

	/// <summary>�����</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IActivityClient : IActivityService, ITuhuServiceClient
    {
    	/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityResponse")]
        OperationResult<TireActivityModel> SelectTireActivity(string vehicleId, string tireSize);
		/// <summary>��ѯ���п��û</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityResponse")]
        OperationResult<List<T_Activity_xhrModel>> GetAllActivity(int pageIndex, int pageSize);
		///<summary>�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivityResponse")]
        OperationResult<bool> AddActivity(T_Activity_xhrModel request);
		///<summary>�޸Ļ��Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityResponse")]
        OperationResult<bool> UpdateActivity(T_Activity_xhrModel request);
		///<summary>�����</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivitiesUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivitiesUserResponse")]
        OperationResult<bool> AddActivitiesUser(ActivityUserInfo_xhrRequest request);
		///<summary>�޸ı�����Ϣ</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivitiesUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivitiesUserResponse")]
        OperationResult<bool> UpdateActivitiesUser(ActivityUserInfo_xhrRequest request);
		/// <summary>��ѯ���е���</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllAreaResponse")]
        OperationResult<IEnumerable<T_ArearModel>> GetAllArea();
		/// <summary>���ݵ�����ѯ�û���Ϣ</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserInfoByArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserInfoByAreaResponse")]
        OperationResult<IEnumerable<T_ActivityUserInfo_xhrModels>> GetActivityUserInfoByArea(int areaId,int pageIndex, int pageSize);
		/// <summary>�Զ����</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ReviewActivityTask", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ReviewActivityTaskResponse")]
        OperationResult<bool> ReviewActivityTask();
		///<summary>����Ա��¼</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerLogin", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerLoginResponse")]
        OperationResult<string> ManagerLogin(T_ActivityManagerUserInfo_xhrModel request);
		///<summary>����Աע��</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerRegister", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerRegisterResponse")]
        OperationResult<bool> ManagerRegister(T_ActivityManagerUserInfo_xhrModel request);
		///<summary>��֤��¼״̬</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckLogin", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckLoginResponse")]
        OperationResult<bool> CheckLogin(int managerId);
		/// <summary>��ѯ���л</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityManager", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityManagerResponse")]
        OperationResult<IEnumerable<T_Activity_xhrModel>> GetAllActivityManager(int pageIndex, int pageSize);
	}

	/// <summary>�����</summary>
	public partial class ActivityClient : TuhuServiceClient<IActivityClient>, IActivityClient
    {
    	/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        public OperationResult<TireActivityModel> SelectTireActivity(string vehicleId, string tireSize) => Invoke(_ => _.SelectTireActivity(vehicleId,tireSize));

	/// <summary>��ѯ��̥�</summary>/// <returns></returns>
        public Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize) => InvokeAsync(_ => _.SelectTireActivityAsync(vehicleId,tireSize));
		/// <summary>��ѯ���п��û</summary>/// <returns></returns>
        public OperationResult<List<T_Activity_xhrModel>> GetAllActivity(int pageIndex, int pageSize) => Invoke(_ => _.GetAllActivity(pageIndex,pageSize));

	/// <summary>��ѯ���п��û</summary>/// <returns></returns>
        public Task<OperationResult<List<T_Activity_xhrModel>>> GetAllActivityAsync(int pageIndex, int pageSize) => InvokeAsync(_ => _.GetAllActivityAsync(pageIndex,pageSize));
		///<summary>�����</summary>
        public OperationResult<bool> AddActivity(T_Activity_xhrModel request) => Invoke(_ => _.AddActivity(request));

	///<summary>�����</summary>
        public Task<OperationResult<bool>> AddActivityAsync(T_Activity_xhrModel request) => InvokeAsync(_ => _.AddActivityAsync(request));
		///<summary>�޸Ļ��Ϣ</summary>
        public OperationResult<bool> UpdateActivity(T_Activity_xhrModel request) => Invoke(_ => _.UpdateActivity(request));

	///<summary>�޸Ļ��Ϣ</summary>
        public Task<OperationResult<bool>> UpdateActivityAsync(T_Activity_xhrModel request) => InvokeAsync(_ => _.UpdateActivityAsync(request));
		///<summary>�����</summary>
        public OperationResult<bool> AddActivitiesUser(ActivityUserInfo_xhrRequest request) => Invoke(_ => _.AddActivitiesUser(request));

	///<summary>�����</summary>
        public Task<OperationResult<bool>> AddActivitiesUserAsync(ActivityUserInfo_xhrRequest request) => InvokeAsync(_ => _.AddActivitiesUserAsync(request));
		///<summary>�޸ı�����Ϣ</summary>
        public OperationResult<bool> UpdateActivitiesUser(ActivityUserInfo_xhrRequest request) => Invoke(_ => _.UpdateActivitiesUser(request));

	///<summary>�޸ı�����Ϣ</summary>
        public Task<OperationResult<bool>> UpdateActivitiesUserAsync(ActivityUserInfo_xhrRequest request) => InvokeAsync(_ => _.UpdateActivitiesUserAsync(request));
		/// <summary>��ѯ���е���</summary>/// <returns></returns>
        public OperationResult<IEnumerable<T_ArearModel>> GetAllArea() => Invoke(_ => _.GetAllArea());

	/// <summary>��ѯ���е���</summary>/// <returns></returns>
        public Task<OperationResult<IEnumerable<T_ArearModel>>> GetAllAreaAsync() => InvokeAsync(_ => _.GetAllAreaAsync());
		/// <summary>���ݵ�����ѯ�û���Ϣ</summary>/// <returns></returns>
        public OperationResult<IEnumerable<T_ActivityUserInfo_xhrModels>> GetActivityUserInfoByArea(int areaId,int pageIndex, int pageSize) => Invoke(_ => _.GetActivityUserInfoByArea(areaId,pageIndex,pageSize));

	/// <summary>���ݵ�����ѯ�û���Ϣ</summary>/// <returns></returns>
        public Task<OperationResult<IEnumerable<T_ActivityUserInfo_xhrModels>>> GetActivityUserInfoByAreaAsync(int areaId,int pageIndex, int pageSize) => InvokeAsync(_ => _.GetActivityUserInfoByAreaAsync(areaId,pageIndex,pageSize));
		/// <summary>�Զ����</summary>/// <returns></returns>
        public OperationResult<bool> ReviewActivityTask() => Invoke(_ => _.ReviewActivityTask());

	/// <summary>�Զ����</summary>/// <returns></returns>
        public Task<OperationResult<bool>> ReviewActivityTaskAsync() => InvokeAsync(_ => _.ReviewActivityTaskAsync());
		///<summary>����Ա��¼</summary>
        public OperationResult<string> ManagerLogin(T_ActivityManagerUserInfo_xhrModel request) => Invoke(_ => _.ManagerLogin(request));

	///<summary>����Ա��¼</summary>
        public Task<OperationResult<string>> ManagerLoginAsync(T_ActivityManagerUserInfo_xhrModel request) => InvokeAsync(_ => _.ManagerLoginAsync(request));
		///<summary>����Աע��</summary>
        public OperationResult<bool> ManagerRegister(T_ActivityManagerUserInfo_xhrModel request) => Invoke(_ => _.ManagerRegister(request));

	///<summary>����Աע��</summary>
        public Task<OperationResult<bool>> ManagerRegisterAsync(T_ActivityManagerUserInfo_xhrModel request) => InvokeAsync(_ => _.ManagerRegisterAsync(request));
		///<summary>��֤��¼״̬</summary>
        public OperationResult<bool> CheckLogin(int managerId) => Invoke(_ => _.CheckLogin(managerId));

	///<summary>��֤��¼״̬</summary>
        public Task<OperationResult<bool>> CheckLoginAsync(int managerId) => InvokeAsync(_ => _.CheckLoginAsync(managerId));
		/// <summary>��ѯ���л</summary>/// <returns></returns>
        public OperationResult<IEnumerable<T_Activity_xhrModel>> GetAllActivityManager(int pageIndex, int pageSize) => Invoke(_ => _.GetAllActivityManager(pageIndex,pageSize));

	/// <summary>��ѯ���л</summary>/// <returns></returns>
        public Task<OperationResult<IEnumerable<T_Activity_xhrModel>>> GetAllActivityManagerAsync(int pageIndex, int pageSize) => InvokeAsync(_ => _.GetAllActivityManagerAsync(pageIndex,pageSize));
	}
}
