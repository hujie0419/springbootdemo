using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.track.getdetails
    /// </summary>
    public class OpenimTrackGetdetailsRequest : BaseTopRequest<Top.Api.Response.OpenimTrackGetdetailsResponse>
    {
        /// <summary>
        /// 查询结束时间(UTC时间，以毫秒为单位)
        /// </summary>
        public string Endtime { get; set; }

        /// <summary>
        /// 用户所属app的prefix
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 查询开始时间(UTC时间，以毫秒为单位)
        /// </summary>
        public string Starttime { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string Uid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.track.getdetails";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("endtime", this.Endtime);
            parameters.Add("prefix", this.Prefix);
            parameters.Add("starttime", this.Starttime);
            parameters.Add("uid", this.Uid);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("endtime", this.Endtime);
            RequestValidator.ValidateRequired("starttime", this.Starttime);
            RequestValidator.ValidateRequired("uid", this.Uid);
        }

        #endregion
    }
}
