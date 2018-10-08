using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.Framework.FileUpload;
using Tuhu.Provisioning.Business.Push;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity.Push;
using Tuhu.Provisioning.Models.Push;

namespace Tuhu.Provisioning.Controllers
{
    public class PushController : Controller
    {
        private const int Push_Broadcast = 1;

        private const int Push_Groupcast = 3;

        private const int Push_Single = 2;

        [PowerManage]
        public ActionResult PushMessage()
        {
            return View();
        }

        [PowerManage]
        [ValidateInput(false)]
        public JsonResult SubmitPushMessage(PushMessageViewModel message)
        {
            var count = -1;

            if (message != null 
                && !string.IsNullOrWhiteSpace(message.Description) 
                && !string.IsNullOrWhiteSpace(message.Title)
                && !string.IsNullOrWhiteSpace(message.Content))
            {

                if (message.ExpireTime.HasValue)
                {
                    message.SendTime = message.SendTime ?? DateTime.Now;
                    if (message.ExpireTime < DateTime.Now || message.ExpireTime < message.SendTime.Value.AddMinutes(30))
                    {
                        return Json(-2);
                    }
                }
                if (message.APPExpireTime.HasValue)
                {
                    message.SendTime = message.SendTime ?? DateTime.Now;
                    if (message.APPExpireTime < DateTime.Now || message.APPExpireTime < message.SendTime.Value.AddMinutes(30))
                    {
                        return Json(-2);
                    }
                }

                if (message.AfterOpen == "1" && string.IsNullOrWhiteSpace(message.AppActivity))
                {
                    return Json(-3);
                }

                if (!string.IsNullOrWhiteSpace(message.IOSValue1))
                {
                    message.IOSValue1 = message.IOSValue1.Replace("“", "\"");
                    message.IOSValue1 = message.IOSValue1.Replace("”", "\"");
                }

                if (!string.IsNullOrWhiteSpace(message.IOSValue2))
                {
                    message.IOSValue2 = message.IOSValue2.Replace("“", "\"");
                    message.IOSValue2 = message.IOSValue2.Replace("”", "\"");
                }

                if (!string.IsNullOrWhiteSpace(message.IOSValue3))
                {
                    message.IOSValue3 = message.IOSValue3.Replace("“", "\"");
                    message.IOSValue3 = message.IOSValue3.Replace("”", "\"");
                }

                if (!string.IsNullOrWhiteSpace(message.AndriodValue1))
                {
                    message.AndriodValue1 = message.AndriodValue1.Replace("“", "\"");
                    message.AndriodValue1 = message.AndriodValue1.Replace("”", "\"");
                }

                if (!string.IsNullOrWhiteSpace(message.AndriodValue2))
                {
                    message.AndriodValue2 = message.AndriodValue2.Replace("“", "\"");
                    message.AndriodValue2 = message.AndriodValue2.Replace("”", "\"");
                }
                if (User == null || User.Identity == null)
                    return Json(10);
                message.OperUser = User.Identity.Name;

                if (message.PushType == Push_Broadcast) // 广播
                {
                    var result = PushHandler.PushTaskMessage(message);
                    count = result ? 1 : -10;
                }
                else if (message.PushType == Push_Groupcast) // 组播
                {
                    if (string.IsNullOrEmpty(message.Tags))
                    {
                        return Json(-6);
                    }
                    var result = PushHandler.PushTaskMessage(message);
                    count = result ? 1 : -10;
                } 
                else if (message.PushType == Push_Single) // 单播
                {
                    #region Get phone numbers

                    var cellphones = new List<string>(10000);
                    var cellphonesFile = Request.Files.Count > 0 ? Request.Files[0] : null;
                    if (cellphonesFile == null || cellphonesFile.ContentLength < 11)
                        return Json(-5);

                    try
                    {
                        using (var stream = new StreamReader(cellphonesFile.InputStream, Encoding.UTF8))
                        {
                            foreach (var item in Regex.Split(stream.ReadToEnd(), @"\D")
                                .Where(
                                    cellphone => !string.IsNullOrEmpty(cellphone) && Regex.IsMatch(cellphone, @"^1\d{10}$"))
                                .Distinct())
                            {
                                cellphones.Add(item);
                            }
                        }

                        //var preList = new PushManager().GetPushMsgPersonConfig();

                        //if (preList != null)
                        //// 添加预配置的手机号
                        //{
                        //    cellphones.AddRange(preList);
                        //    cellphones = cellphones.Select(x => x).Distinct().ToList();
                        //}

                        if (cellphones.Count < 1)
                            return Json(0);
                        var result = PushHandler.PushMessage(message, cellphones);
                        count = result ? cellphones.Count : -10;
                    }
                    catch (Exception)
                    {
                        return Json(-10);
                    }
                    #endregion
                }
            }

            return Json(count);
        }

