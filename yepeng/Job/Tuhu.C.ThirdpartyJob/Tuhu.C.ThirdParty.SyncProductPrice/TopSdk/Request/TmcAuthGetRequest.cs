using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.tmc.auth.get
    /// </summary>
    public class TmcAuthGetRequest : BaseTopRequest<Top.Api.Response.TmcAuthGetResponse>
    {
        /// <summary>
        /// tmc组名
        /// </summary>
        public string Group { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.tmc.auth.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("group", this.Group);
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
