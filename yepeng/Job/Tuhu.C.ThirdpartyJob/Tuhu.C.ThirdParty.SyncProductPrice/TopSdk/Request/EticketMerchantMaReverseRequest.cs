using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.eticket.merchant.ma.reverse
    /// </summary>
    public class EticketMerchantMaReverseRequest : BaseTopRequest<Top.Api.Response.EticketMerchantMaReverseResponse>
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        public Nullable<long> BizType { get; set; }

        /// <summary>
        /// 码值
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 业务id（订单号）
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 机具编号，如果核销时有则必传
        /// </summary>
        public string PosId { get; set; }

        /// <summary>
        /// 冲正份数，需要与核销份数一致
        /// </summary>
        public Nullable<long> ReverseNum { get; set; }

        /// <summary>
        /// 需要冲正的核销序列号
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 需要跟发码通知获取到的参数一致
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.eticket.merchant.ma.reverse";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_type", this.BizType);
            parameters.Add("code", this.Code);
            parameters.Add("outer_id", this.OuterId);
            parameters.Add("pos_id", this.PosId);
            parameters.Add("reverse_num", this.ReverseNum);
            parameters.Add("serial_num", this.SerialNum);
            parameters.Add("token", this.Token);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("code", this.Code);
            RequestValidator.ValidateRequired("outer_id", this.OuterId);
            RequestValidator.ValidateRequired("reverse_num", this.ReverseNum);
            RequestValidator.ValidateRequired("serial_num", this.SerialNum);
            RequestValidator.ValidateRequired("token", this.Token);
        }

        #endregion
    }
}
