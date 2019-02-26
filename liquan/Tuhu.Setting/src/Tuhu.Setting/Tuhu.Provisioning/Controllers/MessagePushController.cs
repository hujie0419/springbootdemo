using Qiniu.IO;
using Qiniu.RS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.Framework.FileUpload;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.ShopsManagement;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using swc = System.Web.Configuration;

namespace Tuhu.Provisioning.Controllers
{

    public class MessagePushController : Controller
	{
		private readonly IMessagePushManager manager = new MessagePushManager();
		[PowerManage]
		public ActionResult MessagePushManage()
		{
			IEnumerable<MessagePush> _AllMessagePushList = manager.GetAllMessagePush();
			ViewBag.AllMessagePushList = _AllMessagePushList;
			return View();
		}
		[HttpPost]
		public ActionResult DeleteMessagePush(int PKID)
		{
			try
			{
				manager.Delete(PKID);
				return Json(true, JsonRequestBehavior.AllowGet);
			}
			catch
			{
				return Json(false, JsonRequestBehavior.AllowGet);
			}
		}
		public ActionResult AddMessagePush(int? PKID)
		{
			int _PKID = PKID.GetValueOrDefault(0);
			if (_PKID != 0)
			{
				ViewBag.Title = "修改推送消息";
				return View(manager.GetMessagePushByID(_PKID));
			}
			else
			{
				ViewBag.Title = "新增推送消息";
				MessagePush _Model = new MessagePush()
				{
					PKID = 0,
					TotalDuration = 0,
					AheadHour = 0
				};
				return View(_Model);
			}
		}

		[HttpPost]
		public ActionResult AddMessagePush(MessagePush messagePush)
		{
			string _ReturnStr = string.Empty;
			var _ExistedMessagePush = manager.GetMessagePushByEnID(messagePush.EnID);
			if (messagePush.PKID == 0)
			{
				if (_ExistedMessagePush != null)
				{
					_ReturnStr = "已经存在该英文标识，请重新添加";
				}
				else
				{ manager.Add(messagePush); }
			}
			else
			{

				if (_ExistedMessagePush != null && _ExistedMessagePush.PKID != messagePush.PKID)
				{
					_ReturnStr = "已经存在该英文标识，请重新修改";
				}
				else
				{
					manager.Update(messagePush);
				}
			}
			if (string.IsNullOrEmpty(_ReturnStr))
			{
				return Redirect("MessagePushManage");
			}
			else
			{
				return Content("<script>alert('" + _ReturnStr + "');location='MessagePushManage'</script>");
			}
		}
		[PowerManage]
		public ActionResult AppMessagePush()
		{
			IEnumerable<MessagePush> _AllMessagePushList = manager.GetAppMessagePush();
			ViewBag.AllMessagePushList = _AllMessagePushList;
			return View();
		}

		public ActionResult DeleteAppMessagePush(int PKID)
		{
			try
			{
				manager.DeleteAppMessagePush(PKID);
				return Json(true, JsonRequestBehavior.AllowGet);
			}
			catch
			{
				return Json(false, JsonRequestBehavior.AllowGet);
			}
		}
		public ActionResult AddAppMessagePush(int? PKID)
		{
			int _PKID = PKID.GetValueOrDefault(0);
			if (_PKID != 0)
			{
				ViewBag.Title = "修改推送消息";
				return View(manager.GetAppMessagePushByID(_PKID));
			}
			else
			{
				ViewBag.Title = "新增推送消息";
				MessagePush _Model = new MessagePush()
				{
					PKID = 0,
					TotalDuration = 0,
					AheadHour = 0
				};
				return View(_Model);
			}
		}

