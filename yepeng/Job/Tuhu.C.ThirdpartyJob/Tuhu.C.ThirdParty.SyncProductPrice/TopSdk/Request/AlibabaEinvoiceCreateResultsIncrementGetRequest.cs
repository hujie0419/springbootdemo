using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.einvoice.create.results.increment.get
    /// </summary>
    public class AlibabaEinvoiceCreateResultsIncrementGetRequest : BaseTopRequest<Top.Api.Response.AlibabaEinvoiceCreateResultsIncrementGetResponse>
    {
        /// <summary>
        /// 终止查询时间
        /// </summary>
        public Nullable<DateTime> EndModified { get; set; }

        /// <summary>
        /// 显示的页码
        /// </summary>
        public Nullable<long> PageNo { get; set; }

        /// <summary>
        /// 页面大小(不能超过200)
        /// </summary>
        public Nullable<long> PageSize { get; set; }

        /// <summary>
        /// 收款方税务登记证号
        /// </summary>
        public string PayeeRegisterNo { get; set; }

        /// <summary>
        /// 起始查询时间
        /// </summary>
        public Nullable<DateTime> StartModified { get; set; }

        /// <summary>
        /// 开票状态 (waiting = 开票中) 、(create_success = 开票成功)、(create_failed = 开票失败)
        /// </summary>
        public string Status { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.einvoice.create.results.increment.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("end_modified", this.EndModified);
            parameters.Add("page_no", this.PageNo);
            parameters.Add("page_size", this.PageSize);
            parameters.Add("payee_register_no", this.PayeeRegisterNo);
            parameters.Add("start_modified", this.StartModified);
            parameters.Add("status", this.Status);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("end_modified", this.EndModified);
            RequestValidator.ValidateRequired("payee_register_no", this.PayeeRegisterNo);
            RequestValidator.ValidateRequired("start_modified", this.StartModified);
        }

        #endregion
    }
}
