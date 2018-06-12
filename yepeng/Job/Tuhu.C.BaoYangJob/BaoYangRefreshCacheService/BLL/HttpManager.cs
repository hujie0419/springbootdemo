using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Utility;

namespace BaoYangRefreshCacheService.BLL
{
    public class HttpManager
    {

        private static readonly Encoding GB2312 = Encoding.GetEncoding("gb2312");

        private static readonly HtmlParser Parser = new HtmlParser();

        public static HttpManager Create(string url) => new HttpManager(url);

        private string url;

        private HttpManager(string url)
        {
            this.url = url;
        }

        public async Task<T> GetResult<T>(Func<IHtmlDocument, T> convertFunc)
        {
            using (var client = new HttpProxyClient())
            {
                var serviceResult = await client.GetAsync(url);
                if (!serviceResult.Success)
                {
                    serviceResult.ThrowIfException(true);
                }
                var bytes = serviceResult.Result;
                var html = GB2312.GetString(bytes);
                var document = Parser.Parse(html);
                return convertFunc(document);
            }
        }

    }
}
