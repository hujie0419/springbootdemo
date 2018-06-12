using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.package.base.get
    /// </summary>
    public class VmarketEticketPackageBaseGetRequest : BaseTopRequest<Top.Api.Response.VmarketEticketPackageBaseGetResponse>
    {
        /// <summary>
        /// 包id
        /// </summary>
        public Nullable<long> PackageId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.package.base.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("package_id", this.PackageId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("package_id", this.PackageId);
        }

        #endregion
    }
}
