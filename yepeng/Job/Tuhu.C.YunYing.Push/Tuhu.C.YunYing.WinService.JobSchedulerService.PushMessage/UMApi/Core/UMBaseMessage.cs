using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi.Core
{
    public class UMBaseMessage
    {
        /// <summary>
        /// 必填 应用唯一标识
        /// </summary>
        public string appkey { get; set; }
        /// <summary>
        /// 注意：该值由UMengMessagePush自动生成，无需主动赋值
        /// 
        /// 必填 时间戳，10位或者13位均可，时间戳有效期为10分钟 
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 必填 消息发送类型,其值可以为:
        /// <example>
        ///unicast-单播
        ///listcast-列播(要求不超过500个device_token)
        ///filecast-文件播
        ///(多个device_token可通过文件形式批量发送）
        ///broadcast-广播
        ///groupcast-组播
        ///(按照filter条件筛选特定用户群, 具体请参照filter参数)
        ///customizedcast(通过开发者自有的alias进行推送), 
        ///包括以下两种case:
        ///- alias: 对单个或者多个alias进行推送
        ///- file_id: 将alias存放到文件后，根据file_id来推送
        ///</example>
        /// </summary>
        public string type { get; set; }

        public string device_tokens { get; set; }
        /// <summary>
        /// 可选 
        /// 当type=customizedcast时必填，alias的类型, 
        /// alias_type可由开发者自定义,
        /// 开发者在SDK中调用setAlias(alias, alias_type)时所设置的alias_type
        /// </summary>
        public string alias_type { get; set; }
        /// <summary>
        /// 可选 当type=customizedcast时, 
        /// 开发者填写自己的alias。 要求不超过50个alias,多个alias以英文逗号间隔。
        /// 在SDK中调用setAlias(alias, alias_type)时所设置的alias
        /// </summary>
        public string alias { get; set; }

        public string file_id { get; set; }

        public JsonObject filter { get; set; }

        /// <summary>
        /// 可选 发送策略
        /// </summary>
        public Policy policy { get; set; }

        public string production_mode { get; set; }
        /// <summary>
        /// 可选 发送消息描述，建议填写。
        /// </summary>
        public string description { get; set; }
        public string thirdparty_id { get; set; }
        /// <summary>
        ///  回调url, 最⼤大⻓长度256字节.
        /// </summary>
        public string receipt_url { get; set; }
        /// <summary>
        /// 回执类型。1-送达回执；2-点击回执；3-送达和点击回执。默认为3。! 这两个参数仅在type=unicast或listcast或type=customizedcast时起作⽤用
        /// </summary>
        public int receipt_type { get; set; }
    }
}
