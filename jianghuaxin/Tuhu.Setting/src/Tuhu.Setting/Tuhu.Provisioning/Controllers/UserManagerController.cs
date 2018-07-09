using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;
using Tuhu.Component.Framework.Extension;
using Tuhu.Service.OAuth;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.DataAccess.Entity.UserManager;
using System.Data;

namespace Tuhu.Provisioning.Controllers
{
    public class UserManagerController : Controller
    {
        // GET: UserManager
        public ActionResult Index()
        {
            return View();
        }

        [PowerManage]
        public ActionResult LogOffUserByMobile()
        {
            return View();
        }

        [PowerManage]
        public ActionResult LogOutUserByMobile()
        {
            return View();
        }

        public ActionResult UserDailyInc()
        {
            return View();
        }

        public JsonResult GetUserDailyInc(DateTime startTime, DateTime endTime)
        {
            PostUserDailyInc postItem = new PostUserDailyInc();
            return Json(postItem);
        }


        public PartialViewResult LogOutUserByMobileInfo(string mobileNumber)
        {
            User info = new Service.UserAccount.Models.User();
            if (!string.IsNullOrWhiteSpace(mobileNumber))
            {
                try
                {
                    using (var client = new UserAccountClient())
                    {
                        var result = client.GetUserByMobile(mobileNumber);
                        result.ThrowIfException(true);
                        if (result.Success && result.Result != null)
                            info = result.Result;
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }
            }
            return PartialView(info);
        }

        public JsonResult LogOutUserById(string userId)
        {
            bool flag = false;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                Guid uId = new Guid(userId);
                try
                {
                    var cleanTokenResult = 0;
                    using (var client = new AccessTokenClient())
                    {
                        var result = client.RemoveAll(uId, "Setting站点工具登出");
                        result.ThrowIfException(true);
                        cleanTokenResult = result.Result;
                    }
                    if (cleanTokenResult >= 0) flag = true;

                    using (var useraccoutClient = new UserAccountClient())
                    {
                        var insertLog = useraccoutClient.LogUserAction(new UserLog
                        {
                            Action = UserActionEnum.LogOut,
                            CreatedTime = DateTime.Now,
                            UserId = uId,
                            ChannelIn = nameof(ChannelIn.H5),
                            Content = "Setting站点内手动登出该用户"
                        });
                        insertLog.ThrowIfException(true);
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }
            }
            return Json(flag);
        }

        public PartialViewResult LogOffUserByMobileInfo(string mobileNumber)
        {
            User info = new Service.UserAccount.Models.User();
            if (!string.IsNullOrWhiteSpace(mobileNumber))
            {
                try
                {
                    using (var client = new UserAccountClient())
                    {
                        var result = client.GetUserByMobile(mobileNumber);
                        result.ThrowIfException(true);
                        if (result.Success && result.Result != null)
                            info = result.Result;
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }
            }
            return PartialView(info);
        }

        public JsonResult LogOffUserById(string userId, string mobile)
        {
            int flag = 0;
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(mobile))
            {
                Guid uId = new Guid(userId);
                try
                {
                    var existYlh = false;
                    var logoffResult = false;
                    using (var ylhClient = new YLHUserAccountClient())
                    {
                        var ylhUser = ylhClient.GetYLHUserInfoByMobile(mobile);
                        ylhUser.ThrowIfException(true);
                        if (ylhUser.Result != null && ylhUser.Result.UserId != Guid.Empty)
                            existYlh = true;
                    }

                    if (existYlh) flag = -1;
                    else
                    {
                        using (var client = new UserAccountClient())
                        {
                            var logoff = client.LogOffUser(uId);
                            logoff.ThrowIfException(true);
                            logoffResult = logoff.Result;
                        }
                        if (logoffResult) flag = 1;

                        using (var useraccoutClient = new UserAccountClient())
                        {
                            var insertLog = useraccoutClient.LogUserAction(new UserLog
                            {
                                Action = UserActionEnum.LogOff,
                                CreatedTime = DateTime.Now,
                                UserId = uId,
                                ChannelIn = nameof(ChannelIn.H5),
                                Content = ThreadIdentity.Operator.Name + "在Setting站点内手动注销该用户"
                            });
                            insertLog.ThrowIfException(true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }
            }
            return Json(flag);
        }

        public ActionResult BatchLogOutUserByUserId()
        {
            return View();
        }

        [HttpPost]
        public JsonResult BatchLogOutUserByUserIdAction()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (!file.FileName.Contains(".xlsx") && !file.FileName.Contains(".xls"))
                        return Json(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件！" }, "text/html");

                    var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("sheet1", true);

                    foreach(DataRow dr in dt.Rows)
                    {
                        var userid = dr[0].ToString();
                        if (!string.IsNullOrWhiteSpace(userid))
                        {
                            LogOutUserByUserId(userid);
                        }
                    }
                }
                return Json(new { Status = -1, Error = "请选择文件" }, "text/html");
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return Json(new { Status = -2, Error = ex }, "text/html");
            }
        }

        private void LogOutUserByUserId(string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                Guid uId = new Guid(userId);
                try
                {
                    using (var client = new AccessTokenClient())
                    {
                        var result = client.RemoveAll(uId, "Setting站点工具登出");
                        result.ThrowIfException(true);
                    }

                    using (var useraccoutClient = new UserAccountClient())
                    {
                        var insertLog = useraccoutClient.LogUserAction(new UserLog
                        {
                            Action = UserActionEnum.LogOut,
                            CreatedTime = DateTime.Now,
                            UserId = uId,
                            ChannelIn = nameof(ChannelIn.H5),
                            Content = "Setting站点内手动登出该用户"
                        });
                        insertLog.ThrowIfException(true);
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }
            }
        }
    }
}