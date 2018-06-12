using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.oplogs.get
    /// </summary>
    public class VmarketEticketOplogsGetRequest : BaseTopRequest<Top.Api.Response.VmarketEticketOplogsGetResponse>
    {
        /// <summary>
        /// 核销码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 码商ID
        /// </summary>
        public Nullable<long> CodemerchantId { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public Nullable<DateTime> EndTime { get; set; }

        /// <summary>
        /// 手机号后四位
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public Nullable<long> PageNo { get; set; }

        /// <summary>
        /// 每页显示的记录数，最大为40，默认为40
        /// </summary>
        public Nullable<long> PageSize { get; set; }

        /// <summary>
        /// 核销身份
        /// </summary>
        public string Posid { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public Nullable<DateTime> StartTime { get; set; }

        /// <summary>
        /// 0:全部 1:核销 2:冲正
        /// </summary>
        public Nullable<long> Type { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.oplogs.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("code", this.Code);
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("end_time", this.EndTime);
            parameters.Add("mobile", this.Mobile);
            parameters.Add("page_no", this.PageNo);
            parameters.Add("page_size", this.PageSize);
            parameters.Add("posid", this.Posid);
            parameters.Add("sort", this.Sort);
            parameters.Add("start_time", this.StartTime);
            parameters.Add("type", this.Type);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("type", this.Type);
        }

        #endregion
    }
}
