using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.eticket.merchant.ma.available
    /// </summary>
    public class EticketMerchantMaAvailableRequest : BaseTopRequest<Top.Api.Response.EticketMerchantMaAvailableResponse>
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        public Nullable<long> BizType { get; set; }

        /// <summary>
        /// 需要被核销的码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 核销份数
        /// </summary>
        public Nullable<long> ConsumeNum { get; set; }

        /// <summary>
        /// 业务id（订单号）
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 机具编号
        /// </summary>
        public string PosId { get; set; }

        /// <summary>
        /// 核销序列号，需要保证唯一
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 需要跟发码通知获取到的参数一致
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.eticket.merchant.ma.available";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_type", this.BizType);
            parameters.Add("code", this.Code);
            parameters.Add("consume_num", this.ConsumeNum);
            parameters.Add("outer_id", this.OuterId);
            parameters.Add("pos_id", this.PosId);
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
            RequestValidator.ValidateRequired("consume_num", this.ConsumeNum);
            RequestValidator.ValidateRequired("outer_id", this.OuterId);
            RequestValidator.ValidateRequired("token", this.Token);
        }

        #endregion
    }
}
