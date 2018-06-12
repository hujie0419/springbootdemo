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
    public class SyncSkuPriceResponse : QplResponse
    {

        private int code = 999;

        public bool Success { get { return code == 1; } }

        public override bool IsError { get { return code == 999; } }

        public WebApiProductInfoModel [] Products { get; private set; }


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
                    var json = JsonConvert.DeserializeObject<JToken>(value).Value<JToken>("qipeilong_product_search_response").
                         Value<JToken>("productSearchResponse");
                    code = json.Value<int>("code");
                    Products= json.Value<JToken>("product").ToObject<WebApiProductInfoModel[]>();
                }
                catch
                {
                    code = 999;
                }
            }
        }
    }
}
