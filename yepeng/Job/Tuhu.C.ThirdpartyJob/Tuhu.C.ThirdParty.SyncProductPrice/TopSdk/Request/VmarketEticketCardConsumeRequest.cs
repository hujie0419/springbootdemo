using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.card.consume
    /// </summary>
    public class VmarketEticketCardConsumeRequest : BaseTopRequest<VmarketEticketCardConsumeResponse>
    {
        /// <summary>
        /// 码商ID,是码商的话必须传递,如果是信任卖家不需要传
        /// </summary>
        public Nullable<long> CodemerchantId { get; set; }

        /// <summary>
        /// 核销份数
        /// </summary>
        public Nullable<long> ConsumeNum { get; set; }

        /// <summary>
        /// 证件的加密串
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 进行验码的电子凭证订单的订单ID
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        /// <summary>
        /// 机具ID(此参数信任卖家可不传递，码商必须传递)
        /// </summary>
        public string Posid { get; set; }

        /// <summary>
        /// 自定义核销流水号，如果核销调用失败，可以用该核销流水号进行冲正操作，需要小于等于100个字符(a-zA-Z0-9_),注意流水号必须保证唯一
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 安全验证token,需要和发码通知中的token一致
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.card.consume";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("consume_num", this.ConsumeNum);
            parameters.Add("id_card", this.IdCard);
            parameters.Add("order_id", this.OrderId);
            parameters.Add("posid", this.Posid);
            parameters.Add("serial_num", this.SerialNum);
            parameters.Add("token", this.Token);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("consume_num", this.ConsumeNum);
            RequestValidator.ValidateRequired("id_card", this.IdCard);
            RequestValidator.ValidateRequired("order_id", this.OrderId);
            RequestValidator.ValidateRequired("serial_num", this.SerialNum);
            RequestValidator.ValidateRequired("token", this.Token);
        }

        #endregion
    }
}
