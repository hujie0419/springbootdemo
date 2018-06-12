using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Qpl.Api.Response
{
    public class UserTokenResponse : QplResponse
    {
        private int code = 999;

        public bool Success { get { return code == 1; } }

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
                    code = JsonConvert.DeserializeObject<JToken>(value).Value<JToken>("qipeilong_userinfo_gettoken_response").Value<JToken>("userinfoGetTokenResponse").Value<int>("code");
                }
                catch
                {
                    code = 999;
                }
            }
        }
    }
}
