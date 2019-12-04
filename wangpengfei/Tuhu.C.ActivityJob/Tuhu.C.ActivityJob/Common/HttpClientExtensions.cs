using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tuhu.C.ActivityJob.Common
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
        }

        public static async Task<T> ReadAsAMapAsync<T>(this HttpContent content)
        {
            var str = await content.ReadAsStringAsync();

            // 高德API未获取到数据会返回空数组，导致反序列化失败
            str = str.Replace("\"formatted_address\":[]", "\"formatted_address\":\"\"");
            str = str.Replace("\"province\":[]", "\"province\":\"\"");
            str = str.Replace("\"city\":[]", "\"city\":\"\"");

            return JsonConvert.DeserializeObject<T>(str);
        }

        public static async Task<T> ReadAsTuHuAsync<T>(this HttpContent content)
        {
            var buffer = await content.ReadAsByteArrayAsync();

            var str = Encoding.UTF8.GetString(buffer);
            str = str.Replace(",\"data\":[]", string.Empty);

            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