		[HttpPost]
		public ActionResult AddAppMessagePush(MessagePush messagePush)
		{
			string _ReturnStr = string.Empty;
			var _ExistedMessagePush = manager.GetAppMessagePushByEnID(messagePush.EnID);
			if (messagePush.PKID == 0)
			{
				if (_ExistedMessagePush != null)
				{
					_ReturnStr = "已经存在该英文标识，请重新添加";
				}
				else
				{ manager.AddAppMessagePush(messagePush); }
			}
			else
			{

				if (_ExistedMessagePush != null && _ExistedMessagePush.PKID != messagePush.PKID)
				{
					_ReturnStr = "已经存在该英文标识，请重新修改";
				}
				else
				{
					manager.UpdateAppMessagePush(messagePush);
				}
			}
			if (string.IsNullOrEmpty(_ReturnStr))
			{
				return Redirect("AppMessagePush");
			}
			else
			{
				return Content("<script>alert('" + _ReturnStr + "');location='AppMessagePush'</script>");
			}
		}
        [PowerManage]
        public ActionResult AppShopBanner()
        {
            ShopsManager manager = new ShopsManager();
            List<ShopAppBanner> _AllShopAppBannerList = manager.GetShopAppBannerList();
            ViewBag.AllShopAppBannerList = _AllShopAppBannerList;
            return View();
        }
        public ActionResult AddAppShopBanner(int? PKID)
        {
            int _PKID = PKID.GetValueOrDefault(0);
            ShopsManager manager = new ShopsManager();
            if (_PKID != 0)
            {
                ShopAppBanner _Model = manager.GetAppShopBannerByPKID(_PKID);

                if (string.IsNullOrWhiteSpace(_Model.Type))
                    _Model.Type = "1";

                return View(_Model);
            }
            else
            {
                ShopAppBanner _Model = new ShopAppBanner()
                {
                    PKID = 0,
                    Type = "1"
                };
                return View(_Model);
            }
        }
        public ActionResult DeleteAppBanner(int PKID)
        {
            string _ReturnStr = string.Empty;
            ShopsManager manager = new ShopsManager();
            var _ExistedAppBanner = manager.GetAppShopBannerByPKID(PKID);
            if (_ExistedAppBanner == null)
            {
                _ReturnStr = "删除失败,当前图片不存在，请刷新页面重试";
            }
            else
            {
                int i = manager.DeleteShopBannerByPKID(PKID);
                if (i > 0)
                {
                    _ReturnStr = "0";
                }
                else
                {
                    _ReturnStr = "删除失败,服务器错误！";
                }
            }
            return Json(_ReturnStr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditShopBanner(int pkid, string type, string theme, string mainTitle, string contents, string url, string acction, string startDate
                            , string endDate, string bgColor, string imgType, string imgUrl1, string imgUrl2, string imgUrl3, string imgUrl4, string remark,string pushType)
        {
            ShopsManager manager = new ShopsManager();
            string _Result = string.Empty;
            ShopAppBanner banner = new ShopAppBanner();
            banner.PKID = pkid;
            banner.Type = type;
            banner.Theme = theme;
            banner.MainTitle = mainTitle;
            banner.Contents = contents;
            banner.URL = url;
            banner.Action = acction;
            banner.StartDate = Convert.ToDateTime(startDate);
            banner.EndDate = Convert.ToDateTime(endDate);
            banner.BgColor = bgColor;
            banner.ImgType = imgType;
            banner.ImgUrl1 = imgUrl1;
            banner.ImgUrl2 = imgUrl2;
            banner.ImgUrl3 = imgUrl3;
            banner.ImgUrl4 = imgUrl4;
            banner.Remark = remark;
            banner.Operator = User.Identity.Name;
            banner.UpdateTime = DateTime.Now;
            banner.PushType = pushType;
            if (pkid == 0)
            {
                int i = manager.InsertShopBanner(banner);
                if (i > 0)
                {
                    _Result = "新增成功！";
                }
                else
                {
                    _Result = "新增失败！";
                }
            }
            else
            {
                int i = manager.UpdateShopBanner(banner);
                if (i > 0)
                {
                    _Result = "修改成功！";
                }
                else
                {
                    _Result = "修改失败！";
                }
            }
            return Json(_Result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Save()
		{
			var _file = Request.Files;
			long size = _file[0].ContentLength;
			//文件类型  
			string type = _file[0].ContentType;
			//文件名  
			string name = _file[0].FileName;
			//文件格式  
			string _tp = System.IO.Path.GetExtension(name);
			string _BImage = string.Empty;
			if (size > 0)//修改的时候i，，如果没有上传图片，则获取隐藏url里面的插入到数据库
			{
				if (_tp.ToLower() == ".jpg" || _tp.ToLower() == ".jpeg" || _tp.ToLower() == ".gif" || _tp.ToLower() == ".png" || _tp.ToLower() == ".swf")
				{
					//获取文件流  
					System.IO.Stream stream = _file[0].InputStream;
					_BImage = UploadFile(stream, Guid.NewGuid() + type);
				}
			}
			else//插入到七牛，并获取返回路径
			{
				_BImage = "";
			}

			return Json(_BImage, JsonRequestBehavior.AllowGet);
		}
		//上传到七牛，并返回路径
		protected string UploadFile(Stream stream, string fileName)
		{
			string _ReturnStr = string.Empty;
			stream.Seek(0, SeekOrigin.Begin);
			IOClient _IOClient = new IOClient();
			var _PutPolicy = new PutPolicy(WebConfigurationManager.AppSettings["Qiniu:comment_scope"], 3600).Token();
			var _Result = _IOClient.Put(_PutPolicy, fileName, stream, new PutExtra());
			if (_Result.OK)
				_ReturnStr = WebConfigurationManager.AppSettings["Qiniu:comment_url"] + fileName;
			return _ReturnStr;
		}

        /// <summary>
        /// 图片上传地址
        /// </summary>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="identifying">标识</param>
        /// <param name="urlStr">地址</param>
        /// <returns></returns>
        public ActionResult ImageUploadToAli()
        {
            var msg = "操作失败,请刷新重试";
            var imageUrl = string.Empty;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string fileExtension = System.IO.Path.GetExtension(Request.Files[0].FileName);
                string[] allowExtension = { ".jpg", ".gif", ".png", ".jpeg" };
                if (allowExtension.Contains(fileExtension.ToLower()))
                {
                    var buffers = new byte[file.ContentLength];
                    file.InputStream.Read(buffers, 0, file.ContentLength);
                    var upLoadResult = buffers.UpdateLoadImage();
                    if (upLoadResult.Item1)
                    {
                        imageUrl = upLoadResult.Item2;
                    }
                    else
                    {
                        msg = upLoadResult.Item2;//图片上传失败
                    }
                }
                else
                {
                    msg = "图片格式不对";
                }
            }
            else
            {
                msg = "请选择文件";
            }
            return Json(new
            {
                SImage = imageUrl,
                Msg = msg
            }, "text/html");
        }

        public class WcfClinet<TService> where TService : class
		{
			public TReturn InvokeWcfClinet<TReturn>(Expression<Func<TService, TReturn>> operation)
			{
				var channelFactory = new ChannelFactory<TService>("*");
				TService channel = channelFactory.CreateChannel();
				var client = (IClientChannel)channel;
				client.Open();
				TReturn result = operation.Compile().Invoke(channel);
				try
				{
					if (client.State != CommunicationState.Faulted) { client.Close(); }
				}
				catch
				{
					client.Abort();
				}
				return result;
			}
		}
	}
}