using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.traderate.impr.imprwords.get
    /// </summary>
    public class TraderateImprImprwordsGetRequest : BaseTopRequest<Top.Api.Response.TraderateImprImprwordsGetResponse>
    {
        /// <summary>
        /// 淘宝叶子类目id
        /// </summary>
        public Nullable<long> CatLeafId { get; set; }

        /// <summary>
        /// 淘宝一级类目id
        /// </summary>
        public Nullable<long> CatRootId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.traderate.impr.imprwords.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("cat_leaf_id", this.CatLeafId);
            parameters.Add("cat_root_id", this.CatRootId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("cat_root_id", this.CatRootId);
        }

        #endregion
    }
}
