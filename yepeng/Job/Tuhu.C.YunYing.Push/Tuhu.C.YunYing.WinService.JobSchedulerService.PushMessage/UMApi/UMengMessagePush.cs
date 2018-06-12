using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using RestSharp;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi.Base;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi.Core;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi
{
    /// <summary>
    /// 友盟消息推送
    /// create by jasnature
    /// </summary>
    public class UMengMessagePush : IPush, IDisposable
    {
        #region 内部字段

        private RestClient requestClient;

        private volatile static Dictionary<Type, PropertyInfo[]> cacheParamType = null;

        private ReaderWriterLock lockrw = null;

        private MD5CryptionUMeng md5;

        private Encoding encoder = Encoding.UTF8;

        private string requestProtocol = "http";

        private string requestMethod = "POST";

        private string hostUrl = "msg.umeng.com";

        private string postPath = "api/send";

        private string queryPath = "api/status";

        private string apiFullUrl = null;

        private string appkey = null;

        private string appMasterSecret = null;

        protected const string USER_AGENT = "Push-Server/4.5";

        #endregion


        #region 公共方法

        /// <summary>
        /// 使用默认的参数构造,参数从友盟网站的应用信息中获取
        /// </summary>
        /// <param name="appkey">appkey</param>
        /// <param name="appMasterSecret">App Master Secret，供API对接友盟服务器使用的密钥</param>
        /// <param name="sendPath">True:指定发送消息api路径；False:指定查询地址路径</param>
        public UMengMessagePush(string appkey, string appMasterSecret, bool sendPath = true)
        {
            var path = sendPath ? postPath : queryPath;
            this.apiFullUrl = string.Concat(requestProtocol, "://", hostUrl, "/", path, "/");
            this.appkey = appkey;
            this.appMasterSecret = appMasterSecret;
            this.md5 = new MD5CryptionUMeng();
            cacheParamType = new Dictionary<Type, PropertyInfo[]>(10);
            this.lockrw = new ReaderWriterLock();
        }

        /// <summary>
        /// 推送消息，注意如果初始化本类已经填写了appkey，
        /// <paramref name="paramsJsonObj"/> 里面的appkey会自动赋值
        /// 反之如果您填写了<paramref name="paramsJsonObj"/> 里面的appkey，将采用参数里面的值，忽略本类初始化值。
        /// </summary>
        /// <param name="paramsJsonObj"></param>
        /// <returns></returns>
        public ReturnJsonClass SendMessage(UMBaseMessage paramsJsonObj)
        {
            try
            {
                var request = CreateHttpRequest(paramsJsonObj);
                var resultResponse = requestClient.Execute(request);
                ReturnJsonClass rjs = SimpleJson.DeserializeObject<ReturnJsonClass>(resultResponse.Content);
                return rjs;
            }
            catch (Exception ex)
            {
                ReturnJsonClass rjs = new ReturnJsonClass();
                rjs.ret = "Fail";
                rjs.data = new ResultInfo();
                rjs.data.error_code = ex.Message;
                return rjs;
            }
        }

        public TaskQueryResult QueryTaskStatus(TaskQueryModel paramsJsonObj)
        {
            try
            {
                var request = CreateHttpRequest(paramsJsonObj);
                var resultResponse = requestClient.Execute(request);
                TaskQueryResult rjs = SimpleJson.DeserializeObject<TaskQueryResult>(resultResponse.Content);
                return rjs;
            }
            catch (Exception ex)
            {
                var rjs = new TaskQueryResult();
                rjs.ret = "Fail";
                rjs.data = new StatusInfo {error_code = ex.Message};
                return rjs;
            }
        }

        /// <summary>
        /// 异步推送消息，注意如果初始化本类已经填写了appkey，
        /// <paramref name="paramsJsonObj"/> 里面的appkey会自动赋值
        /// 反之如果您填写了<paramref name="paramsJsonObj"/> 里面的appkey，将采用参数里面的值，忽略本类初始化值。 
        /// </summary>
        /// <param name="paramsJsonObj"></param>
        /// <param name="callback"></param>
        public void AsynSendMessage(UMBaseMessage paramsJsonObj, Action<ReturnJsonClass> callback)
        {
            var request = CreateHttpRequest(paramsJsonObj);

            requestClient.ExecuteAsync(request, resultResponse =>
            {
                if (callback != null)
                {
                    callback(SimpleJson.DeserializeObject<ReturnJsonClass>(resultResponse.Content));
                }
            });
        }

        #endregion

        #region 私有辅助方法

        private RestRequest CreateHttpRequest(UMBaseMessage paramsJsonObj)
        {
            string bodyJson = InitParamsAndUrl(paramsJsonObj);

            if (requestClient == null)
            {
                requestClient = new RestClient(apiFullUrl);
                requestClient.Encoding = Encoding.UTF8;
                requestClient.UserAgent = USER_AGENT;
            }
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", bodyJson, ParameterType.RequestBody);
            //request.AddJsonBody(paramsJsonObj);
            return request;
        }

        private RestRequest CreateHttpRequest(TaskQueryModel paramsJsonObj)
        {
            string bodyJson = InitParamsAndUrl(paramsJsonObj);

            if (requestClient == null)
            {
                requestClient = new RestClient(apiFullUrl);
                requestClient.Encoding = Encoding.UTF8;
                requestClient.UserAgent = USER_AGENT;
            }
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", bodyJson, ParameterType.RequestBody);
            return request;
        }

        private string InitParamsAndUrl(UMBaseMessage paramsJsonObj)
        {
            if (string.IsNullOrEmpty(paramsJsonObj.appkey)) paramsJsonObj.appkey = this.appkey;

            paramsJsonObj.timestamp = GetTimeStamp().ToString();

            //string json = RestSharp.SimpleJson.SerializeObject(paramsJsonObj);
            Newtonsoft.Json.JsonSerializerSettings jssetting = new Newtonsoft.Json.JsonSerializerSettings();
            jssetting.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(paramsJsonObj, jssetting);

            string calcSign = md5.GenerateMD5(requestMethod + apiFullUrl + json + appMasterSecret).ToLower();

            this.apiFullUrl = string.Format("{0}?sign={1}", this.apiFullUrl, calcSign);

            return json;
        }

        private string InitParamsAndUrl(TaskQueryModel paramsJsonObj)
        {
            if (string.IsNullOrEmpty(paramsJsonObj.appkey)) paramsJsonObj.appkey = this.appkey;

            paramsJsonObj.timestamp = GetTimeStamp().ToString();

            //string json = RestSharp.SimpleJson.SerializeObject(paramsJsonObj);
            Newtonsoft.Json.JsonSerializerSettings jssetting = new Newtonsoft.Json.JsonSerializerSettings();
            jssetting.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(paramsJsonObj, jssetting);

            string calcSign = md5.GenerateMD5(requestMethod + apiFullUrl + json + appMasterSecret).ToLower();

            this.apiFullUrl = string.Format("{0}?sign={1}", this.apiFullUrl, calcSign);

            return json;
        }

        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        private uint GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Convert.ToUInt32(ts.TotalSeconds);
        }

        /// <summary>
        /// 多线程安全缓存参数类型集合
        /// </summary>
        private PropertyInfo[] GetCacheParamType<T>(T pb)
        {
            Type pbtype = pb.GetType();
            try
            {
                if (cacheParamType.ContainsKey(pbtype))
                {
                    return cacheParamType[pbtype];
                }
                else
                {
                    lockrw.AcquireWriterLock(1000);
                    if (!cacheParamType.ContainsKey(pbtype))
                    {
                        PropertyInfo[] pis = pbtype.GetProperties().OrderBy(p => p.Name).ToArray();
                        cacheParamType.Add(pbtype, pis);
                        return pis;
                    }
                    else
                    {
                        return cacheParamType[pbtype];
                    }
                }
            }
            finally
            {
                if (lockrw.IsReaderLockHeld)
                {
                    lockrw.ReleaseReaderLock();
                }
                if (lockrw.IsWriterLockHeld)
                {
                    lockrw.ReleaseWriterLock();
                }
            }

        }

        #endregion

        public void Dispose()
        {
            if (md5 != null) md5.Dispose();
            if (cacheParamType != null) cacheParamType.Clear();
        }
    }
}
