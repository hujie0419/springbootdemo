using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Response
{
    public class CreateOrderResponse : QplResponse
    {
        private int code = 999;

        public bool Success { get { return code == 1; } }

        public override bool IsError { get { return code == 999; } }

        public string orderNo { get;set; }
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
                    var json = JsonConvert.DeserializeObject<JToken>(value).Value<JToken>("qipeilong_coustomerorder_add_response").Value<JToken>("coustomerorderAddResponse");
                      code=json.Value<int>("code");
                    orderNo = json.Value<JToken>("orderno").ToString();
                }
                catch
                {
                    code = 999;
                }
            }
        }

    }
}
