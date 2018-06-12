using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.package.base.list.get
    /// </summary>
    public class VmarketEticketPackageBaseListGetRequest : BaseTopRequest<Top.Api.Response.VmarketEticketPackageBaseListGetResponse>
    {
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.package.base.list.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
        }

        #endregion
    }
}
