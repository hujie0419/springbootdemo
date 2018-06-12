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
    public class SyncProductInfoResponse : QplResponse
    {
        private int code = 999;

        public bool Success { get { return code == 1; } }

        public override bool IsError { get { return code == 999; } }

        public QplProductInfoModel Product { get; private set; }


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
                    var json = JsonConvert.DeserializeObject<JToken>(value).Value<JToken>("qipeilong_userproductinfo_search_response").
                         Value<JToken>("userProductSearchResponse");
                    code = json.Value<int>("code");
                    Product = json.Value<JToken>("product").ToObject<QplProductInfoModel>();
                }
                catch
                {
                    code = 999;
                }
            }
        }
    }

   
}
