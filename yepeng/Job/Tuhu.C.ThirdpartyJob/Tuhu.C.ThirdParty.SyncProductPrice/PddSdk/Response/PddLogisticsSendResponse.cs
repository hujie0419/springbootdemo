using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PddSdk.Response
{
    public class PddLogisticsSendResponse: PddResponse
    {
        public bool IsSuccess { get; set; }
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
                            var json = JsonConvert.DeserializeObject<JToken>(value).Value<JToken>("logistics_online_send_response");
                            IsSuccess = json.Value<bool>("is_success");
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
