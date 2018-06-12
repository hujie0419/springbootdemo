using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.card.reverse
    /// </summary>
    public class VmarketEticketCardReverseRequest : BaseTopRequest<VmarketEticketCardReverseResponse>
    {
        /// <summary>
        /// 码商ID，是码商的话必须传递，如果是信任卖家不要传
        /// </summary>
        public Nullable<long> CodemerchantId { get; set; }

        /// <summary>
        /// 需要冲正的核销记录对应核销流水号（对应的核销操作时候传递的自定义流水号）
        /// </summary>
        public string ConsumeSecialNum { get; set; }

        /// <summary>
        /// 证件的加密串
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 进行验码的电子凭证订单的订单ID
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        /// <summary>
        /// 机具id，如果是码商必须传，如果是信任卖家不要传
        /// </summary>
        public string Posid { get; set; }

        /// <summary>
        /// 冲正份数（必须是和被冲正的核销记录的份数一致）
        /// </summary>
        public Nullable<long> ReverseNum { get; set; }

        /// <summary>
        /// 安全验证token，需要和该订单发码通知中的token一致
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.card.reverse";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("consume_secial_num", this.ConsumeSecialNum);
            parameters.Add("id_card", this.IdCard);
            parameters.Add("order_id", this.OrderId);
            parameters.Add("posid", this.Posid);
            parameters.Add("reverse_num", this.ReverseNum);
            parameters.Add("token", this.Token);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("consume_secial_num", this.ConsumeSecialNum);
            RequestValidator.ValidateRequired("id_card", this.IdCard);
            RequestValidator.ValidateRequired("order_id", this.OrderId);
            RequestValidator.ValidateRequired("reverse_num", this.ReverseNum);
            RequestValidator.ValidateRequired("token", this.Token);
        }

        #endregion
    }
}
