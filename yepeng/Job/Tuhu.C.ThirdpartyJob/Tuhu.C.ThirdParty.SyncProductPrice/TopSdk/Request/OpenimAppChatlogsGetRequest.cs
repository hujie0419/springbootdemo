using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.app.chatlogs.get
    /// </summary>
    public class OpenimAppChatlogsGetRequest : BaseTopRequest<Top.Api.Response.OpenimAppChatlogsGetResponse>
    {
        /// <summary>
        /// 查询结束时间。UTC时间。精度到秒
        /// </summary>
        public Nullable<long> Beg { get; set; }

        /// <summary>
        /// 查询最大条数
        /// </summary>
        public Nullable<long> Count { get; set; }

        /// <summary>
        /// 查询结束时间。UTC时间。精度到秒
        /// </summary>
        public Nullable<long> End { get; set; }

        /// <summary>
        /// 迭代key
        /// </summary>
        public string Next { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.app.chatlogs.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("beg", this.Beg);
            parameters.Add("count", this.Count);
            parameters.Add("end", this.End);
            parameters.Add("next", this.Next);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("beg", this.Beg);
            RequestValidator.ValidateRequired("count", this.Count);
            RequestValidator.ValidateMaxValue("count", this.Count, 1000);
            RequestValidator.ValidateMinValue("count", this.Count, 1);
            RequestValidator.ValidateRequired("end", this.End);
        }

        #endregion
    }
}
