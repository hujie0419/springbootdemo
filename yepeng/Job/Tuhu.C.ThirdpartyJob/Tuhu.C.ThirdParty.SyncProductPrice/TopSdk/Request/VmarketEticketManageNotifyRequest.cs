using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.manage.notify
    /// </summary>
    public class VmarketEticketManageNotifyRequest : BaseTopRequest<Top.Api.Response.VmarketEticketManageNotifyResponse>
    {
        /// <summary>
        /// 码商ID，如果是码商，必须传，如果是信任卖家，不需要传
        /// </summary>
        public Nullable<long> CodemerchantId { get; set; }

        /// <summary>
        /// 需要调用的通知方法，目前仅支持是send（发码）或resend（重新发码）
        /// </summary>
        public string NotifyMethod { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.manage.notify";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("notify_method", this.NotifyMethod);
            parameters.Add("order_id", this.OrderId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("notify_method", this.NotifyMethod);
            RequestValidator.ValidateRequired("order_id", this.OrderId);
        }

        #endregion
    }
}
