using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.failsend
    /// </summary>
    public class VmarketEticketFailsendRequest : BaseTopRequest<Top.Api.Response.VmarketEticketFailsendResponse>
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public Nullable<long> ErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        /// <summary>
        /// 发码通知时的token
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.failsend";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("error_code", this.ErrorCode);
            parameters.Add("error_msg", this.ErrorMsg);
            parameters.Add("order_id", this.OrderId);
            parameters.Add("token", this.Token);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("error_code", this.ErrorCode);
            RequestValidator.ValidateRequired("order_id", this.OrderId);
        }

        #endregion
    }
}
