using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.codes.get
    /// </summary>
    public class VmarketEticketCodesGetRequest : BaseTopRequest<Top.Api.Response.VmarketEticketCodesGetResponse>
    {
        /// <summary>
        /// 码商ID
        /// </summary>
        public Nullable<long> CodemerchantId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.codes.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("codemerchant_id", this.CodemerchantId);
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
