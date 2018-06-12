using System;
using Top.Api.Domain;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.wlb.wms.sku.info.confirm
    /// </summary>
    public class WlbWmsSkuInfoConfirmRequest : BaseTopRequest<WlbWmsSkuInfoConfirmResponse>
    {
        /// <summary>
        /// 商品资料回告
        /// </summary>
        public string Content { get; set; }

        public WlbWmsSkuInfoConfirm Content_{ set{ this.Content = TopUtils.ObjectToJson(value); } }
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.wlb.wms.sku.info.confirm";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("content", this.Content);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
        }

        #endregion
    }
}
