using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.wangwang.eservice.streamweigths.get
    /// </summary>
    public class WangwangEserviceStreamweigthsGetRequest : BaseTopRequest<WangwangEserviceStreamweigthsGetResponse>
    {
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.wangwang.eservice.streamweigths.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
        }

        #endregion
    }
}
