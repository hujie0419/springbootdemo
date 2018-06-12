using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.store.get
    /// </summary>
    public class VmarketEticketStoreGetRequest : BaseTopRequest<Top.Api.Response.VmarketEticketStoreGetResponse>
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.store.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("order_id", this.OrderId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("order_id", this.OrderId);
        }

        #endregion
    }
}
