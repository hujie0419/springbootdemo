#region head comment
/*
Code generate by JdSdkTool.
2013-12-21 10:56:32.90854 +08:00
*/
#endregion

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JdSdk.Domain;
using JdSdk.Response;

namespace JdSdk.Request
{
    /// <summary>
    ///  Request
    /// </summary>
    public class JingdongDataserviceShopsalestop15byweekGetRequest : JdRequestBase<JingdongDataserviceShopsalestop15byweekGetResponse>
    {
        public override String ApiName
        {
            get{ return "jingdong.dataservice.shopsalestop15byweek.get"; }
        }

        protected override void PrepareParam(IDictionary<String, Object> paramters)
        {
        }

        public override void Validate()
        {
        }

    }
}
