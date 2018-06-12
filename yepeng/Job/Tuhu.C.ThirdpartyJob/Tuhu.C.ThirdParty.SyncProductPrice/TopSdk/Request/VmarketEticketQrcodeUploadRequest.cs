using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.qrcode.upload
    /// </summary>
    public class VmarketEticketQrcodeUploadRequest : BaseTopRequest<VmarketEticketQrcodeUploadResponse>, ITopUploadRequest<VmarketEticketQrcodeUploadResponse>
    {
        /// <summary>
        /// 码商ID
        /// </summary>
        public Nullable<long> CodeMerchantId { get; set; }

        /// <summary>
        /// 上传的图片byte[]  小于300K，图片尺寸400*400以内
        /// </summary>
        public FileItem ImgBytes { get; set; }

        #region BaseTopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.qrcode.upload";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("code_merchant_id", this.CodeMerchantId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("code_merchant_id", this.CodeMerchantId);
            RequestValidator.ValidateRequired("img_bytes", this.ImgBytes);
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
