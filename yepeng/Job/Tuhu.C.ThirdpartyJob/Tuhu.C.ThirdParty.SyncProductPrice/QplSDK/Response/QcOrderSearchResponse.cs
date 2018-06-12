using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qpl.Api.Models;

namespace Qpl.Api.Response
{
    public class QcOrderSearchResponse : QplResponse
    {
        private int code = 999;

        public QcCustomerOrderModel[] Orders { get; private set; }

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
                    Orders = json.Value<JToken>("order_list").ToObject<QcCustomerOrderModel[]>();
                }
                catch (Exception ex)
                {
                    code = 999;
                }
            }
        }
    }
}
