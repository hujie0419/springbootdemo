using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.consume
    /// </summary>
    public class VmarketEticketConsumeRequest : BaseTopRequest<Top.Api.Response.VmarketEticketConsumeResponse>
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
        /// 手机后四位(没有特殊说明请不要传该参数)
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 核销后需要重新生成的码，如果不需要重新生成码，不要传该参数
        /// </summary>
        public string NewCode { get; set; }

        /// <summary>
        /// 进行验码的电子凭证订单的订单ID
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        /// <summary>
        /// 机具ID(此参数信任卖家可不传递，码商必须传递)
        /// </summary>
        public string Posid { get; set; }

        /// <summary>
        /// 不需要上传二维码图片或者核销后不需重新生成码码商请不要传，需要传入二维码的码商请先调用taobao.vmarket.eticket.qrcode.upload接口，将返回的img_filename文件名称作为参数（如果二维码不变的话，也可将将发码时传入二维码文件名作为参数传入），文件名与参数new_code必须相互对应。
        /// </summary>
        public string QrImages { get; set; }

        /// <summary>
        /// 自定义核销流水号，如果核销调用失败，可以用该核销流水号进行冲正操作，需要小于等于100个字符(a-zA-Z0-9_)；每次核销都是唯一的流水号
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 安全验证token,需要和发码通知中的token一致
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 核销的码，只支持单个码，多个码核销需要多次调用
        /// </summary>
        public string VerifyCode { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.consume";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("consume_num", this.ConsumeNum);
            parameters.Add("mobile", this.Mobile);
            parameters.Add("new_code", this.NewCode);
            parameters.Add("order_id", this.OrderId);
            parameters.Add("posid", this.Posid);
            parameters.Add("qr_images", this.QrImages);
            parameters.Add("serial_num", this.SerialNum);
            parameters.Add("token", this.Token);
            parameters.Add("verify_code", this.VerifyCode);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("consume_num", this.ConsumeNum);
            RequestValidator.ValidateRequired("order_id", this.OrderId);
            RequestValidator.ValidateRequired("token", this.Token);
            RequestValidator.ValidateRequired("verify_code", this.VerifyCode);
        }

        #endregion
    }
}
