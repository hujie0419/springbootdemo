using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.chatlogs.get
    /// </summary>
    public class OpenimChatlogsGetRequest : BaseTopRequest<Top.Api.Response.OpenimChatlogsGetResponse>
    {
        /// <summary>
        /// 查询开始时间（UTC时间）
        /// </summary>
        public Nullable<long> Begin { get; set; }

        /// <summary>
        /// 查询条数
        /// </summary>
        public Nullable<long> Count { get; set; }

        /// <summary>
        /// 查询结束时间（UTC时间）
        /// </summary>
        public Nullable<long> End { get; set; }

        /// <summary>
        /// 迭代key
        /// </summary>
        public string NextKey { get; set; }

        /// <summary>
        /// 用户1信息
        /// </summary>
        public string User1 { get; set; }

        public OpenImUser User1_ { set { this.User1 = TopUtils.ObjectToJson(value); } } 

        /// <summary>
        /// 用户2信息
        /// </summary>
        public string User2 { get; set; }

        public OpenImUser User2_ { set { this.User2 = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.chatlogs.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("begin", this.Begin);
            parameters.Add("count", this.Count);
            parameters.Add("end", this.End);
            parameters.Add("next_key", this.NextKey);
            parameters.Add("user1", this.User1);
            parameters.Add("user2", this.User2);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("begin", this.Begin);
            RequestValidator.ValidateRequired("count", this.Count);
            RequestValidator.ValidateRequired("end", this.End);
            RequestValidator.ValidateRequired("user1", this.User1);
            RequestValidator.ValidateRequired("user2", this.User2);
        }

        #endregion
    }
}
