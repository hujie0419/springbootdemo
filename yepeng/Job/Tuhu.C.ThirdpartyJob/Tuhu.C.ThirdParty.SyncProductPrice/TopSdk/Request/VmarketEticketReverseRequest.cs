using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.reverse
    /// </summary>
    public class VmarketEticketReverseRequest : BaseTopRequest<Top.Api.Response.VmarketEticketReverseResponse>
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
        /// 进行验码的电子凭证订单的订单ID
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        /// <summary>
        /// 机具id，如果是码商必须传，如果是信任卖家不要传
        /// </summary>
        public string Posid { get; set; }

        /// <summary>
        /// 不需要上传二维码图片或者冲正后不需要变更码的请不要传，需要传入二维码的码商请先调用taobao.vmarket.eticket.qrcode.upload接口，将返回的img_filename文件名称作为参数，多个文件名用逗号隔开且与参数verify_codes按从左到有的顺序一一对应。
        /// </summary>
        public string QrImages { get; set; }

        /// <summary>
        /// 冲正的码，只支持单个码
        /// </summary>
        public string ReverseCode { get; set; }

        /// <summary>
        /// 冲正份数（必须是和被冲正的核销记录的份数一致）
        /// </summary>
        public Nullable<long> ReverseNum { get; set; }

        /// <summary>
        /// 安全验证token，需要和该订单发码通知中的token一致
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 所有冲正后需要重新生成的码和对应的次数。码和次数之间用英文冒号分隔，多个码之间用英文逗号分隔。如果冲正后不需要重新生成码，留空
        /// </summary>
        public string VerifyCodes { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.reverse";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("consume_secial_num", this.ConsumeSecialNum);
            parameters.Add("order_id", this.OrderId);
            parameters.Add("posid", this.Posid);
            parameters.Add("qr_images", this.QrImages);
            parameters.Add("reverse_code", this.ReverseCode);
            parameters.Add("reverse_num", this.ReverseNum);
            parameters.Add("token", this.Token);
            parameters.Add("verify_codes", this.VerifyCodes);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("consume_secial_num", this.ConsumeSecialNum);
            RequestValidator.ValidateRequired("order_id", this.OrderId);
            RequestValidator.ValidateRequired("reverse_code", this.ReverseCode);
            RequestValidator.ValidateRequired("reverse_num", this.ReverseNum);
            RequestValidator.ValidateRequired("token", this.Token);
        }

        #endregion
    }
}
