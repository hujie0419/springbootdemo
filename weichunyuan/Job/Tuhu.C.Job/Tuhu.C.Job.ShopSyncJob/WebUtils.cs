using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Tuhu.C.Job.ShopSyncJob
{
    public class WebUtils
    {
        public static string GetData(string url, Dictionary<string, string> getdata = null)
        {
            string data;
            if (getdata != null)
            {
                IEnumerator<KeyValuePair<string, string>> dem = getdata.GetEnumerator();

                StringBuilder query = new StringBuilder("");
                while (dem.MoveNext())
                {
                    string key = dem.Current.Key;
                    string value = dem.Current.Value;
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        query.Append(key).Append("=").Append(value).Append("&");
                    }
                }

                string content = query.ToString().Substring(0, query.Length - 1);
                //string formatdata = string.Join("&", getdata.Select(x => string.Format("{0}={1}", x.Key, x.Value)).OrderBy(x => x));
                url = string.Format("{0}?{1}", url, content);
            }
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(url))
                {
                    using (var reader = new System.IO.StreamReader(stream))
                        data = reader.ReadToEnd();
                }
            }
            return data;
        }
        public static HttpWebRequest GetWebRequest(string url, string method)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = method;
            req.KeepAlive = true;
            //req.UserAgent = "Aop4Net";
            //req.Timeout = this._timeout;
            return req;
        }

        public static string BuildQuery(IDictionary<string, string> parameters, string charset)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");

                    string encodedValue = HttpUtility.UrlEncode(value, Encoding.GetEncoding(charset));
                    postData.Append(encodedValue);
                    hasParam = true;
                }
            }

            return postData.ToString();
        }
        public static string PostData(string url, Dictionary<string, string> parameters)
        {
            string charset = "utf-8";
            HttpWebRequest req = GetWebRequest(url, "POST");
            req.ContentType = "application/x-www-form-urlencoded;charset=" + charset;

            byte[] postData = Encoding.GetEncoding(charset).GetBytes(BuildQuery(parameters, charset));
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = Encoding.UTF8;
            return GetResponseAsString(rsp, encoding);
        }

        public string DoGet(string url, IDictionary<string, string> parameters, string charset)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildQuery(parameters, charset);
                }
                else
                {
                    url = url + "?" + BuildQuery(parameters, charset);
                }
            }

            HttpWebRequest req = GetWebRequest(url, "GET");
            req.ContentType = "application/x-www-form-urlencoded;charset=" + charset;

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
            return GetResponseAsString(rsp, encoding);
        }
        public static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                // 按字符读取并写入字符串缓冲
                int ch = -1;
                while ((ch = reader.Read()) > -1)
                {
                    // 过滤结束符
                    char c = (char)ch;
                    if (c != '\0')
                    {
                        result.Append(c);
                    }
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }
    }

}
