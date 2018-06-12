using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.traderate.impr.imprword.byfeedid.get
    /// </summary>
    public class TraderateImprImprwordByfeedidGetRequest : BaseTopRequest<Top.Api.Response.TraderateImprImprwordByfeedidGetResponse>
    {
        /// <summary>
        /// 交易订单id号（如果包含子订单，请使用子订单id号）
        /// </summary>
        public Nullable<long> ChildTradeId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.traderate.impr.imprword.byfeedid.get";
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
