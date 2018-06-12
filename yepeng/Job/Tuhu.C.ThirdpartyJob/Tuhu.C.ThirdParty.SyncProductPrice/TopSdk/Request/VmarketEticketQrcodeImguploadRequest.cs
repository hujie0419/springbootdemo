using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.qrcode.imgupload
    /// </summary>
    public class VmarketEticketQrcodeImguploadRequest : BaseTopRequest<VmarketEticketQrcodeImguploadResponse>, ITopUploadRequest<VmarketEticketQrcodeImguploadResponse>
    {
        /// <summary>
        /// 二维码图片对应的码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 码商ID
        /// </summary>
        public Nullable<long> CodeMerchantId { get; set; }

        /// <summary>
        /// 上传的图片byte[]  小于4K，图片尺寸400*400以内
        /// </summary>
        public FileItem ImgBytes { get; set; }

        /// <summary>
        /// 淘宝交易订单号
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        #region BaseTopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.qrcode.imgupload";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("code", this.Code);
            parameters.Add("code_merchant_id", this.CodeMerchantId);
            parameters.Add("order_id", this.OrderId);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("code", this.Code);
            RequestValidator.ValidateMaxLength("code", this.Code, 20);
            RequestValidator.ValidateRequired("code_merchant_id", this.CodeMerchantId);
            RequestValidator.ValidateRequired("img_bytes", this.ImgBytes);
            RequestValidator.ValidateMaxLength("img_bytes", this.ImgBytes, 4096);
            RequestValidator.ValidateRequired("order_id", this.OrderId);
        }

        #endregion

        #region ITopUploadRequest Members

        public IDictionary<string, FileItem> GetFileParameters()
        {
            IDictionary<string, FileItem> parameters = new Dictionary<string, FileItem>();
            parameters.Add("img_bytes", this.ImgBytes);
            return parameters;
        }

        #endregion
    }
}
