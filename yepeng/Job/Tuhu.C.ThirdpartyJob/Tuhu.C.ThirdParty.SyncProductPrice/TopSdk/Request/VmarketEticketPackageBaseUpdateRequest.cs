using System;
using Top.Api.Domain;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.package.base.update
    /// </summary>
    public class VmarketEticketPackageBaseUpdateRequest : BaseTopRequest<VmarketEticketPackageBaseUpdateResponse>
    {
        /// <summary>
        /// 更新包基本信息
        /// </summary>
        public string PackageBaseUpdate { get; set; }

        public PackageBaseUpdateDto PackageBaseUpdate_{ set{ this.PackageBaseUpdate = TopUtils.ObjectToJson(value); } }
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.package.base.update";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("package_base_update", this.PackageBaseUpdate);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("package_base_update", this.PackageBaseUpdate);
        }

        #endregion
    }
}
