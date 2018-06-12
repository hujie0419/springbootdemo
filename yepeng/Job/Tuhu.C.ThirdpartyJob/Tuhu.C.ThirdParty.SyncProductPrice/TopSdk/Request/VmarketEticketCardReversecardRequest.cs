using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.card.reversecard
    /// </summary>
    public class VmarketEticketCardReversecardRequest : BaseTopRequest<VmarketEticketCardReversecardResponse>
    {
        /// <summary>
        /// 买家昵称
        /// </summary>
        public string BuyerNick { get; set; }

        /// <summary>
        /// 卡ID
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// 卡内等级
        /// </summary>
        public Nullable<long> CardLevel { get; set; }

        /// <summary>
        /// 核销时的流水号
        /// </summary>
        public string ConsumeSerialNum { get; set; }

        /// <summary>
        /// 操作员id
        /// </summary>
        public Nullable<long> OperatorId { get; set; }

        /// <summary>
        /// 用户冲正原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 冲正金额
        /// </summary>
        public Nullable<long> ReverseValue { get; set; }

        /// <summary>
        /// 核销时的门店
        /// </summary>
        public Nullable<long> StoreId { get; set; }

        /// <summary>
        /// 安全字段
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.card.reversecard";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("buyer_nick", this.BuyerNick);
            parameters.Add("card_id", this.CardId);
            parameters.Add("card_level", this.CardLevel);
            parameters.Add("consume_serial_num", this.ConsumeSerialNum);
            parameters.Add("operator_id", this.OperatorId);
            parameters.Add("reason", this.Reason);
            parameters.Add("reverse_value", this.ReverseValue);
            parameters.Add("store_id", this.StoreId);
            parameters.Add("token", this.Token);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("buyer_nick", this.BuyerNick);
            RequestValidator.ValidateRequired("card_id", this.CardId);
            RequestValidator.ValidateRequired("card_level", this.CardLevel);
            RequestValidator.ValidateRequired("consume_serial_num", this.ConsumeSerialNum);
            RequestValidator.ValidateRequired("operator_id", this.OperatorId);
            RequestValidator.ValidateRequired("reason", this.Reason);
            RequestValidator.ValidateRequired("reverse_value", this.ReverseValue);
            RequestValidator.ValidateRequired("store_id", this.StoreId);
            RequestValidator.ValidateRequired("token", this.Token);
        }

        #endregion
    }
}
