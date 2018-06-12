using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi.Core
{
    public class PostUMIosJson : UMBaseMessage
    {
        /// <summary>
        /// 必填 消息内容(iOS最大为2012B), 包含参数说明如下(JSON格式):
        /// </summary>
        public JsonObject payload { get; set; }
    }

    public class IosPayload
    {
        /// <summary>
        /// 必填 严格按照APNs定义来填写
        /// </summary>
        public APS aps { get; set; }

        public string VC { get; set; }

        public string url { get; set; }
    }

    public class APS
    {
        /// <summary>
        /// 必填
        /// </summary>
        public object alert { get; set; }
        /// <summary>
        /// 可选
        /// </summary>
        public int? badge { get; set; }
        /// <summary>
        /// 可选
        /// </summary>
        public string sound { get; set; }
        /// <summary>
        /// 可选, 注意: ios8才支持该字段。
        /// </summary>
        public string category { get; set; }
    }
}
