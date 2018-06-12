using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Tuhu.Provisioning.Controllers
{
    /// <summary>
    /// 配置生成器
    /// </summary>
    public class ConfigGeneratorController : Controller
    {
        //
        // GET: /ConfigGenerator/

        public ActionResult Index()
        {
            if (System.IO.File.Exists(Server.MapPath("/Content/config.txt")))
            {
                using (StreamReader reader = new StreamReader(Server.MapPath("/Content/config.txt")))
                {
                    ViewBag.Content = reader.ReadToEnd();
                }

            }
            else
                ViewBag.Content = "";
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Save(string content)
        {
            using (Stream stream = new FileStream(Server.MapPath("/Content/config.txt"), FileMode.OpenOrCreate))
            {
                stream.Write(System.Text.Encoding.UTF8.GetBytes(content), 0, content.Length);
                return JavaScript("1");
            }


        }

        //长链接换短链接
        public class Shortlink
        {
            //状态
            public int status { get; set; }
            //长链接
            public string longurl { get; set; }
            //错误信息
            public string err_msg { get; set; }
            //返回短链接
            public string tinyurl { get; set; }
        }
        [HttpPost]
        public ActionResult GetShortLink(string wxUrl, string type, string data)
        {
            string urls = "http://dwz.cn/create.php";
            var url = HttpUtility.UrlEncode("?type=" + type + "&data=" + data);
            string links = "url=" + wxUrl + url;
            var shortlink = JsonConvert.DeserializeObject<Shortlink>(webRequestPost(urls, links)).tinyurl;
            return Json(shortlink);
        }
        /// <summary>
        /// Post 提交调用抓取
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="param">参数</param>
        /// <returns>string</returns>
        public string webRequestPost(string url, string param)
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(param);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url + "?" + param);
            req.Method = "POST";
            req.Timeout = 120 * 1000;
            req.ContentType = "application/x-www-form-urlencoded;";
            req.ContentLength = bs.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }

            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);

                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                return sb.ToString();
            }
        }
    }
}
