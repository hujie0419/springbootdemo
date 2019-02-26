using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Push;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.Models.Push;
using Tuhu.Service.MemberGroup;
using Tuhu.Service.MemberGroup.Models.Tag.TagInfo;
using Tuhu.Service.PushApi.Models.MessageBox;
using Tuhu.Service.PushApi.Models.Push;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;
using framework = Tuhu.Component.Framework.Extension;
using swc = System.Web.Configuration;

namespace Tuhu.Provisioning.Controllers
{
    public class PushManagerController : Controller
    {

        // GET: PushManager
        [PowerManage]
        public async Task<ActionResult> Index(TemplateQuery query)
        {
            query.PageIndex = query.PageIndex == 0 ? 1 : query.PageIndex;
            query.PageSize = 50;
            query.SendTypes = new List<SendType>() { SendType.Broadcast };
            var datas = await PushManagerNew.SelectPushTemplatesAsync(query);
            if (datas != null)
            {
                query.TotalCount = datas.TotalCount;
            }
            //var result = new QueryResult()
            //{
            //    Result = datas
            //};
            ViewBag.query = query;
            if (datas != null && datas.Result.Any())
            {
                var templateids = datas.Result.Select(x => x.PKID);
                var result = await PushManagerNew.SelectTemplatePushRatesAsync(templateids);
                ViewBag.Statisticaldata = result;
            }
            return View(datas);
        }

        [PowerManage]
        public async Task<ActionResult> TemplateEdit(int? batchid)
        {
            var models = await PushManagerNew.SelectMessageNavigationTypesAsync();
            ViewData["NavigationTypes"] = models;
            if (batchid.HasValue)
            {
                var datas = await PushManagerNew.SelectPushTemplatesByBatchIDAsync(batchid.Value);
                Dictionary<string, string> dict = new Dictionary<string, string>();

                if (datas != null && datas.Count > 0)
                {
                    foreach (var t in datas)
                    {
                        if (t.AppExpireTime.HasValue)
                        {
                            var temp = new DateTime();
                            DateTime d = new DateTime(t.AppExpireTime.Value * 10000);
                            var span = d - temp;

                            string s = string.Format("{0}/{1}/{2}", span.Days, span.Hours, span.Minutes);
                            dict.Add(t.DeviceType.ToString(), s);

                        }
                    }
                }
                ViewData["dict"] = dict;
                return View(datas);
            }
            return View();
        }


