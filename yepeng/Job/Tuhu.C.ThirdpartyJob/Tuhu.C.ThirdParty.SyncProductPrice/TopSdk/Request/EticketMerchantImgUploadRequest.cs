using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.eticket.merchant.img.upload
    /// </summary>
    public class EticketMerchantImgUploadRequest : BaseTopRequest<EticketMerchantImgUploadResponse>, ITopUploadRequest<EticketMerchantImgUploadResponse>
    {
        /// <summary>
        /// 二维码图片
        /// </summary>
        public FileItem ImgBytes { get; set; }

        #region BaseTopRequest Members

        public override string GetApiName()
        {
            return "taobao.eticket.merchant.img.upload";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
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
