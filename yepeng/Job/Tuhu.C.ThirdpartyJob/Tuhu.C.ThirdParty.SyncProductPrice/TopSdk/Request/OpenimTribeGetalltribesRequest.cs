using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.tribe.getalltribes
    /// </summary>
    public class OpenimTribeGetalltribesRequest : BaseTopRequest<Top.Api.Response.OpenimTribeGetalltribesResponse>
    {
        /// <summary>
        /// 群类型
        /// </summary>
        public string TribeTypes { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public string User { get; set; }

        public OpenImUser User_ { set { this.User = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.tribe.getalltribes";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("tribe_types", this.TribeTypes);
            parameters.Add("user", this.User);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("tribe_types", this.TribeTypes);
            RequestValidator.ValidateMaxListSize("tribe_types", this.TribeTypes, 20);
            RequestValidator.ValidateRequired("user", this.User);
        }

        #endregion
    }
}
