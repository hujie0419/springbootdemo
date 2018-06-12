using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.package.remove
    /// </summary>
    public class VmarketEticketPackageRemoveRequest : BaseTopRequest<VmarketEticketPackageRemoveResponse>
    {
        /// <summary>
        /// 包id
        /// </summary>
        public Nullable<long> PackageId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.package.remove";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("package_id", this.PackageId);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
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
