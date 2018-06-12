using System;
using Top.Api.Domain;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.package.base.create
    /// </summary>
    public class VmarketEticketPackageBaseCreateRequest : BaseTopRequest<VmarketEticketPackageBaseCreateResponse>
    {
        /// <summary>
        /// 包基本信息
        /// </summary>
        public string PackageBase { get; set; }

        public PackageBase PackageBase_{ set{ this.PackageBase = TopUtils.ObjectToJson(value); } }
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.package.base.create";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("package_base", this.PackageBase);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("package_base", this.PackageBase);
        }

        #endregion
    }
}
