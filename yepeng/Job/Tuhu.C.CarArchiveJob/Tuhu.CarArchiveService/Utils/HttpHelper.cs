using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.CarArchiveService.Utils
{
    public static class HttpHelper
    {
        private static readonly HttpClient httpClient;

        static HttpHelper()
        {
            httpClient = new HttpClient();
        }

        public static string DoGet()
        {
            string result = string.Empty;

            //httpClient.get

            return result;
        }
    }
}
