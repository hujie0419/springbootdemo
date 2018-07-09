using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class MobileBindUnbindController : Controller
    {
        private readonly Lazy<DownloadAppManager> lazy = new Lazy<DownloadAppManager>();

        private DownloadAppManager DownloadAppManager
        {
            get { return lazy.Value; }
        }

		[PowerManage]
        public ViewResult ChangeMobileBinding()
        {
            IEnumerable<UserChangeBindMobileLog> changeMobileBindingLogs = new List<UserChangeBindMobileLog>();
            return View(changeMobileBindingLogs);
        }

        [HttpPost]
        public ViewResult ChangeMobileBindingOnMobile(string mobileToFind)
        {
            IEnumerable<UserChangeBindMobileLog> changeMobileBindingLogs = new List<UserChangeBindMobileLog>();
            if (!string.IsNullOrEmpty(mobileToFind))
            {
                try
                {
                    using (var client = new UserAccountClient())
                    {
                        var result = client.QueryChangeBindMobileLogByMobile(mobileToFind);
                        result.ThrowIfException(true);
                        if (result.Success && result.Result != null)
                            changeMobileBindingLogs = result.Result;
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }
            }
            ViewBag.mobileToFind = mobileToFind;
            return View("ChangeMobileBinding", changeMobileBindingLogs);
        }

        [HttpPost]
        public ViewResult ChangeMobileBindingOnDate(string SearchStartDate, string SearchEndDate)
        {
            IEnumerable<UserChangeBindMobileLog> changeMobileBindingLogs = new List<UserChangeBindMobileLog>();
            if (!string.IsNullOrEmpty(SearchStartDate) && !string.IsNullOrEmpty(SearchEndDate))
            {
                DateTime startTime, endTime;
                if (DateTime.TryParse(SearchStartDate, out startTime) && DateTime.TryParse(SearchEndDate, out endTime))
                {
                    try
                    {
                        using (var client = new UserAccountClient())
                        {
                            var result = client.QueryChangeBindMobileLogByDateTime(startTime, endTime);
                            result.ThrowIfException(true);
                            if (result.Success && result.Result != null)
                                changeMobileBindingLogs = result.Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        WebLog.LogException(ex);
                    }
                }
            }
            ViewBag.SearchStartDate = SearchStartDate;
            ViewBag.SearchEndDate = SearchEndDate;
            return View("ChangeMobileBinding", changeMobileBindingLogs);
        }

        [HttpPost]
        public async Task<JsonResult> SubmitChangeBindingRequest(string oldNumber, string newNumber)
        {
            try
            {
                var fetchOrderResult = 0;
                using (var client = new UserAccountClient())
                {
                    var oldUser = await client.GetUserByMobileAsync(oldNumber);
                    if (!oldUser.Success || oldUser.Result == null)
                    {
                        await client.LogChangeBindMobileActionAsync(
                            new UserChangeBindMobileLog
                            {
                                SourceBindMobile = oldNumber,
                                TargetBindMobile = newNumber,
                                Operator = User.Identity.Name,
                                OperateStatus = false,
                                FailReason = "需解绑手机号不存在",
                                CreatedTime = DateTime.Now
                            });
                        return Json(-1);
                    }

                    var newUser = await client.GetUserByMobileAsync(newNumber);
                    if (!newUser.Success || newUser.Result?.UserId == null || newUser.Result.UserId.Equals(Guid.Empty))
                    {
                        return Json(1);
                    }
                    fetchOrderResult =
                        DownloadAppManager.HasUnOrRecentlyCompletedOrder(newUser.Result.UserId.ToString("D"));
                    if (fetchOrderResult < 0)
                    {
                        await client.LogChangeBindMobileActionAsync(
                            new UserChangeBindMobileLog
                            {
                                SourceBindMobile = oldNumber,
                                TargetBindMobile = newNumber,
                                Operator = User.Identity.Name,
                                OperateStatus = false,
                                FailReason = fetchOrderResult == -4 ? "需绑定手机号有近期完成订单" : "需绑定手机号有未完成订单",
                                CreatedTime = DateTime.Now
                            });
                        return Json(fetchOrderResult);
                    }
                }
                using (var client = new YLHUserAccountClient())
                {
                    var ylhUser = await client.GetYLHUserInfoByMobileAsync(newNumber);
                    if (ylhUser.Success && ylhUser.Result?.UserId != null && !ylhUser.Result.UserId.Equals(Guid.Empty))
                        fetchOrderResult = -2;
                }
                if (fetchOrderResult == -2)
                {
                    using (var client = new UserAccountClient())
                    {
                        await client.LogChangeBindMobileActionAsync(
                            new UserChangeBindMobileLog
                            {
                                SourceBindMobile = oldNumber,
                                TargetBindMobile = newNumber,
                                Operator = User.Identity.Name,
                                OperateStatus = false,
                                FailReason = "需绑定手机号已关联永隆行账户",
                                CreatedTime = DateTime.Now
                            });
                    }
                }
                return Json(fetchOrderResult);
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return Json(0);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SendNewVcodePerSms(string phoneNumber)
        {
            var generatedVCode = await GenerateVerificationCode(phoneNumber, UserActionEnum.ChangeBindMobile);
            if (string.IsNullOrEmpty(generatedVCode))
                return Json(-2);
            var smsBody = "您正在修改手机号绑定，验证码为：" + generatedVCode + "(5分钟内有效），切勿将验证码泄露给他人哦！如有问题请咨询途虎客服：400-111-8868【途虎养车】";
            // 发送手机验证码
            var sendSmsSucceeded = await Business.Sms.SmsManager.SendVerificationCodeSmsMessageAsync(phoneNumber,generatedVCode, HttpContext.Request.UserHostAddress);

            return sendSmsSucceeded  ? Json(1) : Json(-1);
        }

        private static async Task<string> GenerateVerificationCode(string phoneNumber, UserActionEnum userAction)
        {
            try
            {
                using (var client = new UserAccountClient())
                {
                    var result = await client.GenerateVerificationCodeAsync(phoneNumber, userAction);
                    result.ThrowIfException(true);
                    if (result.Success)
                        return result.Result;
                }
            }
            catch (Exception exception)
            {
                WebLog.LogException(exception);
            }
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> SubmitChangeBindingAction(string oldNumber, string newNumber, string vCode)
        {
            try
            {
                using (var client = new UserAccountClient())
                {
                    var result = await client.ChangeBindMobileActionAsync(new UserChangeBindMobileAction
                    {
                        Operator = User.Identity.Name,
                        SourceBindMobile = oldNumber,
                        TargetBindMobile = newNumber,
                        TargetMobileCode = vCode
                    });
                    result.ThrowIfException(false);
                    if (result.Success && result.Result)
                    {
                        var flag = false;
                        var smsBody = "您的途虎账号已经与本手机号解绑， 如非本人操作请联系客服：400-111-8868【途虎养车】";
                        // 提交验证码
                        await Business.Sms.SmsManager.SubmitVerficationCodeAsync(newNumber, vCode);
                        // 模板：您的途虎账号已经与本手机号解绑， 如非本人操作请联系客服：400-111-8868
                        if (await Business.Sms.SmsManager.SendTemplateSmsMessageAsync(oldNumber, 75))
                            flag = true;

                        await client.LogChangeBindMobileActionAsync(
                            new UserChangeBindMobileLog
                            {
                                SourceBindMobile = oldNumber,
                                TargetBindMobile = newNumber,
                                Operator = User.Identity.Name,
                                OperateStatus = true,
                                CreatedTime = DateTime.Now
                            });
                        return flag ? Json("绑定成功但发送确认短信失败") : Json("绑定成功");
                    }
                    await client.LogChangeBindMobileActionAsync(
                               new UserChangeBindMobileLog
                               {
                                   SourceBindMobile = oldNumber,
                                   TargetBindMobile = newNumber,
                                   Operator = User.Identity.Name,
                                   OperateStatus = false,
                                   FailReason = result.ErrorMessage,
                                   CreatedTime = DateTime.Now
                               });
                    return Json(result.ErrorMessage);
                }
            }
            catch (Exception exception)
            {
                WebLog.LogException(exception);
                return Json("未知异常");
            }
        }
    }
}