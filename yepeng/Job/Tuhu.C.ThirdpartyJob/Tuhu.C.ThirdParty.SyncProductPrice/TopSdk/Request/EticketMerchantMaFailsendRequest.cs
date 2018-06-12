using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.eticket.merchant.ma.failsend
    /// </summary>
    public class EticketMerchantMaFailsendRequest : BaseTopRequest<Top.Api.Response.EticketMerchantMaFailsendResponse>
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        public Nullable<long> BizType { get; set; }

        /// <summary>
        /// 业务id（订单号）
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 错误原因码
        /// </summary>
        public string SubCode { get; set; }

        /// <summary>
        /// 错误码描述
        /// </summary>
        public string SubMsg { get; set; }

        /// <summary>
        /// 需要与发码通知获取的值一致
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.eticket.merchant.ma.failsend";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_type", this.BizType);
            parameters.Add("outer_id", this.OuterId);
            parameters.Add("sub_code", this.SubCode);
            parameters.Add("sub_msg", this.SubMsg);
            parameters.Add("token", this.Token);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("outer_id", this.OuterId);
            RequestValidator.ValidateRequired("sub_code", this.SubCode);
            RequestValidator.ValidateRequired("sub_msg", this.SubMsg);
            RequestValidator.ValidateRequired("token", this.Token);
        }

        #endregion
    }
}
