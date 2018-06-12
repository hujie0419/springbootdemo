using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using DdSdk.Api.Request;
using DdSdk.Api.Response;
using Tuhu;

namespace DdSdk.Api
{
    public class DdClient : IDdClient
    {
        private static readonly ILog _logger;

        static DdClient()
        {
            _logger = LogManager.GetLogger<DdClient>();
        }

        private readonly string apiUrl;
        private readonly string appKey;
        private readonly string appSecret;
        private readonly HttpClient httpClient;
        public DdClient(string apiUrl, string appKey, string appSecret)
        {
            httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri(apiUrl);

            this.apiUrl = apiUrl;
            this.appKey = appKey;
            this.appSecret = appSecret;
        }

        public async Task<T> ExecuteAsync<T>(IDdRequest<T> request, string session) where T : DdResponse
        {
            var dic = new SortedDictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            dic["app_key"] = appKey;
            dic["format"] = "xml";
            dic["method"] = request.ApiName;
            dic["session"] = session;
            dic["sign_method"] = "md5";
            dic["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dic["v"] = "1.0";

            dic["sign"] = MD5(appSecret, dic);


            string message = null;
            if (request.Method == HttpMethod.Get)
            {
                foreach (var item in request.GetParam())
                {
                    if (!dic.ContainsKey(item.Key))
                        dic[item.Key] = item.Value;
                }

                var query = new StringBuilder();
                foreach (var item in dic)
                {
                    query.Append("&");
                    query.Append(item.Key);
                    query.Append("=");
                    query.Append(Uri.EscapeDataString(item.Value));
                }
                query.Replace('&', '?', 0, 1);

                message = await httpClient.GetStringAsync(apiUrl + query);
            }
            else
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" + request.GetXml().ToString())), "sendGoods", "sendGoods.xml");

                    var query = new StringBuilder();
                    foreach (var item in dic)
                    {
                        query.Append("&");
                        query.Append(item.Key);
                        query.Append("=");
                        query.Append(Uri.EscapeDataString(item.Value));
                    }
                    query.Replace('&', '?', 0, 1);

                    var tResponse = await httpClient.PostAsync(apiUrl + query, content);

                    message = await tResponse.Content.ReadAsStringAsync();
                }
            }

            var response = Activator.CreateInstance<T>();
            try
            {
                response.Body = message;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return response;
        }

        private static string MD5(string appSecret, SortedDictionary<string, string> dic)
        {
            var sb = new StringBuilder(dic.Count * 20);

            sb.Append(appSecret);
            foreach (var item in dic)
            {
                sb.Append(item.Key);
                sb.Append(item.Value);
            }
            sb.Append(appSecret);

            return SecurityHelper.Hash(sb.ToString());
        }
    }
}
