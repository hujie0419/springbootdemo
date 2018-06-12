using PddSdk.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PddSdk.Request
{
    public class PddOrderSearchRequest : IPddRequest<PddOrderSearchResponse>
    {
        public PddOrderSearchRequest(string mallId,string secret)
        {
            Type = "pdd.order.number.list.increment.get";
            IsLuckyFlag = 1;
            OrderStatus = 1;
            RefundStatus = 5;
            PageSize = 100;
            Page = 1;
            MallId = mallId;
            Secret = secret;
        }
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 1:非抽奖订单 2：抽奖订单
        /// </summary>
        public int IsLuckyFlag { get; set; }
        /// <summary>
        /// 发货状态，1：待发货，2：已发货待签收，3：已签收 5：全部
        /// </summary>
        public int OrderStatus { get; set; }
        /// <summary>
        /// 售后状态 1：无售后或售后关闭，2：售后处理中， 3：退款中，4：退款成功 5：全部
        /// </summary>
        public int RefundStatus { get; set; }
        /// <summary>
        /// 最后更新时间开始时间的时间戳，指格林威治时间 1970 年 01 月 01 日 00 时 00 分 00 秒(北京时间 1970 年 01 月 01 日 08 时 00 分 00 秒)起至现在的总秒数
        /// </summary>
        public DateTime StartUpdatedAt { get; set; }
        /// <summary>
        /// 最后更新时间结束时间的时间戳，指格林威治 时间 1970 年 01 月 01 日 00 时 00 分 00 秒(北京时间 1970 年 01 月 01 日 08 时 00 分 00 秒)起至现在的总秒数
        /// 注：开始时间结束时间间距不超过 30 分钟
        /// </summary>
        public DateTime EndUpdatedAt { get; set; }
        /// <summary>
        /// 返回数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 返回页码 默认 1，页码从 1 开始
        /// </summary>
        public int Page { get; set; }

        public HttpMethod Method => HttpMethod.Post;
        /// <summary>
        /// 拼多多后台，服务市场->公共接口 页面的接入码
        /// </summary>
        public string MallId { get; set; }

        private Dictionary<string, object> RequestParam
        {
            get
            {
                var param = new Dictionary<string, object>
                {
                    ["type"] = Type,
                    ["is_lucky_flag"] = IsLuckyFlag,
                    ["order_status"] = OrderStatus,
                    ["refund_status"] = RefundStatus,
                    ["start_updated_at"] = CommonHelper.ToTimestamp(StartUpdatedAt.AddHours(-8)),
                    ["end_updated_at"] = CommonHelper.ToTimestamp(EndUpdatedAt.AddHours(-8)),
                    ["page_size"] = PageSize,
                    ["page"] = Page,
                    ["data_type"] = "JSON",
                    ["timestamp"] = CommonHelper.ToTimestamp(DateTime.Now.AddHours(-8)),
                    ["mall_id"] = MallId
                };
                return param;
            }
        }
        private string Sign
        {
            get
            {
                var result = RequestParam.OrderBy(_ => _.Key).Aggregate(string.Empty, (current, p) => current + (p.Key + p.Value));
                return CommonHelper.GetMd5(Secret + result + Secret, Encoding.UTF8);
            }
        }
        /// <summary>
        /// 接口密码
        /// </summary>
        public string Secret { get; set; }
        public string Uri =>string.Empty;



        public object GetParam()
        {
            var param = RequestParam;
            param.Add("sign", Sign);
            return param;
        }

        public void Validate() { }
    }
}
