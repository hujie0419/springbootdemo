using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.card.consumecard
    /// </summary>
    public class VmarketEticketCardConsumecardRequest : BaseTopRequest<Top.Api.Response.VmarketEticketCardConsumecardResponse>
    {
        /// <summary>
        /// 买家昵称
        /// </summary>
        public string BuyerNick { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// 卡内等级
        /// </summary>
        public Nullable<long> CardLevel { get; set; }

        /// <summary>
        /// 核销code
        /// </summary>
        public string ConsumeCode { get; set; }

        /// <summary>
        /// 核销流水号，外部ISV全局唯一
        /// </summary>
        public string ConsumeSerialNum { get; set; }

        /// <summary>
        /// 核销金额，精确到分，例如1.99元=199
        /// </summary>
        public Nullable<long> ConsumeValue { get; set; }

        /// <summary>
        /// 操作人id
        /// </summary>
        public Nullable<long> OperatorId { get; set; }

        /// <summary>
        /// 核销原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 门店id
        /// </summary>
        public Nullable<long> StoreId { get; set; }

        /// <summary>
        /// 安全token
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.card.consumecard";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("buyer_nick", this.BuyerNick);
            parameters.Add("card_id", this.CardId);
            parameters.Add("card_level", this.CardLevel);
            parameters.Add("consume_code", this.ConsumeCode);
            parameters.Add("consume_serial_num", this.ConsumeSerialNum);
            parameters.Add("consume_value", this.ConsumeValue);
            parameters.Add("operator_id", this.OperatorId);
            parameters.Add("reason", this.Reason);
            parameters.Add("store_id", this.StoreId);
            parameters.Add("token", this.Token);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("buyer_nick", this.BuyerNick);
            RequestValidator.ValidateRequired("card_id", this.CardId);
            RequestValidator.ValidateRequired("card_level", this.CardLevel);
            RequestValidator.ValidateRequired("consume_serial_num", this.ConsumeSerialNum);
            RequestValidator.ValidateRequired("consume_value", this.ConsumeValue);
            RequestValidator.ValidateRequired("operator_id", this.OperatorId);
            RequestValidator.ValidateRequired("reason", this.Reason);
            RequestValidator.ValidateRequired("token", this.Token);
        }

        #endregion
    }
}
