using System;
using Top.Api.Domain;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.package.extend.add
    /// </summary>
    public class VmarketEticketPackageExtendAddRequest : BaseTopRequest<VmarketEticketPackageExtendAddResponse>
    {
        /// <summary>
        /// 包扩展信息
        /// </summary>
        public string PackageExtend { get; set; }

        public PackageExtendDto PackageExtend_{ set{ this.PackageExtend = TopUtils.ObjectToJson(value); } }
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.package.extend.add";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("package_extend", this.PackageExtend);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("package_extend", this.PackageExtend);
        }

        #endregion
    }
}
