using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.relations.get
    /// </summary>
    public class OpenimRelationsGetRequest : BaseTopRequest<Top.Api.Response.OpenimRelationsGetResponse>
    {
        /// <summary>
        /// 查询起始日期。格式yyyyMMdd。不得早于一个月
        /// </summary>
        public string BegDate { get; set; }

        /// <summary>
        /// 查询结束日期。格式yyyyMMdd。不得早于一个月
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public string User { get; set; }

        public OpenImUser User_ { set { this.User = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.relations.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("beg_date", this.BegDate);
            parameters.Add("end_date", this.EndDate);
            parameters.Add("user", this.User);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("beg_date", this.BegDate);
            RequestValidator.ValidateRequired("end_date", this.EndDate);
            RequestValidator.ValidateRequired("user", this.User);
        }

        #endregion
    }
}
