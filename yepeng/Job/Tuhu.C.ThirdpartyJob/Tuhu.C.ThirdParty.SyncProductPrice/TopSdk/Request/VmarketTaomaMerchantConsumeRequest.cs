using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.taoma.merchant.consume
    /// </summary>
    public class VmarketTaomaMerchantConsumeRequest : BaseTopRequest<VmarketTaomaMerchantConsumeResponse>
    {
        /// <summary>
        /// 核销份数
        /// </summary>
        public Nullable<long> ConsumeNum { get; set; }

        /// <summary>
        /// 核销操作人员
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 核销流水号
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 核销码
        /// </summary>
        public string VerifyCode { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.taoma.merchant.consume";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("consume_num", this.ConsumeNum);
            parameters.Add("operator", this.Operator);
            parameters.Add("serial_num", this.SerialNum);
            parameters.Add("verify_code", this.VerifyCode);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("consume_num", this.ConsumeNum);
            RequestValidator.ValidateRequired("operator", this.Operator);
            RequestValidator.ValidateRequired("serial_num", this.SerialNum);
            RequestValidator.ValidateRequired("verify_code", this.VerifyCode);
        }

        #endregion
    }
}
