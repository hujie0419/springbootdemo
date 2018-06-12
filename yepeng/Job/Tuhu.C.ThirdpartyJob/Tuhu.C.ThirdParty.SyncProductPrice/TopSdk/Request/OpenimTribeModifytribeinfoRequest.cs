using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.tribe.modifytribeinfo
    /// </summary>
    public class OpenimTribeModifytribeinfoRequest : BaseTopRequest<Top.Api.Response.OpenimTribeModifytribeinfoResponse>
    {
        /// <summary>
        /// 群公告
        /// </summary>
        public string Notice { get; set; }

        /// <summary>
        /// 群id
        /// </summary>
        public Nullable<long> TribeId { get; set; }

        /// <summary>
        /// 群名称
        /// </summary>
        public string TribeName { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public string User { get; set; }

        public OpenImUser User_ { set { this.User = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.tribe.modifytribeinfo";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("notice", this.Notice);
            parameters.Add("tribe_id", this.TribeId);
            parameters.Add("tribe_name", this.TribeName);
            parameters.Add("user", this.User);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("notice", this.Notice);
            RequestValidator.ValidateRequired("tribe_id", this.TribeId);
            RequestValidator.ValidateRequired("tribe_name", this.TribeName);
            RequestValidator.ValidateRequired("user", this.User);
        }

        #endregion
    }
}