        [PowerManage]
        public ActionResult SeachPushHis(int? pageNo)
        {
            int sumNum;
            var pageSize = 20;
            int pageIndex = (pageNo == null) ? 1: pageNo.Value;
            var model =new PushMessageListModel();

            var taskList = PushHandler.SearchPushHis(pageIndex, pageSize,out sumNum);
            model.SumNum = sumNum;
            model.PushMessageList = taskList;

            if (taskList!=null&&taskList.Count > 1)
            {
                double tmp = sumNum / double.Parse(pageSize.ToString());
                int pageNum = (int)Math.Ceiling(tmp);
                string tmpUrl = string.Format("/Push/SeachPushHis?pageno={0}", "{0}");
                model.PagerStr = PageHtmlHelper.GetListPager(tmpUrl, pageNo.Value, pageNum);
                if (pageNum == 1) { model.PagerStr = string.Empty; }
            }
            model.SearchInfo = new PushMessageSeachInfoModel
            {
                PageNo = pageNo
            };

            return View(model);
        }

        public ActionResult DeletePushMessage(int messageId)
        {
            ResponseModel rm = new ResponseModel();
            rm.IsSuccess = false;
            string errmsg = "";
            if (User == null || User.Identity == null)
                rm.OutMessage = "删除失败,登录信息错误";
            else
                try
                {
                    rm.IsSuccess = PushHandler.DeleteMessages(messageId, User.Identity.Name, out errmsg);
                    rm.OutMessage = errmsg;
                }
                catch (Exception ex)
                {
                    rm.IsSuccess = false;
                    rm.OutMessage = "删除失败，服务异常";
                }
            return Json(rm);
        }
        public ActionResult UpdateAPPExpireTime(int messageId, string dateTime)
        {
            ResponseModel rm = new ResponseModel();
            rm.IsSuccess = false;
            string errmsg = "";
            DateTime appTime;
            if (!DateTime.TryParse(dateTime, out appTime))
            {
                rm.OutMessage = "请输入正确的时间格式！";
                return Json(rm);
            }
            if (User == null || User.Identity == null)
            {
                rm.OutMessage = "删除失败,登录信息错误";
                return Json(rm);
            }
            else
                try
                {
                    rm.IsSuccess = PushHandler.UpdateAPPExpireTime(messageId, appTime, out errmsg);
                    rm.OutMessage = errmsg;
                    return Json(rm);
                }
                catch (Exception ex)
                {
                    rm.IsSuccess = false;
                    rm.OutMessage = "删除失败，服务异常";
                    return Json(rm);
                }
        }

        public JsonResult GetAllPushTag()
        {
            var pushTags = new PushManager().GetAllPushTag();

            var tagLevel = new List<TagModel>();

            if (pushTags != null && pushTags.Any())
            {
                var firstLevels = pushTags.Select(x => x.FirstLevel).Distinct();               

                foreach (var first in firstLevels)
                {
                    var firstLevelTag = new TagModel();
                    firstLevelTag.name = first;
                    tagLevel.Add(firstLevelTag);
                    var secondLevels = pushTags.Where(x => x.FirstLevel == first).Select(x => x.SecondLevel).Distinct();

                    foreach (var second in secondLevels)
                    {
                        var secondLevelTag = new TagModel();
                        secondLevelTag.name = second;
                        firstLevelTag.children.Add(secondLevelTag);
                        var tags = pushTags.Where(x => x.SecondLevel == second).Select(x => new { x.TagName, x.Description}).Distinct();

                        foreach (var tag in tags)
                        {
                            var thirdLevelTag = new TagModel();
                            thirdLevelTag.name = tag.Description;
                            thirdLevelTag.key = tag.TagName;
                            secondLevelTag.children.Add(thirdLevelTag);
                        }
                    }
                }
            }

            return Json(tagLevel);
        }

        /// <summary>
        /// 图片上传地址
        /// </summary>
        /// <returns></returns>
        public ActionResult ImageUploadToAli(int checkModel)
        {
            string _BImage = string.Empty;
            string _SImage = string.Empty;
            string _ImgGuid = Guid.NewGuid().ToString();
            Exception ex = null;
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var ext = Path.GetExtension(Imgfile.FileName);
                    var client = new BaoYangYearCardController.WcfClinet<IFileUpload>();
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);

                    var _BytToImg = ImageConverter.BytToImg(buffer);
                    if (checkModel == 1)
                    {
                        if (Imgfile.ContentLength >= 1024 * 256)
                        {
                            ex = new Exception("图片大小不能超过256k");
                        }
                        else
                        { 
                            int height;
                            if (int.TryParse(_BytToImg?["Height"].ToString(), out height)&&
                                height>=256)
                            {
                                ex = new Exception("图片高度最大256dp");
                            }
                        }
                    }


                    if (ex == null && _BytToImg != null && _BytToImg.Count > 0)
                    {
                        _ImgGuid = string.Format(_ImgGuid + "${0}w_{1}h", _BytToImg["Width"], _BytToImg["Height"]);
                        _BImage = WebConfigurationManager.AppSettings["UploadDoMain_image_push"] + _ImgGuid + ext;

                        client.InvokeWcfClinet(w => w.UploadImage(_BImage, buffer, 0, 0));
                    }
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            return Json(new
            {
                FullImage = WebConfigurationManager.AppSettings["DoMain_image"] + _BImage,
                PathImage = _BImage,
                Msg = ex?.Message ?? "上传成功"
            }, "text/html");
        }

    }
}