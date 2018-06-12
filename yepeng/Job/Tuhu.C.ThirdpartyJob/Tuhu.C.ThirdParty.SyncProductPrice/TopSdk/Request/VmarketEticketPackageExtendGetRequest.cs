using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.package.extend.get
    /// </summary>
    public class VmarketEticketPackageExtendGetRequest : BaseTopRequest<VmarketEticketPackageExtendGetResponse>
    {
        /// <summary>
        /// 包扩展信息id
        /// </summary>
        public Nullable<long> PackageExtendId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.package.extend.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("package_extend_id", this.PackageExtendId);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("package_extend_id", this.PackageExtendId);
        }

        #endregion
    }
}
