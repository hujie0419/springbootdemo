using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.time.expand
    /// </summary>
    public class VmarketEticketTimeExpandRequest : BaseTopRequest<Top.Api.Response.VmarketEticketTimeExpandResponse>
    {
        /// <summary>
        /// 延长天数，延长时间=当前过期时间+延长天数
        /// </summary>
        public Nullable<long> ExpandDays { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.time.expand";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("expand_days", this.ExpandDays);
            parameters.Add("order_id", this.OrderId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("expand_days", this.ExpandDays);
            RequestValidator.ValidateRequired("order_id", this.OrderId);
        }

        #endregion
    }
}
