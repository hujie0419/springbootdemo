using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Common.Logging;

namespace Tuhu.C.Job.UserAuthJob.BLL
{
    public class UserAuthSendRequest
    {
        private static readonly ILog Logger = LogManager.GetLogger<UserAuthSendRequest>();
        protected async Task<JObject> GetJsonAsync(string url, Encoding encoding)
        {
            var byResult = await GetAsync(url);
            if (byResult == null) return null;

            try
            {
                return JsonConvert.DeserializeObject<JObject>(encoding.GetString(byResult));
            }
            catch (Exception ex)
            {
                Logger.Error($"异常=> url:{url} 信息:{ex.ToString()}");
                return null;
            }
        }

        private async Task<byte[]> GetAsync(string url)
        {
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.Timeout = TimeSpan.FromSeconds(30);

                    httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                    httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "zh-CN,zh;q=0.8");

                    using (var response = await httpclient.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                            return await response.Content.ReadAsByteArrayAsync();

                        Logger.Warn($"网络异常=> url:{url};Http_Error:${ response.StatusCode};response.StatusCode:{response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"异常=> url:{url} 信息:{ex.ToString()}");
                return null;
            }
        }
    }
}
