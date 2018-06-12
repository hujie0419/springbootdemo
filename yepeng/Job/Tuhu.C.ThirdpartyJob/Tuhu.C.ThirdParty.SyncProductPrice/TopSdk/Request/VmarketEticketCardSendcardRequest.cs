using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.card.sendcard
    /// </summary>
    public class VmarketEticketCardSendcardRequest : BaseTopRequest<VmarketEticketCardSendcardResponse>
    {
        /// <summary>
        /// 实际金额
        /// </summary>
        public Nullable<long> ActualValue { get; set; }

        /// <summary>
        /// 卡ID
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// 卡内等级
        /// </summary>
        public Nullable<long> CardLevel { get; set; }

        /// <summary>
        /// 码商Id,不填则为sellerId
        /// </summary>
        public Nullable<long> CodemerchantId { get; set; }

        /// <summary>
        /// 膨胀金额
        /// </summary>
        public Nullable<long> ExpandValue { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        /// <summary>
        /// 安全字段
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 买家nick
        /// </summary>
        public string UserNick { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.card.sendcard";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("actual_value", this.ActualValue);
            parameters.Add("card_id", this.CardId);
            parameters.Add("card_level", this.CardLevel);
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("expand_value", this.ExpandValue);
            parameters.Add("order_id", this.OrderId);
            parameters.Add("token", this.Token);
            parameters.Add("user_nick", this.UserNick);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("actual_value", this.ActualValue);
            RequestValidator.ValidateRequired("card_id", this.CardId);
            RequestValidator.ValidateRequired("card_level", this.CardLevel);
            RequestValidator.ValidateRequired("expand_value", this.ExpandValue);
            RequestValidator.ValidateRequired("order_id", this.OrderId);
            RequestValidator.ValidateRequired("token", this.Token);
            RequestValidator.ValidateRequired("user_nick", this.UserNick);
        }

        #endregion
    }
}
