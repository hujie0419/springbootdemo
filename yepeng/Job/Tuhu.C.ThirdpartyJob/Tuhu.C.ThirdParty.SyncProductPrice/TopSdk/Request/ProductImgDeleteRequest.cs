using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.product.img.delete
    /// </summary>
    public class ProductImgDeleteRequest : BaseTopRequest<ProductImgDeleteResponse>
    {
        /// <summary>
        /// 非主图ID
        /// </summary>
        public Nullable<long> Id { get; set; }

        /// <summary>
        /// 产品ID.Product的id,通过taobao.product.add接口新增产品的时候会返回id.
        /// </summary>
        public Nullable<long> ProductId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.product.img.delete";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("id", this.Id);
            parameters.Add("product_id", this.ProductId);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("id", this.Id);
            RequestValidator.ValidateRequired("product_id", this.ProductId);
        }

        #endregion
    }
}
