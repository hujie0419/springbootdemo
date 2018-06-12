using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.track.getsummary
    /// </summary>
    public class OpenimTrackGetsummaryRequest : BaseTopRequest<Top.Api.Response.OpenimTrackGetsummaryResponse>
    {
        /// <summary>
        /// 用户所属app的prefix
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string Uid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.track.getsummary";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("prefix", this.Prefix);
            parameters.Add("uid", this.Uid);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("uid", this.Uid);
        }

        #endregion
    }
}
