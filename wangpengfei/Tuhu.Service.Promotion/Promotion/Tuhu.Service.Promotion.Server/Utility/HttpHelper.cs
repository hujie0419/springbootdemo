using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Server.Utility
{



    public  class HttpHelper
    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpHelper(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }


        public  async Task<string> PostAsJsonAsync(string url, string parameters, string mediaType = "application/json", int timeout = 1000)
        {
            string result = string.Empty;
            try
            {
                var client = _clientFactory.CreateClient();

                client.BaseAddress = new Uri(url);

                client.Timeout = TimeSpan.FromMilliseconds(timeout);
                //巨坑
                client.DefaultRequestHeaders.ExpectContinue = false;
                using (var content = new StringContent(parameters, Encoding.UTF8, mediaType))
                {
                    var postResult = await client.PostAsync(url, content).ConfigureAwait(false);
                    result = await postResult.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return result;
        }


        //public static string PostAsJson(string url, string parameters, string mediaType = "application/json", int timeout = 1000)
        //{
        //    string result = string.Empty;
        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            using (HttpContent httpContent = new StringContent(parameters, Encoding.UTF8))
        //            {
        //                HttpResponseMessage response = client.PostAsync(url, httpContent).Result;
        //                return response.Content.ReadAsStringAsync().Result;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    return result;
        //}

        public static string HttpPost(string url, string postStr = "", Encoding encode = null)
        {
            string result;

            try
            {
                using (var webClient = new WebClient { Encoding = Encoding.UTF8 })
                {
                    if (encode != null)
                        webClient.Encoding = encode;

                    var sendData = Encoding.GetEncoding("GB2312").GetBytes(postStr);

                    webClient.Headers.Add("Content-Type", "application/json");
                    webClient.Headers.Add("ContentLength", sendData.Length.ToString(CultureInfo.InvariantCulture));

                    var readData = webClient.UploadData(url, "POST", sendData);

                    result = Encoding.GetEncoding("GB2312").GetString(readData);
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }


        public static async Task<string> Get(string url, int timeout = 1000)
        {
            string result = string.Empty;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeout);
                    var getResult = await client.GetAsync(url).ConfigureAwait(false);
                    result = await getResult.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return result;
        }
    }
}
