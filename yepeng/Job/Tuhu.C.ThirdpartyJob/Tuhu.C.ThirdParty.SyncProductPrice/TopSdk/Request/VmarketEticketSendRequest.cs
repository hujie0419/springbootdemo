using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.send
    /// </summary>
    public class VmarketEticketSendRequest : BaseTopRequest<Top.Api.Response.VmarketEticketSendResponse>
    {
        /// <summary>
        /// 码商ID,是码商的话必须传递,如果是信任卖家,不需要传
        /// </summary>
        public Nullable<long> CodemerchantId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        /// <summary>
        /// 不需要上传二维码图片的码商请不要传，需要传入二维码的码商请先调用taobao.vmarket.eticket.qrcode.upload接口，将返回的img_filename文件名称作为参数，多个文件名用逗号隔开且与参数verify_codes按从左到有的顺序一一对应。
        /// </summary>
        public string QrImages { get; set; }

        /// <summary>
        /// 安全验证token，需要和发码通知中的token一致
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 发送成功的验证码及可验证次数的列表，码和可验证次数用英文冒号分隔，多个码之间用英文逗号分隔，所有字符都为英文半角
        /// </summary>
        public string VerifyCodes { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.send";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("order_id", this.OrderId);
            parameters.Add("qr_images", this.QrImages);
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
            RequestValidator.ValidateRequired("order_id", this.OrderId);
            RequestValidator.ValidateRequired("token", this.Token);
            RequestValidator.ValidateRequired("verify_codes", this.VerifyCodes);
        }

        #endregion
    }
}
