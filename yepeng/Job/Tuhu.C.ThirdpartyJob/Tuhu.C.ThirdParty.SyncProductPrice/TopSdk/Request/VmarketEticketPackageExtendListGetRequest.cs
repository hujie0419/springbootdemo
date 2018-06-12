using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.package.extend.list.get
    /// </summary>
    public class VmarketEticketPackageExtendListGetRequest : BaseTopRequest<VmarketEticketPackageExtendListGetResponse>
    {
        /// <summary>
        /// 包id
        /// </summary>
        public Nullable<long> PackageId { get; set; }

        /// <summary>
        /// 分页序号
        /// </summary>
        public Nullable<long> PageNo { get; set; }

        /// <summary>
        /// 分页页码
        /// </summary>
        public Nullable<long> PageSize { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.package.extend.list.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("package_id", this.PackageId);
            parameters.Add("page_no", this.PageNo);
            parameters.Add("page_size", this.PageSize);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("package_id", this.PackageId);
            RequestValidator.ValidateRequired("page_no", this.PageNo);
            RequestValidator.ValidateRequired("page_size", this.PageSize);
        }

        #endregion
    }
}
