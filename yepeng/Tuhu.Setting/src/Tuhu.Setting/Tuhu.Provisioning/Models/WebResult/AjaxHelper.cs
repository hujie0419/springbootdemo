using System.Net;
using System.Text;
using System.Web.Mvc;

namespace Tuhu.Provisioning.Models.WebResult
{
    //所有Ajax都要返回这个类型的对象
    public class AjaxHelper
    {
        public static JsonResult MvcJsonResult(HttpStatusCode statusCode, string msg, object data = null)
        {
            var result = new AjaxResult
            {
                StatusCode = statusCode,
                Msg = msg,
                Data = data
            };
            return new JsonNetResult { Data = result, ContentEncoding = Encoding.UTF8, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public class AjaxResult
        {
            /// <summary>
            /// 状态码
            /// </summary>
            public HttpStatusCode StatusCode { get; set; }

            /// <summary>
            /// 消息
            /// </summary>
            public string Msg { get; set; }

            /// <summary>
            /// 执行返回的数据
            /// </summary>
            public object Data { get; set; }
        }
    }
}
