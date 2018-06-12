using System;
using Top.Api.Domain;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.wlb.wms.inventory.status.upload
    /// </summary>
    public class WlbWmsInventoryStatusUploadRequest : BaseTopRequest<WlbWmsInventoryStatusUploadResponse>
    {
        /// <summary>
        /// 库存状态调整
        /// </summary>
        public string Content { get; set; }

        public WlbWmsInventoryStatusUpload Content_{ set{ this.Content = TopUtils.ObjectToJson(value); } }
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.wlb.wms.inventory.status.upload";
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
