using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.baichuan.taoke.trace
    /// </summary>
    public class BaichuanTaokeTraceRequest : BaseTopRequest<Top.Api.Response.BaichuanTaokeTraceResponse>
    {
        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.baichuan.taoke.trace";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("name", this.Name);
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
