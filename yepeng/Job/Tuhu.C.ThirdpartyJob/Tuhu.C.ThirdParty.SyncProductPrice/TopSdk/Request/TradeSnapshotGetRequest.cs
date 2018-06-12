using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.trade.snapshot.get
    /// </summary>
    public class TradeSnapshotGetRequest : BaseTopRequest<TradeSnapshotGetResponse>
    {
        /// <summary>
        /// 需要返回的字段列表。现只支持："snapshot"字段
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// 交易编号
        /// </summary>
        public Nullable<long> Tid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.trade.snapshot.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("fields", this.Fields);
            parameters.Add("tid", this.Tid);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("fields", this.Fields);
            RequestValidator.ValidateRequired("tid", this.Tid);
        }

        #endregion
    }
}
