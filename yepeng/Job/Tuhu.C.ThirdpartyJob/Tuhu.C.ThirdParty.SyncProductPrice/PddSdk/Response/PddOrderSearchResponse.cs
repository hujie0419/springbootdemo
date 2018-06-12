using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PddSdk.Response
{
    public class PddOrderSearchResponse : PddResponse
    {
        public List<PddOrderModel> Orders { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }
        public override string Body
        {
            get
            {
                return base.Body;
            }
            internal set
            {
                base.Body = value;

                try
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (value.Contains("error_response"))
                        {
                            var errorResponse = JsonConvert.DeserializeObject<JToken>(value).Value<JToken>("error_response");
                            ErrorCode = errorResponse.Value<int>("error_code");
                            ErrorMsg = errorResponse.Value<string>("error_msg");
                            IsError = true;
                        }
                        else
                        {
                            var json = JsonConvert.DeserializeObject<JToken>(value).Value<JToken>("order_sn_increment_get_response");
                            Orders = json.Value<JToken>("order_sn_list").ToObject<List<PddOrderModel>>();
                            TotalCount = json.Value<int>("total_count");
                            IsError = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorCode = 999;
                    ErrorMsg = ex.ToString();
                    IsError = true;
                }
            }
        }
    }
}
