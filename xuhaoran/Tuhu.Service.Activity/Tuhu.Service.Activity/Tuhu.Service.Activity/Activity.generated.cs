
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Data;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
//不要在Activity.generated.cs文件里加任何代码，此文件内容为自动生成。需要加接口请在Activity.tt或Activity.cs中添加
namespace Tuhu.Service.Activity
{
	/// <summary>活动服务</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IActivityService
    {
    	/// <summary>查询轮胎活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityResponse")]
        Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize);
		/// <summary>查询所有可用活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityResponse")]
        Task<OperationResult<List<T_Activity_xhrModel>>> GetAllActivityAsync(int pageIndex, int pageSize);
		///<summary>新增活动</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivityResponse")]
        Task<OperationResult<bool>> AddActivityAsync(T_Activity_xhrModel request);
		///<summary>修改活动信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityResponse")]
        Task<OperationResult<bool>> UpdateActivityAsync(T_Activity_xhrModel request);
		///<summary>活动报名</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivitiesUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivitiesUserResponse")]
        Task<OperationResult<bool>> AddActivitiesUserAsync(ActivityUserInfo_xhrRequest request);
		///<summary>修改报名信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivitiesUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivitiesUserResponse")]
        Task<OperationResult<bool>> UpdateActivitiesUserAsync(ActivityUserInfo_xhrRequest request);
		/// <summary>查询所有地区</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllAreaResponse")]
        Task<OperationResult<IEnumerable<T_ArearModel>>> GetAllAreaAsync();
		/// <summary>根据地区查询用户信息</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserInfoByArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserInfoByAreaResponse")]
        Task<OperationResult<IEnumerable<T_ActivityUserInfo_xhrModels>>> GetActivityUserInfoByAreaAsync(int areaId,int pageIndex, int pageSize);
		/// <summary>自动审核</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ReviewActivityTask", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ReviewActivityTaskResponse")]
        Task<OperationResult<bool>> ReviewActivityTaskAsync();
		///<summary>管理员登录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerLogin", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerLoginResponse")]
        Task<OperationResult<string>> ManagerLoginAsync(T_ActivityManagerUserInfo_xhrModel request);
		///<summary>管理员注册</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerRegister", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerRegisterResponse")]
        Task<OperationResult<bool>> ManagerRegisterAsync(T_ActivityManagerUserInfo_xhrModel request);
		///<summary>验证登录状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckLogin", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckLoginResponse")]
        Task<OperationResult<bool>> CheckLoginAsync(int managerId);
		/// <summary>查询所有活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityManager", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityManagerResponse")]
        Task<OperationResult<IEnumerable<T_Activity_xhrModel>>> GetAllActivityManagerAsync(int pageIndex, int pageSize);
	}

	/// <summary>活动服务</summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IActivityClient : IActivityService, ITuhuServiceClient
    {
    	/// <summary>查询轮胎活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/SelectTireActivityResponse")]
        OperationResult<TireActivityModel> SelectTireActivity(string vehicleId, string tireSize);
		/// <summary>查询所有可用活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityResponse")]
        OperationResult<List<T_Activity_xhrModel>> GetAllActivity(int pageIndex, int pageSize);
		///<summary>新增活动</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivityResponse")]
        OperationResult<bool> AddActivity(T_Activity_xhrModel request);
		///<summary>修改活动信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivity", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivityResponse")]
        OperationResult<bool> UpdateActivity(T_Activity_xhrModel request);
		///<summary>活动报名</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivitiesUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/AddActivitiesUserResponse")]
        OperationResult<bool> AddActivitiesUser(ActivityUserInfo_xhrRequest request);
		///<summary>修改报名信息</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivitiesUser", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/UpdateActivitiesUserResponse")]
        OperationResult<bool> UpdateActivitiesUser(ActivityUserInfo_xhrRequest request);
		/// <summary>查询所有地区</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllAreaResponse")]
        OperationResult<IEnumerable<T_ArearModel>> GetAllArea();
		/// <summary>根据地区查询用户信息</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserInfoByArea", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetActivityUserInfoByAreaResponse")]
        OperationResult<IEnumerable<T_ActivityUserInfo_xhrModels>> GetActivityUserInfoByArea(int areaId,int pageIndex, int pageSize);
		/// <summary>自动审核</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ReviewActivityTask", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ReviewActivityTaskResponse")]
        OperationResult<bool> ReviewActivityTask();
		///<summary>管理员登录</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerLogin", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerLoginResponse")]
        OperationResult<string> ManagerLogin(T_ActivityManagerUserInfo_xhrModel request);
		///<summary>管理员注册</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerRegister", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/ManagerRegisterResponse")]
        OperationResult<bool> ManagerRegister(T_ActivityManagerUserInfo_xhrModel request);
		///<summary>验证登录状态</summary>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckLogin", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/CheckLoginResponse")]
        OperationResult<bool> CheckLogin(int managerId);
		/// <summary>查询所有活动</summary>/// <returns></returns>
        [OperationContract(Action = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityManager", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Activity/Activity/GetAllActivityManagerResponse")]
        OperationResult<IEnumerable<T_Activity_xhrModel>> GetAllActivityManager(int pageIndex, int pageSize);
	}

	/// <summary>活动服务</summary>
	public partial class ActivityClient : TuhuServiceClient<IActivityClient>, IActivityClient
    {
    	/// <summary>查询轮胎活动</summary>/// <returns></returns>
        public OperationResult<TireActivityModel> SelectTireActivity(string vehicleId, string tireSize) => Invoke(_ => _.SelectTireActivity(vehicleId,tireSize));

	/// <summary>查询轮胎活动</summary>/// <returns></returns>
        public Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize) => InvokeAsync(_ => _.SelectTireActivityAsync(vehicleId,tireSize));
		/// <summary>查询所有可用活动</summary>/// <returns></returns>
        public OperationResult<List<T_Activity_xhrModel>> GetAllActivity(int pageIndex, int pageSize) => Invoke(_ => _.GetAllActivity(pageIndex,pageSize));

	/// <summary>查询所有可用活动</summary>/// <returns></returns>
        public Task<OperationResult<List<T_Activity_xhrModel>>> GetAllActivityAsync(int pageIndex, int pageSize) => InvokeAsync(_ => _.GetAllActivityAsync(pageIndex,pageSize));
		///<summary>新增活动</summary>
        public OperationResult<bool> AddActivity(T_Activity_xhrModel request) => Invoke(_ => _.AddActivity(request));

	///<summary>新增活动</summary>
        public Task<OperationResult<bool>> AddActivityAsync(T_Activity_xhrModel request) => InvokeAsync(_ => _.AddActivityAsync(request));
		///<summary>修改活动信息</summary>
        public OperationResult<bool> UpdateActivity(T_Activity_xhrModel request) => Invoke(_ => _.UpdateActivity(request));

	///<summary>修改活动信息</summary>
        public Task<OperationResult<bool>> UpdateActivityAsync(T_Activity_xhrModel request) => InvokeAsync(_ => _.UpdateActivityAsync(request));
		///<summary>活动报名</summary>
        public OperationResult<bool> AddActivitiesUser(ActivityUserInfo_xhrRequest request) => Invoke(_ => _.AddActivitiesUser(request));

	///<summary>活动报名</summary>
        public Task<OperationResult<bool>> AddActivitiesUserAsync(ActivityUserInfo_xhrRequest request) => InvokeAsync(_ => _.AddActivitiesUserAsync(request));
		///<summary>修改报名信息</summary>
        public OperationResult<bool> UpdateActivitiesUser(ActivityUserInfo_xhrRequest request) => Invoke(_ => _.UpdateActivitiesUser(request));

	///<summary>修改报名信息</summary>
        public Task<OperationResult<bool>> UpdateActivitiesUserAsync(ActivityUserInfo_xhrRequest request) => InvokeAsync(_ => _.UpdateActivitiesUserAsync(request));
		/// <summary>查询所有地区</summary>/// <returns></returns>
        public OperationResult<IEnumerable<T_ArearModel>> GetAllArea() => Invoke(_ => _.GetAllArea());

	/// <summary>查询所有地区</summary>/// <returns></returns>
        public Task<OperationResult<IEnumerable<T_ArearModel>>> GetAllAreaAsync() => InvokeAsync(_ => _.GetAllAreaAsync());
		/// <summary>根据地区查询用户信息</summary>/// <returns></returns>
        public OperationResult<IEnumerable<T_ActivityUserInfo_xhrModels>> GetActivityUserInfoByArea(int areaId,int pageIndex, int pageSize) => Invoke(_ => _.GetActivityUserInfoByArea(areaId,pageIndex,pageSize));

	/// <summary>根据地区查询用户信息</summary>/// <returns></returns>
        public Task<OperationResult<IEnumerable<T_ActivityUserInfo_xhrModels>>> GetActivityUserInfoByAreaAsync(int areaId,int pageIndex, int pageSize) => InvokeAsync(_ => _.GetActivityUserInfoByAreaAsync(areaId,pageIndex,pageSize));
		/// <summary>自动审核</summary>/// <returns></returns>
        public OperationResult<bool> ReviewActivityTask() => Invoke(_ => _.ReviewActivityTask());

	/// <summary>自动审核</summary>/// <returns></returns>
        public Task<OperationResult<bool>> ReviewActivityTaskAsync() => InvokeAsync(_ => _.ReviewActivityTaskAsync());
		///<summary>管理员登录</summary>
        public OperationResult<string> ManagerLogin(T_ActivityManagerUserInfo_xhrModel request) => Invoke(_ => _.ManagerLogin(request));

	///<summary>管理员登录</summary>
        public Task<OperationResult<string>> ManagerLoginAsync(T_ActivityManagerUserInfo_xhrModel request) => InvokeAsync(_ => _.ManagerLoginAsync(request));
		///<summary>管理员注册</summary>
        public OperationResult<bool> ManagerRegister(T_ActivityManagerUserInfo_xhrModel request) => Invoke(_ => _.ManagerRegister(request));

	///<summary>管理员注册</summary>
        public Task<OperationResult<bool>> ManagerRegisterAsync(T_ActivityManagerUserInfo_xhrModel request) => InvokeAsync(_ => _.ManagerRegisterAsync(request));
		///<summary>验证登录状态</summary>
        public OperationResult<bool> CheckLogin(int managerId) => Invoke(_ => _.CheckLogin(managerId));

	///<summary>验证登录状态</summary>
        public Task<OperationResult<bool>> CheckLoginAsync(int managerId) => InvokeAsync(_ => _.CheckLoginAsync(managerId));
		/// <summary>查询所有活动</summary>/// <returns></returns>
        public OperationResult<IEnumerable<T_Activity_xhrModel>> GetAllActivityManager(int pageIndex, int pageSize) => Invoke(_ => _.GetAllActivityManager(pageIndex,pageSize));

	/// <summary>查询所有活动</summary>/// <returns></returns>
        public Task<OperationResult<IEnumerable<T_Activity_xhrModel>>> GetAllActivityManagerAsync(int pageIndex, int pageSize) => InvokeAsync(_ => _.GetAllActivityManagerAsync(pageIndex,pageSize));
	}
}
