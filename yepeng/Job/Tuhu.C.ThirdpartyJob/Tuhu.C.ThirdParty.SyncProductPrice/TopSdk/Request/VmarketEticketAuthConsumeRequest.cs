using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.auth.consume
    /// </summary>
    public class VmarketEticketAuthConsumeRequest : BaseTopRequest<Top.Api.Response.VmarketEticketAuthConsumeResponse>
    {
        /// <summary>
        /// 核销份数
        /// </summary>
        public Nullable<long> ConsumeNum { get; set; }

        /// <summary>
        /// 核销方的ID，如果是普通码商必须传入机具ID,如果是私有码商家（即原有的信任商家）可默认传入私有码商ID
        /// </summary>
        public string Operatorid { get; set; }

        /// <summary>
        /// 自定义核销流水号，需要小于等于100个字符(a-zA-Z0-9_)
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 网点ID,网点授权核销时，必须传入；其他核销方式可不传
        /// </summary>
        public string Storeid { get; set; }

        /// <summary>
        /// 核销的码，只支持单个码，多个码核销需要多次调用
        /// </summary>
        public string VerifyCode { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.auth.consume";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("consume_num", this.ConsumeNum);
            parameters.Add("operatorid", this.Operatorid);
            parameters.Add("serial_num", this.SerialNum);
            parameters.Add("storeid", this.Storeid);
            parameters.Add("verify_code", this.VerifyCode);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("consume_num", this.ConsumeNum);
            RequestValidator.ValidateRequired("operatorid", this.Operatorid);
            RequestValidator.ValidateRequired("serial_num", this.SerialNum);
            RequestValidator.ValidateRequired("verify_code", this.VerifyCode);
        }

        #endregion
    }
}
