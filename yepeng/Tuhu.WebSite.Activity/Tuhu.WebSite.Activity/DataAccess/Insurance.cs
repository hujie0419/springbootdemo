using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;
using Tuhu.WebSite.Component.SystemFramework.Log;

/// <summary>
/// 车险报名
/// </summary>
namespace Tuhu.WebSite.Web.Activity.DataAccess
{
    /// <summary>
    /// 报名状态
    /// </summary>
    public enum EntryStatus
    {
        /// <summary>
        /// 报名失败
        /// </summary>
        EntryError = -1,
        /// <summary>
        /// 手机号为空
        /// </summary>
        NoLogin = 0,
        /// <summary>
        /// 手机号格式错误
        /// </summary>
        FormatError = 1,
        /// <summary>
        /// 验证码错误
        /// </summary>
        CodeError = 2,
        /// <summary>
        /// 已报名
        /// </summary>
        Repeatphone=3,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,
        /// <summary>
        /// 注册并报名成功
        /// </summary>
        RegisterASuccess = 201
    }
    public class Insurance
    {
        private readonly static string connstr = ConfigurationManager.ConnectionStrings["Insurance_db"].ToString();
        /// <summary>
        /// 报名参加
        /// </summary>
        /// <returns></returns>
        public static EntryStatus Entry(string phone)
        {
            try
            {
                //如果手机号已注册
                if (GetUserByPhone(phone) != null)
                {
                    return Insert(phone);
                }
                else
                {
                    //如果手机号注册成功并且报名成功
                    if (CreateUser(phone) != null && Insert(phone) == EntryStatus.Success)
                    {
                        return EntryStatus.RegisterASuccess;
                    }
                    else
                    {
                        return EntryStatus.EntryError;
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return EntryStatus.EntryError;
            }
        }
        /// <summary>
        /// 插入記錄
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static EntryStatus Insert(string phone)
        {
            using (var db = DbHelper.CreateDbHelper(connstr))
            {
                var cmd = new SqlCommand(@"if not exists (select phone from Tuhu_huodong..tbl_Insurance WITH(NOLOCK) WHERE phone=@wphone)
                                                INSERT INTO Tuhu_huodong..tbl_Insurance(phone,CreateDateTime) VALUES(@phone,@CreateDateTime)");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@wphone", phone);
                cmd.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                return db.ExecuteNonQuery(cmd) > 0 ? EntryStatus.Success : EntryStatus.Repeatphone;
            }
        }
        /// <summary>
        /// 获取用户信息by phone
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static User GetUserByPhone(string Phone)
        {
            try
            {
                using (var client = new UserAccountClient())
                {
                    var clientResult = client.GetUserByMobile(Phone);
                    if (clientResult.Success && clientResult.Result != null)
                    {
                        Log(UserActionEnum.Login, clientResult.Result.UserId, "chexian_获取用户信息,GetUserByPhone:成功");
                        return clientResult.Result;
                    }
                    else
                    {
                        Log(UserActionEnum.Login, null, "chexian_获取用户信息,GetUserByMobileAsync:失败");
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return null;
            }
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        public static User CreateUser(string MibleNumber)
        {
            //防止数据库同步时差 导致重复创建账号 Sleep 2s
            System.Threading.Thread.Sleep(2000);
            using (var client = new UserAccountClient())
            {
                var password = new Random().Next(100000, 1000000).ToString();
                CreateUserRequest request = new CreateUserRequest
                {
                    MobileNumber = MibleNumber,
                    ChannelIn = nameof(ChannelIn.H5),
                    Password = password,
                    Profile = new UserProfile(),
                    UserCategoryIn = nameof(UserCategoryIn.Tuhu)
                };
                var Result = client.CreateUserRequest(request);

                Result.ThrowIfException(true);
                if (Result.Success && Result.Result != null)
                {
                    Log(UserActionEnum.Register, Result.Result.UserId, "chexian_创建用户,CreateUser:成功");
                    return Result.Result;
                }
                else
                {
                    Log(UserActionEnum.Register, null, "chexian_创建用户,CreateUser:失败");
                    return null;
                }
            }
        }
        /// <summary>
        /// 记录用户操作日志 登录、注册、登出、更新等操作
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="UserId"></param>
        /// <param name="Content"></param>
        public static void Log(UserActionEnum Action, Guid? UserId, string Content)
        {
            using (var client = new UserAccountClient())
            {
                client.LogUserActionAsync(new UserLog()
                {
                    Action = Action,
                    UserId = UserId.HasValue ? UserId.Value : Guid.Empty,
                    Content = Content,
                    Channel = ChannelEnum.H5,
                    ChannelIn = nameof(ChannelIn.H5),
                    Ip = HttpContext.Current.Request.UserIp()
                });
            }
        }
    }
}