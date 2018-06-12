using System;
using Top.Api.Domain;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.package.extend.update
    /// </summary>
    public class VmarketEticketPackageExtendUpdateRequest : BaseTopRequest<VmarketEticketPackageExtendUpdateResponse>
    {
        /// <summary>
        /// 包扩展信息更新
        /// </summary>
        public string PackageExtendUpdate { get; set; }

        public PackageExtendUpdateDto PackageExtendUpdate_{ set{ this.PackageExtendUpdate = TopUtils.ObjectToJson(value); } }
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.package.extend.update";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("package_extend_update", this.PackageExtendUpdate);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("package_extend_update", this.PackageExtendUpdate);
        }

        #endregion
    }
}
