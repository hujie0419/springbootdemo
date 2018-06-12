using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.item.get
    /// </summary>
    public class AlibabaScmExternalWmsItemGetRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsItemGetResponse>
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// 商家商品编码,不能为空
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 阿里ID,不能为空
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// 货主ID
        /// </summary>
        public string OwnerUserId { get; set; }

        /// <summary>
        /// 卖家身份
        /// </summary>
        public string SellerId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.item.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_type", this.BizType);
            parameters.Add("item_code", this.ItemCode);
            parameters.Add("item_id", this.ItemId);
            parameters.Add("owner_user_id", this.OwnerUserId);
            parameters.Add("seller_id", this.SellerId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("biz_type", this.BizType);
            RequestValidator.ValidateRequired("seller_id", this.SellerId);
        }

        #endregion
    }
}
