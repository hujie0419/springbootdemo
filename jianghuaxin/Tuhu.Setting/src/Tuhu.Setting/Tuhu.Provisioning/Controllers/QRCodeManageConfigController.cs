using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.QRCodeManageConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Push;

namespace Tuhu.Provisioning.Controllers
{
    public class QRCodeManageConfigController : Controller
    {
        private readonly static QRCodeManageConfigManager _QRCodeManageConfigManager = new QRCodeManageConfigManager();

        public ActionResult Index(int pageIndex = 1, int pageSieze = int.MaxValue)
        {
            ViewBag.QRCodeManageList = _QRCodeManageConfigManager.GetListByPage("", "", pageIndex, pageSieze);
            return View();
        }

        public ActionResult AddOrEdit(int id)
        {
            if (id != 0)
            {
                //修改
                QRCodeManageModel model = _QRCodeManageConfigManager.GetModel(id)
                    ?? new QRCodeManageModel()
                    {
                        Id = -1,
                        IsShow = 1,
                        CreateTime = DateTime.Now
                    };
                return View(model);
            }
            else
            {
                //新增
                QRCodeManageModel model = new QRCodeManageModel()
                {
                    Id = 0,
                    IsShow = 1,
                    QRCodeType = 0,
                    CreateTime = DateTime.Now
                };
                return View(model);
            }
        }

        [ValidateInput(false)]
        public ActionResult Save(int id, QRCodeManageModel model)
        {
            bool result = false;
            if (id == 0)
            {
                model.QRCodeUrl = CreateQRCode(model) ?? model.QRCodeUrl;
                //新增
                model.CreateTime = DateTime.Now;
                model.IsShow = 1;
                result = _QRCodeManageConfigManager.Add(model);
            }
            else
            {
                model.QRCodeUrl = CreateQRCode(model) ?? model.QRCodeUrl;
                //修改
                result = _QRCodeManageConfigManager.Update(model);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int RemoveConfig(int id = 0)
        {
            if (id > 0)
                return _QRCodeManageConfigManager.Delete(id) ? 1 : 0;
            else
                return 0;
        }


        public ActionResult CheckedTraceId(int? traceId, int? id = 0)
        {
            if (traceId == null)
                return Content("0");

            bool result = _QRCodeManageConfigManager.CheckedTraceId(traceId ?? 0, id ?? 0);
            return Content(result ? "1" : "0");
        }

        #region access_token 
        /// <summary>
        /// 获取 二维码地址
        /// </summary>
        /// <returns></returns>
        private string CreateQRCode(QRCodeManageModel model)
        {
            if (model != null
                && !string.IsNullOrWhiteSpace(model.appid)
                && !string.IsNullOrWhiteSpace(model.appsecret))
            {
                string access_token = CreateToken(model.appid, model.appsecret).access_token ?? "",
                       tempQRCodeUrl = string.Format("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}", access_token),
                       eikyQRCodeUrl = string.Format("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}", access_token),
                       tempQRCodeParams = "{\"expire_seconds\": " + model.ValidityTime + ", \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": " + model.TraceId + "}}}",
                       eikyQRCodeParams = "{\"action_name\": \"QR_LIMIT_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": " + model.TraceId + "}}}";

                string QRCodeJson = model.QRCodeType == 0
                    ? webRequestPost(tempQRCodeUrl, tempQRCodeParams)
                    : webRequestPost(eikyQRCodeUrl, eikyQRCodeParams);

                QRCodeTicket _QRCodeTicket = JsonConvert.DeserializeObject<QRCodeTicket>(QRCodeJson ?? "");

                return (_QRCodeTicket != null && !string.IsNullOrWhiteSpace(_QRCodeTicket.ticket))
                    ? string.Format("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}", _QRCodeTicket.ticket)
                    : "{error:'返回无效ticket'}";
            }
            return null;
        }
        /// <summary>
        /// 创建微信 accesstoken
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appsecret"></param>
        /// <returns></returns>
        private static token CreateToken(string appkey, string appsecret)
        {
            token token = new token();
            int platform = -1;
            using (var client = new WeiXinPushClient())
            {
                var response = client.SelectWxConfigs();
                if (response.Success && response.Result != null)
                {
                    var first = response.Result
                        .FirstOrDefault(x => string.Equals(x.appKey, appkey, StringComparison.OrdinalIgnoreCase));
                    if (first != null)
                    {
                        platform = first.platform;
                    }
                }
            }
            var access_token = webRequestGet("https://wx.tuhu.cn/packet/test?platform=" + platform);
            token= JsonConvert.DeserializeObject<token>(access_token);
            if (token != null && !string.IsNullOrWhiteSpace(token.access_token))
            {
                token.CreateTime = DateTime.Now;
            }
            else
            {
                throw new Exception("获取token失败");
            }
            return token;
        }
        private static IDictionary<string, Mutex> _lockObjectPool = new Dictionary<string, Mutex>();
        private static Mutex GetLockObject(string key)
        {
            Mutex lockObject;
            if (!_lockObjectPool.TryGetValue(key, out lockObject))
            {
                lockObject = new Mutex(false);
                _lockObjectPool[key] = lockObject;
            }
            return lockObject;
        }
        #endregion

        #region get & Post
        /// <summary>
        /// 提交数据请求
        /// </summary>
        public static string webRequestPost(string url, string param)
        {
            //发送请求的数据
            WebRequest myHttpWebRequest = WebRequest.Create(url);
            myHttpWebRequest.Method = "POST";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(param);
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = byte1.Length;
            using (Stream newStream = myHttpWebRequest.GetRequestStream()) {
                newStream.Write(byte1, 0, byte1.Length);
            }
            //发送成功后接收返回的信息
            HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
            string lcHtml = string.Empty;
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, enc);
            lcHtml = streamReader.ReadToEnd();
            stream.Close();
            streamReader.Close();
            return lcHtml;
        }

        /// <summary>
        /// Get 提交调用抓取
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="param">参数</param>
        /// <returns>string</returns>
        public static string webRequestGet(string url)
        {
            string access_token = "";
            try
            {
                access_token = new WebClient().DownloadString(url);
            }
            catch
            {
            }
            return access_token;
        }
        #endregion Post提交数据
    }

    public class token
    {
        public token() { }
        public string access_token { get; set; }
        public DateTime CreateTime { get; set; }
        public string expires_in { get; set; }
    }

    public class QRCodeTicket
    {
        public string ticket { get; set; }
        public string expire_seconds { get; set;}
        public string url { get; set; }
    }
}