        private long? ConvertExpire(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var d = str.Split('/');
                DateTime time = new DateTime();
                if (d != null && d.Count() > 0)
                {
                    for (int i = 0; i < d.Count(); i++)
                    {
                        int t;
                        if (int.TryParse(d[i], out t))
                        {
                            switch (i)
                            {
                                case 0:
                                    time = time.AddDays(t);
                                    break;
                                case 1:
                                    time = time.AddHours(t);
                                    break;
                                case 2:
                                    time = time.AddMinutes(t);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    return time.Ticks / 10000;
                }
            }
            return null;
        }

        [PowerManage]
        public async Task<ActionResult> IOSSubmitTemplate(IOSPushViewModel model)
        {
            JsonResult jr = new JsonResult();
            string userNo = HttpContext.User.Identity.Name;
            var expiretime = ConvertExpire(model.ExpireTime);
            //if (expiretime.HasValue && expiretime.Value >= 604800000)
            //{
            //    jr.Data = new { code = 0, msg = "消息过期周期过大" };
            //    return jr;
            //}
            //if (model.PushTime.HasValue && model.PushTime.Value <= DateTime.Now)
            //{
            //    jr.Data = new { code = 0, msg = "推送时间不能小于当前时间" };
            //    return jr;
            //}

            if (model.MessageBoxShowType == MessageBoxShowType.OrderText)
            {
                try
                {
                    JsonConvert.DeserializeObject<OrderMessageBoxInfo>(model.Content);
                }
                catch (System.Exception ex)
                {
                    jr.Data = new { code = 0, msg = "推送类型为订单类型,消息内容不合法." };
                    return jr;
                }
#if !DEBUG
                string loginuser = HttpContext.User.Identity.Name;
                if (string.IsNullOrEmpty(loginuser) || (!loginuser.Contains("dongjing1") && !loginuser.Contains("cheliangliang") && !loginuser.Contains("devteam")))
                {
                    jr.Data = new { code = 0, msg = "你没有权限" };
                    return jr;
                }
#endif
            }
            if (string.IsNullOrEmpty(model.Title) && model.MessageBoxShowType != MessageBoxShowType.ActivityText)
            {
                jr.Data = new { code = 0, msg = "非互动消息文字类型标题不能为空" };
                return jr;
            }

            if (model.DeviceType == DeviceType.MessageBox &&
                (model.MessageBoxShowType == MessageBoxShowType.BigImageText ||
                 model.MessageBoxShowType == MessageBoxShowType.SmallImageText))
            {
                var imageconfigs = (await PushManagerNew.SelectImageTextByBatchIdAsync(model.BatchID))?.ToList() ?? new List<MessageImageText>();
                if (model.MessageBoxShowType == MessageBoxShowType.SmallImageText)
                {
                    if (imageconfigs.Count > 1)
                    {
                        jr.Data = new { code = 0, msg = "小图文最多1条图文消息" };
                        return jr;
                    }
                }
                if (model.MessageBoxShowType == MessageBoxShowType.BigImageText)
                {
                    if (imageconfigs.Count > 5)
                    {
                        jr.Data = new { code = 0, msg = "大图文最多5条图文消息" };
                        return jr;
                    }
                }

                var firstconfig = imageconfigs?.OrderBy(x => x.Order)?.FirstOrDefault();
                if (firstconfig != null)
                {
                    model.Title = firstconfig.Title;
                    model.Content = firstconfig.Desctiption;
                    model.AppActivity = firstconfig.JumpUrl;
                }

            }
            #region extradict
            Dictionary<string, string> extradict = new Dictionary<string, string>();
            if (model.ExtraKey != null)
            {
                if (model.DeviceType == DeviceType.MessageBox)
                {
                    Dictionary<string, string> iosdict = new Dictionary<string, string>();
                    Dictionary<string, string> androiddict = new Dictionary<string, string>();
                    var tempdict = model.ExtraKey.ToList();
                    if (!string.IsNullOrEmpty(tempdict[0].Key) && !string.IsNullOrEmpty(tempdict[0].Value))
                    {
                        iosdict[tempdict[0].Key] = tempdict[0].Value;
                    }
                    if (!string.IsNullOrEmpty(tempdict[1].Key) && !string.IsNullOrEmpty(tempdict[1].Value))
                    {
                        androiddict[tempdict[1].Key] = tempdict[1].Value;
                    }
                    if (!string.IsNullOrEmpty(tempdict[2].Key) && !string.IsNullOrEmpty(tempdict[2].Value))
                    {
                        androiddict[tempdict[2].Key] = tempdict[2].Value;
                    }
                    if (!string.IsNullOrEmpty(tempdict[3].Value))
                    {
                        androiddict[tempdict[3].Key] = tempdict[3].Value;
                    }
                    extradict["ios"] = JsonConvert.SerializeObject(iosdict);
                    extradict["android"] = JsonConvert.SerializeObject(androiddict);
                }
                else
                {
                    foreach (var extraKey in model.ExtraKey)
                    {
                        if (!string.IsNullOrEmpty(extraKey.Key))
                        {
                            extradict[extraKey.Key] = extraKey.Value;
                        }
                    }
                }
            }
            else
            {
                if (model.DeviceType == DeviceType.MessageBox)
                {
                    extradict["android"] = extradict["ios"] = JsonConvert.SerializeObject(new Dictionary<string, string>());
                }
            }
            if (model.DeviceType == DeviceType.Android)
            {
                extradict["showDialog"] = "show";
                Action<string> HandlerFormExtra = s =>
                {
                    if (!string.IsNullOrEmpty(Request.Form[s]))
                    {
                        extradict[s] = Request.Form[s];
                    }
                };
                HandlerFormExtra("sound");
                HandlerFormExtra("shake");
                HandlerFormExtra("light");
                HandlerFormExtra("notify_foreground");
                HandlerFormExtra("wifi");
            }


            if (extradict.Count >= 10)
            {
                jr.Data = new { code = 0, msg = "键值对过多" };
                return jr;
            }
            #endregion
            #region 消息预览

            string imgurl = Request.Form["BigImagePath"];


            if (!string.IsNullOrEmpty(Request.Form["SubmitType"]) && Request.Form["SubmitType"].Equals("0"))
            {

                try
                {
                    var templatedata = new PushTemplate()
                    {
                        SourceChannel = SourceChannel.Setting,
                        CreateTime = DateTime.Now,
                        PushStatus = PushStatus.Intend,
                        Title = model.Title,
                        SubTitle = model.SubTitle,
                        Sendtime = DateTime.Now,
                        SendType = SendType.Single,
                        SoundType = model.Soundtype == 1 ? "default" : "",
                        Bdage = model.Bdagetype == 1 && model.Bdage != 0 ? model.Bdage : (int?)null,
                        DeviceType = model.DeviceType,
                        MessageType = model.DeviceType == DeviceType.MessageBox ? (MessageType)model.MessageType : (MessageType?)null,
                        Content = model.Content,
                        IsPreview = true,
                        AppActivity = model.AppActivity,
                        ImageUrl = imgurl,
                        MessageNavigationTypeId = model.MessageNavigationTypeId,
                        MessageBoxShowType = model.MessageBoxShowType,
                        BatchID = model.BatchID,
                        PKID = model.PKID
                    };
                    templatedata.ExtraDict = extradict;
                    templatedata.AppExpireTime = ConvertExpire(model.ExpireTime);
                    if (templatedata.ExtraDict.Count >= 10)
                    {
                        jr.Data = new { code = 0, msg = "键值对过多" };
                        return jr;
                    }
                    string phones = Request.Form["phonepreview"];
                    var issuccess = false;
                    if (model.DeviceType == DeviceType.Android)
                    {
                        issuccess = await PushManagerNew.PushByAliasAsync(phones.Split(','), templatedata);
                    }
                    else
                    {
                        issuccess = await PushManagerNew.PushByAliasAsync(phones.Split(','), templatedata);
                    }


                    jr.Data = new { code = issuccess ? 1 : 0, msg = issuccess ? "推送成功" : "推送失败" };
                }
                catch (Exception ex)
                {
                    framework.WebLog.LogException(ex);
                    jr.Data = new { code = 0, msg = "推送失败..." + ex.Message };
                }

                return jr;
            }
            #endregion

            if (model.PKID != 0)
            {

                var template = await PushManagerNew.SelectTemplateByPKIDAsync(model.PKID);
                if (model.Bdagetype == 1)
                {
                    template.Bdage = model.Bdage;
                }
                else
                {
                    template.Bdage = null;
                }
                template.SendType = SendType.Broadcast;
                template.ImageUrl = imgurl;
                template.SubTitle = model.SubTitle;
                template.Title = model.Title;
                template.Content = model.Content;

                template.SoundType = model.Soundtype == 1 ? "default" : "";
                template.MessageBoxShowType = model.MessageBoxShowType;
                template.MessageNavigationTypeId = model.MessageNavigationTypeId;

                template.MessageType = (MessageType)model.MessageType;

                template.AppActivity = model.AppActivity?.Trim()?.Replace("\n", "")?.Replace("\t", "");
                template.ExtraDict = extradict;
                template.AppExpireTime = ConvertExpire(model.ExpireTime);
                template.Sendtime = model.PushTime;
                if (template.ExtraDict.Count >= 10)
                {
                    jr.Data = new { code = 0, msg = "键值对过多" };
                    return jr;
                }
                template.CreateUser = userNo;
                var result = await PushManagerNew.CreateOrUpdateTemplateAsync(template);
                jr.Data = new { code = result > 0 ? 1 : 0, msg = result > 0 ? "更新成功" : "更新失败" };
                return jr;
            }
            else
            {
                var template = new PushTemplate()
                {
                    SourceChannel = SourceChannel.Setting,
                    CreateTime = DateTime.Now,
                    PushStatus = PushStatus.Prepare,
                    Title = model.Title,
                    SubTitle = model.SubTitle,
                    Sendtime = !model.PushTime.HasValue ? DateTime.Now : model.PushTime.Value,
                    SendType = SendType.Broadcast,
                    SoundType = model.Soundtype == 1 ? "default" : "",
                    Bdage = model.Bdagetype == 1 ? model.Bdage : (int?)null,
                    DeviceType = model.DeviceType,
                    Content = model.Content,
                    PlanName = "计划推送",
                    Description = "全部用户",
                    AppExpireTime = ConvertExpire(model.ExpireTime),
                    AppActivity = model.AppActivity?.Trim()?.Replace("\n", "")?.Replace("\t", ""),
                    ImageUrl = imgurl,
                    MessageBoxShowType = model.MessageBoxShowType,
                    MessageNavigationTypeId = model.MessageNavigationTypeId
                };
                template.CreateUser = userNo;
                template.ExtraDict = extradict;
                if (model.DeviceType == DeviceType.MessageBox)
                {
                    template.MessageType = (MessageType)model.MessageType;
                }
                else
                {
                    template.MessageType = null;
                }

                if (template.ExtraDict.Count >= 10)
                {
                    jr.Data = new { code = 0, msg = "键值对过多" };
                    return jr;
                }

                var result = await PushManagerNew.CreateOrUpdateTemplateAsync(template);

                var batchid = result;
                if (model.BatchID != 0)
                {
                    batchid = model.BatchID;
                }
                if (result > 0)
                {
                    await PushManagerNew.UpdateTemplateBatchIDAsync(result, batchid);
                }
                jr.Data = new { code = result > 0 ? 1 : 0, msg = result > 0 ? "保存成功" : "保存失败", iscreate = batchid };
                return jr;
            }
        }

        /// <summary>
        /// 图片上传地址
        /// </summary>
        /// <returns></returns>
        public ActionResult ImageUploadToAli(int checkModel)
        {
            string _BImage = string.Empty;
            string _SImage = string.Empty;
            string _ImgGuid = Guid.NewGuid().ToString().Replace("-", "");
            Exception ex = null;
            string fullimgurl = "";
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var ext = Path.GetExtension(Imgfile.FileName);
                    if (ext.IndexOf("png") < 0)
                    {
                        ex = new Exception("请上传png格式图片");
                    }
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);

                    var _BytToImg = ImageConverter.BytToImg(buffer);
                    if (checkModel == 1)
                    {
                        if (Imgfile.ContentLength >= 1024 * 200)
                        {
                            ex = new Exception("图片大小不能超过200k");
                        }
                        else
                        {
                            int height;
                            if (int.TryParse(_BytToImg?["Height"].ToString(), out height) &&
                                height >= 256)
                            {
                                ex = new Exception("图片高度最大256dp");
                            }
                        }
                    }


                    if (ex == null && _BytToImg != null && _BytToImg.Count > 0)
                    {
                        //_ImgGuid = string.Format(_ImgGuid + "${0}w_{1}h", _BytToImg["Width"], _BytToImg["Height"]);
                        //_BImage = WebConfigurationManager.AppSettings["UploadDoMain_image_push"] + _ImgGuid + ext;
                        //if (!string.IsNullOrEmpty(_BImage))
                        //{
                        //    fullimgurl = "https://img1.tuhu.org" + _BImage;
                        //}
                        using (var client = new FileUploadClient())
                        {
                            var result = client.UploadImage(new ImageUploadRequest(_BImage, buffer));
                            result.ThrowIfException(true);
                            if (result.Success)
                            {
                                _BImage = result.Result;
                            }
                            if (!string.IsNullOrEmpty(_BImage))
                            {
                                fullimgurl = swc.WebConfigurationManager.AppSettings["DoMain_image"] + _BImage;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            return Json(new
            {
                FullImage = fullimgurl,
                PathImage = _BImage,
                Msg = ex?.Message ?? "上传成功"
            }, "text/html");
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePlanInfo()
        {
            JsonResult jr = new JsonResult();
            var iosenable = Request.Form["iosenable"];
            var iosid = Request.Form["iosid"];
            var androidenable = Request.Form["androidenable"];
            var androidid = Request.Form["androidid"];
            var boxenable = Request.Form["boxenable"];
            var boxid = Request.Form["boxid"];
            var BatchID = Request.Form["BatchID"];
            var PlanName = Request.Form["PlanName"];

            if (string.IsNullOrEmpty(BatchID) || BatchID == "0")
            {
                jr.Data = new { code = 0, msg = "请先创建计划" };
                return jr;
            }
            if (string.IsNullOrEmpty(PlanName))
            {
                jr.Data = new { code = 0, msg = "计划名称不能为空" };
                return jr;
            }
            Dictionary<int, PushStatus> templatestatus = new Dictionary<int, PushStatus>();

            Action<string, string> handleData = (idstr, statusstr) =>
            {
                int id;
                if (string.IsNullOrEmpty(idstr) || idstr == "0")
                {
                    return;
                }
                if (int.TryParse(idstr, out id))
                {
                    if (string.IsNullOrEmpty(statusstr))
                    {
                        throw new NotSupportedException("数据错误");
                    }
                    templatestatus.Add(id, statusstr == "1" ? PushStatus.Prepare : PushStatus.Suspend);

                }
                else
                {
                    throw new NotSupportedException("数据错误");
                }
            };

            try
            {
                var datas = await PushManagerNew.SelectPushTemplatesByBatchIDAsync(Convert.ToInt32(BatchID));
                if (!datas.Any())
                {
                    jr.Data = new { code = 0, msg = "请先创建计划" };
                    return jr;
                }
                if (datas.Any(x => x.PushStatus == PushStatus.Success) ||
                    datas.Any(x => x.PushStatus == PushStatus.Pushing)
                    //|| datas.Any(x => x.PushStatus == PushStatus.Failed)
                    )
                {
                    jr.Data = new { code = 0, msg = "该计划正在推送或已经推送结束,不能修改状态" };
                    return jr;
                }
                handleData(iosid, iosenable);
                handleData(androidid, androidenable);
                handleData(boxid, boxenable);
                if (templatestatus.Any())
                {
                    var result =
                        await PushManagerNew.UpdateTemplatePlanInfoAsync(Convert.ToInt32(BatchID), PlanName,DateTime.Now.AddMonths(1),
                            templatestatus);
                    jr.Data = new { code = result  ? 1 : 0, msg = result ? "更新成功" : "更新失败" };
                    return jr;
                }
                else
                {
                    throw new NotSupportedException("数据错误");
                }

            }
            catch (NotSupportedException)
            {
                jr.Data = new { code = 0, msg = "数据错误" };
                return jr;
            }
            catch (Exception ex)
            {
                framework.WebLog.LogException(ex);
                jr.Data = new { code = 0, msg = ex.Message };
                return jr;
            }

        }

        [HttpPost]
        public async Task<ActionResult> SetPlanReadyPush(int batchid)
        {
            JsonResult jr = new JsonResult();
            try
            {
                var datas = await PushManagerNew.SelectPushTemplatesByBatchIDAsync(batchid);
                if (datas.Any(x => x.PushStatus == PushStatus.Success) ||
                    datas.Any(x => x.PushStatus == PushStatus.Pushing)
                //|| datas.Any(x => x.PushStatus == PushStatus.Failed)
                )
                {
                    jr.Data = new { code = 0, msg = "该计划正在推送或已经推送结束,不能修改状态" };
                    return jr;
                }
                bool issuccess = true;
                foreach (var pushTemplate in datas)
                {
                    if (pushTemplate.PushStatus == PushStatus.Prepare && pushTemplate.SendType == SendType.Broadcast)
                    {
                        var result = await PushManagerNew.UpdateTemplatePushStatusAsync(pushTemplate.PKID, PushStatus.Intend);
                        issuccess = issuccess && result;
                    }
                }
                jr.Data = new { code = issuccess ? 1 : 0, msg = issuccess ? "更新成功" : "更新失败" };
            }
            catch (System.Exception ex)
            {
                framework.WebLog.LogException(ex);
                jr.Data = new { code = 0, msg = ex.ToString() };
            }
            return jr;
        }

        [HttpPost]
        public JsonResult BuildIntentUri(IEnumerable<data> list)
        {
            JsonResult jr = new JsonResult();
            string appactivity = Request.Form["appactivity"];
            if (string.IsNullOrWhiteSpace(appactivity))
            {
                jr.Data = new { code = 0, msg = "app activity 必填" };
            }
            string extra = "";
            if (list != null && list.Count() > 0)
            {
                extra = string.Join(";", list.Where(x => !string.IsNullOrEmpty(x.key)).Distinct().Select(x => $"S.{x.key}={x.value}"));
                if (!string.IsNullOrEmpty(extra))
                {
                    extra += ";";
                }
            }
            string intenturi =
                $"intent:#Intent;component=cn.TuHu.android/{appactivity};{extra}end";
            jr.Data = new { code = 1, msg = intenturi };
            return jr;
        }

        public class data
        {
            public string key { get; set; }
            public string value { get; set; }
        }

        [PowerManage]
        public ActionResult SinglePush()
        {
            return View();
        }


        public async Task<JsonResult> QuerySinglePushTemplateAsync(TemplateQuery query)
        {
            JsonResult jr = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            query.PageIndex = query.PageIndex == 0 ? 1 : query.PageIndex;
            query.PageSize = 50;
            query.SendTypes = new List<SendType>() { SendType.Single };
            var datas = await PushManagerNew.SelectPushTemplatesAsync(query);
            if (datas != null)
            {
                jr.Data = new
                {
                    code = 1,
                    Pager = new
                    {
                        datas.PageIndex,
                        datas.PageSize,
                        datas.TotalCount,
                        datas.PageCount
                    },
                    Templates = datas.Result.Select(x => new
                    {
                        x.PKID,
                        x.BatchID,
                        x.PlanName,
                        x.PushStatus,
                        x.Title,
                        x.TemplateTitle,
                        DeviceType = x.DeviceType.ToString(),
                        SendType = x.SendType.ToString(),
                        CreateTime = x.CreateTime.ToString(),
                        TemplateExpireTime = x.TemplateExpireTime.ToString(),
                        x.CreateUser,
                        IsEnable = x.PushStatus != PushStatus.Suspend
                    })
                };
            }
            else
            {

            }
            return jr;
        }

        /// <summary>
        /// 复制【推送模板】
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CopySinglePushTemplateAsync(int? batchid)
        {
            if (!batchid.HasValue || batchid.Value <= 0)
            {
                return new JsonResult() { Data = new { code = 0, msg = "复制失败【未能获取复制项】" } };
            }
            JsonResult jr = new JsonResult();
            try
            {
                var getPushTemplatesList = await PushManagerNew.SelectPushTemplatesByBatchIDAsync(batchid.Value);
                if (getPushTemplatesList == null || !getPushTemplatesList.Any())
                {
                    jr.Data = new { code = 0, msg = "复制失败【数据不存在】" };
                    return jr;
                }

                var newBatchId = 0;
                int templateTage = 0;
                TemplateType? templateType = null;
                foreach (var item in getPushTemplatesList)
                {
                    if (item.MessageBoxShowType == MessageBoxShowType.OrderText)
                    {
#if !DEBUG
                string loginuser = HttpContext.User.Identity.Name;
                if (string.IsNullOrEmpty(loginuser) || (!loginuser.Contains("dongjing1") && !loginuser.Contains("cheliangliang") && !loginuser.Contains("devteam")))
                {
                    jr.Data = new { code = 0, msg = "你没有权限" };
                    return jr;
                }
#endif
                    }

                    string userNo = HttpContext.User.Identity.Name;
                    item.PlanName = item.PlanName + "(复制)";
                    item.Sendtime = DateTime.Now;
                    item.CreateTime = DateTime.Now;
                    item.CreateUser = userNo;
                    item.PushStatus = PushStatus.Prepare;
                    item.PKID = 0;
                    item.BatchID = newBatchId;
                    var addTemplateIdAndBatchId = await PushManagerNew.CreateOrUpdateTemplateAsync(item);
                    if (newBatchId == 0)
                    {
                        newBatchId = addTemplateIdAndBatchId;
                        templateTage = item.TemplateTag;
                        templateType = item.TemplateType;
                    }
                    if (addTemplateIdAndBatchId > 0)
                    {
                        //设置BatchId等于PKID 新增
                        await PushManagerNew.UpdateTemplateBatchIDAsync(addTemplateIdAndBatchId, newBatchId);
                    }
                }
                //更新缓存
                if (newBatchId > 0)
                {
                    //更新TemplateTag
                    await PushManagerNew.UpdateTemplateTagByBatchIdAsync(newBatchId, templateTage);
                    //更新TpyeBy
                    if (templateType.HasValue)
                    {
                        await PushManagerNew.UpdateTemplateTypeByBatchIdAsync(newBatchId, templateType.Value);
                    }

                    //更新缓存
                    var templateIds = await PushManagerNew.SelectTemplateIdsByBatchId(newBatchId);
                    using (var client = new Tuhu.Service.Push.CacheClient())
                    {
                        var result = await client.RemoveRedisCacheKeyAsync("Push", $"SelectTemplateByBatchID/{newBatchId}");
                        result.ThrowIfException(true);
                        await Task.WhenAll(templateIds.Select(t =>
                            client.RemoveRedisCacheKeyAsync("Push", $"SelectTemplateByPKID/{t}")));
                    }
                }
                jr.Data = new { code = newBatchId > 0 ? 1 : 0, msg = newBatchId > 0 ? "复制成功" : "复制失败" };
            }
            catch (Exception e)
            {
                jr.Data = new { code = 0, msg = "复制异常" };
            }
            return jr;
        }


        [PowerManage]
        public ActionResult EditSinglePushTemplate(int? batchid)
        {
            ViewBag.batchid = batchid;
            return View();
        }

        public async Task<JsonResult> SelectTemplatePlanInfoAsync(int batchid)
        {
            JsonResult jr = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var datas = await PushManagerNew.SelectPushTemplatesByBatchIDAsync(batchid);
            var planname = datas.FirstOrDefault(x => !string.IsNullOrEmpty(x?.PlanName))?.PlanName;
            var planstatus = datas.FirstOrDefault(x => x != null)?.PushStatus;
            var templatetype = datas.FirstOrDefault(x => x != null)?.TemplateType;
            var templateExpireTime = datas.FirstOrDefault(x => x != null)?.TemplateExpireTime.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(planname))
            {
                planname = "计划名称";
            }
            jr.Data = new
            {
                code = 1,
                planname,
                templatetype,
                templateExpireTime,
                planstatus = planstatus.HasValue ? PushManagerNew.GetPushStatusName(planstatus.Value) : ""
            };
            return jr;
        }

        public JsonResult SaveTemplatePlanInfo(int batchid, string planname, IEnumerable<ExtraKey> templatetitles)
        {
            JsonResult jr = new JsonResult();

            return jr;
        }


        [HttpPost]
        public async Task<JsonResult> SubmitSingleTemplateAsync(SinglePushSubmitModel model, string ispreview,
            string targets, string planname, string TemplateTag, TemplateType? TemplateType, int? wxPriority, int? appPriority)
        {
            JsonResult jr = new JsonResult();

            var expiretime = ConvertExpire(model.ExpireDay + "/" + model.ExpireHour + "/" + model.ExpireMinute);
            //if (expiretime.HasValue && expiretime.Value > ConvertExpire("14/0/0"))
            //{
            //    jr.Data = new { code = 0, msg = "消息过期周期过大" };
            //    return jr;
            //}
            if (!expiretime.HasValue || expiretime == 0)
            {
                expiretime = ConvertExpire("14/0/0");

            }
            Func<SinglePushSubmitModel, PushTemplate> convertToTemplate = m =>
            {
                string userNo = HttpContext.User.Identity.Name;
                var template = new PushTemplate()
                {
                    PKID = m.PKID,
                    BatchID = m.BatchID,
                    Bdage = m.Bdage,
                    Description = "",
                    Title = m.Title,
                    SubTitle = m.SubTitle,
                    Content = m.Content,
                    Sendtime = DateTime.Now,
                    ImageUrl = m.ImageUrl,
                    SendType = SendType.Single,
                    SoundType = m.SoundType,
                    SourceChannel = SourceChannel.Setting,
                    PushStatus = PushStatus.Prepare,
                    CreateTime = DateTime.Now,
                    CreateUser = userNo,
                    AppActivity = m.AppActivity?.Trim()?.Replace("\n", "")?.Replace("\t", ""),
                    AppExpireTime = expiretime,
                    DeviceType = m.DeviceType,
                    MessageType = m.MessageType,
                    PlanName = m.PlanName,
                    TemplateTitle = m.Title,
                    WxTemplateID = m.WxTemplateID,
                    RouterUrl = m.AppActivity,
                    BoxID = m.BoxID,
                    MessageBoxShowType = m.MessageBoxShowType,
                    MessageNavigationTypeId = m.MessageNavigationTypeId,
                    TemplateType = TemplateType ??  Service.PushApi.Models.Push.TemplateType.System,
                    BaiduTemplateId=m.BaiduTemplateID,
                    TemplateExpireTime=m.TemplateExpireTime
                };
                if (string.Equals(m.SoundType, "default"))
                {
                    template.SoundType = "default";
                }
                else
                {
                    template.SoundType = "";
                }
                Dictionary<string, string> ParameterDescDict = new Dictionary<string, string>();
                if (m.ContentDesc != null && m.ContentDesc.Any())
                {
                    ParameterDescDict["content"] =
                        JsonConvert.SerializeObject(m.ContentDesc.ToDictionary(x => x.Key, x => x.Value));
                }
                if (m.KeyValueDesc != null && m.KeyValueDesc.Any())
                {
                    ParameterDescDict["keyvalue"] =
                        JsonConvert.SerializeObject(m.KeyValueDesc.ToDictionary(x => x.Key, x => x.Value));
                }
                template.ParameterDescDict = ParameterDescDict;
                Dictionary<string, string> ExtraDict = new Dictionary<string, string>();
                if (string.IsNullOrEmpty(template.PlanName))
                {
                    template.PlanName = template.Title;
                }
                if (m.DeviceType == DeviceType.MessageBox)
                {
                    var tempdict = model.ExtraDict.Distinct(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                    if (tempdict.Any())
                    {
                        ExtraDict["ios"] = JsonConvert.SerializeObject(tempdict);
                        ExtraDict["android"] = JsonConvert.SerializeObject(tempdict);
                    }
                }
                else
                {
                    foreach (var extraKey in m.ExtraDict)
                    {
                        ExtraDict[extraKey.Key] = extraKey.Value;
                    }
                }
                if (model.DeviceType == DeviceType.Android)
                {
                    ExtraDict["showDialog"] = "show";
                    //if (model.Sound != 0)
                    //{
                    //    ExtraDict["sound"] = "1";
                    //}
                    if (model.Shake != 0)
                    {
                        ExtraDict["shake"] = "1";
                    }
                    else
                    {
                        ExtraDict["shake"] = "0";
                    }

                    if (model.Light != 0)
                    {
                        ExtraDict["light"] = "1";
                    }
                    else
                    {
                        ExtraDict["light"] = "0";
                    }

                    if (model.Front != 0)
                    {
                        ExtraDict["notify_foreground"] = "1";
                    }
                    else
                    {
                        ExtraDict["notify_foreground"] = "0";
                    }

                    if (model.Wifi != 0)
                    {
                        ExtraDict["wifi"] = "1";
                    }
                    else
                    {
                        ExtraDict["wifi"] = "0";
                    }
                }
                if (model.DeviceType == DeviceType.WeChat)
                {
                    #region 微信图文配置
                    //填写微信图文消息ID
                    if (!string.IsNullOrEmpty(model.WeixinMediaId))
                    {
                        ExtraDict["WeixinMediaId"] = model.WeixinMediaId;
                    }

                    //勾选 近期积极互动用户
                    if (model.IsUseCurrentActiveUser)
                    {
                        ExtraDict["IsUseCurrentActiveUser"] = "1";
                        ExtraDict["CurrentActiveUserOptionItem"] = model.CurrentActiveUserOptionItem.ToString();
                    }

                    //勾选其余用户
                    if (model.IsUseOtherUser)
                    {
                        ExtraDict["IsUseOtherUser"] = "1";
                    }
                    #endregion

                    #region 微信小程序推送配置

                    if (!string.IsNullOrEmpty(model.MiniProgramAppId))
                    {
                        ExtraDict["MiniProgramAppId"] = model.MiniProgramAppId;
                    }
                    if (!string.IsNullOrEmpty(model.MiniProgramPagePath))
                    {
                        ExtraDict["MiniProgramPagePath"] = model.MiniProgramPagePath;
                    }
                    if (!string.IsNullOrEmpty(model.MiniProgramThumbMediaId))
                    {
                        ExtraDict["MiniProgramThumbMediaId"] = model.MiniProgramThumbMediaId;
                    }
                    if (!string.IsNullOrEmpty(model.MiniProgramTitle))
                    {
                        ExtraDict["MiniProgramTitle"] = model.MiniProgramTitle;
                    }

                    #endregion

                }
                template.ExtraDict = ExtraDict;
                if (string.Equals(m.IsEnable, "0"))
                {
                    template.PushStatus = PushStatus.Suspend;
                }
                else
                {
                    template.PushStatus = PushStatus.Prepare;
                }
                return template;
            };
            var pushtemplate = convertToTemplate(model);
            if (pushtemplate.ExtraDict.Count >= 10)
            {
                jr.Data = new { code = 0, msg = "键值对过多" };
                return jr;
            }
            //if (string.IsNullOrEmpty(pushtemplate.Title))
            //{
            //    jr.Data = new { code = 0, msg = "标题不能为空" };
            //    return jr;
            //}
            //if (string.IsNullOrEmpty(pushtemplate.Content))
            //{
            //    jr.Data = new { code = 0, msg = "内容不能为空" };
            //    return jr;
            //}

            if (pushtemplate.MessageBoxShowType == MessageBoxShowType.OrderText)
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<OrderMessageBoxInfo>(pushtemplate.Content);
                }
                catch (System.Exception ex)
                {
                    jr.Data = new
                    {
                        code = 0,
                        msg = "推送类型为订单类型,消息内容不合法"
                    };
                    return jr;
                }
#if !DEBUG
                string loginuser = HttpContext.User.Identity.Name;
                if (string.IsNullOrEmpty(loginuser) || (!loginuser.Contains("dongjing1") && !loginuser.Contains("cheliangliang") && !loginuser.Contains("devteam")))
                {
                    jr.Data = new { code = 0, msg = "你没有权限" };
                    return jr;
                }
#endif
            }

            if (string.IsNullOrEmpty(pushtemplate.Title) && pushtemplate.MessageBoxShowType != MessageBoxShowType.ActivityText && pushtemplate.MessageBoxShowType != MessageBoxShowType.BigImageText && pushtemplate.MessageBoxShowType != MessageBoxShowType.SmallImageText)
            {
                jr.Data = new { code = 0, msg = "非互动消息文字类型标题不能为空" };
                return jr;
            }

            if (model.WxTemplateColors != null && model.WxTemplateColors.Any())
            {
                pushtemplate.WxTemplateColorDict = model.WxTemplateColors.ToDictionary(x => x.Key, x => x.Value);
            }
            pushtemplate.WxAppEmKeyWord = model.WxAppEmKeyWord;
            if (pushtemplate.DeviceType == DeviceType.MessageBox && pushtemplate.BatchID != 0 &&
                pushtemplate.MessageBoxShowType != MessageBoxShowType.Text)
            {
                var imageconfigs = await PushManagerNew.SelectImageTextByBatchIdAsync(pushtemplate.BatchID);
                if (imageconfigs != null && imageconfigs.Any())
                {
                    var temp = imageconfigs.OrderBy(x => x.Order).FirstOrDefault();
                    if (temp != null)
                    {
                        pushtemplate.Title = temp.Title;
                        pushtemplate.Content = temp.Desctiption;
                        pushtemplate.RouterUrl = temp.JumpUrl;
                        pushtemplate.AppActivity = temp.JumpUrl;
                    }
                }
            }
            if (string.Equals(ispreview, "1"))
            {
                pushtemplate.IsPreview = true;
                if (string.IsNullOrEmpty(targets))
                {
                    jr.Data = new { code = 0, msg = "未填写推送目标" };
                    return jr;
                }
                var issuccess = true;
                if (pushtemplate.DeviceType == DeviceType.WeChat || pushtemplate.DeviceType == DeviceType.WeiXinApp)
                {
                    using (var client = new Tuhu.Service.Push.WeiXinPushClient())
                    {
                   
                        var tasks = targets.Split(',')
                             .Select(x => client.PushByTemplateAsync(pushtemplate.Cast<Tuhu.Service.Push.Models.Push.PushTemplate>(), new Tuhu.Service.Push.Models.Push.PushTemplateLog()
                             {
                                 Target = x
                             }));
                        var ss = await Task.WhenAll(tasks);
                        ss?.ToList().ForEach(x => issuccess &= x.Result);
                    }
                }
                else
                {
                    issuccess = await PushManagerNew.PushByAliasAsync(targets.Split(','), pushtemplate);
                }
                jr.Data = new { code = issuccess ? 1 : 0, msg = issuccess ? "推送成功" : "推送失败" };
                return jr;
            }
            else
            {
                int batchid = model.BatchID;
                pushtemplate.SendType = SendType.Single;
                if (pushtemplate.PKID != 0)
                {
                    var result = await PushManagerNew.CreateOrUpdateTemplateAsync(pushtemplate);
                    jr.Data = new { code = result > 0 ? 1 : 0, msg = result > 0 ? "更新成功" : "更新失败" };

                }
                else
                {
                    int pkid = 0;
                    var result = await PushManagerNew.CreateOrUpdateTemplateAsync(pushtemplate);
                    pkid = result;
                    batchid = result;
                    if (model.BatchID != 0)
                    {
                        batchid = model.BatchID;
                    }
                    if (result > 0)
                    {
                        await PushManagerNew.UpdateTemplateBatchIDAsync(result, batchid);
                    }
                    jr.Data = new { code = result > 0 ? 1 : 0, msg = result > 0 ? "保存成功" : "保存失败", batchid = batchid, pkid = result > 0 ? pkid : 0 };
                }
                if (batchid != 0)
                {
                    //统一时间，该方法增加
                    await PushManagerNew.UpdateTemplatePlanInfoAsync(batchid, planname,model.TemplateExpireTime, new Dictionary<int, PushStatus>());
                    var isUpdateBatchInfo = false;
                    int tagtemp;
                    if (int.TryParse(TemplateTag, out tagtemp))
                    {
                        await PushManagerNew.UpdateTemplateTagByBatchIdAsync(batchid, tagtemp);
                        isUpdateBatchInfo = true;
                    }
                    if (TemplateType.HasValue)
                    {
                        await PushManagerNew.UpdateTemplateTypeByBatchIdAsync(batchid, TemplateType.Value);
                        isUpdateBatchInfo = true;
                    }

                    if (wxPriority.HasValue || appPriority.HasValue)
                    {
                        await PushManagerNew.UpdateTemplatePriorityByBatchIdAsync(batchid, wxPriority, appPriority);
                        isUpdateBatchInfo = true;
                    }

                    if (isUpdateBatchInfo)
                    {
                        var templateIds =
                            await PushManagerNew.SelectTemplateIdsByBatchId(batchid);
                        using (var client = new Tuhu.Service.Push.CacheClient())
                        {
                            var result = await client.RemoveRedisCacheKeyAsync("Push", $"SelectTemplateByBatchID/{batchid}");
                            result.ThrowIfException(true);
                            await Task.WhenAll(templateIds.Select(t =>
                                client.RemoveRedisCacheKeyAsync("Push", $"SelectTemplateByPKID/{t}")));
                        }
                    }
                }

                return jr;
            }
        }


        [HttpGet]
        public async Task<JsonResult> SelectAllWxAppTemplatesAsync(int platform)
        {
            var result = await PushManagerNew.SelectAllWxAppTemplatesAsync(platform);
            JsonResult jr = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = result
            };
            return jr;
        }

        [HttpGet]
        public async Task<JsonResult> SelectAllWxAppConfigAsync()
        {
            var result = (await PushManagerNew.SelectAllWxAppConfigAsync())?.ToList();
            result?.Add(new Service.Push.Models.WeiXinPush.WxConfig()
            {
                platform = -1,
                name = "跳转H5"
            });
            JsonResult jr = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = result
            };
            return jr;
        }

        public async Task<ActionResult> ShowTemplateLog(int templateid)
        {
            var result = await PushManagerNew.SelectTemplateModifyLogs(templateid);
            return View(result);
        }

        public async Task<ActionResult> AllTemplateLog()
        {
            await Task.FromResult(0);

            return View();
        }


        public async Task<JsonResult> QueryTemplateModifyLogsAsync(TemplateLogQueryInfo query)
        {
            JsonResult jr = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            query.PageIndex = query.PageIndex == 0 ? 1 : query.PageIndex;
            query.PageSize = query.PageSize == 0 ? 50 : query.PageSize;
            var datas = await PushManagerNew.SelectPushTemplateModifyLogsAsync(query);
            if (datas != null)
            {
                jr.Data = new
                {
                    code = 1,
                    Pager = new
                    {
                        datas.PageIndex,
                        datas.PageSize,
                        datas.TotalCount,
                        datas.PageCount
                    },
                    TemplateModifyLogs = datas.Result.Select(x => new
                    {
                        DeviceType = x.DeviceType.ToString(),
                        x.PKID,
                        x.TemplateId,
                        x.TemplateTitle,
                        x.PushPlanId,
                        x.PushPlanTitle,
                        x.IsEnable,
                        x.OriginTemplate,
                        x.NewTemplate,
                        x.ModifyUser,
                        LastUpdateDateTime = x.LastUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                };
            }
            return jr;
        }

        [HttpGet]
        public async Task<ActionResult> ExportTemplateModifyLogsFileAsync(TemplateLogQueryInfo query)
        {
            var datas = await PushManagerNew.GetExportTemplateModifyLogDatas(query);
            var result = PushManagerNew.GetExportTemplateModifyLogStreams(datas);
            return File(result, "text/comma-separated-values", "ExportLogs.csv");
        }

        public async Task<JsonResult> SelectTemplatesByBatchIDAsync(int? batchid)
        {
            JsonResult jr = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            List<PushTemplate> datas = new List<PushTemplate>();
            string templatetag = "";
            if (batchid.HasValue)
            {
                datas = await PushManagerNew.SelectPushTemplatesByBatchIDAsync(batchid.Value);
                templatetag = (await PushManagerNew.SelectTemplateTagByBatchIdAsync(batchid.Value))?.ToString();
            }

            if (datas.Count != 6)
            {
                var devicevalues = new List<DeviceType>()
                {
                    DeviceType.iOS,
                    DeviceType.Android,
                    DeviceType.MessageBox,
                    DeviceType.WeChat,
                    DeviceType.WeiXinApp,
                    DeviceType.BaiduApp
                };
                var devicetypes = datas.Select(x => x.DeviceType);
                var notexists = devicevalues.Except(devicetypes);
                if (notexists.Any())
                {
                    datas.AddRange(notexists.Select(x => new PushTemplate()
                    {
                        DeviceType = x,
                        SoundType = "default"
                    }));
                }
                datas = datas.GroupBy(x => x.DeviceType).Select(x => x.First()).ToList();

            }

            int platform = 5;

            var wxapptemplate = datas.FirstOrDefault(x => x.DeviceType == DeviceType.WeiXinApp);

            if (wxapptemplate != null && wxapptemplate.BoxID.HasValue)
            {
                platform = wxapptemplate.BoxID.Value;
            }
            // 获取微信App模板列表
            var allwxapptemplates = await PushManagerNew.SelectAllWxAppTemplatesAsync(platform);

            //获取微信模板列表
            var allwxtemplates = await PushManagerNew.SelectAllWxTemplateAsync();

            //获取所有的百度模板ID 
            var allBaiduTemplates = await PushManagerNew.SelectAllBaiduTemplateAsync();

            //获取所有的模板标签
            var allTemplateTags = await SelectAllTemplateTagsAsync();

            //获取所有模板的类型
            var templateTypes = Enum.GetValues(typeof(TemplateType)).Cast<TemplateType>().ToList();

            var planbatchid = datas.FirstOrDefault(x => x.BatchID != 0)?.BatchID;
             
            List<string> passextra = new List<string>() { "wifi", "sound", "shake", "light", "front", "notify_foreground", "showDialog" };
            List<string> passWeChatExtra = new List<string>() { "WeixinMediaId", "IsUseCurrentActiveUser", "IsUseOtherUser", "CurrentActiveUserOptionItem" };
            List<string> passWeChatMiniProgramExtra = new List<string>() { "MiniProgramAppId", "MiniProgramPagePath", "MiniProgramThumbMediaId", "MiniProgramTitle" };

            Func<long?, string> getexpiretimestr = t =>
            {
                if (t.HasValue)
                {
                    var temp = new DateTime();
                    DateTime d = new DateTime(t.Value * 10000);
                    var span = d - temp;

                    string s = string.Format("{0}/{1}/{2}", span.Days, span.Hours, span.Minutes);
                    return s;
                }
                return "";
            };

            //处理扩展字典
            Func<PushTemplate, List<KeyValuePair<string, string>>> HandleExtraDict = t =>
              {
                  if (t.DeviceType == DeviceType.Android)
                  {
                      var temp = t.ExtraDict.ToList();
                      temp.RemoveAll(x => passextra.Contains(x.Key));
                      return temp;
                  }

                  if (t.DeviceType == DeviceType.WeChat)
                  {
                      var temp = t.ExtraDict.ToList();

                      //图文消息推送
                      temp.RemoveAll(x => passWeChatExtra.Contains(x.Key));

                      //小程序卡片推送
                      temp.RemoveAll(x => passWeChatMiniProgramExtra.Contains(x.Key));
                      return temp;
                  }

                  return t.ExtraDict.ToList();
              };

            //获取模板的第一个ID【微信，微信APP,百度】
            Func<PushTemplate, string> GetTemplateID = t =>
           {
               if (t.DeviceType == DeviceType.WeChat || t.DeviceType == DeviceType.WeiXinApp)
               {
                   if (string.IsNullOrEmpty(t.WxTemplateID))
                   {
                       if (t.DeviceType == DeviceType.WeChat)
                       {
                           return allwxtemplates.FirstOrDefault()?.template_id;
                       }
                       if (t.DeviceType == DeviceType.WeiXinApp)
                       {
                           return allwxapptemplates.FirstOrDefault()?.template_id;
                       }
                   }
                   else
                   {
                       return t.WxTemplateID;
                   }
               }

               if (t.DeviceType == DeviceType.BaiduApp)
               {
                   if (string.IsNullOrEmpty(t.BaiduTemplateId))
                   {
                       return allBaiduTemplates.FirstOrDefault()?.template_id;
                   }
                   else
                   {
                       return t.BaiduTemplateId;
                   }
               }

               return string.Empty;
           };


            //获取模板名称
            Func<TemplateType, string> GetTemplateName = t =>
            {
                switch (t)
                {
                    case TemplateType.None:
                        return "未选择";
                    case TemplateType.System:
                        return "系统";
                    case TemplateType.Marketing:
                        return "营销";
                    case TemplateType.SystemMarketing:
                        return "系统(营销)";
                    default:
                        return t.ToString();
                }
            };

            jr.Data = new
            {
                code = 1,
                batchid = planbatchid ?? 0, 
                TemplateTag = templatetag,
                AllTemplateTags = allTemplateTags,
                WeiXinTemplates = allwxtemplates,
                BaiduTemplates = allBaiduTemplates,

                TemplateTypes = templateTypes.Select(x => new
                {
                    Name = GetTemplateName(x),
                    Value = (int)x
                }),

                datas = datas.OrderBy(x => x.DeviceType).Select(x => new
                {
                    x.PKID,
                    BatchID = planbatchid ?? 0,
                    Sound = string.IsNullOrEmpty(x.SoundType) ? "0" : x.SoundType,
                    Shake = (!x.ExtraDict.ContainsKey("shake")) || (x.ExtraDict.ContainsKey("shake") && x.ExtraDict["shake"] == "1") ? 1 : 0,
                    Light = (!x.ExtraDict.ContainsKey("light")) || (x.ExtraDict.ContainsKey("light") && x.ExtraDict["light"] == "1") ? 1 : 0,
                    Front = (!x.ExtraDict.ContainsKey("notify_foreground")) || (x.ExtraDict.ContainsKey("notify_foreground") && x.ExtraDict["notify_foreground"] == "1") ? 1 : 0,
                    Wifi = (!x.ExtraDict.ContainsKey("wifi")) || (x.ExtraDict.ContainsKey("wifi") && x.ExtraDict["wifi"] == "1") ? 1 : 0,

                    //微信公众号推送策略
                    IsUseCurrentActiveUser = x.ExtraDict.ContainsKey("IsUseCurrentActiveUser"),
                    IsUseOtherUser = x.ExtraDict.ContainsKey("IsUseOtherUser"),
                    CurrentActiveUserOptionItem = (x.ExtraDict.ContainsKey("CurrentActiveUserOptionItem")) ? x.ExtraDict["CurrentActiveUserOptionItem"] : "",
                    WeixinMediaId = (x.ExtraDict.ContainsKey("WeixinMediaId")) ? x.ExtraDict["WeixinMediaId"] : "",

                    //微信小程序卡片推送
                    MiniProgramAppId = (x.ExtraDict.ContainsKey("MiniProgramAppId")) ? x.ExtraDict["MiniProgramAppId"] : "",
                    MiniProgramPagePath = (x.ExtraDict.ContainsKey("MiniProgramPagePath")) ? x.ExtraDict["MiniProgramPagePath"] : "",
                    MiniProgramThumbMediaId = (x.ExtraDict.ContainsKey("MiniProgramThumbMediaId")) ? x.ExtraDict["MiniProgramThumbMediaId"] : "",
                    MiniProgramTitle = (x.ExtraDict.ContainsKey("MiniProgramTitle")) ? x.ExtraDict["MiniProgramTitle"] : "",

                    x.DeviceType,
                    x.Title,
                    x.SubTitle,
                    TemplateTitle = x.Title,
                    x.Bdage,
                    x.PushStatus,
                    x.Content,
                    ExtraDict = HandleExtraDict(x),
                    x.AppActivity,
                    x.AppExpireTime,
                    x.SoundType,
                    x.ImageUrl,
                    x.PlanName,
                    x.BoxID,
                    TemplateExpireTime=x.TemplateExpireTime.ToString("yyyy-MM-dd"),
                    MessageBoxShowType = x.MessageBoxShowType.ToString(),
                    MessageNavigationTypeId = x.MessageNavigationTypeId ?? 1,
                    IsEnable = x.PushStatus == PushStatus.Suspend ? 0 : 1,
                    ContentDesc = x.ParameterDescDict.ContainsKey("content") ?
                      JsonConvert.DeserializeObject<Dictionary<string, string>>(x.ParameterDescDict["content"]).ToList() : new Dictionary<string, string>().ToList(),
                    KeyValueDesc = x.ParameterDescDict.ContainsKey("keyvalue") ?
                          JsonConvert.DeserializeObject<Dictionary<string, string>>(x.ParameterDescDict["keyvalue"]).ToList() : new Dictionary<string, string>().ToList(),
                    ExpireDay = x.AppExpireTime.HasValue ? getexpiretimestr(x.AppExpireTime).Split('/')[0] : "",
                    ExpireHour = x.AppExpireTime.HasValue ? getexpiretimestr(x.AppExpireTime).Split('/')[1] : "",
                    ExpireMinute = x.AppExpireTime.HasValue ? getexpiretimestr(x.AppExpireTime).Split('/')[2] : "",
                    MessageType = x.DeviceType == DeviceType.MessageBox ? (x.MessageType.HasValue ? x.MessageType.Value : MessageType.Activity) : x.MessageType,
                    ShowBdage = x.Bdage.HasValue ? 1 : 0,
                    WxTemplateColorDict = x.WxTemplateColorDict.ToList(),
                    x.WxAppEmKeyWord,
                    x.TemplateType,
                    WxTemplateID = GetTemplateID(x),//x.DeviceType != DeviceType.WeChat ? string.Empty : (!string.IsNullOrEmpty(x.WxTemplateID) ? x.WxTemplateID : allwxtemplates.FirstOrDefault()?.template_id)
                    BaiduTemplateID= GetTemplateID(x)
                })
            };
            return jr;
        }

        public async Task<JsonResult> TestAsync()
        {
            var result = await SelectAllTemplateTagsAsync();
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = result
            };
        }


        public static async Task<List<Tuhu.Service.MemberGroup.Models.Tag.TagInfo.TagModel>> SelectAllTemplateTagsAsync()
        {
            using (var client = new UserFilterClient())
            {
                var result = await client.GetTagsAsync(TagTypeEnum.PushTemplate);
                result.ThrowIfException(true);
                return result.Result;
            }
        }
        public async Task<ActionResult> SelectUserAuthInfoByMobileNumberAsync(string number)
        {
            ViewBag.number = number;
            if (!string.IsNullOrEmpty(number))
            {
                var userid = await PushManagerNew.SelectUserIDByPhoneNumberAsync(number);
                if (userid.HasValue)
                {
                    var configs = (await PushManagerNew.SelectAllWxAppConfigAsync())?.ToList();
                    ViewBag.configs = configs;
                    var queryresult = await Task.WhenAll(PushManagerNew.SelectUserAuthInfoAsync(userid.Value), PushManagerNew.SelectWxOpenUserAuthInfoAsync(userid.Value));
                    List<UserAuthInfo> list = new List<UserAuthInfo>();
                    foreach (var item in queryresult)
                    {
                        if (item != null && item.Any())
                        {
                            list.AddRange(item);
                        }
                    }
                    return View(list);
                }
                else
                {
                    ModelState.AddModelError("", "该手机号未找到用户信息");
                    return View();
                }
            }
            return View();
        }

        public async Task<ActionResult> SelectFormIdByOpenId(string openid, int count = 0)
        {
            count = count == 0 ? 10 : count;
            ViewBag.openid = openid;
            ViewBag.count = count;
            var configs = (await PushManagerNew.SelectAllWxAppConfigAsync())?.ToList();
            ViewBag.configs = configs;
            if (!string.IsNullOrEmpty(openid))
            {
                var results = await PushManagerNew.SelectWxAppPushTokensByOpenIdAsync(openid, count);
                return View(results);
            }
            return View();
        }

        [PowerManage]
        public ActionResult MessageNavigationConfig()
        {

            return View();
        }
        [HttpGet]
        public async Task<JsonResult> SelectMessageNavigationTypesAsync()
        {
            var models = await PushManagerNew.SelectMessageNavigationTypesAsync();
            //return Json(models.OrderBy(x => x.Order), JsonRequestBehavior.AllowGet);
            return Json(models.Where(o => !o.NavigationName.Contains("时间设置")).OrderBy(x => x.Order), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public async Task<JsonResult> DeleteMessageNavigationTypeAsync(int pkid)
        {
            var result = await PushManagerNew.DeleteMessageNavigationTypeAsync(pkid);
            return Json(result ? 1 : 0);
        }
        public async Task<JsonResult> SaveNavigationTypeConfigAsync(MessageNavigationType config)
        {
            var result = false;


            try
            {
                if (string.IsNullOrEmpty(config.NavigationName) || string.IsNullOrEmpty(config.TopNavigationName))
                {
                    throw new Exception("导航名称不能为空");
                }

                if (config.NavigationName.Length > 4)
                {
                    throw new Exception("列表导航名称最多4个字");
                }
                if (config.TopNavigationName.Length > 2)
                {
                    throw new Exception("顶部导航名称最多2个字");
                }

                var regex = new Regex(@"^[a-zA-Z0-9]+$");
                if (!regex.Match(config.PushAlias).Success)
                {
                    throw new Exception("推送别名只能是英文字母或数字");
                }
                var checkresult = await PushManagerNew.CheckNavigationOrderAsync(config.Order, config.PkId);
                if (checkresult > 0)
                {
                    throw new Exception("顺序不能重复");
                }
                if (config.PkId != 0)
                {
                    result = await PushManagerNew.UpdateMessageNavigationTypeAsync(config);
                }
                else
                {
                    result = await PushManagerNew.LogMessageNavigationTypeAsync(config);
                }
                return Json(new
                {
                    code = result ? 1 : 0,
                    data = result ? 1 : 0,
                    msg = result ? "保存成功" : "保存失败"
                });
            }
            catch (System.Exception ex)
            {
                return Json(new
                {
                    code = 0,
                    data = 0,
                    msg = ex.Message
                });
            }

        }

        public async Task<JsonResult> SelectImageTextByBatchIdAsync(int batchid)
        {
            var result = await PushManagerNew.SelectImageTextByBatchIdAsync(batchid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> LogImageTextConfigAsync(MessageImageText config, MessageBoxShowType showtype)
        {
            try
            {
                if (string.IsNullOrEmpty(config.JumpUrl))
                {
                    throw new Exception("跳转地址不能为空");
                }
                if (string.IsNullOrEmpty(config.Title))
                {
                    throw new Exception("标题不能为空");
                }
                if (string.IsNullOrEmpty(config.ImageUrl))
                {
                    throw new Exception("图片不能为空");
                }
                if (await PushManagerNew.CheckMessageImageTextOrderAsync(config.BatchId, config.Order, config.Pkid))
                {
                    throw new Exception("顺序不能重复");
                }
                var result = await PushManagerNew.SubmitMessageImageTextAsync(config);
                if (result)
                {
                    var templates = await PushManagerNew.SelectPushTemplatesByBatchIDAsync(config.BatchId);
                    var imageconfigs = await PushManagerNew.SelectImageTextByBatchIdAsync(config.BatchId);

                    if (templates != null && templates.Any() && imageconfigs != null && imageconfigs.Any())
                    {
                        var temp = imageconfigs.OrderBy(x => x.Order).FirstOrDefault();
                        var messagebox = templates?.FirstOrDefault(x => x.DeviceType == DeviceType.MessageBox);
                        if (messagebox != null)
                        {
                            messagebox.Title = temp.Title;
                            messagebox.Content = temp.Desctiption;
                            messagebox.RouterUrl = temp.JumpUrl;
                            messagebox.AppActivity = temp.JumpUrl;
                            messagebox.MessageBoxShowType = showtype;
                            await PushManagerNew.CreateOrUpdateTemplateAsync(messagebox);
                        }
                    }
                }
                return Json(new
                {
                    code = result ? 1 : 0,
                    data = result ? 1 : 0,
                    msg = result ? "保存成功" : "保存失败"
                });
            }
            catch (System.Exception ex)
            {
                return Json(new
                {
                    code = 0,
                    data = 0,
                    msg = ex.Message
                });
            }
        }

        public async Task<JsonResult> DeleteImageTextConfigAsync(int pkid)
        {
            try
            {
                var result = await PushManagerNew.DeleteMessageImageTextAsync(pkid);
                return Json(new
                {
                    code = result ? 1 : 0,
                    data = result ? 1 : 0,
                    msg = result ? "删除成功" : "删除失败"
                });
            }
            catch (System.Exception ex)
            {
                return Json(new
                {
                    code = 0,
                    data = 0,
                    msg = ex.Message
                });

            }
        }
    }
}