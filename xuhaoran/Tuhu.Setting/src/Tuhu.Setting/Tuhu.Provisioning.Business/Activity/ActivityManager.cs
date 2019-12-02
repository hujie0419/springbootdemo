
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Activity;


namespace Tuhu.Provisioning.Business.Activity
{
    public class ActivityManager
    {
        #region Private Fields  
        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly IDBScopeManager DbGungnirManager = null;
        private readonly IDBScopeManager DbGungnirReadOnlyManager = null;

        private readonly ActivityHandler handler;
        private readonly ActivityHandler handlerReadonly;
        public static readonly Common.Logging.ILog Logger = Common.Logging.LogManager.GetLogger(typeof(ActivityManager));

        #endregion

        public ActivityManager()
        {
            string gungnirConnStr = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string gungnirReadOnlyConnStr = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(gungnirConnStr))
            {
                gungnirConnStr = SecurityHelp.DecryptAES(gungnirConnStr);
            }
            if (SecurityHelp.IsBase64Formatted(gungnirReadOnlyConnStr))
            {
                gungnirReadOnlyConnStr = SecurityHelp.DecryptAES(gungnirReadOnlyConnStr);
            }

            DbGungnirManager = new DBScopeManager(new ConnectionManager(gungnirConnStr));
            DbGungnirReadOnlyManager = new DBScopeManager(new ConnectionManager(gungnirReadOnlyConnStr));
            handler = new ActivityHandler();
            handlerReadonly = new ActivityHandler();
        }

