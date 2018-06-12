using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Tuhu.Nosql;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Component.SystemFramework.Log;
using Tuhu.WebSite.Web.Activity.DataAccess;
using Tuhu.WebSite.Web.Activity.Helpers;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    /// <summary>
    /// 车险
    /// </summary>
    public class InsuranceController : Controller
    {
        private static readonly string _cachePrefix = (string)ConfigurationManager.AppSettings["CacheKeyProfix"];
        /// <summary>
        /// 报名
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [CrossHost]
        public ActionResult Entry(string phone, string code = "")
        {
            if (string.IsNullOrEmpty(phone) || !ValidatePhone(phone))
            {
                return Json(new { isSuccess = false, status = EntryStatus.FormatError }, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrEmpty(code))
            {
                if (GetReVCode(phone.Trim()).ToLower() != code.Trim().ToLower())
                {
                    return Json(new { isSuccess = false, status = EntryStatus.CodeError }, JsonRequestBehavior.AllowGet);
                }
            }
            //如果没有验证码，并且用户不存在返回错误
            else if (Insurance.GetUserByPhone(phone.Trim()) == null)
            {
                return Json(new { isSuccess = false, status = EntryStatus.EntryError }, JsonRequestBehavior.AllowGet);
            }
            var result = Insurance.Entry(phone);
            return Json(new { isSuccess = (result == EntryStatus.Success || result == EntryStatus.RegisterASuccess), status = result }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [CrossHost]
        public ActionResult SendMessage(string phone)
        {
            try
            {
                if (string.IsNullOrEmpty(phone) || !ValidatePhone(phone))
                {
                    return Json(new { isSuccess = false, status = EntryStatus.FormatError }, JsonRequestBehavior.AllowGet);
                }
                var vCode = GeneraCode();
                using (var client = new SmsClient())
                {
                    client.SendVerificationCode(new SendVerificationCodeRequest
                    {
                        Cellphone = phone,
                        Host = Request.Url.Host,
                        UserIp = Request.UserIp(),
                        VerificationCode = vCode
                    }).ThrowIfException(true);
                }
                var result = SetVcode(phone, vCode);
                return Json(new { isSuccess = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return Json(new { isSuccess = false, status = EntryStatus.EntryError }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 设置验证码（Redis）过期时间1minutes
        /// </summary>
        /// <param name="phone"></param>
        private bool SetVcode(string phone, string vCode)
        {
            using (var reclient = CacheHelper.CreateCacheClient("Che_Xian_Cache"))
            {
                var result = reclient.Set<string>(_cachePrefix + phone, vCode);
                if (result.Success == true && result.Exception == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 从Redis中获取验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        private string GetReVCode(string phone)
        {
            try
            {
                using (var reclient = CacheHelper.CreateCacheClient("Che_Xian_Cache"))
                {
                    var result = reclient.Get<string>(_cachePrefix + phone);
                    if (result.Success == true && result.Exception == null)
                    {
                        return result.Value;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private string GeneraCode()
        {
            Random rd = new Random();
            return rd.Next(1000, 10000).ToString();
        }
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [NonAction]
        private bool ValidatePhone(string phone)
        {
            Regex reg = new Regex("^0?(13[0-9]|15[012356789]|17[0678]|18[0-9]|14[57])[0-9]{8}$");
            return reg.IsMatch(phone);
        }

    }

}