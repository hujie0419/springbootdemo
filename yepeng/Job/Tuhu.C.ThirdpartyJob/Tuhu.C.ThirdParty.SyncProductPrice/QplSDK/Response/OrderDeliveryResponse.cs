using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Response
{
    public class OrderDeliveryResponse : QplResponse
    {
        private int code = 999;

        public bool Success { get { return code == 0; } }

        public override bool IsError { get { return code == 999; } }

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
                    code = JsonConvert.DeserializeObject<JToken>(value).Value<JToken>("qipeilong_orderdelivery_response").Value<JToken>("orderDeliveryResponse").Value<int>("code");
                }
                catch
                {
                    code = 999;
                }
            }
        }
    }
}