        /// <summary>
        /// 根据地区查询活动
        /// </summary>
        /// <returns></returns>
        public Reseponse<List<T_ActivityUserInfo_xhrModel>> GetActivityUserInfoByAreaAsync(int AreaId, int pageIndex, int pageSize)
        {
            var result = new Reseponse<List<T_ActivityUserInfo_xhrModel>>();
            if (pageSize > 20)
            {
                result.status = 0;
                result.Message = "请求数据过多！";
                return result;
            }
            try
            {
                using (var client = new ActivityClient())
                {
                    var model = client.GetActivityUserInfoByAreaAsync(AreaId,pageIndex,pageSize).Result;
                    result.status = model.Success ? 1 : 0;
                    result.Message = model.ErrorMessage;
                    result.data = new List<T_ActivityUserInfo_xhrModel>();
                    if(model.Success && model.Result != null)
                    {                   
                        foreach (var item in model.Result)
                        {
                            result.data.Add(new T_ActivityUserInfo_xhrModel()
                            {
                                UserID = TuhuCryption.Encrypt(item.UserId.ToString()),
                                UserName = item.UserName,
                                UserTell = item.UserTell.Trim(),
                                PassStatus = item.PassStatus,
                                AreaID=item.AreaID,
                                ActID = item.ActID,
                                Title = item.ActTitle

                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetActivityUserInfoByAreaAsync -> {AreaId}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取所有可用活动
        /// </summary>
        /// <returns></returns>
        public Reseponse<List<T_Activity_xhrModel>> GetAllActivityAsync(int pageIndex, int pageSize)
        {
            var result = new Reseponse<List<T_Activity_xhrModel>>();
            if (pageSize > 20)
            {
                result.status = 0;
                result.Message = "请求数据过多！";
                return result;
            }
            try
            {
                using (var client = new ActivityClient())
                {
                    var model = client.GetAllActivityAsync(pageIndex, pageSize).Result;
                    result.status = model.Success ? 1 : 0;
                    result.Message = model.ErrorMessage;
                    result.data = new List<T_Activity_xhrModel>();
                    if (model.Success && model.Result !=null)
                    {
                        foreach (var item in model.Result)
                        {
                            result.data.Add(new T_Activity_xhrModel()
                            {
                                ActivityID = item.ActivityId,
                                Title = item.Title,
                                ActivityContent = item.ActivityContent,
                                StartTime = item.StartTime.ToString("yyyy-MM-dd"),
                                EndTime = item.EndTime.ToString("yyyy-MM-dd"),
                                Picture=item.Picture,
                                Remark = item.Remark,
                                ActStatus=item.ActStatus
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetAllActivityAsync", e);
                throw;
            }
            return result;
        }


        /// <summary>
        /// 获取所有活动
        /// </summary>
        /// <returns></returns>
        public Reseponse<List<T_Activity_xhrModel>> GetAllActivityManagerAsync(int pageIndex, int pageSize)
        {
            var result = new Reseponse<List<T_Activity_xhrModel>>();
            if (pageSize > 20)
            {
                result.status = 0;
                result.Message = "请求数据过多！";
                return result;
            }
            try
            {
                using (var client = new ActivityClient())
                {
                    var model = client.GetAllActivityManagerAsync(pageIndex, pageSize).Result;
                    result.status = model.Success ? 1 : 0;
                    result.Message = model.ErrorMessage;
                    result.data = new List<T_Activity_xhrModel>();
                    if (model.Success && model.Result != null)
                    {
                        foreach (var item in model.Result)
                        {
                            result.data.Add(new T_Activity_xhrModel()
                            {
                                ActivityID = item.ActivityId,
                                Title = item.Title,
                                ActivityContent = item.ActivityContent,
                                StartTime = item.StartTime.ToString("yyyy-MM-dd"),
                                EndTime = item.EndTime.ToString("yyyy-MM-dd"),
                                Picture = item.Picture,
                                Remark = item.Remark,
                                ActStatus = item.ActStatus
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetAllActivityManagerAsync", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 活动报名
        /// </summary>
        /// <returns></returns>
        public Reseponse<bool> AddActivitiesUserAsync(ActivityUserInfo_xhrRequestModel model)
        {
            var result = new Reseponse<bool>();
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                result.status = 0;
                result.Message = "用户名不能为空！";
                return result;
            }
            var regex = new Regex("^1[0-9]{10}$");
            if (string.IsNullOrWhiteSpace(model.UserTell) || !regex.IsMatch(model.UserTell))
            {
                result.status = 0;
                result.Message = "请输入正确的手机号！";
                return result;
            }
            if (model.AreaID <= 0)
            {
                result.status = 0;
                result.Message = "请选择地区！";
                return result;
            }
            try
            {
                using (var client = new ActivityClient())
                {
                    var request = new Tuhu.Service.Activity.Models.ActivityUserInfo_xhrRequest()
                    {
                        UserName = model.UserName,
                        UserTell = model.UserTell,
                        AreaID = model.AreaID,
                        ActID = model.ActID
                    };
                    var addresult= client.AddActivitiesUserAsync(request);
                    result.status = addresult.Result.Success ? 1 : 0;
                    result.Message = addresult.Result.ErrorMessage;
                    result.data= addresult.Result.Success;
                }
            }
                catch (Exception e)
                {
                    Logger.Error($"AddActivitiesUser -> {JsonConvert.SerializeObject(model)}", e);
                    throw;
                }
            return result;

        }

        /// <summary>
        /// 修改报名信息
        /// </summary>
        /// <returns></returns>
        public Reseponse<bool> UpdateActivitiesUserAsync(ActivityUserInfo_xhrRequestModel model, string managerId)
        {
            var result = new Reseponse<bool>();
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                result.status = 0;
                result.Message = "用户名不能为空！";
                return result;
            }
            var regex = new Regex("^1[0-9]{10}$");
            if (string.IsNullOrWhiteSpace(model.UserTell) || !regex.IsMatch(model.UserTell))
            {
                result.status = 0;
                result.Message = "请输入正确的手机号！";
                return result;
            }
            if (model.AreaID <= 0)
            {
                result.status = 0;
                result.Message = "请选择地区！";
                return result;
            }
            if (model.PassStatus < 0)
            {
                result.status = 0;
                result.Message = "请选择正确的审核状态！";
                return result;
            }
            if (model.UserStatus < 0)
            {
                result.status = 0;
                result.Message = "请选择正确的用户状态！";
                return result;
            }
            int userid = int.Parse(TuhuCryption.Decrypt(managerId));
            try
            {
                using (var client = new ActivityClient())
                {
                    var checkresult = client.CheckLoginAsync(userid);
                    if (!checkresult.Result.Success)
                    {
                        result.status = 0;
                        result.Message = "请先登录!";
                        return result;
                    }
                    var request = new Tuhu.Service.Activity.Models.ActivityUserInfo_xhrRequest()
                    {
                        UserId = int.Parse(TuhuCryption.Decrypt(model.UserId)),
                        UserName = model.UserName,
                        UserTell = model.UserTell,
                        AreaID = model.AreaID,
                        PassStatus = model.PassStatus,
                        ActID = model.ActID,
                        UserStatus = model.UserStatus
                    };
                    var addresult = client.UpdateActivitiesUserAsync(request);
                    result.status = addresult.Result.Success ? 1 : 0;
                    result.Message = addresult.Result.ErrorMessage;
                    result.data = addresult.Result.Success;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"UpdateActivitiesUser -> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return result;

        }

        /// <summary>
        /// 获取所有活动地区
        /// </summary>
        /// <returns></returns>
        public Reseponse<List<T_ArearModel>> GetAllAreaAsync()
        {
            var result = new Reseponse<List<T_ArearModel>>();
            try
            {
                using (var client = new ActivityClient())
                {
                    var model = client.GetAllAreaAsync().Result;
                    result.status = model.Success ? 1 : 0;
                    result.Message = model.ErrorMessage;
                    result.data = new List<T_ArearModel>();
                    if (model.Success && model.Result != null)
                    {
                        foreach (var item in model.Result)
                        {
                            result.data.Add(new T_ArearModel()
                            {
                                AreaId = item.AreaId,
                                ArearName = item.ArearName

                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetAllAreaAsync", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 新增活动
        /// </summary>
        /// <returns></returns>
        public Reseponse<bool> AddActivity(T_Activity_xhrModel model,string managerId)
        {
            var result = new Reseponse<bool>();
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                result.status = 0;
                result.Message = "标题不能为空！";
                return result;
            }
            if (string.IsNullOrWhiteSpace(model.ActivityContent))
            {
                result.status = 0;
                result.Message = "内容不能为空！";
                return result;
            }
            if (string.IsNullOrWhiteSpace(model.StartTime))
            {
                result.status = 0;
                result.Message = "开始时间不能为空！";
                return result;
            }
            if (string.IsNullOrWhiteSpace(model.EndTime))
            {
                result.status = 0;
                result.Message = "结束时间不能为空！";
                return result;
            }
            if (string.IsNullOrWhiteSpace(model.Picture))
            {
                result.status = 0;
                result.Message = "图片地址不能为空！";
                return result;
            }
            if (string.IsNullOrWhiteSpace(managerId))
            {
                result.status = 0;
                result.Message = "请先登录!";
                return result;
            }
            int userid = int.Parse(TuhuCryption.Decrypt(managerId));
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd";
            try
            {
                using (var client = new ActivityClient())
                {
                    var checkresult = client.CheckLoginAsync(userid);
                    if (!checkresult.Result.Success)
                    {
                        result.status =  0;
                        result.Message = "请先登录!";
                        return result;
                    }
                    var request = new Tuhu.Service.Activity.Models.T_Activity_xhrModel()
                    {
                        Title = model.Title,
                        ActivityContent = model.ActivityContent,
                        
                        StartTime = Convert.ToDateTime(model.StartTime, dtFormat),
                        EndTime =Convert.ToDateTime(model.EndTime, dtFormat),
                        Picture = model.Picture,
                        ActStatus = model.ActStatus
                    };
                    var addresult = client.AddActivityAsync(request);
                    result.status = addresult.Result.Success ? 1 : 0;
                    result.Message = addresult.Result.ErrorMessage;
                    result.data = addresult.Result.Success;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"AddActivity -> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 修改活动
        /// </summary>
        /// <returns></returns>
        public Reseponse<bool> UpdateActivity(T_Activity_xhrModel model, string managerId)
        {
            var result = new Reseponse<bool>();
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                result.status = 0;
                result.Message = "标题不能为空！";
                return result;
            }
            if (string.IsNullOrWhiteSpace(model.ActivityContent))
            {
                result.status = 0;
                result.Message = "内容不能为空！";
                return result;
            }
            if (string.IsNullOrWhiteSpace(model.StartTime))
            {
                result.status = 0;
                result.Message = "开始时间不能为空！";
                return result;
            }
            if (string.IsNullOrWhiteSpace(model.EndTime))
            {
                result.status = 0;
                result.Message = "结束时间不能为空！";
                return result;
            }
            if (string.IsNullOrWhiteSpace(model.Picture))
            {
                result.status = 0;
                result.Message = "图片地址不能为空！";
                return result;
            }
            if (model.ActStatus<0)
            {
                result.status = 0;
                result.Message = "请选择正确的活动状态！";
                return result;
            }
                if (string.IsNullOrWhiteSpace(managerId))
            {
                result.status = 0;
                result.Message = "请先登录!";
                return result;
            }
            int userid = int.Parse(TuhuCryption.Decrypt(managerId));
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd";
            try
            {
                using (var client = new ActivityClient())
                {
                    var checkresult = client.CheckLoginAsync(userid);
                    if (!checkresult.Result.Success)
                    {
                        result.status = 0;
                        result.Message = "请先登录!";
                        return result;
                    }
                    var request = new Tuhu.Service.Activity.Models.T_Activity_xhrModel()
                    {
                        Title = model.Title,
                        ActivityContent = model.ActivityContent,
                        StartTime = Convert.ToDateTime(model.StartTime, dtFormat),
                        EndTime = Convert.ToDateTime(model.EndTime, dtFormat),
                        Picture = model.Picture,
                        ActStatus = model.ActStatus,
                        ActivityId = model.ActivityID
                    };
                    var addresult = client.UpdateActivityAsync(request);
                    result.status = addresult.Result.Success ? 1 : 0;
                    result.Message = addresult.Result.ErrorMessage;
                    result.data = addresult.Result.Success;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"UpdateActivity -> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return result;
        }


        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <returns></returns>
        public Reseponse<string> ManagerLogin(T_ActivityManagerUserInfo_xhrRequest model)
        {
            var result = new Reseponse<string>();
            try
            {
                using (var client = new ActivityClient())
                {
                    var request = new Tuhu.Service.Activity.Models.T_ActivityManagerUserInfo_xhrModel()
                    {
                        Name = model.Name,
                        PassWords = model.PassWords
                    };
                    var addresult = client.ManagerLoginAsync(request);
                    result.status = addresult.Result.Success ? 1 : 0;
                    result.Message = addresult.Result.ErrorMessage;
                    if (addresult.Result.Result == "")
                    {
                        result.status = 0;
                        result.Message = "用户名或密码不正确";
                    }
                    result.data = TuhuCryption.Encrypt(addresult.Result.Result);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ManagerLogin -> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 管理员注册
        /// </summary>
        /// <returns></returns>
        public Reseponse<bool> ManagerRegister(T_ActivityManagerUserInfo_xhrRequest model)
        {
            var result = new Reseponse<bool>();
            try
            {
                using (var client = new ActivityClient())
                {
                    var request = new Tuhu.Service.Activity.Models.T_ActivityManagerUserInfo_xhrModel()
                    {
                        Name = model.Name,
                        PassWords = model.PassWords
                    };
                    var addresult = client.ManagerRegisterAsync(request);
                    result.status = addresult.Result.Success ? 1 : 0;
                    result.Message = addresult.Result.ErrorMessage;
                    result.data = addresult.Result.Success;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ManagerLogin -> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return result;
        }
    }
}
