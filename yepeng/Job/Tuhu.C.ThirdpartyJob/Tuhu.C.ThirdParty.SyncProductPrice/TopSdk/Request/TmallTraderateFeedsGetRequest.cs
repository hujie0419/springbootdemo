using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: tmall.traderate.feeds.get
    /// </summary>
    public class TmallTraderateFeedsGetRequest : BaseTopRequest<Top.Api.Response.TmallTraderateFeedsGetResponse>
    {
        /// <summary>
        /// 交易子订单ID
        /// </summary>
        public Nullable<long> ChildTradeId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "tmall.traderate.feeds.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("child_trade_id", this.ChildTradeId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("child_trade_id", this.ChildTradeId);
        }

        #endregion
    }
}
