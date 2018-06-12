using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.wlb.item.authorization.delete
    /// </summary>
    public class WlbItemAuthorizationDeleteRequest : BaseTopRequest<WlbItemAuthorizationDeleteResponse>
    {
        /// <summary>
        /// 授权关系ID
        /// </summary>
        public Nullable<long> AuthorizeId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.wlb.item.authorization.delete";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("authorize_id", this.AuthorizeId);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("authorize_id", this.AuthorizeId);
        }

        #endregion
    }
}
