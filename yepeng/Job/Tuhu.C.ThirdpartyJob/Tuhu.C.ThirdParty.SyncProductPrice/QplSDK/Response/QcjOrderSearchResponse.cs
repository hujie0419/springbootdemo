using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qpl.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Response
{
    public class QcjOrderSearchResponse : QplResponse
    {
        private int code = 999;

        public QcjOrderModel[] Orders { get; private set; }

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
                    var json = JsonConvert.DeserializeObject<JToken>(value).Value<JToken>("qipeilong_order_search_response").Value<JToken>("orderSearchResponse");

                    code = json.Value<int>("code");
                    Orders = json.Value<JToken>("order_list").ToObject<QcjOrderModel[]>(new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                }
                catch (Exception ex)
                {
                    code = 999;
                }
            }
        }
    }
}
